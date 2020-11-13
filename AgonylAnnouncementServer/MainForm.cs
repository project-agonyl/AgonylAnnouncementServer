using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AgonylAnnouncementServer
{
    public partial class MainForm : Form
    {
        private Config config;
        private List<string> announcements = new List<string>();
        private bool announcementServiceStarted;
        private EventDrivenTCPClient mainServerClient;
        private int currentAnnouncementIndex;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Utils.IsAlreadyRunning())
            {
                Environment.Exit(0);
            }

            if (!File.Exists(Utils.ConfigFilePath()))
            {
                _ = MessageBox.Show("Config.json does not exist!", "Announcement Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            this.LoadConfig();
            this.LoadAnnouncements();
            this.InitializeComponents();
        }

        private void LoadConfig()
        {
            this.config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Utils.ConfigFilePath()));
            this.MainServerIp.Text = this.config.MainServerIp;
            this.MainServerPort.Text = this.config.MainServerPort.ToString();
            this.AnnouncementServiceTimer.Interval = this.config.AnnouncementInterval * 1000;
        }

        private void LoadAnnouncements()
        {
            try
            {
                if (File.Exists(Utils.AnnouncementsFilePath()))
                {
                    this.announcements = new List<string>(File.ReadAllLines(Utils.AnnouncementsFilePath()));
                }

                this.AnnouncementCount.Text = this.announcements.Count + " Announcements";
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Announcement Load", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponents()
        {
            this.mainServerClient = new EventDrivenTCPClient(IPAddress.Parse(this.config.MainServerIp), this.config.MainServerPort);
            this.mainServerClient.ConnectionStatusChanged += this.MainServerClient_ConnectionStatusChanged;
            this.mainServerClient.Connect();
            this.AnnouncementServiceTimer.Tick += this.AnnouncementServiceTimer_Tick;
            this.MainFormUpdater.Tick += this.MainFormUpdater_Tick;
            this.MainFormUpdater.Start();
        }

        private void MainFormUpdater_Tick(object sender, EventArgs e)
        {
            if (this.announcementServiceStarted)
            {
                if (this.mainServerClient.ConnectionState == EventDrivenTCPClient.ConnectionStatus.Connected)
                {
                    this.AnnouncementServiceStatus.Text = "Live";
                }
                else
                {
                    this.AnnouncementServiceStatus.Text = "Enabled";
                }

                this.AnnouncementServiceStatus.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                this.AnnouncementServiceStatus.Text = "Stopped";
                this.AnnouncementServiceStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void AnnouncementServiceTimer_Tick(object sender, EventArgs e)
        {
            if (this.mainServerClient.ConnectionState == EventDrivenTCPClient.ConnectionStatus.Connected)
            {
                if (this.currentAnnouncementIndex >= this.announcements.Count)
                {
                    this.currentAnnouncementIndex = 0;
                }

                var currentAnnouncement = this.announcements[this.currentAnnouncementIndex].Replace("{ServerName}", this.config.ServerName);
                if (currentAnnouncement.Length > 60)
                {
                    currentAnnouncement = currentAnnouncement.Substring(0, 60);
                }

                this.mainServerClient.Send(PacketHelper.BuildAnnouncementPacket("[" + this.config.ServerName +"]", currentAnnouncement, this.config.AnnouncementType));
                this.currentAnnouncementIndex++;
            }
        }

        private void MainServerClient_ConnectionStatusChanged(EventDrivenTCPClient sender, EventDrivenTCPClient.ConnectionStatus status)
        {
            if (status == EventDrivenTCPClient.ConnectionStatus.Connected)
            {
                // Initial handshake message
                this.mainServerClient.Send(PacketHelper.MakeBytesArrayfromHexString("01 A0 00 00 03"));
            }
        }

        private void AnnouncementServiceToggle_Click(object sender, EventArgs e)
        {
            if (!this.announcementServiceStarted)
            {
                if (this.announcements.Count == 0)
                {
                    _ = MessageBox.Show("Announcements.txt is empty", "Announcement Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.announcementServiceStarted = true;
                    this.currentAnnouncementIndex = 0;
                    this.AnnouncementServiceToggle.Enabled = false;
                    this.ReloadAnnouncements.Enabled = false;
                    this.AnnouncementServiceTimer.Start();
                    this.AnnouncementServiceToggle.Text = "STOP";
                    this.AnnouncementServiceToggle.Enabled = true;
                }
            }
            else
            {
                this.AnnouncementServiceTimer.Stop();
                this.AnnouncementServiceToggle.Text = "START";
                this.AnnouncementServiceToggle.Enabled = true;
                this.ReloadAnnouncements.Enabled = true;
                this.announcementServiceStarted = false;
            }
        }

        private void ReloadAnnouncements_Click(object sender, EventArgs e)
        {
            if (!this.announcementServiceStarted)
            {
                this.LoadAnnouncements();
            }
        }
    }
}
