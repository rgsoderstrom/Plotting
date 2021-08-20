using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Text;
using System.Threading.Tasks;

namespace Plot3D_Embedded
{
    public static partial class Utils
    {
        static readonly Random rnd = new Random ();

        public static double RandomDouble (double range = 1)
        {
            return (rnd.NextDouble () - 0.5) * range;
        }

        public static Point3D RandomPoint (double range = 1)
        {
            double X1 = (rnd.NextDouble () - 0.5) * range;
            double Y1 = (rnd.NextDouble () - 0.5) * range;
            double Z1 = (rnd.NextDouble () - 0.5) * range;

            return new Point3D (X1,Y1,Z1);
        }

        public static Vector3D RandomVector (double range = 1)
        {
            double X1 = (rnd.NextDouble () - 0.5) * range;
            double Y1 = (rnd.NextDouble () - 0.5) * range;
            double Z1 = (rnd.NextDouble () - 0.5) * range;

            return new Vector3D (X1,Y1,Z1);
        }

        static public bool IsValidZ (Point3D pt)
        {
            bool nanZ = (double.IsInfinity (pt.Z) || double.IsNaN (pt.Z));
            return !nanZ;
        }

        static public bool IsValidZ (double z)
        {
            bool nanZ = (double.IsInfinity (z) || double.IsNaN (z));
            return !nanZ;
        }

    }
}
