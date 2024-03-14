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
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tasks>>> Gettasks()
        {
            return await _context.tasks.ToListAsync();
        }

        [HttpGet("{id}")]
        public ActionResult<TaskDTO> GetTask(int id)
        {
            var task = _context.tasks
                .Where(t => t.TaskID == id)
                .Select(t => new TaskDTO
                {
                    TaskID = t.TaskID,
                    WorkHour = t.WorkHour,
                    CategoryID = t.CategoryID,
                    Category = t.Category,
                    TaskDescription = t.TaskDescription,
                    ReportID = t.ReportID
                })
                .SingleOrDefault();

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }


        // PUT: api/Tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTasks(int id, Tasks tasks)
        {
            if (id != tasks.TaskID)
            {
                return BadRequest();
            }

            _context.Entry(tasks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TasksExists(id))
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

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Tasks>> PostTasks(Tasks tasks)
        //{
        //    _context.tasks.Add(tasks);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTasks", new { id = tasks.TaskID }, tasks);
        //}

        [HttpPost]
        public async Task<ActionResult<Tasks>> PostTasks(TaskInputModel taskInput)
        {
            // Create a new Tasks object
            var tasks = new Tasks
            {
                WorkHour = taskInput.WorkHour,
                CategoryID = taskInput.CategoryID,
                ReportID = taskInput.ReportID,
                TaskDescription = taskInput.TaskDescription
            };

            _context.tasks.Add(tasks);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTasks", new { id = tasks.TaskID }, tasks);
        }


        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTasks(int id)
        {
            var tasks = await _context.tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }

            _context.tasks.Remove(tasks);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TasksExists(int id)
        {
            return _context.tasks.Any(e => e.TaskID == id);
        }
    }
}
