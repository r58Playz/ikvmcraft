using System.Linq;
using org.objectweb.asm;
using org.objectweb.asm.tree;

internal class NoopAssertThreadTransform : ClassNode
{
	public static IkvmClassLoaderTransformer Transformer = () => {
		return ([Mappings.CurrentMappings.GetClass("com.mojang.blaze3d.systems.RenderSystem").Name], typeof(NoopAssertThreadTransform));
	};
	ClassWriter Writer;

	public NoopAssertThreadTransform(string name, ClassWriter writer) : base(Opcodes.ASM9)
	{
		Writer = writer;
	}

	public override void visitEnd()
	{
		var assertThread = this.methods.toArray().Cast<MethodNode>()
			.First(x => x.name == "assertThread" && x.desc == "(Ljava/util/function/Supplier;)V");

		assertThread.instructions.clear();
		assertThread.instructions.add(new InsnNode(Opcodes.RETURN));
		if (assertThread.tryCatchBlocks != null) assertThread.tryCatchBlocks.clear();
		if (assertThread.localVariables != null) assertThread.localVariables.clear();

		accept(Writer);
	}
}
