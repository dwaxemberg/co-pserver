using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class NobilityInfo : Writer, Interfaces.IPacket
    {
        public const uint
            Donate = 1,
            List = 2,
            Icon = 3,
            NextRank = 4;

        byte[] Buffer;

        public NobilityInfo(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[33 + 8];
                WriteUInt16(33, 0, Buffer);
                WriteUInt16(2064, 2, Buffer);
            }
            Strings = new List<string>();
        }

        public uint Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }


        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public ushort wParam1
        {
            get { return BitConverter.ToUInt16(Buffer, 8); }
            set { WriteUInt16(value, 8, Buffer); }
        }

        public ushort wParam2
        {
            get { return BitConverter.ToUInt16(Buffer, 10); }
            set { WriteUInt16(value, 10, Buffer); }
        }

        public uint dwParam2
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public uint dwParam3
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }

        public uint dwParam4
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }
        //Thanks to ImmuneOne, who fixed the strings offsets, I managed to get nobility done.
        public byte StringCount
        {
            get { return Buffer[32]; }
            set { Buffer[32] = value; }
        }

        public List<string> DecodedStrings
        {
            get
            {
                List<string> list = new List<string>(StringCount);
                int offset = 33;
                for (int count = 0; count < StringCount; count++)
                {
                    byte stringLength = Buffer[offset]; offset++;
                    string String = Encoding.ASCII.GetString(Buffer, offset, stringLength);
                    offset += stringLength;
                    list.Add(String);
                }
                return list;
            }
        }

        public List<string> Strings;

        public void UpdateString(Game.ConquerStructures.NobilityInformation info)
        {
            string buildString = info.EntityUID + " " + info.Donation + " " + (byte)info.Rank + " " + info.Position;
            buildString = (char)buildString.Length + buildString;
            Strings.Add(buildString);
        }

        public void ListString(Game.ConquerStructures.NobilityInformation info)
        {
            string buildString = info.EntityUID + " " + info.Gender + " 0 " + info.Name + " " + info.Donation + " " + (byte)info.Rank + " " + info.Position;
            buildString = (char)buildString.Length + buildString;
            Strings.Add(buildString);
        }

        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            if (Strings.Count == 0)
                return Buffer;
            string theString = "";
            for (int count = 0; count < Strings.Count; count++)
            {
                theString += Strings[count];
            }
            byte[] newBuffer = new byte[33 + 8 + theString.Length];
            Buffer.CopyTo(newBuffer, 0);
            WriteUInt16((ushort)(newBuffer.Length - 8), 0, newBuffer);
            newBuffer[32] = (byte)Strings.Count;
            WriteString(theString, 33, newBuffer);
            return newBuffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
