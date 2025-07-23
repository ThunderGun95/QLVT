namespace QLVT.Models
{
    public class Menu
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public int? ParentID { get; set; }
        public string FormName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public string MenuIcon { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        // Navigation properties
        public List<Menu> SubMenus { get; set; } = new List<Menu>();
        public bool HasPermission { get; set; } = false;
    }
}
