using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

static partial class IkvmWasm
{
    static IkvmClassLoaderDll[] dlls = [
        (["org.lwjgl."], "ikvmc_lwjgl3.dll"),
		(["org.apache.logging.log4j"], "ikvmc_log4j.dll"),
		(["org.objectweb.asm"], "ikvmc_asm.dll"),
    ];
    static string[] jars = [
        "/assets/lwjgl3-demos.jar",
        "/assets/log4j-demo.jar"
    ];

    [DllImport("Emscripten")]
    static internal extern void ikvm_gl_init();

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
            Emscripten.MountFetchFile(0, "/ikvm/bin/libmanagement.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libawt.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libfontmanager.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libmlib_image.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/liblcms.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libjpeg.so");
            Emscripten.MountFetchDir(0, "/ikvm/lib");
            Emscripten.MountFetchFile(0, "/ikvm/lib/currency.data");
            Emscripten.MountFetchFile(0, "/ikvm/lib/tzdb.dat");
            Emscripten.MountFetchFile(0, "/ikvm/lib/content-types.properties");
            Emscripten.MountFetchFile(0, "/ikvm/lib/logging.properties");

            Emscripten.MountFetch(1, fetchbase + "/assets", "/assets");
            Emscripten.MountFetchFile(1, "/assets/lwjgl3-demos.jar");
            Emscripten.MountFetchFile(1, "/assets/log4j-demo.jar");

        	ikvm_gl_init();

            Directory.CreateDirectory("/tmp/lwjgl");
            File.WriteAllText("/tmp/lwjgl/liblwjgl.so", "");
            File.WriteAllText("/tmp/lwjgl/liblwjgl_opengl.so", "");
            File.WriteAllText("/tmp/lwjgl/liblwjgl_stb.so", "");
            File.WriteAllText("/tmp/lwjgl/liblwjgl_tinyfd.so", "");
            File.WriteAllText("/tmp/lwjgl/libffi.so", "");
            File.WriteAllText("/tmp/lwjgl/libglfw.so", "");
            File.WriteAllText("/tmp/lwjgl/libopenal.so", "");
            File.WriteAllText("/tmp/lwjgl/libGL.so.1", "");

            File.WriteAllText("/ikvm.properties", "ikvm.home=/ikvm");

            // -- ikvm will init after this --
            java.lang.Thread.currentThread().setContextClassLoader(new IkvmClassLoader(jars, dlls, []));

            java.lang.System.setProperty("org.lwjgl.system.allocator", "system");
            java.lang.System.setProperty("org.lwjgl.system.SharedLibraryExtractPath", "/tmp/lwjgl");
            java.lang.System.setProperty("org.lwjgl.librarypath", "/tmp/lwjgl");
            java.lang.System.setProperty("log4j2.contextSelector", "org.apache.logging.log4j.core.selector.BasicContextSelector");

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
    internal static Task Run()
    {
        try
        {
            Console.WriteLine($"[IKVM] running mc 1.16.1");

            MinecraftLauncher.LaunchVanilla(new()
            {
                VersionJsonPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.json",
                VersionJarPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.jar",
				ClientMappingsPath = "/libsdl/ikvmcraft/versions/1.16.1/client.txt",
                LibraryDirectoryPath = "/libsdl/ikvmcraft/libraries/",
                AssetsRootPath = "/libsdl/ikvmcraft/assets/",
                GameDirectoryPath = "/libsdl/minecraft/",
                MinecraftOsName = "Emscripten",
                ManagedAssemblyNames = dlls,
                AsmTransformers =
                [
					RemoveDfuPreloadTransform.Transformer,
					SwapNettyBackendTransform.Transformer
				],
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
