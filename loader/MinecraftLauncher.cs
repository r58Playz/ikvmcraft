using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

internal sealed class MinecraftLaunchOptions
{
	public string VersionJsonPath { get; init; } = string.Empty;
	public string VersionJarPath { get; init; } = string.Empty;
	public string LibraryDirectoryPath { get; init; } = string.Empty;
	public string AssetsRootPath { get; init; } = string.Empty;
	public string GameDirectoryPath { get; init; } = string.Empty;

	public string MainClassOverride { get; init; }

	public string PlayerName { get; init; } = "Player";
	public string Uuid { get; init; } = string.Empty;
	public string AccessToken { get; init; } = "0";
	public string UserType { get; init; } = "legacy";
	public string UserPropertiesJson { get; init; } = "{}";

	public int Width { get; init; } = 854;
	public int Height { get; init; } = 480;

	public string NativesDirectoryPath { get; init; } = "/tmp/lwjgl";
	public string MinecraftOsName { get; init; } = "linux";

	public IReadOnlyDictionary<string, bool> FeatureFlags { get; init; } = new Dictionary<string, bool>(StringComparer.Ordinal);
	public IReadOnlyList<string> ExtraGameArguments { get; init; } = Array.Empty<string>();
	public IReadOnlyDictionary<string, string> ExtraLaunchVariables { get; init; } = new Dictionary<string, string>(StringComparer.Ordinal);
	public IReadOnlyDictionary<string, string> SystemProperties { get; init; } = new Dictionary<string, string>(StringComparer.Ordinal);
	public IReadOnlyList<IkvmClassLoaderDll> ManagedAssemblyNames { get; init; } = Array.Empty<IkvmClassLoaderDll>();

	/// <summary>
	/// Raw bytecode transformers applied to every class resolved through the URL
	/// classpath, in order. Use for full-control byte[] rewrites.
	/// </summary>
	public IReadOnlyList<ClassFileTransformer> ClassTransformers { get; init; } = Array.Empty<ClassFileTransformer>();

	/// <summary>
	/// ASM-based transformers. Each entry pairs a class-name filter with a
	/// builder that wraps the downstream <c>ClassWriter</c> in a visitor chain.
	/// Bundled ASM (ikvmc_asm.dll) is on the classpath, so visitor subclasses
	/// can be written directly in C#.
	/// </summary>
	public IReadOnlyList<(Predicate<string> Filter, Func<org.objectweb.asm.ClassWriter, org.objectweb.asm.ClassVisitor> BuildVisitor)> AsmTransformers { get; init; }
		= Array.Empty<(Predicate<string>, Func<org.objectweb.asm.ClassWriter, org.objectweb.asm.ClassVisitor>)>();
}

internal sealed class MinecraftLaunchPlan
{
	public string VersionId { get; init; } = string.Empty;
	public string MainClassName { get; init; } = string.Empty;
	public string AssetIndexId { get; init; } = string.Empty;
	public string[] ClassPathJars { get; init; } = Array.Empty<string>();
	public string[] GameArguments { get; init; } = Array.Empty<string>();
}

internal static class MinecraftLauncher
{
	private static readonly Regex LaunchVariablePattern = new("\\$\\{(?<name>[a-zA-Z0-9_]+)\\}", RegexOptions.Compiled);

