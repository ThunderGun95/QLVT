namespace QLVT.GUI
{
    partial class BaoCaoXuatNhapTonUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            grpFilters = new GroupBox();
            lblFromDate = new Label();
            dtpFromDate = new DateTimePicker();
            lblToDate = new Label();
            dtpToDate = new DateTimePicker();
            lblWarehouse = new Label();
            grpActions = new GroupBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnCreateReport = new Button();
            btnExportCsv = new Button();
            btnClearFilter = new Button();
            grpResults = new GroupBox();
            dgvTransactionReport = new DataGridView();
            lblSummary = new Label();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            grpFilters.SuspendLayout();
            grpActions.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            grpResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTransactionReport).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // grpFilters
            // 
            grpFilters.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFilters.Controls.Add(lblFromDate);
            grpFilters.Controls.Add(dtpFromDate);
            grpFilters.Controls.Add(lblToDate);
            grpFilters.Controls.Add(dtpToDate);
            grpFilters.Controls.Add(lblWarehouse);
            grpFilters.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpFilters.Location = new Point(4, 3);
            grpFilters.Margin = new Padding(4, 3, 4, 3);
            grpFilters.Name = "grpFilters";
            grpFilters.Padding = new Padding(4, 3, 4, 3);
            grpFilters.Size = new Size(1393, 120);
            grpFilters.TabIndex = 0;
            grpFilters.TabStop = false;
            grpFilters.Text = "Bộ lọc báo cáo";
            // 
            // lblFromDate
            // 
            lblFromDate.AutoSize = true;
            lblFromDate.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFromDate.Location = new Point(20, 25);
            lblFromDate.Margin = new Padding(4, 0, 4, 0);
            lblFromDate.Name = "lblFromDate";
            lblFromDate.Size = new Size(46, 13);
            lblFromDate.TabIndex = 0;
            lblFromDate.Text = "Từ ngày:";
            // 
            // dtpFromDate
            // 
            dtpFromDate.CustomFormat = "dd/MM/yyyy";
            dtpFromDate.Format = DateTimePickerFormat.Custom;
            dtpFromDate.Location = new Point(20, 45);
            dtpFromDate.Margin = new Padding(4, 3, 4, 3);
            dtpFromDate.Name = "dtpFromDate";
            dtpFromDate.Size = new Size(150, 20);
            dtpFromDate.TabIndex = 1;
            // 
            // lblToDate
            // 
            lblToDate.AutoSize = true;
            lblToDate.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblToDate.Location = new Point(200, 25);
            lblToDate.Margin = new Padding(4, 0, 4, 0);
            lblToDate.Name = "lblToDate";
            lblToDate.Size = new Size(53, 13);
            lblToDate.TabIndex = 2;
            lblToDate.Text = "Đến ngày:";
            // 
            // dtpToDate
            // 
            dtpToDate.CustomFormat = "dd/MM/yyyy";
            dtpToDate.Format = DateTimePickerFormat.Custom;
            dtpToDate.Location = new Point(200, 45);
            dtpToDate.Margin = new Padding(4, 3, 4, 3);
            dtpToDate.Name = "dtpToDate";
            dtpToDate.Size = new Size(150, 20);
            dtpToDate.TabIndex = 3;



            // 
            // lblWarehouse
            // 
            lblWarehouse.AutoSize = true;
            lblWarehouse.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblWarehouse.Location = new Point(380, 25);
            lblWarehouse.Margin = new Padding(4, 0, 4, 0);
            lblWarehouse.Name = "lblWarehouse";
            lblWarehouse.Size = new Size(29, 13);
            lblWarehouse.TabIndex = 4;
            lblWarehouse.Text = "Kho:";
            // 
            // grpActions
            // 
            grpActions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpActions.Controls.Add(flowLayoutPanel1);
            grpActions.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpActions.Location = new Point(4, 130);
            grpActions.Margin = new Padding(4, 3, 4, 3);
            grpActions.Name = "grpActions";
            grpActions.Padding = new Padding(4, 3, 4, 3);
            grpActions.Size = new Size(1393, 69);
            grpActions.TabIndex = 1;
            grpActions.TabStop = false;
            grpActions.Text = "Thao tác";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnCreateReport);
            flowLayoutPanel1.Controls.Add(btnExportCsv);
            flowLayoutPanel1.Controls.Add(btnClearFilter);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(4, 17);
            flowLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1385, 49);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // btnCreateReport
            // 
            btnCreateReport.BackColor = Color.FromArgb(0, 123, 255);
            btnCreateReport.FlatStyle = FlatStyle.Flat;
            btnCreateReport.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCreateReport.ForeColor = Color.White;
            btnCreateReport.Location = new Point(4, 3);
            btnCreateReport.Margin = new Padding(4, 3, 4, 3);
            btnCreateReport.Name = "btnCreateReport";
            btnCreateReport.Size = new Size(140, 35);
            btnCreateReport.TabIndex = 0;
            btnCreateReport.Text = "Tạo báo cáo";
            btnCreateReport.UseVisualStyleBackColor = false;
            btnCreateReport.Click += btnCreateReport_Click;
            // 
            // btnExportCsv
            // 
            btnExportCsv.BackColor = Color.FromArgb(40, 167, 69);
            btnExportCsv.Enabled = false;
            btnExportCsv.FlatStyle = FlatStyle.Flat;
            btnExportCsv.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExportCsv.ForeColor = Color.White;
            btnExportCsv.Location = new Point(152, 3);
            btnExportCsv.Margin = new Padding(4, 3, 4, 3);
            btnExportCsv.Name = "btnExportCsv";
            btnExportCsv.Size = new Size(117, 35);
            btnExportCsv.TabIndex = 1;
            btnExportCsv.Text = "Xuất Excel";
            btnExportCsv.UseVisualStyleBackColor = false;
            btnExportCsv.Click += btnExportCsv_Click;
            // 
            // btnClearFilter
            // 
            btnClearFilter.BackColor = Color.FromArgb(108, 117, 125);
            btnClearFilter.FlatStyle = FlatStyle.Flat;
            btnClearFilter.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClearFilter.ForeColor = Color.White;
            btnClearFilter.Location = new Point(277, 3);
            btnClearFilter.Margin = new Padding(4, 3, 4, 3);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(117, 35);
            btnClearFilter.TabIndex = 2;
            btnClearFilter.Text = "Xóa bộ lọc";
            btnClearFilter.UseVisualStyleBackColor = false;
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // grpResults
            // 
            grpResults.Controls.Add(dgvTransactionReport);
            grpResults.Controls.Add(lblSummary);
            grpResults.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpResults.Location = new Point(12, 210);
            grpResults.Name = "grpResults";
            grpResults.Size = new Size(1160, 490);
            grpResults.TabIndex = 2;
            grpResults.TabStop = false;
            grpResults.Text = "Kết quả báo cáo (Double-click vào dòng để xem chi tiết)";
            // 
            // dgvTransactionReport
            // 
            dgvTransactionReport.AllowUserToAddRows = false;
            dgvTransactionReport.AllowUserToDeleteRows = false;
            dgvTransactionReport.BackgroundColor = Color.White;
            dgvTransactionReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTransactionReport.Location = new Point(15, 25);
            dgvTransactionReport.Name = "dgvTransactionReport";
            dgvTransactionReport.ReadOnly = true;
            dgvTransactionReport.RowHeadersWidth = 25;
            dgvTransactionReport.Size = new Size(1130, 380);
            dgvTransactionReport.TabIndex = 0;
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
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip1.Location = new Point(0, 693);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1400, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(113, 17);
            lblStatus.Text = "Trạng thái: Sẵn sàng";
            // 
            // BaoCaoXuatNhapTonUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            Controls.Add(statusStrip1);
            Controls.Add(grpResults);
            Controls.Add(grpActions);
            Controls.Add(grpFilters);
            Margin = new Padding(4, 3, 4, 3);
            Name = "BaoCaoXuatNhapTonUserControl";
            Size = new Size(1400, 715);
            grpFilters.ResumeLayout(false);
            grpFilters.PerformLayout();
            grpActions.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            grpResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTransactionReport).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpFilters;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label lblWarehouse;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCreateReport;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.DataGridView dgvTransactionReport;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}
