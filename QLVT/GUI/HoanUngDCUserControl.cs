using QLVT.BLL;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class HoanUngDCUserControl : UserControl
    {
        private readonly HoanUngBLL _hoanUngBLL;
        private List<SuaChuaModel> danhSachHoSo = new();
        private List<SuaChuaModel> danhSachHoSoGoc = new(); // Danh sách gốc để tìm kiếm
        private SuaChuaModel? selectedHoSo;
        private readonly ProgressBar prgTaiDuLieuERP = new();

        public HoanUngDCUserControl()
        {
            InitializeComponent();
            _hoanUngBLL = new HoanUngBLL();
            ApplyModernStyle();
            SetupDataGridView();
            LoadDanhSachHoSo();
            
            // Khởi tạo trạng thái kết nối
            lblConnectionStatus.Text = "🔄 Đang kiểm tra kết nối ERP...";
            lblConnectionStatus.ForeColor = UIColorPalette.StatusProcessing;
            CheckERPConnection();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "HOÀN ỨNG SỬA CHỮA SỰ CỐ";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            var toolbarTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;
            lblConnectionStatus.Location = new Point(6, toolbarTop + 5);
            prgTaiDuLieuERP.Location = new Point(6, toolbarTop + 35);
            prgTaiDuLieuERP.Size = new Size(360, 10);
            btnTaiDuLieuERP.Location = new Point(385, toolbarTop);
            btnRefresh.Location = new Point(545, toolbarTop);
            grpTimKiem.Location = new Point(Math.Max(700, Width - grpTimKiem.Width - 20), toolbarTop - 8);
            grpDanhSach.Location = new Point(6, toolbarTop + 52);
            grpDanhSach.Size = new Size(Math.Max(900, Width - 12), Math.Max(300, Height - grpDanhSach.Top - 10));

            UIStyleHelper.ApplyStatusLabelStyle(lblConnectionStatus, StatusType.Processing);
            prgTaiDuLieuERP.Style = ProgressBarStyle.Marquee;
            prgTaiDuLieuERP.MarqueeAnimationSpeed = 32;
            prgTaiDuLieuERP.Visible = false;
            Controls.Add(prgTaiDuLieuERP);
            prgTaiDuLieuERP.BringToFront();
            UIStyleHelper.ApplyGroupBoxStyle(grpTimKiem);
            UIStyleHelper.ApplyGroupBoxStyle(grpDanhSach);
            grpTimKiem.Text = "Tìm kiếm";
            grpDanhSach.Text = "Danh sách hồ sơ DC chờ hoàn ứng";

            lblTimKiem.Text = "Mã đơn/Vị trí";
            UIStyleHelper.ApplyStandardLabelStyle(lblTimKiem);
            UIStyleHelper.ApplyTextBoxStyle(txtTimKiem);
            txtTimKiem.PlaceholderText = "Nhập mã đơn hoặc vị trí điểm chảy";

            btnTaiDuLieuERP.Text = "Tải dữ liệu ERP";
            btnRefresh.Text = "Làm mới";
            btnTimKiem.Text = "Tìm";
            btnXoaTimKiem.Text = "Xóa";
            UIStyleHelper.ApplyPrimaryButtonStyle(btnTaiDuLieuERP, new Size(132, 30));
            UIStyleHelper.ApplySecondaryButtonStyle(btnRefresh, new Size(92, 30));
            UIStyleHelper.ApplySuccessButtonStyle(btnTimKiem, new Size(58, 27));
            UIStyleHelper.ApplySecondaryButtonStyle(btnXoaTimKiem, new Size(58, 27));
            txtTimKiem.Size = new Size(220, 23);
            btnTimKiem.Location = new Point(355, 18);
            btnXoaTimKiem.Location = new Point(417, 18);

            lblThongKe.Font = UIFonts.HeaderStandard;
            lblThongKe.ForeColor = UIColorPalette.StatusProcessing;
        }

        private void SetupButtonHoverEffects()
        {
            // Hover effect cho btnTaiDuLieuERP (Primary Blue)
            btnTaiDuLieuERP.MouseEnter += (s, e) => btnTaiDuLieuERP.BackColor = UIColorPalette.ButtonPrimary.Hover;
            btnTaiDuLieuERP.MouseLeave += (s, e) => btnTaiDuLieuERP.BackColor = UIColorPalette.ButtonPrimary.Base;

            // Hover effect cho btnRefresh (Success Green)
            btnRefresh.MouseEnter += (s, e) => btnRefresh.BackColor = UIColorPalette.ButtonSuccess.Hover;
            btnRefresh.MouseLeave += (s, e) => btnRefresh.BackColor = UIColorPalette.ButtonSuccess.Base;

            // Hover effect cho btnTimKiem (Success Green)
            btnTimKiem.MouseEnter += (s, e) => btnTimKiem.BackColor = UIColorPalette.ButtonSuccess.Hover;
            btnTimKiem.MouseLeave += (s, e) => btnTimKiem.BackColor = UIColorPalette.ButtonSuccess.Base;

            // Hover effect cho btnXoaTimKiem (Danger Red)
            btnXoaTimKiem.MouseEnter += (s, e) => btnXoaTimKiem.BackColor = UIColorPalette.ButtonDanger.Hover;
            btnXoaTimKiem.MouseLeave += (s, e) => btnXoaTimKiem.BackColor = UIColorPalette.ButtonDanger.Base;
        }

        private void CheckERPConnection()
        {
            try
            {
                bool connected = Utils.ExternalDatabaseHelper.TestExternalConnection();
                lblConnectionStatus.Text = connected ? "✅ Kết nối ERP thành công" : "❌ Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = connected ? UIColorPalette.StatusSuccess : UIColorPalette.StatusError;
                btnTaiDuLieuERP.Enabled = connected;
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "❌ Lỗi kết nối ERP";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
                btnTaiDuLieuERP.Enabled = false;
                MessageBox.Show($"Lỗi kiểm tra kết nối ERP: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetTaiDuLieuERPProgress(bool isLoading)
        {
            prgTaiDuLieuERP.Visible = isLoading;
            prgTaiDuLieuERP.MarqueeAnimationSpeed = isLoading ? 32 : 0;
        }

        private void SetupDataGridView()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvHoSoDC);
            dgvHoSoDC.ReadOnly = true;
            dgvHoSoDC.MultiSelect = false;
            dgvHoSoDC.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvHoSoDC.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoSoDC.Columns.Clear();

            // Thêm các cột
            dgvHoSoDC.Columns.Add("MADON", "Mã đơn");
            dgvHoSoDC.Columns.Add("ViTriDiemChay", "Vị trí điểm chảy");
            dgvHoSoDC.Columns.Add("NhanVienXayLap", "NV Thi công");
            dgvHoSoDC.Columns.Add("NgayHoanUng", "Ngày hoàn ứng");

            // Thêm cột nút chức năng
            DataGridViewButtonColumn colXemChiTiet = new DataGridViewButtonColumn();
            colXemChiTiet.Name = "btnXemChiTiet";
            colXemChiTiet.HeaderText = "Chi tiết VT";
            colXemChiTiet.Text = "Xem";
            colXemChiTiet.UseColumnTextForButtonValue = true;
            colXemChiTiet.Width = 80;
            colXemChiTiet.FlatStyle = FlatStyle.Flat;
            colXemChiTiet.CellTemplate.Style.BackColor = UIColorPalette.ButtonInfo.Base;
            colXemChiTiet.CellTemplate.Style.ForeColor = UIColorPalette.ButtonInfo.Text;
            dgvHoSoDC.Columns.Add(colXemChiTiet);

            DataGridViewButtonColumn colXacNhanHoanUng = new DataGridViewButtonColumn();
            colXacNhanHoanUng.Name = "btnXacNhanHoanUng";
            colXacNhanHoanUng.HeaderText = "Hoàn ứng";
            colXacNhanHoanUng.Text = "Hoàn ứng";
            colXacNhanHoanUng.UseColumnTextForButtonValue = true;
            colXacNhanHoanUng.Width = 80;
            colXacNhanHoanUng.FlatStyle = FlatStyle.Flat;
            colXacNhanHoanUng.CellTemplate.Style.BackColor = UIColorPalette.ButtonAction.Base;
            colXacNhanHoanUng.CellTemplate.Style.ForeColor = UIColorPalette.ButtonAction.Text;
            dgvHoSoDC.Columns.Add(colXacNhanHoanUng);

            // Set column widths
            dgvHoSoDC.Columns["MADON"].Width = 100;
            dgvHoSoDC.Columns["ViTriDiemChay"].Width = 500;
            dgvHoSoDC.Columns["NhanVienXayLap"].Width = 150;
            dgvHoSoDC.Columns["NgayHoanUng"].Width = 120;

            dgvHoSoDC.Columns["ViTriDiemChay"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvHoSoDC.Columns["MADON"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoSoDC.Columns["NgayHoanUng"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvHoSoDC.CellClick += DgvHoSoDC_CellClick;
            dgvHoSoDC.SelectionChanged += DgvHoSoDC_SelectionChanged;
        }

        private void LoadDanhSachHoSo()
        {
            try
            {
                danhSachHoSoGoc = _hoanUngBLL.DC_GetDanhSachChoHoanUng();
                danhSachHoSo = new List<SuaChuaModel>(danhSachHoSoGoc); // Copy danh sách
                DisplayHoSoList();
                UpdateStatusCounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách hồ sơ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayHoSoList()
        {
            dgvHoSoDC.Rows.Clear();
            
            foreach (var hoSo in danhSachHoSo)
            {                
                dgvHoSoDC.Rows.Add(
                    hoSo.MADON,
                    hoSo.ViTriDiemChay,
                    hoSo.NhanVienXayLap,
                    hoSo.NgayHoanUng?.ToString("dd/MM/yyyy") ?? ""
                );

                // Đổi màu cho hàng theo trạng thái
                var row = dgvHoSoDC.Rows[dgvHoSoDC.Rows.Count - 1];
                row.DefaultCellStyle.BackColor = UIColorPalette.BackgroundWhite;
                row.DefaultCellStyle.ForeColor = UIColorPalette.TextBlack;
            }
        }

        private void UpdateStatusCounts()
        {
            int tongSo = danhSachHoSo.Count;
            int tongSoGoc = danhSachHoSoGoc.Count;

            if (tongSo == tongSoGoc)
            {
                lblThongKe.Text = $"Tổng số hồ sơ chờ xử lý: {tongSo}";
            }
            else
            {
                lblThongKe.Text = $"Hiển thị {tongSo}/{tongSoGoc} hồ sơ (đã lọc)";
            }
        }

        private void DgvHoSoDC_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHoSoDC.SelectedRows.Count > 0)
            {
                int index = dgvHoSoDC.SelectedRows[0].Index;
                if (index >= 0 && index < danhSachHoSo.Count)
                {
                    selectedHoSo = danhSachHoSo[index];
                }
            }
        }

        private void DgvHoSoDC_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < danhSachHoSo.Count)
            {
                var hoSo = danhSachHoSo[e.RowIndex];

                if (e.ColumnIndex == dgvHoSoDC.Columns["btnXemChiTiet"].Index)
                {
                    XemChiTietVatTu(hoSo);
                }
                else if (e.ColumnIndex == dgvHoSoDC.Columns["btnXacNhanHoanUng"].Index)
                {
                    XacNhanHoanUng(hoSo);
                }
            }
        }

        private void XemChiTietVatTu(SuaChuaModel hoSo)
        {
            try
            {
                var chiTietListWithTonKho = _hoanUngBLL.DC_GetChiTietVatTuWithTonKho(hoSo.MADON);
                
                using (var formChiTiet = new ChiTietVatTuDCForm(hoSo, chiTietListWithTonKho))
                {
                    // Chỉ refresh khi hoàn ứng thành công
                    if (formChiTiet.ShowDialog() == DialogResult.OK)
                    {
                        LoadDanhSachHoSo();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem chi tiết vật tư: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void XacNhanHoanUng(SuaChuaModel hoSo)
        {
            if (hoSo.DaHoanUng == true)
            {
                MessageBox.Show("Hồ sơ này đã được hoàn ứng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Xác nhận hoàn ứng cho hồ sơ:\n\n" +
                $"Mã đơn: {hoSo.MADON}\n" +
                $"Vị trí: {hoSo.ViTriDiemChay}\n" +
                $"NV Thi công: {hoSo.NhanVienXayLap}\n\n" +
                $"Bạn có chắc chắn muốn thực hiện hoàn ứng?",
                "Xác nhận hoàn ứng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _hoanUngBLL.DC_XacNhanHoanUng(hoSo.MADON, null);
                    
                    MessageBox.Show("Hoàn ứng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachHoSo(); // Reload dữ liệu
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hoàn ứng không thành công!\n\nChi tiết lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnTaiDuLieuERP_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable button và hiển thị trạng thái
                btnTaiDuLieuERP.Enabled = false;
                btnTaiDuLieuERP.Text = "Đang tải...";
                lblConnectionStatus.Text = "⏳ Đang tải dữ liệu từ ERP...";
                lblConnectionStatus.ForeColor = UIColorPalette.WarningOrange;
                SetTaiDuLieuERPProgress(true);
                
                // Xác nhận từ người dùng
                var confirmResult = MessageBox.Show(
                    "Bạn có chắc chắn muốn tải dữ liệu mới từ ERP?\n\n" +
                    "Quá trình này có thể mất một lúc và sẽ tải tất cả đơn sửa chữa mới.",
                    "Xác nhận tải dữ liệu ERP", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                // Thực hiện tải dữ liệu
                var (soLuongDon, soLuongChiTiet, thongBao) = await _hoanUngBLL.DC_TaiDuLieuERP();
                
                // Hiển thị kết quả
                MessageBox.Show(thongBao, "Kết quả tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Reload danh sách
                LoadDanhSachHoSo();
                
                // Cập nhật trạng thái
                lblConnectionStatus.Text = $"✅ Đã tải {soLuongDon} đơn, {soLuongChiTiet} chi tiết";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusSuccess;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ERP: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblConnectionStatus.Text = "❌ Lỗi tải dữ liệu ERP";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
            }
            finally
            {
                btnTaiDuLieuERP.Enabled = true;
                btnTaiDuLieuERP.Text = "Tải dữ liệu ERP";
                SetTaiDuLieuERPProgress(false);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDanhSachHoSo();
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            TimKiem();
        }

        private void BtnXoaTimKiem_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = "";
            danhSachHoSo = new List<SuaChuaModel>(danhSachHoSoGoc);
            DisplayHoSoList();
            UpdateStatusCounts();
        }

        private void TxtTimKiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TimKiem();
                e.Handled = true;
            }
        }

        private void TimKiem()
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                if (string.IsNullOrEmpty(tuKhoa))
                {
                    // Hiển thị tất cả nếu không có từ khóa
                    danhSachHoSo = new List<SuaChuaModel>(danhSachHoSoGoc);
                }
                else
                {
                    // Tìm kiếm theo mã đơn hoặc tên khách hàng
                    danhSachHoSo = danhSachHoSoGoc.Where(x => 
                        (x.MADON?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) == true) ||
                        (x.ViTriDiemChay?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) == true)
                    ).ToList();
                }

                DisplayHoSoList();
                UpdateStatusCounts();

                if (danhSachHoSo.Count == 0 && !string.IsNullOrEmpty(tuKhoa))
                {
                    MessageBox.Show($"Không tìm thấy hồ sơ nào với từ khóa: '{tuKhoa}'", 
                        "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
