using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.DAL;
using QLVT.Models;
using ClosedXML.Excel;

namespace QLVT.GUI
{
    public partial class OpeningInventoryUserControl : UserControl
    {
        private readonly OpeningInventoryBLL openingInventoryBLL;
        private readonly NhapKhoTransactionDAL importTransactionDAL;
        private readonly WarehouseDAL warehouseDAL;
        private List<ExcelImportItem> currentExcelData = new();

        public OpeningInventoryUserControl()
        {
            InitializeComponent();
            openingInventoryBLL = new OpeningInventoryBLL();
            importTransactionDAL = new NhapKhoTransactionDAL();
            warehouseDAL = new WarehouseDAL();
            SetupDataGridView();
            LoadWarehouses();
            // Không load tồn kho ban đầu theo yêu cầu
        }

        private void SetupDataGridView()
        {
            dgvInput.AutoGenerateColumns = false;
            dgvInput.AllowUserToAddRows = false;
            dgvInput.AllowUserToDeleteRows = false;
            dgvInput.ReadOnly = true;
            dgvInput.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                Width = 50,
                ReadOnly = true
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ItemCode",
                HeaderText = "Mã vật tư",
                DataPropertyName = "ItemCode",
                Width = 120
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Số lượng",
                DataPropertyName = "Quantity",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvInput.RowsAdded += (s, e) => UpdateSTT();
            dgvInput.RowsRemoved += (s, e) => UpdateSTT();
        }

        private void LoadWarehouses()
        {
            try
            {
                var warehouses = warehouseDAL.GetAllWarehouses();
                
                cboWarehouse.Items.Clear();
                cboWarehouse.DisplayMember = "TenKho";
                cboWarehouse.ValueMember = "MaKho";
                
                foreach (var warehouse in warehouses)
                {
                    cboWarehouse.Items.Add(warehouse);
                }
                
                if (cboWarehouse.Items.Count > 0)
                    cboWarehouse.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi tải danh sách kho: {ex.Message}");
            }
        }

        private void UpdateSTT()
        {
            for (int i = 0; i < dgvInput.Rows.Count; i++)
            {
                dgvInput.Rows[i].Cells["STT"].Value = (i + 1).ToString();
            }
        }

        private void PerformAutoMapping()
        {
            foreach (var item in currentExcelData)
            {
                try
                {
                    // Mapping chính xác theo mã ERP/Code
                    var supply = openingInventoryBLL.FindSupplyByERPCode(item.ItemCode);

                    // Cập nhật thông tin mapping
                    if (supply != null)
                    {
                        item.SupplyId = supply.ErpId; // Sử dụng ErpId
                        item.SupplyName = supply.TenVatTu;
                        item.MappingStatus = "✅ Đã mapping";
                    }
                    else
                    {
                        item.SupplyId = null;
                        item.SupplyName = "";
                        item.MappingStatus = "❌ Không tìm thấy";
                    }
                }
                catch (Exception ex)
                {
                    item.SupplyId = null;
                    item.SupplyName = "";
                    item.MappingStatus = $"❌ Lỗi: {ex.Message}";
                }
            }
        }

