USE QLVT_DB;
GO

-- Thêm menu Đơn vị tính
INSERT INTO Menus (MenuName, ParentMenuID, FormName, Icon, SortOrder, IsActive) 
VALUES ('Đơn vị tính', 2, 'UnitsForm', 'unit.png', 1, 1);

-- Thêm menu Nhà sản xuất  
INSERT INTO Menus (MenuName, ParentMenuID, FormName, Icon, SortOrder, IsActive) 
VALUES ('Nhà sản xuất', 2, 'ManufacturersForm', 'manufacturer.png', 2, 1);

-- Lấy ID của menu vừa tạo
DECLARE @UnitsMenuID INT = (SELECT MenuID FROM Menus WHERE FormName = 'UnitsForm');
DECLARE @ManufacturersMenuID INT = (SELECT MenuID FROM Menus WHERE FormName = 'ManufacturersForm');

-- Cấp quyền cho tất cả roles đối với menu Đơn vị tính
INSERT INTO MenuPermissions (MenuID, RoleID)
SELECT @UnitsMenuID, RoleID FROM Roles WHERE IsActive = 1;

-- Cấp quyền cho tất cả roles đối với menu Nhà sản xuất
INSERT INTO MenuPermissions (MenuID, RoleID)
SELECT @ManufacturersMenuID, RoleID FROM Roles WHERE IsActive = 1;

-- Thêm dữ liệu mẫu cho bảng Units
INSERT INTO Units (MaDVT, TenDVT) VALUES 
('CAI', N'Cái'),
('CHIEC', N'Chiếc'),
('BO', N'Bộ'),
('KG', N'Kilogram'),
('M', N'Mét'),
('M2', N'Mét vuông'),
('M3', N'Mét khối'),
('LIT', N'Lít'),
('TON', N'Tấn'),
('HOP', N'Hộp');

-- Thêm dữ liệu mẫu cho bảng Manufacturers
INSERT INTO Manufacturers (MaNSX, TenNSX) VALUES 
('SONY', N'Sony Corporation'),
('SAMSUNG', N'Samsung Electronics'),
('LG', N'LG Electronics'),
('APPLE', N'Apple Inc.'),
('MICROSOFT', N'Microsoft Corporation'),
('DELL', N'Dell Technologies'),
('HP', N'HP Inc.'),
('ASUS', N'ASUSTeK Computer Inc.'),
('ACER', N'Acer Inc.'),
('LENOVO', N'Lenovo Group Limited');

PRINT 'Đã thêm thành công menu Đơn vị tính và Nhà sản xuất cùng với dữ liệu mẫu!';
GO
