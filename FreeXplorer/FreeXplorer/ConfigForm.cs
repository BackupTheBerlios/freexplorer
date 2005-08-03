/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 * Copyright (C) 2005 Olivier Marcoux (wiz0u@free.fr / http://wiz0u.free.fr/freexplorer)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace Wizou.FreeXplorer
{
    public partial class ConfigForm : Form
    {
        MainForm mainForm;
        VLC.VLCApp vlcApp;

        public VLC.AudioTranscode audioTranscode
        {
            get
            {
                return TranscodeMPGA.Checked ? VLC.AudioTranscode.MPGA :
                        TranscodeA52.Checked ? VLC.AudioTranscode.A52 : VLC.AudioTranscode.None;
            }
        }


        public ConfigForm(MainForm mainForm, VLC.VLCApp vlcApp)
        {
            this.mainForm = mainForm;
            this.vlcApp = vlcApp;
            InitializeComponent();
            SoundExts.Text = VLC.Utility.SoundExts;
            PictureExts.Text = VLC.Utility.PictureExts;
            VideoExts.Text = VLC.Utility.VideoExts;
            LoadConfig();
            SetVlcConfig();
        }

        private void SetVlcConfig()
        {
            vlcApp.RC_host_port = Convert.ToInt32(VlcPort.Text);
            vlcApp.DVDLetter = DVDLetter.Text[0];
            vlcApp.AudioTranscode = audioTranscode;
            vlcApp.SetConfig(VLCPath.Text, AudioLanguage.Text, SubLanguage.Text, FFMpegInterlace.Checked, HalfScale.Checked ? 0.5 : 1.0, Convert.ToInt32(TranscodeVB.Text));
        }

        public void LoadConfig()
        {
            XmlTextReader reader = new XmlTextReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/config.xml"));
            reader.ReadStartElement("Config");
            do
            {
                if (reader.IsStartElement())
                {
                    string value = reader.ReadString();
                    switch (reader.Name)
                    {
                        case "VLCPath": VLCPath.Text = value; break;
                        case "DVDLetter": DVDLetter.Text = value; break;
                        case "VlcPort": VlcPort.Text = value; break;
                        case "SoundExts": SoundExts.Text = value; break;
                        case "PictureExts": PictureExts.Text = value; break;
                        case "VideoExts": VideoExts.Text = value; break;
                        case "AudioLanguage": AudioLanguage.Text = value; break;
                        case "SubLanguage": SubLanguage.Text = value; break;
                        case "ShowVLC": ShowVLC.Checked = (value == "1"); break;
                        case "Transcode":
                            switch (value.ToUpper())
                            {
                                case "MPGA": TranscodeMPGA.Checked = true; break;
                                case "A52": TranscodeA52.Checked = true; break;
                                default: TranscodeNone.Checked = true; break;
                            }
                            break;
                        case "StartMinimized": StartMinimized.Checked = (value == "1"); break;
                        case "MinimizeToTray": MinimizeToTray.Checked = (value == "1"); break;
                        case "FFMpegInterlace": FFMpegInterlace.Checked = (value == "1"); break;
                        case "HalfScale": HalfScale.Checked = (value == "1"); break;
                        case "LIRCActive": LIRCActive.Checked = (value == "1"); break;
                        case "TranscodeVB": TranscodeVB.Text = value; break;
                    }
                }
            } while (reader.Read());
            reader.Close();
            StartAtBoot.Checked = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "FreeXplorer.lnk"));
            VLC.Utility.SoundExts = SoundExts.Text;
            VLC.Utility.PictureExts = PictureExts.Text;
            VLC.Utility.VideoExts = VideoExts.Text;
        }

        public void SaveConfig()
        {
            XmlTextWriter writer = new XmlTextWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FreeXplorer/config.xml"), null);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("Config");
            writer.WriteElementString("VLCPath", VLCPath.Text);
            writer.WriteElementString("VlcPort", VlcPort.Text);
            writer.WriteElementString("DVDLetter", DVDLetter.Text);
            writer.WriteElementString("SoundExts", SoundExts.Text);
            writer.WriteElementString("PictureExts", PictureExts.Text);
            writer.WriteElementString("VideoExts", VideoExts.Text);
            writer.WriteElementString("AudioLanguage", AudioLanguage.Text);
            writer.WriteElementString("SubLanguage", SubLanguage.Text);
            writer.WriteElementString("ShowVLC", ShowVLC.Checked ? "1" : "0");
            writer.WriteElementString("Transcode",  TranscodeMPGA.Checked   ? "MPGA" :
                                                    TranscodeA52.Checked    ? "A52" : "NONE");
            writer.WriteElementString("StartMinimized", StartMinimized.Checked ? "1" : "0");
            writer.WriteElementString("MinimizeToTray", MinimizeToTray.Checked ? "1" : "0");
            writer.WriteElementString("FFMpegInterlace", FFMpegInterlace.Checked ? "1" : "0");
            writer.WriteElementString("HalfScale", HalfScale.Checked ? "1" : "0");
            writer.WriteElementString("LIRCActive", LIRCActive.Checked ? "1" : "0");
            writer.WriteElementString("TranscodeVB", TranscodeVB.Text);
            
            
            
            writer.Close();
            VLC.Utility.SoundExts = SoundExts.Text;
            VLC.Utility.PictureExts = PictureExts.Text;
            VLC.Utility.VideoExts = VideoExts.Text;
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }


        private void ShowVLC_CheckedChanged(object sender, EventArgs e)
        {
            vlcApp.ShowWindow = ShowVLC.Checked;
        }

        private void RestartVLC_Click(object sender, EventArgs e)
        {
            vlcApp.Stop();
            SetVlcConfig();
            vlcApp.Start();
        }

        private void StartAtBoot_CheckedChanged(object sender, EventArgs e)
        {
            if (!Created) return;
            if (StartAtBoot.Checked)
                File.Copy(Path.Combine(Application.StartupPath, "FreeXplorer.lnk"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "FreeXplorer.lnk"));
            else
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "FreeXplorer.lnk"));
        }
    }
}