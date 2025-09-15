using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    /// <summary>
    /// Data Access Layer cho báo cáo tồn kho
    /// </summary>
    public class BaoCaoTonKhoDAL
    {
        /// <summary>
        /// Lấy báo cáo tồn kho theo điều kiện
        /// </summary>
        public List<InventoryReportItem> GetInventoryReport(InventoryReportFilter filter)
        {
            var reportItems = new List<InventoryReportItem>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    string sql = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY w.TenKho, s.Code) as STT,
                            s.Code as CodeVatTu,
                            s.TenVatTu,
                            u.TenDVT as DonViTinh,
                            w.TenKho,
                            ISNULL(i.SoLuongTon, 0) as SoLuongTon,
                            0 as DonGia,
                            0 as ThanhTien,
                            '' as NhaSanXuat,
                            '' as GhiChu,
                            i.WarehouseId,
                            s.ErpId as SupplyErpId
                        FROM Inventory i
                        INNER JOIN Warehouses w ON i.WarehouseId = w.Id
                        INNER JOIN Supplies s ON i.SupplyErpId = s.ErpId
                        LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                        WHERE 1=1";

                    var parameters = new List<SqlParameter>();

                    // Filter by warehouse
                    if (filter.WarehouseId.HasValue)
                    {
                        sql += " AND i.WarehouseId = @WarehouseId";
                        parameters.Add(new SqlParameter("@WarehouseId", filter.WarehouseId.Value));
                    }

                    // Filter by supply search (combined code and name search)
                    if (!string.IsNullOrWhiteSpace(filter.CodeVatTu) && !string.IsNullOrWhiteSpace(filter.TenVatTu) 
                        && filter.CodeVatTu.Equals(filter.TenVatTu, StringComparison.OrdinalIgnoreCase))
                    {
                        // Combined search: search in both code and name
                        sql += " AND (s.Code LIKE @SearchText OR s.TenVatTu LIKE @SearchText)";
                        parameters.Add(new SqlParameter("@SearchText", $"%{filter.CodeVatTu}%"));
                    }
                    else
                    {
                        // Separate filters
                        if (!string.IsNullOrWhiteSpace(filter.CodeVatTu))
                        {
                            sql += " AND s.Code LIKE @CodeVatTu";
                            parameters.Add(new SqlParameter("@CodeVatTu", $"%{filter.CodeVatTu}%"));
                        }

                        if (!string.IsNullOrWhiteSpace(filter.TenVatTu))
                        {
                            sql += " AND s.TenVatTu LIKE @TenVatTu";
                            parameters.Add(new SqlParameter("@TenVatTu", $"%{filter.TenVatTu}%"));
                        }
                    }

                    // Filter only items with stock
                    if (filter.ChiHienThiCoTon)
                    {
                        sql += " AND i.SoLuongTon > 0";
                    }

                    // Add date filter if needed (for future implementation)
                    if (filter.AsOfDate.HasValue)
                    {
                        // This would require transaction history to calculate stock as of specific date
                        // For now, we'll use current stock
                    }

                    sql += " ORDER BY w.TenKho, s.Code";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new InventoryReportItem
                                {
                                    STT = Convert.ToInt32(reader["STT"]),
                                    CodeVatTu = reader["CodeVatTu"]?.ToString() ?? "",
                                    TenVatTu = reader["TenVatTu"]?.ToString() ?? "",
                                    DonViTinh = reader["DonViTinh"]?.ToString() ?? "",
                                    TenKho = reader["TenKho"]?.ToString() ?? "",
                                    SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                                    DonGia = Convert.ToDecimal(reader["DonGia"]),
                                    ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                                    NhaSanXuat = reader["NhaSanXuat"]?.ToString() ?? "",
                                    GhiChu = reader["GhiChu"]?.ToString() ?? "",
                                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                                    SupplyId = Convert.ToInt32(reader["SupplyErpId"])
                                };

                                reportItems.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo báo cáo tồn kho: {ex.Message}");
            }

            return reportItems;
        }

        /// <summary>
        /// Lấy thống kê báo cáo tồn kho
        /// </summary>
        public InventoryReportSummary GetInventoryReportSummary(InventoryReportFilter filter)
        {
            var summary = new InventoryReportSummary();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    string sql = @"
                        SELECT 
                            COUNT(*) as TongSoMatHang,
                            SUM(ISNULL(i.SoLuongTon, 0)) as TongSoLuongTon,
                            0 as TongGiaTriTon,
                            COUNT(DISTINCT i.WarehouseId) as SoKhoCoTon
                        FROM Inventory i
                        INNER JOIN Warehouses w ON i.WarehouseId = w.Id
                        INNER JOIN Supplies s ON i.SupplyErpId = s.ErpId
                        LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                        WHERE 1=1";

                    var parameters = new List<SqlParameter>();

                    // Apply same filters as main report
                    if (filter.WarehouseId.HasValue)
                    {
                        sql += " AND i.WarehouseId = @WarehouseId";
                        parameters.Add(new SqlParameter("@WarehouseId", filter.WarehouseId.Value));
                    }

                    // Filter by supply search (combined code and name search)
                    if (!string.IsNullOrWhiteSpace(filter.CodeVatTu) && !string.IsNullOrWhiteSpace(filter.TenVatTu) 
                        && filter.CodeVatTu.Equals(filter.TenVatTu, StringComparison.OrdinalIgnoreCase))
                    {
                        // Combined search: search in both code and name
                        sql += " AND (s.Code LIKE @SearchText OR s.TenVatTu LIKE @SearchText)";
                        parameters.Add(new SqlParameter("@SearchText", $"%{filter.CodeVatTu}%"));
                    }
                    else
                    {
                        // Separate filters
                        if (!string.IsNullOrWhiteSpace(filter.CodeVatTu))
                        {
                            sql += " AND s.Code LIKE @CodeVatTu";
                            parameters.Add(new SqlParameter("@CodeVatTu", $"%{filter.CodeVatTu}%"));
                        }

                        if (!string.IsNullOrWhiteSpace(filter.TenVatTu))
                        {
                            sql += " AND s.TenVatTu LIKE @TenVatTu";
                            parameters.Add(new SqlParameter("@TenVatTu", $"%{filter.TenVatTu}%"));
                        }
                    }

                    if (filter.ChiHienThiCoTon)
                    {
                        sql += " AND i.SoLuongTon > 0";
                    }

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                summary.TongSoMatHang = Convert.ToInt32(reader["TongSoMatHang"] ?? 0);
                                summary.TongSoLuongTon = Convert.ToInt32(reader["TongSoLuongTon"] ?? 0);
                                summary.TongGiaTriTon = Convert.ToDecimal(reader["TongGiaTriTon"] ?? 0);
                                summary.SoKhoCoTon = Convert.ToInt32(reader["SoKhoCoTon"] ?? 0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính thống kê báo cáo tồn kho: {ex.Message}");
            }

            return summary;
        }

        /// <summary>
        /// Lấy danh sách kho để filter
        /// </summary>
        public List<Warehouse> GetWarehousesForFilter()
        {
            var warehouses = new List<Warehouse>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    string sql = @"
                        SELECT DISTINCT w.Id, w.TenKho, w.DiaChi
                        FROM Warehouses w
                        INNER JOIN Inventory i ON w.Id = i.WarehouseId
                        WHERE i.SoLuongTon > 0
                        ORDER BY w.TenKho";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            warehouses.Add(new Warehouse
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                TenKho = reader["TenKho"]?.ToString() ?? "",
                                DiaChi = reader["DiaChi"]?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách kho: {ex.Message}");
            }

            return warehouses;
        }
    }
}
