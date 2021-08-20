using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Plot2D_Embedded;

namespace PlottingLib
{
    public static class Common
    {
        public static int instanceCounter = 0;

        public static void WindowLoaded (Window figure)
        {
            int m = instanceCounter++ % 10;
            figure.Top = 30 + 30 * m;
            figure.Left = 500 + 30 * m;
        }
    }
}
