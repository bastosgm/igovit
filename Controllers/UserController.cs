using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igovit.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace igovit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            var userListResponse = users.Select(user => new UserDto
            {
                Login = user.Login,
                Status = user.Status
            }).ToList();

            return userListResponse;
        }

        // GET: api/user/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetActiveUsers()
        {
            var users = await _context.Users.ToListAsync();

            var userListResponse = users
                .Where(user => user.Status == "active")
                .Select(user => new UserDto
                {
                    Login = user.Login,
                    Status = user.Status
                })
                .ToList();

            return userListResponse;
        }

        // GET: api/user/inactive
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetInactiveUsers()
        {
            var users = await _context.Users.ToListAsync();

            var userListResponse = users
                .Where(user => user.Status == "inactive")
                .Select(user => new UserDto
                {
                    Login = user.Login,
                    Status = user.Status
                })
                .ToList();

            return userListResponse;
        }


        // GET: api/user/:id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound();

            var userResponseModel = new UserDto
            {
                Login = user.Login,
                Status = user.Status
            };

            return userResponseModel;
        }

        // PUT: api/user/:id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User updatedUser)
        {
            if (updatedUser.Id != Guid.Empty || updatedUser.Login != null)
            {
                return BadRequest("Unexpected fields. 'Id' and/or 'Login' cannot be modified.");
            }

            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null) return NotFound();

            if (updatedUser.Password != null) existingUser.Password = updatedUser.Password;

            if (updatedUser.Status != null) {
                System.Console.WriteLine(updatedUser.Status);
                if (updatedUser.Status != "active" && updatedUser.Status != "inactive") {
                    return BadRequest("Unexpected status value. Must be 'active' or 'inactive' only.");
                }
                existingUser.Status = updatedUser.Status;
            }

            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {

            if (user.Login == null) return BadRequest("Login field must be provided.");

            if (user.Password == null) return BadRequest("Password field must be provided.");

            if (user.Status == null) return BadRequest("Status field must be provided.");

            if (
                user.Status != "ativo" &&
                user.Status != "inativo" &&
                user.Status != "active" &&
                user.Status != "inactive")
            {
                return BadRequest("Unexpected status value. Must be 'ativo/active' or 'inativo/inactive'.");
            }

            if (user.Status == "ativo") user.Status = "active";

            if (user.Status == "inativo") user.Status = "inactive";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user.Id, user.Login, user.Status);

            return CreatedAtAction("GetUser", new { id = user.Id }, new { user, token });
        }

        private static string GenerateJwtToken(Guid userId, string login, string status)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, login),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("UserId", userId.ToString()),
                new("Status", status)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678-1234-1234-1234-123456789123")), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // Method commented on because it was not mentioned in the challenge
        //
        // DELETE: api/user/:id
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(Guid id)
        // {
        //     var user = await _context.Users.FindAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Users.Remove(user);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
