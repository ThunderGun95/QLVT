USE QLVT_DB;
GO

-- Thêm menu Phòng ban
INSERT INTO Menus (MenuName, ParentMenuID, FormName, Icon, SortOrder, IsActive) 
VALUES ('Phòng ban', 2, 'DepartmentsForm', 'department.png', 4, 1);

-- Thêm menu Nhân viên
INSERT INTO Menus (MenuName, ParentMenuID, FormName, Icon, SortOrder, IsActive) 
VALUES ('Nhân viên', 2, 'StaffsForm', 'staff.png', 5, 1);

-- Lấy ID của menu vừa tạo
DECLARE @DepartmentsMenuID INT = (SELECT MenuID FROM Menus WHERE FormName = 'DepartmentsForm');
DECLARE @StaffsMenuID INT = (SELECT MenuID FROM Menus WHERE FormName = 'StaffsForm');

-- Cấp quyền cho tất cả roles đối với menu Phòng ban
INSERT INTO MenuPermissions (MenuID, RoleID)
SELECT @DepartmentsMenuID, RoleID FROM Roles WHERE IsActive = 1;

-- Cấp quyền cho tất cả roles đối với menu Nhân viên
INSERT INTO MenuPermissions (MenuID, RoleID)
SELECT @StaffsMenuID, RoleID FROM Roles WHERE IsActive = 1;

-- Thêm dữ liệu mẫu cho bảng Departments
INSERT INTO Departments (MaPB, TenPB) VALUES 
('PB001', N'Phòng Hành chính - Nhân sự'),
('PB002', N'Phòng Kế toán - Tài chính'),
('PB003', N'Phòng Kỹ thuật'),
('PB004', N'Phòng Sản xuất'),
('PB005', N'Phòng Kinh doanh'),
('PB006', N'Phòng Marketing'),
('PB007', N'Phòng Nghiên cứu & Phát triển'),
('PB008', N'Phòng Quản lý chất lượng'),
('PB009', N'Phòng An toàn lao động'),
('PB010', N'Phòng Thông tin công nghệ');

-- Thêm dữ liệu mẫu cho bảng Staffs
INSERT INTO Staffs (ErpIdNV, MaNV, TenNV) VALUES 
('ERP_NV001', 'NV001', N'Nguyễn Văn An'),
('ERP_NV002', 'NV002', N'Trần Thị Bình'),
('ERP_NV003', 'NV003', N'Lê Văn Cường'),
('ERP_NV004', 'NV004', N'Phạm Thị Dung'),
('ERP_NV005', 'NV005', N'Hoàng Văn Đức'),
('ERP_NV006', 'NV006', N'Võ Thị Hoa'),
('ERP_NV007', 'NV007', N'Đặng Văn Hưng'),
('ERP_NV008', 'NV008', N'Ngô Thị Lan'),
('ERP_NV009', 'NV009', N'Bùi Văn Minh'),
('ERP_NV010', 'NV010', N'Lý Thị Nga'),
('ERP_NV011', 'NV011', N'Phan Văn Phúc'),
('ERP_NV012', 'NV012', N'Trương Thị Quỳnh'),
('ERP_NV013', 'NV013', N'Vũ Văn Sơn'),
('ERP_NV014', 'NV014', N'Đinh Thị Thảo'),
('ERP_NV015', 'NV015', N'Mai Văn Tuấn'),
('ERP_NV016', 'NV016', N'Lương Thị Uyên'),
('ERP_NV017', 'NV017', N'Đỗ Văn Vinh'),
('ERP_NV018', 'NV018', N'Kiều Thị Xuân'),
('ERP_NV019', 'NV019', N'Tô Văn Yên'),
('ERP_NV020', 'NV020', N'Chu Thị Zung');

PRINT 'Đã thêm thành công menu Phòng ban, Nhân viên và dữ liệu mẫu!';
GO
