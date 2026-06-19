using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class WarehouseManagementBLL
    {
        private readonly WarehouseDAL warehouseDAL;
        private readonly StaffDAL staffDAL;

        public WarehouseManagementBLL()
        {
            warehouseDAL = new WarehouseDAL();
            staffDAL = new StaffDAL();
        }

        public List<Warehouse> GetWarehouses()
        {
            return warehouseDAL.GetWarehousesForManagement();
        }

        public List<Staff> GetStaffs()
        {
            return staffDAL.GetAllStaffs();
        }

        public int SaveWarehouse(Warehouse warehouse)
        {
            ValidateWarehouse(warehouse);
            return warehouseDAL.SaveWarehouse(warehouse);
        }

        public void DeactivateWarehouse(int warehouseId)
        {
            if (warehouseId <= 0)
                throw new Exception("Chưa chọn kho cần ngưng hoạt động");

            warehouseDAL.DeactivateWarehouse(warehouseId);
        }

        public void SetPriorityWarehouse(Warehouse warehouse)
        {
            if (warehouse.Id <= 0)
                throw new Exception("Chưa chọn kho ưu tiên");

            if (!IsPersonalWarehouse(warehouse.LoaiKho))
                throw new Exception("Chỉ kho cá nhân mới được đặt ưu tiên");

            if (string.IsNullOrWhiteSpace(warehouse.MaNV))
                throw new Exception("Kho cá nhân chưa có mã nhân viên");

            warehouseDAL.SetPriorityWarehouse(warehouse.MaNV, warehouse.Id);
        }

        public static bool IsCompanyWarehouse(string loaiKho)
        {
            return string.Equals(loaiKho, "COMPANY", StringComparison.OrdinalIgnoreCase)
                || string.Equals(loaiKho, "CongTy", StringComparison.OrdinalIgnoreCase)
                || string.Equals(loaiKho, "Công ty", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsPersonalWarehouse(string loaiKho)
        {
            return string.Equals(loaiKho, "CANHAN", StringComparison.OrdinalIgnoreCase)
                || string.Equals(loaiKho, "PERSONAL", StringComparison.OrdinalIgnoreCase)
                || string.Equals(loaiKho, "Cá nhân", StringComparison.OrdinalIgnoreCase);
        }

        private static void ValidateWarehouse(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));

            if (string.IsNullOrWhiteSpace(warehouse.MaKho))
                throw new Exception("Mã kho không được để trống");

            if (string.IsNullOrWhiteSpace(warehouse.TenKho))
                throw new Exception("Tên kho không được để trống");

            if (IsPersonalWarehouse(warehouse.LoaiKho) && string.IsNullOrWhiteSpace(warehouse.MaNV))
                throw new Exception("Kho cá nhân phải chọn nhân viên");
        }
    }
}
