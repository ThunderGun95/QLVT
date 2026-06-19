using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;
using System.Threading.Tasks;

namespace QLVT.DAL
{
    public class InventoryDAL
    {
        /// <summary>
        /// Lấy thông tin tồn kho của một vật tư trong một kho
        /// </summary>
        public async Task<Inventory?> GetInventoryAsync(long warehouseCode, long supplyCode)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        SELECT Id, WarehouseId, SupplyErpId, SoLuongTon, LastUpdated 
                        FROM Inventory 
                        WHERE WarehouseId = @MaKho AND SupplyErpId = @MaVatTu";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKho", warehouseCode);
                        command.Parameters.AddWithValue("@MaVatTu", supplyCode);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Inventory
                                {
                                    Id = reader.GetInt32(0),
                                    WarehouseId = reader.GetInt32(1),
                                    SupplyErpId = reader.GetInt32(2),
                                    SoLuongTon = reader.GetDecimal(3),
                                    LastUpdated = reader.GetDateTime(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy thông tin tồn kho {warehouseCode}-{supplyCode}: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Cập nhật thông tin tồn kho
        /// </summary>
        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        UPDATE Inventory 
                        SET SoLuongTon = @SoLuongTon, LastUpdated = @NgayCapNhat 
                        WHERE Id = @Id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", inventory.Id);
                        command.Parameters.AddWithValue("@SoLuongTon", inventory.SoLuongTon);
                        command.Parameters.AddWithValue("@NgayCapNhat", inventory.LastUpdated);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật inventory {inventory.WarehouseId}-{inventory.SupplyErpId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm mới record tồn kho
        /// </summary>
        public async Task AddInventoryAsync(Inventory inventory)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        INSERT INTO Inventory (WarehouseId, SupplyErpId, SoLuongTon, LastUpdated)
                        VALUES (@MaKho, @MaVatTu, @SoLuongTon, @NgayCapNhat)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKho", inventory.WarehouseId);
                        command.Parameters.AddWithValue("@MaVatTu", inventory.SupplyErpId);
                        command.Parameters.AddWithValue("@SoLuongTon", inventory.SoLuongTon);
                        command.Parameters.AddWithValue("@NgayCapNhat", inventory.LastUpdated);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi thêm inventory {inventory.WarehouseId}-{inventory.SupplyErpId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách tồn kho theo kho
        /// </summary>
        public async Task<List<Inventory>> GetInventoryByWarehouseAsync(string warehouseCode)
        {
            var inventories = new List<Inventory>();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        SELECT Id, WarehouseId, SupplyErpId, SoLuongTon, LastUpdated 
                        FROM Inventory 
                        WHERE MaKho = @MaKho";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKho", warehouseCode);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                inventories.Add(new Inventory
                                {
                                    Id = reader.GetInt32(0),
                                    WarehouseId = reader.GetInt64(1),
                                    SupplyErpId = reader.GetInt64(2),
                                    SoLuongTon = reader.GetDecimal(3),
                                    LastUpdated = reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách tồn kho cho kho {warehouseCode}: {ex.Message}");
            }
            return inventories;
        }

        /// <summary>
        /// Lấy tổng tồn kho của một vật tư (tất cả các kho)
        /// </summary>
        public decimal GetTonKhoByErpId(int supplyErpId, string manv)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    var query = @"
                        SELECT ISNULL(SUM(TonKho), 0)
                        FROM ViewTonKhoVatTu 
                        WHERE SupplyErpId = @SupplyErpId AND MaNV = @manv";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplyErpId", supplyErpId);
                        command.Parameters.AddWithValue("@manv", manv);
                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy tồn kho cho vật tư ErpId {supplyErpId}: {ex.Message}");
            }
        }


        /// <summary>
        /// Lấy chi tiết tồn kho của một vật tư
        /// </summary>
        public async Task<List<Inventory>> GetListTonKhoChiTietByErpId(int supplyErpId, string manv)
        {
            var inventories = new List<Inventory>();
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = @"
                        SELECT Id, WarehouseId, SupplyErpId, TonKho
                        FROM ViewTonKhoVatTu 
                        WHERE SupplyErpId = @SupplyErpId AND MaNV = @manv AND TonKho > 0
                        ORDER BY KhoUuTien desc";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 15;
                        command.Parameters.Add("@SupplyErpId", System.Data.SqlDbType.Int).Value = supplyErpId;
                        command.Parameters.Add("@manv", System.Data.SqlDbType.VarChar, 50).Value = manv;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                inventories.Add(new Inventory
                                {
                                    Id = Convert.ToInt64(reader["Id"]),
                                    WarehouseId = Convert.ToInt64(reader["WarehouseId"]),
                                    SupplyErpId = Convert.ToInt64(reader["SupplyErpId"]),
                                    SoLuongTon = reader.GetDecimal(3)
                                });
                            }
                        }
                    }
                }
                return inventories;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy chi tiết tồn kho cho vật tư ErpId {supplyErpId}: {ex.Message}");
            }
        }
    }
}
