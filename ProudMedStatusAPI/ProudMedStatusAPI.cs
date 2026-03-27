using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProudMedStatusAPI
{
    public partial class ProudMedStatusAPI : Form
    {
        private System.Windows.Forms.Timer _uiTimer;
        private Config _config;
        private LogManager _logger;
        private DispenseWorker _worker;
        private DateTime _nextRoundTime;

        public ProudMedStatusAPI(LogManager log)
        {
            InitializeComponent();
            InitializeTray();

            _config = new Config();
            _logger = log;

            // ตรวจสอบ DB connection ครั้งแรก
            CheckDatabaseConnection();
            CheckApiConnection();

            // เริ่ม Worker
            _worker = new DispenseWorker(_config, _logger);
            _worker.OnStatsUpdated = UpdateStats;
            _worker.Start();

            // UI countdown timer (tick ทุก 1 วินาที)
            CalculateNextRoundTime();
            _uiTimer = new System.Windows.Forms.Timer();
            _uiTimer.Interval = 1000;
            _uiTimer.Tick += OnUiTimerTick;
            _uiTimer.Start();

            _logger.Info("โปรแกรมเริ่มทำงาน");
        }

        // ---- Timer UI ----

        private void OnUiTimerTick(object? sender, EventArgs e)
        {
            UpdateNextRoundCountdown();

            // recheck DB + API ทุก 30 วินาที
            if (DateTime.Now.Second % 30 == 0)
            {
                CheckDatabaseConnection();
                CheckApiConnection();
            }
        }

        private void CalculateNextRoundTime()
        {
            int intervalSeconds = _config.TimeProcess;
            var now = DateTime.Now;
            long nowSec = (long)now.TimeOfDay.TotalSeconds;
            long untilNext = intervalSeconds - (nowSec % intervalSeconds);
            _nextRoundTime = now.AddSeconds(untilNext).AddMilliseconds(-now.Millisecond);
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

        // ---- Status checks ----

        private void CheckDatabaseConnection()
        {
            try
            {
                var db = new Database();
                if (db.TestConnection())
                {
                    lblDbStatus.Text = "Connected";
                    lblDbStatus.ForeColor = Color.FromArgb(21, 128, 61);
                    panelDbAccent.BackColor = Color.FromArgb(34, 197, 94);
                }
                else
                {
                    SetDbError("Failed");
                }
            }
            catch (Exception ex)
            {
                SetDbError(ex.Message.Length > 28 ? ex.Message[..28] + "…" : ex.Message);
            }

            lblLastUpdate.Text = "อัปเดต " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void SetDbError(string message)
        {
            lblDbStatus.Text = "Failed";
            lblDbStatus.ForeColor = Color.FromArgb(185, 28, 28);
            panelDbAccent.BackColor = Color.FromArgb(239, 68, 68);
        }

        private async void CheckApiConnection()
        {
            try
            {
                // ตรวจ URL format ก่อน
                var uri = new Uri(_config.DomainAPI.TrimEnd('/') + "/");

                // ping จริง
                using var client = new DispenseApiClient(_config.DomainAPI, _logger);
                bool reachable = await client.PingAsync();

                if (reachable)
                {
                    lblApiStatus.Text = "Connected";
                    lblApiStatus.ForeColor = Color.FromArgb(21, 128, 61);
                    panelApiAccent.BackColor = Color.FromArgb(59, 130, 246);
                }
                else
                {
                    SetApiError("Unreachable");
                }
            }
            catch (UriFormatException)
            {
                SetApiError("Invalid URL");
            }
            catch (Exception ex)
            {
                SetApiError(ex.Message.Length > 28 ? ex.Message[..28] + "…" : ex.Message);
            }
        }
        private void SetApiError(string message)
        {
            lblApiStatus.Text = message;
            lblApiStatus.ForeColor = Color.FromArgb(185, 28, 28);
            panelApiAccent.BackColor = Color.FromArgb(239, 68, 68);
        }
        // ---- Update stats (called from Worker thread) ----

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

        // ---- Tray ----

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
            _uiTimer?.Stop();
            _worker?.Stop();
            _logger?.Info("โปรแกรมปิดโดยผู้ใช้");
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
           
                base.OnFormClosing(e);
            
        }

        
    }
}