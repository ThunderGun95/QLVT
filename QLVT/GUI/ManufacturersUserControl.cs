using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class ManufacturersUserControl : UserControl
    {
        private readonly ManufacturerBLL _manufacturerBLL;
        private List<Manufacturer> _allManufacturers;

        public ManufacturersUserControl()
        {
            InitializeComponent();
            _manufacturerBLL = new ManufacturerBLL();
            _allManufacturers = new List<Manufacturer>();
            LoadManufacturers();
        }

        private void LoadManufacturers()
        {
            try
            {
                _allManufacturers = _manufacturerBLL.GetAllManufacturers();
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
            var displayList = _allManufacturers.Select((manufacturer, index) => new
            {
                STT = index + 1,
                MaNSX = manufacturer.MaNSX,
                TenNSX = manufacturer.TenNSX
            }).ToList();

            dgvManufacturers.DataSource = displayList;

            // Cấu hình hiển thị columns
            if (dgvManufacturers.Columns.Count > 0)
            {
                dgvManufacturers.Columns["STT"].HeaderText = "STT";
                dgvManufacturers.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvManufacturers.Columns["STT"].Width = 60;
                dgvManufacturers.Columns["MaNSX"].HeaderText = "Mã NSX";
                dgvManufacturers.Columns["MaNSX"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvManufacturers.Columns["MaNSX"].Width = 100;
                dgvManufacturers.Columns["TenNSX"].HeaderText = "Tên nhà sản xuất";
                dgvManufacturers.Columns["TenNSX"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                // dgvManufacturers.Columns["TenNSX"].Width = 250;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                List<Manufacturer> filteredManufacturers;

                if (string.IsNullOrEmpty(keyword))
                {
                    filteredManufacturers = _allManufacturers;
                }
                else
                {
                    filteredManufacturers = _manufacturerBLL.SearchManufacturers(keyword);
                }

                // Tạo danh sách có STT cho kết quả tìm kiếm
                var displayList = filteredManufacturers.Select((manufacturer, index) => new
                {
                    STT = index + 1,
                    MaNSX = manufacturer.MaNSX,
                    TenNSX = manufacturer.TenNSX
                }).ToList();

                dgvManufacturers.DataSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}