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

namespace QLVT.GUI
{
    public partial class ImportPhieuBravoUC : UserControl
    {
        private readonly WarehouseDAL warehouseDAL;
        private readonly SupplyDAL supplyDAL;
        private BravoImportData? currentImportData;

        public ImportPhieuBravoUC()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            warehouseDAL = new WarehouseDAL();
            supplyDAL = new SupplyDAL();
            SetupDataGridView();
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
                Name = "NgayGiaoDich",
                HeaderText = "Ngày GD",
                DataPropertyName = "NgayGiaoDichDisplay",
                Width = 100
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoPhieu",
                HeaderText = "Số phiếu",
                DataPropertyName = "SoPhieu",
                Width = 120
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NoiDungPhieu",
                HeaderText = "Nội dung phiếu",
                DataPropertyName = "NoiDungPhieu",
                Width = 200
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaVatTu",
                HeaderText = "Mã vật tư",
                DataPropertyName = "MaVatTu",
                Width = 100
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaKhoXuat",
                HeaderText = "Kho xuất",
                DataPropertyName = "MaKhoXuat",
                Width = 100
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaKhoNhap",
                HeaderText = "Kho nhập",
                DataPropertyName = "MaKhoNhap",
                Width = 100
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
                Name = "LoaiPhieu",
                HeaderText = "Loại phiếu",
                DataPropertyName = "LoaiPhieu",
                Width = 100
            });

            dgvInput.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValidationError",
                HeaderText = "Lỗi",
                DataPropertyName = "ValidationError",
                Width = 250
            });

            dgvInput.RowsAdded += (s, e) => UpdateSTT();
            dgvInput.RowsRemoved += (s, e) => UpdateSTT();
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

                currentImportData = await LoadBravoPhieuFromExcel(txtExcelFilePath.Text);
                if (currentImportData != null && currentImportData.TotalItemsRead > 0)
                {
                    // Hiển thị TẤT CẢ dữ liệu trong DataGridView (cả hợp lệ và lỗi)
                    var allItems = new List<BravoPhieuExcelItem>();
                    
                    // Thêm các item hợp lệ
                    foreach (var phieuGroup in currentImportData.PhieuGroups)
                    {
                        allItems.AddRange(phieuGroup.Items);
                    }
                    
                    // Thêm các item lỗi
                    allItems.AddRange(currentImportData.InvalidItems);
                    
                    // Sắp xếp theo RowNumber để giữ thứ tự như file Excel
                    allItems = allItems.OrderBy(x => x.RowNumber).ToList();

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
                                $"Số phiếu: {currentImportData.PhieuGroups.Count}\n\n";

                    if (currentImportData.InvalidItemsCount > 0)
                    {
                        summary += "⚠️ Có dữ liệu lỗi! Click OK để xem chi tiết.";
                        
                        MessageBox.Show(summary, "Kết quả đọc Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        
                        // Hiển thị chi tiết các dòng lỗi
                        ShowErrorDetails(currentImportData.InvalidItems);
                    }
                    else
                    {
                        summary += "✅ Tất cả dữ liệu hợp lệ. Có thể tiến hành import!";
                        MessageBox.Show(summary, "Kết quả đọc Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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

        private async Task<BravoImportData> LoadBravoPhieuFromExcel(string filePath)
        {
            try
            {
                var importData = new BravoImportData();

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

                    var allItems = new List<BravoPhieuExcelItem>();

                    // Đọc từ dòng 2 (bỏ qua header)
                    // Cột: NgayGiaoDich, SoPhieu, NoiDungPhieu, MaVatTu, MaKhoXuat, MaKhoNhap, SoLuong, LoaiPhieu
                    for (int row = 2; row <= range.RowCount(); row++)
                    {
                        var ngayGiaoDichCell = worksheet.Cell(row, 1);
                        var soPhieuCell = worksheet.Cell(row, 2);
                        var noiDungPhieuCell = worksheet.Cell(row, 3);
                        var maVatTuCell = worksheet.Cell(row, 4);
                        var maKhoXuatCell = worksheet.Cell(row, 5);
                        var maKhoNhapCell = worksheet.Cell(row, 6);
                        var soLuongCell = worksheet.Cell(row, 7);
                        var loaiPhieuCell = worksheet.Cell(row, 8);

                        var ngayGiaoDichText = ngayGiaoDichCell.GetString()?.Trim();
                        var soPhieu = soPhieuCell.GetString()?.Trim();
                        var noiDungPhieu = noiDungPhieuCell.GetString()?.Trim();
                        var maVatTuText = maVatTuCell.GetString()?.Trim();
                        var maKhoXuat = maKhoXuatCell.GetString()?.Trim();
                        var maKhoNhap = maKhoNhapCell.GetString()?.Trim();
                        var soLuongText = soLuongCell.GetString()?.Trim();
                        var loaiPhieu = loaiPhieuCell.GetString()?.Trim();

                        // Thêm prefix "XK-" vào số phiếu
                        var soPhieuWithPrefix = string.IsNullOrWhiteSpace(soPhieu) ? "" : "XK-" + soPhieu;

                        var item = new BravoPhieuExcelItem
                        {
                            RowNumber = row,
                            SoPhieu = soPhieuWithPrefix,
                            NoiDungPhieu = noiDungPhieu ?? "",
                            MaKhoXuat = string.IsNullOrWhiteSpace(maKhoXuat) ? null : maKhoXuat.PadLeft(3, '0'),
                            MaKhoNhap = string.IsNullOrWhiteSpace(maKhoNhap) ? null : maKhoNhap.PadLeft(3, '0'),
                            LoaiPhieu = loaiPhieu ?? ""
                        };

                        // Validation
                        var errors = new List<string>();

                        // Validate ngày giao dịch
                        if (string.IsNullOrWhiteSpace(ngayGiaoDichText))
                        {
                            errors.Add("Ngày giao dịch trống");
                        }
                        else if (!DateTime.TryParse(ngayGiaoDichText, out DateTime ngayGiaoDich))
                        {
                            errors.Add("Ngày giao dịch không hợp lệ");
                        }
                        else
                        {
                            item.NgayGiaoDich = ngayGiaoDich;
                        }

                        if (string.IsNullOrWhiteSpace(soPhieu))
                            errors.Add("Số phiếu trống");

                        if (string.IsNullOrWhiteSpace(maVatTuText))
                        {
                            errors.Add("Mã vật tư trống");
                        }
                        else if (!long.TryParse(maVatTuText, out long maVatTu))
                        {
                            errors.Add("Mã vật tư không hợp lệ");
                        }
                        else
                        {
                            item.MaVatTu = maVatTu;
                        }

                        if (string.IsNullOrWhiteSpace(soLuongText))
                        {
                            errors.Add("Số lượng trống");
                        }
                        else if (!decimal.TryParse(soLuongText, out decimal soLuong))
                        {
                            errors.Add("Số lượng không hợp lệ");
                        }
                        else if (soLuong <= 0)
                        {
                            errors.Add("Số lượng phải > 0");
                        }
                        else
                        {
                            item.SoLuong = soLuong;
                        }

                        if (string.IsNullOrWhiteSpace(loaiPhieu))
                            errors.Add("Loại phiếu trống");

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
                    
                    // Sau khi validate, một số item có thể trở nên invalid
                    // Chuyển các item invalid từ allItems sang InvalidItems
                    var newInvalidItems = allItems.Where(x => !x.IsValid).ToList();
                    foreach (var invalidItem in newInvalidItems)
                    {
                        importData.InvalidItems.Add(invalidItem);
                    }
                    
                    // Chỉ group các item còn valid
                    var validItems = allItems.Where(x => x.IsValid).ToList();

                    // Group by phiếu (SoPhieu)
                    var phieuGroups = validItems
                                             .GroupBy(x => new { x.SoPhieu, x.NgayGiaoDich, x.NoiDungPhieu, x.LoaiPhieu, x.MaKhoXuat, x.MaKhoNhap })
                                             .ToList();

                    foreach (var group in phieuGroups)
                    {
                        var phieuGroup = new PhieuGroup
                        {
                            SoPhieu = group.Key.SoPhieu,
                            NgayGiaoDich = group.Key.NgayGiaoDich,
                            NoiDungPhieu = group.Key.NoiDungPhieu,
                            LoaiPhieu = group.Key.LoaiPhieu,
                            MaKhoXuat = group.Key.MaKhoXuat,
                            MaKhoNhap = group.Key.MaKhoNhap,
                            Items = group.ToList()
                        };

                    importData.PhieuGroups.Add(phieuGroup);
                }

                // Đếm số lượng
                importData.ValidItemsCount = importData.PhieuGroups.Sum(p => p.Items.Count);
                importData.InvalidItemsCount = importData.InvalidItems.Count;
                
                importData.ImportSummary = $"✅ Đọc được {importData.TotalItemsRead} dòng - " +
                                         $"Hợp lệ: {importData.ValidItemsCount}, Lỗi: {importData.InvalidItemsCount}, " +
                                         $"Số phiếu: {importData.PhieuGroups.Count}";
            }

            return importData;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi đọc file Excel: {ex.Message}", ex);
        }
        }

        private async Task ValidateWarehousesAndSupplies(List<BravoPhieuExcelItem> items)
        {
            try
            {
                // Get all warehouses for validation
                var warehouses = warehouseDAL.GetAllWarehouses();
                var warehouseCodes = warehouses.Select(w => w.MaKho).ToHashSet();
                
                foreach (var item in items)
                {
                    var errors = new List<string>();
                    
                    // Validate kho xuất exists (if provided)
                    if (!string.IsNullOrEmpty(item.MaKhoXuat) && !warehouseCodes.Contains(item.MaKhoXuat) && item.MaKhoXuat != "000")
                    {
                        errors.Add($"Kho xuất '{item.MaKhoXuat}' không tồn tại");
                    }
                    
                    // Validate kho nhập exists (if provided)
                    if (!string.IsNullOrEmpty(item.MaKhoNhap) && !warehouseCodes.Contains(item.MaKhoNhap) && item.MaKhoNhap != "000")
                    {
                        errors.Add($"Kho nhập '{item.MaKhoNhap}' không tồn tại");
                    }
                    
                    // Validate supply exists
                    try
                    {
                        var supply = await supplyDAL.GetSupplyByErpIdAsync(item.MaVatTu);
                        if (supply == null)
                        {
                            errors.Add($"Vật tư '{item.MaVatTu}' không tồn tại");
                        }
                    }
                    catch
                    {
                        errors.Add($"Lỗi khi kiểm tra vật tư '{item.MaVatTu}'");
                    }
                    
                    if (errors.Any())
                    {
                        if (string.IsNullOrEmpty(item.ValidationError))
                        {
                            item.ValidationError = string.Join("; ", errors);
                        }
                        else
                        {
                            item.ValidationError += "; " + string.Join("; ", errors);
                        }
                    }
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
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (currentImportData == null || !currentImportData.PhieuGroups.Any())
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
                        $"Tiếp tục import?",
                        "Cảnh báo dữ liệu lỗi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                // Xác nhận cuối cùng
                var summary = $"Xác nhận import phiếu từ Bravo?\n\n" +
                            $"Số phiếu sẽ import: {currentImportData.PhieuGroups.Count}\n" +
                            $"Tổng số dòng chi tiết: {currentImportData.ValidItemsCount}\n\n" +
                            $"Tiếp tục?";

                var finalResult = MessageBox.Show(summary, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (finalResult == DialogResult.Yes)
                {
                    lblStatus.Text = "Đang import phiếu...";
                    lblStatus.ForeColor = Color.Blue;
                    
                    // Disable buttons during processing
                    btnSave.Enabled = false;
                    btnLoadExcel.Enabled = false;

                    await ProcessImportAsync();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi import phiếu: {ex.Message}");
            }
            finally
            {
                // Re-enable buttons
                btnSave.Enabled = true;
                btnLoadExcel.Enabled = true;
            }
        }

        private async Task ProcessImportAsync()
        {
            try
            {
                var currentUser = AuthenticationBLL.GetCurrentUser();
                if (currentUser == null)
                {
                    ShowError("Người dùng chưa đăng nhập!");
                    return;
                }

                // Tạo BLL
                var bravoImportBLL = new BravoImportBLL();

                // Xử lý import
                lblStatus.Text = "Đang xử lý import...";
                lblStatus.ForeColor = Color.Blue;

                await Task.Run(() =>
                {
                    var (successCount, failedCount, errors) = bravoImportBLL.ProcessImport(
                        currentImportData!, 
                        currentUser.Username);

                    // Cập nhật UI trong UI thread
                    this.Invoke((MethodInvoker)delegate
                    {
                        // Tạo báo cáo chi tiết
                        var reportLines = new List<string>();
                        reportLines.Add("KẾT QUẢ IMPORT PHIẾU TỪ BRAVO");
                        reportLines.Add("".PadRight(60, '='));
                        reportLines.Add("");
                        
                        // Tổng quan
                        reportLines.Add("📊 TỔNG QUAN:");
                        reportLines.Add($"   • Tổng số phiếu xử lý: {successCount + failedCount}");
                        reportLines.Add($"   • ✅ Thành công: {successCount} phiếu");
                        reportLines.Add($"   • ❌ Thất bại: {failedCount} phiếu");
                        reportLines.Add("");

                        // Phân loại theo loại phiếu
                        var xkCount = currentImportData!.PhieuGroups.Count(p => p.LoaiPhieu.ToUpper() == "XK" && p.IsProcessed);
                        var xnCount = currentImportData!.PhieuGroups.Count(p => p.LoaiPhieu.ToUpper() == "XN" && p.IsProcessed);
                        var huCount = currentImportData!.PhieuGroups.Count(p => p.LoaiPhieu.ToUpper() == "HU" && p.IsProcessed);

                        if (successCount > 0)
                        {
                            reportLines.Add("📋 CHI TIẾT THEO LOẠI PHIẾU:");
                            if (xkCount > 0) reportLines.Add($"   • Xuất kho (XK): {xkCount} phiếu");
                            if (xnCount > 0) reportLines.Add($"   • Trả kho (XN): {xnCount} phiếu");
                            if (huCount > 0) reportLines.Add($"   • Hoàn ứng (HU): {huCount} phiếu");
                            reportLines.Add("");
                        }

                        // Danh sách phiếu thành công
                        var successPhieus = currentImportData.PhieuGroups.Where(p => p.IsProcessed).ToList();
                        if (successPhieus.Any())
                        {
                            reportLines.Add("✅ PHIẾU THÀNH CÔNG:");
                            foreach (var phieu in successPhieus)
                            {
                                var loaiPhieuText = phieu.LoaiPhieu.ToUpper() switch
                                {
                                    "XK" => "Xuất kho",
                                    "XN" => "Trả kho",
                                    "HU" => "Hoàn ứng",
                                    _ => phieu.LoaiPhieu
                                };
                                reportLines.Add($"   • {phieu.SoPhieu} ({loaiPhieuText}) - {phieu.Items.Count} chi tiết");
                            }
                            reportLines.Add("");
                        }

                        // Chi tiết lỗi
                        if (errors.Any())
                        {
                            reportLines.Add("❌ CHI TIẾT LỖI:");
                            foreach (var error in errors)
                            {
                                reportLines.Add($"   • {error}");
                            }
                            reportLines.Add("");
                        }

                        reportLines.Add("".PadRight(60, '='));

                        var resultMessage = string.Join("\n", reportLines);

                        if (failedCount == 0)
                        {
                            lblStatus.Text = $"✅ Import thành công {successCount} phiếu!";
                            lblStatus.ForeColor = Color.Green;
                            MessageBox.Show(resultMessage, "Import Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Clear data sau khi import thành công
                            currentImportData = null;
                            dgvInput.DataSource = null;
                            txtExcelFilePath.Text = "";
                            btnSave.Enabled = false;
                        }
                        else
                        {
                            lblStatus.Text = $"⚠️ Import hoàn tất: {successCount} thành công, {failedCount} lỗi";
                            lblStatus.ForeColor = Color.Orange;
                            MessageBox.Show(resultMessage, "Import Hoàn Tất Với Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            
                            // Hiển thị popup chi tiết lỗi
                            if (errors.Any())
                            {
                                ShowImportErrorDetails(errors);
                            }
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi import phiếu:\n\n{ex.Message}");
            }
        }

        private void UpdateRowColors()
        {
            if (dgvInput.DataSource == null) return;

            foreach (DataGridViewRow row in dgvInput.Rows)
            {
                if (row.DataBoundItem is BravoPhieuExcelItem item)
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

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowErrorDetails(List<BravoPhieuExcelItem> invalidItems)
        {
            if (invalidItems == null || invalidItems.Count == 0)
                return;

            var errorDetails = "Chi tiết các dòng bị lỗi:\n\n";
            
            foreach (var invalidItem in invalidItems.OrderBy(x => x.RowNumber))
            {
                errorDetails += $"Dòng {invalidItem.RowNumber}: {invalidItem.ValidationError}\n";
            }

            MessageBox.Show(errorDetails, "Danh sách lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowImportErrorDetails(List<string> errors)
        {
            if (errors == null || errors.Count == 0)
                return;

            // Tạo form popup để hiển thị lỗi
            var errorForm = new Form
            {
                Text = "Chi Tiết Lỗi Import Phiếu",
                Width = 700,
                Height = 500,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.Sizable,
                MinimizeBox = false,
                MaximizeBox = true,
                ShowIcon = false
            };

            var txtErrors = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                BackColor = Color.White,
                ForeColor = Color.DarkRed
            };

            var errorText = "CHI TIẾT CÁC PHIẾU BỊ LỖI\n";
            errorText += "".PadRight(80, '=') + "\n\n";

            for (int i = 0; i < errors.Count; i++)
            {
                errorText += $"{i + 1}. {errors[i]}\n";
            }

            errorText += "\n" + "".PadRight(80, '=');
            errorText += $"\n\nTổng số lỗi: {errors.Count}";

            txtErrors.Text = errorText;

            var btnClose = new Button
            {
                Text = "Đóng",
                Width = 100,
                Height = 35,
                Dock = DockStyle.Bottom,
                DialogResult = DialogResult.OK
            };

            errorForm.Controls.Add(txtErrors);
            errorForm.Controls.Add(btnClose);
            errorForm.AcceptButton = btnClose;

            errorForm.ShowDialog();
        }
    }
}
