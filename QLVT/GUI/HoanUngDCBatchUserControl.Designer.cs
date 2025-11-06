namespace QLVT.GUI
{
    partial class HoanUngDCBatchUserControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvChoHoanUng = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblTongSoChoHoanUng = new System.Windows.Forms.Label();
            this.dgvDaChon = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTongSoDaChon = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnBoChonTatCa = new System.Windows.Forms.Button();
            this.btnChonTatCa = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtKetQua = new System.Windows.Forms.TextBox();
            this.lblTienDo = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnBatDauHoanUng = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChoHoanUng)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaChon)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.dgvChoHoanUng, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvDaChon, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1200, 700);
            this.tableLayoutPanel1.TabIndex = 0;
            
            this.dgvChoHoanUng.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChoHoanUng.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChoHoanUng.Location = new System.Drawing.Point(3, 53);
            this.dgvChoHoanUng.Name = "dgvChoHoanUng";
            this.dgvChoHoanUng.Size = new System.Drawing.Size(594, 394);
            this.dgvChoHoanUng.TabIndex = 0;
            
            this.panel1.Controls.Add(this.lblConnectionStatus);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.lblTongSoChoHoanUng);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(594, 44);
            this.panel1.TabIndex = 1;
            
            this.lblConnectionStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConnectionStatus.Location = new System.Drawing.Point(350, 15);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(235, 20);
            this.lblConnectionStatus.TabIndex = 2;
            this.lblConnectionStatus.Text = "Đang kiểm tra kết nối...";
            this.lblConnectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            this.btnRefresh.Location = new System.Drawing.Point(10, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 25);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = " Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            
            this.lblTongSoChoHoanUng.AutoSize = true;
            this.lblTongSoChoHoanUng.Location = new System.Drawing.Point(120, 17);
            this.lblTongSoChoHoanUng.Name = "lblTongSoChoHoanUng";
            this.lblTongSoChoHoanUng.Size = new System.Drawing.Size(162, 13);
            this.lblTongSoChoHoanUng.TabIndex = 0;
            this.lblTongSoChoHoanUng.Text = "Tổng số đơn chờ hoàn ứng: 0";
            
            this.dgvDaChon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDaChon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDaChon.Location = new System.Drawing.Point(603, 53);
            this.dgvDaChon.Name = "dgvDaChon";
            this.dgvDaChon.Size = new System.Drawing.Size(594, 394);
            this.dgvDaChon.TabIndex = 2;
            
            this.panel2.Controls.Add(this.lblTongSoDaChon);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(603, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(594, 44);
            this.panel2.TabIndex = 3;
            
            this.lblTongSoDaChon.AutoSize = true;
            this.lblTongSoDaChon.Location = new System.Drawing.Point(10, 17);
            this.lblTongSoDaChon.Name = "lblTongSoDaChon";
            this.lblTongSoDaChon.Size = new System.Drawing.Size(89, 13);
            this.lblTongSoDaChon.TabIndex = 0;
            this.lblTongSoDaChon.Text = "Đã chọn: 0 đơn";
            
            this.tableLayoutPanel1.SetColumnSpan(this.panel3, 2);
            this.panel3.Controls.Add(this.btnBoChonTatCa);
            this.panel3.Controls.Add(this.btnChonTatCa);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 453);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1194, 44);
            this.panel3.TabIndex = 4;
            
            this.btnBoChonTatCa.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBoChonTatCa.Location = new System.Drawing.Point(610, 10);
            this.btnBoChonTatCa.Name = "btnBoChonTatCa";
            this.btnBoChonTatCa.Size = new System.Drawing.Size(150, 30);
            this.btnBoChonTatCa.TabIndex = 1;
            this.btnBoChonTatCa.Text = " Bỏ chọn tất cả";
            this.btnBoChonTatCa.UseVisualStyleBackColor = true;
            this.btnBoChonTatCa.Click += new System.EventHandler(this.BtnBoChonTatCa_Click);
            
            this.btnChonTatCa.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnChonTatCa.Location = new System.Drawing.Point(434, 10);
            this.btnChonTatCa.Name = "btnChonTatCa";
            this.btnChonTatCa.Size = new System.Drawing.Size(150, 30);
            this.btnChonTatCa.TabIndex = 0;
            this.btnChonTatCa.Text = " Chọn tất cả";
            this.btnChonTatCa.UseVisualStyleBackColor = true;
            this.btnChonTatCa.Click += new System.EventHandler(this.BtnChonTatCa_Click);
            
            this.tableLayoutPanel1.SetColumnSpan(this.panel4, 2);
            this.panel4.Controls.Add(this.txtKetQua);
            this.panel4.Controls.Add(this.lblTienDo);
            this.panel4.Controls.Add(this.progressBar);
            this.panel4.Controls.Add(this.btnBatDauHoanUng);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 503);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1194, 194);
            this.panel4.TabIndex = 5;
            
            this.txtKetQua.BackColor = System.Drawing.Color.Black;
            this.txtKetQua.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtKetQua.ForeColor = System.Drawing.Color.Lime;
            this.txtKetQua.Location = new System.Drawing.Point(10, 70);
            this.txtKetQua.Multiline = true;
            this.txtKetQua.Name = "txtKetQua";
            this.txtKetQua.ReadOnly = true;
            this.txtKetQua.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtKetQua.Size = new System.Drawing.Size(1174, 114);
            this.txtKetQua.TabIndex = 3;
            this.txtKetQua.Visible = false;
            
            this.lblTienDo.AutoSize = true;
            this.lblTienDo.Location = new System.Drawing.Point(10, 50);
            this.lblTienDo.Name = "lblTienDo";
            this.lblTienDo.Size = new System.Drawing.Size(0, 13);
            this.lblTienDo.TabIndex = 2;
            this.lblTienDo.Visible = false;
            
            this.progressBar.Location = new System.Drawing.Point(180, 15);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1004, 25);
            this.progressBar.TabIndex = 1;
            this.progressBar.Visible = false;
            
            this.btnBatDauHoanUng.Location = new System.Drawing.Point(10, 15);
            this.btnBatDauHoanUng.Name = "btnBatDauHoanUng";
            this.btnBatDauHoanUng.Size = new System.Drawing.Size(150, 30);
            this.btnBatDauHoanUng.TabIndex = 0;
            this.btnBatDauHoanUng.Text = " Bắt đầu hoàn ứng";
            this.btnBatDauHoanUng.UseVisualStyleBackColor = true;
            this.btnBatDauHoanUng.Click += new System.EventHandler(this.BtnBatDauHoanUng_Click);
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HoanUngDCBatchUserControl";
            this.Size = new System.Drawing.Size(1200, 700);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChoHoanUng)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaChon)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvChoHoanUng;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblTongSoChoHoanUng;
        private System.Windows.Forms.DataGridView dgvDaChon;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblTongSoDaChon;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnBoChonTatCa;
        private System.Windows.Forms.Button btnChonTatCa;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtKetQua;
        private System.Windows.Forms.Label lblTienDo;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnBatDauHoanUng;
        private System.Windows.Forms.Label lblConnectionStatus;
    }
}
