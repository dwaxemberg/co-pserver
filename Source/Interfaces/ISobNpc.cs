using System;

namespace Conquer_Online_Server.Interfaces
{
    public interface ISobNpc
    {
        Game.Enums.NpcType Type { get; set; }
        uint UID { get; set; }
        ushort X { get; set; }
        ushort Y { get; set; }
        uint MaxHitpoints { get; set; }
        uint Hitpoints { get; set; }
        string Name { get; set; }
        ushort Mesh { get; set; }
        ushort MapID { get; set; }
        void SendSpawn(Client.GameState Client);
    }
}
