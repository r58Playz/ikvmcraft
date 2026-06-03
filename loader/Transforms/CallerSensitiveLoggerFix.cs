using org.objectweb.asm;

// claude'd logmanager pass to make sure loggers everywhere work even when aot frames are skipped
// maybe we should fix stackwalking in aot instead
internal static class CallerSensitiveLoggerFix
{
	private const string LogManager = "org/apache/logging/log4j/LogManager";
	private const string NoArgDesc = "()Lorg/apache/logging/log4j/Logger;";
	private const string ClassArgDesc = "(Ljava/lang/Class;)Lorg/apache/logging/log4j/Logger;";

	public static byte[] Apply(byte[] bytes)
	{
		// Cheap reject: only pay for an ASM round-trip when the class actually references LogManager
		// (its internal name lands in the constant pool verbatim as ASCII UTF-8).
		if (!ContainsAscii(bytes, LogManager))
			return bytes;

		var reader = new ClassReader(bytes);
		// COMPUTE_MAXS only: we insert an LDC before an existing invokestatic within a basic block, which
		// can bump max stack by 1 but never adds a frame boundary, so the existing StackMapTable stays
		// valid. (COMPUTE_FRAMES would force loading referenced types — too costly/fragile to do globally.)
		var writer = new ClassWriter(reader, ClassWriter.COMPUTE_MAXS);
		reader.accept(new Visitor(writer), 0);
		return writer.toByteArray();
	}

	private sealed class Visitor : ClassVisitor
	{
		private string Owner;

		public Visitor(ClassVisitor next) : base(Opcodes.ASM9, next) { }

		public override void visit(int version, int access, string name, string signature, string superName, string[] interfaces)
		{
			Owner = name;
			base.visit(version, access, name, signature, superName, interfaces);
		}

		public override MethodVisitor visitMethod(int access, string name, string descriptor, string signature, string[] exceptions)
		{
			var mv = base.visitMethod(access, name, descriptor, signature, exceptions);
			return mv == null ? null : new MethodPatcher(mv, Owner);
		}
	}

	private sealed class MethodPatcher : MethodVisitor
	{
		private readonly string Owner;

		public MethodPatcher(MethodVisitor next, string owner) : base(Opcodes.ASM9, next) => Owner = owner;

		public override void visitMethodInsn(int opcode, string owner, string name, string descriptor, bool isInterface)
		{
			if (opcode == Opcodes.INVOKESTATIC
				&& owner == LogManager
				&& descriptor == NoArgDesc
				&& (name == "getLogger" || name == "getFormatterLogger"))
			{
				// getLogger() -> getLogger(ThisClass.class)
				base.visitLdcInsn(Type.getObjectType(Owner));
				base.visitMethodInsn(Opcodes.INVOKESTATIC, LogManager, name, ClassArgDesc, false);
				return;
			}
			base.visitMethodInsn(opcode, owner, name, descriptor, isInterface);
		}
	}

	private static bool ContainsAscii(byte[] haystack, string needle)
	{
		int n = needle.Length;
		if (n == 0 || haystack.Length < n)
			return false;
		for (int i = 0; i <= haystack.Length - n; i++)
		{
			int j = 0;
			while (j < n && haystack[i + j] == (byte)needle[j])
				j++;
			if (j == n)
				return true;
		}
		return false;
	}
}
