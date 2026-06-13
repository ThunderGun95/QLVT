using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class SuppliesUserControl : UserControl
    {
        private readonly SupplyBLL _supplyBLL;
        private List<Supply> _allSupplies;

        public SuppliesUserControl()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            _supplyBLL = new SupplyBLL();
            _allSupplies = new List<Supply>();
            LoadSupplies();
        }

        private void LoadSupplies()
        {
            try
            {
                _allSupplies = _supplyBLL.GetAllSupplies();
                BindDataToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindDataToGrid()
        {
            // Tạo danh sách có STT
            var displayList = _allSupplies.Select((supply, index) => new
            {
                STT = index + 1,
                ErpId = supply.ErpId,
                Code = supply.Code,
                TenVatTu = supply.TenVatTu,
                DacTinhKyThuat = supply.DacTinhKyThuat,
                TenDVT = supply.TenDVT
            }).ToList();

            dgvSupplies.DataSource = displayList;

            // Cấu hình hiển thị columns
            if (dgvSupplies.Columns.Count > 0)
            {
                dgvSupplies.Columns["STT"].HeaderText = "STT";
                dgvSupplies.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvSupplies.Columns["STT"].Width = 50;
                dgvSupplies.Columns["ErpId"].HeaderText = "ERP ID";
                dgvSupplies.Columns["ErpId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvSupplies.Columns["ErpId"].Width = 80;
                dgvSupplies.Columns["Code"].HeaderText = "Mã vật tư";
                dgvSupplies.Columns["Code"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvSupplies.Columns["Code"].Width = 100;
                dgvSupplies.Columns["TenVatTu"].HeaderText = "Tên vật tư";
                dgvSupplies.Columns["TenVatTu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvSupplies.Columns["TenVatTu"].Width = 200;
                dgvSupplies.Columns["DacTinhKyThuat"].HeaderText = "Đặc tính kỹ thuật";
                dgvSupplies.Columns["DacTinhKyThuat"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                // dgvSupplies.Columns["DacTinhKyThuat"].Width = 150;
                dgvSupplies.Columns["TenDVT"].HeaderText = "Tên ĐVT";
                dgvSupplies.Columns["TenDVT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvSupplies.Columns["TenDVT"].Width = 100;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                List<Supply> filteredSupplies;

                if (string.IsNullOrEmpty(keyword))
                {
                    filteredSupplies = _allSupplies;
                }
                else
                {
                    filteredSupplies = _supplyBLL.SearchSupplies(keyword);
                }

                // Tạo danh sách có STT cho kết quả tìm kiếm
                var displayList = filteredSupplies.Select((supply, index) => new
                {
                    STT = index + 1,
                    ErpId = supply.ErpId,
                    Code = supply.Code,
                    TenVatTu = supply.TenVatTu,
                    DacTinhKyThuat = supply.DacTinhKyThuat,
                    TenDVT = supply.TenDVT
                }).ToList();

                dgvSupplies.DataSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
