using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class MappingForm : Form
    {
        private List<ERPExportOrderDetail> orderDetails;
        private ExportTransactionBLL exportBLL;
        private DataGridView dgvOrderDetails;
        private DataGridView dgvSearchResults;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnClearSearch;
        private Button btnSave;
        private Button btnCancel;
        private Label lblStatus;
        private Label lblInstruction;

        public MappingForm(List<ERPExportOrderDetail> details, ExportTransactionBLL bll)
        {
            orderDetails = details;
            exportBLL = bll;
            InitializeComponent();
            LoadOrderDetails();
        }

        private void InitializeComponent()
        {
            this.Text = "Mapping thủ công vật tư";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Instruction
            lblInstruction = new Label
            {
                Text = "Click vào vật tư chưa mapping → Tìm kiếm → Double-click để chọn",
                Location = new Point(20, 20),
                Size = new Size(500, 20),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            this.Controls.Add(lblInstruction);

            // Order details grid
            dgvOrderDetails = new DataGridView
            {
                Location = new Point(20, 50),
                Size = new Size(940, 250),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };

            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaVatTuHangHoa", HeaderText = "Mã ERP", DataPropertyName = "MaVatTuHangHoa", Width = 100 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenVatTu", HeaderText = "Tên vật tư ERP", DataPropertyName = "TenVatTu", Width = 200 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoLuong", HeaderText = "Số lượng", DataPropertyName = "SoLuong", Width = 80 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "DonViTinh", HeaderText = "ĐVT", DataPropertyName = "DonViTinh", Width = 60 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "MappedSupplyCode", HeaderText = "Mã VT Map", DataPropertyName = "MappedSupplyCode", Width = 100 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { Name = "MappedSupplyName", HeaderText = "Tên VT Map", DataPropertyName = "MappedSupplyName", Width = 180 });
            dgvOrderDetails.Columns.Add(new DataGridViewCheckBoxColumn { Name = "IsMapped", HeaderText = "Mapped", DataPropertyName = "IsMapped", Width = 70 });

            dgvOrderDetails.CellClick += DgvOrderDetails_CellClick;
            this.Controls.Add(dgvOrderDetails);

            // Search section
            var lblSearch = new Label
            {
                Text = "Tìm kiếm vật tư:",
                Location = new Point(20, 320),
                Size = new Size(100, 20),
                Font = new Font("Microsoft Sans Serif", 9F)
            };
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(120, 318),
                Size = new Size(200, 23),
                Font = new Font("Microsoft Sans Serif", 9F)
            };
            txtSearch.KeyPress += TxtSearch_KeyPress;
            this.Controls.Add(txtSearch);

            btnSearch = new Button
            {
                Text = "🔍 Tìm",
                Location = new Point(330, 316),
                Size = new Size(70, 27),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            btnClearSearch = new Button
            {
                Text = "🗑️ Xóa",
                Location = new Point(410, 316),
                Size = new Size(70, 27),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearSearch.Click += BtnClearSearch_Click;
            this.Controls.Add(btnClearSearch);

            // Search results grid
            dgvSearchResults = new DataGridView
            {
                Location = new Point(20, 360),
                Size = new Size(940, 200),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.LightYellow,
                Visible = false
            };

            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "Mã VT", DataPropertyName = "Code", Width = 100 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenVatTu", HeaderText = "Tên vật tư", DataPropertyName = "TenVatTu", Width = 250 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenDVT", HeaderText = "ĐVT", DataPropertyName = "TenDVT", Width = 80 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "ErpId", HeaderText = "ERP ID", DataPropertyName = "ErpId", Width = 80 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenNhaSanXuat", HeaderText = "Nhà sản xuất", DataPropertyName = "TenNhaSanXuat", Width = 150 });

            dgvSearchResults.CellDoubleClick += DgvSearchResults_CellDoubleClick;
            this.Controls.Add(dgvSearchResults);

            // Bottom buttons
            btnSave = new Button
            {
                Text = "✅ Lưu",
                Location = new Point(800, 580),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "❌ Hủy",
                Location = new Point(890, 580),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            // Status label
            lblStatus = new Label
            {
                Location = new Point(20, 585),
                Size = new Size(600, 20),
                Font = new Font("Microsoft Sans Serif", 9F),
                Text = "Sẵn sàng..."
            };
            this.Controls.Add(lblStatus);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadOrderDetails()
        {
            dgvOrderDetails.DataSource = orderDetails;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            var mappedCount = orderDetails.Count(d => d.IsMapped);
            var totalCount = orderDetails.Count;
            var unmappedCount = totalCount - mappedCount;

            if (unmappedCount == 0)
            {
                lblStatus.Text = $"✅ Đã mapping đầy đủ: {mappedCount}/{totalCount} vật tư";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = $"⚠️ Còn {unmappedCount}/{totalCount} vật tư chưa mapping";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void DgvOrderDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedDetail = orderDetails[e.RowIndex];
                if (!selectedDetail.IsMapped)
                {
                    txtSearch.Text = selectedDetail.TenVatTu;
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    
                    // Auto search
                    BtnSearch_Click(sender, e);
                }
            }
        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnSearch_Click(sender, e);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    dgvSearchResults.Visible = false;
                    return;
                }

                lblStatus.Text = "Đang tìm kiếm...";
                lblStatus.ForeColor = Color.Blue;
                Application.DoEvents();

                var results = exportBLL.SearchSuppliesForMapping(txtSearch.Text.Trim());
                dgvSearchResults.DataSource = results;
                dgvSearchResults.Visible = results.Any();

                if (results.Any())
                {
                    lblStatus.Text = $"Tìm thấy {results.Count} vật tư";
                    lblStatus.ForeColor = Color.Blue;
                }
                else
                {
                    lblStatus.Text = "Không tìm thấy vật tư nào";
                    lblStatus.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            dgvSearchResults.DataSource = null;
            dgvSearchResults.Visible = false;
            lblStatus.Text = "Sẵn sàng...";
            lblStatus.ForeColor = Color.Black;
        }

        private void DgvSearchResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvOrderDetails.CurrentRow != null)
                {
                    var selectedSupply = dgvSearchResults.Rows[e.RowIndex].DataBoundItem as Supply;
                    var currentDetail = orderDetails[dgvOrderDetails.CurrentRow.Index];

                    if (selectedSupply != null && currentDetail != null)
                    {
                        // Apply mapping
                        currentDetail.MappedSupplyId = selectedSupply.ErpId;
                        currentDetail.MappedSupplyCode = selectedSupply.Code;
                        currentDetail.MappedSupplyName = selectedSupply.TenVatTu;
                        currentDetail.MappedUnit = selectedSupply.TenDVT;

                        // Refresh display
                        dgvOrderDetails.Refresh();
                        UpdateStatus();
                        BtnClearSearch_Click(sender, e);
                        
                        lblStatus.Text = $"✅ Đã map: {selectedSupply.Code} - {selectedSupply.TenVatTu}";
                        lblStatus.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mapping:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
