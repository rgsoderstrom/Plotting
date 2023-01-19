using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Media3D;

using WPF3D.Lighting;
using WPF3D.Cameras;
using WPF3D.MouseTracking;
using Common;
using static Plot3D_Embedded.Bare3DPlot;

namespace Plot3D_Embedded
{
    public partial class Bare3DPlot
    {
        //************************************************************************************************************

        private void UserControl_Loaded (object sender, RoutedEventArgs e)
        {
            //EventLog.WriteLine ("UserControl_Loaded");

            Canvas.SetLeft (DataTitle, (DataAreaX1 + DataAreaX0 - 8 * DataTitle.Text.Length) / 2);
            Canvas.SetTop  (DataTitle, DataAreaY0 * 0.4);

            Canvas.SetLeft (InnerBorder, DataAreaLeft);
            Canvas.SetTop  (InnerBorder, DataAreaTop);

            Viewport.Focusable = true;
            Keyboard.Focus (Viewport);
            Viewport.Camera = Camera3D.Camera; ;
        }

        private void UserControl_SizeChanged (object sender, SizeChangedEventArgs e)
        {
            //EventLog.WriteLine ("UserControl_SizeChanged");

            Canvas.SetLeft (DataTitle, (DataAreaX1 + DataAreaX0 - 8 * DataTitle.Text.Length) / 2);
            Canvas.SetTop  (DataTitle, DataAreaY0 * 0.4);

            double w = OuterCanvas.ActualWidth - (DataAreaLeft + DataAreaRight);
            if (w > 0) InnerBorder.Width = w;

            double h = OuterCanvas.ActualHeight - (DataAreaTop + DataAreaBottom);
            if (h > 0) InnerBorder.Height = h;

            PhiScrollbar.Height = InnerBorder.Height - (ThetaScrollbar.Height + 3);
            PhiScrollbar.VerticalAlignment = VerticalAlignment.Top;
        }

        //**************************************************************************************

        // Callbacks for Camera position spherical coordinates

        public delegate void RhoChanged_Callback         (object sender, double rho); // Signature of application callbacks looks like this
        public delegate void ThetaChanged_Callback       (object sender, double rho); 
        public delegate void PhiChanged_Callback         (object sender, double rho); 
        public delegate void CenterChanged_Callback      (object sender, Point3D center); 
        public delegate void AbsPositionChanged_Callback (object sender, Point3D cameraPosition); 
        public delegate void RelPositionChanged_Callback (object sender, Point3D cameraPosition); 

        private event RhoChanged_Callback         RhoChangedCallbacks;    // list of all the callbacks
        private event ThetaChanged_Callback       ThetaChangedCallbacks; 
        private event PhiChanged_Callback         PhiChangedCallbacks;
        private event CenterChanged_Callback      CenterChangedCallbacks;
        private event AbsPositionChanged_Callback AbsPositionChangedCallbacks;
        private event RelPositionChanged_Callback RelPositionChangedCallbacks;

        public void Register_RhoChanged_Callback         (RhoChanged_Callback    cb)      {RhoChangedCallbacks         += cb; cb (RhoScrollbar,   RhoScrollbar.Value); }
        public void Register_ThetaChanged_Callback       (ThetaChanged_Callback  cb)      {ThetaChangedCallbacks       += cb; cb (ThetaScrollbar, ThetaScrollbar.Value); }
        public void Register_PhiChanged_Callback         (PhiChanged_Callback    cb)      {PhiChangedCallbacks         += cb; cb (PhiScrollbar,   PhiScrollbar.Value); }
        public void Register_CenterChanged_Callback      (CenterChanged_Callback cb)      {CenterChangedCallbacks      += cb; cb (Camera3D,       Camera3D.CenterOn); }
        public void Register_AbsPositionChanged_Callback (AbsPositionChanged_Callback cb) {AbsPositionChangedCallbacks += cb; cb (Camera3D,       Camera3D.AbsPosition); }
        public void Register_RelPositionChanged_Callback (RelPositionChanged_Callback cb) {RelPositionChangedCallbacks += cb; cb (Camera3D,       Camera3D.RelPosition); }

