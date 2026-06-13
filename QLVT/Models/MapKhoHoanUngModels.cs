using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;

namespace QLVT.Models
{
    public class MapKhoHoanUng
    {
        public MapKhoHoanUng(int maVatTu, decimal soLuongHoanUng)
        {
            MaVatTu = maVatTu;
            SoLuongHoanUng = soLuongHoanUng;
        }
        public MapKhoHoanUng(int maVatTu, decimal soLuongHoanUng, long maKhoHoanUng)
        {
            MaVatTu = maVatTu;
            SoLuongHoanUng = soLuongHoanUng;
            MaKhoHoanUng = maKhoHoanUng;
        }
        public int MaVatTu { get; set; }
        public decimal SoLuongHoanUng { get; set; }
        public long MaKhoHoanUng { get; set; }
    }
}