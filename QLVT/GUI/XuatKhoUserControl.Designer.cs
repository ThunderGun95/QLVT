namespace QLVT.GUI
{
    partial class XuatKhoUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpThongTinPhieu;
        private Label lblKhoXuat;
        private ComboBox cboKhoXuat;
        private Label lblKhoNhan;
        private ComboBox cboKhoNhan;
        private Label lblNgayGhiNhan;
        private DateTimePicker dtpNgayGhiNhan;
        private Label lblLyDoXuat;
        private TextBox txtLyDoXuat;
        private Label lblGhiChu;
        private TextBox txtGhiChu;
        private GroupBox grpChiTiet;
        private DataGridView dgvChiTietPhieu;
        private Button btnThemVatTu;
        private Panel pnlButtons;
        private Button btnTaoMoi;
        private Button btnLuu;

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
            grpThongTinPhieu = new GroupBox();
            txtGhiChu = new TextBox();
            lblGhiChu = new Label();
            txtLyDoXuat = new TextBox();
            lblLyDoXuat = new Label();
            dtpNgayGhiNhan = new DateTimePicker();
            lblNgayGhiNhan = new Label();
            cboKhoNhan = new ComboBox();
            lblKhoNhan = new Label();
            cboKhoXuat = new ComboBox();
            lblKhoXuat = new Label();
            grpChiTiet = new GroupBox();
            btnThemVatTu = new Button();
            dgvChiTietPhieu = new DataGridView();
            pnlButtons = new Panel();
            btnLuu = new Button();
            btnTaoMoi = new Button();
            grpThongTinPhieu.SuspendLayout();
            grpChiTiet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTietPhieu).BeginInit();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 122, 204);
            lblTitle.Location = new Point(12, 9);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(1176, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "PHIẾU XUẤT KHO (NGOÀI ERP)";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpThongTinPhieu
            // 
            grpThongTinPhieu.Controls.Add(txtGhiChu);
            grpThongTinPhieu.Controls.Add(lblGhiChu);
            grpThongTinPhieu.Controls.Add(txtLyDoXuat);
            grpThongTinPhieu.Controls.Add(lblLyDoXuat);
            grpThongTinPhieu.Controls.Add(dtpNgayGhiNhan);
            grpThongTinPhieu.Controls.Add(lblNgayGhiNhan);
            grpThongTinPhieu.Controls.Add(cboKhoNhan);
            grpThongTinPhieu.Controls.Add(lblKhoNhan);
            grpThongTinPhieu.Controls.Add(cboKhoXuat);
            grpThongTinPhieu.Controls.Add(lblKhoXuat);
            grpThongTinPhieu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpThongTinPhieu.ForeColor = Color.FromArgb(0, 122, 204);
            grpThongTinPhieu.Location = new Point(12, 55);
            grpThongTinPhieu.Name = "grpThongTinPhieu";
            grpThongTinPhieu.Size = new Size(1176, 200);
            grpThongTinPhieu.TabIndex = 1;
            grpThongTinPhieu.TabStop = false;
            grpThongTinPhieu.Text = "Thông tin phiếu xuất";
            // 
            // txtGhiChu
            // 
            txtGhiChu.Font = new Font("Segoe UI", 9F);
            txtGhiChu.Location = new Point(130, 135);
            txtGhiChu.Multiline = true;
            txtGhiChu.Name = "txtGhiChu";
            txtGhiChu.ScrollBars = ScrollBars.Vertical;
            txtGhiChu.Size = new Size(1030, 50);
            txtGhiChu.TabIndex = 9;
            // 
            // lblGhiChu
            // 
            lblGhiChu.Font = new Font("Segoe UI", 9F);
            lblGhiChu.ForeColor = Color.Black;
            lblGhiChu.Location = new Point(20, 135);
            lblGhiChu.Name = "lblGhiChu";
            lblGhiChu.Size = new Size(100, 23);
            lblGhiChu.TabIndex = 8;
            lblGhiChu.Text = "Ghi chú:";
            lblGhiChu.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtLyDoXuat
            // 
            txtLyDoXuat.Font = new Font("Segoe UI", 9F);
            txtLyDoXuat.Location = new Point(130, 75);
            txtLyDoXuat.Multiline = true;
            txtLyDoXuat.Name = "txtLyDoXuat";
            txtLyDoXuat.ScrollBars = ScrollBars.Vertical;
            txtLyDoXuat.Size = new Size(1030, 50);
            txtLyDoXuat.TabIndex = 7;
            // 
            // lblLyDoXuat
            // 
            lblLyDoXuat.Font = new Font("Segoe UI", 9F);
            lblLyDoXuat.ForeColor = Color.Black;
            lblLyDoXuat.Location = new Point(20, 75);
            lblLyDoXuat.Name = "lblLyDoXuat";
            lblLyDoXuat.Size = new Size(100, 23);
            lblLyDoXuat.TabIndex = 6;
            lblLyDoXuat.Text = "Lý do xuất:";
            lblLyDoXuat.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dtpNgayGhiNhan
            // 
            dtpNgayGhiNhan.CustomFormat = "dd/MM/yyyy";
            dtpNgayGhiNhan.Font = new Font("Segoe UI", 9F);
            dtpNgayGhiNhan.Format = DateTimePickerFormat.Custom;
            dtpNgayGhiNhan.Location = new Point(1010, 35);
            dtpNgayGhiNhan.Name = "dtpNgayGhiNhan";
            dtpNgayGhiNhan.Size = new Size(150, 23);
            dtpNgayGhiNhan.TabIndex = 5;
            // 
            // lblNgayGhiNhan
            // 
            lblNgayGhiNhan.Font = new Font("Segoe UI", 9F);
            lblNgayGhiNhan.ForeColor = Color.Black;
            lblNgayGhiNhan.Location = new Point(880, 35);
            lblNgayGhiNhan.Name = "lblNgayGhiNhan";
            lblNgayGhiNhan.Size = new Size(120, 23);
            lblNgayGhiNhan.TabIndex = 4;
            lblNgayGhiNhan.Text = "Ngày ghi nhận S.S:";
            lblNgayGhiNhan.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboKhoNhan
            // 
            cboKhoNhan.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboKhoNhan.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboKhoNhan.Font = new Font("Segoe UI", 9F);
            cboKhoNhan.FormattingEnabled = true;
            cboKhoNhan.Location = new Point(560, 35);
            cboKhoNhan.Name = "cboKhoNhan";
            cboKhoNhan.Size = new Size(300, 23);
            cboKhoNhan.TabIndex = 3;
            cboKhoNhan.SelectedIndexChanged += CboKhoNhan_SelectedIndexChanged;
            // 
            // lblKhoNhan
            // 
            lblKhoNhan.Font = new Font("Segoe UI", 9F);
            lblKhoNhan.ForeColor = Color.Black;
            lblKhoNhan.Location = new Point(450, 35);
            lblKhoNhan.Name = "lblKhoNhan";
            lblKhoNhan.Size = new Size(100, 23);
            lblKhoNhan.TabIndex = 2;
            lblKhoNhan.Text = "Kho nhận:";
            lblKhoNhan.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboKhoXuat
            // 
            cboKhoXuat.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboKhoXuat.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboKhoXuat.Font = new Font("Segoe UI", 9F);
            cboKhoXuat.FormattingEnabled = true;
            cboKhoXuat.Location = new Point(130, 35);
            cboKhoXuat.Name = "cboKhoXuat";
            cboKhoXuat.Size = new Size(300, 23);
            cboKhoXuat.TabIndex = 1;
            // 
            // lblKhoXuat
            // 
            lblKhoXuat.Font = new Font("Segoe UI", 9F);
            lblKhoXuat.ForeColor = Color.Black;
            lblKhoXuat.Location = new Point(20, 35);
            lblKhoXuat.Name = "lblKhoXuat";
            lblKhoXuat.Size = new Size(100, 23);
            lblKhoXuat.TabIndex = 0;
            lblKhoXuat.Text = "Kho xuất:";
            lblKhoXuat.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpChiTiet
            // 
            grpChiTiet.Controls.Add(btnThemVatTu);
            grpChiTiet.Controls.Add(dgvChiTietPhieu);
            grpChiTiet.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpChiTiet.ForeColor = Color.FromArgb(0, 122, 204);
            grpChiTiet.Location = new Point(12, 270);
            grpChiTiet.Name = "grpChiTiet";
            grpChiTiet.Size = new Size(1176, 350);
            grpChiTiet.TabIndex = 2;
            grpChiTiet.TabStop = false;
            grpChiTiet.Text = "Chi tiết phiếu xuất";
            // 
            // btnThemVatTu
            // 
            btnThemVatTu.BackColor = Color.FromArgb(0, 122, 204);
            btnThemVatTu.FlatStyle = FlatStyle.Flat;
            btnThemVatTu.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnThemVatTu.ForeColor = Color.White;
            btnThemVatTu.Location = new Point(20, 30);
            btnThemVatTu.Name = "btnThemVatTu";
            btnThemVatTu.Size = new Size(120, 30);
            btnThemVatTu.TabIndex = 0;
            btnThemVatTu.Text = "Thêm vật tư";
            btnThemVatTu.UseVisualStyleBackColor = false;
            btnThemVatTu.Click += BtnThemVatTu_Click;
            // 
            // dgvChiTietPhieu
            // 
            dgvChiTietPhieu.AllowUserToAddRows = false;
            dgvChiTietPhieu.AllowUserToDeleteRows = false;
            dgvChiTietPhieu.BackgroundColor = Color.White;
            dgvChiTietPhieu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvChiTietPhieu.Location = new Point(20, 70);
            dgvChiTietPhieu.MultiSelect = false;
            dgvChiTietPhieu.Name = "dgvChiTietPhieu";
            dgvChiTietPhieu.RowHeadersVisible = false;
            dgvChiTietPhieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTietPhieu.Size = new Size(1140, 270);
            dgvChiTietPhieu.TabIndex = 1;
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnLuu);
            pnlButtons.Controls.Add(btnTaoMoi);
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Location = new Point(0, 640);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(1200, 60);
            pnlButtons.TabIndex = 3;
            // 
            // btnLuu
            // 
            btnLuu.BackColor = Color.FromArgb(40, 167, 69);
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLuu.ForeColor = Color.White;
            btnLuu.Location = new Point(1070, 15);
            btnLuu.Name = "btnLuu";
            btnLuu.Size = new Size(120, 35);
            btnLuu.TabIndex = 1;
            btnLuu.Text = "Lưu phiếu";
            btnLuu.UseVisualStyleBackColor = false;
            btnLuu.Click += BtnLuu_Click;
            // 
            // btnTaoMoi
            // 
            btnTaoMoi.BackColor = Color.FromArgb(220, 53, 69);
            btnTaoMoi.FlatStyle = FlatStyle.Flat;
            btnTaoMoi.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnTaoMoi.ForeColor = Color.White;
            btnTaoMoi.Location = new Point(940, 15);
            btnTaoMoi.Name = "btnTaoMoi";
            btnTaoMoi.Size = new Size(120, 35);
            btnTaoMoi.TabIndex = 0;
            btnTaoMoi.Text = "Tạo mới";
            btnTaoMoi.UseVisualStyleBackColor = false;
            btnTaoMoi.Click += BtnTaoMoi_Click;
            // 
            // XuatKhoUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 247);
            Controls.Add(pnlButtons);
            Controls.Add(grpChiTiet);
            Controls.Add(grpThongTinPhieu);
            Controls.Add(lblTitle);
            Name = "XuatKhoUserControl";
            Size = new Size(1200, 700);
            grpThongTinPhieu.ResumeLayout(false);
            grpThongTinPhieu.PerformLayout();
            grpChiTiet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvChiTietPhieu).EndInit();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}