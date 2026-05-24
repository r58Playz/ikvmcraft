using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal interface IClassMapping
{
	public string Name { get; }
	public string GetField(string field);
	public string GetMethod(string method);
}

internal interface IMappings
{
	public IClassMapping GetClass(string klass);
}

internal struct MojmapClassMapping : IClassMapping
{
	private static Regex Pattern = new(@"^\s*\d*:\d*:(?<Unmapped>.*) -> (?<Mapped>.*)$");

	public string Unmapped;
	public string Mapped;
	public Dictionary<string, string> Fields;
	public Dictionary<string, string> Methods;

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
				Methods[g["Unmapped"].Value] = g["Mapped"].Value;
				//Console.WriteLine($"[Mojmap]\tmethod {g["Unmapped"].Value} -> {g["Mapped"].Value}");
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
		return Fields[field];
	}

	public string GetMethod(string method)
	{
		return Methods[method];
	}
}

internal class MojmapMappings : IMappings
{
	private static Regex Pattern = new(@"(?<Unmapped>.*) -> (?<Mapped>.*):\n(?<Contents>    .*\n?)*");

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
		return (IClassMapping)Classes[klass];
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

	public static void SetMappings(IMappings mappings)
	{
		_CurrentMappings = mappings;
	}
}
