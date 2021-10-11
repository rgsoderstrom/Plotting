using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;

using Petzold.Media3D;

/// <summary>
/// AxisLine is a component of CartesianAxis and CartesianAxesBox
/// </summary>

namespace Plot3D_Embedded
{
    public class AxisLineGeometry : ViewportObjectGeometry
    {
        // 3D coordinates of this line's zero point
        public Point3D ZeroPoint = new Point3D (0, 0, 0);

        public Vector3D Direction = new Vector3D (); // unit vector

        private double min = 0, max = 1;

        public double TailCoordinate
        {
            get {return min;} 
            set {min = value; UpdateBoundingBox ();}
        }
        
        public double HeadCoordinate
        {
            get {return max;} 
            set {max = value; UpdateBoundingBox ();}
        }

        public Point3D P0 {get {return ZeroPoint + TailCoordinate * Direction;}}
        public Point3D P1 {get {return ZeroPoint + HeadCoordinate * Direction;}}

        public AxisLineGeometry (Vector3D dir)
        {
            Direction = dir;
            Direction.Normalize ();
            UpdateBoundingBox ();
        }

        public void CopyFrom (AxisLineGeometry src)
        { 
            ZeroPoint      = src.ZeroPoint;
            TailCoordinate = src.TailCoordinate;
            HeadCoordinate = src.HeadCoordinate;
        }

        private void UpdateBoundingBox ()
        {
            BoundingBox.Clear ();
            BoundingBox.Union (P0);
            BoundingBox.Union (P1);
        }
    }

    //*******************************************************************************************
    //*******************************************************************************************
    //*******************************************************************************************

    public class AxisLineView : Line3DView
    {
        public AxisLineGeometry geomCopy;

        public String Label {get; set;} = null;

        public enum TicTextDisplayOptions {NoText, Numbers, Custom};

        public TicTextDisplayOptions TicTextDisplay {get; set;} = TicTextDisplayOptions.NoText;
        public List<string> CustomTicText {get; set;} 
    

        public double TicSize  {get; set;}
        public double TicTextSize {get; set;}

        public List<double> TicsAt  {get; set;}
        public List<Vector3D> TicDirs; // usually one or two, orthogonal to axis line

        public List<string> TicText;      // optional characters displayed next to a tic mark
        public Vector3D TicTextDir;       // flow direction, default is along axis line
        public Vector3D TicTextUp;

        public Vector3D TicTextOffsetDirection;
        public double   TicTextOffsetDistance;

        public AxisLineView (AxisLineGeometry geom) : base (new Line3DGeometry (geom.P0, geom.P1))
        {
            geomCopy = geom;

            TicSize = 1;  // defaults that will almost never be acceptable
            TicTextSize = 1;
        }

        public AxisLineView (AxisLineView src, AxisLineGeometry geom) : base (new Line3DGeometry (geom.P0, geom.P1))
        {
            geomCopy   = geom;
            Color      = src.Color;
            Thickness  = src.Thickness;
            Label      = src.Label;
            TicDirs    = src.TicDirs;
            TicTextDir = src.TicTextDir;
            TicTextUp  = src.TicTextUp;

            TicTextOffsetDistance  = src.TicTextOffsetDistance;
            TicTextOffsetDirection = src.TicTextOffsetDirection;

            TicsAt  = src.TicsAt;
            TicSize = src.TicSize;
            CustomTicText = src.CustomTicText;
            TicTextDisplay   = src.TicTextDisplay;

            TicTextSize = src.TicTextSize;
        }



        public void CopyFrom (AxisLineView src)
        {
            Color = src.Color;
            Label = src.Label;
            TicsAt = src.TicsAt; 
            TicSize = src.TicSize;           
            TicTextSize = src.TicTextSize;
            TicTextDisplay = src.TicTextDisplay;
            CustomTicText = src.CustomTicText;
            TicTextOffsetDirection = src.TicTextOffsetDirection;
            TicTextOffsetDistance = src.TicTextOffsetDistance;
        }


