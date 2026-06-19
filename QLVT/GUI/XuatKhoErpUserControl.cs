using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;
using QLVT.ERP.Models;
using QLVT.DAL;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class XuatKhoErpUserControl : UserControl
    {
        private const int PagePadding = 18;

        private readonly XuatKhoBLL exportBLL;
        private readonly WarehouseDAL warehouseDAL;
        private ERP_PhieuXuatKho? currentOrder;
        private int selectedEmployeeWarehouseId = 0;

        public XuatKhoErpUserControl()
        {
            InitializeComponent();
            ApplyModernStyle();
            exportBLL = new XuatKhoBLL();
            warehouseDAL = new WarehouseDAL();
            SetupDataGridView();
            CheckERPConnection();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "XUẤT KHO ERP";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            grpTimKiem.Text = "Tìm kiếm phiếu xuất";
            grpPhieuInfo.Text = "Thông tin phiếu";
            grpChiTiet.Text = "Chi tiết vật tư";
            foreach (var groupBox in new[] { grpTimKiem, grpPhieuInfo, grpChiTiet })
            {
                UIStyleHelper.ApplyGroupBoxStyle(groupBox);
            }

            lblSoPhieuTimLabel.Text = "Số phiếu";
            lblSoPhieuLabel.Text = "Số phiếu";
            lblKhoXuatLabel.Text = "Kho xuất";
            lblNguoiTaoLabel.Text = "Người nhận";
            lblTrangThaiLabel.Text = "Trạng thái";
            foreach (var label in new[]
            {
                lblSoPhieuTimLabel, lblSeparator, lblConnectionStatus,
                lblSoPhieuLabel, lblSoPhieu, lblKhoXuatLabel, lblKhoXuat,
                lblNguoiTaoLabel, lblNguoiTao, lblTrangThaiLabel, lblTrangThai,
                lblMappingStatus
            })
            {
                UIStyleHelper.ApplyStandardLabelStyle(label);
            }

            UIStyleHelper.ApplyTextBoxStyle(txtSoPhieu);
            UIStyleHelper.ApplyTextBoxStyle(txtNam);

            btnTimPhieu.Text = "Tìm phiếu";
            btnRefresh.Text = "Làm mới";
            btnMapping.Text = "Mapping";
            btnXacNhan.Text = "Xác nhận xuất kho";
            UIStyleHelper.ApplyPrimaryButtonStyle(btnTimPhieu, new Size(106, 34));
            UIStyleHelper.ApplySecondaryButtonStyle(btnRefresh, new Size(100, 34));
            UIStyleHelper.ApplyWarningButtonStyle(btnMapping, new Size(108, 34));
            UIStyleHelper.ApplySuccessButtonStyle(btnXacNhan, new Size(176, 42));

            lblStatus.AutoSize = false;
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Padding = new Padding(10, 0, 10, 0);
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus);
            UIStyleHelper.ApplyDataGridViewStyle(dgvChiTiet);

            txtNam.Text = string.IsNullOrWhiteSpace(txtNam.Text) ? DateTime.Now.Year.ToString() : txtNam.Text;

            Resize += (_, _) => LayoutModern();
            LayoutModern();
        }

        private void LayoutModern()
        {
            SuspendLayout();

            var contentTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;
            var contentWidth = Math.Max(760, Width - PagePadding * 2);

            grpTimKiem.Location = new Point(PagePadding, contentTop);
            grpTimKiem.Size = new Size(contentWidth, 82);

            lblSoPhieuTimLabel.Location = new Point(18, 34);
            txtSoPhieu.Location = new Point(82, 30);
            txtSoPhieu.Size = new Size(92, 25);
            lblSeparator.Location = new Point(txtSoPhieu.Right + 8, 33);
            txtNam.Location = new Point(lblSeparator.Right + 8, 30);
            txtNam.Size = new Size(70, 25);
            btnTimPhieu.Location = new Point(txtNam.Right + 14, 26);
            btnRefresh.Location = new Point(btnTimPhieu.Right + 10, 26);
            lblConnectionStatus.Location = new Point(btnRefresh.Right + 20, 34);
            lblConnectionStatus.Size = new Size(Math.Max(260, grpTimKiem.ClientSize.Width - lblConnectionStatus.Left - 18), 22);

            grpPhieuInfo.Location = new Point(PagePadding, grpTimKiem.Bottom + 10);
            grpPhieuInfo.Size = new Size(Math.Max(560, contentWidth - 210), 92);

            lblSoPhieuLabel.Location = new Point(18, 30);
            lblSoPhieu.Location = new Point(102, 30);
            lblKhoXuatLabel.Location = new Point(18, 58);
            lblKhoXuat.Location = new Point(102, 58);
            lblNguoiTaoLabel.Location = new Point(Math.Max(340, grpPhieuInfo.ClientSize.Width / 2), 30);
            lblNguoiTao.Location = new Point(lblNguoiTaoLabel.Right + 14, 30);
            lblTrangThaiLabel.Location = new Point(lblNguoiTaoLabel.Left, 58);
            lblTrangThai.Location = new Point(lblNguoiTao.Location.X, 58);

            btnXacNhan.Location = new Point(PagePadding + contentWidth - btnXacNhan.Width, grpPhieuInfo.Top + 28);

            grpChiTiet.Location = new Point(PagePadding, grpPhieuInfo.Bottom + 10);
            grpChiTiet.Size = new Size(contentWidth, Math.Max(280, Height - grpPhieuInfo.Bottom - 70));

            lblMappingStatus.Location = new Point(16, 28);
            lblMappingStatus.Size = new Size(Math.Max(360, grpChiTiet.ClientSize.Width - 160), 22);
            btnMapping.Location = new Point(grpChiTiet.ClientSize.Width - btnMapping.Width - 16, 22);
            dgvChiTiet.Location = new Point(16, 58);
            dgvChiTiet.Size = new Size(grpChiTiet.ClientSize.Width - 32, grpChiTiet.ClientSize.Height - 74);

            lblStatus.Location = new Point(PagePadding, Height - 42);
            lblStatus.Size = new Size(contentWidth, 28);

            ResumeLayout(false);
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
            dgvChiTiet.Columns.Clear();
            UIStyleHelper.ApplyDataGridViewStyle(dgvChiTiet);
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

            dgvChiTiet.Columns["TenVatTu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvChiTiet.RowHeadersVisible = false;
            dgvChiTiet.DataBindingComplete += (_, _) => UpdateDataGridViewSTT();

            dgvChiTiet.Columns["STT"].HeaderText = "STT";
            dgvChiTiet.Columns["STT"].FillWeight = 5;
            dgvChiTiet.Columns["STT"].ReadOnly = true;
            dgvChiTiet.Columns["STT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvChiTiet.Columns["MappedSupplyCode"].HeaderText = "Mã VT";
            dgvChiTiet.Columns["MappedSupplyCode"].FillWeight = 11;
            dgvChiTiet.Columns["MappedSupplyCode"].ReadOnly = true;

            dgvChiTiet.Columns["TenVatTu"].HeaderText = "Tên vật tư ERP";
            dgvChiTiet.Columns["TenVatTu"].FillWeight = 34;
            dgvChiTiet.Columns["TenVatTu"].ReadOnly = true;

            dgvChiTiet.Columns["SoLuong"].HeaderText = "Số lượng";
            dgvChiTiet.Columns["SoLuong"].FillWeight = 9;
            dgvChiTiet.Columns["SoLuong"].ReadOnly = true;

            dgvChiTiet.Columns["DonViTinh"].HeaderText = "ĐVT";
            dgvChiTiet.Columns["DonViTinh"].FillWeight = 7;
            dgvChiTiet.Columns["DonViTinh"].ReadOnly = true;
            dgvChiTiet.Columns["DonViTinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvChiTiet.Columns["KhoXuatDisplay"].HeaderText = "Kho xuất";
            dgvChiTiet.Columns["KhoXuatDisplay"].FillWeight = 24;
            dgvChiTiet.Columns["KhoXuatDisplay"].ReadOnly = true;

            dgvChiTiet.Columns["IsMapped"].HeaderText = "Mapped";
            dgvChiTiet.Columns["IsMapped"].FillWeight = 10;
            dgvChiTiet.Columns["IsMapped"].ReadOnly = true;
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

                var employeeWarehouse = warehouseDAL.GetKhoUuTienByMaNV(currentOrder.MaNhanVien);
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
