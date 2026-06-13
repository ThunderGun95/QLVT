using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class HoanUngDCBatchUserControl : UserControl
    {
        private readonly Label lblPageTitle = new();
        private const int LayoutMargin = 20;

        private readonly HoanUngBLL hoanUngBLL;
        private List<SuaChuaModel> danhSachDonChoHoanUng = new();
        private List<SuaChuaModel> danhSachDonDaChon = new();
        private BackgroundWorker? batchWorker;
        private bool isProcessing = false;

        public HoanUngDCBatchUserControl()
        {
            InitializeComponent();
            hoanUngBLL = new HoanUngBLL();
            ApplyModernStyle();
            ArrangeModernLayout();
            SetupDataGridViews();
            CheckERPConnection();
            InitializeBackgroundWorker();
            Resize += (_, _) => ArrangeModernLayout();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblPageTitle.Text = "HOÀN ỨNG HÀNG LOẠT - SỬA CHỮA SỰ CỐ";
            UIStyleHelper.ApplyTitleBarStyle(lblPageTitle);
            Controls.Add(lblPageTitle);
            lblPageTitle.BringToFront();

            tableLayoutPanel1.Dock = DockStyle.None;
            tableLayoutPanel1.BackColor = UIColorPalette.BackgroundLight;
            tableLayoutPanel1.ColumnStyles[0].Width = 50F;
            tableLayoutPanel1.ColumnStyles[1].Width = 50F;
            tableLayoutPanel1.RowStyles[0].Height = 58F;

            UIStyleHelper.ApplySecondaryButtonStyle(btnRefresh, new Size(92, 30));
            UIStyleHelper.ApplyPrimaryButtonStyle(btnChonTatCa, new Size(132, 32));
            UIStyleHelper.ApplySecondaryButtonStyle(btnBoChonTatCa, new Size(132, 32));
            UIStyleHelper.ApplyWarningButtonStyle(btnBatDauHoanUng, new Size(178, 36));
            btnRefresh.Text = "Làm mới";
            btnChonTatCa.Text = "Chọn tất cả";
            btnBoChonTatCa.Text = "Bỏ chọn tất cả";
            btnBatDauHoanUng.Text = "Bắt đầu hoàn ứng";

            lblTongSoChoHoanUng.Font = UIFonts.HeaderStandard;
            lblTongSoChoHoanUng.ForeColor = UIColorPalette.StatusProcessing;
            lblTongSoDaChon.Font = UIFonts.HeaderStandard;
            lblTongSoDaChon.ForeColor = UIColorPalette.SuccessGreen;
            UIStyleHelper.ApplyStatusLabelStyle(lblConnectionStatus, StatusType.Processing);
            lblConnectionStatus.TextAlign = ContentAlignment.MiddleRight;

            txtKetQua.BackColor = UIColorPalette.BackgroundWhite;
            txtKetQua.ForeColor = UIColorPalette.TextDark;
            txtKetQua.Font = new Font("Consolas", 9F);
            txtKetQua.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ArrangeModernLayout()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            int top = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;
            tableLayoutPanel1.Location = new Point(LayoutMargin, top);
            tableLayoutPanel1.Size = new Size(
                Math.Max(900, ClientSize.Width - LayoutMargin * 2),
                Math.Max(500, ClientSize.Height - top - LayoutMargin));
        }

        private void CheckERPConnection()
        {
            try
            {
                bool connected = hoanUngBLL.TestERPConnection();
                lblConnectionStatus.Text = connected ? "✅ Kết nối ERP thành công" : "❌ Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = connected ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "❌ Lỗi kết nối ERP";
                lblConnectionStatus.ForeColor = Color.Red;
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
            SetupDgvChoHoanUng();
            SetupDgvDaChon();
            LoadDanhSachDonChoHoanUng();
        }

        private void SetupDgvChoHoanUng()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvChoHoanUng);
            dgvChoHoanUng.ReadOnly = true;
            dgvChoHoanUng.MultiSelect = true;

            dgvChoHoanUng.Columns.Clear();
            dgvChoHoanUng.Columns.Add("MADON", "Mã đơn");
            dgvChoHoanUng.Columns.Add("ViTriDiemChay", "Vị trí");
            dgvChoHoanUng.Columns.Add("NgayHoanThanh", "Thời gian hoàn thành");
            dgvChoHoanUng.Columns.Add("NhanVienXayLap", "Người hoàn ứng");

            dgvChoHoanUng.Columns["MADON"].Width = 120;
            dgvChoHoanUng.Columns["ViTriDiemChay"].Width = 200;
            dgvChoHoanUng.Columns["NgayHoanThanh"].Width = 150;
            dgvChoHoanUng.Columns["NhanVienXayLap"].Width = 150;
            dgvChoHoanUng.Columns["MADON"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvChoHoanUng.Columns["NgayHoanThanh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvChoHoanUng.DoubleClick += DgvChoHoanUng_DoubleClick;
        }

        private void SetupDgvDaChon()
        {
            UIStyleHelper.ApplyDataGridViewStyle(dgvDaChon);
            dgvDaChon.ReadOnly = true;
            dgvDaChon.MultiSelect = true;

            dgvDaChon.Columns.Clear();
            dgvDaChon.Columns.Add("MADON", "Mã đơn");
            dgvDaChon.Columns.Add("ViTriDiemChay", "Vị trí");
            dgvDaChon.Columns.Add("NgayHoanThanh", "Thời gian hoàn thành");
            dgvDaChon.Columns.Add("NhanVienXayLap", "Người hoàn ứng");

            dgvDaChon.Columns["MADON"].Width = 120;
            dgvDaChon.Columns["ViTriDiemChay"].Width = 200;
            dgvDaChon.Columns["NgayHoanThanh"].Width = 150;
            dgvDaChon.Columns["NhanVienXayLap"].Width = 150;
            dgvDaChon.Columns["MADON"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDaChon.Columns["NgayHoanThanh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvDaChon.DoubleClick += DgvDaChon_DoubleClick;
        }

        private void LoadDanhSachDonChoHoanUng()
        {
            try
            {
                danhSachDonChoHoanUng = hoanUngBLL.DC_GetDanhSachChoHoanUng();
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
                    don.MADON,
                    don.ViTriDiemChay,
                    don.NgayHoanThanh?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    don.NhanVienXayLap
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
                    don.MADON,
                    don.ViTriDiemChay,
                    don.NgayHoanThanh?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    don.NhanVienXayLap
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

        private void DgvChoHoanUng_DoubleClick(object? sender, EventArgs e)
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
                        
                        if (!danhSachDonDaChon.Any(x => x.MADON == don.MADON))
                        {
                            danhSachDonDaChon.Add(don);
                        }
                    }
                }

                DisplayDanhSachDaChon();
                UpdateStatusLabels();
            }
        }

        private void DgvDaChon_DoubleClick(object? sender, EventArgs e)
        {
            if (dgvDaChon.SelectedRows.Count > 0)
            {
                var selectedRows = dgvDaChon.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(x => x.Index).ToList();
                
                foreach (var row in selectedRows)
                {
                    int index = row.Index;
                    if (index >= 0 && index < danhSachDonDaChon.Count)
                    {
                        danhSachDonDaChon.RemoveAt(index);
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
            CheckERPConnection();
        }

        private void BtnBatDauHoanUng_Click(object sender, EventArgs e)
        {
            if (isProcessing || danhSachDonDaChon.Count == 0) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn hoàn ứng {danhSachDonDaChon.Count} đơn điểm chảy đã chọn?\n\n" +
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
            
            btnBatDauHoanUng.Enabled = false;
            btnChonTatCa.Enabled = false;
            btnBoChonTatCa.Enabled = false;
            btnRefresh.Enabled = false;

            progressBar.Value = 0;
            progressBar.Maximum = danhSachDonDaChon.Count;
            progressBar.Visible = true;
            lblTienDo.Visible = true;
            lblTienDo.Text = "Đang chuẩn bị...";

            txtKetQua.Clear();
            txtKetQua.Visible = true;

            var danhSachMaDon = danhSachDonDaChon.Select(x => x.MADON).ToList();
            batchWorker?.RunWorkerAsync(danhSachMaDon);
        }

        private void BatchWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is not List<string> danhSachMaDon) return;

            var worker = sender as BackgroundWorker;
            var progress = new ImmediateProgress<(int current, int total, string maDon)>(update =>
            {
                worker?.ReportProgress(update.current, update);
            });

            var ketQua = hoanUngBLL.DC_HoanUngHangLoat(danhSachMaDon, progress);
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
            if (e.UserState is (int current, int total, string maDon))
            {
                progressBar.Value = current;
                lblTienDo.Text = $"Đang xử lý {current}/{total}: {maDon}";
                
                txtKetQua.AppendText($"[{DateTime.Now:HH:mm:ss}] Đang xử lý đơn: {maDon}\r\n");
                txtKetQua.SelectionStart = txtKetQua.Text.Length;
                txtKetQua.ScrollToCaret();
            }
        }

        private void BatchWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            isProcessing = false;

            btnChonTatCa.Enabled = true;
            btnBoChonTatCa.Enabled = true;
            btnRefresh.Enabled = true;

            if (e.Error != null)
            {
                lblTienDo.Text = "❌ Có lỗi xảy ra!";
                txtKetQua.AppendText($"\r\n❌ LỖI: {e.Error.Message}\r\n");
                MessageBox.Show($"Lỗi trong quá trình hoàn ứng hàng loạt:\n{e.Error.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Result is (int thanhCong, int thatBai, List<string> loi))
            {
                txtKetQua.AppendText($"\r\n{"=".PadRight(50, '=')}\r\n");
                txtKetQua.AppendText($"📊 KẾT QUẢ HOÀN ỨNG HÀNG LOẠT:\r\n");
                txtKetQua.AppendText($"✅ Thành công: {thanhCong} đơn\r\n");
                txtKetQua.AppendText($"❌ Thất bại: {thatBai} đơn\r\n");

                if (loi.Count > 0)
                {
                    txtKetQua.AppendText($"\r\n📋 CHI TIẾT LỖI:\r\n");
                    foreach (var loiItem in loi)
                    {
                        txtKetQua.AppendText($"• {loiItem}\r\n");
                    }
                }

                lblTienDo.Text = $"✅ Hoàn thành: {thanhCong} thành công, {thatBai} thất bại";

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
