using QLVT.BLL;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public class WarehouseManagementForm : Form
    {
        private readonly WarehouseManagementBLL warehouseBLL = new();

        private readonly Label lblTitle = new();
        private readonly GroupBox grpEditor = new();
        private readonly Label lblMaKho = new();
        private readonly Label lblTenKho = new();
        private readonly Label lblLoaiKho = new();
        private readonly Label lblNhanVien = new();
        private readonly Label lblDiaChi = new();
        private readonly Label lblGhiChu = new();
        private readonly TextBox txtMaKho = new();
        private readonly TextBox txtTenKho = new();
        private readonly ComboBox cboLoaiKho = new();
        private readonly ComboBox cboNhanVien = new();
        private readonly TextBox txtDiaChi = new();
        private readonly TextBox txtGhiChu = new();
        private readonly CheckBox chkKhoUuTien = new();
        private readonly Button btnThemMoi = new();
        private readonly Button btnLuu = new();
        private readonly Button btnNgungHoatDong = new();
        private readonly Button btnDatUuTien = new();
        private readonly Button btnTaiLai = new();
        private readonly TextBox txtSearch = new();
        private readonly TabControl tabControl = new();
        private readonly TabPage tabCompany = new("Kho công ty");
        private readonly TabPage tabPersonal = new("Kho cá nhân");
        private readonly DataGridView dgvCompany = new();
        private readonly SplitContainer personalSplit = new();
        private readonly DataGridView dgvStaffGroups = new();
        private readonly DataGridView dgvPersonal = new();
        private readonly Label lblStatus = new();

        private List<Warehouse> warehouses = new();
        private List<Staff> staffs = new();
        private Warehouse? selectedWarehouse;
        private string? selectedStaffCode;

        public WarehouseManagementForm()
        {
            InitializeComponent();
            ApplyStyle();
            SetupGrids();
            BindEvents();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Quản lý kho";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(1080, 680);
            Size = new Size(1220, 760);

            Controls.Add(lblTitle);
            Controls.Add(grpEditor);
            Controls.Add(txtSearch);
            Controls.Add(tabControl);
            Controls.Add(lblStatus);

            grpEditor.Controls.AddRange(new Control[]
            {
                lblMaKho, txtMaKho,
                lblTenKho, txtTenKho,
                lblLoaiKho, cboLoaiKho,
                lblNhanVien, cboNhanVien,
                lblDiaChi, txtDiaChi,
                lblGhiChu, txtGhiChu,
                chkKhoUuTien,
                btnThemMoi, btnLuu, btnNgungHoatDong, btnDatUuTien, btnTaiLai
            });

            tabControl.TabPages.Add(tabCompany);
            tabControl.TabPages.Add(tabPersonal);
            tabCompany.Controls.Add(dgvCompany);
            tabPersonal.Controls.Add(personalSplit);
            personalSplit.Panel1.Controls.Add(dgvStaffGroups);
            personalSplit.Panel2.Controls.Add(dgvPersonal);
        }

        private void ApplyStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "QUẢN LÝ KHO";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            grpEditor.Text = "Thông tin kho";
            UIStyleHelper.ApplyGroupBoxStyle(grpEditor);

            foreach (var label in new[] { lblMaKho, lblTenKho, lblLoaiKho, lblNhanVien, lblDiaChi, lblGhiChu })
            {
                UIStyleHelper.ApplyStandardLabelStyle(label);
            }

            lblMaKho.Text = "Mã kho";
            lblTenKho.Text = "Tên kho";
            lblLoaiKho.Text = "Loại kho";
            lblNhanVien.Text = "Nhân viên";
            lblDiaChi.Text = "Địa chỉ";
            lblGhiChu.Text = "Ghi chú";

            UIStyleHelper.ApplyTextBoxStyle(txtMaKho);
            UIStyleHelper.ApplyTextBoxStyle(txtTenKho);
            UIStyleHelper.ApplyTextBoxStyle(txtDiaChi);
            UIStyleHelper.ApplyTextBoxStyle(txtGhiChu);
            UIStyleHelper.ApplyTextBoxStyle(txtSearch);
            UIStyleHelper.ApplyComboBoxStyle(cboLoaiKho);
            UIStyleHelper.ApplyComboBoxStyle(cboNhanVien);

            txtSearch.PlaceholderText = "Tìm mã kho, tên kho, nhân viên...";
            txtGhiChu.Multiline = false;
            txtGhiChu.ScrollBars = ScrollBars.None;

            cboLoaiKho.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLoaiKho.Items.AddRange(new object[] { "COMPANY", "CANHAN" });
            cboLoaiKho.SelectedIndex = 0;

            chkKhoUuTien.Text = "Kho ưu tiên";
            UIStyleHelper.ApplyCheckBoxStyle(chkKhoUuTien);

            UIStyleHelper.ApplyPrimaryButtonStyle(btnLuu, new Size(96, 34));
            UIStyleHelper.ApplySecondaryButtonStyle(btnThemMoi, new Size(96, 34));
            UIStyleHelper.ApplyDangerButtonStyle(btnNgungHoatDong, new Size(132, 34));
            UIStyleHelper.ApplySuccessButtonStyle(btnDatUuTien, new Size(126, 34));
            UIStyleHelper.ApplySecondaryButtonStyle(btnTaiLai, new Size(90, 34));

            btnThemMoi.Text = "Thêm mới";
            btnLuu.Text = "Lưu";
            btnNgungHoatDong.Text = "Ngưng HĐ";
            btnDatUuTien.Text = "Đặt ưu tiên";
            btnTaiLai.Text = "Tải lại";

            UIStyleHelper.ApplyDataGridViewStyle(dgvCompany);
            UIStyleHelper.ApplyDataGridViewStyle(dgvStaffGroups);
            UIStyleHelper.ApplyDataGridViewStyle(dgvPersonal);

            lblStatus.BackColor = UIColorPalette.BackgroundWhite;
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Padding = new Padding(10, 0, 10, 0);
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus);

            tabControl.Font = UIFonts.TextStandard;
            personalSplit.Dock = DockStyle.Fill;
            personalSplit.Orientation = Orientation.Horizontal;
            personalSplit.SplitterWidth = 6;
            personalSplit.Panel1MinSize = 170;
            personalSplit.Panel2MinSize = 150;

            Resize += (_, _) => LayoutForm();
            LayoutForm();
        }

        private void LayoutForm()
        {
            const int padding = 18;
            var contentTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;

            grpEditor.Location = new Point(padding, contentTop);
            grpEditor.Size = new Size(ClientSize.Width - padding * 2, 142);

            lblMaKho.Location = new Point(18, 32);
            txtMaKho.Location = new Point(82, 28);
            txtMaKho.Size = new Size(130, 25);

            lblTenKho.Location = new Point(230, 32);
            txtTenKho.Location = new Point(300, 28);
            txtTenKho.Size = new Size(260, 25);

            lblLoaiKho.Location = new Point(580, 32);
            cboLoaiKho.Location = new Point(650, 28);
            cboLoaiKho.Size = new Size(130, 25);

            lblNhanVien.Location = new Point(800, 32);
            cboNhanVien.Location = new Point(880, 28);
            cboNhanVien.Size = new Size(Math.Max(180, grpEditor.ClientSize.Width - 900), 25);

            lblDiaChi.Location = new Point(18, 70);
            txtDiaChi.Location = new Point(82, 66);
            txtDiaChi.Size = new Size(478, 25);

            lblGhiChu.Location = new Point(580, 70);
            txtGhiChu.Location = new Point(650, 66);
            txtGhiChu.Size = new Size(Math.Max(260, grpEditor.ClientSize.Width - 668), 25);

            chkKhoUuTien.Location = new Point(82, 104);
            chkKhoUuTien.Size = new Size(130, 24);

            btnThemMoi.Location = new Point(230, 100);
            btnLuu.Location = new Point(btnThemMoi.Right + 8, 100);
            btnDatUuTien.Location = new Point(btnLuu.Right + 8, 100);
            btnNgungHoatDong.Location = new Point(btnDatUuTien.Right + 8, 100);
            btnTaiLai.Location = new Point(btnNgungHoatDong.Right + 8, 100);

            txtSearch.Location = new Point(padding, grpEditor.Bottom + 10);
            txtSearch.Size = new Size(ClientSize.Width - padding * 2, 27);

            tabControl.Location = new Point(padding, txtSearch.Bottom + 10);
            tabControl.Size = new Size(ClientSize.Width - padding * 2, Math.Max(260, ClientSize.Height - txtSearch.Bottom - 60));

            dgvCompany.Dock = DockStyle.Fill;
            dgvStaffGroups.Dock = DockStyle.Fill;
            dgvPersonal.Dock = DockStyle.Fill;

            if (personalSplit.Height > 340)
            {
                personalSplit.SplitterDistance = Math.Min(
                    personalSplit.Height - personalSplit.Panel2MinSize,
                    Math.Max(personalSplit.Panel1MinSize, (int)(personalSplit.Height * 0.56)));
            }

            lblStatus.Location = new Point(padding, ClientSize.Height - 40);
            lblStatus.Size = new Size(ClientSize.Width - padding * 2, 26);
        }

        private void SetupGrids()
        {
            SetupCompanyWarehouseGrid();
            SetupPersonalWarehouseGrid();

            dgvStaffGroups.Columns.Clear();
            dgvStaffGroups.AutoGenerateColumns = false;
            dgvStaffGroups.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("MaNV", "Mã NV", "MaNV", 90, 12));
            dgvStaffGroups.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TenNV", "Nhân viên", "TenNV", 220, 28));
            dgvStaffGroups.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("SoKho", "Số kho", "SoKho", 70, 8));
            dgvStaffGroups.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("KhoUuTien", "Kho ưu tiên", "KhoUuTien", 360, 52));
        }

        private void SetupCompanyWarehouseGrid()
        {
            dgvCompany.Columns.Clear();
            dgvCompany.AutoGenerateColumns = false;
            dgvCompany.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("Id", "ID", "Id", 54, 6));
            dgvCompany.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("MaKho", "Mã kho", "MaKho", 90, 12));
            dgvCompany.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TenKho", "Tên kho", "TenKho", 320, 44));
            dgvCompany.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("LoaiKho", "Loại kho", "LoaiKho", 100, 14));
            dgvCompany.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("GhiChu", "Ghi chú", "GhiChu", 240, 24));
        }

        private void SetupPersonalWarehouseGrid()
        {
            dgvPersonal.Columns.Clear();
            dgvPersonal.AutoGenerateColumns = false;
            dgvPersonal.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("Id", "ID", "Id", 54, 6));
            dgvPersonal.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("MaKho", "Mã kho", "MaKho", 90, 12));
            dgvPersonal.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TenKho", "Tên kho cá nhân", "TenKho", 360, 54));
            dgvPersonal.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("KhoUuTienText", "Ưu tiên", "KhoUuTienText", 80, 10));
            dgvPersonal.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("GhiChu", "Ghi chú", "GhiChu", 180, 18));
        }

        private void BindEvents()
        {
            btnThemMoi.Click += (_, _) => ClearEditor();
            btnLuu.Click += (_, _) => SaveWarehouse();
            btnNgungHoatDong.Click += (_, _) => DeactivateSelectedWarehouse();
            btnDatUuTien.Click += (_, _) => SetPrioritySelectedWarehouse();
            btnTaiLai.Click += (_, _) => LoadData();
            txtSearch.TextChanged += (_, _) => BindData();
            cboLoaiKho.SelectedIndexChanged += (_, _) => UpdateEditorState();
            cboNhanVien.SelectedIndexChanged += (_, _) => SelectStaffFromCombo();
            dgvCompany.SelectionChanged += (_, _) => SelectWarehouseFromGrid(dgvCompany);
            dgvPersonal.SelectionChanged += (_, _) => SelectWarehouseFromGrid(dgvPersonal);
            dgvStaffGroups.SelectionChanged += (_, _) => SelectStaffGroup();
        }

        private void LoadData()
        {
            try
            {
                warehouses = warehouseBLL.GetWarehouses();
                staffs = warehouseBLL.GetStaffs();
                BindStaffCombo();
                BindData();
                ClearEditor();
                UpdateStatus($"Đã tải {warehouses.Count:N0} kho", StatusType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"Lỗi tải dữ liệu: {ex.Message}", StatusType.Error);
            }
        }

        private void BindStaffCombo()
        {
            var items = staffs
                .OrderBy(x => x.TenNV)
                .Select(x => new StaffComboItem(x.MaNV, $"{x.MaNV} - {x.TenNV}"))
                .ToList();

            cboNhanVien.DataSource = items;
            cboNhanVien.DisplayMember = nameof(StaffComboItem.Display);
            cboNhanVien.ValueMember = nameof(StaffComboItem.MaNV);
            cboNhanVien.SelectedIndex = -1;
        }

        private void BindData()
        {
            var keyword = txtSearch.Text.Trim();
            var filtered = string.IsNullOrWhiteSpace(keyword)
                ? warehouses
                : warehouses.Where(w =>
                    Contains(w.MaKho, keyword) ||
                    Contains(w.TenKho, keyword) ||
                    Contains(w.MaNV, keyword) ||
                    Contains(w.TenNV, keyword)).ToList();

            var company = filtered
                .Where(w => WarehouseManagementBLL.IsCompanyWarehouse(w.LoaiKho))
                .OrderBy(w => w.MaKho)
                .Select(ToDisplay)
                .ToList();

            var personal = filtered
                .Where(w => WarehouseManagementBLL.IsPersonalWarehouse(w.LoaiKho))
                .ToList();

            dgvCompany.DataSource = company;

            var groups = personal
                .GroupBy(w => new { MaNV = w.MaNV ?? "", TenNV = w.TenNV ?? "" })
                .OrderBy(g => g.Key.TenNV)
                .ThenBy(g => g.Key.MaNV)
                .Select(g => new StaffWarehouseGroup
                {
                    MaNV = g.Key.MaNV,
                    TenNV = g.Key.TenNV,
                    SoKho = g.Count(),
                    KhoUuTien = g.FirstOrDefault(x => x.KhoUuTien)?.TenKho ?? ""
                })
                .ToList();

            dgvStaffGroups.DataSource = groups;

            if (!string.IsNullOrWhiteSpace(selectedStaffCode) && groups.Any(g => g.MaNV == selectedStaffCode))
            {
                BindPersonalWarehouses(personal.Where(w => w.MaNV == selectedStaffCode).ToList());
            }
            else if (groups.Count > 0)
            {
                selectedStaffCode = groups[0].MaNV;
                BindPersonalWarehouses(personal.Where(w => w.MaNV == selectedStaffCode).ToList());
            }
            else
            {
                selectedStaffCode = null;
                dgvPersonal.DataSource = new List<WarehouseDisplay>();
            }
        }

        private void BindPersonalWarehouses(List<Warehouse> personalWarehouses)
        {
            dgvPersonal.DataSource = personalWarehouses
                .OrderByDescending(w => w.KhoUuTien)
                .ThenBy(w => w.MaKho)
                .Select(ToDisplay)
                .ToList();
        }

        private void SelectStaffGroup()
        {
            if (dgvStaffGroups.CurrentRow?.DataBoundItem is not StaffWarehouseGroup group)
                return;

            selectedStaffCode = group.MaNV;
            var personal = warehouses
                .Where(w => WarehouseManagementBLL.IsPersonalWarehouse(w.LoaiKho) && w.MaNV == selectedStaffCode)
                .ToList();
            BindPersonalWarehouses(personal);
        }

        private void SelectStaffFromCombo()
        {
            if (cboLoaiKho.SelectedItem?.ToString() != "CANHAN" || cboNhanVien.SelectedValue == null)
                return;

            var staffCode = cboNhanVien.SelectedValue.ToString();
            if (string.IsNullOrWhiteSpace(staffCode))
                return;

            selectedStaffCode = staffCode;
            tabControl.SelectedTab = tabPersonal;

            var personal = warehouses
                .Where(w => WarehouseManagementBLL.IsPersonalWarehouse(w.LoaiKho) && w.MaNV == selectedStaffCode)
                .ToList();
            BindPersonalWarehouses(personal);
            SelectStaffGroupRow(staffCode);
        }

        private void SelectStaffGroupRow(string staffCode)
        {
            foreach (DataGridViewRow row in dgvStaffGroups.Rows)
            {
                if (row.DataBoundItem is StaffWarehouseGroup group && group.MaNV == staffCode)
                {
                    row.Selected = true;
                    dgvStaffGroups.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private void SelectWarehouseFromGrid(DataGridView grid)
        {
            if (grid.Focused && grid.CurrentRow?.DataBoundItem is WarehouseDisplay display)
            {
                selectedWarehouse = warehouses.FirstOrDefault(w => w.Id == display.Id);
                FillEditor(selectedWarehouse);
            }
        }

        private void FillEditor(Warehouse? warehouse)
        {
            if (warehouse == null)
                return;

            txtMaKho.Text = warehouse.MaKho;
            txtTenKho.Text = warehouse.TenKho;
            cboLoaiKho.SelectedItem = WarehouseManagementBLL.IsPersonalWarehouse(warehouse.LoaiKho) ? "CANHAN" : "COMPANY";
            cboNhanVien.SelectedValue = warehouse.MaNV ?? "";
            txtDiaChi.Text = warehouse.DiaChi ?? "";
            txtGhiChu.Text = warehouse.GhiChu ?? "";
            chkKhoUuTien.Checked = warehouse.KhoUuTien;
            UpdateEditorState();
        }

        private void ClearEditor()
        {
            selectedWarehouse = null;
            txtMaKho.Clear();
            txtTenKho.Clear();
            cboLoaiKho.SelectedItem = "COMPANY";
            cboNhanVien.SelectedIndex = -1;
            txtDiaChi.Clear();
            txtGhiChu.Clear();
            chkKhoUuTien.Checked = false;
            UpdateEditorState();
            txtMaKho.Focus();
        }

        private void UpdateEditorState()
        {
            var isPersonal = cboLoaiKho.SelectedItem?.ToString() == "CANHAN";
            cboNhanVien.Enabled = isPersonal;
            chkKhoUuTien.Enabled = isPersonal;
            if (!isPersonal)
            {
                cboNhanVien.SelectedIndex = -1;
                chkKhoUuTien.Checked = false;
            }
        }

        private void SaveWarehouse()
        {
            try
            {
                var warehouse = new Warehouse
                {
                    Id = selectedWarehouse?.Id ?? 0,
                    MaKho = txtMaKho.Text.Trim(),
                    TenKho = txtTenKho.Text.Trim(),
                    LoaiKho = cboLoaiKho.SelectedItem?.ToString() ?? "COMPANY",
                    MaNV = cboNhanVien.Enabled ? cboNhanVien.SelectedValue?.ToString() : null,
                    DiaChi = txtDiaChi.Text.Trim(),
                    GhiChu = txtGhiChu.Text.Trim(),
                    KhoUuTien = chkKhoUuTien.Checked,
                    IsActive = true
                };

                warehouseBLL.SaveWarehouse(warehouse);
                LoadData();
                UpdateStatus("Đã lưu kho", StatusType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"Lỗi lưu kho: {ex.Message}", StatusType.Error);
            }
        }

        private void DeactivateSelectedWarehouse()
        {
            if (selectedWarehouse == null)
            {
                MessageBox.Show("Chưa chọn kho cần ngưng hoạt động", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show($"Ngưng hoạt động kho {selectedWarehouse.MaKho} - {selectedWarehouse.TenKho}?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                warehouseBLL.DeactivateWarehouse(selectedWarehouse.Id);
                LoadData();
                UpdateStatus("Đã ngưng hoạt động kho", StatusType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"Lỗi ngưng hoạt động: {ex.Message}", StatusType.Error);
            }
        }

        private void SetPrioritySelectedWarehouse()
        {
            if (selectedWarehouse == null)
            {
                MessageBox.Show("Chưa chọn kho cá nhân cần đặt ưu tiên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                warehouseBLL.SetPriorityWarehouse(selectedWarehouse);
                selectedStaffCode = selectedWarehouse.MaNV;
                LoadData();
                UpdateStatus("Đã đặt kho ưu tiên cho nhân viên", StatusType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"Lỗi đặt ưu tiên: {ex.Message}", StatusType.Error);
            }
        }

        private void UpdateStatus(string message, StatusType status)
        {
            lblStatus.Text = message;
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus, status);
        }

        private static bool Contains(string? source, string keyword)
        {
            return !string.IsNullOrWhiteSpace(source)
                && source.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        private static WarehouseDisplay ToDisplay(Warehouse warehouse)
        {
            return new WarehouseDisplay
            {
                Id = warehouse.Id,
                MaKho = warehouse.MaKho,
                TenKho = warehouse.TenKho,
                LoaiKho = warehouse.LoaiKho,
                MaNV = warehouse.MaNV ?? "",
                TenNV = warehouse.TenNV ?? "",
                KhoUuTienText = warehouse.KhoUuTien ? "Có" : "",
                GhiChu = warehouse.GhiChu ?? ""
            };
        }

        private sealed class StaffComboItem
        {
            public StaffComboItem(string maNV, string display)
            {
                MaNV = maNV;
                Display = display;
            }

            public string MaNV { get; }
            public string Display { get; }
        }

        private sealed class StaffWarehouseGroup
        {
            public string MaNV { get; set; } = string.Empty;
            public string TenNV { get; set; } = string.Empty;
            public int SoKho { get; set; }
            public string KhoUuTien { get; set; } = string.Empty;
        }

        private sealed class WarehouseDisplay
        {
            public int Id { get; set; }
            public string MaKho { get; set; } = string.Empty;
            public string TenKho { get; set; } = string.Empty;
            public string LoaiKho { get; set; } = string.Empty;
            public string MaNV { get; set; } = string.Empty;
            public string TenNV { get; set; } = string.Empty;
            public string KhoUuTienText { get; set; } = string.Empty;
            public string GhiChu { get; set; } = string.Empty;
        }
    }
}
