using BookNest.DTO;
using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IHome
    {
        public Task<bool> Register(RegisterDTO dto);
        public Task<User?> Login(LoginDTO dto);
        public Task<int> EditPasssword(ForgetPassword dto);

    }
}
