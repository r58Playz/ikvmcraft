using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

internal class IkvmPgoProfile
{
    static readonly Type RjmType = typeof(IKVM.Runtime.ByteCodeHelper).Assembly.GetType("IKVM.Runtime.RuntimeJavaMethod");
    static readonly MethodInfo FromExecutable = RjmType.GetMethod("FromExecutable", BindingFlags.NonPublic | BindingFlags.Static);
    static readonly MethodInfo GetMethodM = RjmType.GetMethod("GetMethod", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
    // RuntimeJavaMethod.GetMethod() returns the cached `method`, which stays a MethodBuilder until
    // ResolveMethod() swaps it for the baked RuntimeMethodInfo (whose MethodHandle works). This mirrors
    // IKVM's own ByteCodeHelper.DynamicCreateDelegate: ResolveMethod() then GetMethod(). Requires the
    // declaring type to already be finished (we call FinishType first) so TypeAsTBD is the real type.
    static readonly MethodInfo ResolveMethodM = RjmType.GetMethod("ResolveMethod", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
    static MethodBase ToMethodBase(java.lang.reflect.Executable executable)
    {
        object rjm = FromExecutable.Invoke(null, new object[] { executable });
        ResolveMethodM.Invoke(rjm, null);
        return (MethodBase)GetMethodM.Invoke(rjm, null);
    }

    // IKVM defines Java classes into a dynamic (Reflection.Emit) module. Until the type is *finished*
    // (baked via CreateType), RuntimeJavaMethod.GetMethod() hands back a MethodBuilder, whose
    // MethodHandle throws NotSupported_DynamicModule — so we can't get the MonoMethod*. Forcing the
    // type to finish bakes it, after which GetMethod()/getDeclaredMethods() return real
    // RuntimeMethodInfo with a usable MethodHandle.
    static readonly Type RjtType = typeof(IKVM.Runtime.ByteCodeHelper).Assembly.GetType("IKVM.Runtime.RuntimeJavaType");
    static readonly MethodInfo FromClassM = RjtType.GetMethod("FromClass", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(java.lang.Class) }, null);
    static readonly MethodInfo FinishM = RjtType.GetMethod("Finish", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
    static void FinishType(java.lang.Class klass)
    {
        var rjt = FromClassM.Invoke(null, new object[] { klass });
        FinishM.Invoke(rjt, null);
    }

    // mojmap class name -> array of (mojmap method name, mojmap JVM descriptor)
    private Dictionary<string, (string Method, string Descriptor)[]> Profile;

    // loaded (intermediary/obf) class name -> (its mapping, its profile entries). Built by Prepare()
    // from the mappings alone (no class loading); consumed one class at a time by ResolveDefinedClass
    // as each target class is actually defined.
    private Dictionary<string, (IClassMapping Klazz, (string Method, string Descriptor)[] Entries)> targets;

    internal IkvmPgoProfile(Dictionary<string, (string Method, string Descriptor)[]> profile)
    {
        Profile = profile;
    }

    /// Resolve mojmap class names to their loaded (intermediary/obf) names up front. Only needs the
    /// mappings (already set), not the classes themselves — those aren't loadable until Knot defines
    /// them. Call once, after SetMappings/SetIntermediaryMappings, before the game runs.
    internal void Prepare()
    {
        targets = new();
        foreach (var kvp in Profile)
        {
            var klazz = Mappings.CurrentMappings.GetClass(kvp.Key);
            // Fall back to the literal name for classes outside the mojmap mapping — lithium/fabric mod
            // classes keep their real names at load time, so we can still target them directly.
            if (klazz == null) Console.WriteLine($"[pgo] {kvp.Key}: not mojmap-mapped, matching by literal name");
            targets[klazz?.Name ?? kvp.Key] = (klazz, kvp.Value);
        }
        Console.WriteLine($"[pgo] prepared {targets.Count} target class(es)");
    }

    private readonly object gate = new();

    /// Seed the PGO table for a class the instant it is defined — before any of its methods can be
    /// called and compiled. Driven by the class-define hooks (Knot's defineClassFwd in fabric, the
    /// IkvmClassLoader.defineClass in vanilla), which hand us the freshly-defined Class directly, so
    /// there is no forName / loader / ordering dependency. One-shot per class.
    ///
    /// Returns false if the type couldn't be finished yet (e.g. finishing it pulls in a self-nested
    /// subclass while we're still inside the class's own load) — the caller should retry it once the
    /// class is fully registered and out of its own load stack.
    internal bool ResolveDefinedClass(java.lang.Class klass)
    {
        if (targets == null || targets.Count == 0)
            return true;

        var name = klass.getName();
        lock (gate)
        {
            if (!targets.ContainsKey(name))
                return true;   // not a target, or already seeded
        }

        java.lang.reflect.Method[] declared;
        try
        {
            FinishType(klass);   // bake the dynamic type so methods resolve to RuntimeMethodInfo, not MethodBuilder
            declared = klass.getDeclaredMethods();
        }
        catch (Exception e) { Console.WriteLine($"[pgo] {name}: deferring (finish failed: {e.Message})"); return false; }

        // claim only after a successful finish, so a deferred class stays a target for the retry
        (IClassMapping Klazz, (string Method, string Descriptor)[] Entries) t;
        lock (gate)
        {
            if (!targets.TryGetValue(name, out t))
                return true;
            targets.Remove(name);
        }

        var loader = klass.getClassLoader();    // only needed for the rare overloaded-name param lookup
        var cache = new Dictionary<string, java.lang.Class>();

        int added = 0;
        foreach (var (mojMethod, desc) in t.Entries)
        {
            try
            {
                var loadedName = t.Klazz?.GetMethod(mojMethod, desc);

                // loadedName != null -> a normal mojmap method; resolve it by its mapped name.
                // null means one of: the class is unmapped (lithium/fabric), the method isn't in the
                // mapping (a mixin/mod-added method like getChunkLithium, or a lambda), or it's an
                // interface-declared override (e.g. getBlockState, mapped on BlockGetter not Level).
                // For the first two the loaded name IS the literal name we were given; otherwise fall
                // back to matching the override by its (remapped) signature.
                var method = loadedName != null
                    ? ResolveMethod(klass, declared, loadedName, desc, loader, cache)
                    : ResolveMethodByName(declared, mojMethod)
                        ?? (desc.Contains("(") ? ResolveBySignature(declared, desc, loader, cache) : null);
                if (method == null) { Console.WriteLine($"[pgo] not found {name}.{mojMethod} {desc}"); continue; }

                Emscripten.PgoAddMethod(ToMethodBase(method));
                added++;
            }
            catch (Exception e) { Console.WriteLine($"[pgo] skip {name}.{mojMethod}: {e.Message}"); }
        }
        Console.WriteLine($"[pgo] {name}: tiered {added}/{t.Entries.Length}");
        return true;
    }

    // Literal by-name lookup: used for methods that aren't in the mojmap mapping (mixin/mod-added
    // like getChunkLithium, or lambdas), whose loaded name is exactly the name we were given. Only
    // accepts an unambiguous match; overloads fall through to the signature path.
    private java.lang.reflect.Method ResolveMethodByName(java.lang.reflect.Method[] declared, string name)
    {
        var byName = declared.Where(m => m.getName() == name).ToArray();
        return byName.Length == 1 ? byName[0] : null;
    }

    // Resolve the loaded reflect.Method for `loadedName` on `klass`. A single by-name match is
    // unambiguous (always true under intermediary, and for non-overloaded methods in any mode), so
    // we take it without touching parameters. Otherwise — obfuscated/mojmap builds reuse names
    // across overloads — fall back to an exact parameter-type match parsed from the descriptor.
    private java.lang.reflect.Method ResolveMethod(java.lang.Class klass, java.lang.reflect.Method[] declared, string loadedName, string desc, java.lang.ClassLoader loader, Dictionary<string, java.lang.Class> cache)
    {
        var byName = declared.Where(m => m.getName() == loadedName).ToArray();
        if (byName.Length == 1)
            return byName[0];

        var pcls = ResolveParams(desc, loader, cache);
        // getDeclaredMethod resolves overloads exactly (and prefers the real method over a covariant
        // return bridge); getMethod covers a method inherited from a superclass/interface.
        try { return klass.getDeclaredMethod(loadedName, pcls); }
        catch (java.lang.NoSuchMethodException) { return klass.getMethod(loadedName, pcls); }
    }

    // Find a declared method on the loaded class by its (remapped) intermediary signature, used when
    // the mojmap->intermediary name lookup misses because the method is interface/superclass-declared
    // in the mapping but overridden here under the same intermediary name. Matched on parameter AND
    // return types (return distinguishes e.g. getBlockState from getFluidState — identical params).
    private java.lang.reflect.Method ResolveBySignature(java.lang.reflect.Method[] declared, string desc, java.lang.ClassLoader loader, Dictionary<string, java.lang.Class> cache)
    {
        // Compare by Class *name*, not reference: ResolveToken's forName / primitive TYPE can yield a
        // different Class instance than getParameterTypes()/getReturnType() for the same logical type,
        // which makes reference equality silently miss.
        var pcls = ResolveParams(desc, loader, cache);

        var byParams = new List<java.lang.reflect.Method>();
        foreach (var m in declared)
        {
            if (m.getParameterCount() != pcls.Length)   // cheap pre-filter; avoids resolving every method's types
                continue;

            var ps = m.getParameterTypes();
            bool ok = true;
            for (int i = 0; i < ps.Length; i++)
                if (ps[i].getName() != pcls[i].getName()) { ok = false; break; }

            if (ok)
                byParams.Add(m);
        }

        if (byParams.Count == 0)
            return null;
        if (byParams.Count == 1)
            return byParams[0];   // parameters uniquely identify it (the common case)

        // multiple same-parameter overloads (e.g. getBlockState vs getFluidState — identical params,
        // different return) — disambiguate by return type.
        var retName = ResolveToken(desc[(desc.IndexOf(')') + 1)..], loader, cache).getName();
        java.lang.reflect.Method found = null;
        foreach (var m in byParams)
        {
            if (m.getReturnType().getName() != retName)
                continue;
            if (found != null)
                return null;   // still ambiguous — refuse to guess
            found = m;
        }
        return found;
    }

    // Parse the parameter portion of a (mojmap) JVM descriptor into loaded Class objects.
    private java.lang.Class[] ResolveParams(string desc, java.lang.ClassLoader loader, Dictionary<string, java.lang.Class> cache)
    {
        var tokens = new List<string>();
        int i = desc.IndexOf('(') + 1, end = desc.IndexOf(')');
        while (i < end)
        {
            int s = i;
            while (desc[i] == '[') i++;            // array dims
            if (desc[i] == 'L') i = desc.IndexOf(';', i) + 1; else i++;   // class ref vs primitive
            tokens.Add(desc[s..i]);
        }
        return tokens.Select(t => ResolveToken(t, loader, cache)).ToArray();
    }

    // A single descriptor type token (mojmap-named) -> loaded Class. Primitives map to their TYPE;
    // class/array element names are remapped mojmap->loaded via CurrentMappings (mode-agnostic).
    private java.lang.Class ResolveToken(string tok, java.lang.ClassLoader loader, Dictionary<string, java.lang.Class> cache)
    {
        if (cache.TryGetValue(tok, out var cached))
            return cached;

        java.lang.Class c;
        if (tok[0] == '[')
            c = java.lang.Class.forName(ArrayForName(tok), false, loader);
        else if (tok[0] == 'L')
        {
            var moj = tok[1..^1].Replace('/', '.');
            c = java.lang.Class.forName(Mappings.CurrentMappings.GetClass(moj)?.Name ?? moj, false, loader);
        }
        else
            c = tok switch
            {
                "Z" => java.lang.Boolean.TYPE, "B" => java.lang.Byte.TYPE, "C" => java.lang.Character.TYPE,
                "S" => java.lang.Short.TYPE,   "I" => java.lang.Integer.TYPE, "J" => java.lang.Long.TYPE,
                "F" => java.lang.Float.TYPE,   "D" => java.lang.Double.TYPE,  "V" => java.lang.Void.TYPE,
                _ => throw new ArgumentException($"bad descriptor token '{tok}'"),
            };

        cache[tok] = c;
        return c;
    }

    // forName name for an array token: JVM array form, DOTTED element names, classes remapped
    // mojmap->loaded ("[I" stays "[I"; "[Lnet/minecraft/X;" -> "[Lloaded.name;"). forName arrays
    // use dotted element names, not the descriptor's slashes.
    private string ArrayForName(string tok)
    {
        if (tok[0] == '[')
            return "[" + ArrayForName(tok[1..]);
        if (tok[0] == 'L')
        {
            var moj = tok[1..^1].Replace('/', '.');
            return "L" + (Mappings.CurrentMappings.GetClass(moj)?.Name ?? moj) + ";";
        }
        return tok; // primitive code
    }
}
