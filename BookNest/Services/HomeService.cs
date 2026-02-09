using BookNest.DTO;
using BookNest.Enums;
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
        public async Task<RoleMaster?> GetAllRoles(int roleId)
        {
            return await _repo.GetAllRoles(roleId);
        }
        public async Task<bool> Register(RegisterDTO dto)
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
                return await _repo.AddUser(user);
        }

        public async Task<User?> Login (LoginDTO dto)
        {          
            return await _repo.GetUserByUsernamePassword(dto.UserNameEmail, dto.Password);            
        }

        public async Task<PasswordChange> EditPasssword(ForgetPassword dto)
        {
                var user = await _repo.GetUserByEmailUsername(dto.UserNameEmail);
                if (user == null)
                {
                    return PasswordChange.UserNotFound;
                }
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return PasswordChange.PasswordMismatch;
                }
                var isPrevPasswordMatch = await _repo.GetUserByUsernamePassword(dto.UserNameEmail, dto.ConfirmPassword);
                if (isPrevPasswordMatch != null)
                    return PasswordChange.DuplicatePassword;              
                var isPasswordChange = await _repo.EditPasssword(user.Username, dto.NewPassword);
                if (isPasswordChange)
                    return PasswordChange.Success;
                return PasswordChange.UnChanged;
           
        }

        public async Task<User ?> GetUserByUsername(string username)
        {
            return await _repo.GetUserByUsername(username);
        }
    }
}
