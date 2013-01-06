using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class MentorPrize : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;

        public const byte
            ClaimExperience = 1,
            ClaimAmountExpBalls = 2,
            ClaimHeavenBlessing = 3,
            ClaimPlus = 4,
            Show = 5;

        public MentorPrize(bool create)
        {
            if (!create)
            {
                Buffer = new byte[8 + 40];

                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(2067, 2, Buffer);
            }
        }

        public uint Prize_Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint Mentor_ID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public ulong Prize_Experience
        {
            get { return BitConverter.ToUInt64(Buffer, 24); }
            set { WriteUInt64(value, 24, Buffer); }
        }

        public ushort Prize_HeavensBlessing
        {
            get { return BitConverter.ToUInt16(Buffer, 32); }
            set { WriteUInt16(value, 32, Buffer); }
        }

        public ushort Prize_PlusStone
        {
            get { return BitConverter.ToUInt16(Buffer, 34); }
            set { WriteUInt16(value, 34, Buffer); }
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