        public void ConstructChildren ()
        {
            if (Label != null)
            {
                // 
                Point3D labelPoint = geomCopy.P0 + (geomCopy.P1 - geomCopy.P0) * 0.5;
                labelPoint -= (Label.Length * TicTextSize / 3) * TicTextDir;
                labelPoint += TicTextOffsetDirection * TicTextOffsetDistance * 4;

                Text3D txt = new Text3D (labelPoint, TicTextDir, TicTextUp, TicTextSize, Label);
                txt.TextView.Color = Color;
                Children.Add (txt.View);
            }

            if (TicsAt != null && TicsAt.Count > 0)
            {
                for (int i = 0; i<TicsAt.Count; i++)
                {
                    double tv = TicsAt [i];

                    if (tv >= geomCopy.TailCoordinate && tv <= geomCopy.HeadCoordinate)
                    {
                        // tic lines will pass through this point
                        Point3D axisPoint = geomCopy.P0 + (tv - geomCopy.TailCoordinate) * geomCopy.Direction;

                        foreach (Vector3D ticDir in TicDirs)
                        {
                            Point3D p1 = axisPoint + TicSize / 2 * ticDir;
                            Point3D p2 = axisPoint - TicSize / 2 * ticDir;

                            WireLine tic = new WireLine {Point1 = p1, Point2 = p2, Color = Color, Thickness = Thickness};
                            Children.Add (tic);
                        }

                        if (TicTextDisplay == TicTextDisplayOptions.Custom)
                        {
                            if (CustomTicText != null)
                            {
                                Vector3D textOffset = TicTextOffsetDistance * TicTextOffsetDirection - TicTextSize / 2 * TicTextDir;
                                Text3D txt = new Text3D (axisPoint + textOffset, TicTextDir, TicTextUp, TicTextSize, CustomTicText [i]);
                                txt.TextView.Color = Color;
                                Children.Add (txt.View);
                            }
                        }

                        else if (TicTextDisplay == TicTextDisplayOptions.Numbers)
                        {
                            Vector3D textOffset = TicTextOffsetDistance * TicTextOffsetDirection - TicTextSize / 2 * TicTextDir;

                            string tvs = string.Format ("{0:G2}", tv);

                            // remove the exponent field of all but the last one
                            if (i < TicsAt.Count - 1)
                            {
                                int z = tvs.IndexOf ('E');

                                if (z != -1)
                                {
                                    tvs = tvs.Remove (z);
                                }
                            }

                            Text3D txt = new Text3D (axisPoint + textOffset, TicTextDir, TicTextUp, TicTextSize, tvs);
                            txt.TextView.Color = Color;
                            Children.Add (txt.View);
                        }
                    }
                }
            }
        }
    }

    //*********************************************************************************************************
    //*********************************************************************************************************
    //*********************************************************************************************************

    public abstract class AxisLine : ViewportObject
    {
        static public readonly Vector3D XAxis = new Vector3D (1, 0, 0);
        static public readonly Vector3D YAxis = new Vector3D (0, 1, 0);
        static public readonly Vector3D ZAxis = new Vector3D (0, 0, 1);

        public AxisLineGeometry Geometry = null;

        AxisLineView axisView = null;

        public AxisLineView AxisView 
        {
            get 
            {
                if (axisView.Children.Count () == 0) 
                    axisView.ConstructChildren ();  

                return axisView; 
            }

            protected set
            {
                axisView = value;
            }
        }


        public override ModelVisual3D View {get {return AxisView as ModelVisual3D;}}

        protected override ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        public    override BoundingBox3D BoundingBox {get {return Geometry.BoundingBox;}}

        public String Label {get {return AxisView.Label;}
                             set {AxisView.Label = value; AxisView.Children.Clear ();}}

        // for an X axis line, this is the spot where the line would pierce the x = 0 plane.
        // for a Y axis line, where it pierces the y = 0 plane. same for Z axis
        public Point3D ZeroPoint {get {return Geometry.ZeroPoint;} 
                                  set {Geometry.ZeroPoint = value; AxisView = new AxisLineView (AxisView, Geometry);}}

        public double  TailCoordinate {get {return Geometry.TailCoordinate;} 
                                       set {Geometry.TailCoordinate = value; AxisView = new AxisLineView (AxisView, Geometry);}}

        public double  HeadCoordinate {get {return Geometry.HeadCoordinate;} 
                                       set {Geometry.HeadCoordinate = value; AxisView = new AxisLineView (AxisView, Geometry);}}

        public Color Color {get {return AxisView.Color;}
                            set {AxisView.Color = value; AxisView.Children.Clear ();}}

        public List<double> TicsAt {get {return AxisView.TicsAt;}
                                    set {AxisView.TicsAt = value; AxisView.Children.Clear ();}}




        public double TicSize {get {return AxisView.TicSize;}
                               set {AxisView.TicSize = value; AxisView.Children.Clear ();}}





        public double TicTextSize {get {return AxisView.TicTextSize;}
                                   set {AxisView.TicTextSize = value; AxisView.Children.Clear ();}}




        public AxisLineView.TicTextDisplayOptions TicTextDisplay {get {return AxisView.TicTextDisplay;}
                                                                  set {AxisView.TicTextDisplay = value; AxisView.Children.Clear ();}}



