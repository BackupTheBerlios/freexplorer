using System;

namespace Wizou.FreeXplorer
{
    public enum OSType { Windows, Unix }

    public static class OSUtils
    {
        public static OSType OSType
        {
            get
            {
                // Pour une explication sur le 128 voir :
                // http://www.mono-project.com/FAQ:_Technical#How_to_detect_the_execution_platform_.3F
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                    case (PlatformID)128:
                        return OSType.Unix;

                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return OSType.Windows;

                    default:
                        throw new Exception("OS non supporté par FreeXplorer");
                }
            }
        }

        public static bool IsWindows
        {
            get
            {
                return (OSType == OSType.Windows);
            }
        }

        public static bool IsUnix
        {
            get
            {
                return (OSType == OSType.Unix);
            }
        }
    }
}
