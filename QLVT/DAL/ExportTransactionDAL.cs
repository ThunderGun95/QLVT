using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class ExportTransactionDAL
    {
        /// <summary>
        /// Tạo transaction xuất kho với chi tiết từng dòng (nhiều kho nguồn khác nhau)
        /// </summary>
        /// <param name="order">Phiếu xuất từ ERP</param>
        /// <param name="employeeWarehouseId">ID kho nhân viên đích</param>
        /// <param name="createdBy">Người tạo</param>
        /// <param name="staffCode">Mã nhân viên thực hiện</param>
        /// <returns>ID transaction vừa tạo</returns>
        public int CreateExportTransaction(ERPExportOrder order, int employeeWarehouseId, string createdBy, string staffCode)
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
                        string soPhieu = GenerateExportTransactionNumber();
                        
                        // 1. Tạo transaction header (không có kho nguồn cố định vì mỗi detail có kho riêng)
                        string insertTransactionSql = @"
                            INSERT INTO Transactions 
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNguon, MaKhoNhan, MaNV, 
                             GhiChu, CreatedBy, EntityXuatKho)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'XuatKho', NULL, @maKhoNhan, @maNV, 
                             @ghiChu, @createdBy, @entityXuatKho);
                            SELECT SCOPE_IDENTITY();";

                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", order.ThoiGianHoanThanhXuatKho);
                            command.Parameters.AddWithValue("@maKhoNhan", employeeWarehouseId);
                            command.Parameters.AddWithValue("@maNV", staffCode);
                            command.Parameters.AddWithValue("@ghiChu", $"Xuất kho từ phiếu ERP: {order.SoPhieuXuatKho}-{order.NAM} - {order.TenNhanVien} (Nhiều kho nguồn)");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityXuatKho", $"{order.SoPhieuXuatKho}-{order.NAM}");
                            
                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Xử lý từng detail riêng biệt với kho nguồn riêng
                        foreach (var detail in order.ChiTiet.Where(d => d.IsMapped))
                        {
                            // Validate kho nguồn phải có
                            if (!detail.SourceWarehouseId.HasValue)
                                throw new Exception($"Chi tiết vật tư {detail.MaVatTuHangHoa} chưa xác định được kho nguồn");

                            int sourceWarehouseId = detail.SourceWarehouseId.Value;

                            // Insert transaction detail với SourceWarehouseId
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails (TransactionId, ErpId, SoLuong, GhiChu, SourceWarehouseId)
                                VALUES (@transactionId, @erpId, @soLuong, @ghiChu, @sourceWarehouseId)";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@erpId", detail.MappedSupplyId!.Value);
                                command.Parameters.AddWithValue("@soLuong", detail.SoLuongXuatKho);
                                command.Parameters.AddWithValue("@ghiChu", $"Xuất từ kho {detail.TenKhoXuat} ({detail.MaKhoXuat}) - VT: {detail.MaVatTuHangHoa}");
                                command.Parameters.AddWithValue("@sourceWarehouseId", sourceWarehouseId);
                                
                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory: Trừ từ kho nguồn cụ thể, cộng vào kho nhân viên
                            TransferInventory(connection, transaction, sourceWarehouseId, employeeWarehouseId, detail.MappedSupplyId!.Value, (int)detail.SoLuongXuatKho);
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
        /// Chuyển inventory từ kho nguồn cụ thể sang kho nhân viên đích
        /// (Mỗi detail có thể có kho nguồn khác nhau)
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="sourceWarehouseId">ID kho nguồn (từ mapping ERP)</param>
        /// <param name="targetWarehouseId">ID kho nhân viên đích</param>
        /// <param name="erpId">ERP ID vật tư</param>
        /// <param name="quantity">Số lượng chuyển</param>
        private void TransferInventory(SqlConnection connection, SqlTransaction transaction, int sourceWarehouseId, int targetWarehouseId, int erpId, int quantity)
        {
            // Kiểm tra tồn kho đủ không
            string checkStockSql = @"
                SELECT ISNULL(SoLuongTon, 0) FROM Inventory 
                WHERE WarehouseId = @sourceWarehouseId AND SupplyErpId = @erpId";

            using (var command = new SqlCommand(checkStockSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@sourceWarehouseId", sourceWarehouseId);
                command.Parameters.AddWithValue("@erpId", erpId);
                
                var result = command.ExecuteScalar();
                int currentStock = result != null ? Convert.ToInt32(result) : 0;
                
                if (currentStock < quantity)
                    throw new Exception($"Kho nguồn (ID={sourceWarehouseId}) không đủ tồn kho cho vật tư ErpId={erpId}. Yêu cầu: {quantity}, Tồn kho: {currentStock}");
            }

            // 1. Trừ khỏi kho nguồn
            string updateSourceSql = @"
                UPDATE Inventory 
                SET SoLuongTon = SoLuongTon - @quantity, LastUpdated = GETDATE()
                WHERE WarehouseId = @sourceWarehouseId AND SupplyErpId = @erpId";

            using (var command = new SqlCommand(updateSourceSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@sourceWarehouseId", sourceWarehouseId);
                command.Parameters.AddWithValue("@erpId", erpId);
                command.Parameters.AddWithValue("@quantity", quantity);
                
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Không tìm thấy record tồn kho trong kho nguồn (ID={sourceWarehouseId}) cho SupplyErpId={erpId}");
            }

            // 2. Cộng vào kho nhân viên
            string updateTargetSql = @"
                IF EXISTS (SELECT 1 FROM Inventory WHERE WarehouseId = @targetWarehouseId AND SupplyErpId = @erpId)
                BEGIN
                    UPDATE Inventory 
                    SET SoLuongTon = SoLuongTon + @quantity, LastUpdated = GETDATE()
                    WHERE WarehouseId = @targetWarehouseId AND SupplyErpId = @erpId
                END
                ELSE
                BEGIN
                    INSERT INTO Inventory (WarehouseId, SupplyErpId, SoLuongTon, LastUpdated)
                    VALUES (@targetWarehouseId, @erpId, @quantity, GETDATE())
                END";

            using (var command = new SqlCommand(updateTargetSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@targetWarehouseId", targetWarehouseId);
                command.Parameters.AddWithValue("@erpId", erpId);
                command.Parameters.AddWithValue("@quantity", quantity);
                
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Tạo số phiếu cho transaction xuất kho
        /// </summary>
        private string GenerateExportTransactionNumber()
        {
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            string prefix = "XK"; // Xuất Kho
            
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string countSql = @"
                        SELECT COUNT(*) FROM Transactions 
                        WHERE SoPhieu LIKE @pattern 
                        AND LoaiGiaoDich = 'XuatKho'
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
                Random random = new Random();
                return $"{prefix}{dateStr}{random.Next(100, 999)}";
            }
        }
    }
}
