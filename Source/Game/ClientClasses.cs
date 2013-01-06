using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game
{
    public enum ClassID : byte
    {
        MartialArtist = 1,
        Warlock = 2,
        ChiMaster = 3,
        Sage = 4,
        Apothecary = 5,
        Performer = 6,
        Wrangler = 9
        //Apothecary1 = 7,
       // Apothecary2 = 8,
       // Apothecary3 = 9,
       // Apothecary4 = 10,
       // Apothecary5 = 11,
       // Apothecary6 = 12

    }
    public class SubClass
    {
        public byte ID;
        public byte Phase;
        public byte Level;
    }
    public class ClientClasses
    {
        public Dictionary<byte, SubClass> Classes;
        public ulong StudyPoints;
        public byte Active;
        public ClientClasses()
        {
            Classes = new Dictionary<byte, SubClass>();
            StudyPoints = 0;
            Active = 0;
        }
    }
}
