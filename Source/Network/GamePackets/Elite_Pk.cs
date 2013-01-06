using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Elite_Pk : Writer
    {
        private byte[] Packet;
        public Elite_Pk(uint clientID)
        {
            Packet = new byte[356];
            WriteUInt16(348, 0, Packet);
            WriteUInt16(2223, 2, Packet);
            Packet[8] = 3;//type
            Packet[16] = 8; // count
            WriteUInt32(clientID, 20, Packet);
        }
        public byte[] ToArray()
        {
            ushort Position = 32;
            Game.Tournaments.Elite_client[] cometition;
            cometition = Game.Tournaments.EliteTournament.Top8.Values.ToArray();
            for (byte x = 1; x < 9; x++)
            {
                foreach (Game.Tournaments.Elite_client clients in cometition)
                {
                    if (x == clients.Postion)
                    {
                        Packet[Position] = (byte)clients.Postion;
                        Position += 4;
                        WriteString(clients.Name, Position, Packet);
                        Position += 16;
                        WriteUInt32(uint.Parse(clients.Avatar.ToString() + clients.Mesh.ToString()), Position, Packet);
                        Position += 4;
                        WriteUInt32(clients.UID, Position, Packet);
                        Position += 12;
                    }
                }
            }
            return Packet;
        }
        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }
    }
    public class Team_Pk : Writer
    {
        private byte[] Packet;
        public Team_Pk(uint clientID)
        {
            Packet = new byte[356];
            WriteUInt16(348, 0, Packet);
            WriteUInt16(2233, 2, Packet);
            Packet[8] = 3;//type
            Packet[16] = 8; // count
            WriteUInt32(clientID, 20, Packet);
     
        }
        public byte[] ToArray()
        {
            ushort Position = 32;
            Game.Tournaments.Team_client[] cometition;
            cometition = Game.Tournaments.TeamTournament.Top8.Values.ToArray();
            for (byte x = 1; x < 9; x++)
            {
                foreach (Game.Tournaments.Team_client clients in cometition)
                {
                    if (x == clients.Postion)
                    {
                        Packet[Position] = (byte)clients.Postion;
                        Position += 4;
                        WriteString(clients.Name + " Teams", Position, Packet);
                        Position += 16;
                        WriteUInt32(uint.Parse(clients.Avatar.ToString() + clients.Mesh.ToString()), Position, Packet);
                        Position += 4;
                        WriteUInt32(clients.UID, Position, Packet);
                        Position += 12;
                    }
                }
            }
            return Packet;
        }
        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }
    }
    public class Guild_Pk : Writer
    {
        private byte[] Packet;
        public Guild_Pk(uint clientID)
        {
            Packet = new byte[356];
            WriteUInt16(348, 0, Packet);
            WriteUInt16(1063, 2, Packet);
            Packet[8] = 3;//type
            Packet[16] = 8; // count
            WriteUInt32(clientID, 20, Packet);
        }
        public byte[] ToArray()
        {
            ushort Position = 32;
            Game.Tournaments.Elite_client[] cometition;
            cometition = Game.Tournaments.EliteTournament.Top8.Values.ToArray();
            for (byte x = 1; x < 9; x++)
            {
                foreach (Game.Tournaments.Elite_client clients in cometition)
                {
                    if (x == clients.Postion)
                    {
                        Packet[Position] = (byte)clients.Postion;
                        Position += 4;
                        WriteString(clients.Name, Position, Packet);
                        Position += 16;
                        WriteUInt32(uint.Parse(clients.Avatar.ToString() + clients.Mesh.ToString()), Position, Packet);
                        Position += 4;
                        WriteUInt32(clients.UID, Position, Packet);
                        Position += 12;
                    }
                }
            }
            return Packet;
        }
        public void Send(Client.GameState client)
        {
            client.Send(ToArray());
        }
    }
}
