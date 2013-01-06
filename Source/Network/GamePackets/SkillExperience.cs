using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class SkillExperience : Writer, Interfaces.IPacket
    {
        byte[] Buffer;
        public SkillExperience(bool Create)
        {
            Buffer = new byte[24];
            WriteUInt16(16, 0, Buffer);
            WriteUInt16(1104, 2, Buffer);
        }

        public uint Experience
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint NeedExperience
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public ushort ID
        {
            get { return (ushort)BitConverter.ToUInt16(Buffer, 12); }
            set { WriteUInt16(value, 12, Buffer); }
        }

        public ushort IsSkill
        {
            get { return (byte)BitConverter.ToUInt16(Buffer, 14); }
            set { WriteUInt16(value, 14, Buffer); }
        }
        public void AppendSpell(ushort ID, uint Experience)
        {
            this.Experience = Experience;
            this.ID = ID;
            this.IsSkill = 1;
        }

        public void AppendProficiency(ushort ID, uint Experience, uint NeedExperience)
        {
            this.ID = ID;
            this.Experience = Experience;
            this.NeedExperience = NeedExperience;
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
    }
}
