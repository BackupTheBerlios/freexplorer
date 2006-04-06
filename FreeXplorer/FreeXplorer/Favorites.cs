using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Wizou.FreeXplorer
{
    static class Favorites
    {
        internal static void Add(string file, string kind, string title)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/favorites.xml"));
            XmlElement root = doc.DocumentElement;
            XmlElement node = root.SelectSingleNode("MRL[.='" + file + "']") as XmlElement;
            if (node == null)
            {
                node = doc.CreateElement("MRL");
                node.InnerText = file;
                root.AppendChild(node);
            }
            node.SetAttribute("kind", kind);
            node.SetAttribute("title", title);
            doc.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/favorites.xml"));

        }

        internal static void Del(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/favorites.xml"));
            XmlElement root = doc.DocumentElement;
            XmlElement node = root.SelectSingleNode("MRL[.='" + file + "']") as XmlElement;
            if (node != null) root.RemoveChild(node);
            doc.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/favorites.xml"));

        }
    }
}
