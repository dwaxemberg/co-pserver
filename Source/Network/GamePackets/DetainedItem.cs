using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Game;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class DetainedItem : Writer, Interfaces.IPacket
    {
        public const ushort DetainPage = 0, ClaimPage = 1;
        byte[] Buffer;
        private Interfaces.IConquerItem item;

        public DetainedItem(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[112 + 8];
                WriteUInt16(112, 0, Buffer);
                WriteUInt16(1034, 2, Buffer);
            }
        }


        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint ItemUID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint ItemID
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public ushort Durability
        {
            get { return BitConverter.ToUInt16(Buffer, 16); }
            set { WriteUInt16(value, 16, Buffer); }
        }

        public ushort MaximDurability
        {
            get { return BitConverter.ToUInt16(Buffer, 18); }
            set { WriteUInt16(value, 18, Buffer); }
        }

        public byte Page
        {
            get { return Buffer[20]; }
            set { Buffer[20] = value; }
        }

        public uint SocketProgress
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }

        public Enums.Gem SocketOne
        {
            get { return (Enums.Gem)Buffer[28]; }
            set { Buffer[28] = (byte)value; }
        }
        public Enums.Gem SocketTwo
        {
            get { return (Enums.Gem)Buffer[29]; }
            set { Buffer[29] = (byte)value; }
        }

        public Enums.ItemEffect Effect
        {
            get { return (Enums.ItemEffect)BitConverter.ToUInt16(Buffer, 30); }
            set { WriteUInt16((ushort)value, 30, Buffer); }
        }

        public byte Plus
        {
            get { return Buffer[32]; }
            set { Buffer[32] = value; }
        }
        public byte Bless
        {
            get { return Buffer[33]; }
            set { Buffer[33] = value; }
        }
        public bool Bound
        {
            get { return Buffer[34] == 0 ? false : true; }
            set { Buffer[34] = (byte)(value ? 1 : 0); }
        }
        public byte Enchant
        {
            get { return Buffer[35]; }
            set { Buffer[35] = value; }
        }

        public bool Suspicious
        {
            get { return Buffer[40] == 0 ? false : true; }
            set { Buffer[40] = (byte)(value ? 1 : 0); }
        }

        public byte Lock
        {
            get { return Buffer[42]; }
            set { Buffer[42] = value; }
        }

        public uint ItemColor
        {
            get { return BitConverter.ToUInt32(Buffer, 44); }
            set { WriteUInt32(value, 44, Buffer); }
        }

        public uint OwnerUID
        {
            get { return BitConverter.ToUInt32(Buffer, 48); }
            set { WriteUInt32(value, 48, Buffer); }
        }

        public string OwnerName
        {
            get
            {
                return Encoding.ASCII.GetString(Buffer, 52, 16).Split('\0')[0];
            }
            set
            {
                if(value.Length > 16)
                    value = value.Remove(16);
                for (int count = 0; count < value.Length; count++)
                    Buffer[52 + count] = (byte)(value[count]);
            }
        }

        public uint GainerUID
        {
            get { return BitConverter.ToUInt32(Buffer, 68); }
            set { WriteUInt32(value, 68, Buffer); }
        }

        public string GainerName
        {
            get
            {
                return Encoding.ASCII.GetString(Buffer, 72, 16).Split('\0')[0];
            }
            set
            {
                if (value.Length > 16)
                    value = value.Remove(16);
                for (int count = 0; count < value.Length; count++)
                    Buffer[72 + count] = (byte)(value[count]);
            }
        }
        /// <summary>
        /// YYYYMMDD
        /// </summary>
        public uint Date2
        {
            get { return BitConverter.ToUInt32(Buffer, 88); }
            set { WriteUInt32(value, 88, Buffer); }
        }

        public uint ConquerPointsCost
        {
            get { return BitConverter.ToUInt32(Buffer, 104); }
            set { WriteUInt32(value, 104, Buffer); }
        }
        /// <summary>
        /// The value set is not the value shown by the client. The client shows 7 - value as days left until pickup.
        /// </summary>
        public uint DaysLeft
        {
            get { return (BitConverter.ToUInt32(Buffer, 108)); }
            set { WriteUInt32(value, 108, Buffer); }
        }

        public DateTime Date
        {
            get;
            set;
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }

        public Interfaces.IConquerItem Item
        {
            get { return item; }
            set 
            {
                item = value;
                ItemUID = item.UID;
                ItemID = item.ID;
                ItemColor = (uint)item.Color;
                Durability = item.Durability;
                MaximDurability = item.MaximDurability;
                SocketOne = item.SocketOne;
                SocketTwo = item.SocketTwo;
                Effect = item.Effect;
                Plus = item.Plus;
                Bless = item.Bless;
                Enchant = item.Enchant;
                SocketProgress = item.SocketProgress;
                Bound = item.Bound;
                Lock = item.Lock;
            }
        }
        public void MakeItReadyToClaim()
        {
            ItemID = 0;
            ItemColor = 0;
            Durability = 0;
            MaximDurability = 0;
            SocketOne = Enums.Gem.NoSocket;
            SocketTwo = Enums.Gem.NoSocket;
            Effect = Enums.ItemEffect.None;
            Plus = 0;
            Bless = 0;
            Enchant = 0;
            SocketProgress = 0;
            Bound = false;
            Lock = 0;
            WriteUInt32(ConquerPointsCost, 100, Buffer);
            Page = 2;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
