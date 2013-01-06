using System.Drawing;

namespace Zoning
{
    public class Zone
    {
        Point Point1, Point2;
        Point Point3, Point4;
        public Zone(Point point1, Point point2, Point point3, Point point4)
        {
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
            Point4 = point4;
        }

        public bool IsPartOfRectangle(Point point)
        {
            if (Point1.Y >= point.Y && Point2.Y <= point.Y)
                if (point.X >= Point1.X && point.X <= Point3.X)
                    return true;
            return false;
        }
    }
}
