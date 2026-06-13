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
                SELECT  GiaoKhoanNghiemThuVatTuID, GK.SoBanGiaoKhoan, NT.SoNghiemThu, NT.Nam, NT.SoLanNghiemThu,
                        GK.HoTenNhanVienKyThuat, NT.MaNhanVienThiCong, GK.HoTenNhanVienThiCong, GK.NoiDung, NT.ThoiGianHoanUng
                FROM ct.GiaoKhoanNghiemThuVatTus NT
                INNER JOIN ct.ViewGiaoKhoans GK ON NT.GiaoKhoanID = GK.Id AND GK.IsDeleted = 0
                WHERE NT.SoNghiemThu = @SoNghiemThu AND NT.Nam = @NamNghiemThu AND NT.IsDeleted = 0 AND TTHU = 'TT_A'
                    AND NT.ThoiGianHoanUng > '2025/01/01'";

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
                                    SoBGK = reader["SoBanGiaoKhoan"]?.ToString() ?? "",
                                    SoNghiemThu = reader["SoNghiemThu"] != DBNull.Value ? 
                                        Convert.ToInt32(reader["SoNghiemThu"]) : null,
                                    NamNghiemThu = reader["Nam"] != DBNull.Value ? 
                                        Convert.ToInt32(reader["Nam"]) : null,
                                    SoLanNghiemThu = reader["SoLanNghiemThu"] != DBNull.Value ? 
                                        Convert.ToInt32(reader["SoLanNghiemThu"]) : null,
                                    NhanVienKyThuat = reader["HoTenNhanVienKyThuat"]?.ToString() ?? "",
                                    MaNhanVienXayLap = reader["MaNhanVienThiCong"]?.ToString() ?? "",
                                    NhanVienXayLap = reader["HoTenNhanVienThiCong"]?.ToString() ?? "",
                                    NoiDung = reader["NoiDung"]?.ToString() ?? "",
                                    NgayHoanUng = reader["ThoiGianHoanUng"] != DBNull.Value ? 
                                        Convert.ToDateTime(reader["ThoiGianHoanUng"]) : null
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
        /// Kiểm tra kết nối ERP
        /// </summary>
        public bool TestConnection()
        {
            return ExternalDatabaseHelper.TestExternalConnection();
        }
    }
}