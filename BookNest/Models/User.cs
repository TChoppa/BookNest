using System.ComponentModel.DataAnnotations;

namespace BookNest.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }=string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        public int Fk_RoleId { get; set; }

        public RoleMaster RoleMaster { get; set; }

    }
}
