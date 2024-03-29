﻿using System;
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

            Camera3D = new ProjectionCameraWrapper (ProjectionType.Perspective);
            //Camera3D = new ProjectionCameraWrapper (ProjectionType.Orthograpic);
            mouseTracking = new MouseTracking (Viewport, Camera3D);

            lights.Direction = Camera3D.CenterOn - Camera3D.AbsPosition;

            //Camera3D.RelPositionRho = 10; // 150;

            ThetaScrollbar.Value = Camera3D.Theta;
            PhiScrollbar.Value   = Camera3D.Phi;
            RhoScrollbar.Value   = Camera3D.Rho;

            if (Camera3D.Camera is OrthographicCamera)
            {
                WidthScrollbar.Value = Camera3D.Width;
                FovScrollbar.Visibility = Visibility.Collapsed;
            }

            if (Camera3D.Camera is PerspectiveCamera)
            {
                FovScrollbar.Value   = Camera3D.FOV;
                WidthScrollbar.Visibility = Visibility.Collapsed;
            }

            ThetaScrollbar.Maximum = 360;   ThetaScrollbar.Minimum = 0; ThetaScrollbar.SmallChange = 1; ThetaScrollbar.LargeChange = 5;
            PhiScrollbar.Maximum   = 180;   PhiScrollbar.Minimum   = 0; PhiScrollbar.SmallChange = 1;   PhiScrollbar.LargeChange = 5;
            WidthScrollbar.Maximum = 250;   WidthScrollbar.Minimum = 5;
            RhoScrollbar.Maximum   = 100;   RhoScrollbar.Minimum   = 3; RhoScrollbar.SmallChange = 0.5; RhoScrollbar.LargeChange = 5;
            FovScrollbar.Maximum   = 70;    FovScrollbar.Minimum   = 1;

            ThetaScrollbar.ToolTip = "Theta";
            PhiScrollbar.ToolTip   = "Phi";
            WidthScrollbar.ToolTip = "Width";
            RhoScrollbar.ToolTip   = "Rho";
            FovScrollbar.ToolTip   = "FOV";

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

            bind = new Binding ();
            bind.Source = Camera3D;
            bind.Path = new PropertyPath (ProjectionCameraWrapper.RhoProperty);
            RhoScrollbar.SetBinding (ScrollBar.ValueProperty, bind);
        }

        public Brush AnotationBackground {get {return OuterCanvas.Background;} set {OuterCanvas.Background = value;}}
    }
}
