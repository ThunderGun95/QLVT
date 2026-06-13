# WarehouseComboBox - Hướng dẫn sử dụng (Version 2.0)

## Tổng quan

`WarehouseComboBox` là một UserControl nâng cao để chọn kho với các tính năng:
- **Lazy Loading**: Chỉ tải dữ liệu khi người dùng focus vào control
- **Search với Debounce**: Tìm kiếm tự động sau 0.8 giây không gõ phím
- **Highlight Tìm kiếm**: Tự động highlight (màu vàng) từ khóa trong kết quả
- **Multi-Select**: Hỗ trợ chọn nhiều kho cùng lúc
- **Keyboard Support**: Hỗ trợ phím tắt Enter, Escape, Down Arrow
- **Owner Draw ListBox**: Hiển thị format custom với highlight

## Giao diện mới

### Display Format
```
┌──────────────────────────────────────────────┐
│  16  Kho Xi nghiệp XL CTT nước (Phùng Minh)  │
│  52  Kho XNXL CTT Lê Việt Hùng 4 (Phùng Minh)│
└──────────────────────────────────────────────┘
```

Format: `ID  TenKho  MaKho`

### Highlight Search
Khi gõ "phùng minh":
```
┌──────────────────────────────────────────────┐
│  16  Kho... (🟨Phùng Minh🟨 Nam)  🟨Phùng Minh🟨│
│  52  Kho... (🟨Phùng Minh🟨 Nam)  🟨Phùng Minh🟨│
└──────────────────────────────────────────────┘
```
(🟨 = highlight màu vàng)

## Các chế độ sử dụng

### 1. Single Select (mặc định)
```csharp
// Tạo control chọn 1 kho
var warehouseComboBox = new WarehouseComboBox(300, 25, false);

// Hoặc với AllowEmptySelection
var warehouseComboBox = new WarehouseComboBox(300, 25, true);

// Lấy kho đã chọn
int selectedId = warehouseComboBox.SelectedWarehouseId;
Warehouse selectedWarehouse = warehouseComboBox.SelectedWarehouse;
string selectedName = warehouseComboBox.SelectedWarehouseName;
```

### 2. Multi-Select Mode
```csharp
// Tạo control cho phép chọn nhiều kho
var warehouseComboBox = new WarehouseComboBox(300, 25, false, true);

// hoặc set property
warehouseComboBox.MultiSelect = true;

// Lấy danh sách kho đã chọn
List<int> selectedIds = warehouseComboBox.SelectedWarehouseIds;
List<Warehouse> selectedWarehouses = warehouseComboBox.SelectedWarehouses;

// Ví dụ: Đếm số kho đã chọn
int count = warehouseComboBox.SelectedWarehouseIds.Count;
```

## Properties

### Layout
- `ComboBoxWidth`: Chiều rộng (mặc định: 300px)
- `ComboBoxHeight`: Chiều cao (mặc định: 25px)
- `PlaceholderText`: Text hiển thị khi trống (mặc định: "Gõ để tìm kho...")

### Behavior
- `AllowEmptySelection`: Cho phép không chọn gì (mặc định: false)
- `MultiSelect`: Cho phép chọn nhiều (mặc định: false)

### Data
- `SelectedWarehouseId`: ID kho đang chọn (single select)
- `SelectedWarehouseIds`: List ID các kho đang chọn (multi select)
- `SelectedWarehouse`: Object Warehouse đang chọn
- `SelectedWarehouses`: List Warehouse đang chọn
- `SelectedWarehouseName`: Tên kho đang chọn

## Events

### 1. WarehouseSelected (Single Select)
```csharp
warehouseComboBox.WarehouseSelected += (sender, e) =>
{
    Console.WriteLine($"Đã chọn: {e.WarehouseName} (ID: {e.WarehouseId})");
    var warehouse = e.SelectedWarehouse;
};
```

