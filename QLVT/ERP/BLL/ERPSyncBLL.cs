using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using QLVT.ERP.DAL;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.ERP.BLL
{
    /// <summary>
    /// BLL để đồng bộ dữ liệu từ ERP vào QLVT Database
    /// </summary>
    public class ERPSyncBLL
    {
        private readonly ERPConnectionDAL _erpDAL;

        public ERPSyncBLL()
        {
            _erpDAL = new ERPConnectionDAL();
        }

        /// <summary>
        /// Test kết nối đến ERP
        /// </summary>
        public async Task<bool> TestERPConnectionAsync()
        {
            return await _erpDAL.TestConnectionAsync();
        }

        /// <summary>
        /// Lấy dữ liệu BGK từ ERP để hiển thị
        /// </summary>
        public async Task<List<NghiemThuGiaoKhoanModel>> GetNghiemThuGiaoKhoanDataAsync(int soNghiemThu, int namNghiemThu)
        {
            return await _erpDAL.GetNghiemThuGiaoKhoanDataAsync(soNghiemThu, namNghiemThu);
        }

        /// <summary>
        /// Lấy chi tiết vật tư BGK từ ERP để hiển thị
        /// </summary>
        public async Task<List<NghiemThuGiaoKhoanCTModel>> GetNghiemThuGiaoKhoanCTDataAsync(long giaoKhoanNghiemThuVatTuID)
        {
            return await _erpDAL.GetNghiemThuGiaoKhoanCTDataAsync(giaoKhoanNghiemThuVatTuID);
        }

        /// <summary>
        /// 1. Đồng bộ dữ liệu Đơn Đăng Ký
        /// </summary>
        public async Task<ERPSyncResult> SyncDonDangKyAsync(DateTime filterDate)
        {
            var result = new ERPSyncResult();
            
            try
            {
                // Lấy dữ liệu từ ERP
                var erpData = await _erpDAL.GetDonDangKyDataAsync(filterDate);
                
                // Insert vào QLVT database
                var recordsInserted = 0;
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                foreach (var item in erpData)
                {
                    var sql = @"
                        IF NOT EXISTS (SELECT 1 FROM ct.DonDangKy WHERE MADDK = @MADDK)
                        BEGIN
                            INSERT INTO ct.DonDangKy (MADDK, TENKH, DiaChi, NhanVienKyThuat, MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, NgayHoanUng)
                            VALUES (@MADDK, @TENKH, @DiaChi, @NhanVienKyThuat, @MaNhanVienXayLap, @NhanVienXayLap, @NgayHoanThanh, @NgayHoanUng)
                        END";

                    using var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@MADDK", item.MADDK);
                    command.Parameters.AddWithValue("@TENKH", item.TENKH ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DiaChi", item.DiaChi ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NhanVienKyThuat", item.NhanVienKyThuat ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaNhanVienXayLap", item.MaNhanVienXayLap ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NhanVienXayLap", item.NhanVienXayLap ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NgayHoanThanh", item.NgayHoanThanh ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NgayHoanUng", item.NgayHoanUng ?? (object)DBNull.Value);

                    var affected = await command.ExecuteNonQueryAsync();
                    recordsInserted += affected;
                }

                result.Success = true;
                result.RecordsAffected = recordsInserted;
                result.Message = $"Đã đồng bộ {recordsInserted} đơn đăng ký thành công.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Lỗi đồng bộ đơn đăng ký: {ex.Message}";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 2. Đồng bộ chi tiết Đơn Đăng Ký
        /// </summary>
        public async Task<ERPSyncResult> SyncDonDangKyCTAsync(string maddk)
        {
            var result = new ERPSyncResult();
            
            try
            {
                var erpData = await _erpDAL.GetDonDangKyCTDataAsync(maddk);
                
                var recordsInserted = 0;
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                foreach (var item in erpData)
                {
                    var sql = @"
                        IF NOT EXISTS (SELECT 1 FROM ct.DonDangKyCT WHERE MADDK = @MADDK AND MaVTErp = @MaVTErp)
                        BEGIN
                            INSERT INTO ct.DonDangKyCT (MADDK, MaVTErp, SoLuongHoanUng)
                            VALUES (@MADDK, @MaVTErp, @SoLuongHoanUng)
                        END";

                    using var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@MADDK", item.MADDK);
                    command.Parameters.AddWithValue("@MaVTErp", item.MaVTErp);
                    command.Parameters.AddWithValue("@SoLuongHoanUng", item.SoLuongHoanUng);

                    var affected = await command.ExecuteNonQueryAsync();
                    recordsInserted += affected;
                }

                result.Success = true;
                result.RecordsAffected = recordsInserted;
                result.Message = $"Đã đồng bộ {recordsInserted} chi tiết đơn đăng ký cho MADDK: {maddk}.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Lỗi đồng bộ chi tiết đơn đăng ký: {ex.Message}";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 3. Đồng bộ dữ liệu Sửa Chữa
        /// </summary>
        public async Task<ERPSyncResult> SyncSuaChuaAsync(DateTime filterDate)
        {
            var result = new ERPSyncResult();
            
            try
            {
                var erpData = await _erpDAL.GetSuaChuaDataAsync(filterDate);
                
                var recordsInserted = 0;
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                foreach (var item in erpData)
                {
                    var sql = @"
                        IF NOT EXISTS (SELECT 1 FROM ct.SuaChua WHERE MADON = @MADON)
                        BEGIN
                            INSERT INTO ct.SuaChua (MADON, ViTriDiemChay, MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, NgayHoanUng)
                            VALUES (@MADON, @ViTriDiemChay, @MaNhanVienXayLap, @NhanVienXayLap, @NgayHoanThanh, @NgayHoanUng)
                        END";

                    using var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@MADON", item.MADON);
                    command.Parameters.AddWithValue("@ViTriDiemChay", item.ViTriDiemChay ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaNhanVienXayLap", item.MaNhanVienXayLap ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NhanVienXayLap", item.NhanVienXayLap ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NgayHoanThanh", item.NgayHoanThanh ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NgayHoanUng", item.NgayHoanUng ?? (object)DBNull.Value);

                    var affected = await command.ExecuteNonQueryAsync();
                    recordsInserted += affected;
                }

                result.Success = true;
                result.RecordsAffected = recordsInserted;
                result.Message = $"Đã đồng bộ {recordsInserted} đơn sửa chữa thành công.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Lỗi đồng bộ sửa chữa: {ex.Message}";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 4. Đồng bộ chi tiết Sửa Chữa
        /// </summary>
        public async Task<ERPSyncResult> SyncSuaChuaCTAsync(string madon)
        {
            var result = new ERPSyncResult();
            
            try
            {
                var erpData = await _erpDAL.GetSuaChuaCTDataAsync(madon);
                
                var recordsInserted = 0;
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                foreach (var item in erpData)
                {
                    var sql = @"
                        IF NOT EXISTS (SELECT 1 FROM ct.SuaChuaCT WHERE MADON = @MADON AND MaVTErp = @MaVTErp)
                        BEGIN
                            INSERT INTO ct.SuaChuaCT (MADON, MaVTErp, SoLuongHoanUng)
                            VALUES (@MADON, @MaVTErp, @SoLuongHoanUng)
                        END";

                    using var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@MADON", item.MADON);
                    command.Parameters.AddWithValue("@MaVTErp", item.MaVTErp);
                    command.Parameters.AddWithValue("@SoLuongHoanUng", item.SoLuongHoanUng);

                    var affected = await command.ExecuteNonQueryAsync();
                    recordsInserted += affected;
                }

                result.Success = true;
                result.RecordsAffected = recordsInserted;
                result.Message = $"Đã đồng bộ {recordsInserted} chi tiết sửa chữa cho MADON: {madon}.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Lỗi đồng bộ chi tiết sửa chữa: {ex.Message}";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 5. Đồng bộ Nghiệm Thu Giao Khoán
        /// </summary>
        public async Task<ERPSyncResult> SyncNghiemThuGiaoKhoanAsync(int soNghiemThu, int namNghiemThu)
        {
            var result = new ERPSyncResult();
            
            try
            {
                var erpData = await _erpDAL.GetNghiemThuGiaoKhoanDataAsync(soNghiemThu, namNghiemThu);
                
                var recordsInserted = 0;
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                foreach (var item in erpData)
                {
                    var sql = @"
                        IF NOT EXISTS (SELECT 1 FROM ct.NghiemThuGiaoKhoan WHERE GiaoKhoanNghiemThuVatTuID = @GiaoKhoanNghiemThuVatTuID)
                        BEGIN
                            INSERT INTO ct.NghiemThuGiaoKhoan (SoBGK, GiaoKhoanNghiemThuVatTuID, NhanVienKyThuat, MaNhanVienXayLap, NhanVienXayLap, NoiDung, SoLanNghiemThu, SoNghiemThu, NamNghiemThu)
                            VALUES (@SoBGK, @GiaoKhoanNghiemThuVatTuID, @NhanVienKyThuat, @MaNhanVienXayLap, @NhanVienXayLap, @NoiDung, @SoLanNghiemThu, @SoNghiemThu, @NamNghiemThu)
                        END";

                    using var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@SoBGK", item.SoBGK);
                    command.Parameters.AddWithValue("@GiaoKhoanNghiemThuVatTuID", item.GiaoKhoanNghiemThuVatTuID);
                    command.Parameters.AddWithValue("@NhanVienKyThuat", item.NhanVienKyThuat ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaNhanVienXayLap", item.MaNhanVienXayLap ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NhanVienXayLap", item.NhanVienXayLap ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NoiDung", item.NoiDung ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SoLanNghiemThu", item.SoLanNghiemThu ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SoNghiemThu", item.SoNghiemThu ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NamNghiemThu", item.NamNghiemThu ?? (object)DBNull.Value);

                    var affected = await command.ExecuteNonQueryAsync();
                    recordsInserted += affected;
                }

                result.Success = true;
                result.RecordsAffected = recordsInserted;
                result.Message = $"Đã đồng bộ {recordsInserted} nghiệm thu giao khoán (Số: {soNghiemThu}, Năm: {namNghiemThu}).";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Lỗi đồng bộ nghiệm thu giao khoán: {ex.Message}";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 6. Đồng bộ chi tiết Nghiệm Thu Giao Khoán
        /// </summary>
        public async Task<ERPSyncResult> SyncNghiemThuGiaoKhoanCTAsync(long giaoKhoanNghiemThuVatTuID)
        {
            var result = new ERPSyncResult();
            
            try
            {
                var erpData = await _erpDAL.GetNghiemThuGiaoKhoanCTDataAsync(giaoKhoanNghiemThuVatTuID);
                
                var recordsInserted = 0;
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                foreach (var item in erpData)
                {
                    var sql = @"
                        IF NOT EXISTS (SELECT 1 FROM ct.NghiemThuGiaoKhoanCT WHERE GiaoKhoanNghiemThuVatTuID = @GiaoKhoanNghiemThuVatTuID AND MaVTErp = @MaVTErp)
                        BEGIN
                            INSERT INTO ct.NghiemThuGiaoKhoanCT (GiaoKhoanNghiemThuVatTuID, MaVTErp, SoLuongHoanUng)
                            VALUES (@GiaoKhoanNghiemThuVatTuID, @MaVTErp, @SoLuongHoanUng)
                        END";

                    using var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@GiaoKhoanNghiemThuVatTuID", item.GiaoKhoanNghiemThuVatTuID);
                    command.Parameters.AddWithValue("@MaVTErp", item.MaVTErp);
                    command.Parameters.AddWithValue("@SoLuongHoanUng", item.SoLuongHoanUng);

                    var affected = await command.ExecuteNonQueryAsync();
                    recordsInserted += affected;
                }

                result.Success = true;
                result.RecordsAffected = recordsInserted;
                result.Message = $"Đã đồng bộ {recordsInserted} chi tiết nghiệm thu giao khoán cho ID: {giaoKhoanNghiemThuVatTuID}.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Lỗi đồng bộ chi tiết nghiệm thu giao khoán: {ex.Message}";
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// Đồng bộ tất cả dữ liệu với các tham số mẫu
        /// </summary>
        public async Task<List<ERPSyncResult>> SyncAllDataAsync(
            DateTime filterDate,
            string sampleMADDK = "KH2508.0162",
            string sampleMADON = "SC2509.0059",
            int sampleSoNghiemThu = 1468,
            int sampleNamNghiemThu = 2025,
            long sampleGiaoKhoanID = 10438)
        {
            var results = new List<ERPSyncResult>();

            try
            {
                // 1. Đồng bộ DonDangKy
                var result1 = await SyncDonDangKyAsync(filterDate);
                results.Add(result1);

                // 2. Đồng bộ DonDangKyCT
                var result2 = await SyncDonDangKyCTAsync(sampleMADDK);
                results.Add(result2);

                // 3. Đồng bộ SuaChua
                var result3 = await SyncSuaChuaAsync(filterDate);
                results.Add(result3);

                // 4. Đồng bộ SuaChuaCT
                var result4 = await SyncSuaChuaCTAsync(sampleMADON);
                results.Add(result4);

                // 5. Đồng bộ NghiemThuGiaoKhoan
                var result5 = await SyncNghiemThuGiaoKhoanAsync(sampleSoNghiemThu, sampleNamNghiemThu);
                results.Add(result5);

                // 6. Đồng bộ NghiemThuGiaoKhoanCT
                var result6 = await SyncNghiemThuGiaoKhoanCTAsync(sampleGiaoKhoanID);
                results.Add(result6);
            }
            catch (Exception ex)
            {
                var errorResult = new ERPSyncResult
                {
                    Success = false,
                    Message = $"Lỗi tổng quát trong quá trình đồng bộ: {ex.Message}",
                    Exception = ex
                };
                results.Add(errorResult);
            }

            return results;
        }

        /// <summary>
        /// Lấy thống kê dữ liệu trong các bảng ERP
        /// </summary>
        public async Task<Dictionary<string, int>> GetERPDataStatsAsync()
        {
            var stats = new Dictionary<string, int>();

            try
            {
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                var tables = new[] { "DonDangKy", "DonDangKyCT", "SuaChua", "SuaChuaCT", "NghiemThuGiaoKhoan", "NghiemThuGiaoKhoanCT" };

                foreach (var table in tables)
                {
                    var sql = $"SELECT COUNT(*) FROM ct.{table}";
                    using var command = new SqlCommand(sql, connection);
                    var count = (int)await command.ExecuteScalarAsync();
                    stats[table] = count;
                }
            }
            catch (Exception)
            {
                // Return empty stats if error
            }

            return stats;
        }

        /// <summary>
        /// Cập nhật trạng thái hoàn ứng cho nghiệm thu giao khoán
        /// </summary>
        public async Task UpdateNghiemThuGiaoKhoanHoanUngAsync(long giaoKhoanNghiemThuVatTuID, 
            DateTime ngayHoanUng, bool daHoanUng, DateTime thoiGianXacNhan)
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                var sql = @"
                    UPDATE ct.NghiemThuGiaoKhoan 
                    SET NgayHoanUng = @NgayHoanUng,
                        DaHoanUng = @DaHoanUng,
                        ThoiGianXacNhanHoanUng = @ThoiGianXacNhan
                    WHERE GiaoKhoanNghiemThuVatTuID = @GiaoKhoanNghiemThuVatTuID";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@NgayHoanUng", ngayHoanUng);
                command.Parameters.AddWithValue("@DaHoanUng", daHoanUng);
                command.Parameters.AddWithValue("@ThoiGianXacNhan", thoiGianXacNhan);
                command.Parameters.AddWithValue("@GiaoKhoanNghiemThuVatTuID", giaoKhoanNghiemThuVatTuID);

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi cập nhật trạng thái hoàn ứng nghiệm thu giao khoán: {ex.Message}", ex);
            }
        }
    }
}