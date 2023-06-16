using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Common;

namespace PlottingLib
{
    public partial class PlotFigure : Window, IPlotCommon
    {
        // these are read by Plot2D and 3D ctors
        public int ID    {get; set;}
        public bool Hold {get; set;}

        public new double Left   {get { return base.Left;}   set {base.Left = value;}}
        public new double Top    {get { return base.Top;}    set {base.Top = value;}}
        public new double Width  {get { return base.Width;}  set {base.Width = value;}}
        public new double Height {get { return base.Height;} set {base.Height = value;}}

        protected string dataAreaTitle = "";
        public string DataAreaTitle
        {
            get {return dataAreaTitle;}
            set {dataAreaTitle = value; DrawOuterCanvasText ();}
        }

        protected string xAxisLabel = "";
        public string XAxisLabel
        {get {return xAxisLabel;}
         set {xAxisLabel = value; DrawOuterCanvasText ();}}

        protected string yAxisLabel = "";
        public string YAxisLabel
        {get {return yAxisLabel;}
         set {yAxisLabel = value; DrawOuterCanvasText ();}}

        // margins of drawingSurface - copied from Plot2D_Embedded
        static int DataAreaLeft = 80;
        static int DataAreaRight = 50;
        static int DataAreaTop = 50;
        static int DataAreaBottom = 60;

        // where labels and numbers are drawn on outer canvas - copied from Plot2D_Embedded
        protected double DataAreaX0 {get {return DataAreaLeft;}}
        protected double DataAreaX1 {get {return OuterCanvas.ActualWidth - DataAreaRight;}}
        protected double DataAreaY0 {get {return DataAreaTop;}}
        protected double DataAreaY1 {get {return OuterCanvas.ActualHeight - DataAreaBottom;}}

        public PlotFigure ()
        {
            Common.WindowLoaded (this);
            ID = Common.instanceCounter;

            InitializeComponent();
            Show ();
        }

        private void Window_Loaded (object sender, EventArgs e)
        {
            Canvas.SetLeft (InnerBorder, DataAreaLeft);
            Canvas.SetTop (InnerBorder, DataAreaTop);

            InnerCanvas.Background = Brushes.White;

            OuterCanvas.Background = new SolidColorBrush (Color.FromRgb (0xa8, 0xa8, 0xa8));
            OuterBorder.BorderThickness = new Thickness (0);
            OuterBorder.Margin = new Thickness (0);
            InnerBorder.BorderBrush = Brushes.Black;
            InnerBorder.BorderThickness = new Thickness (1);
        }

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

        private void OuterCanvas_SizeChanged (object sender, SizeChangedEventArgs args)
        {
            try
            {
                double outerCanvasWidth = args.NewSize.Width;
                double outerCanvasHeight = args.NewSize.Height;

                InnerBorder.Width  = outerCanvasWidth  - (DataAreaLeft + DataAreaRight);
                InnerBorder.Height = outerCanvasHeight - (DataAreaTop + DataAreaBottom);

                OuterCanvas.Children.Clear ();
                OuterCanvas.Children.Add (InnerBorder);
                DrawOuterCanvasText ();
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (string.Format ("OuterCanvas_SizeChanged Exception: {0}", ex.Message));
                EventLog.WriteLine (string.Format ("OuterCanvas_SizeChanged Exception: {0}", ex.StackTrace));
            }
        }
    }
}
