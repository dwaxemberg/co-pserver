using System;
using System.IO;
using System.Text;
using Conquer_Online_Server.ServerBase;
using System.Collections.Generic;
using Conquer_Online_Server.Game;
using Conquer_Online_Server.Game.ConquerStructures;
using Conquer_Online_Server.Game.Tournaments;
namespace Conquer_Online_Server.Database
{
    public static class EntityTable
    {
        public static void NextUit()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("accounts");
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                uint uid = r.ReadUInt32("EntityID");
                if (uid < 1000000)
                {
                    if (uid > Conquer_Online_Server.Client.AuthState.nextID)
                        Conquer_Online_Server.Client.AuthState.nextID = uid;
                }
            }
            r.Close();
            Conquer_Online_Server.Client.AuthState.nextID += 1;
            Console.WriteLine(Conquer_Online_Server.Client.AuthState.nextID);
        }
        public static bool LoadTop2(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("Top2").Where("UID", client.Account.EntityID);
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                //client.Entity.UID = r.ReadUInt32("UID");
                client.Entity.TopMonk2 = r.ReadUInt16("TopMonk2");
                client.Entity.TopTrojan2 = r.ReadUInt16("TopTrojan2");
                client.Entity.TopWarrior2 = r.ReadUInt16("TopWarrior2");
                client.Entity.TopNinja2 = r.ReadUInt16("TopNinja2");
                client.Entity.TopWaterTaoist2 = r.ReadUInt16("TopWaterTaoist2");
                client.Entity.TopArcher2 = r.ReadUInt16("TopArcher2");
                client.Entity.TopFireTaoist2 = r.ReadUInt16("TopFireTaoist2");
            }
            else
                return false;
            return true;
        }
        public static bool LoadEntity(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Where("UID", client.Account.EntityID);
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                client.Entity = new Game.Entity(Game.EntityFlag.Player, false);
                client.Entity.Name = r.ReadString("Name");
                client.Entity.Spouse = r.ReadString("Spouse");
                client.Entity.Owner = client;
                client.WarehousePW = r.ReadString("WarehousePW");
                client.MoneySave = r.ReadUInt32("MoneySave");
                client.Entity.Experience = r.ReadUInt64("Experience");
                client.Entity.Money = r.ReadUInt32("Money");
                client.Entity.ConquerPoints = (uint)r.ReadUInt32("ConquerPoints");
                client.Entity.UID = r.ReadUInt32("UID");
                client.Entity.Hitpoints = r.ReadUInt32("Hitpoints");
                client.Entity.QuizPoints = r.ReadUInt32("QuizPoints");
                client.Entity.Body = r.ReadUInt16("Body");
                client.Entity.Face = r.ReadUInt16("Face");
                client.Entity.Strength = r.ReadUInt16("Strength");
                client.Entity.Agility = r.ReadUInt16("Agility");
                client.Entity.Spirit = r.ReadUInt16("Spirit");
                client.Entity.Vitality = r.ReadUInt16("Vitality");
                client.Entity.Atributes = r.ReadUInt16("Atributes");
                client.VirtuePoints = r.ReadUInt32("VirtuePoints");
                client.Entity.Mana = r.ReadUInt16("Mana");
                client.Entity.HairStyle = r.ReadUInt16("HairStyle");
                client.Entity.MapID = r.ReadUInt16("MapID");
                client.VendingDisguise = r.ReadUInt16("VendingDisguise");
                if (client.VendingDisguise == 0)
                    client.VendingDisguise = 223;
                client.Entity.X = r.ReadUInt16("X");
                client.Entity.Y = r.ReadUInt16("Y");
                client.BlessTime = r.ReadUInt32("BlessTime");
                client.Entity.TopTrojan = r.ReadUInt16("TopTrojan");
                client.Entity.TopWarrior = r.ReadUInt16("TopWarrior");
                client.Entity.TopNinja = r.ReadUInt16("TopNinja");
                client.Entity.TopWaterTaoist = r.ReadUInt16("TopWaterTaoist");
                client.Entity.TopArcher = r.ReadUInt16("TopArcher");
                client.Entity.TopGuildLeader = r.ReadUInt16("TopGuildLeader");
                client.Entity.TopFireTaoist = r.ReadUInt16("TopFireTaoist");
                client.Entity.TopDeputyLeader = r.ReadUInt16("TopDeputyLeader");
                client.Entity.WeeklyPKChampion = r.ReadUInt16("WeeklyPKChampion");
                client.Entity.TopMonk = r.ReadUInt16("TopMonk");
                client.Entity.TopGuildLeader = r.ReadUInt16("TopGuildLeader");


                client.Entity.QQ1 = r.ReadUInt16("QQ1");
                client.Entity.QQ2 = r.ReadUInt16("QQ2");
                client.Entity.QQ3 = r.ReadUInt16("QQ3");
                client.LotteryEntries2 = r.ReadByte("LotteryEntries2");//not fond
                //client.Entity.TitlePacket = new Network.GamePackets.TitlePacket(true);
                //client.Entity.TitlePacket.UID = client.Entity.UID;
                //client.Entity.TitlePacket.Type = 4;
                //client.Entity.TitlePacket.dwParam = 1;
                //client.Entity.TitlePacket.dwParam2 = r.ReadByte("My_Title");
                client.ChatBanTime = DateTime.FromBinary(r.ReadInt64("ChatBanTime"));
                client.ChatBanLasts = r.ReadUInt32("ChatBanLasts");
                client.ChatBanned = r.ReadBoolean("ChatBanned");
                client.HeadgearClaim = r.ReadBoolean("HeadgearClaim");
                client.NecklaceClaim = r.ReadBoolean("NecklaceClaim");
                client.ArmorClaim = r.ReadBoolean("ArmorClaim");
                client.WeaponClaim = r.ReadBoolean("WeaponClaim");
                client.RingClaim = r.ReadBoolean("RingClaim");
                client.BootsClaim = r.ReadBoolean("BootsClaim");
                client.FanClaim = r.ReadBoolean("FanClaim");
                client.TowerClaim = r.ReadBoolean("TowerClaim");
                client.HeadgearClaim = r.ReadBoolean("HeadgearClaim");
                client.InLottery = r.ReadBoolean("InLottery");
                client.LotteryEntries = r.ReadByte("LotteryEntries");
                // client.OldSkillsLoad = r.ReadBoolean("OldLoadSkills");
                client.Entity.SubClass = r.ReadByte("SubClass");
                client.Entity.SubClassLevel = r.ReadByte("SubClassLevel");
                //if (client.OldSkillsLoad)
                //{
                //    new MySqlCommand(MySqlCommandType.UPDATE).Update("entities").Set("OldLoadSkills", 0).Where("UID", client.Entity.UID).Execute();
                //    var cmdd = new MySqlCommand(MySqlCommandType.INSERT);
                //    cmdd.Insert("n_skills").Insert("EntityID", client.Entity.UID);
                //    cmdd.Execute();
                //}
                client.LastLotteryEntry = DateTime.FromBinary(r.ReadInt64("LastLotteryEntry"));
                if (client.Entity.MapID >= 7008)
                {
                    client.Entity.MapID = 1002;
                    client.Entity.X = 430;
                    client.Entity.Y = 380;
                }
                client.Entity.ReincarnationLev = r.ReadByte("ReincarnationLev");
                client.Entity.PreviousMapID = r.ReadUInt16("PreviousMapID");
                client.Entity.PKPoints = r.ReadUInt16("PKPoints");
                client.Entity.Class = r.ReadByte("Class");
                client.Entity.Reborn = r.ReadByte("Reborn");
                client.Entity.Level = r.ReadByte("Level");
                client.Entity.FirstRebornClass = r.ReadByte("FirstRebornClass");
                client.Entity.SecondRebornClass = r.ReadByte("SecondRebornClass");
                client.Entity.FirstRebornLevel = r.ReadByte("FirstRebornLevel");
                client.Entity.SecondRebornLevel = r.ReadByte("SecondRebornLevel");
                client.LastDragonBallUse = DateTime.FromBinary(r.ReadInt64("LastDragonBallUse"));
                client.LastResetTime = DateTime.FromBinary(r.ReadInt64("LastResetTime"));
                client.Entity.EnlightenPoints = r.ReadUInt16("EnlightenPoints");
                client.Entity.EnlightmentTime = r.ReadUInt16("EnlightmentWait");
                if (client.Entity.EnlightmentTime > 0)
                {
                    if (client.Entity.EnlightmentTime % 20 > 0)
                    {
                        client.Entity.EnlightmentTime -= (ushort)(client.Entity.EnlightmentTime % 20);
                        client.Entity.EnlightmentTime += 20;
                    }
                }
                client.Entity.ReceivedEnlightenPoints = r.ReadByte("EnlightsReceived");
                client.Entity.DoubleExperienceTime = r.ReadUInt16("DoubleExpTime");
                client.Entity.DoubleExperienceTime5 = r.ReadUInt16("DoubleExpTime5");
                client.Entity.DoubleExperienceTime10 = r.ReadUInt16("DoubleExpTime10");
                client.Entity.DoubleExperienceTime15 = r.ReadUInt16("DoubleExpTime15");
                client.Entity.DoubleExperienceTimeV1 = r.ReadUInt16("CPsBag");
                client.DoubleExpToday = r.ReadBoolean("DoubleExpToday");
                client.Entity.HeavenBlessing = r.ReadUInt32("HeavenBlessingTime");
                client.Entity.VIPLevel = r.ReadByte("VIPLevel");
                client.Entity.PrevX = r.ReadUInt16("PreviousX");
                client.Entity.Guild_points = r.ReadUInt32("GuildPoints");
                client.Entity.PrevY = r.ReadUInt16("PreviousY");
                client.ExpBalls = r.ReadByte("ExpBalls");
                client.Entity.AddFlower = r.ReadUInt16("Flower");

                // client.Entity.VotsPoints = r.ReadUInt16("VotsPoints");
                // long timer = r.ReadInt64("TimerVot");
                //// if (timer == 0)
                //// {
                //     client.Entity.TimerVot = DateTime.Now;
                //     client.Entity.SaveTimeVot();
                // }
                // client.Entity.TimerVot = DateTime.FromBinary(timer);

                if (client.Entity.MapID == 601)
                    client.OfflineTGEnterTime = DateTime.FromBinary(r.ReadInt64("OfflineTGEnterTime"));

                if (ServerBase.Kernel.Guilds.ContainsKey(r.ReadUInt16("GuildID")))
                {
                    client.Guild = ServerBase.Kernel.Guilds[r.ReadUInt16("GuildID")];
                    if (client.Guild.Members.ContainsKey(client.Entity.UID))
                    {
                        client.AsMember = client.Guild.Members[client.Entity.UID];
                        if (client.AsMember.GuildID == 0)
                        {
                            client.AsMember = null;
                            client.Guild = null;
                        }
                        else
                        {
                            client.Entity.GuildID = (ushort)client.Guild.ID;
                            client.Entity.GuildRank = (ushort)client.AsMember.Rank;
                        }
                    }
                    else
                        client.Guild = null;
                }
                if (!Game.ConquerStructures.Nobility.Board.TryGetValue(client.Entity.UID, out client.NobilityInformation))
                {
                    client.NobilityInformation = new Conquer_Online_Server.Game.ConquerStructures.NobilityInformation();
                    client.NobilityInformation.EntityUID = client.Entity.UID;
                    client.NobilityInformation.Name = client.Entity.Name;
                    client.NobilityInformation.Donation = 0;
                    client.NobilityInformation.Rank = Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Serf;
                    client.NobilityInformation.Position = -1;
                    client.NobilityInformation.Gender = 1;
                    client.NobilityInformation.Mesh = client.Entity.Mesh;
                    if (client.Entity.Body % 10 >= 3)
                        client.NobilityInformation.Gender = 0;
                }
                else
                    client.Entity.NobilityRank = client.NobilityInformation.Rank;



                if (DateTime.Now.DayOfYear != client.LastResetTime.DayOfYear)
                {
                    if (client.Entity.Level >= 90)
                    {
                        client.Entity.EnlightenPoints = 100;
                        if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Knight ||
                            client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Baron)
                            client.Entity.EnlightenPoints += 100;
                        else if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Earl ||
                            client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Duke)
                            client.Entity.EnlightenPoints += 200;
                        else if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Prince)
                            client.Entity.EnlightenPoints += 300;
                        else if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.King)
                            client.Entity.EnlightenPoints += 400;
                        if (client.Entity.VIPLevel != 0)
                        {
                            if (client.Entity.VIPLevel <= 3)
                                client.Entity.EnlightenPoints += 100;
                            else if (client.Entity.VIPLevel <= 5)
                                client.Entity.EnlightenPoints += 200;
                            else if (client.Entity.VIPLevel == 6)
                                client.Entity.EnlightenPoints += 300;
                        }
                    }
                    client.Entity.ReceivedEnlightenPoints = 0;
                    client.DoubleExpToday = false;
                    client.ExpBalls = 0;
                    client.Entity.AddFlower = 1;
                    client.LastResetTime = DateTime.Now;
                }
                Game.ConquerStructures.Arena.ArenaStatistics.TryGetValue(client.Entity.UID, out client.ArenaStatistic);
                if (client.ArenaStatistic == null || client.ArenaStatistic.EntityID == 0)
                {
                    client.ArenaStatistic = new Conquer_Online_Server.Network.GamePackets.ArenaStatistic(true);
                    client.ArenaStatistic.EntityID = client.Entity.UID;
                    client.ArenaStatistic.Name = client.Entity.Name;
                    client.ArenaStatistic.Level = client.Entity.Level;
                    client.ArenaStatistic.Class = client.Entity.Class;
                    client.ArenaStatistic.Model = client.Entity.Mesh;
                    client.ArenaStatistic.ArenaPoints = ArenaTable.ArenaPointFill(client.Entity.Level);
                    client.ArenaStatistic.LastArenaPointFill = DateTime.Now;
                    ArenaTable.InsertArenaStatistic(client);
                    client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                    Game.ConquerStructures.Arena.ArenaStatistics.Add(client.Entity.UID, client.ArenaStatistic);
                }
                else
                {
                    client.ArenaStatistic.Level = client.Entity.Level;
                    client.ArenaStatistic.Class = client.Entity.Class;
                    client.ArenaStatistic.Model = client.Entity.Mesh;
                    if (DateTime.Now.DayOfYear != client.ArenaStatistic.LastArenaPointFill.DayOfYear)
                    {
                        client.ArenaStatistic.LastSeasonArenaPoints = client.ArenaStatistic.ArenaPoints;
                        client.ArenaStatistic.LastSeasonWin = client.ArenaStatistic.TodayWin;
                        client.ArenaStatistic.LastSeasonLose = client.ArenaStatistic.TodayBattles - client.ArenaStatistic.TodayWin;
                        client.ArenaStatistic.ArenaPoints = ArenaTable.ArenaPointFill(client.Entity.Level);
                        client.ArenaStatistic.LastArenaPointFill = DateTime.Now;
                        client.ArenaStatistic.TodayWin = 0;
                        client.ArenaStatistic.TodayBattles = 0;
                        Game.ConquerStructures.Arena.Sort();
                        Game.ConquerStructures.Arena.YesterdaySort();
                    }
                }
                if (r.ReadUInt32("ClanID") != 0)
                {
                    if (Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(r.ReadUInt32("ClanID")))
                    {
                        client.Entity.ClanId = r.ReadUInt32("ClanID");
                        client.Entity.Myclan = Conquer_Online_Server.ServerBase.Kernel.ServerClans[r.ReadUInt32("ClanID")];
                        client.Entity.ClanName = client.Entity.Myclan.ClanName;
                        client.Entity.ClanRank = (byte)client.Entity.Myclan.Members[client.Entity.UID].Rank;
                    }
                }

                // client.Entity.SubClasses.Active = client.Entity.SubClass;
                client.Entity.SubClasses.StudyPoints = r.ReadUInt64("StudyPoints");
                r.Close();
                SubClassTable.Load(client.Entity);
                SkillTable.LoadProficiencies(client);
                SkillTable.LoadSpells(client);
                KnownPersons.LoadKnownPersons(client);
                ConquerItemTable.LoadItems(client);
                DetainedItemTable.LoadClaimableItems(client);
                DetainedItemTable.LoadDetainedItems(client);
                Database.EntityTable.LoadTop2(client);
                EliteTournament.LoginClient2(client);


                client.Entity.FullyLoaded = true;
            }
            else
                return false;
            return true;
        }
        public static void UpdateOnlineStatus(Client.GameState client, bool online)
        {
            if (online || (!online && client.DoSetOffline))
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("entities").Set("Online", online).Where("UID", client.Entity.UID).Execute();
            }
        }
        public static void ResetLottery(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("LotteryEntries", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetLottery2(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("LotteryEntries2", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetExpball(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("ExpBalls", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static bool SaveEntity(Client.GameState client)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("entities").Set("WarehousePW", client.WarehousePW)
                    .Set("LotteryEntries2", client.LotteryEntries2)
                    .Set("Spouse", client.Entity.Spouse).Set("Money", client.Entity.Money)
                    .Set("ConquerPoints", client.Entity.ConquerPoints).Set("Body", client.Entity.Body)
                    .Set("Face", client.Entity.Face).Set("Class", client.Entity.Class).Set("Reborn", client.Entity.Reborn)
                    .Set("Level", client.Entity.Level).Set("HairStyle", client.Entity.HairStyle).Set("EnlightsReceived", client.Entity.ReceivedEnlightenPoints)
                    .Set("PKPoints", client.Entity.PKPoints).Set("QuizPoints", client.Entity.QuizPoints)
                    .Set("Experience", client.Entity.Experience).Set("MoneySave", client.MoneySave)
                    .Set("Hitpoints", client.Entity.Hitpoints).Set("LastDragonBallUse", client.LastDragonBallUse.ToBinary())
                    .Set("Strength", client.Entity.Strength).Set("Agility", client.Entity.Agility)
                    .Set("Spirit", client.Entity.Spirit).Set("Vitality", client.Entity.Vitality)
                    .Set("PreviousX", client.Entity.PrevX).Set("PreviousY", client.Entity.PrevY)
                    .Set("Atributes", client.Entity.Atributes).Set("Mana", client.Entity.Mana).Set("VIPLevel", client.Entity.VIPLevel)
                    .Set("MapID", client.Map.ID).Set("X", client.Entity.X).Set("Y", client.Entity.Y).Set("VirtuePoints", client.VirtuePoints)
                    .Set("PreviousMapID", client.Entity.PreviousMapID).Set("EnlightenPoints", client.Entity.EnlightenPoints)
                    .Set("LastResetTime", client.LastResetTime.ToBinary())
                    .Set("DoubleExpTime", client.Entity.DoubleExperienceTime)
                    .Set("DoubleExpTime5", client.Entity.DoubleExperienceTime5)
                    .Set("DoubleExpTime10", client.Entity.DoubleExperienceTime10)
                    .Set("DoubleExpTime15", client.Entity.DoubleExperienceTime15)
                    .Set("CPsBag", client.Entity.DoubleExperienceTimeV1)
                    .Set("DoubleExpToday", client.DoubleExpToday).Set("HeavenBlessingTime", client.Entity.HeavenBlessing)
                    .Set("InLottery", client.InLottery).Set("LotteryEntries", client.LotteryEntries).Set("LastLotteryEntry", client.LastLotteryEntry.Ticks)
                    .Set("HeadgearClaim", client.HeadgearClaim).Set("NecklaceClaim", client.NecklaceClaim).Set("ArmorClaim", client.ArmorClaim)
                    .Set("WeaponClaim", client.WeaponClaim).Set("RingClaim", client.RingClaim).Set("BootsClaim", client.BootsClaim)
                    .Set("TowerClaim", client.TowerClaim).Set("FanClaim", client.FanClaim).Set("ChatBanTime", client.ChatBanTime.Ticks)
                    .Set("ChatBanLasts", client.ChatBanLasts).Set("ChatBanned", client.ChatBanned).Set("BlessTime", client.BlessTime)
                    .Set("ExpBalls", client.ExpBalls)


                    .Set("FirstRebornLevel", client.Entity.FirstRebornLevel)
                    .Set("SecondRebornLevel", client.Entity.SecondRebornLevel)
                    .Set("Money", client.Entity.Money)
                    .Set("ConquerPoints", client.Entity.ConquerPoints).Set("EnlightmentWait", client.Entity.EnlightmentTime);
                if (client.Entity.Reborn == 1)
                {
                    cmd.Set("FirstRebornClass", client.Entity.FirstRebornClass);
                }
                if (client.Entity.Reborn == 2)
                {
                    cmd.Set("SecondRebornClass", client.Entity.SecondRebornClass);
                }
                if (client.Entity.MapID == 601)
                    cmd.Set("OfflineTGEnterTime", client.OfflineTGEnterTime.Ticks);
                else
                    cmd.Set("OfflineTGEnterTime", 0);

                if (client.AsMember != null)
                {
                    cmd.Set("GuildID", client.AsMember.GuildID).
                        Set("GuildRank", (ushort)client.AsMember.Rank).
                        Set("GuildSilverDonation", client.AsMember.SilverDonation).
                        Set("GuildConquerPointDonation", client.AsMember.ConquerPointDonation);
                }
                else
                {
                    cmd.Set("GuildID", 0).
                       Set("GuildRank", 0).
                       Set("GuildSilverDonation", 0).
                       Set("GuildConquerPointDonation", 0);
                }
                cmd.Where("UID", client.Entity.UID).Execute();
            }
            catch (Exception e) { Program.SaveException(e); } return true;
        }
        static bool InvalidCharacters(string Name)
        {
            foreach (char c in Name)
                if (ServerBase.Kernel.InvalidCharacters.Contains(c) || (byte)c < 48)
                    return true;
            return false;
        }
        public static void SaveTop2(Entity C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("Top2")
           .Insert("TopTrojan2", C.TopTrojan2)
           .Insert("TopWarrior2", C.TopWarrior2)
           .Insert("TopMonk2", C.TopMonk2)
           .Insert("TopNinja2", C.TopNinja2)
           .Insert("TopWaterTaoist2", C.TopWaterTaoist2)
           .Insert("TopArcher2", C.TopArcher2)
           .Insert("TopFireTaoist2", C.TopFireTaoist2)
           .Insert("UID", C.UID).Execute();
        }
        public static void DeleteTabelTop22(Client.GameState C)
        {
            MySqlCommand cmdd = new MySqlCommand(MySqlCommandType.UPDATE);
            int ress = cmdd.Update("Top2")
                .Set("UID", 0)
                .Set("TopTrojan2", 0)
                .Set("TopWarrior2", 0)
                .Set("TopMonk2", 0)
                .Set("TopNinja2", 0)
                .Set("TopWaterTaoist2", 0)
                .Set("TopArcher2", 0)
                .Set("TopFireTaoist2", 0)
                .Where("UID", C.Entity.UID).Execute();

        }
        public static void DeleteTabelTop2(Client.GameState C)
        {
            MySqlCommand cmds = new MySqlCommand(MySqlCommandType.DELETE)
                .Delete("Top2", "TopTrojan2", C.Entity.TopTrojan2)
                .Delete("Top2", "TopWarrior2", C.Entity.TopWarrior2)
                .Delete("Top2", "TopMonk2", C.Entity.TopMonk2)
                .Delete("Top2", "TopNinja2", C.Entity.TopNinja2)
                .Delete("Top2", "TopWaterTaoist2", C.Entity.TopWaterTaoist2)
                .Delete("Top2", "TopArcher2", C.Entity.TopArcher2)
                .Delete("Top2", "TopFireTaoist2", C.Entity.TopFireTaoist2)
                .Delete("Top2", "UID", C.Entity.UID);
            cmds.Execute();


        }
        public static void SaveTop(Entity C)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities")
                 .Set("QQ3", C.QQ3)
                        .Set("QQ1", C.QQ2)
                         .Set("QQ2", C.QQ1)
                .Set("TopTrojan", C.TopTrojan)
                .Set("TopWarrior", C.TopWarrior)
                .Set("TopMonk", C.TopMonk)
                .Set("TopNinja", C.TopNinja)
                .Set("TopWaterTaoist", C.TopWaterTaoist)
                .Set("TopArcher", C.TopArcher)
                .Set("TopFireTaoist", C.TopFireTaoist)
                .Set("TopGuildLeader", C.TopGuildLeader)
                .Set("TopDeputyLeader", C.TopDeputyLeader)
                .Set("WeeklyPKChampion", C.WeeklyPKChampion)
                .Where("UID", C.UID).Execute();
        }
        public static void SavePlayersVot(PlayersVot PlayerVot)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("VoteIp").Insert("ID", PlayerVot.Uid).Insert("IP", PlayerVot.AdressIp).Execute();

        }
        public static void DeletVotes(PlayersVot PlayerVot)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE).Delete("VoteIp", "ID", PlayerVot.Uid).And("IP", PlayerVot.AdressIp);
            cmd.Execute();
        }

        public static void LoadPlayersVots()
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("VoteIp");
            MySqlReader d = new MySqlReader(cmd);
            while (d.Read())
            {
                PlayersVot Vot = new PlayersVot();
                Vot.Uid = d.ReadUInt32("ID");
                Vot.AdressIp = d.ReadString("IP");
                if (!Kernel.VotePool.ContainsKey(Vot.AdressIp))
                    Kernel.VotePool.Add(Vot.AdressIp, Vot);
                if (!Kernel.VotePoolUid.ContainsKey(Vot.Uid))
                    Kernel.VotePoolUid.Add(Vot.Uid, Vot);
            }
        }
        public static bool CreateEntity(Network.GamePackets.EnitityCreate eC, Client.GameState client, ref string message)
        {
            if (eC.Name.Length > 16)
                eC.Name = eC.Name.Substring(0, 16);
            if (eC.Name == "")
                return false;

            if (InvalidCharacters(eC.Name))
            {
                message = "Invalid characters inside the name.";
                return false;
            }
            var rdr = new MySqlReader(new MySqlCommand(MySqlCommandType.SELECT).Select("entities").Where("name", eC.Name));
            if (rdr.Read())
            {
                rdr.Close();
                message = "The chosen name is already in use.";
                return false;
            }
            rdr.Close();
            client.Entity = new Game.Entity(Game.EntityFlag.Player, false);
            client.Entity.Name = eC.Name;
            DataHolder.GetStats(eC.Class, 1, client);
            client.Entity.UID = Program.EntityUID.Next;
            new MySqlCommand(MySqlCommandType.UPDATE).Update("configuration").Set("EntityID", client.Entity.UID).Where("Server", ServerBase.Constants.ServerName).Execute();
            client.CalculateStatBonus();
            client.CalculateHPBonus();
            client.Entity.Hitpoints = client.Entity.MaxHitpoints;
            client.Entity.Mana = (ushort)(client.Entity.Spirit * 5);
            client.Entity.Class = eC.Class;
            client.Entity.Body = eC.Body;
            if (eC.Body == 1003 || eC.Body == 1004)
                client.Entity.Face = (ushort)ServerBase.Kernel.Random.Next(1, 50);
            else
                client.Entity.Face = (ushort)ServerBase.Kernel.Random.Next(201, 250);
            byte Color = (byte)ServerBase.Kernel.Random.Next(4, 8);
            client.Entity.HairStyle = (ushort)(Color * 100 + 10 + (byte)ServerBase.Kernel.Random.Next(4, 9));
            client.Account.EntityID = client.Entity.UID;
            client.Account.Save();
            //723753

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("entities").Insert("Name", eC.Name).Insert("Owner", client.Account.Username).Insert("Class", eC.Class).Insert("UID", client.Entity.UID)
                .Insert("Hitpoints", client.Entity.Hitpoints).Insert("Mana", client.Entity.Mana).Insert("Body", client.Entity.Body)
                .Insert("Face", client.Entity.Face).Insert("HairStyle", client.Entity.HairStyle).Insert("Strength", client.Entity.Strength)
                .Insert("Agility", client.Entity.Agility).Insert("Vitality", client.Entity.Vitality).Insert("Spirit", client.Entity.Spirit);

            cmd.Execute();
            message = "ANSWER_OK";

            return true;
        }
    }
}
