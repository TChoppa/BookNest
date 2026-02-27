namespace BookNest.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
