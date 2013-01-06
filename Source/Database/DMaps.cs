using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class DMaps
    {
        public static SafeDictionary<ushort, string> MapPaths = new SafeDictionary<ushort, string>(280);
        public static void Load()
        {
            if (File.Exists(ServerBase.Constants.DataHolderPath + "GameMap.dat"))
            {
                Time32 start = Time32.Now;
                FileStream FS = new FileStream(ServerBase.Constants.DataHolderPath + "GameMap.dat", FileMode.Open);
                BinaryReader BR = new BinaryReader(FS);
                uint MapCount = BR.ReadUInt32();
                for (uint i = 0; i < MapCount; i++)
                {
                    ushort MapID = (ushort)BR.ReadUInt32();
                    string Path = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
                    if (MapID >= 1712 && MapID <= 1720)
                    {
                        BR.ReadUInt32();
                        continue;
                    }
                    if (Path.EndsWith(".7z"))
                    {
                        Path = Path.Remove(Path.Length - 3, 3);
                        Path += ".dmap";
                    }
                    if (!File.Exists(ServerBase.Constants.DMapsPath + "\\maps\\" + MapID.ToString() + ".map"))
                    if (!File.Exists(ServerBase.Constants.DMapsPath + "\\maps\\" + MapID.ToString() + ".DMap"))
                    {
                        Game.Map map = new Conquer_Online_Server.Game.Map(MapID, Path);
                    }
                    MapPaths.Add(MapID, Path);
                    BR.ReadInt32();
                }
                BR.Close();
                FS.Close();
                Console.WriteLine("Game map loaded successfully.");
            }
            else
                Console.WriteLine("The specified Conquer Online folder doesn't exist. Game map couldn't be loaded.");
        }
    }
}
