using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using Plot2D_Embedded;
using System.Net.Http.Headers;
using System.Text;

namespace Plot2D_Embedded_Driver
{
    public partial class MainWindow
    {
        Dart dart = null;

        private void DartButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                dart = new Dart (new Point (0, 0), 0, 0);

                dart.Position = new Point (-10 + (20 * random.NextDouble ()), -10 + (20 * random.NextDouble ()));
                dart.Angle = 360 * random.NextDouble ();
                dart.Size = 0.2 + random.NextDouble ();

                dart.BorderThickness = 2.5;
                dart.FillColor = Brushes.DarkOrange;

                figure.Plot (dart);

                dart.RegisterForMouseLeftClick (Dart_LeftClick);
                MoveDartButton.IsEnabled = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("DartButton Exception: " + ex.Message));
            }
        }

        private void MoveDartButton_Click (object sender, RoutedEventArgs e)
        {
            if (dart != null)
            {
                Point center = dart.Position;
                center += new Vector (3, 1);
                dart.Position = center;
                figure.Refresh ();
            }
        }

        private void Dart_LeftClick (object sender, Point pt)
        {
            Print (string.Format ("Dart left click at {0:0.00}", pt));
        }

        //**********************************************************************************

        private void ButterflyButton_Click (object sender, RoutedEventArgs e)
        {
            try
            { 
                Butterfly bf = new Butterfly (new Point (0, 0), 0, 0);

                bf.Position = new Point (-10 + (20 * random.NextDouble ()), -10 + (20 * random.NextDouble ()));
                bf.Angle = 360 * random.NextDouble ();
                bf.Size = 0.2 + random.NextDouble ();

                figure.Plot (bf);
            }

            catch (Exception ex)
            {
                Print (string.Format ("ButterflyButton Exception: " + ex.Message));
            }
        }

        //**********************************************************************************

        private void VectorButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
              //  figure.Clear ();

                List<Vector> basis = new List<Vector> () {new Vector (1, 0), new Vector (0, 1)};
                //List<Vector> basis = new List<Vector> () {new Vector (1, 0.8), new Vector (0.1, 1.5)};

                // random vector, expressed in that basis
                Vector rvect = new Vector ((random.NextDouble () - 0.5) * 40, (random.NextDouble () - 0.5) * 40);

                // random position of tail
                Point rposition = new Point ((random.NextDouble () - 0.5) * 40, (random.NextDouble () - 0.5) * 40);

                Print (string.Format ("Vector ({0:0.00}, {1:0.00}), from ({2:0.00}, {3:0.00})", rvect.X, rvect.Y, rposition.X, rposition.Y));

                VectorView vv = new VectorView (rposition, rvect, basis);
                vv.Color = Brushes.Maroon;
               

                figure.Plot (vv);
                vv.ShowComponents = true;

                //int select = random.Next (1, 100);
                //if (select % 3 == 0) {vv.Color = Brushes.Orchid;}
                //if (select % 2 == 0) {vv.Thickness = 2; vv.Color = Brushes.Green;}
                //if (select % 4 == 0) {vv.LineStyle = LineView.DrawingStyle.Dashes;}

                figure.AxesEqual = true;
                figure.AxesTight = true;
                figure.RectangularGridOn = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("VectorButton Exception: " + ex.Message));
                Print (string.Format ("VectorButton Exception: " + ex.StackTrace));
            }
        }

        //**********************************************************************************

        private void VectorFieldButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                List<Point>  positions = new List<Point> ();
                List<Vector> vectors   = new List<Vector> ();

                for (double x=-0; x<=60; x+=5)
                {
                    for (double y=-0; y<=60; y+=5)
                    {
                        positions.Add (new Point (x, y));
                        vectors.Add (new Vector (1, 0.2 * y));
                    }
                }

                VectorFieldView vv = new VectorFieldView (positions, vectors);

                vv.Thickness = 2;
                figure.Plot (vv);
                vv.Color = Brushes.RoyalBlue;

                figure.AxesEqual = true;
                figure.RectangularGridOn = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("VectorFieldButton Exception: " + ex.Message));
            }
        }

        //***************************************************************************************
        //***************************************************************************************
        //***************************************************************************************

        // Contour Plots

        double ZForContours1 (double x, double y)
        {
            return (x - 2) * (y - 1);
        }

        public bool RandomContours = false;

        //*******************************************************************************************
        //
        // Contour Plot from passed-in function
        //
        private void Contour_1_Button_Click (object sender, RoutedEventArgs e)
        {
            figure.Clear ();

            try
            {
                double minX = -3, maxX = 3, minY = -2, maxY = 2;
                int numberXSamples = 50;
                int numberYSamples = 40;

                List<double> contourValues = new List<double> ();

                //
                // Random Contours - plot a set of random points, then request contours that pass through them
                //
                if (RandomContours)
                {
                    int N = 3;
                    List<Point> randomPoints = new List<Point> ();

                    for (int i = 0; i<N; i++)
                    {
                        randomPoints.Add (new Point ((random.NextDouble () - 0.5) * (maxX - minX), 
                                                     (random.NextDouble () - 0.5) * (maxY - minY)));
                    }

                    foreach (Point pt in randomPoints)
                    {
                        PointView pv = new PointView (pt);
                        pv.Size = 0.1;
                        figure.Plot (pv);
                    }

                    foreach (Point pt in randomPoints)
                        contourValues.Add (ZForContours1 (pt.X, pt.Y));
                }
                else
                {
                    contourValues.AddRange (new double [] {-3, -1, 1, 3, 5, 7});
                }

                ContourPlotView.DrawLines = true;
                ContourPlotView.LabelLines = true;
                ContourPlotView.LabelFontSize = 0.1;
                ContourPlotView.ShowGradientArrows = true;
                ContourPlotView.GradientArrowSize = 0.1;
                ContourPlotView.ShowColoredBackground = true;
                ContourPlotView cp = new ContourPlotView (ZForContours1, contourValues, minX, maxX, minY, maxY, numberXSamples, numberYSamples);
               
                figure.Plot (cp);
                
                figure.SetAxes (minX, maxX, minY, maxY);           
                figure.AxesEqual = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("ContourButton Exception: " + ex.Message));
            }
        }

        //*******************************************************************************************
        //
        // Contour Plot from passed in values
        //
        private void Contour_2_Button_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                figure.Clear ();

                double minX = -3, maxX = 3, minY = -2, maxY = 2;
                int numberXSamples = 50;
                int numberYSamples = 40;

                List<double> contourValues = new List<double> () {-4, -2, 2, 4, 6, 8};

                List<double> xValues = new List<double> (numberXSamples);
                List<double> yValues = new List<double> (numberYSamples);
                CommonMath.Matrix zValues = new CommonMath.Matrix (numberYSamples, numberXSamples);

                double dx = (maxX - minX) / (numberXSamples - 1);
                double dy = (maxY - minY) / (numberYSamples - 1);

                for (int i = 0; i<numberXSamples; i++)
                    xValues.Add (minX + i * dx);

                for (int i = 0; i<numberYSamples; i++)
                    yValues.Add (minY + i * dy);

                for (int xi = 0; xi<numberXSamples; xi++)
                {
                    for (int yi = 0; yi<numberYSamples; yi++)
                    {
                        try
                        {
                            zValues [yi, xi] = ZForContours1 (xValues [xi], yValues [yi]);
                        }

                        catch (NotFiniteNumberException)
                        {
                            zValues [yi, xi] = Double.NaN;
                        }
                    }
                }
                
                ContourPlotView.DrawLines = false;
                ContourPlotView.LabelLines = false;
                ContourPlotView.ShowGradientArrows = true;
                ContourPlotView.ShowColoredBackground = true;
                ContourPlotView cp = new ContourPlotView (xValues, yValues, zValues, contourValues);

                figure.Plot (cp);

                figure.SetAxes (minX, maxX, minY, maxY);
                figure.AxesEqual = true;
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
            }
        }
    }
}
