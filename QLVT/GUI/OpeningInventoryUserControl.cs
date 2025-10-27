using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using QLVT.BLL;
using QLVT.DAL;
using QLVT.Models;
using QLVT.Utils;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;

namespace QLVT.GUI
{
    public partial class OpeningInventoryUserControl : UserControl
    {
        private readonly OpeningInventoryBLL openingInventoryBLL;
        private readonly TransactionDAL transactionDAL;
        private readonly WarehouseDAL warehouseDAL;
        private readonly SupplyDAL supplyDAL;
        private readonly InventoryDAL inventoryDAL;
        private OpeningInventoryImportData? currentImportData;

        public OpeningInventoryUserControl()
        {
            InitializeComponent();
            openingInventoryBLL = new OpeningInventoryBLL();
            transactionDAL = new TransactionDAL();
            warehouseDAL = new WarehouseDAL();
            supplyDAL = new SupplyDAL();
            inventoryDAL = new InventoryDAL();
            SetupDataGridView();
            LoadWarehouses();
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
                Name = "MaKho",
                HeaderText = "Mã kho",
                DataPropertyName = "MaKho",
                Width = 100
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
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValidationError",
                HeaderText = "Lỗi",
                DataPropertyName = "ValidationError",
                Width = 200
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

        private async void btnLoadExcel_Click(object sender, EventArgs e)
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

                currentImportData = await LoadOpeningInventoryFromExcel(txtExcelFilePath.Text);
                if (currentImportData != null && currentImportData.ValidItemsCount > 0)
                {
                    // Hiển thị dữ liệu trong DataGridView
                    var allItems = new List<OpeningInventoryExcelItem>();
                    foreach (var warehouse in currentImportData.WarehouseGroups)
                    {
                        allItems.AddRange(warehouse.Items);
                    }
                    allItems.AddRange(currentImportData.InvalidItems);

                    dgvInput.DataSource = null;
                    dgvInput.DataSource = allItems;
                    UpdateSTT();

                    // Cập nhật màu sắc theo trạng thái validation
                    UpdateRowColors();

                    lblStatus.Text = currentImportData.ImportSummary;
                    lblStatus.ForeColor = currentImportData.InvalidItemsCount == 0 ? Color.Green : Color.Orange;

                    var summary = $"Đã tải thành công file Excel!\n" +
                                $"Tổng số dòng đọc được: {currentImportData.TotalItemsRead}\n" +
                                $"Dòng hợp lệ: {currentImportData.ValidItemsCount}\n" +
                                $"Dòng lỗi: {currentImportData.InvalidItemsCount}\n" +
                                $"Số kho: {currentImportData.WarehouseGroups.Count}\n\n";

                    if (currentImportData.InvalidItemsCount > 0)
                    {
                        summary += "⚠️ Có dữ liệu lỗi! Vui lòng kiểm tra cột 'Lỗi' và sửa file Excel.";
                    }
                    else
                    {
                        summary += "✅ Tất cả dữ liệu hợp lệ. Có thể tiến hành lưu!";
                    }

                    MessageBox.Show(summary, "Kết quả đọc Excel", MessageBoxButtons.OK, 
                        currentImportData.InvalidItemsCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
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

        private async Task<OpeningInventoryImportData> LoadOpeningInventoryFromExcel(string filePath)
        {
            var importData = new OpeningInventoryImportData();

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

                var allItems = new List<OpeningInventoryExcelItem>();
                
                // Đọc từ dòng 2 (bỏ qua header)
                for (int row = 2; row <= range.RowCount(); row++)
                {
                    var maKhoCell = worksheet.Cell(row, 1);
                    var maVatTuCell = worksheet.Cell(row, 2);
                    var soLuongCell = worksheet.Cell(row, 3);
                    
                    var maKho = maKhoCell.GetString()?.Trim();
                    var maVatTu = maVatTuCell.GetString()?.Trim();
                    var soLuongText = soLuongCell.GetString()?.Trim();

                    var item = new OpeningInventoryExcelItem
                    {
                        MaKho = maKho!,
                        MaVatTu = long.Parse(maVatTu!),
                        RowNumber = row
                    };

                    // Validation
                    var errors = new List<string>();
                    
                    if (string.IsNullOrWhiteSpace(maKho))
                        errors.Add("Mã kho trống");
                        
                    if (string.IsNullOrWhiteSpace(maVatTu))
                        errors.Add("Mã vật tư trống");
                        
                    if (string.IsNullOrWhiteSpace(soLuongText))
                    {
                        errors.Add("Số lượng trống");
                    }
                    else if (!decimal.TryParse(soLuongText, out decimal soLuong))
                    {
                        errors.Add("Số lượng không hợp lệ");
                    }
                    else if (soLuong < 0)
                    {
                        errors.Add("Số lượng không được âm");
                    }
                    else
                    {
                        item.SoLuong = soLuong;
                    }

                    if (errors.Any())
                    {
                        item.ValidationError = string.Join("; ", errors);
                        importData.InvalidItems.Add(item);
                    }
                    else
                    {
                        allItems.Add(item);
                    }
                    
                    importData.TotalItemsRead++;
                }

                // Validate warehouses and supplies exist
                await ValidateWarehousesAndSupplies(allItems);
                
                // Group by warehouse
                var warehouseGroups = allItems.Where(x => x.IsValid)
                                            .GroupBy(x => x.MaKho)
                                            .ToList();

                foreach (var group in warehouseGroups)
                {
                    var warehouseInventory = new WarehouseOpeningInventory
                    {
                        MaKho = group.Key,
                        Items = group.ToList()
                    };
                    
                    // Get warehouse name
                    try
                    {
                        var warehouse = await warehouseDAL.GetWarehouseByCodeAsync(group.Key);
                        warehouseInventory.TenKho = warehouse?.TenKho ?? "Không tìm thấy";
                    }
                    catch
                    {
                        warehouseInventory.TenKho = "Lỗi khi tải tên kho";
                    }
                    
                    importData.WarehouseGroups.Add(warehouseInventory);
                }

                // Add validation errors to invalid items
                importData.InvalidItems.AddRange(allItems.Where(x => !x.IsValid));

                importData.ValidItemsCount = allItems.Count(x => x.IsValid);
                importData.InvalidItemsCount = importData.InvalidItems.Count;
                
                importData.ImportSummary = $"✅ Đọc được {importData.TotalItemsRead} dòng - " +
                                         $"Hợp lệ: {importData.ValidItemsCount}, Lỗi: {importData.InvalidItemsCount}, " +
                                         $"Số kho: {importData.WarehouseGroups.Count}";
            }

            return importData;
        }

        private Task ValidateWarehousesAndSupplies(List<OpeningInventoryExcelItem> items)
        {
            try
            {
                // Get all warehouses and supplies for validation
                var warehouses = warehouseDAL.GetAllWarehouses();
                var warehouseCodes = warehouses.Select(w => w.Id).ToHashSet();
                
                foreach (var item in items)
                {
                    var errors = new List<string>();
                    
                    // Validate warehouse exists
                    /*
                    if (!warehouseCodes.Contains(item.MaKho))
                    {
                        errors.Add($"Kho '{item.MaKho}' không tồn tại");
                    }
                    */
                    
                    // Validate supply exists
                    /*
                    try
                    {
                        item.MaVatTu
                        if (int.TryParse(item.MaVatTu, out int erpId))
                        {
                            var supply = await supplyDAL.GetSupplyByErpIdAsync(item.MaVatTu);
                            if (supply == null)
                            {
                                errors.Add($"Vật tư '{item.MaVatTu}' không tồn tại");
                            }
                        }
                        else
                        {
                            errors.Add($"Mã vật tư '{item.MaVatTu}' không hợp lệ (phải là số)");
                        }
                    }
                    catch
                    {
                        errors.Add($"Lỗi khi kiểm tra vật tư '{item.MaVatTu}'");
                    }
                    
                    if (errors.Any())
                    {
                        item.ValidationError = string.Join("; ", errors);
                    }
                    */
                }
            }
            catch (Exception ex)
            {
                // If validation fails, mark all items as invalid
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.ValidationError))
                    {
                        item.ValidationError = $"Lỗi validation: {ex.Message}";
                    }
                }
            }

