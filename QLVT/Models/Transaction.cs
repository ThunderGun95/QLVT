using System;
using System.Collections.Generic;

namespace QLVT.Models
{
    /// <summary>
    /// Model cho bảng Transaction
    /// </summary>
    public class Transaction
    {
        public int TransactionID { get; set; }
        public string SoPhieu { get; set; } = string.Empty; // Số phiếu
        public string LoaiGiaoDich { get; set; } = string.Empty; // 'NhapKho', 'XuatKho', 'TraKho', 'HoanUng'
        public DateTime NgayGiaoDich { get; set; }
        public int? MaKhoNguon { get; set; }
        public int? MaKhoNhan { get; set; }
        public string NguoiThucHien { get; set; } = string.Empty;
        public string? TenNguoiThucHien { get; set; }
        public string? EntityHoanUng { get; set; } // Lý do hoàn ứng
        public string? GhiChu { get; set; }
        public string TrangThai { get; set; } = "DangXuLy"; // 'DangXuLy', 'HoanThanh', 'DaHuy'
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public List<TransactionDetail> ChiTiet { get; set; } = new List<TransactionDetail>();
    }

    /// <summary>
    /// Model cho bảng TransactionDetail
    /// </summary>
    public class TransactionDetail
    {
        public int TransactionDetailID { get; set; }
        public int TransactionID { get; set; }
        public int ErpID { get; set; }
        public decimal SoLuong { get; set; }
        public string? GhiChu { get; set; }
    }
}