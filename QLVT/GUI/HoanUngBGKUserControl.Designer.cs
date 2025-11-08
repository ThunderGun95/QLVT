namespace QLVT.GUI
{
    partial class HoanUngBGKUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpBGKInfo;
        private Label lblSoBGKLabel;
        private Label lblSoBGK;
        private Label lblTrangThaiLabel;
        private Label lblTrangThai;
        private Label lblNhanVienKyThuatLabel;
        private Label lblNhanVienKyThuat;
        private Label lblNhanVienXayLapLabel;
        private Label lblNhanVienXayLap;
        private Label lblNoiDungLabel;
        private Label lblNoiDung;
        private Label lblSoNghiemThuLabel;
        private Label lblSoNghiemThu;
        private GroupBox grpTimKiem;
        private Label lblSoBGKTimLabel;
        private TextBox txtSoBGK;
        private TextBox txtNam;
        private Label lblSeparator;
        private Button btnTimBGK;
        private Button btnRefresh;
        private Label lblConnectionStatus;
        private GroupBox grpChiTiet;
        private DataGridView dgvChiTiet;
        private Button btnXacNhan;
        private Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            lblTitle = new Label();
            grpTimKiem = new GroupBox();
            lblSoBGKTimLabel = new Label();
            txtSoBGK = new TextBox();
            lblSeparator = new Label();
            txtNam = new TextBox();
            btnTimBGK = new Button();
            btnRefresh = new Button();
            lblConnectionStatus = new Label();
            grpBGKInfo = new GroupBox();
            lblSoBGKLabel = new Label();
            lblSoBGK = new Label();
            lblTrangThaiLabel = new Label();
            lblTrangThai = new Label();
            lblNhanVienKyThuatLabel = new Label();
            lblNhanVienKyThuat = new Label();
            lblNhanVienXayLapLabel = new Label();
            lblNhanVienXayLap = new Label();
            lblNoiDungLabel = new Label();
            lblNoiDung = new Label();
            lblSoNghiemThuLabel = new Label();
            lblSoNghiemThu = new Label();
            grpChiTiet = new GroupBox();
            dgvChiTiet = new DataGridView();
            btnXacNhan = new Button();
            lblStatus = new Label();
            grpTimKiem.SuspendLayout();
            grpBGKInfo.SuspendLayout();
            grpChiTiet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTiet).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.FromArgb(41, 128, 185);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Arial", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Padding = new Padding(0, 5, 0, 5);
            lblTitle.Size = new Size(1200, 50);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "HOÀN ỨNG BGK";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpTimKiem
            // 
            grpTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpTimKiem.Controls.Add(lblSoBGKTimLabel);
            grpTimKiem.Controls.Add(txtSoBGK);
            grpTimKiem.Controls.Add(lblSeparator);
            grpTimKiem.Controls.Add(txtNam);
            grpTimKiem.Controls.Add(btnTimBGK);
            grpTimKiem.Controls.Add(btnRefresh);
            grpTimKiem.Controls.Add(lblConnectionStatus);
            grpTimKiem.Font = new Font("Arial", 11F, FontStyle.Bold);
            grpTimKiem.ForeColor = Color.FromArgb(52, 73, 94);
            grpTimKiem.Location = new Point(20, 65);
            grpTimKiem.Name = "grpTimKiem";
            grpTimKiem.Padding = new Padding(10);
            grpTimKiem.Size = new Size(1160, 90);
            grpTimKiem.TabIndex = 1;
            grpTimKiem.TabStop = false;
            grpTimKiem.Text = "🔍 Tìm kiếm BGK";
            // 
            // lblSoBGKTimLabel
            // 
            lblSoBGKTimLabel.AutoSize = true;
            lblSoBGKTimLabel.Font = new Font("Arial", 10.5F);
            lblSoBGKTimLabel.ForeColor = Color.FromArgb(52, 73, 94);
            lblSoBGKTimLabel.Location = new Point(25, 32);
            lblSoBGKTimLabel.Name = "lblSoBGKTimLabel";
            lblSoBGKTimLabel.Size = new Size(54, 17);
            lblSoBGKTimLabel.TabIndex = 0;
            lblSoBGKTimLabel.Text = "Số BGK:";
            // 
            // txtSoBGK
            // 
            txtSoBGK.Font = new Font("Arial", 11F);
            txtSoBGK.Location = new Point(95, 28);
            txtSoBGK.Name = "txtSoBGK";
            txtSoBGK.Size = new Size(110, 25);
            txtSoBGK.TabIndex = 1;
            txtSoBGK.KeyPress += txtSoBGK_KeyPress;
            // 
            // lblSeparator
            // 
            lblSeparator.AutoSize = true;
            lblSeparator.Font = new Font("Arial", 12F, FontStyle.Bold);
            lblSeparator.ForeColor = Color.FromArgb(52, 73, 94);
            lblSeparator.Location = new Point(210, 30);
            lblSeparator.Name = "lblSeparator";
            lblSeparator.Size = new Size(16, 20);
            lblSeparator.TabIndex = 2;
            lblSeparator.Text = "/";
            // 
            // txtNam
            // 
            txtNam.Font = new Font("Arial", 11F);
            txtNam.Location = new Point(225, 28);
            txtNam.Name = "txtNam";
            txtNam.Size = new Size(75, 25);
            txtNam.TabIndex = 3;
            txtNam.Text = "2025";
            txtNam.KeyPress += txtNam_KeyPress;
            // 
            // btnTimBGK
            // 
            btnTimBGK.BackColor = Color.FromArgb(52, 152, 219);
            btnTimBGK.FlatAppearance.BorderSize = 0;
            btnTimBGK.FlatStyle = FlatStyle.Flat;
            btnTimBGK.Font = new Font("Arial", 10.5F, FontStyle.Bold);
            btnTimBGK.ForeColor = Color.White;
            btnTimBGK.Location = new Point(320, 26);
            btnTimBGK.Name = "btnTimBGK";
            btnTimBGK.Size = new Size(100, 30);
            btnTimBGK.TabIndex = 4;
            btnTimBGK.Text = "🔎 Tìm kiếm";
            btnTimBGK.UseVisualStyleBackColor = false;
            btnTimBGK.Click += btnTimBGK_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(46, 204, 113);
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Arial", 10.5F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(430, 26);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.TabIndex = 5;
            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Font = new Font("Arial", 10F);
            lblConnectionStatus.ForeColor = Color.FromArgb(52, 152, 219);
            lblConnectionStatus.Location = new Point(25, 60);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(167, 15);
            lblConnectionStatus.TabIndex = 6;
            lblConnectionStatus.Text = "🔄 Đang kiểm tra kết nối ERP...";
            // 
            // grpBGKInfo
            // 
            grpBGKInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpBGKInfo.Controls.Add(lblSoBGKLabel);
            grpBGKInfo.Controls.Add(lblSoBGK);
            grpBGKInfo.Controls.Add(lblTrangThaiLabel);
            grpBGKInfo.Controls.Add(lblTrangThai);
            grpBGKInfo.Controls.Add(lblNhanVienKyThuatLabel);
            grpBGKInfo.Controls.Add(lblNhanVienKyThuat);
            grpBGKInfo.Controls.Add(lblNhanVienXayLapLabel);
            grpBGKInfo.Controls.Add(lblNhanVienXayLap);
            grpBGKInfo.Controls.Add(lblNoiDungLabel);
            grpBGKInfo.Controls.Add(lblNoiDung);
            grpBGKInfo.Controls.Add(lblSoNghiemThuLabel);
            grpBGKInfo.Controls.Add(lblSoNghiemThu);
            grpBGKInfo.Font = new Font("Arial", 11F, FontStyle.Bold);
            grpBGKInfo.ForeColor = Color.FromArgb(52, 73, 94);
            grpBGKInfo.Location = new Point(20, 165);
            grpBGKInfo.Name = "grpBGKInfo";
            grpBGKInfo.Padding = new Padding(10);
            grpBGKInfo.Size = new Size(1160, 120);
            grpBGKInfo.TabIndex = 2;
            grpBGKInfo.TabStop = false;
            grpBGKInfo.Text = "📋 Thông tin BGK";
            // 
            // lblSoBGKLabel
            // 
            lblSoBGKLabel.AutoSize = true;
            lblSoBGKLabel.Font = new Font("Arial", 10.5F);
            lblSoBGKLabel.ForeColor = Color.FromArgb(127, 140, 141);
            lblSoBGKLabel.Location = new Point(25, 32);
            lblSoBGKLabel.Name = "lblSoBGKLabel";
            lblSoBGKLabel.Size = new Size(54, 17);
            lblSoBGKLabel.TabIndex = 0;
            lblSoBGKLabel.Text = "Số BGK:";
            // 
            // lblSoBGK
            // 
            lblSoBGK.AutoSize = true;
            lblSoBGK.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblSoBGK.ForeColor = Color.FromArgb(41, 128, 185);
            lblSoBGK.Location = new Point(120, 32);
            lblSoBGK.Name = "lblSoBGK";
            lblSoBGK.Size = new Size(15, 19);
            lblSoBGK.TabIndex = 1;
            lblSoBGK.Text = "-";
            // 
            // lblTrangThaiLabel
            // 
            lblTrangThaiLabel.AutoSize = true;
            lblTrangThaiLabel.Font = new Font("Arial", 10.5F);
            lblTrangThaiLabel.ForeColor = Color.FromArgb(127, 140, 141);
            lblTrangThaiLabel.Location = new Point(350, 32);
            lblTrangThaiLabel.Name = "lblTrangThaiLabel";
            lblTrangThaiLabel.Size = new Size(69, 17);
            lblTrangThaiLabel.TabIndex = 8;
            lblTrangThaiLabel.Text = "Trạng thái:";
            // 
            // lblTrangThai
            // 
            lblTrangThai.AutoSize = true;
            lblTrangThai.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblTrangThai.ForeColor = Color.FromArgb(230, 126, 34);
            lblTrangThai.Location = new Point(440, 32);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(15, 19);
            lblTrangThai.TabIndex = 9;
            lblTrangThai.Text = "-";
            // 
            // lblNhanVienKyThuatLabel
            // 
            lblNhanVienKyThuatLabel.AutoSize = true;
            lblNhanVienKyThuatLabel.Font = new Font("Arial", 10.5F);
            lblNhanVienKyThuatLabel.ForeColor = Color.FromArgb(127, 140, 141);
            lblNhanVienKyThuatLabel.Location = new Point(600, 32);
            lblNhanVienKyThuatLabel.Name = "lblNhanVienKyThuatLabel";
            lblNhanVienKyThuatLabel.Size = new Size(80, 17);
            lblNhanVienKyThuatLabel.TabIndex = 10;
            lblNhanVienKyThuatLabel.Text = "NV Kỹ thuật:";
            // 
            // lblNhanVienKyThuat
            // 
            lblNhanVienKyThuat.AutoSize = true;
            lblNhanVienKyThuat.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblNhanVienKyThuat.ForeColor = Color.FromArgb(52, 73, 94);
            lblNhanVienKyThuat.Location = new Point(695, 32);
            lblNhanVienKyThuat.Name = "lblNhanVienKyThuat";
            lblNhanVienKyThuat.Size = new Size(15, 19);
            lblNhanVienKyThuat.TabIndex = 11;
            lblNhanVienKyThuat.Text = "-";
            // 
            // lblNhanVienXayLapLabel
            // 
            lblNhanVienXayLapLabel.AutoSize = true;
            lblNhanVienXayLapLabel.Font = new Font("Arial", 10.5F);
            lblNhanVienXayLapLabel.ForeColor = Color.FromArgb(127, 140, 141);
            lblNhanVienXayLapLabel.Location = new Point(25, 62);
            lblNhanVienXayLapLabel.Name = "lblNhanVienXayLapLabel";
            lblNhanVienXayLapLabel.Size = new Size(76, 17);
            lblNhanVienXayLapLabel.TabIndex = 12;
            lblNhanVienXayLapLabel.Text = "NV Xây lắp:";
            // 
            // lblNhanVienXayLap
            // 
            lblNhanVienXayLap.AutoSize = true;
            lblNhanVienXayLap.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblNhanVienXayLap.ForeColor = Color.FromArgb(52, 73, 94);
            lblNhanVienXayLap.Location = new Point(120, 62);
            lblNhanVienXayLap.Name = "lblNhanVienXayLap";
            lblNhanVienXayLap.Size = new Size(15, 19);
            lblNhanVienXayLap.TabIndex = 13;
            lblNhanVienXayLap.Text = "-";
            // 
            // lblNoiDungLabel
            // 
            lblNoiDungLabel.AutoSize = true;
            lblNoiDungLabel.Font = new Font("Arial", 10.5F);
            lblNoiDungLabel.ForeColor = Color.FromArgb(127, 140, 141);
            lblNoiDungLabel.Location = new Point(25, 92);
            lblNoiDungLabel.Name = "lblNoiDungLabel";
            lblNoiDungLabel.Size = new Size(66, 17);
            lblNoiDungLabel.TabIndex = 14;
            lblNoiDungLabel.Text = "Nội dung:";
            // 
            // lblNoiDung
            // 
            lblNoiDung.AutoSize = true;
            lblNoiDung.Font = new Font("Arial", 10.5F);
            lblNoiDung.ForeColor = Color.FromArgb(52, 73, 94);
            lblNoiDung.Location = new Point(120, 92);
            lblNoiDung.MaximumSize = new Size(1000, 0);
            lblNoiDung.Name = "lblNoiDung";
            lblNoiDung.Size = new Size(13, 17);
            lblNoiDung.TabIndex = 15;
            lblNoiDung.Text = "-";
            // 
            // lblSoNghiemThuLabel
            // 
            lblSoNghiemThuLabel.AutoSize = true;
            lblSoNghiemThuLabel.Font = new Font("Arial", 10.5F);
            lblSoNghiemThuLabel.ForeColor = Color.FromArgb(127, 140, 141);
            lblSoNghiemThuLabel.Location = new Point(350, 62);
            lblSoNghiemThuLabel.Name = "lblSoNghiemThuLabel";
            lblSoNghiemThuLabel.Size = new Size(95, 17);
            lblSoNghiemThuLabel.TabIndex = 16;
            lblSoNghiemThuLabel.Text = "Số nghiệm thu:";
            // 
            // lblSoNghiemThu
            // 
            lblSoNghiemThu.AutoSize = true;
            lblSoNghiemThu.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblSoNghiemThu.ForeColor = Color.FromArgb(41, 128, 185);
            lblSoNghiemThu.Location = new Point(460, 62);
            lblSoNghiemThu.Name = "lblSoNghiemThu";
            lblSoNghiemThu.Size = new Size(15, 19);
            lblSoNghiemThu.TabIndex = 17;
            lblSoNghiemThu.Text = "-";
            // 
            // grpChiTiet
            // 
            grpChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpChiTiet.Controls.Add(dgvChiTiet);
            grpChiTiet.Controls.Add(btnXacNhan);
            grpChiTiet.Font = new Font("Arial", 11F, FontStyle.Bold);
            grpChiTiet.ForeColor = Color.FromArgb(52, 73, 94);
            grpChiTiet.Location = new Point(20, 295);
            grpChiTiet.Name = "grpChiTiet";
            grpChiTiet.Padding = new Padding(10);
            grpChiTiet.Size = new Size(1160, 415);
            grpChiTiet.TabIndex = 3;
            grpChiTiet.TabStop = false;
            grpChiTiet.Text = "📦 Danh sách vật tư hoàn ứng";
            // 
            // dgvChiTiet
            // 
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvChiTiet.BackgroundColor = Color.White;
            dgvChiTiet.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.Padding = new Padding(5);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvChiTiet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvChiTiet.ColumnHeadersHeight = 35;
            dgvChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvChiTiet.EnableHeadersVisualStyles = false;
            dgvChiTiet.GridColor = Color.FromArgb(189, 195, 199);
            dgvChiTiet.Location = new Point(20, 30);
            dgvChiTiet.MultiSelect = false;
            dgvChiTiet.Name = "dgvChiTiet";
            dgvChiTiet.RowHeadersVisible = false;
            dgvChiTiet.RowTemplate.Height = 28;
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTiet.Size = new Size(1120, 330);
            dgvChiTiet.TabIndex = 0;
            // 
            // btnXacNhan
            // 
            btnXacNhan.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnXacNhan.BackColor = Color.FromArgb(230, 126, 34);
            btnXacNhan.FlatAppearance.BorderSize = 0;
            btnXacNhan.FlatStyle = FlatStyle.Flat;
            btnXacNhan.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnXacNhan.ForeColor = Color.White;
            btnXacNhan.Location = new Point(20, 367);
            btnXacNhan.Name = "btnXacNhan";
            btnXacNhan.Size = new Size(200, 40);
            btnXacNhan.TabIndex = 1;
            btnXacNhan.Text = "✅ Xác nhận hoàn ứng";
            btnXacNhan.UseVisualStyleBackColor = false;
            btnXacNhan.Click += btnXacNhan_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Arial", 10.5F);
            lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
            lblStatus.Location = new Point(25, 722);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(61, 17);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Sẵn sàng";
            // 
            // HoanUngBGKUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(236, 240, 241);
            Controls.Add(lblStatus);
            Controls.Add(grpChiTiet);
            Controls.Add(grpBGKInfo);
            Controls.Add(grpTimKiem);
            Controls.Add(lblTitle);
            Name = "HoanUngBGKUserControl";
            Size = new Size(1200, 753);
            Load += HoanUngBGKUserControl_Load;
            grpTimKiem.ResumeLayout(false);
            grpTimKiem.PerformLayout();
            grpBGKInfo.ResumeLayout(false);
            grpBGKInfo.PerformLayout();
            grpChiTiet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvChiTiet).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
