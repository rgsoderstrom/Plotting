using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class CartesianAxisDescription
    {
        public double min;
        public double max;
        public List<double> ticsAt;
    }

    //*********************************************************************************************

    public partial class CartesianAxesBoxGeometry : ViewportObjectGeometry
    {
        public double MaxX { get; protected set; } = 1;
        public double MinX { get; protected set; } = -1;

        public double MaxY { get; protected set; } = 1;
        public double MinY { get; protected set; } = -1;

        public double MaxZ { get; protected set; } = 1;
        public double MinZ { get; protected set; } = -1;

        public List<Point3D> bottom = new List<Point3D> ();
        public List<Point3D> top = new List<Point3D> ();

        public Point3D origin;

        public Vector3D xAxis  = new Vector3D (1, 0, 0);
        public Vector3D yAxis  = new Vector3D (0, 1, 0);
        public Vector3D zAxis  = new Vector3D (0, 0, 1);

        public CartesianAxesBoxGeometry (double minx, double maxx, double miny, double maxy, double minz, double maxz)
        {
            MinX = minx;
            MaxX = maxx;
            MinY = miny;
            MaxY = maxy;
            MinZ = minz;
            MaxZ = maxz;

            origin = new Point3D (MinX, MinY, MinZ);

            bottom.Add (new Point3D (MinX, MinY, MinZ));
            bottom.Add (new Point3D (MaxX, MinY, MinZ));
            bottom.Add (new Point3D (MaxX, MaxY, MinZ));
            bottom.Add (new Point3D (MinX, MaxY, MinZ));

            top.Add (new Point3D (MinX, MinY, MaxZ));
            top.Add (new Point3D (MaxX, MinY, MaxZ));
            top.Add (new Point3D (MaxX, MaxY, MaxZ));
            top.Add (new Point3D (MinX, MaxY, MaxZ));
            
            BoundingBox.Clear ();
            foreach (Point3D pt in bottom) BoundingBox.Union (pt);
            foreach (Point3D pt in top)    BoundingBox.Union (pt);
        }
    }

    //****************************************************************************************************


    public partial class CartesianAxesBoxView : ModelVisual3D
    {
        CartesianAxesBoxGeometry geometry;

        public CartesianAxesBoxView (CartesianAxesBoxGeometry geom, CartesianAxisDescription xDesc, CartesianAxisDescription yDesc, CartesianAxisDescription zDesc)
        {
            geometry = geom;

            double t = 0.75; // wire thickness
            Color color = Colors.Black;

            List<WireLine> lines = new List<WireLine> (12);

            List<Point3D> top = geometry.top;
            List<Point3D> bottom = geometry.bottom;

            WireLine line;
            line = new WireLine (); line.Point1 = top [0]; line.Point2 = top [1]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = top [1]; line.Point2 = top [2]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = top [2]; line.Point2 = top [3]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = top [3]; line.Point2 = top [0]; line.Color = color; line.Thickness = t; lines.Add (line);

            line = new WireLine (); line.Point1 = bottom [0]; line.Point2 = bottom [1]; line.Color = color; line.Thickness = t; lines.Add (line); // gets X axis markings
            line = new WireLine (); line.Point1 = bottom [1]; line.Point2 = bottom [2]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [2]; line.Point2 = bottom [3]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [0]; line.Point2 = bottom [3]; line.Color = color; line.Thickness = t; lines.Add (line); // gets Y axis markings

            line = new WireLine (); line.Point1 = bottom [0]; line.Point2 = top [0]; line.Color = color; line.Thickness = t; lines.Add (line); // gets Z axis markings
            line = new WireLine (); line.Point1 = bottom [1]; line.Point2 = top [1]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [2]; line.Point2 = top [2]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [3]; line.Point2 = top [3]; line.Color = color; line.Thickness = t; lines.Add (line);

            Children.Clear ();

            foreach (Visual3D l in lines)
                Children.Add (l);

            //************************************************************************************

            XAxisDecorations xAxisMarkings = new XAxisDecorations (lines [4], xDesc);
            DrawAxisTics (xAxisMarkings);
            DrawAxisText (xAxisMarkings);

            YAxisDecorations yAxisMarkings = new YAxisDecorations (lines [7], yDesc);
            DrawAxisTics (yAxisMarkings);
            DrawAxisText (yAxisMarkings);

            ZAxisDecorations zAxisMarkings = new ZAxisDecorations (lines [8], zDesc);
            DrawAxisTics (zAxisMarkings);
            DrawAxisText (zAxisMarkings);
        }

        //*******************************************************************************************

        void DrawAxisTics (AxisDecorations ad)
        {
            Point3D start = ad.hostLine.Point1;
            Vector3D axisDir = ad.hostLine.Point2 - ad.hostLine.Point1;
            axisDir.Normalize ();

            foreach (double tv in ad.ticValues)
            {
                foreach (Vector3D ticDir in ad.ticDirs)
                {
                    Point3D p1 = start + (tv - ad.startValue) * axisDir + (ad.ticLength / 2) * ticDir;
                    Point3D p2 = start + (tv - ad.startValue) * axisDir - (ad.ticLength / 2) * ticDir;

                    WireLine tic = new WireLine
                    {
                        Point1 = p1,
                        Point2 = p2,
                        Color = ad.hostLine.Color,
                        Thickness = ad.hostLine.Thickness
                    };

                    Children.Add (tic);
                }
            }
        }

        void DrawAxisText (AxisDecorations ad)
        {
            Point3D start = ad.hostLine.Point1;
            Vector3D axisDir = ad.hostLine.Point2 - ad.hostLine.Point1;
            axisDir.Normalize ();

            foreach (double tv in ad.textValues)
            {
                Point3D p1 = start + (tv - ad.startValue) * axisDir + ad.textOffset;

                Text3D txt = new Text3D (p1, ad.textDir, ad.textUp, 2 * ad.ticLength, string.Format ("{0:0.0}", tv));
                txt.TextView.Color = ad.hostLine.Color;
                Children.Add (txt.View);
            }
        }
    }

    //*******************************************************************************************

    public class CartesianAxesBox : ViewportObject
    {
        public CartesianAxesBoxGeometry Geometry;

        public override ModelVisual3D View {get {return BoxView as ModelVisual3D;}}
        public CartesianAxesBoxView BoxView {get; protected set;}

        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public CartesianAxesBox (CartesianAxisDescription xAxis, CartesianAxisDescription yAxis, CartesianAxisDescription zAxis) 
        {
            Geometry = new CartesianAxesBoxGeometry  (xAxis.min, xAxis.max, yAxis.min, yAxis.max, zAxis.min, zAxis.max);

            BoxView = new CartesianAxesBoxView (Geometry, xAxis, yAxis, zAxis);
        }
    }
}




