using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

internal interface IClassMapping
{
	public string Name { get; }
	public string GetField(string field);
	public string GetMethod(string method, string signature);
}

internal interface IMappings
{
	public IClassMapping GetClass(string klass);
}

class MojmapClassMapping : IClassMapping
{
	private static Regex Pattern = new(@"^\s*\d*:\d*:(?<Unmapped>.*) -> (?<Mapped>.*)$", RegexOptions.Compiled);

	public string Unmapped;
	public string Mapped;
	public Dictionary<string, string> Fields;
	public Dictionary<(string, string), string> Methods;

	private static string MojmapTypeToJvm(string mojmap)
	{
		if (mojmap.EndsWith("[]"))
			return "[" + MojmapTypeToJvm(mojmap[..^2]);

		return mojmap switch
		{
			"void"    => "V",
			"boolean" => "Z",
			"byte"    => "B",
			"char"    => "C",
			"short"   => "S",
			"int"     => "I",
			"long"    => "J",
			"float"   => "F",
			"double"  => "D",
			_ => $"L{mojmap.Replace(".", "/")};",
		};
	}

	private static (string, string) MojmapToTiny(string mojmap)
	{
		var space = mojmap.IndexOf(" ");
		var retType = mojmap[0..space];
		var rest = mojmap[(space + 1)..];

		var sigStart = rest.IndexOf("(");
		var sigEnd = rest.LastIndexOf(")");
		var paramTypeStr = rest[(sigStart + 1)..sigEnd];
		var name = rest[..sigStart];

		var paramTypes = (paramTypeStr.Length == 0 ? [] : paramTypeStr.Split(",")).Select(x => MojmapTypeToJvm(x));

		return (name, "(" + string.Join("", paramTypes) + ")" + MojmapTypeToJvm(retType)); 
	}

	public MojmapClassMapping(string unmapped, string mapped, Group contents)
	{
		Unmapped = unmapped;
		Mapped = mapped;
		Fields = new();
		Methods = new();

		foreach (Capture c in contents.Captures)
		{
			var val = c.Value;
			var matches = Pattern.Matches(val);
			if (matches.Count > 0)
			{
				var g = matches[0].Groups;
				var unmapped_f = MojmapToTiny(g["Unmapped"].Value);
				var mapped_f = g["Mapped"].Value;
				Methods[unmapped_f] = mapped_f;
				//Console.WriteLine($"[Mojmap]\tmethod {unmapped_f.Item1} {unmapped_f.Item2} -> {mapped_f}");
			}
			else
			{
				var split = val.Split(" -> ");
				var unmapped_f = split[0].Split(" ")[1];
				var mapped_f = split[1].Trim();
				Fields[unmapped_f] = mapped_f;
				//Console.WriteLine($"[Mojmap]\tfield {unmapped_f} -> {mapped_f}");
			}
		}
	}

	public string Name => Mapped;

	public string GetField(string field)
	{
		Fields.TryGetValue(field, out var Field);
		return Field;
	}

	public string GetMethod(string method, string signature)
	{
		Methods.TryGetValue((method, signature), out var Method);
		return Method;
	}
}

internal class MojmapMappings : IMappings
{
	private static Regex Pattern = new(@"(?<Unmapped>.*) -> (?<Mapped>.*):\n(?<Contents>    .*\n?)*", RegexOptions.Compiled);

	private Dictionary<string, MojmapClassMapping> Classes;

	public MojmapMappings(string contents)
	{
		Classes = new();
		foreach (Match m in Pattern.Matches(contents))
		{
			var unmapped = m.Groups["Unmapped"].Value;
			var mapped = m.Groups["Mapped"].Value;
			Classes[unmapped] = new(unmapped, mapped, m.Groups["Contents"]);
			//Console.WriteLine($"[Mojmap] class {unmapped} -> {mapped}");
		}
	}

	public IClassMapping GetClass(string klass)
	{
		Classes.TryGetValue(klass, out var Klass);
		return (IClassMapping)Klass;
	}
}

internal class TinyV1ClassMapping : IClassMapping
{
	public string Unmapped;
	public string Mapped;
	public Dictionary<string, string> Fields;
	public Dictionary<(string, string), string> Methods;

	public string Name => Mapped;

	public string GetField(string field)
	{
		Fields.TryGetValue(field, out var Field);
		return Field;
	}

