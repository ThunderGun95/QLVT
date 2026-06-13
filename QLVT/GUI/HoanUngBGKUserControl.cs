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
using QLVT.Utils;

namespace QLVT.GUI
{
    /// <summary>
    /// UserControl để hoàn ứng BGK từ hệ thống ERP
    /// </summary>
    public partial class HoanUngBGKUserControl : UserControl
    {
        private readonly ERPBgkBLL _erpBgkBLL;
        private readonly HoanUngBLL _hoanUngBLL;
        private NghiemThuGiaoKhoanModel? currentBGK;
        private List<NghiemThuGiaoKhoanCTModel>? currentVatTuList;

        public HoanUngBGKUserControl()
        {
            InitializeComponent();
            _erpBgkBLL = new ERPBgkBLL();
            _hoanUngBLL = new HoanUngBLL();
            ApplyModernStyle();
            SetupDataGridView();
            Resize += (_, _) => ArrangeModernLayout();
            // Khởi tạo trạng thái ban đầu
            lblConnectionStatus.Text = "🔄 Đang kiểm tra kết nối ERP...";
            lblConnectionStatus.ForeColor = UIColorPalette.StatusProcessing;

            // Kiểm tra kết nối ERP khi load
            CheckERPConnection();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "HOÀN ỨNG BGK";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            grpTimKiem.Text = "Tìm kiếm BGK";
            grpBGKInfo.Text = "Thông tin BGK";
            grpChiTiet.Text = "Danh sách vật tư hoàn ứng";
            UIStyleHelper.ApplyGroupBoxStyle(grpTimKiem, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            UIStyleHelper.ApplyGroupBoxStyle(grpBGKInfo, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            UIStyleHelper.ApplyGroupBoxStyle(grpChiTiet, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            foreach (var label in new[]
            {
                lblSoBGKTimLabel, lblSeparator, lblSoBGKLabel, lblTrangThaiLabel,
                lblNhanVienKyThuatLabel, lblNhanVienXayLapLabel, lblNoiDungLabel,
                lblSoNghiemThuLabel
            })
            {
                UIStyleHelper.ApplyStandardLabelStyle(label);
            }

            foreach (var valueLabel in new[]
            {
                lblSoBGK, lblTrangThai, lblNhanVienKyThuat, lblNhanVienXayLap,
                lblNoiDung, lblSoNghiemThu
            })
            {
                valueLabel.Font = UIFonts.HeaderStandard;
                valueLabel.ForeColor = UIColorPalette.TextDark;
            }

            UIStyleHelper.ApplyTextBoxStyle(txtSoBGK);
            UIStyleHelper.ApplyTextBoxStyle(txtNam);
            UIStyleHelper.ApplyPrimaryButtonStyle(btnTimBGK, new Size(104, 30));
            UIStyleHelper.ApplySecondaryButtonStyle(btnRefresh, new Size(104, 30));
            UIStyleHelper.ApplyWarningButtonStyle(btnXacNhan, new Size(190, 36));
            btnTimBGK.Text = "Tìm kiếm";
            btnRefresh.Text = "Làm mới";
            btnXacNhan.Text = "Xác nhận hoàn ứng";

            UIStyleHelper.ApplyStatusLabelStyle(lblConnectionStatus, StatusType.Processing);
            lblConnectionStatus.TextAlign = ContentAlignment.MiddleLeft;
            UIStyleHelper.ApplyStandardLabelStyle(lblStatus);

            ArrangeModernLayout();
        }

        private void ArrangeModernLayout()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            const int margin = 20;
            int contentWidth = Math.Max(900, ClientSize.Width - margin * 2);
            int top = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;

            grpTimKiem.Location = new Point(margin, top);
            grpTimKiem.Size = new Size(contentWidth, 90);

            grpBGKInfo.Location = new Point(margin, grpTimKiem.Bottom + 10);
            grpBGKInfo.Size = new Size(contentWidth, 120);

            grpChiTiet.Location = new Point(margin, grpBGKInfo.Bottom + 10);
            grpChiTiet.Size = new Size(contentWidth, Math.Max(300, ClientSize.Height - grpChiTiet.Top - 42));

            dgvChiTiet.Location = new Point(16, 28);
            dgvChiTiet.Size = new Size(Math.Max(700, grpChiTiet.ClientSize.Width - 32), Math.Max(180, grpChiTiet.ClientSize.Height - 82));
            btnXacNhan.Location = new Point(16, grpChiTiet.ClientSize.Height - btnXacNhan.Height - 14);
            lblStatus.Location = new Point(margin + 4, Math.Max(grpChiTiet.Bottom + 8, ClientSize.Height - 26));

            lblNoiDung.MaximumSize = new Size(Math.Max(500, contentWidth - 150), 0);
        }

        private void SetupButtonHoverEffects()
        {
            // Hover effect cho btnTimBGK (Primary Blue)
            btnTimBGK.MouseEnter += (s, e) => btnTimBGK.BackColor = UIColorPalette.ButtonPrimary.Hover;
            btnTimBGK.MouseLeave += (s, e) => btnTimBGK.BackColor = UIColorPalette.ButtonPrimary.Base;

            // Hover effect cho btnRefresh (Success Green)
            btnRefresh.MouseEnter += (s, e) => btnRefresh.BackColor = UIColorPalette.ButtonSuccess.Hover;
            btnRefresh.MouseLeave += (s, e) => btnRefresh.BackColor = UIColorPalette.ButtonSuccess.Base;

            // Hover effect cho btnXacNhan (Warning Orange)
            btnXacNhan.MouseEnter += (s, e) => btnXacNhan.BackColor = UIColorPalette.ButtonWarning.Hover;
            btnXacNhan.MouseLeave += (s, e) => btnXacNhan.BackColor = UIColorPalette.ButtonWarning.Base;
        }

        private void CheckERPConnection()
        {
            try
            {
                bool isConnected = _erpBgkBLL.TestERPConnection();

                if (isConnected)
                {
                    lblConnectionStatus.Text = "✅ Kết nối ERP thành công";
                    lblConnectionStatus.ForeColor = UIColorPalette.StatusSuccess;
                }
                else
                {
                    lblConnectionStatus.Text = "❌ Không thể kết nối ERP";
                    lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
                    txtSoBGK.Enabled = false;
                    btnTimBGK.Enabled = false;
                }
            }
            catch (Exception)
            {
                lblConnectionStatus.Text = "❌ Lỗi kết nối ERP";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
                txtSoBGK.Enabled = false;
                btnTimBGK.Enabled = false;
            }
        }

        private void SetupDataGridView()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvChiTiet);
            dgvChiTiet.AutoGenerateColumns = false;
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AllowUserToDeleteRows = false;
            dgvChiTiet.ReadOnly = false; // Cho phép chỉnh sửa
            dgvChiTiet.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvChiTiet.MultiSelect = false;
            
            // Thiết lập font truyền thống cho dữ liệu trong grid
            dgvChiTiet.DefaultCellStyle.Font = UIFonts.GridData;
            dgvChiTiet.RowTemplate.DefaultCellStyle.Font = UIFonts.GridData;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Font = UIFonts.GridHeader;
            
            // Căn giữa header cho tất cả các cột
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
                Name = "VatTuHangHoa",
                HeaderText = "Mã VT",
                DataPropertyName = "VatTuHangHoa",
                Width = 120,
                FillWeight = 15,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                Width = 400,
                FillWeight = 45,
                ReadOnly = true
            });

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
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

