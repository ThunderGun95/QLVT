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
                
                // Đảm bảo result không null
                if (result == null)
                    result = new List<BaoCaoXuatNhapTonChiTietItem>();
                
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
        public bool ExportToExcel(List<BaoCaoXuatNhapTonChiTietItem> data, BaoCaoXuatNhapTonChiTietFilter filter)
        {
            try
            {
                if (data == null || !data.Any())
                {
                    throw new ArgumentException("Không có dữ liệu để xuất!");
                }

                // Tạo SaveFileDialog
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel Files|*.xlsx";
                    saveDialog.Title = "Lưu báo cáo xuất nhập tồn chi tiết";
                    saveDialog.FileName = $"BaoCaoXuatNhapTonChiTiet_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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

                        return true;
                    };
                };
            }
            catch (Exception ex)
            {
                // Không hiển thị MessageBox ở đây, để Form xử lý
                throw new Exception($"Lỗi khi xuất Excel: {ex.Message}", ex);
            }
            return false;
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
            if (data == null)
                data = new List<BaoCaoXuatNhapTonChiTietItem>();

            var tonDauKyRows = new List<BaoCaoXuatNhapTonChiTietItem>();

            // Trường hợp có dữ liệu giao dịch - lấy từ giao dịch có sẵn
            if (data.Any())
            {
                // Lấy danh sách các cặp kho-vật tư duy nhất từ giao dịch
                var uniquePairs = data
                    .GroupBy(x => new { x.WarehouseId, x.SupplyId, x.MaVatTu, x.TenVatTu, x.DonViTinh, x.TenKho })
                    .Select(g => g.Key)
                    .ToList();

                foreach (var pair in uniquePairs)
                {
                    // Lấy tồn đầu kỳ từ DAL
                    var tonDauKy = await dal.GetTonDauKyAsync(pair.WarehouseId, pair.SupplyId, filter.TuNgay);

                    // Luôn tạo dòng tồn đầu kỳ, kể cả khi = 0
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
            else
            {
                // Trường hợp không có giao dịch nào - tạo dòng tồn đầu kỳ từ filter
                await CreateTonDauKyFromFilterAsync(tonDauKyRows, filter);
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

        /// <summary>
        /// Tạo dòng tồn đầu kỳ từ filter khi không có giao dịch
        /// </summary>
        private async Task CreateTonDauKyFromFilterAsync(List<BaoCaoXuatNhapTonChiTietItem> tonDauKyRows, BaoCaoXuatNhapTonChiTietFilter filter)
        {
            try
            {
                // Lấy thông tin kho
                var warehouses = await dal.GetWarehousesAsync();
                var selectedWarehouses = warehouses.AsEnumerable();

                if (filter.WarehouseId.HasValue)
                {
                    selectedWarehouses = warehouses.Where(w => w.Id == filter.WarehouseId.Value);
                }
                else if (!string.IsNullOrEmpty(filter.TenKho))
                {
                    selectedWarehouses = warehouses.Where(w => w.Name.Contains(filter.TenKho, StringComparison.OrdinalIgnoreCase));
                }

                // Lấy thông tin vật tư
                var supplies = await dal.SearchSuppliesAsync(filter.MaVatTu ?? "");
                
                if (!string.IsNullOrEmpty(filter.MaVatTu))
                {
                    supplies = supplies.Where(s => s.Code.Contains(filter.MaVatTu, StringComparison.OrdinalIgnoreCase) || 
                                                  s.Name.Contains(filter.MaVatTu, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Nếu có cả kho và vật tư được chỉ định
                if (selectedWarehouses.Any() && supplies.Any())
                {
                    foreach (var warehouse in selectedWarehouses)
                    {
                        foreach (var supply in supplies)
                        {
                            // Tìm ErpId của vật tư
                            var erpId = await dal.GetSupplyErpIdAsync(supply.Code);
                            if (erpId.HasValue)
                            {
                                var tonDauKy = await dal.GetTonDauKyAsync(warehouse.Id, erpId.Value, filter.TuNgay);

                                var tonDauKyRow = new BaoCaoXuatNhapTonChiTietItem
                                {
                                    STT = 0,
                                    NgayGiaoDich = filter.TuNgay.Date,
                                    LoaiGiaoDich = "Tồn đầu kỳ",
                                    SoPhieu = "",
                                    MaVatTu = supply.Code,
                                    TenVatTu = supply.Name,
                                    DonViTinh = "", // Sẽ lấy từ ViewVatTus nếu cần
                                    TenKho = warehouse.Name,
                                    SoLuongNhap = 0,
                                    SoLuongXuat = 0,
                                    TonSauGD = tonDauKy,
                                    GhiChu = "Số tồn trước kỳ báo cáo",
                                    WarehouseId = warehouse.Id,
                                    SupplyId = erpId.Value
                                };

                                tonDauKyRows.Add(tonDauKyRow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw để không ảnh hưởng đến báo cáo chính
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo dòng tồn đầu kỳ từ filter: {ex.Message}");
            }
        }
    }
}
