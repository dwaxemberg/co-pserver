using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game
{
    public class ClientStatus
    {
        #region Variables
        ushort _BattlePower;
        public Client.GameState client;
        public uint MaxAttack;
        public uint MinAttack;
        public uint Defense;
        public uint MagicDefense;
        public uint MagicAttack;
        public byte Dodge;
        public ushort HPAdd;
        public ushort MPAdd;
        public ushort Bless;
        public ushort Agility;
        public double DragonGems;
        public double PhoenixGems;
        public double Vigor;
        public ushort Detoxication;
        public ushort Immunity;
        public ushort Breaktrough;
        public ushort CriticalStrike;
        public ushort SkillCStrike;
        public ushort Intensification;
        public ushort Block;
        public ushort FinalMagicDmgPlus;
        public ushort FinalMagicDmgReduct;
        public ushort FinalDmgPlus;
        public ushort FinalDmgReduct;
        public ushort Penetration;
        public ushort Counteraction;
        public ushort MetalResistance;
        public ushort WoodResistance;
        public ushort WaterResistance;
        public ushort FireResistance;
        public ushort EarthResistance;
        #endregion
        #region Create Class
        public ClientStatus(Client.GameState _client)
        {
            client = _client;
        }
        #endregion
        #region Default
        public void AppendDefault()
        {
            Agility = 0;
            MaxAttack = 0;
            MinAttack = 0;
            Defense = 0;
            MagicDefense = 0;
            MagicAttack = 0;
            Dodge = 0;
            HPAdd = 0;
            MPAdd = 0;
            Bless = 0;
            Vigor = 0;
            _BattlePower = 0;
            DragonGems = 1.0;
            PhoenixGems = 1.0;
        }
        #endregion
        #region Statistics Get's
        public uint StatisticsGetMinAttack
        {
            get
            {
                if (GetMinAttack > 0)
                    return (uint)(GetMinAttack / 4);
                else return 1;
            }
        }

        public uint StatisticsGetMaxAttack
        {
            get
            {
                if (GetMaxAttack > 0)
                    return (uint)(GetMaxAttack / 4);
                else return 1;
            }
        }
        #endregion

        public ushort BattlePower
        {
            get { return _BattlePower; }
            set { _BattlePower = value; }
        }

        public uint GetMinAttack
        {
            get
            {
                if (MinAttack > 0)
                    return (uint)((MinAttack * DragonGems) * 0.9193);
                else return 1;
            }
        }

        public uint GetMaxAttack
        {
            get
            {
                if (MaxAttack > 0)
                    return (uint)((MaxAttack * DragonGems) * 0.9181);
                else return 1;
            }
        }

        public int GetAttack(bool Magic)
        {
            if (Magic)
            {
                return (int)(MagicAttack * PhoenixGems);
            }
            else
            {
                uint Damage = 0;

                if (GetMinAttack < GetMaxAttack)
                    Damage = (uint)ServerBase.Kernel.Random.Next((int)GetMinAttack, (int)GetMaxAttack);
                else if (GetMinAttack > GetMaxAttack)
                    Damage = (uint)ServerBase.Kernel.Random.Next((int)GetMaxAttack, (int)GetMinAttack);
                else Damage = (uint)ServerBase.Kernel.Random.Next((int)(GetMaxAttack * 0.8), (int)GetMaxAttack);

                return (int)Damage;
            }
        }

        public void AppendAddition(Interfaces.IConquerItem Item)
        {
            if (Item.ID >= 201003 && Item.ID <= 202009)
                return;

            #region Sub-Variables
            ushort immunity = 0;
            ushort intensification = 0;
            ushort detoxication = 0;
            ushort breaktrough = 0;
            ushort penetration = 0;
            ushort criticalstrike = 0;
            ushort skillcriticalstrike = 0;
            ushort counteraction = 0;
            ushort block = 0;
            ushort maxVal = 0;
            ushort minVal = 0;
            ushort mVal = 0;
            ushort mdVal = 0;
            ushort dVal = 0;
            ushort blessVal = 0;
            double phoenixVal = 0;
            double gemVal = 0;
            byte dodge = 0;
            #endregion
            #region Get Item Info
            if (Item != null)
            {
                if (Item.ID > 0)
                {
                    #region Data Variables
                    Database.ConquerItemInformation Data = new Conquer_Online_Server.Database.ConquerItemInformation(Item.ID, Item.Plus);
                    Database.ConquerItemInformation SoulData = null;
                    //if (Item.SoulPart > 0)
                    //{ SoulData = new Database.ConquerItemInformation(Item.SoulPart, 0); }
                    #endregion
                    if (Data.BaseInformation != null)
                    {
                        if (Item.Bless > 0)
                        { blessVal += Item.Bless; }
                        HPAdd += Item.Enchant;

                        if (SoulData != null)
                        {
                            if (SoulData.BaseInformation != null)
                            {
                                if (SoulData.BaseInformation.ID > 0)
                                {
                                    minVal += (ushort)(SoulData.BaseInformation.MinAttack);
                                    maxVal += (ushort)(SoulData.BaseInformation.MaxAttack);
                                    dVal += SoulData.BaseInformation.PhysicalDefence;
                                    mVal += SoulData.BaseInformation.MagicAttack;
                                    mdVal += SoulData.BaseInformation.MagicDefence;
                                    Agility += SoulData.BaseInformation.Agility;
                                    if (Item.Position == 8)
                                    { dodge += (byte)SoulData.BaseInformation.Dodge; }
                                }
                            }
                        }
                        HPAdd += Data.BaseInformation.ItemHP;
                        MPAdd += Data.BaseInformation.ItemMP;
                        Agility += Data.BaseInformation.Agility;
                        minVal += Data.BaseInformation.MinAttack;
                        maxVal += Data.BaseInformation.MaxAttack;
                        dVal += Data.BaseInformation.PhysicalDefence;
                        mVal += Data.BaseInformation.MagicAttack;
                        mdVal += Data.BaseInformation.MagicDefence;
                        if (Item.Position == 8)
                        { dodge += (byte)Data.BaseInformation.Dodge; }
                        if (Item.Position == 12)
                        { Vigor += (byte)Data.BaseInformation.Agility; }
                        if (Item.Plus > 0)
                        {
                            Agility += Data.PlusInformation.Agility;
                            HPAdd += Data.PlusInformation.ItemHP;
                            minVal += (ushort)Data.PlusInformation.MinAttack;
                            maxVal += (ushort)Data.PlusInformation.MaxAttack;
                            dVal += (ushort)Data.PlusInformation.PhysicalDefence;
                            mVal += (ushort)Data.PlusInformation.MagicAttack;
                            mdVal += (ushort)Data.PlusInformation.MagicDefence;
                            if (Item.Position == 8)
                            { dodge += (byte)Data.PlusInformation.Dodge; }
                            if (Item.Position == 12)
                            { Vigor += (byte)Data.PlusInformation.Agility; }
                        }

                        #region First Socket
                        if ((int)Item.SocketOne > 0)
                        {
                            switch (Item.SocketOne)
                            {
                                case Enums.Gem.NormalPhoenixGem: phoenixVal += 0.05; break;
                                case Enums.Gem.RefinedPhoenixGem: phoenixVal += 0.10; break;
                                case Enums.Gem.SuperPhoenixGem: phoenixVal += 0.15; break;
                                case Enums.Gem.NormalDragonGem: gemVal += 0.05; break;
                                case Enums.Gem.RefinedDragonGem: gemVal += 0.10; break;
                                case Enums.Gem.SuperDragonGem: gemVal += 0.15; break;
                                case Enums.Gem.NormalTortoiseGem: blessVal += 2; break;
                                case Enums.Gem.RefinedTortoiseGem: blessVal += 4; break;
                                case Enums.Gem.SuperTortoiseGem: blessVal += 6; break;
                            }
                        }
                        #endregion
                        #region Second Socket
                        if ((int)Item.SocketTwo > 0)
                        {
                            switch (Item.SocketTwo)
                            {
                                case Enums.Gem.NormalPhoenixGem: phoenixVal += 0.05; break;
                                case Enums.Gem.RefinedPhoenixGem: phoenixVal += 0.10; break;
                                case Enums.Gem.SuperPhoenixGem: phoenixVal += 0.15; break;
                                case Enums.Gem.NormalDragonGem: gemVal += 0.05; break;
                                case Enums.Gem.RefinedDragonGem: gemVal += 0.10; break;
                                case Enums.Gem.SuperDragonGem: gemVal += 0.15; break;
                                case Enums.Gem.NormalTortoiseGem: blessVal += 2; break;
                                case Enums.Gem.RefinedTortoiseGem: blessVal += 4; break;
                                case Enums.Gem.SuperTortoiseGem: blessVal += 6; break;
                            }
                        }
                        #endregion
                        #region Refinery Parts
                        switch ((Game.Features.Refinery.RefineryID)Item.RefineryPart)
                        {
                            #region Detoxication
                            case Features.Refinery.RefineryID.Detoxication:
                                detoxication += Item.RefineryPercent;
                                break;
                            #endregion
                            #region CriticalStrike
                            case Features.Refinery.RefineryID.Critical:
                                criticalstrike += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Block
                            case Features.Refinery.RefineryID.Block:
                                block += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Counteraction
                            case Features.Refinery.RefineryID.Counteraction:
                                counteraction += Item.RefineryPercent;
                                break;
                            #endregion
                            #region SkillCritical
                            case Features.Refinery.RefineryID.SkillCritical:
                                skillcriticalstrike += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Penetration
                            case Features.Refinery.RefineryID.Penetration:
                                penetration += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Immunity
                            case Features.Refinery.RefineryID.Immunity:
                                immunity += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Intensification
                            case Features.Refinery.RefineryID.Intensificaiton:
                                intensification += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Breaktrough
                            case Features.Refinery.RefineryID.Breaktrough:
                                breaktrough += Item.RefineryPercent;
                                break;
                            #endregion
                            default: break;
                        }
                        #endregion
                    }
                }
            }
            #endregion
            #region Setting Values
            BattlePower += Item.BattlePower;
            if (maxVal > 0 && minVal > 0)
            {
                MaxAttack += maxVal;
                MinAttack += minVal;
            }
            if (mVal > 0)
            {
                MagicAttack += mVal;
            }
            if (blessVal > 0)
            {
                Bless += blessVal;
            }
            if (dVal > 0)
            {
                Defense += dVal;
            }
            if (mdVal > 0)
            {
                MagicDefense += mdVal;
            }
            if (dodge > 0)
            {
                Dodge += (byte)dodge;
            }
            if (gemVal > 0)
            {
                DragonGems += gemVal;
            }
            if (phoenixVal > 0)
            {
                PhoenixGems += phoenixVal;
            }
            #endregion
            #region Setting Statistics
            client.Entity.Statistics.Detoxication += detoxication;
            client.Entity.Statistics.Intensification += intensification;
            client.Entity.Statistics.Immunity += immunity;
            client.Entity.Statistics.Penetration += penetration;
            client.Entity.Statistics.SkillCStrike += skillcriticalstrike;
            client.Entity.Statistics.CriticalStrike += criticalstrike;
            client.Entity.Statistics.Breaktrough += breaktrough;
            client.Entity.Statistics.Block += block;
            client.Entity.Statistics.Counteraction += counteraction;
            #endregion
            client.CalculateStatBonus();
            client.CalculateHPBonus();
        }

        public void AppendRemove(Interfaces.IConquerItem Item)
        {
            if (Item.ID >= 201003 && Item.ID <= 202009)
                return;

            #region Sub-Variables
            ushort immunity = 0;
            ushort intensification = 0;
            ushort detoxication = 0;
            ushort breaktrough = 0;
            ushort penetration = 0;
            ushort criticalstrike = 0;
            ushort skillcriticalstrike = 0;
            ushort counteraction = 0;
            ushort block = 0;
            ushort maxVal = 0;
            ushort minVal = 0;
            ushort mVal = 0;
            ushort mdVal = 0;
            ushort dVal = 0;
            ushort blessVal = 0;
            double phoenixVal = 0;
            double gemVal = 0;
            byte dodge = 0;
            #endregion
            #region Get Item Info
            if (Item != null)
            {
                if (Item.ID > 0)
                {
                    #region Data Variables
                    Database.ConquerItemInformation Data = new Conquer_Online_Server.Database.ConquerItemInformation(Item.ID, Item.Plus);
                    Database.ConquerItemInformation SoulData = null;
                    //if (Item.SoulPart > 0)
                    //{ SoulData = new Database.ConquerItemInformation(Item.SoulPart, 0); }
                    #endregion
                    if (Data.BaseInformation != null)
                    {
                        if (Item.Bless > 0)
                        { blessVal += Item.Bless; }
                        HPAdd -= Item.Enchant;
                        if (SoulData != null)
                        {
                            if (SoulData.BaseInformation != null)
                            {
                                if (SoulData.BaseInformation.ID > 0)
                                {
                                    minVal += (ushort)(SoulData.BaseInformation.MinAttack);
                                    maxVal += (ushort)(SoulData.BaseInformation.MaxAttack);
                                    dVal += SoulData.BaseInformation.PhysicalDefence;
                                    mVal += SoulData.BaseInformation.MagicAttack;
                                    mdVal += SoulData.BaseInformation.MagicDefence;
                                    Agility -= SoulData.BaseInformation.Agility;
                                    if (Item.Position == 8)
                                    { dodge += (byte)SoulData.BaseInformation.Dodge; }
                                }
                            }
                        }
                        Agility -= Data.BaseInformation.Agility;
                        HPAdd -= Data.BaseInformation.ItemHP;
                        MPAdd -= Data.BaseInformation.ItemMP;
                        minVal += Data.BaseInformation.MinAttack;
                        maxVal += Data.BaseInformation.MaxAttack;
                        dVal += Data.BaseInformation.PhysicalDefence;
                        mVal += Data.BaseInformation.MagicAttack;
                        mdVal += Data.BaseInformation.MagicDefence;
                        if (Item.Position == 8)
                        { dodge += (byte)Data.BaseInformation.Dodge; }
                        if (Item.Position == 12)
                        { Vigor -= (byte)Data.BaseInformation.Agility; }
                        if (Item.Plus > 0)
                        {
                            Agility -= Data.PlusInformation.Agility;
                            HPAdd -= Data.PlusInformation.ItemHP;
                            minVal += (ushort)Data.PlusInformation.MinAttack;
                            maxVal += (ushort)Data.PlusInformation.MaxAttack;
                            dVal += (ushort)Data.PlusInformation.PhysicalDefence;
                            mVal += (ushort)Data.PlusInformation.MagicAttack;
                            mdVal += (ushort)Data.PlusInformation.MagicDefence;
                            if (Item.Position == 8)
                            { dodge += (byte)Data.PlusInformation.Dodge; }
                            if (Item.Position == 12)
                            { Vigor -= (byte)Data.PlusInformation.Agility; }
                        }

                        #region First Socket
                        if ((int)Item.SocketOne > 0)
                        {
                            switch (Item.SocketOne)
                            {
                                case Enums.Gem.NormalPhoenixGem: phoenixVal += 0.05; break;
                                case Enums.Gem.RefinedPhoenixGem: phoenixVal += 0.10; break;
                                case Enums.Gem.SuperPhoenixGem: phoenixVal += 0.15; break;
                                case Enums.Gem.NormalDragonGem: gemVal += 0.05; break;
                                case Enums.Gem.RefinedDragonGem: gemVal += 0.10; break;
                                case Enums.Gem.SuperDragonGem: gemVal += 0.15; break;
                                case Enums.Gem.NormalTortoiseGem: blessVal += 2; break;
                                case Enums.Gem.RefinedTortoiseGem: blessVal += 4; break;
                                case Enums.Gem.SuperTortoiseGem: blessVal += 6; break;
                            }
                        }
                        #endregion
                        #region Second Socket
                        if ((int)Item.SocketTwo > 0)
                        {
                            switch (Item.SocketTwo)
                            {
                                case Enums.Gem.NormalPhoenixGem: phoenixVal += 0.05; break;
                                case Enums.Gem.RefinedPhoenixGem: phoenixVal += 0.10; break;
                                case Enums.Gem.SuperPhoenixGem: phoenixVal += 0.15; break;
                                case Enums.Gem.NormalDragonGem: gemVal += 0.05; break;
                                case Enums.Gem.RefinedDragonGem: gemVal += 0.10; break;
                                case Enums.Gem.SuperDragonGem: gemVal += 0.15; break;
                                case Enums.Gem.NormalTortoiseGem: blessVal += 2; break;
                                case Enums.Gem.RefinedTortoiseGem: blessVal += 4; break;
                                case Enums.Gem.SuperTortoiseGem: blessVal += 6; break;
                            }
                        }
                        #endregion
                        #region Refinery Parts
                        switch ((Game.Features.Refinery.RefineryID)Item.RefineryPart)
                        {
                            #region Detoxication
                            case Features.Refinery.RefineryID.Detoxication:
                                detoxication += Item.RefineryPercent;
                                break;
                            #endregion
                            #region CriticalStrike
                            case Features.Refinery.RefineryID.Critical:
                                criticalstrike += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Block
                            case Features.Refinery.RefineryID.Block:
                                block += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Counteraction
                            case Features.Refinery.RefineryID.Counteraction:
                                counteraction += Item.RefineryPercent;
                                break;
                            #endregion
                            #region SkillCritical
                            case Features.Refinery.RefineryID.SkillCritical:
                                skillcriticalstrike += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Penetration
                            case Features.Refinery.RefineryID.Penetration:
                                penetration += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Immunity
                            case Features.Refinery.RefineryID.Immunity:
                                immunity += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Intensification
                            case Features.Refinery.RefineryID.Intensificaiton:
                                intensification += Item.RefineryPercent;
                                break;
                            #endregion
                            #region Breaktrough
                            case Features.Refinery.RefineryID.Breaktrough:
                                breaktrough += Item.RefineryPercent;
                                break;
                            #endregion
                            default: break;
                        }
                        #endregion
                    }
                }
                else return;
            }
            #endregion
            #region Setting Values
            BattlePower -= Item.BattlePower;
            if (maxVal > 0 && minVal > 0)
            {
                MaxAttack -= maxVal;
                MinAttack -= minVal;
            }
            if (mVal > 0)
            {
                MagicAttack -= mVal;
            }
            if (blessVal > 0)
            {
                Bless -= blessVal;
            }
            if (dVal > 0)
            {
                Defense -= dVal;
            }
            if (mdVal > 0)
            {
                MagicDefense -= mdVal;
            }
            if (dodge > 0)
            {
                Dodge -= (byte)dodge;
            }
            if (gemVal > 0)
            {
                DragonGems -= gemVal;
            }
            if (phoenixVal > 0)
            {
                PhoenixGems -= phoenixVal;
            }
            #endregion
            #region Setting Statistics
            client.Entity.Statistics.Detoxication -= detoxication;
            client.Entity.Statistics.Intensification -= intensification;
            client.Entity.Statistics.Immunity -= immunity;
            client.Entity.Statistics.Penetration -= penetration;
            client.Entity.Statistics.SkillCStrike -= skillcriticalstrike;
            client.Entity.Statistics.CriticalStrike -= criticalstrike;
            client.Entity.Statistics.Breaktrough -= breaktrough;
            client.Entity.Statistics.Block -= block;
            client.Entity.Statistics.Counteraction -= counteraction;
            #endregion
            client.CalculateStatBonus();
            client.CalculateHPBonus();
        }

        public void AppendStatusMessage()
        {
            client.Send(new Message("Max/Min/Magic = [" + MaxAttack + "|" + MinAttack + "|" + MagicAttack + "], Def/MDef/Dodge/Bless = [" + Defense + "|" + MagicDefense + "|" + Dodge + "|" + Bless + "], Phoenix/Dragon = [" + PhoenixGems + "|" + DragonGems + "], BattlePower = [" + BattlePower + "]", System.Drawing.Color.Red, Message.Talk));
            client.Send(new Message("MaxDmg/MinDmg = [" + GetMaxAttack + "|" + GetMinAttack + "]", System.Drawing.Color.Red, Message.Talk));
        }
    }
}
