USE QLVT_DB;
GO

-- Thêm menu Vật tư
INSERT INTO Menus (MenuName, ParentMenuID, FormName, Icon, SortOrder, IsActive) 
VALUES ('Vật tư', 2, 'SuppliesForm', 'supply.png', 3, 1);

-- Lấy ID của menu vừa tạo
DECLARE @SuppliesMenuID INT = (SELECT MenuID FROM Menus WHERE FormName = 'SuppliesForm');

-- Cấp quyền cho tất cả roles đối với menu Vật tư
INSERT INTO MenuPermissions (MenuID, RoleID)
SELECT @SuppliesMenuID, RoleID FROM Roles WHERE IsActive = 1;

-- Thêm dữ liệu mẫu cho bảng Supplies
INSERT INTO Supplies (ErpId, Code, TenVatTu, DacTinhKyThuat, MaDVT, MaNSX) VALUES 
('ERP001', 'VT001', N'Điện trở 10K Ohm', N'Điện trở carbon, công suất 1/4W, sai số ±5%', 'CAI', 'SONY'),
('ERP002', 'VT002', N'Tụ điện 100uF/25V', N'Tụ điện nhôm, nhiệt độ -40°C đến +85°C', 'CAI', 'SAMSUNG'),
('ERP003', 'VT003', N'IC LM358', N'IC khuếch đại thuật toán kép, nguồn ±15V', 'CAI', 'LG'),
('ERP004', 'VT004', N'Diode 1N4007', N'Diode chỉnh lưu 1A/1000V', 'CAI', 'APPLE'),
('ERP005', 'VT005', N'Transistor BC547', N'Transistor NPN, Ic=100mA, Vceo=45V', 'CAI', 'MICROSOFT'),
('ERP006', 'VT006', N'LED đỏ 5mm', N'LED phát sáng đỏ, góc chiếu 30°, 20mA', 'CAI', 'DELL'),
('ERP007', 'VT007', N'Cuộn dây 100mH', N'Cuộn cảm xoắn ốc, dòng định mức 500mA', 'CAI', 'HP'),
('ERP008', 'VT008', N'Nút bấm tạm', N'Công tắc nhấn nhả, 12V/1A', 'CAI', 'ASUS'),
('ERP009', 'VT009', N'Dây đồng 1.5mm²', N'Dây đồng đơn cứng, cách điện PVC', 'M', 'ACER'),
('ERP010', 'VT010', N'Ốc vít M3x10', N'Ốc vít đầu chìm, thép mạ kẽm', 'CAI', 'LENOVO'),
('ERP011', 'VT011', N'Đế IC 8 chân', N'Đế IC DIP-8, chất liệu nhựa phenolic', 'CAI', 'SONY'),
('ERP012', 'VT012', N'Breadboard 400 lỗ', N'Bảng thử nghiệm không hàn, 400 điểm', 'CHIEC', 'SAMSUNG'),
('ERP013', 'VT013', N'Màn hình LCD 16x2', N'LCD ký tự xanh dương, giao tiếp HD44780', 'CHIEC', 'LG'),
('ERP014', 'VT014', N'Động cơ servo SG90', N'Servo 9g, góc quay 180°, 4.8-6V', 'CAI', 'APPLE'),
('ERP015', 'VT015', N'Cảm biến nhiệt độ DS18B20', N'Cảm biến nhiệt độ số, giao thức 1-Wire', 'CAI', 'MICROSOFT');

PRINT 'Đã thêm thành công menu Vật tư và dữ liệu mẫu!';
GO
