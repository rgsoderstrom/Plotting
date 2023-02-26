using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Common;

using Plot2D_Embedded;

namespace Plot2D_Embedded_Driver
{
    public partial class MainWindow : Window
    {
        Random random = new Random ();

        public MainWindow ()
        {
            EventLog.Open (@"..\..\Log.txt", true);
            EventLog.WriteLine ("Plot2D_Kernel_Driver");

            InitializeComponent ();
        }

        //*******************************************************************************************************

        private void ClearButton_Click (object sender, RoutedEventArgs e)
        {
            figure.Clear ();
           // figure.MouseEnabled ^= true;
           // Print (string.Format ("Enable = {0}", figure.MouseEnabled));
        }

        //***********************************************************************************************************
        //***********************************************************************************************************
        //***********************************************************************************************************


        private void Text_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                figure.AxesEqual = false;
                Point pt = new Point (10, 10);

                TextView text = new TextView (pt, "Hello 1234567890");
                text.FontSizeAppInUnits = 0.25;
                text.Angle = -10;
                text.Color = Brushes.MediumOrchid;

                figure.Plot (text);
                figure.RectangularGridOn = true;

                text.RegisterForMouseLeftClick (Text_LeftClick);
            }

            catch (Exception ex)
            {
                Print (string.Format ("Exception: {0}", ex.Message));
            }
        }

        private void Text_LeftClick (object sender, Point pt)
        {
            Print (string.Format ("Text left click at {0:0.00}", pt));
        }

        private void Refresh_Click (object sender, RoutedEventArgs e)
        {
            figure.Refresh ();
        }

        CoordinateAxesView cv;

        private void Axes_Click (object sender, RoutedEventArgs e)
        {
            //List<NameValuePair> properties = new List<NameValuePairs> ();
            //properties.Add (new NameValuePair ("origin",     new Point (20, 30));
            //properties.Add (new NameValuePair ("relative",   true);
            //properties.Add (new NameValuePair ("axisLength", 50);
            //properties.Add (new NameValuePair ("firstTic",   10);
            //properties.Add (new NameValuePair ("ticStep",    5);
            //
            //figure.Plot (new CoordinateAxesView (properties);



            List<Point> hyper = new List<Point> ();

            for (double x = -5; x<=5; x+=0.1)
                hyper.Add (new Point (x, Math.Sqrt (1 + x * x)));

            LineView h = new LineView (hyper);
            h.LineStyle = LineView.DrawingStyle.Dashes;
            figure.Plot (h);


            cv = new CoordinateAxesView (new Point (3, 2), true, 4, 6, 0.8, 1, 3)
            {
                PositiveOnly = true
            };

            figure.Plot (cv);

            figure.RectangularGridOn = true;
            figure.AxesEqual = true;
        }

        private void ModAxes_Click (object sender, RoutedEventArgs e)
        {
            if (cv == null)
                return;

            figure.Remove (cv);

            cv.PositiveOnly = false;
            //cv.FirstTic = 3;
            cv.TicStep = 1;
            
            figure.Plot (cv);
        }



        //***********************************************************************************************************
        //***********************************************************************************************************
        //***********************************************************************************************************


        private void RectGridButton_Click (object sender, RoutedEventArgs e)
        {
            figure.RectangularGridOn = (bool) (sender as CheckBox).IsChecked;
        }

        private void PolarGridButton_Click (object sender, RoutedEventArgs e)
        {
            figure.PolarGridOn = (bool) (sender as CheckBox).IsChecked;
        }

        private void Equal_Click  (object sender, RoutedEventArgs e) {figure.AxesEqual  = (bool) AxesEqual_Button.IsChecked;}
        private void Tight_Click  (object sender, RoutedEventArgs e) {figure.AxesTight  = (bool) AxesTight_Button.IsChecked;}
        private void Frozen_Click (object sender, RoutedEventArgs e) {figure.AxesFrozen = (bool) AxesFrozen_Button.IsChecked;}
        private void Hold_Click   (object sender, RoutedEventArgs e) {figure.Hold       = (bool) PlotHold_Button.IsChecked;}

        private void MouseEnable_Click (object sender, RoutedEventArgs args) {figure.MouseEnabled = (bool) MouseEnable_Button.IsChecked;}

        private void CheckButtonStates ()
        {
            RectGrid_Button.IsChecked = figure.RectangularGridOn;
            PolarGrid_Button.IsChecked = figure.PolarGridOn;

            //GridLinesFixed_Button.IsChecked = figure.FixedGridLines;
            //GridValuesFixed_Button.IsChecked = figure.FixedGridValues;

            AxesEqual_Button.IsChecked = figure.AxesEqual;
            AxesTight_Button.IsChecked = figure.AxesTight;
            AxesFrozen_Button.IsChecked = figure.AxesFrozen;
            PlotHold_Button.IsChecked = figure.Hold;

            MouseEnable_Button.IsChecked = figure.MouseEnabled;
        }

        private void GridLinesFixed_Click (object sender, RoutedEventArgs args)
        {
//            figure.FixedGridLines = (bool) GridLinesFixed_Button.IsChecked;
        }

        private void GridValuesFixed_Click (object sender, RoutedEventArgs args)
        {
  //           figure.FixedGridValues = (GridLinesFixed_Button.IsChecked == false);
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            Title = "Plot2D Kernel Test Driver";
            figure.DataAreaTitle = "Plot2D Kernel";

            //GridLinesFixed_Button.IsChecked = figure.FixedGridLines;
            //GridValuesFixed_Button.IsChecked = figure.FixedGridValues;

            figure.ControlFlagChangeHandlers += CheckButtonStates;
            CheckButtonStates ();
        }

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
