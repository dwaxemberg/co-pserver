using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Weather : Writer, Interfaces.IPacket
    {
        public const uint Nothing = 1,
                          Rain = 2,
                          Snow = 3,
                          RainWind = 4,
                          AutumnLeaves = 5,
                          CherryBlossomPetals = 7,
                          CherryBlossomPetalsWind = 8,
                          BlowingCotten = 9,
                          Atoms = 10;

        byte[] Buffer;

        public Weather(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[20 + 8];
                WriteUInt16(20, 0, Buffer);
                WriteUInt16(1016, 2, Buffer);
            }
        }

        public uint WeatherType
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint Intensity
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint Direction
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint Appearence
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
