using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class QuestTable
    {
        #region Lotto
        public static void Lotto(Client.GameState client, bool New)
        {
           
            if (New)
            {
                client.LotteryEntries = 1;
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("quest").Insert("EntityName", client.Entity.Name).Insert("IP", client.IP).Insert("LottoDate", DateTime.Now.Ticks).Insert("LottoTries", 1).Insert("Username", client.Account.Username).Execute();
            }
            else
            {
                client.LotteryEntries += 1;
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("quest").Set("LottoDate", DateTime.Now.Ticks).Set("LottoTries", client.LotteryEntries).Set("Username", client.Account.Username).Where("EntityName", client.Entity.Name).Or("IP", client.IP).Execute();
            }
        }
        public static bool canLotto(Client.GameState client, ref bool New)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("quest").Where("EntityName", client.Entity.Name).Or("IP", client.IP);
            MySqlReader rdr = new MySqlReader(cmd);
            if (rdr.Read())
            {
                New = false;
                client.LastLotteryEntry = DateTime.FromBinary(rdr.ReadInt64("LottoDate"));
                client.LotteryEntries = rdr.ReadByte("LottoTries");
                if (client.LotteryEntries <= 9)
                {
                    rdr.Close();
                    return true;
                }
                else if (DateTime.Now.DayOfYear != client.LastLotteryEntry.DayOfYear)
                {
                    client.LotteryEntries = 0;
                    MySqlCommand Do = new MySqlCommand(MySqlCommandType.UPDATE);
                    Do.Update("quest").Set("LottoDate", DateTime.Now.Ticks).Set("LottoTries", 0).Where("EntityName", client.Entity.Name).Or("IP", client.IP).Execute();
                    rdr.Close();
                    return true;
                }
                else
                {
                    rdr.Close();
                    return false;
                }
            }
            else
            {
                rdr.Close();
                New = true;
                return true;
            }
        }
        public static void ResetLotto(Client.GameState client)
        {           
            client.LotteryEntries = 0;
            MySqlCommand Do = new MySqlCommand(MySqlCommandType.UPDATE);
            Do.Update("quest").Set("LottoDate", DateTime.Now.Ticks).Set("LottoTries", 0).Where("EntityName", client.Entity.Name).Or("IP", client.IP).Execute();
        }
        #endregion
        #region PowerEXPBall
        public static void pExpBall(Client.GameState client)
        {
            MySqlCommand Do = new MySqlCommand(MySqlCommandType.UPDATE);
            Do.Update("quest").Set("pExpBallsUsed", "pExpBallsUsed"+1).Where("EntityName", client.Entity.Name).Execute();
        }
        #endregion
    }
}
