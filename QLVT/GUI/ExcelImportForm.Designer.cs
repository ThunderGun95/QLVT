namespace QLVT.GUI
{
    partial class ExcelImportForm
    {
        private System.ComponentModel.IContainer components = null;
        
        private GroupBox grpFileSelection;
        private Label lblFileName;
        private TextBox txtFilePath;
        private Button btnBrowse;
        private Button btnSample;
        private Label lblFileInfo;
        
        private GroupBox grpWarehouse;
        private Label lblWarehouse;
        private ComboBox cboWarehouse;
        
        private GroupBox grpPreview;
        private DataGridView dgvPreview;
        private Label lblPreviewInfo;
        
        private GroupBox grpActions;
        private Button btnMapping;
        private Button btnImport;
        private Button btnCancel;
        private Label lblStatus;
        
        private ProgressBar progressBar;

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
            grpFileSelection = new GroupBox();
            lblFileName = new Label();
            txtFilePath = new TextBox();
            btnBrowse = new Button();
            btnSample = new Button();
            lblFileInfo = new Label();
            
            grpWarehouse = new GroupBox();
            lblWarehouse = new Label();
            cboWarehouse = new ComboBox();
            
            grpPreview = new GroupBox();
            dgvPreview = new DataGridView();
            lblPreviewInfo = new Label();
            
            grpActions = new GroupBox();
            btnMapping = new Button();
            btnImport = new Button();
            btnCancel = new Button();
            lblStatus = new Label();
            
            progressBar = new ProgressBar();

            grpFileSelection.SuspendLayout();
            grpWarehouse.SuspendLayout();
            grpPreview.SuspendLayout();
            grpActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).BeginInit();
            SuspendLayout();

            // 
            // grpFileSelection
            // 
            grpFileSelection.Controls.Add(lblFileName);
            grpFileSelection.Controls.Add(txtFilePath);
            grpFileSelection.Controls.Add(btnBrowse);
            grpFileSelection.Controls.Add(btnSample);
            grpFileSelection.Controls.Add(lblFileInfo);
            grpFileSelection.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpFileSelection.Location = new Point(20, 20);
            grpFileSelection.Name = "grpFileSelection";
            grpFileSelection.Size = new Size(800, 100);
            grpFileSelection.TabIndex = 0;
            grpFileSelection.TabStop = false;
            grpFileSelection.Text = "Chọn file Excel";

            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Font = new Font("Microsoft Sans Serif", 9F);
            lblFileName.Location = new Point(15, 25);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(57, 15);
            lblFileName.TabIndex = 0;
            lblFileName.Text = "File Excel:";

            // 
            // txtFilePath
            // 
            txtFilePath.Font = new Font("Microsoft Sans Serif", 9F);
            txtFilePath.Location = new Point(80, 22);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(450, 21);
            txtFilePath.TabIndex = 1;

            // 
            // btnBrowse
            // 
            btnBrowse.Font = new Font("Microsoft Sans Serif", 9F);
            btnBrowse.Location = new Point(540, 20);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(80, 25);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "📁 Chọn file";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;

            // 
            // btnSample
            // 
            btnSample.Font = new Font("Microsoft Sans Serif", 9F);
            btnSample.Location = new Point(630, 20);
            btnSample.Name = "btnSample";
            btnSample.Size = new Size(80, 25);
            btnSample.TabIndex = 3;
            btnSample.Text = "📄 File mẫu";
            btnSample.UseVisualStyleBackColor = true;
            btnSample.Click += btnSample_Click;

            // 
            // lblFileInfo
            // 
            lblFileInfo.AutoSize = true;
            lblFileInfo.Font = new Font("Microsoft Sans Serif", 9F);
            lblFileInfo.Location = new Point(15, 55);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(350, 15);
            lblFileInfo.TabIndex = 4;
            lblFileInfo.Text = "Chọn file Excel với 2 cột: Mã vật tư (A) và Số lượng (B)";

            // 
            // grpWarehouse
            // 
            grpWarehouse.Controls.Add(lblWarehouse);
            grpWarehouse.Controls.Add(cboWarehouse);
            grpWarehouse.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpWarehouse.Location = new Point(20, 130);
            grpWarehouse.Name = "grpWarehouse";
            grpWarehouse.Size = new Size(400, 60);
            grpWarehouse.TabIndex = 1;
            grpWarehouse.TabStop = false;
            grpWarehouse.Text = "Chọn kho nhập";

            // 
            // lblWarehouse
            // 
            lblWarehouse.AutoSize = true;
            lblWarehouse.Font = new Font("Microsoft Sans Serif", 9F);
            lblWarehouse.Location = new Point(15, 25);
            lblWarehouse.Name = "lblWarehouse";
            lblWarehouse.Size = new Size(32, 15);
            lblWarehouse.TabIndex = 0;
            lblWarehouse.Text = "Kho:";

            // 
            // cboWarehouse
            // 
            cboWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
            cboWarehouse.Font = new Font("Microsoft Sans Serif", 9F);
            cboWarehouse.FormattingEnabled = true;
            cboWarehouse.Location = new Point(60, 22);
            cboWarehouse.Name = "cboWarehouse";
            cboWarehouse.Size = new Size(320, 23);
            cboWarehouse.TabIndex = 1;

            // 
            // grpPreview
            // 
            grpPreview.Controls.Add(dgvPreview);
            grpPreview.Controls.Add(lblPreviewInfo);
            grpPreview.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpPreview.Location = new Point(20, 200);
            grpPreview.Name = "grpPreview";
            grpPreview.Size = new Size(800, 350);
            grpPreview.TabIndex = 2;
            grpPreview.TabStop = false;
            grpPreview.Text = "Xem trước dữ liệu";

            // 
            // dgvPreview
            // 
            dgvPreview.BackgroundColor = Color.White;
            dgvPreview.BorderStyle = BorderStyle.Fixed3D;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPreview.Location = new Point(15, 50);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.ReadOnly = true;
            dgvPreview.RowHeadersWidth = 30;
            dgvPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPreview.Size = new Size(770, 280);
            dgvPreview.TabIndex = 1;

            // 
            // lblPreviewInfo
            // 
            lblPreviewInfo.AutoSize = true;
            lblPreviewInfo.Font = new Font("Microsoft Sans Serif", 9F);
            lblPreviewInfo.Location = new Point(15, 25);
            lblPreviewInfo.Name = "lblPreviewInfo";
            lblPreviewInfo.Size = new Size(150, 15);
            lblPreviewInfo.TabIndex = 0;
            lblPreviewInfo.Text = "Chưa có dữ liệu để hiển thị";

            // 
            // grpActions
            // 
            grpActions.Controls.Add(btnMapping);
            grpActions.Controls.Add(btnImport);
            grpActions.Controls.Add(btnCancel);
            grpActions.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            grpActions.Location = new Point(440, 130);
            grpActions.Name = "grpActions";
            grpActions.Size = new Size(380, 60);
            grpActions.TabIndex = 3;
            grpActions.TabStop = false;
            grpActions.Text = "Thao tác";

            // 
            // btnMapping
            // 
            btnMapping.BackColor = Color.FromArgb(255, 193, 7);
            btnMapping.Enabled = false;
            btnMapping.FlatStyle = FlatStyle.Flat;
            btnMapping.Font = new Font("Microsoft Sans Serif", 9F);
            btnMapping.ForeColor = Color.Black;
            btnMapping.Location = new Point(15, 22);
            btnMapping.Name = "btnMapping";
            btnMapping.Size = new Size(100, 30);
            btnMapping.TabIndex = 0;
            btnMapping.Text = "🔗 Mapping";
            btnMapping.UseVisualStyleBackColor = false;
            btnMapping.Click += btnMapping_Click;

            // 
            // btnImport
            // 
            btnImport.BackColor = Color.FromArgb(40, 167, 69);
            btnImport.Enabled = false;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnImport.ForeColor = Color.White;
            btnImport.Location = new Point(125, 22);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(120, 30);
            btnImport.TabIndex = 1;
            btnImport.Text = "✅ Nhập kho";
            btnImport.UseVisualStyleBackColor = false;
            btnImport.Click += btnImport_Click;

            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Microsoft Sans Serif", 9F);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(255, 22);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 30);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "❌ Đóng";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;

            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft Sans Serif", 9F);
            lblStatus.Location = new Point(20, 560);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(68, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Sẵn sàng...";

            // 
            // progressBar
            // 
            progressBar.Location = new Point(20, 580);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(800, 20);
            progressBar.TabIndex = 5;
            progressBar.Visible = false;

            // 
            // ExcelImportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(850, 620);
            Controls.Add(grpFileSelection);
            Controls.Add(grpWarehouse);
            Controls.Add(grpPreview);
            Controls.Add(grpActions);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExcelImportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Nhập tồn kho từ Excel";
            
            grpFileSelection.ResumeLayout(false);
            grpFileSelection.PerformLayout();
            grpWarehouse.ResumeLayout(false);
            grpWarehouse.PerformLayout();
            grpPreview.ResumeLayout(false);
            grpPreview.PerformLayout();
            grpActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPreview).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
