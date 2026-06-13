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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
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
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitle.BackColor = Color.FromArgb(52, 152, 219);
            lblTitle.Font = new Font("Arial", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(1200, 50);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "HOÀN ỨNG SỬA CHỮA SỰ CỐ";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.AutoSize = true;
            lblConnectionStatus.Font = new Font("Arial", 10F);
            lblConnectionStatus.Location = new Point(20, 65);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(153, 16);
            lblConnectionStatus.TabIndex = 1;
            lblConnectionStatus.Text = "Đang kiểm tra kết nối...";
            // 
            // btnTaiDuLieuERP
            // 
            btnTaiDuLieuERP.BackColor = Color.FromArgb(52, 152, 219);
            btnTaiDuLieuERP.FlatStyle = FlatStyle.Flat;
            btnTaiDuLieuERP.Font = new Font("Arial", 10.5F, FontStyle.Bold);
            btnTaiDuLieuERP.ForeColor = Color.White;
            btnTaiDuLieuERP.Location = new Point(400, 60);
            btnTaiDuLieuERP.Name = "btnTaiDuLieuERP";
            btnTaiDuLieuERP.Size = new Size(150, 30);
            btnTaiDuLieuERP.TabIndex = 2;
            btnTaiDuLieuERP.Text = "📥 Tải dữ liệu ERP";
            btnTaiDuLieuERP.UseVisualStyleBackColor = false;
            btnTaiDuLieuERP.Click += BtnTaiDuLieuERP_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(46, 204, 113);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Arial", 10.5F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(560, 60);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "🔄 Làm mới";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // grpTimKiem
            // 
            grpTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpTimKiem.Controls.Add(lblTimKiem);
            grpTimKiem.Controls.Add(txtTimKiem);
            grpTimKiem.Controls.Add(btnTimKiem);
            grpTimKiem.Controls.Add(btnXoaTimKiem);
            grpTimKiem.Font = new Font("Arial", 11F, FontStyle.Bold);
            grpTimKiem.ForeColor = Color.FromArgb(52, 73, 94);
            grpTimKiem.Location = new Point(700, 50);
            grpTimKiem.Name = "grpTimKiem";
            grpTimKiem.Size = new Size(480, 50);
            grpTimKiem.TabIndex = 5;
            grpTimKiem.TabStop = false;
            grpTimKiem.Text = "🔍 Tìm kiếm";
            // 
            // lblTimKiem
            // 
            lblTimKiem.AutoSize = true;
            lblTimKiem.Font = new Font("Arial", 10.5F);
            lblTimKiem.Location = new Point(10, 23);
            lblTimKiem.Name = "lblTimKiem";
            lblTimKiem.Size = new Size(92, 16);
            lblTimKiem.TabIndex = 0;
            lblTimKiem.Text = "Mã đơn/Vị trí:";
            // 
            // txtTimKiem
            // 
            txtTimKiem.Font = new Font("Arial", 10.5F);
            txtTimKiem.Location = new Point(110, 20);
            txtTimKiem.Name = "txtTimKiem";
            txtTimKiem.Size = new Size(245, 24);
            txtTimKiem.TabIndex = 1;
            txtTimKiem.KeyPress += TxtTimKiem_KeyPress;
            // 
            // btnTimKiem
            // 
            btnTimKiem.BackColor = Color.FromArgb(46, 204, 113);
            btnTimKiem.FlatStyle = FlatStyle.Flat;
            btnTimKiem.Font = new Font("Arial", 10.5F, FontStyle.Bold);
            btnTimKiem.ForeColor = Color.White;
            btnTimKiem.Location = new Point(365, 18);
            btnTimKiem.Name = "btnTimKiem";
            btnTimKiem.Size = new Size(50, 27);
            btnTimKiem.TabIndex = 2;
            btnTimKiem.Text = "🔍";
            btnTimKiem.UseVisualStyleBackColor = false;
            btnTimKiem.Click += BtnTimKiem_Click;
            // 
            // btnXoaTimKiem
            // 
            btnXoaTimKiem.BackColor = Color.FromArgb(220, 53, 69);
            btnXoaTimKiem.FlatStyle = FlatStyle.Flat;
            btnXoaTimKiem.Font = new Font("Arial", 10.5F, FontStyle.Bold);
            btnXoaTimKiem.ForeColor = Color.White;
            btnXoaTimKiem.Location = new Point(425, 18);
            btnXoaTimKiem.Name = "btnXoaTimKiem";
            btnXoaTimKiem.Size = new Size(50, 27);
            btnXoaTimKiem.TabIndex = 3;
            btnXoaTimKiem.Text = "✖";
            btnXoaTimKiem.UseVisualStyleBackColor = false;
            btnXoaTimKiem.Click += BtnXoaTimKiem_Click;
            // 
            // grpDanhSach
            // 
            grpDanhSach.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpDanhSach.Controls.Add(lblThongKe);
            grpDanhSach.Controls.Add(dgvHoSoDC);
            grpDanhSach.Font = new Font("Arial", 11F, FontStyle.Bold);
            grpDanhSach.ForeColor = Color.FromArgb(52, 73, 94);
            grpDanhSach.Location = new Point(20, 110);
            grpDanhSach.Name = "grpDanhSach";
            grpDanhSach.Size = new Size(1160, 558);
            grpDanhSach.TabIndex = 6;
            grpDanhSach.TabStop = false;
            grpDanhSach.Text = "📋 Danh sách hồ sơ DC chờ hoàn ứng";
            // 
            // lblThongKe
            // 
            lblThongKe.AutoSize = true;
            lblThongKe.Font = new Font("Arial", 10.5F, FontStyle.Bold);
            lblThongKe.ForeColor = Color.FromArgb(52, 152, 219);
            lblThongKe.Location = new Point(15, 25);
            lblThongKe.Name = "lblThongKe";
            lblThongKe.Size = new Size(118, 16);
            lblThongKe.TabIndex = 1;
            lblThongKe.Text = "Chưa có dữ liệu";
            // 
            // dgvHoSoDC
            // 
            dgvHoSoDC.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvHoSoDC.BackgroundColor = Color.White;
            dgvHoSoDC.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewCellStyle1.Font = new Font("Arial", 11F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvHoSoDC.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvHoSoDC.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHoSoDC.EnableHeadersVisualStyles = false;
            dgvHoSoDC.GridColor = Color.FromArgb(189, 195, 199);
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
            BackColor = Color.FromArgb(236, 240, 241);
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
