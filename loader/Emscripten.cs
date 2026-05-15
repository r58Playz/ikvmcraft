using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public static class Emscripten
{
    [DllImport("Emscripten")]
    private static extern void start_em_loop();

    private static TaskCompletionSource EmLoopTask;
    private static Func<bool> EmLoopCb;

    [UnmanagedCallersOnly(EntryPoint = "managed_em_loop_callback", CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int EmLoopCallback()
    {
        bool keepGoing;
        Exception caught = null;
        try
        {
            keepGoing = EmLoopCb();
        }
        catch (Exception ex)
        {
            caught = ex;
            keepGoing = false;
        }

        if (!keepGoing)
        {
            var tcs = EmLoopTask;
            EmLoopTask = null;
            if (caught is not null)
            {
                if (!tcs.TrySetException(caught))
                {
                    Console.Error.WriteLine("Failed to fault EmLoopTask");
                }
            }
            else
            {
                if (!tcs.TrySetResult())
                {
                    Console.Error.WriteLine("Failed to end EmLoopTask");
                }
            }
        }

        return keepGoing ? 1 : 0;
    }

    private static unsafe void CompileEmLoopCallback()
    {
        delegate* unmanaged[Cdecl]<int> fnPtr = &EmLoopCallback;
        Console.WriteLine($"EmLoopCb: {(IntPtr)fnPtr}");
    }

    public static Task RunEmLoop(Func<bool> runOneFrame)
    {
        if (EmLoopTask != null)
            throw new Exception("already running");

        CompileEmLoopCallback();

        EmLoopTask = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        EmLoopCb = runOneFrame;
        start_em_loop();

        return EmLoopTask.Task;
    }

    [DllImport("Emscripten")]
    internal extern static int mount_opfs();

    public static void MountOpfs()
    {
        int ret = mount_opfs();
        if (ret != 0)
        {
            throw new Exception($"Failed to mount OPFS: {ret}");
        }
    }

    [DllImport("Emscripten")]
    private extern static int mount_fetch(int id, string srcdir, string dstdir);
    [DllImport("Emscripten")]
    private extern static int mount_fetch_file(int id, string path);
    [DllImport("Emscripten")]
    private extern static int mount_fetch_dir(int id, string path);

	private static string[] FetchIDs = new string[8];

    public static void MountFetch(int id, string src, string dst)
    {
        int ret = mount_fetch(id, src, dst);
        if (ret != 0)
        {
            throw new Exception($"Failed to mount FetchFS from {src} to {dst}: {ret}");
        }
    }

    public static void MountFetchFile(int id, string path)
    {
        int ret = mount_fetch_file(id, path);
        if (ret != 0)
        {
            throw new Exception($"Failed to mount FetchFS file at {path}: {ret}");
        }
    }

    public static void MountFetchDir(int id, string path)
    {
        int ret = mount_fetch_dir(id, path);
        if (ret != 0)
        {
            throw new Exception($"Failed to mount FetchFS directory at {path}: {ret}");
        }
    }
}
