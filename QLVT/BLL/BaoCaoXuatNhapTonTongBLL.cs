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
    /// Business Logic Layer cho báo cáo xuất nhập tồn - TỔNG
    /// </summary>
    public class BaoCaoXuatNhapTonTongBLL
    {
        private readonly BaoCaoXuatNhapTonTongDAL _dal;

        public BaoCaoXuatNhapTonTongBLL()
        {
            _dal = new BaoCaoXuatNhapTonTongDAL();
        }

        /// <summary>
        /// Lấy báo cáo tổng xuất nhập tồn
        /// </summary>
        public async Task<List<TransactionSummaryReportItem>> GetTransactionSummaryAsync(TransactionSummaryFilter filter)
        {
            if (filter.TuNgay > filter.DenNgay)
            {
                throw new ArgumentException("Từ ngày phải nhỏ hơn hoặc bằng đến ngày");
            }

            return await _dal.GetTransactionSummaryAsync(filter);
        }

        /// <summary>
        /// Lấy danh sách kho cho filter
        /// </summary>
        public async Task<List<Warehouse>> GetWarehousesAsync()
        {
            return await _dal.GetWarehousesAsync();
        }

        /// <summary>
        /// Xuất báo cáo tổng ra file Excel
        /// </summary>
        public Task<bool> ExportToExcelAsync(List<TransactionSummaryReportItem> data, TransactionSummaryFilter filter)
        {
            try
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Lưu báo cáo xuất nhập tồn - Tổng";
                    saveFileDialog.FileName = $"BaoCaoTongXuatNhapTon_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Báo cáo tổng");

                            // Tiêu đề báo cáo
                            worksheet.Cell(1, 1).Value = "BÁO CÁO XUẤT NHẬP TỒN - TỔNG";
                            worksheet.Cell(1, 1).Style.Font.Bold = true;
                            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                            worksheet.Range(1, 1, 1, 10).Merge();
                            worksheet.Range(1, 1, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Thông tin thời gian
                            worksheet.Cell(2, 1).Value = $"Từ ngày: {filter.TuNgay:dd/MM/yyyy} - Đến ngày: {filter.DenNgay:dd/MM/yyyy}";
                            worksheet.Cell(2, 1).Style.Font.Bold = true;
                            worksheet.Range(2, 1, 2, 10).Merge();
                            worksheet.Range(2, 1, 2, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Thông tin kho
                            string tenKho = "Tất cả kho";
                            if (filter.WarehouseId.HasValue)
                            {
                                var warehouse = data.Count > 0 ? data[0].TenKho : "Không xác định";
                                tenKho = warehouse;
                            }
                            worksheet.Cell(3, 1).Value = $"Kho: {tenKho}";
                            worksheet.Cell(3, 1).Style.Font.Bold = true;
                            worksheet.Range(3, 1, 3, 10).Merge();
                            worksheet.Range(3, 1, 3, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Headers
                            int headerRow = 5;
                            worksheet.Cell(headerRow, 1).Value = "STT";
                            worksheet.Cell(headerRow, 2).Value = "Mã vật tư";
                            worksheet.Cell(headerRow, 3).Value = "Tên vật tư";
                            worksheet.Cell(headerRow, 4).Value = "Đơn vị tính";
                            worksheet.Cell(headerRow, 5).Value = "Tên kho";
                            worksheet.Cell(headerRow, 6).Value = "Tồn đầu kỳ";
                            worksheet.Cell(headerRow, 7).Value = "Số nhập";
                            worksheet.Cell(headerRow, 8).Value = "Số xuất";
                            worksheet.Cell(headerRow, 9).Value = "Tồn cuối kỳ";

                            // Header styling
                            var headerRange = worksheet.Range(headerRow, 1, headerRow, 9);
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
                                worksheet.Cell(currentRow, 2).Value = item.CodeVatTu;
                                worksheet.Cell(currentRow, 3).Value = item.TenVatTu;
                                worksheet.Cell(currentRow, 4).Value = item.DonViTinh;
                                worksheet.Cell(currentRow, 5).Value = item.TenKho;
                                worksheet.Cell(currentRow, 6).Value = item.TonDauKy;
                                worksheet.Cell(currentRow, 7).Value = item.SoNhap;
                                worksheet.Cell(currentRow, 8).Value = item.SoXuat;
                                worksheet.Cell(currentRow, 9).Value = item.TonCuoiKy;

                                currentRow++;
                            }

                            // Data styling
                            if (data.Count > 0)
                            {
                                var dataRange = worksheet.Range(headerRow + 1, 1, currentRow - 1, 9);
                                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                                
                                // Align số liệu về phải
                                var numberColumns = worksheet.Range(headerRow + 1, 6, currentRow - 1, 9);
                                numberColumns.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
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
                        return Task.FromResult(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}", 
                              "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Validate filter trước khi thực hiện truy vấn
        /// </summary>
        public string ValidateFilter(TransactionSummaryFilter filter)
        {
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
    }
}
