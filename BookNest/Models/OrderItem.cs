namespace BookNest.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
        public string Year { get; set; } = string.Empty;

    }
}
