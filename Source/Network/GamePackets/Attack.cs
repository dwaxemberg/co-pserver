using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Attack : Interfaces.IPacket
    {
        public const uint Melee = 2,
                          MarriageRequest = 8,
                          MarriageAccept = 9,
                          Kill = 14,
                          Magic = 24,
                          Reflect = 26,
                          Dash = 27,
                          Ranged = 28,
                          MonkMelee = 34,
                          MerchantAccept = 40,
                          MerchantRefuse = 41,
                          MerchantProgress = 42,
                          Scapegoat = 43,
                          CounterKillSwitch = 44,
                          FatalStrike = 45,
                          ShowUseSpell = 52,
                          InteractionRequest = 46,
                          InteractionAccept = 47,
                          InteractionRefuse = 48,
                          InteractionEffect = 49,
                          InteractionStopEffect = 50;


        byte[] Buffer;

        public Attack(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[48];
                Writer.WriteUInt16(40, 0, Buffer);
                Writer.WriteUInt16(1022, 2, Buffer);
            }
        }

        public uint Attacker
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { Writer.WriteUInt32(value, 8, Buffer); }
        }

        public uint Attacked
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { Writer.WriteUInt32(value, 12, Buffer); }
        }

        public ushort X
        {
            get { return BitConverter.ToUInt16(Buffer, 16); }
            set { Writer.WriteUInt16(value, 16, Buffer); }
        }

        public ushort Y
        {
            get { return BitConverter.ToUInt16(Buffer, 18); }
            set { Writer.WriteUInt16(value, 18, Buffer); }
        }

        public uint AttackType
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { Writer.WriteUInt32(value, 20, Buffer); }
        }

        public uint Damage
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { Writer.WriteUInt32(value, 24, Buffer); }
        }

        public ushort KOCount
        {
            get { return BitConverter.ToUInt16(Buffer, 26); }
            set { Writer.WriteUInt16(value, 26, Buffer); }
        }

        public uint ResponseDamage
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { Writer.WriteUInt32(value, 28, Buffer); }
        }
        public Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue FirstEffect
        {
            get { return (Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue)Buffer[32]; }
            set { Writer.WriteByte((byte)value, 32, Buffer); }
        }

        public Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue SecondEffect
        {
            get { return (Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue)Buffer[33]; }
            set { Writer.WriteByte((byte)value, 33, Buffer); }
        }
        public bool Decoded = false;

        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
