using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.Sockets;

namespace Conquer_Online_Server.Interfaces
{
    public interface ISocketWrapper
    {
        int BufferSize { get; set; }
        byte[] Buffer { get; set; }
        WinSocket Socket { get; set; }
        object Connector { get; set; }
        ISocketWrapper Create(System.Net.Sockets.Socket socket);
    }
}
