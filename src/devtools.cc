#include "commons.h"

extern HWND riotclient_window_;
extern cef_browser_t *main_browser_;

static void OpenDevTools()
{
    if (!main_browser_)
        return;

    auto host = main_browser_->get_host(main_browser_);

    if (host->has_dev_tools(host))
    {
        host->show_dev_tools(host, nullptr, nullptr, nullptr, nullptr);
    }
    else
    {
        cef_window_info_t wi{};
        wi.x = CW_USEDEFAULT;
        wi.y = CW_USEDEFAULT;
        wi.width = CW_USEDEFAULT;
        wi.height = CW_USEDEFAULT;
        wi.ex_style = WS_EX_APPWINDOW;
        wi.style = WS_OVERLAPPEDWINDOW | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_VISIBLE;

        wi.window_name.str = L"RitoClient DevTools";
        wi.window_name.length = wcslen(wi.window_name.str);

        cef_browser_settings_t settings{};
        host->show_dev_tools(host, &wi, nullptr, &settings, nullptr);
    }

    host->base.release(&host->base);
}

static WNDPROC Old_WndProc;
static LRESULT CALLBACK Hooked_WndProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp)
{
    switch (msg)
    {
        case WM_KEYDOWN:
        {
            if ((HIWORD(lp) & KF_REPEAT) == 0 && GetKeyState(VK_CONTROL) < 0 && GetKeyState(VK_SHIFT) < 0)
            {
                switch (wp)
                {
                    case 'I':
                        OpenDevTools();
                        break;
                    case 'R':
                        if (main_browser_ != nullptr)
                            main_browser_->reload_ignore_cache(main_browser_);
                        break;
                }
            }
            break;
        }
    }

    return Old_WndProc(hwnd, msg, wp, lp);
}

void SetUpDevTools(HWND parent)
{
    Old_WndProc = (WNDPROC)GetWindowLongPtr(parent, GWLP_WNDPROC);
    SetWindowLongPtr(parent, GWLP_WNDPROC, (LONG_PTR)Hooked_WndProc);
}