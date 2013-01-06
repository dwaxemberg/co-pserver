

namespace Conquer_Online_Server
{
    using System;
    using System.Collections.Generic;
    using Conquer_Online_Server;

    public class ManagedThreadTimer
    {
        public int Time, OnHold;
        public DateTime T;
        public System.Threading.TimerCallback Callback;
        public object Obj;
        public ManagedThreadPool Pool;
        public ManagedThreadTimer(int onhold, int time, System.Threading.TimerCallback callback, ManagedThreadPool pool, object obj)
        {
            T = DateTime.Now;
            OnHold = onhold;
            Obj = obj;
            Time = time;
            Callback = callback;
            Pool = pool;
            pool.Add(this);
        }

        public void Execute(DateTime now)
        {
            if (OnHold != 0)
            {
                if (now > T.AddMilliseconds(OnHold))
                {
                    OnHold = 0;
                }
                else return;
            }
            if (now > T.AddMilliseconds(Time))
            {
                if (Callback != null)
                {
                    T = now;
                    Callback.Invoke(Obj);
                }
            }
        }

        public void Dispose()
        {
            Pool.Remove(this);
        }

    }

    public class ManagedThreadPoolChecker
    {
        public Queue<ManagedThreadPool> ToBeAdded;
        public Queue<ManagedThreadPool> ToBeRemoved;
        public List<ManagedThreadPool> CurrentlyUsed;
        private ServerBase.Thread thread;
        private object loker = null;
        public ManagedThreadPoolChecker()
        {
            loker = new object();
            ToBeAdded = new Queue<ManagedThreadPool>();
            ToBeRemoved = new Queue<ManagedThreadPool>();
            CurrentlyUsed = new List<ManagedThreadPool>();
        }
        public static DateTime timer;
        public void StartPool()
        {
            if (thread != null)
                if (!thread.Closed) 
                thread.Closed = true;
            thread = new ServerBase.Thread(1000);
            thread.Execute += new Action(Checker);
            thread.Start();
        }
        private void Checker()
        {
            timer = DateTime.Now;
            DateTime now = DateTime.Now;
            try
            {
                while (ToBeAdded.Count != 0)
                    CurrentlyUsed.Add(ToBeAdded.Dequeue());

                while (ToBeRemoved.Count != 0)
                    CurrentlyUsed.Remove(ToBeRemoved.Dequeue());

                for (int i = 0; i < CurrentlyUsed.Count; i++)
                    CurrentlyUsed[i].Check(now);
            }
            catch { Console.WriteLine("EROR THREAD"); }
        }

        public void Add(ManagedThreadPool pool)
        {
            lock (loker)
            {
                ToBeAdded.Enqueue(pool);
            }
        }

        public void Remove(ManagedThreadPool pool)
        {
            lock (loker)
            {
                ToBeRemoved.Enqueue(pool);
            }
        }
    }

    public class ManagedThreadPool
    {
        public DateTime LastGoThrough;
        public Queue<ManagedThreadTimer> ToBeAdded;
        public Queue<ManagedThreadTimer> ToBeRemoved;
        public List<ManagedThreadTimer> CurrentlyUsed;
        private ServerBase.Thread thread;

        public ManagedThreadPool()
        {
            ToBeAdded = new Queue<ManagedThreadTimer>();
            ToBeRemoved = new Queue<ManagedThreadTimer>();
            CurrentlyUsed = new List<ManagedThreadTimer>();
            thread = new ServerBase.Thread(100);
            thread.Execute += new Action(thread_Execute);
            thread.Start();
        }

        public void Check(DateTime now)
        {
            if (now > LastGoThrough.AddMilliseconds(3000))
            {
                if (thread != null)
                    thread.Closed = true;
                thread = new ServerBase.Thread(100);
                thread.Execute += new Action(thread_Execute);
                thread.Start();
            }
        }

        void thread_Execute()
        {
            DateTime time = DateTime.Now;
            try
            {
                time = DateTime.Now;
                LastGoThrough = time;
                while (ToBeAdded.Count != 0)
                    CurrentlyUsed.Add(ToBeAdded.Dequeue());
                while (ToBeRemoved.Count != 0)
                    CurrentlyUsed.Remove(ToBeRemoved.Dequeue());
                for (int i = 0; i < CurrentlyUsed.Count; i++)
                        CurrentlyUsed[i].Execute(time);
            }
            catch { Console.WriteLine("EROR THREAD CLIENT"); }
        }

        public void Add(ManagedThreadTimer timer)
        {
            Console.WriteLine("INSERT NEW OBJECT IN THREAD");
            ToBeAdded.Enqueue(timer);

        }

        public void Remove(ManagedThreadTimer timer)
        {
            Console.WriteLine("REMOVE OBJECT FROM THREAD");
            ToBeRemoved.Enqueue(timer);

        }
    }
}
