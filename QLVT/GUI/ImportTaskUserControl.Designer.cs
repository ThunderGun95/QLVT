namespace QLVT.GUI
{
    partial class ImportTaskUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpPhieuInfo;
        private Label lblSoPhieuLabel;
        private Label lblSoPhieu;
        private Label lblNgayTaoLabel;
        private Label lblNgayTao;
        private Label lblKhoNhapLabel;
        private Label lblKhoNhap;
        private Label lblNguoiTaoLabel;
        private Label lblNguoiTao;
        private Label lblTrangThaiLabel;
        private Label lblTrangThai;
        private GroupBox grpTimKiem;
        private Label lblSoPhieuTimLabel;
        private TextBox txtSoPhieu;
        private TextBox txtNam;
        private Label lblSeparator;
        private Button btnTimPhieu;
        private Button btnRefresh;
        private Label lblConnectionStatus;
        private GroupBox grpChiTiet;
        private DataGridView dgvChiTiet;
        private Label lblMappingStatus;
        private Button btnMapping;
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

        private void InitializeComponent()
        {
            lblTitle = new Label();
            grpTimKiem = new GroupBox();
            lblSoPhieuTimLabel = new Label();
            txtSoPhieu = new TextBox();
            lblSeparator = new Label();
            txtNam = new TextBox();
            btnTimPhieu = new Button();
            btnRefresh = new Button();
            lblConnectionStatus = new Label();
            grpPhieuInfo = new GroupBox();
            lblSoPhieuLabel = new Label();
            lblSoPhieu = new Label();
            lblNgayTaoLabel = new Label();
            lblNgayTao = new Label();
            lblKhoNhapLabel = new Label();
            lblKhoNhap = new Label();
            lblNguoiTaoLabel = new Label();
            lblNguoiTao = new Label();
            lblTrangThaiLabel = new Label();
            lblTrangThai = new Label();
            grpChiTiet = new GroupBox();
            lblMappingStatus = new Label();
            dgvChiTiet = new DataGridView();
            btnMapping = new Button();
            btnXacNhan = new Button();
            lblStatus = new Label();
            grpTimKiem.SuspendLayout();
            grpPhieuInfo.SuspendLayout();
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
            lblTitle.Text = "NHẬP KHO VẬT TƯ";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpTimKiem
            // 
            grpTimKiem.Controls.Add(lblSoPhieuTimLabel);
            grpTimKiem.Controls.Add(txtSoPhieu);
            grpTimKiem.Controls.Add(lblSeparator);
            grpTimKiem.Controls.Add(txtNam);
            grpTimKiem.Controls.Add(btnTimPhieu);
            grpTimKiem.Controls.Add(btnRefresh);
            grpTimKiem.Controls.Add(lblConnectionStatus);
            grpTimKiem.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpTimKiem.Location = new Point(20, 50);
            grpTimKiem.Name = "grpTimKiem";
            grpTimKiem.Size = new Size(1160, 80);
            grpTimKiem.TabIndex = 1;
            grpTimKiem.TabStop = false;
            grpTimKiem.Text = "Tìm kiếm phiếu nhập";
            // 
            // lblSoPhieuTimLabel
            // 
            lblSoPhieuTimLabel.AutoSize = true;
            lblSoPhieuTimLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblSoPhieuTimLabel.Location = new Point(15, 25);
            lblSoPhieuTimLabel.Name = "lblSoPhieuTimLabel";
            lblSoPhieuTimLabel.Size = new Size(59, 15);
            lblSoPhieuTimLabel.TabIndex = 0;
            lblSoPhieuTimLabel.Text = "Số phiếu:";
            // 
            // txtSoPhieu
            // 
            txtSoPhieu.Font = new Font("Microsoft Sans Serif", 10F);
            txtSoPhieu.Location = new Point(80, 22);
            txtSoPhieu.Name = "txtSoPhieu";
            txtSoPhieu.Size = new Size(80, 23);
            txtSoPhieu.TabIndex = 1;
            txtSoPhieu.KeyPress += txtSoPhieu_KeyPress;
            // 
            // lblSeparator
            // 
            lblSeparator.AutoSize = true;
            lblSeparator.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            lblSeparator.Location = new Point(165, 25);
            lblSeparator.Name = "lblSeparator";
            lblSeparator.Size = new Size(15, 20);
            lblSeparator.TabIndex = 2;
            lblSeparator.Text = "-";
            // 
            // txtNam
            // 
            txtNam.Font = new Font("Microsoft Sans Serif", 10F);
            txtNam.Location = new Point(185, 22);
            txtNam.Name = "txtNam";
            txtNam.Size = new Size(60, 23);
            txtNam.TabIndex = 3;
            txtNam.KeyPress += txtNam_KeyPress;
            // 
            // btnTimPhieu
            // 
            btnTimPhieu.BackColor = Color.FromArgb(0, 123, 255);
            btnTimPhieu.FlatStyle = FlatStyle.Flat;
            btnTimPhieu.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnTimPhieu.ForeColor = Color.White;
            btnTimPhieu.Location = new Point(255, 20);
            btnTimPhieu.Name = "btnTimPhieu";
            btnTimPhieu.Size = new Size(100, 27);
            btnTimPhieu.TabIndex = 4;
            btnTimPhieu.Text = "🔍 Tìm phiếu";
            btnTimPhieu.UseVisualStyleBackColor = false;
            btnTimPhieu.Click += btnTimPhieu_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Microsoft Sans Serif", 9F);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(365, 20);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 27);
            btnRefresh.TabIndex = 5;
            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblConnectionStatus.Location = new Point(15, 50);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(132, 15);
            lblConnectionStatus.TabIndex = 6;
            lblConnectionStatus.Text = "Đang kiểm tra kết nối...";
            // 
            // grpPhieuInfo
            // 
            grpPhieuInfo.Controls.Add(lblSoPhieuLabel);
            grpPhieuInfo.Controls.Add(lblSoPhieu);
            grpPhieuInfo.Controls.Add(lblNgayTaoLabel);
            grpPhieuInfo.Controls.Add(lblNgayTao);
            grpPhieuInfo.Controls.Add(lblKhoNhapLabel);
            grpPhieuInfo.Controls.Add(lblKhoNhap);
            grpPhieuInfo.Controls.Add(lblNguoiTaoLabel);
            grpPhieuInfo.Controls.Add(lblNguoiTao);
            grpPhieuInfo.Controls.Add(lblTrangThaiLabel);
            grpPhieuInfo.Controls.Add(lblTrangThai);
            grpPhieuInfo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpPhieuInfo.Location = new Point(20, 140);
            grpPhieuInfo.Name = "grpPhieuInfo";
            grpPhieuInfo.Size = new Size(719, 120);
            grpPhieuInfo.TabIndex = 2;
            grpPhieuInfo.TabStop = false;
            grpPhieuInfo.Text = "Thông tin phiếu nhập";
            // 
            // lblSoPhieuLabel
            // 
            lblSoPhieuLabel.AutoSize = true;
            lblSoPhieuLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblSoPhieuLabel.Location = new Point(15, 25);
            lblSoPhieuLabel.Name = "lblSoPhieuLabel";
            lblSoPhieuLabel.Size = new Size(59, 15);
            lblSoPhieuLabel.TabIndex = 0;
            lblSoPhieuLabel.Text = "Số phiếu:";
            // 
            // lblSoPhieu
            // 
            lblSoPhieu.AutoSize = true;
            lblSoPhieu.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblSoPhieu.ForeColor = Color.Blue;
            lblSoPhieu.Location = new Point(100, 25);
            lblSoPhieu.Name = "lblSoPhieu";
            lblSoPhieu.Size = new Size(12, 15);
            lblSoPhieu.TabIndex = 1;
            lblSoPhieu.Text = "-";
            // 
            // lblNgayTaoLabel
            // 
            lblNgayTaoLabel.AutoSize = true;
            lblNgayTaoLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblNgayTaoLabel.Location = new Point(15, 50);
            lblNgayTaoLabel.Name = "lblNgayTaoLabel";
            lblNgayTaoLabel.Size = new Size(58, 15);
            lblNgayTaoLabel.TabIndex = 2;
            lblNgayTaoLabel.Text = "Ngày tạo:";
            // 
            // lblNgayTao
            // 
            lblNgayTao.AutoSize = true;
            lblNgayTao.Font = new Font("Microsoft Sans Serif", 9F);
            lblNgayTao.Location = new Point(100, 50);
            lblNgayTao.Name = "lblNgayTao";
            lblNgayTao.Size = new Size(11, 15);
            lblNgayTao.TabIndex = 3;
            lblNgayTao.Text = "-";
            // 
            // lblKhoNhapLabel
            // 
            lblKhoNhapLabel.AutoSize = true;
            lblKhoNhapLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblKhoNhapLabel.Location = new Point(15, 75);
            lblKhoNhapLabel.Name = "lblKhoNhapLabel";
            lblKhoNhapLabel.Size = new Size(63, 15);
            lblKhoNhapLabel.TabIndex = 4;
            lblKhoNhapLabel.Text = "Kho nhập:";
            // 
            // lblKhoNhap
            // 
            lblKhoNhap.AutoSize = true;
            lblKhoNhap.Font = new Font("Microsoft Sans Serif", 9F);
            lblKhoNhap.Location = new Point(100, 75);
            lblKhoNhap.Name = "lblKhoNhap";
            lblKhoNhap.Size = new Size(11, 15);
            lblKhoNhap.TabIndex = 5;
            lblKhoNhap.Text = "-";
            // 
            // lblNguoiTaoLabel
            // 
            lblNguoiTaoLabel.AutoSize = true;
            lblNguoiTaoLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblNguoiTaoLabel.Location = new Point(350, 25);
            lblNguoiTaoLabel.Name = "lblNguoiTaoLabel";
            lblNguoiTaoLabel.Size = new Size(63, 15);
            lblNguoiTaoLabel.TabIndex = 6;
            lblNguoiTaoLabel.Text = "Người tạo:";
            // 
            // lblNguoiTao
            // 
            lblNguoiTao.AutoSize = true;
            lblNguoiTao.Font = new Font("Microsoft Sans Serif", 9F);
            lblNguoiTao.Location = new Point(430, 25);
            lblNguoiTao.Name = "lblNguoiTao";
            lblNguoiTao.Size = new Size(11, 15);
            lblNguoiTao.TabIndex = 7;
            lblNguoiTao.Text = "-";
            // 
            // lblTrangThaiLabel
            // 
            lblTrangThaiLabel.AutoSize = true;
            lblTrangThaiLabel.Font = new Font("Microsoft Sans Serif", 9F);
            lblTrangThaiLabel.Location = new Point(350, 50);
            lblTrangThaiLabel.Name = "lblTrangThaiLabel";
            lblTrangThaiLabel.Size = new Size(65, 15);
            lblTrangThaiLabel.TabIndex = 8;
            lblTrangThaiLabel.Text = "Trạng thái:";
            // 
            // lblTrangThai
            // 
            lblTrangThai.AutoSize = true;
            lblTrangThai.Font = new Font("Microsoft Sans Serif", 9F);
            lblTrangThai.Location = new Point(430, 50);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(11, 15);
            lblTrangThai.TabIndex = 9;
            lblTrangThai.Text = "-";
            // 
            // grpChiTiet
            // 
            grpChiTiet.Controls.Add(lblMappingStatus);
            grpChiTiet.Controls.Add(dgvChiTiet);
            grpChiTiet.Controls.Add(btnMapping);
            grpChiTiet.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpChiTiet.Location = new Point(20, 270);
            grpChiTiet.Name = "grpChiTiet";
            grpChiTiet.Size = new Size(1160, 350);
            grpChiTiet.TabIndex = 4;
            grpChiTiet.TabStop = false;
            grpChiTiet.Text = "Chi tiết phiếu nhập";
            // 
            // lblMappingStatus
            // 
            lblMappingStatus.AutoSize = true;
            lblMappingStatus.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblMappingStatus.Location = new Point(15, 25);
            lblMappingStatus.Name = "lblMappingStatus";
            lblMappingStatus.Size = new Size(107, 15);
            lblMappingStatus.TabIndex = 0;
            lblMappingStatus.Text = "Chưa có dữ liệu";
            // 
            // dgvChiTiet
            // 
            dgvChiTiet.BackgroundColor = Color.White;
            dgvChiTiet.BorderStyle = BorderStyle.Fixed3D;
            dgvChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvChiTiet.Location = new Point(15, 50);
            dgvChiTiet.Name = "dgvChiTiet";
            dgvChiTiet.RowHeadersWidth = 30;
            dgvChiTiet.Size = new Size(1130, 250);
            dgvChiTiet.TabIndex = 1;
            // 
            // btnMapping
            // 
            btnMapping.BackColor = Color.FromArgb(255, 193, 7);
            btnMapping.FlatStyle = FlatStyle.Flat;
            btnMapping.Font = new Font("Microsoft Sans Serif", 9F);
            btnMapping.ForeColor = Color.Black;
            btnMapping.Location = new Point(15, 310);
            btnMapping.Name = "btnMapping";
            btnMapping.Size = new Size(120, 30);
            btnMapping.TabIndex = 2;
            btnMapping.Text = "🔗 Mapping thủ công";
            btnMapping.UseVisualStyleBackColor = false;
            btnMapping.Click += btnMapping_Click;
            // 
            // btnXacNhan
            // 
            btnXacNhan.BackColor = Color.FromArgb(40, 167, 69);
            btnXacNhan.Enabled = false;
            btnXacNhan.FlatStyle = FlatStyle.Flat;
            btnXacNhan.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            btnXacNhan.ForeColor = Color.White;
            btnXacNhan.Location = new Point(769, 180);
            btnXacNhan.Name = "btnXacNhan";
            btnXacNhan.Size = new Size(150, 50);
            btnXacNhan.TabIndex = 5;
            btnXacNhan.Text = "✅ XÁC NHẬN\nNHẬP KHO";
            btnXacNhan.UseVisualStyleBackColor = false;
            btnXacNhan.Click += btnXacNhan_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblStatus.Location = new Point(20, 640);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(68, 15);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Sẵn sàng...";
            // 
            // ImportTaskUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(lblTitle);
            Controls.Add(grpTimKiem);
            Controls.Add(grpPhieuInfo);
            Controls.Add(grpChiTiet);
            Controls.Add(btnXacNhan);
            Controls.Add(lblStatus);
            Name = "ImportTaskUserControl";
            Size = new Size(1200, 640);
            grpTimKiem.ResumeLayout(false);
            grpTimKiem.PerformLayout();
            grpPhieuInfo.ResumeLayout(false);
            grpPhieuInfo.PerformLayout();
            grpChiTiet.ResumeLayout(false);
            grpChiTiet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTiet).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
