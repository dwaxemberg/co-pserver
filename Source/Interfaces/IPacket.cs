using System;

namespace Conquer_Online_Server.Interfaces
{
    public interface IPacket
    {
        byte[] ToArray();
        void Deserialize(byte[] buffer);
        void Send(Client.GameState client);
    }
}
