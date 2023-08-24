#include "internal.h"
#include <fstream>
#include <windows.h>

#include "../res/resource.h"
EXTERN_C IMAGE_DOS_HEADER __ImageBase;
#define THIS_MODULE ((HMODULE)&__ImageBase)

static void readResource(DWORD id, std::string &code)
{
    if (HRSRC res = FindResource(THIS_MODULE, MAKEINTRESOURCE(id), RT_RCDATA))
        if (HGLOBAL handle = LoadResource(THIS_MODULE, res))
        {
            auto source = (char*)LockResource(handle);
            auto size = SizeofResource(THIS_MODULE, res);
            code.assign(source, size);
        }
}

static void readFile(const std::wstring &path, std::string &code)
{
    std::ifstream file(path);
    if (file.good())
        code.assign(std::istreambuf_iterator<char>(file), std::istreambuf_iterator<char>());
    file.close();
}

void GetScriptAndStyle(std::string &script, std::string &style)
{
#if _DEBUG
    std::wstring path = __FILEW__;
    path = path.substr(0, path.find_last_of(L"\\/"));
    path = path.substr(0, path.find_last_of(L"\\/"));

    readFile(path + L"\\res\\script.js", script);
    readFile(path + L"\\res\\style.css", style);
#else
    readResource(IDR_SCRIPT, script);
    readResource(IDR_STYLE, style);
#endif

    if (!style.empty())
    {
        char buf[4096];
        size_t len = sprintf(buf, R"(window.addEventListener('load', async () => {
const style = document.createElement('style');
style.textContent = `%.*s`;
document.head.append(style);
}))", (int)style.length(), style.c_str());
        style.assign(buf, len);
    }
}