using System;
using org.objectweb.asm;

/// <summary>
/// Wraps Minecraft's <c>Main.main(String[])</c> body in:
///
/// <code>
/// try {
///     // ORIGINAL BODY
/// } catch (cli.IkvmEmLoopStarted m) {
///     throw m;
/// }
/// </code>
///
/// Why this is needed: the original <c>Main.main</c> has a <c>catch (Throwable)</c>
/// around <c>minecraft.run()</c>. Since our <see cref="IkvmEmLoopStarted"/>
/// marker is a <c>Throwable</c>, that inner handler would swallow it before it
/// could reach the C# <c>Run()</c> JSExport — and Main would then continue on
/// to <c>minecraft.stop()</c> / <c>minecraft.destroy()</c>, tearing down the
/// game we just handed off to the em-loop.
///
/// JVM walks the exception table top-to-bottom and uses the first matching
/// entry, so registering our outer try-catch <em>first</em> (before the reader
/// forwards the original method's exception table) preempts the inner
/// <c>catch (Throwable)</c> for our marker. Non-marker throwables still hit
/// the original handlers.
/// </summary>
internal sealed class MainCatchEmLoopTransformer : ClassVisitor
{
    public sealed class Mapping
    {
        public string ClassInternalName { get; init; }
        public string MainMethodName { get; init; } = "main";
        public string MainMethodDescriptor { get; init; } = "([Ljava/lang/String;)V";
    }

    /// <summary>Minecraft 1.16.1 — <c>Main</c> stays in its source-level package because it's the launch entry point.</summary>
    public static readonly Mapping Default = new()
    {
        ClassInternalName = "net/minecraft/client/main/Main",
    };

    internal const string MarkerInternalName = "cli/IkvmEmLoopStarted";

    private readonly Mapping mapping;

    private MainCatchEmLoopTransformer(ClassVisitor downstream, Mapping mapping)
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
            writer => new MainCatchEmLoopTransformer(writer, mapping)
        );
    }

    public override MethodVisitor visitMethod(int access, string name, string descriptor, string signature, string[] exceptions)
    {
        var inner = base.visitMethod(access, name, descriptor, signature, exceptions);
        if (name == mapping.MainMethodName && descriptor == mapping.MainMethodDescriptor)
        {
            Console.WriteLine($"[MainCatchEmLoopTransformer] wrapping {mapping.ClassInternalName}.{name}{descriptor}");
            return new BodyWrappingMethodVisitor(inner);
        }
        return inner;
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
