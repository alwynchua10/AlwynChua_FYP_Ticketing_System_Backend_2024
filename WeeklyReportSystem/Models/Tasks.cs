using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WeeklyReportSystem.Models
{
    public class Tasks
    {
        [Key]
        public int TaskID { get; set; }
        public decimal WorkHour { get; set; }
        public int CategoryID { get; set; } // Foreign key for category

        // Navigation property for the associated category
        public virtual Category Category { get; set; }

        public string TaskDescription { get; set; }

        public int ReportID { get; set; }
        // Navigation property for the associated report
        //[JsonIgnore]
        //public virtual Report Report { get; set; }
    }

}
