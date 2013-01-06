using System;
using OpenSSL;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using Conquer_Online_Server.Network.Cryptography;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Network.Sockets;
using Conquer_Online_Server.Game;

namespace Conquer_Online_Server.Client
{
    public class GameState
    {
        public bool LoggedIn = false, Disconnected = false, Disconnected2 = false;
        public bool InPKT = false;
        public bool InPKT22 = true;
        public bool Alive = true;
        private WinSocket _socket;
        public Database.AccountTable Account;
        public GameCryptography Cryptography;
        public Kijo.Network.GamePackets.DHKeyExchange.ServerKeyExchange DHKeyExchance;
        public bool Exchange = true;
        public bool InArenaMatch = false;
        public TimerCallback CallBack;
        public Conquer_Online_Server.Game.ConquerStructures.QuizShow.Info QuizInfo;
        public int quarantineKill = 0;
        public int quarantineDeath = 0;
        public Game.Enums.Color staticArmorColor;
        public bool JustCreated = false;
        public Timer Timer;
        #region Network

        public Logger Logger = null;
        public GameState(WinSocket socket)
        {
            Attackable = false;
            Action = 0;
            _socket = socket;
            Cryptography = new GameCryptography(System.Text.ASCIIEncoding.ASCII.GetBytes(ServerBase.Constants.GameCryptographyKey));
            DHKeyExchance = new Kijo.Network.GamePackets.DHKeyExchange.ServerKeyExchange();
        }
        public void ReadyToPlay()
        {
            Screen = new Game.Screen(this);

            Inventory = new Game.ConquerStructures.Inventory(this);
            Equipment = new Game.ConquerStructures.Equipment(this);
            WarehouseOpen = false;
            WarehouseOpenTries = 0;
            TempPassword = "";
            Warehouses = new SafeDictionary<Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID, Conquer_Online_Server.Game.ConquerStructures.Warehouse>(20);
            Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.TwinCity, new Conquer_Online_Server.Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.TwinCity));
            Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.PhoenixCity, new Conquer_Online_Server.Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.PhoenixCity));
            Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.ApeCity, new Conquer_Online_Server.Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.ApeCity));
            Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.DesertCity, new Conquer_Online_Server.Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.DesertCity));
            Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.BirdCity, new Conquer_Online_Server.Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.BirdCity));
            Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity, new Conquer_Online_Server.Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity));
            Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.Market, new Conquer_Online_Server.Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.Market));
            Trade = new Game.ConquerStructures.Trade();
            ArenaStatistic = new ArenaStatistic(true);
            Prayers = new List<GameState>();
            map = null;
        }
        private bool SocketDisposed = false;
        public void Send(byte[] buffer)
        {
            if (SocketDisposed)
                return;
            byte[] _buffer = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, _buffer, 0, buffer.Length);
            Network.Writer.WriteString(ServerBase.Constants.ServerKey, _buffer.Length - 8, _buffer);
            try
            {
                if (_socket.Connected)
                {
                    if (Monitor.TryEnter(Cryptography, 10))
                    {
                        if (Monitor.TryEnter(_socket, 10))
                        {
                            Cryptography.Encrypt(_buffer);
                            _socket.Send(_buffer);

                            Monitor.Exit(_socket);
                        }
                        Monitor.Exit(Cryptography);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SocketDisposed = true;
                Disconnect();
                ServerBase.Kernel.GamePool.Remove(Account.EntityID);
            }
        }
        public void Send(Interfaces.IPacket buffer)
        {
            Send(buffer.ToArray());
        }
        public void SendScreenSpawn(Interfaces.IMapObject obj, bool self)
        {
            foreach (Interfaces.IMapObject _obj in Screen.Objects)
            {
                if (_obj == null)
                    continue;
                if (_obj.UID != Entity.UID)
                {
                    if (_obj.MapObjType == Game.MapObjectType.Player)
                    {
                        GameState client = _obj.Owner as GameState;
                        obj.SendSpawn(client, false);
                    }
                }
            }
            if (self)
                obj.SendSpawn(this);
        }
        public void RemoveScreenSpawn(Interfaces.IMapObject obj, bool self)
        {
            foreach (Interfaces.IMapObject _obj in Screen.Objects)
            {
                if (_obj == null) continue;
                if (obj == null) continue;
                if (_obj.UID != Entity.UID)
                {
                    if (_obj.MapObjType == Game.MapObjectType.Player)
                    {
                        GameState client = _obj.Owner as GameState;
                        client.Screen.Remove(obj);
                    }
                }
            }
            if (self)
                Screen.Remove(obj);
        }
        public void SendScreen(byte[] buffer, bool self)
        {
            foreach (Interfaces.IMapObject obj in Screen.Objects)
            {
                if (obj == null) continue;
                if (obj.UID != Entity.UID)
                {
                    if (obj.MapObjType == Game.MapObjectType.Player)
                    {
                        GameState client = obj.Owner as GameState;
                        if (WatchingGroup != null && client.WatchingGroup == null)
                            continue;
                        client.Send(buffer);
                    }
                }
            }
            if (self)
                Send(buffer);
        }
        public void SendScreen(Interfaces.IPacket buffer, bool self)
        {
            foreach (Interfaces.IMapObject obj in Screen.Objects)
            {
                if (obj == null) continue;
                if (obj.MapObjType == Game.MapObjectType.Player)
                {
                    GameState client = obj.Owner as GameState;
                    //if (WatchingGroup != null && client.WatchingGroup == null)
                    //   continue;
                    client.Send(buffer);
                }
            }
            if (self)
                Send(buffer);
        }
        public void Handle(byte[] buffer)
        {
        roleAgain:
            ushort Length = BitConverter.ToUInt16(buffer, 0);
            if ((Length + 8) == buffer.Length)
            {
                Network.Writer.WriteString(ServerBase.Constants.ServerKey, (buffer.Length - 8), buffer);


                ServerBase.Kernel.packetPooler.QueueTask(new PacketClient(buffer, this));
                //Network.PacketHandler.HandlePacket(buffer, this);
                return;
            }
            else if ((Length + 8) > buffer.Length)
            {
                return;
            }
            else
            {
                byte[] Packet = new byte[(Length + 8)];
                Buffer.BlockCopy(buffer, 0, Packet, 0, (Length + 8));
                byte[] _buffer = new byte[(buffer.Length - (Length + 8))];
                Buffer.BlockCopy(buffer, (Length + 8), _buffer, 0, (buffer.Length - (Length + 8)));
                buffer = _buffer;
                Network.Writer.WriteString(ServerBase.Constants.ServerKey, (Packet.Length - 8), Packet);

                ServerBase.Kernel.packetPooler.QueueTask(new PacketClient(Packet, this));
                //Network.PacketHandler.HandlePacket(Packet, this);
                goto roleAgain;
            }
        }
        public bool ExecutClient(uint uid)
        {
            bool right = false;
            var cmd = new Database.MySqlCommand(Database.MySqlCommandType.SELECT);
            cmd.Select("accounts").Where("EntityID", uid);
            var r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                this.Account = new Conquer_Online_Server.Database.AccountTable(r.ReadString("Username"));
                right = true;
            }
            r.Close();
            return right;
        }
        public bool TryJoin(UInt32 password, UInt32 _uid)
        {
            bool right = false;
            var cmd = new Database.MySqlCommand(Database.MySqlCommandType.SELECT);
            cmd.Select("accounts").Where("EntityID", _uid);
            var r = new Database.MySqlReader(cmd);
            if (r.Read())
                if (password == (uint)r.ReadString("Password").GetHashCode())
                    right = true;
            r.Close();
            if (right)
            {
                if (_uid != 0)
                    right = ExecutClient(_uid);
            }
            return right;

        }
        public Boolean TestEntity(UInt32 _uid)
        {
            string names = "";
            var cmd = new Database.MySqlCommand(Database.MySqlCommandType.SELECT);
            cmd.Select("entities").Where("UID", _uid);
            var r = new Database.MySqlReader(cmd);
            if (r.Read())
                names = r.ReadString("Name");
            r.Close();
            if (names == "")
                return false;//"NEW_ROLE";
            else
                return true;//"ANSWER_OK";
        }
        public void Disconnect()
        {
            if (_socket.Connected)
            {
                if (!SocketDisposed)
                {

                    SocketDisposed = true;
                    System.Threading.Thread.Sleep(15);
                    _socket.Disconnect(false);
                }
            }
            if (this.Screen != null)
            {
                if (this.Screen.MyTimer != null)
                {
                    this.Screen.MyTimer.Close();
                    this.Screen.MyTimer.Dispose();
                }
            }
            if (this.Entity != null)
            {
                if (this.Entity.MyTimer != null)
                {
                    this.Entity.MyTimer.Close();
                    this.Entity.MyTimer.Dispose();
                }
            }
            ShutDown();

        }

        private void ShutDown()
        {
            if (Disconnected)
                return;
            if (Logger != null)
                Logger.Close();
            if (this != null && this.Entity != null)
            {
                if (this.JustCreated)
                    return;

                Database.EntityTable.UpdateOnlineStatus(this, false);
                Database.EntityTable.SaveEntity(this);
                if (ServerBase.Kernel.GamePool.ContainsKey(this.Entity.UID))
                    ServerBase.Kernel.GamePool.Remove(this.Entity.UID);

                if (Booth != null)
                {
                    Booth.Remove();
                }
                Database.SkillTable.SaveProficiencies(this);
                Database.SkillTable.SaveSpells(this);
                Database.ArenaTable.SaveArenaStatistics(this.ArenaStatistic);

                if (Companion != null)
                {
                    Map.RemoveEntity(Companion);
                    Data data = new Data(true);
                    data.UID = Companion.UID;
                    data.ID = Data.RemoveEntity;
                    Companion.MonsterInfo.SendScreen(data);
                }
                if (QualifierGroup != null)
                    QualifierGroup.End(this);


                if (ArenaStatistic.Status != Network.GamePackets.ArenaStatistic.NotSignedUp)
                    Game.ConquerStructures.Arena.QualifyEngine.DoQuit(this);

                RemoveScreenSpawn(this.Entity, false);

                if (!Disconnected)
                    Console.WriteLine(this.Entity.Name + " has logged off. {" + this.Account.IP + "}");
               // System.Threading.Thread.Sleep(1500);
                //this.Send(new Conquer_Online_Server.Network.GamePackets.Message("http://5.41.141.216/index.php", System.Drawing.Color.Yellow, 2105));
                Disconnected = true;

                #region Friend/TradePartner/Apprentice
                Message msg = new Message("Your friend, " + Entity.Name + ", has logged off.", System.Drawing.Color.Red, Message.TopLeft);
                if (Friends == null)
                    Friends = new SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Friend>(100);
                foreach (Game.ConquerStructures.Society.Friend friend in Friends.Values)
                {
                    if (friend.IsOnline)
                    {
                        var packet = new KnownPersons(true)
                        {
                            UID = Entity.UID,
                            Type = KnownPersons.RemovePerson,
                            Name = Entity.Name,
                            Online = false
                        };
                        friend.Client.Send(packet);
                        packet.Type = KnownPersons.AddFriend;
                        if (friend != null)
                        {
                            if (friend.Client != null)
                            {
                                friend.Client.Send(packet);
                                friend.Client.Send(msg);
                            }
                        }
                    }
                }
                Message msg2 = new Message("Your partner, " + Entity.Name + ", has logged off.", System.Drawing.Color.Red, Message.TopLeft);

                foreach (Game.ConquerStructures.Society.TradePartner partner in Partners.Values)
                {
                    if (partner.IsOnline)
                    {
                        var packet = new TradePartner(true)
                        {
                            UID = Entity.UID,
                            Type = TradePartner.BreakPartnership,
                            Name = Entity.Name,
                            HoursLeft = (int)(new TimeSpan(partner.ProbationStartedOn.AddDays(3).Ticks).TotalHours - new TimeSpan(DateTime.Now.Ticks).TotalHours),
                            Online = false
                        };
                        partner.Client.Send(packet);
                        packet.Type = TradePartner.AddPartner;
                        if (partner != null)
                        {
                            if (partner.Client != null)
                            {
                                partner.Client.Send(packet);
                                partner.Client.Send(msg2);
                            }
                        }
                    }
                }
                MentorInformation Information = new MentorInformation(true);
                Information.Mentor_Type = 1;
                Information.Mentor_ID = Entity.UID;
                Information.Mentor_Level = Entity.Level;
                Information.Mentor_Class = Entity.Class;
                Information.Mentor_PkPoints = Entity.PKPoints;
                Information.Mentor_Mesh = Entity.Mesh;
                Information.Mentor_Online = false;
                Information.String_Count = 3;
                Information.Mentor_Name = Entity.Name;
                Information.Mentor_Spouse_Name = Entity.Spouse;
                foreach (var appr in Apprentices.Values)
                {
                    if (appr.IsOnline)
                    {
                        Information.Apprentice_ID = appr.ID;
                        Information.Enrole_Date = appr.EnroleDate;
                        Information.Apprentice_Name = appr.Name;
                        appr.Client.Send(Information);
                        appr.Client.ReviewMentor();
                    }
                }
                if (Mentor != null)
                {
                    if (Mentor.IsOnline)
                    {
                        ApprenticeInformation AppInfo = new ApprenticeInformation();
                        AppInfo.Apprentice_ID = Entity.UID;
                        AppInfo.Apprentice_Level = Entity.Level;
                        AppInfo.Apprentice_Name = Entity.Name;
                        AppInfo.Apprentice_Online = false;
                        AppInfo.Apprentice_Spouse_Name = Entity.Spouse;
                        AppInfo.Enrole_date = Mentor.EnroleDate;
                        AppInfo.Mentor_ID = Mentor.Client.Entity.UID;
                        AppInfo.Mentor_Mesh = Mentor.Client.Entity.Mesh;
                        AppInfo.Mentor_Name = Mentor.Client.Entity.Name;
                        AppInfo.Type = 2;
                        Mentor.Client.Send(AppInfo);
                    }
                }

                #endregion
                if (Team != null)
                {
                    if (Team.TeamLeader)
                    {
                        Network.GamePackets.Team team = new Team();
                        team.UID = Account.EntityID;
                        team.Type = Network.GamePackets.Team.Dismiss;
                        foreach (Client.GameState Teammate in Team.Teammates)
                        {
                            if (Teammate != null)
                            {
                                if (Teammate.Entity.UID != Account.EntityID)
                                {
                                    Teammate.Send(team);
                                    Teammate.Team = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        Network.GamePackets.Team team = new Team();
                        team.UID = Account.EntityID;
                        team.Type = Network.GamePackets.Team.ExitTeam;
                        foreach (Client.GameState Teammate in Team.Teammates)
                        {
                            if (Teammate != null)
                            {
                                if (Teammate.Entity.UID != Account.EntityID)
                                {
                                    Teammate.Send(team);
                                    Teammate.Team.Remove(Account.EntityID);
                                }
                            }
                        }
                    }
                }
            }
        }

        public WinSocket Socket
        { get { return _socket; } }
        public string IP
        {
            get
            {
                return Socket.RemoteEndPoint.ToString().Split(':')[0];
            }
        }
        #endregion

        #region Game

        public SafeDictionary<uint, DetainedItem> ClaimableItem = new SafeDictionary<uint, DetainedItem>(1000),
                                                  DeatinedItem = new SafeDictionary<uint, DetainedItem>(1000);

        public bool DoSetOffline = true;

        public ushort OnlineTrainingPoints = 0;
        public Time32 LastTrainingPointsUp;

        public List<string> GuildNamesSpawned = new List<string>();

        public byte KylinUpgradeCount = 0;

        public ulong OblivionExperience = 0;
        public byte OblivionKills = 0;

        public int PremShopType = 0;
        public DateTime VIPDate;
        public DateTime LastVote;
        public uint VIPDays;
        public uint DonationPoints;
        public uint VotePoints;

        public Time32 ScreenReloadTime;
        public int MillisecondsScreenReload;
        public bool Reloaded = false;
        public Interfaces.IPacket ReloadWith;

        public ushort VendingDisguise;
        public uint BlessTime;
        public int speedHackSuspiction = 0;
        public Time32 LastPingT;
        public Game.Entity Companion;

        public List<GameState> Prayers;
        public GameState PrayLead;

        public DateTime ChatBanTime;
        public uint ChatBanLasts;
        public bool ChatBanned;

        public byte JewelarLauKind, JewelarLauGems;
        public uint VirtuePoints;
        public DateTime LastLotteryEntry;
        public byte LotteryEntries;
        public bool InLottery;
        public DateTime OfflineTGEnterTime;
        public bool Mining = false;
        public Time32 MiningStamp;


        public bool HeadgearClaim, NecklaceClaim, ArmorClaim, WeaponClaim, RingClaim, BootsClaim, TowerClaim, FanClaim;
        public string PromoteItemNameNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return " nothing but";
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return " five Euxenite Ores and";
                    else
                        return " nothing but";
                if (Entity.Class % 10 == 2)
                    return " one Emerald and";
                if (Entity.Class % 10 == 3)
                    return " one Meteor and";
                if (Entity.Class % 10 == 4)
                    return " one MoonBox and";
                return " nothing but";
            }
        }
        public byte PromoteItemCountNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 0;
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return 5;
                    else
                        return 0;
                if (Entity.Class % 10 == 2)
                    return 1;
                if (Entity.Class % 10 == 3)
                    return 1;
                if (Entity.Class % 10 == 4)
                    return 1;
                return 0;
            }
        }
        public uint PromoteItemNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 0;
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return 1072031;
                    else
                        return 0;
                if (Entity.Class % 10 == 2)
                    return 1080001;
                if (Entity.Class % 10 == 3)
                    return 1088001;
                if (Entity.Class % 10 == 4)
                    return 721020;
                return 0;
            }
        }
        public uint PromoteItemGain
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 0;
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return 500067;
                    else
                        return 0;
                if (Entity.Class % 10 == 2)
                    return 0;
                if (Entity.Class % 10 == 3)
                    return 700031;
                if (Entity.Class % 10 == 4)
                    return 1088000;
                return 0;
            }
        }
        public uint PromoteLevelNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 15;
                if (Entity.Class % 10 == 1)
                    return 40;
                if (Entity.Class % 10 == 2)
                    return 70;
                if (Entity.Class % 10 == 3)
                    return 100;
                if (Entity.Class % 10 == 4)
                    return 110;
                return 0;
            }
        }
        public byte SelectedItem, UpdateType;
        public ushort UplevelProficiency;
        public uint OnHoldGuildJoin = 0;
        public bool SentRequest = false;
        public DateTime LastLotteryEntry2;
        public byte LotteryEntries2;
        public bool InLottery2;
        public Game.ConquerStructures.Society.Guild Guild;
        public Game.ConquerStructures.Society.Guild.Member AsMember;
        public Game.ConquerStructures.Booth Booth;

        public void ReviewMentor()
        {
            #region NotMentor
            uint nowBP = 0;
            if (Mentor != null)
            {
                if (Mentor.IsOnline)
                {
                    nowBP = (uint)(((Mentor.Client.Entity.BattlePower - Mentor.Client.Entity.ExtraBattlePower) - (Entity.BattlePower - Entity.ExtraBattlePower)) / 3.3F);
                }
            }
            if (nowBP > 200)
                nowBP = 0;
            if (nowBP < 0)
                nowBP = 0;
            if (Entity.ExtraBattlePower != nowBP)
            {
                Entity.ExtraBattlePower = nowBP;
                if (Mentor != null)
                {
                    if (Mentor.IsOnline)
                    {
                        MentorInformation Information = new MentorInformation(true);
                        Information.Mentor_Type = 1;
                        Information.Mentor_ID = Mentor.Client.Entity.UID;
                        Information.Apprentice_ID = Entity.UID;
                        Information.Enrole_Date = Mentor.EnroleDate;
                        Information.Mentor_Level = Mentor.Client.Entity.Level;
                        Information.Mentor_Class = Mentor.Client.Entity.Class;
                        Information.Mentor_PkPoints = Mentor.Client.Entity.PKPoints;
                        Information.Mentor_Mesh = Mentor.Client.Entity.Mesh;
                        Information.Mentor_Online = true;
                        Information.Shared_Battle_Power = (uint)(((Mentor.Client.Entity.BattlePower - Mentor.Client.Entity.ExtraBattlePower) - (Entity.BattlePower - Entity.ExtraBattlePower)) / 3.3F);
                        Information.String_Count = 3;
                        Information.Mentor_Name = Mentor.Client.Entity.Name;
                        Information.Apprentice_Name = Entity.Name;
                        Information.Mentor_Spouse_Name = Mentor.Client.Entity.Spouse;
                        Send(Information);
                    }
                }
            }
            #endregion
            #region Mentor
            foreach (var appr in Apprentices.Values)
            {
                if (appr.IsOnline)
                {
                    uint nowBPs = 0;
                    nowBPs = (uint)(((Entity.BattlePower - Entity.ExtraBattlePower) - (appr.Client.Entity.BattlePower - appr.Client.Entity.ExtraBattlePower)) / 3.3F);
                    if (appr.Client.Entity.ExtraBattlePower != nowBPs)
                    {
                        appr.Client.Entity.ExtraBattlePower = nowBPs;
                        MentorInformation Information = new MentorInformation(true);
                        Information.Mentor_Type = 1;
                        Information.Mentor_ID = Entity.UID;
                        Information.Apprentice_ID = appr.Client.Entity.UID;
                        Information.Enrole_Date = appr.EnroleDate;
                        Information.Mentor_Level = Entity.Level;
                        Information.Mentor_Class = Entity.Class;
                        Information.Mentor_PkPoints = Entity.PKPoints;
                        Information.Mentor_Mesh = Entity.Mesh;
                        Information.Mentor_Online = true;
                        Information.Shared_Battle_Power = nowBPs;
                        Information.String_Count = 3;
                        Information.Mentor_Name = Entity.Name;
                        Information.Apprentice_Name = appr.Client.Entity.Name;
                        Information.Mentor_Spouse_Name = Entity.Spouse;
                        appr.Client.Send(Information);
                    }
                }
            }
            #endregion
        }
        public void AddQuarantineKill()
        {
            quarantineKill++;
            UpdateQuarantineScore();
        }
        public void AddQuarantineDeath()
        {
            quarantineDeath++;
            UpdateQuarantineScore();
        }
        public void UpdateQuarantineScore()
        {
            string[] scores = new string[3];
            scores[0] = "Black team: " + Conquer_Online_Server.Game.ConquerStructures.Quarantine.BlackScore.ToString() + " wins";
            scores[1] = "White team: " + Conquer_Online_Server.Game.ConquerStructures.Quarantine.WhiteScore.ToString() + " wins";
            scores[2] = "Your score: " + quarantineKill + " kills, " + quarantineDeath + " death";
            for (int i = 0; i < scores.Length; i++)
            {
                Message msg = new Message(scores[i], System.Drawing.Color.Red, i == 0 ? Message.FirstRightCorner : Message.ContinueRightCorner);
                Send(msg);
            }
        }
        public void AddBless(uint value)
        {
            Entity.HeavenBlessing += value;
            Entity.Update(Network.GamePackets._String.Effect, "bless", true);
            if (Mentor != null)
            {
                if (Mentor.IsOnline)
                {
                    Mentor.Client.PrizeHeavenBlessing += (ushort)(value / 10 / 60 / 60);
                    AsApprentice = Mentor.Client.Apprentices[Entity.UID];
                }
                if (AsApprentice != null)
                {
                    AsApprentice.Actual_HeavenBlessing += (ushort)(value / 10 / 60 / 60);
                    AsApprentice.Total_HeavenBlessing += (ushort)(value / 10 / 60 / 60);
                    if (Time32.Now > LastMentorSave.AddSeconds(5))
                    {
                        Database.KnownPersons.SaveApprenticeInfo(AsApprentice);
                        LastMentorSave = Time32.Now;
                    }
                }
            }
        }
        public ulong PrizeExperience;
        public ushort PrizeHeavenBlessing;
        public ushort PrizePlusStone;

        public uint MentorApprenticeRequest;
        public uint TradePartnerRequest;

        public object[] OnMessageBoxEventParams;
        public Action OnMessageBoxOK;
        public Action OnMessageBoxCANCEL;

        public bool JustLoggedOn = true;
        public Time32 ReviveStamp = Time32.Now;
        public bool Attackable
        {
            get;
            set;
        }

        public Game.ConquerStructures.NobilityInformation NobilityInformation;
        public Game.Entity Entity;
        public Game.Screen Screen;
        public int PingCount = 0;

        public Time32 LastPing = Time32.Now;
        public static ushort NpcTestType = 0;
        public byte TinterItemSelect = 0;
        public DateTime LastDragonBallUse, LastResetTime;
        public byte Action = 0;
        public bool CheerSent = false;
        public Game.ConquerStructures.Arena.QualifierList.QualifierGroup WatchingGroup;
        public byte XPCount = 0;
        public Time32 XPCountStamp = Time32.Now;
        public Time32 XPListStamp = Time32.Now;
        public Game.ConquerStructures.Arena.QualifierList.QualifierGroup QualifierGroup;
        public Network.GamePackets.ArenaStatistic ArenaStatistic;
        public Network.GamePackets.HorseRaceStatistic HorseRaceStatistic;
        public Conquer_Online_Server.Game.ConquerStructures.Trade Trade;
        public byte ExpBalls = 0;
        public uint MoneySave = 0, ActiveNpc = 0;
        public string WarehousePW, TempPassword;
        public bool WarehouseOpen;
        public Time32 CoolStamp;
        public sbyte WarehouseOpenTries;
        public ushort InputLength;
        public Conquer_Online_Server.Game.ConquerStructures.Society.Mentor Mentor;
        public Conquer_Online_Server.Game.ConquerStructures.Society.Apprentice AsApprentice;
        public SafeDictionary<ushort, Interfaces.ISkill> Proficiencies;
        public SafeDictionary<ushort, Interfaces.ISkill> Spells;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Friend> Friends;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Enemy> Enemy;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.TradePartner> Partners;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Apprentice> Apprentices;
        public Game.ConquerStructures.Inventory Inventory;
        public Game.ConquerStructures.Equipment Equipment;
        public SafeDictionary<Game.ConquerStructures.Warehouse.WarehouseID, Game.ConquerStructures.Warehouse> Warehouses;
        public Game.ConquerStructures.Team Team;
        public Time32 lastClientJumpTime = Time32.Now;
        public Time32 lastJumpTime = Time32.Now;
        public int LastJumpTime = 0;
        public short lastJumpDistance = 0;
        public bool DoubleExpToday = false;
        public void SendEquipment(bool Others)
        {
            ItemUsage I = new ItemUsage(true);
            I.UID = this.Entity.UID;
            I.ID = 46;
            for (uint Z = 1; Z < 13; Z++)
            {
                if (!this.Equipment.Free(Z))
                {
                    Interfaces.IConquerItem Item = this.Equipment.TryGetItem(Z);
                    switch (Item.Position)
                    {
                        case 1:
                            I.Pos1 = Item.UID;
                            break;
                        case 2:
                            I.Pos2 = Item.UID;
                            break;
                        case 3:
                            I.Pos3 = Item.UID;
                            break;
                        case 4:
                            I.Pos4 = Item.UID;
                            break;
                        case 5:
                            I.Pos5 = Item.UID;
                            break;
                        case 6:
                            I.Pos6 = Item.UID;
                            break;
                        case 7:
                            I.Pos7 = Item.UID;
                            break;
                        case 8:
                            I.Pos8 = Item.UID;
                            break;
                        case 9:
                            I.Pos9 = Item.UID;
                            break;
                        case 10:
                            I.Pos10 = Item.UID;
                            break;
                        case 11:
                            I.Pos11 = Item.UID;
                            break;
                    }
                }
            }
            if (Others)
                this.SendScreen(I, true);
            else
                this.Send(I);
        }
        private Game.Map map;
        public Game.Map Map
        {
            get
            {
                if (map == null)
                {
                    ServerBase.Kernel.Maps.TryGetValue(Entity.MapID, out map);
                    if (Entity.MapID >= 11000 && map == null)
                        Entity.MapID = 1005;
                    if (map == null)
                        return (map = new Conquer_Online_Server.Game.Map(Entity.MapID, Database.MapsTable.MapInformations[Entity.MapID].BaseID, Database.DMaps.MapPaths[Database.MapsTable.MapInformations[Entity.MapID].BaseID], false));
                }
                else
                {
                    if (map.ID != Entity.MapID)
                    {
                        ServerBase.Kernel.Maps.TryGetValue(Entity.MapID, out map);
                        if (Entity.MapID >= 11000 && map == null)
                            Entity.MapID = 1005;
                        if (map == null)
                            return (map = new Conquer_Online_Server.Game.Map(Entity.MapID, Database.MapsTable.MapInformations[Entity.MapID].BaseID, Database.DMaps.MapPaths[Database.MapsTable.MapInformations[Entity.MapID].BaseID], false));
                    }
                }
                return map;
            }
        }

        public uint ExpBall
        {
            get
            {
                ulong exp = Database.DataHolder.LevelExperience(Entity.Level);
                return (uint)(exp * 13000 / (ulong)((Entity.Level * Entity.Level * Entity.Level / 12) + 1));
            }
        }

        public bool AddProficiency(Interfaces.ISkill proficiency)
        {
            if (Proficiencies.ContainsKey(proficiency.ID))
            {
                Proficiencies[proficiency.ID].Level = proficiency.Level;
                Proficiencies[proficiency.ID].Experience = proficiency.Experience;
                proficiency.Send(this);
                return false;
            }
            else
            {
                Proficiencies.Add(proficiency.ID, proficiency);
                proficiency.Send(this);
                return true;
            }
        }

        public bool AddSpell(Interfaces.ISkill spell)
        {
            if (Spells.ContainsKey(spell.ID))
            {
                if (Spells[spell.ID].Level < spell.Level)
                {
                    Spells[spell.ID].Level = spell.Level;
                    Spells[spell.ID].Experience = spell.Experience;
                    if (spell.ID != 3060)
                        spell.Send(this);
                }
                return false;
            }
            else
            {
                if (spell.ID == 1045 || spell.ID == 1046)
                {
                    if (Proficiencies.ContainsKey(Database.SpellTable.SpellInformations[spell.ID][spell.Level].WeaponSubtype))
                    {
                        if (Proficiencies[Database.SpellTable.SpellInformations[spell.ID][spell.Level].WeaponSubtype].Level < 5)
                            return false;
                    }
                    else
                        return false;
                }
                Spells.Add(spell.ID, spell);
                if (spell.ID != 3060)
                    spell.Send(this);
                return true;
            }
        }
        public bool RemoveSpell(Interfaces.ISkill spell)
        {
            if (Spells.ContainsKey(spell.ID))
            {
                Spells.Remove(spell.ID);
                Network.GamePackets.Data data = new Data(true);
                data.UID = Entity.UID;
                data.dwParam = spell.ID;
                data.ID = 109;
                Send(data);
                Database.SkillTable.DeleteSpell(this, spell.ID);
                return true;
            }
            return false;
        }
        public bool WentToComplete = false;
        public byte SelectedGem = 0;
        public Time32 LastMentorSave = Time32.Now;
        public void IncreaseExperience(ulong experience, bool addMultiple)
        {
            if (Entity.Dead)
                return;

            byte level = Entity.Level;
            ulong _experience = Entity.Experience;
            ulong prExperienece = experience;
            if (addMultiple)
            {
                if (Entity.VIPLevel == 1)
                    experience *= 2;
                if (Entity.VIPLevel == 2)
                    experience *= 3;
                if (Entity.VIPLevel == 3)
                    experience *= 4;
                if (Entity.VIPLevel == 4)
                    experience *= 5;
                if (Entity.VIPLevel == 5)
                    experience *= 6;
                if (Entity.VIPLevel == 6)
                    experience *= 7;
                //if (Entity.Level == 1)
                //{
                //    Inventory.Add(723753, 0, 1, true);
                //}
                if (Entity.Level > 135 && Entity.Level < 141)
                {
                    experience /= 10;
                }
                //if (Program.Today == DayOfWeek.Saturday || Program.Today == DayOfWeek.Sunday)
                //    experience *= 2;
                experience *= ServerBase.Constants.ExtraExperienceRate;
                experience += experience * Entity.Gems[3] / 100;
                if (Entity.DoubleExperienceTime > 0)
                    experience *= 2;
                if (Entity.DoubleExperienceTime5 > 0)
                    experience *= 5;
                if (Entity.DoubleExperienceTime10 > 0)
                    experience *= 10;
                if (Entity.DoubleExperienceTime15 > 0)
                    experience *= 15;
                if (Entity.HeavenBlessing > 0)
                    experience += (uint)(experience * 20 / 100);
                if (Entity.Reborn >= 2)
                    experience /= 3;
                if (Entity.Reborn == 1)
                    experience /= 2;
                if (Map.BaseID == 1039)
                    experience /= 50;
                if (Guild != null)
                {
                    if (Guild.Level > 0)
                    {
                        experience += (ushort)(experience * Guild.Level / 100);
                    }
                }
                prExperienece = experience + (ulong)(experience * ((float)Entity.BattlePower / 100));

                _experience += prExperienece;
            }
            else
                _experience += experience;
            if (Entity.Level < 140)
            {
                while (_experience >= Database.DataHolder.LevelExperience(level) && level < 140)
                {
                    _experience -= Database.DataHolder.LevelExperience(level);
                    level++;
                    if (Entity.Reborn == 1)
                    {
                        if (level >= 130 && Entity.FirstRebornLevel > 130 && level < Entity.FirstRebornLevel)
                            level = Entity.FirstRebornLevel;
                    }
                    else if (Entity.Reborn == 2)
                    {
                        if (level >= 130 && Entity.SecondRebornLevel > 130 && level < Entity.SecondRebornLevel)
                            level = Entity.SecondRebornLevel;
                    }
                    if (Entity.Class >= 10 && Entity.Class <= 15)
                        if (!Spells.ContainsKey(1110))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 1110 });
                    if (Entity.Class >= 20 && Entity.Class <= 25)
                        if (!Spells.ContainsKey(1020))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 1020 });
                    if (Entity.Class >= 40 && Entity.Class <= 45)
                        if (!Spells.ContainsKey(8002))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 8002 });
                    if (Entity.Class >= 50 && Entity.Class <= 55)
                        if (!Spells.ContainsKey(6011))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 6011 });
                    if (Entity.Class >= 60 && Entity.Class <= 65)
                        if (!Spells.ContainsKey(10490))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 10490 });
                    if (Mentor != null)
                    {
                        if (Mentor.IsOnline)
                        {
                            Mentor.Client.PrizeExperience += (ulong)level;
                            AsApprentice = Mentor.Client.Apprentices[Entity.UID];
                            if (Mentor.Client.PrizeExperience > 50 * 606)
                                Mentor.Client.PrizeExperience = 50 * 606;
                        }
                    }
                    if (level == 70)
                    {
                        if (ArenaStatistic == null || ArenaStatistic.EntityID == 0)
                        {
                            ArenaStatistic = new Conquer_Online_Server.Network.GamePackets.ArenaStatistic(true);
                            ArenaStatistic.EntityID = Entity.UID;
                            ArenaStatistic.Name = Entity.Name;
                            ArenaStatistic.Level = Entity.Level;
                            ArenaStatistic.Class = Entity.Class;
                            ArenaStatistic.Model = Entity.Mesh;
                            ArenaStatistic.ArenaPoints = Database.ArenaTable.ArenaPointFill(Entity.Level);
                            ArenaStatistic.LastArenaPointFill = DateTime.Now;
                            Database.ArenaTable.InsertArenaStatistic(this);
                            ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                            Game.ConquerStructures.Arena.ArenaStatistics.Add(Entity.UID, ArenaStatistic);
                        }
                    }
                    if (Entity.Reborn == 0)
                    {
                        if (level <= 120)
                        {
                            Database.DataHolder.GetStats(Entity.Class, level, this);
                            CalculateStatBonus();
                            CalculateHPBonus();
                            GemAlgorithm();
                            SendStatMessage();
                        }
                        else
                            Entity.Atributes += 3;
                    }
                    else
                    {
                        Entity.Atributes += 3;
                    }
                }
                if (Entity.Level != level)
                {
                    if (Team != null)
                    {
                        if (Team.LowestLevelsUID == Entity.UID)
                        {
                            Team.LowestLevel = 0;
                            Team.LowestLevelsUID = 0;
                            Team.SearchForLowest();
                        }
                    }
                    Entity.Level = level;
                    Entity.Hitpoints = Entity.MaxHitpoints;
                    Entity.Mana = Entity.MaxMana;
                    //if (Entity.Reborn == 0 && Inventory.Count < 39)
                    //{
                    //    if (Entity.Level == 10)
                    //        Inventory.Add(723753, 0, 1, true);
                    //    //if (Entity.Level == 70)
                    //    //    Inventory.Add(723768, 0, 1);
                    //    //if (Entity.Level == 100)
                    //    //    Inventory.Add(723772, 0, 1);
                    //    //if (Entity.Level == 110)
                    //    //    Inventory.Add(723774, 0, 1);
                    //    //if (Entity.Level == 120)
                    //    //    Inventory.Add(723776, 0, 1);
                    //}
                }
                if (Entity.Experience != _experience)
                    Entity.Experience = _experience;
            }
        }

        public void IncreaseSpellExperience(uint experience, ushort id)
        {
            if (Spells.ContainsKey(id))
            {
                switch (id)
                {
                    case 1290:
                    case 5030:
                    case 7030:
                        experience = 100; break;
                }
                experience *= ServerBase.Constants.ExtraSpellRate;
                experience += experience * Entity.Gems[6] / 100;
                if (Map.BaseID == 1039)
                    experience /= 40;
                Interfaces.ISkill spell = Spells[id];
                if (spell == null)
                    return;
                //if (Entity.VIPLevel > 0)
                //{
                //    experience *= 5;
                //}
                Database.SpellInformation spellInfo = Database.SpellTable.SpellInformations[spell.ID][spell.Level];
                if (spellInfo != null)
                {
                    if (spellInfo.NeedExperience != 0 && Entity.Level >= spellInfo.NeedLevel)
                    {
                        spell.Experience += experience;
                        bool leveled = false;
                        if (spell.Experience >= spellInfo.NeedExperience)
                        {
                            spell.Experience = 0;
                            spell.Level++;
                            leveled = true;
                            Send(ServerBase.Constants.SpellLeveled);
                        }
                        if (leveled)
                        {
                            spell.Send(this);
                        }
                        else
                        {
                            Network.GamePackets.SkillExperience update = new SkillExperience(true);
                            update.AppendSpell(spell.ID, spell.Experience);
                            update.Send(this);
                        }
                    }
                }
            }
        }

        public void IncreaseProficiencyExperience(uint experience, ushort id)
        {
            if (Proficiencies.ContainsKey(id))
            {
                Interfaces.ISkill proficiency = Proficiencies[id];
                experience *= ServerBase.Constants.ExtraProficiencyRate;
                experience += experience * Entity.Gems[5] / 100;
                if (Map.BaseID == 1039)
                    experience /= 40;
                //if (Entity.VIPLevel > 0)
                //{
                //    experience *= 5;
                //}
                proficiency.Experience += experience;
                if (proficiency.Level < 20)
                {
                    bool leveled = false;
                    while (proficiency.Experience >= Database.DataHolder.ProficiencyLevelExperience(proficiency.Level))
                    {
                        proficiency.Experience -= Database.DataHolder.ProficiencyLevelExperience(proficiency.Level);
                        proficiency.Level++;
                        if (proficiency.Level == 20)
                        {
                            proficiency.Experience = 0;
                            proficiency.Send(this);
                            Send(ServerBase.Constants.ProficiencyLeveled);
                            return;
                        }
                        leveled = true;
                        Send(ServerBase.Constants.ProficiencyLeveled);
                    }
                    if (leveled)
                    {
                        proficiency.Send(this);
                    }
                    else
                    {
                        Network.GamePackets.SkillExperience update = new SkillExperience(true);
                        update.AppendProficiency(proficiency.ID, proficiency.Experience, Database.DataHolder.ProficiencyLevelExperience(proficiency.Level));
                        update.Send(this);
                    }
                }
            }
            else
            {
                AddProficiency(new Network.GamePackets.Proficiency(true) { ID = id });
            }
        }

        public byte ExtraAtributePoints(byte level, byte mClass)
        {
            if (mClass == 135)
            {
                if (level <= 110)
                    return 0;
                switch (level)
                {
                    case 112: return 1;
                    case 114: return 3;
                    case 116: return 6;
                    case 118: return 10;
                    case 120: return 15;
                    case 121: return 15;
                    case 122: return 21;
                    case 123: return 21;
                    case 124: return 28;
                    case 125: return 28;
                    case 126: return 36;
                    case 127: return 36;
                    case 128: return 45;
                    case 129: return 45;
                    default:
                        return 55;
                }
            }
            else
            {
                if (level <= 120)
                    return 0;
                switch (level)
                {
                    case 121: return 1;
                    case 122: return 3;
                    case 123: return 6;
                    case 124: return 10;
                    case 125: return 15;
                    case 126: return 21;
                    case 127: return 28;
                    case 128: return 36;
                    case 129: return 45;
                    default:
                        return 55;
                }
            }
        }
        public bool Reincarnate2(byte toClass)
        {
            #region Items
            if (Inventory.Count > 38)
                return false;
            switch (toClass)
            {
                case 11:
                case 21:
                    {
                        Inventory.Add(410077, Game.Enums.ItemEffect.Poison);
                        break;
                    }
                case 41:
                    {
                        Inventory.Add(500057, Game.Enums.ItemEffect.Shield);
                        break;
                    }
                case 132:
                case 142:
                    {
                        if (toClass == 132)
                            Inventory.Add(421077, Game.Enums.ItemEffect.MP);
                        else
                            Inventory.Add(421077, Game.Enums.ItemEffect.HP);
                        break;
                    }
            }
            #region Low level items
            for (byte i = 1; i < 9; i++)
            {
                if (i != 7)
                {
                    Interfaces.IConquerItem item = Equipment.TryGetItem(i);
                    if (item != null && item.ID != 0)
                    {
                        try
                        {
                            UnloadItemStats(item, false);
                            Database.ConquerItemInformation cii = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                            item.ID = cii.LowestID(Network.PacketHandler.ItemMinLevel(Network.PacketHandler.ItemPosition(item.ID)));
                            item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            item.Send(this);
                            LoadItemStats(item);
                            Database.ConquerItemTable.UpdateItemID(item, this);
                        }
                        catch
                        {
                            Console.WriteLine("Reborn item problem: " + item.ID);
                        }
                    }
                }
            }
            Interfaces.IConquerItem hand = Equipment.TryGetItem(5);
            if (hand != null)
            {
                Equipment.Remove(5);
                CalculateStatBonus();
                CalculateHPBonus();
                SendStatMessage();
            }
            else
                SendScreen(Entity.SpawnPacket, false);
            #endregion

            #endregion
            #region Set Class
            if ( Entity.Level < 130)
                return false;
            //_client = this;
            //RemoveSkill = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(500);
            //Addskill = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(500);
         

            
             Entity.FirstRebornClass =  Entity.SecondRebornClass;
             Entity.SecondRebornClass =  Entity.Class;
           Entity.Class = toClass;
             Entity.SecondRebornLevel =  Entity.Level;
             Entity.ReincarnationLev =  Entity.Level;
             Entity.Level = 15;
             Entity.Experience = 0;
             Entity.Atributes = 182;



            // Spells.Clear();
            // Spells = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(100);
            //Entity.FirstRebornClass = Entity.SecondRebornClass;
            //Entity.SecondRebornClass = Entity.Class;
            //Entity.Atributes = 182;
            //Entity.Class = toClass;
            //Entity.Level = 15;
            //Entity.Experience = 0;
            #endregion
            #region Spells
            // Interfaces.ISkill[] spells = new Conquer_Online_Server.Interfaces.ISkill[Spells.Count];
            // Spells.CopyTo(ref spells, 0);
            Interfaces.ISkill[] spells = Spells.Values.ToArray();
            foreach (Interfaces.ISkill spell in spells)
            {

                Entity.Owner.RemoveSpell(spell);
            }
            #endregion
            #region Proficiencies
            foreach (Interfaces.ISkill proficiency in Proficiencies.Values)
            {
                proficiency.PreviousLevel = proficiency.Level;
                proficiency.Level = 0;
                proficiency.Experience = 0;
                proficiency.Send(this);
            }
            #endregion
            #region Adding earned skills
            if (Entity.Reborn == 2)
            { AddSpell(new Spell(true) { ID = 9876 }); }

            Dictionary<ushort, Spell> toReceive = FirstRebornSpells(Entity.FirstRebornClass, Entity.Class);
            foreach (Spell s in toReceive.Values)
                AddSpell(s);

            Dictionary<ushort, Spell> toReceive2 = SecondRebornSpells(Entity.FirstRebornClass, Entity.SecondRebornClass, Entity.Class);
            foreach (Spell s2 in toReceive2.Values)
                AddSpell(s2);
            #endregion

            Database.DataHolder.GetStats(Entity.Class, Entity.Level, this);

            this.CalculateHPBonus();
            this.CalculateStatBonus();
            ServerBase.Kernel.SendWorldMessage(new Message(Entity.Name + " reincarnated", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
            //Network.PacketHandler.WorldMessage(Entity.Name + " reincarnated.", Message.Talk, System.Drawing.Color.White);
            return true;
        }

        #region Award Reborn Spells
        public Dictionary<ushort, Spell> FirstRebornSpells(int fClass, int currentClass)
        {
            Dictionary<ushort, Spell> spells = new Dictionary<ushort, Spell>();

            switch (fClass)
            {
                case 15:
                    if (currentClass == 11)
                        SafeAddSpell((ushort)Enums.SkillIDs.CruelShade, new Spell(true) { ID = (ushort)Enums.SkillIDs.CruelShade });
                    SafeAddSpell((ushort)Enums.SkillIDs.Cyclone, new Spell(true) { ID = (ushort)Enums.SkillIDs.Cyclone });
                    SafeAddSpell((ushort)Enums.SkillIDs.Robot, new Spell(true) { ID = (ushort)Enums.SkillIDs.Robot });
                    SafeAddSpell((ushort)Enums.SkillIDs.SpiritHealing, new Spell(true) { ID = (ushort)Enums.SkillIDs.SpiritHealing });
                    break;
                case 25:
                    if (currentClass == 21)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.Reflect, new Spell(true) { ID = (ushort)Enums.SkillIDs.Reflect });
                        SafeAddSpell((ushort)Enums.SkillIDs.SuperMan, new Spell(true) { ID = (ushort)Enums.SkillIDs.SuperMan });
                    }
                    if (currentClass == 132)
                        SafeAddSpell((ushort)Enums.SkillIDs.SuperMan, new Spell(true) { ID = (ushort)Enums.SkillIDs.SuperMan });
                    if (currentClass == 142)
                        SafeAddSpell((ushort)Enums.SkillIDs.Shield, new Spell(true) { ID = (ushort)Enums.SkillIDs.Shield });
                    break;
                case 45:
                    break;
                case 55:
                    if (currentClass == 51)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.CannonBarrage, new Spell(true) { ID = (ushort)Enums.SkillIDs.CannonBarrage });
                        SafeAddSpell((ushort)Enums.SkillIDs.ArcherBane, new Spell(true) { ID = (ushort)Enums.SkillIDs.ArcherBane });
                        //SafeAddSpell((ushort)Enums.SkillIDs.PoisonStar, new Spell(true) { ID = (ushort)Enums.SkillIDs.PoisonStar });
                        SafeAddSpell((ushort)Enums.SkillIDs.TwofoldBlades, new Spell(true) { ID = (ushort)Enums.SkillIDs.TwofoldBlades });
                    }
                    SafeAddSpell((ushort)Enums.SkillIDs.ToxicFog, new Spell(true) { ID = (ushort)Enums.SkillIDs.ToxicFog });
                    break;
                case 65:
                    if (currentClass == 61)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.TripleAttack, new Spell(true) { ID = (ushort)Enums.SkillIDs.TripleAttack });
                        SafeAddSpell((ushort)Enums.SkillIDs.Tranquility, new Spell(true) { ID = (ushort)Enums.SkillIDs.Tranquility });
                        SafeAddSpell((ushort)Enums.SkillIDs.TyrantAura, new Spell(true) { ID = (ushort)Enums.SkillIDs.TyrantAura });
                        SafeAddSpell((ushort)Enums.SkillIDs.DeathBlow, new Spell(true) { ID = (ushort)Enums.SkillIDs.DeathBlow });
                        SafeAddSpell((ushort)Enums.SkillIDs.DeflectionAura, new Spell(true) { ID = (ushort)Enums.SkillIDs.DeflectionAura });
                        SafeAddSpell((ushort)Enums.SkillIDs.Compassion, new Spell(true) { ID = (ushort)Enums.SkillIDs.Compassion });
                        SafeAddSpell((ushort)Enums.SkillIDs.WhirlWindKick, new Spell(true) { ID = (ushort)Enums.SkillIDs.WhirlWindKick });
                        SafeAddSpell((ushort)Enums.SkillIDs.RadiantPalm, new Spell(true) { ID = (ushort)Enums.SkillIDs.RadiantPalm });
                    }
                    SafeAddSpell((ushort)Enums.SkillIDs.Serenity, new Spell(true) { ID = (ushort)Enums.SkillIDs.Serenity });
                    break;
                case 135:
                    if (currentClass == 132)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.Pervade, new Spell(true) { ID = (ushort)Enums.SkillIDs.Pervade });
                        SafeAddSpell((ushort)Enums.SkillIDs.Pray, new Spell(true) { ID = (ushort)Enums.SkillIDs.Pray });
                        SafeAddSpell((ushort)Enums.SkillIDs.AdvancedCure, new Spell(true) { ID = (ushort)Enums.SkillIDs.AdvancedCure });
                    }
                    if (currentClass == 142)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.XPRevive, new Spell(true) { ID = (ushort)Enums.SkillIDs.XPRevive });
                        SafeAddSpell((ushort)Enums.SkillIDs.AdvancedCure, new Spell(true) { ID = (ushort)Enums.SkillIDs.AdvancedCure });
                    }

                    SafeAddSpell((ushort)Enums.SkillIDs.Meditation, new Spell(true) { ID = (ushort)Enums.SkillIDs.Meditation });
                    SafeAddSpell((ushort)Enums.SkillIDs.Thunder, new Spell(true) { ID = (ushort)Enums.SkillIDs.Thunder });
                    SafeAddSpell((ushort)Enums.SkillIDs.Cure, new Spell(true) { ID = (ushort)Enums.SkillIDs.Cure });
                    SafeAddSpell((ushort)Enums.SkillIDs.Stigma, new Spell(true) { ID = (ushort)Enums.SkillIDs.Stigma });
                    SafeAddSpell((ushort)Enums.SkillIDs.MagicShield, new Spell(true) { ID = (ushort)Enums.SkillIDs.MagicShield });
                    SafeAddSpell((ushort)Enums.SkillIDs.StarOfAccuracy, new Spell(true) { ID = (ushort)Enums.SkillIDs.StarOfAccuracy });
                    break;
                case 145:
                    if (currentClass == 132)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.FireCircle, new Spell(true) { ID = (ushort)Enums.SkillIDs.FireCircle });
                    }
                    if (currentClass == 142)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.Fire, new Spell(true) { ID = (ushort)Enums.SkillIDs.Fire });
                        SafeAddSpell((ushort)Enums.SkillIDs.Dodge, new Spell(true) { ID = (ushort)Enums.SkillIDs.Dodge });
                    }

                    SafeAddSpell((ushort)Enums.SkillIDs.Meditation, new Spell(true) { ID = (ushort)Enums.SkillIDs.Meditation });
                    SafeAddSpell((ushort)Enums.SkillIDs.Thunder, new Spell(true) { ID = (ushort)Enums.SkillIDs.Thunder });
                    SafeAddSpell((ushort)Enums.SkillIDs.Cure, new Spell(true) { ID = (ushort)Enums.SkillIDs.Cure });
                    break;
            }

            return spells;
        }

        public Dictionary<ushort, Spell> SecondRebornSpells(int fClass, int sClass, int currentClass)
        {
            Dictionary<ushort, Spell> spells = new Dictionary<ushort, Spell>();

            if (sClass == 25)
                SafeAddSpell(3060, new Spell(true) { ID = 3060 });
            if (sClass == 15 && currentClass == 11)
                SafeAddSpell((ushort)Enums.SkillIDs.CruelShade, new Spell(true) { ID = (ushort)Enums.SkillIDs.CruelShade });
            if (sClass == 135 && currentClass == 132)
                SafeAddSpell((ushort)Enums.SkillIDs.Pervade, new Spell(true) { ID = (ushort)Enums.SkillIDs.Pervade });
            if (sClass == 145 && currentClass == 142)
                SafeAddSpell((ushort)Enums.SkillIDs.Dodge, new Spell(true) { ID = (ushort)Enums.SkillIDs.Dodge });

            switch (fClass)
            {
                case 15:
                    SafeAddSpell((ushort)Enums.SkillIDs.Cyclone, new Spell(true) { ID = (ushort)Enums.SkillIDs.Cyclone });
                    SafeAddSpell((ushort)Enums.SkillIDs.Robot, new Spell(true) { ID = (ushort)Enums.SkillIDs.Robot });
                    SafeAddSpell((ushort)Enums.SkillIDs.SpiritHealing, new Spell(true) { ID = (ushort)Enums.SkillIDs.SpiritHealing });
                    break;
                case 25:
                    if (currentClass == 21)
                        SafeAddSpell((ushort)Enums.SkillIDs.SuperMan, new Spell(true) { ID = (ushort)Enums.SkillIDs.SuperMan });
                    if (currentClass == 132)
                        SafeAddSpell((ushort)Enums.SkillIDs.SuperMan, new Spell(true) { ID = (ushort)Enums.SkillIDs.SuperMan });
                    if (currentClass == 142)
                        SafeAddSpell((ushort)Enums.SkillIDs.Shield, new Spell(true) { ID = (ushort)Enums.SkillIDs.Shield });
                    break;
                case 45:
                    break;
                case 55:
                    if (currentClass == 51)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.CannonBarrage, new Spell(true) { ID = (ushort)Enums.SkillIDs.CannonBarrage });
                        SafeAddSpell((ushort)Enums.SkillIDs.ArcherBane, new Spell(true) { ID = (ushort)Enums.SkillIDs.ArcherBane });
                        //SafeAddSpell((ushort)Enums.SkillIDs.PoisonStar, new Spell(true) { ID = (ushort)Enums.SkillIDs.PoisonStar });
                        SafeAddSpell((ushort)Enums.SkillIDs.TwofoldBlades, new Spell(true) { ID = (ushort)Enums.SkillIDs.TwofoldBlades });
                    }
                    SafeAddSpell((ushort)Enums.SkillIDs.ToxicFog, new Spell(true) { ID = (ushort)Enums.SkillIDs.ToxicFog });
                    break;
                case 65:
                    if (currentClass == 61)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.TripleAttack, new Spell(true) { ID = (ushort)Enums.SkillIDs.TripleAttack });
                        SafeAddSpell((ushort)Enums.SkillIDs.Tranquility, new Spell(true) { ID = (ushort)Enums.SkillIDs.Tranquility });
                        SafeAddSpell((ushort)Enums.SkillIDs.TyrantAura, new Spell(true) { ID = (ushort)Enums.SkillIDs.TyrantAura });
                        SafeAddSpell((ushort)Enums.SkillIDs.DeathBlow, new Spell(true) { ID = (ushort)Enums.SkillIDs.DeathBlow });
                        SafeAddSpell((ushort)Enums.SkillIDs.DeflectionAura, new Spell(true) { ID = (ushort)Enums.SkillIDs.DeflectionAura });
                        SafeAddSpell((ushort)Enums.SkillIDs.Compassion, new Spell(true) { ID = (ushort)Enums.SkillIDs.Compassion });
                        SafeAddSpell((ushort)Enums.SkillIDs.WhirlWindKick, new Spell(true) { ID = (ushort)Enums.SkillIDs.WhirlWindKick });
                        SafeAddSpell((ushort)Enums.SkillIDs.RadiantPalm, new Spell(true) { ID = (ushort)Enums.SkillIDs.RadiantPalm });
                    }
                    SafeAddSpell((ushort)Enums.SkillIDs.Serenity, new Spell(true) { ID = (ushort)Enums.SkillIDs.Serenity });
                    break;
                case 135:
                    if (sClass == 135 && currentClass == 132)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.Pervade, new Spell(true) { ID = (ushort)Enums.SkillIDs.Pervade });
                        SafeAddSpell((ushort)Enums.SkillIDs.Pray, new Spell(true) { ID = (ushort)Enums.SkillIDs.Pray });
                        SafeAddSpell((ushort)Enums.SkillIDs.AdvancedCure, new Spell(true) { ID = (ushort)Enums.SkillIDs.AdvancedCure });
                    }
                    if (sClass == 135 && currentClass == 142)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.XPRevive, new Spell(true) { ID = (ushort)Enums.SkillIDs.XPRevive });
                        SafeAddSpell((ushort)Enums.SkillIDs.AdvancedCure, new Spell(true) { ID = (ushort)Enums.SkillIDs.AdvancedCure });
                    }

                    SafeAddSpell((ushort)Enums.SkillIDs.DivineHare, new Spell(true) { ID = (ushort)Enums.SkillIDs.DivineHare });
                    SafeAddSpell((ushort)Enums.SkillIDs.Meditation, new Spell(true) { ID = (ushort)Enums.SkillIDs.Meditation });
                    SafeAddSpell((ushort)Enums.SkillIDs.Thunder, new Spell(true) { ID = (ushort)Enums.SkillIDs.Thunder });
                    SafeAddSpell((ushort)Enums.SkillIDs.Cure, new Spell(true) { ID = (ushort)Enums.SkillIDs.Cure });
                    SafeAddSpell((ushort)Enums.SkillIDs.Stigma, new Spell(true) { ID = (ushort)Enums.SkillIDs.Stigma });
                    SafeAddSpell((ushort)Enums.SkillIDs.MagicShield, new Spell(true) { ID = (ushort)Enums.SkillIDs.MagicShield });
                    SafeAddSpell((ushort)Enums.SkillIDs.StarOfAccuracy, new Spell(true) { ID = (ushort)Enums.SkillIDs.StarOfAccuracy });
                    break;
                case 145:
                    if (sClass == 145 && currentClass == 132)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.FireCircle, new Spell(true) { ID = (ushort)Enums.SkillIDs.FireCircle });
                    }
                    if (sClass == 145 && currentClass == 142)
                    {
                        SafeAddSpell((ushort)Enums.SkillIDs.Fire, new Spell(true) { ID = (ushort)Enums.SkillIDs.Fire });
                        SafeAddSpell((ushort)Enums.SkillIDs.Dodge, new Spell(true) { ID = (ushort)Enums.SkillIDs.Dodge });
                    }

                    SafeAddSpell((ushort)Enums.SkillIDs.Meditation, new Spell(true) { ID = (ushort)Enums.SkillIDs.Meditation });
                    SafeAddSpell((ushort)Enums.SkillIDs.Thunder, new Spell(true) { ID = (ushort)Enums.SkillIDs.Thunder });
                    SafeAddSpell((ushort)Enums.SkillIDs.Cure, new Spell(true) { ID = (ushort)Enums.SkillIDs.Cure });
                    break;
            }

            return spells;
        }
        public void SafeAddSpell(ushort id, Spell s)
        {
            if (!this.Spells.ContainsKey(id))
                Spells.Add(id, s);
        }
        #endregion
        public bool Reborn(byte toClass)
        {
            #region Items
            if (Inventory.Count > 38)
                return false;
            switch (toClass)
            {
                case 11:
                case 21:
                    {
                        Inventory.Add(410077, Game.Enums.ItemEffect.Poison);
                        break;
                    }
                case 41:
                    {
                        Inventory.Add(500057, Game.Enums.ItemEffect.Shield);
                        break;
                    }
                case 132:
                case 142:
                    {
                        if (toClass == 132)
                            Inventory.Add(421077, Game.Enums.ItemEffect.MP);
                        else
                            Inventory.Add(421077, Game.Enums.ItemEffect.HP);
                        break;
                    }
            }
            #region Low level items
            for (byte i = 1; i < 9; i++)
            {
                if (i != 7)
                {
                    Interfaces.IConquerItem item = Equipment.TryGetItem(i);
                    if (item != null && item.ID != 0)
                    {
                        try
                        {
                            //UnloadItemStats(item, false);
                            Database.ConquerItemInformation cii = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                            item.ID = cii.LowestID(Network.PacketHandler.ItemMinLevel(Network.PacketHandler.ItemPosition(item.ID)));
                            item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            item.Send(this);
                            LoadItemStats(item);
                            Database.ConquerItemTable.UpdateItemID(item, this);
                        }
                        catch
                        {
                            Console.WriteLine("Reborn item problem: " + item.ID);
                        }
                    }
                }
            }
            Interfaces.IConquerItem hand = Equipment.TryGetItem(5);
            if (hand != null)
            {
                Equipment.Remove(5);
                CalculateStatBonus();
                CalculateHPBonus();
            }
            else
                SendScreen(Entity.SpawnPacket, false);
            #endregion
            #endregion
            if (Entity.Reborn == 0)
            {
                Entity.FirstRebornClass = Entity.Class;
                Entity.FirstRebornLevel = Entity.Level;
                Entity.Atributes =
                    (ushort)(ExtraAtributePoints(Entity.FirstRebornClass, Entity.FirstRebornLevel) + 52);
            }
            else
            {
                Entity.SecondRebornClass = Entity.Class;
                Entity.SecondRebornLevel = Entity.Level;
                Entity.Atributes =
                    (ushort)(ExtraAtributePoints(Entity.FirstRebornClass, Entity.FirstRebornLevel) +
                    ExtraAtributePoints(Entity.SecondRebornClass, Entity.SecondRebornLevel) + 62);
            }
            byte PreviousClass = Entity.Class;
            Entity.Reborn++;
            Entity.Class = toClass;
            Entity.Level = 15;
            Entity.Experience = 0;
            #region Spells
            Interfaces.ISkill[] spells = Spells.Values.ToArray();
            foreach (Interfaces.ISkill spell in spells)
            {
                spell.PreviousLevel = spell.Level;
                spell.Level = 0;
                spell.Experience = 0;
                #region Monk
                if (PreviousClass == 65)
                {
                    if (Entity.Class != 61)
                    {
                        switch (spell.ID)
                        {
                            case 10490:
                            case 10415:
                            case 10381:
                                RemoveSpell(spell);
                                break;
                        }
                    }
                }
                #endregion
                #region Warrior
                if (PreviousClass == 25)
                {
                    if (Entity.Class != 21)
                    {
                        switch (spell.ID)
                        {
                            case 1025:
                                if (Entity.Class != 21 && Entity.Class != 132)
                                    RemoveSpell(spell);
                                break;
                        }
                    }
                }
                #endregion
                #region Ninja
                if (toClass != 51)
                {
                    switch (spell.ID)
                    {
                        case 6010:
                        case 6000:
                        case 6011:
                            RemoveSpell(spell);
                            break;
                    }
                }
                #endregion
                #region Trojan
                if (toClass != 11)
                {
                    switch (spell.ID)
                    {
                        case 1115:
                            RemoveSpell(spell);
                            break;
                    }
                }
                #endregion
                #region Archer
                if (toClass != 41)
                {
                    switch (spell.ID)
                    {
                        case 8001:
                        case 8000:
                        case 8003:
                        case 9000:
                        case 8002:
                        case 8030:
                            RemoveSpell(spell);
                            break;
                    }
                }
                #endregion
                #region WaterTaoist
                if (PreviousClass == 135)
                {
                    if (toClass != 132)
                    {
                        switch (spell.ID)
                        {
                            case 1000:
                            case 1001:
                            case 1010:
                            case 1125:
                            case 1100:
                            case 8030:
                                RemoveSpell(spell);
                                break;
                            case 1050:
                            case 1175:
                            case 1170:
                                if (toClass != 142)
                                    RemoveSpell(spell);
                                break;
                        }
                    }
                }
                #endregion
                #region FireTaoist
                if (PreviousClass == 145)
                {
                    if (toClass != 142)
                    {
                        switch (spell.ID)
                        {
                            case 1000:
                            case 1001:
                            case 1150:
                            case 1180:
                            case 1120:
                            case 1002:
                            case 1160:
                            case 1165:
                                RemoveSpell(spell);
                                break;
                        }
                    }
                }
                #endregion
                if (Spells.ContainsKey(spell.ID))
                    if (spell.ID != (ushort)Game.Enums.SkillIDs.Reflect)
                        spell.Send(this);
            }
            #endregion
            //#region Proficiencies
            //foreach (Interfaces.IProf proficiency in Proficiencies.Values)
            //{
            //    proficiency.PreviousLevel = proficiency.Level;
            //    proficiency.Level = 0;
            //    proficiency.Experience = 0;
            //    proficiency.Send(this);
            //}
            //#endregion
            #region Adding earned skills
            if (Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 9876 });
            if (toClass == 51 && PreviousClass == 55 && Entity.Reborn == 1)
                AddSpell(new Spell(true) { ID = 6002 });
            if (Entity.FirstRebornClass == 15 && Entity.SecondRebornClass == 15 && Entity.Class == 11 && Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10315 });
            if (Entity.FirstRebornClass == 25 && Entity.SecondRebornClass == 25 && Entity.Class == 21 && Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10311 });
            if (Entity.FirstRebornClass == 45 && Entity.SecondRebornClass == 45 && Entity.Class == 41 && Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10313 });
            if (Entity.FirstRebornClass == 55 && Entity.SecondRebornClass == 55 && Entity.Class == 51 && Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 6003 });
            if (Entity.FirstRebornClass == 65 && Entity.SecondRebornClass == 65 && Entity.Class == 61 && Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10405 });
            if (Entity.FirstRebornClass == 135 && Entity.SecondRebornClass == 135 && Entity.Class == 131 && Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 30000 });
            if (Entity.FirstRebornClass == 145 && Entity.SecondRebornClass == 145 && Entity.Class == 141 && Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10310 });
            if (Entity.Reborn == 1)
            {
                if (Entity.FirstRebornClass == 15 && Entity.Class == 11)
                {
                    AddSpell(new Spell(true) { ID = 3050 });
                }
                else if (Entity.FirstRebornClass == 25 && Entity.Class == 21)
                {
                    AddSpell(new Spell(true) { ID = 3060 });
                }
                else if (Entity.FirstRebornClass == 145 && Entity.Class == 142)
                {
                    AddSpell(new Spell(true) { ID = 3080 });
                }
                else if (Entity.FirstRebornClass == 135 && Entity.Class == 132)
                {
                    AddSpell(new Spell(true) { ID = 3090 });
                }
            }
            if (Entity.Reborn == 2)
            {
                if (Entity.SecondRebornClass == 15 && Entity.Class == 11)
                {
                    AddSpell(new Spell(true) { ID = 3050 });
                }
                else if (Entity.SecondRebornClass == 25)
                {
                    AddSpell(new Spell(true) { ID = 3060 });
                }
                else if (Entity.SecondRebornClass == 145 && Entity.Class == 142)
                {
                    AddSpell(new Spell(true) { ID = 3080 });
                }
                else if (Entity.SecondRebornClass == 135 && Entity.Class == 132)
                {
                    AddSpell(new Spell(true) { ID = 3090 });
                }
            }
            #endregion

            Database.DataHolder.GetStats(Entity.Class, Entity.Level, this);
            CalculateStatBonus();
            CalculateHPBonus();
            GemAlgorithm();
            ServerBase.Kernel.SendWorldMessage(new Message("" + Entity.Name + " has got " + Entity.Reborn + " reborns. Congratulations!", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
            return true;
        }
        #region Items
        private int StatHP;
        public bool Agreed;
        public void CalculateHPBonus()
        {
            switch (Entity.Class)
            {
                case 11: Entity.MaxHitpoints = (uint)(StatHP * 1.12F); break;
                case 12: Entity.MaxHitpoints = (uint)(StatHP * 1.14F); break;
                case 13: Entity.MaxHitpoints = (uint)(StatHP * 1.16F); break;
                case 14: Entity.MaxHitpoints = (uint)(StatHP * 1.18F); break;
                case 15: Entity.MaxHitpoints = (uint)(StatHP * 1.20F); break;
                default: Entity.MaxHitpoints = (uint)StatHP; break;
            }
            Entity.MaxHitpoints += Entity.ItemHP;
            Entity.Hitpoints = Math.Min(Entity.Hitpoints, Entity.MaxHitpoints);
        }
        public void CalculateStatBonus()
        {
            byte ManaBoost = 5;
            const byte HitpointBoost = 30;
            sbyte Class = (sbyte)(Entity.Class / 10);
            if (Class == 13 || Class == 14)
                ManaBoost += (byte)(5 * (Entity.Class - (Class * 10)));
            StatHP = (ushort)((Entity.Strength * 3) +
                                     (Entity.Agility * 3) +
                                     (Entity.Spirit * 3) +
                                     (Entity.Vitality * HitpointBoost));
            Entity.MaxMana = (ushort)((Entity.Spirit * ManaBoost) + Entity.ItemMP);
            Entity.Mana = Math.Min(Entity.Mana, Entity.MaxMana);
        }
        public void SendStatMessage()
        {
            //this.ReviewMentor();
            //Network.GamePackets.Message Msg = new Conquer_Online_Server.Network.GamePackets.Message(" Attack: {0}~{1} MagicAttack: {2} Defence: {3} MagicDefence: {4} Dodge: {5} \n DecreaseDamage: {6} {7} IncreaseDamage: {8} {9} Hitpoints: {10} {11} Mana: {12} {13} BP: {14}", System.Drawing.Color.Coral
            //    , Network.GamePackets.Message.Center);
            //Msg.__Message = string.Format(Msg.__Message,
            //    new object[] { Entity.MinAttack, Entity.MaxAttack, Entity.MagicAttack, Entity.Defence, (Entity.MagicDefence + Entity.MagicDefence), Entity.Dodge, Entity.PhysicalDamageDecrease, Entity.MagicDamageDecrease, Entity.PhysicalDamageIncrease, Entity.MagicDamageIncrease, Entity.Hitpoints, Entity.MaxHitpoints, Entity.Mana, Entity.MaxMana, Entity.BattlePower });
            //this.Send(Msg);
        }
        public void LoadItemStats(Interfaces.IConquerItem item)
        {
            ushort immunity = 0;
            ushort intensification = 0;
            ushort detoxication = 0;
            ushort breaktrough = 0;
            ushort penetration = 0;
            ushort criticalstrike = 0;
            ushort skillcriticalstrike = 0;
            ushort counteraction = 0;
            ushort block = 0;
            if (item == null)
                return;
            if (item.Durability == 0)
                return;
            Database.ConquerItemInformation Infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);

            if (Infos.BaseInformation == null)
                return;

            if (item.Purification.PurificationItemID != 0)
                LoadSoulStats(item.Purification.PurificationItemID);

            if (item.Position == ConquerItem.Tower)
            {
                Entity.PhysicalDamageDecrease += Infos.BaseInformation.PhysicalDefence;
                Entity.MagicDamageDecrease += Infos.BaseInformation.MagicDefence;
            }
            else
            {
                Entity.Defence += Infos.BaseInformation.PhysicalDefence;
                Entity.MagicDefencePercent += Infos.BaseInformation.MagicDefence;
                Entity.Dodge += (byte)Infos.BaseInformation.Dodge;
                if (item.Position != ConquerItem.Fan)
                    Entity.BaseMagicAttack += Infos.BaseInformation.MagicAttack;
            }

            Entity.ItemHP += Infos.BaseInformation.ItemHP;
            Entity.ItemMP += Infos.BaseInformation.ItemMP;
            Entity.ItemBless += item.Bless;
            if (item.Position == ConquerItem.RightWeapon)
            {
                Entity.AttackRange += Infos.BaseInformation.AttackRange;
                if (Network.PacketHandler.IsTwoHand(Infos.BaseInformation.ID))
                    Entity.AttackRange += 3;
                else
                {
                    Entity.AttackRange += 2;
                }
            }
            if (item.Position == ConquerItem.LeftWeapon)
            {
                Entity.BaseMinAttack += (uint)(Infos.BaseInformation.MinAttack * 0.5F);
                Entity.BaseMaxAttack += (uint)(Infos.BaseInformation.MaxAttack * 0.5F);
            }
            else if (item.Position == ConquerItem.Fan)
            {
                Entity.PhysicalDamageIncrease += Infos.BaseInformation.MinAttack;
                Entity.MagicDamageIncrease += Infos.BaseInformation.MagicAttack;
            }
            else
            {
                Entity.BaseMinAttack += Infos.BaseInformation.MinAttack;
                Entity.BaseMaxAttack += Infos.BaseInformation.MaxAttack;
            }
            if (item.Plus != 0)
            {
                if (item.Position == ConquerItem.Tower)
                {
                    Entity.PhysicalDamageDecrease += Infos.PlusInformation.PhysicalDefence;
                    Entity.MagicDamageDecrease += (ushort)Infos.PlusInformation.MagicDefence;
                }
                else if (item.Position == ConquerItem.Fan)
                {
                    Entity.PhysicalDamageIncrease += (ushort)Infos.PlusInformation.MinAttack;
                    Entity.MagicDamageIncrease += (ushort)Infos.PlusInformation.MagicAttack;
                }
                else
                {
                    if (item.Position == ConquerItem.Steed)
                        Entity.Vigor += Infos.PlusInformation.Agility;
                    Entity.BaseMinAttack += Infos.PlusInformation.MinAttack;
                    Entity.BaseMaxAttack += Infos.PlusInformation.MaxAttack;
                    Entity.BaseMagicAttack += Infos.PlusInformation.MagicAttack;
                    Entity.Defence += Infos.PlusInformation.PhysicalDefence;
                    Entity.MagicDefence += Infos.PlusInformation.MagicDefence;
                    Entity.ItemHP += Infos.PlusInformation.ItemHP;
                    if (item.Position == ConquerItem.Boots)
                        Entity.Dodge += (byte)Infos.PlusInformation.Dodge;
                }
            }
            byte socketone = (byte)item.SocketOne;
            byte sockettwo = (byte)item.SocketTwo;
            ushort madd = 0, dadd = 0, aatk = 0, matk = 0;
            //if (item.Position != ConquerItem.Garment &&
            //    item.Position != ConquerItem.Bottle &&
            //    item.Position != ConquerItem.Steed)
                if (item.Position != ConquerItem.LeftWeaponAccessory &&
         item.Position != ConquerItem.RightWeaponAccessory)
                switch (socketone)
                {
                    case 1: Entity.Gems[0] += 5; break;
                    case 2: Entity.Gems[0] += 10; break;
                    case 3: Entity.Gems[0] += 15; break;

                    case 11: Entity.Gems[1] += 5; break;
                    case 12: Entity.Gems[1] += 10; break;
                    case 13: Entity.Gems[1] += 15; break;

                    case 31: Entity.Gems[3] += 10; break;
                    case 32: Entity.Gems[3] += 15; break;
                    case 33: Entity.Gems[3] += 25; break;

                    case 51: Entity.Gems[5] += 30; break;
                    case 52: Entity.Gems[5] += 50; break;
                    case 53: Entity.Gems[5] += 100; break;

                    case 61: Entity.Gems[6] += 15; break;
                    case 62: Entity.Gems[6] += 30; break;
                    case 63: Entity.Gems[6] += 50; break;

                    case 101: aatk = matk += 100; break;
                    case 102: aatk = matk += 300; break;
                    case 103: aatk = matk += 500; break;

                    case 121: madd = dadd += 100; break;
                    case 122: madd = dadd += 300; break;
                    case 123: madd = dadd += 500; break;
                }
            //if (item.Position != ConquerItem.Garment &&
            //     item.Position != ConquerItem.Bottle &&
            //     item.Position != ConquerItem.Steed)
                if (item.Position != ConquerItem.LeftWeaponAccessory &&
      item.Position != ConquerItem.RightWeaponAccessory)
                switch (sockettwo)
                {
                    case 1: Entity.Gems[0] += 5; break;
                    case 2: Entity.Gems[0] += 10; break;
                    case 3: Entity.Gems[0] += 15; break;

                    case 11: Entity.Gems[1] += 5; break;
                    case 12: Entity.Gems[1] += 10; break;
                    case 13: Entity.Gems[1] += 15; break;

                    case 31: Entity.Gems[3] += 10; break;
                    case 32: Entity.Gems[3] += 15; break;
                    case 33: Entity.Gems[3] += 25; break;

                    case 51: Entity.Gems[5] += 30; break;
                    case 52: Entity.Gems[5] += 50; break;
                    case 53: Entity.Gems[5] += 100; break;

                    case 61: Entity.Gems[6] += 15; break;
                    case 62: Entity.Gems[6] += 30; break;
                    case 63: Entity.Gems[6] += 50; break;

                    case 101: aatk = matk += 100; break;
                    case 102: aatk = matk += 300; break;
                    case 103: aatk = matk += 500; break;

                    case 121: madd = dadd += 100; break;
                    case 122: madd = dadd += 300; break;
                    case 123: madd = dadd += 500; break;
                }
            Entity.PhysicalDamageDecrease += dadd;
            Entity.MagicDamageDecrease += madd;
            Entity.PhysicalDamageIncrease += aatk;
            Entity.MagicDamageIncrease += matk;
            #region Refinery Parts
            switch ((Game.Features.Refinery.RefineryID)item.RefineryPart)
            {
                #region Detoxication
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Detoxication:
                    detoxication += item.RefineryPercent;
                    break;
                #endregion
                #region CriticalStrike //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Critical:
                    //criticalstrike += item.RefineryPercent;
                    criticalstrike += (UInt16)((item.RefineryPercent * 100));
                    break;
                #endregion
                #region Block //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Block://
                  //  block += item.RefineryPercent;
                    block += (UInt16)(item.RefineryPercent * 100);
                    break;
                #endregion
                #region Counteraction    / //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Counteraction:
                   // counteraction += item.RefineryPercent;
                    counteraction += (UInt16)(item.RefineryPercent * 10);
                    break;
                #endregion
                #region SkillCritical //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.SkillCritical:
                    // += item.RefineryPercent;
                   skillcriticalstrike += (UInt16)(item.RefineryPercent * 100);
                    break;
                #endregion
                #region Penetration //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Penetration:
                    penetration += (UInt16)(item.RefineryPercent * 100);
                    
                    break;
                #endregion
                #region Immunity //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Immunity:
                    //immunity += item.RefineryPercent;
                    immunity += (UInt16)(item.RefineryPercent * 100);
                    break;
                #endregion
                #region Intensification
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Intensificaiton:
                    intensification += item.RefineryPercent;
                    break;
                #endregion
                #region Breaktrough //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Breaktrough:
                  //  breaktrough += item.RefineryPercent;
                    breaktrough += (UInt16)((item.RefineryPercent * 10));
                    break;
                #endregion
                default: break;
            }
            #endregion
            #region Setting Statistics
            Entity.Statistics.Detoxication += detoxication;
           // Entity.Statistics.Intensification += intensification;
            Entity.Statistics.Immunity += immunity;
            Entity.Statistics.Penetration += penetration;
            Entity.Statistics.SkillCStrike += skillcriticalstrike;
            Entity.Statistics.CriticalStrike += criticalstrike;
            Entity.Statistics.Breaktrough += breaktrough;
            Entity.Statistics.Block += block;
            Entity.Statistics.Counteraction += counteraction;
            #endregion
            if (item.Position != ConquerItem.Garment &&
               item.Position != ConquerItem.Bottle)
            {
                Entity.ItemHP += item.Enchant;
                GemAlgorithm();
            }
            Send(Network.PacketHandler.WindowStats(this));
        }
        public void UnloadItemStats(Interfaces.IConquerItem item, bool onPurpose)
        {
            ushort immunity = 0;

            ushort intensification = 0;
            ushort detoxication = 0;
            ushort breaktrough = 0;
            ushort penetration = 0;
            ushort criticalstrike = 0;
            ushort skillcriticalstrike = 0;
            ushort counteraction = 0;
            ushort block = 0;
            if (item == null) return;
            if (item.Durability == 0 && !onPurpose)
                return;
            Database.ConquerItemInformation Infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
            if (Infos.BaseInformation == null)
                return;

            if (item.Purification.PurificationItemID != 0)
                UnloadSoulStats(item.Purification.PurificationItemID);

            if (item.Position == ConquerItem.Tower)
            {
                Entity.PhysicalDamageDecrease -= Infos.BaseInformation.PhysicalDefence;
                Entity.MagicDamageDecrease -= Infos.BaseInformation.MagicDefence;
            }
            else
            {
                Entity.Defence -= Infos.BaseInformation.PhysicalDefence;
                Entity.MagicDefencePercent -= Infos.BaseInformation.MagicDefence;
                Entity.Dodge -= (byte)Infos.BaseInformation.Dodge;
                if (item.Position != ConquerItem.Fan)
                    Entity.BaseMagicAttack -= Infos.BaseInformation.MagicAttack;
            }

            Entity.ItemHP -= Infos.BaseInformation.ItemHP;
            Entity.ItemMP -= Infos.BaseInformation.ItemMP;
            Entity.ItemBless -= item.Bless;
            if (item.Position == ConquerItem.RightWeapon)
            {
                Entity.AttackRange -= Infos.BaseInformation.AttackRange;
                if (Network.PacketHandler.IsTwoHand(Infos.BaseInformation.ID))
                    Entity.AttackRange -= 2;
            }
            if (item.Position == ConquerItem.LeftWeapon)
            {
                Entity.BaseMinAttack -= (uint)(Infos.BaseInformation.MinAttack * 0.5F);
                Entity.BaseMaxAttack -= (uint)(Infos.BaseInformation.MaxAttack * 0.5F);
            }
            else if (item.Position == ConquerItem.Fan)
            {
                Entity.PhysicalDamageIncrease -= Infos.BaseInformation.MinAttack;
                Entity.MagicDamageIncrease -= Infos.BaseInformation.MagicAttack;
            }
            else
            {
                Entity.BaseMinAttack -= Infos.BaseInformation.MinAttack;
                Entity.BaseMaxAttack -= Infos.BaseInformation.MaxAttack;
            }

            if (item.Plus != 0)
            {
                if (item.Position == ConquerItem.Tower)
                {
                    Entity.PhysicalDamageDecrease -= Infos.PlusInformation.PhysicalDefence;
                    Entity.MagicDamageDecrease -= (ushort)Infos.PlusInformation.MagicDefence;
                }
                else if (item.Position == ConquerItem.Fan)
                {
                    Entity.PhysicalDamageIncrease -= (ushort)Infos.PlusInformation.MinAttack;
                    Entity.MagicDamageIncrease -= (ushort)Infos.PlusInformation.MagicAttack;
                }
                else
                {
                    if (item.Position == ConquerItem.Steed)
                        Entity.Vigor -= Infos.PlusInformation.Agility;
                    Entity.BaseMinAttack -= Infos.PlusInformation.MinAttack;
                    Entity.BaseMaxAttack -= Infos.PlusInformation.MaxAttack;
                    Entity.BaseMagicAttack -= Infos.PlusInformation.MagicAttack;
                    Entity.Defence -= Infos.PlusInformation.PhysicalDefence;
                    Entity.MagicDefence -= Infos.PlusInformation.MagicDefence;
                    Entity.ItemHP -= Infos.PlusInformation.ItemHP;
                    if (item.Position == ConquerItem.Boots)
                        Entity.Dodge -= (byte)Infos.PlusInformation.Dodge;
                }
            }
            byte socketone = (byte)item.SocketOne;
            byte sockettwo = (byte)item.SocketTwo;
            ushort madd = 0, dadd = 0, aatk = 0, matk = 0;
            //if (item.Position != ConquerItem.Garment &&
            //    item.Position != ConquerItem.Bottle &&
            //    item.Position != ConquerItem.Steed)
            if (item.Position != ConquerItem.LeftWeaponAccessory &&
  item.Position != ConquerItem.RightWeaponAccessory)
                switch (socketone)
                {
                    case 1: Entity.Gems[0] -= 5; break;
                    case 2: Entity.Gems[0] -= 10; break;
                    case 3: Entity.Gems[0] -= 15; break;

                    case 11: Entity.Gems[1] -= 5; break;
                    case 12: Entity.Gems[1] -= 10; break;
                    case 13: Entity.Gems[1] -= 15; break;

                    case 31: Entity.Gems[3] -= 10; break;
                    case 32: Entity.Gems[3] -= 15; break;
                    case 33: Entity.Gems[3] -= 25; break;

                    case 51: Entity.Gems[5] -= 30; break;
                    case 52: Entity.Gems[5] -= 50; break;
                    case 53: Entity.Gems[5] -= 100; break;

                    case 61: Entity.Gems[6] -= 15; break;
                    case 62: Entity.Gems[6] -= 30; break;
                    case 63: Entity.Gems[6] -= 50; break;

                    case 101: aatk = matk += 100; break;
                    case 102: aatk = matk += 300; break;
                    case 103: aatk = matk += 500; break;

                    case 121: madd = dadd += 100; break;
                    case 122: madd = dadd += 300; break;
                    case 123: madd = dadd += 500; break;
                }
            //if (item.Position != ConquerItem.Garment &&
            //     item.Position != ConquerItem.Bottle &&
            //     item.Position != ConquerItem.Steed)
            if (item.Position != ConquerItem.LeftWeaponAccessory &&
  item.Position != ConquerItem.RightWeaponAccessory)
                switch (sockettwo)
                {
                    case 1: Entity.Gems[0] -= 5; break;
                    case 2: Entity.Gems[0] -= 10; break;
                    case 3: Entity.Gems[0] -= 15; break;

                    case 11: Entity.Gems[1] -= 5; break;
                    case 12: Entity.Gems[1] -= 10; break;
                    case 13: Entity.Gems[1] -= 15; break;

                    case 31: Entity.Gems[3] -= 10; break;
                    case 32: Entity.Gems[3] -= 15; break;
                    case 33: Entity.Gems[3] -= 25; break;

                    case 51: Entity.Gems[5] -= 30; break;
                    case 52: Entity.Gems[5] -= 50; break;
                    case 53: Entity.Gems[5] -= 100; break;

                    case 61: Entity.Gems[6] -= 15; break;
                    case 62: Entity.Gems[6] -= 30; break;
                    case 63: Entity.Gems[6] -= 50; break;

                    case 101: aatk = matk += 100; break;
                    case 102: aatk = matk += 300; break;
                    case 103: aatk = matk += 500; break;

                    case 121: madd = dadd += 100; break;
                    case 122: madd = dadd += 300; break;
                    case 123: madd = dadd += 500; break;
                }
            Entity.PhysicalDamageDecrease -= dadd;
            Entity.MagicDamageDecrease -= madd;
            Entity.PhysicalDamageIncrease -= aatk;
            Entity.MagicDamageIncrease -= matk;
            #region Refinery Parts
            switch ((Game.Features.Refinery.RefineryID)item.RefineryPart)
            {
                #region Detoxication
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Detoxication:
                    detoxication += item.RefineryPercent;
                    break;
                #endregion
                #region CriticalStrike //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Critical:
                    //criticalstrike += item.RefineryPercent;
                    criticalstrike += (UInt16)((item.RefineryPercent * 100));
                    break;
                #endregion
                #region Block //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Block://
                    //  block += item.RefineryPercent;
                    block += (UInt16)(item.RefineryPercent * 100);
                    break;
                #endregion
                #region Counteraction    / //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Counteraction:
                    // counteraction += item.RefineryPercent;
                    counteraction += (UInt16)(item.RefineryPercent * 10);
                    break;
                #endregion
                #region SkillCritical //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.SkillCritical:
                    // += item.RefineryPercent;
                    skillcriticalstrike += (UInt16)(item.RefineryPercent * 100);
                    break;
                #endregion
                #region Penetration //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Penetration:
                    penetration += (UInt16)(item.RefineryPercent * 100);

                    break;
                #endregion
                #region Immunity //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Immunity:
                    //immunity += item.RefineryPercent;
                    immunity += (UInt16)(item.RefineryPercent * 100);
                    break;
                #endregion
                #region Intensification
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Intensificaiton:
                    intensification += item.RefineryPercent;
                    break;
                #endregion
                #region Breaktrough //
                case Conquer_Online_Server.Game.Features.Refinery.RefineryID.Breaktrough:
                    //  breaktrough += item.RefineryPercent;
                    breaktrough += (UInt16)((item.RefineryPercent * 10));
                    break;
                #endregion
                default: break;
            }
            #endregion
            #region Setting Statistics
            Entity.Statistics.Detoxication -= detoxication;
           // Entity.Statistics.Intensification -= intensification;
            Entity.Statistics.Immunity -= immunity;
            Entity.Statistics.Penetration -= penetration;
            Entity.Statistics.SkillCStrike -= skillcriticalstrike;
            Entity.Statistics.CriticalStrike -= criticalstrike;
            Entity.Statistics.Breaktrough -= breaktrough;
            Entity.Statistics.Block -= block;
            Entity.Statistics.Counteraction -= counteraction;
            #endregion
            if (item.Position != ConquerItem.Garment &&
                item.Position != ConquerItem.Bottle)
            {
                Entity.ItemHP -= item.Enchant;
                GemAlgorithm();
            }
            Send(Network.PacketHandler.WindowStats(this));
        }
        public void LoadSoulStats(uint ID)
        {
            var Infos = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[ID];

            Entity.Defence += Infos.PhysicalDefence;
            Entity.MagicDefence += Infos.MagicDefence;
            Entity.Dodge += (byte)Infos.Dodge;
            Entity.BaseMagicAttack += Infos.MagicAttack;
            Entity.BaseMinAttack += Infos.MinAttack;
            Entity.BaseMaxAttack += Infos.MaxAttack;
            Entity.Statistics.CriticalStrike += Infos.Critical;
            Entity.Statistics.SkillCStrike += Infos.CriticalMag;
            Entity.Statistics.Immunity += Infos.Immunity;
            Entity.Statistics.Penetration += Infos.Penetration;
            Entity.Statistics.Block += Infos.BlockChance;
            Entity.Statistics.Breaktrough += Infos.Breakthrough;
            Entity.Statistics.Counteraction += Infos.Counter;
            Entity.Statistics.WoodResistance += Infos.WoodResist;
            Entity.Statistics.WaterResistance += Infos.WaterResist;
            Entity.Statistics.FireResistance += Infos.FireResist;
            Entity.Statistics.EarthResistance += Infos.EarthResist;

        }
        public void UnloadSoulStats(uint ID)
        {
            var Infos = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[ID];

            Entity.Defence -= Infos.PhysicalDefence;
            Entity.MagicDefence -= Infos.MagicDefence;
            Entity.Dodge -= (byte)Infos.Dodge;
            Entity.BaseMagicAttack -= Infos.MagicAttack;
            Entity.BaseMinAttack -= Infos.MinAttack;
            Entity.BaseMaxAttack -= Infos.MaxAttack;
            Entity.Statistics.CriticalStrike -= Infos.Critical;
            Entity.Statistics.SkillCStrike -= Infos.CriticalMag;
            Entity.Statistics.Immunity -= Infos.Immunity;
            Entity.Statistics.Penetration -= Infos.Penetration;
            Entity.Statistics.Block -= Infos.BlockChance;
            Entity.Statistics.Breaktrough -= Infos.Breakthrough;
            Entity.Statistics.Counteraction -= Infos.Counter;
            Entity.Statistics.WoodResistance -= Infos.WoodResist;
            Entity.Statistics.WaterResistance -= Infos.WaterResist;
            Entity.Statistics.FireResistance -= Infos.FireResist;
            Entity.Statistics.EarthResistance -= Infos.EarthResist;
        }
        public void GemAlgorithm()
        {
            Entity.MaxAttack = Entity.Strength + Entity.BaseMaxAttack;
            Entity.MinAttack = Entity.Strength + Entity.BaseMinAttack;
            Entity.MagicAttack = Entity.BaseMagicAttack;
            if (Entity.Gems[0] != 0)
            {
                Entity.MagicAttack += (uint)Math.Floor(Entity.MagicAttack * (double)(Entity.Gems[0] * 0.01));
            }
            if (Entity.Gems[1] != 0)
            {
                Entity.MaxAttack += (uint)Math.Floor(Entity.MaxAttack * (double)(Entity.Gems[1] * 0.01));
                Entity.MinAttack += (uint)Math.Floor(Entity.MinAttack * (double)(Entity.Gems[1] * 0.01));
            }
        }
        #endregion
        #endregion

        
    }
}
