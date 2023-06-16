//using System.Windows;
//using System.Windows.Media;

///// <summary>
///// PlotFigure - lets the user create a figure without declaring it 2D or 3D. Can set its position
/////              and size, but can't draw on it. Will be used to construct a Plot2D or Plot3D when
/////              a plot function is called
///// </summary>

//namespace PlottingLib
//{
//    /// <summary>
//    /// PlotFigure - a placeholder for a 2D or 3D plot. Lets an application (e.g. PlotLab uses this) open a
//    /// window and assign some properties before deciding whether it is a 2D or 3D plot
//    /// </summary>
    
//    public class PlotFigure : Window, IPlotCommon
//    {
//        // these are read by Plot2D and 3D ctors
//        public int ID {get; set;}
//        public bool Hold {get; set;}

//        public new double Left   {get { return base.Left; }   set { base.Left = value; } }
//        public new double Top    {get { return base.Top; }    set { base.Top = value; } }
//        public new double Width  {get { return base.Width; }  set { base.Width = value; } }
//        public new double Height {get { return base.Height; } set { base.Height = value; } }

//        protected string dataAreaTitle = "";
//        public string DataAreaTitle
//        {
//            get { return dataAreaTitle; }
//            set { dataAreaTitle = value; /*Draw ();*/ }
//        }


//        private System.Windows.Controls.Border Border = new System.Windows.Controls.Border ();

//        public PlotFigure ()
//        {
//            Loaded += Window_Loaded;
//            SizeChanged += Figure_SizeChanged;
//            Show ();
//        }

//        private void Figure_SizeChanged (object sender, SizeChangedEventArgs e)
//        {
//            Border.Height = Height - 150;
//            Border.Width = Width - 200;
//        }

//        protected virtual void Window_Loaded (object sender, RoutedEventArgs e)
//        {
//            Common.WindowLoaded (this);
//            ID = Common.instanceCounter;

//            MatlabStyle ();
//            Title = string.Format ("Figure {0}", ID);
//        }

//        private void  MatlabStyle ()
//        {
//            Background = new SolidColorBrush (Color.FromRgb (0xa8, 0xa8, 0xa8));
//            Height = 650;
//            Width = 800;

//            Border = new System.Windows.Controls.Border (); 
//            Border.BorderThickness = new Thickness (1);
//            Border.BorderBrush = Brushes.Black;
//            Border.Background = Brushes.White;
//            Border.Height = Height - 150;
//            Border.Width = Width - 200;

//            AddChild (Border);
//        }

//        public override string ToString ()
//        {
//            return "PlotFigure " + ID.ToString ();
//        }
//    }
//}
