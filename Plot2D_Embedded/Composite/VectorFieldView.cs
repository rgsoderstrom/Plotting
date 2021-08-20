using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Plot2D_Embedded
{
    public class VectorFieldView : CanvasObject
    {
        public override UIElement View {get {return path;}}
        public Brush Color {get {return path.Stroke;} set {path.Fill = path.Stroke = value;}}
        public double Thickness {get {return path.StrokeThickness;} set {path.StrokeThickness = value;}}

        //******************************************************************************

        //
        // Define template for all arrows
        //

        static Point p0 = new Point (0, 0);    // tail starts here
        static Point p1 = new Point (1, 0);    // tip of arrow end here
        static Point p2 = new Point (0.95, 0); // end of arrow shaft line here
        static Point p3 = new Point (0.8, -0.1);  
        static Point p4 = new Point (0.8,  0.1);

        static readonly List<Point> arrowTemplate = new List<Point> () {p0, p1, p2, p3, p4};

        //********************************************************************************
        //
        // Calculate scale factor, for application length to drawing length conversion
        //

        List<Vector> AutoScaleVectorsForDrawing (List<Point> positions, List<Vector> applicationVectors)
        {
            //
            // find max & min app vector lengths
            //
            double MinVectorLength = Double.MaxValue;
            double MaxVectorLength = 0;

            foreach (Vector v in applicationVectors)
            {
                double l = v.Length;
                if (MinVectorLength > l) MinVectorLength = l;
                if (MaxVectorLength < l) MaxVectorLength = l;
            }

            //
            // calculate max and min drawing vector lengths
            //    - LIMITATION - assumes a regular grid of input positions
            //

            double s = (positions [1] - positions [0]).Length;
            double MinArrowLength = 0.4 * s;
            double MaxArrowLength = 0.9 * s;

            double M = (MaxArrowLength - MinArrowLength) / (MaxVectorLength - MinVectorLength);

            //
            // apply scaling
            //

            List<Vector> DrawingVectors = new List<Vector> ();

            foreach (Vector thisVector in applicationVectors)
            {
                double thisLength = thisVector.Length;
                thisVector.Normalize ();
                Vector scaled = (M * (thisLength - MinVectorLength) + MinArrowLength) * thisVector;
                DrawingVectors.Add (scaled);
            }

            return DrawingVectors;
        }

        //********************************************************************************

        public VectorFieldView (List<Point> positions, List<Vector> vectors) 
        {
            if (positions.Count != vectors.Count)
                throw new Exception ("VectorFieldView - positions and vector counts must be equal");

            List<Vector> drawingVectors = (positions.Count > 1) ? AutoScaleVectorsForDrawing (positions, vectors) : vectors;

            path.Stroke = Brushes.Gray;
            path.StrokeThickness = 1;

            path.Fill = null;

            PathGeometry pg = new PathGeometry ();
            path.Data = pg;

            for (int i=0; i<positions.Count; i++)
            {
                double length = drawingVectors [i].Length;
                double angle = Vector.AngleBetween (new Vector (1, 0), drawingVectors [i]);

                TransformGroup xform = new TransformGroup ();
                xform.Children.Add (new ScaleTransform (length, length));
                xform.Children.Add (new RotateTransform (angle));
                xform.Children.Add (new TranslateTransform (positions [i].X, positions [i].Y));

                List<Point> xformedArrowTemplate = new List<Point> (arrowTemplate.Count);

                foreach (Point pt in arrowTemplate)
                {
                    Point xpt = xform.Transform (pt);
                    xformedArrowTemplate.Add (xpt);
                    BoundingBox.Union (xpt);
                }

                PathFigure v1 = new PathFigure ();
                v1.StartPoint = xformedArrowTemplate [0];
                v1.Segments.Add (new LineSegment (xformedArrowTemplate [2], true));       
                v1.Segments.Add (new LineSegment (xformedArrowTemplate [1], false));       
                v1.Segments.Add (new LineSegment (xformedArrowTemplate [3], true));
                v1.Segments.Add (new LineSegment (xformedArrowTemplate [4], false));       
                v1.Segments.Add (new LineSegment (xformedArrowTemplate [1], true));
            
                pg.Figures.Add (v1);
            }
        }
    }
}
