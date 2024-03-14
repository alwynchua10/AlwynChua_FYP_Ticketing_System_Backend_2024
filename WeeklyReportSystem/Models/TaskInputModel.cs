namespace WeeklyReportSystem.Models
{
    public class TaskInputModel
    {
        public decimal WorkHour { get; set; }
        public int CategoryID { get; set; }
        public int ReportID { get; set; }
        public string TaskDescription { get; set; }
    }
}
