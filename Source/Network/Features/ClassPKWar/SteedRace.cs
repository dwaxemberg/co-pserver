﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.ServerBase;

namespace Conquer_Online_Server.Network.Features.ClassPKWar
{
    public class SteedRaces
    {
        public static DateTime TimerStart = DateTime.Now;
        public static ushort No = 0;
        public static ushort Reward = 0;
        public static bool IsRace = false;


        public static void SteedRace()
        {
            IsRace = true;
            TimerStart = DateTime.Now;
            No = 1;
            Reward = 4000;


        }
        public void Open()
        {
            if (!IsRace)
            {
                IsRace = true;
                TimerStart = DateTime.Now;
                No = 1;
                Reward = 4000;
                Client.GameState[] players = null;
                players = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values;
                foreach (Client.GameState clients in players)
                {
                    clients.Entity.Teleport(1002, 403, 396);
                }
                FinishRace();
            }
        }
        public static void FinishRace()
        {
            if (DateTime.Now > TimerStart.AddMinutes(10))
            {
                Program.Steed = false;
                Client.GameState[] players = null;
                players = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values;
                foreach (Client.GameState clients in players)
                {
                    if (clients.Entity.FullyLoaded)
                    {
                        if (clients.Entity != null)
                        {
                            if (clients.Entity.MapID == 1950)
                            {
                                clients.Entity.InSteedRace = false;
                                clients.Entity.Teleport(1002, 403, 396);
                            }
                        }
                        clients.Send(new Network.GamePackets.Message("Steed Race has Finish", System.Drawing.Color.Red, Network.GamePackets.Message.BroadcastMessage));
                        clients.Send(new Network.GamePackets.Message("Steed Race has Finish", System.Drawing.Color.White, Network.GamePackets.Message.Center));
                    }
                }
            }
        }
        public static void GiveReward(Client.GameState client)
        {

            if (IsRace)
            {
                switch (No)
                {
                    case 1:
                        {
                            client.Entity.ConquerPoints += Reward;
                            client.Entity.Teleport(1002, 403, 396);
                            Kernel.SendWorldMessage(new Message("Rank  " + No + "     " + client.Entity.Name + " finished Steed Race     ", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            Reward -= 1500;
                            No++;
                            break;
                        }
                    case 2:
                        {
                            client.Entity.ConquerPoints += Reward;
                            client.Entity.Teleport(1002, 403, 396);
                            Kernel.SendWorldMessage(new Message("Rank  " + No + "     " + client.Entity.Name + " finished Steed Race     ", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            Reward -= 1000;
                            No++;
                            break;
                        }
                    case 3:
                        {
                            client.Entity.ConquerPoints += Reward;
                            client.Entity.Teleport(1002, 403, 396);
                            Kernel.SendWorldMessage(new Message("Rank  " + No + "     " + client.Entity.Name + " finished Steed Race     ", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            Reward -= 500;
                            No++;
                            break;
                        }
                    default:
                        {
                            client.Entity.ConquerPoints += 500;
                            client.Entity.Teleport(1002, 403, 396);
                            Kernel.SendWorldMessage(new Message("Rank  " + No + "     " + client.Entity.Name + " finished Steed Race     ", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            //Reward -= 500;
                            No++;
                            break;
                        }
                    //if (No < 10)
                    //{
                    //    client.Entity.ConquerPoints += Reward;
                    //    client.Entity.Teleport(1002, 403, 396);
                    //    Kernel.SendWorldMessage(new Message(No + "     " + client.Entity.Name + " finished Steed Race     ", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                    //    Reward -= 1500;
                    //    No++;
                    //}
                }
                //else
                //{
                //    client.Entity.ConquerPoints += 500;
                //    client.Entity.Teleport(1002, 403, 396);
                //    No++;
                //}
                SendTimerStatus(client.Entity.Name);
                FinishRace();
            }

        }
        public static void SendTimerStatus(string name)
        {
            Client.GameState[] players = null;
            players = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values;
            foreach (Client.GameState clients in players)
            {
                if (clients.Entity.FullyLoaded)
                {
                    if (clients.Entity != null)
                    {
                        if (clients.Entity.MapID == 1950)
                        {
                            string Minutes = "0";
                            int Minuts = DateTime.Now.Minute - TimerStart.Minute;
                            if (Minuts.ToString().Contains('-'))
                                Minutes = Minuts.ToString().Replace('-', ' ');

                            string Seconds = "0";
                            int sec = DateTime.Now.Second - TimerStart.Second;
                            if (sec.ToString().Contains('-'))
                                Seconds = sec.ToString().Replace('-', ' ');

                            string milSeconds = "0";
                            int milsec = DateTime.Now.Millisecond - TimerStart.Millisecond;
                            if (sec.ToString().Contains('-'))
                                milSeconds = milsec.ToString().Replace('-', ' ');

                            string mess = "No." + No + "           " + name + "    Time: " + Minutes + " : " + Seconds + " : " + milSeconds + " ";
                            clients.Send(new Network.GamePackets.Message(mess, System.Drawing.Color.White, Network.GamePackets.Message.ContinueRightCorner));
                        }
                    }
                }
            }
        }
        public static void Add_Competition(Client.GameState client)
        {

        }
    }
}
