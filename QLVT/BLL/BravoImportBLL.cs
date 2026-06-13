using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    /// <summary>
    /// Business logic xử lý import phiếu từ Bravo
    /// </summary>
    public class BravoImportBLL
    {
        private readonly BravoImportDAL bravoImportDAL;
        private readonly WarehouseDAL warehouseDAL;
        private readonly SupplyDAL supplyDAL;

        public BravoImportBLL()
        {
            bravoImportDAL = new BravoImportDAL();
            warehouseDAL = new WarehouseDAL();
            supplyDAL = new SupplyDAL();
        }

        /// <summary>
        /// Import phiếu từ Bravo với logic xử lý theo loại phiếu
        /// 
        /// Logic:
        /// - XK (Xuất kho):
        ///   + Kho nhập là COMPANY → Xử lý như phiếu trả kho (trả lại kho công ty)
        ///   + Kho nhập là CANHAN → Xử lý như phiếu xuất kho (xuất ra kho cá nhân)
        /// - XN (Xuất nhập) + Kho nhập là kho công ty → Xử lý như phiếu trả kho (nhập lại kho công ty)
        /// - HU (Hoàn ứng) → Chỉ cần 1 trong 2 cột (kho nhập hoặc kho xuất), kho đó sẽ bị trừ vật tư
        /// </summary>
        /// <param name="importData">Dữ liệu import đã được validate</param>
        /// <param name="createdBy">Người tạo</param>
        /// <returns>(Thành công, Thất bại, Danh sách lỗi)</returns>
        public (int success, int failed, List<string> errors) ProcessImport(BravoImportData importData, string createdBy)
        {
            if (importData == null)
                throw new ArgumentNullException(nameof(importData));

            int successCount = 0;
            int failedCount = 0;
            var errors = new List<string>();

            // Xử lý từng nhóm phiếu
            foreach (var phieuGroup in importData.PhieuGroups)
            {
                try
                {
                    // Kiểm tra phiếu đã được xử lý chưa
                    if (bravoImportDAL.IsPhieuProcessed(phieuGroup.SoPhieu))
                    {
                        errors.Add($"Phiếu {phieuGroup.SoPhieu}: Đã được xử lý trước đó");
                        failedCount++;
                        continue;
                    }

                    // Xử lý theo loại phiếu
                    int transactionId = 0;

                    switch (phieuGroup.LoaiPhieu.ToUpper())
                    {
                        case "XK":
                            transactionId = ProcessXuatKho(phieuGroup, createdBy);
                            break;

                        case "HU":
                            transactionId = ProcessHoanUng(phieuGroup, createdBy);
                            break;

                        default:
                            errors.Add($"Phiếu {phieuGroup.SoPhieu}: Loại phiếu '{phieuGroup.LoaiPhieu}' không hợp lệ");
                            failedCount++;
                            continue;
                    }

                    if (transactionId > 0)
                    {
                        phieuGroup.TransactionID = transactionId;
                        phieuGroup.IsProcessed = true;
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"Phiếu {phieuGroup.SoPhieu}: Không tạo được transaction");
                        failedCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Phiếu {phieuGroup.SoPhieu}: {ex.Message}");
                    failedCount++;
                }
            }

            return (successCount, failedCount, errors);
        }

        /// <summary>
        /// Xử lý phiếu xuất kho (XK)
        /// Logic:
        /// - Nếu kho nhập là kho COMPANY → Xử lý như phiếu trả kho (nhập lại kho công ty)
        /// - Nếu kho nhập là kho CANHAN → Xử lý như phiếu xuất kho (xuất ra kho cá nhân)
        /// </summary>
        private int ProcessXuatKho(PhieuGroup phieuGroup, string createdBy)
        {
            // Validate kho nhập
            if (string.IsNullOrEmpty(phieuGroup.MaKhoNhap))
                throw new Exception("Phiếu xuất kho phải có kho nhập");

            var khoNhap = warehouseDAL.GetWarehouseByCode(phieuGroup.MaKhoNhap);
            if (khoNhap == null)
                throw new Exception($"Không tìm thấy kho nhập: {phieuGroup.MaKhoNhap}");

            // Validate kho xuất
            if (string.IsNullOrEmpty(phieuGroup.MaKhoXuat))
                throw new Exception("Phiếu xuất kho phải có kho xuất");

            var khoXuat = warehouseDAL.GetWarehouseByCode(phieuGroup.MaKhoXuat);
            if (khoXuat == null)
                throw new Exception($"Không tìm thấy kho xuất: {phieuGroup.MaKhoXuat}");

            // Kiểm tra loại kho nhập để xử lý tương ứng
            if (khoNhap.LoaiKho == "CongTy")
            {
                // Kho nhập là kho công ty → Xử lý như phiếu trả kho
                return bravoImportDAL.CreateTraKhoTransaction(
                    phieuGroup.SoPhieu,
                    phieuGroup.NgayGiaoDich,
                    phieuGroup.NoiDungPhieu,
                    khoXuat.Id,
                    khoNhap.Id,
                    phieuGroup.Items,
                    createdBy
                );
            }
            else if (khoNhap.LoaiKho == "CANHAN")
            {
                // Kho nhập là kho cá nhân → Xử lý như phiếu xuất kho
                return bravoImportDAL.CreateXuatKhoTransaction(
                    phieuGroup.SoPhieu,
                    phieuGroup.NgayGiaoDich,
                    phieuGroup.NoiDungPhieu,
                    khoXuat.Id,
                    khoNhap.Id,
                    phieuGroup.Items,
                    createdBy
                );
            }
            else
            {
                throw new Exception($"Loại kho nhập '{khoNhap.LoaiKho}' không hợp lệ. Chỉ chấp nhận COMPANY hoặc CANHAN");
            }
        }

        /// <summary>
        /// Xử lý phiếu trả kho (XN - Xuất nhập)
        /// Điều kiện: Kho nhập là kho công ty
        /// Logic: Trả vật tư từ kho cá nhân (MaKhoXuat) → Nhập lại kho công ty (MaKhoNhap)
        /// </summary>
        private int ProcessTraKho(PhieuGroup phieuGroup, string createdBy)
        {
            // Validate kho nhập phải là kho công ty
            if (string.IsNullOrEmpty(phieuGroup.MaKhoNhap))
                throw new Exception("Phiếu trả kho phải có kho nhập");

            var khoNhap = warehouseDAL.GetWarehouseByCode(phieuGroup.MaKhoNhap);
            if (khoNhap == null)
                throw new Exception($"Không tìm thấy kho nhập: {phieuGroup.MaKhoNhap}");

            if (khoNhap.LoaiKho != "COMPANY")
                throw new Exception($"Kho nhập {phieuGroup.MaKhoNhap} không phải kho công ty (LoaiKho: {khoNhap.LoaiKho})");

            // Validate kho xuất (kho cá nhân)
            if (string.IsNullOrEmpty(phieuGroup.MaKhoXuat))
                throw new Exception("Phiếu trả kho phải có kho xuất");

            var khoXuat = warehouseDAL.GetWarehouseByCode(phieuGroup.MaKhoXuat);
            if (khoXuat == null)
                throw new Exception($"Không tìm thấy kho xuất: {phieuGroup.MaKhoXuat}");

            // Xử lý trả kho (nhập lại kho công ty)
            return bravoImportDAL.CreateTraKhoTransaction(
                phieuGroup.SoPhieu,
                phieuGroup.NgayGiaoDich,
                phieuGroup.NoiDungPhieu,
                khoXuat.Id,
                khoNhap.Id,
                phieuGroup.Items,
                createdBy
            );
        }

        /// <summary>
        /// Xử lý phiếu hoàn ứng (HU)
        /// Logic: Chỉ trừ vật tư từ kho cá nhân, KHÔNG nhập vào kho nào khác
        /// Chỉ cần 1 trong 2 cột (MaKhoNhap hoặc MaKhoXuat) có giá trị, đó chính là kho được hoàn ứng
        /// </summary>
        private int ProcessHoanUng(PhieuGroup phieuGroup, string createdBy)
        {
            // Xác định kho hoàn ứng: ưu tiên MaKhoXuat, nếu không có thì lấy MaKhoNhap
            string? maKhoHoanUng = !string.IsNullOrEmpty(phieuGroup.MaKhoXuat) 
                ? phieuGroup.MaKhoXuat 
                : phieuGroup.MaKhoNhap;

            // Validate phải có ít nhất 1 kho
            if (string.IsNullOrEmpty(maKhoHoanUng))
                throw new Exception("Phiếu hoàn ứng phải có kho nhập hoặc kho xuất");

            var khoHoanUng = warehouseDAL.GetWarehouseByCode(maKhoHoanUng);
            if (khoHoanUng == null)
                throw new Exception($"Không tìm thấy kho: {maKhoHoanUng}");

            // Xử lý hoàn ứng (chỉ trừ kho cá nhân)
            return bravoImportDAL.CreateHoanUngTransaction(
                phieuGroup.SoPhieu,
                phieuGroup.NgayGiaoDich,
                phieuGroup.NoiDungPhieu,
                khoHoanUng.Id,
                phieuGroup.Items,
                createdBy
            );
        }
    }
}
