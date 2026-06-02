using System;
using org.objectweb.asm;
using org.objectweb.asm.tree;

internal enum JavaMoveType
{
	Before,
	AfterLabels,
	After,
}

// Captures are embedded in the match closures and reused across every start
// index a single GotoNext scan tries. Without a per-attempt reset, a value
// captured during a failed partial match would poison all later attempts (e.g.
// the first ALoad capture locking onto the method's leading aload_0). Each
// attempt bumps the generation; a capture set in an older generation is treated
// as unset, so same-value constraints only bind within one attempt.
internal static class JavaMatchScope
{
	[System.ThreadStatic]
	private static int generation;

	public static int Generation => generation;

	public static void NextAttempt()
	{
		generation++;
	}
}

internal sealed class JavaCapture<T>
{
	public bool HasValue { get; private set; }
	public T Value { get; private set; }
	private int generation = -1;

	public bool Match(T value)
	{
		if (!HasValue || generation != JavaMatchScope.Generation)
		{
			Value = value;
			HasValue = true;
			generation = JavaMatchScope.Generation;
			return true;
		}

		return Equals(Value, value);
	}

	public void Reset()
	{
		HasValue = false;
		Value = default;
		generation = -1;
	}

	public static implicit operator T(JavaCapture<T> capture)
	{
		return capture.Value;
	}
}

internal delegate bool JavaInsnMatch(AbstractInsnNode insn);

internal static class JavaMatch
{
	public static JavaInsnMatch Op(int opcode)
	{
		return insn => insn.getOpcode() == opcode;
	}

	public static JavaInsnMatch Insn(int opcode)
	{
		return insn => insn is InsnNode && insn.getOpcode() == opcode;
	}

	public static JavaInsnMatch Var(int opcode, int? local = null, JavaCapture<int> localCapture = null)
	{
		return insn => insn is VarInsnNode varInsn
			&& varInsn.getOpcode() == opcode
			&& MatchValue(localCapture, varInsn.@var)
			&& (!local.HasValue || varInsn.@var == local.Value);
	}

	public static JavaInsnMatch Var(int opcode, out JavaCapture<int> local)
	{
		local = new();
		return Var(opcode, null, local);
	}

	public static JavaInsnMatch ALoad(int? local = null)
	{
		return Var(Opcodes.ALOAD, local);
	}

	public static JavaInsnMatch ALoad(JavaCapture<int> local)
	{
		return Var(Opcodes.ALOAD, null, local);
	}

	public static JavaInsnMatch ALoad(out JavaCapture<int> local)
	{
		local = new();
		return ALoad(local);
	}

	public static JavaInsnMatch AStore(int? local = null)
	{
		return Var(Opcodes.ASTORE, local);
	}

	public static JavaInsnMatch AStore(JavaCapture<int> local)
	{
		return Var(Opcodes.ASTORE, null, local);
	}

	public static JavaInsnMatch AStore(out JavaCapture<int> local)
	{
		local = new();
		return AStore(local);
	}

	public static JavaInsnMatch ILoad(int? local = null)
	{
		return Var(Opcodes.ILOAD, local);
	}

	public static JavaInsnMatch ILoad(JavaCapture<int> local)
	{
		return Var(Opcodes.ILOAD, null, local);
	}

	public static JavaInsnMatch ILoad(out JavaCapture<int> local)
	{
		local = new();
		return ILoad(local);
	}

	public static JavaInsnMatch IStore(int? local = null)
	{
		return Var(Opcodes.ISTORE, local);
	}

	public static JavaInsnMatch IStore(JavaCapture<int> local)
	{
		return Var(Opcodes.ISTORE, null, local);
	}

	public static JavaInsnMatch IStore(out JavaCapture<int> local)
	{
		local = new();
		return IStore(local);
	}

	public static JavaInsnMatch Int(int opcode, int? operand = null, JavaCapture<int> operandCapture = null)
	{
		return insn => insn is IntInsnNode intInsn
			&& intInsn.getOpcode() == opcode
			&& MatchValue(operandCapture, intInsn.operand)
			&& (!operand.HasValue || intInsn.operand == operand.Value);
	}

	public static JavaInsnMatch Int(int opcode, out JavaCapture<int> operand)
	{
		operand = new();
		return Int(opcode, null, operand);
	}

	public static JavaInsnMatch Iinc(int? local = null, int? increment = null, JavaCapture<int> localCapture = null, JavaCapture<int> incrementCapture = null)
	{
		return insn => insn is IincInsnNode iinc
			&& MatchValue(localCapture, iinc.@var)
			&& MatchValue(incrementCapture, iinc.incr)
			&& (!local.HasValue || iinc.@var == local.Value)
			&& (!increment.HasValue || iinc.incr == increment.Value);
	}

