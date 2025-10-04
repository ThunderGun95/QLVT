using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class TimVatTuWithKhoForm : Form
    {
        private readonly XuatKhoManualBLL xuatKhoBLL;
        private readonly int warehouseId;
        private List<VatTuSearchResult> searchResults;
        
        public VatTuSearchResult? SelectedVatTu { get; private set; }

        // Controls
        private TextBox txtTimKiem = null!;
        private Button btnTimKiem = null!;
        private DataGridView dgvVatTu = null!;
        private Button btnChon = null!;
        private Button btnHuy = null!;
        private Label lblKetQua = null!;

        public TimVatTuWithKhoForm(int warehouseId)
        {
            this.warehouseId = warehouseId;
            xuatKhoBLL = new XuatKhoManualBLL();
            searchResults = new List<VatTuSearchResult>();
            
            InitializeComponent();
            LoadInitialData();
        }

        private void InitializeComponent()
        {
            Text = "Tìm kiếm vật tư";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Panel tìm kiếm
            var pnlSearch = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            var lblTimKiem = new Label
            {
                Text = "Tìm kiếm:",
                Location = new Point(10, 15),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtTimKiem = new TextBox
            {
                Location = new Point(100, 12),
                Size = new Size(400, 23),
                Font = new Font("Segoe UI", 10F)
            };

            btnTimKiem = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(520, 10),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };

            lblKetQua = new Label
            {
                Text = "Kết quả: 0",
                Location = new Point(640, 15),
                Size = new Size(100, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlSearch.Controls.AddRange(new Control[] { lblTimKiem, txtTimKiem, btnTimKiem, lblKetQua });

            // DataGridView
            dgvVatTu = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false
            };

            SetupDataGridView();

            // Panel buttons
            var pnlButtons = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10)
            };

            btnChon = new Button
            {
                Text = "Chọn",
                Size = new Size(100, 30),
                Location = new Point(580, 10),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            btnHuy = new Button
            {
                Text = "Hủy",
                Size = new Size(100, 30),
                Location = new Point(690, 10),
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.Cancel
            };

            pnlButtons.Controls.AddRange(new Control[] { btnChon, btnHuy });

            // Add controls to form
            Controls.AddRange(new Control[] { dgvVatTu, pnlSearch, pnlButtons });

            // Event handlers
            txtTimKiem.KeyPress += TxtTimKiem_KeyPress;
            btnTimKiem.Click += BtnTimKiem_Click;
            btnChon.Click += BtnChon_Click;
            dgvVatTu.SelectionChanged += DgvVatTu_SelectionChanged;
            dgvVatTu.CellDoubleClick += DgvVatTu_CellDoubleClick;
        }

        private void SetupDataGridView()
        {
            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Code",
                HeaderText = "Mã VT",
                DataPropertyName = "Code",
                FillWeight = 15
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVT",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                FillWeight = 50
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DVT",
                HeaderText = "ĐVT",
                DataPropertyName = "DonViTinh",
                FillWeight = 15
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongTon",
                HeaderText = "Số lượng tồn",
                DataPropertyName = "SoLuongTon",
                FillWeight = 20,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
        }

        private void LoadInitialData()
        {
            // Load tất cả vật tư có tồn kho > 0
            TimKiemVatTu("");
        }

        private void TimKiemVatTu(string keyword)
        {
            try
            {
                searchResults = xuatKhoBLL.TimKiemVatTu(keyword, warehouseId)
                                         .Where(x => x.SoLuongTon > 0)
                                         .ToList();

                dgvVatTu.DataSource = searchResults;
                lblKetQua.Text = $"Kết quả: {searchResults.Count}";
                btnChon.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtTimKiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnTimKiem_Click(sender, e);
                e.Handled = true;
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            TimKiemVatTu(txtTimKiem.Text.Trim());
        }

        private void DgvVatTu_SelectionChanged(object sender, EventArgs e)
        {
            btnChon.Enabled = dgvVatTu.CurrentRow != null;
        }

        private void DgvVatTu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnChon_Click(sender, e);
            }
        }

        private void BtnChon_Click(object sender, EventArgs e)
        {
            if (dgvVatTu.CurrentRow?.DataBoundItem is VatTuSearchResult selectedItem)
            {
                SelectedVatTu = selectedItem;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một vật tư!", "Thông báo", 
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}