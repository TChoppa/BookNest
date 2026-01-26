using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IUserRepository
    {
        
        public Task<User?> GetUserByEmail(string email);
        public Task<User?> GetUserByEmailUsername(string usernameEmail);
        public Task<bool> EditPasssword(string usernameEmail, string Password);
        public Task<User?> GetUserByUsernamePassword(string email, string password);
        public  Task<bool> AddUser(User user);
    }
}
