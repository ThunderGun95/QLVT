using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    /// <summary>
    /// Business Logic Layer cho báo cáo xuất nhập tồn - CHI TIẾT
    /// </summary>
    public class BaoCaoXuatNhapTonChiTietBLL
    {
        private readonly BaoCaoXuatNhapTonChiTietDAL _dal;

        public BaoCaoXuatNhapTonChiTietBLL()
        {
            _dal = new BaoCaoXuatNhapTonChiTietDAL();
        }

        /// <summary>
        /// Lấy báo cáo chi tiết xuất nhập tồn theo vật tư
        /// </summary>
        public async Task<List<TransactionDetailReportItem>> GetTransactionDetailAsync(TransactionDetailFilter filter)
        {
            if (filter.TuNgay > filter.DenNgay)
            {
                throw new ArgumentException("Từ ngày phải nhỏ hơn hoặc bằng đến ngày");
            }

            return await _dal.GetTransactionDetailAsync(filter);
        }

        /// <summary>
        /// Lấy thông tin vật tư cho tiêu đề báo cáo chi tiết
        /// </summary>
        public async Task<(string CodeVatTu, string TenVatTu, string DonViTinh, string TenKho)> GetSupplyInfoAsync(int supplyErpId, int? warehouseId = null)
        {
            return await _dal.GetSupplyInfoAsync(supplyErpId, warehouseId);
        }

        /// <summary>
        /// Xuất báo cáo chi tiết ra file Excel
        /// </summary>
        public async Task<bool> ExportToExcelAsync(List<TransactionDetailReportItem> data, TransactionDetailFilter filter, 
            string codeVatTu, string tenVatTu, string donViTinh, string tenKho)
        {
            try
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Lưu báo cáo xuất nhập tồn - Chi tiết";
                    saveFileDialog.FileName = $"BaoCaoChiTietXuatNhapTon_{codeVatTu}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Báo cáo chi tiết");

                            // Tiêu đề báo cáo
                            worksheet.Cell(1, 1).Value = "BÁO CÁO XUẤT NHẬP TỒN - CHI TIẾT";
                            worksheet.Cell(1, 1).Style.Font.Bold = true;
                            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                            worksheet.Range(1, 1, 1, 8).Merge();
                            worksheet.Range(1, 1, 1, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Thông tin vật tư
                            worksheet.Cell(2, 1).Value = $"Mã vật tư: {codeVatTu} - {tenVatTu}";
                            worksheet.Cell(2, 1).Style.Font.Bold = true;
                            worksheet.Range(2, 1, 2, 8).Merge();
                            worksheet.Range(2, 1, 2, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Thông tin thời gian và kho
                            worksheet.Cell(3, 1).Value = $"Từ ngày: {filter.TuNgay:dd/MM/yyyy} - Đến ngày: {filter.DenNgay:dd/MM/yyyy} | Kho: {tenKho}";
                            worksheet.Cell(3, 1).Style.Font.Bold = true;
                            worksheet.Range(3, 1, 3, 8).Merge();
                            worksheet.Range(3, 1, 3, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            worksheet.Cell(4, 1).Value = $"Đơn vị tính: {donViTinh}";
                            worksheet.Cell(4, 1).Style.Font.Bold = true;
                            worksheet.Range(4, 1, 4, 8).Merge();
                            worksheet.Range(4, 1, 4, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Headers
                            int headerRow = 6;
                            worksheet.Cell(headerRow, 1).Value = "STT";
                            worksheet.Cell(headerRow, 2).Value = "Ngày giao dịch";
                            worksheet.Cell(headerRow, 3).Value = "Loại giao dịch";
                            worksheet.Cell(headerRow, 4).Value = "Số phiếu";
                            worksheet.Cell(headerRow, 5).Value = "Số lượng nhập";
                            worksheet.Cell(headerRow, 6).Value = "Số lượng xuất";
                            worksheet.Cell(headerRow, 7).Value = "Tồn sau GD";
                            worksheet.Cell(headerRow, 8).Value = "Ghi chú";

                            // Header styling
                            var headerRange = worksheet.Range(headerRow, 1, headerRow, 8);
                            headerRange.Style.Font.Bold = true;
                            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                            // Data
                            int currentRow = headerRow + 1;
                            foreach (var item in data)
                            {
                                worksheet.Cell(currentRow, 1).Value = item.STT;
                                worksheet.Cell(currentRow, 2).Value = item.NgayGiaoDich.ToString("dd/MM/yyyy");
                                worksheet.Cell(currentRow, 3).Value = GetLoaiGiaoDichDisplay(item.LoaiGiaoDich);
                                worksheet.Cell(currentRow, 4).Value = item.SoPhieu;
                                worksheet.Cell(currentRow, 5).Value = item.SoLuongNhap > 0 ? item.SoLuongNhap.ToString() : "";
                                worksheet.Cell(currentRow, 6).Value = item.SoLuongXuat > 0 ? item.SoLuongXuat.ToString() : "";
                                worksheet.Cell(currentRow, 7).Value = item.TonSauGD;
                                worksheet.Cell(currentRow, 8).Value = item.GhiChu;

                                currentRow++;
                            }

                            // Data styling
                            if (data.Count > 0)
                            {
                                var dataRange = worksheet.Range(headerRow + 1, 1, currentRow - 1, 8);
                                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                                
                                // Align số liệu về phải
                                var numberColumns = worksheet.Range(headerRow + 1, 5, currentRow - 1, 7);
                                numberColumns.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                // Align ngày giữa
                                var dateColumn = worksheet.Range(headerRow + 1, 2, currentRow - 1, 2);
                                dateColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }

                            // Auto-fit columns
                            worksheet.Columns().AdjustToContents();

                            // Thêm thông tin xuất file
                            worksheet.Cell(currentRow + 2, 1).Value = $"Báo cáo được tạo lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                            worksheet.Cell(currentRow + 2, 1).Style.Font.Italic = true;

                            workbook.SaveAs(saveFileDialog.FileName);
                        }

                        MessageBox.Show($"Xuất file Excel thành công!\nĐường dẫn: {saveFileDialog.FileName}", 
                                      "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}", 
                              "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        /// <summary>
        /// Chuyển đổi loại giao dịch thành text hiển thị
        /// </summary>
        private string GetLoaiGiaoDichDisplay(string loaiGiaoDich)
        {
            return loaiGiaoDich switch
            {
                "NhapKho" => "Nhập kho",
                "XuatKho" => "Xuất kho", 
                "TraKho" => "Trả kho",
                "HoanUng" => "Hoàn ứng",
                _ => loaiGiaoDich
            };
        }

        /// <summary>
        /// Validate filter trước khi thực hiện truy vấn
        /// </summary>
        public string ValidateFilter(TransactionDetailFilter filter)
        {
            if (filter.SupplyErpId <= 0)
            {
                return "Mã vật tư không hợp lệ";
            }

            if (filter.TuNgay > filter.DenNgay)
            {
                return "Từ ngày phải nhỏ hơn hoặc bằng đến ngày";
            }

            if (filter.DenNgay > DateTime.Now.Date)
            {
                return "Đến ngày không được lớn hơn ngày hiện tại";
            }

            return string.Empty;
        }

        /// <summary>
        /// Lấy báo cáo chi tiết với bộ lọc mở rộng (ngày, mã vật tư, kho)
        /// </summary>
        public async Task<List<TransactionDetailReportItem>> GetReportByFilterAsync(
            DateTime tuNgay, 
            DateTime denNgay, 
            string? maVatTu = null, 
            int? warehouseId = null)
        {
            try
            {
                // Validate ngày
                if (tuNgay > denNgay)
                {
                    throw new ArgumentException("Từ ngày phải nhỏ hơn hoặc bằng đến ngày");
                }

                if (denNgay > DateTime.Now.Date)
                {
                    throw new ArgumentException("Đến ngày không được lớn hơn ngày hiện tại");
                }

                return await _dal.GetTransactionDetailByFilterAsync(tuNgay, denNgay, maVatTu, warehouseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo chi tiết: {ex.Message}");
            }
        }

        /// <summary>
        /// Export báo cáo chi tiết ra Excel với bộ lọc mở rộng
        /// </summary>
        public async Task<bool> ExportDetailReportToExcelAsync(
            List<TransactionDetailReportItem> data, 
            string fileName,
            DateTime tuNgay, 
            DateTime denNgay, 
            string? maVatTu = null, 
            string? tenKho = null)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Báo cáo chi tiết xuất nhập tồn");

                    // Header thông tin
                    worksheet.Cell(1, 1).Value = "BÁO CÁO CHI TIẾT XUẤT NHẬP TỒN";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Range(1, 1, 1, 12).Merge();
                    worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(2, 1).Value = $"Từ ngày: {tuNgay:dd/MM/yyyy} - Đến ngày: {denNgay:dd/MM/yyyy}";
                    worksheet.Range(2, 1, 2, 12).Merge();
                    worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    if (!string.IsNullOrEmpty(maVatTu))
                    {
                        worksheet.Cell(3, 1).Value = $"Mã vật tư: {maVatTu}";
                        worksheet.Range(3, 1, 3, 12).Merge();
                        worksheet.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    if (!string.IsNullOrEmpty(tenKho))
                    {
                        worksheet.Cell(4, 1).Value = $"Kho: {tenKho}";
                        worksheet.Range(4, 1, 4, 12).Merge();
                        worksheet.Cell(4, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    // Headers cột
                    int headerRow = 6;
                    var headers = new[]
                    {
                        "STT", "Ngày", "Loại giao dịch", "Số phiếu", "Mã vật tư", "Tên vật tư", 
                        "ĐVT", "Số lượng nhập", "Số lượng xuất", "Tồn kho", "Kho", "Ghi chú"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(headerRow, i + 1).Value = headers[i];
                        worksheet.Cell(headerRow, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(headerRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(headerRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    // Data
                    int dataRow = headerRow + 1;
                    for (int i = 0; i < data.Count; i++)
                    {
                        var item = data[i];
                        
                        worksheet.Cell(dataRow, 1).Value = i + 1; // STT
                        worksheet.Cell(dataRow, 2).Value = item.NgayGiaoDich.ToString("dd/MM/yyyy");
                        worksheet.Cell(dataRow, 3).Value = item.LoaiGiaoDich;
                        worksheet.Cell(dataRow, 4).Value = item.SoPhieu;
                        worksheet.Cell(dataRow, 5).Value = item.MaVatTu;
                        worksheet.Cell(dataRow, 6).Value = item.TenVatTu;
                        worksheet.Cell(dataRow, 7).Value = item.DonViTinh;
                        worksheet.Cell(dataRow, 8).Value = item.SoLuongNhap;
                        worksheet.Cell(dataRow, 9).Value = item.SoLuongXuat;
                        worksheet.Cell(dataRow, 10).Value = item.TonKho;
                        worksheet.Cell(dataRow, 11).Value = item.TenKho;
                        worksheet.Cell(dataRow, 12).Value = item.GhiChu;

                        // Borders cho data
                        for (int col = 1; col <= 12; col++)
                        {
                            worksheet.Cell(dataRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        dataRow++;
                    }

                    // Auto fit columns
                    worksheet.ColumnsUsed().AdjustToContents();

                    // Save file
                    workbook.SaveAs(fileName);
                }

                MessageBox.Show($"Xuất Excel thành công!\nFile: {fileName}", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
