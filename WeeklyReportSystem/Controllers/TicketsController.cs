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
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets(int? userId = null, int? roleId = null)
        {
            Console.WriteLine($"User ID filter received: {userId}, Role ID: {roleId}");

            var ticketsQuery = _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.User)
                .Include(t => t.Category)
                .Select(t => new TicketDto
                {
                    TicketID = t.TicketID,
                    Title = t.Title,
                    Description = t.Description,
                    StatusID = t.StatusID,
                    StatusName = t.Status.StatusName,
                    PriorityID = t.PriorityID,
                    PriorityName = t.Priority.PriorityLevel,
                    UserID = t.UserID,
                    UserName = t.User.UserName,
                    CategoryID = t.CategoryID,
                    CategoryName = t.Category.CategoryName,
                    SubmissionDate = t.SubmissionDate
                });

            if (roleId == 3 && userId.HasValue)
            {
                ticketsQuery = ticketsQuery.Where(t => t.UserID == userId.Value);
            }

            var tickets = await ticketsQuery.ToListAsync();
            Console.WriteLine($"Number of tickets returned: {tickets.Count}");
            return tickets;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.User)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.TicketID == id);

            if (ticket == null)
            {
                return NotFound();
            }

            // Create and return a TicketDto
            var ticketDto = new TicketDto
            {
                TicketID = ticket.TicketID,
                Title = ticket.Title,
                Description = ticket.Description,
                StatusID = ticket.StatusID,
                StatusName = ticket.Status.StatusName,
                PriorityID = ticket.PriorityID,
                PriorityName = ticket.Priority.PriorityLevel,
                UserID = ticket.UserID,
                UserName = ticket.User.UserName,
                CategoryID = ticket.CategoryID,
                CategoryName = ticket.Category.CategoryName,
                SubmissionDate = ticket.SubmissionDate
            };

            return ticketDto; // Return the DTO instead of the Ticket entity
        }


        // Search users by name or email
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var usersQuery = _context.Users
                .Where(u => u.UserEmail.Contains(searchTerm) || u.UserName.Contains(searchTerm)) // Search by email or username
                .Select(u => new UserDto
                {
                    UserID = u.UserID,
                    UserName = u.UserName,
                    UserEmail = u.UserEmail,
                    RoleID = u.RoleID,
                });

            var users = await usersQuery.ToListAsync();

            if (users.Count == 0)
            {
                return NotFound("No users found matching the search term.");
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(TicketDto ticketDto)
        {
            var ticket = new Ticket
            {
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                StatusID = ticketDto.StatusID,
                PriorityID = ticketDto.PriorityID,
                UserID = ticketDto.UserID, // Ensure this is set correctly
                CategoryID = ticketDto.CategoryID,
                SubmissionDate = DateTime.UtcNow // Set current date
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicket", new { id = ticket.TicketID }, ticket);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, TicketDto ticketDto)
        {
            if (id != ticketDto.TicketID)
            {
                return BadRequest("Ticket ID mismatch.");
            }

            // Find the ticket entity in the database
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            // Update the ticket entity with values from the DTO
            ticket.Title = ticketDto.Title;
            ticket.Description = ticketDto.Description;
            ticket.StatusID = ticketDto.StatusID;
            ticket.PriorityID = ticketDto.PriorityID;
            ticket.UserID = ticketDto.UserID;
            ticket.CategoryID = ticketDto.CategoryID;

            // Mark the entity as modified
            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketID == id);
        }
    }
}
