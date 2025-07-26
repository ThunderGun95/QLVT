namespace QLVT.Models
{
    /// <summary>
    /// Phiếu nhập kho từ hệ thống ERP
    /// </summary>
    public class ERPImportOrder
    {
        public int MaPhieuNhapKhoVatTu { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public int Nam { get; set; }
        public string TenKho { get; set; } = string.Empty;
        public string MaKhoVatTu { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public List<ERPImportOrderDetail> ChiTiet { get; set; } = new();
    }

    /// <summary>
    /// Chi tiết phiếu nhập từ ERP
    /// </summary>
    public class ERPImportOrderDetail
    {
        public int MaPhieuNhapKhoVatTu { get; set; }
        public string MaVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string DacTinhKyThuat { get; set; } = string.Empty;
        public decimal SoLuong { get; set; }
        public string DonViTinh { get; set; } = string.Empty;
        public string MaNhaSanXuat { get; set; } = string.Empty;
        public string TenNhaSanXuat { get; set; } = string.Empty;
        public string VatTuHangHoa { get; set; } = string.Empty;
        
        // Thông tin mapping với hệ thống QLVT
        public int? MappedSupplyErpId { get; set; }
        public string? MappedSupplyCode { get; set; }
        public string? MappedSupplyName { get; set; }
        public string? MappedUnit { get; set; }
        public bool IsMapped => MappedSupplyErpId.HasValue;
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
