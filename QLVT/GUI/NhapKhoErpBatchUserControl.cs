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

namespace QLVT.GUI
{
    /// <summary>
    /// UserControl để nhập hàng loạt phiếu nhập kho từ ERP
    /// </summary>
    public partial class NhapKhoErpBatchUserControl : UserControl
    {
        private readonly NhapKhoBLL nhapKhoBLL;
        private List<BatchProcessItem> batchItems = new();
        private BackgroundWorker batchWorker;
        private bool isProcessing = false;

        public NhapKhoErpBatchUserControl()
        {
            InitializeComponent();
            nhapKhoBLL = new NhapKhoBLL();
            SetupBackgroundWorker();
            CheckERPConnection();
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
            if (!nhapKhoBLL.TestERPConnection())
            {
                lblConnectionStatus.Text = "❌ Không thể kết nối ERP";
                lblConnectionStatus.ForeColor = Color.Red;
                btnStart.Enabled = false;
            }
            else
            {
                lblConnectionStatus.Text = "✅ Kết nối ERP thành công";
                lblConnectionStatus.ForeColor = Color.Green;
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
                    batchItems.Add(new BatchProcessItem
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
                $"Bắt đầu xử lý {batchItems.Count} phiếu nhập kho?\n\n" +
                "Quá trình này có thể mất thời gian và không thể hoàn tác.",
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
            lblStatus.ForeColor = Color.Blue;

            batchWorker.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (batchWorker.IsBusy)
            {
                batchWorker.CancelAsync();
                lblStatus.Text = "Đang dừng...";
                lblStatus.ForeColor = Color.Orange;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            batchItems.Clear();
            dgvBatchList.DataSource = null;
            progressBar.Value = 0;
            lblSummary.Text = "";
            lblStatus.Text = "Sẵn sàng";
            lblStatus.ForeColor = Color.Black;
            
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

                    // Xử lý phiếu nhập kho
                    ProcessSingleOrder(item, currentUser.Username);
                    
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

            e.Result = $"Hoàn thành! Thành công: {successful}, Lỗi: {failed}";
        }

        private void ProcessSingleOrder(BatchProcessItem item, string username)
        {
            // Lấy phiếu nhập từ ERP
            var erpOrder = nhapKhoBLL.GetPhieuNhapKhoErpWithMapping(item.SoPhieu, item.Nam);
            if (erpOrder == null)
            {
                throw new Exception($"Không tìm thấy phiếu {item.SoPhieu}-{item.Nam}");
            }

            // Kiểm tra mapping
            var (total, mapped, unmapped) = nhapKhoBLL.GetMappingStatus(erpOrder);
            if (unmapped > 0)
            {
                throw new Exception($"Còn {unmapped}/{total} vật tư chưa được mapping");
            }

            // Mapping warehouse
            string mappedWarehouseCode = ERPWarehouseMapping.MapERPToQLVT(erpOrder.MaKhoVatTu, erpOrder.ChiTiet);
            
            // Thực hiện nhập kho
            int transactionId = nhapKhoBLL.ProcessNhapKhoErp(erpOrder, mappedWarehouseCode, username);
            
            item.TransactionId = transactionId;
            item.WarehouseCode = mappedWarehouseCode;
        }

        private void BatchWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            
            if (e.UserState is BatchProcessItem item)
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
                lblStatus.ForeColor = Color.Orange;
            }
            else if (e.Error != null)
            {
                lblStatus.Text = $"Lỗi: {e.Error.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            else
            {
                lblStatus.Text = e.Result?.ToString() ?? "Hoàn thành";
                lblStatus.ForeColor = Color.Green;
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
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                        break;
                    case BatchStatus.Processing:
                        row.DefaultCellStyle.BackColor = Color.LightBlue;
                        break;
                    case BatchStatus.Success:
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        break;
                    case BatchStatus.Failed:
                        row.DefaultCellStyle.BackColor = Color.LightPink;
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
                    FileName = $"NhapKhoERP_Batch_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
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
                writer.WriteLine("SoPhieu,Nam,TrangThai,ThongBao,MaGiaoDich,MaKho");

                // Data
                foreach (var item in batchItems)
                {
                    writer.WriteLine($"{item.SoPhieu},{item.Nam},{item.Status},{item.Message},{item.TransactionId},{item.WarehouseCode}");
                }
            }
        }
    }

    /// <summary>
    /// Model cho item trong batch process
    /// </summary>
    public class BatchProcessItem
    {
        public string SoPhieu { get; set; } = "";
        public int Nam { get; set; }
        public BatchStatus Status { get; set; } = BatchStatus.Pending;
        public string Message { get; set; } = "";
        public int TransactionId { get; set; }
        public string WarehouseCode { get; set; } = "";
    }

    /// <summary>
    /// Trạng thái xử lý batch
    /// </summary>
    public enum BatchStatus
    {
        Pending,
        Processing,
        Success,
        Failed
    }
}