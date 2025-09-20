using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;
using QLVT.ERP.BLL;
using QLVT.ERP.Models;

namespace QLVT.GUI
{
    /// <summary>
    /// UserControl để hoàn ứng BGK từ hệ thống ERP
    /// </summary>
    public partial class HoanUngBGKUserControl : UserControl
    {
        private readonly ERPBgkBLL _erpBgkBLL;
        private NghiemThuGiaoKhoanModel? currentBGK;
        private List<NghiemThuGiaoKhoanCTModel>? currentVatTuList;

        public HoanUngBGKUserControl()
        {
            InitializeComponent();
            _erpBgkBLL = new ERPBgkBLL();
            SetupDataGridView();
            // Khởi tạo trạng thái ban đầu
            lblConnectionStatus.Text = "🔄 Đang kiểm tra kết nối ERP...";
            lblConnectionStatus.ForeColor = Color.Blue;
            
            // Kiểm tra kết nối ERP khi load
            CheckERPConnection();
        }

        private void CheckERPConnection()
        {
            try
            {
                bool isConnected = _erpBgkBLL.TestERPConnection();
                
                if (isConnected)
                {
                    lblConnectionStatus.Text = "✅ Kết nối ERP thành công";
                    lblConnectionStatus.ForeColor = Color.Green;
                }
                else
                {
                    lblConnectionStatus.Text = "❌ Không thể kết nối ERP";
                    lblConnectionStatus.ForeColor = Color.Red;
                    txtSoBGK.Enabled = false;
                    btnTimBGK.Enabled = false;
                }
            }
            catch (Exception)
            {
                lblConnectionStatus.Text = "❌ Lỗi kết nối ERP";
                lblConnectionStatus.ForeColor = Color.Red;
                txtSoBGK.Enabled = false;
                btnTimBGK.Enabled = false;
            }
        }

        private void SetupDataGridView()
        {
            dgvChiTiet.AutoGenerateColumns = false;
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.ReadOnly = true;
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Tạo các cột
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                Width = 50
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VatTuHangHoa",
                HeaderText = "Mã VT",
                DataPropertyName = "VatTuHangHoa",
                Width = 100
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                Width = 200
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                Width = 60
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongHoanUng", 
                HeaderText = "SL hoàn ứng",
                DataPropertyName = "SoLuongHoanUng",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Event để hiển thị STT
            dgvChiTiet.RowsAdded += (s, e) => UpdateSTT();
            dgvChiTiet.RowsRemoved += (s, e) => UpdateSTT();
            dgvChiTiet.DataBindingComplete += (s, e) => UpdateSTT();
        }

        private void UpdateSTT()
        {
            for (int i = 0; i < dgvChiTiet.Rows.Count; i++)
            {
                dgvChiTiet.Rows[i].Cells["STT"].Value = (i + 1).ToString();
            }
        }

