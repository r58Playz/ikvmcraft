using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;

struct Dll
{
    public string RealName;
    public string MappedName;
}

static partial class IkvmWasm
{
    private const string GameDirectoryPath = "/libsdl/minecraft";

    internal static void Main()
    {
        Console.WriteLine(":3");
    }

    public static string[][] ConvertJSObjectToStringArray(JSObject jsObject)
    {
        // Get the JS Array length
        var outerLength = jsObject.GetPropertyAsInt32("length");
        var result = new string[outerLength][];

        for (int i = 0; i < outerLength; i++)
        {
            using var innerArray = jsObject.GetPropertyAsJSObject(i.ToString());
            var innerLength = innerArray.GetPropertyAsInt32("length");
            result[i] = new string[innerLength];

            for (int j = 0; j < innerLength; j++)
            {
                result[i][j] = innerArray.GetPropertyAsString(j.ToString()) ?? string.Empty;
            }
        }

        return result;
    }

    private static void MountDlls(string root, string[] rawDlls)
    {
        IEnumerable<Dll> dlls = rawDlls.Select(x =>
        {
            var split = x.Split('|');
            return new Dll() { RealName = split[0], MappedName = split[1] };
        });

        Directory.CreateDirectory("/dlls");
		Emscripten.MountFetch(1, root + "_framework/", "/fetchdlls/");
        foreach (var dll in dlls)
        {
            Emscripten.MountFetchFile(1, $"/fetchdlls/{dll.RealName}");
			File.CreateSymbolicLink($"/dlls/{dll.MappedName}", $"/fetchdlls/{dll.RealName}");
        }
    }
    [JSExport]
    internal static Task PreInit(string fetchbase, string[] jars, string[] rawDlls, JSObject props)
    {
        try
        {
            Emscripten.InstallThreadLogForwarder();

            Emscripten.MountOpfs();

            // Mods open relative paths, so set the process cwd before initializing Java.
            Directory.CreateDirectory(GameDirectoryPath);
            Directory.SetCurrentDirectory(GameDirectoryPath);

            Emscripten.MountFetch(0, fetchbase + "/image", "/ikvm");
            // JDK 25's home: its configuration in conf/, data in lib/, and the libraries the JDK looks up by file before
            // it loads them (they are linked into the wasm; the image has placeholders)
            Emscripten.MountFetchDir(0, "/ikvm/bin");
            foreach (var lib in new[] { "zip", "nio", "net", "management", "management_ext", "prefs", "awt", "awt_headless", "fontmanager", "mlib_image", "lcms", "javajpeg" })
                Emscripten.MountFetchFile(0, $"/ikvm/bin/lib{lib}.so");
            Emscripten.MountFetchDir(0, "/ikvm/conf");
            Emscripten.MountFetchFile(0, "/ikvm/conf/logging.properties");
            Emscripten.MountFetchFile(0, "/ikvm/conf/net.properties");
            Emscripten.MountFetchDir(0, "/ikvm/conf/security");
            Emscripten.MountFetchFile(0, "/ikvm/conf/security/java.security");
            Emscripten.MountFetchDir(0, "/ikvm/conf/security/policy");
            Emscripten.MountFetchDir(0, "/ikvm/conf/security/policy/unlimited");
            Emscripten.MountFetchFile(0, "/ikvm/conf/security/policy/unlimited/default_local.policy");
            Emscripten.MountFetchFile(0, "/ikvm/conf/security/policy/unlimited/default_US_export.policy");
            Emscripten.MountFetchDir(0, "/ikvm/lib");
            Emscripten.MountFetchFile(0, "/ikvm/lib/tzdb.dat");
            Emscripten.MountFetchDir(0, "/ikvm/lib/security");
            Emscripten.MountFetchFile(0, "/ikvm/lib/security/cacerts");
            Emscripten.MountFetchFile(0, "/ikvm/lib/security/blocked.certs");
            Emscripten.MountFetchFile(0, "/ikvm/lib/security/public_suffix_list.dat");

			MountDlls(fetchbase, rawDlls);

            Directory.CreateDirectory("/tmp/lwjgl");
            File.WriteAllText("/tmp/lwjgl/liblwjgl.so", "");
            File.WriteAllText("/tmp/lwjgl/liblwjgl_opengl.so", "");
            File.WriteAllText("/tmp/lwjgl/liblwjgl_stb.so", "");
            File.WriteAllText("/tmp/lwjgl/liblwjgl_tinyfd.so", "");
            File.WriteAllText("/tmp/lwjgl/libffi.so", "");
            File.WriteAllText("/tmp/lwjgl/libglfw.so", "");
            File.WriteAllText("/tmp/lwjgl/libopenal.so", "");
            File.WriteAllText("/tmp/lwjgl/libfreetype.so", "");
            File.WriteAllText("/tmp/lwjgl/libGL.so.1", "");

            File.WriteAllText("/ikvm.properties", "ikvm.home=/ikvm");

            // Accessing JVM.Properties.User loads IKVM and runs its module initializer before
            // the assignment. AppContext supplies these values without initializing the JVM.
            // Use the configured path because WasmFS getcwd drops the /libsdl mount name.
            AppContext.SetData("IKVM.Properties", "user.dir;sun.nio.fs.chdirAllowed");
            AppContext.SetData("user.dir", GameDirectoryPath);
            AppContext.SetData("sun.nio.fs.chdirAllowed", "true");
            InitializeJava(jars, props);

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            ExceptionLogging.WriteException(e, "Error in PreInit()!");
            return Task.FromException(e);
        }
    }

