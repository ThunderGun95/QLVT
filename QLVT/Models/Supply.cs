namespace QLVT.Models
{
    public class Supply
    {
        public int? ErpId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string? DacTinhKyThuat { get; set; }
        public string MaDVT { get; set; } = string.Empty;
        
        // Navigation properties để hiển thị tên
        public string? TenDVT { get; set; }
    }
}
