using QLVT.BLL;
using QLVT.DAL;
using QLVT.ERP.Models;
using QLVT.Models;
using QLVT.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLVT.GUI
{
    public partial class NhapKhoErpUserControl : UserControl
    {
        private readonly NhapKhoBLL nhapKhoErpBLL;
        private ERP_PhieuNhapKho? currentOrder;
        private List<Warehouse> warehouses = new();
        private readonly WarehouseDAL warehouseDAL;
        Warehouse? khoNhap = new Warehouse();

        public NhapKhoErpUserControl()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            nhapKhoErpBLL = new NhapKhoBLL();
            warehouseDAL = new WarehouseDAL();
            SetupDataGridView();
            LoadWarehouses();
            CheckERPConnection();
        }

        private void CheckERPConnection()
        {
            if (!nhapKhoErpBLL.TestERPConnection())
            {
                lblConnectionStatus.Text = "❌ Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = Color.Red;
                txtSoPhieu.Enabled = false;
                btnTimPhieu.Enabled = false;
            }
            else
            {
                lblConnectionStatus.Text = "✅ Kết nối ERP thành công";
                lblConnectionStatus.ForeColor = Color.Green;
            }
        }

        private void LoadWarehouses()
        {
            try
            {
                warehouses = nhapKhoErpBLL.GetWarehouses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kho:\n{ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Name = "MaVatTuHangHoa",
                HeaderText = "Mã VT",
                DataPropertyName = "MaVatTuHangHoa",
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
                Name = "SoLuongNhapKho",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuongNhapKho",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                Width = 80
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenNhaSanXuat",
                HeaderText = "Nhà sản xuất",
                DataPropertyName = "TenNhaSanXuat",
                Width = 300
            });

            // Event để hiển thị STT và màu sắc
            dgvChiTiet.RowsAdded += (s, e) => UpdateSTTAndColors();
            dgvChiTiet.RowsRemoved += (s, e) => UpdateSTTAndColors();
            dgvChiTiet.DataBindingComplete += (s, e) => UpdateSTTAndColors();
        }

        private void UpdateSTTAndColors()
        {
            for (int i = 0; i < dgvChiTiet.Rows.Count; i++)
            {
                var row = dgvChiTiet.Rows[i];
                row.Cells["STT"].Value = (i + 1).ToString();

                // Tô màu dòng theo trạng thái mapping
                if (row.DataBoundItem is ERP_PhieuNhapKhoChiTiet detail)
                {
                    if (detail.IsMapped)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.LightPink;
                    }
                }
            }
        }

        private void btnTimPhieu_Click(object sender, EventArgs e)
        {
            string soPhieu = txtSoPhieu.Text.Trim();
            string nam = txtNam.Text.Trim();
            
            if (string.IsNullOrEmpty(soPhieu))
            {
                MessageBox.Show("Vui lòng nhập số phiếu!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoPhieu.Focus();
                return;
            }

            if (string.IsNullOrEmpty(nam) || !int.TryParse(nam, out int year))
            {
                MessageBox.Show("Vui lòng nhập năm hợp lệ!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNam.Focus();
                return;
            }

            LoadImportOrder(soPhieu, year);
        }

        private async void LoadImportOrder(string soPhieu, int nam)
        {
            try
            {
                lblStatus.Text = "Đang tải phiếu nhập...";
                lblStatus.ForeColor = Color.Blue;
                btnXacNhan.Enabled = false;

                currentOrder = nhapKhoErpBLL.GetPhieuNhapKhoErpWithMapping(soPhieu, nam);

                if (currentOrder == null)
                {
                    lblStatus.Text = $"❌ Không tìm thấy phiếu nhập {soPhieu}-{nam}";
                    lblStatus.ForeColor = Color.Red;
                    ClearOrderInfo();
                    return;
                }

                // Hiển thị thông tin phiếu
                await DisplayOrderInfo(currentOrder);

                // Hiển thị chi tiết
                dgvChiTiet.DataSource = currentOrder.ChiTiet;

                // Kiểm tra mapping status
                UpdateMappingStatus();

                lblStatus.Text = "✅ Đã tải phiếu nhập thành công";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Lỗi khi tải phiếu nhập:\n{ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearOrderInfo();
            }
        }

        private void UpdateMappingStatus()
        {
            if (currentOrder == null) return;

            var (total, mapped, unmapped) = nhapKhoErpBLL.GetMappingStatus(currentOrder);
            
            lblMappingStatus.Text = $"Mapping: {mapped}/{total} vật tư ({unmapped} chưa map)";
            
            if (unmapped == 0)
            {
                lblMappingStatus.ForeColor = Color.Green;
                btnXacNhan.Enabled = true; // Bỏ điều kiện cmbKho
            }
            else
            {
                lblMappingStatus.ForeColor = Color.Orange;
                btnXacNhan.Enabled = false;
            }
        }

        private async Task DisplayOrderInfo(ERP_PhieuNhapKho order)
        {
            lblSoPhieu.Text = $"{order.SoPhieuNhapKho}-{order.NAM}"; // Hiển thị đầy đủ số-năm
            lblNgayTao.Text = order.ThoiGianHoanThanhNhapKho.ToString("dd/MM/yyyy HH:mm");
            
            // Sử dụng warehouse mapping để hiển thị kho đích
            string MaKhoNhap = ERPWarehouseMapping.MapERPToQLVT(order.MaKhoVatTu, order.ChiTiet);
            khoNhap = await warehouseDAL.GetWarehouseByMaKhoAsync(MaKhoNhap);
            lblKhoNhap.Text = $"{khoNhap!.TenKho} ({khoNhap.MaKho})";
            
            lblNguoiTao.Text = order.NhanVienMua;
            lblTrangThai.Text = "Hoàn thành"; // Vì đã filter TrangThai = 'HoanThanh'
        }

        private void ClearOrderInfo()
        {
            currentOrder = null;
            lblSoPhieu.Text = "-";
            lblNgayTao.Text = "-";
            lblKhoNhap.Text = "-"; // Thay vì lblNhaCungCap
            lblNguoiTao.Text = "-";
            lblTrangThai.Text = "-";
            // Bỏ txtGhiChu.Text = "";
            lblMappingStatus.Text = "Chưa có dữ liệu";
            lblMappingStatus.ForeColor = Color.Black;
            dgvChiTiet.DataSource = null;
            btnXacNhan.Enabled = false;
            // Bỏ cmbKho.SelectedIndex = -1;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (currentOrder == null)
                return;

            var result = MessageBox.Show(
                $"Xác nhận nhập kho?\n\n" +
                $"Số phiếu: {currentOrder.SoPhieuDayDu}\n" +
                $"Kho ERP: {currentOrder.MaKhoVatTu}\n" +
                $"Kho QLVT: {khoNhap!.TenKho} ({khoNhap!.MaKho})\n" +
                $"Số mặt hàng: {currentOrder.ChiTiet.Count(d => d.IsMapped)}",
                "Xác nhận nhập kho",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ProcessImportOrder(khoNhap);
            }
        }

        private void ProcessImportOrder(Warehouse warehouse)
        {
            try
            {
                lblStatus.Text = "Đang xử lý nhập kho...";
                lblStatus.ForeColor = Color.Blue;
                btnXacNhan.Enabled = false;

                var currentUser = AuthenticationBLL.GetCurrentUser();

                int transactionId = nhapKhoErpBLL.ProcessNhapKhoErp(currentOrder!, khoNhap!.MaKho, currentUser!.Username);

                lblStatus.Text = "✅ Đã nhập kho thành công";
                lblStatus.ForeColor = Color.Green;

                MessageBox.Show($"Đã nhập kho thành công!\n" +
                              $"Mã giao dịch: NK{DateTime.Now:yyyyMMddHH}\n" +
                              $"Phiếu ERP: {currentOrder?.SoPhieuDayDu}", 
                              "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear form sau khi nhập thành công
                txtSoPhieu.Text = "";
                ClearOrderInfo();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi nhập kho: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Lỗi khi nhập kho:\n{ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnXacNhan.Enabled = true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            CheckERPConnection();
            LoadWarehouses();
            
            if (!string.IsNullOrEmpty(txtSoPhieu.Text.Trim()) && 
                !string.IsNullOrEmpty(txtNam.Text.Trim()) && 
                int.TryParse(txtNam.Text.Trim(), out int year))
            {
                LoadImportOrder(txtSoPhieu.Text.Trim(), year);
            }
        }

        private void btnMapping_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.CurrentRow?.DataBoundItem is ERP_PhieuNhapKhoChiTiet detail && !detail.IsMapped)
            {
                // TODO: Mở form mapping thủ công
                MessageBox.Show($"Chức năng mapping thủ công cho vật tư:\n{detail.TenVatTu}\nSẽ được phát triển trong phiên bản tiếp theo.", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtSoPhieu_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            
            // Enter để tìm phiếu
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnTimPhieu_Click(sender, e);
            }
        }

        private void txtNam_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            
            // Enter để tìm phiếu
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnTimPhieu_Click(sender, e);
            }
        }
    }
}
