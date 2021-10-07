using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    public class BoundingBox3D : ViewportObject
    {
        override protected ModelVisual3D BoundingBoxView {get {return (new BoundingBox3D ()).View;}}
        override public    BoundingBox3D BoundingBox     {get {return new BoundingBox3D ();}}

        bool empty = true;
        public double MaxX {get {return maxX;} protected set {maxX = value;}}
        public double MinX {get {return minX;} protected set {minX = value;}}

        public double MaxY {get {return maxY;} protected set {maxY = value;}}
        public double MinY {get {return minY;} protected set {minY = value;}}

        public double MaxZ {get {return maxZ;} protected set {maxZ = value;}}
        public double MinZ {get {return minZ;} protected set {minZ = value;}}

        public double XSize {get {return MaxX - MinX;}}
        public double YSize {get {return MaxY - MinY;}}
        public double ZSize {get {return MaxZ - MinZ;}}

        //public bool IsValid {get {return XSize > 0 && YSize > 0 && ZSize > 0;}}
        public bool IsValid {get {return !empty;}}

        double minX = 0, maxX = 0;
        double minY = 0, maxY = 0;
        double minZ = 0, maxZ = 0;

        public Point3D Center {get {return new Point3D ((MinX + MaxX) / 2, (MinY + MaxY) / 2, (MinZ + MaxZ) / 2);}}

        public double DiagonalSize
        {
            get
            {
                double dx = MaxX - MinX;
                double dy = MaxY - MinY;
                double dz = MaxZ - MinZ;
                return Math.Sqrt (dx * dx + dy * dy + dz * dz);
            }
        }

        //*****************************************************************

        public void Clear ()
        {
            empty = true;
            UpdateView ();
        }

        //*****************************************************************
    
        public void Union (BoundingBox3D other)
        {
            if (other.empty)
                return;

            if (empty)
            {
                MinX = other.MinX;
                MaxX = other.MaxX;

                MinY = other.MinY;
                MaxY = other.MaxY;

                MinZ = other.MinZ;
                MaxZ = other.MaxZ;

                empty = false;
            }
            else
            {
                if (MaxX < other.MaxX) MaxX = other.MaxX;
                if (MinX > other.MinX) MinX = other.MinX;
                if (MaxY < other.MaxY) MaxY = other.MaxY;
                if (MinY > other.MinY) MinY = other.MinY;
                if (MaxZ < other.MaxZ) MaxZ = other.MaxZ;
                if (MinZ > other.MinZ) MinZ = other.MinZ;
            }

            UpdateView ();
        }

        //*****************************************************************
    
        public void Union (Point3D pt)
        {
            if (empty)
            {
                MinX = pt.X - double.Epsilon;
                MaxX = pt.X + double.Epsilon;

                MinY = pt.Y - double.Epsilon;
                MaxY = pt.Y + double.Epsilon;

                MinZ = pt.Z - double.Epsilon;
                MaxZ = pt.Z + double.Epsilon;

                empty = false;
            }
            else
            {
                if (MaxX < pt.X) MaxX = pt.X + double.Epsilon;
                if (MinX > pt.X) MinX = pt.X - double.Epsilon;
                if (MaxY < pt.Y) MaxY = pt.Y + double.Epsilon;
                if (MinY > pt.Y) MinY = pt.Y - double.Epsilon;
                if (MaxZ < pt.Z) MaxZ = pt.Z + double.Epsilon;
                if (MinZ > pt.Z) MinZ = pt.Z - double.Epsilon;
            }

            UpdateView ();
        }

        //*****************************************************************

        public void Union (List<Point3D> pts)
        {
            foreach (Point3D pt in pts)
                Union (pt);
        }
    
        //*****************************************************************

        public override ModelVisual3D View {get {if (view == null) UpdateView (); return view;}}
        ModelVisual3D view;

        void UpdateView ()
        {
            List<Point3D> bottom = new List<Point3D> ()
            {
                new Point3D (MinX, MinY, MinZ),
                new Point3D (MaxX, MinY, MinZ),
                new Point3D (MaxX, MaxY, MinZ),
                new Point3D (MinX, MaxY, MinZ),
                new Point3D (MinX, MinY, MinZ),
            };

            List<Point3D> top = new List<Point3D> ()
            {
                new Point3D (MinX, MinY, MaxZ),
                new Point3D (MaxX, MinY, MaxZ),
                new Point3D (MaxX, MaxY, MaxZ),
                new Point3D (MinX, MaxY, MaxZ),
                new Point3D (MinX, MinY, MaxZ),
            };

            // copied from PetzoldMedia3D\Test_Wires2\MainWindow.xaml.cs, method Box ()

            double t = 0.5; // wire thickness
            Color color = Colors.Red;

            List<WireLine> lines = new List<WireLine> (12);

            WireLine line;
            line = new WireLine (); line.Point1 = top [0]; line.Point2 = top [1]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = top [1]; line.Point2 = top [2]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = top [2]; line.Point2 = top [3]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = top [3]; line.Point2 = top [0]; line.Color = color; line.Thickness = t; lines.Add (line);

            line = new WireLine (); line.Point1 = bottom [0]; line.Point2 = bottom [1]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [1]; line.Point2 = bottom [2]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [2]; line.Point2 = bottom [3]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [3]; line.Point2 = bottom [0]; line.Color = color; line.Thickness = t; lines.Add (line);

            line = new WireLine (); line.Point1 = bottom [0]; line.Point2 = top [0]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [1]; line.Point2 = top [1]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [2]; line.Point2 = top [2]; line.Color = color; line.Thickness = t; lines.Add (line);
            line = new WireLine (); line.Point1 = bottom [3]; line.Point2 = top [3]; line.Color = color; line.Thickness = t; lines.Add (line);

            if (view == null)
                view = new ModelVisual3D ();
            else
                view.Children.Clear ();

            foreach (Visual3D l in lines)
                view.Children.Add (l);
        }

        //*****************************************************************
        //*****************************************************************
        //*****************************************************************
    
        override public string ToString ()
        {
            string str = string.Format ("({0:0.0}, {1:0.0}),  ({2:0.0}, {3:0.0}),  ({4:0.0}, {5:0.0})", 
                                       MinX, MaxX, MinY, MaxY, MinZ, MaxZ);
            return str;
        }
    }
}
