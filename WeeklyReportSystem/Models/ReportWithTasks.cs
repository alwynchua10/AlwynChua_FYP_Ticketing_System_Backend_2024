namespace WeeklyReportSystem.Models
{
    public class ReportWithTasksViewModel
    {
        public int WorkWeek { get; set; }
        public decimal TotalWorkHour { get; set; }

        public DateOnly SubmissionDateTime { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }

    public class TaskViewModel
    {
        public decimal WorkHour { get; set; }
        public int CategoryID { get; set; }
        public string TaskDescription { get; set; }
    }
}
