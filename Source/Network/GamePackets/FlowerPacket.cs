using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class FlowerPacket : Writer
    {
        private byte[] Buffer;
        public FlowerPacket(Game.Struct.Flowers Flower)
        {
            Buffer = new byte[8 + 56];
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(1150, 2, Buffer);
            Buffer[4] = 1;

            WriteUInt32((uint)Flower.RedRoses, 16, Buffer);
            WriteUInt32((uint)Flower.RedRoses2day, 20, Buffer);

            WriteUInt32((uint)Flower.Lilies, 24, Buffer);
            WriteUInt32((uint)Flower.Lilies2day, 28, Buffer);

            WriteUInt32((uint)Flower.Orchads, 32, Buffer);
            WriteUInt32((uint)Flower.Orchads2day, 36, Buffer);

            WriteUInt32((uint)Flower.Tulips, 40, Buffer);
            WriteUInt32((uint)Flower.Tulips2day, 44, Buffer);

        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
    }
}
