using QLVT.DAL;
using QLVT.ERP.BLL;
using QLVT.ERP.DAL;
using QLVT.ERP.Models;

namespace QLVT.BLL
{
    public class HoanUngBLL
    {
        private readonly HoanUngTransactionDAL hoanUngTransactionDAL;
        private readonly ERPConnectionDAL erpConnectionDAL;

        public HoanUngBLL()
        {
            hoanUngTransactionDAL = new HoanUngTransactionDAL();
            erpConnectionDAL = new ERPConnectionDAL();
        }

        #region Mạng cấp 4
        public List<DonDangKyModel> MC4_GetDanhSachChoHoanUng()
        {
            try
            {
                return hoanUngTransactionDAL.MC4_GetDanhSachDonDangKy();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách hồ sơ mạng cấp 4: {ex.Message}", ex);
            }
        }
        public List<DonDangKyCTModel> MC4_GetChiTietVatTuWithTonKho(string maddk)
        {
            try
            {
                if (string.IsNullOrEmpty(maddk))
                    throw new ArgumentException("Mã DDK không được rỗng");

                return hoanUngTransactionDAL.MC4_GetChiTietVatTuWithTonKho(maddk);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy chi tiết vật tư với tồn kho: {ex.Message}", ex);
            }
        }
        public bool MC4_XacNhanHoanUng(string maddk)
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();

            if (string.IsNullOrEmpty(maddk))
                throw new ArgumentException("Mã DDK không được rỗng");

            // Thực hiện hoàn ứng - DAL sẽ throw exception nếu có lỗi
            return hoanUngTransactionDAL.MC4_UpdateHoanUngDonDangKy(maddk, currentUser!.Username);
        }
        public async Task<(int soLuongDon, int soLuongChiTiet, string thongBao)> MC4_TaiDuLieuERP()
        {
            try
            {
                // Bước 1: Lấy max của NgayHoanUng trong bảng ct.DonDangKy
                DateTime? maxNgayHoanUng = hoanUngTransactionDAL.MC4_GetMaxNgayHoanUngMC4();
                DateTime tuNgay = maxNgayHoanUng ?? DateTime.Now.AddDays(-30); // Nếu chưa có dữ liệu thì lấy 30 ngày gần nhất

                // Bước 2: Lấy danh sách đơn từ ERP qua hàm GetDonDangKyDataAsync
                var danhSachDonERP = await erpConnectionDAL.GetDonDangKyDataAsync(tuNgay);

                int donMoi = 0;
                int chiTietMoi = 0;

                // Bước 3: Lưu các đơn mới vào database
                foreach (var don in danhSachDonERP)
                {
                    // Kiểm tra đơn đã tồn tại chưa
                    if (!hoanUngTransactionDAL.MC4_IsDonDangKyExists(don.MADDK))
                    {
                        // Lấy chi tiết vật tư cho đơn này
                        var chiTietList = await erpConnectionDAL.GetDonDangKyCTDataAsync(don.MADDK);

                        // Thêm đơn và chi tiết trong transaction
                        bool result = hoanUngTransactionDAL.MC4_InsertDonDangKyWithChiTiet(don, chiTietList);
                        if (result)
                        {
                            donMoi++;
                            chiTietMoi += chiTietList.Count;
                        }
                    }
                }

                string thongBao = $"Tải dữ liệu ERP thành công!\n\n" +
                                 $"📊 Kết quả:\n" +
                                 $"- Đơn đăng ký mới: {donMoi}\n" +
                                 $"- Chi tiết vật tư mới: {chiTietMoi}\n" +
                                 $"- Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                                 $"- Từ ngày: {tuNgay:dd/MM/yyyy}";

                return (donMoi, chiTietMoi, thongBao);
            }
            catch (Exception ex)
            {
                string thongBaoLoi = $"❌ Lỗi tải dữ liệu ERP:\n\n{ex.Message}";
                if (ex.InnerException != null)
                    thongBaoLoi += $"\n\nChi tiết: {ex.InnerException.Message}";

                throw new Exception(thongBaoLoi, ex);
            }
        }
        #endregion

