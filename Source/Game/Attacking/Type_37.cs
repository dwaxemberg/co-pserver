using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Game.ConquerStructures;
using Conquer_Online_Server.ServerBase;

namespace Conquer_Online_Server.Game.Attacking
{
    public class Type_37
    {
        ushort X, Y;
        SpellUse Packet;
        Attack Packet_Attack;
        Entity Attacker, Attacked;
        ISkillInfo Spell;
        Database.SpellInformation spell = null;
        uint Damage, Target;

        public Type_37(uint _Target, ushort _X, ushort _Y, Entity _Attacker, Attack _Packet, ISkillInfo _Spell)
        {
            Packet = new SpellUse(true);
            Packet_Attack = _Packet;
            Attacker = _Attacker;
            Spell = _Spell;
            Target = _Target;
            X = _X;
            Y = _Y;
            doIt();
        }

        void doIt()
        {
            #region [Build-Packet]
            Packet.Attacker = Attacker.UID;
            Packet.SpellID = Spell.ID;
            Packet.SpellLevel = Spell.Level;
            Packet_Attack.AttackType = Attack.Magic;
            short CalcDist = ServerBase.Kernel.GetDistance(Attacker.X, Attacker.Y, X, Y);
            #endregion
            #region [Max-Targets]
            byte Counter = 0;
            byte TargetsCount = 0;
            switch (Spell.Level)
            {
                case 0:
                case 1: TargetsCount = 2; break;
                case 2: TargetsCount = 3; break;
                case 3:
                case 4: TargetsCount = 4; break;
            }
            #endregion
            Entity Main = null;
            if (Attacker.Owner.Screen.TryGetValue(Target, out Main))
            {
                Counter++;
                ushort Distance = (ushort)(TargetsCount + 11);
                if (ServerBase.Kernel.GetDistance(Attacker.X, Attacker.Y, Main.X, Main.Y) > Distance)
                    return;

                try
                {
                    foreach (Interfaces.IMapObject _obj in Attacker.Owner.Screen.Objects)
                    {
                        if (_obj == null)
                            continue;
                        if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                        {
                            Attacked = _obj as Entity;
                            if (ServerBase.Kernel.GetDistance(Attacker.X, Attacker.Y, Attacked.X, Attacked.Y) <= CalcDist)
                            {
                                if (CanAttack(Attacker, Attacked, spell, Packet_Attack.AttackType == Attack.Melee))
                                {
                                    uint damage = Game.Attacking.Calculate.Melee(Attacker, Attacked);

                                    ReceiveAttack(Attacker, Attacked, Packet_Attack, damage, Spell);

                                    Packet.Targets.Add(Attacked.UID, damage);
                                }
                            }
                        }
                    }
                    Attacker.Owner.SendScreen(Packet, true);



                    Attacker.Owner.SendScreen(Packet, true);
                }
                catch { }

                Attacker.Owner.Screen.Reload(null);
            }
        }
       public static bool CanAttack(Game.Entity Attacker, Game.Entity Attacked, Database.SpellInformation spell, bool melee)
        {
            if (spell != null)
                if (spell.CanKill && Attacker.EntityFlag == EntityFlag.Player && ServerBase.Constants.PKForbiddenMaps.Contains(Attacker.Owner.Map.ID) && Attacked.EntityFlag == EntityFlag.Player)
                    return false;
            if (Attacker.EntityFlag == EntityFlag.Player)
                if (Attacker.Owner.WatchingGroup != null)
                    return false;
            if (Attacked == null)
                return false;
            if (Attacked.Dead)
            {
                Attacker.AttackPacket = null;
                return false;
            }
            if (Attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Attacked.Companion)
                {
                    if (ServerBase.Constants.PKForbiddenMaps.Contains(Attacker.Owner.Map.ID))
                    {
                        if (Attacked.Owner == Attacker.Owner)
                            return false;
                        if (Attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                         Attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team)
                            return false;
                        else
                        {
                            Attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                            Attacker.FlashingNameStamp = Time32.Now;
                            Attacker.FlashingNameTime = 10;

                            return true;
                        }
                    }
                }
                if (Attacked.Name.Contains("Guard"))
                {
                    if (Attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                    Attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team)
                        return false;
                    else
                    {
                        Attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        Attacker.FlashingNameStamp = Time32.Now;
                        Attacker.FlashingNameTime = 10;

                        return true;
                    }
                }
                else
                    return true;
            }
            else
            {
                if (Attacked.EntityFlag == EntityFlag.Player)
                    if (!Attacked.Owner.Attackable)
                        return false;
                if (Attacker.EntityFlag == EntityFlag.Player)
                    if (Attacker.Owner.WatchingGroup == null)
                        if (Attacked.EntityFlag == EntityFlag.Player)
                            if (Attacked.Owner.WatchingGroup != null)
                                return false;

                if (spell != null)
                    if (spell.OnlyGround)
                        if (Attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                if (melee && Attacked.ContainsFlag(Update.Flags.Fly))
                    return false;

                if (ServerBase.Constants.PKForbiddenMaps.Contains(Attacker.Owner.Map.ID))
                {
                    if (Attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.PK ||
                        Attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Team || (spell != null && spell.CanKill))
                    {
                        Attacker.Owner.Send(ServerBase.Constants.PKForbidden);
                        Attacker.AttackPacket = null;
                    }
                    return false;
                }
                if (Attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Capture)
                {
                    if (Attacked.ContainsFlag(Update.Flags.FlashingName) || Attacked.PKPoints > 99)
                    {
                        return true;
                    }
                }
                if (Attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Team)
                {
                    if (Attacker.Owner.Team != null)
                        if (Attacker.Owner.Team.IsTeammate(Attacked.UID))
                        {
                            Attacker.AttackPacket = null;
                            return false;
                        }
                    if (Attacker.Owner.Guild != null)
                    {
                        if (Attacker.GuildID != 0)
                        {
                            if (Attacked.GuildID != 0)
                            {
                                if (Attacker.GuildID == Attacked.GuildID)
                                {
                                    Attacker.AttackPacket = null;
                                    return false;
                                }
                            }
                        }
                    }
                    if (Attacker.Owner.Friends.ContainsKey(Attacked.UID))
                    {
                        Attacker.AttackPacket = null;
                        return false;
                    }
                }

                if (spell != null)
                    if (spell.OnlyGround)
                        if (Attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                if (spell != null)
                    if (!spell.CanKill)
                    {
                        if (spell != null && !spell.CanKill && Attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Capture)
                        {
                            return false;
                        }
                        if (spell != null && !spell.CanKill && Attacker.Owner.Map.ID >= 10000)
                        {
                            Attacker.AddFlag(Network.GamePackets.Update.Flags.Normal);
                            return true;
                        }
                        else
                        {
                            Attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                            Attacker.FlashingNameStamp = Time32.Now;
                            Attacker.FlashingNameTime = 10;
                            return true;

                        }
                    }



                if (Attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                    Attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team && Attacked.PKPoints < 99)
                {
                    Attacker.AttackPacket = null;
                    return false;
                }
                else
                {
                    if (!Attacked.ContainsFlag(Update.Flags.FlashingName) || !Attacked.ContainsFlag(Update.Flags.BlackName))
                    {
                        if (ServerBase.Constants.PKFreeMaps.Contains(Attacker.Owner.Map.BaseID))
                            return true;
                        Attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        Attacker.FlashingNameStamp = Time32.Now;
                        Attacker.FlashingNameTime = 10;
                    }
                }
                return true;
            }
        }
        private void ReceiveAttack(Game.Entity Attacker, Game.Entity Attacked, Attack attack, uint damage, ISkillInfo spell)
        {
            #region Quarantine Tournament
            if (Attacker.MapID == Quarantine.Map && Attacker.MapID == Quarantine.Map && Quarantine.Started) //Emme
            {
                damage = 1;
                //181315 WhiteElegance
                //181515 BlackElegance
                #region Disqualification & Spell check
                try
                {
                    if (spell == null) { Attacker.Owner.Send(new Message("Only FB/SS is allowed!", System.Drawing.Color.Red, Message.TopLeft)); return; }
                    if (spell.ID != 1045 && spell.ID != 1046)
                    { Attacker.Teleport(1002, 350, 350); return; }
                    if (Attacker.Owner.Equipment.Objects[8] == null)
                    { Attacker.Teleport(1002, 350, 350); return; }
                    uint AttackerID = Attacker.Owner.Equipment.Objects[8].ID % 1000;
                    if (AttackerID != 315 && AttackerID != 515)
                    { Attacker.Teleport(1002, 350, 350); return; }
                }
                catch { }
                #endregion
                if (Attacked.Owner.Equipment.Objects[8].ID == Attacker.Owner.Equipment.Objects[8].ID)
                    return;

                #region Add To Team
                if (Quarantine.Black.ContainsKey(Attacked.UID))
                {
                    Quarantine.Black.Remove(Attacked.UID);
                    Quarantine.White.Add(Attacked.UID, Attacked.Owner);
                    Attacked.Owner.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                    Attacked.Owner.Equipment.Objects[8].ID = 181315;
                }
                else
                {
                    Quarantine.White.Remove(Attacked.UID);
                    Quarantine.Black.Add(Attacked.UID, Attacked.Owner);
                    Attacked.Owner.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                    Attacked.Owner.Equipment.Objects[8].ID = 181515;
                }
                Attacked.Owner.Equipment.Objects[8].Send(Attacked.Owner);
                #endregion


                if (Quarantine.White.Count == 0 || Quarantine.Black.Count == 0)
                {
                    if (Quarantine.White.Count == 0) //Black team win
                    {
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                                "The black team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.Black.Values);
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                           "The black team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.White.Values);
                        Quarantine.BlackScore++;
                        int addCP = 100;
                        if (Quarantine.BlackScore == 5)
                            addCP = 800;
                        foreach (Conquer_Online_Server.Client.GameState memb in Quarantine.Black.Values)
                            memb.Entity.ConquerPoints += (uint)addCP;
                        while (Quarantine.White.Count == 0)
                        {
                            foreach (Conquer_Online_Server.Client.GameState member in Quarantine.Black.Values)
                            {
                                if (Conquer_Online_Server.ServerBase.Kernel.Rate(50))
                                {
                                    Quarantine.White.Add(member.Entity.UID, member);
                                    Quarantine.Black.Remove(member.Entity.UID);
                                    member.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                                    member.Equipment.Objects[8].ID = 181315;
                                    member.Equipment.Objects[8].Send(member);
                                }

                            }
                        }
                    }
                    else //White team win
                    {
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                                "The white team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.Black.Values);
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Message(
                           "The white team has this round!", System.Drawing.Color.Red, Message.TopLeft), Quarantine.White.Values);
                        Quarantine.WhiteScore++;
                        int addCP = 100;
                        if (Quarantine.WhiteScore == 5)
                            addCP = 800;
                        foreach (Conquer_Online_Server.Client.GameState member in Quarantine.White.Values)
                            member.Entity.ConquerPoints += (uint)addCP; //Add 100 cps for winning 1 round
                        while (Quarantine.Black.Count == 0)
                        {
                            foreach (Conquer_Online_Server.Client.GameState member in Quarantine.White.Values)
                            {
                                if (Conquer_Online_Server.ServerBase.Kernel.Rate(50))
                                {
                                    Quarantine.Black.Add(member.Entity.UID, member);
                                    Quarantine.White.Remove(member.Entity.UID);
                                    member.Equipment.Objects[8].UID += (uint)Kernel.Random.Next(1, 100);
                                    member.Equipment.Objects[8].ID = 181515;
                                    member.Equipment.Objects[8].Send(member);
                                }

                            }
                        }
                    }
                    if (Quarantine.BlackScore == 5 || Quarantine.WhiteScore == 5)
                    {
                        foreach (Conquer_Online_Server.Client.GameState participant in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                        {
                            participant.UpdateQuarantineScore();
                            participant.Entity.Teleport(1002, 437, 349);
                            participant.Send(new Message("", System.Drawing.Color.Red, Message.FirstRightCorner));
                            participant.Send(new Message("The tournament is over!", System.Drawing.Color.Red, Message.TopLeft));
                            participant.quarantineDeath = 0;
                            participant.quarantineKill = 0;
                            participant.Equipment.Remove(9);
                            participant.Equipment.Objects[8] = null;
                            participant.SendEquipment(false);
                        }
                        Quarantine.Black = new ThreadSafeDictionary<uint, Conquer_Online_Server.Client.GameState>(50);
                        Quarantine.White = new ThreadSafeDictionary<uint, Conquer_Online_Server.Client.GameState>(50);
                        Quarantine.WhiteScore = 0;
                        Quarantine.BlackScore = 0;
                        Quarantine.Started = false;
                    }
                }
                if (Attacked.Owner.Equipment.Objects[8] != null)
                    Attacked.Owner.Equipment.Objects[8].Send(Attacked.Owner);
                Attacked.Owner.SendEquipment(false);
                Attacker.Owner.SendEquipment(false);
                Attacked.Owner.Equipment.UpdateEntityPacket();
                Attacker.Owner.AddQuarantineKill(); //Increase kills on Attacker
                Attacked.Owner.AddQuarantineDeath(); //Increase death on Attacked

            }
            #endregion
            if (!(Attacked.Name.Contains("Guard") && Attacked.EntityFlag == EntityFlag.Monster))
                if (Attacker.EntityFlag == EntityFlag.Player && Attacked.EntityFlag != EntityFlag.Player && !Attacked.Name.Contains("Guard"))
                {
                    if (damage > Attacked.Hitpoints)
                    {
                        Attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(Attacker.Level, Attacked.Level, Math.Min(damage, Attacked.Hitpoints)), true);
                        if (spell != null)
                            Attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(Attacker.Level, Attacked.Level, Math.Min(damage, Attacked.Hitpoints)), spell.ID);
                    }
                    else
                    {
                        Attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(Attacker.Level, Attacked.Level, damage), true);
                        if (spell != null)
                            Attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(Attacker.Level, Attacked.Level, damage), spell.ID);
                    }
                }
            if (Attacker.EntityFlag == EntityFlag.Monster && Attacked.EntityFlag == EntityFlag.Player)
            {
                if (Attacked.Action == Enums.ConquerAction.Sit)
                    if (Attacked.Stamina > 20)
                        Attacked.Stamina -= 20;
                    else
                        Attacked.Stamina = 0;
                Attacked.Action = Enums.ConquerAction.None;
            }

            if (attack.AttackType == Attack.Magic)
            {
                if (Attacked.Hitpoints <= damage)
                {
                    if (Attacker.Owner.QualifierGroup != null)
                        Attacker.Owner.QualifierGroup.UpdateDamage(Attacker.Owner, Attacked.Hitpoints);
                    Attacked.CauseOfDeathIsMagic = true;
                    Attacked.Die(Attacker);
                }
                else
                {
                    if (Attacker.Owner.QualifierGroup != null)
                        Attacker.Owner.QualifierGroup.UpdateDamage(Attacker.Owner, damage);
                    Attacked.Hitpoints -= damage;
                }
            }
            else
            {
                if (Attacked.Hitpoints <= damage)
                {
                    if (Attacked.EntityFlag == EntityFlag.Player)
                    {
                        if (Attacker.Owner.QualifierGroup != null)
                            Attacker.Owner.QualifierGroup.UpdateDamage(Attacker.Owner, Attacked.Hitpoints);
                        Attacked.Owner.SendScreen(attack, true);
                        Attacker.AttackPacket = null;
                    }
                    else
                    {
                        Attacked.MonsterInfo.SendScreen(attack);
                    }
                    Attacked.Die(Attacker);
                }
                else
                {
                    Attacked.Hitpoints -= damage;
                    if (Attacked.EntityFlag == EntityFlag.Player)
                    {
                        if (Attacker.Owner.QualifierGroup != null)
                            Attacker.Owner.QualifierGroup.UpdateDamage(Attacker.Owner, damage);
                        Attacked.Owner.SendScreen(attack, true);
                    }
                    else
                        Attacked.MonsterInfo.SendScreen(attack);
                    Attacker.AttackPacket = attack;
                    Attacker.AttackStamp = Time32.Now;
                }
            }
        }
    }
 
   
}
