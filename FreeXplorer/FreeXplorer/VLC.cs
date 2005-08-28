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
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Net;

namespace Wizou.VLC
{
    public class VLCApp
    {
        private bool active = false;
        private bool showWindow = false;
        private Process process = null;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private int playlistSize;
        private int itemAddedCounter;
        
        ProcessStartInfo startInfo;
        private int rc_host_port;// = 31186;
        private char dvdLetter;// = 'D';
        private AudioTranscode audioTranscode;// = AudioTranscode.MPGA;

        private static string AudioTranscode2Options(AudioTranscode audioTranscode)
        {
            switch (audioTranscode)
            {
                case AudioTranscode.MPGA: return " --sout-transcode-acodec=mpga --sout-transcode-ab=384 --sout-transcode-channels=2";
                case AudioTranscode.A52: return " --sout-transcode-acodec=a52 --sout-transcode-ab=448 --sout-transcode-channels=6";
                case AudioTranscode.PC: return " --sout=#duplicate{dst=transcode:std,select=video,dst=display,select=audio}";
                default: return " --sout-transcode-acodec= --sout-transcode-ab= --sout-transcode-channels=";
            }
        }

        public void SetConfig(string exeFilename, string audioLanguage, string subLanguage, bool ffmpeg_interlace, double transform_scale, int transcode_vb)
        {
            startInfo = new ProcessStartInfo(exeFilename,
                " --config ." + Path.DirectorySeparatorChar + "vlcrc" +
                " --rc-host 127.0.0.1:" + rc_host_port +
                " --audio-language=" + audioLanguage +
                " --sub-language=" + subLanguage +
                " --dvd=" + dvdLetter + ":" +
                " --" + (ffmpeg_interlace ? "sout-ffmpeg-interlace" : "no-sout-ffmpeg-interlace") +
                " --sout-transcode-scale=" + transform_scale.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) +
                AudioTranscode2Options(audioTranscode)+
                " --sout-transcode-vb=" + transcode_vb.ToString() +
                " --wxwin-config-last=(-1,0,0,1280,1024)(0,650,21,363,141)");
            startInfo.UseShellExecute = true;
        }

        public void Start()
        {
            if (active) return;
            // la ligne suivante n'a pas l'air de marcher avec VLC
            //if (!showWindow) startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process = Process.Start(startInfo);
            if ((process == null) || (!process.WaitForInputIdle(15000)) ||
                process.HasExited || (process.MainWindowHandle == IntPtr.Zero))
            {
                if ((process != null) && !process.HasExited) process.Kill();
                throw new VLCException("Le processus n'a pas démarré normalement");
            }
            if (!showWindow) SysWin32.ShowWindow(process.MainWindowHandle, SysWin32.SW_HIDE);
            Console.WriteLine("VLC started");
            tcpClient = null;
            for (int retries = 0; retries < 5; retries++)
            {
                try
                {
                    tcpClient = new TcpClient(new IPEndPoint(IPAddress.Loopback, 0));
                    tcpClient.Connect(IPAddress.Loopback, Convert.ToInt32(rc_host_port));
                    break;
                }
                catch (SocketException)
                {
                    if (retries == 4)
                    {
                        process.Kill();
                        throw new VLCException("L'interface RC de VLC ne répond pas");
                    }
                    Thread.Sleep(500);
                }
            }
            networkStream = tcpClient.GetStream();
            networkStream.ReadTimeout = 1000;
            /*WriteLine("get_length");
            Thread.Sleep(200);
            networkStream.Read(RC_buffer, 0, 1024);*/
            RC_buffer_count = 0;
            itemAddedCounter = 0;
            active = true;
        }

