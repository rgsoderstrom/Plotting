using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

using System.Reflection;

using Common;

namespace Plot2D_Embedded
{
    internal class Colormap
    {
        public List<Color> colors = new List<Color> ();

        public Colormap (string name, bool reverseFlag = false)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Plot2D_Embedded.Kernel." + name + ".txt";

                Stream stream = assembly.GetManifestResourceStream (resourceName);

                if (stream == null)
                    throw new Exception ("Error reading Colormap resource");

                StreamReader file = new StreamReader (stream);
                string raw;

                while ((raw = file.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        string [] tokens = raw.Split (new char [] { ' ' });

                        if (raw [0] == '%') // comment
                            continue;

                        if (tokens.Length == 3)
                        {
                            byte r = byte.Parse (tokens [0]);
                            byte g = byte.Parse (tokens [1]);
                            byte b = byte.Parse (tokens [2]);

                            colors.Add (Color.FromArgb (128, r, g, b));
                        }
                    }
                }

                file.Close ();

                if (reverseFlag)
                    colors.Reverse ();
            }

            catch (FileNotFoundException ex)
            {
                EventLog.WriteLine  ("Colormap not found exception: " + ex.Message);
                throw new Exception ("Colormap not found exception: " + ex.Message);
            }

            catch (Exception ex)
            {
                EventLog.WriteLine  ("Colormap loading exception: " + ex.Message);
                throw new Exception ("Colormap loading exception: " + ex.Message);
            }
        }
    }
}
