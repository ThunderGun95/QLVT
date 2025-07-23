using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class ManufacturerDAL
    {
        /// <summary>
        /// Lấy tất cả nhà sản xuất
        /// </summary>
        /// <returns>Danh sách nhà sản xuất</returns>
        public List<Manufacturer> GetAllManufacturers()
        {
            var manufacturers = new List<Manufacturer>();
            
            string sql = "SELECT MaNSX, TenNSX FROM Manufacturers ORDER BY MaNSX";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            manufacturers.Add(new Manufacturer
                            {
                                MaNSX = Convert.ToString(reader["MaNSX"]) ?? string.Empty,
                                TenNSX = Convert.ToString(reader["TenNSX"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return manufacturers;
        }

        /// <summary>
        /// Tìm kiếm nhà sản xuất
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách nhà sản xuất</returns>
        public List<Manufacturer> SearchManufacturers(string keyword)
        {
            var manufacturers = new List<Manufacturer>();
            
            string sql = @"SELECT MaNSX, TenNSX FROM Manufacturers 
                          WHERE MaNSX LIKE @keyword OR TenNSX LIKE @keyword 
                          ORDER BY MaNSX";

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
                            manufacturers.Add(new Manufacturer
                            {
                                MaNSX = Convert.ToString(reader["MaNSX"]) ?? string.Empty,
                                TenNSX = Convert.ToString(reader["TenNSX"]) ?? string.Empty
                            });
                        }
                    }
                }
            }
            return manufacturers;
        }
    }
}