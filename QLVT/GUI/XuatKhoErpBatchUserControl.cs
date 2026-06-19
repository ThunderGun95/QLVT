using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;
using QLVT.ERP.Models;
using QLVT.Utils;
using QLVT.DAL;

namespace QLVT.GUI
{
    /// <summary>
    /// UserControl để xuất hàng loạt phiếu xuất kho từ ERP
    /// </summary>
    public partial class XuatKhoErpBatchUserControl : UserControl
    {
        private const int PagePadding = 18;

        private readonly XuatKhoBLL xuatKhoBLL;
        private readonly WarehouseDAL warehouseDAL;
        private readonly Label lblTitle = new();
        private List<BatchExportItem> batchItems = new();
        private BackgroundWorker batchWorker;
        private bool isProcessing = false;

        public XuatKhoErpBatchUserControl()
        {
            InitializeComponent();
            ApplyModernStyle();
            SetupBatchGridColumns();
            xuatKhoBLL = new XuatKhoBLL();
            warehouseDAL = new WarehouseDAL();
            SetupBackgroundWorker();
            CheckERPConnection();
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "XUẤT KHO ERP HÀNG LOẠT";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);
            Controls.Add(lblTitle);
            lblTitle.BringToFront();

            grpInput.Text = "Thông tin phiếu";
            grpProgress.Text = "Trạng thái xử lý";
            grpActions.Text = "Thao tác";
            grpResults.Text = "Danh sách phiếu";
            foreach (var groupBox in new[] { grpInput, grpProgress, grpActions, grpResults })
            {
                UIStyleHelper.ApplyGroupBoxStyle(groupBox);
            }

            lblNam.Text = "Năm";
            lblTuSo.Text = "Từ số";
            lblDenSo.Text = "Đến số";
            lblNote.Text = "Hệ thống tự tìm kho nhân viên theo mã nhân viên trong phiếu, xử lý vật tư VP và vật tư thường theo quy tắc xuất kho ERP.";
            lblNote.ForeColor = UIColorPalette.TextMuted;
            foreach (var label in new[] { lblNam, lblTuSo, lblDenSo, lblConnectionStatus, lblSummary, lblProgress, lblNote })
            {
                UIStyleHelper.ApplyStandardLabelStyle(label);
            }
            lblNote.AutoSize = false;
            lblConnectionStatus.AutoSize = false;
            lblSummary.AutoSize = false;
            lblProgress.AutoSize = false;

            btnGenerate.Text = "Tạo danh sách";
            btnStart.Text = "Bắt đầu";
            btnStop.Text = "Dừng";
            btnReset.Text = "Làm mới";
            btnExportLog.Text = "Xuất log";
            UIStyleHelper.ApplyPrimaryButtonStyle(btnGenerate, new Size(126, 34));
            UIStyleHelper.ApplySuccessButtonStyle(btnStart, new Size(96, 34));
            UIStyleHelper.ApplyDangerButtonStyle(btnStop, new Size(92, 34));
            UIStyleHelper.ApplySecondaryButtonStyle(btnReset, new Size(96, 34));
            UIStyleHelper.ApplySecondaryButtonStyle(btnExportLog, new Size(96, 34));

            foreach (var numeric in new[] { nudNam, nudTuSo, nudDenSo })
            {
                numeric.Font = UIFonts.TextStandard;
                numeric.BorderStyle = BorderStyle.FixedSingle;
                numeric.Height = 25;
            }
            nudNam.Value = Math.Min(nudNam.Maximum, Math.Max(nudNam.Minimum, DateTime.Now.Year));

            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Padding = new Padding(10, 0, 10, 0);
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus);
            UIStyleHelper.ApplyDataGridViewStyle(dgvBatchList);
            dgvBatchList.Dock = DockStyle.None;

