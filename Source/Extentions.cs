using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server
{
    public static class Extentions
    {
        public static void SafeAdd<T, T2>(this Dictionary<T, T2> dictionary, T Key, T2 Value)
        {
            lock (dictionary)
            {
                if (!dictionary.ContainsKey(Key))
                    dictionary.Add(Key, Value);
            }
        }
        public static void SafeRemove<T, T2>(this Dictionary<T, T2> dictionary, T Key)
        {
            lock (dictionary)
            {
                if (dictionary.ContainsKey(Key))
                    dictionary.Remove(Key);
            }
        }
        public static T2[] SafeValueArray<T, T2>(this Dictionary<T, T2> dictionary)
        {
            lock (dictionary)
            {
                T2[] Values = new T2[dictionary.Count];
                dictionary.Values.CopyTo(Values, 0);
                return Values;
            }
        }
    }
}
