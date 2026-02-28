using System.ComponentModel.DataAnnotations;

namespace BookNest.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty; // ORD-xxxx
        public string Username { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // Pending / Issued
    }
}
