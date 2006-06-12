/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 *
 * Copyright (C) 2006 Roncaglia Julien <freexplorer@virtualblackfox.net>
 * 
 * Ce programme est libre, vous pouvez le redistribuer et/ou le modifier selon les 
 * termes de la Licence Publique G�n�rale GNU publi�e par la Free Software 
 * Foundation (version 2 ou bien toute autre version ult�rieure choisie par vous).
 * 
 * Ce programme est distribu� car potentiellement utile, mais SANS AUCUNE GARANTIE, 
 * ni explicite ni implicite, y compris les garanties de commercialisation ou d'adaptation 
 * dans un but sp�cifique. Reportez-vous � la Licence Publique G�n�rale GNU pour 
 * plus de d�tails.
 * 
 * Vous devez avoir re�u une copie de la Licence Publique G�n�rale GNU en m�me 
 * temps que ce programme ; si ce n'est pas le cas, �crivez � la Free Software 
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, �tats-Unis. 
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
                        throw new Exception("OS non support� par FreeXplorer");
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
