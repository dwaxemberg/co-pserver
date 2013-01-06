using System;

namespace Conquer_Online_Server.Network.GamePackets
{   
    public class ItemUsage : Writer, Interfaces.IPacket
    {

        public const ushort
            BuyFromNPC = 1,
            SellToNPC = 2,
            RemoveInventory = 3,
            EquipItem = 4,
            UnequipItem = 6,
            ArrowReload = 8,
            ViewWarehouse = 9,
            WarehouseDeposit = 10,
            WarehouseWithdraw = 11,
            Repair = 14,
            DragonBallUpgrade = 19,
            MeteorUpgrade = 20,
            ShowBoothItems = 21,
            AddItemOnBoothForSilvers = 22,
            RemoveItemFromBooth = 23,
            BuyFromBooth = 24,
            UpdateDurability = 25,
            AddItemOnBoothForConquerPoints = 29,
            Ping = 27,
            Enchant = 28,
            RedeemGear = 32,
            ClaimGear = 33,
            SocketTalismanWithItem = 35,
            SocketTalismanWithCPs = 36,
            DropItem = 37,
            DropMoney = 38,
            SocketerMan = 43,
            NextWepone = 45,
            Accessories = 41,
            ToristSuper = 51,
           
            MergeStackableItems = 48,
            SplitStack = 49;
            

        byte[] Buffer;
        public ItemUsage(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[84];
                WriteUInt16(76, 0, Buffer);
                WriteUInt16(1009, 2, Buffer);
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint ID
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint TimeStamp
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public uint dwExtraInfo
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }
        public uint dwExtraInfo2
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }
        public uint dwExtraInfo3
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { WriteUInt32(value, 28, Buffer); }
        }

        //these are used for showing equipped gear. Simple send the item uids
        public uint Pos1
        {
            get { return BitConverter.ToUInt32(Buffer, 32); }
            set{WriteUInt32(value, 32, Buffer);}
        }
        public uint Pos2
        {
            get { return BitConverter.ToUInt32(Buffer, 36); }
            set { WriteUInt32(value, 36, Buffer); }
        }
        public uint Pos3
        {
            get { return BitConverter.ToUInt32(Buffer, 40); }
            set { WriteUInt32(value, 40, Buffer); }
        }
        public uint Pos4
        {
            get { return BitConverter.ToUInt32(Buffer, 44); }
            set { WriteUInt32(value, 44, Buffer); }
        }
        public uint Pos5
        {
            get { return BitConverter.ToUInt32(Buffer, 48); }
            set { WriteUInt32(value, 48, Buffer); }
        }
        public uint Pos6
        {
            get { return BitConverter.ToUInt32(Buffer, 52); }
            set { WriteUInt32(value, 52, Buffer); }
        }
        public uint Pos7
        {
            get { return BitConverter.ToUInt32(Buffer, 56); }
            set { WriteUInt32(value, 56, Buffer); }
        }
        public uint Pos8
        {
            get { return BitConverter.ToUInt32(Buffer, 60); }
            set { WriteUInt32(value, 60, Buffer); }
        }
        public uint Pos9
        {
            get { return BitConverter.ToUInt32(Buffer, 64); }
            set { WriteUInt32(value, 64, Buffer); }
        }
        public uint Pos10
        {
            get { return BitConverter.ToUInt32(Buffer, 68); }
            set { WriteUInt32(value, 68, Buffer); }
        }
        public uint Pos11
        {
            get { return BitConverter.ToUInt32(Buffer, 72); }
            set { WriteUInt32(value, 72, Buffer); }
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
    }
}
