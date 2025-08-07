namespace QLVT.GUI
{
    partial class OpeningInventoryUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private TabControl tabControl;
        private TabPage tabInput;
        private TabPage tabCurrent;
        private GroupBox grpInput;
        private DataGridView dgvInput;
        private Button btnAutoMapping;
        private Button btnSave;
        private GroupBox grpCurrent;
        private ComboBox cboKhoFilter;
        private Label lblKhoFilter;
        private DataGridView dgvCurrent;
        private Button btnDelete;
        private Button btnRefresh;
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
            this.tabControl = new TabControl();
            this.tabInput = new TabPage();
            this.grpInput = new GroupBox();
            this.dgvInput = new DataGridView();
            this.btnAutoMapping = new Button();
            this.btnSave = new Button();
            this.tabCurrent = new TabPage();
            this.grpCurrent = new GroupBox();
            this.lblKhoFilter = new Label();
            this.cboKhoFilter = new ComboBox();
            this.dgvCurrent = new DataGridView();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.lblStatus = new Label();
            
            this.tabControl.SuspendLayout();
            this.tabInput.SuspendLayout();
            this.grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInput)).BeginInit();
            this.tabCurrent.SuspendLayout();
            this.grpCurrent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrent)).BeginInit();
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
            this.lblTitle.Text = "NHẬP TỒN KHO ĐẦU KỲ";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // tabControl
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Font = new Font("Microsoft Sans Serif", 9F);
            this.tabControl.Location = new Point(0, 40);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(1200, 620);
            this.tabControl.TabIndex = 1;

            // tabInput
            this.tabInput.Controls.Add(this.grpInput);
            this.tabInput.Location = new Point(4, 24);
            this.tabInput.Name = "tabInput";
            this.tabInput.Padding = new Padding(3);
            this.tabInput.Size = new Size(1192, 592);
            this.tabInput.TabIndex = 0;
            this.tabInput.Text = "📝 Nhập tồn mới";
            this.tabInput.UseVisualStyleBackColor = true;

            // grpInput
            this.grpInput.Controls.Add(this.dgvInput);
            this.grpInput.Controls.Add(this.btnAutoMapping);
            this.grpInput.Controls.Add(this.btnSave);
            this.grpInput.Dock = DockStyle.Fill;
            this.grpInput.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpInput.Location = new Point(3, 3);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new Size(1186, 586);
            this.grpInput.TabIndex = 0;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Nhập tồn kho đầu kỳ";

            // dgvInput
            this.dgvInput.AllowUserToOrderColumns = true;
            this.dgvInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvInput.BackgroundColor = Color.White;
            this.dgvInput.BorderStyle = BorderStyle.Fixed3D;
            this.dgvInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInput.Location = new Point(15, 25);
            this.dgvInput.Name = "dgvInput";
            this.dgvInput.RowHeadersWidth = 30;
            this.dgvInput.Size = new Size(1155, 500);
            this.dgvInput.TabIndex = 0;

            // btnAutoMapping
            this.btnAutoMapping.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnAutoMapping.BackColor = Color.FromArgb(255, 193, 7);
            this.btnAutoMapping.FlatStyle = FlatStyle.Flat;
            this.btnAutoMapping.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.btnAutoMapping.ForeColor = Color.Black;
            this.btnAutoMapping.Location = new Point(15, 540);
            this.btnAutoMapping.Name = "btnAutoMapping";
            this.btnAutoMapping.Size = new Size(150, 35);
            this.btnAutoMapping.TabIndex = 1;
            this.btnAutoMapping.Text = "🔗 Auto Mapping";
            this.btnAutoMapping.UseVisualStyleBackColor = false;
            this.btnAutoMapping.Click += new EventHandler(this.btnAutoMapping_Click);

            // btnSave
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSave.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Location = new Point(1020, 540);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(150, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "💾 LÂU TỒN KHO";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            // tabCurrent
            this.tabCurrent.Controls.Add(this.grpCurrent);
            this.tabCurrent.Location = new Point(4, 24);
            this.tabCurrent.Name = "tabCurrent";
            this.tabCurrent.Padding = new Padding(3);
            this.tabCurrent.Size = new Size(1192, 592);
            this.tabCurrent.TabIndex = 1;
            this.tabCurrent.Text = "📊 Tồn kho hiện tại";
            this.tabCurrent.UseVisualStyleBackColor = true;

            // grpCurrent
            this.grpCurrent.Controls.Add(this.lblKhoFilter);
            this.grpCurrent.Controls.Add(this.cboKhoFilter);
            this.grpCurrent.Controls.Add(this.dgvCurrent);
            this.grpCurrent.Controls.Add(this.btnDelete);
            this.grpCurrent.Controls.Add(this.btnRefresh);
            this.grpCurrent.Dock = DockStyle.Fill;
            this.grpCurrent.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpCurrent.Location = new Point(3, 3);
            this.grpCurrent.Name = "grpCurrent";
            this.grpCurrent.Size = new Size(1186, 586);
            this.grpCurrent.TabIndex = 0;
            this.grpCurrent.TabStop = false;
            this.grpCurrent.Text = "Tồn kho đầu kỳ hiện tại";

            // lblKhoFilter
            this.lblKhoFilter.AutoSize = true;
            this.lblKhoFilter.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblKhoFilter.Location = new Point(15, 30);
            this.lblKhoFilter.Name = "lblKhoFilter";
            this.lblKhoFilter.Size = new Size(63, 15);
            this.lblKhoFilter.TabIndex = 0;
            this.lblKhoFilter.Text = "Lọc theo kho:";

            // cboKhoFilter
            this.cboKhoFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboKhoFilter.Font = new Font("Microsoft Sans Serif", 9F);
            this.cboKhoFilter.FormattingEnabled = true;
            this.cboKhoFilter.Location = new Point(90, 27);
            this.cboKhoFilter.Name = "cboKhoFilter";
            this.cboKhoFilter.Size = new Size(200, 23);
            this.cboKhoFilter.TabIndex = 1;
            this.cboKhoFilter.SelectedValueChanged += new EventHandler(this.cboKhoFilter_SelectedValueChanged);

            // dgvCurrent
            this.dgvCurrent.AllowUserToAddRows = false;
            this.dgvCurrent.AllowUserToDeleteRows = false;
            this.dgvCurrent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvCurrent.BackgroundColor = Color.White;
            this.dgvCurrent.BorderStyle = BorderStyle.Fixed3D;
            this.dgvCurrent.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCurrent.Location = new Point(15, 60);
            this.dgvCurrent.Name = "dgvCurrent";
            this.dgvCurrent.ReadOnly = true;
            this.dgvCurrent.RowHeadersWidth = 30;
            this.dgvCurrent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvCurrent.Size = new Size(1155, 465);
            this.dgvCurrent.TabIndex = 2;

            // btnRefresh
            this.btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.Font = new Font("Microsoft Sans Serif", 9F);
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Location = new Point(15, 540);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(100, 35);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // btnDelete
            this.btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.Location = new Point(1070, 540);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new Size(100, 35);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "🗑️ Xóa";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // lblStatus
            this.lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Microsoft Sans Serif", 9F);
            this.lblStatus.Location = new Point(10, 670);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(70, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Sẵn sàng...";

            // OpeningInventoryUserControl
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblTitle);
            this.Name = "OpeningInventoryUserControl";
            this.Size = new Size(1200, 700);
            
            this.tabControl.ResumeLayout(false);
            this.tabInput.ResumeLayout(false);
            this.grpInput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInput)).EndInit();
            this.tabCurrent.ResumeLayout(false);
            this.grpCurrent.ResumeLayout(false);
            this.grpCurrent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
