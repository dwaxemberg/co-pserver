using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Interfaces;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ClanRelations : Writer, IPacket
    {
        private Byte[] mData;
        private Int32 Offset = 20;

        public ClanRelations(Game.Clans  clan, RelationTypes type)
        {
            switch (type)
            {
                case RelationTypes.None:
                    {
                        mData = new Byte[28 + 8];
                        WriteUInt16((UInt16)mData.Length, 0, mData);
                        WriteUInt16(1312, 2, mData);
                        WriteByte((Byte)type, 4, mData);

                        WriteByte((Byte)0, 16, mData);
                        break;
                    }
                case RelationTypes.Allies:
                    {
                        mData = new Byte[28 + (clan.Allies.Count * 56) + 8];
                        WriteUInt16((UInt16)(mData.Length - 8), 0, mData);
                        WriteUInt16(1312, 2, mData);
                        WriteByte((Byte)type, 4, mData);

                        WriteByte((Byte)clan.Allies.Count, 16, mData);

                        foreach (Game.Clans clans in clan.Allies.Values)
                        {
                            WriteUInt16((UInt16)clans.ClanId, Offset, mData); Offset += 4;
                            WriteString(clans.ClanName, Offset, mData); Offset += 36;
                            WriteString(clans.ClanLider, Offset, mData); Offset += 16;
                        }
                        Offset = 20;
                        break;
                    }
                case RelationTypes.Enemies:
                    {
                        mData = new Byte[28 + (clan.Enemies.Count * 56) + 8];
                        WriteUInt16((UInt16)(mData.Length - 8), 0, mData);
                        WriteUInt16(1312, 2, mData);
                        WriteByte((Byte)type, 4, mData);

                        WriteByte((Byte)clan.Enemies.Count, 16, mData);

                        foreach (Game.Clans clans in clan.Enemies.Values)
                        {
                            WriteUInt16((UInt16)clans.ClanId, Offset, mData); Offset += 4;
                            WriteString(clans.ClanName, Offset, mData); Offset += 36;
                            WriteString(clans.ClanLider, Offset, mData); Offset += 16;
                        }
                        Offset = 20;
                        break;
                    }
            }
        }

        public enum RelationTypes : byte
        {
            None = 0,
            Enemies = 13,
            Allies = 16
        }

        public void Send(Client.GameState client)
        {
            client.Send(mData);
        }

        public byte[] ToArray()
        {
            return mData;
        }

        public void Deserialize(byte[] buffer)
        {
            mData = buffer;
        }
    }
}
