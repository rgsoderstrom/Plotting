using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class CubeGeometry : PointMarkerGeometry
    {
        public CubeGeometry (Point3D pt) : base (pt)
        {
        }

        public CubeGeometry (Point3D pt, double r) : base (pt, r)
        {
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class CubeView : PointMarkerView
    {
        static readonly Petzold.Media3D.CubeMesh mesh = new CubeMesh ();

        public override PointMarkerView RefreshView ()
        {
            return new CubeView (geometry);
        }

        public CubeView (PointMarkerGeometry geom) : base (geom, mesh)
        {
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Cube : PointMarker
    {
        public Cube (Point3D pt)
        {
            Geometry = new CubeGeometry (pt);
            PointView = new CubeView (Geometry);
        }

        public Cube (Point3D pt, double radius)
        {
            Geometry = new CubeGeometry (pt, radius);
            PointView = new CubeView (Geometry);
        }

        // these adjust for Petzold's CubeMesh being width 2, others are width 1 ?????????
        //public override double Radius   {get {return base.Diameter;} set {base.Diameter = value;}}
        //public override double Diameter {get {return base.Diameter * 2;} set {base.Diameter = value / 2;}}
        public override double Radius   {get {return base.Diameter / 2;} set {base.Diameter = value * 2;}}
        public override double Diameter {get {return base.Diameter;} set {base.Diameter = value;}}
    }
}
