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
    public partial class Bare2DPlot
    {
        //********************************************************************************************************************
        //
        // Keyboard and Mouse event handlers
        //

        //
        // scrolling
        //

        bool  scrolling = false;
        Point InitialCanvasCoords; // canvas coords under pointer when scrolling begun (i.e. left button pressed)
        Point InitialViewportCenter;     // Viewport when scrolling began

        protected void OuterCanvas_SizeChanged (object sender, SizeChangedEventArgs args)
        {
            try
            {
                double outerCanvasWidth = args.NewSize.Width;
                double outerCanvasHeight = args.NewSize.Height;

                InnerBorder.Width  = outerCanvasWidth  - (DataAreaLeft + DataAreaRight);
                InnerBorder.Height = outerCanvasHeight - (DataAreaTop + DataAreaBottom);
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("OuterCanvas_SizeChanged Exception: {0}", ex.Message));
                EventLog.WriteLine (string.Format ("OuterCanvas_SizeChanged Exception: {0}", ex.StackTrace));
            }
        }


        /***********
            occasional Threading issue:
                InnerCanvas_MouseMove Enter
                InnerCanvas_MouseMove Leave
                InnerCanvas_MouseLeftButtonDown Enter
                InnerCanvas_MouseMove Enter
                InnerCanvas_MouseMove Leave
                InnerCanvas_MouseLeftButtonDown Leave
                InnerCanvas_MouseMove Enter
                InnerCanvas_MouseMove Leave
        ************/

        protected void InnerCanvas_SizeChanged (object sender, SizeChangedEventArgs args)
        {
            //EventLog.WriteLine ("Plot2D_Embedded InnerCanvas_SizeChanged");

            try
            {
                if (args.PreviousSize.Width == 0 || args.PreviousSize.Height == 0)
                    return;

                if (AxesFrozen == false)
                {
                    double dx = (args.NewSize.Width - args.PreviousSize.Width) / mx;
                    double dy = (args.NewSize.Height - args.PreviousSize.Height) / -my;

                    Viewport.Inflate (dx, dy);

                    if (rectangularGrid != null)
                        rectangularGrid.ClearTicValues ();
                }

                Draw ();
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("InnerCanvas_SizeChanged Exception: {0}", ex.Message));
                EventLog.WriteLine (string.Format ("InnerCanvas_SizeChanged Exception: {0}", ex.StackTrace));
            }
        }

        //********************************************************************************************************************

        protected void InnerCanvas_MouseLeftButtonDown (object sender, MouseButtonEventArgs args)
        {
            InitialCanvasCoords = args.GetPosition (InnerCanvas);
            InitialViewportCenter = Viewport.Center;

            Mouse.Capture (InnerCanvas);
            InnerCanvas.Cursor = Cursors.ScrollAll;
            scrolling = true;
        }

        //********************************************************************************************************************

        protected void InnerCanvas_LostMouseCapture (object sender, MouseEventArgs e)
        {
            InnerCanvas_MouseLeftButtonUp (sender, null);
        }

        //********************************************************************************************************************

        protected void InnerCanvas_MouseLeftButtonUp (object sender, MouseButtonEventArgs e)
        {
            scrolling = false;
            Mouse.Capture (null);
            InnerCanvas.Cursor = Cursors.Arrow;
        }

        //********************************************************************************************************************

        protected void InnerCanvas_MouseMove (object sender, MouseEventArgs args)
        {
            if (scrolling == true)
            {
                // only write to these if necessary. eliminate unneeded drawing
                if (AxesTight == true)  AxesTight = false;
                if (AxesFrozen == true) AxesFrozen = false;

                try
                {
                    Point canvasCoords = args.GetPosition (InnerCanvas);
                    Vector delta = canvasCoords - InitialCanvasCoords;
                    Viewport.MoveTo (InitialViewportCenter + new Vector (-delta.X / mx, -delta.Y / my));
                    Draw ();
                }

                catch (Exception ex)
                {
                    EventLog.WriteLine (string.Format ("InnerCanvas_MouseMove Exception: {0}", ex.Message));
                    EventLog.WriteLine (string.Format ("InnerCanvas_MouseMove Exception: {0}", ex.StackTrace));
                }
            } 
        }

        //********************************************************************************************************************

        //
        // zooming
        //

        bool zoomAboutMousePointer = true;

        public void Zoom (double factor) // zoom in or out under program control rather than mouse wheel
        {
            if (AxesTight == true)  AxesTight = false;
            if (AxesFrozen == true) AxesFrozen = false;

            Viewport.Scale (factor, factor);

            if (rectangularGrid != null) rectangularGrid.ClearTicValues ();
            if (polarGrid != null) polarGrid.ClearTicValues ();

            Draw ();
        }

        //********************************************************************************************************************

        protected void InnerCanvas_MouseWheel (object sender, MouseWheelEventArgs args)
        {
            // only write to these if necessary. avoids unneeded drawing
            if (AxesTight == true)  AxesTight = false;
            if (AxesFrozen == true) AxesFrozen = false;
                
            try
            {
                double s1 = 0.8;
                double s2 = 1 / s1;

                double scale = args.Delta < 0 ? s1 : s2;

                if (zoomAboutMousePointer == true)
                {
                    Point pointerCanvasCoords = args.GetPosition (InnerCanvas);
                    Point pointerWorldCoords = CanvasToWorld.Transform (pointerCanvasCoords);

                    double newCenterX = pointerWorldCoords.X - 1/scale * (pointerWorldCoords.X - Viewport.Center.X);
                    double newCenterY = pointerWorldCoords.Y - 1/scale * (pointerWorldCoords.Y - Viewport.Center.Y);

                    Viewport.Scale (scale, scale);
                    Viewport.MoveTo (new Point (newCenterX, newCenterY));
                }
                else
                {
                    Viewport.Scale (scale, scale);
                }

                if (rectangularGrid != null) rectangularGrid.ClearTicValues ();
                //if (polarDecorations != null) polarDecorations.ClearTicValues ();

                Draw ();
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("InnerCanvas_MouseWheel Exception: {0}", ex.Message));
                EventLog.WriteLine (string.Format ("InnerCanvas_MouseWheel Exception: {0}", ex.StackTrace));
            }
        }

        //********************************************************************************************************************

        protected  void InnerCanvas_KeyDown (object sender, KeyEventArgs e)
        {
            /********
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                if (scrolling == false)
                {
                    zooming = true;
                    InnerCanvas.Cursor = Cursors.SizeAll;
                }
            }
            *******/
        }

        //********************************************************************************************************************

        protected void InnerCanvas_KeyUp (object sender, KeyEventArgs e)
        {
            /********
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                zooming = false;
                InnerCanvas.Cursor = Cursors.Arrow;
            }
            **********/
        }

        /*************
        protected override void ManualAxes_Click (object sender, RoutedEventArgs e)
        {
            ManualAxesDialog2D dialog = new ManualAxesDialog2D (this);
            dialog.XMin = Viewport.MinX;
            dialog.XMax = Viewport.MaxX;
            dialog.YMin = Viewport.MinY;
            dialog.YMax = Viewport.MaxY;

            bool? accept = dialog.ShowDialog ();

            if (accept == true)
                SetAxes (dialog.XMin, dialog.XMax, dialog.YMin, dialog.YMax);
        }

       
        protected override void RectangularGrid_Click (object sender, RoutedEventArgs args)
        {
            RectangularGridOn = (sender as MenuItem).IsChecked;
        }

        protected override void PolarGrid_Click (object sender, RoutedEventArgs args)
        {
            PolarGridOn = (sender as MenuItem).IsChecked;
        }

        protected override void AxisFreeze_Click (object sender, RoutedEventArgs args)
        {
            AxesFrozen = (sender as MenuItem).IsChecked;
        }
       

        protected void XAxisFormat_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rb = e.OriginalSource as RadioButton;

                if (rb != null)
                {
                    string str = rb.Tag as string;
                    string fmt= "{0:0.0}";

                    if (str == "Auto") {rectangularDecorations.XAxisFormatAuto = true;}
                    else               {rectangularDecorations.XAxisFormatAuto = false; fmt = "{0:" + str + "}";}

                    rectangularDecorations.XAxisFormat = fmt;
                    Draw ();
                }
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("X Axis Format click Exception: {0}", ex.Message));
            }
        }

        protected void YAxisFormat_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rb = e.OriginalSource as RadioButton;

                if (rb != null)
                {
                    string str = rb.Tag as string;
                    string fmt= "{0:0.0}";

                    if (str == "Auto") {rectangularDecorations.YAxisFormatAuto = true;}
                    else               {rectangularDecorations.YAxisFormatAuto = false; fmt = "{0:" + str + "}";}

                    rectangularDecorations.YAxisFormat = fmt;
                    Draw ();
                }
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("Y Axis Format click Exception: {0}", ex.Message));
            }
        }

        ************************/
    }
}
