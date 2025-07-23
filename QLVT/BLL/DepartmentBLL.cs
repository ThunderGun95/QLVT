using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class DepartmentBLL
    {
        private readonly DepartmentDAL _departmentDAL;

        public DepartmentBLL()
        {
            _departmentDAL = new DepartmentDAL();
        }

        /// <summary>
        /// Lấy tất cả phòng ban
        /// </summary>
        /// <returns>Danh sách phòng ban</returns>
        public List<Department> GetAllDepartments()
        {
            try
            {
                return _departmentDAL.GetAllDepartments();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách phòng ban: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm phòng ban
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách phòng ban</returns>
        public List<Department> SearchDepartments(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return GetAllDepartments();
                }
                return _departmentDAL.SearchDepartments(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm phòng ban: {ex.Message}", ex);
            }
        }
    }
}
