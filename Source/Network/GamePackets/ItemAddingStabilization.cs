using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ItemAddingStabilization : Writer, Interfaces.IPacket
    {
        byte[] Buffer;

        public ItemAddingStabilization(bool Create)
        {
            if (Create)
            {
                Buffer = null;
            }
        }

        public uint ItemUID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint PurificationItemCount
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public List<uint> PurificationItems
        {
            get
            {
                List<uint> items = new List<uint>();
                for (int count = 16; count <= 12 + PurificationItemCount * 4; count += 4)
                {
                    items.Add(BitConverter.ToUInt32(Buffer, count));
                }
                return items;
            }
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
