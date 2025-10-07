using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using QLVT.Models;
using QLVT.DAL;

namespace QLVT.GUI.Controls
{
    public partial class WarehouseComboBox : ComboBox
    {
        private List<Warehouse> _warehouses = new List<Warehouse>();
        private bool _includeAllOption = false;
        private const string ALL_WAREHOUSES_TEXT = "Tất cả";
        private bool _isLoading = false;

        public WarehouseComboBox()
        {
            InitializeComponent();
        }

        #region Properties

        /// <summary>
        /// Có hiển thị tùy chọn "Tất cả" hay không
        /// </summary>
        [Category("Behavior")]
        [Description("Có hiển thị tùy chọn 'Tất cả' hay không")]
        public bool IncludeAllOption
        {
            get => _includeAllOption;
            set
            {
                if (_includeAllOption != value)
                {
                    _includeAllOption = value;
                    RefreshDataSource();
                }
            }
        }

        /// <summary>
        /// Kho được chọn hiện tại (null nếu chọn "Tất cả" hoặc không chọn gì)
        /// </summary>
        [Browsable(false)]
        public Warehouse? SelectedWarehouse
        {
            get
            {
                if (SelectedValue is int id && id > 0)
                {
                    return _warehouses.FirstOrDefault(w => w.Id == id);
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (_includeAllOption)
                    {
                        SelectAllWarehouses();
                    }
                    else
                    {
                        SelectedIndex = -1;
                    }
                }
                else
                {
                    SelectedValue = value.Id;
                }
            }
        }

        /// <summary>
        /// ID của kho được chọn (null nếu chọn "Tất cả" hoặc không chọn gì)
        /// </summary>
        [Browsable(false)]
        public int? SelectedWarehouseId
        {
            get
            {
                if (SelectedValue is int id && id > 0)
                {
                    return id;
                }
                return null;
            }
            set
            {
                if (value.HasValue && value.Value > 0)
                {
                    SelectedValue = value.Value;
                }
                else
                {
                    if (_includeAllOption)
                    {
                        SelectAllWarehouses();
                    }
                    else
                    {
                        SelectedIndex = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Có phải đang chọn "Tất cả" hay không
        /// </summary>
        [Browsable(false)]
        public bool IsAllWarehousesSelected
        {
            get
            {
                if (!_includeAllOption) return false;
                return SelectedValue is int id && id == -1;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event khi selection thay đổi
        /// </summary>
        public event EventHandler? SelectedWarehouseChanged;

        #endregion

        #region Methods

        private void InitializeComponent()
        {
            // Setup ComboBox properties giống như trong XuatKhoUserControl
            AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            AutoCompleteSource = AutoCompleteSource.ListItems;
            DropDownStyle = ComboBoxStyle.DropDown;
            Font = new System.Drawing.Font("Segoe UI", 9F);
            FormattingEnabled = true;

            // Handle selection change
            SelectedIndexChanged += WarehouseComboBox_SelectedIndexChanged;
            
            // Load data khi handle được tạo
            HandleCreated += WarehouseComboBox_HandleCreated;
        }

        private async void WarehouseComboBox_HandleCreated(object? sender, EventArgs e)
        {
            if (!_isLoading && _warehouses.Count == 0 && IsHandleCreated)
            {
                await LoadWarehousesAsync();
            }
        }

        private async Task LoadWarehousesAsync()
        {
            if (_isLoading) return;
            
            try
            {
                _isLoading = true;
                var warehouseDAL = new WarehouseDAL();
                _warehouses = await warehouseDAL.GetAllWarehousesAsync();
                
                // Invoke to UI thread
                if (InvokeRequired)
                {
                    Invoke(new Action(RefreshDataSource));
                }
                else
                {
                    RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                var action = new Action(() => 
                    MessageBox.Show($"Lỗi khi tải danh sách kho: {ex.Message}", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error));
                
                if (InvokeRequired)
                    Invoke(action);
                else
                    action();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadWarehouses()
        {
            // Wrapper cho compatibility
            _ = LoadWarehousesAsync();
        }

        private void RefreshDataSource()
        {
            if (_isLoading) return;

            var items = new List<WarehouseComboItem>();

            // Thêm tùy chọn "Tất cả" nếu cần
            if (_includeAllOption)
            {
                items.Add(new WarehouseComboItem
                {
                    Id = -1,
                    TenKho = ALL_WAREHOUSES_TEXT
                });
            }

            // Thêm các kho
            foreach (var warehouse in _warehouses)
            {
                items.Add(new WarehouseComboItem
                {
                    Id = warehouse.Id,
                    TenKho = warehouse.TenKho
                });
            }

            // Debug info
            Console.WriteLine($"WarehouseComboBox: Loading {items.Count} items, IncludeAll: {_includeAllOption}");

            // Giữ selection hiện tại
            var currentSelection = SelectedValue;
            
            DataSource = items;
            DisplayMember = "TenKho";
            ValueMember = "Id";

            // Khôi phục selection nếu có
            if (currentSelection != null)
            {
                SelectedValue = currentSelection;
            }
            else if (_includeAllOption && items.Count > 0)
            {
                // Chọn "Tất cả" làm mặc định nếu có
                // SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Refresh lại danh sách kho từ database
        /// </summary>
        public async Task RefreshWarehousesAsync()
        {
            await LoadWarehousesAsync();
        }

        /// <summary>
        /// Refresh lại danh sách kho từ database (sync wrapper)
        /// </summary>
        public async void RefreshWarehouses()
        {
            await LoadWarehousesAsync();
        }

        /// <summary>
        /// Chọn "Tất cả" (chỉ có hiệu lực khi IncludeAllOption = true)
        /// </summary>
        public void SelectAllWarehouses()
        {
            if (_includeAllOption)
            {
                SelectedValue = -1;
            }
        }

        /// <summary>
        /// Chọn kho theo ID
        /// </summary>
        public void SelectWarehouse(int warehouseId)
        {
            SelectedValue = warehouseId;
        }

        /// <summary>
        /// Clear selection
        /// </summary>
        public void ClearSelection()
        {
            SelectedIndex = -1;
        }

        #endregion

        #region Event Handlers

        private void WarehouseComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedWarehouseChanged?.Invoke(this, e);
        }

        #endregion

        #region Helper Classes

        private class WarehouseComboItem
        {
            public int Id { get; set; }
            public string TenKho { get; set; } = string.Empty;
        }

        #endregion
    }
}