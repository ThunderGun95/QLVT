using System;
using System.Collections.Generic;

namespace QLVT.Models
{
    public class BravoPhieuExcelItem
    {
        public int RowNumber { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public string NoiDungPhieu { get; set; } = string.Empty;
        public long MaVatTu { get; set; }
        public string? MaKhoXuat { get; set; }
        public string? MaKhoNhap { get; set; }
        public decimal SoLuong { get; set; }
        public string LoaiPhieu { get; set; } = string.Empty;
        
        // Validation
        public string? ValidationError { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ValidationError);
        
        // For display
        public string NgayGiaoDichDisplay => NgayGiaoDich.ToString("dd/MM/yyyy");
    }

    public class PhieuGroup
    {
        public string SoPhieu { get; set; } = string.Empty;
        public DateTime NgayGiaoDich { get; set; }
        public string NoiDungPhieu { get; set; } = string.Empty;
        public string LoaiPhieu { get; set; } = string.Empty;
        public string? MaKhoXuat { get; set; }
        public string? MaKhoNhap { get; set; }
        public List<BravoPhieuExcelItem> Items { get; set; } = new List<BravoPhieuExcelItem>();
        public int TransactionID { get; set; }
        public bool IsProcessed { get; set; }
        
        public int ItemCount => Items.Count;
        public decimal TotalQuantity => Items.Sum(x => x.SoLuong);
    }

    public class BravoImportData
    {
        public List<PhieuGroup> PhieuGroups { get; set; } = new List<PhieuGroup>();
        public List<BravoPhieuExcelItem> InvalidItems { get; set; } = new List<BravoPhieuExcelItem>();
        
        public int TotalItemsRead { get; set; }
        public int ValidItemsCount { get; set; }
        public int InvalidItemsCount { get; set; }
        public string ImportSummary { get; set; } = string.Empty;
    }

    // Models cho import nhập kho
    public class BravoNhapKhoExcelItem
    {
        public int RowNumber { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public string NoiDungPhieu { get; set; } = string.Empty;
        public long MaVatTu { get; set; }
        public string MaKho { get; set; } = string.Empty;
        public decimal SoLuong { get; set; }
        
        // Validation
        public string? ValidationError { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ValidationError);
        
        // For display
        public string NgayGiaoDichDisplay => NgayGiaoDich.ToString("dd/MM/yyyy");
    }

    public class NhapKhoPhieuGroup
    {
        public string SoPhieu { get; set; } = string.Empty;
        public DateTime NgayGiaoDich { get; set; }
        public string NoiDungPhieu { get; set; } = string.Empty;
        public string MaKho { get; set; } = string.Empty;
        public List<BravoNhapKhoExcelItem> Items { get; set; } = new List<BravoNhapKhoExcelItem>();
        public int TransactionID { get; set; }
        public bool IsProcessed { get; set; }
        
        public int ItemCount => Items.Count;
        public decimal TotalQuantity => Items.Sum(x => x.SoLuong);
    }

    public class BravoNhapKhoImportData
    {
        public List<NhapKhoPhieuGroup> PhieuGroups { get; set; } = new List<NhapKhoPhieuGroup>();
        public List<BravoNhapKhoExcelItem> InvalidItems { get; set; } = new List<BravoNhapKhoExcelItem>();
        
        public int TotalItemsRead { get; set; }
        public int ValidItemsCount { get; set; }
        public int InvalidItemsCount { get; set; }
        public string ImportSummary { get; set; } = string.Empty;
    }
}
