using System;
using System.IO;

namespace Conquer_Online_Server.Database
{
    public class ConquerItemTable
    {

        public static Interfaces.IConquerItem GetSingleItem(uint UID)
        {
            Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("items").Where("UID", UID);
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                item.ID = r.ReadUInt32("ID");
                item.UID = r.ReadUInt32("UID");
                item.Durability = r.ReadUInt16("Durability");
                item.MaximDurability = r.ReadUInt16("MaximDurability");
                item.Position = r.ReadUInt16("Position");
                item.SocketProgress = r.ReadUInt32("SocketProgress");
                item.PlusProgress = r.ReadUInt32("PlusProgress");
                item.SocketOne = (Game.Enums.Gem)r.ReadByte("SocketOne");
                item.SocketTwo = (Game.Enums.Gem)r.ReadByte("SocketTwo");
                item.Effect = (Game.Enums.ItemEffect)r.ReadByte("Effect");
                item.Mode = Game.Enums.ItemMode.Default;
                item.Plus = r.ReadByte("Plus");
                item.Bless = r.ReadByte("Bless");
                item.Bound = r.ReadBoolean("Bound");
                item.Enchant = r.ReadByte("Enchant");
                item.Lock = r.ReadByte("Locked");
                item.UnlockEnd = DateTime.FromBinary(r.ReadInt64("UnlockEnd"));
                item.Suspicious = r.ReadBoolean("Suspicious");
                item.RefineryPart = r.ReadUInt32("RefineryPart");
                item.RefineryLevel = r.ReadUInt32("RefineryLevel");
                item.RefineryPercent = r.ReadUInt16("RefineryPercent");
                item.RefineryStarted = DateTime.FromBinary(r.ReadInt64("RefineryStarted"));
                item.SuspiciousStart = DateTime.FromBinary(r.ReadInt64("SuspiciousStart"));
                item.Color = (Game.Enums.Color)r.ReadByte("Color");
                item.Warehouse = r.ReadUInt16("Warehouse");
                if (item.Lock == 2)
                    if (DateTime.Now >= item.UnlockEnd)
                        item.Lock = 0;
            }
            r.Close();
            return item;
        }
        #region [Refinery-Update]
        public static void RefineryUpdate(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand Cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            Cmd.Update("items").Set("EntityID", client.Entity.UID)
                .Set("RefineryPart", Item.RefineryPart)
                .Set("RefineryLevel", Item.RefineryLevel)
                .Set("RefineryPercent", Item.RefineryPercent)
                .Set("RefineryStarted", Item.RefineryStarted.Ticks)
                .Where("UID", Item.UID)
                .Execute();
        }
        #endregion
        public static void LoadItems(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("items").Where("EntityID", client.Entity.UID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                item.ID = r.ReadUInt32("ID");
                item.UID = r.ReadUInt32("UID");
                item.Durability = r.ReadUInt16("Durability");
                item.MaximDurability = r.ReadUInt16("MaximDurability");

                item.Durability = item.MaximDurability;
                item.Position = r.ReadUInt16("Position");
                item.SocketProgress = r.ReadUInt32("SocketProgress");
                item.PlusProgress = r.ReadUInt32("PlusProgress");
                item.SocketOne = (Game.Enums.Gem)r.ReadByte("SocketOne");
                item.SocketTwo = (Game.Enums.Gem)r.ReadByte("SocketTwo");
                item.Effect = (Game.Enums.ItemEffect)r.ReadByte("Effect");
                item.Mode = Game.Enums.ItemMode.Default;
                item.Plus = r.ReadByte("Plus");
                item.Bless = r.ReadByte("Bless");
                item.Bound = r.ReadBoolean("Bound");
                item.Enchant = r.ReadByte("Enchant");
                item.Lock = r.ReadByte("Locked");
                item.UnlockEnd = DateTime.FromBinary(r.ReadInt64("UnlockEnd"));
                item.Suspicious = r.ReadBoolean("Suspicious");
                item.RefineryPart = r.ReadUInt32("RefineryPart");
                item.RefineryLevel = r.ReadUInt32("RefineryLevel");
                item.RefineryPercent = r.ReadUInt16("RefineryPercent");
                item.RefineryStarted = DateTime.FromBinary(r.ReadInt64("RefineryStarted"));
                item.SuspiciousStart = DateTime.FromBinary(r.ReadInt64("SuspiciousStart"));
                item.Color = (Game.Enums.Color)r.ReadByte("Color");
                item.Warehouse = r.ReadUInt16("Warehouse");
                item.Inscribed = (r.ReadByte("Inscribed") == 1 ? true : false);
                item.StackSize = r.ReadUInt16("StackSize");
                item.MaxStackSize = r.ReadUInt16("MaxStackSize");
                if (item.Lock == 2)
                    if (DateTime.Now >= item.UnlockEnd)
                        item.Lock = 0;
                ItemAddingTable.GetAddingsForItem(item);
                if (item.Warehouse == 0)
                {
                    switch (item.Position)
                    {
                        case 0: client.Inventory.Add(item, Game.Enums.ItemUse.None); break;
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                            if (client.Equipment.Free((byte)item.Position))
                            {

                                client.Equipment.Add(item, Game.Enums.ItemUse.None);
                            }
                            else
                            {

                                if (client.Inventory.Count < 40)
                                {
                                    item.Position = 0;
                                    client.Inventory.Add(item, Game.Enums.ItemUse.None);
                                    if (client.Warehouses[Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity].Count < 60)
                                        client.Warehouses[Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity].Add(item);
                                    UpdatePosition(item, client);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    client.Warehouses[(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID)item.Warehouse].Add(item);
                }


                if (item.ID == 720828)
                {
                    string agate = r.ReadString("agate");
                    uint count = 0;
                    string[] maps = agate.Split('#');
                    foreach (string one in maps)
                    {
                        if (one.Length > 6)
                        {
                            item.Agate_map.Add(count, one);
                            count++;
                        }
                    }
                }
            }
            r.Close();
        }
        public static Interfaces.IConquerItem LoadItem(uint UID)
        {
            Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("items").Where("UID", UID);
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                item.ID = r.ReadUInt32("ID");
                item.UID = r.ReadUInt32("UID");
                item.Durability = r.ReadUInt16("Durability");
                item.MaximDurability = r.ReadUInt16("MaximDurability");
                item.Position = r.ReadUInt16("Position");
                item.SocketProgress = r.ReadUInt32("SocketProgress");
                item.PlusProgress = r.ReadUInt32("PlusProgress");
                item.SocketOne = (Game.Enums.Gem)r.ReadByte("SocketOne");
                item.SocketTwo = (Game.Enums.Gem)r.ReadByte("SocketTwo");
                item.Effect = (Game.Enums.ItemEffect)r.ReadByte("Effect");
                item.Mode = Game.Enums.ItemMode.Default;
                item.Plus = r.ReadByte("Plus");
                item.Bless = r.ReadByte("Bless");
                item.Bound = r.ReadBoolean("Bound");
                item.Enchant = r.ReadByte("Enchant");
                item.Lock = r.ReadByte("Locked");
                item.UnlockEnd = DateTime.FromBinary(r.ReadInt64("UnlockEnd"));
                item.Suspicious = r.ReadBoolean("Suspicious");
                item.SuspiciousStart = DateTime.FromBinary(r.ReadInt64("SuspiciousStart"));
                item.Color = (Game.Enums.Color)r.ReadByte("Color");
                item.Warehouse = r.ReadUInt16("Warehouse");
                if (item.Lock == 2)
                    if (DateTime.Now >= item.UnlockEnd)
                        item.Lock = 0;
                ItemAddingTable.GetAddingsForItem(item);
            }
            r.Close();
            return item;
        }
        public static void AddItem(ref Interfaces.IConquerItem Item, Client.GameState client)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("items").Insert("EntityID", client.Entity.UID).Insert("UID", Item.UID)
                    .Insert("ID", Item.ID).Insert("Plus", Item.Plus).Insert("Bless", Item.Bless)
                    .Insert("Enchant", Item.Enchant).Insert("SocketOne", (byte)Item.SocketOne)
                    .Insert("SocketTwo", (byte)Item.SocketTwo).Insert("Durability", Item.Durability)
                    .Insert("MaximDurability", Item.MaximDurability).Insert("SocketProgress", Item.SocketProgress)
                    .Insert("PlusProgress", Item.PlusProgress).Insert("Effect", (ushort)Item.Effect)
                    .Insert("Bound", Item.Bound).Insert("Locked", Item.Lock).Insert("Suspicious", Item.Suspicious)
                    .Insert("Color", (uint)Item.Color).Insert("Position", Item.Position).Insert("Warehouse", Item.Warehouse)
                    .Insert("UnlockEnd", Item.UnlockEnd.ToBinary()).Insert("SuspiciousStart", Item.SuspiciousStart.ToBinary())
                    .Insert("StackSize", Item.StackSize).Insert("MaxStackSize", Item.MaxStackSize);
                cmd.Execute();
            }
            catch
            {
            again:
                Item.UID = Conquer_Online_Server.Network.GamePackets.ConquerItem.ItemUID.Next;
                if (IsThere(Item.UID))
                    goto again;
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("items").Insert("EntityID", client.Entity.UID).Insert("UID", Item.UID)
                    .Insert("ID", Item.ID).Insert("Plus", Item.Plus).Insert("Bless", Item.Bless)
                    .Insert("Enchant", Item.Enchant).Insert("SocketOne", (byte)Item.SocketOne)
                    .Insert("SocketTwo", (byte)Item.SocketTwo).Insert("Durability", Item.Durability)
                    .Insert("MaximDurability", Item.MaximDurability).Insert("SocketProgress", Item.SocketProgress)
                    .Insert("PlusProgress", Item.PlusProgress).Insert("Effect", (ushort)Item.Effect)
                    .Insert("Bound", Item.Bound).Insert("Locked", Item.Lock).Insert("Suspicious", Item.Suspicious)
                    .Insert("Color", (uint)Item.Color).Insert("Position", Item.Position).Insert("Warehouse", Item.Warehouse)
                    .Insert("UnlockEnd", Item.UnlockEnd.ToBinary()).Insert("SuspiciousStart", Item.SuspiciousStart.ToBinary())
                    .Insert("StackSize", Item.StackSize).Insert("MaxStackSize", Item.MaxStackSize);
                cmd.Execute();
            }
        }
        public static bool IsThere(uint uid)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("items").Where("UID", uid);
            var r = new MySqlReader(cmd);
            if (r.Read())
            {
                r.Close();
                return true;
            }
            r.Close();
            return false;
        }
        public static void UpdateItem(Interfaces.IConquerItem Item, Client.GameState client)
        {
            string agate = "";
            if (Item.ID == 720828)
            {
                foreach (string coord in Item.Agate_map.Values)
                {
                    agate += coord + "#";
                }
            }
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Plus", Item.Plus).Set("Bless", Item.Bless)
                .Set("Enchant", Item.Enchant).Set("SocketOne", (byte)Item.SocketOne)
                .Set("SocketTwo", (byte)Item.SocketTwo).Set("Durability", Item.Durability)
                .Set("MaximDurability", Item.MaximDurability).Set("SocketProgress", Item.SocketProgress)
                .Set("PlusProgress", Item.PlusProgress).Set("Effect", (ushort)Item.Effect)
                .Set("Bound", Item.Bound).Set("Locked", Item.Lock).Set("Position", Item.Position).Set("Warehouse", Item.Warehouse).Set("EntityID", client.Entity.UID)
                .Set("Suspicious", Item.Suspicious).Set("Color", (uint)Item.Color).Set("UnlockEnd", Item.UnlockEnd.ToBinary()).Set("SuspiciousStart", Item.SuspiciousStart.ToBinary())
                .Set("StackSize", Item.StackSize).Set("MaxStackSize", Item.MaxStackSize).Set("agate", agate).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateBless(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Bless", Item.Bless).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateColor(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Color", (uint)Item.Color).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateEnchant(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Enchant", Item.Enchant).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateLock(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Locked", Item.Lock).Set("UnlockEnd", Item.UnlockEnd.ToBinary()).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateSockets(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("SocketOne", (byte)Item.SocketOne)
                .Set("SocketTwo", (byte)Item.SocketTwo).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateSocketProgress(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("SocketProgress", Item.SocketProgress).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateDurabilityItem(Interfaces.IConquerItem Item)
        {
            //MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            //cmd.Update("items").Set("Durability", Item.Durability).Set("MaximDurability", Item.MaximDurability).Where("UID", Item.UID).Execute();
        }
        public static void UpdateStackItem(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("StackSize", Item.StackSize).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateLocation(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("EntityID", client.Entity.UID).Set("Position", Item.Position).Set("Warehouse", Item.Warehouse).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdatePosition(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Position", Item.Position).Set("Warehouse", Item.Warehouse).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdatePlus(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Plus", Item.Plus).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdatePlusProgress(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("PlusProgress", Item.PlusProgress).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void UpdateItemID(Interfaces.IConquerItem Item, Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("ID", Item.ID).Where("UID", Item.UID).Execute();
            if (res != 1)
                client.Inventory.ReviewItem(Item);
        }
        public static void RemoveItem(uint UID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("EntityID", 0).Set("Position", 0).Where("UID", UID).Execute();
            MySqlCommand cmds = new MySqlCommand(MySqlCommandType.DELETE).Delete("items", "UID", UID);
            cmds.Execute();
        }
        public static void DeleteItem(uint UID)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            int res = cmd.Delete("items", "UID", UID).Execute();
            MySqlCommand cmds = new MySqlCommand(MySqlCommandType.DELETE).Delete("items", "UID", UID);
            cmds.Execute();
        }
        public static void ClearPosition(uint EntityID, byte position)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            int res = cmd.Update("items").Set("Position", 0).Where("EntityID", EntityID).And("Position", position).Execute();
        }
    }
}
