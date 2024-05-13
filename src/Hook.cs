using System;
using System.Runtime.InteropServices;

namespace RitoClient
{
    // from PenguCore but C# version
    internal class Hook<T> : IDisposable where T : Delegate
    {
        IntPtr func_;
        IntPtr code_;
        object lock_;

#if X64
        const int SHELLCODE_SIZE = 12;
#else
        const int SHELLCODE_SIZE = 7;
#endif

        [StructLayout(LayoutKind.Sequential, Size = SHELLCODE_SIZE, Pack = 1)]
        unsafe struct Shellcode
        {
#if X64
            public byte movabs = 0x48;
#endif
            public byte mov_eax = 0xB8;
            public IntPtr addr;
            public byte push_eax = 0x50;
            public byte ret = 0xC3;

            public Shellcode() { }
        }

        public Hook()
        {
            func_ = IntPtr.Zero;
            lock_ = new object();
        }

        public void Install(string lib, string name, T hook)
        {
            if (func_ != IntPtr.Zero)
                return;

            var mod = Native.GetModuleHandle(lib);
            var proc = Native.GetProcAddress(mod, name);

            Install(proc, hook);
        }

        public void Install(IntPtr orig, T hook)
        {
            if (orig == IntPtr.Zero || func_ != IntPtr.Zero)
                return;

            func_ = orig;
            code_ = Marshal.AllocHGlobal(SHELLCODE_SIZE);
            Native.memcpy(code_, orig, SHELLCODE_SIZE);

            var code = new Shellcode();
            code.addr = Marshal.GetFunctionPointerForDelegate<T>(hook);

            unsafe
            {
                var pcode = &code;
                MemcpySafe(orig, (IntPtr)pcode, sizeof(Shellcode));
            }
        }

        public void Dispose()
        {
            if (func_ != IntPtr.Zero)
            {
                lock (lock_)
                {
                    MemcpySafe(func_, code_, SHELLCODE_SIZE);
                    Marshal.FreeHGlobal(code_);
                }
            }
        }

        public void Call(params object[] args)
        {
            lock (lock_)
            {
                using (var _ = new RestoreGuard(func_, code_))
                {
                    var fn = Marshal.GetDelegateForFunctionPointer<T>(func_);
                    fn.DynamicInvoke(args);
                }
            }
        }

        public R Call<R>(params object[] args)
        {
            lock (lock_)
            {
                using (var _ = new RestoreGuard(func_, code_))
                {
                    var fn = Marshal.GetDelegateForFunctionPointer<T>(func_);
                    return (R)fn.DynamicInvoke(args)!;
                }
            }
        }

        static void MemcpySafe(IntPtr dst, IntPtr src, int size)
        {
            int op;
            Native.VirtualProtect(dst, size, /*PAGE_EXECUTE_READWRITE*/0x40, out op);
            Native.memcpy(dst, src, size);
            Native.VirtualProtect(dst, size, op, out op);
        }

        struct RestoreGuard : IDisposable
        {
            IntPtr func_;
            IntPtr backup_;

            public RestoreGuard(IntPtr func, IntPtr code)
            {
                func_ = func;
                backup_ = Marshal.AllocHGlobal(SHELLCODE_SIZE);

                Native.memcpy(backup_, func, SHELLCODE_SIZE);
                MemcpySafe(func, code, SHELLCODE_SIZE);
            }

            public void Dispose()
            {
                MemcpySafe(func_, backup_, SHELLCODE_SIZE);
                Marshal.FreeHGlobal(backup_);
            }
        }
    }
}
