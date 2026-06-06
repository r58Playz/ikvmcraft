using System.Linq;
using org.objectweb.asm;
using org.objectweb.asm.tree;

internal class ThereAreNoBareClientsTransform : ClassNode
{
	public static IkvmClassLoaderTransformer Transformer = () => {
		return ([Mappings.CurrentMappings.GetClass("net.minecraft.CrashReport").Name], typeof(ThereAreNoBareClientsTransform));
	};
	ClassWriter Writer;

	public ThereAreNoBareClientsTransform(string name, ClassWriter writer) : base(Opcodes.ASM9)
	{
		Writer = writer;
	}

	public override void visitEnd()
	{
		var build = this.methods.toArray().Cast<MethodNode>()
			.First(x => x.name == Mappings.CurrentMappings.GetClass("net.minecraft.CrashReport").GetMethod("getErrorComment", "()Ljava/lang/String;") && x.desc == "()Ljava/lang/String;");

		var c = new JavaCursor(build);
		c.EmitLdc("there are no bare clients");
		c.EmitAReturn();

		accept(Writer);
	}
}
