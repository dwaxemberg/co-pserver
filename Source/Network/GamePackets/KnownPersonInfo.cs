using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class KnownPersonInfo : Writer, Interfaces.IPacket
    {
        byte[] Buffer;

        public KnownPersonInfo(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[52];
                WriteUInt16(44, 0, Buffer);
                WriteUInt16(2033, 2, Buffer);
            }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint Mesh
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public byte Level
        {
            get { return Buffer[12]; }
            set { Buffer[12] = value; }
        }

        public byte Class //This is for friends
        {
            get { return Buffer[13]; }
            set { Buffer[13] = value; }
        }

        public ushort PKPoints
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { WriteUInt16(value, 14, Buffer); }
        }

        public uint GuildID
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public uint GuildRank
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }

        public string Spouse
        {
            get
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(Buffer, 26, 16);
            }
            set
            {
                WriteString(value, 26, Buffer);
            }
        }

        public bool Enemy
        {
            get { return Buffer[42] == 1; }
            set { Buffer[42] = value == true ? (byte)1 : (byte)0; }
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

        public void Fill(Interfaces.IKnownPerson person, bool enemy, bool tradepartner)
        {
            if (person.IsOnline)
            {
                UID = person.Client.Entity.UID;
                Mesh = person.Client.Entity.Mesh;
                Level = person.Client.Entity.Level;
                Class = person.Client.Entity.Class;
                PKPoints = person.Client.Entity.PKPoints;
                Spouse = person.Client.Entity.Spouse;
                if (person.Client.Guild != null)
                {
                    if (person.Client.AsMember != null)
                    {
                        GuildID = person.Client.Guild.ID;
                        GuildRank = (ushort)person.Client.AsMember.Rank;
                    }
                }
                Enemy = enemy;
                if (tradepartner)
                    WriteUInt16(2047, 2, Buffer);
                
            }
        }
    }
}
