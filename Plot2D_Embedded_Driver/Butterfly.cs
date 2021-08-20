using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using Plot2D_Embedded;

namespace Plot2D_Embedded_Driver
{
    public class Butterfly : UserShapeView
    {
        static Template template = new Template ();
        static PathGeometry templateGeometry = new PathGeometry ();

        static Butterfly ()
        {
            template.points.AddRange (new List<Point> () {new Point (-1, 1), new Point (0, 1), new Point (1, 1), 
                                                          new Point (0, 0), 
                                                          new Point (-1, -1), new Point (0, -1), new Point (1, -1)});

            template.polyLines.Add (new List<int> () {0, 3, 4, 0});
            template.polyLines.Add (new List<int> () {3, 2, 6, 3});
            template.polyLines.Add (new List<int> () {1, 5});




            foreach (Point pt in template.points)
                template.boundingBox.Union (pt);

            template.BBCorners.AddRange (new List<Point> () {template.boundingBox.TLC, template.boundingBox.TRC, template.boundingBox.BRC, template.boundingBox.BLC});

            template.strokeColor = Brushes.Red;
            template.fillColor = Brushes.Gray;

            templateGeometry = TemplateToGeometry (template);
        }

        //**************************************************************
        //
        // Instance Constructors
        //

        public Butterfly (Point position, double angle, double size)
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
