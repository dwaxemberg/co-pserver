using System;
using System.Linq;
using System.Collections.Generic;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Team
    {
        public bool ForbidJoin;
        public bool PickupMoney;
        public bool PickupItems;

        public byte LowestLevel;
        public uint LowestLevelsUID;
        private Dictionary<uint, Client.GameState> m_Team;
        private Client.GameState[] m_Teammates;

        public bool TeamLeader;

        public bool Active;

        public bool Full
        {
            get
            {
                if (Teammates != null)
                    return (m_Team.Count == 5);
                return false;
            }
        }
        public Team()
        {
            m_Team = new Dictionary<uint, Client.GameState>(5);
            TeamLeader = false;
            Active = false;
        }
        public Client.GameState[] Teammates
        {
            get
            {
                return m_Teammates;
            }
        }
        public void Add(Client.GameState Teammate)
        {
            if (m_Team.ContainsKey(Teammate.Entity.UID))
                m_Team[Teammate.Entity.UID] = Teammate;
            else
                m_Team.Add(Teammate.Entity.UID, Teammate);
            if (LowestLevel == 0)
            {
                LowestLevel = Teammate.Entity.Level;
                LowestLevelsUID = Teammate.Entity.UID;
            }
            else
            {
                if (Teammate.Entity.Level < LowestLevel)
                {
                    LowestLevel = Teammate.Entity.Level;
                    LowestLevelsUID = Teammate.Entity.UID;
                }
            }
            m_Teammates = m_Team.Values.ToArray();
        }
        public void Remove(uint UID)
        {
            if (LowestLevelsUID == UID)
            {
                LowestLevelsUID = 0;
                LowestLevel = 0;
                SearchForLowest();
            }
            m_Team.Remove(UID);
            m_Teammates = m_Team.Values.ToArray();
        }
        public void SendMessage(Interfaces.IPacket message)
        {
            foreach (var teammate in Teammates)
                teammate.Send(message);
        }
        public void SearchForLowest()
        {
            foreach (Client.GameState client in Teammates)
            {
                if (LowestLevel == 0)
                {
                    LowestLevel = client.Entity.Level;
                    LowestLevelsUID = client.Entity.UID;
                }
                else
                {
                    if (client.Entity.Level < LowestLevel)
                    {
                        LowestLevel = client.Entity.Level;
                        LowestLevelsUID = client.Entity.UID;
                    }
                }
            }
        }
        public bool IsTeammate(uint UID)
        {
            return m_Team.ContainsKey(UID);
        }
        public bool CanGetNoobExperience(Client.GameState Teammate)
        {
            return Teammate.Entity.Level > LowestLevel && LowestLevel < 70;
        }
    }
}
