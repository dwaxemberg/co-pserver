using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class EntityEquipment : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;
        public EntityEquipment(bool Create)
        {
            Buffer = new byte[108];
            if (Create)
            {
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(1009, 2, Buffer);
                WriteUInt16(0x2E, 12, Buffer);
            }
        }

        public void Deserialize(byte[] data)
        {
            Buffer = data;
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }

        public void ParseHero(Client.GameState client)
        {

            if (client.Equipment == null)
                return;
            WriteUInt32(client.Entity.UID, 4, Buffer);
            WriteUInt32(client.Entity.UID, 8, Buffer);
            foreach (var Item in client.Equipment.Objects)
            {
                if (Item == null)
                    continue;
                switch (Item.Position)
                {

                    case ConquerItem.Head: this.Helm = Item.UID; break;
                    case ConquerItem.Necklace: this.Necklace = Item.UID; break;
                    case ConquerItem.Armor:
                        this.Armor = Item.UID;
                        break;
                    case ConquerItem.RightWeapon: this.RHand = Item.UID; break;
                    case ConquerItem.LeftWeapon: this.LHand = Item.UID; break;
                    case ConquerItem.Ring: this.Ring = Item.UID; break;
                    case ConquerItem.Boots: this.Boots = Item.UID; break;
                    case ConquerItem.Garment:
                        this.Garment = Item.UID;
                        break;
                    case ConquerItem.Bottle:
                        this.Talisman = Item.UID;
                        break;
                    case ConquerItem.RightWeaponAccessory:
                        this.AccessoryOne = Item.UID;
                        break;
                    case ConquerItem.LeftWeaponAccessory: 
                        this.AccessoryTwo = Item.UID; 
                        break;
                    case ConquerItem.SteedMount:
                        {
                            this.SteedMount = Item.UID; 
                           // WriteUInt32(Item.UID, 76, Buffer);
                            break;
                        }
                    case ConquerItem.RidingCrop:
                        {
                            this.RidingCrop = Item.UID; 
                            //WriteUInt32(Item.UID, 80, Buffer);
                            break;
                        }
                }
            }
        }

        public uint Helm
        {
            get { return BitConverter.ToUInt32(Buffer, 32); }
            set { WriteUInt32(value, 32, Buffer); }
        }

        public uint Necklace
        {
            get { return BitConverter.ToUInt32(Buffer, 36); }
            set { WriteUInt32(value, 36, Buffer); }
        }

        public uint Armor
        {
            get { return BitConverter.ToUInt32(Buffer, 40); }
            set { WriteUInt32(value, 40, Buffer); }
        }

        public uint RHand
        {
            get { return BitConverter.ToUInt32(Buffer, 44); }
            set { WriteUInt32(value, 44, Buffer); }
        }

        public uint LHand
        {
            get { return BitConverter.ToUInt32(Buffer, 48); }
            set { WriteUInt32(value, 48, Buffer); }
        }

        public uint Ring
        {
            get { return BitConverter.ToUInt32(Buffer, 52); }
            set { WriteUInt32(value, 52, Buffer); }
        }

        public uint Talisman
        {
            get { return BitConverter.ToUInt32(Buffer, 56); }
            set { WriteUInt32(value, 56, Buffer); }
        }

        public uint Boots
        {
            get { return BitConverter.ToUInt32(Buffer, 60); }
            set { WriteUInt32(value, 60, Buffer); }
        }

        public uint Garment
        {
            get { return BitConverter.ToUInt32(Buffer, 64); }
            set { WriteUInt32(value, 64, Buffer); }
        }

        public uint AccessoryOne
        {
            get { return BitConverter.ToUInt32(Buffer, 68); }
            set { WriteUInt32(value, 68, Buffer); }
        }

        public uint AccessoryTwo
        {
            get { return BitConverter.ToUInt32(Buffer, 72); }
            set { WriteUInt32(value, 72, Buffer); }
        }
        public uint SteedMount
        {
            get { return BitConverter.ToUInt32(Buffer, 76); }
            set { WriteUInt32(value, 76, Buffer); }
        }
        public uint RidingCrop
        {
            get { return BitConverter.ToUInt32(Buffer, 80); }
            set { WriteUInt32(value, 80, Buffer); }
        }
    }
}
