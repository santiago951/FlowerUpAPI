using FlowerUp.Models;
using FlowerUpAPI.Data;
using FlowerUpAPI.Models;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // In a real-world app, you would add error checking and hash the password.
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = model.Password
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully!" });
            }

            return BadRequest(ModelState);
        }
    }
}
