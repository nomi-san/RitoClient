using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RitoClient
{
    internal static class Rundll32
    {
        public static bool IsRundll32 => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RUNDLL32"));

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int CreateProcessWFn(nint lpApplicationName, nint lpCommandLine,
            nint lpProcessAttributes, nint lpThreadAttributes, int bInheritHandles,
            uint dwCreationFlags, nint lpEnvironment, nint lpCurrentDirectory,
            nint lpStartupInfo, nint lpProcessInformation);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int CreateProcessAsUserWFn(nint hToken, nint lpApplicationName, nint lpCommandLine,
            nint lpProcessAttributes, nint lpThreadAttributes, int bInheritHandles,
            uint dwCreationFlags, nint lpEnvironment, nint lpCurrentDirectory,
            nint lpStartupInfo, nint lpProcessInformation);

        static Hook<CreateProcessWFn> CreateProcessW = new Hook<CreateProcessWFn>();
        static int Hooked_CreateProcessW(nint lpApplicationName, nint lpCommandLine,
            nint lpProcessAttributes, nint lpThreadAttributes, int bInheritHandles,
            uint dwCreationFlags, nint lpEnvironment, nint lpCurrentDirectory,
            nint lpStartupInfo, nint lpProcessInformation)
        {
            // see BootstrapEntryW
            dwCreationFlags |= 0x2;

            var ret = CreateProcessW.Call<int>(lpApplicationName, lpCommandLine,
                lpProcessAttributes, lpThreadAttributes, bInheritHandles,
                dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation);

            unsafe
            {
                var pi = (Native.PROCESS_INFORMATION*)lpProcessInformation;
                RemoveDebugger(pi->hProcess);
            }

            return ret;
        }

        static Hook<CreateProcessAsUserWFn> CreateProcessAsUserW = new Hook<CreateProcessAsUserWFn>();
        static int Hooked_CreateProcessAsUserW(nint hToken,
            nint lpApplicationName, nint lpCommandLine,
            nint lpProcessAttributes, nint lpThreadAttributes, int bInheritHandles,
            uint dwCreationFlags, nint lpEnvironment, nint lpCurrentDirectory,
            nint lpStartupInfo, nint lpProcessInformation)
        {
            // see BootstrapEntryW
            dwCreationFlags |= 0x2;

            var ret = CreateProcessAsUserW.Call<int>(hToken, lpApplicationName, lpCommandLine,
                lpProcessAttributes, lpThreadAttributes, bInheritHandles,
                dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation);

            unsafe
            {
                var pi = (Native.PROCESS_INFORMATION*)lpProcessInformation;
                RemoveDebugger(pi->hProcess);
            }

            return ret;
        }

        public static void LoadHooks()
        {
            // hook create process
            CreateProcessW.Install("kernel32.dll",
                nameof(CreateProcessW), Hooked_CreateProcessW);

            // CreateProcessAsUserW is used to create lowest privilege renderer processes
            CreateProcessAsUserW.Install("advapi32.dll",
                nameof(CreateProcessAsUserW), Hooked_CreateProcessAsUserW);
        }

        // Get called by rundll32 via IFEO debugger
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)], EntryPoint = nameof(BootstrapEntryW))]
        static int BootstrapEntryW(nint hwnd, nint inst, nint cmdLine, int _)
        {
            // mark the process as created by rundll32
            Environment.SetEnvironmentVariable("RUNDLL32", "1",
                EnvironmentVariableTarget.Process);

            var commandLine = Marshal.PtrToStringUni(cmdLine)!;

            var si = new Native.STARTUPINFO();
            si.cb = Marshal.SizeOf<Native.STARTUPINFO>();

            // create process with suspended & debug flags
            // when debugging, IFEO is not called recursively
            var success = Native.CreateProcessW(null, commandLine, IntPtr.Zero, IntPtr.Zero,
                false, 0x4 | 0x2, IntPtr.Zero, null, ref si, out var pi);

            if (success)
            {
                // remove debugger
                RemoveDebugger(pi.hProcess);
                // inject this dll to bypass IFEO for child proc
                InjectThisDll(pi.hProcess);

                Native.ResumeThread(pi.hThread);
                Native.WaitForSingleObject(pi.hProcess, uint.MaxValue);

                Native.CloseHandle(pi.hThread);
                Native.CloseHandle(pi.hProcess);

                return 0;
            }

            return -1;
        }

        static void InjectThisDll(nint process)
        {
            var dllPath = Module.ThisModulePath + "\0";
            var pathBytes = Encoding.Unicode.GetBytes(dllPath);

            var pathAddr = Native.VirtualAllocEx(process, 0, pathBytes.Length, 0x1000, 0x04);
            Native.WriteProcessMemory(process, pathAddr, pathBytes, pathBytes.Length, 0);

            var kernel32 = Native.GetModuleHandle("kernel32.dll");
            var loadLibraryW = Native.GetProcAddress(kernel32, "LoadLibraryW");

            var loader = Native.CreateRemoteThread(process, 0, 0, loadLibraryW, pathAddr, 0, 0);
            Native.WaitForSingleObject(loader, uint.MaxValue);
            Native.CloseHandle(loader);
        }

        static void RemoveDebugger(nint process)
        {
            if (Native.NtQueryInformationProcess(process,
                30,
                out var hdbg,
                IntPtr.Size,
                IntPtr.Zero) >= 0)
            {
                Native.NtRemoveProcessDebug(process, hdbg);
                Native.NtClose(hdbg);
            }
        }
    }
}