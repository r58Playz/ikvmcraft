using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Collections.Generic;
using IKVM.Runtime;

class IkvmClassLoader : java.net.URLClassLoader
{
    java.lang.ClassLoader SystemLoader;
    List<(string, java.lang.ClassLoader)> DllLoaders;

    public IkvmClassLoader(string[] jars, string[] dlls) 
        : base((from jar in jars select new java.net.URL("file", "", jar)).ToArray(), null)
    {
        var dllLoaders = from dll in dlls select (dll, CreateAsmLoader(Assembly.Load(dll)));
        DllLoaders = dllLoaders.ToList();
        SystemLoader = java.lang.ClassLoader.getSystemClassLoader();
    }

    static java.lang.ClassLoader CreateAsmLoader(Assembly asm)
    {
        var Context = typeof(JVM).GetProperty("Context", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var AssemblyClassLoaderFactory = Context.GetType().GetProperty("AssemblyClassLoaderFactory", BindingFlags.Instance | BindingFlags.Public).GetValue(Context);
        var loader = AssemblyClassLoaderFactory.GetType().GetMethod("FromAssembly", BindingFlags.Instance | BindingFlags.Public).Invoke(AssemblyClassLoaderFactory, [asm]);
        return (java.lang.ClassLoader)loader.GetType().GetMethod("GetJavaClassLoader", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(loader, []);
    }

    protected override java.lang.Class loadClass(string name, bool resolve)
    {
        lock (getClassLoadingLock(name))
        {
            string loadedFrom = "";
            java.lang.Class cls = findLoadedClass(name);

            if (name.StartsWith("java.") || name.StartsWith("javax.") || name.StartsWith("sun."))
            {
                if (cls == null)
                    cls = SystemLoader.loadClass(name);
                if (resolve)
                    resolveClass(cls);
                return cls;
            }

            if (cls == null)
            {
                foreach (var (dll, loader) in DllLoaders)
                {
                    try
                    {
                        cls = loader.loadClass(name);
                        if (cls != null)
                        {
                            loadedFrom = $"DLL {dll}";
                            break;
                        }
                    }
                    catch (java.lang.ClassNotFoundException) { }
                }
            }

            if (cls == null)
            {
                try
                {
                    cls = base.loadClass(name, resolve);
                    loadedFrom = "URLClassLoader";
					resolve = false;
                }
                catch (java.lang.ClassNotFoundException) { }
            }

            if (cls == null)
            {
                cls = SystemLoader.loadClass(name);
                loadedFrom = "System class loader";
            }

            if (resolve)
                resolveClass(cls);

            Console.WriteLine($"[IkvmClassLoader] {name} loaded from {loadedFrom}");

            return cls;
        }
    }
}

static partial class IkvmWasm
{
    static string[] dlls = [
        "ikvmc_lwjgl3.dll",
		"ikvmc_joml.dll"
    ];
    static string[] jars = [
		"/assets/lwjgl3-demos.jar"
    ];

    [DllImport("Emscripten")]
    static internal extern void mg_init();

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
            Emscripten.MountFetchDir(0, "/ikvm/lib");
            Emscripten.MountFetchFile(0, "/ikvm/lib/currency.data");
            Emscripten.MountFetchFile(0, "/ikvm/lib/tzdb.dat");

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
    internal static Task Run(string jar, string mainclass)
    {
        try
        {
            Console.WriteLine($"[IKVM] running jar {jar}");
            RunJar(jar, mainclass);
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            ExceptionLogging.WriteException(e, "[IKVM] Run failed");
            return Task.FromException(e);
        }
    }

    private static void RunJar(string jarPath, string mainclass)
    {
        var mainClassName = mainclass == null ? GetMainClassName(jarPath) : mainclass;
        Console.WriteLine($"[IKVM] main class: {mainClassName}");

        var mainClass = java.lang.Class.forName(mainClassName, true, java.lang.Thread.currentThread().getContextClassLoader());
        var stringArrayClass = java.lang.Class.forName("[Ljava.lang.String;");
        var mainMethod = mainClass.getMethod("main", new[] { stringArrayClass });
        mainMethod.invoke(null, new object[] { Array.Empty<string>() });
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
