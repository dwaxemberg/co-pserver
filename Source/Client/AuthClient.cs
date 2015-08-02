using System;
using Conquer_Online_Server.Network.Cryptography;
using System.Net.Sockets;
using Conquer_Online_Server.Network.Sockets;
using Conquer_Online_Server.Network;

namespace Conquer_Online_Server.Client
{
    public class AuthClient
    {
        private ClientWrapper _socket;
        public Network.AuthPackets.Authentication Info;
        public Database.AccountTable Account;
        public Network.Cryptography.AuthCryptography Cryptographer;
        public int PasswordSeed;
        public ConcurrentPacketQueue Queue;
        public AuthClient(ClientWrapper socket)
        {
            Queue = new ConcurrentPacketQueue(0);
            _socket = socket;
        }
        public void Send(byte[] buffer)
        {
            byte[] _buffer = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, _buffer, 0, buffer.Length);
            Cryptographer.Encrypt(_buffer);
            _socket.Send(_buffer);
        }
        public void Send(Interfaces.IPacket buffer)
        {
            Send(buffer.ToArray());
        }
        public static uint nextID = 0;
    }
}
