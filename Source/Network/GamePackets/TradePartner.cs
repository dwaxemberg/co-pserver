using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class TradePartner : Writer, Interfaces.IPacket
    {
        public const byte
            RequestPartnership = 0,
            RejectRequest = 1,
            BreakPartnership = 4,
            AddPartner = 5;

        byte[] Buffer;

        public TradePartner(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[40];
                WriteUInt16(32, 0, Buffer);
                WriteUInt16(2046, 2, Buffer);
            }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public byte Type
        {
            get { return Buffer[8]; }
            set { Buffer[8] = value; }
        }

        public bool Online
        {
            get { return Buffer[9] == 1; }
            set { Buffer[9] = value == true ? (byte)1 : (byte)0; }
        }

        public int HoursLeft
        {
            get { return BitConverter.ToInt32(Buffer, 10); }
            set
            {
                if (value < 0)
                    value = 0;
                WriteInt32(value * 60, 10, Buffer);
            }
        }

        public string Name
        {
            get
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(Buffer, 16, 16);
            }
            set
            {
                WriteString(value, 16, Buffer);
            }
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
