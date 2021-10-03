using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;
using Plot3D_Embedded;

namespace Plot3D_Embedded_Driver
{
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            EventLog.Open (@"..\..\log.txt", true);
            EventLog.WriteLine ("Plot3D_Embedded_Driver");

            try
            {
                InitializeComponent ();
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("Exception in InitializeComponent: {0}", ex.Message));
            }
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            //EventLog.WriteLine ("Main Window_Loaded");
        }

        private void Window_Closed (object sender, EventArgs e)
        {
            EventLog.WriteLine ("Main Window_Closed");
            EventLog.Close ();
        }

        //**********************************************************************************

        void Clear_Clicked (object sender, RoutedEventArgs args)
        {
            figure.Clear ();
        }

        //**********************************************************************************

        private double ZFunction (double x, double y)
        {
            //return y - x - 0.5;
            return 0.25 * (x * x + y * y - 6) + 0;
            //return 1 + x * y / 4;
        }

        void Surface_Clicked (object sender, RoutedEventArgs args)
        {
            try
            {
                List<double> xCoords = new List<double> ();
                List<double> yCoords = new List<double> ();

                for (double x=-4; x<=4; x+=1)
                    xCoords.Add (x);

                for (double y=-4; y<=4; y+=1)
                    yCoords.Add (y);

                double [,] zValues = new double [yCoords.Count, xCoords.Count];

                for (int r=0; r<yCoords.Count; r++)
                {
                    for (int c=0; c<xCoords.Count; c++)
                    {
                        zValues [r, c] = ZFunction (xCoords [c], yCoords [r]);
                    }
                }

                ZFunctionOfXY ZFunc = new ZFunctionOfXY (xCoords, yCoords, zValues);
                Surface3DView sv = figure.Plot (ZFunc) as Surface3DView;
                ZFunc.ShowTraceLines = true;
                //sv.Color = Colors.Fuchsia;
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception in UserMesh_Clicked: {0}", ex.Message));
            }
        }

        //****************************************************************************

        void CartesianAxis_Clicked (object sender, RoutedEventArgs args)
        {
            try
            {
                XAxisLine xAxis1 = new XAxisLine ()
                {
                    ZeroPoint = new Point3D (0, 1, 1),
                    TailCoordinate = 0,
                    HeadCoordinate = 4,

                    TicsAt = new List<double> () {1, 3, 5},
                    //CustomText = new List<string> () {"Abc", "Def", "Ghi"},
                    TextDisplay = AxisLineView.TextDisplayOptions.Numbers,
                    TextSize = 0.25,
                    TicSize = 0.2,
                    
                    TextOffsetDistance = 0.1,                    
                };

                YAxisLine yAxis1 = new YAxisLine ()
                {
                    //LineOrigin = new Point3D (0, 0, 0),
                    TailCoordinate = 0,
                    HeadCoordinate = 1,

                    Color = Colors.Green,
                };

                ZAxisLine zAxis1 = new ZAxisLine ()
                {
                    //LineOrigin = new Point3D (0, 0, 0),
                    TailCoordinate = 0,
                    HeadCoordinate = 1,

                    Color = Colors.Blue
                };

                figure.Plot (xAxis1);

                //xAxis1.TextDisplay = AxisLineView.TextDisplayOptions.Numbers;
                //figure.Refresh ();

                figure.Plot (yAxis1);
                figure.Plot (zAxis1);




                // line with tic marks
                //XAxisLine xAxis2 = new XAxisLine (new Point3D (0, 0, 0), -3, 15, new List<double> () { -2, -1, 0, 1, 5, 10 }, 0.3);
                //figure.Plot (xAxis2);

                // line with tic marks and default text
                //XAxisLine xAxis3 = new XAxisLine (new Point3D (0, 0, 0), -3, 15, new List<double> () {-2, -1, 0, 1, 5, 10}, 0.3, 0.3); 
                //xAxis3.AxisView.Color = Colors.DarkGreen;
                //figure.Plot (xAxis3);


                //XAxisLine xAxis4 = new XAxisLine (new Point3D (0, 0, 0), 0, 10,
                //                                  new List<double> () {3, 4, 5},
                //                                  new List<string> () {"A", "B", "C"}, 
                //                                  0.5, 0.5);
                //figure.Plot (xAxis4);

                //YAxisLine yAxis = new YAxisLine (0,  6, new Point3D (0, 0, 0), new List<double> () {3, 4, 5}, 1, 1); 
                //ZAxisLine zAxis = new ZAxisLine (0,  3, new Point3D (0, 0, 0), new List<double> () {3, 4, 5}, 1, 1); 


                //figure.Plot (yAxis);
                //figure.Plot (zAxis);



                //CartesianAxes3D axes1 = new CartesianAxes3D
                //{
                //    XMax = 4,
                //    XMin = -2
                //};

                //figure.Plot (axes1);

                //axes1.YMax = 3;
                //axes1.YMin = -0.2;

                //axes1.ZMax = 3;
                //axes1.ZMin = -0.2;
                //figure.AxesTight = true;
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception in CartesianAxis_Clicked: {0}", ex.Message));
            }
        }

        void CartesianBox_Clicked (object sender, RoutedEventArgs args)
        {
            try
            {
                double x1 = -2;
                double x2 =  2;
                double y1 = -1.5;
                double y2 =  1.5;
                double z1 = -3;
                double z2 =  3;

                if (figure.ViewportBoundingBox.IsValid)
                {
                    x1 = figure.ViewportBoundingBox.MinX;
                    x2 = figure.ViewportBoundingBox.MaxX;
                    y1 = figure.ViewportBoundingBox.MinY;
                    y2 = figure.ViewportBoundingBox.MaxY;
                    z1 = figure.ViewportBoundingBox.MinZ;
                    z2 = figure.ViewportBoundingBox.MaxZ;
                }

                double dx = x2 - x1;
                double dy = y2 - y1;
                double dz = z2 - z1;

                double d = Math.Min (dx, dy);
                d = Math.Min (d, dz);

                //CartesianAxisDescription xAxis = new CartesianAxisDescription (x1, x2, 3, d/10, d/12);
                CartesianAxisDescription xAxis = new CartesianAxisDescription (x1, x2, new List<double> () {-3, -2, -1, 0, 1, 2, 3}, 0.2, 0.3);



                CartesianAxisDescription yAxis = new CartesianAxisDescription (y1, y2, 3, d/10, d/12);
                CartesianAxisDescription zAxis = new CartesianAxisDescription (z1, z2, 3, d/10, d/12);

                CartesianAxesBox box = new CartesianAxesBox (xAxis, yAxis, zAxis);
                figure.Plot (box);
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception in CartesianBox_Clicked: {0}", ex.Message));
            }
        }
    
        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        int cnt = 1;

        void Point_Clicked (object sender, RoutedEventArgs args)
        {
            try
            {
                Point3D pt = Utils.RandomPoint (5);
                double radius = 0.15 + Utils.RandomDouble (0.25);

                PlottedPoint3D p3d = new PlottedPoint3D (pt);

                p3d.Radius = radius;
                Point3DView h = figure.Plot (p3d) as Point3DView;
                h.Color = Colors.Coral;

                Text3D txt = new Text3D (pt, 0.5, string.Format ("P{0}", cnt++));
                Text3DView h1 = figure.Plot (txt) as Text3DView;
                h1.Size = 0.8;

                if ((cnt & 1) == 1)
                    txt.TextView.OrientationFrozen = true;

                figure.AxesTight = false;
                figure.DataAreaTitle = "One Point";
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception: {0}", ex.Message));
            }
        }

        private double range = 10;

        void PointCloud_Clicked (object sender, RoutedEventArgs args)
        {
            List<Point3D> points = new List<Point3D> ();

            for (int i=0; i<50; i++)
            {
                points.Add (Utils.RandomPoint (range));
            }

            double radius = 0.5 + Utils.RandomDouble (range / 50);

            range *= 2;

            PointCloud3D pc = new PointCloud3D (points); //, radius);

            PointCloud3DView h =  figure.Plot (pc) as PointCloud3DView;

            h.Diameter = 2 * radius;
            h.Color = Colors.Tomato;
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        // line from 2 points

        void Line_PP_Clicked (object sender, RoutedEventArgs args)
        {
            Point3D pt1 = Utils.RandomPoint (5);
            Point3D pt2 = Utils.RandomPoint (5);

            Line3D l3d = new Line3D (pt1, pt2);

            Line3DView h =  figure.Plot (l3d) as Line3DView;
            h.Color = Colors.MediumAquamarine;
        }

        //*****************************************************************************************

        // line from point and vector

        void Line_PV_Clicked (object sender, RoutedEventArgs args)
        {
            try
            {
                figure.Clear ();

                //Point3D  p1 = Utils.RandomPoint (5);
                Vector3D v1 = Utils.RandomVector (5);

                //v1 = new Vector3D (1, 1, 1);

                List<Vector3D> basis = new List<Vector3D> () {new Vector3D (-1, 0.5, 0),
                                                              new Vector3D (0, 0.6, 0.3),
                                                              new Vector3D (0, 0.2, 1)};

                PlotVector3D pv = new PlotVector3D (v1, basis);
                PlotVector3DView vv = figure.Plot (pv) as PlotVector3DView;
                vv.ShowComponents = true;

                vv.Color = Colors.Black;
                vv.XComponentView.Color = Colors.Red;
                vv.YComponentView.Color = Colors.Green;
                vv.ZComponentView.Color = Colors.Blue;

                //CartesianAxisDescription cd = new CartesianAxisDescription ();
                //cd.min = -1;
                //cd.max = 2;
                //cd.ticsAt = new List<double> () {0};
                //CartesianAxesBox cb = new CartesianAxesBox (cd, cd, cd);
                //figure.Plot (cb);
            }

            catch (Exception ex)
            {
                Print (string.Format ("Line_PV_Click: {0}", ex.Message));
            }
        }

        void Polyline_Points_Clicked (object sender, RoutedEventArgs args)
        {
            double r = Utils.RandomDouble (20);

            PlottedPoint3D p1 = new PlottedPoint3D (new Point3D ( r, 0, 0));
            PlottedPoint3D p2 = new PlottedPoint3D (new Point3D (-r, 0, 0));

            Point3DView p1View = figure.Plot (p1) as Point3DView;
            Point3DView p2View = figure.Plot (p2) as Point3DView;

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
                Polyline3DView pv = figure.Plot (pl3) as Polyline3DView;
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
            Polyline3DView pv = figure.Plot (pl3) as Polyline3DView;
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        // plane from point and vector

        void Plane_PV_Clicked (object sender, RoutedEventArgs args)
        {
            Point3D  p1 = Utils.RandomPoint (5);
            Vector3D v1 = Utils.RandomVector (5);

            Print (string.Format ("Point: {0:0.0} Normal: {1:0.0}", p1, v1));

            Point3DView h1 = figure.Plot (new PlottedPoint3D (p1)) as Point3DView;
            Line3DView  h2 = figure.Plot (new Line3D (p1, v1)) as Line3DView;
            Plane3DView h3 = figure.Plot (new Plane3D (p1, v1)) as Plane3DView;

            h1.Diameter = 0.1;
            h1.Color = Colors.Red;

            h2.Color = Colors.Green;
            h2.ArrowEnds = Petzold.Media2D.ArrowEnds.End;

            h3.Color = Colors.PaleTurquoise;
            h3.BackColor = Colors.LightCoral;
        }

        //*****************************************************************************************

        // stack of translucent planes

        void Plane_Stack_Clicked (object sender, RoutedEventArgs args)
        {
            figure.AxesTight = false;

            Point3D p1 = Utils.RandomPoint (5);
            Vector3D v1 = Utils.RandomVector (5);

            Plane3DView h1 = figure.Plot (new Plane3D (p1 +   0 * v1, v1)) as Plane3DView;
            Plane3DView h2 = figure.Plot (new Plane3D (p1 + 0.5 * v1, v1)) as Plane3DView;
            Plane3DView h3 = figure.Plot (new Plane3D (p1 + 1.0 * v1, v1)) as Plane3DView;
            Plane3DView h4 = figure.Plot (new Plane3D (p1 + 1.5 * v1, v1)) as Plane3DView;

            Point3DView h5 = figure.Plot (new PlottedPoint3D (p1 + 0.75 * v1)) as Point3DView;

            h1.Color = h2.Color = h3.Color = h4.Color = Colors.Red;
            h1.BackColor = h2.BackColor = h3.BackColor = h4.BackColor = Colors.Green;

            h1.BackOpacity = h2.BackOpacity = h3.BackOpacity = h4.BackOpacity = 0.5;
            h1.Opacity = h2.Opacity = h3.Opacity = h4.Opacity = 0.5;

            h5.Diameter = 0.2;

            figure.SortByDistance ();
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        // Vainchtein's "Change of Basis" paper

        void Vainchtein_Clicked (object sender, RoutedEventArgs args)
        {
            Vector3D i1 = new Vector3D (1, 0, 0);
            Vector3D i2 = new Vector3D (0, 1, 0);
            Vector3D i3 = new Vector3D (0, 0, 1);

            Vector3D e1 = i1 + 2 * i2;
            Vector3D e2 = i1 + 2 * i2 + i3;
            Vector3D e3 = -i2  + i3;

            Vector3D u1 = i1 + i2;
            Vector3D u2 = i2 + 2 * i3;
            Vector3D u3 = 2 * i1 + i2  - i3;

            Line3DView h = figure.Plot (new Line3D (new Point3D (), i1)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
            h = figure.Plot (new Line3D (new Point3D (), i2)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
            h = figure.Plot (new Line3D (new Point3D (), i3)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End;

            h = figure.Plot (new Line3D (new Point3D (), e1)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End; h.Color = Colors.Red;
            h = figure.Plot (new Line3D (new Point3D (), e2)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End; h.Color = Colors.Red;
            h = figure.Plot (new Line3D (new Point3D (), e3)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End; h.Color = Colors.Red;

            h = figure.Plot (new Line3D (new Point3D (), u1)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End; h.Color = Colors.Green;
            h = figure.Plot (new Line3D (new Point3D (), u2)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End; h.Color = Colors.Green;
            h = figure.Plot (new Line3D (new Point3D (), u3)) as Line3DView; h.ArrowEnds = Petzold.Media2D.ArrowEnds.End; h.Color = Colors.Green;


        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        // change camera's distance to center and change the range of the rho slider

        void GetRho_Clicked (object sender, RoutedEventArgs args)
        {
            Rho_Text.Text = string.Format ("{0:#.##}", figure.CenterDistance);
        }

        void SetRho_Clicked (object sender, RoutedEventArgs args)
        {
            double dist = Convert.ToDouble (Rho_Text.Text);
            figure.CenterDistance = dist;
        }

        void GetPos_Clicked (object sender, RoutedEventArgs args)
        {
            Point3D pos = figure.CameraPosition;
            CameraPosX_Text.Text = string.Format ("{0:#.##}", pos.X);
            CameraPosY_Text.Text = string.Format ("{0:#.##}", pos.Y);
            CameraPosZ_Text.Text = string.Format ("{0:#.##}", pos.Z);
        }

        void SetPos_Clicked (object sender, RoutedEventArgs args)
        {
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
                TxtBox.Text += string.Format ("{0}: ", lineNumber++);
                TxtBox.Text += str;
                TxtBox.Text += "\n";
                TxtBox.ScrollToEnd ();
            }

            EventLog.WriteLine (str);
        }
    }
}
