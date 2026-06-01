global using IkvmClassLoaderDll = (string[] Prefixes, string Name);
global using IkvmClassLoaderTransformer = System.Func<(string[] Classes, System.Type Visitor)>;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using IKVM.Runtime;

class AssemblyURLConnection : java.net.JarURLConnection
{
	private readonly java.net.URL JarUrl;

	public AssemblyURLConnection(java.net.URL url) : base(new("jar:" + url + "!/"))
	{
		JarUrl = url;
	}

    public override java.net.URL getJarFileURL() => JarUrl;
	public override java.util.jar.JarFile getJarFile() => null;
    public override java.util.jar.JarEntry getJarEntry() => null;
    public override void connect() { }
}

class AssemblyURLStreamHandler : java.net.URLStreamHandler
{
	protected override java.net.URLConnection openConnection(java.net.URL url)
	{
		var name = url.getHost();
		var asm = $"/dlls/{name}.dll";
		return new AssemblyURLConnection(new("file", "", asm));
	}
}

class IkvmURLStreamHandlerFactory : java.net.URLStreamHandlerFactory
{
	public java.net.URLStreamHandler createURLStreamHandler(String protocol)
	{
		if (protocol == "assembly:")
			return new AssemblyURLStreamHandler();

		return null;
	}
}

internal sealed class IkvmClassLoader : java.net.URLClassLoader
{
	static IkvmClassLoader()
	{
		java.net.URL.setURLStreamHandlerFactory(new IkvmURLStreamHandlerFactory());
	}

	[DllImport("Emscripten")]
	private static extern void classloader_debug([MarshalAs(UnmanagedType.LPUTF8Str)] string log);
	[DllImport("Emscripten")]
	private static extern void classloader_set_mono_assembly_filename(IntPtr assembly, [MarshalAs(UnmanagedType.LPUTF8Str)] string filename);

    private readonly java.lang.ClassLoader systemLoader;
    private readonly List<(string Name, string[] Prefixes, java.lang.ClassLoader Loader)> dllLoaders;
	private readonly List<(string[] Classes, System.Type Visitor)> classTransformers; 

	public static IkvmClassLoader LatestInstance;

    public IkvmClassLoader(string[] jars, IkvmClassLoaderDll[] dlls, IkvmClassLoaderTransformer[] transformers)
        : base((from jar in jars select new java.net.URL("file", "", jar)).ToArray(), null)
    {
        dllLoaders =
        [
            ("IkvmWasm", new[] { "cli.Ikvm" }, CreateAssemblyClassLoader("IkvmWasm", typeof(IkvmClassLoader).Assembly)),
            .. from dll in dlls
               select (dll.Name, dll.Prefixes, CreateAssemblyClassLoader(dll.Name, Assembly.Load(dll.Name))),
        ];
        systemLoader = java.lang.ClassLoader.getSystemClassLoader();

		classTransformers = new();
		foreach (var transformerInit in transformers)
		{
			var transformer = transformerInit();
			Type baseType = transformer.Visitor;
			while (baseType != typeof(System.Object))
			{
				baseType = baseType.BaseType;
				if (baseType == typeof(org.objectweb.asm.ClassVisitor))
					goto End;
			}
			throw new InvalidOperationException("transformer must inherit from ClassVisitor");
		End:
			classTransformers.Add(transformer);
		}

		LatestInstance = this;
    }

