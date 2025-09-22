using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.ERP.Models;
using QLVT.BLL;

namespace QLVT.GUI
{
    public partial class ChiTietVatTuMC4Form : Form
    {
        private DonDangKyModel hoSo;
        private List<DonDangKyCTModel> chiTietList;
        private HoanUngBLL hoanUngBLL;

        public ChiTietVatTuMC4Form(DonDangKyModel hoSo, List<DonDangKyCTModel> chiTietList)
        {
            this.hoSo = hoSo;
            this.chiTietList = chiTietList;
            this.hoanUngBLL = new HoanUngBLL();
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // Hiển thị thông tin hồ sơ
            lblMaDDK.Text = hoSo.MADDK;
            lblTenKH.Text = hoSo.TENKH;
            lblDiaChi.Text = hoSo.DiaChi;
            lblNVTaiCong.Text = hoSo.NhanVienXayLap;

            // Load chi tiết vật tư
            LoadChiTietVatTu();
            
            // Tính tổng - bỏ tổng thành tiền
            decimal tongSoLuong = chiTietList.Sum(ct => ct.SoLuongHoanUng);
            lblTongSoLuong.Text = $"Tổng số lượng: {tongSoLuong:N2}";
            lblSoLoaiVT.Text = $"Số loại vật tư: {chiTietList.Count}";
        }

        private void LoadChiTietVatTu()
        {
            dgvChiTiet.Rows.Clear();
            
            foreach (var ct in chiTietList)
            {
                dgvChiTiet.Rows.Add(
                    ct.MaVT,
                    ct.TenVT,
                    ct.SoLuongHoanUng.ToString("N2"),
                    ct.DVT,
                    ct.TonKho.ToString("N2")
                );

                // Tô màu cảnh báo nếu số lượng > tồn kho
                var row = dgvChiTiet.Rows[dgvChiTiet.Rows.Count - 1];
                if (ct.SoLuongHoanUng > ct.TonKho)
                {
                    row.DefaultCellStyle.BackColor = Color.LightPink;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else if (ct.TonKho == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                }
            }
        }

        private void BtnHoanUng_Click(object sender, EventArgs e)
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
                    this.Close(); // Đóng form sau khi hoàn ứng thành công
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hoàn ứng không thành công!\n\nChi tiết lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement export to Excel
                MessageBox.Show("Chức năng xuất Excel sẽ được implement sau.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}