-- ========================================
-- QLVT Database Creation Script
-- ========================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'QLVT_DB')
BEGIN
    CREATE DATABASE QLVT_DB;
END
GO

USE QLVT_DB;
GO

-- ========================================
-- Create Tables
-- ========================================

-- Units Table (Đơn vị tính)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Units' AND xtype='U')
BEGIN
    CREATE TABLE Units (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MaDVT NVARCHAR(10) NOT NULL UNIQUE,
        TenDVT NVARCHAR(100) NOT NULL
    );
END
GO

-- Manufacturers Table (Nhà sản xuất)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Manufacturers' AND xtype='U')
BEGIN
    CREATE TABLE Manufacturers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MaNSX NVARCHAR(10) NOT NULL UNIQUE,
        TenNSX NVARCHAR(200) NOT NULL
    );
END
GO

-- Supplies Table (Vật tư)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Supplies' AND xtype='U')
BEGIN
    CREATE TABLE Supplies (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ErpId INT NULL,
        Code NVARCHAR(20) NOT NULL UNIQUE,
        TenVatTu NVARCHAR(500) NOT NULL,
        DacTinhKyThuat NVARCHAR(1000) NULL,
        MaDVT NVARCHAR(10) NOT NULL,
        FOREIGN KEY (MaDVT) REFERENCES Units(MaDVT)
    );
END
GO

-- Departments Table (Phòng ban)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Departments' AND xtype='U')
BEGIN
    CREATE TABLE Departments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MaPB NVARCHAR(10) NOT NULL UNIQUE,
        TenPB NVARCHAR(200) NOT NULL
    );
END
GO

-- Staffs Table (Nhân viên)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Staffs' AND xtype='U')
BEGIN
    CREATE TABLE Staffs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ErpIdNV NVARCHAR(20) NULL,
        MaNV NVARCHAR(15) NOT NULL UNIQUE,
        TenNV NVARCHAR(100) NOT NULL
    );
END
GO

-- Users Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        UserID INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(256) NOT NULL,
        FullName NVARCHAR(100),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        LastLogin DATETIME NULL
    );
END
GO

-- Roles Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' AND xtype='U')
BEGIN
    CREATE TABLE Roles (
        RoleID INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL UNIQUE,
        Description NVARCHAR(200),
        IsActive BIT DEFAULT 1
    );
END
GO

-- UserRoles Table (Many-to-Many relationship)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
BEGIN
    CREATE TABLE UserRoles (
        UserID INT NOT NULL,
        RoleID INT NOT NULL,
        AssignedDate DATETIME DEFAULT GETDATE(),
        PRIMARY KEY(UserID, RoleID),
        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
        FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE CASCADE
    );
END
GO

-- Menus Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Menus' AND xtype='U')
BEGIN
    CREATE TABLE Menus (
        MenuID INT IDENTITY(1,1) PRIMARY KEY,
        MenuName NVARCHAR(100) NOT NULL,
        ParentID INT NULL,
        FormName NVARCHAR(100),
        SortOrder INT DEFAULT 0,
        MenuIcon NVARCHAR(50),
        IsActive BIT DEFAULT 1,
        FOREIGN KEY (ParentID) REFERENCES Menus(MenuID)
    );
END
GO

-- Permissions Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Permissions' AND xtype='U')
BEGIN
    CREATE TABLE Permissions (
        RoleID INT NOT NULL,
        MenuID INT NOT NULL,
        CanAccess BIT DEFAULT 1,
        CanCreate BIT DEFAULT 0,
        CanRead BIT DEFAULT 1,
        CanUpdate BIT DEFAULT 0,
        CanDelete BIT DEFAULT 0,
        PRIMARY KEY(RoleID, MenuID),
        FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE CASCADE,
        FOREIGN KEY (MenuID) REFERENCES Menus(MenuID) ON DELETE CASCADE
    );
END
GO

-- ========================================
-- Insert Sample Data
-- ========================================

-- Insert Roles
IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName = 'Administrator')
BEGIN
    INSERT INTO Roles (RoleName, Description) VALUES 
    ('Administrator', N'Quản trị viên hệ thống - có toàn quyền'),
    ('Manager', N'Quản lý - có quyền hạn cao'),
    ('User', N'Người dùng thường - quyền hạn giới hạn'),
    ('Guest', N'Khách - chỉ xem');
