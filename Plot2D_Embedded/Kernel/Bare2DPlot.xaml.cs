using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Common;

namespace Plot2D_Embedded
{
    public partial class Bare2DPlot : UserControl
    {
        public ScaleTransform aspectRatio {get; protected set;} = new ScaleTransform ();


        Plot2DGrid_Rectangular rectangularGrid;
        Plot2DGrid_Polar polarGrid;

        public Bare2DPlot ()
        {
            InitializeComponent ();
        }

        public void InnerCanvasBackgroundColor (SolidColorBrush br)
        {
            InnerCanvas.Background = br;
        }


        bool rectangularGridOn = false;
        public bool RectangularGridOn {get {return rectangularGridOn;}
                                       set {rectangularGridOn = value; Draw ();}}

        bool polarGridOn = false;
        public bool PolarGridOn {get {return polarGridOn;}
                                 set {polarGridOn = value; Draw ();}}


        //public int NumberXAxisTics {get {return rectangularGrid.numberXAxisMarks;} set {rectangularGrid.numberXAxisMarks = value; Draw ();}}
        //public int NumberYAxisTics {get {return rectangularGrid.numberYAxisMarks;} set {rectangularGrid.numberYAxisMarks = value; Draw ();}}

        public Brush AnootationBackground {get {return OuterCanvas.Background;} set {OuterCanvas.Background = value;}}



        private void UserControl_Loaded (object sender, RoutedEventArgs e)
        {
           // EventLog.WriteLine ("Plot2D UserControl_Loaded begins");

            Canvas.SetLeft (InnerBorder, DataAreaLeft);
            Canvas.SetTop  (InnerBorder, DataAreaTop);

            rectangularGrid  = new Plot2DGrid_Rectangular (this);
            polarGrid        = new Plot2DGrid_Polar (this);

            InnerCanvas.Background = Brushes.White;
            InnerCanvas.Focusable = true;

            EnableMouse ();
            //InnerCanvas.MouseWheel          += InnerCanvas_MouseWheel;
            //InnerCanvas.MouseLeftButtonDown += InnerCanvas_MouseLeftButtonDown;
            //InnerCanvas.MouseLeftButtonUp   += InnerCanvas_MouseLeftButtonUp;
            //InnerCanvas.KeyDown             += InnerCanvas_KeyDown;
            //InnerCanvas.KeyUp               += InnerCanvas_KeyUp;
            //InnerCanvas.LostMouseCapture    += InnerCanvas_LostMouseCapture;
            //InnerCanvas.MouseMove           += InnerCanvas_MouseMove;

            Keyboard.Focus (InnerCanvas);

            //EventLog.WriteLine ("Plot2D UserControl_Loaded ends");
        }

        void EnableMouse ()
        {
            InnerCanvas.MouseWheel          += InnerCanvas_MouseWheel;
            InnerCanvas.MouseLeftButtonDown += InnerCanvas_MouseLeftButtonDown;
            InnerCanvas.MouseLeftButtonUp   += InnerCanvas_MouseLeftButtonUp;
            InnerCanvas.KeyDown             += InnerCanvas_KeyDown;
            InnerCanvas.KeyUp               += InnerCanvas_KeyUp;
            InnerCanvas.LostMouseCapture    += InnerCanvas_LostMouseCapture;
            InnerCanvas.MouseMove           += InnerCanvas_MouseMove;
        }

        void DisableMouse ()
        {
            InnerCanvas.MouseWheel          -= InnerCanvas_MouseWheel;
            InnerCanvas.MouseLeftButtonDown -= InnerCanvas_MouseLeftButtonDown;
            InnerCanvas.MouseLeftButtonUp   -= InnerCanvas_MouseLeftButtonUp;
            InnerCanvas.KeyDown             -= InnerCanvas_KeyDown;
            InnerCanvas.KeyUp               -= InnerCanvas_KeyUp;
            InnerCanvas.LostMouseCapture    -= InnerCanvas_LostMouseCapture;
            InnerCanvas.MouseMove           -= InnerCanvas_MouseMove;
        }


        private void OuterCanvas_Loaded (object sender, RoutedEventArgs e)
        {
            //EventLog.WriteLine ("Plot2D OuterCanvas loaded");
        }

        private void InnerBorder_Loaded (object sender, RoutedEventArgs e)
        {
            //EventLog.WriteLine ("Plot2D InnerBorder loaded");
        }

        private void InnerCanvas_Loaded (object sender, RoutedEventArgs e)
        {
            //EventLog.WriteLine ("Plot2D InnerCanvas loaded");
        }
    }
}
