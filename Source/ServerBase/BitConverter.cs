using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server
{
    public unsafe class BitConverter1
    {
        public static ulong ToUInt64(byte[] buffer, int offset)
        {
            fixed (byte* Buffer = buffer)
            {
                return *((ulong*)(Buffer + offset));
            }
        }
        public static uint ToUInt32(byte[] buffer, int offset)
        {
            fixed (byte* Buffer = buffer)
            {
                return *((uint*)(Buffer + offset));
            }
        }
        public static int ToInt32(byte[] buffer, int offset)
        {
            fixed (byte* Buffer = buffer)
            {
                return *((int*)(Buffer + offset));
            }
        }
        public static ushort ToUInt16(byte[] buffer, int offset)
        {
            fixed (byte* Buffer = buffer)
            {
                return *((ushort*)(Buffer + offset));
            }
        }
        public static short ToInt16(byte[] buffer, int offset)
        {
            fixed (byte* Buffer = buffer)
            {
                return *((short*)(Buffer + offset));
            }
        }

        internal static string ToString(byte[] Buffer, int p, byte p_2)
        {
            throw new NotImplementedException();
        }
    }
}
