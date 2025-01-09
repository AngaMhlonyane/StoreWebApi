using System.ComponentModel.DataAnnotations;

namespace StoreWebApi.Models
{
    public class Checkout
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();

        public bool IsCompleted { get; set; }
    }
}