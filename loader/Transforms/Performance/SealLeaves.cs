using System;
using System.Collections.Generic;
using System.Linq;
using org.objectweb.asm;
using org.objectweb.asm.tree;

// Marks curated, never-overridden leaf methods ACC_FINAL in the Java bytecode. IKVM then emits them
// with MethodAttributes.Final (RuntimeByteCodeJavaType.JavaTypeImpl: m.IsFinal && m.IsVirtual => Final),
// and the Mono interpreter devirtualizes + INLINES them at tier-1 transform time (transform.c, the
// MONO_METHOD_IS_FINAL devirt path). That eliminates the callvirt at the *bytecode* level — no runtime
// guard, no Call-bailout inflation — which is the call-elimination lever the bench data kept pointing
// at, and the half that speculative devirt couldn't beat (its dispatch/inline still left a call to bail).
//
// SAFETY: only list methods that are NEVER overridden by a subclass.
//   - A genuinely-overridden method => fails LOUD at class-load (IncompatibleClassChangeError), so you
//     see it immediately and remove the entry. No silent corruption.
//   - A wrong mojmap name/descriptor => simply no-ops (the per-class "[seal] sealed N/M" log shows it),
//     so the list is safe to iterate without breaking anything.
// Entries are (mojmap class, mojmap method, mojmap descriptor) — same naming as the PGO profile —
// resolved to loaded (intermediary/yarn) names via Mappings, so this is mapping-mode-agnostic.
internal class SealLeavesTransform : ClassNode
{
	static readonly (string Klass, string Method, string Desc)[] Leaves =
	{
		// --- verified-safe leaves (PGO-derived mojmap, never overridden) -------------------------------
		// coordinate accessors — inlined into every block lookup / index computation.
		("net.minecraft.core.Vec3i", "getX", "()I"),
		("net.minecraft.core.Vec3i", "getY", "()I"),
		("net.minecraft.core.Vec3i", "getZ", "()I"),
		// BlockPos derivations (return a fresh BlockPos; MutableBlockPos does not override these).
		("net.minecraft.core.BlockPos", "relative", "(Lnet/minecraft/core/Direction;)Lnet/minecraft/core/BlockPos;"), // yarn: offset
		("net.minecraft.core.BlockPos", "above", "()Lnet/minecraft/core/BlockPos;"),
		("net.minecraft.core.BlockPos", "below", "()Lnet/minecraft/core/BlockPos;"),                                 // yarn: down
		("net.minecraft.core.BlockPos", "asLong", "()J"),
		// MutableBlockPos in-place moves (MutableBlockPos is a leaf class).
		("net.minecraft.core.BlockPos$MutableBlockPos", "move", "(Lnet/minecraft/core/Direction;I)Lnet/minecraft/core/BlockPos$MutableBlockPos;"),
		("net.minecraft.core.BlockPos$MutableBlockPos", "setWithOffset", "(Lnet/minecraft/core/Vec3i;III)Lnet/minecraft/core/BlockPos$MutableBlockPos;"),
		// chunk section ticking flag — concrete, not overridden.
		("net.minecraft.world.level.chunk.LevelChunkSection", "isRandomlyTicking", "()Z"),
		// GL vertex-state teardown, per draw — concrete VertexFormat method.
		("com.mojang.blaze3d.vertex.VertexFormat", "clearBufferState", "()V"),

		// --- higher-value, verify the mojmap name/desc via the [seal] log (no-op if it misses) ---------
		// world-border bounds check — ~24M hits in the heat; WorldBorder has no subclasses. yarn contains.
		("net.minecraft.world.level.border.WorldBorder", "isWithinBounds", "(Lnet/minecraft/core/BlockPos;)Z"),
		("net.minecraft.world.level.border.WorldBorder", "isWithinBounds", "(Lnet/minecraft/world/phys/AABB;)Z"), // entity collision (lithium)
		// chunk-status compare — yarn isAtLeast; ChunkStatus is a fixed registry (no subclasses).
		("net.minecraft.world.level.chunk.ChunkStatus", "isOrAfter", "(Lnet/minecraft/world/level/chunk/ChunkStatus;)Z"),
		// Vec3i distance check — yarn isWithinDistance; not overridden.
		("net.minecraft.core.Vec3i", "closerThan", "(Lnet/minecraft/core/Vec3i;D)Z"),
		// entity-data read — yarn DataTracker.get; SynchedEntityData is concrete (not subclassed).
		("net.minecraft.network.syncher.SynchedEntityData", "get", "(Lnet/minecraft/network/syncher/EntityDataAccessor;)Ljava/lang/Object;"),
	};

	// loaded class name (== the key IkvmClassLoader matches on) -> [(loaded method name, param count)].
	// Built once from the mojmap list; consumed per-class in visitEnd.
	static Dictionary<string, List<(string Name, int Params)>> Targets;

	static void Build()
	{
		Targets = new();
		foreach (var (k, m, d) in Leaves)
		{
			var klazz = Mappings.CurrentMappings.GetClass(k);
			if (klazz == null) { Console.WriteLine($"[seal] {k}: class not mapped, skipping"); continue; }

			var loaded = klazz.GetMethod(m, d);
			// GetMethod returns the input unchanged (or null) when the (name, desc) pair isn't in the
			// mapping — e.g. an unmapped/mod method, where the loaded name IS the given name.
			if (string.IsNullOrEmpty(loaded)) loaded = m;

			if (!Targets.TryGetValue(klazz.Name, out var list))
				Targets[klazz.Name] = list = new();
			list.Add((loaded, ParamCount(d)));
		}
		Console.WriteLine($"[seal] prepared {Targets.Count} target class(es) from {Leaves.Length} method(s)");
	}

	public static IkvmClassLoaderTransformer Transformer = () =>
	{
		if (Targets == null) Build();
		return (Targets.Keys.ToArray(), typeof(SealLeavesTransform));
	};

	readonly string ClassName;
	readonly ClassWriter Writer;

	public SealLeavesTransform(string name, ClassWriter writer) : base(Opcodes.ASM9)
	{
		ClassName = name;   // exactly the string IkvmClassLoader matched against; matches a Targets key
		Writer = writer;
	}

	public override void visitEnd()
	{
		if (Targets != null && Targets.TryGetValue(ClassName, out var wanted))
		{
			int count = 0;
			foreach (var mn in this.methods.toArray().Cast<MethodNode>())
			{
				// static/private/abstract aren't virtually overridable; already-final is a no-op.
				if ((mn.access & (Opcodes.ACC_STATIC | Opcodes.ACC_PRIVATE | Opcodes.ACC_ABSTRACT | Opcodes.ACC_FINAL)) != 0)
					continue;
				// match on loaded name + parameter count (disambiguates overloads without remapping types).
				if (wanted.Any(w => w.Name == mn.name && w.Params == ParamCount(mn.desc)))
				{
					mn.access |= Opcodes.ACC_FINAL;
					count++;
				}
			}
			Console.WriteLine($"[seal] {ClassName}: sealed {count}/{wanted.Count}");
		}

		accept(Writer);
	}

	// Parameter count of a JVM method descriptor. No type remapping needed (we only count tokens), so
	// it's robust regardless of mapping mode.
	static int ParamCount(string desc)
	{
		int i = desc.IndexOf('(') + 1, end = desc.IndexOf(')'), n = 0;
		while (i < end)
		{
			while (desc[i] == '[') i++;                          // array dims
			if (desc[i] == 'L') i = desc.IndexOf(';', i) + 1; else i++;   // class ref vs primitive
			n++;
		}
		return n;
	}
}
