# MIGRATION CHECKLIST - Converting Existing Forms to Modern UI

## 📋 Form: ________________________________
**Developer:** _______________  
**Date:** _______________  
**Status:** ⬜ Not Started | ⬜ In Progress | ⬜ Completed  

---

## ✅ Pre-Migration Steps

- [ ] Backup current form files
- [ ] Review current form layout and functionality
- [ ] Identify all controls that need styling
- [ ] Plan anchor/dock strategy for responsive design

---

## 🎨 STEP 1: Add References

### File: `YourForm.cs` (Top of file)
```csharp
using QLVT.Utils;  // UIColorPalette, UIFonts, UIStyleHelper
```

**Status:** ⬜ Complete  
**Notes:** _________________________________

---

## 🎨 STEP 2: Update Designer.cs or InitializeComponent

### 2.1 Form/UserControl Background
```csharp
UIStyleHelper.ApplyFormStyle(this);
```
**Status:** ⬜ Complete

---

### 2.2 Title Bar (if applicable)

**Control Name:** _________________

```csharp
UIStyleHelper.ApplyTitleBarStyle(lblTitle);
lblTitle.Text = "YOUR TITLE HERE";
```

**Status:** ⬜ Complete  
**Notes:** _________________________________

---

### 2.3 GroupBoxes

| Control Name | Anchor Style | Status |
|--------------|-------------|--------|
| ____________ | Top\|Left\|Right | ⬜ |
| ____________ | Top\|Left\|Right | ⬜ |
| ____________ | Top\|Bottom\|Left\|Right | ⬜ |
| ____________ | _______________ | ⬜ |

**Code Example:**
```csharp
UIStyleHelper.ApplyGroupBoxStyle(grpSearch, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
```

---

### 2.4 Buttons

| Control Name | Type | Size | Anchor | Status |
|--------------|------|------|--------|--------|
| __________ | Primary | 120x35 | ______ | ⬜ |
| __________ | Success | 120x35 | ______ | ⬜ |
| __________ | Warning | 200x40 | Bottom\|Left | ⬜ |
| __________ | Danger | 120x35 | ______ | ⬜ |

**Code Examples:**
```csharp
UIStyleHelper.ApplyPrimaryButtonStyle(btnSearch);
UIStyleHelper.ApplySuccessButtonStyle(btnRefresh);
UIStyleHelper.ApplyWarningButtonStyle(btnConfirm, new Size(200, 40));
UIStyleHelper.ApplyDangerButtonStyle(btnDelete);
```

**Status:** ⬜ All buttons complete

---

### 2.5 TextBoxes

| Control Name | Status |
|--------------|--------|
| ___________ | ⬜ |
| ___________ | ⬜ |
| ___________ | ⬜ |

**Code Example:**
```csharp
UIStyleHelper.ApplyTextBoxStyle(txtSearch);
```

**Status:** ⬜ All textboxes complete

---

### 2.6 Labels

| Control Name | Type | Status |
|--------------|------|--------|
| ___________ | Standard | ⬜ |
| ___________ | Status | ⬜ |
| ___________ | Standard | ⬜ |

**Code Examples:**
```csharp
UIStyleHelper.ApplyStandardLabelStyle(lblName);
UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Ready);
```

**Status:** ⬜ All labels complete

---

### 2.7 DataGridView (if applicable)

**Control Name:** _________________

```csharp
UIStyleHelper.ApplyDataGridViewStyle(dgvData);
```

**Columns Configuration:**

| Column Name | Type | Width | FillWeight | ReadOnly | Status |
|-------------|------|-------|------------|----------|--------|
| _________ | Text | ___ | ___% | Yes | ⬜ |
| _________ | Text | ___ | ___% | Yes | ⬜ |
| _________ | Numeric | ___ | ___% | No | ⬜ |

**Code Example:**
```csharp
dgv.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("MaVT", "Mã VT", "MaVatTu", 120, 15));
dgv.Columns.Add(UIStyleHelper.CreateNumericColumn("SoLuong", "Số lượng", "SoLuong", 120, 15, false));
```

**Status:** ⬜ Complete  
**Notes:** _________________________________

---