END
GO

-- Insert Sample Users (Password: "123456" -> SHA256)
-- SHA256 of "123456": 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, FullName, IsActive) VALUES 
    ('admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Quản trị viên', 1),
    ('manager', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Nguyễn Văn Quản lý', 1),
    ('user1', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Trần Thị User', 1),
    ('guest', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Khách', 1);
END
GO

-- Assign Roles to Users
IF NOT EXISTS (SELECT * FROM UserRoles WHERE UserID = 1 AND RoleID = 1)
BEGIN
    INSERT INTO UserRoles (UserID, RoleID) VALUES 
    (1, 1), -- admin has Administrator role
    (2, 2), -- manager has Manager role
    (3, 3), -- user1 has User role
    (4, 4); -- guest has Guest role
END
GO

-- Insert Menu Items
IF NOT EXISTS (SELECT * FROM Menus WHERE MenuName = N'Hệ thống')
BEGIN
    INSERT INTO Menus (MenuName, ParentID, FormName, SortOrder, MenuIcon) VALUES 
    -- Main Menus
    (N'Hệ thống', NULL, NULL, 1, 'system'),
    (N'Quản lý', NULL, NULL, 2, 'management'),
    (N'Báo cáo', NULL, NULL, 3, 'report'),
    (N'Tiện ích', NULL, NULL, 4, 'utilities'),
    
    -- System Sub-menus
    (N'Thông tin tài khoản', 1, 'UserProfileForm', 1, 'user'),
    (N'Đổi mật khẩu', 1, 'ChangePasswordForm', 2, 'password'),
    (N'Quản lý người dùng', 1, 'UserManagementForm', 3, 'users'),
    (N'Phân quyền', 1, 'PermissionForm', 4, 'permissions'),
    (N'Đăng xuất', 1, 'LogoutAction', 5, 'logout'),
    
    -- Management Sub-menus
    (N'Quản lý vật tư', 2, 'MaterialManagementForm', 1, 'materials'),
    (N'Quản lý kho', 2, 'WarehouseManagementForm', 2, 'warehouse'),
    (N'Nhập kho', 2, 'ImportForm', 3, 'import'),
    (N'Xuất kho', 2, 'ExportForm', 4, 'export'),
    
    -- Report Sub-menus
    (N'Báo cáo tồn kho', 3, 'InventoryReportForm', 1, 'inventory'),
    (N'Báo cáo xuất nhập', 3, 'TransactionReportForm', 2, 'transaction'),
    
    -- Utilities Sub-menus
    (N'Sao lưu dữ liệu', 4, 'BackupForm', 1, 'backup'),
    (N'Cài đặt hệ thống', 4, 'SettingsForm', 2, 'settings');
END
GO

-- Insert Permissions
IF NOT EXISTS (SELECT * FROM Permissions WHERE RoleID = 1 AND MenuID = 1)
BEGIN
    -- Administrator - Full Access to All Menus
    DECLARE @MenuCount INT = (SELECT COUNT(*) FROM Menus);
    DECLARE @MenuID INT = 1;
    
    WHILE @MenuID <= @MenuCount
    BEGIN
        INSERT INTO Permissions (RoleID, MenuID, CanAccess, CanCreate, CanRead, CanUpdate, CanDelete) 
        VALUES (1, @MenuID, 1, 1, 1, 1, 1);
        SET @MenuID = @MenuID + 1;
    END
    
    -- Manager - Access to most menus except user management
    INSERT INTO Permissions (RoleID, MenuID, CanAccess, CanCreate, CanRead, CanUpdate, CanDelete) VALUES 
    (2, 1, 1, 0, 1, 0, 0),  -- System menu
    (2, 2, 1, 1, 1, 1, 0),  -- Management menu
    (2, 3, 1, 0, 1, 0, 0),  -- Report menu
    (2, 4, 1, 0, 1, 0, 0),  -- Utilities menu
    (2, 5, 1, 0, 1, 1, 0),  -- User profile
    (2, 6, 1, 0, 1, 1, 0),  -- Change password
    (2, 9, 1, 0, 1, 0, 0),  -- Logout
    (2, 10, 1, 1, 1, 1, 0), -- Material management
    (2, 11, 1, 1, 1, 1, 0), -- Warehouse management
    (2, 12, 1, 1, 1, 1, 0), -- Import
    (2, 13, 1, 1, 1, 1, 0), -- Export
    (2, 14, 1, 0, 1, 0, 0), -- Inventory report
    (2, 15, 1, 0, 1, 0, 0); -- Transaction report
    
    -- User - Limited access
    INSERT INTO Permissions (RoleID, MenuID, CanAccess, CanCreate, CanRead, CanUpdate, CanDelete) VALUES 
    (3, 1, 1, 0, 1, 0, 0),  -- System menu
    (3, 2, 1, 0, 1, 0, 0),  -- Management menu (limited)
    (3, 5, 1, 0, 1, 1, 0),  -- User profile
    (3, 6, 1, 0, 1, 1, 0),  -- Change password
    (3, 9, 1, 0, 1, 0, 0),  -- Logout
    (3, 10, 1, 0, 1, 0, 0), -- Material management (read only)
    (3, 11, 1, 0, 1, 0, 0); -- Warehouse management (read only)
    
    -- Guest - Very limited access
    INSERT INTO Permissions (RoleID, MenuID, CanAccess, CanCreate, CanRead, CanUpdate, CanDelete) VALUES 
    (4, 1, 1, 0, 1, 0, 0),  -- System menu
    (4, 5, 1, 0, 1, 0, 0),  -- User profile (read only)
    (4, 6, 1, 0, 1, 1, 0),  -- Change password
    (4, 9, 1, 0, 1, 0, 0);  -- Logout
END
GO

-- ========================================
-- Inventory Management Tables
-- ========================================

-- Warehouses Table (Kho)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Warehouses' AND xtype='U')
BEGIN
    CREATE TABLE Warehouses (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MaKho NVARCHAR(20) NOT NULL UNIQUE,
        TenKho NVARCHAR(100) NOT NULL,
        LoaiKho NVARCHAR(20) NOT NULL, -- 'COMPANY' hoặc 'PERSONAL'
        MaNV NVARCHAR(20) NULL, -- Chỉ có giá trị nếu LoaiKho = 'PERSONAL'
        DiaChi NVARCHAR(255) NULL,
        GhiChu NVARCHAR(255) NULL,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
    
    CREATE INDEX IX_Warehouses_MaKho ON Warehouses(MaKho);
    CREATE INDEX IX_Warehouses_LoaiKho ON Warehouses(LoaiKho);
    PRINT N'✅ Đã tạo bảng Warehouses';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng Warehouses đã tồn tại';
END
GO

-- Inventory Table (Tồn kho)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Inventory' AND xtype='U')
BEGIN
    CREATE TABLE Inventory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        WarehouseId INT NOT NULL, -- Thay đổi từ NVARCHAR(20) thành INT
        SupplyErpId INT NOT NULL,
        SoLuongTon INT NOT NULL DEFAULT 0, -- Số lượng có thực
        
        FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id), -- Sửa FK reference
        UNIQUE(WarehouseId, SupplyErpId) -- Mỗi vật tư chỉ có 1 record trong 1 kho
    );
    
    CREATE INDEX IX_Inventory_MaKho ON Inventory(WarehouseId);
    CREATE INDEX IX_Inventory_ErpId ON Inventory(SupplyErpId);
    PRINT N'✅ Đã tạo bảng Inventory';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng Inventory đã tồn tại';
END
GO

-- Transactions Table (Giao dịch)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U')
BEGIN
    CREATE TABLE Transactions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SoPhieu NVARCHAR(50) NOT NULL UNIQUE,
        NgayGiaoDich DATETIME NOT NULL DEFAULT GETDATE(),
        LoaiGiaoDich NVARCHAR(20) NOT NULL, -- 'NhapKho', 'XuatKho', 'TraKho', 'HoanUng'
        MaKhoNguon INT NULL, -- Kho xuất (null với nhập kho) - Thay đổi thành INT
        MaKhoNhan INT NULL, -- Kho nhận (null với hoàn ứng) - Thay đổi thành INT
        MaNV NVARCHAR(20) NOT NULL, -- Nhân viên thực hiện
        GhiChu NVARCHAR(255) NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        
        -- Entity references cho từng loại giao dịch
        EntityNhapKho NVARCHAR(100) NULL, -- Phiếu ERP hoặc nguồn nhập
        EntityXuatKho NVARCHAR(100) NULL, -- Lý do xuất (dự án, công việc)
        EntityTraKho NVARCHAR(100) NULL, -- Lý do trả
        EntityHoanUng NVARCHAR(100) NULL, -- Lý do hoàn ứng

        IsDeleted BIT DEFAULT 0,
        DeletedBy NVARCHAR(50),
        DeletedDate DATETIME DEFAULT GETDATE(),

        ModifiedBy NVARCHAR(50),
        LastModifiedDate DATETIME DEFAULT GETDATE(),
        
        FOREIGN KEY (MaKhoNguon) REFERENCES Warehouses(Id), -- Sửa FK reference
        FOREIGN KEY (MaKhoNhan) REFERENCES Warehouses(Id)   -- Sửa FK reference
    );
    
    CREATE INDEX IX_Transactions_SoPhieu ON Transactions(SoPhieu);
    CREATE INDEX IX_Transactions_LoaiGiaoDich ON Transactions(LoaiGiaoDich);
    CREATE INDEX IX_Transactions_NgayGiaoDich ON Transactions(NgayGiaoDich);
    PRINT N'✅ Đã tạo bảng Transactions';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng Transactions đã tồn tại';
END
GO

-- TransactionDetails Table (Chi tiết giao dịch)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TransactionDetails' AND xtype='U')
BEGIN
    CREATE TABLE TransactionDetails (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TransactionId INT NOT NULL,
        ErpId INT NOT NULL,
        MaKhoXuat INT NULL, -- Kho xuất (null với nhập kho) - Thay đổi thành INT
        MaKhoNhan INT NULL, -- Kho xuất (null với nhập kho) - Thay đổi thành INT
        SoLuong INT NOT NULL,
        GhiChu NVARCHAR(255) NULL,

        CreatedBy NVARCHAR(50) NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),

        IsDeleted BIT DEFAULT 0,
        DeletedBy NVARCHAR(50),
        DeletedDate DATETIME DEFAULT GETDATE(),

        ModifiedBy NVARCHAR(50),
        LastModifiedDate DATETIME DEFAULT GETDATE(),

        
        FOREIGN KEY (TransactionId) REFERENCES Transactions(Id) ON DELETE CASCADE,
        FOREIGN KEY (SourceWarehouseId) REFERENCES Warehouses(Id)
    );
    
    CREATE INDEX IX_TransactionDetails_TransactionId ON TransactionDetails(TransactionId);
    CREATE INDEX IX_TransactionDetails_ErpId ON TransactionDetails(ErpId);
    CREATE INDEX IX_TransactionDetails_SourceWarehouseId ON TransactionDetails(SourceWarehouseId);
    PRINT N'✅ Đã tạo bảng TransactionDetails với SourceWarehouseId';
