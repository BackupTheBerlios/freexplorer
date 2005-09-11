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
using System.Threading;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Mime;

namespace Wizou.HTTP
{

    class BasicHttpServer
    {
        public string ErrorFile = "/_error.html"; // doit commencer par un slash
        public string BaseDir;
        private int tcpPort;
        private TcpListener tcpListener = null;
        protected bool HackStatusCodeAlwaysOK = false; // cas particulier pour les clients HTTP qui n'aiment pas autre chose qu'OK (ex: Freebox ...)

        // Requête HTTP:
        protected Socket Socket;
        protected string HttpMethod;
        protected Version ProtocolVersion;
        protected string Url;
        protected string Query; // les arguments de l'URL (après l'eventuel '?') sous forme d'une chaine incluant le '?'
        protected NameValueCollection QueryArgs; // les arguments de l'URL sous forme d'une collection de nom/valeurs (eventuellement plusieurs valeurs pour le meme nom)
        protected string Accept;
        protected string Connection;
        protected long RequestContentLength;
        protected ContentType RequestContentType;
        protected string Host;
        protected DateTime IfModifiedSince;
        protected string Referer;
        protected string UserAgent;
        protected WebHeaderCollection RequestHeaders;
        protected StreamReader RequestContent; // pour lire le reste de la requete s'il y a un contenu (ex: requete POST, PUT...)

        // Réponse HTTP:
        public string ErrorDescription;
        public long ContentLength; // setting this to -1 means it should be calculated upon 'content' stream length
        public ContentType ContentType;
        public DateTime LastModified;
        //public CookieCollection Cookies;
        public WebHeaderCollection ResponseHeaders;
        public Uri ResponseUri;
        public Stream Content;
        
        public BasicHttpServer(string baseDir, int port)
        {
            BaseDir = baseDir;
            this.tcpPort = port;
        }


        public void Start()
        {
            if ((tcpListener != null) && tcpListener.Server.IsBound) return;
            // le new TcpListener est effectué ici pour avoir lieu *après* le lancement de VLC
            tcpListener = new TcpListener(IPAddress.Any, tcpPort);
            tcpListener.Start();
            Console.WriteLine("HTTP started");
            Thread serverThread = new Thread(new ThreadStart(ThreadLoop));
            serverThread.Start();
        }

        public void Stop()
        {
            if ((tcpListener == null) || !tcpListener.Server.IsBound) return;
            tcpListener.Stop();
            tcpListener = null;
            Console.WriteLine("HTTP stopped");
        }

