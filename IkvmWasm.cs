using System;
using System.Reflection;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

static partial class IkvmWasm 
{
	internal static void Main()
    {
        Console.WriteLine(":3");
    }

    [JSExport]
    internal static async Task PreInit(string fetchbase)
    {
		Emscripten.MountOpfs();
		Emscripten.MountFetch(fetchbase + "/assets", "/assets");
    }

    [JSExport]
    internal static Task Run(bool app)
    {

    }
}
