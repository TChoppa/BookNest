using BookNest.Models;

namespace BookNest.DTO
{
    public class LoginResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public User UserList { get; set; }
        public string Role { get; set; }
        
    }
}
