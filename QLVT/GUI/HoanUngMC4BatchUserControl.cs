using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class HoanUngMC4BatchUserControl : UserControl
    {
        private const double WaitingListWidthRatio = 0.50;
        private const int LayoutMargin = 20;
        private const int LayoutGap = 20;

        private readonly HoanUngBLL hoanUngBLL;
        private List<DonDangKyModel> danhSachDonChoHoanUng = new();
        private List<DonDangKyModel> danhSachDonDaChon = new();
        private BackgroundWorker? batchWorker;
        private bool isProcessing = false;

        public HoanUngMC4BatchUserControl()
        {
            InitializeComponent();
            hoanUngBLL = new HoanUngBLL();
            ApplyModernStyle();
            ArrangeResponsiveLayout();
            SetupDataGridViews();
            LoadDanhSachDonChoHoanUng();
            CheckERPConnection();
            InitializeBackgroundWorker();
            Resize += (_, _) => ArrangeResponsiveLayout();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "HOÀN ỨNG HÀNG LOẠT - MẠNG CẤP 4";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            UIStyleHelper.ApplyStatusLabelStyle(lblConnectionStatus, StatusType.Processing);
            lblConnectionStatus.Text = "Đang kiểm tra kết nối ERP...";
            lblConnectionStatus.Size = new Size(260, 22);

            lblHuongDan.Text = "Double-click để chọn hoặc bỏ chọn đơn, hoặc dùng nút chọn tất cả";
            lblHuongDan.Font = UIFonts.TextSmall;
            lblHuongDan.ForeColor = UIColorPalette.TextMuted;

            grpChoHoanUng.Text = "Danh sách đơn chờ hoàn ứng";
            grpDaChon.Text = "Danh sách đơn đã chọn";
            grpTienDo.Text = "Tiến độ xử lý và kết quả";
            UIStyleHelper.ApplyGroupBoxStyle(grpChoHoanUng);
            UIStyleHelper.ApplyGroupBoxStyle(grpDaChon);
            UIStyleHelper.ApplyGroupBoxStyle(grpTienDo);

            UIStyleHelper.ApplySecondaryButtonStyle(btnRefresh, new Size(92, 30));
            UIStyleHelper.ApplyPrimaryButtonStyle(btnChonTatCa, new Size(132, 32));
            UIStyleHelper.ApplySecondaryButtonStyle(btnBoChonTatCa, new Size(132, 32));
            UIStyleHelper.ApplyWarningButtonStyle(btnBatDauHoanUng, new Size(178, 38));
            btnRefresh.Text = "Làm mới";
            btnChonTatCa.Text = "Chọn tất cả";
            btnBoChonTatCa.Text = "Bỏ chọn tất cả";
            btnBatDauHoanUng.Text = "Bắt đầu hoàn ứng";

            lblTongSoChoHoanUng.Font = UIFonts.HeaderStandard;
            lblTongSoChoHoanUng.ForeColor = UIColorPalette.StatusProcessing;
            lblTongSoDaChon.Font = UIFonts.HeaderStandard;
            lblTongSoDaChon.ForeColor = UIColorPalette.SuccessGreen;
            lblTienDo.Font = UIFonts.HeaderStandard;
            lblTienDo.ForeColor = UIColorPalette.WarningOrange;

            txtKetQua.BackColor = UIColorPalette.BackgroundWhite;
            txtKetQua.ForeColor = UIColorPalette.TextDark;
            txtKetQua.Font = new Font("Consolas", 9F, FontStyle.Regular);
            txtKetQua.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ArrangeResponsiveLayout()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            int contentWidth = Math.Max(900, ClientSize.Width - (LayoutMargin * 2));
            int toolbarTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;
            int listTop = toolbarTop + 36;
            int listHeight = Math.Max(230, Math.Min(330, ClientSize.Height / 2 - 70));
            int availableListWidth = contentWidth - LayoutGap;
            int waitingWidth = (int)(availableListWidth * WaitingListWidthRatio);
            int selectedWidth = availableListWidth - waitingWidth;

            lblConnectionStatus.Location = new Point(LayoutMargin, toolbarTop + 4);
            btnRefresh.Location = new Point(385, toolbarTop);
            lblHuongDan.Location = new Point(585, toolbarTop + 5);

            grpChoHoanUng.Location = new Point(LayoutMargin, listTop);
            grpChoHoanUng.Size = new Size(waitingWidth, listHeight);

            grpDaChon.Location = new Point(LayoutMargin + waitingWidth + LayoutGap, listTop);
            grpDaChon.Size = new Size(selectedWidth, listHeight);

            ResizeGridInsideGroup(dgvChoHoanUng, grpChoHoanUng);
            ResizeGridInsideGroup(dgvDaChon, grpDaChon);

            int actionTop = grpChoHoanUng.Bottom + 14;
            btnChonTatCa.Location = new Point(LayoutMargin + waitingWidth - btnChonTatCa.Width - btnBoChonTatCa.Width - 12, actionTop);
            btnBoChonTatCa.Location = new Point(btnChonTatCa.Right + 12, actionTop);
            btnBatDauHoanUng.Location = new Point(grpDaChon.Left + selectedWidth - btnBatDauHoanUng.Width, actionTop - 3);

            int progressTop = actionTop + 52;
            int progressHeight = Math.Max(170, ClientSize.Height - progressTop - LayoutMargin);
            grpTienDo.Location = new Point(LayoutMargin, progressTop);
            grpTienDo.Size = new Size(contentWidth, progressHeight);

            lblTienDo.Location = new Point(15, 25);
            progressBar.Location = new Point(15, 50);
            progressBar.Size = new Size(grpTienDo.ClientSize.Width - 30, 24);
            txtKetQua.Location = new Point(15, 84);
            txtKetQua.Size = new Size(grpTienDo.ClientSize.Width - 30, grpTienDo.ClientSize.Height - 100);
        }

        private static void ResizeGridInsideGroup(DataGridView grid, GroupBox groupBox)
        {
            grid.Location = new Point(15, 45);
            grid.Size = new Size(groupBox.ClientSize.Width - 30, groupBox.ClientSize.Height - 60);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void CheckERPConnection()
        {
            try
            {
                bool connected = hoanUngBLL.TestERPConnection();
                lblConnectionStatus.Text = connected ? "Kết nối ERP thành công" : "Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = connected ? UIColorPalette.StatusSuccess : UIColorPalette.StatusError;
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "Lỗi kết nối ERP";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
                MessageBox.Show($"Lỗi kiểm tra kết nối ERP: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeBackgroundWorker()
        {
            batchWorker = new BackgroundWorker();
            batchWorker.WorkerReportsProgress = true;
            batchWorker.WorkerSupportsCancellation = true;
            batchWorker.DoWork += BatchWorker_DoWork;
            batchWorker.ProgressChanged += BatchWorker_ProgressChanged;
            batchWorker.RunWorkerCompleted += BatchWorker_RunWorkerCompleted;
        }

        private void SetupDataGridViews()
        {
            // Setup DataGridView cho danh sách chờ hoàn ứng
            SetupDgvChoHoanUng();
            
            // Setup DataGridView cho danh sách đã chọn
            SetupDgvDaChon();
        }

        private void SetupDgvChoHoanUng()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvChoHoanUng);
            dgvChoHoanUng.ReadOnly = true;
            dgvChoHoanUng.MultiSelect = true;
            dgvChoHoanUng.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvChoHoanUng.Columns.Clear();

            dgvChoHoanUng.Columns.Add("MADDK", "Mã DDK");
            dgvChoHoanUng.Columns.Add("TENKH", "Tên khách hàng");
            dgvChoHoanUng.Columns.Add("DiaChi", "Địa chỉ");
            dgvChoHoanUng.Columns.Add("NhanVienXayLap", "NV thi công");
            dgvChoHoanUng.Columns.Add("NgayHoanUng", "Ngày hoàn ứng");

            ApplyMC4ColumnRatio(dgvChoHoanUng);
            dgvChoHoanUng.Columns["DiaChi"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvChoHoanUng.Columns["TENKH"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvChoHoanUng.Columns["MADDK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvChoHoanUng.Columns["NgayHoanUng"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvChoHoanUng.DoubleClick += DgvChoHoanUng_DoubleClick;
        }

        private void SetupDgvDaChon()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvDaChon);
            dgvDaChon.ReadOnly = true;
            dgvDaChon.MultiSelect = true;
            dgvDaChon.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvDaChon.Columns.Clear();

            dgvDaChon.Columns.Add("MADDK", "Mã DDK");
            dgvDaChon.Columns.Add("TENKH", "Tên khách hàng");
            dgvDaChon.Columns.Add("DiaChi", "Địa chỉ");
            dgvDaChon.Columns.Add("NhanVienXayLap", "NV thi công");
            dgvDaChon.Columns.Add("NgayHoanUng", "Ngày hoàn ứng");

            ApplyMC4ColumnRatio(dgvDaChon);
            dgvDaChon.Columns["DiaChi"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvDaChon.Columns["TENKH"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvDaChon.Columns["MADDK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDaChon.Columns["NgayHoanUng"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvDaChon.DoubleClick += DgvDaChon_DoubleClick;
        }

        private static void ApplyMC4ColumnRatio(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Columns["MADDK"].FillWeight = 13;
            dgv.Columns["TENKH"].FillWeight = 22;
            dgv.Columns["DiaChi"].FillWeight = 37;
            dgv.Columns["NhanVienXayLap"].FillWeight = 17;
            dgv.Columns["NgayHoanUng"].FillWeight = 11;
        }

        private void LoadDanhSachDonChoHoanUng()
        {
            try
            {
                danhSachDonChoHoanUng = hoanUngBLL.MC4_GetDanhSachChoHoanUng();
                DisplayDanhSachChoHoanUng();
                UpdateStatusLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách đơn chờ hoàn ứng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayDanhSachChoHoanUng()
        {
            dgvChoHoanUng.Rows.Clear();
            
            foreach (var don in danhSachDonChoHoanUng)
            {
                dgvChoHoanUng.Rows.Add(
                    don.MADDK,
                    don.TENKH,
                    don.DiaChi,
                    don.NhanVienXayLap,
                    don.NgayHoanUng?.ToString("dd/MM/yyyy") ?? ""
                );

                var row = dgvChoHoanUng.Rows[dgvChoHoanUng.Rows.Count - 1];
                row.DefaultCellStyle.BackColor = UIColorPalette.BackgroundWhite;
                row.DefaultCellStyle.ForeColor = UIColorPalette.TextDark;
            }
        }

        private void DisplayDanhSachDaChon()
        {
            dgvDaChon.Rows.Clear();
            
            foreach (var don in danhSachDonDaChon)
            {
                dgvDaChon.Rows.Add(
                    don.MADDK,
                    don.TENKH,
                    don.DiaChi,
                    don.NhanVienXayLap,
                    don.NgayHoanUng?.ToString("dd/MM/yyyy") ?? ""
                );

                var row = dgvDaChon.Rows[dgvDaChon.Rows.Count - 1];
                row.DefaultCellStyle.BackColor = Color.FromArgb(232, 246, 239);
                row.DefaultCellStyle.ForeColor = UIColorPalette.TextDark;
            }
        }

        private void UpdateStatusLabels()
        {
            lblTongSoChoHoanUng.Text = $"Tổng số đơn chờ hoàn ứng: {danhSachDonChoHoanUng.Count}";
            lblTongSoDaChon.Text = $"Đã chọn: {danhSachDonDaChon.Count} đơn";
            btnBatDauHoanUng.Enabled = danhSachDonDaChon.Count > 0 && !isProcessing;
        }

        private void DgvChoHoanUng_DoubleClick(object sender, EventArgs e)
        {
            if (dgvChoHoanUng.SelectedRows.Count > 0)
            {
                var selectedRows = dgvChoHoanUng.SelectedRows.Cast<DataGridViewRow>().ToList();
                
                foreach (var row in selectedRows)
                {
                    int index = row.Index;
                    if (index >= 0 && index < danhSachDonChoHoanUng.Count)
                    {
                        var don = danhSachDonChoHoanUng[index];
                        
                        // Kiểm tra xem đã có trong danh sách chọn chưa
                        if (!danhSachDonDaChon.Any(x => x.MADDK == don.MADDK))
                        {
                            danhSachDonDaChon.Add(don);
                        }
                    }
                }

                DisplayDanhSachDaChon();
                UpdateStatusLabels();
            }
        }

        private void DgvDaChon_DoubleClick(object sender, EventArgs e)
        {
            if (dgvDaChon.SelectedRows.Count > 0)
            {
                var selectedRows = dgvDaChon.SelectedRows.Cast<DataGridViewRow>().ToList();
                
                foreach (var row in selectedRows)
                {
                    int index = row.Index;
                    if (index >= 0 && index < danhSachDonDaChon.Count)
                    {
                        danhSachDonDaChon.RemoveAt(index);
                        break; // Chỉ xóa một item để tránh index lỗi
                    }
                }

                DisplayDanhSachDaChon();
                UpdateStatusLabels();
            }
        }

        private void BtnChonTatCa_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            danhSachDonDaChon.Clear();
            danhSachDonDaChon.AddRange(danhSachDonChoHoanUng);
            
            DisplayDanhSachDaChon();
            UpdateStatusLabels();
        }

        private void BtnBoChonTatCa_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            danhSachDonDaChon.Clear();
            DisplayDanhSachDaChon();
            UpdateStatusLabels();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            LoadDanhSachDonChoHoanUng();
            danhSachDonDaChon.Clear();
            DisplayDanhSachDaChon();
        }

        private void BtnBatDauHoanUng_Click(object sender, EventArgs e)
        {
            if (isProcessing || danhSachDonDaChon.Count == 0) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn hoàn ứng {danhSachDonDaChon.Count} đơn đã chọn?\n\n" +
                "Thao tác này không thể hoàn tác!",
                "Xác nhận hoàn ứng hàng loạt",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                StartBatchProcessing();
            }
        }

        private void StartBatchProcessing()
        {
            isProcessing = true;
            
            // Disable controls
            btnBatDauHoanUng.Enabled = false;
            btnChonTatCa.Enabled = false;
            btnBoChonTatCa.Enabled = false;
            btnRefresh.Enabled = false;

            // Setup progress
            progressBar.Value = 0;
            progressBar.Maximum = danhSachDonDaChon.Count;
            progressBar.Visible = true;
            lblTienDo.Visible = true;
            lblTienDo.Text = "Đang chuẩn bị...";

            // Clear results
            txtKetQua.Clear();
            txtKetQua.Visible = true;

            // Start background worker
            var danhSachMaDDK = danhSachDonDaChon.Select(x => x.MADDK).ToList();
            batchWorker?.RunWorkerAsync(danhSachMaDDK);
        }

        private void BatchWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is not List<string> danhSachMaDDK) return;

            var worker = sender as BackgroundWorker;
            var progress = new ImmediateProgress<(int current, int total, string maddk)>(update =>
            {
                worker?.ReportProgress(update.current, update);
            });

            var ketQua = hoanUngBLL.MC4_HoanUngHangLoat(danhSachMaDDK, progress).GetAwaiter().GetResult();
            e.Result = ketQua;
        }

        private sealed class ImmediateProgress<T> : IProgress<T>
        {
            private readonly Action<T> handler;

            public ImmediateProgress(Action<T> handler)
            {
                this.handler = handler;
            }

            public void Report(T value)
            {
                handler(value);
            }
        }

        private void BatchWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is (int current, int total, string maddk))
            {
                progressBar.Value = current;
                lblTienDo.Text = $"Đang xử lý {current}/{total}: {maddk}";
                
                // Thêm log vào text box
                txtKetQua.AppendText($"[{DateTime.Now:HH:mm:ss}] Đang xử lý đơn: {maddk}\r\n");
                txtKetQua.SelectionStart = txtKetQua.Text.Length;
                txtKetQua.ScrollToCaret();
            }
        }

        private void BatchWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            isProcessing = false;

            // Enable controls
            btnChonTatCa.Enabled = true;
            btnBoChonTatCa.Enabled = true;
            btnRefresh.Enabled = true;

            if (e.Error != null)
            {
                lblTienDo.Text = "Có lỗi xảy ra";
                txtKetQua.AppendText($"\r\nLỖI: {e.Error.Message}\r\n");
                MessageBox.Show($"Lỗi trong quá trình hoàn ứng hàng loạt:\n{e.Error.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Result is (int thanhCong, int thatBai, List<string> loi))
            {
                // Hiển thị kết quả
                txtKetQua.AppendText($"\r\n{new string('=', 50)}\r\n");
                txtKetQua.AppendText($"KẾT QUẢ HOÀN ỨNG HÀNG LOẠT:\r\n");
                txtKetQua.AppendText($"Thành công: {thanhCong} đơn\r\n");
                txtKetQua.AppendText($"Thất bại: {thatBai} đơn\r\n");

                if (loi.Count > 0)
                {
                    txtKetQua.AppendText($"\r\nCHI TIẾT LỖI:\r\n");
                    foreach (var loiItem in loi)
                    {
                        txtKetQua.AppendText($"• {loiItem}\r\n");
                    }
                }

                lblTienDo.Text = $"Hoàn thành: {thanhCong} thành công, {thatBai} thất bại";

                // Show summary message
                string thongBao = $"Hoàn ứng hàng loạt hoàn thành!\n\n" +
                                 $"Thành công: {thanhCong} đơn\n" +
                                 $"Thất bại: {thatBai} đơn";

                if (thatBai > 0)
                {
                    thongBao += "\n\nVui lòng xem chi tiết lỗi bên dưới.";
                }

                MessageBox.Show(thongBao, "Kết quả hoàn ứng hàng loạt", 
                    MessageBoxButtons.OK, 
                    thatBai > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                // Reload data if there were successful completions
                if (thanhCong > 0)
                {
                    LoadDanhSachDonChoHoanUng();
                    danhSachDonDaChon.Clear();
                    DisplayDanhSachDaChon();
                }
            }

            UpdateStatusLabels();
            progressBar.Value = progressBar.Maximum;
            txtKetQua.SelectionStart = txtKetQua.Text.Length;
            txtKetQua.ScrollToCaret();
        }
    }
}
