using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class StaffDAL
    {
        /// <summary>
        /// Lấy tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách nhân viên</returns>
        public List<Staff> GetAllStaffs()
        {
            var staffs = new List<Staff>();
            
            string sql = @"
                SELECT ErpIdNV, MaNV, TenNV, d.TenPB
                FROM Staffs St
                LEFT JOIN Departments d ON d.MaPB = St.MaPB
                ORDER BY TenNV";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            staffs.Add(new Staff
                            {
                                ErpIdNV = Convert.ToInt32(reader["ErpIdNV"]),
                                MaNV = Convert.ToString(reader["MaNV"]) ?? string.Empty,
                                TenNV = Convert.ToString(reader["TenNV"]) ?? string.Empty,
                                TenPB = Convert.ToString(reader["TenPB"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return staffs;
        }

        /// <summary>
        /// Tìm kiếm nhân viên
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách nhân viên</returns>
        public List<Staff> SearchStaffs(string keyword)
        {
            var staffs = new List<Staff>();
            
            string sql = @"
                        SELECT ErpIdNV, MaNV, TenNV, d.TenPB
                        FROM Staffs St
                        LEFT JOIN Departments d ON d.MaPB = St.MaPB
                          WHERE ErpIdNV LIKE @keyword 
                             OR MaNV LIKE @keyword 
                             OR TenNV LIKE @keyword 
                             OR d.TenPB LIKE @keyword     
                          ORDER BY TenNV";

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
                            staffs.Add(new Staff
                            {
                                ErpIdNV = Convert.ToInt32(reader["ErpIdNV"]),
                                MaNV = Convert.ToString(reader["MaNV"]) ?? string.Empty,
                                TenNV = Convert.ToString(reader["TenNV"]) ?? string.Empty,
                                TenPB = Convert.ToString(reader["TenPB"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return staffs;
        }
    }
}
