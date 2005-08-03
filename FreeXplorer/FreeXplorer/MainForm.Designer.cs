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
            this.ConfigButton = new System.Windows.Forms.Button();
            this.FreeboxIP = new System.Windows.Forms.Label();
            this.TrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TrayRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConfigButton
            // 
            this.ConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigButton.Location = new System.Drawing.Point(329, 8);
            this.ConfigButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ConfigButton.Name = "ConfigButton";
            this.ConfigButton.Size = new System.Drawing.Size(86, 24);
            this.ConfigButton.TabIndex = 9;
            this.ConfigButton.Text = "Config...";
            this.ConfigButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // FreeboxIP
            // 
            this.FreeboxIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FreeboxIP.Location = new System.Drawing.Point(12, 14);
            this.FreeboxIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FreeboxIP.Name = "FreeboxIP";
            this.FreeboxIP.Size = new System.Drawing.Size(309, 16);
            this.FreeboxIP.TabIndex = 0;
            // 
            // TrayContextMenu
            // 
            this.TrayContextMenu.Enabled = true;
            this.TrayContextMenu.GripMargin = new System.Windows.Forms.Padding(2);
            this.TrayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TrayRestore,
            this.TrayConfig,
            this.TrayQuit});
            this.TrayContextMenu.Location = new System.Drawing.Point(24, 65);
            this.TrayContextMenu.Name = "TrayContextMenu";
            this.TrayContextMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TrayContextMenu.Size = new System.Drawing.Size(179, 70);
            // 
            // TrayRestore
            // 
            this.TrayRestore.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.TrayRestore.Name = "TrayRestore";
            this.TrayRestore.Text = "Restaurer la fenêtre";
            this.TrayRestore.Click += new System.EventHandler(this.TrayRestore_Click);
            // 
            // TrayConfig
            // 
            this.TrayConfig.Name = "TrayConfig";
            this.TrayConfig.Text = "Configuration...";
            this.TrayConfig.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // TrayQuit
            // 
            this.TrayQuit.Name = "TrayQuit";
            this.TrayQuit.Text = "Arrêter FreeXplorer";
            this.TrayQuit.Click += new System.EventHandler(this.TrayQuit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 40);
            this.Controls.Add(this.ConfigButton);
            this.Controls.Add(this.FreeboxIP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "FreeXplorer {0} actif";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.TrayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label FreeboxIP;
        private System.Windows.Forms.Button ConfigButton;
        private System.Windows.Forms.ContextMenuStrip TrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem TrayRestore;
        private System.Windows.Forms.ToolStripMenuItem TrayQuit;
        private System.Windows.Forms.ToolStripMenuItem TrayConfig;
    }
}

