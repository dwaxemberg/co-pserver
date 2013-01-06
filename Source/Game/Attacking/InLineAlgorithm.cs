using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.Attacking
{
    public class InLineAlgorithm
    {
        public enum Algorithm
        {
            DDA,
            SomeMath
        }
        public struct coords
        {
            public int X;
            public int Y;

            public coords(double x, double y)
            {
                this.X = (int)x;
                this.Y = (int)y;
            }
        }
        bool Contains(List<coords> Coords, coords Check)
        {
            foreach (coords Coord in Coords)
                if (Coord.X == Check.X && Check.Y == Coord.Y)
                    return true;
            return false;
        }
        List<coords> LineCoords(ushort userx, ushort usery, ushort shotx, ushort shoty)
        {
            return linedda(userx, usery, shotx, shoty);
        }
        void Add(List<coords> Coords, int x, int y)
        {
            coords add = new coords((ushort)x, (ushort)y);
            if (!Coords.Contains(add))
                Coords.Add(add);
        }
        List<coords> linedda(int xa, int ya, int xb, int yb)
        {
            int dx = xb - xa, dy = yb - ya, steps, k;
            float xincrement, yincrement, x = xa, y = ya;

            if (Math.Abs(dx) > Math.Abs(dy)) steps = Math.Abs(dx);
            else steps = Math.Abs(dy);

            xincrement = dx / (float)steps;
            yincrement = dy / (float)steps;
            List<coords> ThisLine = new List<coords>();
            ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));

            for (k = 0; k < MaxDistance; k++)
            {
                x += xincrement;
                y += yincrement;
                ThisLine.Add(new coords(Math.Round(x), Math.Round(y)));
            }
            return ThisLine;
        }
        List<coords> lcoords;
        public byte MaxDistance = 10;
        public InLineAlgorithm(ushort X1, ushort X2, ushort Y1, ushort Y2, byte MaxDistance, Algorithm algo)
        {
            algorithm = algo;
            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;
            if (algo == Algorithm.DDA)
                lcoords = LineCoords(X1, Y1, X2, Y2);
           
            this.MaxDistance = MaxDistance;
            Direction = (byte)ServerBase.Kernel.GetAngle(X1, Y1, X2, Y2); ;
        }
        private Algorithm algorithm;
        public ushort X1 { get; set; }
        public ushort Y1 { get; set; }
        public ushort X2 { get; set; }
        public ushort Y2 { get; set; }
        public byte Direction { get; set; }

        public bool InLine(ushort X, ushort Y)
        {
            int mydst = ServerBase.Kernel.GetDistance((ushort)X1, (ushort)Y1, X, Y);
            byte dir = (byte)ServerBase.Kernel.GetAngle(X1, Y1, X, Y);

            if (mydst <= MaxDistance)
            {
                if (algorithm == Algorithm.SomeMath)
                {

                    if (dir != Direction)
                        return false;
                    //calculate line eq
                    if (X2 - X1 == 0)
                    {
                        //=> X - X1 = 0
                        //=> X = X1
                        return X == X1;
                    }
                    else if (Y2 - Y1 == 0)
                    {
                        //=> Y - Y1 = 0
                        //=> Y = Y1
                        return Y == Y1;
                    }
                    else
                    {
                        double val1 = ((double)(X - X1)) / ((double)(X2 - X1));
                        double val2 = ((double)(Y + Y1)) / ((double)(Y2 + Y1));
                        bool works = Math.Floor(val1) == Math.Floor(val2);
                        return works;
                    }
                }
                else
                    if (algorithm == Algorithm.DDA)
                        return Contains(lcoords, new coords(X, Y));
            }
            return false;
        }
    }
}
