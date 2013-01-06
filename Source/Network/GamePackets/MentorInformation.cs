using System;
using System.Text;
using System.Timers;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class MentorInformation : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;
        private string m_MentorName = "", m_MentorSpouse = "";
        private string m_ApprenticeName = "";

        public MentorInformation(bool create)
        {
            if (create)
            {
                Buffer = new byte[158];

                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(2066, 2, Buffer);
                WriteUInt32(999999, 24, Buffer);
            }
        }

        public uint Mentor_Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
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

        public uint Shared_Battle_Power
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set
            {
                if (value > 200)
                    value = 0;
                if (value < 0)
                    value = 0;
                WriteUInt32(value, 20, Buffer);
            }
        }

        public uint Enrole_Date
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { WriteUInt32(value, 28, Buffer); }
        }

        public uint Mentor_Level
        {
            get { return BitConverter.ToUInt32(Buffer, 32); }
            set { WriteUInt32(value, 32, Buffer); }
        }

        public byte Mentor_Class
        {
            get { return Buffer[33]; }
            set { Buffer[33] = value; }
        }

        public ushort Mentor_PkPoints
        {
            get { return BitConverter.ToUInt16(Buffer, 34); }
            set { WriteUInt16(value, 34, Buffer); }
        }

        public bool Mentor_Online
        {
            get { return (Buffer[56] == 1); }
            set { Buffer[56] = (byte)(value ? 1 : 0); }
        }

        public uint Apprentice_Experience
        {
            get { return BitConverter.ToUInt32(Buffer, 64); }
            set { WriteUInt32(value, 64, Buffer); }
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

        public byte String_Count
        {
            get { return Buffer[76]; }
            set { Buffer[76] = value; }
        }

        public string Mentor_Name
        {
            get { return m_MentorName; }
            set { 
            m_MentorName = value;
            WriteStringWithLength(value, 77, Buffer); }
        }

        public string Apprentice_Name
        {
            get { return m_ApprenticeName; }
            set {
                m_ApprenticeName = value;
                WriteStringWithLength(value, 81 + Mentor_Name.Length + Mentor_Spouse_Name.Length, Buffer);
            }
        }

        public string Mentor_Spouse_Name
        {
            get { return m_MentorSpouse; }
            set { 
                m_MentorSpouse = value;
                WriteStringWithLength(value, 79 + Mentor_Name.Length, Buffer);
            }
        }

        public void Null(int Offset)
        {
            Buffer[Offset] = 0;
        }

        public byte[] ToArray()
        {
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
