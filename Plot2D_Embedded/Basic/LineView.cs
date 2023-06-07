using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;

namespace Plot2D_Embedded
{
    public class LineView : CanvasObject
    {
        //**************************************************************
        //
        // Define template for arrowheads
        //

        static Point p0 = new Point (-1, -0.8);
        static Point p1 = new Point (0, 0);
        static Point p2 = new Point (-1, 0.8);

        static readonly List<Point> arrowheadTemplate = new List<Point> () {p0, p1, p2};

        PathGeometry lineGeometry = null;
        List<Point> LinePoints = null; 

        //*************************************************************

        public Point StartPoint {get {return LinePoints [0];}}
        public Point EndPoint   {get {return LinePoints [LinePoints.Count - 1];}}

        public Point PointAtOffset (double offset) // offset from 0 -> 1
        {
            if (offset < 0) offset = 0;
            if (offset > 1) offset = 1;

            double count = LinePoints.Count * offset;
            int index = (int) Math.Floor (count);
            double fract = count - index;

            if (index < LinePoints.Count - 1)
                return LinePoints [index] + fract * (LinePoints [index + 1] - LinePoints [index]);

            return EndPoint;
        }

        //*************************************************************
        //
        // Instance Constructors
        //

        public LineView (Point p1, Point p2) : this (new List<Point> () { p1, p2 })
        {
        }

        public LineView (List<Point> pts)
        {
            if (pts.Count < 2)
                throw new Exception ("LineView: line must have at least 2 points");

            LinePoints = pts; // save a copy

            lineGeometry = new PathGeometry ();
            path.Data = lineGeometry;
            path.StrokeThickness = DefaultLineThickness;

            path.Stroke = Brushes.Gray; // line itself and outline of any arrowheads
            path.Fill = path.Stroke;    // optional fill color for arrowheads

            //********************************************************************************************
            //
            // the line itself
            //

            BoundingBox bb = new BoundingBox (); // for this object

            foreach (Point pt in pts)
                bb.Union (pt);

            List<Point> bbCorners = new List<Point> () { bb.TLC, bb.TRC, bb.BRC, bb.BLC };

            CalculateBB (bbCorners); // add this object's bounding box to the one for the
                                     // entire canvas

            PathFigure fig0 = new PathFigure ();
            PolyLineSegment seg1 = new PolyLineSegment (LinePoints, true);

            fig0.StartPoint = seg1.Points [0];
            fig0.Segments.Add (seg1);
            fig0.IsFilled = false;

            lineGeometry.Figures.Add (fig0);
        }

        //********************************************************************************************
        //
        // Arrowheads at ends of line
        //                        

        PathFigure ArrowheadAtStart_figure = null;
        PathFigure ArrowheadAtEnd_figure = null;

        public bool ArrowheadAtEnd
        {
            set 
            {
                if (value == true)
                {
                    if (ArrowheadAtStart_figure == null && ArrowheadAtEnd_figure == null)
                        ArrowheadCommon ();

                    if (ArrowheadAtEnd_figure == null)
                        MakeArrowheadAtEnd ();

                    if (ArrowheadAtEnd == false) // don't add if already there
                        lineGeometry.Figures.Add (ArrowheadAtEnd_figure); 
                }
                else
                {
                    lineGeometry.Figures.Remove (ArrowheadAtEnd_figure);
                }
            }

            get
            {
                return lineGeometry.Figures.Contains (ArrowheadAtEnd_figure);
            }
        }

        public bool ArrowheadAtStart
        {
            set 
            {
                if (value == true)
                {
                    if (ArrowheadAtStart_figure == null && ArrowheadAtEnd_figure == null)
                        ArrowheadCommon ();

                    if (ArrowheadAtStart_figure == null)
                        MakeArrowheadAtStart ();

                    if (ArrowheadAtStart == false) // don't add if already there
                        lineGeometry.Figures.Add (ArrowheadAtStart_figure); 
                }
                else
                {
                    lineGeometry.Figures.Remove (ArrowheadAtStart_figure);
                }
            }

            get
            {
                return lineGeometry.Figures.Contains (ArrowheadAtStart_figure);
            }
        }

        //********************************************************************************************
        //
        // Arrow head at either end of line
        //
        double scaleFactor = 1 / 10.0;

        public double ArrowheadScaleFactor 
        { 
            set 
            {
                if (value < 0.001) value = 0.001;
               // if (value > 1) value = 1;

                bool startArrow = ArrowheadAtStart;
                bool endArrow = ArrowheadAtEnd;

                scaleFactor = value; 
                ArrowheadAtStart = ArrowheadAtEnd = false; 
                ArrowheadCommon (); 
                MakeArrowheadAtStart (); 
                MakeArrowheadAtEnd (); 
                
                if (startArrow) ArrowheadAtStart = true; 
                if (endArrow) ArrowheadAtEnd = true; 
            } 

            get {return scaleFactor;}
        }

