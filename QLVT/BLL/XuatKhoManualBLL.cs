using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    /// <summary>
    /// Business Logic Layer cho Xuất Kho Manual (sử dụng Transactions)
    /// </summary>
    public class XuatKhoManualBLL
    {
        private readonly XuatKhoTransactionDAL xuatKhoTransactionDAL;
        private readonly WarehouseDAL warehouseDAL;
        private readonly SupplyDAL supplyDAL;

        public XuatKhoManualBLL()
        {
            xuatKhoTransactionDAL = new XuatKhoTransactionDAL();
            warehouseDAL = new WarehouseDAL();
            supplyDAL = new SupplyDAL();
        }


        /// <summary>
        /// Lấy danh sách kho
        /// </summary>
        public List<Warehouse> GetDanhSachKho()
        {
            try
            {
                return warehouseDAL.GetAllWarehouses();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách kho: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm vật tư
        /// </summary>
        public List<VatTuSearchResult> TimKiemVatTu(string keyword, int? warehouseId = null)
        {
            try
            {
                // Nếu keyword rỗng, trả về tất cả vật tư có tồn kho > 0
                var searchKeyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword.Trim();
                return supplyDAL.SearchVatTuByKho(searchKeyword, warehouseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tìm kiếm vật tư: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra số lượng tồn kho
        /// </summary>
        public decimal KiemTraSoLuongTon(int supplyId, int? warehouseId)
        {
            try
            {
                return supplyDAL.GetSoLuongTon(supplyId, warehouseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi kiểm tra tồn kho: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo phiếu xuất kho mới
        /// </summary>
        public int TaoPhieuXuatKho(PhieuXuatKho phieu)
        {
            try
            {
                // Validate thông tin cơ bản
                ValidatePhieuXuatKho(phieu);

                // Validate chi tiết phiếu
                ValidateChiTietPhieuXuatKho(phieu.ChiTiet, phieu.MaKhoNguon);

                // Tạo phiếu
                var currentUser = AuthenticationBLL.GetCurrentUser();
                return xuatKhoTransactionDAL.CreateXuatKhoManualTransaction(phieu, currentUser!.Username);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tạo phiếu xuất kho: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate thông tin phiếu xuất kho
        /// </summary>
        private void ValidatePhieuXuatKho(PhieuXuatKho phieu)
        {
            if (!phieu.MaKhoNhan.HasValue)
                throw new ArgumentException("Kho nhận không được để trống");

            if (phieu.NgayGiaoDich > DateTime.Now)
                throw new ArgumentException("Ngày giao dịch không được lớn hơn ngày hiện tại");

            if (phieu.ChiTiet == null || !phieu.ChiTiet.Any())
                throw new ArgumentException("Phiếu xuất kho phải có ít nhất một vật tư");
        }

        /// <summary>
        /// Validate chi tiết phiếu xuất kho
        /// </summary>
        private void ValidateChiTietPhieuXuatKho(List<PhieuXuatKhoChiTiet> chiTietList, int? khoXuatId)
        {
            foreach (var chiTiet in chiTietList)
            {
                if (chiTiet.ErpId <= 0)
                    throw new ArgumentException("Vật tư không hợp lệ");

                if (chiTiet.SoLuong <= 0)
                    throw new ArgumentException("Số lượng xuất phải lớn hơn 0");

                // Kiểm tra tồn kho
                var soLuongTon = KiemTraSoLuongTon(chiTiet.ErpId, khoXuatId);
                if (chiTiet.SoLuong > soLuongTon)
                    throw new ArgumentException($"Vật tư {chiTiet.MaVatTu}: Số lượng xuất ({chiTiet.SoLuong}) vượt quá tồn kho ({soLuongTon})");
            }

            // Kiểm tra trùng lặp vật tư
            var duplicates = chiTietList.GroupBy(x => x.ErpId)
                                      .Where(g => g.Count() > 1)
                                      .Select(g => g.Key);
            
            if (duplicates.Any())
                throw new ArgumentException("Không được thêm cùng một vật tư nhiều lần");
        }
    }
}