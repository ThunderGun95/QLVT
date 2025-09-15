using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    /// <summary>
    /// Business Logic Layer cho báo cáo tồn kho
    /// </summary>
    public class BaoCaoTonKhoBLL
    {
        private readonly BaoCaoTonKhoDAL inventoryReportDAL;

        public BaoCaoTonKhoBLL()
        {
            inventoryReportDAL = new BaoCaoTonKhoDAL();
        }

        /// <summary>
        /// Lấy báo cáo tồn kho với validation
        /// </summary>
        public (bool success, string message, List<InventoryReportItem> data, InventoryReportSummary summary) GetInventoryReport(InventoryReportFilter filter)
        {
            try
            {
                // Validation
                var validationResult = ValidateFilter(filter);
                if (!validationResult.isValid)
                {
                    return (false, validationResult.message, new List<InventoryReportItem>(), new InventoryReportSummary());
                }

                // Get report data
                var reportData = inventoryReportDAL.GetInventoryReport(filter);
                var summary = inventoryReportDAL.GetInventoryReportSummary(filter);

                if (!reportData.Any())
                {
                    return (true, "Không tìm thấy dữ liệu theo điều kiện lọc", reportData, summary);
                }

                return (true, $"Tìm thấy {reportData.Count} mặt hàng", reportData, summary);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi tạo báo cáo: {ex.Message}", new List<InventoryReportItem>(), new InventoryReportSummary());
            }
        }

        /// <summary>
        /// Xuất báo cáo ra Excel với SaveFileDialog
        /// </summary>
        public (bool success, string message, string filePath) ExportToExcel(List<InventoryReportItem> data, InventoryReportSummary summary, string fileName = null)
        {
            try
            {
                if (!data.Any())
                {
                    return (false, "Không có dữ liệu để xuất", "");
                }

                // Generate default filename if not provided
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                }

                // Show SaveFileDialog to let user choose location
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    saveFileDialog.FileName = fileName;
                    saveFileDialog.Title = "Lưu báo cáo Excel";

                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return (false, "Người dùng hủy việc lưu file", "");
                    }

                    var filePath = saveFileDialog.FileName;

                    // Create Excel workbook
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Báo cáo tồn kho");

                        // Title
                        worksheet.Cell("A1").Value = "BÁO CÁO TỒN KHO";
                        worksheet.Cell("A1").Style.Font.Bold = true;
                        worksheet.Cell("A1").Style.Font.FontSize = 16;
                        worksheet.Range("A1:G1").Merge();
                        worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        // Date
                        worksheet.Cell("A2").Value = $"Ngày báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}";
                        worksheet.Range("A2:G2").Merge();
                        worksheet.Cell("A2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        // Headers
                        var headers = new[] 
                        {
                            "STT", "Mã vật tư", "Tên vật tư", "Đơn vị tính", "Kho", 
                            "Số lượng tồn", "Nhà sản xuất"
                        };

                        for (int i = 0; i < headers.Length; i++)
                        {
                            var cell = worksheet.Cell(4, i + 1);
                            cell.Value = headers[i];
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        // Data
                        for (int i = 0; i < data.Count; i++)
                        {
                            var item = data[i];
                            var row = i + 5;

                            worksheet.Cell(row, 1).Value = item.STT;
                            worksheet.Cell(row, 2).Value = item.CodeVatTu;
                            worksheet.Cell(row, 3).Value = item.TenVatTu;
                            worksheet.Cell(row, 4).Value = item.DonViTinh;
                            worksheet.Cell(row, 5).Value = item.TenKho;
                            worksheet.Cell(row, 6).Value = item.SoLuongTon;
                            worksheet.Cell(row, 7).Value = item.NhaSanXuat;

                            // Add borders
                            for (int col = 1; col <= headers.Length; col++)
                            {
                                worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                        }

                        // Summary
                        var summaryRow = data.Count + 6;
                        worksheet.Cell(summaryRow, 1).Value = "TỔNG KẾT:";
                        worksheet.Cell(summaryRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(summaryRow + 1, 1).Value = $"Tổng số mặt hàng: {summary.TongSoMatHang}";
                        worksheet.Cell(summaryRow + 2, 1).Value = $"Tổng số lượng tồn: {summary.TongSoLuongTon:N0}";

                        // Auto-fit columns
                        worksheet.Columns().AdjustToContents();

                        // Save the workbook
                        workbook.SaveAs(filePath);
                    }

                    return (true, $"Xuất báo cáo thành công!\nFile đã được lưu tại: {filePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi xuất Excel: {ex.Message}", "");
            }
        }

        /// <summary>
        /// Lấy danh sách kho cho combobox filter
        /// </summary>
        public List<Warehouse> GetWarehousesForFilter()
        {
            try
            {
                return inventoryReportDAL.GetWarehousesForFilter();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách kho: {ex.Message}");
            }
        }

        /// <summary>
        /// Validate filter parameters
        /// </summary>
        private (bool isValid, string message) ValidateFilter(InventoryReportFilter filter)
        {
            if (filter == null)
            {
                return (false, "Filter không được null");
            }

            if (filter.AsOfDate.HasValue && filter.AsOfDate.Value > DateTime.Now.Date)
            {
                return (false, "Ngày báo cáo không được lớn hơn ngày hiện tại");
            }

            // Check if warehouse exists (if specified)
            if (filter.WarehouseId.HasValue)
            {
                try
                {
                    var warehouses = GetWarehousesForFilter();
                    if (!warehouses.Any(w => w.Id == filter.WarehouseId.Value))
                    {
                        return (false, "Kho được chọn không tồn tại hoặc không có tồn kho");
                    }
                }
                catch
                {
                    // If can't validate warehouse, continue anyway
                }
            }

            return (true, "");
        }

        /// <summary>
        /// Tạo filter mặc định
        /// </summary>
        public InventoryReportFilter GetDefaultFilter()
        {
            return new InventoryReportFilter
            {
                AsOfDate = DateTime.Now.Date,
                WarehouseId = null,
                CodeVatTu = "",
                TenVatTu = "",
                ChiHienThiCoTon = true,
                NhaSanXuat = ""
            };
        }

        /// <summary>
        /// Format summary cho hiển thị
        /// </summary>
        public string FormatSummaryText(InventoryReportSummary summary)
        {
            if (summary == null) return "";

            return $"📊 Tổng số mặt hàng: {summary.TongSoMatHang:N0} | " +
                   $"Tổng SL tồn: {summary.TongSoLuongTon:N0} | " +
                   $"Số kho có tồn: {summary.SoKhoCoTon:N0}";
        }

        /// <summary>
        /// Group data by warehouse for better display
        /// </summary>
        public Dictionary<string, List<InventoryReportItem>> GroupByWarehouse(List<InventoryReportItem> data)
        {
            return data.GroupBy(x => x.TenKho)
                      .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Tính thống kê theo kho
        /// </summary>
        public Dictionary<string, InventoryReportSummary> GetWarehouseSummaries(List<InventoryReportItem> data)
        {
            return data.GroupBy(x => x.TenKho)
                      .ToDictionary(g => g.Key, g => new InventoryReportSummary
                      {
                          TongSoMatHang = g.Count(),
                          TongSoLuongTon = g.Sum(x => x.SoLuongTon),
                          TongGiaTriTon = g.Sum(x => x.ThanhTien),
                          SoKhoCoTon = 1
                      });
        }
    }
}
