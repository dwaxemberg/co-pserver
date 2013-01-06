using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class SafeDictionary<T1, T2>
    {
        private Dictionary<T1, T2> DictBase;

        public SafeDictionary(int capacity)
        {
            DictBase = new Dictionary<T1, T2>(capacity);
        }
        public SafeDictionary()
        {
            DictBase = new Dictionary<T1, T2>();
        }
        public Dictionary<T1, T2> Base
        {
            get
            {
                return DictBase;
            }
        }
        public int Count
        {
            get
            {
                return DictBase.Count;
            }
        }

        public bool Add(T1 key, T2 value)
        {
            if (!DictBase.ContainsKey(key))
            {
                DictBase.Add(key, value);
                return true;
            }
            return false;
        }

        public void Remove(T1 key)
        {
            DictBase.Remove(key);
        }

        public bool ContainsKey(T1 key)
        {
            return DictBase.ContainsKey(key);
        }

        public bool ContainsValue(T2 value)
        {
            return DictBase.ContainsValue(value);
        }

        public void Clear()
        {
            DictBase.Clear();
        }

        public T2 this[T1 key]
        {
            get
            {
                if (ContainsKey(key))
                    return DictBase[key];
                else return default(T2);
            }
        }

        public Dictionary<T1, T2>.ValueCollection Values
        {
            get
            {
                if (DictBase == null)
                    DictBase = new Dictionary<T1, T2>();
                return DictBase.Values;
            }
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return DictBase.TryGetValue(key, out value);
        }
    }
}
