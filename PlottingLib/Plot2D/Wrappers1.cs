using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using Plot2D_Embedded;

namespace PlottingLib   
{
    public partial class Plot2D
    {
        public bool Hold     {get {return PlotArea.Hold;} set {PlotArea.Hold = value;}}
        public void Clear () {PlotArea.Clear ();}
        public void SetAxes (double x1, double x2, double y1, double y2) {PlotArea.SetAxes (x1, x2, y1, y2);}
        public void GetAxes (ref double x1, ref double x2, ref double y1, ref double y2) {PlotArea.GetAxes (out x1, out x2, out y1, out y2);}

        public bool RectangularGridOn {get {return PlotArea.RectangularGridOn;} set {PlotArea.RectangularGridOn = value;}}
        public bool PolarGridOn       {get {return PlotArea.PolarGridOn;}       set {PlotArea.PolarGridOn = value;}}

        public bool AxesTight  {get {return PlotArea.AxesTight;}  set {PlotArea.AxesTight = value;}}
        public bool AxesEqual  {get {return PlotArea.AxesEqual;}  set {PlotArea.AxesEqual = value;}}
        public bool AxesFrozen {get {return PlotArea.AxesFrozen;} set {PlotArea.AxesFrozen = value;}}

        void RectangularGrid_Click (object sender, RoutedEventArgs args) {PlotArea.RectangularGridOn = (sender as MenuItem).IsChecked;}
        void PolarGrid_Click       (object sender, RoutedEventArgs args) {PlotArea.PolarGridOn = (sender as MenuItem).IsChecked;}

        public string DataAreaTitle {get {return PlotArea.DataAreaTitle;}  set {PlotArea.DataAreaTitle = value;}}
        public string XAxisLabel    {get {return PlotArea.XAxisLabel;}     set {PlotArea.XAxisLabel = value;}}
        public string YAxisLabel    {get {return PlotArea.YAxisLabel;}     set {PlotArea.YAxisLabel = value;}}

        void AxisFreeze_Click (object sender, RoutedEventArgs args) {AxesFrozen = (sender as MenuItem).IsChecked;}
        void AxisEqual_Click  (object sender, RoutedEventArgs args) {AxesEqual  = (sender as MenuItem).IsChecked;}
        void AxisTight_Click  (object sender, RoutedEventArgs args) {AxesTight  = (sender as MenuItem).IsChecked;}
        void ManualAxes_Click (object sender, RoutedEventArgs args) { }

        private void CheckButtonStates ()
        {
            menuRectangularGrid.IsChecked = PlotArea.RectangularGridOn;
            menuPolarGrid.IsChecked = PlotArea.PolarGridOn;

            //GridLinesFixed_Button.IsChecked = figure.FixedGridLines;
            //GridValuesFixed_Button.IsChecked = figure.FixedGridValues;

            menuAxisEqual.IsChecked = PlotArea.AxesEqual;
            menuAxisTight.IsChecked = PlotArea.AxesTight;
            menuAxisFreeze.IsChecked = PlotArea.AxesFrozen;
            //PlotHold_Button.IsChecked = figure.Hold;
        }

        //****************************************************************************************************
          
        public CanvasObject    Plot (CanvasObject co)    {PlotArea.Plot (co); return co;}
        public ContourPlotView Plot (ContourPlotView pv) {PlotArea.Plot (pv); return pv;}
        public VectorView      Plot (VectorView vv)      {PlotArea.Plot (vv); return vv;}
        public VectorFieldView Plot (VectorFieldView vf) {PlotArea.Plot (vf); return vf;}

        //*************************************************************************************************
    }
}










