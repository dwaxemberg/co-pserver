using System;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class NpcRequest : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }

        public uint NpcID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public byte OptionID
        {
            get { return Buffer[10]; }
            set { Buffer[10] = value; }
        }

        public byte InteractType
        {
            get { return Buffer[11]; }
        }

        public string Input
        {
            get { return Encoding.ASCII.GetString(Buffer, 14, Buffer[13]); }
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
