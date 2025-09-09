using ClosedXML.Excel;
using QLVT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QLVT.Utils
{
    /// <summary>
    /// Utility class để đọc file Excel cho import tồn kho
    /// </summary>
    public static class ExcelImportHelper
    {
        /// <summary>
        /// Đọc file Excel và parse dữ liệu tồn kho
        /// </summary>
        /// <param name="filePath">Đường dẫn file Excel</param>
        /// <returns>Kết quả import</returns>
        public static ExcelImportResult ReadInventoryFromExcel(string filePath)
        {
            var result = new ExcelImportResult();

            try
            {
                if (!File.Exists(filePath))
                {
                    result.Errors.Add("File không tồn tại");
                    return result;
                }

                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                
                if (worksheet == null)
                {
                    result.Errors.Add("Không tìm thấy worksheet trong file Excel");
                    return result;
                }

                // Kiểm tra có dữ liệu không
                var lastRow = worksheet.LastRowUsed();
                if (lastRow == null)
                {
                    result.Errors.Add("File Excel rỗng");
                    return result;
                }

                var startRow = HasHeader(worksheet) ? 2 : 1; // Bỏ qua header nếu có
                var endRow = lastRow.RowNumber();
                result.TotalRows = endRow - startRow + 1;

                // Đọc từng dòng
                for (int row = startRow; row <= endRow; row++)
                {
                    var item = ParseExcelRow(worksheet, row);
                    result.Items.Add(item);

                    if (item.HasError)
                        result.ErrorRows++;
                    else
                        result.ValidRows++;
                }

                if (result.Items.Count == 0)
                {
                    result.Errors.Add("Không có dữ liệu hợp lệ trong file Excel");
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Lỗi đọc file Excel: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra có header không
        /// </summary>
        /// <param name="worksheet">Worksheet</param>
        /// <returns>True nếu có header</returns>
        private static bool HasHeader(IXLWorksheet worksheet)
        {
            try
            {
                var cell1 = worksheet.Cell(1, 1).GetString().ToLower();
                var cell2 = worksheet.Cell(1, 2).GetString().ToLower();

                // Kiểm tra header tiếng Việt
                return (cell1.Contains("mã") || cell1.Contains("code")) &&
                       (cell2.Contains("số lượng") || cell2.Contains("quantity"));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse một dòng Excel thành ExcelImportItem
        /// </summary>
        /// <param name="worksheet">Worksheet</param>
        /// <param name="rowIndex">Index dòng</param>
        /// <returns>ExcelImportItem</returns>
        private static ExcelImportItem ParseExcelRow(IXLWorksheet worksheet, int rowIndex)
        {
            var item = new ExcelImportItem();

            try
            {
                // Đọc mã vật tư (cột A)
                var maVatTuValue = worksheet.Cell(rowIndex, 1).GetString();
                if (!string.IsNullOrWhiteSpace(maVatTuValue))
                {
                    item.ItemCode = maVatTuValue.Trim();
                }

                // Đọc số lượng (cột B)  
                var soLuongValue = worksheet.Cell(rowIndex, 2).GetString();
                if (!string.IsNullOrWhiteSpace(soLuongValue))
                {
                    if (int.TryParse(soLuongValue, out int soLuong))
                    {
                        item.Quantity = soLuong;
                    }
                }

                // Validation
                if (string.IsNullOrWhiteSpace(item.ItemCode))
                {
                    // Set error status if needed
                }
                else if (item.Quantity <= 0)
                {
                    // Set error status if needed
                }
            }
            catch (Exception ex)
            {
                // Handle error
            }

            return item;
        }

        /// <summary>
        /// Tạo file Excel mẫu
        /// </summary>
        /// <param name="filePath">Đường dẫn file xuất</param>
        public static void CreateSampleExcelFile(string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Mau_Ton_Kho");

            // Header
            worksheet.Cell(1, 1).Value = "Mã vật tư";
            worksheet.Cell(1, 2).Value = "Số lượng";

            // Sample data
            worksheet.Cell(2, 1).Value = "VT001";
            worksheet.Cell(2, 2).Value = 100;
            worksheet.Cell(3, 1).Value = "VT002";
            worksheet.Cell(3, 2).Value = 50;
            worksheet.Cell(4, 1).Value = "VT003";
            worksheet.Cell(4, 2).Value = 200;

            // Format
            var headerRange = worksheet.Range(1, 1, 1, 2);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
        }

        /// <summary>
        /// Validate format file Excel
        /// </summary>
        /// <param name="filePath">Đường dẫn file</param>
        /// <returns>True nếu format đúng</returns>
        public static bool ValidateExcelFormat(string filePath)
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                
                var lastColumn = worksheet?.LastColumnUsed();
                if (lastColumn == null)
                    return false;

                // Kiểm tra có ít nhất 2 cột
                return lastColumn.ColumnNumber() >= 2;
            }
            catch
            {
                return false;
            }
        }
    }
}
