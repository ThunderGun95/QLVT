using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class MenuBLL
    {
        private readonly MenuDAL menuDAL;

        public MenuBLL()
        {
            menuDAL = new MenuDAL();
        }

        /// <summary>
        /// Lấy menu của user hiện tại
        /// </summary>
        /// <returns>Danh sách menu có phân quyền</returns>
        public List<Menu> GetCurrentUserMenus()
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();
            if (currentUser == null)
                return new List<Menu>();

            return menuDAL.GetUserMenus(currentUser.UserID);
        }

        /// <summary>
        /// Kiểm tra quyền truy cập menu
        /// </summary>
        /// <param name="menuID">ID menu</param>
        /// <returns>Permission object</returns>
        public Permission? CheckMenuPermission(int menuID)
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();
            if (currentUser == null)
                return null;

            return menuDAL.GetUserPermission(currentUser.UserID, menuID);
        }

        /// <summary>
        /// Kiểm tra xem user có quyền truy cập menu không
        /// </summary>
        /// <param name="menuID">ID menu</param>
        /// <returns>True nếu có quyền</returns>
        public bool HasMenuAccess(int menuID)
        {
            var permission = CheckMenuPermission(menuID);
            return permission?.CanAccess ?? false;
        }

        /// <summary>
        /// Kiểm tra quyền tạo mới
        /// </summary>
        /// <param name="menuID">ID menu</param>
        /// <returns>True nếu có quyền</returns>
        public bool CanCreate(int menuID)
        {
            var permission = CheckMenuPermission(menuID);
            return permission?.CanCreate ?? false;
        }

        /// <summary>
        /// Kiểm tra quyền đọc
        /// </summary>
        /// <param name="menuID">ID menu</param>
        /// <returns>True nếu có quyền</returns>
        public bool CanRead(int menuID)
        {
            var permission = CheckMenuPermission(menuID);
            return permission?.CanRead ?? false;
        }

        /// <summary>
        /// Kiểm tra quyền cập nhật
        /// </summary>
        /// <param name="menuID">ID menu</param>
        /// <returns>True nếu có quyền</returns>
        public bool CanUpdate(int menuID)
        {
            var permission = CheckMenuPermission(menuID);
            return permission?.CanUpdate ?? false;
        }

        /// <summary>
        /// Kiểm tra quyền xóa
        /// </summary>
        /// <param name="menuID">ID menu</param>
        /// <returns>True nếu có quyền</returns>
        public bool CanDelete(int menuID)
        {
            var permission = CheckMenuPermission(menuID);
            return permission?.CanDelete ?? false;
        }

        /// <summary>
        /// Lấy tất cả menu (cho admin)
        /// </summary>
        /// <returns>Danh sách tất cả menu</returns>
        public List<Menu> GetAllMenus()
        {
            return menuDAL.GetAllMenus();
        }
    }
}
