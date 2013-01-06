using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
namespace Conquer_Online_Server.Game.ConquerStructures.Society
{
    public class GuildWar
    {
        public static SobNpcSpawn Pole, RightGate, LeftGate;

        public static SafeDictionary<uint, Guild> Scores = new SafeDictionary<uint, Guild>(100);

        public static bool IsWar = false, Flame10th = false, FirstRound = false;

        public static Time32 ScoreSendStamp, LastWin;

        public static Guild PoleKeeper, CurrentTopLeader;

        private static bool changed = false;

        private static string[] scoreMessages;

        public static DateTime StartTime;

        public static void Initiate()
        {
            var Map = ServerBase.Kernel.Maps[1038];
            Pole = (SobNpcSpawn)Map.Npcs[810];
            LeftGate = (SobNpcSpawn)Map.Npcs[516074];
            RightGate = (SobNpcSpawn)Map.Npcs[516075];
        }
        public static void ElitePkWar()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Elite Pk War Start every Sunday at 19:00 and terminates 20:00 at ElitePKEnvoy Twin(423,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void study()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Conqratulation someone Earn 50 StudyPoints For Killing the Boss Monster(TeratoDragon/SnowBanshe) at Dragog/Terato Map ", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void WeeklyPkWar()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Weekly ClassPk War Start at 16:05 Sign Up before 16:05 at Twin(430, 243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void ClassPkWarTrojan()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Trojan ClassPk War Start at 19:50 Sign Up around 19:45 to 19:49 at Class PkEnvoy Twin(436,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void ClassPkWarWarrior()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Warrior ClassPk War Start at 19:50 Sign Up around 19:45 to 19:49 at Class PkEnvoy Twin(436,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void ClassPkWarNinja()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Ninja ClassPk War Start at 19:50 Sign Up around 19:45 to 19:49 at Class PkEnvoy Twin(436,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void ClassPkWarWater()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Water ClassPk War Start at 19:50 Sign Up around 19:45 to 19:49 at Class PkEnvoy Twin(436,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void ClassPkWarFire()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Fire ClassPk War Start at 19:50 Sign Up around 19:45 to 19:49 at Class PkEnvoy Twin(436,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void ClassPkWarArcher()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Archer ClassPk War Start at 19:50 Sign Up around 19:45 to 19:49 at Class PkEnvoy Twin(436,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void ClassPkWarMonk()
        {
            ServerBase.Kernel.SendWorldMessage(new Message("Monk ClassPk War Start at 19:50 Sign Up around 19:45 to 19:49 at Class PkEnvoy Twin(436,243)", System.Drawing.Color.White, Message.Center), ServerBase.Kernel.GamePool.Values);
        }
        public static void Start()
        {
            Scores = new SafeDictionary<uint, Guild>(100);
            StartTime = DateTime.Now;
            LeftGate.Mesh = (ushort)(240 + LeftGate.Mesh % 10);
            RightGate.Mesh = (ushort)(270 + LeftGate.Mesh % 10);
            ServerBase.Kernel.SendWorldMessage(new Message("Guild war has began!", System.Drawing.Color.Black, Message.Center), ServerBase.Kernel.GamePool.Values);
            FirstRound = true;
            foreach (Guild guild in ServerBase.Kernel.Guilds.Values)
            {
                guild.WarScore = 0;
            }
            Update upd = new Update(true);
            upd.UID = LeftGate.UID;
            upd.Append(Update.Mesh, LeftGate.Mesh);
            upd.Append(Update.Hitpoints, LeftGate.Hitpoints);
            ServerBase.Kernel.SendWorldMessage(upd, ServerBase.Kernel.GamePool.Values, (ushort)1038);
            upd.Clear();
            upd.UID = RightGate.UID;
            upd.Append(Update.Mesh, RightGate.Mesh);
            upd.Append(Update.Hitpoints, RightGate.Hitpoints);
            ServerBase.Kernel.SendWorldMessage(upd, ServerBase.Kernel.GamePool.Values, (ushort)1038);
            IsWar = true;
        }

        public static void Reset()
        {
            Scores = new SafeDictionary<uint, Guild>(100);

            LeftGate.Mesh = (ushort)(240 + LeftGate.Mesh % 10);
            RightGate.Mesh = (ushort)(270 + LeftGate.Mesh % 10);

            LeftGate.Hitpoints = LeftGate.MaxHitpoints;
            RightGate.Hitpoints = RightGate.MaxHitpoints;
            Pole.Hitpoints = Pole.MaxHitpoints;

            Update upd = new Update(true);
            upd.UID = LeftGate.UID;
            upd.Append(Update.Mesh, LeftGate.Mesh);
            upd.Append(Update.Hitpoints, LeftGate.Hitpoints);
            ServerBase.Kernel.SendWorldMessage(upd, ServerBase.Kernel.GamePool.Values, (ushort)1038);
            upd.Clear();
            upd.UID = RightGate.UID;
            upd.Append(Update.Mesh, RightGate.Mesh);
            upd.Append(Update.Hitpoints, RightGate.Hitpoints);
            ServerBase.Kernel.SendWorldMessage(upd, ServerBase.Kernel.GamePool.Values, (ushort)1038);

            foreach (Guild guild in ServerBase.Kernel.Guilds.Values)
            {
                guild.WarScore = 0;
            }

            IsWar = true;
        }

        public static void FinishRound()
        {
            if (PoleKeeper != null && !FirstRound)
            {
                if (PoleKeeper.Wins == 0)
                    PoleKeeper.Losts++;
                else
                    PoleKeeper.Wins--;
                Database.GuildTable.UpdateGuildWarStats(PoleKeeper);
            }
            LastWin = Time32.Now;

            FirstRound = false;
            SortScores(out PoleKeeper);
            if (PoleKeeper != null)
            {
                ServerBase.Kernel.SendWorldMessage(new Message("The guild, " + PoleKeeper.Name + ", owned by " + PoleKeeper.LeaderName + " has won this guild war round!", System.Drawing.Color.Red, Message.Center), ServerBase.Kernel.GamePool.Values);
                ServerBase.Kernel.SendWorldMessage(new Message("It is generald pardon time. You have 5 minutes to leave, run for your life!", System.Drawing.Color.White, Message.TopLeft), ServerBase.Kernel.GamePool.Values, (ushort)6001);
                if (PoleKeeper.Losts == 0)
                    PoleKeeper.Wins++;
                else
                    PoleKeeper.Losts--;
                Database.GuildTable.UpdateGuildWarStats(PoleKeeper);
                Pole.Name = PoleKeeper.Name;
            }
            Pole.Hitpoints = Pole.MaxHitpoints;
            ServerBase.Kernel.SendWorldMessage(Pole, ServerBase.Kernel.GamePool.Values, (ushort)1038);
            Reset();
        }

        public static void End()
        {
            if (PoleKeeper != null)
            {
                ServerBase.Kernel.SendWorldMessage(new Message("The guild, " + PoleKeeper.Name + ", owned by " + PoleKeeper.LeaderName + " has won this guild war!---Guild war has ended!", System.Drawing.Color.Black, Message.Center), ServerBase.Kernel.GamePool.Values);
            }
            else
            {
                ServerBase.Kernel.SendWorldMessage(new Message("Guild war has ended and there was no winner!", System.Drawing.Color.Black, Message.Center), ServerBase.Kernel.GamePool.Values);
            }
            IsWar = false;
            UpdatePole(Pole);
        }

        public static void AddScore(uint addScore, Guild guild)
        {
            if (guild != null)
            {
                guild.WarScore += addScore;
                if ((int)Pole.Hitpoints <= 0)
                {
                    FinishRound();

                    return;
                }
                changed = true;
                if (!Scores.ContainsKey(guild.ID))
                    Scores.Add(guild.ID, guild);
            }
        }

        public static void SendScores()
        {
            if (scoreMessages == null)
                scoreMessages = new string[0];
            if (Scores.Count == 0)
                return;
            if (changed)
                SortScores(out CurrentTopLeader);

            for (int c = 0; c < scoreMessages.Length; c++)
            {
                Message msg = new Message(scoreMessages[c], System.Drawing.Color.Red, c == 0 ? Message.FirstRightCorner : Message.ContinueRightCorner);
                ServerBase.Kernel.SendWorldMessage(msg, ServerBase.Kernel.GamePool.Values, (ushort)1038);
                ServerBase.Kernel.SendWorldMessage(msg, ServerBase.Kernel.GamePool.Values, (ushort)6001);
            }
        }

        private static void SortScores(out Guild winner)
        {
            winner = null;
            List<string> ret = new List<string>();

            SortedDictionary<uint, SortEntry<uint, Guild>> sortdict = new SortedDictionary<uint, SortEntry<uint, Guild>>();

            foreach (Guild guild in Scores.Values)
            {
                if (!ServerBase.Kernel.Guilds.ContainsKey(guild.ID))
                    continue;

                if (sortdict.ContainsKey(guild.WarScore))
                {
                    sortdict[guild.WarScore].Values.Add(guild.ID, guild);
                }
                else
                {
                    sortdict.Add(guild.WarScore, new SortEntry<uint, Guild>());
                    sortdict[guild.WarScore].Values = new Dictionary<uint, Guild>();
                    sortdict[guild.WarScore].Values.Add(guild.ID, guild);
                }
            }
            int Place = 0;
            foreach (KeyValuePair<uint, SortEntry<uint, Guild>> entries in sortdict.Reverse())
            {
                foreach (Guild guild in entries.Value.Values.Values)
                {
                    if (Place == 0)
                        winner = guild;
                    string str = "No  " + (Place + 1).ToString() + ": " + guild.Name + "(" + entries.Key + ")";
                    ret.Add(str);
                    Place++;
                    if (Place == 4)
                        break;
                }
                if (Place == 4)
                    break;
            }

            changed = false;
            scoreMessages = ret.ToArray();
        }

        private static void UpdatePole(SobNpcSpawn pole)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE)
            .Update("sobnpcs").Set("name", pole.Name).Set("life", Pole.Hitpoints).Where("id", pole.UID).Execute();
        }
    }
}
