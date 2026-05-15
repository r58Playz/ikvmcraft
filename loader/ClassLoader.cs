global using IkvmClassLoaderDll = (string[] Prefixes, string Name);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using IKVM.Runtime;

internal partial class ClassLoaderLogging
{
    [JSImport("globalThis.console.debug")]
    private static partial void DebugLog(string message);

    public static void Debug(string message)
    {
        try
        {
            DebugLog(message);
        }
        catch
        {
            // Fallback to Console if JS interop fails
            Console.WriteLine(message);
        }
    }
}

/// <summary>
/// Transforms a class file's bytes before <c>defineClass</c> sees them.
/// Return the original array (or null) to leave the class untouched.
/// </summary>
internal delegate byte[] ClassFileTransformer(string className, byte[] classBytes);

internal sealed class IkvmClassLoader : java.net.URLClassLoader
{
    private readonly java.lang.ClassLoader systemLoader;
    private readonly List<(string Name, string[] Prefixes, java.lang.ClassLoader Loader)> dllLoaders;
    private readonly List<ClassFileTransformer> transformers = new();

    public IkvmClassLoader(string[] jars, IkvmClassLoaderDll[] dlls)
        : base((from jar in jars select new java.net.URL("file", "", jar)).ToArray(), null)
    {
        // Route cli.IkvmBridge (and any future cli.* helpers added to this
        // assembly) through the running assembly's class loader so injected
        // Java bytecode can resolve them.
        var selfLoader = CreateAssemblyClassLoader(typeof(IkvmClassLoader).Assembly);

        dllLoaders =
        [
            ("IkvmWasm.self", new[] { "cli.Ikvm" }, selfLoader),
            .. from dll in dlls
               select (dll.Name, dll.Prefixes, CreateAssemblyClassLoader(Assembly.Load(dll.Name))),
        ];
        systemLoader = java.lang.ClassLoader.getSystemClassLoader();
    }

    /// <summary>
    /// Register a bytecode transformer. Transformers run in registration order on
    /// classes resolved through the URL classpath only (system / DLL-routed
    /// classes are not visited, since their bytes never pass through here).
    /// </summary>
    public IkvmClassLoader AddTransformer(ClassFileTransformer transformer)
    {
        if (transformer is not null)
        {
            transformers.Add(transformer);
        }
        return this;
    }

    /// <summary>
    /// Register an ASM-based transformer. <paramref name="buildVisitor"/> receives
    /// the downstream <c>ClassWriter</c> and returns the head of a visitor chain
    /// (typically a subclass of <c>org.objectweb.asm.ClassVisitor</c>) that will
    /// be driven by a <c>ClassReader</c> over the original bytes. Returning the
    /// writer unchanged is a no-op pass-through.
    /// </summary>
    public IkvmClassLoader AddAsmTransformer(
        Predicate<string> classFilter,
        Func<org.objectweb.asm.ClassWriter, org.objectweb.asm.ClassVisitor> buildVisitor)
    {
        if (buildVisitor is null)
        {
            throw new ArgumentNullException(nameof(buildVisitor));
        }

        return AddTransformer((name, bytes) =>
        {
            if (classFilter is not null && !classFilter(name))
            {
                return bytes;
            }

            var reader = new org.objectweb.asm.ClassReader(bytes);
            var writer = new org.objectweb.asm.ClassWriter(reader, org.objectweb.asm.ClassWriter.COMPUTE_FRAMES);
            var head = buildVisitor(writer) ?? writer;
            reader.accept(head, 0);
            return writer.toByteArray();
        });
    }

    private static java.lang.ClassLoader CreateAssemblyClassLoader(Assembly assembly)
    {
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

    protected override java.lang.Class findClass(string name)
    {
        if (transformers.Count == 0)
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

        var transformed = false;
        foreach (var transformer in transformers)
        {
            byte[] result;
            try
            {
                result = transformer(name, bytes);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"[IkvmClassLoader] transformer threw on '{name}': {e}");
                throw;
            }

            if (result is not null && !ReferenceEquals(result, bytes))
            {
                bytes = result;
                transformed = true;
            }
        }

        if (transformed)
        {
            ClassLoaderLogging.Debug($"[IkvmClassLoader] transformed '{name}' ({bytes.Length} bytes)");
        }

        return defineClass(name, bytes, 0, bytes.Length);
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

					try {
						cls = loader.loadClass(name);
						loadedFrom = $"assembly {assemblyName}";
						fastpath = true;
					} catch (Exception e)
					{
						Console.Error.WriteLine($"[IkvmClassLoader] '{name}' miss in assembly '{assemblyName}': {e}");
						throw;
					}
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
                    Console.Error.WriteLine($"[IkvmClassLoader] '{name}' miss in URLClassLoader: {cnf.getClass().getName()}: {cnf.getMessage()}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[IkvmClassLoader] '{name}' EXCEPTION in URLClassLoader: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                }
            }

            if (cls is null)
            {
                try
                {
                    cls = systemLoader.loadClass(name);
					loadedFrom = "system";
                }
                catch (java.lang.ClassNotFoundException cnf)
                {
                    Console.Error.WriteLine($"[IkvmClassLoader] '{name}' miss in system loader: {cnf.getClass().getName()}: {cnf.getMessage()}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[IkvmClassLoader] '{name}' EXCEPTION in system loader: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                    throw;
                }
            }

            if (resolve)
            {
                resolveClass(cls);
            }

            ClassLoaderLogging.Debug($"[IkvmClassLoader] '{name}' loaded from {loadedFrom}");
            return cls;
        }
    }
}
