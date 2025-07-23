using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class SupplyBLL
    {
        private readonly SupplyDAL _supplyDAL;

        public SupplyBLL()
        {
            _supplyDAL = new SupplyDAL();
        }

        /// <summary>
        /// Lấy tất cả vật tư
        /// </summary>
        /// <returns>Danh sách vật tư</returns>
        public List<Supply> GetAllSupplies()
        {
            try
            {
                return _supplyDAL.GetAllSupplies();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách vật tư: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm vật tư
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách vật tư</returns>
        public List<Supply> SearchSupplies(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return GetAllSupplies();
                }
                return _supplyDAL.SearchSupplies(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm vật tư: {ex.Message}", ex);
            }
        }
    }
}
