using System;

namespace QLVT.Models
{
    /// <summary>
    /// Model cho bảng Transaction
    /// </summary>
    public class Transaction
    {
        public int TransactionID { get; set; }
        public string LoaiGiaoDich { get; set; } = string.Empty; // 'NhapKho', 'XuatKho', 'TraKho', 'HoanUng'
        public DateTime NgayGiaoDich { get; set; }
        public string NguoiThucHien { get; set; } = string.Empty;
        public string? TenNguoiThucHien { get; set; }
        public string? EntityHoanUng { get; set; } // Lý do hoàn ứng
        public string? GhiChu { get; set; }
        public string TrangThai { get; set; } = "DangXuLy"; // 'DangXuLy', 'HoanThanh', 'DaHuy'
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }

    /// <summary>
    /// Model cho bảng TransactionDetail
    /// </summary>
    public class TransactionDetail
    {
        public int TransactionDetailID { get; set; }
        public int TransactionID { get; set; }
        public int VatTuID { get; set; }
        public string MaVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
}