using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{

    public class PremiumTable
    {
        static bool inPremium(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("premium").Where("Username", client.Account.Username);
            MySqlReader rdr = new MySqlReader(cmd);
            if (rdr.Read())
            {
                rdr.Close();
                return true;
            }
            else
            {
                rdr.Close();
                MySqlCommand Do = new MySqlCommand(MySqlCommandType.INSERT);
                Do.Insert("premium").Insert("EntityName", client.Entity.Name).Insert("Username", client.Account.Username).Insert("IP", client.IP).Execute();
                return true;
            }
        }
        
        #region Donation
        public static void getDp(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("premium").Where("Username", client.Account.Username);
            MySqlReader rdr = new MySqlReader(cmd);
            if (rdr.Read())
            {
                client.DonationPoints = rdr.ReadUInt32("DonationPoint");
                rdr.Close();
            }
            else
            {
                client.DonationPoints = 0;
                MySqlCommand Do = new MySqlCommand(MySqlCommandType.INSERT);
                Do.Insert("premium").Insert("EntityName", client.Entity.Name).Insert("Username", client.Account.Username).Insert("IP", client.IP).Execute();
                rdr.Close();
            }
        }
        public static void updateDp(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("premium").Set("DonationPoint", client.DonationPoints).Where("Username", client.Account.Username).Execute();
        }
        #endregion

        #region VoteReward
        public static void getVp(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("premium").Where("EntityName", client.Entity.Name).Or("Username", client.Account.Username);
            MySqlReader rdr = new MySqlReader(cmd);
            if (rdr.Read())
            {
                client.VotePoints = rdr.ReadUInt32("VotePoint");
                rdr.Close();
            }
            else
            {
                client.VotePoints = 0;
                MySqlCommand Do = new MySqlCommand(MySqlCommandType.INSERT);
                Do.Insert("premium").Insert("EntityName", client.Entity.Name).Insert("Username", client.Account.Username).Insert("VotePoint", 0).Insert("IP", client.IP).Execute();
                rdr.Close();
            }
        }
        public static void updateVp(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("premium").Set("VotePoint", client.VotePoints).Where("Username", client.Account.Username).Execute();
        }
        #endregion

        #region VIP
        public static void getVipInfo(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("premium").Where("EntityName", client.Entity.Name);
            MySqlReader rdr = new MySqlReader(cmd);
            if (rdr.Read())
            {
                client.VIPDate = DateTime.FromBinary(rdr.ReadInt64("VipStartDate"));
                client.VIPDays = rdr.ReadUInt32("VipDays");
                rdr.Close();
            }
        }
        public static void activateVip(Client.GameState client)
        {
            if (inPremium(client))
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("premium").Set("VipDays", client.VIPDays).Set("VipStartDate", DateTime.Now.Ticks).Where("Username", client.Account.Username).Execute();
            }
        }
        #endregion
    }
}
