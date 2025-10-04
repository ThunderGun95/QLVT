using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    /// <summary>
    /// Business Logic Layer cho Nhập Kho Manual (sử dụng Transactions)
    /// </summary>
    public class NhapKhoManualBLL
    {
        private readonly NhapKhoTransactionDAL nhapKhoTransactionDAL;
        private readonly WarehouseDAL warehouseDAL;

        public NhapKhoManualBLL()
        {
            nhapKhoTransactionDAL = new NhapKhoTransactionDAL();
            warehouseDAL = new WarehouseDAL();
        }

        /// <summary>
        /// Lưu transaction nhập kho
        /// </summary>
        public bool SaveTransaction(PhieuNhapKho transaction, List<PhieuNhapKhoChiTiet> details, int warehouseId)
        {
            try
            {
                // Validate thông tin cơ bản
                ValidatePhieuNhapKho(transaction);

                // Validate chi tiết phiếu
                ValidateChiTietPhieuNhapKho(details);

                var currentUser = AuthenticationBLL.GetCurrentUser();
                // Tạo transaction trong database với warehouse ID thực tế
                int transactionId = nhapKhoTransactionDAL.CreateNhapKhoManualTransaction(transaction, details, currentUser!.Username, warehouseId);
                return transactionId > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lưu transaction: {ex.Message}", ex);
            }
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
        /// Validate thông tin phiếu nhập kho
        /// </summary>
        private void ValidatePhieuNhapKho(PhieuNhapKho phieu)
        {
            if (!phieu.MaKhoNhan.HasValue)
                throw new ArgumentException("Kho nhận không được để trống");

            if (phieu.NgayGiaoDich > DateTime.Now)
                throw new ArgumentException("Ngày giao dịch không được lớn hơn ngày hiện tại");   
        }

        /// <summary>
        /// Validate chi tiết phiếu nhập kho
        /// </summary>
        private void ValidateChiTietPhieuNhapKho(List<PhieuNhapKhoChiTiet> chiTietList)
        {
            if (chiTietList == null || !chiTietList.Any())
                throw new ArgumentException("Phiếu nhập kho phải có ít nhất một vật tư");

            foreach (var chiTiet in chiTietList)
            {
                if (chiTiet.ErpId <= 0)
                    throw new ArgumentException("Vật tư không hợp lệ");

                if (chiTiet.SoLuong <= 0)
                    throw new ArgumentException("Số lượng nhập phải lớn hơn 0");
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