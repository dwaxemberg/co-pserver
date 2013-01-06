using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Conquer_Online_Server.Network
{
    public class HandlerPool
    {
        PacketPool PacketPooler;

        public HandlerPool()
        {
            PacketPooler = new PacketPool(5);
        }

        public void QueuePacket(Client.PacketClient Data)
        {
            PacketPooler.QueueTask(Data);
        }
    }

    public sealed class PacketPool : IDisposable
    {
        private LinkedList<Thread> workers;
        private LinkedList<Client.PacketClient> tasks;
        private bool disAllowAdd, disposed;

        public PacketPool(int size)
        {
            this.workers = new LinkedList<Thread>();
            this.tasks = new LinkedList<Client.PacketClient>();

            for (int i = 0; i < size; ++i)
            {
                Thread worker = new Thread(this.Worker) { Name = string.Concat("Worker ", i) };
                worker.Start();
                this.workers.AddLast(worker);
            }
        }

        public void Dispose()
        {
            bool waitForThreads = false;
            lock (this.tasks)
            {
                if (!this.disposed)
                {
                    GC.SuppressFinalize(this);

                    this.disAllowAdd = true;

                    while (this.tasks.Count > 0)
                        Monitor.Wait(this.tasks);

                    this.disposed = true;
                    Monitor.PulseAll(this.tasks);
                    waitForThreads = true;
                }
            }
            if (waitForThreads)
            {
                foreach (Thread worker in workers)
                    worker.Join();
            }
        }

        public void QueueTask(Client.PacketClient Data)
        {
            lock (this.tasks)
            {
                if (this.disAllowAdd || this.disposed)
                    return;
                this.tasks.AddLast(Data);
                Monitor.PulseAll(this.tasks);
            }
        }

        private void Worker()
        {
            Client.PacketClient Data = new Client.PacketClient();
            while (true)
            {
                lock (this.tasks)
                {
                    if (this.disposed)
                        return;
                    if (null != Data.Buffer)
                    {
                        this.workers.AddLast(Thread.CurrentThread); Data.Buffer = null;
                    }
                    if (null != this.workers.First && object.ReferenceEquals(Thread.CurrentThread, this.workers.First.Value))
                        if (this.tasks.Count > 0)
                        {
                            Data = this.tasks.First.Value;
                            this.tasks.RemoveFirst();
                            this.workers.RemoveFirst();
                            Monitor.PulseAll(this.tasks);
                        }
                    if (Data.Buffer == null)
                    {
                        Monitor.Wait(this.tasks); continue;
                    }
                }
                //PacketHandler.SwitchPackets(Data);
            }
        }
    }
}
