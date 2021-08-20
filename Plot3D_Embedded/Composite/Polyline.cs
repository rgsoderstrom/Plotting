using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;
using Common;
using CommonMath;

namespace Plot3D_Embedded
{
    public class Polyline3DGeometry : ViewportObjectGeometry
    {
        readonly public Point3DCollection Points;

        public Polyline3DGeometry (ddFunction xf, ddFunction yf, ddFunction zf, double minT, double maxT, double stepT)
        {
            if (minT > maxT)
                throw new System.Exception ("Polyline3DGeometry - minT must not be greater that maxT");

            Points = new Point3DCollection ();

            for (double t=minT; t<=maxT; t+=stepT)
            {
                Point3D pt = new Point3D (xf (t), yf (t), zf (t));
                Points.Add (pt);
                BoundingBox.Union (pt);
            }
        }

        public Polyline3DGeometry (List<Point3D> pts)
        {
            Points = new Point3DCollection (pts);

            foreach (Point3D pt in Points)
                BoundingBox.Union (pt);
        }
    }

    //********************************************************************************************************
    //********************************************************************************************************
    //********************************************************************************************************

    public class Polyline3DView : WirePolyline
    {
        static Color DefaultLineColor = Colors.Black;
        static double DefaultLineThickness = 2;

        Polyline3DGeometry geometry;

        public Polyline3DView (Polyline3DGeometry geom)
        {
            geometry = geom;
            Points = geom.Points;
            Color = DefaultLineColor;
            Thickness = DefaultLineThickness;


           

            //Children.Add (geom.BoundingBox.View);
        }

        new public Color Color
        {
            set {base.Color = value;}
        }

        new public double Thickness
        {
            set {base.Thickness = value;}
        }

        /**
        double minT = 0;
        double maxT = 1;
        double stepT = 0.1;

        public double tMin  {set {minT = value;  Draw ();} get {return minT;}}
        public double tMax  {set {maxT = value;  Draw ();} get {return maxT;}}
        public double tStep {set {stepT = value; Draw ();} get {return stepT;}}

        protected void Draw ()
        {
            if (tMin > tMax) return;
            if (tMin + tStep > tMax) return;

            Point3DCollection pts = new Point3DCollection ();

            for (double t=tMin; t<=tMax; t+=tStep)
            {
                Point3D pt = new Point3D (geometry.xFunction (t), geometry.yFunction (t), geometry.zFunction (t));
                pts.Add (pt);
            }

            Points = pts;
        }

        public Polyline3DView (Polyline3DGeometry geom)
        {
            geometry = geom;

            Points = new Point3DCollection ();

            for (double t=tMin; t<=tMax; t+=tStep)
            {
                Point3D pt = new Point3D (geom.xFunction (t), geom.yFunction (t), geom.zFunction (t));
                Points.Add (pt);
            }

            Color = DefaultLineColor;
            Thickness = DefaultLineThickness;
        }
        **/

    }

    //********************************************************************************************************
    //********************************************************************************************************
    //********************************************************************************************************

    public class Polyline3D : ViewportObject
    {
        public Polyline3DGeometry Geometry {get; protected set;}

        public override ModelVisual3D View {get {return PolylineView;}}
        public Polyline3DView PolylineView {get; protected set;}

        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public Polyline3D (ddFunction xf, ddFunction yf, ddFunction zf, double minT, double maxT, double stepT)
        {
            Geometry = new Polyline3DGeometry (xf, yf, zf, minT, maxT, stepT);
            PolylineView = new Polyline3DView (Geometry);
        }

        public Polyline3D (List<Point3D> appPoints)
        {
            Geometry = new Polyline3DGeometry (appPoints);
            PolylineView = new Polyline3DView (Geometry);
        }
    }
}















