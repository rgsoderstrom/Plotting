
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Plot3D_Embedded
{
    //public delegate double ddFunction  (double x);
    public delegate double dd2Function (double x, double y);
    public delegate double dd3Function (double x, double y, double z);

    public delegate void PrintFunction (string s);

    public delegate double YFromXFunction (double x);
    public delegate double ZFromXYFunction (double x, double y);
    public delegate double WFromXYZFunction (double x, double y, double z);

    public struct MinMax
    {
        public double min;
        public double max;

        public MinMax (double a, double b)
        {
            if (a < b)
            {
                min = a;
                max = b;
            }
            else
            {
                max = a;
                min = b;
            }
        }
    }

    //// pass an instance of Point3DSorter? to listOfPoint3D.Sort ()
    //public class Point3DSorterX : IComparer<Point3D> {public int Compare (Point3D p1, Point3D p2) {return p1.X > p2.X ? 1 : -1;}}
    //public class Point3DSorterY : IComparer<Point3D> {public int Compare (Point3D p1, Point3D p2) {return p1.Y > p2.Y ? 1 : -1;}}
    //public class Point3DSorterZ : IComparer<Point3D> {public int Compare (Point3D p1, Point3D p2) {return p1.Z > p2.Z ? 1 : -1;}}
}