        private void ThreadLoop()
        {
            while (true)
            {
                try
                {
                    Socket = tcpListener.AcceptSocket();
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.Interrupted)
                        return; // arrêt du serveur
                    throw;
                }
                try
                {
                    NetworkStream networkStream = new NetworkStream(Socket, true);
                    using (RequestContent = new StreamReader(networkStream, Encoding.Default))
                    {
                        string line = null;
                        int index;
                        ProtocolVersion = HttpVersion.Version10;
                        HttpStatusCode StatusCode;// = HttpStatusCode.OK;
                        ErrorDescription = null;
                        Url = null;
                        Query = null;
                        LastModified = DateTime.Now;
                        ResponseHeaders = new WebHeaderCollection();
                        ContentType = new ContentType("text/html");
                        ContentLength = -1;
                        Content = null;
                        try
                        {
                            try
                            {
                                line = RequestContent.ReadLine();
                            }
                            catch (IOException e)
                            {
                                throw new ApplicationException(e.Message, e);
                            }
                            Console.WriteLine("HTTP< " + line);
                            index = line.IndexOf(' ');
                            HttpMethod = line.Substring(0, index);
                            line = line.Substring(index + 1);
                            index = line.IndexOf(' ');
                            if (index >= 0)
                            {
                                string protocolVersion = line.Substring(index + 1);
                                Match m = (new Regex("HTTP/(\\d+).(\\d+)")).Match(protocolVersion);
                                if (!m.Success) throw new Exception("Protocole HTTP invalide");
                                line = line.Substring(0, index);
                                ProtocolVersion = new Version(Convert.ToInt32(m.Groups[1].Value), Convert.ToInt32(m.Groups[2].Value));
                            }
                            else
                                ProtocolVersion = HttpVersion.Version10;
                            if (line.Length == 0) throw new Exception("URI manquante");
                            index = line.IndexOf('?');
                            if (index >= 0)
                            {
                                Query = line.Substring(index);
                                QueryArgs = System.Web.HttpUtility.ParseQueryString(System.Web.HttpUtility.HtmlDecode(line.Substring(index + 1)));
                                line = line.Substring(0, index);
                            }
                            else
                            {
                                Query = String.Empty;
                                QueryArgs = new NameValueCollection();
                            }
                            Url = line;
                            
                            Accept = null;
                            Connection = null;
                            RequestContentLength = -1;
                            RequestContentType = null;
                            Host = null;
                            IfModifiedSince = new DateTime(); // DateTime vide
                            Referer = null;
                            UserAgent = null;
                            RequestHeaders = new WebHeaderCollection();
                            while ((line = RequestContent.ReadLine()).Length != 0)
                            {
                                index = line.IndexOf(':');
                                if (index < 0)
                                {
                                    Console.WriteLine("BasicHttpServer: Header HTTP invalide: " + line);
                                    continue;
                                }
                                string headerName = line.Substring(0, index);
                                line = line.Substring(index + 1).Trim();
                                switch (headerName.ToLower())
                                {
                                    case "accept": Accept = line; break;
                                    case "connection": Connection = line; break;
                                    case "content-length": RequestContentLength = Convert.ToInt64(line); break;
                                    case "content-type": RequestContentType = new ContentType(line); break;
                                    case "host": Host = line; Console.WriteLine("HTTP< Host: " + line); break;
                                    case "if-modified-since": IfModifiedSince = DateTime.Parse(line); break;
                                    case "referer": Referer = line; break;
                                    case "user-agent": UserAgent = line; break;
                                    default:
                                        Console.WriteLine("BasicHttpServer: Header inconnu: {0}: {1}", headerName, line);
                                        RequestHeaders.Add(headerName, line);
                                        break;
                                }
                            }

                            try
                            {
                                // handle the request (this function is eventually overriden)
                                StatusCode = HandleRequest();
                                if (StatusCode != HttpStatusCode.OK)
                                    Console.WriteLine("BasicHttpServer: {0} {1}: {2}\r\n\t{3}", (int) StatusCode, StatusCode, Url, ErrorDescription);
                            }
#if DEBUG
                            catch (ApplicationException e)
#else
                            catch (Exception e) // en mode RELEASE, catch toutes les exceptions pour ne pas planter le serveur
#endif
                            {
                                StatusCode = HttpStatusCode.InternalServerError;
                                ErrorDescription = e.Message;
                                Console.WriteLine("BasicHttpServer: {0}:\r\n\tsur la requete {1}{2}", e, Url, Query);
                            }
                        }
#if DEBUG
                        catch (ApplicationException e)
#else
                        catch (Exception e) // en mode RELEASE, catch toutes les exceptions pour ne pas planter le serveur
#endif
                        {
                            StatusCode = HttpStatusCode.BadRequest;
                            ErrorDescription = e.Message;
                            Console.WriteLine("BasicHttpServer: {0}:\r\nsur le parsing de {1}", e, line);
                        }
                        if ((Content == null) && (StatusCode != HttpStatusCode.OK))
                        {
                            ContentType = new ContentType("text/html");
                            ReplyFormattedFile(ErrorFile, (int)StatusCode, StatusCode, Url, ErrorDescription.Replace("\r\n", "<br>"));
                        }

                        if (HackStatusCodeAlwaysOK) // cas particulier des clients HTTP qui n'aiment pas autre chose qu'OK (ex: Freebox ...)
                            StatusCode = HttpStatusCode.OK;

                        // envoi des Headers de la réponse
                        StreamWriter writer = new StreamWriter(networkStream, Encoding.Default);
                        writer.WriteLine("HTTP/{0}.{1} {2} {3}", ProtocolVersion.Major, ProtocolVersion.Minor,
                                                                (int)StatusCode, StatusCode);
                        writer.WriteLine("Last-Modified: " + LastModified.ToString("R"));
                        writer.WriteLine("Location: " + ResponseUri);
                        if (ProtocolVersion > HttpVersion.Version10)
                        {
                            writer.WriteLine("Connection: close"); // c'est un serveur vraiment basique... pas de Keep-Alive
                            for (index = 0; index < ResponseHeaders.Count; index++)
                                writer.WriteLine("{0}: {1}", ResponseHeaders.GetKey(index), ResponseHeaders.Get(index));
                        }
                        if (Content != null)
                        {
                            writer.WriteLine("Content-Type: " + ContentType);
                            Console.WriteLine("HTTP> Content-Type: " + ContentType);
                            if ((ContentLength == -1) && Content.CanSeek) 
                                ContentLength = Content.Length;
                            if (ContentLength != -1)
                            {
                                writer.WriteLine("Content-Length: " + ContentLength);
                                Console.WriteLine("HTTP> Content-Length: " + ContentLength);
                            }
                        }
                        writer.WriteLine(); // termine les headers avec une ligne vide
                        writer.Flush();

                        if (Content != null)
                        {
                            if (HttpMethod != "HEAD")
                                try
                                {
                                    // TODO: faire un choix sur le comportement si la position n'est pas au debut du Stream)
                                    // actuellement, pour MemoryStream, on balance tout depuis le debut, meme si la position n'est pas au debut
                                    // et pour les autres Stream, on balance à partir de la position en cours
                                    // (ce qui permet de n'envoyer qu'une portion précise si on veut)

                                    if ((Content is MemoryStream) && (ContentLength == Content.Length))
                                    { // variante optimisée pour les MemoryStream
                                        ((MemoryStream)Content).WriteTo(networkStream);
                                    }
                                    else if (ContentLength == -1)
                                    {
                                        byte[] buffer = new byte[4096L];
                                        do
                                        {
                                            index = Content.Read(buffer, 0, 4096);
                                            networkStream.Write(buffer, 0, index);
                                        } while (index != 0);
                                    }
                                    else
                                    {
                                        byte[] buffer = new byte[Math.Min(ContentLength, 4096L)];
                                        while (ContentLength > 0)
                                        {
                                            index = Content.Read(buffer, 0, buffer.Length);
                                            if (index == 0)
                                            {
                                                Console.WriteLine("BasicHttpServer: Supplied Content stream did not contain expected ContentLength");
                                                break;
                                            }
                                            networkStream.Write(buffer, 0, index);
                                            ContentLength -= index;
                                        }
                                    }
                                }
                                catch (IOException)
                                {
                                }
                            Content.Close();
                        } // if (Content != null)
                    } // using (content) => Terminate the connection

                    HandleAfterReply(); // opportunité de faire un traitement après que la réponse aie été envoyée
                }
#if DEBUG
                catch (ApplicationException e)
#else
                catch (Exception e) // en mode RELEASE, catch toutes les exceptions pour ne pas planter le serveur
#endif
                {
                    Console.WriteLine("BasicHttpServer: " + e);
                }
            } // while
        }

