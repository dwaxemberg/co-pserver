using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public static class Clans
    {
        public static void DeleteClan(uint ID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("Clans", "ClanID", ID).Execute();
        }
        public static void TransferClan(string name)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd3.Update("entities")
                .Set("ClanRank", 100).Where("Name", name).Execute();
        }
        public static void KickClan(string name)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd3.Update("entities").Set("ClanDonation", 0)
                .Set("ClanRank", 0)
                .Set("ClanID", 0).Where("Name", name).Execute();
        }
        public static void SaveClientDonation(Client.GameState client)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd3.Update("entities").Set("ClanDonation", client.Entity.Myclan.Members[client.Entity.UID].Donation).Where("ClanID", client.Entity.Myclan.ClanId)
                .And("UID", client.Entity.UID).Execute();
        }
        public static void SaveClan(Game.Clans clan)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("Clans").Set("Fund", clan.ClanDonation).Set("Level", clan.ClanLevel)
                .Set("Bulletin",clan.ClanBuletion).Set("Leader",clan.ClanLider).Where("ClanID", clan.ClanId).Execute();
        }
        public static void JoinClan(Client.GameState client)
        {
            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd3.Update("entities").Set("ClanID", client.Entity.Myclan.ClanId).Set("ClanRank", client.Entity.Myclan.Members[client.Entity.UID].Rank)
                .Set("ClanDonation", client.Entity.Myclan.Members[client.Entity.UID].Donation).Where("UID", client.Entity.UID).Execute();

            Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 1);
            client.Send(cl.ToArray());

        }
        public static void CreateClan(Client.GameState client)
        {
            uint clanid = Game.Clans.ClanCount.Next;
            //clanid = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 40000);
            //if (Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(clanid))
              //  while (!Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(clanid))
                //    clanid = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 400000);
            
            client.Entity.Myclan.ClanId = clanid;
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("Clans").Insert("Name", client.Entity.Myclan.ClanName).Insert("ClanID", clanid)
                .Insert("Leader", client.Entity.Name).Insert("Fund", 500000).Execute();

            MySqlCommand cmd3 = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd3.Update("entities").Set("ClanID", client.Entity.Myclan.ClanId).Set("ClanRank", client.Entity.Myclan.Members[client.Entity.UID].Rank)
                .Set("ClanDonation", client.Entity.Myclan.Members[client.Entity.UID].Donation).Where("UID", client.Entity.UID).Execute();

            client.Entity.ClanRank = 100;
            client.Entity.ClanName = client.Entity.Myclan.ClanName;
            client.Entity.ClanId = clanid;
            Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 1);
            client.Send(cl.ToArray());
            Conquer_Online_Server.ServerBase.Kernel.ServerClans.Add(clanid, client.Entity.Myclan);
        }
        public static void LoadAllClans()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("Clans");
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Game.Clans clan = new Conquer_Online_Server.Game.Clans();
                clan.ClanLider = r.ReadString("Leader");
                clan.ClanId = r.ReadUInt32("ClanID");
                clan.ClanName = r.ReadString("Name");
                clan.ClanBuletion = r.ReadString("Bulletin");
                clan.ClanDonation = r.ReadUInt32("Fund");
                clan.ClanLevel = r.ReadByte("Level");
                if (!Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(clan.ClanId))
                    Conquer_Online_Server.ServerBase.Kernel.ServerClans.Add(clan.ClanId, clan);
            }
            r.Close();
            Console.WriteLine("Clans Loading " + Conquer_Online_Server.ServerBase.Kernel.ServerClans.Count);
            GetMembers();

            foreach (Game.Clans c in ServerBase.Kernel.ServerClans.Values)
            {
                c.LoadAssociates();
            }
        }
        public static void GetMembers()
        {
            foreach (KeyValuePair<uint, Game.Clans> G in Conquer_Online_Server.ServerBase.Kernel.ServerClans)
            {
                Game.Clans clan = G.Value;
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
                cmd.Select("entities").Where("ClanID", clan.ClanId);
                MySqlReader r = new MySqlReader(cmd);
                while (r.Read())
                {

                    Game.ClanMembers member = new Conquer_Online_Server.Game.ClanMembers();
                    member.Donation = r.ReadUInt32("ClanDonation");
                    member.Rank = r.ReadByte("ClanRank");
                    member.UID = r.ReadUInt32("UID");
                    member.Name = r.ReadString("Name");
                    member.Class = r.ReadUInt16("Class");
                    member.Level = r.ReadByte("Level");

                    if (!clan.Members.ContainsKey(member.UID))
                        clan.Members.Add(member.UID, member);
                }
                r.Close();
            }
        }
    }
}
