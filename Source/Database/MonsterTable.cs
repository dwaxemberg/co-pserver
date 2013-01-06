using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.ServerBase;

namespace Conquer_Online_Server.Database
{
    public class MonsterInformation
    {
        private struct SpecialItemDrop
        {
            public int ItemID, Rate, Discriminant, Map;
        }
        private static List<SpecialItemDrop> SpecialItemDropList = new List<SpecialItemDrop>();
        public Game.Entity Owner;

        public uint ExcludeFromSend = 0;
        private bool LabirinthDrop = false;
        public uint ID;
        public ushort Mesh;
        public byte Level;
        public string Name;
        public uint Hitpoints;
        public ushort ViewRange;
        public ushort AttackRange;
        public int RespawnTime;
        public uint MinAttack, MaxAttack;
        public byte AttackType;
        public ushort SpellID;
        public uint InSight;
        public bool Boss, SuperBoss;
        public Time32 LastMove;
        public int MoveSpeed;
        public int RunSpeed;
        public int OwnItemID, OwnItemRate;
        public int HPPotionID, MPPotionID;
        public int AttackSpeed;
        public int MinimumSpeed
        {
            get
            {
                int min = 10000000;
                if (min > MoveSpeed)
                    min = MoveSpeed;
                if (min > RunSpeed)
                    min = RunSpeed;
                if (min > AttackSpeed)
                    min = AttackSpeed;
                return min;
            }
        }
        public uint ExtraExperience;
        public uint MinMoneyDropAmount;
        public uint MaxMoneyDropAmount;

        public ushort BoundX, BoundY;
        public ushort BoundCX, BoundCY;

        public static SafeDictionary<byte, List<uint>> ItemDropCache = new SafeDictionary<byte, List<uint>>(3000);
        public static SafeDictionary<byte, List<uint>> SoulItemCache = new SafeDictionary<byte, List<uint>>(3000);

