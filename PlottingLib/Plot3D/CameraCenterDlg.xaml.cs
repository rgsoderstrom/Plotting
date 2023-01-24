using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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

namespace PlottingLib
{
    public partial class CameraCenterDlg : Window
    {
        public CameraCenterDlg ()
        {
            InitializeComponent ();
        }

        bool XValid = true;
        bool YValid = true;
        bool ZValid = true;

        double x, y, z;

        public double X { get {return x;} protected set {x = value;}}
        public double Y { get {return y;} protected set {y = value;}}
        public double Z { get {return z;} protected set {z = value;}}

        private void CenterOnAcceptButton_Click (object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CenterXCoord_TextChanged (object sender, TextChangedEventArgs e)
        {
            if (CenterAcceptButton == null) return;
            XValid = double.TryParse (CenterXCoord.Text, out x);
            CenterAcceptButton.IsEnabled = XValid && YValid && ZValid;
        }

        private void CenterYCoord_TextChanged (object sender, TextChangedEventArgs e)
        {
            if (CenterAcceptButton == null) return;
            YValid = double.TryParse (CenterYCoord.Text, out y);
            CenterAcceptButton.IsEnabled = XValid && YValid && ZValid;
        }

        private void CenterZCoord_TextChanged (object sender, TextChangedEventArgs e)
        {
            if (CenterAcceptButton == null) return;
            ZValid = double.TryParse (CenterZCoord.Text, out z);
            CenterAcceptButton.IsEnabled = XValid && YValid && ZValid;
        }
    }
}
