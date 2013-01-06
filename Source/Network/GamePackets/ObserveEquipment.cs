using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ObserveStats : Writer, Interfaces.IPacket
    {
        Client.GameState client; //Emme
        public ObserveStats(Client.GameState _client)
        {
            client=_client;
        }
        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        public byte[] ToArray()
        {
            //client.Send(new GamePackets.CharacterInfo(client));
            byte[] Pack = new byte[144];//132
            Writer.WriteUInt16(144 - 8, 0, Pack);
            Writer.WriteUInt16(1040, 2, Pack);
            WriteUInt32(client.Entity.UID, 4, Pack);
            WriteUInt32(client.Entity.MaxHitpoints, 8, Pack);
            WriteUInt32(client.Entity.MaxMana, 12, Pack);
            WriteUInt32(client.Entity.MinAttack, 20, Pack);
            WriteUInt32(client.Entity.MaxAttack, 16, Pack);
            WriteUInt32(client.Entity.PhysicalDamageDecrease, 24, Pack);
            WriteUInt32(client.Entity.MagicDamageIncrease, 28, Pack);
            WriteUInt32((uint)(client.Entity.MagicDefencePercent + client.Entity.MagicDamageIncrease), 32, Pack);
            Writer.WriteUInt32(client.Entity.Dodge, 36, Pack);
            Writer.WriteUInt32(client.Entity.Agility, 40, Pack);
            Writer.WriteUInt32(0, 44, Pack);//Accuracy
            WriteUInt32((uint)(6 * 100), 48, Pack);
            Writer.WriteUInt32((uint)(client.Entity.Gems[0]), 52, Pack);
            Writer.WriteUInt32((uint)client.Entity.MagicDefencePercent, 56, Pack);
            Writer.WriteUInt32((uint)client.Entity.Gems[7], 60, Pack);
            WriteUInt32((uint)24, 64, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.CriticalStrike, 68, Pack);// CriticalStrike
            Writer.WriteUInt32((uint)client.Entity.Statistics.SkillCStrike, 72, Pack);// SkillCStrike
            Writer.WriteUInt32((uint)client.Entity.Statistics.Immunity, 76, Pack);// Immunity
            Writer.WriteUInt32((uint)client.Entity.Statistics.Immunity, 80, Pack);// Immunity
            Writer.WriteUInt32((uint)client.Entity.Statistics.Penetration, 84, Pack);// Penetration
            Writer.WriteUInt32((uint)client.Entity.Statistics.Block, 88, Pack);// Block
            Writer.WriteUInt32((uint)client.Entity.Statistics.Counteraction, 92, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.Detoxication, 96, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getFan(false)), 100, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getFan(true)), 104, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getTower(false)), 108, Pack);
            Writer.WriteUInt32((uint)(client.Entity.getTower(true)), 112, Pack);

            Writer.WriteUInt32((uint)client.Entity.Statistics.MetalResistance, 116, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.WoodResistance, 120, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.WaterResistance, 124, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.FireResistance, 128, Pack);
            Writer.WriteUInt32((uint)client.Entity.Statistics.EarthResistance, 132, Pack);

            return Pack;
        }
        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }
    }
}
