using System.ComponentModel.DataAnnotations;

namespace BookNest.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool Action { get; set; }
        public decimal FineAmount { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public string ReturnStatus { get; set; } = string.Empty;


    }
}
