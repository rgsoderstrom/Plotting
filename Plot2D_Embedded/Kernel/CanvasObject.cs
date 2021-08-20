using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

using Common;

namespace Plot2D_Embedded
{
    abstract public class CanvasObject
    {
        public Brush  BorderColor     {get {return path.Stroke;} set {path.Stroke = value;}}
        public Brush  FillColor       {get {return path.Fill;} set {path.Fill = value;}}
        public double BorderThickness {get {return path.StrokeThickness;} set {path.StrokeThickness = value;}}

        static public double DefaultLineThickness = 2;

        string name;

        public string Name {get {return name;}
                            set {name = value; if (path != null) path.ToolTip = value;}}
       

        public Path path = new Path ();
        public virtual UIElement View {get {return path;}}





        protected ScaleTransform scale = new ScaleTransform ();
        protected RotateTransform rotate = new RotateTransform ();
        protected TranslateTransform xlate = new TranslateTransform ();
        TransformGroup templateToWorld = new TransformGroup ();

        public TransformGroup AspectRatioCorrection = new TransformGroup ();

        //********************************************************************************************

        public BoundingBox BoundingBox {set; get;} = new BoundingBox ();

        protected CanvasObject ()
        {
            templateToWorld.Children.Add (scale);
            templateToWorld.Children.Add (rotate);
            templateToWorld.Children.Add (xlate);
        }


        protected void CalculateBB (List<Point> corners)
        {
            BoundingBox.Clear ();

            foreach (Point pt in corners)
                BoundingBox.Union (templateToWorld.Transform (pt));
        }

        //********************************************************************************************






        public Bare2DPlot HostPlot {get; internal set;} // plot this object is drawn on

        internal Canvas         HostCanvas    {get {return HostPlot.InnerCanvas;}} // canvas this object drawn on
        internal TransformGroup WorldToCanvas {get {return HostPlot.WorldToCanvas;}}






        public virtual void DrawBold (object sender, MouseEventArgs e)
        {
            if (path != null)
                path.StrokeThickness += 2; 

            if (sender is TextBlock)
                TextBlock.SetFontWeight (sender as TextBlock, FontWeights.Bold);

            //if (sender is TextBlock)
            //    TextBlock.SetFontStyle (sender as TextBlock, FontStyles.Italic);
        }

        public virtual void DrawNormal (object sender, MouseEventArgs e)
        {
            if (path != null)
                path.StrokeThickness -= 2; 

            if (sender is TextBlock)
                TextBlock.SetFontWeight (sender as TextBlock, FontWeights.Normal);

            //if (sender is TextBlock)
            //    TextBlock.SetFontStyle (sender as TextBlock, FontStyles.Normal);
        }


       
        //
        // Mouse left-click support
        //

        private event Application_LeftClick_Handler Application_MouseButtonHandlers; // list of all the callbacks

        private void Internal_MouseLeftClickHandler (object sender, MouseButtonEventArgs args)
        {
            args.Handled = true;

            if (WorldToCanvas != null)
            {
                Point appCoordsClicked = WorldToCanvas.Inverse.Transform (args.GetPosition (HostCanvas));
                Application_MouseButtonHandlers?.Invoke (this, appCoordsClicked); // "this" Line2D, Point2D, etc. becomes the message sender
            }
            else
            {
                Application_MouseButtonHandlers?.Invoke (this, new Point ());
            }
        }
        
        public void RegisterForMouseLeftClick (Application_LeftClick_Handler userHandler)
        {
            View.MouseLeftButtonDown += Internal_MouseLeftClickHandler;
            View.MouseEnter += DrawBold;
            View.MouseLeave += DrawNormal;
            Application_MouseButtonHandlers += userHandler;
        }

        public void UnregisterForMouseLeftClick (Application_LeftClick_Handler userHandler)
        {
            View.MouseLeftButtonDown -= Internal_MouseLeftClickHandler;
            View.MouseEnter -= DrawBold;
            View.MouseLeave -= DrawNormal;
            Application_MouseButtonHandlers -= userHandler;
        }
    }

    //
    // Signature of application mouse button handlers looks like this
    //
    public delegate void Application_LeftClick_Handler (object sender, Point pt);
}
