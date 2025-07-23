using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class MenuDAL
    {
        /// <summary>
        /// Lấy danh sách menu mà user có quyền truy cập
        /// </summary>
        /// <param name="userID">ID người dùng</param>
        /// <returns>Danh sách Menu có phân cấp</returns>
        public List<Menu> GetUserMenus(int userID)
        {
            var allMenus = new List<Menu>();
            
            string sql = @"
                SELECT DISTINCT m.MenuID, m.MenuName, m.ParentID, m.FormName, 
                       m.SortOrder, m.MenuIcon, m.IsActive
                FROM Menus m
                INNER JOIN Permissions p ON m.MenuID = p.MenuID
                INNER JOIN UserRoles ur ON p.RoleID = ur.RoleID
                WHERE ur.UserID = @UserID 
                  AND m.IsActive = 1 
                  AND p.CanAccess = 1
                ORDER BY m.SortOrder, m.MenuName";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var menu = new Menu
                            {
                                MenuID = Convert.ToInt32(reader["MenuID"]),
                                MenuName = Convert.ToString(reader["MenuName"]) ?? string.Empty,
                                SortOrder = Convert.ToInt32(reader["SortOrder"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                HasPermission = true
                            };

                            // Handle nullable fields
                            if (reader["ParentID"] != DBNull.Value)
                                menu.ParentID = Convert.ToInt32(reader["ParentID"]);

                            if (reader["FormName"] != DBNull.Value)
                                menu.FormName = Convert.ToString(reader["FormName"]) ?? string.Empty;

                            if (reader["MenuIcon"] != DBNull.Value)
                                menu.MenuIcon = Convert.ToString(reader["MenuIcon"]) ?? string.Empty;

                            allMenus.Add(menu);
                        }
                    }
                }
            }

            // Tạo cấu trúc phân cấp menu
            return BuildMenuHierarchy(allMenus);
        }

        /// <summary>
        /// Xây dựng cấu trúc phân cấp menu
        /// </summary>
        /// <param name="flatMenus">Danh sách menu phẳng</param>
        /// <returns>Danh sách menu có phân cấp</returns>
        private List<Menu> BuildMenuHierarchy(List<Menu> flatMenus)
        {
            var menuDictionary = flatMenus.ToDictionary(m => m.MenuID, m => m);
            var rootMenus = new List<Menu>();

            foreach (var menu in flatMenus)
            {
                if (menu.ParentID == null)
                {
                    // Menu gốc
                    rootMenus.Add(menu);
                }
                else if (menuDictionary.ContainsKey(menu.ParentID.Value))
                {
                    // Menu con
                    var parentMenu = menuDictionary[menu.ParentID.Value];
                    parentMenu.SubMenus.Add(menu);
                }
            }

            // Sắp xếp menu theo SortOrder
            rootMenus = rootMenus.OrderBy(m => m.SortOrder).ThenBy(m => m.MenuName).ToList();
            
            foreach (var menu in rootMenus)
            {
                menu.SubMenus = menu.SubMenus.OrderBy(m => m.SortOrder).ThenBy(m => m.MenuName).ToList();
            }

            return rootMenus;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập của user với một menu cụ thể
        /// </summary>
        /// <param name="userID">ID người dùng</param>
        /// <param name="menuID">ID menu</param>
        /// <returns>Permission object</returns>
        public Permission? GetUserPermission(int userID, int menuID)
        {
            string sql = @"
                SELECT p.RoleID, p.MenuID, p.CanAccess, p.CanCreate, 
                       p.CanRead, p.CanUpdate, p.CanDelete
                FROM Permissions p
                INNER JOIN UserRoles ur ON p.RoleID = ur.RoleID
                WHERE ur.UserID = @UserID AND p.MenuID = @MenuID
                  AND p.CanAccess = 1";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@MenuID", menuID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Permission
                            {
                                RoleID = Convert.ToInt32(reader["RoleID"]),
                                MenuID = Convert.ToInt32(reader["MenuID"]),
                                CanAccess = Convert.ToBoolean(reader["CanAccess"]),
                                CanCreate = Convert.ToBoolean(reader["CanCreate"]),
                                CanRead = Convert.ToBoolean(reader["CanRead"]),
                                CanUpdate = Convert.ToBoolean(reader["CanUpdate"]),
                                CanDelete = Convert.ToBoolean(reader["CanDelete"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Lấy tất cả menu trong hệ thống
        /// </summary>
        /// <returns>Danh sách tất cả menu</returns>
        public List<Menu> GetAllMenus()
        {
            var menus = new List<Menu>();
            
            string sql = @"
                SELECT MenuID, MenuName, ParentID, FormName, SortOrder, MenuIcon, IsActive
                FROM Menus
                WHERE IsActive = 1
                ORDER BY SortOrder, MenuName";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var menu = new Menu
                            {
                                MenuID = Convert.ToInt32(reader["MenuID"]),
                                MenuName = Convert.ToString(reader["MenuName"]) ?? string.Empty,
                                SortOrder = Convert.ToInt32(reader["SortOrder"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };

                            // Handle nullable fields
                            if (reader["ParentID"] != DBNull.Value)
                                menu.ParentID = Convert.ToInt32(reader["ParentID"]);

                            if (reader["FormName"] != DBNull.Value)
                                menu.FormName = Convert.ToString(reader["FormName"]) ?? string.Empty;

                            if (reader["MenuIcon"] != DBNull.Value)
                                menu.MenuIcon = Convert.ToString(reader["MenuIcon"]) ?? string.Empty;

                            menus.Add(menu);
                        }
                    }
                }
            }

            return BuildMenuHierarchy(menus);
        }
    }
}
