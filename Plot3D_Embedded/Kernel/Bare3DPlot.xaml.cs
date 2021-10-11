using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using WPF3D.Lighting;
using WPF3D.Cameras;
using WPF3D.MouseTracking;

namespace Plot3D_Embedded
{
    public partial class Bare3DPlot : UserControl
    {
        Lights lights = new Lights ();
        ProjectionCameraWrapper Camera3D = null;
        MouseTracking mouseTracking = null;

        public Bare3DPlot ()
        {
            InitializeComponent ();

            Camera3D = new ProjectionCameraWrapper (ProjectionType.Perspective, lights);
            //Camera3D = new ProjectionCameraWrapper (ProjectionType.Orthograpic, lights);
            mouseTracking = new MouseTracking (Viewport, Camera3D);
            lights.Direction = Camera3D.Direction;

            Camera3D.Width = 16;
            Camera3D.Camera.NearPlaneDistance = 1e-9;
            Camera3D.Camera.FarPlaneDistance = 1e8;
            Camera3D.Rho = 10; // 150;

            ThetaScrollbar.Value = Camera3D.Theta;
            PhiScrollbar.Value   = Camera3D.Phi;
            WidthScrollbar.Value = Camera3D.Width;
            RhoScrollbar.Value   = Camera3D.Rho;
            FovScrollbar.Value   = Camera3D.FOV;

            ThetaScrollbar.Maximum = 360; ThetaScrollbar.Minimum = 0;
            PhiScrollbar.Maximum   = 360; PhiScrollbar.Minimum   = 0;
            WidthScrollbar.Maximum = 250; WidthScrollbar.Minimum = 5;
            RhoScrollbar.Maximum   = 280; RhoScrollbar.Minimum   = 3;
            FovScrollbar.Maximum   = 70; FovScrollbar.Minimum    = 1;

            ThetaScrollbar.ToolTip = "Theta";
            PhiScrollbar.ToolTip   = "Phi";
            WidthScrollbar.ToolTip = "Width";
            RhoScrollbar.ToolTip   = "Rho";
            FovScrollbar.ToolTip   = "FOV";

            if (Camera3D.Camera is PerspectiveCamera)
                WidthScrollbar.Visibility = Visibility.Collapsed; // not needed for perspective camera
            else
                FovScrollbar.Visibility = Visibility.Collapsed; // not needed for orthographic camera

            PhiScrollbar.ValueChanged   += PhiScrollbar_ValueChanged;
            ThetaScrollbar.ValueChanged += ThetaScrollbar_ValueChanged;
            WidthScrollbar.ValueChanged += WidthScrollbar_ValueChanged;
            RhoScrollbar.ValueChanged   += RhoScrollbar_ValueChanged;
            FovScrollbar.ValueChanged   += FovScrollbar_ValueChanged;

            // Data Binding - camera to scroll bars
            Binding bind = new Binding ();
            bind.Source = Camera3D;
            bind.Path = new PropertyPath (ProjectionCameraWrapper.PhiProperty);
            PhiScrollbar.SetBinding (ScrollBar.ValueProperty, bind); 

            bind = new Binding ();
            bind.Source = Camera3D;
            bind.Path = new PropertyPath (ProjectionCameraWrapper.ThetaProperty);
            ThetaScrollbar.SetBinding (ScrollBar.ValueProperty, bind);
        }

        public Brush AnotationBackground {get {return OuterCanvas.Background;} set {OuterCanvas.Background = value;}}
    }
}
