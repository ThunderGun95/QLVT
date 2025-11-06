using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;
using QLVT.ERP.Models;

namespace QLVT.GUI
{
    public partial class XuatKhoErpUserControl : UserControl
    {
        private readonly XuatKhoBLL exportBLL;
        private ERP_PhieuXuatKho? currentOrder;
        private int selectedEmployeeWarehouseId = 0;

        public XuatKhoErpUserControl()
        {
            InitializeComponent();
            exportBLL = new XuatKhoBLL();
            SetupDataGridView();
            CheckERPConnection();
        }

        private void CheckERPConnection()
        {
            if (!exportBLL.TestERPConnection())
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
                Name = "MappedSupplyCode",
                HeaderText = "Mã VT",
                DataPropertyName = "MappedSupplyCode",
                Width = 100
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư ERP",
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
                Width = 60
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "KhoXuatDisplay",
                HeaderText = "Kho xuất",
                DataPropertyName = "KhoXuatDisplay",
                Width = 120
            });

            

            dgvChiTiet.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "IsMapped",
                HeaderText = "Mapped",
                DataPropertyName = "IsMapped",
                Width = 70
            });

            // Set row numbering
            dgvChiTiet.RowPostPaint += (sender, e) =>
            {
                var grid = sender as DataGridView;
                var rowIdx = (e.RowIndex + 1).ToString();

                var centerFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, 
                    grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, grid.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            };

            dgvChiTiet.Columns["TenVatTu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnTimPhieu_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSoPhieu.Text))
                {
                    MessageBox.Show("Vui lòng nhập số phiếu!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoPhieu.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNam.Text) || !int.TryParse(txtNam.Text, out int nam))
                {
                    MessageBox.Show("Vui lòng nhập năm hợp lệ!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNam.Focus();
                    return;
                }

                lblStatus.Text = "Đang tìm phiếu xuất...";
                lblStatus.ForeColor = Color.Blue;
                Application.DoEvents();

                currentOrder = exportBLL.GetExportOrderWithMapping(txtSoPhieu.Text.Trim(), nam);
                
                if (currentOrder != null)
                {
                    LoadOrderData();
                    UpdateMappingStatus();
                    lblStatus.Text = $"✅ Đã tải thành công phiếu {currentOrder.SoPhieuDayDu}";
                    lblStatus.ForeColor = Color.Green;
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy phiếu xuất {txtSoPhieu.Text.Trim()}-{nam}", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetOrderDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm phiếu:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                ResetOrderDisplay();
            }
        }

        private void LoadOrderData()
        {
            if (currentOrder == null) return;

            // Load thông tin phiếu
            lblSoPhieu.Text = currentOrder.SoPhieuDayDu;
            
            // Hiển thị thông tin kho xuất (có thể nhiều kho)
            var uniqueWarehouses = currentOrder.ChiTiet
                .Where(c => !string.IsNullOrEmpty(c.TenKhoXuat))
                .Select(c => c.TenKhoXuat)
                .Distinct()
                .ToList();
            
            if (uniqueWarehouses.Any())
            {
                lblKhoXuat.Text = uniqueWarehouses.Count == 1 
                    ? uniqueWarehouses.First() 
                    : $"Nhiều kho ({uniqueWarehouses.Count} kho)";
            }
            else
            {
                lblKhoXuat.Text = "Chưa xác định kho xuất";
            }
            
            lblNguoiTao.Text = $"{currentOrder.TenNhanVien} ({currentOrder.MaNhanVien})";
            lblTrangThai.Text = "Chờ xử lý";
            lblTrangThai.ForeColor = Color.Orange;

            // Load chi tiết
            if (currentOrder.ChiTiet != null && currentOrder.ChiTiet.Any())
            {
                dgvChiTiet.DataSource = currentOrder.ChiTiet;
                UpdateDataGridViewSTT();
            }
            else
            {
                dgvChiTiet.DataSource = null;
            }
        }

        private void UpdateDataGridViewSTT()
        {
            for (int i = 0; i < dgvChiTiet.Rows.Count; i++)
            {
                dgvChiTiet.Rows[i].Cells["STT"].Value = i + 1;
            }
        }

        private void UpdateMappingStatus()
        {
            if (currentOrder == null)
            {
                lblMappingStatus.Text = "Chưa có dữ liệu";
                lblMappingStatus.ForeColor = Color.Gray;
                btnMapping.Enabled = false;
                btnXacNhan.Enabled = false;
                return;
            }

            var (totalItems, mappedItems, unmappedItems, missingWarehouses) = exportBLL.GetMappingStatus(currentOrder);
            
            if (unmappedItems == 0 && missingWarehouses == 0)
            {
                lblMappingStatus.Text = $"✅ Đã mapping đầy đủ: {mappedItems}/{totalItems} vật tư, tất cả đã có kho nguồn";
                lblMappingStatus.ForeColor = Color.Green;
                btnXacNhan.Enabled = true;
            }
            else
            {
                var issues = new List<string>();
                if (unmappedItems > 0) issues.Add($"{unmappedItems} vật tư chưa mapping");
                if (missingWarehouses > 0) issues.Add($"{missingWarehouses} vật tư chưa có kho nguồn");
                
                lblMappingStatus.Text = $"⚠️ {string.Join(", ", issues)}";
                lblMappingStatus.ForeColor = Color.Red;
                btnXacNhan.Enabled = false;
            }
            
            btnMapping.Enabled = totalItems > 0;
        }

        private void btnMapping_Click(object sender, EventArgs e)
        {
            if (currentOrder == null)
            {
                MessageBox.Show("Chưa có phiếu xuất nào để mapping!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var mappingForm = new MappingForm(currentOrder.ChiTiet, exportBLL);
            if (mappingForm.ShowDialog() == DialogResult.OK)
            {
                // Cập nhật lại DataGridView và trạng thái
                dgvChiTiet.Refresh();
                UpdateMappingStatus();
                lblStatus.Text = "✅ Đã cập nhật mapping";
                lblStatus.ForeColor = Color.Green;
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentOrder == null)
                {
                    MessageBox.Show("Chưa có phiếu xuất nào để xác nhận!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var currentUser = AuthenticationBLL.GetCurrentUser();

                // Kiểm tra mapping
                var (_, _, unmappedItems, missingWarehouses) = exportBLL.GetMappingStatus(currentOrder);
                if (unmappedItems > 0)
                {
                    MessageBox.Show($"Còn {unmappedItems} vật tư chưa được mapping!\nVui lòng hoàn thành mapping trước khi xác nhận.", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (missingWarehouses > 0)
                {
                    MessageBox.Show($"Còn {missingWarehouses} vật tư chưa xác định kho nguồn!\nVui lòng liên hệ IT để cập nhật mapping kho.", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tự động lấy kho cá nhân của nhân viên nhận
                if (string.IsNullOrWhiteSpace(currentOrder.MaNhanVien))
                {
                    MessageBox.Show("Phiếu xuất không có thông tin mã nhân viên nhận!\nVui lòng liên hệ IT để kiểm tra dữ liệu ERP.", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var employeeWarehouse = exportBLL.GetWarehouseByStaffCode(currentOrder.MaNhanVien);
                if (employeeWarehouse == null)
                {
                    MessageBox.Show($"Không tìm thấy kho cá nhân cho nhân viên {currentOrder.MaNhanVien} ({currentOrder.TenNhanVien})!\n" +
                        $"Vui lòng liên hệ IT để tạo kho cá nhân cho nhân viên này.", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                selectedEmployeeWarehouseId = employeeWarehouse.Id;
                

                var confirmResult = MessageBox.Show(
                    $"Xác nhận xuất kho phiếu {currentOrder.SoPhieuDayDu}?\n" +
                    $"Người nhận: {currentOrder.TenNhanVien} ({currentOrder.MaNhanVien})\n" +
                    $"Kho đích: {employeeWarehouse.TenKho}\n" +
                    $"Vật tư sẽ được chuyển từ các kho nguồn tương ứng đến kho nhân viên.\n",
                    "Xác nhận xuất kho", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    lblStatus.Text = "⏳ Đang xử lý xuất kho...";
                    lblStatus.ForeColor = Color.Blue;
                    Application.DoEvents();

                    int transactionId = exportBLL.ProcessExport(
                        currentOrder, selectedEmployeeWarehouseId,
                        currentUser!.Username);

                    lblTrangThai.Text = "✅ Đã xuất kho";
                    lblTrangThai.ForeColor = Color.Green;
                    btnXacNhan.Enabled = false;
                    
                    MessageBox.Show($"✅ Xuất kho thành công!\nMã giao dịch: {transactionId}", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    lblStatus.Text = $"✅ Xuất kho thành công - Mã GD: {transactionId}";
                    lblStatus.ForeColor = Color.Green;
                    
                    // Clear form sau khi xuất kho thành công
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý xuất kho:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ResetForm();
            CheckERPConnection();
            lblStatus.Text = "🔄 Đã làm mới";
            lblStatus.ForeColor = Color.Blue;
        }

        private void ResetForm()
        {
            currentOrder = null;
            selectedEmployeeWarehouseId = 0;
            txtSoPhieu.Clear();
            txtNam.Text = DateTime.Now.Year.ToString();
            ResetOrderDisplay();
        }

        private void ResetOrderDisplay()
        {
            lblSoPhieu.Text = "-";
            lblKhoXuat.Text = "-";
            lblNguoiTao.Text = "-";
            lblTrangThai.Text = "-";
            lblTrangThai.ForeColor = Color.Black;
            
            dgvChiTiet.DataSource = null;
            UpdateMappingStatus();
        }

        private void txtSoPhieu_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnTimPhieu_Click(sender, e);
            }
        }

        private void txtNam_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnTimPhieu_Click(sender, e);
            }
            
            // Chỉ cho phép nhập số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
