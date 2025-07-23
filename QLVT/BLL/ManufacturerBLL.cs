using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class ManufacturerBLL
    {
        private readonly ManufacturerDAL _manufacturerDAL;

        public ManufacturerBLL()
        {
            _manufacturerDAL = new ManufacturerDAL();
        }

        /// <summary>
        /// Lấy tất cả nhà sản xuất
        /// </summary>
        /// <returns>Danh sách nhà sản xuất</returns>
        public List<Manufacturer> GetAllManufacturers()
        {
            try
            {
                return _manufacturerDAL.GetAllManufacturers();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhà sản xuất: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm nhà sản xuất
        /// </summary>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách nhà sản xuất</returns>
        public List<Manufacturer> SearchManufacturers(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return GetAllManufacturers();
                }
                return _manufacturerDAL.SearchManufacturers(keyword.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm nhà sản xuất: {ex.Message}", ex);
            }
        }
    }
}