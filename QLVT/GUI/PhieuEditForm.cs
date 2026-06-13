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

namespace QLVT.GUI
{
    /// <summary>
    /// Class để bind data cho DataGridView
    /// </summary>
    public class TransactionDetailDisplayItem
    {
        public int STT { get; set; }
        public int TransactionDetailID { get; set; }
        public string MaVatTu { get; set; } = "";
        public string TenVatTu { get; set; } = "";
        public string DVT { get; set; } = "";
        public decimal SoLuong { get; set; }
        public string GhiChu { get; set; } = "";
    }

    /// <summary>
    /// Form chỉnh sửa phiếu xuất/nhập/trả/hoàn ứng
    /// Cho phép chỉnh sửa sau khi ấn F2
    /// </summary>
    public partial class PhieuEditForm : Form
    {
        private readonly TransactionDAL _transactionDAL;
        private readonly WarehouseDAL _warehouseDAL;
        private readonly SupplyDAL _supplyDAL;
        
        private Transaction _currentTransaction;
        private List<TransactionDetail> _currentDetails;
        private List<Warehouse> _warehouses;
        private bool _isEditMode = false;
        
        // Controls
        private Label lblSoPhieu;
        private TextBox txtSoPhieu;
        private Label lblLoaiGiaoDich;
        private ComboBox cboLoaiGiaoDich;
        private Label lblNgayGiaoDich;
        private DateTimePicker dtpNgayGiaoDich;
        private Label lblKhoNguon;
        private ComboBox cboKhoNguon;
        private Label lblKhoNhan;
        private ComboBox cboKhoNhan;
        private Label lblNguoiThucHien;
        private TextBox txtNguoiThucHien;
        private Label lblGhiChu;
        private TextBox txtGhiChu;
        private DataGridView dgvChiTiet;
        private Button btnEdit;
        private Button btnSave;
        private Button btnCancel;
        private Button btnAddItem;
        private Button btnRemoveItem;
        private Label lblStatus;

