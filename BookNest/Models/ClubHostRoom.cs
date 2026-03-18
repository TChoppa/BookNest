namespace BookNest.Models
{
    public class ClubHostRoom
    {
        public int Id { get; set; }
        public string Name { get; set; } // Creator
        public string ClubHostCode { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string isActive { get; set; }=string.Empty;

        // Navigation properties
        public virtual ICollection<ClubHostAttachment> Attachments { get; set; } = new List<ClubHostAttachment>();
        public virtual ICollection<ClubHostMember> Members { get; set; } = new List<ClubHostMember>();
    }
}
