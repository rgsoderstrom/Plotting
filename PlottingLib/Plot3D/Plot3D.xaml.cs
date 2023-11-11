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
        public int ID {get; set;}

        public new double Left   {get { return base.Left; }   set { base.Left = value; } }
        public new double Top    {get { return base.Top; }    set { base.Top = value; } }
        public new double Width  {get { return base.Width; }  set { base.Width = value; } }
        public new double Height {get { return base.Height; } set { base.Height = value; } }

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
            ID = src.ID;

            DataAreaTitle = src.DataAreaTitle;
            Show ();
        }

        protected virtual void Window_Loaded (object sender, RoutedEventArgs e)
        {
            PlotArea.MatlabStyle ();
            Title = string.Format ("Figure {0} - Plot3D", ID);
        }

        private void Window_Closed (object sender, EventArgs e)
        {
        }

        public override string ToString ()
        {
            return "Plot3D " + ID.ToString ();
        }

        //***************************************************************************************

        private void cartesianBox_Click (object sender, RoutedEventArgs args)
        {
            MenuItem b1 = args.OriginalSource as MenuItem;
            //PlotArea.AxesFrozen = true;
            PlotArea.AxesBoxOn = b1.IsChecked;
        }

        private void menuAxisTight_Click (object sender, RoutedEventArgs args)
        {
            MenuItem b1 = args.OriginalSource as MenuItem;
            PlotArea.AxesTight = b1.IsChecked;
        }

        private void menuAxisFreeze_Click (object sender, RoutedEventArgs args)
        {
            //MenuItem b1 = args.OriginalSource as MenuItem;
            //PlotArea.AxesFrozen = b1.IsChecked;
        }

        private void cameraCenterMenu_Click (object sender, RoutedEventArgs e)
        {
            CameraCenterDlg dlg = new CameraCenterDlg ();
            dlg.Owner = this;

            Point3D WasCenter = PlotArea.CenterOn;
            dlg.CenterXCoord.Text = WasCenter.X.ToString ();
            dlg.CenterYCoord.Text = WasCenter.Y.ToString ();
            dlg.CenterZCoord.Text = WasCenter.Z.ToString ();

            bool? accepted = dlg.ShowDialog ();

            if (accepted == true)
            {
                PlotArea.CenterOn = new Point3D (dlg.X, dlg.Y, dlg.Z);
            }
        }

        private void cameraAbsPos_Click (object sender, RoutedEventArgs e)
        {

        }

        private void cameraRelCart_Click (object sender, RoutedEventArgs e)
        {

        }

        private void cameraRelSph_Click (object sender, RoutedEventArgs e)
        {

        }
    }
}
