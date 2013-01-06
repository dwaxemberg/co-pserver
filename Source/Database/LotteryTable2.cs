using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class LotteryTable2
    {
        public struct LotteryItem2
        {
            public int Rank, Chance;
            public string Name;
            public uint ID;
            public byte Color;
            public byte Sockets;
            public byte Plus;
        }
        public static List<LotteryItem2> LotteryItems = new List<LotteryItem2>();
        public static void Load()
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("lottery2");
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                LotteryItem2 item = new LotteryItem2();
                item.Rank = reader.ReadInt32("rank");
                item.Chance = reader.ReadInt32("chance");
                item.Name = reader.ReadString("prize_name");
                item.ID = reader.ReadUInt32("prize_item");
                item.Color = reader.ReadByte("color");
                item.Sockets = reader.ReadByte("hole_num");
                item.Plus = reader.ReadByte("addition_lev");
                LotteryItems.Add(item);
            }
            reader.Close();
            Console.WriteLine("Lottery2 items loaded.");
        }
    }
}
