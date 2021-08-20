
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class Text3DGeometry : ViewportObjectGeometry
    {
        public Point3D Origin;
        public Vector3D Direction;
        public Vector3D Up;
        public double Size;

        public string text;

        public Text3DGeometry (Point3D org, Vector3D dir, Vector3D up, double size, string txt)
        {
            BoundingBox.Union (org);
            BoundingBox.Union (org + dir * size * txt.Length + up * size);

            Origin = org;
            Direction = dir;
            Up = up;
            Size = size;
            text = txt;
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Text3DView : WireLines
    {
        static Color DefaultLineColor = Colors.Black;
        static readonly double DefaultLineThickness = 2;

        Text3DGeometry geom;

        public bool OrientationFrozen {get; set;} = false;
        
        public TextGenerator gen = new TextGenerator ();
        public Point3DCollection textStrokes = new Point3DCollection ();

        public Text3DView (Text3DGeometry geo)
        {
            geom = geo;
            Color = DefaultLineColor;
            Thickness = DefaultLineThickness;

            
            gen.FontSize = geom.Size;
            gen.Origin = geom.Origin;
            gen.BaselineDirection = geom.Direction;
            gen.UpDirection = geom.Up;
            gen.Generate (textStrokes, geom.text);

            Lines = textStrokes;
        }

        public double Size {get {return geom.Size;} 
                            set {geom.Size = value; gen.FontSize = geom.Size; textStrokes.Clear (); gen.Generate (textStrokes, geom.text);}}

        public void Orientation (Vector3D Up, Vector3D right)
        {
            geom.Direction = right;
            geom.Up = Up;

            gen.BaselineDirection = geom.Direction;
            gen.UpDirection = geom.Up;

            textStrokes.Clear ();
            gen.Generate (textStrokes, geom.text);

            Lines = textStrokes;
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class Text3D : ViewportObject
    {
        public Text3DGeometry Geometry {get; protected set;}
        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public override ModelVisual3D View {get {return TextView;}}
        public Text3DView TextView {get; protected set;}

        public Text3D (Point3D pos, double size, string txt) : this (pos, new Vector3D (pos.X, pos.Y, 0), new Vector3D (0, 0, 1), size, txt)
        {
            TextView.OrientationFrozen = false;
        }

        public Text3D (Point3D pos, Vector3D dir, Vector3D up, double size, string txt)
        {
            if (Vector3D.DotProduct (dir, dir) < 1e-6) // may happen if "pos" is near Z axis
                dir = new Vector3D (1, 0, 0);

            Geometry = new Text3DGeometry (pos, dir, up, size, txt);
            TextView = new Text3DView (Geometry);
            TextView.OrientationFrozen = true;
        }

        public void Orientation (Vector3D Up, Vector3D right)
        {
            Geometry.Direction = right;
            Geometry.Up = Up;
            TextView = new Text3DView (Geometry);
        }
    }
}
