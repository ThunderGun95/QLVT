namespace QLVT.Models
{
    /// <summary>
    /// Model cho warehouse selection
    /// </summary>
    public class Warehouse
    {
        public int Id { get; set; }
        public string MaKho { get; set; } = string.Empty;
        public string TenKho { get; set; } = string.Empty;
        public string LoaiKho { get; set; } = string.Empty;
        public string? MaNV { get; set; }
        public string? DiaChi { get; set; }
        public string? GhiChu { get; set; }
        public bool IsActive { get; set; }
    }
}
