namespace Wizou.FreeXplorer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SubLanguage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.OkBtn = new System.Windows.Forms.Button();
            this.VLCPath = new System.Windows.Forms.TextBox();
            this.AudioLanguage = new System.Windows.Forms.TextBox();
            this.PictureExts = new System.Windows.Forms.TextBox();
            this.VlcPort = new System.Windows.Forms.TextBox();
            this.VideoExts = new System.Windows.Forms.TextBox();
            this.SoundExts = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.DVDLetter = new System.Windows.Forms.TextBox();
            this.ShowVLC = new System.Windows.Forms.CheckBox();
            this.RestartVLCBtn = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TranscodePC = new System.Windows.Forms.RadioButton();
            this.TranscodeA52 = new System.Windows.Forms.RadioButton();
            this.TranscodeMPGA = new System.Windows.Forms.RadioButton();
            this.TranscodeNone = new System.Windows.Forms.RadioButton();
            this.StartMinimized = new System.Windows.Forms.CheckBox();
            this.StartAtBoot = new System.Windows.Forms.CheckBox();
            this.MinimizeToTray = new System.Windows.Forms.CheckBox();
            this.FFMpegInterlace = new System.Windows.Forms.CheckBox();
            this.HalfScale = new System.Windows.Forms.CheckBox();
            this.LIRCActive = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TranscodeVB = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.PCControlAllowed = new System.Windows.Forms.CheckBox();
            this.LessIconsInExplorer = new System.Windows.Forms.CheckBox();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TrayConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.QuitBtn = new System.Windows.Forms.Button();
            this.BlackBkgnds = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.TrayContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(282, 216);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(253, 19);
            this.label8.TabIndex = 19;
            this.label8.Text = "(par ordre de préférence, séparées par une virgule)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.Location = new System.Drawing.Point(282, 235);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(253, 33);
            this.label9.TabIndex = 22;
            this.label9.Text = "(par ordre de préférence, séparées par une virgule,\nlaisser vide pour ne pas avoi" +
                "r de sous-titres)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SubLanguage
            // 
            this.SubLanguage.Location = new System.Drawing.Point(190, 242);
            this.SubLanguage.Name = "SubLanguage";
            this.SubLanguage.Size = new System.Drawing.Size(86, 20);
            this.SubLanguage.TabIndex = 21;
            this.SubLanguage.Text = "fr,en";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 19);
            this.label4.TabIndex = 6;
            this.label4.Text = "Extensions des vidéos :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 19);
            this.label5.TabIndex = 8;
            this.label5.Text = "Extensions des sons :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 19);
            this.label6.TabIndex = 10;
            this.label6.Text = "Extensions des images :";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(12, 216);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(172, 19);
            this.label7.TabIndex = 17;
            this.label7.Text = "Pistes audio :";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Emplacement de VLC :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port TCP local de contrôle de VLC:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 242);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(172, 19);
            this.label10.TabIndex = 20;
            this.label10.Text = "Langues des sous-titres :";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(11, 436);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(260, 26);
            this.label11.TabIndex = 32;
            this.label11.Text = "Certains réglages nécessitent de relancer VLC pour être pris en compte";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OkBtn
            // 
            this.OkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Location = new System.Drawing.Point(277, 436);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(82, 23);
            this.OkBtn.TabIndex = 33;
            this.OkBtn.Text = "Minimiser";
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // VLCPath
            // 
            this.VLCPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VLCPath.Location = new System.Drawing.Point(190, 9);
            this.VLCPath.Name = "VLCPath";
            this.VLCPath.Size = new System.Drawing.Size(345, 20);
            this.VLCPath.TabIndex = 1;
            this.VLCPath.Text = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe";
            // 
            // AudioLanguage
            // 
            this.AudioLanguage.Location = new System.Drawing.Point(190, 216);
            this.AudioLanguage.Name = "AudioLanguage";
            this.AudioLanguage.Size = new System.Drawing.Size(86, 20);
            this.AudioLanguage.TabIndex = 18;
            this.AudioLanguage.Text = "fr,en";
            // 
            // PictureExts
            // 
            this.PictureExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureExts.Location = new System.Drawing.Point(190, 139);
            this.PictureExts.Name = "PictureExts";
            this.PictureExts.Size = new System.Drawing.Size(345, 20);
            this.PictureExts.TabIndex = 11;
            // 
            // VlcPort
            // 
            this.VlcPort.Location = new System.Drawing.Point(190, 35);
            this.VlcPort.Name = "VlcPort";
            this.VlcPort.Size = new System.Drawing.Size(86, 20);
            this.VlcPort.TabIndex = 3;
            this.VlcPort.Text = "31186";
            // 
            // VideoExts
            // 
            this.VideoExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VideoExts.Location = new System.Drawing.Point(190, 87);
            this.VideoExts.Name = "VideoExts";
            this.VideoExts.Size = new System.Drawing.Size(345, 20);
            this.VideoExts.TabIndex = 7;
            // 
            // SoundExts
            // 
            this.SoundExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SoundExts.Location = new System.Drawing.Point(190, 113);
            this.SoundExts.Name = "SoundExts";
            this.SoundExts.Size = new System.Drawing.Size(345, 20);
            this.SoundExts.TabIndex = 9;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(12, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(172, 19);
            this.label12.TabIndex = 4;
            this.label12.Text = "Lettre du lecteur de DVD :";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DVDLetter
            // 
            this.DVDLetter.Location = new System.Drawing.Point(190, 61);
            this.DVDLetter.MaxLength = 1;
            this.DVDLetter.Name = "DVDLetter";
            this.DVDLetter.Size = new System.Drawing.Size(86, 20);
            this.DVDLetter.TabIndex = 5;
            this.DVDLetter.Text = "D";
            // 
            // ShowVLC
            // 
            this.ShowVLC.Location = new System.Drawing.Point(41, 296);
            this.ShowVLC.Margin = new System.Windows.Forms.Padding(1);
            this.ShowVLC.Name = "ShowVLC";
            this.ShowVLC.Size = new System.Drawing.Size(242, 17);
            this.ShowVLC.TabIndex = 25;
            this.ShowVLC.Text = "Afficher la fenêtre de VLC";
            this.ShowVLC.CheckedChanged += new System.EventHandler(this.ShowVLC_CheckedChanged);
            // 
            // RestartVLCBtn
            // 
            this.RestartVLCBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RestartVLCBtn.Location = new System.Drawing.Point(453, 436);
            this.RestartVLCBtn.Name = "RestartVLCBtn";
            this.RestartVLCBtn.Size = new System.Drawing.Size(82, 23);
            this.RestartVLCBtn.TabIndex = 35;
            this.RestartVLCBtn.Text = "Relancer VLC";
            this.RestartVLCBtn.Click += new System.EventHandler(this.RestartVLCBtn_Click);
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(12, 165);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(172, 19);
            this.label13.TabIndex = 12;
            this.label13.Text = "Transcodage audio :";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.TranscodePC);
            this.panel1.Controls.Add(this.TranscodeA52);
            this.panel1.Controls.Add(this.TranscodeMPGA);
            this.panel1.Controls.Add(this.TranscodeNone);
            this.panel1.Location = new System.Drawing.Point(190, 165);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(345, 19);
            this.panel1.TabIndex = 13;
            // 
            // TranscodePC
            // 
            this.TranscodePC.AutoSize = true;
            this.TranscodePC.Location = new System.Drawing.Point(258, 1);
            this.TranscodePC.Margin = new System.Windows.Forms.Padding(1);
            this.TranscodePC.Name = "TranscodePC";
            this.TranscodePC.Size = new System.Drawing.Size(86, 17);
            this.TranscodePC.TabIndex = 3;
            this.TranscodePC.Text = "Audio sur PC";
            // 
            // TranscodeA52
            // 
            this.TranscodeA52.AutoSize = true;
            this.TranscodeA52.Location = new System.Drawing.Point(159, 1);
            this.TranscodeA52.Margin = new System.Windows.Forms.Padding(1);
            this.TranscodeA52.Name = "TranscodeA52";
            this.TranscodeA52.Size = new System.Drawing.Size(97, 17);
            this.TranscodeA52.TabIndex = 2;
            this.TranscodeA52.Text = "A/52 Dolby 5.1";
            // 
            // TranscodeMPGA
            // 
            this.TranscodeMPGA.AutoSize = true;
            this.TranscodeMPGA.Checked = true;
            this.TranscodeMPGA.Location = new System.Drawing.Point(58, 1);
            this.TranscodeMPGA.Margin = new System.Windows.Forms.Padding(1);
            this.TranscodeMPGA.Name = "TranscodeMPGA";
            this.TranscodeMPGA.Size = new System.Drawing.Size(99, 17);
            this.TranscodeMPGA.TabIndex = 1;
            this.TranscodeMPGA.TabStop = true;
            this.TranscodeMPGA.Text = "MPEG-II Stéréo";
            // 
            // TranscodeNone
            // 
            this.TranscodeNone.AutoSize = true;
            this.TranscodeNone.Location = new System.Drawing.Point(0, 1);
            this.TranscodeNone.Margin = new System.Windows.Forms.Padding(1);
            this.TranscodeNone.Name = "TranscodeNone";
            this.TranscodeNone.Size = new System.Drawing.Size(56, 17);
            this.TranscodeNone.TabIndex = 0;
            this.TranscodeNone.Text = "Aucun";
            // 
            // StartMinimized
            // 
            this.StartMinimized.Checked = true;
            this.StartMinimized.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartMinimized.Location = new System.Drawing.Point(41, 277);
            this.StartMinimized.Margin = new System.Windows.Forms.Padding(1);
            this.StartMinimized.Name = "StartMinimized";
            this.StartMinimized.Size = new System.Drawing.Size(242, 17);
            this.StartMinimized.TabIndex = 23;
            this.StartMinimized.Text = "Minimiser au lancement";
            // 
            // StartAtBoot
            // 
            this.StartAtBoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.StartAtBoot.Location = new System.Drawing.Point(285, 296);
            this.StartAtBoot.Margin = new System.Windows.Forms.Padding(1);
            this.StartAtBoot.Name = "StartAtBoot";
            this.StartAtBoot.Size = new System.Drawing.Size(252, 17);
            this.StartAtBoot.TabIndex = 26;
            this.StartAtBoot.Text = "Lancer FreeXplorer au démarrage de Windows";
            this.StartAtBoot.CheckedChanged += new System.EventHandler(this.StartAtBoot_CheckedChanged);
            // 
            // MinimizeToTray
            // 
            this.MinimizeToTray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MinimizeToTray.Checked = true;
            this.MinimizeToTray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MinimizeToTray.Location = new System.Drawing.Point(285, 277);
            this.MinimizeToTray.Margin = new System.Windows.Forms.Padding(1);
            this.MinimizeToTray.Name = "MinimizeToTray";
            this.MinimizeToTray.Size = new System.Drawing.Size(252, 17);
            this.MinimizeToTray.TabIndex = 24;
            this.MinimizeToTray.Text = "Mini-icone près de l\'horloge lorsque minimisé";
            // 
            // FFMpegInterlace
            // 
            this.FFMpegInterlace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FFMpegInterlace.Location = new System.Drawing.Point(41, 346);
            this.FFMpegInterlace.Margin = new System.Windows.Forms.Padding(1);
            this.FFMpegInterlace.Name = "FFMpegInterlace";
            this.FFMpegInterlace.Size = new System.Drawing.Size(496, 17);
            this.FFMpegInterlace.TabIndex = 29;
            this.FFMpegInterlace.Text = "Suppression du scintillement vidéo (plus agréable pour les yeux mais consomme du " +
                "CPU)";
            // 
            // HalfScale
            // 
            this.HalfScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.HalfScale.Location = new System.Drawing.Point(41, 365);
            this.HalfScale.Margin = new System.Windows.Forms.Padding(1);
            this.HalfScale.Name = "HalfScale";
            this.HalfScale.Size = new System.Drawing.Size(496, 17);
            this.HalfScale.TabIndex = 30;
            this.HalfScale.Text = "Mode d\'économie de CPU sans baisse de qualité visible (traitement sur moins de pi" +
                "xels)";
            // 
            // LIRCActive
            // 
            this.LIRCActive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LIRCActive.Location = new System.Drawing.Point(285, 315);
            this.LIRCActive.Margin = new System.Windows.Forms.Padding(1);
            this.LIRCActive.Name = "LIRCActive";
            this.LIRCActive.Size = new System.Drawing.Size(252, 17);
            this.LIRCActive.TabIndex = 28;
            this.LIRCActive.Text = "Activer le serveur LIRC";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 19);
            this.label3.TabIndex = 14;
            this.label3.Text = "Débit vidéo (en kbit/s) :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TranscodeVB
            // 
            this.TranscodeVB.Location = new System.Drawing.Point(190, 190);
            this.TranscodeVB.Name = "TranscodeVB";
            this.TranscodeVB.Size = new System.Drawing.Size(86, 20);
            this.TranscodeVB.TabIndex = 15;
            this.TranscodeVB.Text = "8000";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.Location = new System.Drawing.Point(282, 190);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(253, 19);
            this.label14.TabIndex = 16;
            this.label14.Text = "(ne modifier que si votre réseau ne le supporte pas)";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PCControlAllowed
            // 
            this.PCControlAllowed.Checked = true;
            this.PCControlAllowed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PCControlAllowed.Location = new System.Drawing.Point(41, 315);
            this.PCControlAllowed.Margin = new System.Windows.Forms.Padding(1);
            this.PCControlAllowed.Name = "PCControlAllowed";
            this.PCControlAllowed.Size = new System.Drawing.Size(242, 17);
            this.PCControlAllowed.TabIndex = 27;
            this.PCControlAllowed.Text = "Autoriser le contrôle du PC par la Freebox";
            this.PCControlAllowed.CheckedChanged += new System.EventHandler(this.PCControlAllowed_CheckedChanged);
            // 
            // LessIconsInExplorer
            // 
            this.LessIconsInExplorer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LessIconsInExplorer.Location = new System.Drawing.Point(41, 384);
            this.LessIconsInExplorer.Margin = new System.Windows.Forms.Padding(1);
            this.LessIconsInExplorer.Name = "LessIconsInExplorer";
            this.LessIconsInExplorer.Size = new System.Drawing.Size(496, 17);
            this.LessIconsInExplorer.TabIndex = 31;
            this.LessIconsInExplorer.Text = "Afficher moins d\'icônes dans l\'explorateur (accélère l\'affichage des pages)";
            this.LessIconsInExplorer.CheckedChanged += new System.EventHandler(this.LessIconsInExplorer_CheckedChanged);
            // 
            // TrayIcon
            // 
            this.TrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.TrayIcon.BalloonTipText = "Entrez dans l\'univers Freeplayer sur votre Freebox pour y accéder...\r\nDouble-cliq" +
                "uez sur l\'icone pour afficher la fenêtre de configuration de FreeXplorer";
            this.TrayIcon.BalloonTipTitle = "FreeXplorer {0} est actif !";
            this.TrayIcon.ContextMenuStrip = this.TrayContextMenu;
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "FreeXplorer {0} actif";
            this.TrayIcon.DoubleClick += new System.EventHandler(this.TrayConfig_Click);
            // 
            // TrayContextMenu
            // 
            this.TrayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TrayConfig,
            this.TrayQuit});
            this.TrayContextMenu.Name = "TrayContextMenu";
            this.TrayContextMenu.Size = new System.Drawing.Size(180, 48);
            // 
            // TrayConfig
            // 
            this.TrayConfig.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.TrayConfig.Name = "TrayConfig";
            this.TrayConfig.Size = new System.Drawing.Size(179, 22);
            this.TrayConfig.Text = "Configuration";
            this.TrayConfig.Click += new System.EventHandler(this.TrayConfig_Click);
            // 
            // TrayQuit
            // 
            this.TrayQuit.Name = "TrayQuit";
            this.TrayQuit.Size = new System.Drawing.Size(179, 22);
            this.TrayQuit.Text = "Arrêter FreeXplorer";
            this.TrayQuit.Click += new System.EventHandler(this.QuitBtn_Click);
            // 
            // QuitBtn
            // 
            this.QuitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.QuitBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.QuitBtn.Location = new System.Drawing.Point(365, 436);
            this.QuitBtn.Name = "QuitBtn";
            this.QuitBtn.Size = new System.Drawing.Size(82, 23);
            this.QuitBtn.TabIndex = 34;
            this.QuitBtn.Text = "Quitter";
            this.QuitBtn.Click += new System.EventHandler(this.QuitBtn_Click);
            // 
            // BlackBkgnds
            // 
            this.BlackBkgnds.Location = new System.Drawing.Point(41, 403);
            this.BlackBkgnds.Margin = new System.Windows.Forms.Padding(1);
            this.BlackBkgnds.Name = "BlackBkgnds";
            this.BlackBkgnds.Size = new System.Drawing.Size(496, 17);
            this.BlackBkgnds.TabIndex = 36;
            this.BlackBkgnds.Text = "Anti-brulûre écrans plasma : Remplace les fonds fixes (ex: Webradios) par du noir" +
                "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 471);
            this.Controls.Add(this.QuitBtn);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TranscodeVB);
            this.Controls.Add(this.LessIconsInExplorer);
            this.Controls.Add(this.PCControlAllowed);
            this.Controls.Add(this.BlackBkgnds);
            this.Controls.Add(this.StartMinimized);
            this.Controls.Add(this.LIRCActive);
            this.Controls.Add(this.FFMpegInterlace);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.HalfScale);
            this.Controls.Add(this.MinimizeToTray);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.StartAtBoot);
            this.Controls.Add(this.DVDLetter);
            this.Controls.Add(this.ShowVLC);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.RestartVLCBtn);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SubLanguage);
            this.Controls.Add(this.AudioLanguage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.PictureExts);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SoundExts);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.VideoExts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VlcPort);
            this.Controls.Add(this.VLCPath);
            this.Controls.Add(this.OkBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Configuration de FreeXplorer {0}";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.TrayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox SoundExts;
        internal System.Windows.Forms.TextBox VideoExts;
        internal System.Windows.Forms.TextBox VlcPort;
        internal System.Windows.Forms.TextBox PictureExts;
        internal System.Windows.Forms.TextBox AudioLanguage;
        internal System.Windows.Forms.TextBox VLCPath;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.TextBox SubLanguage;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        internal System.Windows.Forms.TextBox DVDLetter;
        private System.Windows.Forms.Button RestartVLCBtn;
        internal System.Windows.Forms.CheckBox ShowVLC;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton TranscodeNone;
        private System.Windows.Forms.RadioButton TranscodeA52;
        private System.Windows.Forms.RadioButton TranscodeMPGA;
        internal System.Windows.Forms.CheckBox StartMinimized;
        internal System.Windows.Forms.CheckBox StartAtBoot;
        internal System.Windows.Forms.CheckBox MinimizeToTray;
        private System.Windows.Forms.CheckBox FFMpegInterlace;
        private System.Windows.Forms.CheckBox HalfScale;
        internal System.Windows.Forms.CheckBox LIRCActive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TranscodeVB;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RadioButton TranscodePC;
        private System.Windows.Forms.CheckBox PCControlAllowed;
        private System.Windows.Forms.CheckBox LessIconsInExplorer;
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem TrayConfig;
        private System.Windows.Forms.ToolStripMenuItem TrayQuit;
        private System.Windows.Forms.Button QuitBtn;
        private System.Windows.Forms.CheckBox BlackBkgnds;
    }
}