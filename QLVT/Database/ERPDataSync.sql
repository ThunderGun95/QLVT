-- ========================================
-- ERP Data Synchronization Scripts
-- Các hàm để lấy dữ liệu từ ERP và đồng bộ vào QLVT_DB
-- ========================================

USE QLVT_DB;
GO

-- ========================================
-- 1. Hàm lấy dữ liệu cho bảng ct.DonDangKy
-- ========================================
CREATE OR ALTER PROCEDURE sp_SyncDonDangKy
    @FilterDate DATE = '2025-01-01'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX) = N'
    INSERT INTO ct.DonDangKy (MADDK, TENKH, DiaChi, NhanVienKyThuat, MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, NgayHoanUng)
    SELECT 
        mc4.MADDK, 
        mc4.TENKH, 
        mc4.DIACHILD as DiaChi, 
        NhanVienKyThuat, 
        mc4.MaNhanVienXayLap, 
        NhanVienXayLap, 
        mc4.NGAYTC as NgayHoanThanh, 
        hu.ThoiGianHoanUng as NgayHoanUng
    FROM ld.ViewMangCap4s mc4 
    INNER JOIN DONDANGKY dk on dk.MADDK = mc4.MADDK
    INNER JOIN ld.HoanUngs hu on hu.Id = dk.MaPhieuHoanUng
    WHERE mc4.TTDK = ''DK_A'' 
        AND mc4.TTCT = ''CT_A'' 
        AND mc4.TTTC = ''TC_A''
        AND CONVERT(DATE, hu.ThoiGianHoanUng) > ''' + CONVERT(NVARCHAR(10), @FilterDate, 121) + '''
        AND YEAR(mc4.NgayTC) > 2024
        AND mc4.MaNhanVienKyThuat NOT IN (''hqthong'', ''nqhoan'', ''dtthang'', ''ltthu'', ''vddung'', ''sutm'', ''vhdieu'', ''ldthuan'', ''hvhan'', ''thinpv'')
        AND mc4.IsHuy = 0
        AND NOT EXISTS (SELECT 1 FROM ct.DonDangKy WHERE MADDK = mc4.MADDK)
    ORDER BY ThoiGianHoanUng';
    
    EXEC sp_executesql @SQL;
    
    PRINT N'✅ Đã đồng bộ dữ liệu ct.DonDangKy với filter date: ' + CONVERT(NVARCHAR(10), @FilterDate, 121);
END
GO

-- ========================================
-- 2. Hàm lấy dữ liệu cho bảng ct.DonDangKyCT
-- ========================================
CREATE OR ALTER PROCEDURE sp_SyncDonDangKyCT
    @MADDK NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX) = N'
    INSERT INTO ct.DonDangKyCT (MADDK, MaVTErp, SoLuongHoanUng)
    SELECT 
        mc4.MADDK, 
        VT.Id as VatTuId, 
        ROUND(vthu.SOLUONG,2) as SoLuongHoanUng
    FROM ld.ViewMangCap4s mc4
    INNER JOIN ld.HoanUngs h ON h.MADDK = mc4.MADDK
    LEFT JOIN vt.HoanUngVatTus vthu on vthu.MaHoanUng = h.Id and vthu.MaPhieuXuatKhoVatTuCT is not null
    INNER JOIN vt.ViewVatTuHangHoas VT on vt.MaVatTuHangHoa = vthu.MaVatTuHangHoa
    WHERE mc4.TTDK = ''DK_A'' 
        AND mc4.TTCT = ''CT_A'' 
        AND mc4.TTTC = ''TC_A''
        AND CONVERT(DATE, h.ThoiGianHoanUng) > ''2025/01/01''
        AND YEAR(mc4.NgayTC) > 2024
        AND mc4.IsHuy = 0
        AND mc4.MADDK = ''' + @MADDK + '''
        AND NOT EXISTS (SELECT 1 FROM ct.DonDangKyCT WHERE MADDK = mc4.MADDK AND MaVTErp = VT.Id)';
    
    EXEC sp_executesql @SQL;
    
    PRINT N'✅ Đã đồng bộ dữ liệu ct.DonDangKyCT cho MADDK: ' + @MADDK;
END
GO

