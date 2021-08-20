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

namespace Plot3D_Embedded
{
    public partial class Bare3DPlot
    {
        //************************************************************************************************************

        private void UserControl_Loaded (object sender, RoutedEventArgs e)
        {
            //EventLog.WriteLine ("UserControl_Loaded");

            Canvas.SetLeft (InnerBorder, DataAreaLeft);
            Canvas.SetTop (InnerBorder, DataAreaTop);

            Viewport.Focusable = true;
            Keyboard.Focus (Viewport);
            Viewport.Camera = Camera3D.Camera; ;
        }

        private void UserControl_SizeChanged (object sender, SizeChangedEventArgs e)
        {
            //EventLog.WriteLine ("UserControl_SizeChanged");

            double w = OuterCanvas.ActualWidth - (DataAreaLeft + DataAreaRight);
            if (w > 0) InnerBorder.Width = w;

            double h = OuterCanvas.ActualHeight - (DataAreaTop + DataAreaBottom);
            if (h > 0) InnerBorder.Height = h;

            PhiScrollbar.Height = InnerBorder.Height - (ThetaScrollbar.Height + 3);
            PhiScrollbar.VerticalAlignment = VerticalAlignment.Top;
        }

        private void PhiScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.Phi = e.NewValue;
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
            //SortByDistance ();
        }

        private void WidthScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.Width = e.NewValue;
        }

        private void RhoScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.Rho = e.NewValue;
        }

        private void FovScrollbar_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera3D.FOV = e.NewValue;
        }

        //******************************************************************************************

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
            Camera3D.OnKeyDown (args);
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
