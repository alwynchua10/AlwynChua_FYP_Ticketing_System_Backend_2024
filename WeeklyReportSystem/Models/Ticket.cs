using System;
using System.Collections.Generic;

namespace WeeklyReportSystem.Models
{
    public class Ticket
    {
        public int TicketID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public int PriorityID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; } // Foreign key to Category
        public DateTime SubmissionDate { get; set; } // Add this property if needed

        // Navigation properties
        public virtual Status Status { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual User User { get; set; }
        public virtual Category Category { get; set; } // Navigation property to Category
        public virtual ICollection<Comment> Comments { get; set; } // If you want to allow comments on tickets
    }
}
