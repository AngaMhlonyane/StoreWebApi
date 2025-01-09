using StoreWebApi.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StoreWebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<CheckoutItem> CheckoutItems { get; set; }
    }
}