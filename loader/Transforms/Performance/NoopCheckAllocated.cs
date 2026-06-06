using System.Linq;
using org.objectweb.asm;
using org.objectweb.asm.tree;

internal class NoopCheckAllocatedTransform : ClassNode
{
	public static IkvmClassLoaderTransformer Transformer = () => {
		return ([Mappings.CurrentMappings.GetClass("com.mojang.blaze3d.platform.NativeImage").Name], typeof(NoopCheckAllocatedTransform));
	};
	ClassWriter Writer;

	public NoopCheckAllocatedTransform(string name, ClassWriter writer) : base(Opcodes.ASM9)
	{
		Writer = writer;
	}

	public override void visitEnd()
	{
		var name = Mappings.CurrentMappings.GetClass("com.mojang.blaze3d.platform.NativeImage").GetMethod("checkAllocated", "()V");
		var checkAllocated = this.methods.toArray().Cast<MethodNode>().First(x => x.name == name && x.desc == "()V");

		checkAllocated.instructions.clear();
		checkAllocated.instructions.add(new InsnNode(Opcodes.RETURN));
		if (checkAllocated.tryCatchBlocks != null) checkAllocated.tryCatchBlocks.clear();
		if (checkAllocated.localVariables != null) checkAllocated.localVariables.clear();

		accept(Writer);
	}
}
