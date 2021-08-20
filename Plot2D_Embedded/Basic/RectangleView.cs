using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace Plot2D_Embedded
{
    public class RectangleView : CanvasObject
    {
        //**************************************************************
        //
        // Define template for all RectangleView objects
        //

        static double w = 0.5; // half-width
        static Point tlc = new Point (-w,  w);
        static Point trc = new Point ( w,  w);
        static Point brc = new Point ( w, -w);
        static Point blc = new Point (-w, -w);
        static Rect rect = new Rect (tlc, brc);

        // template bounding box corners
        static List<Point> templateBBCorners = new List<Point> () {tlc, trc, brc, blc};

        //*************************************************************
        //
        // Instance Constructors
        //

        public RectangleView (Point center, double width, double height)
        {
            RectangleGeometry rectangleGeometry = new RectangleGeometry (rect);

            //
            // transforms to get from template to world coordinates. drawing code will append
            // world coord to WPF coord transform
            //
            TransformGroup grp = new TransformGroup ();
            grp.Children.Add (scale);
            grp.Children.Add (rotate);
            grp.Children.Add (xlate);
            rectangleGeometry.Transform = grp;

            Width = width;
            Height = height;
            Center = center;

            // construct objects top-down
            path.Data = rectangleGeometry;

            path.Fill = null;// Brushes.Transparent;  // null doesn't capture mouse clicks in fill region
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
        }

        public double Angle {get {return rotate.Angle;}                
                             set {rotate.Angle = value; CalculateBB (templateBBCorners);}}

        public double Width {get {return scale.ScaleX;}
                             set {scale.ScaleX = value; CalculateBB (templateBBCorners);}}

        public double Height {get {return scale.ScaleY;}
                             set {scale.ScaleY = value; CalculateBB (templateBBCorners);}}

        public Point Center {get {return new Point (xlate.X, xlate.Y);}
                             set {xlate.X = value.X; xlate.Y = value.Y; CalculateBB (templateBBCorners);}}
    }
}
