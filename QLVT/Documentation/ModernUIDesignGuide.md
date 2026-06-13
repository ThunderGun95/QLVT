# Modern UI Design Guide - QLVT Application

## 📋 Tổng quan
Hướng dẫn thiết kế giao diện hiện đại, responsive và dễ sử dụng cho ứng dụng QLVT WinForms.

**Ngày cập nhật:** 08/11/2025  
**Form mẫu:** `HoanUngBGKUserControl`  
**Framework:** .NET 8.0 WinForms

---

## 🎨 Color Palette (Material Design Inspired)

### Primary Colors
```csharp
// Primary Blue - Dành cho tiêu đề, highlights
Color.FromArgb(41, 128, 185)   // #2980B9 - Base
Color.FromArgb(52, 152, 219)   // #3498DB - Lighter variant

// Success Green - Dành cho actions tích cực
Color.FromArgb(46, 204, 113)   // #2ECC71 - Base
Color.FromArgb(39, 174, 96)    // #27AE60 - Darker variant

// Warning Orange - Dành cho actions quan trọng
Color.FromArgb(230, 126, 34)   // #E67E22 - Base
Color.FromArgb(211, 84, 0)     // #D35400 - Darker variant
```

### Background & Text Colors
```csharp
// Background
Color.FromArgb(236, 240, 241)  // #ECF0F1 - Light gray background
Color.White                     // #FFFFFF - White for cards/panels

// Text
Color.FromArgb(52, 73, 94)     // #34495E - Dark gray for headers
Color.Black                     // #000000 - Standard text
Color.DarkRed                   // For error/warning states
```

### Status Colors
```csharp
// Success state
lblStatus.ForeColor = Color.FromArgb(46, 204, 113);  // Green

// Error state
lblStatus.ForeColor = Color.Red;

// Processing state
lblStatus.ForeColor = Color.Blue;

// Warning state (Pink background for grid rows)
row.DefaultCellStyle.BackColor = Color.LightPink;
row.DefaultCellStyle.ForeColor = Color.DarkRed;
```

---

## 📝 Typography

### Font Family
```csharp
// Primary font: Segoe UI (modern, readable, Windows native)
Font primaryFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
```

### Font Sizes & Weights
```csharp
// Title bar (Large, Bold)
new Font("Segoe UI", 16F, FontStyle.Bold)

// Section headers (Medium, Bold)
new Font("Segoe UI", 10F, FontStyle.Bold)

// Standard labels (Regular)
new Font("Segoe UI", 9.5F, FontStyle.Regular)

// DataGridView data (Regular - không in đậm)
new Font("Segoe UI", 9.5F, FontStyle.Regular)

// Small text/status (Small, Regular)
new Font("Segoe UI", 8.5F, FontStyle.Regular)
```

---

## 🔲 Component Styles

### 1. Title Bar (Label)
```csharp
lblTitle.Text = "HỆ THỐNG HOÀN ỨNG BÀO GỒM KIỆN";
lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
lblTitle.ForeColor = Color.White;
lblTitle.BackColor = Color.FromArgb(41, 128, 185);  // Primary Blue
lblTitle.TextAlign = ContentAlignment.MiddleCenter;
lblTitle.Dock = DockStyle.Top;
lblTitle.Height = 50;
```

### 2. GroupBox
```csharp
// Modern flat style
grpTimKiem.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
grpTimKiem.ForeColor = Color.FromArgb(52, 73, 94);  // Dark gray
grpTimKiem.BackColor = Color.White;
grpTimKiem.FlatStyle = FlatStyle.Flat;

// Responsive anchoring
grpTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
grpBGKInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
grpChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
```

### 3. Buttons (Flat Design with Hover Effects)

#### Primary Button (Blue - Search/Query actions)
```csharp
btnTimBGK.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
btnTimBGK.ForeColor = Color.White;
btnTimBGK.BackColor = Color.FromArgb(52, 152, 219);  // Light Blue
btnTimBGK.FlatStyle = FlatStyle.Flat;
btnTimBGK.FlatAppearance.BorderSize = 0;
btnTimBGK.Size = new Size(120, 35);
btnTimBGK.Cursor = Cursors.Hand;

// Hover effect
btnTimBGK.MouseEnter += (s, e) => btnTimBGK.BackColor = Color.FromArgb(41, 128, 185);
btnTimBGK.MouseLeave += (s, e) => btnTimBGK.BackColor = Color.FromArgb(52, 152, 219);
```

