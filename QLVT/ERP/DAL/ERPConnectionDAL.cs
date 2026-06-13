using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.ERP.DAL
{
    /// <summary>
    /// DAL để kết nối và lấy dữ liệu từ ERP Database
    /// </summary>
    public class ERPConnectionDAL
    {
        public ERPConnectionDAL()
        {
            // Sử dụng ExternalDatabaseHelper để lấy connection
        }

        /// <summary>
        /// Test kết nối đến ERP Database
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var connection = ExternalDatabaseHelper.GetExternalConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Thực thi query và trả về DataTable
        /// </summary>
        private async Task<DataTable> ExecuteQueryAsync(string sql, params SqlParameter[] parameters)
        {
            using var connection = ExternalDatabaseHelper.GetExternalConnection();
            using var command = new SqlCommand(sql, connection);
            
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();
            using var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            
            return dataTable;
        }

        /// <summary>
        /// 1. Lấy dữ liệu Đơn Đăng Ký từ ERP
        /// </summary>
        public async Task<List<DonDangKyModel>> GetDonDangKyDataAsync(DateTime filterDate)
        {
            var sql = @"
                SELECT mc4.MADDK, mc4.TENKH, mc4.DIACHILD as DiaChi, 
                       mc4.NhanVienKyThuat, mc4.MaNhanVienXayLap, mc4.NhanVienXayLap, 
                       mc4.NGAYTC as NgayHoanThanh, hu.ThoiGianHoanUng as NgayHoanUng
                FROM ld.ViewMangCap4s mc4 
                INNER JOIN DONDANGKY dk on dk.MADDK = mc4.MADDK
                INNER JOIN ld.HoanUngs hu on hu.Id = dk.MaPhieuHoanUng
                WHERE mc4.TTDK = 'DK_A' and mc4.TTCT = 'CT_A' and mc4.TTTC = 'TC_A'
                  AND CONVERT(DATE, hu.ThoiGianHoanUng) > @FilterDate
                  AND YEAR(mc4.NgayTC) > 2024
                  AND hu.ThoiGianHoanUng < '2025/12/01'  
                  -- AND mc4.MaNhanVienKyThuat NOT IN ('hqthong', 'nqhoan', 'dtthang', 'ltthu', 'vddung', 'sutm', 'vhdieu', 'ldthuan', 'hvhan', 'thinpv')
                  AND mc4.MaNhanVienXayLap in ('ndtan', 'nhhai', 'phhung', 'pvnam', 'hsduan2', 'kienpv', 'pvhoan', 'nhquang2', 'dnba', 'lvhanh', 'ddthuat', 'vdvuong', 'nhthang2', 'dnhat2', 'nncanh', 'pxthang2', 'vtcau', 'ntdung', 'thchien', 'hhtuong', 'nvtuan', 'knbinh') 
                  -- Thêm cái này để hoàn tổ mc4 trước
                    AND mc4.IsHuy = 0
                ORDER BY ThoiGianHoanUng";

            var parameters = new[] { new SqlParameter("@FilterDate", filterDate) };
            var dataTable = await ExecuteQueryAsync(sql, parameters);
            
            var result = new List<DonDangKyModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new DonDangKyModel
                {
                    MADDK = row["MADDK"]?.ToString() ?? "",
                    TENKH = row["TENKH"]?.ToString() ?? "",
                    DiaChi = row["DiaChi"]?.ToString() ?? "",
                    NhanVienKyThuat = row["NhanVienKyThuat"]?.ToString() ?? "",
                    MaNhanVienXayLap = row["MaNhanVienXayLap"]?.ToString() ?? "",
                    NhanVienXayLap = row["NhanVienXayLap"]?.ToString() ?? "",
                    NgayHoanThanh = row["NgayHoanThanh"] as DateTime?,
                    NgayHoanUng = row["NgayHoanUng"] as DateTime?
                });
            }
            
            return result;
        }

        /// <summary>
        /// 2. Lấy chi tiết Đơn Đăng Ký từ ERP
        /// </summary>
        public async Task<List<DonDangKyCTModel>> GetDonDangKyCTDataAsync(string maddk)
        {
            var sql = @"
                SELECT mc4.MADDK, VT.Id as VatTuId, VT.Code as VatTuHangHoa, 
                       VT.TenVatTu, VT.DonViTinh, ROUND(SUM(vthu.SOLUONG),2) as SoLuongHoanUng
                FROM ld.ViewMangCap4s mc4
                INNER JOIN ld.HoanUngs h ON h.MADDK = mc4.MADDK
                LEFT JOIN vt.HoanUngVatTus vthu on vthu.MaHoanUng = h.Id and vthu.MaPhieuXuatKhoVatTuCT is not null and vthu.IsDeleted = 0
                INNER JOIN vt.ViewVatTuHangHoas VT on vt.MaVatTuHangHoa = vthu.MaVatTuHangHoa
                WHERE mc4.TTDK = 'DK_A' and mc4.TTCT = 'CT_A' and mc4.TTTC = 'TC_A'
                  AND CONVERT(DATE, h.ThoiGianHoanUng) > '2025/01/01'
                  AND YEAR(mc4.NgayTC) > 2024
                  AND mc4.IsHuy = 0
                  AND mc4.MADDK = @MADDK
                GROUP BY mc4.MADDK, VT.Id, VT.Code, Vt.TenVatTu, VT.DonViTinh";

            var parameters = new[] { new SqlParameter("@MADDK", maddk) };
            var dataTable = await ExecuteQueryAsync(sql, parameters);
            
            var result = new List<DonDangKyCTModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new DonDangKyCTModel
                {
                    MADDK = row["MADDK"]?.ToString() ?? "",
                    MaVTErp = Convert.ToInt32(row["VatTuId"]),
                    SoLuongHoanUng = Convert.ToDecimal(row["SoLuongHoanUng"])
                });
            }
            
            return result;
        }

        /// <summary>
        /// 3. Lấy dữ liệu Sửa Chữa từ ERP
        /// </summary>
        public async Task<List<SuaChuaModel>> GetSuaChuaDataAsync(DateTime filterDate)
        {
            var sql = @"
                SELECT sc.MADON, sc.ViTriDiemChay, sc.MaNhanVienThiCong as MaNhanVienXayLap, 
                       nvtc.HoVaTen as NhanVienXayLap, Sc.ThoiGianHoanThanh as NgayHoanThanh, 
                       sc.ThoiGianHoanUng as NgayHoanUng
                FROM ld.SuaChuaSuCos AS sc  
                INNER JOIN ViewNhanViens AS nvtc ON nvtc.MaNhanVien = sc.MaNhanVienThiCong 
                WHERE (sc.TTHU = 'TT_A') 
                  AND year(sc.ThoiGianHoanUng) = 2025
                  -- AND CONVERT(DATE, sc.ThoiGianHoanUng) > @FilterDate
                  AND sc.ThoiGianHoanUng > '2025/01/02'
                    AND sc.ThoiGianHoanUng < '2025/04/01'  
                  -- AND sc.MaNhanVienKyThuat NOT IN ('hqthong', 'nqhoan', 'dtthang', 'ltthu', 'vddung', 'sutm', 'vhdieu', 'ldthuan', 'hvhan', 'thinpv')
                ORDER BY sc.ThoiGianHoanUng";

            var parameters = new[] { new SqlParameter("@FilterDate", filterDate) };
            var dataTable = await ExecuteQueryAsync(sql, parameters);
            
            var result = new List<SuaChuaModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new SuaChuaModel
                {
                    MADON = row["MADON"]?.ToString() ?? "",
                    ViTriDiemChay = row["ViTriDiemChay"]?.ToString() ?? "",
                    MaNhanVienXayLap = row["MaNhanVienXayLap"]?.ToString() ?? "",
                    NhanVienXayLap = row["NhanVienXayLap"]?.ToString() ?? "",
                    NgayHoanThanh = row["NgayHoanThanh"] as DateTime?,
                    NgayHoanUng = row["NgayHoanUng"] as DateTime?
                });
            }
            
            return result;
        }

        /// <summary>
        /// 4. Lấy chi tiết Sửa Chữa từ ERP
        /// </summary>
        public async Task<List<SuaChuaCTModel>> GetSuaChuaCTDataAsync(string madon)
        {
            var sql = @"
                SELECT sc.MADON, VT.Id as VatTuId, VT.Code as VatTuHangHoa, 
                       VT.TenVatTu, VT.DonViTinh, ROUND(hu.SoLuong, 2) as SoLuongHoanUng
                FROM ct.SuaChuaSuCoNghiemThuCTs AS s 
                INNER JOIN ld.SuaChuaSuCos AS sc ON sc.MADON = s.MADON 
                INNER JOIN ViewNhanViens AS nvtc ON nvtc.MaNhanVien = sc.MaNhanVienThiCong 
                INNER JOIN vt.ViewVatTuHangHoas AS vt ON vt.MaVatTuHangHoa = s.MaVatTuHangHoa 
                INNER JOIN vt.HoanUngVatTus hu on hu.MaSuaChuaSuCoNghiemThuCT = s.Id and hu.MaPhieuXuatKhoVatTuCT is not null 
                WHERE (sc.TTHU = 'TT_A') and s.SoLuongNghiemThu > 0 and s.IsDeleted = 0 
                  AND sc.MADON = @MADON";

            var parameters = new[] { new SqlParameter("@MADON", madon) };
            var dataTable = await ExecuteQueryAsync(sql, parameters);
            
            var result = new List<SuaChuaCTModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new SuaChuaCTModel
                {
                    MADON = row["MADON"]?.ToString() ?? "",
                    MaVTErp = Convert.ToInt32(row["VatTuId"]),
                    SoLuongHoanUng = Convert.ToDecimal(row["SoLuongHoanUng"])
                });
            }
            
            return result;
        }

        /// <summary>
        /// 5. Lấy dữ liệu Nghiệm Thu Giao Khoán từ ERP
        /// </summary>
        public async Task<List<NghiemThuGiaoKhoanModel>> GetNghiemThuGiaoKhoanDataAsync(int soNghiemThu, int namNghiemThu)
        {
            var sql = @"
                SELECT gk.code as SoBGK, n.GiaoKhoanNghiemThuVatTuID, 
                       gk.HoTenNhanVienKyThuat AS NhanVienKyThuat, 
                       nvtc.MaNhanVien AS MaNhanVienXayLap, nvtc.HoVaTen AS NhanVienXayLap,
                       gk.NoiDung, n.SoLanNghiemThu, n.SoNghiemThu, n.Nam as NamNghiemThu
                FROM ct.GiaoKhoanNghiemThuVatTus AS n 
                LEFT JOIN ct.GiaoKhoanNghiemThus nt ON nt.GiaoKhoanNghiemThuID = n.GiaoKhoanNghiemThuID
                LEFT JOIN ct.ViewGiaoKhoans AS gk ON gk.Id = n.GiaoKhoanID
                LEFT JOIN ViewNhanViens nvtc on nvtc.MaNhanVien = n.MaNhanVienThiCong
                WHERE (n.IsDeleted = 0) 
                  AND n.SoNghiemThu = @SoNghiemThu 
                  AND n.nam = @NamNghiemThu";

            var parameters = new[] { 
                new SqlParameter("@SoNghiemThu", soNghiemThu),
                new SqlParameter("@NamNghiemThu", namNghiemThu)
            };
            var dataTable = await ExecuteQueryAsync(sql, parameters);
            
            var result = new List<NghiemThuGiaoKhoanModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new NghiemThuGiaoKhoanModel
                {
                    SoBGK = row["SoBGK"]?.ToString() ?? "",
                    GiaoKhoanNghiemThuVatTuID = Convert.ToInt64(row["GiaoKhoanNghiemThuVatTuID"]),
                    NhanVienKyThuat = row["NhanVienKyThuat"]?.ToString() ?? "",
                    MaNhanVienXayLap = row["MaNhanVienXayLap"]?.ToString() ?? "",
                    NhanVienXayLap = row["NhanVienXayLap"]?.ToString() ?? "",
                    NoiDung = row["NoiDung"]?.ToString() ?? "",
                    SoLanNghiemThu = row["SoLanNghiemThu"] as int?,
                    SoNghiemThu = row["SoNghiemThu"] as int?,
                    NamNghiemThu = row["NamNghiemThu"] as int?
                });
            }
            
            return result;
        }

        /// <summary>
        /// 6. Lấy chi tiết Nghiệm Thu Giao Khoán từ ERP
        /// </summary>
        public async Task<List<NghiemThuGiaoKhoanCTModel>> GetNghiemThuGiaoKhoanCTDataAsync(long giaoKhoanNghiemThuVatTuID)
        {
            var sql = @"
                SELECT c.GiaoKhoanNghiemThuVatTuID, vt.Id as VatTuId, 
                       vt.Code as VatTuHangHoa, vt.TenVatTu, vt.DonViTinh, 
                       SUM(c.SoLuongNghiemThu) as SoLuongHoanUng
                FROM ct.GiaoKhoanNghiemThuVatTuCTs AS c 
                INNER JOIN vt.ViewVatTuHangHoas AS vt ON vt.MaVatTuHangHoa = c.MaVatTuHangHoa
                WHERE (c.IsDeleted = 0) 
                  AND (c.GiaoKhoanNghiemThuVatTuID = @GiaoKhoanNghiemThuVatTuID)
                GROUP BY c.GiaoKhoanNghiemThuVatTuID, vt.Id, vt.TenVatTu, vt.DacTinhKyThuat, vt.Code, vt.DonViTinh";

            var parameters = new[] { new SqlParameter("@GiaoKhoanNghiemThuVatTuID", giaoKhoanNghiemThuVatTuID) };
            var dataTable = await ExecuteQueryAsync(sql, parameters);
            
            var result = new List<NghiemThuGiaoKhoanCTModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add(new NghiemThuGiaoKhoanCTModel
                {
                    GiaoKhoanNghiemThuVatTuID = Convert.ToInt64(row["GiaoKhoanNghiemThuVatTuID"]),
                    MaVTErp = Convert.ToInt32(row["VatTuId"]),
                    VatTuHangHoa = row["VatTuHangHoa"]?.ToString() ?? "",
                    TenVatTu = row["TenVatTu"]?.ToString() ?? "",
                    DonViTinh = row["DonViTinh"]?.ToString() ?? "",
                    SoLuongHoanUng = Convert.ToDecimal(row["SoLuongHoanUng"])
                });
            }
            
            return result;
        }
    }
}