using QLVT.DAL;
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
        public List<DonDangKyModel> GetDanhSachHoSoChoHoanUng()
        {
            try
            {
                return hoanUngTransactionDAL.GetDanhSachDonDangKy();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy danh sách hồ sơ: {ex.Message}", ex);
            }
        }

        public DonDangKyModel? GetHoSoByMa(string maDDK)
        {
            try
            {
                if (string.IsNullOrEmpty(maDDK))
                    throw new ArgumentException("Mã DDK không được rỗng");

                return hoanUngTransactionDAL.GetDonDangKyByMa(maDDK);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy thông tin hồ sơ: {ex.Message}", ex);
            }
        }

        public List<DonDangKyCTModel> GetChiTietVatTuWithTonKho(string maddk)
        {
            try
            {
                if (string.IsNullOrEmpty(maddk))
                    throw new ArgumentException("Mã DDK không được rỗng");

                return hoanUngTransactionDAL.GetChiTietVatTuWithTonKho(maddk);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi lấy chi tiết vật tư với tồn kho: {ex.Message}", ex);
            }
        }

        public bool XacNhanHoanUng(string maddk)
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();

            if (string.IsNullOrEmpty(maddk))
                throw new ArgumentException("Mã DDK không được rỗng");

            // Thực hiện hoàn ứng - DAL sẽ throw exception nếu có lỗi
            return hoanUngTransactionDAL.UpdateHoanUngDonDangKy(maddk, currentUser!.Username);
        }

        public List<DonDangKyModel> TimKiemHoSo(string? maDDK = null, string? tenKH = null,
            string? nhanVienXayLap = null, bool? trangThai = null)
        {
            try
            {
                return hoanUngTransactionDAL.TimKiemDonDangKy(maDDK, tenKH, nhanVienXayLap, trangThai);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tìm kiếm hồ sơ: {ex.Message}", ex);
            }
        }

        public async Task<(int soLuongDon, int soLuongChiTiet, string thongBao)> TaiDuLieuMC4ERP()
        {
            try
            {
                // Bước 1: Lấy max của NgayHoanUng trong bảng ct.DonDangKy
                DateTime? maxNgayHoanUng = hoanUngTransactionDAL.GetMaxNgayHoanUng();
                DateTime tuNgay = maxNgayHoanUng ?? DateTime.Now.AddDays(-30); // Nếu chưa có dữ liệu thì lấy 30 ngày gần nhất

                // Bước 2: Lấy danh sách đơn từ ERP qua hàm GetDonDangKyDataAsync
                var danhSachDonERP = await erpConnectionDAL.GetDonDangKyDataAsync(tuNgay);

                int donMoi = 0;
                int chiTietMoi = 0;

                // Bước 3: Lưu các đơn mới vào database
                foreach (var don in danhSachDonERP)
                {
                    // Kiểm tra đơn đã tồn tại chưa
                    if (!hoanUngTransactionDAL.IsDonDangKyExists(don.MADDK))
                    {
                        // Lấy chi tiết vật tư cho đơn này
                        var chiTietList = await erpConnectionDAL.GetDonDangKyCTDataAsync(don.MADDK);

                        // Thêm đơn và chi tiết trong transaction
                        bool result = hoanUngTransactionDAL.InsertDonDangKyWithChiTiet(don, chiTietList);
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

        public bool TestERPConnection()
        {
            return Utils.ExternalDatabaseHelper.TestExternalConnection();
        }
    }
}
