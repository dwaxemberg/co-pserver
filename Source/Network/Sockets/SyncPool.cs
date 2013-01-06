using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.Sockets
{
    public class SyncPool
    {
        private Queue<SyncSocketWrapper> OnAdd;
        private List<SyncSocketWrapper> OnExec;
        private ServerBase.Threads _thread;
        private SyncSocket Server;


        public void Add(SyncSocketWrapper war)
        {
            lock (OnAdd)
                OnAdd.Enqueue(war);
        }
        public SyncPool(int size,SyncSocket server_)
        {
            Server = server_;
            OnAdd = new Queue<SyncSocketWrapper>(size);
            OnExec = new List<SyncSocketWrapper>(size);
            StartThread();
        }

        public void StartThread()
        {
            _thread = new Conquer_Online_Server.ServerBase.Threads(1);
            _thread.Execute += new Action(Pole);
            _thread.Start();
        }
        public void Pole()
        {
            try
            {
                while (OnAdd.Count != 0)
                    OnExec.Add(OnAdd.Dequeue());

                for (int x = 0; x < OnExec.Count; x++)
                {
                    if (x >= OnExec.Count) break;
                    if (OnExec[x] == null)
                    {
                        OnExec.Remove(OnExec[x]);
                        continue;
                    }
                    if (OnExec[x].Socket.Connected)
                    {
                        if (OnExec[x].Socket.Pool(0, System.Net.Sockets.SelectMode.SelectRead))
                        {
                            try
                            {
                                int RecvSize = 0;
                                if (OnExec[x].Socket.Connected)
                                    RecvSize = OnExec[x].Socket.Receive(OnExec[x].Buffer, OnExec[x].BufferSize, System.Net.Sockets.SocketFlags.None);
                                else
                                {
                                    OnExec.Remove(OnExec[x]);
                                }
                                if (RecvSize > 0)
                                {
                                    byte[] buffer = new byte[RecvSize];
                                    for (int i = 0; i < RecvSize; i++)
                                    {
                                        buffer[i] = OnExec[x].Buffer[i];
                                    }
                                    Server.InvokeOnClientReceive(OnExec[x], buffer);
                                }
                                else
                                {
                                    Server.InvokeDisconnect(OnExec[x]);
                                }
                            }
                            catch
                            {
                                Server.InvokeDisconnect(OnExec[x]);
                            }
                        }
                    }
                    else
                    {
                        OnExec.Remove(OnExec[x]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
