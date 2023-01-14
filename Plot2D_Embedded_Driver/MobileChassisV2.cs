using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using Plot2D_Embedded;


//
// Draw Version 2 of robot chassis on a plot 2D surface
//  - develop and debug here, then copy into robot control app
//

namespace Plot2D_Embedded_Driver
{
    public class MobileChassisV2 : UserShapeView
    {
        static Template template = new Template ();
        static PathGeometry templateGeometry = new PathGeometry ();

        static MobileChassisV2 ()
        {
            template.points.AddRange (new List<Point> () {new Point (-7.5, 0), new Point (7.5, 0), new Point (7.5, -17), new Point (-7.5, -17),  // chassis
                                                          new Point (-8.5 - 0.75, -14.85 + 2), new Point (-8.5 + 0.75, -14.85 + 2), new Point (-8.5 + 0.75, -14.85 - 2), new Point (-8.5 - 0.75, -14.85 - 2),   // left wheel
                                                          new Point ( 8.5 - 0.75, -14.85 + 2), new Point ( 8.5 + 0.75, -14.85 + 2), new Point ( 8.5 + 0.75, -14.85 - 2), new Point ( 8.5 - 0.75, -14.85 - 2)}); // right wheel

            template.polyLines.Add (new List<int> () {0, 1, 2, 3, 0});   // body
            template.polyLines.Add (new List<int> () {4, 5, 6, 7, 4});   // left wheel
            template.polyLines.Add (new List<int> () {8, 9, 10, 11, 8}); // right wheel

            foreach (Point pt in template.points)
                template.boundingBox.Union (pt);

            template.BBCorners.AddRange (new List<Point> () {template.boundingBox.TLC, template.boundingBox.TRC, template.boundingBox.BRC, template.boundingBox.BLC});

            template.strokeColor = Brushes.Black;
            template.fillColor = Brushes.LightGray;

            templateGeometry = TemplateToGeometry (template);
        }


        //**************************************************************
        //
        // Instance Constructors
        //

        public MobileChassisV2 (Point position, double angle, double size)
        {
            PathGeometry dartGeometry = templateGeometry.Clone ();

            //
            // transforms to get from template to world coordinates. drawing code will append
            // world coord to WPF coord transform
            //
            TransformGroup grp = new TransformGroup ();
            grp.Children.Add (scale);
            grp.Children.Add (rotate);
            grp.Children.Add (xlate);
            dartGeometry.Transform = grp;

            Position = position;
            Angle = angle;
            Size = size;

            // construct objects top-down
            path.Data = dartGeometry;

            path.StrokeThickness = DefaultLineThickness;
            path.Stroke = Brushes.Black;
            path.Fill = Brushes.LightGray;
        }

        public double Angle {get {return rotate.Angle;}
                             set {rotate.Angle = value; CalculateBB (template.BBCorners);}}

        public double Size  {get {return scale.ScaleX;}
                             set {scale.ScaleX = scale.ScaleY = value; CalculateBB (template.BBCorners);}}

        public Point Position {get {return new Point (xlate.X, xlate.Y);}
                               set {xlate.X = value.X; xlate.Y = value.Y; CalculateBB (template.BBCorners);}}

    }
}