            Resize += (_, _) => LayoutModern();
            LayoutModern();
        }

        private void LayoutModern()
        {
            SuspendLayout();

            var contentTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;
            var contentWidth = Math.Max(680, Width - PagePadding * 2);

            grpInput.Location = new Point(PagePadding, contentTop);
            grpInput.Size = new Size(contentWidth, 98);

            lblNam.Location = new Point(18, 34);
            nudNam.Location = new Point(62, 30);
            nudNam.Size = new Size(86, 25);

            lblTuSo.Location = new Point(174, 34);
            nudTuSo.Location = new Point(226, 30);
            nudTuSo.Size = new Size(104, 25);

            lblDenSo.Location = new Point(356, 34);
            nudDenSo.Location = new Point(418, 30);
            nudDenSo.Size = new Size(104, 25);

            btnGenerate.Location = new Point(Math.Max(548, grpInput.ClientSize.Width - btnGenerate.Width - 18), 26);

            lblNote.Location = new Point(18, 66);
            lblNote.Size = new Size(Math.Max(480, grpInput.ClientSize.Width - 36), 22);

            grpProgress.Location = new Point(PagePadding, grpInput.Bottom + 10);
            grpProgress.Size = new Size(contentWidth, 104);

            lblConnectionStatus.Location = new Point(18, 25);
            lblConnectionStatus.Size = new Size(260, 22);
            lblSummary.Location = new Point(Math.Min(300, grpProgress.ClientSize.Width - 280), 25);
            lblSummary.Size = new Size(Math.Max(220, grpProgress.ClientSize.Width - lblSummary.Left - 18), 22);
            lblProgress.Location = new Point(18, 58);
            lblProgress.Size = new Size(Math.Max(360, grpProgress.ClientSize.Width - 420), 22);

            progressBar.Location = new Point(18, 78);
            progressBar.Size = new Size(Math.Max(360, grpProgress.ClientSize.Width - 300), 14);

            lblStatus.Location = new Point(progressBar.Right + 14, 67);
            lblStatus.Size = new Size(Math.Max(180, grpProgress.ClientSize.Width - lblStatus.Left - 18), 26);

            grpActions.Location = new Point(PagePadding, grpProgress.Bottom + 10);
            grpActions.Size = new Size(contentWidth, 66);

            btnStart.Location = new Point(18, 24);
            btnStop.Location = new Point(btnStart.Right + 10, 24);
            btnReset.Location = new Point(btnStop.Right + 10, 24);
            btnExportLog.Location = new Point(grpActions.ClientSize.Width - btnExportLog.Width - 18, 24);

            grpResults.Location = new Point(PagePadding, grpActions.Bottom + 10);
            grpResults.Size = new Size(contentWidth, Math.Max(260, Height - grpActions.Bottom - PagePadding - 10));

            dgvBatchList.Location = new Point(16, 28);
            dgvBatchList.Size = new Size(grpResults.ClientSize.Width - 32, grpResults.ClientSize.Height - 44);

            ResumeLayout(false);
        }

        private void SetupBatchGridColumns()
        {
            dgvBatchList.Columns.Clear();
            dgvBatchList.AutoGenerateColumns = false;
            dgvBatchList.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("SoPhieu", "Số phiếu", "SoPhieu", 100, 10));
            dgvBatchList.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("Nam", "Năm", "Nam", 80, 7));
            dgvBatchList.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("Status", "Trạng thái", "Status", 120, 12));
            dgvBatchList.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("Message", "Thông báo", "Message", 360, 34));
            dgvBatchList.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TransactionId", "Mã GD", "TransactionId", 90, 8));
            dgvBatchList.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("EmployeeName", "Nhân viên", "EmployeeName", 180, 18));
            dgvBatchList.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("EmployeeWarehouseCode", "Kho NV", "EmployeeWarehouseCode", 120, 11));
            dgvBatchList.Columns["Nam"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBatchList.Columns["TransactionId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBatchList.Columns["EmployeeWarehouseCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupBackgroundWorker()
        {
            batchWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            batchWorker.DoWork += BatchWorker_DoWork;
            batchWorker.ProgressChanged += BatchWorker_ProgressChanged;
            batchWorker.RunWorkerCompleted += BatchWorker_RunWorkerCompleted;
        }

        private void CheckERPConnection()
        {
            if (!xuatKhoBLL.TestERPConnection())
            {
                lblConnectionStatus.Text = "Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusError;
                btnStart.Enabled = false;
            }
            else
            {
                lblConnectionStatus.Text = "Kết nối ERP thành công";
                lblConnectionStatus.ForeColor = UIColorPalette.StatusSuccess;
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

                // Tạo danh sách phiếu cần xử lý
                batchItems.Clear();
                for (int soPhieu = tuSo; soPhieu <= denSo; soPhieu++)
                {
                    batchItems.Add(new BatchExportItem
                    {
                        SoPhieu = soPhieu.ToString(),
                        Nam = nam,
                        Status = BatchStatus.Pending,
                        Message = "Chờ xử lý"
                    });
                }

                // Hiển thị danh sách
                dgvBatchList.DataSource = null;
                dgvBatchList.DataSource = batchItems;

                lblSummary.Text = $"Tổng cộng: {batchItems.Count} phiếu cần xử lý";
                btnStart.Enabled = batchItems.Count > 0;
                btnGenerate.Enabled = false;
                
                UpdateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo danh sách: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (batchItems.Count == 0)
            {
                MessageBox.Show("Chưa có phiếu nào để xử lý!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Bắt đầu xử lý {batchItems.Count} phiếu xuất kho?\n\n" +
                "Quá trình này có thể mất thời gian và không thể hoàn tác.\n" +
                "Lưu ý: Hệ thống sẽ tự động xác định kho nhân viên và xử lý vật tư VP phù hợp.",
                "Xác nhận",
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
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnGenerate.Enabled = false;
            btnReset.Enabled = false;
            
            progressBar.Value = 0;
            progressBar.Maximum = batchItems.Count;
            
            lblStatus.Text = "Đang xử lý...";
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Processing);

            batchWorker.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (batchWorker.IsBusy)
            {
                batchWorker.CancelAsync();
                lblStatus.Text = "Đang dừng...";
                UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Warning);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            batchItems.Clear();
            dgvBatchList.DataSource = null;
            progressBar.Value = 0;
            lblSummary.Text = "";
            lblStatus.Text = "Sẵn sàng";
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Ready);
            
            btnGenerate.Enabled = true;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnReset.Enabled = true;
            
            UpdateProgress();
        }

        private void BatchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var currentUser = AuthenticationBLL.GetCurrentUser();
            
            if (currentUser == null)
            {
                e.Result = "Không thể xác định người dùng hiện tại";
                return;
            }

            int processed = 0;
            int successful = 0;
            int failed = 0;

            foreach (var item in batchItems)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                try
                {
                    // Cập nhật trạng thái đang xử lý
                    item.Status = BatchStatus.Processing;
                    item.Message = "Đang xử lý...";
                    worker.ReportProgress(processed, item);

                    // Xử lý phiếu xuất kho
                    ProcessSingleExportOrder(item, currentUser.Username);
                    
                    item.Status = BatchStatus.Success;
                    item.Message = "Thành công";
                    successful++;
                }
                catch (Exception ex)
                {
                    item.Status = BatchStatus.Failed;
                    item.Message = ex.Message;
                    failed++;
                }

                processed++;
                worker.ReportProgress(processed, item);

                // Delay nhỏ để tránh quá tải
                System.Threading.Thread.Sleep(100);
            }

            e.Result = $"Hoàn thành. Thành công: {successful}, Lỗi: {failed}";
        }

        private void ProcessSingleExportOrder(BatchExportItem item, string username)
        {
            // Lấy phiếu xuất từ ERP với mapping
            var erpOrder = xuatKhoBLL.GetExportOrderWithMapping(item.SoPhieu, item.Nam);
            if (erpOrder == null)
            {
                throw new Exception($"Không tìm thấy phiếu {item.SoPhieu}-{item.Nam}");
            }

            // Kiểm tra mapping vật tư
            var (total, mapped, unmapped, missingWarehouses) = xuatKhoBLL.GetMappingStatus(erpOrder);
            if (unmapped > 0)
            {
                throw new Exception($"Còn {unmapped}/{total} vật tư chưa được mapping");
            }

            if (missingWarehouses > 0)
            {
                throw new Exception($"Còn {missingWarehouses} vật tư chưa xác định kho nguồn");
            }

            // Tìm kho nhân viên
            var employeeWarehouse = warehouseDAL.GetKhoUuTienByMaNV(erpOrder.MaNhanVien);
            int employeeWarehouseId = 0; // Mặc định 0 cho trường hợp tất cả VP

            if (employeeWarehouse != null)
            {
                employeeWarehouseId = employeeWarehouse.Id;
                item.EmployeeWarehouseCode = employeeWarehouse.MaKho;
                item.EmployeeName = erpOrder.TenNhanVien;
            }
            else
            {
                // Kiểm tra xem có vật tư không phải VP không
                // Điều này sẽ được validate trong ProcessExport
                item.EmployeeName = erpOrder.TenNhanVien;
                item.EmployeeWarehouseCode = "Không có (VP)";
            }

            // Thực hiện xuất kho (BLL sẽ tự động validate VP vs không VP)
            int transactionId = xuatKhoBLL.ProcessExport(erpOrder, employeeWarehouseId, username);
            
            item.TransactionId = transactionId;
        }

        private void BatchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            
            if (e.UserState is BatchExportItem item)
            {
                // Refresh DataGridView để hiển thị cập nhật
                dgvBatchList.Refresh();
                
                lblStatus.Text = $"Đang xử lý: {item.SoPhieu}-{item.Nam}";
            }
            
            UpdateProgress();
        }

        private void BatchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessing = false;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            btnReset.Enabled = true;
            btnGenerate.Enabled = true;

            if (e.Cancelled)
            {
                lblStatus.Text = "Đã dừng xử lý";
                UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Warning);
            }
            else if (e.Error != null)
            {
                lblStatus.Text = $"Lỗi: {e.Error.Message}";
                UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Error);
            }
            else
            {
                lblStatus.Text = e.Result?.ToString() ?? "Hoàn thành";
                UIStyleHelper.ApplyStatusLabelStyle(lblStatus, StatusType.Success);
            }

            UpdateProgress();
            dgvBatchList.Refresh();
        }

        private void UpdateProgress()
        {
            if (batchItems.Count == 0)
            {
                lblProgress.Text = "";
                return;
            }

            int pending = batchItems.Count(x => x.Status == BatchStatus.Pending);
            int processing = batchItems.Count(x => x.Status == BatchStatus.Processing);
            int success = batchItems.Count(x => x.Status == BatchStatus.Success);
            int failed = batchItems.Count(x => x.Status == BatchStatus.Failed);

            lblProgress.Text = $"Chờ: {pending} | Đang xử lý: {processing} | Thành công: {success} | Lỗi: {failed}";
        }

        private void dgvBatchList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < batchItems.Count)
            {
                var item = batchItems[e.RowIndex];
                var row = dgvBatchList.Rows[e.RowIndex];

                switch (item.Status)
                {
                    case BatchStatus.Pending:
                        row.DefaultCellStyle.BackColor = UIColorPalette.SurfaceMuted;
                        break;
                    case BatchStatus.Processing:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(219, 234, 254);
                        break;
                    case BatchStatus.Success:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231);
                        break;
                    case BatchStatus.Failed:
                        row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        break;
                }
            }
        }

        private void btnExportLog_Click(object sender, EventArgs e)
        {
            if (batchItems.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    FileName = $"XuatKhoERP_Batch_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCsv(saveDialog.FileName);
                    MessageBox.Show("Đã xuất file thành công!", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCsv(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Header
                writer.WriteLine("SoPhieu,Nam,TrangThai,ThongBao,MaGiaoDich,TenNhanVien,MaKhoNhanVien");

                // Data
                foreach (var item in batchItems)
                {
                    writer.WriteLine($"{item.SoPhieu},{item.Nam},{item.Status},{item.Message},{item.TransactionId},{item.EmployeeName},{item.EmployeeWarehouseCode}");
                }
            }
        }
    }

    /// <summary>
    /// Model cho item trong batch export process
    /// </summary>
    public class BatchExportItem
    {
        public string SoPhieu { get; set; } = "";
        public int Nam { get; set; }
        public BatchStatus Status { get; set; } = BatchStatus.Pending;
        public string Message { get; set; } = "";
        public int TransactionId { get; set; }
        public string EmployeeName { get; set; } = "";
        public string EmployeeWarehouseCode { get; set; } = "";
    }
}
