using System;
using System.Windows;

namespace PlottingLib
{
    public partial class Plot2D : Window, IPlotCommon, IPlotDrawable
    {
        public int ID {get; set;} = -1;

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
