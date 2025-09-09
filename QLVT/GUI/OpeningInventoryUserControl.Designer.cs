namespace QLVT.GUI
{
    partial class OpeningInventoryUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private GroupBox grpExcelImport;
        private Label lblExcelFile;
        private TextBox txtExcelFilePath;
        private Button btnBrowseExcel;
        private Button btnLoadExcel;
        private GroupBox grpData;
        private DataGridView dgvInput;
        private GroupBox grpActions;
        private Label lblWarehouse;
        private ComboBox cboWarehouse;
        private Button btnSave;
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
            this.lblTitle = new Label();
            this.grpExcelImport = new GroupBox();
            this.lblExcelFile = new Label();
            this.txtExcelFilePath = new TextBox();
            this.btnBrowseExcel = new Button();
            this.btnLoadExcel = new Button();
            this.grpData = new GroupBox();
            this.dgvInput = new DataGridView();
            this.grpActions = new GroupBox();
            this.lblWarehouse = new Label();
            this.cboWarehouse = new ComboBox();
            this.btnSave = new Button();
            this.lblStatus = new Label();
            
            this.grpExcelImport.SuspendLayout();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInput)).BeginInit();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.BackColor = Color.Navy;
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(1200, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "NHẬP TỒN KHO ĐẦU KỲ TỪ EXCEL";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // grpExcelImport
            this.grpExcelImport.Controls.Add(this.lblExcelFile);
            this.grpExcelImport.Controls.Add(this.txtExcelFilePath);
            this.grpExcelImport.Controls.Add(this.btnBrowseExcel);
            this.grpExcelImport.Controls.Add(this.btnLoadExcel);
            this.grpExcelImport.Dock = DockStyle.Top;
            this.grpExcelImport.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpExcelImport.ForeColor = Color.Blue;
            this.grpExcelImport.Location = new Point(0, 40);
            this.grpExcelImport.Name = "grpExcelImport";
            this.grpExcelImport.Size = new Size(1200, 80);
            this.grpExcelImport.TabIndex = 1;
            this.grpExcelImport.TabStop = false;
            this.grpExcelImport.Text = "📊 1. Chọn file Excel (2 cột: Mã vật tư + Số lượng)";

            // lblExcelFile
            this.lblExcelFile.AutoSize = true;
            this.lblExcelFile.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblExcelFile.ForeColor = Color.Black;
            this.lblExcelFile.Location = new Point(15, 35);
            this.lblExcelFile.Name = "lblExcelFile";
            this.lblExcelFile.Size = new Size(30, 15);
            this.lblExcelFile.TabIndex = 0;
            this.lblExcelFile.Text = "File:";

            // txtExcelFilePath
            this.txtExcelFilePath.Font = new Font("Microsoft Sans Serif", 9F);
            this.txtExcelFilePath.Location = new Point(55, 32);
            this.txtExcelFilePath.Name = "txtExcelFilePath";
            this.txtExcelFilePath.ReadOnly = true;
            this.txtExcelFilePath.Size = new Size(500, 21);
            this.txtExcelFilePath.TabIndex = 1;

            // btnBrowseExcel
            this.btnBrowseExcel.BackColor = Color.FromArgb(23, 162, 184);
            this.btnBrowseExcel.FlatStyle = FlatStyle.Flat;
            this.btnBrowseExcel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.btnBrowseExcel.ForeColor = Color.White;
            this.btnBrowseExcel.Location = new Point(570, 30);
            this.btnBrowseExcel.Name = "btnBrowseExcel";
            this.btnBrowseExcel.Size = new Size(120, 25);
            this.btnBrowseExcel.TabIndex = 2;
            this.btnBrowseExcel.Text = "� Chọn file";
            this.btnBrowseExcel.UseVisualStyleBackColor = false;
            this.btnBrowseExcel.Click += new EventHandler(this.btnBrowseExcel_Click);

            // btnLoadExcel
            this.btnLoadExcel.BackColor = Color.FromArgb(255, 193, 7);
            this.btnLoadExcel.Enabled = false;
            this.btnLoadExcel.FlatStyle = FlatStyle.Flat;
            this.btnLoadExcel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.btnLoadExcel.ForeColor = Color.Black;
            this.btnLoadExcel.Location = new Point(700, 30);
            this.btnLoadExcel.Name = "btnLoadExcel";
            this.btnLoadExcel.Size = new Size(140, 25);
            this.btnLoadExcel.TabIndex = 3;
            this.btnLoadExcel.Text = "⬇️ 2. Tải vào grid";
            this.btnLoadExcel.UseVisualStyleBackColor = false;
            this.btnLoadExcel.Click += new EventHandler(this.btnLoadExcel_Click);

            // grpData
            this.grpData.Controls.Add(this.dgvInput);
            this.grpData.Dock = DockStyle.Fill;
            this.grpData.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpData.Location = new Point(0, 120);
            this.grpData.Name = "grpData";
            this.grpData.Size = new Size(1200, 450);
            this.grpData.TabIndex = 2;
            this.grpData.TabStop = false;
            this.grpData.Text = "📋 3. Dữ liệu tồn kho đầu kỳ";

            // dgvInput
            this.dgvInput.AllowUserToOrderColumns = true;
            this.dgvInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvInput.BackgroundColor = Color.White;
            this.dgvInput.BorderStyle = BorderStyle.Fixed3D;
            this.dgvInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInput.Location = new Point(15, 25);
            this.dgvInput.Name = "dgvInput";
            this.dgvInput.RowHeadersWidth = 30;
            this.dgvInput.Size = new Size(1170, 350);
            this.dgvInput.TabIndex = 0;

            // grpActions
            this.grpActions.Controls.Add(this.lblWarehouse);
            this.grpActions.Controls.Add(this.cboWarehouse);
            this.grpActions.Controls.Add(this.btnSave);
            this.grpActions.Dock = DockStyle.Bottom;
            this.grpActions.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpActions.ForeColor = Color.Green;
            this.grpActions.Location = new Point(0, 570);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(1200, 100);
            this.grpActions.TabIndex = 3;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "⚡ 4. Chọn kho và xác nhận";

            // lblWarehouse
            this.lblWarehouse.AutoSize = true;
            this.lblWarehouse.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.lblWarehouse.ForeColor = Color.Black;
            this.lblWarehouse.Location = new Point(15, 35);
            this.lblWarehouse.Name = "lblWarehouse";
            this.lblWarehouse.Size = new Size(35, 17);
            this.lblWarehouse.TabIndex = 0;
            this.lblWarehouse.Text = "Kho:";

            // cboWarehouse
            this.cboWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboWarehouse.Font = new Font("Microsoft Sans Serif", 10F);
            this.cboWarehouse.FormattingEnabled = true;
            this.cboWarehouse.Location = new Point(60, 32);
            this.cboWarehouse.Name = "cboWarehouse";
            this.cboWarehouse.Size = new Size(250, 24);
            this.cboWarehouse.TabIndex = 1;

            // btnSave
            this.btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnSave.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Location = new Point(1000, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(180, 45);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "� XÁC NHẬN LƯU";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            // lblStatus
            this.lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblStatus.Location = new Point(10, 680);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(120, 15);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Sẵn sàng - Chọn file Excel...";
            this.lblStatus.ForeColor = Color.Blue;

            // OpeningInventoryUserControl
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpExcelImport);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblStatus);
            this.Name = "OpeningInventoryUserControl";
            this.Size = new Size(1200, 700);
            
            this.grpExcelImport.ResumeLayout(false);
            this.grpExcelImport.PerformLayout();
            this.grpData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInput)).EndInit();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
