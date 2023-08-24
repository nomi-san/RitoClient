#include "commons.h"
#include <fstream>

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

std::wstring utils::coreDir()
{
    static std::wstring path_{};
    if (!path_.empty()) return path_;

    WCHAR this_dll[2048];
    GetModuleFileNameW((HINSTANCE)&__ImageBase, this_dll, 2048);

    DWORD attr = GetFileAttributesW(this_dll);
    if ((attr & FILE_ATTRIBUTE_REPARSE_POINT) != FILE_ATTRIBUTE_REPARSE_POINT)
    {
        path_ = this_dll;
        return path_ = path_.substr(0, path_.find_last_of(L"/\\"));
    }

    WCHAR target_path[2048];
    HANDLE symlink = CreateFileW(this_dll, GENERIC_READ, 0x1, NULL, OPEN_EXISTING, 0, NULL);
    DWORD pathLength = GetFinalPathNameByHandleW(symlink, target_path, 2048, FILE_NAME_OPENED);
    CloseHandle(symlink);

    std::wstring dir{ target_path, pathLength };
    if (dir.rfind(L"\\\\?\\", 0) == 0)
        dir.erase(0, 4);

    return path_ = dir.substr(0, dir.find_last_of(L"/\\"));
}

std::wstring utils::clientDir()
{
    WCHAR this_exe[2048];
    GetModuleFileNameW(NULL, this_exe, 2048);

    std::wstring path = this_exe;
    return path.substr(0, path.find_last_of(L"/\\"));
}

std::wstring utils::dataStorePath()
{
    return coreDir() + L"\\datastore";
}

bool utils::isDir(const std::wstring &path)
{
    DWORD attr = GetFileAttributesW(path.c_str());

    if (attr == INVALID_FILE_ATTRIBUTES)
        return false;

    return attr & FILE_ATTRIBUTE_DIRECTORY;
}

bool utils::isFile(const std::wstring &path)
{
    DWORD attr = GetFileAttributesW(path.c_str());

    if (attr == INVALID_FILE_ATTRIBUTES)
        return false;

    return !(attr & FILE_ATTRIBUTE_DIRECTORY);
}

bool utils::readFile(const std::wstring &path, std::string &data)
{
    bool result;
    std::ifstream input(path, std::ios::binary);

    if (result = input.good())
    {
        data.assign((std::istreambuf_iterator<char>(input)),
            (std::istreambuf_iterator<char>()));
    }

    input.close();
    return result;
}

std::vector<std::wstring> utils::readDir(const std::wstring &path)
{
    std::vector<std::wstring> files{};

    WIN32_FIND_DATAW fd;
    HANDLE hFind = FindFirstFileW(path.c_str(), &fd);

    if (hFind != INVALID_HANDLE_VALUE)
    {
        do
        {
            files.push_back(fd.cFileName);
        } while (FindNextFileW(hFind, &fd));

        FindClose(hFind);
    }

    return files;
}