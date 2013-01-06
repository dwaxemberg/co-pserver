using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class TradePartnerInfo : Writer, Interfaces.IPacket
    {
        byte[] Buffer;

        public TradePartnerInfo(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[64];
                WriteUInt16(56, 0, Buffer);
                WriteUInt16(2047, 2, Buffer);
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

        public byte Class
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

        public void Fill(Interfaces.IKnownPerson person)
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
            }
        }
    }
}
