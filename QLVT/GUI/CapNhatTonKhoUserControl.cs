using QLVT.BLL;
using QLVT.Models;
using QLVT.Utils;
using System.ComponentModel;

namespace QLVT.GUI
{
    public class CapNhatTonKhoUserControl : UserControl
    {
        private const int PagePadding = 18;

        private readonly CapNhatTonKhoBLL capNhatTonKhoBLL = new();
        private readonly Label lblTitle = new();
        private readonly GroupBox grpActions = new();
        private readonly GroupBox grpResult = new();
        private readonly Button btnRaSoat = new();
        private readonly Button btnCapNhat = new();
        private readonly Button btnLamMoi = new();
        private readonly CheckBox chkZeroMissing = new();
        private readonly Label lblSummary = new();
        private readonly Label lblStatus = new();
        private readonly DataGridView dgvResult = new();
        private readonly BindingList<TonKhoRebuildItem> bindingData = new();

        public CapNhatTonKhoUserControl()
        {
            InitializeComponent();
            ApplyModernStyle();
            SetupGrid();
            BindEvents();
            UpdateStatus("Sẵn sàng rà soát tồn kho", StatusType.Ready);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Controls.Add(lblTitle);
            Controls.Add(grpActions);
            Controls.Add(grpResult);
            Controls.Add(lblStatus);

            grpActions.Controls.Add(btnRaSoat);
            grpActions.Controls.Add(btnCapNhat);
            grpActions.Controls.Add(btnLamMoi);
            grpActions.Controls.Add(chkZeroMissing);
            grpActions.Controls.Add(lblSummary);

            grpResult.Controls.Add(dgvResult);

            Name = "CapNhatTonKhoUserControl";
            Size = new Size(1200, 720);
            ResumeLayout(false);
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "CẬP NHẬT TỒN KHO";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            grpActions.Text = "Rà soát và cập nhật";
            grpResult.Text = "Danh sách chênh lệch";
            UIStyleHelper.ApplyGroupBoxStyle(grpActions);
            UIStyleHelper.ApplyGroupBoxStyle(grpResult);

            btnRaSoat.Text = "Rà soát";
            btnCapNhat.Text = "Cập nhật tồn kho";
            btnLamMoi.Text = "Làm mới";
            UIStyleHelper.ApplyPrimaryButtonStyle(btnRaSoat, new Size(112, 36));
            UIStyleHelper.ApplyDangerButtonStyle(btnCapNhat, new Size(150, 36));
            UIStyleHelper.ApplySecondaryButtonStyle(btnLamMoi, new Size(104, 36));

            chkZeroMissing.Text = "Đưa tồn không có giao dịch về 0";
            chkZeroMissing.Checked = true;
            UIStyleHelper.ApplyCheckBoxStyle(chkZeroMissing);

            lblSummary.BackColor = UIColorPalette.SurfaceMuted;
            lblSummary.ForeColor = UIColorPalette.TextDark;
            lblSummary.Font = UIFonts.HeaderStandard;
            lblSummary.BorderStyle = BorderStyle.FixedSingle;
            lblSummary.TextAlign = ContentAlignment.MiddleLeft;
            lblSummary.Padding = new Padding(10, 0, 10, 0);

            UIStyleHelper.ApplyDataGridViewStyle(dgvResult);

            lblStatus.BackColor = UIColorPalette.BackgroundWhite;
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Padding = new Padding(10, 0, 10, 0);
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus);

            Resize += (_, _) => LayoutModern();
            LayoutModern();
        }

        private void LayoutModern()
        {
            SuspendLayout();

            var contentTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;

            grpActions.Location = new Point(PagePadding, contentTop);
            grpActions.Size = new Size(Math.Max(760, Width - PagePadding * 2), 92);

            btnRaSoat.Location = new Point(18, 34);
            btnCapNhat.Location = new Point(btnRaSoat.Right + 10, 34);
            btnLamMoi.Location = new Point(btnCapNhat.Right + 10, 34);
            chkZeroMissing.Location = new Point(btnLamMoi.Right + 18, 40);
            chkZeroMissing.Size = new Size(240, 24);

            lblSummary.Location = new Point(Math.Max(chkZeroMissing.Right + 16, grpActions.ClientSize.Width - 360), 34);
            lblSummary.Size = new Size(Math.Min(340, grpActions.ClientSize.Width - lblSummary.Left - 16), 36);

            grpResult.Location = new Point(PagePadding, grpActions.Bottom + 12);
            grpResult.Size = new Size(Math.Max(760, Width - PagePadding * 2), Math.Max(300, Height - grpActions.Bottom - 72));

            dgvResult.Location = new Point(16, 28);
            dgvResult.Size = new Size(grpResult.ClientSize.Width - 32, grpResult.ClientSize.Height - 44);

            lblStatus.Location = new Point(PagePadding, Height - 44);
            lblStatus.Size = new Size(Math.Max(760, Width - PagePadding * 2), 28);

            ResumeLayout(false);
        }