-- ========================================
-- 3. Hàm lấy dữ liệu cho bảng ct.SuaChua
-- ========================================
CREATE OR ALTER PROCEDURE sp_SyncSuaChua
    @FilterDate DATE = '2025-01-01'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX) = N'
    INSERT INTO ct.SuaChua (MADON, ViTriDiemChay, MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, NgayHoanUng)
    SELECT 
        sc.MADON, 
        sc.ViTriDiemChay, 
        sc.MaNhanVienThiCong as MaNhanVienXayLap, 
        nvtc.HoVaTen as NhanVienXayLap, 
        Sc.ThoiGianHoanThanh as NgayHoanThanh, 
        sc.ThoiGianHoanUng as NgayHoanUng
    FROM ld.SuaChuaSuCos AS sc  
    INNER JOIN ViewNhanViens AS nvtc ON nvtc.MaNhanVien = sc.MaNhanVienThiCong 
    WHERE (sc.TTHU = ''TT_A'') 
        AND year(sc.ThoiGianHoanUng) = 2025
        AND CONVERT(DATE, sc.ThoiGianHoanUng) > ''' + CONVERT(NVARCHAR(10), @FilterDate, 121) + '''
        AND sc.ThoiGianHoanUng > ''2025/01/02''
        AND sc.MaNhanVienKyThuat NOT IN (''hqthong'', ''nqhoan'', ''dtthang'', ''ltthu'', ''vddung'', ''sutm'', ''vhdieu'', ''ldthuan'', ''hvhan'', ''thinpv'')
        AND NOT EXISTS (SELECT 1 FROM ct.SuaChua WHERE MADON = sc.MADON)
    ORDER BY sc.ThoiGianHoanUng';
    
    EXEC sp_executesql @SQL;
    
    PRINT N'✅ Đã đồng bộ dữ liệu ct.SuaChua với filter date: ' + CONVERT(NVARCHAR(10), @FilterDate, 121);
END
GO

-- ========================================
-- 4. Hàm lấy dữ liệu cho bảng ct.SuaChuaCT
-- ========================================
CREATE OR ALTER PROCEDURE sp_SyncSuaChuaCT
    @MADON NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX) = N'
    INSERT INTO ct.SuaChuaCT (MADON, MaVTErp, SoLuongHoanUng)
    SELECT 
        sc.MADON, 
        VT.Id as VatTuId, 
        ROUND(hu.SoLuong, 2) as SoLuongHoanUng
    FROM ct.SuaChuaSuCoNghiemThuCTs AS s 
    INNER JOIN ld.SuaChuaSuCos AS sc ON sc.MADON = s.MADON 
    INNER JOIN ViewNhanViens AS nvtc ON nvtc.MaNhanVien = sc.MaNhanVienThiCong 
    INNER JOIN vt.ViewVatTuHangHoas AS vt ON vt.MaVatTuHangHoa = s.MaVatTuHangHoa 
    INNER JOIN vt.HoanUngVatTus hu on hu.MaSuaChuaSuCoNghiemThuCT = s.Id and hu.MaPhieuXuatKhoVatTuCT is not null 
    WHERE (sc.TTHU = ''TT_A'') 
        AND s.SoLuongNghiemThu > 0 
        AND s.IsDeleted = 0 
        AND sc.MADON = ''' + @MADON + '''
        AND NOT EXISTS (SELECT 1 FROM ct.SuaChuaCT WHERE MADON = sc.MADON AND MaVTErp = VT.Id)';
    
    EXEC sp_executesql @SQL;
    
    PRINT N'✅ Đã đồng bộ dữ liệu ct.SuaChuaCT cho MADON: ' + @MADON;
END
GO

-- ========================================
-- 5. Hàm lấy dữ liệu cho bảng ct.NghiemThuGiaoKhoan
-- ========================================
CREATE OR ALTER PROCEDURE sp_SyncNghiemThuGiaoKhoan
    @SoNghiemThu INT,
    @NamNghiemThu INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX) = N'
    INSERT INTO ct.NghiemThuGiaoKhoan (SoBGK, GiaoKhoanNghiemThuVatTuID, NhanVienKyThuat, MaNhanVienXayLap, NhanVienXayLap, NoiDung, SoLanNghiemThu, SoNghiemThu, NamNghiemThu)
    SELECT 
        gk.code as SoBGK, 
        n.GiaoKhoanNghiemThuVatTuID, 
        gk.HoTenNhanVienKyThuat AS NhanVienKyThuat, 
        nvtc.MaNhanVien AS MaNhanVienXayLap, 
        nvtc.HoVaTen AS NhanVienXayLap,
        gk.NoiDung, 
        n.SoLanNghiemThu, 
        n.SoNghiemThu, 
        n.Nam as NamNghiemThu
    FROM ct.GiaoKhoanNghiemThuVatTus AS n 
    LEFT JOIN ct.GiaoKhoanNghiemThus nt ON nt.GiaoKhoanNghiemThuID = n.GiaoKhoanNghiemThuID
    LEFT JOIN ct.ViewGiaoKhoans AS gk ON gk.Id = n.GiaoKhoanID
    LEFT JOIN ViewNhanViens nvtc on nvtc.MaNhanVien = n.MaNhanVienThiCong
    WHERE (n.IsDeleted = 0) 
        AND n.SoNghiemThu = ' + CONVERT(NVARCHAR(10), @SoNghiemThu) + '
        AND n.nam = ' + CONVERT(NVARCHAR(4), @NamNghiemThu) + '
        AND NOT EXISTS (SELECT 1 FROM ct.NghiemThuGiaoKhoan WHERE GiaoKhoanNghiemThuVatTuID = n.GiaoKhoanNghiemThuVatTuID)';
    
    EXEC sp_executesql @SQL;
    
    PRINT N'✅ Đã đồng bộ dữ liệu ct.NghiemThuGiaoKhoan cho SoNghiemThu: ' + CONVERT(NVARCHAR(10), @SoNghiemThu) + ', Năm: ' + CONVERT(NVARCHAR(4), @NamNghiemThu);
