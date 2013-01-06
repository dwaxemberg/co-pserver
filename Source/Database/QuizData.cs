using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Game.ConquerStructures;

namespace Conquer_Online_Server.Database
{
    class QuizData
    {
        public static void Load()
        {
            {
                string[] AllLines = System.IO.File.ReadAllLines("database\\QuizShow.txt");
                int questionscount = AllLines.Length;
                for (int x = 0; x < questionscount; x++)
                {
                    string[] Info = AllLines[x].Split('#');
                    string Question = Info[0];
                    QuizShow.Answer[] Answers = new QuizShow.Answer[4];
                    Answers[0] = new QuizShow.Answer(Info[1].Split(':')[0], ushort.Parse(Info[1].Split(':')[1]));
                    Answers[1] = new QuizShow.Answer(Info[2].Split(':')[0], ushort.Parse(Info[2].Split(':')[1]));
                    Answers[2] = new QuizShow.Answer(Info[3].Split(':')[0], ushort.Parse(Info[3].Split(':')[1]));
                    Answers[3] = new QuizShow.Answer(Info[4].Split(':')[0], ushort.Parse(Info[4].Split(':')[1]));
                    QuizShow.Question Q = new QuizShow.Question(Question, Answers);
                    QuizShow.AllQuestions.Add((ushort)x, Q);
                }
                Console.WriteLine("Loaded " + QuizShow.AllQuestions.Count + " Quiz Questions");
            }
        }
    }
}
