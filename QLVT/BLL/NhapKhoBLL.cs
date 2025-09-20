using QLVT.DAL;
using QLVT.ERP.DAL;
using QLVT.ERP.Models;
using QLVT.Models;

namespace QLVT.BLL
{
    public class NhapKhoBLL
    {
        private readonly ErpNhapKhoDAL erpNhapKhoDAL;
        private readonly SupplyMappingDAL supplyMappingDAL;
        private readonly WarehouseDAL warehouseDAL;
        private readonly NhapKhoTransactionDAL nhapKhoTransactionDAL;
        private readonly WarehouseMappingBLL warehouseMappingBLL;


        public NhapKhoBLL()
        {
            erpNhapKhoDAL = new ErpNhapKhoDAL();
            supplyMappingDAL = new SupplyMappingDAL();
            warehouseDAL = new WarehouseDAL();
            nhapKhoTransactionDAL = new NhapKhoTransactionDAL();
            warehouseMappingBLL = new WarehouseMappingBLL();
        }

        /// <summary>
        /// Lấy phiếu nhập từ ERP và thực hiện mapping tự động
        /// </summary>
        /// <param name="soPhieu">Số phiếu nhập</param>
        /// <param name="nam">Năm của phiếu</param>
        /// <returns>Phiếu nhập với mapping</returns>
        public ERP_PhieuNhapKho? GetPhieuNhapKhoErpWithMapping(string soPhieu, int nam)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(soPhieu))
                    throw new ArgumentException("Số phiếu không được để trống");

                // Kiểm tra phiếu đã được xử lý chưa
                if (erpNhapKhoDAL.IsImportOrderProcessed(soPhieu, nam))
                    throw new Exception($"Phiếu nhập {soPhieu}-{nam} đã được xử lý rồi");

                // Lấy phiếu nhập từ ERP
                var erpOrder = erpNhapKhoDAL.GetPhieuNhapKhoErp(soPhieu, nam);
                if (erpOrder == null)
                    throw new Exception($"Không tìm thấy phiếu nhập {soPhieu}-{nam} trong hệ thống ERP");

                // Convert sang model của QLVT với mapping
                var order = new ERP_PhieuNhapKho
                {
                    MaPhieuNhapKhoVatTu = erpOrder.MaPhieuNhapKhoVatTu,
                    SoPhieuNhapKho = erpOrder.SoPhieuNhapKho,
                    NAM = erpOrder.NAM,
                    TenKho = erpOrder.TenKho,
                    MaKhoVatTu = erpOrder.MaKhoVatTu,
                    ThoiGianHoanThanhNhapKho = erpOrder.ThoiGianHoanThanhNhapKho,
                    MaNhanVienMua = erpOrder.MaNhanVienMua,
                    NhanVienMua = erpOrder.NhanVienMua,
                    ChiTiet = new List<ERP_PhieuNhapKhoChiTiet>()
                };

                // Thực hiện mapping tự động cho từng chi tiết
                foreach (var erpDetail in erpOrder.ChiTiet)
                {
                    var detail = new ERP_PhieuNhapKhoChiTiet
                    {
                        MaPhieuNhapKhoVatTu = erpDetail.MaPhieuNhapKhoVatTu,
                        MaVatTuHangHoa = erpDetail.MaVatTuHangHoa,
                        TenVatTu = erpDetail.TenVatTu,
                        DacTinhKyThuat = erpDetail.DacTinhKyThuat,
                        SoLuongNhapKho = erpDetail.SoLuongNhapKho,
                        DonViTinh = erpDetail.DonViTinh,
                        MaNhaSanXuat = erpDetail.MaNhaSanXuat,
                        TenNhaSanXuat = erpDetail.TenNhaSanXuat
                    };

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

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy phiếu nhập: {ex.Message}");
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
        /// Lấy danh sách kho
        /// </summary>
        /// <returns>Danh sách kho</returns>
        public List<Warehouse> GetWarehouses()
        {
            try
            {
                return warehouseDAL.GetWarehouses();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy danh sách kho: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy ID kho đích cho Import dựa trên mã kho ERP
        /// Áp dụng mapping rule: kho ERP (1,3,4,34) -> kho công ty (ID=6)
        /// </summary>
        /// <param name="erpWarehouseCode">Mã kho từ ERP</param>
        /// <returns>ID kho đích trong hệ thống nội bộ</returns>
        public int? GetTargetWarehouseIdForImport(string erpWarehouseCode)
        {
            try
            {
                var internalWarehouse = warehouseMappingBLL.GetInternalWarehouseFromERP(erpWarehouseCode);
                return internalWarehouse?.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy kho đích cho Import: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái mapping của phiếu nhập
        /// </summary>
        /// <param name="order">Phiếu nhập</param>
        /// <returns>Thông tin trạng thái</returns>
        public (int TotalItems, int MappedItems, int UnmappedItems) GetMappingStatus(ERP_PhieuNhapKho order)
        {
            var totalItems = order.ChiTiet.Count;
            var mappedItems = order.ChiTiet.Count(d => d.IsMapped);
            var unmappedItems = totalItems - mappedItems;

            return (totalItems, mappedItems, unmappedItems);
        }

        /// <summary>
        /// Xử lý nhập kho
        /// </summary>
        /// <param name="order">Phiếu nhập</param>
        /// <param name="maKho">Mã kho đích</param>
        /// <param name="createdBy">Người tạo</param>
        /// <param name="staffCode">Mã nhân viên</param>
        /// <returns>ID transaction</returns>
        public int ProcessNhapKhoErp(ERP_PhieuNhapKho order, int maKho, string createdBy, string staffCode)
        {
            try
            {
                // Validate
                if (order == null)
                    throw new ArgumentException("Phiếu nhập không hợp lệ");

                var (_, _, unmapped) = GetMappingStatus(order);
                if (unmapped > 0)
                    throw new Exception($"Còn {unmapped} vật tư chưa được mapping");

                if (maKho == 0)
                    throw new ArgumentException("Chưa chọn kho");

                if (string.IsNullOrWhiteSpace(staffCode))
                    throw new ArgumentException("Chưa chọn nhân viên thực hiện");

                // Kiểm tra lại phiếu đã xử lý chưa
                if (erpNhapKhoDAL.IsImportOrderProcessed(order.SoPhieuNhapKho, order.NAM))
                    throw new Exception($"Phiếu {order.SoPhieuNhapKho}-{order.NAM} đã được xử lý rồi");

                // Thực hiện nhập kho
                return nhapKhoTransactionDAL.CreateNhapKhoErpTransaction(order, maKho, createdBy, staffCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Xử lý nhập kho: {ex.Message}");
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
