using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IKVM.Runtime;

internal sealed class IkvmClassLoader : java.net.URLClassLoader
{
    private readonly java.lang.ClassLoader systemLoader;
    private readonly List<(string Name, java.lang.ClassLoader Loader)> managedAssemblyLoaders;

    public IkvmClassLoader(string[] jars, string[] managedAssemblyNames)
        : base((from jar in jars select new java.net.URL("file", "", jar)).ToArray(), null)
    {
        managedAssemblyLoaders =
        [
            .. from assemblyName in managedAssemblyNames
               select (assemblyName, CreateAssemblyClassLoader(Assembly.Load(assemblyName))),
        ];
        systemLoader = java.lang.ClassLoader.getSystemClassLoader();
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

    protected override java.lang.Class loadClass(string name, bool resolve)
    {
        lock (getClassLoadingLock(name))
        {
            string loadedFrom = "[already loaded]";
            java.lang.Class cls = findLoadedClass(name);

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

            if (cls is null)
            {
                foreach (var (assemblyName, loader) in managedAssemblyLoaders)
                {
                    try
                    {
                        cls = loader.loadClass(name);
                        if (cls is not null)
                        {
                            loadedFrom = $"assembly {assemblyName}";
                            break;
                        }
                    }
                    catch (java.lang.ClassNotFoundException)
                    {
                    }
                }
            }

            if (cls is null)
            {
                try
                {
                    cls = base.loadClass(name, resolve);
                    loadedFrom = "URLClassLoader";
                    resolve = false;
                }
                catch (java.lang.ClassNotFoundException)
                {
                }
            }

            if (cls is null)
            {
                try
                {
                    cls = systemLoader.loadClass(name);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"[IkvmClassLoader] '{name}' failed to load from system loader");
                    throw;
                }
                loadedFrom = "system";
            }

            if (resolve)
            {
                resolveClass(cls);
            }

            Console.WriteLine($"[IkvmClassLoader] '{name}' loaded from {loadedFrom}");
            return cls;
        }
    }
}
