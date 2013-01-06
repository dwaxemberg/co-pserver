using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Game.ConquerStructures.Society;
using Conquer_Online_Server.Game.ConquerStructures;

namespace NpcDialogs
{
    public class Dialogs
    {
        public static IniFile AvatarLinker = null;
        public GameState Client;
        public List<NpcReply> Replies;
        public void Text(string text)
        {
            if (text.Length > 100)
            {
                if (text.Length > 1000)
                    text = text.Substring(0, 1000);
                int myLength = text.Length;
                while (myLength > 0)
                {
                    int lastIndex = 100;
                    if (myLength < 100)
                        lastIndex = myLength;
                    string txt = text.Substring(0, lastIndex);
                    text = text.Substring(lastIndex, myLength - lastIndex);
                    myLength -= lastIndex;
                    Replies.Add(new NpcReply(NpcReply.Dialog, txt));
                }
            }
            else
                Replies.Add(new NpcReply(NpcReply.Dialog, text));
        }
        public void Option(string text, byte id)
        {
            Replies.Add(new NpcReply(NpcReply.Option, text) { OptionID = id });
        }
        public void Avatar(uint npcMesh)
        {
            if (Replies.Count == 0)
                Replies.Add(new NpcReply(NpcReply.Avatar, "") { InputMaxLength = (byte)npcMesh });
            else
                Replies[0].InputMaxLength = (byte)npcMesh;
        }
        public void Input(string text, byte id, byte maxLength)
        {
            Replies.Add(new NpcReply()
            {
                DontDisplay = true,
                InputMaxLength = maxLength,
                InteractType = NpcReply.Input,
                OptionID = id,
                Text = text
            });
        }
        public void Send()
        {
            foreach (NpcReply nr in Replies)
                Client.Send(nr);
            Client.Send(new NpcReply() { InteractType = 100, DontDisplay = false });
            Replies.Clear();
            Sent = true;
        }
        private bool Sent = false;
        public static ISkill LearnableSpell(ushort spellid)
        {
            ISkill spell = new Spell(true);
            spell.ID = spellid;
            return spell;
        }
        public static ISkill LearnableSpell(ushort spellid, byte level)
        {
            ISkill spell = new Spell(true);
            spell.ID = spellid;
            spell.Level = level;
            return spell;
        }
        public static bool CheckNumberPassword(string password)
        {
            if (password == "")
                return false;
            foreach (char c in password)
                if (!((byte)c >= 48 && (byte)c <= 57))
                    return false;
            return true;
        }
        public static void GetDialog(NpcRequest npcRequest, GameState client)
        {
            if (AvatarLinker == null)
                AvatarLinker = new IniFile("\\database\\npc.ini");
            //Console.WriteLine("[" + client.Entity.Name + "][NPC] " + npcRequest.NpcID + " : " + npcRequest.OptionID);
            if (!client.Map.Npcs.ContainsKey(client.ActiveNpc) || npcRequest == null || client == null || client.Entity == null || (npcRequest.NpcID == 0 && npcRequest.OptionID == 255))
                return;
            if (client.Trade != null)
                if (client.Trade.InTrade)
                    return;
            Dialogs dialog = new Dialogs();
            dialog.Client = client;
            dialog.Replies = new List<NpcReply>();
            INpc npcs = null;
            if (client.Map.Npcs.TryGetValue(client.ActiveNpc, out npcs))
            {
                ushort avatar = (ushort)AvatarLinker.ReadInt16("NpcType" + (npcs.Mesh / 10), "SimpleObjID", 1);

                dialog.Avatar(avatar);
            }

            switch (client.Entity.MapID)
            {
                #region Twin City
                case 1002:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Enter Guild Arena
                            case 380:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Would you like to enter the guild war arena?");
                                                dialog.Option("Yes.", 1);
                                                dialog.Option("No.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1038, 348, 339);
                                                break;
                                            }
                                    } break;
                                }
                            #endregion

