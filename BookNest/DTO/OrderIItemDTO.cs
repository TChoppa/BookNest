namespace BookNest.DTO
{
    public class OrderIItemDTO
    {
        public string OrderCode { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ReturnDate { get; set; } // expected return date
        public decimal FineAmount { get; set; } = 0; // late fee if applicable

    }
}
