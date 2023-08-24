#include "commons.h"
#include "include/cef_version.h"

decltype(&cef_register_extension) CefRegisterExtension;
decltype(&cef_v8value_create_string) CefV8Value_CreateString;

decltype(&cef_string_set) CefString_Set;
decltype(&cef_string_clear) CefString_Clear;
decltype(&cef_string_from_utf8) CefString_FromUtf8;
decltype(&cef_string_from_wide) CefString_FromWide;
decltype(&cef_string_userfree_free) CefString_UserFree_Free;
decltype(&cef_string_to_utf8) CefString_ToUtf8;
decltype(&cef_string_utf8_clear) CefString_ClearUtf8;

static int GetFileMajorVersion(const char *file)
{
    int version = 0;

    DWORD  verHandle = 0;
    UINT   size = 0;
    LPBYTE lpBuffer = NULL;

    if (DWORD verSize = GetFileVersionInfoSize(L"libcef.dll", &verHandle))
    {
        LPSTR verData = new char[verSize];

        if (GetFileVersionInfo(L"libcef.dll", verHandle, verSize, verData)
            && VerQueryValue(verData, L"\\", (VOID FAR* FAR*)&lpBuffer, &size)
            && size > 0)
        {
            VS_FIXEDFILEINFO *verInfo = (VS_FIXEDFILEINFO *)lpBuffer;
            if (verInfo->dwSignature == 0xfeef04bd)
                version = ((verInfo->dwFileVersionMS >> 16) & 0xffff);
        }

        delete[] verData;
    }

    return version;
}

static void WarnInvalidVersion()
{
    MessageBoxA(NULL,
        "This Riot Client version is not supported.\n"
        "Please check existing issues or open new issue about that, and wait for the new update.",
        "Riot Client Insider", MB_TOPMOST | MB_OK | MB_ICONWARNING);
    ShellExecuteA(NULL, "open", "https://git.leagueloader.app", NULL, NULL, NULL);
}

bool LoadLibcefDll()
{
    const char *filename = "libcef.dll";

    if (GetFileMajorVersion(filename) != CEF_VERSION_MAJOR)
    {
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)&WarnInvalidVersion, NULL, 0, NULL);
        return false;
    }

    HMODULE libcef = GetModuleHandleA("libcef.dll");

    (LPVOID &)CefString_Set = GetProcAddress(libcef, "cef_string_utf16_set");
    (LPVOID &)CefString_Clear = GetProcAddress(libcef, "cef_string_utf16_clear");
    (LPVOID &)CefString_FromUtf8 = GetProcAddress(libcef, "cef_string_utf8_to_utf16");
    (LPVOID &)CefString_FromWide = GetProcAddress(libcef, "cef_string_wide_to_utf16");
    (LPVOID &)CefString_UserFree_Free = GetProcAddress(libcef, "cef_string_userfree_utf16_free");
    (LPVOID &)CefString_ToUtf8 = GetProcAddress(libcef, "cef_string_utf16_to_utf8");
    (LPVOID &)CefString_ClearUtf8 = GetProcAddress(libcef, "cef_string_utf8_clear");

    (LPVOID &)CefRegisterExtension = GetProcAddress(libcef, "cef_register_extension");
    (LPVOID &)CefV8Value_CreateString = GetProcAddress(libcef, "cef_v8value_create_string");

    return true;
}