using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RitoClient.DllProxy
{
    internal static class DwriteDll
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int DWriteCreateFactoryFn(int factoryType, nint iid, nint factory);

        static DWriteCreateFactoryFn _pDWriteCreateFactory;

        static DwriteDll()
        {
            var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var dwritePath = Path.Combine(sysDir, "dwrite.dll");

            var lib = Native.LoadLibrary(dwritePath);
            var proc = Native.GetProcAddress(lib, nameof(DWriteCreateFactory));

            _pDWriteCreateFactory = Marshal.GetDelegateForFunctionPointer<DWriteCreateFactoryFn>(proc);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)], EntryPoint = nameof(DWriteCreateFactory))]
        static int DWriteCreateFactory(int factoryType, nint iid, nint factory)
        {
            return _pDWriteCreateFactory(factoryType, iid, factory);
        }
    }
}