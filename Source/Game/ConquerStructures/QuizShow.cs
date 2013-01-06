using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.ServerBase;
using Conquer_Online_Server.Client;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class QuizShow
    {
        public static Dictionary<ushort, Question> AllQuestions = new Dictionary<ushort, Question>();
        public static Dictionary<ushort, Question> Questions = new Dictionary<ushort, Question>();
        static System.Timers.Timer Timer;
        public static ushort QuestionNO = 0;
        public static QuizShowScore[] Scores = new QuizShowScore[500];
        public static bool QuizON = false;
        public static bool QuizON1 = true;
        static List<int> Container;
        public static void Start()
        {
            QuizON = true;
            Random Rand = new Random();
            Container = new List<int>();
            for (int x = 0; x < 20; x++)
            {
            Again:
                int Gen = Rand.Next(0, 70);
                if (!Container.Contains(Gen))
                {
                    Question Q = (Question)AllQuestions[(ushort)Gen];
                    Answer[] RealAnswers = new Answer[4];
                    bool[] Set = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {
                        int e = Rand.Next(0, 4);
                        while (Set[e])
                            e = Rand.Next(0, 4);
                        RealAnswers[i] = Q.Answers[e];
                        Set[e] = true;
                    }
                    Q.Answers = RealAnswers;
                    Questions.Add((ushort)x, Q);
                    Container.Add(Gen);
                }
                else
                    goto Again;
            }
            Timer = new System.Timers.Timer();
            Timer.Interval = 30000;
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            foreach (GameState client in Kernel.GamePool.Values)
            {
                client.Send(new Conquer_Online_Server.Network.GamePackets.QuizInfo().StartBuffer());
                client.QuizInfo = new Info();
            }
            Scores = new QuizShowScore[500];
            for (int x = 0; x < 501; x++)
            { try { Scores[x] = new QuizShowScore(); } catch { continue; } }
            Kernel.MainQuiz = new MainInfo();
            Juststarted = true;
            QuestionNO = 0;
            Timer.Start();
            Timer.AutoReset = true;

        }
        public static void Stop()
        {
            Juststarted = false;
            QuizON = false;
            Timer.AutoReset = false;
        }
        static bool Juststarted = false;

        static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Juststarted)
            {
                foreach (GameState client in Kernel.GamePool.Values)
                {
                    string[] QA = new string[5];
                    QA[0] = Questions[0];
                    QA[1] = Questions[0].Answers[0];
                    QA[2] = Questions[0].Answers[1];
                    QA[3] = Questions[0].Answers[2];
                    QA[4] = Questions[0].Answers[3];
                   // string[] QA = new string[4] { Questions[0], Questions[0].Answers[0], Questions[0].Answers[1],Questions[0].Answers[2],Questions[0].Answers[3]};
                    client.Send(new Conquer_Online_Server.Network.GamePackets.QuizInfo().QuestionBuffer(0, 0, 0, 1, QA));
                    client.QuizInfo.LastAnswer = Environment.TickCount;
                }
                QuestionNO++;
                Juststarted = false;
            }
            else
            {
                if(QuizShow.Questions.ContainsKey((ushort)QuestionNO))
                {
                    ushort nextq = QuestionNO;
                    nextq++;
                    foreach (GameState client in Kernel.GamePool.Values)
                    {
                        string[] QA = new string[5];
                        QA[0] = Questions[0];
                        QA[1] = Questions[0].Answers[0];
                        QA[2] = Questions[0].Answers[1];
                        QA[3] = Questions[0].Answers[2];
                        QA[4] = Questions[0].Answers[3];
                        client.Send(new Conquer_Online_Server.Network.GamePackets.QuizInfo().QuestionBuffer(client.QuizInfo.Score, client.QuizInfo.Time,
                            (ushort)(800 * client.QuizInfo.Score / 100), 1, QA));
                        client.QuizInfo.LastAnswer = Environment.TickCount;
                    }
                }
                else
                {
                    try
                    {
                        foreach (GameState client in Kernel.GamePool.Values)
                        {
                           client.Send(new Conquer_Online_Server.Network.GamePackets.QuizInfo().EndBuffer(client.Entity.Name, client.QuizInfo.Score, client.QuizInfo.Time, client.QuizInfo.Rank, (ushort)(800 * client.QuizInfo.Score / 100)));
                           // Chr.MyClient.AddSend(Packets.QuizShowEnd(Chr.Name, Chr.QuizShowInfo.Score, Chr.QuizShowInfo.Time, Chr.QuizShowInfo.Rank, (ushort)(800 * Chr.QuizShowInfo.Score / 100)));
                        }
                        Timer.Elapsed -= Timer_Elapsed;
                        Timer.Stop();
                        Timer.Dispose();
                    }
                    catch { }
                    QuizON = false;
                }
                QuestionNO++;
            }

        }
        public class QuizShowScore
        {
            public uint EntityID = 0;
            public uint Score = 0;
        }
        public class MainInfo
        {
            public string[] Name;
            public ushort[] Time;
            public ushort[] Score;
            public MainInfo()
            {
                Name = new string[3];
                Time = new ushort[3];
                Score = new ushort[3];
                Name[0] = "none";
                Name[1] = "none";
                Name[2] = "none";
                Time[0] = 0;
                Time[1] = 0;
                Time[2] = 0;
                Score[0] = 0;
                Score[1] = 0;
                Score[2] = 0;
            }
        }
        public class Info
        {
            public int LastAnswer;
            public ushort Time;
            public ushort Score;
            public ushort QNo;
            public byte[] Answers;
            public ushort Rank;
            public Info()
            {
                LastAnswer = 0;
                Time = 0;
                Score = 0;
                QNo = 1;
                Answers = new byte[Questions.Count];
                Rank = 0;
            }
        }
        public class Question
        {
            public string m_Question;
            public Answer[] Answers;
            public Question(string question, Answer[] answers)
            {
                m_Question = question;
                Answers = answers;
            }
            public static implicit operator string(Question q)
            {
                return q.m_Question;
            }
        }
        public class Answer
        {
            public string m_Answer;
            public ushort Points;
            public Answer(string answer, ushort points)
            {
                m_Answer = answer; Points = points;
            }
            public static implicit operator string(Answer q)
            {
                return q.m_Answer;
            }
        }
    }
}
