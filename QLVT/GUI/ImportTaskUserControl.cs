using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class ImportTaskUserControl : UserControl
    {
        private readonly ImportBLL importBLL;
        private ERPImportOrder? currentOrder;
        private List<Warehouse> warehouses = new();

        public ImportTaskUserControl()
        {
            InitializeComponent();
            importBLL = new ImportBLL();
            SetupDataGridView();
            LoadWarehouses();
            CheckERPConnection();
        }

        private void CheckERPConnection()
        {
            if (!importBLL.TestERPConnection())
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
                warehouses = importBLL.GetWarehouses();
                
                cmbKho.DataSource = warehouses;
                cmbKho.DisplayMember = "TenKho";
                cmbKho.ValueMember = "Id";
                cmbKho.SelectedIndex = -1;
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
                Name = "MaVatTu",
                HeaderText = "Mã VT",
                DataPropertyName = "MaVatTu",
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
                Name = "SoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong",
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
                Name = "NhaSanXuat",
                HeaderText = "Nhà sản xuất",
                DataPropertyName = "NhaSanXuat",
                Width = 80
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
                if (row.DataBoundItem is ERPImportOrderDetail detail)
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

        private void LoadImportOrder(string soPhieu, int nam)
        {
            try
            {
                lblStatus.Text = "Đang tải phiếu nhập...";
                lblStatus.ForeColor = Color.Blue;
                btnXacNhan.Enabled = false;

                currentOrder = importBLL.GetImportOrderWithMapping(soPhieu, nam);

                if (currentOrder == null)
                {
                    lblStatus.Text = $"❌ Không tìm thấy phiếu nhập {soPhieu}-{nam}";
                    lblStatus.ForeColor = Color.Red;
                    ClearOrderInfo();
                    return;
                }

                // Hiển thị thông tin phiếu
                DisplayOrderInfo(currentOrder);

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

            var (total, mapped, unmapped) = importBLL.GetMappingStatus(currentOrder);
            
            lblMappingStatus.Text = $"Mapping: {mapped}/{total} vật tư ({unmapped} chưa map)";
            
            if (unmapped == 0)
            {
                lblMappingStatus.ForeColor = Color.Green;
                btnXacNhan.Enabled = cmbKho.SelectedIndex >= 0;
            }
            else
            {
                lblMappingStatus.ForeColor = Color.Orange;
                btnXacNhan.Enabled = false;
            }
        }

        private void DisplayOrderInfo(ERPImportOrder order)
        {
            lblSoPhieu.Text = order.SoPhieu;
            lblNgayTao.Text = order.NgayTao.ToString("dd/MM/yyyy HH:mm");
            lblNguoiTao.Text = order.NguoiTao;
            lblTrangThai.Text = order.TrangThai;
            txtGhiChu.Text = order.GhiChu;
        }

        private void ClearOrderInfo()
        {
            currentOrder = null;
            lblSoPhieu.Text = "-";
            lblNgayTao.Text = "-";
            lblNhaCungCap.Text = "-";
            lblNguoiTao.Text = "-";
            lblTrangThai.Text = "-";
            txtGhiChu.Text = "";
            lblMappingStatus.Text = "Chưa có dữ liệu";
            lblMappingStatus.ForeColor = Color.Black;
            dgvChiTiet.DataSource = null;
            btnXacNhan.Enabled = false;
            cmbKho.SelectedIndex = -1;
        }

        private void cmbKho_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMappingStatus();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (currentOrder == null || cmbKho.SelectedValue == null)
                return;

            var selectedWarehouse = warehouses.FirstOrDefault(w => w.Id == (int)cmbKho.SelectedValue);
            if (selectedWarehouse == null)
                return;

            var result = MessageBox.Show(
                $"Xác nhận nhập kho?\n\n" +
                $"Số phiếu: {currentOrder.SoPhieu}\n" +
                $"Kho đích: {selectedWarehouse.TenKho}\n" +
                $"Số mặt hàng: {currentOrder.ChiTiet.Count(d => d.IsMapped)}",
                "Xác nhận nhập kho",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ProcessImportOrder();
            }
        }

        private void ProcessImportOrder()
        {
            try
            {
                lblStatus.Text = "Đang xử lý nhập kho...";
                lblStatus.ForeColor = Color.Blue;
                btnXacNhan.Enabled = false;

                int warehouseId = (int)cmbKho.SelectedValue;
                string createdBy = "admin"; // TODO: Lấy từ session
                string staffCode = "NV001"; // TODO: Lấy từ session hoặc chọn

                int transactionId = importBLL.ProcessImport(currentOrder!, warehouseId, createdBy, staffCode);

                lblStatus.Text = "✅ Đã nhập kho thành công";
                lblStatus.ForeColor = Color.Green;

                MessageBox.Show($"Đã nhập kho thành công!\n" +
                              $"Mã giao dịch: NK{DateTime.Now:yyyyMMddHHmmss}\n" +
                              $"Phiếu ERP: {currentOrder?.SoPhieu}", 
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
            if (dgvChiTiet.CurrentRow?.DataBoundItem is ERPImportOrderDetail detail && !detail.IsMapped)
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
