using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;

namespace Conquer_Online_Server.Network.Sockets
{
    public class SyncSocketWrapper : Interfaces.ISocketWrapper
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

        private Thread thread;

        private SyncSocket Server;

        public void BeginReceive(SyncSocket server)
        {
            Server = server;
            thread = new Thread(new ThreadStart(Receive));
            thread.Start();
        }

        private void Receive()
        {
            try
            {
                while (Socket.Connected)
                {
                    try
                    {
                        int RecvSize = Socket.Receive(Buffer);
                        if (RecvSize > 0)
                        {
                            byte[] buffer = new byte[RecvSize];
                            for (int i = 0; i < RecvSize; i++)
                            {
                                buffer[i] = Buffer[i];
                            }
                            Server.InvokeOnClientReceive(this, buffer);
                        }
                        else
                        {
                            Server.InvokeDisconnect(this);
                            return;
                        }
                    }
                    catch (SocketException)
                    {
                        Server.InvokeDisconnect(this);
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        Server.InvokeDisconnect(this);
                        return;
                    }
                    catch (Exception e) { Program.SaveException(e); }
                }
                Server.InvokeDisconnect(this);
                return;
            }
            catch (ThreadAbortException)
            {
                Server.InvokeDisconnect(this);
                return;
            }
        }
    }
}
