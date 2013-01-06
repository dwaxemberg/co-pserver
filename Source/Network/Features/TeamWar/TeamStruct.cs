using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.Features.TeamWar
{
    public class TeamStruct
    {
        public uint UID = 0;
        public bool atWar = false;
        public Time32 LastCheck;
        public Time32 NextRound;
        public Time32 LastRound;
        public int Kills;
        public int Wins;
        public Client.GameState Master;
        public Dictionary<uint, Client.GameState> entities;

        public TeamStruct(Client.GameState _master, uint uid)
        {
            Master = _master;
            if (Master.Team != null)
            {
                if (Master.Team.TeamLeader)
                {
                    UID = uid;
                    atWar = true;
                    entities = new Dictionary<uint, Client.GameState>();
                    foreach (Client.GameState e in Master.Team.Teammates)
                    {
                        e.Entity.Teleport(1844, 187, 162);//change to map
                        entities.Add(e.Entity.UID, e);
                    }
                    Kills = 0;
                    Wins = 0;
                }
            }
        }

        public void Remove(bool desclassify)
        {
            atWar = false;
            foreach (Client.GameState e in entities.Values)
            {
                e.Entity.Teleport(1002, 430, 380);
                if (desclassify)
                { e.Send(new Network.GamePackets.Message("You and your team have been desclassified from the team war, wait the next round begin!", System.Drawing.Color.Yellow, 2005)); }
            }
        }
        public void Teleport()
        {
            atWar = true;
            foreach (Client.GameState e in entities.Values)
            {
                e.Entity.Teleport(1002, 430, 380);
                e.Send(new Network.GamePackets.Message("You and your team have been teleported! Hurry up and protect the master!", System.Drawing.Color.Yellow, 2005));
            }
        }
        public void UpdateInfo(int v1, int v2)
        {
            Kills += v1;
            Wins += v2;
        }

        public void Check()
        {
            if (Time32.Now > LastCheck.AddSeconds(1))
            {
                LastCheck = Time32.Now;
                if (atWar)
                {
                    if (!ServerBase.Kernel.GamePool.ContainsKey(Master.Entity.UID))
                    {
                        Remove(true);
                    }
                    if (Master.Entity.Dead)
                    {
                        Remove(true);
                    }
                }
            }
        }

        public string SendInfo()
        {
            return "Your team has won " + Wins.ToString() + " times and killed " + Kills.ToString() + " players, keep going!";
        }
    }
}
