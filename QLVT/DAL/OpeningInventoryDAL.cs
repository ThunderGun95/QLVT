using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class OpeningInventoryDAL
    {
        /// <summary>
        /// Lấy danh sách tồn kho đầu kỳ
        /// </summary>
        /// <param name="maKho">Mã kho (null = tất cả)</param>
        /// <returns>Danh sách tồn kho đầu kỳ</returns>
        public List<OpeningInventory> GetOpeningInventories(string? maKho = null)
        {
            var inventories = new List<OpeningInventory>();
            
            string sql = @"
                SELECT oi.Id, oi.MaKho, oi.SupplyId, oi.SoLuongTonDauKy, 
                       oi.NgayNhapTon, oi.NguoiNhap, oi.GhiChu,
                       w.TenKho, s.Code as CodeVatTu, s.TenVatTu, u.TenDVT as DonViTinh
                FROM OpeningInventory oi
                LEFT JOIN Warehouses w ON oi.MaKho = w.MaKho
                LEFT JOIN Supplies s ON oi.SupplyId = s.Id
                LEFT JOIN Units u ON s.MaDVT = u.MaDVT
                WHERE (@maKho IS NULL OR oi.MaKho = @maKho)
                ORDER BY w.TenKho, s.TenVatTu";

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@maKho", (object?)maKho ?? DBNull.Value);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inventories.Add(new OpeningInventory
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    MaKho = reader["MaKho"].ToString() ?? string.Empty,
                                    SupplyId = Convert.ToInt32(reader["SupplyId"]),
                                    SoLuongTonDauKy = Convert.ToInt32(reader["SoLuongTonDauKy"]),
                                    NgayNhapTon = Convert.ToDateTime(reader["NgayNhapTon"]),
                                    NguoiNhap = reader["NguoiNhap"].ToString() ?? string.Empty,
                                    GhiChu = reader["GhiChu"].ToString() ?? string.Empty,
                                    TenKho = reader["TenKho"].ToString(),
                                    CodeVatTu = reader["CodeVatTu"].ToString(),
                                    TenVatTu = reader["TenVatTu"].ToString(),
                                    DonViTinh = reader["DonViTinh"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy tồn kho đầu kỳ: {ex.Message}");
            }

            return inventories;
        }

        /// <summary>
        /// Thêm/cập nhật tồn kho đầu kỳ
        /// </summary>
        /// <param name="openingInventories">Danh sách tồn kho cần cập nhật</param>
        /// <param name="nguoiNhap">Người thực hiện</param>
        /// <returns>Số bản ghi đã xử lý</returns>
        public int UpdateOpeningInventories(List<OpeningInventoryInput> openingInventories, string nguoiNhap)
        {
            int processedCount = 0;

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in openingInventories.Where(x => x.IsMapped && x.SoLuong > 0))
                            {
                                // Kiểm tra đã tồn tại chưa
                                string checkSql = @"
                                    SELECT Id FROM OpeningInventory 
                                    WHERE MaKho = @maKho AND SupplyId = @supplyId";
                                
                                using (var checkCmd = new SqlCommand(checkSql, connection, transaction))
                                {
                                    checkCmd.Parameters.AddWithValue("@maKho", item.MaKho);
                                    checkCmd.Parameters.AddWithValue("@supplyId", item.SupplyId!.Value);
                                    
                                    var existingId = checkCmd.ExecuteScalar();
                                    
                                    if (existingId != null)
                                    {
                                        // Cập nhật
                                        string updateSql = @"
                                            UPDATE OpeningInventory 
                                            SET SoLuongTonDauKy = @soLuong, 
                                                NgayNhapTon = GETDATE(),
                                                NguoiNhap = @nguoiNhap,
                                                GhiChu = @ghiChu
                                            WHERE Id = @id";
                                        
                                        using (var updateCmd = new SqlCommand(updateSql, connection, transaction))
                                        {
                                            updateCmd.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                            updateCmd.Parameters.AddWithValue("@nguoiNhap", nguoiNhap);
                                            updateCmd.Parameters.AddWithValue("@ghiChu", item.GhiChu);
                                            updateCmd.Parameters.AddWithValue("@id", existingId);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Thêm mới
                                        string insertSql = @"
                                            INSERT INTO OpeningInventory 
                                            (MaKho, SupplyId, SoLuongTonDauKy, NgayNhapTon, NguoiNhap, GhiChu)
                                            VALUES (@maKho, @supplyId, @soLuong, GETDATE(), @nguoiNhap, @ghiChu)";
                                        
                                        using (var insertCmd = new SqlCommand(insertSql, connection, transaction))
                                        {
                                            insertCmd.Parameters.AddWithValue("@maKho", item.MaKho);
                                            insertCmd.Parameters.AddWithValue("@supplyId", item.SupplyId!.Value);
                                            insertCmd.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                            insertCmd.Parameters.AddWithValue("@nguoiNhap", nguoiNhap);
                                            insertCmd.Parameters.AddWithValue("@ghiChu", item.GhiChu);
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                // Cập nhật Inventory table
                                string inventorySql = @"
                                    IF EXISTS (SELECT 1 FROM Inventory WHERE MaKho = @maKho AND ErpId = @erpId)
                                        UPDATE Inventory SET SoLuongTon = @soLuong WHERE MaKho = @maKho AND ErpId = @erpId
                                    ELSE
                                        INSERT INTO Inventory (MaKho, ErpId, SoLuongTon, SoLuongNo) 
                                        VALUES (@maKho, @erpId, @soLuong, 0)";
                                
                                using (var inventoryCmd = new SqlCommand(inventorySql, connection, transaction))
                                {
                                    inventoryCmd.Parameters.AddWithValue("@maKho", item.MaKho);
                                    inventoryCmd.Parameters.AddWithValue("@erpId", item.SupplyId!.Value);
                                    inventoryCmd.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                    inventoryCmd.ExecuteNonQuery();
                                }

                                processedCount++;
                            }

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
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật tồn kho đầu kỳ: {ex.Message}");
            }

            return processedCount;
        }

        /// <summary>
        /// Xóa tồn kho đầu kỳ
        /// </summary>
        /// <param name="id">ID tồn kho</param>
        public void DeleteOpeningInventory(int id)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Lấy thông tin trước khi xóa
                            string getSql = "SELECT MaKho, SupplyId FROM OpeningInventory WHERE Id = @id";
                            string maKho = "";
                            int supplyId = 0;
                            
                            using (var getCmd = new SqlCommand(getSql, connection, transaction))
                            {
                                getCmd.Parameters.AddWithValue("@id", id);
                                using (var reader = getCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        maKho = reader["MaKho"].ToString() ?? "";
                                        supplyId = Convert.ToInt32(reader["SupplyId"]);
                                    }
                                }
                            }

                            // Xóa OpeningInventory
                            string deleteSql = "DELETE FROM OpeningInventory WHERE Id = @id";
                            using (var deleteCmd = new SqlCommand(deleteSql, connection, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@id", id);
                                deleteCmd.ExecuteNonQuery();
                            }

                            // Reset Inventory về 0
                            string resetSql = "UPDATE Inventory SET SoLuongTon = 0 WHERE MaKho = @maKho AND ErpId = @erpId";
                            using (var resetCmd = new SqlCommand(resetSql, connection, transaction))
                            {
                                resetCmd.Parameters.AddWithValue("@maKho", maKho);
                                resetCmd.Parameters.AddWithValue("@erpId", supplyId);
                                resetCmd.ExecuteNonQuery();
                            }

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
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa tồn kho đầu kỳ: {ex.Message}");
            }
        }
    }
}
