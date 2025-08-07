using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class OpeningInventoryBLL
    {
        private readonly OpeningInventoryDAL openingInventoryDAL;
        private readonly SupplyMappingDAL supplyMappingDAL;

        public OpeningInventoryBLL()
        {
            openingInventoryDAL = new OpeningInventoryDAL();
            supplyMappingDAL = new SupplyMappingDAL();
        }

        /// <summary>
        /// Lấy danh sách tồn kho đầu kỳ
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <returns>Danh sách tồn kho</returns>
        public List<OpeningInventory> GetOpeningInventories(string? maKho = null)
        {
            try
            {
                return openingInventoryDAL.GetOpeningInventories(maKho);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Lấy tồn kho đầu kỳ: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý mapping tự động cho danh sách nhập tồn
        /// </summary>
        /// <param name="inputs">Danh sách nhập tồn</param>
        /// <returns>Danh sách đã mapping</returns>
        public List<OpeningInventoryInput> ProcessAutoMapping(List<OpeningInventoryInput> inputs)
        {
            try
            {
                foreach (var input in inputs)
                {
                    if (!input.IsMapped && !string.IsNullOrEmpty(input.MaVatTu))
                    {
                        var supply = supplyMappingDAL.FindSupplyByERPCode(input.MaVatTu);
                        if (supply != null)
                        {
                            input.SupplyId = supply.ErpId;
                            input.TenVatTu = supply.TenVatTu;
                        }
                    }
                }

                return inputs;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Auto mapping: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật tồn kho đầu kỳ
        /// </summary>
        /// <param name="inputs">Danh sách tồn kho</param>
        /// <param name="nguoiNhap">Người thực hiện</param>
        /// <returns>Số bản ghi đã xử lý</returns>
        public int UpdateOpeningInventories(List<OpeningInventoryInput> inputs, string nguoiNhap)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nguoiNhap))
                    throw new ArgumentException("Người nhập không được để trống");

                var validInputs = inputs.Where(x => x.IsMapped && x.SoLuong > 0).ToList();
                
                if (!validInputs.Any())
                    throw new Exception("Không có dữ liệu hợp lệ để xử lý");

                return openingInventoryDAL.UpdateOpeningInventories(validInputs, nguoiNhap);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Cập nhật tồn kho: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa tồn kho đầu kỳ
        /// </summary>
        /// <param name="id">ID tồn kho</param>
        public void DeleteOpeningInventory(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID không hợp lệ");

                openingInventoryDAL.DeleteOpeningInventory(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi BLL - Xóa tồn kho: {ex.Message}");
            }
        }

        /// <summary>
        /// Tìm kiếm vật tư cho mapping
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
        /// Validate dữ liệu trước khi lưu
        /// </summary>
        /// <param name="inputs">Danh sách nhập tồn</param>
        /// <returns>Danh sách lỗi</returns>
        public List<string> ValidateInputs(List<OpeningInventoryInput> inputs)
        {
            var errors = new List<string>();

            try
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    var input = inputs[i];
                    string rowInfo = $"Dòng {i + 1}";

                    if (string.IsNullOrWhiteSpace(input.MaVatTu))
                        errors.Add($"{rowInfo}: Mã vật tư không được để trống");

                    if (input.SoLuong < 0)
                        errors.Add($"{rowInfo}: Số lượng không được âm");

                    if (!input.IsMapped && input.SoLuong > 0)
                        errors.Add($"{rowInfo}: Vật tư '{input.MaVatTu}' chưa được mapping");

                    if (string.IsNullOrWhiteSpace(input.MaKho))
                        errors.Add($"{rowInfo}: Mã kho không được để trống");
                }

                // Kiểm tra trùng lặp
                var duplicates = inputs
                    .Where(x => x.IsMapped)
                    .GroupBy(x => new { x.MaKho, x.SupplyId })
                    .Where(g => g.Count() > 1)
                    .Select(g => $"Vật tư ID {g.Key.SupplyId} trong kho {g.Key.MaKho}")
                    .ToList();

                if (duplicates.Any())
                    errors.Add($"Dữ liệu trùng lặp: {string.Join(", ", duplicates)}");
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi validation: {ex.Message}");
            }

            return errors;
        }
    }
}
