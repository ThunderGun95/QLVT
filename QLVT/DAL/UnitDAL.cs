using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class UnitDAL
    {
        /// <summary>
        /// Lấy tất cả đơn vị tính
        /// </summary>
        /// <returns>Danh sách đơn vị tính</returns>
        public List<Unit> GetAllUnits()
        {
            var units = new List<Unit>();
            
            string sql = "SELECT MaDVT, TenDVT FROM Units ORDER BY MaDVT";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            units.Add(new Unit
                            {
                                MaDVT = Convert.ToString(reader["MaDVT"]) ?? string.Empty,
                                TenDVT = Convert.ToString(reader["TenDVT"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return units;
        }

        /// <summary>
        /// Tìm kiếm đơn vị tính
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách đơn vị tính</returns>
        public List<Unit> SearchUnits(string keyword)
        {
            var units = new List<Unit>();
            
            string sql = @"SELECT MaDVT, TenDVT FROM Units 
                          WHERE MaDVT LIKE @keyword OR TenDVT LIKE @keyword 
                          ORDER BY MaDVT";

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
                            units.Add(new Unit
                            {
                                MaDVT = Convert.ToString(reader["MaDVT"]) ?? string.Empty,
                                TenDVT = Convert.ToString(reader["TenDVT"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return units;
        }
    }
}