#### Success Button (Green - Refresh/Reload)
```csharp
btnRefresh.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
btnRefresh.ForeColor = Color.White;
btnRefresh.BackColor = Color.FromArgb(46, 204, 113);  // Green
btnRefresh.FlatStyle = FlatStyle.Flat;
btnRefresh.FlatAppearance.BorderSize = 0;
btnRefresh.Size = new Size(120, 35);
btnRefresh.Cursor = Cursors.Hand;

// Hover effect
btnRefresh.MouseEnter += (s, e) => btnRefresh.BackColor = Color.FromArgb(39, 174, 96);
btnRefresh.MouseLeave += (s, e) => btnRefresh.BackColor = Color.FromArgb(46, 204, 113);
```

#### Warning Button (Orange - Important actions like Confirm)
```csharp
btnXacNhan.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
btnXacNhan.ForeColor = Color.White;
btnXacNhan.BackColor = Color.FromArgb(230, 126, 34);  // Orange
btnXacNhan.FlatStyle = FlatStyle.Flat;
btnXacNhan.FlatAppearance.BorderSize = 0;
btnXacNhan.Size = new Size(200, 40);  // Larger for important actions
btnXacNhan.Cursor = Cursors.Hand;

// Hover effect
btnXacNhan.MouseEnter += (s, e) => btnXacNhan.BackColor = Color.FromArgb(211, 84, 0);
btnXacNhan.MouseLeave += (s, e) => btnXacNhan.BackColor = Color.FromArgb(230, 126, 34);
```

### 4. TextBox
```csharp
txtSoBGK.Font = new Font("Segoe UI", 9.5F);
txtSoBGK.Size = new Size(250, 27);
txtSoBGK.BorderStyle = BorderStyle.FixedSingle;
```

### 5. Labels
```csharp
// Standard label
lblSoBGK.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
lblSoBGK.ForeColor = Color.FromArgb(52, 73, 94);
lblSoBGK.AutoSize = true;

// Status label (with color coding)
lblStatus.Font = new Font("Segoe UI", 9.5F);
lblStatus.ForeColor = Color.FromArgb(46, 204, 113);  // Green for success
lblStatus.AutoSize = true;
```

### 6. DataGridView

#### Setup
```csharp
dgvChiTiet.AutoGenerateColumns = false;
dgvChiTiet.AllowUserToAddRows = false;
dgvChiTiet.AllowUserToDeleteRows = false;
dgvChiTiet.ReadOnly = false;  // Allow editing where needed
dgvChiTiet.SelectionMode = DataGridViewSelectionMode.CellSelect;
dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

// Font style - Regular (không in đậm)
dgvChiTiet.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
dgvChiTiet.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

// Responsive
dgvChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
```

#### Column Configuration with FillWeight
```csharp
// STT column (5%)
new DataGridViewTextBoxColumn
{
    Name = "STT",
    HeaderText = "STT",
    Width = 60,
    FillWeight = 5,
    ReadOnly = true
}

// Mã vật tư (15%)
new DataGridViewTextBoxColumn
{
    Name = "VatTuHangHoa",
    HeaderText = "Mã VT",
    DataPropertyName = "VatTuHangHoa",
    Width = 120,
    FillWeight = 15,
    ReadOnly = true
}

// Tên vật tư (45% - Cột quan trọng nhất)
new DataGridViewTextBoxColumn
{
    Name = "TenVatTu",
    HeaderText = "Tên vật tư",
    DataPropertyName = "TenVatTu",
    Width = 400,
    FillWeight = 45,
    ReadOnly = true
}

// Đơn vị tính (10%)
new DataGridViewTextBoxColumn
{
    Name = "DonViTinh",
    HeaderText = "ĐVT",
    DataPropertyName = "DonViTinh",
    Width = 80,
    FillWeight = 10,
    ReadOnly = true
}

// Số lượng (15% - Editable)
new DataGridViewTextBoxColumn
{
    Name = "SoLuongHoanUng",
    HeaderText = "SL hoàn ứng",
    DataPropertyName = "SoLuongHoanUngThucTe",
    Width = 120,
    FillWeight = 15,
    ReadOnly = false,  // Cho phép sửa
    DefaultCellStyle = new DataGridViewCellStyle 
    { 
        Format = "N2", 
        Alignment = DataGridViewContentAlignment.MiddleRight 
    }
}

// Tồn kho (15%)
new DataGridViewTextBoxColumn
{
    Name = "TonKho",
    HeaderText = "Tồn kho",
    DataPropertyName = "TonKho",
    Width = 120,
    FillWeight = 15,
    ReadOnly = true,
    DefaultCellStyle = new DataGridViewCellStyle 
    { 
        Format = "N2", 
        Alignment = DataGridViewContentAlignment.MiddleRight 
    }
}
```

