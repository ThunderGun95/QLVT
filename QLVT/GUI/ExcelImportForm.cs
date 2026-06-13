using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using QLVT.Models;
using QLVT.BLL;
using QLVT.DAL;

namespace QLVT.GUI
{
    public partial class ExcelImportForm : Form
    {
        public List<ExcelImportItem>? ImportedItems { get; private set; }
        private readonly WarehouseDAL warehouseDAL;

        public ExcelImportForm()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            warehouseDAL = new WarehouseDAL();
            LoadWarehouses();
            // ClosedXML không cần license setup
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
                openFileDialog.Title = "Chọn file Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                    LoadExcelPreview(openFileDialog.FileName);
                }
            }
        }

        private void LoadExcelPreview(string filePath)
        {
            try
            {
                var previewData = new List<ExcelImportItem>();

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        MessageBox.Show("File Excel không có worksheet nào!", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var range = worksheet.RangeUsed();
                    if (range == null || range.RowCount() < 2)
                    {
                        MessageBox.Show("File Excel phải có ít nhất 2 dòng (header + data)!", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Đọc từ dòng 2 (bỏ qua header)
                    for (int row = 2; row <= range.RowCount(); row++)
                    {
                        var itemCodeCell = worksheet.Cell(row, 1);
                        var quantityCell = worksheet.Cell(row, 2);
                        
                        var itemCode = itemCodeCell.GetString()?.Trim();
                        var quantityText = quantityCell.GetString()?.Trim();

                        if (string.IsNullOrWhiteSpace(itemCode)) continue;

                        if (int.TryParse(quantityText, out int quantity) && quantity > 0)
                        {
                            previewData.Add(new ExcelImportItem
                            {
                                ItemCode = itemCode,
                                Quantity = quantity
                            });
                        }
                    }
                }

                // Hiển thị preview
                dgvPreview.DataSource = null;
                dgvPreview.DataSource = previewData;

                if (dgvPreview.Columns.Count >= 2)
                {
                    dgvPreview.Columns[0].HeaderText = "Mã vật tư";
                    dgvPreview.Columns[1].HeaderText = "Số lượng";
                }

                btnImport.Enabled = previewData.Count > 0;

                MessageBox.Show($"Đã tải {previewData.Count} dòng dữ liệu từ Excel!", "Thành công", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file Excel: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ preview grid
                var excelItems = dgvPreview.DataSource as List<ExcelImportItem>;
                if (excelItems == null || !excelItems.Any())
                {
                    MessageBox.Show("Không có dữ liệu để import!", "Cảnh báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra có chọn kho chưa
                if (cboWarehouse.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn kho nhập!", "Cảnh báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra có dữ liệu hợp lệ
                var validItems = excelItems.Where(item => item.IsMapped && item.Quantity > 0).ToList();
                if (!validItems.Any())
                {
                    MessageBox.Show("Không có dữ liệu hợp lệ để import!\nVui lòng kiểm tra mapping và số lượng.", 
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Trả về dữ liệu đã xử lý cho form gọi
                ImportedItems = validItems;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Lỗi: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Lỗi khi chuẩn bị import: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSample_Click(object sender, EventArgs e)
        {
            // Tạo file mẫu Excel
            try
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                    saveFileDialog.FileName = "Mau_TonKho_DauKy.xlsx";
                    saveFileDialog.Title = "Lưu file mẫu";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        CreateSampleExcelFile(saveFileDialog.FileName);
                        MessageBox.Show($"Đã tạo file mẫu: {saveFileDialog.FileName}", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo file mẫu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSampleExcelFile(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("TonKhoDauKy");
                
                // Header
                worksheet.Cell(1, 1).Value = "Mã vật tư";
                worksheet.Cell(1, 2).Value = "Số lượng";
                
                // Sample data
                worksheet.Cell(2, 1).Value = "VT001";
                worksheet.Cell(2, 2).Value = 100;
                worksheet.Cell(3, 1).Value = "VT002";
                worksheet.Cell(3, 2).Value = 50;
                worksheet.Cell(4, 1).Value = "CC11000043";
                worksheet.Cell(4, 2).Value = 200;
                
                // Format
                worksheet.Range(1, 1, 1, 2).Style.Font.Bold = true;
                worksheet.Range(1, 1, 1, 2).Style.Fill.BackgroundColor = XLColor.LightBlue;
                worksheet.Columns().AdjustToContents();
                
                workbook.SaveAs(filePath);
            }
        }

        private void btnMapping_Click(object sender, EventArgs e)
        {
            // Chức năng mapping sẽ được thêm sau
            MessageBox.Show("Chức năng mapping đang được phát triển!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadWarehouses()
        {
            try
            {
                var warehouses = warehouseDAL.GetAllWarehouses();
                
                cboWarehouse.DisplayMember = "TenKho";
                cboWarehouse.ValueMember = "MaKho";
                cboWarehouse.DataSource = warehouses;
                
                if (warehouses.Any())
                {
                    cboWarehouse.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kho: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
