using System;
using System.Linq;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using System.Collections.Generic;
using Mono.Cecil;

string profileOut = args[0];
string profileAsm = args[1];

// Filters come in two flavors:
//   "<prefix>"  -> StartsWith match. Legacy behavior: matches the type plus any
//                  sibling/nested type sharing the prefix (e.g. "...Int2ObjectMap"
//                  also catches "...Int2ObjectMaps").
//   "@<path>"   -> read the file; each non-empty, non-'#' line is an EXACT type
//                  name. Matches that type and its nested types ("<name>/..."),
//                  but NOT sibling classes. Use for a precise, hand-curated list.
List<string> prefixFilters = new();
HashSet<string> exactFilters = new();
foreach (var arg in args.Skip(2)) {
	if (arg.StartsWith("@")) {
		var path = arg.Substring(1);
		foreach (var raw in File.ReadAllLines(path)) {
			var line = raw.Trim();
			if (line.Length == 0 || line.StartsWith("#"))
				continue;
			exactFilters.Add(line);
		}
	} else {
		prefixFilters.Add(arg);
	}
}

bool CheckFilters(string name)
{
	foreach (var filter in prefixFilters) {
		if (name.StartsWith(filter))
			return true;
	}

	if (exactFilters.Count > 0) {
		// Exact match against the listed type itself...
		if (exactFilters.Contains(name))
			return true;
		// ...and against nested types, whose Cecil FullName is "<declaring>/<nested>".
		int slash = name.IndexOf('/');
		if (slash >= 0 && exactFilters.Contains(name.Substring(0, slash)))
			return true;
	}

	return false;
}

// Mirrors mono_type_get_desc / mono_signature_full_name so that signatures
// produced here match what the AOT compiler computes for loaded methods.
string MonoTypeName(TypeReference tref)
{
	if (tref is ByReferenceType brt)
		return MonoTypeName(brt.ElementType) + "&";
	if (tref is PointerType pt)
		return MonoTypeName(pt.ElementType) + "*";
	if (tref is OptionalModifierType omt)
		return MonoTypeName(omt.ElementType) + " modopt(" + MonoTypeName(omt.ModifierType) + ")";
	if (tref is RequiredModifierType rmt)
		return MonoTypeName(rmt.ElementType) + " modreq(" + MonoTypeName(rmt.ModifierType) + ")";
	if (tref is ArrayType at) {
		if (at.IsVector)
			return MonoTypeName(at.ElementType) + "[]";
		var sb = new StringBuilder();
		sb.Append(MonoTypeName(at.ElementType));
		sb.Append('[');
		for (int i = 1; i < at.Rank; i++) sb.Append(',');
		sb.Append(']');
		return sb.ToString();
	}
	if (tref is GenericInstanceType git) {
		var sb = new StringBuilder();
		sb.Append(MonoTypeName(git.ElementType));
		sb.Append('<');
		for (int i = 0; i < git.GenericArguments.Count; i++) {
			if (i > 0) sb.Append(", ");
			sb.Append(MonoTypeName(git.GenericArguments[i]));
		}
		sb.Append('>');
		return sb.ToString();
	}
	if (tref is GenericParameter gp) {
		if (!string.IsNullOrEmpty(gp.Name))
			return gp.Name;
		return (gp.Type == GenericParameterType.Type ? "!" : "!!") + gp.Position;
	}

	switch (tref.FullName) {
		case "System.Void": return "void";
		case "System.Boolean": return "bool";
		case "System.Char": return "char";
		case "System.SByte": return "sbyte";
		case "System.Byte": return "byte";
		case "System.Int16": return "int16";
		case "System.UInt16": return "uint16";
		case "System.Int32": return "int";
		case "System.UInt32": return "uint";
		case "System.Int64": return "long";
		case "System.UInt64": return "ulong";
		case "System.IntPtr": return "intptr";
		case "System.UIntPtr": return "uintptr";
		case "System.Single": return "single";
		case "System.Double": return "double";
		case "System.String": return "string";
		case "System.Object": return "object";
	}

	if (tref.IsNested)
		return MonoTypeName(tref.DeclaringType) + "/" + tref.Name;

	if (string.IsNullOrEmpty(tref.Namespace))
		return tref.Name;
	return tref.Namespace + "." + tref.Name;
}

