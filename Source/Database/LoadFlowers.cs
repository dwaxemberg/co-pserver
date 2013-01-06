using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
   public class Flowers
    {
        public static void LoadFlower()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("cq_flowers");
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Game.Struct.Flowers F = new Game.Struct.Flowers();
                F.Lilies = r.ReadInt32("lilies");
                F.Lilies2day = r.ReadInt32("liliestoday");
                F.Orchads = r.ReadInt32("orchads");
                F.Orchads2day = r.ReadInt32("orchadstoday");
                F.RedRoses = r.ReadInt32("redroses");
                F.RedRoses2day = r.ReadInt32("redrosestoday");
                F.Tulips = r.ReadInt32("tulips");
                F.Tulips2day = r.ReadInt32("tulipstoday");
                Conquer_Online_Server.ServerBase.Kernel.AllFlower.Add(r.ReadUInt32("charuid"), F);
            }
            r.Close();

        }
        public static void SaveFlowerRank(Client.GameState C)
        {
            Game.Struct.Flowers F = C.Entity.MyFlowers;
            if (Conquer_Online_Server.ServerBase.Kernel.AllFlower.ContainsKey(C.Entity.UID))
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("cq_flowers").Set("redroses", F.RedRoses).Set("redrosestoday", F.RedRoses2day).Set("lilies", F.Lilies).Set("liliestoday", F.Lilies2day).Set("tulips", F.Tulips).Set("tulipstoday", F.Tulips2day).Set("orchads", F.Orchads).Set("orchadstoday", F.Orchads2day).Where("charuid", C.Entity.UID).Execute();
            }
            else
            {
                Conquer_Online_Server.ServerBase.Kernel.AllFlower.Add(C.Entity.UID, F);
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("cq_flowers").Insert("Names", C.Entity.Name).Insert("charuid", C.Entity.UID).Insert("redroses", F.RedRoses).Insert("redrosestoday", F.RedRoses2day).Insert("lilies", F.Lilies).Insert("liliestoday", F.Lilies2day).Insert("tulips", F.Tulips).Insert("tulipstoday", F.Tulips2day).Insert("orchads", F.Orchads).Insert("orchadstoday", F.Orchads2day).Execute();
            }
        }

    }
}
