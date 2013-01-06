namespace Conquer_Online_Server.Network.Cryptography
{
    using System;

    public sealed class RC5
    {
        private readonly uint[] bufKey = new uint[4];
        private readonly uint[] bufSub = new uint[0x1a];

        public RC5(byte[] data)
        {
            if (data.Length != 0x10)
            {
                throw new RC5Exception("Invalid data length. Must be 16 bytes");
            }
            uint index = 0;
            uint num2 = 0;
            uint num3 = 0;
            uint num4 = 0;
            for (int i = 0; i < 4; i++)
            {
                this.bufKey[i] = (uint) (((data[i * 4] + (data[(i * 4) + 1] << 8)) + (data[(i * 4) + 2] << 0x10)) + (data[(i * 4) + 3] << 0x18));
            }
            this.bufSub[0] = 0xb7e15163;
            for (int j = 1; j < 0x1a; j++)
            {
                this.bufSub[j] = this.bufSub[j - 1] - 0x61c88647;
            }
            for (int k = 1; k <= 0x4e; k++)
            {
                this.bufSub[index] = LeftRotate((this.bufSub[index] + num3) + num4, 3);
                num3 = this.bufSub[index];
                index = (index + 1) % 0x1a;
                this.bufKey[num2] = LeftRotate((this.bufKey[num2] + num3) + num4, (int) (num3 + num4));
                num4 = this.bufKey[num2];
                num2 = (num2 + 1) % 4;
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            if ((data.Length % 8) != 0)
            {
                throw new RC5Exception("Invalid password length. Must be multiple of 8");
            }
            int num = (data.Length / 8) * 8;
            if (num <= 0)
            {
                throw new RC5Exception("Invalid password length. Must be greater than 0 bytes.");
            }
            uint[] numArray = new uint[data.Length / 4];
            for (int i = 0; i < (data.Length / 4); i++)
            {
                numArray[i] = (uint) (((data[i * 4] + (data[(i * 4) + 1] << 8)) + (data[(i * 4) + 2] << 0x10)) + (data[(i * 4) + 3] << 0x18));
            }
            for (int j = 0; j < (num / 8); j++)
            {
                uint num4 = numArray[2 * j];
                uint num5 = numArray[(2 * j) + 1];
                for (int m = 12; m >= 1; m--)
                {
                    num5 = RightRotate(num5 - this.bufSub[(2 * m) + 1], (int) num4) ^ num4;
                    num4 = RightRotate(num4 - this.bufSub[2 * m], (int) num5) ^ num5;
                }
                uint num7 = num5 - this.bufSub[1];
                uint num8 = num4 - this.bufSub[0];
                numArray[2 * j] = num8;
                numArray[(2 * j) + 1] = num7;
            }
            byte[] buffer = new byte[numArray.Length * 4];
            for (int k = 0; k < numArray.Length; k++)
            {
                buffer[k * 4] = (byte) numArray[k];
                buffer[(k * 4) + 1] = (byte) (numArray[k] >> 8);
                buffer[(k * 4) + 2] = (byte) (numArray[k] >> 0x10);
                buffer[(k * 4) + 3] = (byte) (numArray[k] >> 0x18);
            }
            return buffer;
        }

        public byte[] Encrypt(byte[] data)
        {
            if ((data.Length % 8) != 0)
            {
                throw new RC5Exception("Invalid password length. Must be multiple of 8");
            }
            int num = (data.Length / 8) * 8;
            if (num <= 0)
            {
                throw new RC5Exception("Invalid password length. Must be greater than 0 bytes.");
            }
            uint[] numArray = new uint[data.Length / 4];
            for (int i = 0; i < (data.Length / 4); i++)
            {
                numArray[i] = (uint) (((data[i * 4] + (data[(i * 4) + 1] << 8)) + (data[(i * 4) + 2] << 0x10)) + (data[(i * 4) + 3] << 0x18));
            }
            for (int j = 0; j < (num / 8); j++)
            {
                uint num4 = numArray[j * 2];
                uint num5 = numArray[(j * 2) + 1];
                uint num6 = num4 + this.bufSub[0];
                uint num7 = num5 + this.bufSub[1];
                for (int m = 1; m <= 12; m++)
                {
                    num6 = LeftRotate(num6 ^ num7, (int) num7) + this.bufSub[m * 2];
                    num7 = LeftRotate(num7 ^ num6, (int) num6) + this.bufSub[(m * 2) + 1];
                }
                numArray[j * 2] = num6;
                numArray[(j * 2) + 1] = num7;
            }
            byte[] buffer = new byte[numArray.Length * 4];
            for (int k = 0; k < numArray.Length; k++)
            {
                buffer[k * 4] = (byte) numArray[k];
                buffer[(k * 4) + 1] = (byte) (numArray[k] >> 8);
                buffer[(k * 4) + 2] = (byte) (numArray[k] >> 0x10);
                buffer[(k * 4) + 3] = (byte) (numArray[k] >> 0x18);
            }
            return buffer;
        }

        private static uint LeftRotate(uint value, int shiftAmount)
        {
            return ((value << shiftAmount) | (value >> (0x20 - (shiftAmount & 0x1f))));
        }

        private static uint RightRotate(uint value, int shiftAmount)
        {
            return ((value >> shiftAmount) | (value << (0x20 - (shiftAmount & 0x1f))));
        }
    }
}

