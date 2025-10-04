using System;
using System.Collections.Generic;

namespace QLVT.Models
{
    /// <summary>
    /// Model cho giao dịch xuất kho (sử dụng bảng Transactions)
    /// </summary>
    public class PhieuXuatKho
    {
        public int Id { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public DateTime NgayGiaoDich { get; set; } = DateTime.Now;
        public string LoaiGiaoDich { get; set; } = "XuatKho";
        public int MaKhoNguon { get; set; } // Kho xuất
        public int? MaKhoNhan { get; set; }  // Kho nhận
        public string MaNV { get; set; } = string.Empty; // Nhân viên thực hiện
        public string? GhiChu { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? EntityXuatKho { get; set; } // Lý do xuất
        
        public List<PhieuXuatKhoChiTiet> ChiTiet { get; set; } = new List<PhieuXuatKhoChiTiet>();
    }

    /// <summary>
    /// Model cho chi tiết phiếu xuất kho (sử dụng bảng TransactionDetails)
    /// </summary>
    public class PhieuXuatKhoChiTiet
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int ErpId { get; set; } // ID vật tư trong ERP
        public decimal SoLuong { get; set; }
        public string? GhiChu { get; set; }
        public decimal SoLuongTon { get; set; }
        public int? SourceWarehouseId { get; set; } // Kho nguồn cho phiếu xuất
        
        // Thông tin bổ sung từ join với Supplies
        public string MaVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho giao dịch nhập kho (sử dụng bảng Transactions)
    /// </summary>
    public class PhieuNhapKho
    {
        public int Id { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public DateTime NgayGiaoDich { get; set; } = DateTime.Now;
        public string LoaiGiaoDich { get; set; } = "NhapKho";
        public int? MaKhoNguon { get; set; } // null với nhập kho
        public int? MaKhoNhan { get; set; }  // Kho nhận
        public string MaNV { get; set; } = string.Empty; // Nhân viên thực hiện
        public string? GhiChu { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? EntityNhapKho { get; set; } // Phiếu ERP hoặc nguồn nhập
        
        public List<PhieuNhapKhoChiTiet> ChiTiet { get; set; } = new List<PhieuNhapKhoChiTiet>();
    }

    /// <summary>
    /// Model cho chi tiết phiếu nhập kho (sử dụng bảng TransactionDetails)
    /// </summary>
    public class PhieuNhapKhoChiTiet
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int ErpId { get; set; } // ID vật tư trong ERP
        public decimal SoLuong { get; set; }
        public string? GhiChu { get; set; }
        public int? SourceWarehouseId { get; set; } // null cho nhập kho
        
        // Thông tin bổ sung từ join với Supplies
        public string MaVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }

    /// <summary>
    /// Model cho tìm kiếm vật tư (từ bảng Supplies join với Inventory)
    /// </summary>
    public class VatTuSearchResult
    {
        public int? ErpId { get; set; } // Supplies.ErpId
        public string Code { get; set; } = string.Empty; // Supplies.Code
        public string TenVatTu { get; set; } = string.Empty; // Supplies.TenVatTu
        public string? DacTinhKyThuat { get; set; } // Supplies.DacTinhKyThuat
        public string DonViTinh { get; set; } = string.Empty; // 
        public decimal SoLuongTon { get; set; } // Inventory.SoLuongTon
        
        // Thuộc tính để hiển thị
        public string MaVatTu => Code;
    }
}