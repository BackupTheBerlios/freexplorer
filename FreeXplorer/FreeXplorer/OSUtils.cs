/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 *
 * Copyright (C) 2006 Roncaglia Julien <freexplorer@virtualblackfox.net>
 * 
 * Ce programme est libre, vous pouvez le redistribuer et/ou le modifier selon les 
 * termes de la Licence Publique Générale GNU publiée par la Free Software 
 * Foundation (version 2 ou bien toute autre version ultérieure choisie par vous).
 * 
 * Ce programme est distribué car potentiellement utile, mais SANS AUCUNE GARANTIE, 
 * ni explicite ni implicite, y compris les garanties de commercialisation ou d'adaptation 
 * dans un but spécifique. Reportez-vous à la Licence Publique Générale GNU pour 
 * plus de détails.
 * 
 * Vous devez avoir reçu une copie de la Licence Publique Générale GNU en même 
 * temps que ce programme ; si ce n'est pas le cas, écrivez à la Free Software 
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, États-Unis. 
 */

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
