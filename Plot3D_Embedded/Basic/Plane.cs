using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Plot3D_Embedded
{
    public class Plane3DGeometry : ViewportObjectGeometry
    {
        public List<Point3D> points;

        public Plane3DGeometry (List<Point3D> p)
        {
            points = new List<Point3D> (p); // make a local copy

            foreach (Point3D pt in points)
                BoundingBox.Union (pt);
        }

        //***************************************************************************************

        public Plane3DGeometry (Point3D p0, Vector3D N)
        {
            // need a vector parallel to plane, i.e. perpendicular to N. will use cross product of any other vector and N

            Vector3D v0 = new Vector3D (); // will be vector perp. to N

            List<Vector3D> candidates = new List<Vector3D> () {p0 - new Point3D (0, 0, 0), p0 - new Point3D (1, 0, 0), p0 - new Point3D (0, 1, 0)};

            foreach (Vector3D v in candidates)
            {
                Vector3D vv = Vector3D.CrossProduct (v, N);

                if (v0.Length < vv.Length)
                    v0 = vv;
            }

            if (v0.Length == 0)
            {
                throw new System.Exception ("Plane3DGeometry ctor: v0.Length == 0");
            }

            // make v0 same length at N. this scales the size of the displayed plane to the size of N
            v0.Normalize ();
            v0 *= N.Length;

            // generate 4 points in the plane
            AxisAngleRotation3D aar = new AxisAngleRotation3D (N, 90);
            RotateTransform3D rot = new RotateTransform3D (aar);
            Point3D P0 = p0 + v0;
            Point3D P1 = p0 + rot.Transform (v0);
            aar.Angle = 180;
            Point3D P2 = p0 + rot.Transform (v0);
            aar.Angle = 270;
            Point3D P3 = p0 + rot.Transform (v0);

            points = new List<Point3D> () {P0, P1, P2, P3};

            foreach (Point3D pt in points)
            {
                BoundingBox.Union (pt);
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Plane3DView : Surface3DView // ModelVisual3D
    {
        static Color DefaultPlaneColor = Colors.LightGray;

        public Plane3DView (Plane3DGeometry geom)
        {
            MeshGeometry3D mesh = new MeshGeometry3D ();

            Point3D [] planePoints = { geom.points [0], geom.points [1], geom.points [2], geom.points [3] };
            int [] planeTriangles = { 0, 1, 2, 0, 2, 3 };

            mesh.Positions = new Point3DCollection (planePoints);
            mesh.TriangleIndices = new Int32Collection (planeTriangles);

            GeometryModel3D gm = new GeometryModel3D ();
            gm.Geometry = mesh;
            Content = gm;
            Color = DefaultPlaneColor;
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Plane3D : ViewportObject
    {
        public Plane3DGeometry Geometry {get; protected set;}
        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public override ModelVisual3D View {get {return PlaneView;}}
        public Plane3DView PlaneView {get; protected set;}

        public Plane3D (Point3D pt0, Point3D pt1, Point3D pt2, Point3D pt3)
        {
            Geometry = new Plane3DGeometry (new List<Point3D> {pt0, pt1, pt2, pt3});
            PlaneView = new Plane3DView (Geometry);
        }

        public Plane3D (Point3D p0, Vector3D v0)
        {
            Geometry = new Plane3DGeometry (p0, v0);
            PlaneView = new Plane3DView (Geometry);
        }
    }
}
