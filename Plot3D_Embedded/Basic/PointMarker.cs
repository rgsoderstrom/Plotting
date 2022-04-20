
//
// PointMarker
//  - abstract base class for objects drawn by Plot3D ()
//

using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public abstract class PointMarkerGeometry : ViewportObjectGeometry
    {
        static readonly double DefaultDiameter = 1;

        public Point3D point;

        protected double radius = DefaultDiameter / 2;

        public double Radius   {get {return radius;} set {radius = value; }}
        public double Diameter {get {return 2 * radius;} set {radius = value / 2;}}

        public PointMarkerGeometry (Point3D pt) : this (pt, DefaultDiameter / 2)
        {
        }

        public PointMarkerGeometry (Point3D pt, double r)
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

    //******************************************************************************************************

    public abstract class PointMarkerView : ModelVisual3D
    {
        public abstract PointMarkerView RefreshView ();

        protected PointMarkerGeometry geometry; // need to change geometry if diameter changes
        protected ScaleTransform3D scale = new ScaleTransform3D (1,1,1);

        readonly Petzold.Media3D.MeshGeneratorBase mesh;
        static readonly Color DefaultPointColor = Colors.LightGray;

        public PointMarkerView (PointMarkerGeometry geom, MeshGeneratorBase mgb)
        {
            mesh = mgb;

            geometry = geom;
            GeometryModel3D gm = new GeometryModel3D ();
            gm.Geometry = mesh.Geometry;
            Content = gm;
            Color = Color; // runs Color setter
            scale.ScaleX = scale.ScaleY = scale.ScaleZ = geometry.Radius * 2; // Diameter;

            TranslateTransform3D xlate = new TranslateTransform3D (new Vector3D (geometry.point.X, geometry.point.Y, geometry.point.Z));

            Transform3DGroup group = new Transform3DGroup ();
            group.Children.Add (scale);
            group.Children.Add (xlate);

            Transform = group;
        }

        Color color = DefaultPointColor;
        double opacity = 1;

        internal Color Color
        {
            get {return color;}

            set
            {
                color = value;

                SolidColorBrush b = new SolidColorBrush ();
                b.Color = color;
                b.Opacity = opacity;

                (Content as GeometryModel3D).Material = new DiffuseMaterial (b);
                (Content as GeometryModel3D).BackMaterial = null;
                //(Content as GeometryModel3D).Material = (Content as GeometryModel3D).BackMaterial = new DiffuseMaterial (b);
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
    }

    //******************************************************************************************************

    public abstract class PointMarker : ViewportObject
    {
        protected override ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        public    override BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}
        public    override ModelVisual3D View {get {return PointView as ModelVisual3D;}}
        
        public PointMarkerGeometry Geometry  {get; protected set;}
        public PointMarkerView     PointView {get; protected set;}

        public Color Color
        {
            get {return PointView.Color;}
            set {PointView.Color = value;}
        }

        public double Opacity
        {
            get {return PointView.Opacity;}
            set {PointView.Opacity = value;}
        }

        public virtual double Radius {get {return Diameter / 2;} set {Diameter = 2 * value;}}

        public virtual double Diameter
        {
            get {return Geometry.Radius * 2;}

            set
            {
                Geometry.Radius = value / 2;
                Geometry.SetBoundingBox ();
                Color wasColor = PointView.Color;
                PointView = PointView.RefreshView ();
                PointView.Color = wasColor;
            }
        }
    }
}
