using System.ComponentModel.DataAnnotations;

namespace BookNest.Models
{
    public class Book
    {
            [Key]
            public int BookId { get; set; }

            public string Username { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;

            public string ImageUrl { get; set; } = string.Empty;

            public int AvailableQuantity { get; set; }

            public string Year { get; set; } = string.Empty;
            public string BranchCode { get; set; } = string.Empty;
            public string BranchName { get; set; } = string.Empty;





    }
}
