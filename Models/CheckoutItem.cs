using System.ComponentModel.DataAnnotations.Schema;

namespace StoreWebApi.Models
{
    public class CheckoutItem
    {
        public int Id { get; set; }

        public int CheckoutId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
