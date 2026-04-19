using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

static partial class IkvmWasm
{
    static string[] dlls = [
        "ikvmc_lwjgl3.dll",
		"ikvmc_joml.dll",
		"ikvmc_log4j.dll"
    ];
    static string[] jars = [
        "/assets/lwjgl3-demos.jar"
    ];

    [DllImport("Emscripten")]
    static internal extern void mg_init();

    internal static void Main()
    {
        Console.WriteLine(":3");

// Pure C# in your WASM build
object[] arr = new object[10];
object x = new object();
arr[0] = x;  // does this throw?

// Simulate what IKVM does
object[] arr2 = (object[])Array.CreateInstance(typeof(object), 10);
arr2[0] = new object();  // does this throw?
    }

    public static string[][] ConvertJSObjectToStringArray(JSObject jsObject)
    {
        // Get the JS Array length
        var outerLength = jsObject.GetPropertyAsInt32("length");
        var result = new string[outerLength][];

        for (int i = 0; i < outerLength; i++)
        {
            using var innerArray = jsObject.GetPropertyAsJSObject(i.ToString());
            var innerLength = innerArray!.GetPropertyAsInt32("length");
            result[i] = new string[innerLength];

            for (int j = 0; j < innerLength; j++)
            {
                result[i][j] = innerArray.GetPropertyAsString(j.ToString()) ?? string.Empty;
            }
        }

        return result;
    }

    [JSExport]
    internal static Task PreInit(string fetchbase, JSObject props)
    {
        try
        {
            Emscripten.MountOpfs();

            Emscripten.MountFetch(0, fetchbase + "/image", "/ikvm");
            Emscripten.MountFetchDir(0, "/ikvm/bin");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libzip.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libnio.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libnet.so");
            Emscripten.MountFetchDir(0, "/ikvm/lib");
            Emscripten.MountFetchFile(0, "/ikvm/lib/currency.data");
            Emscripten.MountFetchFile(0, "/ikvm/lib/tzdb.dat");
            Emscripten.MountFetchFile(0, "/ikvm/lib/content-types.properties");
            Emscripten.MountFetchFile(0, "/ikvm/lib/logging.properties");

            Emscripten.MountFetch(1, fetchbase + "/assets", "/assets");
            Emscripten.MountFetchFile(1, "/assets/lwjgl3-demos.jar");

            Directory.CreateDirectory("/mobileglues");
            File.WriteAllText("/mobileglues/config.json", """{ "customGLVersion": 32 }""");
            mg_init();

            Directory.CreateDirectory("/tmp/lwjgl");
            File.WriteAllText("/tmp/lwjgl/liblwjgl.so", "");
            File.WriteAllText("/tmp/lwjgl/liblwjgl_opengl.so", "");
            File.WriteAllText("/tmp/lwjgl/libffi.so", "");
            File.WriteAllText("/tmp/lwjgl/libglfw.so", "");

            File.WriteAllText("/ikvm.properties", "ikvm.home=/ikvm");

            // -- ikvm will init after this --
            java.lang.Thread.currentThread().setContextClassLoader(new IkvmClassLoader(jars, dlls));

            java.lang.System.setProperty("org.lwjgl.system.allocator", "system");
            java.lang.System.setProperty("org.lwjgl.system.SharedLibraryExtractPath", "/tmp/lwjgl");
            java.lang.System.setProperty("org.lwjgl.librarypath", "/tmp/lwjgl");

            foreach (var prop in ConvertJSObjectToStringArray(props))
            {
                Console.WriteLine($"-D{prop[0]}={prop[1]}");
                java.lang.System.setProperty(prop[0], prop[1]);
            }

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            ExceptionLogging.WriteException(e, "Error in PreInit()!");
            return Task.FromException(e);
        }
    }

    [JSExport]
    internal static Task Run(string jarPath, string mainclass)
    {
        try
        {
            Console.WriteLine($"[IKVM] running mc 1.16.1");

			MinecraftLauncher.LaunchVanilla(new() {
				VersionJsonPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.json",
				VersionJarPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.jar",
				LibraryDirectoryPath = "/libsdl/ikvmcraft/libraries/",
				AssetsRootPath = "/libsdl/ikvmcraft/assets/",
				GameDirectoryPath = "/libsdl/minecraft/",
				MinecraftOsName = "Emscripten",
				ManagedAssemblyNames = dlls,
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
            var mainClassName = mainclass == null ? GetMainClassName(jarPath) : mainclass;
            Console.WriteLine($"[IKVM] main class: {mainClassName}");

            var mainClass = java.lang.Class.forName(mainClassName, true, java.lang.Thread.currentThread().getContextClassLoader());
            var stringArrayClass = java.lang.Class.forName("[Ljava.lang.String;");
            var mainMethod = mainClass.getMethod("main", new[] { stringArrayClass });
            mainMethod.invoke(null, new object[] { Array.Empty<string>() });
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
        var manifest = jar.getManifest();
        if (manifest == null)
        {
            throw new InvalidDataException($"Jar has no manifest: {jarPath}");
        }

        var mainClass = manifest.getMainAttributes()?.getValue(java.util.jar.Attributes.Name.MAIN_CLASS);
        if (string.IsNullOrWhiteSpace(mainClass))
        {
            throw new InvalidDataException($"Jar manifest missing Main-Class: {jarPath}");
        }

        return mainClass.Trim();
    }
}
