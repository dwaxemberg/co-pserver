using System;
using System.Text;
namespace Conquer_Online_Server.Network.AuthPackets
{
    public class Forward : Interfaces.IPacket
    {
        public static ServerBase.Counter Incrementer;
        public enum ForwardType : byte { Ready = 2, InvalidInfo = 1, Banned = 0 }
        byte[] Buffer;
        public Forward()
        {
            Buffer = new byte[52];
            Network.Writer.WriteUInt16(52, 0, Buffer);
            Network.Writer.WriteUInt16(0x41f, 2, Buffer);
        }
        public uint Identifier
        {
            get
            {             
                return BitConverter.ToUInt32(Buffer, 4);
            }
            set
            {
                Network.Writer.WriteUInt32(value, 4, Buffer);
            }
        }
        public ForwardType Type
        {
            get
            {
                return (ForwardType)(byte)BitConverter.ToUInt32(Buffer, 8);
            }
            set
            {
                Network.Writer.WriteUInt32((byte)value, 8, Buffer);
            }
        }
        public string IP
        {
            get
            {
                return Encoding.ASCII.GetString(Buffer, 20, 16);
            }
            set
            {
                Network.Writer.WriteString(value, 20, Buffer);
            }
        }
        public ushort Port
        {
            get
            {
                return BitConverter.ToUInt16(Buffer, 12);
            }
            set
            {
                Network.Writer.WriteUInt16(value, 12, Buffer);
            }
        }
        public void Deserialize(byte[] buffer)
        {
            //no implementation
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