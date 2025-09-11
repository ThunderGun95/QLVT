using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class ERPExportDAL
    {
        /// <summary>
        /// Lấy phiếu xuất kho từ hệ thống ERP theo số phiếu và năm
        /// </summary>
        /// <param name="soPhieu">Số phiếu xuất</param>
        /// <param name="nam">Năm</param>
        /// <returns>Phiếu xuất kho hoặc null nếu không tìm thấy</returns>
        public ERPExportOrder? GetExportOrderByNumber(string soPhieu, int nam)
        {
            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    
                    // Query để lấy thông tin header phiếu xuất
                    string sql = @"
                        SELECT MaPhieuXuatKhoVatTu, SoPhieuXuatKho, NAM,  nv.MaNhanVien, nv.HoVaTen as TenNguoiNhan, ThoiGianTKXK
                        FROM vt.PhieuXuatKhoVatTus px
                            LEFT JOIN ViewNhanViens nv on nv.UserID = px.MaNguoiNhan
                         WHERE TTXK = 'TT_A'
                            AND SoPhieuXuatKho = @soPhieu AND NAM = @nam";
                    

                    ERPExportOrder? order = null;

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@soPhieu", soPhieu);
                        command.Parameters.AddWithValue("@nam", nam);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                order = new ERPExportOrder
                                {
                                    MaPhieuXuatKhoVatTu = Convert.ToInt32(reader["MaPhieuXuatKhoVatTu"]),
                                    SoPhieuXuatKho = reader["SoPhieuXuatKho"].ToString() ?? string.Empty,
                                    NAM = Convert.ToInt32(reader["NAM"]),
                                    TenNhanVien = reader["TenNguoiNhan"]?.ToString() ?? string.Empty,
                                    MaNhanVien = reader["MaNhanVien"]?.ToString() ?? string.Empty,
                                    ThoiGianHoanThanhXuatKho = reader["ThoiGianTKXK"] != DBNull.Value ? 
                                             Convert.ToDateTime(reader["ThoiGianTKXK"]) : DateTime.MinValue
                                };
                            }
                        }
                    }

                    // Nếu không tìm thấy header thì return null
                    if (order == null) return null;

                    // Query để lấy chi tiết phiếu xuất kèm thông tin kho
                    string detailSql = @"
                        SELECT vtxk.MaPhieuXuatKhoVatTu, vtxk.MaVatTuHangHoa, vt.TenVatTu, vt.DacTinhKyThuat, vt.DonViTinh, 
                                vtxk.MaKhoVatTu, kho.TenKho, SUm(SoLuongXuatKho) as SoLuongXuatKho
                        FROM vt.PhieuXuatKhoVatTuCTs vtxk
                             INNER JOIN vt.ViewVatTuHangHoas vt on vt.MaVatTuHangHoa = vtxk.MaVatTuHangHoa
                             INNER JOIN vt.KhoVatTus kho on kho.MaKhoVatTu = vtxk.MaKhoVatTu
                        WHERE MaPhieuXuatKhoVatTu = @maPhieu AND vtxk.IsDeleted = 0
                        GROUP BY vtxk.MaPhieuXuatKhoVatTu, vtxk.MaVatTuHangHoa, vt.TenVatTu, vt.DacTinhKyThuat, vt.DonViTinh, 
                            vtxk.MaKhoVatTu, kho.TenKho";

                    using (var command = new SqlCommand(detailSql, connection))
                    {
                        command.Parameters.AddWithValue("@maPhieu", order.MaPhieuXuatKhoVatTu);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order.ChiTiet.Add(new ERPExportOrderDetail
                                {
                                    MaPhieuXuatKhoVatTu = Convert.ToInt32(reader["MaPhieuXuatKhoVatTu"]),
                                    MaVatTuHangHoa = reader["MaVatTuHangHoa"]?.ToString() ?? string.Empty,
                                    TenVatTu = reader["TenVatTu"]?.ToString() ?? string.Empty,
                                    SoLuongXuatKho = reader["SoLuongXuatKho"] != DBNull.Value ? Convert.ToDecimal(reader["SoLuongXuatKho"]) : 0m,
                                    DonViTinh = reader["DonViTinh"]?.ToString() ?? string.Empty,
                                    MaKhoXuat = reader["MaKhoVatTu"]?.ToString() ?? string.Empty,
                                    TenKhoXuat = reader["TenKho"]?.ToString() ?? string.Empty
                                });
                            }
                        }
                    }

                    return order;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy phiếu xuất kho từ ERP: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra phiếu xuất kho đã được xử lý chưa
        /// </summary>
        /// <param name="soPhieu">Số phiếu</param>
        /// <param name="nam">Năm</param>
        /// <returns>True nếu đã xử lý</returns>
        public bool IsExportOrderProcessed(string soPhieu, int nam)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    string sql = @"
                        SELECT COUNT(*) FROM Transactions 
                        WHERE LoaiGiaoDich = 'XuatKho' 
                        AND EntityXuatKho = @entityXuatKho";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@entityXuatKho", $"{soPhieu}-{nam}");
                        
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra trạng thái phiếu xuất: {ex.Message}");
            }
        }

        /// <summary>
        /// Tìm kiếm phiếu xuất kho theo từ khóa
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="nam">Năm (tùy chọn)</param>
        /// <param name="limit">Số lượng kết quả tối đa</param>
        /// <returns>Danh sách phiếu xuất</returns>
        public List<ERPExportOrder> SearchExportOrders(string keyword, int? nam = null, int limit = 50)
        {
            var orders = new List<ERPExportOrder>();
            
            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    
                    string sql = @"
                        SELECT TOP (@limit) MaPhieuXuatKhoVatTu, SoPhieuXuatKho, NAM,
                               TenNhanVien, MaNhanVien, ThoiGianHoanThanhXuatKho,
                               MaNhanVienXuat, TenNhanVienXuat
                        FROM PhieuXuatKhoVatTu 
                        WHERE (SoPhieuXuatKho LIKE @keyword OR TenNhanVien LIKE @keyword)";

                    if (nam.HasValue)
                    {
                        sql += " AND NAM = @nam";
                    }
                    
                    sql += " ORDER BY ThoiGianHoanThanhXuatKho DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@keyword", $"%{keyword}%");
                        command.Parameters.AddWithValue("@limit", limit);
                        
                        if (nam.HasValue)
                        {
                            command.Parameters.AddWithValue("@nam", nam.Value);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orders.Add(new ERPExportOrder
                                {
                                    MaPhieuXuatKhoVatTu = Convert.ToInt32(reader["MaPhieuXuatKhoVatTu"]),
                                    SoPhieuXuatKho = reader["SoPhieuXuatKho"]?.ToString() ?? string.Empty,
                                    NAM = Convert.ToInt32(reader["NAM"]),
                                    TenNhanVien = reader["TenNhanVien"]?.ToString() ?? string.Empty,
                                    MaNhanVien = reader["MaNhanVien"]?.ToString() ?? string.Empty,
                                    ThoiGianHoanThanhXuatKho = reader["ThoiGianHoanThanhXuatKho"] != DBNull.Value ? 
                                             Convert.ToDateTime(reader["ThoiGianHoanThanhXuatKho"]) : DateTime.MinValue,
                                    MaNhanVienXuat = reader["MaNhanVienXuat"]?.ToString() ?? string.Empty,
                                    TenNhanVienXuat = reader["TenNhanVienXuat"]?.ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm phiếu xuất kho: {ex.Message}");
            }

            return orders;
        }
    }
}
