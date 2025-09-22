using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.ERP.Models;
using QLVT.ERP.BLL;

namespace QLVT.GUI
{
    public partial class HoanUngMC4UserControl : UserControl
    {
        private readonly HoanUngBLL hoanUngBLL;
        private List<DonDangKyModel> danhSachHoSo = new();
        private List<DonDangKyModel> danhSachHoSoGoc = new(); // Danh sách gốc để tìm kiếm
        private DonDangKyModel? selectedHoSo;

        public HoanUngMC4UserControl()
        {
            InitializeComponent();
            hoanUngBLL = new HoanUngBLL();
            SetupDataGridView();
            LoadDanhSachHoSo();
            CheckERPConnection();
        }

        private void CheckERPConnection()
        {
            try
            {
                bool connected = hoanUngBLL.TestERPConnection();
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
            dgvHoSoMC4.AllowUserToAddRows = false;
            dgvHoSoMC4.AllowUserToDeleteRows = false;
            dgvHoSoMC4.ReadOnly = true;
            dgvHoSoMC4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHoSoMC4.MultiSelect = false;
            dgvHoSoMC4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHoSoMC4.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvHoSoMC4.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Thêm các cột
            dgvHoSoMC4.Columns.Add("MADDK", "Mã DDK");
            dgvHoSoMC4.Columns.Add("TENKH", "Tên khách hàng");
            dgvHoSoMC4.Columns.Add("DiaChi", "Địa chỉ");
            dgvHoSoMC4.Columns.Add("NhanVienXayLap", "NV Thi công");
            dgvHoSoMC4.Columns.Add("NgayHoanThanh", "Ngày hoàn thành");

            // Thêm cột nút chức năng
            DataGridViewButtonColumn colXemChiTiet = new DataGridViewButtonColumn();
            colXemChiTiet.Name = "btnXemChiTiet";
            colXemChiTiet.HeaderText = "Chi tiết VT";
            colXemChiTiet.Text = "Xem chi tiết";
            colXemChiTiet.UseColumnTextForButtonValue = true;
            colXemChiTiet.Width = 80;
            dgvHoSoMC4.Columns.Add(colXemChiTiet);

            DataGridViewButtonColumn colXacNhanHoanUng = new DataGridViewButtonColumn();
            colXacNhanHoanUng.Name = "btnXacNhanHoanUng";
            colXacNhanHoanUng.HeaderText = "Hoàn ứng";
            colXacNhanHoanUng.Text = "Hoàn ứng";
            colXacNhanHoanUng.UseColumnTextForButtonValue = true;
            colXacNhanHoanUng.Width = 80;
            dgvHoSoMC4.Columns.Add(colXacNhanHoanUng);

            // Set column widths
            dgvHoSoMC4.Columns["MADDK"].Width = 100;
            dgvHoSoMC4.Columns["TENKH"].Width = 170;
            dgvHoSoMC4.Columns["DiaChi"].Width = 350;
            dgvHoSoMC4.Columns["NhanVienXayLap"].Width = 150;
            dgvHoSoMC4.Columns["NgayHoanThanh"].Width = 120;

            dgvHoSoMC4.Columns["DiaChi"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvHoSoMC4.Columns["TENKH"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvHoSoMC4.CellClick += DgvHoSoMC4_CellClick;
            dgvHoSoMC4.SelectionChanged += DgvHoSoMC4_SelectionChanged;
        }

        private void LoadDanhSachHoSo()
        {
            try
            {
                danhSachHoSoGoc = hoanUngBLL.GetDanhSachHoSoChoHoanUng();
                danhSachHoSo = new List<DonDangKyModel>(danhSachHoSoGoc); // Copy danh sách
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
            dgvHoSoMC4.Rows.Clear();
            
            foreach (var hoSo in danhSachHoSo)
            {                
                dgvHoSoMC4.Rows.Add(
                    hoSo.MADDK,
                    hoSo.TENKH,
                    hoSo.DiaChi,
                    hoSo.NhanVienXayLap,
                    hoSo.NgayHoanUng?.ToString("dd/MM/yyyy") ?? ""
                );

                // Đổi màu cho hàng theo trạng thái
                var row = dgvHoSoMC4.Rows[dgvHoSoMC4.Rows.Count - 1];

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

        private void DgvHoSoMC4_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHoSoMC4.SelectedRows.Count > 0)
            {
                int index = dgvHoSoMC4.SelectedRows[0].Index;
                if (index >= 0 && index < danhSachHoSo.Count)
                {
                    selectedHoSo = danhSachHoSo[index];
                }
            }
        }

        private void DgvHoSoMC4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < danhSachHoSo.Count)
            {
                var hoSo = danhSachHoSo[e.RowIndex];

                if (e.ColumnIndex == dgvHoSoMC4.Columns["btnXemChiTiet"].Index)
                {
                    XemChiTietVatTu(hoSo);
                }
                else if (e.ColumnIndex == dgvHoSoMC4.Columns["btnXacNhanHoanUng"].Index)
                {
                    XacNhanHoanUng(hoSo);
                }
            }
        }

        private void XemChiTietVatTu(DonDangKyModel hoSo)
        {
            try
            {
                var chiTietListWithTonKho = hoanUngBLL.GetChiTietVatTuWithTonKho(hoSo.MADDK);
                
                using (var formChiTiet = new ChiTietVatTuMC4Form(hoSo, chiTietListWithTonKho))
                {
                    formChiTiet.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem chi tiết vật tư: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XacNhanHoanUng(DonDangKyModel hoSo)
        {
            if (hoSo.DaHoanUng == true)
            {
                MessageBox.Show("Hồ sơ này đã được hoàn ứng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Xác nhận hoàn ứng cho hồ sơ:\n\n" +
                $"Mã đơn: {hoSo.MADDK}\n" +
                $"Khách hàng: {hoSo.TENKH}\n" +
                $"NV Thi công: {hoSo.NhanVienXayLap}\n\n" +
                $"Bạn có chắc chắn muốn thực hiện hoàn ứng?",
                "Xác nhận hoàn ứng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string currentUser = Environment.UserName; // Hoặc lấy từ session
                    hoanUngBLL.XacNhanHoanUng(hoSo.MADDK, currentUser);
                    
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
                    "Quá trình này có thể mất một lúc và sẽ tải tất cả đơn đăng ký mới.",
                    "Xác nhận tải dữ liệu ERP", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                // Thực hiện tải dữ liệu
                var (soLuongDon, soLuongChiTiet, thongBao) = await hoanUngBLL.TaiDuLieuERP();
                
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
            danhSachHoSo = new List<DonDangKyModel>(danhSachHoSoGoc);
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
                    danhSachHoSo = new List<DonDangKyModel>(danhSachHoSoGoc);
                }
                else
                {
                    // Tìm kiếm theo mã đơn hoặc tên khách hàng
                    danhSachHoSo = danhSachHoSoGoc.Where(x => 
                        (x.MADDK?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) == true) ||
                        (x.TENKH?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) == true)
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