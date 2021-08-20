using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;
using System.Net.NetworkInformation;
using System.Windows.Media.TextFormatting;
using System.IO.IsolatedStorage;

namespace Plot3D_Embedded
{
    public partial class CartesianAxes3DGeometry : ViewportObjectGeometry
    {
        public class Local
        {
            static readonly public Point3D  origin = new Point3D  (0, 0, 0);
            static readonly public Vector3D xAxis  = new Vector3D (1, 0, 0);
            static readonly public Vector3D yAxis  = new Vector3D (0, 1, 0);
            static readonly public Vector3D zAxis  = new Vector3D (0, 0, 1);

            //
            // this instance, axes extents in local coordinates
            //
            public double MinX = -1;
            public double MaxX =  2;
            public double MinY = -1;
            public double MaxY =  2;
            public double MinZ = -1;
            public double MaxZ =  2;
        }

        public class World
        {
            readonly Local local;
            readonly Transform3DGroup localToWorld;

            public Point3D Origin {get {return localToWorld.Transform (Local.origin);}}

            public Point3D MinX { get { return localToWorld.Transform (Local.origin + local.MinX * Local.xAxis); } }
            public Point3D MaxX { get { return localToWorld.Transform (Local.origin + local.MaxX * Local.xAxis); } }
            public Point3D MinY { get { return localToWorld.Transform (Local.origin + local.MinY * Local.yAxis); } }
            public Point3D MaxY { get { return localToWorld.Transform (Local.origin + local.MaxY * Local.yAxis); } }
            public Point3D MinZ { get { return localToWorld.Transform (Local.origin + local.MinZ * Local.zAxis); } }
            public Point3D MaxZ { get { return localToWorld.Transform (Local.origin + local.MaxZ * Local.zAxis); } }

            public World (Transform3DGroup ltw, Local loc)
            {
                local = loc;
                localToWorld = ltw;
            }
        }

        public Transform3DGroup Rotation;
        public TranslateTransform3D Translation;

        public Transform3DGroup LocalToWorld { get; set; } = new Transform3DGroup ();
        public Transform3DGroup WorldToLocal { get { return LocalToWorld.Inverse as Transform3DGroup; } }

        public Local localCoords = new Local ();
        public World worldCoords;

        internal CartesianAxes3DGeometry (Point3D origin,  // in world coords
                                          EulerAngles.Convention axes,
                                          double Angle1, double Angle2, double Angle3)
        {
            Rotation = EulerAngles.Rotation (Angle1, Angle2, Angle3, axes);
            Translation = new TranslateTransform3D (origin.X, origin.Y, origin.Z);

            LocalToWorld.Children.Add (Rotation);
            LocalToWorld.Children.Add (Translation);

            worldCoords = new World (LocalToWorld, localCoords);
            CalculateBoundingBox ();
        }

        internal void CalculateBoundingBox ()
        {
            BoundingBox.Clear ();
            BoundingBox.Union (worldCoords.Origin);
            BoundingBox.Union (worldCoords.MinX);
            BoundingBox.Union (worldCoords.MaxX);
            BoundingBox.Union (worldCoords.MinY);
            BoundingBox.Union (worldCoords.MaxY);
            BoundingBox.Union (worldCoords.MinZ);
            BoundingBox.Union (worldCoords.MaxZ);
        }
    }

    //******************************************************************************

    public partial class CartesianAxes3DView : ModelVisual3D
    {
        CartesianAxes3DGeometry geometry;

        int NumberXTics {get; set;} = 6;
        int NumberYTics {get; set;} = 6;
        int NumberZTics {get; set;} = 6;

        Line3D xAxis = null;
        Line3D yAxis = null;
        Line3D zAxis = null;               
            
        public Line3DView.DashParameters dp = new Line3DView.DashParameters ();

