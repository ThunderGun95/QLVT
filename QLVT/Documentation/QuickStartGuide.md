# Quick Start Guide - Applying Modern UI to Forms

## 📚 Overview
This guide shows you how to quickly apply the modern UI design to your WinForms using the helper classes.

---

## 🚀 Quick Start - 3 Simple Steps

### Step 1: Add Using Statements
```csharp
using QLVT.Utils;  // For UIColorPalette, UIFonts, UIStyleHelper
```

### Step 2: Style Your Controls

#### In Designer.cs (or InitializeComponent)
```csharp
// Title Bar
UIStyleHelper.ApplyTitleBarStyle(lblTitle);
lblTitle.Text = "YOUR FORM TITLE HERE";

// GroupBoxes
UIStyleHelper.ApplyGroupBoxStyle(grpSearch, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
UIStyleHelper.ApplyGroupBoxStyle(grpDetails, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

// Buttons
UIStyleHelper.ApplyPrimaryButtonStyle(btnSearch);       // Blue - for search
UIStyleHelper.ApplySuccessButtonStyle(btnRefresh);      // Green - for refresh
UIStyleHelper.ApplyWarningButtonStyle(btnConfirm, new Size(200, 40));  // Orange - for confirm
UIStyleHelper.ApplyDangerButtonStyle(btnDelete);        // Red - for delete

// TextBoxes
UIStyleHelper.ApplyTextBoxStyle(txtSearch);

// Labels
UIStyleHelper.ApplyStandardLabelStyle(lblName);
UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Ready);

// DataGridView
UIStyleHelper.ApplyDataGridViewStyle(dgvData);

// Form Background
UIStyleHelper.ApplyFormStyle(this);
```

#### In Code-behind (.cs)
```csharp
// Status messages
UIStyleHelper.SetStatusMessage(lblStatus, "Đang tải dữ liệu...", StatusType.Processing);
UIStyleHelper.SetStatusMessage(lblStatus, "Tải dữ liệu thành công!", StatusType.Success);
UIStyleHelper.SetStatusMessage(lblStatus, "Lỗi kết nối!", StatusType.Error);
```

### Step 3: Build and Run! 🎉

---

## 📋 Complete Examples

### Example 1: Simple Search Form

```csharp
public partial class SearchUserControl : UserControl
{
    public SearchUserControl()
    {
        InitializeComponent();
        SetupUI();
    }
    
    private void SetupUI()
    {
        // Form background
        UIStyleHelper.ApplyFormStyle(this);
        
        // Title
        UIStyleHelper.ApplyTitleBarStyle(lblTitle);
        lblTitle.Text = "TÌM KIẾM VẬT TƯ";
        
        // Group boxes
        UIStyleHelper.ApplyGroupBoxStyle(grpSearch, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
        UIStyleHelper.ApplyGroupBoxStyle(grpResults, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
        
        // Buttons
        UIStyleHelper.ApplyPrimaryButtonStyle(btnSearch);
        UIStyleHelper.ApplySuccessButtonStyle(btnRefresh);
        
        // TextBoxes
        UIStyleHelper.ApplyTextBoxStyle(txtKeyword);
        
        // DataGridView
        UIStyleHelper.ApplyDataGridViewStyle(dgvResults);
        SetupDataGridViewColumns();
    }
    
    private void SetupDataGridViewColumns()
    {
        dgvResults.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("MaVT", "Mã VT", "MaVatTu", 120, 15));
        dgvResults.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TenVT", "Tên vật tư", "TenVatTu", 400, 50));
        dgvResults.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("DVT", "ĐVT", "DonViTinh", 80, 10));
        dgvResults.Columns.Add(UIStyleHelper.CreateNumericColumn("SoLuong", "Số lượng", "SoLuong", 120, 15));
    }
    
    private void btnSearch_Click(object sender, EventArgs e)
    {
        UIStyleHelper.SetStatusMessage(lblStatus, "Đang tìm kiếm...", StatusType.Processing);
        
        // Your search logic here
        
        UIStyleHelper.SetStatusMessage(lblStatus, "Tìm thấy 10 kết quả", StatusType.Success);
    }
}
```

### Example 2: Data Entry Form with Validation

```csharp
public partial class DataEntryForm : Form
{
    public DataEntryForm()
    {
        InitializeComponent();
        SetupModernUI();
    }
    
    private void SetupModernUI()
    {
        // Form
        UIStyleHelper.ApplyFormStyle(this);
        
        // Title
        UIStyleHelper.ApplyTitleBarStyle(lblTitle);
        lblTitle.Text = "NHẬP LIỆU VẬT TƯ";
        
        // GroupBoxes
        UIStyleHelper.ApplyGroupBoxStyle(grpInfo);
        UIStyleHelper.ApplyGroupBoxStyle(grpDetails);
        
        // Labels
        UIStyleHelper.ApplyStandardLabelStyle(lblMaVT);
        UIStyleHelper.ApplyStandardLabelStyle(lblTenVT);
        UIStyleHelper.ApplyStandardLabelStyle(lblSoLuong);
        
        // TextBoxes
        UIStyleHelper.ApplyTextBoxStyle(txtMaVT);
        UIStyleHelper.ApplyTextBoxStyle(txtTenVT);
        UIStyleHelper.ApplyTextBoxStyle(txtSoLuong);
        
        // Buttons
        UIStyleHelper.ApplySuccessButtonStyle(btnSave, new Size(150, 40));
        UIStyleHelper.ApplyDangerButtonStyle(btnCancel, new Size(150, 40));
        
        // DataGridView
        UIStyleHelper.ApplyDataGridViewStyle(dgvItems);
    }
    
    private void btnSave_Click(object sender, EventArgs e)
    {
        if (ValidateInput())
        {
            // Save logic
            UIStyleHelper.SetStatusMessage(lblStatus, "Lưu thành công!", StatusType.Success);
        }
        else
        {
            UIStyleHelper.SetStatusMessage(lblStatus, "Vui lòng kiểm tra lại thông tin", StatusType.Error);
        }
    }
    
    private bool ValidateInput()
    {
        // Your validation logic
        return !string.IsNullOrEmpty(txtMaVT.Text);
    }
}
```