	public static MinecraftLaunchPlan BuildLaunchPlan(MinecraftLaunchOptions options)
	{
		if (options is null)
		{
			throw new ArgumentNullException(nameof(options));
		}

		var versionJsonPath = RequireNonEmpty(options.VersionJsonPath, nameof(options.VersionJsonPath));
		var versionJarPath = RequireNonEmpty(options.VersionJarPath, nameof(options.VersionJarPath));
		var libraryDirectoryPath = RequireNonEmpty(options.LibraryDirectoryPath, nameof(options.LibraryDirectoryPath));
		var assetsRootPath = RequireNonEmpty(options.AssetsRootPath, nameof(options.AssetsRootPath));
		var gameDirectoryPath = RequireNonEmpty(options.GameDirectoryPath, nameof(options.GameDirectoryPath));

		if (!File.Exists(versionJsonPath))
		{
			throw new FileNotFoundException($"Version JSON not found: {versionJsonPath}", versionJsonPath);
		}

		if (!File.Exists(versionJarPath))
		{
			throw new FileNotFoundException($"Version JAR not found: {versionJarPath}", versionJarPath);
		}

		if (!Directory.Exists(assetsRootPath))
		{
			throw new DirectoryNotFoundException($"Assets root directory not found: {assetsRootPath}");
		}

		if (!Directory.Exists(libraryDirectoryPath))
		{
			throw new DirectoryNotFoundException($"Library directory not found: {libraryDirectoryPath}");
		}

		Directory.CreateDirectory(gameDirectoryPath);
		Directory.CreateDirectory(options.NativesDirectoryPath);

		using var versionJson = JsonDocument.Parse(File.ReadAllText(versionJsonPath));
		var root = versionJson.RootElement;

		var versionId = ReadString(root, "id") ?? Path.GetFileNameWithoutExtension(versionJarPath);
		if (string.IsNullOrWhiteSpace(versionId))
		{
			throw new InvalidDataException("Version metadata is missing 'id'.");
		}

		var versionType = ReadString(root, "type") ?? "release";
		var mainClassName = options.MainClassOverride ?? ReadString(root, "mainClass") ?? "net.minecraft.client.main.Main";
		var assetIndexId = ReadAssetIndexId(root, versionId);

		var featureFlags = BuildFeatureFlags(options.FeatureFlags);
		var launchVariables = BuildLaunchVariables(options, versionId, versionType, assetIndexId, assetsRootPath, gameDirectoryPath, libraryDirectoryPath);

		if (options.ExtraLaunchVariables is not null)
		{
			foreach (var pair in options.ExtraLaunchVariables)
			{
				launchVariables[pair.Key] = pair.Value;
			}
		}

		var gameArguments = ResolveGameArguments(root, launchVariables, featureFlags, options.MinecraftOsName);
		gameArguments.Add("--width");
		gameArguments.Add(options.Width.ToString());
		gameArguments.Add("--height");
		gameArguments.Add(options.Height.ToString());

		if (options.ExtraGameArguments is not null)
		{
			foreach (var argument in options.ExtraGameArguments)
			{
				if (!string.IsNullOrWhiteSpace(argument))
				{
					gameArguments.Add(argument);
				}
			}
		}

		var classPathJars = BuildClassPath(root, libraryDirectoryPath, versionJarPath, options.MinecraftOsName, featureFlags);

		return new MinecraftLaunchPlan
		{
			VersionId = versionId,
			MainClassName = mainClassName,
			AssetIndexId = assetIndexId,
			ClassPathJars = classPathJars,
			GameArguments = gameArguments.ToArray(),
		};
	}

	public static void LaunchVanilla(MinecraftLaunchOptions options)
	{
		LaunchVanillaSetup(options);
		var plan = BuildLaunchPlan(options);
		InvokeMain(plan.MainClassName, plan.GameArguments);
	}

	/// <summary>
	/// Set up the Minecraft classpath, native search paths, and system properties
	/// — but do NOT call the main method. Lets external code (e.g. tests, repro
	/// drivers) drive specific classes via Java reflection without running the
	/// full game launch flow.
	/// </summary>
	public static void LaunchVanillaSetup(MinecraftLaunchOptions options)
	{
		if (options is null)
		{
			throw new ArgumentNullException(nameof(options));
		}

		var plan = BuildLaunchPlan(options);
		var managedAssemblyNames = options.ManagedAssemblyNames?.ToArray() ?? [];

		var loader = new IkvmClassLoader(plan.ClassPathJars, managedAssemblyNames);

		if (options.ClassTransformers is not null)
		{
			foreach (var transformer in options.ClassTransformers)
			{
				loader.AddTransformer(transformer);
			}
		}

		if (options.AsmTransformers is not null)
		{
			foreach (var (filter, buildVisitor) in options.AsmTransformers)
			{
				loader.AddAsmTransformer(filter, buildVisitor);
			}
		}

		java.lang.Thread.currentThread().setContextClassLoader(loader);

		SetSystemProperty("java.library.path", options.NativesDirectoryPath);
		SetSystemProperty("org.lwjgl.system.allocator", "system");
		SetSystemProperty("org.lwjgl.system.SharedLibraryExtractPath", options.NativesDirectoryPath);
		SetSystemProperty("org.lwjgl.librarypath", options.NativesDirectoryPath);

		if (options.SystemProperties is not null)
		{
			foreach (var pair in options.SystemProperties)
			{
				SetSystemProperty(pair.Key, pair.Value);
			}
		}
	}

