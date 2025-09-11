using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class ExportTransactionBLL
    {
        private readonly ERPExportDAL erpExportDAL;
        private readonly SupplyMappingDAL supplyMappingDAL;
        private readonly WarehouseDAL warehouseDAL;
        private readonly ExportTransactionDAL exportTransactionDAL;

        public ExportTransactionBLL()
        {
            erpExportDAL = new ERPExportDAL();
            supplyMappingDAL = new SupplyMappingDAL();
            warehouseDAL = new WarehouseDAL();
            exportTransactionDAL = new ExportTransactionDAL();
        }

        /// <summary>
        /// Lấy phiếu xuất từ ERP và thực hiện mapping tự động
        /// </summary>
        /// <param name="soPhieu">Số phiếu xuất</param>
        /// <param name="nam">Năm của phiếu</param>
        /// <returns>Phiếu xuất với mapping</returns>
        public ERPExportOrder? GetExportOrderWithMapping(string soPhieu, int nam)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(soPhieu))
                    throw new ArgumentException("Số phiếu không được để trống");

                // Kiểm tra phiếu đã được xử lý chưa
                if (erpExportDAL.IsExportOrderProcessed(soPhieu, nam))
                    throw new Exception($"Phiếu xuất {soPhieu}-{nam} đã được xử lý rồi");

                // Lấy phiếu xuất từ ERP
                var order = erpExportDAL.GetExportOrderByNumber(soPhieu, nam);
                if (order == null)
                    throw new Exception($"Không tìm thấy phiếu xuất {soPhieu}-{nam} trong hệ thống ERP");

                // Thực hiện mapping tự động cho từng chi tiết
                foreach (var detail in order.ChiTiet)
                {
                    // Mapping vật tư
                    var supply = supplyMappingDAL.FindSupplyByERPCode(detail.MaVatTuHangHoa);
                    if (supply != null)
                    {
                        detail.MappedSupplyId = supply.ErpId;
                        detail.MappedSupplyCode = supply.Code;
                        detail.MappedSupplyName = supply.TenVatTu;
                        detail.MappedUnit = supply.TenDVT;
                    }

                    // Mapping kho nguồn
                    if (!string.IsNullOrEmpty(detail.MaKhoXuat))
                    {
                        var warehouse = warehouseDAL.GetWarehouses()
                            .FirstOrDefault(w => w.MaKho == detail.MaKhoXuat);
                        if (warehouse != null)
                        {
                            detail.SourceWarehouseId = warehouse.Id;
                        }
                    }
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy phiếu xuất: {ex.Message}");
            }
        }

        /// <summary>
        /// Tìm kiếm vật tư cho mapping thủ công
        /// </summary>
        /// <param name="keyword">Từ khóa</param>
        /// <returns>Danh sách vật tư</returns>
        public List<Supply> SearchSuppliesForMapping(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return new List<Supply>();

                return supplyMappingDAL.SearchSupplies(keyword);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Tìm kiếm vật tư: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách kho nhân viên (kho đích để chuyển vật tư)
        /// </summary>
        /// <returns>Danh sách kho</returns>
        public List<Warehouse> GetEmployeeWarehouses()
        {
            try
            {
                return warehouseDAL.GetWarehouses().Where(w => w.LoaiKho == "PERSONAL").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy danh sách kho nhân viên: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy kho cá nhân theo mã nhân viên
        /// </summary>
        /// <param name="staffCode">Mã nhân viên</param>
        /// <returns>Kho cá nhân của nhân viên hoặc null nếu không có</returns>
        public Warehouse? GetWarehouseByStaffCode(string staffCode)
        {
            try
            {
                return warehouseDAL.GetWarehouseByStaffCode(staffCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy kho nhân viên {staffCode}: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái mapping của phiếu xuất
        /// </summary>
        /// <param name="order">Phiếu xuất</param>
        /// <returns>Thông tin trạng thái</returns>
        public (int TotalItems, int MappedItems, int UnmappedItems, int MissingWarehouses) GetMappingStatus(ERPExportOrder order)
        {
            var totalItems = order.ChiTiet.Count;
            var mappedItems = order.ChiTiet.Count(d => d.IsMapped);
            var unmappedItems = totalItems - mappedItems;
            var missingWarehouses = order.ChiTiet.Count(d => !d.HasSourceWarehouse);

            return (totalItems, mappedItems, unmappedItems, missingWarehouses);
        }

        /// <summary>
        /// Xử lý xuất kho
        /// </summary>
        /// <param name="order">Phiếu xuất</param>
        /// <param name="employeeWarehouseId">ID kho nhân viên đích</param>
        /// <param name="createdBy">Người tạo</param>
        /// <param name="staffCode">Mã nhân viên</param>
        /// <returns>ID transaction</returns>
        public int ProcessExport(ERPExportOrder order, int employeeWarehouseId, string createdBy, string staffCode)
        {
            try
            {
                // Validate
                if (order == null)
                    throw new ArgumentException("Phiếu xuất không hợp lệ");

                var (_, _, unmapped, missingWarehouses) = GetMappingStatus(order);
                if (unmapped > 0)
                    throw new Exception($"Còn {unmapped} vật tư chưa được mapping");
                
                if (missingWarehouses > 0)
                    throw new Exception($"Còn {missingWarehouses} vật tư chưa xác định kho nguồn");

                if (employeeWarehouseId <= 0)
                    throw new ArgumentException("Chưa chọn kho nhân viên đích");

                if (string.IsNullOrWhiteSpace(staffCode))
                    throw new ArgumentException("Chưa chọn nhân viên thực hiện");

                // Kiểm tra lại phiếu đã xử lý chưa
                if (erpExportDAL.IsExportOrderProcessed(order.SoPhieuXuatKho, order.NAM))
                    throw new Exception($"Phiếu {order.SoPhieuXuatKho}-{order.NAM} đã được xử lý rồi");

                // Thực hiện xuất kho
                return exportTransactionDAL.CreateExportTransaction(order, employeeWarehouseId, createdBy, staffCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Xử lý xuất kho: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra kết nối ERP
        /// </summary>
        /// <returns>True nếu kết nối OK</returns>
        public bool TestERPConnection()
        {
            return Utils.ExternalDatabaseHelper.TestExternalConnection();
        }
    }
}
