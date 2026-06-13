using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.ERP.BLL;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class HoanUngBGKBatchUserControl : UserControl
    {
        private readonly Label lblPageTitle = new();
        private const int LayoutMargin = 20;

        private readonly ERPBgkBLL _erpBgkBLL;
        private readonly HoanUngBLL _hoanUngBLL;
        private List<BGKBatchItem> _danhSachBGK;
        private List<BGKBatchItem> _danhSachChon;
        private CancellationTokenSource? _cancellationTokenSource;

        public HoanUngBGKBatchUserControl()
        {
            InitializeComponent();
            _erpBgkBLL = new ERPBgkBLL();
            _hoanUngBLL = new HoanUngBLL();
            _danhSachBGK = new List<BGKBatchItem>();
            _danhSachChon = new List<BGKBatchItem>();
            ApplyModernStyle();
            ArrangeModernLayout();
            ConfigureDataGridViews();
            InitializeControls();
            Resize += (_, _) => ArrangeModernLayout();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblPageTitle.Text = "HOÀN ỨNG HÀNG LOẠT - BGK";
            UIStyleHelper.ApplyTitleBarStyle(lblPageTitle);
            Controls.Add(lblPageTitle);
            lblPageTitle.BringToFront();

            grpInput.Text = "Thông tin nhập";
            grpProgress.Text = "Trạng thái";
            grpActions.Text = "Thao tác";
            grpAvailable.Text = "Danh sách BGK có thể chọn";
            grpSelected.Text = "Danh sách BGK đã chọn";

            foreach (var groupBox in new[] { grpInput, grpProgress, grpActions, grpAvailable, grpSelected })
            {
                UIStyleHelper.ApplyGroupBoxStyle(groupBox);
            }

            foreach (var label in new[] { lblNam, lblTuSo, lblDenSo, lblTongSo, lblDaChon, lblProgress, lblTienDo })
            {
                UIStyleHelper.ApplyStandardLabelStyle(label);
            }

            UIStyleHelper.ApplyStatusLabelStyle(lblTrangThai, StatusType.Ready);
            lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;

            foreach (var input in new[] { nudNam, nudTuSo, nudDenSo })
            {
                input.Font = UIFonts.TextStandard;
                input.BackColor = UIColorPalette.BackgroundWhite;
                input.ForeColor = UIColorPalette.TextDark;
                input.BorderStyle = BorderStyle.FixedSingle;
                input.Height = 23;
            }

            UIStyleHelper.ApplyPrimaryButtonStyle(btnGenerate, new Size(120, 30));
            UIStyleHelper.ApplyWarningButtonStyle(btnBatDauXuLy, new Size(150, 32));
            UIStyleHelper.ApplySecondaryButtonStyle(btnHuy, new Size(86, 32));
            UIStyleHelper.ApplyPrimaryButtonStyle(btnChonTatCa, new Size(118, 32));
            UIStyleHelper.ApplySecondaryButtonStyle(btnXoaTatCa, new Size(132, 32));

            btnGenerate.Text = "Tạo danh sách";
            btnBatDauXuLy.Text = "Bắt đầu hoàn ứng";
            btnHuy.Text = "Hủy";
            btnChonTatCa.Text = "Chọn tất cả";
            btnXoaTatCa.Text = "Bỏ chọn tất cả";
        }

        private void ArrangeModernLayout()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            int width = Math.Max(900, ClientSize.Width - LayoutMargin * 2);
            int top = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;

            grpInput.Location = new Point(LayoutMargin, top);
            grpInput.Size = new Size(width, 74);

            grpProgress.Location = new Point(LayoutMargin, grpInput.Bottom + 8);
            grpProgress.Size = new Size(width, 86);

            grpActions.Location = new Point(LayoutMargin, grpProgress.Bottom + 8);
            grpActions.Size = new Size(width, 60);

            int listTop = grpActions.Bottom + 10;
            int listHeight = Math.Max(240, ClientSize.Height - listTop - LayoutMargin);
            int gap = 18;
            int halfWidth = (width - gap) / 2;

            grpAvailable.Location = new Point(LayoutMargin, listTop);
            grpAvailable.Size = new Size(halfWidth, listHeight);
            grpSelected.Location = new Point(grpAvailable.Right + gap, listTop);
            grpSelected.Size = new Size(width - halfWidth - gap, listHeight);

            progressBar.Location = new Point(Math.Max(440, width - 430), 28);
            progressBar.Size = new Size(300, 20);
            lblTienDo.Location = new Point(progressBar.Right + 10, 28);
            lblTrangThai.Location = new Point(progressBar.Left, 56);
        }

        private void InitializeControls()
        {
            // Set năm mặc định là năm hiện tại
            nudNam.Value = DateTime.Now.Year;
            nudTuSo.Value = 1;
            nudDenSo.Value = 1;
        }

        private void ConfigureDataGridViews()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvDanhSachBGK);
            UIStyleHelper.ApplyDataGridViewStyle(dgvDanhSachChon);

            // Cấu hình DataGridView danh sách BGK
            dgvDanhSachBGK.AutoGenerateColumns = false;
            dgvDanhSachBGK.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSachBGK.MultiSelect = true;
            dgvDanhSachBGK.AllowUserToAddRows = false;
            dgvDanhSachBGK.AllowUserToDeleteRows = false;
            dgvDanhSachBGK.ReadOnly = true;
            dgvDanhSachBGK.BackgroundColor = Color.White;

            // Cấu hình DataGridView danh sách đã chọn
            dgvDanhSachChon.AutoGenerateColumns = false;
            dgvDanhSachChon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSachChon.MultiSelect = true;
            dgvDanhSachChon.AllowUserToAddRows = false;
            dgvDanhSachChon.AllowUserToDeleteRows = false;
            dgvDanhSachChon.ReadOnly = true;
            dgvDanhSachChon.BackgroundColor = Color.White;

            // Thêm cột cho dgvDanhSachBGK
            dgvDanhSachBGK.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoNghiemThu",
                HeaderText = "Số nghiệm thu",
                DataPropertyName = "SoNghiemThu",
                Width = 100
            });

            dgvDanhSachBGK.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nam",
                HeaderText = "Năm",
                DataPropertyName = "Nam",
                Width = 80
            });

            dgvDanhSachBGK.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                DataPropertyName = "TrangThai",
                Width = 100
            });

            dgvDanhSachBGK.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThongBao",
                HeaderText = "Thông báo",
                DataPropertyName = "ThongBao",
                Width = 250,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // Sao chép cấu hình cột cho dgvDanhSachChon
            foreach (DataGridViewColumn col in dgvDanhSachBGK.Columns)
            {
                dgvDanhSachChon.Columns.Add((DataGridViewColumn)col.Clone());
            }
            
            // Thêm event formatting để tô màu theo trạng thái
            dgvDanhSachBGK.CellFormatting += DgvBatchList_CellFormatting;
            dgvDanhSachChon.CellFormatting += DgvBatchList_CellFormatting;
        }

        private void DgvBatchList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null || e.RowIndex < 0) return;

            var item = dgv.Rows[e.RowIndex].DataBoundItem as BGKBatchItem;
            if (item == null) return;

            var row = dgv.Rows[e.RowIndex];
            switch (item.TrangThai)
            {
                case BGKBatchStatus.ChoXuLy:
                    row.DefaultCellStyle.BackColor = UIColorPalette.SurfaceMuted;
                    row.DefaultCellStyle.ForeColor = UIColorPalette.TextDark;
                    break;
                case BGKBatchStatus.DangXuLy:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(219, 234, 254);
                    row.DefaultCellStyle.ForeColor = UIColorPalette.StatusProcessing;
                    break;
                case BGKBatchStatus.ThanhCong:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(232, 246, 239);
                    row.DefaultCellStyle.ForeColor = UIColorPalette.StatusSuccess;
                    break;
                case BGKBatchStatus.ThatBai:
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    row.DefaultCellStyle.ForeColor = UIColorPalette.StatusError;
                    break;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                int nam = (int)nudNam.Value;
                int tuSo = (int)nudTuSo.Value;
                int denSo = (int)nudDenSo.Value;

                if (tuSo > denSo)
                {
                    MessageBox.Show("Số bắt đầu phải nhỏ hơn hoặc bằng số kết thúc!", 
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblTrangThai.Text = "Đang tìm kiếm BGK...";
                lblTrangThai.ForeColor = Color.Blue;

                // Tạo danh sách BGK cần xử lý
                _danhSachBGK.Clear();
                for (int soNghiemThu = tuSo; soNghiemThu <= denSo; soNghiemThu++)
                {
                    _danhSachBGK.Add(new BGKBatchItem
                    {
                        SoNghiemThu = soNghiemThu,
                        Nam = nam,
                        TrangThai = BGKBatchStatus.ChoXuLy,
                        ThongBao = "Chờ xử lý"
                    });
                }

                // Hiển thị danh sách
                dgvDanhSachBGK.DataSource = null;
                dgvDanhSachBGK.DataSource = _danhSachBGK;

                lblTongSo.Text = $"Tổng số: {_danhSachBGK.Count} BGK";
                lblTrangThai.Text = "Sẵn sàng";
                lblTrangThai.ForeColor = Color.Green;
                btnGenerate.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo danh sách: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblTrangThai.Text = "Lỗi tạo danh sách";
                lblTrangThai.ForeColor = Color.Red;
            }
        }

        private void btnChonTatCa_Click(object sender, EventArgs e)
        {
            foreach (var bgk in _danhSachBGK.Where(x => x.TrangThai == BGKBatchStatus.ChoXuLy))
            {
                if (!_danhSachChon.Any(x => x.SoNghiemThu == bgk.SoNghiemThu && x.Nam == bgk.Nam))
                {
                    _danhSachChon.Add(bgk);
                }
            }
            
            dgvDanhSachChon.DataSource = null;
            dgvDanhSachChon.DataSource = _danhSachChon;
            lblDaChon.Text = $"Đã chọn: {_danhSachChon.Count} BGK";
        }

        private void btnXoaTatCa_Click(object sender, EventArgs e)
        {
            _danhSachChon.Clear();
            dgvDanhSachChon.DataSource = null;
            dgvDanhSachChon.DataSource = _danhSachChon;
            lblDaChon.Text = $"Đã chọn: {_danhSachChon.Count} BGK";
        }

        private async void btnBatDauXuLy_Click(object sender, EventArgs e)
        {
            if (_danhSachChon.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một BGK để xử lý.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn hoàn ứng {_danhSachChon.Count} BGK đã chọn?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            await XuLyHoanUngBGK();
        }

        private async Task XuLyHoanUngBGK()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                // Disable các button
                SetControlsEnabled(false);
                
                // Reset progress
                progressBar.Value = 0;
                progressBar.Maximum = _danhSachChon.Count;
                lblTienDo.Text = $"0/{_danhSachChon.Count}";
                lblTrangThai.Text = "Đang xử lý...";
                lblTrangThai.ForeColor = Color.Blue;

                int thanhCong = 0;
                int thatBai = 0;
                List<string> danhSachLoi = new List<string>();

                for (int i = 0; i < _danhSachChon.Count; i++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    var item = _danhSachChon[i];
                    
                    try
                    {
                        // Cập nhật trạng thái đang xử lý
                        item.TrangThai = BGKBatchStatus.DangXuLy;
                        item.ThongBao = "Đang xử lý...";
                        dgvDanhSachChon.Refresh();

                        // Tìm BGK từ ERP
                        var bgkList = await Task.Run(() => 
                            _erpBgkBLL.GetNghiemThuGiaoKhoanData(item.SoNghiemThu, item.Nam));

                        if (bgkList == null || bgkList.Count == 0)
                        {
                            throw new Exception($"Không tìm thấy BGK số {item.SoNghiemThu}-{item.Nam}");
                        }

                        var bgk = bgkList.First();

                        // Kiểm tra BGK đã hoàn ứng trong database chưa
                        if (bgk.GiaoKhoanNghiemThuVatTuID.HasValue)
                        {
                            bool daHoanUng = _hoanUngBLL.CheckBGKDaHoanUng(bgk.GiaoKhoanNghiemThuVatTuID.Value);
                            if (daHoanUng)
                            {
                                throw new Exception("Bản nghiệm thu này đã hoàn ứng");
                            }
                        }

                        // Lấy chi tiết vật tư
                        var vatTuList = _erpBgkBLL.GetNghiemThuGiaoKhoanCTData(bgk.GiaoKhoanNghiemThuVatTuID!.Value);
                        if (vatTuList == null || vatTuList.Count == 0)
                        {
                            throw new Exception("Không có vật tư nào để hoàn ứng");
                        }

                        // Thực hiện hoàn ứng qua HoanUngBLL
                        bool result = await Task.Run(() => _hoanUngBLL.BGK_XacNhanHoanUngDonLe(
                            bgk,
                            vatTuList,
                            DateTime.Now
                        ));

                        if (!result)
                        {
                            throw new Exception("Hoàn ứng thất bại");
                        }

                        item.TrangThai = BGKBatchStatus.ThanhCong;
                        item.ThongBao = $"✅ Hoàn ứng thành công BGK {bgk.SoBGK}";
                        thanhCong++;
                    }
                    catch (Exception ex)
                    {
                        item.TrangThai = BGKBatchStatus.ThatBai;
                        item.ThongBao = $"❌ Lỗi: {ex.Message}";
                        thatBai++;
                        danhSachLoi.Add($"BGK {item.SoNghiemThu}-{item.Nam}: {ex.Message}");
                    }

                    // Cập nhật progress
                    progressBar.Value = i + 1;
                    lblTienDo.Text = $"{i + 1}/{_danhSachChon.Count}";
                    dgvDanhSachChon.Refresh();

                    // Delay nhỏ để tránh quá tải
                    await Task.Delay(100);
                }

                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Hiển thị kết quả
                    string thongBao = $"Hoàn ứng BGK hoàn tất!\n\n" +
                                     $"✅ Thành công: {thanhCong}\n" +
                                     $"❌ Thất bại: {thatBai}";

                    if (danhSachLoi.Count > 0)
                    {
                        thongBao += $"\n\n📋 Chi tiết lỗi:\n" + string.Join("\n", danhSachLoi.Take(10));
                        if (danhSachLoi.Count > 10)
                            thongBao += $"\n... và {danhSachLoi.Count - 10} lỗi khác";
                    }

                    MessageBox.Show(thongBao, "Kết quả", MessageBoxButtons.OK, 
                        thatBai == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                    lblTrangThai.Text = $"Hoàn tất: {thanhCong} thành công, {thatBai} thất bại";
                    lblTrangThai.ForeColor = thatBai == 0 ? Color.Green : Color.Orange;
                }
            }
            catch (OperationCanceledException)
            {
                lblTrangThai.Text = "Đã hủy";
                lblTrangThai.ForeColor = Color.Orange;
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi xử lý";
                lblTrangThai.ForeColor = Color.Red;
                MessageBox.Show($"Lỗi trong quá trình xử lý:\n{ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsEnabled(true);
                progressBar.Value = 0;
                lblTienDo.Text = "";
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private void SetControlsEnabled(bool enabled)
        {
            btnGenerate.Enabled = enabled;
            nudNam.Enabled = enabled;
            nudTuSo.Enabled = enabled;
            nudDenSo.Enabled = enabled;
            btnChonTatCa.Enabled = enabled;
            btnXoaTatCa.Enabled = enabled;
            btnBatDauXuLy.Enabled = enabled;
            btnHuy.Enabled = !enabled;
        }

        private void dgvDanhSachBGK_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var bgk = (BGKBatchItem)dgvDanhSachBGK.Rows[e.RowIndex].DataBoundItem;
                if (!_danhSachChon.Any(x => x.SoNghiemThu == bgk.SoNghiemThu && x.Nam == bgk.Nam))
                {
                    _danhSachChon.Add(bgk);
                    dgvDanhSachChon.DataSource = null;
                    dgvDanhSachChon.DataSource = _danhSachChon;
                    lblDaChon.Text = $"Đã chọn: {_danhSachChon.Count} BGK";
                }
            }
        }

        private void dgvDanhSachChon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var bgk = (BGKBatchItem)dgvDanhSachChon.Rows[e.RowIndex].DataBoundItem;
                _danhSachChon.RemoveAll(x => x.SoNghiemThu == bgk.SoNghiemThu && x.Nam == bgk.Nam);
                dgvDanhSachChon.DataSource = null;
                dgvDanhSachChon.DataSource = _danhSachChon;
                lblDaChon.Text = $"Đã chọn: {_danhSachChon.Count} BGK";
            }
        }

        private void UpdateProgress()
        {
            if (_danhSachBGK.Count == 0 && _danhSachChon.Count == 0)
            {
                lblProgress.Text = "";
                return;
            }

            int pending = _danhSachBGK.Count(x => x.TrangThai == BGKBatchStatus.ChoXuLy);
            int processing = _danhSachChon.Count(x => x.TrangThai == BGKBatchStatus.DangXuLy);
            int success = _danhSachChon.Count(x => x.TrangThai == BGKBatchStatus.ThanhCong);
            int failed = _danhSachChon.Count(x => x.TrangThai == BGKBatchStatus.ThatBai);

            lblProgress.Text = $"Chờ: {pending} | Đang xử lý: {processing} | Thành công: {success} | Lỗi: {failed}";
        }
    }

    /// <summary>
    /// Model cho item trong batch process BGK
    /// </summary>
    public class BGKBatchItem
    {
        public int SoNghiemThu { get; set; }
        public int Nam { get; set; }
        public BGKBatchStatus TrangThai { get; set; } = BGKBatchStatus.ChoXuLy;
        public string ThongBao { get; set; } = "";
    }

    /// <summary>
    /// Trạng thái xử lý batch BGK
    /// </summary>
    public enum BGKBatchStatus
    {
        ChoXuLy,
        DangXuLy,
        ThanhCong,
        ThatBai
    }
}
