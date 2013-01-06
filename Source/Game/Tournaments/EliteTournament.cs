using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game.Tournaments
{
    public enum Tournamet_Type
    {
        LeveL100,
        LeveL119,
        LeveL129,
        LeveL130
    }
    public enum top_typ
    {
        Elite_PK_Champion__Low_ = 12,
        Elite_PK_2nd_Place_Low_ = 13,
        Elite_PK_3rd_Place_Low_ = 14,
        Elite_PK_Top_8__Low_ = 15,

        Elite_PK_Champion_High_ = 16,
        Elite_PK_2nd_Place_High_ = 17,
        Elite_PK_3rd_Place__High_ = 18,
        Elite_PK_Top_8_High_ = 19
    }
    public class Elite_client
    {
        public uint Points = 0;
        public uint UID = 0;
        public ushort Avatar = 0;
        public ushort Mesh = 0;
        public string Name = "";
        public ushort Postion = 0;
        public byte MyTitle = 0;

        public Elite_client(Client.GameState client)
        {
            this.UID = client.Entity.UID;
            this.Avatar = client.Entity.Face;
            this.Mesh = client.Entity.Body;
            this.Name = client.Entity.Name;

        }
        public Elite_client(uint _uid, ushort _avatar, ushort _mesh, string _name, uint _points, ushort Position, byte Tytle)
        {
            this.MyTitle = Tytle;
            this.Postion = Position;
            this.Points = _points;
            this.UID = _uid;
            this.Avatar = _avatar;
            this.Mesh = _mesh;
            this.Name = _name;
        }
    }
    public class Team_client
    {
        public uint Points = 0;
        public uint UID = 0;
        public ushort Avatar = 0;
        public ushort Mesh = 0;
        public string Name = "";
        public ushort Postion = 0;
        public byte MyTitle = 0;

        public Team_client(Client.GameState client)
        {
            this.UID = client.Entity.UID;
            this.Avatar = client.Entity.Face;
            this.Mesh = client.Entity.Body;
            this.Name = client.Entity.Name;

        }
        public Team_client(uint _uid, ushort _avatar, ushort _mesh, string _name, uint _points, ushort Position, byte Tytle)
        {
            this.MyTitle = Tytle;
            this.Postion = Position;
            this.Points = _points;
            this.UID = _uid;
            this.Avatar = _avatar;
            this.Mesh = _mesh;
            this.Name = _name;
        }
    }
    public class TeamTournament
    {

        public static Dictionary<uint, Team_client> Elite_PK_Tournament = new Dictionary<uint, Team_client>(500);
        public static Dictionary<uint, Team_client> Top8 = new Dictionary<uint, Team_client>(10);

     
        public static void CreatePacket(Client.GameState client)
        {
            client.Entity.TitlePacket = new Network.GamePackets.TitlePacket(true);
            client.Entity.TitlePacket.UID = client.Entity.UID;
            client.Entity.TitlePacket.Type = 4;
            client.Entity.TitlePacket.dwParam = 1;
            client.Entity.TitlePacket.dwParam2 = Top8[client.Entity.UID].MyTitle;
        }
        public void DeleteTabelInstances()
        {
            foreach (Team_client client in Top8.Values)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                int res = cmd.Delete("elitepk", "UID", client.UID).Execute();
            }
        }
        public void LoadTop8()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("elitepk");
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Team_client client = new Team_client(
                    r.ReadUInt32("UID")
                    , r.ReadUInt16("Avatar")
                    , r.ReadUInt16("Mesh")
                    , r.ReadString("Name")
                    , r.ReadUInt32("Points")
                    , r.ReadUInt16("Postion")
                    , r.ReadByte("MyTitle")
                    );
                if (!Top8.ContainsKey(client.UID))
                    Top8.Add(client.UID, client);
            }
            r.Close();
        }
        public void SaveTop8()
        {
            foreach (Team_client client in Top8.Values)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("elitepk")
                    .Insert("UID", client.UID).Insert("Avatar", client.Avatar)
                    .Insert("Mesh", client.Mesh).Insert("Name", client.Name)
                    .Insert("Points", client.Points).Insert("Postion", client.Postion)
              .Insert("MyTitle", client.MyTitle);
                cmd.Execute();
            }
        }
        public TeamTournament() { LoadTop8(); }
        public void Open()
        {
            if (!Start)
            {
                DeleteTabelInstances();
                Start = true;
                CalculateTime = DateTime.Now;
                StartTimer = DateTime.Now;
                SendInvitation();
                Elite_PK_Tournament.Clear();
                Top8.Clear();
            }
        }
        public void Open(int hour, int minute)
        {
            if (DateTime.Now.Minute == minute && DateTime.Now.Hour == hour)
            {
                if (!Start)
                {
                    DeleteTabelInstances();
                    Start = true;
                    CalculateTime = DateTime.Now;
                    StartTimer = DateTime.Now;
                    SendInvitation();
                    Elite_PK_Tournament.Clear();
                    Top8.Clear();
                }
            }
        }
        public void SendInvitation()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Elite Tournament has Started! You Wana Join?");
                npc.OptionID = 249;
                clientss.Send(npc.ToArray());
            }
        }
        private DateTime CalculateTime;
        public void SendThis()
        {
            if (Start)
            {
                if (DateTime.Now > CalculateTime.AddSeconds(7))
                {
                    CalculateTime = DateTime.Now;

                    CalculateRank();

                    Client.GameState[] Clients = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                    foreach (Client.GameState client in Clients)
                    {
                        if (client.Entity.MapID == Mapid)
                        {
                            SendScore(client);
                        }
                    }
                    Finish();
                }
            }
        }
        public void Finish()
        {
            if (Start)
            {
                if (DateTime.Now > StartTimer.AddMinutes(2))
                {
                    CalculateRank();
                    Client.GameState[] Clients = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                    foreach (Client.GameState client in Clients)
                    {
                        if (client.Entity.MapID == Mapid)
                        {
                            this.ObtinedOutCoord(client);
                            this.ObtinedReward(client);
                        }
                    }
                    SaveTop8();
                    Start = false;
                }
            }

        }
        public void ObtinedReward(Client.GameState client)
        {
            switch (client.Entity.Elite.Postion)
            {
                case 1:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_Champion_High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 8000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 4000;
                        }
                        break;
                    }
                case 2:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_2nd_Place_High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 7000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 3500;
                        }
                        break;
                    }
                case 3:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_3rd_Place__High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 6000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 3000;
                        }
                        break;
                    }
                default:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_Top_8_High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 5000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 2500;
                        }
                        break;
                    }
            }
            CreatePacket(client);
        }
        public DateTime StartTimer;
        public bool Start = false;
        private ushort Mapid = 6002;

        public void AddMap(Client.GameState client)
        {

            if (Start)
            {


                client.Entity.Elite2 = new Team_client(client);
                if (!Elite_PK_Tournament.ContainsKey(client.Entity.UID))
                    Elite_PK_Tournament.Add(client.Entity.Elite.UID, client.Entity.Elite2);
                else
                {
                    Elite_PK_Tournament[client.Entity.UID].Points = 0;
                }
                ObtinedCoord(client);
            }
        }
        public void ObtinedOutCoord(Client.GameState client)
        {
            byte Rand = (byte)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 4);
            switch (Rand)
            {
                case 1: client.Entity.Teleport(1002, 391, 371); break;
                case 2: client.Entity.Teleport(1002, 392, 323); break;
                case 3: client.Entity.Teleport(1002, 475, 373); break;
                case 4: client.Entity.Teleport(1002, 405, 246); break;
            }
        }
        public void ObtinedCoord(Client.GameState client)
        {
            byte Rand = (byte)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 10);
            switch (Rand)
            {
                case 1: client.Entity.Teleport(Mapid, 17, 80); break;
                case 2: client.Entity.Teleport(Mapid, 47, 99); break;
                case 3: client.Entity.Teleport(Mapid, 101, 118); break;
                case 4: client.Entity.Teleport(Mapid, 141, 109); break;
                case 5: client.Entity.Teleport(Mapid, 88, 50); break;
                case 6: client.Entity.Teleport(Mapid, 78, 23); break;
                case 7: client.Entity.Teleport(Mapid, 63, 47); break;
                case 8: client.Entity.Teleport(Mapid, 72, 79); break;
                case 9: client.Entity.Teleport(Mapid, 94, 95); break;
                case 10: client.Entity.Teleport(Mapid, 121, 114); break;
            }
        }
        public void SendScore(Client.GameState client)
        {

            if (Start)
            {
                for (uint x = 1; x < 9; x++)
                {
                    string Mesage = "";
                    foreach (Team_client clients in Top8.Values)
                    {
                        if (clients.Postion == x)
                        {
                            Mesage = "No." + x + " " + clients.Name + ": Score: " + clients.Points + "";
                        }
                    }
                    Network.GamePackets.Message msg = new Network.GamePackets.Message(Mesage, System.Drawing.Color.Red, x == 1 ? Network.GamePackets.Message.FirstRightCorner : Network.GamePackets.Message.ContinueRightCorner);
                    client.Send(msg);
                }
            }
        }
        public void CalculateRank()
        {
            if (Start)
            {
                Dictionary<uint, ulong> ToIndex = new Dictionary<uint, ulong>();
                uint CurKey = 0; int Rank = 1;
                for (short x = 0; x < Elite_PK_Tournament.Count; x++)
                {
                    if (Rank == 9)
                        break;
                    ulong Value = 0;
                    foreach (uint K in Elite_PK_Tournament.Keys)
                    {
                        if (Elite_PK_Tournament[K].Points >= Value && !ToIndex.ContainsKey(K))
                        {
                            Value = Elite_PK_Tournament[K].Points; CurKey = K;
                        }
                    }
                    if (!ToIndex.ContainsKey(CurKey))
                        ToIndex.Add(CurKey, Value);
                    if (Elite_PK_Tournament.ContainsKey(CurKey))
                    {
                        Elite_PK_Tournament[CurKey].Postion = (ushort)Rank;
                    }
                    Rank++;
                }
                lock (Top8)
                {
                    Top8.Clear();
                    for (byte x = 1; x < 10; x++)
                    {
                        foreach (Team_client client in Elite_PK_Tournament.Values)
                        {
                            if (client.Postion == x)
                            {
                                Top8.Add(client.UID, client);
                            }
                        }
                    }
                }
            }
        }
    }
    public class EliteTournament
    {

        public static Dictionary<uint, Elite_client> Elite_PK_Tournament = new Dictionary<uint, Elite_client>(500);
        public static Dictionary<uint, Elite_client> Top8 = new Dictionary<uint, Elite_client>(10);

        public void LoginClient(Client.GameState client)
        {
            if (!Start)
            {
                if (Top8.ContainsKey(client.Entity.UID))
                {
                    client.Entity.Elite = Top8[client.Entity.UID];
                    CreatePacket(client);
                }
            }
        }
        public static void LoginClient2(Client.GameState client)
        {

            if (Top8.ContainsKey(client.Entity.UID))
            {
                client.Entity.Elite = Top8[client.Entity.UID];
                CreatePacket(client);
            }

        }
        public static void CreatePacket(Client.GameState client)
        {
            client.Entity.TitlePacket = new Network.GamePackets.TitlePacket(true);
            client.Entity.TitlePacket.UID = client.Entity.UID;
            client.Entity.TitlePacket.Type = 4;
            client.Entity.TitlePacket.dwParam = 1;
            client.Entity.TitlePacket.dwParam2 = Top8[client.Entity.UID].MyTitle;
        }
        public void DeleteTabelInstances()
        {
            foreach (Elite_client client in Top8.Values)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                int res = cmd.Delete("elitepk", "UID", client.UID).Execute();
            }
        }
        public void LoadTop8()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("elitepk");
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Elite_client client = new Elite_client(
                    r.ReadUInt32("UID")
                    , r.ReadUInt16("Avatar")
                    , r.ReadUInt16("Mesh")
                    , r.ReadString("Name")
                    , r.ReadUInt32("Points")
                    , r.ReadUInt16("Postion")
                    , r.ReadByte("MyTitle")
                    );
                if (!Top8.ContainsKey(client.UID))
                    Top8.Add(client.UID, client);
            }
            r.Close();
        }
        public void SaveTop8()
        {
            foreach (Elite_client client in Top8.Values)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("elitepk")
                    .Insert("UID", client.UID).Insert("Avatar", client.Avatar)
                    .Insert("Mesh", client.Mesh).Insert("Name", client.Name)
                    .Insert("Points", client.Points).Insert("Postion", client.Postion)
              .Insert("MyTitle", client.MyTitle);
                cmd.Execute();
            }
        }
        public EliteTournament() { LoadTop8(); }
        public void Open()
        {
            if (!Start)
            {
                DeleteTabelInstances();
                Start = true;
                CalculateTime = DateTime.Now;
                StartTimer = DateTime.Now;
                SendInvitation();
                Elite_PK_Tournament.Clear();
                Top8.Clear();
            }
        }
        public void Open(int hour, int minute)
        {
            if (DateTime.Now.Minute == minute && DateTime.Now.Hour == hour)
            {
                if (!Start)
                {
                    DeleteTabelInstances();
                    Start = true;
                    CalculateTime = DateTime.Now;
                    StartTimer = DateTime.Now;
                    SendInvitation();
                    Elite_PK_Tournament.Clear();
                    Top8.Clear();
                }
            }
        }
        public void SendInvitation()
        {
            Client.GameState[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameState clientss in client)
            {
                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Elite Tournament has Started! You Wana Join?");
                npc.OptionID = 249;
                clientss.Send(npc.ToArray());
            }
        }
        private DateTime CalculateTime;
        public void SendThis()
        {
            if (Start)
            {
                if (DateTime.Now > CalculateTime.AddSeconds(7))
                {
                    CalculateTime = DateTime.Now;

                    CalculateRank();

                    Client.GameState[] Clients = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                    foreach (Client.GameState client in Clients)
                    {
                        if (client.Entity.MapID == Mapid)
                        {
                            SendScore(client);
                        }
                    }
                    Finish();
                }
            }
        }
        public void Finish()
        {
            if (Start)
            {
                if (DateTime.Now > StartTimer.AddMinutes(15))
                {
                    CalculateRank();
                    Client.GameState[] Clients = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
                    foreach (Client.GameState client in Clients)
                    {
                        if (client.Entity.MapID == Mapid)
                        {
                            this.ObtinedOutCoord(client);
                            this.ObtinedReward(client);
                        }
                    }
                    SaveTop8();
                    Start = false;
                }
            }

        }
        public void ObtinedReward(Client.GameState client)
        {
            switch (client.Entity.Elite.Postion)
            {
                case 1:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_Champion_High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 8000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 4000;
                        }
                        break;
                    }
                case 2:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_2nd_Place_High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 7000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 3500;
                        }
                        break;
                    }
                case 3:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_3rd_Place__High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 6000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 3000;
                        }
                        break;
                    }
                default:
                    {
                        client.Entity.Elite.MyTitle = (byte)top_typ.Elite_PK_Top_8_High_;
                        if (client.Entity.VIPLevel > 0)
                        {
                            client.Entity.ConquerPoints += 5000;
                        }
                        else
                        {
                            client.Entity.ConquerPoints += 2500;
                        }
                        break;
                    }
            }
            CreatePacket(client);
        }
        public DateTime StartTimer;
        public bool Start = false;
        private ushort Mapid = 6002;

        public void AddMap(Client.GameState client)
        {

            if (Start)
            {


                client.Entity.Elite = new Elite_client(client);
                if (!Elite_PK_Tournament.ContainsKey(client.Entity.UID))
                    Elite_PK_Tournament.Add(client.Entity.Elite.UID, client.Entity.Elite);
                else
                {
                    Elite_PK_Tournament[client.Entity.UID].Points = 0;
                }
                ObtinedCoord(client);
            }
        }
        public void ObtinedOutCoord(Client.GameState client)
        {
            byte Rand = (byte)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 4);
            switch (Rand)
            {
                case 1: client.Entity.Teleport(1002, 391, 371); break;
                case 2: client.Entity.Teleport(1002, 392, 323); break;
                case 3: client.Entity.Teleport(1002, 475, 373); break;
                case 4: client.Entity.Teleport(1002, 405, 246); break;
            }
        }
        public void ObtinedCoord(Client.GameState client)
        {
            byte Rand = (byte)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 10);
            switch (Rand)
            {
                case 1: client.Entity.Teleport(Mapid, 17, 80); break;
                case 2: client.Entity.Teleport(Mapid, 47, 99); break;
                case 3: client.Entity.Teleport(Mapid, 101, 118); break;
                case 4: client.Entity.Teleport(Mapid, 141, 109); break;
                case 5: client.Entity.Teleport(Mapid, 88, 50); break;
                case 6: client.Entity.Teleport(Mapid, 78, 23); break;
                case 7: client.Entity.Teleport(Mapid, 63, 47); break;
                case 8: client.Entity.Teleport(Mapid, 72, 79); break;
                case 9: client.Entity.Teleport(Mapid, 94, 95); break;
                case 10: client.Entity.Teleport(Mapid, 121, 114); break;
            }
        }
        public void SendScore(Client.GameState client)
        {

            if (Start)
            {
                for (uint x = 1; x < 9; x++)
                {
                    string Mesage = "";
                    foreach (Elite_client clients in Top8.Values)
                    {
                        if (clients.Postion == x)
                        {
                            Mesage = "No." + x + " " + clients.Name + ": Score: " + clients.Points + "";
                        }
                    }
                    Network.GamePackets.Message msg = new Network.GamePackets.Message(Mesage, System.Drawing.Color.Red, x == 1 ? Network.GamePackets.Message.FirstRightCorner : Network.GamePackets.Message.ContinueRightCorner);
                    client.Send(msg);
                }
            }
        }
        public void CalculateRank()
        {
            if (Start)
            {
                Dictionary<uint, ulong> ToIndex = new Dictionary<uint, ulong>();
                uint CurKey = 0; int Rank = 1;
                for (short x = 0; x < Elite_PK_Tournament.Count; x++)
                {
                    if (Rank == 9)
                        break;
                    ulong Value = 0;
                    foreach (uint K in Elite_PK_Tournament.Keys)
                    {
                        if (Elite_PK_Tournament[K].Points >= Value && !ToIndex.ContainsKey(K))
                        {
                            Value = Elite_PK_Tournament[K].Points; CurKey = K;
                        }
                    }
                    if (!ToIndex.ContainsKey(CurKey))
                        ToIndex.Add(CurKey, Value);
                    if (Elite_PK_Tournament.ContainsKey(CurKey))
                    {
                        Elite_PK_Tournament[CurKey].Postion = (ushort)Rank;
                    }
                    Rank++;
                }
                lock (Top8)
                {
                    Top8.Clear();
                    for (byte x = 1; x < 10; x++)
                    {
                        foreach (Elite_client client in Elite_PK_Tournament.Values)
                        {
                            if (client.Postion == x)
                            {
                                Top8.Add(client.UID, client);
                            }
                        }
                    }
                }
            }
        }
    }
}
