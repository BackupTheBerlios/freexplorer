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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Wizou.VLC;
using Wizou.HTTP;
using System.Net;
using System.Net.Sockets;

namespace Wizou.FreeXplorer
{
    public partial class MainForm : Form
    {
        private VLCApp vlcApp = new VLCApp();
        private FreeboxServer freeboxServer = new FreeboxServer(Path.Combine(Application.StartupPath, "pages"));
        private LIRC.LIRCServer lircServer = new LIRC.LIRCServer();

        public MainForm()
        {
            InitializeComponent();
            TrayIcon.Text = String.Format(TrayIcon.Text, Program.appVersionText); ;
            TrayIcon.BalloonTipTitle = String.Format(TrayIcon.BalloonTipTitle, Program.appVersionText); ;
            Text = String.Format(Text, Program.appVersionText);
#if DEBUG
            Text = "Debug";
#endif

            IPHostEntry freeboxIP;
            try
            {
                freeboxIP = Dns.GetHostEntry("freeplayer.freebox.fr");
            }
            catch (SocketException)
            {
                freeboxIP = new IPHostEntry();
                MessageBox.Show("Impossible de résoudre l'adresse IP de la Freebox\r\n" +
                                "Verifiez la configuration", "Initialisation",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (freeboxIP.AddressList != null) 
                freeboxServer.Init(freeboxIP.AddressList[0], vlcApp, lircServer);

            LoadConfig();
            ApplyConfig(true);
            if (StartMinimized.Checked)
            {
                WindowState = FormWindowState.Minimized;
                VLCPath.Select(0,0); // sinon le champ etait sélectionné bizarrement (scrollé)
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConfig();
            // arrets des serveurs TCP:
            freeboxServer.Stop();
            lircServer.Stop();
            vlcApp.Stop();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            MainForm_SizeChanged(null, e);
            if (WindowState == FormWindowState.Minimized)
                TrayIcon.ShowBalloonTip(5);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (Visible == false) return;
            if ((WindowState == FormWindowState.Minimized))
            {
                if (sender != null) // c'est un minimize utilisateur
                    ApplyConfig(false);

                if (MinimizeToTray.Checked)
                {
                    Hide();
                    TrayIcon.Visible = true;
                }
            }
            else if (TrayIcon.Visible)
            {
                TrayIcon.Visible = false;
                Show();
            }
        }

        public void LoadConfig()
        {
            // préchargement des valeurs par défaut des options
            SoundExts.Text = VLC.Utility.SoundExts;
            PictureExts.Text = VLC.Utility.PictureExts;
            VideoExts.Text = VLC.Utility.VideoExts;

            // chargement des valeurs des options depuis le fichier Config.xml
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
                        case "ShowVLC": ShowVLC.Checked = (value == "1") || (value == System.Boolean.TrueString); break;
                        case "Transcode":
                            switch (value.ToUpper())
                            {
                                case "MPGA": TranscodeMPGA.Checked = true; break;
                                case "A52": TranscodeA52.Checked = true; break;
                                case "PC": TranscodePC.Checked = true; break;
                                default: TranscodeNone.Checked = true; break;
                            }
                            break;
                        case "StartMinimized": StartMinimized.Checked = (value == "1") || (value == System.Boolean.TrueString); break;
                        case "MinimizeToTray": MinimizeToTray.Checked = (value == "1") || (value == System.Boolean.TrueString); break;
                        case "FFMpegInterlace": FFMpegInterlace.Checked = (value == "1") || (value == System.Boolean.TrueString); break;
                        case "HalfScale": HalfScale.Checked = (value == "1") || (value == System.Boolean.TrueString); break;
                        case "LIRCActive": LIRCActive.Checked = (value == "1") || (value == System.Boolean.TrueString); break;
                        case "TranscodeVB": TranscodeVB.Text = value; break;
                        case "PCControlAllowed": PCControlAllowed.Checked = Convert.ToBoolean(value); break;
                        case "LessIconsInExplorer": LessIconsInExplorer.Checked = Convert.ToBoolean(value); break;
                    }
                }
            } while (reader.Read());
            reader.Close();

            // chargement des valeurs des options qui ne sont pas issues de Config.xml
            StartAtBoot.Checked = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "FreeXplorer.lnk"));
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
            writer.WriteElementString("ShowVLC", ShowVLC.Checked.ToString());
            writer.WriteElementString("Transcode",  TranscodeMPGA.Checked   ? "MPGA" :
                                                    TranscodeA52.Checked ? "A52" :
                                                    TranscodePC.Checked ? "PC" : 
                                                                           "NONE");
            writer.WriteElementString("StartMinimized", StartMinimized.Checked.ToString());
            writer.WriteElementString("MinimizeToTray", MinimizeToTray.Checked.ToString());
            writer.WriteElementString("FFMpegInterlace", FFMpegInterlace.Checked.ToString());
            writer.WriteElementString("HalfScale", HalfScale.Checked.ToString());
            writer.WriteElementString("LIRCActive", LIRCActive.Checked.ToString());
            writer.WriteElementString("TranscodeVB", TranscodeVB.Text);
            writer.WriteElementString("PCControlAllowed", PCControlAllowed.Checked.ToString());
            writer.WriteElementString("LessIconsInExplorer", LessIconsInExplorer.Checked.ToString());
            
