using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    /// <summary>
    /// Data Access Layer cho báo cáo xuất nhập tồn - TỔNG
    /// </summary>
    public class BaoCaoXuatNhapTonTongDAL
    {
        /// <summary>
        /// Lấy báo cáo tổng xuất nhập tồn
        /// </summary>
        public async Task<List<TransactionSummaryReportItem>> GetTransactionSummaryAsync(TransactionSummaryFilter filter)
        {
            var result = new List<TransactionSummaryReportItem>();
            
            try
            {
                var sql = @"
                    WITH PeriodTransactions AS
                    -- Trong kỳ
                    (SELECT 
                        td.ErpId, w.Id as MaKho,
                        SUM(CASE WHEN td.MaKhoNhap = w.Id THEN SoLuong ELSE 0 END) AS SoLuongNhap,
                        SUM(CASE WHEN td.MaKhoXuat = w.Id THEN SoLuong ELSE 0 END) AS SoLuongXuat
                    FROM Warehouses w
                    INNER JOIN TransactionDetails td ON td.MaKhoXuat = w.Id OR td.MaKhoNhap = w.Id and td.IsDeleted = 0
                    INNER JOIN Transactions t ON t.Id = td.TransactionId AND t.IsDeleted = 0
                    WHERE t.NgayGiaoDich >= @TuNgay AND t.NgayGiaoDich <= @DenNgay
                    GROUP BY td.ErpId, w.Id),

                    BeforePeriodTransactions AS
                    -- Trước kỳ
                    (SELECT 
                        td.ErpId, w.Id as MaKho,
                        SUM(CASE WHEN td.MaKhoNhap = w.Id THEN SoLuong ELSE 0 END) AS SoLuongNhap,
                        SUM(CASE WHEN td.MaKhoXuat = w.Id THEN SoLuong ELSE 0 END) AS SoLuongXuat
                    FROM Warehouses w
                    INNER JOIN TransactionDetails td ON td.MaKhoXuat = w.Id OR td.MaKhoNhap = w.Id and td.IsDeleted = 0
                    INNER JOIN Transactions t ON t.Id = td.TransactionId AND t.IsDeleted = 0
                    WHERE t.NgayGiaoDich < @TuNgay
                    GROUP BY td.ErpId, w.Id),
                    -- Tổng hợp
                    SummaryData AS (
                    SELECT 
                        COALESCE(bpt.ErpId, pt.ErpId) as ErpId,
                        COALESCE(bpt.MaKho, pt.MaKho) as MaKho,
                        COALESCE(bpt.SoLuongNhap - bpt.SoLuongXuat, 0) as TonDauKy,
                        COALESCE(pt.SoLuongNhap, 0) as SoNhap,
                        COALESCE(pt.SoLuongXuat, 0) as SoXuat,
                        COALESCE(bpt.SoLuongNhap - bpt.SoLuongXuat, 0) + COALESCE(pt.SoLuongNhap, 0) - COALESCE(pt.SoLuongXuat, 0) as CuoiKy
                    FROM BeforePeriodTransactions bpt
                    FULL OUTER JOIN PeriodTransactions pt ON pt.ErpId = bpt.ErpId  and pt.MaKho = bpt.MaKho
                    )

                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY W.Id, Code) as STT,
                        VT.ErpId,
                        VT.Code,
                        VT.TenVatTu,
                        VT.DVT,
                        W.Id as MaKho,
                        W.TenKho,
                        S.TonDauKy,
                        S.SoNhap,
                        S.SoXuat,
                        S.CuoiKy
                    FROM SummaryData S
                    INNER JOIN ViewVatTus VT ON VT.ErpId = S.ErpId
                    INNER JOIN Warehouses W ON W.Id = S.MaKho
                    Where 1=1";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@TuNgay", filter.TuNgay.Date),
                    new SqlParameter("@DenNgay", filter.DenNgay.Date.AddDays(1).AddSeconds(-1))
                };

                // Thêm filter warehouse
                if (filter.WarehouseId.HasValue)
                {
                    sql += " AND W.Id = @WarehouseId";
                    parameters.Add(new SqlParameter("@WarehouseId", filter.WarehouseId.Value));
                }

                // Thêm filter mã vật tư
                if (!string.IsNullOrEmpty(filter.CodeVatTu))
                {
                    sql += " AND (VT.Code LIKE @CodeVatTu OR VT.TenVatTu LIKE @CodeVatTu)";
                    parameters.Add(new SqlParameter("@CodeVatTu", $"%{filter.CodeVatTu}%"));
                }

                // Thêm filter tên vật tư
                if (!string.IsNullOrEmpty(filter.TenVatTu))
                {
                    sql += " AND VT.TenVatTu LIKE @TenVatTu";
                    parameters.Add(new SqlParameter("@TenVatTu", $"%{filter.TenVatTu}%"));
                }

                sql += " ORDER BY W.Id, Code";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new TransactionSummaryReportItem
                                {
                                    STT = Convert.ToInt32(reader["STT"]),
                                    SupplyErpId = Convert.ToInt32(reader["ErpId"]),
                                    CodeVatTu = reader.GetString("Code"),
                                    TenVatTu = reader.GetString("TenVatTu"),
                                    DonViTinh = reader.IsDBNull("DVT") ? "" : reader.GetString("DVT"),
                                    WarehouseId = Convert.ToInt32(reader["MaKho"]),
                                    TenKho = reader.GetString("TenKho"),
                                    TonDauKy = reader.GetDecimal("TonDauKy"),
                                    SoNhap = reader.GetDecimal("SoNhap"),
                                    SoXuat = reader.GetDecimal("SoXuat"),
                                    TonCuoiKy = reader.GetDecimal("CuoiKy")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo tổng xuất nhập tồn: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách kho cho filter
        /// </summary>
        public async Task<List<Warehouse>> GetWarehousesAsync()
        {
            var warehouses = new List<Warehouse>();

            try
            {
                var sql = "SELECT Id, TenKho FROM Warehouses WHERE IsActive = 1 ORDER BY TenKho";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                warehouses.Add(new Warehouse
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    TenKho = reader.GetString("TenKho")
                                });
                            }
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
