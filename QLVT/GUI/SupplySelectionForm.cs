using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.DAL;
using QLVT.Models;

namespace QLVT.GUI
{
    /// <summary>
    /// Form để chọn vật tư khi thêm vào phiếu
    /// </summary>
    public partial class SupplySelectionForm : Form
    {
        private readonly SupplyDAL _supplyDAL;
        private List<Supply> _allSupplies;
        private List<Supply> _filteredSupplies;

        // Controls
        private TextBox txtSearch;
        private DataGridView dgvSupplies;
        private Button btnOK;
        private Button btnCancel;
        private Label lblStatus;

        public Supply SelectedSupply { get; private set; }
        public decimal Quantity { get; private set; }
        public string Note { get; private set; }

        public SupplySelectionForm()
        {
            _supplyDAL = new SupplyDAL();
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            _ = LoadSuppliesAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Chọn vật tư";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(600, 400);

            InitializeControls();
            SetupLayout();
            SetupEventHandlers();

            this.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            // Search
            var lblSearch = new Label
            {
                Text = "Tìm kiếm:",
                Location = new Point(20, 20),
                Size = new Size(80, 25)
            };

            txtSearch = new TextBox
            {
                Location = new Point(100, 17),
                Size = new Size(200, 25),
                PlaceholderText = "Nhập mã hoặc tên vật tư..."
            };

            // DataGridView
            dgvSupplies = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(740, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                MultiSelect = false
            };

            // Quantity and Note inputs
            var lblQuantity = new Label
            {
                Text = "Số lượng:",
                Location = new Point(20, 430),
                Size = new Size(80, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            var txtQuantity = new TextBox
            {
                Name = "txtQuantity",
                Location = new Point(100, 427),
                Size = new Size(100, 25),
                Text = "1",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            var lblNote = new Label
            {
                Text = "Ghi chú:",
                Location = new Point(220, 430),
                Size = new Size(60, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            var txtNote = new TextBox
            {
                Name = "txtNote",
                Location = new Point(280, 427),
                Size = new Size(200, 25),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            // Buttons
            btnOK = new Button
            {
                Text = "Chọn",
                Location = new Point(600, 425),
                Size = new Size(80, 30),
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(690, 425),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Status
            lblStatus = new Label
            {
                Text = "Đang tải danh sách vật tư...",
                Location = new Point(20, 470),
                Size = new Size(400, 25),
                ForeColor = Color.Blue,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            // Add controls
            this.Controls.AddRange(new Control[]
            {
                lblSearch, txtSearch, dgvSupplies, lblQuantity, txtQuantity,
                lblNote, txtNote, btnOK, btnCancel, lblStatus
            });
        }

        private void SetupLayout()
        {
            SetupDataGridViewColumns();
        }

        private void SetupDataGridViewColumns()
        {
            dgvSupplies.Columns.Clear();

            var columns = new[]
            {
                new { Name = "Code", Header = "Mã vật tư", Width = 120 },
                new { Name = "TenVatTu", Header = "Tên vật tư", Width = 300 },
                new { Name = "DonViTinh", Header = "ĐVT", Width = 80 },
                new { Name = "LoaiVatTu", Header = "Loại vật tư", Width = 120 },
                new { Name = "GhiChu", Header = "Ghi chú", Width = 200 }
            };

            foreach (var col in columns)
            {
                dgvSupplies.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    DataPropertyName = col.Name,
                    HeaderText = col.Header,
                    Width = col.Width
                });
            }
        }

        private void SetupEventHandlers()
        {
            txtSearch.TextChanged += TxtSearch_TextChanged;
            dgvSupplies.SelectionChanged += DgvSupplies_SelectionChanged;
            dgvSupplies.CellDoubleClick += DgvSupplies_CellDoubleClick;
            btnOK.Click += BtnOK_Click;
            this.Load += SupplySelectionForm_Load;
        }

        private void SupplySelectionForm_Load(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private async Task LoadSuppliesAsync()
        {
            try
            {
                lblStatus.Text = "Đang tải danh sách vật tư...";
                lblStatus.ForeColor = Color.Orange;

                _allSupplies = await _supplyDAL.GetAllSuppliesAsync();
                _filteredSupplies = new List<Supply>(_allSupplies);

                dgvSupplies.DataSource = _filteredSupplies;

                lblStatus.Text = $"Đã tải {_allSupplies.Count} vật tư. Chọn vật tư và nhập số lượng.";
                lblStatus.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Lỗi khi tải danh sách vật tư: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Lỗi khi tải danh sách vật tư: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterSupplies();
        }

        private void FilterSupplies()
        {
            if (_allSupplies == null) return;

            string searchText = txtSearch.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(searchText))
            {
                _filteredSupplies = new List<Supply>(_allSupplies);
            }
            else
            {
                _filteredSupplies = _allSupplies.Where(s => 
                    s.Code.ToLower().Contains(searchText) || 
                    s.TenVatTu.ToLower().Contains(searchText)).ToList();
            }

            dgvSupplies.DataSource = null;
            dgvSupplies.DataSource = _filteredSupplies;

            lblStatus.Text = $"Hiển thị {_filteredSupplies.Count}/{_allSupplies.Count} vật tư.";
        }

        private void DgvSupplies_SelectionChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = dgvSupplies.SelectedRows.Count > 0;
        }

        private void DgvSupplies_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectSupply();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SelectSupply();
        }

        private void SelectSupply()
        {
            if (dgvSupplies.SelectedRows.Count == 0) return;

            var selectedRow = dgvSupplies.SelectedRows[0];
            SelectedSupply = selectedRow.DataBoundItem as Supply;
            
            if (SelectedSupply == null) return;

            // Get quantity
            var txtQuantity = this.Controls.Find("txtQuantity", true).FirstOrDefault() as TextBox;
            if (txtQuantity != null && decimal.TryParse(txtQuantity.Text, out decimal qty) && qty > 0)
            {
                Quantity = qty;
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity?.Focus();
                return;
            }

            // Get note
            var txtNote = this.Controls.Find("txtNote", true).FirstOrDefault() as TextBox;
            Note = txtNote?.Text ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose resources
            }
            base.Dispose(disposing);
        }
    }
}
