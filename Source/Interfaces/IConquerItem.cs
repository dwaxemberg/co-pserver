using System;
using System.Drawing;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
namespace Conquer_Online_Server.Interfaces
{
    public interface IConquerItem
    {
        uint UID { get; set; }
        uint ID { get; set; }
        ushort Durability { get; set; }
        ushort MaximDurability { get; set; }
        ushort Position { get; set; }
        ushort Warehouse { get; set; }
        uint SocketProgress { get; set; }
        uint PlusProgress { get; set; }
        Game.Enums.Gem SocketOne { get; set; }
        Game.Enums.Gem SocketTwo { get; set; }
        Game.Enums.ItemEffect Effect { get; set; }
        Game.Enums.ItemMode Mode { get; set; }
        byte Plus { get; set; }
        byte Bless { get; set; }
        bool Bound { get; set; }
        byte Enchant { get; set; }
        Dictionary<uint, string> Agate_map { get; set; }
        byte Lock { get; set; }
        DateTime UnlockEnd { get; set; }
        bool Suspicious { get; set; }
        DateTime SuspiciousStart { get; set; }
        Game.Enums.Color Color { get; set; }
        ushort BattlePower { get; }
        bool StatsLoaded { get; set; }
        uint RefineryPart { get; set; }
        uint RefineryLevel { get; set; }
        ushort RefineryPercent { get; set; }
        DateTime RefineryStarted { get; set; }
        ushort Vigor { get; set; }
        ushort StackSize { get; set; }
        ushort MaxStackSize { get; set; }
        ItemAdding.Purification_ Purification { get; set; }
        ItemAdding.Refinery_ ExtraEffect { get; set; }
        void SetID(uint ID);
        bool MobDropped { get; set; }
        bool Inscribed { get; set; }
        void Send(Client.GameState Client);
        void SendAgate(Client.GameState Client);
    }
}
