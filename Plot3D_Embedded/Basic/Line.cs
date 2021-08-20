using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class Line3DGeometry : ViewportObjectGeometry
    {
        public Point3D P0;
        public Point3D P1;

        public double   Length    {get {return (P1 - P0).Length;}}
        public Vector3D Direction {get {Vector3D N = P1 - P0;  N.Normalize (); return N;}}

        public Line3DGeometry (Point3D p, Vector3D d)
        {
            BoundingBox.Union (p);
            BoundingBox.Union (p + d);
            P0 = p;
            P1 = p + d;
        }

        public Line3DGeometry (Point3D p0, Point3D p1)
        {
            BoundingBox.Union (p0);
            BoundingBox.Union (p1);
            P0 = p0;
            P1 = p1;
        }

        protected Line3DGeometry ()
        {
            P0 = new Point3D (0, 0, 0);
            P1 = new Point3D (1, 0, 0);
            BoundingBox.Union (P0);
            BoundingBox.Union (P1);
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Line3DView : WireLine
    {
        public Line3DView (Line3DGeometry geom)
        {
            base.LineCollection.Add (geom.P0);
            base.LineCollection.Add (geom.P0 + geom.Length * geom.Direction);

            Color = DefaultLineColor;
            Thickness = DefaultLineThickness;
        }

        public class DashParameters
        {
            double on = 50;  // percent
            double off = 50;
            
            public double OnPercent  {get {return on;}  set {if (value >= 0 && value <= 100) {on = value; off = 100 - value;}}}
            public double OffPercent {get {return off;} set {if (value >= 0 && value <= 100) {off = value; on = 100 - value;}}}

            double cycles = 1;
            public double Cycles {get {return cycles;} set {if (value >= 1) cycles = value;}}
        }

        static Color DefaultLineColor = Colors.Black;
        static readonly double DefaultLineThickness = 2;

        public void SetDashParameters (DashParameters dp)
        {
            Point3D p0 = LineCollection [0];
            Point3D p1 = LineCollection [LineCollection.Count - 1];

            LineCollection.Clear ();

            // adjusting by "- dp.OffPercent / 100" makes last dash end at line's end point
            Vector3D v = (p1 - p0) / (dp.Cycles - dp.OffPercent / 100);            

            for (int i=0; i<dp.Cycles; i++)
            {
                LineCollection.Add (p0 + i * v);
                LineCollection.Add (p0 + i * v + v * dp.OnPercent / 100);
            }
        }

        //***************************************************************************************

        public double LineThickness
        {
            get {return Thickness;}
            set {Thickness = value;}            
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

                DashParameters dp = new DashParameters ();

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

                SetDashParameters (dp);
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Line3D : ViewportObject
    {
        public Line3DGeometry Geometry {get; protected set;}
        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public override ModelVisual3D View {get {return LineView;}}
        public Line3DView LineView {get; protected set;}

        public Line3D (Point3D pt, Vector3D N)
        {
            Geometry = new Line3DGeometry (pt, N);
            LineView = new Line3DView (Geometry);
        }

        public Line3D (Point3D pt1, Point3D pt2)
        {
            Geometry = new Line3DGeometry (pt1, pt2);
            LineView = new Line3DView (Geometry);
        }

        // "standard form" of line equation, Thomas P638
        public Line3D (double x0, double A, double y0, double B, double z0, double C) 
        {
            Geometry = new Line3DGeometry (new Point3D (x0,y0,z0), new Vector3D (A,B,C));
            LineView = new Line3DView (Geometry);
        }
    }
}
