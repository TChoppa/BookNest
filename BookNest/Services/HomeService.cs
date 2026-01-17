using BookNest.DTO;
using BookNest.Interfaces;
using BookNest.Models;

namespace BookNest.Services
{
    public class HomeService:IHome
    {
        private readonly IUserRepository _repo; 
        public HomeService(IUserRepository repo)
        { 
          _repo = repo ;
        }

        public async Task<bool> Register(RegisterDTO dto)
        {
            try
            {           
                var existingUser = await _repo.GetUserByEmail(dto.Email);
                if (existingUser != null) {
                    return false;
                }
                var user = new User
                {
                    Email = dto.Email,
                    Username = dto.Username,
                    Fk_RoleId = dto.Fk_RoleId,
                    Firstname = dto.Firstname,
                    Lastname = dto.Lastname,
                    Password=dto.Password
                };
                var isSuccess = await _repo.AddUser(user);
           
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        public async Task<bool> Login (LoginDTO dto)
        {
            try
            {
                var isUserExists = await _repo.GetUserByEmailPassword(dto.UserNameEmail, dto.Password);
                if (isUserExists != null)
                {
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }
    }
}
