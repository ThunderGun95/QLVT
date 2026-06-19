using System.ComponentModel;
using QLVT.BLL;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class NhapKhoUserControl : UserControl
    {
        private const int PagePadding = 18;

        private NhapKhoManualBLL nhapKhoBLL;
        private PhieuNhapKho? currentPhieu;
        private BindingList<PhieuNhapKhoChiTiet> chiTietList;
        private int selectedWarehouseId = -1;

        public NhapKhoUserControl()
        {
            InitializeComponent();
            ApplyModernStyle();
            nhapKhoBLL = new NhapKhoManualBLL();
            chiTietList = new BindingList<PhieuNhapKhoChiTiet>();
            InitializeData();
            SetupEvents();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "NHẬP KHO";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);
            lblTitle.AutoSize = false;

            grpThongTinPhieu.Text = "Thông tin phiếu nhập";
            grpChiTiet.Text = "Chi tiết phiếu nhập";
            UIStyleHelper.ApplyGroupBoxStyle(grpThongTinPhieu);
            UIStyleHelper.ApplyGroupBoxStyle(grpChiTiet);

            lblKhoNhap.Text = "Kho nhập";
            lblNgayGhiNhan.Text = "Ngày ghi nhận";
            lblGhiChu.Text = "Ghi chú";
            foreach (var label in new[] { lblKhoNhap, lblNgayGhiNhan, lblGhiChu })
            {
                UIStyleHelper.ApplyStandardLabelStyle(label);
            }

            UIStyleHelper.ApplyComboBoxStyle(cboKhoNhap);
            UIStyleHelper.ApplyTextBoxStyle(txtGhiChu);
            txtGhiChu.Multiline = true;
            txtGhiChu.ScrollBars = ScrollBars.Vertical;

            dtpNgayGhiNhan.Font = UIFonts.TextStandard;
            dtpNgayGhiNhan.CalendarFont = UIFonts.TextStandard;

            btnThemVatTu.Text = "Thêm vật tư";
            btnTaoMoi.Text = "Tạo mới";
            btnLuu.Text = "Lưu phiếu";
            btnIn.Text = "In phiếu";
            UIStyleHelper.ApplyPrimaryButtonStyle(btnThemVatTu, new Size(118, 34));
            UIStyleHelper.ApplySecondaryButtonStyle(btnTaoMoi, new Size(106, 34));
            UIStyleHelper.ApplySuccessButtonStyle(btnLuu, new Size(112, 34));
            UIStyleHelper.ApplySecondaryButtonStyle(btnIn, new Size(100, 34));

            pnlButtons.BackColor = UIColorPalette.BackgroundLight;
            pnlButtons.Dock = DockStyle.None;
            UIStyleHelper.ApplyDataGridViewStyle(dgvChiTietPhieu);

            Resize += (_, _) => LayoutModern();
            LayoutModern();
        }

        private void LayoutModern()
        {
            SuspendLayout();

            var contentTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;
            var contentWidth = Math.Max(620, Width - PagePadding * 2);

            grpThongTinPhieu.Location = new Point(PagePadding, contentTop);
            grpThongTinPhieu.Size = new Size(contentWidth, 124);

            lblKhoNhap.Location = new Point(18, 32);
            cboKhoNhap.Location = new Point(126, 28);
            cboKhoNhap.Size = new Size(Math.Min(320, Math.Max(210, grpThongTinPhieu.ClientSize.Width / 3)), 25);

            lblNgayGhiNhan.Location = new Point(Math.Max(360, grpThongTinPhieu.ClientSize.Width / 2), 32);
            dtpNgayGhiNhan.Location = new Point(lblNgayGhiNhan.Right + 14, 28);
            dtpNgayGhiNhan.Size = new Size(150, 25);

            lblGhiChu.Location = new Point(18, 70);
            txtGhiChu.Location = new Point(126, 66);
            txtGhiChu.Size = new Size(Math.Max(420, grpThongTinPhieu.ClientSize.Width - 144), 44);

            grpChiTiet.Location = new Point(PagePadding, grpThongTinPhieu.Bottom + 10);
            grpChiTiet.Size = new Size(contentWidth, Math.Max(260, Height - grpThongTinPhieu.Bottom - 82));

            btnThemVatTu.Location = new Point(16, 24);
            dgvChiTietPhieu.Location = new Point(16, 66);
            dgvChiTietPhieu.Size = new Size(grpChiTiet.ClientSize.Width - 32, grpChiTiet.ClientSize.Height - 82);

            pnlButtons.Location = new Point(PagePadding, Height - 56);
            pnlButtons.Size = new Size(contentWidth, 42);
            btnIn.Location = new Point(0, 4);
            btnTaoMoi.Location = new Point(pnlButtons.Width - btnTaoMoi.Width - btnLuu.Width - 12, 4);
            btnLuu.Location = new Point(pnlButtons.Width - btnLuu.Width, 4);

            ResumeLayout(false);
        }

        private void InitializeData()
        {
            try
            {
                // Load warehouses
                LoadWarehouses();
                
                // Initialize form
                ResetForm();
                SetupDataGridView();
                GenerateNewPhieuNumber();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWarehouses()
        {
            try
            {
                var warehouses = nhapKhoBLL.GetDanhSachKho();
                cboKhoNhap.Items.Clear();
                cboKhoNhap.DisplayMember = "TenKho";
                cboKhoNhap.ValueMember = "Id";
                cboKhoNhap.DataSource = warehouses;
                
                if (warehouses.Count > 0)
                {
                    selectedWarehouseId = warehouses[0].Id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupEvents()
        {
            // Detail grid events
            dgvChiTietPhieu.CellDoubleClick += DgvChiTietPhieu_CellDoubleClick;
            dgvChiTietPhieu.KeyDown += DgvChiTietPhieu_KeyDown;
            dgvChiTietPhieu.CellValueChanged += DgvChiTietPhieu_CellValueChanged;
            dgvChiTietPhieu.CellClick += DgvChiTietPhieu_CellClick;
            dgvChiTietPhieu.CellValidating += DgvChiTietPhieu_CellValidating;
            
            // ComboBox events
            cboKhoNhap.SelectedIndexChanged += CboKhoNhap_SelectedIndexChanged;
            
            // Button events
            btnLuu.Click += BtnLuu_Click;
            btnTaoMoi.Click += BtnTaoMoi_Click;
            btnThemVatTu.Click += BtnThemVatTu_Click;
            
            // Date picker events
            dtpNgayGhiNhan.ValueChanged += DtpNgayGhiNhan_ValueChanged;
        }

        private void SetupDataGridView()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvChiTietPhieu);
            dgvChiTietPhieu.AutoGenerateColumns = false;
            dgvChiTietPhieu.Columns.Clear();
            dgvChiTietPhieu.AllowUserToAddRows = false;
            dgvChiTietPhieu.AllowUserToDeleteRows = true;
            dgvChiTietPhieu.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvChiTietPhieu.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dgvChiTietPhieu.MultiSelect = false;

            // Mã vật tư
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaVatTu",
                HeaderText = "Mã VT",
                DataPropertyName = "MaVatTu",
                Width = 100,
                ReadOnly = true
            });

            // Tên vật tư
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                Width = 400,
                ReadOnly = true
            });

            // Đơn vị tính
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                Width = 80,
                ReadOnly = true
            });

            // Số lượng - editable, decimal type
            var soLuongColumn = new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "SL nhập",
                DataPropertyName = "SoLuong",
                Width = 120,
                ReadOnly = false
            };
            soLuongColumn.DefaultCellStyle.Format = "N2";
            soLuongColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvChiTietPhieu.Columns.Add(soLuongColumn);

            // Cột nút xóa
            var deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "Xóa",
                Text = "✖", // Dấu X đẹp
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            deleteColumn.DefaultCellStyle.BackColor = Color.FromArgb(220, 53, 69); // Màu đỏ
            deleteColumn.DefaultCellStyle.ForeColor = Color.White;
            deleteColumn.DefaultCellStyle.Font = new Font("Arial", 10F, FontStyle.Bold);
            dgvChiTietPhieu.Columns.Add(deleteColumn);

            dgvChiTietPhieu.Columns["MaVatTu"].HeaderText = "Mã VT";
            dgvChiTietPhieu.Columns["MaVatTu"].FillWeight = 12;
            dgvChiTietPhieu.Columns["TenVatTu"].HeaderText = "Tên vật tư";
            dgvChiTietPhieu.Columns["TenVatTu"].FillWeight = 48;
            dgvChiTietPhieu.Columns["DonViTinh"].HeaderText = "ĐVT";
            dgvChiTietPhieu.Columns["DonViTinh"].FillWeight = 9;
            dgvChiTietPhieu.Columns["DonViTinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvChiTietPhieu.Columns["SoLuong"].HeaderText = "SL nhập";
            dgvChiTietPhieu.Columns["SoLuong"].FillWeight = 12;
            dgvChiTietPhieu.Columns["Delete"].HeaderText = "Xóa";
            dgvChiTietPhieu.Columns["Delete"].FillWeight = 8;
            ((DataGridViewButtonColumn)dgvChiTietPhieu.Columns["Delete"]).Text = "Xóa";
            dgvChiTietPhieu.Columns["Delete"].DefaultCellStyle.BackColor = UIColorPalette.DangerRed;
            dgvChiTietPhieu.Columns["Delete"].DefaultCellStyle.ForeColor = Color.White;
            dgvChiTietPhieu.Columns["Delete"].DefaultCellStyle.Font = UIFonts.ButtonStandard;

            LoadTransactionDetails();
        }

        private void LoadTransactionDetails()
        {
            // BindingList automatically updates DataGridView
            if (dgvChiTietPhieu.DataSource == null)
            {
                dgvChiTietPhieu.DataSource = chiTietList;
            }
            CalculateTotal();
        }

        private void GenerateNewPhieuNumber()
        {
            try
            {
                // Will generate when saving - following XuatKho pattern
                currentPhieu = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo mã phiếu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            chiTietList.Clear();
            txtGhiChu.Text = "";
            dtpNgayGhiNhan.Value = DateTime.Now;
            currentPhieu = null;
            LoadTransactionDetails();
        }

        private void CalculateTotal()
        {
            // Count total items and quantity
            int totalItems = chiTietList.Count;
            decimal totalQuantity = chiTietList.Sum(d => d.SoLuong);
            
            // Update title to show totals
            this.Text = $"NHẬP KHO - {totalItems} mặt hàng, tổng SL: {totalQuantity:N2}";
        }

        // Event handlers
        private void BtnThemVatTu_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var searchForm = new TimVatTuForm())
                {
                    if (searchForm.ShowDialog() == DialogResult.OK)
                    {
                        var selectedSupply = searchForm.SelectedSupply;
                        if (selectedSupply != null)
                        {
                            // Add to chi tiet list
                            var existingDetail = chiTietList.FirstOrDefault(d => d.ErpId == selectedSupply.ErpId);
                            
                            if (existingDetail != null)
                            {
                                existingDetail.SoLuong += 1;
                            }
                            else
                            {
                                var newDetail = new PhieuNhapKhoChiTiet
                                {
                                    ErpId = selectedSupply.ErpId ?? 0,
                                    MaVatTu = selectedSupply.Code,
                                    TenVatTu = selectedSupply.TenVatTu,
                                    DonViTinh = selectedSupply.TenDVT ?? "",
                                    SoLuong = 1m
                                };
                                chiTietList.Add(newDetail);
                            }
                            
                            CalculateTotal();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm vật tư: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvChiTietPhieu_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.ColumnIndex < dgvChiTietPhieu.Columns.Count)
            {
                var column = dgvChiTietPhieu.Columns[e.ColumnIndex];
                // Allow editing quantity column
                if (column.Name == "SoLuong")
                {
                    dgvChiTietPhieu.BeginEdit(true);
                }
            }
        }

        private void DgvChiTietPhieu_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dgvChiTietPhieu.CurrentRow != null)
            {
                var result = MessageBox.Show("Bạn có chắc muốn xóa dòng này?", "Xác nhận", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    int index = dgvChiTietPhieu.CurrentRow.Index;
                    if (index < chiTietList.Count)
                    {
                        chiTietList.RemoveAt(index);
                        CalculateTotal();
                    }
                }
            }
        }

        private void DgvChiTietPhieu_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Handle delete button click
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var column = dgvChiTietPhieu.Columns[e.ColumnIndex];
                if (column.Name == "Delete")
                {
                    var result = MessageBox.Show("Bạn có chắc muốn xóa dòng này?", "Xác nhận", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        if (e.RowIndex < chiTietList.Count)
                        {
                            chiTietList.RemoveAt(e.RowIndex);
                            CalculateTotal();
                        }
                    }
                }
            }
        }

        private void DgvChiTietPhieu_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            // Validate quantity column
            if (e.ColumnIndex >= 0 && e.ColumnIndex < dgvChiTietPhieu.Columns.Count)
            {
                var column = dgvChiTietPhieu.Columns[e.ColumnIndex];
                if (column.Name == "SoLuong")
                {
                    if (!decimal.TryParse(e.FormattedValue?.ToString(), out decimal soLuong) || soLuong <= 0)
                    {
                        e.Cancel = true;
                        MessageBox.Show("Số lượng phải là số dương!", "Lỗi nhập liệu", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void DgvChiTietPhieu_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < chiTietList.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dgvChiTietPhieu.Columns.Count)
            {
                var detail = chiTietList[e.RowIndex];
                var columnName = dgvChiTietPhieu.Columns[e.ColumnIndex].Name;
                
                // Update quantity when changed
                if (columnName == "SoLuong")
                {
                    if (decimal.TryParse(dgvChiTietPhieu[e.ColumnIndex, e.RowIndex].Value?.ToString(), out decimal newQuantity))
                    {
                        detail.SoLuong = newQuantity;
                        CalculateTotal();
                    }
                }
            }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                // Tạo Transaction và TransactionDetail
                int transactionId = TaoNhapKhoTransaction();
                
                if (transactionId > 0)
                {
                    MessageBox.Show($"Lưu phiếu nhập thành công! Mã giao dịch: {transactionId}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Reset form for new transaction
                    ResetForm();
                    // GenerateNewPhieuNumber();
                }
                else
                {
                    MessageBox.Show("Lỗi lưu phiếu nhập!", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu phiếu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tạo Transaction và TransactionDetail cho nhập kho
        /// </summary>
        private int TaoNhapKhoTransaction()
        {
            try
            {
                if (cboKhoNhap.SelectedValue == null)
                {
                    throw new Exception($"Bạn chưa chọn kho nhập");
                }
                else
                {
                    // Tạo Transaction object
                    var transaction = new PhieuNhapKho
                    {
                        LoaiGiaoDich = "NhapKho",
                        NgayGiaoDich = dtpNgayGhiNhan.Value,
                        MaKhoNhan = (int)cboKhoNhap.SelectedValue,
                        GhiChu = txtGhiChu.Text.Trim()
                    };

                    // Tạo TransactionDetail list
                    var transactionDetails = new List<PhieuNhapKhoChiTiet>();
                    foreach (var chiTiet in chiTietList)
                    {
                        var detail = new PhieuNhapKhoChiTiet
                        {
                            ErpId = chiTiet.ErpId,
                            SoLuong = chiTiet.SoLuong
                        };
                        transactionDetails.Add(detail);
                    }

                    // Lưu transaction qua BLL
                    bool success = nhapKhoBLL.SaveTransaction(transaction, transactionDetails, selectedWarehouseId);
                    return success ? 1 : 0; // Return dummy transaction ID
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tạo transaction: {ex.Message}", ex);
            }
        }

        private void BtnTaoMoi_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn tạo phiếu mới?\nDữ liệu hiện tại sẽ bị xóa.", 
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                ResetForm();
                GenerateNewPhieuNumber();
            }
        }

        private void DtpNgayGhiNhan_ValueChanged(object? sender, EventArgs e)
        {
            // Handle date change if needed
        }

        private bool ValidateForm()
        {
            if (selectedWarehouseId <= 0)
            {
                MessageBox.Show("Vui lòng chọn kho nhập!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (chiTietList.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một vật tư!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate all details have quantity
            foreach (var detail in chiTietList)
            {
                if (detail.SoLuong <= 0)
                {
                    MessageBox.Show($"Số lượng của vật tư {detail.MaVatTu} phải lớn hơn 0!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (detail.ErpId <= 0)
                {
                    MessageBox.Show($"Vật tư {detail.MaVatTu} không hợp lệ!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void CboKhoNhap_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                if (cboKhoNhap.SelectedValue != null && int.TryParse(cboKhoNhap.SelectedValue.ToString(), out int warehouseId))
                {
                    selectedWarehouseId = warehouseId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
