using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class GroundMovement:Writer, Interfaces.IPacket
    {
        public const uint Walk = 0,
                          Run = 1,
                          TwoCoordonates = 9;

        private byte[] Buffer;

        public GroundMovement(bool CreateInstance)
        {
            if (CreateInstance)
            {
                Buffer = new byte[32];
                WriteUInt32(24, 0, Buffer);
                WriteUInt32(10005, 2, Buffer);
            }
        }
        public byte[] ToArray()
        {
            return Buffer; 
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }

        public Conquer_Online_Server.Game.Enums.ConquerAngle Direction
        {
            get { return (Conquer_Online_Server.Game.Enums.ConquerAngle)(Buffer[4] % 8); }
            set { Buffer[4] = (byte)value; }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint GroundMovementType
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint TimeStamp
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public uint MapID
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
