using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.ERP.Models;

namespace QLVT.GUI
{
    public partial class HoanUngDCBatchUserControl : UserControl
    {
        private readonly HoanUngBLL hoanUngBLL;
        private List<SuaChuaModel> danhSachDonChoHoanUng = new();
        private List<SuaChuaModel> danhSachDonDaChon = new();
        private BackgroundWorker? batchWorker;
        private bool isProcessing = false;

        public HoanUngDCBatchUserControl()
        {
            InitializeComponent();
            hoanUngBLL = new HoanUngBLL();
            SetupDataGridViews();
            CheckERPConnection();
            InitializeBackgroundWorker();
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
            dgvChoHoanUng.AllowUserToAddRows = false;
            dgvChoHoanUng.AllowUserToDeleteRows = false;
            dgvChoHoanUng.ReadOnly = true;
            dgvChoHoanUng.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvChoHoanUng.MultiSelect = true;
            dgvChoHoanUng.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvChoHoanUng.Columns.Clear();
            dgvChoHoanUng.Columns.Add("MADON", "Mã đơn");
            dgvChoHoanUng.Columns.Add("ViTriDiemChay", "Vị trí");
            dgvChoHoanUng.Columns.Add("NgayHoanThanh", "Thời gian hoàn thành");
            dgvChoHoanUng.Columns.Add("NhanVienXayLap", "Người hoàn ứng");

            dgvChoHoanUng.Columns["MADON"].Width = 120;
            dgvChoHoanUng.Columns["ViTriDiemChay"].Width = 200;
            dgvChoHoanUng.Columns["NgayHoanThanh"].Width = 150;
            dgvChoHoanUng.Columns["NhanVienXayLap"].Width = 150;

            dgvChoHoanUng.DoubleClick += DgvChoHoanUng_DoubleClick;
        }

        private void SetupDgvDaChon()
        {
            dgvDaChon.AllowUserToAddRows = false;
            dgvDaChon.AllowUserToDeleteRows = false;
            dgvDaChon.ReadOnly = true;
            dgvDaChon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDaChon.MultiSelect = true;
            dgvDaChon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvDaChon.Columns.Clear();
            dgvDaChon.Columns.Add("MADON", "Mã đơn");
            dgvDaChon.Columns.Add("ViTriDiemChay", "Vị trí");
            dgvDaChon.Columns.Add("NgayHoanThanh", "Thời gian hoàn thành");
            dgvDaChon.Columns.Add("NhanVienXayLap", "Người hoàn ứng");

            dgvDaChon.Columns["MADON"].Width = 120;
            dgvDaChon.Columns["ViTriDiemChay"].Width = 200;
            dgvDaChon.Columns["NgayHoanThanh"].Width = 150;
            dgvDaChon.Columns["NhanVienXayLap"].Width = 150;

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
                row.DefaultCellStyle.BackColor = Color.LightYellow;
                row.DefaultCellStyle.ForeColor = Color.DarkBlue;
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
                row.DefaultCellStyle.BackColor = Color.LightGreen;
                row.DefaultCellStyle.ForeColor = Color.DarkGreen;
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

            var progress = new Progress<(int current, int total, string maDon)>(update =>
            {
                batchWorker?.ReportProgress(update.current, update);
            });

            var ketQua = hoanUngBLL.DC_HoanUngHangLoat(danhSachMaDon, progress);
            e.Result = ketQua;
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
