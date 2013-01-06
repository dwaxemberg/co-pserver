using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using Conquer_Online_Server.Interfaces;
using System.Text;
using System.Linq;

namespace Conquer_Online_Server.Game
{
    public class Map
    {
        public static ServerBase.Counter DynamicIDs = new Conquer_Online_Server.ServerBase.Counter(11000) { Finish = 60000 };

        public static Enums.ConquerAngle[] Angles = new Enums.ConquerAngle[] {
            Enums.ConquerAngle.SouthWest,
            Enums.ConquerAngle.West,
            Enums.ConquerAngle.NorthWest,
            Enums.ConquerAngle.North,
            Enums.ConquerAngle.NorthEast,
            Enums.ConquerAngle.East,
            Enums.ConquerAngle.SouthEast,
            Enums.ConquerAngle.South };
        public static Floor ArenaBaseFloor = null;
        public ServerBase.Counter EntityUIDCounter = new Conquer_Online_Server.ServerBase.Counter(400000);
        public List<Zoning.Zone> Zones = new List<Zoning.Zone>();
        public ushort ID;
        public ushort BaseID;
        public Floor Floor;
        private string Path;
        public bool IsDynamic()
        {
            return BaseID != ID;
        }
        public SafeDictionary<uint, Entity> Entities;
        public SafeDictionary<uint, Entity> Companions;
        public Dictionary<uint, INpc> Npcs;
        public void AddNpc(INpc npc)
        {
            if (Npcs.ContainsKey(npc.UID) == false)
            {
                Npcs.Add(npc.UID, npc);
                #region Setting the near coords invalid to avoid unpickable items.
                Floor[npc.X, npc.Y, MapObjectType.InvalidCast, npc] = false;
                if (npc.Mesh / 10 != 108 && (byte)npc.Type < 10)
                {
                    ushort X = npc.X, Y = npc.Y;
                    foreach (Enums.ConquerAngle angle in Angles)
                    {
                        ushort xX = X, yY = Y;
                        UpdateCoordonatesForAngle(ref xX, ref yY, angle);
                        Floor[xX, yY, MapObjectType.InvalidCast, null] = false;
                    }
                }
                #endregion
            }
        }
        public void AddEntity(Entity entity)
        {
            if (entity.UID < 800000)
            {
                if (Entities.ContainsKey(entity.UID) == false)
                {
                    Entities.Add(entity.UID, entity);
                    Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
                }
            }
            else
            {
                if (Companions.ContainsKey(entity.UID) == false)
                {
                    Companions.Add(entity.UID, entity);
                    Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
                }
            }
        }
        public void RemoveEntity(Entity entity)
        {
            if (Entities.ContainsKey(entity.UID) == true)
            {
                Entities.Remove(entity.UID);
                Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = true;
            }
            if (Companions.ContainsKey(entity.UID) == true)
            {
                Companions.Remove(entity.UID);
                Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = true;
            }
        }
        public void AddFloorItem(Network.GamePackets.FloorItem floorItem)
        {
            Floor[floorItem.X, floorItem.Y, MapObjectType.Item, floorItem] = false;
        }
        public void RemoveFloorItem(Network.GamePackets.FloorItem floorItem)
        {
            Floor[floorItem.X, floorItem.Y, MapObjectType.Item, floorItem] = true;
        }

