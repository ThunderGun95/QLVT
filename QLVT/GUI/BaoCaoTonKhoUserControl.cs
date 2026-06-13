using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class BaoCaoTonKhoUserControl : UserControl
    {
        private const int PagePadding = 18;
        private readonly BaoCaoTonKhoBLL inventoryReportBLL;
        private readonly ListBox warehouseListBox = new();
        private List<InventoryReportItem> currentReportData = new();
        private InventoryReportSummary currentSummary = new();
        private List<Warehouse> _warehouses = new();
        private bool suppressTextChanged;

        public BaoCaoTonKhoUserControl()
        {
            inventoryReportBLL = new BaoCaoTonKhoBLL();
            InitializeComponent();
            ApplyModernStyle();
            InitializeData();
        }

        private void InitializeData()
        {
            SetupDataGridView();
            LoadWarehouses();
            ResetFilter();
            UpdateStatus("Sẵn sàng tạo báo cáo tồn kho", UIColorPalette.StatusProcessing);
        }

        private void ApplyModernStyle()
        {
            UIStyleHelper.ApplyFormStyle(this);

            lblTitle.Text = "BÁO CÁO TỒN KHO";
            UIStyleHelper.ApplyTitleBarStyle(lblTitle);

            grpFilter.Text = "Điều kiện lọc";
            grpReport.Text = "Kết quả báo cáo";
            UIStyleHelper.ApplyGroupBoxStyle(grpFilter);
            UIStyleHelper.ApplyGroupBoxStyle(grpReport);

            lblWarehouse.Text = "Kho";
            lblSearch.Text = "Vật tư";
            UIStyleHelper.ApplyStandardLabelStyle(lblWarehouse);
            UIStyleHelper.ApplyStandardLabelStyle(lblSearch);

            txtWarehouse.PlaceholderText = "Nhập tên kho hoặc 'Tất cả'";
            txtSearch.PlaceholderText = "Nhập mã hoặc tên vật tư";
            UIStyleHelper.ApplyTextBoxStyle(txtWarehouse);
            UIStyleHelper.ApplyTextBoxStyle(txtSearch);

            chkChiHienThiCoTon.Text = "Chỉ hiện vật tư còn tồn";
            UIStyleHelper.ApplyCheckBoxStyle(chkChiHienThiCoTon);

            btnCreateReport.Text = "Tạo báo cáo";
            btnExportExcel.Text = "Xuất Excel";
            btnResetFilter.Text = "Đặt lại";
            UIStyleHelper.ApplyPrimaryButtonStyle(btnCreateReport, new Size(124, 36));
            UIStyleHelper.ApplySuccessButtonStyle(btnExportExcel, new Size(112, 36));
            UIStyleHelper.ApplySecondaryButtonStyle(btnResetFilter, new Size(96, 36));

            UIStyleHelper.ApplyDataGridViewStyle(dgvReport);

            lblSummary.BackColor = UIColorPalette.SurfaceMuted;
            lblSummary.ForeColor = UIColorPalette.TextDark;
            lblSummary.Font = UIFonts.HeaderStandard;
            lblSummary.BorderStyle = BorderStyle.FixedSingle;
            lblSummary.TextAlign = ContentAlignment.MiddleLeft;
            lblSummary.Padding = new Padding(10, 0, 10, 0);

            lblStatus.BackColor = UIColorPalette.BackgroundWhite;
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Padding = new Padding(10, 0, 10, 0);
            UIStyleHelper.ApplyStatusLabelStyle(lblStatus);

            warehouseListBox.Font = UIFonts.TextStandard;
            warehouseListBox.BorderStyle = BorderStyle.FixedSingle;
            warehouseListBox.BackColor = UIColorPalette.BackgroundWhite;
            warehouseListBox.ForeColor = UIColorPalette.TextDark;

            Resize += (_, _) => LayoutModern();
            LayoutModern();
        }

        private void LayoutModern()
        {
            SuspendLayout();

            var contentTop = UIStyleHelper.PageHeaderHeight + UIStyleHelper.PageHeaderContentGap;

            grpFilter.Location = new Point(PagePadding, contentTop);
            grpFilter.Size = new Size(Math.Max(760, Width - PagePadding * 2), 88);

            var filterWidth = grpFilter.ClientSize.Width;
            var buttonY = 32;
            var right = filterWidth - 16;
            btnResetFilter.Location = new Point(right - btnResetFilter.Width, buttonY);
            btnExportExcel.Location = new Point(btnResetFilter.Left - 10 - btnExportExcel.Width, buttonY);
            btnCreateReport.Location = new Point(btnExportExcel.Left - 10 - btnCreateReport.Width, buttonY);

            lblWarehouse.Location = new Point(16, 36);
            lblWarehouse.Size = new Size(42, 24);
            txtWarehouse.Location = new Point(62, 32);
            txtWarehouse.Size = new Size(220, 25);

            lblSearch.Location = new Point(txtWarehouse.Right + 18, 36);
            lblSearch.Size = new Size(48, 24);

            var searchRight = Math.Max(lblSearch.Right + 180, btnCreateReport.Left - 18);
            txtSearch.Location = new Point(lblSearch.Right + 8, 32);
            txtSearch.Size = new Size(Math.Max(180, searchRight - txtSearch.Left), 25);

            chkChiHienThiCoTon.Location = new Point(62, 60);
            chkChiHienThiCoTon.Size = new Size(190, 22);

            grpReport.Location = new Point(PagePadding, 156);
            grpReport.Size = new Size(Math.Max(760, Width - PagePadding * 2), Math.Max(260, Height - 214));

            dgvReport.Location = new Point(16, 28);
            dgvReport.Size = new Size(grpReport.ClientSize.Width - 32, Math.Max(160, grpReport.ClientSize.Height - 78));

            lblSummary.Location = new Point(16, dgvReport.Bottom + 8);
            lblSummary.Size = new Size(grpReport.ClientSize.Width - 32, 32);

            lblStatus.Location = new Point(PagePadding, Height - 44);
            lblStatus.Size = new Size(Math.Max(760, Width - PagePadding * 2), 28);

            if (warehouseListBox.Visible)
            {
                PositionWarehouseDropdown();
            }

            ResumeLayout(false);
        }

        private void SetupDataGridView()
        {
            dgvReport.Columns.Clear();

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 56,
                FillWeight = 5,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CodeVatTu",
                HeaderText = "Mã vật tư",
                DataPropertyName = "CodeVatTu",
                Width = 110,
                FillWeight = 12
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                Width = 300,
                FillWeight = 34
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonViTinh",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                Width = 70,
                FillWeight = 8,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenKho",
                HeaderText = "Kho",
                DataPropertyName = "TenKho",
                Width = 160,
                FillWeight = 20
            });

            dgvReport.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongTon",
                HeaderText = "SL tồn",
                DataPropertyName = "SoLuongTon",
                Width = 100,
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            });

            UIStyleHelper.ApplyDataGridViewStyle(dgvReport);
        }

        private void LoadWarehouses()
        {
            try
            {
                _warehouses = inventoryReportBLL.GetWarehousesForFilter();
                txtWarehouse.AutoCompleteMode = AutoCompleteMode.None;
                txtWarehouse.TextChanged += TxtWarehouse_TextChanged;
                txtWarehouse.KeyDown += TxtWarehouse_KeyDown;
                txtWarehouse.Leave += TxtWarehouse_Leave;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetFilter()
        {
            var defaultFilter = inventoryReportBLL.GetDefaultFilter();

            txtWarehouse.Text = "";
            txtSearch.Text = "";
            chkChiHienThiCoTon.Checked = defaultFilter.ChiHienThiCoTon;
        }

        private async void btnCreateReport_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus("Đang tạo báo cáo...", UIColorPalette.StatusProcessing);
                btnCreateReport.Enabled = false;
                btnExportExcel.Enabled = false;

                var filter = GetCurrentFilter();

                await Task.Run(() =>
                {
                    var (success, message, data, summary) = inventoryReportBLL.GetInventoryReport(filter);

                    Invoke((MethodInvoker)delegate
                    {
                        if (success)
                        {
                            currentReportData = data;
                            currentSummary = summary;

                            dgvReport.DataSource = data;
                            lblSummary.Text = inventoryReportBLL.FormatSummaryText(summary);

                            UpdateStatus(message, UIColorPalette.StatusSuccess);
                            btnExportExcel.Enabled = data.Any();
                        }
                        else
                        {
                            dgvReport.DataSource = null;
                            lblSummary.Text = "";
                            UpdateStatus(message, UIColorPalette.StatusError);
                            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                UpdateStatus($"Lỗi: {ex.Message}", UIColorPalette.StatusError);
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCreateReport.Enabled = true;
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!currentReportData.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var (success, message, _) = inventoryReportBLL.ExportToExcel(currentReportData, currentSummary);

                if (success)
                {
                    MessageBox.Show($"Xuất Excel thành công.\n{message}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatus("Xuất Excel thành công", UIColorPalette.StatusSuccess);
                }
                else
                {
                    MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus(message, UIColorPalette.StatusError);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"Lỗi xuất Excel: {ex.Message}", UIColorPalette.StatusError);
            }
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            ResetFilter();
            dgvReport.DataSource = null;
            lblSummary.Text = "";
            currentReportData.Clear();
            currentSummary = new InventoryReportSummary();
            UpdateStatus("Đã đặt lại bộ lọc", UIColorPalette.StatusProcessing);
        }

        private InventoryReportFilter GetCurrentFilter()
        {
            var searchText = txtSearch.Text.Trim();
            int? warehouseId = null;
            var warehouseText = txtWarehouse.Text.Trim();

            if (!string.IsNullOrEmpty(warehouseText) &&
                !warehouseText.Equals("Tất cả", StringComparison.OrdinalIgnoreCase) &&
                !warehouseText.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var exactWarehouse = _warehouses.FirstOrDefault(w =>
                    w.TenKho.Equals(warehouseText, StringComparison.OrdinalIgnoreCase));

                if (exactWarehouse != null)
                {
                    warehouseId = exactWarehouse.Id;
                }
                else
                {
                    var partialWarehouse = _warehouses.FirstOrDefault(w =>
                        w.TenKho.Contains(warehouseText, StringComparison.OrdinalIgnoreCase));
                    warehouseId = partialWarehouse?.Id;
                }
            }

            return new InventoryReportFilter
            {
                AsOfDate = DateTime.Now.Date,
                WarehouseId = warehouseId,
                CodeVatTu = searchText,
                TenVatTu = searchText,
                NhaSanXuat = null,
                ChiHienThiCoTon = chkChiHienThiCoTon.Checked
            };
        }

        private void UpdateStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnCreateReport_Click(sender, e);
            }
        }

        private void txtWarehouse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnCreateReport_Click(sender, e);
            }
        }

        private void dgvReport_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void TxtWarehouse_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChanged) return;

            var searchText = txtWarehouse.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                HideWarehouseDropdown();
                return;
            }

            var matchingItems = new List<string>();

            if ("Tất cả".Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                "tat ca".Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                "all".Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                matchingItems.Add("Tất cả");
            }

            matchingItems.AddRange(_warehouses
                .Where(w => w.TenKho.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .Select(w => w.TenKho));

            if (matchingItems.Any())
            {
                ShowWarehouseDropdown(matchingItems);
            }
            else
            {
                HideWarehouseDropdown();
            }
        }

        private void TxtWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            if (!warehouseListBox.Visible) return;

            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (warehouseListBox.SelectedIndex < warehouseListBox.Items.Count - 1)
                        warehouseListBox.SelectedIndex++;
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (warehouseListBox.SelectedIndex > 0)
                        warehouseListBox.SelectedIndex--;
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    if (warehouseListBox.SelectedIndex >= 0)
                    {
                        SelectWarehouse(warehouseListBox.SelectedItem?.ToString() ?? "");
                    }
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    HideWarehouseDropdown();
                    e.Handled = true;
                    break;
            }
        }

        private void TxtWarehouse_Leave(object sender, EventArgs e)
        {
            Task.Delay(160).ContinueWith(_ =>
            {
                if (!IsHandleCreated) return;

                Invoke((MethodInvoker)delegate
                {
                    if (!warehouseListBox.Focused)
                    {
                        HideWarehouseDropdown();
                    }
                });
            });
        }

        private void ShowWarehouseDropdown(List<string> items)
        {
            if (!Controls.Contains(warehouseListBox))
            {
                Controls.Add(warehouseListBox);
                warehouseListBox.BringToFront();

                warehouseListBox.Click += (s, e) =>
                {
                    if (warehouseListBox.SelectedItem != null)
                    {
                        SelectWarehouse(warehouseListBox.SelectedItem.ToString() ?? "");
                    }
                };
            }

            warehouseListBox.Items.Clear();
            warehouseListBox.Items.AddRange(items.Distinct().ToArray());
            PositionWarehouseDropdown();
            warehouseListBox.Visible = true;

            if (warehouseListBox.Items.Count > 0)
            {
                warehouseListBox.SelectedIndex = 0;
            }
        }

        private void PositionWarehouseDropdown()
        {
            var txtLocation = txtWarehouse.PointToScreen(Point.Empty);
            var parentLocation = PointToScreen(Point.Empty);

            warehouseListBox.Location = new Point(
                txtLocation.X - parentLocation.X,
                txtLocation.Y - parentLocation.Y + txtWarehouse.Height + 2);
            warehouseListBox.Width = txtWarehouse.Width;
            warehouseListBox.Height = Math.Min(180, Math.Max(36, warehouseListBox.Items.Count * 28 + 4));
            warehouseListBox.BringToFront();
        }

        private void HideWarehouseDropdown()
        {
            warehouseListBox.Visible = false;
        }

        private void SelectWarehouse(string warehouseName)
        {
            suppressTextChanged = true;
            txtWarehouse.Text = warehouseName;
            suppressTextChanged = false;

            HideWarehouseDropdown();
            txtWarehouse.SelectionStart = txtWarehouse.Text.Length;
        }
    }
}
