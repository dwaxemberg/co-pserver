using System;

namespace Conquer_Online_Server.Interfaces
{
    public interface INpc
    {
        Game.Enums.NpcType Type { get; set; }
        uint UID { get; set; }
        ushort X { get; set; }
        ushort Y { get; set; }
        ushort Mesh { get; set; }
        ushort MapID { get; set; }
        void SendSpawn(Client.GameState Client);
        void SendSpawn(Client.GameState Client, bool checkScreen);
    }
}
