using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // cursors
using System.Windows.Media;
using System.Windows.Shapes;

using Common;

namespace Plot2D_Embedded
{
    internal class Plot2DGrid_Rectangular : Plot2DGrid
    {
        internal int numberXAxisMarks = 5;
        internal int numberYAxisMarks = 4;

        private List<double> xAxisTicValues = new List<double> ();
        private List<double> yAxisTicValues = new List<double> ();

        //******************************************************************************************

        internal Plot2DGrid_Rectangular (Bare2DPlot plot) : base (plot)
        {
        }
        
        public void CalculateTicValues (Viewport2D viewPort)
        {
            double xAnchor = 0;
            double yAnchor = 0;

            double xStep = 0.5;
            double yStep = 0.5;

            xAxisTicValues.Clear ();
            yAxisTicValues.Clear ();

            CalculateTicValues (ref xAxisTicValues, viewPort.Left,   viewPort.Right, xAnchor, xStep);
            CalculateTicValues (ref yAxisTicValues, viewPort.Bottom, viewPort.Top,   yAnchor, yStep);

            //EventLog.WriteLine ("x axis tics");
            //EventLog.WriteLine ("left = " + viewPort.Left.ToString ());
            //EventLog.WriteLine ("right = " + viewPort.Right.ToString ());
            //EventLog.WriteLine ("anchor = " + xAnchor.ToString ());
            //EventLog.WriteLine ("step = " + xStep.ToString ());

            //foreach (double v in xAxisTicValues)
            //    EventLog.WriteLine (v.ToString ());
        }

        public void ClearTicValues ()
        {
            xAxisTicValues.Clear ();
            yAxisTicValues.Clear ();
        }

        //******************************************************************************************

        public void DrawGridLines ()
        {
            double [] gridLineTemplate = new double [] { 1, 3 };

            for (int i = 0; i<xAxisTicValues.Count; i++)
            {
                double wx = xAxisTicValues [i];
                double cx = WorldXToCanvasX (wx);

                Line line = new Line ();
                line.X1 = line.X2 = cx;
                line.Y1 = ticMarkLength;
                line.Y2 = InnerCanvas.ActualHeight - ticMarkLength;
                line.Stroke = Brushes.Black;
                line.StrokeDashArray = new DoubleCollection (gridLineTemplate);
                line.StrokeThickness = 1;
                InnerCanvas.Children.Add (line);
            }

            for (int i = 0; i<yAxisTicValues.Count; i++)
            {
                double wy = yAxisTicValues [i];
                double cy = WorldYToCanvasY (wy);

                Line line = new Line ();
                line.X1 = ticMarkLength;
                line.X2 = InnerCanvas.ActualWidth - ticMarkLength;
                line.Y1 = line.Y2 = cy;
                line.Stroke = Brushes.Black;
                line.StrokeDashArray = new DoubleCollection (gridLineTemplate);
                line.StrokeThickness = 1;
                InnerCanvas.Children.Add (line);
            }
        }

        //******************************************************************************************

        public void DrawTicMarks ()
        {
//          for (int i = 1; i<xAxisTicValues.Count-1; i++)
            for (int i = 0; i<xAxisTicValues.Count; i++)
            {
                double wx = xAxisTicValues [i];
                double cx = WorldXToCanvasX (wx);

                Line line1 = new Line ();
                line1.X1 = line1.X2 = cx;
                line1.Y1 = 0;
                line1.Y2 = ticMarkLength;
                line1.Stroke = Brushes.Black;
                line1.StrokeThickness = ticMarkThickness;
                InnerCanvas.Children.Add (line1);

                Line line2 = new Line ();
                line2.X1 = line2.X2 = cx;
                line2.Y1 = InnerCanvas.ActualHeight;
                line2.Y2 = line2.Y1 - ticMarkLength;
                line2.Stroke = Brushes.Black;
                line2.StrokeThickness = ticMarkThickness;
                InnerCanvas.Children.Add (line2);
            }

            for (int i = 0; i<yAxisTicValues.Count; i++)
            {
                double wy = yAxisTicValues [i];
                double cy = WorldYToCanvasY (wy);

                Line line1 = new Line ();
                line1.X1 = 0;
                line1.X2 = ticMarkLength;
                line1.Y1 = line1.Y2 = cy;
                line1.Stroke = Brushes.Black;
                line1.StrokeThickness = ticMarkThickness;
                InnerCanvas.Children.Add (line1);

                Line line2 = new Line ();
                line2.X1 = InnerCanvas.ActualWidth;
                line2.X2 = line2.X1 - ticMarkLength;
                line2.Y1 = line2.Y2 = cy;
                line2.Stroke = Brushes.Black;
                line2.StrokeThickness = ticMarkThickness;
                InnerCanvas.Children.Add (line2);
            }
        }

        //******************************************************************************************

        internal string XAxisFormat = "{0:0.0}";
        internal bool XAxisFormatAuto = true;

        internal string YAxisFormat = "{0:0.0}";
        internal bool YAxisFormatAuto = true;

        public void DrawAxisNumericLabels (double DataAreaX0, double DataAreaX1, double DataAreaY0, double DataAreaY1)
        {
            //if (xAxisTicValues.Count > 0)
            //{
            //    if (XAxisFormatAuto == true)
            //    {
            //        double range = Math.Abs (xAxisTicValues [xAxisTicValues.Count - 1] - xAxisTicValues [0]);
            //        double logRange = Math.Log10 (range);
            //        int numberDigits = (int)Math.Round (logRange);

            //        XAxisFormat = "{0:0.0}"; // restore default
            //        if (numberDigits == -3) XAxisFormat = "{0:0.0000}";
            //        if (numberDigits == -2) XAxisFormat = "{0:0.000}";
            //        if (numberDigits == -1) XAxisFormat = "{0:0.00}";
            //    }
            //}

            //if (yAxisTicValues.Count > 0)
            //{
            //    if (YAxisFormatAuto == true)
            //    {
            //        double range = Math.Abs (yAxisTicValues [yAxisTicValues.Count - 1] - yAxisTicValues [0]);
            //        double logRange = Math.Log10 (range);
            //        int numberDigits = (int)Math.Round (logRange);

            //        YAxisFormat = "{0:0.0}"; // restore default
            //        if (numberDigits == -3) YAxisFormat = "{0:0.0000}";
            //        if (numberDigits == -2) YAxisFormat = "{0:0.000}";
            //        if (numberDigits == -1) YAxisFormat = "{0:0.00}";
            //    }
            //}

            foreach (double x in xAxisTicValues)
            {
                double cx = WorldXToCanvasX (x);
                double cy = DataAreaY1;
                //string str = string.Format (XAxisFormat, x);
                string str = string.Format ("{0:0.0##}", x);                

                TextBlock tb1 = new TextBlock ();
                tb1.FontSize = 16;
                tb1.Text = str;
                Canvas.SetTop (tb1, cy);
                Canvas.SetLeft (tb1, DataAreaX0 + cx - 8 * str.Length / 2);
                OuterCanvas.Children.Add (tb1);
            }

            foreach (double y in yAxisTicValues)
            {
                double cy = WorldYToCanvasY (y);
                double cx = DataAreaX0;
                //string str = string.Format (YAxisFormat, y);
                string str = string.Format ("{0:0.0##}", y);                

                TextBlock tb1 = new TextBlock ();
                tb1.FontSize = 16;
                tb1.Text = str;
                Canvas.SetTop (tb1, DataAreaY0 + cy - 12);
                Canvas.SetLeft (tb1, cx - 4 - 8 * str.Length);
                OuterCanvas.Children.Add (tb1);
            }
        }
    }
}
