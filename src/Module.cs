using System;
using System.IO;
using System.Text;

#pragma warning disable CS8500

namespace RitoClient
{
    internal static class Module
    {
        static IntPtr ThisModule
        {
            get
            {
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

                var sb = new StringBuilder(2048);
                Native.GetModuleFileName(module, sb, sb.Capacity);

                var fi = new FileInfo(sb.ToString());
                return fi.LinkTarget ?? fi.FullName;
            }
        }
    }
}