        public CartesianAxes3DView (CartesianAxes3DGeometry geom)
        {
            geometry = geom;

            xAxis = new Line3D (geometry.worldCoords.MinX, geometry.worldCoords.MaxX);
            yAxis = new Line3D (geometry.worldCoords.MinY, geometry.worldCoords.MaxY);
            zAxis = new Line3D (geometry.worldCoords.MinZ, geometry.worldCoords.MaxZ);

            xAxis.LineView.Color = Colors.Red;
            yAxis.LineView.Color = Colors.Green;
            zAxis.LineView.Color = Colors.Blue;

            xAxis.LineView.Thickness =
            yAxis.LineView.Thickness =
            zAxis.LineView.Thickness = 3;

            Children.Add (xAxis.View);
            Children.Add (yAxis.View);
            Children.Add (zAxis.View);

            dp.OnPercent = 100; // solid line

            //Children.Add (geometry.BoundingBox.View);
        }

        //***************************************************************************************

        private double thickness = 3;

        public double LineThickness
        {
            get {return thickness;}
            set {thickness = value; xAxis.LineView.Thickness = yAxis.LineView.Thickness = zAxis.LineView.Thickness = thickness;}            
        }

        //***************************************************************************************

        public enum LineStyles {Solid, LongDash, ShortDash, Dot};

        LineStyles style = LineStyles.Solid;

        public LineStyles LineStyle
        {
            get {return style;}

            set
            {
                style = value;

                switch (style)
                {
                    case LineStyles.Solid:
                        dp.OnPercent = 100;
                        dp.Cycles = 1;
                        break;

                    case LineStyles.LongDash:
                        dp.OnPercent = 70;
                        dp.Cycles = 10;
                        break;

                    case LineStyles.ShortDash:
                        dp.OnPercent = 30;
                        dp.Cycles = 10;
                        break;

                    case LineStyles.Dot:
                        dp.OnPercent = 10;
                        dp.Cycles = 30;
                        break;
                }

                xAxis.LineView.SetDashParameters (dp);
                yAxis.LineView.SetDashParameters (dp);
                zAxis.LineView.SetDashParameters (dp);
            }
        }
    }

    //****************************************************************************************************************
    //****************************************************************************************************************
    //****************************************************************************************************************

    public partial class CartesianAxes3D : ViewportObject
    {
        public CartesianAxes3DGeometry Geometry;
        public Point3D Origin {get {return Geometry.worldCoords.Origin;}}

        public override ModelVisual3D View {get {return AxesView as ModelVisual3D;}}
        public CartesianAxes3DView AxesView {get; protected set;}

        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        //*************************************************************************************************

        public CartesianAxes3D () : this (new Point3D (0, 0, 0))
        {
        }

        public CartesianAxes3D (Point3D origin) : this (origin, EulerAngles.Convention.Fixed, 0, 0, 0)
        {
        }

        public CartesianAxes3D (Point3D origin, EulerAngles.Convention eulerConvention,                                  
                                double angle1, double angle2, double angle3)
        {
            Geometry = new CartesianAxes3DGeometry (origin, eulerConvention, angle1, angle2, angle3);
            AxesView = new CartesianAxes3DView (Geometry);
        }

        public double XMax {get {return Geometry.localCoords.MaxX;} set {Geometry.localCoords.MaxX = value; AxesView.RedrawX (); Geometry.CalculateBoundingBox ();}}
        public double XMin {get {return Geometry.localCoords.MinX;} set {Geometry.localCoords.MinX = value; AxesView.RedrawX (); Geometry.CalculateBoundingBox ();}}

        public double YMax {get {return Geometry.localCoords.MaxY;} set {Geometry.localCoords.MaxY = value; AxesView.RedrawY (); Geometry.CalculateBoundingBox ();}}
        public double YMin {get {return Geometry.localCoords.MinY;} set {Geometry.localCoords.MinY = value; AxesView.RedrawY (); Geometry.CalculateBoundingBox ();}}

        public double ZMax {get {return Geometry.localCoords.MaxZ;} set {Geometry.localCoords.MaxZ = value; AxesView.RedrawZ (); Geometry.CalculateBoundingBox ();}}
        public double ZMin {get {return Geometry.localCoords.MinZ;} set {Geometry.localCoords.MinZ = value; AxesView.RedrawZ (); Geometry.CalculateBoundingBox ();}}
    }
}