            writer.Close();
        }

        private void ApplyConfig(bool restartVLC)
        {
            SaveConfig();

            // arrets des serveurs TCP
            freeboxServer.Stop();
            lircServer.Stop();

            // arret eventuel de VLC
            if (restartVLC) vlcApp.Stop();

            // application des paramètres
            VLC.Utility.SoundExts = SoundExts.Text;
            VLC.Utility.PictureExts = PictureExts.Text;
            VLC.Utility.VideoExts = VideoExts.Text;

            if (restartVLC)
            try
            {
                vlcApp.RC_host_port = Convert.ToInt32(VlcPort.Text);
                vlcApp.DVDLetter = DVDLetter.Text[0];
                vlcApp.AudioTranscode = GetAudioTranscode();
                vlcApp.SetConfig(VLCPath.Text, AudioLanguage.Text, SubLanguage.Text, FFMpegInterlace.Checked, HalfScale.Checked ? 0.5 : 1.0, Convert.ToInt32(TranscodeVB.Text));
                vlcApp.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show("Impossible de lancer l'executable vlc.exe\r\n\r\n" +
                                ex.Message + "\r\n\r\n" +
                                "Verifiez la configuration", "Initialisation",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (VLCException ex)
            {
                MessageBox.Show("Problème au lancement de vlc.exe :\r\n\r\n" +
                                ex.Message + "\r\n\r\n" +
                                "Réessayez de lancer FreeXplorer", "Initialisation",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // activation eventuelle des serveurs TCP
            lircServer.Active = LIRCActive.Checked;
            freeboxServer.PCControlAllowed = PCControlAllowed.Checked;

            try
            {
                freeboxServer.Start();
            }
            catch (SocketException)
            {
                throw new Exception("Le port 8080 de cette machine est déjà occupé !\r\n" +
                                "Vérifiez que FreeXplorer, VLC, un autre Freeplayer ou un serveur proxy n'est pas déjà actif");
            }
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void RestartVLCBtn_Click(object sender, EventArgs e)
        {
            ApplyConfig(true);
        }
        
        private void QuitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TrayConfig_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        public VLC.AudioTranscode GetAudioTranscode()
        {
            return TranscodeMPGA.Checked ? VLC.AudioTranscode.MPGA :
                    TranscodeA52.Checked ? VLC.AudioTranscode.A52 :
                    TranscodePC.Checked ? VLC.AudioTranscode.PC :
                                            VLC.AudioTranscode.None;
        }

        private void ShowVLC_CheckedChanged(object sender, EventArgs e)
        {
            vlcApp.ShowWindow = ShowVLC.Checked;
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

        private void PCControlAllowed_CheckedChanged(object sender, EventArgs e)
        {
            freeboxServer.PCControlAllowed = PCControlAllowed.Checked;
            if (!PCControlAllowed.Checked)
                LIRCActive.Checked = false;
            LIRCActive.Enabled = PCControlAllowed.Checked;
        }

        private void LessIconsInExplorer_CheckedChanged(object sender, EventArgs e)
        {
            Helper.LessIconsInExplorer = LessIconsInExplorer.Checked;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("Voulez-vous vraiment quitter FreeXplorer ?", "FreeXplorer",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
                {
                    e.Cancel = true;
                    WindowState = FormWindowState.Minimized;
                    if (WindowState == FormWindowState.Minimized)
                        TrayIcon.ShowBalloonTip(5);
                }
            }
        }

    }
}
