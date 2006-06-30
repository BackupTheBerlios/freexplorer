/*
 * FreeXplorer - Interface type Freeplayer de pilotage du PC et de VLC depuis une Freebox
 * Copyright (C) 2005-2006 Olivier Marcoux (freexplorer@free.fr / http://freexplorer.free.fr)
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
        const string STARTMENU_LINK_NAME = "FreeXplorer.lnk";

        FreeXplorer m_fxp;

        public MainForm()
        {
            InitializeComponent();

            TrayIcon.Text = String.Format(TrayIcon.Text, Program.appVersionText); ;
            TrayIcon.BalloonTipTitle = String.Format(TrayIcon.BalloonTipTitle, Program.appVersionText); ;
            Text = String.Format(Text, Program.appVersionText);
#if DEBUG
            Text = "Debug";
#endif
            m_fxp = new FreeXplorer();

            if (m_fxp.Configuration.StartMinimized)
            {
                WindowState = FormWindowState.Minimized;
                VLCPath.Select(0,0); // sinon le champ etait sélectionné bizarrement (scrollé)
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_fxp.Dispose();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            MainForm_SizeChanged(null, e);
            if ((WindowState == FormWindowState.Minimized) && !Program.autostart)
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
            Configuration config = m_fxp.Configuration;
            config.Load();

            // chargement des valeurs depuis le Config.xml
            VLCPath.Text = config.VLCPath;
            DVDLetter.Text = config.DVDLetter;
            VlcPort.Text = config.VlcPort;
            SoundExts.Text = config.SoundExts;
            PictureExts.Text = config.PictureExts;
            VideoExts.Text = config.VideoExts;
            AudioLanguage.Text = config.AudioLanguage;
            SubLanguage.Text = config.SubLanguage;
            ShowVLC.Checked = config.ShowVLC;

            TranscodeMPGA.Checked = (config.Transcode == AudioTranscode.MPGA);
            TranscodeA52.Checked = (config.Transcode == AudioTranscode.A52);
            TranscodePC.Checked = (config.Transcode == AudioTranscode.PC);
            TranscodeNone.Checked = (config.Transcode == AudioTranscode.None);

            StartMinimized.Checked = config.StartMinimized;
            MinimizeToTray.Checked = config.MinimizeToTray;
            FFMpegInterlace.Checked = config.FFMpegInterlace;
            HalfScale.Checked = config.HalfScale;
            LIRCActive.Checked = config.LIRCActive;
            TranscodeVB.Text = config.TranscodeVB;
            PCControlAllowed.Checked = config.PCControlAllowed;
            LessIconsInExplorer.Checked = config.LessIconsInExplorer;
            BlackBkgnds.Checked = config.BlackBkgnds;
           
            // chargement des valeurs des options qui ne sont pas issues de Config.xml
            StartAtBoot.Checked = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "FreeXplorer.lnk"));
        }

        public void SaveConfig()
        {
            Configuration config = m_fxp.Configuration;

            config.VLCPath = VLCPath.Text;
            config.VlcPort = VlcPort.Text;
            config.DVDLetter = DVDLetter.Text;
            config.SoundExts = SoundExts.Text;
            config.PictureExts = PictureExts.Text;
            config.VideoExts = VideoExts.Text;
            config.AudioLanguage = AudioLanguage.Text;
            config.SubLanguage = SubLanguage.Text;
            config.ShowVLC = ShowVLC.Checked;

            config.Transcode = TranscodeMPGA.Checked ? AudioTranscode.MPGA :
                               TranscodeA52.Checked ? AudioTranscode.A52 :
                               TranscodePC.Checked ? AudioTranscode.PC : 
                               AudioTranscode.None;

            config.StartMinimized = StartMinimized.Checked;
            config.MinimizeToTray = MinimizeToTray.Checked;
            config.FFMpegInterlace = FFMpegInterlace.Checked;
            config.HalfScale = HalfScale.Checked;
            config.LIRCActive = LIRCActive.Checked;
            config.TranscodeVB = TranscodeVB.Text;
            config.PCControlAllowed = PCControlAllowed.Checked;
            config.LessIconsInExplorer = LessIconsInExplorer.Checked;
            config.BlackBkgnds = BlackBkgnds.Checked;

            config.Save();
        }

        private void ApplyConfig(bool restartVLC)
        {
            SaveConfig();
            try
            {
                m_fxp.ApplyConfig(restartVLC);
            }
            catch (VLCLaunchException e)
            {
                MessageBox.Show(e.Message, "Initialisation",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            /*
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
            freeboxServer.BlackBkgnds = BlackBkgnds.Checked;

            try
            {
                freeboxServer.Start();
            }
            catch (SocketException)
            {
                throw new Exception("Le port 8080 de cette machine est déjà occupé !\r\n" +
                                "Vérifiez que FreeXplorer, VLC, un autre Freeplayer ou un serveur proxy n'est pas déjà actif");
            }
            */
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
        /*
        public Wizou.VLC.AudioTranscode GetAudioTranscode()
        {
            return TranscodeMPGA.Checked ? Wizou.VLC.AudioTranscode.MPGA :
                    TranscodeA52.Checked ? VLC.AudioTranscode.A52 :
                    TranscodePC.Checked ? VLC.AudioTranscode.PC :
                                            VLC.AudioTranscode.None;
        }*/

        private void ShowVLC_CheckedChanged(object sender, EventArgs e)
        {
            m_fxp.VLCVisible = ShowVLC.Checked;
        }

        private void StartAtBoot_CheckedChanged(object sender, EventArgs e)
        {
            if (!Created) return;
            if (StartAtBoot.Checked)
                File.Copy(Path.Combine(Application.StartupPath, STARTMENU_LINK_NAME),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), STARTMENU_LINK_NAME));
            else
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), STARTMENU_LINK_NAME));
        }

        private void PCControlAllowed_CheckedChanged(object sender, EventArgs e)
        {
            m_fxp.FreeboxServer.PCControlAllowed = PCControlAllowed.Checked;
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
