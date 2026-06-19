using QLVT.ERP.Models;
using QLVT.BLL;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class ChiTietVatTuDCForm : Form
    {
        private SuaChuaModel hoSo;
        private List<SuaChuaCTModel> chiTietList;
        private HoanUngBLL hoanUngBLL;

        public ChiTietVatTuDCForm(SuaChuaModel hoSo, List<SuaChuaCTModel> chiTietList)
        {
            InitializeComponent();
            this.hoSo = hoSo;
            this.chiTietList = chiTietList;
            this.hoanUngBLL = new HoanUngBLL();
            SetupDataGridView();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvChiTiet.ReadOnly = false;
            SetupButtonHoverEffects();
            LoadData();
        }

        private void SetupDataGridView()
        {
            dgvChiTiet.AutoGenerateColumns = false;
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.ReadOnly = false; // Cho phép chỉnh sửa
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Thiết lập font truyền thống cho dữ liệu trong grid
            dgvChiTiet.DefaultCellStyle.Font = UIFonts.GridData;
            dgvChiTiet.RowTemplate.DefaultCellStyle.Font = UIFonts.GridData;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Font = UIFonts.GridHeader;
            
            // Căn giữa header cho tất cả các cột
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Xóa các cột cũ nếu có
            dgvChiTiet.Columns.Clear();

            // Tạo các cột
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                Width = 60,
                FillWeight = 5,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaVT",
                HeaderText = "Mã VT",
                DataPropertyName = "MaVT",
                Width = 120,
                FillWeight = 15,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVT",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVT",
                Width = 400,
                FillWeight = 45,
                ReadOnly = true
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DVT",
                HeaderText = "ĐVT",
                DataPropertyName = "DVT",
                Width = 80,
                FillWeight = 10,
                ReadOnly = true
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongHoanUng",
                HeaderText = "SL hoàn ứng",
                DataPropertyName = "SoLuongHoanUngThucTe", // Bind trực tiếp vào SoLuongHoanUngThucTe
                Width = 120,
                FillWeight = 15,
                ReadOnly = false, // Cho phép sửa
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TonKho",
                HeaderText = "Tồn kho",
                DataPropertyName = "TonKho",
                Width = 120,
                FillWeight = 15,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Event để hiển thị STT
            dgvChiTiet.RowsAdded += (s, e) => UpdateSTT();
            dgvChiTiet.RowsRemoved += (s, e) => UpdateSTT();
            dgvChiTiet.DataBindingComplete += (s, e) => UpdateSTT();
            
            // Event để đánh dấu màu cho dòng có SL hoàn ứng > tồn kho
            dgvChiTiet.CellFormatting += DgvChiTiet_CellFormatting;
            
            // Event để validate khi người dùng nhập số lượng
            dgvChiTiet.CellValidating += DgvChiTiet_CellValidating;
            dgvChiTiet.CellEndEdit += DgvChiTiet_CellEndEdit;
        }

        private void UpdateSTT()
        {
            for (int i = 0; i < dgvChiTiet.Rows.Count; i++)
            {
                dgvChiTiet.Rows[i].Cells["STT"].Value = (i + 1).ToString();
            }
        }

        private void SetupButtonHoverEffects()
        {
            // Hover effect cho btnHoanUng (Warning Orange)
            btnHoanUng.MouseEnter += (s, e) => btnHoanUng.BackColor = UIColorPalette.ButtonWarning.Hover;
            btnHoanUng.MouseLeave += (s, e) => btnHoanUng.BackColor = UIColorPalette.ButtonWarning.Base;

            // Hover effect cho btnDong (Secondary)
            btnDong.MouseEnter += (s, e) => btnDong.BackColor = Color.FromArgb(90, 90, 90);
            btnDong.MouseLeave += (s, e) => btnDong.BackColor = Color.FromArgb(108, 117, 125);
        }

        private void LoadData()
        {
            // Hiển thị thông tin hồ sơ
            lblMaDon.Text = hoSo.MADON;
            lblViTriDiemChay.Text = hoSo.ViTriDiemChay;
            lblNVTaiCong.Text = hoSo.NhanVienXayLap;

            // Khởi tạo SoLuongHoanUngThucTe nếu chưa có
            foreach (var ct in chiTietList)
            {
                if (!ct.SoLuongHoanUngThucTe.HasValue)
                {
                    ct.SoLuongHoanUngThucTe = ct.SoLuongHoanUng;
                }
            }

            // Load chi tiết vật tư
            LoadChiTietVatTu();
            
            // Tính tổng
            UpdateTongSoLuong();
        }

        private void UpdateTongSoLuong()
        {
            decimal tongSoLuong = chiTietList.Sum(ct => ct.SoLuongHoanUngThucTe ?? ct.SoLuongHoanUng);
            lblTongSoLuong.Text = $"Tổng số lượng: {tongSoLuong:N2}";
            lblSoLoaiVT.Text = $"Số loại vật tư: {chiTietList.Count}";
        }

        private void LoadChiTietVatTu()
        {
            // Sử dụng DataSource để binding dữ liệu
            dgvChiTiet.DataSource = chiTietList;
        }

        private void DgvChiTiet_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= chiTietList.Count) return;

            try
            {
                var ct = chiTietList[e.RowIndex];
                var soLuongHoanUng = ct.SoLuongHoanUngThucTe ?? ct.SoLuongHoanUng;
                var tonKho = ct.TonKho;

                var row = dgvChiTiet.Rows[e.RowIndex];
                
                // Nếu số lượng hoàn ứng > tồn kho, đánh dấu màu hồng
                if (soLuongHoanUng > tonKho)
                {
                    row.DefaultCellStyle.BackColor = UIColorPalette.StatusWarningBackground;
                    row.DefaultCellStyle.ForeColor = UIColorPalette.StatusWarningText;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = UIColorPalette.BackgroundWhite;
                    row.DefaultCellStyle.ForeColor = UIColorPalette.TextBlack;
                }
            }
            catch
            {
                // Ignore formatting errors
            }
        }

        private void DgvChiTiet_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Chỉ validate cột số lượng hoàn ứng
            if (dgvChiTiet.Columns[e.ColumnIndex].Name != "SoLuongHoanUng")
                return;

            if (e.FormattedValue == null || string.IsNullOrWhiteSpace(e.FormattedValue.ToString()))
                return;

            // Kiểm tra giá trị nhập vào có phải số hợp lệ không
            if (!decimal.TryParse(e.FormattedValue.ToString(), out decimal value))
            {
                e.Cancel = true;
                MessageBox.Show("Vui lòng nhập số hợp lệ!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra số lượng không được âm
            if (value < 0)
            {
                e.Cancel = true;
                MessageBox.Show("Số lượng hoàn ứng không được âm!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra số lượng hoàn ứng <= tồn kho
            try
            {
                var row = dgvChiTiet.Rows[e.RowIndex];
                var tonKhoCell = row.Cells["TonKho"].Value;
                
                if (tonKhoCell != null)
                {
                    decimal tonKho = Convert.ToDecimal(tonKhoCell);
                    
                    if (value > tonKho)
                    {
                        var ct = chiTietList[e.RowIndex];
                        var result = MessageBox.Show(
                            $"⚠️ Số lượng hoàn ứng ({value:N2}) vượt quá tồn kho ({tonKho:N2})!\n\n" +
                            $"Mã vật tư: {ct.MaVT}\n" +
                            $"Tên vật tư: {ct.TenVT}\n\n" +
                            $"Bạn có chắc chắn muốn tiếp tục?",
                            "Cảnh báo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                        
                        if (result == DialogResult.No)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi khi không lấy được tồn kho
            }
        }

        private void DgvChiTiet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Sau khi sửa xong, cập nhật lại màu của dòng
            if (dgvChiTiet.Columns[e.ColumnIndex].Name == "SoLuongHoanUng")
            {
                try
                {
                    // DataGridView đã tự động cập nhật vào SoLuongHoanUngThucTe qua DataPropertyName
                    // Cập nhật tổng số lượng
                    UpdateTongSoLuong();
                    
                    // Chỉ cần refresh row để cập nhật màu
                    dgvChiTiet.InvalidateRow(e.RowIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi cập nhật số lượng: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnHoanUng_Click(object sender, EventArgs e)
        {
            if (hoSo.DaHoanUng == true)
            {
                MessageBox.Show("Hồ sơ này đã được hoàn ứng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Tính tổng số lượng hoàn ứng thực tế
            decimal tongSoLuongThucTe = chiTietList.Sum(ct => ct.SoLuongHoanUngThucTe ?? ct.SoLuongHoanUng);
            
            var result = MessageBox.Show(
                $"Xác nhận hoàn ứng cho hồ sơ:\n\n" +
                $"Mã đơn: {hoSo.MADON}\n" +
                $"Vị trí: {hoSo.ViTriDiemChay}\n" +
                $"NV Thi công: {hoSo.NhanVienXayLap}\n" +
                $"Tổng số lượng: {tongSoLuongThucTe:N2}\n\n" +
                $"Thao tác sẽ thực hiện:\n" +
                $"1. Cập nhật số lượng hoàn ứng thực tế\n" +
                $"2. Lưu chi tiết vật tư hoàn ứng\n" +
                $"3. Tạo giao dịch hoàn ứng\n" +
                $"4. Cập nhật tồn kho\n\n" +
                $"Bạn có chắc chắn muốn thực hiện hoàn ứng?",
                "Xác nhận hoàn ứng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Gọi BLL để xác nhận hoàn ứng với số lượng đã chỉnh sửa
                    await hoanUngBLL.DC_XacNhanHoanUng(hoSo.MADON, chiTietList);
                    
                    MessageBox.Show("Hoàn ứng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hoàn ứng không thành công!\n\nChi tiết lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDong_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
