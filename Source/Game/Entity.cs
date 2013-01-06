using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Network.GamePackets;
using System.Collections;
using Conquer_Online_Server.Network.Features.ClassPKWar;
using Conquer_Online_Server.ServerBase;

namespace Conquer_Online_Server.Game
{
    public class Entity : Writer, Interfaces.IBaseEntity, Interfaces.IMapObject
    {
        public Clan GetClan
        {
            get
            {
                Clan cl;
                Kernel.Clans.TryGetValue(ClanId, out cl);
                return cl;
            }
        }
        public int _Topmonk3 = 0;
        public int TopMonk2
        {

            get { return _TopPirate2; }
            set
            {
                _TopPirate2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Monk);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        public int _TopTrojan2 = 0;
        public int TopTrojan2
        {
            get { return _TopTrojan2; }
            set
            {
                _TopTrojan2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Trojan);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        public ushort _TopWarrior2 = 0;
        public ushort TopWarrior2
        {
            get { return _TopWarrior2; }
            set
            {
                _TopWarrior2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Warrior);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        public int _TopNinja2 = 0;
        public int TopNinja2
        {
            get { return _TopNinja2; }
            set
            {
                _TopNinja2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Ninja);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        public int _TopWaterTaoist2 = 0;
        public int TopWaterTaoist2
        {
            get { return _TopWaterTaoist2; }
            set
            {
                _TopWaterTaoist2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Water);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        public int _TopArcher2 = 0;
        public int TopArcher2
        {
            get { return _TopArcher2; }
            set
            {
                _TopArcher2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Archer);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        public int _TopFireTaoist2 = 0;
        public int TopFireTaoist2
        {
            get { return _TopFireTaoist2; }
            set
            {
                _TopFireTaoist2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Fire);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        
        public int _TopPirate2 = 0;
        public int TopPirate2
        {
            get { return _TopPirate2; }
            set
            {
                _TopPirate2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Pirate);
                    Conquer_Online_Server.Database.EntityTable.SaveTop2(this);
                }
            }
        }
        public bool Vot_null = false;
        public DateTime StartVote = DateTime.Now;
        private ushort _vots = 0;
        public bool addVot = false;
        public DateTime TimerVot = DateTime.Now;
        public void SaveTimeVot()
        {
            Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
            cmd.Update("entities").Set("TimerVot", TimerVot.Ticks).Where("UID", this.UID).Execute();
        }
        public ushort VotsPoints
        {
            get { return _vots; }
            set
            {
                _vots = value;
                Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                cmd.Update("entities").Set("VotsPoints", value).Where("UID", this.UID).Execute();
            }
        }
        #region Constantes
        public Game.ConquerStructures.Society.Guild Guild;
        public uint tID = 0;
        private uint _boundCps;
        public uint DisKO = 0;
        public bool DisQuest = false;
        public uint CriticalStrike = 0, SkillCriticalStrike = 0, Immunity = 0, Penetration = 0, Block = 0, Breakthrough = 0, Detoxication = 0, Counteraction = 0, ResistMetal = 0, ResistWood = 0, ResistWater = 0, ResistFire = 0, ResistEarth = 0;
        public bool KillTheTerrorist_IsTerrorist = false;
        public bool Tournament_Signed = false;
        public bool SpawnProtection = false;
        public bool TeamDeathMatch_Signed = false;
        public bool TeamDeathMatch_BlueTeam = false;
        public int TeamDeathMatch_Kills = 0;
        public bool TeamDeathMatch_RedCaptain = false;
        public bool TeamDeathMatch_BlackCaptain = false;
        public bool TeamDeathMatch_WhiteCaptain = false;
        public bool TeamDeathMatch_BlueCaptain = false;
        public bool TeamDeathMatch_RedTeam = false;
        public bool TeamDeathMatch_BlackTeam = false;
        public bool TeamDeathMatch_WhiteTeam = false;
        public uint ExpBallExp
        {
            get
            {
                if (Level < 30)
                    return (uint)(15000 + Level * 430);
                else if (Level < 50)
                    return (uint)(40000 + Level * 430);
                else if (Level < 80)
                    return (uint)(30000 + Level * 500);
                else if (Level < 80)
                    return (uint)(30000 + Level * 600);
                else if (Level < 100)
                    return (uint)(30000 + Level * 700);
                else if (Level < 110)
                    return (uint)(30000 + Level * 900);
                else if (Level < 120)
                    return (uint)(30000 + Level * 1100);
                else if (Level < 125)
                    return (uint)(30000 + Level * 1500);
                else if (Level < 130)
                    return (uint)(30000 + Level * 1000);
                else
                    return (uint)(30000 + Level * 1000);
            }
        }
        #endregion
        #region Fan/Tower Acessor
        public int getFan(bool Magic)
        {
            if (EntityFlag == EntityFlag.Monster)
                return 0;
            if (Owner.Equipment.Free(10))
                return 0;

            ushort magic = 0;
            ushort physical = 0;
            ushort gemVal = 0;

            #region Get
            Interfaces.IConquerItem Item = this.Owner.Equipment.TryGetItem(10);

            if (Item != null)
            {
                if (Item.ID > 0)
                {
                    switch (Item.ID % 10)
                    {
                        case 3:
                        case 4:
                        case 5: physical += 300; magic += 150; break;
                        case 6: physical += 500; magic += 200; break;
                        case 7: physical += 700; magic += 300; break;
                        case 8: physical += 900; magic += 450; break;
                        case 9: physical += 1200; magic += 750; break;
                    }

                    switch (Item.Plus)
                    {
                        case 0: break;
                        case 1: physical += 200; magic += 100; break;
                        case 2: physical += 400; magic += 200; break;
                        case 3: physical += 600; magic += 300; break;
                        case 4: physical += 800; magic += 400; break;
                        case 5: physical += 1000; magic += 500; break;
                        case 6: physical += 1200; magic += 600; break;
                        case 7: physical += 1300; magic += 700; break;
                        case 8: physical += 1400; magic += 800; break;
                        case 9: physical += 1500; magic += 900; break;
                        case 10: physical += 1600; magic += 950; break;
                        case 11: physical += 1700; magic += 1000; break;
                        case 12: physical += 1800; magic += 1050; break;
                    }
                    switch (Item.SocketOne)
                    {
                        case Enums.Gem.NormalThunderGem: gemVal += 100; break;
                        case Enums.Gem.RefinedThunderGem: gemVal += 300; break;
                        case Enums.Gem.SuperThunderGem: gemVal += 500; break;
                    }
                    switch (Item.SocketTwo)
                    {
                        case Enums.Gem.NormalThunderGem: gemVal += 100; break;
                        case Enums.Gem.RefinedThunderGem: gemVal += 300; break;
                        case Enums.Gem.SuperThunderGem: gemVal += 500; break;
                    }
                }
            }
            #endregion


            magic += gemVal;
            physical += gemVal;

            if (Magic)
                return (int)magic;
            else
                return (int)physical;
        }

        public int getTower(bool Magic)
        {
            if (EntityFlag == EntityFlag.Monster)
                return 0;
            if (Owner.Equipment.Free(11))
                return 0;

            ushort magic = 0;
            ushort physical = 0;
            ushort gemVal = 0;

            #region Get
            Interfaces.IConquerItem Item = this.Owner.Equipment.TryGetItem(11);

            if (Item != null)
            {
                if (Item.ID > 0)
                {
                    switch (Item.ID % 10)
                    {
                        case 3:
                        case 4:
                        case 5: physical += 250; magic += 100; break;
                        case 6: physical += 400; magic += 150; break;
                        case 7: physical += 550; magic += 200; break;
                        case 8: physical += 700; magic += 300; break;
                        case 9: physical += 1100; magic += 600; break;
                    }

                    switch (Item.Plus)
                    {
                        case 0: break;
                        case 1: physical += 150; magic += 50; break;
                        case 2: physical += 350; magic += 150; break;
                        case 3: physical += 550; magic += 250; break;
                        case 4: physical += 750; magic += 350; break;
                        case 5: physical += 950; magic += 450; break;
                        case 6: physical += 1100; magic += 550; break;
                        case 7: physical += 1200; magic += 625; break;
                        case 8: physical += 1300; magic += 700; break;
                        case 9: physical += 1400; magic += 750; break;
                        case 10: physical += 1500; magic += 800; break;
                        case 11: physical += 1600; magic += 850; break;
                        case 12: physical += 1700; magic += 900; break;
                    }
                    switch (Item.SocketOne)
                    {
                        case Enums.Gem.NormalGloryGem: gemVal += 100; break;
                        case Enums.Gem.RefinedGloryGem: gemVal += 300; break;
                        case Enums.Gem.SuperGloryGem: gemVal += 500; break;
                    }
                    switch (Item.SocketTwo)
                    {
                        case Enums.Gem.NormalGloryGem: gemVal += 100; break;
                        case Enums.Gem.RefinedGloryGem: gemVal += 300; break;
                        case Enums.Gem.SuperGloryGem: gemVal += 500; break;
                    }
                }
            }
            #endregion

            magic += gemVal;
            physical += gemVal;

            if (Magic)
                return (int)magic;
            else
                return (int)physical;
        }
        #endregion
           public void SetVisible()
        {
            SpawnPacket[99] = 0;
        }
        
        public uint InteractionType = 0;
        public uint InteractionWith = 0;
        public bool InteractionInProgress = false;
        public ushort InteractionX = 0;
        public ushort InteractionY = 0;
        public bool InteractionSet = false;
        public int cp = 0;
        public StatusStatics Statistics;
        public DateTime SteedRaceTime;
        public int srjoin = 0;
        public ushort _ElitePK = 0;
        public ushort ElitePK
        {
            get { return _ElitePK; }
            set
            {
                _ElitePK = value;
                if (value >= 1)
                {
                    TitleActivated = 18;
                    Database.SubClassTable.Update56(this);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        #region New QQ
        public int _QQ1 = 0;
        public int QQ1
        {
            get { return _QQ1; }
            set
            {
                _QQ1 = value;
                if (value >= 1)
                {
                    // AddFlag(Network.GamePackets.Update.Flags.TopFireTaoist);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _QQ2 = 0;
        public int QQ2
        {
            get { return _QQ2; }
            set
            {
                _QQ2 = value;
                if (value >= 1)
                {
                    // AddFlag(Network.GamePackets.Update.Flags.TopFireTaoist);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _QQ3 = 0;
        public int QQ3
        {
            get { return _QQ3; }
            set
            {
                _QQ3 = value;
                if (value >= 1)
                {
                    // AddFlag(Network.GamePackets.Update.Flags.TopFireTaoist);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        #endregion
        public BlessingContribution Contribution_Experience = null; 
        public byte reinc;
        public byte ReincarnationLev
        {
            get
            {
                return reinc;
            }
            set
            {
                reinc = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    if (FullyLoaded)
                        UpdateDatabase("ReincarnationLev", value);
                }
            }
        }
        public bool JustCreated = false;
                public Game.Struct.Flowers MyFlowers = null;
        public int _Topmonk2 = 0;
        public int TopMonk
        {
            get { return _Topmonk2; }
            set
            {
                _Topmonk2 = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.Flashy);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        private uint _LastNpc;
        public uint LastNpc
        {
            get { return _LastNpc; }
            set { _LastNpc = value; }
        }
        public int _TopTrojan = 0;
        public int TopTrojan
        {
            get { return _TopTrojan; }
            set
            {
                _TopTrojan = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopTrojan);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public ushort _TopWarrior = 0;
        public ushort TopWarrior
        {
            get { return _TopWarrior; }
            set
            {
                _TopWarrior = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopWarrior);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _TopNinja = 0;
        public int TopNinja
        {
            get { return _TopNinja; }
            set
            {
                _TopNinja = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopPirate);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _TopWaterTaoist = 0;
        public int TopWaterTaoist
        {
            get { return _TopWaterTaoist; }
            set
            {
                _TopWaterTaoist = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopWaterTaoist);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _TopArcher = 0;
        public int TopArcher
        {
            get { return _TopArcher; }
            set
            {
                _TopArcher = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopArcher);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _TopGuildLeader = 0;
        public int TopGuildLeader
        {
            get { return _TopGuildLeader; }
            set
            {
                _TopGuildLeader = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopGuildLeader);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }//
            }
        }
        public int _TopFireTaoist = 0;
        public int TopFireTaoist
        {
            get { return _TopFireTaoist; }
            set
            {
                _TopFireTaoist = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopFireTaoist);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _TopDeputyLeader = 0;
        public int TopDeputyLeader
        {
            get { return _TopDeputyLeader; }
            set
            {
                _TopDeputyLeader = value;
                if (value >= 1)
                {
                    AddFlag(Network.GamePackets.Update.Flags.TopDeputyLeader);
                    Conquer_Online_Server.Database.EntityTable.SaveTop(this);
                }
            }
        }
        public int _WeeklyPKChampion = 0;
        public int WeeklyPKChampion
        {
            get { return _WeeklyPKChampion; }
            set
            {
                _WeeklyPKChampion = value;
                if (value >= 1)
                    AddFlag(Network.GamePackets.Update.Flags.WeeklyPKChampion);
                Conquer_Online_Server.Database.EntityTable.SaveTop(this);
            }
        }
        public uint RidingCropID = 0;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                SpawnPacket = new byte[8 + 230 + _Name.Length];
                WriteUInt16((ushort)(230 + _Name.Length), 0, SpawnPacket);
                WriteUInt16(10014, 2, SpawnPacket);
                WriteStringList(new List<string> { _Name }, 226, SpawnPacket);
            }
        }
        uint f_flower;
        public uint ActualMyTypeFlower
        {
            get { return f_flower; }
            set
            {
                //30010202 orchids
                //30010002 rouse
                //30010102 lilyes
                //30010302 orchids

                f_flower = value;
                WriteUInt32(value, 113, SpawnPacket);//91
            }
        }
        private uint flower_R;
        public uint AddFlower
        {
            get { return flower_R; }
            set
            {
                flower_R = value;

                Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                cmd.Update("entities").Set("Flower", value).Where("UID", this.UID).Execute();
            }
        }
        public uint _vigor;
        public uint Vigor
        {
            get { return _vigor; }
            set
            {
                _vigor = value;
                if (_vigor > MaxVigor) _vigor = MaxVigor;
                switch (EntityFlag)
                {
                    case EntityFlag.Player:
                        if (FullyLoaded)
                        {
                            Network.GamePackets.Vigor vigor = new Network.GamePackets.Vigor(true);
                            vigor.Type = 2;
                            vigor.VigorValue = _vigor;
                            if (Owner != null)
                            { this.Owner.Send(vigor); }
                        }
                        break;
                }
            }
        }
        public uint MaxVigor
        {
            get
            {
                uint __Vigor = 0;
                Interfaces.IConquerItem IT = Owner.Equipment.TryGetItem(12);

                if (IT != null)
                {
                    switch (IT.Plus)
                    {
                        case 1: __Vigor = 50; break;
                        case 2: __Vigor = 120; break;
                        case 3: __Vigor = 200; break;
                        case 4: __Vigor = 350; break;
                        case 5: __Vigor = 650; break;
                        case 6: __Vigor = 1000; break;
                        case 7: __Vigor = 1400; break;
                        case 8: __Vigor = 2000; break;
                        case 9: __Vigor = 2800; break;
                        case 10: __Vigor = 3100; break;
                        case 11: __Vigor = 3500; break;
                        case 12: __Vigor = 4000; break;
                    }
                }

                return (uint)(30 + __Vigor);
            }
        }
        public bool InSteedRace = false;
        public Tournaments.Elite_client Elite;

        public Tournaments.Team_client Elite2;
        public byte TitleActivated
        {
            get { return SpawnPacket[167]; }
            set { SpawnPacket[167] = value; }
        }
        public TitlePacket TitlePacket = null;
        public uint ClanId
        {
            get { return BitConverter.ToUInt32(SpawnPacket, 155); }
            set { WriteUInt32(value, 155, SpawnPacket); }
        }
        public Game.Clans Myclan;
        public byte ClanRank
        {
            get { return SpawnPacket[159]; }
            set { SpawnPacket[159] = value; /*100 is for clan lider*/ }
        }
        string clan = "";
        public string ClanName
        {
            get { return clan; }
            set
            {
                string oldclan = clan;
                clan = value;
                if (value != null)
                {

                    if (value != "")
                    {
                        byte[] dd33 = new byte[8 + 229 + Name.Length + value.Length + 2];
                        for (int i = 2; i < SpawnPacket.Length - 7; i++)
                        {
                            dd33[i] = SpawnPacket[i];
                        }

                        SpawnPacket = new byte[8 + 229 + Name.Length + value.Length + 2];
                        WriteUInt16((ushort)(229 + Name.Length + value.Length + 2), 0, SpawnPacket);

                        for (int i = 2; i < dd33.Length; i++)
                        {
                            SpawnPacket[i] = dd33[i];
                        }

                        WriteUInt16(10014, 2, SpawnPacket);




                        SpawnPacket[226] = 4;
                        SpawnPacket[227] = (byte)_Name.Length;
                        WriteString(_Name, 228, SpawnPacket);
                        SpawnPacket[227 + SpawnPacket[227] + 2] = (byte)value.Length;
                        WriteString(value, 227 + SpawnPacket[227] + 3, SpawnPacket);
                    }
                    else
                    {
                        byte[] dd33 = new byte[8 + 221 + Name.Length + 2];
                        for (int i = 2; i < SpawnPacket.Length - 8; i++)
                        {
                            if (i < dd33.Length)
                                dd33[i] = SpawnPacket[i];
                        }

                        SpawnPacket = new byte[8 + 229 + Name.Length + 2];
                        WriteUInt16((ushort)(229 + Name.Length + 2), 0, SpawnPacket);

                        for (int i = 2; i < dd33.Length; i++)
                        {
                            SpawnPacket[i] = dd33[i];
                        }

                        WriteUInt16(10014, 2, SpawnPacket);





                        SpawnPacket[226] = 4;
                        SpawnPacket[227] = (byte)_Name.Length;
                        WriteString(_Name, 228, SpawnPacket);
                        SpawnPacket[227 + SpawnPacket[227] + 2] = (byte)value.Length;
                        WriteString(value, 227 + SpawnPacket[227] + 3, SpawnPacket);

                    }
                }
            }
        }
        #region Variables
        public Database.MonsterInformation MonsterInfo;
        public Time32 DeathStamp, VortexAttackStamp,BlackBeardStamp, CannonBarageStamp, AttackStamp, StaminaStamp, FlashingNameStamp, CycloneStamp, SupermanStamp,
                      StigmaStamp, InvisibilityStamp, StarOfAccuracyStamp, MagicShieldStamp, DodgeStamp, EnlightmentStamp,
                      AccuracyStamp, ShieldStamp, FlyStamp, NoDrugsStamp, ToxicFogStamp, FatalStrikeStamp, DoubleExpStamp,
                      ShurikenVortexStamp, IntensifyStamp, TransformationStamp, CounterKillStamp, PKPointDecreaseStamp,
                      HeavenBlessingStamp, OblivionStamp, ShackleStamp, AzureStamp, CPsStamp, StunStamp, DoubleExpStamp5, DoubleExpStamp10, DoubleExpStamp15, WhilrwindKick, Confuse;



        public Time32
           TyrantAura ,
           FendAura ,
           MetalAura ,
           WoodAura ,
           WaterAura ,
           FireAura ,
           EarthAura ;

         public short

           TyrantAuras=0,
           FendAuras = 0,
           MetalAuras = 0,
           WoodAuras = 0,
           WaterAuras = 0,
           FireAuras = 0,
           EarthAuras = 0;


        public bool Stunned = false, Confused = false;
        public bool Companion;
        public bool CauseOfDeathIsMagic = false;
        public bool OnIntensify;
        public Game.ConquerStructures.Inventory Inventory;
        public short KOSpellTime
        {
            get
            {
                if (KOSpell == 1110)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Cyclone))
                    {
                        return CycloneTime;
                    }
                }
                else if (KOSpell == 1025)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Superman))
                    {
                        return SupermanTime;
                    }
                }
                return 0;
            }
            set
            {
                if (KOSpell == 1110)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Cyclone))
                    {
                        int Seconds = CycloneStamp.AddSeconds(value).AllSeconds() - Time32.Now.AllSeconds();
                        if (Seconds >= 20)
                        {
                            CycloneTime = 20;
                            CycloneStamp = Time32.Now;
                        }
                        else
                        {
                            CycloneTime = (short)Seconds;
                            CycloneStamp = Time32.Now;
                        }
                    }
                }
                if (KOSpell == 1025)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Superman))
                    {
                        int Seconds = SupermanStamp.AddSeconds(value).AllSeconds() - Time32.Now.AllSeconds();
                        if (Seconds >= 20)
                        {
                            SupermanTime = 20;
                            SupermanStamp = Time32.Now;
                        }
                        else
                        {
                            SupermanTime = (short)Seconds;
                            SupermanStamp = Time32.Now;
                        }
                    }
                }
            }
        }
        public short CycloneTime = 0, SupermanTime = 0, NoDrugsTime = 0, FatalStrikeTime = 0,  Cannonbarage = 0, Blackbeard = 0, ShurikenVortexTime = 0, OblivionTime = 0, ShackleTime = 0, AzureTime;
        public ushort KOSpell = 0;
        public int AzureDamage = 0;
        private ushort _enlightenPoints;
        private byte _receivedEnlighenPoints;
        private ushort _enlightmenttime;
        public float ToxicFogPercent, StigmaIncrease, MagicShieldIncrease, DodgeIncrease, ShieldIncrease;
        public byte ToxicFogLeft, FlashingNameTime, FlyTime, StigmaTime, InvisibilityTime, StarOfAccuracyTime, MagicShieldTime, DodgeTime, AccuracyTime, ShieldTime;
        public ushort KOCount = 0;
        public bool CounterKillSwitch = false;
        public Network.GamePackets.Attack AttackPacket;
        public Network.GamePackets.Attack VortexPacket;
        public byte[] SpawnPacket;
        private string _Name, _Spouse;
        private ushort _Defence, _MDefence, _MDefencePercent;
        private Client.GameState _Owner;
        public ushort ItemHP = 0, ItemMP = 0, ItemBless = 0, PhysicalDamageDecrease = 0, PhysicalDamageIncrease = 0, MagicDamageDecrease = 0, MagicDamageIncrease = 0, AttackRange = 1, ExtraVigor = 0;
        public ushort[] Gems = new ushort[10];
        private uint _MinAttack, _MaxAttack, _MagicAttack;
        public uint BaseMinAttack, BaseMaxAttack, BaseMagicAttack, BaseDefence, BaseMagicDefence;
        private uint _TransMinAttack, _TransMaxAttack, _TransDodge, _TransPhysicalDefence, _TransMagicDefence;
        public bool Killed = false;
        public bool Transformed
        {
            get
            {
                return TransformationID != 98 && TransformationID != 99 && TransformationID != 0;
            }
        }
        public uint TransformationAttackRange = 0;
        public int TransformationTime = 0;
        public uint TransformationMaxHP = 0;
        private byte _Dodge;
        private Enums.PKMode _PKMode;
        private EntityFlag _EntityFlag;
        private MapObjectType _MapObjectType;
        public Enums.Mode Mode;
        private ulong _experience;
        private uint _heavenblessing, _money, _uid, _hitpoints, _maxhitpoints, _quizpoints;
        private uint _conquerpoints;
        private ushort CpsB, _doubleexp
            , _doubleexp5
            , _doubleexp10
            , _doubleexp15, _body, _transformationid, _face, _strength, _agility, _spirit, _vitality, _atributes, _mana, _maxmana, _hairstyle, _mapid, _previousmapid, _x, _y, _pkpoints;
        private byte _stamina, _class, _reborn, _level;
        public byte FirstRebornClass
        {
            get
            {
                return SpawnPacket[211];
            }
            set
            {
                SpawnPacket[211] = value;
            }
        }
        public byte SecondRebornClass
        {
            get
            {
                return SpawnPacket[213];
            }
            set
            {
                SpawnPacket[213] = value;
            }
        }
        public byte FirstRebornLevel, SecondRebornLevel;
        public bool FullyLoaded = false, SendUpdates = false, HandleTiming = false;
        private Network.GamePackets.Update update;

        #endregion
        #region Acessors
        public int BattlePower
        {
            get
            {
                int potBase = (int)(Level + Reborn * 5);//+ExtraBattlePower);
                potBase += (byte)NobilityRank;
                if (EntityFlag == EntityFlag.Player)
                {
                    foreach (Interfaces.IConquerItem item in Owner.Equipment.Objects)
                    {
                        if (item != null)
                        {
                            potBase += item.BattlePower;
                        }
                    }
                }
                return Math.Min(potBase, 400);
            }
        }

        public byte _SubClass, _SubClassLevel;

        private uint Guild_______Points;

        public uint Guild_points
        {
            get { return Guild_______Points; }
            set
            {
                Guild_______Points = value;
                switch (EntityFlag)
                {
                    case EntityFlag.Player:
                        if (FullyLoaded)
                        { UpdateDatabase("GuildPoints", Guild_______Points); }
                        break;
                }
            }
        }
        public byte SubclassActivated
        {
            get { return SpawnPacket[198]; }
            set { SpawnPacket[198] = value; }
        }
        public Game.ClientClasses SubClasses = new Game.ClientClasses();
      
        private ulong StatusFlag1
        {
            get
            {
                return BitConverter.ToUInt64(SpawnPacket, 22);
            }
            set
            {
                ulong OldV = StatusFlag1;
                if (value != OldV)
                {
                    WriteUInt64(value, 22, SpawnPacket);
                    Update(Network.GamePackets.Update.StatusFlag, value, StatusFlag2, !ContainsFlag(Network.GamePackets.Update.Flags.XPList));
                }
            }
        }
        public ulong StatusFlag2
        {
            get
            {
                return BitConverter.ToUInt64(SpawnPacket, 30);
            }
            set
            {
                ulong OldV = StatusFlag2;
                if (value != OldV)
                {
                    WriteUInt64(value, 30, SpawnPacket);
                    Update(Network.GamePackets.Update.StatusFlag, StatusFlag1, value, !ContainsFlag(Network.GamePackets.Update.Flags.XPList));
                }
            }
        }

        public void SetFlag(ulong val1, ulong val2)
        {
            StatusFlag1 = val1;
            StatusFlag2 = val2;
        }
        public byte SubClass
        {
            get { return _SubClass; }
            set
            {
                _SubClass = value;
                switch (EntityFlag)
                {
                    case EntityFlag.Player:
                        if (FullyLoaded)
                        { UpdateDatabase("SubClass", _SubClass); }
                        SpawnPacket[196] = _SubClass;
                        break;
                }
            }
        }

        public byte SubClassLevel
        {
            get { return _SubClassLevel; }
            set
            {
                _SubClassLevel = value;
                switch (EntityFlag)
                {
                    case EntityFlag.Player:
                        if (FullyLoaded)
                        { UpdateDatabase("SubClassLevel", value); }

                        break;
                }
            }
        }
        public string Spouse
        {
            get
            {
                return _Spouse;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets._String.Spouse, value, false);
                }
                _Spouse = value;
            }
        }
        public uint Money
        {
            get
            {
                return _money;
            }
            set
            {
                _money = value;
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Money, value, false);


            }
        }
        private byte _vipLevel;
        public byte VIPLevel
        {
            get
            {
                return _vipLevel;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.VIPLevel, value, false);
                }
                _vipLevel = value;
            }
        }
        public uint ConquerPoints
        {
            get
            {
                return _conquerpoints;
            }
            set
            {
                if (value <= 0)
                    value = 0;

                _conquerpoints = value;

                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.ConquerPoints, (uint)value, false);
            }
        }
        public ushort Body
        {
            get
            {
                return _body;
            }
            set
            {
                WriteUInt32((uint)(TransformationID * 10000000 + Face * 10000 + value), 4, SpawnPacket);
                _body = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner != null)
                    {
                        if (Owner.ArenaStatistic != null)
                            Owner.ArenaStatistic.Model = (uint)(Face * 10000 + value);
                        Update(Network.GamePackets.Update.Mesh, Mesh, true);
                    }
                }
            }
        }
        public ushort DoubleExperienceTime
        {
            get
            {
                return _doubleexp;
            }
            set
            {
                ushort oldVal = DoubleExperienceTime;
                _doubleexp = value;
                if (FullyLoaded)
                    if (oldVal <= _doubleexp)
                        if (EntityFlag == EntityFlag.Player)
                        {
                            if (Owner != null)
                            {
                                Update(Network.GamePackets.Update.DoubleExpTimer, DoubleExperienceTime, false);
                            }
                        }
            }
        }
        public ushort DoubleExperienceTime5
        {
            get
            {
                return _doubleexp5;
            }
            set
            {
                ushort oldVal3 = DoubleExperienceTime5;
                _doubleexp5 = value;
                if (FullyLoaded)
                    if (oldVal3 <= _doubleexp5)
                        if (EntityFlag == EntityFlag.Player)
                        {
                            if (Owner != null)
                            {
                                Update(Network.GamePackets.Update.DoubleExpTimer, DoubleExperienceTime5, false);
                            }
                        }
            }
        }
        public ushort DoubleExperienceTime10
        {
            get
            {
                return _doubleexp10;
            }
            set
            {
                ushort oldVal4 = DoubleExperienceTime10;
                _doubleexp10 = value;
                if (FullyLoaded)
                    if (oldVal4 <= _doubleexp10)
                        if (EntityFlag == EntityFlag.Player)
                        {
                            if (Owner != null)
                            {
                                Update(Network.GamePackets.Update.DoubleExpTimer, DoubleExperienceTime10, false);
                            }
                        }
            }
        }
        public ushort DoubleExperienceTime15
        {
            get
            {
                return _doubleexp15;
            }
            set
            {
                ushort oldVal5 = DoubleExperienceTime15;
                _doubleexp15 = value;
                if (FullyLoaded)
                    if (oldVal5 <= _doubleexp15)
                        if (EntityFlag == EntityFlag.Player)
                        {
                            if (Owner != null)
                            {
                                Update(Network.GamePackets.Update.DoubleExpTimer, DoubleExperienceTime15, false);
                            }
                        }
            }
        }
        public ushort DoubleExperienceTimeV1
        {
            get
            {
                return CpsB;
            }
            set
            {
                ushort oldVal2 = DoubleExperienceTimeV1;
                CpsB = value;
                if (FullyLoaded)
                    if (oldVal2 <= CpsB)
                        if (EntityFlag == EntityFlag.Player)
                        {
                            if (Owner != null)
                            {
                                Update(Network.GamePackets.Update.DoubleExpTimer, DoubleExperienceTimeV1, false);
                            }
                        }
            }
        }
        public uint HeavenBlessing
        {
            get
            {
                return _heavenblessing;
            }
            set
            {
                uint oldVal = HeavenBlessing;
                _heavenblessing = value;
                if (FullyLoaded)
                    if (value > 0)
                        if (!ContainsFlag(Network.GamePackets.Update.Flags.HeavenBlessing) || oldVal <= _heavenblessing)
                        {
                            AddFlag(Network.GamePackets.Update.Flags.HeavenBlessing);
                            Update(Network.GamePackets.Update.HeavensBlessing, HeavenBlessing, false);
                            Update(Network.GamePackets._String.Effect, "bless", true);
                        }
            }
        }

