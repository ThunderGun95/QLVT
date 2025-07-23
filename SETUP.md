# Hướng dẫn Setup QLVT - Hệ thống Quản lý Vật tư

## 🎯 Mục tiêu
Hướng dẫn này sẽ giúp bạn thiết lập và chạy ứng dụng QLVT trên máy tính của bạn.

## 📋 Yêu cầu hệ thống

### Phần mềm cần thiết:
1. **.NET 8 SDK** - [Download tại đây](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **SQL Server** (một trong các lựa chọn sau):
   - SQL Server Express (miễn phí) - [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
   - SQL Server LocalDB (đi kèm với Visual Studio)
   - SQL Server Developer Edition (miễn phí)
3. **SQL Server Management Studio (SSMS)** - [Download](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)

### IDE (tùy chọn):
- Visual Studio 2022 Community (miễn phí)
- Visual Studio Code với C# Extension
- JetBrains Rider

## 🛠️ Hướng dẫn cài đặt từng bước

### Bước 1: Kiểm tra .NET SDK
Mở Command Prompt hoặc PowerShell và chạy:
```bash
dotnet --version
```
Kết quả phải là 8.x.x trở lên.

### Bước 2: Thiết lập SQL Server

#### Với SQL Server Express:
1. Download và cài đặt SQL Server Express
2. Trong quá trình cài đặt, chọn "Mixed Mode Authentication"
3. Đặt password cho tài khoản SA (ví dụ: `123456`)
4. Ghi nhớ Server Instance Name (thường là `.\SQLEXPRESS`)

#### Với LocalDB:
1. LocalDB thường đã được cài sẵn với Visual Studio
2. Server name sẽ là `(localdb)\MSSQLLocalDB`

### Bước 3: Tạo Database

#### Option 1: Sử dụng SSMS (Khuyến nghị)
1. Mở SQL Server Management Studio
2. Kết nối đến SQL Server instance:
   - Server name: `.\SQLEXPRESS` hoặc `(localdb)\MSSQLLocalDB`
   - Authentication: Windows Authentication hoặc SQL Server Authentication
3. Mở file `Database/CreateDatabase.sql` trong SSMS
4. Nhấn F5 để chạy script
5. Kiểm tra database `QLVT_DB` đã được tạo thành công

#### Option 2: Sử dụng Command Line
```bash
# Với SQL Server Express
sqlcmd -S .\SQLEXPRESS -i "Database\CreateDatabase.sql"

# Với LocalDB
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "Database\CreateDatabase.sql"
```

### Bước 4: Cấu hình Connection String

Mở file `Utils/DatabaseHelper.cs` và cập nhật connection string phù hợp:

#### Với SQL Server Express (Windows Authentication):
```csharp
private static readonly string ConnectionString = 
    "Server=.\\SQLEXPRESS;Database=QLVT_DB;Integrated Security=true;TrustServerCertificate=true;";
```

#### Với SQL Server Express (SQL Authentication):
```csharp
private static readonly string ConnectionString = 
    "Server=.\\SQLEXPRESS;Database=QLVT_DB;User Id=sa;Password=123456;TrustServerCertificate=true;";
```

#### Với LocalDB:
```csharp
private static readonly string ConnectionString = 
    "Server=(localdb)\\MSSQLLocalDB;Database=QLVT_DB;Integrated Security=true;TrustServerCertificate=true;";
```

### Bước 5: Build và chạy ứng dụng

#### Với Command Line:
```bash
cd "đường_dẫn_đến_thư_mục_QLVT\QLVT"
dotnet build
dotnet run
```

#### Với Visual Studio:
1. Mở file `QLVT.sln` hoặc `QLVT.csproj`
2. Nhấn F5 hoặc Ctrl+F5 để chạy

#### Với Visual Studio Code:
1. Mở thư mục chứa project
2. Nhấn F5 để debug hoặc Ctrl+F5 để chạy

## 🔑 Thông tin đăng nhập mặc định

Sau khi ứng dụng khởi động thành công, bạn có thể đăng nhập bằng các tài khoản sau:

| Tài khoản | Mật khẩu | Quyền hạn | Mô tả |
|-----------|----------|-----------|--------|
| **admin** | 123456 | Administrator | Toàn quyền - có thể truy cập tất cả menu |
| **manager** | 123456 | Manager | Quyền quản lý - truy cập hầu hết menu |
| **user1** | 123456 | User | Quyền hạn chế - chỉ xem một số menu |
| **guest** | 123456 | Guest | Quyền tối thiểu - chỉ xem thông tin cá nhân |

## 🚨 Xử lý sự cố

### Lỗi: "Không thể kết nối đến database"
**Nguyên nhân**: Connection string không đúng hoặc SQL Server chưa chạy.

**Giải pháp**:
1. Kiểm tra SQL Server đang chạy:
   - Mở Services (services.msc)
   - Tìm "SQL Server (SQLEXPRESS)" hoặc "SQL Server (MSSQLSERVER)"
   - Đảm bảo service đang chạy

2. Kiểm tra connection string trong `DatabaseHelper.cs`

3. Test kết nối bằng SSMS trước

### Lỗi: "Database 'QLVT_DB' không tồn tại"
**Giải pháp**: Chạy lại script `CreateDatabase.sql`

### Lỗi: Build failed
**Giải pháp**:
1. Đảm bảo đã cài .NET 8 SDK
2. Restore packages: `dotnet restore`
3. Clean và rebuild: `dotnet clean` rồi `dotnet build`

### Lỗi: "TrustServerCertificate" 
**Giải pháp**: Thêm `TrustServerCertificate=true;` vào connection string

## 📊 Kiểm tra cài đặt thành công

1. **Database**: Mở SSMS, kết nối và kiểm tra:
   - Database `QLVT_DB` tồn tại
   - Có 5 bảng: Users, Roles, UserRoles, Menus, Permissions
   - Bảng Users có 4 user mặc định

2. **Application**: 
   - Ứng dụng khởi động không báo lỗi
   - Form đăng nhập hiển thị đúng
   - Đăng nhập thành công với tài khoản `admin/123456`
   - MainForm hiển thị menu động

3. **Chức năng cơ bản**:
   - Menu hiển thị khác nhau tùy theo quyền user
   - Đổi mật khẩu hoạt động
   - Đăng xuất hoạt động bình thường

## 🔧 Tùy chỉnh nâng cao

### Thay đổi Connection String động
Bạn có thể tạo file `appsettings.json` để lưu connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=QLVT_DB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

### Thêm logging
Cài đặt package NLog hoặc Serilog để ghi log:
```bash
dotnet add package NLog
```

### Deploy ứng dụng
Để tạo file executable:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## 📞 Hỗ trợ

Nếu gặp vấn đề, hãy kiểm tra:
1. Tất cả services SQL Server đang chạy
2. Connection string đúng với setup của bạn
3. .NET SDK version đúng
4. Database script đã chạy thành công

**Lưu ý**: Đây là phiên bản demo, password được lưu dạng hash SHA256 đơn giản. Trong production, nên sử dụng các phương pháp bảo mật mạnh hơn như bcrypt hoặc Argon2.
