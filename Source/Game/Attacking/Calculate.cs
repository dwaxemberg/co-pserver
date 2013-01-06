using System;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.ServerBase;

namespace Conquer_Online_Server.Game.Attacking
{
    public class Calculate
    {
        public static uint Melee(Entity attacker, Entity attacked)
        {
            if (attacked.Name.Contains("Guard1"))
                return 1;
            int Damage = 0;
            if (attacker.MinAttack > attacker.MaxAttack)
            {
                return 0;
            }
            //int Damage = 0;
            //int Damage = Kernel.Rand.Next((int)attacker.MinAttack, (int)attacker.MaxAttack);
            int Leveldiffs = (attacker.Level + 2) - attacked.Level;
            int Damageadds = (int)Math.Floor(1 + (Leveldiffs / 5) * 0.8);
            if (Damageadds > 1)
                Damage = Damageadds * Damage;
            Damage -= attacked.Defence;
            if (attacked.Reborn == 1)
            {
                Damage = (int)Math.Floor(Damage * .7);
            }
            else if (attacked.Reborn == 2)
            {
                Damage = (int)Math.Floor(Damage * .7);
                if (attacker.Reborn < 2)
                    Damage = (int)Math.Floor(Damage * .5);
            }
            double Tort = 0;
            Tort += attacked.Gems[7] * 0.02;
            Tort += attacked.Gems[7] * 0.04;
            Tort += attacked.Gems[7] * 0.06;
            Damage = (int)Math.Floor(Damage * (1 - Tort));
            if (attacked.ItemBless > 0)
            {
                Damage = (int)Math.Floor(Damage * (1 - (attacked.ItemBless * 0.01)));
            }
            //if (attacker.EntityFlag == EntityFlag.Monster)
            //    if (attacked.EntityFlag == EntityFlag.Player)
            //        if (ServerBase.Kernel.Rate(Math.Min(60, attacked.Dodge + 30)))
            //            return 0;

            Durability(attacker, attacked, null);

            if (attacker.Name.Contains("Guard1"))
                return 700000;
            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (!attacker.Transformed)
                Damage = ServerBase.Kernel.Random.Next(Math.Min((int)attacker.MinAttack, (int)attacker.MaxAttack), Math.Max((int)attacker.MinAttack, (int)attacker.MaxAttack) + 1);
            else
                Damage = ServerBase.Kernel.Random.Next((int)attacker.TransformationMinAttack, (int)attacker.TransformationMaxAttack + 1);

            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (int)(Damage * attacker.StigmaIncrease);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));
            }
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (attacked.EntityFlag == EntityFlag.Monster)
                {
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        if (!attacker.Owner.Equipment.Free(4) && !attacker.Owner.Equipment.Free(5))
                            Damage = (int)(Damage * 1.5);
                }
                if (attacked.EntityFlag == EntityFlag.Monster)
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        Damage = (int)(Damage * AttackMultiplier(attacker, attacked));
                if (attacker.OnSuperman())
                    if (attacked.EntityFlag == EntityFlag.Monster)

                        if (attacker.ContainsFlag(Update.Flags.Oblivion))
                            if (attacked.EntityFlag == EntityFlag.Monster)
                                Damage *= 2;

                if (attacker.OnFatalStrike())
                    if (attacked.EntityFlag == EntityFlag.Monster)
                        Damage *= 5;
            }
            if (!attacked.Transformed)
            {
                if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                {
                    if (attacked.ShieldTime > 0)
                        Damage -= (ushort)(attacked.Defence * attacked.ShieldIncrease);
                    else
                        Damage -= (ushort)(attacked.Defence * attacked.MagicShieldIncrease);
                }
                else
                {
                    Damage -= attacked.Defence;
                }
            }
            else
                Damage -= attacked.TransformationDefence;

            // Damage = BattlePowerCalculation(Damage, attacker.BattlePower - attacked.BattlePower);

            //if (ServerBase.Kernel.Rate(5))
            //{
            //    if (attacker.EntityFlag == EntityFlag.Player)
            //    {
            //        if (attacker.Owner.BlessTime > 0)
            //        {
            //            Damage *= 2;
            //            _String str = new _String(true);
            //            str.UID = attacker.UID;
            //            str.TextsCount = 1;
            //            str.Type = _String.Effect;
            //            str.Texts.Add("LuckyGuy");
            //            attacker.Owner.SendScreen(str, true);
            //        }
            //    }
            //}

            //if (ServerBase.Kernel.Rate(5))
            //{
            //    if (attacked.EntityFlag == EntityFlag.Player)
            //    {
            //        if (attacked.Owner.BlessTime > 0)
            //        {
            //            Damage = 1;
            //            _String str = new _String(true);
            //            str.UID = attacker.UID;
            //            str.TextsCount = 1;
            //            str.Type = _String.Effect;
            //            str.Texts.Add("LuckyGuy");
            //            attacked.Owner.SendScreen(str, true);
            //        }
            //    }
            //}

            Damage = RemoveExcessDamage(Damage, attacker, attacked);

            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.Name != "Guard1" && attacked.Name != "Guard2")
                {
                    int Leveldiff = (attacker.Level + 2) - attacked.Level;
                    int Damageadd = (int)Math.Floor(1 + (Leveldiff / 5) * 0.8);
                    if (Damageadd > 1)
                        Damage = Damageadd * Damage;
                }
                Damage -= attacked.Defence;
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (int)(700 * attacked.MaxHitpoints);
            }
            if (Damage <= 0)
                Damage = 1;

            AutoRespone(attacker, attacked, ref Damage);

            if (attacked.ContainsFlag(Update.Flags.AzureShield))
            {
                if (Damage >= attacked.AzureDamage)
                {
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;

                }
                else
                {
                    attacked.AzureDamage -= Damage;
                    Damage = 0;
                }

            }
            return (uint)Damage;
        }
        public static uint Melee(Entity attacker, Entity attacked, Database.SpellInformation spell)
        {
            if (attacked.Name.Contains("Guard1"))
                return 1;
            int Damage = 0;

            if (attacker.EntityFlag == EntityFlag.Monster)
                if (attacked.EntityFlag == EntityFlag.Player)
                    if (ServerBase.Kernel.Rate(Math.Min(60, attacked.Dodge + 30)))
                        return 0;

            Durability(attacker, attacked, null);
            if (attacker.Name.Contains("Guard1"))
                return 700000;
            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (!attacker.Transformed)
                Damage = ServerBase.Kernel.Random.Next(Math.Min((int)attacker.MinAttack, (int)attacker.MaxAttack), Math.Max((int)attacker.MinAttack, (int)attacker.MaxAttack) + 1);
            else
                Damage = ServerBase.Kernel.Random.Next((int)attacker.TransformationMinAttack, (int)attacker.TransformationMaxAttack + 1);

            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (int)(Damage * attacker.StigmaIncrease);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));
            }
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (attacked.EntityFlag == EntityFlag.Monster)
                {
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        if (!attacker.Owner.Equipment.Free(4) && !attacker.Owner.Equipment.Free(5))
                            Damage = (int)(Damage * 1.5);
                }
                if (attacked.EntityFlag == EntityFlag.Monster)
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        Damage = (int)(Damage * AttackMultiplier(attacker, attacked));
                // if (attacker.OnSuperman())
                //     if (attacked.EntityFlag == EntityFlag.Monster)
                //         Damage *= 10;

                if (attacker.OnFatalStrike())
                    if (attacked.EntityFlag == EntityFlag.Monster)
                        Damage *= 5;
            }
            if (!attacked.Transformed)
            {
                if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                {
                    if (attacked.ShieldTime > 0)
                        Damage -= (ushort)(attacked.Defence * attacked.ShieldIncrease);
                    else
                        Damage -= (ushort)(attacked.Defence * attacked.MagicShieldIncrease);
                }
                else
                {
                    Damage -= attacked.Defence;
                }
            }
            else
                Damage -= attacked.TransformationDefence;
            Damage = BattlePowerCalculation(Damage, attacker.BattlePower - attacked.BattlePower);
            if (ServerBase.Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (ServerBase.Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }
            if (spell.ID == 6000 && attacked.EntityFlag == EntityFlag.Monster)
            {
                if (spell.PowerPercent != 0)
                    Damage = (int)(Damage * spell.PowerPercent);
            }
            else if (spell.ID != 6000)
            {
                if (spell.PowerPercent != 0)
                    Damage = (int)(Damage * spell.PowerPercent);
            }
            Damage = RemoveExcessDamage(Damage, attacker, attacked);

            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (int)(700 * attacked.MaxHitpoints);
            }
            if (Damage <= 0)
                Damage = 1;

            AutoRespone(attacker, attacked, ref Damage);

            if (attacked.ContainsFlag(Update.Flags.AzureShield))
            {
                if (Damage >= attacked.AzureDamage)
                {
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;

                }
                else
                {
                    attacked.AzureDamage -= Damage;
                    Damage = 0;
                }
            }
            if (attacker.OnSuperman())
                if (attacked.EntityFlag == EntityFlag.Monster)
                    Damage *= 10;
            if (attacker.NobilityRank != attacked.NobilityRank)
            {
                int addnobility_damage = 0;
                if ((byte)attacker.NobilityRank == 12)
                    addnobility_damage = (Damage * ((byte)attacker.NobilityRank + 45)) / 100;
                else
                    if ((byte)attacker.NobilityRank > 5)
                        addnobility_damage = (Damage * ((byte)attacker.NobilityRank + 25)) / 100;
                if ((byte)attacked.NobilityRank > 5)
                {
                    if ((addnobility_damage - (uint)((Damage * ((byte)attacked.NobilityRank + 8)) / 100)) > 0)
                    {
                        addnobility_damage -= (int)((Damage * ((byte)attacked.NobilityRank + 8)) / 100);
                    }
                    else
                    {
                        if (Damage > (uint)((Damage * ((byte)attacked.NobilityRank + 8)) / 100))
                        {
                            Damage -= (int)((Damage * ((byte)attacked.NobilityRank + 8)) / 100);
                        }
                        else
                            Damage = 1;
                    }
                }
                if (Damage != 1)
                {
                    Damage += addnobility_damage;
                }
            }
            return (uint)Damage;
        }
        public static uint Melee(Entity attacker, SobNpcSpawn attacked)
        {
            if (attacked.Name.Contains("Guard1"))
                return 1;
            int Damage = 0;
            if (attacked.UID == 810 && attacker.Owner.Guild != null)
            {
                if (Game.ConquerStructures.Society.GuildWar.PoleKeeper == attacker.Owner.Guild)
                {
                    return 0;
                }
            }
            Durability(attacker, null, null);
            if (!attacker.Transformed)
                Damage = ServerBase.Kernel.Random.Next((int)attacker.MinAttack, (int)attacker.MaxAttack + 1);
            else
                Damage = ServerBase.Kernel.Random.Next(Math.Min((int)attacker.MinAttack, (int)attacker.MaxAttack), Math.Max((int)attacker.MinAttack, (int)attacker.MaxAttack) + 1);
            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (int)(Damage * attacker.StigmaIncrease);
            Damage += attacker.PhysicalDamageIncrease;

            if (ServerBase.Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }
            if (Damage <= 0)
                Damage = 1;

            return (uint)Damage;
        }

        public static uint Magic(Entity attacker, Entity attacked, Database.SpellInformation spell)
        {
            if (!ServerBase.Kernel.Rate(spell.Percent))
                return 0;
            Durability(attacker, attacked, spell);
            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (attacker.Transformed)
                return 0;
            if (attacker.Name.Contains("Guard1"))
                return 700000;
            int Damage = 0;
            Damage = (int)attacker.MagicAttack;
            Damage += spell.Power;

            if (!attacked.Transformed)
            {
                //(uint)((double)damage * (100 - Attacked.Entity.ItemBless) / 95);
                // Damage -= (int)((uint)(attacked.MagicDefencePercent * Damage / 100));
                if (attacked.ItemBless > 60)
                    attacked.ItemBless = 58;
                Damage = (int)((uint)((double)Damage * (100 - attacked.ItemBless) / 340));
                Damage -= attacked.MagicDefence;


            }
            else
                Damage = (int)(((Damage * 0.75) * (1 - (0 * 0.01))) - attacked.TransformationMagicDefence);

            if (attacked.EntityFlag == EntityFlag.Monster)
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * AttackMultiplier(attacker, attacked) / 2);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                Damage = (int)(Damage * 6.6);
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));
            }

            Damage = BattlePowerCalculation(Damage, attacker.BattlePower - attacked.BattlePower);

            //  Damage = RemoveExcessDamage(Damage, attacker, attacked);
            //   Console.WriteLine("[3]" + Damage);
            if (ServerBase.Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    Damage *= 2;
                    _String str = new _String(true);
                    str.UID = attacker.UID;
                    str.TextsCount = 1;
                    str.Type = _String.Effect;
                    str.Texts.Add("LuckyGuy");
                    attacker.Owner.SendScreen(str, true);
                }
            }

            if (ServerBase.Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }

            Damage += attacker.MagicDamageIncrease;
            Damage -= attacked.MagicDamageDecrease;

            if (Damage <= 0)
                Damage = 1;

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (int)(700 * attacked.MaxHitpoints);
            }
            else
            {
                if (attacker.EntityFlag == EntityFlag.Monster)
                {
                    if (attacker.Name.Contains("Guard1"))
                        Damage = (int)attacked.MaxHitpoints + 1;
                }
            }

            AutoRespone(attacker, attacked, ref Damage);
            if (attacked.ContainsFlag(Update.Flags.AzureShield))
            {
                if (Damage >= attacked.AzureDamage)
                {
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;

                }
                else
                {
                    attacked.AzureDamage -= Damage;
                    Damage = 0;
                }
            }
            return (uint)Damage;
        }
        public static uint Magic(Entity attacker, Entity attacked, ushort spellID, byte spellLevel)
        {
            Database.SpellInformation spell = Database.SpellTable.SpellInformations[spellID][spellLevel];
            return Magic(attacker, attacked, spell);
        }
        public static uint Magic(Entity attacker, SobNpcSpawn attacked, Database.SpellInformation spell)
        {
            if (!ServerBase.Kernel.Rate(spell.Percent))
                return 0;
            if (attacked.UID == 810 && attacker.Owner.Guild != null)
            {
                if (Game.ConquerStructures.Society.GuildWar.PoleKeeper == attacker.Owner.Guild)
                {
                    return 0;
                }
            }
            Durability(attacker, null, spell);
            if (attacker.Transformed)
                return 0;
            if (attacker.Name.Contains("Guard"))
                return 700000;
            int Damage = 0;
            Damage = (int)attacker.MagicAttack;
            Damage += spell.Power;
            if (ServerBase.Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }


            Damage += attacker.MagicDamageIncrease;

            if (Damage <= 0)
                Damage = 1;

            return (uint)Damage;
        }

        public static uint Ranged(Entity attacker, Entity attacked)
        {
            if (attacked.Name.Contains("Guard1"))
                return 1;
            int Damage = 0;
            Durability(attacker, attacked, null);

            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (attacker.Transformed)
                return 0;
            if (attacker.Name.Contains("Guard"))
                return 700000;
            Damage = ServerBase.Kernel.Random.Next(Math.Min((int)attacker.MinAttack, (int)attacker.MaxAttack), Math.Max((int)attacker.MinAttack, (int)attacker.MaxAttack) + 1);

            if (attacker.OnSuperman())
                if (attacked.EntityFlag == EntityFlag.Monster)
                    Damage *= 10;
                else
                    Damage *= 2;

            if (attacker.OnFatalStrike())
                if (attacked.EntityFlag == EntityFlag.Monster)
                    Damage *= 5;

            if (!attacked.Transformed)
                Damage -= attacked.Defence;
            else
                Damage -= attacked.TransformationDefence;

            Damage -= Damage * attacked.ItemBless / 100;

            byte dodge = attacked.Dodge;
            if (dodge > 100)
                dodge = 99;
            if (!attacked.Transformed)
                Damage -= Damage * dodge / 100;
            else
                Damage -= Damage * attacked.TransformationDodge / 100;

            if (attacker.OnIntensify && Time32.Now >= attacker.IntensifyStamp.AddSeconds(4))
            {
                Damage *= 2;
                attacker.OnIntensify = false;
            }

            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (int)(Damage * attacker.StigmaIncrease);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));

                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * AttackMultiplier(attacker, attacked));
            }
            Damage = BattlePowerCalculation(Damage, attacker.BattlePower - attacked.BattlePower);
            if (ServerBase.Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (ServerBase.Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }
            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (int)(700 * attacked.MaxHitpoints);
            }

            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;

            if (Damage <= 0)
                Damage = 1;
            AutoRespone(attacker, attacked, ref Damage);
            if (attacked.ContainsFlag(Update.Flags.AzureShield))
            {
                if (Damage >= attacked.AzureDamage)
                {
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;

                }
                else
                {
                    attacked.AzureDamage -= Damage;
                    Damage = 0;
                }
            }
            return (uint)Damage;
        }
        public static uint Ranged(Entity attacker, Entity attacked, Database.SpellInformation spell)
        {
            if (attacked.Name.Contains("Guard1"))
                return 1;
            int Damage = 0;
            Durability(attacker, attacked, null);

            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (attacker.Transformed)
                return 0;
            if (attacker.Name.Contains("Guard1"))
                return 700000;
            Damage = ServerBase.Kernel.Random.Next(Math.Min((int)attacker.MinAttack, (int)attacker.MaxAttack), Math.Max((int)attacker.MinAttack, (int)attacker.MaxAttack) + 1);

            if (attacker.OnSuperman())
                if (attacked.EntityFlag == EntityFlag.Monster)
                    Damage *= 10;
                else
                    Damage *= 2;

            if (attacker.OnFatalStrike())
                if (attacked.EntityFlag == EntityFlag.Monster)
                    Damage *= 5;

            if (!attacked.Transformed)
                Damage -= attacked.Defence;
            else
                Damage -= attacked.TransformationDefence;

            Damage -= Damage * attacked.ItemBless / 100;

            byte dodge = attacked.Dodge;
            if (dodge > 100)
                dodge = 99;
            if (!attacked.Transformed)
                Damage -= Damage * dodge / 100;
            else
                Damage -= Damage * attacked.TransformationDodge / 100;

            if (attacker.OnIntensify && Time32.Now >= attacker.IntensifyStamp.AddSeconds(4))
            {
                Damage *= 2;
                attacker.OnIntensify = false;
            }

            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (int)(Damage * attacker.StigmaIncrease);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));

                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (int)(Damage * AttackMultiplier(attacker, attacked));
            }
            Damage = BattlePowerCalculation(Damage, attacker.BattlePower - attacked.BattlePower);
            if (ServerBase.Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (ServerBase.Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }
            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (int)(700 * attacked.MaxHitpoints);
            }

            if (spell.PowerPercent != 0)
                Damage = (int)(Damage * spell.PowerPercent);

            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;

            if (Damage <= 0)
                Damage = 1;
            AutoRespone(attacker, attacked, ref Damage);
            if (attacked.ContainsFlag(Update.Flags.AzureShield))
            {
                if (Damage >= attacked.AzureDamage)
                {
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;

                }
                else
                {
                    attacked.AzureDamage -= Damage;
                    Damage = 0;
                }
            }
            return (uint)Damage;
        }
        public static uint Ranged(Entity attacker, SobNpcSpawn attacked)
        {
            int Damage = 0;
            Durability(attacker, null, null);
            if (attacker.Transformed)
                return 0;
            if (attacked.UID == 810 && attacker.Owner.Guild != null)
            {
                if (Game.ConquerStructures.Society.GuildWar.PoleKeeper == attacker.Owner.Guild)
                {
                    return 0;
                }
            }
            Damage = ServerBase.Kernel.Random.Next(Math.Min((int)attacker.MinAttack, (int)attacker.MaxAttack), Math.Max((int)attacker.MinAttack, (int)attacker.MaxAttack) + 1);

            if (attacker.OnIntensify && Time32.Now >= attacker.IntensifyStamp.AddSeconds(4))
            {
                Damage *= 2;

                attacker.OnIntensify = false;
            }

            if (ServerBase.Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }
            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (int)(Damage * attacker.StigmaIncrease);

            Damage += attacker.PhysicalDamageIncrease;
            if (Damage <= 0)
                Damage = 1;
            return (uint)Damage;
        }

        public static int RemoveExcessDamage(int CurrentDamage, Entity Attacker, Entity Opponent)
        {
            if (Opponent.EntityFlag != EntityFlag.Player)
                return CurrentDamage;
            if (Opponent.Reborn == 1)
                CurrentDamage = (int)Math.Round((double)(CurrentDamage * 0.7));
            else if (Opponent.Reborn == 2)
                CurrentDamage = (int)Math.Round((double)(CurrentDamage * 0.5));
            CurrentDamage = (int)Math.Round((double)(CurrentDamage * (1.00 - (Opponent.ItemBless * 0.01))));

            return CurrentDamage;
        }

        public static int BattlePowerCalculation(int damage, int battlepowerExcess)
        {
            if (battlepowerExcess == 0)
                return damage;

            if (battlepowerExcess < -50)
                battlepowerExcess = -50;

            if (battlepowerExcess > 50)
                battlepowerExcess = 50;

            return damage + (damage * battlepowerExcess / 100);
        }

        public static uint Percent(Entity attacked, float percent)
        {
            return (uint)(attacked.Hitpoints * percent);
        }

        public static uint Percent(SobNpcSpawn attacked, float percent)
        {
            return (uint)(attacked.Hitpoints * percent);
        }

        public static uint Percent(int target, float percent)
        {
            return (uint)(target * percent);
        }

        private static void Durability(Entity attacker, Entity attacked, Database.SpellInformation spell)
        {
            NewMethod(spell);
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.Map.ID == 1039)
                    return;
            #region Attack
            if (attacker != null)
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    for (byte i = 4; i <= 6; i++)
                    {
                        if (!attacker.Owner.Equipment.Free(i))
                        {
                            var item = attacker.Owner.Equipment.TryGetItem(i);
                            if (i == 5)
                            {
                                if (Network.PacketHandler.IsArrow(item.ID))
                                {
                                    continue;
                                }
                            }
                            if (ServerBase.Kernel.Rate(20, 100))
                            {
                                if (item.Durability != 0)
                                {
                                    item.Durability--;
                                    if (item.Durability == 0)
                                        attacker.Owner.UnloadItemStats(item, true);
                                    Database.ConquerItemTable.UpdateDurabilityItem(item);
                                    item.Mode = Enums.ItemMode.Update;
                                    item.Send(attacker.Owner);
                                    item.Mode = Enums.ItemMode.Default;
                                }
                            }
                        }
                        if (i == 6)
                            break;
                    }
                    if (!attacker.Owner.Equipment.Free(10))
                    {
                        var item = attacker.Owner.Equipment.TryGetItem(10);
                        if (ServerBase.Kernel.Rate(20, 100))
                        {
                            if (item.Durability != 0)
                            {
                                item.Durability--;
                                if (item.Durability == 0)
                                    attacker.Owner.UnloadItemStats(item, true);
                                Database.ConquerItemTable.UpdateDurabilityItem(item);
                                item.Mode = Enums.ItemMode.Update;
                                item.Send(attacker.Owner);
                                item.Mode = Enums.ItemMode.Default;
                            }
                        }
                    }
                }
            #endregion
            #region Defence
            if (attacked != null)
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    for (byte i = 1; i <= 8; i++)
                    {
                        if (i == 4 || i == 6 || i == 7)
                            continue;
                        if (!attacked.Owner.Equipment.Free(i))
                        {
                            var item = attacked.Owner.Equipment.TryGetItem(i);
                            if (i == 5)
                            {
                                if (Network.PacketHandler.ItemPosition(item.ID) != 5 && Network.PacketHandler.IsArrow(item.ID))
                                {
                                    continue;
                                }
                            }
                            if (ServerBase.Kernel.Rate(30, 100))
                            {
                                if (item.Durability != 0)
                                {
                                    item.Durability--;
                                    if (item.Durability == 0)
                                        attacked.Owner.UnloadItemStats(item, true);
                                    Database.ConquerItemTable.UpdateDurabilityItem(item);

                                    item.Mode = Enums.ItemMode.Update;
                                    item.Send(attacked.Owner);
                                    item.Mode = Enums.ItemMode.Default;
                                }
                            }
                        }
                        if (i == 8)
                            break;
                    }
                    if (!attacked.Owner.Equipment.Free(11) && ServerBase.Kernel.Rate(30, 100))
                    {
                        var item = attacked.Owner.Equipment.TryGetItem(11);
                        if (ServerBase.Kernel.Rate(30, 100))
                        {
                            if (item.Durability != 0)
                            {
                                item.Durability--;
                                if (item.Durability == 0)
                                    attacked.Owner.UnloadItemStats(item, true);
                                Database.ConquerItemTable.UpdateDurabilityItem(item);
                                item.Mode = Enums.ItemMode.Update;
                                item.Send(attacked.Owner);
                                item.Mode = Enums.ItemMode.Default;
                            }
                        }
                    }
                }

            #endregion
            return;
        }

        private static void NewMethod(Database.SpellInformation spell)
        {
            if (spell != null)
                if (!spell.CanKill)
                    return;
        }

        private static void AutoRespone(Entity attacker, Entity attacked, ref int Damage)
        {
            try
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.CounterKillSwitch && ServerBase.Kernel.Rate(10) && !attacked.ContainsFlag(Update.Flags.Fly) && Time32.Now > attacker.CounterKillStamp.AddSeconds(15))
                    {
                        attacker.CounterKillStamp = Time32.Now;
                        uint damage = Melee(attacked, attacker);
                        Database.SpellInformation information = Database.SpellTable.SpellInformations[6003][attacked.Owner.Spells[6003].Level];
                        damage = (uint)(Percent((int)damage, information.Percent) / 100);
                        Network.GamePackets.Attack attack = new Conquer_Online_Server.Network.GamePackets.Attack(true);
                        attack.Attacked = attacker.UID;
                        attack.Attacker = attacked.UID;
                        attack.AttackType = Network.GamePackets.Attack.Scapegoat;
                        attack.Damage = 0;
                        attack.ResponseDamage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;

                        if (attacker.Hitpoints <= damage)
                        {
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                if (attacked.Owner.QualifierGroup != null)
                                    attacked.Owner.QualifierGroup.UpdateDamage(attacked.Owner, attacker.Hitpoints);
                                attacker.Owner.SendScreen(attack, true);
                                attacked.AttackPacket = null;
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                            attacker.Die(attacked);
                        }
                        else
                        {
                            attacker.Hitpoints -= damage;
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                if (attacked.Owner.QualifierGroup != null)
                                    attacked.Owner.QualifierGroup.UpdateDamage(attacked.Owner, damage);
                                attacker.Owner.SendScreen(attack, true);
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                        }
                        Damage = 0;
                    }
                    else if (attacked.Owner.Spells.ContainsKey(3060) && ServerBase.Kernel.Rate(10))
                    {
                        uint damage = (uint)(Damage / 10);
                        if (damage <= 0)
                            damage = 1;
                        Network.GamePackets.Attack attack = new Conquer_Online_Server.Network.GamePackets.Attack(true);
                        attack.Attacked = attacker.UID;
                        attack.Attacker = attacked.UID;
                        attack.AttackType = Network.GamePackets.Attack.Reflect;
                        attack.Damage = damage;
                        attack.ResponseDamage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;

                        if (attacker.Hitpoints <= damage)
                        {
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                if (attacked.Owner.QualifierGroup != null)
                                    attacked.Owner.QualifierGroup.UpdateDamage(attacked.Owner, attacker.Hitpoints);
                                attacker.Owner.SendScreen(attack, true);
                                attacked.AttackPacket = null;
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                            attacker.Die(attacked);
                        }
                        else
                        {
                            attacker.Hitpoints -= damage;
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                if (attacked.Owner.QualifierGroup != null)
                                    attacked.Owner.QualifierGroup.UpdateDamage(attacked.Owner, damage);
                                attacker.Owner.SendScreen(attack, true);
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                        }
                        Damage = 0;
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
        }
        public static int GetLevelBonus(int l1, int l2)
        {
            int num = l1 - l2;
            int bonus = 0;
            if (num >= 3)
            {
                num -= 3;
                bonus = 1 + (num / 5);
            }
            return bonus;
        }
        private static double AttackMultiplier(Entity attacker, Entity attacked)
        {
            if (attacked.Level > attacker.Level)
                return 1;
            return ((double)(attacker.Level - attacked.Level)) / 10 + 1;
        }
        public static ulong CalculateExpBonus(ushort Level, ushort MonsterLevel, ulong Experience)
        {
            int leveldiff = (2 + Level - MonsterLevel);
            if (leveldiff < -5)
                Experience = (ulong)(Experience * 1.3);
            else if (leveldiff < -1)
                Experience = (ulong)(Experience * 1.2);
            else if (leveldiff == 4)
                Experience = (ulong)(Experience * 0.8);
            else if (leveldiff == 5)
                Experience = (ulong)(Experience * 0.3);
            else if (leveldiff > 5)
                Experience = (ulong)(Experience * 0.1);
            return Experience;
        }

        internal static uint Melee(Entity attacker, Entity attacked, Database.SpellInformation spell, ref Attack attack)
        {
            throw new NotImplementedException();
        }

        internal static uint Melee(Entity attacker, SobNpcSpawn attackedsob, ref Attack attack)
        {
            throw new NotImplementedException();
        }
    }
}
