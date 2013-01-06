using System;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Equipment
    {
        Interfaces.IConquerItem[] objects;
        Client.GameState Owner;
        public Equipment(Client.GameState client)
        {
            Owner = client;
            objects = new Interfaces.IConquerItem[20];
        }

        public void UpdateEntityPacket()
        {
            for (byte Position = 1; Position < 19; Position++)
            {
                if (Free(Position))
                {
                    ClearItemview(Position);
                }
                else
                {
                    var item = TryGetItem(Position);
                    UpdateItemview(item);
                }
            }
            Owner.SendScreen(Owner.Entity.SpawnPacket, false);
        }
        public bool Add(Interfaces.IConquerItem item)
        {
            try
            {
                if (item.Position > 19)
                    return false;
                if (objects[item.Position - 1] == null)
                {
                    UpdateItemview(item);
                    objects[item.Position - 1] = item;
                    item.Position = item.Position;
                    item.Send(Owner);
                    EntityEquipment equips = new EntityEquipment(true);
                    equips.ParseHero(Owner);
                    Owner.Send(equips);
                    Owner.LoadItemStats(item);
                    Owner.SendScreenSpawn(Owner.Entity, false);
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Program.SaveException(e);
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public bool Add(Interfaces.IConquerItem item, Enums.ItemUse use)
        {
            try
            {
                if (item.Position < 20)
                {
                    if (objects[item.Position - 1] == null)
                    {
                        objects[item.Position - 1] = item;
                        item.Mode = Enums.ItemMode.Default;

                        if (use != Enums.ItemUse.None)
                        {
                            UpdateItemview(item);
                            EntityEquipment equips = new EntityEquipment(true);
                            equips.ParseHero(Owner);
                            Owner.Send(equips);
                            item.Send(Owner);
                            Owner.LoadItemStats(item);
                        }
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            catch { return false; }
        }

        public void ClearItemview(uint Position)
        {
            switch ((ushort)Position)
            {
                case Network.GamePackets.ConquerItem.Head:
                    Network.Writer.WriteUInt32(0, 182, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, 44, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16(0, 109, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.Garment:
                    Network.Writer.WriteUInt32(0, 48, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.Armor:
                    Network.Writer.WriteUInt32(0, 186, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, 42, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16(0, 123, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.RightWeapon:
                    Network.Writer.WriteUInt32(0, 197, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, 60, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.LeftWeapon:
                    Network.Writer.WriteUInt32(0, 194, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, 56, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16(0, 129, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.RightWeaponAccessory:
                    Network.Writer.WriteUInt32(0, 56, Owner.Entity.SpawnPacket);
                    if (!Free(Network.GamePackets.ConquerItem.RightWeapon))
                    {

                        var item = TryGetItem(Network.GamePackets.ConquerItem.RightWeapon);

                        Network.Writer.WriteUInt32(item.ID, 60, Owner.Entity.SpawnPacket);
                    }
                    break;
                case Network.GamePackets.ConquerItem.LeftWeaponAccessory:
                    Network.Writer.WriteUInt32(0, 56, Owner.Entity.SpawnPacket);
                    if (!Free(Network.GamePackets.ConquerItem.LeftWeapon))
                    {
                        var item = TryGetItem(Network.GamePackets.ConquerItem.LeftWeapon);

                        Network.Writer.WriteUInt32(item.ID, 56, Owner.Entity.SpawnPacket);
                    }
                    break;
                case Network.GamePackets.ConquerItem.Steed:
                    Network.Writer.WriteUInt32(0, 72, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16(0, 137, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, 143, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.SteedMount:
                    {
                        Network.Writer.WriteUInt32(0, 76, Owner.Entity.SpawnPacket);
                        break;
                    }
                case Network.GamePackets.ConquerItem.RidingCrop:
                    {
                        Owner.Entity.RidingCropID = 0;
                        break;
                    }
            }
        }
        public void UpdateItemview(Interfaces.IConquerItem item)
        {
            switch ((ushort)item.Position)
            {
                case Network.GamePackets.ConquerItem.Head:
                    if (item.Purification.Available)
                        Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 194, Owner.Entity.SpawnPacket);
                    //else
                    Network.Writer.WriteUInt32(item.ID, 44, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16((byte)item.Color, 135, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.Garment:
                    Network.Writer.WriteUInt32(item.ID, 48, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.Armor:
                    if (item.Purification.Available)
                        Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 194, Owner.Entity.SpawnPacket);
                    // else
                    Network.Writer.WriteUInt32(item.ID, 52, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16((byte)item.Color, 131, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.RightWeapon:
                    if (!Free(Network.GamePackets.ConquerItem.RightWeaponAccessory))
                    {

                        var Item = TryGetItem(Network.GamePackets.ConquerItem.RightWeaponAccessory);
                        //2-Handed accessories
                        if (Item.ID >= 350001 && Item.ID <= 350020)
                        {
                            if (Network.PacketHandler.IsTwoHand(item.ID))
                            {
                                if (item.Purification.Available)
                                    Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 198, Owner.Entity.SpawnPacket);
                                // else
                                Network.Writer.WriteUInt32(Item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                        //1-Handed accessories
                        if (Item.ID >= 360001 && Item.ID <= 360040)
                        {
                            if (!Network.PacketHandler.IsTwoHand(item.ID))
                            {
                                if (item.Purification.Available)
                                    Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 198, Owner.Entity.SpawnPacket);
                                //else
                                Network.Writer.WriteUInt32(Item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                        //Bow accessories
                        if (Item.ID >= 370001 && Item.ID <= 370015)
                        {
                            if (item.ID / 1000 == 500)
                            {
                                if (item.Purification.Available)
                                    Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 198, Owner.Entity.SpawnPacket);
                                //else
                                Network.Writer.WriteUInt32(Item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                    } if (item.Purification.Available)
                        Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 198, Owner.Entity.SpawnPacket);
                    // else
                    Network.Writer.WriteUInt32(item.ID, 60, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.RightWeaponAccessory:
                    if (!Free(Network.GamePackets.ConquerItem.RightWeapon))
                    {
                        var Item = TryGetItem(Network.GamePackets.ConquerItem.RightWeapon);
                        //2-Handed accessories
                        if (item.ID >= 350001 && item.ID <= 350020)
                        {
                            if (Network.PacketHandler.IsTwoHand(Item.ID))
                            {
                                Network.Writer.WriteUInt32(0, 198, Owner.Entity.SpawnPacket);
                                Network.Writer.WriteUInt32(item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                        //1-Handed accessories
                        if (item.ID >= 360001 && item.ID <= 360040)
                        {
                            if (!Network.PacketHandler.IsTwoHand(Item.ID))
                            {
                                Network.Writer.WriteUInt32(0, 198, Owner.Entity.SpawnPacket);
                                Network.Writer.WriteUInt32(item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                        //Bow accessories
                        if (item.ID >= 370001 && item.ID <= 370015)
                        {
                            if (Item.ID / 1000 == 500)
                            {
                                Network.Writer.WriteUInt32(0, 198, Owner.Entity.SpawnPacket);
                                Network.Writer.WriteUInt32(item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                    }
                    break;
                case Network.GamePackets.ConquerItem.LeftWeapon:
                    Network.Writer.WriteUInt16((byte)item.Color, 111, Owner.Entity.SpawnPacket);//125

                    if (!Free(Network.GamePackets.ConquerItem.LeftWeaponAccessory))
                    {

                        var Item = TryGetItem(Network.GamePackets.ConquerItem.LeftWeaponAccessory);

                        //1-Handed accessories
                        if (Item.ID >= 360001 && Item.ID <= 360040 && item.ID / 1000 != 900)
                        {
                            if (!Network.PacketHandler.IsTwoHand(item.ID))
                            {
                                if (item.Purification.Available)
                                    Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 194, Owner.Entity.SpawnPacket);
                                //   else
                                Network.Writer.WriteUInt32(Item.ID, 56, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                        //Shield accessories
                        if (Item.ID >= 380001 && Item.ID <= 380015)
                        {
                            if (item.ID / 1000 == 900)
                            {
                                if (item.Purification.Available)
                                    Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 194, Owner.Entity.SpawnPacket);
                                // else
                                Network.Writer.WriteUInt32(Item.ID, 56, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                    } if (item.Purification.Available)
                        Network.Writer.WriteUInt32(item.Purification.PurificationItemID, 194, Owner.Entity.SpawnPacket);
                    //else
                    Network.Writer.WriteUInt32(item.ID, 56, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.LeftWeaponAccessory:
                    if (!Free(Network.GamePackets.ConquerItem.LeftWeapon))
                    {
                        var Item = TryGetItem(Network.GamePackets.ConquerItem.LeftWeapon);

                        //1-Handed accessories
                        if (item.ID >= 360001 && item.ID <= 360040)
                        {
                            if (!Network.PacketHandler.IsTwoHand(Item.ID))
                            {
                                Network.Writer.WriteUInt32(0, 194, Owner.Entity.SpawnPacket);
                                Network.Writer.WriteUInt32(item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                        //Shield accessories
                        if (item.ID >= 380001 && item.ID <= 380015)
                        {
                            if (Item.ID / 1000 == 900)
                            {
                                Network.Writer.WriteUInt32(0, 190, Owner.Entity.SpawnPacket);
                                Network.Writer.WriteUInt32(item.ID, 60, Owner.Entity.SpawnPacket);
                                break;
                            }
                        }
                    }
                    break;
                case Network.GamePackets.ConquerItem.Steed:
                    Network.Writer.WriteUInt32(item.ID, 72, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16((byte)item.Plus, 141, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(item.SocketProgress, 143, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.SteedMount:
                    {
                        Network.Writer.WriteUInt32(item.ID, 76, Owner.Entity.SpawnPacket);
                        break;
                    }
                case Network.GamePackets.ConquerItem.RidingCrop:
                    {
                        Owner.Entity.RidingCropID = item.ID;
                        break;
                    }
            }
        }
        public bool Remove(byte Position)
        {
            if (Position > 19)
                return false;
            if (objects[Position - 1] != null)
            {
                if (Owner.Inventory.Count <= 39)
                {
                    if (Owner.Inventory.Add(objects[Position - 1], Enums.ItemUse.Move))
                    {
                        objects[Position - 1].Position = Position;
                        Owner.UnloadItemStats(objects[Position - 1], false);
                        objects[Position - 1].Position = 0;
                        if (Position == 12)
                            Owner.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                        if (Position == 4)
                            Owner.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                        Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                        iu.UID = objects[Position - 1].UID;
                        iu.dwParam = Position;
                        iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
                        Owner.Send(iu);

                        EntityEquipment equips = new EntityEquipment(true);
                        equips.ParseHero(Owner);
                        Owner.Send(equips);
                        ClearItemview(Position);
                        objects[Position - 1] = null;
                        Owner.SendScreenSpawn(Owner.Entity, false);
                        return true;
                    }
                }
                else
                {
                    Owner.Send(new Network.GamePackets.Message("Not enough room in your inventory.", System.Drawing.Color.Blue, Network.GamePackets.Message.TopLeft));
                }
            }
            return false;
        }
        public bool DestroyArrow(uint Position)
        {
            if (Position > 19)
                return false;
            if (objects[Position - 1] != null)
            {
                objects[Position - 1].Position = (ushort)Position;
                if (objects[Position - 1].ID == 0)
                {
                    objects[Position - 1].Position = 0;
                    Database.ConquerItemTable.DeleteItem(objects[Position - 1].UID);
                    objects[Position - 1] = null;
                    return true;
                }
                if (!Network.PacketHandler.IsArrow(objects[Position - 1].ID))
                    return false;

                Owner.UnloadItemStats(objects[Position - 1], false);
                Database.ConquerItemTable.DeleteItem(objects[Position - 1].UID);
                Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                iu.UID = objects[Position - 1].UID;
                iu.dwParam = Position;
                iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
                Owner.Send(iu);
                iu.dwParam = 0;
                iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                Owner.Send(iu);
                EntityEquipment equips = new EntityEquipment(true);
                equips.ParseHero(Owner);
                Owner.Send(equips);
                ClearItemview(Position);
                objects[Position - 1].Position = 0;
                objects[Position - 1] = null;
                return true;
            }
            return false;
        }
        public bool RemoveToGround(uint Position)
        {
            if (Position == 0 || Position > 19)
                return true;
            if (objects[Position - 1] != null)
            {
                objects[Position - 1].Position = (ushort)Position;
                Owner.UnloadItemStats(objects[Position - 1], false);
                objects[Position - 1].Position = 0;
                Database.ConquerItemTable.RemoveItem(objects[Position - 1].UID);
                Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                iu.UID = objects[Position - 1].UID;
                iu.dwParam = Position;
                iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
                Owner.Send(iu);
                iu.dwParam = 0;
                iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                Owner.Send(iu);

                EntityEquipment equips = new EntityEquipment(true);
                equips.ParseHero(Owner);
                Owner.Send(equips);
                ClearItemview(Position);
                objects[Position - 1] = null;
                return true;
            }
            return false;
        }
        public Interfaces.IConquerItem[] Objects
        {
            get
            {
                return objects;
            }
        }
        public byte Count
        {
            get
            {
                byte count = 0; foreach (Interfaces.IConquerItem i in objects)
                    if (i != null)
                        count++; return count;
            }
        }
        public bool Free(byte Position)
        {
            return TryGetItem(Position) == null;
        }
        public bool Free(uint Position)
        {
            return TryGetItem((byte)Position) == null;
        }
        public Interfaces.IConquerItem TryGetItem(byte Position)
        {
            Interfaces.IConquerItem item = null;
            if (Position < 1 || Position > 19)
                return item;
            item = objects[Position - 1];
            return item;
        }
        public Interfaces.IConquerItem TryGetItem(uint uid)
        {
            try
            {
                foreach (Interfaces.IConquerItem item in objects)
                {
                    if (item != null)
                        if (item.UID == uid)
                            return item;
                }
            }
            catch (Exception e)
            {
                Program.SaveException(e);
                Console.WriteLine(e);
            }
            return TryGetItem((byte)uid);
        }

        public bool IsArmorSuper()
        {
            if (TryGetItem(3) != null)
                return TryGetItem(3).ID % 10 == 9;
            return false;
        }
        public bool IsAllSuper()
        {
            for (byte count = 1; count < 12; count++)
            {
                if (count == 5)
                {
                    if (Owner.Entity.Class > 100)
                        continue;
                    if (TryGetItem(count) != null)
                    {
                        if (Network.PacketHandler.IsArrow(TryGetItem(count).ID))
                            continue;
                        if (Network.PacketHandler.IsTwoHand(TryGetItem(4).ID))
                            continue;
                        if (TryGetItem(count).ID % 10 != 9)
                            return false;
                    }
                }
                else
                {
                    if (TryGetItem(count) != null)
                    {
                        if (count != Network.GamePackets.ConquerItem.Bottle && count != Network.GamePackets.ConquerItem.Garment)
                            if (TryGetItem(count).ID % 10 != 9)
                                return false;
                    }
                    else
                        if (count != Network.GamePackets.ConquerItem.Bottle && count != Network.GamePackets.ConquerItem.Garment)
                            return false;
                }
            }
            return true;
        }
    }
}
