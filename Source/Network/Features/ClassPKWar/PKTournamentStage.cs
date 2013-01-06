using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Network.Features.ClassPKWar
{
    public enum PKTournamentStage
    {
        None,
        Inviting,
        Countdown,
        Fighting,
        Over
    }
    public enum BroadCastLoc
    {
        World,
        Map
    }
    public static class PKTournament
    {
        public static ushort Map;
        public static ushort X, Y;
        public static PKTournamentStage Stage = PKTournamentStage.None;
        public static Dictionary<uint, Client.GameState> PKTHash;
        public static int CountDown;
        private static Thread PkThread;

        public static void StartTournament(Client.GameState Started)
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1507;
            X = 100;
            Y = 100;


            PkThread = new Thread(new ThreadStart(BeginTournament));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournament()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1507;
            X = 100;
            Y = 100;

            PkThread = new Thread(new ThreadStart(BeginTournament));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournamentTroJan()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1787;
            X = 50;
            Y = 50;

            PkThread = new Thread(new ThreadStart(BeginTrojan));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournamentNinja()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1787;
            X = 50;
            Y = 50;

            PkThread = new Thread(new ThreadStart(BeginNinja));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournamentWarrior()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1787;
            X = 50;
            Y = 50;

            PkThread = new Thread(new ThreadStart(BeginWarrior));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournamentArcher()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1787;
            X = 50;
            Y = 50;

            PkThread = new Thread(new ThreadStart(BeginArcher));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournamentWater()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1787;
            X = 50;
            Y = 50;

            PkThread = new Thread(new ThreadStart(BeginWater));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournamentFire()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1787;
            X = 50;
            Y = 50;

            PkThread = new Thread(new ThreadStart(BeginFire));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournamentMonk()
        {
            PKTHash = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1787;
            X = 50;
            Y = 50;

            PkThread = new Thread(new ThreadStart(BeginMonk));
            PkThread.IsBackground = true;
            PkThread.Start();
        }

        private static void Broadcast(string msg, BroadCastLoc loc)
        {
            //Console.WriteLine(msg);
            if (loc == BroadCastLoc.World)
            {
                foreach (Client.GameState Char in ServerBase.Kernel.GamePool.Values)
                {
                    Char.Send(new Message(msg, System.Drawing.Color.White, 2011));
                    // Char.MyClient.EndSend();
                }
            }
            else if (loc == BroadCastLoc.Map)
            {
                foreach (Client.GameState Char in PKTHash.Values)
                {
                    Char.Send(new Message(msg, System.Drawing.Color.White, 2011));
                    // Char.EndSend();
                }
            }
        }
        private static void AwardWinner(Client.GameState Winner)
        {
            if (Winner.Entity.VIPLevel > 0)
            {

                Broadcast(Winner.Entity.Name + " has won the tournament !He WIn 8.000Cps and WeeklyPKChampion Top cause he VIP", BroadCastLoc.World);
                Winner.Entity.WeeklyPKChampion += 1;
                Winner.Entity.ConquerPoints += 8000;
            }
            else
            {

                Broadcast(Winner.Entity.Name + " has won the tournament !He WIn 4.000Cps and WeeklyPKChampion Top P", BroadCastLoc.World);
                Winner.Entity.WeeklyPKChampion += 1;
                Winner.Entity.ConquerPoints += 4000;
            }
            PKTournament.Stage = PKTournamentStage.None;
            PkThread.Abort();
            return;
        }
        private static void AwardClass(Client.GameState Winner)
        {

            #region Ninja
            if (Winner.Entity.Class >= 50 && Winner.Entity.Class <= 55)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Ninja and Got 8k CPs And TopClassNinja casue he VIp", BroadCastLoc.World);
                    Winner.Entity.TopNinja += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Ninja and Got 4k CPs And TopClassNinja", BroadCastLoc.World);
                    Winner.Entity.TopNinja += 1;
                }

            }
            #endregion
            #region Warrior
            if (Winner.Entity.Class >= 20 && Winner.Entity.Class <= 25)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Worrior and Got 8k CPs And TopClassWorrior casu he VIP", BroadCastLoc.World);
                    Winner.Entity.TopWarrior += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Worrior and Got 4k CPs And TopClassWorrior", BroadCastLoc.World);
                    Winner.Entity.TopWarrior += 1;
                }
            }

            #endregion
            #region Archer
            if (Winner.Entity.Class >= 40 && Winner.Entity.Class <= 45)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Archer and Got 8k CPs And TopClassArcher", BroadCastLoc.World);
                    Winner.Entity.TopArcher += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Archer and Got 4k CPs And TopClassArcher", BroadCastLoc.World);
                    Winner.Entity.TopArcher += 1;
                }

            }
            #endregion
            #region Trojan
            if (Winner.Entity.Class >= 10 && Winner.Entity.Class <= 15)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Trojan and Got 8k CPs And TopClassTrojan cause He VIP", BroadCastLoc.World);
                    Winner.Entity.TopTrojan += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Trojan and Got 4k CPs And TopClassTrojan", BroadCastLoc.World);
                    Winner.Entity.TopTrojan += 1;
                }
            }
            #endregion
            #region Water
            if (Winner.Entity.Class >= 130 && Winner.Entity.Class <= 135)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Water and Got 8k CPs And TopClassWaterToist case he VIP", BroadCastLoc.World);
                    Winner.Entity.TopWaterTaoist += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Water and Got 4k CPs And TopClassWaterToist", BroadCastLoc.World);
                    Winner.Entity.TopWaterTaoist += 1;
                }


            }
            #endregion
            #region Fire
            if (Winner.Entity.Class >= 140 && Winner.Entity.Class <= 145)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Toist and Got 8k CPs And TopClassFireToist case he Vip", BroadCastLoc.World);
                    Winner.Entity.TopFireTaoist += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Toist and Got 4k CPs And TopClassFireToist", BroadCastLoc.World);
                    Winner.Entity.TopFireTaoist += 1;
                }
            }
            #endregion
            #region Monk
            if (Winner.Entity.Class >= 60 && Winner.Entity.Class <= 65)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Monk and Got 8k CPs And TopClassMonk case he VIP", BroadCastLoc.World);
                    Winner.Entity.TopMonk += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins ClassPk War! Monk and Got 4k CPs And TopClassMonk", BroadCastLoc.World);
                    Winner.Entity.TopMonk += 1;
                }

            }
            #endregion
            if (Winner.Entity.VIPLevel > 0)
            {
                Winner.Entity.ConquerPoints += 8000;
            }
            else
            {
                Winner.Entity.ConquerPoints += 4000;
            }
            PKTournament.Stage = PKTournamentStage.None;
            PkThread.Abort();
            return;
        }
        //public static int InMapAlive = PKTHash.Count;
        public static void WaitForClass()
        {//Main.GameClient GC
            //  Main.GameClient GCs = new NewestCOServer.Game.Character();
            PKTournament.Stage = PKTournamentStage.Fighting;
            uint Tick = (uint)Environment.TickCount;
            int InMapAlive = PKTHash.Count;
            while (true)
            {
                int alive = 0;
                foreach (Conquer_Online_Server.Client.GameState players in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                    if (players.Entity.MapID == 1787 && (!players.Entity.Dead))
                        alive++;

                foreach (Client.GameState _GC in PKTHash.Values)
                {
                    if (_GC.InPKT)
                        if (_GC.Entity.Dead == true)
                        {
                            InMapAlive--;
                            _GC.Entity.Teleport(1002, 438, 382);
                            _GC.InPKT = false;
                        }
                    //else if (!ServerBase.Kernel.GamePool.ContainsKey(_GC.Entity.UID))
                    //{
                    //    InMapAlive--;
                    //    _GC.Entity.Teleport(1002, 438, 382);
                    //    _GC.InPKT = false;
                    //}
                }
                System.Threading.Thread.Sleep(2000);
                foreach (Client.GameState _GC in PKTHash.Values)
                {
                    if (_GC.InPKT)
                        if (alive == 1)
                        {
                            _GC.Entity.Teleport(1002, 438, 382);
                            AwardClass(_GC);
                            ServerBase.Kernel.PK = false;
                            Stage = PKTournamentStage.Over;
                            return;
                        }
                }
                if (InMapAlive != 1)
                {
                    Broadcast("There are " + InMapAlive + " Alive ", BroadCastLoc.Map);

                }
                Thread.Sleep(1000);
            }
        }
        public static void WaitForWinner()
        {//Main.GameClient GC
            //  Main.GameClient GCs = new NewestCOServer.Game.Character();
            PKTournament.Stage = PKTournamentStage.Fighting;
            uint Tick = (uint)Environment.TickCount;
            int InMapAlive = PKTHash.Count;
            while (true)
            {
                int alive = 0;
                foreach (Conquer_Online_Server.Client.GameState players in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                    if (players.Entity.MapID == 1507 && (!players.Entity.Dead))
                        alive++;

                foreach (Client.GameState _GC in PKTHash.Values)
                {
                    if (_GC.InPKT)
                        if (_GC.Entity.Dead == true)
                        {
                            InMapAlive--;
                            _GC.Entity.Teleport(1002, 438, 382);
                            _GC.InPKT = false;
                        }
                    //else if (!ServerBase.Kernel.GamePool.ContainsKey(_GC.Entity.UID))
                    //{
                    //    InMapAlive--;
                    //    _GC.Entity.Teleport(1002, 438, 382);
                    //    _GC.InPKT = false;
                    //}
                }
                System.Threading.Thread.Sleep(2000);
                foreach (Client.GameState _GC in PKTHash.Values)
                {
                    if (_GC.InPKT)
                        if (alive == 1)
                        {
                            _GC.Entity.Teleport(1002, 438, 382);
                            AwardWinner(_GC);
                            Stage = PKTournamentStage.Over;
                            ServerBase.Kernel.PK = false;
                            return;
                        }
                }
                if (InMapAlive != 1)
                {
                    Broadcast("There are " + InMapAlive + " Alive ", BroadCastLoc.Map);

                }
                Thread.Sleep(1000);
            }
        }
        public static void BeginNinja()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                if (clientss.Entity.Class >= 50 && clientss.Entity.Class <= 55)
                {

                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Ninja ClassPK Tournament has Started! You Wana Join?");
                    npc.OptionID = 240;
                    clientss.Send(npc.ToArray());
                }
            }

            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 9)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForClass();
        }
        public static void BeginTrojan()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                if (clientss.Entity.Class >= 10 && clientss.Entity.Class <= 15)
                {

                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Trojan ClassPK Tournament has Started! You Wana Join?");
                    npc.OptionID = 241;
                    clientss.Send(npc.ToArray());
                }
            }
            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 9)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForClass();
        }
        public static void BeginWarrior()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                if (clientss.Entity.Class >= 20 && clientss.Entity.Class <= 25)
                {

                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Warrior ClassPK Tournament has Started! You Wana Join?");
                    npc.OptionID = 242;
                    clientss.Send(npc.ToArray());
                }
            }
            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 9)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForClass();
        }
        public static void BeginArcher()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                if (clientss.Entity.Class >= 40 && clientss.Entity.Class <= 45)
                {

                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Archer ClassPK Tournament has Started! You Wana Join?");
                    npc.OptionID = 243;
                    clientss.Send(npc.ToArray());
                }
            }
            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 9)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForClass();
        }
        public static void BeginFire()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                if (clientss.Entity.Class >= 140 && clientss.Entity.Class <= 145)
                {

                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The FireToist ClassPK Tournament has Started! You Wana Join?");
                    npc.OptionID = 244;
                    clientss.Send(npc.ToArray());
                }
            }
            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 9)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForClass();
        }
        public static void BeginWater()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                if (clientss.Entity.Class >= 130 && clientss.Entity.Class <= 135)
                {

                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The WaterToist ClassPK Tournament has Started! You Wana Join?");
                    npc.OptionID = 245;
                    clientss.Send(npc.ToArray());
                }
            }
            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 4)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForClass();
        }
        public static void BeginMonk()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                if (clientss.Entity.Class >= 60 && clientss.Entity.Class <= 65)
                {

                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Monk ClassPK Tournament has Started! You Wana Join?");
                    npc.OptionID = 246;
                    clientss.Send(npc.ToArray());
                }
            }
            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 9)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForClass();
        }
        public static void BeginTournament()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The WeeklyPK Champion Tournament has Started! You Wana Join?");
                npc.OptionID = 248;
                clientss.Send(npc.ToArray());
            }

            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 60)
                    Broadcast("60 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash = null;
                    //    return;
                    //}
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                }
                else if (CountDown < 9)
                    Broadcast(CountDown + " seconds until start", BroadCastLoc.World);

                CountDown--;
                Thread.Sleep(1000);
            }
            Stage = PKTournamentStage.Fighting;
            Broadcast("Fight!", BroadCastLoc.World);
            WaitForWinner();
        }
    }
}
