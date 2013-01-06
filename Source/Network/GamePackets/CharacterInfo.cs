using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class CharacterInfo : Writer, Interfaces.IPacket
    {
        Client.GameState client;
        public CharacterInfo(Client.GameState _client)
        {
            client = _client;
        }
        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        public byte[] ToArray()
        {
            byte[] Packet = new byte[120 + client.Entity.Spouse.Length + client.Entity.Name.Length];
            WriteUInt16((ushort)(Packet.Length - 8), 0, Packet);
            WriteUInt16(0x3ee, 2, Packet);
            WriteUInt32(client.Entity.UID, 4, Packet);
            WriteUInt32(client.Entity.Mesh, 10, Packet);
            WriteUInt16(client.Entity.HairStyle, 14, Packet);
            WriteUInt32(client.Entity.Money, 16, Packet);
            WriteUInt32((uint)client.Entity.ConquerPoints, 20, Packet);
            WriteUInt64(client.Entity.Experience, 24, Packet);
            WriteUInt16(client.Entity.Strength, 52, Packet);
            WriteUInt16(client.Entity.Agility, 54, Packet);
            WriteUInt16(client.Entity.Vitality, 56, Packet);
            WriteUInt16(client.Entity.Spirit, 58, Packet);
            WriteUInt16(client.Entity.Atributes, 60, Packet);
            WriteUInt16((ushort)client.Entity.Hitpoints, 62, Packet);
            WriteUInt16(client.Entity.Mana, 64, Packet);
            WriteUInt16(client.Entity.PKPoints, 66, Packet);
            Packet[68] = client.Entity.Level;
            Packet[69] = client.Entity.Class;
            Packet[70] = client.Entity.FirstRebornClass; /*Previous Reborn: shows up when u highlight the class in the status window*/
            Packet[71] = client.Entity.SecondRebornClass;
            Packet[73] = client.Entity.Reborn;
            WriteUInt32(client.Entity.QuizPoints, 75, Packet);
            WriteUInt16(client.Entity.EnlightenPoints, 79, Packet);
            WriteUInt16(0/*enlightened time left*/, 81, Packet);
            WriteUInt16(0/*enlightened time left*/, 77, Packet);
            WriteUInt16(0/*enlightened time left*/, 87, Packet);
            WriteUInt32(client.Entity.BoundCps, 89, Packet);
            WriteUInt32(client.Entity.BoundCps, 93, Packet);
            WriteUInt16(0/*enlightened time left*/, 81, Packet);
            // 77: Enlightened boolean
            // 87: Title activated
            // 89: BoundCPs
            // 93: Subclass Activated
            WriteUInt16(client.Entity.TitleActivated, 90, Packet);
            Packet[109] = 1;
            WriteByte(3, 110, Packet);
            WriteStringWithLength(client.Entity.Name, 111, Packet);
            WriteByte((byte)client.Entity.Name.Length, 111, Packet);
            WriteByte((byte)client.Entity.Spouse.Length, 113 + client.Entity.Name.Length, Packet);
            WriteString(client.Entity.Spouse, 114 + client.Entity.Name.Length, Packet);
            return Packet;
        }
        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }
    }
}
