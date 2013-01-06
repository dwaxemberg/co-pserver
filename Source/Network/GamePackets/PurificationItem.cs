using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class PurificationItem : Writer
    {
        byte[] buffer;
        public byte[] ToArry()
        { return buffer; }
        public PurificationItem()
        {
            buffer = new byte[44];
            WriteUInt16(36, 0, buffer);
            WriteUInt16(2077, 2, buffer);
        }
        public byte tip
        {
            get { return buffer[12]; }
            set { buffer[12] = value; }
        }
        public uint Position
        {
            get { return BitConverter.ToUInt32(buffer, 4); }
            set { WriteUInt32(value, 4, buffer); }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(buffer, 8); }
            set { WriteUInt32(value, 8, buffer); }
        }
        public uint ID_Purification_Item
        {
            get { return BitConverter.ToUInt32(buffer, 16); }
            set { WriteUInt32(value, 16, buffer); }
        }
        public uint Level
        {
            get { return BitConverter.ToUInt32(buffer, 20); }
            set { WriteUInt32(value, 20, buffer); }
        }
        public uint Time
        {
            get { return BitConverter.ToUInt32(buffer, 28); }
            set { WriteUInt32(value, 28, buffer); }
        }
    }
}
