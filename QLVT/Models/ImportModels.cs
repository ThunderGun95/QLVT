namespace QLVT.Models
{
    /// <summary>
    /// Phiếu nhập kho từ hệ thống ERP
    /// </summary>
    public class ERPImportOrder
    {
        public int MaPhieuNhapKhoVatTu { get; set; }
        public string SoPhieuNhapKho { get; set; } = string.Empty;
        public int NAM { get; set; }
        public string TenKho { get; set; } = string.Empty;
        public string MaKhoVatTu { get; set; } = string.Empty;
        public DateTime ThoiGianHoanThanhNhapKho { get; set; }
        public string MaNhanVienMua { get; set; } = string.Empty;
        public string NhanVienMua { get; set; } = string.Empty;
        
        // Computed property để hiển thị số phiếu đầy đủ
        public string SoPhieuDayDu => $"{SoPhieuNhapKho}-{NAM}";
        
        public List<ERPImportOrderDetail> ChiTiet { get; set; } = new();
    }

    /// <summary>
    /// Chi tiết phiếu nhập từ ERP
    /// </summary>
    public class ERPImportOrderDetail
    {
        public int MaPhieuNhapKhoVatTu { get; set; }
        public string MaVatTuHangHoa { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string DacTinhKyThuat { get; set; } = string.Empty;
        public decimal SoLuongNhapKho { get; set; }
        public string DonViTinh { get; set; } = string.Empty;
        public string MaNhaSanXuat { get; set; } = string.Empty;
        public string TenNhaSanXuat { get; set; } = string.Empty;
        
        // Thông tin mapping với hệ thống QLVT
        public int? MappedSupplyId { get; set; }
        public string? MappedSupplyCode { get; set; }
        public string? MappedSupplyName { get; set; }
        public string? MappedUnit { get; set; }
        public bool IsMapped => MappedSupplyId.HasValue;
    }

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
        public string? TenNV { get; set; }
        public string? DiaChi { get; set; }
        public string? GhiChu { get; set; }
        public bool IsActive { get; set; }
    }
}
