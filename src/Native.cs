using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RitoClient
{
    internal static partial class Native
    {
        /* kernel32.dll */

        [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr LoadLibrary(string name);

        [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr GetModuleHandle(string? name);

        [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleExW")]
        public static partial int GetModuleHandleEx(int flags, IntPtr addr, out IntPtr module);

        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr GetProcAddress(IntPtr module, string name);

        [LibraryImport("kernel32.dll")]
        public static partial IntPtr CreateRemoteThread(IntPtr hProcess,
           IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
           IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [LibraryImport("kernel32.dll")]
        public static partial IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool VirtualProtect(IntPtr lpAddress, nint dwSize, int flNewProtect, out int lpflOldProtect);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, Span<byte> lpBuffer, nint nSize, IntPtr _);

        [LibraryImport("kernel32.dll", EntryPoint = "GetModuleFileNameW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int GetModuleFileName(IntPtr hModule, Span<char> lpFilename, int nSize);

        [LibraryImport("kernel32.dll")]
        public static partial int DisableThreadLibraryCalls(IntPtr hmodule);

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public IntPtr lpReserved;
            public IntPtr lpDesktop;
            public IntPtr lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [LibraryImport("kernel32.dll", EntryPoint = "CreateProcessW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CreateProcess(string? lpApplicationName, string lpCommandLine,
            IntPtr lpProcessAttributes, IntPtr lpThreadAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
            uint dwCreationFlags, IntPtr lpEnvironment, string? lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [LibraryImport("kernel32.dll")]
        public static partial uint ResumeThread(IntPtr handle);

        [LibraryImport("kernel32.dll")]
        public static partial uint WaitForSingleObject(IntPtr handle, uint timeout);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CloseHandle(IntPtr handle);


        /* ntdll.dll */

        [LibraryImport("ntdll.dll")]
        public static partial int NtQueryInformationProcess(IntPtr process, uint flag, out IntPtr debug, int size, IntPtr _);

        [LibraryImport("ntdll.dll")]
        public static partial int NtRemoveProcessDebug(IntPtr process, IntPtr debug);

        [LibraryImport("ntdll.dll")]
        public static partial int NtClose(IntPtr handle);


        /* msvcrt.dll */

        [LibraryImport("msvcrt.dll", EntryPoint = "memcpy")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        public static partial IntPtr memcpy(IntPtr dest, IntPtr src, IntPtr count);


        /* user32.dll */

        [LibraryImport("user32.dll", EntryPoint = "FindWindowW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr FindWindow(string lpClassName, string lpWindowName);

        [LibraryImport("user32.dll")]
        public static partial int GetWindowThreadProcessId(IntPtr hWnd, out int pid);

        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        public static partial IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        public static partial IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [LibraryImport("user32.dll", EntryPoint = "CallWindowProcW")]
        public static /*unsafe*/ partial IntPtr CallWindowProc(
            IntPtr /*delegate* unmanaged[Cdecl]<nint, IntPtr, uint, IntPtr, IntPtr>*/ lpPrevWndFunc,
            IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [LibraryImport("user32.dll")]
        public static partial short GetKeyState(int key);

        public static ushort HIWORD(IntPtr value)
        {
            return unchecked((ushort)((((long)(value)) >> 16) & 0xffff));
        }
    }
}
