using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Clan : Writer
    {
        byte[] Bufffer;
        public byte[] ToArray()
        {
            return Bufffer;
        }
        public byte[] UpgradeBuletin(string name)
        {
            byte[] buletin = new byte[81 + 8 + (byte)name.Length];
            WriteUInt16((ushort)(buletin.Length - 8), 0, buletin);
            WriteUInt16(1312, 2, buletin);

            buletin[4] = 25;//type
            buletin[16] = 1;
            buletin[17] = (byte)name.Length;
            byte pos = 18;
            for (byte x = 0; x < name.Length; x++)
            {
                buletin[x + pos] = Convert.ToByte(name[x]);
            }
           
            return buletin;
        }
        public byte[] SendAlies(string AliesName, string AliesLider)
        {

            int Position = 18;
            //ID  Member Unknow donation Level rank Unknow Unknow Unknow Unknow Unknow Mydonation

            string name1 = AliesName;

            string clanname = AliesLider;
            string LiderName = AliesLider;
            string Unknow = "0 0 0 0 0 0 0";

            byte[]  packet = new byte[8 + 141 + (byte)(clanname.Length + LiderName.Length)];
            WriteUInt16((ushort)(packet.Length - 8), 0, packet);
            WriteUInt16(1312, 2, packet);

            packet[16] = (byte)6;
            packet[17] = (byte)name1.Length;

            WriteByte(16, 4, packet);
            WriteUInt16(459, 8, packet);



            for (int i = 0; i < name1.Length; i++)
            {
                try
                {
                    packet[Position + i] = Convert.ToByte(name1[i]);
                }
                catch { }
            }
            Position += (byte)name1.Length;

            WriteByte((byte)clanname.Length, Position, packet);
            Position += 1;
            for (int i = 0; i < clanname.Length; i++)
            {
                try
                {
                    packet[Position + i] = Convert.ToByte(clanname[i]);
                }
                catch { }
            }


            Position += (byte)clanname.Length;

            WriteByte((byte)LiderName.Length, Position, packet);
            Position += 1;
            for (int i = 0; i < LiderName.Length; i++)
            {
                try
                {
                    packet[Position + i] = Convert.ToByte(LiderName[i]);
                }
                catch { }
            }


            Position += (byte)LiderName.Length;

            WriteByte((byte)Unknow.Length, Position, packet);
            Position += 1;
            for (int i = 0; i < Unknow.Length; i++)
            {
                try
                {
                    packet[Position + i] = Convert.ToByte(Unknow[i]);
                }
                catch { }
            }

            return packet;
        }
        public byte[] SendEnemy(string EnemyName, string EnemyLider)
        {
            byte[] packet = new byte[1];
            return packet;
        }
        private Dictionary<UInt32, Clan> mAllies, mEnemies;
        public Dictionary<UInt32, Clan> Allies { get { return this.mAllies; } }
        public Dictionary<UInt32, Clan> Enemies { get { return this.mEnemies; } }
        public Clan(Client.GameState client, byte ClanType)
        {

            int Position = 18;
            //ID  Member Unknow donation Level rank Unknow Unknow Unknow Unknow Unknow Mydonation

            string name1 = client.Entity.Myclan.ClanId
                + " " + client.Entity.Myclan.Members.Count + " 0 "
                + client.Entity.Myclan.ClanDonation + " "
                + client.Entity.Myclan.ClanLevel + " "
                + client.Entity.Myclan.Members[client.Entity.UID].Rank + " 0 0 0 0 0 "
                + client.Entity.Myclan.Members[client.Entity.UID].Donation;

            string clanname = client.Entity.Myclan.ClanName;
            string LiderName = client.Entity.Myclan.ClanLider;
            string Unknow = "0 0 0 0 0 0 0";

            Bufffer = new byte[8 + 141 + (byte)(clanname.Length + LiderName.Length)];
            WriteUInt16((ushort)(Bufffer.Length - 8), 0, Bufffer);
            WriteUInt16(1312, 2, Bufffer);

            Bufffer[16] = (byte)6;
            Bufffer[17] = (byte)name1.Length;

            WriteByte(ClanType, 4, Bufffer);
            WriteUInt16(459, 8, Bufffer);



            for (int i = 0; i < name1.Length; i++)
            {
                try
                {
                    Bufffer[Position + i] = Convert.ToByte(name1[i]);
                }
                catch { }
            }
            Position += (byte)name1.Length;

            WriteByte((byte)clanname.Length, Position, Bufffer);
            Position += 1;
            for (int i = 0; i < clanname.Length; i++)
            {
                try
                {
                    Bufffer[Position + i] = Convert.ToByte(clanname[i]);
                }
                catch { }
            }


            Position += (byte)clanname.Length;

            WriteByte((byte)LiderName.Length, Position, Bufffer);
            Position += 1;
            for (int i = 0; i < LiderName.Length; i++)
            {
                try
                {
                    Bufffer[Position + i] = Convert.ToByte(LiderName[i]);
                }
                catch { }
            }


            Position += (byte)LiderName.Length;

            WriteByte((byte)Unknow.Length, Position, Bufffer);
            Position += 1;
            for (int i = 0; i < Unknow.Length; i++)
            {
                try
                {
                    Bufffer[Position + i] = Convert.ToByte(Unknow[i]);
                }
                catch { }
            }

        }
    }
}
