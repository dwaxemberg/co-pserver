using System;
using System.Collections.Generic;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Update : Writer, Interfaces.IPacket
    {
        public struct UpdateStruct
        {
            public uint Type;
            public ulong Value;
        }
        public class Effects
        {
            public const ulong
                BlackBeard = 13,
                CannonBarrage = 16,
                EagleEye = 24,
                KrackenRevenge = 29,
                BombUsage = 61,
                GunReloading = 121;

        }
        public class Flags2
        {

            public const ulong
               WeeklyTop8Pk = 0x01,
            WeeklyTop2PkGold = 0x2,
            WeeklyTop2PkBlue = 0x4,
            MonthlyTop8Pk = 0x8,
            MontlyTop2Pk = 0x10,
            MontlyTop3Pk = 0x20,
            Top8Fire = 0x40,
            Top2Fire = 0x80,
            Top3Fire = 0x100,
            Top8Water = 0x200,
            Top2Water = 0x400,
            Top3Water = 0x800,
            Top8Ninja = 0x1000,
            Top2Ninja = 0x2000,
            Top3Ninja = 0x4000,
            Top8Warrior = 0x8000,
            Top2Warrior = 0x10000,
            Top3Warrior = 0x20000,
            Top8Trojan = 0x40000,
            Top2Trojan = 0x80000,
            Top3Trojan = 0x100000,
            Top8Archer = 0x200000,
            Top2Archer = 0x400000,
            Top3Archer = 0x800000,
            Top8Pirate = 0x9000000000000,
            Top2Pirate = 0x30000000000000,
            Top3Pirate = 0x40000000000000,
            Top3SpouseBlue = 0x1000000,
            Top2SpouseBlue = 0x2000000,
            Top3SpouseYellow = 0x4000000,
            Contestant = 0x8000000,
            ChainBoltActive = 0x10000000,
            AzureShield = 0x20000000,
            AzureShieldFade = 0x40000000,
            CaryingFlag = 2147483648,//blank next one?
            TyrantAura = 0x400000000,
            FendAura = 0x1000000000,
            MetalAura = 0x4000000000,
            WoodAura = 0x10000000000,
            WaterAura = 0x40000000000,
            FireAura = 17592186044416,
            EarthAura = 0x400000000000,
            Shackled = 140737488355328,
            Oblivion = 0x1000000000000,
            TopMonk = 1125899906842624,
            Top8Monk = 0x8000000000000,
            Top2Monk = 0x10000000000000,
            LionShield = 0x200000000000000,
            OrangeHaloGlow = 281474976710656,
            LowVigorUnableToJump = 1125899906842624,
            TopSpouse = 2251799813685248,
            SparkleHalo = 4503599627370496,
            PurpleSparkle = 9007199254740992,
            Dazed = 18014398509481984,//no movement
            BlueRestoreAura = 36028797018963968,
            MoveSpeedRecovered = 72057594037927936,
            SuperShieldHalo = 144115188075855872,
            HUGEDazed = 288230376151711744,//no movement
            IceBlock = 576460752303423488, //no movement
            Confused = 1152921504606846976,//reverses movement
            Top3Monk = 0x20000000000000;
  
            public static ulong SoulShackle { get; set; }
        }
        public class Flags
        {
            public const ulong
                Normal = 0x0,
                FlashingName = 0,
                Poisoned = 1,
                Invisible = 2,
                XPList = 4,
                Dead = 5,
                TeamLeader = 6,
                StarOfAccuracy = 7,
                MagicShield = 8,
                Stigma = 9,
                Ghost = 10,
                FadeAway = 11,
                RedName = 14,
                BlackName = 15,
                ReflectMelee = 17,
                Superman = 18,
                Ball = 19,
                Ball2 = 20,
                Invisibility = 22,
                Cyclone = 23,
                Dodge = 26,
                Fly = 27,
                Intensify = 28,
                CastPray = 30,
                Praying = 31,
                HeavenBlessing = 33,
                TopGuildLeader = 34,
                TopDeputyLeader = 35,
                MonthlyPKChampion = 36,
                WeeklyPKChampion = 37,
                TopWarrior = 38,
                TopTrojan = 39,
                TopArcher = 40,
                TopWaterTaoist = 41,
                TopFireTaoist = 42,
                TopNinja = 43,
                TopPirate = 44,
                ShurikenVortex = 46,
                CannonBarrage = 47,
                Flashy = 48,
                Ride = 50,
                AzureShield = 92,
                SoulShackle = 110,
                Oblivion = 111;
        }
        public const byte
                Hitpoints = 0,
                MaxHitpoints = 1,
                Mana = 2,
                MaxMana = 3,
                Money = 4,
                Experience = 5,
                PKPoints = 6,
                Class = 7,
                Stamina = 8,
                WHMoney = 9,
                Atributes = 10,
                Mesh = 11,
                Level = 12,
                Spirit = 13,
                Vitality = 14,
                Strength = 15,
                Agility = 16,
                HeavensBlessing = 17,
                DoubleExpTimer = 18,
                CursedTimer = 20,
                Reborn = 22,
                StatusFlag = 25,
                HairStyle = 26,
                XPCircle = 27,
                LuckyTimeTimer = 28,
                ConquerPoints = 29,
                OnlineTraining = 31,
                ExtraBattlePower = 36,
                Merchant = 38,
                VIPLevel = 39,
                QuizPoints = 40,
                EnlightPoints = 41;

        /*Packet Nr 367. Server -> Client, Length : 80, PacketType: 10017
48 00 21 27 80 D5 17 00 02 00 00 00 FF FF FF FF      ;H !'Õ    ÿÿÿÿ
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
08 00 00 00 5F 00 00 00 00 00 00 00 00 00 00 00      ;   _           
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 00 00 00 00 54 51 53 65 72 76 65 72      ;        TQServer


Packet Nr 38. Client -> Server, Length : 92, PacketType: 1009
54 00 F1 03 80 D5 17 00 00 00 00 00 1B 00 00 00      ;T ñÕ        
07 FA 23 00 00 00 00 00 00 00 00 00 00 00 00 00      ;ú#             
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 54 51 43 6C 69 65 6E 74                  ;    TQClient


Packet Nr 368. Server -> Client, Length : 92, PacketType: 1009
54 00 F1 03 80 D5 17 00 00 00 00 00 1B 00 00 00      ;T ñÕ        
07 FA 23 00 00 00 00 00 00 00 00 00 00 00 00 00      ;ú#             
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 54 51 53 65 72 76 65 72                  ;    TQServer


Packet Nr 369. Server -> Client, Length : 80, PacketType: 10017
48 00 21 27 80 D5 17 00 02 00 00 00 FF FF FF FF      ;H !'Õ    ÿÿÿÿ
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
08 00 00 00 62 00 00 00 00 00 00 00 00 00 00 00      ;   b           
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00      ;                
00 00 00 00 00 00 00 00 54 51 53 65 72 76 65 72      ;        TQServer*/
        byte[] Buffer;
        const byte minBufferSize = 24;
        public Update(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[minBufferSize + 8];
                WriteUInt16(minBufferSize, 0, Buffer);
                WriteUInt16(10017, 2, Buffer);
            }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint UpdateCount
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set
            {
                byte[] buffer = new byte[minBufferSize + 8 + 20 * value];
                Buffer.CopyTo(buffer, 0);
                WriteUInt16((ushort)(minBufferSize + 20 * value), 0, buffer);
                Buffer = buffer;
                WriteUInt32(value, 8, Buffer);
            }
        }

        public void Append(byte type, byte value)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(12 + (UpdateCount - 1) * 20);
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(value, offset + 4, Buffer);
        }
        public void Append(byte type, ushort value)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(12 + (UpdateCount - 1) * 20);
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(value, offset + 4, Buffer);
        }
        public void Append(byte type, uint value)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(12 + (UpdateCount - 1) * 20);
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(value, offset + 4, Buffer);
        }

        public void Append(byte type, ulong value, ulong value2)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(12 + (UpdateCount - 1) * 20);
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(value, offset + 4, Buffer);
            WriteUInt64(value2, offset + 12, Buffer);
        }
        public void Clear()
        {
            byte[] buffer = new byte[minBufferSize + 8];
            WriteUInt16(minBufferSize, 0, Buffer);
            WriteUInt16(10017, 2, Buffer);
            WriteUInt32(UID, 4, buffer);
            Buffer = buffer;
        }

        public List<UpdateStruct> Updates
        {
            get
            {
                List<UpdateStruct> structs = new List<UpdateStruct>();
                ushort offset = 12;
                if (UpdateCount > 0)
                {
                    for (int c = 0; c < UpdateCount; c++)
                    {
                        UpdateStruct st = new UpdateStruct();
                        st.Type = BitConverter.ToUInt32(Buffer, offset); offset += 4;
                        st.Value = BitConverter.ToUInt64(Buffer, offset); offset += 8;
                        ulong v2 = BitConverter.ToUInt64(Buffer, offset); offset += 8;
                        structs.Add(st);
                    }
                }
                return structs;
            }
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }

        public static byte boundCPS { get; set; }
    }
}
