namespace QLVT.GUI
{
    partial class NhapKhoUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpThongTinPhieu;
        private Label lblKhoNhap;
        private ComboBox cboKhoNhap;
        private Label lblNguoiGiao;
        private TextBox txtNguoiGiao;
        private Label lblNgayGhiNhan;
        private DateTimePicker dtpNgayGhiNhan;
        private Label lblLyDoNhap;
        private TextBox txtLyDoNhap;
        private Label lblGhiChu;
        private TextBox txtGhiChu;
        private GroupBox grpChiTiet;
        private DataGridView dgvChiTietPhieu;
        private Button btnThemVatTu;
        private Panel pnlButtons;
        private Button btnTaoMoi;
        private Button btnLuu;
        private Button btnIn;

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
            dtpNgayGhiNhan = new DateTimePicker();
            lblNgayGhiNhan = new Label();
            cboKhoNhap = new ComboBox();
            lblKhoNhap = new Label();
            grpChiTiet = new GroupBox();
            btnThemVatTu = new Button();
            dgvChiTietPhieu = new DataGridView();
            pnlButtons = new Panel();
            btnIn = new Button();
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
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 122, 204);
            lblTitle.Location = new Point(18, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(130, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "NHẬP KHO";
            // 
            // grpThongTinPhieu
            // 
            grpThongTinPhieu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpThongTinPhieu.Controls.Add(txtGhiChu);
            grpThongTinPhieu.Controls.Add(lblGhiChu);
            grpThongTinPhieu.Controls.Add(dtpNgayGhiNhan);
            grpThongTinPhieu.Controls.Add(lblNgayGhiNhan);
            grpThongTinPhieu.Controls.Add(cboKhoNhap);
            grpThongTinPhieu.Controls.Add(lblKhoNhap);
            grpThongTinPhieu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpThongTinPhieu.ForeColor = Color.FromArgb(0, 100, 150);
            grpThongTinPhieu.Location = new Point(18, 52);
            grpThongTinPhieu.Margin = new Padding(3, 2, 3, 2);
            grpThongTinPhieu.Name = "grpThongTinPhieu";
            grpThongTinPhieu.Padding = new Padding(3, 2, 3, 2);
            grpThongTinPhieu.Size = new Size(665, 150);
            grpThongTinPhieu.TabIndex = 1;
            grpThongTinPhieu.TabStop = false;
            grpThongTinPhieu.Text = "Thông tin phiếu nhập";
            // 
            // txtGhiChu
            // 
            txtGhiChu.BorderStyle = BorderStyle.FixedSingle;
            txtGhiChu.Font = new Font("Segoe UI", 9F);
            txtGhiChu.Location = new Point(126, 84);
            txtGhiChu.Margin = new Padding(3, 2, 3, 2);
            txtGhiChu.Multiline = true;
            txtGhiChu.Name = "txtGhiChu";
            txtGhiChu.ScrollBars = ScrollBars.Vertical;
            txtGhiChu.Size = new Size(522, 53);
            txtGhiChu.TabIndex = 9;
            // 
            // lblGhiChu
            // 
            lblGhiChu.AutoSize = true;
            lblGhiChu.Font = new Font("Segoe UI", 9F);
            lblGhiChu.ForeColor = Color.FromArgb(64, 64, 64);
            lblGhiChu.Location = new Point(18, 86);
            lblGhiChu.Name = "lblGhiChu";
            lblGhiChu.Size = new Size(51, 15);
            lblGhiChu.TabIndex = 8;
            lblGhiChu.Text = "Ghi chú:";
            // 
            // dtpNgayGhiNhan
            // 
            dtpNgayGhiNhan.CustomFormat = "dd/MM/yyyy";
            dtpNgayGhiNhan.Font = new Font("Segoe UI", 9F);
            dtpNgayGhiNhan.Format = DateTimePickerFormat.Custom;
            dtpNgayGhiNhan.Location = new Point(126, 54);
            dtpNgayGhiNhan.Margin = new Padding(3, 2, 3, 2);
            dtpNgayGhiNhan.Name = "dtpNgayGhiNhan";
            dtpNgayGhiNhan.Size = new Size(176, 23);
            dtpNgayGhiNhan.TabIndex = 5;
            // 
            // lblNgayGhiNhan
            // 
            lblNgayGhiNhan.AutoSize = true;
            lblNgayGhiNhan.Font = new Font("Segoe UI", 9F);
            lblNgayGhiNhan.ForeColor = Color.FromArgb(64, 64, 64);
            lblNgayGhiNhan.Location = new Point(18, 56);
            lblNgayGhiNhan.Name = "lblNgayGhiNhan";
            lblNgayGhiNhan.Size = new Size(106, 15);
            lblNgayGhiNhan.TabIndex = 4;
            lblNgayGhiNhan.Text = "Ngày ghi nhận S.S:";
            // 
            // cboKhoNhap
            // 
            cboKhoNhap.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboKhoNhap.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboKhoNhap.Font = new Font("Segoe UI", 9F);
            cboKhoNhap.FormattingEnabled = true;
            cboKhoNhap.Location = new Point(126, 24);
            cboKhoNhap.Margin = new Padding(3, 2, 3, 2);
            cboKhoNhap.Name = "cboKhoNhap";
            cboKhoNhap.Size = new Size(176, 23);
            cboKhoNhap.TabIndex = 1;
            // 
            // lblKhoNhap
            // 
            lblKhoNhap.AutoSize = true;
            lblKhoNhap.Font = new Font("Segoe UI", 9F);
            lblKhoNhap.ForeColor = Color.FromArgb(64, 64, 64);
            lblKhoNhap.Location = new Point(18, 26);
            lblKhoNhap.Name = "lblKhoNhap";
            lblKhoNhap.Size = new Size(61, 15);
            lblKhoNhap.TabIndex = 0;
            lblKhoNhap.Text = "Kho nhập:";
            // 
            // grpChiTiet
            // 
            grpChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpChiTiet.Controls.Add(btnThemVatTu);
            grpChiTiet.Controls.Add(dgvChiTietPhieu);
            grpChiTiet.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grpChiTiet.ForeColor = Color.FromArgb(0, 100, 150);
            grpChiTiet.Location = new Point(18, 218);
            grpChiTiet.Margin = new Padding(3, 2, 3, 2);
            grpChiTiet.Name = "grpChiTiet";
            grpChiTiet.Padding = new Padding(3, 2, 3, 2);
            grpChiTiet.Size = new Size(665, 240);
            grpChiTiet.TabIndex = 2;
            grpChiTiet.TabStop = false;
            grpChiTiet.Text = "Chi tiết phiếu nhập";
            // 
            // btnThemVatTu
            // 
            btnThemVatTu.BackColor = Color.FromArgb(13, 110, 253);
            btnThemVatTu.FlatAppearance.BorderSize = 0;
            btnThemVatTu.FlatStyle = FlatStyle.Flat;
            btnThemVatTu.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnThemVatTu.ForeColor = Color.White;
            btnThemVatTu.Location = new Point(18, 26);
            btnThemVatTu.Margin = new Padding(3, 2, 3, 2);
            btnThemVatTu.Name = "btnThemVatTu";
            btnThemVatTu.Size = new Size(105, 26);
            btnThemVatTu.TabIndex = 0;
            btnThemVatTu.Text = "Thêm vật tư";
            btnThemVatTu.UseVisualStyleBackColor = false;
            // 
            // dgvChiTietPhieu
            // 
            dgvChiTietPhieu.AllowUserToAddRows = false;
            dgvChiTietPhieu.AllowUserToDeleteRows = false;
            dgvChiTietPhieu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvChiTietPhieu.BackgroundColor = Color.White;
            dgvChiTietPhieu.BorderStyle = BorderStyle.Fixed3D;
            dgvChiTietPhieu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvChiTietPhieu.Location = new Point(18, 60);
            dgvChiTietPhieu.Margin = new Padding(3, 2, 3, 2);
            dgvChiTietPhieu.MultiSelect = false;
            dgvChiTietPhieu.Name = "dgvChiTietPhieu";
            dgvChiTietPhieu.ReadOnly = false;
            dgvChiTietPhieu.RowHeadersWidth = 51;
            dgvChiTietPhieu.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvChiTietPhieu.Size = new Size(630, 165);
            dgvChiTietPhieu.TabIndex = 1;
            // 
            // pnlButtons
            // 
            pnlButtons.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlButtons.Controls.Add(btnIn);
            pnlButtons.Controls.Add(btnLuu);
            pnlButtons.Controls.Add(btnTaoMoi);
            pnlButtons.Location = new Point(18, 472);
            pnlButtons.Margin = new Padding(3, 2, 3, 2);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(665, 45);
            pnlButtons.TabIndex = 3;
            // 
            // btnIn
            // 
            btnIn.BackColor = Color.FromArgb(13, 110, 253);
            btnIn.FlatAppearance.BorderSize = 0;
            btnIn.FlatStyle = FlatStyle.Flat;
            btnIn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnIn.ForeColor = Color.White;
            btnIn.Location = new Point(262, 11);
            btnIn.Margin = new Padding(3, 2, 3, 2);
            btnIn.Name = "btnIn";
            btnIn.Size = new Size(105, 26);
            btnIn.TabIndex = 2;
            btnIn.Text = "In phiếu";
            btnIn.UseVisualStyleBackColor = false;
            // 
            // btnLuu
            // 
            btnLuu.BackColor = Color.FromArgb(25, 135, 84);
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLuu.ForeColor = Color.White;
            btnLuu.Location = new Point(140, 11);
            btnLuu.Margin = new Padding(3, 2, 3, 2);
            btnLuu.Name = "btnLuu";
            btnLuu.Size = new Size(105, 26);
            btnLuu.TabIndex = 1;
            btnLuu.Text = "Lưu phiếu";
            btnLuu.UseVisualStyleBackColor = false;
            // 
            // btnTaoMoi
            // 
            btnTaoMoi.BackColor = Color.FromArgb(220, 53, 69);
            btnTaoMoi.FlatAppearance.BorderSize = 0;
            btnTaoMoi.FlatStyle = FlatStyle.Flat;
            btnTaoMoi.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnTaoMoi.ForeColor = Color.White;
            btnTaoMoi.Location = new Point(18, 11);
            btnTaoMoi.Margin = new Padding(3, 2, 3, 2);
            btnTaoMoi.Name = "btnTaoMoi";
            btnTaoMoi.Size = new Size(105, 26);
            btnTaoMoi.TabIndex = 0;
            btnTaoMoi.Text = "Tạo mới";
            btnTaoMoi.UseVisualStyleBackColor = false;
            // 
            // NhapKhoUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 249, 250);
            Controls.Add(pnlButtons);
            Controls.Add(grpChiTiet);
            Controls.Add(grpThongTinPhieu);
            Controls.Add(lblTitle);
            Margin = new Padding(3, 2, 3, 2);
            Name = "NhapKhoUserControl";
            Size = new Size(700, 525);
            grpThongTinPhieu.ResumeLayout(false);
            grpThongTinPhieu.PerformLayout();
            grpChiTiet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvChiTietPhieu).EndInit();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}