using QLVT.ERP.DAL;
using QLVT.ERP.Models;

namespace QLVT.ERP.BLL
{
    /// <summary>
    /// BLL để xử lý nghiệp vụ BGK từ ERP - sử dụng chung cách kết nối như ImportBLL
    /// </summary>
    public class ERPBgkBLL
    {
        private readonly ErpHoanUngDAL _erpBgkDAL;

        public ERPBgkBLL()
        {
            _erpBgkDAL = new ErpHoanUngDAL();
        }

        /// <summary>
        /// Kiểm tra kết nối đến ERP
        /// </summary>
        public bool TestERPConnection()
        {
            try
            {
                return _erpBgkDAL.TestConnection();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy dữ liệu BGK từ ERP để hiển thị
        /// </summary>
        public List<NghiemThuGiaoKhoanModel> GetNghiemThuGiaoKhoanData(int soNghiemThu, int namNghiemThu)
        {
            try
            {
                return _erpBgkDAL.GetNghiemThuGiaoKhoanDataAsync(soNghiemThu, namNghiemThu);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy dữ liệu BGK: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy chi tiết vật tư BGK từ ERP để hiển thị
        /// </summary>
        public List<NghiemThuGiaoKhoanCTModel> GetNghiemThuGiaoKhoanCTData(long giaoKhoanNghiemThuVatTuID)
        {
            try
            {
                return _erpBgkDAL.GetNghiemThuGiaoKhoanCTDataAsync(giaoKhoanNghiemThuVatTuID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy chi tiết BGK: {ex.Message}");
            }
        }
    }
}