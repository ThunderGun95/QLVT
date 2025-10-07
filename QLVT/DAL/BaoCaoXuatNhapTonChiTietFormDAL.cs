using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class BaoCaoXuatNhapTonChiTietFormDAL
    {
        public BaoCaoXuatNhapTonChiTietFormDAL()
        {
        }

        /// <summary>
        /// Lấy báo cáo xuất nhập tồn chi tiết theo filter (cho form cửa sổ mới)
        /// </summary>
        public async Task<List<BaoCaoXuatNhapTonChiTietItem>> GetBaoCaoXuatNhapTonChiTietAsync(BaoCaoXuatNhapTonChiTietFilter filter)
        {
            var result = new List<BaoCaoXuatNhapTonChiTietItem>();

            try
            {
                string sql = @"
                    WITH TransactionData AS (
                        -- Lấy tất cả giao dịch trong khoảng thời gian
                        SELECT 
                            t.NgayGiaoDich,
                            CASE 
                                WHEN t.LoaiGiaoDich = 'NhapKho' THEN N'Nhập kho'
                                WHEN t.LoaiGiaoDich = 'XuatKho' THEN N'Xuất kho'
                                WHEN t.LoaiGiaoDich = 'TraKho' THEN N'Trả kho'
                                WHEN t.LoaiGiaoDich = 'HoanUng' THEN N'Hoàn ứng'
                                ELSE t.LoaiGiaoDich
                            END as LoaiGiaoDich,
                            t.SoPhieu,
                            vt.Code as MaVatTu,
                            vt.TenVatTu,
                            vt.DVT as DonViTinh,
                            w.TenKho,
                            CASE 
                                WHEN td.MaKhoNhap = w.Id THEN CAST(td.SoLuong as float)
                                ELSE 0.0
                            END as SoLuongNhap,
                            CASE 
                                WHEN td.MaKhoXuat = w.Id THEN CAST(td.SoLuong as float)
                                ELSE 0.0
                            END as SoLuongXuat,
                            ISNULL(td.GhiChu, t.GhiChu) as GhiChu,
                            w.Id as WarehouseId,
                            vt.ErpId as SupplyId
                        FROM Transactions t
                        INNER JOIN TransactionDetails td ON t.Id = td.TransactionId AND td.IsDeleted = 0
                        INNER JOIN ViewVatTus vt ON td.ErpId = vt.ErpId
                        INNER JOIN Warehouses w ON (td.MaKhoNhap = w.Id OR td.MaKhoXuat = w.Id)
                        WHERE t.IsDeleted = 0 
                          AND t.NgayGiaoDich >= @TuNgay 
                          AND t.NgayGiaoDich <= @DenNgay
                          AND (td.MaKhoNhap = w.Id OR td.MaKhoXuat = w.Id)
                    )
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY NgayGiaoDich, LoaiGiaoDich, SoPhieu) as STT,
                        NgayGiaoDich,
                        LoaiGiaoDich,
                        SoPhieu,
                        MaVatTu,
                        TenVatTu,
                        DonViTinh,
                        TenKho,
                        SoLuongNhap,
                        SoLuongXuat,
                        0.0 as TonSauGD, -- Sẽ tính sau
                        GhiChu,
                        WarehouseId,
                        SupplyId
                    FROM TransactionData
                    WHERE (SoLuongNhap > 0 OR SoLuongXuat > 0)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@TuNgay", filter.TuNgay.Date),
                    new SqlParameter("@DenNgay", filter.DenNgay.Date.AddDays(1).AddSeconds(-1))
                };

                // Thêm filter theo kho (có thể theo ID hoặc tên)
                if (filter.WarehouseId.HasValue)
                {
                    sql += " AND WarehouseId = @WarehouseId";
                    parameters.Add(new SqlParameter("@WarehouseId", filter.WarehouseId.Value));
                }
                else if (!string.IsNullOrEmpty(filter.TenKho))
                {
                    sql += " AND TenKho LIKE @TenKho";
                    parameters.Add(new SqlParameter("@TenKho", $"%{filter.TenKho}%"));
                }

                // Thêm filter theo mã vật tư
                if (!string.IsNullOrEmpty(filter.MaVatTu))
                {
                    sql += " AND (MaVatTu LIKE @MaVatTu OR TenVatTu LIKE @MaVatTu)";
                    parameters.Add(new SqlParameter("@MaVatTu", $"%{filter.MaVatTu}%"));
                }

                sql += " ORDER BY NgayGiaoDich, LoaiGiaoDich, SoPhieu";

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
                                var item = new BaoCaoXuatNhapTonChiTietItem
                                {
                                    STT = Convert.ToInt32(reader["STT"]),
                                    NgayGiaoDich = Convert.ToDateTime(reader["NgayGiaoDich"]),
                                    LoaiGiaoDich = reader["LoaiGiaoDich"].ToString() ?? "",
                                    SoPhieu = reader["SoPhieu"].ToString() ?? "",
                                    MaVatTu = reader["MaVatTu"].ToString() ?? "",
                                    TenVatTu = reader["TenVatTu"].ToString() ?? "",
                                    DonViTinh = reader["DonViTinh"].ToString() ?? "",
                                    TenKho = reader["TenKho"].ToString() ?? "",
                                    SoLuongNhap = Convert.ToDecimal(reader["SoLuongNhap"]),
                                    SoLuongXuat = Convert.ToDecimal(reader["SoLuongXuat"]),
                                    TonSauGD = Convert.ToDecimal(reader["TonSauGD"]),
                                    GhiChu = reader["GhiChu"].ToString() ?? "",
                                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                                    SupplyId = Convert.ToInt32(reader["SupplyId"])
                                };

                                result.Add(item);
                            }
                        }
                    }
                }

                // Tính tồn sau giao dịch
                await CalculateTonSauGiaoDichAsync(result);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy báo cáo xuất nhập tồn chi tiết: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính tồn sau giao dịch cho từng dòng
        /// </summary>
        private async Task CalculateTonSauGiaoDichAsync(List<BaoCaoXuatNhapTonChiTietItem> transactions)
        {
            var tonKhoDict = new Dictionary<string, decimal>(); // Key: WarehouseId_SupplyId

            foreach (var item in transactions.OrderBy(x => x.NgayGiaoDich).ThenBy(x => x.STT))
            {
                string key = $"{item.WarehouseId}_{item.SupplyId}";
                
                if (!tonKhoDict.ContainsKey(key))
                {
                    // Lấy tồn đầu kỳ
                    tonKhoDict[key] = await GetTonDauKyAsync(item.WarehouseId, item.SupplyId, transactions.First().NgayGiaoDich);
                }

                // Cập nhật tồn kho
                tonKhoDict[key] += item.SoLuongNhap - item.SoLuongXuat;
                item.TonSauGD = tonKhoDict[key];
            }
        }

        /// <summary>
        /// Lấy tồn đầu kỳ (method public để BLL có thể gọi)
        /// </summary>
        public async Task<decimal> GetTonDauKyAsync(int warehouseId, int supplyId, DateTime tuNgay)
        {
            try
            {
                string sql = @"
                    SELECT ISNULL(SUM(
                        CASE 
                            WHEN td.MaKhoNhap = @WarehouseId THEN td.SoLuong
                            WHEN td.MaKhoXuat = @WarehouseId THEN -td.SoLuong
                            ELSE 0
                        END), 0)
                    FROM Transactions t
                    INNER JOIN TransactionDetails td ON t.Id = td.TransactionId AND td.IsDeleted = 0
                    INNER JOIN ViewVatTus vt ON td.ErpId = vt.ErpId
                    WHERE vt.ErpId = @SupplyId 
                      AND t.IsDeleted = 0
                      AND t.NgayGiaoDich < @TuNgay
                      AND (td.MaKhoNhap = @WarehouseId OR td.MaKhoXuat = @WarehouseId)";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddRange(new[]
                        {
                            new SqlParameter("@WarehouseId", warehouseId),
                            new SqlParameter("@SupplyId", supplyId),
                            new SqlParameter("@TuNgay", tuNgay)
                        });

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToDecimal(result ?? 0);
                    }
                }
            }
            catch
            {
                return 0; // Nếu có lỗi thì trả về 0
            }
        }

        /// <summary>
        /// Lấy danh sách kho
        /// </summary>
        public async Task<List<WarehouseItem>> GetWarehousesAsync()
        {
            var result = new List<WarehouseItem>();
            
            try
            {
                string sql = "SELECT Id, TenKho FROM Warehouses ORDER BY TenKho";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new WarehouseItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["TenKho"].ToString() ?? ""
                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách kho: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm vật tư theo mã hoặc tên
        /// </summary>
        public async Task<List<(string Code, string Name)>> SearchSuppliesAsync(string keyword)
        {
            var result = new List<(string Code, string Name)>();

            try
            {
                string sql = @"
                    SELECT TOP 20 Code, TenVatTu 
                    FROM Supplies 
                    WHERE Code LIKE @Keyword OR TenVatTu LIKE @Keyword
                    ORDER BY Code";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Keyword", $"%{keyword}%"));
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add((
                                    reader["Code"].ToString() ?? "",
                                    reader["TenVatTu"].ToString() ?? ""
                                ));
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm vật tư: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Search warehouses by name
        /// </summary>
        public async Task<List<WarehouseItem>> SearchWarehousesAsync(string searchText)
        {
            var result = new List<WarehouseItem>();
            
            try
            {
                var sql = @"
                    SELECT Id, TenKho as Name
                    FROM Warehouses 
                    WHERE TenKho LIKE @SearchText
                    ORDER BY TenKho";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@SearchText", $"%{searchText}%"));
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new WarehouseItem
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString() ?? ""
                                });
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm kho: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get transaction detail for report
        /// </summary>
        public async Task<List<TransactionDetailReportItem>> GetTransactionDetailAsync(TransactionDetailFilter filter)
        {
            var result = new List<TransactionDetailReportItem>();
            // Implementation placeholder - needs actual query
            return result;
        }

        /// <summary>
        /// Get supply information
        /// </summary>
        public async Task<(string CodeVatTu, string TenVatTu, string DonViTinh, string TenKho)> GetSupplyInfoAsync(int supplyErpId, int? warehouseId = null)
        {
            // Implementation placeholder - needs actual query
            return ("", "", "", "");
        }

        /// <summary>
        /// Lấy ErpId của vật tư theo mã vật tư
        /// </summary>
        public async Task<int?> GetSupplyErpIdAsync(string maVatTu)
        {
            try
            {
                string sql = @"
                    SELECT TOP 1 ErpId 
                    FROM ViewVatTus 
                    WHERE Code = @MaVatTu";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@MaVatTu", maVatTu));

                        var result = await command.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