	private static string ReadAssetIndexId(JsonElement root, string versionId)
	{
		if (root.TryGetProperty("assetIndex", out var assetIndexNode) && assetIndexNode.ValueKind == JsonValueKind.Object)
		{
			var fromAssetIndex = ReadString(assetIndexNode, "id");
			if (!string.IsNullOrWhiteSpace(fromAssetIndex))
			{
				return fromAssetIndex;
			}
		}

		var fromAssets = ReadString(root, "assets");
		if (!string.IsNullOrWhiteSpace(fromAssets))
		{
			return fromAssets;
		}

		return versionId;
	}

	private static Dictionary<string, string> BuildLaunchVariables(
		MinecraftLaunchOptions options,
		string versionId,
		string versionType,
		string assetIndexId,
		string assetsRootPath,
		string gameDirectoryPath,
		string libraryDirectoryPath)
	{
		var playerName = string.IsNullOrWhiteSpace(options.PlayerName) ? "Player" : options.PlayerName.Trim();
		var authUuid = NormalizeUuidOrCreateOffline(options.Uuid, playerName);
		var accessToken = string.IsNullOrWhiteSpace(options.AccessToken) ? "0" : options.AccessToken;
		var userType = string.IsNullOrWhiteSpace(options.UserType) ? "legacy" : options.UserType;
		var userPropertiesJson = string.IsNullOrWhiteSpace(options.UserPropertiesJson) ? "{}" : options.UserPropertiesJson;

		var libraryDirectory = libraryDirectoryPath;
		var gameAssetsPath = Path.Combine(assetsRootPath, "virtual", "legacy");

		return new Dictionary<string, string>(StringComparer.Ordinal)
		{
			["auth_player_name"] = playerName,
			["version_name"] = versionId,
			["game_directory"] = gameDirectoryPath,
			["assets_root"] = assetsRootPath,
			["assets_index_name"] = assetIndexId,
			["auth_uuid"] = authUuid,
			["auth_access_token"] = accessToken,
			["auth_session"] = $"{accessToken}:{authUuid}",
			["user_type"] = userType,
			["version_type"] = versionType,
			["user_properties"] = userPropertiesJson,
			["game_assets"] = gameAssetsPath,
			["library_directory"] = libraryDirectory,
		};
	}

	private static IReadOnlyDictionary<string, bool> BuildFeatureFlags(IReadOnlyDictionary<string, bool> overrides)
	{
		var features = new Dictionary<string, bool>(StringComparer.Ordinal)
		{
			["is_demo_user"] = false,
			["has_custom_resolution"] = true,
			["is_quick_play_singleplayer"] = false,
			["is_quick_play_multiplayer"] = false,
			["is_quick_play_realms"] = false,
		};

		if (overrides is not null)
		{
			foreach (var pair in overrides)
			{
				features[pair.Key] = pair.Value;
			}
		}

		return features;
	}

	private static List<string> ResolveGameArguments(
		JsonElement root,
		IReadOnlyDictionary<string, string> launchVariables,
		IReadOnlyDictionary<string, bool> featureFlags,
		string minecraftOsName)
	{
		if (root.TryGetProperty("arguments", out var argumentsNode) && argumentsNode.ValueKind == JsonValueKind.Object)
		{
			if (argumentsNode.TryGetProperty("game", out var gameArgumentsNode) && gameArgumentsNode.ValueKind == JsonValueKind.Array)
			{
				return ResolveModernGameArguments(gameArgumentsNode, launchVariables, featureFlags, minecraftOsName);
			}
		}

		var legacyArguments = ReadString(root, "minecraftArguments");
		if (legacyArguments is not null)
		{
			var substituted = SubstituteLaunchVariables(legacyArguments, launchVariables);
			return SplitArguments(substituted);
		}

		throw new InvalidDataException("Version metadata does not provide launch game arguments.");
	}

