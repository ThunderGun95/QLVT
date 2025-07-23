using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class DepartmentDAL
    {
        /// <summary>
        /// Lấy tất cả phòng ban
        /// </summary>
        /// <returns>Danh sách phòng ban</returns>
        public List<Department> GetAllDepartments()
        {
            var departments = new List<Department>();
            
            string sql = "SELECT MaPB, TenPB FROM Departments ORDER BY MaPB";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departments.Add(new Department
                            {
                                MaPB = Convert.ToString(reader["MaPB"]) ?? string.Empty,
                                TenPB = Convert.ToString(reader["TenPB"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return departments;
        }

        /// <summary>
        /// Tìm kiếm phòng ban
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách phòng ban</returns>
        public List<Department> SearchDepartments(string keyword)
        {
            var departments = new List<Department>();
            
            string sql = @"SELECT MaPB, TenPB FROM Departments 
                          WHERE MaPB LIKE @keyword OR TenPB LIKE @keyword 
                          ORDER BY MaPB";

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
                            departments.Add(new Department
                            {
                                MaPB = Convert.ToString(reader["MaPB"]) ?? string.Empty,
                                TenPB = Convert.ToString(reader["TenPB"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return departments;
        }
    }
}
