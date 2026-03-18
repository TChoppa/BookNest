namespace BookNest.DTO
{
    public class OrderIItemDTO
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ReturnDate { get; set; } // expected return date
        public decimal FineAmount { get; set; } = 0; // late fee if applicable
        public bool Action { get; set; }
        public string ReturnStatus { get; set; } = "NA";

        public string Username { get; set; }= string.Empty;
        public string Year { get; set; } = string.Empty;


    }
}
