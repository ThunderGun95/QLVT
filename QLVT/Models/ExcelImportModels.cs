using System.Collections.Generic;
using System.Linq;

namespace QLVT.Models
{
    /// <summary>
    /// Model cho import dữ liệu từ Excel
    /// </summary>
    public class ExcelImportItem
    {
        public int RowIndex { get; set; }
        
        // Tương thích với cả 2 naming convention
        public string MaVatTu { get; set; } = string.Empty;
        public string ItemCode 
        { 
            get => MaVatTu; 
            set => MaVatTu = value; 
        }
        
        public decimal SoLuong { get; set; }
        public int Quantity 
        { 
            get => (int)SoLuong; 
            set => SoLuong = value; 
        }
        
        public string TenVatTu { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public int? SupplyId { get; set; }
        public string SupplyName { get; set; } = string.Empty;
        
        // Field riêng cho manual mapping status
        private string _mappingStatus = string.Empty;
        public string MappingStatus 
        { 
            get => string.IsNullOrEmpty(_mappingStatus) ? (IsMapped ? "✅ Đã map" : "❌ Chưa map") : _mappingStatus;
            set => _mappingStatus = value;
        }
        
        public bool IsMapped => SupplyId.HasValue && !string.IsNullOrWhiteSpace(SupplyName);
        public string Error { get; set; } = string.Empty;
        public bool HasError => !string.IsNullOrEmpty(Error);
    }

    /// <summary>
    /// Kết quả đọc file Excel
    /// </summary>
    public class ExcelImportResult
    {
        public List<ExcelImportItem> Items { get; set; } = new();
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int ErrorRows { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool IsSuccess => !Errors.Any();
    }
}
