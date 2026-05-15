using System;
using org.objectweb.asm;

/// <summary>
/// ASM transformer that rewrites Minecraft's render-thread <c>run()</c> method
/// so the body becomes:
///
/// <code>
///     this.gameThread = Thread.currentThread();
///     cli.IkvmBridge.runMinecraftEmLoop(this, "&lt;runTick&gt;", "&lt;running&gt;");
///     return;
/// </code>
///
/// The original <c>while (this.running) { ... runTick(!flag) ... }</c> body
/// (including the OOM-retry / ReportedException / Throwable catch blocks) is
/// discarded. The C# side picks up driving the loop via
/// <c>emscripten_set_main_loop</c>, so each tick runs from
/// <c>requestAnimationFrame</c> instead of a busy-spin.
///
/// CAVEAT — <c>Main.main</c> calls <c>minecraft.stop()</c> /
/// <c>minecraft.destroy()</c> immediately after <c>run()</c> returns. With
/// this transformer, <c>run()</c> returns the moment the em-loop is registered,
/// so those teardown calls fire right away. A second transformer that patches
/// out the post-<c>run()</c> cleanup in <c>Main.main</c> is needed before this
/// is actually usable; this draft only covers the loop rewrite.
/// </summary>
internal sealed class MinecraftRunLoopTransformer : ClassVisitor
{
    public sealed class Mapping
    {
        /// <summary>JVM internal class name (e.g. <c>dlx</c> or <c>net/minecraft/client/Minecraft</c>).</summary>
        public string ClassInternalName { get; init; }
        /// <summary>Method name of <c>run()V</c>.</summary>
        public string RunMethodName { get; init; } = "run";
        /// <summary>Method name of <c>void runTick(boolean)</c>.</summary>
        public string RunTickMethodName { get; init; } = "runTick";
        /// <summary>Field name of the <c>Thread gameThread</c> field.</summary>
        public string GameThreadFieldName { get; init; } = "gameThread";
        /// <summary>Field name of the <c>volatile boolean running</c> field.</summary>
        public string RunningFieldName { get; init; } = "running";
    }

    /// <summary>1.16.1 obfuscated names (from <c>mappings_client_1.16.1.txt</c>).</summary>
    public static readonly Mapping Obfuscated_1_16_1 = new()
    {
        ClassInternalName = "dlx",
        RunMethodName = "e",
        RunTickMethodName = "e",
        GameThreadFieldName = "aM",
        RunningFieldName = "aN",
    };

    /// <summary>Deobfuscated / Mojang-mapped names (matches the decompiled source).</summary>
    public static readonly Mapping Deobfuscated = new()
    {
        ClassInternalName = "net/minecraft/client/Minecraft",
    };

    private readonly Mapping mapping;

    private MinecraftRunLoopTransformer(ClassVisitor downstream, Mapping mapping)
        : base(Opcodes.ASM9, downstream)
    {
        this.mapping = mapping;
    }

    /// <summary>
    /// Build an <c>(filter, buildVisitor)</c> pair suitable for
    /// <see cref="MinecraftLaunchOptions.AsmTransformers"/>.
    /// </summary>
    public static (Predicate<string> Filter, Func<ClassWriter, ClassVisitor> BuildVisitor) AsTransformer(Mapping mapping)
    {
        var dottedName = mapping.ClassInternalName.Replace('/', '.');
        return (
            name => string.Equals(name, dottedName, StringComparison.Ordinal),
            writer => new MinecraftRunLoopTransformer(writer, mapping)
        );
    }

    public override MethodVisitor visitMethod(int access, string name, string descriptor, string signature, string[] exceptions)
    {
        if (name == mapping.RunMethodName && descriptor == "()V")
        {
            Console.WriteLine($"[MinecraftRunLoopTransformer] rewriting {mapping.ClassInternalName}.{name}{descriptor}");
            var mv = base.visitMethod(access, name, descriptor, signature, exceptions);
            EmitReplacementBody(mv);
            // Returning null discards the original method body — the writer
            // keeps the header we already gave it via super.visitMethod.
            return null;
        }
        return base.visitMethod(access, name, descriptor, signature, exceptions);
    }

    private void EmitReplacementBody(MethodVisitor mv)
    {
        mv.visitCode();

        // this.gameThread = Thread.currentThread();
        mv.visitVarInsn(Opcodes.ALOAD, 0);
        mv.visitMethodInsn(Opcodes.INVOKESTATIC, "java/lang/Thread", "currentThread", "()Ljava/lang/Thread;", false);
        mv.visitFieldInsn(Opcodes.PUTFIELD, mapping.ClassInternalName, mapping.GameThreadFieldName, "Ljava/lang/Thread;");

        // cli.IkvmBridge.runMinecraftEmLoop(this, "<runTick>", "<running>");
        mv.visitVarInsn(Opcodes.ALOAD, 0);
        mv.visitLdcInsn(mapping.RunTickMethodName);
        mv.visitLdcInsn(mapping.RunningFieldName);
        mv.visitMethodInsn(
            Opcodes.INVOKESTATIC,
            "cli/IkvmBridge",
            "RunMinecraftEmLoop",
            "(Ljava/lang/Object;Ljava/lang/String;Ljava/lang/String;)V",
            false);

        mv.visitInsn(Opcodes.RETURN);
        // ClassWriter was created with COMPUTE_FRAMES so the args here are ignored.
        mv.visitMaxs(0, 0);
        mv.visitEnd();
    }
}
