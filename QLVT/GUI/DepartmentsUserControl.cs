using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class DepartmentsUserControl : UserControl
    {
        private readonly DepartmentBLL _departmentBLL;
        private List<Department> _allDepartments;

        public DepartmentsUserControl()
        {
            InitializeComponent();
            _departmentBLL = new DepartmentBLL();
            _allDepartments = new List<Department>();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            try
            {
                _allDepartments = _departmentBLL.GetAllDepartments();
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
            var displayList = _allDepartments.Select((department, index) => new
            {
                STT = index + 1,
                MaPB = department.MaPB,
                TenPB = department.TenPB
            }).ToList();

            dgvDepartments.DataSource = displayList;

            // Cấu hình hiển thị columns
            if (dgvDepartments.Columns.Count > 0)
            {
                dgvDepartments.Columns["STT"].HeaderText = "STT";
                dgvDepartments.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvDepartments.Columns["STT"].Width = 60;
                dgvDepartments.Columns["MaPB"].HeaderText = "Mã phòng ban";
                dgvDepartments.Columns["MaPB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvDepartments.Columns["MaPB"].Width = 120;
                dgvDepartments.Columns["TenPB"].HeaderText = "Tên phòng ban";
                dgvDepartments.Columns["TenPB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                // dgvDepartments.Columns["TenPB"].Width = 250;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                List<Department> filteredDepartments;

                if (string.IsNullOrEmpty(keyword))
                {
                    filteredDepartments = _allDepartments;
                }
                else
                {
                    filteredDepartments = _departmentBLL.SearchDepartments(keyword);
                }

                // Tạo danh sách có STT cho kết quả tìm kiếm
                var displayList = filteredDepartments.Select((department, index) => new
                {
                    STT = index + 1,
                    MaPB = department.MaPB,
                    TenPB = department.TenPB
                }).ToList();

                dgvDepartments.DataSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
