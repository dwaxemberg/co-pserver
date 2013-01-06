using OpenSSL;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Conquer_Online_Server.Network.Sockets
{
    public class SyncSocket
    {
        private WinSocket Connection = new WinSocket();
        private bool enabled;
        public event Action<Interfaces.ISocketWrapper> OnClientConnect;
        public event Action<Interfaces.ISocketWrapper> OnClientDisconnect;
        public event Action<byte[], Interfaces.ISocketWrapper> OnClientReceive;
        private ushort port;
        private Thread mainThread;

        public SyncSocket(ushort port)
        {
            if (!this.enabled)
            {
                this.port = port;
                this.Connection.Bind(new IPEndPoint(IPAddress.Any, this.port));
                this.Connection.Listen(10);
                this.enabled = true;
                mainThread = new Thread(new ThreadStart(this.SyncConnect));
                mainThread.Start();
            }
        }

        private void SyncConnect()
        {
            try
            {
                while (true)
                {
                    if (Connection.Disabled)
                        return;
                    if (!this.enabled)
                        return;
                    SyncSocketWrapper sender = null;
                    try
                    {
                        sender = new SyncSocketWrapper();
                        sender.Create(this.Connection.Accept());
                        if (this.OnClientConnect != null)
                        {
                            this.OnClientConnect(sender);
                        }
                        sender.BeginReceive(this);
                    }
                    catch (SocketException e)
                    {
                        Program.SaveException(e);
                        Console.WriteLine(e);

                    }
                    catch (ObjectDisposedException e)
                    {
                        Program.SaveException(e);
                        Console.WriteLine(e);
                    }
                    catch (Exception e) { Program.SaveException(e); }
                }
            }
            catch (ThreadAbortException e )
            {
                Program.SaveException(e);
                Console.WriteLine(e);
            }
        }

        public void InvokeOnClientConnect(SyncSocketWrapper sender)
        {
            if (this.OnClientConnect != null)
            {
                this.OnClientConnect(sender);
            }
        }

        public void InvokeOnClientReceive(SyncSocketWrapper sender, byte[] buffer)
        {
            if (this.OnClientReceive != null)
            {
                this.OnClientReceive(buffer, sender);
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
                this.Connection.Listen(100);
                this.enabled = true;
                if (mainThread != null)
                {
                    mainThread.Abort();
                    mainThread = null;
                }
                mainThread = new Thread(new ThreadStart(this.SyncConnect));
                mainThread.Start();
            }
        }

        private void enabledCheck(string Variable)
        {
            if (this.enabled)
            {
                throw new Exception("Cannot modify " + Variable + " while socket is enabled.");
            }
        }

        public void InvokeDisconnect(SyncSocketWrapper Client)
        {
            if (Client != null)
            {
                try
                {
                    if (Client.Socket.Connected)
                    {
                        Client.Socket.Disconnect(false);
                        Client.Socket.Shutdown(SocketShutdown.Both);
                        Client.Socket.Close();
                        if (this.OnClientDisconnect != null)
                            this.OnClientDisconnect(Client);
                        Client.Connector = null;
                        Client = null;
                    }
                    else
                    {
                        Client.Socket.Shutdown(SocketShutdown.Both);
                        Client.Socket.Close();
                        if (this.OnClientDisconnect != null)
                            this.OnClientDisconnect(Client);
                        
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
