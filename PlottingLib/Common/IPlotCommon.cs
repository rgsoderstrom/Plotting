using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlottingLib
{
    /// <summary>
    /// IPlotCommon - properties implmented by PlotFigure, Plot2D and Plot3D
    /// </summary>
    public interface IPlotCommon
    {
        int ID {get; set;}
        bool Hold {set; get;}
        double Left   {set; get;}
        double Top    {set; get;}
        double Width  {set; get;}
        double Height {set; get;}

        string DataAreaTitle { set; get; }
    }

    /// <summary>
    /// IPlotDrawable - properties and methods that must be imlplemented by Plot2D and Plot3D
    /// </summary>
    public interface IPlotDrawable
    {
        bool AxesTight  {set; get;}
        bool AxesFrozen {set; get;}
        bool AxesEqual  {set; get;}

        void Clear ();
        void Close ();
    }
}
