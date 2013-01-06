using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Conquer_Online_Server.Game.ConquerStructures;
using System.Collections;

namespace Conquer_Online_Server.ServerBase
{
    public class Kernel
    {
        public static uint MaxRoses = 0;
        public static uint MaxLilies = 0;
        public static uint MaxOrchads = 0;
        public static uint MaxTulips = 0;
        public static bool FireExec = false;
        public static bool Terato_open = false;
        public static Dictionary<uint, Conquer_Online_Server.Network.GamePackets.Clan> Clans = new Dictionary<uint, Conquer_Online_Server.Network.GamePackets.Clan>(100000);
        public static Dictionary<uint, Game.Clans> ServerClans = new Dictionary<uint, Conquer_Online_Server.Game.Clans>();
        public static bool Rate(double value)
        {
            return value > Random.Next() % 100;
        }

        public static bool PercentSuccess(double percent)
        {
            return ((double)Rand.Next(1, 1000000)) / 10000 >= 100 - percent;
        }
        public static Hashtable H_Chars = new Hashtable();
        //public static SafeDictionary<uint, Database.AccountTable> AwaitingPool = new SafeDictionary<uint, Database.AccountTable>(50000);

        public static Game.Tournaments.EliteTournament Elite_PK_Tournament;
        public static List<string> VotsAdress = new List<string>();
        public static Dictionary<string, PlayersVot> VotePool = new Dictionary<string, PlayersVot>();
        public static Dictionary<uint, PlayersVot> VotePoolUid = new Dictionary<uint, PlayersVot>();
        public static Network.PacketPool packetPooler = new Network.PacketPool(100);
        public static SafeDictionary<uint, Database.AccountTable> AwaitingPool = new SafeDictionary<uint, Database.AccountTable>(1000);
        public static ThreadSafeDictionary<uint, Client.GameState> GamePool = new ThreadSafeDictionary<uint, Client.GameState>(1000);
        public static Conquer_Online_Server.Game.ConquerStructures.QuizShow.MainInfo MainQuiz = new Conquer_Online_Server.Game.ConquerStructures.QuizShow.MainInfo();
        public static SafeDictionary<uint, Client.GameState> WasInGamePool = new SafeDictionary<uint, Client.GameState>(10000);
        public static SafeDictionary<ushort, Game.Map> Maps = new SafeDictionary<ushort, Game.Map>(280);
        public static SafeDictionary<uint, Game.Struct.Flowers> AllFlower = new SafeDictionary<uint, Game.Struct.Flowers>(1000);
        public static SafeDictionary<uint, Game.ConquerStructures.Society.Guild> Guilds = new SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Guild>(100000);
        public static List<char> InvalidCharacters = new List<char>() { ' ', '[', ']', '#', '*', '\\', '/', '<', '>', ':', '?', '"', '|', '{', '}', '=', '' };
        public static Random Random = new Random();
        public static System.Random Rand = new System.Random();
        public static bool srs = false;
        public static bool PK = false;
        public static bool ls = false;
        public static bool Steed = false;
        public class SteedTornament
        {
            public static int cps = 0;
            public static bool sr = false;

        }
        public class ClassPkWar
        {
            public static bool troWar = false;
            public static bool warWar = false;
            public static bool archerWar = false;
            public static bool ninjaWar = false;
            public static bool waterWar = false;
            public static bool firewarWar = false;
            public static bool Monk = false;
            public static bool Pirate = false;
            public static void RemovePlayersTorament(Client.GameState C)
            {
                if (Kernel.ClassPkWar.troWar == true || Kernel.ClassPkWar.warWar == true
                            || Kernel.ClassPkWar.archerWar == true || Kernel.ClassPkWar.ninjaWar == true
                            || Kernel.ClassPkWar.Pirate == true || Kernel.ClassPkWar.waterWar == true || Kernel.ClassPkWar.firewarWar == true)
                {
                    if (Kernel.ClassPkWar.AddMap.tro100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.tro100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.tro100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.tro100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.tro120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.tro120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.tro130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.tro130.Remove(C.Entity.UID);

                    if (Kernel.ClassPkWar.AddMap.Monk100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Monk100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.Monk100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Monk100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.Monk120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Monk120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.Pirate130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Pirate130.Remove(C.Entity.UID);

                    if (Kernel.ClassPkWar.AddMap.war100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.war100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.war100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.war100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.war120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.war120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.war130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.war130.Remove(C.Entity.UID);

                    if (Kernel.ClassPkWar.AddMap.archer100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.archer100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.archer100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.archer100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.archer120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.archer120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.archer130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.archer130.Remove(C.Entity.UID);

                    if (Kernel.ClassPkWar.AddMap.ninja100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.ninja100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.ninja100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.ninja100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.ninja120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.ninja120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.ninja130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.ninja130.Remove(C.Entity.UID);

                    if (Kernel.ClassPkWar.AddMap.water100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.water100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.water100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.water100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.water120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.water120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.water130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.water130.Remove(C.Entity.UID);

                    if (Kernel.ClassPkWar.AddMap.fire100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.fire100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.fire100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.fire100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.fire120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.fire120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.fire130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.fire130.Remove(C.Entity.UID);

                    if (Kernel.ClassPkWar.AddMap.Pirate100.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Pirate100.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.Pirate100119.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Pirate100119.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.Pirate120130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Pirate120130.Remove(C.Entity.UID);
                    if (Kernel.ClassPkWar.AddMap.Pirate130.Contains(C.Entity.UID))
                        Kernel.ClassPkWar.AddMap.Pirate130.Remove(C.Entity.UID);
                }
            }
            public static void Start()
            {
                Kernel.ClassPkWar.AddMap.timer = false;
                //Kernel.SendWorldMessage(new Message("Class Pk War has Start go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("Class Pk War has Starteh go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //  System.Threading.Thread.Sleep(60000);
                //  Kernel.SendWorldMessage(new Message("Class Pk War has will start in 5 minuts  go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("Class Pk War has will start in 5 minuts go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //  System.Threading.Thread.Sleep(300000);
                // Kernel.SendWorldMessage(new Message("Class Pk War finished", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //Kernel.SendWorldMessage(new Message("Class Pk War will finish", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
            }
            public static void END()
            {
                // Kernel.ClassPkWar.AddMap.timer = false;
                //Kernel.SendWorldMessage(new Message("Class Pk War has Start go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("Class Pk War has Starteh go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //  System.Threading.Thread.Sleep(60000);
                //  Kernel.SendWorldMessage(new Message("Class Pk War has will start in 5 minuts  go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("Class Pk War has will start in 5 minuts go TwinCity at 350 327 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //  System.Threading.Thread.Sleep(300000);
                //  Kernel.SendWorldMessage(new Message("Class Pk War finished", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //Kernel.SendWorldMessage(new Message("Class Pk War will finish", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                Kernel.ClassPkWar.AddMap.timer = true;
                Kernel.ClassPkWar.troWar = false;
                Kernel.ClassPkWar.warWar = false;
                Kernel.ClassPkWar.archerWar = false;
                Kernel.ClassPkWar.firewarWar = false;
                Kernel.ClassPkWar.waterWar = false;
                Kernel.ClassPkWar.ninjaWar = false;
                Kernel.ClassPkWar.Pirate = false;
            }
            public class AddMap
            {
                public static bool timer = false;
                public static Hashtable tro100 = new Hashtable();
                public static Hashtable tro100119 = new Hashtable();
                public static Hashtable tro120130 = new Hashtable();
                public static Hashtable tro130 = new Hashtable();

                public static Hashtable war100 = new Hashtable();
                public static Hashtable war100119 = new Hashtable();
                public static Hashtable war120130 = new Hashtable();
                public static Hashtable war130 = new Hashtable();

                public static Hashtable archer100 = new Hashtable();
                public static Hashtable archer100119 = new Hashtable();
                public static Hashtable archer120130 = new Hashtable();
                public static Hashtable archer130 = new Hashtable();

                public static Hashtable ninja100 = new Hashtable();
                public static Hashtable ninja100119 = new Hashtable();
                public static Hashtable ninja120130 = new Hashtable();
                public static Hashtable ninja130 = new Hashtable();

                public static Hashtable water100 = new Hashtable();
                public static Hashtable water100119 = new Hashtable();
                public static Hashtable water120130 = new Hashtable();
                public static Hashtable water130 = new Hashtable();

                public static Hashtable fire100 = new Hashtable();
                public static Hashtable fire100119 = new Hashtable();
                public static Hashtable fire120130 = new Hashtable();
                public static Hashtable fire130 = new Hashtable();

                public static Hashtable Monk100 = new Hashtable();
                public static Hashtable Monk100119 = new Hashtable();
                public static Hashtable Monk120130 = new Hashtable();
                public static Hashtable Monk130 = new Hashtable();

                public static Hashtable Pirate100 = new Hashtable();
                public static Hashtable Pirate100119 = new Hashtable();
                public static Hashtable Pirate120130 = new Hashtable();
                public static Hashtable Pirate130 = new Hashtable();

            }
        }
        public class WeeklyPkWar
        {
            public static bool PkWar = false;

            public static void RemovePlayersTorament(Client.GameState C)
            {
                if (Kernel.WeeklyPkWar.PkWar == true)
                {
                    if (Kernel.WeeklyPkWar.AddMap.pk100.Contains(C.Entity.UID))
                        Kernel.WeeklyPkWar.AddMap.pk100.Remove(C.Entity.UID);
                    if (Kernel.WeeklyPkWar.AddMap.pk100119.Contains(C.Entity.UID))
                        Kernel.WeeklyPkWar.AddMap.pk100119.Remove(C.Entity.UID);
                    if (Kernel.WeeklyPkWar.AddMap.pk120130.Contains(C.Entity.UID))
                        Kernel.WeeklyPkWar.AddMap.pk120130.Remove(C.Entity.UID);
                    if (Kernel.WeeklyPkWar.AddMap.pk130.Contains(C.Entity.UID))
                        Kernel.WeeklyPkWar.AddMap.pk130.Remove(C.Entity.UID);

                }
            }
            public static void Start()
            {
                Kernel.WeeklyPkWar.AddMap.timer = false;
                //  Kernel.SendWorldMessage(new Message("WeeklyPkWar has Starteh go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //Kernel.SendWorldMessage(new Message("WeeklyPkWar has Starteh go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //System.Threading.Thread.Sleep(60000);
                // Kernel.SendWorldMessage(new Message("WeeklyPkWar War has will start in 5 minuts  go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("WeeklyPkWar War has will start in 5 minuts go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //  System.Threading.Thread.Sleep(30000);
                //  Kernel.SendWorldMessage(new Message("WeeklyPkWar War will finish", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //Kernel.SendWorldMessage(new Message("WeeklyPkWar War will finish", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.WeeklyPkWar.AddMap.timer = true;
                // Kernel.WeeklyPkWar.PkWar = false;

            }
            public static void end()
            {
                // Kernel.WeeklyPkWar.AddMap.timer = false;
                //  Kernel.SendWorldMessage(new Message("WeeklyPkWar has Starteh go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //Kernel.SendWorldMessage(new Message("WeeklyPkWar has Starteh go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // System.Threading.Thread.Sleep(60000);
                // Kernel.SendWorldMessage(new Message("WeeklyPkWar War has will start in 5 minuts  go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("WeeklyPkWar War has will start in 5 minuts go TwinCity at 440 383 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //  System.Threading.Thread.Sleep(30000);
                //  Kernel.SendWorldMessage(new Message("WeeklyPkWar War will finish", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //Kernel.SendWorldMessage(new Message("WeeklyPkWar War will finish", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                Kernel.WeeklyPkWar.AddMap.timer = true;
                Kernel.WeeklyPkWar.PkWar = false;

            }
            public class AddMap
            {
                public static bool timer = false;
                public static Hashtable pk100 = new Hashtable();
                public static Hashtable pk100119 = new Hashtable();
                public static Hashtable pk120130 = new Hashtable();
                public static Hashtable pk130 = new Hashtable();



            }
        }
        public class ElitePk
        {
            public static bool ElWar = false;

            public static void RemovePlayersTorament(Client.GameState C)
            {
                if (Kernel.ElitePk.ElWar == true)
                {
                    if (Kernel.ElitePk.AddMap.pk100.Contains(C.Entity.UID))
                        Kernel.ElitePk.AddMap.pk100.Remove(C.Entity.UID);
                    if (Kernel.ElitePk.AddMap.pk100119.Contains(C.Entity.UID))
                        Kernel.ElitePk.AddMap.pk100119.Remove(C.Entity.UID);
                    if (Kernel.ElitePk.AddMap.pk120130.Contains(C.Entity.UID))
                        Kernel.ElitePk.AddMap.pk120130.Remove(C.Entity.UID);
                    if (Kernel.ElitePk.AddMap.pk130.Contains(C.Entity.UID))
                        Kernel.ElitePk.AddMap.pk130.Remove(C.Entity.UID);

                }
            }
            public static void Start()
            {
                Kernel.ElitePk.AddMap.timer = false;
                //Kernel.SendWorldMessage(new Message("ElitePk has Starteh go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("ElitePk has Starteh go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // System.Threading.Thread.Sleep(60000);
                // Kernel.SendWorldMessage(new Message("ElitePk War has will start in 5 minuts  go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("ElitePk War has will start in 5 minuts go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // System.Threading.Thread.Sleep(300000);
                //  Kernel.SendWorldMessage(new Message("ElitePk War will finish", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("ElitePk War will finish", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                Kernel.ElitePk.AddMap.timer = true;
                Kernel.ElitePk.ElWar = false;

            }
            public static void end()
            {
                //Kernel.SendWorldMessage(new Message("ElitePk has Starteh go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("ElitePk has Starteh go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                //  System.Threading.Thread.Sleep(60000);
                //  Kernel.SendWorldMessage(new Message("ElitePk War has will start in 5 minuts  go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("ElitePk War has will start in 5 minuts go TwinCity at 416 289 ,for Join.", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // System.Threading.Thread.Sleep(300000);
                // Kernel.SendWorldMessage(new Message("ElitePk War will finish", System.Drawing.Color.Black, Message.Center), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                // Kernel.SendWorldMessage(new Message("ElitePk War will finish", System.Drawing.Color.Black, Message.BroadcastMessage), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                Kernel.ElitePk.AddMap.timer = true;
                Kernel.ElitePk.ElWar = false;

            }
            public class AddMap
            {
                public static bool timer = false;
                public static Hashtable pk100 = new Hashtable();
                public static Hashtable pk100119 = new Hashtable();
                public static Hashtable pk120130 = new Hashtable();
                public static Hashtable pk130 = new Hashtable();



            }
        }
        public static int boundID = 45;
        public static int boundIDEnd = 46;
        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            return (short)Math.Sqrt((X - X2) * (X - X2) + (Y - Y2) * (Y - Y2));
        }
        public static bool ChanceSuccess(double Chance)
        {
            int e = Random.Next(10000000);
            double a = ((double)e / (double)10000000) * 100;
            return Chance >= a;
        }
        public static int GetDegree(int X, int X2, int Y, int Y2)
        {
            int direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);
            if (r < 0) r += (double)Math.PI * 2;

            direction = (int)(360 - (r * 180 / Math.PI));

            return direction;
        }
        public static Game.Enums.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);

            byte Dir = (byte)((7 - (Math.Floor(direction) / 45 % 8)) - 1 % 8);
            return (Game.Enums.ConquerAngle)(byte)((int)Dir % 8);
        }

        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameState[] to)
        {
            foreach (Client.GameState client in to)
            {
                if (client != null)
                {
                    client.Send(message);
                }
            }
        }

        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameState[] to, uint exceptuid)
        {
            foreach (Client.GameState client in to)
            {
                if (client != null)
                {
                    if (client.Entity.UID != exceptuid)
                    {
                        client.Send(message);
                    }
                }
            }
        }

        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameState[] to, ushort mapid)
        {
            foreach (Client.GameState client in to)
            {
                if (client != null)
                {
                    if (client.Map.ID == mapid)
                    {
                        client.Send(message);
                    }
                }
            }
        }

        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameState[] to, ushort mapid, uint exceptuid)
        {
            foreach (Client.GameState client in to)
            {
                if (client != null)
                {
                    if (client.Map.ID == mapid)
                    {
                        if (client.Entity.UID != exceptuid)
                        {
                            client.Send(message);
                        }
                    }
                }
            }
        }

        public static uint maxJumpTime(short distance)
        {
            uint x = 0;
            x = 400 * (uint)distance / 10;
            return x;
        }
        public static bool Rate(int value)
        {
            return value > Random.Next() % 100;
        }
        public static bool Rate(int value, int discriminant)
        {
            return value > Random.Next() % discriminant;
        }
        public static bool Rate(ulong value)
        {
            return Rate((int)value);
        }
    }
}