    private static java.lang.ClassLoader CreateAssemblyClassLoader(string name, Assembly assembly)
    {
		var ptr = (IntPtr)assembly.GetType().GetMethod("GetUnderlyingNativeHandle", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(assembly, []);
		classloader_set_mono_assembly_filename(ptr, $"/dlls/{name}.dll");

        var context = typeof(JVM).GetProperty("Context", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
        if (context is null)
        {
            throw new InvalidOperationException("IKVM runtime context is unavailable.");
        }

        var classLoaderFactory = context.GetType().GetProperty("AssemblyClassLoaderFactory", BindingFlags.Instance | BindingFlags.Public)?.GetValue(context);
        if (classLoaderFactory is null)
        {
            throw new InvalidOperationException("IKVM assembly class loader factory is unavailable.");
        }

        var loader = classLoaderFactory.GetType().GetMethod("FromAssembly", BindingFlags.Instance | BindingFlags.Public)?.Invoke(classLoaderFactory, [assembly]);
        if (loader is null)
        {
            throw new InvalidOperationException($"Failed creating IKVM class loader for assembly '{assembly.FullName}'.");
        }

        var javaClassLoader = loader.GetType().GetMethod("GetJavaClassLoader", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(loader, []);
        if (javaClassLoader is not java.lang.ClassLoader typedLoader)
        {
            throw new InvalidOperationException($"Failed obtaining Java class loader for assembly '{assembly.FullName}'.");
        }

        return typedLoader;
    }

	private void MaybeDefinePackage(string name)
	{
		int lastDot = name.LastIndexOf(".");
		if (lastDot < 0)
			return;

		string packageName = name[..lastDot];

		if (getPackage(packageName) == null)
		{
			definePackage(
				packageName,
				null, // specTitle
				null, // specVersion
				null, // specVendor
				null, // implTitle
				null, // implVersion
				null, // implVendor
				null  // sealBase
			);
		}
	}

	public byte[] TransformClassBytecode(string name, byte[] bytes)
	{
		foreach (var transformer in classTransformers)
		{
			if (!transformer.Classes.Any(x => name == x))
				continue;

			classloader_debug($"[IkvmClassLoader] applying transformer {transformer.Visitor} to '{name}'");

			try
			{
				org.objectweb.asm.ClassReader reader = new(bytes);
				org.objectweb.asm.ClassWriter writer = new(reader, org.objectweb.asm.ClassWriter.COMPUTE_FRAMES | org.objectweb.asm.ClassWriter.COMPUTE_MAXS);
				var visitor = (org.objectweb.asm.ClassVisitor)Activator.CreateInstance(transformer.Visitor, [name, writer]);
				reader.accept(visitor, 0);

				bytes = writer.toByteArray();
			}
			catch (Exception)
			{
				Console.Error.WriteLine($"[IkvmClassLoader] transformer {transformer.Visitor} failed on '{name}'");
				throw;
			}
		}

		return bytes;
	}

    protected override java.lang.Class findClass(string name)
    {
        if (classTransformers.Count == 0)
        {
            return base.findClass(name);
        }

        var resourcePath = name.Replace('.', '/') + ".class";
        var url = findResource(resourcePath);
        if (url is null)
        {
            throw new java.lang.ClassNotFoundException(name);
        }

        byte[] bytes;
        var stream = url.openStream();
        try
        {
            var output = new java.io.ByteArrayOutputStream();
            var buffer = new byte[8192];
            int n;
            while ((n = stream.read(buffer)) > 0)
            {
                output.write(buffer, 0, n);
            }
            bytes = output.toByteArray();
        }
        finally
        {
            stream.close();
        }

		bytes = TransformClassBytecode(name, bytes);

		java.net.URL codeSourceUrl = url;
		var urlStr = url.toString();
		if (urlStr.StartsWith("jar:", StringComparison.OrdinalIgnoreCase))
		{
			var jarPath = urlStr["jar:".Length..];
			var bangSlash = jarPath.IndexOf("!/", StringComparison.Ordinal);
			if (bangSlash >= 0)
				jarPath = jarPath[..bangSlash];
			codeSourceUrl = new java.net.URL(jarPath);
		}
		java.security.CodeSource codeSource = new(codeSourceUrl, (java.security.cert.Certificate[])null);
		java.security.ProtectionDomain protectionDomain = new(codeSource, null, this, null);

		MaybeDefinePackage(name);
        return defineClass(name, bytes, 0, bytes.Length, protectionDomain);
    }

	public override java.net.URL getResource(string name)
	{
		if (!name.EndsWith(".class"))
			return base.getResource(name);

		var klass = name[0..^6].Replace("/", ".");

		foreach (var (assemblyName, prefixes, _) in dllLoaders)
		{
			if (!prefixes.Any(x => klass.StartsWith(x)))
				continue;

            classloader_debug($"[IkvmClassLoader] '{name}' resource loaded from asm {assemblyName}");
			return new java.net.URL("assembly:", assemblyName, name);
		}

		return base.getResource(name);
	}

	public override java.util.Enumeration getResources(string name)
	{
		if (name.EndsWith(".class"))
		{
			var klass = name[0..^6].Replace("/", ".");

			foreach (var (assemblyName, prefixes, _) in dllLoaders)
			{
				if (!prefixes.Any(x => klass.StartsWith(x)))
					continue;

				classloader_debug($"[IkvmClassLoader] '{name}' resources resolved from asm {assemblyName}");
				var urls = new java.util.Vector();
				urls.add(new java.net.URL("assembly:", assemblyName, name));
				return urls.elements();
			}
		}

		return base.getResources(name);
	}

    protected override java.lang.Class loadClass(string name, bool resolve)
    {
        lock (getClassLoadingLock(name))
        {
            string loadedFrom = "[already loaded]";
            java.lang.Class cls = findLoadedClass(name);
			bool fastpath = false;

            if (name.StartsWith("java.", StringComparison.Ordinal) || name.StartsWith("javax.", StringComparison.Ordinal) || name.StartsWith("sun.", StringComparison.Ordinal))
            {
                if (cls is null)
                {
                    cls = systemLoader.loadClass(name);
                }

                if (resolve)
                {
                    resolveClass(cls);
                }

                return cls;
            }

            if (!fastpath && cls is null)
            {
                foreach (var (assemblyName, prefixes, loader) in dllLoaders)
                {
					if (!prefixes.Any(x => name.StartsWith(x)))
						continue;

					loadedFrom = $"assembly {assemblyName}";
					cls = loader.loadClass(name);
					fastpath = true;
					break;
                }
            }

            if (!fastpath && cls is null)
            {
                try
                {
                    cls = base.loadClass(name, resolve);
                    loadedFrom = "URLClassLoader";
                    resolve = false;
                }
                catch (java.lang.ClassNotFoundException cnf)
                {
                    Console.WriteLine($"[IkvmClassLoader] '{name}' miss in URLClassLoader: {cnf.getClass().getName()}: {cnf.getMessage()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[IkvmClassLoader] '{name}' EXCEPTION in URLClassLoader: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }

            if (cls is null)
            {
				cls = systemLoader.loadClass(name);
				loadedFrom = "system";
            }

            if (resolve)
            {
                resolveClass(cls);
            }

            classloader_debug($"[IkvmClassLoader] '{name}' loaded from {loadedFrom}");
            return cls;
        }
    }
}
