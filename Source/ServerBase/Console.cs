using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server
{
    public static class Console
    {
        public static string Title
        {
            get
            {
                return System.Console.Title;
            }
            set
            {
                System.Console.Title = value;
            }
        }

        public static int WindowWidth
        {
            get
            {
                return System.Console.WindowWidth;
            }
            set
            {
                System.Console.WindowWidth = value; ;
            }
        }

        public static int WindowHeight
        {
            get
            {
                return System.Console.WindowHeight;
            }
            set
            {
                System.Console.WindowHeight = value; ;
            }
        }

        public static void WriteLine(object line)
        {
            if (line.ToString() == "" || line.ToString() == " ")
                System.Console.WriteLine();
            else
            {
                ForegroundColor = ConsoleColor.Red;
                System.Console.Write(TimeStamp() + " ");
                ForegroundColor = ConsoleColor.White;
                System.Console.Write(line + "\n");
            }
        }
        public static void WriteLine2(object line)
        {
            if (line.ToString() == "" || line.ToString() == " ")
                System.Console.WriteLine();
            else
            {
                ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write(TimeStamp() + " ");
                ForegroundColor = ConsoleColor.White;
                System.Console.Write(line + "\n");
            }
        }
        public static void WriteLine()
        {
            System.Console.WriteLine();
        }

        public static string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public static ConsoleColor BackgroundColor
        {
            get
            {
                return System.Console.BackgroundColor;
            }
            set
            {
                System.Console.BackgroundColor = value;
            }
        }

        public static void Clear()
        {
            System.Console.Clear();
        }

        public static ConsoleColor ForegroundColor
        {
            get
            {
                return System.Console.ForegroundColor;
            }
            set
            {
                System.Console.ForegroundColor = value;
            }
        }

        public static string TimeStamp()
        {
            DateTime Now = DateTime.Now;
            return "[" + Now.ToString("hh:mm:ss") + "]";
        }
    }
}