    // Keep Java type initialization out of PreInit's compilation, until the filesystem and
    // startup properties above are ready.
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void InitializeJava(string[] jars, JSObject props)
    {
        // Also update the active properties if the JVM was initialized before PreInit.
        // This must precede the classloader's first use of NIO.
        java.lang.System.setProperty("user.dir", GameDirectoryPath);
        java.lang.System.setProperty("sun.nio.fs.chdirAllowed", "true");

        var bootstrapDlls = IkvmcManifest.LoadEmbedded().AlwaysReplaceDlls();
        java.lang.Thread.currentThread().setContextClassLoader(new IkvmClassLoader(jars, bootstrapDlls, []));

        java.lang.System.setProperty("org.lwjgl.system.allocator", "system");
        java.lang.System.setProperty("org.lwjgl.system.SharedLibraryExtractPath", "/tmp/lwjgl");
        java.lang.System.setProperty("org.lwjgl.librarypath", "/tmp/lwjgl");
        java.lang.System.setProperty("log4j2.contextSelector", "org.apache.logging.log4j.core.selector.BasicContextSelector");

        foreach (var prop in ConvertJSObjectToStringArray(props))
        {
            Console.WriteLine($"-D{prop[0]}={prop[1]}");
            java.lang.System.setProperty(prop[0], prop[1]);
        }

        IkvmNetworkInterfaces.Install();
    }

    // BENCHMARK/DIAGNOSTIC, off unless set: IKVM_GAME_OPTIONS=key:value[,key:value...] rewrites those lines of the game's
    // options.txt before the game reads it -- maxFps:260 lifts Minecraft's own frame limiter, which holds a measurement at
    // the seed's 120 once the client is fast enough to reach it. It changes what the game does, so like every knob it
    // travels in the URL (?wj.IKVM_GAME_OPTIONS=...). A run's profile is a copy of the seed, so nothing persists.
    static void ApplyGameOptionOverrides()
    {
        var spec = Environment.GetEnvironmentVariable("IKVM_GAME_OPTIONS");
        if (string.IsNullOrEmpty(spec))
            return;

        var path = Path.Combine(GameDirectoryPath, "options.txt");
        var lines = File.Exists(path) ? File.ReadAllLines(path).ToList() : new List<string>();
        foreach (var kv in spec.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var colon = kv.IndexOf(':');
            if (colon <= 0)
                continue;
            var prefix = kv[..(colon + 1)];
            var at = lines.FindIndex(l => l.StartsWith(prefix, StringComparison.Ordinal));
            if (at >= 0)
                lines[at] = kv;
            else
                lines.Add(kv);
        }
        File.WriteAllLines(path, lines);
        Console.WriteLine($"[IKVM] options.txt overrides applied: {spec}");
    }

