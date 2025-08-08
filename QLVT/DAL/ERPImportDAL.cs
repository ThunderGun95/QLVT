using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class ERPImportDAL
    {
        /// <summary>
        /// Lấy thông tin phiếu nhập từ ERP theo số phiếu và năm
        /// </summary>
        /// <param name="soPhieu">Số phiếu nhập</param>
        /// <param name="nam">Năm của phiếu</param>
        /// <returns>Thông tin phiếu nhập</returns>
        public ERPImportOrder? GetImportOrderByNumber(string soPhieu, int nam)
        {
            ERPImportOrder? order = null;
            
            string sql = @"
                SELECT MaPhieuNhapKhoVatTu, SoPhieuNhapKho, NAM, TenKho, MaKhoVatTu, 
                       ThoiGianHoanThanhNhapKho, MaNhanVienMua, NhanVienMua
                FROM   vt.ViewPhieuNhapKhoVatTus nk
                LEFT JOIN ViewNhanViens nv on nv.UserID = nk.MaNhanVienMua
                LEFT JOIN PHONGBAN pb on pb.MAPB = nv.MAPB
                WHERE LOAIPHIEU = 'BIEU01' AND TrangThai = 'HoanThanh'
                AND SoPhieuNhapKho = @soPhieu AND NAM = @nam";

            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@soPhieu", soPhieu);
                        command.Parameters.AddWithValue("@nam", nam);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                order = new ERPImportOrder
                                {
                                    MaPhieuNhapKhoVatTu = Convert.ToInt32(reader["MaPhieuNhapKhoVatTu"]),
                                    SoPhieuNhapKho = reader["SoPhieuNhapKho"].ToString() ?? string.Empty,
                                    NAM = Convert.ToInt32(reader["NAM"]),
                                    TenKho = reader["TenKho"].ToString() ?? string.Empty,
                                    MaKhoVatTu = reader["MaKhoVatTu"].ToString() ?? string.Empty,
                                    ThoiGianHoanThanhNhapKho = reader["ThoiGianHoanThanhNhapKho"] != DBNull.Value ? 
                                             Convert.ToDateTime(reader["ThoiGianHoanThanhNhapKho"]) : DateTime.Now,
                                    MaNhanVienMua = reader["MaNhanVienMua"].ToString() ?? string.Empty,
                                    NhanVienMua = reader["NhanVienMua"].ToString() ?? string.Empty
                                };
                            }
                        }
                    }
                }

                // Nếu tìm thấy phiếu, lấy chi tiết
                if (order != null)
                {
                    order.ChiTiet = GetImportOrderDetails(order.MaPhieuNhapKhoVatTu);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy phiếu nhập {soPhieu}-{nam}: {ex.Message}");
            }

            return order;
        }

        /// <summary>
        /// Lấy chi tiết phiếu nhập từ ERP
        /// </summary>
        /// <param name="maPhieuNhapKhoVatTu">Mã phiếu nhập kho vật tư</param>
        /// <returns>Danh sách chi tiết</returns>
        public List<ERPImportOrderDetail> GetImportOrderDetails(int maPhieuNhapKhoVatTu)
        {
            var details = new List<ERPImportOrderDetail>();
            
            string sql = @"
                SELECT pnk.MaPhieuNhapKhoVatTu, nk.MaVatTuHangHoa, nk.TenVatTu, nk.DacTinhKyThuat, 
                       nk.DonViTinh, SUM(nk.SoLuongNhapKho) as SoLuongNhapKho, 
                       nk.MaNhaSanXuat, nk.TenNhaSanXuat
                FROM   vt.ViewPhieuNhapKhoVatTuCTs AS nk INNER JOIN
                       vt.PhieuNhapKhoVatTus AS pnk ON pnk.MaPhieuNhapKhoVatTu = nk.MaPhieuNhapKhoVatTu 
                WHERE (pnk.TrangThai = 'HoanThanh') AND (nk.MaPhieuNhapKhoVatTu = @maPhieuNhapKhoVatTu)
                GROUP BY pnk.MaPhieuNhapKhoVatTu, nk.MaVatTuHangHoa, nk.TenVatTu, nk.DacTinhKyThuat, 
                         nk.DonViTinh, nk.MaNhaSanXuat, nk.TenNhaSanXuat";

            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@maPhieuNhapKhoVatTu", maPhieuNhapKhoVatTu);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                details.Add(new ERPImportOrderDetail
                                {
                                    MaPhieuNhapKhoVatTu = Convert.ToInt32(reader["MaPhieuNhapKhoVatTu"]),
                                    MaVatTuHangHoa = reader["MaVatTuHangHoa"].ToString() ?? string.Empty,
                                    TenVatTu = reader["TenVatTu"].ToString() ?? string.Empty,
                                    DacTinhKyThuat = reader["DacTinhKyThuat"].ToString() ?? string.Empty,
                                    SoLuongNhapKho = Convert.ToDecimal(reader["SoLuongNhapKho"]),
                                    DonViTinh = reader["DonViTinh"].ToString() ?? string.Empty,
                                    MaNhaSanXuat = reader["MaNhaSanXuat"].ToString() ?? string.Empty,
                                    TenNhaSanXuat = reader["TenNhaSanXuat"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết phiếu {maPhieuNhapKhoVatTu}: {ex.Message}");
            }

            return details;
        }

        /// <summary>
        /// Kiểm tra phiếu nhập đã được xử lý chưa
        /// </summary>
        /// <param name="soPhieu">Số phiếu</param>
        /// <param name="nam">Năm của phiếu</param>
        /// <returns>True nếu đã xử lý</returns>
        public bool IsImportOrderProcessed(string soPhieu, int nam)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string sql = @"
                        SELECT COUNT(*) 
                        FROM Transactions 
                        WHERE LoaiGiaoDich = 'NhapKho' 
                        AND EntityNhapKho = @entityNhapKho";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@entityNhapKho", $"{soPhieu}-{nam}");
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
