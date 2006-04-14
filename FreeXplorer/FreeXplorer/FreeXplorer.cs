using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Wizou.FreeXplorer
{
    static class FreeXplorer
    {
        public static string ConfigurationFolder
        {
            get
            {
                // http://www.mono-project.com/FAQ:_Technical#How_to_detect_the_execution_platform_.3F
                int platform = (int) Environment.OSVersion.Platform;
                if ((platform == 4) || (platform == 128))
                {
                    return Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.Personal), ".freexplorer");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "FreeXplorer");
                }
            }
        }
    }
}
