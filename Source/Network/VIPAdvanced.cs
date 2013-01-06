using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class VIPAdvanced : Writer, Interfaces.IPacket
    {
        private Byte[] mData;
        public static explicit operator Byte[](VIPAdvanced d) { return d.mData; }
        public VIPAdvanced(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[16];
                WriteUInt16(8, 0, Buffer);
                WriteUInt16(1129, 2, Buffer);
            }
        }
        byte[] Buffer;
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { Writer.WriteUInt32(value, 4, Buffer); }
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
    public class p1128 : Writer, Interfaces.IPacket
    {
        private Byte[] mData;
        public const byte
           Purify1 = 0,
           Purify = 1,
                           Stabilaze = 2;
        public p1128(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[40];
                WriteUInt16(32, 0, Buffer);
                WriteUInt16(1128, 2, Buffer);
            }
        }
        byte[] Buffer;
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { Writer.WriteUInt32(value, 4, Buffer); }
        }
        public uint UID2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { Writer.WriteUInt32(value, 8, Buffer); }
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
