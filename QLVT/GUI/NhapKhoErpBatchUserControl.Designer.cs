namespace QLVT.GUI
{
    partial class NhapKhoErpBatchUserControl
    {
        private System.ComponentModel.IContainer components = null;

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
            this.grpInput = new System.Windows.Forms.GroupBox();
            this.lblNam = new System.Windows.Forms.Label();
            this.nudNam = new System.Windows.Forms.NumericUpDown();
            this.lblTuSo = new System.Windows.Forms.Label();
            this.nudTuSo = new System.Windows.Forms.NumericUpDown();
            this.lblDenSo = new System.Windows.Forms.Label();
            this.nudDenSo = new System.Windows.Forms.NumericUpDown();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.lblSummary = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExportLog = new System.Windows.Forms.Button();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.dgvBatchList = new System.Windows.Forms.DataGridView();
            this.grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTuSo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDenSo)).BeginInit();
            this.grpProgress.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatchList)).BeginInit();
            this.SuspendLayout();

            // grpInput
            this.grpInput.Controls.Add(this.btnGenerate);
            this.grpInput.Controls.Add(this.lblDenSo);
            this.grpInput.Controls.Add(this.nudDenSo);
            this.grpInput.Controls.Add(this.lblTuSo);
            this.grpInput.Controls.Add(this.nudTuSo);
            this.grpInput.Controls.Add(this.lblNam);
            this.grpInput.Controls.Add(this.nudNam);
            this.grpInput.Location = new System.Drawing.Point(12, 12);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new System.Drawing.Size(600, 100);
            this.grpInput.TabIndex = 0;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Thông tin nhập";

            // lblNam
            this.lblNam.AutoSize = true;
            this.lblNam.Location = new System.Drawing.Point(20, 30);
            this.lblNam.Name = "lblNam";
            this.lblNam.Size = new System.Drawing.Size(32, 13);
            this.lblNam.TabIndex = 0;
            this.lblNam.Text = "Năm:";

            // nudNam
            this.nudNam.Location = new System.Drawing.Point(70, 28);
            this.nudNam.Maximum = new decimal(new int[] { 2030, 0, 0, 0 });
            this.nudNam.Minimum = new decimal(new int[] { 2020, 0, 0, 0 });
            this.nudNam.Name = "nudNam";
            this.nudNam.Size = new System.Drawing.Size(80, 20);
            this.nudNam.TabIndex = 1;
            this.nudNam.Value = new decimal(new int[] { 2024, 0, 0, 0 });

            // lblTuSo
            this.lblTuSo.AutoSize = true;
            this.lblTuSo.Location = new System.Drawing.Point(180, 30);
            this.lblTuSo.Name = "lblTuSo";
            this.lblTuSo.Size = new System.Drawing.Size(38, 13);
            this.lblTuSo.TabIndex = 2;
            this.lblTuSo.Text = "Từ số:";

            // nudTuSo
            this.nudTuSo.Location = new System.Drawing.Point(230, 28);
            this.nudTuSo.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.nudTuSo.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudTuSo.Name = "nudTuSo";
            this.nudTuSo.Size = new System.Drawing.Size(80, 20);
            this.nudTuSo.TabIndex = 3;
            this.nudTuSo.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // lblDenSo
            this.lblDenSo.AutoSize = true;
            this.lblDenSo.Location = new System.Drawing.Point(340, 30);
            this.lblDenSo.Name = "lblDenSo";
            this.lblDenSo.Size = new System.Drawing.Size(44, 13);
            this.lblDenSo.TabIndex = 4;
            this.lblDenSo.Text = "Đến số:";

            // nudDenSo
            this.nudDenSo.Location = new System.Drawing.Point(390, 28);
            this.nudDenSo.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.nudDenSo.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudDenSo.Name = "nudDenSo";
            this.nudDenSo.Size = new System.Drawing.Size(80, 20);
            this.nudDenSo.TabIndex = 5;
            this.nudDenSo.Value = new decimal(new int[] { 10, 0, 0, 0 });

            // btnGenerate
            this.btnGenerate.Location = new System.Drawing.Point(490, 25);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(90, 25);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.Text = "Tạo danh sách";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);

            // grpProgress
            this.grpProgress.Controls.Add(this.lblStatus);
            this.grpProgress.Controls.Add(this.progressBar);
            this.grpProgress.Controls.Add(this.lblProgress);
            this.grpProgress.Controls.Add(this.lblSummary);
            this.grpProgress.Controls.Add(this.lblConnectionStatus);
            this.grpProgress.Location = new System.Drawing.Point(12, 118);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(600, 120);
            this.grpProgress.TabIndex = 1;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Trạng thái";

            // lblConnectionStatus
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new System.Drawing.Point(20, 25);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(86, 13);
            this.lblConnectionStatus.TabIndex = 0;
            this.lblConnectionStatus.Text = "Kiểm tra kết nối...";

            // lblSummary
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(20, 45);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(0, 13);
            this.lblSummary.TabIndex = 1;

            // lblProgress
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(20, 65);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 2;

            // progressBar
            this.progressBar.Location = new System.Drawing.Point(20, 85);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(450, 20);
            this.progressBar.TabIndex = 3;

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(480, 88);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(50, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Sẵn sàng";

            // grpActions
            this.grpActions.Controls.Add(this.btnExportLog);
            this.grpActions.Controls.Add(this.btnReset);
            this.grpActions.Controls.Add(this.btnStop);
            this.grpActions.Controls.Add(this.btnStart);
            this.grpActions.Location = new System.Drawing.Point(12, 244);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(600, 60);
            this.grpActions.TabIndex = 2;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Thao tác";

            // btnStart
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(20, 25);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 25);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Bắt đầu";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // btnStop
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(110, 25);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(80, 25);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Dừng";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            // btnReset
            this.btnReset.Location = new System.Drawing.Point(200, 25);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 25);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Làm mới";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);

            // btnExportLog
            this.btnExportLog.Location = new System.Drawing.Point(500, 25);
            this.btnExportLog.Name = "btnExportLog";
            this.btnExportLog.Size = new System.Drawing.Size(80, 25);
            this.btnExportLog.TabIndex = 3;
            this.btnExportLog.Text = "Xuất log";
            this.btnExportLog.UseVisualStyleBackColor = true;
            this.btnExportLog.Click += new System.EventHandler(this.btnExportLog_Click);

            // grpResults
            this.grpResults.Controls.Add(this.dgvBatchList);
            this.grpResults.Location = new System.Drawing.Point(12, 310);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(600, 300);
            this.grpResults.TabIndex = 3;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Kết quả xử lý";

            // dgvBatchList
            this.dgvBatchList.AllowUserToAddRows = false;
            this.dgvBatchList.AllowUserToDeleteRows = false;
            this.dgvBatchList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBatchList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBatchList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBatchList.Location = new System.Drawing.Point(3, 16);
            this.dgvBatchList.Name = "dgvBatchList";
            this.dgvBatchList.ReadOnly = true;
            this.dgvBatchList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBatchList.Size = new System.Drawing.Size(594, 281);
            this.dgvBatchList.TabIndex = 0;
            this.dgvBatchList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvBatchList_CellFormatting);

            // NhapKhoErpBatchUserControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpResults);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpProgress);
            this.Controls.Add(this.grpInput);
            this.Name = "NhapKhoErpBatchUserControl";
            this.Size = new System.Drawing.Size(625, 620);
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTuSo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDenSo)).EndInit();
            this.grpProgress.ResumeLayout(false);
            this.grpProgress.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatchList)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.Label lblNam;
        private System.Windows.Forms.NumericUpDown nudNam;
        private System.Windows.Forms.Label lblTuSo;
        private System.Windows.Forms.NumericUpDown nudTuSo;
        private System.Windows.Forms.Label lblDenSo;
        private System.Windows.Forms.NumericUpDown nudDenSo;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExportLog;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.DataGridView dgvBatchList;
    }
}