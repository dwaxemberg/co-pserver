using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.ServerBase;
using System.Collections;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.Features
{
    class SteedRace
    {
        public static void CpsFowin(Client.GameState GC)
        {
            // Entity GC = new Entity();
            if (Kernel.SteedTornament.cps == 1)
            {
                GC.Entity.ConquerPoints += 20000;
                GC.Entity.cp = 20000;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                                   
                    //Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Entity.MapID == 1950)
                            {
                                
                                Kernel.SendWorldMessage(new Message(""+ " "+" ", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                                Kernel.SendWorldMessage(new Message( " Rank         Name         Score", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                                Kernel.SendWorldMessage(new Message( "No.1          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                                Conquer_Online_Server.Network.GamePackets._String Packet = new Conquer_Online_Server.Network.GamePackets._String(true);
                                //GamePackets.poker Packet2 = new GamePackets.poker(true, client);
                                Packet.UID = GC.Entity.UID;
                                Packet.Type = _String.Effect;
                                Packet.TextsCount = 1;
                                Packet.Texts.Add("ridmatch_first");
                                GC.SendScreen(Packet, true);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 2)
            {
                GC.Entity.ConquerPoints += 17000;
                GC.Entity.cp = 17000;
                foreach (Client.GameState Chaar in ServerBase.Kernel.GamePool.Values)
                {
                                   
                   // Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.Entity.MapID == 1950)
                            {
               Kernel.SendWorldMessage(new Message( "No.2          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
               Conquer_Online_Server.Network.GamePackets._String Packet = new Conquer_Online_Server.Network.GamePackets._String(true);
               //GamePackets.poker Packet2 = new GamePackets.poker(true, client);
               Packet.UID = GC.Entity.UID;
               Packet.Type = _String.Effect;
               Packet.TextsCount = 1;
               Packet.Texts.Add("ridmatch_second");
               GC.SendScreen(Packet, true);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 3)
            {
                GC.Entity.ConquerPoints += 15000;
                GC.Entity.cp = 15000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {
                  Kernel.SendWorldMessage(new Message( "No.3          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                  Conquer_Online_Server.Network.GamePackets._String Packet = new Conquer_Online_Server.Network.GamePackets._String(true);
                  //GamePackets.poker Packet2 = new GamePackets.poker(true, client);
                  Packet.UID = GC.Entity.UID;
                  Packet.Type = _String.Effect;
                  Packet.TextsCount = 1;
                  Packet.Texts.Add("ridmatch_third");
                  GC.SendScreen(Packet, true);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 4)
            {
                GC.Entity.ConquerPoints += 14000;
                GC.Entity.cp = 14000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {
                               Kernel.SendWorldMessage(new Message( "No.4         " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                               Conquer_Online_Server.Network.GamePackets._String Packet = new Conquer_Online_Server.Network.GamePackets._String(true);
                               //GamePackets.poker Packet2 = new GamePackets.poker(true, client);
                               Packet.UID = GC.Entity.UID;
                               Packet.Type = _String.Effect;
                               Packet.TextsCount = 1;
                               Packet.Texts.Add("ridmatch_first");
                               GC.SendScreen(Packet, true);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 5)
            {
                GC.Entity.ConquerPoints += 12000;
                GC.Entity.cp = 12000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {
                                Kernel.SendWorldMessage(new Message( "No.5          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 6)
            {
                GC.Entity.ConquerPoints += 10000;
                GC.Entity.cp = 10000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {
                                Kernel.SendWorldMessage(new Message( "No.6          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 7)
            {
                GC.Entity.ConquerPoints += 8000;
                GC.Entity.cp = 8000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {
                                Kernel.SendWorldMessage(new Message( "No.7          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            }
                        }
                    }
                }

            }
            else if (Kernel.SteedTornament.cps == 8)
            {
                GC.Entity.ConquerPoints += 5000;
                GC.Entity.cp = 5000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {
                                Kernel.SendWorldMessage(new Message( "No.8          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 9)
            {
                GC.Entity.ConquerPoints += 4000;
                GC.Entity.cp = 4000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {
                               Kernel.SendWorldMessage(new Message( "No.9         " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps == 10)
            {
                GC.Entity.ConquerPoints += 3000;
                GC.Entity.cp = 3000;
                foreach (DictionaryEntry DE in Kernel.H_Chars)
                {
                    Entity Chaar = (Game.Entity)DE.Value;
                    {
                        if (Chaar != null)
                        {
                            if (Chaar.MapID == 1950)
                            {

                                Kernel.SendWorldMessage(new Message( "No.10          " + GC.Entity.Name + "      " + (DateTime.Now - GC.Entity.SteedRaceTime) + "", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                            }
                        }
                    }
                }
            }
            else if (Kernel.SteedTornament.cps > 10)
            {
                GC.Entity.ConquerPoints += 1000;
                GC.Entity.cp = 1000;
            }
        }
        public static void WaiForWin(bool mod, Client.GameState GC)
        {
            while (mod)
            {
                //  Game.Entity GC = new Game.Entity();
                try
                {
                    foreach (DictionaryEntry DE in Kernel.H_Chars)
                    {
                        Entity Chaar = (Game.Entity)DE.Value;
                        if (Chaar.MapID == 1950)
                        {
                            if (Chaar.MapID == 1950 && (Chaar.X >= 494 && Chaar.X <= 498) && Chaar.Y == 373)
                            {
                                Kernel.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                Kernel.SendWorldMessage(new Message("Server" + Chaar.Name + " has finished the steed race no " + Kernel.SteedTornament.cps + " and won " + GC.Entity.cp + " Cps", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);
                                //Game.Kernel.SendMsgToAll("Server" + Chaar.Name + " has finished the steed race no " + Kernel.SteedTornament.cps + " and won " + GC.Entity.cp + " Cps", 2011, 0, System.Drawing.Color.Red);

                            }
                            else if (Chaar.MapID == 1950 && (Chaar.X >= 494 && Chaar.X <= 498) && Chaar.Y == 372)
                            {
                                Kernel.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                             Kernel.SendWorldMessage(new Message("Server" + Chaar.Name + " has finished the steed race no " + Kernel.SteedTornament.cps + " and won " + GC.Entity.cp + " Cps", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);

                            }
                            else if (Chaar.MapID == 1950 && (Chaar.X >= 494 && Chaar.X <= 498) && Chaar.Y == 371)
                            {
                                Kernel.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                             Kernel.SendWorldMessage(new Message("Server" + Chaar.Name + " has finished the steed race no " + Kernel.SteedTornament.cps + " and won " + GC.Entity.cp + " Cps", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);

                            }
                            else if (Chaar.MapID == 1950 && (Chaar.X >= 478 && Chaar.X <= 510) && (Chaar.Y >= 346 && Chaar.Y <= 370))
                            {
                                Kernel.SteedTornament.cps += 1;
                                SteedRace.CpsFowin(GC);
                                Chaar.Teleport(1002, 431, 379);
                                             Kernel.SendWorldMessage(new Message("Server" + Chaar.Name + " has finished the steed race no " + Kernel.SteedTornament.cps + " and won " + GC.Entity.cp + " Cps", System.Drawing.Color.Orange, Message.Center), Kernel.GamePool.Values);

                            }

                        }
                    }
                }
                catch { }
            }
        }
    }
}