### 2. MultiWarehouseSelected (Multi Select)
```csharp
warehouseComboBox.MultiWarehouseSelected += (sender, e) =>
{
    Console.WriteLine($"Đã chọn {e.Count} kho");
    foreach (var warehouse in e.SelectedWarehouses)
    {
        Console.WriteLine($"- {warehouse.TenKho}");
    }
    
    List<int> ids = e.WarehouseIds;
};
```

### 3. WarehouseChanged (All modes)
```csharp
warehouseComboBox.WarehouseChanged += (sender, e) =>
{
    Console.WriteLine("Selection changed!");
    // Update UI, enable/disable buttons, etc.
};
```

## Methods

### SetSelectedWarehouse
```csharp
// Set by ID
warehouseComboBox.SetSelectedWarehouse(123);

// Set by Name
warehouseComboBox.SetSelectedWarehouse("Kho tổng");
```

### ClearSelection
```csharp
// Xóa tất cả selection
warehouseComboBox.ClearSelection();
```

### ValidateSelection
```csharp
// Kiểm tra có selection chưa (nếu AllowEmptySelection = false)
if (warehouseComboBox.ValidateSelection())
{
    // OK - có selection
    DoSomething();
}
else
{
    // Sẽ hiện MessageBox "Vui lòng chọn kho!"
}
```

## Ví dụ sử dụng

### Ví dụ 1: Form báo cáo với Single Select
```csharp
public class BaoCaoForm : Form
{
    private WarehouseComboBox warehouseComboBox;
    private Button btnSearch;
    
    private void InitializeComponent()
    {
        // Tạo control
        warehouseComboBox = new WarehouseComboBox(300, 25, true);
        warehouseComboBox.Location = new Point(100, 50);
        
        // Subscribe event
        warehouseComboBox.WarehouseSelected += (s, e) =>
        {
            Console.WriteLine($"Chọn kho: {e.WarehouseName}");
        };
        
        // Button search
        btnSearch = new Button { Text = "Tìm kiếm" };
        btnSearch.Click += BtnSearch_Click;
        
        this.Controls.Add(warehouseComboBox);
        this.Controls.Add(btnSearch);
    }
    
    private void BtnSearch_Click(object sender, EventArgs e)
    {
        if (!warehouseComboBox.ValidateSelection())
            return;
            
        int warehouseId = warehouseComboBox.SelectedWarehouseId;
        // Tìm kiếm với kho đã chọn
    }
}
```

### Ví dụ 2: Form xuất/nhập với Multi-Select
```csharp
public class XuatNhapMultiForm : Form
{
    private WarehouseComboBox warehouseComboBox;
    private Button btnProcess;
    
    private void InitializeComponent()
    {
        // Tạo control multi-select
        warehouseComboBox = new WarehouseComboBox(350, 25, false, true);
        warehouseComboBox.Location = new Point(100, 50);
        warehouseComboBox.PlaceholderText = "Chọn các kho cần xử lý...";
        
        // Subscribe event
        warehouseComboBox.MultiWarehouseSelected += (s, e) =>
        {
            btnProcess.Text = $"Xử lý ({e.Count} kho)";
            btnProcess.Enabled = e.Count > 0;
        };
        
        btnProcess = new Button { Text = "Xử lý" };
        btnProcess.Click += BtnProcess_Click;
        
        this.Controls.Add(warehouseComboBox);
        this.Controls.Add(btnProcess);
    }
    
    private void BtnProcess_Click(object sender, EventArgs e)
    {
        var selectedWarehouses = warehouseComboBox.SelectedWarehouses;
        foreach (var warehouse in selectedWarehouses)
        {
            ProcessWarehouse(warehouse);
        }
    }
    
    private void ProcessWarehouse(Warehouse warehouse)
    {
        // Xử lý từng kho
    }
}
```

## Lazy Loading Behavior

### Khi nào dữ liệu được tải?
1. **Không tự động tải khi khởi tạo** - Tiết kiệm resources
2. **Tải khi user focus vào control** - Khi click hoặc tab vào textbox
3. **Tải khi user nhấn dropdown button** - Khi click nút ▼
4. **Tải khi user bắt đầu gõ** - Sau 0.8 giây debounce

