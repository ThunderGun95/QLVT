namespace QLVT.Models
{
    public class Permission
    {
        public int RoleID { get; set; }
        public int MenuID { get; set; }
        public bool CanAccess { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
