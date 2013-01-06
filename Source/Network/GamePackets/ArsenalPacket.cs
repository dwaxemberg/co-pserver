using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ArsenalPacket : Writer, Interfaces.IPacket
    {
        byte[] Buffer;

        public ArsenalPacket(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[244 + 8];
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(2201, 2, Buffer);
            }
        }
        public ushort Type
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }
        public ushort Headgear
        {
            get { return BitConverter.ToUInt16(Buffer, 20); }
            set { WriteUInt16(value, 20, Buffer); }
        }
        public byte Headgear_Potency
        {
            get { return Buffer[28]; }
            set { Buffer[28] = value; }
        }
        public ulong Headgear_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 36); }
            set { WriteUInt64(value, 36, Buffer); }
        }
        public bool Headgear_Avaliable
        {
            get { return Buffer[44] == 1; }
            set { Buffer[44] = (byte)(value ? 1 : 0); }
        }
        public ushort Armor
        {
            get { return BitConverter.ToUInt16(Buffer, 48); }
            set { WriteUInt16(value, 48, Buffer); }
        }
        public byte Armor_Potency
        {
            get { return Buffer[52]; }
            set { Buffer[52] = value; }
        }
        public ulong Armor_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 60); }
            set { WriteUInt64(value, 60, Buffer); }
        }
        public bool Armor_Avaliable
        {
            get { return Buffer[68] == 1; }
            set { Buffer[68] = (byte)(value ? 1 : 0); }
        }
        public ushort Weapon
        {
            get { return BitConverter.ToUInt16(Buffer, 72); }
            set { WriteUInt16(value, 72, Buffer); }
        }
        public byte Weapon_Potency
        {
            get { return Buffer[76]; }
            set { Buffer[76] = value; }
        }
        public ulong Weapon_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 84); }
            set { WriteUInt64(value, 84, Buffer); }
        }
        public bool Weapon_Avaliable
        {
            get { return Buffer[92] == 1; }
            set { Buffer[92] = (byte)(value ? 1 : 0); }
        }
        public ushort Ring
        {
            get { return BitConverter.ToUInt16(Buffer, 96); }
            set { WriteUInt16(value, 96, Buffer); }
        }
        public byte Ring_Potency
        {
            get { return Buffer[102]; }
            set { Buffer[102] = value; }
        }
        public ulong Ring_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 108); }
            set { WriteUInt64(value, 108, Buffer); }
        }
        public bool Ring_Avaliable
        {
            get { return Buffer[116] == 1; }
            set { Buffer[116] = (byte)(value ? 1 : 0); }
        }
        public ushort Boots
        {
            get { return BitConverter.ToUInt16(Buffer, 120); }
            set { WriteUInt16(value, 120, Buffer); }
        }
        public byte Boots_Potency
        {
            get { return Buffer[124]; }
            set { Buffer[124] = value; }
        }
        public ulong Boots_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 132); }
            set { WriteUInt64(value, 132, Buffer); }
        }
        public bool Boots_Avaliable
        {
            get { return Buffer[140] == 1; }
            set { Buffer[140] = (byte)(value ? 1 : 0); }
        }
        public ushort Necklace
        {
            get { return BitConverter.ToUInt16(Buffer, 144); }
            set { WriteUInt16(value, 144, Buffer); }
        }
        public byte Necklace_Potency
        {
            get { return Buffer[148]; }
            set { Buffer[148] = value; }
        }
        public ulong Necklace_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 156); }
            set { WriteUInt64(value, 156, Buffer); }
        }
        public bool Necklace_Avaliable
        {
            get { return Buffer[164] == 1; }
            set { Buffer[164] = (byte)(value ? 1 : 0); }
        }
        public ushort Fan
        {
            get { return BitConverter.ToUInt16(Buffer, 168); }
            set { WriteUInt16(value, 168, Buffer); }
        }
        public byte Fan_Potency
        {
            get { return Buffer[172]; }
            set { Buffer[172] = value; }
        }
        public ulong Fan_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 180); }
            set { WriteUInt64(value, 180, Buffer); }
        }
        public bool Fan_Avaliable
        {
            get { return Buffer[188] == 1; }
            set { Buffer[188] = (byte)(value ? 1 : 0); }
        }
        public ushort Tower
        {
            get { return BitConverter.ToUInt16(Buffer, 192); }
            set { WriteUInt16(value, 192, Buffer); }
        }
        public byte Tower_Potency
        {
            get { return Buffer[196]; }
            set { Buffer[196] = value; }
        }
        public ulong Tower_Donation
        {
            get { return BitConverter.ToUInt64(Buffer, 204); }
            set { WriteUInt64(value, 204, Buffer); }
        }
        public bool Tower_Avaliable
        {
            get { return Buffer[212] == 1; }
            set { Buffer[212] = (byte)(value ? 1 : 0); }
        }

        public void Start()
        {
            this.Armor = 1;
            this.Weapon = 2;
            this.Ring = 3;
            this.Boots = 4;
            this.Necklace = 5;
            this.Fan = 6;
            this.Tower = 7;
            this.Headgear = 8;
        }
        byte TotalBP = 0;
        public byte MaximumBP { get { return TotalBP; } }
        public void SetTotals()
        {
            TotalBP = (byte)(Headgear_Potency + Armor_Potency + Weapon_Potency + Ring_Potency + Boots_Potency + Necklace_Potency + Fan_Potency + Tower_Potency);
            if (TotalBP > 15) TotalBP = 15;
            WriteByte(TotalBP, 8, Buffer);
        }
        public void SetTotals2()
        {
            TotalBP = (byte)(Headgear_Potency + Armor_Potency + Weapon_Potency + Ring_Potency + Boots_Potency + Necklace_Potency + Fan_Potency + Tower_Potency);
            if (TotalBP > 15) TotalBP = 90;
            WriteByte(TotalBP, 8, Buffer);
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.GameState client)
        {
            client.Send(Buffer);
        }
    }
}
