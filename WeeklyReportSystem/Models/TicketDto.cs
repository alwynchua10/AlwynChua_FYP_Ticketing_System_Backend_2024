namespace WeeklyReportSystem.DTOs
{
    public class TicketDto
    {
        public int TicketID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public int PriorityID { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public DateTime SubmissionDate { get; set; }
        // You can remove these properties if they are not necessary for ticket creation
        public string? StatusName { get; set; } // Make these optional
        public string? PriorityName { get; set; }
        public string? UserName { get; set; }
        public string? CategoryName { get; set; }
    }
}
