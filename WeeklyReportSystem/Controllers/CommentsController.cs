using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeklyReportSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WeeklyReportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments?ticketId={ticketId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(int ticketId)
        {
            var comments = await _context.Comments
                .Where(c => c.TicketID == ticketId) // Filter by TicketID
                .Include(c => c.User) // Include user details to fetch username
                .Select(c => new CommentDto
                {
                    CommentID = c.CommentID,
                    Text = c.CommentText,
                    UserID = c.UserID,
                    TicketID = c.TicketID,
                    CreatedOn = c.CreatedAt,
                    CommentImage = c.CommentImage != null ? Convert.ToBase64String(c.CommentImage) : null, // Convert byte array to Base64 string
                    UserName = c.User.UserName // Get the username
                })
                .ToListAsync();

            return comments;
        }


        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment([FromForm] CommentDto commentDto)
        {
            // Retrieve the currently logged-in user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(); // Handle the case where user is not logged in
            }

            byte[]? imageData = null;
            if (!string.IsNullOrEmpty(commentDto.CommentImage))
            {
                // Extract base64 string if it contains the image format prefix
                if (commentDto.CommentImage.StartsWith("data:image/"))
                {
                    var base64Data = commentDto.CommentImage.Substring(commentDto.CommentImage.IndexOf(',') + 1);
                    imageData = Convert.FromBase64String(base64Data);
                }
            }

            // Convert the byte array from the FormData to the Comment entity
            var comment = new Comment
            {
                CommentText = commentDto.Text,
                CreatedAt = DateTime.UtcNow,
                TicketID = commentDto.TicketID,
                UserID = int.Parse(userId), // Automatically assign the user who made the comment
                CommentImage = imageData // Save the image data
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Return the created comment directly
            return Created($"{Request.Scheme}://{Request.Host}/api/Comments/{comment.CommentID}", comment);
        }



        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