            return Task.CompletedTask;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (currentImportData == null || !currentImportData.WarehouseGroups.Any())
                {
                    MessageBox.Show("Vui lòng tải dữ liệu từ Excel trước!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentImportData.InvalidItemsCount > 0)
                {
                    var result = MessageBox.Show(
                        $"Có {currentImportData.InvalidItemsCount} dòng dữ liệu lỗi!\n\n" +
                        $"Chỉ {currentImportData.ValidItemsCount} dòng hợp lệ sẽ được xử lý.\n\n" +
                        $"Tiếp tục lưu?",
                        "Cảnh báo dữ liệu lỗi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                // Xác nhận cuối cùng
                var summary = $"Xác nhận tạo tồn đầu kỳ?\n\n" +
                            $"Số kho sẽ xử lý: {currentImportData.WarehouseGroups.Count}\n" +
                            $"Tổng số vật tư: {currentImportData.ValidItemsCount}\n\n" +
                            $"Hệ thống sẽ:\n" +
                            $"- Tạo transaction tồn đầu kỳ cho từng kho\n" +
                            $"- Tạo chi tiết transaction cho từng vật tư\n" +
                            $"- Cập nhật inventory (tồn kho)\n\n" +
                            $"Tiếp tục?";

                var finalResult = MessageBox.Show(summary, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (finalResult == DialogResult.Yes)
                {
                    lblStatus.Text = "Đang xử lý tồn đầu kỳ...";
                    lblStatus.ForeColor = Color.Blue;
                    
                    // Disable buttons during processing
                    btnSave.Enabled = false;
                    btnLoadExcel.Enabled = false;

                    await ProcessOpeningInventoryAsync();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi lưu tồn đầu kỳ: {ex.Message}");
            }
            finally
            {
                // Re-enable buttons
                btnSave.Enabled = true;
                btnLoadExcel.Enabled = true;
            }
        }

        private async Task ProcessOpeningInventoryAsync()
        {
            try
            {
                var totalProcessed = 0;
                var successfulWarehouses = new List<string>();
                var failedWarehouses = new List<string>();

                foreach (var warehouseGroup in currentImportData!.WarehouseGroups)
                {
                    try
                    {
                        lblStatus.Text = $"Đang xử lý kho: {warehouseGroup.TenKho} ({warehouseGroup.Items.Count} vật tư)...";
                        
                        // Get warehouse ID
                        var warehouseId = await GetWarehouseIdByCode(warehouseGroup.MaKho);
                        if (warehouseId == null)
                        {
                            failedWarehouses.Add($"{warehouseGroup.MaKho} - Không tìm thấy kho");
                            continue;
                        }

                        // Create transaction using existing pattern from NhapKhoTransactionDAL
                        using (var connection = DatabaseHelper.GetConnection())
                        {
                            await connection.OpenAsync();
                            using (var dbTransaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    // Generate transaction number
                                    string soPhieu = GenerateOpeningInventoryTransactionNumber(warehouseId.Value);
                                    
                                    // Create transaction
                                    string insertTransactionSql = @"
                                        INSERT INTO Transactions 
                                        (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNhan, GhiChu, CreatedBy, CreatedDate)
                                        VALUES 
                                        (@soPhieu, @ngayGiaoDich, 'TonDauKy', @maKhoNhan, @ghiChu, @createdBy, @createdDate);
                                        SELECT SCOPE_IDENTITY();";

                                    int transactionId;
                                    using (var command = new SqlCommand(insertTransactionSql, connection, dbTransaction))
                                    {
                                        command.Parameters.AddWithValue("@soPhieu", soPhieu);
                                        command.Parameters.AddWithValue("@ngayGiaoDich", DateTime.Today);
                                        command.Parameters.AddWithValue("@maKhoNhan", warehouseId.Value);
                                        command.Parameters.AddWithValue("@ghiChu", $"Tồn đầu kỳ 31/12/2024");
                                        command.Parameters.AddWithValue("@createdBy", "SYSTEM");
                                        command.Parameters.AddWithValue("@createdDate", DateTime.Now);

                                        var result = await command.ExecuteScalarAsync();
                                        transactionId = Convert.ToInt32(result);
                                    }

                                    warehouseGroup.TransactionID = transactionId;

                                    // Create transaction details
                                    foreach (var item in warehouseGroup.Items)
                                    {

                                            var supply = await supplyDAL.GetSupplyByErpIdAsync(item.MaVatTu);
                                            if (supply != null)
                                            {
                                                // Insert transaction detail
                                                string insertDetailSql = @"
                                                    INSERT INTO TransactionDetails (TransactionID, ErpID, SoLuong, GhiChu, CreatedBy)
                                                    VALUES (@transactionId, @erpId, @soLuong, @ghiChu, 'admin')";

                                                using (var detailCommand = new SqlCommand(insertDetailSql, connection, dbTransaction))
                                                {
                                                    detailCommand.Parameters.AddWithValue("@transactionId", transactionId);
                                                    detailCommand.Parameters.AddWithValue("@erpId", supply.ErpId ?? 0);
                                                    detailCommand.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                                    detailCommand.Parameters.AddWithValue("@ghiChu", "Tồn đầu kỳ");

                                                    await detailCommand.ExecuteNonQueryAsync();
                                                }

                                                // Update inventory
                                                await UpdateInventoryForOpeningBalance(warehouseId.Value, item.MaVatTu, item.SoLuong);
                                                
                                                totalProcessed++;
                                            }
                                    }

                                    // Commit transaction
                                    dbTransaction.Commit();
                                    warehouseGroup.IsProcessed = true;
                                    successfulWarehouses.Add($"{warehouseGroup.TenKho} ({warehouseGroup.Items.Count} vật tư)");
                                }
                                catch (Exception ex)
                                {
                                    dbTransaction.Rollback();
                                    failedWarehouses.Add($"{warehouseGroup.MaKho} - Lỗi: {ex.Message}");
                                }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        failedWarehouses.Add($"{warehouseGroup.TenKho}: {ex.Message}");
                    }
                }

                // Show completion summary
                var completionMessage = $"Hoàn thành xử lý tồn đầu kỳ!\n\n" +
                                       $"Tổng số vật tư đã xử lý: {totalProcessed}\n" +
                                       $"Kho xử lý thành công: {successfulWarehouses.Count}\n" +
                                       $"Kho lỗi: {failedWarehouses.Count}\n\n";

                if (successfulWarehouses.Any())
                {
                    completionMessage += "✅ Kho thành công:\n" + string.Join("\n", successfulWarehouses) + "\n\n";
                }

                if (failedWarehouses.Any())
                {
                    completionMessage += "❌ Kho lỗi:\n" + string.Join("\n", failedWarehouses);
                }

                lblStatus.Text = failedWarehouses.Any() ? 
                    $"⚠️ Hoàn thành với lỗi: {successfulWarehouses.Count}/{currentImportData.WarehouseGroups.Count} kho" :
                    $"✅ Hoàn thành: {totalProcessed} vật tư, {successfulWarehouses.Count} kho";
                lblStatus.ForeColor = failedWarehouses.Any() ? Color.Orange : Color.Green;

                MessageBox.Show(completionMessage, "Kết quả xử lý", MessageBoxButtons.OK, 
                    failedWarehouses.Any() ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                // Clear data after successful processing
                if (!failedWarehouses.Any())
                {
                    currentImportData = null;
                    dgvInput.DataSource = null;
                    txtExcelFilePath.Text = "";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi xử lý: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                throw;
            }
        }

        private string GenerateOpeningInventoryTransactionNumber(long warehouseCode)
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var time = DateTime.Now.ToString("HHmmss");
            return $"TDK20241231-{warehouseCode}";
        }

        private async Task UpdateInventoryForOpeningBalance(long warehouseCode, long supplyCode, decimal quantity)
        {
            try
            {
                // Check if inventory record exists
                var existingInventory = await inventoryDAL.GetInventoryAsync(warehouseCode, supplyCode);
                
                if (existingInventory != null)
                {
                    // Update existing inventory
                    existingInventory.SoLuongTon = quantity;
                    existingInventory.LastUpdated = DateTime.Now;
                    await inventoryDAL.UpdateInventoryAsync(existingInventory);
                }
                else
                {
                    // Create new inventory record
                    var newInventory = new Inventory
                    {
                        WarehouseId = warehouseCode,
                        SupplyErpId = supplyCode,
                        SoLuongTon = quantity,
                        LastUpdated = DateTime.Now
                    };
                    await inventoryDAL.AddInventoryAsync(newInventory);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật inventory cho {warehouseCode}-{supplyCode}: {ex.Message}");
            }
        }

        private void UpdateRowColors()
        {
            if (dgvInput.DataSource == null) return;

            foreach (DataGridViewRow row in dgvInput.Rows)
            {
                if (row.DataBoundItem is OpeningInventoryExcelItem item)
                {
                    if (!item.IsValid)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }
        private async Task<int?> GetWarehouseIdByCode(string warehouseCode)
        {
            try
            {
                var warehouse = await warehouseDAL.GetWarehouseByCodeAsync(warehouseCode);
                return warehouse?.Id;
            }
            catch
            {
                return null;
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
