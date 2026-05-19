using System;
using org.objectweb.asm;

/// <summary>
/// Lazydfu-style DFU preload disabler for Minecraft 1.16.1 / DFU 4.0.26.
///
/// The upstream lazydfu mod for MC 1.17+ overwrites
/// <c>SharedConstants.enableDataFixerOptimizations()</c> to a no-op, but that
/// method does not exist in 1.16.1 — the type-optimisation work happens
/// unconditionally inside <c>com.mojang.datafixers.DataFixerBuilder.build(Executor)</c>,
/// which loops over every schema/type and submits
/// <c>CompletableFuture.runAsync(...)</c> tasks to pre-cache type representations.
/// That eats minutes of startup time on a slow runtime and pins a lot of memory
/// we never use.
///
/// We rewrite <c>build(Executor)</c> to the body the legacy (pre-1.17) lazydfu
/// mixin used (an inlined <c>buildUnoptimized()</c>): construct a
/// <c>DataFixerUpper</c> from copies of the builder's collections and return it
/// without scheduling any preload. Types are computed lazily the first time
/// they're requested, which is the whole point.
/// </summary>
internal static class LazyDfuTransform
{
    private const string DataFixerBuilderInternal = "com/mojang/datafixers/DataFixerBuilder";
    private const string DataFixerBuilderDotted = "com.mojang.datafixers.DataFixerBuilder";

    private const string DataFixerUpperInternal = "com/mojang/datafixers/DataFixerUpper";
    private const string Int2ObjectSortedMap = "it/unimi/dsi/fastutil/ints/Int2ObjectSortedMap";
    private const string Int2ObjectAVLTreeMap = "it/unimi/dsi/fastutil/ints/Int2ObjectAVLTreeMap";
    private const string IntSortedSet = "it/unimi/dsi/fastutil/ints/IntSortedSet";
    private const string IntAVLTreeSet = "it/unimi/dsi/fastutil/ints/IntAVLTreeSet";

    private const string BuildMethodName = "build";
    private const string BuildMethodDescriptor = "(Ljava/util/concurrent/Executor;)Lcom/mojang/datafixers/DataFixer;";

    public static (Predicate<string> Filter, Func<ClassWriter, ClassVisitor> BuildVisitor) AsTransformer()
    {
        return (
            name => string.Equals(name, DataFixerBuilderDotted, StringComparison.Ordinal),
            writer => new Visitor(writer)
        );
    }

    private sealed class Visitor : ClassVisitor
    {
        public Visitor(ClassVisitor downstream)
            : base(Opcodes.ASM9, downstream) { }

        public override MethodVisitor visitMethod(int access, string name, string descriptor, string signature, string[] exceptions)
        {
            if (name == BuildMethodName && descriptor == BuildMethodDescriptor)
            {
                Console.WriteLine($"[LazyDfuTransform] rewriting {DataFixerBuilderInternal}.{name}{descriptor} to skip type preload");
                var mv = base.visitMethod(access, name, descriptor, signature, exceptions);
                EmitReplacementBody(mv);
                // Discard the original body — the writer already has the header
                // from base.visitMethod above.
                return null;
            }

            return base.visitMethod(access, name, descriptor, signature, exceptions);
        }

        /// <summary>
        /// Emits the equivalent of the original method's first 41 bytecode bytes,
        /// minus the runAsync preload loop:
        /// <code>
        /// return new DataFixerUpper(
        ///     new Int2ObjectAVLTreeMap(this.schemas),
        ///     new ArrayList(this.globalList),
        ///     new IntAVLTreeSet(this.fixerVersions));
        /// </code>
        /// </summary>
        private static void EmitReplacementBody(MethodVisitor mv)
        {
            mv.visitCode();

            // new DataFixerUpper
            mv.visitTypeInsn(Opcodes.NEW, DataFixerUpperInternal);
            mv.visitInsn(Opcodes.DUP);

            // new Int2ObjectAVLTreeMap(this.schemas)
            mv.visitTypeInsn(Opcodes.NEW, Int2ObjectAVLTreeMap);
            mv.visitInsn(Opcodes.DUP);
            mv.visitVarInsn(Opcodes.ALOAD, 0);
            mv.visitFieldInsn(Opcodes.GETFIELD, DataFixerBuilderInternal, "schemas", "L" + Int2ObjectSortedMap + ";");
            mv.visitMethodInsn(Opcodes.INVOKESPECIAL, Int2ObjectAVLTreeMap, "<init>", "(L" + Int2ObjectSortedMap + ";)V", false);

            // new ArrayList(this.globalList)
            mv.visitTypeInsn(Opcodes.NEW, "java/util/ArrayList");
            mv.visitInsn(Opcodes.DUP);
            mv.visitVarInsn(Opcodes.ALOAD, 0);
            mv.visitFieldInsn(Opcodes.GETFIELD, DataFixerBuilderInternal, "globalList", "Ljava/util/List;");
            mv.visitMethodInsn(Opcodes.INVOKESPECIAL, "java/util/ArrayList", "<init>", "(Ljava/util/Collection;)V", false);

            // new IntAVLTreeSet(this.fixerVersions)
            mv.visitTypeInsn(Opcodes.NEW, IntAVLTreeSet);
            mv.visitInsn(Opcodes.DUP);
            mv.visitVarInsn(Opcodes.ALOAD, 0);
            mv.visitFieldInsn(Opcodes.GETFIELD, DataFixerBuilderInternal, "fixerVersions", "L" + IntSortedSet + ";");
            mv.visitMethodInsn(Opcodes.INVOKESPECIAL, IntAVLTreeSet, "<init>", "(L" + IntSortedSet + ";)V", false);

            mv.visitMethodInsn(
                Opcodes.INVOKESPECIAL,
                DataFixerUpperInternal,
                "<init>",
                "(L" + Int2ObjectSortedMap + ";Ljava/util/List;L" + IntSortedSet + ";)V",
                false);

            mv.visitInsn(Opcodes.ARETURN);
            // ClassWriter was created with COMPUTE_FRAMES so these args are ignored.
            mv.visitMaxs(0, 0);
            mv.visitEnd();
        }
    }
}