        public HttpStatusCode ReplyString(string html)
        {
            byte[] buffer = Encoding.Default.GetBytes(html);
            ContentLength = buffer.Length;
            Content = new MemoryStream(buffer);
            return HttpStatusCode.OK;
        }

        public HttpStatusCode ReplyFormattedFile(string name, params object[] args)
        {
            string path = Path.GetFullPath(Path.Combine(BaseDir, '.' + name));
            if (!File.Exists(path))
            {
                ErrorDescription = "Page " + name + " introuvable";
                return HttpStatusCode.NotFound;
            }
            return ReplyString(String.Format(File.ReadAllText(path, Encoding.Default), args));
        }
        
        virtual protected HttpStatusCode HandleRequest()
        {
            if (Url[0] != '/') throw new Exception("URL should start with a slash");
            string path = Path.GetFullPath(Path.Combine(BaseDir, '.' + Url));
            if (!File.Exists(path))
            {
                ErrorDescription = "Fichier " + Url + " introuvable";
                return HttpStatusCode.NotFound;
            }
            ContentType = ExtensionToContentType(Path.GetExtension(path));
            LastModified = File.GetLastWriteTime(path);
            Content = File.OpenRead(path);
            return HttpStatusCode.OK;
        }

        virtual protected void HandleAfterReply()
        {
            // on ne fait rien en standard
        }

        public static ContentType ExtensionToContentType(string extension)
        {
            string result = (string)Microsoft.Win32.Registry.GetValue("HKEY_CLASSES_ROOT\\" + extension, "Content Type", null);
            if (result == null)
                return new ContentType(); // "application/octet-stream" par defaut
            else
                return new ContentType(result);
        }
    }
}
