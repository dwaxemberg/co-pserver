using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.Features.ClassPKWar
{
    public class ClassPKWar
    {
        public static bool Started = false;
        public static bool Running = false;
        public static Time32 LastCheck;
        public static Time32 RoundStarted;
        public static Time32 RoundNext;
        public static string Winner = "";
        public static string Class = "Trojan";
        public static Dictionary<uint, SignedPlayer> Players;

        public static Client.GameState GetPlayer(SignedPlayer s)
        {
            if (Program.SafeReturn().ContainsKey(s.UID))
                return Program.SafeReturn()[s.UID];

            return null;
        }

        public static void Check()
        {
            if (Time32.Now > LastCheck.AddMinutes(1))
            {
                LastCheck = Time32.Now;
                if (!Running)
                {
                    bool Allow = false;
                    switch (DateTime.Now.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            {
                                if (DateTime.Now.Hour == 16 && DateTime.Now.Minute >= 55 && !Started)
                                {
                                    Class = "Trojan";
                                }
                                else if (Started && Time32.Now > RoundNext)
                                {
                                    Running = true;
                                }
                                break;
                            }
                    }

                    if (Allow)
                    {
                        Start();
                    }
                }
                else
                {
                    if (Players.Count == 1)
                        End();
                    if (Players.Count == 0)
                        End();
                }
            }
        }

        public static void Start()
        {
            RoundStarted = Time32.Now;
            RoundNext = Time32.Now.AddMinutes(15);
            Started = true;
            //PacketHandler.WorldMessage("The class PK War will start in 15 minutes for " + Class + "'s today. Let's see who is the strongest!", Message.Center, System.Drawing.Color.White);
        }

        public static void End()
        {
            SignedPlayer w = null;
            foreach (SignedPlayer s in Players.Values)
            {
                if (w != null)
                {
                    if (s.Kills > w.Kills)
                        w = s;
                }
                else w = s;
            }

            if (w != null)
            {
                Client.GameState client = GetPlayer(w);
                Started = false;
                Running = false;
              //  PacketHandler.WorldMessage("Congratulations! " + client.Entity.Name + " has won the " + Class + "'s PkWar and won 5 ExpBalls as reward!", Message.TopLeft, System.Drawing.Color.White);
                client.IncreaseExperience((ulong)(client.ExpBall * 5),true);
            }
            else
            {
                Started = false;
                Running = false;
              //  PacketHandler.WorldMessage("The class PkWar has ended with no winner!", Message.Center, System.Drawing.Color.White);
            }
        }
    }

    public class SignedPlayer
    {
        public uint UID;
        public byte Level;
        public byte Class;
        public int Kills;
    }
}
