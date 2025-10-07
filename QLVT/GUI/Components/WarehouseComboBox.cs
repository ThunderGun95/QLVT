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
    /// Reusable ComboBox component for selecting warehouses - same as NhapKho style
    /// </summary>
    public partial class WarehouseComboBox : UserControl
    {
        private ComboBox cboWarehouse;
        private List<Warehouse> warehouses;
        private NhapKhoManualBLL warehouseBLL;
        private int selectedWarehouseId = -1;

        // Events
        public event EventHandler<WarehouseSelectedEventArgs> WarehouseSelected;
        public event EventHandler WarehouseChanged;

        // Properties
        [Browsable(false)]
        public int SelectedWarehouseId
        {
            get => selectedWarehouseId;
            set
            {
                if (warehouses != null && warehouses.Any(w => w.Id == value))
                {
                    cboWarehouse.SelectedValue = value;
                    selectedWarehouseId = value;
                }
            }
        }

        [Browsable(false)]
        public Warehouse SelectedWarehouse
        {
            get => cboWarehouse.SelectedItem as Warehouse;
        }

        [Browsable(false)]
        public string SelectedWarehouseName
        {
            get => SelectedWarehouse?.TenKho ?? string.Empty;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether to allow empty selection")]
        public bool AllowEmptySelection { get; set; } = false;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text to show for empty selection")]
        public string EmptySelectionText { get; set; } = "-- Chọn kho --";

        [Browsable(true)]
        [Category("Layout")]
        [Description("Width of the ComboBox control")]
        public int ComboBoxWidth { get; set; } = 160;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Height of the ComboBox control")]
        public int ComboBoxHeight { get; set; } = 23;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Total width of the component")]
        public new int Width 
        { 
            get => base.Width; 
            set 
            {
                base.Width = value;
                ComboBoxWidth = value;
                UpdateLayout();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Total height of the component")]
        public new int Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                ComboBoxHeight = value;
                UpdateLayout();
            }
        }

        public WarehouseComboBox() : this(160, 23, false)
        {
        }

        public WarehouseComboBox(int width, int height, bool addAll)
        {
            ComboBoxWidth = width;
            ComboBoxHeight = height;
            AllowEmptySelection = addAll;
            if (addAll)
            {
                EmptySelectionText = "-- Tất cả kho --";
            }
            
            InitializeComponent();
            InitializeData();
        }

        private void InitializeComponent()
        {
            this.cboWarehouse = new ComboBox();
            this.SuspendLayout();

            // cboWarehouse - same style as NhapKho, NO LABEL
            this.cboWarehouse.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.cboWarehouse.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cboWarehouse.Font = new Font("Segoe UI", 9F);
            this.cboWarehouse.FormattingEnabled = true;
            this.cboWarehouse.Location = new Point(0, 0);
            this.cboWarehouse.Name = "cboWarehouse";
            this.cboWarehouse.Size = new Size(ComboBoxWidth, ComboBoxHeight);
            this.cboWarehouse.TabIndex = 0;
            this.cboWarehouse.SelectedIndexChanged += CboWarehouse_SelectedIndexChanged;

            // WarehouseComboBox - chỉ có ComboBox, không có label
            this.Controls.Add(this.cboWarehouse);
            this.Name = "WarehouseComboBox";
            this.Size = new Size(ComboBoxWidth, ComboBoxHeight);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeData()
        {
            try
            {
                warehouseBLL = new NhapKhoManualBLL();
                LoadWarehouses();
            }
            catch (Exception ex)
            {
                if (this.IsHandleCreated && !this.DesignMode)
                {
                    MessageBox.Show($"Lỗi khởi tạo component chọn kho: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void LoadWarehouses()
        {
            try
            {
                // Load warehouses exactly like NhapKho
                warehouses = warehouseBLL.GetDanhSachKho();
                cboWarehouse.Items.Clear();
                cboWarehouse.DisplayMember = "TenKho";
                cboWarehouse.ValueMember = "Id";

                var dataSource = new List<Warehouse>();
                
                if (AllowEmptySelection)
                {
                    dataSource.Add(new Warehouse { Id = -1, TenKho = EmptySelectionText });
                }
                
                dataSource.AddRange(warehouses);
                cboWarehouse.DataSource = dataSource;

                // Chọn giá trị đầu tiên
                if (dataSource.Count > 0)
                {
                    cboWarehouse.SelectedIndex = 0;
                    selectedWarehouseId = ((Warehouse)cboWarehouse.SelectedItem).Id;
                }
            }
            catch (Exception ex)
            {
                if (this.IsHandleCreated && !this.DesignMode)
                {
                    MessageBox.Show($"Lỗi tải danh sách kho: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CboWarehouse_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                // Same logic as NhapKho
                if (cboWarehouse.SelectedValue != null && int.TryParse(cboWarehouse.SelectedValue.ToString(), out int warehouseId))
                {
                    selectedWarehouseId = warehouseId;
                    
                    var selectedWarehouse = SelectedWarehouse;
                    if (selectedWarehouse != null && selectedWarehouse.Id != -1)
                    {
                        WarehouseSelected?.Invoke(this, new WarehouseSelectedEventArgs(selectedWarehouse));
                    }
                    WarehouseChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                if (this.IsHandleCreated && !this.DesignMode)
                {
                    MessageBox.Show($"Lỗi khi chọn kho: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void SetSelectedWarehouse(int warehouseId)
        {
            SelectedWarehouseId = warehouseId;
        }

        public void SetSelectedWarehouse(string warehouseName)
        {
            if (warehouses != null)
            {
                var warehouse = warehouses.FirstOrDefault(w => 
                    string.Equals(w.TenKho, warehouseName, StringComparison.OrdinalIgnoreCase));
                if (warehouse != null)
                {
                    SelectedWarehouseId = warehouse.Id;
                }
            }
        }

        public void ClearSelection()
        {
            if (AllowEmptySelection)
            {
                cboWarehouse.SelectedValue = -1;
                selectedWarehouseId = -1;
            }
            else if (warehouses?.Count > 0)
            {
                cboWarehouse.SelectedValue = warehouses[0].Id;
                selectedWarehouseId = warehouses[0].Id;
            }
        }

        public bool ValidateSelection()
        {
            if (!AllowEmptySelection && selectedWarehouseId == -1)
            {
                MessageBox.Show("Vui lòng chọn kho!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboWarehouse.Focus();
                return false;
            }
            return true;
        }
        
        // Additional helper methods for form integration
        public string GetSelectedWarehouseName()
        {
            return SelectedWarehouseName ?? string.Empty;
        }
        
        public void SetSelectedWarehouse(int warehouseId, string warehouseName)
        {
            var warehouse = warehouses?.FirstOrDefault(w => w.Id == warehouseId);
            if (warehouse != null)
            {
                cboWarehouse.SelectedValue = warehouseId;
                selectedWarehouseId = warehouseId;
            }
        }

        // Override to handle design time
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.DesignMode) return;
            
            // Reload data if needed
            if (warehouses == null || warehouses.Count == 0)
            {
                LoadWarehouses();
            }
        }

        private void UpdateLayout()
        {
            if (cboWarehouse != null)
            {
                // Cập nhật size của ComboBox
                cboWarehouse.Size = new Size(ComboBoxWidth, ComboBoxHeight);
                
                // Cập nhật tổng size component
                this.Size = new Size(ComboBoxWidth, ComboBoxHeight);
            }
        }
    }

    // Event args for warehouse selection
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
}