using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.DAL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class OpeningInventoryUserControl : UserControl
    {
        private readonly OpeningInventoryBLL openingInventoryBLL;
        private readonly WarehouseDAL warehouseDAL;
        private List<OpeningInventoryInput> currentInputs = new();
        private List<OpeningInventory> currentOpeningInventories = new();

        public OpeningInventoryUserControl()
        {
            InitializeComponent();
            openingInventoryBLL = new OpeningInventoryBLL();
            warehouseDAL = new WarehouseDAL();
            SetupDataGridViews();
            LoadWarehouses();
            LoadOpeningInventories();
        }

        private void SetupDataGridViews()
        {
            // Setup DataGridView cho nhập mới
            SetupInputDataGridView();
            
            // Setup DataGridView cho tồn kho hiện tại
            SetupCurrentDataGridView();
        }

        private void SetupInputDataGridView()
        {
            dgvInput.AutoGenerateColumns = false;
            dgvInput.AllowUserToAddRows = true;
            dgvInput.AllowUserToDeleteRows = true;

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                Width = 50,
                ReadOnly = true
            });

            dgvInput.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "MaKho",
                HeaderText = "Kho",
                DataPropertyName = "MaKho",
                Width = 120
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaVatTu",
                HeaderText = "Mã vật tư",
                DataPropertyName = "MaVatTu",
                Width = 120
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư (mapping)",
                DataPropertyName = "TenVatTu",
                Width = 250,
                ReadOnly = true
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MappingStatus",
                HeaderText = "Trạng thái",
                DataPropertyName = "MappingStatus",
                Width = 100,
                ReadOnly = true
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GhiChu",
                HeaderText = "Ghi chú",
                DataPropertyName = "GhiChu",
                Width = 200
            });

            // Events
            dgvInput.RowsAdded += (s, e) => UpdateInputSTT();
            dgvInput.RowsRemoved += (s, e) => UpdateInputSTT();
            dgvInput.CellValueChanged += DgvInput_CellValueChanged;
            dgvInput.CellLeave += DgvInput_CellLeave;
        }

        private void SetupCurrentDataGridView()
        {
            dgvCurrent.AutoGenerateColumns = false;
            dgvCurrent.AllowUserToAddRows = false;
            dgvCurrent.AllowUserToDeleteRows = false;
            dgvCurrent.ReadOnly = true;
            dgvCurrent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                Width = 50
            });

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenKho",
                HeaderText = "Kho",
                DataPropertyName = "TenKho",
                Width = 120
            });

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CodeVatTu",
                HeaderText = "Mã VT",
                DataPropertyName = "CodeVatTu",
                Width = 100
            });

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                Width = 250
            });

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongTonDauKy",
                HeaderText = "Tồn đầu kỳ",
                DataPropertyName = "SoLuongTonDauKy",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                Width = 60
            });

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayNhapTon",
                HeaderText = "Ngày nhập",
                DataPropertyName = "NgayNhapTon",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvCurrent.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NguoiNhap",
                HeaderText = "Người nhập",
                DataPropertyName = "NguoiNhap",
                Width = 100
            });

            dgvCurrent.RowsAdded += (s, e) => UpdateCurrentSTT();
            dgvCurrent.RowsRemoved += (s, e) => UpdateCurrentSTT();
        }

        private void LoadWarehouses()
        {
            try
            {
                var warehouses = warehouseDAL.GetWarehouses();
                
                // Load vào ComboBox filter
                cboKhoFilter.Items.Clear();
                cboKhoFilter.Items.Add(new { MaKho = "", TenKho = "-- Tất cả kho --" });
                foreach (var warehouse in warehouses)
                {
                    cboKhoFilter.Items.Add(warehouse);
                }
                cboKhoFilter.DisplayMember = "TenKho";
                cboKhoFilter.ValueMember = "MaKho";
                cboKhoFilter.SelectedIndex = 0;

                // Load vào ComboBox column của DataGridView
                var maKhoColumn = dgvInput.Columns["MaKho"] as DataGridViewComboBoxColumn;
                if (maKhoColumn != null)
                {
                    maKhoColumn.Items.Clear();
                    foreach (var warehouse in warehouses)
                    {
                        maKhoColumn.Items.Add(warehouse.MaKho);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tải danh sách kho: {ex.Message}");
            }
        }

        private void LoadOpeningInventories()
        {
            try
            {
                lblStatus.Text = "Đang tải tồn kho đầu kỳ...";
                lblStatus.ForeColor = Color.Blue;

                string selectedKho = cboKhoFilter.SelectedValue?.ToString();
                if (selectedKho == "") selectedKho = null;

                currentOpeningInventories = openingInventoryBLL.GetOpeningInventories(selectedKho);
                dgvCurrent.DataSource = currentOpeningInventories;
                UpdateCurrentSTT();

                lblStatus.Text = $"✅ Đã tải {currentOpeningInventories.Count} bản ghi tồn kho đầu kỳ";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi tải tồn kho đầu kỳ: {ex.Message}");
            }
        }

        private void UpdateInputSTT()
        {
            for (int i = 0; i < dgvInput.Rows.Count; i++)
            {
                if (!dgvInput.Rows[i].IsNewRow)
                    dgvInput.Rows[i].Cells["STT"].Value = (i + 1).ToString();
            }
        }

        private void UpdateCurrentSTT()
        {
            for (int i = 0; i < dgvCurrent.Rows.Count; i++)
            {
                dgvCurrent.Rows[i].Cells["STT"].Value = (i + 1).ToString();
            }
        }

        private void DgvInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = dgvInput.Columns[e.ColumnIndex].Name;
                
                if (columnName == "MaVatTu" || columnName == "MaKho")
                {
                    PerformAutoMapping(e.RowIndex);
                }
            }
        }

        private void DgvInput_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            UpdateDataSourceFromGrid();
        }

        private void PerformAutoMapping(int rowIndex)
        {
            try
            {
                var row = dgvInput.Rows[rowIndex];
                if (row.IsNewRow) return;

                var maVatTu = row.Cells["MaVatTu"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(maVatTu)) return;

                var supplies = openingInventoryBLL.SearchSuppliesForMapping(maVatTu);
                var supply = supplies.FirstOrDefault(s => s.Code == maVatTu || s.ErpId.ToString() == maVatTu);

                if (supply != null)
                {
                    row.Cells["TenVatTu"].Value = supply.TenVatTu;
                    row.Cells["MappingStatus"].Value = "✅ Đã map";
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else
                {
                    row.Cells["TenVatTu"].Value = "";
                    row.Cells["MappingStatus"].Value = "❌ Chưa map";
                    row.DefaultCellStyle.BackColor = Color.LightPink;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi mapping: {ex.Message}");
            }
        }

        private void UpdateDataSourceFromGrid()
        {
            try
            {
                currentInputs.Clear();
                
                foreach (DataGridViewRow row in dgvInput.Rows)
                {
                    if (row.IsNewRow) continue;

                    var input = new OpeningInventoryInput
                    {
                        MaKho = row.Cells["MaKho"].Value?.ToString() ?? "COMPANY",
                        MaVatTu = row.Cells["MaVatTu"].Value?.ToString() ?? "",
                        SoLuong = int.TryParse(row.Cells["SoLuong"].Value?.ToString(), out int sl) ? sl : 0,
                        GhiChu = row.Cells["GhiChu"].Value?.ToString() ?? "",
                        TenVatTu = row.Cells["TenVatTu"].Value?.ToString()
                    };

                    if (!string.IsNullOrWhiteSpace(input.TenVatTu))
                    {
                        // Tìm SupplyId từ TenVatTu
                        var supplies = openingInventoryBLL.SearchSuppliesForMapping(input.MaVatTu);
                        var supply = supplies.FirstOrDefault(s => s.TenVatTu == input.TenVatTu);
                        if (supply != null)
                            input.SupplyId = supply.ErpId;
                    }

                    currentInputs.Add(input);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi cập nhật data source: {ex.Message}");
            }
        }

        private void btnAutoMapping_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateDataSourceFromGrid();
                
                lblStatus.Text = "Đang thực hiện auto mapping...";
                lblStatus.ForeColor = Color.Blue;

                currentInputs = openingInventoryBLL.ProcessAutoMapping(currentInputs);
                
                // Cập nhật lại grid
                dgvInput.DataSource = null;
                dgvInput.DataSource = currentInputs;
                UpdateInputSTT();

                // Cập nhật màu sắc
                for (int i = 0; i < dgvInput.Rows.Count; i++)
                {
                    var input = currentInputs[i];
                    if (input.IsMapped)
                    {
                        dgvInput.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else if (!string.IsNullOrWhiteSpace(input.MaVatTu))
                    {
                        dgvInput.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                    }
                }

                var mappedCount = currentInputs.Count(x => x.IsMapped);
                lblStatus.Text = $"✅ Đã mapping {mappedCount}/{currentInputs.Count} vật tư";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi mapping: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi thực hiện auto mapping: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateDataSourceFromGrid();

                // Validation
                var errors = openingInventoryBLL.ValidateInputs(currentInputs);
                if (errors.Any())
                {
                    MessageBox.Show($"Dữ liệu không hợp lệ:\n{string.Join("\n", errors)}", 
                        "Lỗi validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận
                var validInputs = currentInputs.Where(x => x.IsMapped && x.SoLuong > 0).ToList();
                if (!validInputs.Any())
                {
                    MessageBox.Show("Không có dữ liệu hợp lệ để lưu!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Xác nhận cập nhật tồn kho đầu kỳ cho {validInputs.Count} vật tư?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    lblStatus.Text = "Đang lưu tồn kho đầu kỳ...";
                    lblStatus.ForeColor = Color.Blue;

                    string nguoiNhap = "Admin"; // TODO: Lấy từ session hiện tại
                    int processedCount = openingInventoryBLL.UpdateOpeningInventories(validInputs, nguoiNhap);

                    lblStatus.Text = $"✅ Đã cập nhật {processedCount} bản ghi thành công";
                    lblStatus.ForeColor = Color.Green;

                    MessageBox.Show($"Đã cập nhật tồn kho đầu kỳ cho {processedCount} vật tư!", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload dữ liệu
                    LoadOpeningInventories();
                    
                    // Clear input grid
                    dgvInput.DataSource = null;
                    currentInputs.Clear();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi lưu: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi lưu tồn kho đầu kỳ: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCurrent.CurrentRow?.DataBoundItem is OpeningInventory selectedItem)
                {
                    var result = MessageBox.Show(
                        $"Xác nhận xóa tồn kho đầu kỳ:\n{selectedItem.TenVatTu} - {selectedItem.TenKho}?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        openingInventoryBLL.DeleteOpeningInventory(selectedItem.Id);
                        
                        MessageBox.Show("Đã xóa tồn kho đầu kỳ!", 
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        LoadOpeningInventories();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn bản ghi cần xóa!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi xóa: {ex.Message}");
            }
        }

        private void cboKhoFilter_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadOpeningInventories();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOpeningInventories();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
