using System.ComponentModel.DataAnnotations;

namespace BookNest.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int BookId { get; set; }      
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
        public string Year { get; set; } = string.Empty;
        public bool IsOrdered { get; set; } = false;


    }
}
