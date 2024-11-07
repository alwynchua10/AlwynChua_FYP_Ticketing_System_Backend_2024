using System.Collections.Generic;

namespace WeeklyReportSystem.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string PasswordHash { get; set; } // Add this line
        public int RoleID { get; set; }

        // Navigation property for the related role
        public virtual Role Role { get; set; }

        // Navigation property for related tickets
        public virtual ICollection<Ticket> Tickets { get; set; }

        // Navigation property for related comments
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
