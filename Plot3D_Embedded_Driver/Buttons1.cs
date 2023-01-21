using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;
using Plot3D_Embedded;

namespace Plot3D_Embedded_Driver
{
    public partial class MainWindow : Window
    {
        //**********************************************************************************

        void Clear_Clicked (object sender, RoutedEventArgs args)
        {
            figure.Clear ();
        }

        //**********************************************************************************

        private double ZFunction (double x, double y)
        {
            //return y - x - 0.5;
            return 0.025 * (x * x + y * y - 6) + 0;
            //return 1 + x * y / 4;
        }

        void Surface_Clicked (object sender, RoutedEventArgs args)
        {
            try
            {
                List<double> xCoords = new List<double> ();
                List<double> yCoords = new List<double> ();

                for (double x=-40; x<=40; x+=10)
                    xCoords.Add (x);

                for (double y=-40; y<=40; y+=10)
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
                    CustomTicText = new List<string> () {"Abc", "Def", "Ghi"},
                    //TicTextDisplay = AxisLineView.TicTextDisplayOptions.Numbers,
                    TicTextSize = 0.25,
                    TicSize = 0.2,
                    
                    TicTextOffsetDistance = 0.1,                    

                    Label = "X axis label",
                };

                XAxisLine xAxis2 = new XAxisLine (xAxis1)
                {
                    ZeroPoint = new Point3D (0, 0, 0),
                    //TicsAt = null,
                    TicTextDisplay = AxisLineView.TicTextDisplayOptions.NoText,
                    Label = null,
                };

                figure.Plot (xAxis1);
                figure.Plot (xAxis2);



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
                //CartesianAxesBox box = new CartesianAxesBox (new Point3D (0, 0, 0), new Point3D (5, 4, 3));
                //CartesianAxesBox box = new CartesianAxesBox (new Point3D (-5, -5, -5), new Point3D (5, 5, 5));
                //CartesianAxesBox box = new CartesianAxesBox (new Point3D (12, 10, 5), new Point3D (15, 15, 15));

                CartesianAxesBox box;

                if (figure.ViewportBoundingBox.IsValid)
                    box = new CartesianAxesBox (figure.ViewportBoundingBox);
                else
                    box = new CartesianAxesBox (new Point3D (-5, -5, -5), new Point3D (5, 5, 5));

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
                figure.AxesTight = false;

                double radius = 0.25;

                PointMarker s1 = new Cube (new Point3D (0, 0, 0), radius);
                figure.Plot (s1);
                s1.Color = Colors.Red;
                //s1.Radius = 0.2;

                PointMarker s2 = new Cube (new Point3D (1, 0, 0), radius);  
                figure.Plot (s2);
                s2.Color = Colors.Green;

                PointMarker s3 = new Cube (new Point3D (2, 0, 0), radius);
                figure.Plot (s3);
                s3.Color = Colors.Blue;


                PointMarker s4 = new Cube (new Point3D (0, 1, 0), radius);
                figure.Plot (s4);
                s4.Color = Colors.GreenYellow;

                PointMarker s5 = new Tetrahedron (new Point3D (0, 0, 1), radius);
                //PointMarker s5 = new Cube (new Point3D (0, 0, 1), radius);
                figure.Plot (s5);
                s5.Color = Colors.IndianRed;

                figure.CenterOn = s2.BoundingBox.Center;

                Text3D txt = new Text3D (new Point3D (0,0,2), 0.5, string.Format ("P{0}", cnt++));
                Text3DView h1 = figure.Plot (txt) as Text3DView;

                txt = new Text3D (new Point3D (2,2,2), new Vector3D (1,0,0), new Vector3D (0,0,1), 0.5, string.Format ("P{0}", cnt++));
                h1 = figure.Plot (txt) as Text3DView;



                //figure.AxesTight = true;
                figure.DataAreaTitle = "Point Markers";
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception: {0}", ex.Message));
            }
        }


        //****************************************************************************************
        //****************************************************************************************
        //****************************************************************************************

        // Points_In_Line

        void Points_In_Line_Clicked (object sender, RoutedEventArgs args)
        {
            List<double> t = new List<double> ();

            for (int i = 0; i<50; i++)
                t.Add (2.0 * Math.PI * i / 50);

            List<Point3D> points = new List<Point3D> ();

            for (int i = 0; i<50; i++)
                points.Add (f_of_t (t [i]));


            if (false)
            {
                PointCloud3D pc = new PointCloud3D (points);
                PointCloud3DView h = figure.Plot (pc) as PointCloud3DView;

                h.Color = Colors.Red;
                h.Diameter = 0.4;
            }

            else
            {
                Polyline3D pl3 = new Polyline3D (points);
                Polyline3DView pv = figure.Plot (pl3) as Polyline3DView;

                int j = 20;
                Sphere pt = new Sphere (points [j]);
                SphereView p3v = figure.Plot (pt) as SphereView;
                pt.Radius = 0.1;

                //p3v.Color = Colors.Black;
                //p3v.Diameter = 0.45;

                PlotVector3D vect = new PlotVector3D (points [j], D_f_of_t (t [j]));
                PlotVector3DView vv = figure.Plot (vect) as PlotVector3DView;
                vv.Color = Colors.Red;

                figure.DataAreaTitle = "Points_In_Line";
             //   figure.AxesTight = true;

                figure.CenterOn = points [j];
            }
        }