        private void UpdateRowColors()
        {
            if (dgvInput.DataSource == null) return;

            for (int i = 0; i < dgvInput.Rows.Count && i < currentExcelData.Count; i++)
            {
                var item = currentExcelData[i];
                if (item.IsMapped)
                {
                    dgvInput.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else
                {
                    dgvInput.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                }
            }
        }

        private void btnBrowseExcel_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
                openFileDialog.Title = "Chọn file Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtExcelFilePath.Text = openFileDialog.FileName;
                    btnLoadExcel.Enabled = true;
                    
                    lblStatus.Text = $"✅ Đã chọn file: {Path.GetFileName(openFileDialog.FileName)}";
                    lblStatus.ForeColor = Color.Green;
                }
            }
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExcelFilePath.Text))
            {
                MessageBox.Show("Vui lòng chọn file Excel trước!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                lblStatus.Text = "Đang đọc file Excel...";
                lblStatus.ForeColor = Color.Blue;

                currentExcelData = LoadExcelData(txtExcelFilePath.Text);
                if (currentExcelData?.Any() == true)
                {
                    // Thực hiện mapping ngay sau khi load
                    lblStatus.Text = "Đang mapping với dữ liệu vật tư...";
                    lblStatus.ForeColor = Color.Blue;
                    
                    PerformAutoMapping();

                    // Hiển thị dữ liệu trong DataGridView
                    dgvInput.DataSource = null;
                    dgvInput.DataSource = currentExcelData;
                    UpdateSTT();

                    // Cập nhật màu sắc theo trạng thái mapping
                    UpdateRowColors();

                    var mappedCount = currentExcelData.Count(x => x.IsMapped);
                    var totalCount = currentExcelData.Count;
                    
                    lblStatus.Text = $"✅ Đã tải {totalCount} dòng - Mapping thành công: {mappedCount}/{totalCount}";
                    lblStatus.ForeColor = mappedCount == totalCount ? Color.Green : Color.Orange;

                    var warningMessage = "";
                    if (mappedCount < totalCount)
                    {
                        var unmappedCount = totalCount - mappedCount;
                        warningMessage = $"\n\n⚠️ Có {unmappedCount} vật tư chưa mapping được!";
                    }

                    MessageBox.Show($"Đã tải thành công {totalCount} dòng từ Excel!\n" +
                                  $"Mapping thành công: {mappedCount}/{totalCount} vật tư{warningMessage}\n\n" +
                                  $"Vui lòng chọn kho và xác nhận lưu.", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblStatus.Text = "❌ Không có dữ liệu hợp lệ trong file Excel";
                    lblStatus.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi đọc Excel: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi đọc file Excel: {ex.Message}");
            }
        }

        private List<ExcelImportItem> LoadExcelData(string filePath)
        {
            var excelData = new List<ExcelImportItem>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new Exception("File Excel không có worksheet nào!");
                }

                var range = worksheet.RangeUsed();
                if (range == null || range.RowCount() < 2)
                {
                    throw new Exception("File Excel phải có ít nhất 2 dòng (header + data)!");
                }

                // Đọc từ dòng 2 (bỏ qua header)
                for (int row = 2; row <= range.RowCount(); row++)
                {
                    var itemCodeCell = worksheet.Cell(row, 1);
                    var quantityCell = worksheet.Cell(row, 2);
                    
                    var itemCode = itemCodeCell.GetString()?.Trim();
                    var quantityText = quantityCell.GetString()?.Trim();

                    if (string.IsNullOrWhiteSpace(itemCode)) continue;

                    if (int.TryParse(quantityText, out int quantity) && quantity > 0)
                    {
                        excelData.Add(new ExcelImportItem
                        {
                            ItemCode = itemCode,
                            Quantity = quantity
                        });
                    }
                }
            }

            return excelData;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (!currentExcelData?.Any() == true)
                {
                    MessageBox.Show("Vui lòng tải dữ liệu từ Excel trước!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboWarehouse.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn kho!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy thông tin kho được chọn
                var selectedWarehouse = cboWarehouse.SelectedItem as Warehouse;
                var warehouseCode = selectedWarehouse.Id;

                // Debug: Kiểm tra mã kho truyền vào
                System.Diagnostics.Debug.WriteLine($"DEBUG: Warehouse Code = '{warehouseCode}', Name = '{selectedWarehouse?.TenKho}'");

                // Kiểm tra mã kho hợp lệ
                if (warehouseCode == 0)
                {
                    MessageBox.Show("Mã kho không hợp lệ!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra mapping
                var unmappedItems = currentExcelData.Where(x => !x.IsMapped).ToList();
                if (unmappedItems.Any())
                {
                    var unmappedCodes = string.Join(", ", unmappedItems.Take(5).Select(x => x.ItemCode));
                    var extraCount = unmappedItems.Count > 5 ? $" và {unmappedItems.Count - 5} vật tư khác" : "";
                    
                    var result = MessageBox.Show(
                        $"Có {unmappedItems.Count} vật tư chưa mapping được:\n{unmappedCodes}{extraCount}\n\n" +
                        $"Chỉ các vật tư đã mapping sẽ được lưu vào hệ thống.\n\n" +
                        $"Tiếp tục lưu {currentExcelData.Count(x => x.IsMapped)} vật tư đã mapping?",
                        "Cảnh báo mapping",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                // Lấy danh sách vật tư đã mapping
                var validItems = currentExcelData.Where(x => x.IsMapped).ToList();
                if (!validItems.Any())
                {
                    MessageBox.Show("Không có vật tư nào đã mapping để lưu!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Xác nhận cuối cùng
                var finalResult = MessageBox.Show(
                    $"Xác nhận cập nhật tồn kho đầu kỳ?\n\n" +
                    $"Kho: {selectedWarehouse?.TenKho}\n" +
                    $"Số lượng vật tư sẽ lưu: {validItems.Count}\n" +
                    $"Tổng số dòng từ Excel: {currentExcelData.Count}\n\n" +
                    $"Dữ liệu sẽ được lưu vào hệ thống.",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (finalResult == DialogResult.Yes)
                {
                    lblStatus.Text = "Đang lưu tồn kho đầu kỳ...";
                    lblStatus.ForeColor = Color.Blue;

                    // Debug: Hiển thị thông tin mã kho
                    System.Diagnostics.Debug.WriteLine($"DEBUG: Mã kho được chọn: '{selectedWarehouse?.Id}'");
                    System.Diagnostics.Debug.WriteLine($"DEBUG: Tên kho được chọn: '{selectedWarehouse?.TenKho}'");

                    // Chuyển đổi dữ liệu Excel thành OpeningInventoryInput với đầy đủ thông tin
                    var inputs = validItems.Select(item => new OpeningInventoryInput
                    {
                        MaKho = selectedWarehouse?.MaKho, // Mã kho từ ComboBox được chọn
                        MaVatTu = item.ItemCode,
                        SoLuong = item.Quantity,
                        SupplyId = item.SupplyId.Value, // Đã kiểm tra IsMapped nên .Value an toàn
                        TenVatTu = item.SupplyName
                    }).ToList();

                    // Debug: Hiển thị thông tin input đầu tiên
                    if (inputs.Any())
                    {
                        var firstInput = inputs.First();
                        System.Diagnostics.Debug.WriteLine($"DEBUG: Input đầu tiên - MaKho: '{firstInput.MaKho}', MaVatTu: '{firstInput.MaVatTu}', SoLuong: {firstInput.SoLuong}");
                    }

                    string nguoiNhap = "Admin"; // TODO: Lấy từ session hiện tại
                    int transactionId = importTransactionDAL.CreateOpeningInventoryTransaction(selectedWarehouse.Id, inputs, nguoiNhap);

                    lblStatus.Text = $"✅ Đã tạo transaction #{transactionId} thành công";
                    lblStatus.ForeColor = Color.Green;

                    MessageBox.Show($"Đã tạo transaction tồn đầu kỳ #{transactionId} thành công!\n\n" +
                                  $"Chi tiết:\n" +
                                  $"- Tổng dòng Excel: {currentExcelData.Count}\n" +
                                  $"- Đã mapping: {validItems.Count}\n" +
                                  $"- Transaction ID: {transactionId}", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear dữ liệu sau khi lưu thành công
                    currentExcelData.Clear();
                    dgvInput.DataSource = null;
                    txtExcelFilePath.Text = "";
                    btnLoadExcel.Enabled = false;
                    lblStatus.Text = "Sẵn sàng - Chọn file Excel...";
                    lblStatus.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi lưu: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi lưu tồn kho đầu kỳ: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
