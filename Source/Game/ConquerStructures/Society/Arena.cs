/*
 * The following code belongs to Ultimation and impulse.
 * Ultimation is credited for most of this piece of code.
 * Thanks to him, we have arena implented.
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Arena
    {
        public static SafeDictionary<uint, GameState> PlayerList = new SafeDictionary<uint, GameState>(10000);

        public static Dictionary<uint, ArenaStatistic> ArenaStatistics = new Dictionary<uint, ArenaStatistic>();
        public static SafeDictionary<uint, ArenaStatistic> YesterdayArenaStatistics = new SafeDictionary<uint, ArenaStatistic>(10000);

        public static List<ArenaStatistic> ArenaStatisticsList, YesterdayArenaStatisticsList;

        public static int InArenaListCount = 0;

        public static void Sort()
        {
            SortedDictionary<ulong, SortEntry<uint, ArenaStatistic>> sortDictionary = new SortedDictionary<ulong, SortEntry<uint, ArenaStatistic>>();
            foreach (ArenaStatistic info in ArenaStatistics.Values)
            {
                if (sortDictionary.ContainsKey(info.ArenaPoints))
                {
                    SortEntry<uint, ArenaStatistic> entry = sortDictionary[info.ArenaPoints];
                    entry.Values.Add(info.EntityID, info);
                }
                else
                {
                    SortEntry<uint, ArenaStatistic> entry = new SortEntry<uint, ArenaStatistic>();
                    entry.Values = new Dictionary<uint, ArenaStatistic>();
                    entry.Values.Add(info.EntityID, info);
                    sortDictionary.Add(info.ArenaPoints, entry);
                }
            }
            Dictionary<uint, ArenaStatistic> toReplace = new Dictionary<uint, ArenaStatistic>();
            Dictionary<uint, ArenaStatistic> addAtEnd = new Dictionary<uint, ArenaStatistic>();

            uint Place = 1;
            InArenaListCount = 0;
            foreach (KeyValuePair<ulong, SortEntry<uint, ArenaStatistic>> entries in sortDictionary.Reverse())
            {
                foreach (uint e in entries.Value.Values.Keys)
                {
                    if (ArenaStatistics[e].TodayBattles != 0)
                    {
                        ArenaStatistics[e].Rank = Place;
                        Place++;
                        InArenaListCount++;
                        toReplace.Add(e, ArenaStatistics[e]);
                    }
                    else
                    {
                        ArenaStatistics[e].Rank = 0;
                        addAtEnd.Add(e, ArenaStatistics[e]);
                    }
                }
            }
            foreach (var v in addAtEnd)
            {
                toReplace.Add(v.Key, v.Value);
            }
            ArenaStatistics = toReplace;
            ArenaStatisticsList = ArenaStatistics.Values.ToList();
        }

        public static void YesterdaySort()
        {
            SortedDictionary<ulong, SortEntry<uint, ArenaStatistic>> sortDictionary = new SortedDictionary<ulong, SortEntry<uint, ArenaStatistic>>();
            foreach (ArenaStatistic info in ArenaStatistics.Values)
            {
                if (sortDictionary.ContainsKey(info.LastSeasonArenaPoints))
                {
                    SortEntry<uint, ArenaStatistic> entry = sortDictionary[info.LastSeasonArenaPoints];
                    entry.Values.Add(info.EntityID, info);
                }
                else
                {
                    SortEntry<uint, ArenaStatistic> entry = new SortEntry<uint, ArenaStatistic>();
                    entry.Values = new Dictionary<uint, ArenaStatistic>();
                    entry.Values.Add(info.EntityID, info);
                    sortDictionary.Add(info.LastSeasonArenaPoints, entry);
                }
            }
            SafeDictionary<uint, ArenaStatistic> toReplace = new SafeDictionary<uint, ArenaStatistic>(10000);

            uint Place = 1;
            foreach (KeyValuePair<ulong, SortEntry<uint, ArenaStatistic>> entries in sortDictionary.Reverse())
            {
                foreach (uint e in entries.Value.Values.Keys)
                {
                    if (ArenaStatistics[e].YesterdayTotal != 0)
                    {
                        ArenaStatistics[e].LastSeasonRank = Place;
                        Place++;
                        toReplace.Add(e, ArenaStatistics[e]);
                    }
                }
            }

            YesterdayArenaStatistics = toReplace;
        }

        private static void SaveArenaStats()
        {
            foreach (ArenaStatistic stats in ArenaStatistics.Values)
                Database.ArenaTable.SaveArenaStatistics(stats);
        }

        public static class QualifierList
        {
            public static SafeDictionary<uint, QualifierGroup> Groups = new SafeDictionary<uint, QualifierGroup>(10000);
            public static ServerBase.Counter GroupCounter = new Conquer_Online_Server.ServerBase.Counter();
            public static byte[] BuildPacket(ushort page)
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);
                wtr.Write((ushort)0);
                wtr.Write((ushort)2206);
                wtr.Write((uint)page);
                wtr.Write((uint)Enums.ArenaIDs.QualifierList);
                wtr.Write((uint)Groups.Count);
                wtr.Write((uint)PlayerList.Count);
                wtr.Write((uint)0);
                page--;
                wtr.Write((uint)(Groups.Count - page));
                QualifierGroup[] GroupsList = Groups.Values.ToArray();
                for (int count = page; count < page + 6; count++)
                {
                    if (count >= Groups.Count)
                        break;

                    QualifierGroup entry = GroupsList[count];

                    wtr.Write((uint)entry.Player1.ArenaStatistic.EntityID);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.Model);
                    for (int i = 0; i < 16; i++)
                    {
                        if (i < entry.Player1.ArenaStatistic.Name.Length)
                        {
                            wtr.Write((byte)entry.Player1.ArenaStatistic.Name[i]);
                        }
                        else
                            wtr.Write((byte)0);
                    }

                    wtr.Write((uint)entry.Player1.ArenaStatistic.Level);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.Class);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.Rank);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.ArenaPoints);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.TodayWin);
                    wtr.Write((uint)(entry.Player1.ArenaStatistic.TodayBattles - entry.Player1.ArenaStatistic.TodayWin));
                    wtr.Write((uint)0);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.CurrentHonor);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.HistoryHonor);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.EntityID);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.Model);
                    for (int i = 0; i < 16; i++)
                    {
                        if (i < entry.Player2.ArenaStatistic.Name.Length)
                        {
                            wtr.Write((byte)entry.Player2.ArenaStatistic.Name[i]);
                        }
                        else
                            wtr.Write((byte)0);
                    }

                    wtr.Write((uint)entry.Player2.ArenaStatistic.Level);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.Class);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.Rank);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.ArenaPoints);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.TodayWin);
                    wtr.Write((uint)0);
                    wtr.Write((uint)(entry.Player1.ArenaStatistic.TodayBattles - entry.Player1.ArenaStatistic.TodayWin));
                    wtr.Write((uint)entry.Player2.ArenaStatistic.CurrentHonor);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.HistoryHonor);
                }
                GroupsList = null;
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(ASCIIEncoding.ASCII.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }

            public class QualifierGroup
            {
                #region Watchers
                public byte[] BuildWatcherList(List<Client.GameState> list, ushort id)
                {
                    MemoryStream strm = new MemoryStream();
                    BinaryWriter wtr = new BinaryWriter(strm);
                    wtr.Write((ushort)38);
                    wtr.Write((ushort)2211);
                    wtr.Write((ushort)id);
                    wtr.Write((uint)Player1.Entity.UID);
                    wtr.Write((uint)list.Count);
                    wtr.Write((uint)Player1Cheers);
                    wtr.Write((uint)Player2Cheers);
                    foreach (Client.GameState client in list)
                    {
                        wtr.Write((uint)client.Entity.UID);
                        for (int i = 0; i < 16; i++)
                        {
                            if (i < client.ArenaStatistic.Name.Length)
                            {
                                wtr.Write((byte)client.ArenaStatistic.Name[i]);
                            }
                            else
                                wtr.Write((byte)0);
                        }
                        if (client.Entity.UID == Player1.Entity.UID)
                            wtr.Write((uint)Player1Cheers);
                        else
                            wtr.Write((uint)Player2Cheers);
                    }
                    wtr.Write((uint)Player1Cheers);
                    wtr.Write((uint)Player2Cheers);
                    int packetlength = (int)strm.Length;
                    strm.Position = 0;
                    wtr.Write((ushort)packetlength);
                    strm.Position = strm.Length;
                    wtr.Write(ASCIIEncoding.ASCII.GetBytes("TQServer"));
                    strm.Position = 0;
                    byte[] buf = new byte[strm.Length];
                    strm.Read(buf, 0, buf.Length);
                    wtr.Close();
                    strm.Close();
                    return buf;
                }
                public void BeginWatching(Client.GameState client)
                {
                    if (client.WatchingGroup == null)
                    {
                        client.Send(BuildWatcherList(new List<GameState>(), 0));
                        client.Send(this.match.BuildPacket());

                        Watchers.Add(client);
                        byte[] packet = BuildWatcherList(Watchers, 2);
                        foreach (Client.GameState client2 in Watchers)
                            client2.Send(packet);
                        Player1.Send(packet);
                        Player2.Send(packet);
                        client.WatchingGroup = this;
                        client.Entity.Teleport(1005, dynamicMap.ID, (ushort)ServerBase.Kernel.Random.Next(35, 70), (ushort)ServerBase.Kernel.Random.Next(35, 70));
                    }
                }
                public List<Client.GameState> Watchers = new List<GameState>();
                #endregion

                public Time32 CreateTime;
                public Time32 DoneStamp;

                public uint Player1Damage, Player2Damage;
                public uint Player1Cheers, Player2Cheers;

                public bool Inside;
                public bool Done;
                public bool CanEnd;

                private Game.Enums.PKMode P1Mode, P2Mode;

                public uint ID;

                private GroupMatch match = new GroupMatch();

                public Client.GameState Winner, Loser;
                public Client.GameState Player1, Player2;

                private Map dynamicMap;
                public QualifierGroup(Client.GameState player1, Client.GameState player2)
                {
                    Player1 = player1;
                    Player2 = player2;
                    CreateTime = Time32.Now;
                    Inside = false;
                    Player1Damage = 0;
                    Player2Damage = 0;
                    Done = false;
                    ID = GroupCounter.Next;
                    match.Group = this;
                    CanEnd = false;
                    Done = false;
                    Inside = false;
                }

                public void Import()
                {
                    Player1.InArenaMatch = Player2.InArenaMatch = true;
                    Inside = true;
                    Player1.QualifierGroup = this;
                    Player2.QualifierGroup = this;
                    if (!ServerBase.Kernel.Maps.ContainsKey(700))
                        new Map(700, Database.DMaps.MapPaths[700]);
                    Map origMap = ServerBase.Kernel.Maps[700];
                    dynamicMap = origMap.MakeDynamicMap();
                    Player1.Entity.Teleport(origMap.ID, dynamicMap.ID, (ushort)ServerBase.Kernel.Random.Next(35, 70), (ushort)ServerBase.Kernel.Random.Next(35, 70));
                    Player2.Entity.Teleport(origMap.ID, dynamicMap.ID, (ushort)ServerBase.Kernel.Random.Next(35, 70), (ushort)ServerBase.Kernel.Random.Next(35, 70));
                    if (Player1.Map.ID == Player2.Map.ID)
                    {
                        ArenaSignup sign = new ArenaSignup();
                        sign.DialogID = ArenaSignup.MainIDs.StartTheFight;
                        sign.Stats = Player1.ArenaStatistic;
                        Player2.Send(sign.BuildPacket());
                        sign.Stats = Player2.ArenaStatistic;
                        Player1.Send(sign.BuildPacket());
                        sign.DialogID = ArenaSignup.MainIDs.Match;
                        sign.OptionID = ArenaSignup.DialogButton.MatchOn;
                        Player1.Send(sign.BuildPacket());
                        Player2.Send(sign.BuildPacket());
                        Player1.Send(match.BuildPacket());
                        Player2.Send(match.BuildPacket());
                        Player1.Entity.Ressurect();
                        Player2.Entity.Ressurect();
                        Player1.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                        Player2.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                        P1Mode = Player1.Entity.PKMode;
                        Player1.Entity.PKMode = Conquer_Online_Server.Game.Enums.PKMode.PK;
                        Player1.Send(new Data(true) { UID = Player1.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player1.Entity.PKMode });
                        P2Mode = Player2.Entity.PKMode;
                        Player2.Entity.PKMode = Conquer_Online_Server.Game.Enums.PKMode.PK;
                        Player2.Send(new Data(true) { UID = Player2.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player2.Entity.PKMode });

                        Player1.ArenaStatistic.TodayBattles++;
                        Player2.ArenaStatistic.TodayBattles++;
                    }
                    else
                        End();
                }

                public void Export()
                {
                    Player1.InArenaMatch = Player2.InArenaMatch = false;
                    var arr = Watchers.ToArray();
                    foreach (Client.GameState client in arr)
                        QualifyEngine.DoLeave(client);
                    arr = null;
                    Win(Winner, Loser);

                    Inside = false;
                    Player1.Entity.Teleport(Player1.Entity.PreviousMapID, Player1.Entity.PrevX, Player1.Entity.PrevY);
                    Player2.Entity.Teleport(Player2.Entity.PreviousMapID, Player2.Entity.PrevX, Player2.Entity.PrevY);
                    Loser.Entity.Ressurect();
                    Winner.Entity.Ressurect();
                    Player1.ArenaStatistic.AcceptBox = Player2.ArenaStatistic.AcceptBox = false;
                    Player1.ArenaStatistic.AcceptBoxShow = Player2.ArenaStatistic.AcceptBoxShow = Player2.ArenaStatistic.AcceptBoxShow.AddHours(2);
                    Player1.ArenaStatistic.PlayWith = Player2.ArenaStatistic.PlayWith = 0;

                    ArenaSignup sign = new ArenaSignup();
                    sign.DialogID = ArenaSignup.MainIDs.Dialog2;
                    Loser.Send(sign.BuildPacket());
                    sign.OptionID = ArenaSignup.DialogButton.Win;
                    Winner.Send(sign.BuildPacket());
                    Groups.Remove(ID);

                    Player1.Entity.PKMode = P1Mode;
                    Player1.Send(new Data(true) { UID = Player1.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player1.Entity.PKMode });
                    Player2.Entity.PKMode = P2Mode;
                    Player2.Send(new Data(true) { UID = Player2.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player2.Entity.PKMode });

                    Player1.QualifierGroup = null;
                    Player2.QualifierGroup = null;
                    if (dynamicMap != null)
                        dynamicMap.Dispose();
                }

                public void End()
                {
                    Player1.InArenaMatch = Player2.InArenaMatch = false;
                    if (Player1Damage > Player2Damage)
                    {
                        Winner = Player1;
                        Loser = Player2;
                    }
                    else
                    {
                        Winner = Player2;
                        Loser = Player1;
                    }
                    Done = true;
                    DoneStamp = Time32.Now;
                    CanEnd = true;
                }

                public void End(GameState loser)
                {
                    Player1.InArenaMatch = Player2.InArenaMatch = false;
                    if (Player1.Account.EntityID == loser.Account.EntityID)
                    {
                        Winner = Player2;
                        Loser = Player1;
                    }
                    else
                    {
                        Winner = Player1;
                        Loser = Player2;
                    }
                    Done = true;
                    DoneStamp = Time32.Now;
                    CanEnd = true;
                }

                public void End(GameState winner, GameState loser)
                {
                    Player1.InArenaMatch = Player2.InArenaMatch = false;
                    Winner = winner;
                    Loser = loser;
                    Done = true;
                    DoneStamp = Time32.Now;
                    CanEnd = true;
                }

                public void UpdateDamage(GameState client, uint damage)
                {
                    Player1.InArenaMatch = Player2.InArenaMatch = true;
                    if (client != null && Player1 != null)
                    {
                        if (client.Account.EntityID == Player1.Account.EntityID)
                        {
                            Player1Damage += damage;
                        }
                        else
                        {
                            Player2Damage += damage;
                        }
                        Player1.Send(match.BuildPacket());
                        Player2.Send(match.BuildPacket());
                    }
                }
            }
        }

        public class QualifyEngine
        {
            public static void RequestGroupList(Client.GameState client, ushort page)
            {
                client.Send(QualifierList.BuildPacket(page));
            }

            public static void DoQuit(Client.GameState client)
            {
                client.InArenaMatch = false;
                PlayerList.Remove(client.Account.EntityID);
                client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                client.Send(client.ArenaStatistic);
                RequestGroupList(client, 1);
                ArenaSignup sign = new ArenaSignup();
                sign.DialogID = ArenaSignup.MainIDs.OpponentGaveUp;
                if (PlayerList.ContainsKey(client.ArenaStatistic.PlayWith))
                {
                    PlayerList[client.ArenaStatistic.PlayWith].Send(sign.BuildPacket());
                    if (client.QualifierGroup != null)
                    {
                        if (client.QualifierGroup.Inside)
                        {
                            client.QualifierGroup.End(client);
                        }
                        else
                        {
                            if (client.ArenaStatistic.PlayWith != 0)
                                Win(PlayerList[client.ArenaStatistic.PlayWith], client);
                        }
                    }
                    else
                    {
                        if (client.ArenaStatistic.PlayWith != 0)
                            Win(PlayerList[client.ArenaStatistic.PlayWith], client);
                    }
                }
            }

            public static void DoSignup(Client.GameState client)
            {
                client.InArenaMatch = false;
                if (client.ArenaStatistic.ArenaPoints == 0)
                    return;
                if (client.Entity.MapID == 601) return;
                if (client.Map.BaseID == 6000 || client.Map.BaseID == 6001) return;
                if (!PlayerList.ContainsKey(client.Account.EntityID))
                {
                    PlayerList.Add(client.Account.EntityID, client);
                    client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingForOpponent;
                    client.Send(client.ArenaStatistic);
                    RequestGroupList(client, 1);
                }
                else
                {
                    client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                    client.Send(client.ArenaStatistic);
                    client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingForOpponent;
                    client.Send(client.ArenaStatistic);
                    RequestGroupList(client, 1);
                }
            }

            public static void DoGiveUp(Client.GameState client)
            {
                client.InArenaMatch = false;
                client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingInactive;
                client.Send(client.ArenaStatistic);
                RequestGroupList(client, 1);
                ArenaSignup sign = new ArenaSignup();
                sign.DialogID = ArenaSignup.MainIDs.OpponentGaveUp;
                if (PlayerList.ContainsKey(client.ArenaStatistic.PlayWith))
                {
                    PlayerList[client.ArenaStatistic.PlayWith].Send(sign.BuildPacket());
                    PlayerList[client.ArenaStatistic.PlayWith].ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                    client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                    PlayerList[client.ArenaStatistic.PlayWith].Send(PlayerList[client.ArenaStatistic.PlayWith].ArenaStatistic);
                    client.Send(client.ArenaStatistic);
                    if (client.QualifierGroup != null)
                    {
                        if (client.QualifierGroup.Inside)
                        {
                            client.QualifierGroup.End(client);
                        }
                        else
                        {
                            if (client.ArenaStatistic.PlayWith != 0)
                                Win(PlayerList[client.ArenaStatistic.PlayWith], client);
                        }
                    }
                    else
                    {
                        if (client.ArenaStatistic.PlayWith != 0)
                            Win(PlayerList[client.ArenaStatistic.PlayWith], client);
                    }
                }
            }

            public static void DoAccept(Client.GameState client)
            {
                if (!client.ArenaStatistic.AcceptBox)
                {
                    if (client.ArenaStatistic.ArenaPoints == 0)
                        return;

                    if (PlayerList.ContainsKey(client.ArenaStatistic.PlayWith))
                    {
                        if (PlayerList[client.ArenaStatistic.PlayWith].ArenaStatistic.AcceptBox)
                        {
                            QualifierList.QualifierGroup group = new QualifierList.QualifierGroup(client, PlayerList[client.ArenaStatistic.PlayWith]);
                            QualifierList.Groups.Add(group.ID, group);
                            group.Import();
                        }
                        else
                        {
                            client.ArenaStatistic.AcceptBox = true;
                        }
                    }
                }
            }

            public static void DoLeave(Client.GameState client)
            {
                client.InArenaMatch = false;
                if (client.WatchingGroup != null)
                {
                    client.WatchingGroup.Watchers.Remove(client);
                    byte[] packet = client.WatchingGroup.BuildWatcherList(client.WatchingGroup.Watchers, 2);
                    foreach (Client.GameState client2 in client.WatchingGroup.Watchers)
                        client2.Send(packet);
                    client.WatchingGroup.Player1.Send(packet);
                    client.WatchingGroup.Player2.Send(packet);
                    client.Send(client.WatchingGroup.BuildWatcherList(new List<Client.GameState>(), 3));
                    client.WatchingGroup = null;
                    client.Entity.Teleport(client.Entity.PreviousMapID, client.Entity.PrevX, client.Entity.PrevY);
                }
            }

            public static void DoCheer(Client.GameState client, string name)
            {
                if (client.WatchingGroup != null && !client.CheerSent)
                {
                    client.CheerSent = true;
                    if (client.WatchingGroup.Player1.Entity.Name == name)
                    {
                        client.WatchingGroup.Player1Cheers++;
                    }
                    else
                    {
                        client.WatchingGroup.Player2Cheers++;
                    }
                    byte[] packet = client.WatchingGroup.BuildWatcherList(client.WatchingGroup.Watchers, 2);
                    foreach (Client.GameState client2 in client.WatchingGroup.Watchers)
                        client2.Send(packet);
                    client.WatchingGroup.Player1.Send(packet);
                    client.WatchingGroup.Player2.Send(packet);
                }
            }
        }

        public class Statistics
        {
            public static void ShowWiners(Client.GameState client)
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);
                int MyCount = 0;
                wtr.Write((ushort)0);
                wtr.Write((ushort)2208);
                wtr.Write((uint)Enums.ArenaIDs.ShowPlayerRankList);
                foreach (ArenaStatistic entry in YesterdayArenaStatistics.Values)
                {
                    if (entry.LastSeasonArenaPoints != 0)
                    {
                        if (entry.LastSeasonRank != 0)
                        {
                            MyCount++;
                            wtr.Write((uint)entry.EntityID);

                            for (int i = 0; i < 16; i++)
                            {
                                if (i < entry.Name.Length)
                                {
                                    wtr.Write((byte)entry.Name[i]);
                                }
                                else
                                    wtr.Write((byte)0);
                            }
                            wtr.Write((uint)entry.Model);
                            wtr.Write((uint)entry.Level);
                            wtr.Write((uint)entry.Class);
                            wtr.Write((uint)entry.LastSeasonRank);
                            wtr.Write((uint)entry.LastSeasonRank);
                            wtr.Write((uint)entry.LastSeasonArenaPoints);
                            wtr.Write((uint)entry.LastSeasonWin);
                            wtr.Write((uint)entry.LastSeasonLose);
                        }
                    }
                    if (MyCount == 11)
                        break;
                }
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(ASCIIEncoding.ASCII.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                client.Send(buf);
            }

            public static void ShowRankingPage(ushort thisSeason, int pageIndex, Client.GameState client)
            {
                ArenaList list = new ArenaList(pageIndex);
                list.ID = Enums.ArenaIDs.ShowPlayerRankList;
                list.PageNumber = (ushort)pageIndex;
                list.Subtype = thisSeason;
                if (list.Subtype == 0)
                {
                    var Array = ArenaStatisticsList;
                    if (Array.Count > (((pageIndex) * 10) - 10))
                    {
                        list.Players.Clear();
                        for (int i = ((pageIndex) * 10 - 10); i < ((pageIndex) * 10 - 10) + 10; i++)
                        {
                            if (i < Array.Count)
                            {
                                if (Array[i].Rank > 0)
                                {
                                    list.Players.Add(Array[i]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (YesterdayArenaStatisticsList == null || YesterdayArenaStatisticsList.Count == 0)
                        YesterdayArenaStatisticsList = YesterdayArenaStatistics.Values.ToList();
                    var Array = YesterdayArenaStatisticsList;
                    if (Array.Count > (((pageIndex) * 10) - 10))
                    {
                        list.Players.Clear();
                        for (int i = ((pageIndex) * 10 - 10); i < ((pageIndex) * 10 - 10) + 10; i++)
                        {
                            if (i < Array.Count)
                            {
                                if (Array[i].LastSeasonRank > 0)
                                {
                                    list.Players.Add(Array[i]);
                                }
                            }
                        }
                    }
                }
                client.Send(list.BuildPacket());
            }
        }

        class ArenaList
        {
            public ushort Size;
            public ushort Type;
            public ushort Subtype;
            public ushort PageNumber;
            public Enums.ArenaIDs ID;
            public List<ArenaStatistic> Players = new List<ArenaStatistic>();
            public ArenaList(int PageIndex)
            {
                Type = 2207;
                PageNumber = (ushort)PageIndex;
            }
            public ArenaList(byte[] Packet)
            {
                BinaryReader rdr = new BinaryReader(new MemoryStream(Packet));
                Size = rdr.ReadUInt16();
                Type = rdr.ReadUInt16();
                Subtype = rdr.ReadUInt16();
                PageNumber = rdr.ReadUInt16();
            }
            public ArenaList()
            {
                Type = 2207;
            }
            public byte[] BuildPacket()
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);

                wtr.Write((ushort)0);
                wtr.Write((ushort)Type);
                wtr.Write((ushort)Subtype);
                wtr.Write((ushort)PageNumber);
                if (Subtype == 0)
                    wtr.Write((uint)InArenaListCount);
                else
                    wtr.Write((uint)YesterdayArenaStatistics.Count);
                wtr.Write((uint)ID);
                foreach (ArenaStatistic entry in Players)
                {
                    if (Subtype == 0)
                        wtr.Write((ushort)entry.Rank);
                    else
                        wtr.Write((ushort)entry.LastSeasonRank);

                    for (int i = 0; i < 16; i++)
                    {
                        if (i < entry.Name.Length)
                        {
                            wtr.Write((byte)entry.Name[i]);
                        }
                        else
                            wtr.Write((byte)0);
                    }
                    if (Subtype == 1)
                        wtr.Write((ushort)6004);
                    else
                        wtr.Write((ushort)0);
                    if (Subtype == 1)
                        wtr.Write((uint)entry.CurrentHonor);
                    else
                        wtr.Write((uint)entry.ArenaPoints);

                    wtr.Write((uint)entry.Class);
                    wtr.Write((uint)entry.Level);

                    if (Subtype == 1)
                        wtr.Write((uint)33942209);
                    else
                        wtr.Write((uint)1);
                }
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(ASCIIEncoding.ASCII.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }
        }

        public class ArenaSignup
        {
            public abstract class MainIDs
            {
                public const uint ArenaIconOn = 0,
                                    ArenaIconOff = 1,
                                    StartCountDown = 2,
                                    OpponentGaveUp = 4,
                                    Match = 6,
                                    YouAreKicked = 7,
                                    StartTheFight = 8,
                                    Dialog = 9,
                                    Dialog2 = 10;
            }
            public abstract class DialogButton
            {
                public const uint Lose = 3,
                                    Win = 1,
                                    MatchOff = 3,
                                    MatchOn = 5;
            }


            public ushort Type = 2205;
            public uint DialogID;
            public uint OptionID;
            public ArenaStatistic Stats;
            public byte[] BuildPacket()
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);
                if (Stats == null)
                {
                    Stats = new ArenaStatistic(true);
                    Stats.Name = "";
                }
                wtr.Write((ushort)0);
                wtr.Write((ushort)Type);
                wtr.Write((uint)DialogID);
                wtr.Write((uint)OptionID);
                wtr.Write((uint)Stats.EntityID);
                for (int i = 0; i < 20; i++)
                {
                    if (i < Stats.Name.Length)
                    {
                        wtr.Write((byte)Stats.Name[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }
                wtr.Write((uint)Stats.Class);
                wtr.Write((uint)Stats.Rank);
                wtr.Write((uint)Stats.ArenaPoints);
                wtr.Write((uint)Stats.Level);
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(ASCIIEncoding.ASCII.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }
        }

        public class GroupMatch
        {
            public ushort Type = 2210;
            public QualifierList.QualifierGroup Group;
            public byte[] BuildPacket()
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);

                wtr.Write((ushort)56);
                wtr.Write((ushort)Type);
                wtr.Write((uint)Group.Player1.ArenaStatistic.EntityID);
                for (int i = 0; i < 16; i++)
                {
                    if (i < Group.Player1.ArenaStatistic.Name.Length)
                    {
                        wtr.Write((byte)Group.Player1.ArenaStatistic.Name[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }
                wtr.Write((uint)Group.Player1Damage);
                wtr.Write((uint)Group.Player2.ArenaStatistic.EntityID);
                for (int i = 0; i < 16; i++)
                {
                    if (i < Group.Player2.ArenaStatistic.Name.Length)
                    {
                        wtr.Write((byte)Group.Player2.ArenaStatistic.Name[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }
                wtr.Write((uint)Group.Player2Damage);
                wtr.Write((uint)0);
                wtr.Write(ASCIIEncoding.ASCII.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }
        }

        public static void Win(Client.GameState winner, Client.GameState loser)
        {
            if (winner == null && loser != null)
            {
                loser.ArenaStatistic.PlayWith = 0;
                loser.ArenaStatistic.AcceptBox = false;
                if (loser.ArenaStatistic.ArenaPoints > 80000)
                    loser.ArenaStatistic.ArenaPoints = 0;
                ArenaSignup sign = new ArenaSignup();
                sign.DialogID = ArenaSignup.MainIDs.Match;
                loser.Send(sign.BuildPacket());

                sign = new ArenaSignup();
                sign.DialogID = ArenaSignup.MainIDs.Match;
                sign.OptionID = ArenaSignup.DialogButton.MatchOff;
                loser.Send(sign.BuildPacket());
                if (loser.ArenaStatistic.ArenaPoints == 0)
                {
                    QualifyEngine.DoQuit(loser);
                }
                loser.ArenaStatistic.TotalLose++;

                Sort();
                loser.Send(loser.ArenaStatistic);
                QualifyEngine.DoQuit(loser);

            }
            else if (loser == null && winner != null)
            {
                winner.ArenaStatistic.PlayWith = 0;
                winner.ArenaStatistic.AcceptBox = false;
                winner.ArenaStatistic.TodayWin++;
                winner.ArenaStatistic.TotalWin++;
                ArenaSignup sign = new ArenaSignup();
                sign.DialogID = ArenaSignup.MainIDs.Match;
                sign.OptionID = ArenaSignup.DialogButton.Win;
                winner.Send(sign.BuildPacket());

                sign = new ArenaSignup();
                sign.DialogID = ArenaSignup.MainIDs.Match;
                sign.OptionID = ArenaSignup.DialogButton.MatchOff;
                winner.Send(sign.BuildPacket());
                Sort();
                QualifyEngine.DoQuit(winner);
                winner.Send(winner.ArenaStatistic);
            }
            else if (loser == null && winner == null)
            {
                return;
            }
            else
            {
                if (winner.ArenaStatistic.PlayWith != 0 && loser.ArenaStatistic.PlayWith != 0)
                {
                    int diff = (int)winner.ArenaStatistic.ArenaPoints - (int)loser.ArenaStatistic.ArenaPoints;
                    diff = diff < 0 ? -diff : diff;
                    if (diff == 0 || diff > 50)
                        diff = ServerBase.Kernel.Random.Next(30, 50);

                    ArenaSignup sign = new ArenaSignup();
                    sign.DialogID = ArenaSignup.MainIDs.Match;
                    loser.Send(sign.BuildPacket());
                    sign.OptionID = ArenaSignup.DialogButton.Win;
                    winner.Send(sign.BuildPacket());

                    sign = new ArenaSignup();
                    sign.DialogID = ArenaSignup.MainIDs.Match;
                    sign.OptionID = ArenaSignup.DialogButton.MatchOff;
                    loser.Send(sign.BuildPacket());
                    winner.Send(sign.BuildPacket());

                    winner.ArenaStatistic.PlayWith = 0;
                    loser.ArenaStatistic.PlayWith = 0;
                    winner.ArenaStatistic.AcceptBox = false;
                    loser.ArenaStatistic.AcceptBox = false;
                    winner.ArenaStatistic.ArenaPoints += (uint)diff;
                    Message message = null;
                    message = new Message("[Qualifier]" + winner.ArenaStatistic.Name + " has Defeated " + loser.ArenaStatistic.Name + " in the Qualifier, and is currently ranked No. " + winner.ArenaStatistic.Rank + "!", System.Drawing.Color.Red, Message.TopLeft);
                    foreach (Client.GameState client in ServerBase.Kernel.GamePool.Values)
                    {
                        client.Send(message);
                    }
                    loser.ArenaStatistic.ArenaPoints -= (uint)diff;
                    if (loser.ArenaStatistic.ArenaPoints > 80000)
                        loser.ArenaStatistic.ArenaPoints = 0;
                    if (loser.ArenaStatistic.ArenaPoints == 0)
                        QualifyEngine.DoQuit(loser);

                    winner.ArenaStatistic.TodayWin++;
                    winner.ArenaStatistic.TotalWin++;
                    if (winner.ArenaStatistic.TodayWin == 9 || winner.ArenaStatistic.TodayBattles == 20)
                    {
                        winner.Inventory.Add(723912, 0, 1);//EXP Pack
                    }
                    if (winner.ArenaStatistic.TodayWin == 27 || winner.ArenaStatistic.TodayBattles == 40)
                    {
                        winner.Inventory.Add(723912, 0, 1);
                    }
                    if (loser.ArenaStatistic.TodayBattles == 20)
                        loser.IncreaseExperience(loser.ExpBall, false);
                    loser.ArenaStatistic.TotalLose++;

                    Sort();
                    winner.Send(winner.ArenaStatistic);
                    loser.Send(loser.ArenaStatistic);
                    QualifyEngine.DoQuit(winner);
                    QualifyEngine.DoQuit(loser);

                    winner.QualifierGroup = null;
                    loser.QualifierGroup = null;
                }
            }
        }

        private static DateTime YesterdaySorted = DateTime.Now;

        private static bool DayPassed = false;

        public static void ArenaSystem_Execute()
        {
            #region in-game players

            var Players = new GameState[PlayerList.Count + 2];
            PlayerList.Values.CopyTo(Players, 0);
            if (Players.Length >= 2)
            {
                for (int count = 0; count < Players.Length; count++)
                {
                    Client.GameState Client = Players[count];
                    if (Client != null && Client.Entity != null && (!ServerBase.Constants.PKFreeMaps.Contains(Client.Map.ID) || Client.Map.ID == 1005))
                    {
                        if (Client.InArenaMatch)
                            continue;
                        #region Select players
                        if (Client.ArenaStatistic.Status == Network.GamePackets.ArenaStatistic.WaitingForOpponent)
                        {
                            for (int count2 = count + 1; count2 < Players.Length; count2++)
                            {
                                Client.GameState Client2 = Players[count2];
                                if (Client2 != null && Client2.Entity != null && (!ServerBase.Constants.PKFreeMaps.Contains(Client2.Map.ID) || Client2.Map.ID == 1005))
                                {
                                    if (Client2.InArenaMatch)
                                        continue;
                                    if (Client2.ArenaStatistic.Status == Network.GamePackets.ArenaStatistic.WaitingForOpponent)
                                    {
                                        byte type = 1;
                                        int diff = 0;
                                        if (Client2.ArenaStatistic.Rank == 0)
                                        {
                                            diff = (int)Client.ArenaStatistic.ArenaPoints - (int)Client2.ArenaStatistic.ArenaPoints;
                                            diff = diff < 0 ? -diff : diff;
                                        }
                                        else
                                        {
                                            if (Client.ArenaStatistic.Rank == 0)
                                            {
                                                diff = (int)Client.ArenaStatistic.ArenaPoints - (int)Client2.ArenaStatistic.ArenaPoints;
                                                diff = diff < 0 ? -diff : diff;
                                            }
                                            else
                                            {
                                                diff = (int)Client.ArenaStatistic.Rank - (int)Client2.ArenaStatistic.Rank;
                                                diff = diff < 0 ? -diff : diff;
                                                type = 2;
                                            }
                                        }
                                        if ((type == 1 && diff <= 100) || (type == 2 && diff <= 10))
                                        {
                                            Client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingInactive;
                                            Client2.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingInactive;
                                            Client.Send(Client.ArenaStatistic);
                                            Client2.Send(Client2.ArenaStatistic);
                                            Client.ArenaStatistic.AcceptBoxShow = Client2.ArenaStatistic.AcceptBoxShow = Time32.Now;
                                            Client.ArenaStatistic.PlayWith = Client2.ArenaStatistic.EntityID;
                                            Client2.ArenaStatistic.PlayWith = Client.ArenaStatistic.EntityID;
                                            Client.InArenaMatch = true;
                                            Client2.InArenaMatch = true;
                                            ArenaSignup sign = new ArenaSignup();
                                            sign.DialogID = ArenaSignup.MainIDs.StartCountDown;
                                            sign.Stats = Client.ArenaStatistic;
                                            Client.Send(sign.BuildPacket());
                                            sign.Stats = Client2.ArenaStatistic;
                                            Client2.Send(sign.BuildPacket());
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Check for timer
                        else
                        {
                            if (Client != null && Client.ArenaStatistic != null && Time32.Now >= Client.ArenaStatistic.AcceptBoxShow.AddSeconds(60))
                            {
                                if (Client.ArenaStatistic.PlayWith != 0 && Client.QualifierGroup == null)
                                {
                                    Conquer_Online_Server.Client.GameState Client2 = PlayerList[Client.ArenaStatistic.PlayWith];
                                    if (Client2 != null && Client.ArenaStatistic != null)
                                    {
                                        if (Client2.ArenaStatistic.PlayWith != 0 && Client2.QualifierGroup == null)
                                        {
                                            if (Client.ArenaStatistic.AcceptBox && Client2.ArenaStatistic.AcceptBox)
                                            {
                                                Client.ArenaStatistic.AcceptBoxShow = Client2.ArenaStatistic.AcceptBoxShow = Time32.Now;
                                                Client.ArenaStatistic.PlayWith = Client2.ArenaStatistic.EntityID;
                                                Client2.ArenaStatistic.PlayWith = Client.ArenaStatistic.EntityID;
                                                QualifierList.QualifierGroup group = new QualifierList.QualifierGroup(Client, Client2);
                                                QualifierList.Groups.Add(group.ID, group);
                                                group.Import();
                                            }
                                            else
                                            {
                                                if (Client.QualifierGroup != null && Client2.QualifierGroup != null)
                                                    return;
                                                if (Client.ArenaStatistic.AcceptBox == true)
                                                {
                                                    Win(Client, Client2);
                                                }
                                                else
                                                {
                                                    if (Client2.ArenaStatistic.AcceptBox)
                                                    {
                                                        Win(Client2, Client);
                                                    }
                                                    else
                                                    {
                                                        if (Client.ArenaStatistic.ArenaPoints > Client2.ArenaStatistic.ArenaPoints)
                                                        {
                                                            Win(Client, Client2);
                                                        }
                                                        else
                                                        {
                                                            Win(Client2, Client);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Client.ArenaStatistic.PlayWith = 0;
                                        Client.ArenaStatistic.AcceptBox = false;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }

            if (QualifierList.Groups.Count > 0)
            {
                QualifierList.QualifierGroup[] GroupsList = QualifierList.Groups.Values.ToArray();
                for (int count = 0; count < QualifierList.Groups.Count; count++)
                {
                    QualifierList.QualifierGroup group = GroupsList[count];
                    if (Time32.Now > group.CreateTime.AddSeconds(5))
                    {
                        if (group.Inside)
                        {
                            if (!group.Done)
                            {
                                if (Time32.Now > group.CreateTime.AddMinutes(5) && !group.CanEnd)
                                {
                                    group.End();
                                }
                            }
                            else
                            {
                                if (Time32.Now > group.DoneStamp.AddSeconds(1))
                                {
                                    group.Export();
                                    group.Done = false;
                                }
                            }
                        }
                    }
                }
                GroupsList = null;
            }
            #endregion
            #region resseting
            DateTime Noww = DateTime.Now;
            TimeSpan Now = Noww.TimeOfDay;
            if (!DayPassed)
                if (Now.Hours == 23 && Now.Minutes == 59)
                    DayPassed = true;
            if (DayPassed)
            {
                if (Now.Hours == 0 && Now.Minutes <= 2)
                {
                    DayPassed = false;
                    foreach (ArenaStatistic stat in ArenaStatistics.Values)
                    {
                        stat.LastSeasonArenaPoints = stat.ArenaPoints;
                        stat.LastSeasonWin = stat.TodayWin;
                        stat.LastSeasonLose = stat.TodayBattles - stat.TodayWin;
                        stat.TodayWin = 0;
                        stat.TodayBattles = 0;
                        stat.CurrentHonor += stat.Rank * 15000;
                        stat.HistoryHonor += stat.Rank * 15000;
                        stat.ArenaPoints = Database.ArenaTable.ArenaPointFill(stat.Level);
                        stat.LastArenaPointFill = DateTime.Now;
                        if (ServerBase.Kernel.GamePool.ContainsKey(stat.EntityID))
                        {
                            Client.GameState client = ServerBase.Kernel.GamePool[stat.EntityID];
                            client.Send(stat);
                        }
                    }
                    SaveArenaStats();
                    YesterdaySort();
                }
            }
            #endregion
        }
    }
}
