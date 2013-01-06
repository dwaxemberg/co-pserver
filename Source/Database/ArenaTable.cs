using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class ArenaTable
    {
        public static void Load()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("arena");
            MySqlReader reader = new MySqlReader(cmd);
            while (reader.Read())
            {
                Network.GamePackets.ArenaStatistic stat = new Network.GamePackets.ArenaStatistic(true);
                stat.EntityID = reader.ReadUInt32("EntityID");
                stat.Name = reader.ReadString("EntityName");
                stat.LastSeasonRank = reader.ReadUInt32("LastSeasonRank");
                stat.LastSeasonArenaPoints = reader.ReadUInt32("LastSeasonArenaPoints");
                stat.ArenaPoints = reader.ReadUInt32("ArenaPoints");
                stat.TodayWin = reader.ReadByte("TodayWin");
                stat.TodayBattles = reader.ReadByte("TodayBattles"); // doesn't exist in the DB, removed it
                stat.LastSeasonWin = reader.ReadUInt32("LastSeasonWin");
                stat.LastSeasonLose = reader.ReadUInt32("LastSeasonLose");
                stat.TotalWin = reader.ReadUInt32("TotalWin");
                stat.TotalLose = reader.ReadUInt32("TotalLose");
                stat.HistoryHonor = reader.ReadUInt32("HistoryHonor");
                stat.CurrentHonor = reader.ReadUInt32("CurrentHonor");
                stat.Level = reader.ReadByte("Level");
                stat.Class = reader.ReadByte("Class");
                stat.Model = reader.ReadUInt32("Model");
                stat.LastArenaPointFill = DateTime.FromBinary(reader.ReadInt64("ArenaPointFill"));

                if (DateTime.Now.DayOfYear != stat.LastArenaPointFill.DayOfYear)
                {
                    stat.LastSeasonArenaPoints = stat.ArenaPoints;
                    stat.LastSeasonWin = stat.TodayWin;
                    stat.LastSeasonLose = stat.TodayBattles - stat.TodayWin;
                    stat.ArenaPoints = ArenaPointFill(stat.Level);
                    stat.LastArenaPointFill = DateTime.Now;
                    stat.TodayWin = 0;
                    stat.TodayBattles = 0;
                }

                Game.ConquerStructures.Arena.ArenaStatistics.Add(stat.EntityID, stat);
            }
            reader.Close();
            Game.ConquerStructures.Arena.Sort();
            Game.ConquerStructures.Arena.YesterdaySort();
            Console.WriteLine("Arena information loaded.");
        }

        public static uint ArenaPointFill(byte level)
        {
            if (level >= 70 && level < 100)
                return 1000;
            else if (level >= 100 && level < 110)
                return 2000;
            else if (level >= 110 && level < 120)
                return 3000;
            else if (level >= 120)
                return 4000;
            return 0;
        }

        public static void SaveArenaStatistics(Network.GamePackets.ArenaStatistic stats)
        {
            var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("arena")
                .Set("LastSeasonRank", stats.LastSeasonRank)
                .Set("ArenaPoints", stats.ArenaPoints)
                .Set("TodayWin", stats.TodayWin)
                .Set("TodayBattles", stats.TodayBattles)
                .Set("LastSeasonWin", stats.LastSeasonWin)
                .Set("LastSeasonLose", stats.LastSeasonLose)
                .Set("TotalWin", stats.TotalWin)
                .Set("TotalLose", stats.TotalLose)
                .Set("HistoryHonor", stats.HistoryHonor)
                .Set("CurrentHonor", stats.CurrentHonor)
                .Set("Level", stats.Level)
                .Set("Class", stats.Class)
                .Set("ArenaPointFill", stats.LastArenaPointFill.Ticks)
                .Set("Model", stats.Model)
                .Set("LastSeasonArenaPoints", stats.LastSeasonArenaPoints)
                .Where("EntityID", stats.EntityID);
            cmd.Execute();
        }
        public static void SaveArenaStatistics(Network.GamePackets.ArenaStatistic stats, MySql.Data.MySqlClient.MySqlConnection conn)
        {
            var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("arena")
                .Set("LastSeasonRank", stats.LastSeasonRank)
                .Set("ArenaPoints", stats.ArenaPoints)
                .Set("TodayWin", stats.TodayWin)
                .Set("TodayBattles", stats.TodayBattles)
                .Set("LastSeasonWin", stats.LastSeasonWin)
                .Set("LastSeasonLose", stats.LastSeasonLose)
                .Set("TotalWin", stats.TotalWin)
                .Set("TotalLose", stats.TotalLose)
                .Set("HistoryHonor", stats.HistoryHonor)
                .Set("CurrentHonor", stats.CurrentHonor)
                .Set("Level", stats.Level)
                .Set("Class", stats.Class)
                .Set("ArenaPointFill", stats.LastArenaPointFill.Ticks)
                .Set("Model", stats.Model)
                .Set("LastSeasonArenaPoints", stats.LastSeasonArenaPoints)
                .Where("EntityID", stats.EntityID);
            cmd.Execute(conn);
        }
        public static void InsertArenaStatistic(Client.GameState client)
        {
            new MySqlCommand(MySqlCommandType.INSERT).Insert("arena")
                .Insert("EntityName", client.ArenaStatistic.Name)
                .Insert("ArenaPoints", client.ArenaStatistic.ArenaPoints)
                .Insert("Level", client.ArenaStatistic.Level)
                .Insert("Class", client.ArenaStatistic.Class)
                .Insert("Model", client.ArenaStatistic.Model)
                .Insert("ArenaPointFill", client.ArenaStatistic.LastArenaPointFill.Ticks)
                .Insert("EntityID", client.ArenaStatistic.EntityID).Execute();
        }

        internal static void UpgradeNameArena(Client.GameState client)
        {
            throw new NotImplementedException();
        }
    }
}
