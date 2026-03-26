using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProudMedStatusAPI
{
    public partial class ProudMedStatusAPI : Form
    {
        public ProudMedStatusAPI()
        {
            InitializeComponent();
            InitializeTray();
            CheckDatabaseConnection();
        }

        private void CheckDatabaseConnection()
        {
            try
            {
                var db = new Database();
                if (db.TestConnection())
                {
                    Connect.Text = "✅ Database: Connected";
                    Connect.ForeColor = Color.Green;
                }
                else
                {
                    Connect.Text = "❌ Database: Failed";
                    Connect.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Connect.Text = $"❌ Error: {ex.Message}";
                Connect.ForeColor = Color.Red;
            }
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
            notifyIcon1.Visible = false;
            Application.Exit();
        }
    }
}