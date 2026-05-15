using System;
using System.Threading.Tasks;

/// <summary>
/// Control-flow signal thrown after <c>emscripten_set_main_loop</c> is
/// registered. Unwinds the Java stack out of <c>Minecraft.run()</c> and
/// <c>Main.main</c> so the JS event loop can take over and start firing tick
/// callbacks; the attached <see cref="EmLoopTask"/> rides along so the C#
/// JSExport can return it to JS for awaiting.
///
/// Visible to Java as <c>cli.IkvmEmLoopStarted</c>. Extends
/// <see cref="java.lang.Error"/> so it can be thrown from any call site
/// without a <c>throws</c> declaration.
/// </summary>
public sealed class IkvmEmLoopStarted : java.lang.Error
{
    public Task EmLoopTask { get; }

    public IkvmEmLoopStarted(Task emLoopTask)
        : base("em-loop started; control returned to JS event loop")
    {
        EmLoopTask = emLoopTask;
    }
}

/// <summary>
/// Java-callable bridge. Exposed to Java bytecode as <c>cli.IkvmBridge</c>
/// (IKVM's standard mapping for .NET types). The class loader routes that name
/// to the running assembly — see <see cref="IkvmClassLoader"/>'s constructor.
///
/// Members on this class are reflected into by injected bytecode; keep names
/// stable, or update both sides together.
/// </summary>
public static class IkvmBridge
{
    /// <summary>
    /// Drive Minecraft's render thread via <see cref="Emscripten.RunEmLoop"/>.
    /// Replaces the original <c>while (this.running) runTick(...)</c> body —
    /// each frame, the browser's <c>requestAnimationFrame</c> fires one tick.
    ///
    /// Because field/method names get obfuscated, the caller passes them in
    /// (the ASM transformer baked them in as constants when it patched the
    /// callsite, so they match this build of the jar).
    ///
    /// <paramref name="minecraft"/> is the Minecraft instance.
    /// <paramref name="runTickMethodName"/> is the obfuscated name of
    /// <c>void runTick(boolean)</c>.
    /// <paramref name="runningFieldName"/> is the obfuscated name of
    /// <c>volatile boolean running</c>.
    /// </summary>
    public static void RunMinecraftEmLoop(java.lang.Object minecraft, string runTickMethodName, string runningFieldName)
    {
        if (minecraft is null) throw new ArgumentNullException(nameof(minecraft));

        var cls = minecraft.getClass();
        Console.WriteLine($"[IkvmBridge] starting em-loop driver for {cls.getName()} (runTick='{runTickMethodName}', running='{runningFieldName}')");

        var runTick = cls.getDeclaredMethod(runTickMethodName, new java.lang.Class[] { java.lang.Boolean.TYPE });
        runTick.setAccessible(true);

        var runningField = cls.getDeclaredField(runningFieldName);
        runningField.setAccessible(true);

        // Boxed once and reused — the original loop passed `!flag` where flag
        // starts false and only ever flips inside the OOM catch. Normal-path
        // behavior is `runTick(true)` every frame, which is what we replicate.
        var trueArg = new object[] { java.lang.Boolean.TRUE };

        // Don't catch inside the tick — let exceptions propagate to
        // Emscripten.RunEmLoop's callback, which faults the Task. We wrap
        // that Task below so the rich Java stack trace gets logged before
        // the JS-side awaiter sees the fault.
        var rawTask = Emscripten.RunEmLoop(() =>
        {
            runTick.invoke(minecraft, trueArg);
            return runningField.getBoolean(minecraft);
        });

        // emscripten_set_main_loop is registered no-simulate-infinite-loop, so
        // it returns immediately. We throw a marker carrying the wrapped Task
        // — that unwinds the Java stack past Minecraft.run() and Main.main
        // without running their post-run() teardown. The C# JSExport (Run)
        // catches the marker and returns the Task to JS for awaiting.
        throw new IkvmEmLoopStarted(WrapWithRichLogging(rawTask, "[IkvmBridge] em-loop terminated with exception"));
    }

    /// <summary>
    /// Continuation that calls <see cref="ExceptionLogging.WriteException"/>
    /// on fault — which unwraps and runs <c>printStackTrace()</c> on any Java
    /// <see cref="java.lang.Throwable"/> in the chain — before propagating
    /// the fault on a fresh TCS so the original exception (not an
    /// <see cref="AggregateException"/>) reaches the awaiter.
    /// </summary>
    private static Task WrapWithRichLogging(Task task, string header)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                var inner = (Exception)t.Exception.InnerException ?? t.Exception;
                ExceptionLogging.WriteException(inner, header);
                tcs.TrySetException(inner);
            }
            else if (t.IsCanceled)
            {
                tcs.TrySetCanceled();
            }
            else
            {
                tcs.TrySetResult();
            }
        }, TaskContinuationOptions.ExecuteSynchronously);
        return tcs.Task;
    }
}