END
GO

-- ========================================
-- 6. Hàm lấy dữ liệu cho bảng ct.NghiemThuGiaoKhoanCT
-- ========================================
CREATE OR ALTER PROCEDURE sp_SyncNghiemThuGiaoKhoanCT
    @GiaoKhoanNghiemThuVatTuID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX) = N'
    INSERT INTO ct.NghiemThuGiaoKhoanCT (GiaoKhoanNghiemThuVatTuID, MaVTErp, SoLuongHoanUng)
    SELECT  
        c.GiaoKhoanNghiemThuVatTuID, 
        vt.Id as VatTuId, 
        SUM(c.SoLuongNghiemThu) as SoLuongHoanUng
    FROM ct.GiaoKhoanNghiemThuVatTuCTs AS c 
    INNER JOIN vt.ViewVatTuHangHoas AS vt ON vt.MaVatTuHangHoa = c.MaVatTuHangHoa
    WHERE (c.IsDeleted = 0) 
        AND (c.GiaoKhoanNghiemThuVatTuID = ' + CONVERT(NVARCHAR(20), @GiaoKhoanNghiemThuVatTuID) + ')
        AND NOT EXISTS (SELECT 1 FROM ct.NghiemThuGiaoKhoanCT WHERE GiaoKhoanNghiemThuVatTuID = c.GiaoKhoanNghiemThuVatTuID AND MaVTErp = vt.Id)
    GROUP BY c.GiaoKhoanNghiemThuVatTuID, vt.Id, vt.TenVatTu, vt.DacTinhKyThuat, vt.Code, vt.DonViTinh';
    
    EXEC sp_executesql @SQL;
    
    PRINT N'✅ Đã đồng bộ dữ liệu ct.NghiemThuGiaoKhoanCT cho GiaoKhoanNghiemThuVatTuID: ' + CONVERT(NVARCHAR(20), @GiaoKhoanNghiemThuVatTuID);
END
GO

-- ========================================
-- Procedure tổng hợp để đồng bộ tất cả dữ liệu
-- ========================================
CREATE OR ALTER PROCEDURE sp_SyncAllERPData
    @FilterDate DATE = '2025-01-01',
    @SampleMADDK NVARCHAR(15) = 'KH2508.0162',
    @SampleMADON NVARCHAR(15) = 'SC2509.0059',
    @SampleSoNghiemThu INT = 1468,
    @SampleNamNghiemThu INT = 2025,
    @SampleGiaoKhoanID BIGINT = 10438
AS
BEGIN
    SET NOCOUNT ON;
    
    PRINT N'========================================';
    PRINT N'🔄 Bắt đầu đồng bộ dữ liệu từ ERP';
    PRINT N'========================================';
    
    BEGIN TRY
        -- 1. Đồng bộ DonDangKy
        EXEC sp_SyncDonDangKy @FilterDate;
        
        -- 2. Đồng bộ DonDangKyCT (với sample MADDK)
        EXEC sp_SyncDonDangKyCT @SampleMADDK;
        
        -- 3. Đồng bộ SuaChua
        EXEC sp_SyncSuaChua @FilterDate;
        
        -- 4. Đồng bộ SuaChuaCT (với sample MADON)
        EXEC sp_SyncSuaChuaCT @SampleMADON;
        
        -- 5. Đồng bộ NghiemThuGiaoKhoan
        EXEC sp_SyncNghiemThuGiaoKhoan @SampleSoNghiemThu, @SampleNamNghiemThu;
        
        -- 6. Đồng bộ NghiemThuGiaoKhoanCT
        EXEC sp_SyncNghiemThuGiaoKhoanCT @SampleGiaoKhoanID;
        
        PRINT N'========================================';
        PRINT N'✅ Hoàn tất đồng bộ tất cả dữ liệu ERP';
        PRINT N'========================================';
        
    END TRY
    BEGIN CATCH
        PRINT N'❌ Lỗi trong quá trình đồng bộ:';
        PRINT ERROR_MESSAGE();
        THROW;
    END CATCH
