using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Database;
using Conquer_Online_Server.Network.GamePackets;
using System.Drawing;

namespace Conquer_Online_Server
{
    public class Clan2 : Writer, IPacket
    {
        private Byte[] mData;
    
        private UInt32 mLeader, mFund, mAllyRequest;
        private String mName, mInfo, mAnnouncement;
        private Byte mLevel, mBPTower;


        public Clan2()
        {
            mData = new byte[141 + 8];
            WriteUInt16((UInt16)(mData.Length - 8), 0, mData);
            WriteUInt16((UInt16)1312, 2, mData);
        }
        public UInt32 LeaderId
        {
            get { return mLeader; }
            set { mLeader = value; }
        }
        public Game.Clan_Typ Type
        {
            get { return (Game.Clan_Typ)BitConverter.ToUInt32(mData, 4); }
            set { WriteByte((Byte)value, 4, mData); }
        }
        public UInt32 Identifier
        {
            get { return BitConverter.ToUInt32(mData, 8); }
            set { WriteUInt32((UInt32)value, 8, mData); }
        }
        public Byte Offset16
        {
            get { return mData[16]; }
            set { mData[16] = value; }
        }
        public Byte Offset17
        {
            get { return mData[17]; }
            set { mData[17] = value; }
        }
        public String Offset18String
        {
            get { return Encoding.ASCII.GetString(mData, 18, mData[17]).Trim(new Char[] { '\0' }); }
            set { WriteString(value, 18, mData); }
        }
        public String Name
        {
            get { return mName; }
            set { mName = value; }
        }
        public UInt32 Fund
        {
            get { return mFund; }
            set { mFund = value; }
        }
        public Byte Level
        {
            get { return mLevel; }
            set { mLevel = value; }
        }
        public Byte BPTower
        {
            get { return mBPTower; }
            set { mBPTower = value; }
        }
        public String Announcement
        {
            get { return mAnnouncement; }
            set { mAnnouncement = value; }
        }
        public String Info
        {
            get { return mInfo; }
            set { mInfo = value; }
        }
        public UInt32 AllyRequest
        {
            get { return mAllyRequest; }
            set { mAllyRequest = value; }
        }
      
        public UInt32 GetClanId(String name)
        {
            lock (Kernel.ServerClans)
            {
                foreach (Game.Clans clans in Kernel.ServerClans.Values)
                {
                    if (clans.ClanName == name)
                        return clans.ClanId;
                }
            }
            return 0;
        }


        
        public void Send(GameState client) { client.Send(mData); }
        public byte[] ToArray() { return mData; }
        public void Deserialize(byte[] buffer) { mData = buffer; }
    }
}
