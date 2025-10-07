using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;
using QLVT.GUI.Components;

namespace QLVT.GUI
{
    /// <summary>
    /// UserControl cho báo cáo xuất nhập tồn với chế độ Tổng và Chi tiết
    /// </summary>
    public partial class BaoCaoXuatNhapTonUserControl : UserControl
    {
        private readonly BaoCaoXuatNhapTonTongBLL _summaryBll;
        private List<TransactionSummaryReportItem> _currentSummaryData;
        private List<Warehouse> _warehouses;
        private BaoCaoXuatNhapTonChiTietForm? _activeDetailForm; // Track active detail window
        private WarehouseComboBox warehouseComboBox;
        private Label lblVatTu;
        private VatTuTextBox vatTuTextBox;

        public BaoCaoXuatNhapTonUserControl()
        {
            InitializeComponent();
            _summaryBll = new BaoCaoXuatNhapTonTongBLL();
            _currentSummaryData = new List<TransactionSummaryReportItem>();
            _warehouses = new List<Warehouse>();
            
            InitializeForm();
        }

        private async void InitializeForm()
        {
            try
            {
                SetupDataGridView();
                SetupWarehouseComboBox();
                SetupVatTuTextBox();
                await LoadWarehouses();
                LoadDefaultFilter();
                UpdateStatusLabel("Sẵn sàng tạo báo cáo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            dgvTransactionReport.AutoGenerateColumns = false;
            dgvTransactionReport.AllowUserToAddRows = false;
            dgvTransactionReport.AllowUserToDeleteRows = false;
            dgvTransactionReport.ReadOnly = true; // Readonly vì không có button
            dgvTransactionReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransactionReport.MultiSelect = false;
            dgvTransactionReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Clear existing columns
            dgvTransactionReport.Columns.Clear();

            // Mặc định chỉ có báo cáo tổng hợp
            SetupSummaryColumns();
            dgvTransactionReport.CellDoubleClick += DgvTransactionReport_CellDoubleClick;

            // Add row formatting event
            dgvTransactionReport.CellFormatting += DgvTransactionReport_CellFormatting;
        }

        private void SetupSummaryColumns()
        {
            // Columns for summary mode
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "STT", HeaderText = "STT", Width = 50, FillWeight = 5, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CodeVatTu", HeaderText = "Mã VT", Width = 80, FillWeight = 8, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenVatTu", HeaderText = "Tên vật tư", Width = 200, FillWeight = 25, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DonViTinh", HeaderText = "ĐVT", Width = 50, FillWeight = 5, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenKho", HeaderText = "Kho", Width = 120, FillWeight = 12, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TonDauKy", HeaderText = "Tồn đầu kỳ", Width = 80, FillWeight = 8, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoNhap", HeaderText = "Số nhập", Width = 80, FillWeight = 8, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoXuat", HeaderText = "Số xuất", Width = 80, FillWeight = 8, ReadOnly = true });
            dgvTransactionReport.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TonCuoiKy", HeaderText = "Tồn cuối kỳ", Width = 80, FillWeight = 8, ReadOnly = true });

            // Format alignment for number columns
            var numberColumns = new[] { 5, 6, 7, 8 };
            foreach (var col in numberColumns)
            {
                dgvTransactionReport.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void SetupWarehouseComboBox()
        {
            // Create WarehouseComboBox component with constructor parameters (width, height, addAll)
            warehouseComboBox = new WarehouseComboBox(160, 25, true)
            {
                Location = new Point(380, 45)  // Đặt dưới label "Kho:" ở (380, 25)
            };
            
            // Add to filters group
            grpFilters.Controls.Add(warehouseComboBox);
            
            // Setup event handlers
            warehouseComboBox.WarehouseSelected += WarehouseComboBox_WarehouseSelected;
        }

        private void SetupVatTuTextBox()
        {
            // Create label for VatTu - aligned with other filter labels
            lblVatTu = new Label
            {
                Text = "Vật tư:",
                Location = new Point(560, 25), // Align with other labels at Y=25
                Size = new Size(50, 13),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            
            // Create VatTuTextBox component - aligned with other controls at Y=45
            vatTuTextBox = new VatTuTextBox(200, 20)
            {
                Location = new Point(560, 45), // Align with other controls at Y=45
                PlaceholderText = "Nhập mã hoặc tên vật tư..."
            };
            
            // Add to filters group
            grpFilters.Controls.Add(lblVatTu);
            grpFilters.Controls.Add(vatTuTextBox);
            
            // Setup event handlers
            vatTuTextBox.TextChanged += VatTuTextBox_TextChanged;
            vatTuTextBox.KeyPress += VatTuTextBox_KeyPress;
        }

        private void VatTuTextBox_TextChanged(object sender, EventArgs e)
        {
            // Handle text changed event if needed
        }

        private void VatTuTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Auto trigger search when Enter is pressed
                e.Handled = true;
                // You can add search logic here if needed
            }
        }


        private void DgvTransactionReport_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || _currentSummaryData == null || e.RowIndex >= _currentSummaryData.Count)
                {
                    return;
                }

                HandleRowDoubleClick(e.RowIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi double-click: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleRowDoubleClick(int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || _currentSummaryData == null || rowIndex >= _currentSummaryData.Count)
                {
                    return;
                }

                var selectedItem = _currentSummaryData[rowIndex];
                
                // Tạo filter cho báo cáo chi tiết
                var filter = CreateSummaryFilterFromForm();
                
                // Hiển thị thông báo đang tải
                UpdateStatusLabel($"Đang mở chi tiết vật tư: {selectedItem.TenVatTu}...");
                
                
                // Mở form báo cáo chi tiết mới với dữ liệu được điền sẵn
                _activeDetailForm = new BaoCaoXuatNhapTonChiTietForm(
                    filter.TuNgay,          // Từ ngày
                    filter.DenNgay,         // Đến ngày  
                    selectedItem.TenKho,    // Tên kho
                    selectedItem.CodeVatTu  // Mã vật tư
                );
                
                // Đăng ký event để clear reference khi form đóng
                _activeDetailForm.FormClosed += (s, e) => _activeDetailForm = null;
                
                _activeDetailForm.Show();
                
                // Khôi phục trạng thái
                UpdateStatusLabel($"Đã mở chi tiết vật tư: {selectedItem.TenVatTu}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvTransactionReport_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvTransactionReport.Rows.Count)
            {
                var row = dgvTransactionReport.Rows[e.RowIndex];

                // Format for summary mode only
                if (e.ColumnIndex == dgvTransactionReport.Columns["TonCuoiKy"]?.Index)
                {
                    if (int.TryParse(e.Value?.ToString(), out int tonCuoi) && tonCuoi < 0)
                    {
                        e.CellStyle.BackColor = Color.Red;
                        e.CellStyle.ForeColor = Color.White;
                        e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private async Task LoadWarehouses()
        {
            try
            {
                _warehouses = await _summaryBll.GetWarehousesAsync();
                // WarehouseComboBox will load its own data using NhapKhoManualBLL
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WarehouseComboBox_WarehouseSelected(object sender, WarehouseSelectedEventArgs e)
        {
            // Warehouse selected from NhapKho-style component
            UpdateStatusLabel($"Đã chọn kho: {e.SelectedWarehouse.TenKho}");
        }

        private void LoadDefaultFilter()
        {
            // Tạo filter mặc định cho báo cáo tổng hợp
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            warehouseComboBox?.ClearSelection(); // Default to "All warehouses"
            vatTuTextBox?.Clear(); // Sử dụng VatTuTextBox thay vì txtSupplyFilter
        }

        private async void btnCreateReport_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatusLabel("Đang tạo báo cáo...");
                btnCreateReport.Enabled = false;

                // Chỉ tạo báo cáo tổng hợp
                await CreateSummaryReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel("Có lỗi xảy ra");
            }
            finally
            {
                btnCreateReport.Enabled = true;
            }
        }        
        
        private async Task CreateSummaryReport()
        {
            var filter = CreateSummaryFilterFromForm();
            
            _currentSummaryData = await _summaryBll.GetTransactionSummaryAsync(filter);
            dgvTransactionReport.DataSource = _currentSummaryData;
            
            // Cập nhật summary bar
            UpdateSummaryLabel();
            
            UpdateStatusLabel($"Hoàn thành! Tìm thấy {_currentSummaryData.Count} vật tư. Double-click vào dòng để xem chi tiết.");
            btnExportCsv.Enabled = _currentSummaryData.Count > 0;
        }

        private async void btnExportCsv_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentSummaryData?.Count <= 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                UpdateStatusLabel("Đang xuất file Excel...");
                btnExportCsv.Enabled = false;

                var filter = CreateSummaryFilterFromForm();
                await _summaryBll.ExportToExcelAsync(_currentSummaryData, filter);

                UpdateStatusLabel("Xuất file Excel thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel("Lỗi xuất file");
            }
            finally
            {
                btnExportCsv.Enabled = true;
            }
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            LoadDefaultFilter();
            dgvTransactionReport.DataSource = null;
            lblSummary.Text = "";
            btnExportCsv.Enabled = false;
            UpdateStatusLabel("Đã xóa bộ lọc");
        }

        private TransactionSummaryFilter CreateSummaryFilterFromForm()
        {
            var filter = new TransactionSummaryFilter
            {
                TuNgay = dtpFromDate.Value.Date,
                DenNgay = dtpToDate.Value.Date
            };

            // Parse supply filter (can contain code or name)
            if (!string.IsNullOrWhiteSpace(vatTuTextBox.GetText()))
            {
                var searchTerm = vatTuTextBox.GetText().Trim();
                // If it looks like a code (alphanumeric, no spaces), use as code filter
                if (searchTerm.All(c => char.IsLetterOrDigit(c)))
                {
                    filter.CodeVatTu = searchTerm;
                }
                else
                {
                    filter.TenVatTu = searchTerm;
                }
            }

            // Warehouse - get from WarehouseComboBox component
            if (warehouseComboBox.SelectedWarehouseId > 0)
            {
                filter.WarehouseId = warehouseComboBox.SelectedWarehouseId;
            }

            return filter;
        }

        private void UpdateSummaryLabel()
        {
            if (_currentSummaryData == null || _currentSummaryData.Count == 0)
            {
                lblSummary.Text = "Không có dữ liệu";
                return;
            }

            var totalItems = _currentSummaryData.Count;
            var totalSoNhap = _currentSummaryData.Sum(item => item.SoNhap);
            var totalSoXuat = _currentSummaryData.Sum(item => item.SoXuat);
            var totalTonDauKy = _currentSummaryData.Sum(item => item.TonDauKy);
            var totalTonCuoiKy = _currentSummaryData.Sum(item => item.TonCuoiKy);

            lblSummary.Text = $"Tổng số vật tư: {totalItems:N0} | " +
                             $"Tổng tồn đầu: {totalTonDauKy:N0} | " +
                             $"Tổng nhập: {totalSoNhap:N0} | " +
                             $"Tổng xuất: {totalSoXuat:N0} | " +
                             $"Tổng tồn cuối: {totalTonCuoiKy:N0}";
        }

        private void UpdateStatusLabel(string message)
        {
            lblStatus.Text = $"Trạng thái: {message}";
            Application.DoEvents();
        }
    }
}
