using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class GameUpdates : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;

        public const byte
            Header = 0,
            Body = 1,
            Footer = 2;

        public GameUpdates(byte _Type, string _String)
        {
            this.Buffer = new byte[25 + _String.Length];
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(2032, 2, Buffer);
            Buffer[11] = 112;
            Buffer[12] = 1;
            Type = _Type;
            String = _String;
        }

        public byte Type
        {
            get { return Buffer[10]; }
            set { Buffer[10] = value; }
        }

        public string String
        {
            get { return BitConverter.ToString(Buffer, 14, Buffer[13]); }
            set { WriteStringWithLength(value, 13, Buffer); }
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] ToArray()
        {
            return Buffer;
        }
    }
}
