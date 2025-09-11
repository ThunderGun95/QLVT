namespace QLVT.Models
{
    /// <summary>
    /// Phiếu xuất kho từ hệ thống ERP
    /// </summary>
    public class ERPExportOrder
    {
        public int MaPhieuXuatKhoVatTu { get; set; }
        public string SoPhieuXuatKho { get; set; } = string.Empty;
        public int NAM { get; set; }
        public string TenNhanVien { get; set; } = string.Empty; // Nhân viên nhận
        public string MaNhanVien { get; set; } = string.Empty;
        public DateTime ThoiGianHoanThanhXuatKho { get; set; }
        public string MaNhanVienXuat { get; set; } = string.Empty; // Nhân viên xuất
        public string TenNhanVienXuat { get; set; } = string.Empty;
        
        // Computed property để hiển thị số phiếu đầy đủ
        public string SoPhieuDayDu => $"{SoPhieuXuatKho}-{NAM}";
        
        public List<ERPExportOrderDetail> ChiTiet { get; set; } = new();
    }

    /// <summary>
    /// Chi tiết phiếu xuất từ ERP
    /// </summary>
    public class ERPExportOrderDetail
    {
        public int MaPhieuXuatKhoVatTu { get; set; }
        public string MaVatTuHangHoa { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string DacTinhKyThuat { get; set; } = string.Empty;
        public decimal SoLuongXuatKho { get; set; }
        public string DonViTinh { get; set; } = string.Empty;
        public string MaNhaSanXuat { get; set; } = string.Empty;
        public string TenNhaSanXuat { get; set; } = string.Empty;
        
        // Thông tin kho nguồn
        public string MaKhoXuat { get; set; } = string.Empty;
        public string TenKhoXuat { get; set; } = string.Empty;
        public int? SourceWarehouseId { get; set; } // Map với hệ thống nội bộ
        
        // Alias for UI binding
        public decimal SoLuong => SoLuongXuatKho;
        public string KhoXuatDisplay => string.IsNullOrEmpty(TenKhoXuat) ? "Chưa xác định" : $"{TenKhoXuat} ({MaKhoXuat})";
        
        // Thông tin mapping với hệ thống QLVT
        public int? MappedSupplyId { get; set; }
        public string? MappedSupplyCode { get; set; }
        public string? MappedSupplyName { get; set; }
        public string? MappedUnit { get; set; }
        public bool IsMapped => MappedSupplyId.HasValue;
        public bool HasSourceWarehouse => SourceWarehouseId.HasValue;
    }
}
