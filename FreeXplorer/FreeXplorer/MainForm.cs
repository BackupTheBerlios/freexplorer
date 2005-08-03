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
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Wizou.VLC;
using Wizou.HTTP;

namespace Wizou.FreeXplorer
{
    public partial class MainForm : Form
    {
        private ConfigForm configForm;
        private VLCApp vlcApp = new VLCApp();
        private BasicHttpServer freeboxServer;
        private NotifyIcon notifyIcon;
        private LIRC.LIRCServer lircServer = new LIRC.LIRCServer();

        private delegate void DelegateWithString(string text);

        public MainForm()
        {
            InitializeComponent();
            Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Text = String.Format(Text, appVersion.ToString(appVersion.Build == 0 ? 2 : 3));
#if DEBUG
            Text = "Debug";
#endif
            configForm = new ConfigForm(this, vlcApp);

            notifyIcon = new NotifyIcon(components);
            notifyIcon.ContextMenuStrip = TrayContextMenu;
            notifyIcon.Icon = Icon;
            notifyIcon.Text = Text;
            notifyIcon.DoubleClick += new System.EventHandler(TrayRestore_Click);

            if (configForm.StartMinimized.Checked) WindowState = FormWindowState.Minimized;

            try
            {
                vlcApp.Start();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                MessageBox.Show("Impossible de lancer l'executable vlc.exe\r\n\r\n" +
                                e.Message + "\r\n\r\n" +
                                "Verifiez la configuration", "Initialisation",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (VLCException e)
            {
                MessageBox.Show("Problème au lancement de vlc.exe :\r\n\r\n" +
                                e.Message + "\r\n\r\n" +
                                "Réessayez de lancer FreeXplorer", "Initialisation",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lircServer.Active = configForm.LIRCActive.Checked;
            
            // lancer VLC avant le TcpListener pour que le process de VLC n'hérite pas du handle et le conserve ouvert
            IPHostEntry freeboxIP;
            try
            {
                freeboxIP = Dns.GetHostEntry("freeplayer.freebox.fr");
                FreeboxIP.Text = string.Format("Adresse IP de la Freebox: " + freeboxIP.AddressList[0]);
            }
            catch (SocketException)
            {
                freeboxIP = new IPHostEntry();
                MessageBox.Show("Impossible de résoudre l'adresse IP de la Freebox\r\n" +
                                "Verifiez la configuration", "Initialisation",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                freeboxServer = new FreeboxServer(Path.Combine(Application.StartupPath, "pages"), freeboxIP.AddressList[0], vlcApp, lircServer);
                freeboxServer.Start();
            }
            catch (SocketException)
            {
                throw new Exception("Le port 8080 de cette machine est déjà occupé !\r\n" +
                                "Vérifiez que FreeXplorer, VLC, un autre Freeplayer ou un serveur proxy n'est pas déjà actif");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm.SaveConfig();
            if (freeboxServer != null)
                freeboxServer.Stop();
            lircServer.Stop();
            vlcApp.Stop();
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            configForm.ShowDialog(this);
            lircServer.Active = configForm.LIRCActive.Checked;
        }


        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if ((WindowState == FormWindowState.Minimized))
            {
                if (configForm.MinimizeToTray.Checked)
                {
                    Hide();
                    notifyIcon.Visible = true;
                }
            }
            else if (notifyIcon.Visible)
            {
                notifyIcon.Visible = false;
                Show();
            }
        }
        
        private void TrayRestore_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void TrayQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            MainForm_SizeChanged(sender, e);
        }

    }
}
