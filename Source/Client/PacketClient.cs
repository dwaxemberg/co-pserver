using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Client
{
    [Serializable]
    public struct PacketClient
    {
        public byte[] Buffer;
        public Client.GameState client;

        public PacketClient(byte[] packet, Client.GameState _client)
        {
            Buffer = packet;
            client = _client;
        }
    }
}
