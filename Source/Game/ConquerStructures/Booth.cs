using System;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
namespace Conquer_Online_Server.Game.ConquerStructures
{
    public struct BoothItem
    {
        public enum CostType:byte      
        {
            Silvers = 1,
            ConquerPoints = 3
        }
        public Interfaces.IConquerItem Item;
        public uint Cost;
        public CostType Cost_Type;
    }
    public class Booth
    {
        public SafeDictionary<uint, BoothItem> ItemList;
        Client.GameState Owner;
        public SobNpcSpawn Base;
        public Message HawkMessage;
        public Booth(Client.GameState client, Data data)
        {
            Owner = client;
            Owner.Booth = this;
            Owner.Entity.Action = Enums.ConquerAction.Sit;
            ItemList = new SafeDictionary<uint, BoothItem>(20);
            Base = new SobNpcSpawn();
            Base.Owner = Owner;
            Base.UID = (uint)((Owner.Entity.UID % 1000000) + ((Owner.Entity.UID / 1000000) * 100000));
            Base.Mesh = 406;
            Base.Type = Game.Enums.NpcType.Booth;
            Base.ShowName = true;
            Base.Name = Name;
            Base.X = (ushort)(Owner.Entity.X + 1);
            Base.Y = Owner.Entity.Y;
            Owner.SendScreenSpawn(Base, true);
            data.dwParam = Base.UID;
            data.wParam1 = Base.X;
            data.wParam2 = Base.Y;
            data.ID = Data.OwnBooth;
            Owner.Send(data);
        }
        public string Name
        {
            get
            {
                return Owner.Entity.Name;
            }
        }
        public static implicit operator byte[](Booth booth)
        {
            return booth.Base.ToArray();
        }
        public static implicit operator SobNpcSpawn(Booth booth)
        {
            return booth.Base;
        }
        public void Remove()
        {
            Network.GamePackets.Data data = new Network.GamePackets.Data(true);
            data.UID = Base.UID;
            data.ID = Network.GamePackets.Data.RemoveEntity;
            Owner.SendScreen(data, true);
        }
    }
}
