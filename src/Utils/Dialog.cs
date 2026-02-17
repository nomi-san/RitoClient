using System.Runtime.InteropServices;

namespace RitoClient;

static class Dialog
{
    const string Caption = "Riot Client";
    static nint Owner => Window.Handle;

    public static void Show(string message, bool warning = false)
    {
        int flags = 0x0;
        if (warning) flags |= 0x30;

        MsgBox(Owner, message, Caption, flags);
    }

    [DllImport("user32", EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode)]
    static extern int MsgBox(nint hwnd, string msg, string title, int flags);
}