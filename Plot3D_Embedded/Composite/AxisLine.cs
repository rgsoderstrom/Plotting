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
        
        public double HeadCoodinate
        {
            get {return max;} 
            set {max = value; UpdateBoundingBox ();}
        }

        public Point3D P0 {get {return ZeroPoint + TailCoordinate * Direction;}}
        public Point3D P1 {get {return ZeroPoint + HeadCoodinate  * Direction;}}

        public AxisLineGeometry (Vector3D dir)
        {
            Direction = dir;
            Direction.Normalize ();
            UpdateBoundingBox ();
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
        AxisLineGeometry geomCopy;

        public enum TextDisplayOptions {NoText, Numbers, Custom};

        public TextDisplayOptions TextDisplay {get; set;} = TextDisplayOptions.NoText;
        public List<string> CustomText {get; set;} 
    

        public double TicSize  {get; set;}
        public double TextSize {get; set;}

        public List<double> TicsAt  {get; set;}

        public List<Vector3D> TicDirs; // usually one or two, orthogonal to axis line

        public List<string> Text;      // optional characters displayed next to a tic mark
        public Vector3D TextDir;       // usually along axis line
        public Vector3D TextUp;

        public Vector3D TextOffsetDirection;
        public double   TextOffsetDistance;

        public AxisLineView (AxisLineGeometry geom) : base (new Line3DGeometry (geom.P0, geom.P1))
        {
            geomCopy = geom;

            TicSize = 1;  // defaults that will almost never be acceptable
            TextSize = 1;

        }

        public AxisLineView (AxisLineView src, AxisLineGeometry geom) : base (new Line3DGeometry (geom.P0, geom.P1))
        {
            geomCopy   = geom;
            Color      = src.Color;
            TicDirs    = src.TicDirs;
            TextDir    = src.TextDir;
            TextUp     = src.TextUp;

            TextOffsetDistance  = src.TextOffsetDistance;
            TextOffsetDirection = src.TextOffsetDirection;

            TicsAt  = src.TicsAt;
            TicSize = src.TicSize;
            CustomText = src.CustomText;
            TextDisplay = src.TextDisplay;

            TextSize = src.TextSize;

        }

        public void ConstructChildren ()
        {
            if (TicsAt != null && TicsAt.Count > 0)
            {

                //if (TextDisplay == TextDisplayOptions.Custom)
                //{
                //    if (CustomText == null || CustomText.Count == 0)
                //        throw new Exception ("Custom Text option selected but no text specified");

                //    if (CustomText.Count != TicsAt.Count)
                //        throw new Exception ("Number of CustomText strings must match number of TicsAt locations");
                //}


                for (int i = 0; i<TicsAt.Count; i++)
                {
                    double tv = TicsAt [i];

                    if (tv >= geomCopy.TailCoordinate && tv <= geomCopy.HeadCoodinate)
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

                        if (TextDisplay == TextDisplayOptions.Custom)
                        {
                            Vector3D textOffset = TextOffsetDistance * TextOffsetDirection - (TextSize / 2) * TextDir;
                            Text3D txt = new Text3D (axisPoint + textOffset, TextDir, TextUp, TextSize, CustomText [i]);
                            txt.TextView.Color = Color;
                            Children.Add (txt.View);
                        }

                        else if (TextDisplay == TextDisplayOptions.Numbers)
                        {
                            Vector3D textOffset = TextOffsetDistance * TextOffsetDirection - (TextSize / 2) * TextDir;
                            Text3D txt = new Text3D (axisPoint + textOffset, TextDir, TextUp, TextSize, tv.ToString ());
                            txt.TextView.Color = Color;
                            Children.Add (txt.View);
                        }
                    }
                }
            }
        }

        //public AxisLineView (double min, double max, 
        //                     List<double> ticsAt,
        //                     List<Vector3D> ticDirs,
        //                     double ticSize, 
        //                     AxisLineGeometry geom) 
        //    : this (min, max, ticsAt, ticDirs, new Vector3D (), new Vector3D (), new Vector3D (), null, ticSize, 0, geom)
        //{
        //}



        //public AxisLineView (double min, double max, 
        //                     List<double> ticsAt,
        //                     List<Vector3D> ticDirs,
        //                     Vector3D textDir,
        //                     Vector3D textUp,
        //                     Vector3D textOffset,
        //                     List<string> text,  // text to display at each tic. can be null
        //                     double ticSize, 
        //                     double textSize, 
        //                     AxisLineGeometry geom) : base (geom)
        //{
        //    Color = Colors.Blue; // color;
        //    Thickness = 2; // t;
        //    ArrowEnds = Petzold.Media2D.ArrowEnds.End;

        //    TextUp = textUp;
        //    TextOffset = textOffset;

        //    //DashParameters dp = new DashParameters ();
        //    //dp.OffPercent = 60;
        //    //dp.OnPercent = 40;
        //    //dp.Cycles = 3;
        //    //SetDashParameters (dp);

        //    if (text != null)
        //    {
        //        Text = text;
        //    }

        //    else if (ticsAt != null)
        //    {
        //        Text = new List<string> ();

        //        foreach (double tic in ticsAt)
        //        {
        //            Text.Add (tic.ToString ()); // TBR!!!    
        //        }
        //    }

        //    TicDirs  = ticDirs;
        //    TextDir  = textDir;
        //    TicsAt   = ticsAt;
        //    TicSize  = ticSize;
        //    TextSize = textSize;



        //    if (TicsAt != null)
        //    {
        //        for (int i=0; i<ticsAt.Count; i++)
        //        {
        //            double tv = ticsAt [i];

        //            if (tv >= min && tv <= max)
        //            {
        //                // tic lines will pass through this point
        //                Point3D axisPoint = geom.P0 + (tv - min) * geom.Direction;

        //                foreach (Vector3D ticDir in ticDirs)
        //                {
        //                    Point3D p1 = axisPoint + TicSize / 2 * ticDir;
        //                    Point3D p2 = axisPoint - TicSize / 2 * ticDir;

        //                    WireLine tic = new WireLine {Point1 = p1, Point2 = p2, Color = Color, Thickness = Thickness};
        //                    Children.Add (tic);
        //                }

        //                if (Text != null && Text.Count > i)
        //                {
        //                    Point3D p1 = axisPoint + TextOffset;

        //                    Text3D txt = new Text3D (p1, TextDir, TextUp, TextSize, Text [i]);
        //                    txt.TextView.Color = Color;
        //                    Children.Add (txt.View);
        //                }
        //            }
        //        }
        //    }
        //}


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





        public Point3D ZeroPoint {get {return Geometry.ZeroPoint;} 
                                  set {Geometry.ZeroPoint = value; AxisView = new AxisLineView (AxisView, Geometry);}}



        public double  TailCoordinate {get {return Geometry.TailCoordinate;} 
                                       set {Geometry.TailCoordinate = value; AxisView = new AxisLineView (AxisView, Geometry);}}

        public double  HeadCoordinate {get {return Geometry.HeadCoodinate;} 
                                       set {Geometry.HeadCoodinate = value; AxisView = new AxisLineView (AxisView, Geometry);}}

        public Color Color {get {return AxisView.Color;}
                            set {AxisView.Color = value; AxisView.Children.Clear ();}}

        public List<double> TicsAt {get {return AxisView.TicsAt;}
                                    set {AxisView.TicsAt = value; AxisView.Children.Clear ();}}




        public double TicSize {get {return AxisView.TicSize;}
                               set {AxisView.TicSize = value; AxisView.Children.Clear ();}}





        public double TextSize {get {return AxisView.TextSize;}
                                set {AxisView.TextSize = value; AxisView.Children.Clear ();}}




        public AxisLineView.TextDisplayOptions TextDisplay {get {return AxisView.TextDisplay;}
                                                            set {AxisView.TextDisplay = value; AxisView.Children.Clear ();}}



        public List<string> CustomText 
        {
            get {return AxisView.CustomText;}

            set 
            {
                AxisView.CustomText = value; 
                
                if (AxisView.CustomText == null) AxisView.TextDisplay = AxisLineView.TextDisplayOptions.NoText;  
                else                             AxisView.TextDisplay = AxisLineView.TextDisplayOptions.Custom;
                
                AxisView.Children.Clear ();
            }
        }






        public Vector3D TextOffsetDirection  {get {return AxisView.TextOffsetDirection;}
                                              set {AxisView.TextOffsetDirection = value; AxisView.Children.Clear ();}}

        public double TextOffsetDistance  {get {return AxisView.TextOffsetDistance;}
                                           set {AxisView.TextOffsetDistance = value; AxisView.Children.Clear ();}}

    }

    //*********************************************************************************************************

    public class XAxisLine : AxisLine
    {
        public XAxisLine ()
        {
            Geometry = new AxisLineGeometry (new Vector3D (1, 0, 0));

            AxisLineView tempAxisView = new AxisLineView (Geometry);

            tempAxisView.Color      = Colors.Red;
            tempAxisView.TicDirs    = new List<Vector3D> () {YAxis, ZAxis}; // usually 2, perpendicular to axis line
            tempAxisView.TextDir    = XAxis; // usually along axis line
            tempAxisView.TextUp     = ZAxis;

            tempAxisView.TextOffsetDirection = -ZAxis;
            tempAxisView.TextOffsetDistance  = 1;

            AxisView = tempAxisView;
        }
    }

    //*********************************************************************************************************

    public class YAxisLine : AxisLine
    {
        public YAxisLine ()
        {
            Geometry = new AxisLineGeometry (new Vector3D (0, 1, 0));
            AxisView = new AxisLineView (Geometry);
        }

        //public YAxisLine (double min, double max, Point3D lineOrigin, List<double> ticsAt, double ticSize, double textSize)
        //{
            //Geometry = new AxisLineGeometry (min, max, lineOrigin, new Vector3D (0, 1, 0));
            //AxisView = new AxisLineView (ticsAt, ticSize, textSize, Geometry);

            //AxisView.ticDirs    = new List<Vector3D> () {new Vector3D (0, 0, 1), new Vector3D (1, 0, 0)};
            //AxisView.textDir    = new Vector3D (0, 1, 0);
            //AxisView.textUp     = new Vector3D (0, 0, 1);
            //AxisView.textOffset = new Vector3D (0, -3, -1) / Math.Sqrt (10) * textSize;
        //}


    }

    //*********************************************************************************************************

    public class ZAxisLine : AxisLine
    {
        public ZAxisLine ()
        {
            Geometry = new AxisLineGeometry (new Vector3D (0, 0, 1));
            AxisView = new AxisLineView (Geometry);
        }

        //public ZAxisLine (double min, double max, Point3D lineOrigin, List<double> ticsAt, double ticSize, double textSize)
        //{
            //Geometry = new AxisLineGeometry (min, max, lineOrigin, new Vector3D (0, 0, 1));
            //AxisView = new AxisLineView (ticsAt, ticSize, textSize, Geometry);

            //AxisView.ticDirs    = new List<Vector3D> () {new Vector3D (1, 0, 0), new Vector3D (0, 1, 0)};
            //AxisView.textDir    = new Vector3D (0, 0, 1);
            //AxisView.textUp     = new Vector3D (1, 1, 0);
            //AxisView.textOffset = new Vector3D (-1, -1, -3) / Math.Sqrt (11) * textSize;
        //}


    }
}