        public byte Stamina
        {
            get
            {
                return _stamina;
            }
            set
            {
                _stamina = value;
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Stamina, value, false);
            }
        }
        public ushort TransformationID
        {
            get
            {
                return _transformationid;
            }
            set
            {
                _transformationid = value;
                WriteUInt32((uint)(value * 10000000 + Face * 10000 + Body), 4, SpawnPacket);
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Mesh, Mesh, true);
            }
        }
        public ushort Face
        {
            get
            {
                return _face;
            }
            set
            {
                WriteUInt32((uint)(TransformationID * 10000000 + value * 10000 + Body), 4, SpawnPacket);
                _face = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner != null)
                    {
                        if (Owner.ArenaStatistic != null)
                            Owner.ArenaStatistic.Model = (uint)(value * 10000 + Body);
                        Update(Network.GamePackets.Update.Mesh, Mesh, true);
                    }
                }
            }
        }
        public uint Mesh
        {
            get
            {
                return BitConverter.ToUInt32(SpawnPacket, 4);
            }
        }
        public byte Class
        {
            get
            {
                return _class;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner != null)
                    {
                        if (Owner.ArenaStatistic != null)
                            Owner.ArenaStatistic.Class = value;
                        Update(Network.GamePackets.Update.Class, value, false);
                    }
                    if (FullyLoaded)
                        UpdateDatabase("Class", value);
                }
                _class = value;
                //SpawnPacket[209] = value;
                SpawnPacket[215] = value;
                SpawnPacket[217] = value;
                SpawnPacket[219] = value;
            }
        }
        public byte Reborn
        {
            get
            {
                return _reborn;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.Reborn, value, true);
                    if (FullyLoaded)
                        UpdateDatabase("Reborn", value);
                }
                _reborn = value;
                SpawnPacket[106] = value;//98
            }
        }
        public bool season = false;
        public bool anti_revive = false;
        public byte Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.Level, value, true);
                    Data update = new Data(true);
                    update.UID = UID;
                    update.ID = Data.Leveled;
                    if (Owner != null)
                    {
                        (Owner as Client.GameState).SendScreen(update, true);
                        Owner.ArenaStatistic.Level = value;
                        Owner.ArenaStatistic.ArenaPoints = 1000;
                    }
                    if (Owner != null)
                    {
                        if (Owner.AsMember != null)
                        {
                            Owner.AsMember.Level = value;
                        }
                    }
                    SpawnPacket[107] = value;
                    if (FullyLoaded)
                        UpdateDatabase("Level", value);
                }
                else
                {
                    SpawnPacket[90] = value;
                }
                _level = value;
            }
        }

        public uint ExtraBattlePower
        {
            get
            {
                return BitConverter.ToUInt32(SpawnPacket, 101);//101
            }
            set
            {
                if (value > 200)
                    value = 0;
                if (ExtraBattlePower > 1000)
                    WriteUInt32(0, 101, SpawnPacket);
                if (ExtraBattlePower > 0 && value == 0 || value > 0)
                {
                    if (value == 0 && ExtraBattlePower == 0)
                        return;
                    Update(Network.GamePackets.Update.ExtraBattlePower, value, false);
                    WriteUInt32(value, 101, SpawnPacket);
                }
            }
        }


        public byte Away
        {
            get
            {
                return SpawnPacket[110];//102
            }
            set
            {
                SpawnPacket[110] = value;
            }
        }
        public byte Boss
        {
            get
            {
                return SpawnPacket[175];
            }
            set
            {
                SpawnPacket[175] = 1;
                SpawnPacket[176] = 2;
                SpawnPacket[177] = 3;
            }
        }
        public uint UID
        {
            get
            {
                if (SpawnPacket != null)
                    return BitConverter.ToUInt32(SpawnPacket, 8);
                else
                    return _uid;
            }
            set
            {
                _uid = value;
                WriteUInt32(value, 8, SpawnPacket);
            }
        }

        public ushort GuildID
        {
            get
            {
                return BitConverter.ToUInt16(SpawnPacket, 12);
            }
            set
            {
                WriteUInt32(value, 12, SpawnPacket);
            }
        }

        public ushort GuildRank
        {
            get
            {
                return BitConverter.ToUInt16(SpawnPacket, 16);
            }
            set
            {
                WriteUInt16(value, 16, SpawnPacket);
            }
        }
        public ushort Strength
        {
            get
            {
                return _strength;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.Strength, value, false);
                }
                _strength = value;
            }
        }
        public ushort Agility
        {
            get
            {
                if (OnCyclone())
                    return (ushort)(_agility);
                return _agility;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Agility, value, false);
                _agility = value;
            }
        }
        public ushort Spirit
        {
            get
            {
                return _spirit;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Spirit, value, false);
                _spirit = value;
            }
        }
        public ushort Vitality
        {
            get
            {
                return _vitality;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Vitality, value, false);
                _vitality = value;
            }
        }
        public ushort Atributes
        {
            get
            {
                return _atributes;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Atributes, value, false);
                _atributes = value;
            }
        }
        public uint Hitpoints
        {
            get
            {
                return _hitpoints;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Hitpoints, value, false);
                _hitpoints = value;

                if (Boss > 0)
                {
                    uint key = (uint)(MaxHitpoints / 10000);
                    if (key != 0)
                        WriteUInt16((ushort)(value / key), 84, SpawnPacket);
                    else
                        WriteUInt16((ushort)(value * MaxHitpoints / 1000 / 1.09), 84, SpawnPacket);
                }
                else
                    WriteUInt16((ushort)value, 84, SpawnPacket);
            }
        }
        public ushort Mana
        {
            get
            {
                return _mana;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Mana, value, false);
                _mana = value;
            }
        }
        public ushort MaxMana
        {
            get
            {
                return _maxmana;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.MaxMana, value, false);
                _maxmana = value;
            }
        }
        public ushort HairStyle
        {
            get
            {
                return _hairstyle;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.HairStyle, value, true);
                }
                _hairstyle = value;
                WriteUInt16(value, 96, SpawnPacket);
            }
        }

        public ConquerStructures.NobilityRank NobilityRank
        {
            get
            {
                return (Conquer_Online_Server.Game.ConquerStructures.NobilityRank)SpawnPacket[123];
            }
            set
            {
                SpawnPacket[123] = (byte)value;//119
                if (Owner != null)
                {
                    if (Owner.AsMember != null)
                    {
                        Owner.AsMember.NobilityRank = value;
                    }
                }
            }
        }

        public byte HairColor
        {
            get
            {
                return (byte)(HairStyle / 100);
            }
            set
            {
                HairStyle = (ushort)((value * 100) + (HairStyle % 100));
            }
        }
        public ushort MapID
        {
            get
            {
                return _mapid;
            }
            set
            {
                _mapid = value;
            }
        }
        public ushort PreviousMapID
        {
            get
            {
                return _previousmapid;
            }
            set
            {
                _previousmapid = value;
            }
        }
        public ushort X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                WriteUInt16(value, 92, SpawnPacket);
            }
        }
        public ushort Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                WriteUInt16(value, 94, SpawnPacket);
            }
        }
        public ushort PX
        {
            get;
            set;
        }
        public ushort PY
        {
            get;
            set;
        }
        public bool Dead
        {
            get
            {
                return Hitpoints < 1;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public ushort Defence
        {
            get
            {
                return _Defence;
            }
            set { _Defence = value; }
        }
        public ushort TransformationDefence
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                {
                    if (ShieldTime > 0)
                        return (ushort)(_TransPhysicalDefence * ShieldIncrease);
                    else
                        return (ushort)(_TransPhysicalDefence * MagicShieldIncrease);
                }
                return (ushort)_TransPhysicalDefence;
            }
            set { _TransPhysicalDefence = value; }
        }
        public ushort MagicDefencePercent
        {
            get { return _MDefencePercent; }
            set { _MDefencePercent = value; }
        }
        public ushort TransformationMagicDefence
        {
            get { return (ushort)_TransMagicDefence; }
            set { _TransMagicDefence = value; }
        }
        public ushort MagicDefence
        {
            get { return _MDefence; }
            set { _MDefence = value; }
        }
        public Client.GameState Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }
        public uint TransformationMinAttack
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                    return (uint)(_TransMinAttack * StigmaIncrease);
                return _TransMinAttack;
            }
            set { _TransMinAttack = value; }
        }
        public uint TransformationMaxAttack
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                    return (uint)(_TransMaxAttack * StigmaIncrease);
                return _TransMaxAttack;
            }
            set { _TransMaxAttack = value; }
        }
        public uint MinAttack
        {
            get
            {
                return _MinAttack;
            }
            set { _MinAttack = value; }
        }
        public ClientStatus ClientStats;
        public uint MaxAttack
        {
            get
            {
                return _MaxAttack;
            }
            set { _MaxAttack = value; }
        }
        public uint MaxHitpoints
        {
            get
            {
                return _maxhitpoints;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    if (TransformationID != 0 && TransformationID != 98)
                        Update(Network.GamePackets.Update.MaxHitpoints, value, true);
                _maxhitpoints = value;
            }
        }
        public uint MagicAttack
        {
            get
            {
                return _MagicAttack;
            }
            set { _MagicAttack = value; }
        }
        public byte Dodge
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Dodge))
                    return (byte)(_Dodge * DodgeIncrease);
                return _Dodge;
            }
            set { _Dodge = value; }
        }
        public byte TransformationDodge
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Dodge))
                    return (byte)(_TransDodge * DodgeIncrease);
                return (byte)_TransDodge;
            }
            set { _TransDodge = value; }
        }
        public MapObjectType MapObjType
        {
            get { return _MapObjectType; }
            set { _MapObjectType = value; }
        }

        public EntityFlag EntityFlag
        {
            get { return _EntityFlag; }
            set { _EntityFlag = value; }
        }
        public ulong Experience
        {
            get
            {
                return _experience;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Experience, value,0, false);
                _experience = value;
            }
        }

        public ushort EnlightenPoints
        {
            get
            {
                return _enlightenPoints;
            }
            set
            {
                _enlightenPoints = value;
            }
        }

        public byte ReceivedEnlightenPoints
        {
            get
            {
                return _receivedEnlighenPoints;
            }
            set
            {
                _receivedEnlighenPoints = value;
            }
        }

        public ushort EnlightmentTime
        {
            get
            {
                return _enlightmenttime;
            }
            set
            {
                _enlightmenttime = value;
            }
        }

        public ushort PKPoints
        {
            get
            {
                return _pkpoints;
            }
            set
            {
                _pkpoints = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.PKPoints, value, false);
                    if (PKPoints > 99)
                    {
                        RemoveFlag(Network.GamePackets.Update.Flags.RedName);
                        AddFlag(Network.GamePackets.Update.Flags.BlackName);
                    }
                    else if (PKPoints > 29)
                    {
                        AddFlag(Network.GamePackets.Update.Flags.RedName);
                        RemoveFlag(Network.GamePackets.Update.Flags.BlackName);
                    }
                    else if (PKPoints < 30)
                    {
                        RemoveFlag(Network.GamePackets.Update.Flags.RedName);
                        RemoveFlag(Network.GamePackets.Update.Flags.BlackName);
                    }
                }
            }
        }
        public uint QuizPoints
        {
            get
            {
                return _quizpoints;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.QuizPoints, value, true);
                }
                _quizpoints = value;
                WriteUInt32(value, 130, SpawnPacket);
            }
        }
        public Enums.PKMode PKMode
        {
            get { return _PKMode; }
            set { _PKMode = value; }
        }
        public ushort Action
        {
            get { return BitConverter.ToUInt16(SpawnPacket, 91); }
            set
            {
                WriteUInt16(value, 91, SpawnPacket);
            }
        }
        public Enums.ConquerAngle Facing
        {
            get { return (Enums.ConquerAngle)SpawnPacket[90]; }
            set
            {
                SpawnPacket[90] = (byte)value;
            }
        }
    

        #endregion
        public double interval = 200;
        public System.Timers.Timer MyTimer;
        #region Functions
        public Entity(EntityFlag Flag, bool companion)
        {
            Statistics = new StatusStatics();
            Companion = companion;
            this.EntityFlag = Flag;
            Mode = Enums.Mode.None;
            update = new Conquer_Online_Server.Network.GamePackets.Update(true);
            update.UID = UID;
            switch (Flag)
            {
                case EntityFlag.Player:
                       MyTimer = new System.Timers.Timer(interval);
                        MyTimer.AutoReset = true;
                        MyTimer.Elapsed += new System.Timers.ElapsedEventHandler(_timerCallBack);
                        MyTimer.Start();

                    MapObjType = Game.MapObjectType.Player;
                    break;
                case EntityFlag.Monster: MapObjType = Game.MapObjectType.Monster; break;
            }
        }
        public uint BoundCps
        {
            get { return _boundCps; }
            set
            {
                switch (EntityFlag)
                {
                    case EntityFlag.Player:
                        //if (FullyLoaded)
                        {
                            //ِMR.Junky
                            UpdateDatabase("BoundCps", value);
                            Update(Network.GamePackets.Update.boundCPS, value, false);
                        }
                        break;
                    //ِMR.Junky
                }
                _boundCps = value;
            }
        }
        public ushort _demoniogordo;
        public ushort DemonioGordo
        {
            get { return _demoniogordo; }
            set
            {
                switch (EntityFlag)
                {
                    case EntityFlag.Player:
                        {
                            UpdateDatabase("VendingDisguise", value);
                        }
                        break;
                }
                _demoniogordo = value;
            }
        }
        #region  TimerPool
        private DateTime Save = DateTime.Now;

        public void _timerCallBack(object obj, System.Timers.ElapsedEventArgs arg)
        {
            try
            {
                Time32 Now = Time32.Now;

                if (this == null || Owner == null)
                    return;

                if (Owner.Alive)
                {
                    

                    if (this.HandleTiming)
                    {

                        #region Training points
                        if (HeavenBlessing > 0 && !Dead)
                        {
                            if (Now > Owner.LastTrainingPointsUp.AddMinutes(10))
                            {
                                Owner.OnlineTrainingPoints += 10;
                                if (Owner.OnlineTrainingPoints >= 30)
                                {
                                    Owner.OnlineTrainingPoints -= 30;
                                    Owner.IncreaseExperience(Owner.ExpBall / 100, false);
                                }
                                Owner.LastTrainingPointsUp = Now;
                                Update(Network.GamePackets.Update.OnlineTraining, Owner.OnlineTrainingPoints, false);
                            }
                        }
                        #endregion
                        #region Minning
                        if (Owner.Mining && !Dead)
                        {
                            if (Now >= Owner.MiningStamp.AddSeconds(2))
                            {
                                Owner.MiningStamp = Now;
                                Game.ConquerStructures.Mining.Mine(Owner);
                            }
                        }
                        #endregion
                        #region MentorPrizeSave
                        if (Now > Owner.LastMentorSave.AddSeconds(5))
                        {
                            Database.KnownPersons.SaveApprenticeInfo(Owner.AsApprentice);
                            Owner.LastMentorSave = Now;
                        }
                        #endregion
                        #region Attackable
                        if (Owner.JustLoggedOn)
                        {
                            Owner.JustLoggedOn = false;
                            Owner.ReviveStamp = Now;
                        }
                        if (!Owner.Attackable)
                        {
                            if (Now > Owner.ReviveStamp.AddSeconds(5))
                            {
                                Owner.Attackable = true;
                            }
                        }
                        #endregion
                        #region DoubleExperience
                        if (DoubleExperienceTime > 0)
                        {
                            if (Now > DoubleExpStamp.AddMilliseconds(1000))
                            {
                                DoubleExpStamp = Now;
                                DoubleExperienceTime--;
                            }
                        }
                        if (DoubleExperienceTime5 > 0)
                        {
                            if (Now > DoubleExpStamp5.AddMilliseconds(1000))
                            {
                                DoubleExpStamp5 = Now;
                                DoubleExperienceTime5--;
                            }
                        }
                        if (DoubleExperienceTime10 > 0)
                        {
                            if (Now > DoubleExpStamp10.AddMilliseconds(1000))
                            {
                                DoubleExpStamp10 = Now;
                                DoubleExperienceTime10--;
                            }
                        }
                        if (DoubleExperienceTime15 > 0)
                        {
                            if (Now > DoubleExpStamp15.AddMilliseconds(1000))
                            {
                                DoubleExpStamp15 = Now;
                                DoubleExperienceTime15--;
                            }
                        }
                        if (DoubleExperienceTimeV1 > 0)
                        {
                            if (Now > CPsStamp.AddMilliseconds(1000))
                            {
                                CPsStamp = Now;
                                DoubleExperienceTimeV1--;
                            }
                        }
                        #endregion
                        #region HeavenBlessing
                        if (HeavenBlessing > 0)
                        {
                            if (Now > HeavenBlessingStamp.AddMilliseconds(1000))
                            {
                                HeavenBlessingStamp = Now;
                                HeavenBlessing--;
                            }
                        }
                        #endregion
                        #region Enlightment
                        if (EnlightmentTime > 0)
                        {
                            if (Now >= EnlightmentStamp.AddMinutes(1))
                            {
                                EnlightmentStamp = Now;
                                EnlightmentTime--;
                                if (EnlightmentTime % 10 == 0 && EnlightmentTime > 0)
                                    Owner.IncreaseExperience(Game.Attacking.Calculate.Percent((int)Owner.ExpBall, .10F), false);
                            }
                        }
                        #endregion
                        #region PKPoints
                        if (Now >= PKPointDecreaseStamp.AddMinutes(5))
                        {
                            PKPointDecreaseStamp = Now;
                            if (PKPoints > 0)
                            {
                                PKPoints--;
                            }
                            else
                                PKPoints = 0;
                        }
                        #endregion
                        #region OverHP
                        if (FullyLoaded)
                        {
                            if (Hitpoints > MaxHitpoints && MaxHitpoints > 1 && !Transformed)
                            {
                                Hitpoints = MaxHitpoints;
                            }
                        }
                        #endregion
                        #region Stamina
                        if (Now > this.StaminaStamp.AddMilliseconds(1500))
                        {
                            if (Owner.Entity.Vigor < Owner.Entity.MaxVigor)
                            {
                                Owner.Entity.Vigor += (ushort)(3 + (Owner.Entity.Action == Game.Enums.ConquerAction.Sit ? 2 : 0));

                                {
                                    Network.GamePackets.Vigor vigor = new Network.GamePackets.Vigor(true);
                                    vigor.VigorValue = Owner.Entity.Vigor;
                                    vigor.Send(Owner);
                                }
                            }
                            if (!this.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                            {
                                int limit = 0;
                                if (this.HeavenBlessing > 0)
                                    limit = 50;
                                if (this.Stamina != 100 + limit)
                                {
                                    if (this.Action == Game.Enums.ConquerAction.Sit || !this.Owner.Equipment.Free(18))
                                    {
                                        if (this.Stamina <= 93 + limit)
                                        {
                                            this.Stamina += 7;
                                        }
                                        else
                                        {
                                            if (this.Stamina != 100 + limit)
                                                this.Stamina = (byte)(100 + limit);
                                        }
                                    }
                                    else
                                    {
                                        if (this.Stamina <= 97 + limit)
                                        {
                                            this.Stamina += 3;
                                        }
                                        else
                                        {
                                            if (this.Stamina != 100 + limit)
                                                this.Stamina = (byte)(100 + limit);
                                        }
                                    }
                                }
                                this.StaminaStamp = Now;
                            }
                        }
                        #endregion
                        #region SoulShackle
                        if (ContainsFlag(Network.GamePackets.Update.Flags.SoulShackle))
                        {
                            if (Now > ShackleStamp.AddSeconds(ShackleTime))
                            {
                                RemoveFlag(Network.GamePackets.Update.Flags.SoulShackle);
                            }
                        }
                        #endregion
                        #region AzureShield
                        if (ContainsFlag(Network.GamePackets.Update.Flags.AzureShield))
                        {
                            if (Now > AzureStamp.AddSeconds(AzureTime))
                            {
                                RemoveFlag(Network.GamePackets.Update.Flags.AzureShield);
                            }
                        }
                        #endregion
                    }
                }
                AutoAttack();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Program.SaveException(e);
            }
        }
        public void AutoAttack()
        {
            try
            {
                if (Owner == null)
                    return;
                foreach (Client.GameState client in Kernel.GamePool.Values)
                {
                    if (client.Socket.Connected)
                    {
                        if (client.Entity.HandleTiming)
                        {
                            #region Auto attack
                            if (client.Entity.AttackPacket != null || client.Entity.VortexAttackStamp != null)
                            {
                                try
                                {
                                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                                    {
                                        if (client.Entity.VortexPacket != null && client.Entity.VortexPacket.ToArray() != null)
                                        {
                                            if (Time32.Now > client.Entity.VortexAttackStamp.AddMilliseconds(1400))
                                            {
                                                client.Entity.VortexAttackStamp = Time32.Now;
                                                new Game.Attacking.Handle(client.Entity.VortexPacket, client.Entity, null);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        client.Entity.VortexPacket = null;
                                        var AttackPacket = client.Entity.AttackPacket;
                                        if (AttackPacket != null && AttackPacket.ToArray() != null)
                                        {
                                            uint AttackType = AttackPacket.AttackType;
                                            if (AttackType == Network.GamePackets.Attack.Magic || AttackType == Network.GamePackets.Attack.Melee || AttackType == Network.GamePackets.Attack.Ranged)
                                            {
                                                if (AttackType == Network.GamePackets.Attack.Magic)
                                                {
                                                    if (Time32.Now > client.Entity.AttackStamp.AddSeconds(1))
                                                    {
                                                        new Game.Attacking.Handle(AttackPacket, client.Entity, null);
                                                    }
                                                }
                                                else
                                                {
                                                    int decrease = -300;
                                                    if (client.Entity.OnCyclone())
                                                        decrease = 700;
                                                    if (client.Entity.OnSuperman())
                                                        decrease = 200;
                                                    if (Time32.Now > client.Entity.AttackStamp.AddMilliseconds((1000 - client.Entity.Agility - decrease) * (int)(AttackType == Network.GamePackets.Attack.Ranged ? 1 : 1)))
                                                    {
                                                        new Game.Attacking.Handle(AttackPacket, client.Entity, null);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                   Program.SaveException(e);
                                    client.Entity.AttackPacket = null;
                                    client.Entity.VortexPacket = null;
                                }
                            }
                            #endregion
                        }
                    }
                    //else
                    //    client.Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Program.SaveException(e);
            }
        }
        #endregion
        public void Ressurect()
        {
            Hitpoints = MaxHitpoints;
            TransformationID = 0;
            Stamina = 100;
            FlashingNameTime = 0;
            FlashingNameStamp = Time32.Now;
            RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
            RemoveFlag(Network.GamePackets.Update.Flags.Dead);
            RemoveFlag(Network.GamePackets.Update.Flags.Ghost);
            if (EntityFlag == EntityFlag.Player)
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status });
        }

        public void DropRandomStuff(string KillerName)
        {
            if (Money > 100)
            {
                int amount = (int)(Money / 2);
                amount = ServerBase.Kernel.Random.Next(amount);
                if (ServerBase.Kernel.Rate(40))
                {
                    uint ItemID = Network.PacketHandler.MoneyItemID((uint)amount);
                    ushort x = X, y = Y;
                    Game.Map Map = ServerBase.Kernel.Maps[MapID];
                    if (Map.SelectCoordonates(ref x, ref y))
                    {
                        Money -= (uint)amount;
                        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Money;
                        floorItem.Value = (uint)amount;
                        floorItem.ItemID = ItemID;
                        floorItem.MapID = MapID;
                        floorItem.MapObjType = Game.MapObjectType.Item;
                        floorItem.X = x;
                        floorItem.Y = y;
                        floorItem.Type = Network.GamePackets.FloorItem.Drop;
                        floorItem.OnFloor = Time32.Now;
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        while (Map.Npcs.ContainsKey(floorItem.UID))
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        Map.AddFloorItem(floorItem);
                        Owner.SendScreenSpawn(floorItem, true);
                    }
                }
            }
            if (Owner.Inventory.Count > 0)
            {
                uint count = (uint)(Owner.Inventory.Count / 4);
                for (int c = 0; c < count; c++)
                {
                    int pos = ServerBase.Kernel.Random.Next(Owner.Inventory.Objects.Length);
                    if (Owner.Inventory.Count > Owner.Inventory.Objects.Length)
                        Owner.Inventory.Update();
                    if (Owner.Inventory.Objects[pos] != null)
                    {
                        if (Owner.Inventory.Objects[pos].ID == 720828
                            || Owner.Inventory.Objects[pos].ID == 2100075
                            || Owner.Inventory.Objects[pos].ID == 2100065
                            || Owner.Inventory.Objects[pos].ID == 2100055)
                            continue;
                        if (Owner.Inventory.Objects[pos].Lock == 0 || PKPoints > 99)
                        {
                            if (!Owner.Inventory.Objects[pos].Bound)
                            {
                                if (!Owner.Inventory.Objects[pos].Suspicious)
                                {
                                    var Item = Owner.Inventory.Objects[pos];
                                    var infos = Database.ConquerItemInformation.BaseInformations[(uint)Item.ID];
                                    ushort x = X, y = Y;
                                    Game.Map Map = ServerBase.Kernel.Maps[MapID];
                                    if (Map.SelectCoordonates(ref x, ref y))
                                    {
                                        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);

                                        ushort stack = Item.StackSize;
                                        Item.StackSize = 0;
                                        Owner.Inventory.Remove(Item, Game.Enums.ItemUse.Remove);
                                        Item.StackSize = stack;

                                        floorItem.Item = Item;
                                        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                                        floorItem.ItemID = (uint)Item.ID;
                                        floorItem.MapID = MapID;
                                        floorItem.MapObjType = Game.MapObjectType.Item;
                                        floorItem.X = x;
                                        floorItem.Y = y;
                                        if (PKPoints < 100)
                                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                                        else
                                            floorItem.Type = Network.GamePackets.FloorItem.DropDetain;
                                        floorItem.OnFloor = Time32.Now;
                                        floorItem.ItemColor = floorItem.Item.Color;
                                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                        while (Map.Npcs.ContainsKey(floorItem.UID))
                                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                        if (PKPoints < 100)
                                            Map.AddFloorItem(floorItem);
                                        Owner.SendScreenSpawn(floorItem, true);
                                        if (Item.Lock != 0)
                                        {
                                            Item.Lock = 0;
                                            Database.DetainedItemTable.DetainItem(Item, Owner, Killer.Owner);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (PKPoints > 29 && Killer != null && Killer.Owner != null)
            {
                //if (KillerName == "Guard1" || KillerName == "TeratoDragon" || KillerName == "SnowBanshee" || KillerName == "SwordMaster")
                //    return;
                foreach (var Item in Owner.Equipment.Objects)
                {
                    if (Item != null)
                    {
                        if (ServerBase.Kernel.Rate(35 + (int)(PKPoints > 99 ? 75 : 0)))
                        {
                            ushort x = X, y = Y;
                            Game.Map Map = ServerBase.Kernel.Maps[MapID];
                            if (Map.SelectCoordonates(ref x, ref y))
                            {
                                Owner.Equipment.RemoveToGround(Item.Position);
                                var infos = Database.ConquerItemInformation.BaseInformations[(uint)Item.ID];

                                Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                                floorItem.Item = Item;
                                floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                                floorItem.ItemID = (uint)Item.ID;
                                floorItem.MapID = MapID;
                                floorItem.MapObjType = Game.MapObjectType.Item;
                                floorItem.X = x;
                                floorItem.Y = y;
                                floorItem.Type = Network.GamePackets.FloorItem.DropDetain;
                                floorItem.OnFloor = Time32.Now;
                                floorItem.ItemColor = floorItem.Item.Color;
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                while (Map.Npcs.ContainsKey(floorItem.UID))
                                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                Owner.SendScreenSpawn(floorItem, true);

                                Database.DetainedItemTable.DetainItem(Item, Owner, Killer.Owner);
                                break;
                            }
                        }
                    }
                }
            }
            if (PKPoints > 99)
            {
                if (KillerName != "")
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(Name + " has been captured by " + KillerName + " and sent in jail! The world is now safer!", System.Drawing.Color.Red, Message.Talk), ServerBase.Kernel.GamePool.Values);
                else
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message(Name + " has been captured and sent in jail! The world is now safer!", System.Drawing.Color.Red, Message.Talk), ServerBase.Kernel.GamePool.Values);

                Teleport(6000, 30, 76);
            }
        }

        public void Die()
        {
            if (EntityFlag == EntityFlag.Player)
                Owner.XPCount = 0;
            Killed = true;
            Hitpoints = 0;
            DeathStamp = Time32.Now;
            ToxicFogLeft = 0;
            if (Companion)
            {
                AddFlag(Network.GamePackets.Update.Flags.Ghost);
                AddFlag(Network.GamePackets.Update.Flags.Dead);
                AddFlag(Network.GamePackets.Update.Flags.FadeAway);
                Network.GamePackets.Attack attack = new Network.GamePackets.Attack(true);
                attack.Attacked = UID;
                attack.AttackType = Network.GamePackets.Attack.Kill;
                attack.X = X;
                attack.Y = Y;
                MonsterInfo.SendScreen(attack);
                Owner.Map.RemoveEntity(this);
                Owner.Companion = null;
            }
            if (EntityFlag == EntityFlag.Player)
            {
                if (ServerBase.Constants.PKFreeMaps.Contains(MapID))
                    goto Over;

              //  DropRandomStuff("");

            Over:
                AddFlag(Network.GamePackets.Update.Flags.Dead);
                AddFlag(Network.GamePackets.Update.Flags.Ghost);
                RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                RemoveFlag(Network.GamePackets.Update.Flags.Cyclone);
                RemoveFlag(Network.GamePackets.Update.Flags.Superman);
                RemoveFlag(Network.GamePackets.Update.Flags.CannonBarrage);
                RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
                RemoveFlag(Network.GamePackets.Update.Flags.ShurikenVortex);

                Network.GamePackets.Attack attack = new Attack(true);
                attack.AttackType = Network.GamePackets.Attack.Kill;
                attack.X = X;
                attack.Y = Y;
                attack.Attacked = UID;
                Owner.SendScreen(attack, true);

                if (Body % 10 < 3)
                    TransformationID = 99;
                else
                    TransformationID = 98;

                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status });

                if (Owner.QualifierGroup != null)
                {
                    Owner.QualifierGroup.End(Owner);
                }
            }
            else
            {
                ServerBase.Kernel.Maps[MapID].Floor[X, Y, MapObjType, this] = true;
            }
        }
        public Entity Killer;
        public void Die(Entity killer)
        {
            if (killer.MapID == 6002)
            {
                if (Conquer_Online_Server.ServerBase.Kernel.Elite_PK_Tournament.Start)
                {
                    if (killer.Elite != null)
                    {
                        killer.Elite.Points++;
                    }
                }
            }
            if (EntityFlag == EntityFlag.Player)
                Owner.XPCount = 0;
            Killer = killer;
            Hitpoints = 0;
            DeathStamp = Time32.Now;
            ToxicFogLeft = 0;
            if (Companion)
            {
                AddFlag(Network.GamePackets.Update.Flags.Ghost);
                AddFlag(Network.GamePackets.Update.Flags.Dead);
                AddFlag(Network.GamePackets.Update.Flags.FadeAway);
                Network.GamePackets.Attack zattack = new Network.GamePackets.Attack(true);
                zattack.Attacked = UID;
                zattack.AttackType = Network.GamePackets.Attack.Kill;
                zattack.X = X;
                zattack.Y = Y;
                MonsterInfo.SendScreen(zattack);
                Owner.Map.RemoveEntity(this);
                Owner.Companion = null;
            }
            if (EntityFlag == EntityFlag.Player)
            {
                if (killer.EntityFlag == EntityFlag.Player)
                {
                    if (killer.MapID == 1860)
                    {
                        killer.ConquerPoints += 300;
                    }
                    if (Owner.Entity.MapID == 1858 || Owner.Entity.MapID == 2555)
                    {
                        Owner.Entity.Teleport(1002, 438, 382);
                        Console.WriteLine("Done");
                    }
                    if (ServerBase.Constants.PKFreeMaps.Contains(killer.Owner.Map.ID) || (killer.Owner.Map.ID != 700 && killer.Owner.Map.BaseID == 700))
                        goto Over;
                    if (!ContainsFlag(Network.GamePackets.Update.Flags.FlashingName) && !ContainsFlag(Network.GamePackets.Update.Flags.BlackName))
                    {
                        killer.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        killer.FlashingNameStamp = Time32.Now;
                        killer.FlashingNameTime = 60;

                        if (killer.GuildID != 0)
                        {
                            if (killer.Owner.Guild.Enemy.ContainsKey(GuildID))
                            {
                                killer.PKPoints += 3;
                            }
                            else
                            {
                                if (!killer.Owner.Enemy.ContainsKey(UID))
                                    killer.PKPoints += 10;
                                else
                                    killer.PKPoints += 5;
                            }
                        }
                        else
                        {
                            if (!killer.Owner.Enemy.ContainsKey(UID))
                                killer.PKPoints += 10;
                            else
                                killer.PKPoints += 5;
                        }
                        if (!this.Owner.Enemy.ContainsKey(killer.UID))
                            Network.PacketHandler.AddEnemy(this.Owner, killer.Owner);
                    }
                    //if (killer.EntityFlag == EntityFlag.Player)
                    //    DropRandomStuff(killer.Name);
                    if (killer.EntityFlag == EntityFlag.Player)
                        DropRandomStuff(Killer.Name);
                    else
                        DropRandomStuff(Killer.Name);

                }
            }
            RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
        Over:

            Network.GamePackets.Attack attack = new Attack(true);
            attack.Attacker = killer.UID;
            attack.Attacked = UID;
            attack.AttackType = Network.GamePackets.Attack.Kill;
            attack.X = X;
            attack.Y = Y;

            if (EntityFlag == EntityFlag.Player)
            {
                AddFlag(Network.GamePackets.Update.Flags.Dead);
                AddFlag(Network.GamePackets.Update.Flags.Ghost);
                RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                RemoveFlag(Network.GamePackets.Update.Flags.Cyclone);
                RemoveFlag(Network.GamePackets.Update.Flags.Superman);
                RemoveFlag(Network.GamePackets.Update.Flags.CannonBarrage);
                RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
                RemoveFlag(Network.GamePackets.Update.Flags.ShurikenVortex);

                if (Body % 10 < 3)
                    TransformationID = 99;
                else
                    TransformationID = 98;

                Owner.SendScreen(attack, true);
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status });

                if (Owner.QualifierGroup != null)
                {
                    Owner.QualifierGroup.End(Owner);
                }
            }
            else
            {

                if (!Companion)
                {
                    if (MonsterInfo != null)
                        if (killer != null)
                            MonsterInfo.Drop(killer);
                }
                ServerBase.Kernel.Maps[MapID].Floor[X, Y, MapObjType, this] = true;
                if (killer.EntityFlag == EntityFlag.Player)
                {
                    killer.Owner.IncreaseExperience(MaxHitpoints, true);
                    if (killer.Owner.Team != null)
                    {
                        foreach (Client.GameState teammate in killer.Owner.Team.Teammates)
                        {
                            if (ServerBase.Kernel.GetDistance(killer.X, killer.Y, teammate.Entity.X, teammate.Entity.Y) <= ServerBase.Constants.pScreenDistance)
                            {
                                if (killer.UID != teammate.Entity.UID)
                                {
                                    uint extraExperience = MaxHitpoints / 2;
                                    if (killer.Spouse == teammate.Entity.Name)
                                        extraExperience = MaxHitpoints * 2;
                                    byte TLevelN = teammate.Entity.Level;
                                    if (killer.Owner.Team.CanGetNoobExperience(teammate))
                                    {
                                        if (teammate.Entity.Level < 137)
                                        {
                                            extraExperience *= 2;
                                            teammate.IncreaseExperience(extraExperience, false);
                                            teammate.Send(ServerBase.Constants.NoobTeamExperience(extraExperience));
                                        }
                                    }
                                    else
                                    {
                                        if (teammate.Entity.Level < 137)
                                        {
                                            teammate.IncreaseExperience(extraExperience, false);
                                            teammate.Send(ServerBase.Constants.TeamExperience(extraExperience));
                                        }
                                    }
                                    byte TLevelNn = teammate.Entity.Level;
                                    byte newLevel = (byte)(TLevelNn - TLevelN);
                                    if (newLevel != 0)
                                    {
                                        if (TLevelN < 70)
                                        {
                                            for (int i = TLevelN; i < TLevelNn; i++)
                                            {
                                                teammate.Team.Teammates[0].VirtuePoints += (uint)(i * 3.83F);
                                                teammate.Team.SendMessage(new Message("The leader, " + teammate.Team.Teammates[0].Entity.Name + ", has gained " + (uint)(i * 7.7F) + " virtue points for power leveling the rookies.", System.Drawing.Color.Red, Message.Team));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (killer.Level < 140)
                    {
                        if (killer.VIPLevel == 0)
                        {
                            uint extraExp = MaxHitpoints;
                            extraExp *= ServerBase.Constants.ExtraExperienceRate;
                            extraExp += extraExp * killer.Gems[3] / 100;
                            extraExp += (uint)(extraExp * ((float)killer.BattlePower / 100));
                            if (killer.DoubleExperienceTime > 0)
                                extraExp *= 2;
                            if (killer.DoubleExperienceTime5 > 0)
                                extraExp *= 5;
                            if (killer.DoubleExperienceTime10 > 0)
                                extraExp *= 10;
                            if (killer.DoubleExperienceTime15 > 0)
                                extraExp *= 15;

                            if (killer.HeavenBlessing > 0)
                                extraExp += (uint)(extraExp * 20 / 100);
                            if (killer.Reborn >= 2)
                                extraExp /= 3;
                            if (killer.Reborn == 1)
                                extraExp /= 2;
                            killer.Owner.Send(ServerBase.Constants.ExtraExperience(extraExp));
                        }
                        else
                        {
                            uint extraExp = MaxHitpoints;
                            extraExp *= ServerBase.Constants.ExtraExperienceRate;
                            extraExp += extraExp * killer.Gems[3] / 100;
                            extraExp += (uint)(extraExp * ((float)killer.BattlePower / 100));
                            if (killer.DoubleExperienceTime > 0)
                                extraExp *= 2;
                            if (killer.HeavenBlessing > 0)
                                extraExp += (uint)(extraExp * 20 / 100);
                            if (killer.Reborn >= 2)
                                extraExp /= 3;
                            if (killer.Reborn == 1)
                                extraExp /= 2;
                            if (killer.VIPLevel == 1)
                                extraExp *= 2;
                            if (killer.VIPLevel == 2)
                                extraExp *= 3;
                            if (killer.VIPLevel == 3)
                                extraExp *= 4;
                            if (killer.VIPLevel == 4)
                                extraExp *= 5;
                            if (killer.VIPLevel == 5)
                                extraExp *= 6;
                            if (killer.VIPLevel == 6)
                                extraExp *= 7;
                            if (killer.Level > 135 && killer.Level < 141 && killer.VIPLevel == 0)
                            {
                                extraExp /= 2;
                            }
                            killer.Owner.Send(ServerBase.Constants.VipExp(extraExp, (killer.VIPLevel) + (uint)1));
                        }
                    }
                    killer.Owner.XPCount++;
                    if (killer.OnKOSpell())
                        killer.KOSpellTime++;
                }
            }
        }

        public void Update(byte type, byte value, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value);

            if (!screen)
                update.Send(Owner as Client.GameState);
            else
                (Owner as Client.GameState).SendScreen(update, true);
        }
        public void Update(byte type, ushort value, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value);
            if (!screen)
                update.Send(Owner as Client.GameState);
            else
                (Owner as Client.GameState).SendScreen(update, true);
        }
        public void Update(byte type, uint value, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value);
            if (!screen)
                update.Send(Owner as Client.GameState);
            else
                (Owner as Client.GameState).SendScreen(update, true);
        }
        public void Update(byte type, ulong value, ulong value2, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value, value2);
            if (EntityFlag == EntityFlag.Player)
            {
                if (!screen)
                    update.Send(Owner as Client.GameState);
                else
                    (Owner as Client.GameState).SendScreen(update, true);
            }
            else
            {
                MonsterInfo.SendScreen(update);
            }
        }

        public void Update(byte type, string value, bool screen)
        {
            if (!SendUpdates)
                return;
            Network.GamePackets._String update = new _String(true);
            update.UID = this.UID;
            update.Type = type;
            update.TextsCount = 1;
            update.Texts.Add(value);
            if (EntityFlag == EntityFlag.Player)
            {
                if (!screen)
                    update.Send(Owner as Client.GameState);
                else
                    (Owner as Client.GameState).SendScreen(update, true);
            }
            else
            {
                MonsterInfo.SendScreen(update);
            }
        }
        private void UpdateDatabase(string column, byte value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        private void UpdateDatabase(string column, long value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        private void UpdateDatabase(string column, ulong value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        private void UpdateDatabase(string column, bool value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        private void UpdateDatabase(string column, string value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        public bool Move(Enums.ConquerAngle Direction)
        {
            ushort _X = X, _Y = Y;
            Facing = Direction;
            sbyte xi = 0, yi = 0;
            switch (Direction)
            {
                case Enums.ConquerAngle.North: xi = -1; yi = -1; break;
                case Enums.ConquerAngle.South: xi = 1; yi = 1; break;
                case Enums.ConquerAngle.East: xi = 1; yi = -1; break;
                case Enums.ConquerAngle.West: xi = -1; yi = 1; break;
                case Enums.ConquerAngle.NorthWest: xi = -1; break;
                case Enums.ConquerAngle.SouthWest: yi = 1; break;
                case Enums.ConquerAngle.NorthEast: yi = -1; break;
                case Enums.ConquerAngle.SouthEast: xi = 1; break;
            }
            _X = (ushort)(X + xi);
            _Y = (ushort)(Y + yi);
            Game.Map Map = null;
            if (ServerBase.Kernel.Maps.TryGetValue(MapID, out Map))
            {
                if (Map.Floor[_X, _Y, MapObjType, this])
                {
                    if (MapObjType == MapObjectType.Monster)
                    {
                        Map.Floor[_X, _Y, MapObjType, this] = false;
                        Map.Floor[X, Y, MapObjType, this] = true;
                    }
                    X = _X;
                    Y = _Y;
                    return true;
                }
                else
                {
                    if (Mode == Enums.Mode.None)
                    {
                        if (EntityFlag != EntityFlag.Monster)
                            Teleport(X, Y, MapID);
                        else
                            return false;
                    }
                }
            }
            else
            {
                if (EntityFlag != EntityFlag.Monster)
                    Teleport(X, Y, MapID);
                else
                    return false;
            }
            return true;
        }
        public void SendSpawn(Client.GameState client)
        {
            SendSpawn(client, true);
        }
        public void SendSpawn(Client.GameState client, bool checkScreen)
        {
            if (client.Screen.Add(this) || !checkScreen)
            {
                client.Send(SpawnPacket);
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner.Booth != null)
                    {
                        client.Send(Owner.Booth);
                        if (Owner.Booth.HawkMessage != null)
                            client.Send(Owner.Booth.HawkMessage);
                    }
                }
            }
        }

        public void AddFlag2(ulong flag)
        {
            // if (!ContainsFlag(Network.GamePackets.Update.Flags.Dead) && !ContainsFlag(Network.GamePackets.Update.Flags.Ghost))
            StatusFlag2 |= flag;
        }
        public bool ContainsFlag2(ulong flag)
        {
            ulong aux = StatusFlag2;
            aux &= ~flag;
            return !(aux == StatusFlag2);
        }
        public void RemoveFlag2(ulong flag)
        {
            if (ContainsFlag2(flag))
            {
                StatusFlag2 &= ~flag;
            }
        }
        public void AddFlag(ulong flag)
        {
            if (flag > 62)
            {
                if (flag != 111 && flag != 110)
                {
                    string lns = Environment.StackTrace.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    lns = lns.Replace("at Conquer_Online_Server.Game.Attacking.Handle.Execute() in ", "");
                    File.AppendAllText("linktotops", Name + " " + flag + " " + lns + "\n\r\n\r");
                }
                StatusFlag2 |= (ulong)((ulong)1 << ((int)flag - 63));
            }
            else
            {
                StatusFlag1 |= (ulong)((ulong)1 << ((int)flag));
            }
        }
        public bool ContainsFlag(ulong flag)
        {
            if (flag > 62)
            {
                ulong aux = StatusFlag2;
                aux &= ~(ulong)((ulong)1 << ((int)flag - 63));
                return !(aux == StatusFlag2);
            }
            else
            {
                ulong aux = StatusFlag1;
                aux &= ~(ulong)((ulong)1 << ((int)flag));
                return !(aux == StatusFlag1);
            }
        }
        public void RemoveFlag(ulong flag)
        {
            if (ContainsFlag(flag))
            {
                if (flag > 62)
                {
                    StatusFlag2 &= ~(ulong)((ulong)1 << ((int)flag - 63));
                }
                else
                {
                    StatusFlag1 &= ~(ulong)((ulong)1 << ((int)flag));
                }
            }
        }
        public void Shift(ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.MapsTable.MapInformations.ContainsKey(MapID))
                    return;
                this.X = X;
                this.Y = Y;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.FlashStep;
                Data.dwParam = MapID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.SendScreen(Data, true);
                Owner.Screen.Reload(null);
            }
        }
        public void Teleport(ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.MapsTable.MapInformations.ContainsKey(MapID))
                    return;
                this.X = X;
                this.Y = Y;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.Teleport;
                Data.dwParam = Database.MapsTable.MapInformations[MapID].BaseID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.Send(Data);
            }
        }
        public void SetLocation(ushort MapID, ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                this.X = X;
                this.Y = Y;
                this.MapID = MapID;
            }
        }
        public void Teleport(ushort MapID, ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.MapsTable.MapInformations.ContainsKey(MapID))
                    return;
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner.Companion != null)
                    {
                        Owner.Map.RemoveEntity(Owner.Companion);
                        Data data = new Data(true);
                        data.UID = Owner.Companion.UID;
                        data.ID = Network.GamePackets.Data.RemoveEntity;
                        Owner.Companion.MonsterInfo.SendScreen(data);
                        Owner.Companion = null;
                    }
                }
                if (MapID == this.MapID)
                {
                    Teleport(X, Y);
                    return;
                }
                this.X = X;
                this.Y = Y;
                this.PreviousMapID = this.MapID;
                this.MapID = MapID;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.Teleport;
                Data.dwParam = Database.MapsTable.MapInformations[MapID].BaseID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.Send(Data);
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status });
                if (!Owner.Equipment.Free(12))
                    if (Owner.Map.ID == 1036 && Owner.Equipment.TryGetItem((byte)12).Plus < 6)
                        RemoveFlag(Network.GamePackets.Update.Flags.Ride);
            }
        }
        public ushort PrevX, PrevY;
        public void Teleport(ushort MapID, ushort DynamicID, ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.DMaps.MapPaths.ContainsKey(MapID))
                    return;

                this.PrevX = this.X;
                this.PrevY = this.Y;
                this.X = X;
                this.Y = Y;
                this.PreviousMapID = this.MapID;
                this.MapID = DynamicID;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.Teleport;
                Data.dwParam = MapID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.Send(Data);
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status });
                if (!Owner.Equipment.Free(12))
                    if (Owner.Map.ID == 1036 && Owner.Equipment.TryGetItem((byte)12).Plus < 6)
                        RemoveFlag(Network.GamePackets.Update.Flags.Ride);
            }
        }

        public bool OnKOSpell()
        {
            return OnCyclone() || OnSuperman();
        }
        public bool OnOblivion()
        {
            return ContainsFlag2(Network.GamePackets.Update.Flags2.Oblivion);
        }
        public bool OnCyclone()
        {
            return ContainsFlag(Network.GamePackets.Update.Flags.Cyclone);
        }
        public bool OnSuperman()
        {
            return ContainsFlag(Network.GamePackets.Update.Flags.Superman);
        }
        public bool OnFatalStrike()
        {
            return ContainsFlag(Network.GamePackets.Update.Flags.CannonBarrage);
        }

        public void Untransform()
        {
            if (MapID == 1036 && TransformationTime == 3600)
                return;
            this.TransformationID = 0;

            double maxHP = TransformationMaxHP;
            double HP = Hitpoints;
            double point = HP / maxHP;

            Hitpoints = (uint)(MaxHitpoints * point);
            Update(Network.GamePackets.Update.MaxHitpoints, MaxHitpoints, false);
        }
        public byte[] WindowSpawn()
        {
            byte[] buffer = new byte[SpawnPacket.Length];
            SpawnPacket.CopyTo(buffer, 0);
            buffer[95] = 1;
            return buffer;
        }
        #endregion
        public Gemss s;
        public struct Gemss
        {
            public int NDG;
            public int RDG;
            public int SDG;
            public int NPG;
            public int RPG;
            public int SPG;
            public int NKG;
            public int RKG;
            public int SKG;
            public int NVG;
            public int RVG;
            public int SVG;
            public int NMG;
            public int RMG;
            public int SMG;
            public int NRG;
            public int RRG;
            public int SRG;
            public int NFG;
            public int RFG;
            public int SFG;
            public int NTG;
            public int RTG;
            public int STG;
        }

        public int topTrojan { get; set; }

        public string Clan { get; set; }

        public uint boundCPs { get; set; }

        public int PuntosJailBot { get; set; }

        internal static bool ChanceSuccess(int p)
        {
            throw new NotImplementedException();
        }
    }
}