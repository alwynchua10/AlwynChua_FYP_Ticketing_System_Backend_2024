using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeklyReportSystem.Models;
using WeeklyReportSystem.DTOs;

namespace WeeklyReportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrioritiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PrioritiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Priority>>> GetPriorities()
        {
            return await _context.Priorities.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Priority>> GetPriority(int id)
        {
            var priority = await _context.Priorities.FindAsync(id);
            if (priority == null)
            {
                return NotFound();
            }
            return priority;
        }

        [HttpPost]
        public async Task<ActionResult<Priority>> PostPriority(PriorityDto priorityDto)
        {
            var priority = new Priority
            {
                PriorityLevel = priorityDto.PriorityLevel
            };

            _context.Priorities.Add(priority);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPriority", new { id = priority.PriorityID }, priority);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPriority(int id, Priority priority)
        {
            if (id != priority.PriorityID)
            {
                return BadRequest();
            }
            _context.Entry(priority).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriority(int id)
        {
            var priority = await _context.Priorities.FindAsync(id);
            if (priority == null)
            {
                return NotFound();
            }
            _context.Priorities.Remove(priority);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PriorityExists(int id)
        {
            return _context.Priorities.Any(e => e.PriorityID == id);
        }
    }
}
