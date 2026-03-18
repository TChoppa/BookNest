namespace BookNest.Models
{
    public class ClubHostAttachment
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }

        // Navigation property
        public virtual ClubHostRoom Room { get; set; }
    }
}
