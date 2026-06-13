using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    /// <summary>
    /// Data Access Layer cho import phiếu nhập kho từ Bravo
    /// </summary>
    public class BravoNhapKhoDAL
    {
        /// <summary>
        /// Kiểm tra phiếu đã được xử lý chưa (dựa vào SoPhieu trong bảng Transactions)
        /// </summary>
        public bool IsPhieuProcessed(string soPhieu)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT COUNT(*) 
                    FROM Transactions 
                    WHERE SoPhieu = @soPhieu";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@soPhieu", soPhieu);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Tạo transaction nhập kho từ dữ liệu Bravo
        /// </summary>
        /// <param name="soPhieu">Số phiếu (đã thêm prefix NK- nếu yêu cầu)</param>
        /// <param name="ngayGiaoDich">Ngày giao dịch</param>
        /// <param name="noiDung">Nội dung phiếu</param>
        /// <param name="khoNhanId">Kho nhận</param>
        /// <param name="items">Danh sách chi tiết vật tư</param>
        /// <param name="createdBy">Người tạo</param>
        /// <returns>ID transaction vừa tạo</returns>
        public int CreateNhapKhoTransaction(
            string soPhieu,
            DateTime ngayGiaoDich,
            string noiDung,
            int khoNhanId,
            List<BravoNhapKhoExcelItem> items,
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
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNhan, GhiChu, CreatedBy, EntityNhapKho)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'NhapKho', @maKhoNhan, @ghiChu, @createdBy, @entityNhapKho);
                            SELECT SCOPE_IDENTITY();";

                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", ngayGiaoDich);
                            command.Parameters.AddWithValue("@maKhoNhan", khoNhanId);
                            command.Parameters.AddWithValue("@ghiChu", $"[Bravo NK] {noiDung}");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityNhapKho", $"BRAVO-NK:{soPhieu}");

                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Thêm TransactionDetails và cập nhật Inventory
                        foreach (var item in items)
                        {
                            // Insert transaction detail
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails 
                                (TransactionId, ErpId, SoLuong, GhiChu, MaKhoNhap, CreatedBy, CreatedDate)
                                VALUES 
                                (@transactionId, @erpId, @soLuong, @ghiChu, @maKhoNhap, @createdBy, GETDATE())";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@erpId", item.MaVatTu);
                                command.Parameters.AddWithValue("@soLuong", item.SoLuong);
                                command.Parameters.AddWithValue("@ghiChu", noiDung);
                                command.Parameters.AddWithValue("@maKhoNhap", khoNhanId);
                                command.Parameters.AddWithValue("@createdBy", createdBy);

                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory: cộng vào kho nhận
                            AddInventory(connection, transaction, khoNhanId, item.MaVatTu, item.SoLuong);
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
        /// Cộng tồn kho vào kho nhận (nếu chưa có thì tạo mới)
        /// </summary>
        private void AddInventory(SqlConnection connection, SqlTransaction transaction, int warehouseId, long supplyErpId, decimal quantity)
        {
            string updateSql = @"
                UPDATE Inventory 
                SET SoLuongTon = SoLuongTon + @soLuong,
                    LastUpdated = GETDATE()
                WHERE WarehouseId = @warehouseId AND SupplyErpId = @erpId";

            using (var command = new SqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@warehouseId", warehouseId);
                command.Parameters.AddWithValue("@erpId", supplyErpId);
                command.Parameters.AddWithValue("@soLuong", Math.Round(quantity, 2));

                int rows = command.ExecuteNonQuery();
                if (rows == 0)
                {
                    string insertSql = @"
                        INSERT INTO Inventory (WarehouseId, SupplyErpId, SoLuongTon, LastUpdated)
                        VALUES (@warehouseId, @erpId, @soLuong, GETDATE())";

                    using (var insertCommand = new SqlCommand(insertSql, connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@warehouseId", warehouseId);
                        insertCommand.Parameters.AddWithValue("@erpId", supplyErpId);
                        insertCommand.Parameters.AddWithValue("@soLuong", Math.Round(quantity, 2));
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
