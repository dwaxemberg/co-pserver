using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class PkWarEvent2
    {
        public static void ResetTopWeeklyPkWar(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("WeeklyPKChampion", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopTrojan(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopTrojan", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopWarrior(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopWarrior", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopNinja(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopNinja", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopWater(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopWaterTaoist", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopFire(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopFireTaoist", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void QQ(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("QQ1", 0).Set("QQ2", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopArcher(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopArcher", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopMonk(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopMonk", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopGL(Client.GameState client)
        {
            foreach (var pClient in ServerBase.Kernel.GamePool.Values)
            {

                pClient.Entity.RemoveFlag(Network.GamePackets.Update.Flags.TopGuildLeader);
                pClient.Entity.TopGuildLeader = 0;

            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopGuildLeader", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void ResetTopDL(Client.GameState client)
        {
            foreach (var pClient in ServerBase.Kernel.GamePool.Values)
            {

                pClient.Entity.RemoveFlag(Network.GamePackets.Update.Flags.TopDeputyLeader);
                pClient.Entity.TopDeputyLeader = 0;

            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopDeputyLeader", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void PkWarSave(Client.GameState client)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("entities")
                .Set("TopTrojan", client.Entity.TopTrojan)
                .Set("TopWarrior", client.Entity.TopWarrior)
                .Set("TopNinja", client.Entity.TopNinja)
                .Set("TopWaterTaoist", client.Entity.TopWaterTaoist)
                .Set("TopFireTaoist", client.Entity.TopFireTaoist)
                .Set("TopArcher", client.Entity.TopArcher)
                .Set("WeeklyPKChampion", client.Entity.WeeklyPKChampion)
                .Set("TopMonk", client.Entity.TopMonk).Where("UID", client.Account.EntityID);
            }
            catch (Exception e) { Program.SaveException(e); }
        }
    }
    public class PkWarEvent
    {
        public static void ResetTopWeeklyPkWar()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("WeeklyPKChampion", 0).Where("WeeklyPKChampion", 1); cmd.Execute();
        }
        public static void ResetTopTrojan()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopTrojan", 0).Where("TopTrojan", 1); cmd.Execute();
        }
        public static void ResetTopWarrior()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopWarrior", 0).Where("TopWarrior", 1); cmd.Execute();
        }
        public static void ResetTopNinja()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopNinja", 0).Where("TopNinja", 1); cmd.Execute();
        }
        public static void ResetTopWater()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopWaterTaoist", 0).Where("TopWaterTaoist", 1); cmd.Execute();
        }
        public static void ResetTopFire()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopFireTaoist", 0).Where("TopFireTaoist", 1); cmd.Execute();
        }
        public static void ResetTopArcher()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopArcher", 0).Where("TopArcher", 1); cmd.Execute();
        }
        public static void ResetTopMonk()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopMonk", 0).Where("TopMonk", 1); cmd.Execute();
        }
        public static void ResetTopGL()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopGuildLeader", 0).Where("TopGuildLeader", 1); cmd.Execute();
        }
        public static void ResetTopDL()
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopDeputyLeader", 0).Where("TopDeputyLeader", 1); cmd.Execute();

        }
        public static void QQ(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("QQ1", 0).Set("QQ2", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void PkWarSave(Client.GameState client)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("entities")
                .Set("TopTrojan", client.Entity.TopTrojan)
                .Set("TopWarrior", client.Entity.TopWarrior)
                .Set("TopNinja", client.Entity.TopNinja)
                .Set("TopWaterTaoist", client.Entity.TopWaterTaoist)
                .Set("TopFireTaoist", client.Entity.TopFireTaoist)
                .Set("TopArcher", client.Entity.TopArcher)
                .Set("WeeklyPKChampion", client.Entity.WeeklyPKChampion)
                .Set("TopMonk", client.Entity.TopMonk).Where("UID", client.Account.EntityID);
            }
            catch (Exception e) { Program.SaveException(e); }
        }

    }
    public class PkWarEvent3
    {
        public static void ResetTopWeeklyPkWar()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("WeeklyPKChampion", 0).Where("WeeklyPKChampion", 1); cmd.Execute();
        }
        public static void ResetTopTrojan2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopTrojan2", 0).Where("TopTrojan2", 1); cmd.Execute();
        }
        public static void ResetTopWarrior2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopWarrior2", 0).Where("TopWarrior2", 1); cmd.Execute();
        }
        public static void ResetTopNinja2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopNinja2", 0).Where("TopNinja2", 1); cmd.Execute();
        }
        public static void ResetTopWater2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopWaterTaoist2", 0).Where("TopWaterTaoist2", 1); cmd.Execute();
        }
        public static void ResetTopFire2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopFireTaoist2", 0).Where("TopFireTaoist2", 1); cmd.Execute();
        }
        public static void ResetTopArcher2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopArcher2", 0).Where("TopArcher2", 1); cmd.Execute();
        }
        public static void ResetTopMonk2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopMonk2", 0).Where("TopMonk2", 1); cmd.Execute();
        }
        public static void ResetTopGL()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("TopGuildLeader", 0).Where("TopGuildLeader", 1); cmd.Execute();
        }
        public static void ResetTopDL()
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TopDeputyLeader", 0).Where("TopDeputyLeader", 1); cmd.Execute();

        }
        public static void QQ(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Select("entities").Set("QQ1", 0).Set("QQ2", 0).Where("UID", client.Entity.UID).Execute();
        }
        public static void PkWarSave(Client.GameState client)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("entities")
                .Set("TopTrojan", client.Entity.TopTrojan)
                .Set("TopWarrior", client.Entity.TopWarrior)
                .Set("TopNinja", client.Entity.TopNinja)
                .Set("TopWaterTaoist", client.Entity.TopWaterTaoist)
                .Set("TopFireTaoist", client.Entity.TopFireTaoist)
                .Set("TopArcher", client.Entity.TopArcher)
                .Set("WeeklyPKChampion", client.Entity.WeeklyPKChampion)
                .Set("TopMonk", client.Entity.TopMonk).Where("UID", client.Account.EntityID);
            }
            catch (Exception e) { Program.SaveException(e); }
        }

    }
    public class ElitePkEvent
    {
        public static void relite1()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 0).Where("My_Title", 18);
        }
        public static void relite2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 0).Where("My_Title", 15);
        }
        public static void relite3()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 0).Where("My_Title", 17);
        }
        public static void relite4()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 0).Where("My_Title", 16);
        }
        public static void My_Title4()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 18).Where("My_Title", 0);
        }
        public static void My_Title3()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 15).Where("My_Title", 0);
        }
        public static void My_Title2()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 17).Where("My_Title", 0);
        }
        public static void My_Title1()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("entities").Set("My_Title", 16).Where("My_Title", 0);
        }
        public static void PkWarSave(Client.GameState client)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("entities")
                .Set("TopTrojan", client.Entity.TopTrojan)
                .Set("TopWarrior", client.Entity.TopWarrior)
                .Set("TopNinja", client.Entity.TopNinja)
                .Set("TopWaterTaoist", client.Entity.TopWaterTaoist)
                .Set("TopFireTaoist", client.Entity.TopFireTaoist)
                .Set("TopArcher", client.Entity.TopArcher)
                .Set("WeeklyPKChampion", client.Entity.WeeklyPKChampion)
                .Set("TopMonk", client.Entity.TopMonk).Where("UID", client.Account.EntityID);
            }
            catch (Exception e) { Program.SaveException(e); }
        }
    }
}