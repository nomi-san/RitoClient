#include <windows.h>

static HMODULE GetModule()
{
    static HMODULE mod = NULL;

    if (!mod)
    {
        WCHAR dllPath[MAX_PATH + 1];
        GetSystemDirectoryW(dllPath, MAX_PATH);
        lstrcatW(dllPath, L"\\version.dll");
        mod = LoadLibraryW(dllPath);
    }

    return mod;
}

template<typename T>
static T Forward(T, const char* fn)
{
    static T proc = nullptr;
    if (proc != nullptr) return proc;
    return proc = reinterpret_cast<T>(GetProcAddress(GetModule(), fn));
}

BOOL WINAPI GetFileVersionInfoA(LPCSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return Forward(GetFileVersionInfoA, __FUNCTION__)(lptstrFilename, dwHandle, dwLen, lpData);
}

int WINAPI GetFileVersionInfoByHandle(int hMem, LPCWSTR lpFileName, int v2, int v3)
{
    return Forward(GetFileVersionInfoByHandle, __FUNCTION__)(hMem, lpFileName, v2, v3);
}

BOOL WINAPI GetFileVersionInfoExA(DWORD dwFlags, LPCSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return Forward(GetFileVersionInfoExA, __FUNCTION__)(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
}

BOOL WINAPI GetFileVersionInfoExW(DWORD dwFlags, LPCWSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return Forward(GetFileVersionInfoExW, __FUNCTION__)(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
}

DWORD WINAPI GetFileVersionInfoSizeA(LPCSTR lptstrFilename, LPDWORD lpdwHandle)
{
    return Forward(GetFileVersionInfoSizeA, __FUNCTION__)(lptstrFilename, lpdwHandle);
}

DWORD WINAPI GetFileVersionInfoSizeExA(DWORD dwFlags, LPCSTR lpwstrFilename, LPDWORD lpdwHandle)
{
    return Forward(GetFileVersionInfoSizeExA, __FUNCTION__)(dwFlags, lpwstrFilename, lpdwHandle);
}

DWORD WINAPI GetFileVersionInfoSizeExW(DWORD dwFlags, LPCWSTR lpwstrFilename, LPDWORD lpdwHandle)
{
    return Forward(GetFileVersionInfoSizeExW, __FUNCTION__)(dwFlags, lpwstrFilename, lpdwHandle);
}

DWORD WINAPI GetFileVersionInfoSizeW(LPCWSTR lptstrFilename, LPDWORD lpdwHandle)
{
    return Forward(GetFileVersionInfoSizeW, __FUNCTION__)(lptstrFilename, lpdwHandle);
}

BOOL WINAPI GetFileVersionInfoW(LPCWSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return Forward(GetFileVersionInfoW, __FUNCTION__)(lptstrFilename, dwHandle, dwLen, lpData);
}

DWORD WINAPI VerFindFileA(DWORD uFlags, LPCSTR szFileName, LPCSTR szWinDir, LPCSTR szAppDir, LPSTR szCurDir, PUINT lpuCurDirLen, LPSTR szDestDir, PUINT lpuDestDirLen)
{
    return Forward(VerFindFileA, __FUNCTION__)(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
}

DWORD WINAPI VerFindFileW(DWORD uFlags, LPCWSTR szFileName, LPCWSTR szWinDir, LPCWSTR szAppDir, LPWSTR szCurDir, PUINT lpuCurDirLen, LPWSTR szDestDir, PUINT lpuDestDirLen)
{
    return Forward(VerFindFileW, __FUNCTION__)(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
}

DWORD WINAPI VerInstallFileA(DWORD uFlags, LPCSTR szSrcFileName, LPCSTR szDestFileName, LPCSTR szSrcDir, LPCSTR szDestDir, LPCSTR szCurDir, LPSTR szTmpFile, PUINT lpuTmpFileLen)
{
    return Forward(VerInstallFileA, __FUNCTION__)(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
}

DWORD WINAPI VerInstallFileW(DWORD uFlags, LPCWSTR szSrcFileName, LPCWSTR szDestFileName, LPCWSTR szSrcDir, LPCWSTR szDestDir, LPCWSTR szCurDir, LPWSTR szTmpFile, PUINT lpuTmpFileLen)
{
    return Forward(VerInstallFileW, __FUNCTION__)(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
}

DWORD WINAPI VerLanguageNameA(DWORD wLang, LPSTR szLang, DWORD cchLang)
{
    return Forward(VerLanguageNameA, __FUNCTION__)(wLang, szLang, cchLang);
}

DWORD WINAPI VerLanguageNameW(DWORD wLang, LPWSTR szLang, DWORD cchLang)
{
    return Forward(VerLanguageNameW, __FUNCTION__)(wLang, szLang, cchLang);
}

BOOL WINAPI VerQueryValueA(LPCVOID pBlock, LPCSTR lpSubBlock, LPVOID * lplpBuffer, PUINT puLen)
{
    return Forward(VerQueryValueA, __FUNCTION__)(pBlock, lpSubBlock, lplpBuffer, puLen);
}

BOOL WINAPI VerQueryValueW(LPCVOID pBlock, LPCWSTR lpSubBlock, LPVOID * lplpBuffer, PUINT puLen)
{
    return Forward(VerQueryValueW, __FUNCTION__)(pBlock, lpSubBlock, lplpBuffer, puLen);
}