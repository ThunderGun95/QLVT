using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;
using System.Data;

namespace QLVT.DAL
{
    public class SupplyDAL
    {
        /// <summary>
        /// Lấy tất cả vật tư
        /// </summary>
        /// <returns>Danh sách vật tư</returns>
        public List<Supply> GetAllSupplies()
        {
            var supplies = new List<Supply>();
            
            string sql = @"
                SELECT s.ErpId, s.Code, s.TenVatTu, s.DacTinhKyThuat, 
                       s.DVT
                FROM ViewVatTus s
                ORDER BY s.Code";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            supplies.Add(new Supply
                            {
                                ErpId = Convert.ToInt32(reader["ErpId"]),
                                Code = Convert.ToString(reader["Code"]) ?? string.Empty,
                                TenVatTu = Convert.ToString(reader["TenVatTu"]) ?? string.Empty,
                                DacTinhKyThuat = reader["DacTinhKyThuat"] == DBNull.Value ? null : Convert.ToString(reader["DacTinhKyThuat"]),
                                TenDVT = reader["DVT"] == DBNull.Value ? null : Convert.ToString(reader["DVT"])
                            });
                        }
                    }
                }
            }
            return supplies;
        }

        /// <summary>
        /// Tìm kiếm vật tư
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách vật tư</returns>
        public List<Supply> SearchSupplies(string keyword)
        {
            var supplies = new List<Supply>();
            
            string sql = @"
                SELECT s.ErpId, s.Code, s.TenVatTu, s.DacTinhKyThuat, 
                       s.DVT
                FROM ViewVatTus s
                WHERE s.ErpId LIKE @keyword 
                   OR s.Code LIKE @keyword 
                   OR s.TenVatTu LIKE @keyword
                ORDER BY s.Code";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@keyword", $"%{keyword}%");
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            supplies.Add(new Supply
                            {
                                ErpId = Convert.ToInt32(reader["ErpId"]),
                                Code = Convert.ToString(reader["Code"]) ?? string.Empty,
                                TenVatTu = Convert.ToString(reader["TenVatTu"]) ?? string.Empty,
                                DacTinhKyThuat = reader["DacTinhKyThuat"] == DBNull.Value ? null : Convert.ToString(reader["DacTinhKyThuat"]),
                                TenDVT = reader["DVT"] == DBNull.Value ? null : Convert.ToString(reader["DVT"])
                            });
                        }
                    }
                }
            }
            return supplies;
        }


        /// <summary>
        /// Tìm kiếm vật tư
        /// </summary>
        public List<VatTuSearchResult> SearchVatTuByKho(string keyword, int? warehouseId = null)
        {
            var vatTuList = new List<VatTuSearchResult>();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    var query = @"
                        SELECT WarehouseId, TenKho, LoaiKho, SupplyErpId, Code, TenVatTu, DacTinhKyThuat, TenDVT, TonKho
                        FROM ViewTonKhoVatTu
                        WHERE (Code LIKE @Keyword 
                            OR TenVatTu LIKE @Keyword)
                            AND (@WarehouseId IS NULL OR WarehouseId = @WarehouseId)
                        ORDER BY Code";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
                        command.Parameters.AddWithValue("@WarehouseId", (object?)warehouseId ?? DBNull.Value);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                vatTuList.Add(new VatTuSearchResult
                                {
                                    ErpId = reader.IsDBNull("SupplyErpId") ? null : reader.GetInt32("SupplyErpId"),
                                    Code = reader.GetString("Code"),
                                    TenVatTu = reader.GetString("TenVatTu"),
                                    DacTinhKyThuat = reader.IsDBNull("DacTinhKyThuat") ? null : reader.GetString("DacTinhKyThuat"),
                                    DonViTinh = reader.GetString("TenDVT"),
                                    SoLuongTon = reader.GetDecimal("TonKho")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tìm kiếm vật tư: {ex.Message}");
            }
            return vatTuList;
        }

        /// <summary>
        /// Kiểm tra số lượng tồn kho
        /// </summary>
        public decimal GetSoLuongTon(int supplyId, int? warehouseId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    var query = @"
                        SELECT COALESCE(TonKho, 0) 
                        FROM ViewTonKhoVatTu 
                        WHERE SupplyErpId = @SupplyId AND WarehouseId = @WarehouseId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplyId", supplyId);
                        command.Parameters.AddWithValue("@WarehouseId", warehouseId ?? 0);

                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi kiểm tra số lượng tồn: {ex.Message}");
            }
        }
    }
}