END
GO

-- ========================================
-- Procedure kiểm tra và thống kê dữ liệu
-- ========================================
CREATE OR ALTER PROCEDURE sp_CheckERPDataStatus
AS
BEGIN
    SET NOCOUNT ON;
    
    PRINT N'========================================';
    PRINT N'📊 THỐNG KÊ DỮ LIỆU ERP TRONG QLVT_DB';
    PRINT N'========================================';
    
    DECLARE @Count INT;
    
    -- Kiểm tra ct.DonDangKy
    SELECT @Count = COUNT(*) FROM ct.DonDangKy;
    PRINT N'📋 ct.DonDangKy: ' + CONVERT(NVARCHAR(10), @Count) + N' records';
    
    -- Kiểm tra ct.DonDangKyCT
    SELECT @Count = COUNT(*) FROM ct.DonDangKyCT;
    PRINT N'📋 ct.DonDangKyCT: ' + CONVERT(NVARCHAR(10), @Count) + N' records';
    
    -- Kiểm tra ct.SuaChua
    SELECT @Count = COUNT(*) FROM ct.SuaChua;
    PRINT N'🔧 ct.SuaChua: ' + CONVERT(NVARCHAR(10), @Count) + N' records';
    
    -- Kiểm tra ct.SuaChuaCT
    SELECT @Count = COUNT(*) FROM ct.SuaChuaCT;
    PRINT N'🔧 ct.SuaChuaCT: ' + CONVERT(NVARCHAR(10), @Count) + N' records';
    
    -- Kiểm tra ct.NghiemThuGiaoKhoan
    SELECT @Count = COUNT(*) FROM ct.NghiemThuGiaoKhoan;
    PRINT N'✅ ct.NghiemThuGiaoKhoan: ' + CONVERT(NVARCHAR(10), @Count) + N' records';
    
    -- Kiểm tra ct.NghiemThuGiaoKhoanCT
    SELECT @Count = COUNT(*) FROM ct.NghiemThuGiaoKhoanCT;
    PRINT N'✅ ct.NghiemThuGiaoKhoanCT: ' + CONVERT(NVARCHAR(10), @Count) + N' records';
    
    PRINT N'========================================';
END
GO

PRINT N'========================================';
PRINT N'✅ ĐÃ TẠO TẤT CẢ STORED PROCEDURES';
PRINT N'========================================';
PRINT N'📚 Các procedure có sẵn:';
PRINT N'1. sp_SyncDonDangKy - Đồng bộ đơn đăng ký';
PRINT N'2. sp_SyncDonDangKyCT - Đồng bộ chi tiết đơn đăng ký';
PRINT N'3. sp_SyncSuaChua - Đồng bộ sửa chữa';
PRINT N'4. sp_SyncSuaChuaCT - Đồng bộ chi tiết sửa chữa';
PRINT N'5. sp_SyncNghiemThuGiaoKhoan - Đồng bộ nghiệm thu giao khoán';
PRINT N'6. sp_SyncNghiemThuGiaoKhoanCT - Đồng bộ chi tiết nghiệm thu giao khoán';
PRINT N'7. sp_SyncAllERPData - Đồng bộ tất cả (với tham số mẫu)';
PRINT N'8. sp_CheckERPDataStatus - Kiểm tra thống kê dữ liệu';
PRINT N'========================================';
PRINT N'🚀 SỬ DỤNG:';
PRINT N'-- Đồng bộ tất cả với filter date:';
PRINT N'EXEC sp_SyncAllERPData ''2025-01-01'';';
PRINT N'';
PRINT N'-- Kiểm tra thống kê:';
PRINT N'EXEC sp_CheckERPDataStatus;';
PRINT N'';
PRINT N'-- Đồng bộ từng bảng riêng lẻ:';
PRINT N'EXEC sp_SyncDonDangKy ''2025-01-01'';';
PRINT N'EXEC sp_SyncDonDangKyCT ''KH2508.0162'';';
PRINT N'-- ... và các procedure khác';
PRINT N'========================================';