using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class StaffsUserControl : UserControl
    {
        private readonly StaffBLL _staffBLL;
        private List<Staff> _allStaffs;

        public StaffsUserControl()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            _staffBLL = new StaffBLL();
            _allStaffs = new List<Staff>();
            LoadStaffs();
        }

        private void LoadStaffs()
        {
            try
            {
                _allStaffs = _staffBLL.GetAllStaffs();
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
            var displayList = _allStaffs.Select((staff, index) => new
            {
                STT = index + 1,
                ErpIdNV = staff.ErpIdNV,
                MaNV = staff.MaNV,
                TenNV = staff.TenNV,
                TenPB = staff.TenPB
            }).ToList();

            dgvStaffs.DataSource = displayList;

            // Cấu hình hiển thị columns
            if (dgvStaffs.Columns.Count > 0)
            {
                dgvStaffs.Columns["STT"].HeaderText = "STT";
                dgvStaffs.Columns["STT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvStaffs.Columns["STT"].Width = 60;
                dgvStaffs.Columns["ErpIdNV"].HeaderText = "ERP ID";
                dgvStaffs.Columns["ErpIdNV"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvStaffs.Columns["ErpIdNV"].Width = 100;
                dgvStaffs.Columns["MaNV"].HeaderText = "Mã nhân viên";
                dgvStaffs.Columns["MaNV"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;   
                dgvStaffs.Columns["MaNV"].Width = 120;
                dgvStaffs.Columns["TenNV"].HeaderText = "Tên nhân viên";
                dgvStaffs.Columns["TenNV"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvStaffs.Columns["TenPB"].HeaderText = "Phòng ban";
                dgvStaffs.Columns["TenPB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvStaffs.Columns["TenPB"].Width = 120;

            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                List<Staff> filteredStaffs;

                if (string.IsNullOrEmpty(keyword))
                {
                    filteredStaffs = _allStaffs;
                }
                else
                {
                    filteredStaffs = _staffBLL.SearchStaffs(keyword);
                }

                // Tạo danh sách có STT cho kết quả tìm kiếm
                var displayList = filteredStaffs.Select((staff, index) => new
                {
                    STT = index + 1,
                    ErpIdNV = staff.ErpIdNV,
                    MaNV = staff.MaNV,
                    TenNV = staff.TenNV,
                    TenPB = staff.TenPB
                }).ToList();

                dgvStaffs.DataSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
