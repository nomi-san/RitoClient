using System;
using System.Runtime.InteropServices;

namespace RitoClient
{
    internal static class MessageBox
    {
        public static IntPtr Owner { get; set; }

        public static void Show(string message)
        {
            MsgBox(Owner, message, "RitoClient", 0);
        }

        [DllImport("user32.dll", EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode)]
        static extern int MsgBox(IntPtr hwnd, string msg, string title, int flags);
    }
}
