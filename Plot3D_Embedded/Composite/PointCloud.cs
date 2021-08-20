using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

using Petzold.Media3D;
using Common;

namespace Plot3D_Embedded
{
    public class PointCloud3DGeometry : ViewportObjectGeometry
    {
        static readonly double DefaultDiameter = 1;

        public List<Point3D> points;
        public double radius;

        public PointCloud3DGeometry (List<Point3D> pts) : this (pts, DefaultDiameter / 2)
        {
        }

        public PointCloud3DGeometry (List<Point3D> pts, double r)
        {
            points = pts;
            radius = r;
            SetBoundingBox ();
        }

        internal void SetBoundingBox ()
        {
            BoundingBox.Clear ();

            foreach (Point3D point in points)
            {
                BoundingBox.Union (point + radius * new Vector3D ( 1,  0,  0));
                BoundingBox.Union (point + radius * new Vector3D (-1,  0,  0));
                BoundingBox.Union (point + radius * new Vector3D ( 0,  1,  0));
                BoundingBox.Union (point + radius * new Vector3D ( 0, -1,  0));
                BoundingBox.Union (point + radius * new Vector3D ( 0,  0,  1));
                BoundingBox.Union (point + radius * new Vector3D ( 0,  0, -1));
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class PointCloud3DView : ModelVisual3D
    {
        protected PointCloud3DGeometry geometry; // need to change geometry if diameter changes
        protected ScaleTransform3D scale = new ScaleTransform3D (1,1,1);

        protected DiffuseMaterial pointMaterial = new DiffuseMaterial (new SolidColorBrush (DefaultPointColor));

        static Petzold.Media3D.SphereMesh mesh = new SphereMesh ();
        static Color DefaultPointColor = Colors.LightGray;

        static PointCloud3DView ()
        {
            mesh.Slices = 8;// 16;
            mesh.Stacks = 8;// 16;
            mesh.Radius = 0.5;
        }

        public PointCloud3DView (PointCloud3DGeometry geom)
        {
            try
            {
                geometry = geom;

                Content = new Model3DGroup ();

                foreach (Point3D pt in geom.points)
                {
                    GeometryModel3D gm = new GeometryModel3D ();
                    gm.Geometry = mesh.Geometry;

                    gm.Material = pointMaterial;
                    gm.BackMaterial = null;
                    //gm.Material = new DiffuseMaterial (new SolidColorBrush (color));
                    //gm.BackMaterial = new DiffuseMaterial (Brushes.Red);

                    TranslateTransform3D xlate = new TranslateTransform3D (new Vector3D (pt.X, pt.Y, pt.Z));
                    Transform3DGroup group = new Transform3DGroup ();
                    group.Children.Add (scale);
                    group.Children.Add (xlate);
                    gm.Transform = group;

                    (Content as Model3DGroup).Children.Add (gm);
                }

                //Children.Add (geometry.BoundingBox.View);
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("PointCloud Exception: {0}", ex.Message));
            }
        }

        //***********************************************************************

        Color color = DefaultPointColor;

        public Color Color
        {
            get {return color;}

            set
            {
                color = value;
                pointMaterial = new DiffuseMaterial (new SolidColorBrush (color));

                foreach (GeometryModel3D gm in (Content as Model3DGroup).Children)
                    gm.Material = pointMaterial;
            }
        }

        //***********************************************************************

        public double Diameter
        {
            get {return geometry.radius * 2;}

            set
            {
                geometry.radius = value / 2;
                geometry.SetBoundingBox ();
                scale.ScaleX = scale.ScaleY = scale.ScaleZ = value;
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class PointCloud3D : ViewportObject
    {
        protected override ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        public    override BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}
        public    override ModelVisual3D View {get {return PointView as ModelVisual3D;}}
        
        public PointCloud3DGeometry Geometry  {get; protected set;}
        public PointCloud3DView     PointView {get; protected set;}

        public PointCloud3D (List<Point3D> pts)
        {
            Geometry = new PointCloud3DGeometry (pts);
            PointView = new PointCloud3DView (Geometry);
        }
    }
}










