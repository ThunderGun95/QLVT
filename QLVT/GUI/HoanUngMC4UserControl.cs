using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class HoanUngMC4UserControl : UserControl
    {
        private readonly HoanUngBLL hoanUngBLL;
        private List<DonDangKyModel> danhSachHoSo = new();
        private List<DonDangKyModel> danhSachHoSoGoc = new();
        private DonDangKyModel? selectedHoSo;

        public HoanUngMC4UserControl()
        {
            InitializeComponent();
            hoanUngBLL = new HoanUngBLL();
            ApplyModernStyle();
            SetupDataGridView();
            LoadDanhSachHoSo();

            lblConnectionStatus.Text = "Đang kiểm tra kết nối ERP...";
            lblConnectionStatus.ForeColor = UIColorPalette.StatusProcessing;
            CheckERPConnection();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "HOÀN ỨNG MẠNG CẤP 4";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            var toolbarTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;
            lblConnectionStatus.Location = new Point(6, toolbarTop + 5);
            btnTaiDuLieuERP.Location = new Point(385, toolbarTop);
            btnRefresh.Location = new Point(545, toolbarTop);
            grpTimKiem.Location = new Point(Math.Max(700, Width - grpTimKiem.Width - 20), toolbarTop - 8);
            grpDanhSach.Location = new Point(6, toolbarTop + 52);
            grpDanhSach.Size = new Size(Math.Max(900, Width - 12), Math.Max(300, Height - grpDanhSach.Top - 10));

            UIStyleHelper.ApplyStatusLabelStyle(lblConnectionStatus, StatusType.Processing);

            grpTimKiem.Text = "Tìm kiếm";
            grpDanhSach.Text = "Danh sách hồ sơ MC4 chờ hoàn ứng";
            UIStyleHelper.ApplyGroupBoxStyle(grpTimKiem);
            UIStyleHelper.ApplyGroupBoxStyle(grpDanhSach);

            lblTimKiem.Text = "Mã đơn/Tên KH";
            UIStyleHelper.ApplyStandardLabelStyle(lblTimKiem);

            UIStyleHelper.ApplyTextBoxStyle(txtTimKiem);
            txtTimKiem.PlaceholderText = "Nhập mã đơn hoặc tên khách hàng";

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
            lblThongKe.Text = "Chưa có dữ liệu";
        }

        private void CheckERPConnection()
        {
            try
            {
                bool connected = hoanUngBLL.TestERPConnection();
                lblConnectionStatus.Text = connected ? "Kết nối ERP thành công" : "Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = connected ? UIColorPalette.StatusSuccess : UIColorPalette.StatusError;
                btnTaiDuLieuERP.Enabled = connected;
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "Lỗi kết nối ERP";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
                btnTaiDuLieuERP.Enabled = false;
                MessageBox.Show($"Lỗi kiểm tra kết nối ERP: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetupDataGridView()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvHoSoMC4);
            dgvHoSoMC4.ReadOnly = true;
            dgvHoSoMC4.MultiSelect = false;
            dgvHoSoMC4.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvHoSoMC4.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoSoMC4.Columns.Clear();

            dgvHoSoMC4.Columns.Add("MADDK", "Mã DDK");
            dgvHoSoMC4.Columns.Add("TENKH", "Tên khách hàng");
            dgvHoSoMC4.Columns.Add("DiaChi", "Địa chỉ");
            dgvHoSoMC4.Columns.Add("NhanVienXayLap", "NV thi công");
            dgvHoSoMC4.Columns.Add("NgayHoanUng", "Ngày hoàn ứng");

            var colXemChiTiet = new DataGridViewButtonColumn
            {
                Name = "btnXemChiTiet",
                HeaderText = "Chi tiết VT",
                Text = "Xem",
                UseColumnTextForButtonValue = true,
                Width = 80,
                FlatStyle = FlatStyle.Flat
            };
            colXemChiTiet.CellTemplate.Style.BackColor = UIColorPalette.ButtonInfo.Base;
            dgvHoSoMC4.Columns.Add(colXemChiTiet);

            var colXacNhanHoanUng = new DataGridViewButtonColumn
            {
                Name = "btnXacNhanHoanUng",
                HeaderText = "Hoàn ứng",
                Text = "Hoàn ứng",
                UseColumnTextForButtonValue = true,
                Width = 80,
                FlatStyle = FlatStyle.Flat
            };
            colXacNhanHoanUng.CellTemplate.Style.BackColor = UIColorPalette.ButtonAction.Base;
            dgvHoSoMC4.Columns.Add(colXacNhanHoanUng);

            dgvHoSoMC4.Columns["MADDK"].Width = 100;
            dgvHoSoMC4.Columns["TENKH"].Width = 170;
            dgvHoSoMC4.Columns["DiaChi"].Width = 350;
            dgvHoSoMC4.Columns["NhanVienXayLap"].Width = 150;
            dgvHoSoMC4.Columns["NgayHoanUng"].Width = 120;

            dgvHoSoMC4.Columns["DiaChi"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvHoSoMC4.Columns["TENKH"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvHoSoMC4.Columns["MADDK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHoSoMC4.Columns["NgayHoanUng"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvHoSoMC4.CellClick += DgvHoSoMC4_CellClick;
            dgvHoSoMC4.SelectionChanged += DgvHoSoMC4_SelectionChanged;
        }

        private void LoadDanhSachHoSo()
        {
            try
            {
                danhSachHoSoGoc = hoanUngBLL.MC4_GetDanhSachChoHoanUng();
                danhSachHoSo = new List<DonDangKyModel>(danhSachHoSoGoc);
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
                    hoSo.NgayHoanUng?.ToString("dd/MM/yyyy") ?? "");

                var row = dgvHoSoMC4.Rows[dgvHoSoMC4.Rows.Count - 1];
                row.DefaultCellStyle.BackColor = UIColorPalette.BackgroundWhite;
                row.DefaultCellStyle.ForeColor = UIColorPalette.TextBlack;
            }
        }

        private void UpdateStatusCounts()
        {
            int tongSo = danhSachHoSo.Count;
            int tongSoGoc = danhSachHoSoGoc.Count;

            lblThongKe.Text = tongSo == tongSoGoc
                ? $"Tổng số hồ sơ chờ xử lý: {tongSo}"
                : $"Hiển thị {tongSo}/{tongSoGoc} hồ sơ đã lọc";
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
                var chiTietListWithTonKho = hoanUngBLL.MC4_GetChiTietVatTuWithTonKho(hoSo.MADDK);

                using (var formChiTiet = new ChiTietVatTuMC4Form(hoSo, chiTietListWithTonKho))
                {
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

        private async void XacNhanHoanUng(DonDangKyModel hoSo)
        {
            if (hoSo.DaHoanUng == true)
            {
                MessageBox.Show("Hồ sơ này đã được hoàn ứng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Xác nhận hoàn ứng cho hồ sơ:\n\n" +
                $"Mã đơn: {hoSo.MADDK}\n" +
                $"Khách hàng: {hoSo.TENKH}\n" +
                $"NV thi công: {hoSo.NhanVienXayLap}\n\n" +
                $"Bạn có chắc chắn muốn thực hiện hoàn ứng?",
                "Xác nhận hoàn ứng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await hoanUngBLL.MC4_XacNhanHoanUng(hoSo.MADDK, null, false);

                    MessageBox.Show("Hoàn ứng thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachHoSo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hoàn ứng không thành công.\n\nChi tiết lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnTaiDuLieuERP_Click(object sender, EventArgs e)
        {
            try
            {
                btnTaiDuLieuERP.Enabled = false;
                btnTaiDuLieuERP.Text = "Đang tải...";
                lblConnectionStatus.Text = "Đang tải dữ liệu từ ERP...";
                lblConnectionStatus.ForeColor = UIColorPalette.WarningOrange;

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

                var (soLuongDon, soLuongChiTiet, thongBao) = await hoanUngBLL.MC4_TaiDuLieuERP();

                MessageBox.Show(thongBao, "Kết quả tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachHoSo();

                lblConnectionStatus.Text = $"Đã tải {soLuongDon} đơn, {soLuongChiTiet} chi tiết";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusSuccess;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ERP: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblConnectionStatus.Text = "Lỗi tải dữ liệu ERP";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
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

                danhSachHoSo = string.IsNullOrEmpty(tuKhoa)
                    ? new List<DonDangKyModel>(danhSachHoSoGoc)
                    : danhSachHoSoGoc.Where(x =>
                        (x.MADDK?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) == true) ||
                        (x.TENKH?.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) == true))
                    .ToList();

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
