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
    /// Data Access Layer cho báo cáo xuất nhập tồn - CHI TIẾT
    /// </summary>
    public class BaoCaoXuatNhapTonChiTietDAL
    {
        /// <summary>
        /// Lấy báo cáo chi tiết xuất nhập tồn theo vật tư
        /// </summary>
        public async Task<List<TransactionDetailReportItem>> GetTransactionDetailAsync(TransactionDetailFilter filter)
        {
            var result = new List<TransactionDetailReportItem>();
            
            try
            {
                var sql = @"
                    WITH TransactionHistory AS (
                        SELECT 
                            t.NgayGiaoDich,
                            t.LoaiGiaoDich,
                            t.SoPhieu,
                            td.SoLuong,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('NhapKho', 'TraKho') THEN td.SoLuong
                                ELSE 0
                            END as SoLuongNhap,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('XuatKho', 'HoanUng') THEN td.SoLuong
                                ELSE 0
                            END as SoLuongXuat,
                            ISNULL(td.GhiChu, t.GhiChu) as GhiChu,
                            t.Id as TransactionId,
                            td.Id as TransactionDetailId
                        FROM Transactions t
                        INNER JOIN TransactionDetails td ON t.Id = td.TransactionId
                        INNER JOIN Supplies s ON td.ErpId = s.ErpId
                        WHERE s.ErpId = @SupplyErpId
                          AND t.NgayGiaoDich >= @TuNgay 
                          AND t.NgayGiaoDich <= @DenNgay";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@SupplyErpId", filter.SupplyErpId),
                    new SqlParameter("@TuNgay", filter.TuNgay.Date),
                    new SqlParameter("@DenNgay", filter.DenNgay.Date.AddDays(1).AddSeconds(-1))
                };

                // Thêm filter warehouse nếu có
                if (filter.WarehouseId.HasValue)
                {
                    sql += @" AND (
                        (t.LoaiGiaoDich = 'NhapKho' AND t.MaKhoNhan = @WarehouseId) OR
                        (t.LoaiGiaoDich = 'XuatKho' AND td.SourceWarehouseId = @WarehouseId) OR
                        (t.LoaiGiaoDich = 'TraKho' AND t.MaKhoNhan = @WarehouseId) OR
                        (t.LoaiGiaoDich = 'HoanUng' AND td.SourceWarehouseId = @WarehouseId)
                    )";
                    parameters.Add(new SqlParameter("@WarehouseId", filter.WarehouseId.Value));
                }

                sql += @"
                    ),
                    TonTruoc AS (
                        -- Tính tồn trước kỳ báo cáo
                        SELECT 
                            ISNULL(SUM(
                                CASE 
                                    WHEN t.LoaiGiaoDich IN ('NhapKho', 'TraKho') THEN td.SoLuong
                                    ELSE -td.SoLuong
                                END
                            ), 0) as TonDauKy
                        FROM Transactions t
                        INNER JOIN TransactionDetails td ON t.Id = td.TransactionId
                        INNER JOIN Supplies s ON td.ErpId = s.ErpId
                        WHERE s.ErpId = @SupplyErpId
                          AND t.NgayGiaoDich < @TuNgay";

                if (filter.WarehouseId.HasValue)
                {
                    sql += @" AND (
                        (t.LoaiGiaoDich = 'NhapKho' AND t.MaKhoNhan = @WarehouseId) OR
                        (t.LoaiGiaoDich = 'XuatKho' AND td.SourceWarehouseId = @WarehouseId) OR
                        (t.LoaiGiaoDich = 'TraKho' AND t.MaKhoNhan = @WarehouseId) OR
                        (t.LoaiGiaoDich = 'HoanUng' AND td.SourceWarehouseId = @WarehouseId)
                    )";
                }

                sql += @"
                    ),
                    DetailWithRunningTotal AS (
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY th.NgayGiaoDich, th.TransactionId) as STT,
                            th.NgayGiaoDich,
                            th.LoaiGiaoDich,
                            th.SoPhieu,
                            th.SoLuongNhap,
                            th.SoLuongXuat,
                            th.GhiChu,
                            SUM(th.SoLuongNhap - th.SoLuongXuat) OVER (
                                ORDER BY th.NgayGiaoDich, th.TransactionId 
                                ROWS UNBOUNDED PRECEDING
                            ) + tt.TonDauKy as TonSauGD
                        FROM TransactionHistory th
                        CROSS JOIN TonTruoc tt
                    )
                    SELECT * FROM DetailWithRunningTotal
                    ORDER BY NgayGiaoDich, STT";

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
                                result.Add(new TransactionDetailReportItem
                                {
                                    STT = reader.GetInt32("STT"),
                                    NgayGiaoDich = reader.GetDateTime("NgayGiaoDich"),
                                    LoaiGiaoDich = reader.GetString("LoaiGiaoDich"),
                                    SoPhieu = reader.IsDBNull("SoPhieu") ? "" : reader.GetString("SoPhieu"),
                                    SoLuongNhap = reader.GetInt32("SoLuongNhap"),
                                    SoLuongXuat = reader.GetInt32("SoLuongXuat"),
                                    TonSauGD = reader.GetInt32("TonSauGD"),
                                    GhiChu = reader.IsDBNull("GhiChu") ? "" : reader.GetString("GhiChu")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo chi tiết xuất nhập tồn: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Lấy thông tin vật tư cho tiêu đề báo cáo chi tiết
        /// </summary>
        public async Task<(string CodeVatTu, string TenVatTu, string DonViTinh, string TenKho)> GetSupplyInfoAsync(int supplyErpId, int? warehouseId = null)
        {
            try
            {
                var sql = @"
                    SELECT 
                        s.Code as CodeVatTu,
                        s.TenVatTu,
                        u.TenDVT as DonViTinh
                    FROM Supplies s
                    LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                    WHERE s.ErpId = @SupplyErpId";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@SupplyErpId", supplyErpId)
                };

                string codeVatTu = "";
                string tenVatTu = "";
                string donViTinh = "";
                string tenKho = "Tất cả kho";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                codeVatTu = reader.GetString("CodeVatTu");
                                tenVatTu = reader.GetString("TenVatTu");
                                donViTinh = reader.IsDBNull("DonViTinh") ? "" : reader.GetString("DonViTinh");
                            }
                        }
                    }

                    // Lấy tên kho nếu có chỉ định warehouse
                    if (warehouseId.HasValue)
                    {
                        var warehouseSql = "SELECT TenKho FROM Warehouses WHERE Id = @WarehouseId";
                        using (var warehouseCommand = new SqlCommand(warehouseSql, connection))
                        {
                            warehouseCommand.Parameters.Add(new SqlParameter("@WarehouseId", warehouseId.Value));
                            var warehouseResult = await warehouseCommand.ExecuteScalarAsync();
                            if (warehouseResult != null)
                            {
                                tenKho = warehouseResult.ToString() ?? "Không xác định";
                            }
                        }
                    }
                }

                return (codeVatTu, tenVatTu, donViTinh, tenKho);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin vật tư: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo chi tiết xuất nhập tồn với bộ lọc mở rộng
        /// </summary>
        public async Task<List<TransactionDetailReportItem>> GetTransactionDetailByFilterAsync(
            DateTime tuNgay, 
            DateTime denNgay, 
            string? maVatTu = null, 
            int? warehouseId = null)
        {
            var result = new List<TransactionDetailReportItem>();
            
            try
            {
                var sql = @"
                    WITH TransactionHistory AS (
                        SELECT 
                            t.NgayGiaoDich,
                            t.LoaiGiaoDich,
                            t.SoPhieu,
                            td.SoLuong,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('NhapKho', 'TraKho') THEN td.SoLuong
                                ELSE 0
                            END as SoLuongNhap,
                            CASE 
                                WHEN t.LoaiGiaoDich IN ('XuatKho', 'HoanUng') THEN td.SoLuong
                                ELSE 0
                            END as SoLuongXuat,
                            ISNULL(td.GhiChu, t.GhiChu) as GhiChu,
                            t.Id as TransactionId,
                            td.Id as TransactionDetailId,
                            s.Code as MaVatTu,
                            s.Name as TenVatTu,
                            s.Unit as DonViTinh,
                            w.Name as TenKho
                        FROM Transactions t
                        INNER JOIN TransactionDetails td ON t.Id = td.TransactionId
                        INNER JOIN Supplies s ON td.ErpId = s.ErpId
                        LEFT JOIN Warehouses w ON t.WarehouseId = w.Id
                        WHERE t.NgayGiaoDich >= @TuNgay 
                            AND t.NgayGiaoDich <= @DenNgay";

                // Thêm điều kiện lọc theo mã vật tư
                if (!string.IsNullOrEmpty(maVatTu))
                {
                    sql += " AND s.Code LIKE @MaVatTu";
                }

                // Thêm điều kiện lọc theo kho
                if (warehouseId.HasValue)
                {
                    sql += " AND t.WarehouseId = @WarehouseId";
                }

                sql += @"
                    )
                    SELECT 
                        NgayGiaoDich,
                        LoaiGiaoDich,
                        SoPhieu,
                        SoLuong,
                        SoLuongNhap,
                        SoLuongXuat,
                        GhiChu,
                        MaVatTu,
                        TenVatTu,
                        DonViTinh,
                        TenKho,
                        SUM(SoLuongNhap - SoLuongXuat) OVER (
                            PARTITION BY MaVatTu, TenKho 
                            ORDER BY NgayGiaoDich, TransactionId, TransactionDetailId
                            ROWS UNBOUNDED PRECEDING
                        ) as TonKho
                    FROM TransactionHistory
                    ORDER BY MaVatTu, TenKho, NgayGiaoDich, TransactionId";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@TuNgay", tuNgay.Date));
                        command.Parameters.Add(new SqlParameter("@DenNgay", denNgay.Date.AddDays(1).AddMilliseconds(-1)));
                        
                        if (!string.IsNullOrEmpty(maVatTu))
                        {
                            command.Parameters.Add(new SqlParameter("@MaVatTu", $"%{maVatTu}%"));
                        }
                        
                        if (warehouseId.HasValue)
                        {
                            command.Parameters.Add(new SqlParameter("@WarehouseId", warehouseId.Value));
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new TransactionDetailReportItem
                                {
                                    NgayGiaoDich = reader.GetDateTime("NgayGiaoDich"),
                                    LoaiGiaoDich = reader.GetString("LoaiGiaoDich"),
                                    SoPhieu = reader.IsDBNull("SoPhieu") ? string.Empty : reader.GetString("SoPhieu"),
                                    SoLuong = Convert.ToInt32(reader.GetInt64("SoLuong")),
                                    SoLuongNhap = Convert.ToInt32(reader.GetInt64("SoLuongNhap")),
                                    SoLuongXuat = Convert.ToInt32(reader.GetInt64("SoLuongXuat")),
                                    TonKho = Convert.ToInt32(reader.GetInt64("TonKho")),
                                    GhiChu = reader.IsDBNull("GhiChu") ? string.Empty : reader.GetString("GhiChu"),
                                    MaVatTu = reader.IsDBNull("MaVatTu") ? string.Empty : reader.GetString("MaVatTu"),
                                    TenVatTu = reader.IsDBNull("TenVatTu") ? string.Empty : reader.GetString("TenVatTu"),
                                    DonViTinh = reader.IsDBNull("DonViTinh") ? string.Empty : reader.GetString("DonViTinh"),
                                    TenKho = reader.IsDBNull("TenKho") ? "Chưa xác định" : reader.GetString("TenKho")
                                });
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo chi tiết: {ex.Message}");
            }
        }
    }
}
