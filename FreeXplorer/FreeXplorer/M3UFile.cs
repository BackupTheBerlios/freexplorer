/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 * Copyright (C) 2005 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Wizou.VLC
{
    class M3UFile : StreamWriter
    {
        private string name = null;
        public string Name
        {
            get
            {
                return name;
            }
        }


        public M3UFile() // cree la playlist avec un nom temporaire
            : this(Path.GetTempFileName())
        { }

        public M3UFile(string path)
            : base(path, false, Encoding.Default)
        {
            name = path;
            WriteLine("#EXTM3U");
        }

        public void AppendMedia(string media)
        {
            Write("#EXTINF:0,");
            WriteLine(Path.GetFileName(media));
            string[] MRLOptions = Utility.GetMediaMRLOptions(media);
            if (MRLOptions != null)
                foreach (string MRLOption in MRLOptions)
                {
                    Write("#EXTVLCOPT:");
                    WriteLine(MRLOption);
                }
            WriteLine(Utility.GetMediaMRL(media));
        }
    }
}
