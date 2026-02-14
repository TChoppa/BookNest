using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IUserRepository
    {
        public Task<RoleMaster?> GetAllRoles(int roleId);
        public Task<User?> GetUserByEmail(string email);
        public Task<User?> GetUserByUsername(string userName);
        public Task<User?> GetUserByEmailUsername(string username, string Email);
       // public Task<bool> EditPasssword(string usernameEmail, string Password);
        public Task<User?> GetUserByUsernamePassword(string userName, byte[] passwordHash, byte[] passwordSalt);
        public  Task<bool> AddUser(User user);
        public Task<bool> EditPasssword(User editedUser);
    }
}
