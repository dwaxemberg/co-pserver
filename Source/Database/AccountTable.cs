using System;
using System.IO;
using System.Text;
using Conquer_Online_Server.ServerBase;
using System.Collections.Generic;

namespace Conquer_Online_Server.Database
{
    public class AccountTable
    {
        public static List<string> BannedIPs = new List<string>();
        private static bool LoadedIPs = false;
        public enum AccountState : byte
        {
            NotActivated = 100,
            ProjectManager = 4,
            GameMaster = 3,
            Player = 2,
            Banned = 1,
            DoesntExist = 0
        }
        public string Username;
        public string Password;
        public string Email;
        public string IP;
        public DateTime LastCheck;
        public AccountState State;
        public uint EntityID;
        public bool exists = false;
        public AccountTable(string username)
        {
            if (BannedIPs.Count == 0 && !LoadedIPs)
            {
                string[] lines = File.ReadAllLines(ServerBase.Constants.BannedPath);
                foreach (string line in lines)
                {
                    if (line.Length >= 7)
                    {
                        BannedIPs.Add(line.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                }
                LoadedIPs = true;
            }
            this.Username = username;
            this.Password = "";
            this.IP = "";
            this.LastCheck = DateTime.Now;
            this.State = AccountState.DoesntExist;
            this.EntityID = 0;
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("accounts").Where("Username", username);
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                exists = true;
                this.Password = r.ReadString("Password");
                this.IP = r.ReadString("IP");
                this.EntityID = r.ReadUInt32("EntityID");
                this.LastCheck = DateTime.FromBinary(r.ReadInt64("LastCheck"));
                this.State = (AccountState)r.ReadByte("State");
                this.Email = r.ReadString("Email");
            }
            r.Close();
        }
        public void Save()
        {
            if (exists)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("accounts").Set("Password", Password).Set("IP", IP).Set("EntityID", EntityID).Set("LastCheck", (ulong)DateTime.Now.ToBinary()).Where("Username", Username).Execute();
            }
            else
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("accounts").Insert("Username", Username).Insert("Password", Password).Insert("State", (byte)State).Execute();
                }
                catch (Exception e) { Program.SaveException(e); }
            }
        }
        public void Save(MySql.Data.MySqlClient.MySqlConnection conn)
        {
            if (exists)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                cmd.Update("accounts").Set("Password", Password).Set("IP", IP).Set("EntityID", EntityID).Set("LastCheck", (ulong)DateTime.Now.ToBinary()).Where("Username", Username).Execute(conn);
            }
            else
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("accounts").Insert("Username", Username).Insert("Password", Password).Insert("State", (byte)State).Execute(conn);
                }
                catch (Exception e) { Program.SaveException(e); }
            }
        }
        public static void BanIP(string IP)
        {
            if (!BannedIPs.Contains(IP))
            {
                BannedIPs.Add(IP);
                File.AppendAllText(ServerBase.Constants.BannedPath, IP + "\r\n");
            }
        }
        public static void UnBanIP(string ip)
        {
            if (BannedIPs.Contains(ip))
            {
                File.Delete(ServerBase.Constants.BannedPath);
                BannedIPs.Remove(ip);
                foreach (string IP in BannedIPs)
                {
                    File.AppendAllText(ServerBase.Constants.BannedPath, IP + "\r\n");
                }
            }
        }
    }
}
