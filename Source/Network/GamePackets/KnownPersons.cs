using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class KnownPersons : Writer, Interfaces.IPacket
    {
        public const byte
            RequestFriendship = 10,
            RemovePerson = 14,
            AddFriend = 15,
            RemoveEnemy = 18,
            AddEnemy = 19;

        byte[] Buffer;

        public KnownPersons(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[44];
                WriteUInt16(36, 0, Buffer);
                WriteUInt16(1019, 2, Buffer);
            }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public byte Type
        {
            get { return Buffer[8]; }
            set { Buffer[8] = value; }
        }

        public bool Online
        {
            get { return Buffer[9] == 1; }
            set { Buffer[9] = value == true ? (byte)1 : (byte)0; }
        }

        public string Name
        {
            get
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(Buffer, 20, 16);
            }
            set
            {
                WriteString(value, 20, Buffer);
            }
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
