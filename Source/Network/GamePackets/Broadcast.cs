using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Broadcast : Writer, Interfaces.IPacket
    {
        public const byte
            ReleaseSoonMessages = 1,
            MyMessages = 2,
            BroadcastMessage = 3,
            Urgen15CPs = 4,
            Urgen5CPs = 5;

        byte[] Buffer;

        public Broadcast(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[16 + 8];
                WriteUInt16(16, 0, Buffer);
                WriteUInt16(2050, 2, Buffer);
            }
        }

        public byte Type
        {
            get { return Buffer[4]; }
            set { Buffer[4] = value; }
        }

        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public byte StringCount
        {
            get { return Buffer[12]; }
            set { Buffer[12] = value; }
        }

        public List<String> List = new List<string>();

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }

        public byte[] ToArray()
        {
            StringCount = (byte)List.Count;
            int stringlength = StringCount;
            foreach (string str in List)
                stringlength += str.Length;
            byte[] buffer = new byte[16 + 8 + stringlength];
            System.Buffer.BlockCopy(Buffer, 0, buffer, 0, 14);
            int offset = 13;
            foreach (string str in List)
            {
                buffer[offset] = (byte)str.Length; offset++;
                WriteString(str, offset, buffer);
                offset += str.Length;
            }
            Buffer = buffer;
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
            List = new List<string>(StringCount);
            int offset = 13;
            for (int c = 0; c < StringCount; c++)
            {
                string text = Encoding.ASCII.GetString(Buffer, offset + 1, Buffer[offset]);
                offset += text.Length + 1;
                List.Add(text);
            }
        }
    }
}
