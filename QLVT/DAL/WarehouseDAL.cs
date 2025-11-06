using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;
using System.Data;

namespace QLVT.DAL
{
    public class WarehouseDAL
    {
        /// <summary>
        /// Lấy danh sách kho
        /// </summary>
        public List<Warehouse> GetAllWarehouses()
        {
            var warehouses = new List<Warehouse>();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    var query = @"
                        SELECT Id, MaKho, TenKho, LoaiKho, MaNV, DiaChi, GhiChu, IsActive, CreatedDate
                        FROM Warehouses 
                        WHERE IsActive = 1
                        ORDER BY MaKho";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            warehouses.Add(new Warehouse
                            {
                                Id = reader.GetInt32("Id"),
                                MaKho = reader.GetString("MaKho"),
                                TenKho = reader.GetString("TenKho"),
                                LoaiKho = reader.GetString("LoaiKho"),
                                MaNV = reader.IsDBNull("MaNV") ? null : reader.GetString("MaNV"),
                                DiaChi = reader.IsDBNull("DiaChi") ? null : reader.GetString("DiaChi"),
                                GhiChu = reader.IsDBNull("GhiChu") ? null : reader.GetString("GhiChu"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách kho: {ex.Message}");
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

        /// <summary>
        /// Lấy kho cá nhân theo mã nhân viên
        /// </summary>
        /// <param name="staffCode">Mã nhân viên</param>
        /// <returns>Kho cá nhân của nhân viên hoặc null nếu không có</returns>
        public Warehouse? GetWarehouseByStaffCode(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
                return null;

            Warehouse? warehouse = null;
            
            string sql = @"
                SELECT w.Id, w.MaKho, w.TenKho, w.LoaiKho, w.MaNV, 
                       s.TenNV, w.DiaChi, w.GhiChu, w.IsActive
                FROM Warehouses w
                LEFT JOIN Staffs s ON w.MaNV = s.MaNV
                WHERE w.MaNV = @staffCode AND w.LoaiKho = 'CANHAN' AND w.IsActive = 1";

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@staffCode", staffCode);
                        
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
                throw new Exception($"Lỗi khi lấy kho nhân viên {staffCode}: {ex.Message}");
            }

            return warehouse;
        }

        /// <summary>
        /// Lấy danh sách kho (async version)
        /// </summary>
        public async Task<List<Warehouse>> GetAllWarehousesAsync()
        {
            var warehouses = new List<Warehouse>();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        SELECT Id, MaKho, TenKho, LoaiKho, MaNV, DiaChi, GhiChu, IsActive, CreatedDate
                        FROM Warehouses 
                        WHERE IsActive = 1
                        ORDER BY MaKho";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            warehouses.Add(new Warehouse
                            {
                                Id = reader.GetInt32("Id"),
                                MaKho = reader.GetString("MaKho"),
                                TenKho = reader.GetString("TenKho"),
                                LoaiKho = reader.GetString("LoaiKho"),
                                MaNV = reader.IsDBNull("MaNV") ? null : reader.GetString("MaNV"),
                                DiaChi = reader.IsDBNull("DiaChi") ? null : reader.GetString("DiaChi"),
                                GhiChu = reader.IsDBNull("GhiChu") ? null : reader.GetString("GhiChu"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách kho: {ex.Message}");
            }
            return warehouses;
        }

        /// <summary>
        /// Lấy kho theo mã kho
        /// </summary>
        public async Task<Warehouse?> GetWarehouseByIdAsync(long warehouseId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        SELECT Id, MaKho, TenKho, DiaChi, GhiChu 
                        FROM Warehouses 
                        WHERE Id = @MaKho";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKho", warehouseId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Warehouse
                                {
                                    Id = reader.GetInt32("Id"),
                                    MaKho = reader.GetString("MaKho"),
                                    TenKho = reader.GetString("TenKho")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy thông tin kho Id {warehouseId}: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Lấy kho theo mã kho string
        /// </summary>
        public async Task<Warehouse?> GetWarehouseByMaKhoAsync(string maKho)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        SELECT Id, MaKho, TenKho, LoaiKho, MaNV, DiaChi, GhiChu, IsActive 
                        FROM Warehouses 
                        WHERE MaKho = @MaKho AND IsActive = 1";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKho", maKho);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Warehouse
                                {
                                    Id = reader.GetInt32("Id"),
                                    MaKho = reader.GetString("MaKho"),
                                    TenKho = reader.GetString("TenKho"),
                                    LoaiKho = reader.GetString("LoaiKho"),
                                    MaNV = reader.IsDBNull("MaNV") ? null : reader.GetString("MaNV"),
                                    DiaChi = reader.IsDBNull("DiaChi") ? null : reader.GetString("DiaChi"),
                                    GhiChu = reader.IsDBNull("GhiChu") ? null : reader.GetString("GhiChu"),
                                    IsActive = reader.GetBoolean("IsActive")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy thông tin kho mã {maKho}: {ex.Message}");
            }
            return null;
        }
        
    }
}
