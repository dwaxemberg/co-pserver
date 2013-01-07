using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Database;
using Conquer_Online_Server.Network.Sockets;
using Conquer_Online_Server.Network.AuthPackets;
using Conquer_Online_Server.Game.ConquerStructures.Society;
using Conquer_Online_Server.Game;
using System.Collections.Generic;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Network.Features.ClassPKWar;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Game.ConquerStructures;

namespace Conquer_Online_Server
{
    class Program
    {
        public static int PlayerCap = 1500;
        public static long PoolSize = 0;
        public static AsyncSocket AuthServer;
        public static AsyncSocket GameServer;
        public static DateTime m_msgDate;
        public static bool Steed = false;
        public static bool WeeklyPk = false;
        public static bool nobilityrankwar = false;
        public static bool Kingmonster = false;
        public static bool Princesmonster = false;
        public static bool Dukemonster = false;
        //public static GUI GUI;
        public static byte m_Counter;
        public static ServerBase.Counter EntityUID;
        public static string GameIP;
        public static DayOfWeek Today;
        public static ushort GamePort;
        public static ushort AuthPort;
        public static DateTime StartDate;
        public static bool LastManStanding = false;
        public static DateTime RestartDate = DateTime.Now.AddHours(24);
        public static ServerBase.Threads StatusFlagChange = new Conquer_Online_Server.ServerBase.Threads(500);
        public static ServerBase.Threads CharacterThread = new ServerBase.Threads(1000);
        public static ServerBase.Threads AttackThread = new ServerBase.Threads(800);
        public static ServerBase.Threads CompanionThread = new Conquer_Online_Server.ServerBase.Threads(1000);
        public static ServerBase.Threads BlessThread = new ServerBase.Threads(4000);
        public static ServerBase.Threads ServerStuff = new ServerBase.Threads(500);
        public static ServerBase.Threads ArenaSystem = new Conquer_Online_Server.ServerBase.Threads(2000);
        public static Client.GameState[] Values = null;
        static void Main(string[] args)
        {
            Application.ThreadException += Application_ThreadException;
            Application.Run(new NotForPublicNotAtAll.NoCrash());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            SaveException(e.Exception);
        }

        public static void SaveException(Exception e)
        {
            if (e.TargetSite.Name == "ThrowInvalidOperationException")
                return;

            Console.WriteLine(e);

            var dt = DateTime.Now;
            string date = dt.Month + "-" + dt.Day + "//";

            if (!Directory.Exists(Application.StartupPath + ServerBase.Constants.UnhandledExceptionsPath))
                Directory.CreateDirectory(Application.StartupPath + "\\" + ServerBase.Constants.UnhandledExceptionsPath);
            if (!Directory.Exists(Application.StartupPath + "\\" + ServerBase.Constants.UnhandledExceptionsPath + date))
                Directory.CreateDirectory(Application.StartupPath + "\\" + ServerBase.Constants.UnhandledExceptionsPath + date);
            if (!Directory.Exists(Application.StartupPath + "\\" + ServerBase.Constants.UnhandledExceptionsPath + date + e.TargetSite.Name))
                Directory.CreateDirectory(Application.StartupPath + "\\" + ServerBase.Constants.UnhandledExceptionsPath + date + e.TargetSite.Name);

            string fullPath = Application.StartupPath + "\\" + ServerBase.Constants.UnhandledExceptionsPath + date + e.TargetSite.Name + "\\";

            string date2 = dt.Hour + "-" + dt.Minute;
            List<string> Lines = new List<string>();

            Lines.Add("----Exception message----");
            Lines.Add(e.Message);
            Lines.Add("----End of exception message----\r\n");

            Lines.Add("----Stack trace----");
            Lines.Add(e.StackTrace);
            Lines.Add("----End of stack trace----\r\n");

            //Lines.Add("----Data from exception----");
            //foreach (KeyValuePair<object, object> data in e.Data)
            //    Lines.Add(data.Key.ToString() + "->" + data.Value.ToString());
            //Lines.Add("----End of data from exception----\r\n");

            File.WriteAllLines(fullPath + date2 + ".txt", Lines.ToArray());
        }

        public static void StartEngine()
        {
            EngineThread_Execute();
        }
        public static int RandomSeed = 0;
        static void EngineThread_Execute()
        {
            Time32 Start = Time32.Now;
            RandomSeed = Convert.ToInt32(DateTime.Now.Ticks.ToString().Remove(DateTime.Now.Ticks.ToString().Length / 2));
            ServerBase.Kernel.Random = new Random(RandomSeed);
            StartDate = DateTime.Now;
            //Console.Title = "Conquer Online Server Emulator. Start time: " + StartDate.ToString("dd MM yyyy hh:mm"); Console.BackgroundColor = ConsoleColor.Green;
            //Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Load server configuration!");
            string ConfigFileName = "configuration.ini";
            ServerBase.IniFile IniFile = new ServerBase.IniFile(ConfigFileName);

            GameIP = IniFile.ReadString("configuration", "IP");
            GamePort = IniFile.ReadUInt16("configuration", "GamePort");
            AuthPort = IniFile.ReadUInt16("configuration", "AuthPort");
            ServerBase.Constants.ServerName = IniFile.ReadString("configuration", "ServerName");
            Database.DataHolder.CreateConnection(IniFile.ReadString("MySql", "Username"), IniFile.ReadString("MySql", "Password"), IniFile.ReadString("MySql", "Database"), IniFile.ReadString("MySql", "Host"));


            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("configuration").Where("Server", ServerBase.Constants.ServerName);
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                EntityUID = new ServerBase.Counter(r.ReadUInt32("EntityID"));
                Game.Clans.ClanCount = new Conquer_Online_Server.ServerBase.Counter(r.ReadUInt32("ClanUID"));
                Game.ConquerStructures.Society.Guild.GuildCounter = new Conquer_Online_Server.ServerBase.Counter(r.ReadUInt32("GuildID"));
                Network.GamePackets.ConquerItem.ItemUID = new Conquer_Online_Server.ServerBase.Counter(r.ReadUInt32("ItemUID"));
                ServerBase.Constants.ExtraExperienceRate = r.ReadUInt32("ExperienceRate");
                ServerBase.Constants.ExtraSpellRate = r.ReadUInt32("ProficiencyExperienceRate");
                ServerBase.Constants.ExtraProficiencyRate = r.ReadUInt32("SpellExperienceRate");
                ServerBase.Constants.MoneyDropRate = r.ReadUInt32("MoneyDropRate");
                ServerBase.Constants.MoneyDropMultiple = r.ReadUInt32("MoneyDropMultiple");
                ServerBase.Constants.ConquerPointsDropRate = r.ReadUInt32("ConquerPointsDropRate");
                ServerBase.Constants.ConquerPointsDropMultiple = r.ReadUInt32("ConquerPointsDropMultiple");
                ServerBase.Constants.ItemDropRate = r.ReadUInt32("ItemDropRate");
                ServerBase.Constants.ItemDropQualityRates = r.ReadString("ItemDropQualityString").Split('~');
                ServerBase.Constants.WebAccExt = r.ReadString("AccountWebExt");
                ServerBase.Constants.WebVoteExt = r.ReadString("VoteWebExt");
                ServerBase.Constants.WebDonateExt = r.ReadString("DonateWebExt");
                ServerBase.Constants.ServerWebsite = r.ReadString("ServerWebsite");
                ServerBase.Constants.ServerGMPass = r.ReadString("ServerGMPass");
                PlayerCap = r.ReadInt32("PlayerCap");
                Database.DetainedItemTable.Counter = new Conquer_Online_Server.ServerBase.Counter(r.ReadUInt32("DetainItemUID"));
            }
            r.Close();

