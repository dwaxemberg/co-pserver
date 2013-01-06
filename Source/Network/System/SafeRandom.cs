using System;

namespace Conquer_Online_Server
{
    public class SafeRandom
    {
        private Random random;

        public byte Next(byte minValue, byte maxValue)
        {
            random = new Random();
            return (byte)random.Next((int)minValue, (int)maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            random = new Random();
            return random.Next(minValue, maxValue);
        }

        public short Next(short minValue, short maxValue)
        {
            random = new Random();
            return (short)random.Next((int)minValue, (int)maxValue);
        }

        public long Next(long minValue, long maxValue)
        {
            random = new Random();
            return (long)random.Next((int)minValue, (int)maxValue);
        }

        public uint Next(uint minValue, uint maxValue)
        {
            random = new Random();
            return (uint)random.Next((int)minValue, (int)maxValue);
        }

        public ushort Next(ushort minValue, ushort maxValue)
        {
            random = new Random();
            return (ushort)random.Next((int)minValue, (int)maxValue);
        }

        public ulong Next(ulong minValue, ulong maxValue)
        {
            random = new Random();
            return (ulong)random.Next((int)minValue, (int)maxValue);
        }
    }
}
