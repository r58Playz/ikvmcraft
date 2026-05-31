using System;
using System.Collections.Generic;
using java.lang;

internal static class ExceptionLogging
{
    internal static void WriteException(System.Exception ex, string header)
    {
        if (!string.IsNullOrEmpty(header))
        {
            Console.Error.WriteLine(header);
        }
        Console.Error.WriteLine(ex);
        var javaThrowable = FindJavaThrowable(ex);
        if (javaThrowable != null)
        {
			javaThrowable.printStackTrace();
            PrintJavaThrowableChain(javaThrowable);
        }
    }

    private static void PrintJavaThrowableChain(Throwable t)
    {
        var seen = new HashSet<Throwable>(ReferenceEqualityComparer.Instance);
        var current = t;
        var prefix = "";
        while (current != null && seen.Add(current))
        {
            var msg = current.getMessage();
            var className = current.getClass().getName();
            Console.Error.WriteLine($"{prefix}{className}{(msg != null ? ": " + msg : "")}");

            var stackTrace = current.getStackTrace();
            foreach (var frame in stackTrace)
            {
                Console.Error.WriteLine($"\tat {frame}");
            }

            current = current.getCause() as Throwable;
            prefix = "Caused by: ";
        }
    }

    private static Throwable FindJavaThrowable(System.Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException)
        {
            if (current is Throwable throwable)
                return throwable;
        }
        return null;
    }
}
