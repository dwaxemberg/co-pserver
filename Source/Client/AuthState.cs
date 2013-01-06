using System;
using Conquer_Online_Server.Network.Cryptography;
using System.Net.Sockets;
using System.Collections.Generic;
using Conquer_Online_Server.Network.Sockets;

namespace Conquer_Online_Server.Client
{
    public class AuthState
    {
        private bool Alive = false;
        public Network.AuthPackets.Authentication Info;

        public Database.AccountTable Account;
        public Network.Cryptography.AuthCryptography Cryptographer;
        public int PasswordSeed;
        public WinSocket _socket;

        public AuthState(WinSocket socket)
        {
            _socket = socket;
            Alive = true;
        }
        public void Send(byte[] buffer)
        {
            if (Alive)
            {
                byte[] _buffer = new byte[buffer.Length];
                Buffer.BlockCopy(buffer, 0, _buffer, 0, buffer.Length);
                lock (Cryptographer)
                {
                    Cryptographer.Encrypt(_buffer);
                    try
                    {
                        _socket.Send(_buffer);
                    }
                    catch (Exception e)//posible proxy/food with proxy
                    {
                        Console.WriteLine(e.ToString());
                        Program.SaveException(e);
                        this.Disconnect();
                    }
                }
            }
        }
        public void Send(Interfaces.IPacket buffer)
        {
            Send(buffer.ToArray());
        }

        public void Disconnect()
        {
            if (Alive)
            {

                Alive = false;
                _socket.Disconnect(false);
            }
        }
        public static uint nextID = 0;

    }
}