        private void DgvChiTiet_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || currentVatTuList == null || e.RowIndex >= currentVatTuList.Count) return;

            try
            {
                // Lấy data từ data source thay vì cell value để đảm bảo đúng với giá trị đã sửa
                var vatTu = currentVatTuList[e.RowIndex];
                decimal soLuongHoanUng = vatTu.SoLuongHoanUngThucTe ?? vatTu.SoLuongHoanUng;
                decimal tonKho = vatTu.TonKho;

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
                // Bỏ qua lỗi formatting
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
                        var result = MessageBox.Show(
                            $"⚠️ Số lượng hoàn ứng ({value:N2}) vượt quá tồn kho ({tonKho:N2})!\n\n" +
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
                lblStatus.ForeColor = UIColorPalette.StatusProcessing;

                // Tìm BGK theo số nghiệm thu và năm
                var bgkList = _erpBgkBLL.GetNghiemThuGiaoKhoanData(int.Parse(txtSoBGK.Text), nam);

                if (bgkList != null && bgkList.Count > 0)
                {
                    currentBGK = bgkList.First();
                    
                    // Kiểm tra BGK đã hoàn ứng trong database chưa
                    if (currentBGK.GiaoKhoanNghiemThuVatTuID.HasValue)
                    {
                        bool daHoanUng = _hoanUngBLL.CheckBGKDaHoanUng(currentBGK.GiaoKhoanNghiemThuVatTuID.Value);
                        if (daHoanUng)
                        {
                            MessageBox.Show($"⚠️ Bản nghiệm thu này đã hoàn ứng!\n\n" +
                                $"Số BGK: {currentBGK.SoBGK}\n" +
                                $"Số nghiệm thu: {currentBGK.SoNghiemThu}-{nam}\n\n" +
                                $"Không thể hoàn ứng lại BGK này.",
                                "Đã hoàn ứng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            
                            currentBGK.DaHoanUng = true;
                            lblStatus.Text = $"⚠️ BGK {currentBGK.SoBGK} đã được hoàn ứng";
                            lblStatus.ForeColor = UIColorPalette.WarningOrange;
                            
                            // Không load dữ liệu nếu đã hoàn ứng
                            ResetBGKDisplay();
                            return;
                        }
                        else
                        {
                            lblStatus.Text = $"✅ Đã tải BGK {currentBGK.SoBGK}";
                            lblStatus.ForeColor = UIColorPalette.StatusSuccess;
                        }
                    }
                    
                    LoadBGKData();
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
                lblStatus.ForeColor = UIColorPalette.StatusError;
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
                
                // Lấy tồn kho cho từng vật tư và khởi tạo SoLuongHoanUngThucTe
                foreach (var vatTu in currentVatTuList)
                {
                    try
                    {
                        vatTu.TonKho = _hoanUngBLL.GetTonKhoByErpId(vatTu.MaVTErp, currentBGK.MaNhanVienXayLap);
                    }
                    catch
                    {
                        vatTu.TonKho = 0;
                    }
                    
                    // Khởi tạo SoLuongHoanUngThucTe bằng SoLuongHoanUng để có thể edit
                    if (!vatTu.SoLuongHoanUngThucTe.HasValue)
                    {
                        vatTu.SoLuongHoanUngThucTe = vatTu.SoLuongHoanUng;
                    }
                }
                
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
                lblTrangThai.ForeColor = UIColorPalette.StatusSuccess;
            }
            else
            {
                lblTrangThai.Text = "Chưa hoàn ứng";
                lblTrangThai.ForeColor = UIColorPalette.StatusError;
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
            lblTrangThai.ForeColor = UIColorPalette.TextBlack;

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

                if (currentVatTuList == null || currentVatTuList.Count == 0)
                {
                    MessageBox.Show("Không có vật tư nào để hoàn ứng!", "Thông báo",
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
                    $"Xác nhận hoàn ứng BGK {currentBGK.SoBGK}?\n\n" +
                    $"Số nghiệm thu: {currentBGK.SoNghiemThu}-{currentBGK.NamNghiemThu}\n" +
                    $"Nhân viên kỹ thuật: {currentBGK.NhanVienKyThuat}\n" +
                    $"Nhân viên xây lắp: {currentBGK.NhanVienXayLap}\n" +
                    $"Số vật tư: {currentVatTuList.Count}\n\n" +
                    $"Hệ thống sẽ:\n" +
                    $"1. Lưu thông tin BGK vào database\n" +
                    $"2. Lưu chi tiết vật tư hoàn ứng\n" +
                    $"3. Tạo giao dịch hoàn ứng\n" +
                    $"4. Cập nhật tồn kho",
                    "Xác nhận hoàn ứng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    lblStatus.Text = "⏳ Đang xử lý hoàn ứng...";
                    lblStatus.ForeColor = UIColorPalette.StatusProcessing;
                    btnXacNhan.Enabled = false;

                    try
                    {
                        // Gọi BLL để xử lý hoàn ứng với transaction
                        DateTime ngayHoanUng = DateTime.Now; // Sử dụng ngày hiện tại
                        bool result = _hoanUngBLL.BGK_XacNhanHoanUngDonLe(
                            currentBGK, 
                            currentVatTuList, 
                            ngayHoanUng
                        );

                        if (result)
                        {
                            MessageBox.Show($"✅ Hoàn ứng BGK thành công!\n\n" +
                                $"Số BGK: {currentBGK.SoBGK}\n" +
                                $"Số vật tư: {currentVatTuList.Count}\n" +
                                $"Ngày hoàn ứng: {ngayHoanUng:dd/MM/yyyy}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            lblStatus.Text = $"✅ Hoàn ứng thành công BGK {currentBGK.SoBGK}";
                            lblStatus.ForeColor = UIColorPalette.StatusSuccess;

                            // Clear form và grid sau khi hoàn ứng thành công
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("Hoàn ứng thất bại!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblStatus.Text = "❌ Hoàn ứng thất bại";
                            lblStatus.ForeColor = UIColorPalette.StatusError;
                            btnXacNhan.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xử lý hoàn ứng:\n\n{ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                        lblStatus.ForeColor = UIColorPalette.StatusError;
                        btnXacNhan.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi không mong đợi:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = UIColorPalette.StatusError;
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

        private void HoanUngBGKUserControl_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Clear form và grid về trạng thái ban đầu
        /// </summary>
        private void ClearForm()
        {
            // Clear text boxes
            txtSoBGK.Clear();
            txtNam.Clear();
            
            // Clear BGK info labels
            lblSoBGK.Text = "";
            lblTrangThai.Text = "";
            lblNhanVienKyThuat.Text = "";
            lblNhanVienXayLap.Text = "";
            lblNoiDung.Text = "";
            lblSoNghiemThu.Text = "";
            
            // Clear grid
            dgvChiTiet.DataSource = null;
            dgvChiTiet.Rows.Clear();
            
            // Clear data
            currentBGK = null;
            currentVatTuList = null;
            
            // Reset status
            lblStatus.Text = "Nhập số BGK và năm để tìm kiếm";
            lblStatus.ForeColor = UIColorPalette.TextBlack;
            
            // Reset buttons
            btnXacNhan.Enabled = false;
            
            // Focus vào ô nhập số BGK
            txtSoBGK.Focus();
        }
    }
}