#### Conditional Row Formatting
```csharp
private void DgvChiTiet_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
{
    if (e.RowIndex < 0 || dgvChiTiet.Rows[e.RowIndex].DataBoundItem == null)
        return;

    var vatTu = dgvChiTiet.Rows[e.RowIndex].DataBoundItem as NghiemThuGiaoKhoanCTModel;
    if (vatTu == null) return;

    var row = dgvChiTiet.Rows[e.RowIndex];
    decimal soLuongHoanUng = vatTu.SoLuongHoanUngThucTe ?? vatTu.SoLuongHoanUng;

    // Highlight rows where quantity exceeds stock
    if (soLuongHoanUng > vatTu.TonKho)
    {
        row.DefaultCellStyle.BackColor = Color.LightPink;
        row.DefaultCellStyle.ForeColor = Color.DarkRed;
    }
    else
    {
        row.DefaultCellStyle.BackColor = Color.White;
        row.DefaultCellStyle.ForeColor = Color.Black;
    }
}
```

---

## 📐 Responsive Design

### Anchor Property Patterns

#### Top Section (Fixed height, stretch width)
```csharp
// Title bar
lblTitle.Dock = DockStyle.Top;

// Search box, Info box
grpTimKiem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
grpBGKInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
```

#### Main Content Area (Fill remaining space)
```csharp
// Detail section with DataGridView
grpChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
dgvChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
```

#### Bottom Section (Stay at bottom)
```csharp
// Action buttons
btnXacNhan.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
```

### Form Size Standards
```csharp
// UserControl base size
this.Size = new Size(1200, 753);
this.BackColor = Color.FromArgb(236, 240, 241);  // Light gray
```

---

## 🎯 User Experience Enhancements

### 1. Hover Effects (Interactive Feedback)
```csharp
private void SetupButtonHoverEffects()
{
    // Primary button (Blue)
    btnTimBGK.MouseEnter += (s, e) => btnTimBGK.BackColor = Color.FromArgb(41, 128, 185);
    btnTimBGK.MouseLeave += (s, e) => btnTimBGK.BackColor = Color.FromArgb(52, 152, 219);

    // Success button (Green)
    btnRefresh.MouseEnter += (s, e) => btnRefresh.BackColor = Color.FromArgb(39, 174, 96);
    btnRefresh.MouseLeave += (s, e) => btnRefresh.BackColor = Color.FromArgb(46, 204, 113);

    // Warning button (Orange)
    btnXacNhan.MouseEnter += (s, e) => btnXacNhan.BackColor = Color.FromArgb(211, 84, 0);
    btnXacNhan.MouseLeave += (s, e) => btnXacNhan.BackColor = Color.FromArgb(230, 126, 34);
}
```

### 2. Cursor Styles
```csharp
btnTimBGK.Cursor = Cursors.Hand;      // All buttons
txtSoBGK.Cursor = Cursors.IBeam;       // Text inputs
dgvChiTiet.Cursor = Cursors.Default;   // DataGridView
```

### 3. Status Messages with Icons
```csharp
// Success
lblStatus.Text = "✅ Hoàn ứng thành công!";
lblStatus.ForeColor = Color.FromArgb(46, 204, 113);

// Error
lblStatus.Text = "❌ Lỗi kết nối ERP";
lblStatus.ForeColor = Color.Red;

// Processing
lblStatus.Text = "🔄 Đang xử lý...";
lblStatus.ForeColor = Color.Blue;

// Ready
lblStatus.Text = "Sẵn sàng";
lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
```

### 4. Clear Form After Success
```csharp
private void ClearForm()
{
    txtSoBGK.Clear();
    dgvChiTiet.DataSource = null;
    currentBGK = null;
    currentVatTuList = null;
    ResetBGKDisplay();
    txtSoBGK.Focus();
}
```

---

## 🔧 Code Implementation Checklist

### Designer.cs (UI Definition)
- [ ] Title bar với Dock.Top
- [ ] GroupBox với FlatStyle.Flat
- [ ] Buttons với FlatStyle.Flat, BorderSize = 0
- [ ] Anchor properties cho responsive design
- [ ] Font: Segoe UI cho tất cả controls
- [ ] Color scheme theo Material Design
- [ ] Cursor = Cursors.Hand cho buttons

### .cs (Code-behind)
- [ ] SetupButtonHoverEffects() method
- [ ] DataGridView setup với AutoSizeColumnsMode.Fill
- [ ] DataGridView font: Regular (không in đậm)
- [ ] CellFormatting event cho conditional formatting
- [ ] Status message updates với icons
- [ ] ClearForm() method
- [ ] Validation logic

---

## 📊 Layout Structure Example

