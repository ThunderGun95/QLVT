using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class UnitsUserControl : UserControl
    {
        private readonly UnitBLL _unitBLL;
        private List<Unit> _allUnits;

        public UnitsUserControl()
        {
            InitializeComponent();
            _unitBLL = new UnitBLL();
            _allUnits = new List<Unit>();
            LoadUnits();
        }

        private void LoadUnits()
        {
            try
            {
                _allUnits = _unitBLL.GetAllUnits();
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
            var displayList = _allUnits.Select((unit, index) => new
            {
                STT = index + 1,
                MaDVT = unit.MaDVT,
                TenDVT = unit.TenDVT
            }).ToList();

            dgvUnits.DataSource = displayList;

            // Cấu hình hiển thị columns
            if (dgvUnits.Columns.Count > 0)
            {
                dgvUnits.Columns["STT"].HeaderText = "STT";
                dgvUnits.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUnits.Columns["STT"].Width = 60;
                dgvUnits.Columns["MaDVT"].HeaderText = "Mã ĐVT";
                dgvUnits.Columns["MaDVT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUnits.Columns["MaDVT"].Width = 100;
                dgvUnits.Columns["TenDVT"].HeaderText = "Tên đơn vị tính";
                dgvUnits.Columns["TenDVT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                // dgvUnits.Columns["TenDVT"].Width = 200;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                List<Unit> filteredUnits;

                if (string.IsNullOrEmpty(keyword))
                {
                    filteredUnits = _allUnits;
                }
                else
                {
                    filteredUnits = _unitBLL.SearchUnits(keyword);
                }

                // Tạo danh sách có STT cho kết quả tìm kiếm
                var displayList = filteredUnits.Select((unit, index) => new
                {
                    STT = index + 1,
                    MaDVT = unit.MaDVT,
                    TenDVT = unit.TenDVT
                }).ToList();

                dgvUnits.DataSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}