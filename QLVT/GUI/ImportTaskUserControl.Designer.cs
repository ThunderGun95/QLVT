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
        private Label lblNhaCungCapLabel;
        private Label lblNhaCungCap;
        private Label lblNguoiTaoLabel;
        private Label lblNguoiTao;
        private Label lblTrangThaiLabel;
        private Label lblTrangThai;
        private Label lblGhiChuLabel;
        private TextBox txtGhiChu;
        private GroupBox grpTimKiem;
        private Label lblSoPhieuTimLabel;
        private TextBox txtSoPhieu;
        private TextBox txtNam;
        private Label lblSeparator;
        private Button btnTimPhieu;
        private Button btnRefresh;
        private Label lblConnectionStatus;
        private GroupBox grpKho;
        private Label lblKhoLabel;
        private ComboBox cmbKho;
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
            this.lblTitle = new Label();
            this.grpTimKiem = new GroupBox();
            this.lblSoPhieuTimLabel = new Label();
            this.txtSoPhieu = new TextBox();
            this.txtNam = new TextBox();
            this.lblSeparator = new Label();
            this.btnTimPhieu = new Button();
            this.btnRefresh = new Button();
            this.lblConnectionStatus = new Label();
            this.grpPhieuInfo = new GroupBox();
            this.lblSoPhieuLabel = new Label();
            this.lblSoPhieu = new Label();
            this.lblNgayTaoLabel = new Label();
            this.lblNgayTao = new Label();
            this.lblNhaCungCapLabel = new Label();
            this.lblNhaCungCap = new Label();
            this.lblNguoiTaoLabel = new Label();
            this.lblNguoiTao = new Label();
            this.lblTrangThaiLabel = new Label();
            this.lblTrangThai = new Label();
            this.lblGhiChuLabel = new Label();
            this.txtGhiChu = new TextBox();
            this.grpKho = new GroupBox();
            this.lblKhoLabel = new Label();
            this.cmbKho = new ComboBox();
            this.grpChiTiet = new GroupBox();
            this.dgvChiTiet = new DataGridView();
            this.lblMappingStatus = new Label();
            this.btnMapping = new Button();
            this.btnXacNhan = new Button();
            this.lblStatus = new Label();
            
            this.grpTimKiem.SuspendLayout();
            this.grpPhieuInfo.SuspendLayout();
            this.grpKho.SuspendLayout();
            this.grpChiTiet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.BackColor = Color.Navy;
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(1200, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "NHẬP KHO VẬT TƯ";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // grpTimKiem
            this.grpTimKiem.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpTimKiem.Location = new Point(20, 50);
            this.grpTimKiem.Name = "grpTimKiem";
            this.grpTimKiem.Size = new Size(1160, 80);
            this.grpTimKiem.TabIndex = 1;
            this.grpTimKiem.TabStop = false;
            this.grpTimKiem.Text = "Tìm kiếm phiếu nhập";

            // lblSoPhieuTimLabel
            this.lblSoPhieuTimLabel.AutoSize = true;
            this.lblSoPhieuTimLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblSoPhieuTimLabel.Location = new Point(15, 25);
            this.lblSoPhieuTimLabel.Name = "lblSoPhieuTimLabel";
            this.lblSoPhieuTimLabel.Size = new Size(60, 15);
            this.lblSoPhieuTimLabel.TabIndex = 0;
            this.lblSoPhieuTimLabel.Text = "Số phiếu:";

            // txtSoPhieu
            this.txtSoPhieu.Font = new Font("Microsoft Sans Serif", 10F);
            this.txtSoPhieu.Location = new Point(80, 22);
            this.txtSoPhieu.Name = "txtSoPhieu";
            this.txtSoPhieu.Size = new Size(80, 23);
            this.txtSoPhieu.TabIndex = 1;
            this.txtSoPhieu.KeyPress += new KeyPressEventHandler(this.txtSoPhieu_KeyPress);

            // lblSeparator
            this.lblSeparator.AutoSize = true;
            this.lblSeparator.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            this.lblSeparator.Location = new Point(165, 25);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new Size(15, 20);
            this.lblSeparator.TabIndex = 2;
            this.lblSeparator.Text = "-";

            // txtNam
            this.txtNam.Font = new Font("Microsoft Sans Serif", 10F);
            this.txtNam.Location = new Point(185, 22);
            this.txtNam.Name = "txtNam";
            this.txtNam.Size = new Size(60, 23);
            this.txtNam.TabIndex = 3;
            this.txtNam.Text = DateTime.Now.Year.ToString();
            this.txtNam.KeyPress += new KeyPressEventHandler(this.txtNam_KeyPress);

            // btnTimPhieu
            this.btnTimPhieu.BackColor = Color.FromArgb(0, 123, 255);
            this.btnTimPhieu.FlatStyle = FlatStyle.Flat;
            this.btnTimPhieu.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.btnTimPhieu.ForeColor = Color.White;
            this.btnTimPhieu.Location = new Point(255, 20);
            this.btnTimPhieu.Name = "btnTimPhieu";
            this.btnTimPhieu.Size = new Size(100, 27);
            this.btnTimPhieu.TabIndex = 4;
            this.btnTimPhieu.Text = "🔍 Tìm phiếu";
            this.btnTimPhieu.UseVisualStyleBackColor = false;
            this.btnTimPhieu.Click += new EventHandler(this.btnTimPhieu_Click);

            // btnRefresh
            this.btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.Font = new Font("Microsoft Sans Serif", 9F);
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Location = new Point(365, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(80, 27);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // lblConnectionStatus
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblConnectionStatus.Location = new Point(15, 50);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new Size(120, 15);
            this.lblConnectionStatus.TabIndex = 6;
            this.lblConnectionStatus.Text = "Đang kiểm tra kết nối...";

            // grpPhieuInfo
            this.grpPhieuInfo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpPhieuInfo.Location = new Point(20, 140);
            this.grpPhieuInfo.Name = "grpPhieuInfo";
            this.grpPhieuInfo.Size = new Size(700, 150);
            this.grpPhieuInfo.TabIndex = 2;
            this.grpPhieuInfo.TabStop = false;
            this.grpPhieuInfo.Text = "Thông tin phiếu nhập";

            // lblSoPhieuLabel
            this.lblSoPhieuLabel.AutoSize = true;
            this.lblSoPhieuLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblSoPhieuLabel.Location = new Point(15, 25);
            this.lblSoPhieuLabel.Name = "lblSoPhieuLabel";
            this.lblSoPhieuLabel.Size = new Size(60, 15);
            this.lblSoPhieuLabel.TabIndex = 0;
            this.lblSoPhieuLabel.Text = "Số phiếu:";

            // lblSoPhieu
            this.lblSoPhieu.AutoSize = true;
            this.lblSoPhieu.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.lblSoPhieu.ForeColor = Color.Blue;
            this.lblSoPhieu.Location = new Point(100, 25);
            this.lblSoPhieu.Name = "lblSoPhieu";
            this.lblSoPhieu.Size = new Size(12, 15);
            this.lblSoPhieu.TabIndex = 1;
            this.lblSoPhieu.Text = "-";

            // lblNgayTaoLabel
            this.lblNgayTaoLabel.AutoSize = true;
            this.lblNgayTaoLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblNgayTaoLabel.Location = new Point(15, 50);
            this.lblNgayTaoLabel.Name = "lblNgayTaoLabel";
            this.lblNgayTaoLabel.Size = new Size(62, 15);
            this.lblNgayTaoLabel.TabIndex = 2;
            this.lblNgayTaoLabel.Text = "Ngày tạo:";

            // lblNgayTao
            this.lblNgayTao.AutoSize = true;
            this.lblNgayTao.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblNgayTao.Location = new Point(100, 50);
            this.lblNgayTao.Name = "lblNgayTao";
            this.lblNgayTao.Size = new Size(12, 15);
            this.lblNgayTao.TabIndex = 3;
            this.lblNgayTao.Text = "-";

            // lblNhaCungCapLabel
            this.lblNhaCungCapLabel.AutoSize = true;
            this.lblNhaCungCapLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblNhaCungCapLabel.Location = new Point(15, 75);
            this.lblNhaCungCapLabel.Name = "lblNhaCungCapLabel";
            this.lblNhaCungCapLabel.Size = new Size(86, 15);
            this.lblNhaCungCapLabel.TabIndex = 4;
            this.lblNhaCungCapLabel.Text = "Nhà cung cấp:";

            // lblNhaCungCap
            this.lblNhaCungCap.AutoSize = true;
            this.lblNhaCungCap.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblNhaCungCap.Location = new Point(120, 75);
            this.lblNhaCungCap.Name = "lblNhaCungCap";
            this.lblNhaCungCap.Size = new Size(12, 15);
            this.lblNhaCungCap.TabIndex = 5;
            this.lblNhaCungCap.Text = "-";

            // lblNguoiTaoLabel
            this.lblNguoiTaoLabel.AutoSize = true;
            this.lblNguoiTaoLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblNguoiTaoLabel.Location = new Point(350, 25);
            this.lblNguoiTaoLabel.Name = "lblNguoiTaoLabel";
            this.lblNguoiTaoLabel.Size = new Size(66, 15);
            this.lblNguoiTaoLabel.TabIndex = 6;
            this.lblNguoiTaoLabel.Text = "Người tạo:";

            // lblNguoiTao
            this.lblNguoiTao.AutoSize = true;
            this.lblNguoiTao.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblNguoiTao.Location = new Point(430, 25);
            this.lblNguoiTao.Name = "lblNguoiTao";
            this.lblNguoiTao.Size = new Size(12, 15);
            this.lblNguoiTao.TabIndex = 7;
            this.lblNguoiTao.Text = "-";

            // lblTrangThaiLabel
            this.lblTrangThaiLabel.AutoSize = true;
            this.lblTrangThaiLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblTrangThaiLabel.Location = new Point(350, 50);
            this.lblTrangThaiLabel.Name = "lblTrangThaiLabel";
            this.lblTrangThaiLabel.Size = new Size(69, 15);
            this.lblTrangThaiLabel.TabIndex = 8;
            this.lblTrangThaiLabel.Text = "Trạng thái:";

            // lblTrangThai
            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblTrangThai.Location = new Point(430, 50);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new Size(12, 15);
            this.lblTrangThai.TabIndex = 9;
            this.lblTrangThai.Text = "-";

            // lblGhiChuLabel
            this.lblGhiChuLabel.AutoSize = true;
            this.lblGhiChuLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblGhiChuLabel.Location = new Point(15, 100);
            this.lblGhiChuLabel.Name = "lblGhiChuLabel";
            this.lblGhiChuLabel.Size = new Size(53, 15);
            this.lblGhiChuLabel.TabIndex = 10;
            this.lblGhiChuLabel.Text = "Ghi chú:";

            // txtGhiChu
            this.txtGhiChu.Font = new Font("Microsoft Sans Serif", 9F);
            this.txtGhiChu.Location = new Point(100, 97);
            this.txtGhiChu.Multiline = true;
            this.txtGhiChu.Name = "txtGhiChu";
            this.txtGhiChu.ReadOnly = true;
            this.txtGhiChu.Size = new Size(580, 40);
            this.txtGhiChu.TabIndex = 11;

            // grpKho
            this.grpKho.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpKho.Location = new Point(730, 140);
            this.grpKho.Name = "grpKho";
            this.grpKho.Size = new Size(450, 80);
            this.grpKho.TabIndex = 3;
            this.grpKho.TabStop = false;
            this.grpKho.Text = "Chọn kho đích";

            // lblKhoLabel
            this.lblKhoLabel.AutoSize = true;
            this.lblKhoLabel.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblKhoLabel.Location = new Point(15, 35);
            this.lblKhoLabel.Name = "lblKhoLabel";
            this.lblKhoLabel.Size = new Size(31, 15);
            this.lblKhoLabel.TabIndex = 0;
            this.lblKhoLabel.Text = "Kho:";

            // cmbKho
            this.cmbKho.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbKho.Font = new Font("Microsoft Sans Serif", 10F);
            this.cmbKho.FormattingEnabled = true;
            this.cmbKho.Location = new Point(60, 32);
            this.cmbKho.Name = "cmbKho";
            this.cmbKho.Size = new Size(370, 24);
            this.cmbKho.TabIndex = 1;
            this.cmbKho.SelectedIndexChanged += new EventHandler(this.cmbKho_SelectedIndexChanged);

            // grpChiTiet
            this.grpChiTiet.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpChiTiet.Location = new Point(20, 300);
            this.grpChiTiet.Name = "grpChiTiet";
            this.grpChiTiet.Size = new Size(1160, 350);
            this.grpChiTiet.TabIndex = 4;
            this.grpChiTiet.TabStop = false;
            this.grpChiTiet.Text = "Chi tiết phiếu nhập";

            // dgvChiTiet
            this.dgvChiTiet.BackgroundColor = Color.White;
            this.dgvChiTiet.BorderStyle = BorderStyle.Fixed3D;
            this.dgvChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChiTiet.Location = new Point(15, 50);
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.RowHeadersWidth = 30;
            this.dgvChiTiet.Size = new Size(1130, 250);
            this.dgvChiTiet.TabIndex = 1;

            // lblMappingStatus
            this.lblMappingStatus.AutoSize = true;
            this.lblMappingStatus.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.lblMappingStatus.Location = new Point(15, 25);
            this.lblMappingStatus.Name = "lblMappingStatus";
            this.lblMappingStatus.Size = new Size(100, 15);
            this.lblMappingStatus.TabIndex = 0;
            this.lblMappingStatus.Text = "Chưa có dữ liệu";

            // btnMapping
            this.btnMapping.BackColor = Color.FromArgb(255, 193, 7);
            this.btnMapping.FlatStyle = FlatStyle.Flat;
            this.btnMapping.Font = new Font("Microsoft Sans Serif", 9F);
            this.btnMapping.ForeColor = Color.Black;
            this.btnMapping.Location = new Point(15, 310);
            this.btnMapping.Name = "btnMapping";
            this.btnMapping.Size = new Size(120, 30);
            this.btnMapping.TabIndex = 2;
            this.btnMapping.Text = "🔗 Mapping thủ công";
            this.btnMapping.UseVisualStyleBackColor = false;
            this.btnMapping.Click += new EventHandler(this.btnMapping_Click);

            // btnXacNhan
            this.btnXacNhan.BackColor = Color.FromArgb(40, 167, 69);
            this.btnXacNhan.Enabled = false;
            this.btnXacNhan.FlatStyle = FlatStyle.Flat;
            this.btnXacNhan.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            this.btnXacNhan.ForeColor = Color.White;
            this.btnXacNhan.Location = new Point(730, 230);
            this.btnXacNhan.Name = "btnXacNhan";
            this.btnXacNhan.Size = new Size(150, 50);
            this.btnXacNhan.TabIndex = 5;
            this.btnXacNhan.Text = "✅ XÁC NHẬN\nNHẬP KHO";
            this.btnXacNhan.UseVisualStyleBackColor = false;
            this.btnXacNhan.Click += new EventHandler(this.btnXacNhan_Click);

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblStatus.Location = new Point(20, 670);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(70, 15);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Sẵn sàng...";

            // Add controls to groups
            this.grpTimKiem.Controls.Add(this.lblSoPhieuTimLabel);
            this.grpTimKiem.Controls.Add(this.txtSoPhieu);
            this.grpTimKiem.Controls.Add(this.lblSeparator);
            this.grpTimKiem.Controls.Add(this.txtNam);
            this.grpTimKiem.Controls.Add(this.btnTimPhieu);
            this.grpTimKiem.Controls.Add(this.btnRefresh);
            this.grpTimKiem.Controls.Add(this.lblConnectionStatus);

            this.grpPhieuInfo.Controls.Add(this.lblSoPhieuLabel);
            this.grpPhieuInfo.Controls.Add(this.lblSoPhieu);
            this.grpPhieuInfo.Controls.Add(this.lblNgayTaoLabel);
            this.grpPhieuInfo.Controls.Add(this.lblNgayTao);
            this.grpPhieuInfo.Controls.Add(this.lblNhaCungCapLabel);
            this.grpPhieuInfo.Controls.Add(this.lblNhaCungCap);
            this.grpPhieuInfo.Controls.Add(this.lblNguoiTaoLabel);
            this.grpPhieuInfo.Controls.Add(this.lblNguoiTao);
            this.grpPhieuInfo.Controls.Add(this.lblTrangThaiLabel);
            this.grpPhieuInfo.Controls.Add(this.lblTrangThai);
            this.grpPhieuInfo.Controls.Add(this.lblGhiChuLabel);
            this.grpPhieuInfo.Controls.Add(this.txtGhiChu);

            this.grpKho.Controls.Add(this.lblKhoLabel);
            this.grpKho.Controls.Add(this.cmbKho);

            this.grpChiTiet.Controls.Add(this.lblMappingStatus);
            this.grpChiTiet.Controls.Add(this.dgvChiTiet);
            this.grpChiTiet.Controls.Add(this.btnMapping);

            // ImportTaskUserControl
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.grpTimKiem);
            this.Controls.Add(this.grpPhieuInfo);
            this.Controls.Add(this.grpKho);
            this.Controls.Add(this.grpChiTiet);
            this.Controls.Add(this.btnXacNhan);
            this.Controls.Add(this.lblStatus);
            this.Name = "ImportTaskUserControl";
            this.Size = new Size(1200, 700);
            
            this.grpTimKiem.ResumeLayout(false);
            this.grpTimKiem.PerformLayout();
            this.grpPhieuInfo.ResumeLayout(false);
            this.grpPhieuInfo.PerformLayout();
            this.grpKho.ResumeLayout(false);
            this.grpKho.PerformLayout();
            this.grpChiTiet.ResumeLayout(false);
            this.grpChiTiet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
