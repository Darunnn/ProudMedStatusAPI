using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProudMedStatusAPI
{
    public partial class ProudMedStatusAPI : Form
    {
        private System.Windows.Forms.Timer _refreshTimer;
        private Config _config;
        private DateTime _nextRoundTime;

        public ProudMedStatusAPI()
        {
            InitializeComponent();
            InitializeTray();
            _config = new Config();
            CheckDatabaseConnection();
            StartRefreshTimer();
            UpdateNextRoundCountdown();
        }
        private string ExtractHost(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return "ไม่พบ config";

            foreach (var part in connectionString.Split(';'))
            {
                var kv = part.Trim().Split('=', 2);
                if (kv.Length == 2 &&
                    kv[0].Trim().Equals("Server", StringComparison.OrdinalIgnoreCase) ||
                    kv[0].Trim().Equals("Data Source", StringComparison.OrdinalIgnoreCase))
                {
                    return kv[1].Trim();
                }
            }

            return "ไม่พบ Server";
        }
        private void CheckDatabaseConnection()
        {
            try
            {
                var db = new Database();
                if (db.TestConnection())
                {
                    lblDbStatus.Text = "Connected";
                    lblDbStatus.ForeColor = Color.FromArgb(21, 128, 61);
                    lblDbSub.Text = ExtractHost(_config.ConnectionStringJSD);
                    panelDbAccent.BackColor = Color.FromArgb(34, 197, 94);
                }
                else
                {
                    lblDbStatus.Text = "Failed";
                    lblDbStatus.ForeColor = Color.FromArgb(185, 28, 28);
                    lblDbSub.Text = ExtractHost(_config.ConnectionStringJSD);
                    panelDbAccent.BackColor = Color.FromArgb(239, 68, 68);
                }
            }
            catch (Exception ex)
            {
                lblDbStatus.Text = "Error";
                lblDbStatus.ForeColor = Color.FromArgb(185, 28, 28);
                lblDbSub.Text = ex.Message.Length > 28 ? ex.Message[..28] + "…" : ex.Message;
                panelDbAccent.BackColor = Color.FromArgb(239, 68, 68);
            }

            lblLastUpdate.Text = "อัปเดต " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void StartRefreshTimer()
        {
            // คำนวณ _nextRoundTime ครั้งแรก
            CalculateNextRoundTime();

            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 1000; // tick ทุก 1 วินาที เพื่อนับถอยหลัง
            _refreshTimer.Tick += OnTimerTick;
            _refreshTimer.Start();
        }

        private void CalculateNextRoundTime()
        {
            int intervalSeconds = _config.TimeProcess;
            var now = DateTime.Now;

            long nowSeconds = (long)now.TimeOfDay.TotalSeconds;
            long secondsUntilNext = intervalSeconds - (nowSeconds % intervalSeconds);

            _nextRoundTime = now.AddSeconds(secondsUntilNext)
                                .AddMilliseconds(-now.Millisecond);
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            UpdateNextRoundCountdown();

            // ทุกๆ 30 วินาที ให้ recheck DB
            if (DateTime.Now.Second % 30 == 0)
                CheckDatabaseConnection();
        }

        private void UpdateNextRoundCountdown()
        {
            var remaining = _nextRoundTime - DateTime.Now;

            if (remaining.TotalSeconds <= 0)
            {
                CalculateNextRoundTime();
                remaining = _nextRoundTime - DateTime.Now;
            }

            lblNextValue.Text = $"{(int)remaining.TotalSeconds:D3}";
            lblNextSub.Text = $"ทุก {_config.TimeProcess} วินาที";
        }

        public void UpdateStats(int pendingCount, int successCount)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateStats(pendingCount, successCount));
                return;
            }
            lblPendingValue.Text = pendingCount.ToString("N0");
            lblSuccessValue.Text = successCount.ToString("N0");
            lblLastUpdate.Text = "อัปเดต " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void InitializeTray()
        {
            var trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("เปิดหน้าต่าง", null, OnOpen);
            trayMenu.Items.Add("ออกจากโปรแกรม", null, OnExit);
            notifyIcon1.Text = "ProudMedStatusAPI";
            notifyIcon1.ContextMenuStrip = trayMenu;
            notifyIcon1.DoubleClick += OnOpen;
            notifyIcon1.Visible = false;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        private void OnOpen(object? sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            BringToFront();
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _refreshTimer?.Stop();
            notifyIcon1.Visible = false;
            Application.Exit();
        }
    }
}