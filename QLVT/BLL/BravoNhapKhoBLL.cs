using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    /// <summary>
    /// Business logic xử lý import phiếu nhập kho từ Bravo
    /// </summary>
    public class BravoNhapKhoBLL
    {
        private readonly BravoNhapKhoDAL bravoNhapKhoDAL;
        private readonly WarehouseDAL warehouseDAL;

        public BravoNhapKhoBLL()
        {
            bravoNhapKhoDAL = new BravoNhapKhoDAL();
            warehouseDAL = new WarehouseDAL();
        }

        /// <summary>
        /// Import phiếu nhập kho từ Bravo
        /// Logic: Nhập vật tư vào kho theo mã kho
        /// </summary>
        /// <param name="importData">Dữ liệu import đã được validate</param>
        /// <param name="createdBy">Người tạo</param>
        /// <returns>(Thành công, Thất bại, Danh sách lỗi)</returns>
        public (int success, int failed, List<string> errors) ProcessImport(BravoNhapKhoImportData importData, string createdBy)
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
                    if (bravoNhapKhoDAL.IsPhieuProcessed(phieuGroup.SoPhieu))
                    {
                        errors.Add($"Phiếu {phieuGroup.SoPhieu}: Đã được xử lý trước đó");
                        failedCount++;
                        continue;
                    }

                    // Validate kho tồn tại
                    var warehouse = warehouseDAL.GetWarehouseByCode(phieuGroup.MaKho);
                    if (warehouse == null)
                    {
                        errors.Add($"Phiếu {phieuGroup.SoPhieu}: Không tìm thấy kho {phieuGroup.MaKho}");
                        failedCount++;
                        continue;
                    }

                    // Tạo transaction nhập kho
                    int transactionId = bravoNhapKhoDAL.CreateNhapKhoTransaction(
                        phieuGroup.SoPhieu,
                        phieuGroup.NgayGiaoDich,
                        phieuGroup.NoiDungPhieu,
                        warehouse.Id,
                        phieuGroup.Items,
                        createdBy
                    );

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
    }
}
