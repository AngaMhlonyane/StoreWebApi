using System.ComponentModel.DataAnnotations;

namespace StoreWebApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string ApiKey { get; set; }
    }
}