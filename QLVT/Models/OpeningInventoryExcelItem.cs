using System;
using System.Collections.Generic;

namespace QLVT.Models
{
    /// <summary>
    /// Model cho dữ liệu từ Excel file tồn đầu kỳ
    /// </summary>
    public class OpeningInventoryExcelItem
    {
        public string MaKho { get; set; }
        public long MaVatTu { get; set; }
        public decimal SoLuong { get; set; }
        
        public int RowNumber { get; set; } // Để tracking lỗi
        public string ValidationError { get; set; } = string.Empty;
        public bool IsValid => string.IsNullOrEmpty(ValidationError);
    }

    /// <summary>
    /// Model cho group dữ liệu theo kho
    /// </summary>
    public class WarehouseOpeningInventory
    {
        public required string MaKho { get; set; }
        public string TenKho { get; set; } = string.Empty;
        public List<OpeningInventoryExcelItem> Items { get; set; } = new List<OpeningInventoryExcelItem>();
        public int TransactionID { get; set; } // ID của transaction được tạo
        public bool IsProcessed { get; set; } = false;
    }

    /// <summary>
    /// Model tổng hợp cho việc import tồn đầu kỳ
    /// </summary>
    public class OpeningInventoryImportData
    {
        public List<WarehouseOpeningInventory> WarehouseGroups { get; set; } = new List<WarehouseOpeningInventory>();
        public List<OpeningInventoryExcelItem> InvalidItems { get; set; } = new List<OpeningInventoryExcelItem>();
        public int TotalItemsRead { get; set; }
        public int ValidItemsCount { get; set; }
        public int InvalidItemsCount { get; set; }
        public string ImportSummary { get; set; } = string.Empty;
    }
}