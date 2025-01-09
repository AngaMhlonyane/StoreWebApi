using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // Start a new checkout
        [HttpPost("CheckOut")]
        public IActionResult StartCheckout([FromBody] List<ProductRequest> products, string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            // Ensure only one active checkout per user
            var existingCheckout = _context.Checkouts
                                           .Include(c => c.Items)
                                           .FirstOrDefault(c => c.UserId == user.Id && !c.IsCompleted);
            if (existingCheckout != null)
                return BadRequest("You already have an active checkout.");

            var checkout = new Checkout { UserId = user.Id, IsCompleted = false, Items = new List<CheckoutItem>() };
            decimal totalCost = 0;

            foreach (var request in products)
            {
                var product = _context.Products.SingleOrDefault(p => p.Id == request.ProductId);
                if (product == null)
                    return BadRequest($"Product with ID {request.ProductId} does not exist.");
                if (product.Quantity < request.Quantity)
                    return BadRequest($"Product '{product.Name}' has insufficient stock. Available: {product.Quantity}");

                // Add product to checkout
                checkout.Items.Add(new CheckoutItem { ProductId = product.Id, Quantity = request.Quantity });

                // Calculate total cost for this product
                totalCost += product.Price * request.Quantity;
            }

            _context.Checkouts.Add(checkout);
            _context.SaveChanges();

            // Format total cost in ZAR
            string formattedTotalCost = totalCost.ToString("C", new System.Globalization.CultureInfo("en-ZA"));

            return Ok(new
            {
                checkout.Id,
                TotalCost = formattedTotalCost, // Total cost in ZAR
                Items = checkout.Items.Select(item => new
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ProductName = _context.Products.Single(p => p.Id == item.ProductId).Name,
                    UnitPrice = _context.Products.Single(p => p.Id == item.ProductId).Price.ToString("C", new System.Globalization.CultureInfo("en-ZA")), // Price in ZAR
                    TotalPrice = (_context.Products.Single(p => p.Id == item.ProductId).Price * item.Quantity)
                                 .ToString("C", new System.Globalization.CultureInfo("en-ZA")) // Total price per product in ZAR
                })
            });
        }

        // Complete the checkout
        [HttpPost("complete")]
        public IActionResult CompleteCheckout(string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            var checkout = _context.Checkouts
                                   .Include(c => c.Items)
                                   .FirstOrDefault(c => c.UserId == user.Id && !c.IsCompleted);

            if (checkout == null)
                return BadRequest("No active checkout found.");

            // Deduct product quantities
            foreach (var item in checkout.Items)
            {
                var product = _context.Products.Single(p => p.Id == item.ProductId);
                if (product.Quantity < item.Quantity)
                    return BadRequest($"Insufficient stock for product '{product.Name}'.");

                product.Quantity -= item.Quantity;
            }

            checkout.IsCompleted = true;
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Checkout completed successfully!",
                CheckoutSummary = new
                {
                    checkout.Id,
                    Items = checkout.Items.Select(item => new
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = _context.Products.Single(p => p.Id == item.ProductId).Name,
                        UnitPrice = _context.Products.Single(p => p.Id == item.ProductId).Price
                    }),
                    TotalCost = checkout.Items.Sum(item => _context.Products.Single(p => p.Id == item.ProductId).Price * item.Quantity)
                }
            });
        }

        // Modify the quantity of an item in the checkout
        [HttpPut("update")]
        public IActionResult UpdateCheckoutItem([FromBody] ProductRequest updateRequest, string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            var checkout = _context.Checkouts
                                   .Include(c => c.Items)
                                   .FirstOrDefault(c => c.UserId == user.Id && !c.IsCompleted);
            if (checkout == null)
                return BadRequest("No active checkout found.");

            var item = checkout.Items.SingleOrDefault(i => i.ProductId == updateRequest.ProductId);
            if (item == null)
                return BadRequest("Product not found in your checkout.");

            var product = _context.Products.Single(p => p.Id == updateRequest.ProductId);
            if (product.Quantity < updateRequest.Quantity)
                return BadRequest($"Insufficient stock for product '{product.Name}'. Available: {product.Quantity}");

            item.Quantity = updateRequest.Quantity;
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Item updated successfully.",
                Checkout = checkout.Items.Select(i => new
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    ProductName = _context.Products.Single(p => p.Id == i.ProductId).Name,
                    UnitPrice = _context.Products.Single(p => p.Id == i.ProductId).Price
                })
            });
        }

        // Delete an item from the checkout
        [HttpDelete("remove/{productId}")]
        public IActionResult RemoveCheckoutItem(int productId, string apiKey)
        {
            var user = _context.Users.SingleOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return Unauthorized("Invalid API Key");

            var checkout = _context.Checkouts
                                   .Include(c => c.Items)
                                   .FirstOrDefault(c => c.UserId == user.Id && !c.IsCompleted);
            if (checkout == null)
                return BadRequest("No active checkout found.");

            var item = checkout.Items.SingleOrDefault(i => i.ProductId == productId);
            if (item == null)
                return BadRequest("Product not found in your checkout.");

            checkout.Items.Remove(item);
            _context.SaveChanges();

            return Ok(new { Message = "Item removed successfully." });
        }
    }

    public class ProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
