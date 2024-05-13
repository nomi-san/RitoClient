using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RitoClient
{
    internal static class DllMain
    {
        static int DebuggingPort = 50777;
        static IntPtr GlobalCommandLine = 0;
        static Debugger? Debugger;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate IntPtr GetCommandLineWFn();

        static Hook<GetCommandLineWFn> GetCommandLineW = new Hook<GetCommandLineWFn>();
        static IntPtr Hooked_GetCommandLineW()
        {
            if (GlobalCommandLine == 0)
            {
                var cmdline = GetCommandLineW.Call<IntPtr>();
                var text = Marshal.PtrToStringUni(cmdline);
                // enable remote debugger
                text += $" --remote-debugging-port={DebuggingPort}";

                GlobalCommandLine = Marshal.StringToHGlobalUni(text);
            }

            return GlobalCommandLine;
        }

        static void Initialize()
        {
            // find free tcp port for debugger
            DebuggingPort = Utils.GetFreeTcpPort();

            // hook internal command line
            GetCommandLineW.Install("kernel32.dll",
                nameof(GetCommandLineW), Hooked_GetCommandLineW);

            Task.Run(async () =>
            {
                Debugger = new Debugger(DebuggingPort);
                await Debugger.Connect();

                //await Debugger.DevTools.SetBypassCSP(true);
                //await Debugger.DevTools.ReloadPage(true);
                await InjectScripts(Debugger.DevTools);
            });

            SetupWindow();
        }

        static async Task InjectScripts(DevTools devTools)
        {
            var path = Module.ThisModulePath;
            var dir = Path.Combine(Path.GetDirectoryName(path)!, "preload");

            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*.js");
                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    var script = File.ReadAllText(file, System.Text.Encoding.UTF8)
                        + $"\n\n//# sourceURL=@preload/{filename}"; // for debugging script

                    await devTools.EvaluateScript(script);
                }
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)], EntryPoint = nameof(DllMain))]
        static int Entry(IntPtr hinst, uint reason, IntPtr _)
        {
            if (reason == 1)
            {
                Native.DisableThreadLibraryCalls(hinst);
                var args = Environment.CommandLine;

                if (args.Contains("--app-port=")
                    && args.Contains("--remoting-auth-token="))
                {
                    Initialize();
                }
            }

            return 1;
        }

        static void SetupWindow()
        {
            Task.Run(async () =>
            {
                var pid = Process.GetCurrentProcess().Id;

                while (true)
                {
                    var hwnd = Native.FindWindow("Chrome_WidgetWin_1", "Riot Client");
                    Native.GetWindowThreadProcessId(hwnd, out var pid2);

                    if (hwnd != 0 && pid == pid2)
                    {
                        MessageBox.Owner = hwnd;

                        OldWndProc = Native.GetWindowLongPtr(hwnd, -4);
                        Native.SetWindowLongPtr(hwnd, -4, Marshal.GetFunctionPointerForDelegate(HookWndProc));

                        break;
                    }

                    await Task.Delay(100);
                }
            });
        }

        static IntPtr OldWndProc = 0!;
        static Native.WndProc HookWndProc = delegate (IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == 0x0100) // WM_KEYDOWN
            {
                if ((Native.HIWORD(lparam) & 0x4000) == 0   // no-repeat
                    && Native.GetKeyState(0x10) < 0         // shift
                    && Native.GetKeyState(0x11) < 0)        // ctrl
                {
                    switch (wparam)
                    {
                        case 'I':
                            Debugger?.OpenRemoteDevTools();
                            return 0;

                        case 'R':
                            Debugger?.DevTools.ReloadPage();
                            return 0;
                    }
                }
            }

            return Native.CallWindowProc(OldWndProc, hwnd, msg, wparam, lparam);
        };
    }
}
