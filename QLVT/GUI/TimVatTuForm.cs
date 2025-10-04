using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class TimVatTuForm : Form
    {
        private readonly SupplyBLL supplyBLL;
        private List<Supply> allSupplies;
        
        public Supply? SelectedSupply { get; private set; }

        // Controls
        private TextBox txtTimKiem = null!;
        private Button btnTimKiem = null!;
        private DataGridView dgvVatTu = null!;
        private Button btnChon = null!;
        private Button btnHuy = null!;
        private Label lblKetQua = null!;

        public TimVatTuForm()
        {
            supplyBLL = new SupplyBLL();
            allSupplies = new List<Supply>();
            
            InitializeComponent();
            LoadInitialData();
        }

        private void InitializeComponent()
        {
            Text = "Tìm kiếm vật tư";
            Size = new Size(700, 500);
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
                Size = new Size(350, 23),
                Font = new Font("Segoe UI", 10F)
            };

            btnTimKiem = new Button
            {
                Text = "Tìm kiếm",
                Location = new Point(470, 10),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };

            lblKetQua = new Label
            {
                Text = "Kết quả: 0",
                Location = new Point(590, 15),
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
                Location = new Point(480, 10),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            btnHuy = new Button
            {
                Text = "Hủy",
                Size = new Size(100, 30),
                Location = new Point(590, 10),
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.Cancel
            };

            pnlButtons.Controls.AddRange(new Control[] { btnChon, btnHuy });

            // Add controls to form
            Controls.AddRange(new Control[] { dgvVatTu, pnlSearch, pnlButtons });

            // Event handlers
            txtTimKiem.TextChanged += TxtTimKiem_TextChanged;
            txtTimKiem.KeyPress += TxtTimKiem_KeyPress;
            btnTimKiem.Click += BtnTimKiem_Click;
            btnChon.Click += BtnChon_Click;
            btnHuy.Click += BtnHuy_Click;
            dgvVatTu.SelectionChanged += DgvVatTu_SelectionChanged;
            dgvVatTu.CellDoubleClick += DgvVatTu_CellDoubleClick;
        }

        private void SetupDataGridView()
        {
            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Code",
                HeaderText = "Mã vật tư",
                DataPropertyName = "Code",
                FillWeight = 30
            });

            dgvVatTu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenVatTu",
                HeaderText = "Tên vật tư",
                DataPropertyName = "TenVatTu",
                FillWeight = 70
            });
        }

        private void LoadInitialData()
        {
            // Load tất cả vật tư
            TimKiemVatTu("");
            txtTimKiem.Focus();
        }

        private void TimKiemVatTu(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    allSupplies = supplyBLL.GetAllSupplies();
                }
                else
                {
                    allSupplies = supplyBLL.SearchSupplies(keyword);
                }

                dgvVatTu.DataSource = allSupplies;
                lblKetQua.Text = $"Kết quả: {allSupplies.Count}";
                btnChon.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Tìm kiếm realtime khi gõ
            TimKiemVatTu(txtTimKiem.Text.Trim());
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
            if (dgvVatTu.CurrentRow?.DataBoundItem is Supply selectedItem)
            {
                SelectedSupply = selectedItem;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một vật tư!", "Thông báo", 
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnHuy_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}