	public static JavaInsnMatch Type(int opcode, string desc = null, JavaCapture<string> descCapture = null)
	{
		return insn => insn is TypeInsnNode typeInsn
			&& typeInsn.getOpcode() == opcode
			&& MatchValue(descCapture, typeInsn.desc)
			&& (desc is null || typeInsn.desc == desc);
	}

	public static JavaInsnMatch Type(int opcode, out JavaCapture<string> desc)
	{
		desc = new();
		return Type(opcode, null, desc);
	}

	public static JavaInsnMatch Field(int opcode, string owner = null, string name = null, string desc = null, JavaCapture<string> ownerCapture = null, JavaCapture<string> nameCapture = null, JavaCapture<string> descCapture = null)
	{
		return insn => insn is FieldInsnNode fieldInsn
			&& fieldInsn.getOpcode() == opcode
			&& MatchValue(ownerCapture, fieldInsn.owner)
			&& MatchValue(nameCapture, fieldInsn.name)
			&& MatchValue(descCapture, fieldInsn.desc)
			&& (owner is null || fieldInsn.owner == owner)
			&& (name is null || fieldInsn.name == name)
			&& (desc is null || fieldInsn.desc == desc);
	}

	public static JavaInsnMatch Field(int opcode, out JavaCapture<string> owner, out JavaCapture<string> name, out JavaCapture<string> desc)
	{
		owner = new();
		name = new();
		desc = new();
		return Field(opcode, null, null, null, owner, name, desc);
	}

	public static JavaInsnMatch Method(int opcode, string owner = null, string name = null, string desc = null, bool? isInterface = null, JavaCapture<string> ownerCapture = null, JavaCapture<string> nameCapture = null, JavaCapture<string> descCapture = null, JavaCapture<bool> interfaceCapture = null)
	{
		return insn => insn is MethodInsnNode methodInsn
			&& methodInsn.getOpcode() == opcode
			&& MatchValue(ownerCapture, methodInsn.owner)
			&& MatchValue(nameCapture, methodInsn.name)
			&& MatchValue(descCapture, methodInsn.desc)
			&& MatchValue(interfaceCapture, methodInsn.itf)
			&& (owner is null || methodInsn.owner == owner)
			&& (name is null || methodInsn.name == name)
			&& (desc is null || methodInsn.desc == desc)
			&& (!isInterface.HasValue || methodInsn.itf == isInterface.Value);
	}

	public static JavaInsnMatch Method(int opcode, out JavaCapture<string> owner, out JavaCapture<string> name, out JavaCapture<string> desc, out JavaCapture<bool> isInterface)
	{
		owner = new();
		name = new();
		desc = new();
		isInterface = new();
		return Method(opcode, null, null, null, null, owner, name, desc, isInterface);
	}

	public static JavaInsnMatch InvokeVirtual(string owner = null, string name = null, string desc = null)
	{
		return Method(Opcodes.INVOKEVIRTUAL, owner, name, desc, false);
	}

	public static JavaInsnMatch InvokeStatic(string owner = null, string name = null, string desc = null)
	{
		return Method(Opcodes.INVOKESTATIC, owner, name, desc, false);
	}

	public static JavaInsnMatch InvokeSpecial(string owner = null, string name = null, string desc = null)
	{
		return Method(Opcodes.INVOKESPECIAL, owner, name, desc, false);
	}

	public static JavaInsnMatch InvokeSpecial(out JavaCapture<string> owner, out JavaCapture<string> name, out JavaCapture<string> desc)
	{
		owner = new();
		name = new();
		desc = new();
		return Method(Opcodes.INVOKESPECIAL, null, null, null, false, owner, name, desc);
	}

	public static JavaInsnMatch Jump(int opcode, LabelNode label = null, JavaCapture<LabelNode> labelCapture = null)
	{
		return insn => insn is JumpInsnNode jumpInsn
			&& jumpInsn.getOpcode() == opcode
			&& MatchReference(labelCapture, jumpInsn.label)
			&& (label is null || ReferenceEquals(jumpInsn.label, label));
	}

	public static JavaInsnMatch Jump(int opcode, out JavaCapture<LabelNode> label)
	{
		label = new();
		return Jump(opcode, null, label);
	}

	public static JavaInsnMatch Ldc(object cst = null, JavaCapture<object> cstCapture = null)
	{
		return insn => insn is LdcInsnNode ldcInsn
			&& MatchValue(cstCapture, ldcInsn.cst)
			&& (cst is null || Equals(ldcInsn.cst, cst));
	}

