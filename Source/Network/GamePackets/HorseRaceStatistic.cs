using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class HorseRaceStatistic : Writer
    {
        byte[] Buffer;
        public HorseRaceStatistic(bool Create)
        {
            Buffer = new byte[60];
            WriteUInt16(52, 0, Buffer);
            WriteUInt16(2209, 2, Buffer);
        }

        public string Name
        {
            get;
            set;
        }

        public uint EntityID
        {
            get;
            set;
        }

        public uint Rank
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint CurrentPts
        {
            get { return BitConverter.ToUInt32(Buffer, 36); }
            set { WriteUInt32(value, 36, Buffer); }
        }
    }
}