### Trạng thái Loading
```
┌─────────────────────────┐
│  Đang tải...            │  ← Hiển thị khi đang load
└─────────────────────────┘

Sau khi load xong:
┌─────────────────────────┐
│  Gõ để tìm kho...       │  ← Placeholder
└─────────────────────────┘
```

## Search Behavior

### Auto Search với Debounce
```
User gõ: "k" → chờ 0.8s → không gõ thêm → TÌM KIẾM
User gõ: "k" → 0.3s → "h" → chờ 0.8s → TÌM KIẾM
User gõ: "k" → 0.3s → "h" → 0.3s → "o" → chờ 0.8s → TÌM KIẾM
```

### Kết quả tìm kiếm
- **Có kết quả**: Hiển thị danh sách kho matching
- **Không có kết quả**: Hiển thị "❌ Không có kết quả"
- **Trường rỗng**: Hiển thị tất cả kho

### Search Fields
Tìm kiếm trên cả 2 fields:
- `MaKho` (Mã kho)
- `TenKho` (Tên kho)

## Keyboard Shortcuts

| Phím | Chức năng |
|------|-----------|
| `Enter` | Chọn item đầu tiên trong dropdown |
| `Escape` | Đóng dropdown |
| `↓` (Down Arrow) | Mở dropdown (nếu đang đóng) |

## Display Behavior

### Single Select Mode
```
┌─────────────────────────┐
│  Kho tổng              ▼│  ← Hiển thị tên kho
└─────────────────────────┘
```

### Multi Select Mode (1 kho)
```
┌─────────────────────────┐
│  Kho tổng              ▼│  ← Hiển thị tên kho
└─────────────────────────┘
```

### Multi Select Mode (nhiều kho)
```
┌─────────────────────────┐
│  ✓ 3 kho đã chọn       ▼│  ← Hiển thị số lượng
└─────────────────────────┘
```

## Backward Compatibility

Control mới **hoàn toàn tương thích ngược** với code cũ:
```csharp
// Code cũ vẫn hoạt động bình thường
var combo = new WarehouseComboBox(300, 25, true);
combo.WarehouseSelected += OnWarehouseSelected;
int id = combo.SelectedWarehouseId;
```

## Migration từ version cũ

### Không cần thay đổi code
Tất cả code hiện tại vẫn hoạt động bình thường. Không cần sửa gì!

### Để sử dụng tính năng mới
```csharp
// Before (old style)
var combo = new WarehouseComboBox(300, 25, true);

// After (with multi-select)
var combo = new WarehouseComboBox(300, 25, true, true);
// hoặc
combo.MultiSelect = true;
```

## Best Practices

1. **Single Select**: Dùng cho form báo cáo, filter đơn giản
2. **Multi Select**: Dùng cho batch processing, xử lý nhiều kho cùng lúc
3. **Validate**: Luôn gọi `ValidateSelection()` trước khi xử lý
4. **Event**: Dùng `WarehouseChanged` để enable/disable buttons
5. **Width**: Đặt width >= 300px cho hiển thị tốt

## Troubleshooting

### Q: Dropdown không hiện?
A: Kiểm tra `isDataLoaded` - có thể đang loading. Đợi "Đang tải..." biến mất.

### Q: Search không hoạt động?
A: Debounce 0.8s - đợi 0.8 giây sau khi gõ xong.

### Q: Multi-select không hoạt động?
A: Kiểm tra `MultiSelect = true` đã set chưa.

### Q: Event không fire?
A: Subscribe đúng event:
- Single select: `WarehouseSelected`
- Multi select: `MultiWarehouseSelected`
- Both: `WarehouseChanged`

## File Backup

File cũ được backup tại:
```
GUI/Components/WarehouseComboBox.cs.backup
```

Nếu cần rollback, chỉ cần restore file này.
