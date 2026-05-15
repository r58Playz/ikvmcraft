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
    internal static Task Run()
    {
        try
        {
            Console.WriteLine($"[IKVM] running mc 1.16.1");

            MinecraftLauncher.LaunchVanilla(new()
            {
                VersionJsonPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.json",
                VersionJarPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.jar",
                LibraryDirectoryPath = "/libsdl/ikvmcraft/libraries/",
                AssetsRootPath = "/libsdl/ikvmcraft/assets/",
                GameDirectoryPath = "/libsdl/minecraft/",
                MinecraftOsName = "Emscripten",
                ManagedAssemblyNames = dlls,
                AsmTransformers = new[]
                {
                    MinecraftRunLoopTransformer.AsTransformer(MinecraftRunLoopTransformer.Obfuscated_1_16_1),
                    MainCatchEmLoopTransformer.AsTransformer(MainCatchEmLoopTransformer.Default),
                },
            });

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            var emLoopStarted = UnwrapEmLoopStarted(e);
            if (emLoopStarted is not null)
            {
                Console.WriteLine("[IKVM] em-loop registered; returning task for JS to await");
                return emLoopStarted.EmLoopTask;
            }

            ExceptionLogging.WriteException(e, "[IKVM] Run failed");
            return Task.FromException(e);
        }
    }

    /// <summary>
    /// Walk the cause chain looking for the em-loop marker. We have to check
    /// both <see cref="Exception.InnerException"/> (.NET side) and
    /// <c>java.lang.Throwable.getCause()</c> (Java side) — IKVM does not
    /// reliably mirror Java's cause into .NET's InnerException, so an
    /// <c>InvocationTargetException</c> wrapping our marker would otherwise
    /// look like a leaf to a plain InnerException walk.
    /// </summary>
    private static IkvmEmLoopStarted UnwrapEmLoopStarted(Exception ex)
    {
        var seen = new System.Collections.Generic.HashSet<Exception>(System.Collections.Generic.ReferenceEqualityComparer.Instance);
        var current = ex;
        while (current is not null && seen.Add(current))
        {
            if (current is IkvmEmLoopStarted started)
            {
                return started;
            }

            Exception next = current.InnerException;
            if (next is null && current is java.lang.Throwable throwable)
            {
                next = throwable.getCause() as Exception;
            }
            current = next;
        }
        return null;
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

    /// <summary>
    /// Run Minecraft's Main.main(String[]) with the standard launch arguments
    /// computed from the version JSON, but call it via reflection from a
    /// controlled triage entry so we can interleave with other ReflectInvoke
    /// calls. Mirrors what MinecraftLauncher.LaunchVanilla -> InvokeMain does.
    /// </summary>
    [JSExport]
    internal static Task RunMinecraftMain()
    {
        try
        {
            Console.WriteLine($"[IKVM] running Main.main via triage path");
            var options = new MinecraftLaunchOptions
            {
                VersionJsonPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.json",
                VersionJarPath = "/libsdl/ikvmcraft/versions/1.16.1/1.16.1.jar",
                LibraryDirectoryPath = "/libsdl/ikvmcraft/libraries/",
                AssetsRootPath = "/libsdl/ikvmcraft/assets/",
                GameDirectoryPath = "/libsdl/minecraft/",
                MinecraftOsName = "Emscripten",
                ManagedAssemblyNames = dlls,
            };
            var plan = MinecraftLauncher.BuildLaunchPlan(options);

            var classLoader = java.lang.Thread.currentThread().getContextClassLoader();
            var mainClass = java.lang.Class.forName(plan.MainClassName, true, classLoader);
            var stringArrayClass = java.lang.Class.forName("[Ljava.lang.String;");
            var mainMethod = mainClass.getMethod("main", new[] { stringArrayClass });
            mainMethod.invoke(null, new object[] { plan.GameArguments });
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            ExceptionLogging.WriteException(e, "[IKVM] RunMinecraftMain failed");
            return Task.FromException(e);
        }
    }

    /// <summary>
    /// Set up the Minecraft classpath / native paths / system properties without
    /// running the launcher main. After this returns, the JS side can call
    /// <see cref="ReflectInvoke"/> repeatedly to drive specific Java classes by
    /// name and observe which one triggers the bug we're chasing.
    /// </summary>
    [JSExport]
    internal static Task SetupMinecraft()
    {
        try
        {
            Console.WriteLine($"[IKVM] setting up mc 1.16.1 classpath (no main)");

            MinecraftLauncher.LaunchVanillaSetup(new()
            {
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
            ExceptionLogging.WriteException(e, "[IKVM] SetupMinecraft failed");
            return Task.FromException(e);
        }
    }

    /// <summary>
    /// Resolve a Java class by FQN and call a static method on it via reflection.
    /// Useful for narrowing down which class triggers a bug — call from JS with
    /// e.g. <c>("net.minecraft.Util", "<clinit>", [])</c> to force-init that
    /// specific class. Empty <paramref name="methodName"/> just loads/inits the
    /// class without invoking any method.
    ///
    /// Note: arguments are limited to strings (sufficient for triage; extend if
    /// needed). Method must be static and take string parameters in the same
    /// order as <paramref name="argsObj"/>'s string entries.
    /// </summary>
    [JSExport]
    internal static Task ReflectInvoke(string className, string methodName, JSObject argsObj)
    {
        try
        {
            Console.WriteLine($"[IKVM] reflect-invoke {className}.{(string.IsNullOrEmpty(methodName) ? "<load>" : methodName)}");

            var classLoader = java.lang.Thread.currentThread().getContextClassLoader();

            // Just loading & forcing class-init is the most useful case: pass
            // initialize=true so the .cctor runs before we return.
            var clazz = java.lang.Class.forName(className, true, classLoader);

            if (string.IsNullOrEmpty(methodName))
            {
                Console.WriteLine($"[IKVM] reflect-invoke loaded {className}");
                return Task.CompletedTask;
            }

            // Convert JS string-array → Java String[] / parameter Class[].
            int argLen = argsObj == null ? 0 : argsObj.GetPropertyAsInt32("length");
            var paramTypes = new java.lang.Class[argLen];
            var argValues = new object[argLen];
            var stringClass = java.lang.Class.forName("java.lang.String");
            for (int i = 0; i < argLen; i++)
            {
                paramTypes[i] = stringClass;
                argValues[i] = argsObj.GetPropertyAsString(i.ToString()) ?? string.Empty;
            }

            var method = clazz.getDeclaredMethod(methodName, paramTypes);
            method.setAccessible(true);
            var result = method.invoke(null, argValues);
            Console.WriteLine($"[IKVM] reflect-invoke {className}.{methodName} -> {(result == null ? "<null>" : result.ToString())}");
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            ExceptionLogging.WriteException(e, $"[IKVM] reflect-invoke {className}.{methodName} failed");
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
