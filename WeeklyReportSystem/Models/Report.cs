using System.Text.Json.Serialization;

namespace WeeklyReportSystem.Models
{
    public class Report
    {
        public int ReportID { get; set; }
        public int WorkWeek { get; set; }
        public decimal TotalWorkHour { get; set; }
        public DateOnly SubmissionDateTime { get; set; }

        // Navigation property for tasks associated with this report

        public virtual ICollection<Tasks> Tasks { get; set; }
    }

}
