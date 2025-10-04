using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class XuatKhoUserControl : UserControl
    {
        private readonly XuatKhoManualBLL xuatKhoBLL;
        private PhieuXuatKho? currentPhieu;
        private List<PhieuXuatKhoChiTiet> chiTietList;
        private List<VatTuSearchResult> searchResults;

        public XuatKhoUserControl()
        {
            InitializeComponent();
            xuatKhoBLL = new XuatKhoManualBLL();
            chiTietList = new List<PhieuXuatKhoChiTiet>();
            searchResults = new List<VatTuSearchResult>();
            
            InitializeForm();
            SetupDataGridView();
            LoadDanhSachKho();
        }

        private void InitializeForm()
        {
            // Không hiển thị số phiếu và ngày tạo - sẽ tạo khi lưu
            // Thiết lập các giá trị mặc định
            txtLyDoXuat.Multiline = true;
            txtLyDoXuat.ScrollBars = ScrollBars.Vertical;
            txtLyDoXuat.Height = 60;
            
            txtGhiChu.Multiline = true;
            txtGhiChu.ScrollBars = ScrollBars.Vertical;
            txtGhiChu.Height = 60;
            
            // Thiết lập ngày ghi nhận sổ sách mặc định là hôm nay
            dtpNgayGhiNhan.Value = DateTime.Now;
        }

        private void LoadDanhSachKho()
        {
            try
            {
                var danhSachKho = xuatKhoBLL.GetDanhSachKho();
                
                // Setup kho xuất với AutoComplete
                SetupKhoComboBox(cboKhoXuat, danhSachKho.ToList());
                
                // Setup kho nhận với AutoComplete
                SetupKhoComboBox(cboKhoNhan, danhSachKho.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load danh sách kho: {ex.Message}", "Lỗi", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupKhoComboBox(ComboBox cbo, List<Warehouse> danhSachKho)
        {
            cbo.DataSource = danhSachKho;
            cbo.DisplayMember = "TenKho";
            cbo.ValueMember = "Id";
            
            // Thiết lập AutoComplete cho tìm kiếm trực tiếp trong ComboBox
            cbo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbo.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbo.DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void SetupDataGridView()
        {
            // Thiết lập DataGridView chỉ hiển thị chi tiết phiếu xuất
            dgvChiTietPhieu.AutoGenerateColumns = false;
            dgvChiTietPhieu.AllowUserToAddRows = false;
            dgvChiTietPhieu.AllowUserToDeleteRows = true;
            dgvChiTietPhieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvChiTietPhieu.Columns.Clear();

            // Mã vật tư
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaVT",
                DataPropertyName = "MaVatTu",
                HeaderText = "Mã VT",
                Width = 100,
                ReadOnly = true
            });

            // Tên vật tư
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVT", 
                DataPropertyName = "TenVatTu",
                HeaderText = "Tên vật tư",
                Width = 250,
                ReadOnly = true
            });

            // Đơn vị tính
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DVT",
                DataPropertyName = "DonViTinh", 
                HeaderText = "ĐVT",
                Width = 80,
                ReadOnly = true
            });

            // Số lượng tồn (chỉ hiển thị để tham khảo)
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongTon",
                DataPropertyName = "SoLuongTon",
                HeaderText = "SL tồn",
                Width = 100,
                ReadOnly = true
            });

            // Số lượng xuất (có thể sửa)
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                DataPropertyName = "SoLuong",
                HeaderText = "SL xuất",
                Width = 100,
                ReadOnly = false
            });

            // Ghi chú (có thể sửa)
            dgvChiTietPhieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GhiChu",
                DataPropertyName = "GhiChu",
                HeaderText = "Ghi chú",
                Width = 200,
                ReadOnly = false
            });

            // Cột nút xóa
            var deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "Xóa",
                Text = "✖", // Dấu X đẹp
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            deleteColumn.DefaultCellStyle.BackColor = Color.FromArgb(220, 53, 69); // Màu đỏ
            deleteColumn.DefaultCellStyle.ForeColor = Color.White;
            deleteColumn.DefaultCellStyle.Font = new Font("Arial", 10F, FontStyle.Bold);
            dgvChiTietPhieu.Columns.Add(deleteColumn);

            // Bind events
            dgvChiTietPhieu.CellValueChanged += DgvChiTietPhieu_CellValueChanged;
            dgvChiTietPhieu.KeyDown += DgvChiTietPhieu_KeyDown;
            dgvChiTietPhieu.CellClick += DgvChiTietPhieu_CellClick;
        }

        private void DgvChiTietPhieu_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dgvChiTietPhieu.Rows[e.RowIndex];
            var columnName = dgvChiTietPhieu.Columns[e.ColumnIndex].Name;

            if (columnName == "SoLuong")
            {
                ValidateSoLuong(e.RowIndex);
            }
        }

        private void DgvChiTietPhieu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dgvChiTietPhieu.SelectedRows.Count > 0)
            {
                XoaVatTuKhoiPhieu();
            }
        }

        private void DgvChiTietPhieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Xử lý click vào nút xóa
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && 
                dgvChiTietPhieu.Columns[e.ColumnIndex].Name == "Delete")
            {
                dgvChiTietPhieu.Rows[e.RowIndex].Selected = true;
                XoaVatTuKhoiPhieu();
            }
        }

        private void XoaVatTuKhoiPhieu()
        {
            if (dgvChiTietPhieu.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Bạn có muốn xóa vật tư này khỏi phiếu?", "Xác nhận",
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Xóa từ cao xuống thấp để tránh lỗi index
                    var rowsToDelete = dgvChiTietPhieu.SelectedRows.Cast<DataGridViewRow>()
                                     .OrderByDescending(r => r.Index).ToList();
                    
                    foreach (DataGridViewRow row in rowsToDelete)
                    {
                        if (row.Index < chiTietList.Count)
                        {
                            chiTietList.RemoveAt(row.Index);
                        }
                    }
                    RefreshDataGridView();
                }
            }
        }

        private void ValidateSoLuong(int rowIndex)
        {
            try
            {
                var row = dgvChiTietPhieu.Rows[rowIndex];
                var soLuongCell = row.Cells["SoLuong"];
                var soLuongTonCell = row.Cells["SoLuongTon"];

                if (decimal.TryParse(soLuongCell.Value?.ToString(), out decimal soLuongXuat))
                {
                    if (decimal.TryParse(soLuongTonCell.Value?.ToString(), out decimal soLuongTon))
                    {
                        bool needUpdate = false;
                        decimal newValue = soLuongXuat;

                        if (soLuongXuat > soLuongTon)
                        {
                            MessageBox.Show($"Số lượng xuất ({soLuongXuat}) không được vượt quá số lượng tồn ({soLuongTon})!",
                                          "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            newValue = soLuongTon;
                            needUpdate = true;
                        }
                        else if (soLuongXuat <= 0)
                        {
                            MessageBox.Show("Số lượng xuất phải lớn hơn 0!", "Cảnh báo", 
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            newValue = 1;
                            needUpdate = true;
                        }

                        // Tạm thời tắt event để tránh vòng lặp
                        if (needUpdate)
                        {
                            dgvChiTietPhieu.CellValueChanged -= DgvChiTietPhieu_CellValueChanged;
                            soLuongCell.Value = newValue;
                            dgvChiTietPhieu.CellValueChanged += DgvChiTietPhieu_CellValueChanged;
                        }
                    }
                }
                
                // Cập nhật vào chiTietList
                if (rowIndex < chiTietList.Count)
                {
                    chiTietList[rowIndex].SoLuong = decimal.Parse(soLuongCell.Value?.ToString() ?? "0");
                    chiTietList[rowIndex].GhiChu = row.Cells["GhiChu"].Value?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi validate số lượng: {ex.Message}", "Lỗi", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGridView()
        {
            dgvChiTietPhieu.DataSource = null;
            dgvChiTietPhieu.DataSource = chiTietList;

            // Cập nhật số lượng tồn cho từng dòng
            for (int i = 0; i < dgvChiTietPhieu.Rows.Count; i++)
            {
                var row = dgvChiTietPhieu.Rows[i];
                if (i < chiTietList.Count)
                {
                    var chiTiet = chiTietList[i];
                    // Sử dụng số lượng tồn thực từ chiTietList
                    row.Cells["SoLuongTon"].Value = chiTiet.SoLuongTon;
                }
            }
        }

        private void SyncChiTietListFromDataGridView()
        {
            chiTietList.Clear();
            foreach (DataGridViewRow row in dgvChiTietPhieu.Rows)
            {
                if (row.IsNewRow) continue;
                
                var chiTiet = new PhieuXuatKhoChiTiet
                {
                    ErpId = Convert.ToInt32(row.Cells["ErpId"].Value),
                    MaVatTu = row.Cells["MaVatTu"].Value?.ToString() ?? "",
                    TenVatTu = row.Cells["TenVatTu"].Value?.ToString() ?? "",
                    DonViTinh = row.Cells["DonViTinh"].Value?.ToString() ?? "", 
                    SoLuongTon = Convert.ToDecimal(row.Cells["SoLuongTon"].Value ?? 0),
                    SoLuong = Convert.ToDecimal(row.Cells["SoLuong"].Value ?? 0),
                    GhiChu = row.Cells["GhiChu"].Value?.ToString() ?? ""
                };
                chiTietList.Add(chiTiet);
            }
        }

        private void BtnThemVatTu_Click(object sender, EventArgs e)
        {
            if (cboKhoXuat.SelectedValue == null || Convert.ToInt32(cboKhoXuat.SelectedValue) <= 0)
            {
                MessageBox.Show("Vui lòng chọn kho xuất trước khi thêm vật tư!", "Cảnh báo", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var searchForm = new TimVatTuWithKhoForm(Convert.ToInt32(cboKhoXuat.SelectedValue));
            if (searchForm.ShowDialog() == DialogResult.OK && searchForm.SelectedVatTu != null)
            {
                var vatTu = searchForm.SelectedVatTu;
                
                // Kiểm tra xem vật tư đã được thêm chưa
                if (chiTietList.Any(x => x.ErpId == vatTu.ErpId))
                {
                    MessageBox.Show("Vật tư này đã được thêm vào phiếu!", "Cảnh báo", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Thêm vào danh sách
                var chiTiet = new PhieuXuatKhoChiTiet
                {
                    ErpId = vatTu.ErpId ?? 0,
                    MaVatTu = vatTu.MaVatTu,
                    TenVatTu = vatTu.TenVatTu,
                    DonViTinh = vatTu.DonViTinh,
                    SoLuongTon = vatTu.SoLuongTon,
                    SoLuong = 1m, // Mặc định 1
                    GhiChu = ""
                };

                chiTietList.Add(chiTiet);
                RefreshDataGridView();
                
                // Khóa kho xuất sau khi đã thêm vật tư
                cboKhoXuat.Enabled = false;
            }
        }

        private void BtnTaoMoi_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn tạo phiếu mới? Dữ liệu hiện tại sẽ bị xóa.", "Xác nhận",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                TaoPhieuMoi();
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate form
                if (cboKhoXuat.SelectedValue == null || 
                    !int.TryParse(cboKhoXuat.SelectedValue.ToString(), out int khoXuatId) || 
                    khoXuatId <= 0)
                {
                    MessageBox.Show("Vui lòng chọn kho xuất!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Validation: Kho xuất phải khác kho nhận
                if (cboKhoNhan.SelectedValue != null && 
                    int.TryParse(cboKhoNhan.SelectedValue.ToString(), out int khoNhanId) &&
                    khoXuatId == khoNhanId)
                {
                    MessageBox.Show("Kho xuất và kho nhận phải khác nhau!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra có vật tư trong phiếu (backup kiểm tra cả DataGridView)
                if (chiTietList.Count == 0 && dgvChiTietPhieu.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một vật tư vào phiếu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Sync lại chiTietList từ DataGridView nếu cần (đảm bảo data consistency)
                if (chiTietList.Count == 0 && dgvChiTietPhieu.Rows.Count > 0)
                {
                    SyncChiTietListFromDataGridView();
                }

                var phieu = TaoPhieuTuForm();
                
                var phieuId = xuatKhoBLL.TaoPhieuXuatKho(phieu);
                
                if (phieuId > 0)
                {
                    MessageBox.Show("Lưu phiếu xuất kho thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    phieu.Id = phieuId;
                    currentPhieu = phieu;
                    TaoPhieuMoi(); // Reset form
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi lưu phiếu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaoPhieuMoi()
        {
            // Reset form về trạng thái ban đầu
            txtLyDoXuat.Text = "";
            txtGhiChu.Text = "";
            cboKhoXuat.SelectedIndex = -1;
            cboKhoNhan.SelectedIndex = 0;
            dtpNgayGhiNhan.Value = DateTime.Now; // Reset về ngày hiện tại
            
            // Mở khóa kho xuất
            cboKhoXuat.Enabled = true;
            chiTietList.Clear();
            RefreshDataGridView();
            currentPhieu = null;
        }

        private void CboKhoNhan_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Validate kho xuất phải khác kho nhận
                if (cboKhoXuat.SelectedValue != null && cboKhoNhan.SelectedValue != null)
                {
                    // Kiểm tra kiểu dữ liệu trước khi convert
                    if (int.TryParse(cboKhoXuat.SelectedValue.ToString(), out int khoXuatId) &&
                        int.TryParse(cboKhoNhan.SelectedValue.ToString(), out int khoNhanId))
                    {
                        if (khoXuatId == khoNhanId && khoXuatId > 0)
                        {
                            MessageBox.Show("Kho xuất và kho nhận phải khác nhau!", "Cảnh báo", 
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            cboKhoNhan.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không hiển thị cho user để tránh spam
                System.Diagnostics.Debug.WriteLine($"Lỗi validation kho: {ex.Message}");
            }
        }

        private PhieuXuatKho TaoPhieuTuForm()
        {
            
            // Safe parsing cho kho xuất và kho nhận
            int.TryParse(cboKhoXuat.SelectedValue?.ToString(), out int khoXuatId);
            int.TryParse(cboKhoNhan.SelectedValue?.ToString(), out int khoNhanId);
            
            return new PhieuXuatKho
            {
                NgayGiaoDich = dtpNgayGhiNhan.Value, // Sử dụng ngày ghi nhận sổ sách
                MaKhoNguon = khoXuatId,
                MaKhoNhan = khoNhanId == 0 ? null : khoNhanId,
                MaNV = "Admin", // TODO: Lấy từ user session
                EntityXuatKho = string.IsNullOrWhiteSpace(txtLyDoXuat.Text) ? null : txtLyDoXuat.Text.Trim(),
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim(),
                CreatedBy = "Admin", // TODO: Lấy từ user session
                CreatedDate = DateTime.Now,
                ChiTiet = chiTietList
            };
        }
    }
}