END
ELSE
BEGIN
    -- Thêm cột SourceWarehouseId nếu chưa có
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionDetails' AND COLUMN_NAME = 'SourceWarehouseId')
    BEGIN
        ALTER TABLE TransactionDetails ADD SourceWarehouseId INT NULL;
        ALTER TABLE TransactionDetails ADD CONSTRAINT FK_TransactionDetails_SourceWarehouse 
            FOREIGN KEY (SourceWarehouseId) REFERENCES Warehouses(Id);
        CREATE INDEX IX_TransactionDetails_SourceWarehouseId ON TransactionDetails(SourceWarehouseId);
        PRINT N'✅ Đã thêm cột SourceWarehouseId vào TransactionDetails';
    END
    ELSE
    BEGIN
        PRINT N'⚠️ Cột SourceWarehouseId đã tồn tại trong TransactionDetails';
    END
END
GO


-- ========================================
-- Insert Sample Warehouse Data
-- ========================================

-- Insert sample warehouses
IF NOT EXISTS (SELECT * FROM Warehouses WHERE MaKho = 'COMPANY')
BEGIN
    INSERT INTO Warehouses (MaKho, TenKho, LoaiKho, DiaChi, GhiChu) VALUES 
    ('COMPANY', N'Kho công ty chính', 'COMPANY', N'Tầng 1, Tòa nhà ABC', N'Kho chính của công ty'),
    ('KHO_NV001', N'Kho cá nhân NV001', 'PERSONAL', N'Phòng kỹ thuật', N'Kho cá nhân nhân viên NV001'),
    ('KHO_NV002', N'Kho cá nhân NV002', 'PERSONAL', N'Phòng thiết kế', N'Kho cá nhân nhân viên NV002');
    
    PRINT N'✅ Đã thêm dữ liệu mẫu Warehouses';
