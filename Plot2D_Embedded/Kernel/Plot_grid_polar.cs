using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // cursors
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plot2D_Embedded
{
    internal class Plot2DGrid_Polar : Plot2DGrid
    {
        protected int numberRangeMarks = 3;
        protected int numberThetaMarks = 3;

        protected List<double> rhoValues   = new List<double> ();
        protected List<double> thetaValues = new List<double> ();

        double maxRho;
        double minRho;
        double maxTheta;
        double minTheta;

        double dr; // distance between successive rho circles

        //**********************************************************************************

        public Plot2DGrid_Polar (Bare2DPlot plot) : base (plot)
        {
            ClearTicValues ();
        }

        public void ClearTicValues ()
        {
            rhoValues.Clear ();
            thetaValues.Clear ();

            maxRho   = double.MinValue;
            minRho   = double.MaxValue;
            dr       = 1;
            maxTheta = 2 * Math.PI;
            minTheta = 0;
        }


        public void CalculateTicValues (Viewport2D viewport)
        {
            viewport.PolarGridCalculations ();

            minRho   = viewport.MinDistanceToOrigin * 1.01;
            maxRho   = viewport.MaxDistanceFromOrigin * 0.99;
            minTheta = viewport.MinThetaFromOrigin;
            maxTheta = viewport.MaxThetaFromOrigin;
           
          //  CalculateTicValues (ref rhoValues,   minRho,   maxRho,   numberRangeMarks);
          //  CalculateTicValues (ref thetaValues, minTheta, maxTheta, numberThetaMarks);

            if (rhoValues.Count > 1)
                dr = rhoValues [1] - rhoValues [0];

            //Console.WriteLine ("{0:0.0}, {1:0.0}", minRho, maxRho);
            //Console.WriteLine ("{0:0.0}, {1:0.0}", minTheta * 180 / Math.PI, maxTheta * 180 / Math.PI);
        }

        //******************************************************************************************
        //
        // DrawGridLines () - draw range rings and bearing lines
        //

        public void DrawGridLines ()
        {
            if (maxRho <= minRho)
                throw new Exception ("Polar grid, DrawGridLines, maxRho <= minRho");

            double ThetaLabelsCommonRange = minRho + (maxRho - minRho) / 3;
            double RhoLabelsCommonTheta = minTheta + (maxTheta - minTheta) / 3;

            //
            // circles of constant rho
            //

            GeometryGroup circles = new GeometryGroup ();

            foreach (double radius in rhoValues)
            {
                if (radius > 0)
                    circles.Children.Add (new EllipseGeometry (new Point (0, 0), radius, radius));
                else
                {
                    double l1 = dr / 10;
                    LineGeometry lg1 = new LineGeometry (new Point (-l1, 0), new Point (l1, 0));
                    LineGeometry lg2 = new LineGeometry (new Point (0, -l1), new Point (0, l1));

                    circles.Children.Add (lg1);
                    circles.Children.Add (lg2);
                }
            }

            circles.Transform = HostPlot.WorldToCanvas;

            Path path = new Path ();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
            double [] template = new double [] { 1, 2 };
            path.StrokeDashArray = new DoubleCollection (template);
            path.Data = circles;

            InnerCanvas.Children.Add (path);

            // label the range circles

            foreach (double range in rhoValues)
            {
                TextBlock tb = new TextBlock ();
                tb.Text = string.Format ("{0:0.0}", range);
                tb.Background = Brushes.White;

                double left = WorldXToCanvasX (range * Math.Cos (RhoLabelsCommonTheta));
                double top  = WorldYToCanvasY (range * Math.Sin (RhoLabelsCommonTheta));
                Canvas.SetLeft (tb, left);
                Canvas.SetTop (tb, top);
                InnerCanvas.Children.Add (tb);                
            }

            //
            // radial lines
            //


            if (minRho == 0) minRho = dr / 5;


            GeometryGroup radialLines = new GeometryGroup ();

            foreach (double theta in thetaValues)
            {
                radialLines.Children.Add (new LineGeometry (new Point (maxRho * Math.Cos (theta), maxRho * Math.Sin (theta)), 
                                                            new Point (minRho * Math.Cos (theta), minRho * Math.Sin (theta))));
/**
                radialLines.Children.Add (new LineGeometry (new Point (minRho * Math.Cos (theta), minRho * Math.Sin (theta)), 
                                                            new Point (maxRho * Math.Cos (theta), maxRho * Math.Sin (theta)))); **/
            }

            radialLines.Transform = HostPlot.WorldToCanvas;

            path = new Path ();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
            path.StrokeDashArray = new DoubleCollection (template);
            path.Data = radialLines;

            InnerCanvas.Children.Add (path);            

            foreach (double theta in thetaValues)
            {
                TextBlock tb = new TextBlock ();
                tb.Text = string.Format ("{0:0.0}", theta * 180 / Math.PI);
                tb.Background = Brushes.White;

                double left = WorldXToCanvasX (ThetaLabelsCommonRange * Math.Cos (theta));
                double top  = WorldYToCanvasY (ThetaLabelsCommonRange * Math.Sin (theta)) - tb.FontSize / 2;
                Canvas.SetLeft (tb, left);
                Canvas.SetTop (tb, top);
                InnerCanvas.Children.Add (tb);                
            }
        }        
    }
}