        private Point3D f_of_t (double t)
        {
            double x = t * t;
            double y = t * Math.Sin (t);
            double z = 2 * t + 3;

            return new Point3D (x, y, z);
        }

        private Vector3D D_f_of_t (double t)
        {
            double x = 2 * t;
            double y = t * Math.Cos (t) + Math.Sin (t);
            double z = 2;

            return new Vector3D (x, y, z);
        }

        //****************************************************************************************

        private double range = 10;

        void PointCloud_Clicked (object sender, RoutedEventArgs args)
        {
            List<Point3D> points = new List<Point3D> ();

            for (int i=0; i<50; i++)
            {
                points.Add (Utils.RandomPoint (range));
            }

            double radius = 0.5 + Utils.RandomDouble (range / 50);

            //range *= 2;

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

            Line3DView.DashParameters dp = new Line3DView.DashParameters ();
            dp.OnPercent = 60;
            dp.Cycles = 10;
            h.SetDashParameters (dp);

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
            //double r = 10 + Utils.RandomDouble (20);

            //PlottedPoint3D p1 = new PlottedPoint3D (new Point3D (r, 0, 0));
            //PlottedPoint3D p2 = new PlottedPoint3D (new Point3D (-r, 0, 0));

            //Point3DView p1View = figure.Plot (p1) as Point3DView;
            //Point3DView p2View = figure.Plot (p2) as Point3DView;

            //p1View.Color = Colors.Red;
            //p2View.Color = Colors.Black;


            List<Point3D> arcPoints = new List<Point3D> ();

            arcPoints.Add (new Point3D (0, 0, 0));
            arcPoints.Add (new Point3D (-1, 1, 1));
            arcPoints.Add (new Point3D (10, 12, 14));

            //double dt = Math.PI / 8;

            //for (double t = dt; t<Math.PI-dt; t+=dt)
            //{
            //    Point3D pt = new Point3D (r * Math.Cos (t), r * Math.Sin (t), 0);
            //    arcPoints.Add (pt);
            //}

            try
            {
                   
                Polyline3D pl3 = new Polyline3D (CommonMath.Interpolation.Linear (arcPoints, 16));
                //Polyline3D pl3 = new Polyline3D (arcPoints);

                Polyline3DView pv = figure.Plot (pl3) as Polyline3DView;
                pv.Color = Colors.Red;
                pv.Thickness = 1;

                pv.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
                pv.ArrowLength = 5;
                pv.Decimation = 2;
            }

            catch (Exception ex)
            {
                Print (string.Format ("Polyline_Points_Click: {0}", ex.Message));
            }
        }

        double scale = 1;
        //double scale = 1e-4;
        //double scale = 1e-5;
        //double scale = 100;

        double xFunction (double t)
        {
            return 5 * scale * Math.Sin (2 * Math.PI * t) + 10;
        }

        double yFunction (double t)
        {
            return 4 * scale * Math.Cos (2 * Math.PI * t) + 20;
        }

        double zFunction (double t)
        {
            return 2 * t * scale + 30;
        }

        void Polyline_Func_Clicked (object sender, RoutedEventArgs args)
        {
            try
            {
                Polyline3D pl3 = new Polyline3D (xFunction, yFunction, zFunction, -3, 3, 1.0 / 32);
                pl3.PolylineView.Thickness = 2;
                Polyline3DView pv = figure.Plot (pl3) as Polyline3DView;
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
            }
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

            SphereView h1 = figure.Plot (new Sphere (p1)) as SphereView;
            Line3DView  h2 = figure.Plot (new Line3D (p1, v1)) as Line3DView;
            Plane3DView h3 = figure.Plot (new Plane3D (p1, v1)) as Plane3DView;

           // h1.Diameter = 0.1;
         //   h1.Color = Colors.Red;

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

            figure.AxesBoxOn = false;
            Plane3DView h1 = figure.Plot (new Plane3D (p1 +   0 * v1, v1)) as Plane3DView;
            Plane3DView h2 = figure.Plot (new Plane3D (p1 + 0.5 * v1, v1)) as Plane3DView;
            Plane3DView h3 = figure.Plot (new Plane3D (p1 + 1.0 * v1, v1)) as Plane3DView;
            Plane3DView h4 = figure.Plot (new Plane3D (p1 + 1.5 * v1, v1)) as Plane3DView;

            //SphereView h5 = figure.Plot (new Sphere (p1 + 0.75 * v1)) as SphereView;

            h1.Color = h2.Color = h3.Color = h4.Color = Colors.Red;
            h1.BackColor = h2.BackColor = h3.BackColor = h4.BackColor = Colors.Green;

            h1.BackOpacity = h2.BackOpacity = h3.BackOpacity = h4.BackOpacity = 0.5;
            h1.Opacity = h2.Opacity = h3.Opacity = h4.Opacity = 0.5;

           // h5.Diameter = 0.2;

            figure.AxesTight = true;
            figure.AxesBoxOn = true;
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


    }
}
