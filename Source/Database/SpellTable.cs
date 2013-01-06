using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public enum LeftToAdd : ushort
    {
        Bless = 9876,
        //----------
        SummonGuard = 4000,
        SummonBat = 4010,
        SummonBatBoss = 4020,
        BloodyBat = 4050,
        FireEvil = 4060,
        Skeleton = 4070
    }
    public class SpellSort
    {
        public const byte
            Damage = 1,
            Heal = 2,
            MultiWeaponSkill = 4,
            Circle = 5,
            XP = 6,
            Revive = 7,
            XPIncrease = 11,
            Dash = 12,
            Linear = 14,
            SingleWeaponSkill = 16,
            Specials = 19,
            ManaAdd = 20,
            Summon = 23,
            HPPercentDecrease = 26,
            Spook = 30,
            WarCry = 31,
            Ride = 32;
    }
    public class SpellTarget
    {
        public const byte
            Magic = 0,
            EntitiesOnly = 1,
            Self = 2,
            AroundCoordonates = 4,
            Sector = 8,//power % 1000 = sector angle
            AutoAttack = 16,
            PlayersOnly = 32;
    }
    public class SpellInformation
    {
        public ushort ID;
        public byte Level;

        public bool CanKill;
        public byte Sort;
        public bool OnlyGround;
        public bool Multi;
        public byte Target;

        public ushort UseMana;
        public byte UseStamina;
        public byte UseArrows;

        public byte Percent;
        public int Sector;

        public int Duration;

        public ushort Range;
        public ushort Distance;

        public ushort Power;
        public float PowerPercent;

        public ulong Status;

        public uint NeedExperience;
        public byte NeedLevel;
        public byte NeedXP;

        public ushort WeaponSubtype;
        public ushort OnlyWithThisWeaponSubtype;
        public ushort NextSpellID; 
    }
    public class SpellTable
    {
        public static SafeDictionary<ushort, SafeDictionary<byte, SpellInformation>> SpellInformations = new SafeDictionary<ushort, SafeDictionary<byte, SpellInformation>>(100000);
        public static SafeDictionary<ushort, ushort> WeaponSpells = new SafeDictionary<ushort, ushort>(100);
        public static void Load()
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("spells");
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                SpellInformation spell = new SpellInformation();
                spell.ID = reader.ReadUInt16("type");
                spell.Sort = reader.ReadByte("sort");
                spell.CanKill = reader.ReadBoolean("crime");
                spell.OnlyGround = reader.ReadBoolean("ground");
                spell.Multi = reader.ReadBoolean("multi");
                spell.Target = reader.ReadByte("target");
                spell.Level = reader.ReadByte("level");
                spell.UseMana = reader.ReadUInt16("use_mp");
                spell.UseStamina = reader.ReadByte("use_ep");
                spell.UseArrows = reader.ReadByte("use_item_num");
                spell.Power = reader.ReadUInt16("power");
                spell.PowerPercent = (float)(reader.ReadUInt16("power") % 1000) / 100;
                if (spell.Power > 13000)
                    spell.Power = 0;
                spell.Percent = reader.ReadByte("percent");
                spell.Duration = reader.ReadInt32("step_secs");
                spell.Range = reader.ReadUInt16("range");
                spell.Sector = spell.Range * 20;
                spell.Distance = reader.ReadUInt16("distance");
                if (spell.Distance >= 4) spell.Distance--;
                spell.Status = reader.ReadUInt64("status");
                spell.NeedExperience = reader.ReadUInt32("need_exp");
                spell.NeedLevel = reader.ReadByte("need_level");
                spell.WeaponSubtype = spell.OnlyWithThisWeaponSubtype = reader.ReadUInt16("weapon_subtype");
                spell.NextSpellID = reader.ReadUInt16("next_magic");
                spell.NeedXP = reader.ReadByte("use_xp");
                if (SpellInformations.ContainsKey(spell.ID))
                {
                    SpellInformations[spell.ID].Add(spell.Level, spell);
                }
                else
                {
                    SpellInformations.Add(spell.ID, new SafeDictionary<byte, SpellInformation>(10));
                    SpellInformations[spell.ID].Add(spell.Level, spell);
                }
                if (spell.Distance > 17)
                    spell.Distance = 17;
                if (spell.WeaponSubtype != 0)
                {
                    switch (spell.ID)
                    {
                        case 5010:
                        case 7020:
                        case 1290:
                        case 1260:
                        case 5030:
                        case 5040:
                        case 7000:
                        case 7010:
                        case 7030:
                        case 7040:
                        case 1250:
                        case 5050:
                        case 5020:
                        case 10490:
                        case 1300:
                            if (spell.Distance >= 3)
                                spell.Distance = 3;
                            if (spell.Range > 3)
                                spell.Range = 3;
                            if (!WeaponSpells.ContainsKey(spell.WeaponSubtype))
                            {
                                WeaponSpells.Add(spell.WeaponSubtype, spell.ID);
                            }
                            break;
                    }
                }
            }
            reader.Close();
            Console.WriteLine("Spells information loaded.");
        }       
    }
}
