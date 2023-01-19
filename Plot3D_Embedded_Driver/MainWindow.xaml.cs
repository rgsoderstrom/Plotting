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
            figure.Register_RhoChanged_Callback   (RhoChanged_Callback);
            figure.Register_ThetaChanged_Callback (ThetaChanged_Callback);
            figure.Register_PhiChanged_Callback   (PhiChanged_Callback);

            figure.Register_CenterChanged_Callback      (CenterChanged_Callback);
            figure.Register_AbsPositionChanged_Callback (AbsPosChanged_Callback);
            figure.Register_RelPositionChanged_Callback (RelPosChanged_Callback);
        }

        private void Window_Closed (object sender, EventArgs e)
        {
            EventLog.WriteLine ("Main Window_Closed");
            EventLog.Close ();
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        // change camera's distance to center and change the range of the rho slider





        void CenterChanged_Callback (object sender, Point3D newCenter)
        {
            //Print ("CenterChanged callback: " + string.Format ("{0:0.#}", newCenter));
            centerX.Text = string.Format ("{0:0.#}", newCenter.X);
            centerY.Text = string.Format ("{0:0.#}", newCenter.Y);
            centerZ.Text = string.Format ("{0:0.#}", newCenter.Z);
        }

        void AbsPosChanged_Callback (object sender, Point3D pos)
        {
            //Print ("AbsPositionChanged callback: " + string.Format ("{0:0.#}", pos));
            absPositionX.Text = string.Format ("{0:0.#}", pos.X);
            absPositionY.Text = string.Format ("{0:0.#}", pos.Y);
            absPositionZ.Text = string.Format ("{0:0.#}", pos.Z);
        }

        void RelPosChanged_Callback (object sender, Point3D pos)
        {
            //Print ("RelPositionChanged callback: " + string.Format ("{0:0.#}", pos));
            relPositionX.Text = string.Format ("{0:0.#}", pos.X);
            relPositionY.Text = string.Format ("{0:0.#}", pos.Y);
            relPositionZ.Text = string.Format ("{0:0.#}", pos.Z);
        }

        void RhoChanged_Callback (object sender, double rho)
        {
            Rho_Text.Text = string.Format ("{0:0.0#}", rho);
        }

        void ThetaChanged_Callback (object sender, double theta)
        {
            Theta_Text.Text = string.Format ("{0:0.0#}", theta);
        }

        void PhiChanged_Callback (object sender, double phi)
        {
            Phi_Text.Text = string.Format ("{0:0.0#}", phi);
        }

        //******************************************************************************************

        private void CenterX_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                args.Handled = true;

                if (double.TryParse (centerX.Text, out double cx))
                {
                    Point3D center = figure.CenterOn;
                    figure.CenterOn = new Point3D (cx, center.Y, center.Z);
                }
            }
        }

        private void CenterY_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                args.Handled = true;

                if (double.TryParse (centerY.Text, out double cy))
                {
                    Point3D center = figure.CenterOn;
                    figure.CenterOn = new Point3D (center.X, cy, center.Z);
                }
            }
        }

        private void CenterZ_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                args.Handled = true;

                if (double.TryParse (centerZ.Text, out double cz))
                {
                    Point3D center = figure.CenterOn;
                    figure.CenterOn = new Point3D (center.X, center.Y, cz);
                }
            }
        }

        private void RelX_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                args.Handled = true;

                if (double.TryParse (relPositionX.Text, out double cx))
                {
                    Point3D pos = figure.CameraRelPosition;
                    figure.CameraRelPosition = new Point3D (cx, pos.Y, pos.Z);
                }
            }
        }

        private void RelY_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                args.Handled = true;

                if (double.TryParse (relPositionY.Text, out double cy))
                {
                    Point3D pos = figure.CameraRelPosition;
                    figure.CameraRelPosition = new Point3D (pos.X, cy, pos.Z);
                }
            }
        }

        private void RelZ_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                args.Handled = true;

                if (double.TryParse (relPositionZ.Text, out double cz))
                {
                    Point3D pos = figure.CameraRelPosition;
                    figure.CameraRelPosition = new Point3D (pos.X, pos.Y, cz);
                }
            }
        }

        private void AbsX_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
        }

        private void AbsY_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
        }

        private void AbsZ_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
        }




        private void Rho_Text_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.Enter)
            {
                args.Handled = true;

                if (double.TryParse (Rho_Text.Text, out double rho))
                {
                    figure.CenterDistance = rho;
                }
            }
        }

        private void Theta_Text_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
        }

        private void Phi_Text_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs args)
        {
        }

        void GetPos_Clicked (object sender, RoutedEventArgs args)
        {
            //Point3D pos = figure.CameraPosition;
            //CameraPosX_Text.Text = string.Format ("{0:#.##}", pos.X);
            //CameraPosY_Text.Text = string.Format ("{0:#.##}", pos.Y);
            //CameraPosZ_Text.Text = string.Format ("{0:#.##}", pos.Z);
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
