/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 * Copyright (C) 2005 Olivier Marcoux (wiz0u@free.fr / http://wiz0u.free.fr/freexplorer)
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
using Org.Mentalis.Utilities;
using Wizou.VLC;
using System.Net;
using System.Threading;
using System.IO;
using Wizou.HTTP;
using System.Net.Mime;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Specialized;
using System.Xml;
using System.Web;
using System.Globalization;
using System.Diagnostics;

namespace Wizou.FreeXplorer
{
    class FreeboxServer : BasicHttpServer
    {
        private LIRC.LIRCServer lircServer;
        public VLCApp vlcApp;
        public VLCCache vlcCache;
        private IPAddress FreeboxAddress;
        private StringDictionary GlobalVars; // variables globales utilis�es par les pages (conserv�es d'un appel � l'autre au serveur)

        public FreeboxServer(string baseDir, IPAddress freeboxAddress, VLCApp vlcApp, LIRC.LIRCServer lircServer)
            : base(baseDir, 8080)
        {
            HackStatusCodeAlwaysOK = true; // la Freebox ne reagit pas lorsqu'on envoie des StatusCode d'erreur, donc toujours envoyer OK � la Freebox
            this.FreeboxAddress = freeboxAddress;
            this.vlcApp = vlcApp;
            this.vlcCache = new VLCCache(vlcApp);
            this.lircServer = lircServer;
            GlobalVars = new StringDictionary();
            GlobalVars["_file"] = "C:\\";
        }

        override protected HttpStatusCode HandleRequest()
        {
            // verification de l'adresse IP du client :
            // les adresses accept�s sont:
            //  212.27.38.253 (freeplayer.freebox.fr) : Freebox en mode DHCP
            //  127.0.0.1 (localhost) : Pour tests en local
            //  192.168.xx.xx : Freebox en mode Routeur (pas trop de probleme de s�curit� � ce niveau puisque ce sont des adresses LAN locales, pas Internet)
            IPAddress remoteAddr = (Socket.RemoteEndPoint as IPEndPoint).Address;
            if (!remoteAddr.Equals(FreeboxAddress) && !IPAddress.IsLoopback(remoteAddr))
            {
                byte[] remoteAddrBytes = remoteAddr.GetAddressBytes();
                if ((remoteAddrBytes[0] != 192) || (remoteAddrBytes[1] != 168))
                {
                    throw new Exception(string.Format("Adresse IP {0} non autoris�e", ((IPEndPoint)Socket.RemoteEndPoint).Address));
                }
            }

            if (Url[0] != '/') throw new Exception("L'URL doit commencer par un /");

            int index;
            if (Url.StartsWith("/$$/"))
            {
                Url = Url.Substring(3);
                if (Host.Contains(" "))
                { // on vient d'un site Internet, il faut passer par la case "/settings.html" pour rectifier le "Host:" cible
                    return ReplyHtmlFile("/settings.html");
                }
            }
            else if ((index = Host.IndexOf(' ')) >= 0) // indique que la freebox veut obtenir une page Internet
            {
                Host = Host.Substring(index + 1);
                return HandleInternetRequest();
            }
            
            vlcCache.Invalidate();

            // enregistre les nouvelles valeurs des variables globales donn�es sur la requete
            foreach (string argname in QueryArgs)
                if (argname[0] == '_')
                    GlobalVars[argname] = QueryArgs[argname];

            // effectue les actions demand�es par la requete
            string[] actions = QueryArgs.GetValues("action");
            if (actions != null)
                DoActions(actions);

            // cas des URL variable : on obtient le nom de l'URL via GetHTMLArgument
            if ((Url.Length > 2) && (Url[1] == '$'))
                Url = (string)Evaluate(Url.Substring(1));

            string path = Path.GetFullPath(Path.Combine(BaseDir, Url.Substring(1)));
            string extension = Path.GetExtension(path);
            if ((extension != ".htm") && (extension != ".html") && (extension != ".gif") && (extension != ".xsl"))
            {
                ErrorDescription = "Extension de fichier " + extension + " non support�e";
                return HttpStatusCode.UnsupportedMediaType;
            }
            if (!File.Exists(path))
            {
                ErrorDescription = "Page " + Url + " introuvable";
                return HttpStatusCode.NotFound;
            }

            if (extension == ".xsl")
            {
                LastModified = File.GetLastWriteTime(path);
                return ReplyXSLT(path, QueryArgs["xml"]);
            }
            else if ((extension != ".htm") && (extension != ".html"))
                return base.HandleRequest();
            else
                return ReplyHtmlFile(Url);
        }

        override protected void HandleAfterReply()
        { // action a effectuer apr�s avoir r�pondu la page html
            if (vlcApp.Crashed())
            {
                vlcApp.Stop();
                vlcApp.Start();
            }
            string[] afterActions = QueryArgs.GetValues("afterAction");
            if (afterActions != null)
                foreach (string afterAction in afterActions)
                    DoAfterAction(afterAction);
            base.HandleAfterReply();
        }

        private HttpStatusCode ReplyXSLT(string xslPath, string xmlName)
        {
            string xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/" + xmlName);
            if (!File.Exists(xslPath))
            {
                ErrorDescription = "Donn�es " + xmlName + " introuvable";
                return HttpStatusCode.NotFound;
            }
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xslPath);
            XPathDocument xpathdocument = new XPathDocument(xmlPath);
            ContentType = new ContentType("text/html");
            Content = new MemoryStream(8192);
            xslt.Transform(xpathdocument, null, Content);
            return HttpStatusCode.OK;
        }

        private HttpStatusCode ReplyHtmlFile(string name)
        {
            string argpath = Path.ChangeExtension(Path.GetFullPath(Path.Combine(BaseDir, '.' + name)), ".arg");
            if (!File.Exists(argpath))
                return base.HandleRequest();

            // pages HTML avec fichier .arg associ� :
            string[] argExpressions = File.ReadAllLines(argpath);
            object[] args = new object[argExpressions.Length+1];
            args[0] = "";
            int index = 1;
            foreach (string expression in argExpressions)
                args[index++] = Evaluate(expression);
            return ReplyFormattedFile(name, args);
        }

        #region Traitements des actions et fonctions
        protected object Evaluate(string expression)
        {
            if (expression[0] == '_')
                return GlobalVars[expression];
            else if (expression[0] != '$')
                return QueryArgs[expression];

            // l'expression commence par $, c'est une fonction :
            string param;
            int index = expression.IndexOf(' ');
            if (index >= 0) // y a t-il un parametre � evaluer ?
            {
                param = (string)Evaluate(expression.Substring(index + 1));
                expression = expression.Substring(0, index);
            }
            else
                param = "";
            switch (expression.ToLower())
            {
                case "$lirc":
                    return lircServer.Connections;

                case "$explore": // renvoit l'URL a utiliser suivant le contenu de la variable globale _file
                    if (param.Length == 0) param = GlobalVars["_file"]; //TODO: voir si on peut se passer de cette ligne genre href="$explore _file"... histoire de ne plus mettre _file en dur
                    if (param.Contains("://")) // une MRL
                        return "/explore_folders.html";
                    else if (param.EndsWith("/*"))
                        return "/explore_files.html";
                    else if (param[0] == '$')
                        return "/explore_folders.html";
                    else try
                        {
                            if ((File.GetAttributes(param) & FileAttributes.Directory) == 0)
                                return "/explore_files.html";
                            else
                                return "/explore_folders.html";
                        }
                        catch (IOException)
                        {
                            return "/explore_folders.html";
                        }

                case "$folders": // HTML repr�sentant la liste des dossiers avec comme dossier actif celui d�sign� la variable globale _file
                    param = vlcApp.PathFromMRL(Helper.ExpandSpecialFolder(param));
                    StringBuilder html = new StringBuilder(10000);
                    Helper.ExploreFolders(ref html, Directory.GetLogicalDrives(), param, 1, "");
                    return html.ToString();


                case "$files": // HTML repr�sentant la liste des fichiers du repertoire contenant le(s) fichier(s) d�sign�(s) par la variable globale _file
                    string dir = Helper.ExpandSpecialFolder(Path.GetDirectoryName(param));
                    param = Path.GetFileName(param);
                    return Helper.ExploreFiles(dir, param);


                case "$fullurl": // retourne URL+Query (� l'exception de /settings.html qui devient /home.html)
                    param = Url + Query;
                    return (param == "/settings.html") ? "/home.html" : param;

                case "$dir": // repertoire contenant le fichier d�sign� par la variable globale _file
                    return Helper.ExpandSpecialFolder(Path.GetDirectoryName(param));

                case "$url": // version du parametre pour �tre plac� dans une URL
                    return HttpUtility.UrlEncode(param);

                case "$audio_mode": // mode audio dans lequel placer la Freebox
                    return Freebox.AudioTranscode2MetaAud(vlcApp.AudioTranscode);

                case "$version": // version de FreeXplorer
                    Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    return appVersion.ToString(appVersion.Build == 0 ? 2 : 3);

                // propri�t�s VLC directes
                case "$vlc_chapter":
                    return vlcCache.Chapter + 1;
                case "$vlc_chapter_max":
                    return vlcCache.ChapterMax;
                case "$vlc_title":
                    return vlcCache.Title + 1;
                case "$vlc_title_max":
                    return vlcCache.TitleMax;
                case "$vlc_caption":
                    return vlcCache.Caption;
                case "$vlc_duration_time":
                    return new TimeSpan(0, 0, vlcCache.DurationTime);
                case "$vlc_elapsed_time":
                    return new TimeSpan(0, 0, vlcCache.ElapsedTime);

                // propri�t�s VLC calcul�es
                case "$vlc_elapsed_less50%":
                    return (vlcCache.ElapsedTime > vlcCache.DurationTime / 2) ? null : (object)new TimeSpan(0, 0, vlcCache.ElapsedTime);
                case "$vlc_elapsed_more50%":
                    return (vlcCache.ElapsedTime > vlcCache.DurationTime / 2) ? (object)new TimeSpan(0, 0, vlcCache.ElapsedTime) : null;
                case "$vlc_elapsed_percent":
                    return (vlcCache.DurationTime == 0) ? 0 : vlcCache.ElapsedTime * 100 / vlcCache.DurationTime;
                case "$vlc_chapter_percent":
                    return vlcCache.ChapterMax == 0 ? 0 : vlcCache.Chapter * 100 / vlcCache.ChapterMax;
                case "$vlc_title_percent":
                    return vlcCache.TitleMax == 0 ? 0 : vlcCache.Title * 100 / vlcCache.TitleMax;

                case "$nice_caption":
                    if (param.StartsWith("dvdsimple://"))
                    {
                        param = "DVD";
                        if (vlcCache.TitleMax > 1) param += " - Titre " + (vlcCache.Title + 1) + "/" + vlcCache.TitleMax;
                        param += " - Chapitre " + (vlcCache.Chapter + 1) + "/" + vlcCache.ChapterMax;
                    }
                    else if (!param.Contains("/")) // si ce n'est pas une MRL, c'est un fichier, dans ce cas on garde juste son nom
                        param = Path.GetFileName(param);
                    return param;

                default:
                    throw new Exception("Evaluate invalide expression: " + expression);
            }
        }

        private void DoActions(string[] actions)
        {
            bool waitForBkgnd = false;
            foreach (string action in actions)
            {
                string filename;
                int temp;
                switch (action)
                {
                    case "sendkeys":
                        System.Windows.Forms.SendKeys.SendWait(QueryArgs["keys"]);
                        break;
                    case "appcommand":
                        IntPtr fWindow = SysWin32.GetForegroundWindow();
                        if (fWindow != IntPtr.Zero)
                        {
                            foreach (string key in QueryArgs.GetValues("cmd"))
                                SysWin32.SendMessage(fWindow, SysWin32.WM_APPCOMMAND, fWindow, Convert.ToInt32(key) << 16);
                        }
                        break;
                    case "lirc":
                        lircServer.KeyPressed(QueryArgs["key"]);
                        break;
                    case "favadd":
                        Helper.FavoritesAdd(QueryArgs["file"], QueryArgs["kind"], QueryArgs["title"]);
                        break;
                    case "favdel":
                        Helper.FavoritesDel(QueryArgs["file"]);
                        break;
                    case "bkgnd":
                        filename = QueryArgs["bkgnd"];
                        if (filename.StartsWith("http://"))
                        {
                            WebRequest webRequest = WebRequest.Create(filename);
                            WebResponse webResponse = webRequest.GetResponse();
                            Stream responseStream = webResponse.GetResponseStream();
                            long contentLength = webResponse.ContentLength;
                            if (contentLength == -1) throw new Exception("Le site web n'indique pas de Content-Length pour cette image");
                            filename = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(webRequest.RequestUri.AbsolutePath));
                            FileStream tempFile = File.Create(filename, 4096);
                            byte[] buffer = new byte[Math.Min(contentLength, 4096L)];
                            while (contentLength > 0)
                            {
                                temp = responseStream.Read(buffer, 0, buffer.Length);
                                if (temp == 0)
                                    break;
                                tempFile.Write(buffer, 0, temp);
                                contentLength -= temp;
                            }
                            tempFile.Close();
                            responseStream.Close();
                        }
                        vlcApp.Play(filename, 0);
                        vlcCache.Invalidate();
                        waitForBkgnd = true;
                        break;
                    case "add":
                        if (waitForBkgnd) Thread.Sleep(1500);
                        vlcApp.Command("stop");
                        vlcApp.Command("play");
                        filename = GlobalVars["_file"];
                        Helper.AddToRecents(filename);
                        if (filename.EndsWith("/*"))
                            vlcApp.Play(Helper.GetPlayableFilesInDir(Path.GetDirectoryName(filename)));
                        else
                            vlcApp.Play(filename, 0); // 0 pour que ca marche aussi avec les playlist
                        vlcCache.Invalidate();
                        Thread.Sleep(700);
                        break;
                    case "play":
                        vlcApp.Command("play");
                        Thread.Sleep(500);
                        break;
                    case "rewind":
                        if ((vlcCache.ChapterMax > 1) && (vlcCache.Chapter > 0))
                        {
                            GlobalVars["_action"] = string.Format("Chapitre {0}/{1}", vlcCache.Chapter, vlcCache.ChapterMax);
                            vlcApp.Command("chapter_p");
                            vlcCache.Invalidate(VLCCache.CachedFlags.ChapterInfo | VLCCache.CachedFlags.ElapsedTime);
                        }
                        else if ((vlcCache.TitleMax > 1) && (vlcCache.Title > 0))
                        {
                            GlobalVars["_action"] = string.Format("Titre {0}/{1}", vlcCache.Title, vlcCache.TitleMax);
                            vlcApp.Command("title_p");
                            vlcCache.Invalidate(VLCCache.CachedFlags.TitleInfo | VLCCache.CachedFlags.ElapsedTime);
                        }
                        else
                        {
                            GlobalVars["_action"] = "Saut en arri�re";
                            temp = (vlcCache.DurationTime < 480) ? 15 : 60; // les step sont de 15 sec pour les media de moins de 8 min, sinon 60 sec
                            vlcApp.Command("seek " + (vlcCache.ElapsedTime - temp));
                            vlcCache.Invalidate(VLCCache.CachedFlags.ElapsedTime);
                        }
                        Thread.Sleep(800); // laisse le temps � VLC d'effectuer l'action pour que la mise � jour des infos soit correcte
                        break;
                    case "forward":
                        if ((vlcCache.ChapterMax > 1) && (vlcCache.Chapter + 1 < vlcCache.ChapterMax))
                        {
                            GlobalVars["_action"] = string.Format("Chapitre {0}/{1}", vlcCache.Chapter + 2, vlcCache.ChapterMax);
                            vlcApp.Command("chapter_n");
                            vlcCache.Invalidate(VLCCache.CachedFlags.ChapterInfo | VLCCache.CachedFlags.ElapsedTime);
                        }
                        else if ((vlcCache.TitleMax > 1) && (vlcCache.Title + 1 < vlcCache.TitleMax))
                        {
                            GlobalVars["_action"] = string.Format("Titre {0}/{1}", vlcCache.Title + 2, vlcCache.TitleMax);
                            vlcApp.Command("title_n");
                            vlcCache.Invalidate(VLCCache.CachedFlags.TitleInfo | VLCCache.CachedFlags.ElapsedTime);
                        }
                        else
                        {
                            GlobalVars["_action"] = "Saut en avant";
                            temp = (vlcCache.DurationTime < 480) ? 15 : 60; // les step sont de 15 sec pour les media de moins de 8 min, sinon 60 sec
                            vlcApp.Command("seek " + (vlcCache.ElapsedTime + temp));
                            vlcCache.Invalidate(VLCCache.CachedFlags.ElapsedTime);
                        }
                        Thread.Sleep(100); // laisse le temps � VLC d'effectuer l'action pour que la mise � jour des infos soit correcte
                        break;
                    case "plprev":
                        vlcApp.Command("play");
                        vlcApp.PlaylistPrev();
                        Thread.Sleep(1500);
                        break;
                    case "plnext":
                        vlcApp.Command("play");
                        vlcApp.PlaylistNext();
                        Thread.Sleep(1500);
                        break;
                    case "pause":
                    case "stop":
                        vlcApp.Command(action);
                        Thread.Sleep(100);
                        break;
                    default:
                        Console.WriteLine("Action inconnue: " + action);
                        break;
                }
            }
        }

        private void DoAfterAction(string afterAction)
        {
            switch (afterAction)
            {
                case "suspend":
                    WindowsController.ExitWindows(RestartOptions.Suspend, false);
                    break;
                case "poweroff":
                    WindowsController.ExitWindows(RestartOptions.PowerOff, false);
                    break;
                case "reboot":
                    WindowsController.ExitWindows(RestartOptions.Reboot, false);
                    break;
            }
        }        
        #endregion

        #region Traitements pour les pages Internet

        private HttpStatusCode HandleInternetRequest()
        {
            /*
             line = originURL.Replace("&amp;", "&") + '&';
                    scan = -1;
                    int scanend;
                    while ((scan = line.IndexOf("$(", scan + 1)) >= 0)
                    {
                        scanend = line.IndexOf(')', scan + 2);
                        if (scanend >= 0)
                        {
                            string var = line.Substring(scan + 2, scanend - scan - 2);
                            scanend = line.IndexOf("&(" + var + ")=", scanend);
                            if (scanend >= 0)
                            {
                                int valueend = line.IndexOf('&', scanend + 4);
                                string value = line.Substring(scanend + var.Length + 4, valueend - scanend - var.Length - 4);
                                line = line.Remove(scanend, valueend - scanend);
                                line = line.Replace("$(" + var + ")", value);
                            }
                        }
                    }
             */
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create("http://" + Host + Url + Query);
            webRequest.Timeout = 10000;
            webRequest.Method = HttpMethod;
            webRequest.ProtocolVersion = ProtocolVersion;
            webRequest.Accept = Accept;
            if (RequestContentLength >= 0)
                webRequest.ContentLength = RequestContentLength;
            if (RequestContentType != null)
                webRequest.ContentType = RequestContentType.ToString();
            // remplir webRequest.GetRequestStream() quand la Freebox saura traiter les requetes avec contenu
            //protected string Host;
            if (IfModifiedSince != new DateTime())
                webRequest.IfModifiedSince = IfModifiedSince;
            int index = Referer.IndexOf("212.27.38.254 ");
            if (index >= 0)
                webRequest.Referer = Referer.Remove(index, 14).Replace(":8080", "");
            webRequest.UserAgent = UserAgent;
            webRequest.Headers.Add(RequestHeaders);
            HttpWebResponse webResponse;
#if DEBUG // TODO retirer lorsque j'aurais fini de travailler sur les sites Internet
            try
            {
#endif
                webResponse = (HttpWebResponse)webRequest.GetResponse();
#if DEBUG
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message, e);
            }
#endif
            ContentType = new ContentType(webResponse.ContentType);
            LastModified = webResponse.LastModified;
            ResponseHeaders = webResponse.Headers;
            ContentLength = webResponse.ContentLength;
            Content = webResponse.GetResponseStream();

            if (ContentType.MediaType == "text/html")
            {
                HandleInternetResponseHTML();
            } 
            else if (ContentType.MediaType == "text/vnd.wap.wml")
            {
                HandleInternetResponseWML();
            }
            return webResponse.StatusCode;
        }

        private void HandleInternetResponseWML()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(Path.Combine(BaseDir, "wml.xsl"));
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.None;
            XmlReader wmlDoc = XmlReader.Create(Content, settings);
            Content.Close();
            Content = null;
            StringWriter htmlWriter = new StringWriter();
            try
            {
                xslt.Transform(wmlDoc, null, htmlWriter);
            }
            catch (XmlException e)
            {
                throw new ApplicationException("Erreur dans la conversion WML vers HTML: " + e.Message, e);
            }
            string html = htmlWriter.ToString();
            html = ArrangeHTMLLinks(html);
            ContentType.MediaType = "text/html";