    // DIAGNOSTIC, off unless set: IKVM_GAME_ARGS="--quickPlayMultiplayer 127.0.0.1:25565" appends whitespace-separated
    // arguments to Minecraft's command line. The harness uses it to join a local offline-mode server through the dev
    // server's wisp (multiplayer repro, 2026-11-11). It changes what the game does, so it travels in the URL
    // (?wj.IKVM_GAME_ARGS=...) like every knob.
    static string[] ExtraGameArgumentOverrides()
    {
        var spec = Environment.GetEnvironmentVariable("IKVM_GAME_ARGS");
        if (string.IsNullOrWhiteSpace(spec))
            return [];
        var extra = spec.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Console.WriteLine($"[IKVM] extra game arguments: {string.Join(" ", extra)}");
        return extra;
    }

    [JSExport]
    internal static Task Run(string version, string playerName, string uuid, string accessToken, string userType)
    {
        try
        {
            Console.WriteLine($"[IKVM] running mc {version}");
            ApplyGameOptionOverrides();

            MinecraftLauncher.LaunchVanilla(new()
            {
                VersionJsonPath = $"/libsdl/ikvmcraft/versions/{version}/version.json",
                VersionJarPath = $"/libsdl/ikvmcraft/versions/{version}/client.jar",
				ClientMappingsPath = $"/libsdl/ikvmcraft/versions/{version}/client.txt",
                LibraryDirectoryPath = "/libsdl/ikvmcraft/libraries/",
                AssetsRootPath = "/libsdl/ikvmcraft/assets/",
                GameDirectoryPath = GameDirectoryPath,
                MinecraftOsName = "Emscripten",
                AsmTransformers =
                [
					InjectIkvmIntoKnotTransform.Transformer,

					ThereAreNoBareClientsTransform.Transformer,

					LazyDfuTransform.Transformer,

					// BENCHMARK ONLY, off unless IKVM_DEFUSE_RANKED_AUTH=1: MCSR Ranked's account
					// validation never reaches a terminal socket status under this net bridge, so the
					// modded main screen never finishes and the harness kills the run. See
					// Transforms/Bench/DefuseRankedAuth.cs. Conditional rather than always-listed
					// because it changes what the game does.
					.. DefuseRankedAuthTransform.Enabled
						? new[] { DefuseRankedAuthTransform.Transformer }
						: [],
				],

				PlayerName = playerName,
				Uuid = uuid,
				AccessToken = accessToken,
				UserType = userType,
				UserPropertiesJson = "{}",
				ExtraGameArguments = ExtraGameArgumentOverrides()
            });

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            ExceptionLogging.WriteException(e, "[IKVM] Run failed");
            return Task.FromException(e);
        }
    }

    [JSExport]
    internal static Task RunJar(string jarPath, string mainclass)
    {
        try
        {
            Console.WriteLine($"[IKVM] running jar {jarPath}");
            var mainClassName = mainclass ?? GetMainClassName(jarPath);
            Console.WriteLine($"[IKVM] main class: {mainClassName}");

            var mainClass = java.lang.Class.forName(mainClassName, true, java.lang.Thread.currentThread().getContextClassLoader());
            var stringArrayClass = java.lang.Class.forName("[Ljava.lang.String;");
            var mainMethod = mainClass.getMethod("main", [stringArrayClass]);
            mainMethod.invoke(null, [Array.Empty<string>()]);
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            ExceptionLogging.WriteException(e, "[IKVM] Run failed");
            return Task.FromException(e);
        }
    }

    private static string GetMainClassName(string jarPath)
    {
        var jar = new java.util.jar.JarFile(jarPath);
        var manifest = jar.getManifest() ?? throw new InvalidDataException($"Jar has no manifest: {jarPath}");

        var mainClass = manifest.getMainAttributes()?.getValue(java.util.jar.Attributes.Name.MAIN_CLASS);
        if (string.IsNullOrWhiteSpace(mainClass))
        {
            throw new InvalidDataException($"Jar manifest missing Main-Class: {jarPath}");
        }

        return mainClass.Trim();
    }

    [JSExport]
    internal static Task WatchMemoryUsage(int delay, [JSMarshalAs<JSType.Function<JSType.Number, JSType.Boolean>>] Func<double, bool> callback)
    {
        return Task.Run(async () =>
        {
            while (true)
            {
				var info = GC.GetGCMemoryInfo();
                bool stop = callback((double)info.TotalCommittedBytes / (1024 * 1024));
				if (stop) break;
                await Task.Delay(delay);
            }
        });
    }
}
