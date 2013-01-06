using System;
using System.Drawing;
using Conquer_Online_Server.Game;
namespace Conquer_Online_Server.Network.GamePackets
{
    public class BoothItem : Writer, Interfaces.IPacket
    {
        byte[] Buffer;

        public BoothItem(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[92];
                WriteUInt16(84, 0, Buffer);
                WriteUInt16(1108, 2, Buffer);
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint BoothID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        public uint Cost
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }
        public uint ID
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public ushort Durability
        {
            get { return BitConverter.ToUInt16(Buffer, 20); }
            set { WriteUInt16(value, 20, Buffer); }
        }
        public ushort MaximDurability
        {
            get { return BitConverter.ToUInt16(Buffer, 22); }
            set { WriteUInt16(value, 22, Buffer); }
        }
        public uint CostType
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }
        public uint SocketProgress
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { WriteUInt32(value, 28, Buffer); }
        }
        public Enums.Gem SocketOne
        {
            get { return (Enums.Gem)Buffer[32]; }
            set { Buffer[32] = (byte)value; }
        }
        public Enums.Gem SocketTwo
        {
            get { return (Enums.Gem)Buffer[33]; }
            set { Buffer[33] = (byte)value; }
        }
        public Enums.ItemEffect Effect
        {
            get { return (Enums.ItemEffect)BitConverter.ToUInt16(Buffer, 34); }
            set { WriteUInt16((ushort)value, 34, Buffer); }
        }
        public byte Plus
        {
            get { return Buffer[41]; }
            set { Buffer[41] = value; }
        }
        public byte Bless
        {
            get { return Buffer[42]; }
            set { Buffer[42] = value; }
        }
        public bool Bound
        {
            get { return Buffer[43] == 0 ? false : true; }
            set { Buffer[43] = (byte)(value ? 1 : 0); }
        }
        public byte Enchant
        {
            get { return Buffer[44]; }
            set { Buffer[44] = value; }
        }
        public bool Suspicious
        {
            get { return Buffer[53] == 0 ? false : true; }
            set { Buffer[53] = (byte)(value ? 1 : 0); }
        }
        public byte Lock
        {
            get { return Buffer[54]; }
            set { Buffer[54] = value; }
        }
        public Enums.Color Color
        {
            get { return (Enums.Color)BitConverter.ToUInt32(Buffer, 56); }
            set { WriteUInt32((uint)value, 56, Buffer); }
        }
        public uint PlusProgress
        {
            get { return BitConverter.ToUInt32(Buffer, 60); }
            set { WriteUInt32(value, 60, Buffer); }
        }
        public ushort StackSize
        {
            get { return BitConverter.ToUInt16(Buffer, 72); }
            set { WriteUInt16(value, 72, Buffer); }
        }
        public uint PurificationID
        {
            get { return BitConverter.ToUInt32(Buffer, 76); }
            set { WriteUInt32(value, 76, Buffer); }
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

        public override int GetHashCode()
        {
            return (int)this.UID;
        }

        public void Fill(Game.ConquerStructures.BoothItem item, uint boothID)
        {
            UID = item.Item.UID;
            BoothID = boothID;
            Cost = item.Cost;
            ID = item.Item.ID;
            Durability = item.Item.Durability;
            MaximDurability = item.Item.MaximDurability;
            CostType = (byte)item.Cost_Type;
            SocketOne = item.Item.SocketOne;
            SocketTwo = item.Item.SocketTwo;
            Effect = item.Item.Effect;
            Bound = item.Item.Bound;
            Plus = item.Item.Plus;
            Bless = item.Item.Bless;
            Enchant = item.Item.Enchant;
            SocketProgress = item.Item.SocketProgress;
            Color = item.Item.Color;
            PlusProgress = item.Item.PlusProgress;
            StackSize = item.Item.StackSize;
            PurificationID = item.Item.Purification.PurificationItemID;
        }
        public void Fill(ConquerItem item, uint boothID)
        {
            UID = item.UID;
            BoothID = boothID;
            ID = item.ID;
            Durability = item.Durability;
            MaximDurability = item.MaximDurability;
            Buffer[24] = (byte)4;
            Buffer[26] = (byte)item.Position;
            SocketOne = item.SocketOne;
            SocketTwo = item.SocketTwo;
            Effect = item.Effect;
            Plus = item.Plus;
            Bound = item.Bound;
            Bless = item.Bless;
            Enchant = item.Enchant;
            SocketProgress = item.SocketProgress;
            Color = item.Color;
            PlusProgress = item.PlusProgress;
            StackSize = item.StackSize;
            PurificationID = item.Purification.PurificationItemID;
        }
    }
}
