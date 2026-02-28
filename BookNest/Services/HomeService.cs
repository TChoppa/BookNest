using BookNest.DTO;
using BookNest.Enums;
using BookNest.Interfaces;
using BookNest.Models;
using System.Security.Cryptography;
using System.Text;

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
            var existingUser = await _repo.GetUserByEmailUsername(dto.Email , dto.Username);
            if (existingUser != null) {
                return false;
            }
            CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);
            var user = new User
                {
                    Email = dto.Email,
                    Username = dto.Username,
                    Fk_RoleId = dto.Fk_RoleId,
                    Firstname = dto.Firstname,
                    Lastname = dto.Lastname,
                    PasswordHash=hash,
                    PasswordSalt=salt
                };
                return await _repo.AddUser(user);
        }

        public async Task<LoginResultDTO> Login (LoginDTO dto)
        {
            var user = await _repo.GetUserByUsername(dto.UserName);
            if (user == null)
                return new LoginResultDTO { Success = false, Message = "User Not Exists" };
            if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                return new LoginResultDTO { Success = false, Message = "Invalid Password" };
            var roles = await _repo.GetAllRoles(user.Fk_RoleId);
            if (roles == null)
                return new LoginResultDTO { Success = false, Message = "Invalid Role" };
            string roleMessage = user.Fk_RoleId switch
            {
                1 => "Login Student successfully",
                2 => "Login Faculty successfully",
                3 => "Login Librarian successfully",
                4 => "Login Admin successfully",
                _ => "Invalid Role"
            };
            return new LoginResultDTO
            {
                Success=true,
                Message=roleMessage,
                UserList=user,
                Role=roles.Name
            };
        }

        public async Task<PasswordChange> EditPasssword(ForgetPassword dto)
        {
                var user = await _repo.GetUserByUsername(dto.UserName);
                if (user == null)
                {
                    return PasswordChange.UserNotFound;
                }
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return PasswordChange.PasswordMismatch;
                }
                CreatePasswordHash(dto.NewPassword, out byte[] hash, out byte[] salt);

                user.PasswordHash = hash;
                user.PasswordSalt = salt;
           
                 var isPasswordChange = await _repo.EditPasssword(user);
                if (isPasswordChange)
                    return PasswordChange.Success;
                return PasswordChange.UnChanged;
           
        }

        public async Task<User ?> GetUserByUsername(string username)
        {
            return await _repo.GetUserByUsername(username);
        }
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }

        public async Task<UserRoleEnum> GetUserRole(string username)
        {
            var user = await _repo.GetUserByUsername(username);
            if (user == null)
                return UserRoleEnum.UserNotFound;
            var roles = await _repo.GetAllRoles(user.Fk_RoleId);
            if (roles == null)
                return UserRoleEnum.RoleNotFound;
            UserRoleEnum roleMessage = roles.RoleId switch
            {
                1 => UserRoleEnum.Student,
                2 => UserRoleEnum.Faculty,
                3 => UserRoleEnum.Librarian,
                4 => UserRoleEnum.Admin,
                _ => UserRoleEnum.InvalidRole
            };

            return roleMessage;

        }

    }
}
