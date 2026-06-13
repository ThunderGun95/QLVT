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
        private TextBox txtWarehouse; // Use simple TextBox like BaoCaoTonKho
        private Label lblVatTu;
        private VatTuTextBox vatTuTextBox;
        
        // Custom warehouse dropdown
        private ListBox warehouseListBox = new ListBox();
        private bool suppressTextChanged = false;

        public BaoCaoXuatNhapTonUserControl()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
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
                SetupWarehouseTextBox();
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

        private void SetupWarehouseTextBox()
        {
            // Create simple TextBox like in BaoCaoTonKhoUserControl
            txtWarehouse = new TextBox
            {
                Location = new Point(20, 95),  // Dưới label "Kho:" ở (20, 75)
                Size = new Size(330, 25),
                AutoCompleteMode = AutoCompleteMode.None
            };
            
            // Add to filters group
            grpFilters.Controls.Add(txtWarehouse);
            
            // Setup event handlers will be done in LoadWarehouses
        }

        private void SetupVatTuTextBox()
        {
            // Create label for VatTu - aligned with other filter labels
            lblVatTu = new Label
            {
                Text = "Vật tư:",
                Location = new Point(380, 75), // Cạnh Kho, cùng dòng với label Kho
                Size = new Size(50, 13),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0)
            };
            
            // Create VatTuTextBox component - aligned with other controls
            vatTuTextBox = new VatTuTextBox(330, 20)
            {
                Location = new Point(380, 95), // Dưới label "Vật tư:"
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
                // Truyền tên kho từ txtWarehouse nếu có, ngược lại dùng tên kho từ selected item
                var warehouseName = !string.IsNullOrWhiteSpace(txtWarehouse.Text) ? txtWarehouse.Text : selectedItem.TenKho;
                
                _activeDetailForm = new BaoCaoXuatNhapTonChiTietForm(
                    filter.TuNgay,          // Từ ngày
                    filter.DenNgay,         // Đến ngày  
                    warehouseName,          // Tên kho
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
                
                // Setup events like BaoCaoTonKho
                txtWarehouse.TextChanged += TxtWarehouse_TextChanged;
                txtWarehouse.KeyDown += TxtWarehouse_KeyDown;
                txtWarehouse.KeyPress += TxtWarehouse_KeyPress;
                txtWarehouse.Leave += TxtWarehouse_Leave;
                
                // Thêm gợi ý mặc định "Tất cả"
                txtWarehouse.PlaceholderText = "Nhập tên kho hoặc 'Tất cả'...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDefaultFilter()
        {
            // Tạo filter mặc định cho báo cáo tổng hợp
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            txtWarehouse.Text = ""; // Default to empty (all warehouses)
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

            // Xác định warehouse ID từ tên kho được nhập (như BaoCaoTonKho)
            int? warehouseId = null;
            var warehouseText = txtWarehouse.Text.Trim();
            
            // Kiểm tra nếu người dùng nhập "Tất cả" hoặc để trống thì không lọc theo kho
            if (!string.IsNullOrEmpty(warehouseText) && 
                !warehouseText.Equals("Tất cả", StringComparison.OrdinalIgnoreCase) &&
                !warehouseText.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                // Tìm kho khớp chính xác trước
                var exactWarehouse = _warehouses.FirstOrDefault(w => 
                    w.TenKho.Equals(warehouseText, StringComparison.OrdinalIgnoreCase));
                
                if (exactWarehouse != null)
                {
                    warehouseId = exactWarehouse.Id;
                }
                else
                {
                    // Nếu không khớp chính xác, tìm kho đầu tiên có chứa text
                    var partialWarehouse = _warehouses.FirstOrDefault(w => 
                        w.TenKho.Contains(warehouseText, StringComparison.OrdinalIgnoreCase));
                    warehouseId = partialWarehouse?.Id;
                }
            }
            
            filter.WarehouseId = warehouseId;

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

        #region Custom Warehouse AutoComplete (from BaoCaoTonKho)

        private void TxtWarehouse_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChanged) return;

            var searchText = txtWarehouse.Text;
            
            if (string.IsNullOrEmpty(searchText))
            {
                HideWarehouseDropdown();
                return;
            }

            var matchingItems = new List<string>();
            
            // Thêm tùy chọn "Tất cả" nếu người dùng nhập từ khóa phù hợp
            if ("Tất cả".Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                "tat ca".Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                "all".Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                matchingItems.Add("Tất cả");
            }

            // Tìm các kho có chứa text (không phân biệt hoa thường)
            var matchingWarehouses = _warehouses
                .Where(w => w.TenKho.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Select(w => w.TenKho)
                .ToList();
                
            matchingItems.AddRange(matchingWarehouses);

            if (matchingItems.Any())
            {
                ShowWarehouseDropdown(matchingItems);
            }
            else
            {
                HideWarehouseDropdown();
            }
        }

        private void TxtWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (warehouseListBox.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        if (warehouseListBox.SelectedIndex < warehouseListBox.Items.Count - 1)
                            warehouseListBox.SelectedIndex++;
                        e.Handled = true;
                        break;
                    case Keys.Up:
                        if (warehouseListBox.SelectedIndex > 0)
                            warehouseListBox.SelectedIndex--;
                        e.Handled = true;
                        break;
                    case Keys.Enter:
                        if (warehouseListBox.SelectedIndex >= 0)
                        {
                            SelectWarehouse(warehouseListBox.SelectedItem?.ToString() ?? "");
                        }
                        e.Handled = true;
                        break;
                    case Keys.Escape:
                        HideWarehouseDropdown();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void TxtWarehouse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnCreateReport_Click(sender, e);
            }
        }

        private void TxtWarehouse_Leave(object sender, EventArgs e)
        {
            // Delay để cho phép click vào listbox
            Task.Delay(200).ContinueWith(_ =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (!warehouseListBox.Focused)
                    {
                        HideWarehouseDropdown();
                    }
                });
            });
        }

        private void ShowWarehouseDropdown(List<string> items)
        {
            if (!this.Controls.Contains(warehouseListBox))
            {
                this.Controls.Add(warehouseListBox);
                warehouseListBox.BringToFront();
                
                warehouseListBox.Click += (s, e) =>
                {
                    if (warehouseListBox.SelectedItem != null)
                    {
                        SelectWarehouse(warehouseListBox.SelectedItem.ToString() ?? "");
                    }
                };
            }

            warehouseListBox.Items.Clear();
            warehouseListBox.Items.AddRange(items.ToArray());
            
            // Positioning - need to adjust based on grpFilters
            var txtLocation = txtWarehouse.PointToScreen(Point.Empty);
            var parentLocation = this.PointToScreen(Point.Empty);
            
            warehouseListBox.Location = new Point(
                txtLocation.X - parentLocation.X,
                txtLocation.Y - parentLocation.Y + txtWarehouse.Height
            );
            warehouseListBox.Width = txtWarehouse.Width;
            warehouseListBox.Height = Math.Min(150, items.Count * 20 + 4);
            
            warehouseListBox.Visible = true;
            
            if (items.Count > 0)
            {
                warehouseListBox.SelectedIndex = 0;
            }
        }

        private void HideWarehouseDropdown()
        {
            warehouseListBox.Visible = false;
        }

        private void SelectWarehouse(string warehouseName)
        {
            suppressTextChanged = true;
            txtWarehouse.Text = warehouseName;
            suppressTextChanged = false;
            
            HideWarehouseDropdown();
            txtWarehouse.SelectionStart = txtWarehouse.Text.Length;
            UpdateStatusLabel($"Đã chọn kho: {warehouseName}");
        }

        #endregion
    }
}
