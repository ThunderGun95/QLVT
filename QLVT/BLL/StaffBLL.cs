using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class StaffBLL
    {
        private readonly StaffDAL _staffDAL;

        public StaffBLL()
        {
            _staffDAL = new StaffDAL();
        }

        /// <summary>
        /// Lấy tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách nhân viên</returns>
        public List<Staff> GetAllStaffs()
        {
            try
            {
                return _staffDAL.GetAllStaffs();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm nhân viên
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách nhân viên</returns>
        public List<Staff> SearchStaffs(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return GetAllStaffs();
                }
                return _staffDAL.SearchStaffs(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm nhân viên: {ex.Message}", ex);
            }
        }
    }
}