        private void btnTimBGK_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSoBGK.Text))
                {
                    MessageBox.Show("Vui lòng nhập số BGK!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoBGK.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNam.Text) || !int.TryParse(txtNam.Text, out int nam))
                {
                    MessageBox.Show("Vui lòng nhập năm hợp lệ!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNam.Focus();
                    return;
                }

                lblStatus.Text = "Đang tìm BGK...";
                lblStatus.ForeColor = Color.Blue;

                // Tìm BGK theo số nghiệm thu và năm
                var bgkList = _erpBgkBLL.GetNghiemThuGiaoKhoanData(int.Parse(txtSoBGK.Text), nam);
                
                if (bgkList != null && bgkList.Count > 0)
                {
                    currentBGK = bgkList.First();
                    LoadBGKData();
                    lblStatus.Text = $"✅ Đã tải BGK {currentBGK.SoBGK}";
                    lblStatus.ForeColor = Color.Green;
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy BGK số {txtSoBGK.Text}-{nam}", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetBGKDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm BGK:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ResetBGKDisplay();
            }
        }

        private void LoadBGKData()
        {
            if (currentBGK == null) return;

            // Hiển thị thông tin BGK
            DisplayBGKInfo();

            // Load chi tiết vật tư
            if (currentBGK.GiaoKhoanNghiemThuVatTuID.HasValue)
            {
                currentVatTuList = _erpBgkBLL.GetNghiemThuGiaoKhoanCTData(currentBGK.GiaoKhoanNghiemThuVatTuID.Value);
                dgvChiTiet.DataSource = currentVatTuList;
            }

            // Cập nhật trạng thái button
            btnXacNhan.Enabled = currentBGK.DaHoanUng != true;
        }

        private void DisplayBGKInfo()
        {
            if (currentBGK == null) return;

            lblSoBGK.Text = currentBGK.SoBGK ?? "-";
            lblNhanVienKyThuat.Text = currentBGK.NhanVienKyThuat ?? "-";
            lblNhanVienXayLap.Text = currentBGK.NhanVienXayLap ?? "-";
            lblNoiDung.Text = currentBGK.NoiDung ?? "-";
            
            // Hiển thị đầy đủ thông tin nghiệm thu với số lần
            var soNghiemThuText = $"{currentBGK.SoNghiemThu}-{currentBGK.NamNghiemThu}";
            if (currentBGK.SoLanNghiemThu.HasValue)
            {
                soNghiemThuText += $" (Lần {currentBGK.SoLanNghiemThu})";
            }
            lblSoNghiemThu.Text = soNghiemThuText;
            
            if (currentBGK.DaHoanUng == true)
            {
                lblTrangThai.Text = "Đã hoàn ứng";
                lblTrangThai.ForeColor = Color.Green;
                lblNgayHoanUng.Text = currentBGK.NgayHoanUng?.ToString("dd/MM/yyyy") ?? "-";
            }
            else
            {
                lblTrangThai.Text = "Chưa hoàn ứng";
                lblTrangThai.ForeColor = Color.Red;
                lblNgayHoanUng.Text = "-";
            }
        }

        private void ResetBGKDisplay()
        {
            currentBGK = null;
            currentVatTuList = null;
            
            lblSoBGK.Text = "-";
            lblNhanVienKyThuat.Text = "-";
            lblNhanVienXayLap.Text = "-";
            lblNoiDung.Text = "-";
            lblSoNghiemThu.Text = "-";
            lblTrangThai.Text = "-";
            lblTrangThai.ForeColor = Color.Black;
            lblNgayHoanUng.Text = "-";
            
            dgvChiTiet.DataSource = null;
            btnXacNhan.Enabled = false;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentBGK == null)
                {
                    MessageBox.Show("Chưa có BGK nào để xác nhận!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentBGK.DaHoanUng == true)
                {
                    MessageBox.Show("BGK này đã được hoàn ứng!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmResult = MessageBox.Show(
                    $"Xác nhận hoàn ứng BGK {currentBGK.SoBGK}?\n" +
                    $"Nhân viên kỹ thuật: {currentBGK.NhanVienKyThuat}\n" +
                    $"Nhân viên xây lắp: {currentBGK.NhanVienXayLap}\n" +
                    $"Nội dung: {currentBGK.NoiDung}",
                    "Xác nhận hoàn ứng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    lblStatus.Text = "⏳ Đang xử lý hoàn ứng...";
                    lblStatus.ForeColor = Color.Blue;
                    btnXacNhan.Enabled = false;

                    // Cập nhật trạng thái hoàn ứng trong ERP
                    _erpBgkBLL.UpdateNghiemThuGiaoKhoanHoanUng(
                        currentBGK.GiaoKhoanNghiemThuVatTuID!.Value,
                        DateTime.Now,
                        true,
                        DateTime.Now);

                    MessageBox.Show($"✅ Hoàn ứng BGK thành công!", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    lblStatus.Text = $"✅ Hoàn ứng thành công BGK {currentBGK.SoBGK}";
                    lblStatus.ForeColor = Color.Green;
                    
                    // Reload để cập nhật trạng thái
                    LoadBGKData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý hoàn ứng:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                btnXacNhan.Enabled = currentBGK?.DaHoanUng != true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            CheckERPConnection();
            
            if (!string.IsNullOrEmpty(txtSoBGK.Text.Trim()) && 
                !string.IsNullOrEmpty(txtNam.Text.Trim()) && 
                int.TryParse(txtNam.Text.Trim(), out int nam))
            {
                btnTimBGK_Click(sender, e);
            }
        }

        private void txtSoBGK_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            
            // Enter để tìm BGK
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnTimBGK_Click(sender, e);
            }
        }

        private void txtNam_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            
            // Enter để tìm BGK
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnTimBGK_Click(sender, e);
            }
        }
    }
}