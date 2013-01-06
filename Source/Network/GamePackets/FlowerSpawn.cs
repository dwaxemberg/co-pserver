using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class FlowerSpawn : Writer
    {
        byte[] Buffer;
        public FlowerSpawn(string Type, string name, string Flowers, string UID, uint FlowerID)
        {
            string send = Type + " " + Flowers + " " + UID + " " + UID + " " + name + " " + name + "";
            Buffer
                = new byte[88];//18
            WriteUInt16((byte)(80), 0, Buffer);
            WriteUInt16(1151, 2, Buffer);

            Buffer[4] = 2;
            WriteUInt32(FlowerID, 8, Buffer);
            Buffer[16] = 1;
            Buffer[24] = 1;
            Buffer[32] = 1;

            WriteUInt32(uint.Parse(UID), 40, Buffer);
            WriteUInt32(uint.Parse(UID), 44, Buffer);
            //Buffer[17] = 1;//13
            // Buffer[18] = (byte)(send.Length & 255);
            for (int i = 0; i < send.Length; i++)
            {
                try
                {
                    Buffer[48 + i] = Convert.ToByte(send[i]);
                    Buffer[48 + i + 16] = Convert.ToByte(send[i]);

                }
                catch { }
            }
        }
        public byte[] ThePacket()
        {
            return Buffer;
        }
    }
    public class aFlowerSpawn2 : Writer
    {
        byte[] Buffer;
        public aFlowerSpawn2(string UID)
        {
            string send = UID + " 1 0";
            Buffer
                = new byte[21 + send.Length + 8];
            WriteUInt16((byte)(21 + send.Length), 0, Buffer);
            WriteUInt16(1150, 2, Buffer);

            Buffer[4] = 4;
            Buffer[16] = 1;
            Buffer[17] = (byte)(send.Length & 255);
            for (int i = 0; i < send.Length; i++)
            {
                try
                {
                    Buffer[18 + i] = Convert.ToByte(send[i]);

                }
                catch { }
            }
        }
        public byte[] ThePacket()
        {
            return Buffer;
        }
    }
}
