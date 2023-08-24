#include "commons.h"

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

bool LoadLibcefDll();
void HookBrowserProcess();
void HookRendererProcess();
void InjectThisDll(HANDLE hProcess);

static hook::Hook<decltype(&CreateProcessW)> Old_CreateProcessW;
static BOOL WINAPI Hooked_CreateProcessW(
    _In_opt_ LPCWSTR lpApplicationName,
    _Inout_opt_ LPWSTR lpCommandLine,
    _In_opt_ LPSECURITY_ATTRIBUTES lpProcessAttributes,
    _In_opt_ LPSECURITY_ATTRIBUTES lpThreadAttributes,
    _In_ BOOL bInheritHandles,
    _In_ DWORD dwCreationFlags,
    _In_opt_ LPVOID lpEnvironment,
    _In_opt_ LPCWSTR lpCurrentDirectory,
    _In_ LPSTARTUPINFOW lpStartupInfo,
    _Out_ LPPROCESS_INFORMATION lpProcessInformation)
{
    bool is_renderer = std::regex_search(lpCommandLine,
        std::wregex(L"RiotClientUxRender\\.exe.+--type=renderer", std::wregex::icase));

    if (is_renderer)
        dwCreationFlags |= CREATE_SUSPENDED;

    BOOL success = Old_CreateProcessW(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes,
        bInheritHandles, dwCreationFlags, lpEnvironment, lpCurrentDirectory, lpStartupInfo, lpProcessInformation);

    if (success && is_renderer)
    {
        InjectThisDll(lpProcessInformation->hProcess);
        ResumeThread(lpProcessInformation->hThread);
    }

    return success;
}

static void Initialize()
{
    WCHAR exe_path[2048];
    GetModuleFileNameW(NULL, exe_path, _countof(exe_path));

    if (std::regex_search(exe_path,
        std::wregex(L"RiotClientUx\\.exe$", std::wregex::icase)))
    {
        if (LoadLibcefDll())
        {
            HookBrowserProcess();
            Old_CreateProcessW.hook(&CreateProcessW, Hooked_CreateProcessW);
        }
    }
    else if (std::regex_search(exe_path,
        std::wregex(L"RiotClientUxRender\\.exe$", std::wregex::icase)))
    {
        if (wcsstr(GetCommandLineW(), L"--type=renderer") != nullptr)
        {
            if (LoadLibcefDll())
                HookRendererProcess();
        }
    }
}

BOOL APIENTRY DllMain(HMODULE module, DWORD reason, LPVOID reversed)
{
    static HANDLE mutex_ = NULL;

    switch (reason)
    {
        case DLL_PROCESS_ATTACH:
        {
            DisableThreadLibraryCalls(module);
            Initialize();
            break;
        }

        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }

    return TRUE;
}

void InjectThisDll(HANDLE hProcess)
{
    HMODULE kernel32 = GetModuleHandleA("kernel32");
    auto pVirtualAllocEx = (decltype(&VirtualAllocEx))GetProcAddress(kernel32, "VirtualAllocEx");
    auto pWriteProcessMemory = (decltype(&WriteProcessMemory))GetProcAddress(kernel32, "WriteProcessMemory");
    auto pCreateRemoteThread = (decltype(&CreateRemoteThread))GetProcAddress(kernel32, "CreateRemoteThread");

    WCHAR thisDllPath[2048]{};
    GetModuleFileNameW((HMODULE)&__ImageBase, thisDllPath, _countof(thisDllPath));

    size_t pathSize = (wcslen(thisDllPath) + 1) * sizeof(WCHAR);
    LPVOID pathAddr = pVirtualAllocEx(hProcess, NULL, pathSize, MEM_COMMIT, PAGE_READWRITE);
    pWriteProcessMemory(hProcess, pathAddr, thisDllPath, pathSize, NULL);

    HANDLE loader = pCreateRemoteThread(hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)&LoadLibraryW, pathAddr, 0, NULL);
    WaitForSingleObject(loader, INFINITE);
    CloseHandle(loader);
}

int APIENTRY _BootstrapEntry(HWND, HINSTANCE, LPWSTR commandLine, int)
{
    NTSTATUS(NTAPI *NtQueryInformationProcess)(HANDLE, DWORD, PVOID, ULONG, PULONG);
    NTSTATUS(NTAPI *NtRemoveProcessDebug)(HANDLE, HANDLE);
    NTSTATUS(NTAPI *NtClose)(HANDLE Handle);

    STARTUPINFOW si;
    PROCESS_INFORMATION pi;
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);

    if (!CreateProcessW(NULL, commandLine, NULL, NULL, FALSE,
        CREATE_SUSPENDED | DEBUG_ONLY_THIS_PROCESS, NULL, NULL, &si, &pi))
    {
        char msg[512];
        sprintf_s(msg, "Failed to create RiotClientUx process, last error: 0x%08X.", GetLastError());
        MessageBoxA(0, msg, "RitoClient bootstraper", MB_ICONWARNING | MB_OK);
        return 1;
    }

    HMODULE ntdll = GetModuleHandleA("ntdll");
    (LPVOID &)NtQueryInformationProcess = GetProcAddress(ntdll, "NtQueryInformationProcess");
    (LPVOID &)NtRemoveProcessDebug = GetProcAddress(ntdll, "NtRemoveProcessDebug");
    (LPVOID &)NtClose = GetProcAddress(ntdll, "NtClose");

    HANDLE hDebug;
    if (NtQueryInformationProcess(pi.hProcess, 30, &hDebug, sizeof(HANDLE), 0) >= 0)
    {
        NtRemoveProcessDebug(pi.hProcess, hDebug);
        NtClose(hDebug);
    }

    InjectThisDll(pi.hProcess);
    ResumeThread(pi.hThread);
    WaitForSingleObject(pi.hProcess, INFINITE);

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    return 0;
}