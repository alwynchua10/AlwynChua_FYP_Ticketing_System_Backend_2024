using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeklyReportSystem.Models;
using WeeklyReportSystem.DTOs;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace WeeklyReportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserDto
                {
                    UserID = u.UserID,
                    UserName = u.UserName,
                    UserEmail = u.UserEmail,
                    RoleID = u.RoleID,
                    RoleName = u.Role.RoleName // Assuming Role has a RoleName property
                })
                .ToListAsync();
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                UserEmail = userDto.UserEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password), // Hash the password
                RoleID = userDto.RoleID
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(userDto.UserEmail) ||
                string.IsNullOrWhiteSpace(userDto.Password) ||
                string.IsNullOrWhiteSpace(userDto.UserName))
            {
                return BadRequest("Invalid user data.");
            }

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserEmail == userDto.UserEmail);

            if (existingUser != null)
            {
                return Conflict("User already exists."); // 409 Conflict
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // Create a new user
            var user = new User
            {
                UserName = userDto.UserName,
                UserEmail = userDto.UserEmail,
                PasswordHash = hashedPassword, // Save the hashed password
                RoleID = userDto.RoleID != 0 ? userDto.RoleID : 3 // Set default role if RoleID is not provided
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto loginDto)
        {
            // find by email
            var user = await _context.Users.Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.UserEmail == loginDto.UserEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            // generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("UserID", user.UserID.ToString()),
            new Claim("RoleID", user.RoleID.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token), UserID = user.UserID, RoleID = user.RoleID });
        }


        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDto userDto)
        {
            if (id <= 0 || userDto == null)
            {
                return BadRequest("Invalid user ID or data.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Validate input fields (optional fields like password shouldn't trigger an error if not provided)
            if (string.IsNullOrWhiteSpace(userDto.UserName) || string.IsNullOrWhiteSpace(userDto.UserEmail))
            {
                return BadRequest("UserName and UserEmail cannot be empty.");
            }

            if (userDto.RoleID <= 0)
            {
                return BadRequest("Invalid RoleID.");
            }

            // Update user properties from DTO
            user.UserName = userDto.UserName;
            user.UserEmail = userDto.UserEmail;

            // Only update PasswordHash if a new password is provided
            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password); // Hash the new password
            }

            user.RoleID = userDto.RoleID;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound("User does not exist.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); // User not found
            }

            // Check if there are any related tickets for this user
            var hasTickets = await _context.Tickets.AnyAsync(t => t.UserID == id);
            if (hasTickets)
            {
                return Conflict("Cannot delete user with existing tickets."); // User has related tickets
            }

            // No related tickets, proceed to delete the user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // Deletion successful
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
