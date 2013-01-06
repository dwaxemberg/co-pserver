using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.ConquerStructures.Society
{
    public class Apprentice : Interfaces.IKnownPerson
    {
        public uint ID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsOnline
        {
            get
            {
                return ServerBase.Kernel.GamePool.ContainsKey(ID);
            }
        }
        public Client.GameState Client
        {
            get
            {
                return ServerBase.Kernel.GamePool[ID];
            }
        }
        public uint EnroleDate
        {
            get;
            set;
        }

        public ulong Actual_Experience
        {
            get;
            set;
        }

        public ulong Total_Experience
        {
            get;
            set;
        }

        public ushort Actual_Plus
        {
            get;
            set;
        }

        public ushort Total_Plus
        {
            get;
            set;
        }

        public ushort Actual_HeavenBlessing
        {
            get;
            set;
        }

        public ushort Total_HeavenBlessing
        {
            get;
            set;
        }
    }
}
