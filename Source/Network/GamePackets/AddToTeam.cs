using System;
using System.Text;
using System.Collections.Generic;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class AddToTeam : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;
        public AddToTeam()
        {
            Buffer = new byte[44];
            WriteUInt16(36, 0, Buffer);
            WriteUInt16(1026, 2, Buffer);
        }
        public string Name
        {
            set
            {
                Buffer[5] = 1; 
                Buffer[6] = 1;
                Buffer[7] = 1;
                WriteString(value, 8, Buffer);
            }
            get
            {
                return Encoding.ASCII.GetString(Buffer, 8, 16).Trim('\0');
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }
        public uint Mesh
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { WriteUInt32(value, 28, Buffer); }
        }
        public ushort MaxHitpoints
        {
            get { return BitConverter.ToUInt16(Buffer, 32); }
            set { WriteUInt16(value, 32, Buffer); }
        }
        public ushort Hitpoints
        {
            get { return BitConverter.ToUInt16(Buffer, 34); }
            set { WriteUInt16(value, 34, Buffer); }
        }

        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
