using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class SupplyMappingDAL
    {
        /// <summary>
        /// Tìm vật tư trong hệ thống QLVT theo mã ERP
        /// </summary>
        /// <param name="erpCode">Mã vật tư từ ERP</param>
        /// <returns>Thông tin vật tư</returns>
        public Supply? FindSupplyByERPCode(string erpCode)
        {
            Supply? supply = null;
            
            string sql = @"
                SELECT s.ErpId, s.Code, s.TenVatTu, s.DacTinhKyThuat,
                       s.MaDVT, u.TenDVT
                FROM Supplies s
                LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                WHERE s.ErpId = @erpCode OR s.Code = @erpCode";

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@erpCode", erpCode);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                supply = new Supply
                                {
                                    ErpId = Convert.ToInt32(reader["ErpId"]),
                                    Code = reader["Code"].ToString() ?? string.Empty,
                                    TenVatTu = reader["TenVatTu"].ToString() ?? string.Empty,
                                    DacTinhKyThuat = reader["DacTinhKyThuat"].ToString(),
                                    MaDVT = reader["MaDVT"].ToString() ?? string.Empty,
                                    TenDVT = reader["TenDVT"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm vật tư {erpCode}: {ex.Message}");
            }

            return supply;
        }

        /// <summary>
        /// Tìm kiếm vật tư theo tên (cho mapping thủ công)
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách vật tư</returns>
        public List<Supply> SearchSupplies(string keyword)
        {
            var supplies = new List<Supply>();
            
            string sql = @"
                SELECT s.Id, s.ErpId, s.Code, s.TenVatTu, s.DacTinhKyThuat,
                       s.MaDVT, u.TenDVT
                FROM Supplies s
                LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                WHERE s.Code LIKE @keyword OR s.TenVatTu LIKE @keyword OR s.ErpId LIKE @keyword
                ORDER BY s.TenVatTu";

            try
            {
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
                                    Code = reader["Code"].ToString() ?? string.Empty,
                                    TenVatTu = reader["TenVatTu"].ToString() ?? string.Empty,
                                    DacTinhKyThuat = reader["DacTinhKyThuat"].ToString(),
                                    MaDVT = reader["MaDVT"].ToString() ?? string.Empty,
                                    TenDVT = reader["TenDVT"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm vật tư: {ex.Message}");
            }

            return supplies;
        }
    }
}
