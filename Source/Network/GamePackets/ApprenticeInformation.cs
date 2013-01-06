using System;
using System.Text;
using System.Timers;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ApprenticeInformation : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;
        private string m_MentorName = "", m_ApprenticeSpouse = "";
        private string m_ApprenticeName = "";

        public ApprenticeInformation()
        {
            Buffer = new byte[112];

            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(2066, 2, Buffer);
            WriteUInt32(999999, 24, Buffer);
        }

        public byte Type
        {
            get { return Buffer[4]; }
            set { Buffer[4] = value; }
        }

        public uint Mentor_ID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint Apprentice_ID
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint Mentor_Mesh
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public uint Enrole_date
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { WriteUInt32(value, 28, Buffer); }
        }

        public uint Apprentice_Level
        {
            get { return BitConverter.ToUInt32(Buffer, 32); }
            set { WriteUInt32(value, 32, Buffer); }
        }

        public byte Apprentice_Class
        {
            get { return Buffer[33]; }
            set { Buffer[33] = value; }
        }

        public ushort Apprentice_PkPoints
        {
            get { return BitConverter.ToUInt16(Buffer, 34); }
            set { WriteUInt16(value, 34, Buffer); }
        }

        public byte Static
        {
            get { return Buffer[52]; }
            set { Buffer[52] = value; }
        }

        public bool Apprentice_Online
        {
            get { return (Buffer[56] == 1); }
            set { Buffer[56] = (byte)(value ? 1 : 0); }
        }

        public ulong Apprentice_Experience
        {
            get { return BitConverter.ToUInt64(Buffer, 64); }
            set { WriteUInt64(value, 64, Buffer); }
        }

        public ushort Apprentice_Blessing
        {
            get { return BitConverter.ToUInt16(Buffer, 72); }
            set { WriteUInt16(value, 72, Buffer); }
        }

        public ushort Apprentice_Composing
        {
            get { return BitConverter.ToUInt16(Buffer, 74); }
            set { WriteUInt16(value, 74, Buffer); }
        }

        public string Mentor_Name
        {
            get { return m_MentorName; }
            set
            {
                m_MentorName = value;
            }
        }

        public string Apprentice_Name
        {
            get { return m_ApprenticeName; }
            set
            {
                m_ApprenticeName = value;
            }
        }

        public string Apprentice_Spouse_Name
        {
            get { return m_ApprenticeSpouse; }
            set
            {
                m_ApprenticeSpouse = value;
            }
        }

        public void Null(int Offset)
        {
            Buffer[Offset] = 0;
        }

        public byte[] ToArray()
        {
            WriteStringList(new System.Collections.Generic.List<string>() { Mentor_Name, Apprentice_Name, Apprentice_Spouse_Name }, 76, Buffer);
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }

        public void Send(Client.GameState client)
        {
           // client.Send(Buffer);
        }
    }
}
