
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Plot3D_Embedded
{
    public abstract class Surface3DView : ModelVisual3D
    {
        //***********************************************************************

        double frontOpacity = 1;
        double backOpacity = 1;

        public double Opacity
        {
            get {return frontOpacity;}

            set
            {
                FrontOpacity = value;
                BackOpacity = value;
            }
        }

        public double FrontOpacity
        {
            get {return frontOpacity;}

            set
            {
                frontOpacity = value;
                if (frontOpacity > 1) frontOpacity = 1;
                if (frontOpacity < 0) frontOpacity = 0;

                FrontColor = FrontColor; // force setter to run
            }
        }

        public double BackOpacity
        {
            get {return backOpacity;}

            set
            {
                backOpacity = value;
                if (backOpacity > 1) backOpacity = 1;
                if (backOpacity < 0) backOpacity = 0;

                BackColor = BackColor; // force setter to run
            }
        }

        //***********************************************************************

        Color frontColor;
        Color backColor;

        public Color FrontColor
        {
            get {return frontColor;}

            set
            {
                frontColor = value;

                SolidColorBrush b1 = new SolidColorBrush ();
                b1.Opacity = FrontOpacity;
                b1.Color = FrontColor;

                SolidColorBrush b2 = new SolidColorBrush ();
                b2.Opacity = FrontOpacity;
                b2.Color = Colors.White;

                MaterialGroup mg = new MaterialGroup ();
                mg.Children.Add (new DiffuseMaterial (b1));
                mg.Children.Add (new SpecularMaterial (b2, 40));

                if (Content is GeometryModel3D)
                    (Content as GeometryModel3D).Material = mg;

                else if (Content is Model3DGroup)
                    foreach (GeometryModel3D gm in (Content as Model3DGroup).Children)
                        gm.Material = mg;

                else
                    throw new Exception ("Unsupported Content type in Surface3DView Color");
            }
        }

        public Color BackColor
        {
            get {return backColor;}

            set
            {
                backColor = value;

                SolidColorBrush b = new SolidColorBrush ();
                b.Opacity = BackOpacity;
                b.Color =   BackColor;

                if (Content is GeometryModel3D)
                    (Content as GeometryModel3D).BackMaterial = new DiffuseMaterial (b);

                else if (Content is Model3DGroup)
                    foreach (GeometryModel3D gm in (Content as Model3DGroup).Children)
                        gm.BackMaterial = new DiffuseMaterial (b);

                else
                    throw new Exception ("Unsupported Content type in Surface3DView BackColor");
            }
        }

        public Color Color  // set front & back to same
        {
            get {return frontColor;}

            set
            {
                FrontColor = value;
                BackColor = value;
            }
        }
    }
}
