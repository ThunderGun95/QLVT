namespace QLVT.GUI
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuTacVu = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNhapKho = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuXuatKho = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTraKho = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHoanUng = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTacVu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // mnuTacVu
            // 
            this.mnuTacVu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNhapKho,
            this.mnuXuatKho,
            this.mnuTraKho,
            this.mnuHoanUng});
            this.mnuTacVu.Name = "mnuTacVu";
            this.mnuTacVu.Size = new System.Drawing.Size(55, 20);
            this.mnuTacVu.Text = "Tác vụ";
            // 
            // mnuNhapKho
            // 
            this.mnuNhapKho.Name = "mnuNhapKho";
            this.mnuNhapKho.Size = new System.Drawing.Size(162, 22);
            this.mnuNhapKho.Text = "📦 Nhập kho vật tư";
            this.mnuNhapKho.Click += new System.EventHandler(this.mnuNhapKho_Click);
            // 
            // mnuXuatKho
            // 
            this.mnuXuatKho.Name = "mnuXuatKho";
            this.mnuXuatKho.Size = new System.Drawing.Size(162, 22);
            this.mnuXuatKho.Text = "📤 Xuất kho vật tư";
            this.mnuXuatKho.Click += new System.EventHandler(this.mnuXuatKho_Click);
            // 
            // mnuTraKho
            // 
            this.mnuTraKho.Name = "mnuTraKho";
            this.mnuTraKho.Size = new System.Drawing.Size(162, 22);
            this.mnuTraKho.Text = "🔄 Trả kho vật tư";
            this.mnuTraKho.Click += new System.EventHandler(this.mnuTraKho_Click);
            // 
            // mnuHoanUng
            // 
            this.mnuHoanUng.Name = "mnuHoanUng";
            this.mnuHoanUng.Size = new System.Drawing.Size(162, 22);
            this.mnuHoanUng.Text = "↩️ Hoàn ứng vật tư";
            this.mnuHoanUng.Click += new System.EventHandler(this.mnuHoanUng_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblUser,
            this.lblTime});
            this.statusStrip.Location = new System.Drawing.Point(0, 726);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 24);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 19);
            this.lblStatus.Text = "Sẵn sàng";
            // 
            // lblUser
            // 
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(1026, 19);
            this.lblUser.Spring = true;
            this.lblUser.Text = "Người dùng: ";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTime
            // 
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(120, 19);
            this.lblTime.Text = "00:00:00 - 00/00/0000";
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlMain.Controls.Add(this.lblWelcome);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 24);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1200, 702);
            this.pnlMain.TabIndex = 2;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblWelcome.Location = new System.Drawing.Point(400, 300);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(400, 100);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Chào mừng đến với hệ thống\r\nQuản lý Vật tư (QLVT)";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 750);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QLVT - Hệ thống quản lý vật tư";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuTacVu;
        private System.Windows.Forms.ToolStripMenuItem mnuNhapKho;
        private System.Windows.Forms.ToolStripMenuItem mnuXuatKho;
        private System.Windows.Forms.ToolStripMenuItem mnuTraKho;
        private System.Windows.Forms.ToolStripMenuItem mnuHoanUng;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblUser;
        private System.Windows.Forms.ToolStripStatusLabel lblTime;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Timer timer;
    }
}
