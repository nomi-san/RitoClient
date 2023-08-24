#pragma once
#define _CRT_SECURE_NO_WARNINGS

#ifdef _WIN64
#error "Build 32-bit only."
#endif

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <atomic>
#include <string>
#include <vector>
#include <mutex>
#include <regex>
#include <windows.h>

#include "include/internal/cef_string.h"
#include "include/capi/cef_base_capi.h"
#include "include/capi/cef_app_capi.h"
#include "include/capi/cef_client_capi.h"

extern decltype(&cef_register_extension) CefRegisterExtension;
extern decltype(&cef_v8value_create_string) CefV8Value_CreateString;

extern decltype(&cef_string_set) CefString_Set;
extern decltype(&cef_string_clear) CefString_Clear;
extern decltype(&cef_string_from_utf8) CefString_FromUtf8;
extern decltype(&cef_string_from_wide) CefString_FromWide;
extern decltype(&cef_string_userfree_free) CefString_UserFree_Free;
extern decltype(&cef_string_to_utf8) CefString_ToUtf8;
extern decltype(&cef_string_utf8_clear) CefString_ClearUtf8;

template <typename T>
class CefRefCount : public T
{
public:
    template <typename U>
    CefRefCount(const U *) : T{}, ref_(1) {
        base.size = sizeof(U);
        base.add_ref = _Base_AddRef;
        base.release = _Base_Release;
        base.has_one_ref = _Base_HasOneRef;
        base.has_at_least_one_ref = _Base_HasAtLeastOneRef;
        self_delete_ = [](void *self) { delete static_cast<U *>(self); };
    }

    CefRefCount(bool) : CefRefCount(static_cast<T *>(nullptr)) {}

private:
    void(*self_delete_)(void *);
    std::atomic<size_t> ref_;

    static void CALLBACK _Base_AddRef(cef_base_ref_counted_t *_) {
        ++reinterpret_cast<CefRefCount *>(_)->ref_;
    }

    static int CALLBACK _Base_Release(cef_base_ref_counted_t *_) {
        CefRefCount *self = reinterpret_cast<CefRefCount *>(_);
        if (--self->ref_ == 0) {
            self->self_delete_(_);
            return 1;
        }
        return 0;
    }

    static int CALLBACK _Base_HasOneRef(cef_base_ref_counted_t *_) {
        return reinterpret_cast<CefRefCount *>(_)->ref_ == 1;
    }

    static int CALLBACK _Base_HasAtLeastOneRef(cef_base_ref_counted_t *_) {
        return reinterpret_cast<CefRefCount *>(_)->ref_ > 0;
    }
};

struct CefStr : cef_string_t
{
    CefStr();
    CefStr(const std::string &s);
    CefStr(const std::wstring &s);
    CefStr(const cef_string_t *s);
    CefStr(cef_string_userfree_t uf);
    ~CefStr();

    bool equal(const wchar_t *s) const;
    bool equal(const std::wstring &s) const;
    bool equali(const wchar_t *s) const;
    bool equali(const std::wstring &s) const;
    bool operator ==(const wchar_t *s) const { return equal(s); }
    bool operator ==(const std::wstring &s) const { return equal(s); }

    std::string utf8() const;
    bool empty() const { return length == 0; }
    explicit operator bool() const { return !empty(); }

private:
    cef_string_userfree_t uf_;
};

namespace utils
{
    std::wstring coreDir();
    std::wstring clientDir();
    std::wstring dataStorePath();

    bool isDir(const std::wstring &path);
    bool isFile(const std::wstring &path);
    bool readFile(const std::wstring &path, std::string &data);
    std::vector<std::wstring> readDir(const std::wstring &path);
}

namespace hook
{
#   pragma pack(push, 1)
    struct Shellcode
    {
        Shellcode(intptr_t addr) : addr(addr) {}

    private:
        // Special thanks to https://github.com/nbqofficial/divert/
#   ifdef X86_64
        uint8_t movabs = 0x48;      // x86                  x86_64                 
#   endif                           //
        uint8_t mov_eax = 0xB8;     // mov eax [addr]   |   movabs rax [addr]
        intptr_t addr;              //
        uint8_t push_eax = 0x50;    // push eax         |   push rax
        uint8_t ret = 0xC3;         // ret              |   ret
    };
#   pragma pack(pop)

    struct Restorable
    {
        Restorable(void *func, const void *code, size_t size)
            : func_(func)
            , backup_(new uint8_t[size]{})
            , size_(size)
        {
            memcpy(backup_, func, size);
            memcpy_safe(func, code, size);
        }

        ~Restorable()
        {
            memcpy_safe(func_, backup_, size_);
            delete[] backup_;
        }

        Restorable swap()
        {
            return Restorable(func_, backup_, size_);
        }

    private:
        void *func_;
        uint8_t *backup_;
        size_t size_;

        static void memcpy_safe(void *dst, const void *src, size_t size)
        {
            DWORD op;
            VirtualProtect(dst, size, PAGE_EXECUTE_READWRITE, &op);
            memcpy(dst, src, size);
            VirtualProtect(dst, size, op, &op);
        }
    };

    template<typename Fn, typename R, typename ...Args>
    class HookBase
    {
    public:
        HookBase()
            : orig_(nullptr)
            , rest_(nullptr)
            , mutex_{}
        {
        }

        ~HookBase()
        {
            if (rest_ != nullptr)
            {
                std::lock_guard<std::mutex> lock(mutex_);
                {
                    delete rest_;
                }
            }
        }

        bool hook(Fn orig, Fn hook)
        {
            if (orig == nullptr || hook == nullptr)
                return false;

            orig_ = orig;

            Shellcode code(reinterpret_cast<intptr_t>(hook));
            rest_ = new Restorable(orig, &code, sizeof(code));

            return true;
        }

        bool hook(const char *lib, const char *proc, Fn hook)
        {
            if (HMODULE mod = GetModuleHandleA(lib))
                if (Fn orig = reinterpret_cast<Fn>(GetProcAddress(mod, proc)))
                    return this->hook(orig, hook);

            return false;
        }

        R operator ()(Args ...args)
        {
            std::lock_guard<std::mutex> lock(mutex_);
            {
                auto _t = rest_->swap();
                {
                    return orig_(args...);
                }
            }
        }

    protected:
        Fn orig_;
        Restorable *rest_;
        std::mutex mutex_;
    };

    template<typename>
    class Hook;

    template<typename R, typename ...Args>
    class Hook<R(*)(Args...)>
        : public HookBase<R(*)(Args...), R, Args...> {};

#if defined(OS_WIN) && !defined(X86_64)
    // stdcall and fastcall are ignored on x64

    template<typename R, typename ...Args>
    class Hook<R(__stdcall*)(Args...)>
        : public HookBase<R(__stdcall*)(Args...), R, Args...> {};

    template<typename R, typename ...Args>
    class Hook<R(__fastcall*)(Args...)>
        : public HookBase<R(__fastcall*)(Args...), R, Args...> {};
#endif
}