using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class TitlePacket : Writer, Interfaces.IPacket
    {
        byte[] Buffer;
        public TitlePacket(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[24];
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(1130, 2, Buffer);
            }
            else
            {
                Buffer = new byte[20];
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(1130, 2, Buffer);
            }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public byte Title
        {
            get { return Buffer[8]; }
            set { Buffer[8] = value; }
        }

        public byte Type
        {
            get { return Buffer[9]; }
            set { Buffer[9] = value; }
        }

        public byte dwParam
        {
            get { return Buffer[10]; }
            set { Buffer[10] = value; }
        }

        public byte dwParam2
        {
            get { return Buffer[11]; }
            set { Buffer[11] = value; }
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
