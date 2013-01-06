using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Spell : Writer, Interfaces.IPacket, Interfaces.ISkill
    {
        byte[] Buffer;
        private byte _PreviousLevel;
        private bool _Available;
        public Spell(bool Create)
        {
            Buffer = new byte[24];
            WriteUInt16(16, 0, Buffer);
            WriteUInt16(1103, 2, Buffer);
            _Available = false;
        }
        public ushort ID
        {
            get { return (ushort)BitConverter.ToUInt16(Buffer, 8); }
            set { WriteUInt16(value, 8, Buffer); }
        }

        public byte Level
        {
            get { return (byte)BitConverter.ToUInt32(Buffer, 10); }
            set { WriteUInt16(value, 10, Buffer); }
        }
        
        public byte PreviousLevel
        {
            get { return _PreviousLevel; }
            set { _PreviousLevel = value; }
        }

        public uint Experience
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Send(Client.GameState client)
        {
            client.Send(this);
        }

        public bool Available
        {
            get
            {
                return _Available;
            }
            set
            {
                _Available = value;
            }
        }
    }
}
