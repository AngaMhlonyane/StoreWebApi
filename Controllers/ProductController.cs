using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreWebApi.Data;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public IActionResult AddProduct([FromBody] Product product, string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            product.UserId = user.Id;

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        [HttpGet("list")]
        public IActionResult ListProducts()
        {
            var products = _context.Products.Include(p => p.User).ToList();
            return Ok(products);
        }

        [HttpPut("edit/{id}")]
        public IActionResult EditProduct(int id, [FromBody] Product updatedProduct, string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            var product = _context.Products.SingleOrDefault(p => p.Id == id && p.UserId == user.Id);
            if (product == null)
                return NotFound("Product not found or you don't have permission to edit it.");

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            _context.SaveChanges();

            return Ok(product);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteProduct(int id, string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            var product = _context.Products.SingleOrDefault(p => p.Id == id && p.UserId == user.Id);
            if (product == null)
                return NotFound("Product not found or you don't have permission to delete it.");

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok(new { message = "Product deleted successfully." });
        }
    }
}
