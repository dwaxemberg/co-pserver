using System;
using System.Collections.Generic;
using Conquer_Online_Server.Game.ConquerStructures.Society;

namespace Conquer_Online_Server.Database
{
    public class GuildTable
    {
        public static void Load()
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("guilds");
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                Guild guild = new Guild(reader.ReadString("LeaderName"));
                guild.ID = reader.ReadUInt16("ID");
                guild.Name = reader.ReadString("Name");
                guild.Wins = reader.ReadUInt32("Wins");
                guild.Losts = reader.ReadUInt32("Losts");
                guild.Bulletin = reader.ReadString("Bulletin");
                guild.SilverFund = reader.ReadUInt64("SilverFund");
                guild.ConquerPointFund = reader.ReadUInt32("ConquerPointFund");
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
                cmd.Select("entities").Where("GuildID", guild.ID);
                MySqlReader rdr = new MySqlReader(cmd);
                while (rdr.Read())
                {
                    Guild.Member member = new Guild.Member(guild.ID);
                    member.ID = rdr.ReadUInt32("UID");
                    member.Name = rdr.ReadString("Name");
                    member.Level = rdr.ReadByte("Level");

                    if (Game.ConquerStructures.Nobility.Board.ContainsKey(member.ID))
                    {
                        member.NobilityRank = Game.ConquerStructures.Nobility.Board[member.ID].Rank;
                        member.Gender = Game.ConquerStructures.Nobility.Board[member.ID].Gender;
                    }

                    member.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)rdr.ReadUInt16("GuildRank");
                    if (member.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                        guild.Leader = member;
                    else if (member.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                        guild.DeputyLeaderCount++;
                    member.SilverDonation = rdr.ReadUInt64("GuildSilverDonation");
                    member.ConquerPointDonation = rdr.ReadUInt64("GuildConquerPointDonation");
                    guild.Members.Add(member.ID, member);
                }
                guild.MemberCount = (uint)guild.Members.Count;
                ServerBase.Kernel.Guilds.Add(guild.ID, guild);
                rdr.Close();
            }
            reader.Close();
            command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("guildally");
            reader = new MySqlReader(command);
            while (reader.Read())
            {
                ushort guildID = reader.ReadUInt16("GuildID");
                ushort allyID = reader.ReadUInt16("AllyID");
                if (ServerBase.Kernel.Guilds.ContainsKey(allyID))
                {
                    if (Conquer_Online_Server.ServerBase.Kernel.Guilds.ContainsKey(guildID))
                        if (Conquer_Online_Server.ServerBase.Kernel.Guilds.ContainsKey(allyID))
                            ServerBase.Kernel.Guilds[guildID].Ally.Add(allyID, ServerBase.Kernel.Guilds[allyID]);
                }
            }
            reader.Close();
            command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("guildenemy");
            reader = new MySqlReader(command);
            while (reader.Read())
            {
                ushort guildID = reader.ReadUInt16("GuildID");
                ushort enemyID = reader.ReadUInt16("EnemyID");
                if (ServerBase.Kernel.Guilds.ContainsKey(guildID))
                    if (ServerBase.Kernel.Guilds.ContainsKey(enemyID))
                    {
                        if (Conquer_Online_Server.ServerBase.Kernel.Guilds.ContainsKey(guildID))
                            if (Conquer_Online_Server.ServerBase.Kernel.Guilds.ContainsKey(enemyID))
                                ServerBase.Kernel.Guilds[guildID].Enemy.Add(enemyID, ServerBase.Kernel.Guilds[enemyID]);
                    }
            }
            reader.Close();
            Console.WriteLine("Guild information loaded.");
        }
        public static void UpdateBulletin(Game.ConquerStructures.Society.Guild guild, string bulletin)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.UPDATE);
            command.Update("guilds").Set("Bulletin", guild.Bulletin).Where("ID", guild.ID).Execute();
        }
        public static void SaveFunds(Game.ConquerStructures.Society.Guild guild)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.UPDATE);
            command.Update("guilds").Set("ConquerPointFund", guild.ConquerPointFund).Set("SilverFund", guild.SilverFund).Where("ID", guild.ID).Execute();
        }
        public static void Disband(Game.ConquerStructures.Society.Guild guild)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.UPDATE);
            command.Update("entities").Set("GuildID", 0).Where("GuildID", guild.ID).Execute();
            ServerBase.Kernel.Guilds.Remove(guild.ID);
            command = new MySqlCommand(MySqlCommandType.DELETE);
            command.Delete("guilds", "ID", guild.ID).Execute();
        }
        public static void Create(Game.ConquerStructures.Society.Guild guild)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.INSERT);
            command.Insert("guilds").
                Insert("ID", guild.ID).
                Insert("Name", guild.Name).
                Insert("SilverFund", 500000).
                Insert("LeaderName", guild.LeaderName);
            command.Execute();
        }
        public static void AddEnemy(Game.ConquerStructures.Society.Guild guild, uint enemy)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.INSERT);
            command.Insert("guildenemy").Insert("GuildID", guild.ID).Insert("EnemyID", enemy).Execute();
        }
        public static void RemoveEnemy(Game.ConquerStructures.Society.Guild guild, uint enemy)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.DELETE);
            command.Delete("guildenemy", "GuildID", guild.ID).And("EnemyID", enemy).Execute();
        }
        public static void AddAlly(Game.ConquerStructures.Society.Guild guild, uint ally)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.INSERT);
            command.Insert("guildally").Insert("GuildID", guild.ID).Insert("AllyID", ally).Execute();
        }
        public static void RemoveAlly(Game.ConquerStructures.Society.Guild guild, uint ally)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.DELETE);
            command.Delete("guildally", "GuildID", guild.ID).And("AllyID", ally).Execute();
            command = new MySqlCommand(MySqlCommandType.DELETE);
            command.Delete("guildally", "GuildID", ally).And("AllyID", guild.ID).Execute();
        }
        public static void UpdateGuildWarStats(Game.ConquerStructures.Society.Guild guild)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.UPDATE);
            command.Update("guilds").Set("Wins", guild.Wins).Set("Losts", guild.Losts).Where("ID", guild.ID).Execute();
        }
    }
}