END
ELSE
BEGIN
    PRINT N'⚠️ Dữ liệu mẫu Warehouses đã tồn tại';
END
GO

-- ========================================
-- ERP Integration Tables (CT Schema)
-- ========================================

-- Create CT schema if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'ct')
BEGIN
    EXEC('CREATE SCHEMA ct');
    PRINT N'✅ Đã tạo schema ct';
END
ELSE
BEGIN
    PRINT N'⚠️ Schema ct đã tồn tại';
END
GO

-- 1. Bảng ct.DonDangKy
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DonDangKy' AND xtype='U' AND uid = SCHEMA_ID('ct'))
BEGIN
    CREATE TABLE ct.DonDangKy (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MADDK NVARCHAR(15) NOT NULL,
        TENKH NVARCHAR(500) NULL,
        DiaChi NVARCHAR(500) NULL,
        NhanVienKyThuat NVARCHAR(200) NULL,
        MaNhanVienXayLap NVARCHAR(20) NULL,
        NhanVienXayLap NVARCHAR(200) NULL,
        NgayHoanThanh DATETIME NULL,
        NgayHoanUng DATETIME NULL,
        DaHoanUng BIT DEFAULT 0,
        ThoiGianXacNhanHoanUng DATETIME NULL,
        MaNVXacNhan NVARCHAR(20) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE()
    );
    
    CREATE INDEX IX_DonDangKy_MADDK ON ct.DonDangKy(MADDK);
    CREATE INDEX IX_DonDangKy_NgayHoanUng ON ct.DonDangKy(NgayHoanUng);
    PRINT N'✅ Đã tạo bảng ct.DonDangKy';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng ct.DonDangKy đã tồn tại';
