using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class TryTrip : Interfaces.IPacket
    {
        public class DamageClass
        {
            public uint Damage;
            public Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue First;
            public Conquer_Online_Server.Network.GamePackets.SpellUse.EffectValue Second;
            public bool Hit;

            public static implicit operator uint(DamageClass dmg)
            {
                return dmg.Damage;
            }
            public static implicit operator DamageClass(uint dmg)
            {
                return new DamageClass() { Damage = dmg, Hit = true };
            }
        }
        byte[] Buffer;

        public TryTrip(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[8 + 20];
                Writer.WriteUInt16(20, 0, Buffer);
                Writer.WriteUInt16(1105, 2, Buffer);
            }
        }

        public uint Attacker
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { Writer.WriteUInt32(value, 4, Buffer); }
        }
        public uint Attacked
        {

            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { Writer.WriteUInt32(value, 8, Buffer); }
        }

        public ushort SpellID
        {
            get { return BitConverter.ToUInt16(Buffer, 12); }
            set { Writer.WriteUInt16(value, 12, Buffer); }
        }

        public ushort SpellLevel
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { Writer.WriteUInt16(value, 14, Buffer); }
        }

        public Dictionary<uint, DamageClass> Targets = new Dictionary<uint, DamageClass>();

        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }
        public uint Damage
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { Writer.WriteUInt32(value, 24, Buffer); }

        }
        public byte[] ToArray()
        {
            byte[] buffer = new byte[156];
            Writer.WriteUInt16(148, 0, buffer);
            Writer.WriteUInt16(1105, 2, buffer);
            Writer.WriteUInt32(Attacker, 4, buffer);
            Writer.WriteUInt32(Attacked, 8, buffer);
            Writer.WriteUInt16(SpellID, 12, buffer);
            Writer.WriteUInt16(SpellLevel, 14, buffer);
            Writer.WriteUInt32(3, 16, buffer);
            // Writer.WriteUInt32((uint)Targets., 32, buffer);
            //Writer.WriteUInt32((uint)Attacked.Value.Second, 33, buffer);
            //  Writer.WriteUInt32((uint)Attacked , 33, buffer);
            // Writer.WriteUInt32((uint)Attacked , 32, buffer);
            Writer.WriteUInt32(Attacked, 20, buffer);
            Writer.WriteUInt32(Damage / 3, 24, buffer);
            Writer.WriteUInt32(Attacked, 52, buffer);
            Writer.WriteUInt32(Damage / 3, 56, buffer);
            Writer.WriteUInt32(Attacked, 84, buffer);
            Writer.WriteUInt32(Damage / 3, 88, buffer);

            foreach (KeyValuePair<uint, DamageClass> target in Targets)
            {

                Writer.WriteUInt32((uint)target.Value.First, 32, buffer);
                Writer.WriteUInt32((uint)target.Value.Second, 33, buffer);
            }
            return buffer;
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
    public class SpellUse : Interfaces.IPacket
    {
        public enum EffectValue : uint
        {
            None = 0,
            Block = 1,
            Penetration = 2,
            CriticalStrike = 4,
            CriticalBlock = 5,
            WoodResist = 40,
            MetalResist = 48,
            EarthResist = 1,
            StudyPoints = 2,
        }

        public class DamageClass
        {
            public uint Damage;
            public bool Hit;

            public static implicit operator uint(DamageClass dmg)
            {
                return dmg.Damage;
            }
            public static implicit operator DamageClass(uint dmg)
            {
                return new DamageClass() { Damage = dmg, Hit = true };
            }
        }
        byte[] Buffer;

        public SpellUse(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[8 + 20];
                Writer.WriteUInt16(20, 0, Buffer);
                Writer.WriteUInt16(1105, 2, Buffer);
            }
        }

        public uint Attacker
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { Writer.WriteUInt32(value, 4, Buffer); }
        }

        public ushort X
        {
            get { return BitConverter.ToUInt16(Buffer, 8); }
            set { Writer.WriteUInt16(value, 8, Buffer); }
        }

        public ushort Y
        {
            get { return BitConverter.ToUInt16(Buffer, 10); }
            set { Writer.WriteUInt16(value, 10, Buffer); }
        }

        public ushort SpellID
        {
            get { return BitConverter.ToUInt16(Buffer, 12); }
            set { Writer.WriteUInt16(value, 12, Buffer); }
        }

        public ushort SpellLevel
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { Writer.WriteUInt16(value, 14, Buffer); }
        }

        public SafeDictionary<uint, DamageClass> Targets = new SafeDictionary<uint, DamageClass>(40);
        public SafeDictionary<uint, EffectValue> Effect = new SafeDictionary<uint, EffectValue>(40);
        bool constC = false;
        public void MakeConst()
        {
            constC = true;
        }
        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] ToArray()
        {
            int count = 0;
            byte[] buffer = new byte[8 + 20 + 32 + Math.Min((uint)Targets.Count, (uint)30) * 32];
            Writer.WriteUInt16((ushort)(20 + 32 + Math.Min((uint)Targets.Count, (uint)30) * 32), 0, buffer);
            Writer.WriteUInt16(1105, 2, buffer);
            Writer.WriteUInt32(Attacker, 4, buffer);
            Writer.WriteUInt16(X, 8, buffer);
            Writer.WriteUInt16(Y, 10, buffer);
            Writer.WriteUInt16(SpellID, 12, buffer);
            Writer.WriteUInt16(SpellLevel, 14, buffer);
            Writer.WriteUInt32(Math.Min((uint)Targets.Count, (uint)30), 16, buffer);
            ushort offset = 20;
            uint uid = 0;
            foreach (KeyValuePair<uint, DamageClass> target in Targets.Base)
            {
                count++;
                if (count == 30)
                    break;
                if (constC)
                {
                    if (uid == 0)
                        uid = target.Key;
                    Writer.WriteUInt32(uid, offset, buffer);
                }
                else
                    Writer.WriteUInt32(target.Key, offset, buffer);
                offset += 4;
                Writer.WriteUInt32(target.Value.Damage, offset, buffer); offset += 4;
                Writer.WriteBoolean(target.Value.Hit, offset, buffer); offset += 4;
                if (Effect.ContainsKey(target.Key))
                    Writer.WriteByte((byte)Effect[target.Key], offset, buffer);
                offset += 4;
                offset += 16;

                //1
            }
            return buffer;
        }
        private byte[] toArray()
        {
            byte[] buffer = new byte[8 + 20 + 32 + Targets.Count * 32];
            Writer.WriteUInt16((ushort)(20 + 32 + Targets.Count * 32), 0, buffer);
            Writer.WriteUInt16(1105, 2, buffer);
            Writer.WriteUInt32(Attacker, 4, buffer);
            Writer.WriteUInt16(X, 8, buffer);
            Writer.WriteUInt16(Y, 10, buffer);
            Writer.WriteUInt16(SpellID, 12, buffer);
            Writer.WriteUInt16(SpellLevel, 14, buffer);
            Writer.WriteUInt32((uint)Targets.Count, 16, buffer);
            ushort offset = 20;
            uint uid = 0;
            foreach (KeyValuePair<uint, DamageClass> target in Targets.Base)
            {
                if (constC)
                {
                    if (uid == 0)
                        uid = target.Key;
                    Writer.WriteUInt32(uid, offset, buffer);
                }
                else
                    Writer.WriteUInt32(target.Key, offset, buffer);
                offset += 4;
                Writer.WriteUInt32(target.Value.Damage, offset, buffer); offset += 4;
                Writer.WriteBoolean(target.Value.Hit, offset, buffer); offset += 4;
                Writer.WriteBoolean(true, offset, buffer); offset += 4;//critical...
                offset += 16;
            }
            return buffer;
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
