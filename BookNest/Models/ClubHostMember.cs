namespace BookNest.Models
{
    public class ClubHostMember
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }

        // Navigation property
        public virtual ClubHostRoom Room { get; set; }
    }
}
