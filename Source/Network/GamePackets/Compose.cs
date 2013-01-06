using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Compose : Writer, Interfaces.IPacket
    {
        public const byte   Plus = 0,
                            CurrentSteed = 2,
                            NewSteed = 3;

        byte[] Buffer;

        public Compose(bool Create)
        {
            if (Create)
            {
                Buffer = null;
            }
        }

        public byte Mode
        {
            get { return Buffer[4]; }
            set { Buffer[4] = value; }
        }

        public uint ItemUID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint PlusItemUID
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
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
