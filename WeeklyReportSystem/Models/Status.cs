namespace WeeklyReportSystem.Models
{
    public class Status
    {
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        // Navigation property for related tickets (if needed in the future)
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
