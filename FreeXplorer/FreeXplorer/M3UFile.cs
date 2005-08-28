/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 * Copyright (C) 2005 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
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
