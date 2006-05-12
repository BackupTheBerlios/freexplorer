/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 *
 * Copyright (C) 2005-2006 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using ImageManipulation;

namespace Wizou.FreeXplorer
{
    class FreeboxServer : BasicHttpServer
    {
        private LIRC.LIRCServer lircServer;
        private VLCApp vlcApp;
        private VLCCache vlcCache;
        public Boolean PCControlAllowed;
        public Boolean BlackBkgnds;
        private IPAddress FreeboxAddress;
        private StringDictionary GlobalVars = new StringDictionary(); // variables globales utilisées par les pages (conservées d'un appel à l'autre au serveur)
        private CookieContainer webCookieContainer;
        string keyboardHTML;
        string keyboardReferer = null;
        string keyboardURL;

        public FreeboxServer(string baseDir)
            : base(baseDir, 8080)
        {
            HackStatusCodeAlwaysOK = true; // la Freebox ne reagit pas lorsqu'on envoie des StatusCode d'erreur, donc toujours envoyer OK à la Freebox
            GlobalVars["_file"] = "C:\\";
            //Opens file "cookies.dat" and deserializes the CookieContainer from it.
            try
            {
                Stream stream = File.Open(Path.Combine(FreeXplorer.ConfigurationFolder, "cookies.dat"), FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                webCookieContainer = (CookieContainer)formatter.Deserialize(stream);
                stream.Close();
            }
            catch
            {
                webCookieContainer = new CookieContainer();
            }

        }

        public void Init(IPAddress freeboxAddress, VLCApp vlcApp, LIRC.LIRCServer lircServer)
        {
            this.FreeboxAddress = freeboxAddress;
            this.vlcApp = vlcApp;
            this.vlcCache = new VLCCache(vlcApp);
            this.lircServer = lircServer;
        }

        override protected bool ValidateRemoteAddr(IPAddress remoteAddr)
        {
            // verification de l'adresse IP du client :
            // les adresses acceptés sont:
            //  212.27.38.253 (freeplayer.freebox.fr) : Freebox en mode DHCP
            //  127.0.0.1 (localhost) : Pour tests en local
            //  192.168.xx.xx : Freebox en mode Routeur (pas trop de probleme de sécurité à ce niveau puisque ce sont des adresses LAN locales, pas Internet)
            if (!remoteAddr.Equals(FreeboxAddress) && !IPAddress.IsLoopback(remoteAddr))
            {
                byte[] remoteAddrBytes = remoteAddr.GetAddressBytes();
                if ((remoteAddrBytes[0] != 192) || (remoteAddrBytes[1] != 168))
                    return false; // non-acceptée
            }
            return base.ValidateRemoteAddr(remoteAddr);
        }

        override protected HttpStatusCode HandleRequest()
        {
            if (Url[0] != '/') throw new Exception("L'URL doit commencer par un /");

            vlcCache.Invalidate();

            int index;
            if (Url == "/$$keyboard")
            {
                HandleKeyboardRequest(QueryArgs["v"], QueryArgs["f"]);
                return HttpStatusCode.OK;
            }
            else if ((Referer != null) && Referer.Contains("$$keyboard") && (keyboardReferer != null))
            {
                Referer = keyboardReferer;
                keyboardReferer = null;
            }

            if (Url.StartsWith("/$$/"))
            {
                Url = Url.Substring(3);
                if (Host.Contains(" ") && !Url.EndsWith(".gif"))
                { // on vient d'un site Internet, il faut passer par la case "/$$back.html" pour rectifier le "Host:" cible
                    return ReplyHtmlFile("/$$back.html");
                }
            }
            else if ((Host != null) && ((index = Host.IndexOf(' ')) >= 0)) // indique que la freebox veut obtenir une page Internet
            {
                Host = Host.Substring(index + 1);
                return HandleInternetRequest();
            }

            HandleSpecialQueryArgs();

            // cas des URL variable : on obtient le nom de l'URL via GetHTMLArgument
            if ((Url.Length > 2) && (Url[1] == '$'))
                Url = Evaluate(Url.Substring(1)).ToString();

            string path = Path.GetFullPath(Path.Combine(BaseDir, Url.Substring(1)));
            if (!File.Exists(path))
            {
                ErrorDescription = "Page " + Url + " introuvable";
                return HttpStatusCode.NotFound;
            }

            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".xsl":
                    LastModified = File.GetLastWriteTime(path);
                    return ReplyXSLT(path, QueryArgs["xml"]);

                case ".htm":
                case ".html":
                    return ReplyHtmlFile(Url);

                case ".gif":
                    return base.HandleRequest();

                default:
                    ErrorDescription = "Extension de fichier " + extension + " non supportée";
                    return HttpStatusCode.UnsupportedMediaType;
            }
        }

