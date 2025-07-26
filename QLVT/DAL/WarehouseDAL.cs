using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class WarehouseDAL
    {
        /// <summary>
        /// Lấy danh sách kho
        /// </summary>
        /// <returns>Danh sách kho</returns>
        public List<Warehouse> GetWarehouses()
        {
            var warehouses = new List<Warehouse>();
            
            string sql = @"
                SELECT w.Id, w.MaKho, w.TenKho, w.LoaiKho, w.MaNV, 
                       s.TenNV, w.DiaChi, w.GhiChu, w.IsActive
                FROM Warehouses w
                LEFT JOIN Staffs s ON w.MaNV = s.MaNV
                WHERE w.IsActive = 1
                ORDER BY w.LoaiKho, w.TenKho";

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                warehouses.Add(new Warehouse
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    MaKho = reader["MaKho"].ToString() ?? string.Empty,
                                    TenKho = reader["TenKho"].ToString() ?? string.Empty,
                                    LoaiKho = reader["LoaiKho"].ToString() ?? string.Empty,
                                    MaNV = reader["MaNV"].ToString(),
                                    TenNV = reader["TenNV"].ToString(),
                                    DiaChi = reader["DiaChi"].ToString(),
                                    GhiChu = reader["GhiChu"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách kho: {ex.Message}");
            }

            return warehouses;
        }

        /// <summary>
        /// Lấy kho theo ID
        /// </summary>
        /// <param name="warehouseId">ID kho</param>
        /// <returns>Thông tin kho</returns>
        public Warehouse? GetWarehouseById(int warehouseId)
        {
            Warehouse? warehouse = null;
            
            string sql = @"
                SELECT w.Id, w.MaKho, w.TenKho, w.LoaiKho, w.MaNV, 
                       s.TenNV, w.DiaChi, w.GhiChu, w.IsActive
                FROM Warehouses w
                LEFT JOIN Staffs s ON w.MaNV = s.MaNV
                WHERE w.Id = @warehouseId";

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@warehouseId", warehouseId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                warehouse = new Warehouse
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    MaKho = reader["MaKho"].ToString() ?? string.Empty,
                                    TenKho = reader["TenKho"].ToString() ?? string.Empty,
                                    LoaiKho = reader["LoaiKho"].ToString() ?? string.Empty,
                                    MaNV = reader["MaNV"].ToString(),
                                    TenNV = reader["TenNV"].ToString(),
                                    DiaChi = reader["DiaChi"].ToString(),
                                    GhiChu = reader["GhiChu"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin kho {warehouseId}: {ex.Message}");
            }

            return warehouse;
        }
    }
}
