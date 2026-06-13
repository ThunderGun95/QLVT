using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using QLVT.Models;
using QLVT.BLL;
using QLVT.DAL;
using ClosedXML.Excel;

namespace QLVT.GUI
{
    public partial class ImportNhapKhoBravoUC : UserControl
    {
        private BravoNhapKhoImportData? currentImportData;
        private readonly SupplyDAL supplyDAL;
        private readonly WarehouseDAL warehouseDAL;

        public ImportNhapKhoBravoUC()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            supplyDAL = new SupplyDAL();
            warehouseDAL = new WarehouseDAL();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dgvInput.AutoGenerateColumns = false;
            dgvInput.AllowUserToAddRows = false;
            dgvInput.AllowUserToDeleteRows = false;
            dgvInput.ReadOnly = true;
            dgvInput.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInput.MultiSelect = false;

            // Clear existing columns
            dgvInput.Columns.Clear();

            // STT (auto-generated)
            var colSTT = new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                Width = 50,
                ReadOnly = true
            };
            dgvInput.Columns.Add(colSTT);

            // Ngày giao dịch
            var colNgayGD = new DataGridViewTextBoxColumn
            {
                Name = "NgayGiaoDich",
                HeaderText = "Ngày GD",
                DataPropertyName = "NgayGiaoDichDisplay",
                Width = 100
            };
            dgvInput.Columns.Add(colNgayGD);

            // Số phiếu
            var colSoPhieu = new DataGridViewTextBoxColumn
            {
                Name = "SoPhieu",
                HeaderText = "Số phiếu",
                DataPropertyName = "SoPhieu",
                Width = 120
            };
            dgvInput.Columns.Add(colSoPhieu);

            // Nội dung
            var colNoiDung = new DataGridViewTextBoxColumn
            {
                Name = "NoiDung",
                HeaderText = "Nội dung",
                DataPropertyName = "NoiDungPhieu",
                Width = 200
            };
            dgvInput.Columns.Add(colNoiDung);

            // Mã vật tư
            var colMaVT = new DataGridViewTextBoxColumn
            {
                Name = "MaVatTu",
                HeaderText = "Mã VT (ERP)",
                DataPropertyName = "MaVatTu",
                Width = 100
            };
            dgvInput.Columns.Add(colMaVT);

            // Mã kho
            var colMaKho = new DataGridViewTextBoxColumn
            {
                Name = "MaKho",
                HeaderText = "Mã kho",
                DataPropertyName = "MaKho",
                Width = 100
            };
            dgvInput.Columns.Add(colMaKho);

