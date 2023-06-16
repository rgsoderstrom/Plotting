using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;

using PlottingLib;
using Plot2D_Embedded;
using Plot3D_Embedded;
using System.Windows.Documents;


namespace PlottingLib_Driver
{
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            EventLog.Open (@"../../Log.txt", false);
            InitializeComponent ();
        }

        private void DriverWindow_Loaded (object sender, RoutedEventArgs e)
        {
            EventLog.WriteLine ("PlottingLib driver");
            Print ("Loaded");
            Left = 50;
        }

        private void DriverWindow_Closed (object sender, EventArgs e)
        {
            EventLog.Close ();
            Application.Current.Shutdown();
        }

        //*******************************************************************************************
        //
        // Functionality needed by any application that uses PlottingLib
        //

        private List<IPlotCommon> Figures = new List<IPlotCommon> ();
        private IPlotCommon CurrentFigure = null;
        
        //*******************************************************************************************

        private void NewFigButton_Click (object sender, RoutedEventArgs e) 
        {
            CurrentFigure = new PlotFigure ();
            (CurrentFigure as PlotFigure).Closed += Figure_Closed;
            (CurrentFigure as PlotFigure).Activated += Figure_Activated;
            (CurrentFigure as PlotFigure).DataAreaTitle = "DataAreaTitle";
            (CurrentFigure as PlotFigure).XAxisLabel    = "XAxisLabel";
            (CurrentFigure as PlotFigure).YAxisLabel    = "YAxisLabel";

            Figures.Add (CurrentFigure);
        }

        private void New2DFigButton_Click (object sender, RoutedEventArgs e) 
        {
            CurrentFigure = new Plot2D ();
            (CurrentFigure as Plot2D).Closed += Figure_Closed;
            (CurrentFigure as Plot2D).Activated += Figure_Activated;

            Figures.Add (CurrentFigure);
            HoldBox.IsChecked = (CurrentFigure as Plot2D).Hold;
        }

        private void New3DFigButton_Click (object sender, RoutedEventArgs e) 
        {
            CurrentFigure = new Plot3D ();
            (CurrentFigure as Plot3D).Closed += Figure_Closed;
            (CurrentFigure as Plot3D).Activated += Figure_Activated;

            Figures.Add (CurrentFigure);
        }

        private void Figure_Activated (object sender, EventArgs e)
        {
            CurrentFigure = sender as IPlotCommon;
            HoldBox.IsChecked = CurrentFigure.Hold;
        }

        private void Figure_Closed (object sender, EventArgs e)
        {
            Figures.Remove (sender as IPlotCommon);

            if (Figures.Count > 0)
                CurrentFigure = Figures [Figures.Count - 1];
            else
                CurrentFigure = null;
        }

        private void SetCurrentFigureTo2D ()
        {
            if (CurrentFigure == null)
            {
                New2DFigButton_Click (null, null); 
            }

            else if (CurrentFigure is PlotFigure)
            {
                Plot2D fig = new Plot2D (CurrentFigure as PlotFigure);
                (CurrentFigure as PlotFigure).Close ();
                CurrentFigure = fig;
            }

            else if (CurrentFigure is Plot3D)
            {
                New2DFigButton_Click (null, null); 
            }
        }

        private void SetCurrentFigureTo3D ()
        {
            if (CurrentFigure == null)
            {
                New3DFigButton_Click (null, null); 
            }

            else if (CurrentFigure is PlotFigure)
            {
                Plot3D fig = new Plot3D (CurrentFigure as PlotFigure);
                (CurrentFigure as PlotFigure).Close ();
                CurrentFigure = fig;
            }

            else if (CurrentFigure is Plot2D)
            {
                New3DFigButton_Click (null, null); 
            }
        }



        //
        // End of functionality needed 
        //

        private void ClearFigButton_Click (object sender, EventArgs e)
        {
            if (CurrentFigure != null)
            {
                if (CurrentFigure is IPlotDrawable)
                    (CurrentFigure as IPlotDrawable).Clear ();
            }
        }

        private void Hold_Click (object sender, EventArgs e)
        {
            if (CurrentFigure != null)
            {
                CurrentFigure.Hold = (bool) HoldBox.IsChecked;
            }
        }

        //********************************************************************************************************************
        //********************************************************************************************************************
        //********************************************************************************************************************

        //
        // Methods specific to the Application
        //

        private void CoordAxesButton_Click (object sender, RoutedEventArgs e)
        {
            CoordinateAxesView cv = new CoordinateAxesView (10, 10, 2, 2);
            cv.PositiveOnly = true;

            SetCurrentFigureTo2D ();
            (CurrentFigure as Plot2D).Hold = true;
            (CurrentFigure as Plot2D).AxesEqual = true;
            (CurrentFigure as Plot2D).Plot (cv);
        }

        //*******************************************************************************************************************

        Random random = new Random ();

        private void VectorButton_Click (object sender, RoutedEventArgs e) 
        {
            Point pt = new Point  (5 * (random.NextDouble () - 0.5), 5 * (random.NextDouble () - 0.5));
            Vector v = new Vector (5 * (random.NextDouble () - 0.5), 5 * (random.NextDouble () - 0.5));

            VectorView VV = new VectorView (pt, v);
            VV.ShowComponents = true;

            SetCurrentFigureTo2D ();
            (CurrentFigure as Plot2D).Hold = true;
            (CurrentFigure as Plot2D).AxesEqual = true;
            (CurrentFigure as Plot2D).Plot (VV);

            (CurrentFigure as Plot2D).DataAreaTitle = "Vector";
        }

        //**********************************************************************************

        private void VectorFieldButton_Click (object sender, RoutedEventArgs e)
        {
            int N = 10;
            List<Point> points = new List<Point> ();
            List<Vector> vects = new List<Vector> ();

            for (int i = 0; i<N; i++)
            {
                double x = random.NextDouble () - 0.5;
                double y = random.NextDouble () - 0.5;
                points.Add (new Point (x, y));
                vects.Add (new Vector (x/10, y/10));
            }

            VectorFieldView vfv = new VectorFieldView (points, vects);
            SetCurrentFigureTo2D ();
            (CurrentFigure as Plot2D).Hold = true;
            (CurrentFigure as Plot2D).AxesEqual = true;
            (CurrentFigure as Plot2D).Plot (vfv);
        }

        //**********************************************************************************

        double ZForContours1 (double x, double y)
        {
            return (x - 2) * (y - 1);
        }

        private void ContourPlotButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                SetCurrentFigureTo2D ();
                (CurrentFigure as Plot2D).Hold = true;
                (CurrentFigure as Plot2D).AxesEqual = true;
                Plot2D figure = CurrentFigure as Plot2D;

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
                
                ContourPlotView.DrawLines = true;
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

        //**********************************************************************************

        private void CircleButton_Click (object sender, RoutedEventArgs e) 
        {
            EllipseView h = new EllipseView (new Point (2, 2), 1); 
            SetCurrentFigureTo2D ();
            (CurrentFigure as Plot2D).Plot (h);
        }

        private void EllipseButton_Click (object sender, RoutedEventArgs e) 
        {
            CanvasObject h = new EllipseView () {Center = new Point (-1, 2), Width = 2, Height = 1, Angle = 10};
            //EllipseView h = new EllipseView () {Center = new Point (-1, 2), Width = 2, Height = 1, Angle = 10};

            SetCurrentFigureTo2D ();
            (CurrentFigure as Plot2D).Hold = true;
            (CurrentFigure as Plot2D).AxesEqual = true;
            (CurrentFigure as Plot2D).Plot (h);
            (CurrentFigure as Plot2D).RectangularGridOn = true;
        }

        private void RectangleButton_Click (object sender, RoutedEventArgs e) 
        {
            RectangleView h = new RectangleView (new Point (2.5, 5), 1, 0.5); 
            h.Angle = 10;
            SetCurrentFigureTo2D ();
            (CurrentFigure as Plot2D).Plot (h);
        }

        private void RotateRectButton_Click (object sender, RoutedEventArgs e)
        {
        }
        
        private void MoveRectButton_Click (object sender, RoutedEventArgs e) 
        {
        }

        private void SmoothCurveButton_Click (object sender, RoutedEventArgs e) 
        {
            CanvasObject h = new LineView (new Point (1, 2), new Point (2, 3)); 
            SetCurrentFigureTo2D ();
            (CurrentFigure as Plot2D).Plot (h);
            (h as LineView).Color = Brushes.Red;
        }


        void PointCurveButton_Click (object sender, RoutedEventArgs e) 
        { 
        
        }

        //**********************************************************************************

        //
        // 3D Objects
        //

        void Point_Clicked (object sender, RoutedEventArgs e) 
        {
            Point3D pt = Plot3D_Embedded.Utils.RandomPoint (2.0);
            PointMarker ppt = new Sphere (pt);
            ppt.Diameter = 0.2;
            
            SetCurrentFigureTo3D ();
            (CurrentFigure as Plot3D).Plot (ppt);

            (CurrentFigure as Plot3D).DataAreaTitle = "Random point";
        }

        void PointCloud_Clicked (object sender, RoutedEventArgs args)
        {
            List<Point3D> points = new List<Point3D> ();

            for (int i=0; i<50; i++)
                points.Add (Utils.RandomPoint (15));

            double radius = 0.2 + Utils.RandomDouble (1);

            PointCloud3D pc = new PointCloud3D (points);
            SetCurrentFigureTo3D ();

            (CurrentFigure as Plot3D).AxesTight = false;
            (CurrentFigure as Plot3D).Hold = false;
            PointCloud3DView h = (CurrentFigure as Plot3D).Plot (pc) as PointCloud3DView;
           
            h.Diameter = 2 * radius;
            h.Color = Colors.Tomato;
        }


        // line from 2 points

        void Line_PP_Clicked (object sender, RoutedEventArgs args)
        {
            Point3D pt1 = Utils.RandomPoint (5);
            Point3D pt2 = Utils.RandomPoint (5);

            Line3D l3d = new Line3D (pt1, pt2);

            SetCurrentFigureTo3D ();
            Line3DView h = (CurrentFigure as Plot3D).Plot (l3d) as Line3DView;
            h.Color = Colors.MediumAquamarine;
        }

        //*****************************************************************************************

        // line from point and vector

        void Line_PV_Clicked (object sender, RoutedEventArgs args)
        {
            Point3D  p1 = Utils.RandomPoint (5);
            Vector3D v1 = Utils.RandomVector (5);

            Line3D l3d = new Line3D (p1, v1);

            SetCurrentFigureTo3D ();
            Line3DView h = (CurrentFigure as Plot3D).Plot (l3d) as Line3DView;
            h.Color = Colors.MediumAquamarine;
            h.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
        }

        void Polyline_Points_Clicked (object sender, RoutedEventArgs args)
        {
            SetCurrentFigureTo3D ();
            double r = Utils.RandomDouble (20);

            PointMarker p1 = new Sphere (new Point3D ( r, 0, 0));
            PointMarker p2 = new Sphere (new Point3D (-r, 0, 0));

            (CurrentFigure as Plot3D).Plot (p1);
            var qq = (CurrentFigure as Plot3D).Plot (p2);

            
            //PointMarker p1View = (CurrentFigure as Plot3D).Plot (p1) as PointMarker;
            //PointMarker p2View = (CurrentFigure as Plot3D).Plot (p2) as Sphere;

            //p1View.Color = Colors.Red;
            //p2View.Color = Colors.Black;

            List<Point3D> arcPoints = new List<Point3D> ();

            double dt = Math.PI / 32;

            for (double t = dt; t<Math.PI-dt; t+=dt)
            {
                Point3D pt = new Point3D (r * Math.Cos (t), r * Math.Sin (t), 0);
                arcPoints.Add (pt);
            }

            try
            {
                SetCurrentFigureTo3D ();
                Polyline3D pl3 = new Polyline3D (arcPoints);
                Polyline3DView pv = (CurrentFigure as Plot3D).Plot (pl3) as Polyline3DView;
                pv.Color = Colors.Red;
                pv.Thickness = 1;

                pv.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
                pv.ArrowLength = 5;
                pv.Decimation = 1;
            }

            catch (Exception ex)
            {
                Print (string.Format ("Polyline_Points_Click: {0}", ex.Message));
            }
        }

        double xFunction (double t)
        {
            return Math.Sin (2 * Math.PI * t);
        }

        double yFunction (double t)
        {
            return Math.Cos (2 * Math.PI * t);
        }

        double zFunction (double t)
        {
            return t;
        }

        void Polyline_Func_Clicked (object sender, RoutedEventArgs args)
        {
            SetCurrentFigureTo3D ();
            Polyline3D pl3 = new Polyline3D (xFunction, yFunction, zFunction, -3, 3, 1.0 / 32);
            Polyline3DView pv = (CurrentFigure as Plot3D).Plot (pl3) as Polyline3DView;
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        static int lineNumber = 1;
        object TextBoxLock = new object ();

        internal void Print (string str)
        {
            lock (TextBoxLock)
            {
                TextPane.Text += string.Format ("{0}: ", lineNumber++);
                TextPane.Text += str;
                TextPane.Text += "\n";
                TextPane.ScrollToEnd ();
            }

            EventLog.WriteLine (str);
        }
    }
}
