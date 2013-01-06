using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class ClassPk
    {
        public static bool ClassPks = false;
        public static ushort Map = 7001;
        public static bool signup = false;
        public static int howmanyinmap = 0;
        public static int TopDlClaim = 0;
        public static int TopGlClaim = 0;
        public static void AddDl()
        {
            TopDlClaim++;
            //return;
        }
        public static void AddGl()
        {
            TopGlClaim++;
            //return;
        }
        public static void CheackAlive()
        {
            howmanyinmap = 0;
            foreach (Client.GameState client in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
            {
                if (client.Entity.MapID == 7001 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap += 1;
                    Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Players Alive in ClassPk Now: " + howmanyinmap + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                }

            }
        }

        public static void End()
        {
            if (DateTime.Now.Minute == 59)
            {
                signup = false;
                ClassPks = false;
                foreach (Client.GameState client in Conquer_Online_Server.ServerBase.Kernel.GamePool.Values)
                {
                    if (DateTime.Now.Minute == 59)
                    {
                        client.Entity.ConquerPoints += 150;
                        Conquer_Online_Server.ServerBase.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message(" ClassPk Has Ended Come Next Week ", System.Drawing.Color.Red, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Conquer_Online_Server.ServerBase.Kernel.GamePool.Values);
                    }
                    if (client.Entity.MapID == 7001)
                    {
                        client.Entity.Teleport(1002, 400, 400);
                    }
                    client.Entity.RemoveFlag(Update.Flags.Flashy);
                }
            }
        }
    }
}
