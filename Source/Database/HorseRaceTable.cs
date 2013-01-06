using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class HorseRaceTable
    {
        public static void Load()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("HorseRace");
            MySqlReader reader = new MySqlReader(cmd);
            while (reader.Read())
            {
                Network.GamePackets.HorseRaceStatistic stat = new Network.GamePackets.HorseRaceStatistic(true);
                stat.Name = reader.ReadString("EntityName");
                stat.EntityID = reader.ReadUInt32("EntityID");
                stat.Rank = reader.ReadByte("Rank");
                stat.CurrentPts = reader.ReadUInt32("CurrentPts");
                Game.ConquerStructures.Society.HorseRace.HorseRaceStatistic.Add(stat.EntityID, stat);
            }
            reader.Close();
        }

        public static void SaveHorseRaceStatistics(Network.GamePackets.HorseRaceStatistic stats)
        {
            var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("HorseRace")
                .Set("EntityName", stats.Name)
                .Set("Rank", stats.Rank)
                .Set("CurrentPts", stats.CurrentPts)
                .Where("EntityID", stats.EntityID);
            cmd.Execute();
        }
        public static void SaveHorseRaceStatistics(Network.GamePackets.HorseRaceStatistic stats, MySql.Data.MySqlClient.MySqlConnection conn)
        {
            var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("HorseRace")
                .Set("EntityName", stats.Name)
                .Set("Rank", stats.Rank)
                .Set("CurrentPts", stats.CurrentPts)
                .Where("EntityID", stats.EntityID);
            cmd.Execute(conn);
        }
        public static void InsertHorseRaceStatistic(Client.GameState client)
        {
            new MySqlCommand(MySqlCommandType.INSERT).Insert("HorseRace")
                .Insert("EntityID", client.HorseRaceStatistic.EntityID)
                .Insert("EntityName", client.HorseRaceStatistic.Name)
                .Insert("Rank", client.HorseRaceStatistic.Rank)
                .Execute();
        }
    }
}