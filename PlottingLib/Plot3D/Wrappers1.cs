﻿using System.Windows.Media.Media3D;

using Plot3D_Embedded;

namespace PlottingLib
{
    public partial class Plot3D
    {
        public bool Hold     {get {return PlotArea.Hold;} set {PlotArea.Hold = value;}}
        public void Clear () {PlotArea.Clear ();}

        public bool RectangularGridOn {get {return PlotArea.AxesBoxOn;} set {PlotArea.AxesBoxOn = value;}}
        //public bool PolarGridOn       {get {return PlotArea.PolarGridOn;}       set {PlotArea.PolarGridOn = value;}}


        public bool AxesTight  {get {return PlotArea.AxesTight;}  set {PlotArea.AxesTight = value; menuAxisTight.IsChecked = value == true;}}

        // these two are needed to satisfy interface but are not yet supported
        public bool AxesEqual  {get {return true;}  set {bool unused = value;}}
        public bool AxesFrozen {get {return false;} set {bool unused = value;}}

        public string DataAreaTitle {get {return PlotArea.DataAreaTitle;}  set {PlotArea.DataAreaTitle = value;}}
        //public string XAxisLabel    {get {return PlotArea.XAxisLabel;}     set {PlotArea.XAxisLabel = value;}}
        //public string YAxisLabel    {get {return PlotArea.YAxisLabel;}     set {PlotArea.YAxisLabel = value;}}

        public void CenterOn (Point3D pt)     {PlotArea.CenterOn = pt;}

        public double CenterDistance
        {
            get { return PlotArea.CenterDistance; }
            set { PlotArea.CenterDistance = value; }
        }

        public void CameraPosition    (Point3D pt) {PlotArea.CameraPosition = pt;}
        public void CameraRelPosition (Point3D pt) {PlotArea.CameraRelPosition = pt;}

        //****************************************************************************************************

        public ModelVisual3D Plot (ViewportObject viewObj) {return PlotArea.Plot (viewObj);}

        //****************************************************************************************************          
    }
}