        public void Stop()
        {
            if (!active) return;
            if (!Crashed())
            {
                // on essaye de terminer proprement VLC
                try
                {
                    Command("quit");
                }
                catch (VLCException)
                {
                }
            }
            networkStream.Close();
            // si vlc n'est pas mort apres 2 sec, on le tue
            if (!process.WaitForExit(2000))
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException)
                {
                }
            active = false;
            Console.WriteLine("VLC stopped");
        }

        private void MustBeStopped()
        {
            if (active) throw new VLCException("VLC doit être arreté pour changer le reglage");
        }

        private void MustBeActive()
        {
            if (!active) throw new VLCException("VLC n'est pas lancé");
        }

        public bool Crashed()
        {
            return (process.HasExited || !tcpClient.Connected);
        }

        public string PathFromMRL(string param)
        {
            if (!param.Contains("://")) // si ce n'est pas une MRL, c'est directement un chemin d'accès de fichier
                return param;
            else if (param.StartsWith("dvdsimple://"))
            {
                if (param.Length == 12)
                    return dvdLetter + ":\\";
                else
                    return param.Substring(12);
            }
            else // c'est une MRL inconnue
                return "";
        }
       
        #region Socket Buffer

        byte[] RC_buffer = new byte[1024];
        int RC_buffer_count = 0;

        private void WriteLine(string value)
        {
            if (Crashed())
                throw new VLCException("VLC vient (encore!) de planter...\r\nJe vais le relancer mais la lecture en cours est abandonnée");
            Console.WriteLine("VLC> " + value);
            byte[] buffer = Encoding.Default.GetBytes(value);
            networkStream.Write(buffer, 0, buffer.Length);
            try
            {
                networkStream.WriteByte(0x0A);
            }
            catch (IOException)
            {
                if (Crashed())
                    throw new VLCException("VLC vient (encore!) de planter...\r\nJe vais le relancer mais la lecture en cours est abandonnée");
                throw;
            }
        }

        private string ReadLine()
        {
            int index = 0;
            while (index+1 < RC_buffer_count)
            {
                if (RC_buffer[index] == 0x0A)
                    if (RC_buffer[index + 1] == 0x0D)
                    {
                        string result = Encoding.Default.GetString(RC_buffer, 0, index);
                        RC_buffer_count -= index + 2;
                        Array.Copy(RC_buffer, index + 2, RC_buffer, 0, RC_buffer_count);
                        Console.WriteLine("VLC<   " + result);
                        return result;
                    }
                    else
                        throw new VLCException("Réponse inattendue");
                index++;
            }
            while (RC_buffer_count < 1024)
            {
                bool test = process.HasExited;
                try
                {
                    RC_buffer_count += networkStream.Read(RC_buffer, RC_buffer_count, 1024 - RC_buffer_count); // on donne 1 sec pour lire plus
                    //Console.Write("Received from VLC: "); for (int scan = 0; scan < RC_buffer_count; scan++) Console.Write("{0:X2} ", RC_buffer[scan]); Console.WriteLine();
                }
                catch (IOException)
                {
                    if (Crashed())
                        throw new VLCException("VLC vient (encore!) de planter...\r\nJe vais le relancer mais la lecture en cours est abandonnée");
                    throw;
                }
                while (index+1 < RC_buffer_count)
                {
                    if (RC_buffer[index] == 0x0A)
                        if (RC_buffer[index+1] == 0x0D)
                        {
                            string result = Encoding.Default.GetString(RC_buffer, 0, index);
                            RC_buffer_count -= index + 2;
                            Array.Copy(RC_buffer, index + 2, RC_buffer, 0, RC_buffer_count);
                            Console.WriteLine("VLC<   " + result);
                            return result;
                        }
                        else
                            throw new VLCException("Réponse inattendue");
                    index++;
                }
            }
            throw new VLCException("Buffer plein");
        }

        private void MakeSureReadEmpty()
        {
            if ((RC_buffer_count != 0) || networkStream.DataAvailable)
            {
#if DEBUG
                string temp = Encoding.Default.GetString(RC_buffer, 0, RC_buffer_count);
                Debugger.Break();
#endif
                while (networkStream.DataAvailable)
                    networkStream.Read(RC_buffer, 0, 1024);
                RC_buffer_count = 0;
                throw new VLCException("Lignes inattendues dans la reponse precedente");
            }
        }

        #endregion ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region RC commands
        private void ReadCheckNoError(string command)
        {
            if (ReadLine() != command + ": returned 0 (no error)")
                throw new VLCException("La commande RC a renvoyée une erreur");
        }

        public void Command(string command)
        {
            MustBeActive();
            MakeSureReadEmpty();
            WriteLine(command);
            ReadCheckNoError(command.Substring(0,(command+' ').IndexOf(' ')));
        }

        public string GetCurrentCaption()
        {
            MustBeActive();
            MakeSureReadEmpty();
            WriteLine("get_title");
            return ReadLine();
        }

        public int GetDurationTime()
        {
            MustBeActive();
            MakeSureReadEmpty();
            WriteLine("get_length");
            return Convert.ToInt32(ReadLine());
        }

        public int GetElapsedTime()
        {
            MustBeActive();
            MakeSureReadEmpty();
            WriteLine("get_time");
            return Convert.ToInt32(ReadLine());
        }

        public int GetChapterInfo(ref int max)
        {
            MustBeActive();
            MakeSureReadEmpty();
            WriteLine("chapter");
            string line = ReadLine();
            if (line == "chapter: returned 0 (no error)")
            {
                max = 0;
                return -1;
            }
            Match m = (new Regex("Currently playing chapter (\\d+)/(\\d+)")).Match(line);
            if (!m.Success)
                throw new VLCException("Réponse inattendue pour 'chapter'");
            ReadCheckNoError("chapter");
            max = Convert.ToInt32(m.Groups[2].Value);
            return Convert.ToInt32(m.Groups[1].Value);
        }

        public int GetTitleInfo(ref int max)
        {
            MustBeActive();
            MakeSureReadEmpty();
            WriteLine("title");
            string line = ReadLine();
            if (line == "title: returned 0 (no error)")
            {
                max = 0;
                return -1;
            }
            Match m = (new Regex("Currently playing title (\\d+)/(\\d+)")).Match(line);
            if (!m.Success)
                throw new VLCException("Réponse inattendue pour 'title'");
            ReadCheckNoError("title");
            max = Convert.ToInt32(m.Groups[2].Value);
            return Convert.ToInt32(m.Groups[1].Value);
        }

        // renvoit un nombre negatif: indiquant l'indice negatif de l'item en train d'etre joué, relatif aux derniers ajouts
        // -1 représente le dernier de la liste, -2 l'avant-dernier
        public int GetPlayingIndex()
        {
            MustBeActive();
            WriteLine("playlist");
            string line;
            int result = 0, count = 0;
            do
            {
                line = ReadLine();
                if ((line[0] == '|') && (line != "| no entries"))
                {
                    count++;
                    if (line[1] == '*')
                        result = -1;
                    else if (result != 0)
                        result--;
                }
            } while (line != "playlist: returned 0 (no error)");
            itemAddedCounter = count;
            return result;
        }

        // index doit etre un indice negatif de l'item à jouer
        // -1 représente le dernier de la liste, -2 l'avant-dernier
        public void SetPlayingIndex(int index)
        {
            Debugger.Break(); // fonction jamais testée
            MustBeActive();
            WriteLine("goto " + (itemAddedCounter+index));
            ReadCheckNoError("goto");
        }

        // mettre playlistLength à 0 pour calculer automatiquement le nombre d'element à jouer
        //  (quand on ne sait pas s'il s'agit d'une playlist ou non)
        public void Play(string media, int playlistLength)
        {
            MustBeActive();
            // compose la MRL qui sera parsé par VLC [dans rc.c : parse_MRL()]
            string MRL;
            if ((media[0] == '"') || (media[0] == '\''))
                MRL = media;
            else
            {
                MRL = '"' + Utility.GetMediaMRL(media) + '"';
                string[] MRLOptions = Utility.GetMediaMRLOptions(media);
                if (MRLOptions != null)
                    foreach (string MRLOption in MRLOptions)
                        if (MRLOption.Contains(" "))
                            MRL += " \":" + MRLOption + '"';
                        else
                            MRL += " :" + MRLOption;
            }
            MakeSureReadEmpty();
            if (playlistLength != 0)
                playlistSize = playlistLength;
            else if (Utility.GetMediaType(media) != Utility.MediaType.Playlist)
                playlistSize = 1;
            else
            {
                playlistSize = -1; // obtenir de VLC la taille de la playlist
                itemAddedCounter = 0;
            }
            WriteLine("add " + MRL);
            if (!ReadLine().StartsWith("trying to add "))
                throw new VLCException("Réponse inattendue pour 'add'");
            ReadCheckNoError("add");
            itemAddedCounter += playlistSize;
        }

        // jouer plusieurs fichiers
        public void Play(System.Collections.Specialized.StringCollection playlist)
        {
            // compose une playlist puis la joue
            M3UFile m3uFile = new M3UFile();
            foreach (string media in playlist)
                m3uFile.AppendMedia(media);
            m3uFile.Close();
            Play(m3uFile.Name, playlist.Count);
        }

        public void PlaylistPrev()
        {
            if (playlistSize == 1) return;
            int index = GetPlayingIndex();
            if (playlistSize == -1) playlistSize = -index;
            if (index > -playlistSize)
                Command("prev");
        }

        public void PlaylistNext()
        {
            if (playlistSize == 1) return;
            if (playlistSize == -1) playlistSize = -GetPlayingIndex();
            Command("next");
        }


        #endregion ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Properties accessors

        public bool Active
        {
            get
            {
                return active;
            }
        }

        public bool ShowWindow
        {
            get
            {
                return showWindow;
            }
            set
            {
                if (active)
                    SysWin32.ShowWindow(process.MainWindowHandle, value ? SysWin32.SW_SHOWNORMAL : SysWin32.SW_HIDE);
                showWindow = value;
            }
        }

        public AudioTranscode AudioTranscode
        {
            get
            {
                return audioTranscode;
            }
            set
            {
                MustBeStopped();
                audioTranscode = value;
            }
        }

        public char DVDLetter
        {
            get
            {
                return dvdLetter;
            }
            set
            {
                value = Char.ToUpper(value);
                if ((value < 'A') || (value > 'Z'))
                    throw new VLCException("Lettre invalide pour le lecteur de DVD");
                MustBeStopped();
                dvdLetter = value;
            }
        }

        public int RC_host_port
        {
            get
            {
                return rc_host_port;
            }
            set
            {
                MustBeStopped();
                rc_host_port = value;
            }
        }

        
        
        #endregion ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }

    class VLCCache
    {
        VLCApp vlcApp;

        [Flags]
        public enum CachedFlags
        {
            Caption = 0x01,
            DurationTime = 0x02,
            ElapsedTime = 0x04,
            ChapterInfo = 0x08,
            TitleInfo = 0x10,
            PlayingIndex = 0x20,
        };

        private CachedFlags cachedFlags = 0;

        public VLCCache(VLCApp vlcApp)
        {
            this.vlcApp = vlcApp;
        }

        public void Invalidate()
        {
            cachedFlags = 0;
        }
        public void Invalidate(CachedFlags flags)
        {
            cachedFlags &= ~flags;
        }

        private string caption;
        public string Caption
        {
            get
            {
                if ((cachedFlags & CachedFlags.Caption) == 0)
                {
                    caption = vlcApp.GetCurrentCaption();
                    cachedFlags |= CachedFlags.Caption;
                }
                return caption;
            }
        }

        private int durationTime;
        public int DurationTime
        {
            get
            {
                if ((cachedFlags & CachedFlags.DurationTime) == 0)
                {
                    durationTime = vlcApp.GetDurationTime();
                    cachedFlags |= CachedFlags.DurationTime;
                }
                return durationTime;
            }
        }

        private int elapsedTime;
        public int ElapsedTime
        {
            get
            {
                if ((cachedFlags & CachedFlags.ElapsedTime) == 0)
                {
                    elapsedTime = vlcApp.GetElapsedTime();
                    cachedFlags |= CachedFlags.ElapsedTime;
                }
                return elapsedTime;
            }
        }

        private int chapter;
        private int chapterMax;
        public int Chapter
        {
            get
            {
                if ((cachedFlags & CachedFlags.ChapterInfo) == 0)
                {
                    chapter = vlcApp.GetChapterInfo(ref chapterMax);
                    cachedFlags |= CachedFlags.ChapterInfo;
                }
                return chapter;
            }
        }
        public int ChapterMax
        {
            get
            {
                if ((cachedFlags & CachedFlags.ChapterInfo) == 0)
                {
                    chapter = vlcApp.GetChapterInfo(ref chapterMax);
                    cachedFlags |= CachedFlags.ChapterInfo;
                }
                return chapterMax;
            }
        }

        private int title;
        private int titleMax;
        public int Title
        {
            get
            {
                if ((cachedFlags & CachedFlags.TitleInfo) == 0)
                {
                    title = vlcApp.GetTitleInfo(ref titleMax);
                    cachedFlags |= CachedFlags.TitleInfo;
                }
                return title;
            }
        }
        public int TitleMax
        {
            get
            {
                if ((cachedFlags & CachedFlags.TitleInfo) == 0)
                {
                    title = vlcApp.GetTitleInfo(ref titleMax);
                    cachedFlags |= CachedFlags.TitleInfo;
                }
                return titleMax;
            }
        }

        public int playingIndex;
        public int PlayingIndex
        {
            get
            {
                if ((cachedFlags & CachedFlags.PlayingIndex) == 0)
                {
                    playingIndex = vlcApp.GetPlayingIndex();
                    cachedFlags |= CachedFlags.PlayingIndex;
                }
                return playingIndex;
            }
        }
    
    }

    class VLCException : ApplicationException
    {
        public VLCException(string message) : base(message) { }
    }

    public enum AudioTranscode
    {
        None,
        MPGA,
        A52,
        PC
    };

    static class Utility
    {
        public static string VideoExts = ".mpg.ts.mp2.mpeg.vob.avi.ogm.mkv.mp4.mov.mjpeg.asf.wmv.wma.divx";
        public static string SoundExts = ".mp3.aac.au.aif.aiff.wav.ogg";
        public static string PictureExts = ".jpg.jpeg.png.tiff.gif";

        public enum MediaType
        {
            Unknown,
            DVD,
            Playlist,
            Video,
            Sound,
            Picture,
        }

        public static MediaType GetMediaType(string media)
        {
            if (media.StartsWith("dvdsimple://"))
                return MediaType.DVD;
            string value;
            try
            {
                value = Path.GetExtension(media).ToLower();
            }
            catch (ArgumentException)
            {
                return MediaType.Unknown;
            }
            if (value.Length != 0)
            {
                if (value == ".m3u")
                    return MediaType.Playlist;
                else if (SoundExts.Contains(value))
                    return MediaType.Sound;
                else if (VideoExts.Contains(value))
                    return MediaType.Video;
                else if (PictureExts.Contains(value))
                    return MediaType.Picture;
            }
            return MediaType.Unknown;
        }

        // can return null if no options !
        public static string[] GetMediaMRLOptions(string media)
        {
            switch (GetMediaType(media))
            {
                case MediaType.Picture:
                    return new string[]
                    {
                        "fake-file=" + media,
                        "sout-transcode-width=720",
                        "sout-transcode-height=576",
                        "sout-transcode-vfilter=deinterlace",
                        "sout-deinterlace-mode=blend",
                        "sout-ffmpeg-keyint=1",
                    };
                case MediaType.Video:
                    if (File.Exists(Path.ChangeExtension(media,".srt")))
                        return new string[] { "sub-track=0" };
                    else
                        return null;
                default:
                    return null;
            }
        }

        public static string GetMediaMRL(string media)
        {
            if (GetMediaType(media) == MediaType.Picture)
                return "fake:";
            else
                return media;
        }

    }

}
