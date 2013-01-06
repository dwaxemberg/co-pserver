using System;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class EnitityCreate : Interfaces.IPacket
    {
        public string Name;
        public ushort Body;
        public byte Class;
        public void Deserialize(byte[] buffer)
        {
            Name = Encoding.ASCII.GetString(buffer, 24, 16).Replace("\0","");
            Body = BitConverter.ToUInt16(buffer, 72);
            Class = (byte)BitConverter.ToUInt16(buffer, 74);
        }
        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }
        public void Send(Client.GameState client)
        {
            throw new NotImplementedException();
        }
    }
}
