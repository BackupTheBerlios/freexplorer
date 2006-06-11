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
                switch (OSUtils.OSType)
                {
                    case OSType.Windows:
                        return Path.Combine(Environment.GetFolderPath(
                            Environment.SpecialFolder.ApplicationData), "FreeXplorer");
                    
                    case OSType.Unix :
                    default :
                        return Path.Combine(Environment.GetFolderPath(
                            Environment.SpecialFolder.Personal), ".freexplorer");
                }
            }
        }
    }
}
