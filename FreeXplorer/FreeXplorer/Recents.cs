/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 *
 * Copyright (C) 2005 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Wizou.FreeXplorer
{
    static class Recents
    {
        static string FilePath
        {
            get
            {
                return Path.Combine(FreeXplorer.ConfigurationFolder, "recents.xml");
            }
        }

        public static void Add(string file)
        {
            XmlTextReader reader = new XmlTextReader(FilePath);
            reader.ReadStartElement("Recents");
            XmlTextWriter writer = new XmlTextWriter(Path.ChangeExtension(FilePath, ".new"), null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("Recents");
            writer.WriteStartElement("MRL");
            writer.WriteAttributeString("date", DateTime.Now.ToString());
            writer.WriteString(file);
            writer.WriteEndElement();
            int counter = 0;
            while (!reader.EOF)
            {
                if (reader.Value != file)
                {
                    writer.WriteNode(reader, false);
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        counter++;
                        if (counter == 25) break;
                    }
                }
                else
                {
                    reader.Read();
                }
            }
            writer.Close();
            reader.Close();
            File.Delete(FilePath);
            File.Move(Path.ChangeExtension(FilePath, ".new"), FilePath);
        }
    }
}
