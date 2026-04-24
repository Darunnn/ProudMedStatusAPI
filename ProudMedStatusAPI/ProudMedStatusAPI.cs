using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProudMedStatusAPI
{
    public partial class ProudMedStatusAPI : Form
    {
        // ---- Constants ----
        private const int HealthCheckIntervalSeconds = 30;

        // ---- Fields ----
        private readonly Config _config;
        private readonly LogManager _logger;
        private readonly DispenseWorker _worker;    // รับมาจาก Program.cs ไม่สร้างเอง
        private readonly System.Windows.Forms.Timer _uiTimer;

        private DateTime _nextRoundTime;

        // ติดตามสถานะล่าสุด เพื่อ log เฉพาะตอนที่สถานะเปลี่ยน
        private bool? _lastDbReachable = null;
        private string _lastDbError = "";
        private bool? _lastApiReachable = null;
        private string _lastApiError = "";

        // ---- Constructor ----

        /// <summary>
        /// รับ config, log, worker จาก Program.cs
        /// Form ไม่สร้าง Worker เองอีกต่อไป
        /// </summary>
        public ProudMedStatusAPI(Config config, LogManager log, DispenseWorker worker)
        {
            InitializeComponent();
            InitializeTray();

            _config = config;
            _logger = log;
            _worker = worker;

            // เชื่อม callback จาก Worker → UI
            _worker.OnStatsUpdated = UpdateStats;

            // ตรวจสอบ Connection ครั้งแรกทันที
            CheckDatabaseConnection();
            CheckApiConnection();

            // UI countdown timer (tick ทุก 1 วินาที)
            CalculateNextRoundTime();
            _uiTimer = new System.Windows.Forms.Timer();
            _uiTimer.Interval = 1000;
            _uiTimer.Tick += OnUiTimerTick;
            _uiTimer.Start();

            _logger.Info("โปรแกรมเริ่มทำงาน");
        }

        // ---- UI Timer ----

        private void OnUiTimerTick(object? sender, EventArgs e)
        {
            UpdateNextRoundCountdown();

            // Recheck DB + API ทุก HealthCheckIntervalSeconds วินาที
            if (DateTime.Now.Second % HealthCheckIntervalSeconds == 0)
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

        // ---- Health Checks ----

        private void CheckDatabaseConnection()
        {
            bool reachable = false;
            string errorMsg = "";

            try
            {
                var db = new Database();
                reachable = db.TestConnection();
                if (!reachable) errorMsg = "Failed";
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message.Length > 28
                    ? ex.Message[..28] + "…"
                    : ex.Message;
            }

            // Log เฉพาะตอนสถานะเปลี่ยน
            bool statusChanged = _lastDbReachable == null
                              || _lastDbReachable != reachable
                              || (!reachable && _lastDbError != errorMsg);

            if (statusChanged)
            {
                if (reachable)
                    _logger.Info($"DB status → Connected ({_config.ConnectionStringJSD})");
                else
                    _logger.Error($"DB status → {errorMsg} ({_config.ConnectionStringJSD})");
            }

            _lastDbReachable = reachable;
            _lastDbError = errorMsg;

            // อัปเดต UI
            if (reachable)
            {
                lblDbStatus.Text = "Connected";
                lblDbStatus.ForeColor = Color.FromArgb(21, 128, 61);
                panelDbAccent.BackColor = Color.FromArgb(34, 197, 94);
            }
            else
            {
                lblDbStatus.Text = "Failed";
                lblDbStatus.ForeColor = Color.FromArgb(185, 28, 28);
                panelDbAccent.BackColor = Color.FromArgb(239, 68, 68);
            }

            lblLastUpdate.Text = "อัปเดต " + DateTime.Now.ToString("HH:mm:ss");
            UpdateRunningStatus();
        }

        private async void CheckApiConnection()
        {
            bool reachable = false;
            string errorMsg = "";

            try
            {
                using var client = new DispenseApiClient(_config.DomainAPI, _logger);
                reachable = await client.PingAsync();
                if (!reachable) errorMsg = "Unreachable";
            }
            catch (UriFormatException)
            {
                errorMsg = "Invalid URL";
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message.Length > 28
                    ? ex.Message[..28] + "…"
                    : ex.Message;
            }

            // Log เฉพาะตอนสถานะเปลี่ยน
            bool statusChanged = _lastApiReachable == null
                              || _lastApiReachable != reachable
                              || (!reachable && _lastApiError != errorMsg);

            if (statusChanged)
            {
                if (reachable)
                    _logger.Info($"API status → Connected ({_config.DomainAPI})");
                else
                    _logger.Error($"API status → {errorMsg} ({_config.DomainAPI})");
            }

            _lastApiReachable = reachable;
            _lastApiError = errorMsg;

            // อัปเดต UI
            if (reachable)
            {
                lblApiStatus.Text = "Connected";
                lblApiStatus.ForeColor = Color.FromArgb(21, 128, 61);
                panelApiAccent.BackColor = Color.FromArgb(59, 130, 246);
            }
            else
            {
                lblApiStatus.Text = errorMsg;
                lblApiStatus.ForeColor = Color.FromArgb(185, 28, 28);
                panelApiAccent.BackColor = Color.FromArgb(239, 68, 68);
            }

            UpdateRunningStatus();
        }

        private void UpdateRunningStatus()
        {
            bool hasError = _lastDbReachable == false
                         || _lastApiReachable == false;

            lblRunning.Text = hasError ? "● ข้อผิดพลาด" : "● กำลังทำงาน";
            lblRunning.ForeColor = hasError
                ? Color.FromArgb(185, 28, 28)
                : Color.FromArgb(34, 197, 94);
        }

        // ---- Update Stats (เรียกจาก Worker thread) ----

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
            // _worker.Stop() จะถูกเรียกที่ Program.cs หลัง Application.Run() จบ
            // ไม่ต้องเรียกซ้ำที่นี่
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