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
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    // Kiểm tra xem erpCode có phải là số không
                    if (int.TryParse(erpCode, out int erpIdValue))
                    {
                        // Nếu là số, tìm cả ErpId và Code
                        string sql = @"
                            SELECT s.ErpId, s.Code, s.TenVatTu, s.DacTinhKyThuat,
                                   s.MaDVT, u.TenDVT
                            FROM Supplies s
                            LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                            WHERE s.ErpId = @erpIdValue OR s.Code = @erpCode";
                        
                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@erpIdValue", erpIdValue);
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
                    else
                    {
                        // Nếu không phải số, chỉ tìm theo Code
                        string sql = @"
                            SELECT s.ErpId, s.Code, s.TenVatTu, s.DacTinhKyThuat,
                                   s.MaDVT, u.TenDVT
                            FROM Supplies s
                            LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                            WHERE s.Code = @erpCode";
                        
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
                SELECT s.ErpId, s.Code, s.TenVatTu, s.DacTinhKyThuat,
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

        /// <summary>
        /// Lấy thông tin DanhMuc của các vật tư theo ErpId
        /// </summary>
        /// <param name="erpIds">Danh sách ErpId</param>
        /// <returns>Danh sách vật tư với DanhMuc</returns>
        public List<SupplyCategory> GetSuppliesCategoryByIds(List<int> erpIds)
        {
            var supplies = new List<SupplyCategory>();
            
            if (erpIds == null || erpIds.Count == 0)
                return supplies;
                
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    // Tạo IN clause cho danh sách ErpId
                    var parameters = string.Join(",", erpIds.Select((id, index) => $"@erpId{index}"));
                    
                    string sql = $@"
                        SELECT ErpId, Code, TenVatTu, DanhMuc
                        FROM Supplies 
                        WHERE ErpId IN ({parameters})";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        // Thêm parameters
                        for (int i = 0; i < erpIds.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@erpId{i}", erpIds[i]);
                        }
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                supplies.Add(new SupplyCategory
                                {
                                    ErpId = Convert.ToInt32(reader["ErpId"]),
                                    Code = reader["Code"].ToString() ?? string.Empty,
                                    TenVatTu = reader["TenVatTu"].ToString() ?? string.Empty,
                                    DanhMuc = reader["DanhMuc"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin danh mục vật tư: {ex.Message}");
            }

            return supplies;
        }
    }
}
