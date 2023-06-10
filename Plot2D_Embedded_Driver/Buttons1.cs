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

        Point CurveFunction (double x) {return new Point (x, x * x - 0.25) + new Vector (3.456, 4.567);}

        private void LineViewCurveButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                List<Point> parabola = new List<Point> ();

                for (double x = -2; x<=2; x+=0.125)
                    parabola.Add (CurveFunction (x));


                LineView h = new LineView (parabola);
                h.LineStyle = LineView.DrawingStyle.Dashes;
                h.Color = Brushes.LightSteelBlue;

                h.ArrowheadAtStart = true;
                h.ArrowheadAtEnd = true;

                h.Thickness = 3;
                h.ArrowheadScaleFactor = 1/30.0;

                figure.Plot (h);
                figure.RectangularGridOn = true;
                figure.DataAreaTitle = "Title Here";
                figure.XAxisLabel = "X Label Here";
                figure.YAxisLabel = "Y Label Here";
                figure.AxesEqual = true;
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
