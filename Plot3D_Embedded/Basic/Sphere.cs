using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class SphereGeometry : PointMarkerGeometry
    {
        public SphereGeometry (Point3D pt) : base (pt)
        {
        }

        public SphereGeometry (Point3D pt, double r) : base (pt, r)
        {
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class SphereView : PointMarkerView
    {
        static readonly Petzold.Media3D.SphereMesh mesh = new SphereMesh ();

        static SphereView ()
        {
            mesh.Slices = 12; // 3; 
            mesh.Stacks = 12; // 3; 
            mesh.Radius = 0.5;
        }

        public override PointMarkerView RefreshView ()
        {
            return new SphereView (geometry);
        }

        public SphereView (PointMarkerGeometry geom) : base (geom, mesh)
        {
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Sphere : PointMarker
    {
        public Sphere (Point3D pt)
        {
            Geometry = new SphereGeometry (pt);
            PointView = new SphereView (Geometry);
        }

        public Sphere (Point3D pt, double radius)
        {
            Geometry = new SphereGeometry (pt, radius);
            PointView = new SphereView (Geometry);
        }
    }
}
