using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class NobilityTable
    {
        public static void Load()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("nobility");
            MySqlReader reader = new MySqlReader(cmd);
            while (reader.Read())
            {
                Game.ConquerStructures.NobilityInformation nobilityinfo = new Conquer_Online_Server.Game.ConquerStructures.NobilityInformation();
                nobilityinfo.EntityUID = reader.ReadUInt32("EntityUID");
                nobilityinfo.Name = reader.ReadString("EntityName");
                nobilityinfo.Donation = reader.ReadUInt64("Donation");
                nobilityinfo.Gender = reader.ReadByte("Gender");
                nobilityinfo.Mesh = reader.ReadUInt32("Mesh");
                Game.ConquerStructures.Nobility.Board.Add(nobilityinfo.EntityUID, nobilityinfo);
            }
            reader.Close();
            Game.ConquerStructures.Nobility.Sort(0);
            Console.WriteLine("Nobility information loaded.");
        }

        public static void InsertNobilityInformation(Game.ConquerStructures.NobilityInformation information)
        {
            new MySqlCommand(MySqlCommandType.INSERT).Insert("nobility").Insert("EntityUID", information.EntityUID).Insert("Donation", information.Donation.ToString()).Insert("EntityName", information.Name).Insert("Gender", information.Gender).Insert("Mesh", information.Mesh).Execute();
        }
        public static void UpdateNobilityInformation(Game.ConquerStructures.NobilityInformation information)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("nobility").Set("Donation", information.Donation.ToString()).Where("EntityUID", information.EntityUID).Execute();
        }
    }
}