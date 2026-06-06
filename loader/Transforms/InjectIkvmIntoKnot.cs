using System.Linq;
using org.objectweb.asm;
using org.objectweb.asm.tree;

public static class IkvmKnotBridge
{
	public static byte[] Transform(string name, byte[] array)
	{
		return IkvmClassLoader.LatestInstance.TransformClassBytecode(name, array);
	}

	// Called from the patched tryLoadClass with the class Knot just defined, before it is returned
	// (and thus before any of its methods run). Lets us seed the interp PGO table for hot classes
	// at exactly the right moment, with the real (post-mixin) Class in hand.
	public static void OnClassDefined(java.lang.Class klass)
	{
		IkvmClassLoader.PgoOnClassDefined(klass);
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

		// Seed interp PGO the instant a class is defined: dup the Class returned by defineClassFwd
		// (still on the stack, about to be areturn'd) and hand it to the bridge. This is post-define
		// and pre-first-call, so a hot method is always tiered before mono ever compiles it tier-0.
		c.GotoNext(
			JavaMoveType.After,
			JavaMatch.Method(Opcodes.INVOKEINTERFACE,
				"net/fabricmc/loader/impl/launch/knot/KnotClassDelegate$ClassLoaderAccess",
				"defineClassFwd",
				"(Ljava/lang/String;[BIILjava/security/CodeSource;)Ljava/lang/Class;", true));
		c.Emit(Opcodes.DUP);
		c.EmitMethod(Opcodes.INVOKESTATIC, "cli/IkvmKnotBridge", "OnClassDefined", "(Ljava/lang/Class;)V");

		accept(Writer);
	}
}
