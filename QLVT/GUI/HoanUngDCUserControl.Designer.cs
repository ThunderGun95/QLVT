namespace QLVT.GUI
{
    partial class HoanUngDCUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblConnectionStatus;
        private Button btnTaiDuLieuERP;
        private Button btnRefresh;
        private GroupBox grpDanhSach;
        private DataGridView dgvHoSoDC;
        private Label lblThongKe;
        private GroupBox grpTimKiem;
        private TextBox txtTimKiem;
        private Button btnTimKiem;
        private Button btnXoaTimKiem;
        private Label lblTimKiem;

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
            btnTaiDuLieuERP = new Button();
            btnRefresh = new Button();
            grpTimKiem = new GroupBox();
            lblTimKiem = new Label();
            txtTimKiem = new TextBox();
            btnTimKiem = new Button();
            btnXoaTimKiem = new Button();
            grpDanhSach = new GroupBox();
            lblThongKe = new Label();
            dgvHoSoDC = new DataGridView();
            grpTimKiem.SuspendLayout();
            grpDanhSach.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHoSoDC).BeginInit();
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
            lblTitle.Text = "HOÀN ỨNG SỬA CHỮA SỰ CỐ";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblConnectionStatus.Location = new Point(20, 70);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(132, 15);
            lblConnectionStatus.TabIndex = 1;
            lblConnectionStatus.Text = "Đang kiểm tra kết nối...";
            // 
            // btnTaiDuLieuERP
            // 
            btnTaiDuLieuERP.BackColor = Color.FromArgb(0, 123, 255);
            btnTaiDuLieuERP.FlatStyle = FlatStyle.Flat;
            btnTaiDuLieuERP.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnTaiDuLieuERP.ForeColor = Color.White;
            btnTaiDuLieuERP.Location = new Point(400, 64);
            btnTaiDuLieuERP.Name = "btnTaiDuLieuERP";
            btnTaiDuLieuERP.Size = new Size(150, 27);
            btnTaiDuLieuERP.TabIndex = 2;
            btnTaiDuLieuERP.Text = "� Tải dữ liệu ERP";
            btnTaiDuLieuERP.UseVisualStyleBackColor = false;
            btnTaiDuLieuERP.Click += BtnTaiDuLieuERP_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Microsoft Sans Serif", 9F);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(560, 64);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 27);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // grpTimKiem
            // 
            grpTimKiem.Controls.Add(lblTimKiem);
            grpTimKiem.Controls.Add(txtTimKiem);
            grpTimKiem.Controls.Add(btnTimKiem);
            grpTimKiem.Controls.Add(btnXoaTimKiem);
            grpTimKiem.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpTimKiem.Location = new Point(700, 55);
            grpTimKiem.Name = "grpTimKiem";
            grpTimKiem.Size = new Size(480, 45);
            grpTimKiem.TabIndex = 5;
            grpTimKiem.TabStop = false;
            grpTimKiem.Text = "Tìm kiếm";
            // 
            // lblTimKiem
            // 
            lblTimKiem.AutoSize = true;
            lblTimKiem.Font = new Font("Microsoft Sans Serif", 8F);
            lblTimKiem.Location = new Point(10, 20);
            lblTimKiem.Name = "lblTimKiem";
            lblTimKiem.Size = new Size(74, 13);
            lblTimKiem.TabIndex = 0;
            lblTimKiem.Text = "Mã đơn/Vị trí:";
            // 
            // txtTimKiem
            // 
            txtTimKiem.Font = new Font("Microsoft Sans Serif", 8F);
            txtTimKiem.Location = new Point(105, 17);
            txtTimKiem.Name = "txtTimKiem";
            txtTimKiem.Size = new Size(250, 20);
            txtTimKiem.TabIndex = 1;
            txtTimKiem.KeyPress += TxtTimKiem_KeyPress;
            // 
            // btnTimKiem
            // 
            btnTimKiem.BackColor = Color.FromArgb(40, 167, 69);
            btnTimKiem.FlatStyle = FlatStyle.Flat;
            btnTimKiem.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold);
            btnTimKiem.ForeColor = Color.White;
            btnTimKiem.Location = new Point(365, 15);
            btnTimKiem.Name = "btnTimKiem";
            btnTimKiem.Size = new Size(50, 25);
            btnTimKiem.TabIndex = 2;
            btnTimKiem.Text = "�";
            btnTimKiem.UseVisualStyleBackColor = false;
            btnTimKiem.Click += BtnTimKiem_Click;
            // 
            // btnXoaTimKiem
            // 
            btnXoaTimKiem.BackColor = Color.FromArgb(220, 53, 69);
            btnXoaTimKiem.FlatStyle = FlatStyle.Flat;
            btnXoaTimKiem.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold);
            btnXoaTimKiem.ForeColor = Color.White;
            btnXoaTimKiem.Location = new Point(425, 15);
            btnXoaTimKiem.Name = "btnXoaTimKiem";
            btnXoaTimKiem.Size = new Size(50, 25);
            btnXoaTimKiem.TabIndex = 3;
            btnXoaTimKiem.Text = "✖";
            btnXoaTimKiem.UseVisualStyleBackColor = false;
            btnXoaTimKiem.Click += BtnXoaTimKiem_Click;
            // 
            // grpDanhSach
            // 
            grpDanhSach.Controls.Add(lblThongKe);
            grpDanhSach.Controls.Add(dgvHoSoDC);
            grpDanhSach.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpDanhSach.Location = new Point(20, 110);
            grpDanhSach.Name = "grpDanhSach";
            grpDanhSach.Size = new Size(1160, 558);
            grpDanhSach.TabIndex = 6;
            grpDanhSach.TabStop = false;
            grpDanhSach.Text = "Danh sách hồ sơ DC chờ hoàn ứng";
            // 
            // lblThongKe
            // 
            lblThongKe.AutoSize = true;
            lblThongKe.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblThongKe.Location = new Point(15, 25);
            lblThongKe.Name = "lblThongKe";
            lblThongKe.Size = new Size(107, 15);
            lblThongKe.TabIndex = 1;
            lblThongKe.Text = "Chưa có dữ liệu";
            // 
            // dgvHoSoDC
            // 
            dgvHoSoDC.BackgroundColor = Color.White;
            dgvHoSoDC.BorderStyle = BorderStyle.Fixed3D;
            dgvHoSoDC.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHoSoDC.Location = new Point(15, 50);
            dgvHoSoDC.Name = "dgvHoSoDC";
            dgvHoSoDC.RowHeadersWidth = 30;
            dgvHoSoDC.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHoSoDC.Size = new Size(1130, 496);
            dgvHoSoDC.TabIndex = 0;
            // 
            // HoanUngDCUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(lblTitle);
            Controls.Add(lblConnectionStatus);
            Controls.Add(btnTaiDuLieuERP);
            Controls.Add(btnRefresh);
            Controls.Add(grpTimKiem);
            Controls.Add(grpDanhSach);
            Name = "HoanUngDCUserControl";
            Size = new Size(1200, 682);
            grpTimKiem.ResumeLayout(false);
            grpTimKiem.PerformLayout();
            grpDanhSach.ResumeLayout(false);
            grpDanhSach.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHoSoDC).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}