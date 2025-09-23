using QLVT.BLL;
using QLVT.ERP.Models;

namespace QLVT.GUI
{
    public partial class HoanUngDCUserControl : UserControl
    {
        private readonly HoanUngBLL _hoanUngBLL;
        private List<SuaChuaModel> danhSachHoSo = new();
        private List<SuaChuaModel> danhSachHoSoGoc = new(); // Danh sách gốc để tìm kiếm
        private SuaChuaModel? selectedHoSo;

        public HoanUngDCUserControl()
        {
            InitializeComponent();
            _hoanUngBLL = new HoanUngBLL();
            SetupDataGridView();
            LoadDanhSachHoSo();
            CheckERPConnection();
        }

        private void CheckERPConnection()
        {
            try
            {
                bool connected = Utils.ExternalDatabaseHelper.TestExternalConnection();
                lblConnectionStatus.Text = connected ? "✅ Kết nối ERP thành công" : "❌ Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = connected ? Color.Green : Color.Red;
                btnTaiDuLieuERP.Enabled = connected;
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "❌ Lỗi kết nối ERP";
                lblConnectionStatus.ForeColor = Color.Red;
                btnTaiDuLieuERP.Enabled = false;
                MessageBox.Show($"Lỗi kiểm tra kết nối ERP: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetupDataGridView()
        {
            dgvHoSoDC.AllowUserToAddRows = false;
            dgvHoSoDC.AllowUserToDeleteRows = false;
            dgvHoSoDC.ReadOnly = true;
            dgvHoSoDC.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHoSoDC.MultiSelect = false;
            dgvHoSoDC.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHoSoDC.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvHoSoDC.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Thêm các cột
            dgvHoSoDC.Columns.Add("MADON", "Mã đơn");
            dgvHoSoDC.Columns.Add("ViTriDiemChay", "Vị trí điểm chảy");
            dgvHoSoDC.Columns.Add("NhanVienXayLap", "NV Thi công");
            dgvHoSoDC.Columns.Add("NgayHoanUng", "Ngày hoàn ứng");

            // Thêm cột nút chức năng
            DataGridViewButtonColumn colXemChiTiet = new DataGridViewButtonColumn();
            colXemChiTiet.Name = "btnXemChiTiet";
            colXemChiTiet.HeaderText = "Chi tiết VT";
            colXemChiTiet.Text = "Xem chi tiết";
            colXemChiTiet.UseColumnTextForButtonValue = true;
            colXemChiTiet.Width = 80;
            dgvHoSoDC.Columns.Add(colXemChiTiet);

            DataGridViewButtonColumn colXacNhanHoanUng = new DataGridViewButtonColumn();
            colXacNhanHoanUng.Name = "btnXacNhanHoanUng";
            colXacNhanHoanUng.HeaderText = "Hoàn ứng";
            colXacNhanHoanUng.Text = "Hoàn ứng";
            colXacNhanHoanUng.UseColumnTextForButtonValue = true;
            colXacNhanHoanUng.Width = 80;
            dgvHoSoDC.Columns.Add(colXacNhanHoanUng);

            // Set column widths
            dgvHoSoDC.Columns["MADON"].Width = 100;
            dgvHoSoDC.Columns["ViTriDiemChay"].Width = 500;
            dgvHoSoDC.Columns["NhanVienXayLap"].Width = 150;
            dgvHoSoDC.Columns["NgayHoanUng"].Width = 120;

            dgvHoSoDC.Columns["ViTriDiemChay"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

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

                row.DefaultCellStyle.BackColor = Color.LightYellow;
                row.DefaultCellStyle.ForeColor = Color.DarkBlue;
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
                    formChiTiet.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem chi tiết vật tư: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XacNhanHoanUng(SuaChuaModel hoSo)
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
                    _hoanUngBLL.DC_XacNhanHoanUng(hoSo.MADON);
                    
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
                lblConnectionStatus.ForeColor = Color.Orange;
                
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
                lblConnectionStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ERP: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblConnectionStatus.Text = "❌ Lỗi tải dữ liệu ERP";
                lblConnectionStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnTaiDuLieuERP.Enabled = true;
                btnTaiDuLieuERP.Text = "Tải dữ liệu ERP";
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