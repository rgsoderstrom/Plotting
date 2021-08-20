using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // cursors
using System.Windows.Media;

namespace Plot2D_Embedded
{
    //
    // Signature of application control flag change handlers looks like this
    //
    public delegate void ControlFlagChange_Handler ();

    public partial class Bare2DPlot
    {
        public event ControlFlagChange_Handler ControlFlagChangeHandlers; // list of all the callbacks

        protected bool tight = false;
        public bool AxesTight {get {return tight;}
                               set {bool change = (tight != value); if (change) {tight = value; FlagChangeRedraw ();}}}

        protected bool equal = false;
        public bool AxesEqual {get {return equal;}
                               set {bool change = (equal != value); if (change) {equal = value; FlagChangeRedraw ();}}}

        protected bool frozen = false;
        public bool AxesFrozen {get {return frozen;}
                                set {bool change = (frozen != value);    
                                     frozen = value; 
                                     if (change && frozen) 
                                         AxesTight = false;
                                     if (change)
                                         FlagChangeRedraw ();}}

        protected bool hold = true;
        public bool Hold
        {get {return hold;}
         set {hold = value; FlagChangeEvent ();}}


        bool mouseEnabled = true;
        public bool MouseEnabled
        {
            get {return mouseEnabled;}
            //set {mouseEnabled = value; if (mouseEnabled) EnableMouse (); else DisableMouse (); FlagChangeEvent ();}
            set {bool change = (mouseEnabled != value); if (change) {mouseEnabled = value; if (mouseEnabled) EnableMouse (); else DisableMouse (); FlagChangeEvent ();}}
        }

        protected bool fixedGridLines = true;
        public bool FixedGridLines  {get {return fixedGridLines;} set {fixedGridLines = value;}}
        public bool FixedGridValues {get {return !FixedGridLines;} set {FixedGridLines = !value;}}

        private void FlagChangeEvent ()
        {
            ControlFlagChangeHandlers?.Invoke ();
        }

        private void FlagChangeRedraw ()
        {
            FlagChangeEvent ();
            Draw ();
        }

        protected string dataAreaTitle = "";
        public string DataAreaTitle
        {get {return dataAreaTitle;}
         set {dataAreaTitle = value; Draw ();}}

        protected string xAxisLabel = "";
        public string XAxisLabel
        {get {return xAxisLabel;}
         set {xAxisLabel = value; Draw ();}}

        protected string yAxisLabel = "";
        public string YAxisLabel
        {get {return yAxisLabel;}
         set {yAxisLabel = value; Draw ();}}

        //
        // Called by Plot2D to make plots look more like Matlab
        //
        public void MatlabStyle ()
        {
            AnootationBackground = new SolidColorBrush (Color.FromRgb (0xa8, 0xa8, 0xa8));
            OuterBorder.BorderThickness = new Thickness (0);
            OuterBorder.Margin = new Thickness (0);
            InnerBorder.BorderBrush = Brushes.Black;
            InnerBorder.BorderThickness = new Thickness (1);
        }

        public void Clear ()
        {
            InnerCanvas.Children.Clear ();

            plottedObjects.Clear ();

            rectangularGrid.ClearTicValues ();
            RectangularGridOn = false;

            polarGrid.ClearTicValues ();
            PolarGridOn = false;

            DataBoundingBox.Clear ();
            Viewport = new Viewport2D (DataBoundingBox);

            /**
            DataAreaTitle = null;
            xAxisLabel = null;
            yAxisLabel = null;
            **/
            Hold       = true;
            AxesEqual  = false;
            AxesTight  = true;
            AxesFrozen = false;
        }

        public void ClearData ()
        {
            InnerCanvas.Children.Clear ();
            plottedObjects.Clear ();
        }

        public void GetAxes (out double XMin, out double XMax, out double YMin, out double YMax)
        {
            XMin = Viewport.MinX;
            XMax = Viewport.MaxX;
            YMin = Viewport.MinY;
            YMax = Viewport.MaxY;
        }
        
        public void SetAxes (double XMin, double XMax, double YMin, double YMax)
        {
            if (XMin >= XMax)
                throw new Exception ("Plot2D SetAxes: XMin must be less than XMax");

            if (YMin >= YMax)
                throw new Exception ("Plot2D SetAxes: YMin must be less than YMax");

            double ViewportWidth = XMax - XMin;
            double ViewportHeight = YMax - YMin;
           
            mx =  DrawingWidthWPF  / ViewportWidth;
            my = -DrawingHeightWPF / ViewportHeight;
         //   AxesFrozen = true; // do this immediately after setting slopes

            Point center = new Point ((XMax + XMin) / 2, (YMax + YMin) / 2);
            Viewport = new Viewport2D (center, ViewportWidth, ViewportHeight);

            AxesEqual  = false;
            AxesTight  = false;
            rectangularGrid.ClearTicValues ();
            polarGrid.ClearTicValues ();
            Draw ();
        }       

        //*************************************************************************************************
        
        public void Refresh ()
        {
            InnerCanvas.Children.Clear ();

            rectangularGrid.ClearTicValues ();
            polarGrid.ClearTicValues ();

            DataBoundingBox.Clear ();

            foreach (CanvasObject cl in plottedObjects)
                DataBoundingBox.Union (cl.BoundingBox);

            Draw ();
        }

        //*************************************************************************************************
        //*************************************************************************************************
        //*************************************************************************************************

        public void Plot (CanvasObject canvasObject)
        {
            if (canvasObject.HostPlot != null && canvasObject.HostPlot != this)
                throw new Exception ("Plot2D.Plot (): can only plot an object on one figure");

            PlotCommonStart ();

            canvasObject.HostPlot = this;
            canvasObject.AspectRatioCorrection.Children.Add (canvasObject.HostPlot.aspectRatio);

            if (canvasObject.path != null) // path will be null for Text2D
            {
                if (canvasObject.path.Data.Transform is TransformGroup)
                {
                    if ((canvasObject.path.Data.Transform as TransformGroup).Children.Count == 3)
                        (canvasObject.path.Data.Transform as TransformGroup).Children.Add (canvasObject.HostPlot.WorldToCanvas);
                }
                else
                {
                    canvasObject.path.Data.Transform = canvasObject.HostPlot.WorldToCanvas;
                }
            }

            plottedObjects.Add (canvasObject);
            DataBoundingBox.Union (canvasObject.BoundingBox);
            Draw ();
        }

        //*************************************************************************************************
        //
        // Composite objects, made from several CanvasObjects
        //      - VectorView
        //

        public void Plot (List<CanvasObject> lco)
        {
            for (int i=1; i<lco.Count; i++)
                Plot (lco [i]);

            Plot (lco [0]); // plot this last so it displays on top
        }
    }
}