using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public static class DataHolder
    {
        private static string MySqlUsername, MySqlPassword, MySqlDatabase, MySqlHost;
        private static string ConnectionString;
        public static void CreateConnection(string user, string password, string database, string host)
        {
            MySqlUsername = user;
            MySqlHost = host;
            MySqlPassword = password;
            MySqlDatabase = database;
            ConnectionString = "Server=" + MySqlHost + ";Database='" + MySqlDatabase + "';Username='" + MySqlUsername + "';Password='" + MySqlPassword + "';Pooling=true; Max Pool Size = 30; Min Pool Size = 5";

        }

        public static MySql.Data.MySqlClient.MySqlConnection MySqlConnection
        {
            get
            {
                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = ConnectionString;
                return conn;
            }
        }

        static SafeDictionary<byte, string> ArcherStats = new SafeDictionary<byte, string>(130);
        static SafeDictionary<byte, string> NinjaStats = new SafeDictionary<byte, string>(130);
        static SafeDictionary<byte, string> WarriorStats = new SafeDictionary<byte, string>(130);
        static SafeDictionary<byte, string> TrojanStats = new SafeDictionary<byte, string>(130);
        static SafeDictionary<byte, string> TaoistStats = new SafeDictionary<byte, string>(130);
        static SafeDictionary<byte, string> MonkStats = new SafeDictionary<byte, string>(130);

        public static void ReadStats()
        {
            string Path = ServerBase.Constants.DataHolderPath + "Stats.ini";
            ServerBase.IniFile IniFile = new ServerBase.IniFile(Path);

            for (byte lvl = 1; lvl < 122; lvl++)
            {
                string job = "Archer[" + lvl + "]";
                string Data = IniFile.ReadString("Stats", job);
                try
                {
                    ArcherStats.Add(lvl, Data);
                    job = "Ninja[" + lvl + "]";
                    Data = IniFile.ReadString("Stats", job);
                    NinjaStats.Add(lvl, Data);
                    job = "Warrior[" + lvl + "]";
                    Data = IniFile.ReadString("Stats", job);
                    WarriorStats.Add(lvl, Data);
                    job = "Trojan[" + lvl + "]";
                    Data = IniFile.ReadString("Stats", job);
                    TrojanStats.Add(lvl, Data);
                    job = "Taoist[" + lvl + "]";
                    Data = IniFile.ReadString("Stats", job);
                    TaoistStats.Add(lvl, Data);
                    job = "Monk[" + lvl + "]";
                    Data = IniFile.ReadString("Stats", job);
                    MonkStats.Add(lvl, Data);
                }

                catch
                {
                    Console.WriteLine(Data);
                }
            }
        }
        public static ushort[] FindReviveSpot(ushort mapID)
        {
            ServerBase.IniFile IniFile = new ServerBase.IniFile(ServerBase.Constants.RevivePoints);
            string value = IniFile.ReadString(mapID.ToString(), "Value");
            if (value == String.Empty)
                return new ushort[] { 1002, 430, 380 };

            if (value.Contains("L"))
                value = IniFile.ReadString(value.Remove(0, 7), "Value");

            string[] split = value.Split(' ');
            List<ushort> values = new List<ushort>();
            try
            {
                values.Add(ushort.Parse(split[0]));
                values.Add(ushort.Parse(split[1]));
                values.Add(ushort.Parse(split[2]));
            }
            catch
            {
                Console.WriteLine("Revive spot with error: " + value);
                return new ushort[] { 1002, 430, 380 };
            }
            return values.ToArray();
        }
        public static void GetStats(byte inClass, byte inLevel, Client.GameState client)
        {
            string Class = "";
            inClass = (byte)((inClass / 10) * 10);
            switch (inClass)
            {
                case 10: Class = "Trojan"; break;
                case 20: Class = "Warrior"; break;
                case 40: Class = "Archer"; break;
                case 50: Class = "Ninja"; break;
                case 60: Class = "Monk"; break;
                default: Class = "Taoist"; break;
            }
            inLevel = Math.Max((byte)10, inLevel);
            inLevel = Math.Min((byte)120, inLevel);
            string[] Data = null;
            if (Class == "Trojan")
                Data = TrojanStats[inLevel].Split(',');
            else if (Class == "Warrior")
                Data = WarriorStats[inLevel].Split(',');
            else if (Class == "Archer")
                Data = ArcherStats[inLevel].Split(',');
            else if (Class == "Ninja")
                Data = NinjaStats[inLevel].Split(',');
            else if (Class == "Taoist")
                Data = TaoistStats[inLevel].Split(',');
            else if (Class == "Monk")
                Data = MonkStats[inLevel].Split(',');

            client.Entity.Strength = Convert.ToUInt16(Data[0]);
            client.Entity.Vitality = Convert.ToUInt16(Data[1]);
            client.Entity.Agility = Convert.ToUInt16(Data[2]);
            client.Entity.Spirit = Convert.ToUInt16(Data[3]);
        }

        public static ulong LevelExperience(byte Level)
        {
            return levelExperience[Math.Min(Math.Max(Level - 1, 0), 135)];
        }
        public static uint ProficiencyLevelExperience(byte Level)
        {
            return proficiencyLevelExperience[Math.Min(Level, (byte)20)];
        }

        static uint[] proficiencyLevelExperience = new uint[21] { 0, 1200, 68000, 250000, 640000, 1600000, 4000000, 10000000, 22000000, 40000000, 90000000, 95000000, 142500000, 213750000, 320625000, 480937500, 721406250, 1082109375, 1623164063, 2100000000, 0 };
        static ulong[] levelExperience = new ulong[139] { 120, 180, 240, 360, 600, 960, 1200, 2400, 3600, 8400, 12000, 14400, 18000, 21600, 22646, 32203, 37433, 47556, 56609, 68772, 70515, 75936, 97733, 114836, 120853, 123981, 126720, 145878, 173436, 197646, 202451, 212160, 244190, 285823, 305986, 312864, 324480, 366168, 433959, 460590, 506738, 569994, 728527, 850829, 916479, 935118, 940800, 1076593, 1272780, 1357994, 1384861, 1478400, 1632438, 1903104, 2066042, 2104924, 1921085, 2417202, 2853462, 3054574, 3111217, 3225600, 3810962, 4437896, 4880605, 4970962, 5107200, 5652518, 6579162, 6877991, 7100700, 7157657, 9106860, 10596398, 11220549, 11409192, 11424000, 12882952, 15172807, 15896990, 16163799, 16800000, 19230280, 22365208, 23819312, 24219528, 24864000, 27200077, 32033165, 33723801, 34291317, 34944000, 39463523, 45878567, 48924236, 49729220, 51072000, 55808379, 64870058, 68391931, 69537026, 76422968, 96950789, 112676755, 120090482, 121798280, 127680000, 137446887, 193715970, 408832150, 454674685, 461125885, 469189885, 477253885, 480479485, 485317885, 493381885, 580580046, 717424987, 282274058, 338728870, 406474644, 487769572, 585323487, 702388184, 842865821, 1011438985, 1073741823, 1073741823, 8589134588, 25767403764, 77302211292, 231906633876, 347859950814, 447859950814, 547859950814, 1174030000000, 1761040000000, 2641550000000 };

        public static uint StonePlusPoints(byte plus)
        {
            return StonePoints[Math.Min((int)plus, 8)];
        }
        public static uint ComposePlusPoints(byte plus)
        {
            return ComposePoints[Math.Min(plus, (byte)12)];
        }
        public static byte SteedSpeed(byte plus)
        {
            return _SteedSpeed[Math.Min(plus, (byte)12)];
        }
        public static ushort TalismanPlusPoints(byte plus)
        {
            return TalismanExtra[Math.Min(plus, (byte)9)];
        }

        public static ushort PurifyStabilizationPoints(byte plevel)
        {
            return purifyStabilizationPoints[Math.Min(plevel - 1, (byte)5)];
        }

        static ushort[] purifyStabilizationPoints = new ushort[6] { 10, 30, 60, 100, 150, 200 };

        public static ushort RefineryStabilizationPoints(byte elevel)
        {
            return refineryStabilizationPoints[Math.Min(elevel - 1, (byte)4)];
        }
        static ushort[] refineryStabilizationPoints = new ushort[5] { 10, 30, 70, 150, 270 };
    
        public static ushort[] Disguises = new ushort[] { 111, 224, 117, 152, 113, 833, 116, 245, 223, 112, 222, 114, 221, 115, 220 };
        static ushort[] StonePoints = new ushort[9] { 1, 10, 40, 120, 360, 1080, 3240, 9720, 29160 };
        static ushort[] ComposePoints = new ushort[13] { 20, 20, 80, 240, 720, 2160, 6480, 19440, 58320, 2700, 5500, 9000, 0 };
        static byte[] _SteedSpeed = new byte[] { 0, 5, 10, 15, 20, 30, 40, 50, 65, 85, 90, 95, 100 };
        static ushort[] TalismanExtra = new ushort[10] { 0, 6, 30, 70, 240, 740, 2240, 6670, 20000, 60000 };
    }
}
