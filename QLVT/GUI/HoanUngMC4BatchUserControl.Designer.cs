namespace QLVT.GUI
{
    partial class HoanUngMC4BatchUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblConnectionStatus;
        private Button btnRefresh;
        private GroupBox grpChoHoanUng;
        private DataGridView dgvChoHoanUng;
        private Label lblTongSoChoHoanUng;
        private GroupBox grpDaChon;
        private DataGridView dgvDaChon;
        private Label lblTongSoDaChon;
        private Button btnChonTatCa;
        private Button btnBoChonTatCa;
        private Button btnBatDauHoanUng;
        private GroupBox grpTienDo;
        private ProgressBar progressBar;
        private Label lblTienDo;
        private TextBox txtKetQua;
        private Label lblHuongDan;

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
            lblConnectionStatus = new Label();
            btnRefresh = new Button();
            lblHuongDan = new Label();
            grpChoHoanUng = new GroupBox();
            lblTongSoChoHoanUng = new Label();
            dgvChoHoanUng = new DataGridView();
            grpDaChon = new GroupBox();
            lblTongSoDaChon = new Label();
            dgvDaChon = new DataGridView();
            btnChonTatCa = new Button();
            btnBoChonTatCa = new Button();
            btnBatDauHoanUng = new Button();
            grpTienDo = new GroupBox();
            lblTienDo = new Label();
            progressBar = new ProgressBar();
            txtKetQua = new TextBox();
            grpChoHoanUng.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChoHoanUng).BeginInit();
            grpDaChon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDaChon).BeginInit();
            grpTienDo.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.DarkRed;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(1400, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "HOÀN ỨNG HÀNG LOẠT - MẠNG CẤP 4";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblConnectionStatus.Location = new Point(20, 50);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(132, 15);
            lblConnectionStatus.TabIndex = 1;
            lblConnectionStatus.Text = "Đang kiểm tra kết nối...";
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(400, 45);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 30);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // lblHuongDan
            // 
            lblHuongDan.AutoSize = true;
            lblHuongDan.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Italic);
            lblHuongDan.ForeColor = Color.Blue;
            lblHuongDan.Location = new Point(600, 52);
            lblHuongDan.Name = "lblHuongDan";
            lblHuongDan.Size = new Size(500, 15);
            lblHuongDan.TabIndex = 3;
            lblHuongDan.Text = "💡 Hướng dẫn: Double-click để chọn/bỏ chọn đơn, hoặc dùng nút \"Chọn tất cả\"";
            // 
            // grpChoHoanUng
            // 
            grpChoHoanUng.Controls.Add(lblTongSoChoHoanUng);
            grpChoHoanUng.Controls.Add(dgvChoHoanUng);
            grpChoHoanUng.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpChoHoanUng.Location = new Point(20, 85);
            grpChoHoanUng.Name = "grpChoHoanUng";
            grpChoHoanUng.Size = new Size(680, 300);
            grpChoHoanUng.TabIndex = 4;
            grpChoHoanUng.TabStop = false;
            grpChoHoanUng.Text = "Danh sách đơn chờ hoàn ứng";
            // 
            // lblTongSoChoHoanUng
            // 
            lblTongSoChoHoanUng.AutoSize = true;
            lblTongSoChoHoanUng.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblTongSoChoHoanUng.ForeColor = Color.DarkBlue;
            lblTongSoChoHoanUng.Location = new Point(15, 20);
            lblTongSoChoHoanUng.Name = "lblTongSoChoHoanUng";
            lblTongSoChoHoanUng.Size = new Size(107, 15);
            lblTongSoChoHoanUng.TabIndex = 1;
            lblTongSoChoHoanUng.Text = "Chưa có dữ liệu";
            // 
            // dgvChoHoanUng
            // 
            dgvChoHoanUng.BackgroundColor = Color.White;
            dgvChoHoanUng.BorderStyle = BorderStyle.Fixed3D;
            dgvChoHoanUng.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvChoHoanUng.Location = new Point(15, 45);
            dgvChoHoanUng.Name = "dgvChoHoanUng";
            dgvChoHoanUng.RowHeadersWidth = 30;
            dgvChoHoanUng.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChoHoanUng.Size = new Size(650, 240);
            dgvChoHoanUng.TabIndex = 0;
            // 
            // grpDaChon
            // 
            grpDaChon.Controls.Add(lblTongSoDaChon);
            grpDaChon.Controls.Add(dgvDaChon);
            grpDaChon.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpDaChon.Location = new Point(720, 85);
            grpDaChon.Name = "grpDaChon";
            grpDaChon.Size = new Size(660, 300);
            grpDaChon.TabIndex = 5;
            grpDaChon.TabStop = false;
            grpDaChon.Text = "Danh sách đơn đã chọn để hoàn ứng";
            // 
            // lblTongSoDaChon
            // 
            lblTongSoDaChon.AutoSize = true;
            lblTongSoDaChon.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblTongSoDaChon.ForeColor = Color.DarkGreen;
            lblTongSoDaChon.Location = new Point(15, 20);
            lblTongSoDaChon.Name = "lblTongSoDaChon";
            lblTongSoDaChon.Size = new Size(107, 15);
            lblTongSoDaChon.TabIndex = 1;
            lblTongSoDaChon.Text = "Đã chọn: 0 đơn";
            // 
            // dgvDaChon
            // 
            dgvDaChon.BackgroundColor = Color.White;
            dgvDaChon.BorderStyle = BorderStyle.Fixed3D;
            dgvDaChon.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDaChon.Location = new Point(15, 45);
            dgvDaChon.Name = "dgvDaChon";
            dgvDaChon.RowHeadersWidth = 30;
            dgvDaChon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDaChon.Size = new Size(630, 240);
            dgvDaChon.TabIndex = 0;
            // 
            // btnChonTatCa
            // 
            btnChonTatCa.BackColor = Color.FromArgb(0, 123, 255);
            btnChonTatCa.FlatStyle = FlatStyle.Flat;
            btnChonTatCa.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnChonTatCa.ForeColor = Color.White;
            btnChonTatCa.Location = new Point(250, 400);
            btnChonTatCa.Name = "btnChonTatCa";
            btnChonTatCa.Size = new Size(150, 35);
            btnChonTatCa.TabIndex = 6;
            btnChonTatCa.Text = "➤ Chọn tất cả";
            btnChonTatCa.UseVisualStyleBackColor = false;
            btnChonTatCa.Click += BtnChonTatCa_Click;
            // 
            // btnBoChonTatCa
            // 
            btnBoChonTatCa.BackColor = Color.FromArgb(220, 53, 69);
            btnBoChonTatCa.FlatStyle = FlatStyle.Flat;
            btnBoChonTatCa.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnBoChonTatCa.ForeColor = Color.White;
            btnBoChonTatCa.Location = new Point(420, 400);
            btnBoChonTatCa.Name = "btnBoChonTatCa";
            btnBoChonTatCa.Size = new Size(150, 35);
            btnBoChonTatCa.TabIndex = 7;
            btnBoChonTatCa.Text = "✖ Bỏ chọn tất cả";
            btnBoChonTatCa.UseVisualStyleBackColor = false;
            btnBoChonTatCa.Click += BtnBoChonTatCa_Click;
            // 
            // btnBatDauHoanUng
            // 
            btnBatDauHoanUng.BackColor = Color.FromArgb(220, 165, 0);
            btnBatDauHoanUng.FlatStyle = FlatStyle.Flat;
            btnBatDauHoanUng.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnBatDauHoanUng.ForeColor = Color.White;
            btnBatDauHoanUng.Location = new Point(950, 395);
            btnBatDauHoanUng.Name = "btnBatDauHoanUng";
            btnBatDauHoanUng.Size = new Size(200, 45);
            btnBatDauHoanUng.TabIndex = 8;
            btnBatDauHoanUng.Text = "🚀 BẮT ĐẦU HOÀN ỨNG";
            btnBatDauHoanUng.UseVisualStyleBackColor = false;
            btnBatDauHoanUng.Click += BtnBatDauHoanUng_Click;
            // 
            // grpTienDo
            // 
            grpTienDo.Controls.Add(lblTienDo);
            grpTienDo.Controls.Add(progressBar);
            grpTienDo.Controls.Add(txtKetQua);
            grpTienDo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpTienDo.Location = new Point(20, 460);
            grpTienDo.Name = "grpTienDo";
            grpTienDo.Size = new Size(1360, 250);
            grpTienDo.TabIndex = 9;
            grpTienDo.TabStop = false;
            grpTienDo.Text = "Tiến độ xử lý và kết quả";
            // 
            // lblTienDo
            // 
            lblTienDo.AutoSize = true;
            lblTienDo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblTienDo.ForeColor = Color.DarkOrange;
            lblTienDo.Location = new Point(15, 25);
            lblTienDo.Name = "lblTienDo";
            lblTienDo.Size = new Size(150, 15);
            lblTienDo.TabIndex = 1;
            lblTienDo.Text = "Chưa bắt đầu xử lý";
            lblTienDo.Visible = false;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(15, 50);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1330, 25);
            progressBar.TabIndex = 0;
            progressBar.Visible = false;
            // 
            // txtKetQua
            // 
            txtKetQua.BackColor = Color.Black;
            txtKetQua.Font = new Font("Consolas", 9F);
            txtKetQua.ForeColor = Color.Lime;
            txtKetQua.Location = new Point(15, 85);
            txtKetQua.Multiline = true;
            txtKetQua.Name = "txtKetQua";
            txtKetQua.ReadOnly = true;
            txtKetQua.ScrollBars = ScrollBars.Vertical;
            txtKetQua.Size = new Size(1330, 150);
            txtKetQua.TabIndex = 2;
            txtKetQua.Visible = false;
            // 
            // HoanUngMC4BatchUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(lblTitle);
            Controls.Add(lblConnectionStatus);
            Controls.Add(btnRefresh);
            Controls.Add(lblHuongDan);
            Controls.Add(grpChoHoanUng);
            Controls.Add(grpDaChon);
            Controls.Add(btnChonTatCa);
            Controls.Add(btnBoChonTatCa);
            Controls.Add(btnBatDauHoanUng);
            Controls.Add(grpTienDo);
            Name = "HoanUngMC4BatchUserControl";
            Size = new Size(1400, 720);
            grpChoHoanUng.ResumeLayout(false);
            grpChoHoanUng.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvChoHoanUng).EndInit();
            grpDaChon.ResumeLayout(false);
            grpDaChon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDaChon).EndInit();
            grpTienDo.ResumeLayout(false);
            grpTienDo.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}