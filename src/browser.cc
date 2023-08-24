#include "commons.h"
#include <iostream>
#include <fstream>

HWND riotclient_window_ = nullptr;
cef_browser_t *main_browser_ = nullptr;

static decltype(cef_load_handler_t::on_load_start) OnLoadStart;
static void CEF_CALLBACK Hooked_OnLoadStart(
    struct _cef_load_handler_t* self,
    struct _cef_browser_t* browser,
    struct _cef_frame_t* frame,
    cef_transition_type_t transition_type)
{
    CefStr url = frame->get_url(frame);

    OnLoadStart(self, browser, frame, transition_type);
}

static decltype(cef_life_span_handler_t::on_after_created) OnAfterCreated;
static void CALLBACK Hooked_OnAfterCreated(
    struct _cef_life_span_handler_t* self,
    struct _cef_browser_t* browser)
{
    OnAfterCreated(self, browser);

    if (main_browser_ == nullptr)
    {
        browser->base.add_ref(&browser->base);
        main_browser_ = browser;

        auto host = browser->get_host(browser);
        HWND cef_window = host->get_window_handle(host);
        HWND widget_window = FindWindowExA(cef_window, NULL, "Chrome_WidgetWin_0", NULL);
        riotclient_window_ = GetParent(cef_window);
        host->base.release(&host->base);

        void SetUpDevTools(HWND);
        SetUpDevTools(widget_window);
    }
}

static void HookMainBrowser(cef_client_t *client)
{
    static auto GetLifeSpanhandler = client->get_life_span_handler;
    client->get_life_span_handler = [](struct _cef_client_t* self) -> cef_life_span_handler_t *
    {
        auto handler = GetLifeSpanhandler(self);

        OnAfterCreated = handler->on_after_created;
        handler->on_after_created = Hooked_OnAfterCreated;

        return handler;
    };

    static auto GetLoadHandler = client->get_load_handler;
    client->get_load_handler = [](struct _cef_client_t* self) -> cef_load_handler_t *
    {
        auto handler = GetLoadHandler(self);

        OnLoadStart = handler->on_load_start;
        handler->on_load_start = Hooked_OnLoadStart;

        return handler;
    };
}

static hook::Hook<decltype(&cef_browser_host_create_browser)> CefCreateBrowser;
static int Hooked_CefCreateBrowser(
    const cef_window_info_t* windowInfo,
    struct _cef_client_t* client,
    const cef_string_t* url,
    const struct _cef_browser_settings_t* settings,
    struct _cef_request_context_t* request_context)
{
    HookMainBrowser(client);
    return CefCreateBrowser(windowInfo, client, url, settings, request_context);
}

static hook::Hook<decltype(&cef_initialize)> CefInitialize;
static int Hooked_CefInitialize(
    const struct _cef_main_args_t* args,
    const struct _cef_settings_t* settings,
    cef_app_t* application,
    void* windows_sandbox_info)
{
#if _DEBUG
    FILE *fstdout;
    AllocConsole();
    SetConsoleTitleA("RitoClient (browser)");
    freopen_s(&fstdout, "CONOUT$", "w", stdout);
#endif

    //static auto OnBeforeCommandLineProcessing = application->on_before_command_line_processing;
    //application->on_before_command_line_processing = [](
    //    struct _cef_app_t* self,
    //    const cef_string_t* process_type,
    //    struct _cef_command_line_t* command_line)
    //{
    //    OnBeforeCommandLineProcessing(self, process_type, command_line);
    //};

    return CefInitialize(args, settings, application, windows_sandbox_info);
}

void HookBrowserProcess()
{
    CefInitialize.hook("libcef.dll", "cef_initialize", Hooked_CefInitialize);
    CefCreateBrowser.hook("libcef.dll", "cef_browser_host_create_browser", Hooked_CefCreateBrowser);
}