```
┌─────────────────────────────────────────────────────┐
│  TITLE BAR (Dock.Top, 50px, Blue)                  │
├─────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────┐ │
│  │ SEARCH GROUP (Anchor: Top|L|R, 90px)          │ │
│  │ [Label] [TextBox]  [Button] [Button]          │ │
│  └───────────────────────────────────────────────┘ │
│  ┌───────────────────────────────────────────────┐ │
│  │ INFO GROUP (Anchor: Top|L|R, 120px)           │ │
│  │ BGK Info Display Fields                       │ │
│  └───────────────────────────────────────────────┘ │
│  ┌───────────────────────────────────────────────┐ │
│  │ DETAIL GROUP (Anchor: Top|Bottom|L|R)         │ │
│  │ ┌───────────────────────────────────────────┐ │ │
│  │ │ DataGridView (Fill, AutoFill Columns)     │ │ │
│  │ │                                            │ │ │
│  │ │ (Expands vertically and horizontally)     │ │ │
│  │ └───────────────────────────────────────────┘ │ │
│  │                                                │ │
│  │ [Confirm Button] (Anchor: Bottom|Left, 200x40)│ │
│  └───────────────────────────────────────────────┘ │
│  [Status Label] (Anchor: Bottom|Left)              │
└─────────────────────────────────────────────────────┘
```

---

## 🎨 Quick Reference: Button Color Matrix

| Button Type | Base Color | Hover Color | Use Case |
|-------------|-----------|-------------|----------|
| **Primary (Blue)** | #3498DB (52,152,219) | #2980B9 (41,128,185) | Search, Query, Load |
| **Success (Green)** | #2ECC71 (46,204,113) | #27AE60 (39,174,96) | Refresh, Reload, Save |
| **Warning (Orange)** | #E67E22 (230,126,34) | #D35400 (211,84,0) | Confirm, Submit, Execute |
| **Danger (Red)** | #E74C3C (231,76,60) | #C0392B (192,57,43) | Delete, Cancel, Remove |

---

## 📝 Notes & Best Practices

### DO's ✅
- **ALWAYS** use Segoe UI font family
- **ALWAYS** set `FontStyle.Regular` for DataGridView data (không in đậm)
- **ALWAYS** add hover effects to buttons
- **ALWAYS** use Anchor properties for responsive design
- **ALWAYS** use AutoSizeColumnsMode.Fill for DataGridView
- **ALWAYS** add status messages with appropriate icons
- **ALWAYS** set FlatStyle.Flat for modern look
- **ALWAYS** use meaningful FillWeight for important columns

### DON'Ts ❌
- **DON'T** use bold font in DataGridView data rows
- **DON'T** use 3D border styles (use Flat instead)
- **DON'T** leave buttons without hover effects
- **DON'T** forget Anchor properties (leads to fixed layouts)
- **DON'T** use too many colors (stick to palette)
- **DON'T** make buttons too small (minimum 35px height)
- **DON'T** forget to set Cursor = Cursors.Hand for buttons

### Performance Tips ⚡
- Use `AutoSizeColumnsMode.Fill` instead of `AllColumns` for better performance
- Set `AllowUserToAddRows = false` to prevent extra blank rows
- Use `DataBindingComplete` event for STT updates instead of manual loops
- Implement conditional formatting in `CellFormatting` event

---

## 🔄 Migration Guide for Existing Forms

### Step 1: Update Designer.cs
1. Title bar: Add/update with Dock.Top
2. GroupBoxes: Set FlatStyle.Flat, update colors
3. Buttons: Set FlatStyle.Flat, BorderSize = 0, update colors
4. All fonts: Change to Segoe UI
5. Add Anchor properties to all major controls
6. Update background colors

### Step 2: Update Code-behind (.cs)
1. Add `SetupButtonHoverEffects()` method
2. Update `SetupDataGridView()` to include:
   - Font style (Regular for data)
   - AutoSizeColumnsMode.Fill
   - FillWeight for columns
3. Add/update `CellFormatting` event if needed
4. Update status messages with icons

### Step 3: Test
1. Build project
2. Test responsive behavior (resize form)
3. Test button hover effects
4. Verify DataGridView column widths
5. Check font rendering (should be Regular, not bold)

---

## 📚 Reference Files

- **Example Implementation:** `GUI/HoanUngBGKUserControl.cs`
- **Designer File:** `GUI/HoanUngBGKUserControl.Designer.cs`
- **Color Palette:** Material Design Colors
- **Font:** Segoe UI (Windows native)

---

## 🎓 Training Resources

### WinForms Responsive Design
- Anchor vs Dock properties
- AutoSizeColumnsMode options
- TableLayoutPanel for complex layouts

### Material Design Principles
- Flat design aesthetics
- Color psychology
- User feedback (hover effects)
- Consistent spacing

---

**Last Updated:** November 8, 2025  
**Version:** 1.0  
**Author:** GitHub Copilot & Development Team