	public static JavaInsnMatch Ldc(out JavaCapture<object> cst)
	{
		cst = new();
		return Ldc(null, cst);
	}

	public static JavaInsnMatch InvokeDynamic(string name = null, string desc = null, Handle bootstrapMethod = null, JavaCapture<string> nameCapture = null, JavaCapture<string> descCapture = null, JavaCapture<Handle> bootstrapCapture = null)
	{
		return insn => insn is InvokeDynamicInsnNode indyInsn
			&& MatchValue(nameCapture, indyInsn.name)
			&& MatchValue(descCapture, indyInsn.desc)
			&& MatchReference(bootstrapCapture, indyInsn.bsm)
			&& (name is null || indyInsn.name == name)
			&& (desc is null || indyInsn.desc == desc)
			&& (bootstrapMethod is null || Equals(indyInsn.bsm, bootstrapMethod));
	}

	public static JavaInsnMatch MultiANewArray(string desc = null, int? dims = null, JavaCapture<string> descCapture = null, JavaCapture<int> dimsCapture = null)
	{
		return insn => insn is MultiANewArrayInsnNode arrayInsn
			&& MatchValue(descCapture, arrayInsn.desc)
			&& MatchValue(dimsCapture, arrayInsn.dims)
			&& (desc is null || arrayInsn.desc == desc)
			&& (!dims.HasValue || arrayInsn.dims == dims.Value);
	}

	private static bool MatchValue<T>(JavaCapture<T> capture, T value)
	{
		return capture is null || capture.Match(value);
	}

	private static bool MatchReference<T>(JavaCapture<T> capture, T value) where T : class
	{
		return capture is null || capture.Match(value);
	}
}

internal sealed class JavaCursor
{
	private readonly InsnList instructions;
	private AbstractInsnNode lastMatchStart;
	private AbstractInsnNode lastMatchEnd;

	public JavaCursor(MethodNode method)
	{
		instructions = method.instructions;
		Index = 0;
	}

	public int Index { get; private set; }

	public AbstractInsnNode Next => Index < instructions.size() ? instructions.get(Index) : null;

	public AbstractInsnNode Prev => Index > 0 ? instructions.get(Index - 1) : null;

	public JavaCursor Clone()
	{
		return new JavaCursor(instructions, Index, lastMatchStart, lastMatchEnd);
	}

	private JavaCursor(InsnList instructions, int index, AbstractInsnNode lastMatchStart, AbstractInsnNode lastMatchEnd)
	{
		this.instructions = instructions;
		Index = index;
		this.lastMatchStart = lastMatchStart;
		this.lastMatchEnd = lastMatchEnd;
	}

	public bool TryGotoNext(JavaMoveType moveType, params JavaInsnMatch[] pattern)
	{
		if (pattern.Length == 0)
			throw new ArgumentException("pattern must not be empty", nameof(pattern));

		for (var i = Index; i < instructions.size(); i++)
		{
			JavaMatchScope.NextAttempt();
			if (!TryMatchForward(i, pattern, out var firstMatch, out var lastMatch))
				continue;

			lastMatchStart = firstMatch;
			lastMatchEnd = lastMatch;
			Index = GetMoveIndex(firstMatch, lastMatch, moveType);
			return true;
		}

		return false;
	}

	public JavaCursor GotoNext(JavaMoveType moveType, params JavaInsnMatch[] pattern)
	{
		if (!TryGotoNext(moveType, pattern))
			throw new InvalidOperationException("pattern not found");

		return this;
	}

	public JavaCursor Emit(params AbstractInsnNode[] nodes)
	{
		foreach (var node in nodes)
		{
			InsertNode(node);
		}

		return this;
	}

	public JavaCursor Emit(int opcode)
	{
		return Emit(new InsnNode(opcode));
	}

	public JavaCursor EmitVar(int opcode, int local)
	{
		return Emit(new VarInsnNode(opcode, local));
	}

	public JavaCursor EmitInt(int opcode, int operand)
	{
		return Emit(new IntInsnNode(opcode, operand));
	}

	public JavaCursor EmitIinc(int local, int increment)
	{
		return Emit(new IincInsnNode(local, increment));
	}

	public JavaCursor EmitType(int opcode, string desc)
	{
		return Emit(new TypeInsnNode(opcode, desc));
	}

	public JavaCursor EmitField(int opcode, string owner, string name, string desc)
	{
		return Emit(new FieldInsnNode(opcode, owner, name, desc));
	}

	public JavaCursor EmitMethod(int opcode, string owner, string name, string desc, bool isInterface = false)
	{
		return Emit(new MethodInsnNode(opcode, owner, name, desc, isInterface));
	}

