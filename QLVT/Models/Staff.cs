namespace QLVT.Models
{
    public class Staff
    {
        public int ErpIdNV { get; set; }
        public string MaNV { get; set; } = string.Empty;
        public string TenNV { get; set; } = string.Empty;
        public string MaPB { get; set; } = string.Empty;

        // Navigation properties để hiển thị tên
        public string? TenPB { get; set; }
    }
}
