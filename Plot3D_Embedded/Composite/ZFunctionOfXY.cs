
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Plot3D_Embedded
{
    public class ZFunctionOfXYGeometry : ViewportObjectGeometry
    {
        public readonly List<double> xCoords;
        public readonly List<double> yCoords;
        public readonly double [,]   zValues; // [yCoords.Count, xCoords.Count]

        public ZFunctionOfXYGeometry (List<double> x, List<double> y, double [,] z)
        {
            xCoords = x;
            yCoords = y;
            zValues = z;
            SetBoundingBox ();            
        }

        internal void SetBoundingBox ()
        {
            BoundingBox.Clear ();

            for (int yc=0; yc<yCoords.Count; yc++)
            {
                for (int xc=0; xc<xCoords.Count; xc++)
                {
                    BoundingBox.Union (new Point3D (xCoords [xc], yCoords [yc], zValues [yc, xc]));
                }
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class ZFunctionOfXYView : Surface3DView
    {
        private ZFunctionOfXYGeometry geometry;

        public bool ShowTraceLines
        {
            set
            {
                if (value == true)
                    DrawTraceLines (geometry);
                else
                    Children.Clear ();
            }
        }

        public ZFunctionOfXYView (ZFunctionOfXYGeometry geom)
        {
            geometry = geom;
            double minZ = double.MaxValue;
            double maxZ = double.MinValue;

            for (int r=0; r<geom.zValues.GetLength (0); r++)
            {
                for (int c=0; c<geom.zValues.GetLength (1); c++)
                {
                    if (maxZ < geom.zValues [r, c]) maxZ = geom.zValues [r, c];
                    if (minZ > geom.zValues [r, c]) minZ = geom.zValues [r, c];
                }
            }

            LinearGradientBrush lgb = new LinearGradientBrush ();

            if (maxZ > 0 && minZ < 0)
            {
                lgb.GradientStops.Add (new GradientStop (Colors.Blue, 0));
                double d = Math.Abs (minZ) / (maxZ - minZ);
                lgb.GradientStops.Add (new GradientStop (Colors.White, d));
                lgb.GradientStops.Add (new GradientStop (Colors.Red, 1));
            }
            else if (minZ > 0) // all are positive
            {
                lgb.GradientStops.Add (new GradientStop (Colors.Pink, 0));
                lgb.GradientStops.Add (new GradientStop (Colors.Red, 1));
            }
            else // all are negative
            {
                lgb.GradientStops.Add (new GradientStop (Colors.Blue, 0));
                lgb.GradientStops.Add (new GradientStop (Colors.LightBlue, 1));
            }

            MeshGeometry3D mesh = Utils.BuildHorizontalMesh (geom.xCoords, geom.yCoords, geom.zValues);

            GeometryModel3D gm = new GeometryModel3D ();
            gm.Geometry = mesh;
            Content = gm;
                        
            DiffuseMaterial dm = new DiffuseMaterial ();
            dm.Brush = lgb;
            gm.Material = gm.BackMaterial = dm;
        }

        //
        // DrawTraceLines - coordinate-like lines that appear to be embedded in the surface
        //
        private void DrawTraceLines (ZFunctionOfXYGeometry geom)
        {
            int rows = geom.yCoords.Count;
            int cols = geom.xCoords.Count;

            int numberLines = 8; // this many lines along narrow dimension

            int xs = (int) (0.5 + (double) cols / (numberLines - 1));
            int ys = (int) (0.5 + (double) rows / (numberLines - 1));
            int step = Math.Min (xs, ys);

            //
            // pick out (x, y, z) points on the surface we want lines to pass through
            //
            List<double> meshX = new List<double> ();
            List<double> meshY = new List<double> ();

            for (int row = 0; row<rows; row+=step)
                meshY.Add (geom.yCoords [row]);

            for (int col = 0; col<cols; col+=step)
                meshX.Add (geom.xCoords [col]);

            if (meshX.Count < 2 || meshY.Count < 2)
                throw new Exception ("Exception drawing trace lines on 3D surface. Not enough lines");

            List<List<double>> meshZ = new List<List<double>> (); // double [yCoords.Count, xCoords.Count];

            for (int row = 0; row<rows; row+=step)
            {
                List<double> thisRow = new List<double> ();
                meshZ.Add (thisRow);

                for (int col = 0; col<cols; col+=step)
                {
                    thisRow.Add (geom.zValues [row, col]);
                }
            }

            //
            // normal vector at those points
            //
            Vector3D [,] normals = new Vector3D [meshY.Count, meshX.Count];

            for (int i=0; i<meshX.Count; i++)
            {
                int iNext = (i < meshX.Count - 1) ? (i + 1) : (i - 1);
                int xMult = (iNext > i) ? 1 : -1;

                for (int j=0; j<meshY.Count; j++)
                {
                    int jNext = (j < meshY.Count - 1) ? (j + 1) : (j - 1);
                    int yMult = (jNext > j) ? 1 : -1;

                    Point3D here  = new Point3D (meshX [i],     meshY [j],     meshZ [j] [i]);
                    Point3D nextX = new Point3D (meshX [iNext], meshY [j],     meshZ [j] [iNext]);
                    Point3D nextY = new Point3D (meshX [i],     meshY [jNext], meshZ [jNext] [i]);
                    Vector3D vx = (nextX - here) * xMult;
                    Vector3D vy = (nextY - here) * yMult;
                    normals [j, i] = Vector3D.CrossProduct (vx, vy);
                    normals [j, i].Normalize ();
                }
            }

            //
            // draw the lines
            //

            // d = distance of trace lines from surface
            double d = Math.Abs (meshX [1] - meshX [0]) * 0.01;

            // lines along rows
            for (int row=0; row<meshY.Count; row++)
            {
                List<Point3D> above = new List<Point3D> ();
                List<Point3D> below = new List<Point3D> ();

                for (int col=0; col<meshX.Count; col++)
                {
                    above.Add (new Point3D (meshX [col], meshY [row], meshZ [row][col]) + d * normals [row, col]);
                    below.Add (new Point3D (meshX [col], meshY [row], meshZ [row][col]) - d * normals [row, col]);
                }

                Polyline3D pl3 = new Polyline3D (above);
                pl3.PolylineView.Color = Colors.Black;
                //pl3.PolylineView.Thickness = 0.5;
                Children.Add (pl3.PolylineView);

                pl3 = new Polyline3D (below);
                pl3.PolylineView.Color = Colors.Black;
                //pl3.PolylineView.Thickness = 0.5;
                Children.Add (pl3.PolylineView);
            }

            // lines along columns
            for (int col = 0; col<cols; col+=step)
            {
                List<Point3D> above = new List<Point3D> ();
                List<Point3D> below = new List<Point3D> ();

                for (int row = 0; row<rows; row+=step)//1)
                {
                    above.Add (new Point3D (meshX [col], meshY [row], meshZ [row][col]) + d * normals [row, col]);
                    below.Add (new Point3D (meshX [col], meshY [row], meshZ [row][col]) - d * normals [row, col]);
                }

                Polyline3D pl3 = new Polyline3D (above);
                pl3.PolylineView.Color = Colors.Black;
                //pl3.PolylineView.Thickness = 0.5;
                Children.Add (pl3.PolylineView);

                pl3 = new Polyline3D (below);
                pl3.PolylineView.Color = Colors.Black;
                //pl3.PolylineView.Thickness = 0.5;
                Children.Add (pl3.PolylineView);
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class ZFunctionOfXY : ViewportObject
    {
        public ZFunctionOfXYGeometry Geometry {get; protected set;}
        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public override ModelVisual3D View {get {return FunctionView;}}
        public ZFunctionOfXYView FunctionView {get; protected set;}

        public bool ShowTraceLines {set {FunctionView.ShowTraceLines = value;}}

        public ZFunctionOfXY (List<double> xCoords, List<double> yCoords, double [,] zValues)
        {
            Geometry = new ZFunctionOfXYGeometry (xCoords, yCoords, zValues);
            FunctionView = new ZFunctionOfXYView (Geometry);
        }
    }
}