END
GO

-- 2. Bảng ct.DonDangKyCT
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DonDangKyCT' AND xtype='U' AND uid = SCHEMA_ID('ct'))
BEGIN
    CREATE TABLE ct.DonDangKyCT (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MADDK NVARCHAR(15) NOT NULL,
        MaVTErp INT NOT NULL,
        SoLuongHoanUng DECIMAL(18,2) NOT NULL DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE(),
        
        FOREIGN KEY (MaVTErp) REFERENCES Supplies(ErpId)
    );
    
    CREATE INDEX IX_DonDangKyCT_MADDK ON ct.DonDangKyCT(MADDK);
    CREATE INDEX IX_DonDangKyCT_MaVTErp ON ct.DonDangKyCT(MaVTErp);
    PRINT N'✅ Đã tạo bảng ct.DonDangKyCT';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng ct.DonDangKyCT đã tồn tại';
END
GO

-- 3. Bảng ct.SuaChua
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SuaChua' AND xtype='U' AND uid = SCHEMA_ID('ct'))
BEGIN
    CREATE TABLE ct.SuaChua (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MADON NVARCHAR(15) NOT NULL,
        ViTriDiemChay NVARCHAR(500) NULL,
        MaNhanVienXayLap NVARCHAR(20) NULL,
        NhanVienXayLap NVARCHAR(200) NULL,
        NgayHoanThanh DATETIME NULL,
        NgayHoanUng DATETIME NULL,
        DaHoanUng BIT DEFAULT 0,
        ThoiGianXacNhanHoanUng DATETIME NULL,
        MaNVXacNhan NVARCHAR(20) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE()
    );
    
    CREATE INDEX IX_SuaChua_MADON ON ct.SuaChua(MADON);
    CREATE INDEX IX_SuaChua_NgayHoanUng ON ct.SuaChua(NgayHoanUng);
    PRINT N'✅ Đã tạo bảng ct.SuaChua';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng ct.SuaChua đã tồn tại';
END
GO

