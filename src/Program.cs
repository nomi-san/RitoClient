using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RitoClient;

static class Program
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate nint GetCommandLineWFn();

    static nint CommandLine;
    static string? ExtraArgs;

    static Hook<GetCommandLineWFn> GetCommandLineW = new();
    static GetCommandLineWFn NewGetCommandLineW = () =>
    {
        if (CommandLine == 0)
        {
            using var call = GetCommandLineW.GetCall();
            var full = Marshal.PtrToStringUni(call.Func());
            full += ExtraArgs;

            CommandLine = Marshal.StringToHGlobalUni(full);
        }

        return CommandLine;
    };

    public static void Main()
    {
        Config.Load();

        var portHolder = NetUtil.GetFreeTcpPort(out int debugPort);
        ExtraArgs = $" --remote-debugging-port={debugPort}";

        if (Config.I.potato_mode)
        {
            ExtraArgs += " --disable-smooth-scrolling --force-prefers-reduced-motion";
            ExtraArgs += " --wm-window-animations-disabled --animation-duration-scale=0";
        }

        GetCommandLineW.Install("kernel32", "GetCommandLineW", NewGetCommandLineW);
        portHolder.Dispose();

        //var server = new App.WebServer(webPort);
        var debugger = new Debugger(debugPort, 0, false);

        //server.Listen();
        Task.Run(debugger.Connect);

        Window.SetupWindow(debugger);
    }

#if DEBUG
    [ModuleInitializer]
#endif
    public static void Initialize()
    {
        var process = Process.GetCurrentProcess();
        var exe = process.MainModule!.ModuleName;
        var args = Environment.CommandLine;

        if (exe.Equals("Riot Client.exe", StringComparison.OrdinalIgnoreCase)
            && args.Contains("--app-port=")
            && args.Contains("--remoting-auth-token="))
        {
            Rundll32.LoadHooks();
            Main();
        }
    }

    //static async Task InjectScripts(DevTools devTools)
    //{
    //    var path = Module.ThisModulePath;
    //    var dir = Path.Combine(Path.GetDirectoryName(path)!, "preload");

    //    if (Directory.Exists(dir))
    //    {
    //        var files = Directory.GetFiles(dir, "*.js");
    //        foreach (var file in files)
    //        {
    //            var filename = Path.GetFileName(file);
    //            var script = File.ReadAllText(file, System.Text.Encoding.UTF8)
    //                + $"\n\n//# sourceURL=@preload/{filename}"; // for debugging script

    //            await devTools.EvaluateScript(script);
    //            await devTools.AddPreloadScript(script);
    //        }
    //    }
    //}

#if !DEBUG
    [UnmanagedCallersOnly(EntryPoint = "DllMain")]
#endif
    public static int Entry(IntPtr hinst, uint reason, IntPtr _)
    {
        if (reason == 1)
        {
            Native.DisableThreadLibraryCalls(hinst);
            Initialize();
        }

        return 1;
    }
}