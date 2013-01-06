using System;
using Conquer_Online_Server.Network.AuthPackets;
namespace Conquer_Online_Server.Network.GamePackets
{
    public class Connect : Interfaces.IPacket
    {
        byte[] Buffer;
        public uint Identifier
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { Writer.WriteUInt32(value, 4, Buffer); }
        }
        public Forward.ForwardType Type
        {
            get
            {                
                return (Forward.ForwardType)(byte)BitConverter.ToUInt32(Buffer, 8);
            }
            set
            {
                Network.Writer.WriteUInt32((byte)value, 8, Buffer);
            }
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