        public List<string> CustomTicText 
        {
            get {return AxisView.CustomTicText;}

            set 
            {
                AxisView.CustomTicText = value; 
                
                if (AxisView.CustomTicText != null)
                {
                    AxisView.TicTextDisplay = AxisLineView.TicTextDisplayOptions.Custom;
                }

                else
                {
                    if (AxisView.TicTextDisplay == AxisLineView.TicTextDisplayOptions.Custom)
                        AxisView.TicTextDisplay = AxisLineView.TicTextDisplayOptions.NoText;
                }
                
                AxisView.Children.Clear ();
            }
        }






        public Vector3D TicTextOffsetDirection  {get {return AxisView.TicTextOffsetDirection;}
                                                 set {AxisView.TicTextOffsetDirection = value; AxisView.Children.Clear ();}}

        public double TicTextOffsetDistance  {get {return AxisView.TicTextOffsetDistance;}
                                              set {AxisView.TicTextOffsetDistance = value; AxisView.Children.Clear ();}}





        public static List<double> CalculateTicLocations (int maxNumberTics, double min, double step, double max)
        {
            double anchor = max > 0 && min < 0 ? 0 : (max + min) / 2;

            List<double> tics = new List<double> () {anchor};

            double q1 = tics [0] + step;
            double q2 = tics [0] - step;

            while (true)
            {
                if (tics.Count < maxNumberTics && q1 < max) tics.Add (q1);
                if (tics.Count < maxNumberTics && q2 > min) tics.Add (q2);
                q1 += step;
                q2 -= step;

                if (tics.Count >= maxNumberTics) break;                
                if (q1 > max && q2 < min) break;
            }

            tics.Sort ();
            return tics;
        }


    }

    //*********************************************************************************************************

    public class XAxisLine : AxisLine
    {
        public XAxisLine ()
        {
            Geometry = new AxisLineGeometry (new Vector3D (1, 0, 0));

            AxisLineView tempAxisView = new AxisLineView (Geometry);
            tempAxisView.Thickness     = 1;
            tempAxisView.Color         = Colors.Red;
            tempAxisView.TicDirs       = new List<Vector3D> () {YAxis, ZAxis}; // usually 2, perpendicular to axis line
            tempAxisView.TicTextDir    = XAxis; // usually along axis line
            tempAxisView.TicTextUp     = ZAxis;
            tempAxisView.TicTextOffsetDirection = -ZAxis;
            tempAxisView.TicTextOffsetDistance  = 1;

            AxisView = tempAxisView;
        }
    
        public XAxisLine (XAxisLine src) : this ()
        {
            Geometry.CopyFrom (src.Geometry);
            AxisView.CopyFrom (src.AxisView);
        }
    }

    //*********************************************************************************************************

    public class YAxisLine : AxisLine
    {
        public YAxisLine ()
        {
            Geometry = new AxisLineGeometry (new Vector3D (0, 1, 0));

            AxisLineView tempAxisView = new AxisLineView (Geometry);

            tempAxisView.Color         = Colors.Green;
            tempAxisView.Thickness     = 1;
            tempAxisView.TicDirs       = new List<Vector3D> () {XAxis, ZAxis}; // usually 2, perpendicular to axis line
            tempAxisView.TicTextDir    = YAxis; // usually along axis line
            tempAxisView.TicTextUp     = ZAxis;
            tempAxisView.TicTextOffsetDirection = -ZAxis;
            tempAxisView.TicTextOffsetDistance  = 1;

            AxisView = tempAxisView;
        }

        public YAxisLine (YAxisLine src) : this ()
        {
            Geometry.CopyFrom (src.Geometry);
            AxisView.CopyFrom (src.AxisView);
        }
    }

    //*********************************************************************************************************

    public class ZAxisLine : AxisLine
    {
        public ZAxisLine ()
        {
            Geometry = new AxisLineGeometry (new Vector3D (0, 0, 1));

            AxisLineView tempAxisView = new AxisLineView (Geometry);

            tempAxisView.Color         = Colors.Blue;
            tempAxisView.Thickness     = 1;
            tempAxisView.TicDirs       = new List<Vector3D> () {XAxis, YAxis}; // usually 2, perpendicular to axis line
            tempAxisView.TicTextDir    = ZAxis; // usually along axis line
            tempAxisView.TicTextUp     = XAxis;
            tempAxisView.TicTextOffsetDirection = -XAxis;
            tempAxisView.TicTextOffsetDistance  = 1;

            AxisView = tempAxisView;
        }

        public ZAxisLine (ZAxisLine src) : this ()
        {
            Geometry.CopyFrom (src.Geometry);
            AxisView.CopyFrom (src.AxisView);
        }
    }
}