---

## 🎨 Using Colors Directly

If you need custom styling:

```csharp
// Use color palette
button.BackColor = UIColorPalette.PrimaryBlue;
label.ForeColor = UIColorPalette.TextDark;
panel.BackColor = UIColorPalette.BackgroundLight;

// Use nested button colors
btnCustom.BackColor = UIColorPalette.ButtonWarning.Base;
btnCustom.MouseEnter += (s, e) => btnCustom.BackColor = UIColorPalette.ButtonWarning.Hover;
```

---

## 📝 Using Fonts Directly

```csharp
// Use predefined fonts
lblTitle.Font = UIFonts.TitleLarge;
lblHeader.Font = UIFonts.HeaderStandard;
txtInput.Font = UIFonts.TextStandard;
dgv.DefaultCellStyle.Font = UIFonts.GridData;

// Create custom fonts
lblCustom.Font = UIFonts.CreateFont(11F, FontStyle.Bold);
lblNote.Font = UIFonts.CreateItalicFont(9F);
```

---

## ✨ Advanced Tips

### 1. Custom Button with Hover Effect
```csharp
private void SetupCustomButton(Button btn, Color baseColor, Color hoverColor)
{
    btn.Font = UIFonts.ButtonStandard;
    btn.ForeColor = Color.White;
    btn.BackColor = baseColor;
    btn.FlatStyle = FlatStyle.Flat;
    btn.FlatAppearance.BorderSize = 0;
    btn.Cursor = Cursors.Hand;
    
    btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
    btn.MouseLeave += (s, e) => btn.BackColor = baseColor;
}

// Usage
SetupCustomButton(btnCustom, UIColorPalette.PrimaryBlue, UIColorPalette.PrimaryBlueDark);
```

### 2. Conditional Row Coloring in DataGridView
```csharp
private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
{
    if (e.RowIndex < 0) return;
    
    var row = dgv.Rows[e.RowIndex];
    var item = row.DataBoundItem as YourModel;
    
    if (item != null && item.Quantity > item.Stock)
    {
        row.DefaultCellStyle.BackColor = UIColorPalette.StatusWarningBackground;
        row.DefaultCellStyle.ForeColor = UIColorPalette.StatusWarningText;
    }
    else
    {
        row.DefaultCellStyle.BackColor = Color.White;
        row.DefaultCellStyle.ForeColor = Color.Black;
    }
}
```

### 3. Dynamic Status Updates
```csharp
private async Task LoadDataAsync()
{
    try
    {
        UIStyleHelper.SetStatusMessage(lblStatus, "Đang tải dữ liệu...", StatusType.Processing);
        btnLoad.Enabled = false;
        
        var data = await YourService.GetDataAsync();
        dgv.DataSource = data;
        
        UIStyleHelper.SetStatusMessage(lblStatus, $"Đã tải {data.Count} bản ghi", StatusType.Success);
    }
    catch (Exception ex)
    {
        UIStyleHelper.SetStatusMessage(lblStatus, $"Lỗi: {ex.Message}", StatusType.Error);
    }
    finally
    {
        btnLoad.Enabled = true;
    }
}
```

---

## 📋 Checklist for Converting Existing Forms

- [ ] Add `using QLVT.Utils;`
- [ ] Apply form background: `UIStyleHelper.ApplyFormStyle(this);`
- [ ] Style title bar with `UIStyleHelper.ApplyTitleBarStyle()`
- [ ] Style all GroupBoxes with anchors
- [ ] Style all buttons (Primary/Success/Warning/Danger)
- [ ] Style all TextBoxes
- [ ] Style all Labels
- [ ] Style DataGridView (if present)
- [ ] Add status message updates with icons
- [ ] Test responsive behavior (resize form)
- [ ] Build and run

---

## 🎯 Button Color Guide

| Action Type | Helper Method | Color | Use Case |
|-------------|--------------|-------|----------|
| **Search, Query, Load** | `ApplyPrimaryButtonStyle()` | Blue | Tìm kiếm, truy vấn, load dữ liệu |
| **Refresh, Save** | `ApplySuccessButtonStyle()` | Green | Làm mới, lưu, reload |
| **Confirm, Submit** | `ApplyWarningButtonStyle()` | Orange | Xác nhận, thực thi |
| **Delete, Cancel** | `ApplyDangerButtonStyle()` | Red | Xóa, hủy, thoát |

---

## 🔧 Troubleshooting

### Issue: Colors not showing up
**Solution:** Make sure you added `using QLVT.Utils;` at the top of your file.

### Issue: Hover effects not working
**Solution:** The helper methods automatically add hover effects. Don't add them manually.

### Issue: DataGridView text is bold
**Solution:** Use `UIStyleHelper.ApplyDataGridViewStyle()` which sets Regular font automatically.

### Issue: Form not responsive
**Solution:** Make sure you set Anchor properties when calling `ApplyGroupBoxStyle()`.

---

## 📚 Additional Resources

- **Full Documentation:** `ModernUIDesignGuide.md`
- **Color Reference:** `UIColorPalette.cs`
- **Font Reference:** `UIFonts.cs`
- **Helper Methods:** `UIStyleHelper.cs`
- **Example Form:** `HoanUngBGKUserControl.cs`

---

**Happy Coding! 🚀**