	private static List<string> ResolveModernGameArguments(
		JsonElement gameArgumentsNode,
		IReadOnlyDictionary<string, string> launchVariables,
		IReadOnlyDictionary<string, bool> featureFlags,
		string minecraftOsName)
	{
		var result = new List<string>();
		foreach (var node in gameArgumentsNode.EnumerateArray())
		{
			switch (node.ValueKind)
			{
				case JsonValueKind.String:
					result.Add(SubstituteLaunchVariables(node.GetString() ?? string.Empty, launchVariables));
					break;

				case JsonValueKind.Object:
					if (!IsAllowedByRules(node, featureFlags, minecraftOsName))
					{
						continue;
					}

					if (!node.TryGetProperty("value", out var valueNode))
					{
						continue;
					}

					AppendArgumentValue(result, valueNode, launchVariables);
					break;
			}
		}

		return result;
	}

	private static void AppendArgumentValue(List<string> target, JsonElement valueNode, IReadOnlyDictionary<string, string> launchVariables)
	{
		switch (valueNode.ValueKind)
		{
			case JsonValueKind.String:
				target.Add(SubstituteLaunchVariables(valueNode.GetString() ?? string.Empty, launchVariables));
				break;

			case JsonValueKind.Array:
				foreach (var item in valueNode.EnumerateArray())
				{
					if (item.ValueKind == JsonValueKind.String)
					{
						target.Add(SubstituteLaunchVariables(item.GetString() ?? string.Empty, launchVariables));
					}
				}
				break;
		}
	}

