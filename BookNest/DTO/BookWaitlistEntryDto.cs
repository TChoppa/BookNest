namespace BookNest.DTO
{
    public class BookWaitlistEntryDto
    {
        public int WaitlistId { get; set; }
        public int BookId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? Position { get; set; }
    }
}