        #region Điểm chảy
        public List<SuaChuaModel> DC_GetDanhSachChoHoanUng()
        {
            try
            {
                return hoanUngTransactionDAL.DC_GetDanhSachDonSuaChua();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách hồ sơ sửa chữa: {ex.Message}", ex);
            }
        }
        public List<SuaChuaCTModel> DC_GetChiTietVatTuWithTonKho(string maDon)
        {
            try
            {
                if (string.IsNullOrEmpty(maDon))
                    throw new ArgumentException("Mã đơn không được rỗng");

                return hoanUngTransactionDAL.DC_GetChiTietVatTuWithTonKho(maDon);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy chi tiết vật tư với tồn kho: {ex.Message}", ex);
            }
        }
        public bool DC_XacNhanHoanUng(string maDon)
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();

            if (string.IsNullOrEmpty(maDon))
                throw new ArgumentException("Mã đơn không được rỗng");

            // Thực hiện hoàn ứng với transaction-based logic tương tự HoanUngBLL
            return hoanUngTransactionDAL.DC_UpdateHoanUngSuaChua(maDon, currentUser!.Username);
        }
        public async Task<(int soLuongDon, int soLuongChiTiet, string thongBao)> DC_TaiDuLieuERP()
        {
            try
            {
                // Bước 1: Lấy max của NgayHoanUng trong bảng ct.SuaChua
                DateTime? maxNgayHoanUng = hoanUngTransactionDAL.DC_GetMaxNgayHoanUng();
                DateTime tuNgay = maxNgayHoanUng ?? DateTime.Now.AddDays(-30); // Nếu chưa có dữ liệu thì lấy 30 ngày gần nhất

                // Bước 2: Lấy danh sách đơn từ ERP qua hàm GetSuaChuaDataAsync
                var danhSachDonERP = await erpConnectionDAL.GetSuaChuaDataAsync(tuNgay);

                int donMoi = 0;
                int chiTietMoi = 0;

                // Bước 3: Lưu các đơn mới vào database
                foreach (var don in danhSachDonERP)
                {
                    // Kiểm tra đơn đã tồn tại chưa
                    if (!hoanUngTransactionDAL.DC_IsDonSuaChuaExists(don.MADON))
                    {
                        // Lấy chi tiết vật tư cho đơn này
                        var chiTietList = await erpConnectionDAL.GetSuaChuaCTDataAsync(don.MADON);

                        if (chiTietList.Count > 0)
                        {
                            // Thêm đơn và chi tiết trong transaction
                            bool result = hoanUngTransactionDAL.DC_InsertDonSuaChuaWithChiTiet(don, chiTietList);
                            if (result)
                            {
                                donMoi++;
                                chiTietMoi += chiTietList.Count;
                            }
                        }
                    }
                }

                string thongBao = $"Tải dữ liệu ERP thành công!\n\n" +
                                 $"📊 Kết quả:\n" +
                                 $"- Đơn sửa chữa mới: {donMoi}\n" +
                                 $"- Chi tiết vật tư mới: {chiTietMoi}\n" +
                                 $"- Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                                 $"- Từ ngày: {tuNgay:dd/MM/yyyy}";

                return (donMoi, chiTietMoi, thongBao);
            }
            catch (Exception ex)
            {
                string thongBaoLoi = $"❌ Lỗi tải dữ liệu ERP:\n\n{ex.Message}";
                if (ex.InnerException != null)
                    thongBaoLoi += $"\n\nChi tiết: {ex.InnerException.Message}";

                throw new Exception(thongBaoLoi, ex);
            }
        }

        #endregion

        public bool TestERPConnection()
        {
            return Utils.ExternalDatabaseHelper.TestExternalConnection();
        }
    }
}
