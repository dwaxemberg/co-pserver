using System;
using System.Collections.Generic;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class _String : Writer, Interfaces.IPacket
    {
        public const byte GuildName = 3,
        Spouse = 6,
        Effect = 10,
        GuildList = 11,
        Unknown = 13,
        ViewEquipSpouse = 16,
        StartGamble = 17,
        EndGamble = 18,
        Sound = 20,
        GuildAllies = 21,
        GuildEnemies = 22,
        WhisperDetails = 26;

        byte[] Buffer;

        public _String(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[19];
                WriteUInt16(11, 0, Buffer);
                WriteUInt16(1015, 2, Buffer);
                Texts = new List<string>();
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public byte Type
        {
            get { return Buffer[8]; }
            set { Buffer[8] = value; }
        }
        public byte TextsCount
        {
            get { return Buffer[9]; }
            set { Buffer[9] = value; }
        }
        public List<string> Texts;

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
       
        public byte[] ToArray()
        {
            ushort entirelength = 19;
            foreach (string list in Texts)
                entirelength += (ushort)list.Length;
            byte[] buffer = new byte[entirelength];
            WriteUInt16((ushort)(entirelength - 8), 0, buffer);
            WriteUInt16(1015, 2, buffer);
            WriteUInt32(UID, 4, buffer);
            buffer[8] = Type;
            Buffer = buffer;
            WriteStringList(Texts, 9, Buffer);
            return Buffer;
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
            Texts = new List<string>(buffer[9]);
            ushort offset = 10;
            byte count = 0;
            while (count != TextsCount)
            {
                ushort textlength = buffer[offset]; offset++;
                string text = Encoding.ASCII.GetString(buffer, offset, textlength); offset += textlength;
                Texts.Add(text);
                count++;
            }            
        }
    }
}
