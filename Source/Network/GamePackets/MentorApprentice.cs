using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class MentorApprentice : Writer, Interfaces.IPacket
    {
        public const byte
            RequestApprentice = 1,
            RequestMentor = 2,
            LeaveMentor = 3,
            ExpellApprentice = 4,
            AcceptRequestApprentice = 8,
            AcceptRequestMentor = 9,
            DumpApprentice = 18,
            DumpMentor = 19;

        byte[] Buffer;

        public MentorApprentice(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[32];
                WriteUInt16(24, 0, Buffer);
                WriteUInt16(2065, 2, Buffer);
            }
        }

        public uint Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public byte Dynamic
        {
            get { return Buffer[16]; }
            set { Buffer[16] = value; }
        }

        public bool Online
        {
            get { return Buffer[20] == 1; }
            set { Buffer[20] = value == true ? (byte)1 : (byte)0; }
        }

        public string Name
        {
            get
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(Buffer, 22, Buffer[21]);
            }
            set
            {
                byte[] newBuffer = new byte[24 + value.Length + 8];
                Buffer.CopyTo(newBuffer, 0);
                WriteUInt16((ushort)(24 + value.Length), 0, newBuffer);
                WriteStringWithLength(value, 21, newBuffer);
                Buffer = newBuffer;
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
           // client.Send(Buffer);
        }
    }
}