            Console.WriteLine("Initializing database.");
            Database.ConquerItemInformation.Load();
            Database.DataHolder.ReadStats();
            Database.MonsterInformation.Load();
            Database.SpellTable.Load();
            Database.ShopFile.Load();
            Database.EShopFile.Load();
            Database.MapsTable.Load();
            Database.NobilityTable.Load();
            Database.ArenaTable.Load();
            Database.GuildTable.Load();
            Database.LotteryTable.Load();
            Database.DMaps.Load();
            Database.EntityTable.LoadPlayersVots();
            Database.LotteryTable2.Load();
            Database.EntityTable.NextUit();
            // Database.DROP_SOULS.LoadDrops();
            Database.Clans.LoadAllClans();
            ServerBase.FrameworkTimer.SetPole(100, 50);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ServerBase.FrameworkTimer.DoNothing));
            Values = new Client.GameState[0];
            new Game.Map(1038, Database.DMaps.MapPaths[1038]);
            Game.ConquerStructures.Society.GuildWar.Initiate();
            Console.WriteLine("Guild war initializated.");
            Network.AuthPackets.Forward.Incrementer = new ServerBase.Counter();
            Network.Cryptography.AuthCryptography.PrepareAuthCryptography();
            ServerBase.Kernel.Elite_PK_Tournament = new Conquer_Online_Server.Game.Tournaments.EliteTournament();
            ServerBase.Kernel.Elite_PK_Tournament.LoadTop8();
            Console.WriteLine("Initializing sockets.");
            AuthServer = new AsyncSocket(AuthPort);
            AuthServer.OnClientConnect += new Action<Interfaces.ISocketWrapper>(AuthServer_AnnounceNewConnection);
            AuthServer.OnClientReceive += new Action<byte[], Interfaces.ISocketWrapper>(AuthServer_AnnounceReceive);
            AuthServer.OnClientDisconnect += new Action<Interfaces.ISocketWrapper>(AuthServer_AnnounceDisconnection);
            GameServer = new AsyncSocket(GamePort);
            GameServer.OnClientConnect += new Action<Interfaces.ISocketWrapper>(GameServer_AnnounceNewConnection);
            GameServer.OnClientReceive += new Action<byte[], Interfaces.ISocketWrapper>(GameServer_AnnounceReceive);
            GameServer.OnClientDisconnect += new Action<Interfaces.ISocketWrapper>(GameServer_AnnounceDisconnection);
            Console.WriteLine("Authentication server on port " + AuthPort + "is online.");
            Console.WriteLine("Game server on port " + GamePort + "is online.");
            Console.WriteLine();
            Console.WriteLine("Server loaded in " + (Time32.Now - Start) + " milliseconds.");
            StatusFlagChange.Execute += new Action(StatusFlagChange_Execute);
            StatusFlagChange.Start();
            CharacterThread.Execute += new Action(CharacterThread_Execute);
            CharacterThread.Start();
            AttackThread.Execute += new Action(AttackThread_Execute);
            AttackThread.Start();
            CompanionThread.Execute += new Action(CompanionThread_Execute);
            CompanionThread.Start();
            BlessThread.Execute += new Action(BlessThread_Execute);
            BlessThread.Start();
            CharacterThread.Execute += new Action(CharacterThread_Execute);
            CharacterThread.Start();
            ServerStuff.Execute += new Action(ServerStuff_Execute);
            ServerStuff.Start();
            ArenaSystem.Execute += new Action(Game.ConquerStructures.Arena.ArenaSystem_Execute);
            ArenaSystem.Start();
            new MySqlCommand(MySqlCommandType.UPDATE).Update("entities").Set("Online", 0).Execute();
            GC.Collect();
            System.Threading.Thread.Sleep(000);
            Console.WriteLine("----------------Source 5620--------------");
            while (true)
                CommandsAI(Console.ReadLine());
        }

        static void AttackThread_Execute()
        {
            foreach (Client.GameState client in Values)
            {
                if (client.Socket.Connected)
                {
                    if (client.Entity.HandleTiming)
                    {
                        #region Auto attack
                        if (client.Entity.AttackPacket != null || client.Entity.VortexAttackStamp != null)
                        {
                            try
                            {
                                if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                                {
                                    if (client.Entity.VortexPacket != null && client.Entity.VortexPacket.ToArray() != null)
                                    {
                                        if (Time32.Now > client.Entity.VortexAttackStamp.AddMilliseconds(1400))
                                        {
                                            client.Entity.VortexAttackStamp = Time32.Now;
                                            new Game.Attacking.Handle(client.Entity.VortexPacket, client.Entity, null);
                                        }
                                    }
                                }
                                else
                                {
                                    client.Entity.VortexPacket = null;
                                    var AttackPacket = client.Entity.AttackPacket;
                                    if (AttackPacket != null && AttackPacket.ToArray() != null)
                                    {
                                        uint AttackType = AttackPacket.AttackType;
                                        if (AttackType == Network.GamePackets.Attack.Magic || AttackType == Network.GamePackets.Attack.Melee || AttackType == Network.GamePackets.Attack.Ranged)
                                        {
                                            if (AttackType == Network.GamePackets.Attack.Magic)
                                            {
                                                if (Time32.Now > client.Entity.AttackStamp.AddSeconds(1))
                                                {
                                                    new Game.Attacking.Handle(AttackPacket, client.Entity, null);
                                                }
                                            }
                                            else
                                            {
                                                int decrease = -300;
                                                if (client.Entity.OnCyclone())
                                                    decrease = 700;
                                                if (client.Entity.OnSuperman())
                                                    decrease = 200;
                                                if (Time32.Now > client.Entity.AttackStamp.AddMilliseconds((1000 - client.Entity.Agility - decrease) * (int)(AttackType == Network.GamePackets.Attack.Ranged ? 1 : 1)))
                                                {
                                                    new Game.Attacking.Handle(AttackPacket, client.Entity, null);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                SaveException(e);
                                client.Entity.AttackPacket = null;
                                client.Entity.VortexPacket = null;
                            }
                        }
                        #endregion
                    }
                }
                //else
                //    client.Disconnect();
            }
        }
        static DateTime LastRandomReset = DateTime.Now;
        static void ServerStuff_Execute()
        {
            ServerBase.Kernel.Elite_PK_Tournament.SendThis();
            Console.Title = "Source 5620 : " + Program.StartDate.ToString("dd MM yyyy hh:mm") + "- Players Online: " + ServerBase.Kernel.GamePool.Count + "||" + Program.PlayerCap + " Votes =" + Kernel.VotePool.Count;
            new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration")
                .Set("GuildID", Game.ConquerStructures.Society.Guild.GuildCounter.Now)
                .Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now)
                .Set("DetainItemUID", Database.DetainedItemTable.Counter.Now).Set("ClanUID", Game.Clans.ClanCount.Now)
                .Where("Server", ServerBase.Constants.ServerName).Execute();
            ServerBase.FrameworkTimer.Dispose();

            if (DateTime.Now > Game.ConquerStructures.Broadcast.LastBroadcast.AddMinutes(1))
            {
                if (Game.ConquerStructures.Broadcast.Broadcasts.Count > 0)
                {
                    Game.ConquerStructures.Broadcast.CurrentBroadcast = Game.ConquerStructures.Broadcast.Broadcasts[0];
                    Game.ConquerStructures.Broadcast.Broadcasts.Remove(Game.ConquerStructures.Broadcast.CurrentBroadcast);
                    Game.ConquerStructures.Broadcast.LastBroadcast = DateTime.Now;
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(Game.ConquerStructures.Broadcast.CurrentBroadcast.Message, "ALLUSERS", Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityName, System.Drawing.Color.Red, Network.GamePackets.Message.BroadcastMessage), ServerBase.Kernel.GamePool.Values);
                }
                else
                    Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityID = 1;
            }

            DateTime Now = DateTime.Now;
            if (DateTime.Now.Hour == 00 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 || DateTime.Now.Hour == 03 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 || DateTime.Now.Hour == 06 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 || DateTime.Now.Hour == 09 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 || DateTime.Now.Hour == 12 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 || DateTime.Now.Hour == 15 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 || DateTime.Now.Hour == 18 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 || DateTime.Now.Hour == 21 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00)
            {
                Program.CommandsAI("@save");
            }

            if (Now > LastRandomReset.AddMinutes(30))
            {
                LastRandomReset = Now;
                ServerBase.Kernel.Random = new Random(RandomSeed);
            }
            Today = Now.DayOfWeek;

            if (Now >= RestartDate)
            {
                CommandsAI("@restart");
                ServerStuff.Closed = true;
                return;
            }
            var Values = ServerBase.Kernel.WasInGamePool.Base.ToArray();
            foreach (KeyValuePair<uint, Client.GameState> vals in Values)
            {
                Client.GameState client = vals.Value;
                if (client.Disconnected2 == true)
                    return;
                if (client == null || client.Entity == null || client.Account == null)
                {
                    ServerBase.Kernel.WasInGamePool.Remove(vals.Key);
                    return;
                }
                if (!client.Socket.Connected)
                {
                    Database.EntityTable.SaveEntity(client);
                    Database.SkillTable.SaveProficiencies(client);
                    Database.SkillTable.SaveSpells(client);
                    Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic);
                    ServerBase.Kernel.WasInGamePool.Remove(vals.Key);
                    Database.EntityTable.UpdateOnlineStatus(client, false);
                }
            }
            foreach (Client.GameState client in Kernel.GamePool.Values)
            {
                client.Account.Save();
                Database.EntityTable.SaveEntity(client);
                Database.SkillTable.SaveProficiencies(client);
                Database.SkillTable.SaveSpells(client);
                Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic);
                // 
            }
            #region remove monk buff
            foreach (Client.GameState client in Kernel.GamePool.Values)
            {
                if (client.Entity.Owner.Team != null)
                {
                    foreach (Client.GameState teammate in client.Entity.Owner.Team.Teammates)
                    {
                        if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, teammate.Entity.X, teammate.Entity.Y) > 20)
                        {
                            if (client.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                            {
                                client.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                client.Entity.Statistics.CriticalStrike -= 200;
                            }
                            if (client.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                            {
                                client.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                client.Entity.Statistics.MetalResistance -= 30;
                            }
                            if (client.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                            {
                                client.Entity.Statistics.WoodResistance -= 30;
                                client.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                            }
                            if (client.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                            {
                                client.Entity.Statistics.WaterResistance -= 30;
                                client.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                            }
                            if (client.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                            {
                                client.Entity.Statistics.FireResistance -= 30;
                                client.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                            }
                            if (client.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                            {
                                client.Entity.Statistics.EarthResistance -= 30;
                                client.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                            }
                            if (client.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                            {
                                client.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                client.Entity.Statistics.Immunity -= 200;

                            }
                            client.Entity.RemoveFlag2(Update.Flags2.FendAura);
                            client.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                            client.Entity.RemoveFlag2(Update.Flags2.MetalAura);
                            client.Entity.RemoveFlag2(Update.Flags2.WoodAura);
                            client.Entity.RemoveFlag2(Update.Flags2.WaterAura);
                            client.Entity.RemoveFlag2(Update.Flags2.FireAura);
                            client.Entity.RemoveFlag2(Update.Flags2.EarthAura);
                            client.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);
                            teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                            if (teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                            {
                                teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                teammate.Entity.Statistics.CriticalStrike -= 200;
                            }
                            if (teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                            {
                                teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                teammate.Entity.Statistics.MetalResistance -= 30;
                            }
                            if (teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                            {
                                teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                teammate.Entity.Statistics.WoodResistance -= 30;
                            }
                            if (teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                            {
                                teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                teammate.Entity.Statistics.WaterResistance -= 30;
                            }
                            if (teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                            {
                                teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                teammate.Entity.Statistics.FireResistance -= 30;
                            }
                            if (teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                            {
                                teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                teammate.Entity.Statistics.EarthResistance -= 30;
                            }
                            if (teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                            {
                                teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                teammate.Entity.Statistics.Immunity -= 200;
                            }

                        }
                    }

                }
            }
            #endregion
            if (Game.ConquerStructures.Society.GuildWar.IsWar)
            {
                if (Time32.Now > Game.ConquerStructures.Society.GuildWar.ScoreSendStamp.AddSeconds(3))
                {
                    Game.ConquerStructures.Society.GuildWar.ScoreSendStamp = Time32.Now;
                    Game.ConquerStructures.Society.GuildWar.SendScores();
                }
                if (!Game.ConquerStructures.Society.GuildWar.Flame10th)
                {
                    if (Now >= Game.ConquerStructures.Society.GuildWar.StartTime.AddHours(1).AddMinutes(30))
                    {
                        Game.ConquerStructures.Society.GuildWar.Flame10th = true;
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("You can now go and light the last flame which is near the Pole in GuildWar.", System.Drawing.Color.Orange, Network.GamePackets.Message.Center), ServerBase.Kernel.GamePool.Values);
                    }
                }

            }

            else
            {
                try
                {
                    #region kill connection
                    if (Now.Second >= 0)
                    {
                        MySql.Data.MySqlClient.MySqlConnection conn = DataHolder.MySqlConnection;
                        conn.Close();
                        conn.Dispose();
                        //conn.Open();
                        KillConnections.Kill();

                    }
                    #endregion
                    #region restart lottery
                    try
                    {

                        if (Now.Hour == 00 && Now.Minute == 00)
                        {
                            foreach (Client.GameState clients2 in ServerBase.Kernel.GamePool.Values)
                            {
                                Database.PkWarEvent.QQ(clients2);
                                Database.EntityTable.ResetLottery2(clients2);
                                //CommandsAI("@restart");
                                // ServerStuff.Closed = true;

                                Conquer_Online_Server.Game.ConquerStructures.PlayersVot Vot = new Conquer_Online_Server.Game.ConquerStructures.PlayersVot();
                                Vot.AdressIp = clients2.Account.IP;
                                Vot.Uid = clients2.Entity.UID;
                                Kernel.VotePool.Remove(Vot.AdressIp);
                                Kernel.VotePoolUid.Remove(Vot.Uid);
                                Database.EntityTable.DeletVotes(Vot);
                            }
                        }
                    }
                    catch { }
                    //return;
                    #endregion
                    #region Wethard
                    //foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                    //{

                    //    //Game.Weather.CurrentWeatherBase.Send(client);
                    //    Conquer_Online_Server.Network.GamePackets.Weather weather = new Conquer_Online_Server.Network.GamePackets.Weather(true);
                    //    weather.WeatherType = Conquer_Online_Server.Network.GamePackets.Weather.Snow;
                    //    weather.Direction = 255;
                    //    weather.Appearence = 255;
                    //    weather.Intensity = 255;
                    //    weather.Send(clients);


                    //}
                    #endregion
                    #region Message Timing
                    if (DateTime.Now > m_msgDate.AddMinutes(5))
                    {
                        m_msgDate = DateTime.Now;
                        string Msg = "";
                        switch (m_Counter)
                        {
                            case 0: Msg = "GMs have [GM] or [PM] at the end of their names. Don`t trust anyone claiming to be a GM but not having [GM]/[PM] in their names !"; m_Counter++; break;
                            case 1: Msg = "Help UnknownMan at Market (212,204) to level up quickly"; m_Counter++; break;
                            case 2: Msg = "Trading of game accounts is illegal and may result in the instant banning of the accounts involved in the trading."; m_Counter++; break;
                            case 3: Msg = "you can donate and get Cps or Vip or Items call 01278771180"; m_Counter++; break;
                            case 4: Msg = "If You Need To Be Second Reborn Pirate or Reborn To Pirate You can Call PM  01278771180."; m_Counter++; break;
                            case 5: Msg = "you can call 01278771180 only PM number for more donate."; m_Counter = 0; break;
                            default: return;
                        }
                        Network.PacketHandler.WorldMessage(Msg);
                    }
                    #endregion
                    #region SteedRace New Time XX:30
                    try
                    {

                        if (Now.Minute == 30 && Now.Second == 0)
                        {
                            Program.Steed = true;
                            SteedRaces.SteedRace();
                            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                            foreach (Client.GameState clientss in client)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The SteedRace Started! You Wana Join?");
                                npc.OptionID = 239;
                                clientss.Send(npc.ToArray());
                            }
                            Console.WriteLine2("Steed Race has started. [" + Now.Hour + ":" + Now.Minute + ":" + Now.Second + "]");
                        }
                        if (Now.Minute == 40 && Now.Second == 0)
                        {
                            Program.Steed = false;
                            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                            foreach (Client.GameState clientss in client)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The SteedRace finished! come next time");
                                npc.OptionID = 255;
                                clientss.Send(npc.ToArray());
                            }
                            Console.WriteLine2("Steed Race has finished. [" + Now.Hour + ":" + Now.Minute + ":" + Now.Second + "]");
                        }
                    }
                    catch { }
                    #endregion
                    #region Steed Race Finish
                    foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                    {

                        if (Chaar.Map.ID == 1950)
                        {
                            if (Chaar.Map.ID == 1950 && (Chaar.Entity.X >= 132 && Chaar.Entity.X <= 099) && Chaar.Entity.Y == 099)
                            {
                                SteedRaces.GiveReward(Chaar);
                            }
                            else if (Chaar.Map.ID == 1950 && (Chaar.Entity.X >= 138 && Chaar.Entity.X <= 099) && Chaar.Entity.Y == 100)
                            {
                                SteedRaces.GiveReward(Chaar);
                            }
                            else if (Chaar.Map.ID == 1950 && (Chaar.Entity.X >= 135 && Chaar.Entity.X <= 100) && Chaar.Entity.Y == 100)
                            {
                                SteedRaces.GiveReward(Chaar);
                            }
                            else if (Chaar.Map.ID == 1950 && (Chaar.Entity.X >= 130 && Chaar.Entity.X <= 138) && (Chaar.Entity.Y >= 100 && Chaar.Entity.Y <= 120))
                            {
                                SteedRaces.GiveReward(Chaar);
                            }


                        }
                    }
                    #endregion
                    try
                    {
                        foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                        {
                            if (clients.Entity.ContainsFlag(Update.Flags.Ride) && clients.Entity.MapID == 1950)
                            {
                                return;

                            }
                            else
                            {
                                if (clients.Entity.MapID == 1950)
                                {
                                    clients.Entity.Teleport(1002, 431, 379);
                                }
                                //clients.Entity.srjoin = 0;
                            }
                        }
                    }
                    catch { }
                    #region elitepkp
                    try
                    {
                        foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                        {
                            if (Now.DayOfWeek == DayOfWeek.Monday)
                            {

                                if (Now.Hour == 21 && Now.Minute == 00)
                                {
                                    ServerBase.Kernel.Elite_PK_Tournament.Open();
                                }

                            }
                        }
                    }
                    catch { }
                    #endregion
                    #region GuildWar start Sunday

                    if (Now.DayOfWeek == DayOfWeek.Friday) //Ninja
                    {
                        if (Now.Hour == 12 && Now.Minute == 00)
                        {
                            Database.PkWarEvent.ResetTopDL();
                            Database.PkWarEvent.ResetTopGL();
                        }
                        if (Now.Hour == 18 && Now.Minute == 00)
                        {
                            Game.ConquerStructures.Society.GuildWar.Start();
                            foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                            {
                                clients.Send(new NpcReply(6, "Guild War is about to begin! Will you join it?"));
                            }
                        }

                    }
                    if (Now.DayOfWeek == DayOfWeek.Saturday) //Ninja
                    {
                        if (Now.Hour == 21 && Now.Minute == 00)
                        {
                            Game.ConquerStructures.Society.GuildWar.End();
                        }
                    }



                    #endregion
                    #region Weekly Monday and Thursday
                    try
                    {

                        if (Now.DayOfWeek == DayOfWeek.Monday && Now.Hour == 5 && Now.Minute == 00 && Now.Second == 0 || Now.DayOfWeek == DayOfWeek.Thursday && Now.Hour == 5 && Now.Minute == 00 && Now.Second == 0)
                        {
                            Program.WeeklyPk = true;

                            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                            foreach (Client.GameState clientss in client)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The WeeklyPk Started! You Wana Join?");
                                npc.OptionID = 238;
                                clientss.Send(npc.ToArray());
                            }
                            Console.WriteLine2("WeeklyPk has started. [" + Now.Hour + ":" + Now.Minute + ":" + Now.Second + "]");
                        }
                        if (Now.DayOfWeek == DayOfWeek.Monday && Now.Hour == 5 && Now.Minute == 03 && Now.Second == 0 || Now.DayOfWeek == DayOfWeek.Thursday && Now.Hour == 5 && Now.Minute == 03 && Now.Second == 0)
                        {
                            Program.WeeklyPk = false;
                            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                            foreach (Client.GameState clientss in client)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The WeeklyPk finished! come next time");
                                npc.OptionID = 255;
                                clientss.Send(npc.ToArray());
                            }
                            Console.WriteLine2("WeeklyPk Race has finished. [" + Now.Hour + ":" + Now.Minute + ":" + Now.Second + "]");
                        }
                    }
                    catch { }
                    #endregion
                    //#region PkWarEvent Class pk Times
                    //if (Now.DayOfWeek == DayOfWeek.Saturday) //Ninja
                    //{
                    //    if (Now.Hour == 19 && Now.Second == 40)
                    //    {
                    //        Database.PkWarEvent.ResetTopTrojan();
                    //    }
                    //    if (Now.Hour == 14 && Now.Minute == 00 && Now.Second == 00 || Now.Hour == 22 && Now.Minute == 00 && Now.Second == 00)
                    //    {
                    //        PKTournament.StartTournamentNinja();
                    //        Game.ConquerStructures.Society.GuildWar.ClassPkWarNinja();
                    //        ServerBase.Kernel.PK = true;
                    //    }

                    //}
                    //if (Now.DayOfWeek == DayOfWeek.Sunday) //MOnk
                    //{
                    //    if (Now.Hour == 19 && Now.Second == 25)
                    //    {
                    //        Database.PkWarEvent.ResetTopMonk();
                    //    }
                    //    if (Now.Hour == 14 && Now.Minute == 00 && Now.Second == 00 || Now.Hour == 22 && Now.Minute == 00 && Now.Second == 00)
                    //    {
                    //        PKTournament.StartTournamentMonk();
                    //        ServerBase.Kernel.PK = true;
                    //        Game.ConquerStructures.Society.GuildWar.ClassPkWarMonk();
                    //    }

                    //}
                    //if (Now.DayOfWeek == DayOfWeek.Monday)// trojan
                    //{
                    //    if (Now.Hour == 19 && Now.Second == 40)
                    //    {
                    //        Database.PkWarEvent.ResetTopTrojan();
                    //    }
                    //    if (Now.Hour == 14 && Now.Minute == 00 && Now.Second == 00 || Now.Hour == 22 && Now.Minute == 00 && Now.Second == 00)
                    //    {
                    //        PKTournament.StartTournamentTroJan();
                    //        ServerBase.Kernel.PK = true;
                    //        Game.ConquerStructures.Society.GuildWar.ClassPkWarTrojan();
                    //    }
                    //}
                    //if (Now.DayOfWeek == DayOfWeek.Tuesday) // worrior
                    //{
                    //    if (Now.Hour == 19 && Now.Second == 40)
                    //    {
                    //        Database.PkWarEvent.ResetTopWarrior();
                    //    }
                    //    if (Now.Hour == 14 && Now.Minute == 00 && Now.Second == 00 || Now.Hour == 22 && Now.Minute == 00 && Now.Second == 00)
                    //    {
                    //        PKTournament.StartTournamentWarrior();
                    //        ServerBase.Kernel.PK = true;
                    //        Game.ConquerStructures.Society.GuildWar.ClassPkWarWarrior();
                    //    }
                    //}
                    //if (Now.DayOfWeek == DayOfWeek.Wednesday) // archer
                    //{
                    //    if (Now.Hour == 19 && Now.Second == 40)
                    //    {
                    //        Database.PkWarEvent.ResetTopArcher();
                    //    }
                    //    if (Now.Hour == 14 && Now.Minute == 00 && Now.Second == 00 || Now.Hour == 22 && Now.Minute == 00 && Now.Second == 00)
                    //    {
                    //        PKTournament.StartTournamentArcher();
                    //        ServerBase.Kernel.PK = true;
                    //        Game.ConquerStructures.Society.GuildWar.ClassPkWarArcher();
                    //    }
                    //}
                    //if (Now.DayOfWeek == DayOfWeek.Thursday) // water
                    //{
                    //    if (Now.Hour == 19 && Now.Second == 40)
                    //    {
                    //        Database.PkWarEvent.ResetTopWater();

                    //    }
                    //    if (Now.Hour == 14 && Now.Minute == 00 && Now.Second == 00 || Now.Hour == 22 && Now.Minute == 00 && Now.Second == 00)
                    //    {
                    //        PKTournament.StartTournamentWater();
                    //        ServerBase.Kernel.PK = true;
                    //        Game.ConquerStructures.Society.GuildWar.ClassPkWarWater();
                    //    }
                    //}
                    //if (Now.DayOfWeek == DayOfWeek.Friday) // fire
                    //{
                    //    if (Now.Hour == 19 && Now.Second == 40)
                    //    {
                    //        Database.PkWarEvent.ResetTopFire();
                    //    }
                    //    if (Now.Hour == 14 && Now.Minute == 00 && Now.Second == 00 || Now.Hour == 22 && Now.Minute == 00 && Now.Second == 00)
                    //    {
                    //        PKTournament.StartTournamentFire();
                    //        ServerBase.Kernel.PK = true;
                    //        Game.ConquerStructures.Society.GuildWar.ClassPkWarFire();
                    //    }

                    //}
                    //#endregion
                    //if (Game.Features.TeamWar.War.Running)
                    //{
                    //    if (Time32.Now > Game.Features.TeamWar.War.LastCheck.AddMinutes(1))
                    //        Game.Features.TeamWar.War.Check();
                    //    if (Game.Features.TeamWar.War.Teams.Count > 0)
                    //    {
                    //        foreach (Game.Features.TeamWar.TeamStruct Teams in Game.Features.TeamWar.War.Teams.Values)
                    //        {
                    //            Teams.Check();
                    //        }
                    //    }
                    //}
                }
                catch { }

            }

        }
        public static void loadEquip()
        {
            foreach (Client.GameState client in Kernel.GamePool.Values)
            {
                client.Account.Save();
                Database.EntityTable.SaveEntity(client);
                Database.SkillTable.SaveProficiencies(client);
                Database.SkillTable.SaveSpells(client);
                Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic);
                EntityEquipment equips = new EntityEquipment(true);
                equips.ParseHero(client);
                client.Send(equips);
                // 
            }
        }
        static void CharacterThread_Execute()
        {
            Time32 Now = Time32.Now;
            foreach (Client.GameState client in Values)
            {
                if (client.Socket.Connected)
                {
                    if (client.Entity.HandleTiming)
                    {
                        #region Auto Invite Quests

                        #region Donation War
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour == 18 && DateTime.Now.Minute == 47 && DateTime.Now.Second == 00)
                        {
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour == 18 && DateTime.Now.Minute == 47 && DateTime.Now.Second == 00)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "Donation War has Started! You Wana Join?");
                                npc.OptionID = 167;
                                client.Send(npc.ToArray());
                            }
                        }
                        #endregion

                        #region GuildWar Timer
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Hour >= 13 && DateTime.Now.Minute >= 02 && DateTime.Now.Second == 00)
                        {
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Hour >= 13 && DateTime.Now.Minute >= 02 && DateTime.Now.Second == 00)
                            {
                                if (!Game.ConquerStructures.Society.GuildWar.IsWar)
                                {
                                    Game.ConquerStructures.Society.GuildWar.Start();
                                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "GuildWar has Begin! You Wana Join?");
                                    npc.OptionID = 115;
                                    client.Send(npc.ToArray());
                                }

                                //ServerBase.Kernel.SendWorldMessage(new Message("", System.Drawing.Color.Red, Network.GamePackets.Message.World), ServerBase.Kernel.GamePool.Values);
                            }
                        }
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour == 22 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00)
                        {
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour == 22 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00)
                            {
                                if (Game.ConquerStructures.Society.GuildWar.IsWar)
                                {
                                    Game.ConquerStructures.Society.GuildWar.End();
                                }
                            }
                        }
                        #endregion

                        #region HoursPkWar GameHackerPM
                        if (DateTime.Now.Minute == 45 && DateTime.Now.Second == 00)
                        {
                            if (DateTime.Now.Minute == 45 && DateTime.Now.Second == 00)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "HoursPK War has Started! You Wana Join?");
                                npc.OptionID = 202;
                                client.Send(npc.ToArray());
                            }
                        }
                        #endregion

                        #region WeaklyPK war



                        #endregion

                        #endregion
                        #region Training points
                        if (client.Entity.HeavenBlessing > 0 && !client.Entity.Dead)
                        {
                            if (Now > client.LastTrainingPointsUp.AddMinutes(10))
                            {
                                client.OnlineTrainingPoints += 10;
                                if (client.OnlineTrainingPoints >= 30)
                                {
                                    client.OnlineTrainingPoints -= 30;
                                    client.IncreaseExperience(client.ExpBall / 100, false);
                                }
                                client.LastTrainingPointsUp = Now;
                                client.Entity.Update(Network.GamePackets.Update.OnlineTraining, client.OnlineTrainingPoints, false);
                            }
                        }
                        #endregion
                        #region Minning
                        if (client.Mining && !client.Entity.Dead)
                        {
                            if (Now >= client.MiningStamp.AddSeconds(2))
                            {
                                client.MiningStamp = Now;
                                Game.ConquerStructures.Mining.Mine(client);
                            }
                        }
                        #endregion
                        #region MentorPrizeSave
                        if (Now > client.LastMentorSave.AddSeconds(5))
                        {
                            Database.KnownPersons.SaveApprenticeInfo(client.AsApprentice);
                            client.LastMentorSave = Now;
                        }
                        #endregion
                        #region Attackable
                        if (client.JustLoggedOn)
                        {
                            client.JustLoggedOn = false;
                            client.ReviveStamp = Now;
                        }
                        if (!client.Attackable)
                        {
                            if (Now > client.ReviveStamp.AddSeconds(5))
                            {
                                client.Attackable = true;
                            }
                        }
                        #endregion
                        #region DoubleExperience
                        if (client.Entity.DoubleExperienceTime > 0)
                        {
                            if (Now > client.Entity.DoubleExpStamp.AddMilliseconds(1000))
                            {
                                client.Entity.DoubleExpStamp = Now;
                                client.Entity.DoubleExperienceTime--;
                            }
                        }
                        #endregion
                        #region HeavenBlessing
                        if (client.Entity.HeavenBlessing > 0)
                        {
                            if (Now > client.Entity.HeavenBlessingStamp.AddMilliseconds(1000))
                            {
                                client.Entity.HeavenBlessingStamp = Now;
                                client.Entity.HeavenBlessing--;
                            }
                        }
                        #endregion
                        #region Enlightment
                        if (client.Entity.EnlightmentTime > 0)
                        {
                            if (Now >= client.Entity.EnlightmentStamp.AddMinutes(1))
                            {
                                client.Entity.EnlightmentStamp = Now;
                                client.Entity.EnlightmentTime--;
                                if (client.Entity.EnlightmentTime % 10 == 0 && client.Entity.EnlightmentTime > 0)
                                    client.IncreaseExperience(Game.Attacking.Calculate.Percent((int)client.ExpBall, .10F), false);
                            }
                        }
                        #endregion
                        #region PKPoints
                        if (Now >= client.Entity.PKPointDecreaseStamp.AddMinutes(5))
                        {
                            client.Entity.PKPointDecreaseStamp = Now;
                            if (client.Entity.PKPoints > 0)
                            {
                                client.Entity.PKPoints--;
                            }
                            else
                                client.Entity.PKPoints = 0;
                        }
                        #endregion
                        #region OverHP
                        if (client.Entity.FullyLoaded)
                        {
                            if (client.Entity.Hitpoints > client.Entity.MaxHitpoints && client.Entity.MaxHitpoints > 1 && !client.Entity.Transformed)
                            {
                                client.Entity.Hitpoints = client.Entity.MaxHitpoints;
                            }
                        }
                        #endregion
                        #region Stamina
                        if (Now > client.Entity.StaminaStamp.AddMilliseconds(500))
                        {
                            if (client.Entity.Vigor < client.Entity.MaxVigor)
                            {
                                client.Entity.Vigor += (ushort)(3 + (client.Entity.Action == Game.Enums.ConquerAction.Sit ? 2 : 0));

                                {
                                    Network.GamePackets.Vigor vigor = new Network.GamePackets.Vigor(true);
                                    vigor.VigorValue = client.Entity.Vigor;
                                    vigor.Send(client);
                                }
                            }
                            if (!client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Ride) && !client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                            {
                                int limit = 0;
                                if (client.Entity.HeavenBlessing > 0)
                                    limit = 50;
                                if (client.Entity.Stamina != 100 + limit)
                                {
                                    if (client.Entity.Action == Game.Enums.ConquerAction.Sit)
                                    {
                                        if (client.Entity.Stamina <= 93 + limit)
                                        {
                                            client.Entity.Stamina += 7;
                                        }
                                        else
                                        {
                                            if (client.Entity.Stamina != 100 + limit)
                                                client.Entity.Stamina = (byte)(100 + limit);
                                        }
                                    }
                                    else
                                    {
                                        if (client.Entity.Stamina <= 97 + limit)
                                        {
                                            client.Entity.Stamina += 3;
                                        }
                                        else
                                        {
                                            if (client.Entity.Stamina != 100 + limit)
                                                client.Entity.Stamina = (byte)(100 + limit);
                                        }
                                    }
                                }
                                client.Entity.StaminaStamp = Now;
                            }
                        }
                        #endregion
                        #region SoulShackle
                        if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.SoulShackle))
                        {
                            if (Now > client.Entity.ShackleStamp.AddSeconds(90))
                            {
                                client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.SoulShackle);
                                client.Entity.RemoveFlag2(18014398509481984);
                            }
                        }
                        #endregion
                        #region AzureShield
                        if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.AzureShield))
                        {
                            if (Now > client.Entity.MagicShieldStamp.AddSeconds(client.Entity.MagicShieldTime))
                            {
                                client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.AzureShield);
                            }
                        }
                        #endregion
                    }
                }
                //else
                //    client.Disconnect();
            }
        }

        static void CompanionThread_Execute()
        {
            Time32 Now = Time32.Now;
            foreach (Client.GameState client in Values)
            {
                if (client.Companion != null)
                {
                    short distance = ServerBase.Kernel.GetDistance(client.Companion.X, client.Companion.Y, client.Entity.X, client.Entity.Y);
                    if (distance >= 8)
                    {
                        ushort X = (ushort)(client.Entity.X + ServerBase.Kernel.Random.Next(2));
                        ushort Y = (ushort)(client.Entity.Y + ServerBase.Kernel.Random.Next(2));
                        if (!client.Map.SelectCoordonates(ref X, ref Y))
                        {
                            X = client.Entity.X;
                            Y = client.Entity.Y;
                        }
                        client.Companion.X = X;
                        client.Companion.Y = Y;
                        Network.GamePackets.Data data = new Conquer_Online_Server.Network.GamePackets.Data(true);
                        data.ID = Network.GamePackets.Data.Jump;
                        data.dwParam = (uint)((Y << 16) | X);
                        data.wParam1 = X;
                        data.wParam2 = Y;
                        data.UID = client.Companion.UID;
                        client.Companion.MonsterInfo.SendScreen(data);
                    }
                    else if (distance > 4)
                    {
                        Enums.ConquerAngle facing = ServerBase.Kernel.GetAngle(client.Companion.X, client.Companion.Y, client.Companion.Owner.Entity.X, client.Companion.Owner.Entity.Y);
                        if (!client.Companion.Move(facing))
                        {
                            facing = (Enums.ConquerAngle)ServerBase.Kernel.Random.Next(7);
                            if (client.Companion.Move(facing))
                            {
                                client.Companion.Facing = facing;
                                Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                                move.Direction = facing;
                                move.UID = client.Companion.UID;
                                move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                                client.Companion.MonsterInfo.SendScreen(move);
                            }
                        }
                        else
                        {
                            client.Companion.Facing = facing;
                            Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                            move.Direction = facing;
                            move.UID = client.Companion.UID;
                            move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                            client.Companion.MonsterInfo.SendScreen(move);
                        }
                    }
                    else
                    {
                        var monster = client.Companion;
                        if (monster.MonsterInfo.InSight == 0)
                        {
                            if (client.Entity.AttackPacket != null)
                            {
                                if (client.Entity.AttackPacket.AttackType == Network.GamePackets.Attack.Magic)
                                {
                                    if (client.Entity.AttackPacket.Decoded)
                                    {
                                        if (SpellTable.SpellInformations.ContainsKey((ushort)client.Entity.AttackPacket.Damage))
                                        {
                                            var info = Database.SpellTable.SpellInformations[(ushort)client.Entity.AttackPacket.Damage].Values.ToArray()[client.Spells[(ushort)client.Entity.AttackPacket.Damage].Level];
                                            if (info.CanKill)
                                            {
                                                monster.MonsterInfo.InSight = client.Entity.AttackPacket.Attacked;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    monster.MonsterInfo.InSight = client.Entity.AttackPacket.Attacked;
                                }
                            }
                        }
                        else
                        {
                            if (monster.MonsterInfo.InSight > 400000 && monster.MonsterInfo.InSight < 600000 || monster.MonsterInfo.InSight > 800000 && monster.MonsterInfo.InSight != monster.UID)
                            {
                                Entity attacked = null;

                                if (client.Screen.TryGetValue(monster.MonsterInfo.InSight, out attacked))
                                {
                                    if (Now > monster.AttackStamp.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                    {
                                        monster.AttackStamp = Now;
                                        if (attacked.Dead)
                                        {
                                            monster.MonsterInfo.InSight = 0;
                                        }
                                        else
                                            new Game.Attacking.Handle(null, monster, attacked);
                                    }
                                }
                                else
                                    monster.MonsterInfo.InSight = 0;
                            }
                        }
                    }
                }
            }
        }


        static void BlessThread_Execute()
        {
            lock (Values)
                Values = ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState client in Values)
            {
                if (client.Screen == null || client.Entity == null)
                {
                    client.Disconnect();
                }
                if (client.Socket.Connected)
                {
                    if (client.Entity.Reborn > 1)
                        continue;
                    if (!client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Praying))
                    {
                        foreach (Interfaces.IMapObject ClientObj in client.Screen.Objects)
                        {
                            if (ClientObj != null)
                            {
                                if (ClientObj is Game.Entity)
                                {
                                    if (ClientObj.MapObjType == Conquer_Online_Server.Game.MapObjectType.Player)
                                    {
                                        var Client = ClientObj.Owner;
                                        if (Client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.CastPray))
                                        {
                                            if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, ClientObj.X, ClientObj.Y) <= 3)
                                            {
                                                client.Entity.AddFlag(Network.GamePackets.Update.Flags.Praying);
                                                client.PrayLead = Client;
                                                client.Entity.Action = Client.Entity.Action;
                                                Client.Prayers.Add(client);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (client.PrayLead != null)
                        {
                            if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, client.PrayLead.Entity.X, client.PrayLead.Entity.Y) > 4)
                            {
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Praying);
                                client.PrayLead.Prayers.Remove(client);
                                client.PrayLead = null;
                            }
                        }
                    }
                }
            }
        }

        static void StatusFlagChange_Execute()
        {
            Time32 Now = Time32.Now;
            foreach (Client.GameState client in Values)
            {
                if (client.Socket.Connected && client.Entity != null)
                {
                    #region Bless
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.CastPray))
                    {
                        if (client.BlessTime <= 345000)
                            client.BlessTime += 1500;
                        else
                            client.BlessTime = 360000;
                    }
                    else if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Praying))
                    {
                        if (client.PrayLead != null)
                        {
                            if (client.PrayLead.Socket.Connected)
                            {
                                if (client.BlessTime <= 355000)
                                    client.BlessTime += 500;
                                else
                                    client.BlessTime = 3600;
                            }
                            else
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Praying);
                        }
                        else
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Praying);
                    }
                    else
                    {
                        if (client.BlessTime > 0)
                        {
                            if (client.BlessTime >= 500)
                            {
                                client.BlessTime -= 500;
                                client.Entity.Update(Network.GamePackets.Update.LuckyTimeTimer, client.BlessTime, false);
                            }
                            else
                            {
                                client.BlessTime = 0;
                                client.Entity.Update(Network.GamePackets.Update.LuckyTimeTimer, client.BlessTime, false);
                            }
                        }
                    }

                    #endregion
                    #region Flashing name
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.FlashingName))
                    {
                        if (Now > client.Entity.FlashingNameStamp.AddSeconds(client.Entity.FlashingNameTime))
                        {
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
                        }
                    }
                    #endregion
                    #region XPList
                    if (!client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.XPList))
                    {
                        if (Now > client.XPCountStamp.AddSeconds(3))
                        {
                            #region Arrows
                            if (client.Equipment != null)
                            {
                                if (!client.Equipment.Free(5))
                                {
                                    if (Network.PacketHandler.IsArrow(client.Equipment.TryGetItem(5).ID))
                                    {
                                        Database.ConquerItemTable.UpdateDurabilityItem(client.Equipment.TryGetItem(5));
                                    }
                                }
                            }
                            #endregion
                            client.XPCountStamp = Now;
                            client.XPCount++;
                            if (client.XPCount >= 100)
                            {
                                client.Entity.AddFlag(Network.GamePackets.Update.Flags.XPList);
                                client.XPCount = 0;
                                client.XPListStamp = Now;
                            }
                        }
                    }
                    else
                    {
                        if (Now > client.XPListStamp.AddSeconds(20))
                        {
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.XPList);
                        }
                    }
                    #endregion
                    #region KOSpell
                    if (client.Entity.OnKOSpell())
                    {
                        if (client.Entity.OnCyclone())
                        {
                            int Seconds = Now.AllSeconds() - client.Entity.CycloneStamp.AddSeconds(client.Entity.CycloneTime).AllSeconds();
                            if (Seconds >= 1)
                            {
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Cyclone);
                            }
                        }
                        if (client.Entity.OnSuperman())
                        {
                            int Seconds = Now.AllSeconds() - client.Entity.SupermanStamp.AddSeconds(client.Entity.SupermanTime).AllSeconds();
                            if (Seconds >= 1)
                            {
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Superman);
                            }
                        }
                        if (!client.Entity.OnKOSpell())
                        {
                            //Record KO
                            client.Entity.KOCount = 0;
                        }
                    }
                    #endregion
                    #region Buffers
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                    {
                        if (Now >= client.Entity.StigmaStamp.AddSeconds(client.Entity.StigmaTime))
                        {
                            client.Entity.StigmaTime = 0;
                            client.Entity.StigmaIncrease = 0;
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Stigma);
                        }
                    }
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Dodge))
                    {
                        if (Now >= client.Entity.DodgeStamp.AddSeconds(client.Entity.DodgeTime))
                        {
                            client.Entity.DodgeTime = 0;
                            client.Entity.DodgeIncrease = 0;
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Dodge);
                        }
                    }
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Invisibility))
                    {
                        if (Now >= client.Entity.InvisibilityStamp.AddSeconds(client.Entity.InvisibilityTime))
                        {
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Invisibility);
                        }
                    }
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.StarOfAccuracy))
                    {
                        if (client.Entity.StarOfAccuracyTime != 0)
                        {
                            if (Now >= client.Entity.StarOfAccuracyStamp.AddSeconds(client.Entity.StarOfAccuracyTime))
                            {
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.StarOfAccuracy);
                            }
                        }
                        else
                        {
                            if (Now >= client.Entity.AccuracyStamp.AddSeconds(client.Entity.AccuracyTime))
                            {
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.StarOfAccuracy);
                            }
                        }
                    }
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                    {
                        if (client.Entity.MagicShieldTime != 0)
                        {
                            if (Now >= client.Entity.MagicShieldStamp.AddSeconds(client.Entity.MagicShieldTime))
                            {
                                client.Entity.MagicShieldIncrease = 0;
                                client.Entity.MagicShieldTime = 0;
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.MagicShield);
                            }
                        }
                        else
                        {
                            if (Now >= client.Entity.ShieldStamp.AddSeconds(client.Entity.ShieldTime))
                            {
                                client.Entity.ShieldIncrease = 0;
                                client.Entity.ShieldTime = 0;
                                client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.MagicShield);
                            }
                        }
                    }
                    #endregion
                    #region Fly
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                    {
                        if (Now >= client.Entity.FlyStamp.AddSeconds(client.Entity.FlyTime))
                        {
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                            client.Entity.FlyTime = 0;
                        }
                    }
                    #endregion
                    #region PoisonStar
                    if (client.Entity.NoDrugsTime > 0)
                    {
                        if (Now > client.Entity.NoDrugsStamp.AddSeconds(client.Entity.NoDrugsTime))
                        {
                            client.Entity.NoDrugsTime = 0;
                        }
                    }
                    #endregion
                    #region ToxicFog
                    if (client.Entity.ToxicFogLeft > 0)
                    {
                        if (Now >= client.Entity.ToxicFogStamp.AddSeconds(2))
                        {
                            float Percent = client.Entity.ToxicFogPercent;
                            //Remove this line if you want it normal
                            Percent = Math.Min(0.1F, client.Entity.ToxicFogPercent);
                            client.Entity.ToxicFogLeft--;
                            client.Entity.ToxicFogStamp = Now;
                            if (client.Entity.Hitpoints > 1)
                            {
                                uint damage = Game.Attacking.Calculate.Percent(client.Entity, Percent);
                                client.Entity.Hitpoints -= damage;
                                Network.GamePackets.SpellUse suse = new Conquer_Online_Server.Network.GamePackets.SpellUse(true);
                                suse.Attacker = client.Entity.UID;
                                suse.SpellID = 10010;
                                suse.Targets.Add(client.Entity.UID, damage);
                                client.SendScreen(suse, true);
                                if (client.QualifierGroup != null)
                                    client.QualifierGroup.UpdateDamage(ServerBase.Kernel.GamePool[client.ArenaStatistic.PlayWith], damage);
                            }
                        }
                    }
                    #endregion
                    #region FatalStrike
                    if (client.Entity.OnFatalStrike())
                    {
                        if (Now > client.Entity.FatalStrikeStamp.AddSeconds(client.Entity.FatalStrikeTime))
                        {
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.CannonBarrage);
                        }
                    }
                    #endregion
                    if (client.Entity.OnOblivion())
                    {
                        if (Now > client.Entity.OblivionStamp.AddSeconds(client.Entity.OblivionTime))
                        {
                            client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.Oblivion);
                        }
                    }
                    #region ShurikenVortex
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                    {
                        if (Now > client.Entity.ShurikenVortexStamp.AddSeconds(client.Entity.ShurikenVortexTime))
                        {
                            client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.ShurikenVortex);
                        }
                    }
                    #endregion
                    #region Transformations
                    if (client.Entity.Transformed)
                    {
                        if (Now > client.Entity.TransformationStamp.AddSeconds(client.Entity.TransformationTime))
                        {
                            client.Entity.Untransform();
                        }
                    }
                    #endregion
                }
                //else
                //    client.Disconnect();
            }
        }
        public static Dictionary<uint, Client.GameState> SafeReturn()
        {
            Dictionary<uint, Client.GameState> toReturn = new Dictionary<uint, Client.GameState>();

            try
            {
                lock (Kernel.GamePool)
                {
                    foreach (Client.GameState c in Kernel.GamePool.Values)
                        if (c != null)
                            if (c.Entity != null)
                                toReturn.SafeAdd(c.Entity.UID, c);
                }
            }
            catch { Console.WriteLine("Error at safe return"); }

            return toReturn;
        }
        public static void CommandsAI(string command)
        {
            if (command == null)
                return;
            string[] data = command.Split(' ');
            switch (data[0])
            {
                case "@alivetime":
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan t2 = new TimeSpan(StartDate.ToBinary());
                        TimeSpan t1 = new TimeSpan(now.ToBinary());
                        Console.WriteLine("The server has been online " + (int)(t1.TotalHours - t2.TotalHours) + " hours, " + (int)((t1.TotalMinutes - t2.TotalMinutes) % 60) + " minutes.");
                        break;
                    }
                case "@online":
                    {
                        Console.WriteLine("Online players count: " + ServerBase.Kernel.GamePool.Count);
                        string line = "";
                        foreach (Client.GameState pClient in ServerBase.Kernel.GamePool.Values)
                            line += pClient.Entity.Name + ",";
                        if (line != "")
                        {
                            line = line.Remove(line.Length - 1);
                            Console.WriteLine("Players: " + line);
                        }
                        break;
                    }
                case "@endelite":
                    {
                        ServerBase.Kernel.Elite_PK_Tournament.Start = false;
                        ServerBase.Kernel.Elite_PK_Tournament.Finish();
                        break;
                    }
                case "@startelite":
                    {
                        ServerBase.Kernel.Elite_PK_Tournament.Open();
                        break;
                    }
                case "@memoryusage":
                    {
                        var proc = System.Diagnostics.Process.GetCurrentProcess();
                        Console.WriteLine("Thread count: " + proc.Threads.Count);
                        Console.WriteLine("Memory set(MB): " + ((double)((double)proc.WorkingSet64 / 1024)) / 1024);
                        proc.Close();
                        break;
                    }
                case "@loadvote":
                    {
                        Database.EntityTable.LoadPlayersVots();
                        break;
                    }
                case "@loadlottery":
                    {
                        Database.EntityTable.LoadPlayersVots();
                        ServerBase.FrameworkTimer.ShowThreadStats();
                        break;
                    }
                case "@save":
                    {
                        foreach (Client.GameState client in ServerBase.Kernel.GamePool.Values)
                        {
                            client.Account.Save();
                            Database.EntityTable.SaveEntity(client);
                            Database.SkillTable.SaveProficiencies(client);
                            Database.SkillTable.SaveSpells(client);
                            Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic);
                        }
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", ServerBase.Constants.ServerName).Execute();
                        break;
                    } 
                case "@playercap":
                    {
                        try
                        {
                            PlayerCap = int.Parse(data[1]);
                        }
                        catch
                        {

                        }
                        break;
                    }
                case "@exit":
                    {
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", ServerBase.Constants.ServerName).Execute();

                        GameServer.Disable();
                        AuthServer.Disable();

                        var WC = ServerBase.Kernel.GamePool.Values.ToArray();
                        foreach (Client.GameState client in WC)
                            client.Disconnect();

                        if (GuildWar.IsWar)
                            GuildWar.End();
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", ServerBase.Constants.ServerName).Execute();

                        Environment.Exit(0);
                    }
                    break;
                case "@restart":
                    {
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", ServerBase.Constants.ServerName).Execute();

                        GameServer.Disable();
                        AuthServer.Disable();

                        var WC = ServerBase.Kernel.GamePool.Values.ToArray();
                        foreach (Client.GameState client in WC)
                            client.Disconnect();

                        if (GuildWar.IsWar)
                            GuildWar.End();
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", ServerBase.Constants.ServerName).Execute();

                        Application.Restart();
                        Environment.Exit(0);
                    }
                    break;
                case "@account":
                    {
                        Database.AccountTable account = new AccountTable(data[1]);
                        account.Password = data[2];
                        account.State = AccountTable.AccountState.Player;
                        account.Save();
                    }
                    break;
            }
        }

        static void GameServer_AnnounceNewConnection(Interfaces.ISocketWrapper obj)
        {
            obj.Connector = new Client.GameState(obj.Socket);
            Client.GameState Client = obj.Connector as Client.GameState;
            Client.Send(Client.DHKeyExchance.CreateServerKeyPacket());
        }

        public static void GameServer_AnnounceReceive(byte[] arg1, Interfaces.ISocketWrapper arg2)
        {
            Client.GameState Client = arg2.Connector as Client.GameState;
            try
            {
                Client.Cryptography.Decrypt(arg1);
            }
            catch { }
            if (Client != null)
            {
                if (Client.Exchange)
                {
                    try
                    {
                        Client.Exchange = false;

                        Client.Action = 1;
                        ushort position = 7;
                        uint PacketLen = BitConverter.ToUInt32(arg1, position); position += 4;
                        int JunkLen = BitConverter.ToInt32(arg1, position); position += 4; position += (ushort)JunkLen;
                        int Len = BitConverter.ToInt32(arg1, position); position += 4;
                        byte[] pubKey = new byte[Len];
                        for (int x = 0; x < Len; x++)
                            pubKey[x] = arg1[x + position];
                        string PubKey = System.Text.ASCIIEncoding.ASCII.GetString(pubKey);

                        Client.Cryptography = Client.DHKeyExchance.HandleClientKeyPacket(PubKey, Client.Cryptography);
                    }
                    catch
                    {
                        Client.Socket.Disconnect(false);
                    }
                }
                else
                {
                    if (!Client.Exchange && Client.Action != 0)
                        Network.PacketHandler.HandleBuffer(arg1, Client);
                }
            }
        }
        static void GameServer_AnnounceDisconnection(Interfaces.ISocketWrapper obj)
        {
            if (obj.Connector != null)
            {
                Client.GameState Client = obj.Connector as Client.GameState;
                Client.Disconnect();
            }
        }
        static void AuthServer_AnnounceNewConnection(Interfaces.ISocketWrapper obj)
        {
            Client.AuthState authState = new Client.AuthState(obj.Socket);
            authState.Cryptographer = new Network.Cryptography.AuthCryptography();
            Network.AuthPackets.PasswordCryptographySeed pcs = new PasswordCryptographySeed();
            pcs.Seed = ServerBase.Kernel.Random.Next();
            authState.PasswordSeed = pcs.Seed;
            authState.Send(pcs);
            obj.Connector = authState;
        }
        static void AuthServer_AnnounceReceive(byte[] arg1, Interfaces.ISocketWrapper arg2)
        {
            if (arg1.Length == 240)
            {
                Client.AuthState player = arg2.Connector as Client.AuthState;
                player.Cryptographer.Decrypt(arg1);
                player.Info = new Authentication();
                player.Info.Deserialize(arg1);
                player.Account = new AccountTable(player.Info.Username);
                msvcrt.msvcrt.srand(player.PasswordSeed);

                byte[] encpw = new byte[16];
                var rc5Key = new byte[0x10];
                for (int i = 0; i < 0x10; i++)
                    rc5Key[i] = (byte)msvcrt.msvcrt.rand();

                Buffer.BlockCopy(arg1, 132, encpw, 0, 16);
                var password = "";
                try
                {

                    password = System.Text.Encoding.ASCII.GetString(
                                        (new Network.Cryptography.ConquerPasswordCryptpographer(player.Info.Username)).Decrypt(
                                            (new Network.Cryptography.RC5(rc5Key)).Decrypt(encpw)));

                    password = password.Split('\0')[0];
                }
                catch
                {
                    player.Send(new Conquer_Online_Server.Network.GamePackets.Message("Invalid client version you must have client Verion 5375.", "ALLUSERS", System.Drawing.Color.Orange, Conquer_Online_Server.Network.GamePackets.Message.Dialog));
                    return;
                }
                string NoNumPadNumbers = "";
                foreach (char c in password)
                {
                    switch (c.ToString())
                    {
                        case "-": NoNumPadNumbers += "0"; break;
                        case "#": NoNumPadNumbers += "1"; break;
                        case "(": NoNumPadNumbers += "2"; break;
                        case "\"": NoNumPadNumbers += "3"; break;
                        case "%": NoNumPadNumbers += "4"; break;
                        case "\f": NoNumPadNumbers += "5"; break;
                        case "'": NoNumPadNumbers += "6"; break;
                        case "$": NoNumPadNumbers += "7"; break;
                        case "&": NoNumPadNumbers += "8"; break;
                        case "!": NoNumPadNumbers += "9"; break;
                        default: NoNumPadNumbers += c; break;
                    }
                }
                password = NoNumPadNumbers;
                Forward Fw = new Forward();
                if (password == player.Account.Password)
                {
                    if (player.Account.State == AccountTable.AccountState.Banned)
                        Fw.Type = Forward.ForwardType.Banned;
                    else
                        Fw.Type = Forward.ForwardType.Ready;
                }
                else
                {
                   Fw.Type = Forward.ForwardType.InvalidInfo;
                }
                if (Fw.Type != Network.AuthPackets.Forward.ForwardType.InvalidInfo)
                {

                    Fw.Identifier = Network.AuthPackets.Forward.Incrementer.Next;
                    ServerBase.Kernel.AwaitingPool.Add(Fw.Identifier, player.Account);
                }
                Fw.IP = GameIP;
                Fw.Port = GamePort;
                player.Send(Fw);
            }
            else
            {

                arg2.Socket.Disconnect(false);
            }
        }
        static void AuthServer_AnnounceDisconnection(Interfaces.ISocketWrapper obj)
        {

        }

    }
}