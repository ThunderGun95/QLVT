namespace QLVT.GUI
{
    partial class BaoCaoTonKhoUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpFilter;

        private Label lblWarehouse;
        private TextBox txtWarehouse;
        private Label lblSearch;
        private TextBox txtSearch;
        private CheckBox chkChiHienThiCoTon;
        private Button btnCreateReport;
        private Button btnExportExcel;
        private Button btnResetFilter;
        private GroupBox grpReport;
        private DataGridView dgvReport;
        private Label lblSummary;
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
            grpFilter = new GroupBox();
            lblWarehouse = new Label();
            txtWarehouse = new TextBox();
            lblSearch = new Label();
            txtSearch = new TextBox();
            chkChiHienThiCoTon = new CheckBox();
            btnCreateReport = new Button();
            btnExportExcel = new Button();
            btnResetFilter = new Button();
            grpReport = new GroupBox();
            dgvReport = new DataGridView();
            lblSummary = new Label();
            lblStatus = new Label();
            grpFilter.SuspendLayout();
            grpReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
            SuspendLayout();
            
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.DarkBlue;
            lblTitle.Location = new Point(12, 9);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(300, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📊 BÁO CÁO TỒN KHO";
            
            // 
            // grpFilter
            // 
            grpFilter.Controls.Add(lblWarehouse);
            grpFilter.Controls.Add(txtWarehouse);
            grpFilter.Controls.Add(lblSearch);
            grpFilter.Controls.Add(txtSearch);
            grpFilter.Controls.Add(chkChiHienThiCoTon);
            grpFilter.Controls.Add(btnCreateReport);
            grpFilter.Controls.Add(btnExportExcel);
            grpFilter.Controls.Add(btnResetFilter);
            grpFilter.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpFilter.Location = new Point(12, 45);
            grpFilter.Name = "grpFilter";
            grpFilter.Size = new Size(1160, 80);
            grpFilter.TabIndex = 1;
            grpFilter.TabStop = false;
            grpFilter.Text = "Điều kiện lọc";
            
            // 
            // lblWarehouse
            // 
            lblWarehouse.Font = new Font("Segoe UI", 9F);
            lblWarehouse.Location = new Point(15, 25);
            lblWarehouse.Name = "lblWarehouse";
            lblWarehouse.Size = new Size(50, 23);
            lblWarehouse.TabIndex = 0;
            lblWarehouse.Text = "Kho:";
            lblWarehouse.TextAlign = ContentAlignment.MiddleLeft;
            
            // 
            // txtWarehouse
            // 
            txtWarehouse.Font = new Font("Segoe UI", 9F);
            txtWarehouse.Location = new Point(70, 25);
            txtWarehouse.Name = "txtWarehouse";
            txtWarehouse.Size = new Size(180, 23);
            txtWarehouse.TabIndex = 1;
            txtWarehouse.PlaceholderText = "Nhập tên kho hoặc 'Tất cả'...";
            txtWarehouse.KeyPress += txtWarehouse_KeyPress;
            
            // 
            // lblSearch
            // 
            lblSearch.Font = new Font("Segoe UI", 9F);
            lblSearch.Location = new Point(265, 25);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(60, 23);
            lblSearch.TabIndex = 2;
            lblSearch.Text = "Tìm VT:";
            lblSearch.TextAlign = ContentAlignment.MiddleLeft;
            
            // 
            // txtSearch
            // 
            txtSearch.Font = new Font("Segoe UI", 9F);
            txtSearch.Location = new Point(330, 25);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(220, 23);
            txtSearch.TabIndex = 3;
            txtSearch.PlaceholderText = "Nhập mã hoặc tên vật tư...";
            txtSearch.KeyPress += txtSearch_KeyPress;
            
            // 
            // chkChiHienThiCoTon
            // 
            chkChiHienThiCoTon.Font = new Font("Segoe UI", 9F);
            chkChiHienThiCoTon.Location = new Point(565, 25);
            chkChiHienThiCoTon.Name = "chkChiHienThiCoTon";
            chkChiHienThiCoTon.Size = new Size(150, 23);
            chkChiHienThiCoTon.TabIndex = 4;
            chkChiHienThiCoTon.Text = "Chỉ hiển thị có tồn";
            chkChiHienThiCoTon.UseVisualStyleBackColor = true;
            
            // 
            // btnCreateReport
            // 
            btnCreateReport.BackColor = Color.FromArgb(0, 122, 204);
            btnCreateReport.FlatStyle = FlatStyle.Flat;
            btnCreateReport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCreateReport.ForeColor = Color.White;
            btnCreateReport.Location = new Point(730, 20);
            btnCreateReport.Name = "btnCreateReport";
            btnCreateReport.Size = new Size(120, 35);
            btnCreateReport.TabIndex = 5;
            btnCreateReport.Text = "📊 Tạo báo cáo";
            btnCreateReport.UseVisualStyleBackColor = false;
            btnCreateReport.Click += btnCreateReport_Click;
            
            // 
            // btnExportExcel
            // 
            btnExportExcel.BackColor = Color.FromArgb(34, 139, 34);
            btnExportExcel.Enabled = false;
            btnExportExcel.FlatStyle = FlatStyle.Flat;
            btnExportExcel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExportExcel.ForeColor = Color.White;
            btnExportExcel.Location = new Point(860, 20);
            btnExportExcel.Name = "btnExportExcel";
            btnExportExcel.Size = new Size(120, 35);
            btnExportExcel.TabIndex = 6;
            btnExportExcel.Text = "📄 Xuất Excel";
            btnExportExcel.UseVisualStyleBackColor = false;
            btnExportExcel.Click += btnExportExcel_Click;
            
            // 
            // btnResetFilter
            // 
            btnResetFilter.BackColor = Color.FromArgb(108, 117, 125);
            btnResetFilter.FlatStyle = FlatStyle.Flat;
            btnResetFilter.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnResetFilter.ForeColor = Color.White;
            btnResetFilter.Location = new Point(990, 20);
            btnResetFilter.Name = "btnResetFilter";
            btnResetFilter.Size = new Size(120, 35);
            btnResetFilter.TabIndex = 7;
            btnResetFilter.Text = "🔄 Reset";
            btnResetFilter.UseVisualStyleBackColor = false;
            btnResetFilter.Click += btnResetFilter_Click;
            
            // 
            // grpReport
            // 
            grpReport.Controls.Add(dgvReport);
            grpReport.Controls.Add(lblSummary);
            grpReport.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpReport.Location = new Point(12, 135);
            grpReport.Name = "grpReport";
            grpReport.Size = new Size(1160, 490);
            grpReport.TabIndex = 2;
            grpReport.TabStop = false;
            grpReport.Text = "Kết quả báo cáo";
            
            // 
            // dgvReport
            // 
            dgvReport.AllowUserToAddRows = false;
            dgvReport.AllowUserToDeleteRows = false;
            dgvReport.BackgroundColor = Color.White;
            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReport.Location = new Point(15, 25);
            dgvReport.Name = "dgvReport";
            dgvReport.ReadOnly = true;
            dgvReport.Size = new Size(1130, 380);
            dgvReport.TabIndex = 0;
            dgvReport.CellFormatting += dgvReport_CellFormatting;
            
            // 
            // lblSummary
            // 
            lblSummary.BackColor = Color.LightBlue;
            lblSummary.BorderStyle = BorderStyle.FixedSingle;
            lblSummary.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummary.Location = new Point(15, 410);
            lblSummary.Name = "lblSummary";
            lblSummary.Size = new Size(1130, 25);
            lblSummary.TabIndex = 1;
            lblSummary.TextAlign = ContentAlignment.MiddleLeft;
            
            // 
            // lblStatus
            // 
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Font = new Font("Segoe UI", 9F);
            lblStatus.Location = new Point(12, 635);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(1160, 25);
            lblStatus.TabIndex = 3;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            
            // 
            // BaoCaoTonKhoUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTitle);
            Controls.Add(grpFilter);
            Controls.Add(grpReport);
            Controls.Add(lblStatus);
            Name = "BaoCaoTonKhoUserControl";
            Size = new Size(1200, 680);
            grpFilter.ResumeLayout(false);
            grpFilter.PerformLayout();
            grpReport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvReport).EndInit();
            ResumeLayout(false);
        }
    }
}
