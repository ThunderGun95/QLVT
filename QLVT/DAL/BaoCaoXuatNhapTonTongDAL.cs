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
                    WITH PeriodTransactions AS (
                        -- Lấy các giao dịch trong kỳ
                        SELECT 
                            s.ErpId as SupplyErpId,
                            s.Code as CodeVatTu,
                            s.TenVatTu,
                            u.TenDVT as DonViTinh,
                            CASE 
                                WHEN t.LoaiGiaoDich = 'NhapKho' AND t.MaKhoNhan IS NOT NULL 
                                THEN w_nhan.Id
                                WHEN t.LoaiGiaoDich = 'XuatKho' AND td.SourceWarehouseId IS NOT NULL 
                                THEN td.SourceWarehouseId
                                WHEN t.LoaiGiaoDich = 'TraKho' AND t.MaKhoNhan IS NOT NULL 
                                THEN t.MaKhoNhan
                                WHEN t.LoaiGiaoDich = 'HoanUng' AND td.SourceWarehouseId IS NOT NULL 
                                THEN td.SourceWarehouseId
                                ELSE NULL
                            END as WarehouseId,
                            CASE 
                                WHEN t.LoaiGiaoDich = 'NhapKho' AND t.MaKhoNhan IS NOT NULL 
                                THEN w_nhan.TenKho
                                WHEN t.LoaiGiaoDich = 'XuatKho' AND td.SourceWarehouseId IS NOT NULL 
                                THEN w_nguon.TenKho
                                WHEN t.LoaiGiaoDich = 'TraKho' AND t.MaKhoNhan IS NOT NULL 
                                THEN w_nhan.TenKho
                                WHEN t.LoaiGiaoDich = 'HoanUng' AND td.SourceWarehouseId IS NOT NULL 
                                THEN w_nguon.TenKho
                                ELSE N'Không xác định'
                            END as TenKho,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('NhapKho', 'TraKho') THEN td.SoLuong
                                ELSE 0
                            END as SoNhap,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('XuatKho', 'HoanUng') THEN td.SoLuong
                                ELSE 0
                            END as SoXuat
                        FROM Transactions t
                        INNER JOIN TransactionDetails td ON t.Id = td.TransactionId
                        INNER JOIN Supplies s ON td.ErpId = s.ErpId
                        LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                        LEFT JOIN Warehouses w_nhan ON t.MaKhoNhan = w_nhan.Id
                        LEFT JOIN Warehouses w_nguon ON td.SourceWarehouseId = w_nguon.Id
                        WHERE t.NgayGiaoDich >= @TuNgay AND t.NgayGiaoDich <= @DenNgay
                    ),
                    BeforePeriodTransactions AS (
                        -- Lấy các giao dịch trước kỳ để tính tồn đầu
                        SELECT 
                            s.ErpId as SupplyErpId,
                            CASE 
                                WHEN t.LoaiGiaoDich = 'NhapKho' AND t.MaKhoNhan IS NOT NULL 
                                THEN t.MaKhoNhan
                                WHEN t.LoaiGiaoDich = 'XuatKho' AND td.SourceWarehouseId IS NOT NULL 
                                THEN td.SourceWarehouseId
                                WHEN t.LoaiGiaoDich = 'TraKho' AND t.MaKhoNhan IS NOT NULL 
                                THEN t.MaKhoNhan
                                WHEN t.LoaiGiaoDich = 'HoanUng' AND td.SourceWarehouseId IS NOT NULL 
                                THEN td.SourceWarehouseId
                                ELSE NULL
                            END as WarehouseId,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('NhapKho', 'TraKho') THEN td.SoLuong
                                ELSE 0
                            END as SoNhap,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('XuatKho', 'HoanUng') THEN td.SoLuong
                                ELSE 0
                            END as SoXuat
                        FROM Transactions t
                        INNER JOIN TransactionDetails td ON t.Id = td.TransactionId
                        INNER JOIN Supplies s ON td.ErpId = s.ErpId
                        WHERE t.NgayGiaoDich < @TuNgay
                    ),
                    SummaryData AS (
                        SELECT 
                            pt.SupplyErpId,
                            pt.CodeVatTu,
                            pt.TenVatTu,
                            pt.DonViTinh,
                            pt.WarehouseId,
                            pt.TenKho,
                            ISNULL(SUM(bpt.SoNhap - bpt.SoXuat), 0) as TonDauKy,
                            SUM(pt.SoNhap) as SoNhap,
                            SUM(pt.SoXuat) as SoXuat
                        FROM PeriodTransactions pt
                        LEFT JOIN BeforePeriodTransactions bpt ON pt.SupplyErpId = bpt.SupplyErpId 
                                                               AND pt.WarehouseId = bpt.WarehouseId
                        WHERE pt.WarehouseId IS NOT NULL
                        GROUP BY pt.SupplyErpId, pt.CodeVatTu, pt.TenVatTu, pt.DonViTinh, pt.WarehouseId, pt.TenKho
                    )
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY TenKho, CodeVatTu) as STT,
                        SupplyErpId,
                        CodeVatTu,
                        TenVatTu,
                        DonViTinh,
                        WarehouseId,
                        TenKho,
                        TonDauKy,
                        SoNhap,
                        SoXuat,
                        (TonDauKy + SoNhap - SoXuat) as TonCuoiKy
                    FROM SummaryData
                    WHERE 1=1";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@TuNgay", filter.TuNgay.Date),
                    new SqlParameter("@DenNgay", filter.DenNgay.Date.AddDays(1).AddSeconds(-1))
                };

                // Thêm filter warehouse
                if (filter.WarehouseId.HasValue)
                {
                    sql += " AND WarehouseId = @WarehouseId";
                    parameters.Add(new SqlParameter("@WarehouseId", filter.WarehouseId.Value));
                }

                // Thêm filter mã vật tư
                if (!string.IsNullOrEmpty(filter.CodeVatTu))
                {
                    sql += " AND CodeVatTu LIKE @CodeVatTu";
                    parameters.Add(new SqlParameter("@CodeVatTu", $"%{filter.CodeVatTu}%"));
                }

                // Thêm filter tên vật tư
                if (!string.IsNullOrEmpty(filter.TenVatTu))
                {
                    sql += " AND TenVatTu LIKE @TenVatTu";
                    parameters.Add(new SqlParameter("@TenVatTu", $"%{filter.TenVatTu}%"));
                }

                sql += " ORDER BY TenKho, CodeVatTu";

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
                                    SupplyErpId = Convert.ToInt32(reader["SupplyErpId"]),
                                    CodeVatTu = reader.GetString("CodeVatTu"),
                                    TenVatTu = reader.GetString("TenVatTu"),
                                    DonViTinh = reader.IsDBNull("DonViTinh") ? "" : reader.GetString("DonViTinh"),
                                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                                    TenKho = reader.GetString("TenKho"),
                                    TonDauKy = reader.GetDecimal("TonDauKy"),
                                    SoNhap = reader.GetDecimal("SoNhap"),
                                    SoXuat = reader.GetDecimal("SoXuat"),
                                    TonCuoiKy = reader.GetDecimal("TonCuoiKy")
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
