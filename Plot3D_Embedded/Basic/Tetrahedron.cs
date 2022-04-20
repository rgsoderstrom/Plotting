using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class TetrahedronGeometry : PointMarkerGeometry
    {
        public TetrahedronGeometry (Point3D pt) : base (pt)
        {
        }

        public TetrahedronGeometry (Point3D pt, double r) : base (pt, r)
        {
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class TetrahedronView : PointMarkerView
    {
        static readonly Petzold.Media3D.TetrahedronMesh mesh = new TetrahedronMesh ();

        public override PointMarkerView RefreshView ()
        {
            return new TetrahedronView (geometry);
        }

        public TetrahedronView (PointMarkerGeometry geom) : base (geom, mesh)
        {
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Tetrahedron : PointMarker
    {
        public Tetrahedron (Point3D pt)
        {
            Geometry = new TetrahedronGeometry (pt);
            PointView = new TetrahedronView (Geometry);
        }

        public Tetrahedron (Point3D pt, double radius)
        {
            Geometry = new TetrahedronGeometry (pt, radius);
            PointView = new TetrahedronView (Geometry);
        }

        // these adjust for Petzold's TetrahedronMesh being width 2, rather than width 1
        public override double Radius   {get {return base.Diameter;} set {base.Diameter = value;}}
        public override double Diameter {get {return base.Diameter * 2;} set {base.Diameter = value / 2;}}
    }
}
