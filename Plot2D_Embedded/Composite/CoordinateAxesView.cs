using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Media;

namespace Plot2D_Embedded
{
    internal class AxisLine
    {
        //Point Origin;
        //Point End;
        //Brush Color = Brushes.Red;

        internal AxisLine (Point origin, double lengthLeft, double lengthRight, 
                           double firstTic, double ticStep, double angle, double scale, Brush color, List<CanvasObject> lst)
        {
            double ticHalfLength = ticStep / 10;

            RotateTransform rot = new RotateTransform (angle);
            ScaleTransform stretch = new ScaleTransform (scale, scale);
            TranslateTransform move = new TranslateTransform (origin.X, origin.Y);
            TransformGroup xform = new TransformGroup ();
            xform.Children.Add (stretch);
            xform.Children.Add (rot);
            xform.Children.Add (move);

            Vector dir = new Vector (1, 0);

            Point Start = xform.Transform (new Point (0, 0) - dir * lengthLeft);
            Point End   = xform.Transform (new Point (0, 0) + dir * lengthRight);
            LineView axis = new LineView (Start, End);
            axis.ArrowheadAtEnd = true;
            axis.ArrowheadScaleFactor = ticHalfLength; // scale;
            axis.Color = color;
            lst.Add (axis);

            for (double x = firstTic; x < lengthRight; x += ticStep)
            {
                LineView tic = new LineView (xform.Transform (new Point (x, ticHalfLength)), xform.Transform (new Point (x, -ticHalfLength)));
                tic.Color = color;
                lst.Add (tic);

                TextView lbl = new TextView (xform.Transform (new Point (x, -ticHalfLength)), x.ToString ());
                lbl.FontSizeAppInUnits = 2 * ticHalfLength; // 0.5 * scale;
                lbl.Color = color;
                lst.Add (lbl);
            }

            for (double x = -firstTic; x > -lengthLeft; x -= ticStep)
            {
                LineView tic = new LineView (xform.Transform (new Point (x, ticHalfLength)), xform.Transform (new Point (x, -ticHalfLength)));
                tic.Color = color;
                lst.Add (tic);

                TextView lbl = new TextView (xform.Transform (new Point (x, -ticHalfLength)), x.ToString ());
                lbl.FontSizeAppInUnits = 2 * ticHalfLength; // 0.5 * scale;
                lbl.Color = color;
                lst.Add (lbl);
            }
        }
    }





    public class CoordinateAxesView : List<CanvasObject>
    {
        public CoordinateAxesView (Point origin, double axisLength, double firstTic, double ticStep)
        {
            AxisLine X = new AxisLine (origin, axisLength, axisLength, firstTic, ticStep,  0, 1, Brushes.Red, this);
            AxisLine Y = new AxisLine (origin, axisLength, axisLength, firstTic, ticStep, 90, 1, Brushes.Green, this);
        }

        public CoordinateAxesView (double axisLength, double firstTic, double ticStep) : this (new Point (0,0), axisLength, firstTic, ticStep)
        {
        }

        //public CoordinateAxesView ()
        //{
        //    AxisLine X = new AxisLine (new Point (0, 0), 3, 5, 1, 1, 10, 10, Brushes.Red, this);
        //    AxisLine Y = new AxisLine (new Point (0, 0), 3, 5, 1, 1, 100, 10, Brushes.Green, this);
           
        //    //AxisLine X1 = new AxisLine (new Point (3, 4), 2, 3, 0.5, 1, 20, 1, Brushes.Pink, this);
        //    //AxisLine Y2 = new AxisLine (new Point (3, 4), 2, 3, 0.5, 1, 70, 1, Brushes.LightGreen, this);
        //}
    }
}
