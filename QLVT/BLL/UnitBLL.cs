using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class UnitBLL
    {
        private readonly UnitDAL _unitDAL;

        public UnitBLL()
        {
            _unitDAL = new UnitDAL();
        }

        /// <summary>
        /// Lấy tất cả đơn vị tính
        /// </summary>
        /// <returns>Danh sách đơn vị tính</returns>
        public List<Unit> GetAllUnits()
        {
            try
            {
                return _unitDAL.GetAllUnits();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách đơn vị tính: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm đơn vị tính
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách đơn vị tính</returns>
        public List<Unit> SearchUnits(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return GetAllUnits();
                }
                return _unitDAL.SearchUnits(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm đơn vị tính: {ex.Message}", ex);
            }
        }
    }
}