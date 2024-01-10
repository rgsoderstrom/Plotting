using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // cursors
using System.Windows.Media;
using System.Windows.Shapes;

using Common;

//
// Non-public methods and data storage for Plot2D
//

namespace Plot2D_Embedded
{
    public partial class Bare2DPlot
    {
        // Conventional Cartesian, X increases to right, Y increases up
        protected BoundingBox DataBoundingBox = new BoundingBox (); // bounds on current data. 
        protected Viewport2D    Viewport = new Viewport2D (); // area of application coordinates visible on plot

        // WPF Coordinates
        Point  DrawingCenterWPF {get {return new Point (InnerCanvas.ActualWidth / 2, InnerCanvas.ActualHeight / 2);}}
        double DrawingWidthWPF  {get {return InnerCanvas.ActualWidth  - 2 * TraceXMargin;}}
        double DrawingHeightWPF {get {return InnerCanvas.ActualHeight - 2 * TraceYMargin;}}

        // from application data points to coordinates on inner canvas
        protected double mx = 1, my = -1; // slopes for world-to-canvas mapping
        internal TransformGroup   WorldToCanvas = new TransformGroup ();
        protected GeneralTransform CanvasToWorld;

        internal double WorldXToCanvasX (double Xw)
        {
            return  mx * (Xw - Viewport.Center.X) + DrawingCenterWPF.X;
        }

        internal double WorldYToCanvasY (double Yw)
        {
            if (double.IsInfinity (my))
                my = 999;

            return my * (Yw - Viewport.Center.Y) + DrawingCenterWPF.Y;
        }

        //************************************************************************************************

        internal List<CanvasObject> plottedObjects = new List<CanvasObject> ();

        protected void Draw ()
        {
            try
            {
                if (AxesFrozen == false)
                    CalculateWorldToCanvasTransform ();

                OuterCanvas.Children.Clear ();
                OuterCanvas.Children.Add (InnerBorder);

                InnerCanvas.Children.Clear ();
             
                DrawOuterCanvasText ();
                DrawInnerCanvasObjects ();

                if (rectangularGrid != null)
                {
                    if (RectangularGridOn)
                    {
                      //rectangularGrid.ClearTicValues ();
                        rectangularGrid.CalculateTicValues (Viewport);
                        rectangularGrid.DrawGridLines ();
                        rectangularGrid.DrawTicMarks ();
                        rectangularGrid.DrawAxisNumericLabels (DataAreaX0, DataAreaX1, DataAreaY0, DataAreaY1);
                    }
                }

                if (PolarGridOn)
                {
                    //polarGrid.ClearTicValues ();
                    polarGrid.CalculateTicValues (Viewport);

                    polarGrid.DrawGridLines ();
                }

            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("Draw Exception: {0}", ex.Message));
                EventLog.WriteLine (string.Format ("Draw Exception: {0}", ex.StackTrace));
            }
        }

        void DrawInnerCanvasObjects ()
        {
            foreach (CanvasObject obj in plottedObjects)
            {

                if (obj is TextView)
                {
                    Point canvasTLC = WorldToCanvas.Transform ((obj as TextView).Position);

                    (obj as TextView).SetFontSizeWPF (my);

                    Canvas.SetLeft (obj.View, canvasTLC.X);   
                    Canvas.SetTop (obj.View, canvasTLC.Y);
                }

                InnerCanvas.Children.Add (obj.View);
            }
        }
                
        //****************************************************************************************************************************
        //****************************************************************************************************************************
        //****************************************************************************************************************************
        
        void DrawOuterCanvasText ()
        {
            // title
            if (DataAreaTitle != null)
            {
                if (DataAreaTitle.Length > 0)
                {
                    TextBlock tb1 = new TextBlock
                    {
                        FontSize = 24,// 18,
                        Text = DataAreaTitle
                    };

                    Canvas.SetTop (tb1, DataAreaY0 * 0.4);
                    Canvas.SetLeft (tb1, (DataAreaX1 + DataAreaX0 - 8 * DataAreaTitle.Length) / 2);
                    OuterCanvas.Children.Add (tb1);
                }
            }

            // X Axis Label
            if (XAxisLabel != null)
            {
                if (XAxisLabel.Length > 0)
                {
                    TextBlock tb1 = new TextBlock ();
                    tb1.FontSize = 24;// 18;
                    tb1.Text = XAxisLabel;
                    Canvas.SetTop (tb1, DataAreaY1 + 24);
                    Canvas.SetLeft (tb1, (DataAreaX1 + DataAreaX0 - 8 * XAxisLabel.Length) / 2);
                    OuterCanvas.Children.Add (tb1);
                }
            }

            // Y Axis Label
            if (YAxisLabel != null)
            {
                if (YAxisLabel.Length > 0)
                {
                    TextBlock tb1 = new TextBlock ();
                    tb1.FontSize = 24;// 18;
                    tb1.Text = YAxisLabel;
                    tb1.RenderTransform = new RotateTransform (90);
                    Canvas.SetTop (tb1, (DataAreaY1 + DataAreaY0 - 8 * YAxisLabel.Length) / 2);
                    Canvas.SetLeft (tb1, DataAreaX0 - 32); // 48);
                    OuterCanvas.Children.Add (tb1);
                }
            }
        }
        
        protected void CalculateTransformSlopes ()
        {
            // calculate slopes, constrained by Equal, Frozen, Tight
            if (AxesEqual == false)
            {
                mx =   DrawingWidthWPF  / Viewport.Width; // (WPF Units) / (Application Units)
                my = -(DrawingHeightWPF / Viewport.Height);
            }
            else
            {
                double fullMx =   DrawingWidthWPF  / Viewport.Width;   // slopes to show all data
                double fullMy = -(DrawingHeightWPF / Viewport.Height);

                double drawingYExtent = DrawingHeightWPF /  fullMx; // drawing Y extent if both use X slope
                //double drawingXExtent = DrawingWidthWPF  / -fullMy; //    "    X    "   "    "   "  Y   "

                if (drawingYExtent >= Viewport.Height)
                {
                    mx = fullMx;
                    my = -mx;

                    Viewport.Scale (1, Math.Abs (my/fullMy));
                }
                else
                {
                    my = fullMy;
                    mx = -my;

                    Viewport.Scale (Math.Abs (mx/fullMx), 1);
                }
            }
        }

        protected void CalculateWorldToCanvasTransform () 
        {
            if (AxesTight == true)
                Viewport = new Viewport2D (DataBoundingBox);

            CalculateTransformSlopes ();

            // adjust the aspect ratio of displayed circles, squares and user-defined shapes
            aspectRatio.ScaleY = Math.Abs (mx/my);

            WorldToCanvas.Children.Clear ();
            ScaleTransform scale = new ScaleTransform (mx, my);
            TranslateTransform xlate = new TranslateTransform (DrawingCenterWPF.X - mx * Viewport.Center.X, DrawingCenterWPF.Y - my * Viewport.Center.Y);
            WorldToCanvas.Children.Add (scale);
            WorldToCanvas.Children.Add (xlate);

            CanvasToWorld = WorldToCanvas.Inverse;
        }

        void PlotCommonStart ()
        {
            if (Hold == false)
            {
                InnerCanvas.Children.Clear ();
                plottedObjects.Clear ();
            }

            if (AxesFrozen == false)
            { 
                rectangularGrid.ClearTicValues ();
                polarGrid.ClearTicValues ();
            }

            if (plottedObjects.Count == 0)
            {
                DataBoundingBox.Clear ();

                if (AxesFrozen == false)
                  AxesTight = true;
            }
        }
    }
}
