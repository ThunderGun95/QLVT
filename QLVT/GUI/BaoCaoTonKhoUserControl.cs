using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class BaoCaoTonKhoUserControl : UserControl
    {
        private readonly BaoCaoTonKhoBLL inventoryReportBLL;
        private List<InventoryReportItem> currentReportData = new();
        private InventoryReportSummary currentSummary = new();

        public BaoCaoTonKhoUserControl()
        {
            inventoryReportBLL = new BaoCaoTonKhoBLL();
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            SetupDataGridView();
            LoadWarehouses();
            ResetFilter();
            UpdateStatus("Sẵn sàng tạo báo cáo tồn kho", Color.Blue);
        }

        private void SetupDataGridView()
        {
            dgvReport.AutoGenerateColumns = false;
            dgvReport.AllowUserToAddRows = false;
            dgvReport.AllowUserToDeleteRows = false;
            dgvReport.ReadOnly = true;
            dgvReport.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvReport.MultiSelect = true;
            
            dgvReport.Columns.Clear();

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CodeVatTu",
                HeaderText = "Mã vật tư",
                DataPropertyName = "CodeVatTu",
                Width = 100
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                Width = 250
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenKho",
                HeaderText = "Kho",
                DataPropertyName = "TenKho",
                Width = 120
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongTon",
                HeaderText = "SL tồn",
                DataPropertyName = "SoLuongTon",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            });
        }

        private List<Warehouse> _warehouses = new List<Warehouse>();

        private void LoadWarehouses()
        {
            try
            {
                _warehouses = inventoryReportBLL.GetWarehousesForFilter();
                
                // Không dùng AutoComplete mặc định, sẽ tự implement tìm kiếm linh hoạt
                txtWarehouse.AutoCompleteMode = AutoCompleteMode.None;
                
                // Thêm event để xử lý tìm kiếm
                txtWarehouse.TextChanged += TxtWarehouse_TextChanged;
                txtWarehouse.KeyDown += TxtWarehouse_KeyDown;
                txtWarehouse.Leave += TxtWarehouse_Leave;
                
                // Thêm gợi ý mặc định "Tất cả"
                txtWarehouse.PlaceholderText = "Nhập tên kho hoặc 'Tất cả'...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetFilter()
        {
            var defaultFilter = inventoryReportBLL.GetDefaultFilter();
            
            txtWarehouse.Text = "";
            txtSearch.Text = "";
            chkChiHienThiCoTon.Checked = defaultFilter.ChiHienThiCoTon;
        }

        private async void btnCreateReport_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus("🔄 Đang tạo báo cáo...", Color.Blue);
                btnCreateReport.Enabled = false;
                btnExportExcel.Enabled = false;

                var filter = GetCurrentFilter();
                
                await Task.Run(() =>
                {
                    var (success, message, data, summary) = inventoryReportBLL.GetInventoryReport(filter);
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (success)
                        {
                            currentReportData = data;
                            currentSummary = summary;
                            
                            dgvReport.DataSource = data;
                            lblSummary.Text = inventoryReportBLL.FormatSummaryText(summary);
                            
                            UpdateStatus($"✅ {message}", Color.Green);
                            btnExportExcel.Enabled = data.Any();
                        }
                        else
                        {
                            dgvReport.DataSource = null;
                            lblSummary.Text = "";
                            UpdateStatus($"❌ {message}", Color.Red);
                            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Lỗi: {ex.Message}", Color.Red);
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCreateReport.Enabled = true;
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!currentReportData.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var (success, message, filePath) = inventoryReportBLL.ExportToExcel(currentReportData, currentSummary);
                
                if (success)
                {
                    MessageBox.Show($"Xuất Excel thành công!\n{message}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatus("✅ Xuất Excel thành công", Color.Green);
                }
                else
                {
                    MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus($"❌ {message}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"❌ Lỗi xuất Excel: {ex.Message}", Color.Red);
            }
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            ResetFilter();
            dgvReport.DataSource = null;
            lblSummary.Text = "";
            currentReportData.Clear();
            currentSummary = new InventoryReportSummary();
            UpdateStatus("Đã reset bộ lọc", Color.Blue);
        }

        private InventoryReportFilter GetCurrentFilter()
        {
            var searchText = txtSearch.Text.Trim();
            
            // Xác định warehouse ID từ tên kho được nhập
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
            
            return new InventoryReportFilter
            {
                AsOfDate = DateTime.Now.Date, // Luôn dùng ngày hiện tại
                WarehouseId = warehouseId,
                CodeVatTu = searchText,
                TenVatTu = searchText,
                NhaSanXuat = null, // Bỏ nhà sản xuất
                ChiHienThiCoTon = chkChiHienThiCoTon.Checked
            };
        }

        private void UpdateStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnCreateReport_Click(sender, e);
            }
        }

        private void txtWarehouse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnCreateReport_Click(sender, e);
            }
        }

        private void dgvReport_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Bỏ phân màu - tất cả dòng giống nhau
            // Không cần highlight nữa
        }

        #region Custom Warehouse AutoComplete

        private ListBox warehouseListBox = new ListBox();
        private bool suppressTextChanged = false;

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
            
            // Positioning
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
        }

        #endregion
    }
}
