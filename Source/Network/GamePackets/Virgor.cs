using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Vigor : Writer, Interfaces.IPacket
    {
        byte[] Buffer;

        public Vigor(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[44];
                WriteUInt16(36, 0, Buffer);
                WriteUInt16(1033, 2, Buffer);
                Type = 2;
            }
        }
        public uint Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint VigorValue
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
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
            client.Send(ToArray());
        }
    }
}