        public bool SelectCoordonates(ref ushort X, ref ushort Y)
        {
            if (Floor[X, Y, MapObjectType.Item, null])
            {
                bool can = true;
                if (Zones.Count != 0)
                {
                    foreach (Zoning.Zone z in Zones)
                    {
                        if (z.IsPartOfRectangle(new Point() { X = X, Y = Y }))
                        {
                            can = false;
                            break;
                        }
                    }
                }
                if (can)
                    return true;
            }

            foreach (Enums.ConquerAngle angle in Angles)
            {
                ushort xX = X, yY = Y;
                UpdateCoordonatesForAngle(ref xX, ref yY, angle);
                if (Floor[xX, yY, MapObjectType.Item, null])
                {
                    if (Zones.Count != 0)
                    {
                        bool can = true;
                        foreach (Zoning.Zone z in Zones)
                        {
                            if (z.IsPartOfRectangle(new Point() { X = xX, Y = yY }))
                            { can = false; break; }
                        }
                        if (!can)
                            continue;
                    }
                    X = xX;
                    Y = yY;
                    return true;
                }
            }
            return false;
        }
        public static void UpdateCoordonatesForAngle(ref ushort X, ref ushort Y, Enums.ConquerAngle angle)
        {
            sbyte xi = 0, yi = 0;
            switch (angle)
            {
                case Enums.ConquerAngle.North: xi = -1; yi = -1; break;
                case Enums.ConquerAngle.South: xi = 1; yi = 1; break;
                case Enums.ConquerAngle.East: xi = 1; yi = -1; break;
                case Enums.ConquerAngle.West: xi = -1; yi = 1; break;
                case Enums.ConquerAngle.NorthWest: xi = -1; break;
                case Enums.ConquerAngle.SouthWest: yi = 1; break;
                case Enums.ConquerAngle.NorthEast: yi = -1; break;
                case Enums.ConquerAngle.SouthEast: xi = 1; break;
            }
            X = (ushort)(X + xi);
            Y = (ushort)(Y + yi);
        }
        #region Scenes
        private SceneFile[] Scenes;
        private static string NTString(string value)
        {
            value = value.Remove(value.IndexOf("\0"));
            return value;
        }
        private SceneFile CreateSceneFile(BinaryReader Reader)
        {
            SceneFile file = new SceneFile();
            file.SceneFileName = NTString(Encoding.ASCII.GetString(Reader.ReadBytes(260)));
            file.Location = new Point(Reader.ReadInt32(), Reader.ReadInt32());
            using (BinaryReader reader = new BinaryReader(new FileStream(ServerBase.Constants.DataHolderPath + file.SceneFileName, FileMode.Open)))
            {
                ScenePart[] partArray = new ScenePart[reader.ReadInt32()];
                for (int i = 0; i < partArray.Length; i++)
                {
                    reader.BaseStream.Seek(0x14cL, SeekOrigin.Current);
                    partArray[i].Size = new Size(reader.ReadInt32(), reader.ReadInt32());
                    reader.BaseStream.Seek(4L, SeekOrigin.Current);
                    partArray[i].StartPosition = new Point(reader.ReadInt32(), reader.ReadInt32());
                    reader.BaseStream.Seek(4L, SeekOrigin.Current);
                    partArray[i].NoAccess = new bool[partArray[i].Size.Width, partArray[i].Size.Height];
                    for (int j = 0; j < partArray[i].Size.Height; j++)
                    {
                        for (int k = 0; k < partArray[i].Size.Width; k++)
                        {
                            partArray[i].NoAccess[k, j] = reader.ReadInt32() == 0;
                            reader.BaseStream.Seek(8L, SeekOrigin.Current);
                        }
                    }
                }
                file.Parts = partArray;
            }
            return file;
        }
        public struct SceneFile
        {
            public string SceneFileName
            {
                get;
                set;
            }
            public Point Location
            {
                get;
                set;
            }
            public ScenePart[] Parts
            {
                get;
                set;
            }
        }
        public struct ScenePart
        {
            public string Animation;
            public string PartFile;
            public Point Offset;
            public int aniInterval;
            public System.Drawing.Size Size;
            public int Thickness;
            public Point StartPosition;
            public bool[,] NoAccess;
        }
        #endregion