#if DEBUG
            Console.WriteLine("APRES: " + html);
#endif
            ReplyString(html);
        }

        private void HandleInternetResponseHTML()
        {
            if (ContentLength == -1) ContentLength = 16384;
            byte[] bytes = new byte[ContentLength];
            ContentLength = Content.Read(bytes, 0, (int) ContentLength);
            Content.Close();
            Content = null;
            string html = Encoding.Default.GetString(bytes);
#if DEBUG
            Console.WriteLine("AVANT: "+html);
#endif

            if (html.Substring(0, 5).ToLower() == "<wml>")
            { // pour les sites qui renvoient du WML avec text/html
                Content = new MemoryStream(bytes);
                HandleInternetResponseWML();
                return;
            }

            int scan = CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "<body", CompareOptions.IgnoreCase);
            if (scan >= 0)
            {
                scan = html.IndexOf('>', scan + 5);
                if (scan >= 0)
                {
                    html = html.Insert(scan, " bgcolor=#F0F0F030 text=#0000003F link=#0000FF3F alink=#FF00003F vlink=#FF00FF3F>"+
                        "<link rel=\"yellow\" href=\"back\"/><hr><table><tr><td width=3%>&nbsp;</td><td width=92%");
                    scan = CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "</body>", CompareOptions.IgnoreCase);
                    if (scan >= 0)
                    {
                        html = html.Insert(scan, "</td><td width=3%>&nbsp;</td></tr></table>");
                    }
                }
            }
            scan = -1;
            int scanend;
            while ((scan = CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "<style", scan + 1, CompareOptions.IgnoreCase)) >= 0)
            {
                scanend = CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "</style>", scan + 6, CompareOptions.IgnoreCase);
                if (scanend < 0) break;
                html = html.Remove(scan, scanend + 8 - scan);
            }
            html = ArrangeHTMLLinks(html);
