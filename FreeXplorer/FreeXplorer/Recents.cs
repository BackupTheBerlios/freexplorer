/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 *
 * Copyright (C) 2005 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
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
            XmlDocument doc = new XmlDocument();
            doc.Load(FilePath);
            XmlElement root = doc.DocumentElement;
            XmlElement node;
            while ((node = root.SelectSingleNode("MRL[.='" + file + "']") as XmlElement) != null)
            {
                root.RemoveChild(node);
            }
            node = doc.CreateElement("MRL");
            node.SetAttribute("date", DateTime.Now.ToString());
            node.InnerText = file;
            root.InsertAfter(node, null);
            while (root.ChildNodes.Count > 25)
            {
                root.RemoveChild(root.LastChild);
            }
            doc.Save(FilePath);
        }
    }
}
