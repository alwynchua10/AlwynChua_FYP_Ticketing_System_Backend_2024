namespace WeeklyReportSystem.Models
{
    public class Priority
    {
        public int PriorityID { get; set; }
        public string PriorityLevel { get; set; }

        // Navigation property for related tickets (optional)
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
