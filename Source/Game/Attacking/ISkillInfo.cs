using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.Attacking
{
    public class ISkillInfo
    {
        public ushort ID { get; set; }
        public byte Level { get; set; }
        public Enums.SpellType Type { get; set; }
        public uint Power { get; set; }
        public byte Range { get; set; }
        public byte Distance { get; set; }
        public byte Percent { get; set; }
        public byte Needed_Level { get; set; }
        public uint Needed_Exp { get; set; }
        public ushort UseMana { get; set; }
        public byte UseStam { get; set; }
        public ushort StepSecs { get; set; }
        public byte UseXP { get; set; }
        public byte Crime { get; set; }
        public ushort WeaponType { get; set; }
        public bool iSMaxLevel
        {
            get
            {
                if (Needed_Exp != 0) return false;
                else return true;
            }
        }
        public string Name { get; set; }
        public Dictionary<byte, ISkillInfo> Levels = new Dictionary<byte, ISkillInfo>();
    }
}
