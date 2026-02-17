#pragma warning disable CS8500

namespace RitoClient
{
    internal static class Module
    {
        static IntPtr ThisModule
        {
            get
            {
#if DEBUG
                return Native.GetModuleHandle(nameof(RitoClient) + ".dll");
#endif
                unsafe
                {
                    // magic here
                    var func = static () => { };
                    var paddr = *(IntPtr**)&func;

                    var module = IntPtr.Zero;
                    Native.GetModuleHandleEx(0x4, *paddr, out module);

                    return module;
                }
            }
        }

        public static string ThisModulePath
        {
            get
            {
                var module = ThisModule;
                if (module == IntPtr.Zero)
                    return string.Empty;

                Span<char> buf = stackalloc char[2048];
                int length = Native.GetModuleFileName(module, buf, buf.Length);

                var fi = new FileInfo(new string(buf.Slice(0, length)));
                return fi.LinkTarget ?? fi.FullName;
            }
        }

        public static string ThisModuleDir => Path.GetDirectoryName(ThisModulePath) ?? string.Empty;
    }
}