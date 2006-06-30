using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Wizou.VLC;

namespace Wizou.FreeXplorer
{
    public class Configuration
    {
        static string ConfigFile
        { 
            get { return Path.Combine(FreeXplorer.ConfigurationFolder, "config.xml"); }
        }

        public string VLCPath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
        public string DVDLetter = "D";
        public string VlcPort = "31186";
        public string SoundExts = VLC.Utility.SoundExts;
        public string PictureExts = VLC.Utility.PictureExts;
        public string VideoExts = VLC.Utility.VideoExts;
        public string AudioLanguage = "fr,en";
        public string SubLanguage = "fr,en";
        public bool ShowVLC = false;
        public AudioTranscode Transcode = AudioTranscode.MPGA;
        public bool StartMinimized = true;
        public bool MinimizeToTray = true;
        public bool FFMpegInterlace = false;
        public bool HalfScale = false;
        public bool LIRCActive = false;
        public string TranscodeVB = "8000";
        public bool PCControlAllowed = true;
        public bool LessIconsInExplorer = false;
        public bool BlackBkgnds = false;

        public void Load()
        {
            Load(ConfigFile);
        }

        void Load(string fileName)
        {
            // chargement des valeurs des options depuis le fichier Config.xml
            try
            {
                using (XmlTextReader reader = new XmlTextReader(fileName))
                {
                    reader.ReadStartElement("Config");
                    do
                    {
                        if (!reader.IsStartElement()) continue;
                        string value = reader.ReadString();
                        switch (reader.Name)
                        {
                            case "VLCPath": VLCPath = value; break;
                            case "DVDLetter": DVDLetter = value; break;
                            case "VlcPort": VlcPort = value; break;
                            case "SoundExts": SoundExts = value; break;
                            case "PictureExts": PictureExts = value; break;
                            case "VideoExts": VideoExts = value; break;
                            case "AudioLanguage": AudioLanguage = value; break;
                            case "SubLanguage": SubLanguage = value; break;
                            case "ShowVLC": ShowVLC = (value == "1") || (value == System.Boolean.TrueString); break;
                            case "Transcode":
                                switch (value.ToUpper())
                                {
                                    case "MPGA": Transcode = AudioTranscode.MPGA; break;
                                    case "A52": Transcode = AudioTranscode.A52; break;
                                    case "PC": Transcode = AudioTranscode.PC; break;
                                    default: Transcode = AudioTranscode.None; break;
                                }
                                break;
                            case "StartMinimized": StartMinimized = (value == "1") || (value == System.Boolean.TrueString); break;
                            case "MinimizeToTray": MinimizeToTray = (value == "1") || (value == System.Boolean.TrueString); break;
                            case "FFMpegInterlace": FFMpegInterlace = (value == "1") || (value == System.Boolean.TrueString); break;
                            case "HalfScale": HalfScale = (value == "1") || (value == System.Boolean.TrueString); break;
                            case "LIRCActive": LIRCActive = (value == "1") || (value == System.Boolean.TrueString); break;
                            case "TranscodeVB": TranscodeVB = value; break;
                            case "PCControlAllowed": PCControlAllowed = Convert.ToBoolean(value); break;
                            case "LessIconsInExplorer": LessIconsInExplorer = Convert.ToBoolean(value); break;
                            case "BlackBkgnds": BlackBkgnds = Convert.ToBoolean(value); break;
                        }
                    } while (reader.Read());
                }
            }
            catch (FileNotFoundException) { }
        }

        public void Save()
        {
            Save(ConfigFile);
        }

        void Save(string fileName)
        {
            using (XmlTextWriter writer = new XmlTextWriter(fileName, null))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Config");
                writer.WriteElementString("VLCPath", VLCPath);
                writer.WriteElementString("VlcPort", VlcPort);
                writer.WriteElementString("DVDLetter", DVDLetter);
                writer.WriteElementString("SoundExts", SoundExts);
                writer.WriteElementString("PictureExts", PictureExts);
                writer.WriteElementString("VideoExts", VideoExts);
                writer.WriteElementString("AudioLanguage", AudioLanguage);
                writer.WriteElementString("SubLanguage", SubLanguage);
                writer.WriteElementString("ShowVLC", ShowVLC.ToString());
                writer.WriteElementString("Transcode", Transcode.ToString());
                writer.WriteElementString("StartMinimized", StartMinimized.ToString());
                writer.WriteElementString("MinimizeToTray", MinimizeToTray.ToString());
                writer.WriteElementString("FFMpegInterlace", FFMpegInterlace.ToString());
                writer.WriteElementString("HalfScale", HalfScale.ToString());
                writer.WriteElementString("LIRCActive", LIRCActive.ToString());
                writer.WriteElementString("TranscodeVB", TranscodeVB);
                writer.WriteElementString("PCControlAllowed", PCControlAllowed.ToString());
                writer.WriteElementString("LessIconsInExplorer", LessIconsInExplorer.ToString());
                writer.WriteElementString("BlackBkgnds", BlackBkgnds.ToString());
            }
        }
    }
}
