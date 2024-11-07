namespace WeeklyReportSystem.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        // Initialize Tickets to an empty collection to make it optional
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
