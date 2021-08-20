
//
// Plot Vector 
//

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using CommonMath;
using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class PlotVector3DGeometry : ViewportObjectGeometry
    {
        public List<Vector3D> basisList;
        public Matrix3x3 basis;
        
        public ColumnVector tail = new ColumnVector (3);  // in user specified basis
        public ColumnVector vect = new ColumnVector (3);  // "   "       "       "

        public Point3D Tail; // in standard basis
        public Point3D Head; // "     "       "
       
        public PlotVector3DGeometry (Point3D p, Vector3D d, List<Vector3D> b)
        {
            basisList = b;
            tail  = new ColumnVector (new List<double> () {p.X, p.Y, p.Z });
            vect  = new ColumnVector (new List<double> () {d.X, d.Y, d.Z });
            basis = new Matrix3x3 ();

            basis.FillOneColumn (0, new List<double> {b [0].X, b [0].Y, b [0].Z});
            basis.FillOneColumn (1, new List<double> {b [1].X, b [1].Y, b [1].Z});
            basis.FillOneColumn (2, new List<double> {b [2].X, b [2].Y, b [2].Z});

            ColumnVector p0 = new ColumnVector (basis * tail);
            Tail = new Point3D (p0 [0], p0 [1], p0 [2]);

            p0 = new ColumnVector (basis * (tail + vect));
            Head = new Point3D (p0 [0], p0 [1], p0 [2]);
       
            BoundingBox.Union (Tail);
            BoundingBox.Union (Head);
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class PlotVector3DView : ModelVisual3D 
    {
        public Line3DView ArrowView;
        public Line3DView XComponentView;
        public Line3DView YComponentView;
        public Line3DView ZComponentView;

        //***********************************************************************************

        public Color Color {get {return ArrowView.Color;} set {ArrowView.Color = value;}}

        //***********************************************************************************

        private bool showComponents = false;

        public bool ShowComponents
        {
            get {return showComponents;}

            set {if (showComponents != value) {showComponents = value; if (showComponents == true) {Children.Add    (XComponentView); Children.Add    (YComponentView); Children.Add    (ZComponentView);}
                                                                       else                        {Children.Remove (XComponentView); Children.Remove (YComponentView); Children.Remove (ZComponentView);}}
            }
        }

        //***********************************************************************************

        public PlotVector3DView (PlotVector3DGeometry geom)
        {
            ArrowView = new Line3DView (new Line3DGeometry (geom.Tail, geom.Head));
            ArrowView.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
            ArrowView.Color = Colors.Black;




            bool chainComponentVectors = true;

            Point3D x0 = new Point3D (0, 0, 0);
            Point3D x1 = x0 + geom.vect [0] * geom.basisList [0];

            Point3D y0 = chainComponentVectors ? x1 : new Point3D (0, 0, 0);
            Point3D y1 = y0 + geom.vect [1] * geom.basisList [1];

            Point3D z0 = chainComponentVectors ? y1 : new Point3D (0, 0, 0);
            Point3D z1 = z0 + geom.vect [2] * geom.basisList [2];





            XComponentView = new Line3DView (new Line3DGeometry (x0, x1));
            YComponentView = new Line3DView (new Line3DGeometry (y0, y1));
            ZComponentView = new Line3DView (new Line3DGeometry (z0, z1));

            XComponentView.Color = YComponentView.Color = ZComponentView.Color = Colors.LightGray;
            XComponentView.ArrowEnds = YComponentView.ArrowEnds = ZComponentView.ArrowEnds = Petzold.Media2D.ArrowEnds.End;

            Children.Add (ArrowView);
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class PlotVector3D : ViewportObject
    {
        static readonly List<Vector3D> StandardBasis = new List<Vector3D> () {new Vector3D (1, 0, 0), new Vector3D (0, 1, 0), new Vector3D (0, 0, 1)};

        public PlotVector3DGeometry Geometry {get; protected set;}
        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public override ModelVisual3D View {get {return VectorView;}}
        public PlotVector3DView VectorView {get; protected set;}

        public PlotVector3D (Vector3D d) : this (new Point3D (0, 0, 0), d, StandardBasis)
        {
        }

        public PlotVector3D (Point3D p, Vector3D d) : this (p, d, StandardBasis)
        {
        }

        public PlotVector3D (Vector3D d, List<Vector3D> b) : this (new Point3D (0, 0, 0), d, b)
        {
        }

        public PlotVector3D (Point3D p, Vector3D d, List<Vector3D> b)
        {
            Geometry = new PlotVector3DGeometry (p, d, b);
            VectorView = new PlotVector3DView (Geometry);
        }
    }
}
