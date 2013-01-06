using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Trade
    {
        public uint Money, ConquerPoints, TraderUID;
        public List<Interfaces.IConquerItem> Items;
        public bool Accepted, InTrade;
        public Trade()
        {
            InTrade = Accepted = false;
            ConquerPoints = Money = TraderUID = 0;
            Items = new List<Interfaces.IConquerItem>();
        }
    }
}
