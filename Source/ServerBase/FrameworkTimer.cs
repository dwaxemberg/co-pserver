using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security;
using System.Diagnostics;
using Amib.Threading.Internal;

namespace Conquer_Online_Server.ServerBase
{
    public enum PoleTyp
    {
        Add,
        Remove
    }
    public class FrameworkTimer
    {

        private static int Pools = 0;
        public static void UpgradePool(PoleTyp typ)
        {
            switch (typ)
            {
                case PoleTyp.Add:
                    {
                        if (Pools > 300)
                        {

                        }
                        break;
                    }
                case PoleTyp.Remove:
                    {

                        break;
                    }
            }
        }
        private static bool _isDisposed;
        public static void ShowThreadStats()
        {
            // Retrieve maximum number of thread pool threads
            int workerThreads;
            int completionThreads;
            System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out completionThreads);
            System.Console.WriteLine("Max worker threads={0}\nMax I/O completion threads={1}",
             workerThreads, completionThreads);


            // Retrieve minimum idle threads
            System.Threading.ThreadPool.GetMinThreads(out workerThreads, out completionThreads);
           System.Console.WriteLine("Min worker threads={0}\nMin I/O completion threads={1}",
             workerThreads, completionThreads);

            // Show available threads
            System.Threading.ThreadPool.GetAvailableThreads(out workerThreads, out completionThreads);
            System.Console.WriteLine("Available worker threads={0}\nAvailable I/O completion threads={1}",
             workerThreads, completionThreads);
        }
        /// <summary>
        /// Force the SmartThreadPool to shutdown
        /// </summary>
        public static void Shutdown()
        {
            Shutdown(true, 0);
        }

        /// <summary>
        /// Force the SmartThreadPool to shutdown with timeout
        /// </summary>
        public static void Shutdown(bool forceAbort, TimeSpan timeout)
        {
            Shutdown(forceAbort, (int)timeout.TotalMilliseconds);
        }
        public static void Shutdown(bool forceAbort, int millisecondsTimeout)
        {
           // ValidateNotDisposed();

         

            Thread[] threads;
            lock (_workerThreads.SyncRoot)
            {
                // Shutdown the work items queue
                _workItemsQueue.Dispose();

                // Signal the threads to exit
                _shutdown = true;
                _shuttingDownEvent.Set();

                // Make a copy of the threads' references in the pool
                threads = new Thread[_workerThreads.Count];
                _workerThreads.Keys.CopyTo(threads, 0);
            }

            int millisecondsLeft = millisecondsTimeout;
            Stopwatch stopwatch = Stopwatch.StartNew();
            //DateTime start = DateTime.UtcNow;
            bool waitInfinitely = (Timeout.Infinite == millisecondsTimeout);
            bool timeout = false;

            // Each iteration we update the time left for the timeout.
            foreach (Thread thread in threads)
            {
                // Join don't work with negative numbers
                if (!waitInfinitely && (millisecondsLeft < 0))
                {
                    timeout = true;
                    break;
                }

                // Wait for the thread to terminate
                bool success = thread.Join(millisecondsLeft);
                if (!success)
                {
                    timeout = true;
                    break;
                }

                if (!waitInfinitely)
                {
                    // Update the time left to wait
                    //TimeSpan ts = DateTime.UtcNow - start;
                    millisecondsLeft = millisecondsTimeout - (int)stopwatch.ElapsedMilliseconds;
                }
            }

            if (timeout && forceAbort)
            {
                // Abort the threads in the pool
                foreach (Thread thread in threads)
                {

                    if ((thread != null)
#if !(_WINDOWS_CE)
 && thread.IsAlive
#endif
)
                    {
                        try
                        {
                            thread.Abort(); // Shutdown
                        }
                        catch (SecurityException e)
                        {
                            e.GetHashCode();
                        }
                        catch (ThreadStateException ex)
                        {
                            ex.GetHashCode();
                            // In case the thread has been terminated 
                            // after the check if it is alive.
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Empties the queue of work items and abort the threads in the pool.
        /// </summary>
        ///         private readonly SynchronizedDictionary<Thread, ThreadEntry> _workerThreads = new SynchronizedDictionary<Thread, ThreadEntry>();
        private static ManualResetEvent _shuttingDownEvent = EventWaitHandleFactory.CreateManualResetEvent(false);
        private static ManualResetEvent _isIdleWaitHandle = EventWaitHandleFactory.CreateManualResetEvent(true);
        private static readonly WorkItemsQueue _workItemsQueue = new WorkItemsQueue();
        private static readonly SynchronizedDictionary<Thread, ThreadEntry> _workerThreads = new SynchronizedDictionary<Thread, ThreadEntry>();
        #region IDisposable Members
        private static bool _shutdown;
        public static void Dispose()
        {
            if (!_isDisposed)
            {
                if (!_shutdown)
                {
                    Shutdown();
                }

                if (null != _shuttingDownEvent)
                {
                    _shuttingDownEvent.Close();
                    _shuttingDownEvent = null;
                }
                _workerThreads.Clear();

                if (null != _isIdleWaitHandle)
                {
                    _isIdleWaitHandle.Close();
                    _isIdleWaitHandle = null;
                }

                _isDisposed = true;
            }
        }

        private void ValidateNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString(), "The SmartThreadPool has been shutdown");
            }
        }
        #endregion
        public static void DoNothing(object state)
        {
            System.Threading.Thread.Sleep(1000);
            //ServerBase.FrameworkTimer.SetPole(10000, 5000);
            // Console.WriteLine("Doing nothing...");
        }
        public static void SetPole(int max_pole, int competition_port)
        {
            if (!System.Threading.ThreadPool.SetMinThreads(1, 2))
            {
                //Console.WriteLine("set min threads faill");
                return;
            }
            if (!System.Threading.ThreadPool.SetMaxThreads(max_pole, competition_port))
            {
                //Console.WriteLine("set max threads faill");
                return;
            }
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DoNothing));
            System.Threading.Thread.Sleep(10);

            // Show new thread stats
            ShowThreadStats();
        }
    }

}
