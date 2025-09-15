using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class BaoCaoXuatNhapTonChiTietForm : Form
    {
        private readonly BaoCaoXuatNhapTonChiTietFormBLL bll;
        private List<BaoCaoXuatNhapTonChiTietItem> currentData;
        private BaoCaoXuatNhapTonChiTietFilter currentFilter;

        // Controls
        private DateTimePicker dtpTuNgay;
        private DateTimePicker dtpDenNgay;
        private TextBox txtTenKho;
        private ListBox lstKho;
        private TextBox txtMaVatTu;
        private Button btnTimKiem;
        private Button btnExportExcel;
        private DataGridView dgvResults;
        private Label lblTotalRecords;
        private Label lblTuNgay;
        private Label lblDenNgay;
        private Label lblKho;
        private Label lblMaVatTu;

        private bool isInitializing = true;
        private bool isSettingText = false;
        private System.Windows.Forms.Timer searchTimer;

        public BaoCaoXuatNhapTonChiTietForm()
        {
            bll = new BaoCaoXuatNhapTonChiTietFormBLL();
            currentData = new List<BaoCaoXuatNhapTonChiTietItem>();
            currentFilter = new BaoCaoXuatNhapTonChiTietFilter();
            
            // Initialize search timer
            searchTimer = new System.Windows.Forms.Timer();
            searchTimer.Interval = 1000; // 1 second
            searchTimer.Tick += SearchTimer_Tick;
            
            InitializeComponent();
            SetupForm();
            SetupEventHandlers();
        }

        /// <summary>
        /// Constructor với tham số từ báo cáo tổng
        /// </summary>
        public BaoCaoXuatNhapTonChiTietForm(DateTime tuNgay, DateTime denNgay, string tenKho, string maVatTu) : this()
        {
            // Set các giá trị từ báo cáo tổng
            dtpTuNgay.Value = tuNgay;
            dtpDenNgay.Value = denNgay;
            txtTenKho.Text = tenKho;
            txtMaVatTu.Text = maVatTu;
            
            isInitializing = false;
            
            // Tự động chạy báo cáo
            _ = LoadDataAsync();
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

            // Combo box -> TextBox + ListBox for warehouse
            txtTenKho = new TextBox
            {
                Location = new Point(380, 45),
                Size = new Size(180, 25),
                PlaceholderText = "Nhập tên kho..."
            };

            lstKho = new ListBox
            {
                Location = new Point(380, 70),
                Size = new Size(180, 100),
                Visible = false
            };

            // Text box
            txtMaVatTu = new TextBox
            {
                Location = new Point(580, 45),
                Size = new Size(150, 25),
                PlaceholderText = "Nhập mã vật tư..."
            };

            // Buttons
            btnTimKiem = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(750, 45),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnExportExcel = new Button
            {
                Text = "Xuất Excel",
                Location = new Point(870, 45),
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
                BorderStyle = BorderStyle.Fixed3D
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
                dtpTuNgay, dtpDenNgay, txtTenKho, lstKho, txtMaVatTu,
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
                new { Name = "STT", Header = "STT", Width = 50 },
                new { Name = "NgayGiaoDichFormatted", Header = "Ngày", Width = 120 },
                new { Name = "LoaiGiaoDich", Header = "Loại GD", Width = 80 },
                new { Name = "SoPhieu", Header = "Số phiếu", Width = 100 },
                new { Name = "MaVatTu", Header = "Mã vật tư", Width = 100 },
                new { Name = "TenVatTu", Header = "Tên vật tư", Width = 200 },
                new { Name = "DonViTinh", Header = "ĐVT", Width = 60 },
                new { Name = "TenKho", Header = "Kho", Width = 120 },
                new { Name = "SoLuongNhap", Header = "SL nhập", Width = 80 },
                new { Name = "SoLuongXuat", Header = "SL xuất", Width = 80 },
                new { Name = "TonSauGD", Header = "Tồn sau GD", Width = 100 },
                new { Name = "GhiChu", Header = "Ghi chú", Width = 150 }
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    DataPropertyName = col.Name,
                    HeaderText = col.Header,
                    Width = col.Width
                };

                // Format number columns
                if (col.Name.Contains("SoLuong") || col.Name == "TonSauGD")
                {
                    column.DefaultCellStyle.Format = "N2";
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (col.Name == "STT")
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                dgvResults.Columns.Add(column);
            }
        }

        private void SetupEventHandlers()
        {
            btnTimKiem.Click += BtnTimKiem_Click;
            btnExportExcel.Click += BtnExportExcel_Click;
            this.Load += BaoCaoXuatNhapTonChiTietForm_Load;
            
            // Warehouse TextBox events
            txtTenKho.TextChanged += TxtTenKho_TextChanged;
            txtTenKho.Leave += TxtTenKho_Leave;
            
            // Warehouse ListBox events
            lstKho.Click += LstKho_Click;
            
            // Check if all fields are filled
            dtpTuNgay.ValueChanged += CheckAllFieldsFilled;
            dtpDenNgay.ValueChanged += CheckAllFieldsFilled;
            txtTenKho.TextChanged += CheckAllFieldsFilled;
            txtMaVatTu.TextChanged += CheckAllFieldsFilled;
        }

        private void SetupForm()
        {
            // Set default values
            currentFilter.TuNgay = dtpTuNgay.Value;
            currentFilter.DenNgay = dtpDenNgay.Value;
        }

        private async void BaoCaoXuatNhapTonChiTietForm_Load(object sender, EventArgs e)
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

        private async Task SearchWarehousesAsync(string searchText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    lstKho.Visible = false;
                    return;
                }

                // Search warehouses từ database
                var warehouses = await bll.SearchWarehousesAsync(searchText);
                
                lstKho.Items.Clear();
                
                if (warehouses.Count > 0)
                {
                    foreach (var warehouse in warehouses)
                    {
                        lstKho.Items.Add(warehouse.Name);
                    }
                    
                    lstKho.Visible = true;
                    lstKho.BringToFront();
                }
                else
                {
                    lstKho.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm kho: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lstKho.Visible = false;
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
                
                // Update filter
                currentFilter.TuNgay = dtpTuNgay.Value.Date;
                currentFilter.DenNgay = dtpDenNgay.Value.Date;
                currentFilter.TenKho = txtTenKho.Text.Trim();
                currentFilter.MaVatTu = txtMaVatTu.Text.Trim();
                
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
            try
            {
                if (currentData == null || !currentData.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetControlsEnabled(false);
                await bll.ExportToExcelAsync(currentData, currentFilter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            dtpTuNgay.Enabled = enabled;
            dtpDenNgay.Enabled = enabled;
            txtTenKho.Enabled = enabled;
            txtMaVatTu.Enabled = enabled;
            btnTimKiem.Enabled = enabled;
            btnExportExcel.Enabled = enabled && currentData.Count > 0;
        }

        #region Warehouse TextBox and ListBox Events

        private void TxtTenKho_TextChanged(object sender, EventArgs e)
        {
            if (isInitializing || isSettingText) return;

            // Stop previous timer
            searchTimer.Stop();
            
            string searchText = txtTenKho.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                lstKho.Visible = false;
                return;
            }

            // Start timer - sẽ search sau 1 giây
            searchTimer.Start();
        }

        private async void SearchTimer_Tick(object sender, EventArgs e)
        {
            // Stop timer
            searchTimer.Stop();
            
            // Search warehouses
            await SearchWarehousesAsync(txtTenKho.Text.Trim());
        }

        private void TxtTenKho_Leave(object sender, EventArgs e)
        {
            // Hide list after a short delay to allow clicking
            Task.Delay(200).ContinueWith(_ => {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => lstKho.Visible = false));
                }
                else
                {
                    lstKho.Visible = false;
                }
            });
        }

        private void LstKho_Click(object sender, EventArgs e)
        {
            if (lstKho.SelectedItem != null)
            {
                isSettingText = true;
                txtTenKho.Text = lstKho.SelectedItem.ToString();
                lstKho.Visible = false;
                isSettingText = false;
            }
        }

        private void CheckAllFieldsFilled(object sender, EventArgs e)
        {
            if (isInitializing) return;

            // Check if all required fields are filled
            bool allFilled = !string.IsNullOrWhiteSpace(txtTenKho.Text) &&
                           !string.IsNullOrWhiteSpace(txtMaVatTu.Text);

            // Enable button if all fields filled, but don't auto-run
            btnTimKiem.Enabled = allFilled;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                searchTimer?.Stop();
                searchTimer?.Dispose();
                
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private IContainer components = null;
    }
}
