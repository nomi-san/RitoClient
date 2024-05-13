using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RitoClient
{
    internal static class DllProxy
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int DWriteCreateFactoryFn(int factoryType, IntPtr iid, IntPtr factory);

        static DWriteCreateFactoryFn _DWriteCreateFactory;

        static DllProxy()
        {
            var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            var dwritePath = Path.Combine(sysDir, "dwrite.dll");

            var lib = Native.LoadLibraryA(dwritePath);
            var proc = Native.GetProcAddress(lib, nameof(DWriteCreateFactory));

            _DWriteCreateFactory = Marshal.GetDelegateForFunctionPointer<DWriteCreateFactoryFn>(proc);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)], EntryPoint = nameof(DWriteCreateFactory))]
        static int DWriteCreateFactory(int factoryType, nint iid, nint factory)
        {
            return _DWriteCreateFactory(factoryType, iid, factory);
        }
    }
}
