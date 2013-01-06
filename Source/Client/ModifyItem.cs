using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Game;

namespace Conquer_Online_Server.Game
{
    public struct ModifyItem
    {
        uint ID;
        public ModifyItem(uint id)
        {
            ID = id;
        }

        public Enums.ItemQuality Quality
        {
            get
            {
                return (Enums.ItemQuality)Digit(6);
            }
        }
        public Enums.Color Color
        {
            get
            {
                return (Enums.Color)Digit(4);
            }
        }
        public void QualityChange(Enums.ItemQuality Quality)
        {
            ChangeDigit(6, (byte)Quality);
        }
        public void ColorChange(Enums.Color Col)
        {
            ChangeDigit(4, (byte)Col);
        }
        public uint Part(byte From, byte To)
        {
            string Item = Convert.ToString(ID);
            string type = Item.Remove(0, From);
            type = type.Remove(To - From, Item.Length - To);
            return uint.Parse(type);
        }
        public static uint Part(uint ID, byte From, byte To)
        {
            if (ID != 0)
            {
                string Item = Convert.ToString(ID);
                string type = Item.Remove(0, From);
                type = type.Remove(To - From, Item.Length - To);
                return uint.Parse(type);
            }
            return 0;
        }
        public byte Digit(byte Place)
        {
            return (byte)Part((byte)(Place - 1), Place);
        }
        public static byte Digit(uint ID, byte Place)
        {
            return (byte)Part(ID, (byte)(Place - 1), Place);
        }
        public void ChangeDigit(byte Place, byte To)
        {
            string Item = Convert.ToString(ID);
            string N = Item.Remove(Place - 1, Item.Length - Place + 1) + To.ToString();
            N += Item.Remove(0, Place);
            ID = uint.Parse(N);
        }
        public void LowestLevel(byte Pos)
        {
            ChangeDigit(4, 0);
            if (Pos == 1 || Pos == 2 || Pos == 3)
                ChangeDigit(5, 0);
            else if (Pos == 8 || Pos == 6)
                ChangeDigit(5, 1);
            else
                ChangeDigit(5, 2);
        }
        public void IncreaseLevel()
        {
            if (ID != 0)
            {
                if (Database.ConquerItemInformation.BaseInfos.ContainsKey(ID))
                {
                    Database.ConquerItemBaseInformation It = Database.ConquerItemInformation.BaseInfos[ID];
                    Database.ConquerItemInformation Info = new Database.ConquerItemInformation(It.ID, 0);
                    if (Info.BaseInformation == null) return;
                    byte Level = Info.BaseInformation.Level;
                    string Type = Info.BaseInformation.ID.ToString().Remove(2, Info.BaseInformation.ID.ToString().Length - 2);
                    uint WeirdThing = Convert.ToUInt32(Type);
                    if (WeirdThing <= 60 && WeirdThing >= 42)
                    {
                        if (Level < 130)
                        {
                            if (Level >= 120)
                            {
                                Level++;
                                foreach (Database.ConquerItemBaseInformation I in Database.ConquerItemInformation.BaseInfos.Values)
                                {
                                    if (I.ID / 1000 == Info.BaseInformation.ID / 1000)
                                        if (I.ID % 10 == Info.BaseInformation.ID % 10)
                                            if (I.Level == Level)
                                            { ID = I.ID; return; }
                                }
                            }
                            else
                            {
                            Again:
                                Level++;
                                foreach (Database.ConquerItemBaseInformation I in Database.ConquerItemInformation.BaseInfos.Values)
                                {
                                    if (I.ID / 1000 == I.ID / 1000)
                                        if (I.ID % 10 == I.ID % 10)
                                            if (I.Level == Level)
                                            { ID = I.ID; return; }
                                }
                                goto Again;
                            }
                        }
                    }
                    else
                    {
                        if (WeirdThing == 20)
                            return;
                    Again:
                        Level++;
                        foreach (Database.ConquerItemBaseInformation I in Database.ConquerItemInformation.BaseInfos.Values)
                        {
                            if (I.ID / 1000 == Info.BaseInformation.ID / 1000)
                                if (I.ID % 10 == Info.BaseInformation.ID % 10)
                                    if (I.Level == Level)
                                    { ID = I.ID; return; }
                        }
                        goto Again;
                    }
                }
            }
        }
        public uint ToID()
        {
            return ID;
        }
        public uint ToComposeID(byte EqPos)
        {
            uint id = ID;

            if (EqPos == 1)
            {
                ChangeDigit(4, 0);
                ChangeDigit(6, 0);
            }
            else if (EqPos == 3)
            {
                ChangeDigit(3, (byte)(Digit(3) - 5));
                ChangeDigit(4, 0);
                ID += 1;
            }
            else if (EqPos == 2 || EqPos == 6 || EqPos == 8)
                ChangeDigit(6, 0);
            else if (EqPos == 4 || EqPos == 5)
            {
                if (Part(0, 3) == 500 || Part(0, 3) == 421 || Part(0, 3) == 601)
                    ChangeDigit(6, 0);
                else if (Digit(1) == 9)
                    ChangeDigit(4, 0);
                else if (Digit(1) == 4 || Digit(1) == 5)
                {
                    ChangeDigit(3, Digit(1));
                    ChangeDigit(2, Digit(1));
                    ChangeDigit(1, Digit(1));
                    ChangeDigit(6, 0);
                }
            }
            else if (EqPos == 10 || EqPos == 11)
                ChangeDigit(6, 0);

            uint ret = ID;
            ID = id;
            return ret;
        }
    }
}