-- 4. Bảng ct.SuaChuaCT
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SuaChuaCT' AND xtype='U' AND uid = SCHEMA_ID('ct'))
BEGIN
    CREATE TABLE ct.SuaChuaCT (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MADON NVARCHAR(15) NOT NULL,
        MaVTErp INT NOT NULL,
        SoLuongHoanUng DECIMAL(18,2) NOT NULL DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE(),
        
        FOREIGN KEY (MaVTErp) REFERENCES Supplies(ErpId)
    );
    
    CREATE INDEX IX_SuaChuaCT_MADON ON ct.SuaChuaCT(MADON);
    CREATE INDEX IX_SuaChuaCT_MaVTErp ON ct.SuaChuaCT(MaVTErp);
    PRINT N'✅ Đã tạo bảng ct.SuaChuaCT';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng ct.SuaChuaCT đã tồn tại';
END
GO

-- 5. Bảng ct.NghiemThuGiaoKhoan
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NghiemThuGiaoKhoan' AND xtype='U' AND uid = SCHEMA_ID('ct'))
BEGIN
    CREATE TABLE ct.NghiemThuGiaoKhoan (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SoBGK NVARCHAR(15) NOT NULL,
        GiaoKhoanNghiemThuVatTuID BIGINT NOT NULL,
        NhanVienKyThuat NVARCHAR(200) NULL,
        MaNhanVienXayLap NVARCHAR(20) NULL,
        NhanVienXayLap NVARCHAR(200) NULL,
        NoiDung NVARCHAR(1000) NULL,
        SoLanNghiemThu INT NULL,
        SoNghiemThu INT NULL,
        NamNghiemThu INT NULL,
        NgayHoanUng DATETIME NULL,
        DaHoanUng BIT DEFAULT 0,
        ThoiGianXacNhanHoanUng DATETIME NULL,
        MaNVXacNhan NVARCHAR(20) NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME DEFAULT GETDATE()
    );
    
    CREATE INDEX IX_NghiemThuGiaoKhoan_SoBGK ON ct.NghiemThuGiaoKhoan(SoBGK);
    CREATE INDEX IX_NghiemThuGiaoKhoan_GiaoKhoanID ON ct.NghiemThuGiaoKhoan(GiaoKhoanNghiemThuVatTuID);
    CREATE INDEX IX_NghiemThuGiaoKhoan_SoNam ON ct.NghiemThuGiaoKhoan(SoNghiemThu, NamNghiemThu);
    PRINT N'✅ Đã tạo bảng ct.NghiemThuGiaoKhoan';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng ct.NghiemThuGiaoKhoan đã tồn tại';
END
GO

-- 6. Bảng ct.NghiemThuGiaoKhoanCT
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NghiemThuGiaoKhoanCT' AND xtype='U' AND uid = SCHEMA_ID('ct'))
BEGIN
    CREATE TABLE ct.NghiemThuGiaoKhoanCT (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        GiaoKhoanNghiemThuVatTuID BIGINT NOT NULL,
        MaVTErp INT NOT NULL,
        SoLuongHoanUng DECIMAL(18,2) NOT NULL DEFAULT 0,
        NgayHoanUng DATETIME NULL,
        DaHoanUng BIT DEFAULT 0,
        ThoiGianXacNhanHoanUng DATETIME NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        
        FOREIGN KEY (MaVTErp) REFERENCES Supplies(ErpId)
    );
    
    CREATE INDEX IX_NghiemThuGiaoKhoanCT_GiaoKhoanID ON ct.NghiemThuGiaoKhoanCT(GiaoKhoanNghiemThuVatTuID);
    CREATE INDEX IX_NghiemThuGiaoKhoanCT_MaVTErp ON ct.NghiemThuGiaoKhoanCT(MaVTErp);
    PRINT N'✅ Đã tạo bảng ct.NghiemThuGiaoKhoanCT';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng ct.NghiemThuGiaoKhoanCT đã tồn tại';
END
GO