        public void Unregister_RhoChanged_Callback         (RhoChanged_Callback    cb)      {RhoChangedCallbacks         -= cb;}
        public void Unregister_ThetaChanged_Callback       (ThetaChanged_Callback  cb)      {ThetaChangedCallbacks       -= cb;}
        public void Unregister_PhiChanged_Callback         (PhiChanged_Callback    cb)      {PhiChangedCallbacks         -= cb;}
        public void UnRegister_CenterChanged_Callback      (CenterChanged_Callback cb)      {CenterChangedCallbacks      -= cb;}
        public void Unregister_AbsPositionChanged_Callback (AbsPositionChanged_Callback cb) {AbsPositionChangedCallbacks -= cb;}
        public void Unregister_RelPositionChanged_Callback (RelPositionChanged_Callback cb) {RelPositionChangedCallbacks -= cb;}

        private void PhiScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.Phi = e.NewValue;
            PhiChangedCallbacks?.Invoke (sender, e.NewValue);
            RelPositionChangedCallbacks?.Invoke (sender, Camera3D.RelPosition);
            AbsPositionChangedCallbacks?.Invoke (sender, Camera3D.AbsPosition);
            //SortByDistance ();

            /**
            foreach (ViewportObject v in displayObjects)
            {
                if (v is Text3D)
                {
                    (v as Text3D).Up = Camera3D.Up;
                    (v as Text3D).Right = Camera3D.Right;

                    Draw ();
                }
            }
            **/
        }

        private void ThetaScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.Theta = e.NewValue;
            ThetaChangedCallbacks?.Invoke (sender, e.NewValue);
            RelPositionChangedCallbacks?.Invoke (sender, Camera3D.RelPosition);
            AbsPositionChangedCallbacks?.Invoke (sender, Camera3D.AbsPosition);
            //SortByDistance ();
        }

        private void RhoScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.Rho = e.NewValue;
            RhoChangedCallbacks?.Invoke (sender, e.NewValue);
            RelPositionChangedCallbacks?.Invoke (sender, Camera3D.RelPosition);
            AbsPositionChangedCallbacks?.Invoke (sender, Camera3D.AbsPosition);
        }

        private void WidthScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.Width = e.NewValue;
        }
        
        private void FovScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.FOV = e.NewValue;
        }

        //*******************************************************************************************

        protected override void OnMouseWheel (MouseWheelEventArgs args)
        {
            base.OnMouseWheel (args);

            if (Camera3D.Camera is OrthographicCamera)
            {
                if (args.Delta < 0) WidthScrollbar.Value *= 1.1;
                else WidthScrollbar.Value *= 0.8;
            }
            else
            {
                if (args.Delta < 0) RhoScrollbar.Value *= 1.1;
                else RhoScrollbar.Value *= 0.8;
            }
        }

        //******************************************************************************************

        protected override void OnKeyDown (KeyEventArgs args)
        {
            base.OnKeyDown (args);
            args.Handled = true;

            // Camera3D.OnKeyDown (args);

            //Point3D  CamAt = Camera3D.Position;
            //Vector3D LookAt = Camera3D.Direction;

            //Console.WriteLine ("At = {0:0.00}", CamAt);
            //Console.WriteLine ("Dir = {0:0.00}", LookAt);
        }

        //***********************************************************************************************************
        
        protected override void OnMouseUp (MouseButtonEventArgs e)
        {
            base.OnMouseUp (e);
            mouseTracking.OnMouseUp (e);
        }

        //***********************************************************************************************************

        protected override void OnMouseDown (MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Right)
            {
                Console.WriteLine ("right button down");

               

                //Console.WriteLine ("Camera At   = {0:0.00}", Camera3D.Position);
                //Console.WriteLine ("Camera On   = {0:0.00}", Camera3D.Center);
                //Console.WriteLine ("Looking Dir = {0:0.00}", Camera3D.Direction);

                //  Vector3D up = Camera3D.Up;
                // Vector3D right = Camera3D.Right;

                // Console.WriteLine ("Up = {0:0.00}", up);
                //  Console.WriteLine ("right = {0:0.00}", right);



                //Camera3D.CenterOn (new Point3D (0, 0, 0));// (Center);
              //  Center += new Vector3D (0.2, 0, 0);


               // Camera3D.Camera.Position += right;
            }


            base.OnMouseDown (args);
            mouseTracking.OnMouseDown (args);
        }

        //***********************************************************************************************************

        protected override void OnMouseMove (MouseEventArgs args)
        {
            base.OnMouseMove (args);
            mouseTracking.OnMouseMove (args);

            if (args.LeftButton == MouseButtonState.Pressed)
            {
                OrientTextToCamera ();
                SortByDistance (); // only needed if translucent objects in Viewport
            }
        }
    }
}








