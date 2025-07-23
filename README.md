# QLVT - Hệ thống Quản lý Vật tư

## Mô tả
QLVT là một ứng dụng WinForms được xây dựng bằng C# (.NET 8) để quản lý vật tư với hệ thống đăng nhập và phân quyền menu động.

## Tính năng đã hoàn thành (Giai đoạn 1 & 2)

### 🔐 Hệ thống đăng nhập
- Form đăng nhập với xác thực username/password
- Mật khẩu được mã hóa SHA256
- Kiểm tra kết nối database
- Cập nhật thời gian đăng nhập cuối

### 👥 Phân quyền người dùng
- Hệ thống Role-based Access Control (RBAC)
- Menu động dựa trên quyền của user
- 4 role mặc định: Administrator, Manager, User, Guest
- Kiểm tra quyền trước khi truy cập chức năng

### 🎯 Giao diện chính
- MainForm với MenuStrip động
- StatusBar hiển thị thông tin user và thời gian
- Form đổi mật khẩu
- Form thông tin tài khoản

## Cấu trúc dự án

```
QLVT/
├── Database/
│   └── CreateDatabase.sql      # Script tạo database và dữ liệu mẫu
├── Models/                     # Data models
│   ├── User.cs
│   ├── Role.cs
│   ├── Menu.cs
│   └── Permission.cs
├── DAL/                        # Data Access Layer
│   ├── UserDAL.cs
│   └── MenuDAL.cs
├── BLL/                        # Business Logic Layer
│   ├── AuthenticationBLL.cs
│   └── MenuBLL.cs
├── GUI/                        # User Interface
│   ├── LoginForm.cs
│   ├── MainForm.cs
│   └── ChangePasswordForm.cs
└── Utils/                      # Utilities
    ├── DatabaseHelper.cs
    └── PasswordHelper.cs
```

## Cài đặt và chạy

### Yêu cầu hệ thống
- .NET 8.0
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 hoặc VS Code

### Bước 1: Cài đặt Database
1. Mở SQL Server Management Studio
2. Chạy script `Database/CreateDatabase.sql`
3. Kiểm tra database `QLVT_DB` đã được tạo

### Bước 2: Cấu hình Connection String
Mở file `Utils/DatabaseHelper.cs` và cập nhật connection string nếu cần:
```csharp
private static readonly string ConnectionString = 
    "Server=.;Database=QLVT_DB;Integrated Security=true;TrustServerCertificate=true;";
```

### Bước 3: Build và chạy
```bash
cd QLVT
dotnet build
dotnet run
```

## Tài khoản mặc định

| Username | Password | Role | Mô tả |
|----------|----------|------|-------|
| admin | 123456 | Administrator | Toàn quyền |
| manager | 123456 | Manager | Quyền quản lý |
| user1 | 123456 | User | Quyền giới hạn |
| guest | 123456 | Guest | Chỉ xem |

## Database Schema

### Bảng Users
- UserID (PK)
- Username (Unique)
- PasswordHash (SHA256)
- FullName
- IsActive
- CreatedDate
- LastLogin

### Bảng Roles
- RoleID (PK)
- RoleName (Unique)
- Description
- IsActive

### Bảng UserRoles (Many-to-Many)
- UserID (FK)
- RoleID (FK)
- AssignedDate

### Bảng Menus
- MenuID (PK)
- MenuName
- ParentID (FK - Self reference)
- FormName
- SortOrder
- MenuIcon
- IsActive

### Bảng Permissions
- RoleID (FK)
- MenuID (FK)
- CanAccess, CanCreate, CanRead, CanUpdate, CanDelete

## Tính năng menu hiện có

### Hệ thống
- ✅ Thông tin tài khoản
- ✅ Đổi mật khẩu
- 🔄 Quản lý người dùng (Sẽ phát triển)
- 🔄 Phân quyền (Sẽ phát triển)
- ✅ Đăng xuất

### Quản lý (Sẽ phát triển trong giai đoạn tiếp theo)
- 🔄 Quản lý vật tư
- 🔄 Quản lý kho
- 🔄 Nhập kho
- 🔄 Xuất kho

### Báo cáo (Sẽ phát triển trong giai đoạn tiếp theo)
- 🔄 Báo cáo tồn kho
- 🔄 Báo cáo xuất nhập

### Tiện ích (Sẽ phát triển trong giai đoạn tiếp theo)
- 🔄 Sao lưu dữ liệu
- 🔄 Cài đặt hệ thống

## Công nghệ sử dụng
- **Framework**: .NET 8.0
- **UI**: Windows Forms
- **Database**: SQL Server
- **ORM**: ADO.NET (Microsoft.Data.SqlClient)
- **Security**: SHA256 Hash cho password
- **Architecture**: 3-Layer (DAL, BLL, GUI)

## Ghi chú
- Dự án hiện tại chỉ tập trung vào hệ thống đăng nhập và phân quyền menu
- Các chức năng quản lý vật tư sẽ được phát triển trong giai đoạn tiếp theo
- Code được viết rõ ràng, có comment tiếng Việt để dễ hiểu
- Tuân thủ nguyên tắc phân lớp và SOLID principles

## Tác giả
Phát triển bởi GitHub Copilot
Phiên bản: 1.0
Ngày: 2025
