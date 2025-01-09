using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Models;
using System.Security.Cryptography;
using StoreWebApi.Data;

namespace StoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult CreateUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is required");

            string apiKey = GenerateApiKey();
            var user = new User { Username = username, ApiKey = apiKey };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { apiKey });
        }

        private string GenerateApiKey()
        {
            using (var hmac = new HMACSHA256())
            {
                return Convert.ToBase64String(hmac.Key);
            }
        }
    }
}