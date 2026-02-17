using System.Runtime.InteropServices;

namespace RitoClient.DllProxies;

public static class VersionDll
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int GetFileVersionInfoWFn(nint lptstrFilename, uint dwHandle, uint dwLen, nint lpData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint GetFileVersionInfoSizeWFn(nint lptstrFilename, nint lpdwHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int GetFileVersionInfoAFn(nint lptstrFilename, uint dwHandle, uint dwLen, nint lpData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint GetFileVersionInfoSizeAFn(nint lptstrFilename, nint lpdwHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int GetFileVersionInfoExWFn(uint dwFlags, nint lpFileName, uint dwHandle, uint dwLen, nint lpData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint GetFileVersionInfoSizeExWFn(uint dwFlags, nint lpFileName, nint lpdwHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int GetFileVersionInfoExAFn(uint dwFlags, nint lpFileName, uint dwHandle, uint dwLen, nint lpData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint GetFileVersionInfoSizeExAFn(uint dwFlags, nint lpFileName, nint lpdwHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int VerQueryValueWFn(nint pBlock, nint lpSubBlock, nint lplpBuffer, nint puLen);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate int VerQueryValueAFn(nint pBlock, nint lpSubBlock, nint lplpBuffer, nint puLen);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint VerLanguageNameWFn(uint wLang, nint szLang, uint cchLang);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint VerLanguageNameAFn(uint wLang, nint szLang, uint cchLang);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint VerFindFileWFn(uint uFlags, nint szFileName, nint szWinDir, nint szAppDir, nint szCurDir, nint lpuCurDirLen, nint szDestDir, nint lpuDestDirLen);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint VerFindFileAFn(uint uFlags, nint szFileName, nint szWinDir, nint szAppDir, nint szCurDir, nint lpuCurDirLen, nint szDestDir, nint lpuDestDirLen);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint VerInstallFileWFn(uint uFlags, nint szSrcFileName, nint szDestFileName, nint szSrcDir, nint szDestDir, nint szCurDir, nint szTmpFile, nint lpuTmpFileLen);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate uint VerInstallFileAFn(uint uFlags, nint szSrcFileName, nint szDestFileName, nint szSrcDir, nint szDestDir, nint szCurDir, nint szTmpFile, nint lpuTmpFileLen);

    static GetFileVersionInfoWFn _pGetFileVersionInfoW;
    static GetFileVersionInfoSizeWFn _pGetFileVersionInfoSizeW;
    static GetFileVersionInfoAFn _pGetFileVersionInfoA;
    static GetFileVersionInfoSizeAFn _pGetFileVersionInfoSizeA;
    static GetFileVersionInfoExWFn _pGetFileVersionInfoExW;
    static GetFileVersionInfoSizeExWFn _pGetFileVersionInfoSizeExW;
    static GetFileVersionInfoExAFn _pGetFileVersionInfoExA;
    static GetFileVersionInfoSizeExAFn _pGetFileVersionInfoSizeExA;
    static VerQueryValueWFn _pVerQueryValueW;
    static VerQueryValueAFn _pVerQueryValueA;
    static VerLanguageNameWFn _pVerLanguageNameW;
    static VerLanguageNameAFn _pVerLanguageNameA;
    static VerFindFileWFn _pVerFindFileW;
    static VerFindFileAFn _pVerFindFileA;
    static VerInstallFileWFn _pVerInstallFileW;
    static VerInstallFileAFn _pVerInstallFileA;

    static VersionDll()
    {
        var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
        var versionPath = Path.Combine(sysDir, "version.dll");

        var lib = Native.LoadLibrary(versionPath);

        _pGetFileVersionInfoW = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoWFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoW)));
        _pGetFileVersionInfoSizeW = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoSizeWFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoSizeW)));
        _pGetFileVersionInfoA = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoAFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoA)));
        _pGetFileVersionInfoSizeA = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoSizeAFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoSizeA)));
        _pGetFileVersionInfoExW = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoExWFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoExW)));
        _pGetFileVersionInfoSizeExW = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoSizeExWFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoSizeExW)));
        _pGetFileVersionInfoExA = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoExAFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoExA)));
        _pGetFileVersionInfoSizeExA = Marshal.GetDelegateForFunctionPointer<GetFileVersionInfoSizeExAFn>(Native.GetProcAddress(lib, nameof(GetFileVersionInfoSizeExA)));
        _pVerQueryValueW = Marshal.GetDelegateForFunctionPointer<VerQueryValueWFn>(Native.GetProcAddress(lib, nameof(VerQueryValueW)));
        _pVerQueryValueA = Marshal.GetDelegateForFunctionPointer<VerQueryValueAFn>(Native.GetProcAddress(lib, nameof(VerQueryValueA)));
        _pVerLanguageNameW = Marshal.GetDelegateForFunctionPointer<VerLanguageNameWFn>(Native.GetProcAddress(lib, nameof(VerLanguageNameW)));
        _pVerLanguageNameA = Marshal.GetDelegateForFunctionPointer<VerLanguageNameAFn>(Native.GetProcAddress(lib, nameof(VerLanguageNameA)));
        _pVerFindFileW = Marshal.GetDelegateForFunctionPointer<VerFindFileWFn>(Native.GetProcAddress(lib, nameof(VerFindFileW)));
        _pVerFindFileA = Marshal.GetDelegateForFunctionPointer<VerFindFileAFn>(Native.GetProcAddress(lib, nameof(VerFindFileA)));
        _pVerInstallFileW = Marshal.GetDelegateForFunctionPointer<VerInstallFileWFn>(Native.GetProcAddress(lib, nameof(VerInstallFileW)));
        _pVerInstallFileA = Marshal.GetDelegateForFunctionPointer<VerInstallFileAFn>(Native.GetProcAddress(lib, nameof(VerInstallFileA)));
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoW))]
    static int GetFileVersionInfoW(nint lptstrFilename, uint dwHandle, uint dwLen, nint lpData)
    {
        return _pGetFileVersionInfoW(lptstrFilename, dwHandle, dwLen, lpData);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeW))]
    static uint GetFileVersionInfoSizeW(nint lptstrFilename, nint lpdwHandle)
    {
        return _pGetFileVersionInfoSizeW(lptstrFilename, lpdwHandle);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoA))]
    static int GetFileVersionInfoA(nint lptstrFilename, uint dwHandle, uint dwLen, nint lpData)
    {
        return _pGetFileVersionInfoA(lptstrFilename, dwHandle, dwLen, lpData);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeA))]
    static uint GetFileVersionInfoSizeA(nint lptstrFilename, nint lpdwHandle)
    {
        return _pGetFileVersionInfoSizeA(lptstrFilename, lpdwHandle);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoExW))]
    static int GetFileVersionInfoExW(uint dwFlags, nint lpFileName, uint dwHandle, uint dwLen, nint lpData)
    {
        return _pGetFileVersionInfoExW(dwFlags, lpFileName, dwHandle, dwLen, lpData);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeExW))]
    static uint GetFileVersionInfoSizeExW(uint dwFlags, nint lpFileName, nint lpdwHandle)
    {
        return _pGetFileVersionInfoSizeExW(dwFlags, lpFileName, lpdwHandle);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoExA))]
    static int GetFileVersionInfoExA(uint dwFlags, nint lpFileName, uint dwHandle, uint dwLen, nint lpData)
    {
        return _pGetFileVersionInfoExA(dwFlags, lpFileName, dwHandle, dwLen, lpData);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetFileVersionInfoSizeExA))]
    static uint GetFileVersionInfoSizeExA(uint dwFlags, nint lpFileName, nint lpdwHandle)
    {
        return _pGetFileVersionInfoSizeExA(dwFlags, lpFileName, lpdwHandle);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerQueryValueW))]
    static int VerQueryValueW(nint pBlock, nint lpSubBlock, nint lplpBuffer, nint puLen)
    {
        return _pVerQueryValueW(pBlock, lpSubBlock, lplpBuffer, puLen);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerQueryValueA))]
    static int VerQueryValueA(nint pBlock, nint lpSubBlock, nint lplpBuffer, nint puLen)
    {
        return _pVerQueryValueA(pBlock, lpSubBlock, lplpBuffer, puLen);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerLanguageNameW))]
    static uint VerLanguageNameW(uint wLang, nint szLang, uint cchLang)
    {
        return _pVerLanguageNameW(wLang, szLang, cchLang);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerLanguageNameA))]
    static uint VerLanguageNameA(uint wLang, nint szLang, uint cchLang)
    {
        return _pVerLanguageNameA(wLang, szLang, cchLang);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerFindFileW))]
    static uint VerFindFileW(uint uFlags, nint szFileName, nint szWinDir, nint szAppDir, nint szCurDir, nint lpuCurDirLen, nint szDestDir, nint lpuDestDirLen)
    {
        return _pVerFindFileW(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerFindFileA))]
    static uint VerFindFileA(uint uFlags, nint szFileName, nint szWinDir, nint szAppDir, nint szCurDir, nint lpuCurDirLen, nint szDestDir, nint lpuDestDirLen)
    {
        return _pVerFindFileA(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerInstallFileW))]
    static uint VerInstallFileW(uint uFlags, nint szSrcFileName, nint szDestFileName, nint szSrcDir, nint szDestDir, nint szCurDir, nint szTmpFile, nint lpuTmpFileLen)
    {
        return _pVerInstallFileW(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(VerInstallFileA))]
    static uint VerInstallFileA(uint uFlags, nint szSrcFileName, nint szDestFileName, nint szSrcDir, nint szDestDir, nint szCurDir, nint szTmpFile, nint lpuTmpFileLen)
    {
        return _pVerInstallFileA(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
    }
}