        private void HandleSpecialQueryArgs()
        {
            // enregistre les nouvelles valeurs des variables globales données sur la requete
            foreach (string argname in QueryArgs)
                if ((argname != null) && (argname[0] == '_'))
                    GlobalVars[argname] = QueryArgs[argname];

            // effectue les actions demandées par la requete
            string[] actions = QueryArgs.GetValues("action");
            if (actions != null)
                DoActions(actions);
        }

        override protected void HandleAfterReply()
        { // action a effectuer après avoir répondu la page html
            if (vlcApp.Crashed())
            {
                vlcApp.Stop();
                vlcApp.Start();
            }
            if (QueryArgs != null)
            {
                string[] afterActions = QueryArgs.GetValues("afterAction");
                if (afterActions != null)
                    foreach (string afterAction in afterActions)
                        DoAfterAction(afterAction);
            }
            base.HandleAfterReply();
        }

        private HttpStatusCode ReplyXSLT(string xslPath, string xmlName)
        {
            string xmlPath = Path.Combine(FreeXplorer.ConfigurationFolder, xmlName);
            if (!File.Exists(xslPath))
            {
                ErrorDescription = "Données " + xmlName + " introuvable";
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

            // pages HTML avec fichier .arg associé :
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
            if (index >= 0) // y a t-il un parametre à evaluer ?
            {
                param = Evaluate(expression.Substring(index + 1)).ToString();
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

                case "$folders": // HTML représentant la liste des dossiers avec comme dossier actif celui désigné la variable globale _file
                    param = vlcApp.PathFromMRL(Helper.ExpandSpecialFolder(param));
                    StringBuilder html = new StringBuilder(10000);
                    Helper.ExploreFolders(ref html, Directory.GetLogicalDrives(), param, 1, "");
                    return html.ToString();


                case "$files": // HTML représentant la liste des fichiers du repertoire contenant le(s) fichier(s) désigné(s) par la variable globale _file
                    string dir = Helper.ExpandSpecialFolder(Path.GetDirectoryName(param));
                    param = Path.GetFileName(param);
                    return Helper.ExploreFiles(dir, param);


                case "$fullurl": // retourne URL+Query
                    return Url + Query;

                case "$dir": // repertoire contenant le fichier désigné par la variable globale _file
                    return Helper.ExpandSpecialFolder(Path.GetDirectoryName(param));

                case "$url": // version du parametre pour être placé dans une URL
                    return HttpUtility.UrlEncode(param);

                case "$audio_mode": // mode audio dans lequel placer la Freebox
                    return Freebox.AudioTranscode2MetaAud(vlcApp.AudioTranscode);

                case "$pc_control_allowed": // le controle du PC est-il autorisé ?
                    return PCControlAllowed;

                case "$not": // renvoit l'inverse du booleen en paramètre
                    return !Convert.ToBoolean(param);

                case "$comment_if":
                    return Convert.ToBoolean(param) ? "<!-- " : "";
                case "$end_comment_if":
                    return Convert.ToBoolean(param) ? " -->" : "";

                case "$version": // version de FreeXplorer
                    return Program.appVersionText;

                // propriétés VLC directes
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

                // propriétés VLC calculées
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
                        if (PCControlAllowed) System.Windows.Forms.SendKeys.SendWait(QueryArgs["keys"]);
                        break;
                    case "appcommand":
                        IntPtr fWindow = SysWin32.GetForegroundWindow();
                        if (PCControlAllowed && (fWindow != IntPtr.Zero))
                        {
                            foreach (string key in QueryArgs.GetValues("cmd"))
                                SysWin32.SendMessage(fWindow, SysWin32.WM_APPCOMMAND, fWindow, Convert.ToInt32(key) << 16);
                        }
                        break;
                    case "lirc":
                        if (PCControlAllowed) lircServer.KeyPressed(QueryArgs["key"]);
                        break;
                    case "favadd":
                        Favorites.Add(QueryArgs["file"], QueryArgs["kind"], QueryArgs["title"]);
                        break;
                    case "favdel":
                        Favorites.Del(QueryArgs["file"]);
                        break;
                    case "bkgnd":
                        if (BlackBkgnds)
                            filename = Path.Combine(BaseDir, "img"+Path.PathSeparator+"black.png");
                        else
                            filename = QueryArgs["bkgnd"];
                        if (filename.StartsWith("http://"))
                        {
                            WebRequest webRequest = WebRequest.Create(filename);
                            WebResponse webResponse;
                            try
                            {
                                webResponse = webRequest.GetResponse();
                            }
                            catch (WebException)
                            {
                                break;
                            }
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
                        if (waitForBkgnd) Thread.Sleep(2000);
                        vlcApp.Command("stop");
                        vlcApp.Command("play");
                        filename = GlobalVars["_file"];
                        Recents.Add(filename);
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
                            GlobalVars["_action"] = "Saut en arrière";
                            temp = (vlcCache.DurationTime < 480) ? 15 : 60; // les step sont de 15 sec pour les media de moins de 8 min, sinon 60 sec
                            vlcApp.Command("seek " + (vlcCache.ElapsedTime - temp));
                            vlcCache.Invalidate(VLCCache.CachedFlags.ElapsedTime);
                        }
                        Thread.Sleep(800); // laisse le temps à VLC d'effectuer l'action pour que la mise à jour des infos soit correcte
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
                        Thread.Sleep(100); // laisse le temps à VLC d'effectuer l'action pour que la mise à jour des infos soit correcte
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
                    if (PCControlAllowed) WindowsController.ExitWindows(RestartOptions.Suspend, false);
                    break;
                case "poweroff":
                    if (PCControlAllowed) WindowsController.ExitWindows(RestartOptions.PowerOff, false);
                    break;
                case "reboot":
                    if (PCControlAllowed) WindowsController.ExitWindows(RestartOptions.Reboot, false);
                    break;
            }
        }        
        #endregion

        #region Traitements pour les pages Internet

        private HttpStatusCode HandleInternetRequest()
        {
            bool freexplorerAware = false;
            // la boucle suivant réarrange les paramètres de l'URL suivant les regles suivantes :
            // dans la valeur de chaque paramètre, $(XX) est remplacé par la valeur du paramètre (XX)
            // chaque paramètre dont le nom est entre parenthese est retiré
            // cela permet de supporter le format bizarre des cartes WML avec input
            int index;
            for (index = 0; index < QueryArgs.Count; index++)
            {
                string name = QueryArgs.GetKey(index);
                if (name == null) continue;
                if ((name[0] == '(') && name[name.Length - 1] == ')')
                {
                    QueryArgs.Remove(name);
                    index--;
                    if (name == "($$freexplorer)") // l'argument ($$freexplorer) present dans l'URL
                    {
                        HandleSpecialQueryArgs();   // indique de supporter les action= et variables comme en local
                        freexplorerAware = true;
                    }
                }
                else
                {
                    string arg = QueryArgs[name];
                    int scan = arg.IndexOf("$(");
                    if (scan >= 0)
                    {
                        int scanend = arg.IndexOf(')', scan + 2);
                        if (scanend >= 0)
                        {
                            string valname = arg.Substring(scan + 1, scanend - scan);
                            QueryArgs[name] = arg.Replace("$" + valname, QueryArgs[valname]);
                        }
                    }
                }
            }
            Query = QueryArgs.HasKeys() ? "?" + Wizou.HTTP.Utility.ToString(QueryArgs) : ""; // on reforme la chaine Query
            if (Url.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase))
            {
                GlobalVars["_file"] = "http://" + Host + Url + Query;
                DoActions(new string[] { "add" });
                return HttpStatusCode.OK;
            }

            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create("http://" + Host + Url + Query);
            webRequest.CookieContainer = webCookieContainer;
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
            index = Referer.IndexOf("212.27.38.254 ");
            if (index >= 0)
                webRequest.Referer = Referer.Remove(index, 14).Replace(":8080", "");
            webRequest.UserAgent = UserAgent + " FreeXplorer/" + Program.appVersionText;
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
            //Opens "cookies.dat" and serializes the CookieContainer into it in Soap/XML format.
            Stream stream = File.Open(Path.Combine(FreeXplorer.ConfigurationFolder, "cookies.dat"), FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, webCookieContainer);
            stream.Close();

            ContentType = new ContentType(webResponse.ContentType);
            LastModified = webResponse.LastModified;
            ResponseUri = webResponse.ResponseUri;
            ResponseHeaders = webResponse.Headers;
            ContentLength = webResponse.ContentLength;
            Content = webResponse.GetResponseStream();
            

            if (ContentType.MediaType == "text/html")
            {
                HandleInternetResponseHTML(freexplorerAware);
            } 
            else if (ContentType.MediaType == "text/vnd.wap.wml")
            {
                HandleInternetResponseWML();
            }
            else if (ContentType.MediaType.StartsWith("image/"))
            {
                HandleInternetResponseImage();
            }
            return webResponse.StatusCode;
        }

