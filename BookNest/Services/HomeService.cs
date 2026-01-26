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

        public async Task<User?> Login (LoginDTO dto)
        {
            try
            {
                var userDetails = await _repo.GetUserByUsernamePassword(dto.UserNameEmail, dto.Password);
                if (userDetails == null)
                {
                    return null;
                }
                return userDetails;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        public async Task<int> EditPasssword(ForgetPassword dto)
        {
            try
            {
                var user = await _repo.GetUserByEmailUsername(dto.UserNameEmail);
                if (user == null)
                {
                    return 2;
                }
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return 1;
                }
                             
                var isPasswordChange = await _repo.EditPasssword(user.Username, dto.NewPassword);
                if (isPasswordChange)
                    return 3;

                return 0 ;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return 0;
            }
        }
    }
}