        public PhieuEditForm(int transactionId)
        {
            _transactionDAL = new TransactionDAL();
            _warehouseDAL = new WarehouseDAL();
            _supplyDAL = new SupplyDAL();
            
            InitializeComponent();
            SetupForm();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            _ = LoadTransactionAsync(transactionId);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Chỉnh sửa phiếu";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            this.KeyPreview = true;
            
            InitializeControls();
            SetupLayout();
            SetupEventHandlers();
            
            this.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            // Labels
            lblSoPhieu = new Label { Text = "Số phiếu:", AutoSize = true, Location = new Point(20, 20) };
            lblLoaiGiaoDich = new Label { Text = "Loại giao dịch:", AutoSize = true, Location = new Point(20, 55) };
            lblNgayGiaoDich = new Label { Text = "Ngày giao dịch:", AutoSize = true, Location = new Point(300, 55) };
            lblKhoNguon = new Label { Text = "Kho nguồn:", AutoSize = true, Location = new Point(20, 90) };
            lblKhoNhan = new Label { Text = "Kho nhận:", AutoSize = true, Location = new Point(300, 90) };
            lblNguoiThucHien = new Label { Text = "Người thực hiện:", AutoSize = true, Location = new Point(20, 125) };
            lblGhiChu = new Label { Text = "Ghi chú:", AutoSize = true, Location = new Point(20, 160) };

            // Input controls
            txtSoPhieu = new TextBox 
            { 
                Location = new Point(120, 17), 
                Size = new Size(150, 25), 
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            cboLoaiGiaoDich = new ComboBox 
            { 
                Location = new Point(120, 52), 
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            cboLoaiGiaoDich.Items.AddRange(new[] { "NhapKho", "XuatKho", "TraKho", "HoanUng" });

            dtpNgayGiaoDich = new DateTimePicker 
            { 
                Location = new Point(420, 52), 
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                Enabled = false
            };

            cboKhoNguon = new ComboBox 
            { 
                Location = new Point(120, 87), 
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            cboKhoNhan = new ComboBox 
            { 
                Location = new Point(420, 87), 
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            txtNguoiThucHien = new TextBox 
            { 
                Location = new Point(120, 122), 
                Size = new Size(200, 25),
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            txtGhiChu = new TextBox 
            { 
                Location = new Point(120, 157), 
                Size = new Size(400, 25),
                ReadOnly = true,
                BackColor = SystemColors.Control
            };

            // DataGridView
            dgvChiTiet = new DataGridView 
            {
                Location = new Point(20, 200),
                Size = new Size(940, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Buttons
            btnEdit = new Button 
            { 
                Text = "Chỉnh sửa (F2)", 
                Location = new Point(20, 620), 
                Size = new Size(120, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnSave = new Button 
            { 
                Text = "Lưu", 
                Location = new Point(150, 620), 
                Size = new Size(80, 30),
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnCancel = new Button 
            { 
                Text = "Hủy", 
                Location = new Point(240, 620), 
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnAddItem = new Button 
            { 
                Text = "Thêm vật tư", 
                Location = new Point(340, 620), 
                Size = new Size(100, 30),
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            btnRemoveItem = new Button 
            { 
                Text = "Xóa vật tư", 
                Location = new Point(450, 620), 
                Size = new Size(100, 30),
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            // Status label
            lblStatus = new Label 
            { 
                Text = "Chế độ xem. Ấn F2 để chỉnh sửa.", 
                Location = new Point(570, 625), 
                Size = new Size(300, 25),
                ForeColor = Color.Blue,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            // Add all controls to form
            this.Controls.AddRange(new Control[]
            {
                lblSoPhieu, txtSoPhieu, lblLoaiGiaoDich, cboLoaiGiaoDich,
                lblNgayGiaoDich, dtpNgayGiaoDich, lblKhoNguon, cboKhoNguon,
                lblKhoNhan, cboKhoNhan, lblNguoiThucHien, txtNguoiThucHien,
                lblGhiChu, txtGhiChu, dgvChiTiet, btnEdit, btnSave, btnCancel, 
                btnAddItem, btnRemoveItem, lblStatus
            });
        }

        private void SetupLayout()
        {
            SetupDataGridViewColumns();
        }

        private void SetupDataGridViewColumns()
        {
            dgvChiTiet.Columns.Clear();

            var columns = new[]
            {
                new { Name = "STT", Header = "STT", Width = 50, IsReadOnly = true },
                new { Name = "MaVatTu", Header = "Mã vật tư", Width = 100, IsReadOnly = true },
                new { Name = "TenVatTu", Header = "Tên vật tư", Width = 250, IsReadOnly = true },
                new { Name = "DVT", Header = "ĐVT", Width = 60, IsReadOnly = true },
                new { Name = "SoLuong", Header = "Số lượng", Width = 100, IsReadOnly = false },
                new { Name = "GhiChu", Header = "Ghi chú", Width = 200, IsReadOnly = false }
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    DataPropertyName = col.Name,
                    HeaderText = col.Header,
                    Width = col.Width,
                    ReadOnly = col.IsReadOnly // Don't make it dependent on _isEditMode initially
                };

                if (col.Name == "SoLuong")
                {
                    column.DefaultCellStyle.Format = "N2";
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (col.Name == "STT")
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                dgvChiTiet.Columns.Add(column);
            }

            // Configure DataGridView for better editing experience
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.EditMode = DataGridViewEditMode.EditOnEnter; // Allow immediate editing on single click
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.CellSelect; // Allow cell selection for editing
            dgvChiTiet.MultiSelect = false;
        }

        private void SetupEventHandlers()
        {
            btnEdit.Click += BtnEdit_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            btnAddItem.Click += BtnAddItem_Click;
            btnRemoveItem.Click += BtnRemoveItem_Click;
            dgvChiTiet.CellValueChanged += DgvChiTiet_CellValueChanged;
            dgvChiTiet.CurrentCellDirtyStateChanged += DgvChiTiet_CurrentCellDirtyStateChanged;
            dgvChiTiet.CellBeginEdit += DgvChiTiet_CellBeginEdit;
            this.KeyDown += PhieuEditForm_KeyDown;
        }

        private void SetupForm()
        {
            
        }

        private async Task LoadTransactionAsync(int transactionId)
        {
            try
            {
                lblStatus.Text = "Đang tải dữ liệu...";
                lblStatus.ForeColor = Color.Orange;

                // Load transaction
                _currentTransaction = await _transactionDAL.GetTransactionByIdAsync(transactionId);
                if (_currentTransaction == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Load transaction details
                _currentDetails = await _transactionDAL.GetTransactionDetailsAsync(transactionId);

                // Load warehouses
                _warehouses = await _warehouseDAL.GetAllWarehousesAsync();

                // Populate form
                PopulateForm();

                lblStatus.Text = "Chế độ xem. Ấn F2 để chỉnh sửa.";
                lblStatus.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi tải dữ liệu";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void PopulateForm()
        {
            if (_currentTransaction == null) return;

            // Populate transaction info
            txtSoPhieu.Text = _currentTransaction.SoPhieu;
            cboLoaiGiaoDich.SelectedItem = _currentTransaction.LoaiGiaoDich;
            dtpNgayGiaoDich.Value = _currentTransaction.NgayGiaoDich;
            txtNguoiThucHien.Text = _currentTransaction.NguoiThucHien;
            txtGhiChu.Text = _currentTransaction.GhiChu ?? "";

            // Populate warehouses
            PopulateWarehouses();

            // Set selected warehouses
            if (_currentTransaction.MaKhoNguon.HasValue)
            {
                var khoNguon = _warehouses.FirstOrDefault(w => w.Id == _currentTransaction.MaKhoNguon.Value);
                if (khoNguon != null)
                {
                    cboKhoNguon.SelectedItem = khoNguon;
                }
            }

            if (_currentTransaction.MaKhoNhan.HasValue)
            {
                var khoNhan = _warehouses.FirstOrDefault(w => w.Id == _currentTransaction.MaKhoNhan.Value);
                if (khoNhan != null)
                {
                    cboKhoNhan.SelectedItem = khoNhan;
                }
            }

            // Populate details
            PopulateDetails();
        }

        private void PopulateWarehouses()
        {
            cboKhoNguon.DataSource = new List<Warehouse>(_warehouses);
            cboKhoNguon.DisplayMember = "TenKho";
            cboKhoNguon.ValueMember = "Id";

            cboKhoNhan.DataSource = new List<Warehouse>(_warehouses);
            cboKhoNhan.DisplayMember = "TenKho";
            cboKhoNhan.ValueMember = "Id";
        }

        private async void PopulateDetails()
        {
            if (_currentDetails == null) return;

            var detailsWithSupplyInfo = new List<TransactionDetailDisplayItem>();
            int stt = 1;

            foreach (var detail in _currentDetails)
            {
                try
                {
                    var supply = await _supplyDAL.GetSupplyByErpIdAsync(detail.ErpID);
                    detailsWithSupplyInfo.Add(new TransactionDetailDisplayItem
                    {
                        STT = stt++,
                        TransactionDetailID = detail.TransactionDetailID,
                        MaVatTu = supply?.Code ?? $"ERP-{detail.ErpID}",
                        TenVatTu = supply?.TenVatTu ?? "Không tìm thấy",
                        DVT = supply?.DonViTinh ?? "",
                        SoLuong = detail.SoLuong,
                        GhiChu = detail.GhiChu ?? ""
                    });
                }
                catch
                {
                    // Fallback nếu không tìm thấy supply
                    detailsWithSupplyInfo.Add(new TransactionDetailDisplayItem
                    {
                        STT = stt++,
                        TransactionDetailID = detail.TransactionDetailID,
                        MaVatTu = $"ERP-{detail.ErpID}",
                        TenVatTu = "Không tìm thấy",
                        DVT = "",
                        SoLuong = detail.SoLuong,
                        GhiChu = detail.GhiChu ?? ""
                    });
                }
            }

            dgvChiTiet.DataSource = detailsWithSupplyInfo;
        }

        #region Event Handlers

        private void PhieuEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                ToggleEditMode();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape && _isEditMode)
            {
                CancelEdit();
                e.Handled = true;
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            ToggleEditMode();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            await SaveChangesAsync();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                CancelEdit();
            }
            else
            {
                this.Close();
            }
        }

        private async void BtnAddItem_Click(object sender, EventArgs e)
        {
            await AddNewItemAsync();
        }

        private void BtnRemoveItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedItem();
        }

        private void DgvChiTiet_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Only allow editing if in edit mode and on editable columns
            if (!_isEditMode)
            {
                e.Cancel = true;
                return;
            }

            var grid = sender as DataGridView;
            var columnName = grid?.Columns[e.ColumnIndex].Name;
            
            if (columnName != "SoLuong" && columnName != "GhiChu")
            {
                e.Cancel = true; // Cancel editing for readonly columns
            }
        }

        private void DgvChiTiet_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvChiTiet.IsCurrentCellDirty)
            {
                dgvChiTiet.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvChiTiet_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isEditMode) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = sender as DataGridView;
            if (grid?.Rows[e.RowIndex].DataBoundItem is TransactionDetailDisplayItem item)
            {
                // Validate số lượng nếu cột đang edit là SoLuong
                if (e.ColumnIndex == grid.Columns["SoLuong"].Index)
                {
                    var currentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    
                    // Try to parse the new value
                    if (decimal.TryParse(currentCell.Value?.ToString(), out decimal newSoLuong))
                    {
                        if (newSoLuong <= 0)
                        {
                            MessageBox.Show("Số lượng phải lớn hơn 0!", "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            currentCell.Value = 1; // Reset về giá trị mặc định
                            item.SoLuong = 1; // Update the bound item as well
                        }
                        else
                        {
                            item.SoLuong = newSoLuong; // Update the bound item
                        }
                    }
                    else
                    {
                        // Invalid input, reset to 1
                        MessageBox.Show("Số lượng phải là số hợp lệ!", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        currentCell.Value = 1;
                        item.SoLuong = 1;
                    }
                }
                // Handle GhiChu changes
                else if (e.ColumnIndex == grid.Columns["GhiChu"].Index)
                {
                    var currentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    item.GhiChu = currentCell.Value?.ToString() ?? "";
                }
            }
        }

        #endregion

        #region Edit Mode Management

        private void ToggleEditMode()
        {
            _isEditMode = !_isEditMode;
            SetEditMode(_isEditMode);
        }

        private void SetEditMode(bool editMode)
        {
            _isEditMode = editMode;

            // Enable/disable controls - KHÔNG cho phép chỉnh sửa loại phiếu
            // cboLoaiGiaoDich.Enabled = editMode; // Bỏ dòng này để không cho chỉnh sửa loại phiếu
            dtpNgayGiaoDich.Enabled = editMode;
            cboKhoNguon.Enabled = editMode;
            cboKhoNhan.Enabled = editMode;
            txtGhiChu.ReadOnly = !editMode;
            txtGhiChu.BackColor = editMode ? SystemColors.Window : SystemColors.Control;

            // DataGridView columns - chỉ enable những column có thể edit
            foreach (DataGridViewColumn col in dgvChiTiet.Columns)
            {
                if (col.Name == "SoLuong" || col.Name == "GhiChu")
                {
                    col.ReadOnly = !editMode;
                }
                // Các column khác luôn readonly
            }

            // DataGridView settings - không cho phép add/delete rows bằng grid
            dgvChiTiet.AllowUserToAddRows = false; // Always false, use buttons instead
            dgvChiTiet.AllowUserToDeleteRows = false; // Always false, use buttons instead
            
            // Set selection mode based on edit mode
            if (editMode)
            {
                dgvChiTiet.SelectionMode = DataGridViewSelectionMode.CellSelect; // Allow cell selection for editing
                dgvChiTiet.EditMode = DataGridViewEditMode.EditOnEnter; // Immediate editing
            }
            else
            {
                dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Row selection for view mode
                dgvChiTiet.EditMode = DataGridViewEditMode.EditProgrammatically; // No editing
            }

            // Buttons
            btnEdit.Text = editMode ? "Hủy chỉnh sửa" : "Chỉnh sửa (F2)";
            btnSave.Enabled = editMode;
            btnAddItem.Enabled = editMode;
            btnRemoveItem.Enabled = editMode;
            btnCancel.Text = editMode ? "Hủy" : "Đóng";

            // Status
            lblStatus.Text = editMode ? "Chế độ chỉnh sửa. Click vào ô số lượng để sửa. ESC để hủy." : "Chế độ xem. Ấn F2 để chỉnh sửa.";
            lblStatus.ForeColor = editMode ? Color.Red : Color.Blue;
        }

        private void CancelEdit()
        {
            if (_isEditMode)
            {
                // Restore original data
                PopulateForm();
                SetEditMode(false);
            }
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                if (!_isEditMode) return;

                lblStatus.Text = "Đang lưu...";
                lblStatus.ForeColor = Color.Orange;

                // Validate data
                if (!ValidateData())
                {
                    lblStatus.Text = "Dữ liệu không hợp lệ";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                // Update transaction object
                UpdateTransactionFromForm();

                // Save to database
                await _transactionDAL.UpdateTransactionAsync(_currentTransaction);
                await SaveTransactionDetailsAsync();

                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                SetEditMode(false);
                lblStatus.Text = "Đã lưu. Chế độ xem.";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi khi lưu";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private bool ValidateData()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtSoPhieu.Text))
            {
                MessageBox.Show("Số phiếu không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboLoaiGiaoDich.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại giao dịch!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate transaction details
            foreach (DataGridViewRow row in dgvChiTiet.Rows)
            {
                if (row.IsNewRow) continue;

                var soLuongCell = row.Cells["SoLuong"];
                if (soLuongCell.Value == null || !decimal.TryParse(soLuongCell.Value.ToString(), out decimal soLuong) || soLuong <= 0)
                {
                    MessageBox.Show($"Số lượng ở dòng {row.Index + 1} không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void UpdateTransactionFromForm()
        {
            _currentTransaction.LoaiGiaoDich = cboLoaiGiaoDich.SelectedItem?.ToString() ?? "";
            _currentTransaction.NgayGiaoDich = dtpNgayGiaoDich.Value;
            _currentTransaction.GhiChu = txtGhiChu.Text;

            if (cboKhoNguon.SelectedItem is Warehouse khoNguon)
                _currentTransaction.MaKhoNguon = khoNguon.Id;

            if (cboKhoNhan.SelectedItem is Warehouse khoNhan)
                _currentTransaction.MaKhoNhan = khoNhan.Id;
        }

        private async Task SaveTransactionDetailsAsync()
        {
            // Get current items in the grid
            var currentDisplayItems = dgvChiTiet.DataSource as List<TransactionDetailDisplayItem>;
            if (currentDisplayItems == null) return;

            // Cập nhật tất cả items trong grid vào _currentDetails
            foreach (var displayItem in currentDisplayItems)
            {
                var detail = _currentDetails.FirstOrDefault(d => d.TransactionDetailID == displayItem.TransactionDetailID);
                if (detail != null)
                {
                    // Cập nhật dữ liệu từ grid vào detail object
                    detail.SoLuong = displayItem.SoLuong;
                    detail.GhiChu = displayItem.GhiChu;
                }
            }

            // Lưu tất cả existing details
            foreach (var detail in _currentDetails.Where(d => d.TransactionDetailID > 0))
            {
                await _transactionDAL.UpdateTransactionDetailAsync(detail);
            }

            // Lưu new details (nếu có)
            foreach (var detail in _currentDetails.Where(d => d.TransactionDetailID == 0))
            {
                var newId = await _transactionDAL.AddTransactionDetailAsync(detail);
                detail.TransactionDetailID = newId; // Cập nhật ID sau khi thêm
            }

            // Xóa các details đã bị remove (nếu có)
            var currentDetailIds = currentDisplayItems.Where(d => d.TransactionDetailID > 0)
                                                     .Select(d => d.TransactionDetailID).ToList();
            var originalDetailIds = _currentDetails.Where(d => d.TransactionDetailID > 0)
                                                   .Select(d => d.TransactionDetailID).ToList();
            var detailsToDelete = originalDetailIds.Except(currentDetailIds).ToList();

            foreach (var detailId in detailsToDelete)
            {
                await _transactionDAL.DeleteTransactionDetailAsync(detailId);
                // Remove from _currentDetails list
                var detailToRemove = _currentDetails.FirstOrDefault(d => d.TransactionDetailID == detailId);
                if (detailToRemove != null)
                {
                    _currentDetails.Remove(detailToRemove);
                }
            }
        }

        #endregion

        #region Add/Remove Items

        private Task AddNewItemAsync()
        {
            try
            {
                if (!_isEditMode)
                    return Task.CompletedTask;
                using (var form = new SupplySelectionForm())
                {
                    if (form.ShowDialog(this) == DialogResult.OK && form.SelectedSupply != null)
                    {
                        // Check if supply already exists
                        var existingItems = dgvChiTiet.DataSource as List<TransactionDetailDisplayItem>;
                        if (existingItems != null && existingItems.Any(item => item.MaVatTu == form.SelectedSupply.Code))
                        {
                            MessageBox.Show("Vật tư này đã có trong phiếu!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return Task.CompletedTask;
                        }

                        // Create new transaction detail
                        var newDetail = new TransactionDetail
                        {
                            TransactionDetailID = 0, // Will be set when saved
                            TransactionID = _currentTransaction.TransactionID,
                            ErpID = form.SelectedSupply.ErpId!.Value,
                            SoLuong = form.Quantity,
                            GhiChu = form.Note
                        };

                        // Add to current details list
                        _currentDetails.Add(newDetail);

                        // Refresh the display
                        PopulateDetails();

                        lblStatus.Text = "Đã thêm vật tư mới. Nhớ lưu để áp dụng thay đổi.";
                        lblStatus.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm vật tư: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Task.CompletedTask;
        }

        private void RemoveSelectedItem()
        {
            try
            {
                if (!_isEditMode) return;

                if (dgvChiTiet.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn vật tư cần xóa!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedRow = dgvChiTiet.SelectedRows[0];
                var item = selectedRow.DataBoundItem as TransactionDetailDisplayItem;
                
                if (item == null) return;

                var result = MessageBox.Show($"Bạn có chắc muốn xóa vật tư '{item.TenVatTu}' khỏi phiếu?", 
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Remove from current details list
                    var detailToRemove = _currentDetails.FirstOrDefault(d => d.TransactionDetailID == item.TransactionDetailID);
                    if (detailToRemove != null)
                    {
                        _currentDetails.Remove(detailToRemove);
                    }

                    // Refresh the display
                    PopulateDetails();

                    lblStatus.Text = "Đã xóa vật tư. Nhớ lưu để áp dụng thay đổi.";
                    lblStatus.ForeColor = Color.Orange;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa vật tư: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose resources
            }
            base.Dispose(disposing);
        }
    }
}
