using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Interfaces;

namespace Conquer_Online_Server.Game.Features.Flowers
{
    public class FlowerSystem
    {
        private FlowerPacket Packet;

        public FlowerSystem(byte[] BasePacket, Receivers.ClientState Caller)
        {
            Packet = new FlowerPacket(false);
            Packet.Deserialize(BasePacket);

            if (!Kernel.GamePool.ContainsKey(Packet.UID1)) return;
            if (Caller.Entity.Level < 50) return;
            if (Caller.Entity.Body == 2001 || Caller.Entity.Body == 2002) return;
            if (Kernel.GamePool[Packet.UID1].Entity.Body == 1003 || Kernel.GamePool[Packet.UID1].Entity.Body == 1004) return;

            if (Packet.ItemUID == 0)
            {
                if (Caller.Entity.Flowers.LastFlowerSent == null) Caller.Entity.Flowers.LastFlowerSent = DateTime.Now.Subtract(TimeSpan.FromDays(1));
                if (Packet.FlowerType != FlowerType.RedRoses && Packet.Amount != 1) return;
                if (Caller.Entity.Flowers.LastFlowerSent.AddDays(1) <= DateTime.Now)
                {
                    Caller.Entity.Flowers.LastFlowerSent = DateTime.Now;

                    FlowerPacket NewPacket = new FlowerPacket(true);
                    NewPacket.SenderName = Caller.Entity.Name;
                    NewPacket.ReceiverName = Kernel.GamePool[Packet.UID1].Entity.Name;
                    NewPacket.SendAmount = 1;
                    NewPacket.SendFlowerType = FlowerType.RedRoses;
                    Kernel.GamePool[Packet.UID1].Send(NewPacket);
                    Kernel.GamePool[Packet.UID1].Entity.Flowers.RedRoses++;
                    Kernel.GamePool[Packet.UID1].Entity.Flowers.RedRoses2day++;
                }
                else
                    Caller.Send(Constants.OneFlowerADay);
            }
            else
            {
                IConquerItem Item;
                if (Caller.Inventory.TryGetValue(Packet.ItemUID, out Item))
                {
                    if (Item.Durability != Packet.Amount) return;
                    FlowerType Flower = FlowerType.Unknown;
                    switch (Item.ID / 1000)
                    {
                        case 751: Flower = FlowerType.RedRoses; break;
                        case 752: Flower = FlowerType.Lilies; break;
                        case 753: Flower = FlowerType.Orchids; break;
                        case 754: Flower = FlowerType.Tulips; break;
                    }
                    if (Flower != FlowerType.Unknown)
                    {
                        switch (Flower)
                        {
                            case FlowerType.RedRoses: Kernel.GamePool[Packet.UID1].Entity.Flowers.RedRoses += Packet.Amount; Kernel.GamePool[Packet.UID1].Entity.Flowers.RedRoses2day += Packet.Amount; break;
                            case FlowerType.Lilies: Kernel.GamePool[Packet.UID1].Entity.Flowers.Lilies += Packet.Amount; Kernel.GamePool[Packet.UID1].Entity.Flowers.Lilies2day += Packet.Amount; break;
                            case FlowerType.Orchids: Kernel.GamePool[Packet.UID1].Entity.Flowers.Orchads += Packet.Amount; Kernel.GamePool[Packet.UID1].Entity.Flowers.Orchads2day += Packet.Amount; break;
                            case FlowerType.Tulips: Kernel.GamePool[Packet.UID1].Entity.Flowers.Tulips += Packet.Amount; Kernel.GamePool[Packet.UID1].Entity.Flowers.Tulips2day += Packet.Amount; break;
                        }

                        FlowerPacket NewPacket = new FlowerPacket(true);
                        NewPacket.SenderName = Caller.Entity.Name;
                        NewPacket.ReceiverName = Kernel.GamePool[Packet.UID1].Entity.Name;
                        NewPacket.SendAmount = Packet.Amount;
                        NewPacket.SendFlowerType = Flower;
                        Kernel.GamePool[Packet.UID1].Send(NewPacket);
                        Caller.Inventory.Remove(Item, Enums.ItemUse.Remove);
                        Database.ConquerItemTable.RemoveItem(Item.UID);
                    }
                }
            }
        }
    }
}