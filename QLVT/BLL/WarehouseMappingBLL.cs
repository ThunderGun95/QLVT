using QLVT.Models;
using QLVT.DAL;

namespace QLVT.BLL
{
    /// <summary>
    /// Business Logic Layer cho mapping kho giữa ERP và hệ thống nội bộ
    /// </summary>
    public class WarehouseMappingBLL
    {
        private readonly WarehouseDAL warehouseDAL;

        public WarehouseMappingBLL()
        {
            warehouseDAL = new WarehouseDAL();
        }

        /// <summary>
        /// Map mã kho từ ERP sang ID kho nội bộ
        /// Quy tắc: Kho ERP (1, 3, 4) -> Kho công ty (ID = 6)
        /// </summary>
        /// <param name="erpWarehouseCode">Mã kho từ ERP</param>
        /// <returns>ID kho trong hệ thống nội bộ</returns>
        public int? MapERPWarehouseToInternal(string erpWarehouseCode)
        {
            if (string.IsNullOrWhiteSpace(erpWarehouseCode))
                return null;

            // Mapping rules: Các kho ERP này sẽ map về kho công ty (ID = 6)
            var companyWarehouseCodes = new[] { "1", "3", "4" };
            
            if (companyWarehouseCodes.Contains(erpWarehouseCode.Trim()))
            {
                return 6; // ID của kho công ty trong hệ thống nội bộ
            }

            // Nếu không match rule nào, trả về null
            return null;
        }

        /// <summary>
        /// Lấy thông tin kho nội bộ từ mã kho ERP
        /// </summary>
        /// <param name="erpWarehouseCode">Mã kho từ ERP</param>
        /// <returns>Thông tin kho nội bộ hoặc null</returns>
        public Warehouse? GetInternalWarehouseFromERP(string erpWarehouseCode)
        {
            var internalWarehouseId = MapERPWarehouseToInternal(erpWarehouseCode);
            
            if (internalWarehouseId.HasValue)
            {
                return warehouseDAL.GetWarehouseById(internalWarehouseId.Value);
            }

            return null;
        }

        /// <summary>
        /// Kiểm tra xem mã kho ERP có được hỗ trợ không
        /// </summary>
        /// <param name="erpWarehouseCode">Mã kho từ ERP</param>
        /// <returns>True nếu được hỗ trợ</returns>
        public bool IsERPWarehouseSupported(string erpWarehouseCode)
        {
            return MapERPWarehouseToInternal(erpWarehouseCode).HasValue;
        }

        /// <summary>
        /// Lấy danh sách các mã kho ERP được hỗ trợ
        /// </summary>
        /// <returns>Danh sách mã kho ERP</returns>
        public List<string> GetSupportedERPWarehouseCodes()
        {
            return new List<string> { "1", "3", "4" };
        }
    }
}
