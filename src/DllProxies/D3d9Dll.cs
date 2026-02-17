using System.Runtime.InteropServices;

namespace RitoClient.DllProxies;

static class D3d9Dll
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate nint Direct3DCreate9Fn(uint sdkVersion);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int Direct3DCreate9ExFn(uint sdkVersion, nint d3d9ex);

    static Direct3DCreate9Fn _pDirect3DCreate9;
    static Direct3DCreate9ExFn _pDirect3DCreate9Ex;

    static D3d9Dll()
    {
        var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
        var d3d9Path = Path.Combine(sysDir, "d3d9.dll");

        var lib = Native.LoadLibrary(d3d9Path);

        var create9Proc = Native.GetProcAddress(lib, nameof(Direct3DCreate9));
        _pDirect3DCreate9 = Marshal.GetDelegateForFunctionPointer<Direct3DCreate9Fn>(create9Proc);

        var create9ExProc = Native.GetProcAddress(lib, nameof(Direct3DCreate9Ex));
        _pDirect3DCreate9Ex = Marshal.GetDelegateForFunctionPointer<Direct3DCreate9ExFn>(create9ExProc);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(Direct3DCreate9))]
    static nint Direct3DCreate9(uint sdkVersion)
    {
        return _pDirect3DCreate9(sdkVersion);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(Direct3DCreate9Ex))]
    static int Direct3DCreate9Ex(uint sdkVersion, nint d3d9ex)
    {
        return _pDirect3DCreate9Ex(sdkVersion, d3d9ex);
    }
}