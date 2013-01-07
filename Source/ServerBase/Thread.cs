using System;
using System.Threading;

namespace Conquer_Online_Server.ServerBase
{
    public class Threads
    {
#if BASETHREADS
        private System.Threading.Thread base_thread;
#else
        private Timer base_timer;
#endif
        public event Action Execute;
        int Milliseconds;
        public Threads(int milliseconds)
        {
            Closed = false;
            Milliseconds = milliseconds;
        }
        public void Start()
        {
#if BASETHREADS
            base_thread = new System.Threading.Thread(new ThreadStart(Loop));
            base_thread.Start();
#else
            base_timer = new Timer(Loop, this, 2000, Milliseconds);
#endif
        }
        public bool Closed
        {
            get;
            set;
        }
#if BASETHREADS
        private void Loop()
        {
            Sleep(2000);
            while (1 > 0)
            {
                try
                {
                    if (Closed)
                        return;
                    #region ClassPk
                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Second == 00)
                    {
                        foreach (Client.GameState client in Kernel.GamePool.Values)
                        {
                            if (client.Entity.Class >= 21 && client.Entity.Class <= 25)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                                npc.OptionID = 248;
                                client.Send(npc.ToArray());
                            }
                        }
                    }

                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Second == 00)
                    {
                        foreach (Client.GameState client in Kernel.GamePool.Values)
                        {
                            if (client.Entity.Class >= 142 && client.Entity.Class <= 145)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                                npc.OptionID = 248;
                                client.Send(npc.ToArray());
                            }
                        }
                    }
                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday && DateTime.Now.Second == 00)
                    {
                        foreach (Client.GameState client in Kernel.GamePool.Values)
                        {
                            if (client.Entity.Class >= 51 && client.Entity.Class <= 55)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                                npc.OptionID = 248;
                                client.Send(npc.ToArray());
                            }
                        }
                    }
                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Thursday && DateTime.Now.Second == 00)
                    {
                        foreach (Client.GameState client in Kernel.GamePool.Values)
                        {
                            if (client.Entity.Class >= 132 && client.Entity.Class <= 135)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                                npc.OptionID = 248;
                                client.Send(npc.ToArray());
                            }
                        }
                    }
                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Second == 00)
                    {
                        foreach (Client.GameState client in Kernel.GamePool.Values)
                        {
                            if (client.Entity.Class >= 61 && client.Entity.Class <= 65)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                                npc.OptionID = 248;
                                client.Send(npc.ToArray());
                            }
                        }
                    }
                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Second == 00)
                    {
                        foreach (Client.GameState client in Kernel.GamePool.Values)
                        {
                            if (client.Entity.Class >= 41 && client.Entity.Class <= 45)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                                npc.OptionID = 248;
                                client.Send(npc.ToArray());
                            }
                        }
                    }
                    if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Second == 00)
                    {
                        foreach (Client.GameState client in Kernel.GamePool.Values)
                        {
                            if (client.Entity.Class >= 11 && client.Entity.Class <= 15)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                                npc.OptionID = 248;
                                client.Send(npc.ToArray());
                            }
                        }
                    }
                    #endregion
                    #region start lastman Standin
                    try
                    {

                        if (DateTime.Now.Hour == 12 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00
                         || DateTime.Now.Hour == 14 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 16 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 18 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 22 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 00 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00)
                        {
                            Program.LastManStanding = true;
                            foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The LastManStanding Tournament has Started! You Wana Join?");
                                npc.OptionID = 247;
                                clients.Send(npc.ToArray());
                            }
                            Console.WriteLine2("LastManStanding has started. [" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "]");
                        }
                       
                    }
                    catch { }

                    try
                    {

                        if (DateTime.Now.Hour == 12 && DateTime.Now.Minute == 03 && DateTime.Now.Second == 00
                         || DateTime.Now.Hour == 14 && DateTime.Now.Minute == 03 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 16 && DateTime.Now.Minute == 03 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 18 && DateTime.Now.Minute == 03 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 20 && DateTime.Now.Minute == 03 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 22 && DateTime.Now.Minute == 03 && DateTime.Now.Second == 00
                            || DateTime.Now.Hour == 00 && DateTime.Now.Minute == 03 && DateTime.Now.Second == 00)
                        {
                            Program.LastManStanding = false;
                            foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                            {
                                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The LastManStanding Tournament has Finished Come Next Time!");
                                npc.OptionID = 255;
                                clients.Send(npc.ToArray());
                            }
                            Console.WriteLine2("LastManStanding has finished. [" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "]");
                        }
                        
                    }
                    catch { }
                    #endregion
                    #region NobilityRankWar
                    try
                    {

                        if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                        {

                            Program.nobilityrankwar = true;
                            foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                            {
                                if (clients.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.King)
                                {
                                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The NobilityRankWar Tournament has  Started! You Wana Join?");
                                    npc.OptionID = 230;
                                    clients.Send(npc.ToArray());
                                }
                            }
                            Console.WriteLine("NobilityRankWar has  started. [" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "]");
                        }

                    }
                    catch { }
                    try
                    {

                        if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 10 && DateTime.Now.Second == 00 && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                        {
                            Program.nobilityrankwar = false;
                            foreach (Client.GameState clients in ServerBase.Kernel.GamePool.Values)
                            {
                                if (clients.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.King)
                                {
                                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The NobilityRankWar Tournament has  Closed! You Cann't Join?");
                                    npc.OptionID = 255;
                                    clients.Send(npc.ToArray());
                                }
                            }
                            Console.WriteLine("NobilityRankWar has  Closed. [" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "]");
                        }

                    }
                    catch { }
                    #endregion
                    try
                    {
                        if (Execute != null)
                            Execute.Invoke();
                    }
                    catch (Exception e)
                    {
                        Program.SaveException(e);
                        Console.WriteLine(e);
                    }
                }
                catch { }
                Sleep(Milliseconds);
                //if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 01);
            }
        }
        public void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
#else
        private void Loop(object ob)
        {
            if (Close)
            {
                base_timer.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }
            try
            {
                if (Execute != null)
                    Execute.Invoke();
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
        }
#endif
    }
}