        private void SetupGrid()
        {
            dgvResult.Columns.Clear();
            dgvResult.DataSource = bindingData;

            dgvResult.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 54,
                FillWeight = 5,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvResult.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("MaKho", "Mã kho", "MaKho", 90, 8));
            dgvResult.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TenKho", "Tên kho", "TenKho", 180, 18));
            dgvResult.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("MaVatTu", "Mã vật tư", "MaVatTu", 110, 11));
            dgvResult.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TenVatTu", "Tên vật tư", "TenVatTu", 260, 26));
            dgvResult.Columns.Add(UIStyleHelper.CreateNumericColumn("SoLuongNhap", "Tổng nhập", "SoLuongNhap", 100, 10));
            dgvResult.Columns.Add(UIStyleHelper.CreateNumericColumn("SoLuongXuat", "Tổng xuất", "SoLuongXuat", 100, 10));
            dgvResult.Columns.Add(UIStyleHelper.CreateNumericColumn("TonHienTai", "Tồn hiện tại", "TonHienTai", 110, 11));
            dgvResult.Columns.Add(UIStyleHelper.CreateNumericColumn("TonTinhLai", "Tồn tính lại", "TonTinhLai", 110, 11));
            dgvResult.Columns.Add(UIStyleHelper.CreateNumericColumn("ChenhLech", "Chênh lệch", "ChenhLech", 110, 11));
            dgvResult.Columns.Add(UIStyleHelper.CreateReadOnlyColumn("TrangThai", "Trạng thái", "TrangThai", 140, 12));

            UIStyleHelper.ApplyDataGridViewStyle(dgvResult);
        }

        private void BindEvents()
        {
            btnRaSoat.Click += async (_, _) => await RaSoatAsync();
            btnCapNhat.Click += async (_, _) => await CapNhatAsync();
            btnLamMoi.Click += (_, _) => ClearResult();
        }

        private async Task RaSoatAsync()
        {
            await RunWithBusyStateAsync("Đang rà soát tồn kho...", () =>
            {
                var data = capNhatTonKhoBLL.GetChenhLechTonKho(chkZeroMissing.Checked);
                BindResult(data);
                UpdateStatus(data.Count == 0 ? "Tồn kho đang khớp với lịch sử giao dịch" : $"Phát hiện {data.Count:N0} dòng chênh lệch",
                    data.Count == 0 ? StatusType.Success : StatusType.Warning);
            });
        }

        private async Task CapNhatAsync()
        {
            var confirm = MessageBox.Show(
                "Thao tác này sẽ cập nhật lại bảng Inventory theo toàn bộ TransactionDetails hiện có. Bạn muốn tiếp tục?",
                "Xác nhận cập nhật tồn kho",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            await RunWithBusyStateAsync("Đang cập nhật lại tồn kho...", () =>
            {
                var result = capNhatTonKhoBLL.RebuildTonKho(chkZeroMissing.Checked);
                var data = capNhatTonKhoBLL.GetChenhLechTonKho(chkZeroMissing.Checked);
                BindResult(data);
                UpdateStatus($"Đã cập nhật {result.TotalChanged:N0} dòng tồn kho. Còn lệch: {data.Count:N0}", data.Count == 0 ? StatusType.Success : StatusType.Warning);
            });
        }

        private async Task RunWithBusyStateAsync(string status, Action action)
        {
            try
            {
                SetBusy(true);
                UpdateStatus(status, StatusType.Processing);
                await Task.Run(action);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi: {ex.Message}", StatusType.Error);
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void BindResult(List<TonKhoRebuildItem> data)
        {
            if (InvokeRequired)
            {
                Invoke(() => BindResult(data));
                return;
            }

            bindingData.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                data[i].STT = i + 1;
                bindingData.Add(data[i]);
            }

            lblSummary.Text = $"Chênh lệch: {data.Count:N0} dòng";
        }

        private void ClearResult()
        {
            bindingData.Clear();
            lblSummary.Text = "Chưa rà soát";
            UpdateStatus("Sẵn sàng rà soát tồn kho", StatusType.Ready);
        }

        private void SetBusy(bool busy)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetBusy(busy));
                return;
            }

            btnRaSoat.Enabled = !busy;
            btnCapNhat.Enabled = !busy;
            btnLamMoi.Enabled = !busy;
            chkZeroMissing.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void UpdateStatus(string message, StatusType status)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateStatus(message, status));
                return;
            }

            lblStatus.Text = message;
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus, status);
        }
    }
}
