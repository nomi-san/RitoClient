#include "commons.h"

bool HandleDataStore(const std::wstring &fn,
    const std::vector<cef_v8value_t *> &args, cef_v8value_t * &retval);

struct ExtensionHandler : CefRefCount<cef_v8handler_t>
{
    ExtensionHandler() : CefRefCount(this)
    {
        cef_v8handler_t::execute = execute;
    }

    static int CALLBACK execute(cef_v8handler_t* self,
        const cef_string_t* name,
        cef_v8value_t* object,
        size_t argc,
        cef_v8value_t* const* argv,
        cef_v8value_t** retval,
        cef_string_t* exception)
    {
        std::wstring fn(name->str, name->length);
        std::vector<cef_v8value_t *> args(argv, argv + argc);

        if (HandleDataStore(fn, args, *retval))
            return true;

        return false;
    }
};

static decltype(cef_render_process_handler_t::on_web_kit_initialized) OnWebKitInitialized;
static void CEF_CALLBACK Hooked_OnWebKitInitialized(cef_render_process_handler_t* self)
{
    OnWebKitInitialized(self);

    const char *ext_code = 
        #include "extension.h"
        ;

    CefRegisterExtension(&CefStr("v8/RitoClient"),
        &CefStr(ext_code), new ExtensionHandler());
}

static decltype(cef_render_process_handler_t::on_context_created) OnContextCreated;
static void CEF_CALLBACK Hooked_OnContextCreated(
    struct _cef_render_process_handler_t* self,
    struct _cef_browser_t* browser,
    struct _cef_frame_t* frame,
    struct _cef_v8context_t* context)
{
    OnContextCreated(self, browser, frame, context);

    CefStr url = frame->get_url(frame);

    if (url && wcsstr(url.str, L"riot:") && wcsstr(url.str, L"/index.html"))
    {
#if _DEBUG
        FILE *fstdout;
        AllocConsole();
        SetConsoleTitleA("RitoClient (renderer)");
        freopen_s(&fstdout, "CONOUT$", "w", stdout);
#endif

        auto preload_dir = utils::coreDir() + L"\\preload";
        if (utils::isDir(preload_dir))
        {
            for (const auto &name : utils::readDir(preload_dir + L"\\*.js"))
            {
                if (std::regex_search(name, std::wregex(L"\\.js$", std::regex::icase))
                    && utils::isFile(preload_dir + L"\\" + name))
                {
                    std::string script;
                    if (utils::readFile(preload_dir + L"\\" + name, script))
                    {
                        frame->execute_java_script(frame,
                            &CefStr(script), &CefStr(L"https://preload/" + name), 1);
                    }
                }
            }
        }
    }
}

static hook::Hook<decltype(&cef_execute_process)> CefExecuteProcess;
static int Hooked_CefExecuteProcess(const cef_main_args_t* args, cef_app_t* app, void* windows_sandbox_info)
{
    static auto GetRenderProcessHandler = app->get_render_process_handler;
    app->get_render_process_handler = [](cef_app_t* self)
    {
        auto handler = GetRenderProcessHandler(self);

        OnWebKitInitialized = handler->on_web_kit_initialized;
        handler->on_web_kit_initialized = Hooked_OnWebKitInitialized;

        OnContextCreated = handler->on_context_created;
        handler->on_context_created = Hooked_OnContextCreated;

        return handler;
    };

    return CefExecuteProcess(args, app, windows_sandbox_info);
}

void HookRendererProcess()
{
    CefExecuteProcess.hook("libcef.dll", "cef_execute_process", Hooked_CefExecuteProcess);
}