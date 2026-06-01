using System.Linq;
using org.objectweb.asm;
using org.objectweb.asm.tree;

public static class IkvmKnotBridge
{
	public static byte[] Transform(string name, byte[] array)
	{
		return IkvmClassLoader.LatestInstance.TransformClassBytecode(name, array);
	}
}

internal class InjectIkvmIntoKnotTransform : ClassNode
{
	public static IkvmClassLoaderTransformer Transformer = () => {
		return (["net.fabricmc.loader.impl.launch.knot.KnotClassDelegate"], typeof(InjectIkvmIntoKnotTransform));
	};
	ClassWriter Writer;

	public InjectIkvmIntoKnotTransform(string name, ClassWriter writer) : base(Opcodes.ASM9)
	{
		Writer = writer;
	}

	public override void visitEnd()
	{
		var tryLoadClass = this.methods.toArray().Cast<MethodNode>()
			.First(x => x.name == "tryLoadClass" && x.desc == "(Ljava/lang/String;Z)Ljava/lang/Class;");

		var c = new JavaCursor(tryLoadClass);
		c.GotoNext(
			JavaMoveType.After,
			JavaMatch.ALoad(out var nameLocal),
			JavaMatch.ILoad(),
			JavaMatch.InvokeSpecial(null, "getPostMixinClassByteArray", "(Ljava/lang/String;Z)[B"),
			JavaMatch.AStore(out var bytearrayLocal));

		var skip = new LabelNode();
		c.EmitALoad(nameLocal);
		c.EmitJump(Opcodes.IFNULL, skip);
		c.EmitALoad(nameLocal);
		c.EmitALoad(bytearrayLocal);
		c.EmitMethod(Opcodes.INVOKESTATIC, "cli/IkvmKnotBridge", "Transform", "(Ljava/lang/String;[B)[B");
		c.EmitAStore(bytearrayLocal);
		c.EmitLabel(skip);

		accept(Writer);
	}
}
