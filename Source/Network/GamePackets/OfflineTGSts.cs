using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class OfflineTGStats : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;
        public OfflineTGStats(bool create)
        {
            if (create)
            {
                Buffer = new byte[8 + 20];
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(2043, 2, Buffer);
            }
        }

        public ushort TrainedMinutes
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort TotalTrainingMinutesLeft
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public byte Character_AcquiredLevel
        {
            get { return Buffer[8]; }
            set { Buffer[8] = value; }
        }

        public ulong Character_NewExp
        {
            get { return BitConverter.ToUInt64(Buffer, 12); }
            set { WriteUInt64(value, 12, Buffer); }
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] source)
        {
            Buffer = source;
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}