	public string GetMethod(string method, string signature)
	{
		Methods.TryGetValue((method, signature), out var Method);
		return Method;
	}
}

internal class TinyV1Mappings : IMappings
{
	private Dictionary<string, TinyV1ClassMapping> Classes;

	public static TinyV1Mappings FromResource(java.io.InputStream stream)
	{
        byte[] bytes;
        try
        {
            var output = new java.io.ByteArrayOutputStream();
            var buffer = new byte[8192];
            int n;
            while ((n = stream.read(buffer)) > 0)
            {
                output.write(buffer, 0, n);
            }
            bytes = output.toByteArray();
        }
        finally
        {
            stream.close();
        }

		return new(Encoding.UTF8.GetString(bytes));
	}

	public TinyV1Mappings(string contents)
	{
		Classes = new();

		string unmapped = "";
		string mapped = "";
		Dictionary<string, string> fields = new();
		Dictionary<(string, string), string> methods = new();
		foreach (var line in contents.Split("\n").Skip(1))
		{
			var parts = line.Split("\t");
			switch (parts[0]) {
				case "CLASS":
					if (unmapped != "" && mapped != "") {
						// flush to class list
						Classes[unmapped] = new() {
							Unmapped = unmapped,
							Mapped = mapped,
							Fields = fields,
							Methods = methods,
						};
						fields = new();
						methods = new();
					}
					unmapped = parts[1];
					mapped = parts[2].Replace("/", ".");
					break;

				case "FIELD":
					fields[parts[3]] = parts[4];
					break;

				case "METHOD":
					methods[(parts[3], parts[2])] = parts[4];
					break;
			};
		}
		Classes[unmapped] = new() {
			Unmapped = unmapped,
			Mapped = mapped,
			Fields = fields,
			Methods = methods,
		};
	}

	public IClassMapping GetClass(string klass)
	{
		Classes.TryGetValue(klass, out var Klass);
		return (IClassMapping)Klass;
	}
}

internal struct IntermediaryClassMappings : IClassMapping
{
	private IMappings OneMappings;
	public IClassMapping One;
	public IClassMapping Two;

	public IntermediaryClassMappings(IMappings oneMappings, IClassMapping one, IClassMapping two)
	{
		OneMappings = oneMappings;
		One = one;
		Two = two;
	}

	public string Name => Two.Name;

	public string GetField(string field)
	{
		var one = One.GetField(field);
		if (one == null)
			return null;
		return Two.GetField(one);
	}

	public string GetMethod(string method, string signature)
	{
		var one = One.GetMethod(method, signature);
		if (one == null)
			return null;
		return Two.GetMethod(one, Mappings.RemapClassSignature(OneMappings, signature));
	}
}

internal struct IntermediaryMappings : IMappings
{
	public IMappings One;
	public IMappings Two;

	public IntermediaryMappings(IMappings one, IMappings two)
	{
		One = one;
		Two = two;
	}

	public IClassMapping GetClass(string klass)
	{
		var official = One.GetClass(klass);
		if (official == null)
			return null;

		var intermediary = Two.GetClass(official.Name);
		if (intermediary == null)
			return null;

		return new IntermediaryClassMappings(One, official, intermediary);
	}
}

internal class Mappings
{
	private static IMappings _CurrentMappings = null;
	public static IMappings CurrentMappings {
		get {
			if (_CurrentMappings == null)
				throw new InvalidOperationException("no mappings registered");

			return _CurrentMappings;
		}
	}

	private static Regex ClassPattern = new(@"L(?<Class>[^;]+);", RegexOptions.Compiled);

	internal static string RemapClassSignature(IMappings mappings, string sig)
	{
		return ClassPattern.Replace(sig, match => {
			var unmapped = match.Groups["Class"].Value.Replace("/", ".");
			var klass = mappings.GetClass(unmapped);
			var mapped = klass != null ? klass.Name : unmapped;
			return $"L{mapped.Replace(".", "/")};";
		});
	}

	public static void SetMappings(IMappings mappings)
	{
		_CurrentMappings = mappings;
	}

	public static void SetIntermediaryMappings(IMappings mappings)
	{
		if (_CurrentMappings == null)
			throw new InvalidOperationException("no mappings registered");

		_CurrentMappings = new IntermediaryMappings(_CurrentMappings, mappings);
	}
}