        public Map(ushort id, string path)
        {
            if (!ServerBase.Kernel.Maps.ContainsKey(id))
                ServerBase.Kernel.Maps.Add(id, this);
            Npcs = new Dictionary<uint, INpc>();
            Entities = new SafeDictionary<uint, Entity>(10000);
            Floor = new Floor(0, 0, id);
            Companions = new SafeDictionary<uint, Entity>(1000);
            ID = id;
            BaseID = id;
            if (path == "")
                path = Database.DMaps.MapPaths[id];
            Path = path;
            #region Loading floor.
            if (File.Exists(ServerBase.Constants.DMapsPath + "\\maps\\" + id.ToString() + ".map"))
            {
                byte[] buff = File.ReadAllBytes(ServerBase.Constants.DMapsPath + "\\maps\\" + id.ToString() + ".map");
                MemoryStream FS = new MemoryStream(buff);
                BinaryReader BR = new BinaryReader(FS);
                int Width = BR.ReadInt32();
                int Height = BR.ReadInt32();
                Floor = new Game.Floor(Width, Height, ID);
                if (id == 700)
                    if (ArenaBaseFloor == null)
                        ArenaBaseFloor = new Game.Floor(Width, Height, ID);
                for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                {
                    for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                    {
                        Floor[x, y, MapObjectType.InvalidCast, null] = !(BR.ReadByte() == 1 ? true : false);
                        if (id == 700)
                            if (ArenaBaseFloor == null)
                                ArenaBaseFloor[x, y, MapObjectType.InvalidCast, null] = !(BR.ReadByte() == 1 ? true : false);
                    }
                }

                BR.Close();
                FS.Close();
            }
            else
            {
                if (File.Exists(ServerBase.Constants.DMapsPath + Path))
                {
                    byte[] buff = File.ReadAllBytes(ServerBase.Constants.DMapsPath + Path);
                    MemoryStream FS = new MemoryStream(buff);
                    BinaryReader BR = new BinaryReader(FS);
                    BR.ReadBytes(268);
                    int Width = BR.ReadInt32();
                    int Height = BR.ReadInt32();
                    Floor = new Game.Floor(Width, Height, ID);
                    if (id == 700)
                        if (ArenaBaseFloor == null)
                            ArenaBaseFloor = new Game.Floor(Width, Height, ID);
                    for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                    {
                        for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                        {
                            Floor[x, y, MapObjectType.InvalidCast, null] = !Convert.ToBoolean(BR.ReadUInt16());
                            if (id == 700)
                                if (ArenaBaseFloor == null)
                                    ArenaBaseFloor[x, y, MapObjectType.InvalidCast, null] = !(BR.ReadByte() == 1 ? true : false);
                            BR.BaseStream.Seek(4L, SeekOrigin.Current);
                        }
                        BR.BaseStream.Seek(4L, SeekOrigin.Current);
                    }
                    uint amount = BR.ReadUInt32();
                    BR.BaseStream.Seek(amount * 12, SeekOrigin.Current);

                    int num = BR.ReadInt32();
                    List<SceneFile> list = new List<SceneFile>();
                    for (int i = 0; i < num; i++)
                    {
                        switch (BR.ReadInt32())
                        {
                            case 10:
                                BR.BaseStream.Seek(0x48L, SeekOrigin.Current);
                                break;

                            case 15:
                                BR.BaseStream.Seek(0x114L, SeekOrigin.Current);
                                break;

                            case 1:
                                list.Add(this.CreateSceneFile(BR));
                                break;

                            case 4:
                                BR.BaseStream.Seek(0x1a0L, SeekOrigin.Current);
                                break;
                        }
                    }
                    Scenes = list.ToArray();

                    for (int i = 0; i < Scenes.Length; i++)
                    {
                        foreach (ScenePart part in Scenes[i].Parts)
                        {
                            for (int j = 0; j < part.Size.Width; j++)
                            {
                                for (int k = 0; k < part.Size.Height; k++)
                                {
                                    Point point = new Point();
                                    point.X = ((Scenes[i].Location.X + part.StartPosition.X) + j) - part.Size.Width;
                                    point.Y = ((Scenes[i].Location.Y + part.StartPosition.Y) + k) - part.Size.Height;
                                    Floor[(ushort)point.X, (ushort)point.Y, MapObjectType.InvalidCast, null] = part.NoAccess[j, k];
                                }
                            }
                        }
                    }

                    BR.Close();
                    FS.Close();
                    SaveMap();
                }
            }
            #endregion
            LoadNpcs();
            LoadZones();
            LoadMonsters();
            LoadPortals();
            //FloorItemTimerCallBack = new TimerCallback(_timerFloorItemCallBack);
            //_timerFloorItem = new Timer(FloorItemTimerCallBack, this, 10000, 2000);
            System.Threading.Thread.Sleep(100);
        }
        public Map(ushort id, ushort baseid, string path, bool dynamic)
        {
            if (!ServerBase.Kernel.Maps.ContainsKey(id))
                ServerBase.Kernel.Maps.Add(id, this);
            Npcs = new Dictionary<uint, INpc>();
            Entities = new SafeDictionary<uint, Entity>(10000);
            Companions = new SafeDictionary<uint, Entity>(1000);
            ID = id;
            BaseID = baseid;
            Path = path;
            Floor = new Floor(0, 0, id);
            #region Loading floor.
            if (id != baseid && baseid == 700 && ArenaBaseFloor != null)
            {
                Floor = new Game.Floor(ArenaBaseFloor.Bounds.Width, ArenaBaseFloor.Bounds.Height, ID);
                for (ushort y = 0; y < ArenaBaseFloor.Bounds.Height; y = (ushort)(y + 1))
                {
                    for (ushort x = 0; x < ArenaBaseFloor.Bounds.Width; x = (ushort)(x + 1))
                    {
                        Floor[x, y, MapObjectType.Player, null] = ArenaBaseFloor[x, y, MapObjectType.Player, null];
                    }
                }
            }
            else
            {
                if (File.Exists(ServerBase.Constants.DMapsPath + "\\maps\\" + baseid.ToString() + ".map"))
                {
                    byte[] buff = File.ReadAllBytes(ServerBase.Constants.DMapsPath + "\\maps\\" + baseid.ToString() + ".map");
                    MemoryStream FS = new MemoryStream(buff);
                    BinaryReader BR = new BinaryReader(FS);
                    int Width = BR.ReadInt32();
                    int Height = BR.ReadInt32();

                    Floor = new Game.Floor(Width, Height, ID);

                    for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                    {
                        for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                        {
                            Floor[x, y, MapObjectType.InvalidCast, null] = !(BR.ReadByte() == 1 ? true : false);
                        }
                    }
                    BR.Close();
                    FS.Close();
                }
                else
                {
                    if (File.Exists(ServerBase.Constants.DMapsPath + Path))
                    {
                        FileStream FS = new FileStream(ServerBase.Constants.DMapsPath + Path, FileMode.Open);
                        BinaryReader BR = new BinaryReader(FS);
                        BR.ReadBytes(268);
                        int Width = BR.ReadInt32();
                        int Height = BR.ReadInt32();

                        Floor = new Game.Floor(Width, Height, ID);

                        for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                        {
                            for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                            {
                                Floor[x, y, MapObjectType.InvalidCast, null] = !Convert.ToBoolean(BR.ReadUInt16());

                                BR.BaseStream.Seek(4L, SeekOrigin.Current);
                            }
                            BR.BaseStream.Seek(4L, SeekOrigin.Current);
                        }
                        uint amount = BR.ReadUInt32();
                        BR.BaseStream.Seek(amount * 12, SeekOrigin.Current);

                        int num = BR.ReadInt32();
                        List<SceneFile> list = new List<SceneFile>();
                        for (int i = 0; i < num; i++)
                        {
                            switch (BR.ReadInt32())
                            {
                                case 10:
                                    BR.BaseStream.Seek(0x48L, SeekOrigin.Current);
                                    break;

                                case 15:
                                    BR.BaseStream.Seek(0x114L, SeekOrigin.Current);
                                    break;

                                case 1:
                                    list.Add(this.CreateSceneFile(BR));
                                    break;

                                case 4:
                                    BR.BaseStream.Seek(0x1a0L, SeekOrigin.Current);
                                    break;
                            }
                        }
                        Scenes = list.ToArray();

                        for (int i = 0; i < Scenes.Length; i++)
                        {
                            foreach (ScenePart part in Scenes[i].Parts)
                            {
                                for (int j = 0; j < part.Size.Width; j++)
                                {
                                    for (int k = 0; k < part.Size.Height; k++)
                                    {
                                        Point point = new Point();
                                        point.X = ((Scenes[i].Location.X + part.StartPosition.X) + j) - part.Size.Width;
                                        point.Y = ((Scenes[i].Location.Y + part.StartPosition.Y) + k) - part.Size.Height;
                                        Floor[(ushort)point.X, (ushort)point.Y, MapObjectType.InvalidCast, null] = part.NoAccess[j, k];
                                    }
                                }
                            }
                        }

                        BR.Close();
                        FS.Close();
                        SaveMap();
                    }
                }
            }
            #endregion
            LoadNpcs();
            LoadZones();
            LoadMonsters();
            LoadPortals();
            //FloorItemTimerCallBack = new TimerCallback(_timerFloorItemCallBack);
            //_timerFloorItem = new Timer(FloorItemTimerCallBack, this, 10000, 2000);
        }

