using BookNest.DTO;
using BookNest.Enums;
using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IHome
    {
        public Task<RoleMaster?> GetAllRoles(int roleId);
        public Task<bool> Register(RegisterDTO dto);
        public Task<LoginResultDTO> Login(LoginDTO dto);
        public Task<PasswordChange> EditPasssword(ForgetPassword dto);
        public Task<User?> GetUserByUsername(string username);

    }
}