            // Số lượng
            var colSoLuong = new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            };
            dgvInput.Columns.Add(colSoLuong);

            // Lỗi
            var colLoi = new DataGridViewTextBoxColumn
            {
                Name = "ValidationError",
                HeaderText = "Lỗi",
                DataPropertyName = "ValidationError",
                Width = 300,
                DefaultCellStyle = new DataGridViewCellStyle { ForeColor = Color.Red }
            };
            dgvInput.Columns.Add(colLoi);

            // Event để tự động đánh số thứ tự
            dgvInput.RowPostPaint += (sender, e) =>
            {
                var grid = sender as DataGridView;
                var rowIdx = (e.RowIndex + 1).ToString();
                var centerFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid!.Columns["STT"].Width, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, dgvInput.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            };
        }

        private void btnBrowseExcel_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Chọn file Excel nhập kho";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                    btnLoadExcel.Enabled = true;
                }
            }
        }

        private async void btnLoadExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFilePath.Text))
                {
                    MessageBox.Show("Vui lòng chọn file Excel!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblStatus.Text = "Đang đọc file Excel...";
                lblStatus.ForeColor = Color.Blue;
                btnLoadExcel.Enabled = false;

                // Đọc file Excel
                currentImportData = await System.Threading.Tasks.Task.Run(() => LoadBravoNhapKhoFromExcel(txtFilePath.Text));

                if (currentImportData != null)
                {
                    // Hiển thị tất cả items (valid + invalid), sắp xếp theo RowNumber
                    var allItems = new List<BravoNhapKhoExcelItem>();
                    
                    // Thêm valid items
                    foreach (var group in currentImportData.PhieuGroups)
                    {
                        allItems.AddRange(group.Items);
                    }
                    
                    // Thêm invalid items
                    allItems.AddRange(currentImportData.InvalidItems);
                    
                    // Sắp xếp theo RowNumber
                    allItems = allItems.OrderBy(x => x.RowNumber).ToList();
                    
                    dgvInput.DataSource = allItems;
                    
                    // Update row colors
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

                    btnSave.Enabled = currentImportData.ValidItemsCount > 0;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ShowError($"Lỗi khi đọc file Excel:\n\n{ex.Message}");
            }
            finally
            {
                btnLoadExcel.Enabled = true;
            }
        }

        private BravoNhapKhoImportData LoadBravoNhapKhoFromExcel(string filePath)
        {
            var importData = new BravoNhapKhoImportData();
            var allItems = new List<BravoNhapKhoExcelItem>();

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
                // Cột: NgayGiaoDich, SoPhieu, NoiDungPhieu, MaVatTu, MaKho, SoLuong
                for (int row = 2; row <= range.RowCount(); row++)
                {
                    var ngayGDCell = worksheet.Cell(row, 1);
                    var soPhieuCell = worksheet.Cell(row, 2);
                    var noiDungCell = worksheet.Cell(row, 3);
                    var maVTCell = worksheet.Cell(row, 4);
                    var maKhoCell = worksheet.Cell(row, 5);
                    var soLuongCell = worksheet.Cell(row, 6);

                    var ngayGiaoDichText = ngayGDCell.GetString()?.Trim();
                    var soPhieu = soPhieuCell.GetString()?.Trim();
                    var noiDungPhieu = noiDungCell.GetString()?.Trim();
                    var maVatTuText = maVTCell.GetString()?.Trim();
                    var maKho = maKhoCell.GetString()?.Trim();
                    var soLuongText = soLuongCell.GetString()?.Trim();

                    // Thêm prefix "NK-" vào số phiếu
                    var soPhieuWithPrefix = string.IsNullOrWhiteSpace(soPhieu) ? "" : "NK-" + soPhieu;

                    var item = new BravoNhapKhoExcelItem
                    {
                        RowNumber = row,
                        SoPhieu = soPhieuWithPrefix,
                        NoiDungPhieu = noiDungPhieu ?? "",
                        MaKho = string.IsNullOrWhiteSpace(maKho) ? null : maKho.PadLeft(3, '0')
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

                    if (string.IsNullOrWhiteSpace(maKho))
                    {
                        errors.Add("Mã kho trống");
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
            }

            // Validate với database (kiểm tra vật tư và kho có tồn tại không)
            ValidateWarehousesAndSupplies(allItems);

            // Phân loại valid và invalid items (sau khi validate database)
            var newInvalidItems = allItems.Where(x => !x.IsValid).ToList();
            foreach (var invalidItem in newInvalidItems)
            {
                importData.InvalidItems.Add(invalidItem);
            }

            // Chỉ group các item còn valid
            var validItems = allItems.Where(x => x.IsValid).ToList();

            // Group valid items theo số phiếu
            var groupedByPhieu = validItems
                .GroupBy(x => new { x.SoPhieu, x.NgayGiaoDich, x.NoiDungPhieu, x.MaKho })
                .Select(g => new NhapKhoPhieuGroup
                {
                    SoPhieu = g.Key.SoPhieu,
                    NgayGiaoDich = g.Key.NgayGiaoDich,
                    NoiDungPhieu = g.Key.NoiDungPhieu,
                    MaKho = g.Key.MaKho,
                    Items = g.ToList()
                })
                .ToList();

            importData.PhieuGroups = groupedByPhieu;
            importData.ValidItemsCount = validItems.Count;
            importData.InvalidItemsCount = importData.InvalidItems.Count;
            importData.ImportSummary = $"Đọc được {importData.TotalItemsRead} dòng: {validItems.Count} hợp lệ, {importData.InvalidItems.Count} lỗi";

            return importData;
        }

        private void ValidateWarehousesAndSupplies(List<BravoNhapKhoExcelItem> items)
        {
            // Lấy danh sách tất cả supplies và warehouses
            var allSupplies = supplyDAL.GetAllSupplies();
            var allWarehouses = warehouseDAL.GetAllWarehouses();

            foreach (var item in items)
            {
                if (!item.IsValid) continue; // Bỏ qua items đã có lỗi

                var errors = new List<string>();

                // Kiểm tra vật tư tồn tại
                if (!allSupplies.Any(s => s.ErpId == item.MaVatTu))
                {
                    errors.Add($"Vật tư {item.MaVatTu} không tồn tại");
                }

                // Kiểm tra kho tồn tại
                if (!allWarehouses.Any(w => w.MaKho == item.MaKho))
                {
                    errors.Add($"Kho {item.MaKho} không tồn tại");
                }

                if (errors.Any())
                {
                    item.ValidationError = string.Join("; ", errors);
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
                var summary = $"Xác nhận import phiếu nhập kho từ Bravo?\n\n" +
                            $"Số phiếu sẽ import: {currentImportData.PhieuGroups.Count}\n" +
                            $"Tổng số dòng chi tiết: {currentImportData.ValidItemsCount}\n\n" +
                            $"Tiếp tục?";

                var finalResult = MessageBox.Show(summary, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (finalResult == DialogResult.Yes)
                {
                    lblStatus.Text = "Đang import phiếu nhập kho...";
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

        private async System.Threading.Tasks.Task ProcessImportAsync()
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
                var bravoNhapKhoBLL = new BravoNhapKhoBLL();

                // Xử lý import
                lblStatus.Text = "Đang xử lý import...";
                lblStatus.ForeColor = Color.Blue;

                await System.Threading.Tasks.Task.Run(() =>
                {
                    var (successCount, failedCount, errors) = bravoNhapKhoBLL.ProcessImport(
                        currentImportData!, 
                        currentUser.Username);

                    // Cập nhật UI trong UI thread
                    this.Invoke((MethodInvoker)delegate
                    {
                        // Hiển thị kết quả
                        var resultMessage = $"Import hoàn tất!\n\n" +
                                          $"✅ Thành công: {successCount} phiếu\n" +
                                          $"❌ Thất bại: {failedCount} phiếu\n";

                        if (errors.Any())
                        {
                            resultMessage += $"\nChi tiết lỗi:\n";
                            foreach (var error in errors)
                            {
                                resultMessage += $"- {error}\n";
                            }
                        }

                        if (failedCount == 0)
                        {
                            lblStatus.Text = $"✅ Import thành công {successCount} phiếu!";
                            lblStatus.ForeColor = Color.Green;
                            MessageBox.Show(resultMessage, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Clear data sau khi import thành công
                            currentImportData = null;
                            dgvInput.DataSource = null;
                            txtFilePath.Text = "";
                        }
                        else
                        {
                            lblStatus.Text = $"⚠️ Import hoàn tất: {successCount} thành công, {failedCount} lỗi";
                            lblStatus.ForeColor = Color.Orange;
                            MessageBox.Show(resultMessage, "Hoàn tất với lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (row.DataBoundItem is BravoNhapKhoExcelItem item)
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

        private void ShowErrorDetails(List<BravoNhapKhoExcelItem> invalidItems)
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
    }
}