string MonoSignature(MethodDefinition m)
{
	var sb = new StringBuilder();
	sb.Append(MonoTypeName(m.ReturnType));
	sb.Append('(');
	for (int i = 0; i < m.Parameters.Count; i++) {
		if (i > 0) sb.Append(',');
		sb.Append(MonoTypeName(m.Parameters[i].ParameterType));
	}
	sb.Append(')');
	return sb.ToString();
}

string MonoTypeFullName(TypeDefinition td)
{
	if (td.IsNested)
		return MonoTypeName(td.DeclaringType) + "/" + td.Name;
	return td.Namespace + "." + td.Name;
}

IEnumerable<TypeDefinition> WalkTypes(TypeDefinition td)
{
	yield return td;
	foreach (var n in td.NestedTypes)
		foreach (var x in WalkTypes(n))
			yield return x;
}

Console.Error.WriteLine($"Writing aot profile '{profileOut}' for asm '{profileAsm}' with {prefixFilters.Count} prefix filter(s) and {exactFilters.Count} exact filter(s)");

var asm = AssemblyDefinition.ReadAssembly(profileAsm);
var mod = asm.MainModule;

var profileMvid = mod.Mvid.ToString();
var profileName = asm.Name.Name;
List<(TypeDefinition td, List<MethodDefinition> methods)> profileTypes = new();

foreach (var top in mod.Types) {
	foreach (var ty in WalkTypes(top)) {
		if (ty.Name.StartsWith("<"))
			continue;

		if (!CheckFilters(ty.FullName))
			continue;

		var methods = new List<MethodDefinition>();
		foreach (var m in ty.Methods)
			methods.Add(m);

		if (methods.Count > 0) {
			Console.WriteLine($"Got {methods.Count} methods for type '{ty.FullName}'");
			profileTypes.Add((ty, methods));
		}
	}
}

Console.Error.WriteLine($"Collected {profileTypes.Count} types ({profileTypes.Select(x => x.methods.Count).Sum()} methods)");

ArrayBufferWriter<byte> buf = new();
int nextID = 0;

void WriteSpan(int len, Func<Span<byte>, int> func)
{
	Span<byte> span = buf.GetSpan(len);
	buf.Advance(func(span));
}
void WriteByte(byte bite) => buf.Write(new byte[] { bite });
void WriteSInt(int num) => WriteSpan(4, span => { BinaryPrimitives.WriteInt32LittleEndian(span, num); return 4; });
void WriteUtf8(string str, bool length = true)
{
	var bytes = Encoding.UTF8.GetBytes(str);
	if (length)
		WriteSInt(bytes.Length);
	buf.Write(bytes);
}

WriteUtf8("AOTPROFILE", false);
WriteSInt((1 << 16) | 0); // (MAJOR << 16) | MINOR

var imageID = nextID++;
WriteByte(1); // AOTPROF_RECORD_IMAGE
WriteSInt(imageID);
WriteUtf8(profileName);
WriteUtf8(profileMvid);

const byte MONO_TYPE_CLASS = 0x12;

foreach (var (td, methods) in profileTypes)
{
	var typeID = nextID++;
	var typeName = MonoTypeFullName(td);

	WriteByte(2); // AOTPROF_RECORD_TYPE
	WriteSInt(typeID);
	WriteByte(MONO_TYPE_CLASS);
	WriteSInt(imageID);
	WriteSInt(-1); // ginst not supported
	WriteUtf8(typeName);

	foreach (var m in methods)
	{
		var methodID = nextID++;
		var sig = MonoSignature(m);
		WriteByte(4); // AOTPROF_RECORD_METHOD
		WriteSInt(methodID);
		WriteSInt(typeID);
		WriteSInt(-1); // no method ginst
		WriteSInt(m.Parameters.Count);
		WriteUtf8(m.Name);
		WriteUtf8(sig);
	}
}

WriteByte(0); // AOTPROF_RECORD_NONE
WriteSInt(0);

await using var file = File.Create(profileOut);
await file.WriteAsync(buf.WrittenMemory);
