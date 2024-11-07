namespace WeeklyReportSystem.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        // Navigation property for related users (if needed in the future)
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
