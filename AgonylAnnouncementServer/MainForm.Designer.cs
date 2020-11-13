namespace AgonylAnnouncementServer
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.MainServerPort = new System.Windows.Forms.Label();
            this.MainServerIp = new System.Windows.Forms.Label();
            this.ReloadAnnouncements = new System.Windows.Forms.Button();
            this.AnnouncementServiceToggle = new System.Windows.Forms.Button();
            this.AnnouncementServiceStatus = new System.Windows.Forms.Label();
            this.AnnouncementCount = new System.Windows.Forms.Label();
            this.AnnouncementServiceTimer = new System.Windows.Forms.Timer(this.components);
            this.MainFormUpdater = new System.Windows.Forms.Timer(this.components);
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.MainServerPort);
            this.groupBox4.Controls.Add(this.MainServerIp);
            this.groupBox4.Controls.Add(this.ReloadAnnouncements);
            this.groupBox4.Controls.Add(this.AnnouncementServiceToggle);
            this.groupBox4.Controls.Add(this.AnnouncementServiceStatus);
            this.groupBox4.Controls.Add(this.AnnouncementCount);
            this.groupBox4.Location = new System.Drawing.Point(30, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 159);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Announcement Service";
            // 
            // MainServerPort
            // 
            this.MainServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainServerPort.Location = new System.Drawing.Point(9, 61);
            this.MainServerPort.Name = "MainServerPort";
            this.MainServerPort.Size = new System.Drawing.Size(185, 23);
            this.MainServerPort.TabIndex = 5;
            this.MainServerPort.Text = "Main Server Port";
            this.MainServerPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainServerIp
            // 
            this.MainServerIp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainServerIp.Location = new System.Drawing.Point(6, 37);
            this.MainServerIp.Name = "MainServerIp";
            this.MainServerIp.Size = new System.Drawing.Size(188, 23);
            this.MainServerIp.TabIndex = 4;
            this.MainServerIp.Text = "Main Server IP";
            this.MainServerIp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ReloadAnnouncements
            // 
            this.ReloadAnnouncements.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReloadAnnouncements.Location = new System.Drawing.Point(110, 119);
            this.ReloadAnnouncements.Name = "ReloadAnnouncements";
            this.ReloadAnnouncements.Size = new System.Drawing.Size(75, 23);
            this.ReloadAnnouncements.TabIndex = 3;
            this.ReloadAnnouncements.Text = "RELOAD";
            this.ReloadAnnouncements.UseVisualStyleBackColor = true;
            this.ReloadAnnouncements.Click += new System.EventHandler(this.ReloadAnnouncements_Click);
            // 
            // AnnouncementServiceToggle
            // 
            this.AnnouncementServiceToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnnouncementServiceToggle.Location = new System.Drawing.Point(12, 119);
            this.AnnouncementServiceToggle.Name = "AnnouncementServiceToggle";
            this.AnnouncementServiceToggle.Size = new System.Drawing.Size(75, 23);
            this.AnnouncementServiceToggle.TabIndex = 1;
            this.AnnouncementServiceToggle.Text = "START";
            this.AnnouncementServiceToggle.UseVisualStyleBackColor = true;
            this.AnnouncementServiceToggle.Click += new System.EventHandler(this.AnnouncementServiceToggle_Click);
            // 
            // AnnouncementServiceStatus
            // 
            this.AnnouncementServiceStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnnouncementServiceStatus.ForeColor = System.Drawing.Color.Red;
            this.AnnouncementServiceStatus.Location = new System.Drawing.Point(8, 85);
            this.AnnouncementServiceStatus.Name = "AnnouncementServiceStatus";
            this.AnnouncementServiceStatus.Size = new System.Drawing.Size(185, 23);
            this.AnnouncementServiceStatus.TabIndex = 2;
            this.AnnouncementServiceStatus.Text = "Stopped";
            this.AnnouncementServiceStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AnnouncementCount
            // 
            this.AnnouncementCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnnouncementCount.Location = new System.Drawing.Point(6, 16);
            this.AnnouncementCount.Name = "AnnouncementCount";
            this.AnnouncementCount.Size = new System.Drawing.Size(188, 23);
            this.AnnouncementCount.TabIndex = 0;
            this.AnnouncementCount.Text = "0 Announcements";
            this.AnnouncementCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AnnouncementServiceTimer
            // 
            this.AnnouncementServiceTimer.Interval = 6000;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 199);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Announcements";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label MainServerPort;
        private System.Windows.Forms.Label MainServerIp;
        private System.Windows.Forms.Button ReloadAnnouncements;
        private System.Windows.Forms.Button AnnouncementServiceToggle;
        private System.Windows.Forms.Label AnnouncementServiceStatus;
        private System.Windows.Forms.Label AnnouncementCount;
        private System.Windows.Forms.Timer AnnouncementServiceTimer;
        private System.Windows.Forms.Timer MainFormUpdater;
    }
}

