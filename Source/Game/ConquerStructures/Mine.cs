using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Mining
    {
        public static void Mine(Client.GameState client)
        {
            if (client.Equipment.Free(4))
            {
                client.Mining = false;
                return;
            }
            var item = client.Equipment.TryGetItem(4);
            var info = Database.ConquerItemInformation.BaseInformations[item.ID];
            Data data = new Data(true);
            data.ID = Data.SwingPickaxe;
            data.UID = client.Entity.UID;
            client.SendScreen(data, true);
            if (info == null)
            {
                client.Mining = false;
                return;
            }
            if (info.Name != "PickAxe" && info.Name != "Hoe")
            {
                client.Mining = false;
                return;
            }
            if (!ServerBase.Kernel.Rate(40))
            {
                return;
            }
            switch (client.Entity.MapID)
            {
                case 1218://meteor zone mine
                case 6001://jail war mine
                case 6000://jails
                    {
                        Mine(700001, 700011, 700031, 700041, 1072010, 1072020, 1072050, 0, client);
                        break;
                    }
                case 1025://phoenixcity minecave
                case 1028://twincity minecave
                    {
                        Mine(700011, 700001, 700021, 700071, 1072010, 1072050, 1072031, 0, client);
                        break;
                    }
                case 1027://DesertMine
                case 1026://ApeMine
                    {
                        Mine(700051, 700061, 0, 0, 1072010, 1072020, 1072040, 1072050, client);
                        break;
                    }
                default:
                    {
                        client.Send(new Message("You cannot mine here. You must go inside a mine.", System.Drawing.Color.Red, Message.TopLeft));
                        client.Mining = false;
                        break;
                    }
            }
        }
        static void Mine(uint GemID, uint GemID2, uint GemID3, uint GemID4, uint Ore1, uint Ore2, uint Ore3, uint Ore4, Client.GameState client)
        {
            if (client.Inventory.Count <= 39)
            {
                if (Ore1 != 0 && ServerBase.Kernel.Rate(30))//ores type 1
                {
                    if (Ore1 != 1072031) { Ore1 += (uint)ServerBase.Kernel.Random.Next(0, 9); }

                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(Ore1))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)Ore1;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[Ore1].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
                if (Ore2 != 0 && ServerBase.Kernel.Rate(20))//ores type 2
                {
                    if (Ore2 != 1072031) { Ore2 += (uint)ServerBase.Kernel.Random.Next(0, 9); }

                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(Ore2))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)Ore2;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[Ore2].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
                if (Ore3 != 0 && ServerBase.Kernel.Rate(25))//ores type 3
                {
                    if (Ore3 != 1072031) { Ore3 += (uint)ServerBase.Kernel.Random.Next(0, 9); }

                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(Ore3))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)Ore3;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[Ore3].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
                if (Ore4 != 0 && ServerBase.Kernel.Rate(30))//ores type 4
                {
                    if (Ore4 != 1072031) { Ore4 += (uint)ServerBase.Kernel.Random.Next(0, 9); }

                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(Ore4))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)Ore3;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[Ore4].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
                if (GemID != 0 && ServerBase.Kernel.Rate(5))
                {
                    if (ServerBase.Kernel.Rate(1, 250))
                        GemID4 += 1;
                    else if (ServerBase.Kernel.Rate(1, 100))
                        GemID4 += 2;
                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(GemID))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)GemID;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[GemID].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
                if (GemID2 != 0 && ServerBase.Kernel.Rate(5))
                {
                    if (ServerBase.Kernel.Rate(1, 250))
                        GemID4 += 1;
                    else if (ServerBase.Kernel.Rate(1, 100))
                        GemID4 += 2;
                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(GemID2))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)GemID2;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[GemID2].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
                if (GemID3 != 0 && ServerBase.Kernel.Rate(5))
                {
                    if (ServerBase.Kernel.Rate(1, 250))
                        GemID4 += 1;
                    else if (ServerBase.Kernel.Rate(1, 100))
                        GemID4 += 2;
                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(GemID3))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)GemID3;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[GemID3].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
                if (GemID4 != 0 && ServerBase.Kernel.Rate(5))
                {
                    if (ServerBase.Kernel.Rate(1, 250))
                        GemID4 += 1;
                    else if (ServerBase.Kernel.Rate(1, 100))
                        GemID4 += 2;
                    if (Database.ConquerItemInformation.BaseInformations.ContainsKey(GemID4))
                    {
                        var Item = new Network.GamePackets.ConquerItem(true);
                        Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        Item.ID = (uint)GemID4;
                        Item.MaximDurability = 1;
                        Item.Durability = 1;
                        client.Inventory.Add(Item, Enums.ItemUse.CreateAndAdd);
                        client.Send(new Message("You have gained a " + Database.ConquerItemInformation.BaseInformations[GemID4].Name + ".", System.Drawing.Color.Red, Message.TopLeft));
                        return;
                    }
                }
            }
        }
    }
}
