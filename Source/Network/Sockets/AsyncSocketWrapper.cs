using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Conquer_Online_Server.Network.Sockets
{
    public class AsyncSocketWrapper : Interfaces.ISocketWrapper
    {
        public int BufferSize
        {
            get;
            set;
        }
        public byte[] Buffer
        {
            get;
            set;
        }
        public WinSocket Socket
        {
            get;
            set;
        }
        public object Connector
        {
            get;
            set;
        }
        public Interfaces.ISocketWrapper Create(Socket socket)
        {
            BufferSize = 8000;
            Socket = new WinSocket(socket);
            Buffer = new byte[BufferSize];
            return this;
        }
    }
}
