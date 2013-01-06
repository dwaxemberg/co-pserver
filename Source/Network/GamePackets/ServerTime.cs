using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ServerTime : Writer, Interfaces.IPacket
    {
        public ServerTime()
        {
            Buffer = new byte[36 + 8];
            WriteUInt16(36, 0, Buffer);
            WriteUInt16(1033, 2, Buffer);
        }
        byte[] Buffer;
        public uint Year
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { Writer.WriteUInt32(value - 1900, 8, Buffer); }
        }

        public uint Month
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { Writer.WriteUInt32(value - 1, 12, Buffer); }
        }

        public uint DayOfYear
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { Writer.WriteUInt32(value, 16, Buffer); }
        }
        public uint DayOfMonth
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { Writer.WriteUInt32(value, 20, Buffer); }
        }
        public uint Hour
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { Writer.WriteUInt32(value, 24, Buffer); }
        }
        public uint Minute
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { Writer.WriteUInt32(value, 28, Buffer); }
        }
        public uint Second
        {
            get { return BitConverter.ToUInt32(Buffer, 32); }
            set { Writer.WriteUInt32(value, 32, Buffer); }
        }
        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
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
