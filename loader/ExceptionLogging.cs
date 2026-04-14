using System;
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
        }
    }

    private static Throwable FindJavaThrowable(System.Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException)
        {
            if (current is Throwable throwable)
            {
                return throwable;
            }
        }

        return null;
    }
}
