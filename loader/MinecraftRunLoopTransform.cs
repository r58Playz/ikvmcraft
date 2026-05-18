using System;
using org.objectweb.asm;

/// <summary>
/// Single transformer that rewrites Minecraft's render loop and wraps Main.main
/// so the em-loop marker can escape Main's inner catch(Throwable).
/// </summary>
internal sealed class MinecraftRunLoopTransform : ClassVisitor
{
    internal const string MarkerInternalName = "cli/IkvmEmLoopStarted";

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
        /// <summary>Main class internal name (e.g. <c>net/minecraft/client/main/Main</c>).</summary>
        public string MainClassInternalName { get; init; } = "net/minecraft/client/main/Main";
        /// <summary>Main method name.</summary>
        public string MainMethodName { get; init; } = "main";
        /// <summary>Main method descriptor.</summary>
        public string MainMethodDescriptor { get; init; } = "([Ljava/lang/String;)V";
    }

    /// <summary>1.16.1 obfuscated names (from <c>mappings_client_1.16.1.txt</c>).</summary>
    public static readonly Mapping Obfuscated_1_16_1 = new()
    {
        ClassInternalName = "dlx",
        RunMethodName = "e",
        RunTickMethodName = "e",
        GameThreadFieldName = "aM",
        RunningFieldName = "aN",
        MainClassInternalName = "net/minecraft/client/main/Main",
    };

    /// <summary>Deobfuscated / Mojang-mapped names (matches the decompiled source).</summary>
    public static readonly Mapping Deobfuscated = new()
    {
        ClassInternalName = "net/minecraft/client/Minecraft",
        MainClassInternalName = "net/minecraft/client/main/Main",
    };

    private readonly Mapping mapping;

    private MinecraftRunLoopTransform(ClassVisitor downstream, Mapping mapping)
        : base(Opcodes.ASM9, downstream)
    {
        this.mapping = mapping;
    }

    /// <summary>
    /// Build a transformer for a specific mapping.
    /// </summary>
    public static (Predicate<string> Filter, Func<ClassWriter, ClassVisitor> BuildVisitor) AsTransformer(Mapping mapping)
    {
        var runLoopName = mapping.ClassInternalName.Replace('/', '.');
        var mainName = mapping.MainClassInternalName.Replace('/', '.');
        return (
            name => string.Equals(name, runLoopName, StringComparison.Ordinal)
                || string.Equals(name, mainName, StringComparison.Ordinal),
            writer => new MinecraftRunLoopTransform(writer, mapping)
        );
    }

    public override MethodVisitor visitMethod(int access, string name, string descriptor, string signature, string[] exceptions)
    {
        if (name == mapping.RunMethodName && descriptor == "()V")
        {
            Console.WriteLine($"[MinecraftRunLoopTransform] rewriting {mapping.ClassInternalName}.{name}{descriptor}");
            var mv = base.visitMethod(access, name, descriptor, signature, exceptions);
            EmitReplacementBody(mv);
            // Returning null discards the original method body — the writer
            // keeps the header we already gave it via super.visitMethod.
            return null;
        }

        var inner = base.visitMethod(access, name, descriptor, signature, exceptions);
        if (name == mapping.MainMethodName && descriptor == mapping.MainMethodDescriptor)
        {
            Console.WriteLine($"[MinecraftRunLoopTransform] wrapping {mapping.MainClassInternalName}.{name}{descriptor}");
            return new BodyWrappingMethodVisitor(inner);
        }
        return inner;
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

    private sealed class BodyWrappingMethodVisitor : MethodVisitor
    {
        private readonly Label startLabel = new();
        private readonly Label endLabel = new();
        private readonly Label handlerLabel = new();

        public BodyWrappingMethodVisitor(MethodVisitor downstream)
            : base(Opcodes.ASM9, downstream) { }

        public override void visitCode()
        {
            base.visitCode();
            // Outer try-catch registered FIRST so it ranks first in the
            // exception table and preempts the original catch(Throwable)
            // for our marker type.
            base.visitTryCatchBlock(startLabel, endLabel, handlerLabel, MarkerInternalName);
            base.visitLabel(startLabel);
        }

        public override void visitMaxs(int maxStack, int maxLocals)
        {
            // After all original instructions: close the protected range,
            // emit the handler, re-throw the marker so it propagates out of
            // main() and into the reflective invoker in C# Run().
            base.visitLabel(endLabel);
            base.visitLabel(handlerLabel);
            base.visitInsn(Opcodes.ATHROW);
            // COMPUTE_FRAMES (set on the ClassWriter) recomputes maxs.
            base.visitMaxs(0, 0);
        }
    }
}