	public JavaCursor EmitJump(int opcode, LabelNode label)
	{
		return Emit(new JumpInsnNode(opcode, label));
	}

	public JavaCursor EmitLdc(object cst)
	{
		return Emit(new LdcInsnNode(cst));
	}

	public JavaCursor EmitInvokeDynamic(string name, string desc, Handle bootstrapMethod, params object[] bootstrapArgs)
	{
		return Emit(new InvokeDynamicInsnNode(name, desc, bootstrapMethod, bootstrapArgs));
	}

	public JavaCursor EmitMultiANewArray(string desc, int dims)
	{
		return Emit(new MultiANewArrayInsnNode(desc, dims));
	}

	public JavaCursor EmitLabel(LabelNode label)
	{
		return Emit(label);
	}

	public JavaCursor EmitALoad(int local)
	{
		return EmitVar(Opcodes.ALOAD, local);
	}

	public JavaCursor EmitAStore(int local)
	{
		return EmitVar(Opcodes.ASTORE, local);
	}

	public JavaCursor EmitReturn()
	{
		return Emit(Opcodes.RETURN);
	}

	public JavaCursor EmitAReturn()
	{
		return Emit(Opcodes.ARETURN);
	}

	public JavaCursor Remove()
	{
		if (Next is null)
			throw new InvalidOperationException("cursor is at end of instruction list");

		var removed = Next;
		instructions.remove(removed);
		if (lastMatchStart == removed || lastMatchEnd == removed)
		{
			lastMatchStart = null;
			lastMatchEnd = null;
		}

		return this;
	}

	public JavaCursor RemoveRange()
	{
		if (lastMatchStart is null || lastMatchEnd is null)
			throw new InvalidOperationException("no previous match to remove");

		return RemoveRange(lastMatchStart, lastMatchEnd);
	}

	public JavaCursor RemoveRange(int instructionCount)
	{
		if (instructionCount <= 0)
			throw new ArgumentOutOfRangeException(nameof(instructionCount));

		var start = Next ?? throw new InvalidOperationException("cursor is at end of instruction list");
		var matched = 0;
		AbstractInsnNode end = null;
		for (var node = start; node is not null; node = node.getNext())
		{
			end = node;
			if (node.getOpcode() != -1)
			{
				matched++;
				if (matched == instructionCount)
					break;
			}
		}

		if (matched != instructionCount || end is null)
			throw new InvalidOperationException("not enough instructions to remove");

		return RemoveRange(start, end);
	}

	public JavaCursor RemoveRange(AbstractInsnNode start, AbstractInsnNode end)
	{
		var node = start;
		while (node is not null)
		{
			var next = node.getNext();
			instructions.remove(node);
			if (ReferenceEquals(node, end))
				break;
			node = next;
		}

		lastMatchStart = null;
		lastMatchEnd = null;
		if (Index > instructions.size())
			Index = instructions.size();

		return this;
	}

	private void InsertNode(AbstractInsnNode node)
	{
		if (Next is null)
		{
			instructions.add(node);
			Index = instructions.size();
			return;
		}

		instructions.insertBefore(Next, node);
		Index++;
	}

	private bool TryMatchForward(int startIndex, JavaInsnMatch[] pattern, out AbstractInsnNode firstMatch, out AbstractInsnNode lastMatch)
	{
		firstMatch = null;
		lastMatch = null;
		var node = GetNode(startIndex);

		foreach (var match in pattern)
		{
			node = SkipNonInstructions(node);
			if (node is null || !match(node))
				return false;

			firstMatch ??= node;
			lastMatch = node;
			node = node.getNext();
		}

		return firstMatch is not null;
	}

	private int GetMoveIndex(AbstractInsnNode firstMatch, AbstractInsnNode lastMatch, JavaMoveType moveType)
	{
		return moveType switch
		{
			JavaMoveType.Before => instructions.indexOf(RewindNonInstructions(firstMatch)),
			JavaMoveType.AfterLabels => instructions.indexOf(firstMatch),
			JavaMoveType.After => instructions.indexOf(lastMatch) + 1,
			_ => throw new InvalidOperationException($"unknown move type: {moveType}"),
		};
	}

	private AbstractInsnNode GetNode(int index)
	{
		return index >= 0 && index < instructions.size() ? instructions.get(index) : null;
	}

	private static AbstractInsnNode SkipNonInstructions(AbstractInsnNode node)
	{
		while (node is not null && node.getOpcode() == -1)
			node = node.getNext();

		return node;
	}

	private static AbstractInsnNode RewindNonInstructions(AbstractInsnNode node)
	{
		while (node.getPrevious() is not null && node.getPrevious().getOpcode() == -1)
			node = node.getPrevious();

		return node;
	}
}
