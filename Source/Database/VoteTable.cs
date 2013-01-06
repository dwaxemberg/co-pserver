using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Conquer_Online_Server.Database
{
    public class VoteTable
    {
        public static bool CanVote(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("votes").Where("Username", client.Account.Username);
            MySqlReader rdr = new MySqlReader(cmd);
            if (rdr.Read())
            {
                string date = rdr.ReadString("ServerDate");
                date = date.Replace("-", "/");
                if (date.StartsWith("0") || date.Length < 4)
                    date = "2011/01/01 01:01:01";

                client.LastVote = ParseString(date);

                rdr.Close();
                if (DateTime.Now >= client.LastVote.AddHours(12))
                    return true;
                else
                    return false;
            }
            rdr.Close();
            return true;
        }
        public static DateTime ParseString(string value)
        {
            try
            {
                string[] parts = value.Split(' ');
                //date
                string[] date = parts[0].Split('/');
                int year = int.Parse(date[0]);
                int month = int.Parse(date[1]);
                int day = int.Parse(date[2]);
                //time
                string[] time = parts[1].Split(':');
                int hour = int.Parse(time[0]);
                int minute = int.Parse(time[1]);
                int second = int.Parse(time[2]);

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                Console.WriteLine("ERROR: " + value);
                return DateTime.Now;
            }
        }
    }
}