        private void LoadPortals()
        {
            ServerBase.IniFile file = new Conquer_Online_Server.ServerBase.IniFile(ServerBase.Constants.PortalsPath);
            ushort portalCount = file.ReadUInt16(BaseID.ToString(), "Count");

            for (int i = 0; i < portalCount; i++)
            {
                string _PortalEnter = file.ReadString(BaseID.ToString(), "PortalEnter" + i.ToString());
                string _PortalExit = file.ReadString(BaseID.ToString(), "PortalExit" + i.ToString());
                string[] PortalEnter = _PortalEnter.Split(' ');
                string[] PortalExit = _PortalExit.Split(' ');
                Game.Portal portal = new Conquer_Online_Server.Game.Portal();
                portal.CurrentMapID = Convert.ToUInt16(PortalEnter[0]);
                portal.CurrentX = Convert.ToUInt16(PortalEnter[1]);
                portal.CurrentY = Convert.ToUInt16(PortalEnter[2]);
                portal.DestinationMapID = Convert.ToUInt16(PortalExit[0]);
                portal.DestinationX = Convert.ToUInt16(PortalExit[1]);
                portal.DestinationY = Convert.ToUInt16(PortalExit[2]);
                Portals.Add(portal);
            }
        }
        public List<Game.Portal> Portals = new List<Game.Portal>();
        private TimerCallback _timercallback;
        private Timer _timer;
        private void SaveMap()
        {
            if (!File.Exists(ServerBase.Constants.DMapsPath + "\\maps\\" + BaseID.ToString() + ".map"))
            {
                FileStream stream = new FileStream(ServerBase.Constants.DMapsPath + "\\maps\\" + BaseID.ToString() + ".map", FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write((uint)Floor.Bounds.Width);
                writer.Write((uint)Floor.Bounds.Height);
                for (int y = 0; y < Floor.Bounds.Height; y++)
                {
                    for (int x = 0; x < Floor.Bounds.Width; x++)
                    {
                        writer.Write((byte)(Floor[x, y, MapObjectType.InvalidCast, null] == true ? 1 : 0));
                    }
                }
                writer.Close();
                stream.Close();
            }
        }
        private void LoadZones()
        {
            Database.MySqlCommand command = new Conquer_Online_Server.Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.SELECT);
            command.Select("notavailablepaths").Where("mapid", ID);

            Database.MySqlReader reader = new Conquer_Online_Server.Database.MySqlReader(command);
            while (reader.Read())
            {
                Zoning.Zone zone = new Zoning.Zone(
                    new Point() { X = reader.ReadInt32("Point1_X"), Y = reader.ReadInt32("Point1_Y") },
                    new Point() { X = reader.ReadInt32("Point2_X"), Y = reader.ReadInt32("Point2_Y") },
                    new Point() { X = reader.ReadInt32("Point3_X"), Y = reader.ReadInt32("Point3_Y") },
                    new Point() { X = reader.ReadInt32("Point4_X"), Y = reader.ReadInt32("Point4_Y") }
                    );
                Zones.Add(zone);
            }
            reader.Close();
        }
        private void LoadNpcs()
        {
            Database.MySqlCommand command = new Conquer_Online_Server.Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.SELECT);
            command.Select("npcs").Where("mapid", ID);
            Database.MySqlReader reader = new Conquer_Online_Server.Database.MySqlReader(command);
            while (reader.Read())
            {
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = reader.ReadUInt32("id");
                npc.Mesh = reader.ReadUInt16("lookface");
                npc.Type = (Enums.NpcType)reader.ReadByte("type");
                npc.X = reader.ReadUInt16("cellx"); ;
                npc.Y = reader.ReadUInt16("celly");
                npc.MapID = ID;
                AddNpc(npc);
            }
            reader.Close();
            command = new Conquer_Online_Server.Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.SELECT);
            command.Select("sobnpcs").Where("mapid", ID);
            reader = new Conquer_Online_Server.Database.MySqlReader(command);
            while (reader.Read())
            {
                Network.GamePackets.SobNpcSpawn npc = new Network.GamePackets.SobNpcSpawn();
                npc.UID = reader.ReadUInt32("id");
                npc.Mesh = reader.ReadUInt16("lookface");
                if (ID == 1039)
                    npc.Mesh = (ushort)(npc.Mesh - npc.Mesh % 10 + 7);
                npc.Type = (Enums.NpcType)reader.ReadByte("type");
                npc.X = reader.ReadUInt16("cellx"); ;
                npc.Y = reader.ReadUInt16("celly");
                npc.MapID = reader.ReadUInt16("mapid");
                npc.Sort = reader.ReadUInt16("sort");
                npc.ShowName = true;
                npc.Name = reader.ReadString("name");
                npc.Hitpoints = reader.ReadUInt32("life");
                npc.MaxHitpoints = reader.ReadUInt32("maxlife");
                AddNpc(npc);
            }
            reader.Close();
        }
        public bool FreezeMonsters = false;
        public void LoadMonsters()
        {
            Companions = new SafeDictionary<uint, Entity>(1000);
            Database.MySqlCommand command = new Conquer_Online_Server.Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.SELECT);
            command.Select("monsterspawns").Where("mapid", ID);
            Database.MySqlReader reader = new Conquer_Online_Server.Database.MySqlReader(command);
            int mycount = 0;
            try
            {
                while (reader.Read())
                {
                    uint monsterID = reader.ReadUInt32("npctype");
                    ushort CircleDiameter = reader.ReadUInt16("maxnpc");
                    ushort X = reader.ReadUInt16("bound_x");
                    ushort Y = reader.ReadUInt16("bound_y");
                    ushort XPlus = reader.ReadUInt16("bound_cx");
                    ushort YPlus = reader.ReadUInt16("bound_cy");
                    ushort Amount = reader.ReadUInt16("max_per_gen");
                    int respawn = reader.ReadInt32("rest_secs");
                    if (Database.MonsterInformation.MonsterInfos.ContainsKey(monsterID))
                    {
                        Database.MonsterInformation mt = Database.MonsterInformation.MonsterInfos[monsterID];
                        mt.RespawnTime = respawn + 5;
                        mt.BoundX = X;
                        mt.BoundY = Y;
                        mt.BoundCX = XPlus;
                        mt.BoundCY = YPlus;

                        bool more = true;
                        for (int count = 0; count < Amount; count++)
                        {
                            if (!more)
                                break;
                            Entity entity = new Entity(EntityFlag.Monster, false);
                            entity.MapObjType = MapObjectType.Monster;
                            entity.MonsterInfo = mt.Copy();
                            entity.MonsterInfo.Owner = entity;
                            entity.Name = mt.Name;
                            entity.MinAttack = mt.MinAttack;
                            entity.MaxAttack = entity.MagicAttack = mt.MaxAttack;
                            entity.Hitpoints = entity.MaxHitpoints = mt.Hitpoints;
                            entity.Body = mt.Mesh;
                            entity.Level = mt.Level;
                            entity.UID = EntityUIDCounter.Next;
                            entity.MapID = ID;
                            entity.SendUpdates = true;
                            entity.X = (ushort)(X + ServerBase.Kernel.Random.Next(0, XPlus));
                            entity.Y = (ushort)(Y + ServerBase.Kernel.Random.Next(0, YPlus));
                            for (int count2 = 0; count2 < 50; count2++)
                            {
                                if (!Floor[entity.X, entity.Y, MapObjectType.Monster, entity])
                                {
                                    entity.X = (ushort)(X + ServerBase.Kernel.Random.Next(0, XPlus));
                                    entity.Y = (ushort)(Y + ServerBase.Kernel.Random.Next(0, YPlus));
                                    if (count2 == 50)
                                        more = false;
                                }
                                else
                                    break;
                            }
                            if (more)
                            {
                                if (Floor[entity.X, entity.Y, MapObjectType.Monster, entity])
                                {
                                    mycount++;
                                    AddEntity(entity);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
            reader.Close();
            if (mycount != 0)
            {
                MyTimer = new System.Timers.Timer(interval);
                MyTimer.AutoReset = true;
                MyTimer.Elapsed += new System.Timers.ElapsedEventHandler(_timerCallBack);
                MyTimer.Start();
                // Thread_time = DateTime.Now;

                //CallBack = new System.Threading.TimerCallback(_timerCallBack);
                //_timer = new ManagedThreadTimer(5000, 200, CallBack, ThreadPool, this);
                //CallBack = new TimerCallback(_timerCallBack);
                //_timer = new Timer(CallBack, this, 10000, 1000);
                // Console.WriteLine("Loaded and spawned " + mycount + " monsters on " + ID + ".");
            }
        }
        public double interval = 200;
        public System.Timers.Timer MyTimer;
        public Time32 LastReload = Time32.Now;
        public DateTime Thread_time;
        private void _timerCallBack(object myObject, System.Timers.ElapsedEventArgs arg)
        {
            foreach (Entity monster in Companions.Values)
            {
                if (!monster.Owner.Socket.Connected)
                {
                    RemoveEntity(monster);
                    break;
                }
            }
            foreach (Entity monster in Entities.Values)
            {
                if (monster.Dead)
                {
                    if (Time32.Now > monster.DeathStamp.AddSeconds(monster.MonsterInfo.RespawnTime))
                    {
                        monster.X = (ushort)(monster.MonsterInfo.BoundX + ServerBase.Kernel.Random.Next(0, monster.MonsterInfo.BoundCX));
                        monster.Y = (ushort)(monster.MonsterInfo.BoundY + ServerBase.Kernel.Random.Next(0, monster.MonsterInfo.BoundCY));
                        for (int count = 0; count < monster.MonsterInfo.BoundCX * monster.MonsterInfo.BoundCY; count++)
                        {
                            if (!Floor[monster.X, monster.Y, MapObjectType.Monster, null])
                            {
                                monster.X = (ushort)(monster.MonsterInfo.BoundX + ServerBase.Kernel.Random.Next(0, monster.MonsterInfo.BoundCX));
                                monster.Y = (ushort)(monster.MonsterInfo.BoundY + ServerBase.Kernel.Random.Next(0, monster.MonsterInfo.BoundCY));
                            }
                            else
                                break;
                        }
                        if (Floor[monster.X, monster.Y, MapObjectType.Monster, null] || monster.X == monster.MonsterInfo.BoundX && monster.Y == monster.MonsterInfo.BoundY)
                        {
                            monster.Hitpoints = monster.MonsterInfo.Hitpoints;
                            //monster.RemoveFlag(monster.StatusFlag1);
                            Network.GamePackets._String stringPacket = new Conquer_Online_Server.Network.GamePackets._String(true);
                            stringPacket.UID = monster.UID;
                            stringPacket.Type = Network.GamePackets._String.Effect;
                            stringPacket.Texts.Add("MBStandard");
                            monster.SetFlag(0, 0);
                            foreach (Client.GameState client in ServerBase.Kernel.GamePool.Values)
                            {
                                if (client.Map.ID == ID)
                                {
                                    if (ServerBase.Kernel.GetDistance(client.Entity.X, client.Entity.Y, monster.X, monster.Y) < ServerBase.Constants.nScreenDistance)
                                    {
                                        monster.CauseOfDeathIsMagic = false;
                                        monster.SendSpawn(client, false);
                                        client.Send(stringPacket);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (monster.ToxicFogLeft > 0)
                    {
                        if (Time32.Now >= monster.ToxicFogStamp.AddSeconds(2))
                        {
                            monster.ToxicFogLeft--;
                            monster.ToxicFogStamp = Time32.Now;
                            if (monster.Hitpoints > 1)
                            {
                                uint damage = Game.Attacking.Calculate.Percent(monster, monster.ToxicFogPercent);
                                monster.Hitpoints -= damage;
                                Network.GamePackets.SpellUse suse = new Conquer_Online_Server.Network.GamePackets.SpellUse(true);
                                suse.Attacker = monster.UID;
                                suse.SpellID = 10010;
                                suse.Targets.Add(monster.UID, damage);
                                monster.MonsterInfo.SendScreen(suse);
                            }
                        }
                    }
                }
            }
        }

        public Map MakeDynamicMap()
        {
            ushort id = (ushort)DynamicIDs.Next;
            Map myDynamic = new Map(id, this.ID, this.Path, true);
            return myDynamic;
        }
        bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
                ServerBase.Kernel.Maps.Remove(ID);

            disposed = true;
        }
    }
    public class Floor
    {
        public Size Bounds;
        public FillStruct[,] Locations;
        public ushort FloorMapID;
        public Floor(int width, int height, ushort mapID)
        {
            FloorMapID = mapID;
            Bounds = new Size(width, height);
            Locations = new FillStruct[width, height];
        }
        public class FillStruct
        {
            public object item;
            public Network.GamePackets.FloorItem Item
            {
                get
                {
                    if (item == null) return null;
                    return item as Network.GamePackets.FloorItem;
                }
                set
                {
                    item = value;
                }
            }
            public Interfaces.INpc Npc;
            public byte Monsters;
            public bool Full;
        }
        public FillStruct GetLocation(int x, int y)
        {
            if (Bounds.Height == Bounds.Width && Bounds.Width == 0)
                return new FillStruct();
            if (y >= Bounds.Height || x >= Bounds.Width || x < 0 || y < 0)
                return new FillStruct();
            FillStruct filltype = Locations[x, y];
            return filltype;
        }
        public bool this[int x, int y, MapObjectType type, object obj]
        {
            get
            {

                if (Bounds.Height == Bounds.Width && Bounds.Width == 0)
                {
                    Console.WriteLine("Floor " + FloorMapID + " not loaded!!");
                    return true;
                }
                if (y >= Bounds.Height || x >= Bounds.Width || x < 0 || y < 0)
                    return false;

                if (Locations[x, y] == null)
                    Locations[x, y] = new FillStruct() { };

                FillStruct filltype = Locations[x, y];
                if (type == MapObjectType.InvalidCast)
                    return filltype.Full;
                if (filltype.Full)
                    return false;
                if (type == MapObjectType.Player)
                {
                    return true;
                }
                else if (type == MapObjectType.Monster)
                {
                    return filltype.Monsters == 0;
                }
                else if (type == MapObjectType.Item)
                {
                    return filltype.Item == null;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    if (Bounds.Height == Bounds.Width && Bounds.Width == 0)
                        return;
                    if (y >= Bounds.Height || x >= Bounds.Width || x < 0 || y < 0)
                        return;

                    if (Locations[x, y] == null)
                        Locations[x, y] = new FillStruct() { };
                    if (type == MapObjectType.InvalidCast)
                    {
                        Locations[x, y].Full = false;
                    }
                    if (type == MapObjectType.Item)
                        Locations[x, y].Item = null;
                    if (type == MapObjectType.Monster)
                        Locations[x, y].Monsters = 0;
                }
                else
                {
                    if (y >= Bounds.Height || x >= Bounds.Width)
                        return;

                    if (Locations[x, y] == null)
                        Locations[x, y] = new FillStruct() { };
                    if (type == MapObjectType.InvalidCast)
                        Locations[x, y].Full = true;
                    if (obj != null)
                    {
                        if (obj is Interfaces.INpc)
                        {
                            Locations[x, y].Npc = obj as Interfaces.INpc;
                        }
                    }
                    if (type == MapObjectType.Item)
                        Locations[x, y].Item = obj as Network.GamePackets.FloorItem;

                    if (type == MapObjectType.Monster)
                        Locations[x, y].Monsters = 1;
                }
            }
        }
    }
    public enum MapObjectType
    {
        SobNpc, Npc, Item, Monster, Player, Nothing, InvalidCast
    }
    public class Portal
    {
        public Portal(ushort CurrentMapID, ushort CurrentX, ushort CurrentY, ushort DestinationMapID, ushort DestinationX, ushort DestinationY)
        {
            this.CurrentMapID = CurrentMapID;
            this.CurrentX = CurrentX;
            this.CurrentY = CurrentY;
            this.DestinationMapID = DestinationMapID;
            this.DestinationX = DestinationX;
            this.DestinationY = DestinationY;
        }
        public Portal()
        {

        }
        public ushort CurrentMapID
        {
            get;
            set;
        }
        public ushort CurrentX
        {
            get;
            set;
        }
        public ushort CurrentY
        {
            get;
            set;
        }
        public ushort DestinationMapID
        {
            get;
            set;
        }
        public ushort DestinationX
        {
            get;
            set;
        }
        public ushort DestinationY
        {
            get;
            set;
        }
    }
}
