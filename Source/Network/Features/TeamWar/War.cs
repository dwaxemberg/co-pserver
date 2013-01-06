using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.Features.TeamWar
{
    public class War
    {
        public static uint Round = 0;
        public static Time32 LastCheck;
        public static Time32 LastRound;
        public static Time32 StartAt;
        public static Time32 EndAt;
        public static bool Started = false;
        public static bool Running = false;
        public static TeamStruct Winner;
        public static Dictionary<uint, TeamStruct> Teams = new Dictionary<uint, TeamStruct>();

        public static uint Add(Client.GameState Master)
        {
            uint uid = (uint)ServerBase.Kernel.Random.Next(1000000, 1100000);
            while (Teams.ContainsKey(uid))
            { uid++; }

            Teams.Add(uid, new TeamStruct(Master, uid));

            foreach (Client.GameState e in ServerBase.Kernel.GamePool.Values)
            {
                e.Send(new Network.GamePackets.Message("Hurry up! A new team has joined at team wars event. Don't let them win!", System.Drawing.Color.White, 2011));
            }
            return uid;
        }

        public static void Remove(TeamStruct t)
        {
            if (Teams.ContainsKey(t.UID))
            {
                Teams.Remove(t.UID);
                t.Remove(true);
            }
            else t.Remove(true);
        }

        public static void Start()
        {
            foreach (Client.GameState e in ServerBase.Kernel.GamePool.Values)
            {
                e.Send(new Network.GamePackets.Message("The team war will begin in five minutes. Get ready! SingUp at Twin(445,242) TeamPkManager ", System.Drawing.Color.White, 2011));
            }
            Running = true;
            StartAt = Time32.Now.AddMinutes(5);
            EndAt = Time32.Now.AddMinutes(35);
        }

        public static void Next()
        {
            foreach (Client.GameState e in ServerBase.Kernel.GamePool.Values)
            {
                e.Send(new Network.GamePackets.Message("The next round of team war will be in five minutes. Get ready!", System.Drawing.Color.White, 2011));
            }
            Round++;
            StartAt = Time32.Now.AddMinutes(5);
            EndAt = Time32.Now.AddMinutes(35);
        }
        public static void End(bool Round)
        {
            Started = false;

            foreach (TeamStruct t in Teams.Values)
            {
                t.atWar = false;
                if (Winner != null)
                {
                    if (Winner.Kills < t.Kills && Winner.Wins < t.Wins && t.atWar)
                    {
                        Winner = t;
                    }
                }

                t.Remove(false);
            }

            if (!Round)
            {
                foreach (Client.GameState e in ServerBase.Kernel.GamePool.Values)
                {
                    e.Send(new Network.GamePackets.Message("The team war has end for today. See you all in the next time!", System.Drawing.Color.White, 2011));
                }
            }
        }
        public static void Check()
        {
            LastCheck = Time32.Now;
            if (Teams.Count > 0)
            {
                foreach (TeamStruct _team in Teams.Values)
                {
                    _team.Check();
                }
            }
            if (!Started)
            {
                if (Time32.Now > StartAt)
                {
                    Started = true;
                    foreach (Client.GameState e in ServerBase.Kernel.GamePool.Values)
                    {
                        e.Send(new Network.GamePackets.Message("The team war has begin! Hurry up and kill all your enemies!", System.Drawing.Color.White, 2011));
                    }
                }
            }
            else
            {
                if (Time32.Now > EndAt)
                {
                    LastRound = Time32.Now;
                    End(true);
                    foreach (Client.GameState e in ServerBase.Kernel.GamePool.Values)
                    {
                        e.Send(new Network.GamePackets.Message("The round has ended. Please wait some time to the next one!", System.Drawing.Color.White, 2011));
                    }
                }
            }
        }
    }
}
