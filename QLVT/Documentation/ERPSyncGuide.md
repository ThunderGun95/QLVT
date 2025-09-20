# 🔄 ERP Data Synchronization - Hướng dẫn sử dụng

## 📋 Tổng quan
Hệ thống đồng bộ dữ liệu ERP cho phép lấy dữ liệu từ ERP database (máy chủ khác) và đưa vào QLVT database (máy chủ local).

## 🗃️ Cấu trúc dữ liệu đồng bộ

### 6 Bảng được đồng bộ:
1. **ct.DonDangKy** - Đơn đăng ký
2. **ct.DonDangKyCT** - Chi tiết đơn đăng ký  
3. **ct.SuaChua** - Sửa chữa
4. **ct.SuaChuaCT** - Chi tiết sửa chữa
5. **ct.NghiemThuGiaoKhoan** - Nghiệm thu giao khoán
6. **ct.NghiemThuGiaoKhoanCT** - Chi tiết nghiệm thu giao khoán

## ⚙️ Cấu hình ban đầu

### 1. Cập nhật Connection String
Mở file `App.config` và cập nhật thông tin kết nối ERP:

```xml
<connectionStrings>
    <!-- Thay đổi thông tin này theo ERP Server thực tế -->
    <add name="ERPConnection" 
         connectionString="Server=TEN_SERVER_ERP;Database=TEN_DATABASE_ERP;User Id=USERNAME;Password=PASSWORD;TrustServerCertificate=true;Connection Timeout=60;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 2. Tạo Database Schema
Chạy file `CreateDatabase.sql` để tạo 6 bảng ct.* trong QLVT_DB.

## 🚀 Cách sử dụng

### Giao diện đồng bộ
- Truy cập menu **Tiện ích** → **Đồng bộ ERP** 
- Hoặc thêm vào MainForm để hiển thị ERPSyncUserControl

### Các bước thực hiện:

#### 1. **Kiểm tra kết nối**
- Nhấn nút **"Test Kết nối"** 
- Đảm bảo hiển thị ✅ "Kết nối thành công"

#### 2. **Cấu hình tham số**
- **Ngày lọc:** Ngày bắt đầu lấy dữ liệu (mặc định: 2025-01-01)
- **Mã ĐĐK:** Mã đơn đăng ký mẫu (mặc định: KH2508.0162)
- **Mã đơn SC:** Mã đơn sửa chữa mẫu (mặc định: SC2509.0059)
- **Số nghiệm thu:** Số nghiệm thu mẫu (mặc định: 1468)
- **Năm:** Năm nghiệm thu (mặc định: 2025)
- **Giao khoán ID:** ID giao khoán mẫu (mặc định: 10438)

#### 3. **Đồng bộ dữ liệu**

**Đồng bộ từng bảng:**
- 📋 **Đơn đăng ký:** Lấy tất cả đơn đăng ký theo ngày lọc
- 📋 **Chi tiết ĐĐK:** Lấy chi tiết cho 1 mã ĐĐK cụ thể
- 🔧 **Sửa chữa:** Lấy tất cả đơn sửa chữa theo ngày lọc
- 🔧 **Chi tiết SC:** Lấy chi tiết cho 1 mã đơn SC cụ thể
- ✅ **Nghiệm thu GK:** Lấy theo số nghiệm thu + năm
- ✅ **Chi tiết NTGK:** Lấy chi tiết theo Giao khoán ID

**Đồng bộ tất cả:**
- Nhấn **"🔄 ĐỒNG BỘ TẤT CẢ"** để chạy 6 bảng cùng lúc

#### 4. **Theo dõi kết quả**
- **Log đồng bộ:** Hiển thị real-time progress
- **Thống kê dữ liệu:** Xem số bản ghi trong từng bảng

## 🔧 Sử dụng trong code

### ERPSyncBLL - Business Logic Layer
```csharp
var syncBLL = new ERPSyncBLL();

// Test kết nối
var isConnected = await syncBLL.TestERPConnectionAsync();

// Đồng bộ đơn đăng ký
var result = await syncBLL.SyncDonDangKyAsync(DateTime.Now.AddDays(-30));

// Đồng bộ chi tiết theo mã
var detailResult = await syncBLL.SyncDonDangKyCTAsync("KH2508.0162");

// Đồng bộ tất cả
var allResults = await syncBLL.SyncAllDataAsync(
    filterDate: DateTime.Now.AddDays(-30),
    sampleMADDK: "KH2508.0162",
    sampleMADON: "SC2509.0059",
    sampleSoNghiemThu: 1468,
    sampleNamNghiemThu: 2025,
    sampleGiaoKhoanID: 10438
);

// Xem thống kê
var stats = await syncBLL.GetERPDataStatsAsync();
```

### ERPConnectionDAL - Data Access Layer
```csharp
var erpDAL = new ERPConnectionDAL("connection_string");

// Lấy dữ liệu từ ERP
var donDangKy = await erpDAL.GetDonDangKyDataAsync(DateTime.Now.AddDays(-30));
var chiTiet = await erpDAL.GetDonDangKyCTDataAsync("KH2508.0162");
```

## 📊 Models

### ERPSyncResult
```csharp
public class ERPSyncResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int RecordsAffected { get; set; }
    public Exception? Exception { get; set; }
    public DateTime SyncTime { get; set; }
}
```

### Các Model dữ liệu:
- `DonDangKyModel` - Đơn đăng ký
- `DonDangKyCTModel` - Chi tiết đơn đăng ký
- `SuaChuaModel` - Sửa chữa
- `SuaChuaCTModel` - Chi tiết sửa chữa
- `NghiemThuGiaoKhoanModel` - Nghiệm thu giao khoán
- `NghiemThuGiaoKhoanCTModel` - Chi tiết nghiệm thu giao khoán

## ⚠️ Lưu ý quan trọng

### 1. **Performance**
- Đồng bộ có thể mất vài phút tùy thuộc vào lượng dữ liệu
- Nên đồng bộ vào giờ thấp điểm để tránh ảnh hưởng hiệu suất

### 2. **Duplicate Prevention**
- Hệ thống tự động kiểm tra duplicate bằng `IF NOT EXISTS`
- Chỉ insert bản ghi mới, không update bản ghi cũ

### 3. **Error Handling**
- Mỗi bảng đồng bộ độc lập, lỗi 1 bảng không ảnh hưởng bảng khác
- Check log chi tiết khi có lỗi

### 4. **Foreign Key**
- Bảng `*CT` có FK với `Supplies(ErpId)`
- Đảm bảo bảng Supplies đã có dữ liệu ErpId trước khi đồng bộ

### 5. **Connection Security**
- Sử dụng SQL Server Authentication hoặc Windows Authentication
- Đảm bảo account có quyền READ trên ERP database

## 🔍 Troubleshooting

### Lỗi kết nối ERP:
1. Kiểm tra Server name, Database name
2. Kiểm tra Username/Password
3. Kiểm tra firewall, network connectivity
4. Kiểm tra SQL Server đã enable TCP/IP

### Lỗi Foreign Key:
1. Đảm bảo bảng Supplies đã có ErpId tương ứng
2. Sync bảng Supplies trước khi sync các bảng CT

### Lỗi Timeout:
1. Tăng Connection Timeout trong connection string
2. Tăng Command Timeout
3. Chia nhỏ batch size nếu dữ liệu lớn

## 📞 Hỗ trợ
- Xem log chi tiết trong giao diện đồng bộ
- Check thống kê dữ liệu sau mỗi lần đồng bộ
- Liên hệ admin nếu cần hỗ trợ cấu hình ERP connection