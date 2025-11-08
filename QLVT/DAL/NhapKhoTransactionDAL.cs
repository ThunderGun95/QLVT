using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;
using QLVT.ERP.Models;

namespace QLVT.DAL
{
    public class NhapKhoTransactionDAL
    {
        /// <summary>
        /// Tạo transaction nhập kho và cập nhật inventory
        /// </summary>
        /// <param name="order">Phiếu nhập từ ERP</param>
        /// <param name="maKho">Mã kho đích</param>
        /// <param name="createdBy">Người tạo</param>
        /// <returns>ID transaction vừa tạo</returns>
        public int CreateNhapKhoErpTransaction(ERP_PhieuNhapKho order, int maKho, string createdBy)
        {
            int transactionId = 0;
            
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Tạo số phiếu
                        string soPhieu = GenerateNhapKhoTransactionNumber();
                        
                        // 1. Tạo transaction header theo schema mới
                        string insertTransactionSql = @"
                            INSERT INTO Transactions 
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNhan, GhiChu, CreatedBy, EntityNhapKho)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'NhapKho', @maKhoNhan, @ghiChu, @createdBy, @entityNhapKho);
                            SELECT SCOPE_IDENTITY();";

                        
                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", order.ThoiGianHoanThanhNhapKho);
                            command.Parameters.AddWithValue("@maKhoNhan", maKho);
                            command.Parameters.AddWithValue("@ghiChu", $"Nhập kho từ phiếu ERP: {order.SoPhieuNhapKho}-{order.NAM} - {order.TenKho}");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityNhapKho", $"NK:{order.SoPhieuNhapKho}-{order.NAM}");
                            
                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Tạo transaction details và cập nhật inventory
                        foreach (var detail in order.ChiTiet.Where(d => d.IsMapped))
                        {
                            // Insert transaction detail theo schema mới
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails (TransactionId, ErpId, MaKhoNhap, SoLuong, GhiChu, CreatedBy, CreatedDate)
                                VALUES (@transactionId, @erpId, @maKhoNhap, @soLuong, @ghiChu, @createdBy, GETDATE())";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@maKhoNhap", maKho);
                                command.Parameters.AddWithValue("@erpId", detail.MappedSupplyId!.Value);
                                command.Parameters.AddWithValue("@soLuong", detail.SoLuongNhapKho);
                                command.Parameters.AddWithValue("@ghiChu", $"Nhập ERP kho {order.TenKho}, phiếu({order.SoPhieuNhapKho}-{order.NAM})");
                                command.Parameters.AddWithValue("@createdBy", createdBy);

                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory theo schema mới
                            CapNhatTonKho(connection, transaction, maKho, detail.MappedSupplyId!.Value, detail.SoLuongNhapKho);
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
        /// Cập nhật inventory (tồn kho) theo schema mới
        /// </summary>
        private void CapNhatTonKho(SqlConnection connection, SqlTransaction transaction, int maKho, int erpId, decimal quantity)
        {
            string sql = @"
                IF EXISTS (SELECT 1 FROM Inventory WHERE WarehouseId = @maKho AND SupplyErpId = @erpId)
                BEGIN
                    UPDATE Inventory 
                    SET SoLuongTon = SoLuongTon + @quantity, LastUpdated = GETDATE()
                    WHERE WarehouseId = @maKho AND SupplyErpId = @erpId
                END
                ELSE
                BEGIN
                    INSERT INTO Inventory (WarehouseId, SupplyErpId, SoLuongTon, LastUpdated)
                    VALUES (@maKho, @erpId, @quantity, GETDATE())
                END";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@maKho", maKho);
                command.Parameters.AddWithValue("@erpId", erpId);
                command.Parameters.AddWithValue("@quantity", Math.Round(quantity,2));
                
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Tạo số phiếu cho transaction nhập kho
        /// </summary>
        private string GenerateNhapKhoTransactionNumber()
        {
            string dateStr = DateTime.Now.ToString("yyMMdd");
            string prefix = "NK"; // Nhập Kho
            
            try
            {
                // Đếm số phiếu nhập kho trong ngày
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string countSql = @"
                        SELECT COUNT(*) FROM Transactions 
                        WHERE SoPhieu LIKE @pattern 
                        AND LoaiGiaoDich = 'NhapKho'
                        AND CAST(CreatedDate AS DATE) = CAST(GETDATE() AS DATE)";
                    
                    using (var command = new SqlCommand(countSql, connection))
                    {
                        command.Parameters.AddWithValue("@pattern", $"{prefix}{dateStr}%");
                        int count = Convert.ToInt32(command.ExecuteScalar()) + 1;
                        
                        return $"{prefix}{dateStr}-{count:D4}";
                    }
                }
            }
            catch
            {
                // Fallback nếu có lỗi
                Random random = new Random();
                return $"{prefix}{dateStr}-{random.Next(1000, 9999)}";
            }
        }


        /// <summary>
        /// Tạo transaction nhập kho không qua ERP
        /// </summary>
        public int CreateNhapKhoManualTransaction(PhieuNhapKho transaction, List<PhieuNhapKhoChiTiet> details, string createdBy, int warehouseId = 1)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var dbTransaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Tạo số phiếu
                            string soPhieu = GenerateNhapKhoTransactionNumber();

                            // 2. Tạo transaction header
                            var insertTransactionSql = @"
                                INSERT INTO Transactions 
                                (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNhan, GhiChu, CreatedBy, CreatedDate)
                                VALUES 
                                (@soPhieu, @ngayGiaoDich, @loaiGiaoDich, @maKhoNhan, @ghiChu, @createdBy, @createdDate);
                                SELECT SCOPE_IDENTITY();";

                            int transactionId;
                            using (var command = new SqlCommand(insertTransactionSql, connection, dbTransaction))
                            {
                                command.Parameters.AddWithValue("@soPhieu", soPhieu);
                                command.Parameters.AddWithValue("@ngayGiaoDich", transaction.NgayGiaoDich);
                                command.Parameters.AddWithValue("@loaiGiaoDich", "NhapKho");
                                command.Parameters.AddWithValue("@maKhoNhan", warehouseId);
                                command.Parameters.AddWithValue("@ghiChu", transaction.GhiChu ?? "");
                                command.Parameters.AddWithValue("@createdBy", createdBy);
                                command.Parameters.AddWithValue("@createdDate", DateTime.Now);

                                transactionId = Convert.ToInt32(command.ExecuteScalar());
                            }

                            // 3. Tạo transaction details và cập nhật inventory
                            foreach (var detail in details)
                            {
                                // Insert transaction detail
                                string insertDetailSql = @"
                                    INSERT INTO TransactionDetails 
                                    (TransactionId, ErpId, SoLuong, GhiChu, CreatedBy, CreatedDate)
                                    VALUES (@transactionId, @erpId, @soLuong, @ghiChu, @createdBy, GETDATE())";

                                using (var detailCommand = new SqlCommand(insertDetailSql, connection, dbTransaction))
                                {
                                    detailCommand.Parameters.AddWithValue("@transactionId", transactionId);
                                    detailCommand.Parameters.AddWithValue("@erpId", detail.ErpId);
                                    detailCommand.Parameters.AddWithValue("@soLuong", detail.SoLuong);
                                    detailCommand.Parameters.AddWithValue("@ghiChu", detail.GhiChu ?? "");
                                    detailCommand.Parameters.AddWithValue("@createdBy", createdBy);

                                    detailCommand.ExecuteNonQuery();
                                }

                                // Cập nhật tồn kho
                                CapNhatTonKho(connection, dbTransaction, warehouseId, detail.ErpId, detail.SoLuong);
                            }

                            dbTransaction.Commit();
                            return transactionId;
                        }
                        catch
                        {
                            dbTransaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tạo transaction nhập kho manual: {ex.Message}", ex);
            }
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
                                    (TransactionId, ErpId, SoLuong, GhiChu, CreatedBy)
                                    VALUES (@transactionId, @erpId, @soLuong, @ghiChu, @createdBy)";

                                using (var detailCommand = new SqlCommand(insertDetailSql, connection, transaction))
                                {
                                    detailCommand.Parameters.AddWithValue("@transactionId", transactionId);
                                    detailCommand.Parameters.AddWithValue("@erpId", item.SupplyId!.Value);
                                    detailCommand.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                    detailCommand.Parameters.AddWithValue("@ghiChu", item.GhiChu ?? "Tồn đầu kỳ");
                                    detailCommand.Parameters.AddWithValue("@createdBy", nguoiTao);

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
