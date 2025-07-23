namespace QLVT.Models
{
    public class Supply
    {
        public string? ErpId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string? DacTinhKyThuat { get; set; }
        public string MaDVT { get; set; } = string.Empty;
        public string MaNSX { get; set; } = string.Empty;
        
        // Navigation properties để hiển thị tên
        public string? TenDVT { get; set; }
        public string? TenNSX { get; set; }
    }
}
