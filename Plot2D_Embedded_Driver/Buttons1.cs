using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Common;

using Plot2D_Embedded;

namespace Plot2D_Embedded_Driver
{
    partial class MainWindow
    {

        //**************************************************************************************

        private void PointButton_Click (object sender, RoutedEventArgs e)
        {
            List<Point> ps = new List<Point> ();
            ps.Add (new Point (3, 4));

            PointView pv = new PointView (ps); // (new Point (3, 4));

            pv.Size = 0.05;
            figure.Plot (pv);
           // figure.AxesEqual = true;
           // figure.RectangularGridOn = true;
        }

        private void LineButton_Click (object sender, RoutedEventArgs e)
        {
            LineView lv = new LineView (new Point (-1, 1), new Point (3, 4));

            figure.Plot (lv);
            lv.ArrowheadAtEnd = true;
            lv.Color = Brushes.DarkBlue;
            figure.AxesEqual = true;
            figure.RectangularGridOn = true;
        }

        private void CircleButton_Click (object sender, RoutedEventArgs e)
        {
            EllipseView h = new EllipseView (new Point (2, 2), 1);
            figure.Plot (h);
            figure.AxesEqual = true;
            figure.RectangularGridOn = true;
        }

        private void EllipseButton_Click (object sender, RoutedEventArgs e)
        {
            EllipseView h = new EllipseView () {Center = new Point (-1, 2), Width = 2, Height = 1, Angle = 10};

            figure.Plot (h);
            figure.AxesEqual = true;
            figure.RectangularGridOn = true;
        }

        //**************************************************************************************

        RectangleView h;

        private void RectangleButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                h = new RectangleView (new Point (2.5, 5), 1, 0.5);
                h.Angle = 10;
                figure.Plot (h);
                figure.AxesEqual = true;
                figure.RectangularGridOn = true;

                MoveButton.IsEnabled = true;
                RotateButton.IsEnabled = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception: {0}", ex.Message));
            }
        }

        private void RotateRectButton_Click (object sender, RoutedEventArgs e)
        {
            if (h != null)
            {
                h.Angle -= 10;
                figure.Refresh ();
            }
        }

        private void MoveRectButton_Click (object sender, RoutedEventArgs e)
        {
            if (h != null)
            {
                h.Center += new Vector (0.3, 0.1);
                figure.Refresh ();
            }
        }

        //****************************************************************************************

        //Point CurveFunction (double x) {return new Point (x, x * x - 0.25) + new Vector (3.456, 4.567);}
        Point CurveFunction (double x) {return new Point (x, x * x);}

        private void HandKDrillProbButton_Click (object sender, RoutedEventArgs e)
        {
            double R = 400;     // Ohms
            double C = 0.01e-6; // Farads
            double L = 10e-3; // Henrys

            double a = 1 / (2 * R * C);
            double w0 = 1 / Math.Sqrt (L * C);

            double s1 = -a + Math.Sqrt (a * a - w0 * w0);
            double s2 = -a - Math.Sqrt (a * a - w0 * w0);

            double A1 = 6.67;
            double A2 = 13.3;

            // t = (0 : 0.1 : 20) * 1e-6;
            // v = A1 * exp (s1 * t) + A2 * exp (s2 * t);

            List<double> t = new List<double> ();
            List<Point> vt = new List<Point> ();

            //for (double tt = 0; tt < 20e-6; tt += 0.1e-6)
            for (double tt = 3.45e-6; tt < 23e-6; tt += 0.1e-6)
                t.Add (tt);

            foreach (double tt in t)
                vt.Add (new Point (23 + tt * 1e6, A1 * Math.Exp (s1 * tt) + A2 * Math.Exp (s2 * tt)));

            try
            {
                LineView h = new LineView (vt);
                h.Thickness = 3;
                figure.Plot (h);
                //figure.RectangularGridOn = true;
                //figure.DataAreaTitle = "Title Here";
                figure.XAxisLabel = "microseconds";
                //figure.YAxisLabel = "Y Label Here";
                //figure.AxesEqual = false;
                //figure.AxesTight = true;

                figure.RectangularGridOn = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception: {0}", ex.Message));
            }
        }

        private void PointViewCurveButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                List<Point> parabola = new List<Point> ();

                for (double x=-2; x<=2; x+=0.125)
                    parabola.Add (CurveFunction (x));

                PointView h = new PointView (parabola, PointView.DrawingStyle.Plus);
                h.Size = 1.0 / 16;
                h.BorderColor = Brushes.DarkSlateBlue;

                //h.BorderThickness = 3;
                h.Thickness = 3;

                h.FillColor = Brushes.BlueViolet;

                figure.Plot (h);
                figure.AxesEqual = false;
                figure.RectangularGridOn = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception: {0}", ex.Message));
            }
        }
    }
}
