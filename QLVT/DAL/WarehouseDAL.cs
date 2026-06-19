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

        public Warehouse? GetKhoUuTienByMaNV(string manv)
        {
            if (string.IsNullOrWhiteSpace(manv))
                return null;

            Warehouse? warehouse = null;

            string sql = @"
                SELECT w.Id, w.MaKho, w.TenKho, w.LoaiKho, w.MaNV, 
                       s.TenNV, w.DiaChi, w.GhiChu, w.IsActive
                FROM Warehouses w
                LEFT JOIN Staffs s ON w.MaNV = s.MaNV
                WHERE w.MaNV = @staffCode AND w.LoaiKho = 'CANHAN' AND w.IsActive = 1 AND w.KhoUuTien = 1";

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@staffCode", manv);

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
                throw new Exception($"Lỗi khi lấy kho nhân viên {manv}: {ex.Message}");
            }

            return warehouse;
        }

        /// <summary>
        /// Lấy kho theo mã kho
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <returns>Thông tin kho hoặc null</returns>
        public Warehouse? GetWarehouseByCode(string maKho)
        {
            if (string.IsNullOrWhiteSpace(maKho))
                return null;

            Warehouse? warehouse = null;

            string sql = @"
                SELECT w.Id, w.MaKho, w.TenKho, w.LoaiKho, w.MaNV, 
                       s.TenNV, w.DiaChi, w.GhiChu, w.IsActive
                FROM Warehouses w
                LEFT JOIN Staffs s ON w.MaNV = s.MaNV
                WHERE w.MaKho = @maKho AND w.IsActive = 1";

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@maKho", maKho);

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
                                    MaNV = reader["MaNV"] == DBNull.Value ? null : reader["MaNV"].ToString(),
                                    DiaChi = reader["DiaChi"] == DBNull.Value ? null : reader["DiaChi"].ToString(),
                                    GhiChu = reader["GhiChu"] == DBNull.Value ? null : reader["GhiChu"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy kho {maKho}: {ex.Message}");
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

        public List<Warehouse> GetWarehousesForManagement()
        {
            var warehouses = new List<Warehouse>();
            string sql = @"
                SELECT w.Id, w.MaKho, w.TenKho, w.LoaiKho, w.MaNV, s.TenNV,
                       w.DiaChi, w.GhiChu, ISNULL(w.KhoUuTien, 0) AS KhoUuTien, w.IsActive
                FROM Warehouses w
                LEFT JOIN Staffs s ON s.MaNV = w.MaNV
                WHERE w.IsActive = 1
                ORDER BY
                    CASE WHEN w.LoaiKho IN ('CANHAN', 'PERSONAL') THEN 1 ELSE 0 END,
                    ISNULL(s.TenNV, ''),
                    w.MaKho";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        warehouses.Add(ReadWarehouse(reader));
                    }
                }
            }

            return warehouses;
        }

        public int SaveWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));

            if (string.IsNullOrWhiteSpace(warehouse.MaKho))
                throw new Exception("Mã kho không được để trống");

            if (string.IsNullOrWhiteSpace(warehouse.TenKho))
                throw new Exception("Tên kho không được để trống");

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        EnsureUniqueWarehouseCode(connection, transaction, warehouse.MaKho.Trim(), warehouse.Id);

                        int warehouseId;
                        if (warehouse.Id > 0)
                        {
                            string updateSql = @"
                                UPDATE Warehouses
                                SET MaKho = @MaKho,
                                    TenKho = @TenKho,
                                    LoaiKho = @LoaiKho,
                                    MaNV = @MaNV,
                                    DiaChi = @DiaChi,
                                    GhiChu = @GhiChu,
                                    KhoUuTien = @KhoUuTien
                                WHERE Id = @Id";

                            using (var command = new SqlCommand(updateSql, connection, transaction))
                            {
                                AddWarehouseParameters(command, warehouse);
                                command.Parameters.AddWithValue("@Id", warehouse.Id);
                                command.ExecuteNonQuery();
                            }

                            warehouseId = warehouse.Id;
                        }
                        else
                        {
                            string insertSql = @"
                                INSERT INTO Warehouses (MaKho, TenKho, LoaiKho, MaNV, DiaChi, GhiChu, KhoUuTien, IsActive, CreatedDate)
                                VALUES (@MaKho, @TenKho, @LoaiKho, @MaNV, @DiaChi, @GhiChu, @KhoUuTien, 1, GETDATE());
                                SELECT CAST(SCOPE_IDENTITY() AS int);";

                            using (var command = new SqlCommand(insertSql, connection, transaction))
                            {
                                AddWarehouseParameters(command, warehouse);
                                warehouseId = Convert.ToInt32(command.ExecuteScalar());
                            }
                        }

                        if (IsPersonalWarehouse(warehouse.LoaiKho) && warehouse.KhoUuTien && !string.IsNullOrWhiteSpace(warehouse.MaNV))
                        {
                            SetPriorityWarehouse(connection, transaction, warehouse.MaNV!, warehouseId);
                        }

                        transaction.Commit();
                        return warehouseId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeactivateWarehouse(int warehouseId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string sql = "UPDATE Warehouses SET IsActive = 0, KhoUuTien = 0 WHERE Id = @Id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", warehouseId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetPriorityWarehouse(string maNV, int warehouseId)
        {
            if (string.IsNullOrWhiteSpace(maNV))
                throw new Exception("Không xác định được nhân viên của kho cá nhân");

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SetPriorityWarehouse(connection, transaction, maNV, warehouseId);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void SetPriorityWarehouse(SqlConnection connection, SqlTransaction transaction, string maNV, int warehouseId)
        {
            string clearSql = @"
                UPDATE Warehouses
                SET KhoUuTien = 0
                WHERE MaNV = @MaNV AND LoaiKho IN ('CANHAN', 'PERSONAL') AND IsActive = 1";

            using (var command = new SqlCommand(clearSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@MaNV", maNV);
                command.ExecuteNonQuery();
            }

            string setSql = @"
                UPDATE Warehouses
                SET KhoUuTien = 1
                WHERE Id = @Id AND MaNV = @MaNV AND LoaiKho IN ('CANHAN', 'PERSONAL') AND IsActive = 1";

            using (var command = new SqlCommand(setSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@Id", warehouseId);
                command.Parameters.AddWithValue("@MaNV", maNV);
                int rows = command.ExecuteNonQuery();
                if (rows == 0)
                {
                    throw new Exception("Kho được chọn không phải kho cá nhân của nhân viên này");
                }
            }
        }

        private static void EnsureUniqueWarehouseCode(SqlConnection connection, SqlTransaction transaction, string maKho, int currentId)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM Warehouses
                WHERE MaKho = @MaKho AND Id <> @Id";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@MaKho", maKho);
                command.Parameters.AddWithValue("@Id", currentId);
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                {
                    throw new Exception($"Mã kho {maKho} đã tồn tại");
                }
            }
        }

        private static void AddWarehouseParameters(SqlCommand command, Warehouse warehouse)
        {
            command.Parameters.AddWithValue("@MaKho", warehouse.MaKho.Trim());
            command.Parameters.AddWithValue("@TenKho", warehouse.TenKho.Trim());
            command.Parameters.AddWithValue("@LoaiKho", NormalizeWarehouseType(warehouse.LoaiKho));
            command.Parameters.AddWithValue("@MaNV", string.IsNullOrWhiteSpace(warehouse.MaNV) ? DBNull.Value : warehouse.MaNV.Trim());
            command.Parameters.AddWithValue("@DiaChi", string.IsNullOrWhiteSpace(warehouse.DiaChi) ? DBNull.Value : warehouse.DiaChi.Trim());
            command.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(warehouse.GhiChu) ? DBNull.Value : warehouse.GhiChu.Trim());
            command.Parameters.AddWithValue("@KhoUuTien", warehouse.KhoUuTien);
        }

        private static Warehouse ReadWarehouse(SqlDataReader reader)
        {
            return new Warehouse
            {
                Id = Convert.ToInt32(reader["Id"]),
                MaKho = reader["MaKho"].ToString() ?? string.Empty,
                TenKho = reader["TenKho"].ToString() ?? string.Empty,
                LoaiKho = reader["LoaiKho"].ToString() ?? string.Empty,
                MaNV = reader["MaNV"] == DBNull.Value ? null : reader["MaNV"].ToString(),
                TenNV = reader["TenNV"] == DBNull.Value ? null : reader["TenNV"].ToString(),
                DiaChi = reader["DiaChi"] == DBNull.Value ? null : reader["DiaChi"].ToString(),
                GhiChu = reader["GhiChu"] == DBNull.Value ? null : reader["GhiChu"].ToString(),
                KhoUuTien = reader["KhoUuTien"] != DBNull.Value && Convert.ToBoolean(reader["KhoUuTien"]),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
            };
        }

        private static bool IsPersonalWarehouse(string loaiKho)
        {
            return string.Equals(loaiKho, "CANHAN", StringComparison.OrdinalIgnoreCase)
                || string.Equals(loaiKho, "PERSONAL", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeWarehouseType(string loaiKho)
        {
            if (IsPersonalWarehouse(loaiKho))
                return "CANHAN";

            return "COMPANY";
        }
        
    }
}
