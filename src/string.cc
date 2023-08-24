#include "commons.h"

CefStr::CefStr() : cef_string_t{}, uf_(nullptr)
{
    length = 0;
    str = L"";
}

CefStr::CefStr(const std::string &s) : cef_string_t{}, uf_(cef_string_userfree_t(1))
{
    CefString_FromUtf8(s.c_str(), s.length(), this);
}

CefStr::CefStr(const std::wstring &s) : cef_string_t(), uf_(cef_string_userfree_t(1))
{
    CefString_FromWide(s.c_str(), s.length(), this);
}

CefStr::CefStr(const cef_string_t *s) : CefStr()
{
    if (s != nullptr)
    {
        str = s->str;
        length = s->length;
    }
}

CefStr::CefStr(cef_string_userfree_t uf) : CefStr()
{
    if (uf != nullptr)
    {
        str = uf->str;
        length = uf->length;
        uf_ = uf;
    }
}

CefStr::~CefStr()
{
    if (uf_ == cef_string_userfree_t(1))
    {
        CefString_Clear(this);
    }
    else if (uf_ != nullptr)
    {
        CefString_UserFree_Free(uf_);
    }
}

bool CefStr::equal(const wchar_t *s) const
{
    return wcscmp(str, s) == 0;
}

bool CefStr::equal(const std::wstring &s) const
{
    return wcsncmp(str, s.c_str(), length) == 0;
}

bool CefStr::equali(const wchar_t *s) const
{
    return _wcsicmp(str, s) == 0;
}

bool CefStr::equali(const std::wstring &s) const
{
    return _wcsnicmp(str, s.c_str(), length) == 0;
}

std::string CefStr::utf8() const
{
    cef_string_utf8_t out{};
    CefString_ToUtf8(str, length, &out);
    return std::string(out.str, out.length);
}

cef_string_t operator""_s(const wchar_t *s, size_t l)
{
    cef_string_t t{};
    CefString_Set(s, l, &t, false);
    return t;
}