using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Conquer_Online_Server.AimBotSystem
{
    public unsafe class Native
    {
       
        private const string MSVCRT_DLL = @"C:\Windows\system32\msvcrt.dll";
        private const string KERNEL32_DLL = @"kernel32.dll";
        private const string WINMM_DLL = @"winmm.dll";

        public delegate bool ConsoleEventHandler(CtrlType sig);

        [DllImport(MSVCRT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void* memcpy(void* dst, void* src, int length);

        [DllImport(MSVCRT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void* memset(void* dst, byte fill, int length);

        [DllImport(KERNEL32_DLL)]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventHandler handler, bool add);

        [DllImport(WINMM_DLL)]
        public static extern SystemTime timeGetTime();
    }

    public unsafe static class NativeExtensions
    {
        public static void CopyTo(this string str, void* pDest)
        {
            var dest = (byte*)pDest;
            for (var i = 0; i < str.Length; i++)
            {
                dest[i] = (byte)str[i];
            }
        }

        public static byte[] UnsafeClone(this byte[] buffer)
        {
            var bufCopy = new byte[buffer.Length];
            fixed (byte* pBuf = buffer, pCopy = bufCopy)
            {
                Native.memcpy(pCopy, pBuf, buffer.Length);
            }
            return bufCopy;
        }
    }
}
