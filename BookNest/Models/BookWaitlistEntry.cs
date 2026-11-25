using System.ComponentModel.DataAnnotations;

namespace BookNest.Models
{
    public class BookWaitlistEntry
    {
        [Key]
        public int WaitlistId { get; set; }
        public int BookId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Status { get; set; } = "Waiting";
        public DateTime JoinedAt { get; set; }
        public DateTime? OfferedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? CollectedAt { get; set; }
    }
}