                            #region Twin City
                            case 10080:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.PKPoints < 29)
                                                {
                                                    dialog.Text("Hey there " + client.Entity.Name + "! I'm the craziest conductress!\nI also know how to send you to all the main cities.\nAnyways... Where are you headed? I may be able to teleport you there for 5,000 gold.");
                                                    dialog.Option("Phoenix Castle.", 1);
                                                    dialog.Option("Ape City.", 2);
                                                    dialog.Option("Desert City.", 3);
                                                    dialog.Option("Bird Island.", 4);
                                                    dialog.Option("Mine Cave.", 5);
                                                    dialog.Option("Market.", 6);
                                                    dialog.Option("I don't care.", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not help MURDERS!");
                                                    dialog.Option("Ouch..", 255);
                                                    dialog.Option("Then you are of no use to me!\nI'll kill you too!", 100);
                                                    dialog.Send();
                                                    break;
                                                }
                                            }
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                        case 6:
                                            {
                                                ushort Map = 1002, X, Y;
                                                if (npcRequest.OptionID == 1)
                                                {
                                                    X = 958;
                                                    Y = 555;
                                                }
                                                else if (npcRequest.OptionID == 2)
                                                {
                                                    X = 555;
                                                    Y = 957;
                                                }
                                                else if (npcRequest.OptionID == 3)
                                                {
                                                    X = 69;
                                                    Y = 473;
                                                }
                                                else if (npcRequest.OptionID == 4)
                                                {
                                                    X = 232;
                                                    Y = 190;
                                                }
                                                else if (npcRequest.OptionID == 5)
                                                {
                                                    X = 53;
                                                    Y = 399;
                                                }
                                                else
                                                {
                                                    X = 211;
                                                    Y = 196;
                                                    Map = 1036;
                                                }
                                                if (client.Entity.Money >= 5000)
                                                {
                                                    client.Entity.Money -= 5000;
                                                    client.Entity.Teleport(Map, X, Y);
                                                }
                                                else
                                                {
                                                    dialog.Text("Hey! You don't have 5,000 gold do you?\nDon't you dare try to cheat me or I'll call the guards!");
                                                    dialog.Option("Crazy!", 255);
                                                    dialog.Send();
                                                }
                                                break;

                                            }
                                        case 100:
                                            {
                                                client.Entity.AddFlag(Update.Flags.FlashingName);
                                                client.Entity.FlashingNameStamp = Time32.Now;
                                                client.Entity.FlashingNameTime = 30;
                                                client.Send(Conquer_Online_Server.ServerBase.Constants.Warrent);
                                                dialog.Text("Guards! HELP! " + client.Entity.Name + " is trying to kill me!!");
                                                dialog.Option("I'll get you!", 255);
                                                dialog.Send();
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Celestine
                            case 20005:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Did you see those plants while comming over here? Do you see that pond right over there? You see I need clean water badly. If the clean water is united with 7 of 9 unique gems(except tortoise, thunder, glory) it can heal plants and clean ponds. In exchange of those 8 items, I will give you back a celestial stone, which is needed for first reborn.");
                                                dialog.Option("How do I get gems?", 1);
                                                dialog.Option("How do I get clean water?", 2);
                                                dialog.Option("I got the items.", 3);
                                                dialog.Option("Boring...", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("You can get gems simply by minning or killing monsters, as they drop gems sometime.");
                                                dialog.Option("How do I get clean water?", 2);
                                                dialog.Option("I got the items.", 3);
                                                dialog.Option("Boring...", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                dialog.Text("In the first Island Map in Adventure Zone(GC1), you might find the WaterLord(517,732). You may kill it and it may give a CleanWater, with a 50% percent of success. Once killed you may wait 15 minutes, before it will respawn again.");
                                                dialog.Option("How do I get gems?", 1);
                                                dialog.Option("I got the items.", 3);
                                                dialog.Option("Boring...", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                bool hasfornow = true;
                                                uint baseid = 700001;
                                                for (int c = 0; c < 70; c += 10)
                                                    if (!client.Inventory.Contains((uint)(baseid + c), 1))
                                                    {
                                                        hasfornow = false;
                                                        break;
                                                    }
                                                if (!hasfornow)
                                                {
                                                    dialog.Text("You don't meet the requierments!");
                                                    dialog.Send();
                                                    break;
                                                }
                                                if (client.Inventory.Contains(721258, 1))
                                                {
                                                    for (int c = 0; c < 70; c += 10)
                                                        client.Inventory.Remove((uint)(baseid + c), 1);
                                                    client.Inventory.Remove(721258, 1);
                                                    client.Inventory.Add(721259, 0, 1);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments!");
                                                    dialog.Send();
                                                    break;
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region GuildDirector
                            case 10003:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello there. Do you want to create a new guild? You need level 90 and 500000 silver and you have to not belong to any guild. If you are a guild leader, then you can name 5 deputy leaders.");
                                                dialog.Option("Create guild.", 1);
                                                dialog.Option("Name deputy leader.", 3);
                                                dialog.Option("Move leadership.", 6);
                                                dialog.Option("Disband guild.", 9);
                                                dialog.Option("I don't have that.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.Guild != null && client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                                {
                                                    dialog.Text("Are you sure you want to disband your guild?");
                                                    dialog.Option("Yes.", 10);
                                                    dialog.Option("Ah, nevermind.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 10:
                                            {
                                                if (client.Guild != null && client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                                {
                                                    client.Guild.Disband();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Guild != null && client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                                {
                                                    dialog.Text("Input here the member name you want to promote as guild leader.");
                                                    dialog.Input("Here:", 7, 16);
                                                    dialog.Option("Ah, nevermind.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Guild != null && client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                                {
                                                    if (npcRequest.Input != "")
                                                    {
                                                        var member = client.Guild.GetMemberByName(npcRequest.Input);
                                                        if (member == null)
                                                        {
                                                            dialog.Text("There is no such member in your guild.");
                                                            dialog.Option("Ah, nevermind.", 255);
                                                            dialog.Send();
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            if (member.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                                            {
                                                                dialog.Text("You cannot promote this member anymore.");
                                                                dialog.Option("Ah, nevermind.", 255);
                                                                dialog.Send();
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                if (member.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader)
                                                                    client.Guild.DeputyLeaderCount--;
                                                                member.Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader;
                                                                if (member.IsOnline)
                                                                {
                                                                    client.Guild.SendGuild(member.Client);
                                                                    member.Client.Entity.GuildRank = (ushort)member.Rank;
                                                                    member.Client.Screen.FullWipe();
                                                                    member.Client.Screen.Reload(null);
                                                                }
                                                                client.AsMember.Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader;
                                                                if (client.AsMember.IsOnline)
                                                                {
                                                                    client.Guild.SendGuild(client);
                                                                    client.Entity.GuildRank = (ushort)member.Rank;
                                                                    client.Screen.FullWipe();
                                                                    client.Screen.Reload(null);
                                                                }
                                                                client.Guild.DeputyLeaderCount++;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Guild != null && client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                                {
                                                    dialog.Text("You have now " + client.Guild.DeputyLeaderCount + " named deupty leaders.");
                                                    if (client.Guild.DeputyLeaderCount == 5)
                                                    {
                                                        dialog.Text("You cannot name any other deupty leader.");
                                                        dialog.Option("Ah, nevermind.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Input here the member name you want to promote as deputy leader.");
                                                        dialog.Input("Here:", 4, 16);
                                                        dialog.Option("Ah, nevermind.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 4:
                                            {
                                                if (client.Guild != null && client.AsMember.Rank == Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader)
                                                {
                                                    if (npcRequest.Input != "")
                                                    {
                                                        var member = client.Guild.GetMemberByName(npcRequest.Input);
                                                        if (member == null)
                                                        {
                                                            dialog.Text("There is no such member in your guild.");
                                                            dialog.Option("Ah, nevermind.", 255);
                                                            dialog.Send();
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            if (member.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                            {
                                                                dialog.Text("You cannot promote this member anymore.");
                                                                dialog.Option("Ah, nevermind.", 255);
                                                                dialog.Send();
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                member.Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.DeputyLeader;
                                                                if (member.IsOnline)
                                                                {
                                                                    client.Guild.SendGuild(member.Client);
                                                                    member.Client.Entity.GuildRank = (ushort)member.Rank;
                                                                    member.Client.Screen.FullWipe();
                                                                    member.Client.Screen.Reload(null);
                                                                }
                                                                client.Guild.DeputyLeaderCount++;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Guild == null && client.Entity.Level >= 90 && client.Entity.Money >= 500000)
                                                {
                                                    dialog.Text("Name your guild. The name must have less than 16 characters of any type.");
                                                    dialog.Input("Here:", 2, 16);
                                                    dialog.Option("Ah, nevermind.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Guild == null && client.Entity.Level >= 90 && client.Entity.Money >= 500000)
                                                {
                                                    if (npcRequest.Input != "")
                                                    {
                                                        client.Entity.Money -= 500000;
                                                        Guild guild = new Guild(client.Entity.Name);
                                                        guild.ID = Guild.GuildCounter.Next;
                                                        guild.SilverFund = 500000;
                                                        client.AsMember = new Conquer_Online_Server.Game.ConquerStructures.Society.Guild.Member(guild.ID)
                                                        {
                                                            SilverDonation = 500000,
                                                            ID = client.Entity.UID,
                                                            Level = client.Entity.Level,
                                                            Name = client.Entity.Name,
                                                            Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader,
                                                        };
                                                        if (client.NobilityInformation != null)
                                                        {
                                                            client.AsMember.Gender = client.NobilityInformation.Gender;
                                                            client.AsMember.NobilityRank = client.NobilityInformation.Rank;
                                                        }

                                                        client.Entity.GuildID = (ushort)guild.ID;
                                                        client.Entity.GuildRank = (ushort)Conquer_Online_Server.Game.Enums.GuildMemberRank.GuildLeader;
                                                        guild.Leader = client.AsMember;
                                                        client.Guild = guild;
                                                        guild.Create(npcRequest.Input);
                                                        guild.MemberCount++;
                                                        guild.SendGuild(client);
                                                        guild.SendName(client);
                                                        client.Screen.FullWipe();
                                                        client.Screen.Reload(null);
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Barber
                            case 10002:
                                {
                                    dialog.Avatar(241);
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("What kind of hair style would you like?");
                                                dialog.Option("New Styles", 1);
                                                dialog.Option("Nostalgic Styles", 2);
                                                dialog.Option("Special Styles", 3);
                                                dialog.Option("Nevermind", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        #region New Hair Styles
                                        #region Options
                                        case 1:
                                            {

                                                dialog.Text("Which new style do you want?");
                                                dialog.Option("New Hairstyle 1", 10);
                                                dialog.Option("New Hairstyle 2", 11);
                                                dialog.Option("New Hairstyle 3", 12);
                                                dialog.Option("New Hairstyle 4", 13);
                                                dialog.Option("New Hairstyle 5", 14);
                                                dialog.Option("New Hairstyle 6", 15);
                                                dialog.Option("New Hairstyle 7", 16);
                                                dialog.Option("More styles.", 102);
                                                dialog.Send();
                                                break;

                                            }
                                        case 102:
                                            {

                                                dialog.Text("Which new style do you want?");
                                                dialog.Option("New Hairstyle 8", 17);
                                                dialog.Option("New Hairstyle 9", 18);
                                                dialog.Option("New Hairstyle 10", 19);
                                                dialog.Option("New Hairstyle 11", 20);
                                                dialog.Option("New Hairstyle 12", 21);
                                                dialog.Option("Back.", 1);
                                                dialog.Option("Nevermind", 255);
                                                dialog.Send();
                                                break;

                                            }
                                        #endregion
                                        #region ...
                                        case 10:
                                        case 11:
                                        case 12:
                                        case 13:
                                        case 14:
                                        case 15:
                                        case 16:
                                        case 17:
                                        case 18:
                                        case 19:
                                        case 20:
                                        case 21:
                                            {
                                                client.Entity.HairStyle = ushort.Parse(Convert.ToString(client.Entity.HairStyle)[0] + (20 + npcRequest.OptionID).ToString());
                                                dialog.Text("Done!");
                                                dialog.Option("Thanks.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        #endregion
                                        #endregion
                                        #region Nostalgic Hair Styles
                                        #region Options
                                        case 2:
                                            {

                                                dialog.Text("Which new style do you want?");
                                                dialog.Option("Nostalgic Hairstyle 1", 30);
                                                dialog.Option("Nostalgic Hairstyle 2", 31);
                                                dialog.Option("Nostalgic Hairstyle 3", 32);
                                                dialog.Option("Nostalgic Hairstyle 4", 33);
                                                dialog.Option("Nostalgic Hairstyle 5", 34);
                                                dialog.Option("Nostalgic Hairstyle 6", 35);
                                                dialog.Option("Nostalgic Hairstyle 7", 36);
                                                dialog.Option("Nevermind.", 255);
                                                dialog.Send();
                                                break;

                                            }
                                        #endregion
                                        #region ...
                                        case 30:
                                        case 31:
                                        case 32:
                                        case 33:
                                        case 34:
                                        case 35:
                                        case 36:
                                            {
                                                client.Entity.HairStyle = ushort.Parse(Convert.ToString(client.Entity.HairStyle)[0] + (npcRequest.OptionID - 20).ToString());
                                                dialog.Text("Done!");
                                                dialog.Option("Thanks.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        #endregion
                                        #endregion
                                        #region Special Hair Styles
                                        #region Options
                                        case 3:
                                            {

                                                dialog.Text("Which new style do you want?");
                                                dialog.Option("New Hairstyle 1", 40);
                                                dialog.Option("New Hairstyle 2", 41);
                                                dialog.Option("New Hairstyle 3", 42);
                                                dialog.Option("New Hairstyle 4", 43);
                                                dialog.Option("New Hairstyle 5", 44);
                                                dialog.Option("Nevermind.", 255);
                                                dialog.Send();
                                                break;

                                            }
                                        #endregion
                                        #region ...
                                        case 40:
                                        case 41:
                                        case 42:
                                        case 43:
                                        case 44:
                                            {
                                                client.Entity.HairStyle = ushort.Parse(Convert.ToString(client.Entity.HairStyle)[0] + (npcRequest.OptionID - 19).ToString());
                                                dialog.Text("Done!");
                                                dialog.Option("Thanks.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        #endregion
                                        #endregion

                                    }
                                    break;
                                }
                            #endregion

                            #region Assistant
                            case 4293:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Do you want to visit the mine?");
                                                dialog.Option("Yes.", 1);
                                                dialog.Option("No.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1028, 155, 95);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region General Peace
                            case 10054:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Stop!\nThe path ahead of you is dangerous.\nI don't recommend you continue on this path.\n");
                                                if (client.Entity.Level <= 70)
                                                {
                                                    dialog.Text("If you are looking from stronger monsters you should look in the Canyon first.\n");
                                                    dialog.Text("You can find your way there in the south-west part of the wind plain.\n");
                                                }
                                                dialog.Option("Leave me alone General.", 1);
                                                dialog.Option("Okay, thanks for the warning.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1000, 973, 668);
                                                break;
                                            }
                                    }
                                    break;
                                }

                            #endregion

                            #region WarehouseGuardian
                            case 1061: //WarehouseGuardian
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello. I'm the one that was choosen to set/change/remove the password of the warehouse. I must say without a password you could lose a lot yet, which you probably already knew. My advice for you is that you should have a password, because it could save you of many problems. If you have a password yet, you can remove or change it.");
                                                if (client.WarehousePW == "")
                                                {
                                                    dialog.Option("Okay, let me set the password.", 1);
                                                }
                                                else
                                                {
                                                    dialog.Option("I want to change the password.", 2);
                                                    dialog.Option("I want to remove the password.", 3);
                                                }
                                                dialog.Option("Forget it.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        #region Set password
                                        case 1:
                                            {
                                                if (client.WarehousePW == "")
                                                {
                                                    dialog.Text("Please insert a password that's built only with numbers and whose length is less or equal to 8. An example can be 74352.");
                                                    dialog.Input("Here:", 4, 8);
                                                    dialog.Option("Forget it.", 255);
                                                }
                                                else
                                                {
                                                    dialog.Text("You have a password already set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        case 4:
                                            {
                                                if (client.WarehousePW == "")
                                                {
                                                    if (CheckNumberPassword(npcRequest.Input) && npcRequest.Input.Length <= 8)
                                                    {
                                                        client.TempPassword = npcRequest.Input;
                                                        dialog.Text("Please insert the password again.");
                                                        dialog.Input("Here:", 5, 8);
                                                        dialog.Option("Forget it.", 255);
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Invalid password, please try again.");
                                                        dialog.Input("Here:", 4, 8);
                                                        dialog.Option("Alright.", 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You have a password already set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.WarehousePW == "")
                                                {
                                                    if (CheckNumberPassword(npcRequest.Input) && npcRequest.Input.Length <= 8)
                                                    {
                                                        if (client.TempPassword == npcRequest.Input)
                                                        {
                                                            client.TempPassword = "";
                                                            client.WarehousePW = npcRequest.Input;
                                                            dialog.Text("Password set!");
                                                            dialog.Option("Thank you.", 255);
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("The password doesn't match. Insert again?");
                                                            dialog.Input("Here:", 5, 8);
                                                            dialog.Option("Forget it.", 255);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Invalid password, please try again.");
                                                        dialog.Input("Here:", 5, 8);
                                                        dialog.Option("Alright.", 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You have a password already set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        #endregion
                                        #region Change password
                                        case 2:
                                            {
                                                if (client.WarehousePW != "")
                                                {
                                                    dialog.Text("Please insert the password you have now.");
                                                    dialog.Input("Here:", 6, 8);
                                                    dialog.Option("Forget it.", 255);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a password set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.WarehousePW != "")
                                                {
                                                    if (CheckNumberPassword(npcRequest.Input) && npcRequest.Input.Length <= 8)
                                                    {
                                                        if (client.WarehousePW == npcRequest.Input)
                                                        {
                                                            dialog.Text("Please insert the password again.");
                                                            dialog.Input("Here:", 7, 8);
                                                            dialog.Option("Forget it.", 255);
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("Wrong password. Try again?");
                                                            dialog.Input("Here:", 6, 8);
                                                            dialog.Option("Alright.", 255);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Invalid password, please try again.");
                                                        dialog.Input("Here:", 6, 8);
                                                        dialog.Option("Alright.", 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a password set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.WarehousePW != "")
                                                {
                                                    if (CheckNumberPassword(npcRequest.Input) && npcRequest.Input.Length <= 8)
                                                    {
                                                        client.TempPassword = npcRequest.Input;
                                                        dialog.Text("Please insert the password again.");
                                                        dialog.Input("Here:", 8, 8);
                                                        dialog.Option("Forget it.", 255);
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Invalid password, please try again.");
                                                        dialog.Input("Here:", 7, 8);
                                                        dialog.Option("Alright.", 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a password set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.WarehousePW != "")
                                                {
                                                    if (CheckNumberPassword(npcRequest.Input) && npcRequest.Input.Length <= 8)
                                                    {
                                                        if (client.TempPassword == npcRequest.Input)
                                                        {
                                                            client.TempPassword = "";
                                                            client.WarehousePW = npcRequest.Input;
                                                            dialog.Text("Password changed!");
                                                            dialog.Option("Thank you.", 255);
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("Wrong password.");
                                                            dialog.Input("Here:", 8, 8);
                                                            dialog.Option("Alright.", 255);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Invalid password, please try again.");
                                                        dialog.Input("Here:", 8, 8);
                                                        dialog.Option("Alright.", 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a password set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        #endregion
                                        #region Remove password
                                        case 3:
                                            {
                                                if (client.WarehousePW != "")
                                                {
                                                    dialog.Text("Please insert the password you have now.");
                                                    dialog.Input("Here:", 9, 8);
                                                    dialog.Option("Forget it.", 255);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a password set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.WarehousePW != "")
                                                {
                                                    if (CheckNumberPassword(npcRequest.Input) && npcRequest.Input.Length <= 8)
                                                    {
                                                        if (client.WarehousePW == npcRequest.Input)
                                                        {
                                                            dialog.Text("Please insert the password again.");
                                                            dialog.Input("Here:", 10, 8);
                                                            dialog.Option("Forget it.", 255);
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("Wrong password. Try again?");
                                                            dialog.Input("Here:", 9, 8);
                                                            dialog.Option("Alright.", 255);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Invalid password, please try again.");
                                                        dialog.Input("Here:", 9, 8);
                                                        dialog.Option("Alright.", 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a password set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        case 10:
                                            {
                                                if (client.WarehousePW != "")
                                                {
                                                    if (CheckNumberPassword(npcRequest.Input) && npcRequest.Input.Length <= 8)
                                                    {
                                                        if (client.WarehousePW == npcRequest.Input)
                                                        {
                                                            client.WarehousePW = "";
                                                            client.WarehouseOpen = true;
                                                            dialog.Text("Password removed.");
                                                            dialog.Option("Thank you.", 255);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Invalid password, please try again.");
                                                        dialog.Input("Here:", 10, 8);
                                                        dialog.Option("Alright.", 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a password set.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                dialog.Send();
                                                break;
                                            }
                                        #endregion
                                    }
                                    break;
                                }
                            #endregion

                            #region Jail npc
                            case 10081: //Jail npc
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello there. I can teleport you in jail for the amount of 1000 silvers. Do you want to proceed?");
                                                dialog.Option("Sure.", 1);
                                                dialog.Option("I'm standing by.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 1000)
                                                {
                                                    client.Entity.Money -= 1000;
                                                    client.Entity.Teleport(6000, 32, 72);
                                                }
                                                else
                                                {
                                                    dialog.Text("You need 1000 silvers to be able to enter the jail.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Arena npc
                            case 10021: //Arena npc
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello there. I can teleport you in jail for the amount of 50 silvers. Do you want to proceed?");
                                                dialog.Option("Sure.", 1);
                                                dialog.Option("I'm standing by.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 50)
                                                {
                                                    client.Entity.Money -= 50;
                                                    client.Entity.Teleport(1005, 51, 71);
                                                }
                                                else
                                                {
                                                    dialog.Text("You need 50 silvers to be able to enter the jail.");
                                                    dialog.Option("Alright.", 255);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region ShopBoy
                            case 10063: //ShopBoy
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello there. I can teleport you in our shop, but I can also dye your armor black for the price of one DragonBall. What do you say?");
                                                dialog.Option("Teleport me in the shop.", 1);
                                                dialog.Option("Dye my armor black.", 2);
                                                dialog.Option("I'm standing by.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1008, 22, 26);
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Inventory.Contains(1088000, 1))
                                                {
                                                    IConquerItem armor = client.Equipment.TryGetItem(3);
                                                    if (armor != null && armor.ID != 0)
                                                    {
                                                        client.Inventory.Remove(1088000, 1);
                                                        armor.Color = Conquer_Online_Server.Game.Enums.Color.Black;
                                                        Conquer_Online_Server.Database.ConquerItemTable.UpdateColor(armor, client);
                                                        armor.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                        armor.Send(client);
                                                        client.Equipment.UpdateEntityPacket();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't wear an armor right now so I can't do anything.");
                                                        dialog.Option("Ah alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("Hmm...You don't have one DragonBall so I can't dye your armor.");
                                                    dialog.Option("Ah alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Kungfu Boy
                            case 100229:
                                {
                                    // If inv contains jade glyph
                                    // npcRequest.OptionID = 5
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                //#warning (Kungfu Boy) Check for begenniers quest, check for the note. Echo this text.
                                                dialog.Text("I started learning Kungfu when I was very young.\nNow I have learn to master my Chi very well! Wanna have a try?");
                                                dialog.Option("Yeah!", 1);
                                                dialog.Option("I don't care...", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("Oh, a message from the Village Gateman? I see.\nHe wants me to teach you how to master your Chi.");
                                                dialog.Option("I'm listening.", 2);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                dialog.Text("Press the Ctrl button, move the mouse arrow to the place you want to jump to and left click.");
                                                dialog.Text("You should know, you can't jump to a place you can't just normally walk to sometimes.");
                                                dialog.Option("Gotcha.", 3);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                dialog.Text("Good! It seems that you have mastered this skill. One more thing!\n");
                                                dialog.Text("My sister, Kungfu Girl, has stolen my Jade Glyph and taken it up onto the roof behind me. Can you go get it for me?");
                                                dialog.Option("No problem.", 4);
                                                dialog.Send();
                                                break;
                                            }
                                        case 4:
                                            {
                                                //#warning (Kungfu boy) Add quest progress
                                                client.Send(new Message("Jump to the roof behind Kungfu Boy and catch his mischievous sister!", System.Drawing.Color.Green, Message.PopUP));
                                                break;
                                            }
                                        case 5:
                                            {
                                                dialog.Text("Hey you got it! Thank you. I see you've mastered your Chi.");
                                                dialog.Option("I have.", 6);
                                                dialog.Send();
                                                break;
                                            }
                                        case 6:
                                            {
                                                dialog.Text("Well, thats good. I storngly recommend you go visit the Armorer in Twin City.");
                                                dialog.Text(" You need to pick out some armor. It will save your life!");
                                                dialog.Option("I'll think about it.", 255);
                                                dialog.Send();
                                                break;
                                            }

                                    }
                                    break;
                                }

                            #endregion

                            #region Kungfu Girl
                            case 127271:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Haha! I wanted to play a trick on my brother! Don't tell him.");
                                                dialog.Option("He knows already.", 2);
                                                dialog.Option("You are very naughty.", 1);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("Teehee!");
                                                dialog.Option("He knows about this, you know.", 2);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                dialog.Text("Oh, well you got me.... Alright. Here's his Jade Glyph. Take it.");
                                                dialog.Option("Good!", 3);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                //#warning (Kungfu boy) Add quest progress
                                                client.Send(new Message("You got Kungfu Boy's Jade Glyph back from Kungfu Girl! Take it back to Kungfu Boy.", System.Drawing.Color.Green, Message.PopUP));
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Simon
                            case 1152:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hmm, a few years ago, me and some friends of mine, started exploring the mountains. It was an usual day, cloudy with a very happy sun. We all went on the mountain to explore and level up a bit. Somehow we was all tricked into seeing a fantasy land and we went that way. We found a big rock that looked like a door, we broke it and went in. Since then, my friends are in the place we found and which we called the labirinth as it was full of powerful creatures that were leading each of the four levels of the labirith. Each friend of mine chose a floor and they await great conquerors at the end of each floor. ");
                                                dialog.Option("Tell me more about it.", 1);
                                                dialog.Option("Blah, boring.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("In each floor there are three kinds of monsters, floor one has Slingers, GoldGhosts and the powerful Gibbon. The second floor has the AgileRats, Bladelings and the snake Naga. The third floor has BlueBirds, FiendBats and the mighty Talon. The last floor, the fourth, has MinotaurL120 and the Syren. Each of them drops some items, and you'll need some to get to the next floor, or some others to claim a reward from me.");
                                                dialog.Option("What rewards?", 2);
                                                dialog.Option("Blah, boring.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                dialog.Text("In exchange of 17 SunDiamonds(dropped by Slingers) I will give you two meteors, for 17 MoonDiamonds(dropped by Bladelings), I will give you 4 meteors, for 17 StarDiamonds(dropped by BlueBirds), I will give you 4 meteor tears, and lastly for 17 CloudDiamonds, I will give you one DragonBall. More over, to get to the second floor from first one you need a SkyToken(dropped by GoldGhost), from second to third a EarthToken(dropped by AgileRat) and from third to fourth a SoulToken(dropped by FiendBat).");
                                                dialog.Option("Enough talking.", 3);
                                                dialog.Option("Claim my reward.", 4);
                                                dialog.Option("Blah, boring.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                dialog.Text("Right. In exchange of 2000 VirtuePoints, I will be more than glad to teleport you in the first floor of the Labirith.");
                                                dialog.Option("Here, take them.", 5);
                                                dialog.Option("Blah, boring.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 4:
                                            {
                                                dialog.Text("What do you want to claim?");
                                                dialog.Option("Two Meteors.", 6);
                                                dialog.Option("Four Meteors.", 7);
                                                dialog.Option("Four Meteor tears.", 8);
                                                dialog.Option("One DragonBall.", 9);
                                                dialog.Option("Blah, boring.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.VirtuePoints >= 2000)
                                                {
                                                    client.VirtuePoints -= 2000;
                                                    client.Entity.Teleport(1351, 018, 127);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh, sorry.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Inventory.Contains(721533, 17))
                                                {
                                                    client.Inventory.Remove(721533, 17);
                                                    client.Inventory.Add(1088001, 0, 2);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh, sorry.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Inventory.Contains(721534, 17))
                                                {
                                                    client.Inventory.Remove(721534, 17);
                                                    client.Inventory.Add(1088001, 0, 4);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh, sorry.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.Inventory.Contains(721535, 17))
                                                {
                                                    client.Inventory.Remove(721535, 17);
                                                    client.Inventory.Add(1088002, 0, 4);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh, sorry.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.Inventory.Contains(721536, 17))
                                                {
                                                    client.Inventory.Remove(721536, 17);
                                                    client.Inventory.Add(1088000, 0, 1);
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh, sorry.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region MoonBoxQuest
                            #region FortuneTeller
                            case 600050:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello " + client.Entity.Name + ", Have you heard of the palace method?");
                                                dialog.Option("Palace method?..", 1);
                                                dialog.Option("Don't care!", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("I discovered a mystical tactic, but I was not able to solve it.\nI almost died! It's very dangerous.");
                                                dialog.Option("Sounds like my kind of fun.", 2);
                                                dialog.Option("Too dangerous", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {

                                                dialog.Text("I'm serious! You don't understand it's very dangerous...");
                                                dialog.Option("Yea yea. Just take my there.", 3);
                                                dialog.Option("Nevermind... o.o", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                client.Entity.Teleport(1042, 028, 033);
                                                dialog.Text("I warned you....");
                                                dialog.Option("I think you're full of it.\nThere's nothing here.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #endregion

                            #region FrozenGrottoGeneral
                            case 27837:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("There is a dangeous frozen cavern south of nowhere! It's too dangerous for you to enter! Even Leroy Jenkins died there!");
                                                dialog.Option("I want to enter the grotto.", 3);
                                                dialog.Option("Blah, boring.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                dialog.Text("Well, ok. But I'll only let you go in there if you've voted for the establishment!\nOtherwise you will need a substantial bribe...!");
                                                dialog.Option("Let me in!", 4);
                                                if (client.Entity.ConquerPoints >= 1200)
                                                    dialog.Option("Here's 1,200 CPs.", 5);
                                                else
                                                    dialog.Option("I'm too poor.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 5:
                                        case 4:
                                            {
                                                if (npcRequest.OptionID == 5)
                                                {
                                                    client.Entity.ConquerPoints -= 1200;
                                                    dialog.Text("A fine bribe! Although, you really should consider supporting the establishment!\n");
                                                }
                                                dialog.Text("Ok, well.. Which floor of the Grotto do you want to enter?\n");
                                                dialog.Option("Floor 1 [Lv127]", 1);
                                                dialog.Option("Floor 2 [Lv132]", 2);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1762, 61, 257);
                                                break;
                                            }
                                        case 2:
                                            {
                                                client.Entity.Teleport(7007, 61, 257);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                        }
                        break;
                    }
                #endregion
                #region MoonboxQuest
                case 1040:
                case 1041:
                case 1042:
                case 1043:
                case 1044:
                case 1045:
                case 1046:
                case 1047:
                case 1048:
                case 1049:
                case 1050:
                case 1051:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region MoonBoxQuest
                            #region Maggie
                            case 600003:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("What are you here for!?\nPlease do not go ahead, or you will enter the very dangerous tactics.");
                                                dialog.Option("....You too?", 1);
                                                dialog.Option("Send me to the tatics.", 4);
                                                dialog.Option("I had better leave here!", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("The tactic is so changeful, I once thought highly of my self\nand died from the damned tactics.");
                                                dialog.Option("How can I solve the tatics?", 2);
                                                dialog.Option("I do not believe in this!", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                dialog.Text("I have studied it for so may years, but fail to work it out.\nPerhaps you can... Would you like to give it a try?");
                                                dialog.Option("Yes. I like a good challenge.", 3);
                                                dialog.Option("I changed my mind..", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                dialog.Text("You will be entrapped in the tatic if you enter.\nYou won't be able to leave until you've found a special token.");
                                                dialog.Option("I'm ready to do this.", 5);
                                                dialog.Option("No. This is too risky.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 4:
                                            {
                                                dialog.Text("Alright, I'll take you there.\nYou must be sure to pick up the token!");
                                                dialog.Option("Okay.", 5);
                                                dialog.Send();
                                                break;
                                            }
                                        case 5:
                                            {
                                                ushort map = 1042;
                                                map += (ushort)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 8);
                                                client.Entity.Teleport(map, 210, 164);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #region GuardianGod
                            case 600004:
                            case 600006:
                            case 600008:
                            case 600010:
                            case 600012:
                            case 600014:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                string[] Tactics = { "Peace", "Chaos", "Deserted", "Disturbed", "Prosperous", "Calmed" };
                                                uint almostid = client.ActiveNpc / 2;
                                                uint baseId = 721010;
                                                uint tactic = almostid - 300002;
                                                baseId += tactic;
                                                if (client.Inventory.Contains(baseId, 1))
                                                {
                                                    dialog.Text("Congratulations, you have a token for this tactic. This tactic is " + Tactics[tactic] + ".");
                                                    dialog.Option("Ok, thanks!", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't have a token for this tactic, I'm afraid I cannot tell you which tactic is it.");
                                                    dialog.Option("Ahh, thanks anyway!", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                            }
                                    }
                                    break;
                                }
                            case 600016:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("You are in DeathTactic, you either die or proceed if you have 6 tokens that are not the same.");
                                                dialog.Option("Ok, thanks!", 255);
                                                dialog.Send();
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #region Ghost
                            case 600005:
                            case 600007:
                            case 600009:
                            case 600011:
                            case 600013:
                            case 600015:
                                {
                                    uint almostId = (client.ActiveNpc - 1) / 2;
                                    uint baseId = 721010;
                                    uint tactic = almostId - 300002;
                                    baseId += tactic;
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {

                                                if (client.Inventory.Contains(baseId, 1))
                                                {
                                                    dialog.Text("Nice you have the token!\nAre you ready to leave?");
                                                    dialog.Option("Yes.", 1);
                                                    dialog.Option("No.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("How dare you bother me without the token!\nYou will not leave this place until the token has been found!");
                                                    dialog.Option("Sorry I'll go find it!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Inventory.Contains(baseId, 1))
                                                {
                                                    client.Entity.Teleport(1042, 028, 033);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 600017:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("I've been trapped here forever.\nI didn't have all the token before coming here.");
                                                dialog.Option("What about me?", 1);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("If you have all the tokens you may leave this place with a moonbox.\nIf you don't.... You must die.");
                                                dialog.Option("I have them! Please don't kill me!", 2);
                                                dialog.Option("Oh no...", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Inventory.Contains(721010, 1) &&
                                                         client.Inventory.Contains(721011, 1) &&
                                                         client.Inventory.Contains(721012, 1) &&
                                                         client.Inventory.Contains(721013, 1) &&
                                                         client.Inventory.Contains(721014, 1) &&
                                                         client.Inventory.Contains(721015, 1))
                                                {
                                                    client.Inventory.Remove(721010, 1);
                                                    client.Inventory.Remove(721011, 1);
                                                    client.Inventory.Remove(721012, 1);
                                                    client.Inventory.Remove(721013, 1);
                                                    client.Inventory.Remove(721014, 1);
                                                    client.Inventory.Remove(721015, 1);

                                                    client.Inventory.Add(721072, 0, 1);
                                                    client.Entity.Teleport(1050, 210, 164);
                                                }
                                                else
                                                {
                                                    dialog.Text("You're wrong. Enjoy your death.");
                                                    dialog.Option("No!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #region VagrantGhost
                            case 600018:
                            case 600019:
                            case 600020:
                            case 600021:
                            case 600022:
                            case 600023:
                            case 600024:
                            case 600025:
                            case 600026:
                            case 600027:
                            case 600028:
                            case 600029:
                            case 600030:
                            case 600031:
                            case 600032:
                            case 600033:
                            case 600034:
                            case 600035:
                            case 600036:
                            case 600037:
                            case 600038:
                            case 600039:
                            case 600040:
                            case 600041:
                            case 600042:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Help me... Please");
                                                dialog.Option("Who are you?", 1);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("I am a warrior trapped in these tactics please help me to revive!");
                                                dialog.Option("I have the SoulJade.", 2);
                                                dialog.Option("No.", 3);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Inventory.Contains(721072, 1))
                                                {
                                                    dialog.Text("Thank you! Will you recive my gift of a Moonbox?");
                                                    dialog.Option("Yes.", 3);
                                                    dialog.Option("Not needed.", 4);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not...even so, I shall help you leave this cursed map.");
                                                    dialog.Option("Please get me out of here.", 5);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Inventory.Contains(721072, 1))
                                                {
                                                    client.Inventory.Remove(721072, 1);
                                                    client.Inventory.Add(721080, 0, 1);
                                                    client.Entity.Teleport(1042, 028, 033);
                                                }
                                                break;
                                            }
                                        case 4:
                                            {
                                                client.Inventory.Remove(721072, 1);
                                                client.Entity.Teleport(1042, 028, 033);
                                                break;
                                            }
                                        case 5:
                                            {
                                                client.Entity.Teleport(1042, 028, 033);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Ape Canyon
                case 1020:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Ape City
                            case 10053:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello " + client.Entity.Name + ", I can send you on your way for just 100 silvers.");
                                                dialog.Text("\nWhere would you like to go?");
                                                dialog.Option("Twin City", 1);
                                                dialog.Option("Market", 2);
                                                dialog.Option("Nevermind", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Money -= 100;
                                                    client.Entity.Teleport(1020, 378, 13);
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Money -= 100;
                                                    client.Entity.Teleport(1036, 211, 196);
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #region Ape Canyon

                            #region Alex
                            case 3600:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("You don't worry, it's not gonna hurt you a bit when getting the third life. You just need an exemption token and it's all going to be fine.");
                                                dialog.Option("Here is the ExemptionToken.", 1);
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Reborn == 1)
                                                {
                                                    if (client.Entity.Class % 10 == 5 && client.Entity.Level >= (client.Entity.Class == 135 ? 110 : 120))
                                                    {
                                                        dialog.Text("Select the class you want to reborn in.");
                                                        dialog.Option("Trojan.", 11);
                                                        dialog.Option("Warrior.", 21);
                                                        dialog.Option("Archer.", 41);
                                                        dialog.Option("WaterTaoist.", 132);
                                                        dialog.Option("FireTaoist.", 142);
                                                        dialog.Option("Ninja.", 51);
                                                        dialog.Option("Monk.", 61);
                                                        dialog.Option("Nothing thank you.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("If you are a water saint you need level 110+, else you need 120+.");
                                                        dialog.Option("I'll just leave", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You need to be in the second life to be able to get the third life.");
                                                    dialog.Option("I'll just leave", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                if (npcRequest.OptionID == 255)
                                                    return;
                                                if (client.Entity.Reborn == 1)
                                                {
                                                    if (client.Entity.Class % 10 == 5 && client.Entity.Level >= (client.Entity.Class == 135 ? 110 : 120))
                                                    {
                                                        if (client.Inventory.Contains(723701, 1))
                                                        {
                                                            if (client.Reborn(npcRequest.OptionID))
                                                            {
                                                                client.Inventory.Remove(723701, 1);
                                                            }
                                                            else
                                                            {
                                                                dialog.Text("You need two free slots in your inventory.");
                                                                dialog.Option("I'll just leave", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("If you are a water saint you need level 110+, else you need 120+.");
                                                        dialog.Option("I'll just leave", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You need to be in the second life to be able to get the third life.");
                                                    dialog.Option("I'll just leave", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Assistant
                            case 126:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Do you want to visit the mine?");
                                                dialog.Option("Yes.", 1);
                                                dialog.Option("No.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1026, 142, 105);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #endregion
                        }
                        break;
                    }
                #endregion
                #region PhoenixCastle
                case 1011:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Phoenix Castle
                            case 10052:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello " + client.Entity.Name + ", I can send you on your way for just 100 silvers.");
                                                dialog.Text("\nWhere would you like to go?");
                                                dialog.Option("Market", 1);
                                                dialog.Option("Twin City", 2);
                                                dialog.Option("Nevermind", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Money -= 100;
                                                    client.Entity.Teleport(1036, 211, 196);
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Money -= 100;
                                                    client.Entity.Teleport(10, 377);
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #region PhoenixCity

                            #region Assistant
                            case 125:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Do you want to visit the mine?");
                                                dialog.Option("Yes.", 1);
                                                dialog.Option("No.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1025, 30, 71);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #endregion
                        }
                        break;
                    }
                #endregion
                #region DesertCity
                case 1000:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Desert city
                            case 10051:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello " + client.Entity.Name + ", I can set you on your way for just 100 silver.\n");
                                                dialog.Text("Where would you like to go?");
                                                dialog.Option("Twin City", 1);
                                                dialog.Option("Market", 3);
                                                dialog.Option("Mystic Castle", 2);
                                                dialog.Option("Nevermind.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Teleport(1000, 973, 668);
                                                    client.Entity.Money -= 100;
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Teleport(1000, 80, 320);
                                                    client.Entity.Money -= 100;
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Teleport(1036, 211, 196);
                                                    client.Entity.Money -= 100;
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #region DesertCity
                            #region Assistant
                            case 127:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Do you want to visit the mine?");
                                                dialog.Option("Yes.", 1);
                                                dialog.Option("No.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1027, 142, 105);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Bird Village
                case 1015:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Bird Island
                            case 10056:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello " + client.Entity.Name + ", I can send you on your way for just 100 silvers.");
                                                dialog.Text("\nWhere would you like to go?");
                                                dialog.Option("Twin City", 1);
                                                dialog.Option("Market", 2);
                                                dialog.Option("Nevermind", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Money -= 100;
                                                    client.Entity.Teleport(1015, 1015, 710);
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Money >= 100)
                                                {
                                                    client.Entity.Money -= 100;
                                                    client.Entity.Teleport(1036, 211, 196);
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have 100 silvers.");
                                                    dialog.Option("Aww!", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #region Bird Village

                            #region ArtisanOu
                            case 41:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello there my friend. I am here to make your socket your weapon. With a socketed weapon you can add gems inside of the sockets and the gems will give you some extra stats. For the first socket I demand 1 DragonBall and for the second one I demand 5 dragonballs.");
                                                dialog.Option("Alright let's go on.", 1);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (!client.Equipment.Free(4))
                                                {
                                                    Conquer_Online_Server.Interfaces.IConquerItem Item = null;
                                                    Item = client.Equipment.TryGetItem(4);
                                                    if (Item.SocketOne == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                                    {
                                                        dialog.Text("My friend I need one DragonBall to socket your weapon. Are you sure you want to do this?");
                                                        dialog.Option("Yea sure.", 2);
                                                        dialog.Send();
                                                    }
                                                    else if (Item.SocketTwo == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                                    {
                                                        dialog.Text("My friend I need five dragonballs to socket your weapon. Are you sure you want to do this?");
                                                        dialog.Option("Yea sure.", 2);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("I can't socket this weapon once more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You need to wear the weapon first.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (!client.Equipment.Free(4))
                                                {
                                                    Conquer_Online_Server.Interfaces.IConquerItem Item = null;
                                                    Item = client.Equipment.TryGetItem(4);
                                                    if (Item.SocketOne == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                                    {
                                                        if (client.Inventory.Contains(1088000, 1))
                                                        {
                                                            client.Inventory.Remove(1088000, 1);
                                                            Item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                            Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                            Item.Send(client);
                                                            Conquer_Online_Server.Database.ConquerItemTable.UpdateSockets(Item, client);
                                                            dialog.Text("Done!");
                                                            dialog.Option("Thank you.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't have one DragonBall!");
                                                            dialog.Option("Ah...", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else if (Item.SocketTwo == Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                                                    {
                                                        if (client.Inventory.Contains(1088000, 5))
                                                        {
                                                            client.Inventory.Remove(1088000, 5);
                                                            Item.SocketTwo = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                            Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                            Item.Send(client);
                                                            Conquer_Online_Server.Database.ConquerItemTable.UpdateSockets(Item, client);
                                                            dialog.Text("Done!");
                                                            dialog.Option("Thank you.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't have one DragonBall!");
                                                            dialog.Option("Ah...", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You need to wear the weapon first.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Market
                case 1036:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Market

                            #region Shelby
                            case 300000:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("I have here the list with the virtue of all the people in this world. You have so far " + client.VirtuePoints + " virtue points.");
                                                dialog.Option("Thank you.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region LadyLuck
                            case 923:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.InLottery)
                                                {
                                                    dialog.Text("My friend, did something happen to you?\nYou may still want to try your luck since you already paid me the fee.");
                                                    dialog.Option("Alright, take me in.", 2);
                                                    dialog.Option("Later...", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("Hello my friend. Have you ever heard of the lottery?\nWell, my friend, to join the lottery you need 27 ConquerPoints. By entering the lottery you'll gain a big chance to win wonderful items, wonderful and very expensive.\n\nWhat do you say, would you like to join?\n");
                                                    dialog.Option("Alright, take me in.", 1);
                                                    dialog.Option("Later...", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Level >= 70)
                                                {
                                                    if (client.LotteryEntries < 10)
                                                    {
                                                        if (client.Entity.ConquerPoints >= 27)
                                                        {
                                                            client.Entity.ConquerPoints -= 27;
                                                            client.InLottery = true;
                                                            client.LotteryEntries++;
                                                            client.Entity.Teleport(700, 42, 50);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You may not join the lottery today. You have already tried it 10 times today.");
                                                        dialog.Option("Ahh sorry.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You may not join the lottery , you need level 70 first.");
                                                    dialog.Option("Ahh sorry.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.InLottery)
                                                {
                                                    client.Entity.Teleport(700, 42, 50);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Realloter
                            case 350050:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello there brave conqueror, if you have reborn, and you misplaced your atribute points or you want to set them another way, I'll reset your atribute points for one DragonBall. Do you accept?");
                                                dialog.Option("Here is the DragonBall.", 1);
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Reborn > 0)
                                                {
                                                    if (client.Inventory.Contains(1088000, 1))
                                                    {
                                                        client.Inventory.Remove(1088000, 1);
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
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need a DragonBall.");
                                                        dialog.Option("I'll just leave", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You have to have atleast one rebirth atleast.");
                                                    dialog.Option("I'll just leave", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Magic Artisan
                            case 10062:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Have you ever seen my brother Artisan Wind? If so, you might know that he upgrades and downgrades the item level and item quality.");
                                                dialog.Text("Well, he doesn't always succeed, but that's why I am here. I can upgrade/downgrade being sucessfully with a fixed amount of meteors (tears)/dragonballs. ");
                                                dialog.Text("What would you like me to do?");
                                                dialog.Option("Upgrade level.", 1);
                                                dialog.Option("Downgrade level.", 2);
                                                dialog.Option("Upgrade quality.", 3);
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                        case 2:
                                        case 3:
                                            {
                                                int aut = npcRequest.OptionID * 10;
                                                dialog.Text("Select which item's details you want to change?");
                                                dialog.Option("Headgear.", (byte)(aut + 1));
                                                dialog.Option("Necklace.", (byte)(aut + 2));
                                                dialog.Option("Armor.", (byte)(aut + 3));
                                                dialog.Option("Main Weapon.", (byte)(aut + 4));
                                                dialog.Option("Left Weapon/Shield.", (byte)(aut + 5));
                                                dialog.Option("Ring.", (byte)(aut + 6));
                                                dialog.Option("Boots.", (byte)(aut + 8));
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 100:
                                            {
                                                if (client.SelectedItem == 0)
                                                    return;
                                                var item = client.Equipment.TryGetItem(client.SelectedItem);
                                                var itemdetail = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[item.ID];
                                                Conquer_Online_Server.Database.ConquerItemInformation infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                                                switch (client.UpdateType)
                                                {
                                                    case 1:
                                                    case 2:
                                                        {
                                                            //Cost: 36
                                                            //Metscrolls: 4
                                                            byte cost = (byte)(itemdetail.Level / 5);
                                                            byte scrolls = 0;
                                                            bool useScrolls = false;
                                                            byte extraScroll = 0;
                                                            int addMets = 0;
                                                            cost += (byte)((item.ID % 10) / 3);
                                                            cost += (byte)(itemdetail.Level / 10);
                                                            uint id = 1088001;
                                                            if (client.UpdateType == 2)
                                                                id++;
                                                            foreach (IConquerItem scroll in client.Inventory.Objects)
                                                                if (scroll.ID == 720027)
                                                                    scrolls++;
                                                            if (cost % 10 != 0)
                                                            {
                                                                addMets = 10 - (cost % 10);
                                                                extraScroll = 1;
                                                            }
                                                            if (scrolls >= (cost / 10) + extraScroll)
                                                                useScrolls = true;

                                                            if (client.Inventory.Contains(id, cost) || useScrolls)
                                                            {
                                                                if (useScrolls)
                                                                {
                                                                    client.Inventory.Remove(720027, Convert.ToByte((cost / 10) + extraScroll));
                                                                    client.Inventory.Add(1088001,0,(byte)addMets);
                                                                }
                                                                else
                                                                    client.Inventory.Remove(id, cost);
                                                                client.UnloadItemStats(item, false);
                                                                if (client.UpdateType == 1)
                                                                {
                                                                    Conquer_Online_Server.Database.ConquerItemInformation infos2 = new Conquer_Online_Server.Database.ConquerItemInformation(infos.CalculateUplevel(), item.Plus);
                                                                    if (infos2.BaseInformation.ID == infos.BaseInformation.ID)
                                                                        return;
                                                                    if (client.Entity.Level < infos2.BaseInformation.Level)
                                                                    {
                                                                        dialog.Text("You need level " + infos2.BaseInformation.Level + " first.");
                                                                        dialog.Option("Ahh sorry.", 255);
                                                                        dialog.Send();
                                                                        client.SelectedItem = 0;
                                                                        client.UpdateType = 0;
                                                                        return;
                                                                    }
                                                                    item.ID = infos.CalculateUplevel();
                                                                }
                                                                else
                                                                    item.ID = infos.CalculateDownlevel();

                                                                Conquer_Online_Server.Database.ConquerItemTable.UpdateItemID(item, client);
                                                                item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                                item.Send(client);
                                                                client.LoadItemStats(item);
                                                                client.Equipment.UpdateEntityPacket();
                                                            }
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            byte cost = (byte)(itemdetail.Level / 30);
                                                            cost += (byte)((item.ID % 10) / 2);
                                                            uint id = 1088000;

                                                            if (client.Inventory.Contains(id, cost))
                                                            {
                                                                client.Inventory.Remove(id, cost);
                                                                client.UnloadItemStats(item, false);
                                                                if (item.ID % 10 < 5)
                                                                    item.ID += 5 - item.ID % 10;
                                                                item.ID++;
                                                                Conquer_Online_Server.Database.ConquerItemTable.UpdateItemID(item, client);
                                                                item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                                item.Send(client);
                                                                client.LoadItemStats(item);
                                                                client.Equipment.UpdateEntityPacket();
                                                            }
                                                            break;
                                                        }
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                if (npcRequest.OptionID == 255)
                                                    break;
                                                client.SelectedItem = (byte)(npcRequest.OptionID % 10);
                                                client.UpdateType = (byte)(npcRequest.OptionID / 10);
                                                if (client.Equipment.Free(client.SelectedItem))
                                                {
                                                    dialog.Text("You have to be wearing the selected item.");
                                                    dialog.Option("I'll just leave", 255);
                                                    dialog.Send();
                                                    return;
                                                }
                                                var item = client.Equipment.TryGetItem(client.SelectedItem);
                                                var itemdetail = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[item.ID];
                                                if (itemdetail == null)
                                                {
                                                    dialog.Text("An error occured. Please relogin and try again.");
                                                    dialog.Option("Ok.", 255);
                                                    dialog.Send();
                                                    return;
                                                }
                                                switch (client.UpdateType)
                                                {
                                                    case 1:
                                                    case 2:
                                                        {
                                                            if (itemdetail.Level == Conquer_Online_Server.Network.PacketHandler.ItemMaxLevel(client.SelectedItem) && client.UpdateType == 1)
                                                            {
                                                                dialog.Text("This item's level cannot be upgraded anymore.");
                                                                dialog.Option("Ahh sorry.", 255);
                                                                dialog.Send();
                                                                client.SelectedItem = 0;
                                                                client.UpdateType = 0;
                                                                return;
                                                            }
                                                            if (itemdetail.Level == Conquer_Online_Server.Network.PacketHandler.ItemMinLevel(client.SelectedItem) && client.UpdateType == 2)
                                                            {
                                                                dialog.Text("This item's level cannot be downgraded anymore.");
                                                                dialog.Option("Ahh sorry.", 255);
                                                                dialog.Send();
                                                                client.SelectedItem = 0;
                                                                client.UpdateType = 0;
                                                                return;
                                                            }
                                                            byte cost = (byte)(itemdetail.Level / 5);
                                                            cost += (byte)((item.ID % 10) / 3);
                                                            cost += (byte)(itemdetail.Level / 10);
                                                            dialog.Text("It will cost you " + cost + " meteor" + (client.UpdateType == 2 ? " tears" : "s") + ". Do you accept?");
                                                            dialog.Option("Yes.", 100);
                                                            dialog.Option("No thank you.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            if (item.ID % 10 == 9)
                                                            {
                                                                dialog.Text("This item's quality cannot be upgraded anymore.");
                                                                dialog.Option("Ahh sorry.", 255);
                                                                dialog.Send();
                                                                client.SelectedItem = 0;
                                                                client.UpdateType = 0;
                                                                return;
                                                            }
                                                            byte cost = (byte)(itemdetail.Level / 30);
                                                            cost += (byte)((item.ID % 10) / 2);
                                                            dialog.Text("It will cost you " + cost + " dragonballs. Do you accept?");
                                                            dialog.Option("Yes.", 100);
                                                            dialog.Option("No thank you.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Weapon Master
                            case 7050:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("My brothers, Magic Artisan and Artisan Wind, both need meteors to upgrade, but I need only dragonballs.");
                                                dialog.Text("I upgrade item's level only and I want  only one dragonball for my service.");
                                                dialog.Option("Upgrade level.", 1);
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                int aut = npcRequest.OptionID * 10;
                                                dialog.Text("Select which item's details you want to change?");
                                                dialog.Option("Headgear.", (byte)(aut + 1));
                                                dialog.Option("Necklace.", (byte)(aut + 2));
                                                dialog.Option("Armor.", (byte)(aut + 3));
                                                dialog.Option("Main Weapon.", (byte)(aut + 4));
                                                dialog.Option("Left Weapon/Shield.", (byte)(aut + 5));
                                                dialog.Option("Ring.", (byte)(aut + 6));
                                                dialog.Option("Boots.", (byte)(aut + 8));
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 100:
                                            {
                                                if (client.SelectedItem == 0)
                                                    return;
                                                var item = client.Equipment.TryGetItem(client.SelectedItem);
                                                var itemdetail = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[item.ID];
                                                Conquer_Online_Server.Database.ConquerItemInformation infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                                                switch (client.UpdateType)
                                                {
                                                    case 1:
                                                    case 2:
                                                        {

                                                            byte cost = (byte)(itemdetail.Level / 6);
                                                            cost += (byte)((item.ID % 10) / 3);
                                                            uint id = 1088001;
                                                            if (client.UpdateType == 2)
                                                                id++;
                                                            if (client.Inventory.Contains(id, cost))
                                                            {
                                                                client.Inventory.Remove(id, cost);
                                                                client.UnloadItemStats(item, false);
                                                                if (client.UpdateType == 1)
                                                                    item.ID = infos.CalculateUplevel();
                                                                else
                                                                    item.ID = infos.CalculateDownlevel();
                                                                if (infos.BaseInformation.ID == item.ID)
                                                                    return;
                                                                Conquer_Online_Server.Database.ConquerItemTable.UpdateItemID(item, client);
                                                                item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                                item.Send(client);
                                                                client.LoadItemStats(item);
                                                                client.Equipment.UpdateEntityPacket();
                                                            }
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            byte cost = (byte)(itemdetail.Level / 30);
                                                            cost += (byte)((item.ID % 10) / 2);
                                                            uint id = 1088000;

                                                            if (client.Inventory.Contains(id, cost))
                                                            {
                                                                client.Inventory.Remove(id, cost);
                                                                client.UnloadItemStats(item, false);
                                                                item.ID++;
                                                                Conquer_Online_Server.Database.ConquerItemTable.UpdateItemID(item, client);
                                                                item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                                item.Send(client);
                                                                client.LoadItemStats(item);
                                                                client.Equipment.UpdateEntityPacket();
                                                            }
                                                            break;
                                                        }
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                if (npcRequest.OptionID == 255)
                                                    break;
                                                byte SelectedItem = (byte)(npcRequest.OptionID % 10);
                                                byte NowType = (byte)(npcRequest.OptionID / 10);
                                                if (client.Equipment.Free(SelectedItem))
                                                {
                                                    dialog.Text("You have to be wearing the selected item.");
                                                    dialog.Option("I'll just leave", 255);
                                                    dialog.Send();
                                                    return;
                                                }
                                                var item = client.Equipment.TryGetItem(SelectedItem);
                                                var itemdetail = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[item.ID];

                                                if (itemdetail.Level == Conquer_Online_Server.Network.PacketHandler.ItemMaxLevel(client.SelectedItem) && client.UpdateType == 1)
                                                {
                                                    dialog.Text("This item's level cannot be upgraded anymore.");
                                                    dialog.Option("Ahh sorry.", 255);
                                                    dialog.Send();
                                                    client.SelectedItem = 0;
                                                    client.UpdateType = 0;
                                                    return;
                                                }
                                                if (NowType == 1)
                                                {
                                                    dialog.Text("It will cost you one dragonball. Do you accept?");
                                                    dialog.Option("Yes.", (byte)(20 + SelectedItem));
                                                    dialog.Option("No thank you.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    Conquer_Online_Server.Database.ConquerItemInformation infos = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                                                    Conquer_Online_Server.Database.ConquerItemInformation infos2 = new Conquer_Online_Server.Database.ConquerItemInformation(infos.CalculateUplevel(), item.Plus);
                                                    if (client.Entity.Level < infos2.BaseInformation.Level)
                                                    {
                                                        dialog.Text("You need level " + infos2.BaseInformation.Level + " first.");
                                                        dialog.Option("Ahh sorry.", 255);
                                                        dialog.Send();
                                                        client.SelectedItem = 0;
                                                        client.UpdateType = 0;
                                                        return;
                                                    }
                                                    if (client.Inventory.Contains(1088000, 1))
                                                    {
                                                        client.Inventory.Remove(1088000, 1);
                                                        client.UnloadItemStats(item, false);
                                                        item.ID = infos.CalculateUplevel();
                                                        Conquer_Online_Server.Database.ConquerItemTable.UpdateItemID(item, client);
                                                        item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                        item.Send(client);
                                                        client.LoadItemStats(item);
                                                        client.Equipment.UpdateEntityPacket();
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                }
                            #endregion

                            #region Proficiency God
                            case 941:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello my friend. If you believe that, leveling your proficiency is too hard and takes too much time, I can help you. In exchange of a fixed amount of exp balls, I will agree to level up your proficiency.");
                                                dialog.Text("Now, tell me what proficiency you want to level up.");
                                                dialog.Option("One handed.", 1);
                                                dialog.Option("Two handed.", 3);
                                                dialog.Option("Other.", 5);
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("Which one handed proficiency?");
                                                dialog.Option("Blade.", 41);
                                                dialog.Option("Sword.", 42);
                                                dialog.Option("Hook.", 43);
                                                dialog.Option("Whip.", 44);
                                                dialog.Option("Axe.", 45);
                                                dialog.Option("Next page.", 2);
                                                dialog.Option("Nothing, sorry.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                dialog.Text("Which one handed proficiency?");
                                                dialog.Option("PrayerBead.", 61);
                                                dialog.Option("Hammer.", 46);
                                                dialog.Option("Club.", 48);
                                                dialog.Option("Scepter.", 184);
                                                dialog.Option("Katana.", 60);
                                                dialog.Option("Axe.", 45);
                                                dialog.Option("Back.", 1);
                                                dialog.Option("Nothing, sorry.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                dialog.Text("Which two handed proficiency?");
                                                dialog.Option("Backsword.", 124);
                                                dialog.Option("Glaive.", 51);
                                                dialog.Option("Poleaxe.", 53);
                                                dialog.Option("LongHammer.", 54);
                                                dialog.Option("Spear.", 56);
                                                dialog.Option("Next page.", 4);
                                                dialog.Option("Nothing, sorry.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 4:
                                            {
                                                dialog.Text("Which one handed proficiency?");
                                                dialog.Option("Pickaxe.", 142);
                                                dialog.Option("Halberd.", 58);
                                                dialog.Option("Wand.", 165);
                                                dialog.Option("Bow.", 50);
                                                dialog.Option("Back.", 3);
                                                dialog.Option("Nothing, sorry.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 5:
                                            {
                                                dialog.Text("Which one handed proficiency?");
                                                dialog.Option("Boxing.", 254);
                                                dialog.Option("Shield.", 90);
                                                dialog.Option("Nothing, sorry.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 100:
                                            {
                                                if (client.Proficiencies.ContainsKey(client.UplevelProficiency))
                                                {
                                                    var prof = client.Proficiencies[client.UplevelProficiency];
                                                    if (prof.Level >= 12)
                                                    {
                                                        dialog.Text("This proficiency cannot be leveled up anymore.");
                                                        dialog.Option("Oh.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    client.UplevelProficiency = 0;
                                                    if (client.Inventory.Contains(723700, prof.Level))
                                                    {
                                                        client.Inventory.Remove(723700, prof.Level);
                                                        prof.Level++;
                                                        prof.Experience = 0;
                                                        prof.Send(client);
                                                        break;
                                                    }
                                                    dialog.Text("You don't have the requiered exp balls, I'm sorry I cannot help you.");
                                                    dialog.Option("It's alright.", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't know this proficiency.");
                                                    dialog.Option("Ahh, sorry.", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                            }
                                        default:
                                            {
                                                if (npcRequest.OptionID == 255)
                                                    return;
                                                ushort proficiency = 0;
                                                if (npcRequest.OptionID < 100)
                                                    proficiency = (ushort)(npcRequest.OptionID * 10);
                                                else
                                                {
                                                    if (npcRequest.OptionID != 254)
                                                    {
                                                        string off = npcRequest.OptionID.ToString();
                                                        string reverse = off[2].ToString() + off[1].ToString() + off[0].ToString();
                                                        proficiency = ushort.Parse(reverse);
                                                    }
                                                }
                                                if (proficiency == 600) proficiency++;
                                                if (client.Proficiencies.ContainsKey(proficiency))
                                                {
                                                    var prof = client.Proficiencies[proficiency];
                                                    if (prof.Level >= 12)
                                                    {
                                                        dialog.Text("This proficiency cannot be leveled up anymore.");
                                                        dialog.Option("Oh.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    client.UplevelProficiency = proficiency;
                                                    dialog.Text("I need " + prof.Level + " exp balls to be able to level up this proficiency.");
                                                    dialog.Option("Let's do it then.", 100);
                                                    dialog.Option("No, sorry.", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't know this proficiency.");
                                                    dialog.Option("Ahh, sorry.", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region CPAdmin
                            case 111816:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Greetings, here you can exchange either a Dragonball or a DragonScroll for cps. ");
                                                dialog.Text("One Dragonball will get you 215 cps and a DragonScroll will get you 2150. ");
                                                dialog.Text("What would you like to trade? Or would you rather not?");
                                                dialog.Option("Dragonball", 1);
                                                dialog.Option("DragonballScroll", 2);
                                                dialog.Option("I'll just leave", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Inventory.Contains(1088000, 1))
                                                {
                                                    client.Inventory.Remove(1088000, 1);
                                                    client.Entity.ConquerPoints += 215;
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have a Dragonball");
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Inventory.Contains(721511, 1))
                                                {
                                                    client.Inventory.Remove(721511, 1);
                                                    client.Entity.ConquerPoints += 2150;
                                                }
                                                else
                                                {
                                                    dialog.Text("You do not have a DragonballScroll");
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region SurgeonMiracle
                            case 3381:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Do you want to have your size changed so that you can become much more");
                                                dialog.Text(" adorable? Now here is a precious chance for you.");
                                                dialog.Option("I want to change my size.", 1);
                                                //dialog.Option("I want to change my gender.", 2);
                                                dialog.Option("I don't want to change.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("If you pay me one DragonBall, I can have your size changed. You will become");
                                                dialog.Text("more attractive and start a fresh life. By the way, to avoid some unexpected");
                                                dialog.Text("things, make sure you are not in any disguise form.");
                                                dialog.Option("Here is a DragonBall.", 3);
                                                dialog.Option("I have no DragonBall.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                dialog.Text("If you pay me one DragonBall, I can have your gender changed. You will become");
                                                dialog.Text("more attractive and start a fresh life. By the way, to avoid some unexpected");
                                                dialog.Text("things, make sure you are not in any disguise form.");
                                                dialog.Option("Here is a DragonBall.", 4);
                                                dialog.Option("I have no DragonBall.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Inventory.Contains(1088000, 1))
                                                {
                                                    int Model = (int)client.Entity.Body % 10;
                                                    switch (Model)
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

                                                    client.Inventory.Remove(1088000, 1);
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("Sorry, there is no DragonBall in your inventory. I can't have your size changed.");
                                                    dialog.Option("Ok, Sorry.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 4:
                                            {
                                                if (client.Inventory.Contains(1088000, 1))
                                                {
                                                    int Model = (int)client.Entity.Body % 10;
                                                    switch (Model)
                                                    {
                                                        case 3:
                                                        case 4:
                                                            client.Entity.Body -= 2;
                                                            break;
                                                        case 1:
                                                        case 2:
                                                            client.Entity.Body += 2;
                                                            break;
                                                    }
                                                    client.Send(new Message("Your gender has been changed.", System.Drawing.Color.BurlyWood, Message.TopLeft));
                                                    client.Inventory.Remove(1088000, 1);
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("Sorry, there is no DragonBall in your inventory. I can't have your size changed.");
                                                    dialog.Option("Ok, Sorry.", 255);
                                                    dialog.Send();
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                }
                            #endregion

                            #region Mark. Controler
                            case 45: //Mark. Controler
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("If you want to leave just tell me when you are ready. It's free.\nYou will be teleported to the city you were in before coming here.");
                                                dialog.Option("I'm ready.", 1);
                                                dialog.Option("Wait a minute.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                int PrevMap = client.Entity.PreviousMapID;
                                                switch (PrevMap)
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
                                                    case 1015:
                                                        {
                                                            client.Entity.Teleport(1015, 717, 571);
                                                            break;
                                                        }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Enchanter and composer
                            case 35016:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("I can improve the enchant of an item and I can also improve the plus of an item. Please let me know how can I help you.");
                                                dialog.Option("Compose.", 1);
                                                dialog.Option("Enchant.", 2);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                Data data = new Data(true);
                                                data.ID = Data.OpenWindow;
                                                data.UID = client.Entity.UID;
                                                data.TimeStamp = Time32.Now;
                                                data.dwParam = Data.WindowCommands.Compose;
                                                data.wParam1 = client.Entity.X;
                                                data.wParam2 = client.Entity.Y;
                                                client.Send(data);
                                                break;
                                            }
                                        case 2:
                                            {
                                                Data data = new Data(true);
                                                data.ID = Data.OpenCustom;
                                                data.UID = client.Entity.UID;
                                                data.TimeStamp = Time32.Now;
                                                data.dwParam = Data.CustomCommands.Enchant;
                                                data.wParam1 = client.Entity.X;
                                                data.wParam2 = client.Entity.Y;
                                                client.Send(data);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Bless
                            case 35015:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello there mate. I am here to add to your item a very precious element that will make your item more powerful and also more expensive. Each bless point ( - ) gives you 1% less damage. For -1 you need five Super tortoise gems(STG) or you can gain in through first reborn. For -3 you need one STGs, for -5 you need three STGs and for -7 you five STGs. Do you want to go on?");
                                                dialog.Option("Okay let's roll.", 100);
                                                dialog.Option("It's too expensive.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 100:
                                            {
                                                dialog.Text("Select the item. You must wear it.");
                                                dialog.Option("Headgear.", 1);
                                                dialog.Option("Necklace.", 2);
                                                dialog.Option("Armor.", 3);
                                                dialog.Option("Weapon", 4);
                                                dialog.Option("Shield", 5);
                                                dialog.Option("Ring", 6);
                                                dialog.Option("Boots.", 8);
                                                dialog.Option("Nevermind.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                        case 6:
                                        case 8:
                                            {
                                                byte stgs = 5;
                                                Conquer_Online_Server.Interfaces.IConquerItem Item = null;
                                                Item = client.Equipment.TryGetItem((byte)npcRequest.OptionID);
                                                if (Item != null)
                                                {
                                                    if (Item.Bless == 1)
                                                        stgs = 1;
                                                    else if (Item.Bless == 3)
                                                        stgs = 3;
                                                    else if (Item.Bless == 5)
                                                        stgs = 5;
                                                    if (Item.Bless == 7)
                                                    {
                                                        dialog.Text("An item can't have more than -7 bless points.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    if (client.Inventory.Contains(700073, stgs))
                                                    {
                                                        client.Inventory.Remove(700073, stgs);
                                                        if (Item.Bless == 0)
                                                            Item.Bless = 1;
                                                        else
                                                            Item.Bless += 2;
                                                        Item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                        Item.Send(client);
                                                        Conquer_Online_Server.Database.ConquerItemTable.UpdateBless(Item, client);
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have " + stgs + " STGs.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region BlacksmithLee
                            case 1550:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            client.Send(new Data(true) { UID = client.Entity.UID, ID = Data.OpenWindow, dwParam = Data.WindowCommands.OpenSockets, wParam1 = client.Entity.X, wParam2 = client.Entity.Y });
                                            break;
                                    }
                                    break;
                                }
                            #endregion


                            #region UnknownMan
                            case 3825:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello stranger. My mother tough me not to talk with strangers but I feel like I can talk with you. There is something special to you. Since I feel like I can trust you I can make you a service. If your level is below 110, I will give you a lot of exp in exchange of one DragonBall, but if your level is above or equal to 110 I will demand two DragonBalls. If your level is higher than 135, well then, I can't help you. If you are interested in my proposition and if you have DragonBalls, please let me know.");
                                                dialog.Option("Yes I do.", 1);
                                                dialog.Option("No, not really.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Level >= 135)
                                                {
                                                    dialog.Text("I am sorry, your level is too high. I simply can't help you.");
                                                    dialog.Option("Sorry.", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                if (DateTime.Now.DayOfYear != client.LastDragonBallUse.DayOfYear)
                                                {
                                                    byte needTimes = 1;
                                                    ulong exp_reward = 13901336;//default for level 50 or lower
                                                    #region exp reward switch
                                                    byte level = client.Entity.Level;
                                                    if (level > 50 && level < 63)
                                                        exp_reward = 27152909;
                                                    if (level >= 63 && level < 69)
                                                        exp_reward = 28860143;
                                                    if (level >= 69 && level < 74)
                                                        exp_reward = 36822370;
                                                    if (level >= 74 && level < 79)
                                                        exp_reward = 57533091;
                                                    if (level >= 79 && level < 87)
                                                        exp_reward = 70404048;
                                                    if (level >= 87 && level < 90)
                                                        exp_reward = 84097242;
                                                    if (level >= 90 && level < 93)
                                                        exp_reward = 102959118;
                                                    if (level >= 93 && level < 96)
                                                        exp_reward = 134266326;
                                                    if (level >= 96 && level < 98)
                                                        exp_reward = 100801220;
                                                    if (level >= 98 && level <= 100)
                                                        exp_reward = 214351925;

                                                    if (level > 100)
                                                    {
                                                        switch (level)
                                                        {
                                                            case 101: exp_reward = 242910783; break;
                                                            case 102: exp_reward = 286050512; break;
                                                            case 103: exp_reward = 259627544; break;
                                                            case 104: exp_reward = 232767237; break;
                                                            case 105: exp_reward = 241888762; break;
                                                            case 106: exp_reward = 249478280; break;
                                                            case 107: exp_reward = 265126887; break;
                                                            case 108: exp_reward = 187446887; break;
                                                            case 109: exp_reward = 193715970; break;
                                                            case 110: exp_reward = 204416075; break;
                                                            case 111: exp_reward = 227337342; break;
                                                            case 112: exp_reward = 230562942; break;
                                                            case 113: exp_reward = 234594942; break;
                                                            case 114: exp_reward = 238626942; break;
                                                            case 115: exp_reward = 240239742; break;
                                                            case 116: exp_reward = 242658942; break;
                                                            case 117: exp_reward = 246690942; break;
                                                            case 118: exp_reward = 290290023; break;
                                                            case 119: exp_reward = 358712493; break;
                                                            case 120: exp_reward = 282274058; break;
                                                            case 121: exp_reward = 338728870; break;
                                                            case 122: exp_reward = 243884786; break;
                                                            case 123: exp_reward = 292661743; break;
                                                            case 124: exp_reward = 322122547; break;
                                                            case 125: exp_reward = 292661744; break;
                                                            case 126: exp_reward = 351194092; break;
                                                            case 127: exp_reward = 337146328; break;
                                                            case 128: exp_reward = 303431696; break;
                                                            case 129: exp_reward = 322122547; break;
                                                        }
                                                    }
                                                    #endregion
                                                    if (client.Entity.Level >= 110) { needTimes = 2; }
                                                    if (client.Entity.Level >= 130) { needTimes += 8; }
                                                    if (client.Entity.Level >= 135)
                                                    {
                                                        dialog.Text("I can't help you anymore, you're too high leveled!");
                                                        dialog.Option("I see, Thanks!", 255);
                                                        break;
                                                    }
                                                    if (client.Inventory.Contains(1088000, needTimes))
                                                    {
                                                        client.Inventory.Remove(1088000, needTimes);
                                                        client.IncreaseExperience(exp_reward, false);
                                                        client.LastDragonBallUse = DateTime.Now;
                                                        dialog.Text("Congratulations! You've received a lot of experience from me today, enjoy!");
                                                        dialog.Text("I'll see you only tomorrow now.");
                                                        dialog.Option("I see, Thanks!", 255);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("I am sorry, but obviously you want to scam me out because you don't have the amount of required DragonBalls.");
                                                        dialog.Option("Sorry.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I am sorry, but I remeber that I gave you this opportunity already today. I can't do it more than once per day so I'm afraid I have to tell you good bye until tomorrow.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region LoveStone
                            case 390:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello mate, how do you do? Listen, There are not many people that can make a marriage last, but I can sense that you are one of them, if you want to marry someone just let me know. Also, if you are heavenly blessed I can give you one hour of double exp each day.");
                                                dialog.Option("Yes, I want to marry someone.", 1);
                                                dialog.Option("I need double exp.", 2);
                                                dialog.Option("Nothing thank you.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Spouse == "None")
                                                {
                                                    dialog.Text("Here, click on the player you want to be your spouse.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                    Data data = new Data(true);
                                                    data.UID = client.Entity.UID;
                                                    data.ID = Data.OpenCustom;
                                                    data.dwParam = Data.CustomCommands.FlowerPointer;
                                                    client.Send(data);
                                                }
                                                else
                                                {
                                                    dialog.Text("You are already married. If you want to broke the marriage, you have to go to StarLit. He'll handle your request.");
                                                    dialog.Option("Alright that's what I'll do.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.HeavenBlessing > 0)
                                                {
                                                    if (!client.DoubleExpToday)
                                                    {
                                                        dialog.Text("Here. Come back tomorrow for more.");
                                                        dialog.Option("Alright that's what I'll do.", 255);
                                                        dialog.Send();

                                                        client.Entity.DoubleExperienceTime = 3600;
                                                        client.DoubleExpToday = true;
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You already took your double exp today. Come back tomorrow.");
                                                        dialog.Option("Alright that's what I'll do.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot take double exp because you are not heavenly blessed.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Eternity
                            case 300500:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hello conqueror. You know, there are ancient stories about a second life and many people chose not to believe it. Even so, there are some people that believe in it, and that inspired me to learn about it. In my study, I found out how to reborn someone. But, in this quest, a CelestialStone may be needed. To get one, talk with Celestine(TwinCity 365, 92).");
                                                dialog.Option("Tell me more about it.", 1);
                                                dialog.Option("Reborm me.", 2);
                                                dialog.Option("Nothing thank you.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                dialog.Text("So you want to know more? When you reborn, you can chose what class you want to be the second life. Once you reborn you will start again the journey to conquer this world from level 15. Beware that you may gain some special skills depending on the class you chose to reborn, and you'll be able to get an own pet, that will follow and help you level or fight. The cost is a CelestialStone.");
                                                dialog.Option("Reborm me.", 2);
                                                dialog.Option("Nothing thank you.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Reborn == 0)
                                                {
                                                    if (client.Entity.Class % 10 == 5 && client.Entity.Level >= (client.Entity.Class == 135 ? 110 : 120))
                                                    {
                                                        dialog.Text("There are two kinds of reborns. One is the normal one and the second one is blessed. The normal reborn will give you the chance to get a Super Gem and the blessed reborn will set a -1 into one of your equipment that you wear during the reborn. What do you chose?");
                                                        dialog.Option("Normal reborn.", 5);
                                                        dialog.Option("Blessed reborn.", 3);
                                                        dialog.Option("Nothing thank you.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You cannot reborn if your level is not 110+ for water saints and 120+ for other masters.");
                                                        dialog.Option("Ahh.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot reborn again here. Alex, an elder who lives in Ape Canyon, will tell you about the third life.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Reborn == 0)
                                                {
                                                    if (client.Entity.Class % 10 == 5 && client.Entity.Level >= (client.Entity.Class == 135 ? 110 : 120))
                                                    {
                                                        dialog.Text("Select the super gem you desire.");
                                                        dialog.Option("SuperPhoenixGem.", 203);
                                                        dialog.Option("SuperDragonGem.", 213);
                                                        dialog.Option("SuperFuryGem.", 223);
                                                        dialog.Option("SuperRainbowGem.", 233);
                                                        dialog.Option("SuperVioletGem.", 253);
                                                        dialog.Option("SuperMoonGem.", 254);
                                                        dialog.Option("SuperKylinGem.", 243);
                                                        dialog.Option("Nothing thank you.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You cannot reborn if your level is not 110+ for water saints and 120+ for other masters.");
                                                        dialog.Option("Ahh.", 255);
                                                        dialog.Send();
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot reborn again here. Alex, an elder who lives in Ape Canyon, will tell you about the third life.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Reborn == 0)
                                                {
                                                    if (client.Entity.Class % 10 == 5 && client.Entity.Level >= (client.Entity.Class == 135 ? 110 : 120))
                                                    {
                                                        dialog.Text("Select the class you want to reborn in.");
                                                        dialog.Option("Trojan.", (byte)(10 + npcRequest.OptionID));
                                                        dialog.Option("Warrior.", (byte)(20 + npcRequest.OptionID));
                                                        dialog.Option("Archer.", (byte)(40 + npcRequest.OptionID));
                                                        dialog.Option("WaterTaoist.", (byte)(132 + npcRequest.OptionID));
                                                        dialog.Option("FireTaoist.", (byte)(142 + npcRequest.OptionID));
                                                        dialog.Option("Ninja.", (byte)(50 + npcRequest.OptionID));
                                                        dialog.Option("Monk.", (byte)(60 + npcRequest.OptionID));
                                                        dialog.Option("Nothing thank you.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You cannot reborn if your level is not 110+ for water saints and 120+ for other masters.");
                                                        dialog.Option("Ahh.", 255);
                                                        dialog.Send();
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot reborn again here. Alex, an elder who lives in Ape Canyon, will tell you about the third life.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                if (client.Entity.Reborn == 0)
                                                {
                                                    if (client.Entity.Class % 10 == 5 && client.Entity.Level >= (client.Entity.Class == 135 ? 110 : 120))
                                                    {
                                                        if (npcRequest.OptionID == 255)
                                                            return;
                                                        if (npcRequest.OptionID >= 200 && npcRequest.OptionID <= 254)
                                                        {
                                                            client.SelectedGem = (byte)(npcRequest.OptionID % 100);
                                                            if (client.SelectedGem == 54)
                                                                client.SelectedGem = 63;
                                                            if (client.Entity.Reborn == 0)
                                                            {
                                                                byte id = 4;
                                                                dialog.Text("Select the class you want to reborn in.");
                                                                dialog.Option("Trojan.", (byte)(10 + id));
                                                                dialog.Option("Warrior.", (byte)(20 + id));
                                                                dialog.Option("Archer.", (byte)(40 + id));
                                                                dialog.Option("WaterTaoist.", (byte)(132 + id));
                                                                dialog.Option("FireTaoist.", (byte)(142 + id));
                                                                dialog.Option("Ninja.", (byte)(50 + id));
                                                                dialog.Option("Monk.", (byte)(60 + id));
                                                                dialog.Option("Nothing thank you.", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                dialog.Text("You cannot reborn again here. Alex, an elder who lives in Ape Canyon, will tell you about the third life.");
                                                                dialog.Option("Thank you.", 255);
                                                                dialog.Send();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (client.Inventory.Contains(721259, 1))
                                                            {
                                                                byte _class = (byte)(npcRequest.OptionID - npcRequest.OptionID % 10);
                                                                if (_class > 100)
                                                                    _class += 2;
                                                                byte type = (byte)(npcRequest.OptionID - _class);
                                                                if (_class < 100)
                                                                    _class++;
                                                                if (client.Reborn(_class))
                                                                {
                                                                    client.Inventory.Remove(721259, 1);
                                                                    if (type == 4)
                                                                    {
                                                                        if (client.SelectedGem != 0)
                                                                        {
                                                                            uint gemid = (uint)(client.SelectedGem + 700000);
                                                                            client.Inventory.Add(gemid, 0, 1);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        int availableshots = 0;
                                                                        for (byte count = 0; count < 12; count++)
                                                                            if (!client.Equipment.Free(count))
                                                                                if (client.Equipment.TryGetItem(count).Bless == 0)
                                                                                    availableshots++;
                                                                        if (availableshots != 0)
                                                                        {
                                                                            byte ex = (byte)Conquer_Online_Server.ServerBase.Kernel.Random.Next(12);
                                                                            if (!client.Equipment.Free(ex))
                                                                                if (client.Equipment.TryGetItem(ex).Bless == 0)
                                                                                {
                                                                                    var item = client.Equipment.TryGetItem(ex);
                                                                                    item.Bless = 1;
                                                                                    item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                                                    item.Send(client);
                                                                                    Conquer_Online_Server.Database.ConquerItemTable.UpdateBless(item, client);
                                                                                }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dialog.Text("Sorry, but you need atleast 2 free spaces in your inventory.");
                                                                    dialog.Option("Ohh.", 255);
                                                                    dialog.Send();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You cannot reborn if your level is not 110+ for water saints and 120+ for other masters.");
                                                        dialog.Option("Ahh.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot reborn again here. Alex, an elder who lives in Ape Canyon, will tell you about the third life.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region MillionaireLee
                            case 5004:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Hey you! Yeah, it's you I am talking about. I've got a great deal that you cannot leave without taking it. I will give you one meteorscroll for ten meteors and one dragonballscroll for ten dragonballs. With those scrolls you can have more than fourty meteors or dragonballs in your inventory at once!");
                                                dialog.Option("Take my meteors.", 1);
                                                dialog.Option("Take my dragonballs.", 2);
                                                dialog.Option("I'm too poor.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Inventory.Contains(1088001, 10))
                                                {
                                                    client.Inventory.Remove(1088001, 10);
                                                    client.Inventory.Add(720027, 0, 1);
                                                }
                                                else
                                                {
                                                    dialog.Text("You poor man! I can't help you.");
                                                    dialog.Option("Why!?", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Inventory.Contains(1088000, 10))
                                                {
                                                    client.Inventory.Remove(1088000, 10);
                                                    client.Inventory.Add(720028, 0, 1);
                                                }
                                                else
                                                {
                                                    dialog.Text("You poor man! I can't help you.");
                                                    dialog.Option("Why!?", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Granny
                            case 699:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("These youngsters forget to increase the durabuility when they upgrade your items. ");
                                                dialog.Text("I can take your SuperKylinGems and try to increase your item's max durabuility. ");
                                                dialog.Text("It's possible that the item could be completely repaired too. How many will you try with?");
                                                dialog.Input("Enter:", 1, 2);
                                                dialog.Option("Not now.", 255);

                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                byte Kylins = 0;
                                                if (!Byte.TryParse(npcRequest.Input, out Kylins))
                                                {// Input doesn't contain numbers only.
                                                    dialog.Text("Tell me, how am I supposed to get your durability higher if you can't give me a number 1 through 40?");
                                                    dialog.Option("Oops!", 0);
                                                    dialog.Send();
                                                    break;
                                                }
                                                if (Kylins != 0 && Kylins <= 40)
                                                {
                                                    if (client.Inventory.Contains(700043, Kylins))
                                                    {
                                                        client.KylinUpgradeCount = Kylins;
                                                        dialog.Text("Which item do you want to upgrade?");
                                                        dialog.Option("Headgear.", 101);
                                                        dialog.Option("Necklace.", 102);
                                                        dialog.Option("Armor.", 103);
                                                        dialog.Option("Main Weapon.", 104);
                                                        dialog.Option("Left Weapon.", 105);
                                                        dialog.Option("Ring.", 106);
                                                        dialog.Option("Next Page.", 2);
                                                        dialog.Option("I'll just leave", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have that many SuperKylinGems...");
                                                        dialog.Option("Oops...", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Inventory.Contains(700043, client.KylinUpgradeCount))
                                                {
                                                    dialog.Text("Which item do you want to upgrade?");
                                                    dialog.Option("Boots.", 108);
                                                    dialog.Option("Fan.", 110);
                                                    dialog.Option("Tower.", 111);
                                                    dialog.Option("I'll just leave", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 101:
                                        case 102:
                                        case 103:
                                        case 104:
                                        case 105:
                                        case 106:
                                        case 108:
                                        case 110:
                                        case 111:
                                            {
                                                if (client.KylinUpgradeCount != 0 && client.KylinUpgradeCount <= 40)
                                                {
                                                    if (client.Inventory.Contains(700043, client.KylinUpgradeCount))
                                                    {
                                                        var item = client.Equipment.TryGetItem((byte)(npcRequest.OptionID - 100));
                                                        if (item != null)
                                                        {
                                                            #region old
                                                            /*if (item.Durability == item.MaximDurability)
                                                {
                                                    var itemdetail = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[item.ID];
                                                    if (itemdetail.Durability != item.MaximDurability)
                                                    {
                                                        
                                                        int diff = itemdetail.Durability - item.MaximDurability;

                                                        int need = Math.Max((diff / 100) / 6, 1);

                                                        need = Math.Min(need, client.KylinUpgradeCount);

                                                        for (int counter = 0; counter < need; counter++)
                                                        {
                                                            item.MaximDurability += 100;
                                                            client.Inventory.Remove(700043, 1);
                                                        }
                                                        if (item.MaximDurability > itemdetail.Durability)
                                                            item.MaximDurability = itemdetail.Durability;
                                                        item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                        item.Send(client);
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("Your item must be repaired first.");
                                                    dialog.Option("Oh, sorry.", 255);
                                                    dialog.Send();
                                                    break;
                                                }*/
                                                            #endregion
                                                            #region test
                                                            var itemdetail = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[item.ID];
                                                            if (itemdetail.Durability != item.MaximDurability)
                                                            {
                                                                ushort oldDurability = item.MaximDurability;
                                                                bool Upgraded = false;
                                                                while (client.KylinUpgradeCount != 0)
                                                                {
                                                                    client.Inventory.Remove(700043, 1);
                                                                    client.KylinUpgradeCount -= 1;

                                                                    int rndDurability = (Conquer_Online_Server.ServerBase.Kernel.Random.Next(itemdetail.Durability));
                                                                    if (rndDurability >= item.MaximDurability)
                                                                    {
                                                                        //This looks ugly to me, not sure how else to do it.
                                                                        item.MaximDurability = ushort.Parse(rndDurability.ToString());
                                                                        rndDurability = 0;
                                                                        if (!Upgraded)
                                                                            Upgraded = true;
                                                                    }
                                                                }
                                                                if (Upgraded)
                                                                    dialog.Text("Your item's durabuility was increased from " + oldDurability + " to " + item.MaximDurability + ".\n");
                                                                else
                                                                    dialog.Text("I wasen't able to increase your item's durability..\n");
                                                                int Repair = (Conquer_Online_Server.ServerBase.Kernel.Random.Next(3));
                                                                if (Repair == 1)
                                                                {
                                                                    if (Upgraded)
                                                                        dialog.Text("I was also able to repair the item.");
                                                                    else
                                                                    {
                                                                        dialog.Text("But I was able to repair the item.");
                                                                        Upgraded = true;
                                                                    }
                                                                    item.Durability = item.MaximDurability;
                                                                }
                                                                item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                                                item.Send(client);
                                                                if (Upgraded)
                                                                    dialog.Option("Thanks.", 255);
                                                                else
                                                                    dialog.Option("Darn..", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                dialog.Text("This item is already at it's max durabuility!");
                                                                dialog.Option("Oh.", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("Wahh, What is this? I may be forgetful but I'm sure you told me you would give me " + client.KylinUpgradeCount.ToString() + " SuperKylinGems!\n\n I don't see that you have this many..");
                                                        dialog.Option("Lets try again.", 0);
                                                        dialog.Option("Nevermind.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }

                            #endregion

                            #region Confiscator
                            case 4450:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Have you ever arrested anyone? Or ... don't tell me, you have been arrested by anyone? If so, there is a chance that some gear was detained.");
                                                dialog.Option("I want to check my detained gear.", 1);
                                                dialog.Option("I want to check my claimable gear.", 2);
                                                dialog.Option("Thank you.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Send(new Data(true) { ID = Data.OpenWindow, dwParam = Data.WindowCommands.DetainRedeem, UID = client.Entity.UID });
                                                break;
                                            }
                                        case 2:
                                            {
                                                client.Send(new Data(true) { ID = Data.OpenWindow, dwParam = Data.WindowCommands.DetainClaim, UID = client.Entity.UID });
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region PurificationStabilizer
                            case 27279:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Have you ever heard of the ruthless Terato Dragon? It cuts open every single conqueror that wants to take his prize. His prize are items that purify your items. His affiliates, sub-kings of monsters, also drop items like that. If you have such an item, I can purify your gear with it.");
                                                dialog.Text("Once purified, the item must be stabilized, otherwise the purity will dissapear after several days. If you want to stabilize your items, come to me I'll see what I can do.");
                                                dialog.Option("Purify my item.", 1);
                                                dialog.Option("Stabilize my item.", 2);
                                                dialog.Option("Nevermind.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                INpc npc = null;
                                                if (client.Map.Npcs.TryGetValue(client.ActiveNpc, out npc))
                                                {
                                                    Data data = new Data(true);
                                                    data.ID = Data.OpenWindow;
                                                    data.UID = client.Entity.UID;
                                                    data.TimeStamp = Time32.Now;
                                                    data.dwParam = 455;
                                                    data.wParam1 = npc.X;
                                                    data.wParam2 = npc.Y;
                                                    client.Send(data);
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                INpc npc = null;
                                                if (client.Map.Npcs.TryGetValue(client.ActiveNpc, out npc))
                                                {
                                                    Data data = new Data(true);
                                                    data.ID = Data.OpenWindow;
                                                    data.UID = client.Entity.UID;
                                                    data.TimeStamp = Time32.Now;//done
                                                    data.dwParam = 459;
                                                    data.wParam1 = npc.X;
                                                    data.wParam2 = npc.Y;
                                                    client.Send(data);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Guild war
                case 1038:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Pole
                            case 810:
                                {
                                    dialog.Text("Please, don't hurt me!.");
                                    dialog.Option("Sorry.", 255);
                                    dialog.Send();
                                    break;
                                }
                            #endregion

                            #region Gates
                            case 516074:
                                {
                                    if (client.Guild != null)
                                    {
                                        if (client.Guild.PoleKeeper)
                                        {
                                            switch (npcRequest.OptionID)
                                            {
                                                case 0:
                                                    dialog.Text("Select the option you want to pursue.");
                                                    if (client.AsMember.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                    {
                                                        dialog.Option("Open gate.", 1);
                                                        dialog.Option("Close gate.", 2);
                                                    }
                                                    dialog.Option("Get inside.", 3);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                    break;
                                                case 1:
                                                    {
                                                        Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.Mesh = (ushort)(250 + Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.Mesh % 10);

                                                        Update upd = new Update(true);
                                                        upd.UID = Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.UID;
                                                        upd.Append(Update.Mesh, Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.Mesh);
                                                        client.SendScreen(upd, true);
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.Mesh = (ushort)(240 + Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.Mesh % 10);

                                                        Update upd = new Update(true);
                                                        upd.UID = Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.UID;
                                                        upd.Append(Update.Mesh, Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LeftGate.Mesh);
                                                        client.SendScreen(upd, true);
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        client.Entity.Teleport(1038, 162, 198);
                                                        break;
                                                    }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 516075:
                                {
                                    if (client.Guild != null)
                                    {
                                        if (client.Guild.PoleKeeper)
                                        {
                                            switch (npcRequest.OptionID)
                                            {
                                                case 0:
                                                    dialog.Text("Select the option you want to pursue.");
                                                    if (client.AsMember.Rank != Conquer_Online_Server.Game.Enums.GuildMemberRank.Member)
                                                    {
                                                        dialog.Option("Open gate.", 1);
                                                        dialog.Option("Close gate.", 2);
                                                    }
                                                    dialog.Option("Get inside.", 3);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                    break;
                                                case 1:
                                                    {
                                                        Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.Mesh = (ushort)(280 + Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.Mesh % 10);

                                                        Update upd = new Update(true);
                                                        upd.UID = Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.UID;
                                                        upd.Append(Update.Mesh, Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.Mesh);
                                                        client.SendScreen(upd, true);
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.Mesh = (ushort)(270 + Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.Mesh % 10);

                                                        Update upd = new Update(true);
                                                        upd.UID = Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.UID;
                                                        upd.Append(Update.Mesh, Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.RightGate.Mesh);
                                                        client.SendScreen(upd, true);
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        client.Entity.Teleport(1038, 210, 177);
                                                        break;
                                                    }
                                            }
                                        }
                                    }
                                    break;
                                }
                            #endregion

                            #region Guild Conductresses

                            #region Exit Guild Arena
                            case 7000:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Do you want to leave the guild arena?");
                                                dialog.Option("Yes.", 1);
                                                dialog.Option("No.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                client.Entity.Teleport(1002, 429, 378);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion


                            #endregion

                            #region Guild Conductors
                            #region IN
                            case 9884:
                            case 9885:
                            case 9986:
                            case 9987:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("In exchange of a fee of 1000gold, I will teleport you to a special place. If you don't have money, don't bother me.");
                                                dialog.Option("Teleport me.", 1);
                                                dialog.Option("I'm too poor.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 1000)
                                                {
                                                    client.Entity.Money -= 1000;
                                                    switch (client.ActiveNpc)
                                                    {
                                                        case 9884: client.Entity.Teleport(1216, 12, 481); break;
                                                        case 9885: client.Entity.Teleport(1213, 439, 261); break;
                                                        case 9986: client.Entity.Teleport(1217, 536, 558); break;
                                                        case 9987: client.Entity.Teleport(1001, 337, 325); break;
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Promotion Center
                case 1004:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Promotion Center

                            #region HeadAbbot
                            case 4271:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    dialog.Text("I am the monk trainer, master of balence in destruction and harmony.\nWhat do you seek, young monk?");
                                                    dialog.Option("Promote me.", 1);
                                                    dialog.Option("Learn skills.", 2);
                                                    dialog.Option("Claim bound items.", 49);
                                                    dialog.Option("Wait a minute.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #region BoundItems
                                        case 49:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.HeadgearClaim)
                                                    dialog.Option("Headgear (Lvl 15).", 50);
                                                if (!client.RingClaim)
                                                    dialog.Option("Ring (Lvl 40).", 51);
                                                if (!client.NecklaceClaim)
                                                    dialog.Option("Necklace (Lvl 50).", 52);
                                                if (!client.ArmorClaim)
                                                    dialog.Option("Armor (Lvl 70).", 53);
                                                dialog.Option("Next.", 48);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 48:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.BootsClaim)
                                                    dialog.Option("Boots (Lvl 80).", 54);
                                                if (!client.TowerClaim)
                                                    dialog.Option("Tower (Lvl 100).", 55);
                                                if (!client.FanClaim)
                                                    dialog.Option("Fan (Lvl 100).", 56);
                                                if (!client.WeaponClaim)
                                                    dialog.Option("Weapon (Lvl 110).", 57);
                                                dialog.Option("Back.", 49);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 50:
                                            {
                                                uint itemid = 143009;
                                                byte level = 15;
                                                if (!client.HeadgearClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        item.ID = itemid; 
                                                        item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 1;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.HeadgearClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 51:
                                            {
                                                uint itemid = 150079;
                                                byte level = 40;
                                                if (!client.RingClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        item.ID = itemid;
                                                        item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 1;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.RingClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 52:
                                            {
                                                uint itemid = 120099;
                                                byte level = 50;
                                                if (!client.NecklaceClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        item.ID = itemid;
                                                        item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 1;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.NecklaceClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 53:
                                            {
                                                uint itemid = 136069;
                                                byte level = 70;
                                                if (!client.ArmorClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        item.ID = itemid;
                                                        item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 1;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.ArmorClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 54:
                                            {
                                                uint itemid = 160159;
                                                byte level = 80;
                                                if (!client.BootsClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        item.ID = itemid;
                                                        item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 1;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.BootsClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 55:
                                            {
                                                uint itemid = 202009;
                                                byte level = 100;
                                                if (!client.TowerClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        item.ID = itemid;
                                                        item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 1;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.TowerClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 56:
                                            {
                                                uint itemid = 201009;
                                                byte level = 100;
                                                if (!client.FanClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        item.ID = itemid;
                                                        item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 1;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.FanClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 57:
                                            {
                                                byte level = 100;
                                                if (!client.WeaponClaim)
                                                {
                                                    ISkill proff = null;
                                                    ushort maxlevel = 0;
                                                    foreach (var prof in client.Proficiencies.Values)
                                                    {
                                                        if (maxlevel < prof.Level && prof.ID != 0)
                                                        {
                                                            maxlevel = prof.Level;
                                                            proff = prof;
                                                        }
                                                    }
                                                    if (proff == null)
                                                        return;
                                                    uint itemid = 610199;
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.WeaponClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Promote
                                        case 1:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Class == 65)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 3);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Class == 65)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {

                                                        if (client.PromoteItemNeed == 721020)
                                                        {
                                                            if (client.Inventory.Remove("moonbox"))
                                                            {
                                                                client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                                client.Entity.Class++;
                                                                dialog.Text("Congratulations! You have been promoted.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                            }

                                                            else
                                                            {
                                                                dialog.Text("You don't meet the requierments.");
                                                                dialog.Option("Ahh.", 255);
                                                                dialog.Send();
                                                            }
                                                            return;
                                                        }
                                                        if (client.Inventory.Contains(client.PromoteItemNeed, client.PromoteItemCountNeed) && client.Entity.Level >= client.PromoteLevelNeed)
                                                        {
                                                            client.Inventory.Remove(client.PromoteItemNeed, client.PromoteItemCountNeed);
                                                            client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                            client.Entity.Class++;
                                                            dialog.Text("Congratulations! You have been promoted.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't meet the requierments.");
                                                            dialog.Option("Ahh.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        #region Skills
                                        case 2:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("Triple Attack (Lvl 5).", 6);
                                                    dialog.Option("Oblivion (XP) (Lvl 15)", 7);
                                                    dialog.Option("Whirlwind Kick (Lvl 15)", 8);
                                                    dialog.Option("Radiant Palm (Lvl 40)", 9);
                                                    dialog.Option("Serenity (Lvl 40)", 10);
                                                    dialog.Option("Tranquility (Lvl 70)", 11);
                                                    dialog.Option("Compassion (Lvl 100)", 12);
                                                    dialog.Option("Auras (Lvl 20->100)", 5);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    dialog.Text("Which aura do you want to learn?");
                                                    dialog.Option("Tyrant (Lvl 20).", 13);
                                                    dialog.Option("Fend (Lvl 20)", 14);
                                                    //dialog.Option("Metal (Lvl 100)", 15);
                                                    //dialog.Option("Wood (Lvl 100)", 16);
                                                    //dialog.Option("Water (Lvl 100)", 17);
                                                    //dialog.Option("Fire (Lvl 100)", 18);
                                                    //dialog.Option("Earth (Lvl 100)", 19);
                                                    dialog.Option("Back to skills.", 2);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 5)
                                                    {
                                                        dialog.Text("You have learned Triple Attack.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10490)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 5 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 15)
                                                    {
                                                        dialog.Text("You have learned the Oblivion XP skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10390)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 15 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 15)
                                                    {
                                                        dialog.Text("You have learned Whirlwind Kick.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10415)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 15 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        dialog.Text("You have learned Radiant Palm.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10381)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 10:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        dialog.Text("You have learned Serenity.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10400)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 11:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        dialog.Text("You have learned Tranquility.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10425)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 12:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 100)
                                                    {
                                                        dialog.Text("You have learned Compassion.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10430)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 100 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 13:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 20)
                                                    {
                                                        dialog.Text("You have learned Tyrent Aura.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10395)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 20 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 14:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 20)
                                                    {
                                                        dialog.Text("You have learned Fend Aura.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10410)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 20 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 15:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 100)
                                                    {
                                                        dialog.Text("You have learned Metal Elemental Aura.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10420)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 100 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 16:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 100)
                                                    {
                                                        dialog.Text("You have learned Wood Elemental Aura.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10421)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 100 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 17:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 100)
                                                    {
                                                        dialog.Text("You have learned Water Elemental Aura.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10422)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 100 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 18:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 100)
                                                    {
                                                        dialog.Text("You have learned Fire Elemental Aura.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10423)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 100 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 19:
                                            {
                                                if (client.Entity.Class >= 60 && client.Entity.Class <= 65)
                                                {
                                                    if (client.Entity.Level >= 100)
                                                    {
                                                        dialog.Text("You have learned Earth Elemental Aura.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(10424)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 100 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("The ancient secrets of the monk is not for trade.\nIf you wish to learn the secrets of the monk come back in another life. Good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                    }
                                    break;
                                }
                            #endregion

                            #region WarriorGod
                            case 10001:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.Class >= 20 && client.Entity.Class <= 25)
                                                {
                                                    dialog.Text("I am the shield and two hand weapons master. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                    dialog.Option("Promote me.", 1);
                                                    dialog.Option("Learn skills.", 2);
                                                    dialog.Option("Claim bound items.", 49);
                                                    dialog.Option("Wait a minute.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the warrior secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #region BoundItems
                                        case 49:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.HeadgearClaim)
                                                    dialog.Option("Headgear (Lvl 15).", 50);
                                                if (!client.RingClaim)
                                                    dialog.Option("Ring (Lvl 40).", 51);
                                                if (!client.NecklaceClaim)
                                                    dialog.Option("Necklace (Lvl 50).", 52);
                                                if (!client.ArmorClaim)
                                                    dialog.Option("Armor (Lvl 70).", 53);
                                                dialog.Option("Next.", 48);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 48:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.BootsClaim)
                                                    dialog.Option("Boots (Lvl 80).", 54);
                                                if (!client.TowerClaim)
                                                    dialog.Option("Tower (Lvl 100).", 55);
                                                if (!client.FanClaim)
                                                    dialog.Option("Fan (Lvl 100).", 56);
                                                if (!client.WeaponClaim)
                                                    dialog.Option("Weapon (Lvl 110).", 57);
                                                dialog.Option("Back.", 49);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 50:
                                            {
                                                uint itemid = 111009;
                                                byte level = 15;
                                                if (!client.HeadgearClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.HeadgearClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 51:
                                            {
                                                uint itemid = 150079;
                                                byte level = 40;
                                                if (!client.RingClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.RingClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 52:
                                            {
                                                uint itemid = 120099;
                                                byte level = 50;
                                                if (!client.NecklaceClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.NecklaceClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 53:
                                            {
                                                uint itemid = 131069;
                                                byte level = 70;
                                                if (!client.ArmorClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.ArmorClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 54:
                                            {
                                                uint itemid = 160159;
                                                byte level = 80;
                                                if (!client.BootsClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.BootsClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 55:
                                            {
                                                uint itemid = 202009;
                                                byte level = 100;
                                                if (!client.TowerClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.TowerClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 56:
                                            {
                                                uint itemid = 201009;
                                                byte level = 100;
                                                if (!client.FanClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.FanClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 57:
                                            {
                                                byte level = 100;
                                                if (!client.WeaponClaim)
                                                {
                                                    ISkill proff = null;
                                                    ushort maxlevel = 0;
                                                    foreach (var prof in client.Proficiencies.Values)
                                                    {
                                                        if (maxlevel < prof.Level && prof.ID != 0)
                                                        {
                                                            maxlevel = prof.Level;
                                                            proff = prof;
                                                        }
                                                    }
                                                    if (proff == null)
                                                        return;
                                                    uint itemid = (uint)(proff.ID * 1000 + 219);
                                                    if (!Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations.ContainsKey(itemid))
                                                    {
                                                        dialog.Text("You need more proficiency belonging to a weapon, and not a shield or your punch.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                        return;
                                                    }
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.WeaponClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        case 1:
                                            {
                                                if (client.Entity.Class >= 20 && client.Entity.Class <= 25)
                                                {
                                                    if (client.Entity.Class == 25)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 3);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the warrior secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Class >= 20 && client.Entity.Class <= 25)
                                                {
                                                    if (client.Entity.Class == 25)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {

                                                        if (client.PromoteItemNeed == 721020)
                                                        {
                                                            if (client.Inventory.Remove("moonbox"))
                                                            {
                                                                client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                                client.Entity.Class++;
                                                                dialog.Text("Congratulations! You have been promoted.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                            }

                                                            else
                                                            {
                                                                dialog.Text("You don't meet the requierments.");
                                                                dialog.Option("Ahh.", 255);
                                                                dialog.Send();
                                                            }
                                                            return;
                                                        }
                                                        if (client.Inventory.Contains(client.PromoteItemNeed, client.PromoteItemCountNeed) && client.Entity.Level >= client.PromoteLevelNeed)
                                                        {
                                                            client.Inventory.Remove(client.PromoteItemNeed, client.PromoteItemCountNeed);
                                                            client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                            client.Entity.Class++;
                                                            dialog.Text("Congratulations! You have been promoted.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't meet the requierments.");
                                                            dialog.Option("Ahh.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the warrior secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Class >= 20 && client.Entity.Class <= 25)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("XP Skills (Lvl 40).", 5);
                                                    dialog.Option("Dash (Lvl 61).", 6);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the warrior secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Class >= 20 && client.Entity.Class <= 25)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        dialog.Text("You have learned the XP Skills of this class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        client.AddSpell(LearnableSpell(1025));
                                                        client.AddSpell(LearnableSpell(1020));
                                                        client.AddSpell(LearnableSpell(1015));
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the warrior secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Entity.Class >= 20 && client.Entity.Class <= 25)
                                                {
                                                    if (client.Entity.Level >= 61)
                                                    {
                                                        dialog.Text("You have learned the Dash Skill of this class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(1051)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 61 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the warrior secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Archer Master
                            case 400:
                                {
                                    byte mClass = 40;
                                    byte MClass = 45;
                                    string Class = "archer";
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("I am the bow master. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                    dialog.Option("Promote me.", 1);
                                                    dialog.Option("Learn skills.", 2);
                                                    dialog.Option("Claim bound items.", 49);
                                                    dialog.Option("Wait a minute.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #region BoundItems
                                        case 49:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.HeadgearClaim)
                                                    dialog.Option("Headgear (Lvl 15).", 50);
                                                if (!client.RingClaim)
                                                    dialog.Option("Ring (Lvl 40).", 51);
                                                if (!client.NecklaceClaim)
                                                    dialog.Option("Necklace (Lvl 50).", 52);
                                                if (!client.ArmorClaim)
                                                    dialog.Option("Armor (Lvl 70).", 53);
                                                dialog.Option("Next.", 48);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 48:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.BootsClaim)
                                                    dialog.Option("Boots (Lvl 80).", 54);
                                                if (!client.TowerClaim)
                                                    dialog.Option("Tower (Lvl 100).", 55);
                                                if (!client.FanClaim)
                                                    dialog.Option("Fan (Lvl 100).", 56);
                                                if (!client.WeaponClaim)
                                                    dialog.Option("Weapon (Lvl 110).", 57);
                                                dialog.Option("Back.", 49);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 50:
                                            {
                                                uint itemid = 113009;
                                                byte level = 15;
                                                if (!client.HeadgearClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.HeadgearClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 51:
                                            {
                                                uint itemid = 150079;
                                                byte level = 40;
                                                if (!client.RingClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.RingClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 52:
                                            {
                                                uint itemid = 120099;
                                                byte level = 50;
                                                if (!client.NecklaceClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.NecklaceClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 53:
                                            {
                                                uint itemid = 133049;
                                                byte level = 70;
                                                if (!client.ArmorClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.ArmorClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 54:
                                            {
                                                uint itemid = 160159;
                                                byte level = 80;
                                                if (!client.BootsClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.BootsClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 55:
                                            {
                                                uint itemid = 202009;
                                                byte level = 100;
                                                if (!client.TowerClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.TowerClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 56:
                                            {
                                                uint itemid = 201009;
                                                byte level = 100;
                                                if (!client.FanClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.FanClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 57:
                                            {
                                                byte level = 100;
                                                if (!client.WeaponClaim)
                                                {
                                                    ISkill proff = null;
                                                    ushort maxlevel = 0;
                                                    foreach (var prof in client.Proficiencies.Values)
                                                    {
                                                        if (maxlevel < prof.Level && prof.ID != 0)
                                                        {
                                                            maxlevel = prof.Level;
                                                            proff = prof;
                                                        }
                                                    }
                                                    if (proff == null)
                                                        return;
                                                    uint itemid = (uint)(proff.ID * 1000 + 219);
                                                    if (!Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations.ContainsKey(itemid))
                                                    {
                                                        Console.WriteLine("invalid claim weapon " + itemid);
                                                        return;
                                                    }
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.WeaponClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        case 1:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 3);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        if (client.PromoteItemNeed == 721020)
                                                        {
                                                            if (client.Inventory.Remove("moonbox"))
                                                            {
                                                                client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                                client.Entity.Class++;
                                                                dialog.Text("Congratulations! You have been promoted.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                            }

                                                            else
                                                            {
                                                                dialog.Text("You don't meet the requierments.");
                                                                dialog.Option("Ahh.", 255);
                                                                dialog.Send();
                                                            }
                                                            return;
                                                        }
                                                        if (client.Inventory.Contains(client.PromoteItemNeed, client.PromoteItemCountNeed) && client.Entity.Level >= client.PromoteLevelNeed)
                                                        {
                                                            client.Inventory.Remove(client.PromoteItemNeed, client.PromoteItemCountNeed);
                                                            client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                            client.Entity.Class++;
                                                            dialog.Text("Congratulations! You have been promoted.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't meet the requierments.");
                                                            dialog.Option("Ahh.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("XPFly (Lvl 1).", 5);
                                                    dialog.Option("Scatter (Lvl 23).", 6);
                                                    dialog.Option("RapidFire (Lvl 40).", 7);
                                                    dialog.Option("Fly (Lvl 70).", 8);
                                                    dialog.Option("Intensify (Lvl 70).", 9);
                                                    dialog.Option("Arrow rain (Lvl 70).", 10);
                                                    dialog.Option("Advanced Fly (Lvl 100).", 11);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("You have learned the XP Skill of this class.");
                                                    dialog.Option("Thank you master.", 255);
                                                    dialog.Send();
                                                    if (!client.AddSpell(LearnableSpell(8002)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 23)
                                                    {
                                                        dialog.Text("You have learned the scatter.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        if (!client.AddSpell(LearnableSpell(8001)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 23 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(8000)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the rapid fire.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {

                                                        if (!client.AddSpell(LearnableSpell(8003)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the fly.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(9000)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the intensify.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 10:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(8030)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the arrow rain.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 11:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 100)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(8003, 1)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the advanced fly.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 100 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region WindSage
                            case 4720:
                                {
                                    byte mClass = 50;
                                    byte MClass = 55;
                                    string Class = "ninja";
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("I am the master of the no sound attack and the master of the katanas. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                    dialog.Option("Promote me.", 1);
                                                    dialog.Option("Learn skills.", 2);
                                                    dialog.Option("Claim bound items.", 49);
                                                    dialog.Option("Wait a minute.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #region BoundItems
                                        case 49:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.HeadgearClaim)
                                                    dialog.Option("Headgear (Lvl 15).", 50);
                                                if (!client.RingClaim)
                                                    dialog.Option("Ring (Lvl 40).", 51);
                                                if (!client.NecklaceClaim)
                                                    dialog.Option("Necklace (Lvl 50).", 52);
                                                if (!client.ArmorClaim)
                                                    dialog.Option("Armor (Lvl 70).", 53);
                                                dialog.Option("Next.", 48);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 48:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.BootsClaim)
                                                    dialog.Option("Boots (Lvl 80).", 54);
                                                if (!client.TowerClaim)
                                                    dialog.Option("Tower (Lvl 100).", 55);
                                                if (!client.FanClaim)
                                                    dialog.Option("Fan (Lvl 100).", 56);
                                                if (!client.WeaponClaim)
                                                    dialog.Option("Weapon (Lvl 110).", 57);
                                                dialog.Option("Back.", 49);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 50:
                                            {
                                                uint itemid = 123009;
                                                byte level = 15;
                                                if (!client.HeadgearClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.HeadgearClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 51:
                                            {
                                                uint itemid = 150079;
                                                byte level = 40;
                                                if (!client.RingClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.RingClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 52:
                                            {
                                                uint itemid = 120099;
                                                byte level = 50;
                                                if (!client.NecklaceClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.NecklaceClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 53:
                                            {
                                                uint itemid = 135069;
                                                byte level = 70;
                                                if (!client.ArmorClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.ArmorClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 54:
                                            {
                                                uint itemid = 160159;
                                                byte level = 80;
                                                if (!client.BootsClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.BootsClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 55:
                                            {
                                                uint itemid = 202009;
                                                byte level = 100;
                                                if (!client.TowerClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.TowerClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 56:
                                            {
                                                uint itemid = 201009;
                                                byte level = 100;
                                                if (!client.FanClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.FanClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 57:
                                            {
                                                byte level = 100;
                                                if (!client.WeaponClaim)
                                                {
                                                    ISkill proff = null;
                                                    ushort maxlevel = 0;
                                                    foreach (var prof in client.Proficiencies.Values)
                                                    {
                                                        if (maxlevel < prof.Level && prof.ID != 0)
                                                        {
                                                            maxlevel = prof.Level;
                                                            proff = prof;
                                                        }
                                                    }
                                                    if (proff == null)
                                                        return;
                                                    uint itemid = (uint)(proff.ID * 1000 + 219);
                                                    if (!Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations.ContainsKey(itemid))
                                                    {
                                                        Console.WriteLine("invalid claim weapon " + itemid);
                                                        return;
                                                    }
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.WeaponClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        case 1:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 3);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        if (client.PromoteItemNeed == 721020)
                                                        {
                                                            if (client.Inventory.Remove("moonbox"))
                                                            {
                                                                client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                                client.Entity.Class++;
                                                                dialog.Text("Congratulations! You have been promoted.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                            }
                                                            else
                                                            {
                                                                dialog.Text("You don't meet the requierments.");
                                                                dialog.Option("Ahh.", 255);
                                                                dialog.Send();
                                                            }
                                                            return;
                                                        }
                                                        if (client.Inventory.Contains(client.PromoteItemNeed, client.PromoteItemCountNeed) && client.Entity.Level >= client.PromoteLevelNeed)
                                                        {
                                                            client.Inventory.Remove(client.PromoteItemNeed, client.PromoteItemCountNeed);
                                                            client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                            client.Entity.Class++;
                                                            dialog.Text("Congratulations! You have been promoted.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't meet the requierments.");
                                                            dialog.Option("Ahh.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("Let me know what you want to learn. '[...]' and '{...}' means ninja as second life and ninja as third life.");
                                                    dialog.Option("TwofoldBlades (Lvl 40).", 5);
                                                    dialog.Option("ToxicFog (Lvl 70).", 6);
                                                    dialog.Option("PoisonStar [Lvl 70].", 7);
                                                    //dialog.Option("CounterKill {Lvl 70}).", 8);
                                                    dialog.Option("ArcherBane (Lvl 110).", 9);
                                                    dialog.Option("ShurikenVortex (Lvl 70).", 10);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(6000)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the TwofoldBlades.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 23 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(6001)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the ToxicFog.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if ((client.Entity.Reborn == 1 && client.Entity.Class == 55) || (client.Entity.Reborn == 2 && client.Entity.SecondRebornClass == 55))
                                                        {
                                                            if (!client.AddSpell(LearnableSpell(6002)))
                                                            {
                                                                dialog.Text("You already know this skill.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                            dialog.Text("You have learned the PoisonStar.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need to be ninja in the second life.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (client.Entity.Reborn == 2 && client.Entity.Class == 55)
                                                        {
                                                            if (!client.AddSpell(LearnableSpell(6003)))
                                                            {
                                                                dialog.Text("You already know this skill.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                            dialog.Text("You have learned the CounterKill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need to be ninja in the third life.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 110)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(6004)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the ArcherBane.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 10:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(6010)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the ShurikenVortex.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region TrojanMaster
                            case 10022:
                                {
                                    byte mClass = 10;
                                    byte MClass = 15;
                                    string Class = "trojan";
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("I am the master of the one hand weapons. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                    dialog.Option("Promote me.", 1);
                                                    dialog.Option("Learn skills.", 2);
                                                    dialog.Option("Claim bound items.", 49);
                                                    dialog.Option("Wait a minute.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #region BoundItems
                                        case 49:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.HeadgearClaim)
                                                    dialog.Option("Headgear (Lvl 15).", 50);
                                                if (!client.RingClaim)
                                                    dialog.Option("Ring (Lvl 40).", 51);
                                                if (!client.NecklaceClaim)
                                                    dialog.Option("Necklace (Lvl 50).", 52);
                                                if (!client.ArmorClaim)
                                                    dialog.Option("Armor (Lvl 70).", 53);
                                                dialog.Option("Next.", 48);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 48:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.BootsClaim)
                                                    dialog.Option("Boots (Lvl 80).", 54);
                                                if (!client.TowerClaim)
                                                    dialog.Option("Tower (Lvl 100).", 55);
                                                if (!client.FanClaim)
                                                    dialog.Option("Fan (Lvl 100).", 56);
                                                if (!client.WeaponClaim)
                                                    dialog.Option("Weapon (Lvl 110).", 57);
                                                dialog.Option("Back.", 49);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 50:
                                            {
                                                uint itemid = 118009;
                                                byte level = 15;
                                                if (!client.HeadgearClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.HeadgearClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 51:
                                            {
                                                uint itemid = 150079;
                                                byte level = 40;
                                                if (!client.RingClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.RingClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 52:
                                            {
                                                uint itemid = 120099;
                                                byte level = 50;
                                                if (!client.NecklaceClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.NecklaceClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 53:
                                            {
                                                uint itemid = 130069;
                                                byte level = 70;
                                                if (!client.ArmorClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.ArmorClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 54:
                                            {
                                                uint itemid = 160159;
                                                byte level = 80;
                                                if (!client.BootsClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.BootsClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 55:
                                            {
                                                uint itemid = 202009;
                                                byte level = 100;
                                                if (!client.TowerClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.TowerClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 56:
                                            {
                                                uint itemid = 201009;
                                                byte level = 100;
                                                if (!client.FanClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.FanClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 57:
                                            {
                                                byte level = 100;
                                                if (!client.WeaponClaim)
                                                {
                                                    ISkill proff = null;
                                                    ushort maxlevel = 0;
                                                    foreach (var prof in client.Proficiencies.Values)
                                                    {
                                                        if (maxlevel < prof.Level && prof.ID != 0)
                                                        {
                                                            maxlevel = prof.Level;
                                                            proff = prof;
                                                        }
                                                    }
                                                    if (proff == null)
                                                        return;
                                                    uint itemid = (uint)(proff.ID * 1000 + 219);
                                                    if (!Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations.ContainsKey(itemid))
                                                    {
                                                        Console.WriteLine("invalid claim weapon " + itemid);
                                                        return;
                                                    }
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.WeaponClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        case 1:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 3);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        if (client.PromoteItemNeed == 721020)
                                                        {
                                                            if (client.Inventory.Remove("moonbox"))
                                                            {
                                                                client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                                client.Entity.Class++;
                                                                dialog.Text("Congratulations! You have been promoted.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                            }

                                                            else
                                                            {
                                                                dialog.Text("You don't meet the requierments.");
                                                                dialog.Option("Ahh.", 255);
                                                                dialog.Send();
                                                            }
                                                            return;
                                                        }
                                                        if (client.Inventory.Contains(client.PromoteItemNeed, client.PromoteItemCountNeed) && client.Entity.Level >= client.PromoteLevelNeed)
                                                        {
                                                            client.Inventory.Remove(client.PromoteItemNeed, client.PromoteItemCountNeed);
                                                            client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                            client.Entity.Class++;
                                                            dialog.Text("Congratulations! You have been promoted.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't meet the requierments.");
                                                            dialog.Option("Ahh.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("XPSkills (Lvl 40).", 5);
                                                    dialog.Option("Hercules (Lvl 40).", 6);
                                                    dialog.Option("Golem. (Lvl 40).", 7);
                                                    dialog.Option("Spritual Healing. (Lvl 40).", 8);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        dialog.Text("You have learned the XP Skills.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        client.AddSpell(LearnableSpell(1110));
                                                        client.AddSpell(LearnableSpell(1015));
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1115)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the Hercules.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1270)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the Golem.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1190)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the Spritual Healing.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Taoist Moon
                            case 10000:
                                {
                                    byte mClass = 142;
                                    byte MClass = 145;
                                    string Class = "fire taoist";
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.Class >= 100 && client.Entity.Class <= 101)
                                                {
                                                    dialog.Text("I am the master of the fire skills. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                    dialog.Option("Promote me.", 100);
                                                    dialog.Option("Learn basic skills.", 200);
                                                    dialog.Option("Claim bound items.", 49);
                                                    dialog.Option("Wait a minute.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                    {
                                                        dialog.Text("I am the master of the fire skills. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                        dialog.Option("Promote me.", 1);
                                                        dialog.Option("Learn skills.", 2);
                                                        dialog.Option("Learn basic skills.", 200);
                                                        dialog.Option("Claim bound items.", 49);
                                                        dialog.Option("Wait a minute.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                break;
                                            }
                                        #region BoundItems
                                        case 49:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.HeadgearClaim)
                                                    dialog.Option("Headgear (Lvl 15).", 50);
                                                if (!client.RingClaim)
                                                    dialog.Option("Ring (Lvl 40).", 51);
                                                if (!client.NecklaceClaim)
                                                    dialog.Option("Necklace (Lvl 50).", 52);
                                                if (!client.ArmorClaim)
                                                    dialog.Option("Armor (Lvl 70).", 53);
                                                dialog.Option("Next.", 48);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 48:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.BootsClaim)
                                                    dialog.Option("Boots (Lvl 80).", 54);
                                                if (!client.TowerClaim)
                                                    dialog.Option("Tower (Lvl 100).", 55);
                                                if (!client.FanClaim)
                                                    dialog.Option("Fan (Lvl 100).", 56);
                                                if (!client.WeaponClaim)
                                                    dialog.Option("Weapon (Lvl 110).", 57);
                                                dialog.Option("Back.", 49);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 50:
                                            {
                                                uint itemid = 117009;
                                                byte level = 15;
                                                if (!client.HeadgearClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.HeadgearClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 51:
                                            {
                                                uint itemid = 152089;
                                                byte level = 40;
                                                if (!client.RingClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.RingClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 52:
                                            {
                                                uint itemid = 121099;
                                                byte level = 50;
                                                if (!client.NecklaceClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.NecklaceClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 53:
                                            {
                                                uint itemid = 134069;
                                                byte level = 70;
                                                if (!client.ArmorClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.ArmorClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 54:
                                            {
                                                uint itemid = 160159;
                                                byte level = 80;
                                                if (!client.BootsClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.BootsClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 55:
                                            {
                                                uint itemid = 202009;
                                                byte level = 100;
                                                if (!client.TowerClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.TowerClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 56:
                                            {
                                                uint itemid = 201009;
                                                byte level = 100;
                                                if (!client.FanClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.FanClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 57:
                                            {
                                                byte level = 100;
                                                if (!client.WeaponClaim)
                                                {
                                                    ISkill proff = null;
                                                    ushort maxlevel = 0;
                                                    foreach (var prof in client.Proficiencies.Values)
                                                    {
                                                        if (maxlevel < prof.Level && prof.ID != 0)
                                                        {
                                                            maxlevel = prof.Level;
                                                            proff = prof;
                                                        }
                                                    }
                                                    if (proff == null)
                                                        return;
                                                    uint itemid = (uint)(proff.ID * 1000 + 219);
                                                    if (!Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations.ContainsKey(itemid))
                                                    {
                                                        Console.WriteLine("invalid claim weapon " + itemid);
                                                        return;
                                                    }
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.WeaponClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        case 100:
                                            {
                                                if (client.Entity.Class >= 100 && client.Entity.Class <= 101)
                                                {
                                                    if (client.Entity.Class == 101)
                                                    {
                                                        dialog.Text("Do you want to become a fire taoist?");
                                                        dialog.Option("Yes sir.", 254);
                                                        dialog.Option("No thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 254);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 254:
                                            {
                                                if (client.Entity.Class == 100)
                                                {
                                                    client.Entity.Class++;
                                                    dialog.Text("Congratulations! You have been promoted.");
                                                    dialog.Option("Thank you master.", 255);
                                                    dialog.Send();
                                                }
                                                else if (client.Entity.Class == 101)
                                                {
                                                    client.Entity.Class = 142;
                                                    dialog.Text("Congratulations! You have been promoted.");
                                                    dialog.Option("Thank you master.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 200:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("Thunder (Lvl 1).", 205);
                                                    dialog.Option("Cure (Lvl 1).", 206);
                                                    dialog.Option("Meditation. (Lvl 40).", 207);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 205:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    if (!client.AddSpell(LearnableSpell(1000)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    dialog.Text("You have learned thunder.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 206:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    if (!client.AddSpell(LearnableSpell(1005)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    dialog.Text("You have learned cure.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 207:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1195)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned meditation.");
                                                        dialog.Option("Thank you.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 3);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        if (client.PromoteItemNeed == 721020)
                                                        {
                                                            if (client.Inventory.Remove("moonbox"))
                                                            {
                                                                client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                                client.Entity.Class++;
                                                                dialog.Text("Congratulations! You have been promoted.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                            }

                                                            else
                                                            {
                                                                dialog.Text("You don't meet the requierments.");
                                                                dialog.Option("Ahh.", 255);
                                                                dialog.Send();
                                                            }
                                                            return;
                                                        }
                                                        if (client.Inventory.Contains(client.PromoteItemNeed, client.PromoteItemCountNeed) && client.Entity.Level >= client.PromoteLevelNeed)
                                                        {
                                                            client.Inventory.Remove(client.PromoteItemNeed, client.PromoteItemCountNeed);
                                                            client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                            client.Entity.Class++;
                                                            dialog.Text("Congratulations! You have been promoted.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't meet the requierments.");
                                                            dialog.Option("Ahh.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("Fire (Lvl 42 + Thunder lvl 4).", 5);
                                                    dialog.Option("Bomb. (Lvl 43).", 6);
                                                    dialog.Option("FireCircle. (Lvl 48).", 7);
                                                    dialog.Option("FireRing. (Lvl 52).", 8);
                                                    dialog.Option("FireMeteor. (Lvl 55).", 9);
                                                    dialog.Option("Tornado (Lvl 81 + Fire lvl 3).", 10);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 42)
                                                    {
                                                        if (client.Spells.ContainsKey(1000) && client.Spells[1000] != null && client.Spells[1000].Level == 4)
                                                        {
                                                            if (!client.AddSpell(LearnableSpell(1001)))
                                                            {
                                                                dialog.Text("You already know this skill.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                            dialog.Text("You have learned the fire.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need to know thunder very well to be able to learn the fire skill.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 42 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 10:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 81)
                                                    {
                                                        if (client.Spells.ContainsKey(1001) && client.Spells[1001] != null && client.Spells[1001].Level == 3)
                                                        {
                                                            if (!client.AddSpell(LearnableSpell(1002)))
                                                            {
                                                                dialog.Text("You already know this skill.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                                break;
                                                            }
                                                            dialog.Text("You have learned the tornado.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need to know thunder very well to be able to learn the fire skill.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 81 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 43)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1160)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the bomb skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 43 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 48)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1120)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the fire circle skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 48 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 52)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1150)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the fire ring skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 52 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 55)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1180)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the fire meteor skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 55 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region Taoist Star
                            case 30:
                                {
                                    byte mClass = 132;
                                    byte MClass = 135;
                                    string Class = "water taoist";
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.Entity.Class >= 100 && client.Entity.Class <= 101)
                                                {
                                                    dialog.Text("I am the master of the fire skills. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                    dialog.Option("Promote me.", 100);
                                                    dialog.Option("Learn basic skills.", 200);
                                                    dialog.Option("Claim bound items.", 49);
                                                    dialog.Option("Wait a minute.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                    {
                                                        dialog.Text("I am the master of the water skills. The water skills, are not skills that would make wounds but heal them. As I see, you started your way on conquering this world. I will try to help you teaching you warrir skills and promoting you.");
                                                        dialog.Option("Promote me.", 1);
                                                        dialog.Option("Learn skills.", 2);
                                                        dialog.Option("Learn basic skills.", 200);
                                                        dialog.Option("Claim bound items.", 49);
                                                        dialog.Option("Wait a minute.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                break;
                                            }
                                        #region BoundItems
                                        case 49:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.HeadgearClaim)
                                                    dialog.Option("Headgear (Lvl 15).", 50);
                                                if (!client.RingClaim)
                                                    dialog.Option("Ring (Lvl 40).", 51);
                                                if (!client.NecklaceClaim)
                                                    dialog.Option("Necklace (Lvl 50).", 52);
                                                if (!client.ArmorClaim)
                                                    dialog.Option("Armor (Lvl 70).", 53);
                                                dialog.Option("Next.", 48);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 48:
                                            {
                                                dialog.Text("We just found some equipment, and I thought that we should give them to our pupils so they can be less defeatable.");
                                                if (!client.BootsClaim)
                                                    dialog.Option("Boots (Lvl 80).", 54);
                                                if (!client.TowerClaim)
                                                    dialog.Option("Tower (Lvl 100).", 55);
                                                if (!client.FanClaim)
                                                    dialog.Option("Fan (Lvl 100).", 56);
                                                if (!client.WeaponClaim)
                                                    dialog.Option("Weapon (Lvl 110).", 57);
                                                dialog.Option("Back.", 49);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 50:
                                            {
                                                uint itemid = 117009;
                                                byte level = 15;
                                                if (!client.HeadgearClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.HeadgearClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 51:
                                            {
                                                uint itemid = 152089;
                                                byte level = 40;
                                                if (!client.RingClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.RingClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 52:
                                            {
                                                uint itemid = 121099;
                                                byte level = 50;
                                                if (!client.NecklaceClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.NecklaceClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 53:
                                            {
                                                uint itemid = 134069;
                                                byte level = 70;
                                                if (!client.ArmorClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.ArmorClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 54:
                                            {
                                                uint itemid = 160159;
                                                byte level = 80;
                                                if (!client.BootsClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.BootsClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 55:
                                            {
                                                uint itemid = 202009;
                                                byte level = 100;
                                                if (!client.TowerClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.TowerClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 56:
                                            {
                                                uint itemid = 201009;
                                                byte level = 100;
                                                if (!client.FanClaim)
                                                {
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Bound = true;
                                                        item.Plus = 3;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.FanClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 57:
                                            {
                                                byte level = 100;
                                                if (!client.WeaponClaim)
                                                {
                                                    ISkill proff = null;
                                                    ushort maxlevel = 0;
                                                    foreach (var prof in client.Proficiencies.Values)
                                                    {
                                                        if (maxlevel < prof.Level && prof.ID != 0)
                                                        {
                                                            maxlevel = prof.Level;
                                                            proff = prof;
                                                        }
                                                    }
                                                    if (proff == null)
                                                        return;
                                                    uint itemid = (uint)(proff.ID * 1000 + 219);
                                                    if (!Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations.ContainsKey(itemid))
                                                    {
                                                        Console.WriteLine("invalid claim weapon " + itemid);
                                                        return;
                                                    }
                                                    if (client.Entity.Level >= level)
                                                    {
                                                        ConquerItem item = new ConquerItem(true);
                                                        
                                                        item.ID = itemid; item.Color = Conquer_Online_Server.Game.Enums.Color.White;
                                                        item.Bound = true;
                                                        item.Durability = item.MaximDurability = Conquer_Online_Server.Database
                                                            .ConquerItemInformation.BaseInformations[itemid].Durability;
                                                        item.Plus = 3;
                                                        item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (client.Inventory.Add(item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd))
                                                        {
                                                            client.WeaponClaim = true;
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You need atleast one free slot in your inventory.");
                                                            dialog.Option("Alright.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You don't have atleast level " + level + ".");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot claim it twice.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        #endregion
                                        case 100:
                                            {
                                                if (client.Entity.Class >= 100 && client.Entity.Class <= 101)
                                                {
                                                    if (client.Entity.Class == 101)
                                                    {
                                                        dialog.Text("Do you want to become a water taoist?");
                                                        dialog.Option("Yes sir.", 254);
                                                        dialog.Option("No thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 254);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 254:
                                            {
                                                if (client.Entity.Class == 100)
                                                {
                                                    client.Entity.Class++;
                                                    dialog.Text("Congratulations! You have been promoted.");
                                                    dialog.Option("Thank you master.", 255);
                                                    dialog.Send();
                                                }
                                                else if (client.Entity.Class == 101)
                                                {
                                                    client.Entity.Class = 132;
                                                    dialog.Text("Congratulations! You have been promoted.");
                                                    dialog.Option("Thank you master.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 200:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("Thunder (Lvl 1).", 205);
                                                    dialog.Option("Cure (Lvl 1).", 206);
                                                    dialog.Option("Meditation. (Lvl 40).", 207);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 205:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    if (!client.AddSpell(LearnableSpell(1000)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    dialog.Text("You have learned thunder.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 206:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    if (!client.AddSpell(LearnableSpell(1005)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    dialog.Text("You have learned cure.");
                                                    dialog.Option("Thank you.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 207:
                                            {
                                                if (client.Entity.Class >= 100)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1195)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned meditation.");
                                                        dialog.Option("Thank you.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("To promote now you need" + client.PromoteItemNameNeed + " level " + client.PromoteLevelNeed + ".");
                                                        dialog.Option("Promote me sir.", 3);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Class == MClass)
                                                    {
                                                        dialog.Text("You cannot be promoted anymore. You have mastered your class.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        if (client.PromoteItemNeed == 721020)
                                                        {
                                                            if (client.Inventory.Remove("moonbox"))
                                                            {
                                                                client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                                client.Entity.Class++;
                                                                dialog.Text("Congratulations! You have been promoted.");
                                                                dialog.Option("Thank you master.", 255);
                                                                dialog.Send();
                                                            }

                                                            else
                                                            {
                                                                dialog.Text("You don't meet the requierments.");
                                                                dialog.Option("Ahh.", 255);
                                                                dialog.Send();
                                                            }
                                                            return;
                                                        }
                                                        if (client.Inventory.Contains(client.PromoteItemNeed, client.PromoteItemCountNeed) && client.Entity.Level >= client.PromoteLevelNeed)
                                                        {
                                                            client.Inventory.Remove(client.PromoteItemNeed, client.PromoteItemCountNeed);
                                                            client.Inventory.Add(client.PromoteItemGain, 0, 1);
                                                            client.Entity.Class++;
                                                            dialog.Text("Congratulations! You have been promoted.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You don't meet the requierments.");
                                                            dialog.Option("Ahh.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("XP Revive (Lvl 1).", 5);
                                                    dialog.Option("Volcano (Lvl 1).", 6);
                                                    dialog.Option("Lightning (Lvl 1).", 7);
                                                    dialog.Option("HealingRain. (Lvl 40).", 8);
                                                    dialog.Option("StarOfAccuracy. (Lvl 40).", 9);
                                                    dialog.Option("MagicShield. (Lvl 40).", 10);
                                                    dialog.Option("Next.", 20);
                                                    dialog.Option("Nothing.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 20:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    dialog.Text("Let me know what you want to learn.");
                                                    dialog.Option("WaterDevil. (Lvl 40).", 21);
                                                    dialog.Option("Stigma. (Lvl 40).", 11);
                                                    dialog.Option("Invisibility. (Lvl 70).", 12);
                                                    dialog.Option("Pray. (Lvl 70).", 13);
                                                    dialog.Option("SpeedLightning (Lvl 70).", 14);
                                                    dialog.Option("AdvancedCure. (Lvl 80).", 15);
                                                    dialog.Option("Nectar. (Lvl 90).", 16);
                                                    dialog.Option("Back.", 2);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 5:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 1)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1050)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned xp revive skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 1 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 21:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1280)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                        else
                                                        {
                                                            dialog.Text("You have learned water devil.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 6:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 1)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1125)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the volcano xp skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 1 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 7:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 1)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1010)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the lightning xp skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 1 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 8:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1055)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the fire healing rain.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 9:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1085)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the star of acurracy skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 10:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1090)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the magic shield skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 11:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 40)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1095)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the stigma skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 40 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 12:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1075)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the invisibility skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 13:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1100)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the pray skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 14:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 70)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(5001)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the speed lightning skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 15:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 80)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1175)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the advanced cure skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 16:
                                            {
                                                if (client.Entity.Class >= mClass && client.Entity.Class <= MClass)
                                                {
                                                    if (client.Entity.Level >= 80)
                                                    {
                                                        if (!client.AddSpell(LearnableSpell(1170)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                            break;
                                                        }
                                                        dialog.Text("You have learned the nectar skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        dialog.Text("You need to be level 70 or more.");
                                                        dialog.Option("Alright.", 255);
                                                        dialog.Send();
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("I will not tell any of the " + Class + " secrets to another class, so, good bye.");
                                                    dialog.Option("Alright.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region MightyTao
                            case 35500:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("If you have reborn atleast one time, you can learn some great skills. With them you will be able to summon some misterious monsters. They will help you conquer this world much faster.");
                                                dialog.Option("Summon guard - 1 euxite ore.", 1);
                                                dialog.Option("Summon class monster - 1 gold ore rate5.", 2);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Reborn > 0)
                                                {
                                                    if (client.Inventory.Contains(1072031, 1))
                                                    {
                                                        client.Inventory.Remove(1072031, 1);
                                                        if (!client.AddSpell(LearnableSpell(4000)))
                                                        {
                                                            dialog.Text("You already know this skill.");
                                                            dialog.Option("Thank you master.", 255);
                                                            dialog.Send();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot learn those skills until you reborn atleast once.");
                                                    dialog.Option("Alright", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Reborn > 0)
                                                {
                                                    if (client.Entity.Class >= 50 && client.Entity.Class <= 55)
                                                    {
                                                        dialog.Text("You cannot learn skills like this one. Ninjas don't need such thing. They are much more stronger than every other class.");
                                                        dialog.Option("Alright", 255);
                                                        dialog.Send();
                                                        break;
                                                    }
                                                    if (client.Inventory.Contains(1072054, 1))
                                                    {
                                                        client.Inventory.Remove(1072054, 1);
                                                        if (client.Entity.Class <= 15)
                                                            client.AddSpell(LearnableSpell(4050));
                                                        else if (client.Entity.Class <= 25)
                                                            client.AddSpell(LearnableSpell(4060));
                                                        else if (client.Entity.Class <= 45)
                                                            client.AddSpell(LearnableSpell(4070));
                                                        else if (client.Entity.Class <= 135)
                                                            client.AddSpell(LearnableSpell(4010));
                                                        else if (client.Entity.Class <= 145)
                                                            client.AddSpell(LearnableSpell(4020));
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You cannot learn those skills until you reborn atleast once.");
                                                    dialog.Option("Alright", 255);
                                                    dialog.Send();
                                                    break;
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Stables
                case 1006:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region Stables
                            #region  Breeder
                            case 9883:
                                {
                                    Data data = new Data(true);
                                    data.ID = Data.OpenWindow;
                                    data.UID = client.Entity.UID;
                                    data.TimeStamp = Time32.Now;
                                    data.dwParam = Data.WindowCommands.Breeding;
                                    data.wParam1 = client.Entity.X;
                                    data.wParam2 = client.Entity.Y;
                                    client.Send(data);
                                    break;
                                }
                            #endregion
                            #region SkillTeacher
                            case 9881:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                dialog.Text("Ready to learn some great skills? Their prices are as good as they are.");
                                                dialog.Option("Riding - 50000 gold.", 1);
                                                dialog.Option("Spook - 100000 gold.", 2);
                                                dialog.Option("WarCry - 1000000 gold.", 3);
                                                dialog.Option("Nothing.", 255);
                                                dialog.Send();
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.Entity.Money >= 50000)
                                                {
                                                    if (!client.AddSpell(LearnableSpell(7001)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        client.Entity.Money -= 50000;
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh sorry.", 255);
                                                }
                                                break;
                                            }
                                        case 2:
                                            {
                                                if (client.Entity.Money >= 100000)
                                                {
                                                    if (!client.AddSpell(LearnableSpell(7002)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        client.Entity.Money -= 100000;
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh sorry.", 255);
                                                }
                                                break;
                                            }
                                        case 3:
                                            {
                                                if (client.Entity.Money >= 1000000)
                                                {
                                                    if (!client.AddSpell(LearnableSpell(7003)))
                                                    {
                                                        dialog.Text("You already know this skill.");
                                                        dialog.Option("Thank you master.", 255);
                                                        dialog.Send();
                                                    }
                                                    else
                                                    {
                                                        client.Entity.Money -= 1000000;
                                                    }
                                                }
                                                else
                                                {
                                                    dialog.Text("You don't meet the requierments.");
                                                    dialog.Option("Ahh sorry.", 255);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Lottery
                case 700:
                    {
                        switch (client.ActiveNpc)
                        {
                            #region LuckyBox
                            case 925:
                            case 926:
                            case 927:
                            case 928:
                            case 929:
                            case 930:
                            case 931:
                            case 932:
                            case 933:
                            case 934:
                            case 935:
                            case 936:
                            case 937:
                            case 938:
                            case 939:
                            case 940:
                            case 942:
                            case 943:
                            case 944:
                            case 945:
                                {
                                    switch (npcRequest.OptionID)
                                    {
                                        case 0:
                                            {
                                                if (client.InLottery)
                                                {
                                                    dialog.Text("Do you really want to chose me?");
                                                    dialog.Option("Yes.", 1);
                                                    dialog.Option("No.", 255);
                                                    dialog.Send();
                                                }
                                                else
                                                {
                                                    dialog.Text("Re-enter the lottery if you want to try me.");
                                                    dialog.Option("Ahh.", 255);
                                                    dialog.Send();
                                                }
                                                break;
                                            }
                                        case 1:
                                            {
                                                if (client.InLottery)
                                                {
                                                    if (client.Inventory.Count == 40)
                                                    {
                                                        client.Send(Conquer_Online_Server.ServerBase.Constants.FullInventory);
                                                        return;
                                                    }
                                                    client.InLottery = false;
                                                tryagain:
                                                    int rand = Conquer_Online_Server.ServerBase.Kernel.Random.Next(Conquer_Online_Server.Database.LotteryTable.LotteryItems.Count);
                                                    var item = Conquer_Online_Server.Database.LotteryTable.LotteryItems[rand];
                                                    var Itemd = Conquer_Online_Server.Database.ConquerItemInformation.BaseInformations[item.ID];
                                                    if (Itemd == null)
                                                        goto tryagain;
                                                    if (Conquer_Online_Server.ServerBase.Kernel.Rate(item.Rank, item.Chance) && Conquer_Online_Server.ServerBase.Kernel.Rate(item.Rank, 35 - item.Rank))
                                                    {
                                                        IConquerItem Item = new ConquerItem(true);
                                                        Item.ID = item.ID;
                                                        Item.Plus = item.Plus;
                                                        Item.Color = Conquer_Online_Server.Game.Enums.Color.Blue;
                                                        if (item.Sockets > 0)
                                                            Item.SocketOne = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        if (item.Sockets > 1)
                                                            Item.SocketTwo = Conquer_Online_Server.Game.Enums.Gem.EmptySocket;
                                                        Item.Durability = Item.MaximDurability = Itemd.Durability;
                                                        client.Inventory.Add(Item, Conquer_Online_Server.Game.Enums.ItemUse.CreateAndAdd);
                                                        if (item.Rank <= 4)
                                                        {
                                                            Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message("Congratulations! " + client.Entity.Name + " won " + item.Name + " in lottery.", System.Drawing.Color.Black, Message.Talk), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                                                        }
                                                        else
                                                        {
                                                            if (Conquer_Online_Server.ServerBase.Kernel.Rate(item.Chance, 100))
                                                            {
                                                                Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message("Congratulations! " + client.Entity.Name + " won " + item.Name + " in lottery.", System.Drawing.Color.Black, Message.Talk), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto tryagain;
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion
                        }
                        break;
                    }
                #endregion
            }

            switch (client.ActiveNpc)
            {
                #region QuarantineInvite
                case 54236:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    if (Conquer_Online_Server.Game.ConquerStructures.Quarantine.Started)
                                    {
                                        dialog.Text("The quarantine tournament has now started!");
                                        dialog.Text("You will get divided into two teams, black and white, and ");
                                        dialog.Text("must eliminate the other. Only fastblade/scentsword is allowed!");
                                        dialog.Option("Let me join!", 1);
                                        dialog.Option("Not intrested", 255);
                                    }
                                    else
                                    {
                                        dialog.Text("The quarantine tournament is not active");
                                        dialog.Option("I see", 255);
                                    }
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    if (client.Equipment.Free(3))
                                    {
                                     dialog.Text("You need to wear an armor that can be colored.");
                                     dialog.Option("I see", 255);
                                     dialog.Send();
                                     break;
                                    }
                                    uint UID = 92000, ID = 0;
                                    if (Quarantine.Black.Count > Quarantine.White.Count)
                                    {
                                        Quarantine.White.Add(client.Entity.UID, client);
                                        UID += (uint)Quarantine.White.Count + (uint)Kernel.Random.Next(1,1000);
                                        client.Entity.Teleport(1844, 114, 163);
                                    }
                                    else
                                    {
                                        Quarantine.Black.Add(client.Entity.UID, client);
                                        UID += (uint)Quarantine.Black.Count + (uint)Kernel.Random.Next(1, 1000);
                                        ID = 200;
                                        client.Entity.Teleport(1844, 225, 162);
                                    }

                                    Conquer_Online_Server.Interfaces.IConquerItem newItem = new Conquer_Online_Server.Network.GamePackets.ConquerItem(true);
                                    newItem.ID = 181315 + ID;
                                    newItem.UID = UID;
                                    newItem.Durability = 1000;
                                    newItem.MaximDurability = 1000;
                                    newItem.Position = 9;
                                    client.Equipment.Remove(9);
                                    if (client.Equipment.Objects[8] != null)
                                        client.Equipment.Objects[8] = null;
                                    client.Equipment.Add(newItem);
                                    newItem.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                    newItem.Send(client);
                                    client.SendEquipment(false);
                                    client.Equipment.UpdateEntityPacket();

                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Warden
                case 10082:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("I can take you out of the jail. Just tell me.");
                                    dialog.Option("Take me out.", 1);
                                    dialog.Option("Alright.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    client.Entity.Teleport(1002, 514, 356);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Tinter
                case 10064: //Tinter
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Select the item.");
                                    dialog.Option("Headgear.", 1);
                                    dialog.Option("Armor.", 3);
                                    dialog.Option("Shield.", 5);
                                    dialog.Option("I'm standing by.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                            case 3:
                            case 5:
                                {
                                    client.TinterItemSelect = npcRequest.OptionID;
                                    dialog.Text("Select the new color.");
                                    dialog.Option("Orange.", 13);
                                    dialog.Option("Light Blue.", 14);
                                    dialog.Option("Red.", 15);
                                    dialog.Option("Blue.", 16);
                                    dialog.Option("Yellow.", 17);
                                    dialog.Option("Purple.", 18);
                                    dialog.Option("White.", 19);
                                    dialog.Option("I'm standing by.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 13:
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                                {
                                    if (client.TinterItemSelect == 0)
                                        return;
                                    IConquerItem item = client.Equipment.TryGetItem(client.TinterItemSelect);
                                    if (item == null || item.ID == 0)
                                    {
                                        dialog.Text("You don't have a item like this one equiped.");
                                        dialog.Send();
                                        break;
                                    }
                                    item.Color = (Conquer_Online_Server.Game.Enums.Color)(npcRequest.OptionID % 10);
                                    Conquer_Online_Server.Database.ConquerItemTable.UpdateColor(item, client);
                                    client.TinterItemSelect = 0;
                                    item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                                    item.Send(client);
                                    item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Default;
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Lottery

                #region JewlarLau
                case 3953:
                case 2065:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Do you have gems? I'm a great fan of them... I'd give gems with better quality for 15 gems of a lower quality. As long as I'll have many more, I don't care about the quality. I think you know, quantity not quality...ha ha! what fools, they say quality is better than quantity but I don't care.");
                                    dialog.Option("I have 15 normal gems.", 25);
                                    dialog.Option("I have 15 refined gems.", 26);
                                    dialog.Option("I'm too poor.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 25:
                            case 26:
                                {
                                    client.JewelarLauKind = npcRequest.OptionID;
                                    dialog.Text("What gems?");
                                    dialog.Option("PhoenixGem.", 1);
                                    dialog.Option("DragonGem.", 11);
                                    dialog.Option("FuryGem.", 21);
                                    dialog.Option("RainbowGem.", 31);
                                    dialog.Option("KylinGem.", 41);
                                    dialog.Option("Next.", 50);
                                    dialog.Send();
                                    break;
                                }
                            case 50:
                                {
                                    dialog.Text("What gems?");
                                    dialog.Option("VioletGem.", 51);
                                    dialog.Option("MoonGem.", 61);
                                    dialog.Option("TortoiseGem.", 71);
                                    dialog.Option("ThunderGem.", 101);
                                    dialog.Option("GloryGem.", 121);
                                    dialog.Option("Back.", client.JewelarLauKind);
                                    dialog.Send();
                                    break;
                                }
                            default:
                                {
                                    if (npcRequest.OptionID == 255)
                                        return;
                                    byte gemid = (byte)(npcRequest.OptionID + client.JewelarLauKind - 25);
                                    uint findgemid = (uint)(700000 + gemid);
                                    uint givegemid = findgemid + 1;
                                    if (client.Inventory.Contains(findgemid, 15))
                                    {
                                        client.Inventory.Remove(findgemid, 15);
                                        client.Inventory.Add(givegemid, 0, 1);
                                    }
                                    else
                                    {
                                        dialog.Text("You don't have enough " + (Conquer_Online_Server.Game.Enums.Gem)gemid + "s. Come back when you have all of them.");
                                        dialog.Option("Alright sir.", 255);
                                        dialog.Send();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region LadyLuck
                case 924:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    if (client.InLottery)
                                    {
                                        dialog.Text("You may come back later, if you still want to try your luck. It will be free next time you want.");
                                        dialog.Option("Yes please, let me out.", 1);
                                        dialog.Option("No thank you.", 255);
                                        dialog.Send();
                                    }
                                    else
                                    {
                                        dialog.Text("Thank you for trying out the lottery. Please come back any other time, we will wait you.");
                                        dialog.Option("Take me out.", 1);
                                        dialog.Send();
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    client.Entity.Teleport(1036, 219, 189);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                    #endregion

                #region Labirinth
                case 1153:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hello there. Just let me know if you want to move to the second floor of the labirinth with a SkyToken or go to TwinCity for free.");
                                    dialog.Option("Second floor.", 1);
                                    dialog.Option("TwinCity.", 2);
                                    dialog.Option("Nothing.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    if (client.Inventory.Contains(721537, 1))
                                    {
                                        client.Inventory.Remove(721537, 1);
                                        client.Entity.Teleport(1352, 027, 227);
                                    }
                                    else
                                    {
                                        dialog.Text("You don't meet the requierments.");
                                        dialog.Option("Ahh sorry.", 255);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    client.Entity.Teleport(1002, 428, 379);
                                    break;
                                }
                        }
                        break;
                    }
                case 1154:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hello there. Just let me know if you want to move to the third floor of the labirinth with a EarthToken or go to TwinCity for free.");
                                    dialog.Option("Third floor.", 1);
                                    dialog.Option("TwinCity.", 2);
                                    dialog.Option("Nothing.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    if (client.Inventory.Contains(721538, 1))
                                    {
                                        client.Inventory.Remove(721538, 1);
                                        client.Entity.Teleport(1353, 025, 268);
                                    }
                                    else
                                    {
                                        dialog.Text("You don't meet the requierments.");
                                        dialog.Option("Ahh sorry.", 255);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    client.Entity.Teleport(1002, 428, 379);
                                    break;
                                }
                        }
                        break;
                    }
                case 1155:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hello there. Just let me know if you want to move to the fourth floor of the labirinth with a SoulToken or go to TwinCity for free.");
                                    dialog.Option("Fourth floor.", 1);
                                    dialog.Option("TwinCity.", 2);
                                    dialog.Option("Nothing.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    if (client.Inventory.Contains(721539, 1))
                                    {
                                        client.Inventory.Remove(721539, 1);
                                        client.Entity.Teleport(1354, 005, 290);
                                    }
                                    else
                                    {
                                        dialog.Text("You don't meet the requierments.");
                                        dialog.Option("Ahh sorry.", 255);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    client.Entity.Teleport(1002, 428, 379);
                                    break;
                                }
                        }
                        break;
                    }
                case 1156:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hello there. Just let me know if you want to go to TwinCity.");
                                    dialog.Option("TwinCity.", 1);
                                    dialog.Option("Nothing.", 255);
                                    break;
                                }
                            case 1:
                                {
                                    client.Entity.Teleport(1002, 428, 379);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Birth Villiage

                #region RecruitMaster
                case 1060:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("You are about to enter the wonderful world of Trinity Conquer!");
                                    dialog.Text("\n\nYou will have much to learn, to survive here. As the saying goes, if you know yourself and your enemy, you will never lose a battle!");
                                    dialog.Option("Got it!", 1);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    switch (client.Entity.Class)
                                    {
                                        case 50:
                                            {
                                                dialog.Text("What the..! Oh.. It's just you, Ninja!");
                                                dialog.Text("\n\nSuch a mysterious figures, cloaked in shadow.. Ninjas have the most devastating attacks");
                                                dialog.Text(", but the lowest defence and health of all classes. Their speed and surprising skills are what make");
                                                dialog.Text("are what make them such a dangerous force here!\n\nAlways be aware of your surroundings!\n");
                                                dialog.Option("I am like a shadow!", 2);
                                                dialog.Send();
                                                break;
                                            }
                                        case 100:
                                            {
                                                dialog.Text("Well hello there, young Taoist! Right now, you don't have any specialty in your magic skills.");
                                                dialog.Text("Once you train enough, you can be promoted to a Fire or Water Taoist, and then continure on your path to glory!");
                                                dialog.Option("Fire and Water Taoist?", 11);
                                                dialog.Send();
                                                break;
                                            }


                                    }
                                    break;
                                }
                            case 11:
                                {
                                    dialog.Text("Fire Taoists are able to smite their enemies at long range, as their spells cause much more damage than other Taoists.");
                                    dialog.Text("While the Water Taoists are adept at healing and reviving fallen comrades. Both paths offer their own choices.");
                                    dialog.Text("It's up to you to decide where your heart lies.");
                                    dialog.Option("I see.", 2);
                                    dialog.Send();
                                    break;
                                }
                            case 2:
                                {
                                    dialog.Text("In Conquer Online, Path Finding is efficient in helping you locate shops and places of intrest. ");
                                    dialog.Text("You can check the current map's list by clicking the \"Path Finding\" button to the right. ");
                                    dialog.Text("Just highlight where you would like to go and click, and you're on your way!");
                                    dialog.Option("Anything else?", 3);
                                    dialog.Send();
                                    break;
                                }
                            case 3:
                                {
                                    dialog.Text("If you click \"Status\" in the lower right corner of the screen you will be able to check your character level, status, and attributes.");
                                    dialog.Option("I see.", 4);
                                    dialog.Send();
                                    break;
                                }
                            case 4:
                                {
                                    dialog.Text("You can also check your equipment, weapon proficiency, skills, and skill levels by click the tabs on the left side of the Status window.");
                                    if (client.Entity.Class == 100)
                                    { dialog.Option("Got it!", 55); }
                                    else { dialog.Option("Got it!", 5); }
                                    dialog.Send();
                                    break;
                                }
                            case 5:
                                {
                                    dialog.Text("Now you should go speak with Old General Yang.\n I'm sure he will be able to teach you some more useful tips!");
                                    dialog.Option("Okay, thanks!", 6);
                                    dialog.Send();
                                    break;
                                }
                            case 55:
                                {
                                    dialog.Text("Now you should go speak with Taoist Star.\n I'm sure he will be able to teach you some more useful tips!");
                                    dialog.Option("Okay, thanks!", 66);
                                    dialog.Send();
                                    break;
                                }
                            case 6:
                                {
                                    new Message("You should follow the Recruit Master's advice!\nGo find Old General Yang.", System.Drawing.Color.Green, Message.PopUP);
                                    break;
                                }
                            case 66:
                                {
                                    new Message("You should follow the Recruit Master's advice!\nGo find Taoist Star.", System.Drawing.Color.Green, Message.PopUP);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Pharmacist
                case 10008:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hi! I sell all sorts of potions and town scrolls in all the citys.\nI also sell fireworks and skill books in the market.");
                                    dialog.Option("What potions?", 1);
                                    dialog.Option("Consult others.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    dialog.Text("Healing and mana potions.\nHealing potions make you healthy.\nMana potions will enable you to cast spells, If you have any.\n");
                                    dialog.Text("\nThat is all. If you have not talked to others NPCs...\nYou had better have a chat with them so that you can learn more.\n");
                                    dialog.Option("I see. Thanks.", 255);
                                    dialog.Send();
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region WiseGuy
                case 10004:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Well, if you want to talk to any NPCs, just click them. Easy, huh?");
                                    dialog.Option("About my class...", 1);
                                    dialog.Option("Ah, Forget about it.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    switch (client.Entity.Class)
                                    {
                                        case 50:
                                            {
                                                dialog.Text("The Ninja is an expert in various oriental weapons, boasting swift and powerful melee attack skills. I know somebody. In Twin City there are merchants I know. you go talk to them, they'll sort you out. Got it?");
                                                break;
                                            }
                                        case 10:
                                            {
                                                dialog.Text("The Trojan is an expert in various oriental weapons, dual wield and a powerful melee attack skills. I know somebody. In Twin City there are merchants I know. you go talk to them, they'll sort you out. Got it?");
                                                break;
                                            }
                                        case 40:
                                            {
                                                dialog.Text("The Archer is an expert in Ranged oriental weapons, Ppowerful Ranged attack skills. I know somebody. In Twin City there are merchants I know. you go talk to them, they'll sort you out. Got it?");
                                                break;
                                            }
                                        case 20:
                                            {
                                                dialog.Text("The Warrior is an expert in various oriental weapons, two handed weapons, Sheald and a powerful melee attack skills. I know somebody. In Twin City there are merchants I know. you go talk to them, they'll sort you out. Got it?");
                                                break;
                                            }
                                        case 100:
                                            {
                                                dialog.Text("The Taoist is an expert in various Magic Spells, powerful melee Spells and skills. I know somebody. In Twin City there are merchants I know. you go talk to them, they'll sort you out. Got it?");
                                                break;
                                            }
                                    }
                                    dialog.Option("I see, thanks.", 0);
                                    dialog.Send();
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region VilliageIdiot
                case 10009:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hello, new blood! Welcome! It is good that you selected This class!");
                                    dialog.Option("Thank you.", 1);
                                    dialog.Option("Not a chance. Leave me be!", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    switch (client.Entity.Class)
                                    {
                                        case 10:
                                            {
                                                dialog.Text("Some handy weapons are essential for Trojans. But you'd never do that, Cause your're my friend now, right?");
                                                break;
                                            }
                                        case 40:
                                            {
                                                dialog.Text("Some handy weapons are essential for Archers. But you'd never do that, Cause your're my friend now, right?");
                                                break;
                                            }
                                        case 20:
                                            {
                                                dialog.Text("Some handy weapons are essential for Warriors. But you'd never do that, Cause your're my friend now, right?");
                                                break;
                                            }
                                        case 100:
                                            {
                                                dialog.Text("Some handy weapons are essential for Taoists. But you'd never do that, Cause your're my friend now, right?");
                                                break;
                                            }
                                        case 50:
                                            {
                                                dialog.Text("Some handy weapons are essential for Ninjas. But you'd never do that, Cause your're my friend now, right?");
                                                break;
                                            }
                                    }
                                    dialog.Option("Sure.. I'll be back in my next life.", 255);
                                    dialog.Send();
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Armorer
                case 10007:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Armorers swear only to provide the very best in defense.\nWhether it's robes or helmets, we have it all.");
                                    dialog.Option("I see. Where can I find an armorer?", 1);
                                    dialog.Option("Consult others.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {

                                    dialog.Text("Every city or major settlement has one.\nBut our armors very best from place to place.\nSome armorers have better helms, while others specialize in mail or helms.\n");
                                    dialog.Text("Just to be sure to buy armor without red text in the description.\nYou won't be able to wear those!\n");
                                    dialog.Option("I see. Thanks.", 255);
                                    dialog.Send();
                                    break;

                                }
                        }
                        break;
                    }
                #endregion

                #region Warehouseman
                case 10006:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Welcome! I run a warehouse in every city.\nYou can store your money and items in my warehouses, and retrieve them for free.");
                                    dialog.Option("Cool. How do I use them?", 1);
                                    dialog.Option("Consult others.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {

                                    dialog.Text("First, you find my warehouse men in your city..\nClick on him to see your storage box open up.\nTo deposit money, enter the amount in the bank field before clicking Deposit.\n");
                                    dialog.Text("Withdrawing money is the same.\n \nIf you want to put items in your storage space, just drag and drop.\nTaking them out only requires one click.\nAlso, your money is totalled from all warehouses.\n");
                                    dialog.Option("I see. Thanks.", 255);
                                    dialog.Send();
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Blacksmith
                case 10005:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("We blacksmiths can be found from North to South, East to West.\nWe promise a fine selection of weapons for you to purchase!");
                                    dialog.Option("Tell me more.", 1);
                                    dialog.Option("Consult others.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {

                                    dialog.Text("Let me tell you how to choose a good weapon.\nHover your mouse pointer over the weapon, check its stats and find a weapon with all white writing.\nIf there's red text, you won't be able to use it.");
                                    dialog.Text("After you buy it, you open your backpack, find the right item and right-click it to equip.\nIt's as easy as that!\n \nOh, and to sell an item, you just drag and drop to my shop!\n");
                                    dialog.Option("What about repairs?", 2);
                                    dialog.Option("Consult others.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 2:
                                {

                                    dialog.Text("First, unequip the item by double-clicking it in your status window.\nThen find me, click my repair button and click the item.");
                                    dialog.Option("Any other tips?", 3);
                                    dialog.Option("Consult others.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 3:
                                {

                                    dialog.Text("Yeah, some items are high quality: Normal, Refined, Unique, Elite, Super.\nThat's specialist stuff, only found in the market.\nThat is all. If you have not talked to other NPCs...\nYou had better have a chat with them so that you can learn more.");
                                    dialog.Option("Thanks.", 255);
                                    dialog.Send();
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region GateMan
                case 10010:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Good day to you" + client.Entity.Name + "! I'm the gateman of the Birth Village.\nIs there any way I can help you?");
                                    dialog.Option("I'm ready to start the adventure.", 1);
                                    dialog.Option("I'm beyond help...", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    dialog.Text("Welcome to the world of Trinity Conquer Online!\nThe road to becoming a legend is both long and bumpy. You should work very hard to become great! Here is a quest for you. Please take this message to Kungfu Boy in Twin City. You will get a reward for your help.");
                                    dialog.Option("As you wish.", 2);
                                    dialog.Option("I need more advice.", 3);
                                    dialog.Option("I'll stay here for a while.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 2:
                                {
                                    client.Entity.Teleport(1002, 140, 390);
                                    client.Send(new Message("You've recived a present from the gateman and you agreed to take the message to Kungfu Boy in Twin City for him.", System.Drawing.Color.Green, Message.PopUP));
                                    break;


                                }
                            case 3:
                                {
                                    dialog.Text("You will soon be in Twin City. I advise you not to stray too far from the city gates. Don't leave the Wind Plains until you are truly ready. You wouldn't even cross the river until you've visited");
                                    dialog.Text("the job center for your first promotion. Open your backpack and drag some potion down into the quick slot bar. You will be able to use F1-F10 keys to use things you place in the quick slots. They should help you stay alive.");
                                    dialog.Option("Thanks.", 255);
                                    dialog.Send();
                                    break;
                                }
                        }
                        break;
                    }

                #endregion

                #region OldGeneralYang
                case 425:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Oh, I so long for the glory of the field, but I'm too old to go back out and deal with these youngsters...\n");
                                    dialog.Text("I see a spark in your eye that reminds me of my younger days... What can I do for you young one?");
                                    dialog.Option("I'm ready for Initiation.", 1);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    dialog.Text("It would be wise for you to learn alittle more about Conquer before you decide to head out on your own.");
                                    dialog.Option("Fill me in.", 2);
                                    dialog.Send();
                                    break;
                                }
                            case 2:
                                {
                                    dialog.Text("There are two kinds of skills in Conquer.\nOne is the active skills, which are activated through the use of your magic or stamina.");
                                    dialog.Option("And the other?", 3);
                                    dialog.Send();
                                    break;
                                }
                            case 3:
                                {
                                    dialog.Text("The other is known as a passive skill.\nPassive skills will automaticly activate if the situation warrants the passive skills use.");
                                    dialog.Option("Got it!", 4);
                                    dialog.Send();
                                    break;
                                }
                            case 4:
                                {
                                    dialog.Text("Well that is all I can tell you for today. It's time to eat!\nYou can talk to the Village Gateman when you are ready to get started.");
                                    dialog.Option("Okay", 255);
                                    dialog.Send();
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #endregion

                #region OUT
                case 9988:
                case 9989:
                case 9990:
                case 9991:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("In exchange of a fee of 500gold, I will teleport you back to the guild war map. Don't bother me if you are poor.");
                                    dialog.Option("Teleport me.", 1);
                                    dialog.Option("I'm too poor.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    if (client.Entity.Money >= 500)
                                    {
                                        client.Entity.Money -= 500;
                                        client.Entity.Teleport(1038, 348, 339);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Warehousemen
                case 8:
                case 10012:
                case 10028:
                case 10011:
                case 10027:
                case 4101:
                case 44:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    INpc npc = null;
                                    if (client.Map.Npcs.TryGetValue(client.ActiveNpc, out npc))
                                    {
                                        if (client.WarehousePW == "")
                                            client.Send(new Message("To protect your items that are stored in warehouse, you should set an password at WHSGuardian.", System.Drawing.Color.Maroon, Message.TopLeft));
                                        else
                                        {
                                            if (!client.WarehouseOpen)
                                            {
                                                dialog.Text("Please tell me the password of your warehouse so I can open it. It's like a key, if you don't have it I can't do anything.");
                                                dialog.Input("Here:", 1, 8);
                                                dialog.Option("Nevermind.", 255);
                                                dialog.Send();
                                                return;
                                            }
                                        }
                                        Data data = new Data(true);
                                        data.ID = Data.OpenWindow;
                                        data.UID = client.Entity.UID;
                                        data.TimeStamp = Time32.Now;
                                        data.dwParam = Data.WindowCommands.Warehouse;
                                        data.wParam1 = npc.X;
                                        data.wParam2 = npc.Y;
                                        client.Send(data);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (client.WarehouseOpenTries < 3)
                                    {
                                        client.WarehouseOpenTries++;
                                        if (npcRequest.Input == client.WarehousePW)
                                        {
                                            INpc npc = null;
                                            if (client.Map.Npcs.TryGetValue(client.ActiveNpc, out npc))
                                            {
                                                client.WarehouseOpen = true;
                                                Data data = new Data(true);
                                                data.ID = Data.OpenWindow;
                                                data.UID = client.Entity.UID;
                                                data.TimeStamp = Time32.Now;
                                                data.dwParam = 4;
                                                data.wParam1 = npc.X;
                                                data.wParam2 = npc.Y;
                                                client.Send(data);
                                            }
                                        }
                                        else
                                        {
                                            dialog.Text("Wrong password. Tries left: " + client.WarehouseOpenTries + ". Try again?");
                                            dialog.Input("Here:", 1, 8);
                                            dialog.Option("Alright.", 255);
                                            dialog.Send();
                                        }
                                    }
                                    else
                                    {
                                        dialog.Text("You can try no more to open your warehouse. Come back later.");
                                        dialog.Option("Nevermind.", 255);
                                        dialog.Send();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Boxers
                case 180:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("If you want to leave just tell me when you are ready. It's free.\nYou will be teleported to the city you were in before coming here.");
                                    dialog.Option("I'm ready.", 1);
                                    dialog.Option("Wait a minute.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    int PrevMap = client.Entity.PreviousMapID;
                                    switch (PrevMap)
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
                                        case 1015:
                                            {
                                                client.Entity.Teleport(1015, 717, 571);
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 181:
                case 182:
                case 183:
                case 184:
                case 185:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hello " + client.Entity.Name + ", if your level is higher than 19, I can send you to a training ground in exchange of 1000 silvers. Be aware of the fact that you can't attack dumes with level higher than yours.");
                                    dialog.Option("Alright, let me in.", 1);
                                    dialog.Option("Nevermind", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    if (client.Entity.Level >= 20)
                                    {
                                        if (client.Entity.Money >= 1000)
                                        {
                                            client.Entity.Money -= 1000;
                                            client.Entity.Teleport(1039, 216, 214);
                                        }
                                        else
                                        {
                                            dialog.Text("You do not have 1000 silvers.");
                                            dialog.Option("Aww!", 255);
                                            dialog.Send();
                                        }
                                    }
                                    else
                                    {
                                        dialog.Text("Your level is not high enough.");
                                        dialog.Option("Aww!", 255);
                                        dialog.Send();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Guild War

                #region Jail warden
                case 140:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    if (Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.IsWar)
                                    {
                                        if (Time32.Now <= Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LastWin.AddMinutes(5))
                                        {
                                            dialog.Text("My friend, you may leave if you want.");
                                            dialog.Option("Yes please.", 1);
                                            dialog.Option("No need...", 255);
                                            dialog.Send();

                                        }
                                        else
                                        {
                                            dialog.Text("You lost your change. Now wait for the next pardon!");
                                            dialog.Option("No!!!", 255);
                                            dialog.Send();
                                        }
                                    }
                                    else
                                    {
                                        dialog.Text("My friend, you may leave if you want.");
                                        dialog.Option("Yes please.", 1);
                                        dialog.Option("No need...", 255);
                                        dialog.Send();
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.IsWar)
                                    {
                                        if (Time32.Now <= Conquer_Online_Server.Game.ConquerStructures.Society.GuildWar.LastWin.AddMinutes(5))
                                        {
                                            client.Entity.Teleport(1002, 430, 380);
                                        }
                                        else
                                        {
                                            dialog.Text("You lost your change. Now wait for the next pardon!");
                                            dialog.Option("No!!!", 255);
                                            dialog.Send();
                                        }
                                    }
                                    else
                                    {
                                        client.Entity.Teleport(1002, 430, 380);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                #endregion

                #region Light up runes
                case 4452:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Hello friend, as you might know, while guild war, you can light up some runes and after guild war is over, you can come to me and I will give you experience in exchange of your rune.");
                                    dialog.Text("Once guild war starts, come to me and ask for a rune, and you will receive it. If you lose it, you can come back and reclaim it, but you will start from level 1 once again.");
                                    dialog.Option("Give me a rune.", 1);
                                    dialog.Option("Claim experience.", 2);
                                    dialog.Option("Nothing.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    if (GuildWar.IsWar)
                                    {
                                        for (int c = 0; c <= 10; c++)
                                        {
                                            if (client.Inventory.Contains((uint)(729960 + c), 1))
                                            {
                                                dialog.Text("You already have a rune.");
                                                dialog.Option("Thank you.", 255);
                                                dialog.Send();
                                                return;
                                            }
                                        }
                                        if (client.Inventory.Add(729960, 0, 1))
                                        {
                                            dialog.Text("Go, and light up this rune.");
                                            dialog.Option("Thank you.", 255);
                                            dialog.Send();
                                        }
                                        else
                                        {
                                            dialog.Text("There is not enough room in your inventory.");
                                            dialog.Option("Ah, hold on.", 255);
                                            dialog.Send();
                                        }
                                    }
                                    else
                                    {
                                        dialog.Text("I cannot give you a rune now.");
                                        dialog.Option("Ahh.", 255);
                                        dialog.Send();
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (!GuildWar.IsWar)
                                    {
                                        for (int c = 0; c <= 10; c++)
                                        {
                                            if (client.Inventory.Contains((uint)(729960 + c), 1))
                                            {
                                                int expballs = c;
                                                if (729960 + c == 729970)
                                                    expballs += 2;

                                                for (int ex = 0; ex < expballs; ex++)
                                                    client.IncreaseExperience(client.ExpBall, false);
                                                client.Inventory.Remove((uint)(729960 + c), 1);
                                                return;
                                            }
                                        }
                                        dialog.Text("You cannot claim experience if you don't have a rune.");
                                        dialog.Option("Ahh.", 255);
                                        dialog.Send();
                                    }
                                    else
                                    {

                                    }
                                    {
                                        dialog.Text("You cannot claim experience while guild war.");
                                        dialog.Option("Ahh.", 255);
                                        dialog.Send();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 4453:
                case 4454:
                case 4455:
                case 4456:
                case 4457:
                case 4458:
                case 4459:
                case 4460:
                case 4461:
                    {
                        dialog.Avatar(0);
                        uint takeFlame = 725507 + client.ActiveNpc;
                        uint addFlame = 725507 + client.ActiveNpc + 1;
                        if (GuildWar.IsWar)
                        {
                            if (client.Inventory.Contains(takeFlame, 1))
                            {
                                client.Inventory.Remove(takeFlame, 1);
                                client.Inventory.Add(addFlame, 0, 1);
                                dialog.Text("Well done! Next rune is number " + (client.ActiveNpc - 4451) + ".");
                                dialog.Send();
                            }
                            else
                            {
                                dialog.Text("You cannot flame up this stone without the proper rune.");
                                dialog.Send();
                            }
                        }
                        else
                        {
                            dialog.Text("You cannot flame up a rune if guild war is not on.");
                            dialog.Send();
                        }
                        break;
                    }
                case 4462:
                    {
                        dialog.Avatar(0);
                        uint takeFlame = 725507 + client.ActiveNpc;
                        uint addFlame = 725507 + client.ActiveNpc + 1;
                        if (GuildWar.IsWar)
                        {
                            if (GuildWar.Flame10th)
                            {
                                if (client.Inventory.Contains(takeFlame, 1))
                                {
                                    client.Inventory.Remove(takeFlame, 1);
                                    client.Inventory.Add(addFlame, 0, 1);
                                    dialog.Text("Well done! Nothing left to light up. Congratulations!");
                                    dialog.Send();
                                }
                                else
                                {
                                    dialog.Text("You cannot flame up this stone without the proper rune.");
                                    dialog.Send();
                                }
                            }
                            else
                            {
                                dialog.Text("It's not the right time to flame up this rune.");
                                dialog.Send();
                            }
                        }
                    }
                    break;
                #endregion

                #endregion

                #region OfflineTG
                case 3836:
                    {
                        OfflineTGStats sts = new OfflineTGStats(true);
                        var T1 = new TimeSpan(DateTime.Now.Ticks);
                        var T2 = new TimeSpan(client.OfflineTGEnterTime.Ticks);
                        ushort minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                        minutes = (ushort)Math.Min((ushort)900, minutes);
                        sts.TotalTrainingMinutesLeft = (ushort)(900 - minutes);
                        sts.TrainedMinutes = minutes;
                        ulong exp = client.Entity.Experience;
                        byte level = client.Entity.Level;
                        double expballGain = (double)300 * (double)minutes / (double)900;
                        while (expballGain >= 100)
                        {
                            expballGain -= 100;
                            exp += client.ExpBall;
                        }
                        if (expballGain != 0)
                            exp += (uint)(client.ExpBall * (expballGain / 100));

                        while (exp >= Conquer_Online_Server.Database.DataHolder.LevelExperience(level))
                        {
                            exp -= Conquer_Online_Server.Database.DataHolder.LevelExperience(level);
                            level++;
                        }
                        double percent = (double)exp * (double)100 / (double)Conquer_Online_Server.Database.DataHolder.LevelExperience(level);

                        sts.Character_NewExp = (ulong)(percent * 100000);
                        sts.Character_AcquiredLevel = level;
                        sts.Send(client);
                        break;
                    }
                #endregion

                #region Adventure Zone
                #region Adv zone teleporter
                case 300655:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("I have just discovered a new place, where the turtle doves are so big, that they drop awesome items when they die. It's free, do you want to see it?");
                                    dialog.Option("Yes please.", 1);
                                    dialog.Option("No thank you.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    client.Entity.Teleport(1210, 1029, 714);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Adv zone mine supervisor
                case 300652:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("In this mine, the minerals are very good. They are really expensive, would you like to enter?");
                                    dialog.Option("Yes please.", 1);
                                    dialog.Option("No thank you.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    client.Entity.Teleport(1218, 30, 69);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Grandpa
                case 300653:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Would you like to advance?");
                                    dialog.Option("Yes.", 1);
                                    dialog.Option("No.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    client.Entity.Teleport(1219, 448, 272);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region Boatman
                case 300654:
                    {
                        switch (npcRequest.OptionID)
                        {
                            case 0:
                                {
                                    dialog.Text("Would you like to go in TwinCity?");
                                    dialog.Option("Yes.", 1);
                                    dialog.Option("No.", 255);
                                    dialog.Send();
                                    break;
                                }
                            case 1:
                                {
                                    client.Entity.Teleport(1002, 430, 380);
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #endregion

                default:
                    {
                        if (client.Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.GameMaster || client.Account.State == Conquer_Online_Server.Database.AccountTable.AccountState.ProjectManager)
                            client.Send(new Message("NpcID[" + client.ActiveNpc + "]", System.Drawing.Color.Red, Message.TopLeft));
                        break;
                    }
            }

            if (!dialog.Sent)
                if (dialog.Replies.Count > 1)
                    dialog.Send();
        }
    }
}
