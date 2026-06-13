namespace QLVT.GUI
{
    partial class ChiTietVatTuMC4Form
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpHoSoInfo;
        private Label lblMaDDKLabel;
        private Label lblMaDDK;
        private Label lblTenKHLabel;
        private Label lblTenKH;
        private Label lblDiaChiLabel;
        private Label lblDiaChi;
        private Label lblNVTaiCongLabel;
        private Label lblNVTaiCong;
        private GroupBox grpChiTiet;
        private DataGridView dgvChiTiet;
        private Label lblTongSoLuong;
        private Label lblSoLoaiVT;
        private Button btnDong;
        private Button btnHoanUng;

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
            lblMaDDKLabel = new Label();
            lblMaDDK = new Label();
            lblTenKHLabel = new Label();
            lblTenKH = new Label();
            lblDiaChiLabel = new Label();
            lblDiaChi = new Label();
            lblNVTaiCongLabel = new Label();
            lblNVTaiCong = new Label();
            grpChiTiet = new GroupBox();
            dgvChiTiet = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            lblTongSoLuong = new Label();
            lblSoLoaiVT = new Label();
            btnDong = new Button();
            btnHoanUng = new Button();
            btnHoanUngAm = new Button();
            grpHoSoInfo.SuspendLayout();
            grpChiTiet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChiTiet).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Arial", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 152, 219);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(300, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "CHI TIẾT VẬT TƯ MẠNG CẤP 4";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpHoSoInfo
            // 
            grpHoSoInfo.Controls.Add(lblMaDDKLabel);
            grpHoSoInfo.Controls.Add(lblMaDDK);
            grpHoSoInfo.Controls.Add(lblTenKHLabel);
            grpHoSoInfo.Controls.Add(lblTenKH);
            grpHoSoInfo.Controls.Add(lblDiaChiLabel);
            grpHoSoInfo.Controls.Add(lblDiaChi);
            grpHoSoInfo.Controls.Add(lblNVTaiCongLabel);
            grpHoSoInfo.Controls.Add(lblNVTaiCong);
            grpHoSoInfo.Font = new Font("Arial", 11F, FontStyle.Bold);
            grpHoSoInfo.Location = new Point(20, 60);
            grpHoSoInfo.Name = "grpHoSoInfo";
            grpHoSoInfo.Size = new Size(830, 100);
            grpHoSoInfo.TabIndex = 1;
            grpHoSoInfo.TabStop = false;
            grpHoSoInfo.Text = "Thông tin hồ sơ";
            // 
            // lblMaDDKLabel
            // 
            lblMaDDKLabel.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblMaDDKLabel.Location = new Point(15, 25);
            lblMaDDKLabel.Name = "lblMaDDKLabel";
            lblMaDDKLabel.Size = new Size(80, 20);
            lblMaDDKLabel.TabIndex = 0;
            lblMaDDKLabel.Text = "Mã DDK:";
            // 
            // lblMaDDK
            // 
            lblMaDDK.Font = new Font("Arial", 10.5F);
            lblMaDDK.Location = new Point(100, 25);
            lblMaDDK.Name = "lblMaDDK";
            lblMaDDK.Size = new Size(120, 20);
            lblMaDDK.TabIndex = 1;
            // 
            // lblTenKHLabel
            // 
            lblTenKHLabel.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblTenKHLabel.Location = new Point(240, 25);
            lblTenKHLabel.Name = "lblTenKHLabel";
            lblTenKHLabel.Size = new Size(100, 20);
            lblTenKHLabel.TabIndex = 2;
            lblTenKHLabel.Text = "Khách hàng:";
            // 
            // lblTenKH
            // 
            lblTenKH.Font = new Font("Arial", 10.5F);
            lblTenKH.Location = new Point(345, 25);
            lblTenKH.Name = "lblTenKH";
            lblTenKH.Size = new Size(250, 20);
            lblTenKH.TabIndex = 3;
            // 
            // lblDiaChiLabel
            // 
            lblDiaChiLabel.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblDiaChiLabel.Location = new Point(15, 50);
            lblDiaChiLabel.Name = "lblDiaChiLabel";
            lblDiaChiLabel.Size = new Size(80, 20);
            lblDiaChiLabel.TabIndex = 4;
            lblDiaChiLabel.Text = "Địa chỉ:";
            // 
            // lblDiaChi
            // 
            lblDiaChi.Font = new Font("Arial", 10.5F);
            lblDiaChi.Location = new Point(100, 50);
            lblDiaChi.Name = "lblDiaChi";
            lblDiaChi.Size = new Size(400, 20);
            lblDiaChi.TabIndex = 5;
            // 
            // lblNVTaiCongLabel
            // 
            lblNVTaiCongLabel.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblNVTaiCongLabel.Location = new Point(15, 75);
            lblNVTaiCongLabel.Name = "lblNVTaiCongLabel";
            lblNVTaiCongLabel.Size = new Size(90, 20);
            lblNVTaiCongLabel.TabIndex = 6;
            lblNVTaiCongLabel.Text = "NV Thi công:";
            // 
            // lblNVTaiCong
            // 
            lblNVTaiCong.Font = new Font("Arial", 10.5F);
            lblNVTaiCong.Location = new Point(110, 75);
            lblNVTaiCong.Name = "lblNVTaiCong";
            lblNVTaiCong.Size = new Size(200, 20);
            lblNVTaiCong.TabIndex = 7;
            // 
            // grpChiTiet
            // 
            grpChiTiet.Controls.Add(dgvChiTiet);
            grpChiTiet.Controls.Add(lblTongSoLuong);
            grpChiTiet.Controls.Add(lblSoLoaiVT);
            grpChiTiet.Font = new Font("Arial", 11F, FontStyle.Bold);
            grpChiTiet.Location = new Point(20, 180);
            grpChiTiet.Name = "grpChiTiet";
            grpChiTiet.Size = new Size(830, 354);
            grpChiTiet.TabIndex = 2;
            grpChiTiet.TabStop = false;
            grpChiTiet.Text = "Chi tiết vật tư hoàn ứng";
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
            dgvChiTiet.Size = new Size(800, 292);
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
            // lblTongSoLuong
            // 
            lblTongSoLuong.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblTongSoLuong.ForeColor = Color.Blue;
            lblTongSoLuong.Location = new Point(15, 25);
            lblTongSoLuong.Name = "lblTongSoLuong";
            lblTongSoLuong.Size = new Size(200, 20);
            lblTongSoLuong.TabIndex = 1;
            lblTongSoLuong.Text = "Tổng số lượng: 0";
            // 
            // lblSoLoaiVT
            // 
            lblSoLoaiVT.Font = new Font("Arial", 11F, FontStyle.Bold);
            lblSoLoaiVT.ForeColor = Color.Green;
            lblSoLoaiVT.Location = new Point(250, 25);
            lblSoLoaiVT.Name = "lblSoLoaiVT";
            lblSoLoaiVT.Size = new Size(200, 20);
            lblSoLoaiVT.TabIndex = 2;
            lblSoLoaiVT.Text = "Số loại vật tư: 0";
            // 
            // btnDong
            // 
            btnDong.BackColor = Color.FromArgb(108, 117, 125);
            btnDong.FlatStyle = FlatStyle.Flat;
            btnDong.Font = new Font("Arial", 11F, FontStyle.Bold);
            btnDong.ForeColor = Color.White;
            btnDong.Location = new Point(720, 550);
            btnDong.Name = "btnDong";
            btnDong.Size = new Size(80, 35);
            btnDong.TabIndex = 6;
            btnDong.Text = "Đóng";
            btnDong.UseVisualStyleBackColor = false;
            btnDong.Click += BtnDong_Click;
            // 
            // btnHoanUng
            // 
            btnHoanUng.BackColor = Color.FromArgb(220, 53, 69);
            btnHoanUng.FlatStyle = FlatStyle.Flat;
            btnHoanUng.Font = new Font("Arial", 11F, FontStyle.Bold);
            btnHoanUng.ForeColor = Color.White;
            btnHoanUng.Location = new Point(610, 550);
            btnHoanUng.Name = "btnHoanUng";
            btnHoanUng.Size = new Size(100, 35);
            btnHoanUng.TabIndex = 5;
            btnHoanUng.Text = "Hoàn ứng";
            btnHoanUng.UseVisualStyleBackColor = false;
            btnHoanUng.Click += BtnHoanUng_Click;
            // 
            // btnHoanUngAm
            // 
            btnHoanUngAm.BackColor = Color.FromArgb(220, 53, 69);
            btnHoanUngAm.FlatStyle = FlatStyle.Flat;
            btnHoanUngAm.Font = new Font("Arial", 11F, FontStyle.Bold);
            btnHoanUngAm.ForeColor = Color.White;
            btnHoanUngAm.Location = new Point(35, 550);
            btnHoanUngAm.Name = "btnHoanUngAm";
            btnHoanUngAm.Size = new Size(149, 35);
            btnHoanUngAm.TabIndex = 7;
            btnHoanUngAm.Text = "Hoàn ứng âm";
            btnHoanUngAm.UseVisualStyleBackColor = false;
            btnHoanUngAm.Click += btnHoanUngAm_Click;
            // 
            // ChiTietVatTuMC4Form
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(870, 600);
            Controls.Add(btnHoanUngAm);
            Controls.Add(lblTitle);
            Controls.Add(grpHoSoInfo);
            Controls.Add(grpChiTiet);
            Controls.Add(btnDong);
            Controls.Add(btnHoanUng);
            Font = new Font("Arial", 10.5F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChiTietVatTuMC4Form";
            StartPosition = FormStartPosition.CenterParent;
            Text = "s";
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
        private Button btnHoanUngAm;
    }
}

