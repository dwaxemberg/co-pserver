using System;
using System.Drawing;
using Conquer_Online_Server.Game;
using System.Collections.Generic;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ConquerItem : Writer, Interfaces.IPacket, Interfaces.IConquerItem
    {
        public const ushort
               Inventory = 0,
               Head = 1,
               Necklace = 2,
               Armor = 3,
               RightWeapon = 4,
               LeftWeapon = 5,
               Ring = 6,
               Bottle = 7,
               Boots = 8,
               Garment = 9,
               Fan = 10,
               Tower = 11,
               Steed = 12,
               RightWeaponAccessory = 15,
               LeftWeaponAccessory = 16,
               SteedMount = 17,
               RidingCrop = 18,
               Remove = 255;

        byte[] Buffer;

        public static ServerBase.Counter ItemUID = new ServerBase.Counter(0);

        private ulong suspiciousStart = 0, unlockEnd = 0;
        private bool unlocking = false;
        private ushort warehouse = 0;
        public DateTime RefineryStarted { get; set; }
        public Dictionary<uint, string> Agate_map { get; set; }
        public ConquerItem(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[92];
                WriteUInt16(84, 0, Buffer);
                WriteUInt16(1008, 2, Buffer);
                Agate_map = new Dictionary<uint, string>(10);
                Mode = Conquer_Online_Server.Game.Enums.ItemMode.Default;
                StatsLoaded = false;
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint ID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set
            {
                if (value == 0 && ID != 0)
                    throw new Exception("Invalid ID for an Item. Please check the stack trace to find the cause.");
                WriteUInt32(value, 8, Buffer);
            }
        }
        public void SetID(uint ID)
        {
            WriteUInt32(ID, 8, Buffer);
        }
        public ushort Durability
        {
            get { return BitConverter.ToUInt16(Buffer, 12); }
            set { WriteUInt16(value, 12, Buffer); }
        }
        public ushort MaximDurability
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { WriteUInt16(value, 14, Buffer); }
        }
        public Enums.ItemMode Mode
        {
            get { return (Enums.ItemMode)BitConverter.ToUInt16(Buffer, 16); }
            set { WriteUInt16((ushort)value, 16, Buffer); }
        }
        public ushort Position
        {
            get { return BitConverter.ToUInt16(Buffer, 18); }
            set { WriteUInt16(value, 18, Buffer); }
        }
        public ushort Warehouse
        {
            get { return warehouse; }
            set { warehouse = value; }
        }
        public uint SocketProgress
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }
        public Enums.Gem SocketOne
        {
            get { return (Enums.Gem)Buffer[24]; }
            set { Buffer[24] = (byte)value; }
        }
        public Enums.Gem SocketTwo
        {
            get { return (Enums.Gem)Buffer[25]; }
            set { Buffer[25] = (byte)value; }
        }
        public Enums.ItemEffect Effect
        {
            get { return (Enums.ItemEffect)BitConverter.ToUInt16(Buffer, 28); }
            set { WriteUInt16((ushort)value, 28, Buffer); }
        }
        public byte Plus
        {
            get { return Buffer[33]; }
            set { Buffer[33] = value; }
        }
        public byte Bless
        {
            get { return Buffer[34]; }
            set { Buffer[34] = value; }
        }
        public bool Bound
        {
            get { return Buffer[35] == 0 ? false : true; }
            set { Buffer[35] = (byte)(value ? 1 : 0); }
        }
        public byte Enchant
        {
            get { return Buffer[36]; }
            set { Buffer[36] = value; }
        }
        public bool Suspicious
        {
            get { return Buffer[45] == 0 ? false : true; }
            set { Buffer[45] = (byte)(value ? 1 : 0); }
        }
        public byte Lock
        {
            get { return Buffer[46]; }
            set { Buffer[46] = value; }
        }
        public Enums.Color Color
        {
            get { return (Enums.Color)BitConverter.ToUInt32(Buffer, 48); }
            set { WriteUInt32((uint)value, 48, Buffer); }
        }
        public uint PlusProgress
        {
            get { return BitConverter.ToUInt32(Buffer, 52); }
            set { WriteUInt32(value, 52, Buffer); }
        }
        public uint TimeLeftInMinutes
        {
            get { return BitConverter.ToUInt32(Buffer, 60); }
            set { WriteUInt32(value, 60, Buffer); }
        }
        public ushort StackSize
        {
            get { return BitConverter.ToUInt16(Buffer, 64); }
            set { WriteUInt16(value, 64, Buffer); }
        }
        public ushort MaxStackSize
        {
            get;
            set;
        }
        public DateTime SuspiciousStart
        {
            get { return DateTime.FromBinary((long)suspiciousStart); }
            set { suspiciousStart = (ulong)value.Ticks; }
        }
        public DateTime UnlockEnd
        {
            get { return DateTime.FromBinary((long)unlockEnd); }
            set { unlockEnd = (ulong)value.Ticks; }
        }
        public bool Inscribed
        {
            get { return BitConverter.ToUInt16(Buffer, 56) == 1; }
            set { WriteUInt16((byte)(value ? 1 : 0), 56, Buffer); }
        }

        public bool Unlocking
        {
            get { return unlocking; }
            set { unlocking = value; }
        }
        public bool MobDropped
        {
            get;
            set;
        }
        public bool StatsLoaded
        {
            get;
            set;
        }
        uint _refinery = 0;
        uint _refinerylevel = 0;
        ushort _refinerypercent = 0;

        public uint RefineryPart
        {
            get { return _refinery; }
            set { _refinery = value; }
        }
        public uint RefineryLevel
        {
            get { return _refinerylevel; }
            set { _refinerylevel = value; }
        }
        public ushort RefineryPercent
        {
            get { return _refinerypercent; }
            set { _refinerypercent = value; }
        }
        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return ID.ToString() + "#"
                + Durability.ToString() + "#"
                + MaximDurability.ToString() + "#"
                + Position.ToString() + "#"
                + SocketProgress.ToString() + "#"
                + ((byte)SocketOne).ToString() + "#"
                + ((byte)SocketTwo).ToString() + "#"
                + ((ushort)Effect).ToString() + "#"
                + Plus.ToString() + "#"
                + Bless.ToString() + "#"
                + (Bound ? "1" : "0") + "#"
                + Enchant.ToString() + "#"
                + (Suspicious ? "1" : "0") + "#"
                + Lock.ToString() + "#"
                + (Unlocking ? "1" : "0") + "#"
                + PlusProgress.ToString() + "#"
                + suspiciousStart.ToString() + "#"
                + unlockEnd.ToString();
        }
        public ItemAdding.Purification_ Purification
        {
            get;
            set;
        }
        public ItemAdding.Refinery_ ExtraEffect
        {
            get;
            set;
        }
        public void SendAgate(Client.GameState client)
        {
            byte[] packet = new byte[8 + 32 + 48 * Agate_map.Count + 48];
            WriteUInt16((ushort)(packet.Length - 8), 0, packet);
            WriteUInt16(2110, 2, packet);
            WriteUInt32(this.UID, 8, packet);
            WriteUInt32((byte)Agate_map.Count, 12, packet);
            WriteUInt32((byte)Agate_map.Count, 16, packet);
            WriteUInt32(Durability, 24, packet);
            WriteUInt32((byte)Agate_map.Count, 28, packet);
            if (Agate_map.Count > 0)
            {
                int position_next = 32;
                uint x = 0;
                for (; x < Agate_map.Count; x++)
                {
                    WriteUInt32(x, position_next, packet);
                    position_next += 4;
                    WriteUInt16(ushort.Parse(Agate_map[x].Split('~')[0].ToString()), position_next, packet);
                    position_next += 4;
                    WriteUInt16(ushort.Parse(Agate_map[x].Split('~')[1].ToString()), position_next, packet);
                    position_next += 4;
                    WriteUInt16(ushort.Parse(Agate_map[x].Split('~')[2].ToString()), position_next, packet);
                    position_next += 36;
                }
            }
            else
            {

            }
            client.Send(packet);
        }
        public void Send(Client.GameState client)
        {
            if (client == null)
                return;

            client.Send(Buffer);

            ItemAdding add = new ItemAdding(true);
            if (Purification.Available)
                add.Append(Purification);
            if (ExtraEffect.Available)
                add.Append(ExtraEffect);
            if (Purification.Available || ExtraEffect.Available)
                client.Send(add);
            #region refineryPart
            if (this.RefineryPart != 0)
            {
                Refinery refitem = new Refinery(true);
                refitem.ItemUID = this.UID;
                refitem.ID = this.RefineryPart;
                refitem.Level = this.RefineryLevel;
                refitem.Percent = this.RefineryPercent;
                refitem.Type = 1;
                refitem.Time = (UInt32)(RefineryStarted.Subtract(DateTime.Now).TotalSeconds);
                client.Send(refitem);
            }
            #endregion
            if (Lock == 2 && (Mode == Enums.ItemMode.Default || Mode == Enums.ItemMode.Update))
            {
                ItemLock itemLock = new ItemLock(true);
                itemLock.UID = UID;
                itemLock.ID = ItemLock.UnlockDate;
                itemLock.dwParam = (uint)(UnlockEnd.Year * 10000 + UnlockEnd.Month * 100 + UnlockEnd.Day);
                client.Send(itemLock);
            }
            Mode = Enums.ItemMode.Default;
        }
        public Enums.ItemQuality Quality
        {
            get
            {
                return (Enums.ItemQuality)(ID % 10);
            }
        }
        public ushort Vigor
        {
            get;
            set;
        }
        public ushort BattlePower
        {
            get
            {
                ushort t = 0;
                if (ID <= 0 || ID > 900999) goto Jump;

                t += Plus;

                if (Position != 12)
                {
                    if ((int)SocketOne > 0) t += 1;
                    if ((int)SocketTwo > 0) t += 1;
                    if (((int)SocketOne).ToString().EndsWith("3")) t += 1;
                    if (((int)SocketTwo).ToString().EndsWith("3")) t += 1;
                    if (((int)Quality - 5) > 0) t += (ushort)((int)Quality - 5);

                    if (ID.ToString().StartsWith("5") || ID.ToString().Remove(3) == "421")
                        t *= 2;
                }

            Jump:
                return t;
            }
        }
        public override int GetHashCode()
        {
            return (int)this.UID;
        }
        public override bool Equals(object obj)
        {
            return (obj as ConquerItem).UID == GetHashCode();
        }
    }
}
