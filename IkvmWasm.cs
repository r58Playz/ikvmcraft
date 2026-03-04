using System;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.InteropServices.JavaScript;
using java.net;
using java.util.jar;

static partial class IkvmWasm
{
    internal static void Main()
    {
        Console.WriteLine(":3");
    }

    [JSExport]
    internal static Task PreInit(string fetchbase)
    {
        Emscripten.MountOpfs();
        Emscripten.MountFetch(fetchbase + "/assets", "/assets");
        Emscripten.MountFetch(fetchbase + "/ikvm", "/ikvm");
        Emscripten.MountFetchFile("/assets/main.jar");
		AppContext.SetData("IKVM.Home", "/ikvm");

        AssemblyLoadContext.Default.Resolving += (ctx, name) =>
        {
			Console.WriteLine($"LOADING ASM ?? {ctx} {name}");
			return null;
        };
        AssemblyLoadContext.Default.ResolvingUnmanagedDll += (ctx, name) =>
        {
			Console.WriteLine($"LOADING DLL ?? {ctx} {name}");
			return 0;
        };

        return Task.CompletedTask;
    }

    [JSExport]
    internal static Task Run()
    {
        try
        {
            RunJar("/assets/main.jar");
        }
        catch (System.Exception ex)
        {
            Console.Error.WriteLine("[IKVM] Run failed");
            Console.Error.WriteLine(ex.ToString());
        }

        return Task.CompletedTask;
    }

    private static void RunJar(string jarPath)
    {
        var mainClassName = GetMainClassName(jarPath);
        Console.WriteLine($"[IKVM] main class: {mainClassName}");

        var url = new java.io.File(jarPath).toURI().toURL();
        var loader = new URLClassLoader(new[] { url }, java.lang.ClassLoader.getSystemClassLoader());
        java.lang.Thread.currentThread().setContextClassLoader(loader);
        var mainClass = java.lang.Class.forName(mainClassName, true, loader);
        var stringArrayClass = java.lang.Class.forName("[Ljava.lang.String;");
        var mainMethod = mainClass.getMethod("main", new[] { stringArrayClass });
        mainMethod.invoke(null, new object[] { Array.Empty<string>() });
    }

    private static string GetMainClassName(string jarPath)
    {
        var jar = new JarFile(jarPath);
        var manifest = jar.getManifest();
        if (manifest == null)
        {
            throw new InvalidDataException($"Jar has no manifest: {jarPath}");
        }

        var mainClass = manifest.getMainAttributes()?.getValue(Attributes.Name.MAIN_CLASS);
        if (string.IsNullOrWhiteSpace(mainClass))
        {
            throw new InvalidDataException($"Jar manifest missing Main-Class: {jarPath}");
        }

        return mainClass.Trim();
    }
}
