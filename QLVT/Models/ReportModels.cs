using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace QLVT.Models
{
    /// <summary>
    /// Model cho warehouse item
    /// </summary>
    public class WarehouseItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho báo cáo tồn kho
    /// </summary>
    public class InventoryReportItem
    {
        [DisplayName("STT")]
        public int STT { get; set; }
        
        [DisplayName("Mã vật tư")]
        public string CodeVatTu { get; set; } = string.Empty;
        
        [DisplayName("Tên vật tư")]
        public string TenVatTu { get; set; } = string.Empty;
        
        [DisplayName("Đơn vị tính")]
        public string DonViTinh { get; set; } = string.Empty;
        
        [DisplayName("Kho")]
        public string TenKho { get; set; } = string.Empty;
        
        [DisplayName("Số lượng tồn")]
        public int SoLuongTon { get; set; }
        
        [DisplayName("Đơn giá")]
        public decimal DonGia { get; set; }
        
        [DisplayName("Thành tiền")]
        public decimal ThanhTien { get; set; }
        
        [DisplayName("Nhà sản xuất")]
        public string NhaSanXuat { get; set; } = string.Empty;
        
        [DisplayName("Ghi chú")]
        public string GhiChu { get; set; } = string.Empty;
        
        // Internal properties
        public int WarehouseId { get; set; }
        public int SupplyId { get; set; }
    }

    /// <summary>
    /// Model cho báo cáo xuất nhập tồn - TỔNG
    /// </summary>
    public class TransactionSummaryReportItem
    {
        [DisplayName("STT")]
        public int STT { get; set; }
        
        [DisplayName("Mã vật tư")]
        public string CodeVatTu { get; set; } = string.Empty;
        
        [DisplayName("Tên vật tư")]
        public string TenVatTu { get; set; } = string.Empty;
        
        [DisplayName("ĐVT")]
        public string DonViTinh { get; set; } = string.Empty;
        
        [DisplayName("Kho")]
        public string TenKho { get; set; } = string.Empty;
        
        [DisplayName("Tồn đầu kỳ")]
        public decimal TonDauKy { get; set; }
        
        [DisplayName("Số nhập")]
        public decimal SoNhap { get; set; }
        
        [DisplayName("Số xuất")]
        public decimal SoXuat { get; set; }
        
        [DisplayName("Tồn cuối kỳ")]
        public decimal TonCuoiKy { get; set; }
        
        // Internal properties for detail drill-down
        public int SupplyErpId { get; set; }
        public int WarehouseId { get; set; }
    }

    /// <summary>
    /// Model cho báo cáo xuất nhập tồn - CHI TIẾT
    /// </summary>
    public class TransactionDetailReportItem
    {
        [DisplayName("STT")]
        public int STT { get; set; }
        
        [DisplayName("Ngày")]
        public DateTime NgayGiaoDich { get; set; }
        
        [DisplayName("Loại GD")]
        public string LoaiGiaoDich { get; set; } = string.Empty;
        
        [DisplayName("Số phiếu")]
        public string SoPhieu { get; set; } = string.Empty;
        
        [DisplayName("SL nhập")]
        public decimal SoLuongNhap { get; set; }
        
        [DisplayName("SL xuất")]
        public decimal SoLuongXuat { get; set; }
        
        [DisplayName("Tồn sau GD")]
        public decimal TonSauGD { get; set; }
        
        [DisplayName("Ghi chú")]
        public string GhiChu { get; set; } = string.Empty;
        
        // Thêm các properties mới cho báo cáo mở rộng
        [DisplayName("Số lượng")]
        public decimal SoLuong { get; set; }
        
        [DisplayName("Tồn kho")]
        public decimal TonKho { get; set; }
        
        [DisplayName("Mã vật tư")]
        public string MaVatTu { get; set; } = string.Empty;
        
        [DisplayName("Tên vật tư")]
        public string TenVatTu { get; set; } = string.Empty;
        
        [DisplayName("ĐVT")]
        public string DonViTinh { get; set; } = string.Empty;
        
        [DisplayName("Tên kho")]
        public string TenKho { get; set; } = string.Empty;
        
        // Display formatted date
        [DisplayName("Ngày (F)")]
        public string NgayGiaoDichFormatted => NgayGiaoDich.ToString("dd/MM/yyyy HH:mm");
    }

    /// <summary>
    /// Filter cho báo cáo xuất nhập tồn - TỔNG
    /// </summary>
    public class TransactionSummaryFilter
    {
        public DateTime TuNgay { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime DenNgay { get; set; } = DateTime.Now.Date;
        public int? WarehouseId { get; set; }
        public string? CodeVatTu { get; set; }
        public string? TenVatTu { get; set; }
    }

    /// <summary>
    /// Filter cho báo cáo xuất nhập tồn - CHI TIẾT
    /// </summary>
    public class TransactionDetailFilter
    {
        public DateTime TuNgay { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime DenNgay { get; set; } = DateTime.Now.Date;
        public int SupplyErpId { get; set; }
        public int? WarehouseId { get; set; } // nullable
        public string CodeVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string TenKho { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho báo cáo xuất nhập tồn - CŨ (giữ lại để tương thích)
    /// </summary>
    public class TransactionReportItem
    {
        [DisplayName("STT")]
        public int STT { get; set; }
        
        [DisplayName("Ngày")]
        public DateTime NgayGiaoDich { get; set; }
        
        [DisplayName("Loại GD")]
        public string LoaiGiaoDich { get; set; } = string.Empty;
        
        [DisplayName("Số phiếu")]
        public string SoPhieu { get; set; } = string.Empty;
        
        [DisplayName("Mã vật tư")]
        public string CodeVatTu { get; set; } = string.Empty;
        
        [DisplayName("Tên vật tư")]
        public string TenVatTu { get; set; } = string.Empty;
        
        [DisplayName("ĐVT")]
        public string DonViTinh { get; set; } = string.Empty;
        
        [DisplayName("Kho")]
        public string TenKho { get; set; } = string.Empty;
        
        [DisplayName("SL nhập")]
        public decimal SoLuongNhap { get; set; }
        
        [DisplayName("SL xuất")]
        public decimal SoLuongXuat { get; set; }
        
        [DisplayName("Tồn cuối")]
        public decimal TonCuoi { get; set; }
        
        [DisplayName("Ghi chú")]
        public string GhiChu { get; set; } = string.Empty;
        
        // Display formatted date
        [DisplayName("Ngày (F)")]
        public string NgayGiaoDichFormatted => NgayGiaoDich.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Filter cho báo cáo tồn kho
    /// </summary>
    public class InventoryReportFilter
    {
        public DateTime? AsOfDate { get; set; } = DateTime.Now.Date;
        public int? WarehouseId { get; set; }
        public string? CodeVatTu { get; set; }
        public string? TenVatTu { get; set; }
        public bool ChiHienThiCoTon { get; set; } = true;
        public string? NhaSanXuat { get; set; }
    }

    /// <summary>
    /// Filter cho báo cáo xuất nhập tồn
    /// </summary>
    public class TransactionReportFilter
    {
        public DateTime TuNgay { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime DenNgay { get; set; } = DateTime.Now.Date;
        public int? WarehouseId { get; set; }
        public string? CodeVatTu { get; set; }
        public string? TenVatTu { get; set; }
        public string? LoaiGiaoDich { get; set; } // "Nhập", "Xuất", null (tất cả)
        public string? SoPhieu { get; set; }
    }

    /// <summary>
    /// Summary cho báo cáo tồn kho
    /// </summary>
    public class InventoryReportSummary
    {
        public int TongSoMatHang { get; set; }
        public decimal TongSoLuongTon { get; set; }
        public decimal TongGiaTriTon { get; set; }
        public int SoKhoCoTon { get; set; }
    }

    /// <summary>
    /// Summary cho báo cáo xuất nhập tồn
    /// </summary>
    public class TransactionReportSummary
    {
        public int TongSoGiaoDich { get; set; }
        public decimal TongSoLuongNhap { get; set; }
        public decimal TongSoLuongXuat { get; set; }
    }

    /// <summary>
    /// Model cho báo cáo xuất nhập tồn chi tiết (cửa sổ mới)
    /// </summary>
    public class BaoCaoXuatNhapTonChiTietItem
    {
        [DisplayName("STT")]
        public int STT { get; set; }
        
        [DisplayName("Ngày")]
        public DateTime NgayGiaoDich { get; set; }
        
        [DisplayName("Loại GD")]
        public string LoaiGiaoDich { get; set; } = string.Empty;
        
        [DisplayName("Số phiếu")]
        public string SoPhieu { get; set; } = string.Empty;
        
        [DisplayName("Mã vật tư")]
        public string MaVatTu { get; set; } = string.Empty;
        
        [DisplayName("Tên vật tư")]
        public string TenVatTu { get; set; } = string.Empty;
        
        [DisplayName("ĐVT")]
        public string DonViTinh { get; set; } = string.Empty;
        
        [DisplayName("Kho")]
        public string TenKho { get; set; } = string.Empty;
        
        [DisplayName("SL nhập")]
        public decimal SoLuongNhap { get; set; }
        
        [DisplayName("SL xuất")]
        public decimal SoLuongXuat { get; set; }
        
        [DisplayName("Tồn sau GD")]
        public decimal TonSauGD { get; set; }
        
        [DisplayName("Ghi chú")]
        public string GhiChu { get; set; } = string.Empty;
        
        // Display formatted date
        [DisplayName("Ngày (F)")]
        public string NgayGiaoDichFormatted => NgayGiaoDich.ToString("dd/MM/yyyy");
        
        // Internal properties for editing
        public int TransactionId { get; set; }
        public int TransactionDetailId { get; set; }
        public int WarehouseId { get; set; }
        public int SupplyId { get; set; }
    }

    /// <summary>
    /// Filter cho báo cáo xuất nhập tồn chi tiết (cửa sổ mới)
    /// </summary>
    public class BaoCaoXuatNhapTonChiTietFilter
    {
        public DateTime TuNgay { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime DenNgay { get; set; } = DateTime.Now.Date;
        public int? WarehouseId { get; set; }
        public string MaVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string TenKho { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho báo cáo tồn kho chi tiết
    /// </summary>
    public class BaoCaoTonKhoChiTietItem
    {
        [DisplayName("STT")]
        public int STT { get; set; }
        
        [DisplayName("Ngày")]
        public DateTime NgayGiaoDich { get; set; }
        
        [DisplayName("Loại GD")]
        public string LoaiGiaoDich { get; set; } = string.Empty;
        
        [DisplayName("Số phiếu")]
        public string SoPhieu { get; set; } = string.Empty;
        
        [DisplayName("Mã vật tư")]
        public string MaVatTu { get; set; } = string.Empty;
        
        [DisplayName("Tên vật tư")]
        public string TenVatTu { get; set; } = string.Empty;
        
        [DisplayName("ĐVT")]
        public string DonViTinh { get; set; } = string.Empty;
        
        [DisplayName("Kho")]
        public string TenKho { get; set; } = string.Empty;
        
        [DisplayName("SL nhập")]
        public decimal SoLuongNhap { get; set; }
        
        [DisplayName("SL xuất")]
        public decimal SoLuongXuat { get; set; }
        
        [DisplayName("Tồn sau GD")]
        public decimal TonSauGD { get; set; }
        
        [DisplayName("Ghi chú")]
        public string GhiChu { get; set; } = string.Empty;
        
        // Display formatted date
        [DisplayName("Ngày (F)")]
        public string NgayGiaoDichFormatted => NgayGiaoDich.ToString("dd/MM/yyyy HH:mm");
        
        // Internal properties
        public int WarehouseId { get; set; }
        public int SupplyId { get; set; }
    }

    /// <summary>
    /// Filter cho báo cáo tồn kho chi tiết
    /// </summary>
    public class BaoCaoTonKhoChiTietFilter
    {
        public DateTime TuNgay { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime DenNgay { get; set; } = DateTime.Now.Date;
        public int? WarehouseId { get; set; }
        public string MaVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string TenKho { get; set; } = string.Empty;
    }
}
