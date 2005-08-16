namespace Wizou.FreeXplorer
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SubLanguage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.VLCPath = new System.Windows.Forms.TextBox();
            this.AudioLanguage = new System.Windows.Forms.TextBox();
            this.PictureExts = new System.Windows.Forms.TextBox();
            this.VlcPort = new System.Windows.Forms.TextBox();
            this.VideoExts = new System.Windows.Forms.TextBox();
            this.SoundExts = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.DVDLetter = new System.Windows.Forms.TextBox();
            this.ShowVLC = new System.Windows.Forms.CheckBox();
            this.RestartVLC = new System.Windows.Forms.Button();
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
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(282, 216);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(248, 19);
            this.label8.TabIndex = 30;
            this.label8.Text = "(par ordre de préférence, séparées par une virgule)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.Location = new System.Drawing.Point(282, 235);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(248, 33);
            this.label9.TabIndex = 33;
            this.label9.Text = "(par ordre de préférence, séparées par une virgule,\nlaisser vide pour ne pas avoi" +
                "r de sous-titres)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SubLanguage
            // 
            this.SubLanguage.Location = new System.Drawing.Point(190, 242);
            this.SubLanguage.Name = "SubLanguage";
            this.SubLanguage.Size = new System.Drawing.Size(86, 20);
            this.SubLanguage.TabIndex = 31;
            this.SubLanguage.Text = "fr,en";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 19);
            this.label4.TabIndex = 23;
            this.label4.Text = "Extensions des vidéos :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 19);
            this.label5.TabIndex = 25;
            this.label5.Text = "Extensions des sons :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 19);
            this.label6.TabIndex = 27;
            this.label6.Text = "Extensions des images :";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(12, 216);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(172, 19);
            this.label7.TabIndex = 29;
            this.label7.Text = "Pistes audio :";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 19);
            this.label1.TabIndex = 18;
            this.label1.Text = "Emplacement de VLC :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 19);
            this.label2.TabIndex = 19;
            this.label2.Text = "Port TCP local de contrôle de VLC:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(11, 401);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(310, 26);
            this.label11.TabIndex = 34;
            this.label11.Text = "Certains réglages nécessitent de relancer VLC pour être pris en compte";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(12, 242);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(172, 19);
            this.label10.TabIndex = 32;
            this.label10.Text = "Langues des sous-titres :";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CloseBtn
            // 
            this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CloseBtn.Location = new System.Drawing.Point(432, 401);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(98, 23);
            this.CloseBtn.TabIndex = 21;
            this.CloseBtn.Text = "Fermer";
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // VLCPath
            // 
            this.VLCPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VLCPath.Location = new System.Drawing.Point(190, 9);
            this.VLCPath.Name = "VLCPath";
            this.VLCPath.Size = new System.Drawing.Size(340, 20);
            this.VLCPath.TabIndex = 8;
            this.VLCPath.Text = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe";
            // 
            // AudioLanguage
            // 
            this.AudioLanguage.Location = new System.Drawing.Point(190, 216);
            this.AudioLanguage.Name = "AudioLanguage";
            this.AudioLanguage.Size = new System.Drawing.Size(86, 20);
            this.AudioLanguage.TabIndex = 28;
            this.AudioLanguage.Text = "fr,en";
            // 
            // PictureExts
            // 
            this.PictureExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureExts.Location = new System.Drawing.Point(190, 139);
            this.PictureExts.Name = "PictureExts";
            this.PictureExts.Size = new System.Drawing.Size(340, 20);
            this.PictureExts.TabIndex = 26;
            // 
            // VlcPort
            // 
            this.VlcPort.Location = new System.Drawing.Point(190, 35);
            this.VlcPort.Name = "VlcPort";
            this.VlcPort.Size = new System.Drawing.Size(86, 20);
            this.VlcPort.TabIndex = 10;
            this.VlcPort.Text = "31186";
            // 
            // VideoExts
            // 
            this.VideoExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VideoExts.Location = new System.Drawing.Point(190, 87);
            this.VideoExts.Name = "VideoExts";
            this.VideoExts.Size = new System.Drawing.Size(340, 20);
            this.VideoExts.TabIndex = 22;
            // 
            // SoundExts
            // 
            this.SoundExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SoundExts.Location = new System.Drawing.Point(190, 113);
            this.SoundExts.Name = "SoundExts";
            this.SoundExts.Size = new System.Drawing.Size(340, 20);
            this.SoundExts.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(12, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(172, 19);
            this.label12.TabIndex = 36;
            this.label12.Text = "Lettre du lecteur de DVD :";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DVDLetter
            // 
            this.DVDLetter.Location = new System.Drawing.Point(190, 61);
            this.DVDLetter.MaxLength = 1;
            this.DVDLetter.Name = "DVDLetter";
            this.DVDLetter.Size = new System.Drawing.Size(86, 20);
            this.DVDLetter.TabIndex = 35;
            this.DVDLetter.Text = "D";
            // 
            // ShowVLC
            // 
            this.ShowVLC.Location = new System.Drawing.Point(41, 297);
            this.ShowVLC.Name = "ShowVLC";
            this.ShowVLC.Size = new System.Drawing.Size(235, 14);
            this.ShowVLC.TabIndex = 37;
            this.ShowVLC.Text = "Afficher la fenêtre de VLC";
            this.ShowVLC.CheckedChanged += new System.EventHandler(this.ShowVLC_CheckedChanged);
            // 
            // RestartVLC
            // 
            this.RestartVLC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RestartVLC.Location = new System.Drawing.Point(328, 401);
            this.RestartVLC.Name = "RestartVLC";
            this.RestartVLC.Size = new System.Drawing.Size(98, 23);
            this.RestartVLC.TabIndex = 38;
            this.RestartVLC.Text = "Relancer VLC";
            this.RestartVLC.Click += new System.EventHandler(this.RestartVLC_Click);
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(12, 165);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(172, 19);
            this.label13.TabIndex = 40;
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
            this.panel1.Size = new System.Drawing.Size(340, 19);
            this.panel1.TabIndex = 41;
            // 
            // TranscodePC
            // 
            this.TranscodePC.AutoSize = true;
            this.TranscodePC.Location = new System.Drawing.Point(258, 1);
            this.TranscodePC.Name = "TranscodePC";
            this.TranscodePC.Size = new System.Drawing.Size(82, 17);
            this.TranscodePC.TabIndex = 3;
            this.TranscodePC.TabStop = false;
            this.TranscodePC.Text = "Audio sur PC";
            // 
            // TranscodeA52
            // 
            this.TranscodeA52.AutoSize = true;
            this.TranscodeA52.Location = new System.Drawing.Point(159, 1);
            this.TranscodeA52.Name = "TranscodeA52";
            this.TranscodeA52.Size = new System.Drawing.Size(93, 17);
            this.TranscodeA52.TabIndex = 2;
            this.TranscodeA52.TabStop = false;
            this.TranscodeA52.Text = "A/52 Dolby 5.1";
            // 
            // TranscodeMPGA
            // 
            this.TranscodeMPGA.AutoSize = true;
            this.TranscodeMPGA.Checked = true;
            this.TranscodeMPGA.Location = new System.Drawing.Point(58, 1);
            this.TranscodeMPGA.Name = "TranscodeMPGA";
            this.TranscodeMPGA.Size = new System.Drawing.Size(95, 17);
            this.TranscodeMPGA.TabIndex = 1;
            this.TranscodeMPGA.Text = "MPEG-II Stéréo";
            // 
            // TranscodeNone
            // 
            this.TranscodeNone.AutoSize = true;
            this.TranscodeNone.Location = new System.Drawing.Point(0, 1);
            this.TranscodeNone.Name = "TranscodeNone";
            this.TranscodeNone.Size = new System.Drawing.Size(52, 17);
            this.TranscodeNone.TabIndex = 0;
            this.TranscodeNone.TabStop = false;
            this.TranscodeNone.Text = "Aucun";
            // 
            // StartMinimized
            // 
            this.StartMinimized.Location = new System.Drawing.Point(41, 277);
            this.StartMinimized.Name = "StartMinimized";
            this.StartMinimized.Size = new System.Drawing.Size(235, 14);
            this.StartMinimized.TabIndex = 42;
            this.StartMinimized.Text = "Minimiser au lancement";
            // 
            // StartAtBoot
            // 
            this.StartAtBoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.StartAtBoot.Location = new System.Drawing.Point(283, 277);
            this.StartAtBoot.Name = "StartAtBoot";
            this.StartAtBoot.Size = new System.Drawing.Size(247, 14);
            this.StartAtBoot.TabIndex = 43;
            this.StartAtBoot.Text = "Lancer FreeXplorer au démarrage de Windows";
            this.StartAtBoot.CheckedChanged += new System.EventHandler(this.StartAtBoot_CheckedChanged);
            // 
            // MinimizeToTray
            // 
            this.MinimizeToTray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MinimizeToTray.Location = new System.Drawing.Point(283, 297);
            this.MinimizeToTray.Name = "MinimizeToTray";
            this.MinimizeToTray.Size = new System.Drawing.Size(247, 14);
            this.MinimizeToTray.TabIndex = 44;
            this.MinimizeToTray.Text = "Mini-icone près de l\'horloge lorsque minimisé";
            // 
            // FFMpegInterlace
            // 
            this.FFMpegInterlace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FFMpegInterlace.Location = new System.Drawing.Point(41, 347);
            this.FFMpegInterlace.Name = "FFMpegInterlace";
            this.FFMpegInterlace.Size = new System.Drawing.Size(489, 14);
            this.FFMpegInterlace.TabIndex = 45;
            this.FFMpegInterlace.Text = "Suppression du scintillement vidéo (plus agréable pour les yeux mais consomme du " +
                "CPU)";
            // 
            // HalfScale
            // 
            this.HalfScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.HalfScale.Location = new System.Drawing.Point(41, 367);
            this.HalfScale.Name = "HalfScale";
            this.HalfScale.Size = new System.Drawing.Size(489, 14);
            this.HalfScale.TabIndex = 46;
            this.HalfScale.Text = "Mode d\'économie de CPU sans baisse de qualité visible (traitement sur moins de pi" +
                "xels)";
            // 
            // LIRCActive
            // 
            this.LIRCActive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LIRCActive.Location = new System.Drawing.Point(283, 317);
            this.LIRCActive.Name = "LIRCActive";
            this.LIRCActive.Size = new System.Drawing.Size(247, 14);
            this.LIRCActive.TabIndex = 47;
            this.LIRCActive.Text = "Activer le serveur LIRC";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 19);
            this.label3.TabIndex = 49;
            this.label3.Text = "Débit vidéo (en kbit/s) :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TranscodeVB
            // 
            this.TranscodeVB.Location = new System.Drawing.Point(190, 190);
            this.TranscodeVB.Name = "TranscodeVB";
            this.TranscodeVB.Size = new System.Drawing.Size(86, 20);
            this.TranscodeVB.TabIndex = 48;
            this.TranscodeVB.Text = "9000";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.Location = new System.Drawing.Point(282, 190);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(248, 19);
            this.label14.TabIndex = 50;
            this.label14.Text = "(ne modifier que si votre réseau ne le supporte pas)";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PCControlAllowed
            // 
            this.PCControlAllowed.Checked = true;
            this.PCControlAllowed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PCControlAllowed.Location = new System.Drawing.Point(41, 317);
            this.PCControlAllowed.Name = "PCControlAllowed";
            this.PCControlAllowed.Size = new System.Drawing.Size(235, 14);
            this.PCControlAllowed.TabIndex = 51;
            this.PCControlAllowed.Text = "Autoriser le contrôle du PC par la Freebox";
            this.PCControlAllowed.CheckedChanged += new System.EventHandler(this.PCControlAllowed_CheckedChanged);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 436);
            this.Controls.Add(this.PCControlAllowed);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TranscodeVB);
            this.Controls.Add(this.LIRCActive);
            this.Controls.Add(this.HalfScale);
            this.Controls.Add(this.FFMpegInterlace);
            this.Controls.Add(this.MinimizeToTray);
            this.Controls.Add(this.StartAtBoot);
            this.Controls.Add(this.StartMinimized);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.RestartVLC);
            this.Controls.Add(this.ShowVLC);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.DVDLetter);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.SubLanguage);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.AudioLanguage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.PictureExts);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SoundExts);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.VideoExts);
            this.Controls.Add(this.CloseBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VlcPort);
            this.Controls.Add(this.VLCPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "Configuration du FreeXplorer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Button CloseBtn;
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
        private System.Windows.Forms.Button RestartVLC;
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
    }
}