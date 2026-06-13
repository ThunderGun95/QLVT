namespace QLVT.GUI
{
    partial class HoanUngBGKBatchUserControl
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.grpInput = new System.Windows.Forms.GroupBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.nudDenSo = new System.Windows.Forms.NumericUpDown();
            this.lblDenSo = new System.Windows.Forms.Label();
            this.nudTuSo = new System.Windows.Forms.NumericUpDown();
            this.lblTuSo = new System.Windows.Forms.Label();
            this.nudNam = new System.Windows.Forms.NumericUpDown();
            this.lblNam = new System.Windows.Forms.Label();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.lblTienDo = new System.Windows.Forms.Label();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblDaChon = new System.Windows.Forms.Label();
            this.lblTongSo = new System.Windows.Forms.Label();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnXoaTatCa = new System.Windows.Forms.Button();
            this.btnChonTatCa = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.btnBatDauXuLy = new System.Windows.Forms.Button();
            this.grpAvailable = new System.Windows.Forms.GroupBox();
            this.dgvDanhSachBGK = new System.Windows.Forms.DataGridView();
            this.grpSelected = new System.Windows.Forms.GroupBox();
            this.dgvDanhSachChon = new System.Windows.Forms.DataGridView();
            this.grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDenSo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTuSo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNam)).BeginInit();
            this.grpProgress.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpAvailable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachBGK)).BeginInit();
            this.grpSelected.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachChon)).BeginInit();
            this.SuspendLayout();

            // grpInput
            this.grpInput.Controls.Add(this.btnGenerate);
            this.grpInput.Controls.Add(this.nudDenSo);
            this.grpInput.Controls.Add(this.lblDenSo);
            this.grpInput.Controls.Add(this.nudTuSo);
            this.grpInput.Controls.Add(this.lblTuSo);
            this.grpInput.Controls.Add(this.nudNam);
            this.grpInput.Controls.Add(this.lblNam);
            this.grpInput.Location = new System.Drawing.Point(12, 12);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new System.Drawing.Size(1176, 80);
            this.grpInput.TabIndex = 0;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Thong tin nhap";

            // btnGenerate
            this.btnGenerate.Location = new System.Drawing.Point(490, 25);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(100, 25);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.Text = "Tao danh sach";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);

            // nudDenSo
            this.nudDenSo.Location = new System.Drawing.Point(390, 28);
            this.nudDenSo.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.nudDenSo.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudDenSo.Name = "nudDenSo";
            this.nudDenSo.Size = new System.Drawing.Size(80, 20);
            this.nudDenSo.TabIndex = 5;
            this.nudDenSo.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // lblDenSo
            this.lblDenSo.AutoSize = true;
            this.lblDenSo.Location = new System.Drawing.Point(340, 30);
            this.lblDenSo.Name = "lblDenSo";
            this.lblDenSo.Size = new System.Drawing.Size(44, 13);
            this.lblDenSo.TabIndex = 4;
            this.lblDenSo.Text = "Den so:";

            // nudTuSo
            this.nudTuSo.Location = new System.Drawing.Point(230, 28);
            this.nudTuSo.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.nudTuSo.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudTuSo.Name = "nudTuSo";
            this.nudTuSo.Size = new System.Drawing.Size(80, 20);
            this.nudTuSo.TabIndex = 3;
            this.nudTuSo.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // lblTuSo
            this.lblTuSo.AutoSize = true;
            this.lblTuSo.Location = new System.Drawing.Point(180, 30);
            this.lblTuSo.Name = "lblTuSo";
            this.lblTuSo.Size = new System.Drawing.Size(38, 13);
            this.lblTuSo.TabIndex = 2;
            this.lblTuSo.Text = "Tu so:";

            // nudNam
            this.nudNam.Location = new System.Drawing.Point(70, 28);
            this.nudNam.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            this.nudNam.Minimum = new decimal(new int[] { 2020, 0, 0, 0 });
            this.nudNam.Name = "nudNam";
            this.nudNam.Size = new System.Drawing.Size(80, 20);
            this.nudNam.TabIndex = 1;
            this.nudNam.Value = new decimal(new int[] { 2024, 0, 0, 0 });

            // lblNam
            this.lblNam.AutoSize = true;
            this.lblNam.Location = new System.Drawing.Point(20, 30);
            this.lblNam.Name = "lblNam";
            this.lblNam.Size = new System.Drawing.Size(32, 13);
            this.lblNam.TabIndex = 0;
            this.lblNam.Text = "Nam:";

            // grpProgress
            this.grpProgress.Controls.Add(this.lblTienDo);
            this.grpProgress.Controls.Add(this.lblTrangThai);
            this.grpProgress.Controls.Add(this.progressBar);
            this.grpProgress.Controls.Add(this.lblProgress);
            this.grpProgress.Controls.Add(this.lblDaChon);
            this.grpProgress.Controls.Add(this.lblTongSo);
            this.grpProgress.Location = new System.Drawing.Point(12, 98);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(1176, 100);
            this.grpProgress.TabIndex = 1;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Trang thai";

            // lblTienDo
            this.lblTienDo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTienDo.Location = new System.Drawing.Point(1060, 28);
            this.lblTienDo.Name = "lblTienDo";
            this.lblTienDo.Size = new System.Drawing.Size(100, 20);
            this.lblTienDo.TabIndex = 5;
            this.lblTienDo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // lblTrangThai
            this.lblTrangThai.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.Location = new System.Drawing.Point(750, 55);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(50, 13);
            this.lblTrangThai.TabIndex = 4;
            this.lblTrangThai.Text = "San sang";

            // progressBar
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(750, 28);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(300, 20);
            this.progressBar.TabIndex = 3;

            // lblProgress
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(20, 65);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 2;

            // lblDaChon
            this.lblDaChon.AutoSize = true;
            this.lblDaChon.Location = new System.Drawing.Point(20, 45);
            this.lblDaChon.Name = "lblDaChon";
            this.lblDaChon.Size = new System.Drawing.Size(0, 13);
            this.lblDaChon.TabIndex = 1;

            // lblTongSo
            this.lblTongSo.AutoSize = true;
            this.lblTongSo.Location = new System.Drawing.Point(20, 25);
            this.lblTongSo.Name = "lblTongSo";
            this.lblTongSo.Size = new System.Drawing.Size(0, 13);
            this.lblTongSo.TabIndex = 0;

            // grpActions
            this.grpActions.Controls.Add(this.btnXoaTatCa);
            this.grpActions.Controls.Add(this.btnChonTatCa);
            this.grpActions.Controls.Add(this.btnHuy);
            this.grpActions.Controls.Add(this.btnBatDauXuLy);
            this.grpActions.Location = new System.Drawing.Point(12, 204);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(1176, 60);
            this.grpActions.TabIndex = 2;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Thao tac";

            // btnXoaTatCa
            this.btnXoaTatCa.Location = new System.Drawing.Point(360, 25);
            this.btnXoaTatCa.Name = "btnXoaTatCa";
            this.btnXoaTatCa.Size = new System.Drawing.Size(100, 25);
            this.btnXoaTatCa.TabIndex = 3;
            this.btnXoaTatCa.Text = "Bo chon tat ca";
            this.btnXoaTatCa.UseVisualStyleBackColor = true;
            this.btnXoaTatCa.Click += new System.EventHandler(this.btnXoaTatCa_Click);

            // btnChonTatCa
            this.btnChonTatCa.Location = new System.Drawing.Point(250, 25);
            this.btnChonTatCa.Name = "btnChonTatCa";
            this.btnChonTatCa.Size = new System.Drawing.Size(100, 25);
            this.btnChonTatCa.TabIndex = 2;
            this.btnChonTatCa.Text = "Chon tat ca";
            this.btnChonTatCa.UseVisualStyleBackColor = true;
            this.btnChonTatCa.Click += new System.EventHandler(this.btnChonTatCa_Click);

            // btnHuy
            this.btnHuy.Enabled = false;
            this.btnHuy.Location = new System.Drawing.Point(150, 25);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(80, 25);
            this.btnHuy.TabIndex = 1;
            this.btnHuy.Text = "Huy";
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);

            // btnBatDauXuLy
            this.btnBatDauXuLy.Location = new System.Drawing.Point(20, 25);
            this.btnBatDauXuLy.Name = "btnBatDauXuLy";
            this.btnBatDauXuLy.Size = new System.Drawing.Size(120, 25);
            this.btnBatDauXuLy.TabIndex = 0;
            this.btnBatDauXuLy.Text = "Bat dau hoan ung";
            this.btnBatDauXuLy.UseVisualStyleBackColor = true;
            this.btnBatDauXuLy.Click += new System.EventHandler(this.btnBatDauXuLy_Click);

            // grpAvailable
            this.grpAvailable.Controls.Add(this.dgvDanhSachBGK);
            this.grpAvailable.Location = new System.Drawing.Point(12, 270);
            this.grpAvailable.Name = "grpAvailable";
            this.grpAvailable.Size = new System.Drawing.Size(580, 360);
            this.grpAvailable.TabIndex = 3;
            this.grpAvailable.TabStop = false;
            this.grpAvailable.Text = "Danh sach BGK co the chon";

            // dgvDanhSachBGK
            this.dgvDanhSachBGK.AllowUserToAddRows = false;
            this.dgvDanhSachBGK.AllowUserToDeleteRows = false;
            this.dgvDanhSachBGK.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDanhSachBGK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDanhSachBGK.Location = new System.Drawing.Point(3, 16);
            this.dgvDanhSachBGK.Name = "dgvDanhSachBGK";
            this.dgvDanhSachBGK.ReadOnly = true;
            this.dgvDanhSachBGK.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDanhSachBGK.Size = new System.Drawing.Size(574, 341);
            this.dgvDanhSachBGK.TabIndex = 0;
            this.dgvDanhSachBGK.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDanhSachBGK_CellDoubleClick);

            // grpSelected
            this.grpSelected.Controls.Add(this.dgvDanhSachChon);
            this.grpSelected.Location = new System.Drawing.Point(608, 270);
            this.grpSelected.Name = "grpSelected";
            this.grpSelected.Size = new System.Drawing.Size(580, 360);
            this.grpSelected.TabIndex = 4;
            this.grpSelected.TabStop = false;
            this.grpSelected.Text = "Danh sach BGK da chon";

            // dgvDanhSachChon
            this.dgvDanhSachChon.AllowUserToAddRows = false;
            this.dgvDanhSachChon.AllowUserToDeleteRows = false;
            this.dgvDanhSachChon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDanhSachChon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDanhSachChon.Location = new System.Drawing.Point(3, 16);
            this.dgvDanhSachChon.Name = "dgvDanhSachChon";
            this.dgvDanhSachChon.ReadOnly = true;
            this.dgvDanhSachChon.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDanhSachChon.Size = new System.Drawing.Size(574, 341);
            this.dgvDanhSachChon.TabIndex = 0;
            this.dgvDanhSachChon.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDanhSachChon_CellDoubleClick);

            // HoanUngBGKBatchUserControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSelected);
            this.Controls.Add(this.grpAvailable);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpProgress);
            this.Controls.Add(this.grpInput);
            this.Name = "HoanUngBGKBatchUserControl";
            this.Size = new System.Drawing.Size(1200, 640);
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDenSo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTuSo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNam)).EndInit();
            this.grpProgress.ResumeLayout(false);
            this.grpProgress.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpAvailable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachBGK)).EndInit();
            this.grpSelected.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachChon)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nudDenSo;
        private System.Windows.Forms.Label lblDenSo;
        private System.Windows.Forms.NumericUpDown nudTuSo;
        private System.Windows.Forms.Label lblTuSo;
        private System.Windows.Forms.NumericUpDown nudNam;
        private System.Windows.Forms.Label lblNam;
        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.Label lblTienDo;
        private System.Windows.Forms.Label lblTrangThai;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblDaChon;
        private System.Windows.Forms.Label lblTongSo;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnXoaTatCa;
        private System.Windows.Forms.Button btnChonTatCa;
        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.Button btnBatDauXuLy;
        private System.Windows.Forms.GroupBox grpAvailable;
        private System.Windows.Forms.DataGridView dgvDanhSachBGK;
        private System.Windows.Forms.GroupBox grpSelected;
        private System.Windows.Forms.DataGridView dgvDanhSachChon;
    }
}
