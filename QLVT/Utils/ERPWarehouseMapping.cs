using QLVT.DAL;
using QLVT.ERP.Models;
using QLVT.Models;
using System.Collections.Generic;
using System.Linq;

namespace QLVT.Utils
{
    /// <summary>
    /// Utility class để mapping warehouse từ ERP sang QLVT
    /// </summary>
    public class ERPWarehouseMapping
    {
        private static readonly HashSet<int> _specialErpIds = new() { 5103 };
        
        /// <summary>
        /// Map warehouse cho phiếu ERP dựa trên mã kho ERP và ErpId vật tư
        /// </summary>
        /// <param name="erpWarehouseCode">Mã kho từ ERP</param>
        /// <param name="chiTietList">Danh sách chi tiết vật tư</param>
        /// <returns>Mã kho trong QLVT</returns>
        public static string MapERPToQLVT(string erpWarehouseCode, List<ERP_PhieuNhapKhoChiTiet> chiTietList)
        {
            if (string.IsNullOrWhiteSpace(erpWarehouseCode))
                return "001";
            
            var cleanCode = erpWarehouseCode.Trim();
            
            // RULE MỚI: Kho ERP mã 3 - phân loại theo ErpId vật tư
            if (cleanCode == "3" && chiTietList != null && chiTietList.Any(x => x.IsMapped))
            {
                // Kiểm tra ErpId của vật tư đầu tiên đã mapping
                var firstMappedItem = chiTietList.First(x => x.IsMapped);
                if (firstMappedItem.MappedSupplyId.HasValue)
                {
                    return _specialErpIds.Contains(firstMappedItem.MappedSupplyId.Value) ? "037" : "001";
                }
            }
            
            // RULE CŨ: Các kho khác theo mapping thông thường
            return cleanCode switch
            {
                "7" => "037",
                "34" => "037", 
                "6" => "036",
                "45" => "011", // Kho 45 map sang kho 011
                _ => "001" // Default cho các kho khác
            };
        }

        /// <summary>
        /// Map warehouse cho phiếu xuất ERP dựa trên mã kho ERP và ErpId vật tư
        /// </summary>
        /// <param name="erpWarehouseCode">Mã kho từ ERP</param>
        /// <param name="chiTietList">Danh sách chi tiết vật tư xuất kho</param>
        /// <returns>Mã kho trong QLVT</returns>
        public static string MapERPToQLVT(string erpWarehouseCode, List<ERP_PhieuXuatKhoChiTiet> chiTietList)
        {
            if (string.IsNullOrWhiteSpace(erpWarehouseCode))
                return "001";
            
            var cleanCode = erpWarehouseCode.Trim();
            
            // RULE MỚI: Kho ERP mã 3 - phân loại theo ErpId vật tư
            if (cleanCode == "3" && chiTietList != null && chiTietList.Any(x => x.IsMapped))
            {
                // Kiểm tra ErpId của vật tư đầu tiên đã mapping
                var firstMappedItem = chiTietList.First(x => x.IsMapped);
                if (firstMappedItem.MappedSupplyId.HasValue)
                {
                    return _specialErpIds.Contains(firstMappedItem.MappedSupplyId.Value) ? "037" : "001";
                }
            }
            
            // RULE CŨ: Các kho khác theo mapping thông thường
            return cleanCode switch
            {
                "7" => "037",
                "34" => "037", 
                "6" => "036",
                "45" => "011", // Kho 45 map sang kho 011
                _ => "001" // Default cho các kho khác
            };
        }
        
        /// <summary>
        /// Lấy thông tin mapping để debug/log
        /// </summary>
        /// <param name="erpWarehouseCode">Mã kho ERP</param>
        /// <param name="qlvtWarehouseCode">Mã kho QLVT đã map</param>
        /// <returns>Thông tin mapping</returns>
        public static string GetMappingInfo(string erpWarehouseCode, string qlvtWarehouseCode)
        {
            return $"ERP:{erpWarehouseCode} → QLVT:{qlvtWarehouseCode}";
        }
    }
}