namespace StoreWebApi.Models
{
    
        public class AddProductRequest
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
