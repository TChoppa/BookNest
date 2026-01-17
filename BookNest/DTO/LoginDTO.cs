namespace BookNest.DTO
{
    public class LoginDTO
    {
        public int Id { get; set; }
        public string UserNameEmail { get; set; } = string.Empty;      
        public string Password { get; set; } = string.Empty;
    }
}
