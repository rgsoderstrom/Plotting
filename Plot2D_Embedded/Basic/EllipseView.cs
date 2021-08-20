using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace Plot2D_Embedded
{
    public class EllipseView : CanvasObject
    {
        //**************************************************************
        //
        // Define template for all EllipseView objects
        //

        static double templateRadius = 0.5;
        static Point templateCenter = new Point (0, 0);
        static Point tlc = templateCenter + new Vector (templateRadius, 0);
        static Point trc = templateCenter - new Vector (templateRadius, 0);
        static Point brc = templateCenter + new Vector (0, templateRadius);
        static Point blc = templateCenter - new Vector (0, templateRadius);

        // template bounding box corners
        static List<Point> templateBBCorners = new List<Point> () {tlc, trc, brc, blc};

        //************************************************************************************************

        public EllipseView () : this (templateCenter, templateRadius)
        {
        }

        public EllipseView (Point center, double Radius)
        {
            EllipseGeometry ellipseGeometry = new EllipseGeometry (templateCenter, templateRadius, templateRadius);

            //
            // transforms to get from template to world coordinates. drawing code will append
            // world coord to WPF coord transform
            //

            TransformGroup grp = new TransformGroup ();
            grp.Children.Add (scale);
            grp.Children.Add (rotate);
            grp.Children.Add (xlate);
            ellipseGeometry.Transform = grp;

            //Width = width;
            //Height = height;
            Center = center;

            // construct objects top-down
            path.Data = ellipseGeometry;

            path.Fill = null;// Brushes.Transparent;  // null doesn't capture mouse clicks in fill region
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
       }

        public Point Center {get {return new Point (xlate.X, xlate.Y);}
                             set {xlate.X = value.X; xlate.Y = value.Y; CalculateBB (templateBBCorners);}}

        public double Width {get {return scale.ScaleX;}
                             set {scale.ScaleX =  value; CalculateBB (templateBBCorners);}}

        public double Height {get {return scale.ScaleY;}
                             set {scale.ScaleY =  value; CalculateBB (templateBBCorners);}}

        public double Angle {get {return rotate.Angle;}
                             set {rotate.Angle =  value; CalculateBB (templateBBCorners);}}


        // for Circle, special case of ellipse
        public double Radius {get {return scale.ScaleX;}
                             set {scale.ScaleX = scale.ScaleY = value; CalculateBB (templateBBCorners);}}

        public double Diameter {get {return 2 * scale.ScaleX;}
                                set {scale.ScaleX = scale.ScaleY = value / 2; CalculateBB (templateBBCorners);}}
    }
}
