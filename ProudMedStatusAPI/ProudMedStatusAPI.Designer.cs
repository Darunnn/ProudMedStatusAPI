namespace ProudMedStatusAPI
{
    partial class ProudMedStatusAPI
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProudMedStatusAPI));
            notifyIcon1 = new NotifyIcon(components);
            panelDb = new Panel();
            panelDbAccent = new Panel();
            lblDbTitle = new Label();
            lblDbStatus = new Label();
            panelApi = new Panel();
            panelApiAccent = new Panel();
            lblApiTitle = new Label();
            lblApiStatus = new Label();
            panelNext = new Panel();
            lblNextTitle = new Label();
            lblNextValue = new Label();
            lblNextSub = new Label();
            panelPending = new Panel();
            panelPendingAccent = new Panel();
            lblPendingTitle = new Label();
            lblPendingValue = new Label();
            lblPendingSub = new Label();
            panelSuccess = new Panel();
            panelSuccessAccent = new Panel();
            lblSuccessTitle = new Label();
            lblSuccessValue = new Label();
            lblSuccessSub = new Label();
            panelFooter = new Panel();
            lblRunning = new Label();
            lblLastUpdate = new Label();
            panelDb.SuspendLayout();
            panelApi.SuspendLayout();
            panelNext.SuspendLayout();
            panelPending.SuspendLayout();
            panelSuccess.SuspendLayout();
            panelFooter.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "ProudMedStatusAPI";
            notifyIcon1.Visible = true;
            // 
            // panelDb
            // 
            panelDb.BackColor = Color.White;
            panelDb.BorderStyle = BorderStyle.FixedSingle;
            panelDb.Controls.Add(panelDbAccent);
            panelDb.Controls.Add(lblDbTitle);
            panelDb.Controls.Add(lblDbStatus);
            panelDb.Location = new Point(10, 10);
            panelDb.Name = "panelDb";
            panelDb.Size = new Size(192, 68);
            panelDb.TabIndex = 0;
            // 
            // panelDbAccent
            // 
            panelDbAccent.BackColor = Color.FromArgb(34, 197, 94);
            panelDbAccent.Location = new Point(0, 0);
            panelDbAccent.Name = "panelDbAccent";
            panelDbAccent.Size = new Size(3, 68);
            panelDbAccent.TabIndex = 0;
            // 
            // lblDbTitle
            // 
            lblDbTitle.AutoSize = true;
            lblDbTitle.Font = new Font("Segoe UI", 7.5F);
            lblDbTitle.ForeColor = Color.Gray;
            lblDbTitle.Location = new Point(12, 8);
            lblDbTitle.Name = "lblDbTitle";
            lblDbTitle.Size = new Size(49, 12);
            lblDbTitle.TabIndex = 1;
            lblDbTitle.Text = "DATABASE";
            // 
            // lblDbStatus
            // 
            lblDbStatus.AutoSize = true;
            lblDbStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDbStatus.Location = new Point(12, 26);
            lblDbStatus.Name = "lblDbStatus";
            lblDbStatus.Size = new Size(0, 19);
            lblDbStatus.TabIndex = 2;
            // 
            // panelApi
            // 
            panelApi.BackColor = Color.White;
            panelApi.BorderStyle = BorderStyle.FixedSingle;
            panelApi.Controls.Add(panelApiAccent);
            panelApi.Controls.Add(lblApiTitle);
            panelApi.Controls.Add(lblApiStatus);
            panelApi.Location = new Point(210, 10);
            panelApi.Name = "panelApi";
            panelApi.Size = new Size(192, 68);
            panelApi.TabIndex = 1;
            // 
            // panelApiAccent
            // 
            panelApiAccent.BackColor = Color.FromArgb(59, 130, 246);
            panelApiAccent.Location = new Point(0, 0);
            panelApiAccent.Name = "panelApiAccent";
            panelApiAccent.Size = new Size(3, 68);
            panelApiAccent.TabIndex = 0;
            // 
            // lblApiTitle
            // 
            lblApiTitle.AutoSize = true;
            lblApiTitle.Font = new Font("Segoe UI", 7.5F);
            lblApiTitle.ForeColor = Color.Gray;
            lblApiTitle.Location = new Point(12, 8);
            lblApiTitle.Name = "lblApiTitle";
            lblApiTitle.Size = new Size(20, 12);
            lblApiTitle.TabIndex = 1;
            lblApiTitle.Text = "API";
            // 
            // lblApiStatus
            // 
            lblApiStatus.AutoSize = true;
            lblApiStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblApiStatus.Location = new Point(12, 26);
            lblApiStatus.Name = "lblApiStatus";
            lblApiStatus.Size = new Size(0, 19);
            lblApiStatus.TabIndex = 2;
            // 
            // panelNext
            // 
            panelNext.BackColor = Color.White;
            panelNext.BorderStyle = BorderStyle.FixedSingle;
            panelNext.Controls.Add(lblNextTitle);
            panelNext.Controls.Add(lblNextValue);
            panelNext.Controls.Add(lblNextSub);
            panelNext.Location = new Point(10, 86);
            panelNext.Name = "panelNext";
            panelNext.Size = new Size(120, 68);
            panelNext.TabIndex = 2;
            // 
            // lblNextTitle
            // 
            lblNextTitle.AutoSize = true;
            lblNextTitle.Font = new Font("Segoe UI", 7.5F);
            lblNextTitle.ForeColor = Color.Gray;
            lblNextTitle.Location = new Point(0, 6);
            lblNextTitle.Name = "lblNextTitle";
            lblNextTitle.Size = new Size(41, 12);
            lblNextTitle.TabIndex = 0;
            lblNextTitle.Text = "รอบถัดไป";
            lblNextTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblNextValue
            // 
            lblNextValue.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblNextValue.Location = new Point(0, 24);
            lblNextValue.Name = "lblNextValue";
            lblNextValue.Size = new Size(120, 28);
            lblNextValue.TabIndex = 1;
            lblNextValue.Text = "--:--";
            lblNextValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblNextSub
            // 
            lblNextSub.Font = new Font("Segoe UI", 7.5F);
            lblNextSub.ForeColor = Color.Gray;
            lblNextSub.Location = new Point(0, 50);
            lblNextSub.Name = "lblNextSub";
            lblNextSub.Size = new Size(120, 16);
            lblNextSub.TabIndex = 2;
            lblNextSub.Text = "นาที";
            lblNextSub.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelPending
            // 
            panelPending.BackColor = Color.White;
            panelPending.BorderStyle = BorderStyle.FixedSingle;
            panelPending.Controls.Add(panelPendingAccent);
            panelPending.Controls.Add(lblPendingTitle);
            panelPending.Controls.Add(lblPendingValue);
            panelPending.Controls.Add(lblPendingSub);
            panelPending.Location = new Point(138, 86);
            panelPending.Name = "panelPending";
            panelPending.Size = new Size(120, 68);
            panelPending.TabIndex = 3;
            // 
            // panelPendingAccent
            // 
            panelPendingAccent.BackColor = Color.FromArgb(245, 158, 11);
            panelPendingAccent.Location = new Point(0, 0);
            panelPendingAccent.Name = "panelPendingAccent";
            panelPendingAccent.Size = new Size(3, 68);
            panelPendingAccent.TabIndex = 0;
            // 
            // lblPendingTitle
            // 
            lblPendingTitle.AutoSize = true;
            lblPendingTitle.Font = new Font("Segoe UI", 7.5F);
            lblPendingTitle.ForeColor = Color.Gray;
            lblPendingTitle.Location = new Point(8, 6);
            lblPendingTitle.Name = "lblPendingTitle";
            lblPendingTitle.Size = new Size(52, 12);
            lblPendingTitle.TabIndex = 1;
            lblPendingTitle.Text = "รายการรอส่ง";
            // 
            // lblPendingValue
            // 
            lblPendingValue.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblPendingValue.ForeColor = Color.FromArgb(180, 83, 9);
            lblPendingValue.Location = new Point(8, 24);
            lblPendingValue.Name = "lblPendingValue";
            lblPendingValue.Size = new Size(110, 28);
            lblPendingValue.TabIndex = 2;
            lblPendingValue.Text = "0";
            // 
            // lblPendingSub
            // 
            lblPendingSub.Font = new Font("Segoe UI", 7.5F);
            lblPendingSub.ForeColor = Color.Gray;
            lblPendingSub.Location = new Point(8, 50);
            lblPendingSub.Name = "lblPendingSub";
            lblPendingSub.Size = new Size(110, 16);
            lblPendingSub.TabIndex = 3;
            lblPendingSub.Text = "รายการ";
            // 
            // panelSuccess
            // 
            panelSuccess.BackColor = Color.White;
            panelSuccess.BorderStyle = BorderStyle.FixedSingle;
            panelSuccess.Controls.Add(panelSuccessAccent);
            panelSuccess.Controls.Add(lblSuccessTitle);
            panelSuccess.Controls.Add(lblSuccessValue);
            panelSuccess.Controls.Add(lblSuccessSub);
            panelSuccess.Location = new Point(266, 86);
            panelSuccess.Name = "panelSuccess";
            panelSuccess.Size = new Size(136, 68);
            panelSuccess.TabIndex = 4;
            // 
            // panelSuccessAccent
            // 
            panelSuccessAccent.BackColor = Color.FromArgb(139, 92, 246);
            panelSuccessAccent.Location = new Point(0, 0);
            panelSuccessAccent.Name = "panelSuccessAccent";
            panelSuccessAccent.Size = new Size(3, 68);
            panelSuccessAccent.TabIndex = 0;
            // 
            // lblSuccessTitle
            // 
            lblSuccessTitle.AutoSize = true;
            lblSuccessTitle.Font = new Font("Segoe UI", 7.5F);
            lblSuccessTitle.ForeColor = Color.Gray;
            lblSuccessTitle.Location = new Point(8, 6);
            lblSuccessTitle.Name = "lblSuccessTitle";
            lblSuccessTitle.Size = new Size(36, 12);
            lblSuccessTitle.TabIndex = 1;
            lblSuccessTitle.Text = "ส่งสำเร็จ";
            // 
            // lblSuccessValue
            // 
            lblSuccessValue.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblSuccessValue.ForeColor = Color.FromArgb(109, 40, 217);
            lblSuccessValue.Location = new Point(8, 24);
            lblSuccessValue.Name = "lblSuccessValue";
            lblSuccessValue.Size = new Size(126, 28);
            lblSuccessValue.TabIndex = 2;
            lblSuccessValue.Text = "0";
            // 
            // lblSuccessSub
            // 
            lblSuccessSub.Font = new Font("Segoe UI", 7.5F);
            lblSuccessSub.ForeColor = Color.Gray;
            lblSuccessSub.Location = new Point(8, 50);
            lblSuccessSub.Name = "lblSuccessSub";
            lblSuccessSub.Size = new Size(126, 16);
            lblSuccessSub.TabIndex = 3;
            lblSuccessSub.Text = "วันนี้";
            // 
            // panelFooter
            // 
            panelFooter.BackColor = Color.FromArgb(245, 245, 245);
            panelFooter.BorderStyle = BorderStyle.FixedSingle;
            panelFooter.Controls.Add(lblRunning);
            panelFooter.Controls.Add(lblLastUpdate);
            panelFooter.Location = new Point(0, 162);
            panelFooter.Name = "panelFooter";
            panelFooter.Size = new Size(405, 28);
            panelFooter.TabIndex = 5;
            // 
            // lblRunning
            // 
            lblRunning.AutoSize = true;
            lblRunning.Font = new Font("Segoe UI", 8F);
            lblRunning.ForeColor = Color.FromArgb(34, 197, 94);
            lblRunning.Location = new Point(10, 6);
            lblRunning.Name = "lblRunning";
            lblRunning.Size = new Size(65, 13);
            lblRunning.TabIndex = 0;
            lblRunning.Text = "● กำลังทำงาน";
            // 
            // lblLastUpdate
            // 
            lblLastUpdate.AutoSize = true;
            lblLastUpdate.Font = new Font("Segoe UI", 8F);
            lblLastUpdate.ForeColor = Color.Gray;
            lblLastUpdate.Location = new Point(319, 6);
            lblLastUpdate.Name = "lblLastUpdate";
            lblLastUpdate.Size = new Size(0, 13);
            lblLastUpdate.TabIndex = 1;
            // 
            // ProudMedStatusAPI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 248, 248);
            ClientSize = new Size(417, 190);
            Controls.Add(panelDb);
            Controls.Add(panelApi);
            Controls.Add(panelNext);
            Controls.Add(panelPending);
            Controls.Add(panelSuccess);
            Controls.Add(panelFooter);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ProudMedStatusAPI";
            Text = "ProudMedStatusAPI";
            panelDb.ResumeLayout(false);
            panelDb.PerformLayout();
            panelApi.ResumeLayout(false);
            panelApi.PerformLayout();
            panelNext.ResumeLayout(false);
            panelNext.PerformLayout();
            panelPending.ResumeLayout(false);
            panelPending.PerformLayout();
            panelSuccess.ResumeLayout(false);
            panelSuccess.PerformLayout();
            panelFooter.ResumeLayout(false);
            panelFooter.PerformLayout();
            ResumeLayout(false);
        }

        private NotifyIcon notifyIcon1;
        private Panel panelDb, panelApi, panelNext, panelPending, panelSuccess, panelFooter;
        private Panel panelDbAccent, panelApiAccent, panelPendingAccent, panelSuccessAccent;
        private Label lblDbTitle, lblDbStatus;
        private Label lblApiTitle, lblApiStatus ;
        private Label lblNextTitle, lblNextValue, lblNextSub;
        private Label lblPendingTitle, lblPendingValue, lblPendingSub;
        private Label lblSuccessTitle, lblSuccessValue, lblSuccessSub;
        private Label lblRunning, lblLastUpdate;
    }
}