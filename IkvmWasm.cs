using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Loader;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using java.net;
using java.util.jar;

static partial class IkvmWasm
{
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_lii(int a, int b);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liii(int a, int b, int c);
	[DllImport("Emscripten")]
	static internal extern int wasm_icall_iiiliii(int a, int b, long c, int d, int e, int f);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viil(int a, int b, long c);

    internal static void Main()
    {
        Console.WriteLine(":3");
    }

    [JSExport]
    internal static Task PreInit(string fetchbase)
    {
        try
        {
            Emscripten.MountOpfs();

			Directory.CreateDirectory("/ikvm");
            Emscripten.MountFetch(0, fetchbase + "/image/bin", "/ikvm/bin");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libzip.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libnio.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libnet.so");

            AppContext.SetData("IKVM.Home", "/ikvm");

            AssemblyLoadContext.Default.ResolvingUnmanagedDll += (assembly, name) =>
            {
				Console.WriteLine($"RESOLVING {assembly} {name}");
                return NativeLibrary.Load(name, assembly, null);
            };
            AssemblyLoadContext.Default.Resolving += (ctx, name) =>
            {
				Console.WriteLine($"RESOLVING {ctx} {name}");
				return null;
            };
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Error in PreInit()!");
            Console.Error.WriteLine(e);
        }

        return Task.CompletedTask;
    }

    [JSExport]
    internal static Task Run(string jar)
    {
        try
        {
			Console.WriteLine($"[IKVM] running jar {jar}");
            RunJar(jar);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("[IKVM] Run failed");
            Console.WriteLine($"ex: {ex}");
            throw;
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
