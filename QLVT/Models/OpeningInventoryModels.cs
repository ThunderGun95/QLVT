namespace QLVT.Models
{
    /// <summary>
    /// Model cho nhập tồn kho đầu kỳ
    /// </summary>
    public class OpeningInventory
    {
        public int Id { get; set; }
        public string MaKho { get; set; } = string.Empty;
        public int SupplyId { get; set; }
        public int SoLuongTonDauKy { get; set; }
        public DateTime NgayNhapTon { get; set; }
        public string NguoiNhap { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        
        // Navigation properties
        public string? TenKho { get; set; }
        public string? CodeVatTu { get; set; }
        public string? TenVatTu { get; set; }
        public string? DonViTinh { get; set; }
    }

    /// <summary>
    /// DTO để import tồn kho từ Excel hoặc nhập thủ công
    /// </summary>
    public class OpeningInventoryInput
    {
        public string MaKho { get; set; } = "COMPANY"; // Mặc định kho công ty
        public string MaVatTu { get; set; } = string.Empty; // Code hoặc ErpId của vật tư
        public int SoLuong { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        
        // Thông tin mapping
        public int? SupplyId { get; set; }
        public string? TenVatTu { get; set; }
        public bool IsMapped => SupplyId.HasValue;
        public string MappingStatus => IsMapped ? "✅ Đã map" : "❌ Chưa map";
    }
}
