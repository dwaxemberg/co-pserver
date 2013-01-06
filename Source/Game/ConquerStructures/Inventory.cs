using System;
using System.Linq;
using System.Collections.Generic;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Inventory
    {
        Dictionary<uint, Interfaces.IConquerItem> inventory;
        Interfaces.IConquerItem[] objects;
        Client.GameState Owner;
        public Inventory(Client.GameState client)
        {
            Owner = client;
            inventory = new Dictionary<uint, Interfaces.IConquerItem>(40);
            objects = new Interfaces.IConquerItem[0];
        }
        public bool Add(uint id, byte plus, byte times)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
            while (times > 0)
            {
                if (Count <= 39)
                {
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    item.ID = id;
                    item.Plus = plus;
                    item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                    Add(item, Enums.ItemUse.CreateAndAdd);
                }
                else
                {
                    return false;
                }
                times--;
            }
            return true;
        }
        public bool Add35(uint id, byte plus, byte times)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
            while (times > 0)
            {
                if (Count <= 39)
                {
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    item.ID = id;
                    item.Plus = 12;
                    item.Enchant = 255;
                    item.Bless = 7;

                    item.SocketOne = Game.Enums.Gem.SuperDragonGem;
                    item.SocketTwo = Game.Enums.Gem.SuperDragonGem;
                    item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                    Add(item, Enums.ItemUse.CreateAndAdd);
                }
                else
                {
                    return false;
                }
                times--;
            }
            return true;
        }
        public bool Add(uint id, Game.Enums.ItemEffect effect)
        {
            Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
            item.ID = id;
            item.Effect = effect;
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, 0);
            item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
            if (Count <= 39)
            {
                Add(item, Enums.ItemUse.CreateAndAdd);
            }
            else
            {
                return false;
            }
            return true;
        }
        public bool Add(uint id, byte plus, byte times, bool free)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
            uint uid = 0;
            while (times > 0)
            {
                if (Count <= 39)
                {
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    item.ID = id;
                    item.Plus = plus;
                    item.Bound = free;
                    item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                    item.Color = (Game.Enums.Color)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 7);
                    item.UID = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, int.MaxValue);
                    Add(item, Enums.ItemUse.Add);
                    uid = item.UID;
                }
                else
                {
                    Owner.Send(ServerBase.Constants.FullInventory);
                    return false;
                }
                times--;
            }
            return true;
        }
        public bool Add2(uint id, byte plus, byte times)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
            while (times > 0)
            {
                if (Count <= 39)
                {
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    item.ID = id;
                    item.Plus = 2;
                    item.Enchant = 100;
                    // item.Bless = 1;
                    item.Bound = true;
                    item.SocketOne = Game.Enums.Gem.SuperDragonGem;
                    // item.SocketTwo = Game.Enums.Gem.SuperDragonGem;
                    item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                    Add(item, Enums.ItemUse.CreateAndAdd);
                }
                else
                {
                    return false;
                }
                times--;
            }
            return true;
        }
        public bool Add50(uint id, byte plus, byte times)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
            while (times > 0)
            {
                if (Count <= 39)
                {
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    item.ID = id;
                    item.Plus = 12;
                    item.Enchant = 255;
                    item.Bless = 7;
                    item.Color = (Game.Enums.Color)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 7);
                    item.SocketOne = Game.Enums.Gem.SuperDragonGem;
                    item.SocketTwo = Game.Enums.Gem.SuperDragonGem;
                    item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                    Add(item, Enums.ItemUse.CreateAndAdd);
                }
                else
                {
                    return false;
                }
                times--;
            }
            return true;
        }
        public bool Add60(uint id, byte plus, byte times)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
            while (times > 0)
            {
                if (Count <= 39)
                {
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    item.ID = id;
                    item.Plus = 12;
                    item.Enchant = 255;
                    item.Bless = 7;
                    item.Color = (Game.Enums.Color)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 7);
                    item.SocketOne = Game.Enums.Gem.SuperPhoenixGem;
                    item.SocketTwo = Game.Enums.Gem.SuperPhoenixGem;
                    item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                    Add(item, Enums.ItemUse.CreateAndAdd);
                }
                else
                {
                    return false;
                }
                times--;
            }
            return true;
        }
        public bool Add70(uint id, byte plus, byte times)
        {
            Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
            while (times > 0)
            {
                if (Count <= 39)
                {
                    Interfaces.IConquerItem item = new Network.GamePackets.ConquerItem(true);
                    item.ID = id;
                    item.Plus = 12;
                    item.Enchant = 255;
                    item.Bless = 7;
                    item.Color = (Game.Enums.Color)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 7);
                    item.SocketOne = Game.Enums.Gem.SuperPhoenixGem;
                    item.SocketTwo = Game.Enums.Gem.SuperPhoenixGem;
                    item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                    Add(item, Enums.ItemUse.CreateAndAdd);
                }
                else
                {
                    return false;
                }
                times--;
            }
            return true;
        }
        public bool Add(Interfaces.IConquerItem item, Enums.ItemUse use)
        {
            if (!Database.ConquerItemInformation.BaseInformations.ContainsKey(item.ID))
                return true;
            if (Count == 40 && CanStack(item.ID) == null)
            {
                Owner.Send(ServerBase.Constants.FullInventory);
                return false;
            }
            if (!inventory.ContainsKey(item.UID))
            {
                item.Position = 0;
                if (!Network.PacketHandler.IsEquipment(item.ID))
                {
                    if (item.StackSize != item.MaxStackSize)
                    {
                        Interfaces.IConquerItem stacker = null;
                        if ((stacker = CanStack(item.ID)) != null)
                        {
                            bool Continue = false;
                            if (stacker.StackSize + item.StackSize > stacker.MaxStackSize)
                            {
                                Continue = true;
                                item.StackSize = (ushort)(stacker.StackSize + item.StackSize - stacker.MaxStackSize);
                                stacker.StackSize = stacker.MaxStackSize;
                            }
                            else
                                stacker.StackSize++;
                            Database.ConquerItemTable.UpdateStackItem(stacker, Owner);
                            stacker.Mode = Enums.ItemMode.Update;
                            stacker.Send(Owner);
                            stacker.Mode = Enums.ItemMode.Default;
                            if (!Continue)
                                return true;
                        }
                    }
                }
                switch (use)
                {
                    case Enums.ItemUse.CreateAndAdd:
                        item.UID = Conquer_Online_Server.Network.GamePackets.ConquerItem.ItemUID.Next;
                        Database.ConquerItemTable.AddItem(ref item, Owner);
                        item.MobDropped = false;
                        break;
                    case Enums.ItemUse.Add:
                        Database.ConquerItemTable.UpdateLocation(item, Owner);
                        break;
                    case Enums.ItemUse.Move:
                        item.Position = 0;
                        item.StatsLoaded = false;
                        Database.ConquerItemTable.UpdateLocation(item, Owner);
                        break;
                }
                inventory.Add(item.UID, item);
                objects = inventory.Values.ToArray();
                item.Mode = Enums.ItemMode.Default;
                if (use != Enums.ItemUse.None)
                    item.Send(Owner);
                return true;
            }
            return false;
        }
        public void Update()
        {
            objects = inventory.Values.ToArray();
        }
        public bool Remove(Interfaces.IConquerItem item, Enums.ItemUse use)
        {
            if (inventory.ContainsKey(item.UID))
            {
                if (!Network.PacketHandler.IsEquipment(item.ID))
                {
                    if (item.StackSize != 0)
                    {
                        item.StackSize--;
                        if (item.StackSize != 0)
                        {
                            Database.ConquerItemTable.UpdateStackItem(item, Owner);
                            item.Mode = Enums.ItemMode.Update;
                            item.Send(Owner);
                            item.Mode = Enums.ItemMode.Default;
                            return true;
                        }
                    }
                }
                switch (use)
                {
                    case Enums.ItemUse.Remove: Database.ConquerItemTable.RemoveItem(item.UID); break;
                    case Enums.ItemUse.Delete: Database.ConquerItemTable.DeleteItem(item.UID); break;
                    case Enums.ItemUse.Move: Database.ConquerItemTable.UpdateLocation(item, Owner); break;
                }

                inventory.Remove(item.UID);
                objects = inventory.Values.ToArray();
                Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                iu.UID = item.UID;
                iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                Owner.Send(iu);
                return true;
            }
            return false;
        }
        public bool Remove(uint UID, Enums.ItemUse use, bool sendRemove)
        {
            if (inventory.ContainsKey(UID))
            {
                switch (use)
                {
                    case Enums.ItemUse.Delete: Database.ConquerItemTable.DeleteItem(UID); break;
                    case Enums.ItemUse.Remove: Database.ConquerItemTable.RemoveItem(UID); break;
                    case Enums.ItemUse.Move: Database.ConquerItemTable.UpdateLocation(inventory[UID], Owner); break;
                }
                inventory.Remove(UID);
                objects = inventory.Values.ToArray();
                if (sendRemove)
                {
                    Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                    iu.UID = UID;
                    iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                    Owner.Send(iu);
                }
                return true;
            }
            return false;
        }
        public void ReviewItem(Interfaces.IConquerItem item)
        {
            Database.ConquerItemTable.DeleteItem(item.UID);
            if (item.Position != 0)
            {
                Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                iu.UID = item.UID;
                iu.dwParam = item.Position;
                iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
                Owner.Send(iu);
            }
            Network.GamePackets.ItemUsage iud = new Network.GamePackets.ItemUsage(true);
            iud.UID = item.UID;
            iud.ID = Network.GamePackets.ItemUsage.RemoveInventory;
            Owner.Send(iud);

            Database.ConquerItemTable.AddItem(ref item, Owner);

            item.Send(Owner);
        }
        public bool Remove(string name)
        {
            foreach (var item in inventory.Values)
            {
                if (Database.ConquerItemInformation.BaseInformations[item.ID].Name.ToLower() == name.ToLower())
                {
                    Remove(item, Enums.ItemUse.Remove);
                    return true;
                }
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
        public byte Count { get { return (byte)Objects.Length; } }

        public bool TryGetItem(uint UID, out Interfaces.IConquerItem item)
        {
            return inventory.TryGetValue(UID, out item);
        }

        public bool ContainsUID(uint UID)
        {
            return inventory.ContainsKey(UID);
        }

        public bool Contains(uint ID, ushort times)
        {
            if (ID == 0)
                return true;
            ushort has = 0;
            foreach (Interfaces.IConquerItem item in Objects)
                if (item.ID == ID)
                {
                    if (item.StackSize == 0)
                        has++;
                    else
                        has += (byte)item.StackSize;
                }
            return has >= times;
        }
        public Interfaces.IConquerItem GetItemByID(uint ID)
        {
            foreach (Interfaces.IConquerItem item in Objects)
                if (item.ID == ID)
                    return item;
            return null;
        }
        public bool Remove(uint ID, byte times)
        {
            if (ID == 0)
                return true;
            List<Interfaces.IConquerItem> items = new List<Interfaces.IConquerItem>();
            byte has = 0;
            foreach (Interfaces.IConquerItem item in Objects)
                if (item.ID == ID)
                { has++; items.Add(item); if (has >= times) break; }
            if (has >= times)
                foreach (Interfaces.IConquerItem item in items)
                    Remove(item, Enums.ItemUse.Remove);
            return has >= times;
        }

        public Interfaces.IConquerItem CanStack(uint ID)
        {
            foreach (Interfaces.IConquerItem item in Objects)
                if (item.ID == ID)
                    if (item.StackSize != item.MaxStackSize)
                        return item;
            return null;
        }

        internal void Add35(int p, int p_2, int p_3)
        {
            throw new NotImplementedException();
        }
    }
}