        //*****************************************************************************************

        void ArrowheadCommon ()
        {
            scale = new ScaleTransform (scaleFactor, scaleFactor / 2); 
        }

        //********************************************************************************************
        //
        // Arrow head at end of line
        //
        void MakeArrowheadAtEnd ()
        {
            Point endPoint = LinePoints [LinePoints.Count - 1];
            Point h        = LinePoints [LinePoints.Count - 2];

            double finalAngle = Math.Atan2 (endPoint.Y - h.Y, endPoint.X - h.X) * 180 / Math.PI;

            RotateTransform rot = new RotateTransform (finalAngle);
            TranslateTransform xlate = new TranslateTransform (endPoint.X, endPoint.Y);

            TransformGroup group = new TransformGroup ();
            group.Children.Add (scale);
            group.Children.Add (rot);
            group.Children.Add (xlate);

            // transformed template of arrow head
            List<Point> arrowheadPoints = new List<Point> ();
            arrowheadPoints.Add (group.Transform (arrowheadTemplate [0]));
            arrowheadPoints.Add (group.Transform (arrowheadTemplate [1]));
            arrowheadPoints.Add (group.Transform (arrowheadTemplate [2]));
            //    arrowhead1Points.Add (group.Transform (arrowheadTemplate [0]));

            ArrowheadAtEnd_figure = new PathFigure ();
            PolyLineSegment arrowHeadLine = new PolyLineSegment (arrowheadPoints, true);

            ArrowheadAtEnd_figure.StartPoint = arrowHeadLine.Points [0];
            ArrowheadAtEnd_figure.Segments.Add (arrowHeadLine);
            ArrowheadAtEnd_figure.IsFilled = true;
        }

        //********************************************************************************************
        //
        // Arrow head at start of line

        void MakeArrowheadAtStart ()
        { 
            Vector initial = LinePoints [1] - LinePoints [0];
            double initialAngle = Math.Atan2 (initial.Y, initial.X) * 180 / Math.PI;

            RotateTransform rot = new RotateTransform (initialAngle + 180);
            TranslateTransform xlate = new TranslateTransform (LinePoints [0].X, LinePoints [0].Y);

            TransformGroup group = new TransformGroup ();
            group.Children.Add (scale);
            group.Children.Add (rot);
            group.Children.Add (xlate);

            // transformed template of arrow head
            List<Point> arrowheadPoints = new List<Point> ();
            arrowheadPoints.Add (group.Transform (arrowheadTemplate [0]));
            arrowheadPoints.Add (group.Transform (arrowheadTemplate [1]));
            arrowheadPoints.Add (group.Transform (arrowheadTemplate [2]));
      //    arrowheadPoints.Add (group.Transform (arrowheadTemplate [0]));

            ArrowheadAtStart_figure = new PathFigure ();
            PolyLineSegment arrowHeadLine = new PolyLineSegment (arrowheadPoints, true);

            ArrowheadAtStart_figure.StartPoint = arrowHeadLine.Points [0];
            ArrowheadAtStart_figure.Segments.Add (arrowHeadLine);
            ArrowheadAtStart_figure.IsFilled = true;
        }

        //********************************************************************************************
        //********************************************************************************************
        //********************************************************************************************

        public Brush Color {get {return path.Stroke;} set {path.Fill = path.Stroke = value;}}
        public double Thickness {get {return path.StrokeThickness;} set {path.StrokeThickness = value;}}

        public enum DrawingStyle {None, Solid, Dashes, LongDashes, Dots}
        DrawingStyle lineStyle = DrawingStyle.Solid;

        public DrawingStyle LineStyle
        {
            get {return lineStyle;}
            set
            {
                lineStyle = value;

                path.Visibility = Visibility.Visible;

                if (lineStyle == DrawingStyle.None)
                {
                    path.Visibility = Visibility.Hidden;
                }

                else if (lineStyle == DrawingStyle.Solid)
                {
                    path.StrokeDashArray = null;
                }

                else if (lineStyle == DrawingStyle.Dashes)
                {
                    double [] template = new double [] { 2, 4 };
                    path.StrokeDashArray = new DoubleCollection (template);
                }

                else if (lineStyle == DrawingStyle.LongDashes)
                {
                    double [] template = new double [] { 4, 2 };
                    path.StrokeDashArray = new DoubleCollection (template);
                }

                else if (lineStyle == DrawingStyle.Dots)
                {
                    double [] template = new double [] { 1, 2 };
                    path.StrokeDashArray = new DoubleCollection (template);
                }
            }
        }
    }
}
