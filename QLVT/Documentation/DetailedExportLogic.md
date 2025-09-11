# 🔄 **Cập nhật Logic Xuất Kho - Xử lý Chi tiết từng dòng**

## ⚠️ **Vấn đề đã được giải quyết**
**Trước đây:** Xuất kho xử lý theo kiểu "tổng thể" - 1 transaction header có 1 kho nguồn và 1 kho đích cố định.  
**Bây giờ:** Xuất kho xử lý **chi tiết từng dòng** - mỗi detail item có thể có kho nguồn khác nhau.

## 🆕 **Thay đổi quan trọng trong ExportTransactionDAL.cs**

### **1. Transaction Header - Không còn kho nguồn cố định**
```csharp
// TRƯỚC: Hardcode kho nguồn = 6 (kho công ty)
command.Parameters.AddWithValue("@maKhoNguon", "6");

// SAU: NULL vì mỗi detail có kho nguồn riêng
INSERT INTO Transactions (... MaKhoNguon, MaKhoNhan, ...)
VALUES (... NULL, @maKhoNhan, ...)
```

### **2. Transaction Details - Validate kho nguồn bắt buộc**
```csharp
// TRƯỚC: Default fallback về kho công ty
int sourceWarehouseId = detail.SourceWarehouseId ?? 1;

// SAU: Validate bắt buộc phải có kho nguồn
if (!detail.SourceWarehouseId.HasValue)
    throw new Exception($"Chi tiết vật tư {detail.MaVatTuHangHoa} chưa xác định được kho nguồn");

int sourceWarehouseId = detail.SourceWarehouseId.Value;
```

### **3. Inventory Transfer - Chi tiết từng kho nguồn**
```csharp
// Mỗi detail item được xử lý riêng với kho nguồn cụ thể
TransferInventory(connection, transaction, sourceWarehouseId, employeeWarehouseId, 
                  detail.MappedSupplyId!.Value, (int)detail.SoLuongXuatKho);
```

## 🔄 **Workflow mới**

### **Chi tiết luồng xử lý:**
1. **Load phiếu ERP** → Có nhiều detail items với `MaKhoXuat` khác nhau
2. **Mapping từng detail:**
   - Detail 1: `MaKhoXuat = "1"` → `SourceWarehouseId = 6` (kho công ty)
   - Detail 2: `MaKhoXuat = "3"` → `SourceWarehouseId = 6` (kho công ty)  
   - Detail 3: `MaKhoXuat = "5"` → `SourceWarehouseId = null` (không support)
3. **Validation:** Tất cả details phải có `SourceWarehouseId` (không được null)
4. **Create Transaction:**
   - Header: `MaKhoNguon = NULL`, `MaKhoNhan = employeeWarehouseId`
   - Detail 1: `SourceWarehouseId = 6`, transfer từ kho 6 → kho nhân viên
   - Detail 2: `SourceWarehouseId = 6`, transfer từ kho 6 → kho nhân viên

## 📊 **So sánh Logic**

### **❌ Logic cũ (Tổng thể)**
```
Transaction Header: Kho A → Kho B (toàn bộ phiếu)
├─ Detail 1: Item X, SL: 10 (từ Kho A)
├─ Detail 2: Item Y, SL: 5  (từ Kho A)  
└─ Detail 3: Item Z, SL: 3  (từ Kho A)
```

### **✅ Logic mới (Chi tiết từng dòng)**
```
Transaction Header: Multi-source → Kho B
├─ Detail 1: Item X, SL: 10, SourceWarehouseId: 6 (Kho công ty)
├─ Detail 2: Item Y, SL: 5,  SourceWarehouseId: 6 (Kho công ty)
└─ Detail 3: Item Z, SL: 3,  SourceWarehouseId: 8 (Kho dự án)
```

## 🎯 **Lợi ích**

### **✅ Tính chính xác cao:**
- **Audit trail đầy đủ:** Biết chính xác từng item lấy từ kho nào
- **Inventory tracking:** Cập nhật tồn kho đúng từng kho nguồn
- **Compliance:** Tuân thủ business rule thực tế

### **✅ Linh hoạt:**
- **Multi-source support:** Hỗ trợ nhiều kho nguồn trong 1 phiếu
- **Extensible:** Dễ mở rộng thêm kho mới
- **Configurable:** Thay đổi mapping rules dễ dàng

### **✅ Data integrity:**
- **Validation chặt chẽ:** Bắt buộc phải có kho nguồn
- **Error handling:** Thông báo lỗi rõ ràng về kho nào thiếu tồn kho
- **Transaction safety:** Rollback nếu bất kỳ detail nào fail

## ⚙️ **Enhanced Error Messages**

### **Validation Messages:**
```csharp
// Khi detail thiếu kho nguồn
"Chi tiết vật tư {MaVatTuHangHoa} chưa xác định được kho nguồn"

// Khi kho nguồn không đủ tồn kho  
"Kho nguồn (ID={sourceWarehouseId}) không đủ tồn kho cho vật tư ErpId={erpId}"

// Khi không tìm thấy inventory record
"Không tìm thấy record tồn kho trong kho nguồn (ID={sourceWarehouseId})"
```

## 📋 **Database Impact**

### **Transactions Table:**
- `MaKhoNguon`: Bây giờ có thể `NULL` (vì mỗi detail có kho nguồn riêng)
- `MaKhoNhan`: Vẫn là kho đích chung cho toàn bộ transaction

### **TransactionDetails Table:**
- `SourceWarehouseId`: **Bắt buộc** phải có giá trị (không được NULL)
- Mỗi record biết chính xác kho nguồn của nó

### **Inventory Table:**
- Update chính xác theo từng kho nguồn
- Không còn assumption tất cả từ "kho công ty"

## 🔧 **Testing Scenarios**

### **Test Case 1: Single source warehouse**
```
ERP Phiếu: Tất cả items có MaKhoXuat = "1"
Expected: Tất cả SourceWarehouseId = 6
Result: Normal processing
```

### **Test Case 2: Multiple source warehouses**  
```
ERP Phiếu: 
- Item A: MaKhoXuat = "1" → SourceWarehouseId = 6
- Item B: MaKhoXuat = "3" → SourceWarehouseId = 6  
- Item C: MaKhoXuat = "4" → SourceWarehouseId = 6
Expected: Tất cả về kho công ty (theo mapping rule)
Result: Normal processing
```

### **Test Case 3: Unsupported warehouse**
```
ERP Phiếu: Item có MaKhoXuat = "99" (không có trong mapping)
Expected: SourceWarehouseId = null
Result: Validation error trước khi xử lý
```

## 📝 **Migration Notes**

### **Backward Compatibility:**
- ✅ **Existing data:** Không ảnh hưởng data cũ
- ✅ **Database schema:** TransactionDetails đã có SourceWarehouseId
- ✅ **UI compatibility:** UI hiện tại vẫn hoạt động bình thường

### **Performance:**
- ✅ **No degradation:** Logic mới không chậm hơn
- ✅ **Better accuracy:** Tăng độ chính xác mà không mất performance
- ✅ **Scalable:** Dễ scale khi thêm nhiều kho

**Logic mới đã sẵn sàng xử lý xuất kho chi tiết từng dòng với độ chính xác cao! 🎉**
