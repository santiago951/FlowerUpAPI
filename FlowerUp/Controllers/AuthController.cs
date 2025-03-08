using FlowerUp.Models;
using FlowerUpAPI.Data;
using FlowerUpAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlowerUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Registration endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if a user with the given email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User with this email already exists." });

            // Create a new user without a password yet
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email
            };

            // Hash the password using ASP.NET Core's built-in PasswordHasher
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully!" });
        }

        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find the user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials (user not found)." });

            // Verify the hashed password
            var passwordHasher = new PasswordHasher<User>();
            var verifyResult = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (verifyResult == PasswordVerificationResult.Success)
            {
                return Ok(new { message = "Sign in successful!", fullName = user.FullName });
            }
            else
            {
                return Unauthorized(new { message = "Invalid credentials (wrong password)." });
            }
        }
    }
}
