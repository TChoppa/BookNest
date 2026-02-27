namespace BookNest.DTO
{
    public class BookWithCartQtyDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public int AvailableQuantity { get; set; }
        public string Year { get; set; }
        public int CartQty { get; set; }   
    }
}
