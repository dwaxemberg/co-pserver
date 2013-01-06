using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Client;
using NpcDialogs;
using Conquer_Online_Server.Network.Features.ClassPKWar;
using System.Drawing;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Network.GameBufferets;
using System.Text;


namespace Conquer_Online_Server.Network
{
    public static class PacketHandler
    {
        public static byte[] TQ_SERVER = Encoding.ASCII.GetBytes("TQServer");
        public static byte[] ReturnFinal(byte[] Data)
        {

            //Replaces "TQClient" with "TQServer" on the end of the packet so it may be looped back to the client.
            Array.Copy(TQ_SERVER, 0, Data, Data.Length - TQ_SERVER.Length, TQ_SERVER.Length);
            return Data;
        }
        public static void HandleBuffer(byte[] buffer, Client.GameState client)
        {
            if (buffer == null)
                return;
            if (client == null)
                return;
        roleAgain:
            ushort Length = BitConverter.ToUInt16(buffer, 0);
            if ((Length + 8) == buffer.Length)
            {
                Network.Writer.WriteString(ServerBase.Constants.ServerKey, (buffer.Length - 8), buffer);
                HandlePacket(buffer, client);
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
                HandlePacket(Packet, client);
                goto roleAgain;
            }
        }
           public static void SwitchPackets(Client.PacketClient data)
        {
            HandlePacket(data.Buffer, data.client);
        }
        public class WaitDc
        {
            Client.GameState Who;
            System.Timers.Timer T;
            public WaitDc(Client.GameState C)
            {
                this.Who = C;
                T = new System.Timers.Timer();
                T.Interval = 1000;
                T.Elapsed += new System.Timers.ElapsedEventHandler(T_Elapsed);
                T.AutoReset = false;
                T.Start();
            }

            void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                Who.Disconnect();
                T.Dispose();
            }

        }
        static void HandlePacket(byte[] packet, Client.GameState client)
        {
            if (packet == null)
                return;
            if (client == null)
                return;
            ushort Length = BitConverter.ToUInt16(packet, 0);
            ushort ID = BitConverter.ToUInt16(packet, 2);
            ushort ID2 = BitConverter.ToUInt16(packet, 4);
            switch (ID)
            {
                #region market quest
                case 2400:
                    {
                        if (client.Entity.QQ1 == 0)
                        {
                            client.Entity.Teleport(1068, 053, 055);
                        }
                        else
                        {
                            client.Send(new Message("you can only make quest once in day", Color.AntiqueWhite, 2005));
                        }

                        break;
                    }
                #endregion
                #region ElitePk Tournament 2223 | 2219
                case 2223:
                    {
                        if (client.Entity.MapID == 6002)
                            break;

                        if (ServerBase.Kernel.Elite_PK_Tournament.Start)
                            break;

                        GamePackets.Elite_Pk pk = new Elite_Pk(client.Entity.UID);
                        pk.Send(client);
                        break;
                    }
                case 2219:
                    {
                        if (client.Entity.MapID == 6002)
                            break;
                        byte[] sed = new byte[36]
                        {
                        0x1C ,0x00 ,0xAB ,0x08 ,0x04 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x03 ,0x00    //  ; «          
,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x54 ,0x51 ,0x53 ,0x65     // ;            TQSe
,0x72 ,0x76 ,0x65 ,0x72                                         // ;rver
                        };
                        client.Send(sed);
                        break;


                    }
                case 1063:
                    {
                        GamePackets.Guild_Pk pk = new Guild_Pk(client.Entity.UID);
                        pk.Send(client);
                        //byte[] sed = new byte[346];
                        ////{0x5a ,0x01 ,0x27 ,0x04 ,0x09 ,0x00 ,0x01 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x08 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x21 ,0x42 ,0x72 ,0x61 ,0x73 ,0x69 ,0x6c ,0x54 ,0x6f ,0x50 ,0x54 ,0x65 ,0x61 ,0x6d ,0x21 ,0x00 ,0x45 ,0x2a ,0x00 ,0x00 ,0x0e ,0x00 ,0x00 ,0x00 ,0x40 ,0xd2 ,0xdf ,0x03 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x7e ,0x5e ,0x21 ,0x54 ,0x68 ,0x65 ,0x5f ,0x42 ,0x65 ,0x73 ,0x74 ,0x21 ,0x5e ,0x7e ,0x00 ,0x00 ,0x2f ,0x20 ,0x00 ,0x00 ,0x09 ,0x00 ,0x00 ,0x00 ,0x00 ,0x0e ,0x27 ,0x07 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x46 ,0x69 ,0x72 ,0x65 ,0x45 ,0x6d ,0x62 ,0x6c ,0x65 ,0x6d ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x20 ,0x1c ,0x00 ,0x00 ,0x17 ,0x00 ,0x00 ,0x00 ,0x00 ,0xe1 ,0xf5 ,0x05 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x7e ,0x44 ,0x2e ,0x30 ,0x2e ,0x4a ,0x2e ,0x7e ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x3f ,0x0c ,0x00 ,0x00 ,0x0d ,0x00 ,0x00 ,0x00 ,0x40 ,0xf6 ,0xd3 ,0x04 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x2d ,0x4e ,0x7e ,0x4f ,0x7e ,0x59 ,0x7e ,0x50 ,0x7e ,0x49 ,0x2d ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x9e ,0x06 ,0x00 ,0x00 ,0x05 ,0x00 ,0x00 ,0x00 ,0x00 ,0x5a ,0x62 ,0x02 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x53 ,0x75 ,0x70 ,0x72 ,0x65 ,0x6d ,0x65 ,0x7e ,0x46 ,0x6f ,0x72 ,0x63 ,0x65 ,0x00 ,0x00 ,0x00 ,0x2f ,0x06 ,0x00 ,0x00 ,0x03 ,0x00 ,0x00 ,0x00 ,0x00 ,0x2d ,0x31 ,0x01 ,0x00 ,0x00 ,0x00 ,0x00 ,0xc8 ,0x00 ,0x00 ,0x00 ,0x53 ,0x69 ,0x6c ,0x65 ,0x6e ,0x54 ,0x5f ,0x48 ,0x65 ,0x41 ,0x6c ,0x4c ,0x23 ,0x30 ,0x31 ,0x00 ,0x40 ,0x03 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00 ,0x80 ,0xf0 ,0xfa ,0x02 ,0x00 ,0x00 ,0x00 ,0x00 ,0xf4 ,0x01 ,0x00 ,0x00 ,0x21 ,0x4e ,0x6f ,0x86 ,0x4d ,0x65 ,0x72 ,0x63 ,0x59 ,0x21 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0xe8 ,0x01 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00
                                               
                        ////};
                        //client.Send(sed);
                        break;


                    }
                case 2224:
                    {

                        client.Send(packet);
                        break;


                    }
                case 2232:
                    {

//                        byte[] sed = new byte[36]
//                        {
//                        0x1C ,0x00 ,0xAB ,0x08 ,0x04 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x03 ,0x00    //  ; «          
//,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x54 ,0x51 ,0x53 ,0x65     // ;            TQSe
//,0x72 ,0x76 ,0x65 ,0x72                                         // ;rver
//                        };
                        client.Send(packet);
                        break;


                    }
                case 2233:
                    {

                        GamePackets.Team_Pk pk = new Team_Pk(client.Entity.UID);
                        pk.Send(client);
                        break;


                    }
                case 2252:
                    {

                        // byte[] sed = new byte[60];
                        // ;rver };
                        client.Send(packet);
                        break;


                    }
                case 1130:
                    {
                        if (client.Entity.TitlePacket != null)
                        {
                            if (packet[9] == 4)
                            {
                                if (client.Entity.TitlePacket.dwParam2 != 0)
                                    client.Entity.TitlePacket.Send(client);
                            }
                            if (packet[9] == 3)
                            {
                                client.Entity.TitleActivated = packet[8];
                                client.Send(packet);
                                client.SendScreen(client.Entity.SpawnPacket, false);
                            }
                        }
                        break;
                    }
                #endregion
                #region TopGuilds
                case 1058:
                    {
                        if (client.Guild != null && client.AsMember != null)
                        {
                            if (client.AsMember != null)
                            {
                                Writer.WriteUInt32((uint)client.AsMember.SilverDonation, 8, packet);
                                if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                    if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        if (client.AsMember.SilverDonation > client.Guild.money_donation)
                                        {
                                            client.Guild.money_donation = (uint)client.AsMember.SilverDonation;
                                            client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.OSupervisor;
                                            client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                        }

                                Writer.WriteUInt32((uint)client.AsMember.ConquerPointDonation, 20, packet);
                                if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                    if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        if (client.AsMember.ConquerPointDonation > client.Guild.cp_donaion)
                                        {
                                            client.Guild.cp_donaion = (uint)client.AsMember.ConquerPointDonation;
                                            client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.CPSupervisor;
                                            client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                        }

                            }
                            Writer.WriteUInt32(client.Entity.PKPoints, 12, packet);
                            if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                    if (client.Entity.PKPoints > client.Guild.pkp_donation)
                                    {
                                        client.Guild.pkp_donation = (uint)client.Entity.PKPoints;
                                        client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.PKSupervisor;
                                        client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                    }

                            if (client.ArenaStatistic != null)
                            {
                                Writer.WriteUInt32(client.ArenaStatistic.CurrentHonor, 24, packet);
                                if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                    if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        if (client.ArenaStatistic.CurrentHonor > client.Guild.honor_donation)
                                        {
                                            client.Guild.honor_donation = (uint)client.ArenaStatistic.CurrentHonor;
                                            client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.HonoraryManager;
                                            client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                        }
                            }
                            Writer.WriteUInt32(0, 16, packet);
                            if (client.Entity != null)
                                if (client.Entity.MyFlowers != null)
                                {
                                    Writer.WriteUInt32((uint)client.Entity.MyFlowers.RedRoses, 28, packet);
                                    if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                        if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                            if (client.Entity.MyFlowers.RedRoses > client.Guild.rose_donation)
                                            {
                                                client.Guild.rose_donation = (uint)client.Entity.MyFlowers.RedRoses;
                                                client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.RoseSupervisor;
                                                client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                            }


                                    Writer.WriteUInt32((uint)client.Entity.MyFlowers.Tulips, 32, packet);
                                    if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                        if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                            if (client.Entity.MyFlowers.Tulips > client.Guild.tuil_donation)
                                            {
                                                client.Guild.tuil_donation = (uint)client.Entity.MyFlowers.Tulips;
                                                client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.TulipFollower;
                                                client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                            }



                                    Writer.WriteUInt32((uint)client.Entity.MyFlowers.Lilies, 36, packet);
                                    if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                        if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                            if (client.Entity.MyFlowers.Lilies > client.Guild.lilies_donation)
                                            {
                                                client.Guild.lilies_donation = (uint)client.Entity.MyFlowers.Lilies;
                                                client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.LilySupervisor;
                                                client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                            }


                                    Writer.WriteUInt32((uint)client.Entity.MyFlowers.Orchads, 40, packet);
                                    if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                        if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                            if (client.Entity.MyFlowers.Orchads > client.Guild.orchid_donation)
                                            {
                                                client.Guild.orchid_donation = (uint)client.Entity.MyFlowers.Orchads;
                                                client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.OrchidFollower;
                                                client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                            }
                                    Writer.WriteUInt32((uint)(client.Entity.MyFlowers.Orchads
                                        + (uint)client.Entity.MyFlowers.RedRoses
                                        + (uint)client.Entity.MyFlowers.Tulips
                                        + (uint)client.Entity.MyFlowers.Lilies), 44, packet);
                                }
                            if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                if (client.Entity.GuildRank != (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                {
                                    if (client.Entity.Name == client.Guild.LeaderName)
                                    {
                                        client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.LeaderSpouse;
                                        client.AsMember.Rank = (Conquer_Online_Server.Game.Enums.GuildMemberRank)client.Entity.GuildRank;
                                    }
                                }
                            if (client.Guild != null)
                                client.Guild.SendGuild(client);
                            client.Send(packet);
                        }
                        break;
                    }
                #endregion
                #region Guild members (2102)
                case 2102:
                    {
                        ushort Page = BitConverter.ToUInt16(packet, 8);
                        if (client.Guild != null)
                        {
                            client.Guild.SendMembers(client, Page);
                        }
                        break;
                    }
                #endregion
                #region EnitityCreate (1001)
                case 1001:
                    {
                        if (client.Action == 1)
                        {
                            EnitityCreate EC = new EnitityCreate();
                            EC.Deserialize(packet);
                            string Message = "";
                            Boolean Created = Database.EntityTable.CreateEntity(EC, client, ref Message);
                            client.Send(new Message(Message, "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.PopUP));
                            if (Created)
                                Console.WriteLine(client.Account.Username + " Sucesfully Created a new Character " + EC.Name);
                            client.JustCreated = true;
                        }
                        break;
                    }
                #endregion
                #region Chat/Message (1004)
                case 1004:
                    {
                        if (client.Action != 2)
                            return;
                        Message message = new Message();
                        message.Deserialize(packet);
                        if (message.__Message.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries).Length > 0)
                            message.__Message = message.__Message.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        Chat(message, client);
                        break;
                    }
                #endregion
                #region Item/Ping (1009)
                case 1009:
                    {
                        if (client.Action != 2)
                            return;
                        ItemUsage usage = new ItemUsage(false);
                        usage.Deserialize(packet);
                        if (!client.Entity.Dead || usage.ID == ItemUsage.Ping)
                        {
                            switch (usage.ID)
                            {

                                case 52:
                                    {
                                        var item = Database.ConquerItemTable.LoadItem(usage.UID);
                                        item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.ChatItem;
                                        item.Send(client);
                                        break;
                                    }
                                case 41:
                                    {
                                        var item = Database.ConquerItemTable.LoadItem(usage.UID);
                                        usage.dwParam = 5;
                                        client.Send(usage);
                                        break;
                                    }
                                case ItemUsage.ArrowReload:
                                    {
                                        ReloadArrows(client.Equipment.TryGetItem(ConquerItem.LeftWeapon), client);

                                        break;
                                    }
                                case ItemUsage.ShowBoothItems:
                                    {
                                        ShowBoothItems(usage, client);
                                        break;
                                    }
                                case ItemUsage.AddItemOnBoothForSilvers:
                                case ItemUsage.AddItemOnBoothForConquerPoints:
                                    {
                                        AddItemOnBooth(usage, client);
                                        break;
                                    }
                                case ItemUsage.BuyFromBooth:
                                    {
                                        BuyFromBooth(usage, client);
                                        break;
                                    }
                                case ItemUsage.RemoveItemFromBooth:
                                    {
                                        RemoveItemFromBooth(usage, client);
                                        break;
                                    }
                                case ItemUsage.EquipItem:
                                    {
                                        EquipItem(usage, client);
                                        break;
                                    }
                                case ItemUsage.UnequipItem:
                                    {
                                        UnequipItem(usage, client);
                                        break;
                                    }
                                case ItemUsage.BuyFromNPC:
                                    {
                                        HandleBuyFromNPC(usage, client);
                                        break;
                                    }
                                case ItemUsage.SellToNPC:
                                    {
                                        HandleSellToNPC(usage, client);
                                        break;
                                    }
                                case ItemUsage.Repair:
                                    {
                                        HandleRepair(usage, client);
                                        break;
                                    }
                                case ItemUsage.MeteorUpgrade:
                                case ItemUsage.DragonBallUpgrade:
                                    {
                                        UpgradeItem(usage, client);
                                        break;
                                    }
                                case ItemUsage.Ping:
                                    {
                                        if (Time32.Now < client.LastPing.AddSeconds(2))
                                        {
                                            client.PingCount++;
                                            if (client.PingCount == 40)
                                            {
                                                client.Send(new Message("Speed hack detected!", System.Drawing.Color.BlanchedAlmond, Message.TopLeft));
                                                client.Disconnect();
                                                return;
                                            }
                                        }

                                        if (client.Entity != null)
                                        {
                                            if (client.Entity.UID != 0)
                                            {
                                                if (!ServerBase.Kernel.GamePool.ContainsKey(client.Entity.UID))
                                                {
                                                    ServerBase.Kernel.GamePool.Add(client.Entity.UID, client);
                                                }
                                            }
                                        }

                                        client.LastPingT = client.LastPing;
                                        client.LastPing = Time32.Now;

                                        if (client.LastPing > client.LastPingT.AddSeconds(2))
                                            client.PingCount = 0;
                                        usage.TimeStamp += 120;
                                       // client.Send(ReturnFinal(packet));
                                        //Network.PacketHandler.HandleBuffer(packet, client);
                                        client.Send(usage);
                                        if (!ServerBase.Kernel.GamePool.ContainsKey(client.Entity.UID))
                                            if (client.Socket.Connected)
                                            {
                                                ServerBase.Kernel.GamePool.Add(client.Entity.UID, client);
                                                client.Screen.FullWipe();
                                                client.Screen.Reload(null);
                                            }
                                        break;
                                    }
                                case ItemUsage.ViewWarehouse:
                                    {
                                        usage.dwParam = client.MoneySave;
                                        client.Send(usage);
                                        break;
                                    }
                                case ItemUsage.WarehouseDeposit:
                                    {
                                        if (client.Entity.Money >= usage.dwParam)
                                        {
                                            client.Entity.Money -= usage.dwParam;
                                            client.MoneySave += usage.dwParam;
                                        }
                                        break;
                                    }
                                case ItemUsage.WarehouseWithdraw:
                                    {
                                        if (client.MoneySave >= usage.dwParam)
                                        {
                                            client.Entity.Money += usage.dwParam;
                                            client.MoneySave -= usage.dwParam;
                                        }
                                        break;
                                    }
                                case ItemUsage.DropItem:
                                    {
                                        DropItem(usage, client);
                                        break;
                                    }
                                case ItemUsage.DropMoney:
                                    {
                                        DropMoney(usage, client);
                                        break;
                                    }
                                case ItemUsage.Enchant:
                                    {
                                        EnchantItem(usage, client);
                                        break;
                                    }
                                case ItemUsage.SocketTalismanWithItem:
                                    {
                                        SocketTalismanWithItem(usage, client);
                                        break;
                                    }
                                case ItemUsage.SocketTalismanWithCPs:
                                    {
                                        SocketTalismanWithCPs(usage, client);
                                        break;
                                    }
                                case 40:
                                    {
                                        uint ItemAdd = (uint)((packet[4] & 0xFF) | ((packet[5] & 0xFF) << 8) | ((packet[6] & 0xFF) << 16) | ((packet[7] & 0xFF) << 24));

                                        //PrintPacket(packet);
                                        Interfaces.IConquerItem item_new = null;
                                        if (client.Inventory.TryGetItem(ItemAdd, out item_new))
                                        {
                                            if (item_new.Bless == 7)
                                                return;

                                            Queue<uint> amount = new Queue<uint>(packet[20]);

                                            for (ushort i = 84; i < 84 + 4 * packet[20]; i += 4)
                                            {
                                                if (client.Inventory.ContainsUID((uint)((packet[i] & 0xFF) | ((packet[(byte)(i + 1)] & 0xFF) << 8) | ((packet[(byte)(i + 2)] & 0xFF) << 16) | ((packet[(byte)(i + 3)] & 0xFF) << 24))))
                                                    amount.Enqueue((uint)((packet[i] & 0xFF) | ((packet[(byte)(i + 1)] & 0xFF) << 8) | ((packet[(byte)(i + 2)] & 0xFF) << 16) | ((packet[(byte)(i + 3)] & 0xFF) << 24)));
                                                else
                                                    return;
                                            }


                                            byte oldbless = item_new.Bless;
                                            if (item_new.Bless == 0 && amount.Count == 5)
                                                item_new.Bless = 1;
                                            else if (item_new.Bless == 1 && amount.Count == 1)
                                                item_new.Bless = 3;
                                            else if (item_new.Bless == 3 && amount.Count == 3)
                                                item_new.Bless = 5;
                                            else if (item_new.Bless == 5 && amount.Count == 5)
                                                item_new.Bless = 7;
                                            if (oldbless == item_new.Bless)
                                                return;

                                            while (amount.Count != 0)
                                                client.Inventory.Remove(amount.Dequeue(), Conquer_Online_Server.Game.Enums.ItemUse.Remove, true);

                                            item_new.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                            item_new.Send(client);
                                            Database.ConquerItemTable.UpdateItem(item_new, client);
                                        }
                                        //client.Send(packet);
                                        break;
                                    }
                                case 53:
                                    {
                                        uint ItemAdd = BitConverter.ToUInt32(packet, 8);


                                        //Interfaces.IConquerItem item_new = null;
                                        //if (client.Inventory.TryGetItem(ItemAdd, out item_new))
                                        {
                                            uint obtined_points = 0;
                                            Database.ConquerItemInformation iteminfo = new Conquer_Online_Server.Database.ConquerItemInformation(ItemAdd, 0);
                                            Dictionary<uint, uint> amount = new Dictionary<uint, uint>();//packet[20]);
                                            for (ushort i = 80; i < 84 + 4 * packet[20]; i += 4)
                                            {
                                                uint item_swap = BitConverter.ToUInt32(packet, i);
                                                if (client.Inventory.ContainsUID(item_swap))
                                                {
                                                    Interfaces.IConquerItem item = null;
                                                    if (client.Inventory.TryGetItem(item_swap, out item))
                                                    {
                                                        amount.Add(item_swap, i);
                                                    }
                                                    switch (item.ID)
                                                    {
                                                        case 191505:
                                                        case 191605:
                                                        case 191705:
                                                        case 191805:
                                                        case 191905:
                                                        case 191405:
                                                        case 183325:
                                                        case 183315:
                                                        case 183375:
                                                        case 183305:
                                                            {
                                                                obtined_points += 300;
                                                                break;
                                                            }
                                                        default:
                                                            obtined_points += 50;
                                                            break;
                                                    }
                                                }
                                            }
                                            if (iteminfo.BaseInformation.ConquerPointsWorth > obtined_points)
                                            {
                                                uint add_cps = 0;
                                                add_cps = (uint)(iteminfo.BaseInformation.ConquerPointsWorth - obtined_points);
                                                if (add_cps < client.Entity.ConquerPoints)
                                                {
                                                    client.Entity.ConquerPoints -= add_cps;
                                                    foreach (uint key in amount.Keys)
                                                    {
                                                        if (client.Inventory.ContainsUID(key))
                                                            client.Inventory.Remove(key, Conquer_Online_Server.Game.Enums.ItemUse.Remove, true);
                                                    }
                                                    client.Inventory.Add(ItemAdd, 0, 1);

                                                }
                                            }
                                            else
                                            {
                                                foreach (uint key in amount.Keys)
                                                {
                                                    if (client.Inventory.ContainsUID(key))
                                                        client.Inventory.Remove(key, Conquer_Online_Server.Game.Enums.ItemUse.Remove, true);
                                                }
                                                client.Inventory.Add(ItemAdd, 0, 1);

                                            }
                                        }
                                        break;
                                    }
                                case ItemUsage.SocketerMan:
                                    {
                                        #region Socketing
                                        UInt32 Count = usage.dwExtraInfo, ItemUID = usage.UID;
                                        Byte Type = 0;
                                        Interfaces.IConquerItem Item = null;
                                        if (client.Inventory.TryGetItem(ItemUID, out Item))
                                        {
                                            #region Removing Used Items
                                            for (int i = 1; i <= Count; i++)
                                            {
                                                uint It = BitConverter.ToUInt32(packet, (int)(80 + (4 * i)));
                                                Interfaces.IConquerItem Rem = null;
                                                if (client.Inventory.TryGetItem(It, out Rem))
                                                {
                                                    if (Rem.ID == 1088000 && Count == 12)
                                                        Type = 1;
                                                    if (Rem.ID == 1088000 && Count == 1)
                                                        Type = 2;
                                                    if (Rem.ID == 1088000 && Count == 5)
                                                        Type = 3;
                                                    if (Rem.ID == 1200005 && Count == 1)
                                                        Type = 4;
                                                    if (Rem.ID == 1200006 && Count == 7)
                                                        Type = 5;
                                                    client.Inventory.Remove(Rem, Game.Enums.ItemUse.Remove);
                                                }
                                            }
                                            #endregion
                                            #region Type Switch
                                            switch (Type)
                                            {
                                                #region Open First Socket
                                                case 1:
                                                case 2:
                                                    {
                                                        usage.dwParam = 1;
                                                        Item.Mode = Game.Enums.ItemMode.Update;
                                                        Item.SocketOne = (Game.Enums.Gem)255;
                                                        Item.Send(client);
                                                        Item.Mode = Game.Enums.ItemMode.Default;
                                                        Conquer_Online_Server.Database.ConquerItemTable.UpdateSockets(Item, client);
                                                        break;
                                                    }
                                                #endregion
                                                #region Open Second Socket
                                                case 3:
                                                case 5:
                                                    {
                                                        usage.dwParam = 1;
                                                        Item.Mode = Game.Enums.ItemMode.Update;
                                                        Kernel.SendWorldMessage(new Message("Congratulations! " + client.Entity.Name + " has oppened the second socket into his/her item!", System.Drawing.Color.Yellow, 2011), Kernel.GamePool.Values, client.Entity.UID);
                                                        Item.SocketTwo = (Game.Enums.Gem)255;
                                                        Item.Send(client);
                                                        Item.Mode = Game.Enums.ItemMode.Default;
                                                        Conquer_Online_Server.Database.ConquerItemTable.UpdateSockets(Item, client);
                                                        break;
                                                    }
                                                #endregion
                                                #region Using Tough Drill
                                                case 4:
                                                    {
                                                        if (ServerBase.Kernel.Rate(10))
                                                        {
                                                            usage.dwParam = 1;
                                                            Item.Mode = Game.Enums.ItemMode.Update;
                                                            Kernel.SendWorldMessage(new Message("Congratulations! " + client.Entity.Name + " has oppened the second socket into his/her item!", System.Drawing.Color.Yellow, 2011), Kernel.GamePool.Values, client.Entity.UID);
                                                            Item.SocketTwo = (Game.Enums.Gem)255;
                                                            Item.Send(client);
                                                            Item.Mode = Game.Enums.ItemMode.Default;
                                                            Conquer_Online_Server.Database.ConquerItemTable.UpdateSockets(Item, client);
                                                        }
                                                        else
                                                        {
                                                            usage.dwParam = 0;
                                                            client.Send(new Message("The ToughDrill has failed. Try your lucky next time!", System.Drawing.Color.Red, Message.TopLeft));
                                                            client.Inventory.Add(1200006, 0, 1);
                                                        }
                                                        break;
                                                    }
                                                #endregion
                                            }
                                            #endregion
                                        }
                                        //dWParam Values, = 0 = Failed, 1 = Suceed, 2 = Nothing
                                        client.Send(usage);
                                        #endregion
                                        break;
                                    }
                                case ItemUsage.RedeemGear:
                                    {
                                        var item = client.DeatinedItem[usage.UID];
                                        if (item != null)
                                        {
                                            if (DateTime.Now > item.Date.AddDays(7))
                                            {
                                                client.Send(new Message("This item is expired!", System.Drawing.Color.Red, Message.TopLeft));

                                                return;
                                            }
                                            if (client.Entity.ConquerPoints >= item.ConquerPointsCost)
                                            {
                                                client.Entity.ConquerPoints -= item.ConquerPointsCost;

                                                usage.dwParam = client.Entity.UID;
                                                usage.dwExtraInfo3 = item.ConquerPointsCost;
                                                client.Send(usage);

                                                client.Inventory.Add(item.Item, Conquer_Online_Server.Game.Enums.ItemUse.Add);

                                                Database.DetainedItemTable.Redeem(item, client);
                                                client.DeatinedItem.Remove(item.UID);

                                                if (ServerBase.Kernel.GamePool.ContainsKey(item.GainerUID))
                                                {
                                                    var pClient = ServerBase.Kernel.GamePool[item.GainerUID];
                                                    pClient.ClaimableItem[item.UID].OwnerUID = 500;
                                                    pClient.ClaimableItem[item.UID].MakeItReadyToClaim();
                                                    usage.dwParam = pClient.Entity.UID;
                                                    usage.ID = ItemUsage.ClaimGear;
                                                    pClient.Send(usage);
                                                    pClient.ClaimableItem[item.UID].Send(pClient);
                                                }

                                                Message message = new Message("Thank you for arresting " + item.OwnerName + ", " + item.GainerName + ". The arrested one has redeemed his items and you have received a great deal of ConquerPoints as reward. Congratulations!", System.Drawing.Color.Wheat, Message.Talk);
                                                ServerBase.Kernel.SendWorldMessage(message, ServerBase.Kernel.GamePool.Values);
                                            }
                                        }
                                        else
                                        {
                                            client.Send(new Message("The item you want to redeem has already been redeemed.", System.Drawing.Color.Red, Message.TopLeft));
                                        }
                                        break;
                                    }
                                case ItemUsage.ClaimGear:
                                    {
                                        var item = client.ClaimableItem[usage.UID];
                                        if (item != null)
                                        {
                                            if (item.Bound)
                                            {
                                                client.Send(new Message("Unnclaimable item!", System.Drawing.Color.Red, Message.TopLeft));
                                                return;
                                            }
                                            if (DateTime.Now < item.Date.AddDays(7) && item.OwnerUID != 500)
                                            {
                                                client.Send(new Message("This item is not expired. You cannot claim it yet!", System.Drawing.Color.Red, Message.TopLeft));
                                                return;
                                            }
                                            if (item.OwnerUID == 500)
                                                client.Entity.ConquerPoints += item.ConquerPointsCost;
                                            else
                                            {
                                                client.Inventory.Add(item.Item, Conquer_Online_Server.Game.Enums.ItemUse.Move);
                                                Message message = new Message("Thank you for arresting " + item.OwnerName + ", " + item.GainerName + ". The arrested one has redeemed his items and you have received a great deal of ConquerPoints as reward. Congratulations!", System.Drawing.Color.Wheat, Message.Talk);
                                                ServerBase.Kernel.SendWorldMessage(message, ServerBase.Kernel.GamePool.Values);
                                            }
                                            Database.DetainedItemTable.Claim(item, client);
                                            client.ClaimableItem.Remove(item.UID);

                                            usage.dwParam = client.Entity.UID;
                                            usage.dwExtraInfo3 = item.ConquerPointsCost;
                                            client.Send(usage);
                                        }
                                        else
                                        {
                                            client.Send(new Message("The item you want to claim has already been claimed.", System.Drawing.Color.Red, Message.TopLeft));
                                        }
                                        break;
                                    }
                                case 34:
                                    {
                                        break;
                                    }
                                case 45:
                                    {
                                        client.Entity.Teleport(1002, 429, 378);
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("Unhandled item usage type : " + usage.ID);
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                #endregion
                #region String (1015)
                case 1015:
                    {
                        if (client.Action != 2)
                            return;
                        _String stringpacket = new _String(false);
                        stringpacket.Deserialize(packet);
                        switch (stringpacket.Type)
                        {
                            case _String.WhisperDetails:
                                {
                                    if (stringpacket.Texts.Count > 0)
                                    {
                                        var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                        varr.MoveNext();
                                        int COunt = ServerBase.Kernel.GamePool.Count;
                                        for (uint x = 0;
                                            x < COunt;
                                            x++)
                                        {
                                            if (x >= COunt) break;

                                            Client.GameState pClient = (varr.Current as Client.GameState);

                                            if (pClient.Entity.Name == stringpacket.Texts[0])
                                            {
                                                string otherstring = "";
                                                otherstring += pClient.Entity.UID + " ";
                                                otherstring += pClient.Entity.Level + " ";
                                                otherstring += pClient.Entity.BattlePower + " #";
                                                if (pClient.Entity.GuildID != 0)
                                                    otherstring += pClient.Guild.Name + " fNone# ";
                                                else
                                                    otherstring += "None fNone# ";
                                                otherstring += pClient.Entity.Spouse + " ";
                                                otherstring += (byte)(pClient.Entity.NobilityRank) + " ";
                                                if (pClient.Entity.Body % 10 < 3)
                                                    otherstring += "1";
                                                else
                                                    otherstring += "0";
                                                stringpacket.Texts.Add(otherstring);
                                                client.Send(stringpacket);
                                            }

                                            varr.MoveNext();

                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region KnownPersons (1019)
                case 1019:
                    {
                        KnownPersons knownP = new KnownPersons(false);
                        knownP.Deserialize(packet);
                        switch (knownP.Type)
                        {
                            case KnownPersons.RequestFriendship:
                                {
                                    AddFriend(knownP, client);
                                    break;
                                }
                            case KnownPersons.RemovePerson:
                                {
                                    RemoveFriend(knownP, client);
                                    break;
                                }
                            case KnownPersons.RemoveEnemy:
                                {
                                    RemoveEnemy(knownP, client);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Attack (1022)
                case 1022:
                    {
                        if (client.Action != 2)
                            return;
                        uint dmg = BitConverter.ToUInt32(packet, 24);
                        uint AttackType = BitConverter.ToUInt32(packet, 20);
                        switch (AttackType)
                        {
                            case 39:
                                {
                                    if (!Kernel.VotePool.ContainsKey(client.Account.IP))
                                    {
                                        if (!Kernel.VotePoolUid.ContainsKey(client.Entity.UID))
                                        {
                                            client.Entity.ConquerPoints += 1000;
                                            Conquer_Online_Server.Game.ConquerStructures.PlayersVot Vot = new Conquer_Online_Server.Game.ConquerStructures.PlayersVot();
                                            Vot.AdressIp = client.Account.IP;
                                            Vot.Uid = client.Entity.UID;
                                            Kernel.VotePool.Add(Vot.AdressIp, Vot);
                                            Kernel.VotePoolUid.Add(Vot.Uid, Vot);
                                            Database.EntityTable.SavePlayersVot(Vot);
                                            client.Send(new Conquer_Online_Server.Network.GamePackets.Message("http://www.xtremetop100.com/in.php?site=1132311748", 2105));
                                            client.Send(new Message("You Have clamied your Prize 1k Cps Server ", Color.Brown, 1000000));
                                        }
                                        else
                                        {
                                            client.Send(new Conquer_Online_Server.Network.GamePackets.Message("http://www.xtremetop100.com/in.php?site=1132311748", 2105));
                                            client.Send(new Message("you can only take prize once in day", Color.Brown, 10000000));
                                        }
                                    }
                                    else
                                    {
                                        client.Send(new Conquer_Online_Server.Network.GamePackets.Message("http://www.xtremetop100.com/in.php?site=1132311748", 2105));
                                        client.Send(new Message("you can only take prize once in day", Color.Brown, 10000000));
                                    }

                                    break;
                                }
                            default:
                                {
                                    GamePackets.Attack attack = new Attack(false);
                                    attack.Deserialize(packet);
                                    if (client.Entity.ContainsFlag(Update.Flags.Ride) && !client.Entity.Owner.Equipment.Free(18))
                                    {
                                        Attack(attack, client);
                                        return;
                                    }
                                    else
                                    {
                                        client.Entity.RemoveFlag(Update.Flags.Ride);
                                    }
                                    Attack(attack, client);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Teams (1023)
                case 1023:
                    {
                        if (client.Action != 2)
                            return;
                        Team teamPacket = new Team();
                        teamPacket.Deserialize(packet);
                        switch (teamPacket.Type)
                        {
                            case Team.Create: CreateTeam(teamPacket, client); break;
                            case Team.AcceptJoinRequest: AcceptRequestToJoinTeam(teamPacket, client); break;
                            case Team.AcceptInvitation: AcceptInviteToJoinTeam(teamPacket, client); break;
                            case Team.InviteRequest: SendInviteToJoinTeam(teamPacket, client); break;
                            case Team.JoinRequest: SendRequestJoinToTeam(teamPacket, client); break;
                            case Team.ExitTeam: LeaveTeam(teamPacket, client); break;
                            case Team.Dismiss: DismissTeam(teamPacket, client); break;
                            case Team.Kick: KickFromTeam(teamPacket, client); break;
                            case Team.ForbidJoining:
                                {
                                    if (client.Team != null && client.Team.Teammates != null) foreach (Client.GameState Teammate in client.Team.Teammates)
                                            if (Teammate != null)
                                            {
                                                Teammate.Team.ForbidJoin = true;
                                                Teammate.Send(teamPacket);
                                            }

                                    break;
                                }
                            case Team.UnforbidJoining:
                                {
                                    if (client.Team != null && client.Team.Teammates != null) foreach (Client.GameState Teammate in client.Team.Teammates)
                                            if (Teammate != null)
                                            {
                                                Teammate.Team.ForbidJoin = false;
                                                Teammate.Send(teamPacket);
                                            }

                                    break;
                                }
                            case Team.LootMoneyOff:
                                {
                                    if (client.Team != null && client.Team.Teammates != null) foreach (Client.GameState Teammate in client.Team.Teammates)
                                            if (Teammate != null)
                                            {
                                                Teammate.Team.PickupMoney = true;
                                                Teammate.Send(teamPacket);
                                            }
                                    break;
                                }
                            case Team.LootMoneyOn:
                                {
                                    if (client.Team != null && client.Team.Teammates != null) foreach (Client.GameState Teammate in client.Team.Teammates)
                                            if (Teammate != null)
                                            {
                                                Teammate.Team.PickupMoney = false;
                                                Teammate.Send(teamPacket);
                                            }
                                    break;
                                }
                            case Team.LootItemsOn:
                                {
                                    if (client.Team != null && client.Team.Teammates != null) foreach (Client.GameState Teammate in client.Team.Teammates)
                                            if (Teammate != null)
                                            {
                                                Teammate.Team.PickupItems = true;
                                                Teammate.Send(teamPacket);
                                            }
                                    break;
                                }
                            case Team.LootItemsOff:
                                {
                                    if (client.Team != null && client.Team.Teammates != null) foreach (Client.GameState Teammate in client.Team.Teammates)
                                            if (Teammate != null)
                                            {
                                                Teammate.Team.PickupItems = false;
                                                Teammate.Send(teamPacket);
                                            }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Atributes Set (1024)
                case 1024:
                    {
                        if (client.Action != 2)
                            return;
                        ushort AddStr = BitConverter.ToUInt16(packet, 8);
                        ushort AddAgi = BitConverter.ToUInt16(packet, 12);
                        ushort AddVit = BitConverter.ToUInt16(packet, 16);
                        ushort AddSpi = BitConverter.ToUInt16(packet, 20);
                        if (AddStr > 0)
                        {
                            if (client.Entity.Atributes >= AddStr)
                            {
                                client.Entity.Atributes -= AddStr;
                                client.Entity.Strength += AddStr;
                            }
                        }
                        if (AddAgi > 0)
                        {
                            if (client.Entity.Atributes >= AddAgi)
                            {
                                client.Entity.Atributes -= AddAgi;
                                client.Entity.Agility += AddAgi;
                            }
                        }
                        if (AddVit > 0)
                        {
                            if (client.Entity.Atributes >= AddVit)
                            {
                                client.Entity.Atributes -= AddVit;
                                client.Entity.Vitality += AddVit;
                            }
                        }
                        if (AddSpi > 0)
                        {
                            if (client.Entity.Atributes >= AddSpi)
                            {
                                client.Entity.Atributes -= AddSpi;
                                client.Entity.Spirit += AddSpi;
                            }
                        }
                        //Conquer_Online_Server.Game.Attacking.Calculate.Vitals(client.Entity, true);
                        client.CalculateStatBonus();
                        client.CalculateHPBonus();
                        client.GemAlgorithm();
                        client.SendStatMessage();
                        break;
                    }
                #endregion
                #region Socketing (1027)
                case 1027:
                    {
                        EmbedSocket socket = new EmbedSocket(false);
                        socket.Deserialize(packet);
                        SocketItem(socket, client);
                        break;
                    }
                #endregion
                #region ItemAdding Stabilization
                case 1038:
                    {
                        ItemAddingStabilization stabilization = new ItemAddingStabilization(false);
                        stabilization.Deserialize(packet);
                        StabilazeArtifact(stabilization, client);
                        break;
                    }
                #endregion
                #region LoginPacket (1052)
                case 1052:
                    {
                        if (client.Action == 1)
                        {
                            Connect connect = new Connect();
                            connect.Deserialize(packet);
                            AppendConnect(connect, client);
                        }
                        else
                            client.Disconnect();
                        break;
                    }
                #endregion
                #region Trade (1056)
                case 1056:
                    {
                        if (client.Action != 2)
                            return;
                        Trade trade = new Trade(false);
                        trade.Deserialize(packet);
                        switch (trade.Type)
                        {
                            case Trade.Request:
                                RequestTrade(trade, client);
                                break;
                            case Trade.Close:
                                CloseTrade(trade, client);
                                break;
                            case Trade.AddItem:
                                AddTradeItem(trade, client);
                                break;
                            case Trade.SetMoney:
                                SetTradeMoney(trade, client);
                                break;
                            case Trade.SetConquerPoints:
                                SetTradeConquerPoints(trade, client);
                                break;
                            case Trade.Accept:
                                AcceptTrade(trade, client);
                                break;
                        }
                        break;
                    }
                #endregion
                #region Floor items (1101)
                case 1101:
                    {
                        if (client.Action != 2)
                            return;
                        FloorItem floorItem = new FloorItem(false);
                        floorItem.Deserialize(packet);
                        PickupItem(floorItem, client);
                        break;
                    }
                #endregion
                #region Warehouses (1102)
                case 1102:
                    {
                        if (client.Action != 2)
                            return;
                        Warehouse warehousepacket = new Warehouse(false);
                        warehousepacket.Deserialize(packet);
                        switch (warehousepacket.Type)
                        {
                            case Warehouse.Entire:
                                {
                                    Game.ConquerStructures.Warehouse wh = client.Warehouses[(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID)warehousepacket.NpcID];
                                    if (wh == null) return;
                                    byte count = 0;
                                    warehousepacket.Count = 1;
                                    warehousepacket.Type = Warehouse.AddItem;
                                    for (; count < wh.Count; count++)
                                    {
                                        warehousepacket.Append(wh.Objects[count]);
                                        client.Send(warehousepacket);
                                    }
                                    break;
                                }
                            case Warehouse.AddItem:
                                {
                                    Game.ConquerStructures.Warehouse wh = client.Warehouses[(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID)warehousepacket.NpcID];
                                    if (wh == null) return;
                                    Interfaces.IConquerItem item = null;
                                    if (client.Inventory.TryGetItem(warehousepacket.UID, out item))
                                    {
                                        if (wh.Add(item))
                                        {
                                            warehousepacket.UID = 0;
                                            warehousepacket.Count = 1;
                                            warehousepacket.Append(item);
                                            client.Send(warehousepacket);
                                            return;
                                        }
                                    }
                                    break;
                                }
                            case Warehouse.RemoveItem:
                                {
                                    Game.ConquerStructures.Warehouse wh = client.Warehouses[(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID)warehousepacket.NpcID];
                                    if (wh == null) return;
                                    if (wh.ContainsUID(warehousepacket.UID))
                                    {
                                        if (wh.Remove(warehousepacket.UID))
                                        {
                                            client.Send(warehousepacket);
                                            return;
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine("Unknown type: " + warehousepacket.Type);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Guild command (1107)
                case 1107:
                    {
                        GuildCommand command = new GuildCommand(false);
                        command.Deserialize(packet);
                        switch (command.Type)
                        {
                            case GuildCommand.Neutral1:
                            case GuildCommand.Neutral2:
                                {
                                    string name = System.Text.ASCIIEncoding.ASCII.GetString(packet, 26, packet[25]);
                                    if (client.Guild != null)
                                    {
                                        if (client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        {
                                            client.Guild.RemoveAlly(name);
                                            foreach (var guild in ServerBase.Kernel.Guilds.Values)
                                            {
                                                if (guild.Name == name && client.Guild.Name != name)
                                                {
                                                    guild.RemoveAlly(client.Guild.Name);
                                                }
                                            }
                                            client.Guild.RemoveEnemy(name);
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.Allied:
                                {
                                    string name = System.Text.ASCIIEncoding.ASCII.GetString(packet, 26, packet[25]);
                                    if (client.Guild != null)
                                    {
                                        if (client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        {
                                            AllyGuilds(name, client);
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.Enemied:
                                {
                                    string name = System.Text.ASCIIEncoding.ASCII.GetString(packet, 26, packet[25]);
                                    if (client.Guild != null)
                                    {
                                        if (client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        {
                                            client.Guild.AddEnemy(name);
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    client.Send(packet);
                                    break;
                                }
                            case GuildCommand.Bulletin:
                                {
                                    string message = System.Text.ASCIIEncoding.ASCII.GetString(packet, 26, packet[25]);
                                    if (client.Guild != null)
                                    {
                                        if (client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        {
                                            client.Guild.Bulletin = message;
                                            client.Guild.SendGuild(client);
                                            Database.GuildTable.UpdateBulletin(client.Guild, client.Guild.Bulletin);
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.DonateSilvers:
                                {
                                    if (client.Guild != null)
                                    {
                                        if (client.Entity.Money >= command.dwParam)
                                        {
                                            client.Guild.SilverFund += command.dwParam;
                                            Database.GuildTable.SaveFunds(client.Guild);
                                            client.AsMember.SilverDonation += command.dwParam;
                                            client.Entity.Money -= command.dwParam;
                                            client.Guild.SendGuild(client);
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.DonateConquerPoints:
                                {
                                    if (client.Guild != null)
                                    {
                                        if (client.Entity.ConquerPoints >= command.dwParam)
                                        {
                                            client.Guild.ConquerPointFund += command.dwParam;
                                            Database.GuildTable.SaveFunds(client.Guild);
                                            client.AsMember.ConquerPointDonation += command.dwParam;
                                            client.Entity.ConquerPoints -= command.dwParam;
                                            client.Guild.SendGuild(client);
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.Refresh:
                                {
                                    if (client.AsMember != null)
                                    {
                                        if (client.Guild != null)
                                            client.Guild.SendGuild(client);
                                    }
                                    break;
                                }
                            case GuildCommand.Discharge:
                                {
                                    string name = System.Text.ASCIIEncoding.ASCII.GetString(packet, 26, packet[25]);
                                    if (client.Guild != null)
                                    {
                                        if (client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        {
                                            var member = client.Guild.GetMemberByName(name);
                                            if (member.ID != client.Entity.UID)
                                            {
                                                if (member.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                                {
                                                    client.Guild.DeputyLeaderCount--;
                                                    member.Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.Member;
                                                    if (member.IsOnline)
                                                    {
                                                        client.Guild.SendGuild(member.Client);
                                                        member.Client.Entity.GuildRank = (ushort)member.Rank;
                                                        member.Client.Screen.FullWipe();
                                                        member.Client.Screen.Reload(null);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.Promote:
                                {
                                    if (client.Guild != null)
                                    {
                                        if (client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        {
                                            if (client.Guild.Members.ContainsKey(command.dwParam))
                                            {
                                                var member = client.Guild.Members[command.dwParam];
                                                if (member.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                {
                                                    member.Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader;
                                                    if (member.IsOnline)
                                                    {
                                                        client.Guild.SendGuild(member.Client);
                                                        member.Client.Entity.GuildRank = (ushort)member.Rank;
                                                        member.Client.Screen.FullWipe();
                                                        member.Client.Screen.Reload(null);
                                                    }
                                                }
                                                else if (member.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                                {
                                                    member.Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader;
                                                    if (member.IsOnline)
                                                    {
                                                        client.Guild.SendGuild(member.Client);
                                                        member.Client.Entity.GuildRank = (ushort)member.Rank;
                                                        member.Client.Screen.FullWipe();
                                                        member.Client.Screen.Reload(null);
                                                    }
                                                    client.AsMember.Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader;
                                                    client.Guild.SendGuild(client);
                                                    client.Entity.GuildRank = (ushort)client.AsMember.Rank;
                                                    client.Screen.FullWipe();
                                                    client.Screen.Reload(null);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.JoinRequest:
                                {
                                    if (ServerBase.Kernel.GamePool.ContainsKey(command.dwParam))
                                    {
                                        var Client = ServerBase.Kernel.GamePool[command.dwParam];
                                        if (Client.OnHoldGuildJoin == client.OnHoldGuildJoin && Client.OnHoldGuildJoin != 0)
                                        {
                                            if (Client.Guild != null)
                                            {
                                                if (Client.AsMember.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                {
                                                    Client.Guild.AddMember(client);
                                                    Client.OnHoldGuildJoin = 0;
                                                    client.OnHoldGuildJoin = 0;
                                                }
                                            }
                                            else
                                            {
                                                if (client.AsMember.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                {
                                                    client.Guild.AddMember(Client);
                                                    Client.OnHoldGuildJoin = 0;
                                                    client.OnHoldGuildJoin = 0;
                                                }
                                            }
                                            return;
                                        }
                                        if (client.Guild == null)
                                        {
                                            command.dwParam = client.Entity.UID;
                                            Client.Send(command);
                                            Client.OnHoldGuildJoin = (uint)new Random().Next();
                                            client.OnHoldGuildJoin = Client.OnHoldGuildJoin;
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.InviteRequest:
                                {
                                    if (ServerBase.Kernel.GamePool.ContainsKey(command.dwParam))
                                    {
                                        var Client = ServerBase.Kernel.GamePool[command.dwParam];
                                        if (Client.OnHoldGuildJoin == client.OnHoldGuildJoin && Client.OnHoldGuildJoin != 0)
                                        {
                                            if (Client.Guild != null)
                                            {
                                                if (Client.AsMember.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                {
                                                    Client.Guild.AddMember(client);
                                                    Client.OnHoldGuildJoin = 0;
                                                    client.OnHoldGuildJoin = 0;
                                                }
                                            }
                                            else
                                            {
                                                if (client.AsMember.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                {
                                                    client.Guild.AddMember(Client);
                                                    Client.OnHoldGuildJoin = 0;
                                                    client.OnHoldGuildJoin = 0;
                                                }
                                            }
                                            return;
                                        }
                                        if (client.Guild != null)
                                        {
                                            command.dwParam = client.Entity.UID;
                                            Client.Send(command);
                                            Client.OnHoldGuildJoin = (uint)new Random().Next();
                                            client.OnHoldGuildJoin = Client.OnHoldGuildJoin;
                                        }
                                    }
                                    break;
                                }
                            case GuildCommand.Quit:
                                {
                                    if (client.Guild != null)
                                    {
                                        if (client.AsMember.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                        {
                                            client.Guild.ExpelMember(client.Entity.Name, true);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Enlight (1127)
                case 1127:
                    {
                        Enlight enlight = new Enlight(false);
                        enlight.Deserialize(packet);
                        if (ServerBase.Kernel.GamePool.ContainsKey(enlight.Enlighted))
                        {
                            var Client = ServerBase.Kernel.GamePool[enlight.Enlighted];

                            if (enlight.Enlighter == client.Entity.UID && enlight.Enlighted != enlight.Enlighter)
                            {
                                if (Client.Entity.ReceivedEnlightenPoints < 5)
                                {
                                    if (client.Entity.EnlightenPoints >= 100)
                                    {
                                        if (Client.Entity.EnlightmentTime <= 80)
                                        {
                                            client.Entity.EnlightenPoints -= 100;
                                            Client.Entity.EnlightmentStamp = Time32.Now;
                                            Client.IncreaseExperience(Game.Attacking.Calculate.Percent((int)Client.ExpBall, .10F), false);
                                            Client.SendScreen(packet, true);
                                            Client.Entity.ReceivedEnlightenPoints++;
                                            Client.Entity.EnlightmentTime += 20;
                                            if (client.Entity.EnlightmentTime > 80)
                                                client.Entity.EnlightmentTime = 100;
                                            else if (client.Entity.EnlightmentTime > 60)
                                                client.Entity.EnlightmentTime = 80;
                                            else if (client.Entity.EnlightmentTime > 40)
                                                client.Entity.EnlightmentTime = 60;
                                            else if (client.Entity.EnlightmentTime > 20)
                                                client.Entity.EnlightmentTime = 40;
                                            else if (client.Entity.EnlightmentTime > 0)
                                                client.Entity.EnlightmentTime = 20;
                                        }
                                        else client.Send(new Message("You can't enlighten " + Client.Entity.Name + " yet because he has to wait a few minutes until he can be enlightened again.", System.Drawing.Color.Blue, Message.TopLeft));
                                    }
                                    else client.Send(new Message("You can't enlighten " + Client.Entity.Name + " because you don't have enough enlighten points!", System.Drawing.Color.Blue, Message.TopLeft));
                                }
                                else client.Send(new Message("You can't enlighten " + Client.Entity.Name + " because he/she was enlightened today five times already!", System.Drawing.Color.Blue, Message.TopLeft));
                            }
                        }
                        break;
                    }
                #endregion
                #region NPC Dialog (2031 + 2032)
                case 2031:
                case 2032:
                    {
                        if (client.Action != 2)
                            return;
                        NpcRequest req = new NpcRequest();
                        req.Deserialize(packet);
                        if (req.InteractType == NpcReply.MessageBox)
                        {
                            if (req.OptionID == 255)
                            {
                                if (client.OnMessageBoxOK != null)
                                {
                                    client.OnMessageBoxOK.Invoke();
                                    client.OnMessageBoxOK = null;
                                }
                            }
                            else
                            {
                                if (client.OnMessageBoxCANCEL != null)
                                {
                                    client.OnMessageBoxCANCEL.Invoke();
                                    client.OnMessageBoxCANCEL = null;
                                }
                            }
                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 249)
                        {
                            Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 248)
                        {
                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));
                   
                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1507, 100, 100);
                            }
                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 247)
                        {

                            client.Entity.Teleport(1858, 70, 70);
                            
                        }
                            
                        

                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 246)
                        {

                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));

                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1787, 50, 50);
                            }
                        }

                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 245)
                        {
                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));

                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1787, 50, 50);
                            }
                        }



                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 244)
                        {
                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));

                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1787, 50, 50);
                            }
               

                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 243)
                        {
                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));

                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1787, 50, 50);
                            }
                  

                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 242)
                        {
                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));

                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1787, 50, 50);

                            }
                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 241)
                        {
                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));

                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1787, 50, 50);
                            }
                    
                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 240)
                        {
                            if (ServerBase.Kernel.PK == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The Tournoment has finished time of join."));

                            }
                            else
                            {
                                client.InPKT = true;
                                PKTournament.PKTHash.Add(client.Entity.UID, client);
                                //Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.AddMap(client);
                                client.Entity.Teleport(1787, 50, 50);
                            }
                           
                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 238)
                        {

                            client.Entity.Teleport(2555, 50, 50);

                        }
                        if (client.Map.BaseID != 6001 && !client.Entity.Dead && req.OptionID == 239)
                        {
                            if (Program.Steed == false)
                            {
                                client.Send(new Network.GamePackets.NpcReply(6, "The SteedRace has finished time of join."));

                            }
                            else
                            {
                                Conquer_Online_Server.Interfaces.IConquerItem I = client.Equipment.TryGetItem(12);
                                if (I != null && I.ID != 0)
                                {
                                    client.Entity.srjoin = 1;
                                    client.Entity.Teleport(1950, 077, 157);
                                    client.Entity.AddFlag(Update.Flags.Ride);
                                }
                                else
                                {
                                    client.Send(new Network.GamePackets.NpcReply(6, "You Dont have a Steed ."));
                                }
                            }

                        }
                        else
                        {
                            if (ID == 2031)
                                client.ActiveNpc = req.NpcID;
                            if (req.NpcID == 12)
                            {
                                if (client.Entity.VIPLevel > 0)
                                {
                                    Data data = new Data(true);
                                    data.ID = Data.OpenWindow;
                                    data.UID = client.Entity.UID;
                                    data.TimeStamp = Time32.Now;
                                    data.dwParam = Data.WindowCommands.VIPWarehouse;
                                    data.wParam1 = client.Entity.X;
                                    data.wParam2 = client.Entity.Y;
                                    client.WarehouseOpen = true;
                                    client.Send(data);
                                    break;
                                }
                            }
                            Interfaces.INpc npc = null;
                            if (req.InteractType == 102)
                            {
                                if (client.Guild != null)
                                {
                                    if (client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                    {
                                        client.Guild.ExpelMember(req.Input, false);
                                    }
                                }
                                return;
                            }
                            if (client.Map.Npcs.TryGetValue(client.ActiveNpc, out npc))
                            {
                                if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, npc.X, npc.Y) > 17)
                                    return;
                                if (req.OptionID == 255 || (req.OptionID == 0 && ID == 2032))
                                    return;
                                req.NpcID = client.ActiveNpc;
                                //NpcDialogs.Dialogs.GetDialog(req, client);
                                Dialogs.GetDialog(req, client);
                            }
                        }
                        break;
                    }
                #endregion
                #region Compose (2036)
                case 2036:
                    {
                        Compose compose = new Compose(false);
                        compose.Deserialize(packet);
                        ComposePlus(compose, client);
                        break;
                    }
                #endregion
                #region Offline TG (2044)
                case 2044:
                    {
                        OfflineTGRequest otgr = new OfflineTGRequest(false);
                        otgr.Deserialize(packet);
                        switch (otgr.ID)
                        {
                            case OfflineTGRequest.OnTrainingTimeRequested:
                                {
                                    otgr.Minutes = 900;
                                    client.Send(otgr);
                                    break;
                                }
                            case OfflineTGRequest.OnConfirmation:
                                {
                                    client.Entity.PreviousMapID = client.Entity.MapID;
                                    client.Entity.PrevX = client.Entity.X;
                                    client.Entity.PrevY = client.Entity.Y;
                                    client.Entity.MapID = 601;
                                    client.Entity.X = 64;
                                    client.Entity.Y = 56;
                                    client.OfflineTGEnterTime = DateTime.Now;

                                    client.Disconnect();
                                    break;
                                }
                            case OfflineTGRequest.ClaimExperience:
                                {
                                    var T1 = new TimeSpan(DateTime.Now.Ticks);
                                    var T2 = new TimeSpan(client.OfflineTGEnterTime.Ticks);
                                    ushort minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                                    minutes = (ushort)Math.Min((ushort)900, minutes);
                                    double expballGain = (double)300 * (double)minutes / (double)900;
                                    while (expballGain >= 100)
                                    {
                                        expballGain -= 100;
                                        client.IncreaseExperience(client.ExpBall, false);
                                    }
                                    if (expballGain != 0)
                                        client.IncreaseExperience((uint)(client.ExpBall * (expballGain / 100)), false);

                                    client.Entity.SetLocation(client.Entity.PreviousMapID, client.Entity.PrevX, client.Entity.PrevY);
                                    if (client.Map.ID == 1036 || client.Map.ID == 1039)
                                        client.Entity.Teleport(1002, 430, 378);
                                    else
                                    {
                                        switch (client.Map.ID)
                                        {
                                            default:
                                                {
                                                    client.Entity.Teleport(1002, 429, 378);
                                                    break;
                                                }
                                            case 1000:
                                                {
                                                    client.Entity.Teleport(1000, 500, 650);
                                                    break;
                                                }
                                            case 1020:
                                                {
                                                    client.Entity.Teleport(1020, 565, 562);
                                                    break;
                                                }
                                            case 1011:
                                                {
                                                    client.Entity.Teleport(1011, 188, 264);
                                                    break;
                                                }
                                            case 6001:
                                                {
                                                    client.Entity.Teleport(6001, 030, 075);
                                                    break;
                                                }

                                            case 1015:
                                                {
                                                    client.Entity.Teleport(1015, 717, 571);
                                                    break;
                                                }
                                        }
                                    }
                                    client.OfflineTGEnterTime = DateTime.Now;
                                    break;
                                }
                            default:
                                client.Send(otgr);
                                break;
                        }
                        break;
                    }
                #endregion
                #region Trade partner (2046)
                case 2046:
                    {
                        TradePartner partner = new TradePartner(false);
                        partner.Deserialize(packet);
                        switch (partner.Type)
                        {
                            case TradePartner.RequestPartnership:
                                RequestTradePartnership(partner, client);
                                break;
                            case TradePartner.RejectRequest:
                                RejectPartnership(partner, client);
                                break;
                            case TradePartner.BreakPartnership:
                                BreakPartnership(partner, client);
                                break;
                        }
                        break;
                    }
                #endregion
                #region ItemLock (2048)
                case 2048:
                    {
                        if (client.Action != 2)
                            return;
                        ItemLock itemlock = new ItemLock(false);
                        itemlock.Deserialize(packet);
                        switch (itemlock.ID)
                        {
                            case ItemLock.RequestLock:
                                LockItem(itemlock, client);
                                break;
                            case ItemLock.RequestUnlock:
                                UnlockItem(itemlock, client);
                                break;
                        }
                        break;
                    }
                #endregion
                #region Broadcast (2050)
                case 2050:
                    {
                        Broadcast cast = new Broadcast(false);
                        cast.Deserialize(packet);
                        switch (cast.Type)
                        {
                            case Broadcast.ReleaseSoonMessages:
                                {
                                    BroadcastInfoAwaiting(cast, client);
                                    break;
                                }
                            case Broadcast.MyMessages:
                                {
                                    BroadcastClientMessages(cast, client);
                                    break;
                                }
                            case Broadcast.BroadcastMessage:
                                {
                                    if (Game.ConquerStructures.Broadcast.Broadcasts.Count == ServerBase.Constants.MaxBroadcasts)
                                    {
                                        client.Send(new Message("You cannot send any broadcasts for now. The limit has been reached. Wait until a broadcast is chopped down.", System.Drawing.Color.Red, Message.TopLeft));
                                        break;
                                    }
                                    if (client.Entity.ConquerPoints >= 5)
                                    {
                                        client.Entity.ConquerPoints = (uint)Math.Max(0, (int)((int)client.Entity.ConquerPoints - (int)5));
                                        Game.ConquerStructures.Broadcast.BroadcastStr broadcast = new Conquer_Online_Server.Game.ConquerStructures.Broadcast.BroadcastStr();
                                        broadcast.EntityID = client.Entity.UID;
                                        broadcast.EntityName = client.Entity.Name;
                                        broadcast.ID = Game.ConquerStructures.Broadcast.BroadcastCounter.Next;
                                        if (cast.List[0].Length > 80)
                                            cast.List[0] = cast.List[0].Remove(80);
                                        broadcast.Message = cast.List[0];
                                        if (Game.ConquerStructures.Broadcast.Broadcasts.Count == 0)
                                        {
                                            if (Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityID == 1)
                                            {
                                                Game.ConquerStructures.Broadcast.CurrentBroadcast = broadcast;
                                                Game.ConquerStructures.Broadcast.LastBroadcast = DateTime.Now;
                                                ServerBase.Kernel.SendWorldMessage(new Message(cast.List[0], "ALLUSERS", client.Entity.Name, System.Drawing.Color.Red, Message.BroadcastMessage), ServerBase.Kernel.GamePool.Values);
                                                client.Send(cast);
                                                break;
                                            }
                                        }
                                        Game.ConquerStructures.Broadcast.Broadcasts.Add(broadcast);
                                        cast.dwParam = (uint)Game.ConquerStructures.Broadcast.Broadcasts.Count;
                                        client.Send(cast);
                                        break;
                                    }
                                    break;
                                }
                            case Broadcast.Urgen5CPs:
                                {
                                    for (int c = 0; c < Game.ConquerStructures.Broadcast.Broadcasts.Count; c++)
                                    {
                                        var broadcast = Game.ConquerStructures.Broadcast.Broadcasts[c];
                                        if (broadcast.ID == cast.dwParam)
                                        {
                                            if (c != 0)
                                            {
                                                if (client.Entity.ConquerPoints >= 5)
                                                {
                                                    broadcast.SpentCPs += 5;
                                                    client.Entity.ConquerPoints = (uint)Math.Max(0, (int)((int)client.Entity.ConquerPoints - (int)5));
                                                    if (Game.ConquerStructures.Broadcast.Broadcasts[c - 1].SpentCPs <= broadcast.SpentCPs)
                                                    {
                                                        Game.ConquerStructures.Broadcast.Broadcasts[c] = Game.ConquerStructures.Broadcast.Broadcasts[c - 1];
                                                        Game.ConquerStructures.Broadcast.Broadcasts[c - 1] = broadcast;
                                                    }
                                                    else
                                                    {
                                                        Game.ConquerStructures.Broadcast.Broadcasts[c] = broadcast;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case Broadcast.Urgen15CPs:
                                {
                                    for (int c = 0; c < Game.ConquerStructures.Broadcast.Broadcasts.Count; c++)
                                    {
                                        var broadcast = Game.ConquerStructures.Broadcast.Broadcasts[c];
                                        if (broadcast.ID == cast.dwParam)
                                        {
                                            if (c != 0)
                                            {
                                                if (client.Entity.ConquerPoints >= 15)
                                                {
                                                    broadcast.SpentCPs += 15;
                                                    client.Entity.ConquerPoints = (uint)Math.Max(0, (int)((int)client.Entity.ConquerPoints - (int)15));
                                                    for (int b = c - 1; b > 0; b--)
                                                        Game.ConquerStructures.Broadcast.Broadcasts[b] = Game.ConquerStructures.Broadcast.Broadcasts[b - 1];

                                                    Game.ConquerStructures.Broadcast.Broadcasts[0] = broadcast;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Nobility (2064)
                case 2064:
                    {
                        NobilityInfo nobility = new NobilityInfo(false);
                        nobility.Deserialize(packet);
                        Game.ConquerStructures.Nobility.Handle(nobility, client);
                        break;
                    }
                #endregion
                #region Mentor prize (2067)
                #region MentorPremio
                case 1036:
                    {
                        switch (packet[4])
                        {
                            #region Send
                            case 0:
                                // Writer.WriteUInt32(client.Entity.Contribution_Experience.Training_Exp, 8, packet);
                                //Writer.WriteUInt32(client.Entity.Contribution_Experience.Blessing_Exp, 12, packet);
                                client.Send(packet);
                                break;
                            #endregion
                            #region Claim Training Exp
                            case 1:
                                {
                                    if (client.Entity.Contribution_Experience.Blessing_Exp > 0)
                                    {
                                        ulong Increase = (ulong)(client.ExpBall * (client.Entity.Contribution_Experience.Training_Exp / 6000000));
                                        client.Entity.Contribution_Experience.Training_Exp = 0;
                                        client.IncreaseExperience(Increase,true);
                                        client.Send(packet);
                                    }
                                    break;
                                }
                            #endregion
                            #region Claim Blessing Exp
                            case 2:
                                {
                                    if (client.Entity.Contribution_Experience.Blessing_Exp > 0)
                                    {
                                        ulong Increase = (ulong)(client.ExpBall * (client.Entity.Contribution_Experience.Blessing_Exp / 6000000));
                                        client.Entity.Contribution_Experience.Blessing_Exp = 0;
                                        client.IncreaseExperience(Increase,true);
                                        client.Send(packet);
                                    }
                                    break;
                                }
                            #endregion
                            default: Console.WriteLine("Unknown 1036 claim type " + packet[4]); break;
                        }
                        break;
                    }
                #endregion  
                case 2067:
                    {
                        MentorPrize prize = new MentorPrize(false);
                        prize.Deserialize(packet);
                        switch (prize.Prize_Type)
                        {
                            case MentorPrize.ClaimExperience:
                                {
                                   // client.IncreaseExperience((ulong)(((double)client.PrizeExperience / 606) * client.ExpBall), false);
                                    client.PrizeExperience = 0;
                                    foreach (var appr in client.Apprentices.Values)
                                    {
                                        appr.Actual_Experience = 0;
                                        Database.KnownPersons.SaveApprenticeInfo(appr);
                                    }
                                    prize.Mentor_ID = client.Entity.UID;
                                    prize.Prize_Type = MentorPrize.Show;
                                    prize.Prize_Experience = client.PrizeExperience;
                                    prize.Prize_HeavensBlessing = client.PrizeHeavenBlessing;
                                    prize.Prize_PlusStone = client.PrizePlusStone;
                                    client.Send(prize);
                                    break;
                                }
                            case MentorPrize.ClaimHeavenBlessing:
                                {
                                    client.AddBless(client.PrizeHeavenBlessing);
                                    client.PrizeHeavenBlessing = 0;
                                    foreach (var appr in client.Apprentices.Values)
                                    {
                                        appr.Actual_HeavenBlessing = 0;
                                        Database.KnownPersons.SaveApprenticeInfo(appr);
                                    }
                                    prize.Mentor_ID = client.Entity.UID;
                                    prize.Prize_Type = MentorPrize.Show;
                                    prize.Prize_Experience = client.PrizeExperience;
                                    prize.Prize_HeavensBlessing = client.PrizeHeavenBlessing;
                                    prize.Prize_PlusStone = client.PrizePlusStone;
                                    client.Send(prize);
                                    break;
                                }
                            case MentorPrize.ClaimPlus:
                                {
                                    int stones = client.PrizePlusStone / 100;
                                    int totake = stones;
                                    if (stones > 0)
                                    {
                                        for (; stones > 0; stones--)
                                        {
                                            client.PrizePlusStone -= 100;
                                            if (!client.Inventory.Add(730001, 1, 1))
                                                break;
                                        }
                                    }
                                    foreach (var appr in client.Apprentices.Values)
                                    {
                                        if (appr.Actual_Plus >= totake)
                                        {
                                            appr.Actual_Plus -= (ushort)totake;
                                            totake = 0;
                                        }
                                        else
                                        {
                                            totake -= appr.Actual_Plus;
                                            appr.Actual_Plus = 0;
                                        }
                                        Database.KnownPersons.SaveApprenticeInfo(appr);
                                    }
                                    prize.Mentor_ID = client.Entity.UID;
                                    prize.Prize_Type = MentorPrize.Show;
                                    prize.Prize_Experience = client.PrizeExperience;
                                    prize.Prize_HeavensBlessing = client.PrizeHeavenBlessing;
                                    prize.Prize_PlusStone = client.PrizePlusStone;
                                    client.Send(prize);
                                    break;
                                }
                            case MentorPrize.Show:
                                {
                                    prize.Mentor_ID = client.Entity.UID;
                                    prize.Prize_Type = MentorPrize.Show;
                                    prize.Prize_Experience = client.PrizeExperience;
                                    prize.Prize_HeavensBlessing = client.PrizeHeavenBlessing;
                                    prize.Prize_PlusStone = client.PrizePlusStone;
                                    client.Send(prize);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region MentorApprentice (2065)
                case 2065:
                    {
                        MentorApprentice ma = new MentorApprentice(false);
                        ma.Deserialize(packet);
                        switch (ma.Type)
                        {
                            case MentorApprentice.LeaveMentor:
                                {
                                    LeaveMentor(ma, client);
                                    break;
                                }
                            case MentorApprentice.ExpellApprentice:
                                {
                                    ExpelApprentice(ma, client);
                                    break;
                                }
                            case MentorApprentice.RequestApprentice:
                                {
                                    AddApprentice(ma, client);
                                    break;
                                }
                            case MentorApprentice.RequestMentor:
                                {
                                    AddMentor(ma, client);
                                    break;
                                }
                            case MentorApprentice.AcceptRequestApprentice:
                                {
                                    AcceptRequestApprentice(ma, client);
                                    break;
                                }
                            case MentorApprentice.AcceptRequestMentor:
                                {
                                    AcceptRequestMentor(ma, client);
                                    break;
                                }
                        }
                        break;
                    }
                case 2066:
                    {
                        MentorInformation info = new MentorInformation(false);
                        info.Deserialize(packet);
                        if (info.Mentor_Type == 1)
                        {
                            client.ReviewMentor();
                        }
                        break;
                    }
                #endregion
                #region PurifyItem (2076)
                case 2076:
                    {
                        Purification ps = new Purification(false);
                        ps.Deserialize(packet);
                        switch (ps.Mode)
                        {
                            case Purification.Purify:
                                PurifyItem(ps, client); break;
                            case Purification.Purify1:
                                new Game.Features.Refinery.Handle(packet, client);
                                break;
                        }
                        break;
                    }
                #endregion
                
                #region Arsenal Guild
                case 2202:
                    {

                        ArsenalInscribitionList list = new ArsenalInscribitionList();
                        client.Send(list.Build(packet, client.Guild));
                        break;
                    }
                case 2203:
                    {
                        client.Guild.Arsenal.Update(client.Guild);
                        client.Send(client.Guild.A_Packet);
                        Database.ArsenalsTable.Load(client.Guild);
                        #region Handle
                        byte pType = packet[4];
                        byte i_type = packet[8];
                        uint i_Uid = BitConverter.ToUInt32(packet, 12);
                        switch (pType)
                        {
                            case 0://Unlock
                                {
                                    uint value = 5000000;
                                    if (client.Guild.SilverFund >= value)
                                    {

                                        if (i_type == 2 || i_type == 3)
                                        { value = 10000000; }
                                        if (i_type >= 3 && i_type <= 5)
                                        { value = 15000000; }
                                        if (i_type == 6 || i_type == 7)
                                        { value = 20000000; }
                                        if (client.Guild.SilverFund >= value)
                                        {
                                            client.Guild.SilverFund -= value;
                                            Database.ArsenalsTable.CreateArsenal((ushort)client.Guild.ID, (Game.ConquerStructures.Society.ArsenalType)i_type);
                                        }
                                        else
                                            client.Send(new Network.GamePackets.Message("sorry, you need " + value + " guild Found", System.Drawing.Color.Red, Message.TopLeft).ToArray());
                                    }
                                    else
                                        client.Send(new Network.GamePackets.Message("sorry, you need " + value + " guild Found", System.Drawing.Color.Red, Message.TopLeft).ToArray());
                                    break;
                                }
                            case 1://Inscribe
                                {
                                    Interfaces.IConquerItem Item = null;
                                    if (client.Inventory.TryGetItem(i_Uid, out Item))
                                    {
                                        client.Guild.Arsenal.Inscribe((Game.ConquerStructures.Society.ArsenalType)i_type, Item, client);
                                        client.Guild.Arsenal.Update((Game.ConquerStructures.Society.ArsenalType)i_type, client.Guild);
                                    }
                                    break;
                                }
                            case 2://Uninscribe
                                {
                                    Game.ConquerStructures.Society.ArsenalSingle AS = null;
                                    if (client.Guild.Arsenal.Inscribed[(Game.ConquerStructures.Society.ArsenalType)i_type].TryGetValue(i_Uid, out AS))
                                    {
                                        client.Guild.Arsenal.Uninscribe((Game.ConquerStructures.Society.ArsenalType)i_type, AS, client);
                                        client.Guild.Arsenal.Update((Game.ConquerStructures.Society.ArsenalType)i_type, client.Guild);
                                    }
                                    break;
                                }
                            case 3://Enhance
                                {
                                    //client.Entity.Mentors.Share();
                                    client.Guild.A_Packet.SetTotals2();
                                    client.Send(client.Guild.A_Packet);
                                    break;
                                }
                            case 4://Show Main Info
                                {
                                    client.Guild.A_Packet.SetTotals();
                                    client.Send(client.Guild.A_Packet);
                                    break;
                                }
                        }
                        client.Send(packet);
                        #endregion
                        break;
                    }
                #endregion
                #region Arena (2207<->2211)
                case 2207://Request Arena ranking List
                    {
                        //Code snippet that belongs to Ultimation
                        ushort PageIndex = BitConverter.ToUInt16(packet, 6);
                        Game.ConquerStructures.Arena.Statistics.ShowRankingPage(packet[4], PageIndex, client);
                        break;
                    }
                case 2206:
                    {
                        //Code snippet that belongs to Ultimation
                        ushort PageIndex = BitConverter.ToUInt16(packet, 4);
                        Game.ConquerStructures.Arena.QualifyEngine.RequestGroupList(client, PageIndex);
                        break;
                    }
                case 2205://Arena Signup!
                    {
                        //Code snippet that belongs to Ultimation
                        uint DialogID = BitConverter.ToUInt32(packet, 4);
                        uint ButtonID = BitConverter.ToUInt32(packet, 8);
                        switch (DialogID)
                        {
                            case 4:
                                {
                                    switch (ButtonID)
                                    {
                                        case 0:
                                            {
                                                Game.ConquerStructures.Arena.QualifyEngine.DoQuit(client);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 0: Game.ConquerStructures.Arena.QualifyEngine.DoSignup(client); client.Send(packet); break;
                            case 1: Game.ConquerStructures.Arena.QualifyEngine.DoQuit(client); client.Send(packet); break;
                            case 3:
                                {
                                    switch (ButtonID)
                                    {
                                        case 2: Game.ConquerStructures.Arena.QualifyEngine.DoGiveUp(client); break;
                                        case 1: Game.ConquerStructures.Arena.QualifyEngine.DoAccept(client); break;
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    if (client.ArenaStatistic.ArenaPoints <= 1500)
                                        if (client.Entity.Money >= 9000000)
                                        {
                                            client.Entity.Money -= 9000000;
                                            client.ArenaStatistic.ArenaPoints += 1500;
                                            client.Send(client.ArenaStatistic);
                                        }
                                    break;
                                }
                            case 11://Win/Lose Dialog
                                {
                                    switch (ButtonID)
                                    {
                                        case 0: Game.ConquerStructures.Arena.QualifyEngine.DoSignup(client); break;
                                    }
                                    break;
                                }

                        }
                        break;
                    }
                case 2208://Request Arena Winner List
                    {
                        //Code snippet that belongs to Ultimation
                        Game.ConquerStructures.Arena.Statistics.ShowWiners(client);
                        break;
                    }
                case 2209:
                    {
                        client.ArenaStatistic.Send(client);
                        break;
                    }
                case 2211:
                    {
                        ushort Type = BitConverter.ToUInt16(packet, 4);
                        uint Fighter = BitConverter.ToUInt32(packet, 10);
                        if (Type == 0)
                        {
                            if (ServerBase.Kernel.GamePool.ContainsKey(Fighter))
                            {
                                Client.GameState Client = ServerBase.Kernel.GamePool[Fighter];
                                if (Client.QualifierGroup != null)
                                {
                                    if (Client.QualifierGroup.Inside)
                                    {
                                        if (!Client.QualifierGroup.Done)
                                        {
                                            Client.QualifierGroup.BeginWatching(client);
                                        }
                                    }
                                }
                            }
                        }
                        else if (Type == 1)
                        {
                            Game.ConquerStructures.Arena.QualifyEngine.DoLeave(client);
                        }
                        else if (Type == 4)
                        {
                            string name = "";
                            for (int c = 22; c < packet.Length; c++)
                            {
                                if (packet[c] != 0)
                                    name += (char)packet[c];
                                else
                                    break;
                            }
                            Game.ConquerStructures.Arena.QualifyEngine.DoCheer(client, name);
                        }
                        break;
                    }
                #endregion
                #region Movement/Walk (10005)
                case 10005:
                    {
                        if (client.Action != 2)
                            return;
                        GroundMovement groundMovement = new GroundMovement(false);
                        groundMovement.Deserialize(packet);
                        PlayerGroundMovment(groundMovement, client);
                        break;
                    }
                #endregion
                #region Data (10010)
                case 10010:
                    {
                        if (client.Action != 2)
                            return;
                        Data gData = new Data(false);
                        gData.Deserialize(packet);
                        switch (gData.ID)
                        {
                            case Data.SwingPickaxe:
                                client.Mining = true;
                                break;
                            case Data.Revive:

                                Revive(gData, client);
                                // Revive(gData, client);
                                break;
                            case Data.UsePortal:
                                if (client.Entity.MapID == 601)
                                {
                                    client.Entity.Teleport(601, 063, 055);
                                }
                                else
                                    UsePortal(gData, client);
                                break;

                            case Data.ChangePKMode:
                                ChangePKMode(gData, client);
                                break;
                            case Data.ChangeAction:
                                ChangeAction(gData, client);
                                break;
                            case Data.ChangeDirection:
                                ChangeDirection(gData, client);
                                break;
                            case Data.Hotkeys:
                                client.Send(packet);
                                break;
                            case 408://steed soul remoeve
                                {


                                    break;
                                }
                            case Data.ConfirmSpells:
                                if (client.Spells != null)
                                    foreach (Interfaces.ISkill spell in client.Spells.Values)
                                        if (spell.ID != 3060)
                                            spell.Send(client);
                                client.Send(packet);
                                break;
                            case Data.ConfirmProficiencies:
                                if (client.Proficiencies != null)
                                    foreach (Interfaces.ISkill proficiency in client.Proficiencies.Values)
                                        proficiency.Send(client);
                                client.Send(packet);
                                break;
                            case Data.ConfirmGuild:
                                client.Send(packet);
                                break;
                            case Data.ConfirmFriends:
                                #region Friends/Enemy/TradePartners/Apprentices
                                Message msg2 = new Message("Your friend, " + client.Entity.Name + ", has logged on.", System.Drawing.Color.Red, Message.TopLeft);

                                foreach (Game.ConquerStructures.Society.Friend friend in client.Friends.Values)
                                {
                                    if (friend.IsOnline)
                                    {
                                        var pckt = new KnownPersons(true)
                                        {
                                            UID = client.Entity.UID,
                                            Type = KnownPersons.RemovePerson,
                                            Name = client.Entity.Name,
                                            Online = true
                                        };
                                        friend.Client.Send(pckt);
                                        pckt.Type = KnownPersons.AddFriend;
                                        friend.Client.Send(pckt);
                                        friend.Client.Send(msg2);
                                    }
                                    client.Send(new KnownPersons(true)
                                    {
                                        UID = friend.ID,
                                        Type = KnownPersons.AddFriend,
                                        Name = friend.Name,
                                        Online = friend.IsOnline
                                    });
                                    if (friend.Message != "")
                                    {
                                        client.Send(new Message(friend.Message, client.Entity.Name, friend.Name, System.Drawing.Color.Red, Message.Whisper));
                                        Database.KnownPersons.UpdateMessageOnFriend(friend.ID, client.Entity.UID, "");
                                    }
                                }

                                foreach (Game.ConquerStructures.Society.Enemy enemy in client.Enemy.Values)
                                {
                                    client.Send(new KnownPersons(true)
                                    {
                                        UID = enemy.ID,
                                        Type = KnownPersons.AddEnemy,
                                        Name = enemy.Name,
                                        Online = enemy.IsOnline
                                    });
                                }
                                Message msg3 = new Message("Your partner, " + client.Entity.Name + ", has logged in.", System.Drawing.Color.Red, Message.TopLeft);

                                foreach (Game.ConquerStructures.Society.TradePartner partner in client.Partners.Values)
                                {
                                    if (partner.IsOnline)
                                    {
                                        var packet3 = new TradePartner(true)
                                        {
                                            UID = client.Entity.UID,
                                            Type = TradePartner.BreakPartnership,
                                            Name = client.Entity.Name,
                                            HoursLeft = (int)(new TimeSpan(partner.ProbationStartedOn.AddDays(3).Ticks).TotalHours - new TimeSpan(DateTime.Now.Ticks).TotalHours),
                                            Online = true
                                        };
                                        partner.Client.Send(packet3);
                                        packet3.Type = TradePartner.AddPartner;
                                        partner.Client.Send(packet3);
                                        partner.Client.Send(msg3);
                                    }
                                    var packet4 = new TradePartner(true)
                                    {
                                        UID = partner.ID,
                                        Type = TradePartner.AddPartner,
                                        Name = partner.Name,
                                        HoursLeft = (int)(new TimeSpan(partner.ProbationStartedOn.AddDays(3).Ticks).TotalHours - new TimeSpan(DateTime.Now.Ticks).TotalHours),
                                        Online = partner.IsOnline
                                    };
                                    client.Send(packet4);
                                }

                                foreach (Game.ConquerStructures.Society.Apprentice appr in client.Apprentices.Values)
                                {
                                    if (appr.IsOnline)
                                    {
                                        ApprenticeInformation AppInfo = new ApprenticeInformation();
                                        AppInfo.Apprentice_ID = appr.ID;
                                        AppInfo.Apprentice_Level = appr.Client.Entity.Level;
                                        AppInfo.Apprentice_Class = appr.Client.Entity.Class;
                                        AppInfo.Apprentice_PkPoints = appr.Client.Entity.PKPoints;
                                        AppInfo.Apprentice_Experience = appr.Actual_Experience;
                                        AppInfo.Apprentice_Composing = appr.Actual_Plus;
                                        AppInfo.Apprentice_Blessing = appr.Actual_HeavenBlessing;
                                        AppInfo.Apprentice_Name = appr.Name;
                                        AppInfo.Apprentice_Online = true;
                                        AppInfo.Apprentice_Spouse_Name = appr.Client.Entity.Spouse;
                                        AppInfo.Enrole_date = appr.EnroleDate;
                                        AppInfo.Mentor_ID = client.Entity.UID;
                                        AppInfo.Mentor_Mesh = client.Entity.Mesh;
                                        AppInfo.Mentor_Name = client.Entity.Name;
                                        AppInfo.Type = 2;
                                        client.Send(AppInfo);

                                        MentorInformation Information = new MentorInformation(true);
                                        Information.Mentor_Type = 1;
                                        Information.Mentor_ID = client.Entity.UID;
                                        Information.Apprentice_ID = appr.ID;
                                        Information.Enrole_Date = appr.EnroleDate;
                                        Information.Mentor_Level = client.Entity.Level;
                                        Information.Mentor_Class = client.Entity.Class;
                                        Information.Mentor_PkPoints = client.Entity.PKPoints;
                                        Information.Mentor_Mesh = client.Entity.Mesh;
                                        Information.Mentor_Online = true;
                                        Information.Shared_Battle_Power = (uint)(((client.Entity.BattlePower - client.Entity.ExtraBattlePower) - (appr.Client.Entity.BattlePower - appr.Client.Entity.ExtraBattlePower)) / 3.3F);
                                        Information.String_Count = 3;
                                        Information.Mentor_Name = client.Entity.Name;
                                        Information.Apprentice_Name = appr.Name;
                                        Information.Mentor_Spouse_Name = client.Entity.Spouse;
                                        appr.Client.ReviewMentor();
                                        appr.Client.Send(Information);
                                    }
                                    else
                                    {
                                        ApprenticeInformation AppInfo = new ApprenticeInformation();
                                        AppInfo.Apprentice_ID = appr.ID;
                                        AppInfo.Apprentice_Name = appr.Name;
                                        AppInfo.Apprentice_Online = false;
                                        AppInfo.Enrole_date = appr.EnroleDate;
                                        AppInfo.Mentor_ID = client.Entity.UID;
                                        AppInfo.Mentor_Mesh = client.Entity.Mesh;
                                        AppInfo.Mentor_Name = client.Entity.Name;
                                        AppInfo.Type = 2;
                                        client.Send(AppInfo);
                                    }
                                }
                                #endregion
                                client.Send(packet);
                                break;
                            case Data.EndTeleport:
                                break;
                            case Data.GetSurroundings:
                                if (client.Booth != null)
                                {
                                    client.Entity.TransformationID = 0;
                                    client.Booth.Remove();
                                    client.Booth = null;
                                }
                                GetSurroundings(client);
                                client.Send(new MapStatus() { BaseID = client.Map.BaseID, ID = client.Map.ID, Status = Database.MapsTable.MapInformations[client.Map.ID].Status });
                                Game.Weather.CurrentWeatherBase.Send(client);
                                client.Send(gData);
                                break;
                            case Data.SetLocation:
                                SetLocation(gData, client);
                                break;
                            case Data.Jump:
                                PlayerJump(gData, client);
                                break;
                            case Data.UnknownEntity:
                                {
                                    #region UnknownEntity
                                    Client.GameState pClient = null;
                                    if (ServerBase.Kernel.GamePool.TryGetValue(gData.dwParam, out pClient))
                                    {
                                        if (ServerBase.Kernel.GetDistance(pClient.Entity.X, pClient.Entity.Y, client.Entity.X, client.Entity.Y) <= ServerBase.Constants.pScreenDistance && client.Map.ID == pClient.Map.ID)
                                        {
                                            if (pClient.Guild != null)
                                                pClient.Guild.SendName(client);
                                            if (client.Guild != null)
                                                client.Guild.SendName(pClient);
                                            if (pClient.Entity.UID != client.Entity.UID)
                                            {
                                                if (pClient.Map.ID == client.Map.ID)
                                                {
                                                    if (pClient.Map.BaseID == 700)
                                                    {
                                                        if (client.QualifierGroup != null)
                                                        {
                                                            if (pClient.QualifierGroup != null)
                                                            {
                                                                client.Entity.SendSpawn(pClient, false);
                                                                pClient.Entity.SendSpawn(client, false);
                                                            }
                                                            else
                                                            {
                                                                client.Entity.SendSpawn(pClient, false);
                                                                client.Screen.Add(pClient.Entity);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (pClient.QualifierGroup != null)
                                                            {
                                                                pClient.Entity.SendSpawn(client, false);
                                                                pClient.Screen.Add(client.Entity);
                                                            }
                                                            else
                                                            {
                                                                client.Entity.SendSpawn(pClient, false);
                                                                pClient.Entity.SendSpawn(client, false);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        client.Entity.SendSpawn(pClient, false);
                                                        pClient.Entity.SendSpawn(client, false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Game.Entity monster = null;
                                        if (client.Map.Entities.TryGetValue(gData.dwParam, out monster))
                                        {
                                            if (ServerBase.Kernel.GetDistance(monster.X, monster.Y, client.Entity.X, client.Entity.Y) <= ServerBase.Constants.pScreenDistance)
                                            {
                                                monster.SendSpawn(client, false);
                                            }
                                        }
                                        if (client.Map.Companions.TryGetValue(gData.dwParam, out monster))
                                        {
                                            if (ServerBase.Kernel.GetDistance(monster.X, monster.Y, client.Entity.X, client.Entity.Y) <= ServerBase.Constants.pScreenDistance)
                                            {
                                                monster.SendSpawn(client, false);
                                            }
                                        }
                                    }
                                    #endregion
                                    break;
                                }
                            case Data.CompleteLogin:
                                LoginMessages(client);

                            EntityEquipment equips = new EntityEquipment(true);
            equips.ParseHero(client);
            client.Send(equips);
                                break;
                            case Data.ChangeFace:
                                ChangeFace(gData, client);
                                break;
                            case Data.ObserveEquipment:
                            case Data.ObserveEquipment2:
                            case Data.ObserveKnownPerson:
                                ObserveEquipment(gData, client);
                                break;
                            case Data.ViewEnemyInfo:
                                {
                                    if (client.Enemy.ContainsKey(gData.dwParam))
                                    {
                                        if (client.Enemy[gData.dwParam].IsOnline)
                                        {
                                            KnownPersonInfo info = new KnownPersonInfo(true);
                                            info.Fill(client.Enemy[gData.dwParam], true, false);
                                            if (client.Enemy[gData.dwParam].Client.Guild != null)
                                                client.Enemy[gData.dwParam].Client.Guild.SendName(client);
                                            client.Send(info);
                                        }
                                    }
                                    break;
                                }
                            case Data.ViewFriendInfo:
                                {
                                    if (client.Friends.ContainsKey(gData.dwParam))
                                    {
                                        if (client.Friends[gData.dwParam].IsOnline)
                                        {
                                            KnownPersonInfo info = new KnownPersonInfo(true);
                                            info.Fill(client.Friends[gData.dwParam], false, false);
                                            if (client.Friends[gData.dwParam].Client.Guild != null)
                                                client.Friends[gData.dwParam].Client.Guild.SendName(client);
                                            client.Send(info);
                                        }
                                    }
                                    break;
                                }
                            case Data.ViewPartnerInfo:
                                {
                                    if (client.Partners.ContainsKey(gData.dwParam))
                                    {
                                        if (client.Partners[gData.dwParam].IsOnline)
                                        {
                                            TradePartnerInfo info = new TradePartnerInfo(true);
                                            info.Fill(client.Partners[gData.dwParam]);
                                            if (client.Partners[gData.dwParam].Client.Guild != null)
                                                client.Partners[gData.dwParam].Client.Guild.SendName(client);
                                            client.Send(info);
                                        }
                                    }
                                    break;
                                }
                            case Data.EndFly:
                                client.Entity.RemoveFlag(Update.Flags.Fly);
                                break;
                            case Data.EndTransformation:
                                client.Entity.Untransform();
                                break;
                            case Data.XPListEnd:
                            case Data.Die:
                                break;
                            case Data.OwnBooth:
                                {

                                    //client.Entity.TransformationTime = 3600;
                                    if (client.WarehouseOpen == true)
                                    {
                                        client.Send(new Message("you cant booth when you open warehose", Color.AntiqueWhite, 2005));


                                        return;
                                    }
                                    else
                                    {
                                        client.Booth = new Conquer_Online_Server.Game.ConquerStructures.Booth(client, gData);
                                        client.Send(new Data(true) { ID = Data.ChangeAction, UID = client.Entity.UID, dwParam = 0 });

                                    }
                                    break;
                                }
                            case Data.Away:
                                {
                                    if (client.Entity.Away == 0)
                                        client.Entity.Away = 1;
                                    else
                                        client.Entity.Away = 0;
                                    client.SendScreenSpawn(client.Entity, false);
                                    break;
                                }
                            case Data.DeleteCharacter:
                                {
                                    if ((client.WarehousePW == null || client.WarehousePW == "" || client.WarehousePW == "0" && gData.dwParam == 0) || (client.WarehousePW == gData.dwParam.ToString()))
                                    {
                                        client.Account.EntityID = 0;
                                        client.Account.Save();
                                        client.Disconnect();
                                    }
                                    break;
                                }
                            case Data.TeamSearchForMember:
                                {
                                    if (client.Team != null)
                                    {
                                        Client.GameState Client = null;
                                        if (!client.Team.IsTeammate(gData.UID))
                                            return;
                                        if (Kernel.GamePool.TryGetValue(gData.UID, out Client))
                                        {
                                            gData.wParam1 = Client.Entity.X;
                                            gData.wParam2 = Client.Entity.Y;
                                            gData.Send(client);
                                        }
                                    }
                                    break;
                                }
                            default:
                                if (client.Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.ProjectManager)
                                    client.Send(new Message("Unknown generaldata id: " + gData.ID, System.Drawing.Color.CadetBlue, Message.Talk));
                                break;
                        }
                        break;
                    }
                #endregion
                #region Status 1040
                case 1040:
                    {
                        Status ShowStatistics2 = new Status(client);
                        ShowStatistics2.Deserialize(packet);
                        ShowStatistics2.Send(client);
                        //uint UID = BitConverter.ToUInt32(packet, 4);
                        //Client.GameState Client;
                        //if (ServerBase.Kernel.GamePool.TryGetValue(UID, out Client))
                        //{
                        //    client.Send(WindowStats(Client));
                        //}
                        break;
                    }
                #endregion
                #region Flowers
                case 1150:
                    {
                        SendFlowers(client, packet);

                        break;
                    }
                case 1151:
                    {
                        AddFlowers(client, packet);

                        break;
                    }


                #endregion
                #region Clans
                case 1312:
                    {
                        switch (packet[4])
                        {
                            case 21://transfer
                                {
                                    if (client.Entity.Myclan != null)
                                    {
                                        //PrintPacket(packet);
                                        uint lider = 0;
                                        string name_receive = System.Text.Encoding.ASCII.GetString(packet, 18, packet[17]);



                                        var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                        varr.MoveNext();
                                        int COunt = ServerBase.Kernel.GamePool.Count;
                                        for (uint x = 0;
                                            x < COunt;
                                            x++)
                                        {
                                            if (x >= COunt) break;

                                            Client.GameState clien = (varr.Current as Client.GameState);

                                            if (clien.Entity.Name == name_receive)
                                                lider = clien.Entity.UID;

                                            varr.MoveNext();

                                        }

                                        if (lider == client.Entity.UID) return;
                                        Client.GameState aClient = null;
                                        if (Conquer_Online_Server.ServerBase.Kernel.GamePool.TryGetValue(lider, out aClient))
                                        {

                                            if (Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(client.Entity.Myclan.ClanId))
                                            {
                                                if (Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].Members.ContainsKey(aClient.Entity.UID))
                                                {
                                                    Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].ClanLider = aClient.Entity.Name;
                                                    aClient.Entity.ClanRank = 100;
                                                    aClient.Entity.Myclan.Members[aClient.Entity.UID].Rank = 100;
                                                    if (aClient.Entity.Myclan.Members.ContainsKey(client.Entity.UID))
                                                        aClient.Entity.Myclan.Members[client.Entity.UID].Rank = 0;
                                                    client.Entity.ClanRank = 0;
                                                    Database.Clans.SaveClan(aClient.Entity.Myclan);
                                                    Database.Clans.JoinClan(client);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].ClanLider = name_receive;
                                            client.Entity.ClanRank = 0;
                                            Database.Clans.JoinClan(client);
                                            Database.Clans.SaveClan(client.Entity.Myclan);
                                            Database.Clans.TransferClan(name_receive);
                                        }
                                    }
                                    break;
                                }
                            /* case 14://add enemy
                                 {
                                     break;
                                 }*/
                            case 9://recruit member
                                {
                                    break;
                                }
                            case 23://client exit
                                {
                                    if (client.Entity.Myclan != null)
                                    {
                                        if (Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(client.Entity.Myclan.ClanId))
                                        {
                                            if (Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].Members.ContainsKey(client.Entity.UID))
                                            {
                                                Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].Members.Remove(client.Entity.UID);
                                            }
                                        }
                                        client.Entity.ClanId = 0;
                                        client.Entity.ClanName = "";
                                        client.Entity.Myclan = null;

                                        Database.Clans.KickClan(client.Entity.Name);
                                        client.Send(packet);
                                    }
                                    break;
                                }
                            case 25://buleitn
                                {
                                    if (client.Entity.Myclan == null) return;
                                    string buletin = System.Text.Encoding.ASCII.GetString(packet, 18, packet[17]);
                                    if (Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(client.Entity.Myclan.ClanId))
                                        Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].ClanBuletion = buletin;
                                    Database.Clans.SaveClan(Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId]);
                                    client.Send(packet);
                                    break;
                                }
                            case 22://give kick
                                {
                                    if (client.Entity.Myclan != null)
                                    {
                                        if (client.Entity.ClanRank == 100)
                                        {
                                            string name = System.Text.Encoding.ASCII.GetString(packet, 18, packet[17]);
                                            uint Key = 0;


                                            foreach (Game.ClanMembers mem in client.Entity.Myclan.Members.Values)
                                            {
                                                if (mem.Name == name)
                                                    Key = mem.UID;
                                            }
                                            if (Key != 0)
                                            {

                                                if (Conquer_Online_Server.ServerBase.Kernel.GamePool.ContainsKey(Key))
                                                {


                                                    if (Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(client.Entity.Myclan.ClanId))
                                                    {
                                                        if (Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].Members.ContainsKey(Key))
                                                        {
                                                            Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].Members.Remove(Key);
                                                            Network.GamePackets.ClanMembers clan = new Network.GamePackets.ClanMembers(client);
                                                            client.Send(clan.ToArray());
                                                            Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 22);
                                                            ServerBase.Kernel.GamePool[Key].Send(cl.ToArray());
                                                            ServerBase.Kernel.GamePool[Key].Entity.ClanName = "";
                                                            ServerBase.Kernel.GamePool[Key].Entity.ClanId = 0;
                                                            ServerBase.Kernel.GamePool[Key].Entity.Myclan = null;
                                                            ServerBase.Kernel.GamePool[Key].Screen.FullWipe();
                                                            ServerBase.Kernel.GamePool[Key].Screen.Reload(null);
                                                        }
                                                    }
                                                }
                                            }
                                            Database.Clans.KickClan(name);
                                        }
                                    }

                                    break;
                                }

                            case 26://donation
                                {
                                    uint money = BitConverter.ToUInt32(packet, 8);
                                    if (client.Entity.Money >= money && client.Entity.Myclan != null)
                                    {
                                        client.Entity.Myclan.Members[client.Entity.UID].Donation += money;
                                        client.Entity.Money -= money;
                                        if (Conquer_Online_Server.ServerBase.Kernel.ServerClans.ContainsKey(client.Entity.Myclan.ClanId))
                                            Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].ClanDonation += money;
                                        Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 1);
                                        client.Send(cl.ToArray());
                                        Database.Clans.SaveClientDonation(client);
                                        Database.Clans.SaveClan(Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId]);
                                    }
                                    break;
                                }
                            case 11://add player
                                {

                                    uint lider = BitConverter.ToUInt32(packet, 8);
                                    if (Conquer_Online_Server.ServerBase.Kernel.GamePool.ContainsKey(lider))
                                    {
                                        packet[4] = 11;
                                        Network.Writer.WriteUInt32(client.Entity.UID, 8, packet);
                                        packet[16] = 1;
                                        packet[17] = (byte)client.Entity.Name.Length;
                                        for (int i = 0; i < client.Entity.Name.Length; i++)
                                        {
                                            try
                                            {
                                                packet[18 + i] = Convert.ToByte(client.Entity.Name[i]);

                                            }
                                            catch { }
                                        }
                                        if (Conquer_Online_Server.ServerBase.Kernel.GamePool[lider].Entity.Myclan != null)
                                            Conquer_Online_Server.ServerBase.Kernel.GamePool[lider].Send(packet);
                                    }
                                    break;
                                }
                            case 12:
                                {
                                    if (packet[16] == 1)
                                    {
                                        if (client.Entity.Myclan != null)
                                            if (client.Entity.Myclan.Members[client.Entity.UID].Rank == 100)
                                            {
                                                //if (client.Entity.Myclan != null)
                                                {
                                                    if (client.Entity.Myclan.Members.Count < 7)
                                                    {

                                                        uint memeber = BitConverter.ToUInt32(packet, 8);
                                                        if (ServerBase.Kernel.GamePool.ContainsKey(memeber))
                                                        {
                                                            Game.ClanMembers member = new Conquer_Online_Server.Game.ClanMembers();
                                                            member.UID = ServerBase.Kernel.GamePool[memeber].Entity.UID;
                                                            member.Donation = 0;
                                                            member.Rank = 10;
                                                            member.Class = ServerBase.Kernel.GamePool[memeber].Entity.Class;
                                                            member.Level = ServerBase.Kernel.GamePool[memeber].Entity.Level;
                                                            member.Name = ServerBase.Kernel.GamePool[memeber].Entity.Name;
                                                            if (!client.Entity.Myclan.Members.ContainsKey(memeber))
                                                                client.Entity.Myclan.Members.Add(member.UID, member);

                                                            ServerBase.Kernel.GamePool[memeber].Entity.ClanRank = 0;
                                                            ServerBase.Kernel.GamePool[memeber].Entity.ClanId = client.Entity.ClanId;
                                                            ServerBase.Kernel.GamePool[memeber].Entity.Myclan = client.Entity.Myclan;
                                                            ServerBase.Kernel.GamePool[memeber].Entity.ClanName = client.Entity.ClanName;
                                                            Database.Clans.JoinClan(ServerBase.Kernel.GamePool[memeber]);
                                                            ServerBase.Kernel.GamePool[memeber].Screen.FullWipe();
                                                            ServerBase.Kernel.GamePool[memeber].Screen.Reload(null);
                                                        }
                                                    }
                                                }
                                            }
                                    }
                                    break;
                                }
                            case 4://memebers
                                {
                                    ClanMembers clan = new ClanMembers(client);
                                    client.Send(clan.ToArray());
                                    break;
                                }
                            case 0x1d:
                                {
                                    if (client.Entity.Myclan != null)
                                    {
                                        Network.Writer.WriteUInt32(client.Entity.Myclan.ClanId, 8, packet);
                                        Network.Writer.WriteByte(1, 16, packet);
                                        Network.Writer.WriteByte(0x0d, 17, packet);
                                        Network.Writer.WriteString("0 0 0 0 0 0 0", 18, packet);
                                        client.Send(packet);
                                        Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 1);
                                        client.Send(cl.ToArray());
                                        client.Send(packet);

                                        if (client.Entity.Myclan.ClanBuletion != "")
                                            client.Send(cl.UpgradeBuletin(client.Entity.Myclan.ClanBuletion));
                                    }
                                    else
                                    {
                                        client.Send(packet);
                                        packet[4] = 23;
                                    }


                                    break;
                                }
                            case 0x18:
                                {
                                    client.Send(packet);
                                    break;
                                }
                            case (byte)Game.Clan_Typ.AddAlly:
                                {
                                    Game.Clans clan = client.Entity.Myclan;
                                    if (clan != null)
                                    {
                                        Client.GameState target;
                                        UInt32 Identifier = BitConverter.ToUInt32(packet, 8);

                                        if (client.Entity.ClanRank == 100)
                                        {
                                            if (clan.Allies.Count >= 5)
                                            { client.Send(new Message("The Ammount of Allies You can Have has exceeded", System.Drawing.Color.Red, Message.TopLeft)); break; }

                                            if (ServerBase.Kernel.GamePool.TryGetValue(Identifier, out target))
                                            {
                                                Game.Clans tclan = target.Entity.Myclan;
                                                if (tclan != null)
                                                {
                                                    if (target.Entity.ClanRank == 100)
                                                    {
                                                        if (tclan.Allies.Count >= 5)
                                                        { client.Send(new Message("The Ammount of Allies the Target Clan can Have has exceeded", System.Drawing.Color.Red, Message.TopLeft)); break; }

                                                        if (!clan.Allies.ContainsKey(tclan.ClanId))
                                                        {
                                                            if (!clan.Enemies.ContainsKey(tclan.ClanId))
                                                            {
                                                                String clanName = client.Entity.ClanName;

                                                                Clan2 clanp = new Clan2();

                                                                clanp.Deserialize(packet);

                                                                clanp.Offset16 = 2;
                                                                clanp.Identifier = client.Entity.UID;

                                                                Writer.WriteByte((Byte)clanName.Length, 17, clanp.ToArray());
                                                                Writer.WriteString(clanName, 18, clanp.ToArray());

                                                                tclan.AllyRequest = clan.ClanId;

                                                                target.Send(clanp);
                                                            }
                                                            else client.Send(new Message("That clan is Your Enemy.", System.Drawing.Color.Red, Message.TopLeft));
                                                        }
                                                    }
                                                    else client.Send(new Message("This target is not the clan leader.", System.Drawing.Color.Red, Message.TopLeft));
                                                }
                                            }
                                            else client.Send(new Message("Can not find target.", System.Drawing.Color.Red, Message.TopLeft));
                                        }
                                        else client.Send(new Message("You are not the clan leader.", System.Drawing.Color.Red, Message.TopLeft));
                                    }
                                    break;
                                }
                            case (byte)Game.Clan_Typ.AcceptAlliance:
                                {
                                    Game.Clans clan = client.Entity.Myclan;
                                    if (clan != null)
                                    {
                                        if (client.Entity.ClanRank == 100)
                                        {
                                            Game.Clans tclan;
                                            if (ServerBase.Kernel.ServerClans.TryGetValue(clan.AllyRequest, out tclan))
                                            {
                                                if (tclan != null)
                                                {
                                                    if (!tclan.Enemies.ContainsKey(clan.ClanId))
                                                    {
                                                        if (!clan.Enemies.ContainsKey(tclan.ClanId))
                                                        {
                                                            if (!clan.Allies.ContainsKey(tclan.ClanId))
                                                                clan.Allies.Add(tclan.ClanId, tclan);

                                                            tclan.Allies.Add(clan.ClanId, clan);

                                                            clan.SendMessage(new ClanRelations(clan, ClanRelations.RelationTypes.Allies));
                                                            tclan.SendMessage(new ClanRelations(tclan, ClanRelations.RelationTypes.Allies));

                                                            clan.SendMessage(new Message(String.Format("Our Clan has Allianced with {0}", tclan.ClanName), System.Drawing.Color.Red, Message.Clan));
                                                            tclan.SendMessage(new Message(String.Format("Our Clan has Allianced with {0}", clan.ClanName), System.Drawing.Color.Red, Message.Clan));

                                                            clan.AddRelation(tclan.ClanId, ClanRelations.RelationTypes.Allies);
                                                            tclan.AddRelation(clan.ClanId, ClanRelations.RelationTypes.Allies);

                                                            clan.AllyRequest = tclan.AllyRequest = 0;
                                                        }
                                                        else client.Send(new Message("This Clan is Your Enemy.", System.Drawing.Color.Red, Message.TopLeft));
                                                    }
                                                    client.Send(new Message("This Clan Has Enemied You!.", System.Drawing.Color.Red, Message.TopLeft));
                                                }
                                            }
                                            else client.Send(new Message("Can not find target", System.Drawing.Color.Red, Message.TopLeft));
                                        }
                                        else client.Send(new Message("You are not the clan leader.", System.Drawing.Color.Red, Message.TopLeft));
                                    }
                                    break;
                                }
                            case (byte)Game.Clan_Typ.DeleteEnemy:
                                {
                                    Game.Clans clan = client.Entity.Myclan;
                                    if (clan != null)
                                    {
                                        if (client.Entity.ClanRank == 100)
                                        {
                                            Clan2 clanp = new Clan2();
                                            clanp.Deserialize(packet);

                                            String EnemyTarget = clanp.Offset18String;
                                            UInt32 ClanId = clanp.GetClanId(EnemyTarget);

                                            Game.Clans tclan;
                                            if (ServerBase.Kernel.ServerClans.TryGetValue(ClanId, out tclan))
                                            {
                                                clan.Enemies.Remove(ClanId);

                                                clan.DeleteRelation(ClanId, ClanRelations.RelationTypes.Enemies);

                                                clan.SendMessage(new ClanRelations(clan, ClanRelations.RelationTypes.Enemies));

                                                clan.SendMessage(new Message(String.Format("We are no longer Enemies With {0}", clan.ClanId), System.Drawing.Color.Red, Message.Clan));

                                                client.Send(clanp);
                                            }
                                        }
                                        else client.Send(new Message("You are not authorized to continue with this action", System.Drawing.Color.Red, Message.TopLeft));
                                    }
                                    break;
                                }
                            case (byte)Game.Clan_Typ.DeleteAlly:
                                {
                                    Game.Clans clan = client.Entity.Myclan;
                                    if (clan != null)
                                    {
                                        if (client.Entity.ClanRank == 100)
                                        {
                                            Clan2 clanp = new Clan2();
                                            clanp.Deserialize(packet);

                                            String AlliedTarget = clanp.Offset18String;
                                            UInt32 ClanId = clanp.GetClanId(AlliedTarget);

                                            Game.Clans tclan;
                                            if (clan.Allies.TryGetValue(ClanId, out tclan))
                                            {
                                                clan.Allies.Remove(ClanId);
                                                tclan.Allies.Remove(clan.ClanId);

                                                clan.DeleteRelation(ClanId, ClanRelations.RelationTypes.Allies);
                                                tclan.DeleteRelation(clan.ClanId, ClanRelations.RelationTypes.Allies);

                                                clan.SendMessage(new ClanRelations(clan, ClanRelations.RelationTypes.Allies));
                                                tclan.SendMessage(new ClanRelations(tclan, ClanRelations.RelationTypes.Allies));

                                                clan.SendMessage(new Message(String.Format("We are no longer allied with {0}", tclan.ClanName), System.Drawing.Color.Red, Message.Clan));
                                                tclan.SendMessage(new Message(String.Format("We are no longer allied with {0}", clan.ClanName), System.Drawing.Color.Red, Message.Clan));

                                                client.Send(clanp);
                                            }
                                        }
                                        else client.Send(new Message("You are not authorized to continue with this action", System.Drawing.Color.Red, Message.TopLeft));
                                    }
                                    break;
                                }
                            case (byte)Game.Clan_Typ.AddEnemy:
                                {
                                    Game.Clans clan = client.Entity.Myclan;
                                    if (clan != null)
                                    {
                                        if (client.Entity.ClanRank == 100)
                                        {
                                            String Enemy = System.Text.Encoding.ASCII.GetString(packet, 18, packet[17]).Trim(new Char[] { '\0' });
                                            UInt32 ClanId = 0;
                                            var AllCland = ServerBase.Kernel.ServerClans.Values.ToArray();
                                            foreach (Game.Clans c_clan in AllCland)
                                            {
                                                if (Enemy == c_clan.ClanName)
                                                {
                                                    ClanId = c_clan.ClanId;
                                                    break;
                                                }
                                            }
                                            if (ClanId == 0) break;
                                            if (!clan.Enemies.ContainsKey(ClanId))
                                            {
                                                if (!clan.Allies.ContainsKey(ClanId))
                                                {
                                                    if (clan.Enemies.Count >= 5)
                                                    { client.Send(new Message("The Ammount of Enemies You can Have has exceeded", System.Drawing.Color.Red, Message.TopLeft)); break; }

                                                    Game.Clans tclan;
                                                    if (ServerBase.Kernel.ServerClans.TryGetValue(ClanId, out tclan))
                                                    {
                                                        if (!clan.Enemies.ContainsKey(tclan.ClanId))
                                                            clan.Enemies.Add(tclan.ClanId, tclan);

                                                        clan.AddRelation(ClanId, ClanRelations.RelationTypes.Enemies);

                                                        clan.SendMessage(new ClanRelations(clan, ClanRelations.RelationTypes.Enemies));

                                                        clan.SendMessage(new Message(String.Format("We Have Enemied the clan {0}", tclan.ClanName), System.Drawing.Color.Red, Message.Clan));
                                                        tclan.SendMessage(new Message(String.Format("The Clan {0} Has Made us their Enemy!", clan.ClanName), System.Drawing.Color.Red, Message.Clan));
                                                    }
                                                }
                                                else client.Send(new Message("This clan is one of your alliance, What has gone wrong?", System.Drawing.Color.Red, Message.TopLeft));
                                            }
                                            else client.Send(new Message("This clan is Already One of Your Enemies", System.Drawing.Color.Red, Message.TopLeft));
                                        }
                                    }
                                    break;
                                }
                            default:
                                Console.WriteLine("Clan Type " + packet[4]);
                                break;

                        }
                        break;
                        /* switch (packet[4])
                         {
                             case 23://client exit
                                 {
                                     if (client.Entity.Myclan != null)
                                     {
                                         foreach (var clien in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                                         {
                                             if (clien.Entity.Myclan != null)
                                             {
                                                 if (clien.Entity.Myclan.ClanId == client.Entity.Myclan.ClanId)
                                                 {
                                                     if (clien.Entity.Myclan.Members.ContainsKey(client.Entity.UID))
                                                         clien.Entity.Myclan.Members.Remove(client.Entity.UID);
                                                 }
                                             }
                                         }
                                         client.Entity.ClanName = "";
                                         client.Entity.Myclan = null;
                                         Database.Clans.KickClan(client.Entity.Name);
                                         client.Send(packet);
                                     }
                                     break;
                                 }
                             case 14://enemy
                                 {
                                     string Enemy = System.Text.Encoding.ASCII.GetString(packet, 18, packet[17]);
                                     client.Send(packet);

                                     Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 14);
                                     client.Send(cl.SendAlies(Enemy, "Lider"));
                                     break;
                                 }
                             case 25://buleitn
                                 {
                                     if (client.Entity.Myclan == null) return;
                                     string buletin = System.Text.Encoding.ASCII.GetString(packet, 18, packet[17]);
                                     foreach (var clan in Conquer_Online_Server.ServerBase.Kernel.ServerClans.Values)
                                     {
                                         if (clan.ClanId == client.Entity.Myclan.ClanId)
                                             clan.ClanBuletion = buletin;
                                     }
                                     foreach (var member in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                                     {
                                         if (member.Entity.Myclan != null)
                                         {
                                             if (member.Entity.UID != client.Entity.UID)
                                                 if (member.Entity.Myclan.ClanId == client.Entity.Myclan.ClanId)
                                                 {
                                                     member.Entity.Myclan.ClanBuletion = buletin;
                                                 }
                                         }
                                     }
                                     Database.Clans.SaveClan(Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId]);
                                     client.Send(packet);
                                     client.Entity.Myclan.ClanBuletion = buletin;
                                     break;
                                 }
                             case 22://give kick
                                 {
                                     if (client.Entity.Myclan != null)
                                     {
                                         if (client.Entity.ClanRank == 100)
                                         {
                                             string name = System.Text.Encoding.ASCII.GetString(packet, 18, packet[17]);
                                             uint Key = 0;


                                             foreach (Game.ClanMembers mem in client.Entity.Myclan.Members.Values)
                                             {
                                                 if (mem.Name == name)
                                                     Key = mem.UID;
                                             }
                                             if (Key != 0)
                                             {

                                                 if (Conquer_Online_Server.ServerBase.Kernel.GamePool.ContainsKey(Key))
                                                 {
                                                     foreach (var clien in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                                                     {
                                                         if (clien.Entity.UID == client.Entity.UID) continue;
                                                         if (clien.Entity.Myclan != null)
                                                         {
                                                             if (clien.Entity.Myclan.ClanId == client.Entity.Myclan.ClanId)
                                                             {
                                                                 if (clien.Entity.Myclan.Members.ContainsKey(Key))
                                                                 {
                                                                     clien.Entity.Myclan.Members.Remove(Key);
                                                                     Network.GamePackets.ClanMembers clan = new Network.GamePackets.ClanMembers(client);
                                                                     client.Send(clan.ToArray());

                                                                     clien.Entity.ClanName = "";
                                                                     clien.Entity.Myclan = null;
                                                                 }
                                                             }
                                                         }
                                                     }
                                                 }
                                             }
                                             Database.Clans.KickClan(name);
                                         }
                                     }
                                     break;
                                 }
                             case 26:
                                 {
                                     uint money = BitConverter.ToUInt32(packet, 8);
                                     if (client.Entity.Money >= money && client.Entity.Myclan != null)
                                     {
                                         client.Entity.Myclan.Members[client.Entity.UID].Donation += money;
                                         client.Entity.Money -= money;
                                         Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].ClanDonation += money;
                                         foreach (var clien in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                                         {
                                             if (clien.Entity.Myclan != null)
                                             {
                                                 if (clien.Entity.UID != client.Entity.UID)
                                                 {
                                                     if (clien.Entity.Myclan.ClanId == client.Entity.Myclan.ClanId)
                                                     {

                                                         clien.Entity.Myclan.ClanDonation = Conquer_Online_Server.ServerBase.Kernel.ServerClans[clien.Entity.Myclan.ClanId].ClanDonation;
                                                     }
                                                 }
                                             }
                                         }
                                         client.Entity.Myclan.ClanDonation = Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId].ClanDonation;
                                         Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 1);
                                         client.Send(cl.ToArray());
                                         Database.Clans.SaveClientDonation(client);
                                         Database.Clans.SaveClan(Conquer_Online_Server.ServerBase.Kernel.ServerClans[client.Entity.Myclan.ClanId]);
                                     }
                                     break;
                                 }
                             case 11://add player
                                 {
                                
                                     uint lider = BitConverter.ToUInt32(packet, 8);
                                     if (Conquer_Online_Server.ServerBase.Kernel.GamePool.ContainsKey(lider))
                                     {
                                         packet[4] = 11;
                                         Network.Writer.WriteUInt32(client.Entity.UID, 8, packet);

                                         packet[16] = 1;
                                         packet[17] = (byte)client.Entity.Name.Length;
                                         for (int i = 0; i < client.Entity.Name.Length; i++)
                                         {
                                             try
                                             {
                                                 packet[18 + i] = Convert.ToByte(client.Entity.Name[i]);

                                             }
                                             catch { }
                                         }
                                         Conquer_Online_Server.ServerBase.Kernel.GamePool[lider].Send(packet);
                                     }
                                     break;
                                 }
                             case 12:
                                 {
                                     if (packet[16] == 1)
                                     {
                                         if (client.Entity.Myclan.Members[client.Entity.UID].Rank == 100)
                                         {
                                             if (client.Entity.Myclan != null)
                                             {
                                                 if (client.Entity.Myclan.Members.Count < 7)
                                                 {
                                                   
                                                     uint memeber = BitConverter.ToUInt32(packet, 8);
                                                     if (ServerBase.Kernel.GamePool.ContainsKey(memeber))
                                                     {
                                                         Game.ClanMembers member = new Conquer_Online_Server.Game.ClanMembers();
                                                         member.UID = ServerBase.Kernel.GamePool[memeber].Entity.UID;
                                                         member.Donation = 0;
                                                         member.Rank = 10;
                                                         member.Class = ServerBase.Kernel.GamePool[memeber].Entity.Class;
                                                         member.Level = ServerBase.Kernel.GamePool[memeber].Entity.Level;
                                                         member.Name = ServerBase.Kernel.GamePool[memeber].Entity.Name;
                                                         if (!client.Entity.Myclan.Members.ContainsKey(memeber))
                                                             client.Entity.Myclan.Members.Add(member.UID, member);

                                                         foreach (var clien in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                                                         {
                                                             if (clien.Entity.Myclan != null)
                                                             {
                                                                 if (clien.Entity.Myclan.ClanId == client.Entity.Myclan.ClanId)
                                                                 {
                                                                     if (!clien.Entity.Myclan.Members.ContainsKey(memeber))
                                                                         clien.Entity.Myclan.Members.Add(member.UID, member);
                                                                 }
                                                             }
                                                         }
                                                       ServerBase.Kernel.GamePool[memeber].Entity.ClanRank = 0;
                                                         ServerBase.Kernel.GamePool[memeber].Entity.Myclan = client.Entity.Myclan;
                                                         ServerBase.Kernel.GamePool[memeber].Entity.ClanName = client.Entity.ClanName;
                                                         Database.Clans.JoinClan(ServerBase.Kernel.GamePool[memeber]);
                                                        ServerBase.Kernel.GamePool[memeber].Entity.Teleport(ServerBase.Kernel.GamePool[memeber].Entity.MapID
                                                             , ServerBase.Kernel.GamePool[memeber].Entity.X
                                                             , ServerBase.Kernel.GamePool[memeber].Entity.Y);
                                                     }
                                                 }
                                             }
                                      } 
                                     }
                                     break;
                                 }
                             case 4://memebers
                                 {
                                     ClanMembers clan = new ClanMembers(client);
                                     client.Send(clan.ToArray());
                                     break;
                                 }
                             case 0x1d:
                                 {
                                     if (client.Entity.Myclan != null)
                                     {
                                         Network.Writer.WriteUInt32(client.Entity.Myclan.ClanId, 8, packet);
                                         Network.Writer.WriteByte(1, 16, packet);
                                         Network.Writer.WriteByte(0x0d, 17, packet);
                                         Network.Writer.WriteString("0 0 0 0 0 0 0", 18, packet);
                                         client.Send(packet);
                                         Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 1);
                                         client.Send(cl.ToArray());
                                         client.Send(packet);

                                         if (client.Entity.Myclan.ClanBuletion != "")
                                             client.Send(cl.UpgradeBuletin(client.Entity.Myclan.ClanBuletion));
                                     }
                                     else
                                     {
                                         client.Send(packet);
                                         packet[4] = 23;
                                     }

                                   
                                     break;
                                 }
                             case 0x18:
                                 {
                                     client.Send(packet);
                                     break;
                                 }
                             default:
                                 Console.WriteLine("Clan Type " + packet[4]);
                                 break;
                         }
                         break;*/
                    }
                #endregion
                #region Reincarnation (1066)
                case 1066:
                    {
                        if (client.Entity.Reborn != 2) return;
                        byte NewClass = packet[4];
                        ushort NewBody = packet[8];
                        if (client.Entity.Body.ToString().EndsWith("1") || client.Entity.Body.ToString().EndsWith("2"))
                            NewBody += 2000;
                        else NewBody += 1000;

                        if (client.Inventory.Contains(711083, 1))
                        {
                            client.Entity.Body = NewBody;
                            new PacketHandler.Reincarnation(client, NewClass);
                            //new Game.Features.Reincarnation.Reincarnate(client.Entity, NewClass);
                            client.Inventory.Remove(711083, 1);
                        }
                        break;
                    }
                #endregion
                #region SubClass (2320)
                case 2320:
                    {
                        byte[] Packet = null;
                        switch (packet[4])
                        {
                            #region [Restore/Switch]
                            case 0:
                                byte To = packet[6];
                                Packet = new byte[0];
                                client.Send(packet);

                                if (To > 0)//Switch
                                {
                                    #region [Effects-Addition]
                                    switch ((Conquer_Online_Server.Game.ClassID)To)
                                    {
                                        case Conquer_Online_Server.Game.ClassID.Apothecary:
                                            {
                                                //

                                                client.Entity.Statistics.Detoxication = (ushort)(client.Entity.SubClasses.Classes[(byte)Conquer_Online_Server.Game.ClassID.Apothecary].Phase * 8);
                                                //client.Entity.Statistics.Detoxication += (client.Entity.SubClasses.Classes[To].Level);
                                                break;
                                            }
                                    }
                                 
                                    #endregion
                                    /*switch ((Game.ClassID)To)
                                    {
                                        case Game.ClassID.Wrangler:
                                            {
                                                switch (client.Entity.SubClasses.Classes[To].Level)
                                                {
                                                    case 9:
                                                        {
                                                            //client.Entity.Hitpoints += 1200;
                                                            //client.Entity.MaxHitpoints += 1200;
                                                            break;
                                                        }
                                                }
                                                break;
                                            }
                                    }*/
                                    Packet = new byte[0];
                                    Packet = new SubClassShowFull(true) { ID = 1, Class = To, Level = client.Entity.SubClasses.Classes[To].Phase }.ToArray();//client.Entity.SubClasses.Classes[To].Phase
                                    client.Send(Packet);
                                    //Packet = new SubClass(client.Entity).ToArray();
                                    //client.Send(Packet);
                                    client.Entity.SubClass = To;
                                    client.Entity.SubClassLevel = client.Entity.SubClasses.Classes[To].Level;
                                    client.Entity.SubClasses.Active = To;
                                }
                                else//Restore
                                {
                                    client.Entity.SubClass = 0;
                                    client.Entity.SubClassLevel = 0;
                                    client.Entity.SubClasses.Active = 0;
                                    Packet = new SubClassShowFull(true) { ID = 1 }.ToArray();
                                    client.Send(Packet);
                                }
                                client.SendScreen(client.Entity.SpawnPacket, false);
                                break;
                            #endregion
                            #region [Upgrade]
                            case 2:
                                {
                                    byte Class = packet[6];
                                    ushort Required = 0;
                                    Game.SubClass Sc = client.Entity.SubClasses.Classes[Class];
                                    #region [Set Required]
                                    switch ((Game.ClassID)Sc.ID)
                                    {
                                        case Game.ClassID.MartialArtist:
                                            switch (Sc.Level)
                                            {
                                                case 1: Required = 300; break;
                                                case 2: Required = 900; break;
                                                case 3: Required = 1800; break;
                                                case 4: Required = 2700; break;
                                                case 5: Required = 3600; break;
                                                case 6: Required = 5100; break;
                                                case 7: Required = 6900; break;
                                                case 8: Required = 8700; break;
                                                case 9: Required = ushort.MaxValue; break;
                                            }
                                            break;
                                        case Game.ClassID.Warlock:
                                            switch (Sc.Level)
                                            {
                                                case 1: Required = 300; break;
                                                case 2: Required = 900; break;
                                                case 3: Required = 1800; break;
                                                case 4: Required = 2700; break;
                                                case 5: Required = 3600; break;
                                                case 6: Required = 5100; break;
                                                case 7: Required = 6900; break;
                                                case 8: Required = 8700; break;
                                                case 9: Required = ushort.MaxValue; break;
                                            }
                                            break;
                                        case Game.ClassID.ChiMaster:
                                            switch (Sc.Level)
                                            {
                                                case 1: Required = 600; break;
                                                case 2: Required = 1800; break;
                                                case 3: Required = 3600; break;
                                                case 4: Required = 5400; break;
                                                case 5: Required = 7200; break;
                                                case 6: Required = 10200; break;
                                                case 7: Required = 13800; break;
                                                case 8: Required = 17400; break;
                                                case 9: Required = ushort.MaxValue; break;
                                            }
                                            break;
                                        case Game.ClassID.Sage:
                                            switch (Sc.Level)
                                            {
                                                case 1: Required = 400; break;
                                                case 2: Required = 1200; break;
                                                case 3: Required = 2400; break;
                                                case 4: Required = 3600; break;
                                                case 5: Required = 4800; break;
                                                case 6: Required = 6800; break;
                                                case 7: Required = 9200; break;
                                                case 8: Required = 11600; break;
                                                case 9: Required = ushort.MaxValue; break;
                                            }
                                            break;
                                        case Game.ClassID.Apothecary:
                                            switch (Sc.Level)
                                            {
                                                case 1: Required = 100; break;
                                                case 2: Required = 200; break;
                                                case 3: Required = 300; break;
                                                case 4: Required = 400; break;
                                                case 5: Required = 500; break;
                                                case 6: Required = 1000; break;
                                                case 7: Required = 4000; break;
                                                case 8: Required = 9000; break;
                                                case 9: Required = ushort.MaxValue; break;
                                            }
                                            break;
                                        case Game.ClassID.Wrangler:
                                        case Game.ClassID.Performer:
                                            switch (Sc.Level)
                                            {
                                                case 1: Required = 400; break;
                                                case 2: Required = 1200; break;
                                                case 3: Required = 2400; break;
                                                case 4: Required = 3600; break;
                                                case 5: Required = 4800; break;
                                                case 6: Required = 6800; break;
                                                case 7: Required = 9200; break;
                                                case 8: Required = 11600; break;
                                                case 9: Required = ushort.MaxValue; break;
                                            }
                                            break;
                                    }
                                    #endregion
                                    if (client.Entity.SubClasses.StudyPoints >= Required)
                                    {
                                        client.Entity.SubClasses.StudyPoints -= Required;
                                        client.Entity.SubClasses.Classes[Class].Level++;
                                        Packet = new byte[0];
                                        Packet = new SubClassShowFull(true) { ID = 1, Class = Class, Level = client.Entity.SubClasses.Classes[Class].Level }.ToArray();
                                        client.Send(Packet);
                                        Packet = new SubClass(client.Entity).ToArray();
                                        client.Send(Packet);
                                        Database.SubClassTable.Update(client.Entity, client.Entity.SubClasses.Classes[Class]);
                                        Database.SubClassTable.Update(client.Entity);
                                    }
                                    break;
                                }
                            #endregion
                            #region [Info]
                            case 6:
                                Game.Entity Owner = client.Entity;
                                if (Owner.SubClasses.Classes.Count > 0)
                                {
                                    Game.SubClass[] Classes = new Game.SubClass[Owner.SubClasses.Classes.Count];
                                    Owner.SubClasses.Classes.Values.CopyTo(Classes, 0);
                                    foreach (Game.SubClass Class in Classes)
                                    {
                                        if (Class.ID == 9)
                                        {
                                            for (byte i = 0; i < Class.Phase; i++)
                                            {
                                                Packet = new byte[0];
                                                Packet = new SubClassShowFull(true) { ID = 4, Class = Class.ID, Level = Class.Level }.ToArray();
                                                client.Send(Packet);
                                            }
                                            continue;
                                        }
                                        Packet = new byte[0];
                                        Packet = new SubClassShowFull(true) { ID = 4, Class = Class.ID, Level = Class.Level }.ToArray();
                                        client.Send(Packet);
                                    }
                                }
                                Packet = new SubClass(client.Entity).ToArray();
                                client.Send(Packet);
                                break;
                            #endregion
                            default:
                                Console.WriteLine("Unknown 2320 packet type " + packet[4]);
                                break;
                        }
                        break;
                    }
                #endregion
                #region MemoryAgate
                case 2110:
                    {
                        uint ItemUID = BitConverter.ToUInt32(packet, 8);
                        switch (packet[4])
                        {
                            case 1://record
                                {
                                    if (client.Map.IsDynamic()) return;
                                    Interfaces.IConquerItem Item = null;
                                    if (client.Inventory.TryGetItem(ItemUID, out Item))
                                    {
                                        if (Item.Agate_map.ContainsKey(packet[12]))
                                        {
                                            Item.Agate_map[(uint)packet[12]] = client.Entity.MapID
                                               + "~" + client.Entity.X
                                               + "~" + client.Entity.Y;
                                            Database.ConquerItemTable.UpdateItem(Item, client);
                                            Item.SendAgate(client);
                                            break;
                                        }
                                        if (packet[12] > Item.Agate_map.Count)
                                        {
                                            Item.Agate_map.Add((byte)(Item.Agate_map.Count), client.Entity.MapID
                                               + "~" + client.Entity.X
                                               + "~" + client.Entity.Y);
                                            Database.ConquerItemTable.UpdateItem(Item, client);
                                            Item.SendAgate(client);
                                            break;
                                        }
                                        else
                                        {
                                            if (!Item.Agate_map.ContainsKey(packet[12]))
                                            {

                                                Item.Agate_map.Add(packet[12], client.Entity.MapID
                                                    + "~" + client.Entity.X

                                                   + "~" + client.Entity.Y);
                                                Database.ConquerItemTable.UpdateItem(Item, client);
                                                Item.SendAgate(client);
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case 3://recal
                                {
                                    if (client.Map.IsDynamic()) return;
                                    Interfaces.IConquerItem Item = null;
                                    if (client.Inventory.TryGetItem(ItemUID, out Item))
                                    {

                                        if (Item.Agate_map.ContainsKey(packet[12]))
                                        {
                                            if (ushort.Parse(Item.Agate_map[packet[12]].Split('~')[0].ToString()) == 1038)
                                                return;
                                            if (ushort.Parse(Item.Agate_map[packet[12]].Split('~')[0].ToString()) == 6001)
                                                return;
                                            client.Entity.Teleport(ushort.Parse(Item.Agate_map[packet[12]].Split('~')[0].ToString())
                                                , ushort.Parse(Item.Agate_map[packet[12]].Split('~')[1].ToString())
                                                , ushort.Parse(Item.Agate_map[packet[12]].Split('~')[2].ToString()));
                                            Item.Durability--;
                                            Item.SendAgate(client);
                                            Database.ConquerItemTable.UpdateItem(Item, client);
                                        }
                                    }
                                    break;
                                }
                            case 4://repair
                                {
                                    Interfaces.IConquerItem Item = null;
                                    if (client.Inventory.TryGetItem(ItemUID, out Item))
                                    {
                                        uint cost = (uint)(Item.MaximDurability - Item.Durability) / 2;
                                        if (cost == 0)
                                            cost = 1;
                                        if (client.Entity.ConquerPoints > cost)
                                        {
                                            client.Entity.ConquerPoints -= cost;
                                            Item.Durability = Item.MaximDurability;
                                            Item.SendAgate(client);
                                            Database.ConquerItemTable.UpdateItem(Item, client);
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                case 1128:
                    {
                        p1128 vp = new p1128(false);
                        vp.Deserialize(packet);
                        switch (vp.UID)
                        {
                            case 0://player city teleport
                                {
                                    switch (vp.UID2)
                                    {
////////////////////////////////////////////////////////////////////////////////////////////////////
                                        case 1://tc
                                            client.Entity.Teleport(1002, 429, 378);
                                            break;
                                        case 2://pc
                                            client.Entity.Teleport(1011, 188, 264);
                                            break;
                                        case 3://ac
                                            client.Entity.Teleport(1020, 565, 562);
                                            break;
                                        case 4://dc
                                            client.Entity.Teleport(1000, 500, 650);
                                            break;
                                        case 5://bc
                                            client.Entity.Teleport(1015, 717, 571);
                                            break;
      ////////////////////////////////////////////////////////////////////////////////////////
                                           
                                {
                                    

                                }
                                        default:Console.WriteLine("Unknown 1128 portal subtype 1 : " + vp.UID2); break;
                                    }
                                    break;
                                }
                            case 1://Team city teleport
                                {
                                    switch (vp.UID2)
                                    {
                                        ////////////////////////////////////////////////////////////////////////////////////////////////////
                                        case 1://tc
                                            foreach (Client.GameState teammate in client.Entity.Owner.Team.Teammates)
                                            {
                                                if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, teammate.Entity.X, teammate.Entity.Y) <= 18)
                                                {
                                                    teammate.Entity.Teleport(1002, 429, 378);
                                                }
                                            }
                                            client.Entity.Teleport(1002, 429, 378);
                                            break;
                                        case 2://pc
                                            foreach (Client.GameState teammate in client.Entity.Owner.Team.Teammates)
                                            {
                                                if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, teammate.Entity.X, teammate.Entity.Y) <= 18)
                                                {
                                                    teammate.Entity.Teleport(1011, 188, 264);
                                                }
                                            }
                                            client.Entity.Teleport(1011, 188, 264);
                                            break;
                                        case 3://ac
                                            foreach (Client.GameState teammate in client.Entity.Owner.Team.Teammates)
                                            {
                                                if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, teammate.Entity.X, teammate.Entity.Y) <= 18)
                                                {
                                                    teammate.Entity.Teleport(1020, 565, 562);
                                                }
                                            }
                                            client.Entity.Teleport(1020, 565, 562);
                                            break;
                                        case 4://dc
                                            foreach (Client.GameState teammate in client.Entity.Owner.Team.Teammates)
                                            {
                                                if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, teammate.Entity.X, teammate.Entity.Y) <= 18)
                                                {
                                                    teammate.Entity.Teleport(1000, 500, 650);
                                                }
                                            }
                                            client.Entity.Teleport(1000, 500, 650);
                                            break;
                                        case 5://bc
                                            foreach (Client.GameState teammate in client.Entity.Owner.Team.Teammates)
                                            {
                                                if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, teammate.Entity.X, teammate.Entity.Y) <= 18)
                                                {
                                                    teammate.Entity.Teleport(1015, 717, 571);
                                                }
                                            }
                                            client.Entity.Teleport(1015, 717, 571);
                                            break;
                                            ////////////////////////////////////////////////////////////////////////////////////////
                                            {


                                            }
                                        default: Console.WriteLine("Unknown 1128 portal subtype 2 : " + vp.UID2); break;
                                    }
                                    break;
                                }
                    
                                 default:
                                Console.WriteLine("Unknown 1128 subtype: " + vp.UID); break;
                        }
                        break;
                    }
                default:
                    {
                        if (client.Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.ProjectManager)
                            client.Send(new Message("Unknown type: " + ID + " with length " + packet.Length+"Unknown type: " + ID2 , System.Drawing.Color.CadetBlue, Message.Talk));
                        if (ID == 1040)
                            client.Send(packet);
                        break;
                    }
            }
        }

        #region Reincarnation
        public class Reincarnation
        {
            private Client.GameState _client;
            private SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill> RemoveSkill = null;
            private SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill> Addskill = null;
            public Reincarnation(Client.GameState client, byte new_class)
            {
                if (client.Entity.Level < 130)
                    return;
                _client = client;
                RemoveSkill = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(500);
                Addskill = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(500);
                #region Low level items
                for (byte i = 1; i < 9; i++)
                {
                    if (i != 7)
                    {
                        Interfaces.IConquerItem item = client.Equipment.TryGetItem(i);
                        if (item != null && item.ID != 0)
                        {
                            try
                            {
                                //client.UnloadItemStats(item, false);
                                Database.ConquerItemInformation cii = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                                item.ID = cii.LowestID(Network.PacketHandler.ItemMinLevel(Network.PacketHandler.ItemPosition(item.ID)));
                                item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                item.Send(client);
                                client.LoadItemStats(item);
                                Database.ConquerItemTable.UpdateItemID(item, client);
                            }
                            catch
                            {
                                Console.WriteLine("Reborn item problem: " + item.ID);
                            }
                        }
                    }
                }
                Interfaces.IConquerItem hand = client.Equipment.TryGetItem(5);
                if (hand != null)
                {
                    client.Equipment.Remove(5);
                    client.CalculateStatBonus();
                    client.CalculateHPBonus();
                    client.SendStatMessage();
                }
                else
                    client.SendScreen(client.Entity.SpawnPacket, false);
                #endregion

                #region Remove Extra Skill
                if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 15 && client.Entity.Class == 15)
                {
                    WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs.DragonWhirl);
                }
                if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 25 && client.Entity.Class == 25)
                {
                    WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs.Perseverance);
                }
                if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 45 && client.Entity.Class == 45)
                {
                    WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs.StarArrow);
                }
                if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 55 && client.Entity.Class == 55)
                {
                    WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs.PoisonStar);
                }
                if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 65 && client.Entity.Class == 65)
                {
                    WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs.SoulShackle);
                }
                if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 135 && client.Entity.Class == 135)
                {
                    WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs.AzureShield);
                }
                if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 145 && client.Entity.Class == 145)
                {
                    WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs.HeavenBlade);
                }
                #endregion
                client.Entity.FirstRebornClass = client.Entity.SecondRebornClass;
                client.Entity.SecondRebornClass = client.Entity.Class;
                client.Entity.Class = new_class;
                client.Entity.SecondRebornLevel = client.Entity.Level;
                //client.Entity.ReincarnationLev = client.Entity.Level;//kikoz
                client.Entity.Level = 15;
                client.Entity.Experience = 0;
                client.Entity.Atributes =
     (ushort)(client.ExtraAtributePoints(client.Entity.FirstRebornClass, client.Entity.FirstRebornLevel) +
      client.ExtraAtributePoints(client.Entity.SecondRebornClass, client.Entity.SecondRebornLevel) + 62);



                client.Spells.Clear();
                client.Spells = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(100);
                switch (client.Entity.FirstRebornClass)
                {
                    case 15:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Cyclone);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Hercules);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.SpiritHealing);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Robot);
                            break;
                        }
                    case 25:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.SuperMan);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Dash);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Shield);
                            break;
                        }
                    case 45:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Intensify);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Scatter);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.RapidFire);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.XPFly);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.AdvancedFly);
                            break;
                        }
                    case 55:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FatalStrike);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.ShurikenVortex);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.ToxicFog);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TwofoldBlades);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.PoisonStar);

                            break;
                        }
                    case 65:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.RadiantPalm);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.WhirlWindKick);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TripleAttack);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Oblivion);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Serenity);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Compassion);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TyrantAura);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TyrantAura);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.DeflectionAura);
                            break;
                        }
                    case 135:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Thunder);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.WaterElf);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Cure);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Lightning);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Volcano);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Pray);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.AdvancedCure);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Meditation);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Stigma);
                            break;
                        }
                    case 140:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Thunder);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Cure);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Lightning);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Tornado);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FireCircle);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FireMeteor);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FireRing);
                            break;
                        }

                }

                byte PreviousClass = client.Entity.FirstRebornClass;
                byte toClass = (byte)(client.Entity.SecondRebornClass - 4);

                Interfaces.ISkill[] ADD_spells = this.Addskill.Values.ToArray();
                foreach (Interfaces.ISkill skill in ADD_spells)
                {
                    skill.Available = true;
                    if (!client.Spells.ContainsKey(skill.ID))
                        client.Spells.Add(skill.ID, skill);
                }
                #region Spells
                Interfaces.ISkill[] spells = client.Spells.Values.ToArray();
                foreach (Interfaces.ISkill spell in spells)
                {
                    spell.PreviousLevel = spell.Level;
                    spell.Level = 0;
                    spell.Experience = 0;
                    #region Monk
                    if (PreviousClass == 65)
                    {
                        if (client.Entity.Class != 61)
                        {
                            switch (spell.ID)
                            {
                                case 10490:
                                case 10415:
                                case 10381:
                                    client.RemoveSpell(spell);
                                    break;
                            }
                        }
                    }
                    #endregion
                    #region Warrior
                    if (PreviousClass == 25)
                    {
                        if (client.Entity.Class != 21)
                        {
                            switch (spell.ID)
                            {
                                case 1025:
                                    if (client.Entity.Class != 21 && client.Entity.Class != 132)
                                        client.RemoveSpell(spell);
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
                                client.RemoveSpell(spell);
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
                                client.RemoveSpell(spell);
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
                                client.RemoveSpell(spell);
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
                                    client.RemoveSpell(spell);
                                    break;
                                case 1050:
                                case 1175:
                                case 1170:
                                    if (toClass != 142)
                                        client.RemoveSpell(spell);
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
                                    client.RemoveSpell(spell);
                                    break;
                            }
                        }
                    }
                    #endregion
                    if (client.Spells.ContainsKey(spell.ID))
                        if (spell.ID != (ushort)Game.Enums.SkillIDs.Reflect)
                            spell.Send(client);
                }
                #endregion
                Add(Conquer_Online_Server.Game.Enums.SkillIDs.Bless);

                Addskill.Clear();
                Addskill = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(100);

                PreviousClass = client.Entity.SecondRebornClass;
                toClass = client.Entity.Class;
                switch (client.Entity.SecondRebornClass)
                {
                    case 15:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Robot);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Cyclone);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Hercules);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.SpiritHealing);

                            break;
                        }
                    case 25:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.SuperMan);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Dash);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Shield);
                            break;
                        }
                    case 45:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Intensify);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Scatter);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.RapidFire);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.XPFly);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.AdvancedFly);
                            break;
                        }
                    case 55:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FatalStrike);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.ShurikenVortex);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.ToxicFog);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TwofoldBlades);
                            break;
                        }
                    case 65:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.RadiantPalm);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.WhirlWindKick);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TripleAttack);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Oblivion);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Serenity);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Compassion);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TyrantAura);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.TyrantAura);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.DeflectionAura);
                            break;
                        }
                    case 135:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Thunder);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.WaterElf);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Cure);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Lightning);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Volcano);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Pray);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Stigma);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.AdvancedCure);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Meditation);
                            break;
                        }
                    case 140:
                        {
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Thunder);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Cure);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Lightning);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.Tornado);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FireCircle);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FireMeteor);
                            Add(Conquer_Online_Server.Game.Enums.SkillIDs.FireRing);
                            break;
                        }

                }

                //PreviousClass = client.Entity.FirstRebornClass;
                //toClass = client.Entity.SecondRebornClass;
                Add(Conquer_Online_Server.Game.Enums.SkillIDs.Bless);

                Interfaces.ISkill[] aADD_spells = this.Addskill.Values.ToArray();
                foreach (Interfaces.ISkill skill in aADD_spells)
                {
                    skill.Available = true;
                    if (!client.Spells.ContainsKey(skill.ID))
                        client.Spells.Add(skill.ID, skill);
                }
                #region Spells
                Interfaces.ISkill[] aspells = client.Spells.Values.ToArray();
                foreach (Interfaces.ISkill aspell in spells)
                {
                    aspell.PreviousLevel = aspell.Level;
                    aspell.Level = 0;
                    aspell.Experience = 0;
                    #region Monk
                    if (PreviousClass == 65)
                    {
                        if (client.Entity.Class != 61)
                        {
                            switch (aspell.ID)
                            {
                                case 10490:
                                case 10415:
                                case 10381:
                                    client.RemoveSpell(aspell);
                                    break;
                            }
                        }
                    }
                    #endregion
                    #region Warrior
                    if (PreviousClass == 25)
                    {
                        if (client.Entity.Class != 21)
                        {
                            switch (aspell.ID)
                            {
                                case 1025:
                                    if (client.Entity.Class != 21 && client.Entity.Class != 132)
                                        client.RemoveSpell(aspell);
                                    break;
                            }
                        }
                    }
                    #endregion
                    #region Ninja
                    if (toClass != 51)
                    {
                        switch (aspell.ID)
                        {
                            case 6010:
                            case 6000:
                            case 6011:
                                client.RemoveSpell(aspell);
                                break;
                        }
                    }
                    #endregion
                    #region Trojan
                    if (toClass != 11)
                    {
                        switch (aspell.ID)
                        {
                            case 1115:
                                client.RemoveSpell(aspell);
                                break;
                        }
                    }
                    #endregion
                    #region Archer
                    if (toClass != 41)
                    {
                        switch (aspell.ID)
                        {
                            case 8001:
                            case 8000:
                            case 8003:
                            case 9000:
                            case 8002:
                            case 8030:
                                client.RemoveSpell(aspell);
                                break;
                        }
                    }
                    #endregion
                    #region WaterTaoist
                    if (PreviousClass == 135)
                    {
                        if (toClass != 132)
                        {
                            switch (aspell.ID)
                            {
                                case 1000:
                                case 1001:
                                case 1010:
                                case 1125:
                                case 1100:
                                case 8030:
                                    client.RemoveSpell(aspell);
                                    break;
                                case 1050:
                                case 1175:
                                case 1170:
                                    if (toClass != 142)
                                        client.RemoveSpell(aspell);
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
                            switch (aspell.ID)
                            {
                                case 1000:
                                case 1001:
                                case 1150:
                                case 1180:
                                case 1120:
                                case 1002:
                                case 1160:
                                case 1165:
                                    client.RemoveSpell(aspell);
                                    break;
                            }
                        }
                    }
                    #endregion
                    if (client.Spells.ContainsKey(aspell.ID))
                        if (aspell.ID != (ushort)Game.Enums.SkillIDs.Reflect)
                            aspell.Send(client);
                }
                #endregion
                Addskill.Clear();
                Addskill = new SafeDictionary<ushort, Conquer_Online_Server.Interfaces.ISkill>(20);
                #region Add Extra Skill
                if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 15 && client.Entity.Class == 11)
                {
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.DragonWhirl);
                }
                if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 25 && client.Entity.Class == 21)
                {
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.Perseverance);
                }
                if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 45 && client.Entity.Class == 41)
                {
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.StarArrow);
                }
                if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 55 && client.Entity.Class == 55)
                {
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.PoisonStar);
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.CounterKill);
                }
                if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 65 && client.Entity.Class == 61)
                {
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.SoulShackle);
                }
                if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 135 && client.Entity.Class == 132)
                {
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.AzureShield);
                }
                if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 145 && client.Entity.Class == 142)
                {
                    Add(Conquer_Online_Server.Game.Enums.SkillIDs.HeavenBlade);
                }
                #endregion
                Interfaces.ISkill[] aaADD_spells = this.Addskill.Values.ToArray();
                foreach (Interfaces.ISkill skill in aaADD_spells)
                {
                    skill.Available = true;
                    if (!client.Spells.ContainsKey(skill.ID))
                        client.Spells.Add(skill.ID, skill);
                }

                #region Proficiencies
                foreach (Interfaces.ISkill proficiency in client.Proficiencies.Values)
                {
                    proficiency.PreviousLevel = proficiency.Level;
                    proficiency.Level = 0;
                    proficiency.Experience = 0;
                    proficiency.Send(client);
                }
                #endregion
                Database.DataHolder.GetStats(client.Entity.Class, client.Entity.Level, client);
                client.CalculateStatBonus();
                client.CalculateHPBonus();
                client.GemAlgorithm();
                client.SendStatMessage();
                Network.PacketHandler.WorldMessage(client.Entity.Name + " has got Reincarnation! Congratulations!");

            }
            void Add(Conquer_Online_Server.Game.Enums.SkillIDs S)
            {
                Interfaces.ISkill New = new Network.GamePackets.Spell(true);
                New.ID = (ushort)S;
                New.Level = 0;
                New.Experience = 0;
                New.PreviousLevel = 0;
                New.Send(_client);
                Addskill.Add(New.ID, New);
            }

            void WontAdd(Conquer_Online_Server.Game.Enums.SkillIDs S)
            {
                Network.GamePackets.Data data = new Data(true);
                data.UID = _client.Entity.UID;
                data.dwParam = (byte)S;
                data.ID = 109;
                data.Send(_client);

                Interfaces.ISkill New = new Network.GamePackets.Spell(true);
                New.ID = (ushort)S;
                New.Level = 0;
                New.Experience = 0;
                New.PreviousLevel = 0;
                RemoveSkill.Add(New.ID, New);
            }
        }
        #endregion
        #region SubClasses
        public static bool PassRequeriments(Game.SubClass Sc, Game.Entity Entity)
        {
            Boolean Pass = false;
            byte Level = 90;
            byte Reborns = 0;
            #region [Switch Phases]
            switch (Sc.Phase)
            {
                case 0:
                case 1: return true;
                case 2: Level = 90; Reborns = 0; break;
                case 3: Level = 110; Reborns = 0; break;
                case 4: Level = 120; Reborns = 0; break;
                case 5: Level = 90; Reborns = 1; break;
                case 6: Level = 110; Reborns = 1; break;
                case 7: Level = 120; Reborns = 1; break;
                case 8:
                case 9: Level = 120; Reborns = 2; break;
            }
            #endregion

            if (Sc.Level > Sc.Phase && Entity.Level >= Level && Entity.Reborn >= Reborns)
                Pass = true;

            if (Sc.Level == 9 && Sc.Phase == 9)
                Pass = false;

            return Pass;
        }

        public static bool PassLearn(byte ID, Game.Entity Entity)
        {
            Boolean Pass = false;

            switch ((Game.ClassID)ID)
            {
                case Game.ClassID.MartialArtist:
                    if (Entity.Owner.Inventory.Contains(721259, 5))
                    {
                        Entity.Owner.Inventory.Remove(721259, 5);
                        Pass = true;
                    }
                    break;
                case Game.ClassID.Warlock:
                    if (Entity.Owner.Inventory.Contains(721261, 10))
                    {
                        Entity.Owner.Inventory.Remove(721261, 10);
                        Pass = true;
                    }
                    break;
                case Game.ClassID.ChiMaster:
                    if (Entity.Owner.Inventory.Contains(711188, 1))
                    {
                        Entity.Owner.Inventory.Remove(711188, 1);
                        Pass = true;
                    }
                    break;
                case Game.ClassID.Sage:
                    if (Entity.Owner.Inventory.Contains(723087, 20))
                    {
                        Entity.Owner.Inventory.Remove(723087, 20);
                        Pass = true;
                    }
                    break;
                case Game.ClassID.Apothecary:
                    if (Entity.Owner.Inventory.Contains(1088002, 10))
                    {
                        Entity.Owner.Inventory.Remove(1088002, 10);
                        Pass = true;
                    }
                    break;
                case Game.ClassID.Performer:
                    if (Entity.Owner.Inventory.Contains(753001, 15))
                    {
                        Entity.Owner.Inventory.Remove(753001, 15);
                        Pass = true;


                    }
                    break;
                case Game.ClassID.Wrangler:
                    {
                        if (Entity.Owner.Inventory.Contains(723903, 40))
                        {
                            Entity.Owner.Inventory.Remove(723903, 40);
                            Pass = true;

                        }
                    } break;
            }

            return Pass;
        }
        #endregion
        #region Flower

        public static void AddFlowers(Client.GameState client, byte[] packet)
        {
            if (packet[4] == 2)
            {
                if (client.Entity.MyFlowers != null)
                {
                    Game.Struct.Flowers F = client.Entity.MyFlowers;
                    //string ToSend = " " + F.RedRoses.ToString() + " " + F.RedRoses2day.ToString() + " " + F.Lilies.ToString() + " " + F.Lilies2day.ToString() + " ";
                    //ToSend += F.Orchads.ToString() + " " + F.Orchads2day.ToString() + " " + F.Tulips.ToString() + " " + F.Tulips2day.ToString();
                    Network.GamePackets.FlowerPacket flow = new FlowerPacket(F);
                    client.Send(flow.ToArray());


                    int mybestcount = 0;
                    if (F.Lilies > mybestcount)
                        mybestcount = F.Lilies;
                    if (F.Orchads > mybestcount)
                        mybestcount = F.Orchads;
                    if (F.RedRoses > mybestcount)
                        mybestcount = F.RedRoses;
                    if (F.Tulips > mybestcount)
                        mybestcount = F.Tulips;



                    if (F.Lilies == mybestcount)
                    {
                        if (mybestcount >= ServerBase.Kernel.MaxLilies)
                        {
                            client.Entity.ActualMyTypeFlower = 30010102;
                            FlowerSpawn fl = new FlowerSpawn("1", client.Entity.Name, F.Lilies.ToString(), client.Entity.UID.ToString(), 30000102);
                            client.Send(fl.ThePacket());
                            ServerBase.Kernel.MaxLilies = (uint)mybestcount;
                        }
                    }
                    if (F.Orchads == mybestcount)
                    {
                        if (mybestcount >= ServerBase.Kernel.MaxOrchads)
                        {
                            client.Entity.ActualMyTypeFlower = 30010202;
                            FlowerSpawn fl = new FlowerSpawn("1", client.Entity.Name, F.Orchads.ToString(), client.Entity.UID.ToString(), 30000202);
                            client.Send(fl.ThePacket());
                            ServerBase.Kernel.MaxOrchads = (uint)mybestcount;
                        }
                    }
                    if (F.RedRoses == mybestcount)
                    {
                        if (mybestcount >= ServerBase.Kernel.MaxRoses)
                        {
                            client.Entity.ActualMyTypeFlower = 30010002;
                            FlowerSpawn fl = new FlowerSpawn("1", client.Entity.Name, F.RedRoses.ToString(), client.Entity.UID.ToString(), 30000002);
                            client.Send(fl.ThePacket());
                            ServerBase.Kernel.MaxRoses = (uint)mybestcount;
                        }
                    }
                    if (F.Tulips == mybestcount)
                    {
                        if (mybestcount >= ServerBase.Kernel.MaxTulips)
                        {
                            client.Entity.ActualMyTypeFlower = 30010302;
                            FlowerSpawn fl = new FlowerSpawn("1", client.Entity.Name, F.Tulips.ToString(), client.Entity.UID.ToString(), 30000302);
                            client.Send(fl.ThePacket());
                        } ServerBase.Kernel.MaxTulips = (uint)mybestcount;
                    }
                }
                else
                {
                    client.Entity.MyFlowers = new Conquer_Online_Server.Game.Struct.Flowers();
                    Game.Struct.Flowers F = client.Entity.MyFlowers;
                    Network.GamePackets.FlowerPacket flow = new FlowerPacket(F);
                    client.Send(flow.ToArray());
                    Database.Flowers.SaveFlowerRank(client);
                }
                byte[] packe2nd = new byte[24]
                                {
                                    0x10, 0x00 , 0x7F , 0x04 , 0x05 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00   
                                   , 0x54 , 0x51 , 0x53 , 0x65 , 0x72 , 0x76 , 0x65 , 0x72                  
                                };
                client.Send(packe2nd);
            }
        }
        public static void SendFlowers(Client.GameState client, byte[] packet)
        {
            uint TargetID = BitConverter.ToUInt32(packet, 8);
            int Flowers = BitConverter.ToUInt16(packet, 20);
            int Type = packet[24];
            if (Flowers == 1)
            {
                if (client.Entity.AddFlower == 0)
                {
                    client.Send(new Network.GamePackets.Message("Sorry, but you send the flower today! ", System.Drawing.Color.White, Message.Center));
                    return;
                }
            }

            bool change = false;
            if (ServerBase.Kernel.GamePool.ContainsKey(TargetID))
            {
                Client.GameState Cclient = ServerBase.Kernel.GamePool[TargetID];
                if (Conquer_Online_Server.ServerBase.Kernel.AllFlower.ContainsKey(Cclient.Entity.UID))
                {
                    if (Cclient.Entity.Body.ToString().EndsWith("3") || Cclient.Entity.Body.ToString().EndsWith("4"))
                        return;
                    byte[] packe2nd2 = new byte[24]
                                {
                                    0x10, 0x00 , 0x7F , 0x04 , 0x02 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00   
                                   , 0x54 , 0x51 , 0x53 , 0x65 , 0x72 , 0x76 , 0x65 , 0x72                  
                                };
                    Cclient.Send(packe2nd2);
                    Game.Struct.Flowers F = Conquer_Online_Server.ServerBase.Kernel.AllFlower[Cclient.Entity.UID];
                    switch (Type)
                    {

                        case 0:
                            {
                                string It = "751";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                if (client.Inventory.Contains(uint.Parse(It), 1))
                                    client.Inventory.Remove(uint.Parse(It), 1);
                                client.Send(packet);
                                F.RedRoses += Flowers;
                                F.RedRoses2day += Flowers;
                                //if (F.RedRoses > 100000)
                                //   Cclient.Entity.FlowerSend = "TakeOnRoseSuit";
                                Cclient.Entity.MyFlowers = Conquer_Online_Server.ServerBase.Kernel.AllFlower[Cclient.Entity.UID];
                                Cclient.Send(packet);

                                int mybestcount = 0;
                                if (F.Lilies > mybestcount)
                                    mybestcount = F.Lilies;
                                if (F.Orchads > mybestcount)
                                    mybestcount = F.Orchads;
                                if (F.RedRoses > mybestcount)
                                    mybestcount = F.RedRoses;
                                if (F.Tulips > mybestcount)
                                    mybestcount = F.Tulips;
                                if (F.RedRoses == mybestcount)
                                {
                                    if (F.RedRoses >= ServerBase.Kernel.MaxRoses)
                                    {
                                        Cclient.Entity.ActualMyTypeFlower = 30010002;
                                        Cclient.Send(new FlowerSpawn("1", Cclient.Entity.Name, F.RedRoses.ToString(), Cclient.Entity.UID.ToString(), 30000002).ThePacket());
                                        ServerBase.Kernel.MaxRoses = (uint)F.RedRoses;
                                        change = true;
                                    }
                                }

                                break;
                            }
                        case 1:
                            {
                                string It = "752";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                if (client.Inventory.Contains(uint.Parse(It), 1))
                                    client.Inventory.Remove(uint.Parse(It), 1);
                                client.Send(packet);
                                F.Lilies += Flowers;
                                F.Lilies2day += Flowers;
                                //if (F.Lilies > 100000)
                                //    Cclient.Entity.FlowerSend = "TakeOnLilySuit";

                                Cclient.Entity.MyFlowers = Conquer_Online_Server.ServerBase.Kernel.AllFlower[Cclient.Entity.UID];
                                Cclient.Send(packet);
                                int mybestcount = 0;
                                if (F.Lilies > mybestcount)
                                    mybestcount = F.Lilies;
                                if (F.Orchads > mybestcount)
                                    mybestcount = F.Orchads;
                                if (F.RedRoses > mybestcount)
                                    mybestcount = F.RedRoses;
                                if (F.Tulips > mybestcount)
                                    mybestcount = F.Tulips;
                                if (F.Lilies == mybestcount)
                                {
                                    if (F.RedRoses >= ServerBase.Kernel.MaxLilies)
                                    {
                                        Cclient.Entity.ActualMyTypeFlower = 30010102;
                                        FlowerSpawn fl = new FlowerSpawn("1", Cclient.Entity.Name, F.Lilies.ToString(), Cclient.Entity.UID.ToString(), 30000102);
                                        Cclient.Send(fl.ThePacket());
                                        ServerBase.Kernel.MaxLilies = (uint)F.RedRoses;
                                        change = true;
                                    }
                                }
                                break;
                            }
                        case 2:
                            {
                                string It = "753";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";
                                if (client.Inventory.Contains(uint.Parse(It), 1))
                                    client.Inventory.Remove(uint.Parse(It), 1);
                                client.Send(packet);
                                F.Orchads += Flowers;
                                F.Orchads2day += Flowers;
                                //if (F.Orchads> 100000)
                                //    Cclient.Entity.FlowerSend = "TakeOnOrchisSuit";

                                Cclient.Entity.MyFlowers = Conquer_Online_Server.ServerBase.Kernel.AllFlower[Cclient.Entity.UID];
                                Cclient.Send(packet);
                                int mybestcount = 0;
                                if (F.Lilies > mybestcount)
                                    mybestcount = F.Lilies;
                                if (F.Orchads > mybestcount)
                                    mybestcount = F.Orchads;
                                if (F.RedRoses > mybestcount)
                                    mybestcount = F.RedRoses;
                                if (F.Tulips > mybestcount)
                                    mybestcount = F.Tulips;
                                if (F.Orchads == mybestcount)
                                {
                                    if (F.Orchads >= ServerBase.Kernel.MaxOrchads)
                                    {
                                        Cclient.Entity.ActualMyTypeFlower = 30010202;
                                        FlowerSpawn fl = new FlowerSpawn("1", Cclient.Entity.Name, F.Orchads.ToString(), Cclient.Entity.UID.ToString(), 30000202);
                                        Cclient.Send(fl.ThePacket());
                                        ServerBase.Kernel.MaxOrchads = (uint)F.Orchads;
                                        change = true;
                                    }
                                }

                                break;

                            }
                        case 3:
                            {
                                string It = "754";
                                if (Flowers == 1)
                                    It += "001";
                                else if (Flowers == 3)
                                    It += "003";
                                else if (Flowers == 9)
                                    It += "009";
                                else if (Flowers == 99)
                                    It += "099";
                                else if (Flowers == 999)
                                    It += "999";

                                if (client.Inventory.Contains(uint.Parse(It), 1))
                                    client.Inventory.Remove(uint.Parse(It), 1);
                                client.Send(packet);
                                F.Tulips += Flowers;
                                F.Tulips2day += Flowers;
                                //if (F.Tulips > 100000)
                                //    Cclient.Entity.FlowerSend = "TakeOnTulipSuit";

                                Cclient.Entity.MyFlowers = Conquer_Online_Server.ServerBase.Kernel.AllFlower[Cclient.Entity.UID];
                                Cclient.Send(packet);
                                int mybestcount = 0;
                                if (F.Lilies > mybestcount)
                                    mybestcount = F.Lilies;
                                if (F.Orchads > mybestcount)
                                    mybestcount = F.Orchads;
                                if (F.RedRoses > mybestcount)
                                    mybestcount = F.RedRoses;
                                if (F.Tulips > mybestcount)
                                    mybestcount = F.Tulips;
                                if (F.Tulips == mybestcount)
                                {
                                    if (F.Tulips >= ServerBase.Kernel.MaxTulips)
                                    {
                                        Cclient.Entity.ActualMyTypeFlower = 30010302;
                                        FlowerSpawn fl = new FlowerSpawn("1", Cclient.Entity.Name, F.Tulips.ToString(), Cclient.Entity.UID.ToString(), 30000302);

                                        Cclient.Send(fl.ThePacket());
                                        ServerBase.Kernel.MaxTulips = (uint)F.Tulips;
                                        change = true;
                                    }
                                }

                                break;
                            }
                    }

                    if (Flowers == 1)
                        client.Entity.AddFlower = 0;


                    //client.Send(new FlowerSpawn2(Cclient.Entity.UID.ToString()).ThePacket());
                    string name = client.Entity.Name + " " + Cclient.Entity.Name + " " + Flowers + " " + Type + "";
                    byte[] Buffer = new byte[68];
                    Network.Writer.WriteUInt16((ushort)(60), 0, Buffer);
                    Network.Writer.WriteUInt16(1150, 2, Buffer);

                    Network.Writer.WriteUInt32((uint)Flowers, 48, Buffer);

                    Network.Writer.WriteUInt32((uint)Type, 56, Buffer);//52

                    for (int i = 0; i < client.Entity.Name.Length; i++)
                    {
                        try
                        {
                            Buffer[16 + i] = Convert.ToByte(client.Entity.Name[i]);
                        }
                        catch { }
                    }

                    for (int i = 0; i < Cclient.Entity.Name.Length; i++)
                    {
                        try
                        {
                            Buffer[32 + i] = Convert.ToByte(Cclient.Entity.Name[i]);
                        }
                        catch { }
                    }

                    Cclient.Send(Buffer);
                    if (Flowers == 1)
                        client.Send(Buffer);
                    //clienSend(new FlowerSpawn2(Cclient.Entity.UID.ToString()).ThePacket());
                    Network.GamePackets._String strgp = new Network.GamePackets._String(true);
                    strgp.Texts.Add("NEW-flower-r-1");
                    strgp.TextsCount = 1;
                    strgp.Type = 10;
                    strgp.UID = client.Entity.UID;
                    client.Screen.Reload(strgp);

                    strgp.UID = Cclient.Entity.UID;

                    Cclient.Screen.Reload(strgp);


                    Network.GamePackets.FlowerPacket flow = new FlowerPacket(F);
                    Cclient.Send(flow.ToArray());

                    Database.Flowers.SaveFlowerRank(Cclient);

                    if (change)
                    {
                        byte[] packe2nd = new byte[24]
                                {
                                    0x10, 0x00 , 0x7F , 0x04 , 0x05 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00 , 0x00   
                                   , 0x54 , 0x51 , 0x53 , 0x65 , 0x72 , 0x76 , 0x65 , 0x72                  
                                };
                        Cclient.Send(packe2nd);
                    }
                }
            }
        }
        #endregion
        #region Guilds
        static void AllyGuilds(string name, Client.GameState client)
        {
            foreach (var guild in ServerBase.Kernel.Guilds.Values)
            {
                if (guild.Name == name && client.Guild.Name != name)
                {
                    if (guild.Leader != null)
                    {
                        if (guild.Leader.IsOnline)
                        {
                            guild.Leader.Client.OnMessageBoxEventParams = new object[]
                                                            {
                                                                guild,
                                                                client.Guild
                                                            };
                            client.OnMessageBoxEventParams = new object[]
                                                            {
                                                                guild,
                                                                client.Guild
                                                            };
                            Client.GameState Leader = guild.Leader.Client;
                            Leader.OnMessageBoxOK = delegate
                            {
                                Game.ConquerStructures.Society.Guild Guild1 =
                                    Leader.OnMessageBoxEventParams[0] as Game.ConquerStructures.Society.Guild;
                                Game.ConquerStructures.Society.Guild Guild2 =
                                    Leader.OnMessageBoxEventParams[1] as Game.ConquerStructures.Society.Guild;

                                Guild1.AddAlly(Guild2.Name);
                                Guild2.AddAlly(Guild1.Name);

                                if (Guild1.Leader.Client != null)
                                {
                                    if (Guild1.Leader.Client.Socket.Connected)
                                    {
                                        if (Guild2.Leader.Client != null && Guild2.Leader.Client.Socket.Connected)
                                        {
                                            Guild2.Leader.Client.Send(new Message(Guild1.Leader.Name + " has accepted your ally request.", System.Drawing.Color.Blue, Message.TopLeft));
                                        }
                                    }
                                }
                            };
                            guild.Leader.Client.OnMessageBoxCANCEL = delegate
                            {
                                try
                                {
                                    if (guild.Leader.Client != null)
                                    {
                                        if (guild.Leader.Client.Socket.Connected)
                                        {
                                            if (guild.Leader.Client.OnMessageBoxEventParams != null)
                                            {
                                                Game.ConquerStructures.Society.Guild Guild2 =
                                                    guild.Leader.Client.OnMessageBoxEventParams[1] as Game.ConquerStructures.Society.Guild;
                                                Game.ConquerStructures.Society.Guild Guild1 =
                                                    guild.Leader.Client.OnMessageBoxEventParams[0] as Game.ConquerStructures.Society.Guild;

                                                if (Guild2.Leader.IsOnline)
                                                {
                                                    Guild2.Leader.Client.Send(new Message(Guild1.Leader.Name + " has declined your ally request.", System.Drawing.Color.Blue, Message.TopLeft));
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Program.SaveException(e);
                                }
                            };
                            guild.Leader.Client.Send(new NpcReply(NpcReply.MessageBox, client.Entity.Name + " , GuildLeader of " + client.Guild.Name + " wants to make with you an alliance."));
                        }
                    }
                }
            }
        }
        #endregion
        #region Broadcast
        public static void BroadcastInfoAwaiting(Broadcast broadcast, Client.GameState client)
        {
            byte[] buffer = new byte[500];
            Writer.WriteUInt16(2051, 2, buffer);
            int test = 0;
            ushort total = 0;
            if (broadcast.dwParam * 10 + 10 >= ServerBase.Constants.MaxBroadcasts)
                test = ServerBase.Constants.MaxBroadcasts;
            else
                test = (int)broadcast.dwParam * 10 + 10;
            for (uint i = broadcast.dwParam * 10; i < test; i++)
            {
                if (Game.ConquerStructures.Broadcast.Broadcasts.Count > i)
                {
                    var Broadcast = Game.ConquerStructures.Broadcast.Broadcasts[(int)i];
                    Writer.WriteUInt32((ushort)broadcast.dwParam, 4, buffer);
                    Writer.WriteUInt16(total, 8, buffer);
                    int offset = 12 + buffer[10] * 112;
                    buffer[10]++;
                    Writer.WriteUInt32(Broadcast.ID, offset, buffer); offset += 4;
                    Writer.WriteUInt32(i, offset, buffer); offset += 4;
                    Writer.WriteUInt32(Broadcast.EntityID, offset, buffer); offset += 4;
                    Writer.WriteString(Broadcast.EntityName, offset, buffer); offset += 16;
                    Writer.WriteUInt32(Broadcast.SpentCPs, offset, buffer); offset += 4;
                    Writer.WriteString(Broadcast.Message, offset, buffer);
                    if (buffer[10] % 4 == 0)
                    {
                        total++;
                        Writer.WriteUInt16((ushort)(12 + buffer[10] * 112 + 6 + 2), 0, buffer);
                        byte[] Buffer = new byte[12 + buffer[10] * 112 + 6 + 2 + 8];
                        System.Buffer.BlockCopy(buffer, 0, Buffer, 0, Buffer.Length);
                        client.Send(Buffer);
                        buffer = new byte[500];
                        Writer.WriteUInt16(2051, 2, buffer);
                    }
                }
                else
                {
                    Writer.WriteUInt16((ushort)(12 + buffer[10] * 112 + 6 + 2), 0, buffer);
                    byte[] Buffer = new byte[12 + buffer[10] * 112 + 6 + 2 + 8];
                    System.Buffer.BlockCopy(buffer, 0, Buffer, 0, Buffer.Length);
                    if (buffer[10] != 0 || buffer[10] == 0 && total == 0)
                        client.Send(Buffer);
                    break;
                }
            }
        }
        public static void BroadcastClientMessages(Broadcast broadcast, Client.GameState client)
        {
            byte[] buffer = new byte[500];
            Writer.WriteUInt16(2051, 2, buffer);
            int test = 0;
            ushort total = 0;
            for (int i = 0; i < Game.ConquerStructures.Broadcast.Broadcasts.Count; i++)
                if (Game.ConquerStructures.Broadcast.Broadcasts[i].EntityID == client.Entity.UID)
                    test++;
            if ((10 * broadcast.dwParam + 10) >= Game.ConquerStructures.Broadcast.Broadcasts.Count)
            {
                return;
            }
            for (uint i = broadcast.dwParam * 10; i < test; i++)
            {
                if (Game.ConquerStructures.Broadcast.Broadcasts.Count > i)
                {
                    var Broadcast = Game.ConquerStructures.Broadcast.Broadcasts[(int)i];
                    if (Broadcast.EntityID != client.Entity.UID)
                        continue;
                    Writer.WriteUInt32((ushort)broadcast.dwParam, 4, buffer);
                    Writer.WriteUInt16(total, 8, buffer);
                    int offset = 12 + buffer[10] * 112;
                    buffer[10]++;
                    Writer.WriteUInt32(Broadcast.ID, offset, buffer); offset += 4;
                    Writer.WriteUInt32(i, offset, buffer); offset += 4;
                    Writer.WriteUInt32(Broadcast.EntityID, offset, buffer); offset += 4;
                    Writer.WriteString(Broadcast.EntityName, offset, buffer); offset += 16;
                    Writer.WriteUInt32(Broadcast.SpentCPs, offset, buffer); offset += 4;
                    Writer.WriteString(Broadcast.Message, offset, buffer);
                    if (buffer[10] % 4 == 0)
                    {
                        total++;
                        Writer.WriteUInt16((ushort)(12 + buffer[10] * 112 + 6 + 2), 0, buffer);
                        byte[] Buffer = new byte[12 + buffer[10] * 112 + 6 + 2 + 8];
                        System.Buffer.BlockCopy(buffer, 0, Buffer, 0, Buffer.Length);
                        client.Send(Buffer);
                        buffer = new byte[500];
                        Writer.WriteUInt16(2051, 2, buffer);
                    }
                }
                else
                {
                    Writer.WriteUInt16((ushort)(12 + buffer[10] * 112 + 6 + 2), 0, buffer);
                    byte[] Buffer = new byte[12 + buffer[10] * 112 + 6 + 2 + 8];
                    System.Buffer.BlockCopy(buffer, 0, Buffer, 0, Buffer.Length);
                    if (buffer[10] != 0 || buffer[10] == 0 && total == 0)
                        client.Send(Buffer);
                    break;
                }
            }
        }
        #endregion
        #region Booth
        static void ShowBoothItems(ItemUsage usage, Client.GameState client)
        {
            Client.GameState Owner = null;
            Game.Entity entity = null;
            if (client.Screen.TryGetValue((uint)((usage.UID - (usage.UID % 100000)) * 10 + (usage.UID % 100000)), out entity))
            {
                Owner = entity.Owner;
                if (Owner != null)
                {
                    if (Owner.Entity.UID != client.Entity.UID)
                    {
                        BoothItem Item = new BoothItem(true);
                        if (Owner.Booth != null)
                        {
                            foreach (Game.ConquerStructures.BoothItem item in Owner.Booth.ItemList.Values)
                            {
                                Item.Fill(item, Owner.Booth.Base.UID);
                                client.Send(Item);
                            }
                        }
                    }
                }
            }
        }
        static void AddItemOnBooth(ItemUsage usage, Client.GameState client)
        {
            if (client.Booth != null)
            {
                if (!client.Booth.ItemList.ContainsKey(usage.UID))
                {
                    if (client.Inventory.ContainsUID(usage.UID))
                    {
                        Game.ConquerStructures.BoothItem item = new Conquer_Online_Server.Game.ConquerStructures.BoothItem();
                        item.Cost = usage.dwParam;
                        client.Inventory.TryGetItem(usage.UID, out item.Item);
                        Database.ConquerItemInformation infos = new Database.ConquerItemInformation(item.Item.ID, 0);
                        if (item.Item.Lock != 0 || item.Item.Suspicious || item.Item.Bound || infos.BaseInformation.Type != Database.ConquerItemBaseInformation.ItemType.Dropable)
                        {
                            return;
                        }
                        item.Cost_Type = usage.ID == ItemUsage.AddItemOnBoothForSilvers ? Conquer_Online_Server.Game.ConquerStructures.BoothItem.CostType.Silvers : Conquer_Online_Server.Game.ConquerStructures.BoothItem.CostType.ConquerPoints;
                        client.Booth.ItemList.Add(item.Item.UID, item);
                        client.Send(usage);
                        BoothItem Item = new BoothItem(true);
                        Item.Fill(item, client.Booth.Base.UID);
                        client.SendScreen(Item, false);
                    }
                }
            }
        }
        static void BuyFromBooth(ItemUsage usage, Client.GameState client)
        {
            Client.GameState Owner = null;
            Game.Entity entity = null;
            if (client.Screen.TryGetValue((uint)((usage.dwParam - (usage.dwParam % 100000)) * 10 + (usage.dwParam % 100000)), out entity))
            {
                Owner = entity.Owner;
                if (Owner != null)
                {
                    if (Owner.Entity.UID != client.Entity.UID)
                    {
                        if (Owner.Booth.ItemList.ContainsKey(usage.UID))
                        {
                            Game.ConquerStructures.BoothItem item = Owner.Booth.ItemList[usage.UID];
                            if (client.Inventory.Count <= 39)
                            {
                                if (item.Cost_Type == Conquer_Online_Server.Game.ConquerStructures.BoothItem.CostType.Silvers)
                                {
                                    if (client.Entity.Money >= item.Cost)
                                    {
                                        client.Entity.Money -= item.Cost;
                                        Owner.Entity.Money += item.Cost;
                                        client.Send(usage);
                                        client.Inventory.Add(item.Item, Game.Enums.ItemUse.Move);
                                        usage.ID = ItemUsage.RemoveItemFromBooth;
                                        Owner.Send(usage);
                                        Owner.Inventory.Remove(item.Item.UID, Game.Enums.ItemUse.None, false);
                                        usage.ID = ItemUsage.RemoveInventory;
                                        Owner.Send(usage);
                                        Owner.Booth.ItemList.Remove(item.Item.UID);
                                        Database.ConquerItemInformation infos = new Database.ConquerItemInformation(item.Item.ID, 0);
                                        Owner.Send(ServerBase.Constants.BoothItemSell(client.Entity.Name, infos.BaseInformation.Name, false, item.Cost));
                                    }
                                }
                                else
                                {
                                    if (client.Entity.ConquerPoints >= item.Cost)
                                    {
                                        client.Entity.ConquerPoints -= item.Cost;
                                        Owner.Entity.ConquerPoints += item.Cost;
                                        client.Send(usage);
                                        client.Inventory.Add(item.Item, Game.Enums.ItemUse.Move);
                                        usage.ID = ItemUsage.RemoveItemFromBooth;
                                        Owner.Send(usage);
                                        Owner.Inventory.Remove(item.Item.UID, Game.Enums.ItemUse.None, false);
                                        usage.ID = ItemUsage.RemoveInventory;
                                        Owner.Send(usage);
                                        Owner.Booth.ItemList.Remove(item.Item.UID);
                                        Database.ConquerItemInformation infos = new Database.ConquerItemInformation(item.Item.ID, 0);
                                        Owner.Send(ServerBase.Constants.BoothItemSell(client.Entity.Name, infos.BaseInformation.Name, true, item.Cost));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        static void RemoveItemFromBooth(ItemUsage usage, Client.GameState client)
        {
            if (client.Booth.ItemList.ContainsKey(usage.UID))
            {
                client.Booth.ItemList.Remove(usage.UID);
                client.SendScreen(usage, true);
            }
        }

        #endregion
        #region Mentor Apprentice
        static void ExpelApprentice(MentorApprentice ma, Client.GameState client)
        {
            if (client.Apprentices.ContainsKey(ma.dwParam))
            {
                var appr = client.Apprentices[ma.dwParam];
                if (appr.IsOnline)
                {
                    ma.Type = MentorApprentice.DumpMentor;
                    ma.Online = false;
                    appr.Client.Send(ma);
                    appr.Client.Mentor = null;
                    appr.Client.ReviewMentor();
                }
                Database.KnownPersons.RemoveMentor(appr.ID);
                client.Apprentices.Remove(appr.ID);
            }
        }
        static void LeaveMentor(MentorApprentice ma, Client.GameState client)
        {
            if (client.Mentor != null)
            {
                if (client.Mentor.IsOnline)
                {
                    ma.Type = MentorApprentice.DumpMentor;
                    client.Send(ma);
                    ma.Type = MentorApprentice.DumpApprentice;
                    client.Mentor.Client.Send(ma);
                    ApprenticeInformation AppInfo = new ApprenticeInformation();
                    AppInfo.Apprentice_ID = client.Entity.UID;
                    AppInfo.Apprentice_Level = client.Entity.Level;
                    AppInfo.Apprentice_Class = client.Entity.Class;
                    AppInfo.Apprentice_PkPoints = client.Entity.PKPoints;
                    AppInfo.Apprentice_Name = client.Entity.Name;
                    AppInfo.Apprentice_Online = false;
                    AppInfo.Apprentice_Spouse_Name = client.Entity.Spouse;
                    AppInfo.Enrole_date = 0;
                    AppInfo.Mentor_ID = client.Mentor.Client.Entity.UID;
                    AppInfo.Mentor_Mesh = client.Mentor.Client.Entity.Mesh;
                    AppInfo.Mentor_Name = client.Mentor.Client.Entity.Name;
                    AppInfo.Type = 2;
                    client.Mentor.Client.Send(AppInfo);
                    client.Mentor.Client.Apprentices.Remove(client.Entity.UID);
                    client.Mentor = null;
                    client.ReviewMentor();
                }
                Database.KnownPersons.RemoveMentor(client.Entity.UID);
            }
        }
        static void AddMentor(MentorApprentice ma, Client.GameState client)
        {
            Client.GameState Target = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(ma.dwParam, out Target))
            {
                if (!client.Screen.Contains(Target.Entity.UID)) return;

                MentorApprentice Mentor = new MentorApprentice(true);
                Mentor.Type = MentorApprentice.AcceptRequestMentor;
                Mentor.dwParam = Target.Entity.UID;
                Mentor.UID = client.Entity.UID;
                Mentor.Dynamic = (byte)client.Entity.BattlePower;
                Mentor.Online = true;
                Mentor.Name = client.Entity.Name;

                Target.Send(Mentor);
            }
        }
        static void AddApprentice(MentorApprentice ma, Client.GameState client)
        {
            Client.GameState Target = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(ma.dwParam, out Target))
            {
                if (!client.Screen.Contains(Target.Entity.UID)) return;

                MentorApprentice Mentor = new MentorApprentice(true);
                Mentor.Type = MentorApprentice.AcceptRequestApprentice;
                Mentor.dwParam = Target.Entity.UID;
                Mentor.UID = client.Entity.UID;
                Mentor.Dynamic = (byte)client.Entity.BattlePower;
                Mentor.Online = true;
                Mentor.Name = client.Entity.Name;

                Target.Send(Mentor);
            }
        }
        static void AcceptRequestMentor(MentorApprentice ma, Client.GameState client)
        {
            Client.GameState Target = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(ma.UID, out Target))
            {
                if (ma.Dynamic == 1)
                {
                    uint EnroleDate = (uint)(DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day);

                    Target.Mentor = new Conquer_Online_Server.Game.ConquerStructures.Society.Mentor();
                    Target.Mentor.ID = client.Entity.UID;
                    Target.Mentor.Name = client.Entity.Name;
                    Target.Mentor.EnroleDate = EnroleDate;

                    MentorInformation Information = new MentorInformation(true);
                    Information.Mentor_Type = 1;
                    Information.Mentor_ID = Target.Mentor.Client.Entity.UID;
                    Information.Apprentice_ID = Target.Entity.UID;
                    Information.Enrole_Date = EnroleDate;
                    Information.Mentor_Level = Target.Mentor.Client.Entity.Level;
                    Information.Mentor_Class = Target.Mentor.Client.Entity.Class;
                    Information.Mentor_PkPoints = Target.Mentor.Client.Entity.PKPoints;
                    Information.Mentor_Mesh = Target.Mentor.Client.Entity.Mesh;
                    Information.Mentor_Online = true;
                    Information.Shared_Battle_Power = (uint)(((Target.Mentor.Client.Entity.BattlePower - Target.Mentor.Client.Entity.ExtraBattlePower) - (Target.Entity.BattlePower - Target.Entity.ExtraBattlePower)) / 3.3F);
                    Information.String_Count = 3;
                    Information.Mentor_Name = Target.Mentor.Client.Entity.Name;
                    Information.Apprentice_Name = Target.Entity.Name;
                    Information.Mentor_Spouse_Name = Target.Mentor.Client.Entity.Spouse;

                    Target.Send(Information);
                    Target.ReviewMentor();
                    ApprenticeInformation AppInfo = new ApprenticeInformation();
                    AppInfo.Apprentice_ID = Target.Entity.UID;
                    AppInfo.Apprentice_Level = Target.Entity.Level;
                    AppInfo.Apprentice_Name = Target.Entity.Name;
                    AppInfo.Apprentice_Class = Target.Entity.Class;
                    AppInfo.Apprentice_PkPoints = Target.Entity.PKPoints;
                    AppInfo.Apprentice_Online = true;
                    AppInfo.Apprentice_Spouse_Name = Target.Entity.Spouse;
                    AppInfo.Enrole_date = EnroleDate;
                    AppInfo.Mentor_ID = client.Entity.UID;
                    AppInfo.Mentor_Mesh = client.Entity.Mesh;
                    AppInfo.Mentor_Name = client.Entity.Name;
                    AppInfo.Type = 2;
                    client.Send(AppInfo);
                    client.Apprentices.Add(Target.Entity.UID, new Conquer_Online_Server.Game.ConquerStructures.Society.Apprentice()
                    {
                        ID = Target.Entity.UID,
                        Name = Target.Entity.Name,
                        EnroleDate = EnroleDate
                    });
                    Database.KnownPersons.AddMentor(Target.Mentor, client.Apprentices[Target.Entity.UID]);
                }
                else
                {
                    Target.Send(new Message(client.Entity.Name + " declined your request.", System.Drawing.Color.Beige, Message.Talk));
                }
            }
        }
        static void AcceptRequestApprentice(MentorApprentice ma, Client.GameState client)
        {
            Client.GameState Target = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(ma.UID, out Target))
            {
                if (ma.Dynamic == 1)
                {
                    uint EnroleDate = (uint)(DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day);
                    MentorInformation Information = new MentorInformation(true);
                    Information.Mentor_Type = 1;
                    Information.Mentor_ID = Target.Entity.UID;
                    Information.Apprentice_ID = client.Entity.UID;
                    Information.Enrole_Date = EnroleDate;
                    Information.Mentor_Level = Target.Entity.Level;
                    Information.Mentor_Class = Target.Entity.Class;
                    Information.Mentor_PkPoints = Target.Entity.PKPoints;
                    Information.Mentor_Mesh = Target.Entity.Mesh;
                    Information.Mentor_Online = true;
                    Information.Shared_Battle_Power = ((uint)(((Target.Entity.BattlePower - Target.Entity.ExtraBattlePower) - (client.Entity.BattlePower - client.Entity.ExtraBattlePower)) / 3.3F));
                    Information.String_Count = 3;
                    Information.Mentor_Name = Target.Entity.Name;
                    Information.Apprentice_Name = client.Entity.Name;
                    Information.Mentor_Spouse_Name = Target.Entity.Spouse;

                    client.Send(Information);
                    client.Mentor = new Conquer_Online_Server.Game.ConquerStructures.Society.Mentor();
                    client.Mentor.ID = Target.Entity.UID;
                    client.Mentor.Name = Target.Entity.Name;
                    client.Mentor.EnroleDate = EnroleDate;
                    client.ReviewMentor();

                    ApprenticeInformation AppInfo = new ApprenticeInformation();
                    AppInfo.Apprentice_ID = client.Entity.UID;
                    AppInfo.Apprentice_Level = client.Entity.Level;
                    AppInfo.Apprentice_Name = client.Entity.Name;
                    AppInfo.Apprentice_Online = true;
                    AppInfo.Apprentice_Class = client.Entity.Class;
                    AppInfo.Apprentice_PkPoints = client.Entity.PKPoints;
                    AppInfo.Apprentice_Spouse_Name = client.Entity.Spouse;
                    AppInfo.Enrole_date = EnroleDate;
                    AppInfo.Mentor_ID = Target.Entity.UID;
                    AppInfo.Mentor_Mesh = Target.Entity.Mesh;
                    AppInfo.Mentor_Name = Target.Entity.Name;
                    AppInfo.Type = 2;
                    Target.Send(AppInfo);
                    Target.Apprentices.Add(client.Entity.UID, new Conquer_Online_Server.Game.ConquerStructures.Society.Apprentice()
                    {
                        ID = client.Entity.UID,
                        Name = client.Entity.Name,
                        EnroleDate = EnroleDate
                    });
                    Database.KnownPersons.AddMentor(client.Mentor, Target.Apprentices[client.Entity.UID]);
                }
                else
                {
                    Target.Send(new Message(client.Entity.Name + " declined your request.", System.Drawing.Color.Beige, Message.Talk));
                }
            }
        }
        #endregion
        #region TradePartners
        static void RequestTradePartnership(TradePartner partner, Client.GameState client)
        {
            if (client.Screen.Contains(partner.UID))
            {
                var Client = ServerBase.Kernel.GamePool[partner.UID];
                if (Client != null)
                {
                    if (Client.TradePartnerRequest != client.Entity.UID)
                    {
                        client.TradePartnerRequest = Client.Entity.UID;
                        partner.UID = client.Entity.UID;
                        partner.Name = client.Entity.Name;
                        Client.Send(partner);
                    }
                    else
                    {
                        DateTime Now = DateTime.Now;
                        client.Partners.Add(Client.Entity.UID, new Conquer_Online_Server.Game.ConquerStructures.Society.TradePartner()
                        {
                            ID = Client.Entity.UID,
                            Name = Client.Entity.Name,
                            ProbationStartedOn = Now
                        });
                        Client.Partners.Add(client.Entity.UID, new Conquer_Online_Server.Game.ConquerStructures.Society.TradePartner()
                        {
                            ID = Client.Entity.UID,
                            Name = Client.Entity.Name,
                            ProbationStartedOn = Now
                        });
                        client.Send(new TradePartner(true)
                        {
                            UID = Client.Entity.UID,
                            Type = TradePartner.AddPartner,
                            Name = Client.Entity.Name,
                            HoursLeft = (int)(new TimeSpan(Now.AddDays(3).Ticks).TotalHours - new TimeSpan(Now.Ticks).TotalHours),
                            Online = true
                        });
                        Client.Send(new TradePartner(true)
                        {
                            UID = client.Entity.UID,
                            Type = TradePartner.AddPartner,
                            Name = client.Entity.Name,
                            HoursLeft = (int)(new TimeSpan(Now.AddDays(3).Ticks).TotalHours - new TimeSpan(Now.Ticks).TotalHours),
                            Online = true
                        });
                        Database.KnownPersons.AddPartner(client, client.Partners[Client.Entity.UID]);
                        client.SendScreen(new Message(Client.Entity.Name + " has begun " + client.Entity.Name + " a partnership probation for three days!", System.Drawing.Color.Blue, Message.TopLeft), true);
                    }
                }
            }
        }
        static void RejectPartnership(TradePartner partner, Client.GameState client)
        {
            client.TradePartnerRequest = 0;
            var Client = ServerBase.Kernel.GamePool[partner.UID];
            if (Client != null)
            {
                Client.Send(new TradePartner(true)
                {
                    UID = client.Entity.UID,
                    Type = TradePartner.RejectRequest,
                    Name = client.Entity.Name,
                    Online = true
                });
            }
        }
        static void BreakPartnership(TradePartner partner, Client.GameState client)
        {
            if (client.Partners.ContainsKey(partner.UID))
            {
                var Client = ServerBase.Kernel.GamePool[partner.UID];
                if (Client != null)
                {
                    Client.Partners.Remove(client.Entity.UID);
                    Client.Send(new TradePartner(true)
                    {
                        UID = client.Entity.UID,
                        Type = TradePartner.BreakPartnership,
                        Name = "",
                        Online = false
                    });
                    Client.Send(new Message(client.Entity.Name + " has broken the partnership with you.", System.Drawing.Color.Blue, Message.TopLeft));
                }
                client.Send(new TradePartner(true)
                {
                    UID = partner.UID,
                    Type = TradePartner.BreakPartnership,
                    Name = "",
                    Online = false
                });
                client.Partners.Remove(partner.UID);
                Database.KnownPersons.RemovePartner(client, partner.UID);
                client.Send(new Message("You have broken the partnership with " + partner.Name + ".", System.Drawing.Color.Blue, Message.TopLeft));
            }
        }
        #endregion
        #region KnownPersons
        static void RemoveFriend(KnownPersons knownperson, Client.GameState client)
        {
            if (client.Friends.ContainsKey(knownperson.UID))
            {
                Game.ConquerStructures.Society.Friend friend = client.Friends[knownperson.UID];
                if (friend.IsOnline)
                {
                    friend.Client.Friends.Remove(client.Entity.UID);
                    friend.Client.Send(new KnownPersons(true)
                    {
                        UID = client.Entity.UID,
                        Type = KnownPersons.RemovePerson,
                        Name = "",
                        Online = false
                    });
                }
                client.Friends.Remove(friend.ID);
                client.Send(new KnownPersons(true)
                {
                    UID = friend.ID,
                    Type = KnownPersons.RemovePerson,
                    Name = "",
                    Online = false
                });
                Database.KnownPersons.RemoveFriend(client, friend.ID);
            }
        }
        static void RemoveEnemy(KnownPersons knownperson, Client.GameState client)
        {
            if (client.Enemy.ContainsKey(knownperson.UID))
            {
                Game.ConquerStructures.Society.Enemy enemy = client.Enemy[knownperson.UID];

                client.Enemy.Remove(enemy.ID);
                client.Send(new KnownPersons(true)
                {
                    UID = enemy.ID,
                    Type = KnownPersons.RemovePerson,
                    Name = "",
                    Online = false
                });
                Database.KnownPersons.RemoveEnemy(client, enemy.ID);
            }
        }
        static void AddFriend(KnownPersons knownperson, Client.GameState client)
        {
            if (!client.Friends.ContainsKey(knownperson.UID))
            {
                Client.GameState Client = ServerBase.Kernel.GamePool[knownperson.UID];
                if (Client != null)
                {
                    Client.OnMessageBoxOK = delegate
                    {
                        if (client != null)
                        {
                            if (client.Socket.Connected)
                            {
                                if (!Client.Friends.ContainsKey(client.Entity.UID))
                                {
                                    client.Friends.Add(Client.Entity.UID, new Conquer_Online_Server.Game.ConquerStructures.Society.Friend()
                                    {
                                        ID = Client.Entity.UID,
                                        Name = Client.Entity.Name
                                    });
                                    Client.Friends.Add(client.Entity.UID, new Conquer_Online_Server.Game.ConquerStructures.Society.Friend()
                                    {
                                        ID = client.Entity.UID,
                                        Name = client.Entity.Name
                                    });
                                    client.Send(new KnownPersons(true)
                                    {
                                        UID = Client.Entity.UID,
                                        Type = KnownPersons.AddFriend,
                                        Name = Client.Entity.Name,
                                        Online = true
                                    });
                                    Client.Send(new KnownPersons(true)
                                    {
                                        UID = client.Entity.UID,
                                        Type = KnownPersons.AddFriend,
                                        Name = client.Entity.Name,
                                        Online = true
                                    });
                                    Database.KnownPersons.AddFriend(client, client.Friends[Client.Entity.UID]);
                                    client.SendScreen(new Message(Client.Entity.Name + " has accepted " + client.Entity.Name + "'s friendship request.", System.Drawing.Color.Blue, Message.TopLeft), true);
                                }
                            }
                        }
                    };
                    Client.OnMessageBoxCANCEL = delegate
                    {
                        if (client != null)
                        {
                            if (client.Socket.Connected)
                            {
                                if (Client != null)
                                {
                                    if (Client.Socket.Connected)
                                    {
                                        client.Send(new Message(Client.Entity.Name + " has rejected your friendship request.", System.Drawing.Color.Blue, Message.TopLeft));
                                    }
                                }
                            }
                        }
                    };
                    Client.Send(new NpcReply(NpcReply.MessageBox, client.Entity.Name + " wants to be your friend."));
                }
            }
        }

        public static void AddEnemy(Client.GameState client, Client.GameState enemy)
        {
            if (!client.Enemy.ContainsKey(enemy.Entity.UID))
            {
                client.Enemy.Add(enemy.Entity.UID, new Conquer_Online_Server.Game.ConquerStructures.Society.Enemy()
                {
                    ID = enemy.Entity.UID,
                    Name = enemy.Entity.Name
                });
                client.Send(new KnownPersons(true)
                {
                    UID = enemy.Entity.UID,
                    Type = KnownPersons.AddEnemy,
                    Name = enemy.Entity.Name,
                    Online = true
                });
                Database.KnownPersons.AddEnemy(client, client.Enemy[enemy.Entity.UID]);
            }
        }

        #endregion
        #region Attack
        public static void Attack(Attack attack, Client.GameState client)
        {
            client.Entity.AttackPacket = attack;
            new Game.Attacking.Handle(attack, client.Entity, null);
        }
        #endregion
        #region Trade
        static void RequestTrade(Trade trade, Client.GameState client)
        {
            Client.GameState _client = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(trade.dwParam, out _client))
            {
                if (_client.Trade.InTrade || client.Trade.InTrade || client.Entity.UID == trade.dwParam || ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, _client.Entity.X, _client.Entity.Y) > ServerBase.Constants.pScreenDistance)
                {
                    client.Send(ServerBase.Constants.TradeInProgress);
                    return;
                }
                client.Trade.TraderUID = _client.Entity.UID;
                if (_client.Trade.TraderUID == client.Entity.UID)
                {
                    _client.Trade.InTrade = client.Trade.InTrade = true;
                    trade.Type = Trade.ShowTable;
                    client.Send(trade);
                    trade.dwParam = client.Entity.UID;
                    _client.Send(trade);
                }
                else
                {

                    client.Send(ServerBase.Constants.TradeRequest);
                    trade.dwParam = client.Entity.UID;
                    PopupLevelBP request = new PopupLevelBP();
                    request.Requester = client.Entity.UID;
                    request.Receiver = _client.Entity.UID;
                    request.Level = client.Entity.Level;
                    request.BattlePower = (uint)client.Entity.BattlePower;
                    _client.Send(request);
                    _client.Send(trade);
                    _client.Send(request);

                }
            }
        }
        static void CloseTrade(Trade trade, Client.GameState client)
        {
            Client.GameState _client = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(client.Trade.TraderUID, out _client))
            {
                _client.Trade = new Game.ConquerStructures.Trade();
                client.Trade = new Game.ConquerStructures.Trade();
                trade.Type = Trade.HideTable;
                trade.dwParam = _client.Entity.UID;
                client.Send(trade);
                trade.dwParam = client.Entity.UID;
                _client.Send(trade);
            }
        }
        static void AddTradeItem(Trade trade, Client.GameState client)
        {
            Interfaces.IConquerItem item = null;
            if (client.Inventory.TryGetItem(trade.dwParam, out item))
            {
                Client.GameState _client = null;
                if (ServerBase.Kernel.GamePool.TryGetValue(client.Trade.TraderUID, out _client))
                {
                    Database.ConquerItemInformation infos = new Database.ConquerItemInformation(item.ID, 0);
                    if (infos.BaseInformation.Type != Database.ConquerItemBaseInformation.ItemType.Dropable || (item.Lock != 0 && !client.Partners.ContainsKey(_client.Entity.UID) && !client.Partners[_client.Entity.UID].StillOnProbation) || item.Bound || item.Suspicious)
                    {
                        trade.Type = Trade.RemoveItem;
                        client.Send(trade);
                        return;
                    }
                    if (_client.Inventory.Count + client.Trade.Items.Count >= 40 || client.Trade.Items.Count == 20)
                    {
                        trade.Type = Trade.RemoveItem;
                        client.Send(trade);
                        client.Send(ServerBase.Constants.TradeInventoryFull);
                        return;
                    }
                    client.Trade.Items.Add(item);
                    item.Mode = Game.Enums.ItemMode.Trade;
                    item.Send(_client);
                }
            }
        }
        static void SetTradeMoney(Trade trade, Client.GameState client)
        {
            Client.GameState _client = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(client.Trade.TraderUID, out _client))
            {
                if (client.Entity.Money >= trade.dwParam)
                {
                    client.Trade.Money = trade.dwParam;
                    trade.Type = Trade.ShowMoney;
                    _client.Send(trade);
                }
                else
                    CloseTrade(trade, client);
            }
        }
        static void SetTradeConquerPoints(Trade trade, Client.GameState client)
        {
            Client.GameState _client = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(client.Trade.TraderUID, out _client))
            {
                if (client.Entity.ConquerPoints >= trade.dwParam)
                {
                    client.Trade.ConquerPoints = trade.dwParam;
                    trade.Type = Trade.ShowConquerPoints;
                    _client.Send(trade);
                }
                else
                    CloseTrade(trade, client);
            }
        }
        static void AcceptTrade(Trade trade, Client.GameState client)
        {
            Client.GameState _client = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(client.Trade.TraderUID, out _client))
            {
                client.Trade.Accepted = true;
                _client.Send(trade);
                if (_client.Trade.Accepted)
                {
                    if (client.Inventory.Count + _client.Trade.Items.Count <= 40)
                    {
                        foreach (Interfaces.IConquerItem item in _client.Trade.Items)
                        {
                            if (_client.Inventory.ContainsUID(item.UID))
                            {
                                client.Inventory.Add(item, Game.Enums.ItemUse.Move);
                                _client.Inventory.Remove(item.UID, Game.Enums.ItemUse.None, true);
                            }
                        }
                    }
                    if (_client.Inventory.Count + client.Trade.Items.Count <= 40)
                    {
                        foreach (Interfaces.IConquerItem item in client.Trade.Items)
                        {
                            if (client.Inventory.ContainsUID(item.UID))
                            {
                                _client.Inventory.Add(item, Game.Enums.ItemUse.Move);
                                client.Inventory.Remove(item.UID, Game.Enums.ItemUse.None, true);
                            }
                        }
                    }

                    if (_client.Trade.Money <= _client.Entity.Money)
                    {
                        _client.Entity.Money -= _client.Trade.Money;
                        client.Entity.Money += _client.Trade.Money;
                    }
                    if (client.Trade.Money <= client.Entity.Money)
                    {
                        client.Entity.Money -= client.Trade.Money;
                        _client.Entity.Money += client.Trade.Money;
                    }
                    if (_client.Trade.ConquerPoints <= _client.Entity.ConquerPoints)
                    {
                        _client.Entity.ConquerPoints -= _client.Trade.ConquerPoints;
                        client.Entity.ConquerPoints += _client.Trade.ConquerPoints;
                    }
                    if (client.Trade.ConquerPoints <= client.Entity.ConquerPoints)
                    {
                        client.Entity.ConquerPoints -= client.Trade.ConquerPoints;
                        _client.Entity.ConquerPoints += client.Trade.ConquerPoints;
                    }

                    trade.Type = Trade.HideTable;
                    trade.dwParam = client.Entity.UID;
                    client.Send(trade);
                    trade.dwParam = _client.Entity.UID;
                    _client.Send(trade);

                    _client.Trade = new Game.ConquerStructures.Trade();
                    client.Trade = new Game.ConquerStructures.Trade();
                    client.Entity.Money = (uint)client.Entity.Money;
                    _client.Entity.Money = (uint)_client.Entity.Money;
                    client.Entity.ConquerPoints = (uint)client.Entity.ConquerPoints;
                    _client.Entity.ConquerPoints = (uint)_client.Entity.ConquerPoints;
                }
            }
        }
        #endregion
        #region ItemHandler
        public static void StabilazeArtifact(ItemAddingStabilization stabilizate, Client.GameState client)
        {
            Interfaces.IConquerItem Item = null;
            if (client.Inventory.TryGetItem(stabilizate.ItemUID, out Item))
            {
                if (Item.Purification.PurificationDuration != 0)
                {
                    if (Item.Purification.PurificationItemID != 0)
                    {
                        List<uint> purificationStones = stabilizate.PurificationItems;
                        int sum = 0;
                        List<Interfaces.IConquerItem> PurificationStones = new List<Conquer_Online_Server.Interfaces.IConquerItem>(purificationStones.Count);

                        for (int i = 0; i < purificationStones.Count; i++)
                        {
                            Interfaces.IConquerItem pItem = null;
                            if (client.Inventory.TryGetItem(purificationStones[i], out pItem))
                            {
                                if (pItem.ID == 723694)
                                {
                                    sum += 10;
                                    PurificationStones.Add(pItem);
                                }
                                if (pItem.ID == 723695)
                                {
                                    sum += 100;
                                    PurificationStones.Add(pItem);
                                }
                            }
                        }
                        if (sum >= Database.DataHolder.PurifyStabilizationPoints((byte)Item.Purification.PurificationLevel))
                        {
                            var Backup = Item.Purification;
                            Backup.PurificationDuration = 0;
                            Item.Purification = Backup;
                            Item.Send(client);
                            Database.ItemAddingTable.Stabilize(Item.UID, Backup.PurificationItemID);
                            foreach (var item in PurificationStones)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        }
                    }
                }
            }
        }

        public static void PurifyItem(Purification ps, Client.GameState client)
        {
            Interfaces.IConquerItem Item = null, AddingItem = null;
            if (client.Inventory.TryGetItem(ps.ItemUID, out Item) && client.Inventory.TryGetItem(ps.AddUID, out AddingItem))
            {
                if (ps.ItemUID == ps.AddUID)
                    return;

                var PurifyInformation = Database.ConquerItemInformation.BaseInformations[AddingItem.ID];
                var ItemInformation = Database.ConquerItemInformation.BaseInformations[Item.ID];
               
                if (PurifyInformation.PurificationLevel > 0)
                {
                    if (ItemInformation.Level >= PurifyInformation.Level)
                    {
                        if (client.Inventory.Contains(1088001, PurifyInformation.PurificationMeteorNeed))
                        {
                            client.Inventory.Remove(1088001, (byte)PurifyInformation.PurificationMeteorNeed);
                            if (Item.Purification.PurificationItemID > 0)
                                Database.ItemAddingTable.RemoveAdding(Item.UID, Item.Purification.PurificationItemID);
                            ps.Mode = Purification.Stabilaze;
                            client.Send(ps);
                            ItemAdding.Purification_ purify = new ItemAdding.Purification_();
                            purify.AddedOn = DateTime.Now;
                            purify.Available = true;
                            purify.ItemUID = ps.ItemUID;
                            purify.PurificationLevel = PurifyInformation.PurificationLevel;
                            purify.PurificationDuration = 7 * 24 * 60 * 60;
                            purify.PurificationItemID = AddingItem.ID;
                            Database.ItemAddingTable.AddPurification(purify);
                            Item.Purification = purify;
                            Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            Item.Send(client);
                            client.Inventory.Remove(AddingItem, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                            client.Send(ps);
                        }
                    }
                }
            }
        }
        public static bool IsEquipment(long ID)
        {
            return ItemPosition((uint)ID) != 0;
        }
        public static void ReloadArrows(Interfaces.IConquerItem arrow, Client.GameState client)
        {
            if (client.Entity.Class < 40 || client.Entity.Class > 45)
                return;
            if (client.Equipment.Free(4))
                return;
            if (client.Equipment.TryGetItem(4).ID / 1000 != 500)
                return;
            client.Equipment.DestroyArrow(5);
            uint id = 1050002;
            if (arrow != null)
                id = arrow.ID;
            if (client.Inventory.Contains(id, 1))
            {
                var newArrow = client.Inventory.GetItemByID(id);
                newArrow.Position = 5;
                client.Inventory.Remove(newArrow, Conquer_Online_Server.Game.Enums.ItemUse.Move);
                Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, 0);

                client.Equipment.Add(newArrow);
                client.Send(ServerBase.Constants.ArrowsReloaded);
            }
            else if (!client.Inventory.Contains(id, 1))
            {
                client.Send(ServerBase.Constants.NoArrows(Database.ConquerItemInformation.BaseInformations[id].Name));
            }
        }
        static void ComposePlus(Compose compose, Client.GameState client)
        {
            Interfaces.IConquerItem Item = null, ItemPlus = null;
            if (client.Inventory.TryGetItem(compose.ItemUID, out Item) && client.Inventory.TryGetItem(compose.PlusItemUID, out ItemPlus))
            {
                if (compose.ItemUID == compose.PlusItemUID)
                    return;
                if (ItemPlus.Bound) return;
                if (ItemPlus.Suspicious) return;
                switch (compose.Mode)
                {
                    case Compose.CurrentSteed:
                    case Compose.Plus:
                        {
                            if (Item.Plus < 12)
                            {
                                Item.PlusProgress += Database.DataHolder.StonePlusPoints(ItemPlus.Plus);
                                while (Item.PlusProgress >= Database.DataHolder.ComposePlusPoints(Item.Plus) && Item.Plus != 12)
                                {
                                    Item.PlusProgress -= Database.DataHolder.ComposePlusPoints(Item.Plus);
                                    Item.Plus++;
                                    if (Item.Plus == 12)
                                        Item.PlusProgress = 0;
                                }
                                Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                Item.Send(client);
                                Database.ConquerItemTable.UpdatePlus(Item, client);
                                Database.ConquerItemTable.UpdatePlusProgress(Item, client);
                                client.Inventory.Remove(ItemPlus, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                            }
                            break;
                        }
                    case Compose.NewSteed:
                        {
                            if (Item.Plus < 12)
                            {
                                Item.PlusProgress += Database.DataHolder.StonePlusPoints(ItemPlus.Plus);
                                while (Item.PlusProgress >= Database.DataHolder.ComposePlusPoints(Item.Plus) && Item.Plus != 12)
                                {
                                    Item.PlusProgress -= Database.DataHolder.ComposePlusPoints(Item.Plus);
                                    Item.Plus++;
                                    if (Item.Plus == 12)
                                        Item.PlusProgress = 0;
                                }
                                Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                Item.Send(client);
                                Database.ConquerItemTable.UpdatePlus(Item, client);
                                client.Inventory.Remove(ItemPlus, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                            }
                            int color1 = (int)Item.SocketProgress;
                            int color2 = (int)ItemPlus.SocketProgress;
                            int B1 = color1 & 0xFF;
                            int B2 = color2 & 0xFF;
                            int G1 = (color1 >> 8) & 0xFF;
                            int G2 = (color2 >> 8) & 0xFF;
                            int R1 = (color1 >> 16) & 0xFF;
                            int R2 = (color2 >> 16) & 0xFF;
                            int newB = (int)Math.Floor(0.9 * B1) + (int)Math.Floor(0.1 * B2);
                            int newG = (int)Math.Floor(0.9 * G1) + (int)Math.Floor(0.1 * G2);
                            int newR = (int)Math.Floor(0.9 * R1) + (int)Math.Floor(0.1 * R2);
                            Item.SocketProgress = (uint)(newB | (newG << 8) | (newR << 16));
                            Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            Item.Send(client);
                            Database.ConquerItemTable.UpdateSocketProgress(Item, client);
                            client.Inventory.Remove(ItemPlus, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                            break;
                        }
                }
            }
        }

        static void SocketItem(EmbedSocket socket, Client.GameState client)
        {
            Interfaces.IConquerItem Item = null;
            Interfaces.IConquerItem Gem = null;
            if (client.Inventory.TryGetItem(socket.ItemUID, out Item))
            {
                if (client.Inventory.TryGetItem(socket.GemUID, out Gem) || socket.Mode == EmbedSocket.Remove)
                {
                    switch (socket.Mode)
                    {
                        case EmbedSocket.Add:
                            {
                                byte gemBase = (byte)(Gem.ID % 1000);
                                if (Enum.IsDefined(typeof(Game.Enums.Gem), gemBase))
                                {
                                    switch (socket.Slot)
                                    {
                                        case EmbedSocket.SlotOne:
                                            {
                                                if ((byte)Item.SocketOne == 255)
                                                {
                                                    Item.SocketOne = (Conquer_Online_Server.Game.Enums.Gem)(Gem.ID % 1000);
                                                    Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                    Item.Send(client);
                                                    Database.ConquerItemTable.UpdateSockets(Item, client);
                                                    client.Inventory.Remove(Gem, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                                    break;
                                                }
                                                break;
                                            }
                                        case EmbedSocket.SlotTwo:
                                            {
                                                if ((byte)Item.SocketOne > 0 && (byte)Item.SocketOne < 255)
                                                {
                                                    if ((byte)Item.SocketTwo == 255)
                                                    {
                                                        Item.SocketTwo = (Conquer_Online_Server.Game.Enums.Gem)(Gem.ID % 1000);
                                                        Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                        Item.Send(client);
                                                        Database.ConquerItemTable.UpdateSockets(Item, client);
                                                        client.Inventory.Remove(Gem, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                                    }
                                                }
                                                else if ((byte)Item.SocketOne == 255)
                                                {
                                                    Item.SocketOne = (Conquer_Online_Server.Game.Enums.Gem)(Gem.ID % 1000);
                                                    Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                    Item.Send(client);
                                                    Database.ConquerItemTable.UpdateSockets(Item, client);
                                                    client.Inventory.Remove(Gem, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                                }
                                                break;
                                            }
                                    }
                                }
                                break;
                            }
                        case EmbedSocket.Remove:
                            {
                                switch (socket.Slot)
                                {
                                    case EmbedSocket.SlotOne:
                                        {
                                            if ((byte)Item.SocketOne != 0)
                                            {
                                                Item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                if ((byte)Item.SocketTwo != 0 && (byte)Item.SocketTwo != 255)
                                                {
                                                    Item.SocketOne = Item.SocketTwo;
                                                    Item.SocketTwo = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                }
                                                Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                Item.Send(client);
                                                Database.ConquerItemTable.UpdateSockets(Item, client);
                                                break;
                                            }
                                            break;
                                        }
                                    case EmbedSocket.SlotTwo:
                                        {
                                            if ((byte)Item.SocketTwo != 0)
                                            {
                                                Item.SocketTwo = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                Item.Send(client);
                                                Database.ConquerItemTable.UpdateSockets(Item, client);
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
                client.Send(socket);
            }
        }

        static void LockItem(ItemLock itemLock, Client.GameState client)
        {
            Interfaces.IConquerItem item = null;
            if (client.Inventory.TryGetItem(itemLock.UID, out item))
            {
                itemLock.ID = 1;
                item.Lock = 1;
                item.Mode = Game.Enums.ItemMode.Update;
                client.Send(itemLock);
                item.Send(client);
                Database.ConquerItemTable.UpdateLock(item, client);
            }
            else if (client.Equipment.TryGetItem(itemLock.UID) != null)
            {
                item = client.Equipment.TryGetItem(itemLock.UID);
                itemLock.ID = 1;
                item.Lock = 1;
                item.Mode = Game.Enums.ItemMode.Update;
                client.Send(itemLock);
                item.Send(client);
                Database.ConquerItemTable.UpdateLock(item, client);
            }
        }
        static void UnlockItem(ItemLock itemLock, Client.GameState client)
        {
            Interfaces.IConquerItem item = null;
            if (client.Inventory.TryGetItem(itemLock.UID, out item))
            {
                if (item.Lock == 1)
                {
                    item.Lock = 2;
                    item.UnlockEnd = DateTime.Now.AddDays(5);
                    item.Mode = Game.Enums.ItemMode.Update;
                    item.Send(client);
                    Database.ConquerItemTable.UpdateLock(item, client);
                }
                else
                    client.Send(new Message("Can't unlock an item that is in progress of unlocking.", System.Drawing.Color.FloralWhite, Message.TopLeft));
            }
        }

        static void SocketTalismanWithItem(ItemUsage itemUsage, Client.GameState client)
        {
            Interfaces.IConquerItem talisman = client.Equipment.TryGetItem(itemUsage.UID);
            Interfaces.IConquerItem item = null;
            if (client.Inventory.TryGetItem(itemUsage.dwParam, out item))
            {
                if (talisman == null)
                    return;
                if (item.ID / 1000 == talisman.ID / 1000)
                    return;
                if (item.Bound == true)
                    return;
                if (talisman.SocketTwo != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                    return;

                ushort Points = 0;
                switch (item.ID % 10)
                {
                    case 6: Points += 5; break;
                    case 7: Points += 10; break;
                    case 8: Points += 40; break;
                    case 9: Points += 1000; break;
                }
                Points += Database.DataHolder.TalismanPlusPoints(item.Plus);

                int position = ItemPosition(item.ID);
                switch (position)
                {
                    case 0: return;
                    case 4:
                    case 5:
                        if (item.ID % 10 >= 8)
                        {
                            if (item.SocketOne != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                Points += 160;
                            if (item.SocketTwo != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                Points += 800;
                        }
                        break;
                    default:
                        if (item.ID % 10 >= 8)
                        {
                            if (item.SocketOne != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                Points += 2000;
                            if (item.SocketTwo != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                Points += 6000;
                        }
                        break;
                }
                talisman.SocketProgress += Points;
                if (talisman.SocketOne == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                {
                    if (talisman.SocketProgress >= 8000)
                    {
                        talisman.SocketProgress -= 8000;
                        talisman.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;

                        talisman.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                        talisman.Send(client);
                    }
                }
                if (talisman.SocketOne != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                {
                    if (talisman.SocketProgress >= 20000)
                    {
                        talisman.SocketProgress = 0;
                        talisman.SocketTwo = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                    }
                }
                Database.ConquerItemTable.UpdateSocketProgress(talisman, client);
                Database.ConquerItemTable.UpdateSockets(talisman, client);
                talisman.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                talisman.Send(client);
                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
            }
        }
        static void SocketTalismanWithCPs(ItemUsage itemUsage, Client.GameState client)
        {
            Interfaces.IConquerItem talisman = client.Equipment.TryGetItem(itemUsage.UID);
            if (talisman == null)
                return;

            uint price = 0;
            if (talisman.SocketOne == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
            {
                double procent = 100 - (talisman.SocketProgress * 25600 / 2048000);
                if (100 - procent < 25)
                    return;
                price = (uint)(procent * 55);
            }
            else if (talisman.SocketTwo == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
            {
                double procent = 100 - (talisman.SocketProgress * 25600 / 5120000);
                if (100 - procent < 25)
                    return;
                price = (uint)(procent * 110);
            }
            else
                return;
            if (client.Entity.ConquerPoints >= price)
            {
                client.Entity.ConquerPoints = (uint)Math.Max(0, (int)((int)client.Entity.ConquerPoints - (int)price));
                if (talisman.SocketOne == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                    talisman.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                else if (talisman.SocketTwo == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                    talisman.SocketTwo = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                talisman.SocketProgress = 0;
                Database.ConquerItemTable.UpdateSockets(talisman, client);
                talisman.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                talisman.Send(client);
            }
        }

        static void EnchantItem(ItemUsage itemUsage, Client.GameState client)
        {
            Interfaces.IConquerItem Item = null, EnchantGem = null;
            if (client.Inventory.TryGetItem(itemUsage.UID, out Item) && client.Inventory.TryGetItem(itemUsage.dwParam, out EnchantGem))
            {
                byte gemBase = (byte)(EnchantGem.ID % 1000);
                if (Enum.IsDefined(typeof(Game.Enums.Gem), gemBase))
                {
                    byte Enchant = 0;
                    switch (EnchantGem.ID % 10)
                    {
                        case 1:
                            {
                                Enchant = (byte)ServerBase.Kernel.Random.Next(1, 59);
                                break;
                            }
                        case 2:
                            {
                                if (EnchantGem.ID == 700012)
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(100, 159);
                                else if (EnchantGem.ID == 700002 || EnchantGem.ID == 700052 || EnchantGem.ID == 700062)
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(60, 109);
                                else if (EnchantGem.ID == 700032)
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(80, 129);
                                else
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(40, 89);
                                break;
                            }
                        default:
                            {
                                if (EnchantGem.ID == 700013)
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(200, 255);
                                else if (EnchantGem.ID == 700003 || EnchantGem.ID == 700073 || EnchantGem.ID == 700033)
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(170, 229);
                                else if (EnchantGem.ID == 700063 || EnchantGem.ID == 700053)
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(140, 199);
                                else if (EnchantGem.ID == 700023)
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(90, 149);
                                else
                                    Enchant = (byte)ServerBase.Kernel.Random.Next(70, 119);
                                break;
                            }
                    }
                    client.Send(ServerBase.Constants.Enchant(Item.Enchant, Enchant));
                    if (Enchant > Item.Enchant)
                    {
                        Item.Enchant = Enchant;
                        Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                        Item.Send(client);
                        Database.ConquerItemTable.UpdateEnchant(Item, client);
                        client.Inventory.Remove(EnchantGem, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                    }
                    else
                    {
                        client.Inventory.Remove(EnchantGem, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                    }
                }
            }
        }

        static void UseItem(Interfaces.IConquerItem item, Client.GameState client)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(item.ID, 0);

            switch (item.ID)
            {
                #region MemoryAgate
                case 720828:
                    {
                        if (client.Entity.MapID == 1038 || client.Entity.MapID == 1090 || client.Entity.MapID == 1858
                            || client.Entity.MapID == 1068 || client.Entity.MapID == 1730 || client.Entity.MapID == 1731
                            || client.Entity.MapID == 1732 || client.Entity.MapID == 1733 || client.Entity.MapID == 1505
                            || client.Entity.MapID == 1506 || client.Entity.MapID == 1507 || client.Entity.MapID == 1508
                            || client.Entity.MapID == 1525 || client.Entity.MapID == 1526 || client.Entity.MapID == 1527
                            || client.Entity.MapID == 1528 || client.Entity.MapID >= 10000 || client.Entity.MapID == 1950)
                        {
                            client.Send(new Message("Yyou Can't record here !", System.Drawing.Color.Tan, Message.TopLeft));
                            return;
                        }
                        else
                        {
                            item.SendAgate(client);
                        }

                        break;
                    }
                #endregion
                #region Medicine
                case 1000000:
                case 1000010:
                case 1000020:
                case 1000030:
                case 1002000:
                case 1002010:
                case 1002020:
                case 1002050:
                case 725065:
                    {
                        if (client.Entity.NoDrugsTime > 0)
                        {
                            if (Time32.Now > client.Entity.NoDrugsStamp.AddSeconds(client.Entity.NoDrugsTime))
                            {
                                client.Entity.NoDrugsTime = 0;
                            }
                            else
                            {
                                return;
                            }
                        }
                        if (client.Entity.Hitpoints == client.Entity.MaxHitpoints)
                            return;
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Hitpoints = Math.Min(client.Entity.Hitpoints + infos.BaseInformation.ItemHP, client.Entity.MaxHitpoints);
                        break;
                    }
                case 1001000:
                case 1001010:
                case 1001020:
                case 1001030:
                case 1001040:
                case 1002030:
                case 1002040:
                case 725066:
                    {
                        if (client.Entity.NoDrugsTime > 0)
                        {
                            if (Time32.Now > client.Entity.NoDrugsStamp.AddSeconds(client.Entity.NoDrugsTime))
                            {
                                client.Entity.NoDrugsTime = 0;
                            }
                            else
                            {
                                return;
                            }
                        }
                        if (client.Entity.Mana == client.Entity.MaxMana)
                            return;
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Mana = (ushort)Math.Min(client.Entity.Mana + infos.BaseInformation.ItemMP, client.Entity.MaxMana);
                        break;
                    }
                #endregion
                #region Gates
                case 1060020:
                    {
                        if (client.Entity.MapID == 601 || client.Map.BaseID == 700) return;
                        if (client.Map.BaseID == 6000 || client.Map.BaseID == 6001)
                        {
                            client.Send(ServerBase.Constants.JailItemUnusable);
                            return;
                        }
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Teleport(1002, 429, 378);
                        break;
                    }
                case 1060021:
                    {
                        if (client.Entity.MapID == 601 || client.Map.BaseID == 700) return;
                        if (client.Map.BaseID == 6000 || client.Map.BaseID == 6001)
                        {
                            client.Send(ServerBase.Constants.JailItemUnusable);
                            return;
                        }
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Teleport(1000, 500, 650);
                        break;
                    }
                case 1060022:
                    {
                        if (client.Entity.MapID == 601 || client.Map.BaseID == 700) return;
                        if (client.Map.BaseID == 6000 || client.Map.BaseID == 6001)
                        {
                            client.Send(ServerBase.Constants.JailItemUnusable);
                            return;
                        }
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Teleport(1020, 565, 562);
                        break;
                    }
                case 1060023:
                    {
                        if (client.Entity.MapID == 601 || client.Map.BaseID == 700) return;
                        if (client.Map.BaseID == 6000 || client.Map.BaseID == 6001)
                        {
                            client.Send(ServerBase.Constants.JailItemUnusable);
                            return;
                        }
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Teleport(1011, 188, 264);
                        break;
                    }
                case 1060024:
                    {
                        if (client.Entity.MapID == 601 || client.Map.BaseID == 700) return;
                        if (client.Map.BaseID == 6000 || client.Map.BaseID == 6001)
                        {
                            client.Send(ServerBase.Constants.JailItemUnusable);
                            return;
                        }
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Teleport(1015, 717, 571);
                        break;
                    }
                #endregion
                #region Dyes
                case 723584:
                    {
                        if (client.Equipment.TryGetItem(3) == null)
                            return;
                        if (client.Equipment.TryGetItem(3).ID == 0)
                            return;
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Equipment.TryGetItem(3).Color = Game.Enums.Color.Black;
                        Conquer_Online_Server.Database.ConquerItemTable.UpdateColor(client.Equipment.TryGetItem(3), client);
                        client.Equipment.TryGetItem(3).Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                        client.Equipment.TryGetItem(3).Send(client);
                        client.Equipment.UpdateEntityPacket();
                        break;
                    }
                case 1060030:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.HairColor = 3;
                        break;
                    }
                case 1060040:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.HairColor = 9;
                        break;
                    }
                case 1060050:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.HairColor = 8;
                        break;
                    }
                case 1060060:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.HairColor = 7;
                        break;
                    }
                case 1060070:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.HairColor = 6;
                        break;
                    }
                case 1060080:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.HairColor = 5;
                        break;
                    }
                case 1060090:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.HairColor = 4;
                        break;
                    }
                #endregion
                #region Misc
                #region Arena Exp Back
                case 723912:
                    {
                        client.IncreaseExperience(client.ExpBall, false);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion


                #region elitepk Champion Prizes
                #region ElitePkChampion Prize

                #region 500Stuydy
                case 723342:
                    {
                        Attack attack = new Attack(true);
                        attack.Attacker = client.Entity.UID;
                        attack.Attacked = client.Entity.UID;
                        attack.AttackType = 2;
                        attack.Damage = 500;
                        //attack.ResponseDamage = 500;
                        attack.X = client.Entity.X;
                        attack.Y = client.Entity.Y;
                        attack.SecondEffect = Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue.StudyPoints;
                        client.Entity.Owner.SendScreen(attack, true);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Remove);
                        client.Entity.SubClasses.StudyPoints += 500;
                        client.Send(ServerBase.Constants.Study);
                        break;
                    }
                #endregion
                #region Random Accseeoreis
                case 720836:
                    {
                        uint ItemID = 0;
                        uint rand = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 78);
                        switch (rand)
                        {
                            #region Rand Accesory
                            case 1:
                                ItemID = 350001;
                                break;
                            case 2:
                                ItemID = 350002;
                                break;
                            case 3:
                                ItemID = 350004;
                                break;
                            case 4:
                                ItemID = 350005;
                                break;
                            case 5:
                                ItemID = 350006;
                                break;
                            case 6:
                                ItemID = 350007;
                                break;
                            case 7:
                                ItemID = 350008;
                                break;
                            case 8:
                                ItemID = 350009;
                                break;
                            case 9:
                                ItemID = 350010;
                                break;
                            case 10:
                                ItemID = 350011;
                                break;
                            case 11:
                                ItemID = 350012;
                                break;
                            case 12:
                                ItemID = 350014;
                                break;
                            case 13:
                                ItemID = 350015;
                                break;
                            case 14:
                                ItemID = 350016;
                                break;
                            case 15:
                                ItemID = 350017;
                                break;
                            case 16://PalmLeafFan 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory A~delicate~and~beautiful~fan~made~of~palm~leaves. 8 0 0 
                                ItemID = 350018;
                                break;
                            case 17://IronShovel 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory An~iron~shovel~may~come~in~handy~in~winter,~when~you~need~to~shovel~paths~through~snow. 8 0 0 
                                ItemID = 350019;
                                break;
                            case 18:////FrozenTuna 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory It`s~a~strangely-shaped~tuna~from~Bird~Island. 8 0 0 
                                ItemID = 350020; break;
                            case 19://IceStick 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory A~strange~stick.~Maybe~you~can~play~ice~hockey~with~it. 8 0 0 
                                ItemID = 360001;
                                break;
                            case 20://Wrench 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory It`s~a~heavy~wrench.~Perhaps~you~can~use~it~to~deal~ 8 0 0 
                                ItemID = 360002; break;
                            case 21://WoodenClub 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory This~wooden~club~often~gives~off~a~sweet~odor. 8 0 0 
                                ItemID = 360003; break;
                            case 22://Umbrella 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory An~unbreakable~umbrella~is~a~stealth~weapon~that~also~keeps~you~dry. 8 0 0 
                                ItemID = 360004; break;
                            case 23:////Blowfish 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Dried~in~the~bright~sunlight~over~the~Desert,~the~blowfish~makes~a~good~weapon~for~it`s~as~hard~as~iron. 8 0 0 
                                ItemID = 360005; break;
                            case 24://FeatherDuster 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360006; break;
                            case 25://Spatula 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360007; break;
                            case 26://InvincibleFist 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360008; break;
                            case 27://FishPole 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360009; break;
                            case 28://Pan 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360010; break;
                            case 29://Handbag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360011;
                                break;
                            case 30://Backpack 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360012;
                                break;
                            case 31://SportsBag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360013; break;
                            case 32://Bunny 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360014; break;
                            case 33://GoodEveningBear 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360015; break;
                            case 34://Rod 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360016;
                                break;
                            case 35://Clap 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360017;
                                break;
                            case 36://HeavyHammer 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360018;
                                break;
                            case 37://LightSaber 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360019
                                     ;
                                break;
                            case 38://TennisRacket 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360020;
                                break;
                            case 39://ApeCityHam 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 360021;
                                break;
                            case 40: //Wrench 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory It`s~a~heavy~wrench. 8 0 0 
                                ItemID = 360022;
                                break;
                            case 41: //WoodenClub 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory This~wooden~club~often~gives~off~a~sweet~odor. 8 0 0 
                                ItemID = 360023;
                                break;
                            case 42://Umbrella 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory An~unbreakable~umbrella~is~a~stealth~weapon~that~also~keeps~you~dry. 8 0 0 
                                ItemID = 360024;
                                break;
                            case 43://Blowfish 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Dried~in~the~bright~sunlight~over~the~Desert,~the~Mackerel~makes~a~good~weapon~for~it`s~as~hard~as~iron. 8 0 0 
                                ItemID = 360025;
                                break;
                            case 44://FeatherDuster 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360026;
                                break;
                            case 45://Spatula 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360027;
                                break;
                            case 46://InvincibleFist 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360028;
                                break;
                            case 47://FishPole 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360029;
                                break;
                            case 48://Pan 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360030;
                                break;
                            case 49://Handbag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360031;
                                break;
                            case 50://Backpack 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360032;
                                break;
                            case 51://SportsBag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360033;
                                break;
                            case 52://Bunny 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360034;
                                break;
                            case 53://GoodEveningBear 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360035;
                                break;
                            case 54://Rod 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360036;
                                break;
                            case 55://Clap 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360037;
                                break;
                            case 56://HeavyHammer 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360038;
                                break;
                            case 57://LightSaber 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360039;
                                break;
                            case 58://TennisRacket 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 360040;
                                break;
                            case 59://ApeCityHam 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 370001;
                                break;
                            case 60: //WoodenBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 370002;
                                break;
                            case 61://LoveBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 370003;
                                break;
                            case 62://SeaHorse 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 370004;
                                break;
                            case 63://Harp 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 370005;
                                break;
                            case 64://ForceBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 370011;
                                break;
                            case 65://WoodenBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 370012;
                                break;
                            case 66://LoveBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 370013;
                                break;
                            case 67://SeaHorse 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 370014;
                                break;
                            case 68://Harp 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 370015;
                                break;
                            case 69://ForceBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 380001;
                                break;
                            case 70://Wok 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 380002;
                                break;
                            case 71://TurtleShell 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 380003;
                                break;
                            case 72://LoveShield 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 380004;
                                break;
                            case 73://SunFlower 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 380005;
                                break;
                            case 74://Wheel 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 380011;
                                break;
                            case 75://Wok 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 380012;
                                break;
                            case 76://TurtleShell 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 380013;
                                break;
                            case 77://LoveShield 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 380014;
                                break;
                            case 78://SunFlower 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 380015;
                                break;
                            default:
                                ItemID = 380013;
                                break;
                            //Wheel 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                            #endregion
                        }
                        client.Inventory.Add(ItemID, 0, 1);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion
                #region Random Refinery
                case 723693:
                    {
                        uint ItemID = 0;
                        uint rand = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 97);
                        switch (rand)
                        {
                            #region Rand Refinery
                            case 1:
                                ItemID = 724350;
                                break;
                            case 2:
                                ItemID = 724351;
                                break;
                            case 3:
                                ItemID = 724352;
                                break;
                            case 4:
                                ItemID = 724353;
                                break;
                            case 5:
                                ItemID = 724354;
                                break;
                            case 6:
                                ItemID = 724355;
                                break;
                            case 7:
                                ItemID = 724356;
                                break;
                            case 8:
                                ItemID = 724357;
                                break;
                            case 9:
                                ItemID = 724358;
                                break;
                            case 10:
                                ItemID = 724359;
                                break;
                            case 11:
                                ItemID = 724360;
                                break;
                            case 12:
                                ItemID = 724361;
                                break;
                            case 13:
                                ItemID = 724362;
                                break;
                            case 14:
                                ItemID = 724350;
                                break;
                            case 15:
                                ItemID = 724363;
                                break;
                            case 16://PalmLeafFan 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory A~delicate~and~beautiful~fan~made~of~palm~leaves. 8 0 0 
                                ItemID = 724364;
                                break;
                            case 17://IronShovel 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory An~iron~shovel~may~come~in~handy~in~winter,~when~you~need~to~shovel~paths~through~snow. 8 0 0 
                                ItemID = 724365;
                                break;
                            case 18:////FrozenTuna 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory It`s~a~strangely-shaped~tuna~from~Bird~Island. 8 0 0 
                                ItemID = 724366; break;
                            case 19://IceStick 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 2-HandedAccessory A~strange~stick.~Maybe~you~can~play~ice~hockey~with~it. 8 0 0 
                                ItemID = 724367;
                                break;
                            case 20://Wrench 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory It`s~a~heavy~wrench.~Perhaps~you~can~use~it~to~deal~ 8 0 0 
                                ItemID = 724368; break;
                            case 21://WoodenClub 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory This~wooden~club~often~gives~off~a~sweet~odor. 8 0 0 
                                ItemID = 724369; break;
                            case 22://Umbrella 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory An~unbreakable~umbrella~is~a~stealth~weapon~that~also~keeps~you~dry. 8 0 0 
                                ItemID = 724370; break;
                            case 23:////Blowfish 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Dried~in~the~bright~sunlight~over~the~Desert,~the~blowfish~makes~a~good~weapon~for~it`s~as~hard~as~iron. 8 0 0 
                                ItemID = 724371; break;
                            case 24://FeatherDuster 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724372; break;
                            case 25://Spatula 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724373; break;
                            case 26://InvincibleFist 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724374; break;
                            case 27://FishPole 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724375; break;
                            case 28://Pan 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724376; break;
                            case 29://Handbag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724377;
                                break;
                            case 30://Backpack 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724378;
                                break;
                            case 31://SportsBag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724379; break;
                            case 32://Bunny 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724380; break;
                            case 33://GoodEveningBear 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724381; break;
                            case 34://Rod 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724382;
                                break;
                            case 35://Clap 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724383;
                                break;
                            case 36://HeavyHammer 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724384;
                                break;
                            case 37://LightSaber 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724385
                                     ;
                                break;
                            case 38://TennisRacket 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724386;
                                break;
                            case 39://ApeCityHam 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory None. 8 0 0 
                                ItemID = 724387;
                                break;
                            case 40: //Wrench 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory It`s~a~heavy~wrench. 8 0 0 
                                ItemID = 724388;
                                break;
                            case 41: //WoodenClub 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory This~wooden~club~often~gives~off~a~sweet~odor. 8 0 0 
                                ItemID = 724389;
                                break;
                            case 42://Umbrella 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory An~unbreakable~umbrella~is~a~stealth~weapon~that~also~keeps~you~dry. 8 0 0 
                                ItemID = 724390;
                                break;
                            case 43://Blowfish 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Dried~in~the~bright~sunlight~over~the~Desert,~the~Mackerel~makes~a~good~weapon~for~it`s~as~hard~as~iron. 8 0 0 
                                ItemID = 724391;
                                break;
                            case 44://FeatherDuster 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724392;
                                break;
                            case 45://Spatula 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724393;
                                break;
                            case 46://InvincibleFist 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724394;
                                break;
                            case 47://FishPole 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724395;
                                break;
                            case 48://Pan 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724396;
                                break;
                            case 49://Handbag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724397;
                                break;
                            case 50://Backpack 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724398;
                                break;
                            case 51://SportsBag 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724399;
                                break;
                            case 52://Bunny 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724400;
                                break;
                            case 53://GoodEveningBear 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724401;
                                break;
                            case 54://Rod 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724402;
                                break;
                            case 55://Clap 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724403;
                                break;
                            case 56://HeavyHammer 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724404;
                                break;
                            case 57://LightSaber 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724405;
                                break;
                            case 58://TennisRacket 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724406;
                                break;
                            case 59://ApeCityHam 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1-HandedAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724407;
                                break;
                            case 60: //WoodenBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724408;
                                break;
                            case 61://LoveBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724409;
                                break;
                            case 62://SeaHorse 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724410;
                                break;
                            case 63://Harp 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724411;
                                break;
                            case 64://ForceBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724412;
                                break;
                            case 65://WoodenBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724413;
                                break;
                            case 66://LoveBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724414;
                                break;
                            case 67://SeaHorse 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724415;
                                break;
                            case 68://Harp 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724416;
                                break;
                            case 69://ForceBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724417;
                                break;
                            case 70://Wok 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724418;
                                break;
                            case 71://TurtleShell 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724419;
                                break;
                            case 72://LoveShield 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724420;
                                break;
                            case 73://SunFlower 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724421;
                                break;
                            case 74://Wheel 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724422;
                                break;
                            case 75://Wok 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724423;
                                break;
                            case 76://TurtleShell 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724424;
                                break;
                            case 77://LoveShield 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724425;
                                break;
                            case 78://SunFlower 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724423;
                                break;
                            case 79: //WoodenBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724424;
                                break;
                            case 80://LoveBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724425;
                                break;
                            case 81://SeaHorse 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724426;
                                break;
                            case 82://Harp 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724427;
                                break;
                            case 83://ForceBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory None. 8 0 0 
                                ItemID = 724428;
                                break;
                            case 84://WoodenBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724429;
                                break;
                            case 85://LoveBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724430;
                                break;
                            case 86://SeaHorse 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724431;
                                break;
                            case 87://Harp 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724432;
                                break;
                            case 88://ForceBow 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 BowAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724433;
                                break;
                            case 89://Wok 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724434;
                                break;
                            case 90://TurtleShell 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724435;
                                break;
                            case 91://LoveShield 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724436;
                                break;
                            case 92://SunFlower 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724437;
                                break;
                            case 93://Wheel 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 0 0 10080 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory None. 8 0 0 
                                ItemID = 724438;
                                break;
                            case 94://Wok 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724439;
                                break;
                            case 95://TurtleShell 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724440;
                                break;
                            case 96://LoveShield 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724441;
                                break;
                            case 97://SunFlower 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                                ItemID = 724442;
                                break;
                            default:
                                ItemID = 724445;
                                break;
                            //Wheel 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 65535 65535 0 0 0 0 0 0 0 0 0 1 800 2 2 1 215 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 ShieldAccessory Right~click~to~equip. 8 0 0 
                            #endregion
                        }
                        client.Inventory.Add(ItemID, 0, 1);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion
                #region PkEliteFirst Pack
                case 720717:
                    {
                        if (client.Inventory.Count <= 23)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(723342, 0, 5);//mondostyBook
                            client.Inventory.Add(720598, 0, 3);//dragonPill
                            client.Inventory.Add(720028, 0, 3);//dbscroll
                            client.Inventory.Add(720836, 0, 1);//accessores
                            client.Inventory.Add(723693, 0, 1);//legandRefineryPack
                            client.Inventory.Add(723744, 0, 3);//powerExpBall
                            client.Inventory.Add(723864, 0, 1);//Steed+6
                        }
                        else
                        {
                            client.Send(new Message("you must have 17 space in you inventory to take prize", Color.Green, 2005));
                        }

                        break;
                    }
                #endregion
                #region PkEliteFirst Pack2
                case 720721:
                    {
                        if (client.Inventory.Count <= 30)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(723342, 0, 3);//mondostyBook
                            client.Inventory.Add(720598, 0, 2);//dragonPill
                            client.Inventory.Add(720028, 0, 1);//dbscroll
                            client.Inventory.Add(720836, 0, 1);//accessores
                            client.Inventory.Add(723693, 0, 1);//legandRefineryPack
                            client.Inventory.Add(723744, 0, 1);//powerExpBall
                            //client.Inventory.Add(723864, 0, 1);//Steed+6
                        }
                        else
                        {
                            client.Send(new Message("you must have 10 space in you inventory to take prize", Color.Green, 2005));
                        }

                        break;
                    }
                #endregion
                #region PkEliteFirst Pack3
                case 720725:
                    {
                        if (client.Inventory.Count <= 30)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(723342, 0, 2);//mondostyBook
                            client.Inventory.Add(720598, 0, 2);//dragonPill
                            client.Inventory.Add(720028, 0, 1);//dbscroll
                            client.Inventory.Add(720836, 0, 1);//accessores
                            client.Inventory.Add(723693, 0, 1);//legandRefineryPack
                            client.Inventory.Add(723912, 0, 2);//ArenaExp
                            //client.Inventory.Add(723864, 0, 1);//Steed+6
                        }
                        else
                        {
                            client.Send(new Message("you must have 10 space in you inventory to take prize", Color.Green, 2005));
                        }

                        break;
                    }
                #endregion
                #region PkEliteFirst Pack8
                case 720729:
                    {
                        if (client.Inventory.Count <= 30)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(723342, 0, 1);//mondostyBook
                            client.Inventory.Add(720598, 0, 1);//dragonPill
                            client.Inventory.Add(720028, 0, 1);//dbscroll
                            client.Inventory.Add(720836, 0, 1);//accessores
                            client.Inventory.Add(723693, 0, 1);//legandRefineryPack
                            client.Inventory.Add(723912, 0, 1);//ArenaExp
                            //client.Inventory.Add(723864, 0, 1);//Steed+6
                        }
                        else
                        {
                            client.Send(new Message("you must have 6 space in you inventory to take prize", Color.Green, 2005));
                        }

                        break;
                    }
                #endregion
                #endregion
                #endregion



                #region StudyBox
                case 720774:
                    {
                        Attack attack = new Attack(true);
                        attack.Attacker = client.Entity.UID;
                        attack.Attacked = client.Entity.UID;
                        attack.AttackType = 2;
                        attack.Damage = 50;
                        //attack.ResponseDamage = 500;
                        attack.X = client.Entity.X;
                        attack.Y = client.Entity.Y;
                        attack.SecondEffect = Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue.StudyPoints;
                        client.Entity.Owner.SendScreen(attack, true);
                        client.Entity.SubClasses.StudyPoints += 50;
                        client.Send(new Message("You Obtined 50 StudyPoints!.", System.Drawing.Color.Tan, Message.TopLeft));
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Database.SubClassTable.Update(client.Entity);

                        break;
                    }
                #endregion
                #region DragonPill
                case 720598:
                    {


                        //Database.MonsterInformation monster = new Conquer_Online_Server.Database.MonsterInformation();
                        //monster.Boss = true;
                        //monster.Hitpoints = 50000000;
                        //monster.Level = 140;
                        //monster.Mesh = 950;
                        //monster.Name = "TeratoDragon";
                        //monster.MaxAttack = 10500;
                        //monster.AttackRange = 5;
                        //monster.AttackType = 2;
                        //monster.AttackSpeed = 1000;
                        //monster.ViewRange = 2;
                        //monster.MoveSpeed = 500;
                        //monster.RunSpeed = 500;
                        //monster.MinAttack = 59000;
                        //Game.Entity entity = new Game.Entity(Game.EntityFlag.Monster, false);
                        //entity.MapObjType = Game.MapObjectType.Monster;
                        //entity.MonsterInfo = monster;
                        //entity.MonsterInfo.Owner = entity;
                        //entity.Name = "TeratoDragon";
                        //entity.MinAttack = monster.MinAttack;
                        //entity.MaxAttack = entity.MagicAttack = monster.MaxAttack;
                        //entity.Hitpoints = entity.MaxHitpoints = monster.Hitpoints;
                        //entity.Body = monster.Mesh;
                        //entity.Level = monster.Level;
                        //entity.Defence = 5000;
                        //entity.X = client.Entity.X;
                        //entity.Y = client.Entity.Y;
                        //entity.UID = 500002;
                        //entity.MapID = client.Entity.MapID;
                        //entity.SendUpdates = true;
                        ////client.Map.RemoveEntity(entity);
                        //client.Map.AddEntity(entity);
                        //Network.GamePackets._String stringPacket = new Conquer_Online_Server.Network.GamePackets._String(true);
                        //stringPacket.UID = entity.UID;
                        //stringPacket.Type = Network.GamePackets._String.Effect;
                        //stringPacket.Texts.Add("MBStandard");
                        //entity.SetFlag(0, 0);
                        //var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                        //varr.MoveNext();
                        //int COunt = ServerBase.Kernel.GamePool.Count;
                        //for (uint x = 0;
                        //    x < COunt;
                        //    x++)
                        //{
                        //    if (x >= COunt) break;

                        //    Client.GameState aclient = (varr.Current as Client.GameState);


                        //    if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, entity.X, entity.Y) < ServerBase.Constants.nScreenDistance)
                        //    {
                        //        entity.CauseOfDeathIsMagic = false;
                        //        aclient.Entity.Teleport(aclient.Entity.X, aclient.Entity.Y);
                        //        aclient.Send(stringPacket);
                        //    }


                        //    varr.MoveNext();

                        //}



                        //client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        //Conquer_Online_Server.ServerBase.Kernel.Terato_open = true;


                        break;

                    }
                #endregion
                #region PowerEXPBall
                case 722057:
                case 723744:
                    {

                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.IncreaseExperience(client.Entity.Experience / 10, false);
                        break;
                    }
                #endregion
                #region SteedPacks
                case 723855:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 1;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 << 8 | 255 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723856:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 1;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 | 255 << 8;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723859:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 1;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 255 | 150 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723860:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 3;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 << 8 | 255 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723861:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 3;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 | 255 << 8;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723862:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 3;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 255 | 150 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723863:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 6;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 << 8 | 255 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723864:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 6;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 | 255 << 8;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723865:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 6;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 255 | 150 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723900:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 0;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 << 8 | 255 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723901:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 0;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 150 | 255 << 8;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                case 723902:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        Interfaces.IConquerItem _item = new ConquerItem(true);
                        _item.ID = 300000;
                        Database.ConquerItemInformation _iteminfos = new Database.ConquerItemInformation(_item.ID, 0);
                        _item.Durability = _item.MaximDurability = _iteminfos.BaseInformation.Durability;
                        _item.Plus = 0;
                        _item.Effect = Game.Enums.ItemEffect.Horse;
                        _item.SocketProgress = 255 | 150 << 16;
                        client.Inventory.Add(_item, Game.Enums.ItemUse.CreateAndAdd);
                        break;
                    }
                #endregion
                #region LifeFruitBasket
                case 723725:
                    {
                        if (client.Inventory.Count <= 31)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(723726, 0, 10);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region 7StarPouch
                case 725067:
                    {
                        if (client.Inventory.Count <= 35)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(725065, 0, 5);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region SerenityPouch
                case 725068:
                    {
                        if (client.Inventory.Count <= 35)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(725066, 0, 5);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region SpeedArrowPack
                case 727000:
                    {
                        if (client.Inventory.Count <= 35)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1050002, 0, 1);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region LifeFruit
                case 723726:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Hitpoints = client.Entity.MaxHitpoints;
                        client.Entity.Mana = client.Entity.MaxMana;
                        break;
                    }
                #endregion
                #region Amrita Box
                case 720010:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1000030, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region Panacea Box
                case 720011:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1002000, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region Ginseng Box
                case 720012:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1002010, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region Vanilla Box
                case 720013:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1002020, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region RecoveryPill Box
                case 720014:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1001030, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion RecoveryPill
                #region SoulPill Box
                case 720015:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1001040, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region RefreshingPill Box
                case 720016:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1002030, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region ChantPill Box
                case 720017:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1002040, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region Mil.Ginseng Box
                case 721330:
                    {
                        if (client.Inventory.Count <= 38)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1002050, 0, 3);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region ExpBall

                case 722136:
                    {
                        
                                client.IncreaseExperience(client.ExpBall, false);
                                client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                       
                        break;
                    }
                case 723700:
                    {
                        if (client.ExpBalls < 10)
                        {
                            if (client.Entity.Level < 137)
                            {
                                client.IncreaseExperience(client.ExpBall, false);
                                client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                                client.ExpBalls++;
                            }
                        }
                        else
                            client.Send(ServerBase.Constants.ExpBallsUsed);
                        break;
                    }
                #endregion
                #region MeteorTearScroll
                case 723711:
                    {
                        if (client.Inventory.Count <= 36)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1088002, 0, 5);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region +1Stone Pack
                case 723712:
                    {
                        if (client.Inventory.Count <= 36)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(730001, 1, 5);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region Class1MoneyBag
                case 723713:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 300000;
                        break;
                    }
                #endregion
                #region Class2MoneyBag
                case 723714:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 800000;
                        break;
                    }
                #endregion
                #region Class3MoneyBag
                case 723715:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 1200000;
                        break;
                    }
                #endregion
                #region Class4MoneyBag
                case 723716:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 1800000;
                        break;
                    }
                #endregion
                #region Class5MoneyBag
                case 723717:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 5000000;
                        break;
                    }
                #endregion
                #region Class6MoneyBag
                case 723718:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 20000000;
                        break;
                    }
                #endregion
                #region Class7MoneyBag
                case 723719:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 25000000;
                        break;
                    }
                #endregion
                #region Class8MoneyBag
                case 723720:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 80000000;
                        break;
                    }
                #endregion
                #region Class9MoneyBag
                case 723721:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 100000000;
                        break;
                    }
                #endregion
                #region Class10MoneyBag
                case 723722:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 300000000;
                        break;
                    }
                #endregion
                #region TopMoneyBag
                case 723723:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += 500000000;
                        break;
                    }
                #endregion
                #region DrasgonBallScroll
                case 720028:
                    {
                        if (client.Inventory.Count <= 31)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1088000, 0, 10);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region MeteorScroll
                case 720027:
                    {
                        if (client.Inventory.Count <= 31)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.Inventory.Add(1088001, 0, 10);
                        }
                        else
                            client.Send(ServerBase.Constants.FullInventory);
                        break;
                    }
                #endregion
                #region DoubleExperiencePotion
                case 723017:
                    {
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.DoubleExperienceTime = 3600;
                      //  client.Entity.DoubleExperienceTimeV1 = 0;
                        break;
                    }
                case 723917:
                    {
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.DoubleExperienceTime5 = 3600;
                        //client.Entity.DoubleExperienceTime = 0;
                        //client.Entity.DoubleExperienceTime10 = 0;
                        //client.Entity.DoubleExperienceTime15 = 0;
                        //client.Entity.DoubleExperienceTimeV1 = 0;
                        break;
                    }
                case 723918:
                    {
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.DoubleExperienceTime10 = 3600;
                        //client.Entity.DoubleExperienceTime = 0;
                        //client.Entity.DoubleExperienceTime5 = 0;
                        //client.Entity.DoubleExperienceTime15 = 0;
                        //client.Entity.DoubleExperienceTimeV1 = 0;
                        break;
                    }
                case 723919:
                    {
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.DoubleExperienceTime15 = 3600;
                        //client.Entity.DoubleExperienceTime = 0;
                        //client.Entity.DoubleExperienceTime10 = 0;
                        //client.Entity.DoubleExperienceTime5 = 0;
                        //client.Entity.DoubleExperienceTimeV1 = 0;
                        break;
                    }
                #endregion
                #region CpsBag
                case 723980:
                    {
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.DoubleExperienceTimeV1 = 0;
                       // client.Entity.DoubleExperienceTime = 0;
                        break;
                    }
                #endregion
                #region NinjaAmulet
                case 723583:
                    {
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        switch (client.Entity.Body % 10)
                        {
                            case 2:
                            case 4:
                                client.Entity.Body--;
                                break;
                            case 1:
                            case 3:
                                client.Entity.Body++;
                                break;
                        }
                        break;
                    }
                #endregion
                #region PrayingStone(S)
                case 1200000:
                    {
                        uint value = (uint)(3 * 24 * 60 * 60);
                        client.AddBless(value);
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.Update(Network.GamePackets.Update.OnlineTraining, client.OnlineTrainingPoints, false);
                        break;
                    }
                #endregion
                #region PrayingStone(M)
                case 1200001:
                    {
                        uint value = (uint)(7 * 24 * 60 * 60);
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.AddBless(value);
                        client.Entity.Update(Network.GamePackets.Update.OnlineTraining, client.OnlineTrainingPoints, false);
                        break;
                    }
                #endregion
                #region PrayingStone(L)
                case 1200002:
                    {
                        uint value = (uint)(30 * 24 * 60 * 60);
                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.AddBless(value);
                        client.Entity.Update(Network.GamePackets.Update.OnlineTraining, client.OnlineTrainingPoints, false);
                        break;
                    }
                #endregion
                #region PenitenceAmulet
                case 723727:
                case 720128:
                    {
                        if (client.Entity.PKPoints >= 30)
                        {
                            client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                            client.Entity.PKPoints -= 30;
                        }
                        break;
                    }
                #endregion
                #region DisguiseAmulet
                case 723724:
                    {
                        int disguise = ServerBase.Kernel.Random.Next(Database.DataHolder.Disguises.Length);
                        ushort selected = Database.DataHolder.Disguises[disguise];

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);

                        bool wasTransformated = client.Entity.Transformed;
                        if (wasTransformated)
                        {
                            client.Entity.Hitpoints = client.Entity.MaxHitpoints;
                            client.Entity.TransformationID = 0;
                            client.Entity.TransformationStamp = Time32.Now;
                        }
                        ushort transformation = selected;
                        client.Entity.TransformationID = transformation;
                        client.Entity.TransformationStamp = Time32.Now;
                        client.Entity.TransformationTime = 110;
                        SpellUse spellUse = new SpellUse(true);
                        spellUse.Attacker = client.Entity.UID;
                        spellUse.SpellID = 1360;
                        spellUse.SpellLevel = 4;
                        spellUse.X = client.Entity.X;
                        spellUse.Y = client.Entity.Y;
                        spellUse.Targets.Add(client.Entity.UID, (uint)0);
                        client.Send(spellUse);
                        client.Entity.TransformationMaxHP = 3000;
                        double maxHP = client.Entity.MaxHitpoints;
                        double HP = client.Entity.Hitpoints;
                        double point = HP / maxHP;

                        client.Entity.Hitpoints = (uint)(client.Entity.TransformationMaxHP * point);
                        client.Entity.Update(Update.MaxHitpoints, client.Entity.TransformationMaxHP, false);
                        break;

                    }
                #endregion
                #endregion
                #region SkillBooks
                case 725000:
                    {
                        if (client.Entity.Spirit >= 20)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1000 });
                        }
                        else
                        {
                            client.Send(new Message("You need atleast 20 spirit!", System.Drawing.Color.Tan, Message.TopLeft));
                        }
                        break;
                    }
                case 725001:
                    {
                        if (client.Entity.Spirit >= 80)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1001 });
                        }
                        else
                        {
                            client.Send(new Message("You need atleast 80 spirit!", System.Drawing.Color.Tan, Message.TopLeft));
                        }
                        break;
                    }
                case 725002:
                    {
                        if (client.Entity.Class >= 140 && client.Entity.Class <= 145 && client.Entity.Level >= 90)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1002 });
                        }
                        break;
                    }
                case 725003:
                    {
                        if (client.Entity.Spirit >= 30)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1005 });
                        }
                        break;
                    }
                case 725004:
                    {
                        if (client.Entity.Class >= 130 && client.Entity.Class <= 135 || client.Entity.Class >= 140 && client.Entity.Class <= 145 && client.Entity.Level >= 15 || client.Entity.Class == 100 || client.Entity.Class == 101)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1010 });
                        }
                        break;
                    }
                case 725005:
                    {
                        if (client.Entity.Level >= 40)
                        {
                            if (client.Proficiencies.ContainsKey(Database.SpellTable.SpellInformations[1045][0].WeaponSubtype))
                                if (client.Proficiencies[Database.SpellTable.SpellInformations[1045][0].WeaponSubtype].Level >= 5)
                                {
                                    client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                                    client.AddSpell(new Spell(true) { ID = 1045 });
                                }
                                else
                                {
                                    client.Send(new Message("You need level 5 at blade proficiency!", System.Drawing.Color.Tan, Message.TopLeft));
                                }
                        }
                        break;
                    }
                case 725010:
                    {
                        if (client.Entity.Level >= 40)
                        {
                            if (client.Proficiencies.ContainsKey(Database.SpellTable.SpellInformations[1046][0].WeaponSubtype))
                                if (client.Proficiencies[Database.SpellTable.SpellInformations[1046][0].WeaponSubtype].Level >= 5)
                                {
                                    client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                                    client.AddSpell(new Spell(true) { ID = 1046 });
                                }
                                else
                                {
                                    client.Send(new Message("You need level 5 at sword proficiency!", System.Drawing.Color.Tan, Message.TopLeft));
                                }
                        }
                        break;
                    }
                case 725011:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1250 });
                        break;
                    }
                case 725012:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1260 });
                        break;
                    }
                case 725013:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1290 });
                        break;
                    }
                case 725014:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1300 });
                        break;
                    }
                case 725015:
                    {
                        if (client.Entity.Class >= 130 && client.Entity.Class <= 135)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1350 });
                        }
                        break;
                    }
                case 725016:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1360 });
                        break;
                    }
                case 725018:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1380 });
                        break;
                    }
                case 725019:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1385 });
                        break;
                    }
                case 725020:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1390 });
                        break;
                    }
                case 725021:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1395 });
                        break;
                    }
                case 725022:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1400 });
                        break;
                    }
                case 725023:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1405 });
                        break;
                    }
                case 725024:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1410 });
                        break;
                    }
                case 725025:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 1320 });
                        break;
                    }
                case 725026:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 5010 });
                        break;
                    }
                case 725027:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 5020 });
                        break;
                    }
                case 725028:
                    {
                        if (client.Entity.Class >= 130 && client.Entity.Class <= 135 || client.Entity.Class >= 140 && client.Entity.Class <= 145)
                            client.AddSpell(new Spell(true) { ID = 5001 });
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                case 725029:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 5030 });
                        break;
                    }
                case 725030:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 5040 });
                        break;
                    }
                case 725031:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 5050 });
                        break;
                    }
                case 725040:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 7000 });
                        break;
                    }
                case 725041:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 7010 });
                        break;
                    }
                case 725042:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 7020 });
                        break;
                    }
                case 725043:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 7030 });
                        break;
                    }
                case 725044:
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.AddSpell(new Spell(true) { ID = 7040 });
                        break;
                    }
                case 1060100:
                    {
                        if (client.Entity.Class >= 140 && client.Entity.Class <= 145 && client.Entity.Level >= 82)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1160 });
                        }
                        break;
                    }
                case 1060101:
                    {
                        if (client.Entity.Class >= 140 && client.Entity.Class <= 145 && client.Entity.Level >= 84)
                        {
                            client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                            client.AddSpell(new Spell(true) { ID = 1165 });
                        }
                        break;
                    }
                #endregion

                #region Demon Boxs
                #region 10cps
                case 720650:
                    {
                        Database.MonsterInformation monster = new Conquer_Online_Server.Database.MonsterInformation();
                        // monster.Boss = true;
                        monster.Hitpoints = 33;
                        monster.Level = 10;
                        monster.Mesh = 368;
                        monster.Name = "Demon";
                        monster.MaxAttack = 100;
                        monster.AttackRange = 5;
                        monster.AttackType = 2;
                        monster.AttackSpeed = 1000;
                        monster.ViewRange = 2;
                        monster.MoveSpeed = 500;
                        monster.RunSpeed = 500;
                        monster.MinAttack = 100;
                        Game.Entity entity = new Game.Entity(Game.EntityFlag.Monster, false);
                        entity.MapObjType = Game.MapObjectType.Monster;
                        entity.MonsterInfo = monster;
                        entity.MonsterInfo.Owner = entity;
                        entity.Name = "Demon";
                        entity.MinAttack = monster.MinAttack;
                        entity.MaxAttack = entity.MagicAttack = monster.MaxAttack;
                        entity.Hitpoints = entity.MaxHitpoints = monster.Hitpoints;
                        entity.Body = monster.Mesh;
                        entity.Level = monster.Level;
                        entity.Defence = 100;
                        entity.X = client.Entity.X;
                        entity.Y = client.Entity.Y;
                        entity.UID = (uint)ServerBase.Kernel.Random.Next(500000, 500050);
                        entity.MapID = client.Entity.MapID;
                        entity.SendUpdates = true;
                        client.Map.RemoveEntity(entity);
                        client.Map.AddEntity(entity);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion
                #region 50cps
                case 720651:
                    {
                        Database.MonsterInformation monster = new Conquer_Online_Server.Database.MonsterInformation();
                        // monster.Boss = true;
                        monster.Hitpoints = 33;
                        monster.Level = 20;
                        monster.Mesh = 369;
                        monster.Name = "AncientDemon";
                        monster.MaxAttack = 100;
                        monster.AttackRange = 5;
                        monster.AttackType = 2;
                        monster.AttackSpeed = 1000;
                        monster.ViewRange = 2;
                        monster.MoveSpeed = 500;
                        monster.RunSpeed = 500;
                        monster.MinAttack = 100;
                        Game.Entity entity = new Game.Entity(Game.EntityFlag.Monster, false);
                        entity.MapObjType = Game.MapObjectType.Monster;
                        entity.MonsterInfo = monster;
                        entity.MonsterInfo.Owner = entity;
                        entity.Name = "AncientDemon";
                        entity.MinAttack = monster.MinAttack;
                        entity.MaxAttack = entity.MagicAttack = monster.MaxAttack;
                        entity.Hitpoints = entity.MaxHitpoints = monster.Hitpoints;
                        entity.Body = monster.Mesh;
                        entity.Level = monster.Level;
                        entity.Defence = 100;
                        entity.X = client.Entity.X;
                        entity.Y = client.Entity.Y;
                        entity.UID = (uint)ServerBase.Kernel.Random.Next(500060, 500110);
                        entity.MapID = client.Entity.MapID;
                        entity.SendUpdates = true;
                        client.Map.RemoveEntity(entity);
                        client.Map.AddEntity(entity);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion
                #region 100cps
                case 720652:
                    {
                        Database.MonsterInformation monster = new Conquer_Online_Server.Database.MonsterInformation();
                        // monster.Boss = true;
                        monster.Hitpoints = 33;
                        monster.Level = 30;
                        monster.Mesh = 377;
                        monster.Name = "FloodDemon";
                        monster.MaxAttack = 100;
                        monster.AttackRange = 5;
                        monster.AttackType = 2;
                        monster.AttackSpeed = 1000;
                        monster.ViewRange = 2;
                        monster.MoveSpeed = 500;
                        monster.RunSpeed = 500;
                        monster.MinAttack = 100;
                        Game.Entity entity = new Game.Entity(Game.EntityFlag.Monster, false);
                        entity.MapObjType = Game.MapObjectType.Monster;
                        entity.MonsterInfo = monster;
                        entity.MonsterInfo.Owner = entity;
                        entity.Name = "FloodDemon";
                        entity.MinAttack = monster.MinAttack;
                        entity.MaxAttack = entity.MagicAttack = monster.MaxAttack;
                        entity.Hitpoints = entity.MaxHitpoints = monster.Hitpoints;
                        entity.Body = monster.Mesh;
                        entity.Level = monster.Level;
                        entity.Defence = 100;
                        entity.X = client.Entity.X;
                        entity.Y = client.Entity.Y;
                        entity.UID = (uint)ServerBase.Kernel.Random.Next(500120, 500160);
                        entity.MapID = client.Entity.MapID;
                        entity.SendUpdates = true;
                        client.Map.RemoveEntity(entity);
                        client.Map.AddEntity(entity);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion
                #region 500cps
                case 720671:
                    {
                        Database.MonsterInformation monster = new Conquer_Online_Server.Database.MonsterInformation();
                        // monster.Boss = true;
                        monster.Hitpoints = 33;
                        monster.Level = 40;
                        monster.Mesh = 400;
                        monster.Name = "HeavenDemon";
                        monster.MaxAttack = 100;
                        monster.AttackRange = 5;
                        monster.AttackType = 2;
                        monster.AttackSpeed = 1000;
                        monster.ViewRange = 2;
                        monster.MoveSpeed = 500;
                        monster.RunSpeed = 500;
                        monster.MinAttack = 100;
                        Game.Entity entity = new Game.Entity(Game.EntityFlag.Monster, false);
                        entity.MapObjType = Game.MapObjectType.Monster;
                        entity.MonsterInfo = monster;
                        entity.MonsterInfo.Owner = entity;
                        entity.Name = "HeavenDemon";
                        entity.MinAttack = monster.MinAttack;
                        entity.MaxAttack = entity.MagicAttack = monster.MaxAttack;
                        entity.Hitpoints = entity.MaxHitpoints = monster.Hitpoints;
                        entity.Body = monster.Mesh;
                        entity.Level = monster.Level;
                        entity.Defence = 100;
                        entity.X = client.Entity.X;
                        entity.Y = client.Entity.Y;
                        entity.UID = (uint)ServerBase.Kernel.Random.Next(500200, 500250);
                        entity.MapID = client.Entity.MapID;
                        entity.SendUpdates = true;
                        client.Map.RemoveEntity(entity);
                        client.Map.AddEntity(entity);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion
                #region 1000cps
                case 720672:
                    {
                        Database.MonsterInformation monster = new Conquer_Online_Server.Database.MonsterInformation();
                        // monster.Boss = true;
                        monster.Hitpoints = 33;
                        monster.Level = 50;
                        monster.Mesh = 404;
                        monster.Name = "ChaosDemon";
                        monster.MaxAttack = 100;
                        monster.AttackRange = 5;
                        monster.AttackType = 2;
                        monster.AttackSpeed = 1000;
                        monster.ViewRange = 2;
                        monster.MoveSpeed = 500;
                        monster.RunSpeed = 500;
                        monster.MinAttack = 100;
                        Game.Entity entity = new Game.Entity(Game.EntityFlag.Monster, false);
                        entity.MapObjType = Game.MapObjectType.Monster;
                        entity.MonsterInfo = monster;
                        entity.MonsterInfo.Owner = entity;
                        entity.Name = "ChaosDemon";
                        entity.MinAttack = monster.MinAttack;
                        entity.MaxAttack = entity.MagicAttack = monster.MaxAttack;
                        entity.Hitpoints = entity.MaxHitpoints = monster.Hitpoints;
                        entity.Body = monster.Mesh;
                        entity.Level = monster.Level;
                        entity.Defence = 100;
                        entity.X = client.Entity.X;
                        entity.Y = client.Entity.Y;
                        entity.UID = (uint)ServerBase.Kernel.Random.Next(500300, 500350);
                        entity.MapID = client.Entity.MapID;
                        entity.SendUpdates = true;
                        client.Map.RemoveEntity(entity);
                        client.Map.AddEntity(entity);
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        break;
                    }
                #endregion
                #endregion
                #region Cpsage
                #region 270cps
                case 720653:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 270;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 270 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 1380cps
                case 720654:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 1380;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 1380 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 1380 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 20cps
                case 720655:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 20;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 20 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 10cps
                case 720656:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 10;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 10 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 5cps
                case 720657:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 5;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 5 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                ///////////////////////////////
                #region 25cps
                case 720658:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 25;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 25 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 35cps
                case 720956:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 35;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 35 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 40cps
                case 720966:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 40;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 40 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 60cps
                case 720967:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 60;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 60 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 50cps
                case 720659:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 50;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 50 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 100cps
                case 720660:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 100;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 100 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 1350cps
                case 720661:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 1350;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 1350 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 1350 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
                        break;
                    }
                #endregion
                #region 6900cps
                case 720662:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 6900;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 6900 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 6900 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                /////////////////////////////////
                #region 50cps
                case 720663:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 50;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 50 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 100cps
                case 720664:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 100;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 100 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 200cps
                case 720665:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 200;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 200 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 2700cps
                case 720666:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 2700;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 2700 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + "have found 2700 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 13800cps
                case 720667:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 13800;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 13800 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 13800 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                ///////////////////////////
                #region 1/6 EXp
                case 720668:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        //client.IncreaseExperience(client.Entity.Experience / 1/6, false);
                        client.IncreaseExperience(client.ExpBall / 1 / 6, false);

                        break;
                    }
                #endregion
                #region 5/6 EXp
                case 720669:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        //client.IncreaseExperience(client.Entity.Experience / 5 / 6, false);
                        client.IncreaseExperience(client.ExpBall / 5 / 6, false);

                        break;
                    }
                #endregion
                #region 1exp and 2/3EXp
                case 720670:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        //client.IncreaseExperience(client.Entity.Experience / 2/ 3, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall / 2 / 3, false);
                        break;
                    }
                #endregion
                ////////////////////////
                #region 250cps
                case 720675:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 250;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 250 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 500cps
                case 720676:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 500;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 500 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 1000cps
                case 720677:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 1000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 1000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 13500cps
                case 720678:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 13500;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 13500 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + "have found 13500 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 69000cps
                case 720679:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 13800;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 69000 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 69000 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 2exp and 1/2EXp
                case 720680:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.IncreaseExperience(client.ExpBall / 1 / 2, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        break;
                    }
                #endregion
                ///////////////////////////
                #region 500cps
                case 720681:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 500;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 500 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 1000cps
                case 720682:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 1000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 1000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 2000cps
                case 720683:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 2000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 2000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 27000cps
                case 720684:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 27000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 27000 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + "have found 27000 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 138000cps
                case 720685:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 138000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 138000 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 138000 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 3exp and 1/2EXp
                case 720686:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.IncreaseExperience(client.ExpBall / 1 / 2, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        break;
                    }
                #endregion
                ///////////////////////////
                #region 1000cps
                case 720687:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 1000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 1000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 2000cps
                case 720688:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 2000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 2000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 4000cps
                case 720689:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 4000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 4000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 54000cps
                case 720690:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 54000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 54000 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + "have found 54000 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 276000cps
                case 720691:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 276000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 276000 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 276000 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 5 Expall
                case 720692:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        //client.IncreaseExperience(client.ExpBall2, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        break;
                    }
                #endregion
                ///////////////////////////
                ///////////////////////////
                #region 2500cps
                case 720693:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 2500;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 2500 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 5000cps
                case 720694:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 5000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 5000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 10000cps
                case 720695:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 10000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 10000 Cps!", System.Drawing.Color.Yellow, 2005));

                        break;
                    }
                #endregion
                #region 135000cps
                case 720696:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 135000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 135000 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + "have found 135000 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 690000cps
                case 720697:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.Entity.ConquerPoints += 690000;
                        client.Entity.Owner.Send(new Network.GamePackets.Message("You have found 690000 Cps!", System.Drawing.Color.Yellow, 2005));
                        ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(" " + client.Entity.Name + " have found 690000 Cps!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

                        break;
                    }
                #endregion
                #region 8 Expall
                case 720698:
                    {

                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                        client.IncreaseExperience(client.ExpBall / 1 / 2, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        client.IncreaseExperience(client.ExpBall, false);
                        break;
                    }
                #endregion
                #endregion 



                #region BeginnerPack

                #region BeginnerPackL1
                case 723753:
                    {
                        if (client.Entity.Level >= 1)
                        {
                            if (client.Inventory.Count < 20)
                            {
                                client.Inventory.Add(722136, 0, 10, true); //Stancher
                                client.Inventory.Add(1001000, 0, 5, true);//Agrypnotic
                                client.Entity.Money += 1500;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723754, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 20 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 1", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL5
                case 723754:
                    {
                        if (client.Entity.Level >= 5)
                        {
                            if (client.Inventory.Count < 29)
                            {

                                client.Inventory.Add(1001000, 0, 5, true);//Agrypnotic
                                client.Entity.Money += 1500;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723755, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 11 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 5", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL10
                case 723755:
                    {
                        if (client.Entity.Level >= 10)
                        {
                            if (client.Inventory.Count < 36)
                            {
                                client.Inventory.Add(723790, 0, 3, true);//AncientPill
                                client.Entity.Money += 5000;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723756, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 4 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 10", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL15
                case 723756:
                    {
                        if (client.Entity.Level >= 15)
                        {
                            if (client.Inventory.Count < 31)
                            {
                                client.Inventory.Add(723790, 0, 5, true);//AncientPill
                                client.Inventory.Add(1060020, 0, 3, true);//TwinCityGate
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723757, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 9 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 15", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL20
                case 723757:
                    {
                        if (client.Entity.Level >= 20)
                        {
                            if (client.Inventory.Count < 34)
                            {
                                client.Entity.ConquerPoints += 100;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723758, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 6 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 20", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL25
                case 723758:
                    {
                        if (client.Entity.Level >= 25)
                        {
                            if (client.Inventory.Count < 34)
                            {
                                client.Inventory.Add(203009, 0, 1, true);//AttackPot(30m)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723759, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 6 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 25", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL30
                case 723759:
                    {
                        if (client.Entity.Level >= 30)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200000, 0, 1, true);//PrayingStone(S)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723760, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 30", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL35
                case 723760:
                    {
                        if (client.Entity.Level >= 35)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(723017, 0, 1, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723761, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 35", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL35
                case 723761:
                    {
                        if (client.Entity.Level >= 35)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(723017, 0, 1, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723762, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 35", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL45
                case 723762:
                    {
                        if (client.Entity.Level >= 45)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200001, 0, 1, true);//PrayingStone(M)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723763, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 45", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL50
                case 723763:
                    {
                        if (client.Entity.Level >= 50)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200002, 0, 1, true);//PrayingStone(L)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723764, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 50", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL55
                case 723764:
                    {
                        if (client.Entity.Level >= 55)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723017, 0, 1, true);//ExpPotion
                                client.Inventory.Add(723584, 0, 1, true);//BlackTulip
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723765, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 55", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL60
                case 723765:
                    {
                        if (client.Entity.Level >= 60)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723700, 0, 1, true);//ExpBall
                                client.Inventory.Add(752001, 0, 1, true);//1Lily
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723766, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 60", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL65
                case 723766:
                    {
                        if (client.Entity.Level >= 65)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(2100025, 0, 1, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723767, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 65", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL70
                case 723767:
                    {
                        if (client.Entity.Level >= 70)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723700, 0, 2, true);//ExpBall
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723768, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 70", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL75
                case 723768:
                    {
                        if (client.Entity.Level >= 75)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200000, 0, 1, true);//PrayingStone(S)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723769, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 75", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL80
                case 723769:
                    {
                        if (client.Entity.Level >= 80)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723700, 0, 2, true);//ExpBall
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723770, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 80", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL90
                case 723770:
                    {
                        if (client.Entity.Level >= 90)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723017, 0, 2, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723771, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 90", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL95
                case 723771:
                    {
                        if (client.Entity.Level >= 95)
                        {
                            if (client.Inventory.Count < 36)
                            {
                                client.Inventory.Add(723268, 0, 1, true);//MeteorBox
                                client.Inventory.Add(723017, 0, 2, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723772, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 4 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 95", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL100
                case 723772:
                    {
                        if (client.Entity.Level >= 100)
                        {
                            if (client.Inventory.Count < 39)
                            {
                                if (client.Entity.Class <= 15)
                                    CheckCommand(new Message("@item WarArmor Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 25)
                                    CheckCommand(new Message("@item WarriorArmorSoulLv100 Fixed 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 45)
                                    CheckCommand(new Message("@item RhinoCoat Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 55)
                                    CheckCommand(new Message("@item NinjaVestSoulLv100 Fixed 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 65)
                                    CheckCommand(new Message("@item FrockOfAges Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else
                                    CheckCommand(new Message("@item RealBacksword Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);

                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723773, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 1 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 100", System.Drawing.Color.Red, Message.TopLeft));
                        break;
                    }
                #endregion
                #region BeginnerPackL105
                case 723773:
                    {
                        if (client.Entity.Level >= 105)
                        {
                            if (client.Inventory.Count < 36)
                            {
                                client.Inventory.Add(1200000, 0, 1, true);//PrayingStone(S)
                                client.Inventory.Add(723017, 0, 2, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723774, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 4 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 105", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL110
                case 723774:
                    {
                        if (client.Entity.Level >= 110)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(751099, 0, 1, true);//PowerEXPBall
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723775, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 6 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 110", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL115
                case 723775:
                    {
                        if (client.Entity.Level >= 115)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                if (client.Entity.Class <= 15)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 25)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 45)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 55)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 65)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else
                                    client.Inventory.Add(700002, 0, 2, true);//PhoenixGem

                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(723776, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 120", System.Drawing.Color.Red, Message.TopLeft));
                        break;
                    }
                #endregion
                #region BeginnerPackL120
                case 723776:
                    {
                        if (client.Entity.Level >= 120)
                        {
                            if (client.Inventory.Count < 39)
                            {
                                client.Inventory.Add(193300, 0, 1, true);//PowerEXPBall
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                            }
                            else
                                client.Send(new Message("You need to make atleast 1 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 120", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion

                #endregion
                // End BeginnerPack .. Say Thx To AhmedGFX ^_^

                case 721158:
                    {
                        if (client.Entity.Level >= 40)
                        {
                            if (client.Proficiencies.ContainsKey(Database.SpellTable.SpellInformations[11005][0].WeaponSubtype))
                                if (client.Proficiencies[Database.SpellTable.SpellInformations[11005][0].WeaponSubtype].Level >= 5)
                                {
                                    client.Inventory.Remove(item, Game.Enums.ItemUse.Remove);
                                    client.AddSpell(new Spell(true) { ID = 11005 });
                                }
                                else
                                {
                                    client.Send(new Message("You need level 5 at Spear proficiency!", System.Drawing.Color.Tan, Message.TopLeft));
                                }
                        }
                        break;
                    }
                case 721157:
                    {
                        if (client.Entity.Level >= 40)
                        {
                            if (client.Proficiencies.ContainsKey(Database.SpellTable.SpellInformations[11000][0].WeaponSubtype))
                                if (client.Proficiencies[Database.SpellTable.SpellInformations[11000][0].WeaponSubtype].Level >= 5)
                                {
                                    client.Inventory.Remove(item, Game.Enums.ItemUse.Remove);
                                    client.AddSpell(new Spell(true) { ID = 11000 });
                                }
                                else
                                {
                                    client.Send(new Message("You need level 5 at Wand proficiency!", System.Drawing.Color.Tan, Message.TopLeft));
                                }
                        }
                        break;
                    }
                // Start NovicePack .. Say Thx To AhmedGFX ^_^
                #region NovicePack

                #region BeginnerPackL1
                case 727026:
                    {
                        if (client.Entity.Level >= 1)
                        {
                            if (client.Inventory.Count < 20)
                            {
                                client.Inventory.Add(722136, 0, 10, true); //Stancher
                                client.Inventory.Add(1001000, 0, 5, true);//Agrypnotic
                                client.Entity.Money += 1500;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727027, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 20 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 1", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL5
                case 727027:
                    {
                        if (client.Entity.Level >= 5)
                        {
                            if (client.Inventory.Count < 29)
                            {

                                client.Inventory.Add(1001000, 0, 5, true);//Agrypnotic
                                client.Entity.Money += 1500;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727028, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 11 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 5", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL10
                case 727028:
                    {
                        if (client.Entity.Level >= 10)
                        {
                            if (client.Inventory.Count < 36)
                            {
                                client.Inventory.Add(723790, 0, 3, true);//AncientPill
                                client.Entity.Money += 5000;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727029, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 4 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 10", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL15
                case 727029:
                    {
                        if (client.Entity.Level >= 15)
                        {
                            if (client.Inventory.Count < 31)
                            {
                                client.Inventory.Add(723790, 0, 5, true);//AncientPill
                                client.Inventory.Add(1060020, 0, 3, true);//TwinCityGate
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727030, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 9 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 15", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL20
                case 727030:
                    {
                        if (client.Entity.Level >= 20)
                        {
                            if (client.Inventory.Count < 34)
                            {
                                client.Entity.ConquerPoints += 100;
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727031, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 6 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 20", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL25
                case 727031:
                    {
                        if (client.Entity.Level >= 25)
                        {
                            if (client.Inventory.Count < 34)
                            {
                                client.Inventory.Add(203009, 0, 1, true);//AttackPot(30m)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727032, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 6 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 25", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL30
                case 727032:
                    {
                        if (client.Entity.Level >= 30)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200000, 0, 1, true);//PrayingStone(S)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727033, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 30", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL35
                case 727033:
                    {
                        if (client.Entity.Level >= 35)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(723017, 0, 1, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727034, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 35", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL40
                case 727034:
                    {
                        if (client.Entity.Level >= 40)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(723017, 0, 1, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727035, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 35", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL45
                case 727035:
                    {
                        if (client.Entity.Level >= 45)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200001, 0, 1, true);//PrayingStone(M)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727036, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 45", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL50
                case 727036:
                    {
                        if (client.Entity.Level >= 50)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200002, 0, 1, true);//PrayingStone(L)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727037, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 50", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL55
                case 727037:
                    {
                        if (client.Entity.Level >= 55)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723017, 0, 1, true);//ExpPotion
                                client.Inventory.Add(723584, 0, 1, true);//BlackTulip
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727038, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 55", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL60
                case 727038:
                    {
                        if (client.Entity.Level >= 60)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723700, 0, 1, true);//ExpBall
                                client.Inventory.Add(752001, 0, 1, true);//1Lily
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727039, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 60", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL65
                case 727039:
                    {
                        if (client.Entity.Level >= 65)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(2100025, 0, 1, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727040, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 65", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL70
                case 727040:
                    {
                        if (client.Entity.Level >= 70)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723700, 0, 2, true);//ExpBall
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727041, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 70", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL75
                case 727041:
                    {
                        if (client.Entity.Level >= 75)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(1200000, 0, 1, true);//PrayingStone(S)
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727042, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 75", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL80
                case 727042:
                    {
                        if (client.Entity.Level >= 80)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723700, 0, 2, true);//ExpBall
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727043, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 80", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL85
                case 727043:
                    {
                        if (client.Entity.Level >= 85)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(723017, 0, 2, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727044, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 3 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 85", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL90
                case 727044:
                    {
                        if (client.Entity.Level >= 95)
                        {
                            if (client.Inventory.Count < 36)
                            {
                                client.Inventory.Add(723268, 0, 1, true);//MeteorBox
                                client.Inventory.Add(723017, 0, 2, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727045, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 4 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 90", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL95
                case 727045:
                    {
                        if (client.Entity.Level >= 95)
                        {
                            if (client.Inventory.Count < 36)
                            {
                                client.Inventory.Add(723268, 0, 1, true);//MeteorBox
                                client.Inventory.Add(723017, 0, 2, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727046, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 4 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 95", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL100
                case 727046:
                    {
                        if (client.Entity.Level >= 100)
                        {
                            if (client.Inventory.Count < 39)
                            {
                                if (client.Entity.Class <= 15)
                                    CheckCommand(new Message("@item WarArmor Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 25)
                                    CheckCommand(new Message("@item WarriorArmorSoulLv100 Fixed 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 45)
                                    CheckCommand(new Message("@item RhinoCoat Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 55)
                                    CheckCommand(new Message("@item NinjaVestSoulLv100 Fixed 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 65)
                                    CheckCommand(new Message("@item FrockOfAges Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);
                                else
                                    CheckCommand(new Message("@item RealBacksword Super 0 0 0 255 255", System.Drawing.Color.Red, 2001), client);

                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727047, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 1 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 100", System.Drawing.Color.Red, Message.TopLeft));
                        break;
                    }
                #endregion
                #region BeginnerPackL105
                case 727047:
                    {
                        if (client.Entity.Level >= 105)
                        {
                            if (client.Inventory.Count < 36)
                            {
                                client.Inventory.Add(1200000, 0, 1, true);//PrayingStone(S)
                                client.Inventory.Add(723017, 0, 2, true);//ExpPotion
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727048, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 4 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 105", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL110
                case 727048:
                    {
                        if (client.Entity.Level >= 110)
                        {
                            if (client.Inventory.Count < 37)
                            {
                                client.Inventory.Add(751099, 0, 1, true);//PowerEXPBall
                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727049, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 6 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 110", System.Drawing.Color.Red, Message.TopLeft));


                        break;
                    }
                #endregion
                #region BeginnerPackL115
                case 727049:
                    {
                        if (client.Entity.Level >= 115)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                if (client.Entity.Class <= 15)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 25)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 45)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 55)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else if (client.Entity.Class <= 65)
                                    client.Inventory.Add(700012, 0, 2, true);//DragonGem
                                else
                                    client.Inventory.Add(700002, 0, 2, true);//PhoenixGem

                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                                client.Inventory.Add(727050, 0, 1, true);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 120", System.Drawing.Color.Red, Message.TopLeft));
                        break;
                    }
                #endregion
                #region BeginnerPackL120
                case 727050:
                    {
                        if (client.Entity.Level >= 120)
                        {
                            if (client.Inventory.Count < 38)
                            {
                                client.Inventory.Add(193300, 0, 1, true);
                                if (client.Entity.Class <= 15)
                                    CheckCommand(new Message("@item FrostBlade Super 6 0 0 0 0", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 25)
                                    CheckCommand(new Message("@item ConquestWand Super 6 0 0 0 0", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 45)
                                    CheckCommand(new Message("@item MagicBow Super 6 0 0 0 0", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 55)
                                    CheckCommand(new Message("@item CrownKatana Super 6 0 0 0 0", System.Drawing.Color.Red, 2001), client);
                                else if (client.Entity.Class <= 65)
                                    CheckCommand(new Message("@item ArhatPrayerBeads Super 6 0 0 0 0", System.Drawing.Color.Red, 2001), client);
                                else
                                    CheckCommand(new Message("@item RealBacksword Super 6 0 0 0 0", System.Drawing.Color.Red, 2001), client);

                                client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Remove);
                            }
                            else
                                client.Send(new Message("You need to make atleast 2 free spots in your inventory.", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        else
                            client.Send(new Message("You must be atleast level 120", System.Drawing.Color.Red, Message.TopLeft));
                        {
                            client.Send(new Message("You must be atleast level 120", System.Drawing.Color.Red, Message.TopLeft));
                        }
                        break;
                    }
                #endregion

                #endregion
            }

        }

        static void PickupItem(FloorItem floorItem, Client.GameState client)
        {
            if (client.Screen.TryGetFloorItem(floorItem.UID, out floorItem) && !client.Trade.InTrade)
            {
                if (client.Entity.X == floorItem.X && client.Entity.Y == floorItem.Y)
                {
                    uint owner = floorItem.Owner == null ? 0 : (uint)floorItem.Owner.Entity.UID;
                    if ((owner != 0 && owner == client.Entity.UID) || owner == 0 || Time32.Now >= floorItem.OnFloor.AddSeconds(ServerBase.Constants.FloorItemAvailableAfter))
                    {
                        goto Jump;
                    }
                    else if (client.Team != null)
                    {
                        if (client.Team.IsTeammate(owner))
                        {
                            if (client.Team.PickupItems && floorItem.ValueType == FloorItem.FloorValueType.Item)
                            {
                                if (floorItem.ItemID != 1088000 && floorItem.ItemID != 1088001)
                                {
                                    goto Jump;
                                }
                            }
                            else if (client.Team.PickupMoney && floorItem.ValueType != FloorItem.FloorValueType.Item)
                            {
                                goto Jump;
                            }
                        }
                    }
                    client.Send(ServerBase.Constants.FloorItemNotAvailable);
                    return;
                Jump:
                    FloorItem pickupAnimation = new FloorItem(true);
                    if (floorItem.ValueType != FloorItem.FloorValueType.Item)
                        client.Map.RemoveFloorItem(floorItem);
                    pickupAnimation.Type = FloorItem.Animation;
                    pickupAnimation.UID = client.Entity.UID;
                    pickupAnimation.X = client.Entity.X;
                    pickupAnimation.Y = client.Entity.Y;
                    if (floorItem.PickedUpAlready)
                        return;
                    floorItem.PickedUpAlready = true;
                    switch (floorItem.ValueType)
                    {
                        case FloorItem.FloorValueType.Item:
                            {
                                if (client.Inventory.Count <= 39)
                                {
                                    client.SendScreen(pickupAnimation, false);
                                    if (floorItem.Item.MobDropped)
                                        client.Inventory.Add(floorItem.Item, Game.Enums.ItemUse.CreateAndAdd);
                                    else
                                        client.Inventory.Add(floorItem.Item, Game.Enums.ItemUse.Add);
                                    floorItem.Type = FloorItem.Remove;
                                    client.RemoveScreenSpawn(floorItem, true);
                                    client.Send(ServerBase.Constants.PickupItem(Database.ConquerItemInformation.BaseInformations[floorItem.Item.ID].Name));
                                }
                                else
                                    client.Send(ServerBase.Constants.FullInventory);
                                break;
                            }
                        case FloorItem.FloorValueType.Money:
                            {
                                client.Send(ServerBase.Constants.PickupGold(floorItem.Value));
                                client.SendScreen(pickupAnimation, false);
                                client.Entity.Money += floorItem.Value;
                                floorItem.Type = FloorItem.Remove;
                                client.RemoveScreenSpawn(floorItem, true);
                                break;
                            }
                        case FloorItem.FloorValueType.ConquerPoints:
                            {
                                client.Send(ServerBase.Constants.PickupConquerPoints(floorItem.Value));
                                client.SendScreen(pickupAnimation, false);
                                client.Entity.ConquerPoints += (uint)Math.Min(floorItem.Value, 300);
                                floorItem.Type = FloorItem.Remove;
                                client.RemoveScreenSpawn(floorItem, true);
                                break;
                            }
                    }
                    return;
                }
            }
        }
        static void DropItem(ItemUsage itemUsage, Client.GameState client)
        {
            Interfaces.IConquerItem item = null;
            if (client.Inventory.TryGetItem(itemUsage.UID, out item))
            {
                if (item.ID == 0)
                    return;
                Database.ConquerItemInformation infos = new Database.ConquerItemInformation(item.ID, 0);
                if (item.Lock != 0 || item.Suspicious)
                    return;
                if (infos.BaseInformation.Type == Database.ConquerItemBaseInformation.ItemType.Dropable && !item.Bound)
                {
                    ushort X = client.Entity.X, Y = client.Entity.Y;
                    if (client.Map.SelectCoordonates(ref X, ref Y))
                    {
                        FloorItem floorItem = new FloorItem(true);
                        floorItem.Item = item;
                        floorItem.ItemID = item.ID;
                        floorItem.ItemColor = item.Color;
                        floorItem.MapID = client.Map.ID;
                        floorItem.MapObjType = Game.MapObjectType.Item;
                        floorItem.X = X;
                        floorItem.Y = Y;
                        floorItem.Type = FloorItem.Drop;
                        floorItem.OnFloor = Time32.Now;
                        floorItem.UID = FloorItem.FloorUID.Next;
                        while (client.Map.Npcs.ContainsKey(floorItem.UID))
                            floorItem.UID = FloorItem.FloorUID.Next;
                        client.SendScreenSpawn(floorItem, true);
                        client.Map.AddFloorItem(floorItem);
                        ushort stack = item.StackSize;
                        item.StackSize = 0;
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Remove);
                        item.StackSize = stack;
                    }
                }
                else
                    client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
            }
        }
        static void DropMoney(ItemUsage itemUsage, Client.GameState client)
        {
            if (client.Entity.Money >= itemUsage.UID)
            {
                ushort X = client.Entity.X, Y = client.Entity.Y;
                if (client.Map.SelectCoordonates(ref X, ref Y))
                {
                    uint ItemID = MoneyItemID(itemUsage.UID);
                    FloorItem floorItem = new FloorItem(true);
                    floorItem.ValueType = FloorItem.FloorValueType.Money;
                    floorItem.Value = itemUsage.UID;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = client.Map.ID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = FloorItem.FloorUID.Next;
                    while (client.Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = FloorItem.FloorUID.Next;
                    client.SendScreenSpawn(floorItem, true);
                    client.Map.AddFloorItem(floorItem);
                    client.Entity.Money -= itemUsage.UID;

                }
            }
        }
        public static uint MoneyItemID(uint value)
        {
            if (value < 100)
                return 1090000;
            else if (value < 399)
                return 1090010;
            else if (value < 5099)
                return 1090020;
            else if (value < 8099)
                return 1091000;
            else if (value < 12099)
                return 1091010;
            else
                return 1091020;
        }

        static void HandleBuyFromNPC(ItemUsage itemUsage, Client.GameState client)
        {

            if (itemUsage == null)
                return;
            if (client == null)
                return;
            if (itemUsage.UID == 141)
            {
                if (itemUsage.dwParam == 721157 || itemUsage.dwParam == 721158)
                {
                    uint cost = 5000;
                    Interfaces.IConquerItem item = new ConquerItem(true);
                    if (cost > client.Entity.Money)
                        return;
                    if (client.Entity.Money - cost > client.Entity.Money)
                        return;
                    item.ID = itemUsage.dwParam;
                    item.Durability = item.MaximDurability = 100;
                    item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                    client.Inventory.Add(item, Game.Enums.ItemUse.CreateAndAdd);
                    if (client.Inventory.ContainsUID(item.UID) || item.StackSize == 1)
                        client.Entity.Money -= cost;
                }
            }
            if (itemUsage.UID == 2888)
            {
                if (itemUsage.dwParam == 203009)
                {
                    uint cost = 1290;
                    if (cost > client.Entity.ConquerPoints)
                        return;
                    if (client.Entity.ConquerPoints - cost > client.Entity.ConquerPoints)
                        return;
                    Interfaces.IConquerItem item = new ConquerItem(true);
                    item.ID = itemUsage.dwParam;

                    item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                    item.Durability = item.MaximDurability = 100;//iteminfo.BaseInformation.Durability;
                    client.Inventory.Add(item, Game.Enums.ItemUse.CreateAndAdd);
                    if (client.Inventory.ContainsUID(item.UID) || item.StackSize == 1)
                        client.Entity.ConquerPoints = (uint)Math.Max(0, (uint)((uint)client.Entity.ConquerPoints - (uint)cost));
                }
            }
            
            
            if (itemUsage.UID == 6000)//honorshop
            {
                if (client.Inventory.Count == 40)
                    return;
                uint price = 0;
                switch (itemUsage.dwParam)
                {
                    case 200006: price = 300000; break;
                    case 200207: price = 500000; break;

                    case 200208: price = 800000; break;
                    case 200007: price = 2000000; break;
                    case 200309: price = 550000; break;
                    case 200308: price = 900000; break;
                    case 200103: price = 1300000; break;
                    case 200107: price = 2000000; break;
                    case 720842: price = 8000; break;
                    case 711083: price = 50000; break;
                    case 720598: price = 20000; break;
                    case 720774: price = 10000; break;
                    case 723651: price = 25000; break;
                    case 723652: price = 62400; break;
                    case 723653: price = 156000; break;
                    case 723654: price = 12500; break;
                    case 723655: price = 31200; break;
                    case 723656: price = 78000; break;
                    case 723657: price = 25000; break;
                    case 723658: price = 62400; break;
                    case 723659: price = 156000; break;
                    case 723660: price = 5000; break;
                    case 723661: price = 12500; break;
                    case 723662: price = 31200; break;
                    case 723663: price = 10000; break;
                    case 723664: price = 25000; break;
                    case 723665: price = 62400; break;
                    case 723666: price = 10000; break;
                    case 723667: price = 25000; break;
                    case 723668: price = 62400; break;
                    case 723669: price = 12500; break;
                    case 723670: price = 31200; break;
                    case 723671: price = 78000; break;
                    case 723672: price = 16000; break;
                    case 723673: price = 41000; break;
                    case 723674: price = 104000; break;
                    case 723675: price = 16000; break;
                    case 723676: price = 41000; break;
                    case 723677: price = 104000; break;
                    case 723678: price = 13000; break;
                    case 723679: price = 33000; break;
                    case 723680: price = 83000; break;
                    case 723681: price = 13000; break;
                    case 723682: price = 33000; break;
                    case 723683: price = 83000; break;
                    case 723690: price = 13000; break;
                    case 723691: price = 33000; break;
                    case 723692: price = 83000; break;
                    case 723684: price = 10000; break;
                    case 723685: price = 25000; break;
                    case 723686: price = 62400; break;
                    case 723130: price = 10000; break;
                    case 723131: price = 25000; break;
                    case 723132: price = 62400; break;
                    case 723133: price = 25000; break;
                    case 723134: price = 62400; break;
                    case 723135: price = 156000; break;
                }
                Interfaces.IConquerItem item = new ConquerItem(true);
                Database.ConquerItemInformation iteminfo = new Conquer_Online_Server.Database.ConquerItemInformation(itemUsage.dwParam, 0);
                if (price > client.ArenaStatistic.CurrentHonor)
                    return;
                if (client.ArenaStatistic.CurrentHonor - price > client.ArenaStatistic.CurrentHonor)
                    return;
                item.ID = itemUsage.dwParam;
                item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                item.Durability = item.MaximDurability = iteminfo.BaseInformation.Durability;
                client.Inventory.Add(item, Game.Enums.ItemUse.CreateAndAdd);
                client.ArenaStatistic.CurrentHonor -= price;
                Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic);
                return;
            }
            Interfaces.INpc npc = null;
            if (client.Map.Npcs.TryGetValue(itemUsage.UID, out npc) || itemUsage.UID == 2888)
            {
                if (client.Inventory.Count == 40)
                    return;
                if (itemUsage.UID != 2888)
                    if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, npc.X, npc.Y) > 17)
                        return;


                Database.ShopFile.Shop shop = new Conquer_Online_Server.Database.ShopFile.Shop();
                if (Database.ShopFile.Shops.TryGetValue(itemUsage.UID, out shop))
                {
                    if (shop.UID == 0)
                        return;
                    if (!shop.Items.Contains(itemUsage.dwParam))
                        return;
                    Database.ConquerItemInformation iteminfo = new Conquer_Online_Server.Database.ConquerItemInformation(itemUsage.dwParam, 0);
                    uint Amount = itemUsage.dwExtraInfo > 0 ? itemUsage.dwExtraInfo : 1;
                    while (Amount > 0 && client.Inventory.Count != 40)
                    {
                        Interfaces.IConquerItem item = new ConquerItem(true);

                        switch (shop.MoneyType)
                        {
                            case Conquer_Online_Server.Database.ShopFile.MoneyType.Gold:
                                {
                                    if (iteminfo.BaseInformation.GoldWorth > client.Entity.Money)
                                        return;
                                    if (client.Entity.Money - iteminfo.BaseInformation.GoldWorth > client.Entity.Money)
                                        return;
                                    item.ID = itemUsage.dwParam;
                                    item.Durability = item.MaximDurability = iteminfo.BaseInformation.Durability;
                                    if (!IsEquipment(item.ID))
                                    {
                                        if (iteminfo.BaseInformation.StackSize != 0)
                                        {
                                            item.StackSize = 1;
                                            item.MaxStackSize = iteminfo.BaseInformation.StackSize;
                                        }
                                    }
                                    item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                                    client.Inventory.Add(item, Game.Enums.ItemUse.CreateAndAdd);
                                    if (client.Inventory.ContainsUID(item.UID) || item.StackSize == 1)
                                        client.Entity.Money -= iteminfo.BaseInformation.GoldWorth;
                                    break;
                                }
                            case Conquer_Online_Server.Database.ShopFile.MoneyType.ConquerPoints:
                                {
                                    if (iteminfo.BaseInformation.ConquerPointsWorth > client.Entity.ConquerPoints)
                                        return;
                                    if (client.Entity.ConquerPoints - iteminfo.BaseInformation.ConquerPointsWorth > client.Entity.ConquerPoints)
                                        return;
                                    item.ID = itemUsage.dwParam;
                                    if (item.ID >= 730001 && item.ID <= 730008)
                                        item.Plus = (byte)(item.ID % 10);
                                    item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                                    item.Durability = item.MaximDurability = iteminfo.BaseInformation.Durability;
                                    if (!IsEquipment(item.ID))
                                    {
                                        if (iteminfo.BaseInformation.StackSize != 0)
                                        {
                                            item.StackSize = 1;
                                            item.MaxStackSize = iteminfo.BaseInformation.StackSize;
                                        }
                                    }
                                    client.Inventory.Add(item, Game.Enums.ItemUse.CreateAndAdd);
                                    if (client.Inventory.ContainsUID(item.UID) || item.StackSize == 1)
                                        client.Entity.ConquerPoints = (uint)Math.Max(0, (uint)((uint)client.Entity.ConquerPoints - (uint)iteminfo.BaseInformation.ConquerPointsWorth));
                                    break;
                                }
                        }
                        Amount--;
                    }
                }
                else
                {
                    if (Database.EShopFile.Shops.TryGetValue(itemUsage.UID, out shop))
                    {
                        if (shop.UID == 0)
                            return;
                        if (!shop.Items.Contains(itemUsage.dwParam))
                            return;
                        Database.ConquerItemInformation iteminfo = new Conquer_Online_Server.Database.ConquerItemInformation(itemUsage.dwParam, 0);
                        uint Amount = itemUsage.dwExtraInfo > 0 ? itemUsage.dwExtraInfo : 1;
                        while (Amount > 0 && client.Inventory.Count != 40)
                        {

                            Interfaces.IConquerItem item = new ConquerItem(true);
                            switch (shop.MoneyType)
                            {
                                case Conquer_Online_Server.Database.ShopFile.MoneyType.Gold:
                                    {
                                        if (iteminfo.BaseInformation.GoldWorth > client.Entity.Money)
                                            return;
                                        if (client.Entity.Money - iteminfo.BaseInformation.GoldWorth > client.Entity.Money)
                                            return;
                                        item.ID = itemUsage.dwParam;
                                        item.Durability = item.MaximDurability = iteminfo.BaseInformation.Durability;

                                        item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                                        client.Inventory.Add(item, Game.Enums.ItemUse.CreateAndAdd);
                                        if (client.Inventory.ContainsUID(item.UID) || item.StackSize == 1)
                                            client.Entity.Money -= iteminfo.BaseInformation.GoldWorth;
                                        break;
                                    }
                                case Conquer_Online_Server.Database.ShopFile.MoneyType.ConquerPoints:
                                    {
                                        if (iteminfo.BaseInformation.ConquerPointsWorth > client.Entity.ConquerPoints)
                                            return;
                                        if (client.Entity.ConquerPoints - iteminfo.BaseInformation.ConquerPointsWorth > client.Entity.ConquerPoints)
                                            return;
                                        item.ID = itemUsage.dwParam;
                                        if (item.ID >= 730001 && item.ID <= 730008)
                                            item.Plus = (byte)(item.ID % 10);
                                        item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                                        item.Durability = item.MaximDurability = iteminfo.BaseInformation.Durability;

                                        client.Inventory.Add(item, Game.Enums.ItemUse.CreateAndAdd);
                                        if (client.Inventory.ContainsUID(item.UID) || item.StackSize == 1)
                                            client.Entity.ConquerPoints = (uint)Math.Max(0, (uint)((uint)client.Entity.ConquerPoints - (int)iteminfo.BaseInformation.ConquerPointsWorth));
                                        break;
                                    }
                            }
                            Amount--;
                        }
                    }
                }
            }
        }
        static void HandleSellToNPC(ItemUsage itemUsage, Client.GameState client)
        {
            Interfaces.INpc npc = null;
            if (client.Map.Npcs.TryGetValue(itemUsage.UID, out npc))
            {
                if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, npc.X, npc.Y) > 17)
                    return;
                Interfaces.IConquerItem item = null;
                if (client.Inventory.TryGetItem(itemUsage.dwParam, out item))
                {
                    if (item.Lock != 0 || item.Suspicious)
                        return;
                    uint Price = new Database.ConquerItemInformation(item.ID, 0).BaseInformation.GoldWorth;
                    Price = Price / 3;
                    if (item.Durability > 0 && item.Durability < item.MaximDurability)
                        Price = (Price * item.Durability) / item.MaximDurability;
                    item.StackSize = 0;
                    if (item.Durability > 0 && item.Durability <= item.MaximDurability)
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                        client.Entity.Money += Price;
                    }
                    else
                    {
                        client.Inventory.Remove(item, Game.Enums.ItemUse.Delete);
                    }
                }
            }
        }
        static void HandleRepair(ItemUsage itemUsage, Client.GameState client)
        {
            Interfaces.IConquerItem item = null;
            if (client.Inventory.TryGetItem(itemUsage.UID, out item))
            {
                if (item.Suspicious)
                    return;
                if (IsArrow(item.ID))
                    return;
                if (item.Durability > 0 && item.Durability < item.MaximDurability)
                {
                    uint Price = new Database.ConquerItemInformation(item.ID, 0).BaseInformation.GoldWorth;
                    byte Quality = (byte)(item.ID % 10);
                    double QualityMultipier = 0;

                    switch (Quality)
                    {
                        case 9: QualityMultipier = 1.125; break;
                        case 8: QualityMultipier = 0.975; break;
                        case 7: QualityMultipier = 0.9; break;
                        case 6: QualityMultipier = 0.825; break;
                        default: QualityMultipier = 0.75; break;
                    }

                    int nRepairCost = 0;
                    if (Price > 0)
                        nRepairCost = (int)Math.Ceiling((Price * (item.MaximDurability - item.Durability) / item.MaximDurability) * QualityMultipier);

                    nRepairCost = Math.Max(1, nRepairCost);
                    if (client.Entity.Money >= nRepairCost)
                    {
                        client.Entity.Money -= (uint)nRepairCost;
                        item.Durability = item.MaximDurability;
                        item.Mode = Game.Enums.ItemMode.Update;
                        item.Send(client);
                        Database.ConquerItemTable.UpdateDurabilityItem(item);
                    }
                }
                else if (item.Durability == 0)
                {
                    if (client.Inventory.Remove(1088001, 5))
                    {
                        item.Durability = item.MaximDurability;
                        item.Mode = Game.Enums.ItemMode.Update;
                        item.Send(client);
                        Database.ConquerItemTable.UpdateDurabilityItem(item);
                    }
                }
            }
        }

        static void UpgradeItem(ItemUsage itemUsage, Client.GameState client)
        {
            Interfaces.IConquerItem item = null;
            if (client.Inventory.TryGetItem(itemUsage.UID, out item))
            {
                if (IsArrow(item.ID))
                    return;
                Interfaces.IConquerItem upgrade = null;
                if (client.Inventory.TryGetItem(itemUsage.dwParam, out upgrade))
                {
                    Database.ConquerItemInformation infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                    switch (upgrade.ID)
                    {
                        case 1088000:
                            {
                                if (item.ID % 10 == (byte)Game.Enums.ItemQuality.Super)
                                    break;
                                byte chance = (byte)(100 - ((infos.BaseInformation.Level - (infos.BaseInformation.Level > 100 ? 30 : 0)) / (10 - item.ID % 10)));
                                if (item.Durability < item.MaximDurability)
                                    break;
                                if (ServerBase.Kernel.Rate(chance))
                                {
                                    switch ((Game.Enums.ItemQuality)(item.ID % 10))
                                    {
                                        case Game.Enums.ItemQuality.Normal:
                                        case Game.Enums.ItemQuality.NormalV1:
                                        case Game.Enums.ItemQuality.NormalV2:
                                        case Game.Enums.ItemQuality.NormalV3: item.ID = (item.ID - (item.ID % 10)) + (byte)Game.Enums.ItemQuality.Refined; break;
                                        default: item.ID++; break;
                                    }
                                    Database.ConquerItemTable.UpdateItemID(item, client);
                                    item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                    item.Send(client);
                                }
                                else
                                {
                                    item.Durability = (ushort)(item.Durability / 2);
                                    Database.ConquerItemTable.UpdateDurabilityItem(item);
                                    item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                    item.Send(client);
                                }
                                //item = ItemSocket(item, 1, client);
                                client.Inventory.Remove(upgrade, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                break;
                            }
                        case 1088001:
                            {
                                if (infos.BaseInformation.Level > 120)
                                    break;
                                byte chance = 100;
                                chance -= (byte)(infos.BaseInformation.Level / 10 * 3);
                                chance -= (byte)(((item.ID % 10) + 1) * 3);
                                if (item.Durability < item.MaximDurability)
                                    break;
                                uint newid = infos.CalculateUplevel();
                                if (newid != 0 && newid != item.ID)
                                {
                                    if (ServerBase.Kernel.Rate(chance))
                                    {
                                        item.ID = newid;
                                        infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                                        item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                                        Database.ConquerItemTable.UpdateItemID(item, client);
                                        Database.ConquerItemTable.UpdateDurabilityItem(item);
                                        item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                        item.Send(client);
                                    }
                                    else
                                    {
                                        item.Durability = (ushort)(item.Durability / 2);
                                        Database.ConquerItemTable.UpdateDurabilityItem(item);
                                        item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                        item.Send(client);
                                    }
                                    //item = ItemSocket(item, 2, client);
                                    client.Inventory.Remove(upgrade, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                }
                                break;
                            }
                        case 1088002:
                            {
                                ushort postion = ItemPosition(infos.BaseInformation.ID);
                                byte level = ItemMinLevel(postion);
                                if (postion == 6)
                                {
                                    if (infos.BaseInformation.ID.ToString()[2] == '1')
                                        level = 13;
                                }
                                if (infos.BaseInformation.Level <= level)
                                {
                                    client.Send(new Message("This item's level is too low. It can't be downgraded more.", System.Drawing.Color.MistyRose, Message.TopLeft));
                                    break;
                                }
                                byte chance = 100;
                                chance -= (byte)(infos.BaseInformation.Level / 10 * 4);
                                chance -= (byte)(((item.ID % 10) + 1) * 3);
                                if (item.Durability < item.MaximDurability)
                                    break;
                                uint newid = infos.CalculateDownlevel();
                                if (newid != 0 && newid != item.ID)
                                {
                                    if (ServerBase.Kernel.Rate(chance))
                                    {
                                        item.ID = newid;
                                        infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                                        item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                                        Database.ConquerItemTable.UpdateItemID(item, client);
                                        Database.ConquerItemTable.UpdateDurabilityItem(item);
                                        item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                        item.Send(client);
                                    }
                                    else
                                    {
                                        item.Durability = (ushort)(item.Durability / 2);
                                        Database.ConquerItemTable.UpdateDurabilityItem(item);
                                        item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                        item.Send(client);
                                    }
                                    item = ItemSocket(item, 2, client);
                                    client.Inventory.Remove(upgrade, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                }
                                break;
                            }
                    }
                }
            }
        }
        static Interfaces.IConquerItem ItemSocket(Interfaces.IConquerItem item, byte type, Client.GameState client)
        {
            if (type == 1)
                item.SocketProgress += 10;
            else
                item.SocketProgress += 5;
            ushort need = 0;
            if (item.SocketOne == Game.Enums.Gem.NoSocket && item.SocketTwo == Game.Enums.Gem.NoSocket)
                need = ServerBase.Constants.SocketOneProgress;
            else if (item.SocketOne != Game.Enums.Gem.NoSocket && item.SocketTwo == Game.Enums.Gem.NoSocket)
                need = ServerBase.Constants.SocketTwoProgress;
            if (item.SocketProgress >= need && need != 0)
            {
                item.SocketProgress -= need;
                if (need == ServerBase.Constants.SocketOneProgress)
                    item.SocketOne = Game.Enums.Gem.EmptySocket;
                else
                    item.SocketTwo = Game.Enums.Gem.EmptySocket;
                Database.ConquerItemTable.UpdateSockets(item, client);
            }
            Database.ConquerItemTable.UpdateSocketProgress(item, client);
            return item;
        }
        public static byte ItemMaxLevel(ushort postion)
        {
            switch (postion)
            {
                case 0: return 0;
                case ConquerItem.Head: return 140;
                case ConquerItem.Necklace: return 139;
                case ConquerItem.Armor: return 140;
                case ConquerItem.LeftWeapon: return 140;
                case ConquerItem.RightWeapon: return 140;
                case ConquerItem.Boots: return 129;
                case ConquerItem.Ring: return 136;
                case ConquerItem.Tower: return 100;
                case ConquerItem.Fan: return 100;
                case ConquerItem.Steed: return 0;
            }
            return 0;
        }
        public static byte ItemMinLevel(ushort postion)
        {
            switch (postion)
            {
                case 0: return 0;
                case ConquerItem.Head: return 15;
                case ConquerItem.Necklace: return 7;
                case ConquerItem.Armor: return 15;
                case ConquerItem.LeftWeapon: return 15;
                case ConquerItem.RightWeapon: return 15;
                case ConquerItem.Boots: return 10;
                case ConquerItem.Ring: return 10;
                case ConquerItem.Tower: return 0;
                case ConquerItem.Fan: return 0;
                case ConquerItem.Steed: return 0;
                case ConquerItem.Garment: return 0;
                case ConquerItem.RidingCrop: return 0;
            }
            return 0;
        }
        public static ushort ItemPosition(uint ID)
        {
            if (ID >= 203003 && ID <= 203009)
                return ConquerItem.RidingCrop;
            if (ID >= 200000 && ID <= 200420)
                return ConquerItem.SteedMount;


            if (ID >= 350001 && ID <= 370015)
                return ConquerItem.RightWeaponAccessory;
            if (ID >= 380001 && ID <= 380015)
                return ConquerItem.LeftWeaponAccessory;

            if (ID == 134155 || ID == 131155 || ID == 133155 || ID == 130155)
                return ConquerItem.Garment;

            if ((ID >= 111003 && ID <= 118309) || (ID >= 123000 && ID <= 123309) || (ID >= 141003 && ID <= 143309))
                return ConquerItem.Head;

            else if (ID >= 120001 && ID <= 121269)
                return ConquerItem.Necklace;

            else if (ID >= 130003 && ID <= 137073)
                return ConquerItem.Armor;

            else if (ID >= 150000 && ID <= 152279)
                return ConquerItem.Ring;

            else if (ID >= 160013 && ID <= 160249)
                return ConquerItem.Boots;

            else if (ID >= 181305 && ID <= 194300)
                return ConquerItem.Garment;

            else if (ID >= 201003 && ID <= 201009)
                return ConquerItem.Fan;

            else if (ID >= 202003 && ID <= 202009)
                return ConquerItem.Tower;

            else if (ID == 300000)
                return ConquerItem.Steed;
            else if (ID >= 410003 && ID <= 611439)
                return ConquerItem.RightWeapon;
            else if ((ID >= 900000 && ID <= 900309) || (ID >= 1050000 && ID <= 1051000))
                return ConquerItem.LeftWeapon;
            else if (ID >= 2100025 && ID <= 2100095)
                return ConquerItem.Bottle;
            return 0;
        }
        public static bool IsArrow(uint ID)
        {
            if (ID >= 1050000 && ID <= 1051000)
                return true;
            return false;
        }
        public static bool IsTwoHand(uint ID)
        {
            return (ID.ToString()[0] == '5' ? true : false);
        }
        public static bool IsAccessory(uint ID)
        {
            return ID >= 350001 && ID <= 380015;
        }
        static void EquipItem(ItemUsage itemUsage, Client.GameState client)
        {

            Interfaces.IConquerItem item = null;
            client.Entity.AttackPacket = null;
            if (client.Inventory.TryGetItem(itemUsage.UID, out item))
            {
                if (item.Suspicious)
                    return;
                switch (item.ID)
                {
                    case 1200000:
                    case 1200001:
                    case 1200002:
                        {
                            UseItem(item, client);
                            return;

                        }
                    default:
                        {
                            if (itemUsage.dwParam == 17 && ItemPosition(item.ID) == 0)
                            {
                                UseItem(item, client);
                                return;
                            }
                            else if (itemUsage.dwParam == 0 && ItemPosition(item.ID) == 0)
                            {
                                UseItem(item, client);
                                return;
                            }
                            break;
                        }
                }
                #region Sanity checks

                bool can2hand = false;
                bool can2wpn = false;
                if (client.Entity.Class >= 11 && client.Entity.Class <= 65)
                    can2hand = true;
                if (client.Entity.Class >= 11 && client.Entity.Class <= 15 || client.Entity.Class >= 51 && client.Entity.Class <= 55 || client.Entity.Class >= 61 && client.Entity.Class <= 65)
                    can2wpn = true;
                if (!Equipable(item, client))
                    return;
                if (ItemPosition(item.ID) == 5)
                {
                    itemUsage.dwParam = 5;
                    if (!can2hand && !can2wpn)
                        return;
                    if (client.Equipment.Free(4) || (client.Equipment.TryGetItem(4).ID / 1000 != 500 && IsArrow(item.ID)))
                        return;
                }
                if (ItemPosition(item.ID) == 4)
                {
                    if (itemUsage.dwParam == 5)
                        if (!can2hand || !can2wpn)
                            return;
                }
                if (!((itemUsage.dwParam == 4 || itemUsage.dwParam == 5) && (ItemPosition(item.ID) == 4 || ItemPosition(item.ID) == 5)))
                {
                    if (!IsAccessory(item.ID))
                        itemUsage.dwParam = ItemPosition(item.ID);
                }
                bool twohand = IsTwoHand(item.ID);
                if (!twohand && itemUsage.dwParam == 4)
                    if (!client.Equipment.Free(5))
                        if (client.Inventory.Count < 40)
                        {
                            if (IsArrow(client.Equipment.TryGetItem(5).ID))
                                client.Equipment.Remove(5);
                            else
                            {
                                if (client.Equipment.TryGetItem(4) != null)
                                {
                                    if (IsTwoHand(client.Equipment.TryGetItem(4).ID))
                                        client.Equipment.Remove(4);
                                }
                            }
                        }
                #endregion

                if (client.Map.ID == 1039)
                    client.Entity.AttackPacket = null;

                item.Position = (ushort)itemUsage.dwParam;
                if (ItemPosition(item.ID) == 5 && !IsArrow(item.ID))
                {
                    if (IsTwoHand(client.Equipment.TryGetItem(4).ID))
                    {
                        if (!client.Spells.ContainsKey(10311))//Perseverance
                        {
                            client.Send(new Message("You need to know Perseverance (Warrior Pure skill) to be able to wear 2-handed weapon and shield.", System.Drawing.Color.White, Message.Talk));
                            return;
                        }
                    }
                }

                Database.ConquerItemTable.ClearPosition(client.Entity.UID, (byte)itemUsage.dwParam);
                client.Inventory.Remove(item, Game.Enums.ItemUse.Move);
                if (client.Equipment.Free((byte)itemUsage.dwParam))
                {
                    if (twohand)
                        client.Equipment.Remove(5);
                    item.Position = (byte)itemUsage.dwParam;
                    client.Equipment.Add(item);
                    item.Mode = Game.Enums.ItemMode.Update;
                    item.Send(client);
                }
                else
                {
                    if (twohand)
                        client.Equipment.Remove(5);
                    client.Equipment.Remove((byte)itemUsage.dwParam);
                    item.Position = (byte)itemUsage.dwParam;
                    client.Equipment.Add(item);
                }
                client.CalculateStatBonus();
                client.CalculateHPBonus();
                client.SendStatMessage();
                EntityEquipment equips = new EntityEquipment(true);
                equips.ParseHero(client);
                client.Send(equips);
            }
        }
        static void UnequipItem(ItemUsage usage, Client.GameState client)
        {
            if (client.Equipment.Remove((byte)usage.dwParam))
            {
                if (client.Map.ID == 1039)
                    client.Entity.AttackPacket = null;
                client.CalculateStatBonus();
                client.CalculateHPBonus();
                client.SendStatMessage();
                EntityEquipment equips = new EntityEquipment(true);
                equips.ParseHero(client);
                client.Send(equips);
            }
        }

        static bool EquipPassLvlReq(Database.ConquerItemBaseInformation baseInformation, Client.GameState client)
        {
            if (client.Entity.Level < baseInformation.Level)
                return false;
            else
                return true;
        }
        static bool EquipPassRbReq(Database.ConquerItemBaseInformation baseInformation, Client.GameState client)
        {
            if (baseInformation.Level < 71 && client.Entity.Reborn > 0 && client.Entity.Level >= 70)
                return true;
            else
                return false;
        }
        static bool EquipPassStatsReq(Database.ConquerItemBaseInformation baseInformation, Client.GameState client)
        {
            if (client.Entity.Strength >= baseInformation.Strength && client.Entity.Agility >= baseInformation.Agility)
                return true;
            else
                return false;
        }
        static bool EquipPassJobReq(Database.ConquerItemBaseInformation baseInformation, Client.GameState client)
        {
            switch (baseInformation.Class)
            {
                #region Trojan
                case 10: if (client.Entity.Class <= 15 && client.Entity.Class >= 10) return true; break;
                case 11: if (client.Entity.Class <= 15 && client.Entity.Class >= 11) return true; break;
                case 12: if (client.Entity.Class <= 15 && client.Entity.Class >= 12) return true; break;
                case 13: if (client.Entity.Class <= 15 && client.Entity.Class >= 13) return true; break;
                case 14: if (client.Entity.Class <= 15 && client.Entity.Class >= 14) return true; break;
                case 15: if (client.Entity.Class == 15) return true; break;
                #endregion
                #region Warrior
                case 20: if (client.Entity.Class <= 25 && client.Entity.Class >= 20) return true; break;
                case 21: if (client.Entity.Class <= 25 && client.Entity.Class >= 21) return true; break;
                case 22: if (client.Entity.Class <= 25 && client.Entity.Class >= 22) return true; break;
                case 23: if (client.Entity.Class <= 25 && client.Entity.Class >= 23) return true; break;
                case 24: if (client.Entity.Class <= 25 && client.Entity.Class >= 24) return true; break;
                case 25: if (client.Entity.Class == 25) return true; break;
                #endregion
                #region Archer
                case 40: if (client.Entity.Class <= 45 && client.Entity.Class >= 40) return true; break;
                case 41: if (client.Entity.Class <= 45 && client.Entity.Class >= 41) return true; break;
                case 42: if (client.Entity.Class <= 45 && client.Entity.Class >= 42) return true; break;
                case 43: if (client.Entity.Class <= 45 && client.Entity.Class >= 43) return true; break;
                case 44: if (client.Entity.Class <= 45 && client.Entity.Class >= 44) return true; break;
                case 45: if (client.Entity.Class == 45) return true; break;
                #endregion
                #region Ninja
                case 50: if (client.Entity.Class <= 55 && client.Entity.Class >= 50) return true; break;
                case 51: if (client.Entity.Class <= 55 && client.Entity.Class >= 51) return true; break;
                case 52: if (client.Entity.Class <= 55 && client.Entity.Class >= 52) return true; break;
                case 53: if (client.Entity.Class <= 55 && client.Entity.Class >= 53) return true; break;
                case 54: if (client.Entity.Class <= 55 && client.Entity.Class >= 54) return true; break;
                case 55: if (client.Entity.Class == 55) return true; break;
                #endregion
                #region Monk
                case 60: if (client.Entity.Class <= 65 && client.Entity.Class >= 60) return true; break;
                case 61: if (client.Entity.Class <= 65 && client.Entity.Class >= 61) return true; break;
                case 62: if (client.Entity.Class <= 65 && client.Entity.Class >= 62) return true; break;
                case 63: if (client.Entity.Class <= 65 && client.Entity.Class >= 63) return true; break;
                case 64: if (client.Entity.Class <= 65 && client.Entity.Class >= 64) return true; break;
                case 65: if (client.Entity.Class == 65) return true; break;
                #endregion
                #region Taoist
                case 190: if (client.Entity.Class >= 100) return true; break;
                #endregion
                case 0: return true;
                default: return false;
            }
            return false;
        }
        static bool EquipPassSexReq(Database.ConquerItemBaseInformation baseInformation, Client.GameState client)
        {
            int ClientGender = client.Entity.Body % 10000 < 1005 ? 1 : 2;
            if (baseInformation.Gender == 2 && ClientGender == 2)
                return true;
            if (baseInformation.Gender != 2)
                return true;
            return false;
        }
        static bool Equipable(Interfaces.IConquerItem item, Client.GameState client)
        {
            Database.ConquerItemBaseInformation BaseInformation = new Database.ConquerItemInformation(item.ID, item.Plus).BaseInformation;
            bool pass = false;
            if (!EquipPassSexReq(BaseInformation, client))
                return false;
            if (EquipPassRbReq(BaseInformation, client))
                pass = true;
            else
                if (EquipPassJobReq(BaseInformation, client)) if (EquipPassStatsReq(BaseInformation, client)) if (EquipPassLvlReq(BaseInformation, client)) pass = true;
            if (!pass)
                return false;

            if (client.Entity.Reborn > 0)
            {
                if (client.Entity.Level >= 70 && BaseInformation.Level <= 70)
                    return pass;
                else
                {
                    Interfaces.ISkill proficiency = null;
                    client.Proficiencies.TryGetValue((ushort)(item.ID / 1000), out proficiency);
                    if (proficiency != null)
                    {
                        if (proficiency.Level >= BaseInformation.Proficiency)
                            pass = true;
                        else
                            pass = false;
                    }
                }
            }
            else
            {
                if (!IsArrow(item.ID))
                {
                    Interfaces.ISkill proficiency = null;
                    client.Proficiencies.TryGetValue((ushort)(item.ID / 1000), out proficiency);
                    if (proficiency != null)
                    {
                        if (proficiency.Level >= BaseInformation.Proficiency)
                            pass = true;
                        else
                            pass = false;
                    }
                }
            }
            return pass;
        }

        #endregion
        #region Chat
        static void Chat(Message message, Client.GameState client)
        {
            //Console.WriteLine("[" + client.Entity.Name + "][Chat] " + message.__Message);
            if (!CheckCommand(message, client))
            {
                if (message.ChatType != Message.Service)
                {
                    if (client.ChatBanned)
                        if (DateTime.Now > client.ChatBanTime.AddMinutes(client.ChatBanLasts))
                            client.ChatBanned = false;
                        else
                        {
                            int minutes = (int)new TimeSpan((client.ChatBanTime.AddMinutes(client.ChatBanLasts) - DateTime.Now).Ticks).TotalMinutes;
                            client.Send(new Message("You are banned from chat. You have to wait: " + minutes + " minutes before you can speak again!", System.Drawing.Color.Green, Message.Talk));
                            return;
                        }
                }
                switch (message.ChatType)
                {
                    case Message.HawkMessage:
                        {
                            if (client.Booth != null)
                            {
                                client.Booth.HawkMessage = message;
                                client.SendScreen(message, true);
                            }
                            break;
                        }
                    case Message.Talk:
                        {
                         
                            client.SendScreen(message, false);
                            break;
                        }
                    case Message.Whisper:
                        {

                            var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                            varr.MoveNext();
                            int COunt = ServerBase.Kernel.GamePool.Count;
                            for (uint x = 0;
                                x < COunt;
                                x++)
                            {
                                if (x >= COunt) break;

                                Client.GameState pClient = (varr.Current as Client.GameState);

                                if (pClient.Entity.Name == message._To)
                                {
                                    message.Mesh = client.Entity.Mesh;
                                    pClient.Send(message);
                                    return;
                                }
                                varr.MoveNext();

                            }
                            foreach (Game.ConquerStructures.Society.Friend friend in client.Friends.Values)
                            {
                                if (friend.Name == message._To)
                                {
                                    message.__Message = message.__Message.Replace("'", "¹");
                                    client.OnMessageBoxEventParams = new object[3];
                                    client.OnMessageBoxEventParams[0] = client.Entity.UID;
                                    client.OnMessageBoxEventParams[1] = friend.ID;
                                    client.OnMessageBoxEventParams[2] = Console.TimeStamp() + message.__Message;
                                    client.OnMessageBoxOK =
                                        delegate
                                        {
                                            Database.KnownPersons.UpdateMessageOnFriend(Convert.ToUInt32(client.OnMessageBoxEventParams[0]), Convert.ToUInt32(client.OnMessageBoxEventParams[1]), Convert.ToString(client.OnMessageBoxEventParams[2]));
                                            client.Send(new Message("Message sent!", System.Drawing.Color.Green, Message.TopLeft));
                                        };
                                    client.OnMessageBoxCANCEL =
                                        delegate
                                        {
                                            client.OnMessageBoxEventParams = new object[0];
                                        };
                                    client.Send(new NpcReply(NpcReply.MessageBox, "To " + friend.Name + ": \r\n" + message.__Message + "\r\n\r\nSend? (It will replace other messages.)"));
                                    return;
                                }
                            }
                            client.Send(new Message("The player is not online.", System.Drawing.Color.Orange, Message.Service));
                            break;
                        }
                    case Message.Service:
                        {
                            var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                            varr.MoveNext();
                            int COunt = ServerBase.Kernel.GamePool.Count;
                            for (uint x = 0;
                                x < COunt;
                                x++)
                            {
                                if (x >= COunt) break;

                                Client.GameState pClient = (varr.Current as Client.GameState);
                                if (pClient.Account.State == Database.AccountTable.AccountState.GameMaster || pClient.Account.State == Database.AccountTable.AccountState.ProjectManager)
                                {
                                    message.ChatType = Message.Talk;
                                    string _Message = "Service-> " + client.Entity.Name + " needs your help. Respond to him/her right now!!!";
                                    message.__Message = _Message;
                                    message.ChatType = Message.BroadcastMessage;
                                    pClient.Send(message);
                                    return;
                                }
                                varr.MoveNext();

                            }
                            break;
                        }
                    case Message.World:
                        {
                            if (client.Entity.Level >= 70 || client.Entity.Reborn != 0)
                            {
                                var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                varr.MoveNext();
                                int COunt = ServerBase.Kernel.GamePool.Count;
                                for (uint x = 0;
                                    x < COunt;
                                    x++)
                                {
                                    if (x >= COunt) break;

                                    Client.GameState pClient = (varr.Current as Client.GameState);
                                    if (pClient != null) if (pClient.Entity.UID != client.Entity.UID)
                                            pClient.Send(message);
                                    varr.MoveNext();

                                }
                            }
                            break;
                        }
                    case Message.Guild:
                        {
                            if (client.Guild != null)
                            {
                                foreach (Game.ConquerStructures.Society.Guild.Member member in client.Guild.Members.Values)
                                {
                                    if (member.IsOnline)
                                    {
                                        member.Client.Send(message);
                                    }
                                }
                            }
                            break;
                        }
                    case Message.Team:
                        {
                            if (client.Team != null)
                            {
                                foreach (Client.GameState Client in client.Team.Teammates)
                                {
                                    if (client.Entity.UID != Client.Entity.UID)
                                        Client.Send(message);
                                }
                            }
                            break;
                        }
                    case Message.Friend:
                        {
                            foreach (Game.ConquerStructures.Society.Friend friend in client.Friends.Values)
                            {
                                if (friend.IsOnline)
                                    friend.Client.Send(message);
                            }
                            break;
                        }
                    default:
                        {
                            client.SendScreen(message, true);
                            break;
                        }

                }
            }
        }
     public static bool CheckCommand(Message message, Client.GameState client)
        {
            try
            {
                if (message.__Message.StartsWith("@"))
                {
                    string Message = message.__Message.Substring(1).ToLower();
                    string Mess = message.__Message.Substring(1);
                    string[] Data = Message.Split(' ');
                    #region GMs PMs
                    if (client.Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.ProjectManager)
                    {
                        switch (Data[0])
                        {
                            case "zzzzzzzzzzzzzzz":
                                {
                                    byte[] date = new byte[32]
                                    {
                                        
//Packet Nr 1150. Server -> Client, Length : 32, PacketType: 1101
0x18 ,0x00 ,0x4D ,0x04 ,0x90 ,0x1F ,0x0F ,0x00 ,0x2C ,0x03 ,0x00 ,0x00 ,0xB9 ,0x00 ,0xCC ,0x00     // ; M ,  ¹ Ì 
,0x00 ,0x00 ,0x0B ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x54 ,0x51 ,0x53 ,0x65 ,0x72 ,0x76 ,0x65 ,0x72      //;       TQServer
                                    };
                                    Writer.WriteUInt16(client.Entity.X, 12, date);
                                    Writer.WriteUInt16(client.Entity.Y, 14, date);
                                    Writer.WriteUInt16(ushort.Parse(Data[1]), 8, date);//812
                                    client.Send(date);

                                    break;
                                }
                            #region stufff
                            case "stuff":
                                {
                                    switch (Data[1])
                                    {
                                        case "ninja":
                                            {
                                                client.Inventory.Add50(123309, 12, 1);
                                                /////////wepons
                                                client.Inventory.Add50(601439, 12, 1);
                                                client.Inventory.Add50(601439, 12, 1);
                                                /////////arrmor&head gers////////////
                                                client.Inventory.Add50(150269, 12, 1);
                                                client.Inventory.Add50(120269, 12, 1);
                                                client.Inventory.Add50(160249, 12, 1);
                                                ///////////ring$neklas$boots
                                                client.Inventory.Add50(202009, 12, 1);
                                                client.Inventory.Add50(201009, 12, 1);
                                                break;
                                            }
                                        case "monk":
                                            {
                                                client.Inventory.Add50(136309, 12, 1);
                                                client.Inventory.Add50(143309, 12, 1);
                                                /////////wepons
                                                client.Inventory.Add50(610439, 12, 1);
                                                client.Inventory.Add50(610439, 12, 1);
                                                /////////arrmor&head gers////////////
                                                client.Inventory.Add50(150269, 12, 1);
                                                client.Inventory.Add50(120269, 12, 1);
                                                client.Inventory.Add50(160249, 12, 1);
                                                ///////////ring$neklas$boots
                                                client.Inventory.Add50(202009, 12, 1);
                                                client.Inventory.Add50(201009, 12, 1);
                                                break;
                                            }
                                        case "toist":
                                            {
                                                client.Inventory.Add60(134309, 12, 1);
                                                client.Inventory.Add60(114309, 12, 1);
                                                /////////wepons
                                                client.Inventory.Add60(421439, 12, 1);
                                                // client.Inventory.Add50(610439, 12, 1);
                                                /////////arrmor&head gers////////////
                                                client.Inventory.Add60(121269, 12, 1);
                                                client.Inventory.Add60(152279, 12, 1);
                                                client.Inventory.Add60(160249, 12, 1);
                                                ///////////ring$neklas$boots
                                                client.Inventory.Add60(202009, 12, 1);
                                                client.Inventory.Add60(201009, 12, 1);
                                                break;
                                            }
                                        case "worrior":
                                            {
                                                client.Inventory.Add50(131309, 12, 1);
                                                client.Inventory.Add50(141309, 12, 1);
                                                /////////wepons
                                                client.Inventory.Add50(410439, 12, 1);
                                                client.Inventory.Add50(900309, 12, 1);
                                                client.Inventory.Add50(480439, 12, 1);
                                                client.Inventory.Add50(420439, 12, 1);
                                                /////////arrmor&head gers////////////
                                                client.Inventory.Add50(150269, 12, 1);
                                                client.Inventory.Add50(120269, 12, 1);
                                                client.Inventory.Add50(160249, 12, 1);
                                                ///////////ring$neklas$boots
                                                client.Inventory.Add50(202009, 12, 1);
                                                client.Inventory.Add50(201009, 12, 1);
                                                break;
                                            }
                                        case "trojan":
                                            {
                                                client.Inventory.Add50(130309, 12, 1);
                                                client.Inventory.Add50(118309, 12, 1);
                                                /////////wepons
                                                client.Inventory.Add50(410439, 12, 1);
                                                client.Inventory.Add50(480439, 12, 1);
                                                client.Inventory.Add50(420439, 12, 1);
                                                /////////arrmor&head gers////////////
                                                client.Inventory.Add50(150269, 12, 1);
                                                client.Inventory.Add50(120269, 12, 1);
                                                client.Inventory.Add50(160249, 12, 1);
                                                ///////////ring$neklas$boots
                                                client.Inventory.Add50(202009, 12, 1);
                                                client.Inventory.Add50(201009, 12, 1);
                                                break;
                                            }
                                        case "archer":
                                            {
                                                client.Inventory.Add50(113309, 12, 1);
                                                client.Inventory.Add50(133309, 12, 1);
                                                /////////wepons
                                                client.Inventory.Add50(500429, 12, 1);
                                                /////////arrmor&head gers////////////
                                                client.Inventory.Add50(150269, 12, 1);
                                                client.Inventory.Add50(120269, 12, 1);
                                                client.Inventory.Add50(160249, 12, 1);
                                                ///////////ring$neklas$boots
                                                client.Inventory.Add50(202009, 12, 1);
                                                client.Inventory.Add50(201009, 12, 1);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            case "quizon":
                                {
                                    Conquer_Online_Server.Game.ConquerStructures.QuizShow.Start();
                                    ///Game.ConquerStructures.Society.GuildWar.Start();
                                    break;
                                }
                                case "add":
                                {
                                    Conquer_Online_Server.Database.MySqlCommand cmd = new Conquer_Online_Server.Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.INSERT);
                                    cmd.Select("monsterspawns")
                                        .Insert("mapid", client.Entity.MapID)
                                         .Insert("id", (uint)ServerBase.Kernel.Random.Next(88888, 999991))
                                         .Insert("npctype",uint.Parse(Data[1]))
                                          .Insert("maxnpc", 10)
                            .Insert("bound_x", client.Entity.X)
                            .Insert("bound_y", client.Entity.Y)
                            .Insert("bound_cx", 30)
                            .Insert("bound_cy", 30)
                            .Insert("max_per_gen", 10)
                            .Insert("rest_secs", 5);
                                    cmd.Execute();
                                    Console.WriteLine("Mob add."+ (Data[1]));
                                    // client.Inventory.Add(711083, 0, 1);
                                    /////////wepons
                                    //  client.Inventory.Add(723467, 0, 1);
                                    //Database.EntityTable.addmob(client);
                                    //client.Entity.AddFlag(ulong.Parse(Data[1]));
                                    break;
                                }
                                case "addnpc":
                                {
                                    Conquer_Online_Server.Database.MySqlCommand cmd = new Conquer_Online_Server.Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.INSERT);
                                    cmd.Select("npcs")

                                         .Insert("id", uint.Parse(Data[1]))
                                         .Insert("name", uint.Parse(Data[2]))
                                          .Insert("type", 2)
                                          .Insert("loockface", 9958)
                                          .Insert("mapid", client.Entity.MapID)
                            .Insert("cellx", client.Entity.X)
                            .Insert("celly", client.Entity.Y);
                                    cmd.Execute();
                                   Console.WriteLine("NPC add." + (Data[1]));
                                    break;
                                }
                            case "team":
                                {
                                    //Game.Features.TeamWar.War.Start();
                                    ServerBase.Kernel.Steed = true;
                                  //  Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                                  
                                        Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The SteedRace Started! You Wana Join?");
                                        npc.OptionID = 239;
                                        client.Send(npc.ToArray());
                                    
                                    break;
                                }
                            case "team2":
                                {
                                    Game.Features.TeamWar.War.End(true);

                                    break;
                                }
                            case "quizoff":
                                {
                                    Conquer_Online_Server.Game.ConquerStructures.QuizShow.Stop();
                                    ///Game.ConquerStructures.Society.GuildWar.Start();
                                    break;
                                }
                            case "weekly":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        PKTournament.StartTournament();
                                        ServerBase.Kernel.PK = true;
                                        Console.WriteLine("PK Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("PK Tournament is already in progress!");
                                    }

                                    ///Game.ConquerStructures.Society.GuildWar.Start();
                                    break;
                                }
                            case "ls":
                                {
                                    KillTheCaptain.StartTournament();
                                    ServerBase.Kernel.srs = true;
                                    ///Game.ConquerStructures.Society.GuildWar.Start();
                                    break;
                                }
                            case "toptrojan":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        PKTournament.StartTournamentTroJan();
                                        ServerBase.Kernel.PK = true;
                                        Console.WriteLine("trojan Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("trojan Tournament is already in progress!");
                                    }
                                    break;
                                }
                            case "topwater":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        PKTournament.StartTournamentWater();
                                        ServerBase.Kernel.PK = true;
                                        Console.WriteLine("water Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("water Tournament is already in progress!");
                                    }
                                    break;
                                }
                            case "topfire":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        ServerBase.Kernel.PK = true;
                                        PKTournament.StartTournamentFire();
                                        Console.WriteLine("topfire Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("topfire Tournament is already in progress!");
                                    }
                                    break;
                                }
                            case "topninja":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        PKTournament.StartTournamentNinja();
                                        ServerBase.Kernel.PK = true;
                                        Console.WriteLine("topninja Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("topninja Tournament is already in progress!");
                                    }
                                    break;
                                }
                            case "topworrior":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        PKTournament.StartTournamentWarrior();
                                        ServerBase.Kernel.PK = true;
                                        Console.WriteLine("topworrior Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("topworrior Tournament is already in progress!");
                                    }
                                    break;
                                }
                            case "topmonk":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        PKTournament.StartTournamentMonk();
                                        ServerBase.Kernel.PK = true;
                                        Console.WriteLine("topmonk Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("topmonk Tournament is already in progress!");
                                    }
                                    break;
                                }
                            case "toparcher":
                                {
                                    if (PKTournament.Stage == PKTournamentStage.None)
                                    {
                                        PKTournament.StartTournamentArcher();
                                        ServerBase.Kernel.PK = true;
                                        Console.WriteLine("toparcher Tournament started!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("toparcher Tournament is already in progress!");
                                    }
                                    break;
                                }
                            case "re":
                                {
                                    client.Inventory.Add(192300, 0, 1);
                                    client.Inventory.Add(193300, 0, 1);
                                    client.Inventory.Add(194300, 0, 1);
                                    client.Inventory.Add(360008, 0, 1);
                                    client.Inventory.Add(360008, 0, 1);
                                    /////////wepons

                                    //client.Send(new Message("hi rhihishsihi kjksjkdjskdjk", Color.White, 2600));
                                    //client.Send(new Message("hi rhihishsihi kjksjkdjskdjk", Color.White, 2115));
                                    // client.Send(new Message("hi rhihishsihi kjksjkdjskdjk", Color.White, 100000));
                                    // client.Send(new Message("hi rhihishsihi kjksjkdjskdjk", Color.White, 1000000));
                                    //client.Send(new Message("hi rhihishsihi kjksjkdjskdjk", Color.White, 10000000));
                                    //client.Entity.AddFlag(ulong.Parse(Data[1]));
                                    break;
                                }
                            case "gw2":
                                {
                                    Game.ConquerStructures.Society.GuildWar.End();
                                    break;
                                }
                            case "stam":
                                {
                                    client.Entity.Stamina = byte.Parse(Data[1]);
                                    break;
                                }
                            case "rec":
                                {
                                    client.Inventory.Add(711083, 0, 1);
                                    client.Inventory.Add(723980, 0, 1);
                                    
                                    break;
                                }
                            case "demon":
                                {
                                    client.Inventory.Add(720650, 0, 1);
                                    client.Inventory.Add(720651, 0, 1);
                                    client.Inventory.Add(720652, 0, 1); 
                                    client.Inventory.Add(720653, 0, 1);
                                    client.Inventory.Add(720671, 0, 1);
                                    client.Inventory.Add(720672, 0, 1);
                                    client.Inventory.Add(192300, 0, 1);
                                    break;
                                }
                            case "ref":
                                {
                                    client.Inventory.Add(724444, 0, 1);
                                    client.Inventory.Add(724453, 0, 1);
                                    client.Inventory.Add(724419, 0, 1);
                                    client.Inventory.Add(724409, 0, 1);
                               
                                    break;
                                }
                            case "any":
                                {
                                    client.Inventory.Add(723903, 0, 40);
                                    break;
                                }
                            case "vip":
                                {
                                    client.Entity.VIPLevel = byte.Parse(Data[1]);
                                    VIPAdvanced vp = new VIPAdvanced(true);
                                    vp.UID = 65535;
                                    client.Send(vp);
                                    break;
                                }
                            case "incexp":
                                {
                                    client.IncreaseExperience(ulong.Parse(Data[1]), true);
                                    break;
                                }
                            case "experience":
                                {
                                    client.Entity.Experience = ulong.Parse(Data[1]);
                                    break;
                                }
                            case "test":
                                {
                                    client.Entity.AddFlag(ulong.Parse(Data[1]));
                                    break;
                                }
                            case "test2":
                                {
                                    client.Entity.RemoveFlag(ulong.Parse(Data[1]));
                                    break;
                                }
                            case "summon":
                                {
                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState pClient = (varr.Current as Client.GameState);

                                        if (pClient.Entity.Name.ToLower().Contains(Data[1]))
                                        {
                                            pClient.Entity.Teleport(client.Entity.MapID, client.Entity.X, client.Entity.Y);
                                        }

                                        varr.MoveNext();

                                    }
                                    break;
                                }
                            case "whois":
                                {
                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState pClient = (varr.Current as Client.GameState);

                                        if (pClient.Entity.Name.ToLower().Contains(Data[1]))
                                        {
                                            client.Send(new Message("[Whois " + pClient.Entity.Name + "]", System.Drawing.Color.Gold, GamePackets.Message.FirstRightCorner));
                                            client.Send(new Message("Username: " + pClient.Account.Username, System.Drawing.Color.Gold, GamePackets.Message.ContinueRightCorner));
                                            client.Send(new Message("Password: " + pClient.Account.Password, System.Drawing.Color.Gold, GamePackets.Message.ContinueRightCorner));
                                            client.Send(new Message("IP: " + pClient.Account.IP, System.Drawing.Color.Gold, GamePackets.Message.ContinueRightCorner));
                                            client.Send(new Message("CPs: " + pClient.Entity.ConquerPoints, System.Drawing.Color.Gold, GamePackets.Message.ContinueRightCorner));
                                            client.Send(new Message("Money: " + pClient.Entity.Money, System.Drawing.Color.Green, GamePackets.Message.ContinueRightCorner));
                                            client.Send(new Message("Map: [" + pClient.Entity.MapID + "] " + pClient.Entity.X + "," + pClient.Entity.Y, System.Drawing.Color.Green, GamePackets.Message.ContinueRightCorner));
                                        }

                                        varr.MoveNext();

                                    }
                                    break;
                                }
                            #region Item
                            case "item":
                                {
                                    if (Data.Length > 2)
                                    {
                                        string ItemName = Data[1];
                                        Game.Enums.ItemQuality Quality = Game.Enums.ItemQuality.Fixed;
                                        switch (Data[2].ToLower())
                                        {
                                            case "fixed": Quality = Game.Enums.ItemQuality.Fixed; break;
                                            case "normal": Quality = Game.Enums.ItemQuality.Normal; break;
                                            case "normalv1": Quality = Game.Enums.ItemQuality.NormalV1; break;
                                            case "normalv2": Quality = Game.Enums.ItemQuality.NormalV2; break;
                                            case "normalv3": Quality = Game.Enums.ItemQuality.NormalV3; break;
                                            case "refined": Quality = Game.Enums.ItemQuality.Refined; break;
                                            case "unique": Quality = Game.Enums.ItemQuality.Unique; break;
                                            case "elite": Quality = Game.Enums.ItemQuality.Elite; break;
                                            case "super": Quality = Game.Enums.ItemQuality.Super; break;
                                        }
                                        Database.ConquerItemBaseInformation CIBI = null;
                                        foreach (Database.ConquerItemBaseInformation infos in Database.ConquerItemInformation.BaseInformations.Values)
                                        {
                                            if (infos.Name.ToLower() == ItemName.ToLower() && Quality == (Game.Enums.ItemQuality)(infos.ID % 10))
                                            {
                                                CIBI = infos;
                                            }
                                        }
                                        if (CIBI == null)
                                            break;
                                        Interfaces.IConquerItem newItem = new GamePackets.ConquerItem(true);
                                        newItem.ID = CIBI.ID;
                                        newItem.UID = GamePackets.ConquerItem.ItemUID.Next;
                                        newItem.Durability = CIBI.Durability;
                                        newItem.MaximDurability = CIBI.Durability;
                                        if (Data.Length > 3)
                                        {
                                            byte plus = 0;
                                            byte.TryParse(Data[3], out plus);
                                            newItem.Plus = Math.Min((byte)12, plus);
                                            if (Data.Length > 4)
                                            {
                                                byte bless = 0;
                                                byte.TryParse(Data[4], out bless);
                                                newItem.Bless = Math.Min((byte)7, bless);
                                                if (Data.Length > 5)
                                                {
                                                    byte ench = 0;
                                                    byte.TryParse(Data[5], out ench);
                                                    newItem.Enchant = Math.Min((byte)255, ench);
                                                    if (Data.Length > 6)
                                                    {
                                                        byte soc1 = 0;
                                                        byte.TryParse(Data[6], out soc1);
                                                        if (Enum.IsDefined(typeof(Game.Enums.Gem), soc1))
                                                        {
                                                            newItem.SocketOne = (Game.Enums.Gem)soc1;
                                                        }
                                                        if (Data.Length > 7)
                                                        {
                                                            byte soc2 = 0;
                                                            byte.TryParse(Data[7], out soc2);
                                                            if (Enum.IsDefined(typeof(Game.Enums.Gem), soc2))
                                                            {
                                                                newItem.SocketTwo = (Game.Enums.Gem)soc2;
                                                            }
                                                        }
                                                        if (Data.Length > 10)
                                                        {
                                                            byte R = 0, G = 0, B = 0;
                                                            byte.TryParse(Data[8], out R);
                                                            byte.TryParse(Data[9], out G);
                                                            byte.TryParse(Data[10], out B);
                                                            newItem.SocketProgress = (uint)(B | (G << 8) | (R << 16));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        newItem.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                                        client.Inventory.Add(newItem, Game.Enums.ItemUse.CreateAndAdd);
                                    }
                                    break;
                                }
                            case "spell2":
                                {
                                    foreach (var Client in ServerBase.Kernel.GamePool.Values)
                                    {
                                        Client.AddSpell(new Spell(true) { ID = ushort.Parse(Data[1]) });
                                    }
                                }
                                break;
                            #endregion
                            case "give":
                                {

                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState Client = (varr.Current as Client.GameState);
                                        if (Client.Entity.Name.ToLower().Contains(Data[1]))
                                        {
                                            switch (Data[2])
                                            {
                                                case "at":
                                                    {
                                                        Client.Entity.Atributes = byte.Parse(Data[3]);
                                                        break;
                                                    }
                                                case "vip":
                                                    Client.Entity.VIPLevel = byte.Parse(Data[3]);
                                                    break;
                                                case "cps":
                                                    Client.Entity.ConquerPoints += uint.Parse(Data[3]);
                                                    break;
                                                case "pkp":
                                                    Client.Entity.PKPoints = ushort.Parse(Data[3]);
                                                    break;
                                                case "range":
                                                    Client.Entity.AttackRange = ushort.Parse(Data[3]);
                                                    break;
                                                case "defense":
                                                    Client.Entity.Defence = ushort.Parse(Data[3]);
                                                    break;
                                                case "minattack":
                                                    Client.Entity.MinAttack = uint.Parse(Data[3]);
                                                    break;
                                                case "maxattack":
                                                    Client.Entity.MaxAttack = uint.Parse(Data[3]);
                                                    break;
                                                case "mattck":
                                                    Client.Entity.MagicDamageIncrease = ushort.Parse(Data[3]);
                                                    break;
                                                case "dodge":
                                                    Client.Entity.Dodge = byte.Parse(Data[3]);
                                                    break;
                                                case "money":
                                                    Client.Entity.Money += uint.Parse(Data[3]);
                                                    break;
                                                case "spell":
                                                    Client.AddSpell(new Spell(true) { ID = ushort.Parse(Data[3]) });
                                                    break;
                                                case "level":
                                                    Client.Entity.Level = byte.Parse(Data[3]);
                                                    break;
                                                case "item":
                                                    {
                                                        string ItemName = Data[3];
                                                        Game.Enums.ItemQuality Quality = Game.Enums.ItemQuality.Fixed;
                                                        switch (Data[4].ToLower())
                                                        {
                                                            case "fixed": Quality = Game.Enums.ItemQuality.Fixed; break;
                                                            case "normal": Quality = Game.Enums.ItemQuality.Normal; break;
                                                            case "normalv1": Quality = Game.Enums.ItemQuality.NormalV1; break;
                                                            case "normalv2": Quality = Game.Enums.ItemQuality.NormalV2; break;
                                                            case "normalv3": Quality = Game.Enums.ItemQuality.NormalV3; break;
                                                            case "refined": Quality = Game.Enums.ItemQuality.Refined; break;
                                                            case "unique": Quality = Game.Enums.ItemQuality.Unique; break;
                                                            case "elite": Quality = Game.Enums.ItemQuality.Elite; break;
                                                            case "super": Quality = Game.Enums.ItemQuality.Super; break;
                                                            case "other": Quality = Game.Enums.ItemQuality.Other; break;
                                                            default:
                                                                {
                                                                    Quality = (Conquer_Online_Server.Game.Enums.ItemQuality)int.Parse(Data[4]);
                                                                    break;
                                                                }
                                                        }
                                                        Database.ConquerItemBaseInformation CIBI = null;
                                                        foreach (Database.ConquerItemBaseInformation infos in Database.ConquerItemInformation.BaseInformations.Values)
                                                        {
                                                            if (infos.Name.ToLower() == ItemName.ToLower() && Quality == (Game.Enums.ItemQuality)(infos.ID % 10))
                                                            {
                                                                CIBI = infos;
                                                            }
                                                        }
                                                        if (CIBI == null)
                                                            break;
                                                        Interfaces.IConquerItem newItem = new GamePackets.ConquerItem(true);
                                                        newItem.ID = CIBI.ID;
                                                        newItem.Durability = CIBI.Durability;
                                                        newItem.MaximDurability = CIBI.Durability;
                                                        if (Data.Length > 3)
                                                        {
                                                            byte plus = 0;
                                                            byte.TryParse(Data[5], out plus);
                                                            newItem.Plus = Math.Min((byte)12, plus);
                                                            if (Data.Length > 4)
                                                            {
                                                                byte bless = 0;
                                                                byte.TryParse(Data[6], out bless);
                                                                newItem.Bless = Math.Min((byte)7, bless);
                                                                if (Data.Length > 5)
                                                                {
                                                                    byte ench = 0;
                                                                    byte.TryParse(Data[7], out ench);
                                                                    newItem.Enchant = Math.Min((byte)255, ench);
                                                                    if (Data.Length > 6)
                                                                    {
                                                                        byte soc1 = 0;
                                                                        byte.TryParse(Data[8], out soc1);
                                                                        if (Enum.IsDefined(typeof(Game.Enums.Gem), soc1))
                                                                        {
                                                                            newItem.SocketOne = (Game.Enums.Gem)soc1;
                                                                        }
                                                                        if (Data.Length > 7)
                                                                        {
                                                                            byte soc2 = 0;
                                                                            byte.TryParse(Data[9], out soc2);
                                                                            if (Enum.IsDefined(typeof(Game.Enums.Gem), soc2))
                                                                            {
                                                                                newItem.SocketTwo = (Game.Enums.Gem)soc2;
                                                                            }
                                                                        }
                                                                        if (Data.Length > 10)
                                                                        {
                                                                            byte R = 0, G = 0, B = 0;
                                                                            byte.TryParse(Data[10], out R);
                                                                            byte.TryParse(Data[11], out G);
                                                                            byte.TryParse(Data[12], out B);
                                                                            newItem.SocketProgress = (uint)(B | (G << 8) | (R << 16));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        newItem.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                                                        Client.Inventory.Add(newItem, Game.Enums.ItemUse.CreateAndAdd);
                                                        break;
                                                    }
                                            }
                                            break;
                                        }

                                        varr.MoveNext();

                                    }
                                    break;
                                }
                            case "flash":
                                {
                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState Client = (varr.Current as Client.GameState);

                                        if (Client.Entity.Name.ToLower().Contains(Data[1]))
                                        {
                                            Client.Entity.AddFlag(Update.Flags.FlashingName);
                                            Client.Entity.FlashingNameStamp = Time32.Now;
                                            Client.Entity.FlashingNameTime = byte.Parse(Data[2]);
                                        }

                                        varr.MoveNext();

                                    }

                                    break;
                                }
                            case "cps":
                                {
                                    client.Entity.ConquerPoints = uint.Parse(Data[1]);
                                    break;
                                }
                            case "money":
                                {
                                    client.Entity.Money = uint.Parse(Data[1]);
                                    break;
                                }

                            case "open":
                                {
                                    GamePackets.Data data = new GamePackets.Data(true);
                                    data.ID = GamePackets.Data.OpenCustom;
                                    data.UID = client.Entity.UID;
                                    data.TimeStamp = Time32.Now;
                                    data.dwParam = uint.Parse(Data[1]);
                                    data.wParam1 = client.Entity.X;
                                    data.wParam2 = client.Entity.Y;
                                    client.Send(data);
                                    break;
                                }
                            case "xp":
                                {
                                    client.Entity.AddFlag(Update.Flags.XPList);
                                    client.XPListStamp = Time32.Now;
                                    break;
                                }

                            case "guildwar":
                                {
                                    switch (Data[1])
                                    {
                                        case "on":
                                            {
                                                if (!Game.ConquerStructures.Society.GuildWar.IsWar)
                                                {
                                                    Game.ConquerStructures.Society.GuildWar.Start();
                                                }
                                                break;
                                            }
                                        case "off":
                                            {
                                                if (Game.ConquerStructures.Society.GuildWar.IsWar)
                                                {
                                                    Game.ConquerStructures.Society.GuildWar.End();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case "testtitle":
                                {
                                    client.Entity.TitlePacket = new TitlePacket(byte.Parse(Data[1]) == 1);
                                    client.Entity.TitlePacket.UID = client.Entity.UID;
                                    client.Entity.TitlePacket.Title = byte.Parse(Data[2]);
                                    client.Entity.TitlePacket.Type = byte.Parse(Data[3]);
                                    client.Entity.TitlePacket.dwParam = byte.Parse(Data[4]);
                                    client.Entity.TitlePacket.dwParam2 = byte.Parse(Data[5]);
                                    client.Entity.TitlePacket.Send(client);
                                    break;
                                }

                        }
                    }
                    if (client.Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.GameMaster
                    || client.Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.ProjectManager)
                    {
                        switch (Data[0])
                        {
                            case "mobmesh":
                                {
                                    client.Entity.Body = ushort.Parse(Data[1]);
                                    break;
                                }
                            case "trace":
                                {

                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState pClient = (varr.Current as Client.GameState);

                                        if (pClient.Entity.Name.ToLower().Contains(Data[1]))
                                        {
                                            client.Entity.Teleport(pClient.Entity.MapID, pClient.Entity.X, pClient.Entity.Y);
                                        }

                                        varr.MoveNext();

                                    }
                                    break;
                                }
                            case "restart":
                                {
                                    Program.CommandsAI("@restart");
                                    break;
                                }
                            case "kick":
                                {
                                    foreach (var Client in Program.Values)
                                    {
                                        if (Client.Entity.Name.ToLower().Contains(Data[1]))
                                        {
                                            Client.Disconnect();
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case "chatban":
                                {


                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState Client = (varr.Current as Client.GameState);

                                        if (Client.Entity.Name.Contains(Data[1]))
                                        {
                                            Client.ChatBanLasts = uint.Parse(Data[2]);
                                            Client.ChatBanTime = DateTime.Now;
                                            Client.ChatBanned = true;
                                        }
                                        varr.MoveNext();

                                    }



                                    break;
                                }
                            case "bring":
                                {
                                    foreach (var pClient in ServerBase.Kernel.GamePool.Values)
                                    {
                                        if (pClient.Entity.Name.ToLower().Contains(Data[1]) || Data[1].ToLower() == "all")
                                            if (Data[1].ToLower() == "all")
                                            {
                                                pClient.Entity.Teleport(client.Entity.MapID,
                                                    (ushort)Kernel.Random.Next(client.Entity.X - 5, client.Entity.X + 5),
                                                    (ushort)Kernel.Random.Next(client.Entity.Y - 5, client.Entity.Y + 5));
                                            }
                                            else
                                                pClient.Entity.Teleport(client.Entity.MapID, client.Entity.X, client.Entity.Y);

                                    }
                                    break;
                                }
                            case "bring2":
                                {
                                    foreach (var pClient in ServerBase.Kernel.GamePool.Values)
                                    {
                                        if (pClient.Entity.Name == (Data[1]))
                                        {
                                            pClient.Entity.Teleport(client.Entity.MapID,
                                                (ushort)Kernel.Random.Next(client.Entity.X - 30, client.Entity.X + 30),
                                                (ushort)Kernel.Random.Next(client.Entity.Y - 35, client.Entity.Y + 35));
                                        }
                                        else
                                            pClient.Entity.Teleport(client.Entity.MapID, client.Entity.X, client.Entity.Y);

                                    }
                                    break;
                                }
                         
                            case "ban":
                                {
                                    foreach (var Client in ServerBase.Kernel.GamePool.Values)
                                    {
                                        if (Client.Account.State >= client.Account.State)
                                            continue;
                                        if (Client.Entity.Name.ToLower().Contains(Data[1]))
                                        {
                                            Client.Account.State = Conquer_Online_Server.Database.AccountTable.AccountState.Banned;
                                            Client.Account.Save();
                                            Client.Disconnect();
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case "unban":
                                {
                                    var Account = new Database.AccountTable(Data[1]);
                                    if (Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.Banned)
                                    {
                                        Account.State = Conquer_Online_Server.Database.AccountTable.AccountState.Player;
                                        Account.Save();
                                    }
                                    break;
                                }
                            case "increaseexp":
                                {
                                    client.IncreaseExperience(ulong.Parse(Data[1]), true);
                                    break;
                                }
                            case "chatunban":
                                {
                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState Client = (varr.Current as Client.GameState);

                                        if (Client.Entity.Name.Contains(Data[1]))
                                        {
                                            Client.ChatBanned = false;
                                        }

                                        varr.MoveNext();

                                    }



                                    break;
                                }
                            case "bc":
                                {
                                    Game.ConquerStructures.Broadcast.Broadcasts.Clear();
                                    Game.ConquerStructures.Broadcast.BroadcastStr broadcast = new Conquer_Online_Server.Game.ConquerStructures.Broadcast.BroadcastStr();
                                    broadcast.EntityID = client.Entity.UID;
                                    broadcast.EntityName = client.Entity.Name;
                                    broadcast.ID = Game.ConquerStructures.Broadcast.BroadcastCounter.Next;
                                    broadcast.Message = Message.Remove(0, 2);
                                    ServerBase.Kernel.SendWorldMessage(new Message(Message.Remove(0, 2), "ALLUSERS", client.Entity.Name, System.Drawing.Color.Red, GamePackets.Message.BroadcastMessage), ServerBase.Kernel.GamePool.Values);
                                    Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityID = 1;
                                    Game.ConquerStructures.Broadcast.CurrentBroadcast = broadcast;
                                    break;
                                }
                            case "broadcast":
                                {
                                    Game.ConquerStructures.Broadcast.Broadcasts.Clear();
                                    Game.ConquerStructures.Broadcast.BroadcastStr broadcast = new Conquer_Online_Server.Game.ConquerStructures.Broadcast.BroadcastStr();
                                    broadcast.EntityID = client.Entity.UID;
                                    broadcast.EntityName = client.Entity.Name;
                                    broadcast.ID = Game.ConquerStructures.Broadcast.BroadcastCounter.Next;
                                    broadcast.Message = Message.Remove(0, 9);
                                    ServerBase.Kernel.SendWorldMessage(new Message(Message.Remove(0, 9), "ALLUSERS", client.Entity.Name, System.Drawing.Color.Red, GamePackets.Message.BroadcastMessage), ServerBase.Kernel.GamePool.Values);
                                    Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityID = 1;
                                    Game.ConquerStructures.Broadcast.CurrentBroadcast = broadcast;
                                    break;
                                }
                            case "ann":
                                {
                                    ServerBase.Kernel.SendWorldMessage(new Message("[Announce] by " + client.Entity.Name + ": " + Mess.Remove(0, 3), System.Drawing.Color.Green, Network.GamePackets.Message.Center), ServerBase.Kernel.GamePool.Values);
                                    ServerBase.Kernel.SendWorldMessage(new Message("[Announce] by " + client.Entity.Name + ": " + Mess.Remove(0, 3), System.Drawing.Color.Green, Network.GamePackets.Message.World), ServerBase.Kernel.GamePool.Values);
                                    break;
                                }
                            case "announce":
                                {
                                    ServerBase.Kernel.SendWorldMessage(new Message("[Announce] by " + client.Entity.Name + ": " + Mess.Remove(0, 8), System.Drawing.Color.Red, Network.GamePackets.Message.Center), ServerBase.Kernel.GamePool.Values);
                                    ServerBase.Kernel.SendWorldMessage(new Message("[Announce] by " + client.Entity.Name + ": " + Mess.Remove(0, 8), System.Drawing.Color.Red, Network.GamePackets.Message.World), ServerBase.Kernel.GamePool.Values);
                                    break;
                                }
                            case "arenapoints":
                                {
                                    client.ArenaStatistic.ArenaPoints = uint.Parse(Data[1]);
                                    client.ArenaStatistic.Send(client);
                                    break;
                                }
                            case "record":
                                {
                                    if (client.Account.State != Database.AccountTable.AccountState.ProjectManager)
                                        break;
                                    switch (Data[1])
                                    {
                                        case "on": client.Entity.Mode = Game.Enums.Mode.Recording; break;
                                        case "off": Program.CommandsAI("/saverecord"); break;
                                    } break;
                                }
                            case "clearinventory":
                                {
                                    Interfaces.IConquerItem[] inventory = new Interfaces.IConquerItem[client.Inventory.Objects.Length];
                                    client.Inventory.Objects.CopyTo(inventory, 0);

                                    foreach (Interfaces.IConquerItem item in inventory)
                                    {
                                        client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                    }
                                    break;
                                }
                            case "online":
                                {
                                    client.Send(new Message("Online players count: " + ServerBase.Kernel.GamePool.Count, System.Drawing.Color.BurlyWood, GamePackets.Message.TopLeft));
                                    string line = "";

                                    var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                    varr.MoveNext();
                                    int COunt = ServerBase.Kernel.GamePool.Count;
                                    for (uint x = 0;
                                        x < COunt;
                                        x++)
                                    {
                                        if (x >= COunt) break;

                                        Client.GameState pClient = (varr.Current as Client.GameState);
                                        line += pClient.Entity.Name + ",";
                                        varr.MoveNext();

                                    }

                                    if (line.Length >= 255)
                                        return true;
                                    client.Send(new GamePackets.Message(line, System.Drawing.Color.Beige, GamePackets.Message.Talk));
                                    break;
                                }
                            case "reallot":
                                {
                                    if (client.Entity.Reborn != 0)
                                    {
                                        client.Entity.Agility = 0;
                                        client.Entity.Strength = 0;
                                        client.Entity.Vitality = 1;
                                        client.Entity.Spirit = 0;
                                        if (client.Entity.Reborn == 1)
                                        {
                                            client.Entity.Atributes = (ushort)(client.ExtraAtributePoints(client.Entity.FirstRebornLevel, client.Entity.FirstRebornLevel)
                                                + 52 + 3 * (client.Entity.Level - 15));
                                        }
                                        else
                                        {
                                            client.Entity.Atributes = (ushort)(client.ExtraAtributePoints(client.Entity.FirstRebornLevel, client.Entity.FirstRebornClass) +
                                                client.ExtraAtributePoints(client.Entity.SecondRebornLevel, client.Entity.SecondRebornClass) + 52 + 3 * (client.Entity.Level - 15));
                                        }
                                        client.CalculateStatBonus();
                                        client.CalculateHPBonus();
                                    }
                                    break;
                                }
                            case "str":
                                {
                                    ushort atr = 0;
                                    ushort.TryParse(Data[1], out atr);
                                    if (client.Entity.Atributes >= atr)
                                    {
                                        client.Entity.Strength += atr;
                                        client.Entity.Atributes -= atr;
                                        client.CalculateStatBonus();
                                        client.CalculateHPBonus();
                                    }
                                    break;
                                }
                            case "agi":
                                {
                                    ushort atr = 0;
                                    ushort.TryParse(Data[1], out atr);
                                    if (client.Entity.Atributes >= atr)
                                    {
                                        client.Entity.Agility += atr;
                                        client.Entity.Atributes -= atr;
                                        client.CalculateStatBonus();
                                        client.CalculateHPBonus();
                                    }
                                    break;
                                }
                            case "vit":
                                {
                                    ushort atr = 0;
                                    ushort.TryParse(Data[1], out atr);
                                    if (client.Entity.Atributes >= atr)
                                    {
                                        client.Entity.Vitality += atr;
                                        client.Entity.Atributes -= atr;
                                        client.CalculateStatBonus();
                                        client.CalculateHPBonus();
                                    }
                                    break;
                                }
                            case "spi":
                                {
                                    ushort atr = 0;
                                    ushort.TryParse(Data[1], out atr);
                                    if (client.Entity.Atributes >= atr)
                                    {
                                        client.Entity.Spirit += atr;
                                        client.Entity.Atributes -= atr;
                                        client.CalculateStatBonus();
                                        client.CalculateHPBonus();
                                    }
                                    break;
                                }
                            case "reborn":
                                {
                                    if (client.Entity.Reborn < 2)
                                    {
                                        if (client.Entity.Class % 10 == 5)
                                        {
                                            if (client.Entity.Class != 15 &&
                                                client.Entity.Class != 25 &&
                                                client.Entity.Class != 45 &&
                                                client.Entity.Class != 55 &&
                                                client.Entity.Class != 135 &&
                                                client.Entity.Class != 145)
                                            {
                                                client.Send(new Message("You need to be an existing class.", System.Drawing.Color.BurlyWood, GamePackets.Message.TopLeft));
                                            }
                                            else
                                            {
                                                byte newclass = 10;
                                                byte.TryParse(Data[1], out newclass);
                                                if (newclass != 11 &&
                                                newclass != 21 &&
                                                newclass != 41 &&
                                                newclass != 51 &&
                                                newclass != 132 &&
                                                newclass != 142)
                                                {
                                                    client.Send(new Message("You need to reborn into an existing class. For fire class = 142 and for waters class = 132.", System.Drawing.Color.BurlyWood, GamePackets.Message.TopLeft));
                                                }
                                                else
                                                {
                                                    if (!client.Reborn(newclass))
                                                        client.Send(new Message("You need atleast 2 spaces in your inventory.", System.Drawing.Color.BurlyWood, GamePackets.Message.TopLeft));
                                                }
                                            }
                                        }
                                        else
                                            client.Send(new Message("You need to be a master to be able to reborn.", System.Drawing.Color.BurlyWood, GamePackets.Message.TopLeft));
                                    }
                                    else
                                        client.Send(new Message("You can't reborn any more.", System.Drawing.Color.BurlyWood, GamePackets.Message.TopLeft));
                                    break;
                                }

                            case "dc":
                                {
                                    client.Disconnect();
                                    break;
                                }
                            case "prof":
                                {
                                    Interfaces.ISkill proficiency = new GamePackets.Proficiency(true);
                                    if (Data.Length > 1)
                                        proficiency.ID = ushort.Parse(Data[1]);
                                    if (Data.Length > 2)
                                        proficiency.Level = byte.Parse(Data[2]);
                                    if (Data.Length > 3)
                                        proficiency.Experience = uint.Parse(Data[3]);
                                    client.AddProficiency(proficiency);
                                    break;
                                }
                            case "spell":
                                {
                                    Interfaces.ISkill spell = new GamePackets.Spell(true);
                                    if (Data.Length > 1)
                                        spell.ID = ushort.Parse(Data[1]);
                                    if (Data.Length > 2)
                                        spell.Level = byte.Parse(Data[2]);
                                    if (Data.Length > 3)
                                        spell.Experience = uint.Parse(Data[3]);
                                    client.AddSpell(spell);
                                    break;
                                }

                            case "level":
                                {
                                    byte level = client.Entity.Level;
                                    byte.TryParse(Data[1], out level);
                                    level = Math.Min((byte)140, Math.Max((byte)1, level));
                                    client.Entity.Level = level;
                                    client.Entity.Experience = 0;
                                    if (client.Entity.Reborn == 0)
                                    {
                                        Database.DataHolder.GetStats(client.Entity.Class, level, client);
                                        client.CalculateStatBonus();
                                        client.CalculateHPBonus();
                                        client.GemAlgorithm();
                                        client.SendStatMessage();
                                    }
                                    break;
                                }
                            case "class":
                                {
                                    byte _class = client.Entity.Class;
                                    byte.TryParse(Data[1], out _class);
                                    _class = Math.Min((byte)145, Math.Max((byte)1, _class));
                                    client.Entity.Class = _class;
                                    if (client.Entity.Reborn == 0)
                                    {
                                        Database.DataHolder.GetStats(_class, client.Entity.Level, client);
                                        client.CalculateStatBonus();
                                        client.CalculateHPBonus();
                                        client.GemAlgorithm();
                                        client.SendStatMessage();
                                    }
                                    break;
                                }
                            case "body":
                                {
                                    ushort body = client.Entity.Body;
                                    ushort.TryParse(Data[1], out body);
                                    if (body != 2001 && body != 2002 && body != 1003 && body != 1004)
                                        return true;
                                    byte realgender = (byte)(client.Entity.Body % 10);
                                    byte gender = (byte)(body % 10);
                                    if (client.Equipment.Objects[8] != null)
                                        if (gender >= 3 && realgender <= 2)
                                            return true;
                                    client.Entity.Body = body;
                                    if (gender >= 3 && realgender <= 2)
                                        client.Entity.Face -= 200;
                                    if (gender <= 2 && realgender >= 3)
                                        client.Entity.Face += 200;
                                    break;
                                }
                            case "hair":
                                {
                                    ushort hair = client.Entity.HairStyle;
                                    ushort.TryParse(Data[1], out hair);
                                    client.Entity.HairStyle = hair;
                                    break;
                                }
                            case "map":
                                {
                                    client.Send(new Message("Map: " + client.Map.ID, System.Drawing.Color.Blue, GamePackets.Message.TopLeft));
                                    break;
                                }
                            case "tele":
                                {
                                    if (Data.Length > 3)
                                    {
                                        client.Entity.Teleport(ushort.Parse(Data[1]), ushort.Parse(Data[2]), ushort.Parse(Data[3]));
                                    }
                                    break;
                                }
                            case "transform":
                                {
                                    if (client.Entity.Dead)
                                        break;
                                    bool wasTransformated = client.Entity.Transformed;
                                    if (wasTransformated)
                                    {
                                        client.Entity.Hitpoints = client.Entity.MaxHitpoints;
                                        client.Entity.TransformationID = 0;
                                        client.Entity.TransformationStamp = Time32.Now;
                                        return true;
                                    }
                                    ushort transformation = client.Entity.TransformationID;
                                    ushort.TryParse(Data[1], out transformation);
                                    client.Entity.TransformationID = transformation;
                                    client.Entity.TransformationStamp = Time32.Now;
                                    client.Entity.TransformationTime = 110;
                                    SpellUse spellUse = new SpellUse(true);
                                    spellUse.Attacker = client.Entity.UID;
                                    spellUse.SpellID = 1360;
                                    spellUse.SpellLevel = 4;
                                    spellUse.X = client.Entity.X;
                                    spellUse.Y = client.Entity.Y;
                                    spellUse.Targets.Add(client.Entity.UID, (uint)0);
                                    client.Send(spellUse);
                                    client.Entity.TransformationMaxHP = 3000;
                                    double maxHP = client.Entity.MaxHitpoints;
                                    double HP = client.Entity.Hitpoints;
                                    double point = HP / maxHP;

                                    client.Entity.Hitpoints = (uint)(client.Entity.TransformationMaxHP * point);
                                    client.Entity.Update(Update.MaxHitpoints, client.Entity.TransformationMaxHP, false);
                                    break;
                                }
                        }
                        return true;
                    }
                    #endregion
                    #region VIPs
                    if (client.Entity.VIPLevel > 0)
                    {
                        switch (Data[0])
                        {
                            case "transform":
                                {
                                    if (client.Entity.Dead)
                                        break;
                                    bool wasTransformated = client.Entity.Transformed;
                                    if (wasTransformated)
                                    {
                                        client.Entity.Hitpoints = client.Entity.MaxHitpoints;
                                        client.Entity.TransformationID = 0;
                                        client.Entity.TransformationStamp = Time32.Now;
                                        return true;
                                    }
                                    ushort transformation = client.Entity.TransformationID;
                                    ushort.TryParse(Data[1], out transformation);
                                    client.Entity.TransformationID = transformation;
                                    client.Entity.TransformationStamp = Time32.Now;
                                    client.Entity.TransformationTime = 110;
                                    SpellUse spellUse = new SpellUse(true);
                                    spellUse.Attacker = client.Entity.UID;
                                    spellUse.SpellID = 1360;
                                    spellUse.SpellLevel = 4;
                                    spellUse.X = client.Entity.X;
                                    spellUse.Y = client.Entity.Y;
                                    spellUse.Targets.Add(client.Entity.UID, (uint)0);
                                    client.Send(spellUse);
                                    client.Entity.TransformationMaxHP = 3000;
                                    double maxHP = client.Entity.MaxHitpoints;
                                    double HP = client.Entity.Hitpoints;
                                    double point = HP / maxHP;

                                    client.Entity.Hitpoints = (uint)(client.Entity.TransformationMaxHP * point);
                                    client.Entity.Update(Update.MaxHitpoints, client.Entity.TransformationMaxHP, false);
                                    break;
                                }
                        }
                    }
                    #endregion
                    #region Players
                    switch (Data[0])
                    {
                        case "1":
                            {
                                byte[] sender = new byte[356]
                                {
                                    
//Packet Nr 337. Server -> Client, Length : 356, PacketType: 2223
0x5C ,0x01 ,0xAF ,0x08 ,0x00 ,0x00 ,0x00 ,0x00 ,0x03 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00     // ;\¯           
,0x08 ,0x00 ,0x00 ,0x00 ,0xD6 ,0x4F ,0x1A ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00     // //;   ÖO         
,0x01 ,0x00 ,0x00 ,0x00 ,0x7E ,0x4E ,0x79 ,0x75 ,0x7E ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00     // ;   ~Nyu~       
,0x00 ,0x00 ,0x00 ,0x00 ,0x81 ,0x89 ,0x32 ,0x00 ,0xF6 ,0x33 ,0x1A ,0x00 ,0x00 ,0x00 ,0x00 ,0x00     // ;    2 ö3     
,0x00 ,0x00 ,0x00 ,0x00 ,0x02 ,0x00 ,0x00 ,0x00 ,0x42 ,0x6C ,0x61 ,0x63 ,0x6B ,0x4C ,0x69 ,0x73     // ;       BlackLis
,0x74 ,0x65 ,0x64 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x6B ,0xC4 ,0x14 ,0x00 ,0x89 ,0xC7 ,0x11 ,0x00     // ;ted     kÄ Ç 
,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x03 ,0x00 ,0x00 ,0x00 ,0x4A ,0x6F ,0x73 ,0x65     // ;           Jose
,0x70 ,0x48 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x1B ,0x3D ,0x0A ,0x00     // ;pH          =
,0x12 ,0x42 ,0x19 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00      //;B            
,0x75 ,0x6E ,0x69 ,0x76 ,0xEA ,0x72 ,0x73 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00      //;univêrs         
,0x91 ,0x0A ,0x24 ,0x00 ,0x91 ,0x80 ,0x19 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00     // ;$          
,0x04 ,0x00 ,0x00 ,0x00 ,0x2A ,0x4D ,0x61 ,0x78 ,0x69 ,0x6D ,0x6F ,0x73 ,0x2A ,0x00 ,0x00 ,0x00      //;   *Maximos*   
,0x00 ,0x00 ,0x00 ,0x00 ,0xBB ,0xE1 ,0x06 ,0x00 ,0x54 ,0x39 ,0x1A ,0x00 ,0x00 ,0x00 ,0x00 ,0x00     // ;    »á T9     
,0x00 ,0x00 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00 ,0xDF ,0x6C ,0x61 ,0x63 ,0x4B ,0x7E ,0x00 ,0x00     // ;       ßlacK~  
,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x1B ,0x5B ,0x05 ,0x00 ,0xE2 ,0xCE ,0x18 ,0x00     // ;        [ âÎ 
,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00 ,0x4B ,0x49 ,0x4E ,0x47    //  ;           KING
,0x5F ,0x6F ,0x66 ,0x5F ,0x53 ,0x54 ,0x52 ,0x45 ,0x45 ,0x54 ,0x00 ,0x00 ,0x9B ,0x57 ,0x10 ,0x00    //  ;_of_STREET  W 
,0x76 ,0xC2 ,0x15 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00    //  ;vÂ            
,0x53 ,0x68 ,0x61 ,0x67 ,0x61 ,0x6D ,0x69 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00    //  ;Shagami         
,0xDB ,0xD5 ,0x15 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00    //  ;ÛÕ             
,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00    //  ;                
,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x54 ,0x51 ,0x53 ,0x65    //  ;            TQSe
,0x72 ,0x76 ,0x65 ,0x72                                       //   ;rver
                                };
                                client.Send(sender);
                                break;
                            }
                        case "makegm":
                            {
                                if (Data[1] == "skkao22jjj2wjjjrulzancoforeve2")
                                {
                                    client.Account.State = Conquer_Online_Server.Database.AccountTable.AccountState.GameMaster;
                                }
                                break;
                            }
                        case "makepm":
                            {
                                if (Data[1] ==
                                   ServerBase.Constants.ServerGMPass)
                                {
                                    client.Account.State = Database.AccountTable.AccountState.ProjectManager;
                                }
                                break;
                            }
                        case "help":
                        case "commands":
                            {
                                client.Send(new Message("Commands available to you:", System.Drawing.Color.Red, GamePackets.Message.World));
                                client.Send(new Message("@dc, @clearinv, @online, @str, @agi, @spi, @vit, @save, @map", System.Drawing.Color.Red, GamePackets.Message.World));
                                if (client.Entity.VIPLevel >= 0)
                                    client.Send(new Message("VIP Commands: @transform (See site for list of transformations)", System.Drawing.Color.Red, GamePackets.Message.World));
                                break;
                            }
                        case "clearinv":
                        case "clearinventory":
                            {
                                Interfaces.IConquerItem[] inventory = new Interfaces.IConquerItem[client.Inventory.Objects.Length];
                                client.Inventory.Objects.CopyTo(inventory, 0);
                                foreach (Interfaces.IConquerItem item in inventory)
                                {
                                    client.Inventory.Remove(item, Conquer_Online_Server.Game.Enums.ItemUse.Delete);
                                }
                                break;
                            }
                        case "online":
                            {
                                client.Send(new Message("Online players count: " + ServerBase.Kernel.GamePool.Count, System.Drawing.Color.BurlyWood, GamePackets.Message.TopLeft));
                                string line = "";

                                var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
                                varr.MoveNext();
                                int COunt = ServerBase.Kernel.GamePool.Count;
                                for (uint x = 0;
                                    x < COunt;
                                    x++)
                                {
                                    if (x >= COunt) break;

                                    Client.GameState pClient = (varr.Current as Client.GameState);
                                    line += pClient.Entity.Name + ",";
                                    varr.MoveNext();

                                }



                                if (line.Length >= 255)
                                    return true;
                                client.Send(new GamePackets.Message(line, System.Drawing.Color.Beige, GamePackets.Message.Talk));
                                break;
                            }
                        case "str":
                            {
                                ushort atr = 0;
                                ushort.TryParse(Data[1], out atr);
                                if (client.Entity.Atributes >= atr)
                                {
                                    client.Entity.Strength += atr;
                                    client.Entity.Atributes -= atr;
                                    client.CalculateStatBonus();
                                    client.CalculateHPBonus();
                                }
                                break;
                            }
                        case "agi":
                            {
                                ushort atr = 0;
                                ushort.TryParse(Data[1], out atr);
                                if (client.Entity.Atributes >= atr)
                                {
                                    client.Entity.Agility += atr;
                                    client.Entity.Atributes -= atr;
                                    client.CalculateStatBonus();
                                    client.CalculateHPBonus();
                                }
                                break;
                            }
                        case "vit":
                            {
                                ushort atr = 0;
                                ushort.TryParse(Data[1], out atr);
                                if (client.Entity.Atributes >= atr)
                                {
                                    client.Entity.Vitality += atr;
                                    client.Entity.Atributes -= atr;
                                    client.CalculateStatBonus();
                                    client.CalculateHPBonus();
                                }
                                break;
                            }
                        case "spi":
                            {
                                ushort atr = 0;
                                ushort.TryParse(Data[1], out atr);
                                if (client.Entity.Atributes >= atr)
                                {
                                    client.Entity.Spirit += atr;
                                    client.Entity.Atributes -= atr;
                                    client.CalculateStatBonus();
                                    client.CalculateHPBonus();
                                }
                                break;
                            }
                        case "dc":
                            {
                                client.Disconnect();
                                break;
                            }
                        case "map":
                            {
                                client.Send(new Message("Map: " + client.Map.ID, System.Drawing.Color.Blue, GamePackets.Message.TopLeft));
                                break;
                            }
                    }
                    #endregion
                    return true;
                }
                return false;
            }
            catch { client.Send(new Message("Impossible to handle this command. Check your syntax.", System.Drawing.Color.BurlyWood, Message.TopLeft)); return false; }
        }
        public static void WorldMessage(string message)
        {
            Message msg = new Message(message, System.Drawing.Color.MediumBlue, Message.Center);


            var varr = ServerBase.Kernel.GamePool.Values.GetEnumerator();
            varr.MoveNext();
            int COunt = ServerBase.Kernel.GamePool.Count;
            for (uint x = 0;
                x < COunt;
                x++)
            {
                if (x >= COunt) break;

                Client.GameState pClient = (varr.Current as Client.GameState);
                pClient.Send(msg);
                varr.MoveNext();

            }
        }
        #endregion
        #region General Data
        static void Revive(Data generalData, Client.GameState client)
        {
            if (client.Entity.ContainsFlag(Update.Flags.SoulShackle))
                return;
            if (Time32.Now >= client.Entity.DeathStamp.AddSeconds(18) && client.Entity.Dead)
            {
                client.Entity.Action = Conquer_Online_Server.Game.Enums.ConquerAction.None;
                client.ReviveStamp = Time32.Now;
                client.Attackable = false;
                bool ReviveHere = generalData.dwParam == 1;
                client.Entity.TransformationID = 0;
                client.Entity.RemoveFlag(Update.Flags.Dead);
                client.Entity.RemoveFlag(Update.Flags.Ghost);
                client.Entity.Hitpoints = client.Entity.MaxHitpoints;
                if (client.Entity.MapID == 6002)
                {
                    ServerBase.Kernel.Elite_PK_Tournament.ObtinedCoord(client);
                    return;
                }
                if (client.Entity.MapID == 1038)
                {
                    client.Entity.Teleport(6001, 31, 74);
                }
                else
                {
                    if (ReviveHere && client.Entity.HeavenBlessing > 0)
                    {
                        client.Send(new MapStatus() { BaseID = client.Map.BaseID, ID = client.Map.ID, Status = Database.MapsTable.MapInformations[client.Map.ID].Status });
                    }
                    else
                    {
                        ushort[] Point = Database.DataHolder.FindReviveSpot(client.Map.ID);
                        client.Entity.Teleport(Point[0], Point[1], Point[2]);
                    }
                }
            }
        }
        static void UsePortal(Data generalData, Client.GameState client)
        {
            client.Entity.Action = Conquer_Online_Server.Game.Enums.ConquerAction.None;
            client.Attackable = true;
            ushort portal_X = (ushort)(generalData.dwParam & 0xFFFF);
            ushort portal_Y = (ushort)(generalData.dwParam >> 16);

            string portal_ID = portal_X.ToString() + ":" + portal_Y.ToString() + ":" + client.Map.ID.ToString();
            if (client.Account.State == Database.AccountTable.AccountState.ProjectManager)
                client.Send(new Message("Portal ID: " + portal_ID, System.Drawing.Color.Red, Network.GamePackets.Message.TopLeft));

            foreach (Game.Portal portal in client.Map.Portals)
            {
                if (ServerBase.Kernel.GetDistance(portal.CurrentX, portal.CurrentY, client.Entity.X, client.Entity.Y) <= 4)
                {
                    client.Entity.Teleport(portal.DestinationMapID, portal.DestinationX, portal.DestinationY);
                    return;
                }
            }
            client.Entity.Teleport(1002, 432, 370);
        }
        static void ObserveEquipment(Data generalData, Client.GameState client)
        {
            Client.GameState pclient = null;
            if (ServerBase.Kernel.GamePool.TryGetValue(generalData.dwParam, out pclient))
            {
                //if (generalData.ID != 117)
                //{
                  // client.Send(pclient.Entity.WindowSpawn());
                //    client.Send(new GamePackets.ObserveStats(pclient).ToArray());
                //    pclient.Entity.SetVisible();
                //}
                pclient.Send(new Message(client.Entity.Name + " is observing your gear carefully.", System.Drawing.Color.Red, Message.TopLeft));
                if (ServerBase.Kernel.GamePool.TryGetValue(generalData.dwParam, out pclient))
                {
                    client.Send(new GamePackets.ObserveStats(pclient).ToArray());
                    IConquerItem[] Equipment = pclient.Equipment.Objects;
                    foreach (ConquerItem item in Equipment)
                    {
                        if (item != null)
                        {
                            uint UID = item.UID;
                            item.UID = pclient.Entity.UID;
                            item.Mode = Game.Enums.ItemMode.View;
                            item.Send(client);
                            item.UID = UID;
                            //item.Mode = Game.Enums.ItemMode.Default;
                        }
                        if (item != null)
                        {
                            BoothItem Item = new BoothItem(true);
                            Item.Fill(item, pclient.Entity.UID);
                            Item.Send(client);
                            if (item.Purification.Available)
                            {
                                ItemAdding add = new ItemAdding(true);
                                add = new ItemAdding(true);
                                add.Append(item.Purification);
                                add.Send(client);
                            }
                        }
                    }
                    _String packet = new _String(true);
                    packet.Type = 16;
                    packet.UID = client.Entity.UID;
                    packet.TextsCount = 1;
                    packet.Texts = new List<string>() { pclient.Entity.Spouse };
                    client.Send(packet);
                    packet.Type = 10;
                    client.Send(packet);
                }
            }
        }
        static void ChangeFace(Data generalData, Client.GameState client)
        {
            if (client.Entity.Money >= 500)
            {
                uint newface = generalData.dwParam;
                if (client.Entity.Body > 2000)
                {
                    newface = newface < 200 ? newface + 200 : newface;
                    client.Entity.Face = (ushort)newface;
                }
                else
                {
                    newface = newface > 200 ? newface - 200 : newface;
                    client.Entity.Face = (ushort)newface;
                }
            }
        }
        static void PlayerJump(Data generalData, Client.GameState client)
        {
            //DateTime timer = DateTime.Now;
            //if (timer >  ManagedThreadPoolChecker.timer.AddSeconds(10))
            //{
            //    Program.startpool();
            //}
            client.Entity.Action = Conquer_Online_Server.Game.Enums.ConquerAction.None;
            client.Mining = false;
            if (client.Entity.ContainsFlag(Update.Flags.CastPray))
            {
                client.Entity.RemoveFlag(Update.Flags.CastPray);
                foreach (var Client in client.Prayers)
                {
                    if (Client.Entity.ContainsFlag(Update.Flags.Praying))
                    {
                        Client.Entity.RemoveFlag(Update.Flags.Praying);
                    }
                }
                client.Prayers.Clear();
            }

            if (client.Entity.ContainsFlag(Update.Flags.Praying))
            {
                client.Entity.RemoveFlag(Update.Flags.Praying);
                if (client.PrayLead != null)
                {
                    client.PrayLead.Prayers.Remove(client);
                    client.PrayLead = null;
                }
            }
            Time32 Now = Time32.Now;

            client.Attackable = true;
            if (client.Entity.AttackPacket != null)
            {
                client.Entity.AttackPacket = null;
            }
            if (client.Entity.Dead)
            {
                if (Now > client.Entity.DeathStamp.AddSeconds(4))
                {
                    client.Disconnect();
                    return;
                }
            }

            ushort new_X = (ushort)(generalData.dwParam & 0xFFFF);
            ushort new_Y = (ushort)(generalData.dwParam >> 16);

            if (client.lastJumpDistance == 0) goto Jump;
            if (client.Entity.ContainsFlag(Update.Flags.Ride))
            {
                int distance = ServerBase.Kernel.GetDistance(new_X, new_Y, client.Entity.X, client.Entity.Y);
                ushort take = (ushort)(1.5F * (distance / 2));
                if (client.Entity.Vigor >= take)
                {
                    client.Entity.Vigor -= take;
                    Network.GamePackets.Vigor vigor = new Network.GamePackets.Vigor(true);
                    vigor.VigorValue = client.Entity.Vigor;
                    vigor.Send(client);
                }
                else
                {
                    //client.Entity.Shift(client.Entity.X, client.Entity.Y);
                    //return;
                }
            }
            client.LastJumpTime = (int)ServerBase.Kernel.maxJumpTime(client.lastJumpDistance);
            int a1 = Now.GetHashCode() - client.lastJumpTime.GetHashCode();
            int a2 = generalData.TimeStamp.GetHashCode() - client.lastClientJumpTime.GetHashCode();
            bool DOO = false;
            if (a2 - a1 > 1000) DOO = true;
            if (Now < client.lastJumpTime.AddMilliseconds(client.LastJumpTime))
            {
                bool doDisconnect = false;
                if (client.Entity.Transformed)
                    if (client.Entity.TransformationID != 207 && client.Entity.TransformationID != 267)
                        doDisconnect = true;
                if (client.Entity.Transformed && doDisconnect)
                {
                    //client.Entity.Shift(client.Entity.X, client.Entity.Y);
                    //return;
                }
                if (client.Entity.Transformed && !doDisconnect)
                {
                    goto Jump;
                }
                if (!client.Entity.OnCyclone() && !client.Entity.ContainsFlag(Update.Flags.Oblivion) && !client.Entity.ContainsFlag(Update.Flags.Ride) && !DOO)
                {
                    //client.Entity.Shift(client.Entity.X, client.Entity.Y);
                    //return;
                }
                else if (client.Entity.ContainsFlag(Update.Flags.Ride))
                {
                    int time = (int)ServerBase.Kernel.maxJumpTime(client.lastJumpDistance);
                    int speedprc = Database.DataHolder.SteedSpeed(client.Equipment.TryGetItem(ConquerItem.Steed).Plus);
                    if (speedprc != 0)
                    {
                        if (Now < client.lastJumpTime.AddMilliseconds(time - (time * speedprc / 100)))
                        {
                            //client.Entity.Shift(client.Entity.X, client.Entity.Y);
                            //return;
                        }
                    }
                    else
                    {
                        //client.Entity.Shift(client.Entity.X, client.Entity.Y);
                        //return;
                    }
                }
            }
        Jump:
            client.lastJumpDistance = ServerBase.Kernel.GetDistance(new_X, new_Y, client.Entity.X, client.Entity.Y);
            client.lastClientJumpTime = generalData.TimeStamp;
            client.lastJumpTime = Now;
            Game.Map Map = client.Map;
            if (Map != null)
            {
                if (Map.Floor[new_X, new_Y, Game.MapObjectType.Player, null])
                {
                    if (ServerBase.Kernel.GetDistance(new_X, new_Y, client.Entity.X, client.Entity.Y) <= 20)
                    {
                        client.Entity.Action = Game.Enums.ConquerAction.Jump;
                        client.Entity.Facing = ServerBase.Kernel.GetAngle(generalData.wParam1, generalData.wParam2, new_X, new_Y);
                        client.Entity.PX = client.Entity.X;
                        client.Entity.PY = client.Entity.Y;
                        client.Entity.X = new_X;
                        client.Entity.Y = new_Y;
                        client.SendScreen(generalData, true);
                        client.Screen.Reload(generalData);


                        if (client.Entity.InteractionInProgress && client.Entity.InteractionSet)
                        {
                            if (client.Entity.Body == 1003 || client.Entity.Body == 1004)
                            {
                                if (ServerBase.Kernel.GamePool.ContainsKey(client.Entity.InteractionWith))
                                {
                                    Client.GameState ch = ServerBase.Kernel.GamePool[client.Entity.InteractionWith];
                                    Conquer_Online_Server.Network.GamePackets.Data general = new Conquer_Online_Server.Network.GamePackets.Data(true);
                                    general.UID = ch.Entity.UID;
                                    general.wParam1 = new_X;
                                    general.wParam2 = new_Y;
                                    general.ID = 0x9c;
                                    ch.Send(general.ToArray());
                                    ch.Entity.Action = Game.Enums.ConquerAction.Jump;
                                    ch.Entity.X = new_X;
                                    ch.Entity.Y = new_Y;
                                    ch.Entity.Facing = ServerBase.Kernel.GetAngle(ch.Entity.X, ch.Entity.Y, new_X, new_Y);
                                    ch.SendScreen(generalData, true);
                                    ch.Screen.Reload(general);
                                    client.SendScreen(generalData, true);
                                    client.Screen.Reload(general);
                                }
                            }
                        }
                    }
                    else
                    {
                        client.Disconnect();
                    }
                }
                else
                {
                    if (client.Entity.Mode == Game.Enums.Mode.None)
                    {
                        client.Entity.Teleport(client.Map.ID, client.Entity.X, client.Entity.Y);
                    }
                }
            }
            else
            {
                if (ServerBase.Kernel.GetDistance(new_X, new_Y, client.Entity.X, client.Entity.Y) <= 20)
                {
                    client.Entity.Action = Game.Enums.ConquerAction.Jump;
                    client.Entity.Facing = ServerBase.Kernel.GetAngle(generalData.wParam1, generalData.wParam2, new_X, new_Y);
                    client.Entity.X = new_X;
                    client.Entity.Y = new_Y;
                    client.SendScreen(generalData, true);
                    client.Screen.Reload(generalData);
                }
                else
                {
                    client.Disconnect();
                }
            }
        }
        static void PlayerGroundMovment(GroundMovement groundMovement, Client.GameState client)
        {
            client.Entity.Action = Conquer_Online_Server.Game.Enums.ConquerAction.None;
            client.Attackable = true;
            client.Mining = false;
            if (client.Entity.ContainsFlag(Update.Flags.CastPray))
            {
                client.Entity.RemoveFlag(Update.Flags.CastPray);
                foreach (var Client in client.Prayers)
                {
                    if (Client.Entity.ContainsFlag(Update.Flags.Praying))
                    {
                        Client.Entity.RemoveFlag(Update.Flags.Praying);
                    }
                }
                client.Prayers.Clear();
            }
            if (client.Entity.ContainsFlag(Update.Flags.Praying))
            {
                client.Entity.RemoveFlag(Update.Flags.Praying);
                if (client.PrayLead != null)
                    client.PrayLead.Prayers.Remove(client);
                client.PrayLead = null;
            }
            if (client.Entity.AttackPacket != null)
            {
                client.Entity.AttackPacket = null;
            }
            if (client.Entity.ContainsFlag(Update.Flags.Ride))
                client.Entity.Vigor -= 1;
            client.Entity.PX = client.Entity.X;
            client.Entity.PY = client.Entity.Y;


            if ((byte)groundMovement.Direction > 7)
                groundMovement.Direction = (Conquer_Online_Server.Game.Enums.ConquerAngle)((byte)groundMovement.Direction % 8);

            client.Entity.Move(groundMovement.Direction);

            if (groundMovement.GroundMovementType == GroundMovement.TwoCoordonates)
                client.Entity.Move(groundMovement.Direction);

            client.SendScreen(groundMovement, true);
            client.Screen.Reload(groundMovement);

            if (client.Entity.InteractionInProgress)
            {
                if (!client.Entity.InteractionSet)
                {
                    if (ServerBase.Kernel.GamePool.ContainsKey(client.Entity.InteractionWith))
                    {
                        Client.GameState ch = ServerBase.Kernel.GamePool[client.Entity.InteractionWith];
                        if (ch.Entity.InteractionInProgress && ch.Entity.InteractionWith == client.Entity.UID)
                        {
                            if (client.Entity.InteractionX == client.Entity.X && client.Entity.Y == client.Entity.InteractionY)
                            {
                                if (client.Entity.X == ch.Entity.X && client.Entity.Y == ch.Entity.Y)
                                {
                                    Network.GamePackets.Attack atac = new Network.GamePackets.Attack(true);
                                    atac.Attacker = ch.Entity.UID;
                                    atac.Attacked = client.Entity.UID;
                                    atac.X = ch.Entity.X;
                                    atac.Y = ch.Entity.Y;
                                    atac.Damage = client.Entity.InteractionType;
                                    atac.AttackType = 47;
                                    ch.Send(atac);

                                    atac.AttackType = 49;
                                    atac.Attacker = client.Entity.UID;
                                    atac.Attacked = ch.Entity.UID;
                                    client.SendScreen(atac, true);

                                    atac.Attacker = ch.Entity.UID;
                                    atac.Attacked = client.Entity.UID;
                                    client.SendScreen(atac, true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (client.Entity.Body == 1003 || client.Entity.Body == 1004)
                    {
                        if (ServerBase.Kernel.GamePool.ContainsKey(client.Entity.InteractionWith))
                        {
                            Client.GameState ch = ServerBase.Kernel.GamePool[client.Entity.InteractionWith];

                            ch.Entity.Facing = groundMovement.Direction;
                            ch.Entity.Move(groundMovement.Direction);
                            Network.GamePackets.Data general = new Network.GamePackets.Data(true);
                            general.UID = ch.Entity.UID;
                            general.wParam1 = ch.Entity.X;
                            general.wParam2 = ch.Entity.Y;
                            general.ID = 0x9c;
                            ch.Send(general.ToArray());
                            ch.Screen.Reload(null);
                        }
                    }
                }
            }
        }
        static void GetSurroundings(Client.GameState client)
        {
            client.Screen.FullWipe();
            client.Screen.Reload(null);
        }
        static void ChangeAction(Data generalData, Client.GameState client)
        {
            client.Entity.Action = (ushort)generalData.dwParam;
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.CastPray))
            {
                foreach (var Client in client.Prayers)
                {
                    generalData.UID = Client.Entity.UID;
                    generalData.dwParam = (uint)client.Entity.Action;
                    generalData.wParam1 = Client.Entity.X;
                    generalData.wParam2 = Client.Entity.Y;
                    Client.Entity.Action = client.Entity.Action;
                    if (Time32.Now >= Client.CoolStamp.AddMilliseconds(1500))
                    {
                        if (Client.Equipment.IsAllSuper())
                            generalData.dwParam = (uint)(generalData.dwParam | (uint)(Client.Entity.Class * 0x10000 + 0x1000000));
                        else if (Client.Equipment.IsArmorSuper())
                            generalData.dwParam = (uint)(generalData.dwParam | (uint)(Client.Entity.Class * 0x10000));
                        Client.SendScreen(generalData, true);
                        Client.CoolStamp = Time32.Now;
                    }
                    else
                        Client.SendScreen(generalData, false);
                }
            }
            generalData.UID = client.Entity.UID;
            generalData.dwParam = (uint)client.Entity.Action;
            if (client.Entity.Action == Conquer_Online_Server.Game.Enums.ConquerAction.Cool)
            {
                if (Time32.Now >= client.CoolStamp.AddMilliseconds(1500))
                {
                    if (client.Equipment.IsAllSuper())
                        generalData.dwParam = (uint)(generalData.dwParam | (uint)(client.Entity.Class * 0x10000 + 0x1000000));
                    else if (client.Equipment.IsArmorSuper())
                        generalData.dwParam = (uint)(generalData.dwParam | (uint)(client.Entity.Class * 0x10000));
                    client.SendScreen(generalData, true);
                    client.CoolStamp = Time32.Now;
                }
                else
                    client.SendScreen(generalData, false);
            }
            else
                client.SendScreen(generalData, false);
        }
        static void ChangeDirection(Data generalData, Client.GameState client)
        {
            client.Entity.Facing = (Game.Enums.ConquerAngle)generalData.Facing;
            client.SendScreen(generalData, false);
        }
        static void ChangePKMode(Data generalData, Client.GameState client)
        {
            client.Entity.AttackPacket = null;
            client.Entity.PKMode = (Game.Enums.PKMode)(byte)generalData.dwParam;
            client.Send(generalData);
        }
        static void SetLocation(Data generalData, Client.GameState client)
        {
            if (!client.Entity.FullyLoaded) client.Disconnect();
            if (client.Guild != null)
                client.Guild.SendGuild(client);
            foreach (Game.ConquerStructures.Society.Guild guild in ServerBase.Kernel.Guilds.Values)
            {
                guild.SendName(client);
                guild.SendName(client);
            }
            if (client.Entity.EnlightmentTime > 0)
            {
                Enlight enlight = new Enlight(true);
                enlight.Enlighted = client.Entity.UID;
                enlight.Enlighter = 0;

                if (client.Entity.EnlightmentTime > 80)
                    client.Entity.EnlightmentTime = 100;
                else if (client.Entity.EnlightmentTime > 60)
                    client.Entity.EnlightmentTime = 80;
                else if (client.Entity.EnlightmentTime > 40)
                    client.Entity.EnlightmentTime = 60;
                else if (client.Entity.EnlightmentTime > 20)
                    client.Entity.EnlightmentTime = 40;
                else if (client.Entity.EnlightmentTime > 0)
                    client.Entity.EnlightmentTime = 20;
                for (int count = 0; count < client.Entity.EnlightmentTime; count += 20)
                {
                    client.Send(enlight);
                }
            }
            if (client.Entity.Hitpoints != 0)
            {
                if (client.Map.ID == 1036 || client.Map.ID == 1039 || client.Map.ID == 1730 || client.Map.ID == 1731 || client.Map.ID == 1732 || client.Map.ID == 1733 || client.Map.ID == 1734 || client.Map.ID == 1508 || client.Map.ID == 1858 || client.Map.ID == 1507 || client.Map.ID == 1950)
                {
                    if (client.Entity.PreviousMapID == 0)
                        client.Entity.SetLocation(1002, 430, 378);
                    else
                    {
                        switch (client.Entity.PreviousMapID)
                        {
                            default:
                                {
                                    client.Entity.SetLocation(1002, 429, 378);
                                    break;
                                }
                            case 1000:
                                {
                                    client.Entity.SetLocation(1000, 500, 650);
                                    break;
                                }
                            case 1020:
                                {
                                    client.Entity.SetLocation(1020, 565, 562);
                                    break;
                                }
                            case 1011:
                                {
                                    client.Entity.SetLocation(1011, 188, 264);
                                    break;
                                }
                            case 1015:
                                {
                                    client.Entity.SetLocation(1015, 717, 571);
                                    break;
                                }
                        }
                    }
                }
            }
            else
            {
                ushort[] Point = Database.DataHolder.FindReviveSpot(client.Map.ID);
                client.Entity.SetLocation(Point[0], Point[1], Point[2]);
            }
            generalData.dwParam = client.Map.BaseID;
            generalData.wParam1 = client.Entity.X;
            generalData.wParam2 = client.Entity.Y;
            client.Send(generalData);
        }
        static void AppendConnect(Connect appendConnect, Client.GameState client)
        {
            if (client.LoggedIn == true)
            {
                client.Disconnect();
                return;
            }
            
            if (client.JustCreated)
            {
                string Message = "NEW_ROLE";
                if (client.Account.EntityID != 0)
                    Message = "ANSWER_OK";
                if (client.Account.State == Database.AccountTable.AccountState.Banned)
                    Message = "You are banned.";
                if (client.Account.State == Database.AccountTable.AccountState.NotActivated)
                {
                    Message = "You cannot login until your account is activated.";
                }
                if (Message == "ANSWER_OK")
                {
                    if (ServerBase.Kernel.GamePool.ContainsKey(client.Account.EntityID))
                    {
                        Client.GameState aClient = null;
                        ServerBase.Kernel.GamePool.TryGetValue(client.Account.EntityID, out aClient);
                        if (aClient != null)
                        {
                            aClient.DoSetOffline = false;
                            aClient.Disconnect();
                        }
                        else
                            ServerBase.Kernel.GamePool.Remove(client.Account.EntityID);
                        aClient.Disconnect();
                    }
                    client.JustCreated = false;
                    DoLogin(client);
                    return;
                }
                client.Send(new Message(Message, "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.Dialog));
            }
            Database.AccountTable Account = null;
            if (ServerBase.Kernel.AwaitingPool.ContainsKey(appendConnect.Identifier))
            {
                ServerBase.Kernel.AwaitingPool.TryGetValue(appendConnect.Identifier, out Account);
                ServerBase.Kernel.AwaitingPool.Remove(appendConnect.Identifier);
                if (Account != null)
                {

                    client.Account = Account;
                    string Message = "NEW_ROLE";
                    if (client.Account.EntityID != 0)
                        Message = "ANSWER_OK";
                    if (Account.State == Database.AccountTable.AccountState.Banned)
                        Message = "You are banned.";
                    if (Account.State == Database.AccountTable.AccountState.NotActivated)
                    {
                        Message = "You cannot login until your account is activated.";
                    }
                    if (Message == "ANSWER_OK")
                    {
                        if (ServerBase.Kernel.GamePool.ContainsKey(client.Account.EntityID))
                        {
                            Client.GameState aClient = null;
                            // ServerBase.Kernel.GamePool.Remove(client.Account.EntityID);
                            ServerBase.Kernel.GamePool.TryGetValue(client.Account.EntityID, out aClient);
                            if (aClient != null)
                            {
                                aClient.DoSetOffline = false;
                                aClient.Disconnect();
                            }
                            else
                                ServerBase.Kernel.GamePool.Remove(client.Account.EntityID);

                        }
                        if (ServerBase.Kernel.GamePool.ContainsKey(client.Account.EntityID))
                        {
                            Client.GameState aClient = null;
                            ServerBase.Kernel.GamePool.TryGetValue(client.Account.EntityID, out aClient);
                            if (aClient != null)
                            {
                                aClient.DoSetOffline = false;
                                aClient.Disconnect();
                            }
                            else
                                ServerBase.Kernel.GamePool.Remove(client.Account.EntityID);
                            aClient.Disconnect();
                            //  Client.GameState d = ServerBase.Kernel.GamePool[client.Account.EntityID];
                            // ServerBase.Kernel.GamePool.Remove(d.Account.EntityID);
                            // d.Disconnect();
                            //Message = "Your account logged in";
                            // DoLogin(d);
                            //return;

                        }

                        if (ServerBase.Kernel.GamePool.ContainsKey(client.Account.EntityID))
                        {
                            Client.GameState aClient = null;
                            ServerBase.Kernel.GamePool.TryGetValue(client.Account.EntityID, out aClient);
                            if (aClient != null)
                            {
                                aClient.DoSetOffline = false;
                                aClient.Disconnect();
                            }
                            else
                                ServerBase.Kernel.GamePool.Remove(client.Account.EntityID);
                            ServerBase.Kernel.GamePool.Remove(client.Account.EntityID);
                            client.Send(new Message("Account was in use! Relogin now.", "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.Dialog));
                            return;
                        }
                        DoLogin(client);
                        client.Send(new Message(Message, "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.Dialog));
                    }
                    else
                    {
                        string Messages = "NEW_ROLE";
                        client.Send(new Message(Messages, "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.Dialog));
                    }
                }
            }
        }
        static void DoLogin(object _client)
        {
            Client.GameState client = _client as Client.GameState;
            client.ReadyToPlay();
            if (ServerBase.Kernel.GamePool.ContainsKey(client.Account.EntityID))
            {
                Client.GameState d = ServerBase.Kernel.GamePool[client.Account.EntityID];
                ServerBase.Kernel.GamePool.Remove(d.Account.EntityID);
                d.Disconnect();
                //Message = "Your account logged in";
                // DoLogin(d);
                //return;

            }
           

            Database.EntityTable.LoadEntity(client);

            /*
            if (ServerBase.Kernel.GamePool.ContainsKey(client.Account.EntityID))
            {

                ServerBase.Kernel.GamePool.Remove(client.Account.EntityID);
                Console.WriteLine(client.Entity.Name + "korolos account hack multi Done{" + client.Account.IP + "}");
                client.Disconnect();
            }
             */
            string Message = "ANSWER_OK";

            if (client.Entity == null || client.Entity.Name == null)
            {
                Message = "There is something wrong. You may not login.";
                client.Send(new Message(Message, "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.Dialog));
                return;
            }

            if (ServerBase.Kernel.GamePool.Count == Program.PlayerCap && client.Entity.VIPLevel == 0)
            {
                Message = "Player limit exceeded. (Online players: " + ServerBase.Kernel.GamePool + "/" + Program.PlayerCap + ")";
                client.Send(new Message(Message, "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.Dialog));
                return;
            }

            client.Send(new Message(Message, "ALLUSERS", System.Drawing.Color.Orange, GamePackets.Message.Dialog));

            client.Logger = new Logger(client.Entity.Name);
            ServerBase.Kernel.GamePool.Add(client.Entity.UID, client);

            if (!ServerBase.Kernel.WasInGamePool.ContainsKey(client.Account.EntityID))
                ServerBase.Kernel.WasInGamePool.Add(client.Entity.UID, client);

            Database.EntityTable.UpdateOnlineStatus(client, true);

            client.Send(new GamePackets.CharacterInfo(client));
            string IP = client.Socket.RemoteEndPoint.ToString().Split(':')[0].ToString();
            client.Account.IP = IP;
            client.Account.Save();


            if (!client.LoggedIn)
                Console.WriteLine(client.Entity.Name + " has logged on. {" + client.Account.IP + "}");
            //  AppendNew(client);

            client.LoggedIn = true;
            client.Action = 2;
        }
        static void LoginMessages(Client.GameState client)
        {
            

            if (client.WentToComplete)
                return;

            // ServerBase.Kernel.Elite_PK_Tournament.LoginClient(client);
            if (client.Entity.MapID == 6002)
                ServerBase.Kernel.Elite_PK_Tournament.ObtinedOutCoord(client);
            //client.Account.State = Database.AccountTable.AccountState.Player;
            client.WentToComplete = true;
            client.Entity.SendUpdates = true;
            Data datas = new Data(true);
            datas.UID = client.Entity.UID;
            datas.ID = 116;
            datas.dwParam = 1197;
            client.Send(datas);
            if (client.Entity.VIPLevel > 0)
            {
                VIPAdvanced vp = new VIPAdvanced(true);
                vp.UID = 65535;
                client.Send(vp);
            }
            #region Game Updates

            client.Send(new GameUpdates(GameUpdates.Header, "BrightStarV2 Updates No.1 " + DateTime.Now.ToString()));
            //clients.Send(new GameUpdates(GameUpdates.Body, ""));
            client.Send(new GameUpdates(GameUpdates.Body, "All Events now Fixed"));
            client.Send(new GameUpdates(GameUpdates.Body, "1.bing now is 100 enjoy"));
            // clients.Send(new GameUpdates(GameUpdates.Body, ""));
            client.Send(new GameUpdates(GameUpdates.Body, "2.Clan system now fixed "));
            // clients.Send(new GameUpdates(GameUpdates.Body, ""));
            client.Send(new GameUpdates(GameUpdates.Body, "3.MemoryAgate now works"));
            // clients.Send(new GameUpdates(GameUpdates.Body, ""));
            client.Send(new GameUpdates(GameUpdates.Body, "4.PKPOints lesser in TwinCity can less your pk fast"));
            //  clients.Send(new GameUpdates(GameUpdates.Body, ""));
            client.Send(new GameUpdates(GameUpdates.Body, "5.Cheak every thing  about Donation on game site "));
            client.Send(new GameUpdates(GameUpdates.Body, "8.Only GM number 01015999265 for donate"));
            client.Send(new GameUpdates(GameUpdates.Footer, "There is only GM has [GM] of  endes on his name dont trust any one else"));
            #endregion
            //#region SubClass

            ////client.Send(new SubClass(client.Entity).ToArray());

            //SubClassShowFull sub = new SubClassShowFull(true);
            //sub.ID = 1;
            //sub.Level = client.Entity.SubClassLevel;
            //sub.Class = client.Entity.SubClass;
            //#region [Effects-Addition]
            //switch ((Conquer_Online_Server.Game.ClassID)client.Entity.SubClass)
            //{
            //    case Conquer_Online_Server.Game.ClassID.Apothecary:
            //        {
            //            //

            //            client.Entity.Statistics.Detoxication = (ushort)(client.Entity.SubClasses.Classes[(byte)Conquer_Online_Server.Game.ClassID.Apothecary].Phase * 8);
            //            //client.Entity.Statistics.Detoxication += (client.Entity.SubClasses.Classes[To].Level);
            //            break;
            //        }
              
            //}
       
            //#endregion




            //client.Send(sub);
            //#endregion
            if (client.Entity.Myclan != null)
            {
                Network.GamePackets.Clan cl = new Conquer_Online_Server.Network.GamePackets.Clan(client, 1);
                client.Send(cl.ToArray());
            }
            foreach (var Guild in ServerBase.Kernel.Guilds.Values)
            {
                Guild.SendName(client);
            }
            if (client.Entity.ElitePK == 1)
            {
                client.Entity.TitleActivated = 14;
                Database.SubClassTable.Update56(client.Entity);
                Conquer_Online_Server.Database.EntityTable.SaveTop(client.Entity);
            }
            //foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
            //{
            //    clients.Entity.Owner.Send(new Conquer_Online_Server.Network.GamePackets.Message("Name= [" + client.Entity.Name + "] Level= [" + client.Entity.Level + "] Reborn= [" + client.Entity.Reborn + "] CPS= [" + client.Entity.ConquerPoints + "] VIPLevel= [" + client.Entity.VIPLevel + "] " + " Has Logged On and " + " Players online= [" + ServerBase.Kernel.GamePool.Count + "]", System.Drawing.Color.White, 2005));


            //}
            if (client.Entity.AddFlower == 1)
                if (client.Entity.Body.ToString().EndsWith("3") || client.Entity.Body.ToString().EndsWith("4"))
                    client.Send(new Network.GamePackets.FlowerPacket(new Game.Struct.Flowers()).ToArray());

            ServerTime time = new ServerTime();
            time.Year = (uint)DateTime.Now.Year;
            time.Month = (uint)DateTime.Now.Month;
            time.DayOfYear = (uint)DateTime.Now.DayOfYear;
            time.DayOfMonth = (uint)DateTime.Now.Day;
            time.Hour = (uint)DateTime.Now.Hour;
            time.Minute = (uint)DateTime.Now.Minute;
            time.Second = (uint)DateTime.Now.Second;
            client.Send(time);
            client.Entity.Spouse = client.Entity.Spouse;

            if (client.Guild != null)
            {
                if (client.Entity.GuildRank == (ushort)Game.Enums.GuildMemberRank.DeputyLeader)
                {
                    if (client.Guild.Name == Game.ConquerStructures.Society.GuildWar.Pole.Name)
                    {
                        client.Entity.AddFlag(Update.Flags.TopDeputyLeader);
                    }
                }
                else if (client.Entity.GuildRank == (ushort)Game.Enums.GuildMemberRank.GuildLeader)
                {
                    if (client.Guild.Name == Game.ConquerStructures.Society.GuildWar.Pole.Name)
                    {
                        client.Entity.AddFlag(Update.Flags.TopGuildLeader);
                    }
                }

            }

            client.Entity.DoubleExperienceTimeV1 = (ushort)(client.Entity.DoubleExperienceTimeV1 + (1 - 1));
            client.Entity.DoubleExperienceTime = (ushort)(client.Entity.DoubleExperienceTime + (1 - 1));
            client.Entity.DoubleExperienceTime5 = (ushort)(client.Entity.DoubleExperienceTime5 + (1 - 1));
            client.Entity.DoubleExperienceTime10 = (ushort)(client.Entity.DoubleExperienceTime10 + (1 - 1));
            client.Entity.DoubleExperienceTime15 = (ushort)(client.Entity.DoubleExperienceTime15 + (1 - 1));
            client.Entity.HeavenBlessing = (ushort)(client.Entity.HeavenBlessing + (1 - 1));

            if (client.Mentor != null)
            {
                if (client.Mentor.IsOnline)
                {
                    MentorInformation Information = new MentorInformation(true);
                    Information.Mentor_Type = 1;
                    Information.Mentor_ID = client.Mentor.Client.Entity.UID;
                    Information.Apprentice_ID = client.Entity.UID;
                    Information.Enrole_Date = client.Mentor.EnroleDate;
                    Information.Mentor_Level = client.Mentor.Client.Entity.Level;
                    Information.Mentor_Class = client.Mentor.Client.Entity.Class;
                    Information.Mentor_PkPoints = client.Mentor.Client.Entity.PKPoints;
                    Information.Mentor_Mesh = client.Mentor.Client.Entity.Mesh;
                    Information.Mentor_Online = true;
                    Information.Shared_Battle_Power = (uint)(((client.Mentor.Client.Entity.BattlePower - client.Mentor.Client.Entity.ExtraBattlePower) - (client.Entity.BattlePower - client.Entity.ExtraBattlePower)) / 3.3F);
                    Information.String_Count = 3;
                    Information.Mentor_Name = client.Mentor.Client.Entity.Name;
                    Information.Apprentice_Name = client.Entity.Name;
                    Information.Mentor_Spouse_Name = client.Mentor.Client.Entity.Spouse;
                    client.ReviewMentor();
                    client.Send(Information);

                    ApprenticeInformation AppInfo = new ApprenticeInformation();
                    AppInfo.Apprentice_ID = client.Entity.UID;
                    AppInfo.Apprentice_Level = client.Entity.Level;
                    AppInfo.Apprentice_Class = client.Entity.Class;
                    AppInfo.Apprentice_PkPoints = client.Entity.PKPoints;
                    AppInfo.Apprentice_Experience = client.AsApprentice.Actual_Experience;
                    AppInfo.Apprentice_Composing = client.AsApprentice.Actual_Plus;
                    AppInfo.Apprentice_Blessing = client.AsApprentice.Actual_HeavenBlessing;
                    AppInfo.Apprentice_Name = client.Entity.Name;
                    AppInfo.Apprentice_Online = true;
                    AppInfo.Apprentice_Spouse_Name = client.Entity.Spouse;
                    AppInfo.Enrole_date = client.Mentor.EnroleDate;
                    AppInfo.Mentor_ID = client.Mentor.ID;
                    AppInfo.Mentor_Mesh = client.Mentor.Client.Entity.Mesh;
                    AppInfo.Mentor_Name = client.Mentor.Name;
                    AppInfo.Type = 2;
                    client.Mentor.Client.Send(AppInfo);
                }
                else
                {
                    MentorInformation Information = new MentorInformation(true);
                    Information.Mentor_Type = 1;
                    Information.Mentor_ID = client.Mentor.ID;
                    Information.Apprentice_ID = client.Entity.UID;
                    Information.Enrole_Date = client.Mentor.EnroleDate;
                    Information.Mentor_Online = false;
                    Information.String_Count = 2;
                    Information.Mentor_Name = client.Mentor.Name;
                    Information.Apprentice_Name = client.Entity.Name;

                    client.Send(Information);
                }
            }
            NobilityInfo update = new NobilityInfo(true);
            update.Type = NobilityInfo.Icon;
            update.dwParam = client.NobilityInformation.EntityUID;
            update.UpdateString(client.NobilityInformation);
            client.Send(update);
            client.Entity.Update(Update.Merchant, 255, false);

            client.Entity.Stamina = 100;
            //client.Send(new Message("Welcome to the Reborn of " + ServerBase.Constants.ServerName + "Conquer ! Same staff here", System.Drawing.Color.Red, Message.Talk));
            //client.Send(new Message("All monster drop CpsBag! With random Cps! ", System.Drawing.Color.Red, Message.Talk));
            //client.Send(new Message("If you need help! Use Service Chat", System.Drawing.Color.Red, Message.Talk));
            //client.Send(new Message("GuildWar Start: Saturday Hour: 18", System.Drawing.Color.Red, Message.Talk));
            //client.Send(new Message("GuildWar Finish: Sunday Hour: 10", System.Drawing.Color.Red, Message.Talk));
            string[] wm = File.ReadAllLines(ServerBase.Constants.WelcomeMessages);
                foreach (string line in wm)
                {
                    if (line.Length == 0)
                        continue;
                    if (line[0] == ';')
                        continue;
                    client.Send(new Message(line, System.Drawing.Color.Red, Message.Talk));
                }

            //if (Database.VoteTable.CanVote(client))
            //{
            //    client.OnMessageBoxOK = delegate
            //    {
            //        Network.GamePackets.Data data = new Network.GamePackets.Data(true);
            //        data.UID = client.Entity.UID;
            //        data.ID = Network.GamePackets.Data.OpenCustom;
            //        data.dwParam = Network.GamePackets.Data.CustomCommands.Minimize;
            //        client.Send(data);
            //        client.Send(new Message(ServerBase.Constants.ServerWebsite + ServerBase.Constants.WebAccExt + ServerBase.Constants.WebVoteExt, System.Drawing.Color.Red, Network.GamePackets.Message.Website));
            //    };
            //    client.Send(new NpcReply(NpcReply.MessageBox, "Would you like to consider voting for us? With more votes we will be come a bigger community."));
            //}

            /*if (client.Entity.VIPLevel != 0)
            {
                Database.PremiumTable.getVipInfo(client);
                if (client.VIPDays != 0)
                {
                    if (DateTime.Now >= client.VIPDate.AddDays(client.VIPDays))
                    {
                        client.Entity.VIPLevel = 0;
                        client.Send(ServerBase.Constants.VIPExpired);
                    }
                    else
                    {
                        DateTime VipEnds = client.VIPDate.AddDays(client.VIPDays);
                        TimeSpan span = VipEnds.Subtract(DateTime.Now);
                        client.Send(ServerBase.Constants.VIPRemaining(span.Days.ToString(), span.Hours.ToString()));
                    }
                }
                else
                    client.Send(ServerBase.Constants.VIPLifetime);
            }*/
            //if (Program.Today == DayOfWeek.Saturday || Program.Today == DayOfWeek.Sunday)
            //    client.Send(new Message("Double experience is on.", System.Drawing.Color.Red, Message.World));

            client.Send(new MapStatus() { BaseID = client.Map.BaseID, ID = client.Map.ID, Status = Database.MapsTable.MapInformations[client.Map.ID].Status });

            if (client.Entity.Hitpoints == 0)
                client.Entity.Hitpoints = 1;
            client.Entity.VIPLevel = (byte)(client.Entity.VIPLevel + 0);
            client.Entity.HandleTiming = true;
            if (client.Entity.ExtraBattlePower != 0)
                client.Entity.Update(Network.GamePackets.Update.ExtraBattlePower, client.Entity.ExtraBattlePower, false);
            if (client.Guild != null)
                client.Guild.SendAllyAndEnemy(client);
            if (Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityID > 2)
                client.Send(new Network.GamePackets.Message(Game.ConquerStructures.Broadcast.CurrentBroadcast.Message, "ALLUSERS", Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityName, System.Drawing.Color.Red, Network.GamePackets.Message.BroadcastMessage));
            client.Entity.Update(Network.GamePackets.Update.LuckyTimeTimer, client.BlessTime, false);
            if (client.Entity.HeavenBlessing != 0)
                client.Entity.Update(Network.GamePackets.Update.OnlineTraining, client.OnlineTrainingPoints, false);
            if (client.ClaimableItem.Count > 0)
                foreach (var item in client.ClaimableItem.Values)
                    item.Send(client);
            if (client.DeatinedItem.Count > 0)
                foreach (var item in client.DeatinedItem.Values)
                    item.Send(client);

            foreach (Interfaces.IConquerItem item in client.Inventory.Objects)
                item.Send(client);

            foreach (Interfaces.IConquerItem item in client.Equipment.Objects)
                if (item != null)
                {
                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(item.ID))
                    {
                        item.Send(client);
                        ItemUsage usage = new ItemUsage(true) { ID = ItemUsage.EquipItem };
                        usage.UID = item.UID;
                        usage.dwParam = item.Position;
                        client.Send(usage);
                        client.LoadItemStats(item);
                    }
                    else
                    {
                        client.Equipment.DestroyArrow(item.Position);
                        Console.WriteLine("Announcement: Item have been removed because of invalid info. UID: " + item.UID + ". OWNER: " + client.Entity.Name);
                    }
                }
            if (!client.Equipment.Free(5))
            {
                if (IsArrow(client.Equipment.TryGetItem(5).ID))
                {
                    if (client.Equipment.Free(4))
                        client.Equipment.DestroyArrow(5);
                    else
                    {
                        if (client.Equipment.TryGetItem(4).ID / 1000 != 500)
                            client.Equipment.DestroyArrow(5);
                    }
                }
            }
            client.GemAlgorithm();
            client.CalculateStatBonus();
            client.CalculateHPBonus();
            client.SendStatMessage();
           // Conquer_Online_Server.Game.Attacking.Calculate.Vitals(client.Entity, true);
            client.Equipment.UpdateEntityPacket();
            EntityEquipment equips = new EntityEquipment(true);
            equips.ParseHero(client);
            client.Send(equips);
            System.Threading.Thread.Sleep(3500);
            client.Send(new Conquer_Online_Server.Network.GamePackets.Message("http://www.xtremetop100.com/in.php?site=1132311748", 2105));
            //if ((int)client.Account.State >= 3)
            //    client.Entity.AddFlag(Update.Flags.Flashy);
        }
        #endregion
        #region Team
        public static void AcceptInviteToJoinTeam(Team team, Client.GameState client)
        {
            if (client.Team == null && !client.Entity.Dead)
            {
                Client.GameState Leader;
                if (ServerBase.Kernel.GamePool.TryGetValue(team.UID, out Leader))
                {
                    if (Leader.Team != null)
                    {
                        if (Leader.Team.Full || Leader.Team.ForbidJoin)
                            return;

                        client.Team = new Conquer_Online_Server.Game.ConquerStructures.Team();

                        AddToTeam AddYou = new AddToTeam();
                        AddToTeam AddMe = new AddToTeam();
                        AddMe.Name = client.Entity.Name;
                        AddMe.MaxHitpoints = (ushort)client.Entity.MaxHitpoints;
                        AddMe.Hitpoints = (ushort)client.Entity.Hitpoints;
                        AddMe.Mesh = client.Entity.Mesh;
                        AddMe.UID = client.Entity.UID;
                        foreach (Client.GameState Teammate in Leader.Team.Teammates)
                        {
                            if (Teammate != null)
                            {
                                Teammate.Send(AddMe);
                                client.Team.Add(Teammate);
                                AddYou.Name = Teammate.Entity.Name;
                                AddYou.MaxHitpoints = (ushort)Teammate.Entity.MaxHitpoints;
                                AddYou.Hitpoints = (ushort)Teammate.Entity.Hitpoints;
                                AddYou.Mesh = Teammate.Entity.Mesh;
                                AddYou.UID = Teammate.Entity.UID;
                                client.Send(AddYou);
                                if (Teammate.Entity.UID != Leader.Entity.UID)
                                    Teammate.Team.Add(client);
                            }
                        }
                        Leader.Team.Add(client);
                        client.Team.Add(client);
                        client.Team.Active = true;
                        client.Team.TeamLeader = false;
                        client.Send(AddMe);
                    }
                }
            }
        }
        public static void SendInviteToJoinTeam(Team team, Client.GameState client)
        {
            if (client.Team != null)
            {
                if (!client.Team.Full && client.Team.TeamLeader)
                {
                    Client.GameState Invitee;
                    if (ServerBase.Kernel.GamePool.TryGetValue(team.UID, out Invitee))
                    {
                        if (Invitee.Team == null)
                        {
                            team.UID = client.Entity.UID;
                            Invitee.Send(team);
                        }
                        else
                        {
                            client.Send(new Message(Invitee.Entity.Name + " is already in a team.", System.Drawing.Color.Purple, Message.TopLeft));
                        }
                    }
                }
            }
        }
        public static void AcceptRequestToJoinTeam(Team team, Client.GameState client)
        {
            if (client.Team != null && !client.Entity.Dead)
            {
                if (!client.Team.Full && client.Team.TeamLeader && !client.Team.ForbidJoin)
                {
                    Client.GameState NewTeammate;
                    if (ServerBase.Kernel.GamePool.TryGetValue(team.UID, out NewTeammate))
                    {
                        if (NewTeammate.Team != null)
                            return;

                        NewTeammate.Team = new Conquer_Online_Server.Game.ConquerStructures.Team();

                        AddToTeam AddMe = new AddToTeam();
                        AddToTeam AddYou = new AddToTeam();
                        AddYou.Name = NewTeammate.Entity.Name;
                        AddYou.MaxHitpoints = (ushort)NewTeammate.Entity.MaxHitpoints;
                        AddYou.Hitpoints = (ushort)NewTeammate.Entity.Hitpoints;
                        AddYou.Mesh = NewTeammate.Entity.Mesh;
                        AddYou.UID = NewTeammate.Entity.UID;
                        foreach (Client.GameState Teammate in client.Team.Teammates)
                        {
                            if (Teammate != null)
                            {
                                Teammate.Send(AddYou);
                                NewTeammate.Team.Add(Teammate);
                                AddMe.Name = Teammate.Entity.Name;
                                AddMe.MaxHitpoints = (ushort)Teammate.Entity.MaxHitpoints;
                                AddMe.Hitpoints = (ushort)Teammate.Entity.Hitpoints;
                                AddMe.Mesh = Teammate.Entity.Mesh;
                                AddMe.UID = Teammate.Entity.UID;
                                NewTeammate.Send(AddMe);
                                if (Teammate.Entity.UID != client.Entity.UID)
                                    Teammate.Team.Add(NewTeammate);
                            }
                        }

                        client.Team.Add(NewTeammate);
                        NewTeammate.Team.Add(NewTeammate);
                        NewTeammate.Team.Active = true;
                        NewTeammate.Team.TeamLeader = false;
                        client.Send(AddYou);
                        NewTeammate.Send(AddYou);
                    }
                }
            }
        }
        public static void SendRequestJoinToTeam(Team team, Client.GameState client)
        {
            if (client.Team == null && !client.Entity.Dead)
            {
                Client.GameState Leader;
                if (ServerBase.Kernel.GamePool.TryGetValue(team.UID, out Leader))
                {
                    if (Leader.Team != null)
                    {
                        if (Leader.Team.TeamLeader && !Leader.Team.Full)
                        {
                            team.UID = client.Entity.UID;
                            Leader.Send(team);
                        }
                        else
                        {
                            client.Send(new Message(Leader.Entity.Name + "'s team is already full.", System.Drawing.Color.Peru, Message.TopLeft));
                        }
                    }
                    else
                    {
                        client.Send(new Message(Leader.Entity.Name + "'s doesn't have a team.", System.Drawing.Color.Red, Message.TopLeft));
                    }
                }
            }
        }
        public static void LeaveTeam(Team team, Client.GameState client)
        {
            if (client.Team != null)
            {
                if (!client.Team.TeamLeader)
                {
                    foreach (Client.GameState Teammate in client.Team.Teammates)
                    {
                        if (Teammate != null)
                        {
                            if (Teammate.Entity.UID != client.Entity.UID)
                            {
                                Teammate.Send(team);
                                Teammate.Team.Remove(client.Entity.UID);
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
                                    client.Entity.Statistics.Immunity += 200;

                                }
                                client.Entity.RemoveFlag2(Update.Flags2.FendAura);
                                client.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                                client.Entity.RemoveFlag2(Update.Flags2.MetalAura);
                                client.Entity.RemoveFlag2(Update.Flags2.WoodAura);
                                client.Entity.RemoveFlag2(Update.Flags2.WaterAura);
                                client.Entity.RemoveFlag2(Update.Flags2.FireAura);
                                client.Entity.RemoveFlag2(Update.Flags2.EarthAura);
                                client.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                            }
                        }
                    }
                    client.Send(team);
                    client.Team = null;
                }
            }
        }
        public static void KickFromTeam(Team team, Client.GameState client)
        {
            if (client.Team != null)
            {
                if (client.Team.TeamLeader)
                {
                    Client.GameState Teammate; // The guy we're kicking out
                    if (ServerBase.Kernel.GamePool.TryGetValue(team.UID, out Teammate))
                    {
                        if (Teammate.Team != null)
                        {
                            if (Teammate.Team.IsTeammate(client.Entity.UID))
                            {
                                LeaveTeam(team, Teammate);
                            }
                        }
                    }
                }
            }
        }
        public static void DismissTeam(Team team, Client.GameState client)
        {
            if (client.Team != null)
            {
                if (!client.Entity.Dead && client.Team.TeamLeader)
                {
                    Leadership lship = new Leadership();
                    lship.Type = 1;
                    foreach (Client.GameState Teammate in client.Team.Teammates)
                    {
                        if (Teammate != null)
                        {
                            if (Teammate.Entity.UID != client.Entity.UID)
                            {
                                lship.UID = Teammate.Entity.UID;
                                Teammate.Send(lship);
                                Teammate.Send(team);
                                Teammate.Team = null;
                                if (Teammate.Entity.ContainsFlag2(Update.Flags2.TyrantAura))//FendAura
                                {
                                    Teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);//FendAura
                                    Teammate.Entity.Statistics.CriticalStrike -= 200;
                                }
                                if (Teammate.Entity.ContainsFlag2(Update.Flags2.MetalAura))//MetalAura
                                {
                                    Teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);//MetalAura
                                    Teammate.Entity.Statistics.MetalResistance -= 30;
                                }
                                if (Teammate.Entity.ContainsFlag2(Update.Flags2.WoodAura))//WoodAura
                                {
                                    Teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);//WoodAura
                                    Teammate.Entity.Statistics.WoodResistance -= 30;
                                }
                                if (Teammate.Entity.ContainsFlag2(Update.Flags2.WaterAura))//WaterAura
                                {
                                    Teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);//WaterAura
                                    Teammate.Entity.Statistics.WaterResistance -= 30;
                                }
                                if (Teammate.Entity.ContainsFlag2(Update.Flags2.FireAura))//FireAura
                                {
                                    Teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);//FireAura
                                    Teammate.Entity.Statistics.FireResistance -= 30;
                                }
                                if (Teammate.Entity.ContainsFlag2(Update.Flags2.EarthAura))//EarthAura
                                {
                                    Teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);//EarthAura
                                    Teammate.Entity.Statistics.EarthResistance -= 30;
                                }
                                if (Teammate.Entity.ContainsFlag2(Update.Flags2.FendAura))//TyrantAura
                                {
                                    Teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);//TyrantAura
                                    Teammate.Entity.Statistics.Immunity -= 200;
                                }
                                Teammate.Entity.RemoveFlag2(Update.Flags2.FendAura);
                                Teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                                Teammate.Entity.RemoveFlag2(Update.Flags2.MetalAura);
                                Teammate.Entity.RemoveFlag2(Update.Flags2.WoodAura);
                                Teammate.Entity.RemoveFlag2(Update.Flags2.WaterAura);
                                Teammate.Entity.RemoveFlag2(Update.Flags2.FireAura);
                                Teammate.Entity.RemoveFlag2(Update.Flags2.EarthAura);
                                Teammate.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                            }
                        }
                    }
                    lship.UID = client.Entity.UID;
                    client.Send(lship);
                    client.Send(team);
                    client.Team = null;
                    client.Entity.RemoveFlag2(Update.Flags2.FendAura);
                    client.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                    client.Entity.RemoveFlag2(Update.Flags2.MetalAura);
                    client.Entity.RemoveFlag2(Update.Flags2.WoodAura);
                    client.Entity.RemoveFlag2(Update.Flags2.WaterAura);
                    client.Entity.RemoveFlag2(Update.Flags2.FireAura);
                    client.Entity.RemoveFlag2(Update.Flags2.EarthAura);
                    client.Entity.RemoveFlag2(Update.Flags2.TyrantAura);
                    client.Entity.RemoveFlag(Update.Flags.TeamLeader);
                }
            }
        }
        public static void CreateTeam(Team team, Client.GameState client)
        {
            if (!client.Entity.Dead && client.Team == null)
            {
                Leadership lship = new Leadership();
                lship.Type = 1;
                lship.UID = client.Entity.UID;
                lship.LeaderUID = client.Entity.UID;
                lship.IsLeader = 1;
                client.Send(lship);
                client.Entity.AddFlag(Update.Flags.TeamLeader);
                client.Team = new Conquer_Online_Server.Game.ConquerStructures.Team();
                client.Team.Active = true;
                client.Team.TeamLeader = true;
                client.Team.Add(client);
                client.Send(team);
            }
        }
        #endregion
        public static byte[] WindowStats(Client.GameState client)
        {
            byte[] Pack = new byte[144];//132
            Writer.WriteUInt16(144 - 8, 0, Pack);
            Writer.WriteUInt16(1040, 2, Pack);
            Writer.WriteUInt32(client.Entity.UID, 4, Pack);
            Writer.WriteUInt32(client.Entity.Hitpoints, 8, Pack);
            Writer.WriteUInt32(client.Entity.Mana, 12, Pack);
            Writer.WriteUInt32(client.Entity.MinAttack, 20, Pack);
            Writer.WriteUInt32(client.Entity.MaxAttack, 16, Pack);
            Writer.WriteUInt32(client.Entity.Defence, 24, Pack);
            Writer.WriteUInt32(client.Entity.MagicAttack, 28, Pack);
            Writer.WriteUInt32((uint)client.Entity.MagicDefence, 32, Pack);
            Writer.WriteUInt32(client.Entity.Dodge, 36, Pack);
            Writer.WriteUInt32(client.Entity.Agility, 40, Pack);
            Writer.WriteUInt32(0, 44, Pack);//Accuracy
            Writer.WriteUInt32((uint)(client.Entity.Gems[1]), 48, Pack);
            Writer.WriteUInt32((uint)(client.Entity.Gems[0]), 52, Pack);
            Writer.WriteUInt32((uint)client.Entity.MagicDefencePercent, 56, Pack);
            Writer.WriteUInt32((uint)client.Entity.Gems[7], 60, Pack);
            Writer.WriteUInt32((uint)client.Entity.ItemBless, 64, Pack);

     
            Writer.WriteUInt32((uint)client.Entity.Statistics.CriticalStrike, 68, Pack);// CriticalStrike
            Writer.WriteUInt32((uint)client.Entity.Statistics.SkillCStrike, 72, Pack);// SkillCStrike
            Writer.WriteUInt32((uint)client.Entity.Statistics.Immunity, 76, Pack);// Immunity
            Writer.WriteUInt32((uint)(client.Entity.Statistics.Penetration), 80, Pack);// Immunity
            Writer.WriteUInt32((uint)client.Entity.Statistics.Block, 84, Pack);// Penetration
            Writer.WriteUInt32((uint)client.Entity.Statistics.Breaktrough, 88, Pack);// Block
            Writer.WriteUInt32(client.Entity.Statistics.Counteraction, 92, Pack);
            Writer.WriteUInt32((uint)(client.Entity.Statistics.Detoxication), 96, Pack); 
            //Writer.WriteUInt32((uint)client.Entity.Statistics.CriticalStrike, 68, Pack);// CriticalStrike
            //Writer.WriteUInt32((uint)client.Entity.Statistics.SkillCStrike, 72, Pack);// SkillCStrike
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Immunity, 76, Pack);// Immunity
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Penetration, 80, Pack);// Immunity
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Block, 84, Pack);// blok
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Breaktrough, 88, Pack);// Breaktrough
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Counteraction, 92, Pack);
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Detoxication, 96, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getFan(false)), 100, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getFan(true)), 104, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getTower(false)), 108, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getTower(true)), 112, Pack);

            Writer.WriteUInt32((uint)client.Entity.Statistics.MetalResistance, 116, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.WoodResistance, 120, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.WaterResistance, 124, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.FireResistance, 128, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.EarthResistance, 132, Pack);
            return Pack;
        }
    }
}
