using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.ConquerStructures
{
   public class Quarantine
    {
        public static ThreadSafeDictionary<uint, Client.GameState> White = 
            new ThreadSafeDictionary<uint, Client.GameState>(50);
        public static ThreadSafeDictionary<uint, Client.GameState> Black =
            new ThreadSafeDictionary<uint, Client.GameState>(50);

        public static int BlackScore, WhiteScore = 0;
        public static bool Started = false;
        public static ushort Map = 1844;
        public static ushort X = 119;
        public static ushort Y = 159;


    }
}