	private static bool IsAllowedByRules(JsonElement argumentNode, IReadOnlyDictionary<string, bool> featureFlags, string minecraftOsName)
	{
		if (!argumentNode.TryGetProperty("rules", out var rulesNode) || rulesNode.ValueKind != JsonValueKind.Array)
		{
			return true;
		}

		var allowed = false;
		foreach (var ruleNode in rulesNode.EnumerateArray())
		{
			if (ruleNode.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			if (!DoesRuleMatch(ruleNode, featureFlags, minecraftOsName))
			{
				continue;
			}

			var action = ReadString(ruleNode, "action") ?? "allow";
			allowed = string.Equals(action, "allow", StringComparison.OrdinalIgnoreCase);
		}

		return allowed;
	}

	private static bool DoesRuleMatch(JsonElement ruleNode, IReadOnlyDictionary<string, bool> featureFlags, string minecraftOsName)
	{
		if (ruleNode.TryGetProperty("os", out var osNode) && osNode.ValueKind == JsonValueKind.Object)
		{
			var requiredOsName = ReadString(osNode, "name");
			if (requiredOsName is not null && !string.Equals(requiredOsName, minecraftOsName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}

		if (ruleNode.TryGetProperty("features", out var featuresNode) && featuresNode.ValueKind == JsonValueKind.Object)
		{
			foreach (var featureRequirement in featuresNode.EnumerateObject())
			{
				if (featureRequirement.Value.ValueKind != JsonValueKind.True && featureRequirement.Value.ValueKind != JsonValueKind.False)
				{
					continue;
				}

				var expected = featureRequirement.Value.GetBoolean();
				var actual = featureFlags.TryGetValue(featureRequirement.Name, out var value) && value;
				if (actual != expected)
				{
					return false;
				}
			}
		}

		return true;
	}

	private static string[] BuildClassPath(
		JsonElement root,
		string libraryDirectoryPath,
		string versionJarPath,
		string minecraftOsName,
		IReadOnlyDictionary<string, bool> featureFlags)
	{
		var classPath = new List<string>();
		foreach (var relativePath in ResolveLibraryRelativePaths(root, minecraftOsName, featureFlags))
		{
			var libraryJar = Path.Combine(libraryDirectoryPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
			if (!File.Exists(libraryJar))
			{
				throw new FileNotFoundException($"Required library not found: {libraryJar}", libraryJar);
			}

			classPath.Add(libraryJar);
		}

		classPath.Add(versionJarPath);
		return classPath.Distinct(StringComparer.Ordinal).ToArray();
	}

	private static List<string> ResolveLibraryRelativePaths(
		JsonElement root,
		string minecraftOsName,
		IReadOnlyDictionary<string, bool> featureFlags)
	{
		if (!root.TryGetProperty("libraries", out var librariesNode) || librariesNode.ValueKind != JsonValueKind.Array)
		{
			return [];
		}

		var normalizedOsName = NormalizeMinecraftOsName(minecraftOsName);
		var relativePaths = new List<string>();
		foreach (var libraryNode in librariesNode.EnumerateArray())
		{
			if (libraryNode.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			if (!IsLibraryAllowedByRules(libraryNode, normalizedOsName, featureFlags))
			{
				continue;
			}

			var relativePath = ResolveLibraryRelativePath(libraryNode);
			relativePaths.Add(relativePath);
		}

		return relativePaths.Distinct(StringComparer.Ordinal).ToList();
	}

	private static bool IsLibraryAllowedByRules(
		JsonElement libraryNode,
		string minecraftOsName,
		IReadOnlyDictionary<string, bool> featureFlags)
	{
		if (!libraryNode.TryGetProperty("rules", out var rulesNode) || rulesNode.ValueKind != JsonValueKind.Array)
		{
			return true;
		}

		var allowed = false;
		foreach (var ruleNode in rulesNode.EnumerateArray())
		{
			if (ruleNode.ValueKind != JsonValueKind.Object)
			{
				continue;
			}

			if (!DoesRuleMatch(ruleNode, featureFlags, minecraftOsName))
			{
				continue;
			}

			var action = ReadString(ruleNode, "action") ?? "allow";
			allowed = string.Equals(action, "allow", StringComparison.OrdinalIgnoreCase);
		}

		return allowed;
	}

	private static string ResolveLibraryRelativePath(JsonElement libraryNode)
	{
		if (libraryNode.TryGetProperty("downloads", out var downloadsNode) && downloadsNode.ValueKind == JsonValueKind.Object)
		{
			if (downloadsNode.TryGetProperty("artifact", out var artifactNode) && artifactNode.ValueKind == JsonValueKind.Object)
			{
				var explicitPath = ReadString(artifactNode, "path");
				if (!string.IsNullOrWhiteSpace(explicitPath))
				{
					return explicitPath;
				}
			}
		}

		var libraryName = ReadString(libraryNode, "name");
		if (string.IsNullOrWhiteSpace(libraryName))
		{
			throw new InvalidDataException("Library entry is missing both downloads.artifact.path and name.");
		}

		return ParseMavenLibraryPath(libraryName);
	}

	private static string ParseMavenLibraryPath(string libraryName)
	{
		var segments = libraryName.Split(':');
		if (segments.Length < 3)
		{
			throw new InvalidDataException($"Cannot parse Maven coordinates '{libraryName}'.");
		}

		var groupPath = segments[0].Replace('.', '/');
		var artifact = segments[1];
		var version = segments[2];
		var classifier = segments.Length > 3 && !string.IsNullOrWhiteSpace(segments[3]) ? $"-{segments[3]}" : string.Empty;
		return $"{groupPath}/{artifact}/{version}/{artifact}-{version}{classifier}.jar";
	}

	private static string NormalizeMinecraftOsName(string minecraftOsName)
	{
		if (string.Equals(minecraftOsName, "Emscripten", StringComparison.OrdinalIgnoreCase))
		{
			return "linux";
		}

		return string.IsNullOrWhiteSpace(minecraftOsName) ? "linux" : minecraftOsName.Trim().ToLowerInvariant();
	}

	private static string SubstituteLaunchVariables(string value, IReadOnlyDictionary<string, string> launchVariables)
	{
		return LaunchVariablePattern.Replace(value, (match) =>
		{
			var key = match.Groups["name"].Value;
			return launchVariables.TryGetValue(key, out var replacement) ? replacement : match.Value;
		});
	}

	private static List<string> SplitArguments(string argumentsLine)
	{
		var result = new List<string>();
		var current = new StringBuilder();
		var inQuotes = false;
		var quoteChar = '\0';
		var escaped = false;

		foreach (var ch in argumentsLine)
		{
			if (escaped)
			{
				current.Append(ch);
				escaped = false;
				continue;
			}

			if (inQuotes && ch == '\\')
			{
				escaped = true;
				continue;
			}

			if (ch == '\'' || ch == '"')
			{
				if (inQuotes && ch == quoteChar)
				{
					inQuotes = false;
					quoteChar = '\0';
					continue;
				}

				if (!inQuotes)
				{
					inQuotes = true;
					quoteChar = ch;
					continue;
				}
			}

			if (!inQuotes && char.IsWhiteSpace(ch))
			{
				FlushCurrent(result, current);
				continue;
			}

			current.Append(ch);
		}

		if (escaped)
		{
			current.Append('\\');
		}

		if (inQuotes)
		{
			throw new InvalidDataException($"Unterminated quoted argument in: {argumentsLine}");
		}

		FlushCurrent(result, current);
		return result;
	}

	private static void FlushCurrent(List<string> result, StringBuilder current)
	{
		if (current.Length == 0)
		{
			return;
		}

		result.Add(current.ToString());
		current.Clear();
	}

	private static string NormalizeUuidOrCreateOffline(string uuid, string playerName)
	{
		if (!string.IsNullOrWhiteSpace(uuid))
		{
			if (Guid.TryParse(uuid, out var guid))
			{
				return guid.ToString("D").ToLowerInvariant();
			}

			throw new ArgumentException($"Invalid UUID: {uuid}", nameof(uuid));
		}

		return CreateOfflineUuid(playerName);
	}

	private static string CreateOfflineUuid(string playerName)
	{
		var digest = java.security.MessageDigest.getInstance("MD5"); // mono-wasm doesn't have md5 but ikvm does lol
		var hash = digest.digest(Encoding.UTF8.GetBytes($"OfflinePlayer:{playerName}"));
		hash[6] = (byte)((hash[6] & 0x0F) | 0x30);
		hash[8] = (byte)((hash[8] & 0x3F) | 0x80);

		return $"{hash[0]:x2}{hash[1]:x2}{hash[2]:x2}{hash[3]:x2}-{hash[4]:x2}{hash[5]:x2}-{hash[6]:x2}{hash[7]:x2}-{hash[8]:x2}{hash[9]:x2}-{hash[10]:x2}{hash[11]:x2}{hash[12]:x2}{hash[13]:x2}{hash[14]:x2}{hash[15]:x2}";
	}

	private static void SetSystemProperty(string key, string value)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return;
		}

		java.lang.System.setProperty(key, value ?? string.Empty);
	}

	private static void InvokeMain(string mainClassName, string[] gameArguments)
	{
		Console.WriteLine($"[MinecraftLauncher] Main class: {mainClassName}");
		Console.WriteLine($"[MinecraftLauncher] Launch args count: {gameArguments.Length}");

		var classLoader = java.lang.Thread.currentThread().getContextClassLoader();
		var mainClass = java.lang.Class.forName(mainClassName, true, classLoader);
		var stringArrayClass = java.lang.Class.forName("[Ljava.lang.String;");
		var mainMethod = mainClass.getMethod("main", new[] { stringArrayClass });
		mainMethod.invoke(null, new object[] { gameArguments });
	}

	private static string RequireNonEmpty(string value, string argumentName)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException($"'{argumentName}' must be provided.", argumentName);
		}

		return value;
	}

	private static string ReadString(JsonElement root, string propertyName)
	{
		if (!root.TryGetProperty(propertyName, out var valueNode) || valueNode.ValueKind != JsonValueKind.String)
		{
			return null;
		}

		return valueNode.GetString();
	}
}
