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
                startValue = descr.min;

                ticValues = descr.ticsAt;
                ticLength = 0.2;
                textValues = descr.ticsAt;
            }

            public WireLine hostLine;
            public double startValue;

            // where to draw tics
            public List<double> ticValues;
            public List<Vector3D> ticDirs;
            public double ticLength;

            // where to write text
            public List<double> textValues;
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
                textOffset = (new Vector3D (-3, -1, -1) / Math.Sqrt (11)) * 0.2;
            }
        }

        public class YAxisDecorations : AxisDecorations
        {
            public YAxisDecorations (WireLine host, CartesianAxisDescription descr) : base (host, descr)
            {
                ticDirs = new List<Vector3D> () {new Vector3D (0, 0, 1), new Vector3D (1, 0, 0)};
                textDir = new Vector3D (0, 1, 0);
                textUp = new Vector3D (0, 0, 1);
                textOffset = (new Vector3D (-1, -3, -1) / Math.Sqrt (11)) * 0.2;
            }
        }

        public class ZAxisDecorations : AxisDecorations
        {
            public ZAxisDecorations (WireLine host, CartesianAxisDescription descr) : base (host, descr)
            {
                ticDirs = new List<Vector3D> () {new Vector3D (1, 0, 0), new Vector3D (0, 1, 0)};
                textDir = new Vector3D (0, 0, 1);
                textUp = new Vector3D (1, 1, 0);
                textOffset = (new Vector3D (-1, -1, -3) / Math.Sqrt (11)) * 0.2;
            }
        }


        /***********

        //********************************************************************************

        AxisDecorations XAxisDecorations (WireLine line, CartesianAxisDescription descr)
        {
            AxisDecorations Markings = new AxisDecorations ()
            {
                hostLine = line,
                startValue = descr.min,

            };

            return Markings;
        }

        //********************************************************************************

        AxisDecorations YAxisDecorations (WireLine line, CartesianAxisDescription descr)
        {
            AxisDecorations Markings = new AxisDecorations ()
            {
                hostLine = line,
                startValue = descr.min,

                ticValues = descr.ticsAt,
                ticLength = 0.2,
                ticDir = new Vector3D (1, 0, 0),

                textValues = descr.ticsAt,
                textDir = new Vector3D (0, 1, 0),
                textUp = new Vector3D (0, 0, 1),
                textOffset = (new Vector3D (-1, -3, -1) / Math.Sqrt (11)) * 0.2
            };

            return Markings;
        }

        //********************************************************************************

        AxisDecorations ZAxisDecorations (WireLine line, CartesianAxisDescription descr)
        {
            AxisDecorations Markings = new AxisDecorations ()
            {
                hostLine = line,
                startValue = descr.min,

                ticValues = descr.ticsAt,
                ticLength = 0.2,
                ticDir = new Vector3D (1, 0, 0),

                textValues = descr.ticsAt,
                textDir = new Vector3D (0, 0, 1),
                textUp = new Vector3D (1, 1, 0),
                textOffset = (new Vector3D (-1, -3, -1) / Math.Sqrt (11)) * 0.2
            };

            return Markings;
        }************/
    }
}



