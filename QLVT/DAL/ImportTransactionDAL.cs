using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class ImportTransactionDAL
    {
        /// <summary>
        /// Tạo transaction nhập kho và cập nhật inventory
        /// </summary>
        /// <param name="order">Phiếu nhập từ ERP</param>
        /// <param name="warehouseId">ID kho đích</param>
        /// <param name="createdBy">Người tạo</param>
        /// <param name="staffCode">Mã nhân viên thực hiện</param>
        /// <returns>ID transaction vừa tạo</returns>
        public int CreateImportTransaction(ERPImportOrder order, int warehouseId, string createdBy, string staffCode)
        {
            int transactionId = 0;
            
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo transaction header
                        string insertTransactionSql = @"
                            INSERT INTO Transactions 
                            (MaGiaoDich, LoaiGiaoDich, NgayGiaoDich, WarehouseId, MaNV, 
                             GhiChu, TrangThai, CreatedBy, EntityNhapKho)
                            VALUES 
                            (@maGiaoDich, 'NhapKho', @ngayGiaoDich, @warehouseId, @maNV, 
                             @ghiChu, 'HoanThanh', @createdBy, @entityNhapKho);
                            SELECT SCOPE_IDENTITY();";

                        string maGiaoDich = GenerateTransactionCode();
                        
                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@maGiaoDich", maGiaoDich);
                            command.Parameters.AddWithValue("@ngayGiaoDich", DateTime.Now);
                            command.Parameters.AddWithValue("@warehouseId", warehouseId);
                            command.Parameters.AddWithValue("@maNV", staffCode);
                            command.Parameters.AddWithValue("@ghiChu", $"Nhập kho từ phiếu ERP: {order.SoPhieuNhapKho}-{order.NAM} - {order.TenKho}");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityNhapKho", $"{order.SoPhieuNhapKho}-{order.NAM}");
                            
                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Tạo transaction details và cập nhật inventory
                        foreach (var detail in order.ChiTiet.Where(d => d.IsMapped))
                        {
                            // Insert transaction detail
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails (TransactionId, SupplyId, SoLuong)
                                VALUES (@transactionId, @supplyId, @soLuong)";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@supplyId", detail.MappedSupplyId!.Value);
                                command.Parameters.AddWithValue("@soLuong", detail.SoLuongNhapKho);
                                
                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory
                            UpdateInventory(connection, transaction, warehouseId, detail.MappedSupplyId!.Value, detail.SoLuongNhapKho);
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

            return transactionId;
        }

        /// <summary>
        /// Cập nhật inventory (tồn kho)
        /// </summary>
        private void UpdateInventory(SqlConnection connection, SqlTransaction transaction, int warehouseId, int supplyId, decimal quantity)
        {
            string sql = @"
                IF EXISTS (SELECT 1 FROM Inventory WHERE WarehouseId = @warehouseId AND SupplyId = @supplyId)
                BEGIN
                    UPDATE Inventory 
                    SET SoLuongTon = SoLuongTon + @quantity, LastUpdated = GETDATE()
                    WHERE WarehouseId = @warehouseId AND SupplyId = @supplyId
                END
                ELSE
                BEGIN
                    INSERT INTO Inventory (WarehouseId, SupplyId, SoLuongTon, LastUpdated)
                    VALUES (@warehouseId, @supplyId, @quantity, GETDATE())
                END";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@warehouseId", warehouseId);
                command.Parameters.AddWithValue("@supplyId", supplyId);
                command.Parameters.AddWithValue("@quantity", quantity);
                
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Tạo mã giao dịch tự động
        /// </summary>
        private string GenerateTransactionCode()
        {
            return $"NK{DateTime.Now:yyyyMMddHHmmss}";
        }

        /// <summary>
        /// Tạo transaction tồn kho đầu kỳ
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <param name="items">Danh sách vật tư</param>
        /// <param name="nguoiTao">Người tạo</param>
        /// <returns>ID transaction đã tạo</returns>
        public int CreateOpeningInventoryTransaction(int maKho, List<OpeningInventoryInput> items, string nguoiTao)
        {
            int transactionId = 0;
            
            // Debug: Log thông tin đầu vào
            System.Diagnostics.Debug.WriteLine($"DEBUG: CreateOpeningInventoryTransaction - MaKho: '{maKho}', Items: {items.Count}, NguoiTao: '{nguoiTao}'");
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Tạo số phiếu
                            string soPhieu = GenerateOpeningInventoryTransactionNumber();
                            
                            // 1. Tạo transaction header theo schema mới
                            string insertTransactionSql = @"
                                INSERT INTO Transactions 
                                (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNhan, MaNV, GhiChu, CreatedBy, EntityNhapKho)
                                VALUES 
                                (@soPhieu, @ngayGiaoDich, 'NhapKho', @maKhoNhan, @maNV, @ghiChu, @createdBy, @entityNhapKho);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@soPhieu", soPhieu);
                                command.Parameters.AddWithValue("@ngayGiaoDich", DateTime.Now);
                                command.Parameters.AddWithValue("@maKhoNhan", maKho);
                                command.Parameters.AddWithValue("@maNV", nguoiTao);
                                command.Parameters.AddWithValue("@ghiChu", "Nhập tồn kho đầu kỳ từ Excel");
                                command.Parameters.AddWithValue("@createdBy", nguoiTao);
                                command.Parameters.AddWithValue("@entityNhapKho", "Import tồn đầu kỳ từ Excel");

                                transactionId = Convert.ToInt32(command.ExecuteScalar());
                            }

                            // 2. Tạo transaction details
                            foreach (var item in items.Where(x => x.IsMapped && x.SoLuong > 0))
                            {
                                string insertDetailSql = @"
                                    INSERT INTO TransactionDetails 
                                    (TransactionId, ErpId, SoLuong, GhiChu)
                                    VALUES (@transactionId, @erpId, @soLuong, @ghiChu)";

                                using (var detailCommand = new SqlCommand(insertDetailSql, connection, transaction))
                                {
                                    detailCommand.Parameters.AddWithValue("@transactionId", transactionId);
                                    detailCommand.Parameters.AddWithValue("@erpId", item.SupplyId!.Value);
                                    detailCommand.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                    detailCommand.Parameters.AddWithValue("@ghiChu", item.GhiChu ?? "Tồn đầu kỳ");

                                    detailCommand.ExecuteNonQuery();
                                }

                                // 3. Cập nhật bảng Inventory
                                string updateInventorySql = @"
                                    IF EXISTS (SELECT 1 FROM Inventory WHERE WarehouseId = @maKho AND SupplyErpId = @erpId)
                                        UPDATE Inventory SET SoLuongTon = SoLuongTon + @soLuong, LastUpdated = GETDATE()
                                        WHERE WarehouseId = @maKho AND SupplyErpId = @erpId
                                    ELSE
                                        INSERT INTO Inventory (WarehouseId, SupplyErpId, SoLuongTon, LastUpdated)
                                        VALUES (@maKho, @erpId, @soLuong, GETDATE())";

                                using (var inventoryCommand = new SqlCommand(updateInventorySql, connection, transaction))
                                {
                                    inventoryCommand.Parameters.AddWithValue("@maKho", maKho);
                                    inventoryCommand.Parameters.AddWithValue("@erpId", item.SupplyId!.Value);
                                    inventoryCommand.Parameters.AddWithValue("@soLuong", item.SoLuong);

                                    inventoryCommand.ExecuteNonQuery();
                                }
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
                throw new Exception($"Lỗi tạo transaction tồn đầu kỳ: {ex.Message}");
            }

            return transactionId;
        }

        /// <summary>
        /// Tạo số phiếu cho transaction tồn đầu kỳ
        /// </summary>
        private string GenerateOpeningInventoryTransactionNumber()
        {
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            string prefix = "TDK"; // Tồn Đầu Kỳ
            
            try
            {
                // Đếm số phiếu tồn đầu kỳ trong ngày
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string countSql = @"
                        SELECT COUNT(*) FROM Transactions 
                        WHERE SoPhieu LIKE @pattern 
                        AND CAST(NgayGiaoDich AS DATE) = CAST(GETDATE() AS DATE)";
                    
                    using (var command = new SqlCommand(countSql, connection))
                    {
                        command.Parameters.AddWithValue("@pattern", $"{prefix}{dateStr}%");
                        int count = Convert.ToInt32(command.ExecuteScalar()) + 1;
                        
                        return $"{prefix}{dateStr}{count:D3}";
                    }
                }
            }
            catch
            {
                // Fallback nếu có lỗi
                Random random = new Random();
                return $"{prefix}{dateStr}{random.Next(100, 999)}";
            }
        }
    }
}
