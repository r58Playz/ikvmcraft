using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class IkvmcManifest
{
	public IkvmcBundle[] Bundles { get; init; } = Array.Empty<IkvmcBundle>();

	private const string EmbeddedResourceName = "IkvmWasm.ikvmc-manifest.json";

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		ReadCommentHandling = JsonCommentHandling.Skip,
	};

	public static IkvmcManifest LoadEmbedded()
	{
		var assembly = typeof(IkvmcManifest).Assembly;
		using var stream = assembly.GetManifestResourceStream(EmbeddedResourceName);
		if (stream is null)
		{
			var available = string.Join(", ", assembly.GetManifestResourceNames());
			throw new InvalidOperationException(
				$"Embedded manifest '{EmbeddedResourceName}' not found in assembly. Available: [{available}]. "
				+ "Did scripts/build-ikvmc.py run before dotnet publish?");
		}

		var manifest = JsonSerializer.Deserialize<IkvmcManifest>(stream, JsonOptions)
			?? throw new InvalidDataException("ikvmc manifest deserialized to null.");

		manifest.Validate();
		return manifest;
	}

	private void Validate()
	{
		foreach (var bundle in Bundles)
		{
			if (string.IsNullOrWhiteSpace(bundle.AssemblyName))
			{
				throw new InvalidDataException($"ikvmc bundle '{bundle.Name}' is missing assemblyName.");
			}
			if (bundle.Prefixes is null || bundle.Prefixes.Length == 0)
			{
				throw new InvalidDataException($"ikvmc bundle '{bundle.Name}' has no class prefixes.");
			}
		}
	}

	/// <summary>
	/// Bootstrap classloader configuration: every <see cref="IkvmcBundle.AlwaysReplace"/> bundle
	/// (typically lwjgl3) gets included; version-matched bundles are not active until
	/// <see cref="MatchVersion(System.Text.Json.JsonElement)"/> is called.
	/// </summary>
	public IkvmClassLoaderDll[] AlwaysReplaceDlls()
	{
		return (from bundle in Bundles
				where bundle.AlwaysReplace
				select (bundle.Prefixes, bundle.AssemblyName)).ToArray();
	}

	/// <summary>
	/// Match this manifest against an MC version JSON's "libraries" array. A bundle activates
	/// when every maven coord declared in it appears verbatim (group:artifact:version) in the
	/// version's libraries; otherwise the bundle's classes are left to URLClassLoader (JIT). The
	/// returned <paramref name="jarsToSkip"/> is the set of relative paths whose URLClassLoader
	/// entry should be dropped because their classes will come from the AOT'd DLL instead.
	/// </summary>
	public IkvmcMatchResult MatchVersion(JsonElement versionRoot)
	{
		var availableGavs = new HashSet<string>(StringComparer.Ordinal);
		if (versionRoot.TryGetProperty("libraries", out var librariesNode) && librariesNode.ValueKind == JsonValueKind.Array)
		{
			foreach (var libraryNode in librariesNode.EnumerateArray())
			{
				if (libraryNode.ValueKind != JsonValueKind.Object) continue;
				if (!libraryNode.TryGetProperty("name", out var nameNode) || nameNode.ValueKind != JsonValueKind.String) continue;

				var coords = ParseMavenCoords(nameNode.GetString());
				if (coords is null) continue;
				availableGavs.Add(coords);
			}
		}

		var activeDlls = new List<IkvmClassLoaderDll>();
		var jarsToSkip = new HashSet<string>(StringComparer.Ordinal);

		foreach (var bundle in Bundles)
		{
			if (bundle.AlwaysReplace)
			{
				activeDlls.Add((bundle.Prefixes, bundle.AssemblyName));
				AddHiddenJars(bundle, jarsToSkip);
				continue;
			}

			if (bundle.Jars.Length == 0)
			{
				continue;
			}

			bool allMatch = true;
			foreach (var jar in bundle.Jars)
			{
				var gav = $"{jar.Group}:{jar.Artifact}:{jar.Version}";
				if (!availableGavs.Contains(gav))
				{
					allMatch = false;
					break;
				}
			}

			if (!allMatch)
			{
				continue;
			}

			activeDlls.Add((bundle.Prefixes, bundle.AssemblyName));
			foreach (var jar in bundle.Jars)
			{
				if (!string.IsNullOrWhiteSpace(jar.RelativePath))
				{
					jarsToSkip.Add(jar.RelativePath);
				}
			}
			AddHiddenJars(bundle, jarsToSkip);
		}

		return new IkvmcMatchResult(activeDlls.ToArray(), jarsToSkip);
	}

	/// <summary>
	/// Drop a bundle's declared <see cref="IkvmcBundle.HideJars"/> from the JIT classpath. These are
	/// jars that ship a subset of classes under the bundle's prefixes (e.g. patchy's patched
	/// io.netty.bootstrap.Bootstrap); left on the classpath they split the runtime package between the
	/// AOT bundle and URLClassLoader/Knot and trigger IllegalAccessError.
	/// </summary>
	private static void AddHiddenJars(IkvmcBundle bundle, HashSet<string> jarsToSkip)
	{
		foreach (var jar in bundle.HideJars)
		{
			if (!string.IsNullOrWhiteSpace(jar.RelativePath))
			{
				jarsToSkip.Add(jar.RelativePath);
			}
		}
	}

	private static string ParseMavenCoords(string name)
	{
		if (string.IsNullOrWhiteSpace(name)) return null;
		var parts = name.Split(':');
		if (parts.Length < 3) return null;
		// MC version json uses "group:artifact:version[:classifier]"; we match on G:A:V only,
		// matching how the bundle's jars list declares its inputs.
		return $"{parts[0]}:{parts[1]}:{parts[2]}";
	}
}

internal sealed class IkvmcBundle
{
	public string Name { get; init; } = string.Empty;
	public string AssemblyName { get; init; } = string.Empty;
	public string[] Prefixes { get; init; } = Array.Empty<string>();
	public bool AlwaysReplace { get; init; }
	public IkvmcBundleJar[] Jars { get; init; } = Array.Empty<IkvmcBundleJar>();
	public IkvmcBundleJar[] HideJars { get; init; } = Array.Empty<IkvmcBundleJar>();
}

internal sealed class IkvmcBundleJar
{
	public string Group { get; init; } = string.Empty;
	public string Artifact { get; init; } = string.Empty;
	public string Version { get; init; } = string.Empty;
	public string RelativePath { get; init; } = string.Empty;
}

internal readonly struct IkvmcMatchResult
{
	public IkvmClassLoaderDll[] ActiveDlls { get; }
	public HashSet<string> JarsToSkip { get; }

	public IkvmcMatchResult(IkvmClassLoaderDll[] activeDlls, HashSet<string> jarsToSkip)
	{
		ActiveDlls = activeDlls;
		JarsToSkip = jarsToSkip;
	}
}
