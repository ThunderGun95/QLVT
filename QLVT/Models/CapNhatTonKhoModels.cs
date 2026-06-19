using System;

namespace QLVT.Models
{
    public class TonKhoRebuildItem
    {
        public int STT { get; set; }
        public long WarehouseId { get; set; }
        public long SupplyErpId { get; set; }
        public string MaKho { get; set; } = string.Empty;
        public string TenKho { get; set; } = string.Empty;
        public string MaVatTu { get; set; } = string.Empty;
        public string TenVatTu { get; set; } = string.Empty;
        public decimal SoLuongNhap { get; set; }
        public decimal SoLuongXuat { get; set; }
        public decimal TonHienTai { get; set; }
        public decimal TonTinhLai { get; set; }
        public decimal ChenhLech { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }

    public class TonKhoRebuildResult
    {
        public int RowsUpdated { get; set; }
        public int RowsInserted { get; set; }
        public int RowsZeroed { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int TotalChanged => RowsUpdated + RowsInserted + RowsZeroed;
    }
}
