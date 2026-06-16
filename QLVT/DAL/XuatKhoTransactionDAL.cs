using Microsoft.Data.SqlClient;
using QLVT.ERP.Models;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class XuatKhoTransactionDAL
    {
        /// <summary>
        /// Tạo transaction xuất kho với chi tiết từng dòng (nhiều kho nguồn khác nhau)
        /// </summary>
        /// <param name="order">Phiếu xuất từ ERP</param>
        /// <param name="employeeWarehouseId">ID kho nhân viên đích</param>
        /// <param name="createdBy">Người tạo</param>
        /// <param name="staffCode">Mã nhân viên thực hiện</param>
        /// <returns>ID transaction vừa tạo</returns>
        public int CreateXuatKhoErpTransaction(ERP_PhieuXuatKho order, int employeeWarehouseId, string createdBy)
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
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNguon, MaKhoNhan, GhiChu, CreatedBy, EntityXuatKho)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'XuatKho', NULL, @maKhoNhan, @ghiChu, @createdBy, @entityXuatKho);
                            SELECT SCOPE_IDENTITY();";

                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", order.ThoiGianHoanThanhXuatKho);
                            // Kho nhận có thể là null nếu tất cả vật tư đều là VP
                            command.Parameters.AddWithValue("@maKhoNhan", employeeWarehouseId > 0 ? (object)employeeWarehouseId : DBNull.Value);
                            command.Parameters.AddWithValue("@ghiChu", $"Xuất kho từ phiếu ERP: {order.SoPhieuXuatKho}-{order.NAM}");
                            command.Parameters.AddWithValue("@createdBy", createdBy);
                            command.Parameters.AddWithValue("@entityXuatKho", $"XK:{order.SoPhieuXuatKho}-{order.NAM}");
                            
                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Thêm TransactionDetails
                        foreach (var detail in order.ChiTiet.Where(d => d.IsMapped))
                        {
                            // Validate kho nguồn phải có
                            if (!detail.SourceWarehouseId.HasValue)
                                throw new Exception($"Chi tiết vật tư {detail.MaVatTuHangHoa} chưa xác định được kho nguồn");

                            int sourceWarehouseId = detail.SourceWarehouseId.Value;

                            // Kiểm tra DanhMuc để xác định kho đích
                            int? targetWarehouseId = DetermineTargetWarehouseId(connection, transaction, detail.MappedSupplyId!.Value, employeeWarehouseId);

                            // Insert transaction detail với SourceWarehouseId
                            string insertDetailSql = @"
                                INSERT INTO TransactionDetails (TransactionId, ErpId, SoLuong, GhiChu, MaKhoXuat, MaKhoNhap, CreatedBy, CreatedDate)
                                VALUES (@transactionId, @erpId, @soLuong, @ghiChu, @maKhoXuat, @maKhoNhap, @createdBy, GETDATE())";

                            using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@transactionId", transactionId);
                                command.Parameters.AddWithValue("@erpId", detail.MappedSupplyId!.Value);
                                command.Parameters.AddWithValue("@soLuong", detail.SoLuongXuatKho);
                                command.Parameters.AddWithValue("@ghiChu", $"{detail.MucDichSuDung} (phiếu {order.SoPhieuXuatKho}-{order.NAM})");
                                command.Parameters.AddWithValue("@maKhoXuat", sourceWarehouseId);
                                command.Parameters.AddWithValue("@maKhoNhap", (object?)targetWarehouseId ?? DBNull.Value);
                                command.Parameters.AddWithValue("@createdBy", createdBy);

                                command.ExecuteNonQuery();
                            }

                            // Cập nhật inventory: Trừ từ kho nguồn cụ thể, cộng vào kho nhân viên (nếu có)
                            TransferInventory(connection, transaction, sourceWarehouseId, targetWarehouseId, detail.MappedSupplyId!.Value, detail.SoLuongXuatKho);
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
        /// Xác định kho đích dựa trên DanhMuc của vật tư
        /// Nếu DanhMuc = "VP" thì kho đích sẽ là null
        /// Ngược lại sẽ là kho nhân viên
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="erpId">ERP ID vật tư</param>
        /// <param name="employeeWarehouseId">ID kho nhân viên</param>
        /// <returns>ID kho đích hoặc null nếu là VP</returns>
        private int? DetermineTargetWarehouseId(SqlConnection connection, SqlTransaction transaction, int erpId, int employeeWarehouseId)
        {
            string checkCategorySql = @"
                SELECT DanhMuc FROM Supplies 
                WHERE ErpId = @erpId";

            using (var command = new SqlCommand(checkCategorySql, connection, transaction))
            {
                command.Parameters.AddWithValue("@erpId", erpId);
                
                var result = command.ExecuteScalar();
                string danhMuc = result?.ToString() ?? "";
                
                // Nếu là VP thì kho đích là null, ngược lại là kho nhân viên
                return danhMuc == "VP" ? null : employeeWarehouseId;
            }
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
        private void TransferInventory(SqlConnection connection, SqlTransaction transaction, int sourceWarehouseId, int? targetWarehouseId, int erpId, decimal quantity)
        {
            decimal roundedQuantity = Math.Round(quantity, 2);

            // Kiểm tra tồn kho đủ không
            string checkStockSql = @"
                SELECT ISNULL(SoLuongTon, 0) FROM Inventory 
                WHERE WarehouseId = @sourceWarehouseId AND SupplyErpId = @erpId";

            using (var command = new SqlCommand(checkStockSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@sourceWarehouseId", sourceWarehouseId);
                command.Parameters.AddWithValue("@erpId", erpId);
                
                var result = command.ExecuteScalar();
                decimal currentStock = result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                
                if (currentStock < roundedQuantity)
                    throw new Exception($"Kho nguon (ID={sourceWarehouseId}) khong du ton kho cho vat tu ErpId={erpId}. Yeu cau: {roundedQuantity:N2}, Ton kho: {currentStock:N2}");
            }

            // 1. Trừ khỏi kho nguồn
            string updateSourceSql = @"
                UPDATE Inventory 
                SET SoLuongTon = SoLuongTon - @quantity, LastUpdated = GETDATE()
                WHERE WarehouseId = @sourceWarehouseId AND SupplyErpId = @erpId
                  AND SoLuongTon >= @quantity";

            using (var command = new SqlCommand(updateSourceSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@sourceWarehouseId", sourceWarehouseId);
                command.Parameters.AddWithValue("@erpId", erpId);
                command.Parameters.AddWithValue("@quantity", roundedQuantity);
                
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Khong the tru ton kho nguon (ID={sourceWarehouseId}) cho SupplyErpId={erpId}. Ton kho co the khong du hoac da bi thay doi.");
            }

            if (targetWarehouseId != null)
            {
                // 2. Cộng vào kho nhận
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
                    command.Parameters.AddWithValue("@quantity", roundedQuantity);

                    command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Tạo transaction nhập kho không qua ERP
        /// </summary>
        public int CreateXuatKhoManualTransaction(PhieuXuatKho phieu, string createdBy)
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
                            // 1. Tạo số phiếu
                            string soPhieu = GenerateExportTransactionNumber();
                            // Tạo phiếu xuất kho
                            var insertQuery = @"
                                INSERT INTO Transactions (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNguon, MaKhoNhan, GhiChu, CreatedBy, CreatedDate)
                                VALUES (@SoPhieu, @NgayGiaoDich, @LoaiGiaoDich, @MaKhoNguon, @MaKhoNhan, @GhiChu, @CreatedBy, GETDATE());
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int transactionId;
                            using (var command = new SqlCommand(insertQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@SoPhieu", soPhieu);
                                command.Parameters.AddWithValue("@NgayGiaoDich", phieu.NgayGiaoDich);
                                command.Parameters.AddWithValue("@LoaiGiaoDich", "XuatKho");
                                command.Parameters.AddWithValue("@MaKhoNguon", (object?)phieu.MaKhoNguon ?? DBNull.Value);
                                command.Parameters.AddWithValue("@MaKhoNhan", (object?)phieu.MaKhoNhan ?? DBNull.Value);
                                command.Parameters.AddWithValue("@GhiChu", (object?)phieu.GhiChu ?? DBNull.Value);
                                command.Parameters.AddWithValue("@CreatedBy", createdBy);

                                transactionId = (int)command.ExecuteScalar();
                            }

                            // Thêm chi tiết và cập nhật inventory
                            foreach (var chiTiet in phieu.ChiTiet)
                            {
                                var insertDetailsQuery = @"
                                    INSERT INTO TransactionDetails (TransactionId, ErpId, SoLuong, GhiChu, SourceWarehouseId, CreatedBy, CreatedDate)
                                    VALUES (@TransactionId, @ErpId, @SoLuong, @GhiChu, @SourceWarehouseId, @createdBy, GETDATE())";

                                using (var command = new SqlCommand(insertDetailsQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@TransactionId", transactionId);
                                    command.Parameters.AddWithValue("@ErpId", chiTiet.ErpId);
                                    command.Parameters.AddWithValue("@SoLuong", chiTiet.SoLuong);
                                    command.Parameters.AddWithValue("@GhiChu", (object?)chiTiet.GhiChu ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@SourceWarehouseId", (object?)chiTiet.SourceWarehouseId ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@createdBy", createdBy);

                                    command.ExecuteNonQuery();
                                }

                                TransferInventory(connection, transaction, phieu.MaKhoNguon, phieu.MaKhoNhan, chiTiet.ErpId, chiTiet.SoLuong);
                            }

                            transaction.Commit();
                            return transactionId;
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
                throw new Exception($"Lỗi tạo phiếu xuất kho: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo số phiếu cho transaction xuất kho
        /// </summary>
        private string GenerateExportTransactionNumber()
        {
            string dateStr = DateTime.Now.ToString("yyMMdd");
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
                Random random = new Random();
                return $"{prefix}{dateStr}{random.Next(1000, 9999)}";
            }
        }
    }
}
