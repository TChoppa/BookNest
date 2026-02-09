using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.Models;
using System.Globalization;

namespace BookNest.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RoleMaster?> GetAllRoles(int roleId)
        {
            return _dbContext.RoleMasters.Where(x => x.RoleId == roleId).FirstOrDefault();
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return _dbContext.users.Where(x => x.Email == email).FirstOrDefault();
        }
        public async Task<User?> GetUserByUsername(string userName)
        {
            return _dbContext.users.Where(x => x.Username == userName).FirstOrDefault();
        }
        public async Task<User?> GetUserByEmailUsername(string usernameEmail)
        {
            return _dbContext.users.Where(x => x.Email == usernameEmail || x.Username == usernameEmail).FirstOrDefault();
        }
        public async Task<bool> EditPasssword(string usernameEmail , string Password)
        {
            var user = _dbContext.users.Where(x => (x.Email == usernameEmail || x.Username == usernameEmail) && x.Password != Password).FirstOrDefault();
            user.Password = Password;   
            _dbContext.SaveChanges();
            return true;

        }

        public async Task<User?> GetUserByUsernamePassword(string usernameEmail, string password)
        {
            return _dbContext.users.Where(x => (x.Username == usernameEmail && x.Password == password)).FirstOrDefault();
        }
        public async Task<bool> AddUser(User user)
        {
             _dbContext.users.Add(user);
             _dbContext.SaveChanges();
            return true;
        }
    }
}
