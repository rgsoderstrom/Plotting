//using System;
//using System.Collections.Generic;
//using System.Windows.Media;
//using System.Windows.Media.Media3D;

//using Petzold.Media3D;
//using Common;

////
//// Cylinder types:
////      1. Constant circular cross section, straight axis
////

//namespace Plot3D_Embedded
//{
//    public class Cylinder3DGeometry : ViewportObjectGeometry
//    {
//        //
//        // constant circular cylinder in standard orientation
//        //      - circular cross section in x-y plane, extends along z axis
//        //
//        public readonly double MinZ, MaxZ, Radius;

//        public Cylinder3DGeometry (double z1, double z2, double radius)
//        {
//            MinZ = Math.Min (z1, z2);
//            MaxZ = Math.Max (z1, z2);
//            Radius = radius;
//        }
//    }

//    //****************************************************************************************************
//    //****************************************************************************************************
//    //****************************************************************************************************

//    public class Cylinder3DView : Surface3DView
//    {
//        static Color DefaultPlaneColor = Colors.LightGray;

//        static int DefaultNumberLayers = 5;  // Z range divided into this many layers.

//        static int DefaultNumberSlices = 16; // number of pie slices around a circle  

//        public Cylinder3DView (Cylinder3DGeometry geom, int numberLayers, int numberSlices)
//        {
//            try
//            {
//                if (numberLayers < 1)
//                    throw new Exception ("Cylinder3DView: numberLayers must be 1 or greater");

//                if (numberSlices < 3)
//                    throw new Exception ("Cylinder3DView: numberSlices must be 3 or greater");

//                Model3DGroup grp = new Model3DGroup ();
//                Content = grp;

//                double ZStep = (geom.MaxZ - geom.MinZ) / numberLayers;

//                double thetaStep = (2 * Math.PI) / numberSlices;



//                for (int i = 0; i<geom.Sections.Count; i++)
//                {
//                    GeometryModel3D gm = new GeometryModel3D ();
//                    grp.Children.Add (gm);

//                    gm.Geometry = Plot3D.BuildVerticalMesh (geom.Sections [i].XDomain.min, geom.Sections [i].XDomain.max,
//                                                            geom.Sections [i].YRange,
//                                                            geom.ZRange.min, geom.ZRange.max);
//                }

//                Color = DefaultPlaneColor;

//                // add outlines. these are Visuals, so they will be Children of base ModelVisual3D class

//                WirePolyline wp = new WirePolyline (); // derived from ModelVisual3D
//                wp.Points = new Point3DCollection ();
//                wp.Color = Colors.Black;
//                wp.Thickness = 2;
//                Children.Add (wp);

//                int s = 0;

//                int xSamples = 100;
//                double xMin = geom.Sections [s].XDomain.min;
//                double xMax = geom.Sections [s].XDomain.max;
//                double dx = (xMax - xMin) /  (xSamples - 1);

//                for (int i = 0; i<xSamples; i++)
//                {
//                    double x = xMin + i * dx;
//                    double y = geom.Sections [s].YRange (x);
//                    wp.Points.Add (new Point3D (x, y, geom.ZRange.max + 0.001));
//                }
//            }

//            catch (Exception ex)
//            {
//                EventLog.WriteLine ("Cylinder3DView Exception: " + ex.Message);
//            }
//        }
//    }

//    //****************************************************************************************************
//    //****************************************************************************************************
//    //****************************************************************************************************

//    public class Cylinder3D : ViewportObject
//    {
//        public Cylinder3DGeometry Geometry {get; protected set;}
//        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
//        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

//        public override ModelVisual3D View {get {return CylinderView;}}
//        public Line3DView CylinderView {get; protected set;}

//        //*********************************************************************************

//        public Cylinder3D (double minZ, double maxZ, double radius)
//        {
//            Geometry = new Cylinder3DGeometry (minZ, maxZ, radius);

//            Geometry.ZRange.min = minZ;
//            Geometry.ZRange.max = maxZ;

//            CylinderSectionGeometry s1 = new CylinderSectionGeometry (minX1, maxX1, yFromX1);
//            Geometry.Sections.Add (s1);

//            View = new Cylinder3DView (Geometry);
//        }
//    }
//}


///**************/






