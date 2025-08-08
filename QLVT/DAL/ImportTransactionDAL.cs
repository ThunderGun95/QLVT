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
    }
}
