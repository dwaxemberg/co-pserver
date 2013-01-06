using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class MapsTable
    {
        public struct MapInformation
        {
            public ushort ID;
            public ushort BaseID;
            public uint Status;
            public uint Weather;
        }
        public static SafeDictionary<ushort, MapInformation> MapInformations = new SafeDictionary<ushort, MapInformation>(280);
        public static void Load()
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("maps");
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                MapInformation info = new MapInformation();
                info.ID = reader.ReadUInt16("id");
                info.BaseID = reader.ReadUInt16("mapdoc");
                info.Status = reader.ReadUInt32("type");
                info.Weather = reader.ReadUInt32("weather");
                MapInformations.Add(info.ID, info);
            }
            reader.Close();
            Console.WriteLine("Map informations loaded.");
        }
    }
}
