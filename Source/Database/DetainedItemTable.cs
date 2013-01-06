using System;
using System.IO;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Database
{
    public class DetainedItemTable
    {
        public static ServerBase.Counter Counter = new Conquer_Online_Server.ServerBase.Counter(1);
        public static void LoadDetainedItems(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("detaineditems").Where("OwnerUID", client.Entity.UID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                DetainedItem item = new DetainedItem(true);
                item.ItemUID = r.ReadUInt32("ItemUID");
                item.UID = item.ItemUID - 1;
                item.Item = ConquerItemTable.LoadItem(item.ItemUID);
                item.ConquerPointsCost = r.ReadUInt32("ConquerPointsCost");
                item.OwnerUID = r.ReadUInt32("OwnerUID");
                item.OwnerName = r.ReadString("OwnerName");
                item.GainerUID = r.ReadUInt32("GainerUID");
                item.GainerName = r.ReadString("GainerName");
                item.Date = DateTime.FromBinary(r.ReadInt64("Date"));
                item.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(item.Date.Ticks).Days);
                if (DateTime.Now < item.Date.AddDays(7))
                    client.DeatinedItem.Add(item.UID, item);
                else
                    if (item.Bound)
                        Claim(item, client);
            }
            r.Close();
        }
        public static void LoadClaimableItems(Client.GameState client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("detaineditems").Where("GainerUID", client.Entity.UID);
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                DetainedItem item = new DetainedItem(true);
                item.ItemUID = r.ReadUInt32("ItemUID");
                item.UID = item.ItemUID - 1;
                item.Page = (byte)DetainedItem.ClaimPage;
                item.Item = ConquerItemTable.LoadItem(item.ItemUID);
                item.ConquerPointsCost = r.ReadUInt32("ConquerPointsCost");
                item.OwnerUID = r.ReadUInt32("GainerUID");
                item.GainerName = r.ReadString("GainerName");
                item.GainerUID = r.ReadUInt32("OwnerUID");
                item.OwnerName = r.ReadString("OwnerName");
                item.Date = DateTime.FromBinary(r.ReadInt64("Date"));
                item.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(item.Date.Ticks).Days);
                if (item.GainerUID == 500)
                {
                    item.MakeItReadyToClaim();
                    item.GainerUID = r.ReadUInt32("GainerUID");
                    item.OwnerUID = r.ReadUInt32("OwnerUID");
                }
                client.ClaimableItem.Add(item.UID, item);
            }
            r.Close();
        }
        public static void DetainItem(Interfaces.IConquerItem item, Client.GameState owner, Client.GameState gainer)
        {
            DetainedItem Item = new DetainedItem(true);
            Item.ItemUID = item.UID;
            Item.Item = item;
            Item.UID = Item.ItemUID - 1;
            Item.ConquerPointsCost = CalculateCost(item);
            Item.OwnerUID = owner.Entity.UID;
            Item.OwnerName = owner.Entity.Name;
            Item.GainerUID = gainer.Entity.UID;
            Item.GainerName = gainer.Entity.Name;
            Item.Date = DateTime.Now;
            Item.DaysLeft = 0;
            owner.DeatinedItem.Add(Item.UID, Item);
            owner.Send(Item);

            DetainedItem Item2 = new DetainedItem(true);
            Item2.ItemUID = item.UID;
            Item2.UID = Item2.ItemUID - 1;
            Item2.Item = item;
            Item2.Page = (byte)DetainedItem.ClaimPage;
            Item2.ConquerPointsCost = CalculateCost(item);
            Item2.OwnerUID = gainer.Entity.UID;
            Item2.GainerName = gainer.Entity.Name;
            Item2.GainerUID = owner.Entity.UID;
            Item2.OwnerName = owner.Entity.Name;
            Item2.Date = Item.Date;
            Item2.DaysLeft = 0;
            gainer.ClaimableItem.Add(Item2.UID, Item2);
            gainer.Send(Item2);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT)
                .Insert("detaineditems")
                .Insert("ItemUID", item.UID)
                .Insert("ConquerPointsCost", Item.ConquerPointsCost)
                .Insert("Date", Item.Date.Ticks)
                .Insert("OwnerUID", owner.Entity.UID)
                .Insert("OwnerName", owner.Entity.Name)
                .Insert("GainerUID", gainer.Entity.UID)
                .Insert("GainerName", gainer.Entity.Name);
            //.Where("ItemUID", item.UID);
            cmd.Execute();

        }

        public static void Redeem(DetainedItem item, Client.GameState owner)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE)
                .Update("detaineditems").Set("OwnerUID", 500).Where("ItemUID", item.Item.UID).And("OwnerUID", owner.Entity.UID);
            cmd.Execute();
        }

        public static void Claim(DetainedItem item, Client.GameState owner)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE).Delete("detaineditems", "ItemUID", item.Item.UID).And("GainerUID", owner.Entity.UID);
            cmd.Execute();
        }

        public static uint CalculateCost(Interfaces.IConquerItem item)
        {
            int basic = 10;
            if (item.ID % 10 == 9)
                basic += 10;
            if (item.ID / 100000 == 4 || item.ID / 100000 == 5)
            {
                if (item.SocketOne != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                    basic += 50;
                if (item.SocketTwo != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                    basic += 200;
            }
            else
            {
                if (item.SocketOne != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                    basic += 200;
                if (item.SocketTwo != Conquer_Online_Server.Game.Enums.Gem.NoSocket)
                    basic += 800;
            }
            basic += item.Plus * 50;
            return (uint)basic;
        }
    }
}
