using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class Point3DGeometry : ViewportObjectGeometry
    {
        static readonly double DefaultDiameter = 1;

        public Point3D point;

        public double Radius   {get; set;}
        public double Diameter {get {return 2 * Radius;} set {Radius = value / 2;}}

        public Point3DGeometry (Point3D pt) : this (pt, DefaultDiameter / 2)
        {
        }

        public Point3DGeometry (Point3D pt, double r)
        {
            point = pt;
            Radius = r;
            SetBoundingBox ();
        }

        internal void SetBoundingBox ()
        {
            BoundingBox.Clear ();
            BoundingBox.Union (point + Radius * new Vector3D ( 1,  0,  0));
            BoundingBox.Union (point + Radius * new Vector3D (-1,  0,  0));
            BoundingBox.Union (point + Radius * new Vector3D ( 0,  1,  0));
            BoundingBox.Union (point + Radius * new Vector3D ( 0, -1,  0));
            BoundingBox.Union (point + Radius * new Vector3D ( 0,  0,  1));
            BoundingBox.Union (point + Radius * new Vector3D ( 0,  0, -1));
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Point3DView : ModelVisual3D
    {
        protected Point3DGeometry geometry; // need to change geometry if diameter changes
        protected ScaleTransform3D scale = new ScaleTransform3D (1,1,1);

        static readonly Petzold.Media3D.SphereMesh mesh = new SphereMesh ();
        static readonly Color DefaultPointColor = Colors.LightGray;

        static Point3DView ()
        {
            mesh.Slices = 16; // 3; 
            mesh.Stacks = 16; // 3; 
            mesh.Radius = 0.5;
        }

        public Point3DView (Point3DGeometry geom)
        {
            geometry = geom;
            GeometryModel3D gm = new GeometryModel3D ();
            gm.Geometry = mesh.Geometry;
            Content = gm;
            Color = DefaultPointColor;
            scale.ScaleX = scale.ScaleY = scale.ScaleZ = geometry.Diameter;

            TranslateTransform3D xlate = new TranslateTransform3D (new Vector3D (geometry.point.X, geometry.point.Y, geometry.point.Z));

            Transform3DGroup group = new Transform3DGroup ();
            group.Children.Add (scale);
            group.Children.Add (xlate);

            Transform = group;
        }

        //***********************************************************************

        Color color;
        double opacity = 1;

        public Color Color
        {
            get {return color;}

            set
            {
                color = value;

                SolidColorBrush b = new SolidColorBrush ();
                b.Color = color;
                b.Opacity = opacity;

                (Content as GeometryModel3D).Material = (Content as GeometryModel3D).BackMaterial = new DiffuseMaterial (b);
            }
        }

        public double Opacity
        {
            get {return opacity;}

            set
            {
                opacity = value;

                SolidColorBrush b = new SolidColorBrush ();
                b.Color = color;
                b.Opacity = opacity;

                (Content as GeometryModel3D).Material = (Content as GeometryModel3D).BackMaterial = new DiffuseMaterial (b);
            }
        }

        //***********************************************************************

        public double Diameter
        {
            get {return geometry.Diameter;}

            set
            {
                geometry.Diameter = value;
                geometry.SetBoundingBox ();
                scale.ScaleX = scale.ScaleY = scale.ScaleZ = value;
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class PlottedPoint3D : ViewportObject
    {
        protected override ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        public    override BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}
        public    override ModelVisual3D View {get {return PointView as ModelVisual3D;}}
        
        public Point3DGeometry Geometry  {get; protected set;}
        public Point3DView     PointView {get; protected set;}

        public double Radius {get {return Diameter / 2;} set {Diameter = 2 * value;}}

        public double Diameter
        {
            get {return Geometry.Radius * 2;}

            set
            {
                Geometry.Radius = value / 2;
                Geometry.SetBoundingBox ();
                PointView = new Point3DView (Geometry);
            }
        }

        public PlottedPoint3D (Point3D pt)
        {
            Geometry = new Point3DGeometry (pt);
            PointView = new Point3DView (Geometry);
        }

        public PlottedPoint3D (Point3D pt, double radius)
        {
            Geometry = new Point3DGeometry (pt, radius);
            PointView = new Point3DView (Geometry);
        }
    }
}
