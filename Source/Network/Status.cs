using System;

namespace Conquer_Online_Server.Network.GameBufferets
{
    public class Status : Writer, Interfaces.IPacket
    {
        Client.GameState client;
        byte[] Buffer;
        public Status(Client.GameState _client)
        {
            client = _client;

        }
        public byte[] ToArray()
        {
            Buffer = new byte[144];
            WriteUInt16(144 - 8, 0, Buffer);
            WriteUInt16(1040, 2, Buffer);
            Writer.WriteUInt32(client.Entity.UID, 4, Buffer);
            Writer.WriteUInt32(client.Entity.Hitpoints, 8, Buffer);
            Writer.WriteUInt32(client.Entity.Mana, 12, Buffer);
            Writer.WriteUInt32(client.Entity.MinAttack, 20, Buffer);
            Writer.WriteUInt32(client.Entity.MaxAttack, 16, Buffer);
            Writer.WriteUInt32(client.Entity.Defence, 24, Buffer);
            Writer.WriteUInt32(client.Entity.MagicAttack, 28, Buffer);
            Writer.WriteUInt32((uint)client.Entity.MagicDefence, 32, Buffer);
            Writer.WriteUInt32(client.Entity.Dodge, 36, Buffer);
            Writer.WriteUInt32(client.Entity.Agility, 40, Buffer);
            Writer.WriteUInt32(0, 44, Buffer);//Accuracy
            Writer.WriteUInt32((uint)(client.Entity.Gems[1]), 48, Buffer);
            Writer.WriteUInt32((uint)(client.Entity.Gems[0]), 52, Buffer);
            Writer.WriteUInt32((uint)client.Entity.MagicDefencePercent, 56, Buffer);
            Writer.WriteUInt32((uint)client.Entity.Gems[7], 60, Buffer);
            Writer.WriteUInt32((uint)client.Entity.ItemBless, 64, Buffer);


            Writer.WriteUInt32((uint)client.Entity.Statistics.CriticalStrike, 68, Buffer);// CriticalStrike
            Writer.WriteUInt32((uint)client.Entity.Statistics.SkillCStrike, 72, Buffer);// SkillCStrike
            Writer.WriteUInt32((uint)client.Entity.Statistics.Immunity, 76, Buffer);// Immunity
            Writer.WriteUInt32((uint)(client.Entity.Statistics.Penetration), 80, Buffer);// Immunity
            Writer.WriteUInt32((uint)client.Entity.Statistics.Block, 84, Buffer);// Penetration
            Writer.WriteUInt32((uint)client.Entity.Statistics.Breaktrough, 88, Buffer);// Block
            Writer.WriteUInt32(client.Entity.Statistics.Counteraction, 92, Buffer);
            Writer.WriteUInt32((uint)(client.Entity.Statistics.Detoxication), 96, Buffer);
            //Writer.WriteUInt32((uint)client.Entity.Statistics.CriticalStrike, 68, Buffer);// CriticalStrike
            //Writer.WriteUInt32((uint)client.Entity.Statistics.SkillCStrike, 72, Buffer);// SkillCStrike
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Immunity, 76, Buffer);// Immunity
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Penetration, 80, Buffer);// Immunity
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Block, 84, Buffer);// blok
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Breaktrough, 88, Buffer);// Breaktrough
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Counteraction, 92, Buffer);
            //Writer.WriteUInt32((uint)client.Entity.Statistics.Detoxication, 96, Buffer);
            Writer.WriteUInt32((uint)(client.Entity.getFan(false)), 100, Buffer);
            Writer.WriteUInt32((uint)(client.Entity.getFan(true)), 104, Buffer);
            Writer.WriteUInt32((uint)(client.Entity.getTower(false)), 108, Buffer);
            Writer.WriteUInt32((uint)(client.Entity.getTower(true)), 112, Buffer);

            Writer.WriteUInt32((uint)client.Entity.Statistics.MetalResistance, 116, Buffer);
            Writer.WriteUInt32((uint)client.Entity.Statistics.WoodResistance, 120, Buffer);
            Writer.WriteUInt32((uint)client.Entity.Statistics.WaterResistance, 124, Buffer);
            Writer.WriteUInt32((uint)client.Entity.Statistics.FireResistance, 128, Buffer);
            Writer.WriteUInt32((uint)client.Entity.Statistics.EarthResistance, 132, Buffer);
            return Buffer;
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }
    }
}