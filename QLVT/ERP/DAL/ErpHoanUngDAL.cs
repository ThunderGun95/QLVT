using Microsoft.Data.SqlClient;
using QLVT.ERP.Models;
using QLVT.Utils;
using System.Data;

namespace QLVT.ERP.DAL
{
    /// <summary>
    /// DAL để xử lý dữ liệu BGK từ ERP - sử dụng chung cách kết nối như ImportDAL
    /// </summary>
    public class ErpHoanUngDAL
    {
        /// <summary>
        /// Lấy dữ liệu BGK từ ERP theo số nghiệm thu và năm
        /// </summary>
        public List<NghiemThuGiaoKhoanModel> GetNghiemThuGiaoKhoanDataAsync(int soNghiemThu, int namNghiemThu)
        {
            var result = new List<NghiemThuGiaoKhoanModel>();
            
            string sql = @"
                SELECT GiaoKhoanNghiemThuVatTuID, SoBGK, SoNghiemThu, NamNghiemThu, SoLanNghiemThu,
                       NhanVienKyThuat, NhanVienXayLap, NoiDung, NgayNghiemThu, DonViThau, GoiThau,
                       TongGiaTri, GhiChu, NgayHoanUng, DaHoanUng, ThoiGianXacNhanHoanUng
                FROM ct.GiaoKhoanNghiemThuVatTus
                WHERE SoNghiemThu = @SoNghiemThu AND NamNghiemThu = @NamNghiemThu AND IsDeleted = 0";

            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SoNghiemThu", soNghiemThu);
                        command.Parameters.AddWithValue("@NamNghiemThu", namNghiemThu);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new NghiemThuGiaoKhoanModel
                                {
                                    GiaoKhoanNghiemThuVatTuID = reader["GiaoKhoanNghiemThuVatTuID"] != DBNull.Value ? 
                                        Convert.ToInt64(reader["GiaoKhoanNghiemThuVatTuID"]) : null,
                                    SoBGK = reader["SoBGK"]?.ToString() ?? "",
                                    SoNghiemThu = reader["SoNghiemThu"] != DBNull.Value ? 
                                        Convert.ToInt32(reader["SoNghiemThu"]) : null,
                                    NamNghiemThu = reader["NamNghiemThu"] != DBNull.Value ? 
                                        Convert.ToInt32(reader["NamNghiemThu"]) : null,
                                    SoLanNghiemThu = reader["SoLanNghiemThu"] != DBNull.Value ? 
                                        Convert.ToInt32(reader["SoLanNghiemThu"]) : null,
                                    NhanVienKyThuat = reader["NhanVienKyThuat"]?.ToString() ?? "",
                                    NhanVienXayLap = reader["NhanVienXayLap"]?.ToString() ?? "",
                                    NoiDung = reader["NoiDung"]?.ToString() ?? "",
                                    NgayNghiemThu = reader["NgayNghiemThu"] != DBNull.Value ? 
                                        Convert.ToDateTime(reader["NgayNghiemThu"]) : null,
                                    DonViThau = reader["DonViThau"]?.ToString() ?? "",
                                    GoiThau = reader["GoiThau"]?.ToString() ?? "",
                                    TongGiaTri = reader["TongGiaTri"] != DBNull.Value ? 
                                        Convert.ToDecimal(reader["TongGiaTri"]) : null,
                                    GhiChu = reader["GhiChu"]?.ToString() ?? "",
                                    NgayHoanUng = reader["NgayHoanUng"] != DBNull.Value ? 
                                        Convert.ToDateTime(reader["NgayHoanUng"]) : null,
                                    DaHoanUng = reader["DaHoanUng"] != DBNull.Value ? 
                                        Convert.ToBoolean(reader["DaHoanUng"]) : false,
                                    ThoiGianXacNhanHoanUng = reader["ThoiGianXacNhanHoanUng"] != DBNull.Value ? 
                                        Convert.ToDateTime(reader["ThoiGianXacNhanHoanUng"]) : null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy dữ liệu BGK {soNghiemThu}-{namNghiemThu}: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Lấy chi tiết vật tư BGK từ ERP
        /// </summary>
        public List<NghiemThuGiaoKhoanCTModel> GetNghiemThuGiaoKhoanCTDataAsync(long giaoKhoanNghiemThuVatTuID)
        {
            var result = new List<NghiemThuGiaoKhoanCTModel>();
            
            string sql = @"
                SELECT c.GiaoKhoanNghiemThuVatTuID, vt.Id as VatTuId, 
                       vt.Code as VatTuHangHoa, vt.TenVatTu, vt.DonViTinh, 
                       SUM(c.SoLuongNghiemThu) as SoLuongHoanUng
                FROM ct.GiaoKhoanNghiemThuVatTuCTs AS c 
                INNER JOIN vt.ViewVatTuHangHoas AS vt ON vt.MaVatTuHangHoa = c.MaVatTuHangHoa
                WHERE (c.IsDeleted = 0) 
                  AND (c.GiaoKhoanNghiemThuVatTuID = @GiaoKhoanNghiemThuVatTuID)
                GROUP BY c.GiaoKhoanNghiemThuVatTuID, vt.Id, vt.TenVatTu, vt.DacTinhKyThuat, vt.Code, vt.DonViTinh";

            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@GiaoKhoanNghiemThuVatTuID", giaoKhoanNghiemThuVatTuID);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new NghiemThuGiaoKhoanCTModel
                                {
                                    GiaoKhoanNghiemThuVatTuID = Convert.ToInt64(reader["GiaoKhoanNghiemThuVatTuID"]),
                                    MaVTErp = Convert.ToInt32(reader["VatTuId"]),
                                    VatTuHangHoa = reader["VatTuHangHoa"]?.ToString() ?? "",
                                    TenVatTu = reader["TenVatTu"]?.ToString() ?? "",
                                    DonViTinh = reader["DonViTinh"]?.ToString() ?? "",
                                    SoLuongHoanUng = Convert.ToDecimal(reader["SoLuongHoanUng"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết BGK {giaoKhoanNghiemThuVatTuID}: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Cập nhật trạng thái hoàn ứng cho nghiệm thu giao khoán
        /// </summary>
        public void UpdateNghiemThuGiaoKhoanHoanUng(long giaoKhoanNghiemThuVatTuID, 
            DateTime ngayHoanUng, bool daHoanUng, DateTime thoiGianXacNhan)
        {
            string sql = @"
                UPDATE ct.GiaoKhoanNghiemThuVatTus 
                SET NgayHoanUng = @NgayHoanUng,
                    DaHoanUng = @DaHoanUng,
                    ThoiGianXacNhanHoanUng = @ThoiGianXacNhan,
                    UpdatedDate = GETDATE()
                WHERE GiaoKhoanNghiemThuVatTuID = @GiaoKhoanNghiemThuVatTuID";

            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NgayHoanUng", ngayHoanUng);
                        command.Parameters.AddWithValue("@DaHoanUng", daHoanUng);
                        command.Parameters.AddWithValue("@ThoiGianXacNhan", thoiGianXacNhan);
                        command.Parameters.AddWithValue("@GiaoKhoanNghiemThuVatTuID", giaoKhoanNghiemThuVatTuID);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật trạng thái hoàn ứng BGK: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra kết nối ERP
        /// </summary>
        public bool TestConnection()
        {
            return ExternalDatabaseHelper.TestExternalConnection();
        }
    }
}