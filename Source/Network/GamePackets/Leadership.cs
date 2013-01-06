using System;
using System.Drawing;
using Conquer_Online_Server.Game;
namespace Conquer_Online_Server.Network.GamePackets
{
    public class Leadership : Writer, Interfaces.IPacket
    {
        public const ushort
             Leader = 1,
             Teammate = 0;

        byte[] Buffer;
        public Leadership()
        {
            Buffer = new byte[32 + 8];
            WriteUInt16(32, 0, Buffer);
            WriteUInt16(2045, 2, Buffer);
        }
        //Type = 1 for team
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
        public uint LeaderUID
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }
        public uint IsLeader
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
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
