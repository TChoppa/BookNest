namespace BookNest.Models
{
    public class IssuedBook
    {
        public int IssuedBookId { get; set; }
        public int OrderId { get; set; }
        public string Username { get; set; } = string.Empty;

        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int FineAmount { get; set; }
        public bool IsReturned { get; set; }
    }
}
