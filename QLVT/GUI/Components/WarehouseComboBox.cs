using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI.Components
{
    /// <summary>
    /// Advanced warehouse selection with lazy loading and multi-select
    /// </summary>
    public partial class WarehouseComboBox : UserControl
    {
        // UI Controls
        private TextBox txtSearch = null!;
        private Button btnDropdown = null!;
        private Panel pnlDropdown = null!;
        private ListBox lstWarehouses = null!;
        
        // Data
        private List<Warehouse>? allWarehouses;
        private List<Warehouse>? filteredWarehouses;
        private NhapKhoManualBLL warehouseBLL = null!;
        private System.Windows.Forms.Timer searchTimer = null!;
        
        // State
        private bool isDropdownOpen = false;
        private bool isSearching = false;
        private bool isDataLoaded = false;
        private List<int> selectedWarehouseIds = new List<int>();

        // Events
        public event EventHandler<WarehouseSelectedEventArgs>? WarehouseSelected;
        public event EventHandler? WarehouseChanged;
        public event EventHandler<MultiWarehouseSelectedEventArgs>? MultiWarehouseSelected;

        // Properties
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enable multi-select mode")]
        public bool MultiSelect { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether to allow empty selection")]
        public bool AllowEmptySelection { get; set; } = false;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Placeholder text when empty")]
        public string PlaceholderText { get; set; } = "Gõ để tìm kho...";

        [Browsable(true)]
        [Category("Layout")]
        [Description("Width of the control")]
        public int ComboBoxWidth { get; set; } = 300;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Height of the control")]
        public int ComboBoxHeight { get; set; } = 25;

        [Browsable(false)]
        public int SelectedWarehouseId
        {
            get => selectedWarehouseIds.FirstOrDefault();
            set
            {
                if (!MultiSelect)
                {
                    selectedWarehouseIds.Clear();
                    if (value > 0)
                    {
                        selectedWarehouseIds.Add(value);
                    }
                    UpdateDisplayText();
                }
            }
        }

        [Browsable(false)]
        public List<int> SelectedWarehouseIds
        {
            get => new List<int>(selectedWarehouseIds);
            set
            {
                selectedWarehouseIds = value ?? new List<int>();
                UpdateDisplayText();
            }
        }

        [Browsable(false)]
        public Warehouse? SelectedWarehouse
        {
            get
            {
                if (allWarehouses == null || selectedWarehouseIds.Count == 0)
                    return null;
                return allWarehouses.FirstOrDefault(w => w.Id == selectedWarehouseIds[0]);
            }
        }

        [Browsable(false)]
        public List<Warehouse> SelectedWarehouses
        {
            get
            {
                if (allWarehouses == null)
                    return new List<Warehouse>();
                return allWarehouses.Where(w => selectedWarehouseIds.Contains(w.Id)).ToList();
            }
        }

        [Browsable(false)]
        public string SelectedWarehouseName
        {
            get => SelectedWarehouse?.TenKho ?? string.Empty;
        }

        // Constructors
        public WarehouseComboBox() : this(300, 25, false, false)
        {
        }

        public WarehouseComboBox(int width, int height, bool allowEmpty) 
            : this(width, height, allowEmpty, false)
        {
        }

        public WarehouseComboBox(int width, int height, bool allowEmpty, bool multiSelect)
        {
            ComboBoxWidth = width;
            ComboBoxHeight = height;
            AllowEmptySelection = allowEmpty;
            MultiSelect = multiSelect;
            
            InitializeComponent();
            InitializeBLL();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // TextBox for search
            txtSearch = new TextBox
            {
                Location = new Point(0, 0),
                Size = new Size(ComboBoxWidth - 25, ComboBoxHeight),
                Font = new Font("Segoe UI", 9F)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyDown += TxtSearch_KeyDown;
            txtSearch.GotFocus += TxtSearch_GotFocus;
            txtSearch.LostFocus += TxtSearch_LostFocus;

            // Dropdown button
            btnDropdown = new Button
            {
                Location = new Point(ComboBoxWidth - 25, 0),
                Size = new Size(25, ComboBoxHeight),
                Text = "▼",
                Font = new Font("Segoe UI", 7F),
                FlatStyle = FlatStyle.Flat
            };
            btnDropdown.FlatAppearance.BorderSize = 1;
            btnDropdown.Click += BtnDropdown_Click;

            // Dropdown panel
            pnlDropdown = new Panel
            {
                Location = new Point(0, ComboBoxHeight),
                Size = new Size(ComboBoxWidth, 200),
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // ListBox with owner draw for highlighting
            lstWarehouses = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                IntegralHeight = false,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 20,
                SelectionMode = MultiSelect ? SelectionMode.MultiExtended : SelectionMode.One
            };
            lstWarehouses.DrawItem += LstWarehouses_DrawItem;
            lstWarehouses.SelectedIndexChanged += LstWarehouses_SelectedIndexChanged;
            lstWarehouses.MouseClick += LstWarehouses_MouseClick;

            pnlDropdown.Controls.Add(lstWarehouses);

            // Add controls
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnDropdown);
            this.Controls.Add(pnlDropdown);

            this.Name = "WarehouseComboBox";
            this.Size = new Size(ComboBoxWidth, ComboBoxHeight);
            
            // Search timer
            searchTimer = new System.Windows.Forms.Timer();
            searchTimer.Interval = 800; // 0.8 second
            searchTimer.Tick += SearchTimer_Tick;

            this.ResumeLayout(false);
        }

        private void InitializeBLL()
        {
            try
            {
                warehouseBLL = new NhapKhoManualBLL();
            }
            catch (Exception ex)
            {
                if (this.IsHandleCreated && !this.DesignMode)
                {
                    MessageBox.Show($"Lỗi khởi tạo: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TxtSearch_GotFocus(object? sender, EventArgs e)
        {
            // Load data if not loaded yet
            if (!isDataLoaded && !this.DesignMode)
            {
                LoadWarehousesAsync();
            }
            else if (isDataLoaded && !isDropdownOpen)
            {
                // Show all warehouses when focused
                FilterWarehouses("");
                ShowDropdown();
            }
        }

        private void TxtSearch_LostFocus(object? sender, EventArgs e)
        {
            // Auto-hide dropdown after short delay
            System.Threading.Tasks.Task.Delay(200).ContinueWith(t =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (!btnDropdown.Focused && !lstWarehouses.Focused)
                        {
                            HideDropdown();
                        }
                    }));
                }
            });
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            searchTimer.Stop();

            if (!isDataLoaded)
            {
                // If not loaded, trigger load first
                LoadWarehousesAsync();
                return;
            }

            // Filter immediately on text change
            FilterWarehouses(txtSearch.Text);
            
            if (!isDropdownOpen)
            {
                ShowDropdown();
            }
        }

        private void SearchTimer_Tick(object? sender, EventArgs e)
        {
            searchTimer.Stop();
            // Timer không còn cần thiết vì đã filter trực tiếp trong TextChanged
        }

        private async void LoadWarehousesAsync()
        {
            if (isSearching || this.DesignMode) return;

            try
            {
                isSearching = true;
                txtSearch.Enabled = false;
                txtSearch.Text = "Đang tải...";

                await System.Threading.Tasks.Task.Run(() =>
                {
                    allWarehouses = warehouseBLL.GetDanhSachKho();
                    filteredWarehouses = new List<Warehouse>(allWarehouses);
                });

                isDataLoaded = true;
                txtSearch.Text = "";
                txtSearch.PlaceholderText = PlaceholderText;
                
                UpdateListBox();
                ShowDropdown();
            }
            catch (Exception ex)
            {
                txtSearch.Text = "";
                MessageBox.Show($"Lỗi tải danh sách kho: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isSearching = false;
                txtSearch.Enabled = true;
            }
        }

        private void FilterWarehouses(string searchText)
        {
            if (allWarehouses == null) return;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredWarehouses = new List<Warehouse>(allWarehouses);
            }
            else
            {
                string search = searchText.ToLower().Trim();
                filteredWarehouses = allWarehouses.Where(w =>
                    w.TenKho.ToLower().Contains(search) ||
                    w.MaKho.ToLower().Contains(search)
                ).ToList();
            }

            UpdateListBox();
            
            if (!isDropdownOpen)
            {
                ShowDropdown();
            }
        }

        private void UpdateListBox()
        {
            lstWarehouses.Items.Clear();

            if (filteredWarehouses == null || filteredWarehouses.Count == 0)
            {
                lstWarehouses.Items.Add("❌ Không có kết quả");
                return;
            }

            // Add "Tất cả kho" option if AllowEmptySelection is true and no search text
            if (AllowEmptySelection && string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                lstWarehouses.Items.Add("-- Tất cả kho --");
            }

            foreach (var warehouse in filteredWarehouses)
            {
                lstWarehouses.Items.Add(warehouse);
                
                // Select if already in selected list
                if (selectedWarehouseIds.Contains(warehouse.Id))
                {
                    int index = lstWarehouses.Items.Count - 1;
                    lstWarehouses.SetSelected(index, true);
                }
            }
        }

        private void LstWarehouses_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            var item = lstWarehouses.Items[e.Index];
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            
            // Handle special messages ("Tất cả kho", "Không có kết quả")
            if (item is string message)
            {
                Color textColor = isSelected ? SystemColors.HighlightText : Color.Gray;
                
                // "Tất cả kho" in normal color, "Không có kết quả" in gray
                if (message == "-- Tất cả kho --")
                {
                    textColor = isSelected ? SystemColors.HighlightText : Color.Black;
                }
                
                using (var brush = new SolidBrush(textColor))
                {
                    e.Graphics.DrawString(message, e.Font!, brush, e.Bounds.Left + 5, e.Bounds.Top + 2);
                }
                e.DrawFocusRectangle();
                return;
            }

            var warehouse = item as Warehouse;
            if (warehouse == null) return;

            // Get search text for highlighting
            string searchText = txtSearch.Text.Trim().ToLower();

            // Format: "16  Kho Xi nghiệp XL CTT nước (Phùng Minh Nam)  Phùng Minh Nam"
            string displayText = $"{warehouse.Id}  {warehouse.TenKho}  {warehouse.MaKho}";

            // Background color
            Color backColor = isSelected ? SystemColors.Highlight : e.BackColor;
            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            // Draw text with highlighting
            if (!string.IsNullOrEmpty(searchText))
            {
                DrawHighlightedText(e.Graphics, displayText, searchText, e.Font!, e.Bounds, isSelected);
            }
            else
            {
                // No search - draw normal
                Color textColor = isSelected ? SystemColors.HighlightText : e.ForeColor;
                using (var textBrush = new SolidBrush(textColor))
                {
                    e.Graphics.DrawString(displayText, e.Font!, textBrush, e.Bounds.Left + 5, e.Bounds.Top + 2);
                }
            }

            e.DrawFocusRectangle();
        }

        private void DrawHighlightedText(Graphics graphics, string text, string searchText, Font font, Rectangle bounds, bool isSelected)
        {
            float x = bounds.Left + 5;
            float y = bounds.Top + 2;
            
            Color normalTextColor = isSelected ? SystemColors.HighlightText : Color.Black;
            Color highlightBackColor = Color.Yellow;
            Color highlightTextColor = Color.Black;

            // Find all occurrences of search text
            string textLower = text.ToLower();
            int startIndex = 0;

            while (startIndex < text.Length)
            {
                int foundIndex = textLower.IndexOf(searchText, startIndex);
                
                if (foundIndex == -1)
                {
                    // Draw remaining text
                    string remainingText = text.Substring(startIndex);
                    using (var brush = new SolidBrush(normalTextColor))
                    {
                        SizeF size = graphics.MeasureString(remainingText, font);
                        graphics.DrawString(remainingText, font, brush, x, y);
                    }
                    break;
                }

                // Draw text before match
                if (foundIndex > startIndex)
                {
                    string beforeText = text.Substring(startIndex, foundIndex - startIndex);
                    using (var brush = new SolidBrush(normalTextColor))
                    {
                        SizeF size = graphics.MeasureString(beforeText, font);
                        graphics.DrawString(beforeText, font, brush, x, y);
                        x += size.Width;
                    }
                }

                // Draw highlighted match
                string matchText = text.Substring(foundIndex, searchText.Length);
                SizeF matchSize = graphics.MeasureString(matchText, font);
                
                // Draw highlight background
                using (var highlightBrush = new SolidBrush(highlightBackColor))
                {
                    graphics.FillRectangle(highlightBrush, x, y, matchSize.Width, matchSize.Height);
                }
                
                // Draw match text
                using (var textBrush = new SolidBrush(highlightTextColor))
                {
                    graphics.DrawString(matchText, font, textBrush, x, y);
                }
                
                x += matchSize.Width;
                startIndex = foundIndex + searchText.Length;
            }
        }

        private void LstWarehouses_MouseClick(object? sender, MouseEventArgs e)
        {
            int index = lstWarehouses.IndexFromPoint(e.Location);
            if (index < 0 || index >= lstWarehouses.Items.Count) return;

            var item = lstWarehouses.Items[index];
            
            // Handle "Tất cả kho" selection
            if (item is string message)
            {
                if (message == "-- Tất cả kho --")
                {
                    selectedWarehouseIds.Clear();
                    txtSearch.Text = "";
                    HideDropdown();
                    RaiseSelectionChangedEvents();
                }
                return; // Skip other string messages
            }

            var warehouse = item as Warehouse;
            if (warehouse == null) return;

            if (MultiSelect)
            {
                // Toggle selection in multi-select mode
                if (selectedWarehouseIds.Contains(warehouse.Id))
                {
                    selectedWarehouseIds.Remove(warehouse.Id);
                    lstWarehouses.SetSelected(index, false);
                }
                else
                {
                    selectedWarehouseIds.Add(warehouse.Id);
                    lstWarehouses.SetSelected(index, true);
                }

                UpdateDisplayText();
                RaiseSelectionChangedEvents();
            }
            else
            {
                // Single select mode handled by SelectedIndexChanged
            }
        }

        private void LstWarehouses_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Single click selection (without checkbox)
            if (!MultiSelect && lstWarehouses.SelectedIndex >= 0)
            {
                var item = lstWarehouses.SelectedItem;
                
                // Handle "Tất cả kho"
                if (item is string message && message == "-- Tất cả kho --")
                {
                    selectedWarehouseIds.Clear();
                    txtSearch.Text = "";
                    HideDropdown();
                    RaiseSelectionChangedEvents();
                    return;
                }
                
                if (item is Warehouse warehouse)
                {
                    selectedWarehouseIds.Clear();
                    selectedWarehouseIds.Add(warehouse.Id);
                    UpdateDisplayText();
                    HideDropdown();
                    RaiseSelectionChangedEvents();
                }
            }
        }

        private void UpdateDisplayText()
        {
            if (selectedWarehouseIds.Count == 0)
            {
                txtSearch.Text = "";
                return;
            }

            if (MultiSelect && selectedWarehouseIds.Count > 1)
            {
                txtSearch.Text = $"✓ {selectedWarehouseIds.Count} kho đã chọn";
            }
            else
            {
                var warehouse = SelectedWarehouse;
                if (warehouse != null)
                {
                    txtSearch.Text = warehouse.TenKho;
                }
            }
        }

        private void RaiseSelectionChangedEvents()
        {
            if (MultiSelect)
            {
                var selectedWarehouses = SelectedWarehouses;
                MultiWarehouseSelected?.Invoke(this, new MultiWarehouseSelectedEventArgs(selectedWarehouses));
            }
            else
            {
                var warehouse = SelectedWarehouse;
                if (warehouse != null)
                {
                    WarehouseSelected?.Invoke(this, new WarehouseSelectedEventArgs(warehouse));
                }
            }

            WarehouseChanged?.Invoke(this, EventArgs.Empty);
        }

        private void BtnDropdown_Click(object? sender, EventArgs e)
        {
            if (isDropdownOpen)
            {
                HideDropdown();
            }
            else
            {
                if (!isDataLoaded)
                {
                    LoadWarehousesAsync();
                }
                else
                {
                    ShowDropdown();
                }
            }
        }

        private void ShowDropdown()
        {
            pnlDropdown.Visible = true;
            pnlDropdown.BringToFront();
            isDropdownOpen = true;
            btnDropdown.Text = "▲";
        }

        private void HideDropdown()
        {
            pnlDropdown.Visible = false;
            isDropdownOpen = false;
            btnDropdown.Text = "▼";
        }

        private void TxtSearch_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                
                if (isDropdownOpen && lstWarehouses.Items.Count > 0)
                {
                    if (lstWarehouses.Items[0] is Warehouse warehouse)
                    {
                        selectedWarehouseIds.Clear();
                        selectedWarehouseIds.Add(warehouse.Id);
                        lstWarehouses.SetSelected(0, true);
                        UpdateDisplayText();
                        HideDropdown();
                        RaiseSelectionChangedEvents();
                    }
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                HideDropdown();
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (!isDropdownOpen)
                {
                    if (!isDataLoaded)
                    {
                        LoadWarehousesAsync();
                    }
                    else
                    {
                        ShowDropdown();
                    }
                }
            }
        }

        // Public methods for backward compatibility
        public void SetSelectedWarehouse(int warehouseId)
        {
            SelectedWarehouseId = warehouseId;
        }

        public void SetSelectedWarehouse(string warehouseName)
        {
            if (allWarehouses != null)
            {
                var warehouse = allWarehouses.FirstOrDefault(w =>
                    string.Equals(w.TenKho, warehouseName, StringComparison.OrdinalIgnoreCase));
                if (warehouse != null)
                {
                    SelectedWarehouseId = warehouse.Id;
                }
            }
        }

        public void ClearSelection()
        {
            selectedWarehouseIds.Clear();
            UpdateDisplayText();
            
            // Clear all selections in ListBox
            for (int i = 0; i < lstWarehouses.Items.Count; i++)
            {
                lstWarehouses.SetSelected(i, false);
            }
        }

        public bool ValidateSelection()
        {
            if (!AllowEmptySelection && selectedWarehouseIds.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn kho!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearch.Focus();
                return false;
            }
            return true;
        }

        public string GetSelectedWarehouseName()
        {
            return SelectedWarehouseName;
        }

        public string GetText()
        {
            return txtSearch.Text;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.DesignMode) return;
        }
    }

    // Event args
    public class WarehouseSelectedEventArgs : EventArgs
    {
        public Warehouse SelectedWarehouse { get; }
        public string WarehouseName => SelectedWarehouse?.TenKho ?? string.Empty;
        public int WarehouseId => SelectedWarehouse?.Id ?? -1;

        public WarehouseSelectedEventArgs(Warehouse warehouse)
        {
            SelectedWarehouse = warehouse;
        }
    }

    public class MultiWarehouseSelectedEventArgs : EventArgs
    {
        public List<Warehouse> SelectedWarehouses { get; }
        public List<int> WarehouseIds => SelectedWarehouses.Select(w => w.Id).ToList();
        public int Count => SelectedWarehouses.Count;

        public MultiWarehouseSelectedEventArgs(List<Warehouse> warehouses)
        {
            SelectedWarehouses = warehouses ?? new List<Warehouse>();
        }
    }
}
