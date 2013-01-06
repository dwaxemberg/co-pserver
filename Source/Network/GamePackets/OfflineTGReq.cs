using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class OfflineTGRequest : Writer, Interfaces.IPacket
    {
        public const byte
            OnTrainingTimeRequested = 0,
            OnConfirmation = 1,
            ReplyToConfirmation = 3,
            ClaimExperience = 4;

        private byte[] Buffer;
        public OfflineTGRequest(bool create)
        {
            if (create)
            {
                Buffer = new byte[8 + 12];
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(2044, 2, Buffer);
            }
        }

        public byte ID
        {
            get { return Buffer[4]; }
            set { Buffer[4] = value; }
        }

        public ushort Minutes
        {
            get { return BitConverter.ToUInt16(Buffer, 8); }
            set { WriteUInt16(value, 8, Buffer); }
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