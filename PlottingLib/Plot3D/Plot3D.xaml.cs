using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Plot3D_Embedded;

namespace PlottingLib
{
    public partial class Plot3D : Window, IPlotCommon, IPlotDrawable
    {
        public int ID {get; set;} = -1;

        public Plot3D ()
        {
            Common.WindowLoaded (this);
            ID = Common.instanceCounter;

            InitializeComponent();
            Show ();
        }

        public Plot3D (PlotFigure src)
        {
            InitializeComponent();

            Left = src.Left;
            Top = src.Top;
            Width = src.Width;
            Height = src.Height;
            Hold = src.Hold;
            Title = src.Title;
            ID = src.ID;

            Show ();
        }

        protected virtual void Window_Loaded (object sender, RoutedEventArgs e)
        {
            PlotArea.MatlabStyle ();
            Title = string.Format ("Figure {0}", ID);
        }

        private void Window_Closed (object sender, EventArgs e)
        {
        }

        public override string ToString ()
        {
            return "Plot3D " + ID.ToString ();
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
