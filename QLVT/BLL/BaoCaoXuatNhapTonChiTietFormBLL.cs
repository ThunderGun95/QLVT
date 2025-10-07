using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class BaoCaoXuatNhapTonChiTietFormBLL
    {
        private readonly BaoCaoXuatNhapTonChiTietFormDAL dal;

        public BaoCaoXuatNhapTonChiTietFormBLL()
        {
            dal = new BaoCaoXuatNhapTonChiTietFormDAL();
        }

        /// <summary>
        /// Lấy báo cáo xuất nhập tồn chi tiết
        /// </summary>
        public async Task<List<BaoCaoXuatNhapTonChiTietItem>> GetBaoCaoXuatNhapTonChiTietAsync(BaoCaoXuatNhapTonChiTietFilter filter)
        {
            try
            {
                if (filter.TuNgay > filter.DenNgay)
                {
                    throw new ArgumentException("Từ ngày không được lớn hơn đến ngày!");
                }

                // Nếu có TenKho nhưng không có WarehouseId, thử tìm warehouse ID
                if (!filter.WarehouseId.HasValue && !string.IsNullOrEmpty(filter.TenKho))
                {
                    var warehouseId = await GetWarehouseIdByNameAsync(filter.TenKho);
                    if (warehouseId.HasValue)
                    {
                        filter.WarehouseId = warehouseId.Value;
                    }
                }

                var result = await dal.GetBaoCaoXuatNhapTonChiTietAsync(filter);
                
                // Thêm dòng tồn đầu kỳ vào đầu grid
                await AddTonDauKyRowAsync(result, filter);
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo xuất nhập tồn chi tiết: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xuất báo cáo ra Excel
        /// </summary>
        public async Task<bool> ExportToExcelAsync(List<BaoCaoXuatNhapTonChiTietItem> data, BaoCaoXuatNhapTonChiTietFilter filter)
        {
            try
            {
                if (data == null || !data.Any())
                {
                    throw new ArgumentException("Không có dữ liệu để xuất!");
                }

                // Tạo SaveFileDialog
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Lưu báo cáo xuất nhập tồn chi tiết",
                    FileName = $"BaoCaoXuatNhapTonChiTiet_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Báo cáo xuất nhập tồn chi tiết");

                    // Thiết lập header
                    SetupWorksheetHeader(worksheet, filter);

                    // Thiết lập cột tiêu đề
                    SetupColumnHeaders(worksheet);

                    // Ghi dữ liệu
                    WriteDataToWorksheet(worksheet, data);

                    // Định dạng worksheet
                    FormatWorksheet(worksheet, data.Count);

                    // Lưu file
                    workbook.SaveAs(saveDialog.FileName);

                    MessageBox.Show($"Đã xuất báo cáo thành công!\nFile: {saveDialog.FileName}", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Thiết lập header cho worksheet
        /// </summary>
        private void SetupWorksheetHeader(IXLWorksheet worksheet, BaoCaoXuatNhapTonChiTietFilter filter)
        {
            // Tiêu đề chính
            worksheet.Cell(1, 1).Value = "BÁO CÁO XUẤT NHẬP TỒN CHI TIẾT";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(1, 1, 1, 11).Merge(); // Giảm từ 14 xuống 11

            // Thông tin filter
            int row = 3;
            worksheet.Cell(row, 1).Value = $"Từ ngày: {filter.TuNgay:dd/MM/yyyy}";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            
            worksheet.Cell(row, 5).Value = $"Đến ngày: {filter.DenNgay:dd/MM/yyyy}";
            worksheet.Cell(row, 5).Style.Font.Bold = true;

            if (filter.WarehouseId.HasValue)
            {
                worksheet.Cell(row + 1, 1).Value = $"Kho: {filter.TenKho}";
                worksheet.Cell(row + 1, 1).Style.Font.Bold = true;
            }

            if (!string.IsNullOrEmpty(filter.MaVatTu))
            {
                worksheet.Cell(row + 1, 5).Value = $"Mã vật tư: {filter.MaVatTu}";
                worksheet.Cell(row + 1, 5).Style.Font.Bold = true;
            }

            worksheet.Cell(row + 3, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            worksheet.Cell(row + 3, 1).Style.Font.Italic = true;
        }

        /// <summary>
        /// Thiết lập tiêu đề cột
        /// </summary>
        private void SetupColumnHeaders(IXLWorksheet worksheet)
        {
            int headerRow = 8;
            var headers = new[]
            {
                "STT", "Ngày", "Loại GD", "Số phiếu", "Mã vật tư", 
                "Tên vật tư", "ĐVT", "SL nhập", "SL xuất", 
                "Tồn kho", "Ghi chú"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
        }

        /// <summary>
        /// Ghi dữ liệu vào worksheet
        /// </summary>
        private void WriteDataToWorksheet(IXLWorksheet worksheet, List<BaoCaoXuatNhapTonChiTietItem> data)
        {
            int dataStartRow = 9;
            
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                int row = dataStartRow + i;

                worksheet.Cell(row, 1).Value = item.STT;
                worksheet.Cell(row, 2).Value = item.NgayGiaoDichFormatted;
                worksheet.Cell(row, 3).Value = item.LoaiGiaoDich;
                worksheet.Cell(row, 4).Value = item.SoPhieu;
                worksheet.Cell(row, 5).Value = item.MaVatTu;
                worksheet.Cell(row, 6).Value = item.TenVatTu;
                worksheet.Cell(row, 7).Value = item.DonViTinh;
                // Bỏ cột TenKho (item.TenKho) vì đã có ở header
                worksheet.Cell(row, 8).Value = item.SoLuongNhap;
                worksheet.Cell(row, 9).Value = item.SoLuongXuat;
                worksheet.Cell(row, 10).Value = item.TonSauGD;
                worksheet.Cell(row, 11).Value = item.GhiChu;

                // Định dạng số
                worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00"; // SL nhập
                worksheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00"; // SL xuất
                worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00"; // Tồn kho
            }
        }

        /// <summary>
        /// Định dạng worksheet
        /// </summary>
        private void FormatWorksheet(IXLWorksheet worksheet, int dataCount)
        {
            int dataStartRow = 9;
            int dataEndRow = dataStartRow + dataCount - 1;

            // Định dạng border cho toàn bộ bảng dữ liệu
            var dataRange = worksheet.Range(8, 1, dataEndRow, 11); // Giảm từ 12 xuống 11 cột
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Auto fit columns
            worksheet.Columns().AdjustToContents();

            // Căn giữa cho các cột số
            for (int row = dataStartRow; row <= dataEndRow; row++)
            {
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // STT
                worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Ngày
                worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Loại GD
                worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; // SL nhập
                worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; // SL xuất
                worksheet.Cell(row, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; // Tồn sau GD
                worksheet.Cell(row, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; // Đơn giá
                worksheet.Cell(row, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; // Thành tiền
            }

            // Đặt width cho các cột
            worksheet.Column(1).Width = 5;   // STT
            worksheet.Column(2).Width = 12;  // Ngày
            worksheet.Column(3).Width = 10;  // Loại GD
            worksheet.Column(4).Width = 12;  // Số phiếu
            worksheet.Column(5).Width = 12;  // Mã vật tư
            worksheet.Column(6).Width = 25;  // Tên vật tư
            worksheet.Column(7).Width = 8;   // ĐVT
            worksheet.Column(8).Width = 15;  // Kho
            worksheet.Column(9).Width = 10;  // SL nhập
            worksheet.Column(10).Width = 10; // SL xuất
            worksheet.Column(11).Width = 12; // Tồn sau GD
            worksheet.Column(12).Width = 20; // Ghi chú
        }

        /// <summary>
        /// Lấy danh sách kho
        /// </summary>
        public async Task<List<WarehouseItem>> GetWarehousesAsync()
        {
            return await dal.GetWarehousesAsync();
        }

        /// <summary>
        /// Tìm kiếm vật tư
        /// </summary>
        public async Task<List<(string Code, string Name)>> SearchSuppliesAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword) || keyword.Length < 2)
                {
                    return new List<(string Code, string Name)>();
                }

                return await dal.SearchSuppliesAsync(keyword);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm vật tư: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Search warehouses by name
        /// </summary>
        public async Task<List<WarehouseItem>> SearchWarehousesAsync(string searchText)
        {
            return await dal.SearchWarehousesAsync(searchText);
        }

        /// <summary>
        /// Get warehouse ID by name (exact match)
        /// </summary>
        public async Task<int?> GetWarehouseIdByNameAsync(string warehouseName)
        {
            try
            {
                if (string.IsNullOrEmpty(warehouseName))
                    return null;

                var warehouses = await dal.GetWarehousesAsync();
                var warehouse = warehouses.FirstOrDefault(w => 
                    string.Equals(w.Name, warehouseName, StringComparison.OrdinalIgnoreCase));
                
                return warehouse?.Id;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Thêm dòng tồn đầu kỳ vào đầu danh sách
        /// </summary>
        private async Task AddTonDauKyRowAsync(List<BaoCaoXuatNhapTonChiTietItem> data, BaoCaoXuatNhapTonChiTietFilter filter)
        {
            if (data == null || !data.Any())
                return;

            // Lấy danh sách các cặp kho-vật tư duy nhất
            var uniquePairs = data
                .GroupBy(x => new { x.WarehouseId, x.SupplyId, x.MaVatTu, x.TenVatTu, x.DonViTinh, x.TenKho })
                .Select(g => g.Key)
                .ToList();

            var tonDauKyRows = new List<BaoCaoXuatNhapTonChiTietItem>();

            foreach (var pair in uniquePairs)
            {
                // Lấy tồn đầu kỳ từ DAL
                var tonDauKy = await dal.GetTonDauKyAsync(pair.WarehouseId, pair.SupplyId, filter.TuNgay);

                if (tonDauKy > 0) // Chỉ hiển thị nếu có tồn đầu kỳ
                {
                    var tonDauKyRow = new BaoCaoXuatNhapTonChiTietItem
                    {
                        STT = 0, // Sẽ được cập nhật lại sau
                        NgayGiaoDich = filter.TuNgay.Date,
                        LoaiGiaoDich = "Tồn đầu kỳ",
                        SoPhieu = "",
                        MaVatTu = pair.MaVatTu,
                        TenVatTu = pair.TenVatTu,
                        DonViTinh = pair.DonViTinh,
                        TenKho = pair.TenKho,
                        SoLuongNhap = 0,
                        SoLuongXuat = 0,
                        TonSauGD = tonDauKy,
                        GhiChu = "Số tồn trước kỳ báo cáo",
                        WarehouseId = pair.WarehouseId,
                        SupplyId = pair.SupplyId
                    };

                    tonDauKyRows.Add(tonDauKyRow);
                }
            }

            // Chèn các dòng tồn đầu kỳ vào đầu danh sách
            if (tonDauKyRows.Any())
            {
                data.InsertRange(0, tonDauKyRows);

                // Cập nhật lại STT cho toàn bộ danh sách
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].STT = i + 1;
                }
            }
        }
    }
}
