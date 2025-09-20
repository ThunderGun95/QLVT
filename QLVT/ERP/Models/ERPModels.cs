using System;

namespace QLVT.ERP.Models
{
    /// <summary>
    /// Model cho bảng ct.DonDangKy
    /// </summary>
    public class DonDangKyModel
    {
        public int Id { get; set; }
        public string MADDK { get; set; } = "";
        public string TENKH { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string NhanVienKyThuat { get; set; } = "";
        public string MaNhanVienXayLap { get; set; } = "";
        public string NhanVienXayLap { get; set; } = "";
        public DateTime? NgayHoanThanh { get; set; }
        public DateTime? NgayHoanUng { get; set; }
        public bool DaHoanUng { get; set; } = false;
        public DateTime? ThoiGianXacNhanHoanUng { get; set; }
        public string MaNVXacNhan { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model cho bảng ct.DonDangKyCT
    /// </summary>
    public class DonDangKyCTModel
    {
        public int Id { get; set; }
        public string MADDK { get; set; } = "";
        public int MaVTErp { get; set; }
        public decimal SoLuongHoanUng { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model cho bảng ct.SuaChua
    /// </summary>
    public class SuaChuaModel
    {
        public int Id { get; set; }
        public string MADON { get; set; } = "";
        public string ViTriDiemChay { get; set; } = "";
        public string MaNhanVienXayLap { get; set; } = "";
        public string NhanVienXayLap { get; set; } = "";
        public DateTime? NgayHoanThanh { get; set; }
        public DateTime? NgayHoanUng { get; set; }
        public bool DaHoanUng { get; set; } = false;
        public DateTime? ThoiGianXacNhanHoanUng { get; set; }
        public string MaNVXacNhan { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model cho bảng ct.SuaChuaCT
    /// </summary>
    public class SuaChuaCTModel
    {
        public int Id { get; set; }
        public string MADON { get; set; } = "";
        public int MaVTErp { get; set; }
        public decimal SoLuongHoanUng { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model cho bảng ct.NghiemThuGiaoKhoan
    /// </summary>
    public class NghiemThuGiaoKhoanModel
    {
        public int Id { get; set; }
        public string SoBGK { get; set; } = "";
        public long? GiaoKhoanNghiemThuVatTuID { get; set; }
        public string NhanVienKyThuat { get; set; } = "";
        public string MaNhanVienXayLap { get; set; } = "";
        public string NhanVienXayLap { get; set; } = "";
        public string NoiDung { get; set; } = "";
        public int? SoLanNghiemThu { get; set; }
        public int? SoNghiemThu { get; set; }
        public int? NamNghiemThu { get; set; }
        public DateTime? NgayNghiemThu { get; set; }
        public string DonViThau { get; set; } = "";
        public string GoiThau { get; set; } = "";
        public decimal? TongGiaTri { get; set; }
        public string GhiChu { get; set; } = "";
        public DateTime? NgayHoanUng { get; set; }
        public bool? DaHoanUng { get; set; } = false;
        public DateTime? ThoiGianXacNhanHoanUng { get; set; }
        public string MaNVXacNhan { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model cho bảng ct.NghiemThuGiaoKhoanCT
    /// </summary>
    public class NghiemThuGiaoKhoanCTModel
    {
        public int Id { get; set; }
        public long GiaoKhoanNghiemThuVatTuID { get; set; }
        public string MaVatTu { get; set; } = "";
        public int MaVTErp { get; set; }
        public string VatTuHangHoa { get; set; } = "";
        public string TenVatTu { get; set; } = "";
        public string DonViTinh { get; set; } = "";
        public decimal? SoLuongNghiemThu { get; set; } = 0;
        public decimal? SoLuongDaHoanUng { get; set; } = 0;
        public decimal? DonGia { get; set; } = 0;
        public decimal SoLuongHoanUng { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    #region Phiếu Xuất kho
    /// Model cho phiếu xuất kho từ hệ thống ERP 
    public class ERP_PhieuXuatKho
    {
        public int MaPhieuXuatKhoVatTu { get; set; }
        public string SoPhieuXuatKho { get; set; } = "";
        public int NAM { get; set; }
        public string TenNhanVien { get; set; } = ""; // Nhân viên nhận
        public string MaNhanVien { get; set; } = "";
        public DateTime ThoiGianHoanThanhXuatKho { get; set; }
        public string MaNhanVienXuat { get; set; } = ""; // Nhân viên xuất
        public string TenNhanVienXuat { get; set; } = "";
        public string SoPhieuDayDu => $"{SoPhieuXuatKho}-{NAM}";
        public List<ERP_PhieuXuatKhoChiTiet> ChiTiet { get; set; } = new();
    }

    /// Model cho chi tiết đơn xuất ERP
    public class ERP_PhieuXuatKhoChiTiet
    {
        public int MaPhieuXuatKhoVatTu { get; set; }
        public string MaVatTuHangHoa { get; set; } = "";
        public string TenVatTu { get; set; } = "";
        public string DacTinhKyThuat { get; set; } = "";
        public decimal SoLuongXuatKho { get; set; }
        public string DonViTinh { get; set; } = "";
        public string MaNhaSanXuat { get; set; } = "";
        public string TenNhaSanXuat { get; set; } = "";
        
        // Thông tin kho nguồn
        public string MaKhoXuat { get; set; } = "";
        public string TenKhoXuat { get; set; } = "";
        public int? SourceWarehouseId { get; set; }
        
        // Alias for UI binding
        public decimal SoLuong => SoLuongXuatKho;
        public string KhoXuatDisplay => string.IsNullOrEmpty(TenKhoXuat) ? "Chưa xác định" : $"{TenKhoXuat} ({MaKhoXuat})";
        
        // Thông tin mapping với hệ thống QLVT
        public int? MappedSupplyId { get; set; }
        public string? MappedSupplyCode { get; set; }
        public string? MappedSupplyName { get; set; }
        public string? MappedUnit { get; set; }
        public bool IsMapped => MappedSupplyId.HasValue;
        public bool HasWarehouseMapping => SourceWarehouseId.HasValue;
    }
    #endregion

    #region Phiếu Nhập kho
    /// Model cho phiếu nhập kho từ hệ thống ERP 
    public class ERP_PhieuNhapKho
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

        public List<ERP_PhieuNhapKhoChiTiet> ChiTiet { get; set; } = new();
    }

    /// Chi tiết phiếu nhập từ ERP
    public class ERP_PhieuNhapKhoChiTiet
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
    #endregion

    /// <summary>
    /// Model kết quả đồng bộ dữ liệu
    /// </summary>
    public class ERPSyncResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int RecordsAffected { get; set; }
        public Exception? Exception { get; set; }
        public DateTime SyncTime { get; set; } = DateTime.Now;
    }
}