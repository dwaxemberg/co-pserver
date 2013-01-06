using OpenSSL;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Conquer_Online_Server.Network.Sockets
{
    public class AsyncSocket
    {
        private int backlog;
        private int clientbuffersize = 0xffff;
        private WinSocket Connection = new WinSocket();
        private bool enabled;
        public bool GameServer = false;
        public event Action<Interfaces.ISocketWrapper> OnClientConnect;
        public event Action<Interfaces.ISocketWrapper> OnClientDisconnect;
        public event Action<byte[], Interfaces.ISocketWrapper> OnClientReceive;
        private ushort port;

        public AsyncSocket(ushort port)
        {
            if (!this.enabled)
            {
                this.port = port;
                this.Connection.Bind(new IPEndPoint(IPAddress.Any, this.port));
                this.Connection.Listen(100);
                this.Connection.BeginAccept(new AsyncCallback(this.AsyncConnect), null);
                this.enabled = true;
            }
        }

        private void AsyncConnect(IAsyncResult res)
        {
            AsyncSocketWrapper sender = null;
            try
            {
                sender = new AsyncSocketWrapper();
                sender.Create(this.Connection.EndAccept(res));
                if (sender.Socket == null)
                {
                    this.Connection.BeginAccept(new AsyncCallback(this.AsyncConnect), null);

                    return;
                }
                if (this.OnClientConnect != null)
                {
                    this.OnClientConnect(sender);
                }
                sender.Socket.BeginReceive(sender.Buffer, 0, sender.Buffer.Length, SocketFlags.None, new AsyncCallback(this.AsyncReceive), sender);

                this.Connection.BeginAccept(new AsyncCallback(this.AsyncConnect), null);
            }
            catch (SocketException e)
            {
                Program.SaveException(e);
                Console.WriteLine(e);
                if (this.enabled)
                {
                    this.Connection.BeginAccept(new AsyncCallback(this.AsyncConnect), null);
                }
            }
            catch (ObjectDisposedException e)
            {
                Program.SaveException(e);
                Console.WriteLine(e);
            }
        }

        private unsafe void AsyncReceive(IAsyncResult res)
        {
            bool was = false;
            try
            {
                SocketError error;
                AsyncSocketWrapper asyncState = (AsyncSocketWrapper)res.AsyncState;
                int RecvSize = asyncState.Socket.EndReceive(res, out error);
                if ((error == SocketError.Success) && (RecvSize > 0))
                {
                    was = true;
                    byte[] buffer = new byte[RecvSize];
                    for (int i = 0; i < RecvSize; i++)
                    {
                        buffer[i] = asyncState.Buffer[i];
                    }
                    if (this.OnClientReceive != null)
                    {
                        this.OnClientReceive(buffer, asyncState);
                    }
                    asyncState.Socket.BeginReceive(asyncState.Buffer, 0, asyncState.Buffer.Length, SocketFlags.None, new AsyncCallback(this.AsyncReceive), asyncState);
                }
                else
                {
                    this.InvokeDisconnect(asyncState);
                }
            }
            catch (SocketException e )
            {
                Program.SaveException(e);
                Console.WriteLine(e);
            }
            catch (ObjectDisposedException e )
            {
                Program.SaveException(e);
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Program.SaveException(e);
                if (was)
                {
                    AsyncSocketWrapper asyncState = (AsyncSocketWrapper)res.AsyncState;
                    asyncState.Socket.BeginReceive(asyncState.Buffer, 0, asyncState.Buffer.Length, SocketFlags.None, new AsyncCallback(this.AsyncReceive), asyncState);
                }
            }
        }

        public void Disable()
        {
            if (this.enabled)
            {
                this.Connection.Disable();
                this.enabled = false;
            }
        }

        public void Enable()
        {
            if (!this.enabled)
            {
                this.Connection.Bind(new IPEndPoint(IPAddress.Any, this.port));
                this.Connection.Listen(this.backlog);
                this.Connection.BeginAccept(new AsyncCallback(this.AsyncConnect), null);
                this.enabled = true;
            }
        }

        private void enabledCheck(string Variable)
        {
            if (this.enabled)
            {
                throw new Exception("Cannot modify " + Variable + " while socket is enabled.");
            }
        }

        public void InvokeDisconnect(AsyncSocketWrapper Client)
        {
            if (Client != null)
            {
                try
                {
                    if (Client.Socket.Connected)
                    {
                        Client.Socket.Shutdown(SocketShutdown.Both);
                        Client.Socket.Close();
                        if (this.OnClientDisconnect != null)
                        {
                            this.OnClientDisconnect(Client);
                        }
                        Client.Connector = null;
                        Client = null;
                    }
                    else
                    {
                        if (this.OnClientDisconnect != null)
                        {
                            this.OnClientDisconnect(Client);
                        }
                        Client.Connector = null;
                        Client = null;
                    }
                }
                catch (ObjectDisposedException e)
                {
                    Program.SaveException(e);
                    Console.WriteLine(e);
                }
            }
        }

        public int Backlog
        {
            get
            {
                return this.backlog;
            }
            set
            {
                this.enabledCheck("Backlog");
                this.backlog = value;
            }
        }

        public int ClientBufferSize
        {
            get
            {
                return this.clientbuffersize;
            }
            set
            {
                this.enabledCheck("ClientBufferSize");
                this.clientbuffersize = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
        }

        public ushort Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.enabledCheck("Port");
                this.port = value;
            }
        }
    }
}
