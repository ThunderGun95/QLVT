using System;

namespace QLVT.Models
{
    /// <summary>
    /// Model cho bảng Inventory (tồn kho)
    /// </summary>
    public class Inventory
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public long SupplyErpId { get; set; }
        public decimal SoLuongTon { get; set; }
        public DateTime LastUpdated { get; set; }
        
        // Navigation properties (nếu cần)
        public string? TenKho { get; set; }
        public string? TenVatTu { get; set; }
        public string? DonViTinh { get; set; }
    }
}