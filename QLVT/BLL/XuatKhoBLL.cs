using QLVT.DAL;
using QLVT.Models;
using QLVT.ERP.DAL;
using QLVT.ERP.Models;
using QLVT.Utils;

namespace QLVT.BLL
{
    public class XuatKhoBLL
    {
        private readonly ERPXuatKhoDAL erpXuatKhoErpDAL;
        private readonly SupplyMappingDAL supplyMappingDAL;
        private readonly WarehouseDAL warehouseDAL;
        private readonly XuatKhoTransactionDAL xuatKhoTransactionDAL;

        public XuatKhoBLL()
        {
            erpXuatKhoErpDAL = new ERPXuatKhoDAL();
            supplyMappingDAL = new SupplyMappingDAL();
            warehouseDAL = new WarehouseDAL();
            xuatKhoTransactionDAL = new XuatKhoTransactionDAL();
        }

        /// <summary>
        /// Lấy phiếu xuất từ ERP và thực hiện mapping tự động
        /// </summary>
        /// <param name="soPhieu">Số phiếu xuất</param>
        /// <param name="nam">Năm của phiếu</param>
        /// <returns>Phiếu xuất với mapping</returns>
        public ERP_PhieuXuatKho? GetExportOrderWithMapping(string soPhieu, int nam)
        {
            try
            {
                // Kiểm tra phiếu đã được xử lý chưa
                if (erpXuatKhoErpDAL.IsExportOrderProcessed(soPhieu, nam))
                    throw new Exception($"Phiếu xuất {soPhieu}-{nam} đã được xử lý rồi");

                // Lấy phiếu xuất từ ERP
                var erpOrder = erpXuatKhoErpDAL.GetPhieuXuatKhoErp(soPhieu, nam);
                if (erpOrder == null)
                    throw new Exception($"Không tìm thấy phiếu xuất {soPhieu}-{nam} trong hệ thống ERP");

                // Convert sang model của QLVT với mapping
                var order = new ERP_PhieuXuatKho
                {
                    MaPhieuXuatKhoVatTu = erpOrder.MaPhieuXuatKhoVatTu,
                    SoPhieuXuatKho = erpOrder.SoPhieuXuatKho,
                    NAM = erpOrder.NAM,
                    TenNhanVien = erpOrder.TenNhanVien,
                    MaNhanVien = erpOrder.MaNhanVien,
                    ThoiGianHoanThanhXuatKho = erpOrder.ThoiGianHoanThanhXuatKho,
                    MaNhanVienXuat = erpOrder.MaNhanVienXuat,
                    TenNhanVienXuat = erpOrder.TenNhanVienXuat,
                    ChiTiet = new List<ERP_PhieuXuatKhoChiTiet>()
                };

                // Thực hiện mapping vật tư cho tất cả chi tiết trước
                foreach (var erpDetail in erpOrder.ChiTiet)
                {
                    var detail = new ERP_PhieuXuatKhoChiTiet
                    {
                        MaPhieuXuatKhoVatTu = erpDetail.MaPhieuXuatKhoVatTu,
                        MaVatTuHangHoa = erpDetail.MaVatTuHangHoa,
                        TenVatTu = erpDetail.TenVatTu,
                        SoLuongXuatKho = erpDetail.SoLuongXuatKho,
                        MucDichSuDung = erpDetail.MucDichSuDung,
                        DonViTinh = erpDetail.DonViTinh,
                        MaKhoXuat = erpDetail.MaKhoXuat,
                        TenKhoXuat = erpDetail.TenKhoXuat
                    };

                    // Mapping vật tư
                    var supply = supplyMappingDAL.FindSupplyByERPCode(detail.MaVatTuHangHoa);
                    if (supply != null)
                    {
                        detail.MappedSupplyId = supply.ErpId;
                        detail.MappedSupplyCode = supply.Code;
                        detail.MappedSupplyName = supply.TenVatTu;
                        detail.MappedUnit = supply.TenDVT;
                    }

                    order.ChiTiet.Add(detail);
                }

                // Sau khi đã mapping tất cả vật tư, thực hiện warehouse mapping
                // Sử dụng ERPWarehouseMapping với toàn bộ danh sách chi tiết để áp dụng quy tắc đặc biệt cho kho 3
                foreach (var detail in order.ChiTiet)
                {
                    if (!string.IsNullOrEmpty(detail.MaKhoXuat))
                    {
                        // Sử dụng ERPWarehouseMapping để map mã kho ERP sang QLVT với danh sách chi tiết
                        var qlvtWarehouseCode = ERPWarehouseMapping.MapERPToQLVT(detail.MaKhoXuat, order.ChiTiet);
                        
                        // Tìm warehouse theo mã QLVT đã được map
                        var warehouse = warehouseDAL.GetAllWarehouses()
                            .FirstOrDefault(w => w.MaKho == qlvtWarehouseCode);
                            
                        if (warehouse != null)
                        {
                            detail.SourceWarehouseId = warehouse.Id;
                        }
                        else
                        {
                            // Fallback: tìm trực tiếp theo MaKho gốc
                            var directWarehouse = warehouseDAL.GetAllWarehouses()
                                .FirstOrDefault(w => w.MaKho == detail.MaKhoXuat);
                            if (directWarehouse != null)
                            {
                                detail.SourceWarehouseId = directWarehouse.Id;
                            }
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
        /// Lấy kho cá nhân theo mã nhân viên
        /// </summary>
        /// <param name="staffCode">Mã nhân viên</param>
        /// <returns>Kho cá nhân của nhân viên hoặc null nếu không có</returns>
        public Warehouse? GetWarehouseByStaffCode(string staffCode)
        {
            try
            {
                return warehouseDAL.GetKhoUuTienByMaNV(staffCode);
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
        public (int TotalItems, int MappedItems, int UnmappedItems, int MissingWarehouses) GetMappingStatus(ERP_PhieuXuatKho order)
        {
            var totalItems = order.ChiTiet.Count;
            var mappedItems = order.ChiTiet.Count(d => d.IsMapped);
            var unmappedItems = totalItems - mappedItems;
            var missingWarehouses = order.ChiTiet.Count(d => !d.HasWarehouseMapping);

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
        public int ProcessExport(ERP_PhieuXuatKho order, int employeeWarehouseId, string createdBy)
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

                // Kiểm tra trường hợp vật tư không phải VP và không tìm thấy kho nhân viên
                ValidateNonVPSuppliesHaveEmployeeWarehouse(order, employeeWarehouseId);

                // Kiểm tra lại phiếu đã xử lý chưa
                if (erpXuatKhoErpDAL.IsExportOrderProcessed(order.SoPhieuXuatKho, order.NAM))
                    throw new Exception($"Phiếu {order.SoPhieuXuatKho}-{order.NAM} đã được xử lý rồi");

                // Thực hiện xuất kho
                return xuatKhoTransactionDAL.CreateXuatKhoErpTransaction(order, employeeWarehouseId, createdBy);
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

        /// <summary>
        /// Kiểm tra vật tư không phải VP phải có kho nhân viên hợp lệ
        /// </summary>
        /// <param name="order">Phiếu xuất</param>
        /// <param name="employeeWarehouseId">ID kho nhân viên</param>
        private void ValidateNonVPSuppliesHaveEmployeeWarehouse(ERP_PhieuXuatKho order, int employeeWarehouseId)
        {
            // Lấy tất cả vật tư trong đơn để kiểm tra DanhMuc
            var supplyIds = order.ChiTiet
                .Where(d => d.MappedSupplyId.HasValue)
                .Select(d => d.MappedSupplyId!.Value)
                .Distinct()
                .ToList();

            if (supplyIds.Count == 0) return;

            // Lấy thông tin DanhMuc từ database
            var suppliesWithCategory = supplyMappingDAL.GetSuppliesCategoryByIds(supplyIds);
            
            // Tìm vật tư không phải VP
            var nonVPSupplies = suppliesWithCategory
                .Where(s => s.DanhMuc != "VP")
                .ToList();

            // Nếu có vật tư không phải VP và không có kho nhân viên
            if (nonVPSupplies.Any() && employeeWarehouseId <= 0)
            {
                var employeeName = !string.IsNullOrEmpty(order.TenNhanVien) 
                    ? order.TenNhanVien 
                    : order.MaNhanVien;
                    
                throw new Exception($"Không tìm thấy kho nhân viên ({employeeName})");
            }
        }
    }
}