#if DEBUG
            Console.WriteLine("APRES: " + html);
#endif
            ReplyString(html);
        }

        private string ArrangeHTMLLinks(string html)
        {
            int scanend;
            int scan = -1;
            while ((scan = CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "http://", scan + 1, CompareOptions.IgnoreCase)) >= 0)
            {
                scanend = html.IndexOfAny(new char[] { '/', '"' }, scan + 7);
                if (scanend < 0) break;
                html = html.Substring(0, scan) + "http://212.27.38.254 " + html.Substring(scan + 7, scanend - scan - 7) + ":8080" + html.Substring(scanend);
                scan = scanend + 18;
            }
            return html;
        }
        #endregion
    }

    static class Freebox
    {
        public static string AudioTranscode2MetaAud(AudioTranscode audioTranscode)
        {
            switch (audioTranscode)
            {
                case AudioTranscode.A52: return "69(en,ac3)";
                case AudioTranscode.MPGA:
                default: return "69(en,mp2)";
            }
        }
    }

    static class Helper
    {
        internal static void FavoritesAdd(string file, string kind, string title)
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

        internal static void FavoritesDel(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/favorites.xml"));
            XmlElement root = doc.DocumentElement;
            XmlElement node = root.SelectSingleNode("MRL[.='" + file + "']") as XmlElement;
            if (node != null) root.RemoveChild(node);
            doc.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/favorites.xml"));

        }

        internal static void AddToRecents(string file)
        {
            XmlTextReader reader = new XmlTextReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/recents.xml"));
            reader.ReadStartElement("Recents");
            XmlTextWriter writer = new XmlTextWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/recents.new"), null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("Recents");
            writer.WriteStartElement("MRL");
            writer.WriteAttributeString("date", DateTime.Now.ToString());
            writer.WriteString(file);
            writer.WriteEndElement();
            int counter = 0;
            while (!reader.EOF)
            {
                writer.WriteNode(reader, false);
                if (reader.NodeType == XmlNodeType.Element)
                {
                    counter++;
                    if (counter == 25) break;
                }
            }
            writer.Close();
            reader.Close();
            File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/recents.xml"));
            File.Move(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/recents.new"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/recents.xml"));
        }

        internal static StringCollection GetPlayableFilesInDir(string path)
        {
            StringCollection result = new StringCollection();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                VLC.Utility.MediaType mediaType = VLC.Utility.GetMediaType(file);
                if ((mediaType != VLC.Utility.MediaType.Unknown) && (mediaType != VLC.Utility.MediaType.Playlist))
                    result.Add(file);
            }
            return result;
        }

        internal static string ExploreFiles(string dir, string deffile)
        {
            string[] files;
            try
            {
                files = Directory.GetFiles(dir);
            }
            catch (IOException e)
            {
                return "<i>" + e.Message + "</i>";
            }
            StringBuilder html = new StringBuilder(256 * files.Length); // chaque ligne prend environ 225 caract�res
            foreach (string file in files)
            {
                VLC.Utility.MediaType mediaType = VLC.Utility.GetMediaType(file);
                if (mediaType == VLC.Utility.MediaType.Unknown)
                    continue; // skip unknown type files
                html.Append("<img src=");
                html.Append("img/mediafile.gif"); // TODO: mettre le bon icone suivant le mediaType
                html.Append(" width=16 height=17>&nbsp;<a href=\"play.html?action=add&_file=");
                html.Append(HttpUtility.UrlEncode(file));
                html.Append("&param=Lecture+du+fichier...\"");
                if (Path.GetFileName(file) == deffile)
                    html.Append(" focused");
                html.Append(">");
                html.Append(Path.GetFileName(file));
                html.Append("</a><br>\r\n");
            }
            if (html.Length == 0)
                return "<i>Aucun fichier multimedia dans ce dossier</i>\r\n";
            else
                return html.ToString();
        }

        internal static void ExploreFolders(ref StringBuilder html, string[] folders, string dir, int level, string prefix)
        {
            foreach (string path in folders)
            {
                string folder = path.Remove(0, prefix.Length);
                if (folder == "A:\\") continue; // on n'affiche pas les lecteurs de disquettes quand meme... tss...
                string[] subdirs = { };
                if (level > 1)
                {
                    if ((File.GetAttributes(path) & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                        continue;
                }
                for (int i = 0; i < level; i++)
                    html.Append("&nbsp;&nbsp;&nbsp;");
                html.Append("<img src=");
                if (level == 1)
                {
                    folder = folder.TrimEnd(Path.DirectorySeparatorChar);
                    string volumeName, fileSystemName;
                    int dummy;
                    int iResult = SysWin32.GetVolumeInformation(path, out volumeName, out dummy, out dummy, out dummy, out fileSystemName);
                    if (fileSystemName == "CDFS")
                        html.Append("img/cddvd.gif");
                    else
                        html.Append("img/harddisk.gif");
                    if (volumeName.Length != 0)
                        folder = volumeName + " (" + folder + ")";
                    
                    html.Append(" width=16 height=17>&nbsp;");
                    if (iResult != 0)
                    try
                    {
                        subdirs = Directory.GetDirectories(path);
                    }
                    catch (IOException)
                    {
                    }
                }
                else
                {
                    html.Append("img/folder.gif");
                    html.Append(" width=16 height=17>&nbsp;");
                    try
                    {
                        subdirs = Directory.GetDirectories(path);
                    }
                    catch (IOException)
                    {
                    }
                }
                if (path == dir)
                {
                    html.Append("<a href=\"");
                    html.Append("$explore?");
                    html.Append("_file=");
                    html.Append(HttpUtility.UrlEncode(path.TrimEnd(Path.DirectorySeparatorChar) + "/*"));
                    html.Append("\" focused><font color=\"#006F003F\">");
                    html.Append(folder);
                    html.Append(" &nbsp; <i>&gt;&gt; Voir les fichiers</i></font></a><br>\r\n");
                }
                else
                {
                    html.Append("<a href=\"");
                    if (subdirs.Length == 0)
                    {
                        if (folder == "VIDEO_TS")
                        {
                            html.Append("play.html?action=add&_file=");
                            html.Append(HttpUtility.UrlEncode("dvdsimple://" + path));
                            html.Append("&param=");
                            html.Append(HttpUtility.UrlEncode("Lecture+du+DVD..."));
                        }
                        else
                        {
                            html.Append("$explore?_file=");
                            html.Append(HttpUtility.UrlEncode(path.TrimEnd(Path.DirectorySeparatorChar) + "/*"));
                        }
                    }
                    else
                    {
                        html.Append("$explore?_file=");
                        html.Append(HttpUtility.UrlEncode(path));
                    }
                    html.Append("\">");
                    html.Append(folder);
                    html.Append("</a><br>\r\n");
                }
                if (dir.StartsWith(path))
                {
                    ExploreFolders(ref html, subdirs, dir, level + 1, level == 1 ? path : path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
                }
            }
        }

        internal static string ExpandSpecialFolder(string folder)
        {
            if (folder[0] != '$')
                return folder;
            switch (folder.ToLower())
            {
                case "$personal": return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                case "$my music": return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); 
                case "$my pictures": return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); 
                case "$my video": return (string)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders",
                                        "My Video", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); 
                default: throw new Exception("Dossier special inconnu: " + folder);
            }
        }

    }
}