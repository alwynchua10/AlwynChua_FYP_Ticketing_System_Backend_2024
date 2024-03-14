using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeklyReportSystem.Models;

namespace WeeklyReportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> Getreports([FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate, [FromQuery] int? workWeek)
        {
            // Check if startDate and endDate are provided, if not, set them to the current week
            if (startDate == null || endDate == null)
            {
                // Get the current date as DateTime
                var currentDate = DateTime.Today;

                // Convert the current date to DateOnly
                var currentDateOnly = new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day);

                // Calculate the start date of the current week (Sunday)
                startDate = currentDateOnly.AddDays(-(int)currentDateOnly.DayOfWeek);

                // Calculate the end date of the current week (Saturday)
                endDate = startDate.Value.AddDays(6);
            }

            // Fetch reports from the database within the specified date range
            var reportsQuery = _context.reports
                .Where(r => r.SubmissionDateTime >= startDate && r.SubmissionDateTime <= endDate);

            // Filter reports by work week if provided
            if (workWeek.HasValue)
            {
                reportsQuery = reportsQuery.Where(r => r.WorkWeek == workWeek.Value);
            }

            var reports = await reportsQuery.ToListAsync();

            return reports;
        }





        // GET: api/Reports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            var report = await _context.reports.FindAsync(id);

            if (report == null)
            {
                return NotFound();
            }

            return report;
        }

        // PUT: api/Reports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutReport(int id, Report report)
        //{
        //    if (id != report.ReportID)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(report).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ReportExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // PUT: api/Reports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReport(int id, Report report)
        {
            if (id != report.ReportID)
            {
                return BadRequest();
            }

            // Retrieve the existing report from the database
            var existingReport = await _context.reports
                .Include(r => r.Tasks) // Include tasks related to the report
                .FirstOrDefaultAsync(r => r.ReportID == id);

            if (existingReport == null)
            {
                return NotFound();
            }

            // Update report properties
            existingReport.WorkWeek = report.WorkWeek;
            existingReport.TotalWorkHour = report.TotalWorkHour;
            existingReport.SubmissionDateTime = report.SubmissionDateTime;

            // Update tasks associated with the report
            foreach (var updatedTask in report.Tasks)
            {
                var existingTask = existingReport.Tasks.FirstOrDefault(t => t.TaskID == updatedTask.TaskID);
                if (existingTask != null)
                {
                    existingTask.WorkHour = updatedTask.WorkHour;
                    existingTask.CategoryID = updatedTask.CategoryID;
                    existingTask.TaskDescription = updatedTask.TaskDescription;
                }
                else
                {
                    // Task not found, handle accordingly (e.g., create new task, return an error, etc.)
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Reports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Report>> PostReport(Report report)
        //{
        //    _context.reports.Add(report);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetReport", new { id = report.ReportID }, report);
        //}

        // DELETE: api/Reports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await _context.reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            _context.reports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReportExists(int id)
        {
            return _context.reports.Any(e => e.ReportID == id);
        }


        [HttpPost("CreateReportWithTasks")]
        public async Task<IActionResult> CreateReportWithTasks(ReportWithTasksViewModel reportWithTasksViewModel)
        {
            try
            {
                // Create a new Report entity
                var report = new Report
                {
                    WorkWeek = reportWithTasksViewModel.WorkWeek,
                    TotalWorkHour = reportWithTasksViewModel.TotalWorkHour,
                    SubmissionDateTime = reportWithTasksViewModel.SubmissionDateTime,
                    Tasks = new List<Tasks>()
                };

                // Map each task from the view model to the Task entity and add it to the report
                foreach (var taskViewModel in reportWithTasksViewModel.Tasks)
                {
                    var task = new Tasks
                    {
                        WorkHour = taskViewModel.WorkHour,
                        CategoryID = taskViewModel.CategoryID,
                        TaskDescription = taskViewModel.TaskDescription
                    };
                    report.Tasks.Add(task);
                }

                // Add the report to the context and save changes
                _context.reports.Add(report);
                await _context.SaveChangesAsync();

                // Return the created report
                return CreatedAtAction(nameof(GetReport), new { id = report.ReportID }, report);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "An error occurred while creating the report with tasks.");
            }
        }





    }
}
