using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.ConquerStructures.Society
{
    public class ArsenalSingle
    {
        public uint D_UID;
        public uint UID;
        public string Name;
        public Interfaces.IConquerItem Item;
        public uint Donation;
        public ArsenalType Type;
    }
    public enum ArsenalType : int
    {
        Headgear = 0,
        Armor = 1,
        Weapon = 2,
        Ring = 3,
        Boots = 4,
        Necklace = 5,
        Fan = 6,
        Tower = 7,
    }
    public class Arsenals
    {
        public SafeDictionary<ArsenalType, SafeDictionary<uint, ArsenalSingle>> Inscribed;
        public Dictionary<ArsenalType, ulong> Donation;
        public Arsenals()
        {
            Inscribed = new SafeDictionary<ArsenalType, SafeDictionary<uint, ArsenalSingle>>(1000);
            Donation = new Dictionary<ArsenalType, ulong>(1000);
            Inscribed.Add(ArsenalType.Headgear, new SafeDictionary<uint, ArsenalSingle>(1000));
            Inscribed.Add(ArsenalType.Armor, new SafeDictionary<uint, ArsenalSingle>(1000));
            Inscribed.Add(ArsenalType.Weapon, new SafeDictionary<uint, ArsenalSingle>(1000));
            Inscribed.Add(ArsenalType.Ring, new SafeDictionary<uint, ArsenalSingle>(1000));
            Inscribed.Add(ArsenalType.Boots, new SafeDictionary<uint, ArsenalSingle>(1000));
            Inscribed.Add(ArsenalType.Necklace, new SafeDictionary<uint, ArsenalSingle>(1000));
            Inscribed.Add(ArsenalType.Fan, new SafeDictionary<uint, ArsenalSingle>(1000));
            Inscribed.Add(ArsenalType.Tower, new SafeDictionary<uint, ArsenalSingle>(1000));
            Donation.Add(ArsenalType.Headgear, 0);
            Donation.Add(ArsenalType.Armor, 0);
            Donation.Add(ArsenalType.Weapon, 0);
            Donation.Add(ArsenalType.Ring, 0);
            Donation.Add(ArsenalType.Boots, 0);
            Donation.Add(ArsenalType.Necklace, 0);
            Donation.Add(ArsenalType.Fan, 0);
            Donation.Add(ArsenalType.Tower, 0);
        }

        public void Inscribe(ArsenalType Type, Interfaces.IConquerItem Item, Client.GameState client)
        {
            if (!Inscribed[Type].ContainsKey(Item.UID))
            {
                Item.Mode = Enums.ItemMode.Update;
                Item.Inscribed = true;
                Item.Send(client);
                Item.Mode = Enums.ItemMode.Default;
                ArsenalSingle Inscriber = new ArsenalSingle();
                Inscriber.D_UID = client.Entity.UID;
                Inscriber.Item = Item;
                Inscriber.Name = client.Entity.Name;
                Inscriber.UID = Item.UID;
                Inscriber.Type = Type;
                Inscriber.Donation = GetDonation(Item);
                Inscribed[Type].Add(Inscriber.UID, Inscriber);
                Database.ArsenalsTable.Inscribe(Type, Inscriber.Donation, Item, client.Entity);
                Donation[Type] += Inscriber.Donation;
            }
            else
            {
                client.Send(new Network.GamePackets.Message("This item is already inscribed!", System.Drawing.Color.Red, 2005));
                return;
            }
        }

        public void Uninscribe(ArsenalType Type, ArsenalSingle AS, Client.GameState client)
        {
            if (Inscribed[Type].ContainsKey(AS.Item.UID))
            {
                AS.Item.Mode = Enums.ItemMode.Update;
                AS.Item.Inscribed = false;
                AS.Item.Send(client);
                AS.Item.Mode = Enums.ItemMode.Default;
                    Inscribed[Type].Remove(AS.Item.UID);
                Database.ArsenalsTable.Delete(AS.Item.UID);
                Donation[Type] -= AS.Donation;
            }
            else
            {
                client.Send(new Network.GamePackets.Message("This item isn't inscribed!", System.Drawing.Color.Red, 2005));
                return;
            }
        }

        public void Inscribe(ArsenalType Type, ArsenalSingle Inscriber)
        {
            if (!Inscribed[Type].ContainsKey(Inscriber.UID))
            {
                Inscribed[Type].Add(Inscriber.UID, Inscriber);
                Donation[Type] += Inscriber.Donation;
            }
        }

        public void Update(ArsenalType Type, Guild Guild)
        {
            ulong DonationValue = Donation[Type];
            byte pValue = 0;
            if (DonationValue >= 2000000 && DonationValue < 4000000)
                pValue = 1;
            else if (DonationValue >= 4000000 && DonationValue < 10000000)
                pValue = 2;
            else if (DonationValue >= 10000000)
                pValue = 3;
            else pValue = 0;
            switch (Type)
            {
                case ArsenalType.Headgear:
                    Guild.A_Packet.Headgear_Donation = DonationValue;
                    Guild.A_Packet.Headgear_Potency = pValue;
                    break;
                case ArsenalType.Armor:
                    Guild.A_Packet.Armor_Donation = DonationValue;
                    Guild.A_Packet.Armor_Potency = pValue;
                    break;
                case ArsenalType.Weapon:
                    Guild.A_Packet.Weapon_Donation = DonationValue;
                    Guild.A_Packet.Weapon_Potency = pValue;
                    break;
                case ArsenalType.Ring:
                    Guild.A_Packet.Ring_Donation = DonationValue;
                    Guild.A_Packet.Ring_Potency = pValue;
                    break;
                case ArsenalType.Boots:
                    Guild.A_Packet.Boots_Donation = DonationValue;
                    Guild.A_Packet.Boots_Potency = pValue;
                    break;
                case ArsenalType.Necklace:
                    Guild.A_Packet.Necklace_Donation = DonationValue;
                    Guild.A_Packet.Necklace_Potency = pValue;
                    break;
                case ArsenalType.Fan:
                    Guild.A_Packet.Fan_Donation = DonationValue;
                    Guild.A_Packet.Fan_Potency = pValue;
                    break;
                case ArsenalType.Tower:
                    Guild.A_Packet.Tower_Donation = DonationValue;
                    Guild.A_Packet.Tower_Potency = pValue;
                    break;
            }
        }
        public ulong TotalDonation
        {
            get
            {
                ulong DonationValue = 0;
                for (byte i = 0; i < 8; i++)
                {
                    ArsenalType Type = (ArsenalType)i;
                    DonationValue += Donation[Type];
                }
                return DonationValue;
            }
        }
        public void Update(Guild Guild)
        {
            for (byte i = 0; i < 8; i++)
            {
                ArsenalType Type = (ArsenalType)i;
                ulong DonationValue = Donation[Type];
                byte pValue = 0;
                if (DonationValue >= 2000000 && DonationValue < 4000000)
                    pValue = 1;
                else if (DonationValue >= 4000000 && DonationValue < 10000000)
                    pValue = 2;
                else if (DonationValue >= 10000000)
                    pValue = 3;
                else pValue = 0;
                switch (Type)
                {
                    case ArsenalType.Headgear:
                        Guild.A_Packet.Headgear_Donation = DonationValue;
                        Guild.A_Packet.Headgear_Potency = pValue;
                        break;
                    case ArsenalType.Armor:
                        Guild.A_Packet.Armor_Donation = DonationValue;
                        Guild.A_Packet.Armor_Potency = pValue;
                        break;
                    case ArsenalType.Weapon:
                        Guild.A_Packet.Weapon_Donation = DonationValue;
                        Guild.A_Packet.Weapon_Potency = pValue;
                        break;
                    case ArsenalType.Ring:
                        Guild.A_Packet.Ring_Donation = DonationValue;
                        Guild.A_Packet.Ring_Potency = pValue;
                        break;
                    case ArsenalType.Boots:
                        Guild.A_Packet.Boots_Donation = DonationValue;
                        Guild.A_Packet.Boots_Potency = pValue;
                        break;
                    case ArsenalType.Necklace:
                        Guild.A_Packet.Necklace_Donation = DonationValue;
                        Guild.A_Packet.Necklace_Potency = pValue;
                        break;
                    case ArsenalType.Fan:
                        Guild.A_Packet.Fan_Donation = DonationValue;
                        Guild.A_Packet.Fan_Potency = pValue;
                        break;
                    case ArsenalType.Tower:
                        Guild.A_Packet.Tower_Donation = DonationValue;
                        Guild.A_Packet.Tower_Potency = pValue;
                        break;
                }
            }
        }
        public uint GetDonation(Interfaces.IConquerItem Item)
        {
            uint Return = 0;

         if(Item.ToString().EndsWith("8"))
             Return = 1000;
         if (Item.ToString().EndsWith("9"))
             Return = 16660;
            /*switch (Item.ToString())
            {
                case Enums.ItemQuality.Elite: Return = 1000; break;
                case Enums.ItemQuality.Super: Return = 16660; break;
            }*/
            if (Item.SocketOne > 0 && Item.SocketTwo == 0)
                Return += 33330;
            if (Item.SocketTwo > 0 && Item.SocketTwo > 0)
                Return += 133330;

            switch (Item.Plus)
            {
                case 1: Return += 90; break;
                case 2: Return += 490; break;
                case 3: Return += 1350; break;
                case 4: Return += 4070; break;
                case 5: Return += 12340; break;
                case 6: Return += 37030; break;
                case 7: Return += 111110; break;
                case 8: Return += 333330; break;
                case 9: Return += 1000000; break;
                case 10: Return += 1033330; break;
                case 11: Return += 1101230; break;
                case 12: Return += 1212340; break;
                default: break;
            }

            return Return;
        }
    }
}
