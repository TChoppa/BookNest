using System.ComponentModel.DataAnnotations;

namespace BookNest.Models
{
    public class RoleMaster
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }=string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();

    }
}
