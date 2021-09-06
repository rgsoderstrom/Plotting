using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;

namespace Plot2D_Embedded
{
    public class PointView : CanvasObject
    {
        public enum DrawingStyle {None, Circle, Square, X, Plus, Star, Triangle}

        public static bool PreserveShape {get; set;} = true; // if set false shapes of circles and squares will
                                                             // distort with changes in aspect ratio

        static Rect rect;
        static List<LineGeometry> StarLines = new List<LineGeometry> ();
        static List<LineGeometry> TriangleLines = new List<LineGeometry> ();

        //*************************************************************
        //
        // Static Constructor
        //

        static PointView ()
        {
            RotateTransform rot;

            rect = new Rect (new Point () - new Vector (1, 1) / 2, new Point () + new Vector (1, 1) / 2);

            // star lines
            double angleStep = 60;  // angle in degrees

            Point p0 = new Point (0, 1);

            for (double angle = 0; angle<360; angle += angleStep)
            {
                rot = new RotateTransform (angle);
                StarLines.Add (new LineGeometry (new Point (0, 0), rot.Transform (p0)));
            }

            // triangle lines
            rot = new RotateTransform (120);
            Point p1 = rot.Transform (p0);
            Point p2 = rot.Transform (p1);
            TriangleLines.Add (new LineGeometry (p0, p1));
            TriangleLines.Add (new LineGeometry (p1, p2));
            TriangleLines.Add (new LineGeometry (p2, p0));
        }

        //*************************************************************
        //
        // Instance Constructors
        //

        public PointView (Point pt) : this (new List<Point> () {pt}, DrawingStyle.Circle)
        {
        }

        public PointView (List<Point> pts) : this (pts, DrawingStyle.Circle)
        {
        }

        public PointView (List<Point> pts, DrawingStyle style)
        {
            GeometryGroup geomGroup = new GeometryGroup ();
            path.Data = geomGroup;

            //************************************************

            BoundingBox bb = new BoundingBox ();

            foreach (Point pt in pts)
                bb.Union (pt);

            List<Point> bbCorners = new List<Point> () {bb.TLC, bb.TRC, bb.BRC, bb.BLC};

            CalculateBB (bbCorners);

            //************************************************

            Geometry template = null; // = style == DrawingStyle.Circle ? (Geometry)new EllipseGeometry (rect) // compiler requires the casts
                                      //                       : (Geometry)new RectangleGeometry (rect);

            switch (style)
            {
                case DrawingStyle.Circle:
                    template = new EllipseGeometry (rect);
                    break;

                case DrawingStyle.Square:
                    template = new RectangleGeometry (rect);
                    break;

                case DrawingStyle.Plus:
                {
                    Vector vx = rect.TopRight - rect.TopLeft;
                    Vector vy = rect.BottomLeft - rect.TopLeft;
                    Point tm = rect.TopLeft + vx / 2; // top middle
                    Point lm = rect.TopLeft + vy / 2; // left middle

                    template = new GeometryGroup ();
                    (template as GeometryGroup).Children.Add (new LineGeometry (tm, tm + vy));
                    (template as GeometryGroup).Children.Add (new LineGeometry (lm, lm + vx));
                }
                break;

                case DrawingStyle.X:
                    template = new GeometryGroup ();
                    (template as GeometryGroup).Children.Add (new LineGeometry (rect.TopLeft, rect.BottomRight));
                    (template as GeometryGroup).Children.Add (new LineGeometry (rect.TopRight, rect.BottomLeft));
                    break;

                case DrawingStyle.Star:
                {
                    template = new GeometryGroup ();

                    foreach (LineGeometry lg in StarLines)
                        (template as GeometryGroup).Children.Add (lg);
                }
                break;

                case DrawingStyle.Triangle:
                {
                    template = new GeometryGroup ();

                    foreach (LineGeometry lg in TriangleLines)
                        (template as GeometryGroup).Children.Add (lg);
                }
                break;

                default:
                    throw new Exception ("Unsupported Point DrawingStyle: " + style);
            }

            foreach (Point pt in pts)
            {
                Geometry geometry = template.Clone ();

                geometry.Transform = new TransformGroup ();

                if (PreserveShape == true)  // always display as a circle or square whether or not AxesEqual is true
                {
                    (geometry.Transform as TransformGroup).Children.Add (AspectRatioCorrection); // identity xform initially. Axes aspect ratio
                }                                                                                // xform will be added when this is plotted

                (geometry.Transform as TransformGroup).Children.Add (scale);
                (geometry.Transform as TransformGroup).Children.Add (new TranslateTransform (pt.X, pt.Y));

                geomGroup.Children.Add (geometry);
            }

            path.Fill = Brushes.GreenYellow; // null;// Brushes.Transparent;  // null doesn't capture mouse clicks in fill region
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
        }

        public double Thickness {get {return path.StrokeThickness;}
                                 set {path.StrokeThickness = value;}}

        public double Size {get {return scale.ScaleX;}
                            set {scale.ScaleX = scale.ScaleY = value;}}

        public Brush Color {set {BorderColor = FillColor = value;}}
    }
}
