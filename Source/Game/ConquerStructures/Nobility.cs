using System;
using System.Linq;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
using System.IO;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Nobility
    {
        public static SafeDictionary<uint, NobilityInformation> Board = new SafeDictionary<uint, NobilityInformation>(10000);
        public static List<NobilityInformation> BoardList = new List<NobilityInformation>(10000);
        public static void Handle(NobilityInfo information, Client.GameState client)
        {
            switch (information.Type)
            {
                case NobilityInfo.Donate:
                    {
                        if (client.Trade.InTrade)
                            return;
                        uint silvers = information.dwParam;
                        bool newDonator = false;
                        if (client.NobilityInformation.Donation == 0)
                            newDonator = true;
                        if (information.dwParam2 == 1)
                        {
                            uint cps = silvers / 50000;
                            if (client.Entity.ConquerPoints >= cps)
                            {
                                client.Entity.ConquerPoints -= cps;
                                client.NobilityInformation.Donation += silvers;
                            }
                        }
                        else
                        {
                            if (client.Entity.Money >= silvers)
                            {
                                client.Entity.Money -= silvers;
                                client.NobilityInformation.Donation += silvers;
                            }
                        }

                        if (!Board.ContainsKey(client.Entity.UID) && client.NobilityInformation.Donation == silvers && newDonator)
                        {
                            Board.Add(client.Entity.UID, client.NobilityInformation);
                            try
                            {
                                Database.NobilityTable.InsertNobilityInformation(client.NobilityInformation);
                            }
                            catch
                            {
                                Database.NobilityTable.UpdateNobilityInformation(client.NobilityInformation);
                            }
                        }
                        else
                        {
                            Database.NobilityTable.UpdateNobilityInformation(client.NobilityInformation);
                        }
                        Sort(client.Entity.UID);
                        break;
                    }
                case NobilityInfo.List:
                    {
                        byte Count = 0;
                        MemoryStream strm = new MemoryStream();
                        BinaryWriter wtr = new BinaryWriter(strm);
                        wtr.Write((ushort)0);
                        wtr.Write((ushort)2064);
                        wtr.Write((uint)NobilityInfo.List);
                        wtr.Write((ushort)information.wParam1);
                        wtr.Write((ushort)5);
                        wtr.Write((uint)0);
                        wtr.Write((uint)0);
                        wtr.Write((uint)0);
                        wtr.Write((uint)0);
                        wtr.Write((uint)0);
                        for (int i = (int)(information.wParam1 * 10); i < information.wParam1 * 10 + 10; i++)
                        {
                            if (BoardList.Count > i)
                            {
                                Count++;
                                wtr.Write((uint)BoardList[i].EntityUID);
                                wtr.Write((uint)BoardList[i].Gender);
                                wtr.Write((uint)BoardList[i].Mesh);
                                for (int c = 0; c < 20; c++)
                                {
                                    if (BoardList[i].Name.Length > c)
                                        wtr.Write((byte)(BoardList[i].Name[c]));
                                    else
                                        wtr.Write((byte)(0));
                                }
                                wtr.Write((ulong)BoardList[i].Donation);
                                wtr.Write((uint)BoardList[i].Rank);
                                wtr.Write((uint)BoardList[i].Position);
                            }
                        }
                        int packetlength = (int)strm.Length;
                        strm.Position = 0;
                        wtr.Write((ushort)packetlength);
                        strm.Position = strm.Length;
                        wtr.Write(System.Text.Encoding.ASCII.GetBytes("TQServer"));
                        strm.Position = 0;
                        byte[] buf = new byte[strm.Length];
                        strm.Read(buf, 0, buf.Length);
                        Network.Writer.WriteUInt32(Count, 12, buf);
                        client.Send(buf);

                        information.Type = NobilityInfo.NextRank;
                        ulong value = 0;
                        information.dwParam2 = 0;
                        if (client.NobilityInformation.Rank == NobilityRank.Prince)
                            value = (ulong)(BoardList[02].Donation - client.NobilityInformation.Donation + 1);
                        if (client.NobilityInformation.Rank == NobilityRank.Duke)
                            value = (ulong)(BoardList[14].Donation - client.NobilityInformation.Donation + 1);
                        if (client.NobilityInformation.Rank == NobilityRank.Earl)
                            value = (ulong)(BoardList[49].Donation - client.NobilityInformation.Donation + 1);
                        Network.Writer.WriteUInt64(value, 8, information.ToArray());
                        information.dwParam3 = 60;
                        information.dwParam4 = uint.MaxValue;
                        client.Send(information);
                        break;
                    }
            }
        }

        public static void Sort(uint updateUID)
        {
            SortedDictionary<ulong, SortEntry<uint, NobilityInformation>> sortdict = new SortedDictionary<ulong, SortEntry<uint, NobilityInformation>>();

            foreach (NobilityInformation info in Board.Values)
            {
                if (sortdict.ContainsKey(info.Donation))
                {
                    SortEntry<uint, NobilityInformation> entry = sortdict[info.Donation];
                    entry.Values.Add(info.EntityUID, info);
                }
                else
                {
                    SortEntry<uint, NobilityInformation> entry = new SortEntry<uint, NobilityInformation>();
                    entry.Values = new Dictionary<uint, NobilityInformation>();
                    entry.Values.Add(info.EntityUID, info);
                    sortdict.Add(info.Donation, entry);
                }
            }

            SafeDictionary<uint, NobilityInformation> sortedBoard = new SafeDictionary<uint, NobilityInformation>(10000);

            int Place = 0;
            foreach (KeyValuePair<ulong, SortEntry<uint, NobilityInformation>> entries in sortdict.Reverse())
            {
                foreach (KeyValuePair<uint, NobilityInformation> value in entries.Value.Values)
                {
                    Client.GameState client = null;
                    try
                    {
                        int previousPlace = value.Value.Position;
                        value.Value.Position = Place;
                        NobilityRank Rank = NobilityRank.Serf;

                        if (Place >= 50)
                        {
                            if (value.Value.Donation >= 200000000)
                            {
                                Rank = NobilityRank.Earl;
                                //ServerBase.Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + "Donation To Earl in Nobility Rank.", System.Drawing.Color.White, Message.TopLeft), ServerBase.Kernel.GamePool.Values);
                                //Rank = NobilityRank.Earl;
                            }
                            else if (value.Value.Donation >= 100000000)
                            {
                                Rank = NobilityRank.Baron;
                                //ServerBase.Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + "Donation To Baron in Nobility Rank.", System.Drawing.Color.White, Message.TopLeft), ServerBase.Kernel.GamePool.Values);
                                //Rank = NobilityRank.Baron;
                            }
                            else if (value.Value.Donation >= 30000000)
                            {
                                Rank = NobilityRank.Knight;
                                //ServerBase.Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + "Donation To Knight in Nobility Rank.", System.Drawing.Color.White, Message.TopLeft), ServerBase.Kernel.GamePool.Values);
                                //Rank = NobilityRank.Knight;
                            }
                        }
                        else
                        {
                            if (Place < 3)
                            {
                                //Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Congratulation! " + client.Entity.Name + "Donation To King in Nobility Rank!", System.Drawing.Color.White, 2011), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                                Rank = NobilityRank.King;
                                //Conquer_Online_Server.Clan.nobmas(client);
                                // ServerBase.Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + "Donation To King/Queen in Nobility Rank.", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
                                //Rank = NobilityRank.King;
                            }
                            else if (Place < 15)
                            {
                                Rank = NobilityRank.Prince;
                                //Conquer_Online_Server.Clan.nobmas(client);
                                // ServerBase.Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + "Donation To Prince in Nobility Rank.", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
                                // Rank = NobilityRank.Prince;
                            }
                            else
                            {
                                Rank = NobilityRank.Duke;
                                //Conquer_Online_Server.Clan.nobmas(client);
                                //ServerBase.Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + "Donation To Duke in Nobility Rank.", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
                                //Rank = NobilityRank.Duke;
                            }
                        }
                        var oldRank = value.Value.Rank;
                        value.Value.Rank = Rank;
                        if (ServerBase.Kernel.GamePool.TryGetValue(value.Key, out client))
                        {
                            bool updateTheClient = false;
                            if (oldRank != Rank)
                            {
                                updateTheClient = true;
                            }
                            else
                            {
                                if (previousPlace != Place)
                                {
                                    updateTheClient = true;
                                }
                            }
                            if (updateTheClient || client.Entity.UID == updateUID)
                            {
                                NobilityInfo update = new NobilityInfo(true);
                                update.Type = NobilityInfo.Icon;
                                update.dwParam = value.Key;
                                update.UpdateString(value.Value);
                                client.SendScreen(update, true);
                                client.Entity.NobilityRank = value.Value.Rank;
                            }
                        }
                        sortedBoard.Add(value.Key, value.Value);
                        Place++;
                    }
                    catch { }
                }

            }

            Board = sortedBoard;
            lock (BoardList)
            {
                BoardList = Board.Values.ToList();
            }
        }
    }
    public class NobilityInformation
    {
        public string Name;
        public uint EntityUID;
        public uint Mesh;
        public ulong Donation;
        public byte Gender;
        public int Position;
        public NobilityRank Rank;
    }

    public enum NobilityRank : byte
    {
        Serf = 0,
        Knight = 1,
        Baron = 3,
        Earl = 5,
        Duke = 7,
        Prince = 9,
        King = 12
    }
}
