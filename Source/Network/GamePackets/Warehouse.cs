using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Warehouse : Writer, Interfaces.IPacket
    {
        public const byte Entire = 0, AddItem = 1, RemoveItem = 2;
        private byte[] buffer;
        public Warehouse(bool Create)
        {
            if (Create)
            {
                buffer = new byte[32];
                WriteUInt16(24, 0, buffer);
                WriteUInt16(1102, 2, buffer);
            }
        }

        public uint NpcID
        {
            get { return BitConverter.ToUInt32(buffer, 4); }
            set { WriteUInt32(value, 4, buffer); }
        }

        public byte Type
        {
            get
            {
                return buffer[8];
            }
            set
            {
                buffer[8] = value;
            }
        }

        public uint Count
        {
            get { return BitConverter.ToUInt32(buffer, 20); }
            set
            {
                if (value > 20)
                    throw new Exception("Invalid Count value.");
                byte[] Buffer = new byte[8 + 24 + (72 * value)];
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(1102, 2, Buffer);
                WriteUInt32(NpcID, 4, Buffer);
                WriteUInt32(Type, 8, Buffer);
                Buffer[9] = buffer[9];
                WriteUInt32(value, 20, Buffer);
                buffer = Buffer;
            }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(buffer, 16); }
            set { WriteUInt32(value, 16, buffer); }
        }

        public void Append(Interfaces.IConquerItem item)
        {
            WriteUInt32(item.UID, 24, buffer);
            WriteUInt32(item.ID, 28, buffer);
            WriteByte((byte)item.SocketOne, 33, buffer);
            WriteByte((byte)item.SocketTwo, 34, buffer);
            WriteByte(item.Plus, 41, buffer);
            WriteByte(item.Bless, 42, buffer);
            WriteByte((byte)(item.Bound == true ? 1 : 0), 43, buffer);
            WriteUInt16(item.Enchant, 44, buffer);
            WriteUInt16((ushort)item.Effect, 46, buffer);
            WriteByte(item.Lock, 48, buffer);
            WriteByte((byte)(item.Suspicious == true ? 1 : 0), 49, buffer);
            WriteByte((byte)item.Color, 51, buffer);
            WriteUInt32(item.SocketProgress, 52, buffer);
            WriteUInt32(item.PlusProgress, 56, buffer);
        }

        public byte[] ToArray()
        {
            return buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public void Send(Client.GameState client)
        {
            client.Send(buffer);
        }
    }
}
