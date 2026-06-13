namespace QLVT.GUI
{
    partial class ChiTietVatTuDCForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpHoSoInfo;
        private Label lblMaDonLabel;
        private Label lblMaDon;
        private Label lblViTriDiemChayLabel;
        private Label lblViTriDiemChay;
        private Label lblNVTaiCongLabel;
        private Label lblNVTaiCong;
        private GroupBox grpChiTiet;
        private DataGridView dgvChiTiet;
        private Label lblTongSoLuong;
        private Label lblSoLoaiVT;
        private Button btnHoanUng;
        private Button btnDong;

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
            grpHoSoInfo = new GroupBox();
            lblMaDonLabel = new Label();
            lblMaDon = new Label();
            lblViTriDiemChayLabel = new Label();
            lblViTriDiemChay = new Label();
            lblNVTaiCongLabel = new Label();
            lblNVTaiCong = new Label();
            grpChiTiet = new GroupBox();
            lblTongSoLuong = new Label();
            dgvChiTiet = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            lblSoLoaiVT = new Label();
            btnHoanUng = new Button();
            btnDong = new Button();
            grpHoSoInfo.SuspendLayout();
            grpChiTiet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTiet).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = QLVT.Utils.UIFonts.TitleLarge;
            lblTitle.ForeColor = Color.FromArgb(52, 152, 219);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(300, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "CHI TIẾT VẬT TƯ SỬA CHỮA";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpHoSoInfo
            // 
            grpHoSoInfo.Controls.Add(lblMaDonLabel);
            grpHoSoInfo.Controls.Add(lblMaDon);
            grpHoSoInfo.Controls.Add(lblViTriDiemChayLabel);
            grpHoSoInfo.Controls.Add(lblViTriDiemChay);
            grpHoSoInfo.Controls.Add(lblNVTaiCongLabel);
            grpHoSoInfo.Controls.Add(lblNVTaiCong);
            grpHoSoInfo.Font = QLVT.Utils.UIFonts.HeaderStandard;
            grpHoSoInfo.Location = new Point(20, 60);
            grpHoSoInfo.Name = "grpHoSoInfo";
            grpHoSoInfo.Size = new Size(830, 120);
            grpHoSoInfo.TabIndex = 1;
            grpHoSoInfo.TabStop = false;
            grpHoSoInfo.Text = "Thông tin đơn sửa chữa";
            // 
            // lblMaDonLabel
            // 
            lblMaDonLabel.Font = QLVT.Utils.UIFonts.HeaderStandard;
            lblMaDonLabel.Location = new Point(15, 25);
            lblMaDonLabel.Name = "lblMaDonLabel";
            lblMaDonLabel.Size = new Size(80, 20);
            lblMaDonLabel.TabIndex = 0;
            lblMaDonLabel.Text = "Mã đơn:";
            // 
            // lblMaDon
            // 
            lblMaDon.Font = QLVT.Utils.UIFonts.TextStandard;
            lblMaDon.Location = new Point(112, 25);
            lblMaDon.Name = "lblMaDon";
            lblMaDon.Size = new Size(120, 20);
            lblMaDon.TabIndex = 1;
            // 
            // lblViTriDiemChayLabel
            // 
            lblViTriDiemChayLabel.Font = QLVT.Utils.UIFonts.HeaderStandard;
            lblViTriDiemChayLabel.Location = new Point(16, 50);
            lblViTriDiemChayLabel.Name = "lblViTriDiemChayLabel";
            lblViTriDiemChayLabel.Size = new Size(90, 20);
            lblViTriDiemChayLabel.TabIndex = 6;
            lblViTriDiemChayLabel.Text = "Vị trí điểm chảy:";
            // 
            // lblViTriDiemChay
            // 
            lblViTriDiemChay.Font = QLVT.Utils.UIFonts.TextStandard;
            lblViTriDiemChay.Location = new Point(112, 50);
            lblViTriDiemChay.Name = "lblViTriDiemChay";
            lblViTriDiemChay.Size = new Size(200, 20);
            lblViTriDiemChay.TabIndex = 7;
            // 
            // lblNVTaiCongLabel
            // 
            lblNVTaiCongLabel.Font = QLVT.Utils.UIFonts.HeaderStandard;
            lblNVTaiCongLabel.Location = new Point(16, 75);
            lblNVTaiCongLabel.Name = "lblNVTaiCongLabel";
            lblNVTaiCongLabel.Size = new Size(90, 20);
            lblNVTaiCongLabel.TabIndex = 8;
            lblNVTaiCongLabel.Text = "NV Thi công:";
            // 
            // lblNVTaiCong
            // 
            lblNVTaiCong.Font = QLVT.Utils.UIFonts.TextStandard;
            lblNVTaiCong.Location = new Point(112, 75);
            lblNVTaiCong.Name = "lblNVTaiCong";
            lblNVTaiCong.Size = new Size(200, 20);
            lblNVTaiCong.TabIndex = 9;
            // 
            // grpChiTiet
            // 
            grpChiTiet.Controls.Add(lblTongSoLuong);
            grpChiTiet.Controls.Add(dgvChiTiet);
            grpChiTiet.Controls.Add(lblSoLoaiVT);
            grpChiTiet.Font = QLVT.Utils.UIFonts.HeaderStandard;
            grpChiTiet.Location = new Point(20, 190);
            grpChiTiet.Name = "grpChiTiet";
            grpChiTiet.Size = new Size(830, 350);
            grpChiTiet.TabIndex = 2;
            grpChiTiet.TabStop = false;
            grpChiTiet.Text = "Chi tiết vật tư sửa chữa";
            // 
            // lblTongSoLuong
            // 
            lblTongSoLuong.Font = QLVT.Utils.UIFonts.HeaderStandard;
            lblTongSoLuong.ForeColor = Color.FromArgb(52, 152, 219);
            lblTongSoLuong.Location = new Point(15, 25);
            lblTongSoLuong.Name = "lblTongSoLuong";
            lblTongSoLuong.Size = new Size(400, 20);
            lblTongSoLuong.TabIndex = 1;
            lblTongSoLuong.Text = "Tổng số lượng: 0";
            // 
            // dgvChiTiet
            // 
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.BackgroundColor = Color.White;
            dgvChiTiet.BorderStyle = BorderStyle.Fixed3D;
            dgvChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvChiTiet.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5 });
            dgvChiTiet.Location = new Point(15, 50);
            dgvChiTiet.Name = "dgvChiTiet";
            dgvChiTiet.ReadOnly = true;
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChiTiet.Size = new Size(800, 270);
            dgvChiTiet.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Mã vật tư";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn2.HeaderText = "Tên vật tư";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Số lượng";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Đơn vị";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Tồn kho";
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // lblSoLoaiVT
            // 
            lblSoLoaiVT.Font = QLVT.Utils.UIFonts.TextStandard;
            lblSoLoaiVT.Location = new Point(450, 25);
            lblSoLoaiVT.Name = "lblSoLoaiVT";
            lblSoLoaiVT.Size = new Size(200, 20);
            lblSoLoaiVT.TabIndex = 2;
            lblSoLoaiVT.Text = "Số loại vật tư: 0";
            // 
            // btnHoanUng
            // 
            btnHoanUng.BackColor = Color.FromArgb(220, 53, 69);
            btnHoanUng.FlatStyle = FlatStyle.Flat;
            btnHoanUng.Font = QLVT.Utils.UIFonts.HeaderStandard;
            btnHoanUng.ForeColor = Color.White;
            btnHoanUng.Location = new Point(670, 550);
            btnHoanUng.Name = "btnHoanUng";
            btnHoanUng.Size = new Size(90, 35);
            btnHoanUng.TabIndex = 4;
            btnHoanUng.Text = "Hoàn ứng";
            btnHoanUng.UseVisualStyleBackColor = false;
            btnHoanUng.Click += BtnHoanUng_Click;
            // 
            // btnDong
            // 
            btnDong.BackColor = Color.FromArgb(108, 117, 125);
            btnDong.FlatStyle = FlatStyle.Flat;
            btnDong.Font = QLVT.Utils.UIFonts.HeaderStandard;
            btnDong.ForeColor = Color.White;
            btnDong.Location = new Point(770, 550);
            btnDong.Name = "btnDong";
            btnDong.Size = new Size(80, 35);
            btnDong.TabIndex = 5;
            btnDong.Text = "Đóng";
            btnDong.UseVisualStyleBackColor = false;
            btnDong.Click += BtnDong_Click;
            // 
            // ChiTietVatTuDCForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(870, 600);
            Controls.Add(lblTitle);
            Controls.Add(grpHoSoInfo);
            Controls.Add(grpChiTiet);
            Controls.Add(btnHoanUng);
            Controls.Add(btnDong);
            Font = QLVT.Utils.UIFonts.TextStandard;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChiTietVatTuDCForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chi tiết vật tư sửa chữa sự cố";
            grpHoSoInfo.ResumeLayout(false);
            grpChiTiet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvChiTiet).EndInit();
            ResumeLayout(false);
        }
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    }
}

