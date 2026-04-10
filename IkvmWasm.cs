using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using IKVM.Runtime;
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
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viili(int a, int b, long c, int d);
	[DllImport("Emscripten")]
	static internal extern int wasm_icall_iiilii(int a, int b, long c, int d, int e);
	[DllImport("Emscripten")]
	static internal extern int wasm_icall_iiiili(int a, int b, int c, long d, int e);
	[DllImport("Emscripten")]
	static internal extern int wasm_icall_iiiiilli(int a, int b, int c, int d, long e, long f, int g);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liil(int a, int b, long c);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiill(int a, int b, int c, long d, long e);
	[DllImport("Emscripten")]
	static internal extern int wasm_icall_iiill(int a, int b, long c, long d);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liili(int a, int b, long c, int d);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liill(int a, int b, long c, long d);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liiiillll(int a, int b, int c, int d, long e, long f, long g, long h);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiliil(int a, int b, long c, int d, int e, long f);
	[DllImport("Emscripten")]
	static internal extern int wasm_icall_iiiliill(int a, int b, long c, int d, int e, long f, long g);
	[DllImport("Emscripten")]
	static internal extern int wasm_icall_iiilllll(int a, int b, long c, long d, long e, long f, long g);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liilll(int a, int b, long c, long d, long e);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viillll(int a, int b, long c, long d, long e, long f);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viill(int a, int b, long c, long d);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liiiil(int a, int b, int c, int d, long e);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiil(int a, int b, int c, long d);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiffff(int a, int b, float c, float d, float e, float f);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viillllll(int a, int b, long c, long d, long e, long f, long g, long h);
	[DllImport("Emscripten")]
	static internal extern long wasm_icall_liilillli(int a, int b, long c, int d, long e, long f, long g, int h);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, long k);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiiill(int a, int b, int c, int d, long e, long f);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiilli(int a, int b, int c, long d, long e, int f);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiiiiil(int a, int b, int c, int d, int e, int f, long g);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiilll(int a, int b, int c, long d, long e, long f);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiiiill(int a, int b, int c, int d, int e, long f, long g);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viiililil(int a, int b, int c, long d, int e, long f, int g, long h);
	[DllImport("Emscripten")]
	static internal extern void wasm_icall_viilll(int a, int b, long c, long d, long e);

	[DllImport("Emscripten")]
	static internal extern void mg_init();

	private static readonly string[] BaseClassPath =
	[
		"/assets/lwjgl3.jar",
		"/assets/lwjgl3-demos.jar",
	];

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

            Emscripten.MountFetch(0, fetchbase + "/image", "/ikvm");
			Emscripten.MountFetchDir(0, "/ikvm/bin");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libzip.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libnio.so");
            Emscripten.MountFetchFile(0, "/ikvm/bin/libnet.so");
			Emscripten.MountFetchDir(0, "/ikvm/lib");
            Emscripten.MountFetchFile(0, "/ikvm/lib/currency.data");

			Emscripten.MountFetch(1, fetchbase + "/assets", "/assets");
			Emscripten.MountFetchFile(1, "/assets/lwjgl3-demos.jar");
			Emscripten.MountFetchFile(1, "/assets/lwjgl3.jar");

			Directory.CreateDirectory("/mobileglues");
			File.WriteAllText("/mobileglues/config.json", """{ "customGLVersion": 32 }""");
			mg_init();

			File.WriteAllText("/ikvm.properties", "ikvm.home=/ikvm");
			SetClassPath(BaseClassPath);

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
            ExceptionLogging.WriteException(e, "Error in PreInit()!");
        }

        return Task.CompletedTask;
    }

    [JSExport]
    internal static Task Run(string jar, string mainclass)
    {
        try
        {
			Console.WriteLine($"[IKVM] running jar {jar}");
            RunJar(jar, mainclass);
        }
        catch (System.Exception ex)
        {
            ExceptionLogging.WriteException(ex, "[IKVM] Run failed");
            throw;
        }

        return Task.CompletedTask;
    }

	private static void RunJar(string jarPath, string mainclass)
    {
        var mainClassName = mainclass == null ? GetMainClassName(jarPath) : mainclass;
        Console.WriteLine($"[IKVM] main class: {mainClassName}");
		java.lang.System.setProperty("org.lwjgl.util.Debug", "true");
		java.lang.System.setProperty("org.lwjgl.util.DebugLoader", "true");
		java.lang.System.setProperty("org.lwjgl.system.allocator", "system");
		java.lang.System.setProperty("org.lwjgl.system.SharedLibraryExtractPath", "/tmp/lwjgl");
		Console.WriteLine($"[LWJGL] debug: {java.lang.System.getProperty("org.lwjgl.util.Debug")}");
		Console.WriteLine($"[LWJGL] debugloader: {java.lang.System.getProperty("org.lwjgl.util.DebugLoader")}");

		var classPathUrls = BuildClassPathUrls(jarPath);
		var loader = new URLClassLoader(classPathUrls, java.lang.ClassLoader.getSystemClassLoader());
        java.lang.Thread.currentThread().setContextClassLoader(loader);
        var mainClass = java.lang.Class.forName(mainClassName, true, loader);
        var stringArrayClass = java.lang.Class.forName("[Ljava.lang.String;");
        var mainMethod = mainClass.getMethod("main", new[] { stringArrayClass });
        mainMethod.invoke(null, new object[] { Array.Empty<string>() });
    }

	private static URL[] BuildClassPathUrls(string jarPath)
	{
		var entries = new List<string>();
		var separator = java.io.File.pathSeparatorChar;

		if (JVM.Properties.User.TryGetValue("java.class.path", out var configuredClassPath) &&
			!string.IsNullOrWhiteSpace(configuredClassPath))
		{
			entries.AddRange(configuredClassPath.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
		}

		entries.Add(jarPath);
		var normalizedEntries = NormalizeClassPathEntries(entries);
		if (normalizedEntries.Count == 0)
		{
			throw new InvalidDataException("Classpath is empty.");
		}

		SetClassPath(normalizedEntries.ToArray());

		var urls = new URL[normalizedEntries.Count];
		for (var i = 0; i < normalizedEntries.Count; i++)
		{
			urls[i] = new java.io.File(normalizedEntries[i]).toURI().toURL();
		}

		return urls;
	}

	private static List<string> NormalizeClassPathEntries(IEnumerable<string> entries)
	{
		var normalized = new List<string>();
		var seen = new HashSet<string>(StringComparer.Ordinal);

		foreach (var entry in entries)
		{
			if (string.IsNullOrWhiteSpace(entry))
				continue;

			var trimmed = entry.Trim();
			if (seen.Add(trimmed))
				normalized.Add(trimmed);
		}

		return normalized;
	}

	private static void SetClassPath(params string[] entries)
	{
		var normalizedEntries = NormalizeClassPathEntries(entries);
		var classPath = string.Join(java.io.File.pathSeparatorChar.ToString(), normalizedEntries);
		JVM.Properties.User["java.class.path"] = classPath;
		Console.WriteLine($"[IKVM] java.class.path: {classPath}");
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
