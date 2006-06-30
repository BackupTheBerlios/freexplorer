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

        /// <remarks>
        /// can return null if no options !
        /// </remarks>
        public static string[] GetMediaMRLOptions(string media)
        {
            switch (GetMediaType(media))
            {
                case MediaType.Picture:
                    return new string[]
                    {
                        "fake-file=" + media,
                        "sout-transcode-vfilter=deinterlace",
                        "sout-deinterlace-mode=blend",
                        "sout-ffmpeg-keyint=8",
                    };
                case MediaType.Video:
                    if (File.Exists(Path.ChangeExtension(media, ".srt")))
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
