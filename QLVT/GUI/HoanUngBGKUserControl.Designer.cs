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
        private DateTimePicker dtpNgayHoanUng;
        private Label lblNgayHoanUng;
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
            lblNgayHoanUng = new Label();
            dtpNgayHoanUng = new DateTimePicker();
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
            lblTitle.BackColor = Color.Navy;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(1200, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "HOÀN ỨNG BGK";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpTimKiem
            // 
            grpTimKiem.Controls.Add(lblSoBGKTimLabel);
            grpTimKiem.Controls.Add(txtSoBGK);
            grpTimKiem.Controls.Add(lblSeparator);
            grpTimKiem.Controls.Add(txtNam);
            grpTimKiem.Controls.Add(btnTimBGK);
            grpTimKiem.Controls.Add(btnRefresh);
            grpTimKiem.Controls.Add(lblConnectionStatus);
            grpTimKiem.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpTimKiem.Location = new Point(20, 50);
            grpTimKiem.Name = "grpTimKiem";
            grpTimKiem.Size = new Size(1160, 80);
            grpTimKiem.TabIndex = 1;
            grpTimKiem.TabStop = false;
            grpTimKiem.Text = "Tìm kiếm BGK";
            // 
            // lblSoBGKTimLabel
            // 
            lblSoBGKTimLabel.AutoSize = true;
            lblSoBGKTimLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblSoBGKTimLabel.Location = new Point(20, 25);
            lblSoBGKTimLabel.Name = "lblSoBGKTimLabel";
            lblSoBGKTimLabel.Size = new Size(53, 15);
            lblSoBGKTimLabel.TabIndex = 0;
            lblSoBGKTimLabel.Text = "Số BGK:";
            // 
            // txtSoBGK
            // 
            txtSoBGK.Font = new Font("Microsoft Sans Serif", 9F);
            txtSoBGK.Location = new Point(80, 22);
            txtSoBGK.Name = "txtSoBGK";
            txtSoBGK.Size = new Size(100, 21);
            txtSoBGK.TabIndex = 1;
            txtSoBGK.KeyPress += txtSoBGK_KeyPress;
            // 
            // lblSeparator
            // 
            lblSeparator.AutoSize = true;
            lblSeparator.Font = new Font("Microsoft Sans Serif", 9F);
            lblSeparator.Location = new Point(185, 25);
            lblSeparator.Name = "lblSeparator";
            lblSeparator.Size = new Size(10, 15);
            lblSeparator.TabIndex = 2;
            lblSeparator.Text = "/";
            // 
            // txtNam
            // 
            txtNam.Font = new Font("Microsoft Sans Serif", 9F);
            txtNam.Location = new Point(205, 22);
            txtNam.Name = "txtNam";
            txtNam.Size = new Size(60, 21);
            txtNam.TabIndex = 3;
            txtNam.Text = "2025";
            txtNam.KeyPress += txtNam_KeyPress;
            // 
            // btnTimBGK
            // 
            btnTimBGK.BackColor = Color.LightBlue;
            btnTimBGK.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnTimBGK.Location = new Point(280, 20);
            btnTimBGK.Name = "btnTimBGK";
            btnTimBGK.Size = new Size(80, 25);
            btnTimBGK.TabIndex = 4;
            btnTimBGK.Text = "Tìm BGK";
            btnTimBGK.UseVisualStyleBackColor = false;
            btnTimBGK.Click += btnTimBGK_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.LightGreen;
            btnRefresh.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnRefresh.Location = new Point(370, 20);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 25);
            btnRefresh.TabIndex = 5;
            btnRefresh.Text = "Làm mới";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblConnectionStatus.ForeColor = Color.Blue;
            lblConnectionStatus.Location = new Point(20, 50);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(178, 15);
            lblConnectionStatus.TabIndex = 6;
            lblConnectionStatus.Text = "🔄 Đang kiểm tra kết nối ERP...";
            // 
            // grpBGKInfo
            // 
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
            grpBGKInfo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpBGKInfo.Location = new Point(20, 140);
            grpBGKInfo.Name = "grpBGKInfo";
            grpBGKInfo.Size = new Size(1160, 110);
            grpBGKInfo.TabIndex = 2;
            grpBGKInfo.TabStop = false;
            grpBGKInfo.Text = "Thông tin BGK";
            // 
            // lblSoBGKLabel
            // 
            lblSoBGKLabel.AutoSize = true;
            lblSoBGKLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblSoBGKLabel.Location = new Point(20, 25);
            lblSoBGKLabel.Name = "lblSoBGKLabel";
            lblSoBGKLabel.Size = new Size(53, 15);
            lblSoBGKLabel.TabIndex = 0;
            lblSoBGKLabel.Text = "Số BGK:";
            // 
            // lblSoBGK
            // 
            lblSoBGK.AutoSize = true;
            lblSoBGK.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblSoBGK.ForeColor = Color.Blue;
            lblSoBGK.Location = new Point(80, 25);
            lblSoBGK.Name = "lblSoBGK";
            lblSoBGK.Size = new Size(0, 15);
            lblSoBGK.TabIndex = 1;
            // 
            // lblTrangThaiLabel
            // 
            lblTrangThaiLabel.AutoSize = true;
            lblTrangThaiLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblTrangThaiLabel.Location = new Point(250, 25);
            lblTrangThaiLabel.Name = "lblTrangThaiLabel";
            lblTrangThaiLabel.Size = new Size(65, 15);
            lblTrangThaiLabel.TabIndex = 8;
            lblTrangThaiLabel.Text = "Trạng thái:";
            // 
            // lblTrangThai
            // 
            lblTrangThai.AutoSize = true;
            lblTrangThai.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblTrangThai.ForeColor = Color.Orange;
            lblTrangThai.Location = new Point(325, 25);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(0, 15);
            lblTrangThai.TabIndex = 9;
            // 
            // lblNhanVienKyThuatLabel
            // 
            lblNhanVienKyThuatLabel.AutoSize = true;
            lblNhanVienKyThuatLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblNhanVienKyThuatLabel.Location = new Point(470, 25);
            lblNhanVienKyThuatLabel.Name = "lblNhanVienKyThuatLabel";
            lblNhanVienKyThuatLabel.Size = new Size(72, 15);
            lblNhanVienKyThuatLabel.TabIndex = 10;
            lblNhanVienKyThuatLabel.Text = "NV Kỹ thuật:";
            // 
            // lblNhanVienKyThuat
            // 
            lblNhanVienKyThuat.AutoSize = true;
            lblNhanVienKyThuat.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblNhanVienKyThuat.ForeColor = Color.Black;
            lblNhanVienKyThuat.Location = new Point(580, 25);
            lblNhanVienKyThuat.Name = "lblNhanVienKyThuat";
            lblNhanVienKyThuat.Size = new Size(0, 15);
            lblNhanVienKyThuat.TabIndex = 11;
            // 
            // lblNhanVienXayLapLabel
            // 
            lblNhanVienXayLapLabel.AutoSize = true;
            lblNhanVienXayLapLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblNhanVienXayLapLabel.Location = new Point(20, 50);
            lblNhanVienXayLapLabel.Name = "lblNhanVienXayLapLabel";
            lblNhanVienXayLapLabel.Size = new Size(69, 15);
            lblNhanVienXayLapLabel.TabIndex = 12;
            lblNhanVienXayLapLabel.Text = "NV Xây lắp:";
            // 
            // lblNhanVienXayLap
            // 
            lblNhanVienXayLap.AutoSize = true;
            lblNhanVienXayLap.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblNhanVienXayLap.ForeColor = Color.Black;
            lblNhanVienXayLap.Location = new Point(125, 50);
            lblNhanVienXayLap.Name = "lblNhanVienXayLap";
            lblNhanVienXayLap.Size = new Size(0, 15);
            lblNhanVienXayLap.TabIndex = 13;
            // 
            // lblNoiDungLabel
            // 
            lblNoiDungLabel.AutoSize = true;
            lblNoiDungLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblNoiDungLabel.Location = new Point(20, 75);
            lblNoiDungLabel.Name = "lblNoiDungLabel";
            lblNoiDungLabel.Size = new Size(60, 15);
            lblNoiDungLabel.TabIndex = 14;
            lblNoiDungLabel.Text = "Nội dung:";
            // 
            // lblNoiDung
            // 
            lblNoiDung.AutoSize = true;
            lblNoiDung.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblNoiDung.ForeColor = Color.Black;
            lblNoiDung.Location = new Point(95, 75);
            lblNoiDung.Name = "lblNoiDung";
            lblNoiDung.Size = new Size(0, 15);
            lblNoiDung.TabIndex = 15;
            // 
            // lblSoNghiemThuLabel
            // 
            lblSoNghiemThuLabel.AutoSize = true;
            lblSoNghiemThuLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblSoNghiemThuLabel.Location = new Point(250, 50);
            lblSoNghiemThuLabel.Name = "lblSoNghiemThuLabel";
            lblSoNghiemThuLabel.Size = new Size(90, 15);
            lblSoNghiemThuLabel.TabIndex = 16;
            lblSoNghiemThuLabel.Text = "Số nghiệm thu:";
            // 
            // lblSoNghiemThu
            // 
            lblSoNghiemThu.AutoSize = true;
            lblSoNghiemThu.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblSoNghiemThu.ForeColor = Color.Blue;
            lblSoNghiemThu.Location = new Point(340, 50);
            lblSoNghiemThu.Name = "lblSoNghiemThu";
            lblSoNghiemThu.Size = new Size(0, 15);
            lblSoNghiemThu.TabIndex = 17;
            // 
            // grpChiTiet
            // 
            grpChiTiet.Controls.Add(dgvChiTiet);
            grpChiTiet.Controls.Add(lblNgayHoanUng);
            grpChiTiet.Controls.Add(dtpNgayHoanUng);
            grpChiTiet.Controls.Add(btnXacNhan);
            grpChiTiet.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpChiTiet.Location = new Point(20, 260);
            grpChiTiet.Name = "grpChiTiet";
            grpChiTiet.Size = new Size(1160, 430);
            grpChiTiet.TabIndex = 3;
            grpChiTiet.TabStop = false;
            grpChiTiet.Text = "Danh sách vật tư hoàn ứng";
            // 
            // dgvChiTiet
            // 
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.BackgroundColor = Color.White;
            dgvChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvChiTiet.Location = new Point(20, 25);
            dgvChiTiet.MultiSelect = false;
            dgvChiTiet.Name = "dgvChiTiet";
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTiet.Size = new Size(1120, 350);
            dgvChiTiet.TabIndex = 0;
            // 
            // lblNgayHoanUng
            // 
            lblNgayHoanUng.AutoSize = true;
            lblNgayHoanUng.Font = new Font("Microsoft Sans Serif", 9F);
            lblNgayHoanUng.Location = new Point(20, 390);
            lblNgayHoanUng.Name = "lblNgayHoanUng";
            lblNgayHoanUng.Size = new Size(93, 15);
            lblNgayHoanUng.TabIndex = 1;
            lblNgayHoanUng.Text = "Ngày hoàn ứng:";
            // 
            // dtpNgayHoanUng
            // 
            dtpNgayHoanUng.Font = new Font("Microsoft Sans Serif", 9F);
            dtpNgayHoanUng.Format = DateTimePickerFormat.Short;
            dtpNgayHoanUng.Location = new Point(130, 387);
            dtpNgayHoanUng.Name = "dtpNgayHoanUng";
            dtpNgayHoanUng.Size = new Size(120, 21);
            dtpNgayHoanUng.TabIndex = 2;
            // 
            // btnXacNhan
            // 
            btnXacNhan.BackColor = Color.Orange;
            btnXacNhan.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnXacNhan.ForeColor = Color.White;
            btnXacNhan.Location = new Point(270, 385);
            btnXacNhan.Name = "btnXacNhan";
            btnXacNhan.Size = new Size(150, 30);
            btnXacNhan.TabIndex = 3;
            btnXacNhan.Text = "Xác nhận hoàn ứng";
            btnXacNhan.UseVisualStyleBackColor = false;
            btnXacNhan.Click += btnXacNhan_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblStatus.ForeColor = Color.Green;
            lblStatus.Location = new Point(20, 700);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(59, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Sẵn sàng";
            // 
            // HoanUngBGKUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(lblStatus);
            Controls.Add(grpChiTiet);
            Controls.Add(grpBGKInfo);
            Controls.Add(grpTimKiem);
            Controls.Add(lblTitle);
            Name = "HoanUngBGKUserControl";
            Size = new Size(1200, 720);
            Load += HoanUngBGKUserControl_Load;
            grpTimKiem.ResumeLayout(false);
            grpTimKiem.PerformLayout();
            grpBGKInfo.ResumeLayout(false);
            grpBGKInfo.PerformLayout();
            grpChiTiet.ResumeLayout(false);
            grpChiTiet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTiet).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}