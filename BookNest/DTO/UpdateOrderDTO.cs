namespace BookNest.DTO
{
    public class UpdateOrderDTO
    {
        public decimal FineAmount { get; set; }  // late fee if applicable
        public bool Action { get; set; }
        public string ReturnStatus { get; set; } = string.Empty;
    }
}
