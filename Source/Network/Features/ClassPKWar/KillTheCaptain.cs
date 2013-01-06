using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.ServerBase;

namespace Conquer_Online_Server.Network.Features.ClassPKWar
{

    public enum PKTournamentStage2
    {
        None,
        Inviting,
        Countdown,
        Fighting,
        Over
    }
    public enum BroadCastLoc2
    {
        World,
        Map
    }
    public static class KillTheCaptain
    {
        public static ushort Map;
        public static ushort X, Y;
        public static PKTournamentStage Stage = PKTournamentStage.None;
        public static Dictionary<uint, Client.GameState> PKTHash2;
        public static int CountDown;
        private static Thread PkThread;

        public static void StartTournament(Client.GameState Started)
        {
            PKTHash2 = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1858;
            X = 70;
            Y = 70;


            PkThread = new Thread(new ThreadStart(BeginTournament));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        public static void StartTournament()
        {
            PKTHash2 = new Dictionary<uint, Client.GameState>();
            CountDown = 320;
            Stage = PKTournamentStage.Inviting;
            Map = 1858;
            X = 70;
            Y = 70;

            PkThread = new Thread(new ThreadStart(BeginTournament));
            PkThread.IsBackground = true;
            PkThread.Start();
        }
        private static void Broadcast(string msg, BroadCastLoc loc)
        {
            if (loc == BroadCastLoc.World)
            {
                foreach (Client.GameState Char in ServerBase.Kernel.GamePool.Values)
                {
                    Char.Send(new Message(msg, System.Drawing.Color.White, 2011));
                }
            }
            else if (loc == BroadCastLoc.Map)
            {
                foreach (Client.GameState Char in PKTHash2.Values)
                {
                    Char.Send(new Message(msg, System.Drawing.Color.White, 2011));

                }
            }
        }
        private static void AwardClass(Client.GameState Winner)
        {

            #region Ninja
            if (Winner.Entity.Class >= 50 && Winner.Entity.Class <= 55)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Winner.Entity.TopNinja2 += 1;
                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 4k CPs And TopClassNinja2 cause he VIP", BroadCastLoc.World);
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 2k CPs And TopClassNinja2", BroadCastLoc.World);
                    Winner.Entity.TopNinja2 += 1;
                }
            }
            #endregion
            #region Warrior
            if (Winner.Entity.Class >= 20 && Winner.Entity.Class <= 25)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding and Got 4K CPs And TopClassWorrior2 cause he VIP", BroadCastLoc.World);
                    Winner.Entity.TopWarrior2 += 1;
                }
                else
                {
                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding and Got 2K CPs And TopClassWorrior2", BroadCastLoc.World);
                    Winner.Entity.TopWarrior2 += 1;
                }
            }
            #endregion
            #region Archer
            if (Winner.Entity.Class >= 40 && Winner.Entity.Class <= 45)
            {

                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 4k CPs And TopClassArcher2 cause VIP", BroadCastLoc.World);
                    Winner.Entity.TopArcher2 += 1;
                }
                else
                {

                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 2k CPs And TopClassArcher2", BroadCastLoc.World);
                    Winner.Entity.TopArcher2 += 1;
                }
            }
            #endregion
            #region Trojan
            if (Winner.Entity.Class >= 10 && Winner.Entity.Class <= 15)
            {
                if (Winner.Entity.VIPLevel > 0)
                {
                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 4k CPs And TopClassTrojan2 cause he VIP", BroadCastLoc.World);
                    Winner.Entity.TopTrojan2 += 1;
                }
                else
                {


                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 2k CPs And TopClassTrojan2", BroadCastLoc.World);
                    Winner.Entity.TopTrojan2 += 1;
                }
            }
            #endregion
            #region Water
            if (Winner.Entity.Class >= 130 && Winner.Entity.Class <= 135)
            {
                if (Winner.Entity.VIPLevel > 0)
                {

                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 4k CPs And TopClassWaterToist2 cause he VIP", BroadCastLoc.World);
                    Winner.Entity.TopWaterTaoist2 += 1;
                }
                else
                {


                    Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 2k CPs And TopClassWaterToist2", BroadCastLoc.World);
                    Winner.Entity.TopWaterTaoist2 += 1;
                }
            }
            #endregion
            #region Fire
            if (Winner.Entity.Class >= 140 && Winner.Entity.Class <= 145)
            {
                   if (Winner.Entity.VIPLevel > 0)
                {
            
                Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 4k CPs And TopClassFireToist2 cause he VIP", BroadCastLoc.World);
                Winner.Entity.TopFireTaoist2 += 1;
                }
                else
                {
                Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 2k CPs And TopClassFireToist2", BroadCastLoc.World);
                Winner.Entity.TopFireTaoist2 += 1;
                   }
            }
            #endregion
            #region Monk
            if (Winner.Entity.Class >= 60 && Winner.Entity.Class <= 65)
            {
                   if (Winner.Entity.VIPLevel > 0)
                {
            
             Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 4k CPs And TopClassMonk2 cause He VIp", BroadCastLoc.World);
                Winner.Entity.TopMonk2 += 1;
                }
                else
                {
                Broadcast(Winner.Entity.Name + " Has Wins LastManStanding war and Got 2k CPs And TopClassMonk2", BroadCastLoc.World);
                Winner.Entity.TopMonk2 += 1;
                   }
            }
            #endregion

            if (Winner.Entity.VIPLevel > 0)
            {
                Winner.Entity.ConquerPoints += 4000;
            }
            else
            {
                Winner.Entity.ConquerPoints += 2000;
            }
            PKTournament.Stage = PKTournamentStage.None;
            PkThread.Abort();
           
            return;
        }
        public static void WaitForWinner()
        {
            Database.PkWarEvent3.ResetTopTrojan2();
            Database.PkWarEvent3.ResetTopArcher2();
            Database.PkWarEvent3.ResetTopFire2();
            Database.PkWarEvent3.ResetTopMonk2();
            Database.PkWarEvent3.ResetTopNinja2();
            Database.PkWarEvent3.ResetTopWarrior2();
            Database.PkWarEvent3.ResetTopWater2();
            PKTournament.Stage = PKTournamentStage.Fighting;
            uint Tick = (uint)Environment.TickCount;
            int InMapAlive = PKTHash2.Count;
            while (true)
            {
                int alive = 0;
                foreach (Conquer_Online_Server.Client.GameState players in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                    if (players.Entity.MapID == 1858 && (!players.Entity.Dead))
                        alive++;

                foreach (Client.GameState _GC in PKTHash2.Values)
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
                System.Threading.Thread.Sleep(5000);
                foreach (Client.GameState _GC in PKTHash2.Values)
                {
                    if (_GC.InPKT)
                        if (alive == 1)
                        {
                            _GC.Entity.Teleport(1002, 438, 382);
                            AwardClass(_GC);
                            ServerBase.Kernel.srs = false;
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


        public static void BeginTournament()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The LastManStanding Tournament has Started! You Wana Join?");
                npc.OptionID = 247;
                clientss.Send(npc.ToArray());
            }

            Stage = PKTournamentStage.Inviting;
            while (CountDown > 0)
            {
                if (CountDown == 10)
                    Broadcast("10 seconds until start", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    Stage = PKTournamentStage.Countdown;
                    //if (PKTHash2.Count < 2)
                    //{
                    //    Broadcast("The tournament requires atleast 2 people to start, Tournament Cancelled.", BroadCastLoc.World);
                    //    Stage = PKTournamentStage.None;
                    //    PKTHash2 = null;
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
