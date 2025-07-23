using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

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
                       u.TenDVT, m.TenNSX
                FROM Supplies s
                LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                LEFT JOIN Manufacturers m ON s.MaNSX = m.MaNSX
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
                                ErpId = reader["ErpId"] == DBNull.Value ? null : Convert.ToString(reader["ErpId"]),
                                Code = Convert.ToString(reader["Code"]) ?? string.Empty,
                                TenVatTu = Convert.ToString(reader["TenVatTu"]) ?? string.Empty,
                                DacTinhKyThuat = reader["DacTinhKyThuat"] == DBNull.Value ? null : Convert.ToString(reader["DacTinhKyThuat"]),
                                TenDVT = reader["TenDVT"] == DBNull.Value ? null : Convert.ToString(reader["TenDVT"]),
                                TenNSX = reader["TenNSX"] == DBNull.Value ? null : Convert.ToString(reader["TenNSX"])
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
                       u.TenDVT, m.TenNSX
                FROM Supplies s
                LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                LEFT JOIN Manufacturers m ON s.MaNSX = m.MaNSX
                WHERE s.ErpId LIKE @keyword 
                   OR s.Code LIKE @keyword 
                   OR s.TenVatTu LIKE @keyword
                   OR s.DacTinhKyThuat LIKE @keyword
                   OR s.MaDVT LIKE @keyword
                   OR s.MaNSX LIKE @keyword
                   OR u.TenDVT LIKE @keyword
                   OR m.TenNSX LIKE @keyword
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
                                ErpId = reader["ErpId"] == DBNull.Value ? null : Convert.ToString(reader["ErpId"]),
                                Code = Convert.ToString(reader["Code"]) ?? string.Empty,
                                TenVatTu = Convert.ToString(reader["TenVatTu"]) ?? string.Empty,
                                DacTinhKyThuat = reader["DacTinhKyThuat"] == DBNull.Value ? null : Convert.ToString(reader["DacTinhKyThuat"]),
                                TenDVT = reader["TenDVT"] == DBNull.Value ? null : Convert.ToString(reader["TenDVT"]),
                                TenNSX = reader["TenNSX"] == DBNull.Value ? null : Convert.ToString(reader["TenNSX"])
                            });
                        }
                    }
                }
            }
            return supplies;
        }
    }
}