## 💻 STEP 3: Update Code-Behind (.cs file)

### 3.1 Remove Old Styling Code

**Items to remove:**
- [ ] Old color definitions
- [ ] Old font definitions
- [ ] Manual hover effect handlers (if using helper)
- [ ] Old status message formatting

**Status:** ⬜ Complete

---

### 3.2 Update Status Messages

**Replace old status updates with:**
```csharp
UIStyleHelper.SetStatusMessage(lblStatus, "Message here", StatusType.Success);
```

**Locations to update:**

| Method/Event | Old Code Line # | Status |
|--------------|----------------|--------|
| __________ | _______ | ⬜ |
| __________ | _______ | ⬜ |
| __________ | _______ | ⬜ |

**Status:** ⬜ All status messages updated

---

### 3.3 DataGridView Events (if applicable)

**CellFormatting for conditional coloring:**
```csharp
private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
{
    if (e.RowIndex < 0) return;
    var row = dgv.Rows[e.RowIndex];
    // Add your conditional logic here
}
```

**Status:** ⬜ Complete  
**Notes:** _________________________________

---

## 🧪 STEP 4: Testing

### 4.1 Build Test
- [ ] Project builds without errors
- [ ] No warnings related to UI changes
- [ ] All references resolved

**Status:** ⬜ Build successful

---

### 4.2 Visual Test
- [ ] Title bar displays correctly (if applicable)
- [ ] GroupBoxes have flat style and correct colors
- [ ] All buttons have correct colors (Blue/Green/Orange/Red)
- [ ] Button hover effects work
- [ ] TextBoxes have correct font and style
- [ ] Labels have correct font and colors
- [ ] DataGridView has correct font (Regular, not bold)
- [ ] Form background color is correct
- [ ] Status messages display with icons

**Status:** ⬜ Visual test passed

---

### 4.3 Responsive Test
- [ ] **Resize form wider:** All controls stretch horizontally
- [ ] **Resize form narrower:** No overlap or clipping
- [ ] **Resize form taller:** Main content area expands vertically
- [ ] **Resize form shorter:** Layout remains functional
- [ ] **Test on 1024x768 resolution**
- [ ] **Test on 1920x1080 resolution**
- [ ] **Test on high DPI (150%, 200%)**

**Status:** ⬜ Responsive test passed

---

### 4.4 Functional Test
- [ ] All buttons still work correctly
- [ ] All events fire properly
- [ ] DataGridView editing works (if applicable)
- [ ] Validation still works
- [ ] Data loading works
- [ ] Status messages update correctly
- [ ] No regression in functionality

**Status:** ⬜ Functional test passed

---

## 📝 STEP 5: Documentation

### Code Comments
- [ ] Added comments for complex styling
- [ ] Documented any custom color/font usage
- [ ] Updated XML documentation (if applicable)

### Screenshots (Optional)
- [ ] Before screenshot saved
- [ ] After screenshot saved
- [ ] Added to documentation folder

**Status:** ⬜ Documentation complete

---

## 🐛 Issues Found

| Issue Description | Severity | Status | Resolution |
|------------------|----------|--------|------------|
| ______________ | High/Med/Low | Open/Fixed | _________ |
| ______________ | High/Med/Low | Open/Fixed | _________ |
| ______________ | High/Med/Low | Open/Fixed | _________ |

---

## ✅ Final Checklist

- [ ] All styling applied correctly
- [ ] All tests passed
- [ ] No regressions in functionality
- [ ] Code committed to version control
- [ ] Peer review completed (if required)
- [ ] Documentation updated
- [ ] Form marked as "Migrated" in tracking sheet

---

## 📊 Migration Summary

**Total Controls Styled:** _______  
**Time Spent:** _______ hours  
**Complexity:** Easy / Medium / Hard  
**Issues Found:** _______  
**Overall Rating:** ⭐⭐⭐⭐⭐

---

## 💡 Notes & Lessons Learned

_________________________________________
_________________________________________
_________________________________________
_________________________________________

---

## 👤 Sign-off

**Developer:** _________________  
**Date:** _________________  
**Reviewer:** _________________  
**Date:** _________________  

---

**Migration Status:** ⬜ Complete ✅
