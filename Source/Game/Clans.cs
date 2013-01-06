using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game
{
    public enum Clan_Typ : byte
    {
        Info = 1,
        Members = 4,
        Recruit = 9,
        AcceptRecruit = 10,
        Join = 11,
        AcceptJoinRequest = 12,
        AddEnemy = 14,
        DeleteEnemy = 15,
        AddAlly = 17,
        AcceptAlliance = 18,
        DeleteAlly = 20,
        TransferLeader = 21,
        KickMember = 22,
        Quit = 23,
        Announce = 24,
        SetAnnouncement = 25,
        Dedicate = 26,
        MyClan = 29
    }
    public class ClanMembers
    {
        public uint UID = 0;
        public uint Level = 0;
        public uint Donation = 0;
        public uint Rank = 0;
        public string Name = "";
        public uint Class = 0;
    }
    public class Clans
    {
        public static ServerBase.Counter ClanCount;
        public static uint[] MoneyClanLevel = new uint[6] { 300000, 600000, 1300000, 300000, 5000000, 10000000 };

        public Clans()
        {

            Members = new Dictionary<uint, ClanMembers>();
            Allies = new SafeDictionary<uint, Clans>(10);
            Enemies = new SafeDictionary<uint, Clans>(10);
        }

        public SafeDictionary<uint, Game.Clans> Allies;
        public SafeDictionary<uint, Game.Clans> Enemies;

        public uint AllyRequest = 0;
        public string ClanName = "";
        public string ClanLider = "";
        public uint ClanId = 0;
        public uint ClanLevel = 0;
        public uint ClanDonation = 0;
        public string ClanBuletion = "";

        public Dictionary<uint, ClanMembers> Members;

        public void AddRelation(UInt32 Relative, Network.GamePackets.ClanRelations.RelationTypes type)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.INSERT);
            Command.Insert("clanrelation")
                .Insert("clanid", this.ClanId)
                .Insert("AssociatedId", Relative)
                .Insert("type", Convert.ToByte(type))
                .Execute();
        }
        public void DeleteRelation(UInt32 Relative, Network.GamePackets.ClanRelations.RelationTypes type)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.DELETE);
            Command.Delete("clanrelation", "clanid", this.ClanId).And("AssociatedId", Relative).And("type", Convert.ToByte(type)).Execute();
        }

        public void SendMessage(Interfaces.IPacket packet)
        {
            Client.GameState mem;
            foreach (ClanMembers member in this.Members.Values)
            {
                if (ServerBase.Kernel.GamePool.TryGetValue(member.UID, out mem))
                    mem.Send(packet);
            }
        }
        public void LoadAssociates()
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.SELECT);
            Command.Select("clanrelation").Where("clanid", this.ClanId).Execute();
            MySqlReader cmd = new MySqlReader(Command);
            while (cmd.Read())
            {
                UInt32 AssociateId = cmd.ReadUInt32("associatedid");
                Network.GamePackets.ClanRelations.RelationTypes Type = (Network.GamePackets.ClanRelations.RelationTypes)(cmd.ReadByte("type"));
                Game.Clans c;
                if (ServerBase.Kernel.ServerClans.TryGetValue(AssociateId, out c))
                {
                    if (Type == Network.GamePackets.ClanRelations.RelationTypes.Allies)
                        this.Allies.Add(AssociateId, c);
                    else
                        this.Enemies.Add(AssociateId, c);
                }
            }
        }
    }
}
