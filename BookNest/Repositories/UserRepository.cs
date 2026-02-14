using BookNest.DatabaseContext;
using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;
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
            return _dbContext.users.Where(x => x.Email == email ).FirstOrDefault();
        }
        public async Task<User?> GetUserByUsername(string userName)
        {
            return await _dbContext.users.FirstOrDefaultAsync(x => x.Username == userName);
        }
        public async Task<User?> GetUserByEmailUsername(string email , string userName)
        {
            return _dbContext.users.Where(x => x.Email == email || x.Username == userName).FirstOrDefault();
        }
        public async Task<bool> EditPasssword(User editedUser)
        {
              _dbContext.users.Update(editedUser);
            var result = _dbContext.SaveChanges();
             return result > 0; ;

        }

        public async Task<User?> GetUserByUsernamePassword(string userName, byte[] passwordHash , byte[] passwordSalt)
        {
            return _dbContext.users.Where(x => (x.Username == userName && x.PasswordHash == passwordHash && x.PasswordSalt==passwordSalt)).FirstOrDefault();
        }
        public async Task<bool> AddUser(User user)
        {
             _dbContext.users.Add(user);
             var result = _dbContext.SaveChanges();
            return result>0;
        }
    }
}
