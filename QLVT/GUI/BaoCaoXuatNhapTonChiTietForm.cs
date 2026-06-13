using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.DAL;
using QLVT.Models;
using QLVT.GUI.Components;

namespace QLVT.GUI
{
    public partial class BaoCaoXuatNhapTonChiTietForm : Form
    {
        private readonly BaoCaoXuatNhapTonChiTietFormBLL bll;
        private List<BaoCaoXuatNhapTonChiTietItem> currentData;
        private BaoCaoXuatNhapTonChiTietFilter currentFilter;
        private List<WarehouseItem> _warehouses; // Use WarehouseItem for this BLL

        // Controls
        private DateTimePicker dtpTuNgay;
        private DateTimePicker dtpDenNgay;
        private TextBox txtWarehouse; // Use simple TextBox like BaoCaoTonKho
        private VatTuTextBox vatTuTextBox;
        private Button btnTimKiem;
        private Button btnExportExcel;
        private DataGridView dgvResults;
        private Label lblTotalRecords;
        private Label lblTuNgay;
        private Label lblDenNgay;
        private Label lblKho;
        private Label lblMaVatTu;
        
        // Custom warehouse dropdown
        private ListBox warehouseListBox = new ListBox();
        private bool suppressTextChanged = false;

        private bool isInitializing = true;
        private bool isExporting = false;

        public BaoCaoXuatNhapTonChiTietForm()
        {
            bll = new BaoCaoXuatNhapTonChiTietFormBLL();
            currentData = new List<BaoCaoXuatNhapTonChiTietItem>();
            currentFilter = new BaoCaoXuatNhapTonChiTietFilter();
            _warehouses = new List<WarehouseItem>();
            
            InitializeComponent();
            SetupForm();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            SetupEventHandlers();
            LoadWarehousesAsync();
        }

        /// <summary>
        /// Constructor với tham số từ báo cáo tổng
        /// </summary>
        public BaoCaoXuatNhapTonChiTietForm(DateTime tuNgay, DateTime denNgay, string tenKho, string maVatTu) : this()
        {
            // Set các giá trị từ báo cáo tổng
            dtpTuNgay.Value = tuNgay;
            dtpDenNgay.Value = denNgay;
            
            // Set warehouse using simple text
            if (!string.IsNullOrEmpty(tenKho))
            {
                txtWarehouse.Text = tenKho;
            }
            
            // Set VatTu using component method
            if (!string.IsNullOrEmpty(maVatTu))
            {
                vatTuTextBox.SetText(maVatTu);
            }
            
            isInitializing = false;
            
            // Tự động chạy báo cáo sau khi form load
            this.Load += async (sender, e) => {
                await Task.Delay(100); // Delay để component load xong
                await LoadDataAsync();
            };
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            
            // Form properties
            this.Text = "Báo cáo xuất nhập tồn chi tiết";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);

            // Initialize controls
            InitializeControls();
            SetupLayout();
            SetupEventHandlers();
        }

        private void InitializeControls()
        {
            // Labels
            lblTuNgay = new Label
            {
                Text = "Từ ngày:",
                AutoSize = true,
                Location = new Point(20, 20)
            };

            lblDenNgay = new Label
            {
                Text = "Đến ngày:",
                AutoSize = true,
                Location = new Point(200, 20)
            };

            lblKho = new Label
            {
                Text = "Kho:",
                AutoSize = true,
                Location = new Point(380, 20)
            };

            lblMaVatTu = new Label
            {
                Text = "Mã vật tư:",
                AutoSize = true,
                Location = new Point(580, 20)
            };

            // Date pickers
            dtpTuNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Location = new Point(20, 45),
                Size = new Size(150, 25),
                Value = DateTime.Now.AddDays(-30)
            };

            dtpDenNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Location = new Point(200, 45),
                Size = new Size(150, 25),
                Value = DateTime.Now
            };

            // TextBox for warehouse (simple like BaoCaoTonKho)
            txtWarehouse = new TextBox
            {
                Location = new Point(380, 45),
                Size = new Size(160, 25),
                AutoCompleteMode = AutoCompleteMode.None
            };

            // Text box
            vatTuTextBox = new VatTuTextBox(150, 25)
            {
                Location = new Point(560, 45),
                PlaceholderText = "Nhập mã vật tư..."
            };

            // Buttons
            btnTimKiem = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(730, 45),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnExportExcel = new Button
            {
                Text = "Xuất Excel",
                Location = new Point(850, 45),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // DataGridView
            dgvResults = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(1140, 500),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AutoGenerateColumns = false // Không tự tạo cột
            };

            // Label for total records
            lblTotalRecords = new Label
            {
                Text = "Tổng số bản ghi: 0",
                Location = new Point(20, 610),
                Size = new Size(200, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                lblTuNgay, lblDenNgay, lblKho, lblMaVatTu,
                dtpTuNgay, dtpDenNgay, txtWarehouse, vatTuTextBox,
                btnTimKiem, btnExportExcel, dgvResults, lblTotalRecords
            });
        }

        private void SetupLayout()
        {
            // Setup DataGridView columns
            SetupDataGridViewColumns();
        }

        private void SetupDataGridViewColumns()
        {
            dgvResults.Columns.Clear();

            var columns = new[]
            {
                new { Name = "NgayGiaoDichFormatted", Header = "Ngày", Width = 100 }, // Vừa đủ cho ngày tháng
                new { Name = "SoPhieu", Header = "Số phiếu", Width = 120 }, // Vừa đủ cho 15 ký tự
                new { Name = "GhiChu", Header = "Nội dung phiếu", Width = -1 }, // -1 = Fill remaining space
                new { Name = "SoLuongNhap", Header = "Số lượng nhập", Width = 100 }, // Vừa đủ cho 10 chữ số
                new { Name = "SoLuongXuat", Header = "Số lượng xuất", Width = 100 }, // Vừa đủ cho 10 chữ số
                new { Name = "TonSauGD", Header = "Tồn kho", Width = 100 } // Vừa đủ cho 10 chữ số
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    DataPropertyName = col.Name,
                    HeaderText = col.Header,
                    Width = col.Width > 0 ? col.Width : 200 // Temporary width for fill column
                };

                // Format number columns
                if (col.Name.Contains("SoLuong") || col.Name == "TonSauGD")
                {
                    column.DefaultCellStyle.Format = "N2";
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (col.Name == "NgayGiaoDichFormatted")
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Set AutoSizeMode for fill column
                if (col.Width == -1)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    column.MinimumWidth = 150; // Minimum width for content
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }

                dgvResults.Columns.Add(column);
            }
        }

        private void SetupEventHandlers()
        {
            btnTimKiem.Click += BtnTimKiem_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            this.Load += BaoCaoXuatNhapTonChiTietForm_Load;
            
            // DataGridView events
            dgvResults.CellDoubleClick += DgvResults_CellDoubleClick;
            
            // Component events
            vatTuTextBox.TextChanged += VatTuTextBox_TextChanged;
            
            // Check if all fields are filled
            dtpTuNgay.ValueChanged += CheckAllFieldsFilled;
            dtpDenNgay.ValueChanged += CheckAllFieldsFilled;
            vatTuTextBox.TextChanged += (sender, e) => CheckAllFieldsFilled(null!, EventArgs.Empty);
        }

        private void SetupForm()
        {
            // Set default values
            currentFilter.TuNgay = dtpTuNgay.Value;
            currentFilter.DenNgay = dtpDenNgay.Value;
        }

        private void BaoCaoXuatNhapTonChiTietForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Không load trước danh sách kho nữa
                isInitializing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo form: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadWarehousesAsync()
        {
            try
            {
                _warehouses = await bll.GetWarehousesAsync();
                
                // Setup events like BaoCaoTonKho
                txtWarehouse.TextChanged += TxtWarehouse_TextChanged;
                txtWarehouse.KeyDown += TxtWarehouse_KeyDown;
                txtWarehouse.KeyPress += TxtWarehouse_KeyPress;
                txtWarehouse.Leave += TxtWarehouse_Leave;
                
                // Thêm gợi ý mặc định "Tất cả"
                txtWarehouse.PlaceholderText = "Nhập tên kho hoặc 'Tất cả'...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTimKiem_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Disable controls
                SetControlsEnabled(false);
                
                // Update filter using new components
                currentFilter.TuNgay = dtpTuNgay.Value.Date;
                currentFilter.DenNgay = dtpDenNgay.Value.Date;
                currentFilter.TenKho = txtWarehouse.Text.Trim();
                currentFilter.MaVatTu = vatTuTextBox.GetText();
                
                // Load data
                currentData = await bll.GetBaoCaoXuatNhapTonChiTietAsync(currentFilter);
                
                // Bind to grid
                dgvResults.DataSource = currentData;
                
                // Update record count
                lblTotalRecords.Text = $"Tổng số bản ghi: {currentData.Count}";
                
                // Enable export button if has data
                btnExportExcel.Enabled = currentData.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                currentData.Clear();
                dgvResults.DataSource = null;
                lblTotalRecords.Text = "Tổng số bản ghi: 0";
                btnExportExcel.Enabled = false;
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private async void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (isExporting) return; // Prevent double-click
            
            try
            {
                if (currentData == null || !currentData.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                isExporting = true;
                SetControlsEnabled(false);
                bool success = bll.ExportToExcel(currentData, currentFilter);
                
                if (success)
                {
                    MessageBox.Show("Đã xuất báo cáo Excel thành công!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isExporting = false;
                SetControlsEnabled(true);
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            dtpTuNgay.Enabled = enabled;
            dtpDenNgay.Enabled = enabled;
            txtWarehouse.Enabled = enabled;
            vatTuTextBox.Enabled = enabled;
            btnTimKiem.Enabled = enabled;
            btnExportExcel.Enabled = enabled && currentData.Count > 0;
        }

        #region Warehouse and VatTu Events

        private void VatTuTextBox_TextChanged(object sender, EventArgs e)
        {
            // Handle VatTu text change if needed
        }

        private void CheckAllFieldsFilled(object sender, EventArgs e)
        {
            if (isInitializing) return;

            // Check if all required fields are filled using new components
            bool allFilled = !string.IsNullOrWhiteSpace(txtWarehouse.Text) &&
                           !string.IsNullOrWhiteSpace(vatTuTextBox.GetText());

            // Enable button if all fields filled, but don't auto-run
            btnTimKiem.Enabled = allFilled;
        }

        private static bool _isEditFormOpen = false; // Prevent multiple forms

        private async void DgvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Prevent opening multiple edit forms
                if (_isEditFormOpen)
                {
                    return;
                }

                // Kiểm tra có row được chọn và không phải header
                if (e.RowIndex < 0 || dgvResults.Rows[e.RowIndex].DataBoundItem == null)
                    return;

                // Lấy item từ row được double-click
                var item = dgvResults.Rows[e.RowIndex].DataBoundItem as BaoCaoXuatNhapTonChiTietItem;
                if (item == null) return;

                // Kiểm tra SoPhieu có hợp lệ không
                if (string.IsNullOrWhiteSpace(item.SoPhieu) || item.SoPhieu == "Tồn đầu kỳ")
                {
                    MessageBox.Show("Không thể chỉnh sửa dòng này!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _isEditFormOpen = true;

                // Tìm transaction bằng số phiếu
                var transactionDAL = new TransactionDAL();
                var transaction = await transactionDAL.GetTransactionBySoPhieuAsync(item.SoPhieu);
                
                if (transaction == null)
                {
                    MessageBox.Show($"Không tìm thấy phiếu với số: {item.SoPhieu}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isEditFormOpen = false;
                    return;
                }

                // Mở form chỉnh sửa phiếu
                var editForm = new PhieuEditForm(transaction.TransactionID);
                try
                {
                    editForm.ShowDialog();
                }
                finally
                {
                    _isEditFormOpen = false; // Reset flag when form closes
                }

                // Có thể refresh lại dữ liệu sau khi chỉnh sửa
                // await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Custom Warehouse AutoComplete (from BaoCaoTonKho)

        private void TxtWarehouse_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChanged) return;

            var searchText = txtWarehouse.Text;
            
            if (string.IsNullOrEmpty(searchText))
            {
                HideWarehouseDropdown();
                return;
            }

            var matchingItems = new List<string>();
            
            // Thêm tùy chọn "Tất cả" nếu người dùng nhập từ khóa phù hợp
            if ("Tất cả".Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                "tat ca".Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                "all".Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                matchingItems.Add("Tất cả");
            }

            // Tìm các kho có chứa text (không phân biệt hoa thường)
            var matchingWarehouses = _warehouses
                .Where(w => w.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Select(w => w.Name)
                .ToList();
                
            matchingItems.AddRange(matchingWarehouses);

            if (matchingItems.Any())
            {
                ShowWarehouseDropdown(matchingItems);
            }
            else
            {
                HideWarehouseDropdown();
            }
        }

        private void TxtWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (warehouseListBox.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        if (warehouseListBox.SelectedIndex < warehouseListBox.Items.Count - 1)
                            warehouseListBox.SelectedIndex++;
                        e.Handled = true;
                        break;
                    case Keys.Up:
                        if (warehouseListBox.SelectedIndex > 0)
                            warehouseListBox.SelectedIndex--;
                        e.Handled = true;
                        break;
                    case Keys.Enter:
                        if (warehouseListBox.SelectedIndex >= 0)
                        {
                            SelectWarehouse(warehouseListBox.SelectedItem?.ToString() ?? "");
                        }
                        e.Handled = true;
                        break;
                    case Keys.Escape:
                        HideWarehouseDropdown();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void TxtWarehouse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnTimKiem_Click(sender, e);
            }
        }

        private void TxtWarehouse_Leave(object sender, EventArgs e)
        {
            // Delay để cho phép click vào listbox
            Task.Delay(200).ContinueWith(_ =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (!warehouseListBox.Focused)
                    {
                        HideWarehouseDropdown();
                    }
                });
            });
        }

        private void ShowWarehouseDropdown(List<string> items)
        {
            if (!this.Controls.Contains(warehouseListBox))
            {
                this.Controls.Add(warehouseListBox);
                warehouseListBox.BringToFront();
                
                warehouseListBox.Click += (s, e) =>
                {
                    if (warehouseListBox.SelectedItem != null)
                    {
                        SelectWarehouse(warehouseListBox.SelectedItem.ToString() ?? "");
                    }
                };
            }

            warehouseListBox.Items.Clear();
            warehouseListBox.Items.AddRange(items.ToArray());
            
            // Positioning
            var txtLocation = txtWarehouse.PointToScreen(Point.Empty);
            var parentLocation = this.PointToScreen(Point.Empty);
            
            warehouseListBox.Location = new Point(
                txtLocation.X - parentLocation.X,
                txtLocation.Y - parentLocation.Y + txtWarehouse.Height
            );
            warehouseListBox.Width = txtWarehouse.Width;
            warehouseListBox.Height = Math.Min(150, items.Count * 20 + 4);
            
            warehouseListBox.Visible = true;
            
            if (items.Count > 0)
            {
                warehouseListBox.SelectedIndex = 0;
            }
        }

        private void HideWarehouseDropdown()
        {
            warehouseListBox.Visible = false;
        }

        private void SelectWarehouse(string warehouseName)
        {
            suppressTextChanged = true;
            txtWarehouse.Text = warehouseName;
            suppressTextChanged = false;
            
            HideWarehouseDropdown();
            txtWarehouse.SelectionStart = txtWarehouse.Text.Length;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private IContainer components = null!;
    }
}
