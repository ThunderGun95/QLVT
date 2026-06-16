using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    /// <summary>
    /// Data Access Layer cho import phiếu từ Bravo
    /// </summary>
    public class BravoImportDAL
    {
        /// <summary>
        /// Kiểm tra phiếu đã được xử lý chưa
        /// </summary>
        public bool IsPhieuProcessed(string soPhieu)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                // Kiểm tra trong Transactions với EntityXuatKho hoặc EntityNhapKho
                string sql = @"
                    SELECT COUNT(*) 
                    FROM Transactions 
                    WHERE SoPhieu = @soPhieu";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@soPhieu", soPhieu);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Tạo transaction xuất kho từ Bravo
        /// Từ kho công ty → Kho cá nhân
        /// </summary>
        public int CreateXuatKhoTransaction(
            string soPhieu,
            DateTime ngayGiaoDich,
            string noiDung,
            int khoXuatId,
            int khoNhapId,
            List<BravoPhieuExcelItem> items,
            string createdBy)
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
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNguon, MaKhoNhan, GhiChu, CreatedBy, EntityXuatKho)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'XuatKho', @maKhoNguon, @maKhoNhan, @ghiChu, @createdBy, @entityXuatKho);
                            SELECT SCOPE_IDENTITY();";

                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", ngayGiaoDich);
                            command.Parameters.AddWithValue("@maKhoNguon", khoXuatId);
                            command.Parameters.AddWithValue("@maKhoNhan", khoNhapId);
                            command.Parameters.AddWithValue("@ghiChu", $"[Bravo Import] {noiDung}");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityXuatKho", $"BRAVO-XK:{soPhieu}");

                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Thêm TransactionDetails và cập nhật Inventory
                        foreach (var item in items)
                        {
                            // Insert transaction detail
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails 
                                (TransactionId, ErpId, SoLuong, GhiChu, MaKhoXuat, MaKhoNhap, CreatedBy, CreatedDate)
                                VALUES 
                                (@transactionId, @erpId, @soLuong, @ghiChu, @maKhoXuat, @maKhoNhap, @createdBy, GETDATE())";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@erpId", item.MaVatTu);
                                command.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                command.Parameters.AddWithValue("@ghiChu", noiDung);
                                command.Parameters.AddWithValue("@maKhoXuat", khoXuatId);
                                command.Parameters.AddWithValue("@maKhoNhap", khoNhapId);
                                command.Parameters.AddWithValue("@createdBy", createdBy);

                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory: Trừ kho xuất, cộng kho nhập
                            TransferInventory(connection, transaction, khoXuatId, khoNhapId, (int)item.MaVatTu, item.SoLuong);
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
        /// Tạo transaction trả kho từ Bravo
        /// Từ kho cá nhân → Kho công ty
        /// </summary>
        public int CreateTraKhoTransaction(
            string soPhieu,
            DateTime ngayGiaoDich,
            string noiDung,
            int khoXuatId,
            int khoNhapId,
            List<BravoPhieuExcelItem> items,
            string createdBy)
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
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNguon, MaKhoNhan, GhiChu, CreatedBy, EntityNhapKho)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'TraKho', @maKhoNguon, @maKhoNhan, @ghiChu, @createdBy, @entityNhapKho);
                            SELECT SCOPE_IDENTITY();";

                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", ngayGiaoDich);
                            command.Parameters.AddWithValue("@maKhoNguon", khoXuatId);
                            command.Parameters.AddWithValue("@maKhoNhan", khoNhapId);
                            command.Parameters.AddWithValue("@ghiChu", $"[Bravo Import] {noiDung}");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityNhapKho", $"BRAVO-XN:{soPhieu}");

                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Thêm TransactionDetails và cập nhật Inventory
                        foreach (var item in items)
                        {
                            // Insert transaction detail
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails 
                                (TransactionId, ErpId, SoLuong, GhiChu, MaKhoXuat, MaKhoNhap, CreatedBy, CreatedDate)
                                VALUES 
                                (@transactionId, @erpId, @soLuong, @ghiChu, @maKhoXuat, @maKhoNhap, @createdBy, GETDATE())";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@erpId", item.MaVatTu);
                                command.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                command.Parameters.AddWithValue("@ghiChu", noiDung);
                                command.Parameters.AddWithValue("@maKhoXuat", khoXuatId);
                                command.Parameters.AddWithValue("@maKhoNhap", khoNhapId);
                                command.Parameters.AddWithValue("@createdBy", createdBy);

                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory: Trừ kho xuất (cá nhân), cộng kho nhập (công ty)
                            TransferInventory(connection, transaction, khoXuatId, khoNhapId, item.MaVatTu, item.SoLuong);
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
        /// Tạo transaction hoàn ứng từ Bravo
        /// CHỈ trừ từ kho cá nhân, KHÔNG nhập vào kho nào
        /// </summary>
        public int CreateHoanUngTransaction(
            string soPhieu,
            DateTime ngayGiaoDich,
            string noiDung,
            int khoXuatId,
            List<BravoPhieuExcelItem> items,
            string createdBy)
        {
            int transactionId = 0;

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo transaction header (KHÔNG có kho nhận)
                        string insertTransactionSql = @"
                            INSERT INTO Transactions 
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNguon, MaKhoNhan, GhiChu, CreatedBy, EntityNhapKho)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'HoanUng', @maKhoNguon, NULL, @ghiChu, @createdBy, @entityNhapKho);
                            SELECT SCOPE_IDENTITY();";

                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", ngayGiaoDich);
                            command.Parameters.AddWithValue("@maKhoNguon", khoXuatId);
                            command.Parameters.AddWithValue("@ghiChu", $"[Bravo Import] {noiDung}");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityNhapKho", $"BRAVO-HU:{soPhieu}");

                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Thêm TransactionDetails và cập nhật Inventory (CHỈ TRỪ KHO XUẤT)
                        foreach (var item in items)
                        {
                            // Insert transaction detail (MaKhoNhap = NULL)
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails 
                                (TransactionId, ErpId, SoLuong, GhiChu, MaKhoXuat, MaKhoNhap, CreatedBy, CreatedDate)
                                VALUES 
                                (@transactionId, @erpId, @soLuong, @ghiChu, @maKhoXuat, NULL, @createdBy, GETDATE())";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@erpId", item.MaVatTu);
                                command.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                command.Parameters.AddWithValue("@ghiChu", noiDung);
                                command.Parameters.AddWithValue("@maKhoXuat", khoXuatId);
                                command.Parameters.AddWithValue("@createdBy", createdBy);

                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory: CHỈ TRỪ kho xuất (kho cá nhân)
                            DeductInventory(connection, transaction, khoXuatId, (int)item.MaVatTu, item.SoLuong);
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
        /// Chuyển inventory giữa các kho
        /// </summary>
        private void TransferInventory(
            SqlConnection connection,
            SqlTransaction transaction,
            int sourceWarehouseId,
            int targetWarehouseId,
            long supplyErpId,
            decimal quantity)
        {
            decimal roundedQuantity = Math.Round(quantity, 2);

            // 1. Trừ tồn kho từ kho nguồn
            string updateSourceSql = @"
                UPDATE Inventory 
                SET SoLuongTon = SoLuongTon - @soLuong,
                    LastUpdated = GETDATE()
                WHERE WarehouseId = @warehouseId AND SupplyErpId = @erpId
                  AND SoLuongTon >= @soLuong";

            using (var command = new SqlCommand(updateSourceSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@warehouseId", sourceWarehouseId);
                command.Parameters.AddWithValue("@erpId", supplyErpId);
                command.Parameters.AddWithValue("@soLuong", roundedQuantity);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception($"Ton kho khong du de xuat kho Bravo. Kho nguon ID={sourceWarehouseId}, VT={supplyErpId}, so luong can xuat={roundedQuantity:N2}");
                }
            }

            // 2. Cộng tồn kho vào kho đích
            string updateTargetSql = @"
                UPDATE Inventory 
                SET SoLuongTon = SoLuongTon + @soLuong,
                    LastUpdated = GETDATE()
                WHERE WarehouseId = @warehouseId AND SupplyErpId = @erpId";

            using (var command = new SqlCommand(updateTargetSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@warehouseId", targetWarehouseId);
                command.Parameters.AddWithValue("@erpId", supplyErpId);
                command.Parameters.AddWithValue("@soLuong", roundedQuantity);

                int rowsAffected = command.ExecuteNonQuery();

                // Nếu không có bản ghi nào được update, tạo mới
                if (rowsAffected == 0)
                {
                    string insertTargetSql = @"
                        INSERT INTO Inventory (WarehouseId, SupplyErpId, SoLuongTon, LastUpdated)
                        VALUES (@warehouseId, @erpId, @soLuong, GETDATE())";

                    using (var insertCommand = new SqlCommand(insertTargetSql, connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@warehouseId", targetWarehouseId);
                        insertCommand.Parameters.AddWithValue("@erpId", supplyErpId);
                        insertCommand.Parameters.AddWithValue("@soLuong", roundedQuantity);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Chỉ trừ inventory từ kho (dùng cho hoàn ứng)
        /// </summary>
        private void DeductInventory(
            SqlConnection connection,
            SqlTransaction transaction,
            int warehouseId,
            int supplyErpId,
            decimal quantity)
        {
            decimal roundedQuantity = Math.Round(quantity, 2);

            // Trừ tồn kho từ kho
            string updateSql = @"
                UPDATE Inventory 
                SET SoLuongTon = SoLuongTon - @soLuong,
                    LastUpdated = GETDATE()
                WHERE WarehouseId = @warehouseId AND SupplyErpId = @erpId
                  AND SoLuongTon >= @soLuong";

            using (var command = new SqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@warehouseId", warehouseId);
                command.Parameters.AddWithValue("@erpId", supplyErpId);
                command.Parameters.AddWithValue("@soLuong", roundedQuantity);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception($"Ton kho khong du de hoan ung Bravo. Kho ID={warehouseId}, VT={supplyErpId}, so luong can tru={roundedQuantity:N2}");
                }
            }
        }
    }
}
