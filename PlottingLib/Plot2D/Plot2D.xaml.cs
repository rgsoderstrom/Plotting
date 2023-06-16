using System;
using System.Windows;

namespace PlottingLib
{
    public partial class Plot2D : Window, IPlotCommon, IPlotDrawable
    {
        public int ID {get; set;} = -1;

        public new double Left   {get { return base.Left; }   set { base.Left = value; } }
        public new double Top    {get { return base.Top; }    set { base.Top = value; } }
        public new double Width  {get { return base.Width; }  set { base.Width = value; } }
        public new double Height {get { return base.Height; } set { base.Height = value; } }

        public Plot2D ()
        {
            Common.WindowLoaded (this);
            ID = Common.instanceCounter;

            InitializeComponent();
            Show ();
        }

        public Plot2D (PlotFigure src)
        {
            InitializeComponent();

            Left = src.Left;
            Top = src.Top;
            Width = src.Width;
            Height = src.Height;
            Hold = src.Hold;
            ID = src.ID;

            DataAreaTitle = src.DataAreaTitle;
            XAxisLabel = src.XAxisLabel;
            YAxisLabel = src.YAxisLabel;
            Show ();
        }

        protected virtual void Window_Loaded (object sender, RoutedEventArgs e)
        {
            PlotArea.MatlabStyle ();
            Title = string.Format ("Figure {0} - Plot2D", ID);
        }

        private void Window_Closed (object sender, EventArgs e)
        {
        }

        public override string ToString ()
        {
            return "Plot2D " + ID.ToString ();
        }

        //***************************************************************************************

        public void xDataAreaTitle (string txt)
        {

        }

        public void xAxesTight ()
        {

        }
    }
}
