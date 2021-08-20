using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Text;
using System.Threading.Tasks;

namespace Plot3D_Embedded
{
    public static class EulerAngles
    {
        //*******************************************************************

        // Supported Euler Angle conventions. Upper case indicate fixed (or parent) axes,
        // lower case indicate mobile (or child) axes

        // see https://www.mecademic.com/resources/Euler-angles/Euler-angles 
        // see https://demonstrations.wolfram.com/EulerAngles/ for Taylor and M-T conventions

        public enum Convention {Fixed, Mobile, Taylor, MT}

        static Dictionary<Convention, string> ConventionToString = new Dictionary<Convention, string> ()
        {
            {Convention.Fixed,  "XYZ"},
            {Convention.Mobile, "xyz"},
            {Convention.Taylor, "Zyz"},  // Taylor's book, Ch. 10
            {Convention.MT,     "Zxz"},  // Marion-Thornton book, Ch. 10
        };

        //**********************************************************************************************
        //**********************************************************************************************
        //**********************************************************************************************
        //
        // Construct rotation based on pre-defined convention
        //
        public static Transform3DGroup Rotation (double Angle1, double Angle2, double Angle3,
                                                 Convention convention)
        {
            try
            {
                return Rotation (Angle1, Angle2, Angle3, ConventionToString [convention]);
            }

            catch (Exception ex)
            {
                throw new Exception (string.Format ("Exception Creating Euler Rotation: ", ex.Message)); 
            }
        }

        //**********************************************************************************************
        //
        // Construct rotation for arbitrary set of axes
        //
       public static Transform3DGroup Rotation (double A, double B, double C,
                                                 string axes) // 3 character string
        {
            Transform3DGroup rotate = new Transform3DGroup ();

            AddOneRotation (rotate, A, axes [0]);
            AddOneRotation (rotate, B, axes [1]);
            AddOneRotation (rotate, C, axes [2]);

            return rotate;
        }

        //**********************************************************************************************
        //**********************************************************************************************
        //**********************************************************************************************

        // Upper case designator indicates rotation about fixed axis
        // Lower case about mobile (i.e. this one being created) axis

        static Vector3D xAxis = new Vector3D (1, 0, 0);
        static Vector3D yAxis = new Vector3D (0, 1, 0);
        static Vector3D zAxis = new Vector3D (0, 0, 1);

        private static void AddOneRotation (Transform3DGroup current, 
                                            double angle, char axisDesignator)
        {
            RotateTransform3D thisRotate;
            AxisAngleRotation3D AAR; 

            switch (axisDesignator)
            {
                case 'X':
                    AAR = new AxisAngleRotation3D (xAxis, angle); 
                    break;

                case 'Y':
                    AAR = new AxisAngleRotation3D (yAxis, angle); 
                    break;

                case 'Z':
                    AAR = new AxisAngleRotation3D (zAxis, angle); 
                    break;

                case 'x':
                    {
                        Vector3D nextAxis = current.Transform (xAxis);
                        AAR = new AxisAngleRotation3D (nextAxis, angle); 
                    }
                    break;

                case 'y':
                    {
                        Vector3D nextAxis = current.Transform (yAxis);
                        AAR = new AxisAngleRotation3D (nextAxis, angle); 
                    }
                    break;

                case 'z':
                    {
                        Vector3D nextAxis = current.Transform (zAxis);
                        AAR = new AxisAngleRotation3D (nextAxis, angle); 
                    }
                    break;

                default: 
                    throw new Exception ("CartesianAxes3DEulerAngles.OneRotation: unknown axis designation");
            }

            thisRotate = new RotateTransform3D (AAR);
            current.Children.Add (thisRotate);            
        }
    }
}
