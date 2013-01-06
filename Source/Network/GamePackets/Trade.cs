using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Trade : Writer, Interfaces.IPacket
    {
        public const uint Request = 1,
            Close = 2,
            ShowTable = 3,
            HideTable = 5,
            AddItem = 6,
            SetMoney = 7,
            ShowMoney = 8,
            Accept = 10,
            RemoveItem = 11,
            ShowConquerPoints = 12,
            SetConquerPoints = 13;

        byte[] Buffer;

        public Trade(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[20];
                WriteUInt16(12, 0, Buffer);
                WriteUInt16(1056, 2, Buffer);
            }
        }
        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint Type
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
