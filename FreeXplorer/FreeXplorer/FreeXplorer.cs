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
                return Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "FreeXplorer");
            }
        }
    }
}