PRINT N'========================================';
PRINT N'✅ Hoàn tất tạo 6 bảng ERP Integration';
PRINT N'1. ct.DonDangKy - Đơn đăng ký';
PRINT N'2. ct.DonDangKyCT - Chi tiết đơn đăng ký';
PRINT N'3. ct.SuaChua - Sửa chữa';
PRINT N'4. ct.SuaChuaCT - Chi tiết sửa chữa';
PRINT N'5. ct.NghiemThuGiaoKhoan - Nghiệm thu giao khoán';
PRINT N'6. ct.NghiemThuGiaoKhoanCT - Chi tiết nghiệm thu giao khoán';
PRINT N'========================================';

-- ========================================
-- Menu System Tables
-- ========================================

-- Menus Table (Menu hệ thống)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Menus' AND xtype='U')
BEGIN
    CREATE TABLE Menus (
        MenuID INT IDENTITY(1,1) PRIMARY KEY,
        MenuName NVARCHAR(100) NOT NULL,
        ParentID INT NULL,
        FormName NVARCHAR(100) NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        MenuIcon NVARCHAR(50) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (ParentID) REFERENCES Menus(MenuID)
    );
    
    PRINT N'✅ Đã tạo bảng Menus';
END
ELSE
BEGIN
    PRINT N'⚠️ Bảng Menus đã tồn tại';
END
GO

-- Insert sample menu data
IF NOT EXISTS (SELECT * FROM Menus WHERE MenuID = 1)
BEGIN
    INSERT INTO Menus (MenuName, ParentID, FormName, SortOrder, MenuIcon) VALUES
    -- Main menus
    (N'Tác vụ', NULL, NULL, 1, '🔧'),
    (N'Báo cáo', NULL, NULL, 2, '📊'),
    (N'Danh mục', NULL, NULL, 3, '📝'),
    (N'ERP', NULL, NULL, 4, '🔄'),
    
    -- Tác vụ sub-menus
    (N'Nhập tồn đầu kỳ', 1, 'OpeningInventoryUserControl', 1, '📋'),
    (N'Nhập kho vật tư', 1, 'ImportTaskUserControl', 2, '📦'),
    (N'Xuất kho vật tư', 1, 'ExportTaskUserControl', 3, '📤'),
    (N'Trả kho vật tư', 1, 'TraKhoForm', 4, '🔄'),
    (N'Hoàn ứng mạng cấp 4', 1, 'HoanUngMC4Form', 5, '↩️'),
    (N'Hoàn ứng điểm chảy', 1, 'HoanUngDCForm', 6, '↩️'),
    (N'Hoàn ứng BGK', 1, 'HoanUngBGKUserControl', 7, '↩️'),
    (N'Hoàn ứng (ngoài ERP)', 1, 'HoanUngForm', 8, '↩️'),
    
    -- Báo cáo sub-menus
    (N'Báo cáo tồn kho', 2, 'BaoCaoTonKhoUserControl', 1, '📊'),
    (N'Báo cáo xuất nhập tồn', 2, 'BaoCaoXuatNhapTonUserControl', 2, '📊'),
    (N'Báo cáo xuất nhập tồn chi tiết', 2, 'BaoCaoXuatNhapTonChiTietForm', 3, '📊'),
    
    -- Danh mục sub-menus
    (N'Đơn vị tính', 3, 'UnitsUserControl', 1, '📏'),
    (N'Nhà sản xuất', 3, 'ManufacturersUserControl', 2, '🏭'),
    (N'Vật tư', 3, 'SuppliesUserControl', 3, '📦'),
    (N'Phòng ban', 3, 'DepartmentsUserControl', 4, '🏢'),
    (N'Nhân viên', 3, 'StaffsUserControl', 5, '👥'),
    
    -- ERP sub-menus
    (N'Đồng bộ ERP', 4, 'ERPSyncUserControl', 1, '🔄');
    
    PRINT N'✅ Đã thêm dữ liệu mẫu cho bảng Menus';
END
GO

PRINT N'========================================';
PRINT N'✅ Hoàn tất tạo hệ thống Menu';
PRINT N'Bảng Menus đã được tạo với dữ liệu mẫu';
PRINT N'========================================';

PRINT 'Database and sample data created successfully!';
PRINT 'Sample Login Accounts:';
PRINT 'Username: admin, Password: 123456 (Administrator)';
PRINT 'Username: manager, Password: 123456 (Manager)';
PRINT 'Username: user1, Password: 123456 (User)';
PRINT 'Username: guest, Password: 123456 (Guest)';
