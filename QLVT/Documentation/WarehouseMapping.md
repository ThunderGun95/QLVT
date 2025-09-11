# 📋 **Hệ thống Mapping Kho ERP - Hướng dẫn sử dụng**

## 🎯 **Mục đích**
Tự động map các mã kho từ hệ thống ERP sang kho nội bộ theo quy tắc:
- **Kho ERP**: `1, 3, 4, 34` → **Kho công ty** (ID = 6)
- Hỗ trợ cả **Xuất kho** và **Nhập kho**

## 🔧 **Cấu trúc hệ thống**

### **1. WarehouseMappingBLL.cs**
```csharp
/// <summary>
/// Business Logic Layer cho mapping kho giữa ERP và hệ thống nội bộ
/// </summary>
public class WarehouseMappingBLL
{
    // Map mã kho ERP sang ID kho nội bộ
    public int? MapERPWarehouseToInternal(string erpWarehouseCode)
    
    // Lấy thông tin kho nội bộ từ mã kho ERP  
    public Warehouse? GetInternalWarehouseFromERP(string erpWarehouseCode)
    
    // Kiểm tra mã kho ERP có được hỗ trợ không
    public bool IsERPWarehouseSupported(string erpWarehouseCode)
    
    // Lấy danh sách mã kho ERP được hỗ trợ
    public List<string> GetSupportedERPWarehouseCodes()
}
```

### **2. Mapping Rules**
```csharp
// Các kho ERP này sẽ map về kho công ty (ID = 6)
var companyWarehouseCodes = new[] { "1", "3", "4", "34" };
```

## 🚀 **Workflow Xuất Kho**

### **Trước khi có Mapping**
1. Load phiếu xuất từ ERP
2. **Chọn kho thủ công** 👎
3. **Nhập mã nhân viên thủ công** 👎
4. Xử lý xuất kho

### **Sau khi có Mapping** ✅
1. Load phiếu xuất từ ERP
2. **Tự động mapping kho nguồn** theo rule
3. **Tự động lấy kho cá nhân** từ `MaNguoiNhan`
4. Hiển thị thông tin đầy đủ và xác nhận
5. Xử lý xuất kho

### **Enhanced ExportTransactionBLL**
```csharp
// Mapping kho nguồn sử dụng WarehouseMappingBLL
if (!string.IsNullOrEmpty(detail.MaKhoXuat))
{
    // Sử dụng mapping rule: kho ERP (1,3,4,34) -> kho công ty (ID=6)
    var internalWarehouse = warehouseMappingBLL.GetInternalWarehouseFromERP(detail.MaKhoXuat);
    if (internalWarehouse != null)
    {
        detail.SourceWarehouseId = internalWarehouse.Id;
    }
    else
    {
        // Fallback: tìm trực tiếp theo MaKho (để hỗ trợ các kho khác)
        var warehouse = warehouseDAL.GetWarehouses()
            .FirstOrDefault(w => w.MaKho == detail.MaKhoXuat);
        if (warehouse != null)
        {
            detail.SourceWarehouseId = warehouse.Id;
        }
    }
}
```

## 📥 **Workflow Nhập Kho**

### **Enhanced ImportBLL**
```csharp
/// <summary>
/// Lấy ID kho đích cho Import dựa trên mã kho ERP
/// Áp dụng mapping rule: kho ERP (1,3,4,34) -> kho công ty (ID=6)
/// </summary>
public int? GetTargetWarehouseIdForImport(string erpWarehouseCode)
{
    var internalWarehouse = warehouseMappingBLL.GetInternalWarehouseFromERP(erpWarehouseCode);
    return internalWarehouse?.Id;
}
```

### **Cách sử dụng trong ImportTaskUserControl**
```csharp
// Thay vì chọn kho thủ công:
var warehouseForm = new WarehouseSelectionForm(warehouses);

// Sử dụng mapping tự động:
var targetWarehouseId = importBLL.GetTargetWarehouseIdForImport(order.MaKhoVatTu);
if (targetWarehouseId.HasValue)
{
    // Tự động xử lý với kho đã mapping
    ProcessImport(order, targetWarehouseId.Value);
}
else
{
    // Fallback hoặc báo lỗi
    MessageBox.Show($"Mã kho ERP '{order.MaKhoVatTu}' chưa được hỗ trợ!");
}
```

## ⚙️ **Cấu hình Mapping Rules**

### **Để thay đổi mapping rules:**
```csharp
// Trong WarehouseMappingBLL.cs
public int? MapERPWarehouseToInternal(string erpWarehouseCode)
{
    // Cập nhật danh sách kho ERP ở đây
    var companyWarehouseCodes = new[] { "1", "3", "4", "34", "5", "7" }; // Thêm kho mới
    
    if (companyWarehouseCodes.Contains(erpWarehouseCode.Trim()))
    {
        return 6; // ID của kho công ty trong hệ thống nội bộ
    }
    
    // Thêm rule mapping khác nếu cần
    if (erpWarehouseCode == "2")
    {
        return 8; // Map sang kho khác
    }
    
    return null;
}
```

### **Để thêm mapping cho kho khác:**
```csharp
// Ví dụ: Kho ERP "100" -> Kho dự án (ID = 10)
var projectWarehouseCodes = new[] { "100", "101", "102" };
if (projectWarehouseCodes.Contains(erpWarehouseCode.Trim()))
{
    return 10; // ID của kho dự án
}
```

## 📊 **Lợi ích**

### **✅ Cho người dùng:**
- **Tự động hóa hoàn toàn**: Không cần chọn kho thủ công
- **Giảm sai sót**: Mapping theo rules cố định
- **Tăng tốc độ**: Xử lý nhanh hơn đáng kể
- **Thông tin rõ ràng**: Hiển thị đầy đủ kho nguồn và đích

### **✅ Cho hệ thống:**
- **Nhất quán dữ liệu**: Tất cả kho ERP (1,3,4,34) đều về kho công ty
- **Audit trail đầy đủ**: Lưu trữ mapping history
- **Dễ bảo trì**: Chỉ cần sửa 1 chỗ trong WarehouseMappingBLL
- **Mở rộng linh hoạt**: Dễ thêm rules mới

### **✅ Cho IT:**
- **Tập trung quản lý**: Tất cả logic mapping ở 1 file
- **Testing dễ dàng**: Có thể test từng mapping rule
- **Monitoring**: Có thể log và track mapping
- **Backward compatibility**: Vẫn hỗ trợ fallback cho kho không có rule

## 🔧 **Troubleshooting**

### **Khi thêm mã kho ERP mới:**
1. Cập nhật array trong `MapERPWarehouseToInternal()`
2. Cập nhật `GetSupportedERPWarehouseCodes()`
3. Build và test

### **Khi thay đổi ID kho công ty:**
1. Cập nhật return value trong mapping rule
2. Kiểm tra database Warehouses có kho với ID đó
3. Test với dữ liệu thực tế

### **Debugging:**
- Check `IsERPWarehouseSupported()` để xem kho có được mapping không
- Log output của `GetInternalWarehouseFromERP()` để debug mapping
- Verify database có kho với ID đúng không

## 📝 **Ghi chú quan trọng**

1. **Database constraints**: Đảm bảo kho công ty (ID=6) tồn tại trong bảng Warehouses
2. **Performance**: Mapping rules được cache, không ảnh hưởng performance
3. **Backup**: Luôn backup trước khi thay đổi mapping rules
4. **Testing**: Test với dữ liệu ERP thực tế trước khi deploy production
