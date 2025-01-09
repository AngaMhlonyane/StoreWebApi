using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Data;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("start")]
        public IActionResult StartCheckout([FromBody] List<int> productIds, string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            var checkout = new Checkout { UserId = user.Id, IsCompleted = false };

            foreach (var productId in productIds)
            {
                var product = _context.Products.SingleOrDefault(p => p.Id == productId);
                if (product == null || product.Quantity <= 0)
                    return BadRequest($"Product {productId} is unavailable.");

                checkout.Items.Add(new CheckoutItem { ProductId = productId, Quantity = 1 });
            }

            _context.Checkouts.Add(checkout);
            _context.SaveChanges();

            return Ok(checkout);
        }
    }
}