        public void SendScreen(byte[] buffer)
        {
            foreach (Client.GameState client in Program.Values)
            {
                if (client != null)
                {
                    if (client.Entity.UID != ExcludeFromSend)
                    {
                        if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, Owner.X, Owner.Y) > 18)
                        {
                            continue;
                        }
                        client.Send(buffer);
                    }
                }
            }
        }
        public void SendScreen(Interfaces.IPacket buffer)
        {
            SendScreen(buffer.ToArray());
        }
        public void SendScreenSpawn(Interfaces.IMapObject _object)
        {
            foreach (Client.GameState client in Program.Values)
            {
                if (client != null)
                {
                    if (client.Entity.UID != ExcludeFromSend)
                    {
                        if (client.Map.ID == Owner.MapID)
                        {
                            if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, Owner.X, Owner.Y) > 25)
                            {
                                continue;
                            }
                            _object.SendSpawn(client, false);

                        }
                    }
                }
            }
        }
        public void Drop(Game.Entity killer)
        {
            if (Owner.Name.Contains("Guard") || killer.Name.Contains("Guard1"))
                return;

            #region CleansingDevil
            if (Name == "CleansingDevil" && killer.MapID == 1116)
            {
                Random R = new Random();
                int Nr = R.Next(1, 10);
                if (Nr == 1) killer.Owner.Inventory.Add(184355, 0, 1);
                if (Nr == 2) killer.Owner.Inventory.Add(560339, 0, 1);
                if (Nr == 3) killer.Owner.Inventory.Add(183365, 0, 1);
                if (Nr == 4) killer.Owner.Inventory.Add(183335, 0, 1);
                if (Nr == 5) killer.Owner.Inventory.Add(184345, 0, 1);
                if (Nr == 6) killer.Owner.Inventory.Add(184365, 0, 1);
                if (Nr == 7) killer.Owner.Inventory.Add(184385, 0, 1);
                if (Nr == 8) killer.Owner.Inventory.Add(183395, 0, 1);
                if (Nr == 9) killer.Owner.Inventory.Add(183385, 0, 1);
                if (Nr == 10) killer.Owner.Inventory.Add(722057, 0, 1);
                killer.Teleport(1002, 400, 400);
                Time32 Now = Time32.Now;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                    if (Chaar != null)
                    {
                        if (Chaar.Entity.MapID == 1116)
                        {
                            Chaar.Entity.Teleport(1002, 400, 400);
                        }
                    }
                }
            }
            #endregion
            #region Sizer
            if (Name == "Sizer" && killer.MapID == 3030)
            {
                Random R = new Random();
                int Nr = R.Next(1, 21);
                if (Nr == 1) killer.Owner.Inventory.Add35(184355, 12, 1);
                if (Nr == 2) killer.Owner.Inventory.Add35(560339, 12, 1);
                if (Nr == 3) killer.Owner.Inventory.Add35(183365, 12, 1);
                if (Nr == 4) killer.Owner.Inventory.Add35(183335, 12, 1);
                if (Nr == 5) killer.Owner.Inventory.Add35(184345, 12, 1);
                if (Nr == 6) killer.Owner.Inventory.Add35(184365, 12, 1);
                if (Nr == 7) killer.Owner.Inventory.Add35(184385, 12, 1);
                if (Nr == 8) killer.Owner.Inventory.Add35(183395, 12, 1);
                if (Nr == 9) killer.Owner.Inventory.Add35(183385, 12, 1);
                if (Nr == 10) killer.Owner.Inventory.Add35(192300, 12, 1);
                if (Nr == 11) killer.Owner.Inventory.Add35(184335, 12, 1);
                if (Nr == 12) killer.Owner.Inventory.Add35(187355, 12, 1);
                if (Nr == 13) killer.Owner.Inventory.Add35(193300, 12, 1);
                if (Nr == 14) killer.Owner.Inventory.Add35(194300, 12, 1);
                if (Nr == 15) killer.Owner.Inventory.Add35(420339, 12, 1);
                if (Nr == 16) killer.Owner.Inventory.Add35(480339, 12, 1);
                if (Nr == 17) killer.Owner.Inventory.Add35(410339, 12, 1);
                if (Nr == 18) killer.Owner.Inventory.Add35(500329, 12, 1);
                if (Nr == 19) killer.Owner.Inventory.Add35(421339, 12, 1);
                if (Nr == 20) killer.Owner.Inventory.Add35(561339, 12, 1);
                if (Nr == 21) killer.Owner.Inventory.Add(722057, 0, 1);
            }
            #endregion
            #region Peter
            if (Name == "Peter" && killer.MapID == 3030)
            {
                Random R = new Random();
                int Nr = R.Next(1, 21);
                if (Nr == 1) killer.Owner.Inventory.Add35(184355, 12, 1);
                if (Nr == 2) killer.Owner.Inventory.Add35(560339, 12, 1);
                if (Nr == 3) killer.Owner.Inventory.Add35(183365, 12, 1);
                if (Nr == 4) killer.Owner.Inventory.Add35(183335, 12, 1);
                if (Nr == 5) killer.Owner.Inventory.Add35(184345, 12, 1);
                if (Nr == 6) killer.Owner.Inventory.Add35(184365, 12, 1);
                if (Nr == 7) killer.Owner.Inventory.Add35(184385, 12, 1);
                if (Nr == 8) killer.Owner.Inventory.Add35(183395, 12, 1);
                if (Nr == 9) killer.Owner.Inventory.Add35(183385, 12, 1);
                if (Nr == 10) killer.Owner.Inventory.Add35(192300, 12, 1);
                if (Nr == 11) killer.Owner.Inventory.Add35(184335, 12, 1);
                if (Nr == 12) killer.Owner.Inventory.Add35(187355, 12, 1);
                if (Nr == 13) killer.Owner.Inventory.Add35(193300, 12, 1);
                if (Nr == 14) killer.Owner.Inventory.Add35(194300, 12, 1);
                if (Nr == 15) killer.Owner.Inventory.Add35(420339, 12, 1);
                if (Nr == 16) killer.Owner.Inventory.Add35(480339, 12, 1);
                if (Nr == 17) killer.Owner.Inventory.Add35(410339, 12, 1);
                if (Nr == 18) killer.Owner.Inventory.Add35(500329, 12, 1);
                if (Nr == 19) killer.Owner.Inventory.Add35(421339, 12, 1);
                if (Nr == 20) killer.Owner.Inventory.Add35(561339, 12, 1);
                if (Nr == 21) killer.Owner.Inventory.Add(722057, 0, 1);
            }
            #endregion
            #region DragonSon 1
            if (Name == "DragonSon1")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }
                killer.Teleport(1596, 130, 130);
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("" + killer.Name + " Has Killed First DragonSon And Will Make DragonMon Angry !");
                Time32 Now = Time32.Now;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                    if (Chaar != null)
                    {
                        if (Chaar.Entity.MapID == 1595)
                        {
                            Chaar.Entity.Teleport(1596, 130, 130);
                        }
                    }
                }
            }
            #endregion
            #region DragonSon 2
            if (Name == "DragonSon2")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }
                killer.Teleport(1597, 130, 130);
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("" + killer.Name + " Has Killed 2nd DragonSon And Will Make DragonMon Angry !");
                Time32 Now = Time32.Now;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                    if (Chaar != null)
                    {
                        if (Chaar.Entity.MapID == 1596)
                        {
                            Chaar.Entity.Teleport(1597, 130, 130);
                        }
                    }
                }
            }
            #endregion
            #region DragonSon 3
            if (Name == "DragonSon3")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }
                killer.Teleport(1598, 130, 130);
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("" + killer.Name + " Has Killed 3rd DragonSon And Will Make DragonMon Angry !");
                Time32 Now = Time32.Now;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                    if (Chaar != null)
                    {
                        if (Chaar.Entity.MapID == 1597)
                        {
                            Chaar.Entity.Teleport(1598, 130, 130);
                        }
                    }
                }
            }
            #endregion
            #region DragonSon 4
            if (Name == "DragonSon4")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }
                killer.Teleport(1599, 130, 130);
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("" + killer.Name + " Has Killed 4th DragonSon And Make DragonMon Angry !");
                Time32 Now = Time32.Now;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                    if (Chaar != null)
                    {
                        if (Chaar.Entity.MapID == 1598)
                        {
                            Chaar.Entity.Teleport(1599, 130, 130);
                        }
                    }
                }
            }
            #endregion
            #region DragonMom
            if (Name == "TeratoDragon" && killer.MapID == 1599)
            {
                killer.ConquerPoints += 5000;
                killer.Teleport(1002, 400, 400);
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("" + killer.Name + " Has Killed The DragonMon And Won 5K CPs :D !");
                Time32 Now = Time32.Now;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                    if (Chaar != null)
                    {
                        if (Chaar.Entity.MapID == 1599)
                        {
                            Chaar.Entity.Teleport(1002, 400, 400);
                        }
                    }
                }
            }
            #endregion

            #region DiabloRojoL117 Cps Drop
            if (Name == "DiabloRojoL117")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 15;
                killer.SubClasses.StudyPoints += 50;
                killer.Owner.Send(new Network.GamePackets.Message("15 Cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion

            #region Pheasant Cps Drop
            if (Name == "Faisan")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 3;
                killer.SubClasses.StudyPoints += 2;
                killer.Owner.Send(new Network.GamePackets.Message("3 Cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region Birdman Cps Drop
            if (Name == "HombrePajaro")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 17;
                killer.SubClasses.StudyPoints += 5;
                killer.Owner.Send(new Network.GamePackets.Message(" 17 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion

            #region HawKing Cps Drop
            if (Name == "JefeDeAguila")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 15;
                killer.SubClasses.StudyPoints += 5;
                killer.Owner.Send(new Network.GamePackets.Message("15 cps ", System.Drawing.Color.Blue, 2005));
            }
            #endregion

            #region Birdman Cps Drop
            if (Name == "cuatrero")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 17;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 17 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region BanditL97 Cps Drop
            if (Name == "BandidoLv97")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 16;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 16 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region Murcielago Cps Drop
            if (Name == "Murcielago")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 15;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 15 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region Vampiro Cps Drop
            if (Name == "Vampiro")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 15;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 18 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region BuhoVampiro Cps Drop
            if (Name == "BuhoVampiro")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 15;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 15 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region ToroMonstruoso Cps Drop
            if (Name == "ToroMonstruoso")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 13;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 13 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region MonstrToroLv113 Cps Drop
            if (Name == "MonstrToroLv113")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 15;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 30 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region Macaco Cps Drop
            if (Name == "Macaco")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 12;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 30 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion
            #region MonoGigante Cps Drop
            if (Name == "MonoGigante")
            {
                if (killer.Name.Contains("Guard"))
                {
                    return;
                }

                killer.ConquerPoints += 9;
                killer.SubClasses.StudyPoints += 10;
                killer.Owner.Send(new Network.GamePackets.Message(" 9 cps", System.Drawing.Color.Blue, 2005));
            }
            #endregion

            


            #region TeratoDragon Cps Drop
            if (Name == "TeratoDragon")
            {

                killer.ConquerPoints += 3000;
                killer.SubClasses.StudyPoints += 1000;
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Congratulations!  " + killer.Name + "! killed TeratoDragon and Got 3000 CPS and 1000 StudyPoint.");
            }
            if (Name == "KingMonster")
            { Program.Kingmonster = true; }
            if (Name == "princesMonster")
            { Program.Princesmonster = true; }
            if (Name == "DukeMonster")
            { Program.Dukemonster = true; }
            #endregion

            #region SnowBanshee Cps Drop

            if (Name == "SnowBanshee")
            {

                killer.ConquerPoints += 3000;
                killer.SubClasses.StudyPoints += 1000;
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Congratulations!  " + killer.Name + "! killed SnowBanshee and got 3000 CPS and 1000  StudyPoint.");

            }
            #endregion

            #region DragonSoul Drops
            if (Name == "TeratoDragon")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 22);
                    switch (type)
                    {
                        case 1: Uid = 800000; break;
                        case 2: Uid = 800017; break;
                        case 3: Uid = 800110; break;
                        case 4: Uid = 800320; break;
                        case 5: Uid = 800421; break;
                        case 6: Uid = 800513; break;
                        case 7: Uid = 800616; break;
                        case 8: Uid = 800722; break;
                        case 9: Uid = 820056; break;
                        case 10: Uid = 820057; break;
                        case 11: Uid = 822053; break;
                        case 12: Uid = 822055; break;
                        case 13: Uid = 823055; break;
                        case 14: Uid = 823056; break;
                        case 15: Uid = 820071; break;
                        case 16: Uid = 821031; break;
                        case 17: Uid = 821032; break;
                        case 18: Uid = 823055; break;
                        case 19: Uid = 823056; break;
                        case 20: Uid = 823057; break;
                        case 21: Uid = 824017; break;



                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "LavaBeast")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 22);
                    switch (type)
                    {
                        case 1: Uid = 800000; break;
                        case 2: Uid = 800017; break;
                        case 3: Uid = 800110; break;
                        case 4: Uid = 800320; break;
                        case 5: Uid = 800421; break;
                        case 6: Uid = 800513; break;
                        case 7: Uid = 800616; break;
                        case 8: Uid = 800722; break;
                        case 9: Uid = 820056; break;
                        case 10: Uid = 820057; break;
                        case 11: Uid = 822053; break;
                        case 12: Uid = 822055; break;
                        case 13: Uid = 823055; break;
                        case 14: Uid = 823056; break;
                        case 15: Uid = 820071; break;
                        case 16: Uid = 821031; break;
                        case 17: Uid = 821032; break;
                        case 18: Uid = 823055; break;
                        case 19: Uid = 823056; break;
                        case 20: Uid = 823057; break;
                        case 21: Uid = 824017; break;


            
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "SnowBanshee")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 22);
                    switch (type)
                    {
                        case 1: Uid = 800000; break;
                        case 2: Uid = 800017; break;
                        case 3: Uid = 800110; break;
                        case 4: Uid = 800320; break;
                        case 5: Uid = 800421; break;
                        case 6: Uid = 800513; break;
                        case 7: Uid = 800616; break;
                        case 8: Uid = 800722; break;
                        case 9: Uid = 820056; break;
                        case 10: Uid = 820057; break;
                        case 11: Uid = 822053; break;
                        case 12: Uid = 822055; break;
                        case 13: Uid = 823055; break;
                        case 14: Uid = 823056; break;
                        case 15: Uid = 820071; break;
                        case 16: Uid = 821031; break;
                        case 17: Uid = 821032; break;
                        case 18: Uid = 823055; break;
                        case 19: Uid = 823056; break;
                        case 20: Uid = 823057; break;
                        case 21: Uid = 824017; break;


                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "ThrillingSpook")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 22);
                    switch (type)
                    {
                        case 1: Uid = 800000; break;
                        case 2: Uid = 800014; break;
                        case 3: Uid = 800015; break;
                        case 4: Uid = 800016; break;
                        case 5: Uid = 800017; break;
                        case 6: Uid = 800110; break;
                        case 7: Uid = 800214; break;
                        case 8: Uid = 800320; break;
                        case 9: Uid = 800415; break;
                        case 10: Uid = 800512; break;
                        case 11: Uid = 800513; break;
                        case 12: Uid = 800613; break;
                        case 13: Uid = 800616; break;
                        case 14: Uid = 800720; break;
                        case 15: Uid = 800722; break;
                        case 16: Uid = 820056; break;
                        case 17: Uid = 820057; break;
                        case 18: Uid = 822053; break;
                        case 19: Uid = 822055; break;
                        case 20: Uid = 823052; break;
                        case 21: Uid = 823053; break;
                        case 22: Uid = 823054; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "SwordMaster")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 22);
                    switch (type)
                    {
                        case 1: Uid = 800019; break;
                        case 2: Uid = 800050; break;
                        case 3: Uid = 800070; break;
                        case 4: Uid = 800071; break;
                        case 5: Uid = 800140; break;
                        case 6: Uid = 800141; break;
                        case 7: Uid = 800200; break;
                        case 8: Uid = 800230; break;
                        case 9: Uid = 800414; break;
                        case 10: Uid = 800520; break;
                        case 11: Uid = 800521; break;
                        case 12: Uid = 800615; break;
                        case 13: Uid = 800617; break;
                        case 14: Uid = 800723; break;
                        case 15: Uid = 800724; break;
                        case 16: Uid = 820052; break;
                        case 17: Uid = 820053; break;
                        case 18: Uid = 822056; break;
                        case 19: Uid = 822057; break;
                        case 20: Uid = 823041; break;
                        case 21: Uid = 823043; break;
                        case 22: Uid = 823045; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "FlyingRooster")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 45);
                    switch (type)
                    {
                        case 1: Uid = 800001; break;
                        case 2: Uid = 800002; break;
                        case 3: Uid = 800003; break;
                        case 4: Uid = 800004; break;
                        case 5: Uid = 800005; break;
                        case 6: Uid = 800201; break;
                        case 7: Uid = 800202; break;
                        case 8: Uid = 800203; break;
                        case 9: Uid = 800402; break;
                        case 10: Uid = 800403; break;
                        case 11: Uid = 800501; break;
                        case 12: Uid = 800502; break;
                        case 13: Uid = 800503; break;
                        case 14: Uid = 800601; break;
                        case 15: Uid = 800602; break;
                        case 16: Uid = 800603; break;
                        case 17: Uid = 800701; break;
                        case 18: Uid = 800702; break;
                        case 19: Uid = 800703; break;
                        case 20: Uid = 820002; break;
                        case 21: Uid = 820003; break;
                        case 22: Uid = 820012; break;
                        case 23: Uid = 820022; break;
                        case 24: Uid = 820023; break;
                        case 25: Uid = 820032; break;
                        case 26: Uid = 820033; break;
                        case 27: Uid = 820042; break;
                        case 28: Uid = 820043; break;
                        case 29: Uid = 821002; break;
                        case 30: Uid = 821003; break;
                        case 31: Uid = 821004; break;
                        case 32: Uid = 821015; break;
                        case 33: Uid = 821016; break;
                        case 34: Uid = 821025; break;
                        case 35: Uid = 821026; break;
                        case 36: Uid = 822012; break;
                        case 37: Uid = 822013; break;
                        case 38: Uid = 822022; break;
                        case 39: Uid = 822032; break;
                        case 40: Uid = 822042; break;
                        case 41: Uid = 823001; break;
                        case 42: Uid = 823002; break;
                        case 43: Uid = 823015; break;
                        case 44: Uid = 823028; break;
                        case 45: Uid = 824002; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "GreenDevil")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 36);
                    switch (type)
                    {
                        case 1: Uid = 800010; break;
                        case 2: Uid = 800011; break;
                        case 3: Uid = 800012; break;
                        case 4: Uid = 800013; break;
                        case 5: Uid = 800210; break;
                        case 6: Uid = 800211; break;
                        case 7: Uid = 800212; break;
                        case 8: Uid = 800213; break;
                        case 9: Uid = 800310; break;
                        case 10: Uid = 800018; break;
                        case 11: Uid = 800090; break;
                        case 12: Uid = 800130; break;
                        case 13: Uid = 800401; break;
                        case 14: Uid = 800413; break;
                        case 15: Uid = 800420; break;
                        case 16: Uid = 800514; break;
                        case 17: Uid = 800614; break;
                        case 18: Uid = 820001; break;
                        case 19: Uid = 820054; break;
                        case 20: Uid = 820055; break;
                        case 21: Uid = 821014; break;
                        case 22: Uid = 821026; break;
                        case 23: Uid = 822001; break;
                        case 24: Uid = 822052; break;
                        case 25: Uid = 822054; break;
                        case 26: Uid = 823040; break;
                        case 27: Uid = 823042; break;
                        case 28: Uid = 823044; break;
                        case 29: Uid = 823046; break;
                        case 30: Uid = 823047; break;
                        case 31: Uid = 823048; break;
                        case 32: Uid = 823049; break;
                        case 33: Uid = 823050; break;
                        case 34: Uid = 823051; break;
                        case 35: Uid = 824001; break;
                        case 36: Uid = 824014; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "BullDevil")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 36);
                    switch (type)
                    {
                        case 1: Uid = 800010; break;
                        case 2: Uid = 800011; break;
                        case 3: Uid = 800012; break;
                        case 4: Uid = 800013; break;
                        case 5: Uid = 800210; break;
                        case 6: Uid = 800211; break;
                        case 7: Uid = 800212; break;
                        case 8: Uid = 800213; break;
                        case 9: Uid = 800310; break;
                        case 10: Uid = 800018; break;
                        case 11: Uid = 800090; break;
                        case 12: Uid = 800130; break;
                        case 13: Uid = 800401; break;
                        case 14: Uid = 800413; break;
                        case 15: Uid = 800420; break;
                        case 16: Uid = 800514; break;
                        case 17: Uid = 800614; break;
                        case 18: Uid = 820001; break;
                        case 19: Uid = 820054; break;
                        case 20: Uid = 820055; break;
                        case 21: Uid = 821014; break;
                        case 22: Uid = 821026; break;
                        case 23: Uid = 822001; break;
                        case 24: Uid = 822052; break;
                        case 25: Uid = 822054; break;
                        case 26: Uid = 823040; break;
                        case 27: Uid = 823042; break;
                        case 28: Uid = 823044; break;
                        case 29: Uid = 823046; break;
                        case 30: Uid = 823047; break;
                        case 31: Uid = 823048; break;
                        case 32: Uid = 823049; break;
                        case 33: Uid = 823050; break;
                        case 34: Uid = 823051; break;
                        case 35: Uid = 824001; break;
                        case 36: Uid = 824014; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "BloodyKing")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 36);
                    switch (type)
                    {
                        case 1: Uid = 800010; break;
                        case 2: Uid = 800011; break;
                        case 3: Uid = 800012; break;
                        case 4: Uid = 800013; break;
                        case 5: Uid = 800210; break;
                        case 6: Uid = 800211; break;
                        case 7: Uid = 800212; break;
                        case 8: Uid = 800213; break;
                        case 9: Uid = 800310; break;
                        case 10: Uid = 800018; break;
                        case 11: Uid = 800090; break;
                        case 12: Uid = 800130; break;
                        case 13: Uid = 800401; break;
                        case 14: Uid = 800413; break;
                        case 15: Uid = 800420; break;
                        case 16: Uid = 800514; break;
                        case 17: Uid = 800614; break;
                        case 18: Uid = 820001; break;
                        case 19: Uid = 820054; break;
                        case 20: Uid = 820055; break;
                        case 21: Uid = 821014; break;
                        case 22: Uid = 821026; break;
                        case 23: Uid = 822001; break;
                        case 24: Uid = 822052; break;
                        case 25: Uid = 822054; break;
                        case 26: Uid = 823040; break;
                        case 27: Uid = 823042; break;
                        case 28: Uid = 823044; break;
                        case 29: Uid = 823046; break;
                        case 30: Uid = 823047; break;
                        case 31: Uid = 823048; break;
                        case 32: Uid = 823049; break;
                        case 33: Uid = 823050; break;
                        case 34: Uid = 823051; break;
                        case 35: Uid = 824001; break;
                        case 36: Uid = 824014; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            if (Name == "BirdKing")
            {

                byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 36);
                    switch (type)
                    {
                        case 1: Uid = 800010; break;
                        case 2: Uid = 800011; break;
                        case 3: Uid = 800012; break;
                        case 4: Uid = 800013; break;
                        case 5: Uid = 800210; break;
                        case 6: Uid = 800211; break;
                        case 7: Uid = 800212; break;
                        case 8: Uid = 800213; break;
                        case 9: Uid = 800310; break;
                        case 10: Uid = 800018; break;
                        case 11: Uid = 800090; break;
                        case 12: Uid = 800130; break;
                        case 13: Uid = 800401; break;
                        case 14: Uid = 800413; break;
                        case 15: Uid = 800420; break;
                        case 16: Uid = 800514; break;
                        case 17: Uid = 800614; break;
                        case 18: Uid = 820001; break;
                        case 19: Uid = 820054; break;
                        case 20: Uid = 820055; break;
                        case 21: Uid = 821014; break;
                        case 22: Uid = 821026; break;
                        case 23: Uid = 822001; break;
                        case 24: Uid = 822052; break;
                        case 25: Uid = 822054; break;
                        case 26: Uid = 823040; break;
                        case 27: Uid = 823042; break;
                        case 28: Uid = 823044; break;
                        case 29: Uid = 823046; break;
                        case 30: Uid = 823047; break;
                        case 31: Uid = 823048; break;
                        case 32: Uid = 823049; break;
                        case 33: Uid = 823050; break;
                        case 34: Uid = 823051; break;
                        case 35: Uid = 824001; break;
                        case 36: Uid = 824014; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.MobDropped = true;
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }
            #endregion

            #region RespawnTime
            if (Name == "TeratoDragon")
            {
                RespawnTime += 15;
            }
            #endregion

            #region RespawnTime
            if (Name == "SnowBanshee")
            {
                RespawnTime += 30;
            }
            #endregion

            #region RespawnTime
            if (Name == "LavaBeast")
            {
                RespawnTime += 15;
            }
            #endregion

            #region RespawnTime
            if (Name == "ThrillingSpook")
            {
                RespawnTime += 15;
            }
            #endregion

            #region RespawnTime
            if (Name == "SwordMaster")
            {
                RespawnTime += 30;
            }
            #endregion

            if (Name == "LavaBeast")
                goto Jump4;
            if (Name == "TeratoDragon" || Name == "SnowBanshee" || Name == "ThrillingSpook")
                goto Jump;
            if (this.Name == "Demon")
            {
                goto Jump5;
            }
            if (this.Name == "AncientDemon")
            {
                goto Jump6;
            }
            if (this.Name == "FloodDemon")
            {
                goto Jump7;
            }
            if (this.Name == "HeavenDemon")
            {
                goto Jump8;
            }
            if (this.Name == "ChaosDemon")
            {
                goto Jump9;
            }
        Jump:
            #region TeratoDragon  SnowBanshee  ThrillingSpook
            if (this.Name == "TeratoDragon" || this.Name == "SnowBanshee" || this.Name == "ThrillingSpook")
            {

                uint Uid = 0;
                byte type = (byte)ServerBase.Kernel.Random.Next(1, 50);
                switch (type)
                {
                    case 1:
                        Uid = 800000;
                        break;

                    case 2:
                        Uid = 822054;
                        break;

                    case 3:
                        Uid = 822053;
                        break;

                    case 4:
                        Uid = 822055;
                        break;

                    case 5:
                        Uid = 822056;
                        break;

                    case 6:
                        Uid = 822057;
                        break;

                    case 7:
                        Uid = 800014;
                        break;

                    case 8:
                        Uid = 800019;
                        break;

                    case 9:
                        Uid = 800050;
                        break;

                    case 10:
                        Uid = 800015;
                        break;

                    case 11:
                        Uid = 800090;
                        break;

                    case 12:
                        Uid = 800110;
                        break;

                    case 13:
                        Uid = 800070;
                        break;

                    case 14:
                        Uid = 800071;
                        break;

                    case 15:
                        Uid = 800016;
                        break;

                    case 16:
                        Uid = 800017;
                        break;

                    case 17:
                        Uid = 800130;
                        break;

                    case 18:
                        Uid = 800140;
                        break;

                    case 19:
                        Uid = 800141;
                        break;

                    case 20:
                        Uid = 800200;
                        break;

                    case 21:
                        Uid = 800310;
                        break;

                    case 22:
                        Uid = 800320;
                        break;

                    case 23:
                        Uid = 800214;
                        break;

                    case 24:
                        Uid = 800230;
                        break;

                    case 25:
                        Uid = 800414;
                        break;

                    case 26:
                        Uid = 800415;
                        break;

                    case 27:
                        Uid = 800420;
                        break;

                    case 28:
                        Uid = 800401;
                        break;

                    case 29:
                        Uid = 800512;
                        break;

                    case 30:
                        Uid = 800513;
                        break;

                    case 31:
                        Uid = 800514;
                        break;

                    case 32:
                        Uid = 800520;
                        break;

                    case 33:
                        Uid = 800521;
                        break;

                    case 34:
                        Uid = 800613;
                        break;

                    case 35:
                        Uid = 800614;
                        break;

                    case 36:
                        Uid = 800615;
                        break;

                    case 37:
                        Uid = 800616;
                        break;

                    case 38:
                        Uid = 800617;
                        break;

                    case 39:
                        Uid = 800720;
                        break;

                    case 40:
                        Uid = 800721;
                        break;

                    case 41:
                        Uid = 800722;
                        break;

                    case 42:
                        Uid = 800723;
                        break;

                    case 43:
                        Uid = 800724;
                        break;

                    case 44:
                        Uid = 800018;
                        break;

                    case 45:
                        Uid = 820001;
                        break;

                    case 46:
                        Uid = 820052;
                        break;

                    case 47:
                        Uid = 820053;
                        break;

                    case 48:
                        Uid = 820054;
                        break;

                    case 49:
                        Uid = 820055;
                        break;

                    case 50:
                        Uid = 820057;
                        break;
                }
                killer.Owner.Inventory.Add(Uid, 0, 1);
                killer.SubClasses.StudyPoints += 200;

            }
            #endregion

        Jump4:
            #region LavaBeast  SwordMaster
            if (this.Name == "LavaBeast" || this.Name == "SwordMaster")
            {

                uint Uid = 0;
                byte type = (byte)ServerBase.Kernel.Random.Next(1, 21);
                switch (type)
                {
                    case 1: Uid = 822052; break;
                    case 2: Uid = 822053; break;
                    case 3: Uid = 822054; break;
                    case 4: Uid = 822055; break;
                    case 5: Uid = 800413; break;
                    case 6: Uid = 800414; break;
                    case 7: Uid = 800014; break;
                    case 8: Uid = 800015; break;
                    case 9: Uid = 800016; break;
                    case 10: Uid = 800512; break;
                    case 11: Uid = 800613; break;
                    case 12: Uid = 800415; break;
                    case 13: Uid = 800420; break;
                    case 14: Uid = 800513; break;
                    case 15: Uid = 800017; break;
                    case 16: Uid = 820052; break;
                    case 17: Uid = 820053; break;
                    case 18: Uid = 820054; break;
                    case 19: Uid = 820055; break;
                    case 20: Uid = 820056; break;
                    case 21: Uid = 820057; break;
                }


                killer.Owner.Inventory.Add(Uid, 0, 1);

                //killer.ConquerPoints += 100;
                killer.SubClasses.StudyPoints += 100;
                //ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + "Has Killed " + Owner.Name + " And Take 100 CPs!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);

            }
            #endregion


        Jump5:
            {
                #region Demon
                if (this.Name == "Demon")
                {
                    Random R1 = new Random();
                    int Nr2 = R1.Next(1, 6);
                    if (Nr2 == 1 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(70, 85)))
                    {
                        killer.Owner.Inventory.Add(720657, 0, 1);
                    }
                    if (Nr2 == 2 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(45, 60)))
                    {
                        killer.Owner.Inventory.Add(720656, 0, 1);
                    }
                    if (Nr2 == 3 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(20, 40)))
                    {
                        killer.Owner.Inventory.Add(720655, 0, 1);
                    }
                    if (Nr2 == 4 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(1, 3)))
                    {
                        killer.Owner.Inventory.Add(720654, 0, 1);
                    }
                    if (Nr2 == 5 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(80, 100)))
                    {
                        killer.Owner.Inventory.Add(720668, 0, 1);
                    }
                }
                #endregion
            }
        Jump6:
            {
                #region AncientDemon
                if (this.Name == "AncientDemon")
                {
                    Random R1 = new Random();
                    int Nr2 = R1.Next(1, 7);
                    if (Nr2 == 1 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(70, 85)))
                    {
                        killer.Owner.Inventory.Add(720658, 0, 1);
                    }
                    if (Nr2 == 2 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(45, 60)))
                    {
                        killer.Owner.Inventory.Add(720659, 0, 1);
                    }
                    if (Nr2 == 3 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(20, 40)))
                    {
                        killer.Owner.Inventory.Add(720660, 0, 1);
                    }
                    if (Nr2 == 4 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(3, 5)))
                    {
                        killer.Owner.Inventory.Add(720661, 0, 1);
                    }
                    if (Nr2 == 5 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(1, 2)))
                    {
                        killer.Owner.Inventory.Add(720662, 0, 1);
                    }
                    if (Nr2 == 6 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(80, 100)))
                    {
                        killer.Owner.Inventory.Add(720669, 0, 1);
                    }
                }
                #endregion
            }
        Jump7:
            {
                #region FloodDemon
                if (this.Name == "FloodDemon")
                {
                    Random R1 = new Random();
                    int Nr2 = R1.Next(1, 7);
                    if (Nr2 == 1 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(70, 85)))
                    {
                        killer.Owner.Inventory.Add(720663, 0, 1);
                    }
                    if (Nr2 == 2 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(45, 60)))
                    {
                        killer.Owner.Inventory.Add(720664, 0, 1);
                    }
                    if (Nr2 == 3 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(20, 40)))
                    {
                        killer.Owner.Inventory.Add(720665, 0, 1);
                    }
                    if (Nr2 == 4 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(3, 5)))
                    {
                        killer.Owner.Inventory.Add(720666, 0, 1);
                    }
                    if (Nr2 == 5 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(1, 2)))
                    {
                        killer.Owner.Inventory.Add(720667, 0, 1);
                    }
                    if (Nr2 == 6 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(80, 100)))
                    {
                        killer.Owner.Inventory.Add(720670, 0, 1);
                    }
                }
                #endregion
            }
        Jump8:
            {
                #region HeavenDemon
                if (this.Name == "HeavenDemon")
                {
                    Random R1 = new Random();
                    int Nr2 = R1.Next(1, 7);
                    if (Nr2 == 1 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(70, 85)))
                    {
                        killer.Owner.Inventory.Add(720675, 0, 1);
                    }
                    if (Nr2 == 2 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(45, 60)))
                    {
                        killer.Owner.Inventory.Add(720676, 0, 1);
                    }
                    if (Nr2 == 3 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(20, 40)))
                    {
                        killer.Owner.Inventory.Add(720677, 0, 1);
                    }
                    if (Nr2 == 4 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(3, 5)))
                    {
                        killer.Owner.Inventory.Add(720678, 0, 1);
                    }
                    if (Nr2 == 5 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(1, 2)))
                    {
                        killer.Owner.Inventory.Add(720679, 0, 1);
                    }
                    if (Nr2 == 6 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(80, 100)))
                    {
                        killer.Owner.Inventory.Add(720680, 0, 1);
                    }
                }
                #endregion
            }
        Jump9:
            {
                #region ChaosDemon
                if (this.Name == "ChaosDemon")
                {
                    Random R1 = new Random();
                    int Nr2 = R1.Next(1, 7);
                    if (Nr2 == 1 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(70, 85)))
                    {
                        killer.Owner.Inventory.Add(720681, 0, 1);
                    }
                    if (Nr2 == 2 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(45, 60)))
                    {
                        killer.Owner.Inventory.Add(720682, 0, 1);
                    }
                    if (Nr2 == 3 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(20, 40)))
                    {
                        killer.Owner.Inventory.Add(720683, 0, 1);
                    }
                    if (Nr2 == 4 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(3, 5)))
                    {
                        killer.Owner.Inventory.Add(720684, 0, 1);
                    }
                    if (Nr2 == 5 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(1, 2)))
                    {
                        killer.Owner.Inventory.Add(720685, 0, 1);
                    }
                    if (Nr2 == 6 && ServerBase.Kernel.ChanceSuccess(ServerBase.Kernel.Random.Next(80, 100)))
                    {
                        killer.Owner.Inventory.Add(720686, 0, 1);
                    }
                }
                #endregion
            }
            if (killer.MapID == 1013)
            {
                if (ServerBase.Kernel.Rate(10))
                {

                    var infos = Database.ConquerItemInformation.BaseInformations[723903];
                    ushort X = Owner.X, Y = Owner.Y;
                    Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                    if (Map.SelectCoordonates(ref X, ref Y))
                    {
                        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                        floorItem.Item = new Network.GamePackets.ConquerItem(true);
                        floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        floorItem.Item.ID = (uint)723903;

                        floorItem.Item.MaximDurability = infos.Durability;
                        floorItem.Item.MobDropped = true;
                        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                        floorItem.ItemID = (uint)723903;
                        floorItem.MapID = Owner.MapID;
                        floorItem.MapObjType = Game.MapObjectType.Item;
                        floorItem.X = X;
                        floorItem.Y = Y;
                        floorItem.Type = Network.GamePackets.FloorItem.Drop;
                        floorItem.OnFloor = Time32.Now;
                        floorItem.ItemColor = floorItem.Item.Color;
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        while (Map.Npcs.ContainsKey(floorItem.UID))
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        Map.AddFloorItem(floorItem);
                        SendScreenSpawn(floorItem);
                    }
                }
            }
            if (ServerBase.Kernel.Rate(40))
            {
                uint amount = (uint)ServerBase.Kernel.Random.Next(500, 1000);
                amount *= ServerBase.Constants.MoneyDropMultiple;



                uint ItemID = Network.PacketHandler.MoneyItemID(amount);
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Money;
                    floorItem.Value = amount;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }
            }
            #region CPs
            /*
            if (Name == "Pheasant" && ServerBase.Kernel.Rate(30))
            {
                killer.ConquerPoints += 10;

                killer.Owner.Send(new Network.GamePackets.Message("You have found 10 Cps!", System.Drawing.Color.Yellow, 2005));
            }
            else if (ServerBase.Kernel.Rate(12))
            {
                killer.ConquerPoints += 50;

                killer.Owner.Send(new Network.GamePackets.Message("You have found 50 Cps!", System.Drawing.Color.Yellow, 2005));
            }
            else if (ServerBase.Kernel.Rate(30))
            {
                uint amount = 20;
                killer.ConquerPoints += 20;
                killer.Owner.Send(new Network.GamePackets.Message("The monster has runned out and let " + amount + " cps.", System.Drawing.Color.Yellow, 2005));
            }
         
            else  if (Name == "Pheasant" && ServerBase.Kernel.Rate(0.001))
            {
                Random R = new Random();
                int Nr = R.Next(1, 5);
                if (Nr == 1)
                {
                    killer.ConquerPoints += 1000;
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + " Has Killed  " + Owner.Name + "  and found Lv4 (Magic Cps) 1000 cps in hunting monster!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
                    //killer.Owner.Send(new Network.GamePackets.Message("You have found (Magic Cps) 5000!", System.Drawing.Color.Yellow, 2005));
                }
                if (Nr == 2)
                {
                    killer.ConquerPoints += 800;
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + " Has Killed  " + Owner.Name + "  and found Lv3 (Magic Cps) 800 cps in hunting monster!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
                    //killer.Owner.Send(new Network.GamePackets.Message("You have found (Magic Cps) 5000!", System.Drawing.Color.Yellow, 2005));
                }
                if (Nr == 3)
                {
                    killer.ConquerPoints += 500;
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + " Has Killed  " + Owner.Name + "  and found Lv2 (Magic Cps) 800 cps in hunting monster!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
                    //killer.Owner.Send(new Network.GamePackets.Message("You have found (Magic Cps) 5000!", System.Drawing.Color.Yellow, 2005));
                }
                if (Nr == 4)
                {
                    killer.ConquerPoints += 100;
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + " Has Killed  " + Owner.Name + "  and found Lv1 (Magic Cps) 100 cps in hunting monster!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
                    //killer.Owner.Send(new Network.GamePackets.Message("You have found (Magic Cps) 5000!", System.Drawing.Color.Yellow, 2005));
                }
            }
             */
            #endregion
            /*
            ushort X2 = Owner.X, Y2 = Owner.Y;
            Game.Map Map2 = ServerBase.Kernel.Maps[Owner.MapID];
              if (ServerBase.Kernel.Rate(8))
            {
                if (Map2.SelectCoordonates(ref X2, ref Y2))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = 1000;
                    floorItem.ItemID = 720694;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X2;
                    floorItem.Y = Y2;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map2.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map2.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                    ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + "Has Killed " + Owner.Name + "and found 4000 cps in hunting monster!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
                }
            }
          else if (ServerBase.Kernel.Rate(3))
          {
              if (Map2.SelectCoordonates(ref X2, ref Y2))
              {
                  Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                  floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                  floorItem.Value = 13500;
                  floorItem.ItemID = 720678;
                  floorItem.MapID = Owner.MapID;
                  floorItem.MapObjType = Game.MapObjectType.Item;
                  floorItem.X = X2;
                  floorItem.Y = Y2;
                  floorItem.Type = Network.GamePackets.FloorItem.Drop;
                  floorItem.OnFloor = Time32.Now;
                  floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                  while (Map2.Npcs.ContainsKey(floorItem.UID))
                      floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                  Map2.AddFloorItem(floorItem);
                  SendScreenSpawn(floorItem);
                  ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + "Has Killed " + Owner.Name + "and found 13500 cps in hunting monster!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
              }
          }
          else if (ServerBase.Kernel.Rate(0.01))
          {
              if (Map2.SelectCoordonates(ref X2, ref Y2))
              {
                  Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                  floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                  floorItem.Value = 27000;
                  floorItem.ItemID = 720684;
                  floorItem.MapID = Owner.MapID;
                  floorItem.MapObjType = Game.MapObjectType.Item;
                  floorItem.X = X2;
                  floorItem.Y = Y2;
                  floorItem.Type = Network.GamePackets.FloorItem.Drop;
                  floorItem.OnFloor = Time32.Now;
                  floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                  while (Map2.Npcs.ContainsKey(floorItem.UID))
                      floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                  Map2.AddFloorItem(floorItem);
                  SendScreenSpawn(floorItem);
                  ServerBase.Kernel.SendWorldMessage(new Network.GamePackets.Message("  " + killer.Name + "Has Killed " + Owner.Name + "and found 27000 cps in hunting monster!", System.Drawing.Color.Yellow, 2011), ServerBase.Kernel.GamePool.Values);
              }
          }
             */
            byte morepercent = 0;
            byte morepercent2 = NewMethod();
            if (SuperBoss)
                morepercent2 = 30;
            byte lessrate = 0;
            if (killer.VIPLevel > 0)
                morepercent = (byte)(killer.VIPLevel * 5);
            if (killer.Level <= 10 && killer.MapID == 1002)
                morepercent += 100;
            if (killer.VIPLevel != 6 && killer.Class >= 40 && killer.Class <= 45)
                lessrate = 3;
            if (killer.VIPLevel != 6 && killer.Level >= 132 && killer.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                lessrate = 3;

            if (ServerBase.Kernel.Rate(ServerBase.Constants.MoneyDropRate - lessrate + morepercent))
            {
                uint amount = (uint)ServerBase.Kernel.Random.Next((int)MinMoneyDropAmount, (int)MaxMoneyDropAmount);
                amount *= ServerBase.Constants.MoneyDropMultiple;

                if (amount > 300000)
                    amount = 10;

                if (amount == 0)
                    return;
                if (killer.VIPLevel > 0)
                {
                    int percent = 10;
                    percent += killer.VIPLevel * 5 - 5;
                    amount += (uint)(amount * percent / 100);
                }
                if (killer.VIPLevel > 4)
                {
                    killer.Money += amount;
                    return;
                }
                uint ItemID = Network.PacketHandler.MoneyItemID(amount);
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Money;
                    floorItem.Value = amount;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }

            }
            if (ServerBase.Kernel.Rate(ServerBase.Constants.ConquerPointsDropRate - lessrate))
            {
                // uint amount = (uint)ServerBase.Kernel.Random.Next((int)((Level / 4) * ServerBase.Constants.ConquerPointsDropMultiple), (int)((Level / 2) * ServerBase.Constants.ConquerPointsDropMultiple));
                // if (amount == 0)
                //     amount = 2;
                // if (amount > 300)
                //      amount = 10;
                //  amount /= 2;

                uint amount = (uint)Level / 8;
                if (amount < 1)
                    amount = 1;
                // if (killer.VIPLevel > 4)
                // {

                //  }

                #region CPBag

                // uint ItemID = 729911;
                // ushort X = Owner.X, Y = Owner.Y;
                //  Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                // if (Map.SelectCoordonates(ref X, ref Y))
                // {
                //  Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                //   floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                //  floorItem.Value = amount;
                ///  floorItem.ItemID = ItemID;
                ///  floorItem.MapID = Owner.MapID;
                //  floorItem.MapObjType = Game.MapObjectType.Item;
                // floorItem.X = X;
                //  floorItem.Y = Y;
                // floorItem.Type = Network.GamePackets.FloorItem.Drop;
                ///  floorItem.OnFloor = Time32.Now;
                // floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                // while (Map.Npcs.ContainsKey(floorItem.UID))
                //    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                //  Map.AddFloorItem(floorItem);
                // SendScreenSpawn(floorItem);
                //}
                #endregion
            }
            if (ServerBase.Kernel.Rate(OwnItemRate + morepercent) && OwnItemID != 0)
            {
                if (killer.VIPLevel > 4)
                {
                    if (killer.Owner.Inventory.Count <= 39)
                    {
                        killer.Owner.Inventory.Add((uint)OwnItemID, 0, 1);
                        return;
                    }
                }
                var infos = Database.ConquerItemInformation.BaseInformations[(uint)OwnItemID];
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.Item = new Network.GamePackets.ConquerItem(true);
                    floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                    floorItem.Item.ID = (uint)OwnItemID;
                    floorItem.Item.MaximDurability = infos.Durability;
                    if (!Network.PacketHandler.IsEquipment(OwnItemID) && infos.ConquerPointsWorth == 0)
                    {
                        floorItem.Item.StackSize = 1;
                        floorItem.Item.MaxStackSize = infos.StackSize;
                    }
                    floorItem.Item.MobDropped = true;
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                    floorItem.ItemID = (uint)OwnItemID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.ItemColor = floorItem.Item.Color;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }
            }
            //if (ServerBase.Kernel.Rate(ServerBase.Constants.ItemDropRate + morepercent + morepercent2))
            //{
            //    if (Name == "TeratoDragon" || Name == "ThrillingSpook" || Name == "SwordMaster" || Name == "SnowBanshee")
            //    {
            //        return;
            //    }
            //    int quality = 3;
            //    for (int count = 0; count < 5; count++)
            //    {
            //        int rate = int.Parse(ServerBase.Constants.ItemDropQualityRates[count]);
            //        if (ServerBase.Kernel.Rate(rate, 1000))
            //        {
            //            quality = count + 5;
            //            break;
            //        }
            //    }
            //    int times = 50;
            //    byte lvl = Owner.Level;
            //    if (LabirinthDrop)
            //        lvl = 20;
            //    List<uint> itemdroplist = ItemDropCache[lvl];
            //    if (Boss || SuperBoss)
            //        itemdroplist = SoulItemCache[lvl];
            //retry:
            //    times--;
            //    int generateItemId = ServerBase.Kernel.Random.Next(itemdroplist.Count);
            //    uint id = itemdroplist[generateItemId];
            //    if (!Boss)
            //    {
            //        if (Database.ConquerItemInformation.BaseInformations[id].Level > 121 && times > 0)
            //            goto retry;
            //        id = (id / 10) * 10 + (uint)quality;
            //    }
            //    if (!Database.ConquerItemInformation.BaseInformations.ContainsKey(id))
            //    {
            //        id = itemdroplist[generateItemId];
            //    }
            //    if (killer.VIPLevel > 4)
            //    {
            //        if (killer.Owner.Inventory.Count <= 39)
            //        {
            //            if (id % 10 > 7)
            //            {
            //                killer.Owner.Inventory.Add(id, 0, 1);
            //                return;
            //            }
            //        }
            //    }
            //    var infos = Database.ConquerItemInformation.BaseInformations[id];
            //    ushort X = Owner.X, Y = Owner.Y;
            //    Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
            //    if (Map.SelectCoordonates(ref X, ref Y))
            //    {
            //        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
            //        floorItem.Item = new Network.GamePackets.ConquerItem(true);
            //        floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
            //        floorItem.Item.ID = id;
            //        floorItem.Item.MaximDurability = infos.Durability;
            //        if (quality >= 6)
            //            floorItem.Item.Durability = (ushort)(infos.Durability - ServerBase.Kernel.Random.Next(500));
            //        else
            //            floorItem.Item.Durability = (ushort)(ServerBase.Kernel.Random.Next(infos.Durability / 10));
            //        if (!Network.PacketHandler.IsEquipment(id) && infos.ConquerPointsWorth == 0)
            //        {
            //            floorItem.Item.StackSize = 1;
            //            floorItem.Item.MaxStackSize = infos.StackSize;
            //        }
            //        floorItem.Item.MobDropped = true;
            //        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
            //        floorItem.ItemID = id;
            //        floorItem.MapID = Owner.MapID;
            //        floorItem.MapObjType = Game.MapObjectType.Item;
            //        floorItem.X = X;
            //        floorItem.Y = Y;
            //        floorItem.Type = Network.GamePackets.FloorItem.Drop;
            //        floorItem.OnFloor = Time32.Now;
            //        floorItem.ItemColor = floorItem.Item.Color;
            //        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
            //        while (Map.Npcs.ContainsKey(floorItem.UID))
            //            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
            //        Map.AddFloorItem(floorItem);
            //        SendScreenSpawn(floorItem);
            //    }
            //}
            //if (ServerBase.Kernel.Rate(1 + morepercent))
            //{
            //    if (HPPotionID == 0)
            //        return;
            //    var infos = Database.ConquerItemInformation.BaseInformations[(uint)HPPotionID];
            //    ushort X = Owner.X, Y = Owner.Y;
            //    Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
            //    if (Map.SelectCoordonates(ref X, ref Y))
            //    {
            //        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
            //        floorItem.Item = new Network.GamePackets.ConquerItem(true);
            //        floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
            //        floorItem.Item.ID = (uint)HPPotionID;
            //        floorItem.Item.MobDropped = true;
            //        floorItem.Item.MaximDurability = infos.Durability;
            //        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
            //        if (!Network.PacketHandler.IsEquipment(HPPotionID))
            //        {
            //            floorItem.Item.StackSize = 1;
            //            floorItem.Item.MaxStackSize = infos.StackSize;
            //        }
            //        floorItem.ItemID = (uint)HPPotionID;
            //        floorItem.MapID = Owner.MapID;
            //        floorItem.MapObjType = Game.MapObjectType.Item;
            //        floorItem.X = X;
            //        floorItem.Y = Y;
            //        floorItem.Type = Network.GamePackets.FloorItem.Drop;
            //        floorItem.OnFloor = Time32.Now;
            //        floorItem.ItemColor = floorItem.Item.Color;
            //        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
            //        while (Map.Npcs.ContainsKey(floorItem.UID))
            //            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
            //        Map.AddFloorItem(floorItem);
            //        SendScreenSpawn(floorItem);
            //    }
            //}
            //if (ServerBase.Kernel.Rate(1 + morepercent))
            //{
            //    if (MPPotionID == 0)
            //        return;
            //    var infos = Database.ConquerItemInformation.BaseInformations[(uint)MPPotionID];
            //    ushort X = Owner.X, Y = Owner.Y;
            //    Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
            //    if (Map.SelectCoordonates(ref X, ref Y))
            //    {
            //        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
            //        floorItem.Item = new Network.GamePackets.ConquerItem(true);
            //        floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
            //        floorItem.Item.ID = (uint)MPPotionID;
            //        floorItem.Item.MaximDurability = infos.Durability;
            //        floorItem.Item.MobDropped = true;
            //        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
            //        if (!Network.PacketHandler.IsEquipment(MPPotionID))
            //        {
            //            floorItem.Item.StackSize = 1;
            //            floorItem.Item.MaxStackSize = infos.StackSize;
            //        }
            //        floorItem.ItemID = (uint)MPPotionID;
            //        floorItem.MapID = Owner.MapID;
            //        floorItem.MapObjType = Game.MapObjectType.Item;
            //        floorItem.X = X;
            //        floorItem.Y = Y;
            //        floorItem.Type = Network.GamePackets.FloorItem.Drop;
            //        floorItem.OnFloor = Time32.Now;
            //        floorItem.ItemColor = floorItem.Item.Color;
            //        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
            //        while (Map.Npcs.ContainsKey(floorItem.UID))
            //            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
            //        Map.AddFloorItem(floorItem);
            //        SendScreenSpawn(floorItem);
            //    }
            //}

            foreach (SpecialItemDrop sitem in SpecialItemDropList)
            {
                if (sitem.Map != 0)
                    if (Owner.MapID != sitem.Map)
                        continue;
                if (ServerBase.Kernel.Rate(sitem.Rate + morepercent, sitem.Discriminant))
                {
                    if (killer.VIPLevel > 4)
                    {
                        if (killer.Owner.Inventory.Count <= 39)
                        {
                            killer.Owner.Inventory.Add((uint)sitem.ItemID, 0, 1);
                            continue;
                        }
                    }
                    var infos = Database.ConquerItemInformation.BaseInformations[(uint)sitem.ItemID];
                    ushort X = Owner.X, Y = Owner.Y;
                    Game.Map Map = ServerBase.Kernel.Maps[Owner.MapID];
                    if (Map.SelectCoordonates(ref X, ref Y))
                    {
                        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                        floorItem.Item = new Network.GamePackets.ConquerItem(true);
                        floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                        floorItem.Item.ID = (uint)sitem.ItemID;
                        floorItem.Item.MaximDurability = infos.Durability;
                        floorItem.Item.MobDropped = true;
                        if (!Network.PacketHandler.IsEquipment(sitem.ItemID) && infos.ConquerPointsWorth == 0)
                        {
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = infos.StackSize;
                        }
                        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                        floorItem.ItemID = (uint)sitem.ItemID;
                        floorItem.MapID = Owner.MapID;
                        floorItem.MapObjType = Game.MapObjectType.Item;
                        floorItem.X = X;
                        floorItem.Y = Y;
                        floorItem.Type = Network.GamePackets.FloorItem.Drop;
                        floorItem.OnFloor = Time32.Now;
                        floorItem.ItemColor = floorItem.Item.Color;
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        while (Map.Npcs.ContainsKey(floorItem.UID))
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        Map.AddFloorItem(floorItem);
                        SendScreenSpawn(floorItem);
                    }
                }
            }
        }

        private static byte NewMethod()
        {
            byte morepercent2 = 0;
            return morepercent2;
        }

        public static SafeDictionary<uint, MonsterInformation> MonsterInfos = new SafeDictionary<uint, MonsterInformation>(8000);

        public static void Load()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("specialdrops");
            MySqlReader rdr = new MySqlReader(cmd);
            while (rdr.Read())
            {
                SpecialItemDrop sitem = new SpecialItemDrop();
                sitem.ItemID = rdr.ReadInt32("itemid");
                sitem.Rate = rdr.ReadInt32("rate");
                sitem.Discriminant = rdr.ReadInt32("discriminant");
                sitem.Map = rdr.ReadInt32("map");
                SpecialItemDropList.Add(sitem);
            }
            rdr.Close();
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("monsterinfos");
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                MonsterInformation mf = new MonsterInformation();
                mf.ID = reader.ReadUInt32("id");
                mf.Name = reader.ReadString("name");
                mf.Mesh = reader.ReadUInt16("lookface");
                mf.Level = reader.ReadByte("level");
                mf.Hitpoints = reader.ReadUInt32("life");
                ServerBase.IniFile IniFile = new ServerBase.IniFile(ServerBase.Constants.MonstersPath);
                if (IniFile.ReadString(mf.Name, "MaxLife") != "")
                {
                    if (uint.Parse(IniFile.ReadString(mf.Name, "MaxLife")) != 0)
                    {
                        mf.Hitpoints = uint.Parse(IniFile.ReadString(mf.Name, "MaxLife"));
                        byte boss = byte.Parse(IniFile.ReadString(mf.Name, "Boss"));
                        if (boss == 0)
                            mf.Boss = false;
                        else mf.Boss = true;
                        if (mf.Name == "TeratoDragon" || mf.Name == "SnowBanshee")
                        {
                            mf.SuperBoss = true;
                        }

                    }
                }
                mf.ViewRange = reader.ReadUInt16("view_range");
                mf.AttackRange = reader.ReadUInt16("attack_range");
                mf.AttackType = reader.ReadByte("attack_user");
                mf.MinAttack = reader.ReadUInt32("attack_min");
                mf.MaxAttack = reader.ReadUInt32("attack_max");


                mf.SpellID = reader.ReadUInt16("magic_type");
                mf.MoveSpeed = reader.ReadInt32("move_speed");
                mf.RunSpeed = reader.ReadInt32("run_speed");
                mf.OwnItemID = reader.ReadInt32("ownitem");
                mf.HPPotionID = reader.ReadInt32("drop_hp");
                mf.MPPotionID = reader.ReadInt32("drop_mp");
                mf.OwnItemRate = reader.ReadInt32("ownitemrate");
                mf.AttackSpeed = reader.ReadInt32("attack_speed");
                mf.ExtraExperience = reader.ReadUInt32("extra_exp");
                #region TeratoDragon
                if (mf.Name == "TeratoDragon")
                {
                    byte times = (byte)ServerBase.Kernel.Random.Next(1, 4);
                    byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                    for (byte i = 0; i < times; i++)
                    {
                        uint Uid = 0;//  
                        uint type = (byte)ServerBase.Kernel.Random.Next(1, 4);
                        switch (type)
                        {
                            case 1: Uid = 7013; break;
                            case 2: Uid = 7014; break;
                            case 3: Uid = 7017; break;
                            case 4: Uid = 10361; break;
                        }
                        if (Uid != 0)
                        {
                            mf.SpellID = (ushort)Uid;
                        }
                    }
                }
                #endregion
                #region Banshee
                if (mf.Name == "SnowBanshee")
                {
                    byte times = (byte)ServerBase.Kernel.Random.Next(1, 3);
                    byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                    for (byte i = 0; i < times; i++)
                    {
                        uint Uid = 0;//   
                        uint type = (byte)ServerBase.Kernel.Random.Next(1, 3);
                        switch (type)
                        {
                            case 1: Uid = 30010; break;
                            case 2: Uid = 30011; break;
                            case 3: Uid = 30012; break;
                        }

                        if (Uid != 0)
                        {
                            mf.SpellID = (ushort)Uid;
                        }
                    }
                }
                #endregion
                #region ThrillingSpook
                if (mf.Name == "ThrillingSpook")
                {
                    byte times = (byte)ServerBase.Kernel.Random.Next(1, 4);
                    byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                    for (byte i = 0; i < times; i++)
                    {
                        uint Uid = 0;//  
                        uint type = (byte)ServerBase.Kernel.Random.Next(1, 4);
                        switch (type)
                        {
                            case 1: Uid = 10363; break;
                            case 2: Uid = 10362; break;
                            case 3: Uid = 10360; break;
                            case 4: Uid = 10361; break;
                        }
                        if (Uid != 0)
                        {
                            mf.SpellID = (ushort)Uid;
                        }
                    }
                }
                #endregion
                #region SwordMaster
                if (mf.Name == "SwordMaster")
                {
                    byte times = (byte)ServerBase.Kernel.Random.Next(1, 4);
                    byte ref_times = (byte)ServerBase.Kernel.Random.Next(1, 6);
                    for (byte i = 0; i < times; i++)
                    {
                        uint Uid = 0;//  
                        uint type = (byte)ServerBase.Kernel.Random.Next(1, 4);
                        switch (type)
                        {
                            case 1: Uid = 10504; break;
                            case 2: Uid = 10506; break;
                            case 3: Uid = 10502; break;
                            case 4: Uid = 10505; break;
                        }
                        if (Uid != 0)
                        {
                            mf.SpellID = (ushort)Uid;
                        }
                    }
                }
                #endregion
                //
                uint MoneyDropAmount = reader.ReadUInt16("level");
                if (MoneyDropAmount != 0)
                {
                    mf.MaxMoneyDropAmount = MoneyDropAmount * 25;
                    if (mf.MaxMoneyDropAmount != 0)
                        mf.MinMoneyDropAmount = 1;
                }
                if (mf.MoveSpeed <= 500)
                    mf.MoveSpeed += 500;
                if (mf.AttackSpeed <= 500)
                    mf.AttackSpeed += 500;
                MonsterInfos.Add(mf.ID, mf);
                byte lvl = mf.Level;
                if (mf.Name == "Slinger" ||
                    mf.Name == "GoldGhost" ||
                    mf.Name == "AgileRat" ||
                    mf.Name == "Bladeling" ||
                    mf.Name == "BlueBird" ||
                    mf.Name == "BlueFiend" ||
                    mf.Name == "MinotaurL120")
                {
                    mf.LabirinthDrop = true;
                    lvl = 20;
                }

                if (!ItemDropCache.ContainsKey(lvl))
                {
                    List<uint> itemdroplist = new List<uint>();
                    foreach (ConquerItemBaseInformation itemInfo in ConquerItemInformation.BaseInformations.Values)
                    {

                        if (itemInfo.ID >= 800000 && itemInfo.ID <= 824014)
                            continue;
                        ushort position = Network.PacketHandler.ItemPosition(itemInfo.ID);
                        if (Network.PacketHandler.IsArrow(itemInfo.ID) || itemInfo.Level == 0 || itemInfo.Level > 121)
                            continue;
                        if (position < 9 && position != 7)
                        {
                            if (itemInfo.Level == 100)
                                if (itemInfo.Name.Contains("Dress"))
                                    continue;
                            if (itemInfo.Level > 121)
                                continue;
                            int diff = (int)lvl - (int)itemInfo.Level;
                            if (!(diff > 10 || diff < -10))
                            {
                                itemdroplist.Add(itemInfo.ID);
                            }
                        }
                        if (position == 10 || position == 11 && lvl >= 70)
                            itemdroplist.Add(itemInfo.ID);
                    }
                    ItemDropCache.Add(lvl, itemdroplist);
                }
                if (mf.Boss)
                {
                    List<uint> itemdroplist = new List<uint>();
                    foreach (ConquerItemBaseInformation itemInfo in ConquerItemInformation.BaseInformations.Values)
                    {
                        if (itemInfo.ID >= 800000 && itemInfo.ID <= 824014)
                        {
                            if (itemInfo.PurificationLevel <= 3 || (mf.SuperBoss && itemInfo.PurificationLevel > 3))
                            {
                                int diff = (int)mf.Level - (int)itemInfo.Level;
                                if (!(diff > 20 || diff < -20))
                                {
                                    if (itemInfo.Level <= 110)
                                        itemdroplist.Add(itemInfo.ID);
                                }
                            }
                        }
                    }

                    SoulItemCache.Add(lvl, itemdroplist);
                }
            }

            //723755, 723768, 723772  ,723774 
            reader.Close();
            Console.WriteLine("Monster information loaded.");
            Console.WriteLine("Monster drops generated.");
        }

        public MonsterInformation Copy()
        {
            MonsterInformation mf = new MonsterInformation();
            mf.ID = this.ID;
            mf.Name = this.Name;
            mf.Mesh = this.Mesh;
            mf.Level = this.Level;
            mf.Hitpoints = this.Hitpoints;
            mf.ViewRange = this.ViewRange;
            mf.AttackRange = this.AttackRange;
            mf.AttackType = this.AttackType;
            mf.MinAttack = this.MinAttack;
            mf.MaxAttack = this.MaxAttack;
            mf.SpellID = this.SpellID;
            mf.MoveSpeed = this.MoveSpeed;
            mf.RunSpeed = this.RunSpeed;
            mf.AttackSpeed = this.AttackSpeed;
            mf.BoundX = this.BoundX;
            mf.BoundY = this.BoundY;
            mf.BoundCX = this.BoundCX;
            mf.BoundCY = this.BoundCY;
            mf.RespawnTime = this.RespawnTime;
            mf.ExtraExperience = this.ExtraExperience;
            mf.MaxMoneyDropAmount = this.MaxMoneyDropAmount;
            mf.MinMoneyDropAmount = this.MinMoneyDropAmount;
            mf.OwnItemID = this.OwnItemID;
            mf.HPPotionID = this.HPPotionID;
            mf.MPPotionID = this.MPPotionID;
            mf.OwnItemRate = this.OwnItemRate;
            mf.LabirinthDrop = this.LabirinthDrop;
            mf.Boss = this.Boss;
            mf.SuperBoss = this.SuperBoss;
            return mf;
        }
    }
}
     