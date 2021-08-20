using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plot3D_Embedded
{
    public partial class Bare3DPlot
    {
        // margins of drawingSurface
        static int DataAreaLeft = 80;
        static int DataAreaRight = 50;
        static int DataAreaTop = 50;
        static int DataAreaBottom = 60;

        // where labels and numbers are drawn on outer canvas
        protected double DataAreaX0 {get {return DataAreaLeft;}}
        protected double DataAreaX1 {get {return OuterCanvas.ActualWidth - DataAreaRight;}}
        protected double DataAreaY0 {get {return DataAreaTop;}}
        protected double DataAreaY1 {get {return OuterCanvas.ActualHeight - DataAreaBottom;}}

        protected int TraceXMargin = 1; // bounds on how close trace can come to edge of data plotting area
        protected int TraceYMargin = 5;

    }
}
