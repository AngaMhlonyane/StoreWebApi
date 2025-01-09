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
        public IActionResult AddProduct([FromBody] AddProductRequest productRequest, string apiKey)
        {
            // Validate user
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized(new { Message = "Invalid API Key." });

            // Create a new product and map values from the request
            var product = new Product
            {
                Name = productRequest.Name,
                Price = productRequest.Price,
                Quantity = productRequest.Quantity,
                UserId = user.Id // Associate the product with the authenticated user
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Product added successfully.",
                Product = new
                {
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Quantity,
                    Owner = user.Username
                }
            });
        }


        [HttpGet("list")]
        public IActionResult ListProducts()
        {
            var products = _context.Products.Include(p => p.User).ToList();
            return Ok(products);
        }

        [HttpPut("edit/{id}")]
        public IActionResult EditProduct(int id, [FromBody] EditProductRequest updatedProduct, string apiKey)
        {
            // Validate user
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized(new { Message = "Invalid API Key." });

            // Find the product by ID
            var product = _context.Products.SingleOrDefault(p => p.Id == id);

            // Check if the product exists
            if (product == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            // Ensure the product belongs to the current user
            if (product.UserId != user.Id)
                return StatusCode(403, new { Message = "Access Denied: You do not have permission to edit this product." });

            // Update product details
            if (!string.IsNullOrEmpty(updatedProduct.Name))
                product.Name = updatedProduct.Name;

            if (updatedProduct.Price > 0)
                product.Price = updatedProduct.Price;

            if (updatedProduct.Quantity >= 0)
                product.Quantity = updatedProduct.Quantity;

            // Save changes
            _context.SaveChanges();

            return Ok(new
            {
                Message = $"Product with ID {id} updated successfully.",
                Product = new
                {
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Quantity,
                    Owner = user.Username
                }
            });
        }



        [HttpDelete("delete/{id}")]
        public IActionResult DeleteProduct(int id, string apiKey)
        {
            // Validate the user
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            // Find the product by ID
            var product = _context.Products.SingleOrDefault(p => p.Id == id);

            // Check if the product exists
            if (product == null)
                return NotFound($"Product with ID {id} not found.");

            // Ensure the product belongs to the current user
            if (product.UserId != user.Id)
                return StatusCode(403, new { Message = "Access Denied: You do not have permission to delete this product." });

            // Remove the product
            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok(new { message = $"Product with ID {id} deleted successfully." });
        }


    }
}
