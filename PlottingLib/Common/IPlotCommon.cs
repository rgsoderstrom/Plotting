using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlottingLib
{
    //
    // methods implemented by both Plot2D and Plot3D
    //






    public interface IPlotCommon
    {
        int ID {get; set;}
        bool Hold {set; get;}
    }





    public interface IPlotDrawable
    {
        int  ID {get; set;}
        bool Hold {get; set;}
        bool AxesTight  {set; get;}
        bool AxesFrozen {set; get;}
        bool AxesEqual  {set; get;}

        void Clear ();
        void Close ();
    }
}