        /// <summary>
        /// Store the card palette
        /// </summary>
        private static ArrayList _freeboxPalette;

        /// <summary>
        /// Retrieve the palette used for the card
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetFreeboxPalette()
        {
            if (null == _freeboxPalette)
            {
                _freeboxPalette = new ArrayList();
                _freeboxPalette.Add(Color.FromArgb(0, 0, 0, 0));
                int[] cols = { 0, 51, 102, 153, 204, 255 };
                foreach (int red in cols)
                    foreach (int green in cols)
                        foreach (int blue in cols)
                        {
                            if (red == 51)
                            {
                                if ((green == 102) || (blue == 102)) continue; // 51 102 not in palette
                                if ((green == 51 ) ^  (blue == 51)) continue;  // 51 51 51 only in palette
                                if ((green == 204) ^  (blue == 204)) continue; // 51 204 204 only in palette
                            }
                            _freeboxPalette.Add(Color.FromArgb(255, red, green, blue));
                        }
                for (int index = 192; index < 256; index++)
                    _freeboxPalette.Add(Color.FromArgb(0, 0, 0, 0));
            }

            return _freeboxPalette;
        }

        private void HandleInternetResponseImage()
        {
            // GDI+ needed
            using (Image originalImage = Image.FromStream(Content))
            {
                Content = new MemoryStream();
                ContentLength = -1;
                ContentType = new ContentType("image/gif");
                // following line eventually do an ordered dither to a standard 256-color palette
                originalImage.Save(Content, ImageFormat.Gif);
                using (Bitmap ditheredImage = new Bitmap(Content)) // retrieve the dithered image
                {
                    Content = new MemoryStream();
                    // create the quantizer for the freebox palette
                    PaletteQuantizer quantizer = new PaletteQuantizer(GetFreeboxPalette()); 
                    using (Bitmap quantized = quantizer.Quantize(ditheredImage))
                    {
                        quantized.Save(Content, ImageFormat.Gif);
                    }
                }
            }
        }

