using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public partial class CartesianAxesBoxView
    {
        public abstract class AxisDecorations
        {
            protected AxisDecorations (WireLine h, CartesianAxisDescription descr)
            {
                hostLine = h;
                description = descr;
            }

            CartesianAxisDescription description;

            public WireLine hostLine;

            public double startValue {get {return description.min;}}
            public double ticLength  {get {return description.ticSize;}}
            public double textSize   {get {return description.textSize;}}

            // where to draw tics
            public List<double> ticValues  {get {return description.ticsAt;}}
            public List<double> textValues {get {return ticValues;}}

            // filled-in by derived class
            public List<Vector3D> ticDirs;
            public Vector3D textDir;
            public Vector3D textUp;
            public Vector3D textOffset;
        }

        public class XAxisDecorations : AxisDecorations
        {
            public XAxisDecorations (WireLine host, CartesianAxisDescription descr) : base (host, descr)
            {
                ticDirs = new List<Vector3D> () {new Vector3D (0, 0, 1), new Vector3D (0, 1, 0)};
                textDir = new Vector3D (1, 0, 0);
                textUp = new Vector3D (0, 0, 1);
                textOffset = new Vector3D (-3, 0, -1) / Math.Sqrt (10) * descr.textSize;
            }
        }

        public class YAxisDecorations : AxisDecorations
        {
            public YAxisDecorations (WireLine host, CartesianAxisDescription descr) : base (host, descr)
            {
                ticDirs = new List<Vector3D> () {new Vector3D (0, 0, 1), new Vector3D (1, 0, 0)};
                textDir = new Vector3D (0, 1, 0);
                textUp = new Vector3D (0, 0, 1);
                textOffset = new Vector3D (0, -3, -1) / Math.Sqrt (10) * descr.textSize;
            }
        }

        public class ZAxisDecorations : AxisDecorations
        {
            public ZAxisDecorations (WireLine host, CartesianAxisDescription descr) : base (host, descr)
            {
                ticDirs = new List<Vector3D> () {new Vector3D (1, 0, 0), new Vector3D (0, 1, 0)};
                textDir = new Vector3D (0, 0, 1);
                textUp = new Vector3D (1, 1, 0);
                textOffset = new Vector3D (-1, -1, -3) / Math.Sqrt (11) * descr.textSize;
            }
        }
    }
}
