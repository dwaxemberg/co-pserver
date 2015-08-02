using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Client
{
    public class PacketFilter : IEnumerable<int>
    {
        private struct PacketEntry
        {
            public int Time;
            public int Count;
        }

        public const int Timelimit = 500; //clear every 500 ms
        public const int PacketLimit = 8;  //must be more than 1
        
        private ConcurrentDictionary<int, int> Limits;
        private ConcurrentDictionary<int, PacketEntry> Entries;
        private object SyncRoot;

        public PacketFilter()
        {
            Limits = new ConcurrentDictionary<int, int>();
            Entries = new ConcurrentDictionary<int, PacketEntry>();
            SyncRoot = new object();
        }

        /// <summary>
        /// Add packet limits
        /// </summary>
        public void Add(int id, int value)
        {
            Limits[id] = value;
        }

        public bool Filter(int id)
        {
            lock (SyncRoot)
            {
                int time = Time64.Now.GetHashCode();
                PacketEntry filter;
                if (!Entries.TryGetValue(id, out filter))
                {
                    Entries.TryAdd(id, filter = new PacketEntry()
                    {
                        Count = 1,
                        Time = time
                    });
                    return false;
                }
                if (time - filter.Time > PacketFilter.Timelimit)
                {
                    filter.Time = time;
                    filter.Count = 0;
                }
                filter.Count++;
                Entries[id] = filter;
                return (filter.Count > Limit(id));
            }
        }

        private int Limit(int id)
        {
            if (Limits.ContainsKey(id)) return Limits[id];
            return PacketLimit;
        }

        #region Useless
        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
