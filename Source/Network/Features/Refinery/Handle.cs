using System;
using Conquer_Online_Server.Network.GamePackets;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Drawing;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game.Features.Refinery
{
    public class Handle
    {
        public Handle(byte[] packet, Receivers.ClientState client)
        {
            #region Variables
            byte Type = packet[4];
            uint ItemUID = BitConverter.ToUInt32(packet, 8);
            uint RefineryUID = BitConverter.ToUInt32(packet, 12);
            Interfaces.IConquerItem Main = null;
            Interfaces.IConquerItem Minor = null;
            #endregion

            #region Refinery
            if (Type == 0)
            {
                #region Main Item
                Main = client.Equipment.TryGetItem(ItemUID);
                if (Main == null)
                {
                    client.Send(new Message("You need to be wearing the target item to refine", Color.Red, Message.TopLeft));
                    return;
                }
                #endregion
                #region Minor Item
                client.Inventory.TryGetItem(RefineryUID, out Minor);
                #endregion

                if (Minor != null)
                {
                    ConquerItemInformation Info = new ConquerItemInformation(Minor.ID, 0);
                    if (Info != null)
                    {
                        #region Info Split
                        string[] RefineryInfo = Info.BaseInformation.FinalDescription.Split('~');
                        #endregion
                        #region Safe RefineryName
                        string RefineryName = Info.BaseInformation.Name.Replace("Material", "");
                        RefineryName = RefineryName.Replace("(Super)", "");
                        RefineryName = RefineryName.Replace("(Unique)", "");
                        RefineryName = RefineryName.Replace("(Refined)", "");
                        RefineryName = RefineryName.Replace("(Elite)", "");
                        RefineryName = RefineryName.Replace("(Normal)", "");
                        #endregion
                        #region Set Info
                        RefineryID RefineryType = RefineryID.None;
                        byte RefineryLevel = byte.Parse(RefineryInfo[1]);
                        string ItemType = Info.BaseInformation.Description;
                        ushort RefineryPercent = 0;

                        switch (RefineryName)
                        {
                            case "Block": RefineryType = RefineryID.Block; break;
                            case "Detoxication": RefineryType = RefineryID.Detoxication; break;
                            case "Breakthrough": RefineryType = RefineryID.Breaktrough; break;
                            case "CriticalStrike": RefineryType = RefineryID.Critical; break;
                            case "Intensification": RefineryType = RefineryID.Intensificaiton; break;
                            default: Console.WriteLine("Uknown refinery item " + RefineryName); return;
                        }
                        RefineryPercent = getPercentage(RefineryName, ItemType, RefineryLevel);
                        if (RefineryPercent <= 0)
                            return;

                        #endregion
                        #region Switch RefineryType
                        #region Allow Refinery
                        bool AllowedRefinery = false;
                        switch (ItemType)
                        {
                            case "Headgear":
                                if (Main.Position == 1)
                                    AllowedRefinery = true;
                                break;
                            case "Necklace":
                                if (Main.Position == 2)
                                    AllowedRefinery = true;
                                break;
                            case "Armor":
                                if (Main.Position == 3)
                                    AllowedRefinery = true;
                                break;
                            case "Bracelet":
                            case "Ring":
                                if (Main.Position == 6)
                                    AllowedRefinery = true;
                                break;
                            case "1-Handed":
                                if (Main.Position == 4 || Main.Position == 5)
                                    if (Main.ID >= 410000 && Main.ID < 500000)
                                        AllowedRefinery = true;
                                break;
                            case "2-Handed":
                                if (Main.Position == 4)
                                    if (Main.ID >= 510000 && Main.ID < 600000)
                                        AllowedRefinery = true;
                                break;
                            case "Bow":
                                if (Main.Position == 4)
                                    if (Main.ID >= 500000 && Main.ID < 501000)
                                        AllowedRefinery = true;
                                break;
                            case "Shield":
                                if (Main.Position == 5)
                                    if (Main.ID >= 900000 && Main.ID < 901000)
                                        AllowedRefinery = true;
                                break;
                            case "Boots":
                                if (Main.Position == 8)
                                    AllowedRefinery = true;
                                break;
                            default: AllowedRefinery = false; break;
                        }

                        if (!AllowedRefinery)
                        {
                            client.Send(new Message("You can't refine the target item!", Color.Red, Message.TopLeft));
                            return;
                        }
                        #endregion

                        if (RefineryType != RefineryID.None)
                        {
                            Main.RefineryLevel = RefineryLevel;
                            Main.RefineryPart = (uint)RefineryType;
                            Main.RefineryPercent = RefineryPercent;
                            Main.Send(client);
                            ConquerItemTable.RefineryUpdate(Main, client);
                            client.Inventory.Remove(Minor, Enums.ItemUse.Remove);
                        }
                        #endregion
                    }
                    else
                    {
                        client.Send(new Message("You can't use this item to refine!", Color.Red, Message.TopLeft));
                        Console.WriteLine("Minor refinery info is null");
                        return;
                    }
                }
                else
                {
                    client.Send(new Message("You can't use this item to refine!", Color.Red, Message.TopLeft));
                    Console.WriteLine("Minor refinery is null");
                    return;
                }
            }
            #endregion
            #region Purify
            else if (Type == 1)
            {
                client.Inventory.TryGetItem(ItemUID, out Main);
                client.Inventory.TryGetItem(RefineryUID, out Minor);
                if (Main == null)
                    return;
                if (Minor == null)
                    return;

                if (client.Inventory.Contains(1088001, 15))
                {
                    Main.Mode = Enums.ItemMode.Update;
                    Main.SoulPart = Minor.ID;
                    Main.SoulStarted = DateTime.Now;
                    Main.Send(client);
                    Main.Mode = Enums.ItemMode.Default;
                    ConquerItemTable.SoulUpdate(Main, client);
                    client.Inventory.Remove(Minor, Enums.ItemUse.Remove);
                    client.Inventory.Remove(1088001, 15);
                    client.Send(packet);
                }
                else { client.Send(new Message("You don't have 15 meteors!", Color.Red, Message.TopLeft)); return; }
            }
           
            #endregion
            
        }
       
        #region Percentage
        public ushort getPercentage(string Name, string Type, byte Level)
        {
            ushort final = 0;
            switch (Name)
            {
                #region Block
                case "Block":
                    switch (Type)
                    {
                        #region Shield
                        case "Shield":
                            if (Level == 1)
                                final = 6;
                            if (Level == 2)
                                final = 8;
                            if (Level == 3)
                                final = 10;
                            if (Level == 4)
                                final = 12;
                            if (Level == 5)
                                final = 15;
                            break;
                        #endregion
                        #region Headgear
                        case "Headgear":
                            if (Level == 1)
                                final = 5;
                            if (Level == 2)
                                final = 6;
                            if (Level == 3)
                                final = 7;
                            if (Level == 4)
                                final = 8;
                            if (Level == 5)
                                final = 10;
                            break;
                        #endregion
                    }
                    break;
                #endregion
                #region Detoxication
                case "Detoxication":
                    if (Level == 1)
                        final = 8;
                    if (Level == 2)
                        final = 9;
                    if (Level == 3)
                        final = 11;
                    if (Level == 4)
                        final = 13;
                    if (Level == 5)
                        final = 15;
                    break;
                #endregion
                #region Breakthrough
                case "Breakthrough":
                    switch (Type)
                    {
                        #region 1-Handed
                        case "1-Handed":
                            if (Level == 1)
                                final = 5;
                            if (Level == 2)
                                final = 6;
                            if (Level == 3)
                                final = 7;
                            if (Level == 4)
                                final = 8;
                            if (Level == 5)
                                final = 9;
                            break;
                        #endregion
                        #region 2-Handed
                        case "2-Handed":
                            if (Level == 1)
                                final = 10;
                            if (Level == 2)
                                final = 12;
                            if (Level == 3)
                                final = 14;
                            if (Level == 4)
                                final = 16;
                            if (Level == 5)
                                final = 18;
                            break;
                        #endregion
                        #region Bow
                        case "Bow":
                            if (Level == 1)
                                final = 10;
                            if (Level == 2)
                                final = 12;
                            if (Level == 3)
                                final = 14;
                            if (Level == 4)
                                final = 16;
                            if (Level == 5)
                                final = 18;
                            break;
                        #endregion
                        #region Ring
                        case "Ring":
                            if (Level == 1)
                                final = 4;
                            if (Level == 2)
                                final = 6;
                            if (Level == 3)
                                final = 8;
                            if (Level == 4)
                                final = 10;
                            if (Level == 5)
                                final = 12;
                            break;
                        #endregion
                    }
                    break;
                #endregion
                #region CriticalStrike
                case "CriticalStrike":
                    switch (Type)
                    {
                        #region 1-Handed
                        case "1-Handed":
                            if (Level == 1)
                                final = 2;
                            if (Level == 2)
                                final = 3;
                            if (Level == 3)
                                final = 4;
                            if (Level == 4)
                                final = 5;
                            if (Level == 5)
                                final = 6;
                            break;
                        #endregion
                        #region 2-Handed
                        case "2-Handed":
                            if (Level == 1)
                                final = 4;
                            if (Level == 2)
                                final = 6;
                            if (Level == 3)
                                final = 8;
                            if (Level == 4)
                                final = 10;
                            if (Level == 5)
                                final = 250;
                            break;
                        #endregion
                        #region Bow
                        case "Bow":
                            if (Level == 1)
                                final = 4;
                            if (Level == 2)
                                final = 6;
                            if (Level == 3)
                                final = 8;
                            if (Level == 4)
                                final = 10;
                            if (Level == 5)
                                final = 12;
                            break;
                        #endregion
                        #region Ring
                        case "Ring":
                            if (Level == 1)
                                final = 2;
                            if (Level == 2)
                                final = 3;
                            if (Level == 3)
                                final = 4;
                            if (Level == 4)
                                final = 5;
                            if (Level == 5)
                                final = 6;
                            break;
                        #endregion
                    }
                    break;
                #endregion
                #region Intensification
                case "Intensification":
                    switch (Type)
                    {
                        #region Headgear
                        case "Headgear":
                            if (Level == 1)
                                final = 200;
                            if (Level == 2)
                                final = 500;
                            if (Level == 3)
                                final = 1000;
                            if (Level == 4)
                                final = 1600;
                            if (Level == 5)
                                final = 2500;
                            break;
                        #endregion
                    }
                    break;
                #endregion
                default:
                    Console.WriteLine("Uknown refinery item " + Name);
                    break;
            }

            return final;
        }
        #endregion
    }
}
