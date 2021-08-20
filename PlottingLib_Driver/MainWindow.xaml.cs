using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;

using PlottingLib;
using Plot2D_Embedded;
using Plot3D_Embedded;


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

        List<Plot2D> Figures2D = new List<Plot2D> ();       
        Plot2D Fig2D = null;

        List<Plot3D> Figures3D = new List<Plot3D> ();       
        Plot3D Fig3D = null;

        IPlotCommon CurrentFigure = null;

        private void Plot2DFigure_Closed (object sender, EventArgs e)
        {
            if (sender is Plot2D)
            {
                Figures2D.Remove (sender as Plot2D);

                if (Figures2D.Count > 0)
                    Fig2D = Figures2D [Figures2D.Count - 1];
                else
                {
                    Fig2D = null;
                    Basic2D.IsEnabled = false; // disable buttons
                    Composite2D.IsEnabled = false;
                }

                CurrentFigure = Fig2D;
            }
        }

        private void Plot3DFigure_Closed (object sender, EventArgs e)
        {
            if (sender is Plot3D)
            {
                Figures3D.Remove (sender as Plot3D);

                if (Figures3D.Count > 0)
                    Fig3D = Figures3D [Figures3D.Count - 1];
                else
                {
                    Fig3D = null;
                    Basic3D.IsEnabled = false; // disable buttons
                }

                CurrentFigure = Fig3D;
            }
        }

        //*******************************************************************************************

        PlotFigure pf = null;

        void NewFigButton_Click (object sender, RoutedEventArgs e) 
        {
            pf = new PlotFigure ();
        }

        void New2DFigButton_Click (object sender, RoutedEventArgs e) 
        {
            if (pf != null)
            {
                Fig2D = new Plot2D (pf);
                pf.Close ();
                pf = null; ;
            }

            else
                Fig2D = new Plot2D ();

            Fig2D.Title += " - Plot2D";
            Fig2D.Closed += Plot2DFigure_Closed;
            Fig2D.Activated += Fig2D_Activated;

            Figures2D.Add (Fig2D);
            Basic2D.IsEnabled = true;  // buttons
            Composite2D.IsEnabled = true;
            HoldBox.IsChecked = Fig2D.Hold;
            CurrentFigure = Fig2D;
        }

        void New3DFigButton_Click (object sender, RoutedEventArgs e) 
        { 
            if (pf != null)
            {
                Fig3D = new Plot3D (pf);
                pf.Close ();
                pf = null;;
            }

            else
                Fig3D = new Plot3D ();

            Fig3D.Title += " - Plot3D";
            Fig3D.Closed += Plot3DFigure_Closed;
            Fig3D.Activated += Fig3D_Activated;

            Figures3D.Add (Fig3D);
            Basic3D.IsEnabled = true;  // buttons
            CurrentFigure = Fig3D;
        }

        private void Fig2D_Activated (object sender, EventArgs e)
        {
            if (sender is Plot2D)
            {
                Fig2D = sender as Plot2D;
                HoldBox.IsChecked = Fig2D.Hold;
                CurrentFigure = Fig2D;
            }
        }

        private void Fig3D_Activated (object sender, EventArgs e)
        {
            if (sender is Plot3D)
            {
                Fig3D = sender as Plot3D;
                HoldBox.IsChecked = Fig3D.Hold;
                CurrentFigure = Fig3D;
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

        Random random = new Random ();

        void VectorButton_Click (object sender, RoutedEventArgs e) 
        {
            Point pt = new Point (random.NextDouble (), random.NextDouble ());
            Vector v = new Vector (random.NextDouble (), random.NextDouble ());

            VectorView VV = new VectorView (pt, v);
            VV.ShowComponents = true;
            Fig2D.Plot (VV);
        }

        void VectorFieldButton_Click (object sender, RoutedEventArgs e)
        {
            int N = 20;
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
            Fig2D.Plot (vfv);
        }

        void CircleButton_Click (object sender, RoutedEventArgs e) {EllipseView h = new EllipseView (new Point (2, 2), 1); Fig2D.Plot (h);}

        void EllipseButton_Click (object sender, RoutedEventArgs e) 
        {
            EllipseView h = new EllipseView () {Center = new Point (-1, 2), Width = 2, Height = 1, Angle = 10};

            Fig2D.Plot (h);
            Fig2D.AxesEqual = true;
            Fig2D.RectangularGridOn = true;
        }


        void RectangleButton_Click (object sender, RoutedEventArgs e) {RectangleView h = new RectangleView (new Point (2.5, 5), 1, 0.5); h.Angle = 10; Fig2D.Plot (h);}


        void RotateRectButton_Click (object sender, RoutedEventArgs e) { }
        void MoveRectButton_Click (object sender, RoutedEventArgs e) { }


        void SmoothCurveButton_Click (object sender, RoutedEventArgs e) 
        {
            LineView lv = new LineView (new Point (1, 2), new Point (2, 3)); 
            Fig2D.Plot (lv);
            lv.Color = Brushes.Red;
        }


        void CurveButton2_Click (object sender, RoutedEventArgs e) { }

        //**********************************************************************************

        //
        // 3D Objects
        //

        void Point_Clicked (object sender, RoutedEventArgs e) 
        {
            Point3D pt = Plot3D_Embedded.Utils.RandomPoint (2.0);
            PlottedPoint3D ppt = new PlottedPoint3D (pt);
            ppt.Diameter = 0.2;
            
            Fig3D.Plot (ppt);
        }

        void PointCloud_Clicked (object sender, RoutedEventArgs args)
        {
            List<Point3D> points = new List<Point3D> ();

            for (int i=0; i<50; i++)
                points.Add (Utils.RandomPoint (15));

            double radius = 0.2 + Utils.RandomDouble (0.3);

            PointCloud3D pc = new PointCloud3D (points); //, radius);

            PointCloud3DView h =  Fig3D.Plot (pc) as PointCloud3DView;

            h.Diameter = 2 * radius;
            h.Color = Colors.Tomato;
        }


        // line from 2 points

        void Line_PP_Clicked (object sender, RoutedEventArgs args)
        {
            Point3D pt1 = Utils.RandomPoint (5);
            Point3D pt2 = Utils.RandomPoint (5);

            Line3D l3d = new Line3D (pt1, pt2);

            Line3DView h =  Fig3D.Plot (l3d) as Line3DView;
            h.Color = Colors.MediumAquamarine;
        }

        //*****************************************************************************************

        // line from point and vector

        void Line_PV_Clicked (object sender, RoutedEventArgs args)
        {
            Point3D  p1 = Utils.RandomPoint (5);
            Vector3D v1 = Utils.RandomVector (5);

            Line3D l3d = new Line3D (p1, v1);

            Line3DView h =  Fig3D.Plot (l3d) as Line3DView;
            h.Color = Colors.MediumAquamarine;
            h.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
        }

        void Polyline_Points_Clicked (object sender, RoutedEventArgs args)
        {
            double r = Utils.RandomDouble (20);

            PlottedPoint3D p1 = new PlottedPoint3D (new Point3D ( r, 0, 0));
            PlottedPoint3D p2 = new PlottedPoint3D (new Point3D (-r, 0, 0));

            Point3DView p1View = Fig3D.Plot (p1) as Point3DView;
            Point3DView p2View = Fig3D.Plot (p2) as Point3DView;

            p1View.Color = Colors.Red;
            p2View.Color = Colors.Black;

            List<Point3D> arcPoints = new List<Point3D> ();

            double dt = Math.PI / 32;

            for (double t = dt; t<Math.PI-dt; t+=dt)
            {
                Point3D pt = new Point3D (r * Math.Cos (t), r * Math.Sin (t), 0);
                arcPoints.Add (pt);
            }

            try
            {
                Polyline3D pl3 = new Polyline3D (arcPoints);
                Polyline3DView pv = Fig3D.Plot (pl3) as Polyline3DView;
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
            Polyline3D pl3 = new Polyline3D (xFunction, yFunction, zFunction, -3, 3, 1.0 / 32);
            Polyline3DView pv = Fig3D.Plot (pl3) as Polyline3DView;
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************





        //*****************************************************************************************

        //static int lineNumber = 1;
        object TextBoxLock = new object ();

        internal void Print (string str)
        {
            lock (TextBoxLock)
            {
                //TextPane.Text += string.Format ("{0}: ", lineNumber++);
                TextPane.Text += str;
                TextPane.Text += "\n";
                TextPane.ScrollToEnd ();
            }

            EventLog.WriteLine (str);
        }




    }
}