        private void HandleInternetResponseWML()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(Path.Combine(BaseDir, "wml.xsl"));
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.None;
            XmlReader wmlDoc = XmlReader.Create(Content, settings);
            StringWriter htmlWriter = new StringWriter();
            try
            {
                xslt.Transform(wmlDoc, null, htmlWriter);
            }
            catch (WebException e)
            {
                throw new ApplicationException("Erreur dans la conversion WML vers HTML: " + e.Message, e);
            }
            catch (XmlException e)
            {
                throw new ApplicationException("Erreur dans la conversion WML vers HTML: " + e.Message, e);
            }
            Content.Close();
            Content = null;
            string html = htmlWriter.ToString();
            html = ArrangeHTMLLinks(html);
            ContentType.MediaType = "text/html";
#if DEBUG
            Console.WriteLine("APRES: " + html);
#endif
            ReplyString(html);
        }

        private void HandleInternetResponseHTML(bool freexplorerAware)
        {
            if (ContentLength == -1) ContentLength = 0xFFFFFF;
            string html = "";
            while (ContentLength > 0)
            {
                byte[] bytes = new byte[4096];
                //Content.R
                int bytesRead = Content.Read(bytes, 0, (int) 4096);
                if (bytesRead == 0) break;
                html += Encoding.Default.GetString(bytes, 0, bytesRead);
                ContentLength -= bytesRead;
            }
            Content.Close();
            Content = null;
             
#if DEBUG
            Console.WriteLine("AVANT: "+html);
#endif

            if (html.Length < 5) return;

            if (html.Substring(0, 5).ToLower() == "<wml>")
            { // pour les sites qui renvoient du WML avec text/html
                Content = new MemoryStream(Encoding.Default.GetBytes(html));
                HandleInternetResponseWML();
                return;
            }

            int scan = CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "<body", CompareOptions.IgnoreCase);
            if (scan >= 0)
            {
                scan = html.IndexOf('>', scan + 5);
                if (scan >= 0)
                {
                    html = html.Insert(scan, " bgcolor=#F0F0F037 text=#5050503F link=#0000FF3F alink=#FF00003F vlink=#FF00FF3F>"+
                        "<link rel=\"yellow\" href=\"back\"/><box height=5><table><tr><td width=3%>&nbsp;</td><td width=94%><box width=5");
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
            html = html.Replace("<br/>", "<br>").Replace("<BR/>", "<BR>");
            if ((CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "charset=UTF-8") >= 0) ||
                ((CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "charset=iso") < 0) && (CultureInfo.InvariantCulture.CompareInfo.Compare(ContentType.CharSet, "UTF-8", CompareOptions.IgnoreCase) == 0)))
                try
                {
                    html = Encoding.UTF8.GetString(Encoding.Default.GetBytes(html));
                }
                catch (Exception)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
            html = ArrangeHTMLLinks(html);
            if (freexplorerAware)
            {
                string paramname = QueryArgs["$$keyboard"];
                if ((paramname != null) && (paramname.Length != 0))
                {
                    string paramvalue = QueryArgs[paramname];
                    if (paramvalue == null) paramvalue = "";
                    QueryArgs.Remove("$$keyboard");
                    QueryArgs.Remove(paramname);
                    keyboardReferer = Referer;
                    keyboardHTML = html;
                    keyboardURL = "http://212.27.38.254 " + Host + ":8080" + Url + "?" +
                        (QueryArgs.HasKeys() ? Wizou.HTTP.Utility.ToString(QueryArgs) + '&' : "") + paramname + '=';
                    HandleKeyboardRequest(paramvalue, null);
                    return;
                }
            }
#if DEBUG
            Console.WriteLine("APRES: " + html);
#endif
            ReplyString(html);
        }

        private void HandleKeyboardRequest(string value, string focus)
        {
            string keyboard = @"
<a href=/$$keyboard?v=¤A&f=A>A</a> 
<a href=/$$keyboard?v=¤B&f=B>B</a> 
<a href=/$$keyboard?v=¤C&f=C>C</a> 
<a href=/$$keyboard?v=¤D&f=D>D</a> 
<a href=/$$keyboard?v=¤E&f=E>E</a> 
<a href=/$$keyboard?v=¤F&f=F>F</a> 
<a href=/$$keyboard?v=¤G&f=G>G</a> 
<a href=/$$keyboard?v=¤H&f=H>H</a> 
<a href=/$$keyboard?v=¤I&f=I>I</a> 
<a href=/$$keyboard?v=¤J&f=J>J</a> 
<a href=/$$keyboard?v=¤K&f=K>K</a> 
<a href=/$$keyboard?v=¤L&f=L>L</a> 
<a href=/$$keyboard?v=¤M&f=M>M</a> 
<a href=/$$keyboard?v=¤N&f=N>N</a> 
<a href=/$$keyboard?v=¤O&f=O>O</a> 
<a href=/$$keyboard?v=¤P&f=P>P</a> 
<a href=/$$keyboard?v=¤Q&f=Q>Q</a> 
<a href=/$$keyboard?v=¤R&f=R>R</a> 
<a href=/$$keyboard?v=¤S&f=S>S</a> 
<a href=/$$keyboard?v=¤T&f=T>T</a> 
<a href=/$$keyboard?v=¤U&f=U>U</a> 
<a href=/$$keyboard?v=¤V&f=V>V</a> 
<a href=/$$keyboard?v=¤W&f=W>W</a> 
<a href=/$$keyboard?v=¤X&f=X>X</a> 
<a href=/$$keyboard?v=¤Y&f=Y>Y</a> 
<a href=/$$keyboard?v=¤Z&f=Z>Z&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</a>
</td></tr><tr><td>
&nbsp;<a href=/$$keyboard?v=¤+&f=+>Espace</a> |
<a href=/$$keyboard?v=¤a&f=a>a</a> 
<a href=/$$keyboard?v=¤b&f=b>b</a> 
<a href=/$$keyboard?v=¤c&f=c>c</a> 
<a href=/$$keyboard?v=¤d&f=d>d</a> 
<a href=/$$keyboard?v=¤e&f=e>e</a> 
<a href=/$$keyboard?v=¤f&f=f>f</a> 
<a href=/$$keyboard?v=¤g&f=g>g</a> 
<a href=/$$keyboard?v=¤h&f=h>h</a> 
<a href=/$$keyboard?v=¤i&f=i>i</a> 
<a href=/$$keyboard?v=¤j&f=j>j</a> 
<a href=/$$keyboard?v=¤k&f=k>k</a> 
<a href=/$$keyboard?v=¤l&f=l>l</a> 
<a href=/$$keyboard?v=¤m&f=m>m</a> 
<a href=/$$keyboard?v=¤n&f=n>n</a> 
<a href=/$$keyboard?v=¤o&f=o>o</a> 
<a href=/$$keyboard?v=¤p&f=p>p</a> 
<a href=/$$keyboard?v=¤q&f=q>q</a> 
<a href=/$$keyboard?v=¤r&f=r>r</a> 
<a href=/$$keyboard?v=¤s&f=s>s</a> 
<a href=/$$keyboard?v=¤t&f=t>t</a> 
<a href=/$$keyboard?v=¤u&f=u>u</a> 
<a href=/$$keyboard?v=¤v&f=v>v</a> 
<a href=/$$keyboard?v=¤w&f=w>w</a> 
<a href=/$$keyboard?v=¤x&f=x>x</a> 
<a href=/$$keyboard?v=¤y&f=y>y</a> 
<a href=/$$keyboard?v=¤z&f=z>z</a> | 
<a href=/$$keyboard?v=¤%c3%a0&f=%c3%a0>à</a>
<a href=/$$keyboard?v=¤%c3%a7&f=%c3%a7>ç</a>
<a href=/$$keyboard?v=¤%c3%a9&f=%c3%a9>é</a>
<a href=/$$keyboard?v=¤%c3%a8&f=%c3%a8>è</a>
<a href=/$$keyboard?v=¤%c3%aa&f=%c3%aa>ê</a>
<a href=/$$keyboard?v=¤%c3%af&f=%c3%af>ï</a>
<a href=/$$keyboard?v=¤%c3%b4&f=%c3%b4>ô</a>
<a href=/$$keyboard?v=¤%c3%b9&f=%c3%b9>ù</a>
<a href=/$$keyboard?v=¤%c3%bb&f=%c3%bb>û</a>
</td></tr><tr><td>&nbsp;";
            string keyboard2 = @"
<a href=/$$keyboard?v=¤%26&f=%26>&</a> 
<a href=/$$keyboard?v=¤%27&f='>'</a> 
<a href=/$$keyboard?v=¤(&f=(>(</a> 
<a href=/$$keyboard?v=¤)&f=)>)</a> 
<a href=/$$keyboard?v=¤%5b&f=%5b>[</a> 
<a href=/$$keyboard?v=¤%5d&f=%5d>]</a> 
<a href=/$$keyboard?v=¤%7b&f=%7b>{</a> 
<a href=/$$keyboard?v=¤%7d&f=%7d>}</a> 
<a href=/$$keyboard?v=¤%3c&f=%3c>&lt;</a> 
<a href=/$$keyboard?v=¤%3e&f=%3e>&gt;</a> 
<a href=/$$keyboard?v=¤%7c&f=%7c>|</a> 
<a href=/$$keyboard?v=¤_&f=_>_</a> 
<a href=/$$keyboard?v=¤%24&f=%24>$</a> 
<a href=/$$keyboard?v=¤%2b&f=%2b>+</a> 
<a href=/$$keyboard?v=¤-&f=->-</a> 
<a href=/$$keyboard?v=¤*&f=*>*</a> 
<a href=/$$keyboard?v=¤%2f&f=%2f>/</a> 
<a href=/$$keyboard?v=¤%3d&f=%3d>=</a> 
<a href=/$$keyboard?v=¤%5c&f=%5c>\</a> 
<a href=/$$keyboard?v=¤!&f=!>!</a> 
<a href=/$$keyboard?v=¤%3f&f=%3f>?</a> 
<a href=/$$keyboard?v=¤%25&f=%25>%</a> 
<a href=/$$keyboard?v=¤%3a&f=%3a>:</a> 
<a href=/$$keyboard?v=¤%3b&f=%3b>;</a> 
<a href=/$$keyboard?v=¤.&f=.>.</a> 
<a href=/$$keyboard?v=¤%5e&f=%5e>^</a> 
<a href=/$$keyboard?v=¤%23&f=%23>#</a>
<a href=/$$keyboard?v=¤%40&f=%40>@</a>
";
            keyboard = @"<meta name=nochannel_page content=/$$keyboard?v=¤%d>
<table border=1 cellspacing=0><tr><td>
&nbsp;<a href=""" + keyboardURL + HttpUtility.UrlEncode(value) + @"""&f=valid>Valider</a> |" + keyboard;
            if (value.Length != 0)
                keyboard += "<a href=/$$keyboard?v=&f=valid>Vider</a> | <a href=/$$keyboard?v=" + HttpUtility.UrlEncode(value.Substring(0, value.Length - 1)) + "&f=delete><font family=Symbol>$$</font></a> | ";
            keyboard += keyboard2.Replace("¤", HttpUtility.UrlEncode(value));
            keyboard = keyboard.Replace("¤", HttpUtility.UrlEncode(value));
            if (focus != null)
            {
                focus = HttpUtility.UrlEncode(focus);
                keyboard = keyboard.Replace("&f=" + focus, "&f=" + focus + " focused");
            }
            string html = keyboardHTML.Replace("$$keyboardhtml", HttpUtility.HtmlEncode(value).Replace(" ","&nbsp;")+"|")
                .Replace("$$keyboardval", value+"|").Replace("$$keyboard", keyboard+"</td></tr></table>");
            ReplyString(html);
        }

        private string ArrangeHTMLLinks(string html)
        {
            int scan = -1;
            int urlbegin, urlend, hostend;
            while ((scan = CultureInfo.InvariantCulture.CompareInfo.IndexOf(html, "http://", scan + 1, CompareOptions.IgnoreCase)) >= 0)
            {
                urlbegin = scan;
                while (html[--urlbegin] <= ' ') ;
                char quotedURL = html[urlbegin];
                if ((quotedURL != '"') && (quotedURL != '\''))
                    quotedURL = '\0';
                else
                    while (html[--urlbegin] <= ' ') ;
                if (html[urlbegin] != '=') 
                    continue;
                if (quotedURL != '\0')
                    urlend = html.IndexOf(quotedURL, scan+7);
                else
                    urlend = html.IndexOfAny(new char[] { ' ', '\t', '>' }, scan+7);
                hostend = html.IndexOfAny(new char[] { '/', '\'', '"', ' ', '\t', '>' }, scan + 7);
                if ((hostend < 0) || (urlend < 0) || (hostend > urlend)) 
                    break;
                html = html.Substring(0, urlbegin) +
                    "=\"http://212.27.38.254 " +
                    html.Substring(scan + 7, hostend - scan - 7) +
                    ":8080" +
                    html.Substring(hostend, urlend-hostend)+
                    "\""+
                    html.Substring(urlend+(quotedURL == '\0' ? 0 : 1));
                scan = urlbegin + 18;
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
        public static Boolean LessIconsInExplorer;

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

        static string PathCombine(params string[] elems)
        {
            string result = "";
            foreach (string elem in elems)
            {
                result = Path.Combine(result, elem);
            }
            return result;
        }

        internal static string GetIconForMediaType(VLC.Utility.MediaType type, string defaultIcon)
        {
            //FIXME: Path relatif au CurrentDirectory mais devrait être relatif au BaseDir du serveur
            FileInfo iconFile = new FileInfo(PathCombine("pages", "img", "media", type.ToString() + ".gif"));
            return iconFile.Exists ? "img/media/" + iconFile.Name : defaultIcon;

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
            Array.Sort<string>(files);

            StringBuilder html = new StringBuilder(256 * files.Length); // chaque ligne prend environ 225 caractères
            foreach (string file in files)
            {
                VLC.Utility.MediaType mediaType = VLC.Utility.GetMediaType(file);
                if (mediaType == VLC.Utility.MediaType.Unknown)
                    continue; // skip unknown type files
                if (!LessIconsInExplorer)
                {
                    html.Append("<img src=");
                    html.Append(GetIconForMediaType(mediaType, "img/mediafile.gif"));
                    html.Append(" width=16 height=17>");
                }
                html.Append("&nbsp;<a href=\"play.html?action=add&_file=");
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
                if (level == 1)
                {
                    folder = folder.TrimEnd(Path.DirectorySeparatorChar);
                    string volumeName, fileSystemName;
                    int dummy;
                    int iResult = SysWin32.GetVolumeInformation(path, out volumeName, out dummy, out dummy, out dummy, out fileSystemName);
                    if (fileSystemName == "CDFS")
                        html.Append("<img src=img/cddvd.gif width=16 height=17>&nbsp;");
                    else
                        html.Append("<img src=img/harddisk.gif width=16 height=17>&nbsp;");
                    if (volumeName.Length != 0)
                        folder = volumeName + " (" + folder + ")";
                    
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
                    if (!LessIconsInExplorer)
                    {
                        html.Append("<img src=img/folder.gif width=16 height=17>&nbsp;");
                    }
                    try
                    {
                        subdirs = Directory.GetDirectories(path);
                    }
                    catch (IOException)
                    {
                    }
                }
                Array.Sort<string>(subdirs); 
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
            if ((folder.Length == 0) || (folder[0] != '$'))
            {
                return folder;
            }

            switch (folder.ToLower())
            {
                case "$personal":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                case "$my music":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); 

                case "$my pictures":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); 

                case "$my video":
                    return (string)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders",
                        "My Video", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)); 

                default:
                    throw new Exception("Dossier special inconnu: " + folder);
            }
        }

    }
}
/* notes diverses

Webcam sur TV:
 input: dshow://
 output: sout=#transcode{vcodec=mp2v,vb=3072,scale=1,acodec=mpga,ab=256,channels=2} :duplicate{dst=std{access=udp,mux=ts,url=212.27.38.253:1234}}
========================================================================
Forcer le son à sortir par la carte son du PC
    * Effacez les lignes contenant sout-transcode-ab et sout-transcode-acodec
    * Ajouter la ligne #EXTVLCOPT:sout=#duplicate{dst=transcode:std,select=video,dst=display,select=audio} 
========================================================================

manuel:
start vlc\vlc.exe --extraintf http --no-playlist-autostart --http-src=./http-fbx --http-host :8080 --sout="#transcode:std" --sout-standard-access=udp --sout-standard-mux=ts --sout-standard-url=212.27.38.253:1234 --sout-transcode-vcodec=mp2v --sout-transcode-vb=900 --sout-transcode-acodec=mpga --sout-transcode-ab=384 --sout-transcode-channels=2 --file-caching=1000 --sout-ts-pid-video=68 --sout-ts-pid-audio=69 --sout-ts-pcr=80 --sout-ts-dts-delay=400 --play-and-stop --open=

sout=#transcode{vcodec=mp2v,vb=9000,scale=1,acodec=mpga,ab=384,channels=2} :duplicate{dst=std{access=udp,mux=ts,url=212.27.38.253:1234}}

Note: Pour ceux qui sont en Wi-Fi, remplacez vb=9000 par vb=2000 pour ainsi éviter les freezes.

*/
